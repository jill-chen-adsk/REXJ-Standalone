using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Create
{
    /// ================================================================================
    /// <summary>コマンド 平均地盤面算定</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdCreate : Revit.UI.IExternalCommand
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
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);

            System.Windows.Forms.DialogResult retDlg;

            // 戻り値
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_SYMTAG"));
            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                // 共有パラメータ - 注釈
                RvtExtApp.Entities.SpAnnotation entSpAnnotation = new RvtExtApp.Entities.SpAnnotation(cmpAttribute,
                                                                                                      cmpParameters,
                                                                                                      cmpSettings);

                // 共有パラメータ - 寸法タイプ
                RvtExtApp.Entities.SpDimType entSpDimType = new RvtExtApp.Entities.SpDimType(cmpAttribute,
                                                                                             cmpParameters,
                                                                                             cmpSettings);

                // サービス
                RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                           cmpElements,
                                                                                           cmpGeometry,
                                                                                           cmpParameters,
                                                                                           cmpSettings,
                                                                                           entSpAnnotation,
                                                                                           entSpDimType);
                cmpService.trans = trans;
                cmpElements.trans = trans;

                // 現在ビューチェック[エリア平面図]
                Revit.DB.ViewPlan activeViewAreaPlan = cmpElements.ActiveViewAreaPlan;
                if (activeViewAreaPlan == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_VIEWAREA"));
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セットチェック[エリア境界線]
                Collections.Generic.IList<Revit.DB.CurveElement> areaCurves = cmpElements.SelAreaCurve();

                // 平均地盤面算定ポイント
                Collections.Generic.IList<ObjectTag> aveGlLvlCalcPoss = cmpElements.AveGlLevelCalcPos(activeViewAreaPlan);
                bool flagAreaCurvesConnect = true;
                bool flagEndPosConnect = true;

                // エリア境界線を指定
                bool flagNewCalcPoss = false;
                if (areaCurves.Count > 0)
                {
                    // Is user select element
                    cmpElements._IsSelectElement = true;

                    // 直線チェック
                    if (cmpService.IsLine(areaCurves) == false)
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOLINEAREABOUNDARY"));
                        // トランザクションを統合
                        transGroup.Assimilate();
                        return retExtCom;
                    }

                    // 平均地盤面算定ポイント存在
                    if (aveGlLvlCalcPoss.Count > 0)
                    {
                        retDlg = System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_WAR_DELAVEGLLEVELPOINT"), "",
                                                                      System.Windows.Forms.MessageBoxButtons.OKCancel);
                        if (retDlg == System.Windows.Forms.DialogResult.OK)
                        {
                            trans.Start("DelAveGlLevelCalcPos");
                            cmpElements.DelAveGlLevelCalcPos(aveGlLvlCalcPoss);
                            aveGlLvlCalcPoss = new Collections.Generic.List<ObjectTag>();
                            trans.Commit();
                        }
                        else
                        {
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                    }
                }

                // エリアから平均地盤面算定ポイント作成
                trans.Start("CreateAveGlLevelCalcPos");
                if (aveGlLvlCalcPoss.Count == 0)
                {
                    if (areaCurves.Count > 0)
                    {
                        if (cmpService.CreateAveGlLevelCalcPos(areaCurves,
                                                               ref aveGlLvlCalcPoss,
                                                               ref flagAreaCurvesConnect,
                                                               ref flagEndPosConnect) == false)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATEAVEGLLEVELCALCPOS"));

                            trans.RollBack();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                        if (flagAreaCurvesConnect == false)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTCLOSEDAREABOUNDARY"));

                            trans.RollBack();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                        flagNewCalcPoss = true;
                    }
                }
                trans.Commit();

                // 平均地盤面算定ポイントなし
                if (aveGlLvlCalcPoss.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELAREABOUNDARY"));
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
                                                                                cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_CREATE"),
                                                                                7);
                if (entDtCmd.ErrMsg != "")
                {
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);

                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // データテーブル - 注釈
                RvtExtApp.Entities.DtAnnotation entDtAnnotation = new RvtExtApp.Entities.DtAnnotation(cmpAttribute,
                                                                                                      cmpElements,
                                                                                                      cmpGeometry,
                                                                                                      cmpParameters,
                                                                                                      cmpSettings,
                                                                                                      aveGlLvlCalcPoss,
                                                                                                      flagNewCalcPoss);

                entDtAnnotation.GetValue(flagNewCalcPoss,
                                         entDtCmd.Data[0],
                                         entDtCmd.Data[1],
                                         entDtCmd.Data[2],
                                         entDtCmd.Data[3],
                                         entDtCmd.Data[4],
                                         entDtCmd.Data[5],
                                         entDtCmd.Data[6]);

                trans.Commit();

                //  平均地盤面算定ポイント表示レベル設定
                trans.Start("SetAveGlLevelCalcPosDispLevel");
                cmpService.SetAveGlLevelCalcPosDispLevel(entDtAnnotation, cmpElements._IsSelectElement);
                trans.Commit();

                // Form show
                // 画面表示
                RvtExtApp.Create.FormCalcDraw form = new RvtExtApp.Create.FormCalcDraw(cmpAttribute,
                                                                                       entDtAnnotation,
                                                                                       entDtCmd);
                retDlg = form.ShowDialog();
                if (retDlg != System.Windows.Forms.DialogResult.Cancel)
                {
                    trans.Start("SetParamValue");

                    // コマンドデータ設定
                    entDtCmd.SetData();

                    entDtAnnotation.SetParamValueAveGlLvlCalcPos();
                    entDtAnnotation.DelAveGlLevelCalcPos();
                    trans.Commit();

                    // 平均地盤面算定ポイント表示レベル設定
                    trans.Start("SettAveGlLevelCalcPosDispLevel2");
                    cmpService.SetAveGlLevelCalcPosDispLevel(entDtAnnotation, cmpElements._IsSelectElement);

                    trans.Commit();

                    if (retDlg == System.Windows.Forms.DialogResult.Yes)
                    {
                        // 算定図作成
                        trans.Start("CreateCalcDrawing");
                        Revit.DB.Dimension elemDim1 = null;
                        Revit.DB.Dimension elemDim2 = null;
                        Collections.Generic.IList<int> numbers = new Collections.Generic.List<int>();
                        Collections.Generic.IList<double> levels = new Collections.Generic.List<double>();

                        if (cmpService.CreateCalcDrawing(activeViewAreaPlan.Name,
                                                         entDtAnnotation.BMHeight,
                                                         entDtAnnotation.Scale,
                                                         entDtAnnotation.RaiteHorizontal,
                                                         entDtAnnotation.RaiteVertical,
                                                         entDtAnnotation.TableAveGlLvlCalcPos,
                                                         flagEndPosConnect,
                                                         ref elemDim1,
                                                         ref elemDim2,
                                                         ref numbers,
                                                         ref levels) == false)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATECALCDRAW"));

                            trans.RollBack();
                            cmpParameters.SetSharedParamDefault();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                        trans.Commit();

                        // 根拠式作成
                        //trans.Start("CreateGroundsExp");
                        if (cmpService.CreateGroundsExp(activeViewAreaPlan.Name,
                                                        elemDim1,
                                                        elemDim2,
                                                        numbers,
                                                        levels,
                                                        entDtAnnotation.LengthUnit,
                                                        entDtAnnotation.AreaDecimal,
                                                        entDtAnnotation.AreaRoundingOpt) == false)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATEGROUNDSEXP"));

                            //trans.RollBack();
                            cmpParameters.SetSharedParamDefault();
                            // トランザクションを統合
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                        //trans.Commit();
                    }
                }
                else
                {
                    cmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return retExtCom;
                }

                // 選択セット解除
                cmpElements.ReleaseElementsSelection();

                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                string errMsg = cmpAttribute.ResourceText("IDS_ERR_COMMAND")
                    + System.Environment.NewLine + System.Environment.NewLine
                    + ex.GetType().Name + ": " + ex.Message;
                System.Windows.Forms.MessageBox.Show(errMsg);

                if (trans.GetStatus() != Revit.DB.TransactionStatus.Committed)
                {
                    trans.RollBack();
                }
            }

            cmpParameters.SetSharedParamDefault();
            // トランザクションを統合
            transGroup.Assimilate();

            return retExtCom;
        }

        #endregion Member Functions
    }
}