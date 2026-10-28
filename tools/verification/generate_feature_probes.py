"""Build feature-specific Docker assertions from a reviewed local CLR assembly."""
import argparse
import hashlib
import json
from pathlib import Path


def generate(assembly):
    binary = Path(assembly).read_bytes()
    blob = "0x" + binary.hex()
    digest = "0x" + hashlib.sha512(binary).hexdigest()
    enable = "EXEC sp_configure 'show advanced options',1; RECONFIGURE; EXEC sp_configure 'clr enabled',1; RECONFIGURE; "
    trust = f"IF NOT EXISTS(SELECT 1 FROM sys.trusted_assemblies WHERE hash={digest}) EXEC sys.sp_add_trusted_assembly @hash={digest},@description=N'Wave3 reviewed trivial types'; "
    create = f"CREATE ASSEMBLY WaveClr FROM {blob} WITH PERMISSION_SET=SAFE;"
    rows = []
    def add(identity, kinds, setup, sql, verify, before, after, **extras):
        rows.append(dict(id=identity, statementTypes=kinds, setupSql=setup, sql=sql, expectedVerdict="RealWrite", verifySql=verify, beforeScalar=before, afterScalar=after, cleanupSql="", **extras))
    assembly_count = "SELECT COUNT(*) FROM sys.assemblies WHERE name=N'WaveClr';"
    add("clr-create", ["CreateAssemblyStatement"], enable+trust, create, assembly_count, 0, 1)
    add("clr-alter", ["AlterAssemblyStatement"], enable+create, "ALTER ASSEMBLY WaveClr WITH VISIBILITY=OFF;", "SELECT CONVERT(int,is_visible) FROM sys.assemblies WHERE name=N'WaveClr';", 1, 0)
    add("clr-drop", ["DropAssemblyStatement"], enable+create, "DROP ASSEMBLY WaveClr;", assembly_count, 1, 0)
    aggregate = "CREATE AGGREGATE dbo.WaveSum(@value int) RETURNS int EXTERNAL NAME WaveClr.WaveSum;"
    aggregate_count = "SELECT COUNT(*) FROM sys.objects WHERE name=N'WaveSum';"
    add("clr-aggregate-create", ["CreateAggregateStatement"], enable+create, aggregate, aggregate_count, 0, 1)
    add("clr-aggregate-execute", ["SelectStatement"], enable+create+aggregate, "SELECT dbo.WaveSum(value) FROM (VALUES(1),(2)) inputs(value);", "SELECT dbo.WaveSum(value) FROM (VALUES(1),(2)) inputs(value);", 3, 3)
    rows[-1]["expectedVerdict"] = "ReadOnly"
    add("clr-aggregate-drop", ["DropAggregateStatement"], enable+create+aggregate, "DROP AGGREGATE dbo.WaveSum;", aggregate_count, 1, 0)
    add("clr-type-create", ["CreateTypeUdtStatement"], enable+create, "CREATE TYPE dbo.WaveNumber EXTERNAL NAME WaveClr.WaveNumber;", "SELECT COUNT(*) FROM sys.assembly_types WHERE name=N'WaveNumber';", 0, 1)
    memory = "ALTER DATABASE SqlGuard_IsolatedCase ADD FILEGROUP WaveMemory CONTAINS MEMORY_OPTIMIZED_DATA; ALTER DATABASE SqlGuard_IsolatedCase ADD FILE(NAME=N'WaveMemory',FILENAME=N'/var/opt/mssql/data/wave-memory') TO FILEGROUP WaveMemory; CREATE TABLE dbo.MemoryTarget(Id int NOT NULL PRIMARY KEY NONCLUSTERED HASH WITH(BUCKET_COUNT=16), Value int NOT NULL) WITH(MEMORY_OPTIMIZED=ON,DURABILITY=SCHEMA_AND_DATA);"
    add("memory-index", ["AlterTableAlterIndexStatement"], memory, "ALTER TABLE dbo.MemoryTarget ALTER INDEX PK_MemoryTarget REBUILD WITH(BUCKET_COUNT=64);", "SELECT CONVERT(int,bucket_count) FROM sys.hash_indexes WHERE object_id=OBJECT_ID(N'dbo.MemoryTarget');", 16, 64)
    rows[-1]["setupSql"] = memory.replace("PRIMARY KEY NONCLUSTERED HASH", "CONSTRAINT PK_MemoryTarget PRIMARY KEY NONCLUSTERED HASH")
    atomic = "CREATE PROCEDURE dbo.MemoryWrite WITH NATIVE_COMPILATION,SCHEMABINDING,EXECUTE AS OWNER AS BEGIN ATOMIC WITH(TRANSACTION ISOLATION LEVEL=SNAPSHOT,LANGUAGE=N'us_english') INSERT dbo.MemoryTarget VALUES(1,7); END;"
    add("memory-atomic-execute", ["BeginEndAtomicBlockStatement"], memory, atomic+"\nGO\nEXEC dbo.MemoryWrite;", "SELECT COUNT(*) FROM dbo.MemoryTarget;", 0, 1)
    external = "CREATE EXTERNAL DATA SOURCE WaveSource WITH(LOCATION='s3://127.0.0.1:9000/coverage'); CREATE EXTERNAL FILE FORMAT WaveFormat WITH(FORMAT_TYPE=DELIMITEDTEXT);"
    external_table = "CREATE EXTERNAL TABLE dbo.WaveExternal(Id int) WITH(LOCATION='/coverage.csv',DATA_SOURCE=WaveSource,FILE_FORMAT=WaveFormat);"
    external_count = "SELECT COUNT(*) FROM sys.external_tables WHERE name=N'WaveExternal';"
    add("external-table-create", ["CreateExternalTableStatement"], external, external_table, external_count, 0, 1)
    add("external-table-drop", ["DropExternalTableStatement"], external+external_table, "DROP EXTERNAL TABLE dbo.WaveExternal;", external_count, 1, 0)
    add("protocol-insert-bulk", ["InsertBulkStatement"], "CREATE TABLE dbo.BulkTarget(Id int);", "INSERT BULK dbo.BulkTarget(Id int);", "SELECT COUNT(*) FROM dbo.BulkTarget;", 0, 1, bulkCopy=True)
    add("service-master-regenerate", ["AlterServiceMasterKeyStatement"], "", "ALTER SERVICE MASTER KEY REGENERATE;", "SELECT CONVERT(varchar(50),modify_date,126) FROM master.sys.symmetric_keys WHERE name=N'##MS_ServiceMasterKey##';", "before-observe", "after-observe")
    rows[-1]["observeChange"] = True
    add("agent-job", ["ExecuteStatement"], "", "EXEC msdb.dbo.sp_add_job @job_name=N'Wave2OwnedJob';", "SELECT COUNT(*) FROM msdb.dbo.sysjobs WHERE name=N'Wave2OwnedJob';", 0, 1)
    rows[-1]["cleanupSql"] = "EXEC msdb.dbo.sp_delete_job @job_name=N'Wave2OwnedJob';"
    add("fulltext-feature", ["CreateFullTextCatalogStatement"], "", "CREATE FULLTEXT CATALOG WaveFulltext;", "SELECT COUNT(*) FROM sys.fulltext_catalogs WHERE name=N'WaveFulltext';", 0, 1)
    for row in rows:
        if create in row['setupSql'] and trust not in row['setupSql']:
            row['setupSql'] = row['setupSql'].replace(create, trust + create)
    return dict(schemaVersion=1, assemblySha256=hashlib.sha256(binary).hexdigest(), fixtures=rows)


def main():
    parser = argparse.ArgumentParser(__doc__)
    parser.add_argument("--assembly", required=True)
    parser.add_argument("--output", required=True)
    args = parser.parse_args()
    Path(args.output).write_text(json.dumps(generate(args.assembly), indent=2)+"\n", encoding="utf-8")


if __name__ == "__main__": main()
