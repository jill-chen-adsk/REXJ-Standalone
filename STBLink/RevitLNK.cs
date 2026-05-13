using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Diagnostics;

using Autodesk;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Xml.Linq;
using System.Security.AccessControl;

namespace STBLink
{



    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RevitLNK : IExternalApplication
    {
        internal const string formtitle = "ST-Bridge Link for Revit";

#if REVIT2022
        internal const string RevitVersion = "2022";
#elif REVIT2023
        internal const string RevitVersion = "2023";
#elif REVIT2024
        internal const string RevitVersion = "2024";
#elif REVIT2025
        internal const string RevitVersion = "2025";
#elif REVIT2026
        internal const string RevitVersion = "2026";
#elif REVIT2027
        internal const string RevitVersion = "2027";
#endif

        internal const string groupName = "SS3 Parameters";
        // File save location
        internal const string Configuration = "\\Configuration";
        internal const string SampleData = "\\SampleData";
        internal const string mydocuFileFolderName = "\\ST-Bridge Link";
        // File names
        //internal const string chmpath = "STBLink.chm";
        internal const string HelpPath = "STBLinkHelp\\001.html";
        internal const string MaterialMappingTbl = "Material_Mapping.tbl";
        internal const string ColumnBase = "ColumnBase";
        // Under My Documents\Autodesk REXJ\<year>
        internal const string ConvRFA_tbl = "ConvRFA" + RevitVersion + ".tbl";
        internal const string ConvBase_tbl = "ConvBase" + RevitVersion + ".tbl";
        internal const string REXStructual = "REXStructuralLink.txt";
        internal const string REXStructual_org = "REXStructuralLink_org.txt";

        // First mapping table number shipped with this version (includes SS3Link)
        internal const int ConvRFA_1stNo = 3;
        // Current latest mapping table number
        internal const string ConvRFA_RecentNo = "3";

        internal const string ConvRFA_xls = "ConvRFA" + RevitVersion + "_" + ConvRFA_RecentNo + ".xls"; // Latest
        internal const string ConvBase_xls = "ConvBase" + RevitVersion + "_4.xls";
        internal const string RFAtableVersion = RevitVersion + ".1";

        // File names for loading
        internal static string familyTableFile = "";
        internal static string BaseTableFile = "";
        internal static string sharedParamsFile = "";
        internal static string sharedParamsFile_org = "";
        internal static string convRFA = "";
        internal static string convBase = "";
        internal static string openfilename = "";
        internal static string filedata = "";
        // Column Base file path setting
        internal const string BaseFileTag = "Column Base file path : ";

        // Label text (conversion form and add-parameters form)
        /// <summary>Columns / stud columns
        /// </summary>
        internal static string[][] ClmText = { new string[] { "RC Column", "RC Circular Column" },
                               new string[] { "S Column H-Section", "S Column Built-up H-Section", "S Column Square Steel Pipe", "S Column Built-up Square Steel Pipe", "S Column Steel Pipe", "S Column T-Steel", "S Column Channel", "S Column Angle" },
                               new string[] { "SRC Column H-Section","SRC Column Cross","SRC Column T-Section", "SRC Column H-Section (Circular)", "SRC Column Cross (Circular)","SRC Column T-Section (Circular)"},
                               new string[] { "CFT Column Square Steel Pipe","CFT Column Circular Steel Pipe"} };
        // For add-parameters form
        /// <summary>Columns / stud columns
        /// </summary>
        internal static string[][] ClmText2 = { new string[] { "RC Column", "RC Circular Column" },
                               new string[] { "S Column H-Section", "S Column Built-up H-Section", "S Column Square Steel Pipe", "S Column Built-up Square Steel Pipe", "S Column Steel Pipe", "S Column T-Steel", "S Column Channel", "S Column Angle" },
                               new string[] { "SRC Column H-Section","SRC Column Cross","SRC Column T-Section", "SRC Column H-Section (Circular)", "SRC Column Cross (Circular)", "SRC Column T-Section (Circular)", "CFT Column Square Steel Pipe","CFT Column Circular Steel Pipe"} };
        /// <summary>Foundation columns
        /// </summary>
        internal static string[][] FClmText = { new string[] { "RC Foundation Column", "RC Foundation Circular Column" } };
        /// <summary>Girders
        /// </summary>
        internal static string[][] GirText = { new string[] { "Foundation Girder", "Foundation Girder with Haunch", "RC Girder", "RC Girder with Haunch" },
                               new string[] { "S Girder","S Girder Built-up H-Section","S Girder Channel","S Girder Angle","S Girder Lip Channel", "S Girder with Haunch" },
                               new string[] { "SRC Girder" } };
        /// <summary>Beams
        /// </summary>
        internal static string[][] BeamText = { new string[] { "Foundation Beam", "Foundation Beam with Haunch", "RC Beam", "RC Beam with Haunch" },
                               new string[] { "S Beam","S Beam Built-up H-Section","S Beam Channel","S Beam Angle","S Beam Lip Channel", "S Beam with Haunch" },
                               new string[] { "SRC Beam" } };
        /// <summary>Cantilever girders
        /// </summary>
        internal static string[][] CGirText = { new string[] { "RC Cantilever Foundation Girder","RC Cantilever Girder" },
                                new string[] { "S Cantilever Girder H-Section", "S Cantilever Girder Built-up H-Section", "S Cantilever Girder Channel", "S Cantilever Girder Angle", "S Cantilever Girder Lip Channel"},
                                new string[] { "SRC Cantilever Girder" } };
        /// <summary>Cantilever beams
        /// </summary>
        internal static string[][] CBeamText = { new string[] { "RC Cantilever Foundation Beam","RC Cantilever Beam" },
                                new string[] { "S Cantilever Beam H-Section", "S Cantilever Beam Built-up H-Section", "S Cantilever Beam Channel", "S Cantilever Beam Angle", "S Cantilever Beam Lip Channel"},
                                new string[] { "SRC Cantilever Beam" } };
        internal static string[][] SBraText = { new string[] { "S Brace H-Section", "S Brace Built-up H-Section", "S Brace Square Steel Pipe", "S Brace Built-up Square Steel Pipe", "S Brace Circular Steel Pipe" },
                               new string[] { "S Brace Channel", "S Brace Angle", "S Brace Lip Channel", "S Brace Flat Bar", "S Brace Round Bar" } };

        internal static string[][] SlabText = { new string[] { "RC Slab", "Deck Plate" } };

        internal static string[][] WallText = { new string[] { "Wall", "RC Parapet" } };
        // Conversion form
        internal static string[][] BaseText = {new string[] { "Foundation Rectangle","Foundation Rectangle Taper","Foundation Triangle","Foundation Equilateral Triangle","Foundation Octagon"},
                               new string[] { "Mat Foundation"},
                               new string[] { "Cast-in-Place Pile","Precast Pile"} };
        // Add-parameters form
        internal static string[][] FoundationText2 = {new string[] { "Foundation Rectangle","Foundation Rectangle Taper","Foundation Triangle","Foundation Equilateral Triangle","Foundation Octagon"},
                               new string[] { "Mat Foundation"},
                               new string[] { "Cast-in-Place Pile","Precast Pile"} };
        internal static string[][] SBraText1 = { new string[] { "S Brace H-Section", "S Brace Built-up H-Section", "S Brace Square Steel Pipe", "S Brace Built-up Square Steel Pipe", "S Brace Circular Steel Pipe" ,
                                                                "S Brace Channel", "S Brace Angle", "S Brace Lip Channel", "S Brace Flat Bar", "S Brace Round Bar" } };
        internal static LoadFamily LoFa;

        // Structural steel symbol names (STB identifiers)
        internal const string st_steel_H = "StbSecRoll-H";
        internal const string st_steel_BH = "StbSecBuild-H";
        internal const string st_steel_Box = "StbSecRoll-BOX";
        internal const string st_steel_BBox = "StbSecBuild-BOX";
        internal const string st_steel_C = "StbSecRoll-C";
        internal const string st_steel_L = "StbSecRoll-L";
        internal const string st_steel_LipC = "StbSecRoll-LipC";
        internal const string st_steel_T = "StbSecRoll-T";
        internal const string st_steel_Pipe = "StbSecPipe";
        internal const string st_steel_FB = "StbSecRoll-FB";
        internal const string st_steel_Bar = "StbSecRoll-Bar";



        // Column Base class
        internal class BaseColumn
        {
            internal string product_company = "";
            internal string product_code = "";
            internal string rfa_pass = "";
            internal string pass = "";
            internal string typename = "";
            /// <summary>true when information exists in STB
            /// </summary>
            internal bool flg = false;
        }
        /// <summary>Column Base table list
        /// </summary>
        internal static List<BaseColumn> BClm = new List<BaseColumn>();


        // Stored values from level mapping dialog (floors / levels)
        internal class LevelPare
        {
            /// <summary>STB story name
            /// </summary>
            internal string stbStrory;
            /// <summary>Revit level name
            /// </summary>
            internal string RevitLevel;
            /// <summary>Offset value
            /// </summary>
            internal int offset;
        }
        internal static List<LevelPare> LPare;

        // Radio button selections
        internal static bool radb1check = false;
        internal static bool radb2check = false;

        // Axis reference and offset values
        internal class AxisPare
        {
            /// <summary>STB axis name
            /// </summary>
            internal string stbAxis;
            /// <summary>Revit grid name
            /// </summary>
            internal string RevitGrid;
            /// <summary>Offset value
            /// </summary>
            internal double offset;
            internal AxisPare()
            {
                stbAxis = "";
                RevitGrid = "";
                offset = 0;
            }
        }
        internal static AxisPare XPare = new AxisPare();
        internal static AxisPare YPare = new AxisPare();



        //************ Material mapping form *********************************************************        
        internal class Materialdata
        {
            internal string stbmatName;
            internal string RevitmatName;
        }
        internal class Concredata
        {
            internal string kouzou;
            internal string STBstrength;
            internal string Revitname;
        }

        
        // Steel materials
        internal static List<Materialdata> MateData = new List<Materialdata>();
        // Concrete materials
        internal static List<Concredata> ConcData = new List<Concredata>();

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            const string TabName = "REXJ Standalone";
            const string rbnName = "ST-Bridge Link";

            string assembly = this.GetType().Assembly.Location;
            Commons.DLLFilePath = System.IO.Path.GetDirectoryName(assembly) + "\\";
            RibbonPanel rbnPanel = null;

            List<RibbonPanel> rbnList = new List<RibbonPanel>();
            try { rbnList = application.GetRibbonPanels(TabName); } catch { }
            for (int i = 0; i < rbnList.Count; i++)
            {
                if (rbnList[i].Name == rbnName)
                {
                    rbnPanel = rbnList[i];
                    break;
                }
            }

            if (rbnPanel == null)
            {
                try
                {
                    rbnPanel = application.CreateRibbonPanel(TabName, rbnName);
                }
                catch
                {
                    application.CreateRibbonTab(TabName);
                    rbnPanel = application.CreateRibbonPanel(TabName, rbnName);
                }
            }

            string[] btnName = new string[] { "ST-Bridge Import", "Diff Import (Section)", "Diff Import (+Placement)", "Diff Import (+Detail)", "STB Export" };
            string[] clsName = new string[] { "Cmd_1", "Cmd_8_1", "Cmd_8_2", "Cmd_8_3", "Cmd_7" };
            string[] tooltip = new string[] { "Full ST-Bridge import", "Diff import - sections only", "Diff import - sections and placement", "Diff import - sections and detailed positions", "Export to ST-Bridge" };

            string configDir = System.IO.Path.GetDirectoryName(assembly) + Configuration + "\\";
            string mainIconPath = configDir + "Icon.png";
            var helpPath = $"{Path.GetDirectoryName( assembly )}\\{HelpPath}";
            var chmhelp = new ContextualHelp( ContextualHelpType.Url, helpPath );

            var pulldownData = new PulldownButtonData( "STBLinkPulldown", "ST-Bridge\nLink" );
            if ( System.IO.File.Exists( mainIconPath ) ) {
                pulldownData.LargeImage = new BitmapImage( new Uri( mainIconPath ) );
                pulldownData.Image = new BitmapImage( new Uri( mainIconPath ) );
            }
            PulldownButton pulldown = rbnPanel.AddItem( pulldownData ) as PulldownButton;
            pulldown.ToolTip = "Legacy ST-Bridge Link Tools";
            pulldown.SetContextualHelp( chmhelp );

            for ( int i = 0; i < btnName.Length; i++ ) {
                var pbd = new PushButtonData( "STB_" + clsName[i], btnName[i], assembly, "STBLink." + clsName[i] );
                pbd.ToolTip = tooltip[i];
                pbd.SetContextualHelp( chmhelp );
                if ( System.IO.File.Exists( mainIconPath ) ) {
                    pbd.LargeImage = new BitmapImage( new Uri( mainIconPath ) );
                    pbd.Image = new BitmapImage( new Uri( mainIconPath ) );
                }
                pulldown.AddPushButton( pbd );
            }


            CopyAllFile(assembly);
           
            return Result.Succeeded;

        }

