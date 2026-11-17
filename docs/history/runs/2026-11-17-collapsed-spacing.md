> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Separation in the collapsed color summary

Local, unpublished UI polish on 2026-11-17; published release 1.0.5 is unchanged.

The collapsed strip uses a 14-DIP gap between its visible red/yellow/green categories and its visible
purple/blue/gray categories, retaining 6-DIP spacing within each group. The extra gap is placed before the
first visible code/context category only when a risk category is also present. Missing or empty groups do
not reserve space. Existing circles, counts, hover meanings, filtering and the single expand action are preserved.

## Verification

- Debug and Release builds PASS; existing DialogChrome threading warnings remain.
- Locked offline suite: 1,913 .NET and 10 Python tests PASS; generated references and the 548-rule inventory fresh.
- Existing Release WPF pane suite: 64 checks PASS.
- Isolated SSMS 22.11, verified Dark: 28 captures; 43 collapsed-summary, 19 automatic-pane, 175 typed-body,
  21 nested-body and 15 context/gutter/hover checks PASS. The actual collapsed strip was visually inspected.
- Isolated settings restored byte-for-byte and owned SSMS closed cleanly.
- Local Release installation verified against all 25 payload files; normal-profile settings unchanged and
  extension cache refreshed.

No SQL execution, analyzer/guard changes, all-theme campaign, commit, push or publication was performed.
Raw evidence and frozen packages remain outside the repository.

Installed package SHA-256: `C01AC65DAF81BCD8A596C1E2447CB77B1EB5EC2C048D4D1A23B77DBABF484862`.
