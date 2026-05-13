using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening
{
  /// ================================================================================
  /// <summary>コマンド 島フカシ</summary>
  /// ================================================================================
  [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdOpening : Revit.UI.IExternalCommand
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
    /// <history><p>2016/11/17 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/01/10 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                            ref string message,
                            Revit.DB.ElementSet elements)
    {
      // 初期化
      Revit.UI.UIApplication                rvtUIApp  = commandData.Application;
      Revit.UI.UIDocument                   rvtUIDoc  = rvtUIApp.ActiveUIDocument;
      Revit.DB.Document                     rvtDBDoc  = rvtUIDoc.Document;
      Revit.ApplicationServices.Application rvtSvcApp = rvtDBDoc.Application;

      RvtExtApp.Components.Attribute  cmpAttribute  = new RvtExtApp.Components.Attribute();
      RvtExtApp.Components.Elements   cmpElements   = new RvtExtApp.Components.Elements(rvtUIDoc, cmpAttribute);
      RvtExtApp.Components.Geometry   cmpGeometry   = new RvtExtApp.Components.Geometry(rvtUIDoc);
      RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(rvtUIDoc, cmpAttribute);
      RvtExtApp.Components.Settings   cmpSettings   = new RvtExtApp.Components.Settings(rvtUIDoc);
      RvtExtApp.Components.Service    cmpService    = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                       cmpElements,
                                                                                       cmpGeometry,
                                                                                       cmpParameters,
                                                                                       cmpSettings);
      RvtExtApp.Components.UI cmpUI = new RvtExtApp.Components.UI(cmpAttribute,
                                                                  rvtUIApp);

      // 戻り値
      Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

      // トランザクショングループ
      Revit.DB.TransactionGroup transGrp = new Revit.DB.TransactionGroup(rvtDBDoc);
      transGrp.Start(cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));
      // トランザクション
      Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDBDoc);

      // ワークフロー
      trans.Start(cmpAttribute.ResourceText("IDS_TXT_FLOW"));
      string retMsg = cmpService.WorkFlow();
      trans.Commit();

      if (retMsg != null)
      {
        System.Windows.Forms.MessageBox.Show(retMsg);
        retCmd = Revit.UI.Result.Failed;
      }

      // ビュー
      Revit.DB.View actView = rvtDBDoc.ActiveView;

      // 平面図以外
      if (actView as Revit.DB.ViewPlan == null)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FLOORPLAN"),
                                             cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

        transGrp.RollBack();
        return retCmd;
      }

      // レベル
      Revit.DB.Level viewLevel = actView.GenLevel;
      if (viewLevel == null)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETVIEWLEVEL"),
                                             cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

        transGrp.RollBack();
        return retCmd;
      }
      double viewElevation = viewLevel.Elevation;

      // 作業面
      if (actView.SketchPlane == null)
      {
        trans.Start("作業面");
        Revit.DB.SketchPlane sktPln = Revit.DB.SketchPlane.Create(rvtDBDoc, viewLevel.Id);
        actView.SketchPlane = sktPln;
        trans.Commit();
      }

      // ビューの向き
      Revit.DB.XYZ viewDirection = actView.ViewDirection;
      
      // カテゴリ
      Revit.DB.Category category = Revit.DB.Category.GetCategory(rvtDBDoc, Revit.DB.BuiltInCategory.OST_GenericModel);
      
      // ビュー全体表示
      // コマンド実行直前に行った作業が再描画されていない場合がある
      // 再描画さていないと形状情報が正しく取得できない
      cmpElements.FitActiveView(actView);

      // ビュー再ズーム
      cmpElements.ZoomActiveView(actView);

      //DnfCom.ProgressBarThread thread = new DnfCom.ProgressBarThread(false, false);
      //thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 0);
      //thread.ShowDialog();

      // 辺取得
      // ビューで表示されている範囲に限定
      //Collections.Generic.IList<Revit.DB.Curve> allCurveAry = cmpService.GetInViewEdgeCurves3();
      //// ビューの範囲に限定
      //Collections.Generic.IList<Revit.DB.Curve> allCurveAry = cmpGeometry.GetAllEdgeCurves();
      
      //DnfCom.ProgressBarThread thread = new DnfCom.ProgressBarThread(false, false);
      //thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 0);
      //thread.ShowDialog();

      //thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 1);
      //thread.Active();

      //// カーブの交点位置取得
      //Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> interPosAry = cmpGeometry.GetInterPosCurves(allCurveAry);

      //thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 2);
      //thread.Active();

      //// カーブ交点の平面カーブ取得
      //Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> curvesTmp = cmpGeometry.GetPlanFaceCurveInterPos(interPosAry, viewElevation);

      //thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 3);
      //thread.Active();
      //System.Threading.Thread.Sleep(300);
      //thread.Close();

      //// ビュー再ズーム
      //cmpElements.ZoomActiveView(actView);

      // マテリアルID
      Revit.DB.ElementId materialId = Revit.DB.ElementId.InvalidElementId;

      // オフセット値
      string strOffsetVal = "";

      while (true)
      {
        Collections.Generic.ICollection<Revit.DB.Reference> pickObjs = new Collections.Generic.List<Revit.DB.Reference>();

        try
        {
          pickObjs = rvtUIDoc.Selection.PickObjects(Revit.UI.Selection.ObjectType.Element);
        }
        catch (Revit.Exceptions.OperationCanceledException)
        {
          System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_FINISHCMD"),
                                               cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

          break;
        }

        // 辺取得
        // ビューで表示されている範囲に限定
        Collections.Generic.IList<Revit.DB.Curve> allCurveAry = cmpService.GetInViewEdgeCurves3(pickObjs);
        
        ADSK.Ext.Fukashi.Utils.ProgressBarForm thread = new ADSK.Ext.Fukashi.Utils.ProgressBarForm(false, false);
        thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 0);
        thread.ShowDialog();

        thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 1);
        thread.Active();

        // カーブの交点位置取得
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> interPosAry = cmpGeometry.GetInterPosCurves(allCurveAry);

        thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 2);
        thread.Active();

        // カーブ交点の平面カーブ取得
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> curvesTmp = cmpGeometry.GetPlanFaceCurveInterPos(interPosAry, viewElevation);

        thread.SetData(cmpAttribute.ResourceText("IDS_TXT_EDGEGETTING"), 3, 3);
        thread.Active();
        System.Threading.Thread.Sleep(300);
        thread.Close();

        while (true)
        {
          //  点指定
          Revit.DB.XYZ pickPos = null;

          try
          {
            pickPos = rvtUIDoc.Selection.PickPoint();
          }
          catch (Revit.Exceptions.OperationCanceledException)
          {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_FINISHCMD"),
                                                 cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

            break;
          }

          // フカシ高さ
          #region フカシ高さ

          // レベル高さ
          double levelHeight = 0;

          string cmbBoxVal = cmpUI.GetCurrentUpperLevelCmbBoxValue();

          int id = 0;
          if (int.TryParse(cmbBoxVal, out id) == false)
          {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETLEVEL"),
                                                 cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

            continue;
          }

          Revit.DB.ElementId levelId = new Autodesk.Revit.DB.ElementId(id);

          // 上レベル
          Revit.DB.Level upperLvl = rvtDBDoc.GetElement(levelId) as Revit.DB.Level;
          if (upperLvl != null)
          {
            double upperHeight = upperLvl.Elevation;
            levelHeight = upperHeight - viewElevation;
          }

          // オフセット値
          strOffsetVal = cmpUI.GetCurrentOffsetValue();
          double offset = 0;

          if (string.IsNullOrEmpty(strOffsetVal) == false)
          {
            if (double.TryParse(strOffsetVal, out offset) == false)
            {
              System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_OFFSETVALUE"),
                                                   cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

              continue;
            }
          }

          // フカシ高さ
          double fukashiHeight = levelHeight + (offset / 304.8);

          if (fukashiHeight <= 0)
          {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FUKASIHEIGHT"),
                                                 cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

            continue;
          }
          #endregion

          // マテリアル
          #region マテリアル
          cmbBoxVal = cmpUI.GetCurrentMaterialCmbBoxValue();
          id = 0;
          if (int.TryParse(cmbBoxVal, out id) == false)
          {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETMATERIAL"),
                                                 cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));

            continue;
          }

          materialId = new Revit.DB.ElementId(id);
          #endregion

          bool created = false;

          for (int i = 0; i < curvesTmp.Count; ++i)
          {
            Collections.Generic.IList<Revit.DB.Curve> curveAry = new Collections.Generic.List<Revit.DB.Curve>();
            for (int j = 0; j < curvesTmp[i].Count; ++j)
            {
              curveAry.Add(curvesTmp[i][j]);
            }

            curveAry = cmpGeometry.OptimizeLineVertexConvLine(curveAry);

            // 多角形の重心
            Revit.DB.XYZ polGravity = cmpGeometry.PolygonGravity2D(curveAry);

            // 位置補正(重心が多角形の外の場合)
            if (curveAry.Count > 3)
            {
              polGravity = cmpGeometry.PointAdjustInCurves(polGravity, curveAry);
              if (polGravity == null)
              {
                continue;
              }
            }

            // 指定点がカーブ内
            if (cmpGeometry.IsPointInPolygon(curveAry, polGravity, pickPos, 0))
            {
              int mode = cmpGeometry.CurvesGeometryMode(curveAry);

              Revit.DB.FamilyInstance famIns = null;
              Revit.DB.DirectShape directShape = null;

              // 三角形
              if (mode == 1)
              {
                famIns = cmpService.CreateFukashi_Triangle(curveAry,
                                                           fukashiHeight,
                                                           materialId);
              }
              // 台形
              else if (mode == 2)
              {
                famIns = cmpService.CreateFukashi_Torapezoid(curveAry,
                                                             fukashiHeight,
                                                             materialId);
              }
              // 長方形、正方形
              else if (mode == 3 || mode == 4)
              {
                famIns = cmpService.CreateFukashi_Rectangle(curveAry,
                                                            fukashiHeight,
                                                            materialId);
              }
              // ひし形、平行四辺形
              else if (mode == 5 || mode == 6)
              {
                famIns = cmpService.CreateFukashi_Parallelogram(curveAry,
                                                                fukashiHeight,
                                                                materialId);
              }
              // L字形
              else if (mode == 7)
              {
                famIns = cmpService.CreateFukashi_Lshape(curveAry,
                                                         fukashiHeight,
                                                         materialId);
              }
              // 凸形
              else if (mode == 8)
              {
                famIns = cmpService.CreateFukashi_Convex(curveAry,
                                                         fukashiHeight,
                                                         materialId);
              }
              // 凹型
              else if (mode == 9)
              {
                famIns = cmpService.CreateFukashi_Concave(curveAry,
                                                          fukashiHeight,
                                                          materialId);
              }
              // DirectShape
              else if (mode == 10)
              {
                directShape = cmpService.CreateFukashi_DirectShape(category,
                                                                   curveAry,
                                                                   viewDirection,
                                                                   fukashiHeight,
                                                                   materialId,
                                                                   Revit.DB.ElementId.InvalidElementId);
              }

              // 要素内は削除
              if (famIns != null)
              {
                bool inElement = cmpService.InElementInView(famIns);

                if (inElement)
                {
                  Revit.UI.TaskDialog taskDlg = new Revit.UI.TaskDialog(cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));
                  taskDlg.MainInstruction = cmpAttribute.ResourceText("IDS_TXT_INELEMENT_INSTRUCTION");
                  taskDlg.MainContent = cmpAttribute.ResourceText("IDS_TXT_INELEMENT_CONTENT");
                  taskDlg.AddCommandLink(Autodesk.Revit.UI.TaskDialogCommandLinkId.CommandLink1, cmpAttribute.ResourceText("IDS_TXT_INELEMENT_OK"));
                  taskDlg.AddCommandLink(Autodesk.Revit.UI.TaskDialogCommandLinkId.CommandLink2, cmpAttribute.ResourceText("IDS_TXT_INELEMENT_NG"));

                  Revit.UI.TaskDialogResult taskDlgRlt = taskDlg.Show();

                  if (taskDlgRlt == Revit.UI.TaskDialogResult.CommandLink2)
                  {
                    trans.Start("削除");
                    rvtDBDoc.Delete(famIns.Id);
                    trans.Commit();
                    famIns = null;
                  }
                }
              }

              if (famIns != null ||
                  directShape != null)
              {
                created = true;
                break;
              }
            }
          }

          if (created == false)
          {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATE"),
                                                 cmpAttribute.ResourceText("IDS_TXT_FUKASHIOPENING"));
          }
        }

        break;
      }


      // 設定値の保存
      // （マテリアルの取得はビュー変更イベント時）
      cmpParameters.GetStrVal(materialId.ToString(), strOffsetVal);
      trans.Start("設定値保存");
      cmpService.Set();
      trans.Commit();

      trans.Start("初期共有パラメータファイル");
      cmpParameters.SetSharedParamDefault();
      trans.Commit();

      transGrp.Assimilate();

      retCmd = Autodesk.Revit.UI.Result.Succeeded;
      return retCmd;
    }
    #endregion
  }
}