        /// <summary>Copies deployment files into the user's document folder when the add-in loads
        /// </summary>
        /// <param name="assembly"></param>
        private static void CopyAllFile(string assembly)
        {
            bool REXStructire_org_flg = false;
            string path = System.IO.Path.GetDirectoryName(assembly) + Configuration;
            // Paths for copied files
            string mydocu = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
            if (!System.IO.Directory.Exists(mydocu))
            {
                System.IO.Directory.CreateDirectory(mydocu);
            }
            if (!System.IO.Directory.Exists(mydocu + mydocuFileFolderName))
            {
                System.IO.Directory.CreateDirectory(mydocu + mydocuFileFolderName);
            }
            if (!System.IO.Directory.Exists(mydocu + mydocuFileFolderName + Configuration))
            {
                System.IO.Directory.CreateDirectory(mydocu + mydocuFileFolderName + Configuration);
            }           

            // Configuration folder path
            string cfd = mydocu + mydocuFileFolderName + Configuration;

            // Whether to show Excel update prompts
            bool excelflg = false; // true = show prompts

            // Copy files from the deployment Configuration folder
            foreach (string stCopyFrom in System.IO.Directory.GetFiles(path))
            {
                // overwrite / copy destination
                bool copyflg = false;
                string stCopyTo = "";
                string filename = System.IO.Path.GetFileName(stCopyFrom);
                stCopyTo = System.IO.Path.Combine(mydocu, System.IO.Path.GetFileName(stCopyFrom));
                switch (filename)
                {
                    /*
                    case ConvBase_tbl:// Column Base .tbl → copy into mydocu if convBase Excel is missing (commented legacy block)
                        if (!File.Exists(stCopyTo))
                        {
                            copyflg = true;
                        }
                        else
                        {
                            if (!File.Exists(mydocu + "\\" + ConvBase_xls))
                            {
                                copyflg = true;
                            }
                        }
                        break;
                    case ConvRFA_tbl:// Family .tbl → legacy copy logic (Aug 25, 2017: ConvRFA_02 bump)
                        if (!File.Exists(mydocu + "\\" + ConvRFA_xls))
                        {
                            int.TryParse(ConvRFA_RecentNo, out int n);
                            bool existOldExcel = false;
                            for (int i = n - 1; n >= ConvRFA_1stNo; n--)
                            {
                                existOldExcel = File.Exists(mydocu + "\\" + "ConvRFA" + RevitVersion + "_" + i.ToString() + ".xls");
                                if (existOldExcel) break;
                            }

                            if (existOldExcel) // No latest Excel; an older numbered file exists
                            {
                                // Backup the previous .tbl
                                if (File.Exists(stCopyTo))
                                {
                                    int index = stCopyTo.IndexOf("ConvRFA");
                                    int bkupnum = 1;
                                    string bkupname = "";
                                    do
                                    {
                                        bkupname = stCopyTo.Insert(index, "bkup" + bkupnum.ToString("00#") + "_");
                                        if (!File.Exists(bkupname))
                                        { break; }
                                        else
                                        {
                                            bkupnum++;
                                        }
                                    }
                                    while (File.Exists(bkupname));
                                    System.IO.File.Copy(stCopyTo, bkupname, true);
                                }
                                excelflg = true;
                            }
                            copyflg = true;

                        }
                        
                        break;
                    //case ConvRFA_xls_1:// Legacy family-mapping copy-if-missing branch
                    //    if (!File.Exists(stCopyTo))
                    //    {
                    //        copyflg = true;
                    //    }
                    //    break;
                    case ConvRFA_xls:// Family mapping Excel (Aug 25, 2017) → copy if missing under mydocu
                        if (!File.Exists(stCopyTo))
                        {
                            copyflg = true;

                        }
                        break;
                    case ConvBase_xls:// Column Base mapping Excel → copy if missing
                    case REXStructual:// Shared parameters file → copy if missing
                        if (!File.Exists(stCopyTo))
                        {
                            copyflg = true;
                        }
                        break;
                    case REXStructual_org:
                        copyflg = true;
                        REXStructire_org_flg = true;
                        break;
                    //*/

                    // case chmpath:
                    //     stCopyTo = System.IO.Path.Combine(cfd, System.IO.Path.GetFileName(stCopyFrom));
                    //     copyflg = true;
                    //     break;

                    case MaterialMappingTbl:
                        stCopyTo = System.IO.Path.Combine(cfd, System.IO.Path.GetFileName(stCopyFrom));
                        if (!File.Exists(stCopyTo))
                        {
                            copyflg = true;
                        }
                        break;

                    default:// Icons and other packaged resources
                        //stCopyTo = System.IO.Path.Combine(cfd, System.IO.Path.GetFileName(stCopyFrom));
                        //copyflg = true;
                        break;
                }
                if (copyflg)
                {
                    try
                    {
                        System.IO.File.Copy(stCopyFrom, stCopyTo, true);
                    }
                    catch { }
                }
            }

            // Assign resolved paths used when loading mapping data
            familyTableFile = mydocu + "\\" + ConvRFA_tbl;
            BaseTableFile = mydocu + "\\" + ConvBase_tbl;
            sharedParamsFile = mydocu + "\\" + REXStructual;
            sharedParamsFile_org = mydocu + "\\" + REXStructual_org;
            convRFA = mydocu + "\\" + ConvRFA_xls;
            convBase = mydocu + "\\" + ConvBase_xls;

            if (REXStructire_org_flg) { Write_REXStructure(); }


            // Copy bundled Column Base .rfa folders
            CopyDirectory(path + "\\" + ColumnBase, cfd + "\\" + ColumnBase);


            Get_New_FileName();

            // Notify user when Mapping Table Excel was refreshed
            if (excelflg)
            {
                DialogResult re = MessageBox.Show("The Mapping Table editor Excel workbook has been updated.\r\n" +
                                                  "Open the Mapping Table editor and migrate your current mapping settings?\r\n" +
                                                  "Because the Mapping Table editor workbook was updated, " +
                                                  "if you use SS3 Link, RC Section List Creation, or S Section List Creation, please update those workflows accordingly.\r\n\r\n" +
                                                  "Note: Editing the Mapping Table should be done only by someone with Family edit permission. " +
                                                  "Users without Family edit permission should obtain an updated Mapping Table from someone who has it.",
                                                  formtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (re == DialogResult.Yes)
                {
                    if (File.Exists(RevitLNK.convRFA))
                    {
                        System.Diagnostics.Process.Start(RevitLNK.convRFA);

                    }
                }
            }
        }

        internal static void Write_REXStructure()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> add_text = new List<string>();
            if (!File.Exists(sharedParamsFile) || !File.Exists(sharedParamsFile_org)) { return; }
            StreamReader sr = new StreamReader(sharedParamsFile, Encoding.GetEncoding("Shift_JIS"));
            StreamReader sr_org = new StreamReader(sharedParamsFile_org, Encoding.GetEncoding("Shift_JIS"));

            string str_all = sr.ReadToEnd();
            bool addflg = false;
            sr.Close();
            while (sr_org.Peek() >= 0)
            {
                do
                {
                    string str_org = sr_org.ReadLine();
                    if (str_org == "") { continue; }
                    if (!str_org.Contains("PARAM")) { continue; }

                    string[] split;
                    string[] jouken = { "\t" };
                    split = str_org.Split(jouken, StringSplitOptions.None);
                    if (!str_all.Contains(split[1]))
                    {
                        addflg = true;
                        str_all += str_org + "\r\n";
                    }

                } while (false);
            }
            sr_org.Close();

            if (addflg)
            {
                StreamWriter sw = new StreamWriter(sharedParamsFile, false, Encoding.GetEncoding("Shift_JIS"));
                sw.Write(str_all);
                sw.Close();
            }
        }
                
