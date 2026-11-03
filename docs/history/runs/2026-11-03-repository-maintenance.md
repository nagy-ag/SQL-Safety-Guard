> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Repository maintenance, 2026-11-03

This record describes an uncommitted working tree based on `0dbe4b4`, not a released binary or a new server campaign.
The starting working tree was clean. No SQL, live SSMS session, installation, scheduled-task change, commit or push was performed.

## Completed stages

1. **Explicit verification entry points.** The harness requires exactly one mode; legacy discovery also requires an explicit
   disposable-server argument. Invalid or rejected campaign/isolated inputs stop before execution. Extension-check summaries
   distinguish FAIL and INCOMPLETE; individually approved BLOCKED results remain explicit exceptions. Debug launch uses an
   experimental profile, and capture tooling discovers the instance settings path.
2. **Traceability and provenance.** Roslyn indexes attributed test declarations with qualified names and source locations.
   Generated associations are checked against those declarations. All 527 existing rule IDs and semantic fields were compared
   with the starting revision and retained; source paths and test associations changed. Reports require fresh external outputs
   and explicit measurement identity. Freezing rebuilds clean source, records SDK/build commands and hashes the bundle.
   The launcher validates inputs before execution and preserves each generated case's principal. Old launch names forward
   to maintained entry points; the obsolete overnight-registration helper refuses to run.
3. **Repository navigation.** Existing guard configuration/decision types and grouped test classes now have matching files;
   extracted type/class bodies were compared with the starting source. Harness arguments and reporting have their own files.
   Architecture, development commands, current runbooks, historical evidence and generated references have distinct purposes.
   Original moved evidence is byte-preserved and checksum-protected. Agent skills retain identical mirrors and link to the
   reusable human runbooks.
4. **Offline reproducibility and access boundary.** SDK feature-band policy, package locks, a single offline gate and a
   Windows CI workflow were added. Release preparation uses the same gate. Production dependency/source checks reject
   SQL/network-client additions and SQL execution APIs in Debug and Release source.

## Validation actually performed

| Check | Observed result | What it does not establish |
|---|---|---|
| Original offline suite | 1,574 passed before changes | Server or SSMS behavior |
| Final `scripts/Verify-Offline.ps1` | Exit 0: 1,592 .NET tests, 527-rule freshness check, 10 Python tooling tests | Live server effects; remote CI result |
| Locked solution builds, Debug and Release | Both succeeded, zero warnings and errors | Installation or live host routing |
| Release UI interaction scripts | 13 pane, 15 confirmation and 4 settings checks passed | A running SSMS instance or visual theme acceptance |
| Campaign validation-only modes | 33,035 lane cases accepted, zero rejected; 71 isolated feature fixtures accepted | Case execution, oracle observations or campaign coverage |
| Freeze/launcher orchestration fixtures | Stubbed git/dotnet checks covered dirty source, failed builds, forwarding, validation-only operation and tampered binaries | A real clean-source freeze or SQL campaign |
| Release VSIX ZIP inspection | No SqlClient, ServerHarness, SMO or WaveClr assembly in the package | A security sandbox against arbitrary future code |
| Repository checks | Maintained PowerShell syntax, JSON/XML/Python/JSONL parsing, links, mirror/checksum checks and `git diff --check` passed | Runtime behavior outside the checks' scope |

One full-suite run made while a solution build was also running failed the existing 3-second broken-batch assertion at
3.0798846 seconds. A subsequent full offline gate without competing build work passed. Contention is a possible explanation,
not a proven cause; the test and its limit were left unchanged. An earlier wrapper run also exposed PowerShell treating
unittest's normal stderr progress as an error during log capture; the offline gate now directs that progress to stdout.

Raw logs and validation inputs remain outside the repository under
`../harness-out/repository-maintenance-20261103-093343/`, including the failed run and final successful gate. They are local
artifacts, not committed evidence. The two newly archived original reports have in-repository checksum manifests.

## Product boundary and remaining work

The extension continues to analyze text/options and read labels from SSMS-owned objects. It opens no SQL connection,
executes no SQL and queries no server metadata. Server clients remain confined to verification tooling. The source tests
and package inspection are regression guardrails, not a universal security proof. Production runtime changes are limited
to behavior-preserving type extraction; the VSIX runtime changes are comments. Debug launch configuration changes are
separate from runtime behavior.

Analyzer verdict policy, guard decisions and confirmation behavior are unchanged. Unknown analysis still fails closed;
the [owner policy questions](../../verification/policy-questions.md) remain pending. Verification entry-point validation,
reporting and campaign principals intentionally changed and require their updated runbooks.

The new CI workflow has not run remotely. A real freeze needs a clean reviewed checkout; release packaging was not run
against this dirty working tree. Live SSMS routing, theme review and server verification require separately authorized
environments. Historical server/host reports continue to describe their recorded builds only.
