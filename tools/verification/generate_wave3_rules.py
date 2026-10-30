"""Generate focused semantic triples, without calling generic reads branch coverage."""
import json
from pathlib import Path
from generate_inventory import apply_file

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'tests/SqlSafetyGuard.ServerHarness/Campaign/cases/wave3'


def main():
    rules = json.loads((ROOT / 'docs/verification/rules/rules.json').read_text())['rules']
    wanted = {entry['ruleId'] for entry in json.loads((ROOT / 'tests/SqlSafetyGuard.ServerHarness/Campaign/cases/generated/manifest.json').read_text())['rules'] if entry['status'] == 'needs-focused-options-or-AST-probe'}
    rows = []
    dispositions = []

    def triple(identity, positive, negative='SELECT 1;', boundary=None, options=None, setup='', lane='host'):
        if 'dbo.Unresolved' in positive or 'dbo.Unresolved' in (boundary or ''):
            setup += " EXEC(N'CREATE PROCEDURE dbo.Unresolved AS SELECT 1;');"
        for variant, sql in [('positive', positive), ('negative', negative), ('boundary', boundary or ('BEGIN\n' + positive + '\nEND;'))]:
            row = dict(id='w3-' + identity + '-' + variant, sql=sql, setup=setup, ruleIds=[identity], family='focused-' + variant, principal='runner', seed=20261028)
            if identity == 'dbcc.fallback':
                row['principal'] = 'admin'
            if options:
                row['localProbeOptions'] = options
            if lane == 'isolated':
                row.update(principal='admin', privileged=True, executionLane='isolated')
            rows.append((lane, row))
        dispositions.append(dict(ruleId=identity, status='focused-triple-generated', note='Negative is a contrast, not evidence of this branch. Server outcomes must be joined by case id.'))

    general = {'contextinfo':'0x01', 'datefirst':'1', 'dateformat':'dmy', 'deadlockpriority':'LOW', 'language':"N'us_english'", 'locktimeout':'100', 'querygovernorcostlimit':'0'}
    general_tokens = {'contextinfo':'CONTEXT_INFO', 'deadlockpriority':'DEADLOCK_PRIORITY', 'locktimeout':'LOCK_TIMEOUT', 'querygovernorcostlimit':'QUERY_GOVERNOR_COST_LIMIT'}
    for rule in rules:
        identity = rule['id']
        if identity not in wanted:
            continue
        if identity.startswith('set-option.'):
            token = rule['sql']
            triple(identity, f'SET {token} ON;', f'SET {token} OFF;', f'SET {token} ON;\nGO\nSET {token} OFF;')
        elif identity.startswith('set-family.generalsetcommandtype.'):
            token = identity.rsplit('.', 1)[1]
            triple(identity, f'SET {general_tokens.get(token, token.upper())} {general[token]};')
        elif identity.startswith('set-family.setstatisticsoptions.'):
            token = identity.rsplit('.', 1)[1].upper()
            triple(identity, f'SET STATISTICS {token} ON; SELECT 1;', f'SET STATISTICS {token} OFF; SELECT 1;')
        elif identity.startswith('set-family.setoffsets.'):
            token = identity.rsplit('.', 1)[1].upper()
            triple(identity, f'SET OFFSETS {token} ON;', f'SET OFFSETS {token} OFF;')

    update = 'UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;'
    read = 'SELECT Value FROM dbo.Counter WHERE Id=1;'
    samples = {
        'aggregate.max': ('SELECT 1; ' + update, read, 'SELECT 1; EXEC dbo.Unresolved; ' + update),
        'aggregate.presentation': ('SELECT 1; ' + update, read, 'BEGIN ' + update + ' END;'),
        'verdict.severity': ('EXEC dbo.Unresolved; ' + update, read, 'SELECT 1; EXEC dbo.Unresolved;'),
        'entry.empty': ('', read, ' -- comment only\n'),
        'entry.settings': (update, read, update),
        'options.defaults': (read, update, 'EXEC dbo.Unresolved;'),
        'parser.newest': ('SELECT JSON_OBJECT(\'value\':1);', read, 'SELECT 1;\nGO\nSELECT 2;'),
        'position.offset': ('\n\n' + update, read, 'SELECT 1;\nGO\n' + update),
        'finding.metadata': (update, read, '/* write */\n' + update),
        'scope.batch-offset': ('SELECT 1;\nGO\n' + update, read, '\r\nGO\r\n' + update),
        'scope.newlines': ('SELECT 1;\r\nGO\r\n' + update, read, 'SELECT 1;\nGO\n' + update),
        'scope.dynamic': ("EXEC(N'" + update + "');", "EXEC(N'SELECT 1;');", "EXEC sys.sp_executesql N'" + update + "';"),
        'exec.dynamic': ("EXEC(N'" + update + "');", "EXEC(N'SELECT 1;');", "EXEC('SELECT 1;');"),
        'exec.sp-executesql': ("EXEC sys.sp_executesql N'" + update + "';", "EXEC sys.sp_executesql N'SELECT 1;';", "EXEC master.sys.sp_executesql N'SELECT 1;';"),
        'exec.namespace': ('EXEC dbo.sp_help;', 'EXEC sys.sp_help;', 'EXEC master.dbo.sp_help;'),
        'exec.ordinary': ('EXEC app.WriteOrder;', 'EXEC sys.sp_help;', 'EXEC [app].[WriteOrder];'),
        'exec.spaceused': ('EXEC sys.sp_spaceused @updateusage=N\'true\';', 'EXEC sys.sp_spaceused;', 'EXEC sys.sp_spaceused @updateusage=N\'false\';'),
        'exec.dispatch': ('EXEC app.WriteOrder;', 'EXEC sys.sp_help;', "EXEC(N'EXEC app.WriteOrder;');"),
        'txn.begin': ('BEGIN TRANSACTION; SELECT 1; ROLLBACK;', read, 'BEGIN TRANSACTION; COMMIT;'),
        'txn.commit': ('BEGIN TRANSACTION; COMMIT;', read, 'BEGIN TRANSACTION; SELECT 1; COMMIT;'),
        'txn.rollback': ('BEGIN TRANSACTION; ROLLBACK;', read, 'BEGIN TRANSACTION; SAVE TRANSACTION here; ROLLBACK TRANSACTION here; ROLLBACK;'),
        'txn.finish': ('BEGIN TRANSACTION; SELECT 1;', read, 'BEGIN TRANSACTION; SELECT 1; ROLLBACK;'),
        'txn.if': ('BEGIN TRANSACTION; IF 1=1 COMMIT;', read, 'IF 1=0 BEGIN TRANSACTION;'),
        'txn.merge': ('IF 1=1 BEGIN TRANSACTION; ELSE SELECT 1; ROLLBACK;', read, 'IF 1=0 BEGIN TRANSACTION; ELSE SELECT 1;'),
        'txn.implicit-start': ('SET IMPLICIT_TRANSACTIONS ON; ' + read, 'SET IMPLICIT_TRANSACTIONS OFF; ' + read, 'SET IMPLICIT_TRANSACTIONS ON; ' + read + ' COMMIT; SET IMPLICIT_TRANSACTIONS OFF;'),
        'walk.return': ('RETURN; ' + update, read, update + ' RETURN;'),
        'walk.while-zero': ('WHILE 1=0 BEGIN ' + update + ' END;', read, 'WHILE 1=0 BEGIN SELECT 1; END;'),
        'walk.select-into': ('SELECT 1 AS Id INTO #Focused;', read, 'SELECT 1 AS Id INTO dbo.Focused;'),
        'walk.output': ('CREATE TABLE dbo.WriteAudit(Id int); UPDATE dbo.Counter SET Value=Value+1 OUTPUT inserted.Id INTO dbo.WriteAudit(Id) WHERE Id=1;', read, 'UPDATE dbo.Counter SET Value=Value+1 OUTPUT inserted.Id WHERE Id=1;'),
        'walk.waitfor': ("WAITFOR DELAY '00:00:00';", read, "BEGIN WAITFOR DELAY '00:00:00'; END;"),
        'walk.throw': ("BEGIN TRY THROW 51000,N'focused',1; END TRY BEGIN CATCH SELECT ERROR_NUMBER(); END CATCH;", read, "BEGIN TRY THROW 51000,N'focused boundary',1; END TRY BEGIN CATCH SELECT ERROR_NUMBER(); END CATCH;"),
        'walk.rowfilter': (update, read, 'UPDATE dbo.Counter SET Value=Value+1 WHERE 1=0;'),
        'walk.dml-targets': (update, read, 'DELETE dbo.Counter WHERE Id=-1;'),
        'resolve.alias': ('UPDATE target SET Value=Value+1 FROM dbo.Counter target WHERE Id=1;', read, 'UPDATE target SET Value=Value+1 FROM dbo.Counter AS target WHERE 1=0;'),
        'resolve.join': ('UPDATE target SET Value=target.Value+1 FROM dbo.Counter target JOIN dbo.Counter source ON target.Id=source.Id WHERE target.Id=1;', read, 'UPDATE target SET Value=target.Value+1 FROM dbo.Counter target JOIN dbo.Counter source ON target.Id=source.Id WHERE 1=0;'),
        'resolve.variable': ('DECLARE @target TABLE(Id int); INSERT @target VALUES(1);', read, 'DECLARE @target TABLE(Id int); DELETE @target;'),
        'resolve.derived': ('UPDATE target SET Value=Value+1 FROM (SELECT Id,Value FROM dbo.Counter) target WHERE Id=1;', read, 'UPDATE target SET Value=Value+1 FROM (SELECT Id,Value FROM dbo.Counter) target WHERE 1=0;'),
    }
    for identity, values in samples.items():
        if identity in wanted:
            triple(identity, *values)
    additional = {
        'exec.empty': ("EXEC(N'');", read, "EXEC sys.sp_executesql N' -- empty';"),
        'walk.hidden': ('SELECT NEXT VALUE FOR app.OrderSequence;', read, 'SELECT NEXT VALUE FOR app.OrderSequence WHERE 1=0;'),
        'walk.fallback': ('CREATE TABLE dbo.FallbackFocused(Id int);', read, 'CREATE TABLE #FallbackFocused(Id int);'),
        'walk.parse': ("EXEC(N'SELECT FROM');", read, "EXEC(N'UPDATE');"),
        'dbcc.checkident': ("CREATE TABLE dbo.FocusIdentity(Id int IDENTITY); DBCC CHECKIDENT('dbo.FocusIdentity', RESEED,42);", read, "CREATE TABLE dbo.FocusIdentity(Id int IDENTITY); DBCC CHECKIDENT('dbo.FocusIdentity',NORESEED);"),
        'dbcc.clear': ('DBCC CLEANTABLE(0,\'dbo.Counter\',0);', read, 'DBCC CHECKDB WITH NO_INFOMSGS;'),
        'dbcc.fallback': ('DBCC SQLPERF(LOGSPACE);', read, 'DBCC CHECKDB WITH NO_INFOMSGS;'),
        'exec.missing': ('EXEC dbo.NoSuchProcedure;', read, 'BEGIN EXEC dbo.NoSuchProcedure; END;'),
        'green-statement.cursorstatement': ('DECLARE focusCursor CURSOR LOCAL FOR SELECT Id FROM dbo.Counter; OPEN focusCursor; CLOSE focusCursor; DEALLOCATE focusCursor;', read, 'DECLARE focusCursor CURSOR LOCAL FOR SELECT Id FROM dbo.Counter; DEALLOCATE focusCursor;'),
        'green-statement.setonoffstatement': ('SET NOCOUNT ON;', read, 'SET NOCOUNT OFF;'),
        'green-statement.transactionstatement': ('BEGIN TRAN; COMMIT;', read, 'BEGIN TRAN; ROLLBACK;'),
        'resolve.alias-priority': ('UPDATE target SET Value=target.Value+1 FROM dbo.Counter target JOIN app.Counter other ON target.Id=other.Id WHERE target.Id=1;', read, 'UPDATE target SET Value=target.Value+1 FROM dbo.Counter target WHERE 1=0;'),
        'resolve.cte-cache': ('WITH target AS(SELECT Id,Value FROM dbo.Counter) UPDATE target SET Value=Value+1 WHERE Id=1;', read, 'WITH target AS(SELECT Id,Value FROM dbo.Counter) DELETE target WHERE 1=0;'),
        'resolve.no-base': ('WITH target AS(SELECT 1 AS Value) UPDATE target SET Value=2;', read, 'UPDATE target SET Value=2 FROM (SELECT 1 AS Value) target;'),
        'resolve.union': ('WITH target AS(SELECT Id,Value FROM dbo.Counter UNION ALL SELECT Id,Value FROM app.Counter) UPDATE target SET Value=Value+1;', read, 'WITH target AS(SELECT Id FROM dbo.Counter UNION SELECT Id FROM app.Counter) DELETE target;'),
        'scope.blocked': ('CREATE PROCEDURE dbo.Focused AS UPDATE dbo.Counter SET Value=Value+1;', read, 'CREATE PROCEDURE dbo.Focused AS SELECT 1;'),
        'scope.display': ("EXEC(N'UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;');", read, "EXEC(N'SELECT 1;');"),
        'walk.blocked-transaction': ('CREATE PROCEDURE dbo.Focused AS BEGIN TRAN;', read, 'CREATE PROCEDURE dbo.Focused AS BEGIN TRAN; ROLLBACK;'),
        'walk.catch-exit': ("BEGIN TRY THROW 51000,N'focused',1; END TRY BEGIN CATCH RETURN; END CATCH; " + update, read, "BEGIN TRY SELECT 1; END TRY BEGIN CATCH " + update + ' END CATCH;'),
        'walk.composable': ('CREATE TABLE dbo.FocusOutput(Id int); INSERT dbo.FocusOutput SELECT Id FROM(UPDATE dbo.Counter SET Value=Value+1 OUTPUT inserted.Id WHERE Id=1) source;', read, 'UPDATE dbo.Counter SET Value=Value+1 OUTPUT inserted.Id WHERE 1=0;'),
        'walk.drop-index': ('CREATE INDEX FocusIndex ON dbo.Counter(Value); DROP INDEX FocusIndex ON dbo.Counter;', read, 'CREATE INDEX FocusIndex ON dbo.Counter(Value); DROP INDEX dbo.Counter.FocusIndex;'),
        'walk.drop-statistics': ('CREATE STATISTICS FocusStats ON dbo.Counter(Value); DROP STATISTICS dbo.Counter.FocusStats;', read, 'CREATE TABLE #Focused(Id int); CREATE STATISTICS FocusStats ON #Focused(Id); DROP STATISTICS #Focused.FocusStats;'),
        'walk.drop-procedures': ('CREATE PROCEDURE dbo.Focused AS SELECT 1;\nGO\nDROP PROCEDURE dbo.Focused;', read, 'CREATE PROCEDURE #Focused AS SELECT 1;\nGO\nDROP PROCEDURE #Focused;'),
        'walk.insert-exec': ('CREATE TABLE #Focused(Id int); INSERT #Focused EXEC(N\'SELECT 1;\');', read, 'CREATE TABLE dbo.Focused(Id int); INSERT dbo.Focused EXEC(N\'SELECT 1;\');'),
        'walk.module': ('CREATE PROCEDURE dbo.Focused AS UPDATE dbo.Counter SET Value=Value+1;', read, 'CREATE PROCEDURE dbo.Focused AS SELECT 1;'),
        'walk.named-target': ('TRUNCATE TABLE dbo.Counter;', read, 'UPDATE dbo.Counter SET Value=Value+1 WHERE 1=0;'),
        'walk.guess-target': ('CREATE TABLE dbo.Focused(Id int);', read, 'DROP TABLE IF EXISTS dbo.Focused;'),
        'walk.repeat': ('DECLARE @iteration int=0; WHILE @iteration<1 BEGIN UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1; SET @iteration+=1; END;', read, 'WHILE 1=0 BEGIN '+update+' END;'),
        'walk.switch-both': ('IF 1=1 BEGIN '+update+' END ELSE SELECT 1;', read, 'IF 1=0 BEGIN '+update+' END ELSE SELECT 1;'),
        'walk.target-switch': ('UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;', read, 'DELETE dbo.Counter WHERE 1=0;'),
        'walk.temp-procedure': ('CREATE PROCEDURE #Focused AS SELECT 1;\nGO\nEXEC #Focused;', read, 'CREATE PROCEDURE #Focused AS UPDATE dbo.Counter SET Value=Value+1;\nGO\nEXEC #Focused;'),
        'walk.transfer': ('CREATE TABLE dbo.Focused(Id int); ALTER SCHEMA app TRANSFER dbo.Focused;', read, 'CREATE TABLE app.Focused(Id int); ALTER SCHEMA dbo TRANSFER app.Focused;'),
        'walk.try-peak': ('BEGIN TRY BEGIN TRAN; SELECT 1; ROLLBACK; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; END CATCH;', read, 'BEGIN TRY SELECT 1; END TRY BEGIN CATCH SELECT 2; END CATCH;'),
        'temp.tempdb-opt-in': ('SELECT 1 AS Id INTO tempdb.dbo.Wave3Focused;', read, 'CREATE TABLE #Focused(Id int); INSERT #Focused VALUES(1);'),
    }
    for identity, values in additional.items():
        if identity in wanted:
            triple(identity, *values, lane='isolated' if identity == 'temp.tempdb-opt-in' else 'host', options=dict(tempdbIsTemp=True) if identity == 'temp.tempdb-opt-in' else None)
    sqlcmd = {
        'sqlcmd.process': ':setvar target dbo.Counter\nUPDATE $(target) SET Value=Value+1 WHERE Id=1;',
        'sqlcmd.ordered-definitions': ':setvar target dbo.Counter\nSELECT * FROM $(target);\n:setvar target app.Counter\nSELECT * FROM $(target);',
        'sqlcmd.remove-definition': ':setvar target dbo.Counter\nSELECT * FROM $(target);\nGO\n:setvar target\nSELECT * FROM $(target);',
        'sqlcmd.walk': ':on error ignore\nSELECT 1;\nGO\n:exit',
        'sqlcmd-command.on': ':on error ignore\nSELECT 1;',
        'sqlcmd-command.setvar': ':setvar value 42\nSELECT $(value);',
        'sqlcmd-command.variable': ':setvar value 42\nSELECT $(value);',
        'sqlcmd-command.exit': 'SELECT 1;\nGO\n:exit',
        'sqlcmd-command.quit': 'SELECT 1;\nGO\n:quit',
    }
    for identity, sql in sqlcmd.items():
        if identity in wanted:
            triple(identity, sql, read, sql, options=dict(sqlCmdMode=True))
    if 'hidden.bulk-file' in wanted:
        triple('hidden.bulk-file', "SELECT BulkColumn FROM OPENROWSET(BULK '/var/opt/mssql/data/sqlguard-coverage-bulk.csv',SINGLE_CLOB) source;", read, lane='isolated')
    adhoc_connection = 'Server=127.0.0.1,1433;User ID=SgRun_AdhocProbe;Password=Wave3_Disposable_Only_171!;Encrypt=Optional;TrustServerCertificate=yes;'
    adhoc_setup = "EXEC sys.sp_configure N'show advanced options',1; RECONFIGURE; EXEC sys.sp_configure N'Ad Hoc Distributed Queries',1; RECONFIGURE; IF SUSER_ID(N'SgRun_AdhocProbe') IS NULL CREATE LOGIN SgRun_AdhocProbe WITH PASSWORD=N'Wave3_Disposable_Only_171!',CHECK_POLICY=OFF;"
    for database in ['Core', 'Sales', 'Audit', 'Archive']:
        adhoc_setup += ' USE [{{' + database + '}}]; CREATE USER SgRun_AdhocProbe FOR LOGIN SgRun_AdhocProbe; ALTER ROLE db_owner ADD MEMBER SgRun_AdhocProbe;'
    adhoc_setup += ' USE [{{Core}}];'
    for identity in ['hidden.adhoc', 'hidden.openrowset']:
        if identity in wanted:
            sql = "SELECT * FROM OPENROWSET('MSOLEDBSQL','" + adhoc_connection + "','SELECT Id FROM [{{Core}}].dbo.Counter') source;"
            if identity == 'hidden.adhoc':
                sql = "SELECT * FROM OPENDATASOURCE('MSOLEDBSQL','" + adhoc_connection + "').[{{Core}}].dbo.Counter;"
            triple(identity, sql, read, sql, setup=adhoc_setup, lane='isolated')
    if 'dbcc.repair' in wanted:
        triple('dbcc.repair', "DBCC CHECKDB('{{Core}}',REPAIR_REBUILD);", "DBCC CHECKDB('{{Core}}') WITH NO_INFOMSGS;", setup='ALTER DATABASE [{{Core}}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;', lane='isolated')
    if 'unknown-statement.backupstatement' in wanted:
        triple('unknown-statement.backupstatement', "BACKUP DATABASE [{{Core}}] TO DISK=N'/var/opt/mssql/data/wave3-focused.bak' WITH INIT;", read, lane='isolated')
    if 'exec.trusted' in wanted:
        triple('exec.trusted', 'EXEC app.WriteOrder;', read, 'EXEC dbo.NoSuchProcedure;', options=dict(trustedProcedures=['app.WriteOrder']))
    for identity in sorted(wanted):
        if identity.startswith('unknown-dbcc.'):
            token = identity.split('.', 1)[1].upper()
            triple(identity, 'DBCC '+token+';', read, 'DBCC '+token+'(0);', lane='isolated')
    for identity in sorted(wanted):
        if identity.startswith('allow.') and identity not in {'allow.empty-name','allow.remote'}:
            pattern = {'allow.glob':'dbo.Coun*','allow.quoted-parts':'[dbo].[Counter]','allow.invalid':'*','allow.ignore-comments':'-- ignored\ndbo.Counter','allow.cross-database':'{{Core}}.dbo.Counter'}.get(identity, 'dbo.Counter')
            triple(identity, update, 'UPDATE app.Counter SET Value=Value+1 WHERE Id=1;', 'UPDATE [dbo].[Counter] SET Value=Value+1 WHERE 1=0;', options=dict(allowListPatterns=[pattern], defaultSchema='dbo'))
        elif identity.startswith('environment.') and identity not in {'environment.confirm-failure','environment.presentation-failure'}:
            mode = {'environment.off':'Off','environment.red':'Red'}.get(identity, 'YellowRed')
            text = 'wave3|{{Core}}|Owned|' + mode + '|true|dbo.Counter|'
            if identity == 'environment.invalid-line': text = 'not a valid line'
            if identity == 'environment.invalid-list': text = 'wave3|{{Core}}|Owned|YellowRed|true|*|'
            if identity == 'environment.selection': text += '\nwave3|{{Core}}|Second|Off|false||'
            triple(identity, update, read, 'EXEC dbo.Unresolved;', options=dict(environmentRules=text, server='wave3', defaultSchema='dbo'))
    generated = {entry['ruleId'] for entry in dispositions}
    extra = {
        'resolve.cte-cycle': ('WITH target AS(SELECT Id,Value FROM dbo.Counter WHERE Id=1 UNION ALL SELECT source.Id,source.Value FROM dbo.Counter source JOIN target ON source.Id=target.Id WHERE 1=0) UPDATE target SET Value=Value+1 OPTION(MAXRECURSION 1);', None),
        'scope.target-truncate': ('DBCC CHECKDB /*' + 'x'*240 + '*/ WITH NO_INFOMSGS;', None),
        'exec.linked': ("EXEC(N'SELECT 1;') AT SgLoop;", 'isolated'),
        'hidden.openquery': ("SELECT * FROM OPENQUERY(SgLoop,'SELECT 1 AS Id;');", 'isolated'),
        'hidden.deduplicate': ("SELECT * FROM OPENQUERY(SgLoop,'SELECT 1 AS Id;') AS source;", 'isolated'),
    }
    for identity, (sql, lane) in extra.items():
        if identity in wanted:
            triple(identity, sql, read, sql, lane=lane or 'host')
    if 'allow.remote' in wanted:
        triple('allow.remote', 'UPDATE SgLoop.[{{Core}}].dbo.Counter SET Value=Value+1 WHERE Id=1;', read,
               'UPDATE SgLoop.[{{Core}}].dbo.Counter SET Value=Value+1 WHERE 1=0;',
               options=dict(allowListPatterns=['SgLoop.{{Core}}.dbo.Counter']), lane='isolated')
    generated = {entry['ruleId'] for entry in dispositions}
    def exclusion(identity):
        if identity.startswith('recovery-keyword.') or identity in {'recovery.regions', 'parser.lexical-errors'}:
            return 'This branch requires unparsed/lexically invalid text. A successful SQL statement exercises the normal parser, not recovery. Server rejection is not semantic execution coverage; see the linked recovery unit tests.'
        if identity.startswith('sqlcmd-command.'):
            command = identity.split('.', 1)[1]
            return f'SQLCMD client directive {command}, not T-SQL. SqlClient cannot execute it; the restricted campaign preprocessor refuses external/file/UI directives rather than executing shell commands or changing connections. Unit tests cover recognition; no surrogate SELECT is counted.'
        if identity == 'sqlcmd.mode-off':
            return 'Requires SQLCMD syntax with SQLCMD mode disabled, which is a deliberate T-SQL syntax error. Successful substitution would test mode-on instead.'
        if identity.startswith('environment.'):
            return 'Requires a throwing presentation/confirmation callback in GuardExecutionContract, not the Analyze SQL API. Server execution cannot trigger this host callback; targeted callback unit tests are the evidence.'
        if identity == 'allow.empty-name':
            return 'Direct AllowList.Matches(null/empty) API guard; a parsed executable DML target is nonempty. Existing AllowList unit tests exercise the actual guard.'
        if identity == 'limits.cancel':
            return 'Requires an externally cancelled CancellationToken during analysis; not representable in campaign SQL/settings. No server write is needed or appropriate for this API failure path.'
        if identity in {'entry.length','entry.limit','limits.work','limits.action-reset','limits.control-count','limits.nesting-count','limits.syntax','resolve.cte-budget','walk.depth'}:
            return 'Analyzer resource/recursion guard, not a SQL feature. Crossing its bound may be rejected by SQL Server nesting/compile limits or abort analysis before an AST exists. Exact bounded-input unit tests are cited in rules.json; no successful ordinary read is relabeled guard execution.'
        if identity == 'dbcc.dll':
            return 'Requires a genuine loaded extended stored procedure DLL. Linux Developer container has no reviewed xproc DLL/Windows loader. Unknown DBCC spelling only proves the failure branch, not DLL unloading.'
        if identity == 'exec.streaming-wrapper':
            return 'Requires sys.sp_create_streaming_job and a streaming backend; the SQL2025 Linux Developer attempt returns missing/unsupported procedure. Wrapper recognition is unit-tested; no substitute procedure is installed.'
        if identity == 'hidden.internal-openrowset':
            return 'Internal OPENROWSET AST has no supported public standalone provider recipe for this engine. Constructed AST/parser coverage is not real provider execution.'
        if identity in {'hidden.adhoc','hidden.openrowset'}:
            return 'Ad hoc OLE DB provider execution needs enabled ad hoc access and an authenticated provider connection string. Existing SgLoop linked-server execution does not cover this different AST. No validated ad hoc recipe was provisioned; remains a real prerequisite gap, not a platform impossibility.'
        raise ValueError(identity)
    for identity in sorted(wanted - generated):
        dispositions.append(dict(ruleId=identity, status='open-not-claimed', note=exclusion(identity)))
    for lane in ['host','isolated']:
        apply_file(DEST / lane / 'focused.jsonl', ''.join(json.dumps(row) + '\n' for row_lane,row in rows if row_lane == lane))
    apply_file(DEST / 'focused-manifest.json', json.dumps(dispositions, indent=2) + '\n')
    print(f'{len(rows)} focused cases for {len(generated)}/{len(wanted)} requested rules; open {len(wanted-generated)}')


if __name__ == '__main__':
    main()
