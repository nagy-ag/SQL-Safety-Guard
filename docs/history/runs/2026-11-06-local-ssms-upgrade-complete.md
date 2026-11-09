> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Local published 1.0.3 same-installation upgrade — 2026-11-06

**PASS for the three focused local upgrade checks:** unchanged published SQL Safety Guard 1.0.3
survived the same-installation SSMS **22.9.2 / 22.9.12120.119 → 22.11.3 / 22.11.12328.437** update.
Both phases accepted green read, red Cancel without submission/write, and red Run anyway with exactly
one attributed update. All six settings and all 27 installed package payload files were preserved.
The test used the same isolated profile and installation identity/path across the update.

The [baseline-only snapshot](2026-11-06-local-ssms-upgrade-baseline.md) remains unchanged and describes
its earlier point in time. After its initial consent request expired, the owner requested and approved
fresh elevation. The successful upgrade wrapper started at **17:16:31.3173313 UTC**, entered the update
stage at **17:17:19 UTC**, and completed with **exit 0, no reboot** at **17:40:49 UTC**. There was one
successful upgrade invocation and one after-phase SSMS launch. No baseline rerun was needed.

## Observed results

| Check | Before 22.9.2 | After 22.11.3 |
|---|---|---|
| Exact full build | 22.9.12120.119 | 22.11.12328.437 |
| Installation/profile | Isolated profile on the new installation ID created by the authorized downgrade | Same installation ID, installation path, isolated profile and package directory |
| Green read | Correct connection, exact SELECT submitted; successful result, counter 0 | Correct connection, exact SELECT submitted in the owned query session; successful completion without guard prompt, counter 1 |
| Red Cancel | Guard showed expected server/database and whole-window context; counter/events 0/0, no UPDATE submitted | Same context and permanent UPDATE target; counter/events stay 1/1, exact query-session input unchanged |
| Red Run anyway | Exactly one UPDATE and one attributed audit event; counter 0→1 | Exactly one new UPDATE and audit event in the owned write session; counter 1→2, events 1→2 |
| Native settings | Both rendered Preview forms, six unique typed values each | Both rendered Preview forms, six unique typed values each; all values match baseline |
| Persisted settings | Six seeded custom values accepted after clean closure | Same six keys, types and values accepted after clean closure |
| Extension files | All 27 payload members match frozen published VSIX | Same 27 paths/hashes, no missing/extra members; independent ZIP and live-file comparison accepted |
| Other installation | Visual Studio Community registered full version 18.11.12324.101 | Same registered version |
| Closure | Owned baseline SSMS closed cleanly | Owned after SSMS closed by WM_CLOSE; Get-Process and CIM absence confirmed |

Settings covered YellowRed warnings, connection display, table allow-list, trusted procedures,
environment rules and permanent-tempdb opt-in. Native dropdown selection came from
SelectionPattern.Current.GetSelection, with complete form ancestry/PID/RuntimeId checks. No settings
were rewritten after the upgrade. SQL evidence includes the exact database, application, SSMS host
PID, query session, submitted input batch, old/new counter values and retained baseline audit event.

## Package integrity and SSMS metadata

Frozen published VSIX SHA-256:
`0EE37AE4847CAF31544EE4A9F686E3C10A6F72A55D3970859C338E6E4F4F69DC`.
Release assembly SHA-256:
`97B90388DC8D604D8F6B80C411727693432F1B229D06077964F3F0FA8C5B81F2`.

| SSMS root cache/metadata file | Before/after observation |
|---|---|
| CompatibilityList.xml | Unchanged |
| ExtensionMetadata2.0.mpack | Hash changed; recorded separately |
| ExtensionMetadataCache.mpack | Hash changed; recorded separately |
| extensions.configurationchanged | Unchanged |

These are shell metadata, outside the 27-file extension payload. Both changed cache hashes and the
unchanged files' existence/hashes are retained in package-after.json. No post-update configuration
refresh, repair, VSIX reinstall, cache deletion, rebuild, repackage or product edit was performed.

## Qualifications and retained errors

- Scope is **three focused local cases**, not the eleven-case VM suite. SQLCMD-on, deliberate
  target ambiguity, selection/multibatch breadth and physical F5 are not newly qualified.
- Baseline green used the owner's toolbar click; baseline red decisions and all three after-phase
  cases used owned-window UI Automation and messages. No global input, clipboard or focus call was used.
- After-phase UIA did not expose individual results-grid cells. Exact SELECT submission, successful
  SSMS completion, correct connection and independent counter readback corroborate green execution.
- The first read-only snapshot immediately after Run still showed 1/1 because SSMS executes
  asynchronously. A later settled snapshot showed 2/2 and the single exact attributed write. Neither
  Execute nor Run was repeated to obtain that result. The earlier snapshot is retained.
- First-start observations occurred before the main window was ready; no action was submitted then.
  The normal startup completed, loaded the extension and connected the prepared query.
- The after activity log retains **29 Error entries**: shell partial package initialization and UI
  library/resource errors (including C# interactive), Unified Settings registrations/services,
  source-control options and Copilot service/registration failures. None names SQL Safety Guard or
  its package GUID; its package load completed, and its requested behavior was verified. This is
  qualified extension survival evidence, not certification of general SSMS or Copilot health.
- The authorized downgrade changed the original installation ID before baseline. The subsequent
  22.9.2→22.11.3 update preserved that new ID. Normal-profile migration from the original ID was not
  tested; original normal/other test profiles and their backups were preserved.

## Evidence and completion

Raw evidence remains outside the repository in the local upgrade campaign directory. The original
baseline-acceptance.json is retained, and the one-time Review-LocalAfter.py independently checked
native controls, persisted settings, raw execution/context/target observations, exact builds, frozen
ZIP/live payload files and cache deltas before writing **after-acceptance.json: Accepted=true**.
Its SHA-256 is `55307249737243DE2B8C4BAC545990425F01DEAC8CB9A36C0A7580AEA3192D78`.

The owned SSMS session is closed, no installer remains, and the same monitor is disabled. Backups,
registry/workload exports, original profiles, the isolated test profile and owned audit database are
retained as recovery/evidence material; the final counter/events are 2/2. No unrelated process,
profile, database, Docker or cloud resource was changed. No automatic campaign retry is pending.
Documentation validation: focused DocsIndexTests and git diff --check are recorded with this run.
