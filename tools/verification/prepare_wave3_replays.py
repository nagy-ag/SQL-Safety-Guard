import json
from pathlib import Path

from generate_inventory import apply_file

ROOT = Path(__file__).resolve().parents[2]
CASES = ROOT / 'tests/SqlSafetyGuard.ServerHarness/Campaign/cases'


def main():
    repaired = []
    for file in sorted((CASES / 'independent').rglob('*.jsonl')):
        rows = [json.loads(line) for line in file.read_text(encoding='utf-8-sig').split('\n') if line.strip()]
        changed = False
        for row in rows:
            if row.get('wave3PrerequisiteRepair'):
                continue
            identity = row['id']
            original = row['sql']
            setup = row.get('setup', '')
            if identity == 'ind-metadata-spaceused-extra':
                row['sql'] = original.replace("N'false',N'false'", "N'false',N'ALL'")
            elif identity in ('ind-remote-openrowset-read', 'ind-remote-opendatasource'):
                connection = 'Server=127.0.0.1,1433;User ID=SgRun_AdhocProbe;Password=Wave3_Disposable_Only_171!;Encrypt=Optional;TrustServerCertificate=yes;'
                row['sql'] = original.replace('Server=127.0.0.1,1433;Trusted_Connection=yes;', connection).replace('Server=127.0.0.1,1433;Integrated Security=SSPI;', connection)
                row['setup'] = setup + " EXEC sys.sp_configure N'show advanced options',1; RECONFIGURE; EXEC sys.sp_configure N'Ad Hoc Distributed Queries',1; RECONFIGURE; IF SUSER_ID(N'SgRun_AdhocProbe') IS NULL CREATE LOGIN SgRun_AdhocProbe WITH PASSWORD=N'Wave3_Disposable_Only_171!',CHECK_POLICY=OFF;"
                for database in ['Core', 'Sales', 'Audit', 'Archive']:
                    row['setup'] += ' USE [{{' + database + '}}]; CREATE USER SgRun_AdhocProbe FOR LOGIN SgRun_AdhocProbe; ALTER ROLE db_owner ADD MEMBER SgRun_AdhocProbe;'
                row['setup'] += ' USE [{{Core}}];'
            elif identity == 'ind-metadata-sp_releaseapplock':
                row['sql'] = "EXEC sys.sp_getapplock @Resource=N'independent',@LockMode=N'Exclusive',@LockOwner=N'Session'; " + original
            elif identity == 'ind-persistent-targets-partition-index':
                row['setup'] = setup + ' CREATE INDEX IndPartitionIndex ON staging.Events(Id);'
            elif identity == 'ind-session-close-symmetric':
                row['sql'] = "OPEN SYMMETRIC KEY IndKey DECRYPTION BY PASSWORD='IndKey_171!'; " + original
            elif identity.startswith('ind-physical-bulk-'):
                row['sql'] = original.replace('/tmp/independent-no-such-file.csv', '/var/opt/mssql/data/sqlguard-independent.csv').replace('/tmp/independent-no-such-file.txt', '/var/opt/mssql/data/sqlguard-independent.csv')
            elif identity == 'ind-dbcc-help-noargs':
                row['sql'] = "DBCC HELP(N'CHECKTABLE');"
            elif identity == 'ind-dbcc-showstatistics':
                row['setup'] = setup + ' CREATE STATISTICS IndValueStats ON dbo.IndTarget(Value);'
                row['sql'] = "DBCC SHOW_STATISTICS(N'dbo.IndTarget',N'IndValueStats') WITH NO_INFOMSGS;"
            elif identity == 'ind-dbcc-extentinfo':
                row['sql'] = "DECLARE @object int=OBJECT_ID(N'dbo.IndTarget'); DBCC EXTENTINFO(0,@object,-1) WITH NO_INFOMSGS;"
            elif identity.startswith('ind-dbcc-repair-'):
                row['setup'] = setup + " ALTER DATABASE [{{Core}}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"
            elif identity.startswith('ind-metadata-msdb-') and identity != 'ind-metadata-msdb-sp_send_dbmail':
                procedure = identity.removeprefix('ind-metadata-msdb-')
                job = 'SgRun_' + procedure
                if procedure == 'sp_delete_backuphistory':
                    row['sql'] = "EXEC msdb.dbo.sp_delete_backuphistory @oldest_date='19000101';"
                elif procedure == 'sp_add_job':
                    row['sql'] = f"EXEC msdb.dbo.sp_add_job @job_name=N'{job}';"
                elif procedure == 'sp_add_schedule':
                    row['sql'] = f"EXEC msdb.dbo.sp_add_schedule @schedule_name=N'{job}',@freq_type=1,@active_start_date=20990101;"
                else:
                    row['setup'] = setup + f" EXEC msdb.dbo.sp_add_job @job_name=N'{job}';"
                    arguments = f"@job_name=N'{job}'"
                    if procedure in ('sp_delete_jobstep', 'sp_update_jobstep', 'sp_help_jobstep', 'sp_start_job', 'sp_stop_job'):
                        row['setup'] += f" EXEC msdb.dbo.sp_add_jobstep @job_name=N'{job}',@step_name=N'owned',@subsystem=N'TSQL',@command=N'WAITFOR DELAY ''00:00:30'';';"
                    if procedure in ('sp_start_job', 'sp_stop_job'):
                        row['setup'] += f" EXEC msdb.dbo.sp_add_jobserver @job_name=N'{job}';"
                    if procedure == 'sp_stop_job':
                        row['setup'] += f" EXEC msdb.dbo.sp_start_job @job_name=N'{job}'; WAITFOR DELAY '00:00:02';"
                    if procedure in ('sp_delete_jobstep', 'sp_update_jobstep'):
                        arguments += ',@step_id=1'
                    if procedure == 'sp_update_job':
                        arguments += ',@description=N"wave3 owned"'.replace('"', "'")
                    if procedure == 'sp_update_jobstep':
                        arguments += ',@step_name=N\'updated\''
                    if procedure == 'sp_add_jobstep':
                        arguments += ",@step_name=N'owned',@subsystem=N'TSQL',@command=N'SELECT 1;'"
                    if procedure == 'sp_add_jobschedule':
                        arguments += f",@name=N'{job}_schedule',@freq_type=1,@active_start_date=20990101"
                    if procedure == 'sp_attach_schedule':
                        row['setup'] += f" EXEC msdb.dbo.sp_add_schedule @schedule_name=N'{job}_schedule',@freq_type=1,@active_start_date=20990101;"
                        arguments += f",@schedule_name=N'{job}_schedule'"
                    row['sql'] = f'EXEC msdb.dbo.{procedure} {arguments};'
            if row['sql'] != original or row.get('setup', '') != setup:
                row.setdefault('wave3OriginalSql', original)
                row['wave3PrerequisiteRepair'] = 'Owned server fixture/valid arguments; original SQL retained for review. No original failure relabeled as a pass.'
                row.pop('localObservation', None)
                repaired.append(identity)
                changed = True
        if changed:
            apply_file(file, ''.join(json.dumps(row, ensure_ascii=False, separators=(',', ':')) + '\n' for row in rows))
    print(json.dumps({'repaired': repaired}, indent=2))


if __name__ == '__main__':
    main()
