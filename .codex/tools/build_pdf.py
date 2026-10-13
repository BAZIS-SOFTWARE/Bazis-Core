#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Собирает векторные диаграммы PlantUML/SVG и документацию в PDF с навигацией."""

import argparse
import base64
import glob
import hashlib
import json
import math
import os
import re
import sys
import urllib.parse
import urllib.request
import zlib
from datetime import date
from pathlib import Path
from xml.sax.saxutils import escape


class DiagramSources:
    """Выбирает исходники и разрешает ссылки относительно исходного документа."""

    def __init__(self, options):
        self.options = options
        self.diagrams = {}
        self.documents = {}

    def collect(self):
        """Собирает заданный набор и, при необходимости, замыкание его ссылок."""
        for argument in self.options.inputs:
            matches = glob.glob(argument, recursive=True)
            if not matches:
                raise ValueError(f"Input does not exist or glob has no matches: {argument}")
            for match in sorted(matches, key=self.natural_key):
                path = Path(match).resolve()
                candidates = [path]
                if path.is_dir():
                    candidates = sorted(path.iterdir(), key=self.natural_key)
                for candidate in candidates:
                    if candidate.is_file() and candidate.suffix.lower() in ('.mdpuml', '.puml', '.svg'):
                        self.add_diagram(candidate)
        if not self.diagrams:
            raise ValueError('No active PlantUML or SVG diagrams selected.')
        selected = list(self.diagrams)
        pending = list(self.diagrams)
        checked = set()
        while pending:
            origin = pending.pop(0)
            if origin in checked:
                continue
            checked.add(origin)
            content = self.read_text(origin)
            targets = re.findall(r'\[\[([^\s\]]+)', content)
            if origin.suffix.lower() == '.svg':
                from lxml import etree
                svg = etree.parse(str(origin))
                targets = [node.get('href') or node.get('{http://www.w3.org/1999/xlink}href') for node in svg.iter() if node.tag.split('}')[-1] == 'a']
            if origin.suffix.lower() == '.md':
                targets = re.findall(r'\[[^\]]+\]\(([^)]+)\)', content)
            if not self.options.no_markdown and origin in self.diagrams:
                sibling = origin.with_suffix('.md')
                if sibling.exists() and sibling not in self.documents:
                    self.documents[sibling] = self.read_text(sibling)
                    pending.append(sibling)
            for url in targets:
                if not url:
                    continue
                reference = self.local_path(origin, url)
                if reference is None:
                    continue
                related_document = any(reference.with_suffix(suffix) in self.diagrams for suffix in ('.mdpuml', '.puml', '.svg'))
                if reference.suffix.lower() == '.md' and not self.options.no_markdown and reference.exists() and (origin in self.diagrams or related_document):
                    if reference not in self.documents:
                        self.documents[reference] = self.read_text(reference)
                        pending.append(reference)
                elif self.options.include_linked and reference.suffix.lower() in ('.mdpuml', '.puml', '.svg'):
                    if reference not in self.diagrams:
                        self.add_diagram(reference)
                        if reference in self.diagrams:
                            pending.append(reference)
        parents = list(dict.fromkeys(path.parent for path in selected))
        linked = [path for path in self.diagrams if path not in selected]
        linked.sort(key=lambda path: [parents.index(path.parent) if path.parent in parents else len(parents), *self.natural_key(path)])
        self.diagrams = {path: self.diagrams[path] for path in selected + linked}
        ordered_documents = [path.with_suffix('.md') for path in self.diagrams if path.with_suffix('.md') in self.documents]
        ordered_documents.extend(path for path in self.documents if path not in ordered_documents)
        self.documents = {path: self.documents[path] for path in ordered_documents}
        return self

    def add_diagram(self, path):
        """Добавляет уникальную схему; архивные схемы исключает."""
        path = path.resolve()
        source = self.read_text(path)
        if re.search(r'^caption\s+АРХИВ', source, re.M | re.I):
            return
        self.diagrams.setdefault(path, source)

    def read_text(self, path):
        """Читает UTF-8, включая файлы с BOM."""
        return path.read_text(encoding='utf-8-sig')

    def natural_key(self, value):
        """Обеспечивает числовой порядок поддиаграмм."""
        parts = re.split(r'(\d+)', str(value).lower())
        return [int(part) if part.isdigit() else part for part in parts]

    def local_path(self, origin, url):
        """Возвращает путь локальной ссылки; сетевые URL оставляет внешними."""
        parsed = urllib.parse.urlsplit(url)
        if parsed.scheme or parsed.netloc:
            return None
        decoded = urllib.parse.unquote(parsed.path)
        return (origin.parent / decoded).resolve() if decoded else origin.resolve()


