# Release process

How to cut a release. The version lives in four places that `VersionConsistencyTests` keeps equal: the VSIX manifest
(`src/SqlSafetyGuard.Vsix/source.extension.vsixmanifest`, `MAJOR.MINOR.PATCH`), `src/SqlSafetyGuard.Vsix/Properties/AssemblyInfo.cs`
and `src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj` (`MAJOR.MINOR.PATCH.0`), and the newest `CHANGELOG.md` heading.

## Version numbers

- **Patch** (1.0.1): fixes that change no colour.
- **Minor** (1.1.0): any colour change or new feature. Colour changes get their own changelog section.
- **Major** (2.0.0): changes users must relearn.
- The number must always go up: VSIXInstaller only upgrades to a higher version. There are no pre-release suffixes; a pilot build
  is the real number, labelled "pilot" in its release notes.

## Steps

1. Set the version in the four places and finish the `CHANGELOG.md` entry (replace "unreleased" with the date).
2. `dotnet test tests\SqlSafetyGuard.Tests` passes; Release build with Visual Studio MSBuild succeeds.
3. Smoke test in an isolated SSMS profile (see the `ssms-extension-ui` skill): install by double-click, pane, F5 on green /
   yellow / red, confirmation dialog, settings, import/export, upgrade from the previous version, uninstall.
4. For a release users will rely on: update [release-acceptance.md](release-acceptance.md); the all-themes pass happens only when
   the owner declares the release final.
5. Commit, tag `v<version>`, push the tag.
6. Create the GitHub Release `v<version>` with the changelog entry as notes and these assets (never commit them):
   `SqlSafetyGuard-<version>.vsix`, `SqlSafetyGuard-<version>-scripts.zip` (`install.ps1`, `uninstall.ps1`,
   `scripts/SsmsCommon.ps1`) and `SHA256SUMS.txt`.
7. If a code-signing certificate becomes available, sign the `.vsix` before upload.
