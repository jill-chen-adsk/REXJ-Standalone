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
using System.Reflection;
using System.IO;
using System.Runtime.Versioning ;
using Autodesk.Revit.DB;

namespace MappingTable
{
    public partial class AddParameterForm : System.Windows.Forms.Form
    {
        private BuiltInCategory mode = BuiltInCategory.INVALID;

        //private string[] parameterSet = new string[]
        //{   //↓SelectedIndex
        //    /* 0*/ "RC Column - Parameters",
        //    /* 1*/ "RC Circular Column - Parameters",
        //    /* 2*/ "S Column H-Section - Parameters",
        //    /* 3*/ "S Column Built-up H - Parameters",
        //    /* 4*/ "S Column Box Section - Parameters",
        //    /* 5*/ "S Column Built-up Box - Parameters",
        //    /* 6*/ "S Column Pipe - Parameters",
        //    /* 7*/ "S Column T-Section - Parameters",
        //    /* 8*/ "S Column Channel - Parameters",
        //    /* 9*/ "S Column Angle - Parameters",
        //    /*10*/ "SRC Column H-shape (Rectangular) - Parameters",
        //    /*11*/ "SRC Column Cross (Rectangular) - Parameters",
        //    /*12*/ "SRC Column T-shape (Rectangular) - Parameters",
        //    /*13*/ "SRC Column H-shape (Circular) - Parameters",
        //    /*14*/ "SRC Column Cross (Circular) - Parameters",
        //    /*15*/ "SRC Column T-shape (Circular) - Parameters",
        //    /*16*/ "CFT Column Box - Parameters",
        //    /*17*/ "CFT Column Pipe - Parameters",

        //    /*18*/ "RC Beam - Parameters",
        //    /*19*/ "RC Cantilever Beam - Parameters",
        //    /*20*/ "S Beam - Brace - H-Section - Parameters",
        //    /*21*/ "S Beam - Brace - Built-up H - Parameters",
        //    /*22*/ "S Beam - Brace - Channel - Parameters",
        //    /*23*/ "S Beam - Brace - Lip Channel - Parameters",
        //    /*24*/ "S Beam - Brace - Angle - Parameters",
        //    /*25*/ "S Cantilever Beam - Parameters",
        //    /*26*/ "SRC Beam - Parameters",
        //    /*27*/ "SRC Cantilever Beam - Parameters",

        //    /*28*/ "Brace Box Section - Parameters",
        //    /*29*/ "Brace Built-up Box - Parameters",
        //    /*30*/ "Brace Circular Pipe - Parameters",
        //    /*31*/ "Brace Round Bar - Parameters",
        //    /*32*/ "Brace Flat Bar - Parameters",

        //    /*33*/ "RC Rect. Footing - Parameters",
        //    /*34*/ "RC Tapered Rect. Footing - Parameters",
        //    /*35*/ "RC Triangle Footing - Parameters",
        //    /*36*/ "RC Equilateral Triangle Footing - Parameters",
        //    /*37*/ "RC Octagonal Footing - Parameters",
        //    /*38*/ "Strip Footing - Parameters",

        //    /*39*/ "Cast-in-place Pile - Parameters",
        //    /*40*/ "Precast Pile - Parameters",

        //    //"Floor - Parameters",
        //    //"Wall - Parameters",

