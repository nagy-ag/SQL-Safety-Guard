# Offline coverage gaps - wave 1

Measured October 28, 2026 using coverlet.collector 6.0.4 / XPlat Code Coverage; analyzer only, Debug build, default rewrite count. No production instrumentation or SQL execution.

| Suite | Lines hit / executable | Line coverage | Branch arms hit / total | Branch coverage |
| --- | ---: | ---: | ---: | ---: |
| Existing 824 tests | 2294/2438 | 94.09% | 1508/1738 | 86.77% |
| Augmented suite | 2300/2438 | 94.34% | 1517/1738 | 87.28% |

Executable lines come from Cobertura, not every source line. Duplicate class entries for the same file/line are merged by maximum hits/branch counts. Branch counts are instrumentation arms, not semantic rules. Missing/partial branches below report covered/total; Cobertura does not reliably identify which source true/false arm was missed. Generated/compiler branches may be included. Coverage is evidence of execution, not assertions or SQL Server correctness.

Rule-reference gate: 505/1501 IDs mapped; 996 uncovered (report-only). Most unreviewed decision.* checkpoints remain unmapped deliberately. A named-test association is not a branch hit or an executable-server pass.

## Lines and branch locations

Each column lists **every** unexecuted executable line or incomplete branch location for that suite; ranges are inclusive. Baseline and augmented snapshots remain distinct.

### src/SqlSafetyGuard.Analyzer/AllowList.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 35 | 17: 1/2, 38: 1/2, 43: 1/2, 74: 3/4, 85: 1/4, 88: 1/2 |
| Augmented | 35 | 17: 1/2, 38: 1/2, 43: 1/2, 74: 3/4, 85: 1/4, 88: 1/2 |

### src/SqlSafetyGuard.Analyzer/AnalysisResult.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 55-56, 58-60 | 54: 1/2, 56: 0/4, 72: 1/2, 79: 1/2 |
| Augmented | 55-56, 58-60 | 54: 1/2, 56: 0/4, 72: 1/2, 79: 1/2 |

### src/SqlSafetyGuard.Analyzer/AnalysisWork.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 36, 123, 125, 134-137, 139-144 | 35: 2/4, 109: 3/4, 115: 1/2 |
| Augmented | 36, 123, 125, 134-137, 139-144 | 35: 2/4, 109: 3/4, 115: 1/2 |

### src/SqlSafetyGuard.Analyzer/AnalyzerOptions.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | None | None |
| Augmented | None | None |

### src/SqlSafetyGuard.Analyzer/BatchRecovery.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 85, 135, 250, 252, 303, 310-312, 314-315, 334-335, 337-339, 371-376, 380-385, 419, 465, 484, 496-497, 499-500 | 77: 3/4, 84: 2/4, 134: 1/2, 143: 3/4, 210: 3/4, 233: 1/2, 246: 1/2, 248: 1/2, 302: 1/2, 309: 1/2, 311: 0/2, 322: 26/28, 325: 4/8, 331: 1/2, 344: 2/4, 359: 1/10, 370: 1/2, 378: 2/12, 402: 4/8, 408: 2/4, 413: 1/2, 464: 1/2, 477: 3/4, 483: 1/2, 486: 8/10, 509: 3/4, 524: 1/2 |
| Augmented | 85, 135, 250, 252, 303, 312, 334-335, 337-339, 371-376, 380-385, 419, 465, 484, 496-497, 499-500 | 77: 3/4, 84: 2/4, 134: 1/2, 143: 3/4, 210: 3/4, 233: 1/2, 246: 1/2, 248: 1/2, 302: 1/2, 311: 1/2, 322: 26/28, 325: 4/8, 331: 1/2, 344: 2/4, 359: 1/10, 370: 1/2, 378: 7/12, 402: 4/8, 408: 2/4, 413: 1/2, 464: 1/2, 477: 3/4, 483: 1/2, 486: 8/10, 509: 3/4, 524: 1/2 |

### src/SqlSafetyGuard.Analyzer/Classifier.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 212-213, 264, 299 | 209: 1/2, 259: 1/2, 263: 2/4, 267: 5/6, 276: 13/14, 298: 1/2 |
| Augmented | 264, 299 | 259: 1/2, 263: 2/4, 267: 5/6, 276: 13/14, 298: 1/2 |

### src/SqlSafetyGuard.Analyzer/EnvironmentRules.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | None | 60: 1/2, 93: 1/2, 105: 3/4 |
| Augmented | None | 60: 1/2, 93: 1/2, 105: 3/4 |

### src/SqlSafetyGuard.Analyzer/Finding.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 114, 118, 122, 145, 149 | 22: 1/2, 23: 1/2, 26: 1/2, 92: 5/6, 97: 3/4, 109: 1/2, 134: 1/2 |
| Augmented | 114, 118, 122, 145, 149 | 22: 1/2, 23: 1/2, 26: 1/2, 92: 5/6, 97: 3/4, 109: 1/2, 134: 1/2 |

### src/SqlSafetyGuard.Analyzer/ParserFactory.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 24, 36-39, 53-55, 57, 59 | 22: 1/2, 51: 1/2 |
| Augmented | 24, 36-39, 53-55, 57, 59 | 22: 1/2, 51: 1/2 |

### src/SqlSafetyGuard.Analyzer/Scope.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 93, 140, 147 | 76: 1/2, 105: 3/4, 123: 3/4, 139: 1/2, 146: 3/6 |
| Augmented | 93, 140, 147 | 76: 1/2, 105: 3/4, 123: 3/4, 139: 1/2, 146: 3/6 |

### src/SqlSafetyGuard.Analyzer/SideEffectScanner.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 48, 100 | 93: 1/2 |
| Augmented | 48, 100 | 93: 1/2 |

### src/SqlSafetyGuard.Analyzer/SqlCmdPreprocessor.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 53 | 45: 3/4, 74: 5/6, 80: 3/4, 104: 5/6 |
| Augmented | 53 | 45: 3/4, 74: 5/6, 80: 3/4, 104: 5/6 |

### src/SqlSafetyGuard.Analyzer/SqlSafetyAnalyzer.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | None | 21: 3/4 |
| Augmented | None | 21: 3/4 |

