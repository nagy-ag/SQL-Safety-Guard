import importlib.util
import itertools
import unittest
import tempfile
from pathlib import Path

spec = importlib.util.spec_from_file_location("campaign", Path(__file__).with_name("generate_campaign.py"))
campaign = importlib.util.module_from_spec(spec)
spec.loader.exec_module(campaign)


class CampaignGeneratorTests(unittest.TestCase):
    def test_rule_ids_are_unique_and_known(self):
        offline, live, manifest = campaign.rule_probes(42)
        for rows in (offline, live):
            self.assertEqual(len(rows), len({row["id"] for row in rows}))
        known = {entry["ruleId"] for entry in manifest}
        self.assertTrue(all(set(row["ruleIds"]) <= known for row in offline + live))

    def test_feature_fixtures_require_real_effect_observers(self):
        spec = importlib.util.spec_from_file_location("features", Path(__file__).with_name("generate_feature_probes.py"))
        features = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(features)
        with tempfile.TemporaryDirectory() as directory:
            assembly = Path(directory) / "WaveClr.dll"
            assembly.write_bytes(b"test-only assembly bytes")
            fixtures = features.generate(assembly)["fixtures"]
        self.assertEqual(15, len(fixtures))
        self.assertEqual(15, len({row["id"] for row in fixtures}))
        by_id = {row["id"]: row for row in fixtures}
        self.assertTrue(by_id["protocol-insert-bulk"]["bulkCopy"])
        self.assertTrue(by_id["service-master-regenerate"]["observeChange"])
        self.assertIn("\nGO\nEXEC dbo.MemoryWrite", by_id["memory-atomic-execute"]["sql"])
        self.assertEqual(1, by_id["memory-atomic-execute"]["afterScalar"])
        self.assertIn("DROP", by_id["external-table-drop"]["sql"])

    def test_covering_arrays_cover_every_interaction(self):
        for strength in (2, 3):
            levels = [3, 2, 4, 2]
            rows = campaign.covering_rows(levels, strength, campaign.random.Random(42))
            expected = set().union(*(campaign.interactions(row, strength) for row in itertools.product(*(range(level) for level in levels))))
            actual = set().union(*(campaign.interactions(row, strength) for row in rows))
            self.assertEqual(expected, actual)

    def test_seeded_company_ground_truth(self):
        rows = campaign.company(42)
        self.assertEqual(rows, campaign.company(42))
        self.assertEqual(24, len(rows))
        for row in rows:
            self.assertGreaterEqual(row["lineCount"], 500)
            writes = row["sql"].count("UPDATE dbo.Counter SET")
            self.assertEqual(int(row["groundTruthPermanentWrite"]), writes)
            if writes:
                self.assertTrue(row["sql"].splitlines()[row["buriedLine"] - 1].startswith("UPDATE dbo.Counter SET"))


if __name__ == "__main__":
    unittest.main()
