using System.Text.Json;

namespace Tooded_EFCore;

// E-posti saatja ja rakenduse parool hoitakse failis AppData\Local\ToodedApp\email.json.
// See fail asub väljaspool projekti, seega ei jõua see GitHubi.
public class EpostSeaded
{
    public string Saatja { get; set; } = "";
    public string Parool { get; set; } = "";

    public static string Tee => Path.Combine(Kaustad.Juur, "email.json");

    // Tagastab seaded või null, kui fail puudub või on täitmata (siis luuakse tühi mall)
    public static EpostSeaded? LaeVoiLoo()
    {
        try
        {
            if (File.Exists(Tee))
            {
                var s = JsonSerializer.Deserialize<EpostSeaded>(File.ReadAllText(Tee));
                if (s != null && !string.IsNullOrWhiteSpace(s.Saatja)
                    && !string.IsNullOrWhiteSpace(s.Parool)) return s;
            }
            else
            {
                File.WriteAllText(Tee, JsonSerializer.Serialize(new EpostSeaded(),
                    new JsonSerializerOptions { WriteIndented = true }));
            }
        }
        catch { /* vigane fail = käsitleme nagu täitmata faili */ }
        return null;
    }
}
