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
    public partial class LevelMapping : Form
    {
        private const string newLevel = "Create New Level";
        private const string notGenerate = "Do not create level";
        internal List<int> LevelMatchingIndex;
        const int len = 10;
        bool DGVflg = false;

        private bool first = true;

        public LevelMapping()
        {
            InitializeComponent();

            first = true;
        }

        private void LevelMapping_Load(object sender, EventArgs e)
        {
            if (!first) return;


            // Form title
            // this.Text = RevitLNK.formtitle + " Level Mapping " + Commons.GetVersion();            
            this.Text = RevitLNK.formtitle + " Level Mapping " ;

            // Initialize
            Init();

            // Story / level / offset header row
            DGV.Rows.Add();
            DGV.Rows[0].ReadOnly = true;

            // Form size
            int x = len + DGV.Width + len;
            int y = len + label1.Height + DGV.Height + len + groupBox1.Height + len + btOK.Height + len;
            this.ClientSize = new Size(x, y);

            // Control positions
            label1.Top = len;
            label1.Left = len;
            DGV.Top = label1.Bottom;
            DGV.Left = len;
            groupBox1.Top = DGV.Bottom + len;
            groupBox1.Left = len;
            btOK.Top = groupBox1.Bottom + len;
            btOK.Left = this.ClientSize.Width - len - Cancel.Width - len / 2 - btOK.Width;
            Cancel.Top = btOK.Top;
            Cancel.Left = btOK.Right + len / 2;
            btBack.Top = btOK.Top;
            btBack.Left = btOK.Left - len / 2 - btBack.Width;
            linkLabel1.Top = this.ClientSize.Height - len - linkLabel1.Height;
            linkLabel1.Left = len;
            linkLabel2.Top = linkLabel1.Top;
            linkLabel2.Left = linkLabel1.Right + len / 2;
            // Positions inside groupBox1
            radb1.Top = len * 2;
            radb1.Left = len * 2;
            radb2.Top = radb1.Bottom;
            radb2.Left = len * 2;
            groupBox2.AutoSize = false;
            groupBox2.Top = radb2.Bottom;
            groupBox2.Left = len;
            groupBox2.Width = groupBox1.ClientSize.Width - len * 2;
            groupBox2.Height = groupBox1.Height - len * 2 - radb1.Height * 2 - len;


            // Combo box for Revit level selection
            DataGridViewComboBoxColumn combo = (DataGridViewComboBoxColumn)DGV.Columns[1];
            combo.Items.AddRange(RevitLNK.LoFa.LevelNameList.Select(a => a.name).ToArray());
            combo.Items.Add(notGenerate);
            combo.Items.Add(newLevel);


            // Rows for STB story data
            List<string> storiesName = new List<string>();
            if (ConvertForm.stb != null)
            {
                storiesName = ConvertForm.stb.StbModel.StbStories.Select(a => a.name).ToList();
            }
            else if (ConvertForm.stb2 != null)
            {
                storiesName = ConvertForm.stb2.StbModel.StbStories.Select(a => a.name).ToList();
            }

            for (int i = storiesName.Count - 1; i >= 0; i--)
            {
                DGV.Rows.Add();

                DGV.Rows[storiesName.Count - i].Cells[0].ReadOnly = true;
                DGV.Rows[storiesName.Count - i].Cells[0].Value = storiesName[i];
                DGV.Rows[storiesName.Count - i].Cells[2].Value = 0;
                if (i + 1 < RevitLNK.LoFa.LevelNameList.Count) // GL omitted; populate combo from Revit levels starting at index i+1
                {
                    DGV.Rows[storiesName.Count - i].Cells[1].Value = RevitLNK.LoFa.LevelNameList[i + 1].name;
                }
                else
                {
                    DGV.Rows[storiesName.Count - i].Cells[1].Value = newLevel;
                }
            }

            if (!ConvertForm.LMD.flg)
            {
                Data.ReadKiten();
            }
            // Initial DataGridView cell selection
            DGV.CurrentCell = DGV[0, 1];
            DGVflg = true;

            ReadProjectParameter();

            // Reference point (datum) position
            if (ConvertForm.stb != null)
            {
                cmbSTB_Xaxis.Items.AddRange(ConvertForm.stb.StbModel.StbAxes.StbX_Axis.Select(a => a.name).ToArray());
                cmbSTB_Yaxis.Items.AddRange(ConvertForm.stb.StbModel.StbAxes.StbY_Axis.Select(a => a.name).ToArray());
            }
            else if (ConvertForm.stb2 != null)
            {
                List<string> x_axis = new List<string>();
                List<string> y_axis = new List<string>();

                // For datum mapping, parallel axes only for now
                if (ConvertForm.stb2.StbModel.StbAxes.StbParallelAxes.Count > 0)
                {
                    x_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("X")).SelectMany(a => a.StbParallelAxis.Select(b => b.name)));
                    y_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("Y")).SelectMany(a => a.StbParallelAxis.Select(b => b.name)));
                }

                //if (ConvertForm.stb2.StbModel.StbAxes.StbArcAxes.Count > 0)
                //{
                //    x_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbArcAxes.Where(a => a.group_name.StartsWith("X")).SelectMany(a => a.StbArcAxis.Select(b => b.name)));
                //    y_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbArcAxes.Where(a => a.group_name.StartsWith("Y")).SelectMany(a => a.StbArcAxis.Select(b => b.name)));
                //}
                //if (ConvertForm.stb2.StbModel.StbAxes.StbRadialAxes.Count > 0)
                //{
                //    x_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbRadialAxes.Where(a => a.group_name.StartsWith("X")).SelectMany(a => a.StbRadialAxis.Select(b => b.name)));
                //    y_axis.AddRange(ConvertForm.stb2.StbModel.StbAxes.StbRadialAxes.Where(a => a.group_name.StartsWith("Y")).SelectMany(a => a.StbRadialAxis.Select(b => b.name)));
                //}

                cmbSTB_Xaxis.Items.AddRange(x_axis.Distinct().ToArray());
                cmbSTB_Yaxis.Items.AddRange(y_axis.Distinct().ToArray());
            }
            cmbRevit_Xaxis.Items.AddRange(RevitLNK.LoFa.GridX.Select(a => a.Name).ToArray());
            cmbRevit_Yaxis.Items.AddRange(RevitLNK.LoFa.GridY.Select(a => a.Name).ToArray());



            Groupbox2_ControlSet();

            if (cmbRevit_Xaxis.Items.Count != 0)
            {
                cmbSTB_Xaxis.SelectedIndex = 0;
                cmbSTB_Yaxis.SelectedIndex = 0;
                cmbRevit_Xaxis.SelectedIndex = 0;
                cmbRevit_Yaxis.SelectedIndex = 0;
            }

            // When form state has already been saved
            if (ConvertForm.LMD.flg)
            {
                if (ConvertForm.LMD.rdb == 1)
                {
                    radb1.Checked = true;

                }
                else
                {
                    radb2.Checked = true;
                }
                cmbSTB_Xaxis.Text = ConvertForm.LMD.STB_X;
                cmbSTB_Yaxis.Text = ConvertForm.LMD.STB_Y;
                cmbRevit_Xaxis.Text = ConvertForm.LMD.RVT_X;
                cmbRevit_Yaxis.Text = ConvertForm.LMD.RVT_Y;
                Numoffset_X1.Value = (decimal)ConvertForm.LMD.Offset_X1;
                Numoffset_Y1.Value = (decimal)ConvertForm.LMD.Offset_Y1;
                Numoffset_X2.Value = (decimal)ConvertForm.LMD.Offset_X2;
                Numoffset_Y2.Value = (decimal)ConvertForm.LMD.Offset_Y2;
                if (ConvertForm.LMD.RevitLevel != null)
                {
                    for (int i = DGV.Rows.Count - 1; i > 0; i--)
                    {
                        DGV.Rows[i].Cells[1].Value = ConvertForm.LMD.RevitLevel[i];
                        DGV.Rows[i].Cells[2].Value = ConvertForm.LMD.RevitOffset[i];
                    }
                }
            }
            

            first = false;
        }
        private void Radb1_CheckedChanged(object sender, EventArgs e)
        {
            // Arrange controls
            Groupbox2_ControlSet();

            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            { cmbSTB_Xaxis.Focus(); }
        }
        private void Radb2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            { Numoffset_X2.Focus(); }
        }
        private void BtOK_Click(object sender, EventArgs e)
        {
            if (!CheckError())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            if (label2.Text == "Cannot specify mapping because grid lines were not entered.")
            {
                MessageBox.Show("Cannot specify mapping because grid lines were not entered.\r\nFor the reference position, choose \"Offset specification\".",
                                this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Persist form state
            ConvertForm.LMD.flg = true;
            Array.Resize(ref ConvertForm.LMD.RevitLevel, DGV.Rows.Count);
            Array.Resize(ref ConvertForm.LMD.RevitOffset, DGV.Rows.Count);
            for (int i = 1; i < DGV.Rows.Count; i++)
            {
                ConvertForm.LMD.RevitLevel[i] = DGV[1, i].Value.ToString();
                ConvertForm.LMD.RevitOffset[i] = DGV[2, i].Value.ToString();
            }
            if (radb1.Checked)
            {
                ConvertForm.LMD.rdb = 1;

            }
            else
            {
                ConvertForm.LMD.rdb = 2;

            }
            ConvertForm.LMD.STB_X = cmbSTB_Xaxis.Text;
            ConvertForm.LMD.STB_Y = cmbSTB_Yaxis.Text;
            ConvertForm.LMD.RVT_X = cmbRevit_Xaxis.Text;
            ConvertForm.LMD.RVT_Y = cmbRevit_Yaxis.Text;
            ConvertForm.LMD.Offset_X1 = (double)Numoffset_X1.Value;
            ConvertForm.LMD.Offset_Y1 = (double)Numoffset_Y1.Value;
            ConvertForm.LMD.Offset_X2 = (double)Numoffset_X2.Value;
            ConvertForm.LMD.Offset_Y2 = (double)Numoffset_Y2.Value;

            RevitLNK.radb1check = radb1.Checked;
            RevitLNK.radb2check = radb2.Checked;
            if (radb1.Checked)
            {
                RevitLNK.XPare.stbAxis = ConvertForm.LMD.STB_X;
                RevitLNK.XPare.RevitGrid = ConvertForm.LMD.RVT_X;
                RevitLNK.YPare.stbAxis = ConvertForm.LMD.STB_Y;
                RevitLNK.YPare.RevitGrid = ConvertForm.LMD.RVT_Y;
            }
            if (radb1.Checked)
            {
                if (ConvertForm.stb2 != null)
                {
                    // Difference conversion needs effective translation; computed here for project persistence.
                    double kitenX = 0, kitenY = 0;
                    double revitX = 0, revitY = 0;
                    for (int i = 0; i < RevitLNK.LoFa.GridX.Count(); i++)
                    {
                        if (RevitLNK.LoFa.GridX[i].Name == RevitLNK.XPare.RevitGrid)
                        {
                            revitX = Commons.ft2mm(RevitLNK.LoFa.GridX[i].Curve.GetEndPoint(0).X);
                            break;
                        }
                    }
                    for (int i = 0; i < RevitLNK.LoFa.GridY.Count(); i++)
                    {
                        if (RevitLNK.LoFa.GridY[i].Name == RevitLNK.YPare.RevitGrid)
                        {
                            revitY = Commons.ft2mm(RevitLNK.LoFa.GridY[i].Curve.GetEndPoint(0).Y);
                            break;
                        }
                    }

                    foreach (var axisGroup in ConvertForm.stb2.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("X")))
                    {
                        var axis = axisGroup.StbParallelAxis.Find(a => a.name == RevitLNK.XPare.stbAxis);
                        if (axis != null)
                        {
                            kitenX = revitX - axis.distance;
                            break;
                        }
                    }
                    foreach (var axisGroup in ConvertForm.stb2.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("Y")))
                    {
                        var axis = axisGroup.StbParallelAxis.Find(a => a.name == RevitLNK.YPare.stbAxis);
                        if (axis != null)
                        {
                            kitenY = revitY - axis.distance;
                            break;
                        }
                    }
                    RevitLNK.XPare.offset = ConvertForm.LMD.Offset_X1 + kitenX;
                    RevitLNK.YPare.offset = ConvertForm.LMD.Offset_Y1 + kitenY;
                    ConvertForm.LMD.Offset_X2 = RevitLNK.XPare.offset;
                    ConvertForm.LMD.Offset_Y2 = RevitLNK.YPare.offset;
                }
                else
                {
                    RevitLNK.XPare.offset = ConvertForm.LMD.Offset_X1;
                    RevitLNK.YPare.offset = ConvertForm.LMD.Offset_Y1;
                }
            }
            else
            {
                RevitLNK.XPare.offset = ConvertForm.LMD.Offset_X2;
                RevitLNK.YPare.offset = ConvertForm.LMD.Offset_Y2;
            }

            RevitLNK.LPare = new List<RevitLNK.LevelPare>();
            for (int i = 1; i < DGV.Rows.Count; i++)
            {
                RevitLNK.LevelPare lp = new RevitLNK.LevelPare
                {
                    stbStrory = DGV.Rows[i].Cells[0].Value.ToString(),
                    RevitLevel = DGV.Rows[i].Cells[1].Value.ToString()
                };
                int.TryParse(DGV.Rows[i].Cells[2].Value.ToString(), out lp.offset);
                RevitLNK.LPare.Add(lp);
            }

            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;            
            this.Close();
        }
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
            f.Dispose();
        }
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var helppath = RevitLNK.HelpPath ;
            if (System.IO.File.Exists(helppath))
            {
                System.Windows.Forms.Help.ShowHelp(this, helppath);
            }
            else
            {
                string mes = "The help file could not be found.";
                MessageBox.Show(mes + "\r\n\r\n" + helppath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void Help_Requested(object sender, HelpEventArgs hlpevent)
        {
            LinkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }

        private bool closeflg = true;

        private void LevelMapping_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ActiveControl is DataGridViewComboBoxEditingControl cb)
            {
                if (cb.DroppedDown == true)
                {
                    if (e.KeyData == Keys.Escape)
                    {
                        // Do not close form while dropdown is open
                        closeflg = false;
                    }
                }
            }

        }
        private void LevelMapping_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.ActiveControl is DataGridViewComboBoxEditingControl cb)
            {
                // For ComboBox column, check suppress-close flag
                if (e.KeyChar == (char)Keys.Escape && closeflg == true)
                {
                    this.Close();
                }
            }
            else
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    this.Close();
                }
            }

            // Reset suppress-close flag each time
            closeflg = true;
        }
        private void Numoffset_X_Enter(object sender, EventArgs e)
        {
            Numoffset_X1.Select(0, Numoffset_X1.Value.ToString().Length);
        }
        private void Numoffset_Y_Enter(object sender, EventArgs e)
        {
            Numoffset_Y1.Select(0, Numoffset_Y1.Value.ToString().Length);
        }
        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGVflg)
            {
                if (DGV.CurrentRow.Index == 0)
                {
                    DGV.CurrentCell = DGV[DGV.CurrentCell.ColumnIndex, 1];
                }
            }
        }
        /// <summary>Open the combo drop-down when the cell receives focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                SendKeys.Send("{F4}");
            }
        }
        /// <summary>Initialization.
        /// </summary>
        private void Init()
        {
            RevitLNK.radb1check = false;
            RevitLNK.radb2check = false;
            RevitLNK.XPare = new RevitLNK.AxisPare();
            RevitLNK.YPare = new RevitLNK.AxisPare();
            RevitLNK.LPare = new List<RevitLNK.LevelPare>();
        }

        /// <summary>Arrange groupBox2 controls.
        /// </summary>
        private void Groupbox2_ControlSet()
        {
            if (radb1.Checked)
            {
                if (RevitLNK.LoFa.GridX.Count != 0 && RevitLNK.LoFa.GridY.Count != 0)
                {
                    // Visibility
                    label3.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;
                    cmbSTB_Xaxis.Visible = true;
                    cmbSTB_Yaxis.Visible = true;
                    cmbRevit_Xaxis.Visible = true;
                    cmbRevit_Yaxis.Visible = true;
                    Numoffset_X1.Visible = true;
                    Numoffset_Y1.Visible = true;
                    Numoffset_X2.Visible = false;
                    Numoffset_Y2.Visible = false;

                    // Text
                    label2.Text = "Enter the reference origin coordinates.";
                    label3.Text = "ST-Bridge";
                    label4.Text = " intersection maps to Revit ";
                    label5.Text = " intersection + ";
                    label6.Text = " (place)";

                    // Combo box items                    
                    //cmbSTB_Xaxis.SelectedIndex = 0;
                    //cmbSTB_Yaxis.SelectedIndex = 0;
                    //cmbRevit_Xaxis.SelectedIndex = 0;
                    //cmbRevit_Yaxis.SelectedIndex = 0;

                    btOK.Focus();

                    // Sizes
                    int cmbsizex = 60, cmbsizey = 20;
                    cmbRevit_Xaxis.Size = new Size(cmbsizex, cmbsizey);
                    cmbRevit_Yaxis.Size = new Size(cmbsizex, cmbsizey);
                    cmbSTB_Xaxis.Size = new Size(cmbsizex, cmbsizey);
                    cmbSTB_Yaxis.Size = new Size(cmbsizex, cmbsizey);
                    Numoffset_X1.Size = new Size(cmbsizex + 10, cmbsizey);
                    Numoffset_Y1.Size = new Size(cmbsizex + 10, cmbsizey);

                    // Layout
                    label2.Top = len * 2;
                    label2.Left = len;

                    int labely = groupBox2.Height / 2;     // Label Y position
                    int boxy = len;       // Vertical spacing for two-row combo layout
                    label3.Top = labely;
                    label3.Left = groupBox2.Width / 2 - (label3.Width + cmbSTB_Xaxis.Width + label4.Width + cmbRevit_Xaxis.Width +
                                  label5.Width + labX.Width + Numoffset_X1.Width + labmm1.Width + label6.Width) / 2;
                    cmbSTB_Xaxis.Top = labely - boxy;
                    cmbSTB_Xaxis.Left = label3.Right;
                    cmbSTB_Yaxis.Top = labely + boxy;
                    cmbSTB_Yaxis.Left = cmbSTB_Xaxis.Left;
                    label4.Top = labely;
                    label4.Left = cmbSTB_Xaxis.Right;
                    cmbRevit_Xaxis.Top = labely - boxy;
                    cmbRevit_Xaxis.Left = label4.Right;
                    cmbRevit_Yaxis.Top = labely + boxy;
                    cmbRevit_Yaxis.Left = cmbRevit_Xaxis.Left;
                    label5.Top = labely;
                    label5.Left = cmbRevit_Xaxis.Right;
                    labX.Top = labely - boxy;
                    labX.Left = label5.Right;
                    labY.Top = labely + boxy;
                    labY.Left = labX.Left;
                    Numoffset_X1.Top = labely - boxy;
                    Numoffset_X1.Left = labX.Right;
                    Numoffset_Y1.Top = labely + boxy;
                    Numoffset_Y1.Left = Numoffset_X1.Left;
                    labmm1.Top = Numoffset_X1.Bottom - labmm1.Height;
                    labmm1.Left = Numoffset_X1.Right;
                    labmm2.Top = Numoffset_Y1.Bottom - labmm2.Height;
                    labmm2.Left = labmm1.Left;
                    label6.Top = labely;
                    label6.Left = labmm2.Right;

                }
                else
                {
                    // Visibility
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;
                    cmbSTB_Xaxis.Visible = false;
                    cmbSTB_Yaxis.Visible = false;
                    cmbRevit_Xaxis.Visible = false;
                    cmbRevit_Yaxis.Visible = false;
                    labX.Visible = false;
                    labY.Visible = false;
                    labmm1.Visible = false;
                    labmm2.Visible = false;
                    Numoffset_X1.Visible = false;
                    Numoffset_Y1.Visible = false;
                    Numoffset_X2.Visible = false;
                    Numoffset_Y2.Visible = false;

                    // Text                    
                    label2.Text = "Cannot specify mapping because grid lines were not entered.";
                    label2.ForeColor = Color.Red;

                    // Layout
                    label2.Top = groupBox2.Height / 2;
                    label2.Left = groupBox2.Width / 2 - label2.Width / 2;
                }
            }
            else
            {
                // Visibility
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                cmbSTB_Xaxis.Visible = false;
                cmbSTB_Yaxis.Visible = false;
                cmbRevit_Xaxis.Visible = false;
                cmbRevit_Yaxis.Visible = false;
                labX.Visible = true;
                labY.Visible = true;
                labmm1.Visible = true;
                labmm2.Visible = true;
                Numoffset_X1.Visible = false;
                Numoffset_Y1.Visible = false;
                Numoffset_X2.Visible = true;
                Numoffset_Y2.Visible = true;

                // Text
                label2.Text = "Enter the plan coordinates for importing the ST-Bridge model.";
                label2.ForeColor = Color.Black;

                // Sizes
                Numoffset_X2.Width = 100;
                Numoffset_Y2.Width = 100;

                // Layout                
                labX.Top = groupBox2.ClientSize.Height / 2;
                labX.Left = label1.Left;
                label2.Top = len * 2;
                label2.Left = len;
                Numoffset_X2.Top = labX.Top;
                Numoffset_X2.Left = labX.Right;
                labmm1.Top = labX.Bottom - labmm1.Height;
                labmm1.Left = Numoffset_X2.Right;
                labY.Top = labX.Top;
                labY.Left = labmm1.Right + len * 2;
                Numoffset_Y2.Top = labX.Top;
                Numoffset_Y2.Left = labY.Right;
                labmm2.Top = labX.Bottom - labmm2.Height;
                labmm2.Left = Numoffset_Y2.Right;
            }
        }

        /// <summary>Check for duplicate Revit level assignments.
        /// </summary>
        /// <returns></returns>
        private bool CheckError()
        {
            LevelMatchingIndex = new List<int>();

            // Run validation

            // Duplicate Revit level mapped to multiple rows
            List<string> names = new List<string>();
            for (int i = DGV.Rows.Count - 1; i >= 1; i--)
            {
                if (DGV.Rows[i].Cells[1].Value.ToString() == newLevel)
                {
                    // Create New
                    LevelMatchingIndex.Add(-1);
                }
                else if (DGV.Rows[i].Cells[1].Value.ToString() == notGenerate)
                {
                    // Do not create level
                    LevelMatchingIndex.Add(-2);
                }
                else
                {
                    if (names.Contains(DGV.Rows[i].Cells[1].Value.ToString()))
                    {
                        MessageBox.Show("Duplicate level assignment.\r\nThe same Revit level is mapped more than once, which is not supported.",
                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        names.Add(DGV.Rows[i].Cells[1].Value.ToString());
                        LevelMatchingIndex.Add(0);
                    }
                }
            }
            if (LevelMatchingIndex.Count() != 0)
            {
                bool flg = true; // True if every row uses "Do not create level"
                for (int i = 0; i < LevelMatchingIndex.Count(); i++)
                {
                    if (LevelMatchingIndex[i] != -2)
                    {
                        flg = false;
                        break;
                    }
                }
                if (flg)
                {
                    string mes = "Every story is set to not create levels.";
                    MessageBox.Show(mes + "\r\nConversion cannot proceed if no levels are created.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }


        /// <summary>Copied from SS3Link…</summary>
        private void ReadProjectParameter()
        {
            string str = "";
            string[] split;
            Autodesk.Revit.DB.ProjectInfo pinfo = Commons.doc.ProjectInformation;
            // Level mapping shared parameter (name must match REXStructuralLink.txt; existing projects use this identifier).
            Autodesk.Revit.DB.Parameter p = pinfo.LookupParameter("レベルマッピング設定");
            if (p != null) str = p.AsString();

            if (str == null) return;
            split = str.Split(',');

            if (split.Count() < 1) return;

            switch (split[0])
            {
                case "1":

                    #region Version 1

                    DataGridViewComboBoxCell combo;

                    for (int i = 1; i < split.Count(); i++)
                    {
                        if (i <= DGV.Rows.Count)
                        {
                            if (split[i] == "[-1]")
                            {
                                DGV.Rows[DGV.Rows.Count - 1 - (i - 1)].Cells[1].Value = newLevel;
                            }
                            else if (split[i] == "[-2]")
                            {
                                DGV.Rows[DGV.Rows.Count - 1 - (i - 1)].Cells[1].Value = notGenerate;
                            }
                            else
                            {
                                combo = DGV.Rows[DGV.Rows.Count - 1 - (i - 1)].Cells[1] as DataGridViewComboBoxCell;
                                if (combo.Items.Contains(split[i]))
                                {
                                    DGV.Rows[DGV.Rows.Count - 1 - (i - 1)].Cells[1].Value = split[i];
                                }
                            }
                        }
                    }

                    #endregion

                    break;
            }
        }

        /// <summary>Custom paint for DataGridView cells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                if (e.ColumnIndex == 0)
                {
                    // Get cell bounds
                    Rectangle rect = e.CellBounds;
                    DataGridView dgv = (DataGridView)sender;

                    Paint1(e, rect, dgv, "Story");
                }
                else if (e.ColumnIndex == 1)
                {
                    // Get cell bounds
                    Rectangle rect = e.CellBounds;
                    DataGridView dgv = (DataGridView)sender;

                    Paint1(e, rect, dgv, "Level");
                }
                else if (e.ColumnIndex == 2)
                {
                    // Get cell bounds
                    Rectangle rect = e.CellBounds;
                    DataGridView dgv = (DataGridView)sender;

                    Paint1(e, rect, dgv, "Offset");
                }
            }
            else if(e.RowIndex == -1)
            {
                // Merge columns 2 and 3
                if (e.ColumnIndex == 0)
                {
                    // Get cell bounds
                    Rectangle rect = e.CellBounds;
                    DataGridView dgv = (DataGridView)sender;

                    Paint1(e, rect, dgv, "ST-Bridge");
                }
                if (e.ColumnIndex == 1)
                {
                    // Only handle column 2; extend width to include column 3
                    // Get cell bounds
                    Rectangle rect = e.CellBounds;

                    DataGridView dgv = (DataGridView)sender;

                    // Add column 3 width to column 2
                    rect.Width += dgv.Columns[2].Width;
                    
                    Paint1(e, rect, dgv, "Revit");
                }
            }
            else
            {
                return;
            }

            // Mark as handled
            e.Handled = true;
        }

        /// <summary>Shared paint helper for DataGridView cells.</summary>
        /// <param name="e"></param>
        /// <param name="rect"></param>
        /// <param name="dgv"></param>
        /// <param name="str"></param>
        private void Paint1(DataGridViewCellPaintingEventArgs e, Rectangle rect, DataGridView dgv, string str)
        {
            // Background, grid line, cell text
            using (SolidBrush brush = new SolidBrush(DGV.ColumnHeadersDefaultCellStyle.BackColor))
            {
                // Background fill
                e.Graphics.FillRectangle(brush, rect);

                using (Pen pen = new Pen(dgv.GridColor))
                {
                    // Border
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
            // Vertical text offset
            rect.Y += 2;
            // Draw cell text
            TextRenderer.DrawText(e.Graphics,
                                  str,
                                  e.CellStyle.Font,
                                  rect,
                                  e.CellStyle.ForeColor,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        #region DGV events

        private void DGV_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            // Validate only when not a new row and the cell is dirty
            if (e.RowIndex == dgv.NewRowIndex || !dgv.IsCurrentCellDirty)
            {
                return;
            }
            double re = 0;
            if (dgv.Columns[e.ColumnIndex].Name == "Revit2")
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out re))
                {
                    dgv.CancelEdit();
                    e.Cancel = true;
                }
                else
                {
                    if (re > 1000000 || re < -1000000)
                    {
                        dgv.CancelEdit();
                        e.Cancel = true;
                    }
                }
            }
        }

        private void DGV_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            // Clear error text
            dgv.Rows[e.RowIndex].ErrorText = null;
            if (dgv.Columns[e.ColumnIndex].Name == "Revit2")
            {
                if (double.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out double re))
                {
                    double d = Math.Pow(10, 1);
                    if (re > 0)
                    { re = Math.Floor((re * d) + 0.5) / d; }
                    else
                    { re = Math.Ceiling((re * d) - 0.5) / d; }
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = re;
                }
            }

        }
        #endregion


    }
}
