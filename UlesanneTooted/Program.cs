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
        }

        Application.Run(new Form2()); // osas 5 asendatakse FormMain-iga
    }
}
