using System;

using Autodesk.Revit.DB;


using RvtExtApp = RSTExtension;

namespace RSTExtension.Config
{
    /// ================================================================================
    /// <summary>コマンド 設定</summary>
    /// ================================================================================
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CmdExclusionSpecialMention : Autodesk.Revit.UI.IExternalCommand
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
        /// <history><p>2011/11/25 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        Autodesk.Revit.UI.Result Execute(Autodesk.Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                ElementSet elements)
        {
            // 初期化
            Autodesk.Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Autodesk.Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.ESM_Elements cmpElements = new RvtExtApp.Components.ESM_Elements(rvtUIDoc);
            RvtExtApp.Components.ESM_Geometry cmpGeometry = new RvtExtApp.Components.ESM_Geometry(rvtUIDoc);
            RvtExtApp.Components.ESM_Parameters cmpParameters = new RvtExtApp.Components.ESM_Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.ESM_Settings cmpSettings = new RvtExtApp.Components.ESM_Settings(rvtUIDoc);
            RvtExtApp.Components.ESM_Service cmpService = new RvtExtApp.Components.ESM_Service(rvtUIDoc.Document,
                                                                                            cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);

            // 戻り値
            Autodesk.Revit.UI.Result retExtCom = Autodesk.Revit.UI.Result.Cancelled;

            System.Windows.Forms.DialogResult retDlg;

            // トランザクショングループ
            TransactionGroup transGroup = new TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_EXCLUSIONSPECIALMENTION"));
            Transaction trans = new Transaction(cmpElements.RvtDBDoc);

            try
            {
                // アクティブビュー
                View activeView = cmpElements.RvtDBDoc.ActiveView;
                if (activeView == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEVIEW"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // ビューデータ
                trans.Start("DtView");
                RvtExtApp.Entities.Exclusion.DtView entDtView = new RvtExtApp.Entities.Exclusion.DtView(cmpAttribute,
                                                                                    cmpElements,
                                                                                    cmpGeometry,
                                                                                    cmpParameters,
                                                                                    cmpSettings);
                if (entDtView.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtView.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtView.GetData(activeView);

                // 壁タグデータ
                trans.Start("DtWallTag");
                RvtExtApp.Entities.Exclusion.DtWallTag entDtWallTag = new RvtExtApp.Entities.Exclusion.DtWallTag(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);
                if (entDtWallTag.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtWallTag.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtWallTag.GetData();

                // 柱タグデータ
                trans.Start("DtColumnTag");
                RvtExtApp.Entities.Exclusion.DtColumnTag entDtColumnTag = new RvtExtApp.Entities.Exclusion.DtColumnTag(cmpAttribute,
                                                                                                   cmpElements,
                                                                                                   cmpGeometry,
                                                                                                   cmpParameters,
                                                                                                   cmpSettings);
                if (entDtColumnTag.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtColumnTag.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtColumnTag.GetData();

                // 梁タグデータ
                trans.Start("DtBeamTag");
                RvtExtApp.Entities.Exclusion.DtBeamTag entDtBeamTag = new RvtExtApp.Entities.Exclusion.DtBeamTag(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);
                if (entDtBeamTag.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtBeamTag.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtBeamTag.GetData();

                // スラブタグデータ
                trans.Start("DtSlabTag");
                RvtExtApp.Entities.Exclusion.DtSlabTag entDtSlabTag = new RvtExtApp.Entities.Exclusion.DtSlabTag(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);
                if (entDtSlabTag.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtSlabTag.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtSlabTag.GetData();

                // 基礎タグデータ
                trans.Start("DtFoundationTag");
                RvtExtApp.Entities.Exclusion.DtFoundationTag entDtFoundationTag = new RvtExtApp.Entities.Exclusion.DtFoundationTag(cmpAttribute,
                                                                                                               cmpElements,
                                                                                                               cmpGeometry,
                                                                                                               cmpParameters,
                                                                                                               cmpSettings);
                if (entDtFoundationTag.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtFoundationTag.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtFoundationTag.GetData();

                // 画面表示
                RvtExtApp.Config.ESM_FormConfig form = new RvtExtApp.Config.ESM_FormConfig(cmpAttribute,
                                                                                   entDtView,
                                                                                   entDtWallTag,
                                                                                   entDtColumnTag,
                                                                                   entDtBeamTag,
                                                                                   entDtSlabTag,
                                                                                   entDtFoundationTag);

                retDlg = System.Windows.Forms.DialogResult.Retry;

                while (retDlg == System.Windows.Forms.DialogResult.Retry)
                {
                    retDlg = form.ShowDialog();
                    form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

                    if (retDlg == System.Windows.Forms.DialogResult.Retry)
                    {
                        // 書出し処理
                        trans.Start("WorkOutput");
                        if (cmpService.WorkOutput(activeView,
                                                  entDtWallTag,
                                                  entDtColumnTag,
                                                  entDtBeamTag,
                                                  entDtSlabTag,
                                                  entDtFoundationTag) == false)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpService.ErrMsg);
                        }
                        trans.Commit();
                    }
                }

                if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // ビュー処理
                int mode = 0;
                if (form.DialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    mode = 1;
                }
                else if (form.DialogResult == System.Windows.Forms.DialogResult.No)
                {
                    mode = 2;
                }
                if (mode > 0)
                {
                    trans.Start("WorkView");
                    if (cmpService.WorkView(mode,
                                            activeView,
                                            entDtWallTag,
                                            entDtColumnTag,
                                            entDtBeamTag,
                                            entDtSlabTag,
                                            entDtFoundationTag) == false)
                    {
                        trans.RollBack();
                        System.Windows.Forms.MessageBox.Show(cmpService.ErrMsg);
                        cmpParameters.SetSharedParamDefault();
                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                    trans.Commit();
                }

                // ビューデータ設定
                if (mode == 2)
                {
                    trans.Start("SetData");
                    entDtView.SetData(activeView);
                    trans.Commit();
                }
                retExtCom = Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"));
            }

            cmpParameters.SetSharedParamDefault();
            // トランザクションを統合
            transGroup.Assimilate();
            return retExtCom;
        }

        #endregion Member Functions
    }
}
