# Colors and meanings

The pane keeps one primary dot on the left. Red, yellow and green describe effects; purple, blue and gray describe code or context. Counts describe operations, not affected records. All six have display toggles; they never change execution warnings.

| Mark / term | Meaning |
|---|---|
| Red · RealWrite | Changes a permanent object now, including storing or altering code. |
| Yellow · Unknown | Effects cannot be proven from the available text and settings. |
| Green · ReadOnly | Proven no permanent data change for the recognized operation. `USE` retains this analyzer verdict but appears under blue Context in the pane. |
| Green · TempOnly | Changes temporary objects only, under the configured tempdb policy. |
| Green · AllowListed | Explicit authorization for the matched target or routine call and its side effects. This is not proof of read-only behavior. |
| Blue circle · Context | `USE` changes database name resolution without itself writing permanent data. Both the object and operation use blue on the left; the object has a blue count. Context is counted separately from Safe and has its own independent toggle. |
| Purple circle · Inside | On an object, a total of operations inside deferred code. On a body preview, a left marker with the actual risk breakdown on the right. Full definitions also keep their purple editor outlines. |
| Gray circle · Remote | A separate remote preview; its total is not part of submitted-operation totals. |
| Definition | Stores or changes code now. The body normally runs later. |
| Call / EXEC | Requests execution. An unproven procedure stays yellow even when a same-named definition is visible. |
| Dynamic SQL | Visible decoded SQL is analyzed at execution. A declaration is supporting source; runtime-dependent SQL stays yellow. |
| Body preview | Potential effects of visible code when invoked. Its scoped total is separate from submitted operations; its own row shows the risk breakdown. |
| Remote preview | Visible code in a separate server or provider context. Stored job commands use deferred Inside previews. It does not authorize the outer call. |
| Whole-target warning | A lowest-level explanation such as “No WHERE clause”; no redundant All rows badge on ancestors. |
| Approved write target | A server.database.schema.object rule for data writes, not definition changes. |
| Approved procedure / function | Separate kinds of qualified call authorization. Neither grants the other. |
| Inactive legacy rule | A retained short rule requiring explicit qualification before it authorizes anything. |

The six color meanings appear as individual rows under **Settings → Colors and meanings**, grouped into Effects and Code and context. Toggle tooltips show the name, count and the same description. SQL terms expand separately. See [settings](settings.md) for matching, context and migration rules, and the [safety contract](../safety/contract.md) for the precise promises.
