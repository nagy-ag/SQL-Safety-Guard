import argparse
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def fixture(identity, sql, verify, before=0, after=1, setup="", kinds=None, verdict="RealWrite", **extra):
    return dict(id=identity, statementTypes=kinds or ["ExecuteStatement"], setupSql=setup, sql=sql,
                expectedVerdict=verdict, verifySql=verify, beforeScalar=before, afterScalar=after,
                cleanupSql="", **extra)


def generate():
    monitor = fixture("wave3-monitor", "EXEC master.dbo.sp_monitor;",
                      "SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM master.dbo.spt_monitor;",
                      "observe-before", "observe-after", setup="WAITFOR DELAY '00:00:01';",
                      verdict="RealWrite", observeChange=True)
    log_count = "CREATE TABLE #logs(LogDate datetime,ProcessInfo nvarchar(100),Text nvarchar(max)); INSERT #logs EXEC master.dbo.xp_readerrorlog 0,1,N'Wave3OwnedLogMarker'; SELECT COUNT(*) FROM #logs; DROP TABLE #logs;"
    logging = fixture("wave3-raiserror-log", "RAISERROR(N'Wave3OwnedLogMarker',10,1) WITH LOG;", log_count,
                      "observe-before", "observe-after", kinds=["RaiseErrorStatement"], verdict="ReadOnly", observeChange=True)
    availability = "USE master; CREATE AVAILABILITY GROUP Wave3OwnedAg WITH (CLUSTER_TYPE = NONE) FOR REPLICA ON N'{{ServerName}}' WITH (ENDPOINT_URL=N'TCP://127.0.0.1:5022',AVAILABILITY_MODE=ASYNCHRONOUS_COMMIT,FAILOVER_MODE=MANUAL,SEEDING_MODE=MANUAL);"
    ag_count = "SELECT COUNT(*) FROM sys.availability_groups WHERE name=N'Wave3OwnedAg';"
    wrappers = ["EXEC dbo.sp_monitor;", "EXEC sp_monitor;", "EXEC [master].[dbo].[sp_monitor];", "EXEC sys.sp_executesql N'EXEC master.dbo.sp_monitor;';", "EXEC(N'EXEC master.dbo.sp_monitor;');", "BEGIN EXEC master.dbo.sp_monitor; END;", "IF 1=1 EXEC master.dbo.sp_monitor;", "DECLARE @result int; EXEC @result=master.dbo.sp_monitor;"]
    monitors = [monitor] + [fixture('wave3-monitor-wrapper-' + str(index), sql, monitor['verifySql'], 'observe-before', 'observe-after', setup="WAITFOR DELAY '00:00:01';", verdict='RealWrite', observeChange=True) for index, sql in enumerate(wrappers, 1)]
    probes = monitors + [logging,
              fixture("wave3-ag-create", availability, ag_count, kinds=["CreateAvailabilityGroupStatement"]),
              fixture("wave3-ag-alter", "USE master; ALTER AVAILABILITY GROUP Wave3OwnedAg SET (AUTOMATED_BACKUP_PREFERENCE = SECONDARY);", "SELECT CONVERT(int,automated_backup_preference) FROM sys.availability_groups WHERE name=N'Wave3OwnedAg';", 1, 2, setup=availability.replace('CLUSTER_TYPE = NONE', 'CLUSTER_TYPE = NONE, AUTOMATED_BACKUP_PREFERENCE = PRIMARY'), kinds=["AlterAvailabilityGroupStatement"]),
              fixture("wave3-ag-drop", "USE master; DROP AVAILABILITY GROUP Wave3OwnedAg;", ag_count, 1, 0, setup=availability, kinds=["DropAvailabilityGroupStatement"])]
    for probe in probes[len(monitors)+1:]:
        probe["cleanupSql"] = "IF EXISTS(SELECT 1 FROM sys.availability_groups WHERE name=N'Wave3OwnedAg') DROP AVAILABILITY GROUP Wave3OwnedAg;"
        probe["attemptOnly"] = True
    probes.append(fixture('wave3-query-notification-kill', 'KILL QUERY NOTIFICATION SUBSCRIPTION {{NotificationId}};',
                          'SELECT COUNT(*) FROM sys.dm_qn_subscriptions WHERE database_id=DB_ID();', 1, 0,
                          setup='CREATE TABLE dbo.NotifyTarget(Id int NOT NULL PRIMARY KEY, Value int NOT NULL); INSERT dbo.NotifyTarget VALUES(1,7);',
                          kinds=['KillQueryNotificationSubscriptionStatement'], queryNotification=True))
    model = "CREATE EXTERNAL MODEL Wave3Model WITH (LOCATION='/models/wave3-owned', API_FORMAT='OpenAI', MODEL_TYPE=EMBEDDINGS, MODEL='wave3-before');"
    model_count = "SELECT COUNT(*) FROM sys.external_models WHERE name=N'Wave3Model';"
    probes.extend([
        fixture('wave3-model-create', model, model_count, kinds=['CreateExternalModelStatement']),
        fixture('wave3-model-alter', "ALTER EXTERNAL MODEL Wave3Model SET (MODEL='wave3-after');", "SELECT model FROM sys.external_models WHERE name=N'Wave3Model';", 'wave3-before', 'wave3-after', setup=model, kinds=['AlterExternalModelStatement']),
        fixture('wave3-model-drop', 'DROP EXTERNAL MODEL Wave3Model;', model_count, 1, 0, setup=model, kinds=['DropExternalModelStatement'])])
    return dict(schemaVersion=1, fixtures=probes)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", required=True)
    args = parser.parse_args()
    Path(args.output).write_text(json.dumps(generate(), indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
