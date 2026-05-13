using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.List
{
    /// ================================================================================
    /// <summary>コマンド 柱リスト作成</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdPostType : Revit.UI.IExternalCommand
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

            SectionListSteel.List.CreateColumnList createColumnList = new SectionListSteel.List.CreateColumnList(cmpAttribute,
                                                                                                   cmpElements,
                                                                                                   cmpGeometry,
                                                                                                   cmpParameters,
                                                                                                   cmpSettings,
                                                                                                   cmpService);

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return retCmd;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return retCmd;
            }

            // トランザクショングループ
            Revit.DB.TransactionGroup transacGrp = new Revit.DB.TransactionGroup(rvtDbDoc);
            transacGrp.Start(cmpAttribute.ResourceText("IDS_BTN_COLUMNLIST_NAME"));

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

            // 構造平面取得判定
            if (cmpElements.IsStrPlaneGet() == false)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTSTRPLANEVIEW"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                transacGrp.RollBack();
                return retCmd;
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

            if (cmpParameters.GetSettingValue(settingFileName, settingFileDirectory).Length != 75 &&
                cmpParameters.GetSettingValue(settingFileName, settingFileDirectory).Length != 76)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGFILE"),
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                transacGrp.RollBack();
                return retCmd;
            }

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

                transacGrp.RollBack();
                return retCmd;
            }

            rvtUiDoc.Selection.SetElementIds(new Collections.Generic.List<Revit.DB.ElementId>());

            // 作成
            errMsg = createColumnList.Create(false, true);

            if (errMsg != "")
            {
                System.Windows.MessageBox.Show(errMsg,
                                               cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                transacGrp.RollBack();
                retCmd = Autodesk.Revit.UI.Result.Failed;
                return retCmd;
            }

            // ウェブとフランジのマテリアル違い
            string materialVary = createColumnList.MaterialVary;
            if (materialVary != "")
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_MATERIALVARY_COLUMN") + "\r\n\r\n" + materialVary,
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
            }

            string materialVaryT = createColumnList.MaterialVaryT;
            if (materialVaryT != "")
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_DIFWEBFLANGEMAT") + "\r\n\r\n" + materialVaryT,
                                                     cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
            }

            cmpParameters.SetSharedParamDefault();

            transacGrp.Assimilate();

            retCmd = Autodesk.Revit.UI.Result.Succeeded;
            return retCmd;
        }

        #endregion Member Functions
    }
}