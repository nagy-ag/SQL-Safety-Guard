"""Report analyzer executable-line/branch gaps from a coverlet Cobertura file."""
import argparse
import collections
import datetime
import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path
from artifact_io import write_generated, new_report_directory

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / "docs/verification/rules"


def summarize(path):
    root = ET.parse(path).getroot()
    files = collections.defaultdict(dict)
    for cls in root.findall(".//class"):
        filename = cls.get("filename", "").replace("\\", "/")
        if not cls.get("name", "").startswith("SqlSafetyGuard.Analyzer."):
            continue
        filename = "src/SqlSafetyGuard.Analyzer/" + filename.split("/")[-1]
        for line in cls.findall("./lines/line"):
            number = int(line.get("number"))
            hits = int(line.get("hits", "0"))
            coverage = line.get("condition-coverage", "")
            match = re.search(r"\((\d+)/(\d+)\)", coverage)
            covered, total = map(int, match.groups()) if match else (0, 0)
            old = files[filename].get(number, (0, 0, 0))
            files[filename][number] = (max(old[0], hits), max(old[1], covered), max(old[2], total))
    if not files:
        raise ValueError("No analyzer files found in coverage report")
    return files


def totals(files):
    lines = [value for entries in files.values() for value in entries.values()]
    return sum(value[0] > 0 for value in lines), len(lines), sum(value[1] for value in lines), sum(value[2] for value in lines)


def compact(numbers):
    groups = []
    for number in sorted(numbers):
        if groups and number == groups[-1][-1] + 1:
            groups[-1].append(number)
        else:
            groups.append([number])
    return ", ".join(str(group[0]) if len(group) == 1 else f"{group[0]}-{group[-1]}" for group in groups) or "None"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("baseline", type=Path)
    parser.add_argument("augmented", type=Path)
    parser.add_argument("--output", required=True, type=Path, help="New directory outside the repository")
    parser.add_argument("--measured-at", required=True, type=datetime.date.fromisoformat)
    parser.add_argument("--source-revision", required=True, help="Revision of the instrumented build, supplied by the caller")
    parser.add_argument("--baseline-label", default="Baseline")
    parser.add_argument("--augmented-label", default="Augmented")
    args = parser.parse_args()
    baseline, augmented = summarize(args.baseline), summarize(args.augmented)
    inventory = json.loads((DEST / "rules.json").read_text(encoding="utf-8"))
    mapping = json.loads((DEST / "rule-test-map.json").read_text(encoding="utf-8"))
    uncovered = [rule["id"] for rule in inventory["rules"] if rule["id"] not in mapping]
    output_directory = new_report_directory(args.output, ROOT)
    metadata = {"measuredAt": args.measured_at.isoformat(), "sourceRevision": args.source_revision,
                "revisionAttribution": "Caller-supplied; not independently established by this report",
                "inputs": [{"name": path.name, "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
                           for path in (args.baseline, args.augmented)]}
    output = ["# Offline analyzer coverage gaps", "",
              f"Measured {args.measured_at.isoformat()}; build revision supplied by caller: `{args.source_revision}`.", "",
              "Analyzer-only Cobertura inputs. See coverage-metadata.json for input hashes. No SQL was executed by this report.", "",
              "| Suite | Lines hit / executable | Line coverage | Branch arms hit / total | Branch coverage |",
              "| --- | ---: | ---: | ---: | ---: |"]
    for name, files in [(args.baseline_label, baseline), (args.augmented_label, augmented)]:
        hit, total, branches, branch_total = totals(files)
        output.append(f"| {name} | {hit}/{total} | {(hit/total if total else 0):.2%} | {branches}/{branch_total} | {(branches/branch_total if branch_total else 0):.2%} |")
    output += ["", "Executable lines come from Cobertura, not every source line. Duplicate class entries for the same file/line are merged by maximum hits/branch counts. Branch counts are instrumentation arms, not semantic rules. Missing/partial branches below report covered/total; Cobertura does not reliably identify which source true/false arm was missed. Generated/compiler branches may be included. Coverage is evidence of execution, not assertions or SQL Server correctness.", "", f"Rule-reference gate: {len(mapping)}/{len(inventory['rules'])} IDs mapped; {len(uncovered)} uncovered (report-only). A named-test association is not a branch hit or an executable-server pass.", "", "## Lines and branch locations", "", "Each column lists **every** unexecuted executable line or incomplete branch location for that suite; ranges are inclusive. Baseline and augmented snapshots remain distinct."]
    for filename in sorted(baseline.keys() | augmented.keys()):
        output += ["", f"### {filename}", "", "| Suite | Never-hit lines | Incomplete branches (line: hit/total) |", "| --- | --- | --- |"]
        for name, files in [(args.baseline_label, baseline), (args.augmented_label, augmented)]:
            entries = files.get(filename, {})
            zeros = compact(number for number, value in entries.items() if value[0] == 0)
            branches = ", ".join(f"{number}: {value[1]}/{value[2]}" for number, value in sorted(entries.items()) if value[2] > value[1]) or "None"
            output.append(f"| {name} | {zeros} | {branches} |")
    output += ["", "## Unmapped rule IDs", "", "Re-run the gate with detailed output for the live list. This snapshot intentionally does not fail the build.", "", "```text", *sorted(uncovered), "```", "", "## Reproduce", "", "See tools/verification/README.md in the source checkout for collection and report commands. Raw coverage files and measured reports stay outside the repository."]
    write_generated(output_directory / "coverage.md", "\n".join(output) + "\n")
    write_generated(output_directory / "coverage-metadata.json", json.dumps(metadata, indent=2) + "\n")
    print(json.dumps({"baseline": totals(baseline), "augmented": totals(augmented), "rules": len(inventory["rules"]), "mapped": len(mapping), "unmapped": len(uncovered)}))


if __name__ == "__main__":
    main()
