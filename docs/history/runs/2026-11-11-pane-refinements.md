> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Compact pane refinements

Local, unpublished follow-up to the [compact pane trial](2026-11-11-balanced-pane.md), requested after owner inspection.
The normal-profile candidate retains manifest version 1.0.5. No commit, push or publication.

## Result

- Object rows keep their names, verdict counts and source lines. Whole-target labels appear only on individual
  permanent-write operations. Green temporary/allow-listed operations do not carry these risk labels.
- Removed the explanatory text below the outer and procedure-body filter buttons.
- Horizontal filter rows and a minimum expanded width measured from current labels/counts keep all four buttons on
  one row. Measurement uses current text directly, avoiding stale ContentPresenter layout after counts change.
- Removed the highlight timeout. Object and operation highlights, including inside procedure bodies, persist through
  passive reanalysis. Clicking elsewhere clears them; selecting another row replaces them. Editor interaction, edits,
  execution-selection changes, filter/page changes and pane collapse also clear them. Matching rows retain focus and
  the exact SQL execution selection. The process-local WPF input observer is detached on margin disposal.
- SQL classification, execution scope and guard policy are unchanged.

## Verification

| Check | Result |
|---|---|
| .NET suite | 1,783 PASS |
| Builds | Debug and Release PASS; existing DialogChrome threading warnings remain |
| Release WPF scripts | 38 pane checks, including five-digit counts and temporary/permanent labels; 15 confirmation ledger/dropdown checks PASS |
| Isolated SSMS 22.11, Dark | 36 mixed-verdict checks; 21 editor checks plus refresh; 25 procedure-body checks PASS |
| Persistent highlights | Outer object/operation and body object/operation each survive the former timeout and passive analysis, retain on the chosen row and clear on another click |
| Live minimum width | Post-layout receipt PASS at 244.8 DIP; captured all four complete buttons on one row |
| Visual evidence and cleanup | 31 captures, including minimum width and expanded body; inspected Dark layout; owned SSMS closed cleanly and isolated settings restored byte-for-byte |
| Normal-profile installation | VSIXInstaller and configuration refresh succeeded; 27 installed package members match the frozen VSIX; normal settings unchanged |

The earlier synchronous live-width assertion ran before the host layout settled and did not allow pixel rounding.
The final check runs after the minimum-width capture and allows one DIP of rounding. The separate five-digit Release
check also passes. A previous large-count check exposed stale label measurement; current text measurement fixes it.
The retained offline tooling evidence is the preceding trial's 10 Python tests and fresh 533-rule inventory; those tools
were not changed in this refinement. No SQL execution, new server campaign, physical input or all-theme pass is claimed.
Normal-profile launch and personal acceptance remain for the owner.

## Package identity

Frozen local VSIX SHA-256:

`ba33aa66ffb9ef44ea30f6d67055a26b243fa8aa2964953f4e9035a1530c0909`

The preceding candidate remains available for rollback outside the repository. Raw build/test logs, failed and final
capture receipts, settings backups, the frozen candidate and installation hashes also remain outside the repository.
