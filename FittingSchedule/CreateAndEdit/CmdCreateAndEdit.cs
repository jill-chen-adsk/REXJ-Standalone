using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using ADSK.JExtRAC.FittingSchedule.Components;

namespace ADSK.JExtRAC.FittingSchedule.CreateAndEdit
{
    /// ================================================================================
    /// <summary>コマンド 建具姿図作成・更新</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdCreateAndEdit : Revit.UI.IExternalCommand
    {
        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>コマンド実行処理</summary>
        ///
        /// <param name="commandData" >Revit コマンドデータ</param>
        /// <param name="message"     >エラーメッセージ</param>
        /// <param name="elements"    >エラー要素</param>
        /// <returns>実行結果</returns>
        ///
        /// <history><p>2011/08/02 Created  GSA,Inc. Shinichi Ishii</p>
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
            string errMsg = string.Empty;
            // 対象建具
            Collections.Generic.IList<Revit.DB.FamilyInstance> familyInstances = new Collections.Generic.List<Revit.DB.FamilyInstance>();
            // プログレスバー
            ProgressBarThread progressBarThread = new ProgressBarThread(false, true);

            System.Windows.Forms.DialogResult retDlg;

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_CREATEPARTSDRAWING"));

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // 選択要素数
                Collections.Generic.IList<Revit.DB.FamilyInstance> elemsDoor;
                Collections.Generic.IList<Revit.DB.FamilyInstance> elemsWindow;

                // 選択要素数が0の場合
                Collections.Generic.ICollection<Revit.DB.ElementId> elemIds = rvtUIDoc.Selection.GetElementIds();
                int selSetCount = elemIds.Count;
                if (selSetCount == 0)
                {
                    // 図面から建具取得
                    elemsDoor = cmpElements.ElemntsDoorTypes;
                    elemsWindow = cmpElements.ElemntsWindowTypes;
                    if ((elemsDoor.Count == 0) && (elemsWindow.Count == 0))
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARTS"));
                        cmpParameters.SetSharedParamDefault();
                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }

                // 選択要素が1以上の場合
                else
                {
                    // 選択セットから建具取得
                    elemsDoor = cmpElements.SelSetDoorTypes;
                    elemsWindow = cmpElements.SelSetWindowTypes;
                    if ((elemsDoor.Count == 0) && (elemsWindow.Count == 0))
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELPARTS"));
                        cmpParameters.SetSharedParamDefault();
                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
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
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_CREATEANDEDIT"),
                                                                                 6);
                if (entDtCmd.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);
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
                entDtView.GetDataCreateAndEdit(entDtCmd.Data[2], entDtCmd.Data[3], entDtCmd.Data[4], entDtCmd.Data[5]);

                // データテーブル - 建具タイプ
                RvtExtApp.Entities.DtWinDoorType entDtWinDoorType = new RvtExtApp.Entities.DtWinDoorType(cmpAttribute,
                                                                                                         cmpElements,
                                                                                                         cmpGeometry,
                                                                                                         cmpParameters,
                                                                                                         cmpSettings);
                entDtWinDoorType.GetDataCreateAndEdit(entDtCmd.Data[0], entDtCmd.Data[1]);

                // 画面表示
                RvtExtApp.CreateAndEdit.FormCreatePartsDrawing form = new RvtExtApp.CreateAndEdit.FormCreatePartsDrawing(cmpAttribute,
                                                                                                                         entDtView,
                                                                                                                         entDtWinDoorType,
                                                                                                                         entDtCmd);
                retDlg = form.ShowDialog();
                if (retDlg == System.Windows.Forms.DialogResult.OK)
                {
                    // コマンドデータ設定
                    entDtCmd.SetData();
                }
                else
                {
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // 対象建具
                //Collections.Generic.IList<Revit.DB.FamilyInstance> familyInstances = new Collections.Generic.List<Revit.DB.FamilyInstance>();
                if (elemsDoor != null)
                {
                    if (elemsDoor.Count > 0)
                    {
                        foreach (Revit.DB.FamilyInstance elem in elemsDoor)
                        {
                            familyInstances.Add(elem);
                        }
                    }
                }
                if (elemsWindow != null)
                {
                    if (elemsWindow.Count > 0)
                    {
                        foreach (Revit.DB.FamilyInstance elem in elemsWindow)
                        {
                            familyInstances.Add(elem);
                        }
                    }
                }

                // ビュー縮尺
                int viewScale = 100;
                if (entDtView.ViewScaleDefault > 0)
                {
                    viewScale = entDtView.ViewScaleDefault;
                }
                else
                {
                    if (entDtView.ViewScaleCustom > 0)
                    {
                        viewScale = entDtView.ViewScaleCustom;
                    }
                }

                // サービス
                RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                           cmpElements,
                                                                                           cmpGeometry,
                                                                                           cmpParameters,
                                                                                           cmpSettings,
                                                                                           entDtWinDoorType.EntSpWinDoorType,
                                                                                           entDtView.EntSpView);
                // ビューが重複している時のオプションが古いビューを削除する場合、アクティブビュー削除の確認
                if (entDtView.DuplicateViewOpt == 0 && rvtUIDoc.ActiveView.ViewType == Revit.DB.ViewType.Section
                  && (rvtUIDoc.ActiveView.Name.StartsWith(cmpAttribute.ResourceText("IDS_LST_SECVIEW_DOOR")) || rvtUIDoc.ActiveView.Name.StartsWith(cmpAttribute.ResourceText("IDS_LST_SECVIEW_WINDOW"))))
                {
                    foreach (Revit.DB.FamilyInstance familyInstance in familyInstances)
                    {
                        // 姿図名
                        string partsDrawName = " ";
                        Revit.DB.BuiltInCategory categoryType = cmpSettings.GetPartsSymbolType(familyInstance.Symbol);
                        if (categoryType == Revit.DB.BuiltInCategory.OST_Doors)
                        {
                            partsDrawName = cmpAttribute.ResourceText("IDS_LST_SECVIEW_DOOR");
                        }
                        else if (categoryType == Revit.DB.BuiltInCategory.OST_Windows)
                        {
                            partsDrawName = cmpAttribute.ResourceText("IDS_LST_SECVIEW_WINDOW");
                        }
                        // 建具名
                        string partsName = cmpService.GetPartsName(familyInstance.Symbol);
                        // ビュー名
                        string viewName = cmpService.SetPartsViewName(familyInstance.Symbol, partsName, partsDrawName);
                        if (viewName == rvtUIDoc.ActiveView.Name)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_DELETEACTIVEVIEW"));
                            cmpParameters.SetSharedParamDefault();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                    }
                }
                trans.Start("SetPartsView");

                // プログレスバー表示
                progressBarThread.ShowDialog();
                // 姿図ビュー作成
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_CREATEVIEWPARTS"), 0);

                if (cmpService.SetPartsView(familyInstances,
                                            entDtWinDoorType.IdDoorTag,
                                            entDtWinDoorType.IdWindowTag,
                                            viewScale,
                                            entDtView.ViewDetailLevel,
                                            entDtView.DuplicateViewOpt,
                                            ref progressBarThread, ref errMsg) == false)
                {
                    trans.RollBack();
                    progressBarThread.Close();

                    if (errMsg == "VIEWSEC_FAILD") {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_VIEWSEC"), cmpAttribute.ResourceText("IDS_TXT_INFO"));
                    }
                    else {
                        strLog.AppendLine("-----------------------");
                        strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_CREATEPARTSDRAW"));

                        if (errMsg != string.Empty)
                            strLog.AppendLine(errMsg);
                        foreach (var fa in familyInstances) {
                            if (fa == null)
                                continue;
                            strLog.AppendLine("\t" + fa.Category.Name + ": " + (rvtUIDoc.Document.GetElement(fa.GetTypeId()) as Revit.DB.ElementType).FamilyName + ": " + fa.Name + " [ID: " + fa.Id.ToString() + "]");
                        }
                        strLog.AppendLine("-----------------------");
                        // show form log
                        if (strLog.Length != 0) {
                            RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(cmpAttribute, strLog);
                            frmLog.ShowDialog();
                        }
                    }
                    cmpParameters.SetSharedParamDefault();

                    // トランザクションを統合
                    transGroup.Assimilate();

                    return retExtCom;
                }
                trans.Commit();

                progressBarThread.Close();
                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                if (trans.HasStarted())
                {
                    trans.RollBack();
                }
                if (progressBarThread != null)
                {
                    progressBarThread.Close();
                }
                errMsg = ex.Message;

                strLog.AppendLine("-----------------------");
                strLog.AppendLine(cmpAttribute.ResourceText("IDS_ERR_COMMAND"));
                strLog.AppendLine(errMsg);
                foreach (var fa in familyInstances)
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

            // トランザクションを統合
            transGroup.Assimilate();

            cmpParameters.SetSharedParamDefault();
            return retExtCom;
        }

        #endregion Member Functions
    }
}
