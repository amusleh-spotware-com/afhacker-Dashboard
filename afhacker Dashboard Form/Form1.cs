using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace afhacker_Dashboard_Form
{
    public partial class Form1 : MetroForm
    {
        private MetroForm SymbolsForm;

        public Form1()
        {
            InitializeComponent();
            SymbolsForm = new MetroForm();
            SymbolsForm.Name = "SymbolsForm";
            SymbolsForm.Text = "Symbols";

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            SymbolsForm.ShowDialog();
        }
    }
}
