using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning ;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;


namespace MappingTable
{
    class Commons
    {
        internal const string SystemName = "Mapping Table";
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

        //今バージョンで最初に配布したマッピングテーブル番号
        internal const int ConvRFA_1stNo = 3;
        //現在の最新マッピングテーブル番号
        internal const string ConvRFA_RecentNo = "3";

        //今バージョンで最初に配布したマッピングテーブル番号
        internal const int ConvBase_1stNo = 3;
        //現在の最新マッピングテーブル番号
        internal const string ConvBase_RecentNo = "4";


        //テーブルファイル内のバージョン
        internal const string RFAtableVersion = RevitVersion + ".1";


        //ファイル名
        internal const string ConvRFA_tbl = "ConvRFA" + RevitVersion + ".tbl";
        internal const string ConvBase_tbl = "ConvBase" + RevitVersion + ".tbl";
        internal const string REXStructual = "REXStructuralLink.txt";
        internal const string REXStructual_org = "REXStructuralLink_org.txt";
        internal const string ConvRFA_xls = "ConvRFA" + RevitVersion + "_" + ConvRFA_RecentNo + ".xls";
        internal const string ConvBase_xls = "ConvBase" + RevitVersion + "_" + ConvBase_RecentNo + ".xls";
        internal const string ConvRFA_STB2_xlsm = "ConvRFA" + RevitVersion + "_STB2.0_1.xlsm";


        //ファイルの保存場所
        internal const string Configuration = "Configuration\\";

        internal const string chmfile = "MappingTable.chm";

        internal static string DLLFilePath = "";

        internal static Document doc;


        internal static string ConfigPath(string filename) => DLLFilePath + Configuration + filename;
        internal static string HelpPath() => DLLFilePath + "MappingTableHelp\\Top.html";
        internal static string RexJPath(string filename, bool mydoc = false)
        {
            if (!mydoc)
            {
                string SharedParametersFilename = System.IO.Path.GetFileName(Commons.doc.Application.SharedParametersFilename);
                if (SharedParametersFilename == Commons.REXStructual &&
                    System.IO.File.Exists(Commons.doc.Application.SharedParametersFilename))
                {
                    //Documentフォルダにセキュリティのロックが掛けられている会社があるため、
                    //共有パラメータファイル名と同じ場所を使う。
                    //ファイル名が同じなら、どこにあってもよしとする。
                    string folderpath = System.IO.Path.GetDirectoryName(Commons.doc.Application.SharedParametersFilename);
                    return folderpath + "\\" + filename;
                }
            }

            //通常はDocument
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitVersion + "\\" + filename;
        }

        /// <summary>
        /// 共有パラメータファイルパスの設定
        /// </summary>
        internal static void SetSharedParametersFile()
        {
            string filename = System.IO.Path.GetFileName(Commons.doc.Application.SharedParametersFilename);
            if (filename == Commons.REXStructual &&
                System.IO.File.Exists(Commons.doc.Application.SharedParametersFilename))
            {
                //Documentフォルダにセキュリティのロックが掛けられている会社があるため、
                //共有パラメータファイル名が同じなら上書きしないようにする。
                //ファイル名が同じなら、どこにあってもよしとする。
            }
            else
            {
                Commons.doc.Application.SharedParametersFilename = Commons.RexJPath(Commons.REXStructual);
            }
        }



        //Labelのテキスト(変換画面＆パラメータ追加画面)
        /// <summary>柱・間柱
        /// </summary>
        internal static string[][] ClmText = { new string[] { "RC Column", "RC Circular Column" },
                               new string[] { "S Column H-Section", "S Column Built-up H", "S Column Box", "S Column Built-up Box", "S Column Pipe", "S Column T-Section", "S Column Channel", "S Column Angle" },
                               new string[] { "SRC Column H","SRC Column Cross","SRC Column T", "SRC Column H (Circular)", "SRC Column Cross (Circular)","SRC Column T (Circular)"},
                               new string[] { "CFT Column Box","CFT Column Pipe"} };
        //パラメータ追加画面用
        /// <summary>柱・間柱
        /// </summary>
        internal static string[][] ClmText2 = { new string[] { "RC Column", "RC Circular Column" },
                               new string[] { "S Column H-Section", "S Column Built-up H", "S Column Box", "S Column Built-up Box", "S Column Pipe", "S Column T-Section", "S Column Channel", "S Column Angle" },
                               new string[] { "SRC Column H","SRC Column Cross","SRC Column T", "SRC Column H (Circular)", "SRC Column Cross (Circular)", "SRC Column T (Circular)", "CFT Column Box","CFT Column Pipe"} };
        /// <summary>基礎柱
        /// </summary>
        internal static string[][] FClmText = { new string[] { "RC Foundation Column", "RC Circular Foundation Column" } };
        /// <summary>大梁
        /// </summary>
        internal static string[][] GirText = { new string[] { "Foundation Beam", "Haunched Foundation Beam", "RC Beam", "Haunched RC Beam" },
                               new string[] { "S Main Beam","S Beam Built-up H","S Beam Channel","S Beam Angle","S Beam Lip Channel", "Haunched S Main Beam" },
                               new string[] { "SRC Beam" } };
        /// <summary>小梁
        /// </summary>
        internal static string[][] BeamText = { new string[] { "Foundation Sub Beam", "Haunched Foundation Sub Beam", "RC Sub Beam", "Haunched RC Sub Beam" },
                               new string[] { "S Sub Beam","S Sub Beam Built-up H","S Sub Beam Channel","S Sub Beam Angle","S Sub Beam Lip Channel", "Haunched S Sub Beam" },
                               new string[] { "SRC Sub Beam" } };
        /// <summary>片持梁
        /// </summary>
        internal static string[][] CGirText = { new string[] { "RC Cantilever Foundation Beam","RC Cantilever Beam" },
                                new string[] { "S Cantilever H-Section", "S Cantilever Built-up H", "S Cantilever Channel", "S Cantilever Angle", "S Cantilever Lip Channel"},
                                new string[] { "SRC Cantilever Beam" } };
        /// <summary>片持小梁
        /// </summary>
        internal static string[][] CBeamText = { new string[] { "RC Cantilever Foundation Sub Beam","RC Cantilever Sub Beam" },
                                new string[] { "S Cantilever Sub H-Section", "S Cantilever Sub Built-up H", "S Cantilever Sub Channel", "S Cantilever Sub Angle", "S Cantilever Sub Lip Channel"},
                                new string[] { "SRC Cantilever Sub Beam" } };
        internal static string[][] SBraText = { new string[] { "S Brace H-Section", "S Brace Built-up H", "S Brace Box", "S Brace Built-up Box", "S Brace Circular Pipe" },
                               new string[] { "S Brace Channel", "S Brace Angle", "S Brace Lip Channel", "S Brace Flat Bar", "S Brace Round Bar" } };

