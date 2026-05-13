using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STBLink
{
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {
            // this.Text = RevitLNK.formtitle + " " + Commons.GetVersion();
            this.Text = RevitLNK.formtitle ;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Data.pileSetting = Data.ExportPileSetting.input;
            }
            else if (radioButton3.Checked)
            {
                Data.pileSetting = Data.ExportPileSetting.none;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
