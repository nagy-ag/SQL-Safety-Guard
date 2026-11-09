---
name: docs-upkeep
description: Use before adding, moving or updating anything under docs/ in SQL Safety Guard, including after a verification run or a classifier change. Keeps docs/README.md the single list of current documents and history/ for dated snapshots.
---

# Keeping the docs current

The owner wants to see the current state at all times. `docs/README.md` is the start page and lists **only** current documents.

## Rules

1. **Current files** have no date or wave in their name, live in the folder for their audience (`user/`, `it/`, `dev/`,
   `safety/`; only `README.md` and `status.md` sit directly in `docs/`) and are updated in place. Every current `.md`
   outside `docs/history/` must be linked from `docs/README.md` (`DocsIndexTests` fails otherwise). A page kept at an old
   path only so that old links work starts with `# Moved:` and links to the new page; it needs no index entry.
2. **Dated results** (a run report, a wave summary) go to `docs/history/` and start with:
   `> **Snapshot — not maintained.** Current state: [status.md](<relative>/status.md). All current docs: [docs/README.md](<relative>/README.md).`
   Add a row to `docs/history/README.md`, newest first. Never edit a snapshot afterwards; files covered by a `SHA256SUMS.txt`
   must not change at all. Byte-preserved archives may retain their original header when their archive README explains the snapshot and a checksum manifest protects the original file; DocsIndexTests validates those hashes.
3. **`docs/status.md` describes now only:** At a glance, latest release, open limits, where the evidence is. After a
   verification run, add the report under `docs/history/runs/` and update those sections; never append history to it.
   After a release, write `docs/history/releases/<version>.md`, add its row to `docs/history/releases/README.md`, and
   point status at it.
4. **After a fixed false green or a new policy question:** update "At a glance" in `docs/status.md` or `docs/safety/policy-questions.md`
   (short list on top, details below).
5. **Generated files** (`docs/user/statement-verdicts.md`, `docs/verification/rules/*`) are regenerated, never hand-edited; see
   `AGENTS.md` for the commands.
6. Relative links must resolve (`DocsIndexTests`). Keep machine-specific paths and server names out of committed docs.
7. Never commit zips or raw results.

## Product documentation

See `docs/dev/development.md` for build/debug and implementation details; `docs/user/install.md`, `docs/user/user-guide.md` and
`docs/user/settings.md` for installation, use and settings examples; and `docs/status.md` for current
evidence. The root README is the product description.