### src/SqlSafetyGuard.Analyzer/StatementWalker.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 152, 380-381, 395, 429, 465, 802-803, 812-814, 896-897, 904, 942-944, 962-965, 1026, 1070, 1072, 1077, 1092, 1101, 1165-1166, 1258-1260 | 121: 1/2, 124: 1/2, 132: 1/2, 145: 1/2, 150: 6/12, 153: 1/2, 228: 1/2, 282: 1/2, 357: 29/32, 376: 2/4, 407: 3/4, 453: 3/4, 502: 1/2, 538: 1/2, 547: 1/2, 579: 7/10, 584: 3/10, 587: 1/2, 679: 2/4, 689: 1/2, 716: 5/6, 744: 4/6, 745: 2/4, 747: 1/6, 788: 3/4, 796: 1/2, 797: 2/4, 798: 1/2, 811: 1/2, 823: 3/4, 828: 7/10, 835: 2/4, 842: 1/2, 843: 3/4, 880: 3/4, 917: 1/2, 921: 3/4, 932: 1/2, 941: 1/2, 944: 0/2, 956: 4/8, 967: 3/4, 978: 9/10, 980: 1/2, 994: 2/4, 1025: 1/2, 1060: 5/6, 1063: 2/4, 1085: 5/6, 1100: 1/2, 1106: 5/6, 1111: 2/4, 1149: 3/4, 1241: 3/4, 1312: 3/4, 1314: 3/4 |
| Augmented | 152, 380-381, 395, 429, 465, 802-803, 812-814, 896-897, 904, 942-944, 962-965, 1026, 1070, 1072, 1077, 1092, 1101, 1165-1166, 1258-1260 | 121: 1/2, 124: 1/2, 132: 1/2, 145: 1/2, 150: 6/12, 153: 1/2, 228: 1/2, 282: 1/2, 357: 29/32, 376: 2/4, 407: 3/4, 453: 3/4, 502: 1/2, 538: 1/2, 547: 1/2, 579: 7/10, 584: 3/10, 587: 1/2, 679: 2/4, 716: 5/6, 744: 4/6, 745: 2/4, 747: 1/6, 788: 3/4, 796: 1/2, 797: 2/4, 798: 1/2, 811: 1/2, 823: 3/4, 828: 7/10, 835: 2/4, 842: 1/2, 843: 3/4, 880: 3/4, 917: 1/2, 921: 3/4, 932: 1/2, 941: 1/2, 944: 0/2, 956: 4/8, 967: 3/4, 978: 9/10, 980: 1/2, 994: 2/4, 1025: 1/2, 1060: 5/6, 1063: 2/4, 1085: 5/6, 1100: 1/2, 1106: 5/6, 1111: 2/4, 1149: 3/4, 1241: 3/4, 1312: 3/4, 1314: 3/4 |

### src/SqlSafetyGuard.Analyzer/TargetResolver.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 98, 106, 136, 169-170, 173-174, 176, 205-206, 247-249, 260 | 59: 1/2, 96: 1/2, 104: 1/2, 116: 7/10, 135: 1/2, 155: 1/2, 156: 3/4, 173: 1/2, 183: 3/4, 192: 4/6, 226: 6/10, 240: 5/6, 247: 0/2, 259: 2/4, 267: 6/8 |
| Augmented | 98, 106, 136, 169-170, 173-174, 176, 205-206, 247-249, 260 | 59: 1/2, 96: 1/2, 104: 1/2, 116: 7/10, 135: 1/2, 155: 1/2, 156: 3/4, 173: 1/2, 183: 3/4, 192: 4/6, 226: 6/10, 240: 5/6, 247: 0/2, 259: 2/4, 267: 6/8 |

### src/SqlSafetyGuard.Analyzer/TextPosition.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 13, 16, 26, 28-29, 52-55 | 15: 1/2, 23: 3/4, 25: 1/2 |
| Augmented | 13, 16, 26, 28-29, 52-55 | 15: 1/2, 23: 3/4, 25: 1/2 |

### src/SqlSafetyGuard.Analyzer/Verdict.cs

| Suite | Never-hit lines | Incomplete branches (line: hit/total) |
| --- | --- | --- |
| 824 baseline | 29, 31, 34-35, 37-42, 44 | 35: 0/6 |
| Augmented | 29, 31, 34-35, 37-42, 44 | 35: 0/6 |

## Unmapped rule IDs

Re-run the gate with detailed output for the live list. This snapshot intentionally does not fail the build.

