# -*- coding: utf-8 -*-
"""Проверки универсальной сборки и навигации на независимых SVG-примерах."""

import json
import os
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path


class PdfNavigationTests(unittest.TestCase):
    """Проверяет переходы, замыкание ссылок и сохранность выходного файла при ошибке."""

    def setUp(self):
        """Создаёт изолированный каталог исходников."""
        self.temporary = tempfile.TemporaryDirectory()
        self.directory = Path(self.temporary.name)
        self.tools = Path(__file__).resolve().parents[1]
        self.svg = '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="200" height="100"><rect x="1" y="1" width="198" height="98" fill="white" stroke="black"/>{}</svg>'

    def tearDown(self):
        """Удаляет временные исходники и результаты теста."""
        self.temporary.cleanup()

    def run_builder(self, inputs, *arguments):
        """Собирает независимый набор через публичный интерфейс командной строки."""
        command = [sys.executable, str(self.tools / 'build_pdf.py'), *[str(path) for path in inputs], '-o', str(self.directory / 'result.pdf'), *arguments]
        return subprocess.run(command, capture_output=True, text=True, encoding='utf-8', env={**os.environ, 'PYTHONIOENCODING': 'utf-8'})

    def test_duplicate_names_and_href_variants(self):
        """Одинаковые имена в разных папках не смешивают адреса страниц."""
        for attribute in ['href', 'xlink:href']:
            with self.subTest(attribute=attribute):
                first = self.directory / 'первый' / 'Node.svg'
                second = self.directory / 'второй' / 'Node.svg'
                first.parent.mkdir(exist_ok=True)
                second.parent.mkdir(exist_ok=True)
                link = f'<a {attribute}="../%D0%B2%D1%82%D0%BE%D1%80%D0%BE%D0%B9/Node.svg"><rect x="10" y="10" width="60" height="30" fill="blue"/></a>'
                first.write_text(self.svg.format(link), encoding='utf-8')
                second.write_text(self.svg.format(''), encoding='utf-8')
                result = self.run_builder([first, second], '--no-markdown', '--offline')
                self.assertEqual(result.returncode, 0, result.stderr)
                manifest = json.loads((self.directory / 'result.manifest.json').read_text(encoding='utf-8'))
                diagrams = [page for page in manifest['PageIndex'] if str(page['Destination']).startswith('diagram-')]
                self.assertEqual(len(set(page['Destination'] for page in diagrams)), 2)
                self.assertEqual(manifest['DiagramLinks'][0]['Destination'], diagrams[1]['Destination'])
                verified = subprocess.run([sys.executable, str(self.tools / 'verify_pdf.py'), str(self.directory / 'result.pdf')], capture_output=True, text=True)
                self.assertEqual(verified.returncode, 0, verified.stderr)

    def test_cyclic_closure(self):
        """Включение связанных схем завершается и не дублирует циклические ссылки."""
        first = self.directory / 'A.svg'
        second = self.directory / 'B.svg'
        first.write_text(self.svg.format('<a href="B.svg"><rect x="10" y="10" width="40" height="20"/></a>'), encoding='utf-8')
        second.write_text(self.svg.format('<a href="A.svg"><rect x="10" y="10" width="40" height="20"/></a>'), encoding='utf-8')
        result = self.run_builder([first], '--include-linked', '--offline')
        self.assertEqual(result.returncode, 0, result.stderr)
        summary = json.loads(result.stdout)
        self.assertEqual(summary['Diagrams'], 2)
        self.assertEqual(summary['Pages'], 4)

    def test_missing_link_preserves_output(self):
        """Ошибка выбора набора не повреждает существующий PDF."""
        first = self.directory / 'A.svg'
        first.write_text(self.svg.format('<a href="Missing.svg"><rect x="10" y="10" width="40" height="20"/></a>'), encoding='utf-8')
        output = self.directory / 'result.pdf'
        output.write_bytes(b'previous output')
        result = self.run_builder([first], '--offline')
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('outside the selected set', result.stderr)
        self.assertEqual(output.read_bytes(), b'previous output')

    def test_nested_transform_and_viewbox(self):
        """Область ссылки учитывает преобразования родителей и смещение viewBox."""
        first = self.directory / 'A.svg'
        first.write_text('<svg xmlns="http://www.w3.org/2000/svg" width="200" height="100" viewBox="10 20 200 100"><g transform="translate(30 40)"><g transform="scale(2)"><a href="A.svg"><rect x="5" y="5" width="15" height="10"/></a></g></g></svg>', encoding='utf-8')
        result = self.run_builder([first], '--offline')
        self.assertEqual(result.returncode, 0, result.stderr)
        manifest = json.loads((self.directory / 'result.manifest.json').read_text(encoding='utf-8'))
        rect = manifest['DiagramLinks'][0]['Rect']
        self.assertAlmostEqual(rect[2] - rect[0], 22.5, places=3)
        self.assertAlmostEqual(rect[3] - rect[1], 15, places=3)
        self.assertAlmostEqual(rect[0], (595.28 - 150) / 2 + 22.5, places=3)

    def test_markdown_fragments_and_long_content(self):
        """Разделы Markdown и длинные сигнатуры остаются доступны по ссылкам."""
        diagram = self.directory / 'A.svg'
        diagram.write_text(self.svg.format('<a href="A.md#контракт"><rect x="10" y="10" width="40" height="20"/></a>'), encoding='utf-8')
        markdown = self.directory / 'A.md'
        markdown.write_text('# Описание\n\n' + 'длинный абзац ' * 1200 + '\n\n## Контракт\n\n```text\nMethod(' + 'IReadOnlyCollection<IModelObject> arguments, ' * 8 + 'int count) : void\n```\n\n[Схема](A.svg)\n', encoding='utf-8')
        result = self.run_builder([diagram], '--offline')
        self.assertEqual(result.returncode, 0, result.stderr)
        manifest = json.loads((self.directory / 'result.manifest.json').read_text(encoding='utf-8'))
        destination = manifest['DiagramLinks'][0]['Destination']
        self.assertGreater(manifest['DestinationPages'][destination], 4)
        verified = subprocess.run([sys.executable, str(self.tools / 'verify_pdf.py'), str(self.directory / 'result.pdf')], capture_output=True, text=True)
        self.assertEqual(verified.returncode, 0, verified.stderr)

    def test_large_table_of_contents(self):
        """Оглавление переносится между страницами при большом числе схем."""
        for index in range(1, 71):
            (self.directory / f'Diagram-{index}.svg').write_text(self.svg.format(''), encoding='utf-8')
        result = self.run_builder([self.directory], '--offline', '--no-markdown')
        self.assertEqual(result.returncode, 0, result.stderr)
        manifest = json.loads((self.directory / 'result.manifest.json').read_text(encoding='utf-8'))
        self.assertEqual(manifest['Diagrams'], 70)
        self.assertGreater(manifest['Pages'], 72)
        pages = [page['Title'] for page in manifest['PageIndex'] if str(page['Destination']).startswith('diagram-')]
        self.assertEqual(pages[:3], ['Diagram-1', 'Diagram-2', 'Diagram-3'])

    def test_external_url(self):
        """Сетевые ссылки сохраняются как внешние URL."""
        first = self.directory / 'A.svg'
        first.write_text(self.svg.format('<a href="https://example.com/reference"><rect x="10" y="10" width="40" height="20"/></a>'), encoding='utf-8')
        result = self.run_builder([first], '--offline')
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(json.loads(result.stdout)['ExternalLinks'], 1)

    def test_offline_requires_current_cache(self):
        """Исходник PlantUML без проверенного SVG не считается готовым к сборке."""
        first = self.directory / 'A.mdpuml'
        first.write_text('@startuml\nclass A\n@enduml\n', encoding='utf-8')
        result = self.run_builder([first], '--offline')
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('No verified SVG cache', result.stderr)

    def test_multpage_markdown_table(self):
        """Таблицы описаний переносятся с повторением строки заголовков."""
        diagram = self.directory / 'A.svg'
        diagram.write_text(self.svg.format(''), encoding='utf-8')
        rows = ['| Тип | Назначение | Схема |', '| --- | --- | --- |']
        rows.extend(f'| Type-{index} | Описание контракта {index} | [Диаграмма](A.svg) |' for index in range(90))
        (self.directory / 'A.md').write_text('# Контракты\n\n' + '\n'.join(rows) + '\n', encoding='utf-8')
        result = self.run_builder([diagram], '--offline')
        self.assertEqual(result.returncode, 0, result.stderr)
        verified = subprocess.run([sys.executable, str(self.tools / 'verify_pdf.py'), str(self.directory / 'result.pdf')], capture_output=True, text=True)
        self.assertEqual(verified.returncode, 0, verified.stderr)

    def test_stale_render_report_is_rejected(self):
        """SVG из отчёта с чужим хешем исходника не попадает в PDF."""
        diagram = self.directory / 'A.mdpuml'
        diagram.write_text('@startuml\nclass A\n@enduml\n', encoding='utf-8')
        svg = self.directory / 'A.svg'
        svg.write_text(self.svg.format(''), encoding='utf-8')
        report = self.directory / 'render-report.json'
        report.write_text(json.dumps([{'File': str(diagram), 'SourceSHA256': 'stale', 'Output': str(svg), 'Error': None}]), encoding='utf-8')
        result = self.run_builder([diagram], '--offline', '--render-report', str(report))
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('No verified SVG cache', result.stderr)

    def test_minimum_font_size_in_pdf(self):
        """Размер текста учитывает преобразование пикселей SVG в пункты PDF."""
        import pdfplumber
        diagram = self.directory / 'A.svg'
        diagram.write_text(self.svg.format('<text x="10" y="50" font-family="Arial" font-size="8">Readable</text>'), encoding='utf-8')
        result = self.run_builder([diagram], '--offline', '--font-size', '11')
        self.assertEqual(result.returncode, 0, result.stderr)
        with pdfplumber.open(self.directory / 'result.pdf') as document:
            chars = [char for char in document.pages[2].chars if 45 < char['top'] < document.pages[2].height - 40]
            self.assertTrue(chars)
            self.assertGreaterEqual(min(char['size'] for char in chars), 10.999)


if __name__ == '__main__':
    unittest.main()
