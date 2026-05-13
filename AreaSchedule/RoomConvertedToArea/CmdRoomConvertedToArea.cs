
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.RoomConvertedToArea
{
    /// ================================================================================
    /// <summary>コマンド 部屋をエリアに変換</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdRoomConvertedToArea : Revit.UI.IExternalCommand
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
        ///          <p>2012/06/05 Modified GSA,Inc. Shinichi Ishii</p></history>
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

            System.Windows.Forms.DialogResult retDlg;

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_CONVERTAREABOUNDARY"));

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // 現在ビューチェック[エリア平面図]
                Revit.DB.ViewPlan activeViewAreaPlan = cmpElements.ActiveViewAreaPlan;
                if (activeViewAreaPlan == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セットチェック[部屋]
                Collections.Generic.IList<Revit.DB.Architecture.Room> selSetRooms = cmpElements.SelSetRooms;
                if (selSetRooms.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELROOM"));
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
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_ROOMCONVERTEDTOAREA"),
                                                                                 4);
                if (entDtCmd.ErrMsg != "")
                {
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);

                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // データテーブル - 部屋
                RvtExtApp.Entities.DtRoom entDtRoom = new RvtExtApp.Entities.DtRoom(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);
                entDtRoom.GetDataRoomConvertedToArea(entDtCmd.Data[0]);

                // データテーブル - エリア
                RvtExtApp.Entities.DtArea entDtArea = new RvtExtApp.Entities.DtArea(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);
                entDtArea.GetDataRoomConvertedToArea(entDtCmd.Data[1], entDtCmd.Data[2], entDtCmd.Data[3]);

                // 画面表示
                RvtExtApp.RoomConvertedToArea.FormChoiceWork form = new RvtExtApp.RoomConvertedToArea.FormChoiceWork(cmpAttribute,
                                                                                                                     entDtRoom,
                                                                                                                     entDtArea,
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

                // サービス
                RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                           cmpElements,
                                                                                           cmpGeometry,
                                                                                           cmpParameters,
                                                                                           cmpSettings,
                                                                                           entDtArea.EntSpArea,
                                                                                           entDtRoom.EntSpRoom);

                // 面積計算オプションをチェック
                if (cmpService.CheckRoomBndLocType(entDtRoom.RoomBndLocType) == false)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETAREACALCOPT"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Start("SetRoomBndLocType_Change");
                cmpService.SetRoomBndLocType(entDtRoom.RoomBndLocType);
                trans.Commit();

                // プログレスバー表示
                progressBarThread.ShowDialog();

                // 部屋境界からエリア境界を作成
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_CREATEAREABOUNDARY"), 0);
                if (cmpService.CreateAreaBndByRoomBnd(selSetRooms, activeViewAreaPlan, ref progressBarThread, ref trans) == false)
                {
                    progressBarThread.Close();
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATEAREABNDBYROOMBND"));

                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 部屋からエリアを作成
                trans.Start("CreateAreaByRoom");
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_CREATEAREA"), 0);
                Collections.Generic.IList<Revit.DB.Area> areas = new Collections.Generic.List<Revit.DB.Area>();
                if (cmpService.CreateAreaByRoom(selSetRooms, activeViewAreaPlan, ref areas, ref progressBarThread) == false)
                {
                    progressBarThread.Close();
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATEAREABYROOM"));

                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // エリアタグ作成
                trans.Start("CreateAreaTag");
                if (entDtArea.ChkAddAreaTag == true)
                {
                    progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_CREATEAREATAG"), 0);
                    if (cmpService.CreateAreaTag(activeViewAreaPlan, areas, entDtArea.TagNameOpt, entDtArea.TagID, ref progressBarThread) == false)
                    {
                        progressBarThread.Close();
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATEAREATAG"));

                        trans.RollBack();
                        cmpParameters.SetSharedParamDefault();
                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }
                trans.Commit();

                // 面積計算オプションを元に戻す
                trans.Start("SetRoomBndLocType_Restore");
                cmpService.SetRoomBndLocType(entDtRoom.RvtRoomBndLocType);
                trans.Commit();

                progressBarThread.Close();
                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                if (progressBarThread != null)
                {
                    progressBarThread.Close();
                }

                if (trans.GetStatus() != Revit.DB.TransactionStatus.Committed)
                {
                    trans.RollBack();
                }

                string errMsg = cmpAttribute.ResourceText("IDS_ERR_COMMAND")
                    + System.Environment.NewLine + System.Environment.NewLine
                    + ex.GetType().Name + ": " + ex.Message;
                System.Windows.Forms.MessageBox.Show(errMsg);
            }

            // トランザクションを統合
            transGroup.Assimilate();

            cmpParameters.SetSharedParamDefault();
            return retExtCom;
        }

        #endregion Member Functions
    }
}