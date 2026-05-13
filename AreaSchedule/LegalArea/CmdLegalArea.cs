
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
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTROOM"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELROOM"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        cmpParameters.SetSharedParamDefault();
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }

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
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETLEGALAREA"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(errMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }

        #endregion Member Functions
    }
}