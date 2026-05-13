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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        int len = 10;
        private void LogForm_Load(object sender, EventArgs e)
        {
            //this.Text = RevitLNK.formtitle + " Conversion log [Ver." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "]";
            const int sizeW = 20;
            const int sizeH = 20;
            List<LogData.Log> newlog = new List<LogData.Log>();
            bool flg = false;
            if (LogData.STBLog.Count() > 0)
            {
                newlog = LogData.STBLog;
                dgvLog.Rows.Add(newlog.Count());
            }
            else
            {
                newlog = LogData.Data;
                flg = true;
                dgvLog.Rows.Add(newlog.Count() + (ConvertForm.stb?.unknownList?.Count() ?? 0) );
            }
            

            int count_inf = 0;
            int count_war = 0;
            int count_err = 0;

            chkInfo.Top = len;
            chkInfo.Left = len;
            chkWarning.Top = len;
            chkWarning.Left = chkInfo.Right;
            chkError.Top = len;
            chkError.Left = chkWarning.Right;

            // Information icon
            Bitmap canvasInfo = new Bitmap(20, 20);
            Graphics g = Graphics.FromImage(canvasInfo);
            // Use high-quality bicubic interpolation when scaling icons
            g.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image img = SystemIcons.Information.ToBitmap();
            g.DrawImage(img, 0, 0, img.Width * 20 / 32, img.Height * 20 / 32);

            // Warning icon
            Bitmap canvasWar = new Bitmap(20, 20);
            Graphics g2 = Graphics.FromImage(canvasWar);
            g2.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image img2 = SystemIcons.Warning.ToBitmap();
            g2.DrawImage(img2, 0, 0, img2.Width * 20 / 32, img2.Height * 20 / 32);

            // Error icon
            Bitmap canvasError = new Bitmap(20, 20);
            Graphics g3 = Graphics.FromImage(canvasError);
            g3.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image img3 = SystemIcons.Error.ToBitmap();
            g3.DrawImage(img3, 0, 0, img3.Width * 20 / 32, img3.Height * 20 / 32);

            for (int r  =0; r < dgvLog.Rows.Count; r++)
            {
                DataGridViewImageCell cell = new DataGridViewImageCell
                {
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                if (r < newlog.Count())
                {
                    switch (newlog[r].Kind)
                    {
                        case LogData.LogKind.Infmoation:
                            dgvLog.Rows[r].Cells[0].Value = chkInfo.Text;
                            cell.Value = SystemIcons.Information;
                            count_inf += 1;
                            break;
                        case LogData.LogKind.Warning:
                            dgvLog.Rows[r].Cells[0].Value = chkWarning.Text;
                            cell.Value = canvasWar;
                            count_war += 1;
                            break;
                        case LogData.LogKind.Error:
                            dgvLog.Rows[r].Cells[0].Value = chkError.Text;
                            cell.Value = canvasError;
                            cell.Value = new Bitmap(SystemIcons.Error.ToBitmap(), sizeW, sizeH);
                            count_err += 1;
                            break;
                    }
                    dgvLog.Rows[r].Cells[2].Value = newlog[r].Message;
                    dgvLog.Rows[r].Cells[1] = cell;
                }
                else
                {
                    if (flg)
                    {
                        int n = r - newlog.Count();
                        if (ConvertForm.stb.unknownList[n] == "")
                        {
                            dgvLog.Rows.RemoveAt(r);
                            r--;
                            continue;
                        }
                        dgvLog.Rows[r].Cells[0].Value = chkWarning.Text;
                        cell.Value = canvasWar;
                        count_war += 1;

                        dgvLog.Rows[r].Cells[2].Value = ConvertForm.stb.unknownList[n];
                        dgvLog.Rows[r].Cells[1] = cell;
                    }
                }

                
            }
            

            // Adjust filter control layout
            if(count_inf == 0)
            {
                chkInfo.Visible = false;
                chkError.Location = chkWarning.Location;
                chkWarning.Location = chkInfo.Location;
            }
            if(count_war == 0)
            {
                chkWarning.Visible = false;
                chkError.Location = chkWarning.Location;
            }
            if(count_err == 0)
            {
                chkError.Visible = false;
            }

            chkInfo.ImageAlign = ContentAlignment.MiddleLeft;
            chkInfo.Image = canvasInfo;
            chkWarning.ImageAlign = ContentAlignment.MiddleLeft;
            chkWarning.Image = canvasWar;
            chkError.ImageAlign = ContentAlignment.MiddleLeft;
            chkError.Image = canvasError;

            // Column width
            dgvLog.AutoResizeColumn(2);
            dgvLog.AutoSize = false;

            // Form client size
            this.ClientSize = new Size(dgvLog.Width + len * 2, len + chkInfo.Height +  dgvLog.ClientSize.Height + len + btnClose.Height + len);
            bool regflg = true;
            do
            {
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Autodesk\Revit\ST-Bridge Link\" + RevitLNK.RevitVersion, false);
                if (regkey == null) break;
                int fWidth = (int)regkey.GetValue("LogFormWidth", this.Width);
                int fHeight = (int)regkey.GetValue("LogFormHeight", this.Height);
                this.Width = fWidth;
                this.Height = fHeight;
                regflg = false;
            } while (regflg);
            dgvLog.Width = this.ClientSize.Width - 2 * len;
            dgvLog.Height = this.ClientSize.Height - len - chkError.Height - len - btnClose.Height - len;
            dgvLog.Top = chkInfo.Bottom;
            dgvLog.Left = len;
            btnSave.Top = dgvLog.Bottom + len;
            btnSave.Left = len;
            btnClose.Top = dgvLog.Bottom + len;
            btnClose.Left = this.ClientSize.Width - len - btnClose.Width;

            dgvLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.TopMost = true;
            this.Tag = "Init";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void ChkInfo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if(cb == null) { return; }

            // Hide scroll bars briefly to avoid scrollbar flicker when toggling visibility
            dgvLog.ScrollBars = ScrollBars.None;

            // Show or hide rows for this severity
            for(int r = 0; r< dgvLog.Rows.Count; r++)
            {
                if(dgvLog.Rows[r].Cells[0].Value == null) { continue; }
                if(dgvLog.Rows[r].Cells[0].Value.ToString() == cb.Text)
                { dgvLog.Rows[r].Visible = cb.Checked; }
            }

            dgvLog.AutoResizeColumn(2);
            dgvLog.ScrollBars = ScrollBars.Both;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (this.Text.Contains("Import log", StringComparison.OrdinalIgnoreCase)
                || this.Text.Contains("Export log", StringComparison.OrdinalIgnoreCase)
                || this.Text.Contains("Conversion log", StringComparison.OrdinalIgnoreCase))
            {
                sfd.Title = RevitLNK.formtitle + " Save import log";
                sfd.Filter = "Import log (*.txt)|*.txt";
                string filename = System.IO.Path.GetFileNameWithoutExtension(RevitLNK.openfilename);
                sfd.FileName = filename + "-import-log";
                if (RevitLNK.openfilename != "")
                {
                    sfd.InitialDirectory = System.IO.Path.GetDirectoryName(RevitLNK.openfilename);
                }
            }
            else if (this.Text.Contains("read error", StringComparison.OrdinalIgnoreCase))
            {
                sfd.Title = RevitLNK.formtitle + " Save STB file read errors";
                sfd.Filter = "STB read errors (*.txt)|*.txt";
                string filename = System.IO.Path.GetFileNameWithoutExtension(RevitLNK.openfilename);
                sfd.FileName = filename + "-stb-read-errors";
                if (RevitLNK.openfilename != "")
                {
                    sfd.InitialDirectory = System.IO.Path.GetDirectoryName(RevitLNK.openfilename);
                }
            }
            else
            {
                sfd.Title = RevitLNK.formtitle + " Save bulk parameter log";
                sfd.Filter = "Bulk parameter log (*.txt)|*.txt";
                string filename = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                sfd.FileName = "bulk-parameter-log";
                sfd.InitialDirectory = filename;
            }
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                List<string> writedata = new List<string>();

                for(int r = 0; r< dgvLog.Rows.Count; r++)
                {
                    if(dgvLog.Rows[r].Visible == true || chkSaveOption.Checked == true)
                    {
                        string msg = "";
                        msg += dgvLog.Rows[r].Cells[0].Value.ToString() + "\t";
                        msg += dgvLog.Rows[r].Cells[2].Value.ToString();
                        writedata.Add(msg);
                    }
                }
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                System.IO.File.WriteAllLines(sfd.FileName, writedata.ToArray(), Encoding.GetEncoding("Shift_JIS"));
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LogForm_ResizeEnd(object sender, EventArgs e)
        {
            dgvLog.AutoResizeColumn(2);
        }

        private void LogForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helppath = RevitLNK.HelpPath ;
            if (System.IO.File.Exists(helppath))
            {
                System.Windows.Forms.Help.ShowHelp(this, helppath);
            }
            else
            {
                MessageBox.Show("The help file could not be found.\r\n\r\n" + helppath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Remember window size
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Autodesk\Revit\ST-Bridge Link\" + RevitLNK.RevitVersion);
            regkey.SetValue("LogFormWidth", this.Width);
            regkey.SetValue("LogFormHeight", this.Height);
        }

        private void LogForm_Activated(object sender, EventArgs e)
        {
            if(this.Tag == null) { return; }
            if (this.Tag.ToString() == "Init")
            {
                this.Tag = "";
                this.TopMost = false;
            }
        }
    }
}
