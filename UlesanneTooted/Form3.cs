using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public partial class Form3 : Form
{
    private readonly ToodedContext _db = new();
    private readonly TabControl tabPood = new();

    public Form3()
    {
        InitializeComponent();
        Text = "Pood";
        ClientSize = new Size(930, 630);

        tabPood.Dock = DockStyle.Fill;
        Controls.Add(tabPood);

        LaePood();
    }

    private void LaePood()
    {
        tabPood.TabPages.Clear();

        var kategooriad = _db.Kategooriatabel
            .Include(k => k.Tooded) // laeb kohe ka kategooria tooted
            .OrderBy(k => k.Kategooria_nimetus)
            .ToList();

        foreach (var kat in kategooriad)
        {
            var leht = new TabPage(kat.Kategooria_nimetus) { AutoScroll = true };

            int i = 0;
            foreach (var toode in kat.Tooded.OrderBy(t => t.Toodenimetus))
            {
                var kaart = LooTootekaart(toode);
                // 5 kaarti reas: veerg = i % 5, rida = i / 5
                kaart.Location = new Point(10 + (i % 5) * 175, 10 + (i / 5) * 250);
                leht.Controls.Add(kaart);
                i++;
            }
            tabPood.TabPages.Add(leht);
        }
    }

    private Panel LooTootekaart(Toode t)
    {
        var kaart = new Panel { Size = new Size(160, 235), BorderStyle = BorderStyle.FixedSingle };

        var pilt = new PictureBox
        {
            Bounds = new Rectangle(5, 5, 148, 140),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = Kaustad.LaePilt(t.Pilt)
        };

        var silt = new Label
        {
            Bounds = new Rectangle(5, 150, 148, 40),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = $"{t.Toodenimetus}\n{t.Hind:F2} €"
        };

        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(5, 197, 60, 27),
            Minimum = 1,
            Maximum = Math.Max(1, t.Kogus), // rohkem kui laos ei saa valida
            Value = 1
        };

        var nupp = new Button
        {
            Bounds = new Rectangle(70, 195, 83, 30),
            Text = "Lisa korvi",
            Enabled = t.Kogus > 0 // laos pole = nupp hall
        };

        nupp.Click += (s, e) =>
        {
            Ostukorv.Lisa(t, (int)kogus.Value);
            MessageBox.Show($"{t.Toodenimetus} ({kogus.Value} tk) lisati ostukorvi.");
        };

        pilt.Click += (s, e) =>
            MessageBox.Show($"{t.Toodenimetus}\nHind: {t.Hind:F2} €\nLaos: {t.Kogus} tk");

        kaart.Controls.AddRange(new Control[] { pilt, silt, kogus, nupp });
        return kaart;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _db.Dispose();
        base.OnFormClosed(e);
    }
}
