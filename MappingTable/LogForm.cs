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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        int len = 10;
        private void LogForm_Load(object sender, EventArgs e)
        {
            //呼び出し側で設定
            //this.Text = Commons.SystemName + " ログ [Ver." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "]";
            const int sizeW = 20;
            const int sizeH = 20;
            List<LogData.Log> newlog = LogData.Data;
            dgvLog.Rows.Add(newlog.Count());


            int count_inf = 0;
            int count_war = 0;
            int count_err = 0;

            chkInfo.Top = len;
            chkInfo.Left = len;
            chkWarning.Top = len;
            chkWarning.Left = chkInfo.Right;
            chkError.Top = len;
            chkError.Left = chkWarning.Right;

            //インフォメーションマーク
            Bitmap canvasInfo = new Bitmap(20, 20);
            Graphics g = Graphics.FromImage(canvasInfo);
            //補間方法として高品質双三次補間を指定する
            g.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image img = SystemIcons.Information.ToBitmap();
            g.DrawImage(img, 0, 0, img.Width * 20 / 32, img.Height * 20 / 32);

            //警告マーク
            Bitmap canvasWar = new Bitmap(20, 20);
            Graphics g2 = Graphics.FromImage(canvasWar);
            //補間方法として高品質双三次補間を指定する
            g2.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image img2 = SystemIcons.Warning.ToBitmap();
            g2.DrawImage(img2, 0, 0, img2.Width * 20 / 32, img2.Height * 20 / 32);

            //エラーマーク
            Bitmap canvasError = new Bitmap(20, 20);
            Graphics g3 = Graphics.FromImage(canvasError);
            //補間方法として高品質双三次補間を指定する
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
            }
            

            //コントロール表示位置の調整
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

            //列幅調整
            dgvLog.AutoResizeColumn(2);
            dgvLog.AutoSize = false;

            //Formサイズ
            this.ClientSize = new Size(dgvLog.Width + len * 2, len + chkInfo.Height +  dgvLog.ClientSize.Height + len + btnClose.Height + len);
            bool regflg = true;
            do
            {
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Autodesk\Revit\ST-Bridge Link\" + Commons.RevitVersion, false);
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

            //表示切替時にスクロールバーが伸び縮みするのが気になるので、一旦バーを非表示に
            dgvLog.ScrollBars = ScrollBars.None;

            //表示非表示
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
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = Commons.SystemName + " Save Batch Parameter Add Log",
                Filter = "Batch Parameter Add Log (*.txt)|*.txt"
            };
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + Commons.RevitVersion;
            sfd.FileName = "Batch Parameter Add Log";
            sfd.InitialDirectory = filename;

            if (sfd.ShowDialog() == DialogResult.OK)
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

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Formサイズ記憶
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Autodesk\Revit\ST-Bridge Link\" + Commons.RevitVersion);
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
