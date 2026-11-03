"""Offline orchestration fixtures: git/build/validation commands are stubbed, never SQL execution."""
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


@unittest.skipUnless(sys.platform == "win32", "Windows PowerShell tooling")
class CampaignToolTests(unittest.TestCase):
    def run_fixture(self, scenario):
        with tempfile.TemporaryDirectory(prefix="SgRun_tooling_") as directory:
            quote = lambda value: str(value).replace("'", "''")
            script = r'''
$ErrorActionPreference = 'Stop'
$fixtureRoot = '__FIXTURE__'
$toolRoot = '__TOOLS__'
$scenario = '__SCENARIO__'
$global:SgFixtureBuilds = 0
$global:SgFixtureValidations = 0
function git {
    $global:LASTEXITCODE = 0
    if ($args[0] -eq 'rev-parse') { return ('a' * 40) }
    if ($args[0] -eq 'status') {
        if ($scenario -eq 'dirty') { return ' M source.cs' }
        return
    }
    throw 'Unexpected fixture git command.'
}
function dotnet {
    $global:LASTEXITCODE = 0
    if ($args[0] -eq '--version') { return '10.0.403-fixture' }
    if ($args[0] -eq 'build') {
        $global:SgFixtureBuilds++
        if ($scenario -eq 'build-failure') { $global:LASTEXITCODE = 1; return 'fixture compiler failure' }
        $destination = $args[[Array]::IndexOf($args, '--output') + 1]
        [IO.Directory]::CreateDirectory($destination) | Out-Null
        $name = if ($args[1] -like '*ServerHarness*') { 'SqlSafetyGuard.ServerHarness.dll' } else { 'SqlSafetyGuard.Analyzer.dll' }
        [IO.File]::WriteAllText((Join-Path $destination $name), 'fixture bytes')
        return 'fixture build'
    }
    if ($args -contains '--campaign-validate' -or $args -contains '--validate-coverage') {
        $global:SgFixtureValidations++
        return
    }
    throw 'Fixture forbids every execution command; only build and validation are allowed.'
}
$caught = ''
try {
    & (Join-Path $toolRoot 'Freeze-Campaign.ps1') -OutputRoot $fixtureRoot | Out-Null
    $frozen = Join-Path $fixtureRoot ('bin\' + ('a' * 40))
    if ($scenario -eq 'tampered') {
        [IO.File]::WriteAllText((Join-Path $frozen 'analyzer\SqlSafetyGuard.Analyzer.dll'), 'changed')
    }
    & (Join-Path $toolRoot 'Run-Campaign.ps1') -Frozen $frozen -Output (Join-Path $fixtureRoot 'validation') -ValidateOnly | Out-Null
} catch { $caught = $_.Exception.Message }
$manifest = Get-ChildItem -LiteralPath $fixtureRoot -Filter manifest.json -Recurse | Select-Object -First 1
$buildCommands = 0
if ($manifest) { $buildCommands = @((Get-Content $manifest.FullName -Raw | ConvertFrom-Json).Build.Commands).Count }
[pscustomobject]@{Caught=$caught;Builds=$global:SgFixtureBuilds;Validations=$global:SgFixtureValidations;Manifest=[bool]$manifest;BuildCommands=$buildCommands} | ConvertTo-Json -Compress
'''
            script = script.replace("__FIXTURE__", quote(directory)).replace("__TOOLS__", quote(ROOT / "tools/verification"))
            script = script.replace("__SCENARIO__", scenario)
            process = subprocess.run(["powershell.exe", "-NoProfile", "-Command", script],
                                     capture_output=True, text=True, timeout=60)
            self.assertEqual(0, process.returncode, process.stderr)
            return json.loads(process.stdout)

    def test_dirty_source_and_failed_build_cannot_produce_a_runnable_bundle(self):
        for scenario in ("dirty", "build-failure"):
            with self.subTest(scenario=scenario):
                result = self.run_fixture(scenario)
                self.assertTrue(result["Caught"])
                self.assertFalse(result["Manifest"])
                self.assertEqual(0, result["Validations"])

    def test_campaign_entry_points_preserve_validate_only_and_build_provenance(self):
        result = self.run_fixture("success")
        self.assertEqual("", result["Caught"])
        self.assertEqual(2, result["Builds"])
        self.assertEqual(2, result["BuildCommands"])
        self.assertEqual(5, result["Validations"])

    def test_changed_binary_is_rejected_before_validation_or_execution(self):
        result = self.run_fixture("tampered")
        self.assertIn("Frozen file changed", result["Caught"])
        self.assertEqual(0, result["Validations"])
