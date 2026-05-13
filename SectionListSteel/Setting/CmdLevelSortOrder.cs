using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Setting
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
        /// <param name="elemenets"   >エラー要素</param>
        ///
        /// <returns>実行結果</returns>
        ///
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                       ref string message,
                                       Revit.DB.ElementSet elemenets)
        {
            // 初期化
            Revit.UI.UIApplication rvtUiApp = commandData.Application;
            Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument;
            Revit.DB.Document rvtDbDoc = rvtUiDoc.Document;
            Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application;

            SectionListSteel.Components.Attribute cmpAttribute = new SectionListSteel.Components.Attribute();
            SectionListSteel.Components.Elements cmpElements = new SectionListSteel.Components.Elements(cmpAttribute, rvtUiDoc);
            SectionListSteel.Components.Geometry cmpGeometry = new SectionListSteel.Components.Geometry(cmpAttribute, rvtUiDoc);
            SectionListSteel.Components.Parameters cmpParameters = new SectionListSteel.Components.Parameters(cmpAttribute, rvtUiDoc);
            SectionListSteel.Components.Settings cmpSettings = new SectionListSteel.Components.Settings(cmpAttribute, rvtUiDoc);
            SectionListSteel.Components.Service cmpService = new SectionListSteel.Components.Service(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"));
                return retCmd;
            }

            // トランザクショングループ
            Revit.DB.TransactionGroup transacGrp = new Revit.DB.TransactionGroup(rvtDbDoc);
            transacGrp.Start(cmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_NAME"));

            // トランザクション
            Revit.DB.Transaction transac = new Revit.DB.Transaction(rvtDbDoc);

            // ワークフロー
            transac.Start(cmpAttribute.ResourceText("IDS_TXT_FLOW"));
            string retMsg = cmpService.WorkFlow();
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETSETTINGFILE"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                transacGrp.RollBack();
                return retCmd;
            }

            // 設定値を取得
            string[] settingValues = cmpParameters.GetSettingValue(settingFileName, settingFileDirectory);
            cmpParameters.GetSettingValue(settingValues);

            // 予備ファイルコピー
            cmpParameters.ReserveFileCopy();

            // パラメータ名の取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamName = cmpParameters.GetParamNames();

            if (allParamName == null ||
                allParamName.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILE"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                transacGrp.RollBack();
                return retCmd;
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

            // S梁
            Collections.Generic.IDictionary<string, string> dicGirder = null;
            // S片持ち梁
            Collections.Generic.IDictionary<string, string> dicCantiGirder = null;

            // パラメータ名取得
            bool getName = cmpParameters.GetParamNames(ref dicHColumn,
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
                //dicFBColumn == null || dicFBColumn.Count == 0 ||
                //dicMColumn == null || dicMColumn.Count == 0 ||
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILEVALUE"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                transacGrp.RollBack();
                return retCmd;
            }

            // パラメータ名設定
            cmpParameters.SetParamNames(dicHColumn,
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

            // プロジェクト内のレベル名
            Collections.Generic.IList<string> projLvlNames = cmpElements.ProjLevelNames();

            // 階表示枠接尾語
            string lvlEndword = cmpParameters.LvlEndword;

            for (int i = 0; i < projLvlNames.Count; ++i)
            {
                string str = projLvlNames[i];

                if (str.EndsWith(lvlEndword))
                {
                    str = str.Substring(0, str.LastIndexOf(lvlEndword));

                    projLvlNames[i] = str;
                }
            }

            // 柱
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpService.ColumnDivision(null,
                                        ref steelHAry,
                                       ref steelRectAry,
                                       ref steelRoundAry,
                                       ref cftRectAry,
                                       ref cftRoundAry,
                                       ref steelLAry,
                                       ref steelUAry,
                                       ref steelCAry,
                                       ref steelFBAry,
                                       ref steelMAry,
                                       ref steelTAry);

            // 梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> girdersteelPAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpService.GirderDivision(null,
                                      ref girderAry,
                                      ref cantiGirderAry,
                                      ref girdersteelLAry,
                                       ref girdersteelUAry,
                                       ref girdersteelCAry,
                                       ref girdersteelFBAry,
                                       ref girdersteelMAry,
                                       ref girdersteelTAry,
                                       ref girdersteelRectAry,
                                       ref girdersteelPAry
                                       );

            // 階名
            Collections.Generic.IList<string> levelNames = cmpService.GetLevelName(steelHAry,
                                                                                   steelRectAry,
                                                                                   steelRoundAry,
                                                                                   cftRectAry,
                                                                                   cftRoundAry,
                                                                                   steelLAry,
                                                                                   steelUAry,
                                                                                   steelCAry,
                                                                                   steelFBAry,
                                                                                   steelMAry,
                                                                                   steelTAry,
                                                                                   girderAry,
                                                                                   cantiGirderAry,
                                                                                   girdersteelLAry,
                                                                                    girdersteelUAry,
                                                                                    girdersteelCAry,
                                                                                    girdersteelFBAry,
                                                                                    girdersteelMAry,
                                                                                    girdersteelTAry,
                                                                                    girdersteelRectAry,
                                                                                    girdersteelPAry
                                                                                   );

            if (levelNames.Count == 1)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ONELISTLEVEL"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                transacGrp.Assimilate();
                return retCmd;
            }
            else if (levelNames.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOLISTLEVEL"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                transacGrp.Assimilate();
                return retCmd;
            }

            // 接頭文字を考慮したソート
            levelNames = cmpService.LevelSortOrder_TopName(levelNames);

            // フォーム表示
            SectionListSteel.Setting.FormLevelSortOrder formLvlSortOrder = new SectionListSteel.Setting.FormLevelSortOrder(cmpAttribute, levelSortOrder, levelNames);
            formLvlSortOrder.ShowDialog();

            if (formLvlSortOrder.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // ソート結果
                string sortedOrder = formLvlSortOrder.SortedOrder;

                cmpParameters.GetStrVal(settingFileName, settingFileDirectory, sortedOrder);
                transac.Start("write");
                cmpService.Set();
                transac.Commit();
                cmpService.SetInfoFile(settingFileName, settingFileDirectory, transac);
            }
            else
            {
                transacGrp.RollBack();
                return retCmd;
            }

            cmpParameters.SetSharedParamDefault();

            transacGrp.Assimilate();

            retCmd = Revit.UI.Result.Succeeded;
            return retCmd;
        }

        #endregion Member Functions
    }
}