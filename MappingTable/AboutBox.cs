using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning ;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MappingTable
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            this.Text = Commons.SystemName + " " + Commons.GetVersion();

            label1.Font = new Font("Arial", 18, FontStyle.Bold | FontStyle.Italic);
            label1.Text = "Mapping Table for Revit";
            label1.Left = this.ClientSize.Width / 2 - label1.Width / 2;
            label2.Font = new Font("Arial", 12, FontStyle.Bold);
            label2.Text = "© 2018 Autodesk, Inc.  All rights reserved.\r\n";
            label2.Text += "\r\nAll use of this Software is subject to the terms and conditions of ";
            label2.Text += "\r\nthe Autodesk license agreement accepted upon installation of ";
            label2.Text += "\r\nthis Software and/or packaged with the Software.\r\n";
            label2.Text += "\r\n\r\nTrademarks";

            label3.Font = new Font("Arial", 11);
            label3.Text = "Autodesk, the Autodesk logo, Revit are registered trademarks or ";
            label3.Text += "\r\ntrademarks of Autodesk, Inc., and/or its subsidiaries and/or affiliates.";
            label3.Text += "\r\n\r\nAll other brand names, product names or trademarks belong to their ";
            label3.Text += "\r\nrespective holders.";

            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void AboutBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void AboutBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            string helppath = Commons.HelpPath();
            if (System.IO.File.Exists(helppath))
            {
                System.Windows.Forms.Help.ShowHelp(this, helppath);
            }
            else
            {
                string mes = "Help file not found.";
                MessageBox.Show(mes + "\r\n\r\n" + helppath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
