using System;
using System.Text;
using System.Collections.Generic ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    internal class Service
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private Elements _CmpElements;

        /// <summary>図形</summary>
        private Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private Parameters _CmpParameters;

        /// <summary>設定</summary>
        private Settings _CmpSettings;

        /// <summary>データテーブル コマンド</summary>
        private Entities.DtCmd _EntDtCmd;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2016/08/05 Created Ryo Kuroda</history>
        /// ================================================================================
        public
        Service(Attribute cmpAttribute,
                Elements cmpElements,
                Geometry cmpGeometry,
                Parameters cmpParameters,
                Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>ワークフロー</summary>
        ///
        /// <history>2016/08/18 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string WorkFlow()
        {
            string ret = null;

            // プロジェクト情報
            Revit.DB.ProjectInfo projInfo = _CmpElements.ProjectInfo;

            _EntDtCmd = new Entities.DtCmd(_CmpAttribute,
                                                     _CmpElements,
                                                     _CmpGeometry,
                                                     _CmpParameters,
                                                     _CmpSettings,
                                                     projInfo,
                                                     _CmpAttribute.ResourceText("IDS_SHPARAM_DEF"),
                                                     3);

            return ret;
        }

        /// ================================================================================
        /// <summary>設定</summary>
        ///
        /// <history>2016/08/30 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void Set()
        {
            _EntDtCmd.SetData();
        }

        /// ================================================================================
        /// <summary>設定ファイル情報</summary>
        ///
        /// <param name="settingFileName"     >設定ファイル名</param>
        /// <param name="settingFileDirectory">設定ファイルディレクトリ</param>
        /// <param name="trans"               >トランザクション</param>
        ///
        /// <history>2016/08/30 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetInfoFile(string settingFileName,
                         string settingFileDirectory,
                         Revit.DB.Transaction trans)
        {
            string infoDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string infoName = _CmpAttribute.ResourceText("IDS_TXT_SETTINGFILEINFONAME");

            string infoFile = infoDirectory + "\\" + infoName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (System.IO.File.Exists(infoFile))
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

                string write = settingFileDirectory + "\r\n" + settingFileName;

                trans.Start("write");
                System.IO.File.WriteAllText(infoFile, write, enc);
                trans.Commit();
            }
            else
            {
                trans.Start("create");
                System.IO.File.Create(infoFile).Close();
                trans.Commit();

                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

                string write = settingFileDirectory + "\r\n" + settingFileName;

                trans.Start("write");
                System.IO.File.WriteAllText(infoFile, write, enc);
                trans.Commit();
            }
        }

        /// ================================================================================
        /// <summary>文字列取得</summary>
        ///
        /// <param name="settingFileName"     >設定ファイル名</param>
        /// <param name="settingFileDirectory">設定ファイルパス</param>
        /// <param name="levelSortOrder"      >階記号ソート順序</param>
        ///
        /// <history>2016/08/19 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetString(ref string settingFileName,
                       ref string settingFileDirectory,
                       ref string levelSortOrder)
        {
            levelSortOrder = _EntDtCmd.LevelSortOrdeer;

            settingFileName = "";
            settingFileDirectory = "";

            // 設定ファイル情報ファイル
            string infoDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string infoName = _CmpAttribute.ResourceText("IDS_TXT_SETTINGFILEINFONAME");

            string infoFile = infoDirectory + "\\" + infoName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (System.IO.File.Exists(infoFile))
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");
                string[] strAry = System.IO.File.ReadAllLines(infoFile, enc);

                if (strAry.Length > 0)
                {
                    settingFileDirectory = strAry[0];
                }
                if (strAry.Length > 1)
                {
                    settingFileName = strAry[1];
                }

                // 初回設定ファイル
                if (settingFileDirectory == "Default")
                {
                    settingFileDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
                    settingFileName = _CmpAttribute.ResourceText("IDS_TXT_SETTINGFILENAME");
                }

                string fullName = settingFileDirectory + settingFileName;
                if (!System.IO.File.Exists(fullName))
                {
                    settingFileName = "";
                    settingFileDirectory = "";
                }
            }
        }

        /// ================================================================================
        /// <summary>設定ファイル書き出し - 上書き保存</summary>
        ///
        /// <history>2016/08/30 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void OverWriteSettingValues(Collections.Generic.IList<string> commonAry,
                                    Collections.Generic.IList<string> columnAry,
                                    Collections.Generic.IList<string> subItemPostAry,
                                    Collections.Generic.IList<string> beamAry,
                                    Collections.Generic.IList<string> subItemBeamAry,
                                    Collections.Generic.IList<string> braceAry)
        {
            string settingFileName = "";
            string settingFileDirectory = "";
            string levelSortOrder = "";

            GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (settingFileDirectory != "" && settingFileName != "" && settingFileDirectory != null && settingFileName != null &&
                System.IO.File.Exists(settingFileDirectory + settingFileName))
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");
                string[] strAry = System.IO.File.ReadAllLines(settingFileDirectory + settingFileName, enc);

                Collections.Generic.IList<string> mappingAry_LevelSort = new Collections.Generic.List<string>();

                string write = "";

                foreach (string str in commonAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in columnAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in subItemPostAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in beamAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in subItemBeamAry)
                {
                    write += str + Environment.NewLine;
                }

                write += Environment.NewLine;
                foreach (string str in braceAry)
                {
                    write += str + Environment.NewLine;
                }

                // 書き出し(ファイルが存在するときは上書き)
                System.IO.File.WriteAllText(settingFileDirectory + settingFileName, write, enc);
            }
            else
            {
                SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);
            }
        }

        /// ================================================================================
        /// <summary>設定ファイル書き出し - 名前を付けて保存</summary>
        ///
        /// <history>2016/08/30 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void SaveAsSettingValues(Collections.Generic.IList<string> commonAry,
                                 Collections.Generic.IList<string> columnAry,
                                 Collections.Generic.IList<string> subItemPostAry,
                                 Collections.Generic.IList<string> beamAry,
                                 Collections.Generic.IList<string> subItemBeamAry,
                                 Collections.Generic.IList<string> braceAry)
        {
            string settingFileName = "";
            string settingFileDirectory = "";
            string levelSortOrder = "";

            GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);

            System.Windows.Forms.SaveFileDialog saveFileDlg = new System.Windows.Forms.SaveFileDialog();
            saveFileDlg.InitialDirectory = settingFileDirectory;
            saveFileDlg.Filter = "Text File (*.txt)|*.txt";

            if (saveFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string saveFileName = saveFileDlg.FileName;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

                string write = "";

                foreach (string str in commonAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in columnAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in subItemPostAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in beamAry)
                {
                    write += str + Environment.NewLine;
                }
                write += Environment.NewLine;
                foreach (string str in subItemBeamAry)
                {
                    write += str + Environment.NewLine;
                }

                write += Environment.NewLine;
                foreach (string str in braceAry)
                {
                    write += str + Environment.NewLine;
                }

                // 書き出し(ファイルが存在するときは上書き)
                System.IO.File.WriteAllText(saveFileName, write, enc);
            }
        }

        /// ================================================================================
        /// <summary>ディクショナリ値の重複判定</summary>
        ///
        /// <history><p>2016/08/23 Created  GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string OverlapDicValues(Collections.Generic.IDictionary<string, string> dicStrs)
        {
            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();
            string retStr = "";

            Collections.Generic.List<string> list = new Collections.Generic.List<string>();
            foreach (string s in dicStrs.Values)
            {
                list.Add(s);
            }

            // 次と比較
            for (int i = 0; i < list.Count; ++i)
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    if (i == j || i > j)
                    {
                        continue;
                    }

                    string str1 = list[i];
                    string str2 = list[j];

                    if (str1 == str2)
                    {
                        if (ret.Contains(str1) == false)
                        {
                            ret.Add(str1);

                            if (retStr != "")
                            {
                                retStr += "\r\n";
                            }

                            retStr += str1;
                        }
                    }
                }
            }

            return retStr;
        }

        /// ================================================================================
        /// <summary>ディクショナリ値の重複判定</summary>
        ///
        /// <param name="dicHColumn"        >鉄骨 H形鋼</param>
        /// <param name="dicRectColumn"     >鉄骨 角形鋼管</param>
        /// <param name="dicRoundColumn"    >鉄骨 鋼管</param>
        /// <param name="dicCFTRectColumn"  >CFT 角形鋼管</param>
        /// <param name="dicCFTRoundColumn" >CFT 鋼管</param>
        ///
        /// <history><p>2016/09/01 Created  CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/26 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public string OverlapDicValues_Column(Collections.Generic.IDictionary<string, string> dicHColumn,
                                       Collections.Generic.IDictionary<string, string> dicRectColumn,
                                       Collections.Generic.IDictionary<string, string> dicRoundColumn,
                                       Collections.Generic.IDictionary<string, string> dicCFTRectColumn,
                                       Collections.Generic.IDictionary<string, string> dicCFTRoundColumn,
                                        Collections.Generic.IDictionary<string, string> dicLColumn,
                                        Collections.Generic.IDictionary<string, string> dicUColumn,
                                        Collections.Generic.IDictionary<string, string> dicCColumn,
                                        Collections.Generic.IDictionary<string, string> dicFBColumn,
                                        Collections.Generic.IDictionary<string, string> dicMColumn,
                                        Collections.Generic.IDictionary<string, string> dicTColumn)
        {
            string ret = "";

            // 鉄骨 H形鋼
            string overlapValues = OverlapDicValues(dicHColumn);

            if (overlapValues != "")
            {
                ret = overlapValues;
            }

            // 鉄骨 角形鋼管
            overlapValues = OverlapDicValues(dicRectColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            // 鉄骨 鋼管
            overlapValues = OverlapDicValues(dicRoundColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            // CFT 角形鋼管
            overlapValues = OverlapDicValues(dicCFTRectColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            // CFT 鋼管
            overlapValues = OverlapDicValues(dicCFTRoundColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            //////////////////////////////////////////////////////////////////////////
            overlapValues = OverlapDicValues(dicLColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            overlapValues = OverlapDicValues(dicCColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            overlapValues = OverlapDicValues(dicUColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            overlapValues = OverlapDicValues(dicFBColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            overlapValues = OverlapDicValues(dicMColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            overlapValues = OverlapDicValues(dicTColumn);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            //////////////////////////////////////////////////////////////////////////
            return ret;
        }

        /// ================================================================================
        /// <summary>ディクショナリ値の重複判定</summary>
        ///
        /// <param name="dicGirder"     >梁</param>
        /// <param name="dicCantiGirder">片持ち梁</param>
        ///
        /// <history><p>2016/09/01 Created  CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/26 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public string OverlapDicValues_Beam(Collections.Generic.IDictionary<string, string> dicGirder,
                                     Collections.Generic.IDictionary<string, string> dicCantiGirder)
        {
            string ret = "";

            // 梁
            string overlapValues = OverlapDicValues(dicGirder);

            if (overlapValues != "")
            {
                ret = overlapValues;
            }

            // 片持ち梁
            overlapValues = OverlapDicValues(dicCantiGirder);

            if (overlapValues != "")
            {
                if (ret != "")
                {
                    ret += "\r\n";
                }

                ret += overlapValues;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Excel確認</summary>
        ///
        /// <history>2016/08/18 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool IsExcelInComputer()
        {
            // 戻り値
            bool ret = false;

            try
            {
                Type type = Type.GetTypeFromProgID("Excel.Application");

                // Wordの場合
                //System.Type wordType = System.Type.GetTypeFromProgID("Word.Application");

                if (type == null)
                {
                    ret = false;
                }
                else if (type != null)
                {
                    ret = true;
                }
            }
            catch
            {
                return ret;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>パラメータ設定 - 柱</summary>
        ///
        /// <history>2016/09/16 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SetParameters(bool isColumn)
        {
            // 戻り値
            string ret = "";

            // パラメータファイル取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamNames = _CmpParameters.GetParamNames();

            if (allParamNames == null ||
                allParamNames.Count == 0)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILE");

                return ret;
            }

            // S柱 H形鋼
            Collections.Generic.IDictionary<string, string> dicHColumn = null;
            // S柱 角形鋼管
            Collections.Generic.IDictionary<string, string> dicRectColumn = null;
            // S柱 鋼管
            Collections.Generic.IDictionary<string, string> dicRoundColumn = null;

            // CFT柱 角形鋼管
            Collections.Generic.IDictionary<string, string> dicCFTRectColumn = null;
            // CFT柱 鋼管
            Collections.Generic.IDictionary<string, string> dicCFTRoundColumn = null;

            //////////////////////////////////////////////////////////////////////////
            Collections.Generic.IDictionary<string, string> dicLColumn = null;
            Collections.Generic.IDictionary<string, string> dicUColumn = null;
            Collections.Generic.IDictionary<string, string> dicCColumn = null;
            Collections.Generic.IDictionary<string, string> dicFBColumn = null;
            Collections.Generic.IDictionary<string, string> dicMColumn = null;
            Collections.Generic.IDictionary<string, string> dicTColumn = null;

            Collections.Generic.IDictionary<string, string> dicLGirder = null;
            Collections.Generic.IDictionary<string, string> dicUGirder = null;
            Collections.Generic.IDictionary<string, string> dicCGirder = null;
            Collections.Generic.IDictionary<string, string> dicFBGirder = null;
            Collections.Generic.IDictionary<string, string> dicMGirder = null;
            Collections.Generic.IDictionary<string, string> dicTGirder = null;
            Collections.Generic.IDictionary<string, string> dicRectGirder = null;
            Collections.Generic.IDictionary<string, string> dicPGirder = null;

            //////////////////////////////////////////////////////////////////////////

            // S梁
            Collections.Generic.IDictionary<string, string> dicGirder = null;
            // S片持ち梁
            Collections.Generic.IDictionary<string, string> dicCantiGirder = null;

            // 各パラメータ名取得
            bool getName = _CmpParameters.GetParamNames(ref dicHColumn,
                                                        ref dicRectColumn,
                                                        ref dicRoundColumn,
                                                        ref dicCFTRectColumn,
                                                        ref dicCFTRoundColumn,
                                                        ref dicLColumn,
                                                        ref dicUColumn,
                                                        ref dicCColumn,
                                                        ref dicFBColumn,
                                                        ref dicMColumn,
                                                        ref dicTColumn,
                                                        ref dicGirder,
                                                        ref dicCantiGirder,
                                                        ref dicLGirder,
                                                        ref dicUGirder,
                                                        ref dicCGirder,
                                                        ref dicFBGirder,
                                                        ref dicMGirder,
                                                        ref dicTGirder,
                                                        ref dicRectGirder,
                                                        ref dicPGirder);

            if (dicHColumn == null || dicHColumn.Count == 0 ||
                dicRectColumn == null || dicRectColumn.Count == 0 ||
                dicRoundColumn == null || dicRoundColumn.Count == 0 ||
                dicCFTRectColumn == null || dicCFTRectColumn.Count == 0 ||
                dicCFTRoundColumn == null || dicCFTRoundColumn.Count == 0 ||

                dicLColumn == null || dicLColumn.Count == 0 ||
                dicUColumn == null || dicUColumn.Count == 0 ||
                //dicCColumn == null || dicCColumn.Count == 0 ||
                // dicFBColumn == null || dicFBColumn.Count == 0 ||
                // dicMColumn == null || dicMColumn.Count == 0 ||
                dicTColumn == null || dicTColumn.Count == 0 ||

                dicGirder == null || dicGirder.Count == 0 ||
                dicCantiGirder == null || dicCantiGirder.Count == 0 ||

                dicLGirder == null || dicLGirder.Count == 0 ||
                dicUGirder == null || dicUGirder.Count == 0 ||
                dicCGirder == null || dicCGirder.Count == 0 ||
                dicFBGirder == null || dicFBGirder.Count == 0 ||
                dicMGirder == null || dicMGirder.Count == 0 ||
                //dicTGirder == null || dicTGirder.Count == 0 ||
                dicRectGirder == null || dicRectGirder.Count == 0 ||
                dicPGirder == null || dicPGirder.Count == 0
                )
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILEVALUE");

                return ret;
            }

            string overlapValues = string.Empty;

            if (isColumn)
            {
                // パラメータ名の重複判定
                overlapValues = OverlapDicValues_Column(dicHColumn,
                                               dicRectColumn,
                                               dicRoundColumn,
                                               dicCFTRectColumn,
                                               dicCFTRoundColumn,
                                               dicLColumn,
                                               dicUColumn,
                                               dicCColumn,
                                                dicFBColumn,
                                                dicMColumn,
                                                dicTColumn
                                               );
            }
            else
            {
                // パラメータ名の重複判定
                overlapValues = OverlapDicValues_Beam(dicGirder,
                                                            dicCantiGirder);
            }

            if (overlapValues != "")
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_SETTINGOVERLAP") + "\r\n\r\n" + overlapValues;

                return ret;
            }

            // パラメータ名設定
            _CmpParameters.SetParamNames(dicHColumn,
                                        dicRectColumn,
                                        dicRoundColumn,
                                        dicCFTRectColumn,
                                        dicCFTRoundColumn,
                                        dicLColumn,
                                        dicUColumn,
                                        dicCColumn,
                                        dicFBColumn,
                                        dicMColumn,
                                        dicTColumn,
                                        dicGirder,
                                        dicCantiGirder,
                                        dicLGirder,
                                        dicUGirder,
                                        dicCGirder,
                                        dicFBGirder,
                                        dicMGirder,
                                        dicTGirder,
                                        dicRectGirder,
                                        dicPGirder);

            return ret;
        }

        /// ================================================================================
        /// <summary>柱の振り分け (CFT 角形鋼管 > CFT 鋼管 > 鉄骨 H形鋼 > 鉄骨 角形鋼管 > 鉄骨 鋼管)</summary>
        ///
        /// <param name="columns"       >柱</param>
        /// <param name="steelHAry"     >鉄骨 H形鋼</param>
        /// <param name="steelRectAry"  >鉄骨 角形鋼管</param>
        /// <param name="steelRoundAry" >鉄骨 鋼管</param>
        /// <param name="cftRectAry"    >CFT 角形鋼管</param>
        /// <param name="cftRoundAry"   >CFT 鋼管</param>
        ///
        /// <history><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2016/09/27 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void ColumnDivision(Collections.Generic.IList<Revit.DB.FamilySymbol> columns,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry,
                             ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry,
                              ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry,
                               ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry,
                                ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry,
                                 ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry,
                                  ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry)
        {
            if (columns == null)
            {
                columns = _CmpElements.SteelColumnFamSyms;
            }

            var emptyStructuralSectionShapeItemsString = string.Empty ;
            var emptyStructuralSectionShapeFamilyNames = new List<string>() ;
            
            foreach (Revit.DB.FamilySymbol famSym in columns)
            {
                // 断面形状が未設定の場合、あとでログを表示する。
                var parGirderMark = famSym.Family.get_Parameter( BuiltInParameter.STRUCTURAL_SECTION_SHAPE ) ;
                if ( parGirderMark != null && parGirderMark.AsInteger() == 0 ) {
                    if ( ! emptyStructuralSectionShapeFamilyNames.Contains( famSym.Family.Name ) ) {
                        emptyStructuralSectionShapeFamilyNames.Add( famSym.Family.Name ) ;
                        emptyStructuralSectionShapeItemsString = $"{emptyStructuralSectionShapeItemsString}{famSym.Family.Name}\n" ;
                    }
                    continue;
                }
                
                // フラグ
                bool flagSteelH = false;
                bool flagSteelRect = false;
                bool flagSteelRound = false;
                bool flagCFTRect = false;
                bool flagCFTRound = false;

                //////////////////////////////////////////////////////////////////////////
                bool flagSteelL = false;
                bool flagSteelU = false;
                bool flagSteelC = false;
                bool flagSteelFB = false;
                bool flagSteelM = false;
                bool flagSteelT = false;

                Revit.DB.Parameter p1 = famSym.LookupParameter(_CmpParameters.LColumnStrcMaterial);
                Revit.DB.Parameter p2 = famSym.LookupParameter(_CmpParameters.LColumnSyubetsu);
                Revit.DB.Parameter p3 = famSym.LookupParameter(_CmpParameters.LColumnSei);
                Revit.DB.Parameter p4 = famSym.LookupParameter(_CmpParameters.LColumnHaba);
                Revit.DB.Parameter p5 = famSym.LookupParameter(_CmpParameters.LColumnDirThick);
                Revit.DB.Parameter p6 = famSym.LookupParameter(_CmpParameters.LColumnWidthThick);
                Revit.DB.Parameter p7 = famSym.LookupParameter(_CmpParameters.LColumnFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 11)
                        {
                            flagSteelL = true;
                        }
                    }
                }

                p1 = famSym.LookupParameter(_CmpParameters.UColumnStrcMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.UColumnSyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.UColumnSei);
                p4 = famSym.LookupParameter(_CmpParameters.UColumnHaba);
                p5 = famSym.LookupParameter(_CmpParameters.UColumnWebAtsu);
                p6 = famSym.LookupParameter(_CmpParameters.UColumnFlangeAtsu);
                p7 = famSym.LookupParameter(_CmpParameters.UColumnFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 10)
                        {
                            flagSteelU = true;
                        }
                    }
                }

                p1 = famSym.LookupParameter(_CmpParameters.CColumnStrcMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.CColumnSyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.CColumnSei);
                p4 = famSym.LookupParameter(_CmpParameters.CColumnHaba);
                p5 = famSym.LookupParameter(_CmpParameters.CColumnLipLength);
                p6 = famSym.LookupParameter(_CmpParameters.CColumnBoardThick);
                p7 = famSym.LookupParameter(_CmpParameters.CColumnFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 21)
                        {
                            flagSteelC = true;
                        }
                    }
                }

                p1 = famSym.LookupParameter(_CmpParameters.FBColumnStrcMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.FBColumnSyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.FBColumnWidth);
                p4 = famSym.LookupParameter(_CmpParameters.FBColumnBoardThick);
                p5 = famSym.LookupParameter(_CmpParameters.FBColumnFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 31)
                        {
                            flagSteelFB = true;
                        }
                    }
                }

                p1 = famSym.LookupParameter(_CmpParameters.MColumnStrcMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.MColumnSyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.MColumnDiameter);
                p4 = famSym.LookupParameter(_CmpParameters.MColumnFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 13)
                        {
                            flagSteelM = true;
                        }
                    }
                }

                p1 = famSym.LookupParameter(_CmpParameters.TColumnWebMat);
                p2 = famSym.LookupParameter(_CmpParameters.TColumnFlangeMat);
                p3 = famSym.LookupParameter(_CmpParameters.TColumnSei);
                p4 = famSym.LookupParameter(_CmpParameters.TColumnHaba);
                p5 = famSym.LookupParameter(_CmpParameters.TColumnWebAtsu);
                p6 = famSym.LookupParameter(_CmpParameters.TColumnFlangeAtsu);
                p7 = famSym.LookupParameter(_CmpParameters.TColumnFugo);
                var p8 = famSym.LookupParameter(_CmpParameters.TColumnSyubetsu);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null &&
                    p8 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 17)
                        {
                            flagSteelT = true;
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////

                // 鉄骨 H形鋼
                Revit.DB.Parameter parSteelHSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnHSyubetsu);
                Revit.DB.Parameter parSteelHWebMat = famSym.LookupParameter(_CmpParameters.SColumnHWebMaterial);
                Revit.DB.Parameter parSteelHFlangeMat = famSym.LookupParameter(_CmpParameters.SColumnHFlangeMaterial);
                Revit.DB.Parameter parSteelHHaba = famSym.LookupParameter(_CmpParameters.SColumnHHaba);
                Revit.DB.Parameter parSteelHSei = famSym.LookupParameter(_CmpParameters.SColumnHSei);
                Revit.DB.Parameter parSteelHWebAtsu = famSym.LookupParameter(_CmpParameters.SColumnHWebAtsu);
                Revit.DB.Parameter parSteelHFlangeAtsu = famSym.LookupParameter(_CmpParameters.SColumnHFlangeAtsu);
                Revit.DB.Parameter parSteelHFugo = famSym.LookupParameter(_CmpParameters.SColumnHFugo);
                Revit.DB.Parameter parSteelHFillet = famSym.LookupParameter(_CmpParameters.SColumnHFillet);

                if (parSteelHWebMat != null &&
                    parSteelHFlangeMat != null &&
                    parSteelHSyubetsu != null &&
                    parSteelHHaba != null &&
                    parSteelHSei != null &&
                    parSteelHWebAtsu != null &&
                    parSteelHFlangeAtsu != null &&
                    parSteelHFugo != null &&
                    parSteelHFillet != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 6)
                        {
                            flagSteelH = true;
                        }
                    }
                }

                // 鉄骨 角形鋼管
                Revit.DB.Parameter parSteelRectMat = famSym.LookupParameter(_CmpParameters.SColumnRectStructuralMaterial);
                Revit.DB.Parameter parSteelRectSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRectSyubetsu);
                Revit.DB.Parameter parSteelRectHaba = famSym.LookupParameter(_CmpParameters.SColumnRectHaba);
                Revit.DB.Parameter parSteelRectSei = famSym.LookupParameter(_CmpParameters.SColumnRectSei);
                Revit.DB.Parameter parSteelRectAtsu = famSym.LookupParameter(_CmpParameters.SColumnRectItaAtsu);
                Revit.DB.Parameter parSteelRectFugo = famSym.LookupParameter(_CmpParameters.SColumnRectFugo);
                Revit.DB.Parameter parSteelRectFillet = famSym.LookupParameter(_CmpParameters.SColumnRectFillet);
                Revit.DB.Parameter parSteelRectt2 = null;
                if (parSteelRectFillet!=null && parSteelRectFillet.AsDouble() == 0)
                    parSteelRectt2 = famSym.LookupParameter(_CmpParameters.SColumnRectT2);

                if (parSteelRectFillet!=null && parSteelRectFillet.AsDouble() != 0)
                {
                    if (parSteelRectMat != null &&
                        parSteelRectSyubetsu != null &&
                        parSteelRectHaba != null &&
                        parSteelRectSei != null &&
                        parSteelRectAtsu != null &&
                        parSteelRectFugo != null &&
                        parSteelRectFillet != null)
                    {
                        var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                        if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                        {
                            if (para_shape.AsInteger() == 14)
                            {
                                flagSteelRect = true;
                            }
                        }
                    }
                }
                else
                {
                    if (parSteelRectMat != null &&
                        parSteelRectSyubetsu != null &&
                        parSteelRectHaba != null &&
                        parSteelRectSei != null &&
                        parSteelRectAtsu != null &&
                        parSteelRectFugo != null &&
                        parSteelRectFillet != null &&
                        parSteelRectt2 != null)
                    {
                        var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                        if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                        {
                            if (para_shape.AsInteger() == 14)
                            {
                                flagSteelRect = true;
                            }
                        }
                    }
                }

                // 鉄骨 鋼管
                Revit.DB.Parameter parSteelRoundMat = famSym.LookupParameter(_CmpParameters.SColumnRoundStructuralMaterial);
                Revit.DB.Parameter parSteelRoundSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);
                Revit.DB.Parameter parSteelRoundDiameter = famSym.LookupParameter(_CmpParameters.SColumnRoundDiameter);
                Revit.DB.Parameter parSteelRoundAtsu = famSym.LookupParameter(_CmpParameters.SColumnRoundItaAtsu);
                Revit.DB.Parameter parSteelRoundFugo = famSym.LookupParameter(_CmpParameters.SColumnRoundFugo);

                if (parSteelRoundMat != null &&
                    parSteelRoundSyubetsu != null &&
                    parSteelRoundDiameter != null &&
                    parSteelRoundAtsu != null &&
                    parSteelRoundFugo != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 15)
                        {
                            flagSteelRound = true;
                        }
                    }
                }

                // CFT 角形鋼管
                Revit.DB.Parameter parCFTRectMat = famSym.LookupParameter(_CmpParameters.CFTColumnRectStructuralMaterial);
                Revit.DB.Parameter parCFTRectConcMat = famSym.LookupParameter(_CmpParameters.CFTColumnRectConcreteMaterial);
                Revit.DB.Parameter parCFTRectSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRectSyubetsu);
                Revit.DB.Parameter parCFTRectHaba = famSym.LookupParameter(_CmpParameters.CFTColumnRectHaba);
                Revit.DB.Parameter parCFTRectSei = famSym.LookupParameter(_CmpParameters.CFTColumnRectSei);
                Revit.DB.Parameter parCFTRectAtsu = famSym.LookupParameter(_CmpParameters.CFTColumnRectItaAtsu);
                Revit.DB.Parameter parCFTRectFugo = famSym.LookupParameter(_CmpParameters.CFTColumnRectFugo);
                //Revit.DB.Parameter parCFTRectT2 = famSym.LookupParameter(_CmpParameters.CFTColumnRoundT2);
                Revit.DB.Parameter parCFTRectFillet = famSym.LookupParameter(_CmpParameters.CFTColumnRectFillet);
                Revit.DB.Parameter parCFTRectT2 = null;
                if (parSteelRectFillet != null && parSteelRectFillet.AsDouble() == 0)
                    parCFTRectT2 = famSym.LookupParameter(_CmpParameters.CFTColumnRectT2);

                if (parSteelRectFillet != null && parSteelRectFillet.AsDouble() != 0)
                {
                    if (parCFTRectMat != null &&
                    parCFTRectConcMat != null &&
                    parCFTRectSyubetsu != null &&
                    parCFTRectHaba != null &&
                    parCFTRectSei != null &&
                    parCFTRectAtsu != null &&
                    parCFTRectFugo != null &&
                    parCFTRectFillet != null)
                    {
                        var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                        if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                        {
                            if (para_shape.AsInteger() == 14)
                            {
                                flagCFTRect = true;
                            }
                        }
                    }
                }
                else
                {
                    if (parCFTRectMat != null &&
                    parCFTRectConcMat != null &&
                    parCFTRectSyubetsu != null &&
                    parCFTRectHaba != null &&
                    parCFTRectSei != null &&
                    parCFTRectAtsu != null &&
                    parCFTRectFugo != null &&
                    parCFTRectFillet != null &&
                    parCFTRectT2!= null)
                    {
                        //ToDo: 他の条件に合致しない場合に円形の柱と判定しているのでRevitの形状追加時に注意が必要
                        var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                        if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                        {
                            if (para_shape.AsInteger() == 14)
                            {
                                flagCFTRect = true;
                            }
                        }
                    }
                }

                // CFT 鋼管
                Revit.DB.Parameter parCFTRoundMat = famSym.LookupParameter(_CmpParameters.SColumnRoundStructuralMaterial);
                Revit.DB.Parameter parCFTRoundConcMat = famSym.LookupParameter(_CmpParameters.CFTColumnRoundConcreteMaterial);
                Revit.DB.Parameter parCFTRoundSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);
                Revit.DB.Parameter parCFTRoundDiameter = famSym.LookupParameter(_CmpParameters.SColumnRoundDiameter);
                Revit.DB.Parameter parCFTRoundAtsu = famSym.LookupParameter(_CmpParameters.SColumnRoundItaAtsu);
                Revit.DB.Parameter parCFTRoundFugo = famSym.LookupParameter(_CmpParameters.SColumnRoundFugo);

                if (parCFTRoundMat != null &&
                    parCFTRoundConcMat != null &&
                    parCFTRoundSyubetsu != null &&
                    parCFTRoundDiameter != null &&
                    parCFTRoundAtsu != null &&
                    parCFTRoundFugo != null)
                {
                    flagCFTRound = true;
                }
                //////////////////////////////////////////////////////////////////////////
                if (flagSteelH)
                {
                    steelHAry.Add(famSym);
                }
                else if (flagSteelL)
                {
                    steelLAry.Add(famSym);
                }
                else if (flagSteelU)
                {
                    steelUAry.Add(famSym);
                }
                else if (flagSteelT)
                {
                    steelTAry.Add(famSym);
                }
                else if (flagCFTRect)
                {
                    cftRectAry.Add(famSym);
                }
                else if (flagSteelRect)
                {
                    steelRectAry.Add(famSym);
                }
                else if (flagCFTRound)
                {
                    cftRoundAry.Add(famSym);
                }
                else if (flagSteelRound)
                {
                    steelRoundAry.Add(famSym);
                }
                else if (flagSteelC)
                {
                    steelCAry.Add(famSym);
                }
                else if (flagSteelFB)
                {
                    steelFBAry.Add(famSym);
                }
                else if (flagSteelM)
                {
                    steelMAry.Add(famSym);
                }
                //////////////////////////////////////////////////////////////////////////

                //                 // CFT 角形鋼管
                //                 if (flagCFTRect)
                //                 {
                //                     cftRectAry.Add(famSym);
                //                 }
                //                 // CFT 鋼管
                //                 else if (flagCFTRound)
                //                 {
                //                     cftRoundAry.Add(famSym);
                //                 }
                //                 else
                //                 {
                //                     // 鉄骨 H形鋼管
                //                     if (flagSteelH)
                //                     {
                //                         steelHAry.Add(famSym);
                //                     }
                //                     // 鉄骨 角形鋼管
                //                     else if (flagSteelRect)
                //                     {
                //                         steelRectAry.Add(famSym);
                //                     }
                //                     //  鉄骨 鋼管
                //                     else if (flagSteelRound)
                //                     {
                //                         steelRoundAry.Add(famSym);
                //                     }
                //                 }
            }

            if ( emptyStructuralSectionShapeItemsString != string.Empty ) {
                emptyStructuralSectionShapeItemsString = $"{_CmpAttribute.ResourceText( "IDS_TXT_STRUCTURAL_SECTION_SHAPE_NOT_DEFINED_DIALOG" )}\n\n{emptyStructuralSectionShapeItemsString.TrimEnd( '\n' )}" ;
                TaskDialog.Show( _CmpAttribute.ResourceText( "IDS_TXT_STRUCTURAL_SECTION_SHAPE_NOT_DEFINED" ), emptyStructuralSectionShapeItemsString , TaskDialogCommonButtons.Close ) ;
            }
        }

        /// ================================================================================
        /// <summary>梁の振り分け (梁 > 片持ち梁)</summary>
        ///
        /// <param name="girders"       >梁</param>
        /// <param name="girderAry"     >S梁</param>
        /// <param name="cantiGirderAry">S片持ち梁</param>
        ///
        /// <history><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2016/09/27 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GirderDivision(Collections.Generic.IList<Revit.DB.FamilySymbol> girders,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry,
                             ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry,
                               ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry,
                               ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry,
                                ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry,
                                 ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry,
                                  ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry,
                            ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry,
                             ref Collections.Generic.IList<Revit.DB.FamilySymbol> steelPAry
                  )
        {
            if (girders == null)
            {
                girders = _CmpElements.SteelGirderFamSyms;
            }
            
            var emptyStructuralSectionShapeItemsString = string.Empty ;
            var emptyStructuralSectionShapeFamilyNames = new List<string>() ;

            foreach (Revit.DB.FamilySymbol famSym in girders)
            {
                // 断面形状が未設定の場合、あとでログを表示する。
                var parGirderMark = famSym.Family.get_Parameter( BuiltInParameter.STRUCTURAL_SECTION_SHAPE ) ;
                if ( parGirderMark != null && parGirderMark.AsInteger() == 0 ) {
                    if ( ! emptyStructuralSectionShapeFamilyNames.Contains( famSym.Family.Name ) ) {
                        emptyStructuralSectionShapeFamilyNames.Add( famSym.Family.Name ) ;
                        emptyStructuralSectionShapeItemsString = $"{emptyStructuralSectionShapeItemsString}{famSym.Family.Name}\n" ;
                    }
                    continue;
                }
                
                // フラグ
                bool flagGirder = false;
                bool flagCantiGirder = false;

                //////////////////////////////////////////////////////////////////////////
                bool flagSteelL = false;
                bool flagSteelU = false;
                bool flagSteelC = false;
                bool flagSteelFB = false;
                bool flagSteelM = false;
                bool flagSteelT = false;
                bool flagSteelRect = false;
                bool flagSteelP = false;
                //////////////////////////////////////////////////////////////////////////

                // 梁
                Revit.DB.Parameter parGirderWebMat_S = famSym.LookupParameter(_CmpParameters.GirderWebMaterial_S);
                Revit.DB.Parameter parGirderFlangeMat_S = famSym.LookupParameter(_CmpParameters.GirderFlangeMaterial_S);
                Revit.DB.Parameter parGirderWebMat_C = famSym.LookupParameter(_CmpParameters.GirderWebMaterial_C);
                Revit.DB.Parameter parGirderFlangeMat_C = famSym.LookupParameter(_CmpParameters.GirderFlangeMaterial_C);
                Revit.DB.Parameter parGirderWebMat_E = famSym.LookupParameter(_CmpParameters.GirderWebMaterial_E);
                Revit.DB.Parameter parGirderFlangeMat_E = famSym.LookupParameter(_CmpParameters.GirderFlangeMaterial_E);
                Revit.DB.Parameter parGirderSyubetsu = famSym.LookupParameter(_CmpParameters.GirderSyubetsu);
                Revit.DB.Parameter parGirderSei_S = famSym.LookupParameter(_CmpParameters.GirderSei_S);
                Revit.DB.Parameter parGirderHaba_S = famSym.LookupParameter(_CmpParameters.GirderHaba_S);
                Revit.DB.Parameter parGirderWebAtsu_S = famSym.LookupParameter(_CmpParameters.GirderWebAtsu_S);
                Revit.DB.Parameter parGirderFlangeAtsu_S = famSym.LookupParameter(_CmpParameters.GirderFlangeAtsu_S);
                Revit.DB.Parameter parGirderSei_C = famSym.LookupParameter(_CmpParameters.GirderSei_C);
                Revit.DB.Parameter parGirderHaba_C = famSym.LookupParameter(_CmpParameters.GirderHaba_C);
                Revit.DB.Parameter parGirderWebAtsu_C = famSym.LookupParameter(_CmpParameters.GirderWebAtsu_C);
                Revit.DB.Parameter parGirderFlangeAtsu_C = famSym.LookupParameter(_CmpParameters.GirderFlangeAtsu_C);
                Revit.DB.Parameter parGirderSei_E = famSym.LookupParameter(_CmpParameters.GirderSei_E);
                Revit.DB.Parameter parGirderHaba_E = famSym.LookupParameter(_CmpParameters.GirderHaba_E);
                Revit.DB.Parameter parGirderWebAtsu_E = famSym.LookupParameter(_CmpParameters.GirderWebAtsu_E);
                Revit.DB.Parameter parGirderFlangeAtsu_E = famSym.LookupParameter(_CmpParameters.GirderFlangeAtsu_E);
                Revit.DB.Parameter parGirderFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

                Revit.DB.Parameter parGirderFillet_S = famSym.LookupParameter(_CmpParameters.GirderFillet_S);
                Revit.DB.Parameter parGirderFillet_C = famSym.LookupParameter(_CmpParameters.GirderFillet_C);
                Revit.DB.Parameter parGirderFillet_E = famSym.LookupParameter(_CmpParameters.GirderFillet_E);

                if (parGirderWebMat_S != null &&
                    parGirderFlangeMat_S != null &&
                    parGirderWebMat_C != null &&
                    parGirderFlangeMat_C != null &&
                    parGirderWebMat_E != null &&
                    parGirderFlangeMat_E != null &&
                    parGirderSyubetsu != null &&
                    parGirderSei_S != null &&
                    parGirderHaba_S != null &&
                    parGirderWebAtsu_S != null &&
                    parGirderFlangeAtsu_S != null &&
                    parGirderSei_C != null &&
                    parGirderHaba_C != null &&
                    parGirderWebAtsu_C != null &&
                    parGirderFlangeAtsu_C != null &&
                    parGirderSei_E != null &&
                    parGirderHaba_E != null &&
                    parGirderWebAtsu_E != null &&
                    parGirderFlangeAtsu_E != null &&
                    parGirderFugo != null &&

                    parGirderFillet_S != null &&
                    parGirderFillet_C != null &&
                    parGirderFillet_E != null

                    )
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 6)
                        {
                            flagGirder = true;
                        }
                    }
                }

                // 片持ち梁
                Revit.DB.Parameter parCantiGirderWebMat_S = famSym.LookupParameter(_CmpParameters.CantiGirderWebMaterial_S);
                Revit.DB.Parameter parCantiGirderFlangeMat_S = famSym.LookupParameter(_CmpParameters.CantiGirderFlangeMaterial_S);
                Revit.DB.Parameter parCantiGirderWebMat_E = famSym.LookupParameter(_CmpParameters.CantiGirderWebMaterial_E);
                Revit.DB.Parameter parCantiGirderFlangeMat_E = famSym.LookupParameter(_CmpParameters.CantiGirderFlangeMaterial_E);
                Revit.DB.Parameter parCantiGirderSyubetsu = famSym.LookupParameter(_CmpParameters.CantiGirderSyubetsu);
                Revit.DB.Parameter parCantiGirderSei_S = famSym.LookupParameter(_CmpParameters.CantiGirderSei_S);
                Revit.DB.Parameter parCantiGirderHaba_S = famSym.LookupParameter(_CmpParameters.CantiGirderHaba_S);
                Revit.DB.Parameter parCantiGirderWebAtsu_S = famSym.LookupParameter(_CmpParameters.CantiGirderWebAtsu_S);
                Revit.DB.Parameter parCantiGirderFlangeAtsu_S = famSym.LookupParameter(_CmpParameters.CantiGirderFlangeAtsu_S);
                Revit.DB.Parameter parCantiGirderSei_E = famSym.LookupParameter(_CmpParameters.CantiGirderSei_E);
                Revit.DB.Parameter parCantiGirderHaba_E = famSym.LookupParameter(_CmpParameters.CantiGirderHaba_E);
                Revit.DB.Parameter parCantiGirderWebAtsu_E = famSym.LookupParameter(_CmpParameters.CantiGirderWebAtsu_E);
                Revit.DB.Parameter parCantiGirderFlangeAtsu_E = famSym.LookupParameter(_CmpParameters.CantiGirderFlangeAtsu_E);
                Revit.DB.Parameter parCantiGirderFugo = famSym.LookupParameter(_CmpParameters.CantiGirderFugo);

                Revit.DB.Parameter parCantiGirderFillet_S = famSym.LookupParameter(_CmpParameters.CantiGirderFillet_S);
                Revit.DB.Parameter parCantiGirderFillet_E = famSym.LookupParameter(_CmpParameters.CantiGirderFillet_E);

                if (parCantiGirderWebMat_S != null &&
                    parCantiGirderFlangeMat_S != null &&
                    parCantiGirderWebMat_E != null &&
                    parCantiGirderFlangeMat_E != null &&
                    parCantiGirderSyubetsu != null &&
                    parCantiGirderSei_S != null &&
                    parCantiGirderHaba_S != null &&
                    parCantiGirderWebAtsu_S != null &&
                    parCantiGirderFlangeAtsu_S != null &&
                    parCantiGirderSei_E != null &&
                    parCantiGirderHaba_E != null &&
                    parCantiGirderWebAtsu_E != null &&
                    parCantiGirderFlangeAtsu_E != null &&
                    parCantiGirderFugo != null &&

                     parCantiGirderFillet_S != null &&
                        parCantiGirderFillet_E != null
                    )
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 6)
                        {
                            flagCantiGirder = true;
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////

                //S梁・ブレース山形鋼
                var p1 = famSym.LookupParameter(_CmpParameters.LGirderMaterial);
                var p2 = famSym.LookupParameter(_CmpParameters.LGirderHashiyubetsu);
                var p3 = famSym.LookupParameter(_CmpParameters.LGirderSei_C);
                var p4 = famSym.LookupParameter(_CmpParameters.LGirderHaba_C);
                var p5 = famSym.LookupParameter(_CmpParameters.LGirderDirThick_C);
                var p6 = famSym.LookupParameter(_CmpParameters.LGirderWidthThick_C);
                var p7 = famSym.LookupParameter(_CmpParameters.LGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 11)
                        {
                            flagSteelL = true;
                        }
                    }
                }

                //S梁・ブレース溝形鋼
                p1 = famSym.LookupParameter(_CmpParameters.UGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.UGirderHashiyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.UGirderSei_C);
                p4 = famSym.LookupParameter(_CmpParameters.UGirderHaba_C);
                p5 = famSym.LookupParameter(_CmpParameters.UGirderWebAtsu_C);
                p6 = famSym.LookupParameter(_CmpParameters.UGirderFlangeAtsu_C);
                p7 = famSym.LookupParameter(_CmpParameters.UGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 10)
                        {
                            flagSteelU = true;
                        }
                    }
                }

                //S梁・ブレースリップ溝形鋼
                p1 = famSym.LookupParameter(_CmpParameters.CGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.CGirderHashiyubetsu);
                p3 = famSym.LookupParameter(_CmpParameters.CGirderSei_C);
                p4 = famSym.LookupParameter(_CmpParameters.CGirderHaba_C);
                p5 = famSym.LookupParameter(_CmpParameters.CGirderLipLength_C);
                p6 = famSym.LookupParameter(_CmpParameters.CGirderThick_C);
                p7 = famSym.LookupParameter(_CmpParameters.CGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 21)
                        {
                            flagSteelC = true;
                        }
                    }
                }

                //ブレースフラットバー
                p1 = famSym.LookupParameter(_CmpParameters.FBGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.FBGirderBraceType);
                p3 = famSym.LookupParameter(_CmpParameters.FBGirderWidth);
                p4 = famSym.LookupParameter(_CmpParameters.FBGirderBoardThick);
                p5 = famSym.LookupParameter(_CmpParameters.FBGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null
                    )
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 31)
                        {
                            flagSteelFB = true;
                        }
                    }
                }

                //ブレース丸鋼
                p1 = famSym.LookupParameter(_CmpParameters.MGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.MGirderBraceType);
                p3 = famSym.LookupParameter(_CmpParameters.MGirderDiameter);
                p4 = famSym.LookupParameter(_CmpParameters.MGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null
                    )
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 13)
                        {
                            flagSteelM = true;
                        }
                    }
                }

                //ブレース円形鋼管
                p1 = famSym.LookupParameter(_CmpParameters.PGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.PGirderBraceType);
                p3 = famSym.LookupParameter(_CmpParameters.PGirderDiameter);
                p4 = famSym.LookupParameter(_CmpParameters.PGirderItaatsu);
                p5 = famSym.LookupParameter(_CmpParameters.PGirderFugo);

                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 15)
                        {
                            flagSteelP = true;
                        }
                    }
                }

                //ブレース角形鋼管
                p1 = famSym.LookupParameter(_CmpParameters.RectGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.RectGirderBraceType);
                p3 = famSym.LookupParameter(_CmpParameters.RectGirderSei);
                p4 = famSym.LookupParameter(_CmpParameters.RectGirderHaba);
                p5 = famSym.LookupParameter(_CmpParameters.RectGirderDirThick);
                p6 = famSym.LookupParameter(_CmpParameters.RectGirderDirWidth);
                p7 = famSym.LookupParameter(_CmpParameters.RectGirderFillet);
                var p8 = famSym.LookupParameter(_CmpParameters.RectGirderFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null &&
                    p8 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 14)
                        {
                            flagSteelRect = true;
                        }
                    }
                }

                //S梁カットティー
                p1 = famSym.LookupParameter(_CmpParameters.TGirderMaterial);
                p2 = famSym.LookupParameter(_CmpParameters.TGirderBraceType);
                p3 = famSym.LookupParameter(_CmpParameters.TGirderSei);
                p4 = famSym.LookupParameter(_CmpParameters.TGirderHaba);
                p5 = famSym.LookupParameter(_CmpParameters.TGirderWebAtsu);
                p6 = famSym.LookupParameter(_CmpParameters.TGirderFlangeAtsu);
                p7 = famSym.LookupParameter(_CmpParameters.TGirderFugo);
                if (p1 != null &&
                    p2 != null &&
                    p3 != null &&
                    p4 != null &&
                    p5 != null &&
                    p6 != null &&
                    p7 != null)
                {
                    var para_shape = famSym.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                    if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                    {
                        if (para_shape.AsInteger() == 17)
                        {
                            flagSteelT = true;
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////

                if (flagGirder)
                {
                    girderAry.Add(famSym);
                }
                else if (flagCantiGirder)
                {
                    cantiGirderAry.Add(famSym);
                }
                else if (flagSteelL)
                {
                    steelLAry.Add(famSym);
                }
                else if (flagSteelU)
                {
                    steelUAry.Add(famSym);
                }
                else if (flagSteelC)
                {
                    steelCAry.Add(famSym);
                }
                else if (flagSteelFB)
                {
                    steelFBAry.Add(famSym);
                }
                else if (flagSteelM)
                {
                    steelMAry.Add(famSym);
                }
                else if (flagSteelRect)
                {
                    steelRectAry.Add(famSym);
                }
                else if (flagSteelP)
                {
                    steelPAry.Add(famSym);
                }
                else if (flagSteelT)
                {
                    steelTAry.Add(famSym);
                }
            }
            
            // 断面形状が設定されていないファミリについて警告ダイアログを出す。
            if ( emptyStructuralSectionShapeItemsString != string.Empty ) {
                emptyStructuralSectionShapeItemsString = $"{_CmpAttribute.ResourceText( "IDS_TXT_STRUCTURAL_SECTION_SHAPE_NOT_DEFINED_DIALOG" )}\n\n{emptyStructuralSectionShapeItemsString.TrimEnd( '\n' )}" ;
                TaskDialog.Show( _CmpAttribute.ResourceText( "IDS_TXT_STRUCTURAL_SECTION_SHAPE_NOT_DEFINED" ), emptyStructuralSectionShapeItemsString , TaskDialogCommonButtons.Close ) ;
            }
        }

        /// ================================================================================
        /// <summary>レベル名取得</summary>
        ///
        /// <param name="steelHAry"     >鉄骨 H形鋼</param>
        /// <param name="steelRectAry"  >鉄骨 角形鋼管</param>
        /// <param name="steelRoundAry" >鉄骨 鋼管</param>
        /// <param name="cftRectAry"    >CFT 角形鋼管</param>
        /// <param name="cftRoundAry"   >CFT 鋼管</param>
        /// <param name="girderAry"     >梁</param>
        /// <param name="cantiGirderAry">片持ち梁</param>
        ///
        /// <history><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2016/10/20 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> GetLevelName(Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelLAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelUAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelCAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelFBAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelMAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelTAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelRectAry,
                                                        Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelPAry
                                                       )
        {
            Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

            // 鉄骨 H形鋼
            foreach (Revit.DB.FamilySymbol famSym in steelHAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnHSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.SColumnHFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // 鉄骨 角形鋼管
            foreach (Revit.DB.FamilySymbol famSym in steelRectAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRectSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.SColumnRectFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // 鉄骨 鋼管
            foreach (Revit.DB.FamilySymbol famSym in steelRoundAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.SColumnRoundFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // CFT 角形鋼管
            foreach (Revit.DB.FamilySymbol famSym in cftRectAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRectSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CFTColumnRectFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // CFT 鋼管
            foreach (Revit.DB.FamilySymbol famSym in cftRoundAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRoundSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CFTColumnRoundFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            foreach (Revit.DB.FamilySymbol famSym in steelLAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.LColumnFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in steelUAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CFTColumnRoundFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in steelCAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CColumnFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in steelFBAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.FBColumnFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in steelMAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.MColumnFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in steelTAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_COLUMN") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_POST"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.TColumnFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            // 梁
            foreach (Revit.DB.FamilySymbol famSym in girderAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.GirderSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_GIRDER") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.GirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // 片持ち梁
            foreach (Revit.DB.FamilySymbol famSym in cantiGirderAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CantiGirderSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_GIRDER") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CantiGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            //brace
            foreach (Revit.DB.FamilySymbol famSym in girdersteelLAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.LGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelUAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.UGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelCAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.CGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelFBAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.FBGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelMAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.MGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelTAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.PGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.PGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelRectAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.RectGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.RectGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            foreach (Revit.DB.FamilySymbol famSym in girdersteelPAry)
            {
                // 種別
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.PGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BRACE") &&
                    syubetsu != _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    continue;
                }

                string fugoParamName = _CmpParameters.PGirderFugo;

                string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            // ソート
            ret.Sort();

            if (ret.Count > 1)
            {
                if (string.Compare(ret[0], ret[ret.Count - 1], false) < 0)
                {
                    ret.Reverse();
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階記号ソート順 - 接頭語順</summary>
        ///
        /// <param name="levelNames">階記号</param>
        ///
        /// <history>2016/08/31 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.List<string> LevelSortOrder_TopName(Collections.Generic.IList<string> levelNames)
        {
            // 戻り値
            Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

            if (levelNames.Count == 0)
            {
                return ret;
            }

            if (levelNames.Count == 1)
            {
                ret.Add(levelNames[0]);
                return ret;
            }

            // 符号名並び替え
            FugoNameComparer comparer = new FugoNameComparer();

            // 屋上レベル
            Collections.Generic.List<string> okujo = new Collections.Generic.List<string>();

            // 一般レベル
            Collections.Generic.List<string> ippan = new Collections.Generic.List<string>();

            // 中間レベル
            Collections.Generic.List<string> naka = new Collections.Generic.List<string>();

            // 地下レベル
            Collections.Generic.List<string> tika = new Collections.Generic.List<string>();

            // 地下中間レベル
            Collections.Generic.List<string> tikanaka = new Collections.Generic.List<string>();

            // 他
            Collections.Generic.List<string> hoka = new Collections.Generic.List<string>();

            foreach (string str in levelNames)
            {
                // 接頭文字で振り分け

                // 屋上
                if (str.StartsWith("R") || str.StartsWith("P") || str.StartsWith("PH"))
                {
                    okujo.Add(str);
                }
                // 地下中間
                else if (str.StartsWith("MB"))
                {
                    tikanaka.Add(str);
                }
                // 地下
                else if (str.StartsWith("B"))
                {
                    tika.Add(str);
                }
                // 中間
                else if (str.StartsWith("M"))
                {
                    naka.Add(str);
                }
                // 一般
                else if (str.StartsWith("1") ||
                         str.StartsWith("2") ||
                         str.StartsWith("3") ||
                         str.StartsWith("4") ||
                         str.StartsWith("5") ||
                         str.StartsWith("6") ||
                         str.StartsWith("7") ||
                         str.StartsWith("8") ||
                         str.StartsWith("9"))
                {
                    ippan.Add(str);
                }
                else
                {
                    hoka.Add(str);
                }
            }

            // それそれでソートして追加

            // 屋上
            okujo.Sort();
            okujo.Reverse();

            Collections.Generic.List<string> okujo_R = new Collections.Generic.List<string>();
            Collections.Generic.List<string> okujo_P = new Collections.Generic.List<string>();
            Collections.Generic.List<string> okujo_PH = new Collections.Generic.List<string>();

            foreach (string str in okujo)
            {
                if (str.StartsWith("R"))
                {
                    // 2文字目から
                    if (str.Length > 1)
                    {
                        string s = str.Substring(1);
                        okujo_R.Add(s);
                    }
                    else
                    {
                        okujo_R.Add("");
                    }
                }
                else if (str.StartsWith("PH"))
                {
                    // 3文字目から
                    if (str.Length > 2)
                    {
                        string s = str.Substring(2);
                        okujo_PH.Add(s);
                    }
                    else
                    {
                        okujo_PH.Add("");
                    }
                }
                else if (str.StartsWith("P"))
                {
                    // 2文字目から
                    if (str.Length > 1)
                    {
                        string s = str.Substring(1);
                        okujo_P.Add(s);
                    }
                    else
                    {
                        okujo_P.Add("");
                    }
                }
            }

            okujo_R.Sort(comparer);
            okujo_P.Sort(comparer);
            okujo_PH.Sort(comparer);

            okujo_R.Reverse();
            okujo_P.Reverse();
            okujo_PH.Reverse();

            foreach (string s in okujo_PH)
            {
                ret.Add("PH" + s);
            }
            foreach (string s in okujo_P)
            {
                ret.Add("P" + s);
            }
            foreach (string s in okujo_R)
            {
                ret.Add("R" + s);
            }

            // 通常、中
            Collections.Generic.List<string> togo = new Collections.Generic.List<string>();

            foreach (string s in ippan)
            {
                togo.Add(s);
            }
            foreach (string s in naka)
            {
                if (s.Length > 1)
                {
                    string sub = s.Substring(1);
                    togo.Add(sub);
                }
                else
                {
                    togo.Add("");
                }
            }

            togo.Sort(comparer);
            togo.Reverse();

            int i = 0;

            Collections.Generic.List<string> strAry = new Collections.Generic.List<string>();

            while (i < togo.Count)
            {
                string str = togo[i];

                if (naka.Contains("M" + str))
                {
                    if (i == 0)
                    {
                        if (togo.Count > 1)
                        {
                            string ato = togo[1];

                            if (str != ato)
                            {
                                str = "M" + str;
                            }
                        }
                    }
                    else if (i > 0)
                    {
                        string mae = togo[i - 1];

                        if (mae == str)
                        {
                            str = "M" + str;
                        }
                        else
                        {
                            if (i < togo.Count - 1)
                            {
                                // 1つ後も違った場合
                                string ato = togo[i + 1];

                                if (str != ato)
                                {
                                    str = "M" + str;
                                }
                            }
                        }
                    }
                }

                ret.Add(str);

                i += 1;
            }

            // 地下、地下中
            togo = new Collections.Generic.List<string>();

            foreach (string s in tika)
            {
                togo.Add(s);
            }
            foreach (string s in tikanaka)
            {
                string sub = s.Substring(1);
                togo.Add(sub);
            }

            togo.Sort(comparer);
            //togo.Reverse();

            i = 0;

            while (i < togo.Count)
            {
                string str = togo[i];

                if (tikanaka.Contains("M" + str))
                {
                    if (i == 0)
                    {
                        str = "M" + str;
                    }
                    else if (i > 0)
                    {
                        if (i < togo.Count - 1)
                        {
                            string ato = togo[i + 1];

                            if (str == ato)
                            {
                                str = "M" + str;
                            }
                            else
                            {
                                string mae = togo[i - 1];

                                if (str != mae)
                                {
                                    str = "M" + str;
                                }
                            }
                        }
                    }
                }

                ret.Add(str);

                i += 1;
            }

            // そのほか
            hoka.Sort(comparer);
            //hoka.Reverse();

            foreach (string str in hoka)
            {
                ret.Add(str);
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階記号ソート順 + 名前降順</summary>
        ///
        /// <history>2016/09/01 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> LevelSortOrder_NameDESC()
        {
            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

            string sortedOrder = LevelSortOrder;

            // ソート済みを追加
            while (sortedOrder != "")
            {
                if (sortedOrder.Contains("/SortOrder"))
                {
                    string subs = sortedOrder.Substring(0, sortedOrder.IndexOf("/SortOrder"));
                    sortedOrder = sortedOrder.Substring(sortedOrder.IndexOf("/SortOrder") + 10);

                    ret.Add(subs);
                }
                else
                {
                    ret.Add(sortedOrder);
                    break;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階記号ソート順 + 名前降順</summary>
        ///
        /// <param name="levelNames">階名</param>
        ///
        /// <history>2016/09/01 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> LevelSortOrder_NameDESC(Collections.Generic.IList<string> levelNames)
        {
            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

            Collections.Generic.IList<string> sortedOrder = LevelSortOrder_NameDESC();

            foreach (string str in sortedOrder)
            {
                if (levelNames.Contains(str))
                {
                    ret.Add(str);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階記号ソート順</summary>
        ///
        /// <param name="levelNames">階名</param>
        ///
        /// <history>2016/09/14 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> LevelOrder(Collections.Generic.IList<string> levelNames)
        {
            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

            // 接頭語ソート
            Collections.Generic.IList<string> topNameOrder = LevelSortOrder_TopName(levelNames);

            // 階記号ソート
            ret = LevelSortOrder_NameDESC(topNameOrder);

            // 階記号ソートに含まれない階記号を追加
            foreach (string lvlName in topNameOrder)
            {
                if (ret.Contains(lvlName) == false)
                {
                    ret.Add(lvlName);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字作成</summary>
        ///
        /// <param name="view"      >ビュー</param>
        /// <param name="origin"    >原点</param>
        /// <param name="baseVec"   >文字方向</param>
        /// <param name="lineWidth" >幅</param>
        /// <param name="typeId"    >タイプID</param>
        /// <param name="text"      >文字</param>
        /// <param name="doc"       >ドキュメント</param>
        ///
        /// <history><p>2015/04/28 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/06/25 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.TextNote CreateNewTextNote(Revit.DB.View view,
                                            Revit.DB.XYZ origin,
                                            Revit.DB.XYZ baseVec,
                                            double lineWidth,
                                            Revit.DB.ElementId typeId,
                                            string text,
                                            Revit.DB.Document doc,
                                            Revit.DB.Transaction trans)
        {
            Revit.DB.TextNote ret = null;

            if (text == "")
            {
                return ret;
            }

            Revit.DB.XYZ p0 = new Revit.DB.XYZ(0, 0, 0);
            Revit.DB.XYZ p1 = new Revit.DB.XYZ(1, 0, 0);
            Revit.DB.XYZ p2 = baseVec;

            // 回転角
            double dotProduct = _CmpGeometry.DotProduct2D(p0, p1, p2);
            double crossProduct = _CmpGeometry.CrossProduct2D(p0, p1, p2);
            double rotate = Math.Atan2(crossProduct, dotProduct);

            // 各種設定
            Revit.DB.TextNoteOptions opt = new Revit.DB.TextNoteOptions();

            opt.HorizontalAlignment = Revit.DB.HorizontalTextAlignment.Center;
            opt.KeepRotatedTextReadable = false;
            opt.Rotation = rotate;
            opt.TypeId = typeId;

            // 作成
            trans.Start("TextNote.Create");
            ret = Revit.DB.TextNote.Create(doc, view.Id, origin, text, opt);
            trans.Commit();

            double txtMove = ret.Height * view.Scale / 2.0;

            // 文字要素は上部を基点に作成されるため、文字高さの半分移動させ、中心を指定座標の位置に合わせる

            trans.Start("Location.Move");
            // 外形
            Revit.DB.BoundingBoxXYZ bbXYZ = ret.get_BoundingBox(view);
            Revit.DB.XYZ max = bbXYZ.Max;
            Revit.DB.XYZ min = bbXYZ.Min;

            if (rotate == 0.0)
            {
                // 移動量
                // 外形の縦方向の半分
                double dis = txtMove;
                Revit.DB.XYZ move = new Revit.DB.XYZ(baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis);

                // 移動
                ret.Location.Move(move);
            }
            else if (rotate != 0.0)
            {
                // 移動量
                double dis = -txtMove;
                Revit.DB.XYZ move = new Revit.DB.XYZ(baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis);

                // 移動
                ret.Location.Move(move);
            }
            trans.Commit();
            return ret;
        }

        /// ================================================================================
        /// <summary>文字移動</summary>
        ///
        /// <param name="txtNote"   >文字</param>
        /// <param name="viewScale" >尺度</param>
        /// <param name="view"      >ビュー</param>
        /// <param name="trans"     >トランザクション</param>
        ///
        /// <history>2017/07/13 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void MoveTextNote(Revit.DB.TextNote txtNote,
                          int viewScale,
                          Revit.DB.View view,
                          Revit.DB.Transaction trans)
        {
            double width = txtNote.Width * viewScale;

            #region
            //Revit.DB.TextNoteType textType = txtNote.TextNoteType;

            //Revit.DB.Parameter paramTextFont = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_FONT);

            //Revit.DB.Parameter paramTextSize = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE);

            //Revit.DB.Parameter paramBorderSize = textType.get_Parameter(Revit.DB.BuiltInParameter.LEADER_OFFSET_SHEET);

            //Revit.DB.Parameter paramTextBold = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_BOLD);

            //Revit.DB.Parameter paramTextItalic = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_ITALIC);

            //Revit.DB.Parameter paramTextUnderline = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_UNDERLINE);

            //Revit.DB.Parameter paramTextWidthScale = textType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_WIDTH_SCALE);

            //string fontName = paramTextFont.AsString();

            //double textHeight = paramTextSize.AsDouble();

            //bool textBold = paramTextBold.AsInteger() == 1 ? true : false;

            //bool textItalic = paramTextItalic.AsInteger() == 1 ? true : false;

            //bool textUnderline = paramTextUnderline.AsInteger() == 1 ? true : false;

            //double textBorder = paramBorderSize.AsDouble();

            //double textWidthScale = paramTextWidthScale.AsDouble();

            //System.Drawing.FontStyle textStyle = System.Drawing.FontStyle.Regular;

            //if (textBold)
            //{
            //  textStyle |= System.Drawing.FontStyle.Bold;
            //}

            //if (textItalic)
            //{
            //  textStyle |= System.Drawing.FontStyle.Italic;
            //}

            //if (textUnderline)
            //{
            //  textStyle |= System.Drawing.FontStyle.Underline;
            //}

            //float fontHeightInch = (float)textHeight * 12.0f;
            //float displayDpiX = GetDpiX();

            //float fontDpi = displayDpiX;// 96.0f;
            //float pointSize = (float)(textHeight * 12.0 * fontDpi);

            //System.Drawing.Font font = new System.Drawing.Font(fontName, pointSize, textStyle);

            //string text = txtNote.Text;
            //text = text.TrimEnd('\r', '\n');

            //System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(Autodesk.Windows.ComponentManager.ApplicationWindow);
            //double stringWidthPx = g.MeasureString(text, font).Width;
            //double stringWidthIn = stringWidthPx / displayDpiX;

            //double stringWidthFt = stringWidthIn / 12.0;

            //double lineWidth = (stringWidthFt * textWidthScale) * viewScale;

            //if (width > lineWidth)
            //{
            //  width = lineWidth;
            //}

            //float w = (float)(txtNote.Width * 12.0 * fontDpi);
            //System.Drawing.Size proposedSize = new System.Drawing.Size((int)stringWidthPx, (int)pointSize);
            //g = System.Drawing.Graphics.FromHwnd(Autodesk.Windows.ComponentManager.ApplicationWindow);
            //System.Windows.Forms.TextRenderer.DrawText(g, text, font, new System.Drawing.Point(0, 0), System.Drawing.Color.Black, System.Windows.Forms.TextFormatFlags.NoPadding);
            //System.Drawing.Size nopadSize = System.Windows.Forms.TextRenderer.MeasureText(g, text, font, proposedSize, System.Windows.Forms.TextFormatFlags.NoPadding);
            //double stringWidthPx2 = nopadSize.Width;
            //double stringWidthIn2 = stringWidthPx2 / displayDpiX;

            //double stringWidthFt2 = stringWidthIn2 / 12.0;

            //double lineWidth2 = (stringWidthFt2 * textWidthScale) * viewScale;

            //if (width > lineWidth2)
            //{
            //  width = (width + lineWidth2) / 2;
            //}

            //g.Dispose();
            //font.Dispose();
            #endregion Member Functions

            Revit.DB.XYZ move = new Revit.DB.XYZ(width * 0.99 / 2 + 3 / 304.8 * viewScale, 0, 0);

            trans.Start("Move");

            txtNote.Location.Move(move);

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>DPI取得</summary>
        ///
        /// <history>2017/07/13 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        private static float GetDpiX()
        {
            float xDpi, yDpi;

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                xDpi = g.DpiX;
                yDpi = g.DpiY;
            }
            return xDpi;
        }

        #region

        /// <summary>
        /// Graphics.DrawStringで文字列を描画した時の大きさと位置を正確に計測する
        /// </summary>
        /// <param name="g">文字列を描画するGraphics</param>
        /// <param name="text">描画する文字列</param>
        /// <param name="font">描画に使用するフォント</param>
        /// <param name="proposedSize">これ以上大きいことはないというサイズ。
        /// できるだけ小さくすること。</param>
        /// <param name="stringFormat">描画に使用するStringFormat</param>
        /// <returns>文字列が描画される範囲。
        /// 見つからなかった時は、Rectangle.Empty。</returns>
        public
        static System.Drawing.Rectangle MeasureStringPrecisely(System.Drawing.Graphics g,
                                                               string text,
                                                               System.Drawing.Font font,
                                                               System.Drawing.Size proposedSize,
                                                               System.Drawing.StringFormat stringFormat)
        {
            //解像度を引き継いで、Bitmapを作成する
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(proposedSize.Width,
                                                                  proposedSize.Height,
                                                                  g);
            //BitmapのGraphicsを作成する
            System.Drawing.Graphics bmpGraphics = System.Drawing.Graphics.FromImage(bmp);
            //Graphicsのプロパティを引き継ぐ
            bmpGraphics.TextRenderingHint = g.TextRenderingHint;
            bmpGraphics.TextContrast = g.TextContrast;
            bmpGraphics.PixelOffsetMode = g.PixelOffsetMode;
            //文字列の描かれていない部分の色を取得する
            System.Drawing.Color backColor = bmp.GetPixel(0, 0);
            //実際にBitmapに文字列を描画する
            bmpGraphics.DrawString(text,
                                   font,
                                   System.Drawing.Brushes.Black,
                                   new System.Drawing.RectangleF(0f, 0f, proposedSize.Width, proposedSize.Height),
                                   stringFormat);
            bmpGraphics.Dispose();
            //文字列が描画されている範囲を計測する
            System.Drawing.Rectangle resultRect = MeasureForegroundArea(bmp, backColor);
            bmp.Dispose();

            return resultRect;
        }

        /// <summary>
        /// 指定されたBitmapで、backColor以外の色が使われている範囲を計測する
        /// </summary>
        private
        static System.Drawing.Rectangle MeasureForegroundArea(System.Drawing.Bitmap bmp, System.Drawing.Color backColor)
        {
            int backColorArgb = backColor.ToArgb();
            int maxWidth = bmp.Width;
            int maxHeight = bmp.Height;

            //左側の空白部分を計測する
            int leftPosition = -1;
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    //違う色を見つけたときは、位置を決定する
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        leftPosition = x;
                        break;
                    }
                }
                if (0 <= leftPosition)
                {
                    break;
                }
            }
            //違う色が見つからなかった時
            if (leftPosition < 0)
            {
                return System.Drawing.Rectangle.Empty;
            }

            //右側の空白部分を計測する
            int rightPosition = -1;
            for (int x = maxWidth - 1; leftPosition < x; x--)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        rightPosition = x;
                        break;
                    }
                }
                if (0 <= rightPosition)
                {
                    break;
                }
            }
            if (rightPosition < 0)
            {
                rightPosition = leftPosition;
            }

            //上の空白部分を計測する
            int topPosition = -1;
            for (int y = 0; y < maxHeight; y++)
            {
                for (int x = leftPosition; x <= rightPosition; x++)
                {
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        topPosition = y;
                        break;
                    }
                }
                if (0 <= topPosition)
                {
                    break;
                }
            }
            if (topPosition < 0)
            {
                return System.Drawing.Rectangle.Empty;
            }

            //下の空白部分を計測する
            int bottomPosition = -1;
            for (int y = maxHeight - 1; topPosition < y; y--)
            {
                for (int x = leftPosition; x <= rightPosition; x++)
                {
                    if (bmp.GetPixel(x, y).ToArgb() != backColorArgb)
                    {
                        bottomPosition = y;
                        break;
                    }
                }
                if (0 <= bottomPosition)
                {
                    break;
                }
            }
            if (bottomPosition < 0)
            {
                bottomPosition = topPosition;
            }

            //結果を返す
            return new System.Drawing.Rectangle(leftPosition, topPosition,
                rightPosition - leftPosition, bottomPosition - topPosition);
        }

        private
        static System.Drawing.Rectangle MeasureForegroundArea(System.Drawing.Bitmap bmp)
        {
            return MeasureForegroundArea(bmp, bmp.GetPixel(0, 0));
        }

        #endregion

        /// ================================================================================
        /// <summary>符号順序</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> FugoOrder(System.Data.DataTable data)
        {
            Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

            foreach (System.Data.DataRow row in data.Rows)
            {
                string fugo = (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")];

                if (ret.Contains(fugo) == false)
                {
                    ret.Add(fugo);
                }
            }

            ret.Sort(new FugoNameComparer());

            return ret;
        }

        /// ================================================================================
        /// <summary>リスト折り返しごとの符号</summary>
        ///
        /// <param name="fugo"        >符号</param>
        /// <param name="newLine"     >リストの折り返し</param>
        /// <param name="newLineSpan" >折り返しスパン</param>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Collections.Generic.IList<string>> FugoAryByNewLine(Collections.Generic.IList<string> fugo,
                                                                                      int newLine,
                                                                                      int newLineSpan)
        {
            // 戻り値
            Collections.Generic.IList<Collections.Generic.IList<string>> ret = new Collections.Generic.List<Collections.Generic.IList<string>>();

            // リストの折り返し
            int _NewLineSpan = newLineSpan;

            // 折り返しなし
            if (newLine == 0)
            {
                _NewLineSpan = fugo.Count;
            }
            // 折り返し列数が0
            else if (newLineSpan == 0)
            {
                _NewLineSpan = fugo.Count;
            }

            Collections.Generic.IList<string> list = new Collections.Generic.List<string>();

            int count = 0;

            foreach (string f in fugo)
            {
                count += 1;
                if (count > _NewLineSpan)
                {
                    ret.Add(list);

                    list = new Collections.Generic.List<string>();
                    count = 1;
                }

                list.Add(f);
            }

            if (list.Count > 0)
            {
                ret.Add(list);
            }

            return ret;
        }

        #endregion

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>文字列取得 - 階記号ソート順序</summary>
        ///
        /// <history>2016/08/30 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public string LevelSortOrder
        {
            get
            {
                return _EntDtCmd.LevelSortOrdeer;
            }
        }

        #endregion
    }

    /// ================================================================================
    /// <summary>符号名の並び替え</summary>
    /// ================================================================================
    public
    class FugoNameComparer : Collections.Generic.IComparer<string>
    {
        public static bool NumCheck = true;

        private static string _NumRegex = @"^(.*?)([0-9]+).*?$";
        private static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(_NumRegex);

        // xがyより小さいときはマイナス、大きいときはプラス
        // 同じときは0を返す
        public int Compare(string x, string y)
        {
            string a = x;
            string b = y;

            string a2 = a;
            string b2 = b;

            // 等しい
            if (a == b)
            {
                return 0;
            }

            // 数値部分
            int? ai = null;
            int? bi = null;

            if (NumCheck)
            {
                // 正規表現で切り出し
                System.Text.RegularExpressions.Match match = regex.Match(a);

                if (match.Success)
                {
                    a = match.Groups[1].Value;
                    ai = Convert.ToInt32(match.Groups[2].Value);
                }

                match = regex.Match(b);

                if (match.Success)
                {
                    b = match.Groups[1].Value;
                    bi = Convert.ToInt32(match.Groups[2].Value);
                }
            }

            // 文字の比較
            int t = string.Compare(a, b);

            if (NumCheck && t == 0)
            {
                if (ai == null && bi != null)
                {
                    t = -1;
                }
                else if (ai != null && bi == null)
                {
                    t = 1;
                }
                else if (ai == null && bi == null)
                {
                    t = string.Compare(a2, b2);
                }
                else
                {
                    t = (int)(ai - bi);
                    if (t == 0)
                    {
                        t = string.Compare(a2, b2);
                    }
                }
            }

            return t;
        }
    }
}