using System;
using System.Windows;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.LegalArea
{
    /// ================================================================================
    /// <summary>コマンド 法定面積</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdLegalArea : Revit.UI.IExternalCommand
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
        /// <history><p>2011/08/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/02 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);

            // プログレスバー
            ProgressBarThread progressBarThread = new ProgressBarThread(false, true);
            progressBarThread.SetOwner(rvtUIApp.MainWindowHandle);
            progressBarThread.SetCommandTitle(WeaveCommandTitles.LegalArea(cmpAttribute));

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_LEGALAREA"));

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // 現在ビューチェック[エリア平面図]
                Revit.DB.ViewPlan activeViewAreaPlan = cmpElements.ActiveViewAreaPlan;
                if (activeViewAreaPlan == null)
                {
                    ShowError(rvtUIApp, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // Resolve rooms: use selected rooms when present, otherwise all rooms on the area plan level.
                Collections.Generic.IList<Revit.DB.Architecture.Room> selectedRooms = cmpElements.SelSetRooms;
                Collections.Generic.IList<Revit.DB.Architecture.Room> rooms = selectedRooms;
                bool usedSelectedRooms = selectedRooms.Count > 0;
                if (rooms.Count == 0)
                {
                    rooms = cmpElements.GetRoomsForAreaPlan(activeViewAreaPlan);
                }

                if (rooms.Count == 0)
                {
                    string levelName = activeViewAreaPlan.GenLevel?.Name ?? string.Empty;
                    string errMsg = string.IsNullOrEmpty(levelName)
                        ? cmpAttribute.ResourceText("IDS_ERR_NOTROOM")
                        : string.Format(cmpAttribute.ResourceText("IDS_ERR_NOTROOM_ONLEVEL"), levelName);
                    ShowError(rvtUIApp, cmpAttribute, errMsg);
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // Bind shared parameters inside a transaction so they persist in the project
                trans.Start("BindSharedParameters");

                // データテーブル - 部屋
                RvtExtApp.Entities.DtRoom entDtRoom = new RvtExtApp.Entities.DtRoom(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);

                // データテーブル - エリア
                RvtExtApp.Entities.DtArea entDtArea = new RvtExtApp.Entities.DtArea(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);

                trans.Commit();

                // サービス
                RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                           cmpElements,
                                                                                           cmpGeometry,
                                                                                           cmpParameters,
                                                                                           cmpSettings,
                                                                                           entDtArea.EntSpArea,
                                                                                           entDtRoom.EntSpRoom);

                // プログレスバー表示
                progressBarThread.ShowDialog();

                trans.Start("CountArea");
                // 計算面積を集計
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_SETLEGALAREA"), 0);
                Collections.Generic.IList<Revit.DB.Architecture.Room> warningRooms = new Collections.Generic.List<Revit.DB.Architecture.Room>();
                if (cmpService.CountArea(rooms, activeViewAreaPlan, ref warningRooms, ref progressBarThread) == false)
                {
                    trans.RollBack();
                    progressBarThread.Close();
                    ShowError(rvtUIApp, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_SETLEGALAREA"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 警告部屋設定
                progressBarThread.Close();
                if (warningRooms.Count > 0)
                {
                    // 画面表示
                    System.Data.DataTable data = cmpService.GetWarningRoomsTable(warningRooms);
                    RvtExtApp.LegalArea.FormWarningRoomsWPF form = new RvtExtApp.LegalArea.FormWarningRoomsWPF(
                        cmpAttribute, data, cmpGeometry.AreaUnitLabel);
                    SetRevitAsOwner(rvtUIApp, form);
                    bool? dialogResult = form.ShowDialog();
                    if (dialogResult != true)
                    {
                        trans.RollBack();
                        cmpParameters.SetSharedParamDefault();
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }
                else
                {
                    ShowSuccess(
                        rvtUIApp,
                        cmpAttribute,
                        rooms.Count,
                        usedSelectedRooms,
                        activeViewAreaPlan.GenLevel?.Name ?? string.Empty);
                }
                trans.Commit();

                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                if (progressBarThread != null)
                {
                    progressBarThread.Close();
                }
                string errMsg = cmpAttribute.ResourceText("IDS_ERR_COMMAND")
                    + System.Environment.NewLine + System.Environment.NewLine
                    + ex.GetType().Name + ": " + ex.Message;
                ShowError(rvtUIApp, cmpAttribute, errMsg);
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }

        private static void ShowError(Revit.UI.UIApplication rvtUIApp, RvtExtApp.Components.Attribute cmpAttribute, string msg)
        {
            WeaveDialogHost.ShowMessage(
                rvtUIApp.MainWindowHandle,
                msg,
                WeaveCommandTitles.LegalArea(cmpAttribute),
                cmpAttribute.ResourceText("IDS_TXT_OK"));
        }

        private static void ShowSuccess(
            Revit.UI.UIApplication rvtUIApp,
            RvtExtApp.Components.Attribute cmpAttribute,
            int roomCount,
            bool usedSelectedRooms,
            string levelName)
        {
            string message = usedSelectedRooms
                ? string.Format(cmpAttribute.ResourceText("IDS_TXT_LEGALAREA_SELECTED_COMPLIANT"), roomCount)
                : string.Format(cmpAttribute.ResourceText("IDS_TXT_LEGALAREA_ALL_COMPLIANT"), roomCount, levelName);

            WeaveDialogHost.ShowMessage(
                rvtUIApp.MainWindowHandle,
                message,
                WeaveCommandTitles.LegalArea(cmpAttribute),
                cmpAttribute.ResourceText("IDS_TXT_OK"));
        }

        private static void SetRevitAsOwner(Revit.UI.UIApplication rvtUIApp, Window window)
        {
            WeaveDialogHost.SetOwner(window, rvtUIApp.MainWindowHandle);
        }

        #endregion Member Functions
    }
}