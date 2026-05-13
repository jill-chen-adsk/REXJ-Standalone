using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Config
{
    /// ================================================================================
    /// <summary>コマンド 設定</summary>
    /// ================================================================================
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CmdCorrectFramingPlan : Autodesk.Revit.UI.IExternalCommand
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
        /// <history>2011/11/25 Created GSA,Inc. Shinichi Ishii</history>
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
            RvtExtApp.Components.CFP_Elements cmpElements = new RvtExtApp.Components.CFP_Elements(rvtUIDoc);
            RvtExtApp.Components.CFP_Geometry cmpGeometry = new RvtExtApp.Components.CFP_Geometry(rvtUIDoc);
            RvtExtApp.Components.CFP_Parameters cmpParameters = new RvtExtApp.Components.CFP_Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.CFP_Settings cmpSettings = new RvtExtApp.Components.CFP_Settings(rvtUIDoc);
            RvtExtApp.Components.CFP_Service cmpService = new RvtExtApp.Components.CFP_Service(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);

            // 戻り値
            Autodesk.Revit.UI.Result retExtCom = Autodesk.Revit.UI.Result.Cancelled;
            // トランザクショングループ
            TransactionGroup transGroup = new TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_CORRECTFRAMINGPLAN"));

            Transaction trans = new Transaction(cmpElements.RvtDBDoc);

            try
            {
                // アクティブ平面図ビュー
                ViewPlan viewPlan = cmpElements.ActiveViewAreaPlan;
                if (viewPlan == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_OPENVIEWPLAN"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // レベル要素
                IList<Level> elemLevels = cmpElements.Levels;
                if (elemLevels.Count == 0)
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // レベルデータ
                trans.Start("DtLevel");

                RvtExtApp.Entities.DtLevel entDtLevel = new RvtExtApp.Entities.DtLevel(cmpAttribute,
                                                                                       cmpElements,
                                                                                       cmpGeometry,
                                                                                       cmpParameters,
                                                                                       cmpSettings);
                if (entDtLevel.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtLevel.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();
                entDtLevel.GetData(elemLevels);
                entDtLevel.SetDefault();

                // アクティブレベル
                Level activeLevel = viewPlan.GenLevel;
                if (activeLevel == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETLEVELVIEW"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                entDtLevel.ActiveLevel = activeLevel;

                // 画面表示
                RvtExtApp.Config.FormConfig form = new RvtExtApp.Config.FormConfig(cmpAttribute, entDtLevel);

                form.ShowDialog();
                if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // ビュー処理
                trans.Start("WorkView");
                if (cmpService.WorkView(viewPlan, entDtLevel) == false)
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(cmpService.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                // レベルデータ設定
                entDtLevel.SetData(elemLevels);

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
