> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Inline procedure and dynamic SQL operations

Local, unpublished implementation of the owner-selected Inline operations design, using
`samples/04-compact-pane.sql` for acceptance. The package retains manifest version 1.0.5.

## Result

- A procedure object retains separate **Definition** and **Call** operations. Expanding Definition opens a flat,
  source-ordered **Visible body · preview** list, including repeated operations. The extra body expander, nested object
  tree and second filter bar are removed. Each preview row opens one short reason.
- Body counts describe deferred potential effects. They do not enter submission counts, the header or F5 decisions.
  The global display filter applies to submitted operations; an open preview remains complete. A visible definition
  does not resolve or prove a separate procedure call, which retains its own verdict.
- Definition navigation highlights its header line instead of the whole body. Single clicks navigate/highlight without
  expanding or changing execution selection. Double clicks and arrows toggle details. Matching expansion and highlights
  survive passive analysis; another interaction clears highlights as before.
- Decoded dynamic SQL has one container per outer EXEC, with all analyzed operations directly below it. Separate EXECs
  do not merge even when they affect the same table. Unavailable runtime text remains yellow without invented contents.
  An ordinal keeps identical decoded operations independently expandable despite their shared source span.
- Shared editor spans are painted once at the worst visible verdict: a green decoded child cannot cover red siblings in
  All view. Filtering to Safe highlights only visible green operations while preserving the container's full risk/counts.
  Only adornments are combined; operation rows and counts are never deduplicated.
- Existing statement-only risk labels, minimum filter-row width, shell theme resources, analyzer classifications,
  execution scope and guard policy are retained. Production remains text-only.

## Verification

| Check | Result |
|---|---|
| Offline gate | 1,784 .NET tests, 10 Python tests and fresh generated references/533-rule inventory PASS |
| Builds | Final Debug and Release PASS; existing DialogChrome threading warnings remain |
| Final Release WPF scripts | 38 pane interaction and 15 confirmation ledger/dropdown checks PASS |
| Final isolated SSMS 22.11, Light | 23 inline-body checks, exact compact-sample Definition/Call separation and ten dynamic-block checks PASS |
| Shared-span colours | Real adornment assertions verify red for a mixed decoded block in All and green for its visible Safe operation |
| Persistence | Procedure object/body-operation and dynamic block/operation highlights survive passive analysis and the former timeout, then clear on another click |
| Earlier functional run | 36 mixed-verdict checks, 21 editor checks plus cleared-refresh and minimum-width receipt PASS; the final paint-only fix was then checked by the final isolated run |
| Capture cleanup | Final run produced 24 captures; owned SSMS exited cleanly and isolated settings were restored byte-for-byte |
| Normal-profile installation | VSIXInstaller and configuration refresh PASS; all 25 installed payload files match the frozen final VSIX; normal settings unchanged |

The first offline run, concurrent with SSMS startup, hit the existing cancellation test's three-second deadline.
The complete offline gate passed when rerun after SSMS closed. The first capture lane requested Dark but rendered Light;
the final, explicitly Light lane is the theme evidence. No Dark or all-theme coverage, physical input, SQL execution,
new server campaign or personal normal-profile acceptance is claimed.

## Package identity

Final locally installed VSIX SHA-256:

`ac15e7485eb51e3906c55a8a4d1681000cf148194cf854057d6054c15e565e21`

Raw logs, earlier candidates, capture receipts, settings backups, installation hashes and the frozen final package
remain outside the repository. Source changes remain local; no commit, push or publication was requested.
