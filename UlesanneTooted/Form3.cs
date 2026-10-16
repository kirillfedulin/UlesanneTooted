using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UlesanneTooted;

public partial class Form3 : Form
{
    private readonly ToodedContext _db;

    public Form3()
    {
        InitializeComponent();
        _db = new ToodedContext();
        LaeKategooriadJaPildid();
    }
}