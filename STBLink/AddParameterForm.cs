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

using Autodesk.Revit.DB;


namespace STBLink
{
    public partial class AddParameterForm : System.Windows.Forms.Form
    {
        private BuiltInCategory mode = BuiltInCategory.INVALID;

        //private string[] parameterSet = new string[]
        //{   //↓SelectedIndex
        //    /* 0*/ "RC Column－Parameter",
        //    /* 1*/ "RC Round Column－Parameter",
        //    /* 2*/ "S Column H-Shaped Steel－Parameter",
        //    /* 3*/ "S Column Built-Up H-Shaped Steel－Parameter",
        //    /* 4*/ "S Column Square Steel Tube－Parameter",
        //    /* 5*/ "S Column Built-Up Square Steel Tube－Parameter",
        //    /* 6*/ "S Column Steel Pipe－Parameter",
        //    /* 7*/ "S Column T-Shaped Steel－Parameter",
        //    /* 8*/ "S Column Channel Steel－Parameter",
        //    /* 9*/ "S Column Angle Steel－Parameter",
        //    /*10*/ "SRC Column H-Shaped (Rectangular)－Parameter",
        //    /*11*/ "SRC Column Cross (+) (Rectangular)－Parameter",
        //    /*12*/ "SRC Column T-Shaped (Rectangular)－Parameter",
        //    /*13*/ "SRC Column H-Shaped (Circular)－Parameter",
        //    /*14*/ "SRC Column Cross (+) (Circular)－Parameter",
        //    /*15*/ "SRC Column T-Shaped (Circular)－Parameter",
        //    /*16*/ "CFT Column Square Steel Tube－Parameter",
        //    /*17*/ "CFT Column Steel Pipe－Parameter",

        //    /*18*/ "RC Girder－Parameter",
        //    /*19*/ "RC Cantilever Girder－Parameter",
        //    /*20*/ "S Girder/Brace H-Shaped Steel－Parameter",
        //    /*21*/ "S Girder/Brace Built-Up H-Shaped Steel－Parameter",
        //    /*22*/ "S Girder/Brace Channel Steel－Parameter",
        //    /*23*/ "S Girder/Brace Lip Channel Steel－Parameter",
        //    /*24*/ "S Girder/Brace Angle Steel－Parameter",
        //    /*25*/ "S Cantilever Girder－Parameter",
        //    /*26*/ "SRC Girder－Parameter",
        //    /*27*/ "SRC Cantilever Girder－Parameter",

        //    /*28*/ "Brace Square Steel Tube－Parameter",
        //    /*29*/ "Brace Built-Up Square Steel Tube－Parameter",
        //    /*30*/ "Brace Round Steel Pipe－Parameter",
        //    /*31*/ "Brace Round Bar－Parameter",
        //    /*32*/ "Brace Flat Bar－Parameter",

        //    /*33*/ "RC Footing Rectangle－Parameter",
        //    /*34*/ "RC Footing Rectangular Tapered－Parameter",
        //    /*35*/ "RC Footing Triangle－Parameter",
        //    /*36*/ "RC Footing Equilateral Triangle－Parameter",
        //    /*37*/ "RC Footing Octagon－Parameter",
        //    /*38*/ "Spread Footing－Parameter",

        //    /*39*/ "Cast-In-Place Pile－Parameter",
        //    /*40*/ "Precast Pile－Parameter",

        //    //"Structural Floor－Parameter",
        //    //"Wall－Parameter",

