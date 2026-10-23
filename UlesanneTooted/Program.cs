using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        Kaustad.Loo();

        using (var db = new ToodedContext())
        {
            db.Database.Migrate();
            LooAlgkasutajad(db);
            Algandmed.LooTooted(db);
        }

        Application.Run(new LoginForm());
    }

    private static void LooAlgkasutajad(ToodedContext db)
    {
        // Admin: login "admin", parool "123".
        // Kui andmebaasis on vana admin (admin@admin.ee), uuendame selle.
        var vanaAdmin = db.Kasutajad.FirstOrDefault(k => k.Email == "admin@admin.ee");
        if (vanaAdmin != null)
        {
            vanaAdmin.Email = "admin";
            vanaAdmin.ParoolHash = PasswordHelper.Hash("123");
        }
        else if (!db.Kasutajad.Any(k => k.Email == "admin"))
        {
            db.Kasutajad.Add(new Kasutaja { Nimi = "Admin", Email = "admin",
                                            ParoolHash = PasswordHelper.Hash("123"), Roll = "admin" });
        }

        // Tavakasutaja: login "kirill", parool "456".
        // Kui andmebaasis on vana kasutaja (kasutaja@pood.ee), uuendame selle.
        var vanaKasutaja = db.Kasutajad.FirstOrDefault(k => k.Email == "kasutaja@pood.ee");
        if (vanaKasutaja != null)
        {
            vanaKasutaja.Nimi = "Kirill";
            vanaKasutaja.Email = "kirill";
            vanaKasutaja.ParoolHash = PasswordHelper.Hash("456");
        }
        else if (!db.Kasutajad.Any(k => k.Email == "kirill"))
        {
            db.Kasutajad.Add(new Kasutaja { Nimi = "Kirill", Email = "kirill",
                                            ParoolHash = PasswordHelper.Hash("456"), Roll = "kasutaja" });
        }

        db.SaveChanges();
    }
}
