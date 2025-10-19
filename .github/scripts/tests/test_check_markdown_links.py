import subprocess
import sys
import tempfile
from pathlib import Path
import textwrap
import unittest

SCRIPT_PATH = Path(__file__).resolve().parents[1] / "check_markdown_links.py"


class MarkdownLinkCheckerTests(unittest.TestCase):
    def run_checker(self, files):
        with tempfile.TemporaryDirectory() as tmpdir:
            tmp_path = Path(tmpdir)
            docs_dir = tmp_path / "docs"
            docs_dir.mkdir()

            for relative, content in files.items():
                file_path = docs_dir / relative
                file_path.parent.mkdir(parents=True, exist_ok=True)
                if isinstance(content, str):
                    file_path.write_text(textwrap.dedent(content), encoding="utf-8")
                else:
                    file_path.write_bytes(content)

            result = subprocess.run(
                [sys.executable, str(SCRIPT_PATH), str(docs_dir)],
                cwd=tmp_path,
                capture_output=True,
                text=True,
            )
            return result

    def test_links_with_symbols_and_spaces_pass(self):
        result = self.run_checker(
            {
                "README.md": """
                # Test
                [Doc【企画】.md](./Doc【企画】.md)
                [Doc With Space](<./Doc With Space.md>)
                [Doc With Space](./Doc With Space.md)
                [Doc！＠＆＜＞.md](./Doc！＠＆＜＞.md)
                [TEST-->HOGE.md](./TEST-->HOGE.md)
                """,
                "Doc【企画】.md": "# ok\n",
                "Doc With Space.md": "# ok\n",
                "Doc！＠＆＜＞.md": "# ok\n",
                "TEST-->HOGE.md": "# ok\n",
            }
        )
        self.assertEqual(result.returncode, 0, msg=result.stdout + result.stderr)
        self.assertIn("Checked", result.stdout)

    def test_missing_file_with_symbols_is_reported(self):
        result = self.run_checker(
            {
                "README.md": """
                # Test
                [Doc](./Doc【企画】.md)
                [Doc With Space](<./Doc With Space.md>)
                """,
            }
        )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Doc【企画】.md", result.stdout)
        self.assertIn("Doc With Space.md", result.stdout)

    def test_display_text_must_match_target_file(self):
        result = self.run_checker(
            {
                "README.md": """
                # Test
                [HOGE](./FUGA.md)
                """,
                "FUGA.md": "# ok\n",
            }
        )
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("display text 'HOGE'", result.stdout)


if __name__ == "__main__":
    unittest.main()
