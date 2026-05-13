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
    public partial class ProgressBar2 : Form
    {
        private bool Closeflag { get; set; } = false;

        private string Msg { get; set; } = "";


        public ProgressBar2(string msg, int maximum)
        {
            InitializeComponent();

            this.Text = Commons.SystemName;
            Msg = msg;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = maximum;
            progressBar1.Step = 1;
            SetLabel();

        }

        public ProgressBar2(string msg, int maximum, Point location) : this(msg, maximum)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (Closeflag)
            {
                timer1.Stop();
                this.Close();
            }
        }

        private void ProgressBar2_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }



        /// <summary>
        /// Advances the progress bar one step.
        /// </summary>
        internal void PerformStep()
        {
            if (this.InvokeRequired)
            {
                // Invoke required (update from another thread)
                this.Invoke((MethodInvoker)(() =>
                {
                    progressBar1.PerformStep();
                    SetLabel();
                }));
            }
            else
            {
                progressBar1.PerformStep();
                SetLabel();
            }
        }

        /// <summary>
        /// Closes the form (scheduled on timer tick).
        /// </summary>
        internal void ProgressClose()
        {
            Closeflag = true;
        }

        /// <summary>
        /// Refreshes the label text from current progress values.
        /// </summary>
        private void SetLabel()
        {
            label1.Text = $"{Msg}  {progressBar1.Value} / {progressBar1.Maximum}";
        }


        /// <summary>
        /// Keeps the form visible, updates the message, and resets progress from the start.
        /// </summary>
        internal void Restart(string msg, int maximum)
        {
            Msg = msg;

            if (this.InvokeRequired)
            {
                // Invoke required (update from another thread)
                this.Invoke((MethodInvoker)(() =>
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = maximum;
                    SetLabel();
                }));
            }
            else
            {
                progressBar1.Value = 0;
                progressBar1.Maximum = maximum;
                SetLabel();
            }
        }

    }
}
