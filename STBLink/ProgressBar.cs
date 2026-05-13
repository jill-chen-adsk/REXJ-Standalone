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
    public partial class ProgressBarForm : Form
    {
        public ProgressBarForm()
        {
            InitializeComponent();
        }

        internal Panel panelFooter;
        internal Label lab;
        private void ProgressBar_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            panelFooter = new Panel();
            panelFooter = panel1;
            lab = new Label();
            lab = label1;
            lab.AutoSize = true;
            int x = panel1.Width;
            int y = panel1.Height;
            this.ClientSize = new Size(x, y);
            panelFooter.Width = this.ClientSize.Width;
            panel1.Top = 0;
            panel1.Left = 0;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
        }
        // Override CreateParams property
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;

                return createParams;
            }
        }
      
    }
}
