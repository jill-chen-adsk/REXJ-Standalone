using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListRC.Setting
{
    /// ================================================================================
    /// <summary>コマンド レベルソート順序</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdLevelSortOrder : Revit.UI.IExternalCommand
    {
        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>コマンド実行処理</summary>
        ///
        /// <param name="commandData" >Revit コマンドデータ</param>
        /// <param name="message"     >エラーメッセージ</param>
        /// <param name="elements"    >エラー要素</param>
        ///
        /// <returns>実行結果</returns>
        ///
        /// <history><p>2013/05/24 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2016/09/07 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                       ref string message,
                                       Revit.DB.ElementSet elements)
        {
            // 初期化
            Revit.UI.UIApplication rvtUiApp = commandData.Application;
            Revit.DB.Document rvtDbDoc = rvtUiApp.ActiveUIDocument.Document;
            Revit.UI.UIDocument rvtUiDoc = commandData.Application.ActiveUIDocument;
            Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application;
            SectionListRC.Components.Attribute cmpAttribute = new SectionListRC.Components.Attribute();
            SectionListRC.Components.Elements cmpElements = new SectionListRC.Components.Elements(cmpAttribute, rvtUiDoc);
            SectionListRC.Components.Geometry cmpGeometry = new SectionListRC.Components.Geometry(rvtUiDoc);
            SectionListRC.Components.Parameters cmpParameters = new SectionListRC.Components.Parameters(cmpAttribute, rvtUiDoc);
            SectionListRC.Components.Settings cmpSettings = new SectionListRC.Components.Settings(rvtUiDoc);
            SectionListRC.Components.Service cmpService = new SectionListRC.Components.Service(cmpAttribute,
                                                                                                     cmpElements,
                                                                                                     cmpGeometry,
                                                                                                     cmpParameters,
                                                                                                     cmpSettings);
            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TOOLTIP_S"));

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"));

                transGroup.Assimilate();
                return retCmd;
            }

            // フロー
            Revit.DB.Transaction transac = new Revit.DB.Transaction(rvtDbDoc);
            transac.Start("フロー");
            string retMsg = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHPARAM_DEF"));
            transac.Commit();

            if (retMsg != null)
            {
                System.Windows.MessageBox.Show(retMsg);
                retCmd = Revit.UI.Result.Failed;
            }

            // 設定ファイル名
            string settingFileName = "";
            // 設定ファイルディレクトリ
            string settingFileDirectory = "";
            // 階記号ソート順序
            string levelSortOrder = "";

            cmpService.GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);

            string fullname = settingFileDirectory + settingFileName;
            if (System.IO.File.Exists(fullname) == false)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETSETTINGFILE"));
                transGroup.Assimilate();
                return retCmd;
            }

            // 設定ファイルの値を取得
            cmpParameters.GetSettingValue(cmpParameters.GetSettingValue(settingFileName, settingFileDirectory));

            // 予備ファイルコピー
            cmpParameters.ReserveFileCopy();

            // パラメータ名の取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamName = cmpParameters.GetParamName();

            if (allParamName == null ||
                allParamName.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILE"));

                transGroup.Assimilate();
                return retCmd;
            }

            // 矩形柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicKakuParamNames = null;
            // 円柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicEnParamNames = null;

            // 梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicHariParamNames = null;
            // 片持ち梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicKatamotiParamNames = null;

            cmpParameters.GetColumnParamName(ref dicKakuParamNames,
                                             ref dicEnParamNames,
                                             ref dicHariParamNames,
                                             ref dicKatamotiParamNames);

            if (dicKakuParamNames == null ||
                dicKakuParamNames.Count == 0 ||
                dicEnParamNames == null ||
                dicEnParamNames.Count == 0 ||
                dicHariParamNames == null ||
                dicHariParamNames.Count == 0 ||
                dicKatamotiParamNames == null ||
                dicKatamotiParamNames.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILEVALUE"));

                transGroup.Assimilate();
                return retCmd;
            }

            cmpParameters.GetParameterValue(dicKakuParamNames,
                                            dicEnParamNames,
                                            dicHariParamNames,
                                            dicKatamotiParamNames);

            // 柱分類
            string column_Category_Kaku = cmpParameters.HashiraBunrui_Kaku;
            // 柱幅
            string dX = cmpParameters.DX_Kaku;
            // 柱成
            string dY = cmpParameters.DY_Kaku;
            // 柱分類
            string column_Category_En = cmpParameters.Column_Category_En;
            // 直径
            string tyokkei = cmpParameters.Tyokkei_En;

            // 設定ファイルでの順番
            string setLevelSortOrder = cmpService.GetStringLevelSortOrder; // cmpParameters.LevelSortOrder;

            // プロジェクトのレベル
            Collections.Generic.List<string> projLvlNames = cmpElements.ProjLevelNames();// cmpService.LevelSortOrder_TopName(cmpElements.ProjLevelNames());

            // 階表示枠接尾語
            string lvlEndWord = cmpParameters.LevelFrameEndWord;

            for (int i = 0; i < projLvlNames.Count; ++i)
            {
                string str = projLvlNames[i];

                if (str.EndsWith(lvlEndWord))
                {
                    str = str.Substring(0, str.LastIndexOf(lvlEndWord));

                    projLvlNames[i] = str;
                }
            }

            // 全柱
            Collections.Generic.List<Revit.DB.FamilySymbol> allColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            foreach (Revit.DB.FamilySymbol famSymbol in cmpElements.GetRCColumnFamSymAry)
            {
                // パラメータ項目を持つ柱
                if (cmpService.IsHaveColumnParam(famSymbol) == true)
                {
                    allColumnAry.Add(famSymbol);
                }
            }

            // 柱
            Collections.Generic.IList<Revit.DB.FamilySymbol> columnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 間柱
            Collections.Generic.IList<Revit.DB.FamilySymbol> postAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpElements.ColumnDivision(allColumnAry,
                                       cmpParameters.HashiraBunrui_Kaku,
                                       cmpParameters.Column_Category_En,
                                       cmpParameters.DX_Kaku,
                                       cmpParameters.DY_Kaku,
                                       cmpParameters.Tyokkei_En,
                                       ref columnAry,
                                       ref postAry);

            allColumnAry.Clear();

            foreach (Revit.DB.FamilySymbol columnFamSym in columnAry)
            {
                allColumnAry.Add(columnFamSym);
            }
            foreach (Revit.DB.FamilySymbol columnFamSym in postAry)
            {
                allColumnAry.Add(columnFamSym);
            }

            // 全梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> allGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            foreach (Revit.DB.FamilySymbol fs in cmpElements.GetRCBeamFamSymAry)
            {
                if (cmpService.IsHaveGirderParam(fs) == true)
                {
                    allGirderAry.Add(fs);
                }
            }

            // 大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>(); // cmpElements.GetGirderFamInsAry(cmpParameters.Girder_Category);
                                                                                                                                // 小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 片持ち大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 片持ち小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> foundationGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> foundationBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎片持ち大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiFoundationGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎片持ち小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiFoundationBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpElements.GirderDivision(allGirderAry,
                                       cmpParameters.Girder_Category,
                                       cmpParameters.HariSyubetu_Katamoti,
                                       ref girderAry,
                                       ref beamAry,
                                       ref cantiGirderAry,
                                       ref cantiBeamAry,
                                       ref foundationGirderAry,
                                       ref foundationBeamAry,
                                       ref cantiFoundationGirderAry,
                                       ref cantiFoundationBeamAry);

            allGirderAry.Clear();

            foreach (Revit.DB.FamilySymbol girderFamSym in girderAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in beamAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in cantiGirderAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in cantiBeamAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in foundationGirderAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in foundationBeamAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in cantiFoundationGirderAry)
            {
                allGirderAry.Add(girderFamSym);
            }
            foreach (Revit.DB.FamilySymbol girderFamSym in cantiFoundationBeamAry)
            {
                allGirderAry.Add(girderFamSym);
            }

            // 全階名
            Collections.Generic.IList<string> allColumnLevelAry = cmpService.GetAllColumnLevelAry(allColumnAry);
            Collections.Generic.IList<string> allGirderLevelAry = cmpService.GetAllBeamLevelAry(allGirderAry);

            Collections.Generic.List<string> allLevelAry = new Collections.Generic.List<string>();
            foreach (string str in allColumnLevelAry)
            {
                if (allLevelAry.Contains(str) == false)
                {
                    allLevelAry.Add(str);
                }
            }
            foreach (string str in allGirderLevelAry)
            {
                if (allLevelAry.Contains(str) == false)
                {
                    allLevelAry.Add(str);
                }
            }

            // 階数が少ない = ソートしないで終了
            if (allLevelAry.Count == 1)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ONELISTLEVEL"));
                transGroup.Assimilate();
                return retCmd;
            }
            else if (allLevelAry.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOLISTLEVEL"));
                transGroup.Assimilate();
                return retCmd;
            }

            // プロジェクト内のレベルが階記号以上
            //if (allLevelAry.Count <= projLvlNames.Count)
            //{
            //  allLevelAry = projLvlNames;
            //}
            //else
            {
                // ソート
                allLevelAry.Sort();
                // 降順でなければ逆順
                if (string.Compare(allLevelAry[0], allLevelAry[allLevelAry.Count - 1]) < 0)
                {
                    allLevelAry.Reverse();
                }

                // 接頭文字を考慮したソート
                allLevelAry = cmpService.LevelSortOrder_TopName(allLevelAry);
                string name = allLevelAry[0];
            }

            // フォーム表示
            SectionListRC.Setting.FormLevelSortOrder formLevelSortOrder = new SectionListRC.Setting.FormLevelSortOrder(cmpAttribute, levelSortOrder, allLevelAry);
            System.Windows.Forms.DialogResult dlgRet = formLevelSortOrder.ShowDialog();

            if (dlgRet == System.Windows.Forms.DialogResult.OK)
            {
                // ソート結果
                string sortedOrder = formLevelSortOrder.SortedOrder;

                cmpParameters.GetStrVal(settingFileName, settingFileDirectory, sortedOrder);
                transac.Start("write");
                cmpService.Set();
                transac.Commit();
                cmpService.SetInfoFile(settingFileName, settingFileDirectory, transac);
            }
            else
            {
                transGroup.Assimilate();
                return retCmd;
            }

            retCmd = Revit.UI.Result.Succeeded;
            transGroup.Assimilate();
            return retCmd;
        }

        #endregion Member Functions
    }
}