---
name: server-verification
description: Development testing only, on a throwaway server - never production. Use before running or interpreting tests/SqlSafetyGuard.ServerHarness or any verification campaign that executes SQL to check SQL Safety Guard verdicts against what actually changed. Covers server safety, outcome classes, false-green triage and reporting.
---

# Verifying verdicts on a real server

**This is how the analyzer is tested during development.** It deliberately runs writes, so it only ever runs on a throwaway
server. Users of the extension never do this, and the extension itself never executes SQL: on any real or production server it
only reads the query text and shows a verdict before F5.

The harness runs each case on a real SQL Server, snapshots the databases before and after, and compares what really changed
with the analyzer's verdict. Machine-specific details (which server, which databases) are in `AGENTS.local.md` if present.

## Server safety

- Only run against a server you are explicitly allowed to change: a local playground instance or a disposable container.
  Never a production or shared business server, never a database you did not create for this purpose.
- The harness clones disposable database sets from templates and drops them afterwards; never point it at existing user
  databases. Privileged (server-level) cases belong in a disposable container, not the host instance.
- Freeze the analyzer and harness binaries for a long run (hash-checked copy) so results stay attributable; a changed binary
  or input needs a new output folder.

## Outcome classes

| Outcome | Meaning | Action |
|---|---|---|
| FALSE GREEN | Green verdict, but something permanent changed | Highest priority, triage immediately (below) |
| WRONG TARGET | A changed object is not named by any red finding or reachable edge | Fix the finding's target |
| INDIRECT EFFECT | Change reachable through a trigger, view, synonym, temporal table, cascade or sequence | Expected; check it is reported |
| POLICY EFFECT | Allowed by a setting (allow-list, trusted procedure, opt-in) but still writes | Record in policy-questions.md |
| NOISE | Non-green verdict, nothing changed | Acceptable; never "fix" without the ladder in `classifier-change` |
| EXTERNAL CHANGE | Change outside the case's databases (another process or worker) | Replay if the verdict was green |
| OBSERVATION GAP / UNRESOLVED TARGET | The oracle could not see or attribute the effect | Needs a human decision, not a pass |
| NOT EXECUTABLE / CRASH / TIMEOUT | The case did not run cleanly | Not evidence either way |

## Triage of a false green

1. Read the SQL, the verdict, and exactly what changed (`ModifiedObjects`, logical/physical changes, server and tempdb diffs).
2. If the only change is server-level or outside the case's databases, suspect parallel workers: replay with `--workers 1`.
   Only a false green that reproduces alone is real.
3. If real: write a failing analyzer test, fix it with the `classifier-change` checklist, and rerun the case.
4. If it is an opt-in or setting doing what it says, it is a policy item, not a bug: record it in policy-questions.md.

## Run hygiene

- Run privileged lanes first and the large generated lane last, so the most valuable results arrive early.
- Run generated cases as their own principals; replay as a full-access principal only the cases blocked by permissions (and
  green cases whose `CATCH` could swallow a permission error).
- Long runs must be resumable; monitor progress and restart on stalls.

## Reporting

- Put the run report in `docs/history/runs/<date>-<name>.md` with the snapshot header.
- Rewrite `docs/verification/status.md`: bottom line, run table, defects fixed, coverage, open items.
- Never commit raw results or zips.

## Product documentation

See `docs/development.md` for build/debug and implementation details, `docs/user-guide.md` for installation, use and
settings examples, and `docs/verification/status.md` for current evidence. The root README is a short front page.