class DiagramRenderer:
    """Получает актуальный SVG с сервера PlantUML или из проверенного кэша."""

    def __init__(self, options):
        self.options = options
        self.cache = options.cache_dir or options.output.parent / '.pdf-cache'
        self.reports = {}
        if options.render_report:
            records = json.loads(options.render_report.read_text(encoding='utf-8-sig'))
            for record in records:
                path = (options.root / record['File']).resolve()
                self.reports[path] = record

    def render(self, path, source):
        """Возвращает SVG и проверяет его актуальность относительно исходника."""
        if path.suffix.lower() == '.svg':
            return path
        digest = hashlib.sha256(source.encode('utf-8')).hexdigest()
        record = self.reports.get(path)
        if record and record.get('SourceSHA256') == digest and not record.get('Error'):
            svg = Path(record['Output'])
            if not svg.is_absolute():
                svg = self.options.render_report.parent / svg
            if svg.exists():
                return svg
        identity = hashlib.sha256(str(path).encode('utf-8')).hexdigest()[:16]
        target = self.cache / f'{identity}-{digest}.svg'
        if target.exists():
            return target
        if self.options.offline:
            raise ValueError(f'No verified SVG cache for {path}. Render online or supply --render-report.')
        packed = zlib.compress(source.encode('utf-8'))[2:-4]
        encoded = base64.b64encode(packed).decode('ascii')
        alphabet = str.maketrans('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/', '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_')
        encoded = encoded.translate(alphabet).rstrip('=')
        request = urllib.request.Request(self.options.server.rstrip('/') + '/svg/' + encoded)
        with urllib.request.urlopen(request, timeout=self.options.timeout) as response:
            data = response.read()
            error = response.headers.get('X-PlantUML-Diagram-Error')
        if error:
            raise ValueError(f'PlantUML failed for {path}: {error}')
        from lxml import etree
        root = etree.fromstring(data)
        text = ' '.join(root.itertext())
        if root.tag.split('}')[-1] != 'svg' or 'Syntax Error' in text or 'An error has occurred' in text:
            raise ValueError(f'Invalid PlantUML SVG for {path}.')
        self.cache.mkdir(parents=True, exist_ok=True)
        target.write_bytes(data)
        return target