        //};
        private string[] parameterSet_Column = new string[]
       {   //↓SelectedIndex
            /* 0*/ "RC Column－Parameter",
            /* 1*/ "RC Round Column－Parameter",
            /* 2*/ "S Column H-Shaped Steel－Parameter",
            /* 3*/ "S Column Built-Up H-Shaped Steel－Parameter",
            /* 4*/ "S Column Square Steel Tube－Parameter",
            /* 5*/ "S Column Built-Up Square Steel Tube－Parameter",
            /* 6*/ "S Column Steel Pipe－Parameter",
            /* 7*/ "S Column T-Shaped Steel－Parameter",
            /* 8*/ "S Column Channel Steel－Parameter",
            /* 9*/ "S Column Angle Steel－Parameter",
            /*10*/ "SRC Column H-Shaped (Rectangular)－Parameter",
            /*11*/ "SRC Column Cross (+) (Rectangular)－Parameter",
            /*12*/ "SRC Column T-Shaped (Rectangular)－Parameter",
            /*13*/ "SRC Column H-Shaped (Circular)－Parameter",
            /*14*/ "SRC Column Cross (+) (Circular)－Parameter",
            /*15*/ "SRC Column T-Shaped (Circular)－Parameter",
            /*16*/ "CFT Column Square Steel Tube－Parameter",
            /*17*/ "CFT Column Steel Pipe－Parameter"
       };
        private string[] parameterSet_Frame = new string[]
       {   //↓SelectedIndex
           
            /*0*/ "RC Girder－Parameter",
            /*1*/ "RC Cantilever Girder－Parameter",
            /*2*/ "S Girder/Brace H-Shaped Steel－Parameter",
            /*3*/ "S Girder/Brace Built-Up H-Shaped Steel－Parameter",
            /*4*/ "S Girder/Brace Channel Steel－Parameter",
            /*5*/ "S Girder/Brace Lip Channel Steel－Parameter",
            /*6*/ "S Girder/Brace Angle Steel－Parameter",
            /*7*/ "S Cantilever Girder－Parameter",
            /*8*/ "SRC Girder－Parameter",
            /*9*/ "SRC Cantilever Girder－Parameter",

            /*10*/ "Brace Square Steel Tube－Parameter",
            /*11*/ "Brace Built-Up Square Steel Tube－Parameter",
            /*12*/ "Brace Round Steel Pipe－Parameter",
            /*13*/ "Brace Round Bar－Parameter",
            /*14*/ "Brace Flat Bar－Parameter"
           
       };
        private string[] parameterSet_Foundation = new string[]
       {   //↓SelectedIndex
           
            /*0*/ "RC Footing Rectangle－Parameter",
            /*1*/ "RC Footing Rectangular Tapered－Parameter",
            /*2*/ "RC Footing Triangle－Parameter",
            /*3*/ "RC Footing Equilateral Triangle－Parameter",
            /*4*/ "RC Footing Octagon－Parameter",
            /*5*/ "Spread Footing－Parameter",

            /*6*/ "Cast-In-Place Pile－Parameter",
            /*7*/ "Precast Pile－Parameter"
       };


        private List<Family> targetFamily = new List<Family>();

        private string buzai = ""; // Member category name such as Structural Column

        public AddParameterForm(BuiltInCategory m)
        {
            InitializeComponent();

            mode = m;
        }

