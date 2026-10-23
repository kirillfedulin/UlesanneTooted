namespace Tooded_EFCore;

public partial class FormMain : Form
{
    private readonly Button btnTooted = new(), btnPood = new(), btnArve = new();

    public FormMain()
    {
        InitializeComponent();
        Text = "Peamenüü";
        ClientSize = new Size(260, 200);

        SeaNupp(btnTooted, "Tootehaldus", 30, (s, e) => Ava(new Form2()));
        SeaNupp(btnPood, "Pood", 80, (s, e) => Ava(new Form3()));
        SeaNupp(btnArve, "Ostukorv ja arve", 130, (s, e) => Ava(new ArveForm()));

        Controls.AddRange(new Control[] { btnTooted, btnPood, btnArve });

        btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
        Text = $"Peamenüü – {AktiivneKasutaja.Nimi}";
    }

    private void SeaNupp(Button nupp, string tekst, int y, EventHandler kasitleja)
    {
        nupp.Text = tekst;
        nupp.SetBounds(30, y, 200, 40);
        nupp.Click += kasitleja;
    }

    private static void Ava(Form vorm)
    {
        using (vorm) // vorm vabastatakse pärast sulgemist
            vorm.ShowDialog();
    }
}
