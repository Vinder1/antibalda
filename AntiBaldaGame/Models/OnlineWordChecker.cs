using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntiBaldaGame.Models;

public static class OnlineWordChecker
{
    public static async Task<bool> ExistsInWiktionary(string word)
    {
        using var client = new HttpClient();
        var url = $"https://api.languagetool.org/v2/check?text={Uri.EscapeDataString(word)}&language=ru-RU&disabledRules=UPPERCASE_SENTENCE_START";
        //Console.WriteLine(url);

        try
        {
            var response = await client.GetStringAsync(url);
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("matches", out JsonElement matches) && matches.GetArrayLength() > 0)
            {
                return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
