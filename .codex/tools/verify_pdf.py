#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Проверяет PDF, собранный build_pdf.py, по независимому чтению его структуры."""

import argparse
import hashlib
import json
import re
import subprocess
from pathlib import Path


class PdfVerifier:
    """Проверяет страницы, адреса ссылок, размещение текста и полноту сигнатур."""

    def __init__(self, options):
        self.options = options

    def verify(self):
        """Читает готовый PDF и сопоставляет его с манифестом сборщика."""
        import pdfplumber
        from pypdf import PdfReader
        manifest_path = self.options.manifest or self.options.pdf.with_suffix('.manifest.json')
        manifest = json.loads(manifest_path.read_text(encoding='utf-8-sig'))
        reader = PdfReader(self.options.pdf)
        if len(reader.pages) != manifest['Pages']:
            raise ValueError('PDF page count does not match the manifest.')
        digest = hashlib.sha256(self.options.pdf.read_bytes()).hexdigest()
        if digest != manifest['SHA256']:
            raise ValueError('PDF SHA-256 does not match the manifest.')
        for source in manifest['Sources']:
            path = Path(source['Path'])
            text = path.read_text(encoding='utf-8-sig')
            actual = hashlib.sha256(text.encode('utf-8')).hexdigest()
            if actual != source['SHA256']:
                raise ValueError(f'Source changed since the PDF was built: {path}')
        page_refs = {page.indirect_reference.idnum: index for index, page in enumerate(reader.pages, 1)}
        expected_pages = manifest['DestinationPages']
        internal = external = verified = images = 0
        for index, page in enumerate(reader.pages, 1):
            annotations = [ref.get_object() for ref in page.get('/Annots', [])]
            for expected in manifest['DiagramLinks']:
                if expected['Page'] != index:
                    continue
                matches = [annotation for annotation in annotations if annotation.get('/Subtype') == '/Link' and all(abs(float(annotation['/Rect'][i]) - expected['Rect'][i]) < .002 for i in range(4))]
                correct = []
                for annotation in matches:
                    if 'Destination' in expected:
                        target = annotation.get('/Dest')
                        if target and page_refs.get(target[0].idnum) == expected_pages[expected['Destination']]:
                            correct.append(annotation)
                    elif str(annotation.get('/A', {}).get('/URI')) == expected['URL']:
                        correct.append(annotation)
                if not correct:
                    raise ValueError(f'Wrong diagram link destination on page {index}: {expected["URL"]}')
                verified += 1
            for annotation in annotations:
                if annotation.get('/Subtype') != '/Link':
                    continue
                rect = [float(value) for value in annotation['/Rect']]
                if not (0 <= rect[0] < rect[2] <= float(page.mediabox.width) and 0 <= rect[1] < rect[3] <= float(page.mediabox.height)):
                    raise ValueError(f'Link rectangle outside page {index}: {rect}')
                destination = annotation.get('/Dest')
                if destination:
                    if destination[0].idnum not in page_refs:
                        raise ValueError(f'Invalid internal link on page {index}.')
                    internal += 1
                elif annotation.get('/A', {}).get('/S') == '/URI':
                    external += 1
                else:
                    raise ValueError(f'Unsupported PDF link action on page {index}.')
            resources = page.get('/Resources', {}).get('/XObject', {})
            images += sum(value.get_object().get('/Subtype') == '/Image' for value in resources.values())
        if internal != manifest['InternalLinks'] or external != manifest['ExternalLinks']:
            raise ValueError('PDF link count does not match the manifest.')
        if images and not self.options.allow_images:
            raise ValueError('PDF contains raster images; use --allow-images for SVGs with embedded images.')
        body_text = []
        with pdfplumber.open(self.options.pdf) as document:
            for index, page in enumerate(document.pages, 1):
                text = page.extract_text() or ''
                if '\ufffd' in text:
                    raise ValueError(f'Invalid Unicode replacement character on page {index}.')
                for char in page.chars:
                    if not (-.5 <= char['x0'] <= char['x1'] <= page.width + .5 and -.5 <= char['top'] <= char['bottom'] <= page.height + .5):
                        raise ValueError(f'Text outside page {index}: {char["text"]}')
                body = page.crop((0, 45, page.width, page.height - 40))
                body_text.append(body.extract_text() or '')
        compact = re.sub(r'\s+', '', ''.join(body_text))
        for signature in manifest['SignatureLines']:
            expected = re.sub(r'\s+', '', signature['Text'])
            if expected not in compact:
                raise ValueError(f'Signature missing from PDF: {signature["Source"]}: {signature["Text"]}')
        result = {'PDF': str(self.options.pdf), 'Pages': len(reader.pages), 'InternalLinks': internal, 'ExternalLinks': external, 'DiagramLinksVerified': verified, 'SignatureLinesVerified': len(manifest['SignatureLines']), 'VectorDiagrams': images == 0, 'TextWithinPages': True}
        if self.options.render_dir:
            self.render_pages(reader, result)
        output = self.options.pdf.with_suffix('.verification.json')
        output.write_text(json.dumps(result, ensure_ascii=False, indent=2), encoding='utf-8')
        return result

    def render_pages(self, reader, result):
        """Рендерит PDF через Poppler и создаёт обзорные листы для визуальной проверки."""
        from PIL import Image, ImageDraw
        directory = self.options.render_dir
        directory.mkdir(parents=True, exist_ok=True)
        prefix = directory / self.options.pdf.stem
        command = [self.options.pdftoppm, '-scale-to', '1800', '-png', str(self.options.pdf), str(prefix)]
        subprocess.run(command, check=True, capture_output=True)
        images = sorted(directory.glob(self.options.pdf.stem + '-*.png'))
        if len(images) != len(reader.pages):
            raise ValueError('Rendered image count does not match the PDF page count.')
        sheets = []
        for start in range(0, len(images), 12):
            sheet = Image.new('RGB', (1360, 1710), '#edf1f5')
            draw = ImageDraw.Draw(sheet)
            for offset, path in enumerate(images[start:start + 12]):
                with Image.open(path) as opened:
                    image = opened.convert('RGB')
                image.thumbnail((320, 530))
                x, y = (offset % 4) * 340 + 10, (offset // 4) * 570 + 28
                sheet.paste(image, (x + (320 - image.width) // 2, y))
                draw.text((x, y - 23), str(start + offset + 1), fill='#17507a')
            output = directory / f'{self.options.pdf.stem}-contact-{start // 12 + 1}.png'
            sheet.save(output)
            sheets.append(str(output))
        result['ContactSheets'] = sheets


def main():
    """Запускает проверку выбранного PDF без привязки к проекту Model."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('pdf', type=Path)
    parser.add_argument('--manifest', type=Path)
    parser.add_argument('--render-dir', type=Path, help='Render pages and contact sheets for visual QA.')
    parser.add_argument('--pdftoppm', default='pdftoppm', help='Poppler executable path.')
    parser.add_argument('--allow-images', action='store_true')
    options = parser.parse_args()
    try:
        result = PdfVerifier(options).verify()
        print(json.dumps(result, ensure_ascii=False))
    except (OSError, ValueError, ImportError, subprocess.CalledProcessError) as error:
        parser.exit(1, f'PDF verification failed: {error}\n')


if __name__ == '__main__':
    main()