```text
action-name.altertableaddclusterbystatement
action-name.raiseerrorlegacystatement
decision.allowlist.0295ac6c4fca
decision.allowlist.0558189e6768
decision.allowlist.102b72dce219
decision.allowlist.1944979fd89c
decision.allowlist.196a18650237
decision.allowlist.1b11e86f16e3
decision.allowlist.3e7aaa9e7e4b
decision.allowlist.41130042276c
decision.allowlist.54aa482f649e
decision.allowlist.55f756a7ab0d
decision.allowlist.64a2de30d45f
decision.allowlist.6d74dc145970
decision.allowlist.854b1740a28e
decision.allowlist.86f2115b6473
decision.allowlist.926e8c82aebb
decision.allowlist.93b32cb4ef21
decision.allowlist.9d2e17644a9c
decision.allowlist.9df0a51c6d97
decision.allowlist.9df0a51c6d97.2
decision.allowlist.9fc61e62a8f1
decision.allowlist.a4ec36febd31
decision.allowlist.a8b440e74ff5
decision.allowlist.aebb4a67c04a
decision.allowlist.b13ed996bd9f
decision.allowlist.b97d212f7dc2
decision.allowlist.c9e8040414bd
decision.allowlist.cab3af42af2d
decision.allowlist.ceb5b1431160
decision.allowlist.d1c618a1c94a
decision.allowlist.d79aa287ea4b
decision.allowlist.dc3e35d8c418
decision.allowlist.ee324a9c8e39
decision.analysisresult.017207a826ef
decision.analysisresult.03739fc4ed7f
decision.analysisresult.086e5a84ca4d
decision.analysisresult.2f4fc67a8c77
decision.analysisresult.331530df59c9
decision.analysisresult.3d0cb8a92d17
decision.analysisresult.3f8a3fe6cf38
decision.analysisresult.46ce08d83304
decision.analysisresult.50f1668e5dd8
decision.analysisresult.60906e23d317
decision.analysisresult.6234f26bebcc
decision.analysisresult.65fbead900fd
decision.analysisresult.6b7fb61a21be
decision.analysisresult.8559c3d5ce74
decision.analysisresult.95f361423602
decision.analysisresult.c1ae6d7329bd
decision.analysisresult.c2e49163389b
decision.analysisresult.c7f15cb19cd5
decision.analysisresult.dbd32ca4a1dd
decision.analysisresult.f18698b1c450
decision.analysisresult.f2393f0a665b
decision.analysisresult.fa843db9fc3c
decision.analysiswork.006cc59f5e53
decision.analysiswork.0152256303e3
decision.analysiswork.0178a2449593
decision.analysiswork.026a573235cd
decision.analysiswork.10dc4b7dc558
decision.analysiswork.14de93724b67
decision.analysiswork.15107026391e
decision.analysiswork.16f0a326f81b
decision.analysiswork.1fe12b791c07
decision.analysiswork.22b27225c4b4
decision.analysiswork.23cb5875ae61
decision.analysiswork.269cee6ff2af
decision.analysiswork.29b296a58819
decision.analysiswork.3bc176ed294b
decision.analysiswork.3fb47a0382da
decision.analysiswork.4026bfb09ed7
decision.analysiswork.403337430d37
decision.analysiswork.4a63d9628040
decision.analysiswork.4d20554641a2
decision.analysiswork.51592837c148
decision.analysiswork.56d459c564f8
decision.analysiswork.5a4d5b137b35
decision.analysiswork.622d20d69c75
decision.analysiswork.66a225846b34
decision.analysiswork.6dd64bba8e06
decision.analysiswork.7217e5d637a0
decision.analysiswork.727581783960
decision.analysiswork.783c13c3dc95
decision.analysiswork.7ac4a19ee278
decision.analysiswork.7bd8a5603d55
decision.analysiswork.7cb994e79946
decision.analysiswork.7f21dc4da8d8
decision.analysiswork.81bac7e29f53
decision.analysiswork.8255daf667b2
decision.analysiswork.8382695581d9
decision.analysiswork.86d902288d1b
decision.analysiswork.94bc2165b57a
decision.analysiswork.9ab5da657e0f
decision.analysiswork.a066949e4685
decision.analysiswork.bd06501b8ae1
decision.analysiswork.c2d27de5b93a
decision.analysiswork.c371c8f8fe23
decision.analysiswork.c43b76ebadc2
decision.analysiswork.cfe22035e100
decision.analysiswork.d2bdc87287de
decision.analysiswork.d4affc90e5a1
decision.analysiswork.d51422745295
decision.analysiswork.d87b538d638f
decision.analysiswork.d9d3b0dc8a20
decision.analysiswork.e17ccb82cbf3
decision.analysiswork.e22ed5a2cfe1
decision.analysiswork.e5f8ea999362
decision.analysiswork.e94d82b170b7
decision.analysiswork.eae14527874c
decision.analysiswork.ebf5619be347
decision.analysiswork.ed5ea21d901b
decision.analysiswork.f62ff4a64ba3
decision.analyzeroptions.39e9ead66517
decision.analyzeroptions.563b5833c82a
decision.analyzeroptions.7482abc3c151
decision.analyzeroptions.a29b0a425317
decision.analyzeroptions.e1c35d66f703
decision.batchrecovery.05c68cd42894
decision.batchrecovery.07ffa56f2b31
decision.batchrecovery.0935ab872d68
decision.batchrecovery.0e284ee32d01
decision.batchrecovery.122361eb070f
decision.batchrecovery.13cb37ef8d6a
decision.batchrecovery.1495c4c7bd42
decision.batchrecovery.15d894a9ee92
decision.batchrecovery.16e745a6688f
decision.batchrecovery.17aee5072eb0
decision.batchrecovery.1a97672cf188
decision.batchrecovery.1cc2f002ad07
decision.batchrecovery.1dfecc1a8307
decision.batchrecovery.21b73119bfa7
decision.batchrecovery.241b5806c150
decision.batchrecovery.25aaea731f8e
decision.batchrecovery.289f3813e04e
decision.batchrecovery.2e5f629269dd
decision.batchrecovery.30a4f4f7363c
decision.batchrecovery.3406d637b704
decision.batchrecovery.349b832c52dc
decision.batchrecovery.34b39225ede7
decision.batchrecovery.34b39225ede7.2
decision.batchrecovery.34b39225ede7.3
decision.batchrecovery.34b39225ede7.4
decision.batchrecovery.34b39225ede7.5
decision.batchrecovery.3cb07161d122
decision.batchrecovery.3dd450770088
decision.batchrecovery.3e141f54c937
decision.batchrecovery.3eaf8117e1ef
decision.batchrecovery.402e5a9b1e86
decision.batchrecovery.4697fc6bcbe2
decision.batchrecovery.4703e51099ce
decision.batchrecovery.47a94bf76eb9
decision.batchrecovery.4b1426ff57a2
decision.batchrecovery.4c1bff91811b
decision.batchrecovery.4d371c6463a7
decision.batchrecovery.50571ac104a4
decision.batchrecovery.508ad2835f6b
decision.batchrecovery.5555f924cd8b
decision.batchrecovery.55ee21034973
decision.batchrecovery.596a8f6d5b8e
decision.batchrecovery.5a0833c33748
decision.batchrecovery.5b4e0b3cfd52
decision.batchrecovery.5be854cac042
decision.batchrecovery.5d36ed03b6a1
decision.batchrecovery.5dcb94c642d7
decision.batchrecovery.5e3f6a5c5358
decision.batchrecovery.5e96cc4f264d
decision.batchrecovery.601bc399ee1e
decision.batchrecovery.65527e34b698
decision.batchrecovery.6bdcbcbb8cf9
decision.batchrecovery.6c63143724d7
decision.batchrecovery.6e1de645d3be
decision.batchrecovery.72fd26ad9661
decision.batchrecovery.75a1468f41dd
decision.batchrecovery.766d198c3897
decision.batchrecovery.7ab1d8b37301
decision.batchrecovery.7ac9b0f5b69e
decision.batchrecovery.7c79a23413f0
decision.batchrecovery.7f386e3a483a
decision.batchrecovery.861427d5d5c4
decision.batchrecovery.87480c754f7d
decision.batchrecovery.8ee842e7f1a9
decision.batchrecovery.901db2faac17
decision.batchrecovery.984bdc6c1e1f
decision.batchrecovery.9bc3ac58085e
decision.batchrecovery.9c51acd868f8
decision.batchrecovery.a0f504ab6e1c
decision.batchrecovery.a17dbf0ddf70
decision.batchrecovery.a784ed0a6428
decision.batchrecovery.ab6e01f9e8a5
decision.batchrecovery.ab89a821c0f1
decision.batchrecovery.ad232602acb9
decision.batchrecovery.ae653b71777e
decision.batchrecovery.b470ac2101bb
decision.batchrecovery.b50369369b73
decision.batchrecovery.b67812914150
decision.batchrecovery.b75e23763db6
decision.batchrecovery.b9e770a2ce51
decision.batchrecovery.bafe722e7b29
decision.batchrecovery.bafe722e7b29.2
decision.batchrecovery.c34774242d2a
decision.batchrecovery.c42b3830c48c
decision.batchrecovery.c8fb5f0dd104
decision.batchrecovery.c906390098ca
decision.batchrecovery.caa34ddd9e26
decision.batchrecovery.cdedec590c49
decision.batchrecovery.cdf6dd9e54db
decision.batchrecovery.ce65f3cf45a5
decision.batchrecovery.d17ef57be981
decision.batchrecovery.d65a8ea727db
decision.batchrecovery.d7f264950d33
decision.batchrecovery.da750fdbcbb5
decision.batchrecovery.da750fdbcbb5.2
decision.batchrecovery.db46b258a16d
decision.batchrecovery.db46b258a16d.2
decision.batchrecovery.db46b258a16d.3
decision.batchrecovery.dd65e2c236d3
decision.batchrecovery.dfbd5a883843
decision.batchrecovery.e7bd60b0b84b
decision.batchrecovery.eab0361e1bbf
decision.batchrecovery.eed6835835d3
decision.batchrecovery.f42b98654496
decision.batchrecovery.f582a9817f5f
decision.batchrecovery.f99463cb2214
decision.batchrecovery.fc47d00f5071
decision.batchrecovery.fd822028a4b1
decision.batchrecovery.fe535ae22196
decision.batchrecovery.ff1661572d02
decision.batchrecovery.ffe0642080a5
decision.classifier.01592b8ed2a0
decision.classifier.02b84ffe2658
decision.classifier.038e2fd72df0
decision.classifier.11f320c3559b
decision.classifier.1e9508faeeee
decision.classifier.1eaba5e4d065
decision.classifier.220b2fe2d409
decision.classifier.22d176288295
decision.classifier.26695b9cd6c1
decision.classifier.380178cd83e3
decision.classifier.429be7dd060e
decision.classifier.51f91896aea1
decision.classifier.53bb5481a177
decision.classifier.56efabd02330
decision.classifier.56efabd02330.2
decision.classifier.56efabd02330.3
decision.classifier.56efabd02330.4
decision.classifier.669abc033dd3
decision.classifier.681cfc101e9e
decision.classifier.6e988fc71b82
decision.classifier.74a82d833e1c
decision.classifier.7e170c5df5f4
decision.classifier.7f06cbc77e29
decision.classifier.881673f8ae50
decision.classifier.8d3630ed23f0
decision.classifier.9533d13ee736
decision.classifier.99ba6208ad88
decision.classifier.9bccdb68ebec
decision.classifier.a42bcba9727f
decision.classifier.aedd0c938811
decision.classifier.b0b577d6017e
decision.classifier.b5a413ab98b0
decision.classifier.bf0b7c727b30
decision.classifier.c63eba4e9edc
decision.classifier.caea0120be4f
decision.classifier.cfc01c23883c
decision.classifier.d2edc3c58d7f
decision.classifier.d3842d6b0ae7
decision.classifier.d7b26961ed79
decision.classifier.df0dd1647ecf
decision.classifier.dfd60ae13930
decision.classifier.e18cd79a178f
decision.classifier.f15c778de2e5
decision.classifier.f90172dc7f04
decision.classifier.f95291e1240a
decision.environmentrules.008de0d9f78a
decision.environmentrules.017706c8b699
decision.environmentrules.085d9114192b
decision.environmentrules.22f86d786aa2
decision.environmentrules.2ad900409536
decision.environmentrules.3633c5a224fb
decision.environmentrules.44cd3ad50e64
decision.environmentrules.44d4648112fe
decision.environmentrules.4e235731df98
decision.environmentrules.4e62191aa7a8
decision.environmentrules.5a35bdd3a558
decision.environmentrules.5bf213163c42
decision.environmentrules.694779980d2e
decision.environmentrules.71f7f6e97d25
decision.environmentrules.78f4d199832e
decision.environmentrules.822291981b6f
decision.environmentrules.925d70cf8e9f
decision.environmentrules.a04ef54aaf64
decision.environmentrules.a245751c987c
decision.environmentrules.a663cdbf7147
decision.environmentrules.a7c07cdd69e5
decision.environmentrules.af45223d9e81
decision.environmentrules.b1cda85c26f6
decision.environmentrules.bfb87f571b65
decision.environmentrules.c349b9a924b6
decision.environmentrules.cb6a33aa2a6a
decision.environmentrules.d0d07ce961ed
decision.environmentrules.d6675a1c60e3
decision.environmentrules.d9cfd2a2aca9
decision.environmentrules.dbdcc1e83edc
decision.environmentrules.ebf529aa43fc
decision.finding.0daf31e9b515
decision.finding.156f62107d45
decision.finding.1bc4373491ad
decision.finding.1f81f8211bb7
decision.finding.218caad0db9b
decision.finding.2538c135954b
decision.finding.4ab3d74dd2cd
decision.finding.4b1d4872002f
decision.finding.566a460bf0aa
decision.finding.6b0eb32082aa
decision.finding.6d7382d8959f
decision.finding.6e157b1634f8
decision.finding.7e94d7df39e2
decision.finding.8393343ab4ea
decision.finding.8b6243eec967
decision.finding.901d5e214931
decision.finding.a9b3aadab87a
decision.finding.a9de8c981cb3
decision.finding.ac227cbdf2fc
decision.finding.c7b24ed3ae62
decision.finding.d919a3fd3095
decision.finding.de676ca102a2
decision.finding.e783927b272d
decision.finding.f17e140aa7c7
decision.finding.fd7ff288b793
decision.finding.fe888401008e
decision.finding.ff42b9f483dc
decision.parserfactory.12fc54bbb619
decision.parserfactory.24c5f283591a
decision.parserfactory.36bf6504b7c5
decision.parserfactory.49585cbd38ca
decision.parserfactory.5063bbaa16dd
decision.parserfactory.530865613c1f
decision.parserfactory.5548133f2541
decision.parserfactory.5ac172c68655
decision.parserfactory.5e1c971f3fc8
decision.parserfactory.719f24f53da3
decision.parserfactory.72e9062608e2
decision.parserfactory.8509fc603ff3
decision.parserfactory.96f2943bdc88
decision.parserfactory.a8c13fadc4d8
decision.parserfactory.b688842d9d7f
decision.parserfactory.c6ee0337e3b7
decision.parserfactory.ed8989cb21f7
decision.scope.062836fa985d
decision.scope.064c6d84becb
decision.scope.0884cdf1f486
decision.scope.111e872d87f8
decision.scope.240a2704e945
decision.scope.2e17d6d21f90
decision.scope.386119a8b230
decision.scope.3a74894a832c
decision.scope.3f742c4dc85f
decision.scope.416f00bfaa6c
decision.scope.59e5d179a0a3
decision.scope.5dc9489f2dfa
decision.scope.6003ffe4860a
decision.scope.62ca15f8ec63
decision.scope.6597cc1ed87c
decision.scope.704d69569d20
decision.scope.704d69569d20.2
decision.scope.72cdd73f4c7f
decision.scope.85e39ed71d57
decision.scope.88ba0ed0f17d
decision.scope.93936b92bb3d
decision.scope.af91cf296a2f
decision.scope.b16448fe7028
decision.scope.c0fc97c2d9fd
decision.scope.c25fe58553ac
decision.scope.c2940eb06ee5
decision.scope.cd7611964053
decision.scope.cd9b2e16abbe
decision.scope.d55aba66c4ad
decision.scope.dd8f0fb70236
decision.scope.de68b1e3ef9c
decision.scope.e69268c3c4f0
decision.scope.e80a5203290d
decision.scope.efad0f38a80c
decision.scope.f553a3fdec25
decision.scope.f8a11a4bc74d
decision.scope.fb2b74d65ce9
decision.scope.ff7863723e7c
decision.sideeffectscanner.0c492af02e9d
decision.sideeffectscanner.1c590bb5dbef
decision.sideeffectscanner.1db06c927b5b
decision.sideeffectscanner.2cf28fd70c44
decision.sideeffectscanner.363de174d4f9
decision.sideeffectscanner.3c9caa8db8dd
decision.sideeffectscanner.51e12c22aa31
decision.sideeffectscanner.63c5547b40eb
decision.sideeffectscanner.721cdd17ee08
decision.sideeffectscanner.777ba3e271ed
decision.sideeffectscanner.7d901d76f675
decision.sideeffectscanner.9d6d47a143ff
decision.sideeffectscanner.ab9363be5797
decision.sideeffectscanner.b9d3f31f1e97
decision.sideeffectscanner.bb17f32e542e
decision.sideeffectscanner.ca6e1d304556
decision.sideeffectscanner.cb8c9e97a710
decision.sideeffectscanner.d5f24ffea0c3
decision.sideeffectscanner.da7187e02fed
decision.sideeffectscanner.ed7bc3e50ca9
decision.sideeffectscanner.f1269b3c9d53
decision.sideeffectscanner.f2a50bc9adad
decision.sideeffectscanner.ff8745f0c06a
decision.sqlcmdpreprocessor.109c62de41d3
decision.sqlcmdpreprocessor.12e257be24c2
decision.sqlcmdpreprocessor.159813d522b8
decision.sqlcmdpreprocessor.1c83bd96ab54
decision.sqlcmdpreprocessor.2e221d2de9e7
decision.sqlcmdpreprocessor.451f62a69f74
decision.sqlcmdpreprocessor.45b74518ebb8
decision.sqlcmdpreprocessor.560d0eef56b9
decision.sqlcmdpreprocessor.647543704fb5
decision.sqlcmdpreprocessor.6795c546773b
decision.sqlcmdpreprocessor.6bbd847381b4
decision.sqlcmdpreprocessor.851c3d25894a
decision.sqlcmdpreprocessor.9cd6f69cbd71
decision.sqlcmdpreprocessor.a9538bbd4df9
decision.sqlcmdpreprocessor.ad64c739b375
decision.sqlcmdpreprocessor.c5cdd8a2e4fc
decision.sqlcmdpreprocessor.de5853587de4
decision.sqlcmdpreprocessor.e4f9754bfc0f
decision.sqlcmdpreprocessor.e54431aeee9f
decision.sqlcmdpreprocessor.fa078ca0d9d8
decision.sqlsafetyanalyzer.4218633f7602
decision.sqlsafetyanalyzer.439066d60f75
decision.sqlsafetyanalyzer.46782bf7eabc
decision.sqlsafetyanalyzer.49663d58a61f
decision.sqlsafetyanalyzer.4f3f9c0adcbb
decision.sqlsafetyanalyzer.6f5462ce43cc
decision.sqlsafetyanalyzer.77a1bb8e062d
decision.sqlsafetyanalyzer.78641c391790
decision.sqlsafetyanalyzer.88d6f3decb5b
decision.sqlsafetyanalyzer.aefdb7739a1c
decision.sqlsafetyanalyzer.b152b922c28b
decision.sqlsafetyanalyzer.c6873cf3a5b8
decision.sqlsafetyanalyzer.d99005c18a45
decision.sqlsafetyanalyzer.dee779564254
decision.sqlsafetyanalyzer.e0d417378d7c
decision.sqlsafetyanalyzer.e80d782ca174
decision.sqlsafetyanalyzer.e9bc2d39dad0
decision.sqlsafetyanalyzer.ef39b4369094
decision.statementwalker.00dbf5f8bab4
decision.statementwalker.00dbf5f8bab4.2
decision.statementwalker.01d41b979efe
decision.statementwalker.022b6a59792b
decision.statementwalker.03572d3d2158
decision.statementwalker.0360762d518e
decision.statementwalker.0360ae29648d
decision.statementwalker.0385d2495d6a
decision.statementwalker.03b84cf37e85
decision.statementwalker.0449bf58a254
decision.statementwalker.0688799c9da3
decision.statementwalker.0843ce4c74c7
decision.statementwalker.089e564865c3
decision.statementwalker.08f0f8ab78c1
decision.statementwalker.09834f70ef1b
decision.statementwalker.09d98c2ee9b0
decision.statementwalker.0a1c3c5389d1
decision.statementwalker.0a76f853e439
decision.statementwalker.0a79f25193e6
decision.statementwalker.0a8160e3644f
decision.statementwalker.0b4b60c3f1e8
decision.statementwalker.0bf8be0fce5b
decision.statementwalker.0d8de92a5e81
decision.statementwalker.0e1f39723573
decision.statementwalker.0e52a5ff63a1
decision.statementwalker.0e5ee2d12d9c
decision.statementwalker.0f92fb1220e4
decision.statementwalker.0fa6c2d6e87e
decision.statementwalker.10741931018f
decision.statementwalker.1133e519ce8d
decision.statementwalker.11db6ba7723a
decision.statementwalker.11fc2b7e6c55
decision.statementwalker.11fc2b7e6c55.2
decision.statementwalker.121fc1061082
decision.statementwalker.1309713b8b4b
decision.statementwalker.152180ca6977
decision.statementwalker.15314f76e3dd
decision.statementwalker.154bc23c2af2
decision.statementwalker.16fb3602cc9b
decision.statementwalker.17ee7f81c74f
decision.statementwalker.1a6ca28b2c12
decision.statementwalker.1a89a8ee22ec
decision.statementwalker.1aa6c1137847
decision.statementwalker.1bccbeb4ba62
decision.statementwalker.1bf1ec3336db
decision.statementwalker.1bf1ec3336db.2
decision.statementwalker.1c64799f82ac
decision.statementwalker.1c64799f82ac.2
decision.statementwalker.1c6e606005ea
decision.statementwalker.1f4a5c26df83
decision.statementwalker.1f86402f7ae9
decision.statementwalker.21c87910f3e3
decision.statementwalker.22134e1a4890
decision.statementwalker.23ccb72cc1f4
decision.statementwalker.25464337a4bc
decision.statementwalker.2723f3988307
decision.statementwalker.27ac38512da8
decision.statementwalker.27cc94ef2bdf
decision.statementwalker.28a36f5d7fed
decision.statementwalker.28edad8ddf0b
decision.statementwalker.2cc330644ab4
decision.statementwalker.2d04dc9235d2
decision.statementwalker.2f130852440a
decision.statementwalker.2f28e6ea7000
decision.statementwalker.2f334801ff86
decision.statementwalker.2f528c177a05
decision.statementwalker.2f5c79a038cc
decision.statementwalker.2f8a653e90aa
decision.statementwalker.300503ee880e
decision.statementwalker.302bddb3ba2b
decision.statementwalker.30553137a59a
decision.statementwalker.3076308c9abe
decision.statementwalker.30b75bfb1453
decision.statementwalker.30d0d281e4a1
decision.statementwalker.3128c20b50f4
decision.statementwalker.315d6319ee3d
decision.statementwalker.3273b6126762
decision.statementwalker.3310d0b1402c
decision.statementwalker.331ca50f38bc
decision.statementwalker.33881e85a174
decision.statementwalker.33a1117c4c5b
decision.statementwalker.34f325ff3f1d
decision.statementwalker.35dad436dc9f
decision.statementwalker.365e9154adb8
decision.statementwalker.36639951dae5
decision.statementwalker.367bdb7f4dd1
decision.statementwalker.375a2a0f3d37
decision.statementwalker.379cd1602511
decision.statementwalker.382681fe7e53
decision.statementwalker.387d52a314d9
decision.statementwalker.399b6374d3ab
decision.statementwalker.39c2cfd20a2b
decision.statementwalker.39f591587de4
decision.statementwalker.3a931988d40f
decision.statementwalker.3b35c1ad03be
decision.statementwalker.3b532de05e31
decision.statementwalker.3c7f3e50d387
decision.statementwalker.3c8cf000c490
decision.statementwalker.3df4b26ce74b
decision.statementwalker.3e116a3c92dc
decision.statementwalker.3e116a3c92dc.2
decision.statementwalker.3fccb51a2acc
decision.statementwalker.3fcd1780f61d
decision.statementwalker.4052cd501c15
decision.statementwalker.411e55a90f3a
decision.statementwalker.42df4b579858
decision.statementwalker.43ab37efe3c3
decision.statementwalker.448ba9706674
decision.statementwalker.45395ca9dbfc
decision.statementwalker.45f1ee8eb43c
decision.statementwalker.46675bcfd3c3
decision.statementwalker.46b4c93a8016
decision.statementwalker.4734c120fc43
decision.statementwalker.478a3ea68c4c
decision.statementwalker.486c62d34f95
decision.statementwalker.489ad2904a1b
decision.statementwalker.49a465b88019
decision.statementwalker.49b1e45917ba
decision.statementwalker.49bb9bba9041
decision.statementwalker.49bee2aef464
decision.statementwalker.4e5f43d07b0f
decision.statementwalker.4ea6b24ceea7
decision.statementwalker.4eb6bac0f99c
decision.statementwalker.4efab30f0e57
decision.statementwalker.4f49384fdee5
decision.statementwalker.5036246874bb
decision.statementwalker.5356fbc5561c
decision.statementwalker.545894ba04af
decision.statementwalker.549a42f30f32
decision.statementwalker.54c9c7d4b43e
decision.statementwalker.577dd7c13434
decision.statementwalker.57969b13e441
decision.statementwalker.57ad9177900b
decision.statementwalker.580db1b2e15b
decision.statementwalker.5904e0027d0c
decision.statementwalker.5bd0a4dc5c96
decision.statementwalker.5e93c9c5648e
decision.statementwalker.5eb5784965b2
decision.statementwalker.5ee0ce2a8480
decision.statementwalker.5f2e0069fc55
decision.statementwalker.5fbc6a5aa453
decision.statementwalker.5fef1ad9a4ef
decision.statementwalker.5ff472fa47fc
decision.statementwalker.5ffaeb5e8ad0
decision.statementwalker.6045810f0bda
decision.statementwalker.6102dd41e5a5
decision.statementwalker.6132e2a8f54a
decision.statementwalker.630b38fae951
decision.statementwalker.635bd1339089
decision.statementwalker.644e9715202b
decision.statementwalker.6596dc4468aa
decision.statementwalker.659e57004a51
decision.statementwalker.663153526a43
decision.statementwalker.6684508dda2f
decision.statementwalker.66f080e109bb
decision.statementwalker.6802d3f2ab44
decision.statementwalker.68bd289a5ed8
decision.statementwalker.68be163986cf
decision.statementwalker.6930519d482e
decision.statementwalker.69a089b14f89
decision.statementwalker.6ac2f2f42563
decision.statementwalker.6ae8a582e19e
decision.statementwalker.6c0b381c4665
decision.statementwalker.6c3b99d86128
decision.statementwalker.6cd3f415518d
decision.statementwalker.6cd3f415518d.10
decision.statementwalker.6cd3f415518d.11
decision.statementwalker.6cd3f415518d.12
decision.statementwalker.6cd3f415518d.13
decision.statementwalker.6cd3f415518d.14
decision.statementwalker.6cd3f415518d.15
decision.statementwalker.6cd3f415518d.16
decision.statementwalker.6cd3f415518d.17
decision.statementwalker.6cd3f415518d.18
decision.statementwalker.6cd3f415518d.19
decision.statementwalker.6cd3f415518d.2
decision.statementwalker.6cd3f415518d.20
decision.statementwalker.6cd3f415518d.21
decision.statementwalker.6cd3f415518d.22
decision.statementwalker.6cd3f415518d.23
decision.statementwalker.6cd3f415518d.24
decision.statementwalker.6cd3f415518d.25
decision.statementwalker.6cd3f415518d.26
decision.statementwalker.6cd3f415518d.27
decision.statementwalker.6cd3f415518d.28
decision.statementwalker.6cd3f415518d.29
decision.statementwalker.6cd3f415518d.3
decision.statementwalker.6cd3f415518d.30
decision.statementwalker.6cd3f415518d.31
decision.statementwalker.6cd3f415518d.32
decision.statementwalker.6cd3f415518d.33
decision.statementwalker.6cd3f415518d.34
decision.statementwalker.6cd3f415518d.35
decision.statementwalker.6cd3f415518d.36
decision.statementwalker.6cd3f415518d.37
decision.statementwalker.6cd3f415518d.38
decision.statementwalker.6cd3f415518d.4
decision.statementwalker.6cd3f415518d.5
decision.statementwalker.6cd3f415518d.6
decision.statementwalker.6cd3f415518d.7
decision.statementwalker.6cd3f415518d.8
decision.statementwalker.6cd3f415518d.9
decision.statementwalker.6e1ca61daa3e
decision.statementwalker.6eb30d546372
decision.statementwalker.6ecc826240e5
decision.statementwalker.6ed1bea99dff
decision.statementwalker.6ffd52fb6fd3
decision.statementwalker.70886b1c00ef
decision.statementwalker.709acd6748a4
decision.statementwalker.70ba4e1a9970
decision.statementwalker.7102e4a8670a
decision.statementwalker.719d6f3a6558
decision.statementwalker.737a6270bd39
decision.statementwalker.73c8355e029f
decision.statementwalker.741ea9b69969
decision.statementwalker.743f8d71bace
decision.statementwalker.745104112757
decision.statementwalker.7483b60118e6
decision.statementwalker.7483cd29bb6f
decision.statementwalker.75a08d8a5ab2
decision.statementwalker.76247ac806e9
decision.statementwalker.762d2fcbde77
decision.statementwalker.762d2fcbde77.2
decision.statementwalker.762d2fcbde77.3
decision.statementwalker.762d2fcbde77.4
decision.statementwalker.762d2fcbde77.5
decision.statementwalker.77a98961ccb9
decision.statementwalker.79b29008db88
decision.statementwalker.79def1790357
decision.statementwalker.7c3a302bc675
decision.statementwalker.7c64a1662ae2
decision.statementwalker.7ca75d544aff
decision.statementwalker.7d934996c6f1
decision.statementwalker.7dcc5524fa23
decision.statementwalker.7e4f72b2dc77
decision.statementwalker.7eb69b7e5c11
decision.statementwalker.8274e2c3a192
decision.statementwalker.82d17a82bebc
decision.statementwalker.847a9dd79a41
decision.statementwalker.849b742c7c6c
decision.statementwalker.85e513ae4c65
decision.statementwalker.85f7d2b66b61
decision.statementwalker.868af32a4140
decision.statementwalker.892c2554b614
decision.statementwalker.89910da5e652
decision.statementwalker.8a27dafda142
decision.statementwalker.8a4b2d944d8a
decision.statementwalker.8aabff260f02
decision.statementwalker.8b41b91cb31f
decision.statementwalker.8c18919768c7
decision.statementwalker.8cd591ef0315
decision.statementwalker.8d80d3222c20
decision.statementwalker.8dae1f70a638
decision.statementwalker.8e343981b166
decision.statementwalker.8e4641113917
decision.statementwalker.91729f44e96c
decision.statementwalker.91924c20f5d8
decision.statementwalker.91cac0daf892
decision.statementwalker.924873d6ade9
decision.statementwalker.925a42407f23
decision.statementwalker.9280a1cd1a13
decision.statementwalker.92b6ecf879b6
decision.statementwalker.9302fb33f7a9
decision.statementwalker.9330ab601250
decision.statementwalker.9539d79cccba
decision.statementwalker.95ba8b0218a9
decision.statementwalker.96527b4a9725
decision.statementwalker.96b8c4ee5401
decision.statementwalker.9723d0018502
decision.statementwalker.975a9864229e
decision.statementwalker.995fea0dccc9
decision.statementwalker.99701b9db3ad
decision.statementwalker.9a344ae2ff12
decision.statementwalker.9a37c7704089
decision.statementwalker.9a82d600544d
decision.statementwalker.9a82d600544d.10
decision.statementwalker.9a82d600544d.11
decision.statementwalker.9a82d600544d.2
decision.statementwalker.9a82d600544d.3
decision.statementwalker.9a82d600544d.4
decision.statementwalker.9a82d600544d.5
decision.statementwalker.9a82d600544d.6
decision.statementwalker.9a82d600544d.7
decision.statementwalker.9a82d600544d.8
decision.statementwalker.9a82d600544d.9
decision.statementwalker.9b13022ed71b
decision.statementwalker.9bc3b5a10c13
decision.statementwalker.9c20b4a62fcb
decision.statementwalker.9d3327dd9076
decision.statementwalker.9ef38c17539c
decision.statementwalker.9f43409c77e4
decision.statementwalker.a04e25bdeffb
decision.statementwalker.a19c3931817e
decision.statementwalker.a19f541e3d62
decision.statementwalker.a1be170855fb
decision.statementwalker.a237c9d5b85f
decision.statementwalker.a397567aa093
decision.statementwalker.a476f968efc6
decision.statementwalker.a527e9efb2bb
decision.statementwalker.a605f365ceff
decision.statementwalker.a665083edf9a
decision.statementwalker.a78be7ca2c29
decision.statementwalker.a87ea92ca564
decision.statementwalker.a8d6658e3aea
decision.statementwalker.a8f8d0256df0
decision.statementwalker.aa3285bcc519
decision.statementwalker.aa33bb27d622
decision.statementwalker.aaac602a548a
decision.statementwalker.aaccc7747c9b
decision.statementwalker.aacf4ca3064a
decision.statementwalker.aba327ad7fc7
decision.statementwalker.abe4df6e1420
decision.statementwalker.ae0190a0696c
decision.statementwalker.ae667249c4b5
decision.statementwalker.aeb246575499
decision.statementwalker.b0909ed80751
decision.statementwalker.b1168f29f4c6
decision.statementwalker.b17a57d61a0e
decision.statementwalker.b1d6b1b8d63f
decision.statementwalker.b29aee39f6c1
decision.statementwalker.b2bbc4b70dba
decision.statementwalker.b3272661bf81
decision.statementwalker.b4a5210175c7
decision.statementwalker.b4ade47179eb
decision.statementwalker.b5aa766ba3e4
decision.statementwalker.b60372e6207c
decision.statementwalker.b67a35791ac1
decision.statementwalker.b6b8f5f20326
decision.statementwalker.b6d76850158f
decision.statementwalker.b75e23763db6
decision.statementwalker.b77a14a7ff46
decision.statementwalker.b7abc4000ba4
decision.statementwalker.b7e113f53f00
decision.statementwalker.b81963afdcce
decision.statementwalker.b880cab57611
decision.statementwalker.b8b90248f8bb
decision.statementwalker.b97ed9bf7a45
decision.statementwalker.bb0459330389
decision.statementwalker.bb977ae99599
decision.statementwalker.bccadc4a9e0b
decision.statementwalker.bec7bbea7b85
decision.statementwalker.bed3879dc81a
decision.statementwalker.bf06655418aa
decision.statementwalker.bf9171d2e31a
decision.statementwalker.c0a50d41c72a
decision.statementwalker.c1a0c0414114
decision.statementwalker.c1be305a32fd
decision.statementwalker.c27330f21008
decision.statementwalker.c32ca1c6208b
decision.statementwalker.c3fd005bde23
decision.statementwalker.c43c6380e261
decision.statementwalker.c57d2592cdf2
decision.statementwalker.c59ae1f500b8
decision.statementwalker.c5e1b417159d
decision.statementwalker.c68bafcd6beb
decision.statementwalker.c7826fd7c566
decision.statementwalker.c9ae3a9d5fe4
decision.statementwalker.ca408327f213
decision.statementwalker.ca81ee93b0f7
decision.statementwalker.cab7d3f6e238
decision.statementwalker.caca601981cc
decision.statementwalker.cb121b348534
decision.statementwalker.cbdb644c7398
decision.statementwalker.cd1fef1f6746
decision.statementwalker.cebe228f6f9d
decision.statementwalker.cf10b0aa63a2
decision.statementwalker.d0d3f5a8f1b0
decision.statementwalker.d124beaf7316
decision.statementwalker.d183c6d829fb
decision.statementwalker.d219ce031bef
decision.statementwalker.d254e09ed4a7
decision.statementwalker.d31f5b2041c2
decision.statementwalker.d38dc1f6dea6
decision.statementwalker.d3b826729a22
decision.statementwalker.d42f47ba42d9
decision.statementwalker.d4685a20ef79
decision.statementwalker.d477631eb0df
decision.statementwalker.d477631eb0df.2
decision.statementwalker.d4a75fd407d7
decision.statementwalker.d62bc568c479
decision.statementwalker.d68a7de540a1
decision.statementwalker.d69a8e16f8c7
decision.statementwalker.d6cace3f0326
decision.statementwalker.d7c45edb7771
decision.statementwalker.d80c2fd184f5
decision.statementwalker.d850a5977255
decision.statementwalker.d89b4bfcbbd4
decision.statementwalker.d8d4fdbbc867
decision.statementwalker.d965bedbbef7
decision.statementwalker.d9b66eb39171
decision.statementwalker.da1fad7eeb8d
decision.statementwalker.da61185e4731
decision.statementwalker.db5d157782b3
decision.statementwalker.db83e832cdcd
decision.statementwalker.dc371e2d77b1
decision.statementwalker.dcf2a19d39e8
decision.statementwalker.dcf2d0a09c6c
decision.statementwalker.df4303196ee6
decision.statementwalker.dfdd074db2b5
decision.statementwalker.dfe3fb6d12ff
decision.statementwalker.e00ad6909b71
decision.statementwalker.e0a068e018d2
decision.statementwalker.e21b35aeb387
decision.statementwalker.e293850368de
decision.statementwalker.e3c7d89f6cf5
decision.statementwalker.e4ba6a21a06f
decision.statementwalker.e7c2bb1c672e
decision.statementwalker.e7fbe9c966e7
decision.statementwalker.e825708d1fc9
decision.statementwalker.e849d76842b8
decision.statementwalker.e918b276866a
decision.statementwalker.eb2d28ab7f4f
decision.statementwalker.eb7da19d3756
decision.statementwalker.eb9fc55bcd36
decision.statementwalker.ec8a0cef3700
decision.statementwalker.edebadd8bd4e
decision.statementwalker.ee2692b3224b
decision.statementwalker.ef6427d69e0d
decision.statementwalker.efb81354f6bc
decision.statementwalker.f097dd178e0c
decision.statementwalker.f0b630e5d5e2
decision.statementwalker.f1600b51b11e
decision.statementwalker.f178e208905c
decision.statementwalker.f26ea6f7439d
decision.statementwalker.f39f730ed385
decision.statementwalker.f3ca900bd1e3
decision.statementwalker.f4370f634501
decision.statementwalker.f558929e3f3a
decision.statementwalker.f7323b1cc370
decision.statementwalker.f75a4585a86e
decision.statementwalker.f7ddb3d2993b
decision.statementwalker.f7ddb3d2993b.2
decision.statementwalker.f835f189a77f
decision.statementwalker.f8b5ce621c0e
decision.statementwalker.f8ea7cf35f86
decision.statementwalker.f9752b4f2a48
decision.statementwalker.f9b50d4a5926
decision.statementwalker.fa3d26fe33bc
decision.statementwalker.fad99ad06d90
decision.statementwalker.fb50ed3d472d
decision.statementwalker.fc577780e5ac
decision.statementwalker.fcf19ff848e9
decision.statementwalker.fd7c1aa4d918
decision.targetresolver.008950b37ba5
decision.targetresolver.0202024c3497
decision.targetresolver.0401af0cd7ef
decision.targetresolver.06a26814f976
decision.targetresolver.09d6fb8c78fe
decision.targetresolver.0abebc8a6f0b
decision.targetresolver.0f21d53d0b06
decision.targetresolver.10b74d2dac65
decision.targetresolver.16c1e13f40bc
decision.targetresolver.177d5251d30f
decision.targetresolver.186f1021c078
decision.targetresolver.253c242478f6
decision.targetresolver.308d18deefdd
decision.targetresolver.33e89da144ce
decision.targetresolver.376b8fb2c314
decision.targetresolver.3c98a4f70c4e
decision.targetresolver.3deda599e038
decision.targetresolver.3f680bbf0677
decision.targetresolver.424e36309078
decision.targetresolver.43032f557274
decision.targetresolver.522622ef5f29
decision.targetresolver.6052be62a9cc
decision.targetresolver.6618d4ea7df5
decision.targetresolver.670a05c5cbe1
decision.targetresolver.68d391036ee3
decision.targetresolver.68eeea6e5d78
decision.targetresolver.6a3d88cf004d
decision.targetresolver.6de02e556886
decision.targetresolver.71d169f5a24c
decision.targetresolver.7647da6dc85e
decision.targetresolver.77c753b522d4
decision.targetresolver.78bfd7f09aae
decision.targetresolver.7c20be9cc7c4
decision.targetresolver.808a64aa4a3c
decision.targetresolver.830be71cc7dc
decision.targetresolver.84f4572646ee
decision.targetresolver.87000f86779e
decision.targetresolver.8984aa375b20
decision.targetresolver.8c3c6ceae5c7
decision.targetresolver.8c3c6ceae5c7.2
decision.targetresolver.8e1850ea69bd
decision.targetresolver.91b2c4971774
decision.targetresolver.91b2c4971774.2
decision.targetresolver.93b062aee62f
decision.targetresolver.956f62dbfeea
decision.targetresolver.97eb43d62506
decision.targetresolver.98d703307786
decision.targetresolver.99ad40fdb0e4
decision.targetresolver.a1780a63f9ea
decision.targetresolver.a6384cc364bb
decision.targetresolver.a902c5de2577
decision.targetresolver.a99af18bd0dd
decision.targetresolver.ac1a9672b74c
decision.targetresolver.af2017297ca1
decision.targetresolver.b107d485c7a7
decision.targetresolver.bc98db749560
decision.targetresolver.c96f00e0b5b8
decision.targetresolver.c9db724d7a74
decision.targetresolver.cc5d0e398658
decision.targetresolver.cd79a4dca3a7
decision.targetresolver.d7e10a82b1ec
decision.targetresolver.deacb97679b4
decision.targetresolver.e1c27c2fcc9d
decision.targetresolver.e220afd82793
decision.targetresolver.e9d2fe94ef26
decision.targetresolver.ee0b654f679c
decision.targetresolver.ef344249d866
decision.targetresolver.f2055dc2c016
decision.targetresolver.f4dd5871e001
decision.targetresolver.f5b19d765a5e
decision.targetresolver.fd8806473970
decision.textposition.1466827fa9f1
decision.textposition.3a97171ecd06
decision.textposition.521d4d501f9d
decision.textposition.7399b05460d1
decision.textposition.8ab25e185e28
decision.textposition.9634a5b7b958
decision.textposition.9634a5b7b958.2
decision.textposition.ce6f0c247796
decision.textposition.efb422d8724d
decision.textposition.f6737678b334
decision.verdict.110608503abb
decision.verdict.4ef550b4118b
decision.verdict.5f6a563604d1
decision.verdict.7b435361c0d5
decision.verdict.94c040bb0e50
decision.verdict.ab5c149c795a
decision.verdict.af02b062d38a
decision.verdict.cdd4baa9c52c
decision.verdict.ddcb78b46dac
decision.verdict.f860e22e8b33
entry.limit
exec.empty
exec.missing
green-statement.cursorstatement
green-statement.raiseerrorlegacystatement
green-statement.setonoffstatement
green-statement.transactionstatement
limits.cancel
resolve.no-base
unknown-statement.backupstatement
walk.depth
walk.drop-index
walk.guess-target
walk.repeat
```

## Reproduce

See README.md in this directory for collection commands; pass the two resulting coverage.cobertura.xml paths to `python tools/verification/report_coverage.py <baseline> <augmented>`. Report generation uses apply_patch. Raw coverage files live outside the repository and are not committed.
