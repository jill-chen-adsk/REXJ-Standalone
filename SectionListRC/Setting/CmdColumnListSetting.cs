using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListRC.Setting
{
    /// ================================================================================
    /// <summary>コマンド 柱リスト設定</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdColumnListSetting : Revit.UI.IExternalCommand
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
        /// <history><p>2013/02/05 Created GSA,Inc. Ryo Kuroda</p>
        ///          <p>2013/02/07 Modified GSA,Inc. Ryo Kuroda</p></history>
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
            SectionListRC.Components.UI cmpUI = new SectionListRC.Components.UI(rvtUiApp);
            SectionListRC.Components.Service cmpService = new SectionListRC.Components.Service(cmpAttribute,
                                                                                                 cmpElements,
                                                                                                 cmpGeometry,
                                                                                                 cmpParameters,
                                                                                                 cmpSettings);
            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_BTN_COLUMNLISTSETTING_NAME"));

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"));

                transGroup.Assimilate();
                return retCmd;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"));

                transGroup.Assimilate();
                return retCmd;
            }

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
            string levelSortORder = "";
            cmpService.GetString(ref settingFileName, ref settingFileDirectory, ref levelSortORder);

            // 文字タイプ
            Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypeAry = cmpElements.TxtNoteTypes;
            // 寸法タイプ
            Collections.Generic.IList<Revit.DB.DimensionType> dimTypeAry = cmpElements.DimTypes;
            // 線種タイプ
            Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyleAry = cmpElements.DetailGraStyles;

            // フォーム表示
            SectionListRC.Setting.FormSetting formSetting = new SectionListRC.Setting.FormSetting(cmpAttribute, settingFileName, settingFileDirectory, txtNoteTypeAry, dimTypeAry, graStyleAry);
            SectionListRC.Setting.FormColumnListSetting formColumnSetting = new SectionListRC.Setting.FormColumnListSetting(cmpAttribute, settingFileName, settingFileDirectory);
            SectionListRC.Setting.FormBeamListSetting1 formBeamSetting1 = new SectionListRC.Setting.FormBeamListSetting1(cmpAttribute, settingFileName, settingFileDirectory);
            SectionListRC.Setting.FormBeamListSetting2 formBeamSetting2 = new SectionListRC.Setting.FormBeamListSetting2(cmpAttribute, settingFileName, settingFileDirectory);

            System.Windows.Forms.DialogResult dlgRet = formColumnSetting.ShowDialog();

            int formRet = formColumnSetting.ColumnListSettingResult;

            while (dlgRet == System.Windows.Forms.DialogResult.Yes)
            {
                if (formRet == 0)
                {
                    formSetting.ShowDialog();

                    dlgRet = formSetting.DialogResult;
                    formRet = formSetting.SettingResult;
                }
                else if (formRet == 1)
                {
                    formColumnSetting.ShowDialog();

                    dlgRet = formColumnSetting.DialogResult;
                    formRet = formColumnSetting.ColumnListSettingResult;
                }
                else if (formRet == 2)
                {
                    formBeamSetting1.ShowDialog();

                    dlgRet = formBeamSetting1.DialogResult;
                    formRet = formBeamSetting1.BeamListSettingResult1;
                }
                else if (formRet == 3)
                {
                    formBeamSetting2.ShowDialog();

                    dlgRet = formBeamSetting2.DialogResult;
                    formRet = formBeamSetting2.BeamListSettingResult2;
                }
                else if (formRet == 4)
                {
                }
                else if (formRet == 5)
                {
                    // 設定ファイルの取得先を上書き
                    settingFileName = formSetting.SettingFileName;
                    settingFileDirectory = formSetting.SettingFileDirectory;
                    cmpParameters.GetStrVal(settingFileName, settingFileDirectory, cmpService.GetStringLevelSortOrder);
                    cmpService.Set();
                    cmpService.SetInfoFile(settingFileName, settingFileDirectory, transac);

                    formSetting = new SectionListRC.Setting.FormSetting(cmpAttribute, settingFileName, settingFileDirectory, txtNoteTypeAry, dimTypeAry, graStyleAry);
                    formColumnSetting = new SectionListRC.Setting.FormColumnListSetting(cmpAttribute, settingFileName, settingFileDirectory);
                    formBeamSetting1 = new SectionListRC.Setting.FormBeamListSetting1(cmpAttribute, settingFileName, settingFileDirectory);
                    formBeamSetting2 = new SectionListRC.Setting.FormBeamListSetting2(cmpAttribute, settingFileName, settingFileDirectory);

                    // データテーブルを上書き

                    formSetting.ShowDialog();

                    dlgRet = formSetting.DialogResult;
                    formRet = formSetting.SettingResult;
                }
                else if (formRet == 6)
                {
                    // 上書き保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.OverWriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formSetting.ShowDialog();

                    dlgRet = formSetting.DialogResult;
                    formRet = formSetting.SettingResult;
                }
                else if (formRet == 7)
                {
                    // 名前を付けて保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.WriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formSetting.ShowDialog();

                    dlgRet = formSetting.DialogResult;
                    formRet = formSetting.SettingResult;
                }
                else if (formRet == 8)
                {
                    // 上書き
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.OverWriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formColumnSetting.ShowDialog();

                    dlgRet = formColumnSetting.DialogResult;
                    formRet = formColumnSetting.ColumnListSettingResult;
                }
                else if (formRet == 9)
                {
                    // 名前をつけて保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.WriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formColumnSetting.ShowDialog();

                    dlgRet = formColumnSetting.DialogResult;
                    formRet = formColumnSetting.ColumnListSettingResult;
                }
                else if (formRet == 10)
                {
                    // 上書き保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.OverWriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formBeamSetting1.ShowDialog();

                    dlgRet = formBeamSetting1.DialogResult;
                    formRet = formBeamSetting1.BeamListSettingResult1;
                }
                else if (formRet == 11)
                {
                    // 名前を付けて保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.WriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formBeamSetting1.ShowDialog();

                    dlgRet = formBeamSetting1.DialogResult;
                    formRet = formBeamSetting1.BeamListSettingResult1;
                }
                else if (formRet == 12)
                {
                    // 上書き保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.OverWriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formBeamSetting2.ShowDialog();

                    dlgRet = formBeamSetting2.DialogResult;
                    formRet = formBeamSetting2.BeamListSettingResult2;
                }
                else if (formRet == 13)
                {
                    // 名前を付けて保存
                    Collections.Generic.IList<string> commonAry = formSetting.SettingValues_Common;
                    Collections.Generic.IList<string> columnAry = formColumnSetting.SettingValues_Column;
                    Collections.Generic.IList<string> beamAry1 = formBeamSetting1.SettingValues_Beam1;
                    Collections.Generic.IList<string> beamAry2 = formBeamSetting2.SettingValues_Beam2;
                    Collections.Generic.IList<string> paramAry = formColumnSetting.SettingValues_WriteParam;
                    paramAry.Add(formBeamSetting1.SettingValue_WriteParam);
                    cmpService.WriteSettingValues(commonAry, columnAry, beamAry1, beamAry2, paramAry);

                    formBeamSetting2.ShowDialog();

                    dlgRet = formBeamSetting2.DialogResult;
                    formRet = formBeamSetting2.BeamListSettingResult2;
                }
            }

            // キャンセル
            if (dlgRet != System.Windows.Forms.DialogResult.OK)
            {
                transGroup.Assimilate();
                return retCmd;
            }

            // OK
            if (dlgRet == System.Windows.Forms.DialogResult.OK)
            {
                // 値をセット

                retCmd = Revit.UI.Result.Succeeded;
            }

            transGroup.Assimilate();
            return retCmd;
        }

        #endregion Member Functions
    }
}