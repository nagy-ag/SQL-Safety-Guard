using System.Collections;
using System.Reflection;
using System.Text.Json;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlSafetyGuard.ServerHarness;

internal sealed record CoverageEntry(string StatementType, string Status, string[] FixtureIds, string? Reason);
internal sealed record CoverageManifest(int SchemaVersion, string ScriptDomVersion, CoverageEntry[] Entries);
internal sealed record CoverageFixture(string Id, string[] StatementTypes, string SetupSql, string Sql,
    string ExpectedVerdict, string ExpectedPermanentEffect, string VerifySql, JsonElement ExpectedVerifyScalar,
    string[] Prerequisites, bool? ExpectedVisibleChange = null, bool? ExpectedPersistedChange = null,
    int ExpectedTransactionCount = 0);
internal sealed record CoverageFixtures(int SchemaVersion, CoverageFixture[] Fixtures);

internal static class CoverageCorpus
{
    public static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    public static (CoverageManifest Manifest, CoverageFixture[] Fixtures) Load(string directory)
    {
        var manifest = JsonSerializer.Deserialize<CoverageManifest>(File.ReadAllText(Path.Combine(directory, "manifest.json")), Json)!;
        var corpus = JsonSerializer.Deserialize<CoverageFixtures>(File.ReadAllText(Path.Combine(directory, "fixtures.json")), Json)!;
        if (manifest.SchemaVersion != 1 || corpus.SchemaVersion != 1 || manifest.ScriptDomVersion != "180.140.0")
            throw new InvalidDataException("Unsupported coverage schema or ScriptDom package version.");
        Validate(manifest, corpus.Fixtures);
        return (manifest, corpus.Fixtures);
    }

    public static void Validate(CoverageManifest manifest, CoverageFixture[] fixtures)
    {
        var inventory = typeof(TSqlStatement).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(TSqlStatement).IsAssignableFrom(type)).Select(type => type.Name).ToHashSet();
        if (manifest.Entries.Select(entry => entry.StatementType).Distinct().Count() != manifest.Entries.Length ||
            !inventory.SetEquals(manifest.Entries.Select(entry => entry.StatementType)))
            throw new InvalidDataException("Manifest must exactly match the reflected statement inventory without duplicates.");
        if (fixtures.Select(fixture => fixture.Id).Distinct().Count() != fixtures.Length)
            throw new InvalidDataException("Duplicate fixture IDs.");
        foreach (var fixture in fixtures)
        {
            if (string.IsNullOrWhiteSpace(fixture.Id) || fixture.StatementTypes.Length == 0 ||
                fixture.StatementTypes.Distinct().Count() != fixture.StatementTypes.Length ||
                string.IsNullOrWhiteSpace(fixture.VerifySql) || string.IsNullOrWhiteSpace(fixture.Sql) ||
                fixture.ExpectedVerifyScalar.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                !new[] { "ReadOnly", "TempOnly", "RealWrite", "Unknown" }.Contains(fixture.ExpectedVerdict))
                throw new InvalidDataException($"Invalid fixture contract: {fixture.Id}");
            var parsed = StatementTypes(fixture.Sql);
            StatementTypes(fixture.SetupSql);
            StatementTypes(fixture.VerifySql);
            foreach (var kind in fixture.StatementTypes)
                if (!parsed.Contains(kind) || !manifest.Entries.Any(entry => entry.StatementType == kind && entry.FixtureIds.Contains(fixture.Id)))
                    throw new InvalidDataException($"Fixture {fixture.Id}: missing parsed type or reverse link for {kind}.");
        }
        foreach (var entry in manifest.Entries)
        {
            if (!new[] { "fixture", "missing", "safety-excluded", "unsupported" }.Contains(entry.Status) ||
                entry.FixtureIds.Distinct().Count() != entry.FixtureIds.Length ||
                (entry.Status == "fixture") != (entry.FixtureIds.Length > 0) ||
                (entry.Status is "safety-excluded" or "unsupported" && string.IsNullOrWhiteSpace(entry.Reason)))
                throw new InvalidDataException($"Invalid manifest entry: {entry.StatementType}");
            foreach (var id in entry.FixtureIds)
                if (!fixtures.Any(fixture => fixture.Id == id && fixture.StatementTypes.Contains(entry.StatementType)))
                    throw new InvalidDataException($"Unknown or asymmetric fixture link: {entry.StatementType}/{id}");
        }
    }

    public static HashSet<string> StatementTypes(string sql)
        => StatementTypes(sql, new TSql180Parser(true));

    public static HashSet<string> StatementTypes(string sql, TSqlParser parser)
    {
        var fragment = parser.Parse(new StringReader(sql), out var errors);
        if (errors.Count > 0)
            throw new InvalidDataException(string.Join("; ", errors.Select(error => $"L{error.Line}: {error.Message}")));
        var kinds = new HashSet<string>();
        var pending = new Stack<TSqlFragment>();
        var seen = new HashSet<TSqlFragment>(ReferenceEqualityComparer.Instance);
        pending.Push(fragment);
        while (pending.TryPop(out var current))
        {
            if (!seen.Add(current)) continue;
            if (current is TSqlStatement) kinds.Add(current.GetType().Name);
            foreach (var property in current.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetIndexParameters().Length > 0 || property.Name == "ScriptTokenStream") continue;
                var value = property.GetValue(current);
                if (value is TSqlFragment child) pending.Push(child);
                else if (value is IEnumerable children && value is not string)
                    foreach (var item in children)
                        if (item is TSqlFragment nested) pending.Push(nested);
            }
        }
        return kinds;
    }

    public static string Scalar(object? value) => JsonSerializer.Serialize(value is DBNull ? null : value);
}
