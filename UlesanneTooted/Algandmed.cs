using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

// Näidistooted poe täitmiseks. Lisatakse ainult need, mida andmebaasis veel pole.
public static class Algandmed
{
    private record Rida(string Nimi, int Kogus, decimal Hind, string Pilt, int KategooriaId);

    private static readonly Rida[] Tooted =
    {
        // 1 = Joogid
        new("Õunamahl", 40, 1.20m, "oun_mahl.png", 1),
        new("Apelsinimahl", 35, 1.50m, "apelsinimahl.png", 1),
        new("Mineraalvesi", 80, 0.80m, "vesi.png", 1),
        new("Kokakoola", 60, 1.30m, "kokakoola.png", 1),
        new("Roheline tee", 25, 2.20m, "roheline_tee.png", 1),
        new("Energiajook", 0, 1.90m, "energiajook.png", 1), // laos pole: nupp hall
        // 2 = Puu- ja köögiviljad
        new("Õun", 120, 0.99m, "oun.png", 2),
        new("Banaan", 90, 1.29m, "banaan.png", 2),
        new("Tomat", 70, 2.49m, "tomat.png", 2),
        new("Kurk", 50, 0.89m, "kurk.png", 2),
        new("Porgand", 100, 0.79m, "porgand.png", 2),
        // 3 = Maiustused
        new("Piimašokolaad", 45, 1.99m, "sokolaad.png", 3),
        new("Kommid", 60, 2.50m, "kommid.png", 3),
        new("Küpsised", 30, 1.75m, "kupsised.png", 3),
        new("Vahukommid", 20, 1.60m, "vahukommid.png", 3),
        new("Martsipan", 15, 3.20m, "martsipan.png", 3),
    };

    public static void LooTooted(ToodedContext db)
    {
        // Kopeerime näidispildid kasutaja pildikausta (kui neid seal veel pole)
        string allikas = Path.Combine(AppContext.BaseDirectory, "SeedPildid");
        if (Directory.Exists(allikas))
            foreach (var fail in Directory.GetFiles(allikas))
            {
                string siht = Path.Combine(Kaustad.Pildid, Path.GetFileName(fail));
                if (!File.Exists(siht)) File.Copy(fail, siht);
            }

        var olemas = db.Toodetabel.Select(t => t.Toodenimetus).ToHashSet();
        foreach (var r in Tooted.Where(r => !olemas.Contains(r.Nimi)))
            db.Toodetabel.Add(new Toode
            {
                Toodenimetus = r.Nimi, Kogus = r.Kogus, Hind = r.Hind,
                Pilt = r.Pilt, KategooriaId = r.KategooriaId
            });
        db.SaveChanges();
    }
}
