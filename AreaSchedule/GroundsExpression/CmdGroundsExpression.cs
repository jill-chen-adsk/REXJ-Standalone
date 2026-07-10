
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.GroundsExpression
{
    /// ================================================================================
    /// <summary>コマンド 根拠式</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdGroundsExpression : Revit.UI.IExternalCommand
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
            IntPtr ownerHandle = rvtUIApp.MainWindowHandle;

            // プログレスバー
            ProgressBarThread progressBarThread = new ProgressBarThread(false, true);
            progressBarThread.SetOwner(ownerHandle);
            progressBarThread.SetCommandTitle(WeaveCommandTitles.GroundsExpression(cmpAttribute));

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_CREATEBASISEXPN"));

            try
            {
                // 現在ビューチェック[エリア平面図]
                Revit.DB.ViewPlan activeViewAreaPlan = cmpElements.ActiveViewAreaPlan;
                if (activeViewAreaPlan == null)
                {
                    ShowMessage(ownerHandle, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セットチェック[エリア]
                if (cmpElements.SelSetAreas.Count == 0)
                {
                    ShowMessage(ownerHandle, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_SELAREA"));
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
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_GROUNDSEXPRESSION"),
                                                                                 6);
                if (entDtCmd.ErrMsg != "")
                {
                    trans.RollBack();
                    ShowMessage(ownerHandle, cmpAttribute, entDtCmd.ErrMsg);
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

                // データテーブル - エリア
                RvtExtApp.Entities.DtArea entDtArea = new RvtExtApp.Entities.DtArea(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);
                entDtArea.GetDataGroundsExpression(entDtCmd.Data[0],
                                                   entDtCmd.Data[1],
                                                   entDtCmd.Data[2],
                                                   entDtCmd.Data[3],
                                                   entDtCmd.Data[4],
                                                   entDtCmd.Data[5]);

                // 画面表示
                RvtExtApp.GroundsExpression.FormCalcAreaWPF form = new RvtExtApp.GroundsExpression.FormCalcAreaWPF(cmpAttribute, entDtArea, entDtCmd);
                bool? dialogResult = WeaveDialogHost.ShowDialog(form, ownerHandle);
                if (dialogResult == true)
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

                trans.Start("SetAreaParameter");
                // エリアのパラメータ設定
                if (cmpService.SetAreaParameter(cmpElements.SelSetAreas, cmpElements.ActiveViewAreaPlan) == false)
                {
                    trans.RollBack();
                    ShowMessage(ownerHandle, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_PARAMAREA"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // プログレスバー表示
                progressBarThread.ShowDialog();

                // 根拠式作成
                trans.Start("CreateBasisExpression");
                progressBarThread.SetData(cmpAttribute.ResourceText("IDS_TXT_CREATEBASISEXPN"), 0);
                string PiOptStr = entDtArea.DataPI.Rows[entDtArea.PiOpt][1].ToString();
                
                // "根拠式"パラメータに根拠式、"計算面積"パラメータに計算面積を入れる。
                
                if (cmpService.CreateBasisExpression(cmpElements.ActiveViewAreaPlan,
                                                     cmpElements.SelSetAreas,
                                                     entDtArea.LengthUnit,
                                                     entDtArea.LengthDecimal,
                                                     entDtArea.LengthRoundingOpt,
                                                     entDtArea.AreaDecimal,
                                                     entDtArea.AreaRoundingOpt,
                                                     PiOptStr,
                                                     ref progressBarThread) == false)
                {
                    trans.RollBack();
                    progressBarThread.Close();
                    ShowMessage(ownerHandle, cmpAttribute, cmpAttribute.ResourceText("IDS_ERR_CREATEBASISEXPRESSION"));
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
                if (progressBarThread != null)
                {
                    progressBarThread.Close();
                }
                string errMsg = cmpAttribute.ResourceText("IDS_ERR_COMMAND")
                    + System.Environment.NewLine + System.Environment.NewLine
                    + ex.GetType().Name + ": " + ex.Message;
                ShowMessage(ownerHandle, cmpAttribute, errMsg);
            }

            // トランザクションを統合
            transGroup.Assimilate();

            cmpParameters.SetSharedParamDefault();
            return retExtCom;
        }

        private static void ShowMessage(
            IntPtr ownerHandle,
            RvtExtApp.Components.Attribute cmpAttribute,
            string message)
        {
            WeaveDialogHost.ShowMessage(
                ownerHandle,
                message,
                WeaveCommandTitles.GroundsExpression(cmpAttribute),
                cmpAttribute.ResourceText("IDS_TXT_OK"));
        }

        #endregion Member Functions
    }
}