        //};
        private string[] parameterSet_Column = new string[]
       {   //↓SelectedIndex
            /* 0*/ "RC Column - Parameters",
            /* 1*/ "RC Circular Column - Parameters",
            /* 2*/ "S Column H-Section - Parameters",
            /* 3*/ "S Column Built-up H - Parameters",
            /* 4*/ "S Column Box Section - Parameters",
            /* 5*/ "S Column Built-up Box - Parameters",
            /* 6*/ "S Column Pipe - Parameters",
            /* 7*/ "S Column T-Section - Parameters",
            /* 8*/ "S Column Channel - Parameters",
            /* 9*/ "S Column Angle - Parameters",
            /*10*/ "SRC Column H-shape (Rectangular) - Parameters",
            /*11*/ "SRC Column Cross (Rectangular) - Parameters",
            /*12*/ "SRC Column T-shape (Rectangular) - Parameters",
            /*13*/ "SRC Column H-shape (Circular) - Parameters",
            /*14*/ "SRC Column Cross (Circular) - Parameters",
            /*15*/ "SRC Column T-shape (Circular) - Parameters",
            /*16*/ "CFT Column Box - Parameters",
            /*17*/ "CFT Column Pipe - Parameters"
       };
        private string[] parameterSet_Frame = new string[]
       {   //↓SelectedIndex
           
            /*0*/ "RC Beam - Parameters",
            /*1*/ "RC Cantilever Beam - Parameters",
            /*2*/ "S Beam - Brace - H-Section - Parameters",
            /*3*/ "S Beam - Brace - Built-up H - Parameters",
            /*4*/ "S Beam - Brace - Channel - Parameters",
            /*5*/ "S Beam - Brace - Lip Channel - Parameters",
            /*6*/ "S Beam - Brace - Angle - Parameters",
            /*7*/ "S Cantilever Beam - Parameters",
            /*8*/ "SRC Beam - Parameters",
            /*9*/ "SRC Cantilever Beam - Parameters",

            /*10*/ "Brace Box Section - Parameters",
            /*11*/ "Brace Built-up Box - Parameters",
            /*12*/ "Brace Circular Pipe - Parameters",
            /*13*/ "Brace Round Bar - Parameters",
            /*14*/ "Brace Flat Bar - Parameters"
           
       };
        private string[] parameterSet_Foundation = new string[]
       {   //↓SelectedIndex
           
            /*0*/ "RC Rect. Footing - Parameters",
            /*1*/ "RC Tapered Rect. Footing - Parameters",
            /*2*/ "RC Triangle Footing - Parameters",
            /*3*/ "RC Equilateral Triangle Footing - Parameters",
            /*4*/ "RC Octagonal Footing - Parameters",
            /*5*/ "Strip Footing - Parameters",

            /*6*/ "Cast-in-place Pile - Parameters",
            /*7*/ "Precast Pile - Parameters"
       };


        private List<Family> targetFamily = new List<Family>();

        private string buzai = ""; //構造柱などの部材名

        public AddParameterForm(BuiltInCategory m)
        {
            InitializeComponent();

            mode = m;
        }