class PdfBuilder:
    """Формирует страницы и переносит ссылки из исходных документов в PDF."""

    def __init__(self, options, sources):
        self.options = options
        self.sources = sources
        self.pages = []
        self.links = []
        self.signatures = []
        self.destinations = {}
        self.headings = {}
        self.bookmark_pages = {}
        self.canvas = None
        self.page = 0
        self.width = 595.28
        self.height = 841.89
        self.cursor = 0
        self.date = date.today().isoformat()
        for path in sources.diagrams:
            digest = hashlib.sha256(str(path).encode('utf-8')).hexdigest()[:20]
            self.destinations[path] = f'diagram-{digest}'
        for path, text in sources.documents.items():
            digest = hashlib.sha256(str(path).encode('utf-8')).hexdigest()[:20]
            destination = f'document-{digest}'
            self.destinations[path] = destination
            slugs = {}
            for index, match in enumerate(re.finditer(r'^#{1,6}\s+(.+)$', text, re.M)):
                slug = re.sub(r'[^\w\- ]', '', match.group(1).strip().lower()).replace(' ', '-')
                count = slugs.get(slug, 0)
                slugs[slug] = count + 1
                if count:
                    slug += f'-{count}'
                self.headings[path, slug] = f'{destination}-heading-{index}'

    def build(self):
        """Готовит все данные, сохраняет PDF и проверяет внутренние переходы."""
        self.prepare_fonts()
        renderer = DiagramRenderer(self.options)
        diagrams = []
        for path, source in self.sources.diagrams.items():
            svg = renderer.render(path, source)
            diagram = self.load_diagram(path, svg)
            diagrams.append(diagram)
        # Проверяем ссылки до открытия выходного файла.
        for diagram in diagrams:
            for link in diagram['Links']:
                link['Target'] = self.resolve_link(diagram['Path'], link['URL'], strict=True)
        from reportlab.pdfgen import canvas
        self.options.output.parent.mkdir(parents=True, exist_ok=True)
        temporary = self.options.output.with_suffix('.building.pdf')
        self.canvas = canvas.Canvas(str(temporary), pagesize=(self.width, self.height), pageCompression=1)
        self.canvas.setTitle(self.options.title)
        self.canvas.setAuthor('BazisCore')
        self.canvas.setSubject('Диаграммы классов и контракты с внутренней навигацией')
        self.canvas.setViewerPreference('DisplayDocTitle', 'true')
        self.canvas.showOutline()
        try:
            self.write_front_matter(diagrams)
            for diagram in diagrams:
                self.write_diagram(diagram)
            for path, text in self.sources.documents.items():
                self.write_document(path, text)
            self.canvas.save()
            summary = self.check_pdf(temporary)
            temporary.replace(self.options.output)
            manifest = self.options.manifest or self.options.output.with_suffix('.manifest.json')
            manifest.parent.mkdir(parents=True, exist_ok=True)
            manifest.write_text(json.dumps(summary, ensure_ascii=False, indent=2), encoding='utf-8')
            return summary
        except Exception:
            temporary.unlink(missing_ok=True)
            raise

    def prepare_fonts(self):
        """Регистрирует шрифты с кириллицей для текста и векторных диаграмм."""
        from reportlab.pdfbase import pdfmetrics
        from reportlab.pdfbase.ttfonts import TTFont
        from reportlab.lib.styles import ParagraphStyle
        from reportlab.lib.colors import HexColor
        directory = self.options.font_dir
        for name, filename in [('Arial', 'arial.ttf'), ('Arial-Bold', 'arialbd.ttf'), ('Arial-Italic', 'ariali.ttf'), ('Arial-BoldItalic', 'arialbi.ttf'), ('Consolas', 'consola.ttf')]:
            path = directory / filename
            if not path.exists():
                raise ValueError(f'Font not found: {path}. Set --font-dir.')
            pdfmetrics.registerFont(TTFont(name, str(path)))
        pdfmetrics.registerFontFamily('Arial', normal='Arial', bold='Arial-Bold', italic='Arial-Italic', boldItalic='Arial-BoldItalic')
        self.styles = {
            'body': ParagraphStyle('body', fontName='Arial', fontSize=10, leading=15, textColor=HexColor('#1d2936'), splitLongWords=True),
            'small': ParagraphStyle('small', fontName='Arial', fontSize=9, leading=13, textColor=HexColor('#536272')),
            'heading': ParagraphStyle('heading', fontName='Arial-Bold', fontSize=14, leading=18, textColor=HexColor('#17507a')),
        }

    def load_diagram(self, path, svg):
        """Получает векторную схему и области ссылок с учётом всех SVG-преобразований."""
        from lxml import etree
        from svglib.svglib import SvgRenderer
        from reportlab.graphics.transform import mmult, transformPoint

        class LinkedSvgRenderer(SvgRenderer):
            """Сохраняет связи между SVG-ссылками и отрисованными группами."""

            def renderA(renderer, node):
                """Регистрирует область гиперссылки при обычном рендеринге SVG."""
                group = super().renderA(node)
                url = node.getAttribute('href') or node.getAttribute('{http://www.w3.org/1999/xlink}href')
                if url:
                    renderer.pdf_anchors[id(group)] = url
                return group

        svg_renderer = LinkedSvgRenderer(str(svg))
        svg_renderer.pdf_anchors = {}
        parser = etree.XMLParser(resolve_entities=False, no_network=True)
        root = etree.parse(str(svg), parser).getroot()
        drawing = svg_renderer.render(root)
        if drawing.width <= 0 or drawing.height <= 0:
            raise ValueError(f'Empty SVG: {svg}')
        anchors = []
        fonts = []
        pending = [{'Shape': drawing, 'Matrix': (1, 0, 0, 1, 0, 0)}]
        while pending:
            entry = pending.pop()
            shape, matrix = entry['Shape'], entry['Matrix']
            url = svg_renderer.pdf_anchors.get(id(shape))
            if url:
                bounds = shape.getBounds()
                if bounds is None:
                    raise ValueError(f'Empty SVG hyperlink: {path}: {url}')
                corners = [transformPoint(matrix, point) for point in [(bounds[0], bounds[1]), (bounds[0], bounds[3]), (bounds[2], bounds[1]), (bounds[2], bounds[3])]]
                rect = [min(point[0] for point in corners), min(point[1] for point in corners), max(point[0] for point in corners), max(point[1] for point in corners)]
                anchors.append({'URL': url, 'Bounds': rect})
            if getattr(shape, 'fontSize', None):
                font_size = shape.fontSize * math.hypot(matrix[2], matrix[3])
                if font_size > 0:
                    fonts.append(font_size)
            if hasattr(shape, 'contents'):
                child_matrix = mmult(matrix, shape.transform)
                for child in reversed(shape.contents):
                    pending.append({'Shape': child, 'Matrix': child_matrix})
        scale = self.options.font_size / min(fonts) if fonts else 1
        return {'Path': path, 'Drawing': drawing, 'Links': anchors, 'Scale': scale, 'SHA256': hashlib.sha256(self.sources.diagrams[path].encode('utf-8')).hexdigest()}

    def resolve_link(self, origin, url, strict=False):
        """Преобразует локальную ссылку в именованную страницу или раздел PDF."""
        reference = self.sources.local_path(origin, url)
        if reference is None:
            return {'URL': url}
        fragment = urllib.parse.unquote(urllib.parse.urlsplit(url).fragment)
        if fragment and (reference, fragment) in self.headings:
            return {'Destination': self.headings[reference, fragment]}
        if reference in self.destinations:
            return {'Destination': self.destinations[reference]}
        if self.options.no_markdown and reference.suffix.lower() == '.md':
            for suffix in ('.mdpuml', '.puml', '.svg'):
                diagram = reference.with_suffix(suffix)
                if diagram in self.destinations:
                    return {'Destination': self.destinations[diagram]}
        if strict:
            raise ValueError(f'Link target is outside the selected set: {origin} -> {url}. Add the target or use --include-linked.')
        return None

    def start_page(self, title, destination=None, width=595.28, height=841.89):
        """Создаёт страницу с закладкой, заголовком и полями для навигации."""
        from reportlab.pdfbase import pdfmetrics
        if max(width, height) > 14400:
            raise ValueError('Diagram exceeds the PDF page size limit. Split it into subdiagrams.')
        self.width, self.height = width, height
        self.page += 1
        self.canvas.setPageSize((width, height))
        self.canvas.setFillColorRGB(.09, .31, .48)
        self.canvas.setFont('Arial-Bold', 10)
        label = self.options.title
        while pdfmetrics.stringWidth(label, 'Arial-Bold', 10) > width - 72:
            label = label[:-2] + '…'
        self.canvas.drawString(36, height - 28, label)
        self.canvas.setStrokeColorRGB(.85, .89, .93)
        self.canvas.line(36, height - 39, width - 36, height - 39)
        if destination:
            self.canvas.bookmarkPage(destination)
            self.canvas.addOutlineEntry(title, destination, level=0)
            self.bookmark_pages[destination] = self.page
        self.cursor = height - 64
        self.pages.append({'Page': self.page, 'Title': title, 'Destination': destination, 'Width': width, 'Height': height})

    def finish_page(self):
        """Завершает страницу, добавляя возврат к оглавлению и номер страницы."""
        self.canvas.setStrokeColorRGB(.85, .89, .93)
        self.canvas.line(36, 37, self.width - 36, 37)
        self.canvas.setFont('Arial', 8)
        self.canvas.setFillColorRGB(.09, .31, .48)
        self.canvas.drawString(36, 22, 'Оглавление')
        self.canvas.linkRect('', 'contents', (36, 19, 93, 32), relative=0, thickness=0)
        self.canvas.drawString(110, 22, 'Обзор')
        self.canvas.linkRect('', 'overview', (110, 19, 145, 32), relative=0, thickness=0)
        self.canvas.drawRightString(self.width - 36, 22, f'{self.date} | {self.page}')
        self.canvas.showPage()

    def paragraph(self, text, style='body', gap=8, destination=None):
        """Размещает абзац и при необходимости переносит его на следующие страницы."""
        from reportlab.platypus import Paragraph
        item = Paragraph(text, self.styles[style])
        available_width = self.width - 72
        _, required = item.wrap(available_width, self.height)
        if required + gap > self.cursor - 52:
            if self.cursor < self.height - 64:
                self.finish_page()
                self.start_page('Продолжение')
            _, required = item.wrap(self.width - 72, self.height)
        if destination:
            self.canvas.bookmarkPage(destination)
            self.bookmark_pages[destination] = self.page
        pending = [item]
        while pending:
            current = pending.pop(0)
            _, required = current.wrap(self.width - 72, self.height)
            available = self.cursor - 52
            if required > available:
                pieces = current.split(self.width - 72, available)
                if len(pieces) < 2:
                    raise ValueError('Paragraph cannot fit on a PDF page.')
                current = pieces[0]
                pending[0:0] = pieces[1:]
                _, required = current.wrap(self.width - 72, available)
            current.drawOn(self.canvas, 36, self.cursor - required)
            self.cursor -= required + gap
            if pending:
                self.finish_page()
                self.start_page('Продолжение')

    def write_front_matter(self, diagrams):
        """Создаёт обзор и оглавление произвольного размера."""
        self.start_page('Обзор', 'overview')
        self.paragraph(escape(self.options.title), 'heading', 20)
        self.paragraph(f'Редакция: {self.date}. Диаграмм: {len(diagrams)}. Документов: {len(self.sources.documents)}.')
        self.paragraph('Диаграммы сохранены векторными. Ссылки на включённые схемы и описания открывают соответствующие страницы этого PDF. Оглавление и закладки позволяют перейти к любому разделу.')
        self.paragraph('Размеры страниц схем подобраны по содержимому для сохранения читаемости текста. Внешние сетевые ссылки, если они есть в исходниках, сохранены как URL.', 'small')
        self.finish_page()
        self.start_page('Оглавление', 'contents')
        self.paragraph('Диаграммы', 'heading', 12)
        for diagram in diagrams:
            path = diagram['Path']
            label = f'{path.stem} - {path.parent.parent.name}'
            self.paragraph(f'<a href="#{self.destinations[path]}" color="#17507a">{escape(label)}</a>', gap=6)
        if self.sources.documents:
            self.paragraph('Описания и полные сигнатуры', 'heading', 12)
            for path in self.sources.documents:
                self.paragraph(f'<a href="#{self.destinations[path]}" color="#17507a">{escape(path.stem)} - описание</a>', gap=6)
        self.finish_page()

    def write_diagram(self, diagram):
        """Рисует SVG и создаёт кликабельные области поверх векторной схемы."""
        from reportlab.graphics import renderPDF
        drawing, scale = diagram['Drawing'], diagram['Scale']
        draw_width, draw_height = drawing.width * scale, drawing.height * scale
        width, height = max(595.28, draw_width + 72), max(841.89, draw_height + 110)
        path = diagram['Path']
        self.start_page(path.stem, self.destinations[path], width, height)
        x, y = (width - draw_width) / 2, height - 64 - draw_height
        self.canvas.saveState()
        self.canvas.translate(x, y)
        self.canvas.scale(scale, scale)
        renderPDF.draw(drawing, self.canvas, 0, 0)
        self.canvas.restoreState()
        for link in diagram['Links']:
            bounds = link['Bounds']
            rect = [x + bounds[0] * scale, y + bounds[1] * scale, x + bounds[2] * scale, y + bounds[3] * scale]
            if not (0 <= rect[0] < rect[2] <= width and 0 <= rect[1] < rect[3] <= height):
                raise ValueError(f'SVG hyperlink lies outside its PDF page: {path}: {link["URL"]}: {rect}')
            target = link['Target']
            if 'Destination' in target:
                self.canvas.linkRect('', target['Destination'], rect, relative=0, thickness=0)
            else:
                self.canvas.linkURL(target['URL'], rect, relative=0, thickness=0)
            self.links.append({'Source': str(path), 'Page': self.page, 'URL': link['URL'], 'Rect': rect, **target})
        self.finish_page()

    def inline_markdown(self, origin, text):
        """Преобразует ссылки и выделения Markdown, сохраняя безопасный текст."""
        tokens = []
        for match in re.finditer(r'\[([^\]]+)\]\(([^)]+)\)', text):
            label, url = match.groups()
            target = self.resolve_link(origin, url)
            formatted = escape(label)
            if target:
                href = '#' + target['Destination'] if 'Destination' in target else target['URL']
                formatted = f'<a href="{escape(href, {chr(34): "&quot;"})}" color="#17507a">{formatted}</a>'
            tokens.append(formatted)
        index = iter(range(len(tokens)))
        text = re.sub(r'\[([^\]]+)\]\(([^)]+)\)', lambda match: f'PDFLINKTOKEN{next(index)}END', text)
        text = escape(text)
        text = re.sub(r'`([^`]+)`', r'<font name="Consolas">\1</font>', text)
        text = re.sub(r'\*\*([^*]+)\*\*', r'<b>\1</b>', text)
        for number, token in enumerate(tokens):
            text = text.replace(f'PDFLINKTOKEN{number}END', token)
        return text

    def write_document(self, path, text):
        """Печатает соседний Markdown, включая полные сигнатуры и ссылки на схемы."""
        from reportlab.pdfbase import pdfmetrics
        self.start_page(path.stem + ' - описание', self.destinations[path])
        code = False
        paragraph = []
        table = []
        slugs = {}
        for line in text.splitlines() + ['']:
            heading = re.match(r'^#{1,6}\s+(.+)$', line)
            table_line = not code and line.strip().startswith('|') and line.strip().endswith('|')
            if table and not table_line:
                self.write_table(path, table)
                table = []
            if not code and (not line.strip() or heading or line.startswith('```') or table_line) and paragraph:
                content = self.inline_markdown(path, ' '.join(paragraph))
                self.paragraph(content)
                paragraph = []
            if line.startswith('```'):
                code = not code
                if not code:
                    self.cursor -= 10
                continue
            if table_line:
                cells = line.strip().strip('|').split('|')
                if not all(re.fullmatch(r'\s*:?-+:?\s*', cell) for cell in cells):
                    table.append([cell.strip() for cell in cells])
                continue
            if code:
                if line.strip():
                    self.signatures.append({'Source': str(path), 'Text': line.strip()})
                remaining = line or ' '
                while remaining:
                    part = remaining
                    while pdfmetrics.stringWidth(part, 'Consolas', 8.5) > self.width - 72:
                        part = part[:-1]
                    if len(part) < len(remaining) and ' ' in part:
                        boundary = part.rfind(' ')
                        if boundary > 0:
                            part = part[:boundary + 1]
                    if not part:
                        raise ValueError('Code line cannot fit on the page.')
                    if self.cursor - 12 < 52:
                        self.finish_page()
                        self.start_page(path.stem + ' - продолжение')
                    self.canvas.setFillColorRGB(.11, .16, .21)
                    self.canvas.setFont('Consolas', 8.5)
                    self.canvas.drawString(36, self.cursor - 9, part)
                    self.cursor -= 12
                    remaining = remaining[len(part):]
                continue
            if heading:
                title = heading.group(1)
                slug = re.sub(r'[^\w\- ]', '', title.strip().lower()).replace(' ', '-')
                count = slugs.get(slug, 0)
                slugs[slug] = count + 1
                if count:
                    slug += f'-{count}'
                destination = self.headings[path, slug]
                content = self.inline_markdown(path, title)
                self.paragraph(content, 'heading', 10, destination)
            elif line.strip():
                paragraph.append(line)
        if code:
            raise ValueError(f'Unclosed Markdown code block: {path}')
        self.finish_page()

    def write_table(self, path, rows):
        """Размещает Markdown-таблицу с повторением заголовка на новых страницах."""
        from reportlab.platypus import LongTable, Paragraph, TableStyle
        from reportlab.lib.colors import HexColor
        columns = max(len(row) for row in rows)
        content = []
        for row in rows:
            cells = row + [''] * (columns - len(row))
            content.append([Paragraph(self.inline_markdown(path, cell), self.styles['small']) for cell in cells])
        widths = [(self.width - 72) / columns] * columns
        if columns == 3:
            widths = [(self.width - 72) * proportion for proportion in (.24, .24, .52)]
        table = LongTable(content, colWidths=widths, repeatRows=1)
        table.setStyle(TableStyle([('BACKGROUND', (0, 0), (-1, 0), HexColor('#edf1f5')), ('GRID', (0, 0), (-1, -1), .3, HexColor('#dae3ec')), ('VALIGN', (0, 0), (-1, -1), 'TOP'), ('TOPPADDING', (0, 0), (-1, -1), 5), ('BOTTOMPADDING', (0, 0), (-1, -1), 5)]))
        pending = [table]
        while pending:
            current = pending.pop(0)
            _, required = current.wrap(self.width - 72, self.height)
            available = self.cursor - 52
            if required > available:
                pieces = current.split(self.width - 72, available)
                if len(pieces) < 2:
                    if self.cursor < self.height - 64:
                        pending.insert(0, current)
                        self.finish_page()
                        self.start_page(path.stem + ' - продолжение таблицы')
                        continue
                    raise ValueError(f'Markdown table row cannot fit on a PDF page: {path}')
                current = pieces[0]
                pending[0:0] = pieces[1:]
                _, required = current.wrap(self.width - 72, available)
            current.drawOn(self.canvas, 36, self.cursor - required)
            self.cursor -= required + 10
            if pending:
                self.finish_page()
                self.start_page(path.stem + ' - продолжение таблицы')

    def check_pdf(self, path):
        """Проверяет адреса PDF-ссылок перед публикацией результата."""
        from pypdf import PdfReader
        reader = PdfReader(path)
        references = {page.indirect_reference.idnum: index for index, page in enumerate(reader.pages, 1)}
        internal = external = 0
        for page in reader.pages:
            for reference in page.get('/Annots', []):
                annotation = reference.get_object()
                if annotation.get('/Subtype') != '/Link':
                    continue
                destination = annotation.get('/Dest')
                if destination:
                    if destination[0].idnum not in references:
                        raise ValueError('PDF contains an invalid internal destination.')
                    internal += 1
                elif annotation.get('/A', {}).get('/S') == '/URI':
                    external += 1
                else:
                    raise ValueError('PDF contains an unsupported link action.')
        all_sources = {**self.sources.diagrams, **self.sources.documents}
        return {'PDF': str(self.options.output), 'Pages': len(reader.pages), 'Diagrams': len(self.sources.diagrams), 'Documents': len(self.sources.documents), 'InternalLinks': internal, 'ExternalLinks': external, 'Bytes': path.stat().st_size, 'SHA256': hashlib.sha256(path.read_bytes()).hexdigest(), 'PageIndex': self.pages, 'DestinationPages': self.bookmark_pages, 'DiagramLinks': self.links, 'SignatureLines': self.signatures, 'Sources': [{'Path': str(source), 'SHA256': hashlib.sha256(text.encode('utf-8')).hexdigest()} for source, text in all_sources.items()]}


