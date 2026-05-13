using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace STBLink
{
    public partial class ConvertForm : Form
    {
        public ConvertForm()
        {
            InitializeComponent();

            first = true;
        }

        // Loaded STB data
        private static int stb_ver = 1;
        internal static STBclass stb = null;
        internal static ST_BRIDGE_V2.ST_BRIDGE stb2 = null;

        public bool CheckDebugLogOutput => this.CheckBoxDebug.Checked ;

        // Control layout positions
        int labx = 15;
        int laby = 23;  // Label Y baseline
        int labyiv = 55; // Label vertical spacing
        int len = 10;
        int p5 = 5;
        int komefirst = 0;
        // Control sizes
        int labsizex = 150; int labsizey = 20;
        int txbsizex = 300; int txbsizey = 20;
        int btsizex = 45; int btsizey = 19;
        int grpbsizex1 = 255;

        bool[] radChecked = new bool[0];

        // Conversion order
        internal static string[] Conv_Order = { "RC Slab",
                                                "Deck Plate",
                                                "Precast Slab",
                                                "Foundation Slab",
                                                "Wall",
                                                "RC Parapet",                                                
                                                "Column",
                                                "Post",
                                                "Foundation Column",
                                                "Girder",
                                                "Beam",
                                                "Cantilever Girder",
                                                "Cantilever Beam",
                                                "S Brace",
                                                "Foundation, Strip Foundation, Pile",
                                                "Column Base"};

        // ************ Initialization required below ************
        internal class FormContr
        {
            internal TextBox txb;
            internal Label lab;
            internal Label errlab;                
            internal Button bt;            
            internal FormContr()
            {
                txb = new TextBox();
                lab = new Label();
                errlab = new Label();
                bt = new Button();
            }
        }
        /// <summary> Controls placed on groupBox1
        /// </summary>
        FormContr[] FCont = new FormContr[38];
        GroupBox[] group = new GroupBox[4];

        /// <summary> Stores loaded state; used for not-loaded checks
        /// </summary>
        internal class Loadflg_Class
        {
            /// <summary> Member / list entry name
            /// </summary>
            internal string listname;
            /// <summary> Family names not yet loaded
            /// </summary>
            internal List<string> faminame ;
            /// <summary> false when selected for conversion but family not loaded
            /// </summary>
            internal List<bool> flg  ;
            internal Loadflg_Class()
            {
                listname = "";
                faminame = new List<string>();
                flg = new List<bool>();
            }
        }
        // Conversion checkbox state
        internal class Chb_class
        {
            internal string buzai;
            internal bool chbchecked;
            internal Chb_class()
            {
                buzai = "";
                chbchecked = false;
            }
        }

        internal static List<Chb_class> Chb_Checked = new List<Chb_class>();
        internal List<Loadflg_Class> Lof = new List<Loadflg_Class>();

        // Whether STB contains data for each member type
        internal class STBLoadflg
        {
            internal string name;
            internal bool flg;
            internal string kind;
        }
        internal class BClmData
        {
            internal string product_code = "";
            internal string company = "";
        }
        /// <summary> Column Base rows present in the STB file
        /// </summary>
        private List<BClmData> BClm = new List<BClmData>();
        private List<STBLoadflg> STBload = new List<STBLoadflg>();
        public class LevelMappingData
        {
            internal bool flg = false;
            internal string[] RevitLevel;
            internal string[] RevitOffset;
            internal int rdb = 0;
            internal string STB_X = "";
            internal string STB_Y = "";
            internal string RVT_X = "";
            internal string RVT_Y = "";
            internal double Offset_X1 = 0;
            internal double Offset_Y1 = 0;
            internal double Offset_X2 = 0;
            internal double Offset_Y2 = 0;
        }
        /// <summary> Persisted settings from level mapping dialog
        /// </summary>
        public static LevelMappingData LMD = new LevelMappingData();

        //public static List<string> Concname = new List<string>(); //[0]=RC/SRC [1]=CFT
        ///// <summary>Steel data to append to project information
        ///// </summary>
        //public class Tekkotu
        //{
        //    internal string STB = "";
        //    internal string RVT = "";
        //}
        //public static List<Tekkotu> TekkotuPare = new List<Tekkotu>();

        //*************************************************************************************


        private bool first = true;

        private void Convert_Load(object sender, EventArgs e)
        {
            if (!first) return;


            // Form title
            // this.Text = RevitLNK.formtitle + " Conversion Confirmation " + Commons.GetVersion();
            this.Text = RevitLNK.formtitle + " Conversion Confirmation ";

            //// Material and concrete init
            //RevitLNK.MateData = new List<RevitLNK.Materialdata>();
            //RevitLNK.ConcData = new List<RevitLNK.Concredata>();

            //// Reset state
            //BClm = new List<BClmData>();
            //STBload = new List<STBLoadflg>();
            //LMD = new LevelMappingData();
            //Concname = new List<string>();
            //TekkotuPare = new List<Tekkotu>();
            //Lof = new List<Loadflg_Class>();


            for (int i = 0; i < FCont.Length; i++)
            {
                FCont[i] = new FormContr();                
            }
            FCont[0].txb = txb1;
            FCont[1].txb = txb2;
            FCont[2].txb = txb3;
            FCont[3].txb = txb4;
            FCont[4].txb = txb5;
            FCont[5].txb = txb6;
            FCont[6].txb = txb7;
            FCont[7].txb = txb8;
            FCont[8].txb = txb9;
            FCont[9].txb = txb10;
            FCont[10].txb = txb11;
            FCont[11].txb = txb12;
            FCont[12].txb = txb13;
            FCont[13].txb = txb14;
            FCont[14].txb = txb15;

            FCont[0].lab = lab1;
            FCont[1].lab = lab2;
            FCont[2].lab = lab3;
            FCont[3].lab = lab4;
            FCont[4].lab = lab5;
            FCont[5].lab = lab6;
            FCont[6].lab = lab7;
            FCont[7].lab = lab8;
            FCont[8].lab = lab9;
            FCont[9].lab = lab10;
            FCont[10].lab = lab11;
            FCont[11].lab = lab12;
            FCont[12].lab = lab13;
            FCont[13].lab = lab14;
            FCont[14].lab = lab15;

            FCont[0].bt = bt1;
            FCont[1].bt = bt2;
            FCont[2].bt = bt3;
            FCont[3].bt = bt4;
            FCont[4].bt = bt5;
            FCont[5].bt = bt6;
            FCont[6].bt = bt7;
            FCont[7].bt = bt8;
            FCont[8].bt = bt9;
            FCont[9].bt = bt10;
            FCont[10].bt = bt11;
            FCont[11].bt = bt12;
            FCont[12].bt = bt13;
            FCont[13].bt = bt14;
            FCont[14].bt = bt15;

            // 2017/11/07 Per-family-detail mode: controls added for expanded families
            for (int i = 15; i<FCont.Length;i++)
            {
                TextBox txb = new TextBox
                {
                    Name = "txb" + (i + 1).ToString()
                };
                FCont[i].txb = txb;
                Label lab = new Label
                {
                    AutoSize = true,
                    Name = "lab" + (i + 1).ToString()
                };
                FCont[i].lab = lab;
                Button bt = new Button
                {
                    Name = "bt" + (i + 1).ToString(),
                    Text = "Load"
                };
                bt.Click += Bt1_1_Click;
                FCont[i].bt = bt;
            }

           

            group[0] = groupBox2;
            group[1] = groupBox3;
            group[2] = groupBox4;
            group[3] = groupBox5;

            checkBox1.Checked = true;
            checkBox2.Checked = false;

            //// Shape check — 2017/05/19 run check before showing initial UI
            //if (!CheckSTB(false))
            //{
            //    this.Close();
            //    return;
            //}

            // CheckBox initialization
            for (int i = 0; i < Conv_Order.Count(); i++)
            {
                string[][] tx = new string[0][];
                string kind = "";
                string name = "";
                bool flg = false;
                Chb_class chb = new Chb_class();
                switch(Conv_Order[i])
                {
                    case "RC Slab":
                    case "Deck Plate":
                    case "Precast Slab":
                        name = "Slab, Deck Plate";                       
                        break;
                    case "Foundation Slab":
                        name = "Foundation Slab";
                        break;
                    case "Wall":
                    case "RC Parapet":
                        name = "Wall, RC Parapet";
                        break;
                    case "Girder":
                        tx = RevitLNK.GirText; ;
                        kind = "GIRDER";
                        break;
                    case "Beam":
                        tx = RevitLNK.BeamText;
                        kind = "BEAM";
                        break;
                    case "Cantilever Girder":
                        tx = RevitLNK.CGirText;
                        kind = "GIRDER";
                        break;
                    case "Cantilever Beam":
                        tx = RevitLNK.CBeamText;
                        kind = "BEAM";
                        break;
                    case "Column":
                        tx = RevitLNK.ClmText;
                        kind = "COLUMN";
                        break;
                    case "Post":
                        tx = RevitLNK.ClmText;
                        kind = "POST";
                        break;
                    case "Foundation Column":
                        tx = RevitLNK.FClmText;
                        break;
                    case "S Brace":
                        tx = RevitLNK.SBraText;
                        break;
                    case "Foundation, Strip Foundation, Pile":
                        tx = RevitLNK.BaseText;
                        break;
                    case "Column Base":
                        name = "Column Base";
                        break;

                }
                if (tx.Count() == 0)
                { flg = Check_STBLoadflg(name, Conv_Order[i]); }
                else
                { flg = Check_STBLoadflg(tx, kind); }
                Chb_class chb1 = new Chb_class
                {
                    buzai = Conv_Order[i],
                    chbchecked = flg
                };
                Chbclass_Add(chb1);
            }
            AllControlInit();

            // Configure DataGridView
            DGV2_set();

            // Walk all list items and set controls; append <Not loaded> when applicable
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                bool listchangeflg = false;
                if (i == 8 || i == 9 || i == 11) { continue; } // 2016/11/07 Family detail mode: indices changed
                else if(i == 12)
                {
                    for(int j = 0; j < DGV2.Rows.Count; j++)
                    {
                        if(DGV2.Rows[j].Cells[4].Value.ToString() == "Load" || DGV2.Rows[j].Cells[4].Value.ToString() == "Cannot load")
                        {
                            listchangeflg = true;
                            Loadflg_Class newlo = new Loadflg_Class
                            {
                                listname = "Column Base"
                            };
                            if (DGV2.Rows[j].Cells[2].Value != null)
                            { newlo.faminame.Add(System.IO.Path.GetFileNameWithoutExtension(DGV2.Rows[j].Cells[2].Value.ToString())); }
                            else { newlo.faminame.Add(""); }
                            newlo.flg.Add( false);
                            Lof_Add(newlo);                           
                        }
                    }
                }
                else
                {
                    listBox1.SelectedIndex = i;
                    for (int j = 0; j < FCont.Count(); j++)
                    {
                        if (FCont[j].errlab.Visible)
                        {
                            listchangeflg = true;
                            break;
                        }
                    }
                }

                if (listchangeflg)
                {
                    listBox1.Items[i] = STBParaBuild.listboxtext(listBox1.Items[i].ToString());
                }
            }
            listBox1.SelectedIndex = 0;
           

            // groupBox1 size
            groupBox1.Width = groupBox4.Right + len;
            groupBox1.Height = groupBox3.Bottom + len;           

            // Form client size
            int x = len + listBox1.Width + len + groupBox1.Width + len;
            int y = len + checkBox1.Height + len + groupBox1.Height + len + btOK.Height + len ;
            this.ClientSize = new System.Drawing.Size(x, y);

            // Lay out controls
            listBox1.Left = len;
            listBox1.Top = len;
            checkBox1.Left = listBox1.Right + len;
            checkBox1.Top = len;
            checkBox2.Left = checkBox1.Right + len;
            checkBox2.Top = len;
            checkBox3.Left = checkBox2.Right + len;
            checkBox3.Top = len;
            groupBox1.Left = checkBox1.Left;
            groupBox1.Top = checkBox1.Bottom + len;
            btOK.Left = this.ClientSize.Width - len - Cancel.Width - len / 2 - btOK.Width;
            btOK.Top = groupBox1.Bottom + len;
            Cancel.Left = btOK.Right + len / 2;
            Cancel.Top = btOK.Top;
            linkLabel1.Left = len;
            linkLabel1.Top = this.ClientSize.Height - len - linkLabel1.Height;
            linkLabel2.Left = linkLabel1.Right + len / 2;
            linkLabel2.Top = linkLabel1.Top;
            DGV2.Left = len;
            DGV2.Top = len * 2;
            DGV2.Height = groupBox1.ClientSize.Height - len * 3 - (label1.Height + label2.Height + label3.Height );
            button1.Top = DGV2.Bottom + 1;
            button1.Left = DGV2.Right - button1.Width;
            button1.BringToFront();
            label1.Left = DGV2.Left;
            komefirst = DGV2.Bottom + 1;
            label1.Top = komefirst; 
            label2.Left = DGV2.Left;
            label2.Top = label1.Bottom - 2;
            label3.Left = DGV2.Left;
            label3.Top = label2.Bottom - 2;
            label3.Text = "*3: The specified type was not found in the Column Base family.\r\n    Check the Column Base family or the Column Base mapping table.";
            


#if DEBUG
            button3.Visible = true;
            button3.Enabled = true;
#else
            button3.Visible = false;
            button3.Enabled = false;
#endif

            //this.Show();
            first = false;
        }

        //****** Events ******
        /// <summary> List selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox1_SelectedIndextxbanged(object sender, EventArgs e)
        {
            Selectlistbox(listBox1.SelectedIndex);
        }

        /// <summary> Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary> OK — start conversion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtOK_Click(object sender, EventArgs e)
        {          
            bool convflg = false;
           
            for(int i = 0; i < Chb_Checked.Count(); i++)
            {
                if(Chb_Checked[i].buzai == "") { continue; }
               
                if (Chb_Checked[i].chbchecked)
                {
                    for (int j = 0; j < Lof.Count(); j++)
                    {
                        if(Lof[j].listname != Chb_Checked[i].buzai) { continue; }
                        for (int k = 0; k < Lof[j].flg.Count(); k++)
                        {
                            if (!Lof[j].flg[k])
                            {
                                string message = "Some members still have unloaded families.";
                                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    convflg = true;                    
                }
            }
            if (convflg)
            {
                Commons.GridMode = radioButton3.Checked ? 0 : 1;
                this.DialogResult = DialogResult.Yes;

                //this.DialogResult = DialogResult.OK;
               
                //LevelMapping lev = new LevelMapping();
                //if (lev.ShowDialog() == DialogResult.OK)
                //{
                //    MaterialMapping map = new MaterialMapping();
                //    if (map.ShowDialog() == DialogResult.OK)
                //    { this.Dispose(); }
                //    else
                //    { this.DialogResult = DialogResult.None; }
                //}
                //else
                //{
                //    this.DialogResult = DialogResult.None;
                //}
            }
            else
            {
                string message = "No members are selected for conversion.";
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
                MessageBox.Show("Could not find the help file.\r\n\r\n" + helppath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
            f.Dispose();
        }
        private void Help_Requested(object sender, HelpEventArgs hlpevent)
        {
            LinkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }
        private void ConvertForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Escape)
            { this.Close(); }
        }

        /// <summary> Per-row Load buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bt1_1_Click(object sender, EventArgs e)
        {
            string familyfile = "";
            string faminame = "";
            string rfaname = "";
            string buzaimei = "";
            Button bt = (Button)sender;
            for (int i = 0; i < FCont.Count(); i++)
            {
                if (FCont[i].bt.Name == bt.Name)
                {
                    rfaname = FCont[i].txb.Text + ".rfa";
                    faminame = FCont[i].txb.Text;
                    buzaimei = FCont[i].lab.Text;
                    break;
                }
            }
            OpenFileDialog opf = new OpenFileDialog();
            string title = STBParaBuild.listboxtext_del(listBox1.SelectedItem.ToString());
            opf.Title = RevitLNK.formtitle + " — Select " + buzaimei + " family";
            opf.Filter = rfaname + "|"+ rfaname + "|" + "Revit family files|*.rfa|All files|*.*";
            opf.FileName = rfaname;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                familyfile = opf.FileName;
                if(faminame != System.IO.Path.GetFileNameWithoutExtension(familyfile))
                {
                    MessageBox.Show(buzaimei + ": Please select " + faminame + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if(!ReloadFamily(familyfile, ref faminame))                
                {
                    string catename = "";
                    switch(listBox1.SelectedIndex)
                    {
                        case 0:
                        case 1:
                        case 2:
                            catename = "Structural Columns";
                            break;                        
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            catename = "Structural Framing";
                            break;
                        case 10:
                            catename = "Structural Foundations";
                            break;
                    }
                    string mes = "Verify the family category is [" + catename + "].";
                    MessageBox.Show(buzaimei + ": Failed to load " + faminame + ".\r\n" + mes,
                                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        /// <summary> Column Base Load cell in DGV2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DGV2_btLoad_Click(e.ColumnIndex, e.RowIndex, dgv);
        }
        /// <summary> Column Base "Load all" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            bool flg = true;
            this.Enabled = false;
            this.Tag = "BaseAllLoad";
            for (int i = 0; i < DGV2.Rows.Count; i++)
            {
                if(!DGV2_btLoad_Click(4, i, DGV2, true))
                { flg = false; }
            }

            if(!flg) // Failed to load one or more Column Base families
            {
                string mes = "One or more Column Base families failed to load.";
                MessageBox.Show(mes + "\r\nCheck the Notes column and the captions below.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                button1.Enabled = false;
            }
            this.Enabled = true;
            Application.DoEvents();
            this.Tag = "";
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Chb_class chb = new Chb_class();
            switch(listBox1.SelectedIndex)
            {
                case 8:
                    chb.buzai = "RC Slab";
                    break;
                case 9:
                    chb.buzai = "Wall";
                    break;
                default:
                    chb.buzai = listBox1.Text;
                    if(!checkBox1.Checked)
                    {
                        groupBox1.Enabled = false;
                    }
                    else
                    {
                        chb.chbchecked = checkBox1.Checked;
                        Chbclass_Add(chb);
                        groupBox1.Enabled = true;
                        //selectlistbox(listBox1.SelectedIndex);
                    }  
                    break;
            }
           
            chb.chbchecked = checkBox1.Checked;
            Chbclass_Add(chb);


        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Chb_class chb = new Chb_class();
            switch (listBox1.SelectedIndex)
            {
                case 8:
                    chb.buzai = "Deck Plate";
                    break;
                case 9:
                    chb.buzai = "RC Parapet";
                    break;
                default:
                    chb.buzai = listBox1.Text;
                    break;
            }
            chb.chbchecked = checkBox2.Checked;
            Chbclass_Add(chb);
        }
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            Chb_class chb = new Chb_class
            {
                buzai = "Precast Slab",
                chbchecked = checkBox3.Checked
            };
            Chbclass_Add(chb);
        }


        //****** Core logic ******
        private void Lof_Add(Loadflg_Class newlo)
        {
            bool addflg = false;

            for (int i = 0; i < Lof.Count(); i++)
            {
                if (newlo.listname == Lof[i].listname)
                {
                    addflg = true;

                    for (int k = 0; k < newlo.faminame.Count(); k++)
                    {
                        bool nameflg = false;
                        for (int j = 0; j < Lof[i].faminame.Count(); j++)
                        {
                            if (newlo.faminame[k] == Lof[i].faminame[j])
                            {
                                if (newlo.flg[k] != Lof[i].flg[j])
                                { Lof[i].flg[j] = newlo.flg[k]; }
                                nameflg = true;
                            }
                        }
                        if(!nameflg)
                        {
                            Lof[i].faminame.Add(newlo.faminame[k]);
                            Lof[i].flg.Add(newlo.flg[k]);
                        }
                    }
                }
            }
            if (!addflg)
            {
                Lof.Add(newlo);
            }
        }
        
        /// <summary> Reset controls to initial state
        /// </summary>
        private void AllControlInit()
        {
            for (int i = 0; i < FCont.Count(); i++)
            {
                FCont[i].txb.Visible = false;
                FCont[i].txb.SetBounds(0, 0, txbsizex, txbsizey);
                FCont[i].lab.Visible = false;
                FCont[i].lab.TextAlign = ContentAlignment.BottomLeft;
                FCont[i].lab.SetBounds(labx, laby + labyiv * i, labsizex, labsizey);
                FCont[i].bt.Visible = false;
                FCont[i].bt.SetBounds(0, 0, btsizex, btsizey);
                FCont[i].errlab.AutoSize = true;
                FCont[i].errlab.Visible = false;
                FCont[i].errlab.TextAlign = ContentAlignment.BottomLeft;
                FCont[i].errlab.Text = "Cannot convert: family not loaded";　
                FCont[i].errlab.Left = FCont[i].bt.Left - len - FCont[i].errlab.Width;
                FCont[i].errlab.Top = FCont[i].lab.Top;               
                
            }
            
            

            for (int i = 0; i < group.Count(); i++)
            {
                group[i].Visible = false;
                group[i].Width = grpbsizex1;
                group[i].Top = 20;
                group[i].Left = 10;
            }
            groupBox1.Visible = false;
            checkBox1.Text = "Generate members";
            checkBox2.Visible = false;
            checkBox3.Visible = false;
            //Column Base grid
            DGV2.Visible = false;
            button1.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            // Column Base footnote labels
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
        }
        /// <summary> Handle list selection and family load UI updates
        /// </summary>
        /// <param name="flg"> false when returning from load and family names are already set </param>
        private void Selectlistbox(int ind, bool flg = true)
        {
            if(ind == -1) { return; }
            // 0 Column, 1 Post, 2 Foundation Column, 3 Girder, 4 Beam, 5 Cantilever Girder, 6 Cantilever Beam, 7 S Brace
            // 8 Slab & deck slab, 9 Wall & parapet, 10 Foundation, 11 Foundation Slab, 12 Column Base
            
            // Prior checkbox checked state (on first load, ON if convertible data exists)
            bool chboxflg = false;
            // True if any member is selected for conversion
            bool chbenabled = false;
            int yn = 0;
            string kind = "";
            Chb_class newch = new Chb_class();
            Chb_class newch2 = new Chb_class();
            string listname = STBParaBuild.listboxtext_del(listBox1.Items[ind].ToString());


            /// Families not yet loaded
            Loadflg_Class newlo = new Loadflg_Class
            {
                listname = listname
            };

            switch (ind)
            {
                case 0:
                    if (!flg)
                    {
                        SetFamily.SetClmFamilyName();
                    }

                    AllControlInit();

                    kind = "";
                    if (listBox1.SelectedIndex == 0)
                    { kind = "COLUMN"; }
                    else
                    { kind = "POST"; }

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }

                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";
                    groupBox5.Text = "CFT";

                    ControlsSet(RevitLNK.ClmText);
                    for (int i = 0; i < RevitLNK.ClmText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.ClmText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.ClmFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.ClmText[i][j], kind);
                            SetLabel(lf, ref SetFamily.ClmFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.ClmFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.ClmFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.ClmFName.flg[i][j]);
                                }
                                SetFamily.ClmFName.convflg[i][j] = true;
                            }
                            else { SetFamily.ClmFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);
                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 1:
                    if (!flg)
                    {
                        SetFamily.SetClmFamilyName();
                    }

                    AllControlInit();

                    kind = "";
                    if (listBox1.SelectedIndex == 0)
                    { kind = "COLUMN"; }
                    else
                    { kind = "POST"; }

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }

                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";
                    groupBox5.Text = "CFT";

                    ControlsSet(RevitLNK.ClmText);
                    for (int i = 0; i < RevitLNK.ClmText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.ClmText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.PClmFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.ClmText[i][j], kind);
                            SetLabel(lf, ref SetFamily.PClmFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.PClmFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.PClmFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.PClmFName.flg[i][j]);
                                }
                                SetFamily.PClmFName.convflg[i][j] = true;
                            }
                            else { SetFamily.PClmFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);

                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 2:
                    if (!flg)
                    {
                        SetFamily.SetBClmFamilyName();
                    }

                    AllControlInit();

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;



                    groupBox2.Text = "RC";
                    ControlsSet(RevitLNK.FClmText);

                    for (int i = 0; i < RevitLNK.FClmText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.FClmText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.BClmFName.FamilyName[i][j];
                            // Uses same family as RC column; Disabled when STB has no data
                            bool lf = Check_STBLoadflg(RevitLNK.FClmText[i][j], kind);
                            SetLabel(lf, ref SetFamily.BClmFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.BClmFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.BClmFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.BClmFName.flg[i][j]);
                                }
                                SetFamily.BClmFName.convflg[i][j] = true;
                            }
                            else { SetFamily.BClmFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);

                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    //this.Refresh();
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 3:
                    if (!flg)
                    {
                        SetFamily.SetGirFamilyName();
                    }

                    AllControlInit();

                    kind = "GIRDER";

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;


                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";

                    ControlsSet(RevitLNK.GirText);

                    for (int i = 0; i < RevitLNK.GirText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.GirText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.GirFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.GirText[i][j], kind);
                            SetLabel(lf, ref SetFamily.GirFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.GirFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.GirFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.GirFName.flg[i][j]);
                                }
                                SetFamily.GirFName.convflg[i][j] = true;
                            }
                            else { SetFamily.GirFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);
                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 4:
                    if (!flg)
                    {
                        SetFamily.SetBeamFamilyName();
                    }

                    AllControlInit();

                    kind = "BEAM";

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";

                    ControlsSet(RevitLNK.BeamText);
                    for (int i = 0; i < RevitLNK.BeamText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.BeamText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.BeamFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.BeamText[i][j], kind);
                            SetLabel(lf, ref SetFamily.BeamFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.BeamFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.BeamFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.BeamFName.flg[i][j]);
                                }
                                SetFamily.BeamFName.convflg[i][j] = true;
                            }
                            else { SetFamily.BeamFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);
                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 5:
                    if (!flg)
                    {
                        SetFamily.SetCGirFamilyName();
                    }

                    AllControlInit();

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";

                    kind = "GIRDER";

                    ControlsSet(RevitLNK.CGirText);
                    for (int i = 0; i < RevitLNK.CGirText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.CGirText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.CGirFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.CGirText[i][j], kind);
                            SetLabel(lf, ref SetFamily.CGirFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.CGirFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.CGirFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.CGirFName.flg[i][j]);
                                }
                                SetFamily.CGirFName.convflg[i][j] = true;
                            }
                            else { SetFamily.CGirFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);

                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 6:
                    if (!flg)
                    {
                        SetFamily.SetCBeamFamilyName();
                    }

                    AllControlInit();

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "RC";
                    groupBox3.Text = "S";
                    groupBox4.Text = "SRC";

                    kind = "BEAM";

                    ControlsSet(RevitLNK.CBeamText);
                    for (int i = 0; i < RevitLNK.CBeamText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.CBeamText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.CBeamFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.CBeamText[i][j], kind);
                            SetLabel(lf, ref SetFamily.CBeamFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.CBeamFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.CBeamFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.CBeamFName.flg[i][j]);
                                }
                                SetFamily.CBeamFName.convflg[i][j] = true;
                            }
                            else { SetFamily.CBeamFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);

                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 7:
                    if (!flg)
                    {
                        SetFamily.SetBraFamilyName();
                    }

                    AllControlInit();

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;

                    groupBox2.Text = "S";

                    ControlsSet(RevitLNK.SBraText1);
                    for (int i = 0; i < RevitLNK.SBraText.Length; i++)
                    {
                        for (int j = j = 0; j < RevitLNK.SBraText[i].Length; j++)
                        {
                            FCont[yn].txb.Text = SetFamily.SBraFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.SBraText[i][j], "");
                            SetLabel(lf, ref SetFamily.SBraFName.flg[i][j], yn);
                            if (lf)
                            {
                                chbenabled = true;
                                if (!SetFamily.SBraFName.flg[i][j])
                                {
                                    newlo.faminame.Add(SetFamily.SBraFName.FamilyName[i][j]);
                                    newlo.flg.Add(SetFamily.SBraFName.flg[i][j]);
                                }
                                SetFamily.SBraFName.convflg[i][j] = true;
                            }
                            else { SetFamily.SBraFName.flg[i][j] = lf; }
                            yn++;
                        }
                    }
                    Lof_Add(newlo);

                    checkBox1.Enabled = chbenabled;
                    groupBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for(int j = 0; j < group[i].Controls.Count;j++)
                        {
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 8:
                    AllControlInit();
                    checkBox1.Text = "Generate RC slab";
                    checkBox2.Visible = true;
                    checkBox2.Text = "Generate deck slab";
                    checkBox2.Left = checkBox1.Right + len;
                    checkBox3.Visible = true;
                    checkBox3.Text = "Generate precast slab";
                    checkBox3.Left = checkBox2.Right + len;
                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == "RC Slab")
                        {
                            checkBox1.Checked = Chb_Checked[i].chbchecked;
                        }
                        if (Chb_Checked[i].buzai == "Deck Plate")
                        {
                            checkBox2.Checked = Chb_Checked[i].chbchecked;
                        }
                        if (Chb_Checked[i].buzai == "Precast Slab")
                        {
                            checkBox3.Checked = Chb_Checked[i].chbchecked;
                        }
                    }

                    checkBox1.Enabled = Check_STBLoadflg("Slab, Deck Plate", "RC Slab");
                    checkBox2.Enabled = Check_STBLoadflg("Slab, Deck Plate", "Deck Plate");
                    checkBox3.Enabled = Check_STBLoadflg("Slab, Deck Plate", "Precast Slab");
                    break;
                case 9:
                    AllControlInit();
                    checkBox1.Text = "Generate wall";
                    checkBox2.Visible = true;
                    checkBox2.Text = "Generate RC Parapet";
                    checkBox2.Left = checkBox1.Right + len;
                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == "Wall")
                        {
                            checkBox1.Checked = Chb_Checked[i].chbchecked;
                            checkBox1.Enabled = Chb_Checked[i].chbchecked;
                        }
                        if (Chb_Checked[i].buzai == "RC Parapet")
                        {
                            checkBox2.Checked = Chb_Checked[i].chbchecked;
                            checkBox2.Enabled = Chb_Checked[i].chbchecked;
                        }
                    }

                    checkBox1.Enabled = Check_STBLoadflg("Wall, RC Parapet", "Wall");
                    checkBox2.Enabled = Check_STBLoadflg("Wall, RC Parapet", "RC Parapet");
                    break;
                case 10:
                    if (!flg)
                    {
                        SetFamily.SetFoundationFamilyName();
                    }

                    AllControlInit();

                    // Configure member conversion checkboxes                   
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == listname)
                        {
                            chboxflg = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Checked = chboxflg;
                    checkBox1.Enabled = chboxflg;

                    groupBox2.Text = "Foundation";
                    groupBox3.Text = "Strip foundation";
                    groupBox4.Text = "Pile";
                    bool pileflg = false;
                    ControlsSet(RevitLNK.BaseText);
                    for (int i = 0; i < RevitLNK.BaseText.Length; i++)
                    {
                        for (int j = 0; j < RevitLNK.BaseText[i].Length; j++)
                        {
                            if (RevitLNK.BaseText[i][j] == "") continue;

                            FCont[yn].txb.Text = SetFamily.FoFName.FamilyName[i][j];
                            bool lf = Check_STBLoadflg(RevitLNK.BaseText[i][j], "");

                            SetLabel(lf, ref SetFamily.FoFName.flg[i][j], yn);


                            if (lf)
                            {
                                if (i < 2 || stb_ver == 2)
                                {
                                    groupBox1.Visible = true;
                                    chbenabled = true;
                                    if (!SetFamily.FoFName.flg[i][j])
                                    {
                                        newlo.faminame.Add(SetFamily.FoFName.FamilyName[i][j]);
                                        newlo.flg.Add(SetFamily.FoFName.flg[i][j]);
                                    }
                                    SetFamily.FoFName.convflg[i][j] = true;
                                }
                                else
                                { pileflg = lf; }
                            }
                            else
                            {
                                FCont[yn].txb.Text = "";

                                if (i < 2 && stb_ver == 1)
                                {
                                    // STB 1.4: user picks pile family on this dialog
                                    SetFamily.FoFName.flg[i][j] = lf;
                                }
                            }

                            yn++;
                        }
                    }

                    // Pile options
                    if (stb_ver == 1)
                    {
                        radioButton1.Left = FCont[yn - 2].lab.Left;
                        radioButton1.Top = FCont[yn - 2].lab.Top - 3;
                        FCont[yn - 2].lab.Visible = false;
                        radioButton1.Visible = true;
                        radioButton1.Text = "Cast-in-place pile";

                        radioButton2.Left = FCont[yn - 1].lab.Left;
                        radioButton2.Top = FCont[yn - 1].lab.Top - 3;
                        FCont[yn - 1].lab.Visible = false;
                        radioButton2.Visible = true;
                        radioButton2.Text = "Precast pile";

                        if (pileflg)
                        {
                            FCont[yn - 2].txb.Text = SetFamily.FoFName.FamilyName[2][0];
                            FCont[yn - 1].txb.Text = SetFamily.FoFName.FamilyName[2][1];
                            // Selection state
                            if (radChecked.Count() == 0)
                            {
                                if (FCont[yn - 2].errlab.Visible) // not loaded
                                { radioButton1.Checked = false; }
                                else
                                { radioButton1.Checked = true; }
                                if (FCont[yn - 1].errlab.Visible)
                                { radioButton2.Checked = false; }
                                else
                                {
                                    if (!radioButton1.Checked)
                                    { radioButton2.Checked = true; }
                                }
                                Array.Resize(ref radChecked, 2);
                                radChecked[0] = radioButton1.Checked;
                                radChecked[1] = radioButton2.Checked;
                            }
                            else
                            {
                                radioButton1.Checked = radChecked[0];
                                radioButton2.Checked = radChecked[1];
                            }
                            // Disable when not applicable
                            if (FCont[yn - 2].errlab.Visible) // not loaded
                            { radioButton1.Enabled = false; }
                            else
                            { radioButton1.Enabled = true; }
                            if (FCont[yn - 1].errlab.Visible) // not loaded
                            { radioButton2.Enabled = false; }
                            else
                            { radioButton2.Enabled = true; }
                        }
                    }


                    Lof_Add(newlo);

                    // Grey out when chbenabled=false (no STB data) or chboxflg=false
                    groupBox1.Enabled = chbenabled;
                    checkBox1.Enabled = chbenabled;
                    for (int i = 0; i < group.Count(); i++)
                    {
                        group[i].Enabled = chbenabled;
                        for (int j = 0; j < group[i].Controls.Count; j++)
                        {                            
                            group[i].Controls[j].Enabled = chbenabled;
                        }
                    }
                    if (!pileflg)
                    {
                        radioButton1.Enabled = false;
                        radioButton1.Checked = false;
                        radioButton2.Enabled = false;
                        radioButton2.Checked = false;
                    }
                    if (!chboxflg)
                    {
                        groupBox1.Enabled = false;
                    }
                    break;
                case 11:
                    AllControlInit();
                    // Configure member conversion checkboxes
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == "Foundation Slab")
                        {
                            checkBox1.Checked = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Text = "Generate members";
                    chbenabled = Check_STBLoadflg("Foundation Slab", "Foundation Slab");
                    checkBox1.Enabled = chbenabled;
                    break;
                case 12:
                    AllControlInit();
                    groupBox1.Visible = true;

                    // Configure member conversion checkboxes          
                    for (int i = 0; i < Chb_Checked.Count(); i++)
                    {
                        if (Chb_Checked[i].buzai == "Column Base")
                        {
                            checkBox1.Checked = Chb_Checked[i].chbchecked;
                            checkBox1.Enabled = Chb_Checked[i].chbchecked;
                            break;
                        }
                    }
                    checkBox1.Text = "Generate members";
                    DGV2.Visible = true; // Show grid even when STB has no Column Base rows
                    bool btnflg = false;
                    if (DGV2.Rows.Count > 0)
                    {
                        checkBox1.Enabled = true;
                        groupBox1.Enabled = true;
                        DGV2.Enabled = true;

                        // Show footnote labels
                        bool lab1flg = false, lab2flg = false, lab3flg = false;
                        for (int i = 0; i < DGV2.RowCount; i++)
                        {
                            if (lab1flg && lab2flg && lab3flg && btnflg) { break; }
                            if (DGV2.Rows[i].Cells[5].Value.ToString() == "*1")
                            { lab1flg = true; }
                            if (DGV2.Rows[i].Cells[5].Value.ToString() == "*2")
                            { lab2flg = true; }
                            if (DGV2.Rows[i].Cells[5].Value.ToString() == "*3")
                            { lab3flg = true; }   
                            if(DGV2.Rows[i].Cells[4].Value.ToString() == "Load")
                            { btnflg = true; }                         
                        }
                        if(!lab1flg)
                        { label2.Top = komefirst; }
                        if (lab2flg)
                        { label3.Top = label2.Bottom - 2; }
                        else
                        {
                            if (lab1flg)
                            { label3.Top = label1.Bottom - 2; }
                            else
                            { label3.Top = komefirst; }
                        }
                        label1.Visible = lab1flg;
                        label2.Visible = lab2flg;
                        label3.Visible = lab3flg;
                       
                    }
                    else // No Column Base data in STB
                    {
                        checkBox1.Enabled = false;
                        groupBox1.Enabled = false;
                    }

                    if (checkBox1.Checked)
                    {
                        //DGV2.Enabled = true;
                        //button1.Enabled = btnflg;
                        //label1.Enabled = true;
                        //label2.Enabled = true;
                        //label3.Enabled = true;
                        groupBox1.Enabled = true;
                    }
                    else
                    {
                        //DGV2.Enabled = false;
                        //button1.Enabled = false;
                        //label1.Enabled = false;
                        //label2.Enabled = false;
                        //label3.Enabled = false;
                        groupBox1.Enabled = false;
                    }
                    button1.Visible = true;
                    break;

            }
        }
        /// <summary> Add and position member controls
        /// </summary>
        /// <param name="str"></param>
        private void ControlsSet(string[][] str)
        {
            groupBox1.Visible = true;
            int yn = 0;
            int skip_count = 0;

            // Add controls and sizing           
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < str[i].Length; j++)
                {
                    if (str[i][j] == "")
                    {
                        skip_count++;
                        continue;
                    }

                    switch (i)
                    {
                        case 0:
                            groupBox2.Controls.Add(FCont[yn].txb);
                            groupBox2.Controls.Add(FCont[yn].lab);
                            groupBox2.Controls.Add(FCont[yn].errlab);
                            groupBox2.Controls.Add(FCont[yn].bt);
                            break;
                        case 1:
                            groupBox3.Controls.Add(FCont[yn].txb);
                            groupBox3.Controls.Add(FCont[yn].lab);
                            groupBox3.Controls.Add(FCont[yn].errlab);
                            groupBox3.Controls.Add(FCont[yn].bt);
                            break;
                        case 2:
                            groupBox4.Controls.Add(FCont[yn].txb);
                            groupBox4.Controls.Add(FCont[yn].lab);
                            groupBox4.Controls.Add(FCont[yn].errlab);
                            groupBox4.Controls.Add(FCont[yn].bt);
                            break;
                        case 3:
                            groupBox5.Controls.Add(FCont[yn].txb);
                            groupBox5.Controls.Add(FCont[yn].lab);
                            groupBox5.Controls.Add(FCont[yn].errlab);
                            groupBox5.Controls.Add(FCont[yn].bt);
                            break;
                    }
                    int laby = 0;

                    if (j != 0)
                    {
                        laby = FCont[yn - 1].txb.Bottom + p5;
                        
                    }
                    else
                    {
                        laby = len + p5 ;                       
                    }
                    FCont[yn].txb.Visible = true;
                    FCont[yn].lab.Visible = true;
                    FCont[yn].lab.Text = str[i][j];
                    FCont[yn].lab.Left = len;
                    FCont[yn].lab.Top = laby;
                    FCont[yn].lab.Enabled = true;
                    FCont[yn].txb.Left = len;
                    FCont[yn].txb.Top = FCont[yn].lab.Bottom;
                    FCont[yn].txb.ReadOnly = true;
                    FCont[yn].txb.TabIndex = yn + 6;
                    FCont[yn].bt.Left = len + FCont[yn].txb.Width - FCont[yn].bt.Width;
                    FCont[yn].bt.Top = FCont[yn].txb.Top - FCont[yn].bt.Height;
                    FCont[yn].bt.TabIndex = yn + 5;
                    FCont[yn].errlab.BringToFront();
                    FCont[yn].errlab.Left = FCont[yn].bt.Left - len - 122;
                    FCont[yn].errlab.Top = FCont[yn].lab.Top;
                    
                    
                    
                    yn++;
                }
                int x = len + FCont[yn-1].txb.Width + len;
                int y = (str[i].Length - skip_count) * (FCont[yn - 1].lab.Height + FCont[yn - 1].txb.Height + p5) + len * 2;
                switch (i)
                {
                    case 0:
                        
                        groupBox1.Controls.Add(groupBox2);
                        groupBox2.Visible = true;
                        groupBox2.ClientSize = new Size(x, y);
                        groupBox2.Top = len * 2;
                        groupBox2.Left = len;
                        groupBox2.Refresh();
                        break;
                    case 1:
                       
                        groupBox1.Controls.Add(groupBox3);
                        groupBox3.Visible = true;
                        groupBox3.ClientSize = new Size(x, y);
                        groupBox3.Top = groupBox2.Bottom + len;
                        groupBox3.Left = len;
                        groupBox3.Refresh();
                        break;
                    case 2:
                        
                        groupBox1.Controls.Add(groupBox4);
                        groupBox4.Visible = true;
                        groupBox4.ClientSize = new Size(x, y);
                        groupBox4.Top = len * 2;
                        groupBox4.Left = groupBox2.Right + len;
                        groupBox4.Refresh();
                        break;
                    case 3:
                        
                        groupBox1.Controls.Add(groupBox5);
                        groupBox5.Visible = true;
                        groupBox5.ClientSize = new Size(x, y);
                        groupBox5.Top = groupBox4.Bottom + len;
                        groupBox5.Left = groupBox4.Left;
                        groupBox5.Refresh();
                        break;
                }
            }
           
        }
        /// <summary> Show or hide the not-loaded label
        /// </summary>       
        private void SetLabel(bool stbflg, ref bool fnameflg, int yn)
        {
            FCont[yn].errlab.ForeColor = Color.Red;

            if (stbflg)
            {
                if (!fnameflg)
                {
                    FCont[yn].errlab.Visible = true;
                    FCont[yn].bt.Visible = true;
                    FCont[yn].bt.Enabled = true;
                    FCont[yn].txb.Enabled = false;
                    FCont[yn].lab.Enabled = true;
                }
                else
                {
                    FCont[yn].txb.Enabled = true;
                    FCont[yn].lab.Enabled = true;
                }
            }
            else
            {
                FCont[yn].txb.Text = "";
                FCont[yn].txb.Enabled = false;
                FCont[yn].lab.Enabled = false;
            }
        }
        /// <summary> Load a not-yet-loaded family into the document
        /// </summary>
        /// <param name="FamilyFile"></param>
        /// <param name="familyName"></param>
        /// <returns></returns>
        internal bool ReloadFamily(string FamilyFile, ref string familyName, string typename = "")
        {
            bool retcode = false;
            string rfaname = System.IO.Path.GetFileNameWithoutExtension(FamilyFile);
            STBLink.FamilyOption famop = new FamilyOption();
            Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(Commons.doc, rfaname + " — load");            
            try
            {
                transaction.Start(rfaname + " — load");

                if (Commons.doc.LoadFamily(FamilyFile, famop, out Autodesk.Revit.DB.Family family))
                {
                    if (familyName != family.Name)
                    {
                        retcode = false;
                        transaction.RollBack();
                    }
                    else
                    {
                        if (typename != "") // Column Base with type filter: load only symbols matching the given type name
                        {
                            Autodesk.Revit.DB.FamilySymbol symbol = null;
                            if (!Data.SearchFamilySymbol(family, typename, ref symbol))
                            {
                                transaction.RollBack();
                                return false;
                            }
                        }
                        retcode = true;
                        LoadFamily.ProFami.Add(family);

                        RevitLNK.LoFa.LoadFfamily_fromProject();
                        // Refresh each member family name
                        SetFamily.SetClmFamilyName();
                        SetFamily.SetBClmFamilyName();
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                        SetFamily.SetBraFamilyName();
                        SetFamily.SetFoundationFamilyName();


                        for (int l = 0; l < listBox1.Items.Count; l++)
                        {
                            bool flg2 = false;
                            string listname = STBParaBuild.listboxtext_del(listBox1.Items[l].ToString());

                            for (int i = 0; i < Lof.Count(); i++)
                            {
                                if (Lof[i].listname == listname)
                                {
                                    for (int j = 0; j < Lof[i].faminame.Count(); j++)
                                    {
                                        if (Lof[i].faminame[j] == familyName)
                                        {
                                            Lof[i].flg[j] = true;
                                        }
                                        if (!Lof[i].flg[j])
                                        {
                                            flg2 = true;
                                        }
                                    }
                                }
                            }
                            if (flg2)
                            {
                                string newlistname = STBParaBuild.listboxtext(listname);
                                if (newlistname != listBox1.Items[l].ToString())
                                {
                                    listBox1.Items[l] = STBParaBuild.listboxtext(listname);
                                }

                            }
                            else
                            {
                                if (listBox1.Items[l].ToString() != listname)
                                { listBox1.Items[l] = listname; }
                            }
                        }
                        transaction.Commit();
                    }
                }
                else
                {
                    retcode = false;
                    transaction.RollBack();
                }

            }
            catch (Exception)
            {
                transaction.RollBack();
                retcode = false;
            }

            return retcode;
        }
        /// <summary> Whether each member type is selected for conversion
        /// </summary>
        /// <param name="chb"></param>
        private void Chbclass_Add(Chb_class chb)
        {
            bool sameflg = false;
            chb.buzai = STBParaBuild.listboxtext_del(chb.buzai);

            for (int i = 0; i < Chb_Checked.Count(); i++)
            {
                if (chb.buzai == Chb_Checked[i].buzai)
                {
                    sameflg = true;
                    if (chb.chbchecked != Chb_Checked[i].chbchecked )
                    {
                        Chb_Checked[i].chbchecked = chb.chbchecked;
                    }
                    break;
                }
            }

            if (!sameflg)
            { Chb_Checked.Add(chb); }
        }
        /// <summary> Build the Column Base grid
        /// </summary>
        private void DGV2_set()
        {
            int r = 0;
            bool addflg = false; // true = add rows to DataGridView
            string rfaname = "";

            for (int b = 0; b < BClm.Count(); b++)
            {
                addflg = false;
                bool conflg = false; // true = same family already in DGV2, skip duplicate

                for (int i = 0; i < RevitLNK.BClm.Count(); i++)
                {
                    if (conflg) { break; }
                    if (addflg) { break; }
                    if (!RevitLNK.BClm[i].flg) { continue; }
                    if(BClm[b].company != RevitLNK.BClm[i].product_company || BClm[b].product_code != RevitLNK.BClm[i].product_code) { continue; }

                    RevitLNK.BaseColumn bc = RevitLNK.BClm[i];

                    if (DGV2.Rows.Count > 1)
                    {
                        for (int j = 0; j < DGV2.Rows.Count; j++)
                        {
                            if (DGV2.Rows[j].Cells[0].Value.ToString() == BClm[b].company && DGV2.Rows[j].Cells[1].Value.ToString() == BClm[b].product_code)
                            {
                                conflg = true;
                                addflg = true;
                                break;
                            }
                        }
                    }
                    if (conflg) { break; }
                    if (System.IO.File.Exists(bc.pass + bc.rfa_pass))
                    {
                        bool btnflg = true; // show Load button in column

                        // Family name from the specified .rfa
                        rfaname = System.IO.Path.GetFileNameWithoutExtension(bc.rfa_pass);

                        bool typeflg = true; // show *3 in Notes column
                        for (int j = 0; j < LoadFamily.ProFami.Count(); j++)
                        {
                            if (!btnflg) { break; }
                            if (rfaname == LoadFamily.ProFami[j].Name)
                            {
                                if (bc.typename != "")
                                {
                                    ISet<Autodesk.Revit.DB.ElementId> slist = LoadFamily.ProFami[j].GetFamilySymbolIds();
                                    IList<Autodesk.Revit.DB.ElementId> elist = slist.ToList<Autodesk.Revit.DB.ElementId>();
                                    foreach (Autodesk.Revit.DB.ElementId e in elist)
                                    {
                                        Autodesk.Revit.DB.FamilySymbol s = (Autodesk.Revit.DB.FamilySymbol)Commons.doc.GetElement(e);
                                        if (s == null) { continue; }
                                        if (s.Name == bc.typename)
                                        {
                                            btnflg = false;
                                            typeflg = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    btnflg = false;
                                    typeflg = false;
                                }

                            }
                        }
                        try
                        {
                            DGV2.Rows.Add();
                            r = DGV2.Rows.Count - 1;
                            DGV2.Rows[r].ReadOnly = true;
                            DGV2.Rows[r].Cells[0].Value = bc.product_company;
                            DGV2.Rows[r].Cells[1].Value = bc.product_code;
                            DGV2.Rows[r].Cells[2].Value = bc.rfa_pass;
                            DGV2.Rows[r].Cells[3].Value = bc.typename;
                            if (btnflg) // mapping OK, family not loaded
                            {
                                DataGridViewButtonCell btn1 = new DataGridViewButtonCell();
                                DGV2.Rows[r].Cells[4].OwningColumn.Name = "load";
                                DGV2.Rows[r].Cells[4] = btn1;
                                DGV2.Rows[r].Cells[4].Value = "Load";
                                DGV2.Rows[r].Cells[2].Style.ForeColor = Color.Red;
                                DGV2.Rows[r].Cells[3].Style.ForeColor = Color.Red;
                                if (typeflg)
                                {
                                    DGV2.Rows[r].Cells[5].Value = "Not loaded";
                                }
                                else
                                {
                                    DGV2.Rows[r].Cells[5].Value = "*3";
                                }
                                DGV2.Rows[r].Cells[5].Style.ForeColor = Color.Red;
                            }
                            else // mapping OK, family already loaded
                            {
                                DataGridViewTextBoxCell tb = new DataGridViewTextBoxCell();
                                DGV2.Rows[r].Cells[4].OwningColumn.Name = "Text";
                                DGV2.Rows[r].Cells[4] = tb;
                                DGV2.Rows[r].Cells[4].Value = "Loaded";
                                DGV2.Rows[r].Cells[5].Value = "";
                                DGV2.Rows[r].Cells[4].ReadOnly = true;
                                DGV2.Rows[r].Cells[2].Style.ForeColor = Color.Black;
                                DGV2.Rows[r].Cells[3].Style.ForeColor = Color.Black;
                            }
                            addflg = true;
                        }


                        catch (Exception e)
                        {
                            e.ToString();
                        }
                    }
                    else
                    {
                        // Family file not found
                        DGV2.Rows.Add();
                        r = DGV2.Rows.Count - 1;
                        DGV2.Rows[r].ReadOnly = true;
                        DGV2.Rows[r].Cells[0].Value = bc.product_company;
                        DGV2.Rows[r].Cells[1].Value = bc.product_code;
                        DGV2.Rows[r].Cells[2].Value = bc.rfa_pass;
                        DGV2.Rows[r].Cells[2].Style.ForeColor = Color.Red;
                        DGV2.Rows[r].Cells[3].Value = bc.typename;
                        DGV2.Rows[r].Cells[3].Style.ForeColor = Color.Red;
                        DataGridViewTextBoxCell tb = new DataGridViewTextBoxCell();
                        DataGridViewButtonCell btn2 = new DataGridViewButtonCell();
                        DGV2.Rows[r].Cells[4].OwningColumn.Name = "load";
                        DGV2.Rows[r].Cells[4] = btn2;
                        DGV2.Rows[r].Cells[4].Value = "Load";
                        DGV2.Rows[r].Cells[5].Value = "*2";
                        DGV2.Rows[r].Cells[5].Style.ForeColor = Color.Red;
                        addflg = true;
                    }
                }
                if (addflg) { continue; }
                // Not specified in mapping
                DGV2.Rows.Add();
                r = DGV2.Rows.Count - 1;
                DGV2.Rows[r].ReadOnly = true;
                DGV2.Rows[r].Cells[0].Value = BClm[b].company;
                DGV2.Rows[r].Cells[0].Style.ForeColor = Color.Red;
                DGV2.Rows[r].Cells[1].Value = BClm[b].product_code;
                DGV2.Rows[r].Cells[1].Style.ForeColor = Color.Red;
                DGV2.Rows[r].Cells[2].Value = "";
                DGV2.Rows[r].Cells[3].Value = "";
                DataGridViewTextBoxCell tb1 = new DataGridViewTextBoxCell();
                DGV2.Rows[r].Cells[4].OwningColumn.Name = "Text";
                DGV2.Rows[r].Cells[4] = tb1;
                DGV2.Rows[r].Cells[4].Value = "Cannot load";
                DGV2.Rows[r].Cells[4].Style.ForeColor = Color.Red;
                DGV2.Rows[r].Cells[5].Value = "*1";
                DGV2.Rows[r].Cells[5].Style.ForeColor = Color.Red;
                r++;
            }
        }
        /// <summary> Handle Column Base Load / Load-all actions
        /// </summary>
        /// <param name="clmindex"></param>
        /// <param name="rowindex"></param>
        /// <param name="dgv"></param>
        private bool DGV2_btLoad_Click(int clmindex, int rowindex, DataGridView dgv, bool allflg = false)
        {
            bool ret = true;
            if(rowindex == -1 || clmindex == -1) { return ret; }
            // Load button column
            if (dgv.Rows[rowindex].Cells[clmindex].Value.ToString() == "Load")
            {
                string familyfile = "";
                string faminame = "";
                string typename = "";
                for (int i = 0; i < RevitLNK.BClm.Count(); i++)
                {
                    if(dgv.Rows[rowindex].Cells[2].Value == null) { continue; }
                    if (dgv.Rows[rowindex].Cells[2].Value.ToString() == RevitLNK.BClm[i].rfa_pass)
                    {
                        familyfile = RevitLNK.BClm[i].pass + RevitLNK.BClm[i].rfa_pass;
                        faminame = System.IO.Path.GetFileNameWithoutExtension(dgv.Rows[rowindex].Cells[2].Value.ToString());
                        typename = RevitLNK.BClm[i].typename;
                        break;
                    }
                }
                if (System.IO.File.Exists(familyfile))
                {
                    if (ReloadFamily(familyfile, ref faminame, typename))
                    {
                        DataGridViewTextBoxCell tb = new DataGridViewTextBoxCell();
                        dgv.Columns[clmindex].Name = "Text";
                        dgv.Rows[rowindex].Cells[4] = tb;
                        dgv.Rows[rowindex].Cells[4].Value = "Loaded";
                        dgv.Rows[rowindex].Cells[5].Value = "";
                        dgv.Rows[rowindex].Cells[2].Style.ForeColor = Color.Black;
                        dgv.Rows[rowindex].Cells[3].Style.ForeColor = Color.Black;

                        for (int i = 0; i < Lof.Count(); i++)
                        {
                            bool breakflg = false;
                            if (breakflg) { break; }
                            if (Lof[i].listname != "Column Base") { continue; }
                            for (int j = 0; j < Lof[i].faminame.Count(); j++)
                            {
                                if (Lof[i].faminame[j] == faminame)
                                {
                                    Lof[i].flg[j] = true;
                                    breakflg = true;
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        dgv.Rows[rowindex].Cells[2].Style.ForeColor = Color.Red;
                        dgv.Rows[rowindex].Cells[3].Style.ForeColor = Color.Red;
                        DataGridViewButtonCell btn1 = new DataGridViewButtonCell();
                        dgv.Rows[rowindex].Cells[4].OwningColumn.Name = "load";
                        dgv.Rows[rowindex].Cells[4] = btn1;
                        dgv.Rows[rowindex].Cells[4].Value = "Load";
                        dgv.Rows[rowindex].Cells[5].Value = "*3";
                        dgv.Rows[rowindex].Cells[5].Style.ForeColor = Color.Red;
                        if (!allflg)
                        {
                            string messtext = "Failed to load Column Base family.\r\n";
                            messtext += dgv[0, rowindex].Value.ToString() + ": " + dgv[1, rowindex].Value.ToString();
                            messtext += " — verify the following family used for conversion";
                            if (typename != "")
                            { messtext += " (type name: " + typename + ")"; }
                            messtext += ".\r\n\r\nFamily:\r\n";
                            messtext += familyfile;
                            MessageBox.Show(messtext, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        if(label2.Visible)
                        { label3.Top = label2.Bottom - 2; }
                        else
                        {
                            if(label1.Visible)
                            { label3.Top = label1.Bottom - 2; }
                            else
                            { label3.Top = komefirst; }
                        }
                        label3.Visible = true;
                        ret = false;
                    }
                }
                else
                {
                    if (!allflg)
                    {
                        string messtext = "Failed to load Column Base family.\r\n";
                        messtext += dgv[0, rowindex].Value.ToString() + ": " + dgv[1, rowindex].Value.ToString();
                        messtext += " — verify the following family used for conversion";
                        if (typename != "")
                        { messtext += " (type name: " + typename + ")"; }
                        messtext += ".\r\n\r\nFamily:\r\n";
                        messtext += familyfile;
                        MessageBox.Show(messtext, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    ret = false;
                }

                
            }
            bool loadflg = false;
            for (int i = 0; i < DGV2.Rows.Count; i++)
            {
                if (DGV2.Rows[i].Cells[4].Value.ToString() == "Load" || DGV2.Rows[i].Cells[4].Value.ToString().Contains("Cannot load"))
                {
                    loadflg = true;
                    break;
                }
            }
            if (!loadflg)
            {
                //string newlistname = "";
                //newlistname = STBParaBuild.listboxtext_del(listBox1.SelectedItem.ToString()); 
                //listBox1.Items[listBox1.SelectedIndex] = newlistname;
                button1.Enabled = false;
            }
            else
            { DGV2.Refresh(); }

            return ret;
        }

        #region STB data intake
        /// <summary> Whether STB file contains convertible members (shape checks)
        /// </summary>
        internal bool CheckSTB()
        {
            bool ret = true;
            string readerror = "";
            // Materials and concrete tables
            RevitLNK.MateData = new List<RevitLNK.Materialdata>();
            RevitLNK.ConcData = new List<RevitLNK.Concredata>();

            // Reset run state
            BClm = new List<BClmData>();
            STBload = new List<STBLoadflg>();
            LMD = new LevelMappingData();
            //Concname = new List<string>();
            //TekkotuPare = new List<Tekkotu>();
            Lof = new List<Loadflg_Class>();

            stb = null;
            stb2 = null;

            LogData.STBLog = new List<LogData.Log>();


            var data = XDocument.Load(RevitLNK.openfilename);
            var version = data.Root.Attribute("version")?.Value ?? "";

            var encoding = data.Declaration.Encoding switch
            {
                "UTF-8" => Encoding.UTF8,
                "utf-8" => Encoding.UTF8,
                "SHIFT_JIS" => Encoding.GetEncoding( "Shift_JIS" ),
                "Shift_JIS" => Encoding.GetEncoding( "Shift_JIS" ),
                "shift_jis" => Encoding.GetEncoding( "Shift_JIS" ),
                _ => Encoding.GetEncoding( data.Declaration.Encoding )
            } ;
                
            
            if (version.StartsWith("2"))
            {
                //ver2.0
                stb_ver = 2;
                RevitLNK.BaseText[2] = new string[] { "RC pile", "", "steel pipe pile", "precast pile PHC", "precast pile ST", "precast pile SC", "precast pile PRC", "precast pile CPRC" };

                stb2 = ST_BRIDGE_V2.ST_BRIDGE.Read(RevitLNK.openfilename, encoding);
                if ( stb2 is null ) return false ;
                stb2.SetApplyConditionsList();

                FromSTB_v2.CheckSTB_Column(stb2, this);
                FromSTB_v2.CheckSTB_Girder(stb2, this);
                FromSTB_v2.CheckSTB_Brace(stb2, this);
                FromSTB_v2.CheckSTB_Foundation(stb2, this);
                FromSTB_v2.CheckSTB_Sonota(stb2, this);
            }
            else
            {
                //ver1.4
                stb_ver = 1;
                RevitLNK.BaseText[2] = new string[] { "Cast-in-place pile", "Precast pile" };

                // Progress form
                ProgressBarForm pform = new ProgressBarForm
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                pform.Show();
                // Read STB
                pform.Text = RevitLNK.formtitle + " Loading file…";
                stb = new STBclass
                {
                    version = "1.4.00"
                };
                readerror = stb.ReadFile(RevitLNK.openfilename, pform);
                if (readerror != "")
                {
                    pform.Close();
                    MessageBox.Show(readerror, RevitLNK.formtitle + " File read", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return false;
                }
                pform.Close();

                CheckSTB_Column();
                CheckSTB_Girder();
                CheckSTB_Brace();
                CheckSTB_Foundation();
                CheckSTB_Sonota();
            }


            if (LogData.STBLog.Count() != 0)
            {
                ret = false;
            }

            return ret;
        }
        internal void STBload_Add(STBLoadflg nf )
        {
            bool addflg = true;
            for(int i =0; i < STBload.Count(); i++)
            {
                if(nf.kind == STBload[i].kind && nf.name == STBload[i].name && nf.flg == STBload[i].flg)
                {
                    addflg = false;
                    break;
                }
            }
            if (addflg) { STBload.Add(nf); }
        }

        internal void BClm_Add(BClmData bc)
        {
            bool addflg = true;
            for(int i =0; i < BClm.Count(); i++)
            {
                if(bc.company == BClm[i].company && bc.product_code == BClm[i].product_code)
                {
                    addflg = false;
                    break;
                }
            }

            if (addflg) { BClm.Add(bc); }
        }
        /// <summary> STB v1 columns — validate sections and register loads
        /// </summary>
        private void CheckSTB_Column()
        {            
            if (stb.StbModel.StbSections.StbSecColumns_RC != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecColumns_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC clm = stb.StbModel.StbSections.StbSecColumns_RC[i];
                    if (clm == null) { continue; }
                    try
                    {
                        STBLoadflg nf = new STBLoadflg
                        {
                            kind = clm.kind_column
                        };
                        if (nf.kind == "")
                        {
                            nf.kind = "COLUMN";
                            if (stb.StbModel.StbMembers.StbPosts != null)
                            {
                                if (stb.StbModel.StbMembers.StbPosts.Find(a => (a.id_section == clm.id)) != null)
                                {
                                    nf.kind = "POST";
                                }
                            }
                            clm.kind_column = nf.kind;
                        }
                        nf.flg = true;
                        if (clm.StbSecFigure.StbSecRect != null)
                        {
                            nf.name = RevitLNK.ClmText[0][0];
                        }
                        if (clm.StbSecFigure.StbSecCircle != null)
                        {
                            nf.name = RevitLNK.ClmText[0][1];
                        }
                        STBload_Add(nf);
                        ConcData_Add(clm.floor, "RC", ref clm.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC column (StbSecColumn_RC)");
                    }
                    
                }
            }
            if (stb.StbModel.StbSections.StbSecColumns_S != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecColumns_S.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_S clm = stb.StbModel.StbSections.StbSecColumns_S[i];
                    if (clm == null) { continue; }
                    try
                    {
                        for (int j = 0; j < clm.StbSecSteelColumn.Count(); j++)
                        {
                            if(j == 1 && clm.StbSecSteelColumn[j] == null) { continue; }
                            Data.MateData_Add(clm.StbSecSteelColumn[j].strength_main);
                            Data.MateData_Add(clm.StbSecSteelColumn[j].strength_web);

                            string shape = clm.StbSecSteelColumn[j].shape;
                            int ind = 0;
                            string shapetype = fromSTB.Check_Steel(stb, shape, ref ind);

                            STBLoadflg nf = new STBLoadflg
                            {
                                kind = clm.kind_column
                            };
                            if (nf.kind == "")
                            {
                                nf.kind = "COLUMN";
                                if (stb.StbModel.StbMembers.StbPosts != null)
                                {
                                    if (stb.StbModel.StbMembers.StbPosts.Find(a => (a.id_section == clm.id)) != null)
                                    { nf.kind = "POST"; }
                                }
                                clm.kind_column = nf.kind;
                            }
                            nf.flg = true;
                            if (shapetype == RevitLNK.st_steel_H)
                            {
                                nf.name = RevitLNK.ClmText[1][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_BH)
                            {
                                nf.name = RevitLNK.ClmText[1][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_Box)
                            {
                                nf.name = RevitLNK.ClmText[1][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.ClmText[1][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.ClmText[1][4];
                            }
                            else if (shapetype == RevitLNK.st_steel_T)
                            {
                                nf.name = RevitLNK.ClmText[1][5];
                            }
                            else if (shapetype == RevitLNK.st_steel_C)
                            {
                                nf.name = RevitLNK.ClmText[1][6];
                            }
                            else if (shapetype == RevitLNK.st_steel_L)
                            {
                                nf.name = RevitLNK.ClmText[1][7];
                            }
                            else if(shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S column (StbSecColumn_S)");
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + clm.name + "(section id=" + clm.id.ToString() + ") shape=\"" + shape + "\" ");
                            }
                            STBload_Add(nf);

                            if (clm.base_type == "EXPOSE")
                            {
                                if (clm.StbSecBaseProduct != null)
                                {
                                    STBLoadflg lf = new STBLoadflg
                                    {
                                        kind = "Column Base",
                                        flg = true,
                                        name = "Column Base"
                                    };
                                    STBload_Add(lf);

                                    BClmData bc = new BClmData
                                    {
                                        company = clm.StbSecBaseProduct.product_company,
                                        product_code = clm.StbSecBaseProduct.product_code
                                    };
                                    BClm_Add(bc);

                                    for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                    {
                                        if (RevitLNK.BClm[b].product_company == clm.StbSecBaseProduct.product_company &&
                                           RevitLNK.BClm[b].product_code == clm.StbSecBaseProduct.product_code)
                                        {
                                            RevitLNK.BClm[b].flg = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S column (StbSecColumn_S)");
                    }
                }
            }
            if (stb.StbModel.StbSections.StbSecColumns_SRC != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecColumns_SRC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm = stb.StbModel.StbSections.StbSecColumns_SRC[i];
                    if (clm == null) { continue; }
                    try
                    {
                        for (int j = 0; j < clm.StbSecSteelColumn_SRC.Count(); j++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class src = clm.StbSecSteelColumn_SRC[j];
                            if (j == 1 && src == null) { continue; }
                            string shape = clm.StbSecSteelColumn_SRC[j].build_up_shape;

                            STBLoadflg nf = new STBLoadflg
                            {
                                kind = clm.kind_column
                            };
                            if (nf.kind == "")
                            {
                                nf.kind = "COLUMN";
                                if (stb.StbModel.StbMembers.StbPosts != null)
                                {
                                    if (stb.StbModel.StbMembers.StbPosts.Find(a => (a.id_section == clm.id)) != null)
                                    { nf.kind = "POST"; }
                                }
                                clm.kind_column = nf.kind;
                            }
                            nf.flg = true;
                            if (clm.StbSecFigure == null) { continue; }
                            if (shape == "H")
                            {
                                if (clm.StbSecFigure.StbSecFigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][0]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][3]; }
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeH.strength_main);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeH.strength_web);
                            }
                            else if (shape == "CROSS")
                            {
                                if (clm.StbSecFigure.StbSecFigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][1]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][4]; }
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeCross.strength_main_X);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeCross.strength_web_X);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeCross.strength_main_Y);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeCross.strength_web_Y);

                            }
                            else if (shape == "T")
                            {
                                if (clm.StbSecFigure.StbSecFigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][2]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][5]; }
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeT.strength_main_H);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeT.strength_web_H);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeT.strength_main_T);
                                Data.MateData_Add(src.StbSecColumn_SRC_ShapeT.strength_web_T);
                            }
                            else if(shape == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC column (StbSecColumn_SRC)");
                            }
                            STBload_Add(nf);

                            if (clm.base_type == "UNEMBEDDED")
                            {
                                if (clm.StbSecBaseProduct != null)
                                {
                                    STBLoadflg lf = new STBLoadflg
                                    {
                                        kind = "Column Base",
                                        flg = true,
                                        name = "Column Base"
                                    };
                                    STBload_Add(lf);

                                    BClmData bc = new BClmData
                                    {
                                        company = clm.StbSecBaseProduct.product_company,
                                        product_code = clm.StbSecBaseProduct.product_code
                                    };
                                    BClm_Add(bc);

                                    for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                    {
                                        if (RevitLNK.BClm[b].product_company == clm.StbSecBaseProduct.product_company &&
                                           RevitLNK.BClm[b].product_code == clm.StbSecBaseProduct.product_code)
                                        {
                                            RevitLNK.BClm[b].flg = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC column (StbSecColumn_SRC)");
                    }




                    ConcData_Add(clm.floor, "SRC", ref clm.strength_concrete);
                }
            }
            if (stb.StbModel.StbSections.StbSecColumns_CFT != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecColumns_CFT.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_CFT clm = stb.StbModel.StbSections.StbSecColumns_CFT[i];
                    if(clm == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add(clm.floor, "CFT", ref clm.strength_concrete);                    
                        for (int j = 0; j < clm.StbSecSteelColumn_CFT.Count(); j++)
                        {
                            if(j == 1 && clm.StbSecSteelColumn_CFT[j] == null) { continue; }
                            // Steel material grades
                            Data.MateData_Add(clm.StbSecSteelColumn_CFT[j].strength_main);

                            string shape = clm.StbSecSteelColumn_CFT[j].shape;
                            int ind = 0;
                            string shapetype = fromSTB.Check_Steel(stb, shape, ref ind);
                            STBLoadflg nf = new STBLoadflg
                            {
                                kind = clm.kind_column
                            };
                            if (nf.kind == "")
                            {
                                nf.kind = "COLUMN";
                                if (stb.StbModel.StbMembers.StbPosts != null)
                                {
                                    if (stb.StbModel.StbMembers.StbPosts.Find(a => (a.id_section == clm.id)) != null)
                                    { nf.kind = "POST"; }                                    
                                }
                                clm.kind_column = nf.kind;
                            }
                            nf.flg = true;
                            if (shapetype == RevitLNK.st_steel_Box || shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.ClmText[3][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.ClmText[3][1];
                            }
                            else if(shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "CFT column (StbSecColumn_CFT)");
                            }
                            STBload_Add(nf);
                            if(clm.base_type == "UNEMBEDDED")
                            {
                                if(clm.StbSecBaseProduct != null)
                                {
                                    STBLoadflg lf = new STBLoadflg
                                    {
                                        kind = "Column Base",
                                        flg = true,
                                        name = "Column Base"
                                    };
                                    STBload_Add(lf);

                                    BClmData bc = new BClmData
                                    {
                                        company = clm.StbSecBaseProduct.product_company,
                                        product_code = clm.StbSecBaseProduct.product_code
                                    };
                                    BClm_Add(bc);

                                    for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                    {
                                        if (RevitLNK.BClm[b].product_company == clm.StbSecBaseProduct.product_company &&
                                           RevitLNK.BClm[b].product_code == clm.StbSecBaseProduct.product_code)
                                        {
                                            RevitLNK.BClm[b].flg = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }    
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "CFT column (StbSecColumn_CFT)");
                    }                
                }
            }
        }
        /// <summary> STB v1 beams / girders
        /// </summary>
        private void CheckSTB_Girder()
        {
            if (stb.StbModel.StbSections.StbSecBeams_RC != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecBeams_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC gir = stb.StbModel.StbSections.StbSecBeams_RC[i];
                    if(gir == null) { continue; }
                    try
                    {
                        STBLoadflg nf = new STBLoadflg
                        {
                            kind = gir.kind_beam
                        };
                        if (nf.kind == "")
                        {
                            nf.kind = "GIRDER";
                            if (stb.StbModel.StbMembers.StbBeams != null)
                            {
                                if (stb.StbModel.StbMembers.StbBeams.Find(a => (a.id_section == gir.id)) != null)
                                { nf.kind = "BEAM"; }
                            }
                            gir.kind_beam = nf.kind;
                        }
                        nf.flg = true;
                        if (gir.isCanti)
                        {
                            if (gir.isFoundation)
                            {
                                if (gir.kind_beam == "GIRDER")
                                { nf.name = RevitLNK.CGirText[0][0]; }
                                else
                                { nf.name = RevitLNK.CBeamText[0][0]; }
                            }
                            else
                            {
                                if (gir.kind_beam == "GIRDER")
                                { nf.name = RevitLNK.CGirText[0][1]; }
                                else
                                { nf.name = RevitLNK.CBeamText[0][1]; }
                            }
                        }
                        else
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecFigureClass fig = gir.StbSecFigure;
                            switch (gir.StbSecFigure.StbSecFigureType) // 2016/11/07 family detail mode: detect haunches / three identical sections
                            {
                                case 1:
                                    if (gir.StbSecBar_Arrangement == null) // 2017/05/19 no rebar tags → treat as full-section conversion
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType == 1)
                                        {
                                            if (gir.isFoundation)
                                            {
                                                if (gir.kind_beam == "GIRDER")
                                                { nf.name = RevitLNK.GirText[0][0]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][0]; }
                                            }
                                            else
                                            {
                                                if (gir.kind_beam == "GIRDER")
                                                { nf.name = RevitLNK.GirText[0][2]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][2]; }
                                            }
                                        }
                                        else
                                        {
                                            if (gir.isFoundation)
                                            {
                                                if (gir.kind_beam == "GIRDER")
                                                { nf.name = RevitLNK.GirText[0][1]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][1]; }
                                            }
                                            else
                                            {
                                                if (gir.kind_beam == "GIRDER")
                                                { nf.name = RevitLNK.GirText[0][3]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][3]; }
                                            }
                                        }
                                    }
                                    break;
                                case 2:
                                    if (fig.StbSecTaper.depth_start != fig.StbSecTaper.depth_end || fig.StbSecTaper.width_start != fig.StbSecTaper.width_end)
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][1]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][1]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][3]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][3]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    break;
                                case 3:
                                    if (fig.StbSecHaunch.depth_start != fig.StbSecHaunch.depth_center || fig.StbSecHaunch.depth_end != fig.StbSecHaunch.depth_center ||
                                        fig.StbSecHaunch.width_start != fig.StbSecHaunch.width_center || fig.StbSecHaunch.width_end != fig.StbSecHaunch.width_center)
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][1]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][1]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][3]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][3]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    break;
                            }
                        }
                        STBload_Add(nf);

                        // Concrete strength
                        ConcData_Add(gir.floor, "RC", ref gir.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC beam (StbSecBeam_RC)");
                    }
                    
                }
            }
            if (stb.StbModel.StbSections.StbSecBeams_S != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecBeams_S.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir = stb.StbModel.StbSections.StbSecBeams_S[i];
                    if (gir == null) { continue; }
                    try
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (j == 1 && gir.StbSecSteelBeam[j] == null) { continue; } // mid-span section may be absent

                            // Steel material grades
                            Data.MateData_Add(gir.StbSecSteelBeam[j].strength_main);
                            Data.MateData_Add(gir.StbSecSteelBeam[j].strength_web);

                            // Steel shape / profile
                            string shape = gir.StbSecSteelBeam[j].shape;
                            int ind = 0;
                            string shapetype = fromSTB.Check_Steel(stb, shape, ref ind);

                            STBLoadflg nf = new STBLoadflg
                            {
                                kind = gir.kind_beam
                            };
                            if (nf.kind == "")
                            {
                                nf.kind = "GIRDER";
                                if (stb.StbModel.StbMembers.StbBeams != null)
                                {
                                    if (stb.StbModel.StbMembers.StbBeams.Find(a => (a.id_section == gir.id)) != null)
                                    { nf.kind = "BEAM"; }
                                }

                                gir.kind_beam = nf.kind;
                            }
                            nf.flg = true;
                            switch (shapetype)
                            {
                                case RevitLNK.st_steel_H:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.CGirText[1][0]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][0]; }
                                    }
                                    else
                                    {
                                        // 2016/11/07 family detail mode (see RC beam path)
                                        bool getflg = false;
                                        for (int s = 0; s < gir.StbSecSteelBeam.Count(); s++)
                                        {
                                            if (gir.StbSecSteelBeam[s] == null) { continue; }
                                            if (gir.StbSecSteelBeam[s].shape != shape)
                                            {
                                                if (gir.kind_beam == "GIRDER")
                                                { nf.name = RevitLNK.GirText[1][5]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[1][5]; }
                                                getflg = true;
                                            }
                                        }
                                        if (!getflg)
                                        {
                                            if (gir.kind_beam == "GIRDER")
                                            { nf.name = RevitLNK.GirText[1][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[1][0]; }
                                        }
                                    }
                                    break;
                                case RevitLNK.st_steel_BH:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.CGirText[1][1]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][1]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.GirText[1][1]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][1]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_C:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.CGirText[1][2]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][2]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.GirText[1][2]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][2]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_L:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.CGirText[1][3]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][3]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.GirText[1][3]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][3]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_LipC:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.CGirText[1][4]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][4]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == "GIRDER")
                                        { nf.name = RevitLNK.GirText[1][4]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][4]; }
                                    }
                                    break;
                                case "":
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S beam (StbSecBeam_S)");
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + gir.name + "(section id=" + gir.id.ToString() + ") shape=\"" + shape + "\" ");
                                    break;

                            }

                            STBload_Add(nf);
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S beam (StbSecBeam_S)");
                    }                   
                }
            }
            if (stb.StbModel.StbSections.StbSecBeams_SRC != null)
            {
                
                for (int i = 0; i < stb.StbModel.StbSections.StbSecBeams_SRC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC gir = stb.StbModel.StbSections.StbSecBeams_SRC[i];
                    if (gir == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add(gir.floor, "SRC", ref gir.strength_concrete);

                        for (int j = 0; j < 3; j++)
                        {
                            if (gir.StbSecSteelBeam[j] == null) { continue; }
                            // Steel material grades
                            Data.MateData_Add(gir.StbSecSteelBeam[j].strength_main);
                            Data.MateData_Add(gir.StbSecSteelBeam[j].strength_web);

                            // Steel shape / profile
                            string shape = gir.StbSecSteelBeam[j].shape;
                            int ind = 0;
                            string shapetype = fromSTB.Check_Steel(stb, shape, ref ind);

                            if(shapetype == "") { LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC beam (StbSecBeam_SRC)"); }

                            STBLoadflg nf = new STBLoadflg
                            {
                                kind = gir.kind_beam
                            };
                            if (nf.kind == "")
                            {
                                nf.kind = "GIRDER";
                                if (stb.StbModel.StbMembers.StbBeams != null)
                                {
                                    if (stb.StbModel.StbMembers.StbBeams.Find(a => (a.id_section == gir.id)) != null)
                                    { nf.kind = "BEAM"; }
                                }

                                gir.kind_beam = nf.kind;
                            }
                            nf.flg = true;
                            if (shapetype == RevitLNK.st_steel_H || shapetype == RevitLNK.st_steel_BH)
                            {
                                if (gir.isCanti)
                                {
                                    if (gir.kind_beam == "GIRDER")
                                    { nf.name = RevitLNK.CGirText[2][0]; }
                                    else
                                    { nf.name = RevitLNK.CBeamText[2][0]; }
                                }
                                else
                                {
                                    if (gir.kind_beam == "GIRDER")
                                    { nf.name = RevitLNK.GirText[2][0]; }
                                    else
                                    { nf.name = RevitLNK.BeamText[2][0]; }
                                }
                            }
                            else if(shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC beam (StbSecBeam_SRC)");
                            }
                            STBload_Add(nf);
                        }

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC beam (StbSecBeam_SRC)");
                    }
                    
                }
            }
        }
        /// <summary> STB v1 braces
        /// </summary>
        private void CheckSTB_Brace()
        {
            if (stb.StbModel.StbSections.StbSecBraces_S != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecBraces_S.Count(); i++)
                {                   
                    STBclass.StbModelClass.StbSectionsClass.StbSecBrace_S bra = stb.StbModel.StbSections.StbSecBraces_S[i];
                    if (bra == null) { continue; }
                    try
                    {
                        for (int j = 0; j < bra.StbSecSteelBrace.Count(); j++)
                        {
                            if (bra.StbSecSteelBrace[j] == null) { continue; }
                            // Steel material grades
                            Data.MateData_Add(bra.StbSecSteelBrace[j].strength_main);
                            Data.MateData_Add(bra.StbSecSteelBrace[j].strength_web);

                            string shape = bra.StbSecSteelBrace[j].shape;
                            int ind = 0;
                            string shapetype = fromSTB.Check_Steel(stb, shape, ref ind);

                            STBLoadflg nf = new STBLoadflg
                            {
                                flg = true
                            };
                            if (shapetype == RevitLNK.st_steel_H)
                            {
                                nf.name = RevitLNK.SBraText[0][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_BH)
                            {
                                nf.name = RevitLNK.SBraText[0][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_Box)
                            {
                                nf.name = RevitLNK.SBraText[0][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.SBraText[0][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.SBraText[0][4];
                            }
                            else if (shapetype == RevitLNK.st_steel_C)
                            {
                                nf.name = RevitLNK.SBraText[1][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_L)
                            {
                                nf.name = RevitLNK.SBraText[1][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_LipC)
                            {
                                nf.name = RevitLNK.SBraText[1][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_FB)
                            {
                                nf.name = RevitLNK.SBraText[1][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Bar)
                            {
                                nf.name = RevitLNK.SBraText[1][4];
                            }
                            else
                            {
                                // Log (not converted)
                                if (shapetype == "")
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S Brace(StbSecBrace_S)");
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + bra.name + "(section id=" + bra.id.ToString() + ") shape=\"" + shape + "\" ");
                                }
                                else
                                { LogData.AddLog(LogData.LogKind.Warning, 2200, "[S Brace]" + bra.name + "(section id=" + bra.id.ToString() + ") is tee steel (" + shapetype + ")"); }
                            }
                            STBload_Add(nf);
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S Brace(StbSecBrace_S)");
                    }
                    
                }
            }
        }
        /// <summary> STB v1 foundations and piles
        /// </summary>
        private void CheckSTB_Foundation()
        {
            if (stb.StbModel.StbSections.StbSecFoundations_RC != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecFoundations_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC frc = stb.StbModel.StbSections.StbSecFoundations_RC[i];
                    if (frc == null) { continue; }
                    try
                    {
                        STBLoadflg nf = new STBLoadflg
                        {
                            flg = true
                        };
                        if (frc.StbSecFigure != null)
                        {
                            switch (frc.StbSecFigure.StbSecFigureType)
                            {
                                case 1:
                                    nf.name = RevitLNK.BaseText[0][0];
                                    break;
                                case 2:
                                    nf.name = RevitLNK.BaseText[0][1];
                                    break;
                                case 3:
                                    nf.name = RevitLNK.BaseText[0][2];
                                    break;
                                case 4:
                                    nf.name = RevitLNK.BaseText[0][3];
                                    break;
                                case 5:
                                    nf.name = RevitLNK.BaseText[0][4];
                                    break;
                                case 6:
                                    nf.name = RevitLNK.BaseText[1][0];
                                    break;
                            }
                        }
                        STBload_Add(nf);

                        // Concrete strength
                        ConcData_Add("", "RC", ref frc.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC foundation (StbSecFoundations_RC)");
                    }
                    
                }
            }
            if (stb.StbModel.StbSections.StbSecPiles_RC != null)
            {
                for (int i = 0; i < stb.StbModel.StbSections.StbSecPiles_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecPile_RC prc = stb.StbModel.StbSections.StbSecPiles_RC[i];
                    if (prc == null) { continue; }
                    try
                    {
                        bool logflg = false;
                        if (prc.StbSecFigure != null)
                        {                           
                            switch(prc.StbSecFigure.StbSecFigureType)
                            {
                                case 1:
                                    if (prc.StbSecFigure.StbSecStraight.D == 0)
                                    { logflg = true; }
                                    break;
                                case 2:
                                    if(prc.StbSecFigure.StbSecExtended_Foot.D_axial == 0|| prc.StbSecFigure.StbSecExtended_Foot.D_extended_foot == 0)
                                    { logflg = true; }
                                    break;
                                case 3:
                                    if(prc.StbSecFigure.StbSecExtended_Top.D_axial == 0 || prc.StbSecFigure.StbSecExtended_Top.D_extended_top == 0)
                                    { logflg = true; }
                                    break;
                                case 4:
                                    if (prc.StbSecFigure.StbSecExtended_Top_Foot.D_axial == 0 || prc.StbSecFigure.StbSecExtended_Top_Foot.D_extended_foot == 0 ||
                                        prc.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top == 0)
                                    { logflg = true; }
                                    break;
                            }
                            
                        }
                        if(logflg == true)
                        {
                            LogData.AddSTBLog(LogData.LogKind.Error, 3100, "Pile foundation (StbSecPiles_RC)");
                            break; 
                        }
                        STBLoadflg nf = new STBLoadflg
                        {
                            flg = true,
                            name = RevitLNK.BaseText[2][0]
                        };
                        STBload_Add(nf);
                        STBLoadflg nf2 = new STBLoadflg
                        {
                            flg = true,
                            name = RevitLNK.BaseText[2][1]
                        };
                        STBload_Add(nf2);

                        // Concrete strength
                        ConcData_Add("", "RC", ref prc.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC pile (StbSecPiles_RC)");
                    }
                    
                }
            }
        }
        /// <summary> STB v1 — slabs, walls, parapets, foundation columns
        /// </summary>
        private void CheckSTB_Sonota()
        {
            if (stb.StbModel.StbMembers.StbFoundationColumns != null)
            {
                bool recflg = false, rouflg = false;
                for (int i = 0; i < stb.StbModel.StbMembers.StbFoundationColumns.Count(); i++)
                {
                    if (recflg && rouflg) { break; }

                    if (stb.StbModel.StbMembers.StbFoundationColumns[i] == null) { continue; }
                    try
                    {
                        int id_section = stb.StbModel.StbMembers.StbFoundationColumns[i].id_section;
                        for (int j = 0; j < stb.StbModel.StbSections.StbSecColumns_RC.Count(); j++)
                        {
                            if (stb.StbModel.StbSections.StbSecColumns_RC[j] == null) { continue; }

                            if (stb.StbModel.StbSections.StbSecColumns_RC[j].id == id_section)
                            {
                                if (stb.StbModel.StbSections.StbSecColumns_RC[j].StbSecFigure == null) { continue; }

                                if (stb.StbModel.StbSections.StbSecColumns_RC[j].StbSecFigure.StbSecFigureType == 1)
                                {
                                    STBLoadflg nf = new STBLoadflg
                                    {
                                        flg = true,
                                        name = RevitLNK.FClmText[0][0]
                                    };
                                    STBload_Add(nf);
                                    recflg = true;
                                }
                                else
                                {
                                    STBLoadflg nf = new STBLoadflg
                                    {
                                        flg = true,
                                        name = RevitLNK.FClmText[0][1]
                                    };
                                    STBload_Add(nf);
                                    rouflg = true;
                                }
                            }
                        }
                    }
                    catch
                    {
                       // Shape already validated via RC column
                    }
                   
                }
                // Concrete already collected on Column_RC pass
            }
            if (stb.StbModel.StbSections.StbSecSlabs_RC != null)
            {
                STBLoadflg nf = new STBLoadflg
                {
                    kind = "RC Slab",
                    flg = true,
                    name = "Slab, Deck Plate"
                };
                STBload_Add(nf);
               
                for (int i = 0; i < stb.StbModel.StbSections.StbSecSlabs_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC sla = stb.StbModel.StbSections.StbSecSlabs_RC[i];
                    if(sla == null) { continue; }
                    try
                    {
                        if (sla.isFoundation)
                        {
                            STBLoadflg nf2 = new STBLoadflg
                            {
                                kind = "Foundation Slab",
                                flg = true,
                                name = "Foundation Slab"
                            };
                            STBload_Add(nf2);
                        }
                        // Concrete strength
                        ConcData_Add("", "RC", ref sla.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC Slab(StbSecSlabs_RC)");
                    }                   
                }
            }
            if (stb.StbModel.StbSections.StbSecSlabs_Deck != null)
            {
                STBLoadflg nf = new STBLoadflg
                {
                    kind = "Deck Plate",
                    flg = true,
                    name = "Slab, Deck Plate"
                };
                STBload_Add(nf);
                for (int i = 0; i < stb.StbModel.StbSections.StbSecSlabs_Deck.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck deck = stb.StbModel.StbSections.StbSecSlabs_Deck[i];
                    if(deck == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add("", "RC", ref deck.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "Deck slab (StbSecSlabs_Deck)");
                    }
                    
                }
            }
            if (stb.StbModel.StbSections.StbSecSlabs_Precast != null)
            {
                STBLoadflg nf = new STBLoadflg
                {
                    kind = "Precast Slab",
                    flg = true,
                    name = "Slab, Deck Plate"
                };
                STBload_Add(nf);
                for (int i = 0; i < stb.StbModel.StbSections.StbSecSlabs_Precast.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast prod = stb.StbModel.StbSections.StbSecSlabs_Precast[i];
                    if(prod == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add("", "RC", ref prod.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "Precast Slab(StbSecSlabs_Precast)");
                    }                   
                }
            }
            if (stb.StbModel.StbSections.StbSecWalls_RC != null)
            {
                STBLoadflg nf = new STBLoadflg
                {
                    kind = "Wall",
                    flg = true,
                    name = "Wall, RC Parapet"
                };
                STBload_Add(nf);
                for (int i = 0; i < stb.StbModel.StbSections.StbSecWalls_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC wall = stb.StbModel.StbSections.StbSecWalls_RC[i];
                    if(wall == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add("", "RC", ref wall.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC wall (StbSecWalls_RC)");
                    }
                }
            }
            if (stb.StbModel.StbSections.StbSecParapets_RC != null)
            {
                STBLoadflg nf = new STBLoadflg
                {
                    kind = "RC Parapet",
                    flg = true,
                    name = "Wall, RC Parapet"
                };
                STBload_Add(nf);
                for (int i = 0; i < stb.StbModel.StbSections.StbSecParapets_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC wall = stb.StbModel.StbSections.StbSecParapets_RC[i];
                    if(wall == null) { continue; }
                    try
                    {
                        // Concrete strength
                        ConcData_Add("", "RC", ref wall.strength_concrete);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC Parapet(StbSecParapets_RC)");
                    }                   
                }
            }
        }


        /// <summary> Collect STB concrete strength into RevitLNK.ConcData
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="kouzou"></param>
        /// <param name="conc"></param>
        private void ConcData_Add(string floor, string kouzou, ref string conc)
        {
            if(conc == "") // member-level concrete grade empty → resolve from floor / building defaults
            {
                // Prefer story-level concrete grade
                if (floor != "")
                {
                    for (int i = 0; i < stb.StbModel.StbStories.Count(); i++)
                    {
                        STBclass.StbModelClass.StbStory story = stb.StbModel.StbStories[i];
                        if (floor == story.name)
                        {
                            if (story.concrete_strength != "")
                            {
                                conc = story.concrete_strength;
                                break;
                            }
                        }
                    }
                }
                // Story grade still empty → use common building default
                if(conc == "")
                {
                    // Whole-building concrete from common settings
                    conc = stb.StbCommon.concrete_strength;
                }
            }
            if (conc == "") return; // no grade at member, story, or building scope

            bool sameflg = false;
            for(int i = 0;i < RevitLNK.ConcData.Count(); i++)
            {
                if(RevitLNK.ConcData[i].kouzou == kouzou && RevitLNK.ConcData[i].STBstrength == conc)
                {
                    sameflg = true;
                    break;
                }
            }
            if(!sameflg)
            {
                RevitLNK.Concredata cd = new RevitLNK.Concredata
                {
                    kouzou = kouzou,
                    STBstrength = conc
                };
                RevitLNK.ConcData.Add(cd);
            }
        }        
        /// <summary> Tests whether named STB loads are present (paired with Optional kind)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        private bool Check_STBLoadflg(string name, string kind )
        {
            bool ret = false;

            for (int i =0; i < STBload.Count();i++)
            {
                if (kind != "")
                {
                    if (STBload[i].name == name && STBload[i].kind == kind)
                    {
                        ret = STBload[i].flg;
                        break;
                    }
                }
                else
                {
                    if (STBload[i].name == name)
                    {
                        ret = STBload[i].flg;
                        break;
                    }
                }
            }

            return ret;
        }
        private bool Check_STBLoadflg(string[][] name, string kind)
        {
            bool ret = false;
            for(int i = 0; i < name.Count();i++)
            {
                for (int j = 0; j < name[i].Count(); j++)
                {
                    ret = Check_STBLoadflg(name[i][j], kind);
                    if (ret) { break; }
                }
                if (ret) { break; }
            }
            return ret;
        }

        
        #endregion

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                SetFamily.FoFName.convflg[2][0] = true;
                SetFamily.FoFName.convflg[2][1] = false;
                if(radChecked.Count() == 2)
                {
                    radChecked[0] = true;
                    radChecked[1] = false;
                }                
            }
            else
            {
                SetFamily.FoFName.convflg[2][0] = false;
                SetFamily.FoFName.convflg[2][1] = true;
                if (radChecked.Count() == 2)
                {
                    radChecked[0] = false;
                    radChecked[1] = true;
                }
            }
        }

        private void ConvertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.Tag == null) { return; }
            if(this.Tag.ToString() != "")
            {
                e.Cancel = true;
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            // Debug: clear all conversion checkboxes
            Chb_Checked.ForEach(a => a.chbchecked = false);
            Selectlistbox(listBox1.SelectedIndex);
        }
    }
}