        internal static void Get_New_FileName()
        {
            string mydocu = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitVersion;
            string openxls = "";
            int lastnum = 0;
            string[] excelFiles = Directory.GetFiles(mydocu, "ConvRFA" + RevitVersion + "_*.xls");
            foreach (string convrfafile in excelFiles)
            {
                string name = Path.GetFileNameWithoutExtension(convrfafile);
                name = name.Replace("ConvRFA" + RevitVersion + "_", "");

                if (openxls == "")
                {
                    openxls = convrfafile;
                    int.TryParse(name, out lastnum);
                }
                else
                {
                    if (int.TryParse(name, out int n) == true)
                    {
                        if (n > lastnum)
                        {
                            lastnum = n;
                            openxls = convrfafile;
                        }
                    }
                }
            }
            if (File.Exists(openxls))
            {
                RevitLNK.convRFA = openxls;
            }

            excelFiles = Directory.GetFiles(mydocu, "ConvBase" + RevitVersion + "_*.xls");
            lastnum = 0;
            openxls = "";
            foreach (string convrfafile in excelFiles)
            {
                string name = Path.GetFileNameWithoutExtension(convrfafile);
                name = name.Replace("ConvBase" + RevitVersion + "_", "");

                if (openxls == "")
                {
                    openxls = convrfafile;
                    int.TryParse(name, out lastnum);
                }
                else
                {
                    if (int.TryParse(name, out int n) == true)
                    {
                        if (n > lastnum)
                        {
                            lastnum = n;
                            openxls = convrfafile;
                        }
                    }
                }
            }
            if (File.Exists(openxls))
            {
                RevitLNK.convBase = openxls;
            }
        }

       
        /// <summary>Recursively copies a directory tree.
        /// </summary>
        /// <param name="sourceDirName">Source directory</param>
        /// <param name="destDirName">Destination directory</param>
        private static void CopyDirectory(
            string sourceDirName, string destDirName)
        {
            if (!System.IO.Directory.Exists(sourceDirName))
            {
                return;
            }

            // Create destination folder if missing
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
                // Copy attributes from source
                System.IO.File.SetAttributes(destDirName,
                    System.IO.File.GetAttributes(sourceDirName));
            }

            // Ensure destination path ends with directory separator
            if (destDirName[destDirName.Length - 1] !=
                    System.IO.Path.DirectorySeparatorChar)
                destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;

