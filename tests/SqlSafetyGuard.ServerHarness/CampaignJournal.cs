using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SqlSafetyGuard.ServerHarness;

internal static class CampaignJournal
{
    private sealed record Receipt(string CampaignKey, CampaignResult Result);

    private static string Name(string identifier) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identifier))) + ".json";

    internal static List<CampaignResult> Merge(IEnumerable<CampaignResult> persisted, IEnumerable<CampaignResult> journal) =>
        persisted.Concat(journal).GroupBy(result => result.Case.Id, StringComparer.Ordinal).Select(group => group.Last()).ToList();

    internal static void Save(string directory, string campaignKey, CampaignResult result)
    {
        var journal = Directory.CreateDirectory(Path.Combine(directory, "receipts")).FullName;
        var destination = Path.Combine(journal, Name(result.Case.Id));
        var temporary = destination + "." + Guid.NewGuid().ToString("N") + ".tmp";
        var bytes = JsonSerializer.SerializeToUtf8Bytes(new Receipt(campaignKey, result), CampaignRunner.Json);
        using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            stream.Write(bytes);
            stream.Flush(true);
        }
        File.Move(temporary, destination, true);
    }

    internal static IEnumerable<CampaignResult> Load(string directory, string campaignKey)
    {
        var journal = Path.Combine(directory, "receipts");
        if (!Directory.Exists(journal)) yield break;
        foreach (var file in Directory.EnumerateFiles(journal, "*.json"))
        {
            var receipt = JsonSerializer.Deserialize<Receipt>(File.ReadAllText(file), CampaignRunner.Json)
                ?? throw new InvalidDataException("Empty campaign receipt: " + file);
            if (receipt.CampaignKey != campaignKey || Path.GetFileName(file) != Name(receipt.Result.Case.Id))
                throw new InvalidDataException("Campaign receipt identity mismatch: " + file);
            yield return receipt.Result;
        }
    }
}
