# Verification contract

This is the campaign's desired safety contract, not a claim that every current rule meets it. Analyze and execute exactly the same expanded text, connection context, settings and initial session state. Classification is not an execution permission check.

## Verdict promises

- **ReadOnly (green):** no intentional persistent change at all, and no unapproved session state left behind. Engine-internal incidental bookkeeping (plan cache, automatic statistics, telemetry) must be reported separately, not silently treated as a user write.
- **TempOnly (green):** only this session's local `#temp` and `@table` objects may change; no persistent, remote, shared-global-temp or unrelated session effects. Invoked code does not inherit permission to mutate elsewhere.
- **AllowListed (green):** writes authorized only to resolved targets matched by the effective allow list. Other effects, triggers, cascades and session residue still need accounting. A trusted-procedure declaration is an administrator assertion, not proof of read-only behavior.
- **Unknown (yellow):** no safety promise; confirmation required under yellow-and-red warning policy. Missing evidence is not evidence of safety.
- **RealWrite (red):** operation capable of a non-authorized persistent or external change; confirmation required under an enabled red warning policy. Zero affected rows or execution failure does not invalidate this capability classification.

Current opt-ins and classifications (including `TempdbIsTemp`, `##temp`, session SET statements and trusted procedures) may differ from this contract; record discrepancies rather than redefining the contract. Warning modes can suppress prompts; typed database-name confirmation remains intentionally disabled.

## What counts as changing the server

Measure rows; schema/catalog metadata and statistics; permissions, principals and impersonation; sequences and identity allocation (even after rollback); server/database configuration; Service Broker queues/conversations; permanent objects in tempdb; files, backups and OS/network/mail effects; linked-server and remote changes. Include effects before an error and asynchronous effects.

Measure on the **same live execution connection before disposal or cleanup**: transaction count/state, `IMPLICIT_TRANSACTIONS`, EXECUTE AS/SETUSER context, current database, persistent SET options, open keys/cursors, session context and application locks. Do not let rollback, pooling reset or cleanup erase evidence. Record visible and committed effects; restore only after observation.

## Outcome classes

| Outcome | Severity and disposition |
| --- | --- |
| FALSE GREEN | Blocker: unauthorized effect violates a green promise. Green-but-not-executable is inconclusive, not proof. |
| WRONG TARGET | High: finding identifies a different object than actual mutation; blocker if it causes unauthorized green. Compare canonical server/database/schema/object identity. |
| NOISE | Measure per rule: yellow/red but no observed change. Separate expected capability warnings (zero-row DML, denied permissions) from avoidable warnings. |
| CRASH / TIMEOUT | High availability/coverage failure; never pass. Preserve input, settings, timing and partial effects. |
| NOT EXECUTABLE | Inconclusive setup, syntax, permissions or missing-feature result; never pass. Preserve any effects that did occur. |

## Oracle blind spots

Snapshot equality cannot prove universal absence of effects. Transient reverted writes, cache/log/identity changes, asynchronous Broker/Agent/mail work, external hosts/files, encrypted procedures, triggers/cascades, inaccessible catalogs and permission-filtered metadata can escape an oracle. Concurrency contaminates attribution; isolation and repeats reduce but do not eliminate it. Feature/version/collation/context differences limit generalization. Instrumentation can alter timing and state. Offline rewrites prove only classification relationships, never actual SQL Server behavior.