        private void AddParameterForm_Load(object sender, EventArgs e)
        {
            targetFamily = new List<Family>();
            

            //タイトルバー
            this.Text = Commons.SystemName +  " ";
            switch (mode)
            {
                case BuiltInCategory.OST_StructuralColumns:    this.Text += "Structural Column";  buzai = "Structural Column";   break;
                case BuiltInCategory.OST_StructuralFraming:    this.Text += "Structural Framing"; buzai = "Structural Framing"; break;
                //case BuiltInCategory.OST_Floors:               this.Text += "構造床";       break;
                //case BuiltInCategory.OST_Walls:                this.Text += "構造壁";       break;
                case BuiltInCategory.OST_StructuralFoundation: this.Text += "Structural Foundation"; buzai = "Structural Foundation"; break;
            }
            this.Text += " Add Parameters";

            

            this.Text += " " + Commons.GetVersion();


            bool enabled = true;
            switch (mode)
            {
                case BuiltInCategory.OST_Floors:
                case BuiltInCategory.OST_Walls:
                    //switch (mode)
                    //{
                    //    case BuiltInCategory.OST_Floors:
                    //        cmbFamily.Items.Add("床");
                    //        break;
                    //    case BuiltInCategory.OST_Walls:
                    //        cmbFamily.Items.Add("標準壁");
                    //        break;
                    //}
                    enabled = false;
                    break;
                default:
                    if (Commons.doc.IsFamilyDocument == false)
                    {
                        //プロジェクト：ロードされているファミリを取得
                        FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                        ElementFilter filter = new ElementCategoryFilter(mode);
                        IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                        List<string> familyname = new List<string>();
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (!(elements[i] is FamilySymbol symbol)) continue;

                            if (familyname.Contains(symbol.Family.Name) == false)
                            {
                                familyname.Add(symbol.Family.Name);
                                targetFamily.Add(symbol.Family);
                            }
                        }
                        familyname.Sort();
                        cmbFamily.Items.AddRange(familyname.ToArray());
                        if (familyname.Count > 0)
                        {
                            cmbFamily.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        //ファミリドキュメント：このファミリのみ
                        cmbFamily.Items.Add(Path.GetFileNameWithoutExtension(Commons.doc.PathName));
                        enabled = false;
                    }

                    break;
            }

            if (enabled == false)
            {
                cmbFamily.SelectedIndex = 0;
                cmbFamily.Enabled = false;
                cmbFamily.Width = cmbParameter.Width;
                btnLoad.Visible = false;
            }

            //cmbParameter.Items.AddRange(parameterSet);
            switch (mode)
            {
                case BuiltInCategory.OST_StructuralColumns:
                    cmbParameter.Items.AddRange(parameterSet_Column);
                    cmbParameter.SelectedItem = "RC Column - Parameters";
                    break;
                case BuiltInCategory.OST_StructuralFraming:
                    cmbParameter.Items.AddRange(parameterSet_Frame);
                    cmbParameter.SelectedItem = "RC Beam - Parameters";                    
                    break;
                case BuiltInCategory.OST_Floors: cmbParameter.SelectedItem = "Floor - Parameters"; break;
                case BuiltInCategory.OST_Walls: cmbParameter.SelectedItem = "Wall - Parameters"; break;
                case BuiltInCategory.OST_StructuralFoundation:
                    if (SetFamily.IsSTB2table)
                    {
                        parameterSet_Foundation = new string[]
                        {   //↓SelectedIndex
           
                            /*0*/ "RC Rect. Footing - Parameters",
                            /*1*/ "RC Tapered Rect. Footing - Parameters",
                            /*2*/ "RC Triangle Footing - Parameters",
                            /*3*/ "RC Equilateral Triangle Footing - Parameters",
                            /*4*/ "RC Octagonal Footing - Parameters",
                            /*5*/ "Strip Footing - Parameters",

                            /* 6*/ "RC Pile - Parameters",
                            /* 7*/ "Steel Pipe Pile - Parameters",
                            /* 8*/ "Precast Pile PHC - Parameters",
                            /* 9*/ "Precast Pile ST - Parameters",
                            /*10*/ "Precast Pile SC - Parameters",
                            /*11*/ "Precast Pile PRC - Parameters",
                            /*12*/ "Precast Pile CPRC - Parameters",
                        };
                    }
                    else
                    {
                        parameterSet_Foundation = new string[]
                        {   //↓SelectedIndex
           
                            /*0*/ "RC Rect. Footing - Parameters",
                            /*1*/ "RC Tapered Rect. Footing - Parameters",
                            /*2*/ "RC Triangle Footing - Parameters",
                            /*3*/ "RC Equilateral Triangle Footing - Parameters",
                            /*4*/ "RC Octagonal Footing - Parameters",
                            /*5*/ "Strip Footing - Parameters",

                            /*6*/ "Cast-in-place Pile - Parameters",
                            /*7*/ "Precast Pile - Parameters"
                        };
                    }
                    cmbParameter.Items.AddRange(parameterSet_Foundation);
                    cmbParameter.SelectedItem = "RC Rect. Footing - Parameters";
                    break;
            }

        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = buzai + " Open Family";
            string name = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FamilyOption famop = new FamilyOption();
                name = Path.GetFileNameWithoutExtension(openFileDialog1.FileNames[0]);
                Transaction tran = new Transaction(Commons.doc, name + " - Loading");
                
                try
                {
                    tran.Start();
                    for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                    {
                        bool nextflg = false;
                        if (File.Exists(openFileDialog1.FileNames[i]) == true) //念のため
                        {
                            //ファミリのロード
                            name = Path.GetFileNameWithoutExtension(openFileDialog1.FileNames[i]);
                            for (int c = 0; c < cmbFamily.Items.Count; c++)
                            {
                                if(cmbFamily.Items[c].ToString() == name)
                                {
                                    nextflg = true;
                                    break;
                                }
                            }
                            if (nextflg)
                            {
                                string mes = name + " is already loaded.";
                                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //追加したファミリを選択
                                cmbFamily.SelectedItem = name;
                                continue;
                            }
                            if (Commons.doc.LoadFamily(openFileDialog1.FileNames[i], famop, out Family family))
                            {
#if REVIT2022 || REVIT2023
    var value = Commons.doc.OwnerFamily.FamilyCategoryId.IntegerValue ;
    #else
    var value = family.FamilyCategoryId.Value ;
#endif
                                
                                if (value == (long)mode)
                                {
                                    //ロード成功して、カテゴリが一致していればComboに追加
                                    if (cmbFamily.Items.Contains(family.Name) == false)
                                    {
                                        cmbFamily.Items.Add(family.Name);
                                        targetFamily.Add(family);
                                        //追加したファミリを選択
                                        cmbFamily.SelectedItem = family.Name;
                                        tran.Commit();
                                        tran.Start();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(family.Name + " failed to load.\r\n" + 
                                                    "Please verify that the family category is [" + buzai + "].", 
                                                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    tran.RollBack();
                                    tran.Start();
                                }
                            }
                            else
                            {
                                string mes = name + " is already loaded.";
                                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //追加したファミリを選択
                                cmbFamily.SelectedItem = name;
                                continue;
                            }
                        }
                    }
                    tran.Commit();
                }
                catch(Exception)
                {
                    MessageBox.Show(name + " failed to load.\r\n" +
                                    "Please verify that the family category is [" + buzai + "].",
                                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tran.RollBack();
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string sharedParamsFile = Commons.RexJPath(Commons.REXStructual);
            if (!File.Exists(sharedParamsFile))
            {
                string mes = "Shared parameters file not found.";
                MessageBox.Show(mes + "\r\n\r\n" + sharedParamsFile, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string familyname = cmbFamily.SelectedItem.ToString();
            string paraname = cmbParameter.SelectedItem.ToString();

            Transaction tran = null;

            ProgressBarForm pform = new ProgressBarForm();
            bool pfflg = false;
            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            try
            {
                string buzaimei = cmbParameter.SelectedItem.ToString().Replace(" - Parameters", "Parameters");
                FamilyManager fmg = null;
                Document d = null;
                if (Commons.doc.IsFamilyDocument == false)
                {
                    for(int i = 0; i < targetFamily.Count(); i++)
                    {
                        if(targetFamily[i].Name == cmbFamily.SelectedItem.ToString())
                        {
                            d = Commons.doc.EditFamily(targetFamily[i]);
                            break;
                        }
                    }
                    if (d != null)
                    {
                        fmg = d.FamilyManager;
                        tran = new Transaction(d, buzaimei + "Add");
                    }
                }
                else
                {
                    fmg = Commons.doc.FamilyManager;
                    tran = new Transaction(Commons.doc, buzaimei + "Add");
                }
                if (tran == null) { return; }
                tran.Start();

                //ステータスバーの表示
                Pform_Show(pform, ref pfflg, Commons.SystemName + " Adding Parameters");
                ProgressBar_Show(pform, paraname + " - Adding");
                Commons.GaugePercent(familyname, (int)((double)1 / (double)1 * 100));

                if (this.Text.Contains("Structural Column"))
                {
                    switch (cmbParameter.SelectedIndex)
                    {
                        case 0: ParaSet.SetPara_RCClmRe(fmg, SetFamily.RCClmRe); break;
                        case 1: ParaSet.SetPara_RCClmRo(fmg, SetFamily.RCClmRo); break;
                        case 2: ParaSet.SetPara_SClmH(fmg, SetFamily.SClmH); break;
                        case 3: ParaSet.SetPara_SClmBH(fmg, SetFamily.SClmBH); break;
                        case 4: ParaSet.SetPara_SClmBox(fmg, SetFamily.SClmBox); break;
                        case 5: ParaSet.SetPara_SClmBBox(fmg, SetFamily.SClmBBox); break;
                        case 6: ParaSet.SetPara_SClmPipe(fmg, SetFamily.SClmPipe); break;
                        case 7: ParaSet.SetPara_SClmT(fmg, SetFamily.SClmT); break;
                        case 8: ParaSet.SetPara_SClmC(fmg, SetFamily.SClmC); break;
                        case 9: ParaSet.SetPara_SClmL(fmg, SetFamily.SClmL); break;
                        case 10: ParaSet.SetPara_SRCClmH(fmg, SetFamily.SRCClmH); break;
                        case 11: ParaSet.SetPara_SRCClmCross(fmg, SetFamily.SRCClmCross); break;
                        case 12: ParaSet.SetPara_SRCClmT(fmg, SetFamily.SRCClmT); break;
                        case 13: ParaSet.SetPara_SRCClmH_Rou(fmg, SetFamily.SRCClmH_Rou); break;
                        case 14: ParaSet.SetPara_SRCClmCross_Rou(fmg, SetFamily.SRCClmCross_Rou); break;
                        case 15: ParaSet.SetPara_SRCClmT_Rou(fmg, SetFamily.SRCClmT_Rou); break;
                        case 16: ParaSet.SetPara_CFTClmBox(fmg, SetFamily.CFTClmBox); break;
                        case 17: ParaSet.SetPara_CFTClmPipe(fmg, SetFamily.CFTClmPipe); break;

                   
                    }
                }
                else if(this.Text.Contains("Structural Framing"))
                {
                    switch(cmbParameter.SelectedIndex)
                    {
                        case 0: ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir); break;
                        case 1: ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir); break;
                        case 2: ParaSet.SetPara_SGirH(fmg, SetFamily.SGirH); break;
                        case 3: ParaSet.SetPara_SGirBH(fmg, SetFamily.SGirBH); break;
                        case 4: ParaSet.SetPara_SGirC(fmg, SetFamily.SGirC); break;
                        case 5: ParaSet.SetPara_SGirLipC(fmg, SetFamily.SGirLipC); break;
                        case 6: ParaSet.SetPara_SGirL(fmg, SetFamily.SGirL); break;
                        case 7: ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirH); break;
                        case 8: ParaSet.SetPara_SRCGirH(fmg, SetFamily.SRCGirH); break;
                        case 9: ParaSet.SetPara_SRCCGirH(fmg, SetFamily.SRCCGirH); break;

                        case 10: ParaSet.SetPara_SBraBox(fmg, SetFamily.SBraBox); break;
                        case 11: ParaSet.SetPara_SBraBBox(fmg, SetFamily.SBraBBox); break;
                        case 12: ParaSet.SetPara_SBraPipe(fmg, SetFamily.SBraPipe); break;
                        case 13: ParaSet.SetPara_SBraRollBar(fmg, SetFamily.SBraRollBar); break;
                        case 14: ParaSet.SetPara_SBraFB(fmg, SetFamily.SBraFB); break;
                    }
                }
                else if(this.Text.Contains("Structural Foundation"))
                {
                    switch(cmbParameter.SelectedIndex)
                    {
                        case 0: ParaSet.SetPara_Foundation_Rect(fmg, SetFamily.FRect); break;
                        case 1: ParaSet.SetPara_Foundation_Tapered_Rect(fmg, SetFamily.FTRect); break;
                        case 2: ParaSet.SetPara_Foundation_Triangle(fmg, SetFamily.FTri); break;
                        case 3: ParaSet.SetPara_Foundation_ETriangle(fmg, SetFamily.FETriangle); break;
                        case 4: ParaSet.SetPara_Foundation_Octagon(fmg, SetFamily.FOct); break;
                        case 5: ParaSet.SetPara_Foundation_Continuous(fmg, SetFamily.FConti); break;

                        case 6: ParaSet.SetPara_Castinpile(fmg, SetFamily.CastinPile); break;
                        case 7:
                            if (SetFamily.IsSTB2table)
                            {
                                ParaSet.SetPara_Pile(fmg, SetFamily.Pile_S);
                            }
                            else
                            {
                                ParaSet.SetPara_Precastpile(fmg, SetFamily.PrecastPile);
                            }
                            break;
                        case  8: ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PHC); break;
                        case  9: ParaSet.SetPara_Pile(fmg, SetFamily.Pile_ST); break;
                        case 10: ParaSet.SetPara_Pile(fmg, SetFamily.Pile_SC); break;
                        case 11: ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PRC); break;
                        case 12: ParaSet.SetPara_Pile(fmg, SetFamily.Pile_CPRC); break;

                    }
                }

                if (Commons.doc.IsFamilyDocument == false)
                {
                    FamilyOption famop = new FamilyOption();
                    d.LoadFamily(Commons.doc, famop);
                }

                // 進捗ゲージの消去
                if (this != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    Commons.GaugeClose();
                    pform.Close();
                }

                string mes = "Parameters added to " + familyname + ": " + paraname + ".";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                tran.Commit();

                if (Commons.doc.IsFamilyDocument == false)
                {
                    d.Close(false);
                }

            }
            catch(Exception)
            {
                // 進捗ゲージの消去
                if (this != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    Commons.GaugeClose();
                    pform.Close();
                }

                string buzaimei = cmbParameter.Text.Replace("ー", "");
                MessageBox.Show("Failed to add parameters to " + familyname + ": " + paraname + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tran.RollBack();
            }


            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void AddParameterForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            LinkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }


        internal static void Pform_Show(ProgressBarForm pform, ref bool pformflg, string title)
        {
            pform.Text = title;
            pform.Show();
            pform.lab.Visible = true;
            int px = pform.panelFooter.Width + 15;
            int py = pform.lab.Height + pform.panelFooter.Height + 6;
            pform.ClientSize = new Size(px, py);
            pform.lab.Top = 3;
            pform.lab.Left = 3;
            pform.panelFooter.Top = pform.lab.Bottom;
            pform.panelFooter.Left = pform.lab.Left;
            pformflg = true;
        }
        private void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            Commons.gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            Commons.GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            Commons.GaugeShow();
            pform.Refresh();
        }
    }
}
