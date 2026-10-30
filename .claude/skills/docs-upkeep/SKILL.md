---
name: docs-upkeep
description: Use before adding, moving or updating anything under docs/ in SQL Safety Guard, including after a verification run or a classifier change. Keeps docs/README.md the single list of current documents and history/ for dated snapshots.
---

# Keeping the docs current

The owner wants to see the current state at all times. `docs/README.md` is the start page and lists **only** current documents.

## Rules

1. **Current files** have no date or wave in their name and are updated in place. Every current `.md` outside `docs/history/`
   must be linked from `docs/README.md` (`DocsIndexTests` fails otherwise).
2. **Dated results** (a run report, a wave summary) go to `docs/history/` and start with:
   `> **Snapshot — not maintained.** Current state: [verification/status.md](<relative>/verification/status.md). All current docs: [docs/README.md](<relative>/README.md).`
   Add a row to `docs/history/README.md`, newest first. Never edit a snapshot afterwards; files covered by a `SHA256SUMS.txt`
   must not change at all.
3. **After a verification run:** rewrite `docs/verification/status.md` (date, bottom line, run table, defects fixed, coverage,
   open items) and add the report under `docs/history/runs/`.
4. **After a fixed false green or a new policy question:** update the status table or `docs/verification/policy-questions.md`
   (short list on top, details below).
5. **Generated files** (`docs/statement-verdicts.md`, `docs/verification/rules/*`) are regenerated, never hand-edited; see
   `AGENTS.md` for the commands.
6. Relative links must resolve (`DocsIndexTests`). Keep machine-specific paths and server names out of committed docs.
7. Never commit zips or raw results.

## Product documentation

See `docs/development.md` for build/debug and implementation details, `docs/user-guide.md` for installation, use and
settings examples, and `docs/verification/status.md` for current evidence. The root README is a short front page.