        private void AddParameterForm_Load(object sender, EventArgs e)
        {
            targetFamily = new List<Family>();
            

            // Title bar
            this.Text = RevitLNK.formtitle +  " ";
            switch (mode)
            {
                case BuiltInCategory.OST_StructuralColumns:    this.Text += "Structural Column";  buzai = "Structural Column";   break;
                case BuiltInCategory.OST_StructuralFraming:    this.Text += "Structural Framing"; buzai = "Structural Framing"; break;
                //case BuiltInCategory.OST_Floors:               this.Text += "Structural Floor/Slab";       break;
                //case BuiltInCategory.OST_Walls:                this.Text += "Structural Wall";       break;
                case BuiltInCategory.OST_StructuralFoundation: this.Text += "Structural Foundation"; buzai = "Structural Foundation"; break;
            }
            this.Text += " Add Parameter";



            // this.Text += " " + Commons.GetVersion();


            bool enabled = true;
            switch (mode)
            {
                case BuiltInCategory.OST_Floors:
                case BuiltInCategory.OST_Walls:
                    //switch (mode)
                    //{
                    //    case BuiltInCategory.OST_Floors:
                    //        cmbFamily.Items.Add("Floor");
                    //        break;
                    //    case BuiltInCategory.OST_Walls:
                    //        cmbFamily.Items.Add("Basic Wall");
                    //        break;
                    //}
                    enabled = false;
                    break;
                default:
                    if (Commons.doc.IsFamilyDocument == false)
                    {
                        // Project document: loaded families
                        FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                        ElementFilter filter = new ElementCategoryFilter(mode);
                        IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                        List<string> familyname = new List<string>();
                        for (int i = 0; i < elements.Count; i++)
                        {
                            FamilySymbol symbol = elements[i] as FamilySymbol;
                            if (symbol == null) continue;

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
                        // Family document: this family only
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
                    cmbParameter.SelectedItem = "RC Column－Parameter";
                    break;
                case BuiltInCategory.OST_StructuralFraming:
                    cmbParameter.Items.AddRange(parameterSet_Frame);
                    cmbParameter.SelectedItem = "RC Girder－Parameter";                    
                    break;
                case BuiltInCategory.OST_Floors: cmbParameter.SelectedItem = "Structural Floor－Parameter"; break;
                case BuiltInCategory.OST_Walls: cmbParameter.SelectedItem = "Wall－Parameter"; break;
                case BuiltInCategory.OST_StructuralFoundation:
                    cmbParameter.Items.AddRange(parameterSet_Foundation);
                    cmbParameter.SelectedItem = "RC Footing Rectangle－Parameter";
                    break;
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open " + buzai + " Family";
            string name = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FamilyOption famop = new FamilyOption();
                name = Path.GetFileNameWithoutExtension(openFileDialog1.FileNames[0]);
                Transaction tran = new Transaction(Commons.doc, "Load " + name);
                
                try
                {
                    tran.Start();
                    for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                    {
                        bool nextflg = false;
                        if (File.Exists(openFileDialog1.FileNames[i]) == true) // As a precaution
                        {
                            // Load family
                            Family family = null;
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
                                // Select loaded family
                                cmbFamily.SelectedItem = name;
                                continue;
                            }
                            if (Commons.doc.LoadFamily(openFileDialog1.FileNames[i], famop, out family))
                            {
                                if (family.FamilyCategoryId.Value() == (long)mode)
                                {
                                    // Successfully loaded with matching category: add to combo
                                    if (cmbFamily.Items.Contains(family.Name) == false)
                                    {
                                        cmbFamily.Items.Add(family.Name);
                                        targetFamily.Add(family);
                                        // Select newly added family
                                        cmbFamily.SelectedItem = family.Name;
                                        tran.Commit();
                                        tran.Start();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Failed to load " + family.Name + ".\r\n" + 
                                                    "Verify the family category is [" + buzai + "].", 
                                                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    tran.RollBack();
                                    tran.Start();
                                }
                            }
                            else
                            {
                                string mes = name + " is already loaded.";
                                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                // Select loaded family
                                cmbFamily.SelectedItem = name;
                                continue;
                            }
                        }
                    }
                    tran.Commit();
                }
                catch(Exception)
                {
                    MessageBox.Show("Failed to load " + name + ".\r\n" +
                                    "Verify the family category is [" + buzai + "].",
                                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tran.RollBack();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(RevitLNK.sharedParamsFile))
            {
                string mes = "Shared parameter file could not be found.";
                MessageBox.Show(mes + "\r\n\r\n" + RevitLNK.sharedParamsFile, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                string buzaimei = cmbParameter.SelectedItem.ToString().Replace("－", "");
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
                        tran = new Transaction(d, buzaimei + " Add");
                    }
                }
                else
                {
                    fmg = Commons.doc.FamilyManager;
                    tran = new Transaction(Commons.doc, buzaimei + " Add");
                }
                if (tran == null) { return; }
                tran.Start();

                // Status bar
                pform_Show(pform, ref pfflg, RevitLNK.formtitle + " Adding Parameter");
                ProgressBar_Show(pform, "Adding " + paraname);
                fromSTB.GaugePercent(familyname, (int)((double)1 / (double)1 * 100));
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
                        case 7: ParaSet.SetPara_Precastpile(fmg, SetFamily.PrecastPile); break;                            
                    }
                }

                if (Commons.doc.IsFamilyDocument == false)
                {
                    FamilyOption famop = new FamilyOption();
                    d.LoadFamily(Commons.doc, famop);
                }

                // Clear progress gauge
                if (this != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    fromSTB.GaugeClose();
                    pform.Close();
                }
                string mes = "Added " + paraname + " to " + familyname + ".";
                MessageBox.Show(mes, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                tran.Commit();

                if (Commons.doc.IsFamilyDocument == false)
                {
                    d.Close(false);
                }

            }
            catch(Exception)
            {
                // Clear progress gauge
                if (this != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    fromSTB.GaugeClose();
                    pform.Close();
                }
                MessageBox.Show("Failed to add " + paraname + " to " + familyname + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tran.RollBack();
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void AddParameterForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            linkLabel1_LinkClicked(null, null);
            hlpevent.Handled = true;
        }

        internal static void pform_Show(ProgressBarForm pform, ref bool pformflg, string title)
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
            fromSTB.gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            fromSTB.GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            fromSTB.GaugeShow();
            pform.Refresh();
        }
        
    }
}
