using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.List
{
    /// ================================================================================
    /// <summary>コマンド 梁リスト作成</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdAll : Revit.UI.IExternalCommand
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
        /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda
        ///           <p>2017/06/19 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                       ref string message,
                                       Revit.DB.ElementSet elemenets)
        {
            // 初期化
            Revit.UI.UIApplication rvtUiApp = commandData.Application;
            Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument;
            Revit.DB.Document rvtDbDoc = rvtUiDoc.Document;

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

            // トランザクション
            Revit.DB.Transaction transac = new Revit.DB.Transaction(rvtDbDoc);

            // ワークフロー
            transac.Start(cmpAttribute.ResourceText("IDS_TXT_FLOW"));
            string retMsg = cmpService.WorkFlow();
            transac.Commit();

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return Revit.UI.Result.Cancelled;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return Revit.UI.Result.Cancelled;
            }            

            // 設定ファイル名
            string settingFileName = "";
            // 設定ファイルディレクトリ
            string settingFileDirectory = "";
            // 階記号ソート
            string levelSortOrder = "";

            transac.Start(cmpAttribute.ResourceText("IDS_TXT_READ"));
            cmpService.GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);
            transac.Commit();

            // 設定ファイルの値を取得
            string[] strSetAry = cmpParameters.GetSettingValue(settingFileName, settingFileDirectory);

            cmpParameters.GetSettingValue(strSetAry);

            // 予備ファイルコピー
            cmpParameters.ReserveFileCopy();

            // パラメータ設定
            string errMsg = cmpService.SetParameters(true);

            if (errMsg != "")
            {
                System.Windows.MessageBox.Show(errMsg,
                                               cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return Revit.UI.Result.Cancelled;
            }

            errMsg = cmpService.SetParameters(false);

            if (errMsg != "")
            {
                System.Windows.MessageBox.Show(errMsg,
                                               cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return Revit.UI.Result.Cancelled;
            }

            CmdColumnList cmdColumn = new CmdColumnList();
            cmdColumn.Create(commandData);

            CmdBeamList cmdBeam = new CmdBeamList();
            cmdBeam.Create(commandData);

            return Revit.UI.Result.Succeeded;
        }

        #endregion Member Functions
    }
}