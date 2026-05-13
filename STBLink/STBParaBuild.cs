using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace STBLink
{
    public partial class STBParaBuild : Form
    {
        public STBParaBuild()
        {
            InitializeComponent();
        }

        CheckBox[][] ch = { new CheckBox[5],
                            new CheckBox[8],
                            new CheckBox[8],
                            new CheckBox[2] };

        Label[][] lab = {new Label[5],
                         new Label[8],
                         new Label[8],
                         new Label[2] };

        Button[][] errbt = { new Button[5],
                             new Button[8],
                             new Button[8],
                             new Button[2] };

        Label[][] errlab = {new Label[5],
                            new Label[8],
                            new Label[8],
                            new Label[2] };

        GroupBox[] group = new GroupBox[3];



        // CheckBox on/off state
        bool[][] ChClm = new bool[0][] { };
        //bool[][] ChBClm = new bool[0][] { };
        bool[][] ChGir = new bool[0][] { };
        bool[][] ChBeam = new bool[0][] { };
        bool[][] ChCGir = new bool[0][] { };
        bool[][] ChCBeam = new bool[0][] { };
        bool[][] ChSBra = new bool[0][] { };
        bool[][] ChSlab = new bool[0][] { };
        bool[][] ChFSlab = new bool[0][] { };
        bool[][] ChWall = new bool[0][] { };
        bool[][] ChFound = new bool[0][] { };

        string[][] ChClm_name = new string[0][] { };
        string[][] ChGir_name = new string[0][] { };
        string[][] ChBeam_name = new string[0][] { };
        string[][] ChCGir_name = new string[0][] { };
        string[][] ChCBeam_name = new string[0][] { };
        string[][] ChSBra_name = new string[0][] { };
        string[][] ChSlab_name = new string[0][] { };
        string[][] ChFSlab_name = new string[0][] { };
        string[][] ChWall_name = new string[0][] { };
        string[][] ChFound_name = new string[0][] { };

        // Distance from control to form edge
        const int len = 10;
        // Spacing between member labels and checkboxes
        int interval = 240;
        // Distance between label top and text box bottom
        const int p5 = 2;
        // Distance between label bottom and text box top
        const int p2 = 2;



        private void STBParaBuild_Load(object sender, EventArgs e)
        {
            // Form title (overrides designer text)
            this.Text = RevitLNK.formtitle + " Batch Parameter Add " + Commons.GetVersion();

            LogData.Data = new List<LogData.Log>();

            // Initialize CheckBox on/off state
            ChflgSet(RevitLNK.ClmText2, ref ChClm, ref ChClm_name);
            //ChflgSet(RevitLNK.FClmText, ref ChBClm);
            ChflgSet(RevitLNK.GirText, ref ChGir, ref ChGir_name);
            ChflgSet(RevitLNK.BeamText, ref ChBeam, ref ChBeam_name);
            ChflgSet(RevitLNK.CGirText, ref ChCGir, ref ChCGir_name);
            ChflgSet(RevitLNK.CBeamText, ref ChCBeam, ref ChCBeam_name);
            ChflgSet(RevitLNK.SBraText, ref ChSBra, ref ChSBra_name);
            ChflgSet(RevitLNK.SlabText, ref ChSlab, ref ChSlab_name);
            ChflgSet(RevitLNK.WallText, ref ChWall, ref ChWall_name);
            ChflgSet(RevitLNK.FoundationText2, ref ChFound, ref ChFound_name);
            Array.Resize(ref ChFSlab, 1);
            Array.Resize(ref ChFSlab[0], 1);
            Array.Resize(ref ChFSlab_name, 1);
            Array.Resize(ref ChFSlab_name[0], 1);
            ChFSlab[0][0] = true;

            ch[0][0] = ch1_1;
            ch[0][1] = ch1_2;
            ch[0][2] = ch1_3;
            ch[0][3] = ch1_4;
            ch[0][4] = ch1_5;
            ch[1][0] = ch2_1;
            ch[1][1] = ch2_2;
            ch[1][2] = ch2_3;
            ch[1][3] = ch2_4;
            ch[1][4] = ch2_5;
            ch[1][5] = ch2_6;
            ch[1][6] = ch2_7;
            ch[1][7] = ch2_8;
            ch[2][0] = ch3_1;
            ch[2][1] = ch3_2;
            ch[2][2] = ch3_3;
            ch[2][3] = ch3_4;
            ch[2][4] = ch3_5;
            ch[2][5] = ch3_6;
            ch[2][6] = ch3_7;
            ch[2][7] = ch3_8;
            ch[3][0] = ch4_1;
            ch[3][1] = ch4_2;

            lab[0][0] = lab1_1;
            lab[0][1] = lab1_2;
            lab[0][2] = lab1_3;
            lab[0][3] = lab1_4;
            lab[0][4] = lab1_5;
            lab[1][0] = lab2_1;
            lab[1][1] = lab2_2;
            lab[1][2] = lab2_3;
            lab[1][3] = lab2_4;
            lab[1][4] = lab2_5;
            lab[1][5] = lab2_6;
            lab[1][6] = lab2_7;
            lab[1][7] = lab2_8;
            lab[2][0] = lab3_1;
            lab[2][1] = lab3_2;
            lab[2][2] = lab3_3;
            lab[2][3] = lab3_4;
            lab[2][4] = lab3_5;
            lab[2][5] = lab3_6;
            lab[2][6] = lab3_7;
            lab[2][7] = lab3_8;
            lab[3][0] = lab4_1;
            lab[3][1] = lab4_2;

            for (int i = 0; i < errbt.Count(); i++)
            {
                for (int j = 0; j < errbt[i].Count(); j++)
                {
                    errbt[i][j] = new Button();
                    errbt[i][j].Click += Button_Click;
                    errbt[i][j].Text = "Load";
                    errbt[i][j].Width = 45;
                    errbt[i][j].Height = 19;
                    errbt[i][j].Name = i.ToString() + "_" + j.ToString();
                    errlab[i][j] = new Label();
                    errlab[i][j].AutoSize = true;
                    errlab[i][j].Text = "Cannot add parameters (not loaded)";
                    errlab[i][j].ForeColor = Color.Red;
                    groupBox1.Controls.Add(errbt[i][j]);
                    groupBox1.Controls.Add(errlab[i][j]);
                }
            }
            interval = errlab[0][0].Width + len + errbt[0][0].Width + len;

            group[0] = groupBox2;
            group[1] = groupBox3;
            group[2] = groupBox4;
            AllControl_Init();
            
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }

            // groupBox1 size
            groupBox1.ClientSize = new Size(interval * 3, ch[1][7].Bottom + errlab[0][0].Height + len + button1.Height + len);

            // Form size
            int x = len + listBox1.Width + len + groupBox1.Width + len;
            int y = len + groupBox1.Height + len + OK.Height + len;
            this.ClientSize = new Size(x, y);

            // listBox1 position
            listBox1.Left = len;
            listBox1.Top = len;
            // groupBox1 position
            groupBox1.Left = listBox1.Width + len * 2;
            groupBox1.Top = len;
            // Button and label positions (spacing between button and link labels is len/2)
            OK.Top = groupBox1.Bottom + len;
            OK.Left = this.ClientSize.Width - len - Cancel.Width - len / 2 - OK.Width;
            Cancel.Top = OK.Top;
            Cancel.Left = OK.Right + len / 2;
            linkLabel1.Top = this.ClientSize.Height - len - linkLabel1.Height;
            linkLabel1.Left = len;
            linkLabel2.Top = linkLabel1.Top;
            linkLabel2.Left = linkLabel1.Right + len / 2;
            button1.Top = groupBox1.Height - len - button1.Height;
            button1.Left = groupBox1.Width - len / 2 - button2.Width - len - button1.Width;
            button2.Top = button1.Top;
            button2.Left = button1.Right + len / 2;
            

            this.StartPosition = FormStartPosition.CenterScreen;

           
            listBox1.SetSelected(0, true);
        }

        /// <summary> Initializes CheckBox on/off state
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Chflg"></param>
        private void ChflgSet(string[][] Text, ref bool[][] Chflg, ref string[][] Chname)
        {
            Array.Resize(ref Chflg, Text.Length);
            Array.Resize(ref Chname, Text.Length);
            for (int i = 0; i < Chflg.Length; i++)
            {
                Array.Resize(ref Chflg[i], Text[i].Length);
                Array.Resize(ref Chname[i], Text[i].Length);
            }
            for (int i = 0; i < Chflg.Length; i++)
            {
                for (int j = 0; j < Chflg[i].Length; j++)
                {
                    Chflg[i][j] = true;
                }
                for (int j = 0; j < Chname[i].Length; j++)
                {
                    Chname[i][j] = "";
                }
            }
        }
      

        /// <summary> Lay out controls
        /// </summary>
        private void AllControl_Set()
        {
            for (int i = 0; i < lab.Count(); i++)
            {
                for (int j = 0; j < lab[i].Count(); j++)
                {
                    lab[i][j].Visible = false;
                    lab[i][j].AutoSize = true;
                    if (j == 0)
                    {
                        lab[i][j].Top = len * 2;
                    }
                    else
                    {
                        lab[i][j].Top = ch[i][j - 1].Bottom + len * 2;
                    }

                    if (i == 0)
                    {
                        lab[i][j].Left = len;
                    }
                    else
                    {
                        lab[i][j].Left = len + interval * i;
                    }

                    ch[i][j].Visible = false;
                    ch[i][j].Top = lab[i][j].Bottom - p5;
                    ch[i][j].Left = lab[i][j].Left;

                    errlab[i][j].Visible = false;
                    errbt[i][j].Visible = false;
                }
            }

            for (int i = 0; i < group.Count(); i++)
            {
                group[i].Visible = false;
            }
        }
        private void AllControl_Init()
        {
            AllControl_Set();
            bool flg = true;

            // Lay out controls for columns
            flg = true;
            for (int i = 0; i < SetFamily.ClmFName.FamilyName.Length; i++)
            {
                for (int j = j = 0; j < SetFamily.ClmFName.FamilyName[i].Length; j++)
                {
                    if (i == SetFamily.ClmFName.FamilyName.Length - 1)
                    {
                        int newi = i - 1;
                        int newj = j + SetFamily.ClmFName.FamilyName[i - 1].Length;
                        ChClm[newi][newj] = SetFamily.ClmFName.flg[i][j];
                    }
                    else
                    {
                        ChClm[i][j] = SetFamily.ClmFName.flg[i][j];
                    }
                    if (!SetFamily.ClmFName.flg[i][j]) { flg = false; }
                }
            }
            if(!flg)
            {
                if(!listBox1.Items[0].ToString().Contains("Not loaded"))
                { listBox1.Items[0] = listboxtext(listBox1.Items[0].ToString()); }
            }
            else
            {
                listBox1.Items[0] = listboxtext_del(listBox1.Items[0].ToString());
            }
            // Girder checkbox layout
            flg = true;
            for (int i = 0; i < RevitLNK.GirText.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.GirText[i].Length; j++)
                {
                    ChGir[i][j] = SetFamily.GirFName.flg[i][j];
                    if (!SetFamily.GirFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[1].ToString().Contains("Not loaded"))
                { listBox1.Items[1] = string.Format(listboxtext(listBox1.Items[1].ToString())); }
            }
            else
            {
                listBox1.Items[1] = listboxtext_del(listBox1.Items[1].ToString());
            }
            // Beam checkbox layout
            flg = true;
            for (int i = 0; i < RevitLNK.BeamText.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.BeamText[i].Length; j++)
                {
                    ChBeam[i][j] = SetFamily.BeamFName.flg[i][j];
                    if (!SetFamily.BeamFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[2].ToString().Contains("Not loaded"))
                { listBox1.Items[2] = string.Format(listboxtext(listBox1.Items[2].ToString())); }
            }
            else
            {
                listBox1.Items[2] = listboxtext_del(listBox1.Items[2].ToString());
            }
            // Cantilever girder checkbox layout
            flg = true;
            for (int i = 0; i < RevitLNK.CGirText.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.CGirText[i].Length; j++)
                {
                    ChCGir[i][j] = SetFamily.CGirFName.flg[i][j];
                    if (!SetFamily.CGirFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[3].ToString().Contains("Not loaded"))
                { listBox1.Items[3] = string.Format(listboxtext(listBox1.Items[3].ToString())); }
            }
            else
            {
                listBox1.Items[3] = listboxtext_del(listBox1.Items[3].ToString());
            }
            // Cantilever beam checkbox layout
            flg = true;
            for (int i = 0; i < RevitLNK.CBeamText.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.CBeamText[i].Length; j++)
                {
                    ChCBeam[i][j] = SetFamily.CBeamFName.flg[i][j];
                    if (!SetFamily.CBeamFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[4].ToString().Contains("Not loaded"))
                { listBox1.Items[4] = string.Format(listboxtext(listBox1.Items[4].ToString())); }
            }
            else
            {
                listBox1.Items[4] = listboxtext_del(listBox1.Items[4].ToString());
            }
            // Brace checkboxes
            flg = true;
            for (int i = 0; i < RevitLNK.SBraText.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.SBraText[i].Length; j++)
                {
                    ChSBra[i][j] = SetFamily.SBraFName.flg[i][j];
                    if (!SetFamily.SBraFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[5].ToString().Contains("Not loaded"))
                { listBox1.Items[5] = string.Format(listboxtext(listBox1.Items[5].ToString())); }
            }
            else
            {
                listBox1.Items[5] = listboxtext_del(listBox1.Items[5].ToString());
            }
            // Foundation checkbox layout
            flg = true;
            for (int i = 0; i < RevitLNK.FoundationText2.Length; i++)
            {
                for (int j = j = 0; j < RevitLNK.FoundationText2[i].Length; j++)
                {
                    ChFound[i][j] = SetFamily.FoFName.flg[i][j];
                    if (!SetFamily.FoFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[8].ToString().Contains("Not loaded"))
                { listBox1.Items[8] = string.Format(listboxtext(listBox1.Items[8].ToString())); }
            }
            else
            {
                listBox1.Items[8] = listboxtext_del(listBox1.Items[8].ToString());
            }
        }

        /// <summary> Appends the not-loaded marker to list text
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string listboxtext(string str)
        {
            string ret = str;
            if (str.Contains("Not loaded")) { return ret; }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding sjis = System.Text.Encoding.GetEncoding("Shift_JIS");
            string mi = "<Not loaded>";
            int templen = sjis.GetByteCount(str);
            //mi = mi.PadLeft(36 - (sjis.GetByteCount(mi) - mi.Length) - (templen - str.Length) - templen);       
            if (templen < 16)
            {
                ret += "\t";

                for (int i = 0; i < 5; i++)
                {
                    ret += "　";
                }
            }
            else
            { ret += "　"; }
            ret += mi;
           
            return ret;
        }
        internal static string listboxtext_del(string str)
        {
            string ret = str;

            if (!str.Contains("Not loaded")) { return ret; }
            if(str.Contains("\t"))
            {
                ret = ret.Replace("\t", "");
            }
            do
            {
                ret = ret.Replace("　","");
            } while (ret.Contains("　"));
            ret = ret.Replace("<Not loaded>", "");

            return ret;
        }

        /// <summary> Sets control Visible to false
        /// </summary>
        private void AllControl_Reset()
        {
            AllControl_Set();

            for (int i = 0; i < lab.Count(); i++)
            {
                for (int j = 0; j < lab[i].Count(); j++)
                {
                    lab[i][j].Visible = false;
                    ch[i][j].Visible = false;
                    errbt[i][j].Visible = false;
                    errlab[i][j].Visible = false;
                }
            }

            for (int i = 0; i < group.Count(); i++)
            {
                group[i].Visible = false;
            }
            button1.Visible = true;
            button2.Visible = true;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            int listindex = listBox1.SelectedIndex;
            Button bt = (Button)sender;
            string familyfile = "";
            string faminame = "";
            string rfaname = "";
            int bi = 0, bj = 0;
            bool flg = false;
            if (bt.Text == "Load")
            {
                for (int i = 0; i < errbt.Count(); i++)
                {
                    if (flg) { break; }
                    for (int j = 0; j < errbt[i].Count(); j++)
                    {
                        if (errbt[i][j].Name == bt.Name)
                        {
                            rfaname = ch[i][j].Text + ".rfa";
                            faminame = ch[i][j].Text;
                            bi = i;
                            bj = j;
                            flg = true;
                            break;
                        }
                    }
                }

                OpenFileDialog opf = new OpenFileDialog();
                opf.Title = RevitLNK.formtitle + " Family Selection";
                opf.Filter = rfaname + "|" + rfaname + "|" + "Revit Family (*.rfa)|*.rfa|All files|*.*";
                opf.FileName = rfaname;
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    familyfile = opf.FileName;
                    // Selected file must match mapped family filename
                    if (faminame != System.IO.Path.GetFileNameWithoutExtension(familyfile))
                    {
                        string mes = lab[bi][bj].Text + ": Select " + faminame + ".";
                        MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    if (ReloadFamily(familyfile, faminame))
                    {
                        for(int i = 0; i < ch.Count(); i++)
                        {
                            for(int j = 0; j < ch[i].Count(); j++)
                            {
                                if(ch[bi][bj].Text == ch[i][j].Text)
                                {
                                    ch[i][j].Checked = true;
                                }
                            }
                        }
                        // Keeps grouped controls intact in group boxes
                        selectlistbox(listindex);                       
                    }
                    else
                    {
                        string catename = "";
                        switch (listBox1.SelectedIndex)
                        {
                            case 0:                         
                                catename = "Structural Column";
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                catename = "Structural Framing";
                                break;
                            case 8:
                                catename = "Structural Foundation";
                                break;
                        }
                        string mes = "Verify the family category is [" + catename + "].";
                        MessageBox.Show(lab[bi][bj].Text + ": Failed to load " + faminame + ".\r\n" + mes,
                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    
        private bool ReloadFamily(string FamilyFile,  string familyName)
        {
            bool retcode = false;
            Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(Commons.doc, "Load Family");
            STBLink.FamilyOption famop = new FamilyOption();
            try
            {
                transaction.Start("Load " + familyName);

                Autodesk.Revit.DB.Family family = null;
                if (Commons.doc.LoadFamily(FamilyFile, famop, out family))
                {
                    if (familyName == family.Name)
                    {
                        
                        retcode = true;
                        // Verify loaded families
                        RevitLNK.LoFa.LoadFfamily_fromProject();
                        // Refresh family mapping for each structural type
                        SetFamily.SetClmFamilyName();
                        SetFamily.SetBClmFamilyName();
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                        SetFamily.SetBraFamilyName();
                        SetFamily.SetFoundationFamilyName();
                        // Refresh controls
                        AllControl_Init();
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.RollBack();
                        retcode = false;
                    }
                }
                else
                {
                    transaction.RollBack();
                    retcode = false;
                }
                
            }
            catch (Exception)
            {
                transaction.RollBack();
                retcode = false;
            }

            //bool flg2 = false;
            //for (int j = 0; j < errlab.Count(); j++)
            //{
            //    for (int k = 0; k < errlab[j].Count(); k++)
            //    {
            //        if (errlab[j][k].Visible)
            //        {
            //            flg2 = true;
            //            break;
            //        }
            //    }
            //    if (flg2) { break; }
            //}
            //if (!flg2)
            //{ listBox1.Items[listBox1.SelectedIndex] = string.Format(listBox1.Items[listBox1.SelectedIndex].ToString().Replace("\t<Not loaded>", ""), listBox1.SelectedIndex); }

            return retcode;
        }
        private void errLab_and_errbt_Set(int i, int j)
        {
            errlab[i][j].Left = ch[i][j].Left;
            errlab[i][j].Top = lab[i][j].Bottom -p5;
            errbt[i][j].Top = errlab[i][j].Bottom - errbt[i][j].Height;
            errbt[i][j].Left = errlab[i][j].Right;
            ch[i][j].Top = errlab[i][j].Bottom -p5;            
            errbt[i][j].Visible = true;
            errlab[i][j].Visible = true;
            if (!listBox1.Items[listBox1.SelectedIndex].ToString().Contains("Not loaded"))
            {
                if (listBox1.SelectedIndex == 8 || listBox1.SelectedIndex == 0)
                { listBox1.Items[listBox1.SelectedIndex] = string.Format(listBox1.Items[listBox1.SelectedIndex].ToString() + "　<Not loaded>", listBox1.SelectedIndex);  }
                else
                { listBox1.Items[listBox1.SelectedIndex] = string.Format(listBox1.Items[listBox1.SelectedIndex].ToString() + "\t<Not loaded>", listBox1.SelectedIndex); }
            }
        }
        /// <summary>List box selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            selectlistbox(listBox1.SelectedIndex);

        }

        private void selectlistbox(int ind, bool flg = true)
        {
            // 0–9: Column/pier/foundation column, Main girder, Beam, Cantilever girder, Cantilever beam,
            // S brace, Slab/deck plate, Wall, Footing/mat/piles, Foundation slab
            bool allcbfalse = true; // When checkboxes can be used, Select All / Clear All stay enabled
            
            switch (ind)
            {
                case 0:
                    if (!flg)
                    {
                        SetFamily.SetClmFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < SetFamily.ClmFName.FamilyName.Length; i++)
                    {
                        for (int j = j = 0; j < SetFamily.ClmFName.FamilyName[i].Length; j++)
                        {
                            if (SetFamily.ClmFName.flg[i][j]) { allcbfalse = false; }

                            if (i == SetFamily.ClmFName.FamilyName.Length - 1)
                            {
                                int newi = i - 1;
                                int newj = j + SetFamily.ClmFName.FamilyName[i - 1].Length;
                                CheckBox_Change(ch[newi][newj], SetFamily.ClmFName.FamilyName[i][j], SetFamily.ClmFName.flg[i][j]);
                                ChClm_name[newi][newj] = SetFamily.ClmFName.FamilyName[i][j];
                                if (!SetFamily.ClmFName.flg[i][j])
                                {
                                    errLab_and_errbt_Set(newi, newj);
                                }
                            }
                            else
                            {
                                CheckBox_Change(ch[i][j], SetFamily.ClmFName.FamilyName[i][j], SetFamily.ClmFName.flg[i][j]);
                                ChClm_name[i][j] = SetFamily.ClmFName.FamilyName[i][j];
                                if (!SetFamily.ClmFName.flg[i][j])
                                {
                                    errLab_and_errbt_Set(i, j);
                                }
                            }
                            
                        }
                    }
                    ControlsSet(RevitLNK.ClmText2);
                    break;
                case 1:
                    if (!flg)
                    {
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.GirText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.GirText[i].Length; j++)
                        {
                            if (SetFamily.GirFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.GirFName.FamilyName[i][j], SetFamily.GirFName.flg[i][j]);
                            ChGir_name[i][j] = SetFamily.GirFName.FamilyName[i][j];
                            if (!SetFamily.GirFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.GirText);
                    break;
                case 2:
                    if (!flg)
                    {
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.BeamText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.BeamText[i].Length; j++)
                        {
                            if (SetFamily.BeamFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.BeamFName.FamilyName[i][j], SetFamily.BeamFName.flg[i][j]);
                            ChBeam_name[i][j] = SetFamily.BeamFName.FamilyName[i][j];
                            if (!SetFamily.BeamFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.BeamText);
                    break;
                case 3:
                    if (!flg)
                    {
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.CGirText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.CGirText[i].Length; j++)
                        {
                            if (SetFamily.CGirFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.CGirFName.FamilyName[i][j], SetFamily.CGirFName.flg[i][j]);
                            ChCGir_name[i][j] = SetFamily.CGirFName.FamilyName[i][j];
                            if (!SetFamily.CGirFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.CGirText);
                    break;
                case 4:
                    if (!flg)
                    {
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.CBeamText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.CBeamText[i].Length; j++)
                        {
                            if (SetFamily.CBeamFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.CBeamFName.FamilyName[i][j], SetFamily.CBeamFName.flg[i][j]);
                            ChCBeam_name[i][j] = SetFamily.CBeamFName.FamilyName[i][j];
                            if (!SetFamily.CBeamFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.CBeamText);
                    break;
                case 5:
                    if (!flg)
                    {
                        SetFamily.SetBraFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.SBraText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.SBraText[i].Length; j++)
                        {
                            if (SetFamily.SBraFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.SBraFName.FamilyName[i][j], SetFamily.SBraFName.flg[i][j]);
                            ChSBra_name[i][j] = SetFamily.SBraFName.FamilyName[i][j];
                            if (!SetFamily.SBraFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.SBraText);
                    break;
                case 6:
                    AllControl_Reset();
                    ControlSave();
                    //ControlsSet(RevitLNK.SlabText);
                    lab[0][0].Text = "RC Slab, Deck Plate";
                    lab[0][0].Visible = true;
                    ch[0][0].Text = "Structural Floor Family";
                    ch[0][0].Enabled = true;
                    ch[0][0].Visible = true;
                    ch[0][0].Checked = ChSlab[0][0];
                    groupBox1.Controls.Add(ch[0][0]);
                    groupBox1.Controls.Add(lab[0][0]);
                    ChSlab_name[0][0] = ch[0][0].Text;
                    button1.Visible = false;
                    button2.Visible = false;
                    break;
                case 7:
                    AllControl_Reset();
                    ControlSave();
                    //ControlsSet(RevitLNK.SlabText);
                    lab[0][0].Text = "Wall, RC Parapet";
                    lab[0][0].Visible = true;
                    ch[0][0].Text = "Structural Wall Family";
                    ch[0][0].Enabled = true;
                    ch[0][0].Visible = true;
                    ch[0][0].Checked = ChWall[0][0];
                    groupBox1.Controls.Add(ch[0][0]);
                    groupBox1.Controls.Add(lab[0][0]);
                    ChWall_name[0][0] = ch[0][0].Text;
                    button1.Visible = false;
                    button2.Visible = false;
                    break;
                case 9:
                    AllControl_Reset();
                    ControlSave();
                    //ControlsSet(RevitLNK.SlabText);
                    lab[0][0].Text = "Foundation Slab";
                    lab[0][0].Visible = true;
                    ch[0][0].Text = "Mat Foundation";
                    ch[0][0].Enabled = true;
                    ch[0][0].Visible = true;
                    ch[0][0].Checked = ChFSlab[0][0];
                    groupBox1.Controls.Add(ch[0][0]);
                    groupBox1.Controls.Add(lab[0][0]);
                    ChFSlab_name[0][0] = ch[0][0].Text;
                    button1.Visible = false;
                    button2.Visible = false;
                    break;
                case 8:
                    if (!flg)
                    {
                        SetFamily.SetFoundationFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < RevitLNK.FoundationText2.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.FoundationText2[i].Length; j++)
                        {
                            if (SetFamily.FoFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.FoFName.FamilyName[i][j], SetFamily.FoFName.flg[i][j]);
                            ChFound_name[i][j] = SetFamily.FoFName.FamilyName[i][j];
                            if (!SetFamily.FoFName.flg[i][j])
                            {
                                errLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(RevitLNK.FoundationText2);
                    break;
            }
                
            
            if (allcbfalse)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }
        private void CheckBox_Change(CheckBox ch, string text, bool flg)
        {
            ch.Text = text;
            ch.Enabled = flg;           
        }
      
        private void ControlSave()
        {

            // Persist current checkbox states
            if (lab1_1.Text == "RC Column")
            { SaveCheckBox(ChClm); }
            //else if (lab1_1.Text == "RC Foundation Column")
            //{ SaveCheckBox(ChBClm); }
            else if (lab1_1.Text == "Foundation Girder")
            { SaveCheckBox(ChGir); }
            else if (lab1_1.Text == "Foundation Beam" )
            { SaveCheckBox(ChBeam); }
            else if (lab1_1.Text == "RC Cantilever Foundation Girder")
            { SaveCheckBox(ChCGir); }
            else if (lab1_1.Text == "RC Cantilever Foundation Beam")
            { SaveCheckBox(ChCBeam); }
            else if (lab1_1.Text == "S Brace H-Section")
            { SaveCheckBox(ChSBra); }
            else if (lab1_1.Text == "RC Slab, Deck Plate")
            { SaveCheckBox(ChSlab); }
            else if (lab1_1.Text == "Wall, RC Parapet")
            { SaveCheckBox(ChWall); }
            else if (lab1_1.Text == "Foundation Slab")
            { SaveCheckBox(ChFSlab); }
            else if (lab1_1.Text == "Foundation Rectangle")
            { SaveCheckBox(ChFound); }
        }

        private void ControlsSet(string[][] str)
        {
            ControlSave();

            // Add and show controls
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < str[i].Length; j++)
                {
                    ch[i][j].Visible = true;
                    lab[i][j].Visible = true;

                    lab[i][j].Text = str[i][j];

                    groupBox1.Controls.Add(ch[i][j]);
                    groupBox1.Controls.Add(lab[i][j]);
                }
            }
            
            // Checkbox state for newly shown controls
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < str[i].Length; j++)
                {
                    if (lab1_1.Text == "RC Column")
                    { ch[i][j].Checked = ChClm[i][j]; }
                    //else if (lab1_1.Text == "RC Foundation Column")
                    //{ ch[i][j].Checked = ChBClm[i][j]; }
                    else if (lab1_1.Text == "Foundation Girder")
                    { ch[i][j].Checked = ChGir[i][j]; }
                    else if (lab1_1.Text == "Foundation Beam")
                    { ch[i][j].Checked = ChBeam[i][j]; }
                    else if (lab1_1.Text == "RC Cantilever Foundation Girder")
                    { ch[i][j].Checked = ChCGir[i][j]; }
                    else if (lab1_1.Text == "RC Cantilever Foundation Beam")
                    { ch[i][j].Checked = ChCBeam[i][j]; }
                    else if (lab1_1.Text == "S Brace H-Section")
                    { ch[i][j].Checked = ChSBra[i][j]; }
                    else if (lab1_1.Text == "RC Slab, Deck Plate")
                    { ch[i][j].Checked = ChSlab[i][j]; }
                    else if (lab1_1.Text == "Wall, RC Parapet")
                    { ch[i][j].Checked = ChWall[i][j]; }
                    else if (lab1_1.Text == "Foundation Slab")
                    { ch[i][j].Checked = ChFSlab[i][j]; }
                    else if (lab1_1.Text == "Foundation Rectangle")
                    { ch[i][j].Checked = ChFound[i][j]; }                    
                }
            }
            
        }

        /// <summary> Persists CheckBox on/off state
        /// </summary>
        /// <param name="flg"></param>
        private void SaveCheckBox(bool[][] flg)
        {
            for (int i = 0; i < flg.Count(); i++)
            {
                for (int j = 0; j < flg[i].Count(); j++)
                {
                    flg[i][j] = ch[i][j].Checked;
                }
            }
        }
       

        /// <summary>Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, EventArgs e)
        {
            ControlSave();
            if (!ConvFlg())
            {
                string mes = "No member types are selected for adding parameters.";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (!System.IO.File.Exists(RevitLNK.sharedParamsFile))
            {
                string mes = "Shared parameter file could not be found.";
                MessageBox.Show(mes + "\r\n\r\n" + RevitLNK.sharedParamsFile, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tuika = "Start batch parameter add now?";
            DialogResult dr = MessageBox.Show(tuika, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            ControlSave();
            this.Enabled = false;
            int listboxind = listBox1.SelectedIndex;

            // Prepare progress bar form
            ProgressBarForm pform = new ProgressBarForm();
            Stopwatch stopw = new Stopwatch();            
            stopw.Start();
            bool pformflg = false;
            string logfamily = "";
            // Columns
            try
            {
                int allchclm = 0; // Number of families to add parameters to
                for (int i = 0; i < ChClm.Length; i++)
                {
                    for (int j = 0; j < ChClm[i].Length; j++)
                    {
                        if (ChClm[i][j])
                        { allchclm++; }
                    }
                }
                if (allchclm != 0)
                {
                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filterClm = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns);
                    IList<Autodesk.Revit.DB.Element> clmel = collector.WherePasses(filterClm).WhereElementIsElementType().ToElements();
                    int clmnum = clmel.Count();
                    logfamily = "Column / pier / foundation column";

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 0;
                    this.Refresh();

                    int endnum = 0; // Families finished so far

                    for (int el = 0; el < clmnum; el++)
                    {
                        bool endflg = false; // Finished applying parameter set for this element

                        Autodesk.Revit.DB.FamilySymbol symbol = clmel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        for (int i = 0; i < ChClm.Length; i++)
                        {
                            if (endflg) { break; }
                            for (int j = 0; j < ChClm[i].Length; j++)
                            {
                                if (!ChClm[i][j]) { continue; }
                                int newi = i;
                                int newj = j;
                                if( i > 1)
                                {                                    
                                    if (j > SetFamily.ClmFName.FamilyName[i].Count() - 1)
                                    {
                                        newi = i + 1;
                                        newj = j - SetFamily.ClmFName.FamilyName[i].Count();
                                    }
                                    
                                }
                                if (symbol.FamilyName != SetFamily.ClmFName.FamilyName[newi][newj]) { continue; }
                                logfamily = symbol.FamilyName;
                                // Show progress
                                if (!pformflg)
                                { pform_Show(pform, ref pformflg); }
                                
                                endnum++;
                                ProgressBar_Show(pform, "Adding column / pier / foundation column parameters " + endnum.ToString() + "/" + allchclm.ToString());
                                fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchclm * 100));
                                Set_Column_Parameter(symbol, SetFamily.ClmFName.FamilyName[newi][newj]);
                                endflg = true;                               
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, logfamily + " failed to add parameters.");
            }

            // Families that already finished parameter addition
            List<Autodesk.Revit.DB.FamilySymbol> AddEndFamilySymbol = new List<Autodesk.Revit.DB.FamilySymbol>();
            // Girders / beams
            try
            {
                Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                Autodesk.Revit.DB.ElementFilter filterGir = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming);
                IList<Autodesk.Revit.DB.Element> girel = collector.WherePasses(filterGir).WhereElementIsElementType().ToElements();
                int num = girel.Count();

                int allchgir = 0; // Number of families to add parameters to
                for (int i = 0; i < ChGir.Length; i++)
                {
                    for (int j = 0; j < ChGir[i].Length; j++)
                    {
                        if (ChGir[i][j])
                        { allchgir++; }
                    }
                }
                if (allchgir != 0)
                {
                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 1;
                    this.Refresh();

                    int endnum = 0; // Families finished so far
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Main girder";
                        Autodesk.Revit.DB.FamilySymbol symbol = girel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }
                        bool addflg = true;
                        for (int k = 0; k < AddEndFamilySymbol.Count(); k++)
                        {
                            if (AddEndFamilySymbol[k] != symbol) { continue; }

                            addflg = false;
                            endnum++;                           
                            break;
                        }
                        if (!addflg) { continue; }
                        for (int i = 0; i < ChGir.Length; i++)
                        {
                            for (int j = 0; j < ChGir[i].Length; j++)
                            {
                                if (!ChGir[i][j]) { continue; } // Only add parameters where the checkbox is checked
                                if (symbol.FamilyName != SetFamily.GirFName.FamilyName[i][j]) { continue; }

                                logfamily = symbol.FamilyName;
                                // Show progress bar
                                if (!pformflg)
                                { pform_Show(pform, ref pformflg); }

                                endnum++;
                                ProgressBar_Show(pform, "Adding main girder parameters " + endnum.ToString() + "/" + allchgir.ToString());
                                fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchgir * 100));

                                Set_Girder_Parameter(symbol, SetFamily.GirFName.FamilyName[i][j]);
                                AddEndFamilySymbol.Add(symbol);                               
                                break;

                            }
                        }
                    }
                }

                int allchbeam = 0; // Number of families to add parameters to
                for (int i = 0; i < ChBeam.Length; i++)
                {
                    for (int j = 0; j < ChBeam[i].Length; j++)
                    {
                        if (ChBeam[i][j])
                        { allchbeam++; }
                    }
                }
                if (allchbeam != 0)
                {
                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 2;
                    this.Refresh();

                    int endnum = 0; // Families finished so far
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Beam";
                        Autodesk.Revit.DB.FamilySymbol symbol = girel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        bool addflg = true;
                        for (int k = 0; k < AddEndFamilySymbol.Count(); k++)
                        {
                            if (AddEndFamilySymbol[k] != symbol) { continue; }

                            addflg = false;
                            endnum++;                          
                            break;

                        }
                        if (!addflg) { continue; }
                        for (int i = 0; i < ChBeam.Length; i++)
                        {
                            for (int j = 0; j < ChBeam[i].Length; j++)
                            {
                                if (!ChBeam[i][j]) { continue; } // Only add parameters where the checkbox is checked
                                if (symbol.FamilyName != SetFamily.BeamFName.FamilyName[i][j]) { continue; }

                                logfamily = symbol.FamilyName;
                                // Show progress bar
                                if (!pformflg)
                                { pform_Show(pform, ref pformflg); }

                                endnum++;
                                ProgressBar_Show(pform, "Adding beam parameters " + endnum.ToString() + "/" + allchbeam.ToString());
                                fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchbeam * 100));

                                Set_Beam_Parameter(symbol, SetFamily.BeamFName.FamilyName[i][j]);
                                AddEndFamilySymbol.Add(symbol);                               
                                break;
                            }
                        }
                    }
                }
                AddEndFamilySymbol = new List<Autodesk.Revit.DB.FamilySymbol>();
                int allchcgir = 0; // Number of families to add parameters to
                for (int i = 0; i < ChCGir.Length; i++)
                {
                    for (int j = 0; j < ChCGir[i].Length; j++)
                    {
                        if (ChCGir[i][j])
                        { allchcgir++; }
                    }
                }
                if (allchcgir != 0)
                {
                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 3;
                    this.Refresh();

                    int endnum = 0; // Families finished so far
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Cantilever girder";
                        Autodesk.Revit.DB.FamilySymbol symbol = girel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        bool addflg = true;
                        for (int k = 0; k < AddEndFamilySymbol.Count(); k++)
                        {
                            if (AddEndFamilySymbol[k] != symbol) { continue; }

                            addflg = false;
                            endnum++;
                            break;

                        }
                        if (!addflg) { continue; }
                        for (int i = 0; i < ChCGir.Length; i++)
                        {
                            for (int j = 0; j < ChCGir[i].Length; j++)
                            {
                                if (ChCGir[i][j] == true) // Only add parameters where the checkbox is checked
                                {
                                    if (symbol.FamilyName != SetFamily.CGirFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    // Show progress bar
                                    if (!pformflg)
                                    { pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding cantilever girder parameters " + endnum.ToString() + "/" + allchcgir.ToString());
                                    fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchcgir * 100));

                                    Set_CGirder_Parameter(symbol, SetFamily.CGirFName.FamilyName[i][j]);
                                    AddEndFamilySymbol.Add(symbol);                                    
                                    break;
                                }
                            }
                        }
                    }
                }

                // Sync UI with the selected member category
                listBox1.SelectedIndex = 4;
                this.Refresh();
                int allchcbeam = 0; // Number of families to add parameters to
                for (int i = 0; i < ChCBeam.Length; i++)
                {
                    for (int j = 0; j < ChCBeam[i].Length; j++)
                    {
                        if (ChCBeam[i][j])
                        { allchcbeam++; }
                    }
                }
                if (allchcbeam != 0)
                {
                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 4;
                    this.Refresh();

                    int endnum = 0; // Families finished so far
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Cantilever beam";
                        Autodesk.Revit.DB.FamilySymbol symbol = girel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        bool addflg = true;
                        for (int k = 0; k < AddEndFamilySymbol.Count(); k++)
                        {
                            if (AddEndFamilySymbol[k] != symbol) { continue; }

                            addflg = false;
                            endnum++;
                            break;

                        }
                        if (!addflg) { continue; }
                        for (int i = 0; i < ChCBeam.Length; i++)
                        {
                            for (int j = 0; j < ChCBeam[i].Length; j++)
                            {
                                if (ChCBeam[i][j] == true) // Only add parameters where the checkbox is checked
                                {
                                    if (symbol.FamilyName != SetFamily.CBeamFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    // Show progress bar
                                    if (!pformflg)
                                    { pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding cantilever beam parameters " + endnum.ToString() + "/" + allchcbeam.ToString());
                                    fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchcbeam * 100));

                                    Set_CBeam_Parameter(symbol, SetFamily.CBeamFName.FamilyName[i][j]);
                                    AddEndFamilySymbol.Add(symbol);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, logfamily + " failed to add parameters.");
            }
            // Braces
            try
            {
                int allchsbra = 0; // Number of families to add parameters to
                for (int i = 0; i < ChSBra.Length; i++)
                {
                    for (int j = 0; j < ChSBra[i].Length; j++)
                    {
                        if (ChSBra[i][j])
                        { allchsbra++; }
                    }
                }
                if (allchsbra != 0)
                {
                    logfamily = "S brace";
                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filterGir = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming);
                    IList<Autodesk.Revit.DB.Element> brael = collector.WherePasses(filterGir).WhereElementIsElementType().ToElements();
                    int num = brael.Count();

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 5;
                    this.Refresh();

                    int endnum = 0; // Families finished so far
                    for (int el = 0; el < num; el++)
                    {
                        bool endflg = false; // Finished applying parameter set for this element
                        Autodesk.Revit.DB.FamilySymbol symbol = brael[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        bool addflg = true;
                        for (int k = 0; k < AddEndFamilySymbol.Count(); k++)
                        {
                            if (AddEndFamilySymbol[k] != symbol) { continue; }

                            addflg = false;
                            endnum++;
                            break;
                        }
                        if (!addflg) { continue; }

                        for (int i = 0; i < ChSBra.Length; i++)
                        {
                            if (endflg) { break; }
                            for (int j = 0; j < ChSBra[i].Length; j++)
                            {
                                if (ChSBra[i][j] == true) // Only add parameters where the checkbox is checked
                                {
                                    if (symbol.FamilyName != SetFamily.SBraFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    // Show progress
                                    if (!pformflg)
                                    { pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding S brace parameters " + endnum.ToString() + "/" + allchsbra.ToString());
                                    fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchsbra * 100));

                                    Set_SBrace_Parameter(symbol, SetFamily.SBraFName.FamilyName[i][j]);
                                    endflg = true;                                   
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, logfamily + " failed to add parameters.");
            }
            // Slabs / floors
            try
            {
                if (ChSlab[0][0]) // Foundation slab, RC slab, and deck plate use the same structural floor family
                {
                    logfamily = "Slab / deck plate";

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 6;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_Floors);
                    IList<Autodesk.Revit.DB.Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = elms.Count();
                    foreach (Autodesk.Revit.DB.Element el in elms)
                    {
                        Autodesk.Revit.DB.FloorType symbol = el as Autodesk.Revit.DB.FloorType;
                        if (symbol != null && symbol.IsFoundationSlab == false)
                        {
                            logfamily = symbol.FamilyName;
                            // Show progress
                            if (!pformflg)
                            { pform_Show(pform, ref pformflg); }
                            ProgressBar_Show(pform, "Adding slab / deck plate parameters");
                            fromSTB.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                            ParaSet.SetPara_Slab("Floors", el, SetFamily.Slab);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to structural floor families.");
            }
            // Walls
            try
            {
                if (ChWall[0][0])
                {
                    logfamily = "Wall / RC parapet";

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 7;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_Walls);
                    IList<Autodesk.Revit.DB.Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = elms.Count();
                    foreach (Autodesk.Revit.DB.Element el in elms)
                    {
                        Autodesk.Revit.DB.WallType symbol = el as Autodesk.Revit.DB.WallType;
                        if (symbol != null && symbol.Kind == Autodesk.Revit.DB.WallKind.Basic)
                        {
                            logfamily = symbol.FamilyName;
                            // Show progress
                            if (!pformflg)
                            { pform_Show(pform, ref pformflg); }
                            ProgressBar_Show(pform, "Adding wall / RC parapet parameters");
                            fromSTB.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                            ParaSet.SetPara_Wall("Walls", symbol, SetFamily.Wall);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to structural wall families.");
            }

            // Foundations
            try
            {
                int allchf = 0; // Number of families to add parameters to
                for (int i = 0; i < ChFound.Length; i++)
                {
                    for (int j = 0; j < ChFound[i].Length; j++)
                    {
                        if (ChFound[i][j])
                        { allchf++; }
                    }
                }
                if (allchf != 0)
                {
                    logfamily = "Footing / mat / piles";
                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation);
                    IList<Autodesk.Revit.DB.Element> fel = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = fel.Count();

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 8;
                    this.Refresh();

                    int endnum = 0; // Families finished so far

                    for (int el = 0; el < num; el++)
                    {
                        bool endflg = false; // Finished applying parameter set for this element
                        Autodesk.Revit.DB.FamilySymbol symbol = fel[el] as Autodesk.Revit.DB.FamilySymbol;
                        if (symbol == null) { continue; }

                        for (int i = 0; i < ChFound.Length; i++)
                        {
                            if (endflg) { break; }
                            for (int j = 0; j < ChFound[i].Length; j++)
                            {
                                if (ChFound[i][j] == true) // Only add parameters where the checkbox is checked
                                {
                                    if (symbol.FamilyName != SetFamily.FoFName.FamilyName[i][j]) { continue; }
                                    logfamily = symbol.FamilyName;
                                    // Show progress
                                    if (!pformflg)
                                    { pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding footing / mat / pile parameters " + endnum.ToString() + "/" + allchf.ToString());
                                    fromSTB.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchf * 100));

                                    Set_SFooting_Parameter(symbol, SetFamily.FoFName.FamilyName[i][j]);
                                    endflg = true;                                    
                                    break;

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, logfamily + " failed to add parameters.");
            }
            // Foundation slab
            try
            {
                if (ChFSlab[0][0])
                {
                    logfamily = "Foundation slab";

                    // Sync UI with the selected member category
                    listBox1.SelectedIndex = 9;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector2 = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter2 = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation);
                    IList<Autodesk.Revit.DB.Element> el2 = collector2.WherePasses(filter2).WhereElementIsElementType().ToElements();
                    int num2 = el2.Count();
                    for (int i = 0; i < num2; i++)
                    {
                        Autodesk.Revit.DB.FloorType symbol = el2[i] as Autodesk.Revit.DB.FloorType;
                        if (symbol == null) { continue; }
                        if(symbol.FamilyName != "Mat Foundation") { continue; }
                        logfamily = symbol.FamilyName;
                        // Show progress
                        if (!pformflg)
                        { pform_Show(pform, ref pformflg); }
                        ProgressBar_Show(pform, "Adding foundation slab parameters");
                        fromSTB.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                        ParaSet.SetPara_Slab("Structural Foundations", symbol, SetFamily.Slab);
                        break;
                    }
                }
            }
            catch
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to the mat foundation family.");
            }

            // Clear progress gauge
            if (this != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                fromSTB.GaugeClose();
            }

            if (!pform.Visible)
            {
                pform.Close();
            }
            else
            {
                listBox1.SelectedIndex = listboxind;
                this.Refresh();
                pform.Close();
                this.Close();
                string mes = "Batch parameter add completed.";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Write log output
                if (LogData.Data.Count() != 0)
                {
                    LogForm lf = new LogForm();
                    lf.Text = RevitLNK.formtitle + " Batch Parameter Add Log " + Commons.GetVersion();
                    lf.ShowDialog();
                }
            }
        }
        
        private bool ConvFlg()
        {
            bool convflg = false;
            for(int i = 0; i < ChClm.Count(); i++)
            {
                if (convflg) { break; }
                for(int j = 0; j < ChClm[i].Count(); j++)
                {
                    if(ChClm[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChGir.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChGir[i].Count(); j++)
                {
                    if (ChGir[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChBeam.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChBeam[i].Count(); j++)
                {
                    if (ChBeam[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChCGir.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChCGir[i].Count(); j++)
                {
                    if (ChCGir[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChCBeam.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChCBeam[i].Count(); j++)
                {
                    if (ChCBeam[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChSBra.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChSBra[i].Count(); j++)
                {
                    if (ChSBra[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChSlab.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChSlab[i].Count(); j++)
                {
                    if (ChSlab[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChFSlab.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChFSlab[i].Count(); j++)
                {
                    if (ChFSlab[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChWall.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChWall[i].Count(); j++)
                {
                    if (ChWall[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            for (int i = 0; i < ChFound.Count(); i++)
            {
                if (convflg) { break; }
                for (int j = 0; j < ChFound[i].Count(); j++)
                {
                    if (ChFound[i][j])
                    {
                        convflg = true;
                        break;
                    }
                }
            }
            return convflg;
        }
        private void pform_Show(ProgressBarForm pform, ref bool pformflg)
        {
            pform.Text = RevitLNK.formtitle + " Adding Parameters";
            pform.Show();
            pform.lab.Visible = true;
            int px = pform.panelFooter.Width + 10;
            int py = pform.lab.Height + pform.panelFooter.Height + 6;
            pform.ClientSize = new Size(px, py);
            pform.panelFooter.Height = pform.ClientSize.Height - pform.lab.Height - 6;
            pform.panelFooter.Width = pform.ClientSize.Width - 6;
            pform.lab.Top = 3;
            pform.lab.Left = 3;
            pform.panelFooter.Top = pform.lab.Bottom;
            pform.panelFooter.Left = pform.lab.Left;
            pformflg = true;
        }
        private void Help_Requested(object sender, HelpEventArgs hlpevent)
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
                string mes = "Help file could not be found.";
                MessageBox.Show(mes + "\r\n\r\n" + helppath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
            f.Dispose();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lab.Count(); i++)
            {
                for (int j = 0; j < lab[i].Count(); j++)
                {
                    if (ch[i][j].Visible == true && ch[i][j].Enabled == true)
                    {
                        ch[i][j].Checked = true;
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lab.Count(); i++)
            {
                for (int j = 0; j < lab[i].Count(); j++)
                {
                    if (ch[i][j].Visible == true && ch[i][j].Enabled == true)
                    {
                        ch[i][j].Checked = false;
                    }
                }
            }
        }
        private void STBParaBuild_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        #region Parameter assignment
        internal bool Set_Column_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                // Column family parameter set
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();
            
                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
                if (FamilyName == SetFamily.RCClmRe.FamilyName)
                {
                    ParaSet.SetPara_RCClmRe(fmg, SetFamily.RCClmRe);
                }
                if (FamilyName == SetFamily.RCClmRo.FamilyName)
                {
                    ParaSet.SetPara_RCClmRo(fmg, SetFamily.RCClmRo);
                }
                if (FamilyName == SetFamily.SClmH.FamilyName)
                {
                    ParaSet.SetPara_SClmH(fmg, SetFamily.SClmH);
                }
                if (FamilyName == SetFamily.SClmBH.FamilyName)
                {
                    ParaSet.SetPara_SClmBH(fmg, SetFamily.SClmBH);
                }
                if (FamilyName == SetFamily.SClmBox.FamilyName)
                {
                    ParaSet.SetPara_SClmBox(fmg, SetFamily.SClmBox);
                }
                if (FamilyName == SetFamily.SClmBBox.FamilyName)
                {
                    ParaSet.SetPara_SClmBBox(fmg, SetFamily.SClmBBox);
                }
                if (FamilyName == SetFamily.SClmPipe.FamilyName)
                {
                    ParaSet.SetPara_SClmPipe(fmg, SetFamily.SClmPipe);
                }
                if (FamilyName == SetFamily.SClmT.FamilyName)
                {
                    ParaSet.SetPara_SClmT(fmg, SetFamily.SClmT);
                }
                if (FamilyName == SetFamily.SClmC.FamilyName)
                {
                    ParaSet.SetPara_SClmC(fmg, SetFamily.SClmC);
                }
                if (FamilyName == SetFamily.SClmL.FamilyName)
                {
                    ParaSet.SetPara_SClmL(fmg, SetFamily.SClmL);
                }
                if (FamilyName == SetFamily.SRCClmH.FamilyName)
                {
                    ParaSet.SetPara_SRCClmH(fmg, SetFamily.SRCClmH);
                }
                if (FamilyName == SetFamily.SRCClmCross.FamilyName)
                {
                    ParaSet.SetPara_SRCClmCross(fmg, SetFamily.SRCClmCross);
                }
                if (FamilyName == SetFamily.SRCClmT.FamilyName)
                {
                    ParaSet.SetPara_SRCClmT(fmg, SetFamily.SRCClmT);
                }
                if (FamilyName == SetFamily.SRCClmH_Rou.FamilyName)
                {
                    ParaSet.SetPara_SRCClmH_Rou(fmg, SetFamily.SRCClmH_Rou);
                }
                if (FamilyName == SetFamily.SRCClmCross_Rou.FamilyName)
                {
                    ParaSet.SetPara_SRCClmCross_Rou(fmg, SetFamily.SRCClmCross_Rou);
                }
                if (FamilyName == SetFamily.SRCClmT_Rou.FamilyName)
                {
                    ParaSet.SetPara_SRCClmT_Rou(fmg, SetFamily.SRCClmT_Rou);
                }
                if (FamilyName == SetFamily.CFTClmBox.FamilyName)
                {
                    ParaSet.SetPara_CFTClmBox(fmg, SetFamily.CFTClmBox);
                }
                if (FamilyName == SetFamily.CFTClmPipe.FamilyName)
                {
                    ParaSet.SetPara_CFTClmPipe(fmg, SetFamily.CFTClmPipe);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch(Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_Girder_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                // Main girder family parameter set
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();

                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
                if (FamilyName == SetFamily.RCGir_F.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_F);
                }
                if (FamilyName == SetFamily.RCGir_F_Haunch.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_F_Haunch);
                }
                if (FamilyName == SetFamily.RCGir.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir);
                }
                if (FamilyName == SetFamily.RCGir_Haunch.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_Haunch);
                }
                if (FamilyName == SetFamily.SGirH.FamilyName)
                {
                    ParaSet.SetPara_SGirH(fmg, SetFamily.SGirH);
                }
                if (FamilyName == SetFamily.SGirBH.FamilyName)
                {
                    ParaSet.SetPara_SGirBH(fmg, SetFamily.SGirBH);
                }
                if (FamilyName == SetFamily.SGirC.FamilyName)
                {
                    ParaSet.SetPara_SGirC(fmg, SetFamily.SGirC);
                }
                if (FamilyName == SetFamily.SGirL.FamilyName)
                {
                    ParaSet.SetPara_SGirL(fmg, SetFamily.SGirL);
                }
                if(FamilyName == SetFamily.SGirLipC.FamilyName)
                {
                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SGirLipC);
                }
                if (FamilyName == SetFamily.SRCGirH.FamilyName)
                {
                    ParaSet.SetPara_SRCGirH(fmg, SetFamily.SRCGirH);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_Beam_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                // Beam family parameter set

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();

                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

                if (FamilyName == SetFamily.RCBeam_F.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_F);
                }
                if (FamilyName == SetFamily.RCBeam_F_Haunch.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_F_Haunch);
                }
                if (FamilyName == SetFamily.RCBeam.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam);
                }
                if (FamilyName == SetFamily.RCBeam_Haunch.FamilyName)
                {
                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_Haunch);
                }
                if (FamilyName == SetFamily.SBeamH.FamilyName)
                {
                    ParaSet.SetPara_SGirH(fmg, SetFamily.SBeamH);
                }
                if (FamilyName == SetFamily.SBeamBH.FamilyName)
                {
                    ParaSet.SetPara_SGirBH(fmg, SetFamily.SBeamBH);
                }
                if (FamilyName == SetFamily.SBeamC.FamilyName)
                {
                    ParaSet.SetPara_SGirC(fmg, SetFamily.SBeamC);
                }
                if (FamilyName == SetFamily.SBeamL.FamilyName)
                {
                    ParaSet.SetPara_SGirL(fmg, SetFamily.SBeamL);
                }
                if (FamilyName == SetFamily.SBeamLipC.FamilyName)
                {
                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SBeamLipC);
                }
                if (FamilyName == SetFamily.SRCBeamH.FamilyName)
                {
                    ParaSet.SetPara_SRCGirH(fmg, SetFamily.SRCBeamH);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_CGirder_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                // Cantilever girder family parameter set
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();

                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

                if (FamilyName == SetFamily.RCCGir.FamilyName)
                {
                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir);
                }
                if(FamilyName == SetFamily.RCCGir_F.FamilyName)
                {
                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir_F);
                }
                if (FamilyName == SetFamily.SCGirH.FamilyName)
                {
                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirH);
                }
                if (FamilyName == SetFamily.SCGirBH.FamilyName)
                {
                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirBH);
                }
                if (FamilyName == SetFamily.SCGirC.FamilyName)
                {
                    ParaSet.SetPara_SGirC(fmg, SetFamily.SCGirC);
                }
                if (FamilyName == SetFamily.SCGirL.FamilyName)
                {
                    ParaSet.SetPara_SGirL(fmg, SetFamily.SCGirL);
                }
                if (FamilyName == SetFamily.SCGirLipC.FamilyName)
                {
                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SCGirLipC);
                }
                if (FamilyName == SetFamily.SRCCGirH.FamilyName)
                {
                    ParaSet.SetPara_SRCCGirH(fmg, SetFamily.SRCCGirH);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);


            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_CBeam_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                // Cantilever beam family parameter set
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();

                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

                if (FamilyName == SetFamily.RCCBeam.FamilyName)
                {
                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCBeam);
                }
                if (FamilyName == SetFamily.SCBeamH.FamilyName)
                {
                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCBeamH);
                }
                if (FamilyName == SetFamily.SCBeamBH.FamilyName)
                {
                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCBeamBH);
                }
                if (FamilyName == SetFamily.SCBeamC.FamilyName)
                {
                    ParaSet.SetPara_SGirC(fmg, SetFamily.SCBeamC);
                }
                if (FamilyName == SetFamily.SCBeamL.FamilyName)
                {
                    ParaSet.SetPara_SGirL(fmg, SetFamily.SCBeamL);
                }
                if (FamilyName == SetFamily.SCBeamLipC.FamilyName)
                {
                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SCBeamLipC);
                }
                if (FamilyName == SetFamily.SRCBeamH.FamilyName)
                {
                    ParaSet.SetPara_SRCCGirH(fmg, SetFamily.SRCCBeamH);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);


            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_SBrace_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                // Brace family parameter set

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();
           
                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

                if (FamilyName == SetFamily.SBraH.FamilyName)
                {
                     ParaSet.SetPara_SBraH(fmg, SetFamily.SBraH);
                }
                if (FamilyName == SetFamily.SBraBH.FamilyName)
                {
                    ParaSet.SetPara_SBraBH(fmg, SetFamily.SBraBH);
                }
                if (FamilyName == SetFamily.SBraBox.FamilyName)
                {
                    ParaSet.SetPara_SBraBox(fmg, SetFamily.SBraBox);
                }
                if (FamilyName == SetFamily.SBraBBox.FamilyName)
                {
                    ParaSet.SetPara_SBraBBox(fmg, SetFamily.SBraBBox);
                }
                if (FamilyName == SetFamily.SBraPipe.FamilyName)
                {
                    ParaSet.SetPara_SBraPipe(fmg, SetFamily.SBraPipe);
                }
                if (FamilyName == SetFamily.SBraC.FamilyName)
                {
                    ParaSet.SetPara_SBraC(fmg, SetFamily.SBraC);
                }
                if (FamilyName == SetFamily.SBraL.FamilyName)
                {
                    ParaSet.SetPara_SBraL(fmg, SetFamily.SBraL);
                }
                if (FamilyName == SetFamily.SBraLipC.FamilyName)
                {
                    ParaSet.SetPara_SBraLipC(fmg, SetFamily.SBraLipC);
                }
                if(FamilyName == SetFamily.SBraFB.FamilyName)
                {
                    ParaSet.SetPara_SBraFB(fmg, SetFamily.SBraFB);
                }
                if (FamilyName == SetFamily.SBraRollBar.FamilyName)
                {
                    ParaSet.SetPara_SBraRollBar(fmg, SetFamily.SBraRollBar);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_SFooting_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                // Foundation family parameter set

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameter");
            try
            {
                tran1.Start();

                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

                if (FamilyName == SetFamily.FRect.FamilyName)
                {
                    ParaSet.SetPara_Foundation_Rect(fmg, SetFamily.FRect);
                }
                if (FamilyName == SetFamily.FTRect.FamilyName)
                {
                    ParaSet.SetPara_Foundation_Tapered_Rect(fmg, SetFamily.FTRect);
                }
                if (FamilyName == SetFamily.FTri.FamilyName)
                {
                    ParaSet.SetPara_Foundation_Triangle(fmg, SetFamily.FTri);
                }
                if (FamilyName == SetFamily.FETriangle.FamilyName)
                {
                    ParaSet.SetPara_Foundation_ETriangle(fmg, SetFamily.FETriangle);
                }
                if(FamilyName == SetFamily.FOct.FamilyName)
                {
                    ParaSet.SetPara_Foundation_Octagon(fmg, SetFamily.FOct);
                }
                if (FamilyName == SetFamily.FConti.FamilyName)
                {
                    ParaSet.SetPara_Foundation_Continuous(fmg, SetFamily.FConti);
                }
                if (FamilyName == SetFamily.CastinPile.FamilyName)
                {
                    ParaSet.SetPara_Castinpile(fmg, SetFamily.CastinPile);
                }
                if (FamilyName == SetFamily.PrecastPile.FamilyName)
                {
                    ParaSet.SetPara_Precastpile(fmg, SetFamily.PrecastPile);
                }

                // Reload family with added parameters into the project
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, FamilyName + " failed to add parameters.");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        #endregion

        private void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            fromSTB.gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            fromSTB.GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            fromSTB.GaugeShow();
            pform.Refresh();
        }

     

        private void chb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox sender_ch = (CheckBox)sender;

            for(int i =0;i< ch.Count(); i++)
            {
                for(int j =0; j < ch[i].Count();j++)
                {
                    if(sender_ch.Text == ch[i][j].Text)
                    { ch[i][j].Checked = sender_ch.Checked; }
                }
            }
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChClm_name, ref ChClm);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChGir_name, ref ChGir);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChBeam_name, ref ChBeam);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChCGir_name, ref ChCGir);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChCBeam_name, ref ChCBeam);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChSBra_name, ref ChSBra);
            Other_list_Chb_Changed(sender_ch.Checked, sender_ch.Text, ChFound_name, ref ChFound);
        }

        private void Other_list_Chb_Changed(bool flg, string str, string[][] Ch_name, ref bool[][] Ch_Checked)
        {
            for(int i =0; i < Ch_name.Count(); i++)
            {
                for(int j = 0; j < Ch_name[i].Count(); j++)
                {
                    if(Ch_name[i][j] == str)
                    {
                        Ch_Checked[i][j] = flg;
                    }
                }
            }
        }
    }
}
