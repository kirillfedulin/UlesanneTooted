using System.Net;
using System.Net.Mail;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Tooded_EFCore;

public partial class ArveForm : Form
{
    // ===== SIIA KIRJUTA OMA ANDMED =====
    private const string Saatja = "sinu.konto@gmail.com";          // Gmail, millelt kiri saadetakse
    private const string RakenduseParool = "xxxx xxxx xxxx xxxx";  // Google'i rakenduse parool (16 märki)
    private const string VaikeSaaja = "";                          // e-post, kuhu arve saadetakse (võib jätta tühjaks)
    // ===================================

    private readonly ListBox listOstukorv = new();
    private readonly Label lblKokku = new();
    private readonly TextBox txtEmail = new();
    private readonly Button btnEemalda = new(), btnGenPDF = new(), btnSaadaEmail = new();

    private string? viimanePdf; // viimati loodud arve täistee

    public ArveForm()
    {
        InitializeComponent();
        Text = "Ostukorv ja arve";
        ClientSize = new Size(600, 380);

        listOstukorv.SetBounds(20, 20, 330, 300);

        lblKokku.SetBounds(20, 330, 330, 30);
        lblKokku.Font = new Font(Font, FontStyle.Bold);

        txtEmail.SetBounds(370, 20, 200, 27);
        txtEmail.PlaceholderText = "Saaja e-post";
        txtEmail.Text = VaikeSaaja;

        SeaNupp(btnEemalda, "Eemalda valitud rida", 70, btnEemalda_Click);
        SeaNupp(btnGenPDF, "Genereeri PDF", 115, btnGenPDF_Click);
        SeaNupp(btnSaadaEmail, "Saada e-postiga", 160, btnSaadaEmail_Click);

        Controls.AddRange(new Control[] { listOstukorv, lblKokku, txtEmail,
            btnEemalda, btnGenPDF, btnSaadaEmail });

        NaitaKorvi();
    }

    private void SeaNupp(Button nupp, string tekst, int y, EventHandler kasitleja)
    {
        nupp.Text = tekst;
        nupp.SetBounds(370, y, 200, 35);
        nupp.Click += kasitleja;
    }

    private void NaitaKorvi()
    {
        listOstukorv.Items.Clear();
        foreach (var rida in Ostukorv.Ridad)
            listOstukorv.Items.Add(rida); // ListBox kuvab OstukorviRida.ToString()
        lblKokku.Text = $"Kokku: {Ostukorv.Kokku:F2} €";
    }

    private void btnEemalda_Click(object? sender, EventArgs e)
    {
        if (listOstukorv.SelectedItem is OstukorviRida rida)
        {
            Ostukorv.Ridad.Remove(rida);
            NaitaKorvi();
        }
    }

    // ---------- PDF ----------
    private void btnGenPDF_Click(object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show("Ostukorv on tühi.");
            return;
        }

        viimanePdf = Path.Combine(Kaustad.Arved, $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        using var dokument = new PdfDocument();
        PdfPage leht = dokument.AddPage();

        using (XGraphics gfx = XGraphics.FromPdfPage(leht))
        {
            var unicode = new XPdfFontOptions(PdfFontEncoding.Unicode); // täpitähed ja €
            var pealkiri = new XFont("Arial", 20, XFontStyle.Bold, unicode);
            var tavaline = new XFont("Arial", 12, XFontStyle.Regular, unicode);
            var paks = new XFont("Arial", 14, XFontStyle.Bold, unicode);

            gfx.DrawString("ARVE", pealkiri, XBrushes.Black,
                new XRect(0, 30, leht.Width, 30), XStringFormats.TopCenter);
            gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy}", tavaline, XBrushes.Black, 40, 90);

            double y = 130;
            foreach (var rida in Ostukorv.Ridad)
            {
                gfx.DrawString(rida.ToString(), tavaline, XBrushes.Black, 40, y);
                y += 22;
            }

            gfx.DrawString($"Kokku: {Ostukorv.Kokku:F2} €", paks, XBrushes.Black, 40, y + 20);
        }

        dokument.Save(viimanePdf);
        MessageBox.Show($"PDF on loodud:\n{viimanePdf}");
    }

    // ---------- E-post ----------
    private void btnSaadaEmail_Click(object? sender, EventArgs e)
    {
        if (viimanePdf == null || !File.Exists(viimanePdf))
        {
            MessageBox.Show("Genereeri enne PDF.");
            return;
        }

        string saaja = txtEmail.Text.Trim();
        if (!MailAddress.TryCreate(saaja, out _))
        {
            MessageBox.Show("Sisesta korrektne e-posti aadress.");
            return;
        }

        try
        {
            // using vabastab manuse, muidu jääb PDF-fail lukku
            using var kiri = new MailMessage(Saatja, saaja, "Arve", "Arve on manuses.");
            kiri.Attachments.Add(new Attachment(viimanePdf));

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(Saatja, RakenduseParool),
                EnableSsl = true
            };
            smtp.Send(kiri);

            MessageBox.Show("Arve saadetud!");
            Ostukorv.Tyhjenda();
            NaitaKorvi();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Viga: " + ex.Message);
        }
    }
}
