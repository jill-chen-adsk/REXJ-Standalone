
using System;
using System.Windows;
using System.Windows.Interop;
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
                    ShowError(rvtUIApp, cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セットチェック[部屋]
                Collections.Generic.IList<Revit.DB.Architecture.Room> rooms = cmpElements.SelSetRooms;
                Collections.Generic.ICollection<Revit.DB.ElementId> elemIds = rvtUIDoc.Selection.GetElementIds();
                if (elemIds.Count == 0)
                {
                    rooms = cmpElements.GetElementsRoom(1, 1, activeViewAreaPlan.GenLevel);
                    if (rooms.Count == 0)
                    {
                        ShowError(rvtUIApp, cmpAttribute.ResourceText("IDS_ERR_NOTROOM"));
                        cmpParameters.SetSharedParamDefault();
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }
                else
                {
                    rooms = cmpElements.SelSetRooms;
                    if (rooms.Count == 0)
                    {
                        ShowError(rvtUIApp, cmpAttribute.ResourceText("IDS_ERR_SELROOM"));
                        cmpParameters.SetSharedParamDefault();
                        transGroup.Assimilate();
                        return retExtCom;
                    }
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
                    ShowError(rvtUIApp, cmpAttribute.ResourceText("IDS_ERR_SETLEGALAREA"));
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
                    RvtExtApp.LegalArea.FormWarningRoomsWPF form = new RvtExtApp.LegalArea.FormWarningRoomsWPF(cmpAttribute, data);
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
                ShowError(rvtUIApp, errMsg);
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }

        private static void ShowError(Revit.UI.UIApplication rvtUIApp, string msg)
        {
            var ownerWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Opacity = 0,
                Width = 0,
                Height = 0
            };
            new WindowInteropHelper(ownerWindow) { Owner = rvtUIApp.MainWindowHandle };
            ownerWindow.Show();
            MessageBox.Show(ownerWindow, msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ownerWindow.Close();
        }

        private static void SetRevitAsOwner(Revit.UI.UIApplication rvtUIApp, Window window)
        {
            new WindowInteropHelper(window) { Owner = rvtUIApp.MainWindowHandle };
        }

        #endregion Member Functions
    }
}