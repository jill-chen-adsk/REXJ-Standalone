using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>コマンド 梁リスト設定</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdBeamSetting : Revit.UI.IExternalCommand
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
            transacGrp.Start(cmpAttribute.ResourceText("IDS_BTN_GIRDERSETTING_NAME"));

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

            // 文字タイプ
            Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypeAry = cmpElements.TxtNoteTypes;
            // 線種タイプ
            Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyleAry = cmpElements.DetailGraStyles;

            //// フォーム
            //SectionListSteel.Setting.FormCommonSetting formCommonSetting = new SectionListSteel.Setting.FormCommonSetting(cmpAttribute, settingFileName, settingFileDirectory, txtNoteTypeAry, graStyleAry);
            //SectionListSteel.Setting.FormColumnSetting formColumnSetting = new SectionListSteel.Setting.FormColumnSetting(cmpAttribute, settingFileName, settingFileDirectory);
            //SectionListSteel.Setting.FormSubItemSetting_Post formSubItemPost = new SectionListSteel.Setting.FormSubItemSetting_Post(cmpAttribute, settingFileName, settingFileDirectory);
            //SectionListSteel.Setting.FormBeamSetting formBeamSetting = new SectionListSteel.Setting.FormBeamSetting(cmpAttribute, settingFileName, settingFileDirectory);
            //SectionListSteel.Setting.FormBeamSetting_Sub formSubItemBeam = new SectionListSteel.Setting.FormBeamSetting_Sub(cmpAttribute, settingFileName, settingFileDirectory);
            //SectionListSteel.Setting.FormBraceSetting formBrace = new SectionListSteel.Setting.FormBraceSetting(cmpAttribute, settingFileName, settingFileDirectory);

            //System.Windows.Forms.DialogResult dlgRlt = formBeamSetting.ShowDialog();

            //int formRet = formBeamSetting.SettingResult;

            //while (dlgRlt == System.Windows.Forms.DialogResult.Yes)
            //{
            //    // 共通設定へ
            //    if (formRet == 0)
            //    {
            //        formCommonSetting.ShowDialog();

            //        dlgRlt = formCommonSetting.DialogResult;
            //        formRet = formCommonSetting.SettingResult;
            //    }
            //    // 柱リスト設定へ
            //    else if (formRet == 1)
            //    {
            //        formColumnSetting.ShowDialog();

            //        dlgRlt = formColumnSetting.DialogResult;
            //        formRet = formColumnSetting.SettingResult;
            //    }
            //    // 間柱リスト設定へ
            //    else if (formRet == 2)
            //    {
            //        formSubItemPost.ShowDialog();

            //        dlgRlt = formSubItemPost.DialogResult;
            //        formRet = formSubItemPost.SettingResult;
            //    }
            //    // 梁リスト設定へ
            //    else if (formRet == 3)
            //    {
            //        formBeamSetting.ShowDialog();

            //        dlgRlt = formBeamSetting.DialogResult;
            //        formRet = formBeamSetting.SettingResult;
            //    }
            //    // 小梁リスト設定へ
            //    else if (formRet == 4)
            //    {
            //        formSubItemBeam.ShowDialog();

            //        dlgRlt = formSubItemBeam.DialogResult;
            //        formRet = formSubItemBeam.SettingResult;
            //    }
            //    // 設定ファイルの変更
            //    else if (formRet == 5)
            //    {
            //        // 設定ファイルの取得先を上書き
            //        settingFileName = formCommonSetting.SettingFileName;
            //        settingFileDirectory = formCommonSetting.SettingFileDirectory;
            //        cmpParameters.GetStrVal(settingFileName, settingFileDirectory, cmpService.LevelSortOrder);
            //        transac.Start("write");
            //        cmpService.Set();
            //        transac.Commit();
            //        cmpService.SetInfoFile(settingFileName, settingFileDirectory, transac);

            //        formCommonSetting = new SectionListSteel.Setting.FormCommonSetting(cmpAttribute, settingFileName, settingFileDirectory, txtNoteTypeAry, graStyleAry);
            //        formColumnSetting = new SectionListSteel.Setting.FormColumnSetting(cmpAttribute, settingFileName, settingFileDirectory);
            //        formSubItemPost = new SectionListSteel.Setting.FormSubItemSetting_Post(cmpAttribute, settingFileName, settingFileDirectory);
            //        formBeamSetting = new SectionListSteel.Setting.FormBeamSetting(cmpAttribute, settingFileName, settingFileDirectory);
            //        formSubItemBeam = new SectionListSteel.Setting.FormBeamSetting_Sub(cmpAttribute, settingFileName, settingFileDirectory);
            //        formBrace = new SectionListSteel.Setting.FormBraceSetting(cmpAttribute, settingFileName, settingFileDirectory);

            //        formCommonSetting.ShowDialog();

            //        dlgRlt = formCommonSetting.DialogResult;
            //        formRet = formCommonSetting.SettingResult;
            //    }
            //    // 上書き保存(共通設定)
            //    else if (formRet == 6)
            //    {
            //        // 上書き保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formCommonSetting.ShowDialog();

            //        dlgRlt = formCommonSetting.DialogResult;
            //        formRet = formCommonSetting.SettingResult;
            //    }
            //    // 名前を付けて保存(共通設定)
            //    else if (formRet == 7)
            //    {
            //        // 名前を付けて保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formCommonSetting.ShowDialog();

            //        dlgRlt = formCommonSetting.DialogResult;
            //        formRet = formCommonSetting.SettingResult;
            //    }
            //    // 上書き保存(柱リスト設定)
            //    else if (formRet == 8)
            //    {
            //        // 上書き保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formColumnSetting.ShowDialog();

            //        dlgRlt = formColumnSetting.DialogResult;
            //        formRet = formColumnSetting.SettingResult;
            //    }
            //    // 名前を付けて保存(柱リスト設定)
            //    else if (formRet == 9)
            //    {
            //        // 名前を付けて保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formColumnSetting.ShowDialog();

            //        dlgRlt = formColumnSetting.DialogResult;
            //        formRet = formColumnSetting.SettingResult;
            //    }
            //    // 上書き保存(間柱リスト設定)
            //    else if (formRet == 10)
            //    {
            //        // 上書き保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formSubItemPost.ShowDialog();

            //        dlgRlt = formSubItemPost.DialogResult;
            //        formRet = formSubItemPost.SettingResult;
            //    }
            //    // 名前を付けて保存(間柱リスト設定)
            //    else if (formRet == 11)
            //    {
            //        // 名前を付けて保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formSubItemPost.ShowDialog();

            //        dlgRlt = formSubItemPost.DialogResult;
            //        formRet = formSubItemPost.SettingResult;
            //    }
            //    // 上書き保存(梁リスト設定)
            //    else if (formRet == 12)
            //    {
            //        // 上書き保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formBeamSetting.ShowDialog();

            //        dlgRlt = formBeamSetting.DialogResult;
            //        formRet = formBeamSetting.SettingResult;
            //    }
            //    // 名前を付けて保存(梁リスト設定)
            //    else if (formRet == 13)
            //    {
            //        // 名前を付けて保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formBeamSetting.ShowDialog();

            //        dlgRlt = formBeamSetting.DialogResult;
            //        formRet = formBeamSetting.SettingResult;
            //    }
            //    // 上書き保存(小梁リスト設定)
            //    else if (formRet == 14)
            //    {
            //        // 上書き保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.OverWriteSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formSubItemBeam.ShowDialog();

            //        dlgRlt = formSubItemBeam.DialogResult;
            //        formRet = formSubItemBeam.SettingResult;
            //    }
            //    // 名前を付けて保存(小梁リスト設定)
            //    else if (formRet == 15)
            //    {
            //        // 名前を付けて保存
            //        Collections.Generic.IList<string> commonAry = formCommonSetting.SettingValues_Common;
            //        Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
            //        Collections.Generic.IList<string> subItemPostAry = formSubItemPost.SettingValues_SubItemPost;
            //        Collections.Generic.IList<string> beamAry = formBeamSetting.SettingValues_Beam;
            //        Collections.Generic.IList<string> subItemBeamAry = formSubItemBeam.SettingValues_BeamSub;
            //        Collections.Generic.IList<string> braceAry = formBrace.SettingValues_Brace;

            //        cmpService.SaveAsSettingValues(commonAry, columnAry, subItemPostAry, beamAry, subItemBeamAry, braceAry);

            //        formSubItemBeam.ShowDialog();

            //        dlgRlt = formSubItemBeam.DialogResult;
            //        formRet = formSubItemBeam.SettingResult;
            //    }
            //}

            //// OK以外
            //if (dlgRlt != System.Windows.Forms.DialogResult.OK)
            //{
            //    retCmd = Autodesk.Revit.UI.Result.Cancelled;

            //    transacGrp.RollBack();
            //}

            //// OK
            //if (dlgRlt == System.Windows.Forms.DialogResult.OK)
            //{
            //    retCmd = Autodesk.Revit.UI.Result.Succeeded;

            //    transacGrp.Assimilate();
            //}

            cmpParameters.SetSharedParamDefault();

            return retCmd;
        }

        #endregion Member Functions
    }
}