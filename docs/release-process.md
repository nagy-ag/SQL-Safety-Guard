# Release process

GitHub releases are **always unsigned**. Signing is optional and happens only on the owner's company computer,
after downloading the GitHub package; see [vsix-signing.md](vsix-signing.md). No signing keys or certificates belong
in this repository or in GitHub release assets.

## Version numbers

Keep the VSIX manifest (`src/SqlSafetyGuard.Vsix/source.extension.vsixmanifest`, `MAJOR.MINOR.PATCH`),
`src/SqlSafetyGuard.Vsix/Properties/AssemblyInfo.cs` and `src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj`
(`MAJOR.MINOR.PATCH.0`), and the newest `CHANGELOG.md` heading equal. `VersionConsistencyTests` enforces this.

- **Patch** (1.0.1): fixes that change no colour.
- **Minor** (1.1.0): a colour change or new feature. Colour changes have their own changelog section.
- **Major** (2.0.0): changes users must relearn.

Versions must increase for upgrades. A pre-release uses the real numeric version; it has no manifest suffix.
Keep 1.0.0 marked unreleased during preparation. Finish the changelog date when publication is authorized.

## Build the assets

Use Windows PowerShell 5.1, .NET SDK 10, and Visual Studio with extension-development and .NET desktop workloads.
Start with a clean committed working tree, then run from the repository root:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/New-ReleaseAssets.ps1
```

The script reads the manifest version, runs all unit tests, and rebuilds the Release solution with Visual Studio
MSBuild found through vswhere. Use `-MSBuildPath <path>` to choose it explicitly. It then runs the pane, confirmation
and settings interaction scripts in Release. `-SkipUiChecks` skips only those three checks; use that switch only
when recording the omission. Builds use deterministic compilation and neutral repository paths in Release only, including SourceLink metadata.

Before writing assets, it reads every VSIX entry, validates ZIP CRCs, checks the manifest Id/version and SSMS target,
and requires the assemblies, symbols, package registration, `Options/registration.json`, license and third-party
notices. It rejects signatures (`_xmlsignatures/`), private-key files (`.pfx`, `.p12`, `.key`), unsafe entry paths,
and the local user profile path in UTF-8 or UTF-16LE entry data. This checks the built artifact, not just build flags.

The default output is `../release-out/<version>` relative to the repository. Override with
`-OutputDirectory <absolute-path>`; output must be outside the repository in a new or empty directory.
The three files are:

- `SqlSafetyGuard-<version>.vsix` — validated unsigned package.
- `SqlSafetyGuard-<version>-scripts.zip` — `install.ps1`, `uninstall.ps1`, `scripts/SsmsCommon.ps1`, preserving that layout.
- `SHA256SUMS.txt` — SHA-256 hashes of both assets.

The script never signs, tags, uploads or creates a release. Never commit these files or raw run output.

## Publish and try it

1. Complete [release-acceptance.md](release-acceptance.md) and the isolated-profile smoke checks described in
   [development.md](development.md) and the [extension UI skill](../.agents/skills/ssms-extension-ui/SKILL.md).
   Check install, green/yellow/red F5, failure confirmation, settings/import/export, upgrade and uninstall.
   The all-themes pass happens only when the owner asks for it.
   Use [ThemeAcceptanceCapture.ps1](../scripts/ThemeAcceptanceCapture.ps1) with the Debug VSIX installed in Exp
   to capture the pane, real dialog previews, integrated settings and toolbar without input or SQL execution.
   See the extension UI skill for the command, stored theme values and restoration checks.
2. When publication is requested, tag the committed source `v<version>`, push it, and create the GitHub release
   as a **pre-release**, attaching the script's three unsigned assets and the changelog notes.
3. The owner downloads the package and checks its hash. On their company computer they may sign a copy using
   [vsix-signing.md](vsix-signing.md), then try the exact downloaded or signed copy.
4. After the owner's acceptance, the owner changes the GitHub pre-release into a full release and marks it latest.
   The GitHub files remain unsigned; an internally signed copy has its own checksum and distribution record.

A pre-release is a published package marked as still being tried and is not the normal latest release.
A full release is the accepted package offered as the latest version for general use.
