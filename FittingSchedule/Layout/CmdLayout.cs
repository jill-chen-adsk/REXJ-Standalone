using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using ADSK.JExtRAC.FittingSchedule.Components;

namespace ADSK.JExtRAC.FittingSchedule.Layout
{
    /// ================================================================================
    /// <summary>コマンド 建具姿図レイアウト</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdLayout : Revit.UI.IExternalCommand
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
        /// <history><p>2011/08/03 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/02 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/13 Modified Applied Technology</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            // 初期化
            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            System.Text.StringBuilder strLog = new System.Text.StringBuilder();
            Collections.Generic.IList<Revit.DB.ViewSection> viewSections = new Collections.Generic.List<Revit.DB.ViewSection>();
            string errMsg = string.Empty;

            // プログレスバー
            ProgressBarThread progressBarThread = new ProgressBarThread(false, true);

            System.Windows.Forms.DialogResult retDlg;

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_LAYOUTPARTSDRAWING"));

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // 現在ビューチェック[シート]
                if (cmpElements.ActiveViewSheet == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_VIEWSHEET"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 要素 - プロジェクト情報
                Revit.DB.ProjectInfo elemProjInfo = cmpElements.ProjectInfo;

                // コマンドデータ
                trans.Start("SetCommand");
                RvtExtApp.Entities.DtCmd entDtCmd = new RvtExtApp.Entities.DtCmd(cmpAttribute,
                                                                                 cmpElements,
                                                                                 cmpGeometry,
                                                                                 cmpParameters,
                                                                                 cmpSettings,
                                                                                 elemProjInfo,
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_LAYOUT"),
                                                                                 5);
                if (entDtCmd.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // データテーブル - ビューシート
                RvtExtApp.Entities.DtViewSheet entDtViewSheet = new RvtExtApp.Entities.DtViewSheet(cmpAttribute,
                                                                                                   cmpElements,
                                                                                                   cmpGeometry,
                                                                                                   cmpParameters,
                                                                                                   cmpSettings);
                entDtViewSheet.GetDataLayout(entDtCmd.Data[0], entDtCmd.Data[1], entDtCmd.Data[2], entDtCmd.Data[3], entDtCmd.Data[4]);

                // Form show
                // 画面表示
                RvtExtApp.Layout.FormLayoutPartsDrawing form = new RvtExtApp.Layout.FormLayoutPartsDrawing(cmpAttribute,
                                                                                                           entDtViewSheet,
                                                                                                           entDtCmd);
                retDlg = form.ShowDialog();
                if (retDlg == System.Windows.Forms.DialogResult.OK)
                {
                    // コマンドデータ設定
                    trans.Start("SaveCommandData");
                    entDtCmd.SetData();
                    trans.Commit();
                }
                else
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // データテーブル - ビュー
                RvtExtApp.Entities.DtView entDtView = new RvtExtApp.Entities.DtView(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);

                // データテーブル - 建具タイプ
                RvtExtApp.Entities.DtWinDoorType entDtWinDoorType = new RvtExtApp.Entities.DtWinDoorType(cmpAttribute,
                                                                                                         cmpElements,
                                                                                                         cmpGeometry,
                                                                                                         cmpParameters,
                                                                                                         cmpSettings);

                // サービス
                RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                           cmpElements,
                                                                                           cmpGeometry,
                                                                                           cmpParameters,
                                                                                           cmpSettings,
                                                                                           entDtWinDoorType.EntSpWinDoorType,
                                                                                           entDtView.EntSpView);
                // プログレスバー表示
                progressBarThread.ShowDialog();

                // ビューリスト取得
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_GETVIEWPARTS"), 0);
                //Collections.Generic.IList<Revit.DB.ViewSection> viewSections = new Collections.Generic.List<Revit.DB.ViewSection>();

                Collections.Generic.IList<int> viewsLayoutStatus = new Collections.Generic.List<int>();
                if (cmpService.GetViewSection(entDtViewSheet.DataViewTarget, ref viewSections, ref viewsLayoutStatus, ref progressBarThread, ref errMsg) == false)
                {
                    progressBarThread.Close();

                    strLog.AppendLine("-----------------------");
                    strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_GETVIEWPARTS"));
                    if (errMsg != string.Empty)
                        strLog.AppendLine(errMsg);
                    strLog.AppendLine("\t" + cmpElements.ActiveViewSheet.Category.Name + ": " + (rvtUIDoc.Document.GetElement(cmpElements.ActiveViewSheet.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + cmpElements.ActiveViewSheet.Name + " [ID: " + cmpElements.ActiveViewSheet.Id.ToString() + "]");
                    foreach (var fa in viewSections)
                    {
                        if (fa == null)
                            continue;
                        strLog.AppendLine("\t" + fa.Category.Name + ": " + (rvtUIDoc.Document.GetElement(fa.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + fa.Name + " [ID: " + fa.Id.ToString() + "]");
                    }
                    strLog.AppendLine("-----------------------");
                    // show form log
                    if (strLog.Length != 0)
                    {
                        RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                        frmLog.ShowDialog();
                    }
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 建具姿図ビューのレイアウトを設定
                trans.Start("SetLayoutPartsView");
                errMsg = string.Empty;
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_LAYOUTVIEWPARTS"), 0);
                if (cmpService.SetLayoutPartsView(cmpElements.ActiveViewSheet,
                                                  entDtViewSheet.BlankTop,
                                                  entDtViewSheet.BlankBottom,
                                                  entDtViewSheet.BlankLeft,
                                                  entDtViewSheet.BlankRight,
                                                  ref viewSections,
                                                  ref viewsLayoutStatus,
                                                  ref progressBarThread, ref errMsg) == false)
                {
                    trans.RollBack();
                    progressBarThread.Close();

                    strLog.AppendLine("-----------------------");
                    strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_LAYOUTPARTSDRAW"));
                    if (errMsg != string.Empty)
                        strLog.AppendLine(errMsg);
                    strLog.AppendLine("\t" + cmpElements.ActiveViewSheet.Category.Name + ": " + (rvtUIDoc.Document.GetElement(cmpElements.ActiveViewSheet.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + cmpElements.ActiveViewSheet.Name + " [ID: " + cmpElements.ActiveViewSheet.Id.ToString() + "]");
                    foreach (var fa in viewSections)
                    {
                        if (fa == null)
                            continue;
                        strLog.AppendLine("\t" + fa.Category.Name + ": " + (rvtUIDoc.Document.GetElement(fa.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + fa.Name + " [ID: " + fa.Id.ToString() + "]");
                    }
                    strLog.AppendLine("-----------------------");
                    // show form log
                    if (strLog.Length != 0)
                    {
                        RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                        frmLog.ShowDialog();
                    }
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // ビューポートを設定
                trans.Start("SetViewPort");
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_SETVIEWPORT"), 0);
                errMsg = string.Empty;
                if (cmpService.SetViewPort(cmpElements.ActiveViewSheet,
                                           ref viewSections,
                                           ref progressBarThread, ref errMsg) == false)
                {
                    trans.RollBack();
                    progressBarThread.Close();

                    strLog.AppendLine("-----------------------");
                    strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_SETVIEWPORT"));
                    if (errMsg != string.Empty)
                        strLog.AppendLine(errMsg);
                    strLog.AppendLine("\t" + cmpElements.ActiveViewSheet.Category.Name + ": " + (rvtUIDoc.Document.GetElement(cmpElements.ActiveViewSheet.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + cmpElements.ActiveViewSheet.Name + " [ID: " + cmpElements.ActiveViewSheet.Id.ToString() + "]");
                    foreach (var fa in viewSections)
                    {
                        if (fa == null)
                            continue;
                        strLog.AppendLine("\t" + fa.Category.Name + ": " + (rvtUIDoc.Document.GetElement(fa.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + fa.Name + " [ID: " + fa.Id.ToString() + "]");
                    }
                    strLog.AppendLine("-----------------------");
                    // show form log
                    if (strLog.Length != 0)
                    {
                        RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                        frmLog.ShowDialog();
                    }
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                progressBarThread.Close();

                if (errMsg != string.Empty)
                {
                    strLog.AppendLine("-----------------------");
                    strLog.AppendLine(errMsg);
                    strLog.AppendLine("-----------------------");
                    RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                    frmLog.ShowDialog();
                }

                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                if (progressBarThread != null)
                {
                    progressBarThread.Close();
                }
                errMsg = ex.Message;

                strLog.AppendLine("-----------------------");
                strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_COMMAND"));
                strLog.AppendLine(errMsg);
                foreach (var fa in viewSections)
                {
                    if (fa == null)
                        continue;
                    strLog.AppendLine("\t" + fa.Category.Name + ": " + (rvtUIDoc.Document.GetElement(fa.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + fa.Name + " [ID: " + fa.Id.ToString() + "]");
                }
                strLog.AppendLine("-----------------------");
                // show form log
                if (strLog.Length != 0)
                {
                    RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                    frmLog.ShowDialog();
                }
                trans.RollBack();
            }

            cmpParameters.SetSharedParamDefault();
            // トランザクションを統合
            transGroup.Assimilate();
            return retExtCom;
        }

        #endregion Member Functions
    }
}
