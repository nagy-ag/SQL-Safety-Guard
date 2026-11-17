> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Automatic filters and circular preview counts

Local, unpublished update on 2026-11-12. Published release 1.0.5 is unchanged.

Six independent display toggles replace All: Writes, Unknown, Safe, Inside, Context and Remote. Risk colors combine;
scope toggles enable their corresponding content. Matching preview operations retain their ancestors without changing
the ancestor's actual risk marker. Empty selections have an explicit empty state. All six enabled shows all content.
Counts, overall warnings and execution analysis remain unfiltered.

Automatic labels switch the whole row from text/counts to circles/counts at narrow widths. The filter row and Analysis
button have a context menu for Automatic, Text and Circles. Text mode raises the minimum width; no mode wraps the row.
Enabled filters survive resizing, label changes and passive reanalysis in the live pane. No persisted settings format changed.

Objects keep their actual risk circle on the left and submitted risk counts on the right. A separate purple circle counts
operations inside deferred code; a gray circle counts remote-preview operations. The body preview row uses its purple/gray
circle on the left and the actual risk breakdown on the right. Individual operations retain their own risk colors. The
trigger example shows red 1 plus purple 2, then a purple body row with red 2 and individual INSERT/DELETE rows. Function,
view, procedure, trigger, stored-command and remote previews retain their navigation and full-container outlines.

## Verification

| Check | Result |
|---|---|
| Locked offline gate | 1,911 .NET tests, 10 Python tests PASS; generated references and 548-rule inventory fresh |
| Builds | Debug and Release PASS; existing DialogChrome threading-analyzer warnings remain |
| Release WPF | 50 pane checks PASS, including independent real ToggleButton events, large counts, minimum width, automatic resizing and forced labels |
| Isolated SSMS 22.11, Light | 36 nonblank captures; 19 automatic-pane, 175 typed-body, 21 nested-body, 24 procedure, 15 dynamic and 38 balanced-pane checks PASS |
| Restoration | Isolated settings restored byte-for-byte; owned SSMS exited cleanly |
| Local installation | All 25 installed payload files match the frozen final Release package; normal-profile settings unchanged; extension cache refreshed |

The new filter-rule test covers all 64 masks, preserving each operation and the original verdict counts. SSMS checks
include all function body forms, unavailable CLR bodies, safe preview matches beneath a red definition, independent
scope toggles and preview-row click/double-click behavior. Existing persistent highlight and adjacent-row outline checks pass.

Early UI attempts stopped on an obsolete assertion prohibiting a body expander and on unintended body tinting during
object selection. The assertion was updated to the approved hierarchy; object selection was corrected to prefer submitted
matches and use preview matches only for retained ancestors. The full final isolated run passed. A tooltip punctuation
cleanup followed the captures; final Debug/Release builds and the final Release WPF checks passed afterward.

This is display-only work: no SQL execution, connection opening, server metadata lookup, new server campaign, physical
input simulation, all-theme matrix or release publication was performed. Existing real-server evidence remains in the
[qualified-trust report](2026-11-12-qualified-trust.md). Raw receipts, screenshots and packages remain outside the repository.

Installed package SHA-256: `BF79A88A422752C0B66E4932CBA3C20D601B27E91C713A67B98038E8B3D752A5`.
