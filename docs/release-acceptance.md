# Release 1.0.1 acceptance

Updated **2026-11-05** for the owner-requested **1.0.1 uninstall UI update**. The owner authorized committing/pushing
GitHub `main` and publishing 1.0.1 with a ZIP containing the VSIX. The earlier 1.0.0 release is retained. The package
remains unsigned and supports **SSMS 22.0 and later 22.x releases**, 64-bit, bounded to `[22.0,23.0)`.

## Changes

- Adds **Uninstall SQL Safety Guard...** beside Import and Export under **Tools > Options > SQL Safety Guard**.
- Corrects the command-group hierarchy of the Tools submenu. Its live popup visibility remains unverified.
- Reuses the existing No-default confirmation and instance/profile-specific installer handoff. It does not disable
  the guard, force-close SSMS, elevate, or delete loaded extension files itself.
- Treats the observed installer exit 1002 (not installed) as an already-absent result in the fallback uninstall script,
  matching the install helper; other failures still abort. Offline fixtures cover both supported not-installed codes.
- Adds `SqlSafetyGuard-1.0.1.zip` containing the standalone VSIX, scripts, instructions, legal notices and company signing
  kit. Separate VSIX/scripts/signing-kit downloads remain available; `SHA256SUMS.txt` covers all four assets.
- Keeps GitHub downloads unsigned. The signing kit checks the input and signs a separate company copy with an existing
  local code-signing key; no key is included or created here. Sign CLI 0.9.1-beta.26552.4 help/version checks passed;
  company-key signing, signature/chain/timestamp verification and signed-copy SSMS acceptance remain company-side work.
- Synchronizes manifest, assembly and product versions. Analyzer classification and execution-guard policy are unchanged.

## Published result and evidence

[SQL Safety Guard 1.0.1](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.1) was published stable and Latest
on **2026-11-05 at 09:47:09 UTC**, release ID **435014557**, from clean source
`939ab15ee419daa86a61cab2a7cbd39ace64632a` (annotated tag `v1.0.1`). `main` was fast-forwarded and pushed without force;
the 1.0.0 release/tag were preserved. All five asset digests and fresh downloads match the validated local files.
See the [publication record](history/runs/2026-11-05-release-1.0.1-publication.md) for exact hashes and CI results.

The [November 5 UI record](history/runs/2026-11-05-uninstall-ui.md) identifies the earlier local candidate and measured
evidence. Its Release settings button was visible/enabled in isolated SSMS 22.11.2. No preserved the installation and
started no installer; Yes launched the correct profile-specific installer. Extension files and registration were later
absent. The final installer page/normal interactive exit and Tools popup were not captured. The hidden test installer
was stopped only after files were removed; the normal SSMS process and protected settings/assemblies were preserved.

That candidate passed both builds, 1,628 .NET tests, 32 Release interaction checks and local package validation.
Those are not a fresh execution campaign or a live test of the final 1.0.1 artifact. The final clean-source release gate
passed **1,635 .NET tests, 10 Python tests, 527 current generated rules**, the Release rebuild, 32 interaction checks and
package validation; Debug also rebuilt successfully. Independent archive checks confirm matching nested VSIX/scripts/kits,
correct version/target, unsigned packages, no certificate/key files and working online guide links after extraction.
Two earlier full-gate attempts failed an unchanged three-second timing assertion; it passed alone. The new external
PowerShell fixtures were isolated from parallel analyzer timing tests, after which the full gate passed. Assertions and
analyzer behavior were retained. Actual company-key signing and signed-copy verification were not performed here.

## Retained compatibility limits

Earlier frozen compatibility candidates have focused execution evidence on one representative of every SSMS 22.x
minor through 22.11, with the distinctions and observation gaps in [verification/status.md](verification/status.md).
No new SQL/server campaign, full-theme pass or exhaustive execution-route acceptance is part of this packaging update.
The earlier eleven-row manual report remains user-reported evidence; its retained counters did not independently
corroborate the entire sequence. New release metadata does not turn incomplete observations into passes.

The [1.0.0 publication record](history/runs/2026-11-04-ssms-22-publication.md) preserves its source and checksums.
The final 1.0.1 package is unsigned; company signing remains a separate internal copy.