        internal static string[][] SlabText = { new string[] { "RC Slab", "Deck Plate" } };

        internal static string[][] WallText = { new string[] { "Wall", "RC Parapet" } };
        //変換画面用
        internal static string[][] BaseText = {new string[] { "Rect. Footing","Tapered Rect. Footing","Triangle Footing","Equilateral Triangle Footing","Octagonal Footing"},
                               new string[] { "Strip Footing"},
                               new string[] { "Cast-in-place Pile","Precast Pile"} };
        //パラメータ追加画面用
        internal static string[][] FoundationText2 = {new string[] { "Rect. Footing","Tapered Rect. Footing","Triangle Footing","Equilateral Triangle","Octagonal Footing"},
                               new string[] { "Strip Footing"},
                               new string[] { "Cast-in-place Pile","Precast Pile"} };
        internal static string[][] SBraText1 = { new string[] { "S Brace H-Section", "S Brace Built-up H", "S Brace Box", "S Brace Built-up Box", "S Brace Circular Pipe" ,
                                                                "S Brace Channel", "S Brace Angle", "S Brace Lip Channel", "S Brace Flat Bar", "S Brace Round Bar" } };




        #region 進捗状況表示

        internal static void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            gaugeForm = pform;
            pform.lab.Visible = true;
            pform.lab.Text = labtext;
            GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);
            GaugeShow();
            pform.Refresh();
        }

        /// <summary>進捗状況を表示するForm
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

        /// <summary>進捗ゲージの表示位置サイズの設定
        /// </summary>
        /// <param name="setFlag">true=位置サイズを指定, false=初期化</param>
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
            Brush FBrsh; //ステータスバーの色
            Brush MBrsh; //文字の色
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

                // 5%ずつDoEventsを発生させて画面を更新する
                if (preGaugePer - RefreshPer > 5)
                {
                    Application.DoEvents();
                    RefreshPer = preGaugePer;
                }
            }
            g.Dispose();
        }

        #endregion



        /// <summary>
        /// フィートをメートル系単位に変換
        /// </summary>
        /// <param name="ft">フィート</param>
        /// <param name="unit">単位 =0:mm/=1:cm/=3:m</param>
        /// <returns></returns>
        internal static double ft2mm(double ft, int unit = 0, int round = 1)
        {
            int pow = (int)Math.Pow(10, unit);
            double mm = ft * 304.8 * Math.Pow(10, round);

            mm = Math.Round(mm, MidpointRounding.AwayFromZero) / Math.Pow(10, round) / pow;

            return mm;
        }
        internal static XYZ ft2mm(XYZ ft, int unit = 0)
        {
            XYZ mm = new XYZ(ft2mm(ft.X, unit),
                             ft2mm(ft.Y, unit),
                             ft2mm(ft.Z, unit));

            return mm;
        }
        /// <summary>
        /// メートル系単位をフィートに変換
        /// </summary>
        /// <param name="mm">寸法</param>
        /// <param name="unit">単位 =0:mm/=1:cm/=3:m</param>
        /// <returns></returns>
        internal static double mm2ft(double mm, int unit = 0)
        {
            int pow = (int)Math.Pow(10, unit);
            double ft = mm / 304.8 * pow;

            return ft;
        }
        internal static XYZ mm2ft(XYZ mm, int unit = 0)
        {
            XYZ ft = new XYZ(mm2ft(mm.X, unit),
                             mm2ft(mm.Y, unit),
                             mm2ft(mm.Z, unit));

            return ft;
        }


        /// <summary>
        /// XMLファイルに書かれているバージョンを読み取る
        /// </summary>
        /// <returns></returns>
        internal static string GetVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version = $"[Ver.{version}]";
            return version;
        }


    }


    class FamilyOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(
          bool familyInUse,
          out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(
          Family sharedFamily,
          bool familyInUse,
          out FamilySource source,
          out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}
