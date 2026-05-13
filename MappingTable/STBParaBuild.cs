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
using System.Runtime.Versioning ;


namespace MappingTable
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



        //CheckBoxのONOFF
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

        //コントロールのフォーム端からの距離
        const int len = 10;
        //部材用ラベル・チェックボックスの表示間隔
        int interval = 240;
        //ラベル上部とテキストボックス下部の距離
        const int p5 = 2;
        //ラベル下部とテキストボックス上部の距離
        const int p2 = 2;



        private void STBParaBuild_Load(object sender, EventArgs e)
        {
            //フォームタイトル
            this.Text = Commons.SystemName + " Batch Add Parameters " + Commons.GetVersion();

            LogData.Data = new List<LogData.Log>();

            //CheckBoxのONOFF初期化
            ChflgSet(Commons.ClmText2, ref ChClm, ref ChClm_name);
            ChflgSet(Commons.GirText, ref ChGir, ref ChGir_name);
            ChflgSet(Commons.BeamText, ref ChBeam, ref ChBeam_name);
            ChflgSet(Commons.CGirText, ref ChCGir, ref ChCGir_name);
            ChflgSet(Commons.CBeamText, ref ChCBeam, ref ChCBeam_name);
            ChflgSet(Commons.SBraText, ref ChSBra, ref ChSBra_name);
            ChflgSet(Commons.SlabText, ref ChSlab, ref ChSlab_name);
            ChflgSet(Commons.WallText, ref ChWall, ref ChWall_name);
            ChflgSet(Commons.FoundationText2, ref ChFound, ref ChFound_name);
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
                    errlab[i][j] = new Label
                    {
                        AutoSize = true,
                        Text = "Cannot add parameters (not loaded)",
                        ForeColor = Color.Red
                    };
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

            //groupBox1のサイズ
            groupBox1.ClientSize = new Size(interval * 3, ch[1][7].Bottom + errlab[0][0].Height + len + button1.Height + len);

            //Formのサイズ
            int x = len + listBox1.Width + len + groupBox1.Width + len;
            int y = len + groupBox1.Height + len + OK.Height + len;
            this.ClientSize = new Size(x, y);

            //listBox1の位置
            listBox1.Left = len;
            listBox1.Top = len;
            //groupbox1の位置
            groupBox1.Left = listBox1.Width + len * 2;
            groupBox1.Top = len;
            //ボタン・ラベルの位置 ※ボタン・リンクラベルの間隔はlen/2
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

        /// <summary>CheckBoxのONOFF初期化
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
      

        /// <summary>コントロールの配置
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

            //柱のチェックボックスのセット
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
                if(!listBox1.Items[0].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[0] = Listboxtext(listBox1.Items[0].ToString()); }
            }
            else
            {
                listBox1.Items[0] = Listboxtext_del(listBox1.Items[0].ToString());
            }
            //梁のチェックボックスのセット
            flg = true;
            for (int i = 0; i < Commons.GirText.Length; i++)
            {
                for (int j = j = 0; j < Commons.GirText[i].Length; j++)
                {
                    ChGir[i][j] = SetFamily.GirFName.flg[i][j];
                    if (!SetFamily.GirFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[1].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[1] = string.Format(Listboxtext(listBox1.Items[1].ToString())); }
            }
            else
            {
                listBox1.Items[1] = Listboxtext_del(listBox1.Items[1].ToString());
            }
            //小梁のチェックボックスのセット
            flg = true;
            for (int i = 0; i < Commons.BeamText.Length; i++)
            {
                for (int j = j = 0; j < Commons.BeamText[i].Length; j++)
                {
                    ChBeam[i][j] = SetFamily.BeamFName.flg[i][j];
                    if (!SetFamily.BeamFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[2].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[2] = string.Format(Listboxtext(listBox1.Items[2].ToString())); }
            }
            else
            {
                listBox1.Items[2] = Listboxtext_del(listBox1.Items[2].ToString());
            }
            //片持梁のチェックボックスのセット
            flg = true;
            for (int i = 0; i < Commons.CGirText.Length; i++)
            {
                for (int j = j = 0; j < Commons.CGirText[i].Length; j++)
                {
                    ChCGir[i][j] = SetFamily.CGirFName.flg[i][j];
                    if (!SetFamily.CGirFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[3].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[3] = string.Format(Listboxtext(listBox1.Items[3].ToString())); }
            }
            else
            {
                listBox1.Items[3] = Listboxtext_del(listBox1.Items[3].ToString());
            }
            //片持小梁のチェックボックスのセット
            flg = true;
            for (int i = 0; i < Commons.CBeamText.Length; i++)
            {
                for (int j = j = 0; j < Commons.CBeamText[i].Length; j++)
                {
                    ChCBeam[i][j] = SetFamily.CBeamFName.flg[i][j];
                    if (!SetFamily.CBeamFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[4].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[4] = string.Format(Listboxtext(listBox1.Items[4].ToString())); }
            }
            else
            {
                listBox1.Items[4] = Listboxtext_del(listBox1.Items[4].ToString());
            }
            //ブレースのチェックボックス
            flg = true;
            for (int i = 0; i < Commons.SBraText.Length; i++)
            {
                for (int j = j = 0; j < Commons.SBraText[i].Length; j++)
                {
                    ChSBra[i][j] = SetFamily.SBraFName.flg[i][j];
                    if (!SetFamily.SBraFName.flg[i][j]) { flg = false; }
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[5].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[5] = string.Format(Listboxtext(listBox1.Items[5].ToString())); }
            }
            else
            {
                listBox1.Items[5] = Listboxtext_del(listBox1.Items[5].ToString());
            }
            //基礎のチェックボックスのセット
            flg = true;
            for (int i = 0; i < Commons.FoundationText2.Length; i++)
            {
                int jj = 0;
                for (int j = j = 0; j < Commons.FoundationText2[i].Length; j++)
                {
                    if (Commons.FoundationText2[i][j] == "") continue;
                    ChFound[i][jj] = SetFamily.FoFName.flg[i][j];
                    if (!SetFamily.FoFName.flg[i][j]) { flg = false; }
                    jj++;
                }
            }
            if (!flg)
            {
                if (!listBox1.Items[8].ToString().Contains("<Not Loaded>"))
                { listBox1.Items[8] = string.Format(Listboxtext(listBox1.Items[8].ToString())); }
            }
            else
            {
                listBox1.Items[8] = Listboxtext_del(listBox1.Items[8].ToString());
            }
        }

        /// <summary> 未ロードありテキストの追加
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string Listboxtext(string str)
        {
            string ret = str;
            if (str.Contains("<Not Loaded>")) { return ret; }
            System.Text.Encoding sjis = System.Text.Encoding.GetEncoding("Shift_JIS");
            string mi = "<Not Loaded>";
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
        internal static string Listboxtext_del(string str)
        {
            string ret = str;

            if (!str.Contains("<Not Loaded>")) { return ret; }
            if(str.Contains("\t"))
            {
                ret = ret.Replace("\t", "");
            }
            do
            {
                ret = ret.Replace("　","");
            } while (ret.Contains("　"));
            ret = ret.Replace("<Not Loaded>", "");

            return ret;
        }

        /// <summary>コントロールのVisibleをfalseに
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
                    ch[i][j].Text = "";
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

                OpenFileDialog opf = new OpenFileDialog
                {
                    Title = "Select Family",
                    Filter = rfaname + "|" + rfaname + "|" + "Revit Family Files|*.rfa|All Files|*.*",
                    FileName = rfaname
                };
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    familyfile = opf.FileName;
                    //選択したファイルがマッピング指定されたファミリファイルでない場合
                    if (faminame != System.IO.Path.GetFileNameWithoutExtension(familyfile))
                    {
                        string mes = lab[bi][bj].Text + ": Please select " + faminame + ".";
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
                        //これが無いとgroupbox内のコントロールが消える
                        Selectlistbox(listindex);                       
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
                        string mes = "Please verify the family category is [" + catename + "].";
                        MessageBox.Show(lab[bi][bj].Text + ": Failed to load " + faminame + ".\r\n" + mes,
                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    
        private bool ReloadFamily(string FamilyFile,  string familyName)
        {
            bool retcode = false;
            Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(Commons.doc, "Loading Family");
            FamilyOption famop = new FamilyOption();
            try
            {
                transaction.Start(familyName + " — Load");

                if (Commons.doc.LoadFamily(FamilyFile, famop, out Autodesk.Revit.DB.Family family))
                {
                    if (familyName == family.Name)
                    {
                        retcode = true;
                        //ロードしたファミリをチェック
                        LoadFamily.LoadFfamily_fromProject();

                        //それぞれの部材のファミリ名を更新
                        SetFamily.SetClmFamilyName();
                        SetFamily.SetBClmFamilyName();
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                        SetFamily.SetBraFamilyName();
                        SetFamily.SetFoundationFamilyName();
                        //コントロールをセット
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

            return retcode;
        }
        private void ErrLab_and_errbt_Set(int i, int j)
        {
            errlab[i][j].Left = ch[i][j].Left;
            errlab[i][j].Top = lab[i][j].Bottom -p5;
            errbt[i][j].Top = errlab[i][j].Bottom - errbt[i][j].Height;
            errbt[i][j].Left = errlab[i][j].Right;
            ch[i][j].Top = errlab[i][j].Bottom -p5;            
            errbt[i][j].Visible = true;
            errlab[i][j].Visible = true;
            if (!listBox1.Items[listBox1.SelectedIndex].ToString().Contains("<Not Loaded>"))
            {
                if (listBox1.SelectedIndex == 8 || listBox1.SelectedIndex == 0)
                { listBox1.Items[listBox1.SelectedIndex] = string.Format(listBox1.Items[listBox1.SelectedIndex].ToString() + " <Not Loaded>", listBox1.SelectedIndex);  }
                else
                { listBox1.Items[listBox1.SelectedIndex] = string.Format(listBox1.Items[listBox1.SelectedIndex].ToString() + "\t<Not Loaded>", listBox1.SelectedIndex); }
            }
        }
        /// <summary>リストボックスの選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            Selectlistbox(listBox1.SelectedIndex);

        }

        private void Selectlistbox(int ind, bool flg = true)
        {
            //0：柱・間柱　0：基礎柱　1：梁　2：小梁　3：片持梁　4：片持小梁　5：ブレース　6：床　7：壁　8：基礎　9：基礎スラブ
            bool allcbfalse = true; //全てのチェックボックスがEnabled⇒全選択・全解除もEnabled
            
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
                                    ErrLab_and_errbt_Set(newi, newj);
                                }
                            }
                            else
                            {
                                CheckBox_Change(ch[i][j], SetFamily.ClmFName.FamilyName[i][j], SetFamily.ClmFName.flg[i][j]);
                                ChClm_name[i][j] = SetFamily.ClmFName.FamilyName[i][j];
                                if (!SetFamily.ClmFName.flg[i][j])
                                {
                                    ErrLab_and_errbt_Set(i, j);
                                }
                            }
                            
                        }
                    }
                    ControlsSet(Commons.ClmText2);
                    break;
                case 1:
                    if (!flg)
                    {
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < Commons.GirText.Length; i++)
                    {
                        for (int j = j = 0; j < Commons.GirText[i].Length; j++)
                        {
                            if (SetFamily.GirFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.GirFName.FamilyName[i][j], SetFamily.GirFName.flg[i][j]);
                            ChGir_name[i][j] = SetFamily.GirFName.FamilyName[i][j];
                            if (!SetFamily.GirFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(Commons.GirText);
                    break;
                case 2:
                    if (!flg)
                    {
                        SetFamily.SetGirFamilyName();
                        SetFamily.SetBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < Commons.BeamText.Length; i++)
                    {
                        for (int j = j = 0; j < Commons.BeamText[i].Length; j++)
                        {
                            if (SetFamily.BeamFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.BeamFName.FamilyName[i][j], SetFamily.BeamFName.flg[i][j]);
                            ChBeam_name[i][j] = SetFamily.BeamFName.FamilyName[i][j];
                            if (!SetFamily.BeamFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(Commons.BeamText);
                    break;
                case 3:
                    if (!flg)
                    {
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < Commons.CGirText.Length; i++)
                    {
                        for (int j = j = 0; j < Commons.CGirText[i].Length; j++)
                        {
                            if (SetFamily.CGirFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.CGirFName.FamilyName[i][j], SetFamily.CGirFName.flg[i][j]);
                            ChCGir_name[i][j] = SetFamily.CGirFName.FamilyName[i][j];
                            if (!SetFamily.CGirFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(Commons.CGirText);
                    break;
                case 4:
                    if (!flg)
                    {
                        SetFamily.SetCGirFamilyName();
                        SetFamily.SetCBeamFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < Commons.CBeamText.Length; i++)
                    {
                        for (int j = j = 0; j < Commons.CBeamText[i].Length; j++)
                        {
                            if (SetFamily.CBeamFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.CBeamFName.FamilyName[i][j], SetFamily.CBeamFName.flg[i][j]);
                            ChCBeam_name[i][j] = SetFamily.CBeamFName.FamilyName[i][j];
                            if (!SetFamily.CBeamFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(Commons.CBeamText);
                    break;
                case 5:
                    if (!flg)
                    {
                        SetFamily.SetBraFamilyName();
                    }
                    AllControl_Reset();
                    
                    for (int i = 0; i < Commons.SBraText.Length; i++)
                    {
                        for (int j = j = 0; j < Commons.SBraText[i].Length; j++)
                        {
                            if (SetFamily.SBraFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][j], SetFamily.SBraFName.FamilyName[i][j], SetFamily.SBraFName.flg[i][j]);
                            ChSBra_name[i][j] = SetFamily.SBraFName.FamilyName[i][j];
                            if (!SetFamily.SBraFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, j);
                            }
                        }
                    }
                    ControlsSet(Commons.SBraText);
                    break;
                case 6:
                    AllControl_Reset();
                    ControlSave();
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
                    lab[0][0].Text = "Foundation Slab (Mat Foundation)";
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
                    
                    for (int i = 0; i < Commons.FoundationText2.Length; i++)
                    {
                        int jj = 0;
                        for (int j = 0; j < Commons.FoundationText2[i].Length; j++)
                        {
                            if (Commons.FoundationText2[i][j] == "") continue;
                            if (SetFamily.FoFName.flg[i][j]) { allcbfalse = false; }
                            CheckBox_Change(ch[i][jj], SetFamily.FoFName.FamilyName[i][j], SetFamily.FoFName.flg[i][j]);
                            ChFound_name[i][jj] = SetFamily.FoFName.FamilyName[i][j];
                            if (!SetFamily.FoFName.flg[i][j])
                            {
                                ErrLab_and_errbt_Set(i, jj);
                            }
                            jj++;
                        }
                    }
                    ControlsSet(Commons.FoundationText2);
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

            //現在のCheckBoxのONOFFを保存
            if (lab1_1.Text == "RC Column")
            { SaveCheckBox(ChClm); }
            else if (lab1_1.Text == "Foundation Beam")
            { SaveCheckBox(ChGir); }
            else if (lab1_1.Text == "Foundation Sub Beam" )
            { SaveCheckBox(ChBeam); }
            else if (lab1_1.Text == "RC Cantilever Foundation Beam")
            { SaveCheckBox(ChCGir); }
            else if (lab1_1.Text == "RC Cantilever Foundation Sub Beam")
            { SaveCheckBox(ChCBeam); }
            else if (lab1_1.Text == "S Brace H-Shaped Steel")
            { SaveCheckBox(ChSBra); }
            else if (lab1_1.Text == "RC Slab, Deck Plate")
            { SaveCheckBox(ChSlab); }
            else if (lab1_1.Text == "Wall, RC Parapet")
            { SaveCheckBox(ChWall); }
            else if (lab1_1.Text == "Foundation Slab (Mat Foundation)")
            { SaveCheckBox(ChFSlab); }
            else if (lab1_1.Text == "Rectangular Footing")
            { SaveCheckBox(ChFound); }
        }

        private void ControlsSet(string[][] str)
        {
            ControlSave();

            //コントロールの追加・表示設定
            for (int i = 0; i < str.Length; i++)
            {
                int jj = 0;
                for (int j = 0; j < str[i].Length; j++)
                {
                    if (str[i][j] == "") continue;

                    ch[i][jj].Visible = true;
                    lab[i][jj].Visible = true;

                    lab[i][jj].Text = str[i][j];

                    groupBox1.Controls.Add(ch[i][jj]);
                    groupBox1.Controls.Add(lab[i][jj]);

                    jj++;
                }
            }
            
            //新たに表示するCheckBoxのONOFFを設定
            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < str[i].Length; j++)
                {
                    if (lab1_1.Text == "RC Column")
                    { ch[i][j].Checked = ChClm[i][j]; }
                    //else if (lab1_1.Text == "RC基礎柱")
                    //{ ch[i][j].Checked = ChBClm[i][j]; }
                    else if (lab1_1.Text == "Foundation Beam")
                    { ch[i][j].Checked = ChGir[i][j]; }
                    else if (lab1_1.Text == "Foundation Sub Beam")
                    { ch[i][j].Checked = ChBeam[i][j]; }
                    else if (lab1_1.Text == "RC Cantilever Foundation Beam")
                    { ch[i][j].Checked = ChCGir[i][j]; }
                    else if (lab1_1.Text == "RC Cantilever Foundation Sub Beam")
                    { ch[i][j].Checked = ChCBeam[i][j]; }
                    else if (lab1_1.Text == "S Brace H-Shaped Steel")
                    { ch[i][j].Checked = ChSBra[i][j]; }
                    else if (lab1_1.Text == "RC Slab, Deck Plate")
                    { ch[i][j].Checked = ChSlab[i][j]; }
                    else if (lab1_1.Text == "Wall, RC Parapet")
                    { ch[i][j].Checked = ChWall[i][j]; }
                    else if (lab1_1.Text == "Foundation Slab (Mat Foundation)")
                    { ch[i][j].Checked = ChFSlab[i][j]; }
                    else if (lab1_1.Text == "Rectangular Footing")
                    { ch[i][j].Checked = ChFound[i][j]; }                    
                }
            }
            
        }

        /// <summary>CheckBoxのONOFFを保存
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
       

        /// <summary>キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, EventArgs e)
        {
            ControlSave();
            if (!ConvFlg())
            {
                string mes = "No member types selected for parameter addition.";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (!System.IO.File.Exists(Commons.RexJPath(Commons.REXStructual)))
            {
                string mes = "Shared parameters file not found.";
                MessageBox.Show(mes + "\r\n\r\n" + Commons.RexJPath(Commons.REXStructual), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tuika = "Start batch parameter addition?";
            DialogResult dr = MessageBox.Show(tuika, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            ControlSave();
            this.Enabled = false;
            int listboxind = listBox1.SelectedIndex;

            //プログレスバーフォームの準備
            ProgressBarForm pform = new ProgressBarForm();
            Stopwatch stopw = new Stopwatch();            
            stopw.Start();
            bool pformflg = false;
            string logfamily = "";
            //柱
            try
            {
                int allchclm = 0; //追加するファミリの数
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
                    logfamily = "Column / Partition / Foundation Column";

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 0;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数

                    for (int el = 0; el < clmnum; el++)
                    {
                        bool endflg = false; //パラメータセット終了フラグ   

                        if (!(clmel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

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
                                //プログレスバーの表示
                                if (!pformflg)
                                { Pform_Show(pform, ref pformflg); }
                                
                                endnum++;
                                ProgressBar_Show(pform, "Adding Parameters — Column / Partition / Foundation Column " + endnum.ToString() + "/" + allchclm.ToString());
                                Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchclm * 100));
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
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + logfamily + ".");
            }

            //パラメータを既に追加したファミリ
            List<Autodesk.Revit.DB.FamilySymbol> AddEndFamilySymbol = new List<Autodesk.Revit.DB.FamilySymbol>();
            //梁
            try
            {
                Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                Autodesk.Revit.DB.ElementFilter filterGir = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming);
                IList<Autodesk.Revit.DB.Element> girel = collector.WherePasses(filterGir).WhereElementIsElementType().ToElements();
                int num = girel.Count();

                int allchgir = 0; //追加するファミリの数
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
                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 1;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Main Beam";
                        if (!(girel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }
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
                                if (!ChGir[i][j]) { continue; } //チェックボックスがtrueのものだけパラメータを追加する
                                if (symbol.FamilyName != SetFamily.GirFName.FamilyName[i][j]) { continue; }

                                logfamily = symbol.FamilyName;
                                //プログレスバーの表示 
                                if (!pformflg)
                                { Pform_Show(pform, ref pformflg); }

                                endnum++;
                                ProgressBar_Show(pform, "Adding Parameters — Main Beam " + endnum.ToString() + "/" + allchgir.ToString());
                                Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchgir * 100));

                                Set_Girder_Parameter(symbol, SetFamily.GirFName.FamilyName[i][j]);
                                AddEndFamilySymbol.Add(symbol);                               
                                break;

                            }
                        }
                    }
                }

                int allchbeam = 0; //追加するファミリの数
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
                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 2;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Sub Beam";
                        if (!(girel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

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
                                if (!ChBeam[i][j]) { continue; } //チェックボックスがtrueのものだけパラメータを追加する
                                if (symbol.FamilyName != SetFamily.BeamFName.FamilyName[i][j]) { continue; }

                                logfamily = symbol.FamilyName;
                                //プログレスバーの表示 
                                if (!pformflg)
                                { Pform_Show(pform, ref pformflg); }

                                endnum++;
                                ProgressBar_Show(pform, "Adding Parameters — Sub Beam " + endnum.ToString() + "/" + allchbeam.ToString());
                                Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchbeam * 100));

                                Set_Beam_Parameter(symbol, SetFamily.BeamFName.FamilyName[i][j]);
                                AddEndFamilySymbol.Add(symbol);                               
                                break;
                            }
                        }
                    }
                }
                AddEndFamilySymbol = new List<Autodesk.Revit.DB.FamilySymbol>();
                int allchcgir = 0; //追加するファミリの数
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
                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 3;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Cantilever Beam";
                        if (!(girel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

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
                                if (ChCGir[i][j] == true) //チェックボックスがtrueのものだけパラメータを追加する
                                {
                                    if (symbol.FamilyName != SetFamily.CGirFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    //プログレスバーの表示 
                                    if (!pformflg)
                                    { Pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding Parameters — Cantilever Beam " + endnum.ToString() + "/" + allchcgir.ToString());
                                    Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchcgir * 100));

                                    Set_CGirder_Parameter(symbol, SetFamily.CGirFName.FamilyName[i][j]);
                                    AddEndFamilySymbol.Add(symbol);                                    
                                    break;
                                }
                            }
                        }
                    }
                }

                //追加する部材の画面にする
                listBox1.SelectedIndex = 4;
                this.Refresh();
                int allchcbeam = 0; //追加するファミリの数
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
                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 4;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数
                    for (int el = 0; el < num; el++)
                    {
                        logfamily = "Cantilever Sub Beam";
                        if (!(girel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

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
                                if (ChCBeam[i][j] == true) //チェックボックスがtrueのものだけパラメータを追加する
                                {
                                    if (symbol.FamilyName != SetFamily.CBeamFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    //プログレスバーの表示 
                                    if (!pformflg)
                                    { Pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding Parameters — Cantilever Sub Beam " + endnum.ToString() + "/" + allchcbeam.ToString());
                                    Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchcbeam * 100));

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
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + logfamily + ".");
            }
            //ブレース
            try
            {
                int allchsbra = 0; //追加するファミリの数
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
                    logfamily = "S Brace";
                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filterGir = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming);
                    IList<Autodesk.Revit.DB.Element> brael = collector.WherePasses(filterGir).WhereElementIsElementType().ToElements();
                    int num = brael.Count();

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 5;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数
                    for (int el = 0; el < num; el++)
                    {
                        bool endflg = false; //パラメータセット終了フラグ
                        if (!(brael[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

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
                                if (ChSBra[i][j] == true) //チェックボックスがtrueのものだけパラメータを追加する
                                {
                                    if (symbol.FamilyName != SetFamily.SBraFName.FamilyName[i][j]) { continue; }

                                    logfamily = symbol.FamilyName;
                                    //プログレスバーの表示
                                    if (!pformflg)
                                    { Pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding Parameters — S Brace " + endnum.ToString() + "/" + allchsbra.ToString());
                                    Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchsbra * 100));

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
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + logfamily + ".");
            }
            //床
            try
            {
                if (ChSlab[0][0]) //基礎スラブ・RCスラブ・デッキプレートスラブは同じ構造床ファミリを使用する
                {
                    logfamily = "Slab & Deck Plate";

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 6;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_Floors);
                    IList<Autodesk.Revit.DB.Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = elms.Count();
                    foreach (Autodesk.Revit.DB.Element el in elms)
                    {
                        if (el is Autodesk.Revit.DB.FloorType symbol && symbol.IsFoundationSlab == false)
                        {
                            logfamily = symbol.FamilyName;
                            //プログレスバーの表示
                            if (!pformflg)
                            { Pform_Show(pform, ref pformflg); }
                            ProgressBar_Show(pform, "Adding Parameters — Slab & Deck Plate");
                            Commons.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                            ParaSet.SetPara_Slab("Floor", el, SetFamily.Slab);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to structural floor family.");
            }
            //壁
            try
            {
                if (ChWall[0][0])
                {
                    logfamily = "Wall & RC Parapet";

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 7;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_Walls);
                    IList<Autodesk.Revit.DB.Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = elms.Count();
                    foreach (Autodesk.Revit.DB.Element el in elms)
                    {
                        if (el is Autodesk.Revit.DB.WallType symbol && symbol.Kind == Autodesk.Revit.DB.WallKind.Basic)
                        {
                            logfamily = symbol.FamilyName;
                            //プログレスバーの表示
                            if (!pformflg)
                            { Pform_Show(pform, ref pformflg); }
                            ProgressBar_Show(pform, "Adding Parameters — Wall & RC Parapet");
                            Commons.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                            ParaSet.SetPara_Wall("Wall", symbol, SetFamily.Wall);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to structural wall family.");
            }

            //基礎
            try
            {
                int allchf = 0; //追加するファミリの数
                for (int i = 0; i < ChFound.Length; i++)
                {
                    for (int j = 0; j < ChFound[i].Length; j++)
                    {
                        if (ChFound[i][j] && ChFound_name[i][j] != "")
                        {
                            allchf++;
                        }
                    }
                }
                if (allchf != 0)
                {
                    logfamily = "Footing / Strip Footing / Piles";
                    Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation);
                    IList<Autodesk.Revit.DB.Element> fel = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                    int num = fel.Count();

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 8;
                    this.Refresh();

                    int endnum = 0; //追加が終わったファミリの数

                    for (int el = 0; el < num; el++)
                    {
                        bool endflg = false; //パラメータセット終了フラグ
                        if (!(fel[el] is Autodesk.Revit.DB.FamilySymbol symbol)) { continue; }

                        for (int i = 0; i < ChFound.Length; i++)
                        {
                            if (endflg) { break; }
                            for (int j = 0; j < ChFound[i].Length; j++)
                            {
                                if (ChFound[i][j] == true) //チェックボックスがtrueのものだけパラメータを追加する
                                {
                                    if (symbol.FamilyName != ChFound_name[i][j]) { continue; }
                                    logfamily = symbol.FamilyName;
                                    //プログレスバーの表示
                                    if (!pformflg)
                                    { Pform_Show(pform, ref pformflg); }

                                    endnum++;
                                    ProgressBar_Show(pform, "Adding Parameters — Footing / Strip Footing / Piles " + endnum.ToString() + "/" + allchf.ToString());
                                    Commons.GaugePercent(symbol.FamilyName, (int)((double)endnum / (double)allchf * 100));

                                    Set_SFooting_Parameter(symbol, ChFound_name[i][j]);
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
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + logfamily + ".");
            }
            //基礎スラブ
            try
            {
                if (ChFSlab[0][0])
                {
                    logfamily = "Foundation Slab (Mat Foundation)";

                    //追加する部材の画面にする
                    listBox1.SelectedIndex = 9;
                    this.Refresh();

                    Autodesk.Revit.DB.FilteredElementCollector collector2 = new Autodesk.Revit.DB.FilteredElementCollector(Commons.doc);
                    Autodesk.Revit.DB.ElementFilter filter2 = new Autodesk.Revit.DB.ElementCategoryFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation);
                    IList<Autodesk.Revit.DB.Element> el2 = collector2.WherePasses(filter2).WhereElementIsElementType().ToElements();
                    int num2 = el2.Count();
                    for (int i = 0; i < num2; i++)
                    {
                        if (!(el2[i] is Autodesk.Revit.DB.FloorType symbol)) { continue; }
                        if (symbol.FamilyName != "Mat Foundation") { continue; }
                        logfamily = symbol.FamilyName;
                        //プログレスバーの表示
                        if (!pformflg)
                        { Pform_Show(pform, ref pformflg); }
                        ProgressBar_Show(pform, "Adding Parameters — Foundation Slab (Mat Foundation)");
                        Commons.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                        ParaSet.SetPara_Slab("Structural Foundation", symbol, SetFamily.Slab);
                        break;
                    }
                }
            }
            catch
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to mat foundation family.");
            }

            // 進捗ゲージの消去
            if (this != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                Commons.GaugeClose();
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
                string mes = "Batch parameter addition completed.";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //ログ出力
                if (LogData.Data.Count() != 0)
                {
                    LogForm lf = new LogForm
                    {
                        Text = Commons.SystemName + " Batch Add Parameters Log " + Commons.GetVersion()
                    };
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
        private void Pform_Show(ProgressBarForm pform, ref bool pformflg)
        {
            pform.Text = Commons.SystemName + " Adding Parameters";
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
            LinkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox f = new AboutBox();
            f.ShowDialog();
            f.Dispose();
        }
        private void Button1_Click(object sender, EventArgs e)
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
        private void Button2_Click(object sender, EventArgs e)
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

        #region パラメータのセット
        internal bool Set_Column_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                //柱ファミリのパラメータセット
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする          
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch(Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_Girder_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                //大梁ファミリのパラメータセット
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_Beam_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                //小梁ファミリのパラメータセット

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_CGirder_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                //片持ち梁ファミリのパラメータセット
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);


            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_CBeam_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
            
                //片持ち梁ファミリのパラメータセット
                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);


            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_SBrace_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                //ブレースファミリのパラメータセット

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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

                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        private bool Set_SFooting_Parameter(Autodesk.Revit.DB.Element element, string FamilyName)
        {
            bool ret = true;
           
                //基礎ファミリのパラメータセット

                Autodesk.Revit.DB.FamilySymbol fams = element as Autodesk.Revit.DB.FamilySymbol;
                Autodesk.Revit.DB.Family fam = fams.Family;
                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(fam);

                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, FamilyName + " Add Parameters");
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
                if (FamilyName == SetFamily.Pile_S.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_S);
                }
                if (FamilyName == SetFamily.Pile_PHC.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PHC);
                }
                if (FamilyName == SetFamily.Pile_ST.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_ST);
                }
                if (FamilyName == SetFamily.Pile_SC.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_SC);
                }
                if (FamilyName == SetFamily.Pile_PRC.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PRC);
                }
                if (FamilyName == SetFamily.Pile_CPRC.FamilyName)
                {
                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_CPRC);
                }


                //プロジェクトにパラメータを追加したファミリをロードする
                FamilyOption famop = new FamilyOption();
                fam = doc.LoadFamily(Commons.doc, famop);
                tran1.Commit();
                doc.Close(false);
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to add parameters to " + FamilyName + ".");
                tran1.RollBack();
                ret = false;
            }
            return ret;
        }
        #endregion

        private void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            Commons.gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            Commons.GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            Commons.GaugeShow();
            pform.Refresh();
        }

     

        private void Chb_CheckedChanged(object sender, EventArgs e)
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
