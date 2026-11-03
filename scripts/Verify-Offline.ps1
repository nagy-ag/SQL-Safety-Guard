<# Repository verification with no SQL execution, SSMS launch or installation. #>
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
function Assert-NativeSuccess([string]$Step) {
    if ($LASTEXITCODE -ne 0) { throw "$Step failed (exit $LASTEXITCODE)." }
}
Push-Location $repository
try {
    & dotnet restore tests/SqlSafetyGuard.Tests --locked-mode --nologo
    Assert-NativeSuccess 'Locked offline-project restore'
    & dotnet test tests/SqlSafetyGuard.Tests --no-restore --nologo --verbosity minimal
    Assert-NativeSuccess 'Offline tests'
    & python -B tools/verification/generate_inventory.py --check
    Assert-NativeSuccess 'Generated reference freshness'
    # unittest defaults to stderr; use stdout so PowerShell log capture does not manufacture NativeCommandError records.
    & python -B -c "import sys, unittest; suite = unittest.defaultTestLoader.discover('tools/verification', pattern='test_*.py'); result = unittest.TextTestRunner(stream=sys.stdout).run(suite); sys.exit(not result.wasSuccessful())"
    Assert-NativeSuccess 'Offline Python tooling tests'
}
finally { Pop-Location }