def main():
    """Разбирает параметры командной строки и запускает сборку выбранного набора."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('inputs', nargs='+', help='Files, folders or globs: .mdpuml, .puml, .svg; folders are non-recursive.')
    parser.add_argument('-o', '--output', required=True, type=Path, help='Output PDF path.')
    parser.add_argument('--title', default='Диаграммы классов', help='PDF title.')
    parser.add_argument('--include-linked', action='store_true', help='Recursively include referenced diagrams.')
    parser.add_argument('--no-markdown', action='store_true', help='Omit Markdown; map links to sibling diagram pages.')
    parser.add_argument('--server', default='http://192.168.11.202:8080', help='PlantUML server URL.')
    parser.add_argument('--timeout', default=45, type=float)
    parser.add_argument('--offline', action='store_true', help='Use SVG files and verified render cache only.')
    parser.add_argument('--cache-dir', type=Path)
    parser.add_argument('--render-report', type=Path, help='Existing renderer JSON report, validated by source SHA-256.')
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2], help='Root for relative paths in render reports.')
    parser.add_argument('--font-dir', type=Path, default=Path(os.environ.get('WINDIR', 'C:/Windows')) / 'Fonts')
    parser.add_argument('--font-size', type=float, default=10, help='Minimum diagram text size in PDF points.')
    parser.add_argument('--manifest', type=Path, help='Default: output filename with .manifest.json suffix.')
    options = parser.parse_args()
    options.output = options.output.resolve()
    options.root = options.root.resolve()
    if options.font_size <= 0 or options.timeout <= 0:
        parser.error('--font-size and --timeout must be positive.')
    if options.output.suffix.lower() != '.pdf':
        parser.error('--output must have the .pdf extension.')
    try:
        sources = DiagramSources(options).collect()
        builder = PdfBuilder(options, sources)
        summary = builder.build()
        print(json.dumps({key: value for key, value in summary.items() if key not in ('PageIndex', 'DestinationPages', 'DiagramLinks', 'SignatureLines', 'Sources')}, ensure_ascii=False))
    except (OSError, ValueError, ImportError) as error:
        parser.exit(1, f'PDF build failed: {error}\n')


if __name__ == '__main__':
    main()