            // Copy files from source
            string[] files = System.IO.Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                if (!File.Exists(destDirName + System.IO.Path.GetFileName(file)))
                {
                    System.IO.File.Copy(file, destDirName + System.IO.Path.GetFileName(file), true);
                }
            }

            // Recurse into subdirectories
            string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
                CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
        }

        internal static void OnFailuresProcessing(object sender, Autodesk.Revit.DB.Events.FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor
              = e.GetFailuresAccessor();
            // Remove all Revit warnings from the failure list
            failuresAccessor.DeleteAllWarnings();
          
            e.SetProcessingResult(FailureProcessingResult.Continue);
        }

        #region Progress display

        internal static void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            GaugeShow();
            pform.Refresh();
        }

        /// <summary>Form used to show progress
        /// </summary>
        internal static System.Windows.Forms.Form gaugeForm = null;

        internal static string preGaugeText = "";
        internal static int preGaugePer = 0;
        internal static int RefreshPer = 0;

        internal static bool gaugePosSet = false;
        internal static int preGaugeLeft;
        internal static int preGaugeTop;
        internal static int preGaugeHeight;
        internal static int preGaugeWidth;

        /// <summary>Sets size and position of the progress gauge
        /// </summary>
        /// <param name="setFlag">true = use specified bounds, false = reset defaults</param>
        internal static void GaugePositionSet(bool setFlag, int left, int top, int width, int height)
        {
            if (setFlag)
            {
                preGaugeLeft = left;
                preGaugeTop = top;
                preGaugeWidth = width;
                preGaugeHeight = height;
            }
            else
            {
                left = 0;
                top = 0;
                width = 300;
                height = 38;
                preGaugeLeft = left;
                preGaugeTop = top;
                preGaugeWidth = width;
                preGaugeHeight = height;
            }
            gaugePosSet = setFlag;
        }
        internal static void GaugeShow()
        {
            if (gaugeForm == null) return;


            int GaugeWidth = 300;
            int GaugeHeight = 38;
            int GaugeLeft = 0;
            int GaugeTop = 0;

            System.Windows.Forms.Control[] Ctr;
            gaugeForm.SuspendLayout();
            Ctr = gaugeForm.Controls.Find("Gauge", false);
            if (Ctr.Length != 0) return;

            if (GaugeWidth > gaugeForm.ClientSize.Width) GaugeWidth = gaugeForm.ClientSize.Width - 10;
            GaugeLeft = (int)((gaugeForm.ClientSize.Width - GaugeWidth) / 2);
            GaugeTop = (int)((gaugeForm.ClientSize.Height - GaugeHeight) / 2);

            if (gaugePosSet)
            {
                GaugeLeft = preGaugeLeft;
                GaugeTop = preGaugeTop;
                GaugeWidth = preGaugeWidth;
                GaugeHeight = preGaugeHeight;
            }

            PictureBox Gauge = new PictureBox
            {
                Name = "Gauge",
                Width = GaugeWidth,
                Height = GaugeHeight,
                Visible = false
            };
            gaugeForm.Controls.Add(Gauge);
            Gauge.Visible = false;
            Gauge.BringToFront();
            Gauge.Left = GaugeLeft;
            Gauge.Top = GaugeTop;
            Gauge.BorderStyle = BorderStyle.Fixed3D;
            Gauge.BackColor = System.Drawing.Color.White;
            gaugeForm.ResumeLayout(true);
            Gauge.Visible = true;
            Application.DoEvents();

            preGaugeText = "";
            preGaugePer = 0;
            RefreshPer = 0;
        }



        internal static void GaugeClose()
        {
            if (gaugeForm != null)
            {
                System.Windows.Forms.Control[] Ctr;
                gaugeForm.SuspendLayout();
                Ctr = gaugeForm.Controls.Find("Gauge", false);
                if (Ctr.Length == 0) return;
                gaugeForm.Controls.Remove(Ctr[0]);
                gaugeForm.ResumeLayout(false);
            }
        }



        internal static void GaugePercent(string Txt, int Per)
        {
            if (gaugeForm == null) return;

            System.Windows.Forms.Control[] Ctr;
            PictureBox Gauge;

            Ctr = gaugeForm.Controls.Find("Gauge", false);
            if (Ctr.Length == 0) return;
            Gauge = (PictureBox)Ctr[0];

            bool GoFlag;
            int p;
            string TxtMoji;
            Single MW, MH, sX, sY;
            System.Drawing.Point sp = new System.Drawing.Point();
            System.Drawing.Point ep = new System.Drawing.Point();
            System.Drawing.Color BC, FC;
            Brush FBrsh; // Progress bar fill color
            Brush MBrsh; // Text color
            Brush BBrsh;
            Gauge.Image = new Bitmap(Gauge.ClientRectangle.Width, Gauge.ClientRectangle.Height);
            Graphics g = Graphics.FromImage(Gauge.Image);
            Font ft = new Font(SystemInformation.MenuFont, FontStyle.Regular);
            StringFormat sf = StringFormat.GenericDefault;

            if (preGaugeText != Txt) { preGaugeText = ""; preGaugePer = -1; }
            p = Per;
            if (Per < 0) p = 0;
            if (Per > 100) p = 100;
            TxtMoji = Txt;
            if (TxtMoji == "") TxtMoji = p.ToString() + "%";

            GoFlag = false;
            if (preGaugeText != TxtMoji || p > preGaugePer)
            { GoFlag = true; }
            else
            { if (Txt == "") GoFlag = true; }
            if (GoFlag)
            {
                BC = System.Drawing.Color.White;
                FC = System.Drawing.Color.Yellow;
                BBrsh = Brushes.White;
                FBrsh = Brushes.DeepSkyBlue;
                MBrsh = Brushes.DarkOliveGreen;
                SizeF StringSize = g.MeasureString(TxtMoji, ft, 1000, sf);
                MW = StringSize.Width;
                MH = StringSize.Height;
                sX = (Gauge.Width - MW) / 2;
                sY = (Gauge.Height - MH) / 2;
                sp.X = (int)sX; sp.Y = (int)sY;
                ep.X = (int)MW; ep.Y = (int)MH;
                sp.X = 0;
                sp.Y = 0;
                ep.X = Gauge.Width;
                ep.Y = Gauge.Height;
                g.FillRectangle(BBrsh, sp.X, sp.Y, (ep.X - sp.X), (ep.Y - sp.Y));
                g.DrawString(TxtMoji, ft, MBrsh, sX, sY);
                if (p != -1)
                {
                    sp.X = 0;
                    sp.Y = 0;
                    ep.X = (int)(Gauge.Width * Per / 100);
                    ep.Y = Gauge.Height;
                    g.FillRectangle(FBrsh, sp.X, sp.Y, ep.X, ep.Y);
                    g.DrawString(TxtMoji, ft, MBrsh, sX, sY);
                }
                Gauge.Refresh();
                preGaugeText = TxtMoji;
                preGaugePer = Per;

                // Call DoEvents every 5% to refresh the UI
                if (preGaugePer - RefreshPer > 5)
                {
                    Application.DoEvents();
                    RefreshPer = preGaugePer;
                }
            }
            g.Dispose();
        }

        #endregion



        internal static bool FileCheck()
        {
            // Launched from Family Editor (not supported)
            if (Commons.doc.IsFamilyDocument == true)
            {
                MessageBox.Show("This command is not available in Family Editor.", RevitLNK.formtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            bool tblfileflg = File.Exists(RevitLNK.familyTableFile);
            bool basetblsfileflg = File.Exists(RevitLNK.BaseTableFile);
            string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
            if (!tblfileflg && !basetblsfileflg)
            {
                MessageBox.Show("The Mapping Table and Column Base Mapping Table files were not found. " +
                                "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                " and Column Base Mapping Table file \"" + RevitLNK.ConvBase_tbl + "\" exist at:\r\n\r\n" +
                                "Mapping Table file storage location: " + mydocu,
                                RevitLNK.formtitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (!tblfileflg && basetblsfileflg)
            {
                MessageBox.Show("The Mapping Table file was not found. " +
                                "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                " exists at:\r\n\r\n" +
                                "Mapping Table file storage location: " + mydocu,
                                RevitLNK.formtitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (tblfileflg && !basetblsfileflg)
            {
                MessageBox.Show("The Column Base Mapping Table file was not found. " +
                               "Please verify the Column Base Mapping Table file \"" + RevitLNK.ConvBase_tbl + "\" exists at:\r\n\r\n" +
                               "Mapping Table file storage location: " + mydocu,
                               RevitLNK.formtitle,
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


            return true;
        }

        internal static bool Convert_Start(ExternalCommandData commandData, int mode)
        {
            RevitLNK.LoFa = new LoadFamily();

            // Load family names already present in the project
            RevitLNK.LoFa.LoadFfamily_fromProject();
            RevitLNK.LoFa.LoadLevelfamily_fromProject();
            RevitLNK.LoFa.ViewPlanfamily_fromProject();
            RevitLNK.LoFa.Axisfamily_fromProject();
            RevitLNK.LoFa.Materialfamily_fromProject();
            RevitLNK.LoFa.Concretefamily_fromProject();

            // Read family and parameter names from the Mapping Table
            if (!SetFamily.LoadTable())
            {
                MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                 RevitLNK.formtitle,
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Load Column Base table
            SetFamily.LoadBaseTable();


            // Suppress Revit failure warnings for this workflow
            UIApplication app = commandData.Application;
            app.Application.FailuresProcessing -= new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(RevitLNK.OnFailuresProcessing);
            app.Application.FailuresProcessing += new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(RevitLNK.OnFailuresProcessing);


            OpenFileDialog opf = new OpenFileDialog
            {
                Title = RevitLNK.formtitle + " Select File",
                Filter = "ST-Bridge files|*.stb;*.XML|All files|*.*"
            };

            if (opf.ShowDialog() == DialogResult.OK)
            {

                // Selected ST-B file path
                RevitLNK.openfilename = opf.FileName;

                // File last-write time
                RevitLNK.filedata = System.IO.File.GetLastWriteTime(opf.FileName).ToString();

                if (mode > 0)
                {
                    var data = XDocument.Load(RevitLNK.openfilename);
                    var version = data.Root.Attribute("version")?.Value ?? "";
                    if (!version.StartsWith("2"))
                    {
                        MessageBox.Show("Diff Import applies to ST-B Ver 2.0 only.", RevitLNK.formtitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }


                // Import / conversion settings dialog
                ConvertForm f = new ConvertForm();

                // Validate ST-B geometry before opening the main UI
                if (!f.CheckSTB())
                {
                    if (LogData.STBLog.Count() > 0)
                    {
                        LogForm lf = new LogForm
                        {
                            Text = RevitLNK.formtitle + " STB file read error " + Commons.GetVersion(),
                        };
                        LogData.AddSTBLog(LogData.LogKind.Error, 3000, "Import was cancelled because one or more members could not be read.");
                        lf.ShowDialog();
                    }
                }
                else
                {
                    int n = 0;
                    DialogResult result = DialogResult.OK;

                    LevelMapping lev = new LevelMapping();
                    MaterialMapping map = new MaterialMapping();
                    SabunForm sabun = new SabunForm();
                    Data.WindowWrapper wrapper = new Data.WindowWrapper(Data.RevitHandle);

                    while (true)
                    {
                        switch (n)
                        {
                            case 0: // Conversion menu
                                result = f.ShowDialog(wrapper);
                                break;
                            case 1: // Level mapping
                                if (mode == 0)
                                {
                                    result = lev.ShowDialog(wrapper);
                                }
                                break;
                            case 2: // Material mapping
                                result = map.ShowDialog(wrapper);
                                break;
                            case 3: // Diff: type and instance selection
                                if (mode > 0)
                                {
                                    result = sabun.ShowDialog(wrapper);
                                }
                                break;
                            case 4: // Confirm start
                                string okmessage = "Start the import?";
                                if (MessageBox.Show(okmessage, RevitLNK.formtitle + " " + Commons.GetVersion(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    result = DialogResult.OK;
                                }
                                else
                                {
                                    result = DialogResult.No;
                                }
                                break;
                        }

                        if (result == DialogResult.Yes)
                        {
                            // Next
                            n++;
                        }
                        else if (result == DialogResult.No)
                        {
                            // Back
                            n--;
                        }
                        else if (result == DialogResult.OK)
                        {
                            // Start import
                            break;
                        }
                        else
                        {
                            // Cancel
                            break;
                        }
                    }

                    if (result == DialogResult.OK)
                    {
                        if (mode == 0)
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            // Initialize log buffer
                            LogData.Data = new List<LogData.Log>();

                            LogData.AddLog(LogData.LogKind.Infmoation, 0, "Import source file: " + RevitLNK.openfilename);
                            for (int i = 0; i < LogData.STBLog.Count(); i++)
                            {
                                LogData.AddLog(LogData.STBLog[i].Kind, 0, LogData.STBLog[i].Message);
                            }

                            // Run import pipeline
                            if (ConvertForm.stb2 != null) {

                                // ST-B 2.0 path
                                FromSTB_v2.Initialize();

                                FromSTB_v2.ShouldOutputCommentDebugLog = f.CheckDebugLogOutput ;
                                    
                                // Merge project-level parameters
                                FromSTB_v2.AddProjectParameter(ConvertForm.stb2);

                                // Create Revit levels
                                FromSTB_v2.Level_Convert(ConvertForm.stb2, RevitLNK.LPare);

                                // Create grid datum lines
                                FromSTB_v2.Kiten_Convert(ConvertForm.stb2, RevitLNK.radb1check, RevitLNK.XPare, RevitLNK.YPare);

                                // Create structural members
                                FromSTB_v2.CreateBuzai(ConvertForm.stb2, ConvertForm.Chb_Checked);

                                Data.ProgressClose();
                            }
                            else
                            {
                                // ST-B 1.x path

                                fromSTB fstb = new fromSTB(f);
                                
                                fstb.ShouldOutputCommentDebugLog = f.CheckDebugLogOutput ;
                                
                                ProgressBarForm pform = new ProgressBarForm
                                {
                                    Text = RevitLNK.formtitle + " Importing"
                                };

                                pform.Show();
                                int x = pform.panelFooter.Width + 6;
                                int y = pform.lab.Height + pform.panelFooter.Height + 6;
                                pform.ClientSize = new System.Drawing.Size(x, y);
                                pform.lab.Top = 3;
                                pform.lab.Left = 3;
                                pform.panelFooter.Top = pform.lab.Bottom;
                                pform.panelFooter.Left = 3;

                                fstb.Initialize();

                                // Merge project-level parameters
                                fstb.AddProjectParameter(ConvertForm.stb, pform);
                                // Create Revit levels
                                fstb.Level_Convert(ConvertForm.stb, pform, RevitLNK.LPare);
                                // Create grid datum lines
                                fstb.Kiten_Convert(ConvertForm.stb, pform, RevitLNK.radb1check, RevitLNK.radb2check, RevitLNK.XPare, RevitLNK.YPare);
                                // Create structural members
                                fstb.CreateBuzai(ConvertForm.stb, pform, ConvertForm.Chb_Checked);

                                pform.Close();
                            }

                            sw.Stop();
                            LogData.AddLog(LogData.LogKind.Infmoation, 0, "Import finished (elapsed: " + sw.Elapsed.Minutes + " min " + sw.Elapsed.Seconds + " sec).");

                            LogForm lf = new LogForm
                            {
                                Text = RevitLNK.formtitle + " Import log " + Commons.GetVersion(),
                            };
                            lf.ShowDialog();
                        }
                        else
                        {
                            // Diff import
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            LogData.Data = new List<LogData.Log>();

                            switch (mode)
                            {
                                case 1:
                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "Diff Import (Section/Cross-section Only)");
                                    break;
                                case 2:
                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "Diff Import (Section/Cross-section + Placement)");
                                    break;
                                case 3:
                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "Diff Import (Section/Cross-section + Detailed Position)");
                                    break;
                            }

                            LogData.AddLog(LogData.LogKind.Infmoation, 0, "Import source file: " + RevitLNK.openfilename);
                            for (int i = 0; i < LogData.STBLog.Count(); i++)
                            {
                                LogData.AddLog(LogData.STBLog[i].Kind, 0, LogData.STBLog[i].Message);
                            }



                            FromSTB_v2.Initialize();
                            FromSTB_v2.ShouldOutputCommentDebugLog = f.CheckDebugLogOutput ;
                            Data.ReadKiten();
                            FromSTB_v2.SetAllOffset();


                            TransactionGroup trang = new TransactionGroup(Commons.doc, "ST-Bridge Link Diff Import");
                            trang.Start();
                            try
                            {
                                // Merge project-level parameters
                                FromSTB_v2.AddProjectParameter(ConvertForm.stb2);

                                FromSTB_v2.GuidData = new Dictionary<ElementId, string>();
                                var schema1 = Data.GetSchema(Data.schemaName_Guid);
                                if (schema1 != null)
                                {
                                    Autodesk.Revit.DB.ExtensibleStorage.Entity entity = Commons.doc.ProjectInformation.GetEntity(schema1);
                                    var field = schema1.GetField(Data.FieldName);
                                    FromSTB_v2.GuidData = entity.Get<IDictionary<ElementId, string>>(field);
                                }
                                FromSTB_v2.CheckGuid(ConvertForm.stb2);


                                // Update member sections
                                FromSTB_v2.UpdateSection(ConvertForm.stb2, ConvertForm.Chb_Checked);

                                if (mode >= 2)
                                {
                                    // Update placement
                                    FromSTB_v2.UpdateMember(ConvertForm.stb2, ConvertForm.Chb_Checked, mode == 3);

                                    // Delete members removed in ST-B
                                    FromSTB_v2.DeleteElement();

                                    // Reorder joins
                                    FromSTB_v2.ChangeOrder();
                                }


                                Data.DeleteStoageElementId();
                            }
                            catch
                            {
                                LogData.AddLog(LogData.LogKind.Error, 3000, "An error occurred.");
                            }

                            trang.Assimilate();


                            sw.Stop();
                            LogData.AddLog(LogData.LogKind.Infmoation, 0, "Import finished (elapsed: " + sw.Elapsed.Minutes + " min " + sw.Elapsed.Seconds + " sec).");

                            LogForm lf = new LogForm
                            {
                                Text = RevitLNK.formtitle + " Import log " + Commons.GetVersion(),
                            };
                            lf.ShowDialog();
                        }

                    }
                }

            }

            // Restore default failure handling
            app.Application.FailuresProcessing -= new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(RevitLNK.OnFailuresProcessing);

            return true;
        }

    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Data.RevitHandle = commandData.Application.MainWindowHandle;
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            if (!RevitLNK.FileCheck())
            {
                return Result.Succeeded;
            }

            RevitLNK.Convert_Start(commandData, 0);


            return Result.Succeeded;
        }

    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitLNK.Get_New_FileName();
            Commons.doc = commandData.Application.ActiveUIDocument.Document;

            string assembly = this.GetType().Assembly.Location;

            //RevitLNK.CopyAllFile(assembly);
            
            DialogResult result = MessageBox.Show("You are about to open the Mapping Table editor.\r\n" +
                                                  "Do you have edit permission on the Mapping Table?\r\n\r\n" +
                                                  "Only users with Mapping Table edit permission should edit it.",
                                                  RevitLNK.formtitle + " Mapping Table Editor", 
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);

            
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.convRFA))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table editor Excel file was not found. " +
                                    "Please verify the Mapping Table editor Excel file \"" + RevitLNK.ConvRFA_xls + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table editor Excel storage location: " + mydocu,
                                    RevitLNK.formtitle + " Mapping Table Editor",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
                else
                {
                    System.Diagnostics.Process.Start(RevitLNK.convRFA);
                }
            }
            return Result.Succeeded;
        }

    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Commons.doc = commandData.Application.ActiveUIDocument.Document;

            string assembly = this.GetType().Assembly.Location;
            //RevitLNK.CopyAllFile(assembly);
            if(!File.Exists(RevitLNK.BaseTableFile))
            {
                string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                MessageBox.Show("The Column Base Mapping Table file was not found. " +
                                "Please verify the Column Base Mapping Table file \"" + RevitLNK.ConvBase_tbl + "\" exists at:\r\n\r\n" +
                                "Mapping Table file storage location: " + mydocu,
                                RevitLNK.formtitle + " Column Base Family Path",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader sr = new StreamReader(RevitLNK.BaseTableFile, Encoding.GetEncoding("Shift_JIS"));

            string oldpath = "";
            string str = "";
            List<string> table = new List<string>();
            string[] jouken = { " : " }; // Delimiter used when splitting key / value pairs
            do
            {
                str = sr.ReadLine();
                if (str == null) { break; }
                string[] split = str.Split(jouken, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() == 0) { continue; }
                if (split[0] == "柱脚ファイルパス" || split[0] == "Column Base file path")
                {
                    if (split.Count() != 1)
                    { oldpath = split[1]; }
                    else
                    { oldpath = ""; }
                }
                if (split.Count() > 2)
                {
                    table.Add(split[2]);
                }
            } while (!sr.EndOfStream);
            sr.Close();

            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Select the folder where Column Base Families are saved.\r\n" +
                              "Current Column Base Family path: " + oldpath,
                ShowNewFolderButton = false,
                SelectedPath = oldpath
            };
            bool endflg = false;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                do
                {
                    endflg = false;
                    for (int i = 0; i < table.Count(); i++)
                    {
                        if (File.Exists(fbd.SelectedPath + table[i]))
                        {
                            endflg = true;
                            break;
                        }
                    }
                    if (endflg)
                    {
                        sr = new StreamReader(RevitLNK.BaseTableFile, Encoding.GetEncoding("Shift_JIS"));

                        str = "";
                        List<string> newstr = new List<string>();
                        while (sr.Peek() >= 0)
                        {
                            do
                            {
                                str = sr.ReadLine();
                                if (!str.Contains(RevitLNK.BaseFileTag) && !str.Contains("柱脚ファイルパス"))
                                {
                                    newstr.Add(str);
                                    continue;
                                }
                                else
                                {
                                    string[] split = str.Split(jouken, StringSplitOptions.RemoveEmptyEntries);
                                    if (split.Count() == 0) { continue; }
                                    newstr.Add(RevitLNK.BaseFileTag + fbd.SelectedPath);
                                }

                            } while (false);
                        }
                        sr.Close();


                        StreamWriter sw = new StreamWriter(RevitLNK.BaseTableFile, false, Encoding.GetEncoding("Shift_JIS"));

                        for (int i = 0; i < newstr.Count(); i++)
                        {
                            sw.WriteLine(newstr[i]);
                        }
                        sw.Close();

                        MessageBox.Show("Column Base Family path has been set.\r\n\r\n" + fbd.SelectedPath, RevitLNK.formtitle + " Column Base Family Path",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        endflg = true;
                    }
                    else
                    {
                        MessageBox.Show("Column Base Families were not found. Please verify the Column Base Family path is correct.\r\n\r\n" +
                                        "Selected path:\r\n" + fbd.SelectedPath,
                                        RevitLNK.formtitle + " Column Base Family Path",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        fbd.Description = "Select the folder where Column Base Families are saved.\r\n" +
                             "Current Column Base Family path: " + oldpath;
                        fbd.ShowNewFolderButton = false;
                        fbd.SelectedPath = oldpath;
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                           
                            endflg = false;
                        }
                        else
                        {
                            endflg = true;
                        }
                    }

                } while (!endflg);
            }
           
            fbd.Dispose();

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitLNK.Get_New_FileName();
            Commons.doc = commandData.Application.ActiveUIDocument.Document;

            string assembly = this.GetType().Assembly.Location;
            //RevitLNK.CopyAllFile(assembly);
          
            DialogResult result = MessageBox.Show("You are about to edit the Column Base Mapping Table.\r\n" +
                                                 "\r\nDo you have edit permission on the Column Base Mapping Table?\r\n" +
                                                 "\r\nOnly users with Column Base Mapping Table edit permission should edit it.",
                                                 RevitLNK.formtitle + " Column Base Mapping Table Editor",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.convBase))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Column Base Mapping Table editor Excel file was not found. " +
                                    "Please verify the Column Base Mapping Table editor Excel file \"" + RevitLNK.ConvBase_xls + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Column Base Mapping Table editor Excel storage location: " + mydocu,
                                    RevitLNK.formtitle + " Column Base Mapping Table Editor",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    System.Diagnostics.Process.Start(RevitLNK.convBase);
                }
            }
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_5 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            string assembly = this.GetType().Assembly.Location;
            //RevitLNK.CopyAllFile(assembly);

            if (Commons.doc.IsFamilyDocument == true)
            {
                MessageBox.Show("This command is not available in Family Editor.", RevitLNK.formtitle + " Batch Add Parameters", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }

            DialogResult result = MessageBox.Show("Parameters will be added to the Families specified in the Mapping Table.\r\n" +
                                                  "\r\nDo you have Family edit permission?\r\n" +
                                                  "\r\nOnly users with Family edit permission should run parameter addition.",
                                                  RevitLNK.formtitle + " Batch Add Parameters", MessageBoxButtons.YesNo, 
                                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if(!File.Exists(RevitLNK.familyTableFile))
            {
                string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                MessageBox.Show("The Mapping Table file was not found. " +
                                "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                " exists at:\r\n\r\n" +
                                "Mapping Table file storage location: " + mydocu,
                                RevitLNK.formtitle + " Batch Add Parameters",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }

            if (result == DialogResult.Yes)
            {
                // Collect family names loaded in the project
                RevitLNK.LoFa = new LoadFamily();
                RevitLNK.LoFa.LoadFfamily_fromProject();
                // Read Families and parameters from the Mapping Table                
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                 RevitLNK.formtitle + " Batch Add Parameters",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                STBParaBuild f = new STBParaBuild();

                if (f.ShowDialog() == DialogResult.OK)
                {

                }
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_6c : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string buzai = "Structural Column";
            string title = RevitLNK.formtitle + " " + buzai + " Add Parameters";
            DialogResult result = DialogResult.Yes;

            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();
            if (Commons.doc.IsFamilyDocument == false)
            {
                result = MessageBox.Show("Parameters will be added to " + buzai + " families.\r\n" +
                                                  "\r\nDo you have Family edit permission?\r\n" +
                                                  "\r\nOnly users with Family edit permission should add parameters.",
                                                  title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else
            {
                if (Commons.doc.OwnerFamily.FamilyCategoryId.Value() != (long)BuiltInCategory.OST_StructuralColumns)
                {
                    MessageBox.Show(Commons.doc.OwnerFamily.FamilyCategory.Name + ": This command is not available in Family Editor.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
            }
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.familyTableFile))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table file was not found. " +
                                    "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table file storage location: " + mydocu, title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Load Mapping Table
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                    title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                AddParameterForm f = new AddParameterForm(BuiltInCategory.OST_StructuralColumns);
                f.ShowDialog();
            }
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_6g : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string buzai = "Structural Framing";
            string title = RevitLNK.formtitle + " " + buzai + " Add Parameters";
            DialogResult result = DialogResult.Yes;

            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();
            if (Commons.doc.IsFamilyDocument == false)
            {
                result = MessageBox.Show("Parameters will be added to " + buzai + " families.\r\n" +
                                         "\r\nDo you have Family edit permission?\r\n" +
                                         "\r\nOnly users with Family edit permission should add parameters.",
                                         title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else
            {
                if (Commons.doc.OwnerFamily.FamilyCategoryId.Value() != (long)BuiltInCategory.OST_StructuralFraming)
                {
                    MessageBox.Show(Commons.doc.OwnerFamily.FamilyCategory.Name + ": This command is not available in Family Editor.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
            }
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.familyTableFile))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table file was not found. " +
                                    "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table file storage location: " + mydocu, title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Load Mapping Table
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                    title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
                
                AddParameterForm f = new AddParameterForm(BuiltInCategory.OST_StructuralFraming);
                f.ShowDialog();
            }
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_6s : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string buzai = "Structural Floor/Slab";
            string title = RevitLNK.formtitle + " " + buzai + " Add Parameters";

            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();
            if (Commons.doc.IsFamilyDocument == true)
            {
                MessageBox.Show(Commons.doc.OwnerFamily.FamilyCategory.Name + ": This command is not available in Family Editor.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }

            DialogResult result = MessageBox.Show("Parameters will be added to " + buzai + " families.\r\n" +
                                                  "\r\nDo you have Family edit permission?\r\n" +
                                                  "\r\nOnly users with Family edit permission should add parameters.",
                                                  title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.familyTableFile))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table file was not found. " +
                                    "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table file storage location: " + mydocu, title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Load Mapping Table
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                    title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                if (!System.IO.File.Exists(RevitLNK.sharedParamsFile))
                {
                    string mes = "Shared parameters file was not found.";
                    MessageBox.Show(mes + "\r\n\r\n" + RevitLNK.sharedParamsFile, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
                // Title including build version
                string title_ver = title + " " + Commons.GetVersion();

                string tuika = "Add parameters to Structural Floor/Slab families?";
                if (MessageBox.Show(tuika, title_ver, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Parameter helper methods run their own transactions
                    //Transaction tran = new Transaction(Commons.doc, "Add Parameters");
                    //tran.Start();
                    try
                    {
                        FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                        ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                        IList<Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

                        // Show progress bar
                        ProgressBarForm pform = new ProgressBarForm();
                        Stopwatch stopw = new Stopwatch();
                        stopw.Start();
                        bool flg = false;
                        AddParameterForm.pform_Show(pform, ref flg, title);
                        pform.Text = RevitLNK.formtitle + " Adding parameters";

                        foreach (Element el in elms)
                        {
                            if (el is FloorType symbol && symbol.IsFoundationSlab == false)
                            {
                                RevitLNK.ProgressBar_Show(pform, "Adding Structural Floor/Slab parameters");
                                RevitLNK.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                                ParaSet.SetPara_Slab("床", el, SetFamily.Slab);
                                break;
                            }
                        }

                        // Remove progress gauge
                        if (this != null)
                        {
                            do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                            stopw.Stop();
                            RevitLNK.GaugeClose();
                            pform.Close();
                        }
                        string mes = "Parameters were added to Structural Floor/Slab families.";
                        MessageBox.Show(mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //tran.Commit();
                    }
                    catch
                    {
                        string mes = "Failed to add parameters to Structural Floor/Slab families.";
                        MessageBox.Show(mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //tran.RollBack();
                    }
                }
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_6w : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string buzai = "Structural Wall";
            string title = RevitLNK.formtitle + " " + buzai + " Add Parameters";

            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();
            if (Commons.doc.IsFamilyDocument == true)
            {
                MessageBox.Show(Commons.doc.OwnerFamily.FamilyCategory.Name + ": This command is not available in Family Editor.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }

            DialogResult result = MessageBox.Show("Parameters will be added to " + buzai + " families.\r\n" +
                                                "\r\nDo you have Family edit permission?\r\n" +
                                                "\r\nOnly users with Family edit permission should add parameters.",
                                                title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.familyTableFile))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table file was not found. " +
                                    "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table file storage location: " + mydocu, title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Load Mapping Table
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                    title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                if (!System.IO.File.Exists(RevitLNK.sharedParamsFile))
                {
                    string mes = "Shared parameters file was not found.";
                    MessageBox.Show(mes + "\r\n\r\n" + RevitLNK.sharedParamsFile, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Title including build version
                string title_ver = title + " " + Commons.GetVersion();

                string tuika = "Add parameters to Structural Wall families?";
                if (MessageBox.Show(tuika, title_ver, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Parameter helper methods run their own transactions
                    //Transaction tran = new Transaction(Commons.doc, "Add Parameters");
                    //tran.Start();
                    try
                    {
                        FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                        ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                        IList<Element> elms = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

                        // Show progress bar
                        ProgressBarForm pform = new ProgressBarForm();
                        Stopwatch stopw = new Stopwatch();
                        stopw.Start();
                        bool flg = false;
                        AddParameterForm.pform_Show(pform, ref flg, title);
                        pform.Text = RevitLNK.formtitle + " Adding parameters";

                        foreach (Element el in elms)
                        {
                            if (el is WallType symbol && symbol.Kind == WallKind.Basic)
                            {
                                RevitLNK.ProgressBar_Show(pform, "Adding Structural Wall parameters");
                                RevitLNK.GaugePercent(symbol.FamilyName, (int)((double)1 / (double)1 * 100));

                                ParaSet.SetPara_Wall("壁", el, SetFamily.Wall);
                                break;
                            }
                        }

                        // Remove progress gauge
                        if (this != null)
                        {
                            do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                            stopw.Stop();
                            RevitLNK.GaugeClose();
                            pform.Close();
                        }
                        string mes = "Parameters were added to Structural Wall families.";
                        MessageBox.Show(mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //tran.Commit();
                    }
                    catch
                    {
                        string mes = "Failed to add parameters to Structural Wall families.";
                        MessageBox.Show(mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //tran.RollBack();
                    }
                }
            }

            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_6f : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string buzai = "Structural Foundation";
            string title = RevitLNK.formtitle + " " + buzai + " Add Parameters";
            DialogResult result = DialogResult.Yes;

            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();
            if (Commons.doc.IsFamilyDocument == false)
            {
                result = MessageBox.Show("Parameters will be added to " + buzai + " families.\r\n" +
                                         "\r\nDo you have Family edit permission?\r\n" +
                                         "\r\nOnly users with Family edit permission should add parameters.",
                                         title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            else
            {
                if (Commons.doc.OwnerFamily.FamilyCategoryId.Value() != (long)BuiltInCategory.OST_StructuralFoundation)
                {
                    MessageBox.Show(Commons.doc.OwnerFamily.FamilyCategory.Name + ": This command is not available in Family Editor.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }
            }
            if (result == DialogResult.Yes)
            {
                if (!File.Exists(RevitLNK.familyTableFile))
                {
                    string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                    MessageBox.Show("The Mapping Table file was not found. " +
                                    "Please verify the Mapping Table file \"" + RevitLNK.ConvRFA_tbl + "\"" +
                                    " exists at:\r\n\r\n" +
                                    "Mapping Table file storage location: " + mydocu, title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                // Load Mapping Table
                if (!SetFamily.LoadTable())
                {
                    MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                    title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Succeeded;
                }

                AddParameterForm f = new AddParameterForm(BuiltInCategory.OST_StructuralFoundation);
                f.ShowDialog();
            }
            return Result.Succeeded;
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_7 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Data.RevitHandle = commandData.Application.MainWindowHandle;
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            if (!RevitLNK.FileCheck())
            {
                return Result.Succeeded;
            }


            RevitLNK.LoFa = new LoadFamily();

            // Load family names from the project
            RevitLNK.LoFa.LoadFfamily_fromProject();
            RevitLNK.LoFa.LoadLevelfamily_fromProject();
            RevitLNK.LoFa.ViewPlanfamily_fromProject();
            RevitLNK.LoFa.Axisfamily_fromProject();
            RevitLNK.LoFa.Materialfamily_fromProject();
            RevitLNK.LoFa.Concretefamily_fromProject();

            // Read Families and parameters from the Mapping Table
            if (!SetFamily.LoadTable())
            {
                MessageBox.Show("This feature cannot be used because an older Mapping Table version is in use.",
                                 RevitLNK.formtitle,
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Result.Succeeded;
            }

            // Column Base table load
            SetFamily.LoadBaseTable();

            // Reset log collector
            LogData.Data = new List<LogData.Log>();
            if (ConvertForm.stb != null)
            {
                ConvertForm.stb.unknownList = new List<string>();
            }

            Transaction tran = null;
            try
            {
                RevitLNK.openfilename = Commons.doc.PathName;

                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "ST-Bridge file (Ver 2.0)|*.stb|ST-Bridge file (Ver 1.4)|*.stb",
                    FilterIndex = 1,
                };
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return Result.Succeeded;
                }

                if (sfd.FilterIndex == 1)
                {
                    //2.0

                    // Extensible storage writes require an open transaction
                    tran = new Transaction(Commons.doc, "Export");
                    tran.Start();

                    ToSTB_v2.ExportSTB(sfd.FileName);

                    tran.Commit();
                }
                else
                {
                    //1.4

                    // SubTransactions require an outer Transaction
                    tran = new Transaction(Commons.doc, "Export");
                    tran.Start();

                    ToSTB.ExportSTB(sfd.FileName);

                    tran.Commit();
                }

            }
            catch (Exception ex)
            {
                LogData.AddLog(LogData.LogKind.Error, 3000, ex.Message);
                LogData.AddLog(LogData.LogKind.Error, 3000, "Failed to export the ST-B file.");

                if (tran != null && tran.HasStarted())
                {
                    tran.RollBack();
                }

#if DEBUG
                MessageBox.Show(ex.Message + "\r\n\r\n" + ex.StackTrace);
#endif
            }

            if (LogData.Data.Count > 0)
            {
                LogForm lf = new LogForm
                {
                    Text = RevitLNK.formtitle + " Export log " + Commons.GetVersion()
                };
                lf.ShowDialog();
            }


            return Result.Succeeded;
        }
    }



    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_8_1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Data.RevitHandle = commandData.Application.MainWindowHandle;
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            if (!RevitLNK.FileCheck())
            {
                return Result.Succeeded;
            }


            RevitLNK.Convert_Start(commandData, 1);

            return Result.Succeeded;
        }

    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_8_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Data.RevitHandle = commandData.Application.MainWindowHandle;
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            if (!RevitLNK.FileCheck())
            {
                return Result.Succeeded;
            }


            RevitLNK.Convert_Start(commandData, 2);

            return Result.Succeeded;
        }

    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Cmd_8_3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Data.RevitHandle = commandData.Application.MainWindowHandle;
            Commons.doc = commandData.Application.ActiveUIDocument.Document;
            Commons.SetSharedParametersFile();

            if (!RevitLNK.FileCheck())
            {
                return Result.Succeeded;
            }


            RevitLNK.Convert_Start(commandData, 3);

            return Result.Succeeded;
        }

    }


}
