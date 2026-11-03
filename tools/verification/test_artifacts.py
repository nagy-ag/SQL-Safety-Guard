import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

from artifact_io import new_report_directory, write_generated


class ArtifactTests(unittest.TestCase):
    def test_failed_replacement_preserves_previous_evidence(self):
        with tempfile.TemporaryDirectory() as directory:
            target = Path(directory) / "report.md"
            target.write_text("previous", encoding="utf-8")
            with patch("artifact_io.os.replace", side_effect=OSError("interrupted")):
                with self.assertRaises(OSError):
                    write_generated(target, "new")
            self.assertEqual("previous", target.read_text(encoding="utf-8"))
            self.assertEqual([target], list(Path(directory).iterdir()))

    def test_report_output_cannot_replace_history_or_an_existing_run(self):
        with tempfile.TemporaryDirectory() as directory:
            repository = Path(directory) / "repository"
            repository.mkdir()
            with self.assertRaises(ValueError):
                new_report_directory(repository / "docs/history/new", repository)
            report = new_report_directory(Path(directory) / "run", repository)
            with self.assertRaises(FileExistsError):
                new_report_directory(report, repository)

    def test_coverage_report_uses_supplied_metadata_and_preserves_existing_run(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            coverage = root / "coverage.xml"
            coverage.write_text('<coverage><class name="SqlSafetyGuard.Analyzer.Example" filename="Example.cs"><lines>'
                                '<line number="1" hits="0" /></lines></class></coverage>', encoding="utf-8")
            output = root / "report"
            command = [sys.executable, "-B", str(Path(__file__).with_name("report_coverage.py")),
                       str(coverage), str(coverage), "--output", str(output), "--measured-at", "2026-11-03",
                       "--source-revision", "fixture-build", "--baseline-label", "Fixture baseline"]
            first = subprocess.run(command, capture_output=True, text=True)
            self.assertEqual(0, first.returncode, first.stderr)
            text = (output / "coverage.md").read_text(encoding="utf-8")
            self.assertIn("Fixture baseline", text)
            self.assertNotIn("824", text)
            metadata = json.loads((output / "coverage-metadata.json").read_text(encoding="utf-8"))
            self.assertEqual("fixture-build", metadata["sourceRevision"])
            self.assertEqual(64, len(metadata["inputs"][0]["sha256"]))
            second = subprocess.run(command, capture_output=True, text=True)
            self.assertNotEqual(0, second.returncode)
            self.assertEqual(text, (output / "coverage.md").read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
