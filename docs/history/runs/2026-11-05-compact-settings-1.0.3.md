> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Compact Options 1.0.3 verification

## Scope

The owner chose the compact design C, requested preservation of settings behavior, an up-to-date repository format
reference, main publication, release 1.0.3 and a changelog update. Six existing settings are now aligned beside their
explanations, with the existing editor/transfer/uninstall commands below. Storage keys, types, defaults, migration,
parsers, analyzer verdicts and execution policy are unchanged. The user guide remains the format reference and now
contains copyable list/rule examples and a seven-field rule table.

The native SSMS registration schema exposes a GEL content factory rather than a full layout template. The compact
form uses that host; SSMS adds its own Preview caption. These controls are live and persist immediately. A scoped
focus handler permits keyboard entry inside this form; no other host preview or global focus behavior is changed.
The generated duplicate property controls are hidden, while the category includes each setting name as a search keyword.
Classic-mode property-grid fallback remains available.

## Results

| Check | Result |
|---|---|
| Offline suite | PASS: 1,635 .NET tests, 10 Python tests, 527 current rules |
| Debug rebuild and Release build | PASS |
| Release WPF interaction checks | PASS: 26 pane, 15 confirmation, 14 settings checks |
| Registration against installed SSMS schema | PASS |
| Real isolated native settings check | PASS: 12 assertions (focus routing and native readback for all six controls) |
| Light-theme capture | PASS: 13 surfaces; compact page visually inspected |
| Capture shutdown and settings restoration | PASS: clean exit; original settings bytes restored |
| Final committed-source assets and GitHub publication | Pending; recorded separately after publication |

The native check used the pre-commit Debug candidate DLL SHA-256
`d210e611af9a75f027ed1a9cec3b86169b050cd7a0e078018f5c2bef5633aef8`.
Its installed DLL hash matched the local candidate. Raw logs, receipts and captures are retained outside the repository.
The capture profile's before/after settings SHA-256 was
`27307919e64630c81feb920f868a37d7307b4c4fbfc3ef66fc30a1dd985e9be5`.

An incremental Debug build retained an older VSIX manifest; it was rejected as the final candidate. A capture launched
before that installer/configuration sequence settled timed out with zero captures, closed its owned session, and restored
settings successfully. The final candidate was rebuilt from scratch, checked as version 1.0.3 with an identical installed
DLL, then captured only after installation/configuration completed. No test assertion or product safety rule was relaxed.

## Limits

No SQL was executed, and no SQL connection was opened. This is settings/UI evidence, not a new F5/server campaign,
exact Release-artifact execution acceptance, full-theme pass, all-version assessment, uninstall acceptance or company
signing check. Those prior evidence limits remain in the current verification status. The original isolated installation
is restored as part of publication cleanup; that final restoration is recorded with the published assets.
