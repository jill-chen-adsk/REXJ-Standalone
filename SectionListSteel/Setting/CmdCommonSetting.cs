using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Windows.Forms;

namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>コマンド 共通設定</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdCommonSetting : Revit.UI.IExternalCommand
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
        /// <history>2016/08/30 Created GSA,Inc. Ryo Kuroda</history>
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return retCmd;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"), cmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));

                return retCmd;
            }

            // トランザクショングループ
            Revit.DB.TransactionGroup transacGrp = new Revit.DB.TransactionGroup(rvtDbDoc);
            transacGrp.Start(cmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_NAME"));

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
            // 階記号ソート
            string levelSortOrder = "";

            transac.Start(cmpAttribute.ResourceText("IDS_TXT_READ"));
            cmpService.GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);
            transac.Commit();

            SectionListSteel.Setting.FormAllSetting formAllSetting = new SectionListSteel.Setting.FormAllSetting(cmpElements, cmpAttribute, settingFileName, settingFileDirectory);

            System.Windows.Forms.DialogResult dlgRlt = formAllSetting.ShowDialog();

            int formRet = formAllSetting._FormCommonSetting.SettingResult;

            while (dlgRlt == System.Windows.Forms.DialogResult.Yes)
            {
                // 設定ファイルの変更
                if (formRet == 5)
                {
                    // 設定ファイルの取得先を上書き
                    settingFileName = formAllSetting._FormCommonSetting.SettingFileName;
                    settingFileDirectory = formAllSetting._FormCommonSetting.SettingFileDirectory;
                    cmpParameters.GetStrVal(settingFileName, settingFileDirectory, cmpService.LevelSortOrder);
                    transac.Start("write");
                    cmpService.Set();
                    transac.Commit();
                    cmpService.SetInfoFile(settingFileName, settingFileDirectory, transac);

                    formAllSetting = new SectionListSteel.Setting.FormAllSetting(cmpElements, cmpAttribute, settingFileName, settingFileDirectory);
                    dlgRlt = formAllSetting.ShowDialog();
                    if (dlgRlt == DialogResult.OK)
                    {
                        break;
                    }

                    formRet = formAllSetting._FormCommonSetting.SettingResult;
                }
                else if (formRet == 6 || formRet == 7)
                {
                    // 上書き保存
                    Collections.Generic.IList<string> commonAry = formAllSetting._FormCommonSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formAllSetting._FormColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> subItemPostAry = formAllSetting._FormSubItemPost.SettingValues_SubItemPost;
                    Collections.Generic.IList<string> beamAry = formAllSetting._FormBeamSetting.SettingValues_Beam;
                    Collections.Generic.IList<string> subItemBeamAry = formAllSetting._FormSubItemBeam.SettingValues_BeamSub;
                    Collections.Generic.IList<string> braceAry = formAllSetting._FormBrace.SettingValues_Brace;

                    if (formRet == 6)
                        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);
                    else if (formRet == 7)
                        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

                    dlgRlt = formAllSetting.ShowDialog();
                    if (dlgRlt == DialogResult.OK)
                    {
                        break;
                    }

                    formRet = formAllSetting._FormCommonSetting.SettingResult;
                }
            }

            // OK以外
            if (dlgRlt != System.Windows.Forms.DialogResult.OK)
            {
                retCmd = Autodesk.Revit.UI.Result.Cancelled;

                transacGrp.RollBack();
            }

            // OK
            if (dlgRlt == System.Windows.Forms.DialogResult.OK)
            {
                retCmd = Autodesk.Revit.UI.Result.Succeeded;

                transacGrp.Assimilate();
            }

            cmpParameters.SetSharedParamDefault();

            return retCmd;
        }

        #endregion Member Functions
    }
}