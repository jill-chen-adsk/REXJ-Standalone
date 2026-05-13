using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STBLink
{
    public partial class MaterialMapping : Form
    {
        private bool first = true;

        public MaterialMapping()
        {
            InitializeComponent();
            
            first = true;
        }

        const int len = 10;

        private Dictionary<string, string> SteelMaterial = new Dictionary<string, string>();

        private void MaterialMapping_Load(object sender, EventArgs e)
        {
            ReadMaterialMapping();

            if (!first) return;


            // Form title
            // this.Text = RevitLNK.formtitle + " Material Mapping " + Commons.GetVersion();
            this.Text = RevitLNK.formtitle + " Material Mapping " ;

            // Steel material definitions
            if (RevitLNK.MateData.Count() != 0)
            {
                //// Load project parameters
                //Autodesk.Revit.DB.ProjectInfo pinfo = Commons.doc.ProjectInformation;
                //Autodesk.Revit.DB.Parameter p = pinfo.LookupParameter(Data.projectParams[5]);
                //string[] split = new string[0];
                //if (p != null)
                //{
                //    string str = p.AsString();                    
                //    string[] jouken = { "," };
                //    split = str.Split(jouken, StringSplitOptions.None);                    
                //}

                DataGridViewComboBoxColumn comb = DGV.Columns[1] as DataGridViewComboBoxColumn;
                // Add Revit materials in category Metal to the combo box
                for (int j = 0; j < RevitLNK.LoFa.RevitMatName.Count(); j++)
                {
                    CombBox_Add(RevitLNK.LoFa.RevitMatName[j].Name, ref comb);
                }
                //// Add STB steel material names (optional)
                //for (int j = 0; j < RevitLNK.MateData.Count(); j++)
                //{
                //    CombBox_Add(RevitLNK.MateData[j].stbmatName, ref comb);
                //}
                //// Add STB steel settings from project (optional)
                //for (int j = 0; j < split.Count(); j++)
                //{
                //    CombBox_Add(split[j], ref comb);
                //}

                //// Combo default aligned with SS3Link: optionally add Metal
                //CombBox_Add("Metal", ref comb);


                // Sort by name and refill the combo
                var items = comb.Items.OfType<string>().ToList();
                items.Sort();
                comb.Items.Clear();
                foreach (var s in items)
                {
                    CombBox_Add(s, ref comb);
                }

                RevitLNK.MateData = RevitLNK.MateData.OrderBy(a => a.stbmatName).ToList();
                for (int i = 0; i < RevitLNK.MateData.Count(); i++)
                {
                    DGV.Rows.Add();
                    DGV.Rows[i].Cells[0].ReadOnly = true;
                    DGV.Rows[i].Cells[0].Value = RevitLNK.MateData[i].stbmatName;

                    bool setflag = false;
                    if (SteelMaterial.ContainsKey(RevitLNK.MateData[i].stbmatName))
                    {
                        // Use mapping from table file when present
                        string matname = SteelMaterial[RevitLNK.MateData[i].stbmatName];

                        var m = RevitLNK.LoFa.RevitMatName.FirstOrDefault(a => a.Name == matname);
                        if (m != null)
                        {
                            DGV.Rows[i].Cells[1].Value = m.Name;
                            setflag = true;
                        }
                    }

                    if (!setflag)
                    {
                        // Otherwise pick an existing material whose name contains Metal (EN or JP Revit)
                        var m = RevitLNK.LoFa.RevitMatName.FirstOrDefault(a =>
                            a.Name.Contains("Metal") || a.Name.Contains("メタル")); // Japanese-localized Revit material category name for Metal
                        if (m != null)
                        {
                            DGV.Rows[i].Cells[1].Value = m.Name;
                            setflag = true;
                        }

                    }

                    if (!setflag)
                    {
                        // Top of combo list
                        DGV.Rows[i].Cells[1].Value = comb.Items[0];
                    }

                    //// Optionally default to Revit materials whose names contain STB steel name
                    //var m = RevitLNK.LoFa.RevitMatName.Where(a => a.Name.Contains(RevitLNK.MateData[i].stbmatName)).ToList();
                    //if (m.Count > 0)
                    //{
                    //    // Sort by name and pick first combo entry
                    //    DGV.Rows[i].Cells[1].Value = m.OrderBy(a => a.Name).First().Name;
                    //}
                    //else
                    //{
                    //    DGV.Rows[i].Cells[1].Value = "Metal";
                    //}

                    //// Set defaults from project: STB material, Revit material, pairs...
                    //for (int j = 0; j < split.Length; j+=2)
                    //{
                    //    if (split[j].Trim().ToUpper() == RevitLNK.MateData[i].stbmatName.ToUpper())
                    //    {
                    //        if (j + 1 < split.Length && comb.Items.Contains(split[j + 1].Trim()) == true)
                    //        {
                    //            DGV.Rows[i].Cells[1].Value = split[j + 1].Trim();
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                groupBox2.Enabled = false;
                DGV.Visible = false;
            }
            // Concrete materials
            if (RevitLNK.ConcData.Count() != 0)
            {
                bool cftflg = false, rcflg = false;
                for (int i = 0; i < RevitLNK.ConcData.Count(); i++)
                {
                    if (cftflg && rcflg) break;
                    if (RevitLNK.ConcData[i].kouzou == "CFT")
                    {
                        cftflg = true;
                    }
                    else
                    {
                        rcflg = true;
                    }
                }
                //// Load project parameters
                //Autodesk.Revit.DB.ProjectInfo pinfo = Commons.doc.ProjectInformation;
                //Autodesk.Revit.DB.Parameter p = pinfo.LookupParameter(Data.projectParams[4]);
                string rcsrc = "Concrete-CONCR Fc##";
                string cft = "Concrete-CONCR Fc##";
                //if (p != null)
                //{
                //    string str = p.AsString();
                //    string[] split;
                //    string[] jouken = { "," };
                //    split = str.Split(jouken, StringSplitOptions.None);
                //    if(split[0] != null)
                //    { rcsrc = split[0]; }
                //    if(split.Count() > 1)
                //    { cft = split[1]; }
                //}
                if (rcflg)
                { textBox1.Text = rcsrc; }
                else { label1.Enabled = false; textBox1.Enabled = false; }
                if (cftflg) { textBox2.Text = cft; }
                else { label2.Enabled = false; textBox2.Enabled = false; }
            }
            else
            {
                groupBox1.Enabled = false;
            }

            // groupBox2 control layout
            groupBox2.ClientSize = new Size(DGV.Width + len * 2, DGV.Height + len * 3);
            DGV.Top = len * 2;
            DGV.Left = len;

            // groupBox1 control layout
            groupBox1.ClientSize = new Size(groupBox2.Width, groupBox2.Height);
            label1.Top = len * 2;
            label1.Left = len;
            label2.Top = textBox1.Bottom + len;
            label2.Left = len;
            textBox1.Width = groupBox1.ClientSize.Width - label1.Width - len * 3;
            textBox2.Width = textBox1.Width;
            textBox1.Top = label1.Bottom - textBox1.Height;
            textBox1.Left = label1.Right + len;
            textBox2.Top = label2.Bottom - textBox2.Height;
            textBox2.Left = textBox1.Left;
            label3.Top = textBox2.Bottom + len;
            label3.Left = len;
            int gx1 = len + label1.Width + len + textBox1.Width + len;
            int gy1 = len + textBox1.Height + len + textBox2.Height + len + label3.Height + len;

            // Form size
            int x = len + groupBox1.Width + len + groupBox2.Width + len;
            int y = len + groupBox2.Height + len + OK.Height + len;
            this.ClientSize = new Size(x, y);

            // Position controls
            groupBox1.Left = len;
            groupBox1.Top = len;
            groupBox2.Left = groupBox1.Right + len;
            groupBox2.Top = len;
            OK.Top = groupBox2.Bottom + len;
            OK.Left = this.ClientSize.Width - len - Cancel.Width - len / 2 - OK.Width;
            Cancel.Top = OK.Top;
            Cancel.Left = OK.Right + len / 2;
            Back.Top = OK.Top;
            Back.Left = OK.Left - len / 2 - Back.Width;
            linkLabel1.Top = this.ClientSize.Height - len - linkLabel1.Height;
            linkLabel1.Left = len;
            linkLabel2.Top = linkLabel1.Top;
            linkLabel2.Left = linkLabel1.Right + len / 2;


            first = false;
        }
        private void OK_Click(object sender, EventArgs e)
        {
            // Validate required fields
            bool rcconflg = false;
            bool cftconflg = false;
            bool tekkotuflg = false;
            string messagetxt = "";
            if(textBox1.Enabled && textBox1.Text == "")
            {
                rcconflg = true;
               
            }
            if (textBox2.Enabled && textBox2.Text == "")
            {
                cftconflg = true;
               
            }
            for(int i = 0; i < DGV.Rows.Count; i++)
            {
                if(DGV.Rows[i].Cells[1].Value == null)
                {
                    tekkotuflg = true;
                    break;
                }
                if(DGV.Rows[i].Cells[1].Value.ToString() == "")
                {
                    tekkotuflg = true;
                    break;                    
                }
            }

            if (rcconflg && !cftconflg && !tekkotuflg)
            {
                messagetxt = "Material for concrete RC/SRC is not specified.";
            }
            else if (cftconflg && !rcconflg && !tekkotuflg)
            {
                messagetxt = "Material for concrete CFT is not specified.";
            }
            else if (rcconflg || cftconflg || tekkotuflg)
            {
                messagetxt = "";                
                if (rcconflg)
                {
                    messagetxt += "Concrete RC/SRC";
                }
                if (cftconflg)
                {
                    if (messagetxt == "")
                    { messagetxt += "Concrete CFT"; }
                    else
                    { messagetxt += ", Concrete CFT"; }
                }
                if (tekkotuflg)
                {
                    if (messagetxt == "")
                    { messagetxt += "Steel"; }
                    else
                    { messagetxt += ", Steel"; }
                }
                messagetxt += " material mapping is incomplete.";
               
            }
            if(messagetxt != "")
            {
                MessageBox.Show(messagetxt, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            //string okmessage = "Start conversion?";
            //DialogResult dr = MessageBox.Show(okmessage, RevitLNK.formtitle + " [Ver." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "]", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (dr == DialogResult.Yes)
            {
                //this.DialogResult = DialogResult.OK;

                //// Persist concrete naming for project metadata
                //ConvertForm.Concname = new List<string>
                //{
                //    textBox1.Text,
                //    textBox2.Text
                //};

                //ConvertForm.TekkotuPare = new List<ConvertForm.Tekkotu>();

                bool updateflag = false;
                for (int i = 0; i < DGV.RowCount; i++)
                {
                    for (int j = 0; j < RevitLNK.MateData.Count(); j++)
                    {
                        if ((string)DGV.Rows[i].Cells[0].Value == RevitLNK.MateData[j].stbmatName)
                        {
                            RevitLNK.MateData[j].RevitmatName = (string)DGV.Rows[i].Cells[1].Value.ToString();
                            //// Persist steel naming for project metadata
                            //ConvertForm.Tekkotu t = new ConvertForm.Tekkotu();
                            //t.RVT = (string)DGV.Rows[i].Cells[1].Value.ToString();
                            //t.STB = (string)DGV.Rows[i].Cells[0].Value.ToString();
                            //ConvertForm.TekkotuPare.Add(t);
                            break;
                        }
                    }

                    // Update material mapping table
                    string m0 = DGV.Rows[i].Cells[0].Value.ToString();
                    string m1 = DGV.Rows[i].Cells[1].Value.ToString();

                    if (SteelMaterial.ContainsKey(m0))
                    {
                        if (SteelMaterial[m0] != m1)
                        {
                            // Changed
                            SteelMaterial[m0] = m1;
                            updateflag = true;
                        }
                    }
                    else
                    {
                        // New STB material entry
                        SteelMaterial.Add(m0, m1);
                        updateflag = true;
                    }
                }
                if (updateflag)
                {
                    WriteMaterialMapping();
                }


                // Concrete Revit names
                for (int i = 0; i < RevitLNK.ConcData.Count(); i++)
                {
                    int strength = Data.Get_Num(RevitLNK.ConcData[i].STBstrength);
                    if (RevitLNK.ConcData[i].kouzou == "CFT")
                    {
                        RevitLNK.ConcData[i].Revitname = textBox2.Text.Replace("##", strength.ToString());

                    }
                    else
                    {
                        RevitLNK.ConcData[i].Revitname = textBox1.Text.Replace("##", strength.ToString());
                    }
                }
            }
            //else
            //{
            //    this.DialogResult = DialogResult.None;
            //}

            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void Back_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }

        #region DGV Escape key handling
        private void DGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                // Check target column
                DataGridView dgv = (DataGridView)sender;
                if (dgv.CurrentCell.OwningColumn.Name == "Combbox")
                {
                    // Editing combo on grid
                    DataGridViewComboBoxEditingControl cb =
                        (DataGridViewComboBoxEditingControl)e.Control;
                    cb.DropDownStyle = ComboBoxStyle.DropDown;

                    cb.KeyDown -= new KeyEventHandler(keydown);
                    cb.KeyDown += new KeyEventHandler(keydown);

                    cb.PreviewKeyDown -= new PreviewKeyDownEventHandler(prekeydown);
                    cb.PreviewKeyDown += new PreviewKeyDownEventHandler(prekeydown);

                    // Prevent typing backslash
                    cb.KeyPress -= new KeyPressEventHandler(prekeypress);
                    cb.KeyPress += new KeyPressEventHandler(prekeypress);
                }
            }
        }

        private bool closeflg = true;

        private void keydown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape && closeflg == true)
            {
                this.Close();
            }
           
            closeflg = true;
        }

        private void prekeydown(object sender, PreviewKeyDownEventArgs e)
        {
            DataGridViewComboBoxEditingControl cb = (DataGridViewComboBoxEditingControl)sender;
            if (cb.DroppedDown == false)
            {
            }
            else
            {
                // Dropdown open
                if (e.KeyData == Keys.Escape)
                {
                    closeflg = false;
                }
                
            }
        }

        private void prekeypress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\\':
                case ':':
                case '{':
                case '}':
                case '[':
                case ']':
                case '|':
                case ';':
                case '<':
                case '>':
                case '?':
                case '`':
                case '~':
                    // Disallowed characters; programmatic creation fails on these (UI-created names may still contain them).
                    e.Handled = true;
                    MessageBox.Show(@"The characters \ : { } [ ] | ; < > ? ` ~ cannot be used.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }





        #endregion

        private void DGV_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            // Check target column
            if (dgv.Columns[e.ColumnIndex].Name == "Combbox" &&
                dgv.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                DataGridViewComboBoxColumn cbc =
                    (DataGridViewComboBoxColumn)dgv.Columns[e.ColumnIndex];
                // Add typed value to combo when missing
                if (!cbc.Items.Contains(e.FormattedValue))
                {
                    cbc.Items.Add(e.FormattedValue);
                    // Assign cell value or edit reverts
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (string)e.FormattedValue;
                }

                
            }
        }

        private void MaterialMapping_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            linkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
            f.Dispose();
        }
        private void MaterialMapping_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }
        
        private void CombBox_Add(string str, ref DataGridViewComboBoxColumn comb)
        {
            bool addflg = true;
            for (int c = 0; c < comb.Items.Count; c++)
            {
                if (comb.Items[c].ToString() == str)
                {
                    addflg = false;
                    break;
                }
            }
            if (addflg) { comb.Items.Add(str); }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Suppress system beep on Enter/Escape in text boxes
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }



        private void ReadMaterialMapping()
        {
            SteelMaterial = new Dictionary<string, string>();

            string mydocu = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
            string filepath = mydocu + RevitLNK.mydocuFileFolderName + RevitLNK.Configuration + "\\" + RevitLNK.MaterialMappingTbl;

            if (File.Exists(filepath))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                // Skip header row
                var data = File.ReadAllLines(filepath, Encoding.GetEncoding("Shift_JIS")).Skip(1);
                foreach (string s in data)
                {
                    var s2 = s.Split(',');
                    if (s2.Length >= 2)
                    {
                        if (SteelMaterial.ContainsKey(s2[0]))
                        {
                            SteelMaterial[s2[0]] = s2[1];
                        }
                        else
                        {
                            SteelMaterial.Add(s2[0], s2[1]);
                        }
                    }
                }
            }
        }

        private void WriteMaterialMapping()
        {
            string mydocu = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
            string filepath = mydocu + RevitLNK.mydocuFileFolderName + RevitLNK.Configuration + "\\" + RevitLNK.MaterialMappingTbl;

            if (SteelMaterial.Count > 0)
            {
                List<string> data = new List<string>()
                {
                    "Steel strength, Material name",
                };

                foreach (var s in SteelMaterial)
                {
                    data.Add(s.Key + "," + s.Value);
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                File.WriteAllLines(filepath, data, Encoding.GetEncoding("Shift_JIS"));
            }
        }

    }

}
