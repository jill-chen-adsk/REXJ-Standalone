using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;
using System.Linq;

namespace ADSK.JExtRAC.GridDimension.Commands
{
    /// ================================================================================
    /// <summary>コマンド 設定</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdGridDimension : Revit.UI.IExternalCommand
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
        /// <history><p>2011/11/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///         <p>2018/12/11 Modified Applied Technology</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            // 初期化
            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Revit.DB.Document rvtDoc = rvtUIDoc.Document;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(rvtUIDoc.Document,
                                                                                       cmpAttribute,
                                                                                       cmpElements,
                                                                                       cmpGeometry,
                                                                                       cmpParameters,
                                                                                       cmpSettings);

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start("通り芯寸法の作成");

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // アクティブ平面図ビュー
                Revit.DB.View activeView = cmpElements.ActiveView;
                if (activeView == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_OPENVIEWPLAN"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セット - 通り芯
                Collections.Generic.IList<Revit.DB.Element> elemGrids = cmpElements.GetAllGrids(out Collections.Generic.IList<Revit.DB.Element> LstSegmentGrids);
                if (elemGrids.Count == 0)
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOGRIDSELECT"), cmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);

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
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_GRIDDIMENSION"),
                                                                                 8);
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

                // get all Dimension Type
                Revit.DB.FilteredElementCollector dimesionTypeCollector = new Revit.DB.FilteredElementCollector(rvtDoc);
                dimesionTypeCollector.OfClass(typeof(Revit.DB.DimensionType));
                Collections.Generic.IList<Revit.DB.DimensionType> list_dimensionType = dimesionTypeCollector.Cast<Revit.DB.DimensionType>().ToList()
                                                                                        .Where(x => x.StyleType == Revit.DB.DimensionStyleType.Linear)
                                                                                        .Where(x => x.GetSimilarTypes().Count != 0).ToList();

                if (list_dimensionType.Count == 0)
                    return retExtCom;

                // Check has curve
                bool isCurveDim = cmpGeometry.IsHasGridArc(rvtDoc, elemGrids);

                // Get direction of grids
                bool isDirectionX = cmpGeometry.GetDirectionOfGrids(rvtDoc.ActiveView, elemGrids, out int optDirection);

                if (isCurveDim)
                {
                    bool isErrorLog = cmpService.IsMultiIntersection(rvtDoc, elemGrids);
                    if (isErrorLog)
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_HAS_MUTIL_INTERSECTION"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                        cmpParameters.SetSharedParamDefault();

                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                }
                // Check is grid segment but it not parallel
                bool isError = cmpService.IsGridParallel(rvtDoc, LstSegmentGrids);
                if (isError)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_POLYLINES"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();

                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // Check grid x and grid y
                bool hasGridXAndY = cmpGeometry.IsHasDirectionXAndY(elemGrids);
                if (!isCurveDim && hasGridXAndY)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CHOOSE_X_AND_Y"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));

                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 画面表示
                RvtExtApp.UI.FormConfig form = null;
                while (true)
                {
                    // Create form
                    form = new RvtExtApp.UI.FormConfig(cmpAttribute, entDtCmd, list_dimensionType, isCurveDim, optDirection);
                    form.ShowDialog();
                    if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                    {
                        cmpParameters.SetSharedParamDefault();

                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }
                    if (form.GetSelectedCheckBox())
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NO_CHECKBOX"), cmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                    }
                    else
                        break;
                }

                System.Collections.Generic.List<Autodesk.Revit.DB.View> viewList = new System.Collections.Generic.List<Autodesk.Revit.DB.View>();
                Collections.Generic.Dictionary<string, System.Collections.Generic.List<Autodesk.Revit.DB.View>> dic_SortView = new Collections.Generic.Dictionary<string, System.Collections.Generic.List<Autodesk.Revit.DB.View>>();
                if (bool.Parse(entDtCmd.Data[2]))
                {
                    // Get all plan view
                    System.Collections.Generic.ICollection<Autodesk.Revit.DB.Element> elementList = new Revit.DB.FilteredElementCollector(rvtDoc).OfCategory(Revit.DB.BuiltInCategory.OST_Views).ToElements();
                    // Convert element to view section or view plan
                    foreach (Autodesk.Revit.DB.View view in elementList)
                    {
                        // Check valid for view
                        if (view.IsTemplate)
                            continue;

                        if (view is Revit.DB.ViewPlan || view is Revit.DB.ViewSection)
                        {
                            Revit.DB.View mView = view as Revit.DB.View;
                            if (mView == null)
                                continue;

                            // sort view
                            if (dic_SortView.ContainsKey(view.ViewType.ToString()))
                            {
                                dic_SortView[view.ViewType.ToString()].Add(view);
                            }
                            else
                            {
                                Collections.Generic.List<Revit.DB.View> viewListSort = new Collections.Generic.List<Revit.DB.View>();
                                viewListSort.Add(view);
                                dic_SortView.Add(view.ViewType.ToString(), viewListSort);
                            }
                        }
                    }
                    foreach (var item in dic_SortView)
                    {
                        var list = item.Value;

                        list.Sort(delegate (Revit.DB.View v1, Revit.DB.View v2)
                        {
                            return v1.Title.CompareTo(v2.Title);
                        });

                        viewList.AddRange(list);
                    }

                    if (viewList.Count != 0)
                    {
                        // 画面表示
                        RvtExtApp.UI.FormSelectView frmSelectView = new RvtExtApp.UI.FormSelectView(cmpAttribute, entDtCmd, viewList, activeView);

                        frmSelectView.ShowDialog();
                        if (frmSelectView.DialogResult != System.Windows.Forms.DialogResult.OK)
                        {
                            cmpParameters.SetSharedParamDefault();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                    }
                }
                else
                    viewList.Add(rvtDoc.ActiveView);

                // 寸法作成
                trans.Start("CreateDimension");

                foreach (Revit.DB.View view in viewList)
                {
                    // Start sub transaction
                    Revit.DB.SubTransaction subtr = new Revit.DB.SubTransaction(rvtDoc);
                    subtr.Start();
                    try
                    {
                        if (isCurveDim)
                        {
                            if (cmpService.CreateDimensionCurve(view, elemGrids, entDtCmd.Data[1],
                               form.GetSelectDimensionType, form.GetSelectLeft, form.GetSelectRight, form.GetSelectTop, form.GetSelectBottom) == false)
                            {
                                subtr.Dispose();
                                continue;
                            }
                        }
                        else
                        {
                            Revit.DB.Plane plane = Revit.DB.Plane.CreateByNormalAndOrigin(view.ViewDirection, view.UpDirection);
                            if (plane == null)
                                continue;

                            if (cmpService.CreateDimension(view, plane, elemGrids, entDtCmd.Data[0], entDtCmd.Data[1], (form as RvtExtApp.UI.FormConfig).GetSelectDimensionType,
                                isDirectionX, form.GetSelectLeft, form.GetSelectRight, form.GetSelectTop, form.GetSelectBottom) == false)
                            {
                                subtr.Dispose();
                                continue;
                            }
                        }

                        subtr.Commit();
                    }
                    catch
                    {
                        subtr.Dispose();
                    }
                    // コマンドデータ設定
                    entDtCmd.SetData();
                }
                trans.Commit();

                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
            }

            cmpParameters.SetSharedParamDefault();
            // トランザクションを統合
            transGroup.Assimilate();
            return retExtCom;
        }

        #endregion Member Functions
    }
}