using System;
using System.Linq;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;


namespace Quantity
{
  /// ================================================================================
  /// <summary>コマンド 拾い書書き出し - 配管</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdQuantityPipeExport : Revit.UI.IExternalCommand
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
    /// <history><p>2015/11/25 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/03/18 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                            ref string message,
                            Revit.DB.ElementSet elements)
    {
      // 初期化
      Revit.UI.UIApplication                rvtUiApp    = commandData.Application;
      Revit.UI.UIDocument                   rvtUiDoc    = rvtUiApp.ActiveUIDocument;
      Revit.DB.Document                     rvtDbDoc    = rvtUiApp.ActiveUIDocument.Document;
      Revit.ApplicationServices.Application rvtSrvcApp  = rvtUiApp.Application;
      Quantity.Components.Attribute  cmpAttribute  = new Quantity.Components.Attribute();
      Quantity.Components.Elements   cmpElements   = new Quantity.Components.Elements(rvtUiDoc,
                                                                                                            cmpAttribute);
      Quantity.Components.Geometry   cmpGeometry   = new Quantity.Components.Geometry(rvtUiDoc,
                                                                                                            cmpAttribute);
      Quantity.Components.Parameters cmpParameters = new Quantity.Components.Parameters(rvtUiDoc,
                                                                                                              cmpAttribute);
      Quantity.Components.Settings   cmpSettings   = new Quantity.Components.Settings(rvtUiDoc,
                                                                                                            cmpAttribute);
      Quantity.Components.Service    cmpService    = new Quantity.Components.Service(cmpElements,
                                                                                                           cmpGeometry,
                                                                                                           cmpParameters,
                                                                                                           cmpSettings,
                                                                                                           cmpAttribute);

      // 戻り値
      Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

      // 現在ビュー
      Revit.DB.View actView = rvtDbDoc.ActiveView;

      // 平面図ビュー
      Revit.DB.ViewPlan viewPlan = actView as Revit.DB.ViewPlan;

      if (viewPlan == null)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTVIEWPLAN"));
        return retCmd;
      }
      
      // 計算済みID
      Collections.Generic.IList<string> usedIds = new Collections.Generic.List<string>();


      // 書き出しフォルダ選択
      if (cmpService.GetExportFolderPath() == false)
      {
        return retCmd;
      }

      Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(rvtDbDoc);
      transGroup.Start("Pipe");

      Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDbDoc);

      // アンダーレイ非表示
      cmpParameters.HideUnderLay(viewPlan);

      // 再描画
      trans.Start("Redraw");
      rvtDbDoc.Regenerate();
      trans.Commit();

      // ビュー内スペース
      Collections.Generic.IList<Revit.DB.Mechanical.Space> allSpaces = cmpService.InViewPlanSpaceAry(viewPlan);
      cmpElements.Spaces = allSpaces;
      cmpGeometry.GetSpacesBndryCrv(allSpaces, 1);

      cmpParameters.GetSpaceElev(allSpaces);
      
      // ビュー範囲
      cmpParameters.GetViewPlanRange(viewPlan);
      
      // ビュー範囲高さ
      double viewRangeTopElev = cmpParameters.ViewRangeTopElevation;
      double viewRangeBtmElev = cmpParameters.ViewRangeBottomElevation;

      // オフセット抜き高さ
      double viewRangeTopElevNotOffset = viewRangeTopElev - cmpParameters.ViewRangeTopOffset;
      double viewRangeBtmElevNotOffset = viewRangeBtmElev - cmpParameters.ViewRangeBottomOffset;

      // ビュー内配管
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> inViewPipes = cmpService.InViewPipe(viewPlan, rvtDbDoc);

      // 選択している場合
      #region 選択している場合

      Revit.DB.ElementSet elemSet = new Revit.DB.ElementSet();
      Collections.Generic.ICollection<Revit.DB.ElementId> selElemIds = rvtUiDoc.Selection.GetElementIds();

      foreach (Revit.DB.ElementId selElemId in selElemIds)
      {
        Revit.DB.Element selElem = rvtDbDoc.GetElement(selElemId);
        elemSet.Insert(selElem);
      }

      if (elemSet.Size > 0)
      {
        Collections.Generic.IList<Revit.DB.Plumbing.Pipe> selPipes = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

        foreach (Revit.DB.Element elem in elemSet)
        {
          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeCurves).ToString()))
            {
              Revit.DB.Plumbing.Pipe pipe = elem as Revit.DB.Plumbing.Pipe;

              if (pipe != null)
              {
                selPipes.Add(pipe);
              }
            }
          }
        }

        inViewPipes = selPipes;
      }

      #endregion

      // ビュー内ID
      Collections.Generic.IList<string> inViewIds = new Collections.Generic.List<string>();

      foreach (Revit.DB.Plumbing.Pipe p in inViewPipes)
      {
        inViewIds.Add(p.Id.ToString());
      }

      int maxCount = inViewIds.Count;

      // 対象配管なし
      if (maxCount < 1)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOSUBJECT"));

        transGroup.RollBack();

        return retCmd;
      }

      // ビュー領域形状
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = viewPlan.GetCropRegionShapeManager();

      Collections.Generic.IList<Revit.DB.Curve> crvCrop = new Collections.Generic.List<Revit.DB.Curve>();
      Revit.DB.XYZ inAreaPoint = null;

      // トリミング = 形状変形
      if (viewCropMgr.ShapeSet)
      {
        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = viewCropMgr.GetCropShape();

        foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
        {
          foreach (Revit.DB.Curve crv in crvLoop)
          {
            crvCrop.Add(crv);
          }
        }

        // 多角形の重心
        inAreaPoint = cmpGeometry.PolygonGravity2D(crvCrop);
      }
      else
      {
        Revit.DB.BoundingBoxXYZ bbXYZ = viewPlan.CropBox;

        Revit.DB.XYZ pntTopRight = bbXYZ.Max;
        Revit.DB.XYZ pntBtmLeft = bbXYZ.Min;
        Revit.DB.XYZ pntTopLeft = new Revit.DB.XYZ(pntBtmLeft.X, pntTopRight.Y, pntTopRight.Z);
        Revit.DB.XYZ pntBtmRight = new Revit.DB.XYZ(pntTopRight.X, pntBtmLeft.Y, pntTopRight.Z);

        Revit.DB.Line l1 = Revit.DB.Line.CreateBound(pntTopLeft, pntBtmLeft);
        Revit.DB.Line l2 = Revit.DB.Line.CreateBound(pntBtmLeft, pntBtmRight);
        Revit.DB.Line l3 = Revit.DB.Line.CreateBound(pntBtmRight, pntTopRight);
        Revit.DB.Line l4 = Revit.DB.Line.CreateBound(pntTopRight, pntTopLeft);

        crvCrop.Add(l1);
        crvCrop.Add(l2);
        crvCrop.Add(l3);
        crvCrop.Add(l4);

        // 中心点
        inAreaPoint = cmpGeometry.Center2Point(pntBtmLeft, pntTopRight);
      }

      // 数量計算
      #region 数量計算

      cmpParameters._OutPutPipeList = new Collections.Generic.List<Components.OutPutParam>();

      foreach (Revit.DB.Plumbing.Pipe pipe in inViewPipes)
      {
        // システム未分類
        if (pipe.MEPSystem == null)
        {
          continue;
        }

        // 連続する配管として計算済み
        if (usedIds.Contains(pipe.Id.ToString()))
        {
          continue;
        }

        // 管径
        Revit.DB.Parameter parDiameter = pipe.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
        double diameter = parDiameter.AsDouble() * 304.8;
        diameter = cmpGeometry.ToHalfAdjust(diameter, 0);

        string size = diameter.ToString();
        
        // 横管、立管
        bool isVertical = cmpGeometry.IsVerticalSinglePipe(pipe);

        // 連続する(交点を計算できる)配管
        Revit.DB.Plumbing.Pipe pipe1 = null;
        Revit.DB.Plumbing.Pipe pipe2 = null;

        // Pipe1側の点
        Revit.DB.XYZ farPnt1 = null;
        // Pipe2側の点
        Revit.DB.XYZ farPnt2 = null;

        // 直線的に連続する配管の拾い用端点
        Collections.Generic.IList<Revit.DB.XYZ> linePnts = new Collections.Generic.List<Revit.DB.XYZ>();

        // 横管
        #region 横管

        if (isVertical == false)
        {
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> horizontalPipes = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

          // 連続する横管
          cmpService.StraightConnectPipesXY(pipe,
                                            ref farPnt1,
                                            ref farPnt2,
                                            ref pipe1,
                                            ref pipe2,
                                            ref horizontalPipes,
                                            inViewIds);

          // 使用済みに登録
          foreach (Revit.DB.Plumbing.Pipe usePipe in horizontalPipes)
          {
            usedIds.Add(usePipe.Id.ToString());
          }

          // Pipe1側からの順にソート
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sortedPipes = cmpService.SortPipes(horizontalPipes, farPnt1);

          // 全端点
          linePnts = cmpGeometry.GetPoints(sortedPipes, farPnt1);

          if (linePnts.Count < 2)
          {
            continue;
          }

          // 延長上の交点を計算
          if (pipe1 != null)
          {
            // Pipe1側の2点
            Revit.DB.XYZ pnt0 = linePnts[0];
            Revit.DB.XYZ pnt1 = linePnts[1];

            Revit.DB.Line mainLine = Revit.DB.Line.CreateBound(pnt0, pnt1);
            Revit.DB.Line pipe1Line = cmpGeometry.GetPipeLine(pipe1);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(mainLine, pipe1Line);

            if (cross != null)
            {
              // 交点計算用の配管が立管
              if (cmpGeometry.IsVerticalSinglePipe(pipe1))
              {
                cross = new Revit.DB.XYZ(pipe1Line.GetEndPoint(0).X, pipe1Line.GetEndPoint(0).Y, cross.Z);
              }

              // 近い方の点を修正
              if (cmpGeometry.Distance2D(pnt0, cross) < cmpGeometry.Distance2D(pnt1, cross))
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt1))
                {
                  linePnts[0] = cross;
                }
              }
              else
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt0))
                {
                  linePnts[1] = cross;
                }
              }
            }
          }

          if (pipe2 != null)
          {
            // Pipe2側の2点
            Revit.DB.XYZ pnt0 = linePnts[linePnts.Count - 1];
            Revit.DB.XYZ pnt1 = linePnts[linePnts.Count - 2];

            Revit.DB.Line mainLine = Revit.DB.Line.CreateBound(pnt0, pnt1);
            Revit.DB.Line pipe2Line = cmpGeometry.GetPipeLine(pipe2);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(mainLine, pipe2Line);

            if (cross != null)
            {
              // 交点計算用の配管が立管
              if (cmpGeometry.IsVerticalSinglePipe(pipe2))
              {
                cross = new Revit.DB.XYZ(pipe2Line.GetEndPoint(0).X, pipe2Line.GetEndPoint(0).Y, cross.Z);
              }

              // 近い方の点を修正
              if (cmpGeometry.Distance2D(pnt0, cross) < cmpGeometry.Distance2D(pnt1, cross))
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt1))
                {
                  linePnts[linePnts.Count - 1] = cross;
                }
              }
              else
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt0))
                {
                  linePnts[linePnts.Count - 2] = cross;
                }
              }
            }
          }

          // ソート
          linePnts = cmpGeometry.SortNearPoints(linePnts, farPnt1);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);

          // 配管の分岐点追加
          linePnts = cmpService.GetPipeJunctionPoint(linePnts, horizontalPipes);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);

          // 線分領域内に限定
          linePnts = cmpGeometry.GetLineEndPointInLinesArea(linePnts, crvCrop, inAreaPoint);
        }

        #endregion

        // 立管
        #region 立管

        else
        {
          // 連続する横管
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cnctVerticalPipes = cmpService.ConnectVerticalPipe(pipe,
                                                                                                               inViewPipes,
                                                                                                               ref farPnt1,
                                                                                                               ref farPnt2,
                                                                                                               ref pipe1,
                                                                                                               ref pipe2,
                                                                                                               inViewIds);

          // 使用済みに登録
          foreach (Revit.DB.Plumbing.Pipe usePipe in cnctVerticalPipes)
          {
            usedIds.Add(usePipe.Id.ToString());
          }

          // 最大、最小高さ
          double maxZ = 0;
          double minZ = 0;

          cmpGeometry.GetVerticalPipesTopBtm(cnctVerticalPipes, ref maxZ, ref minZ, farPnt1, farPnt2);

          // 高さを制限

          // 上端が下端からのオフセットのみ
          if (viewRangeTopElevNotOffset == viewRangeBtmElevNotOffset)
          {
            // 上下とも(フロア)レベルの範囲外
            if (maxZ > viewRangeTopElev && minZ < viewRangeBtmElev)
            {
              // オフセットを含まない範囲
              if (maxZ > viewRangeTopElev)
              {
                maxZ = viewRangeTopElev;
              }
              if (minZ < viewRangeBtmElevNotOffset)
              {
                minZ = viewRangeBtmElevNotOffset;
              }
            }
            else
            {
              // オフセットを含む範囲
              if (maxZ > viewRangeTopElev)
              {
                maxZ = viewRangeTopElev;
              }
              if (minZ < viewRangeBtmElev)
              {
                minZ = viewRangeBtmElev;
              }
            }
          }
          // 上端を下端とは違うレベルに設定
          else
          {
            // 上下とも(フロア)レベルの範囲外
            if (maxZ > viewRangeTopElevNotOffset && minZ < viewRangeBtmElevNotOffset)
            {
              // オフセットを含まない範囲
              if (maxZ > viewRangeTopElevNotOffset)
              {
                maxZ = viewRangeTopElevNotOffset;
              }
              if (minZ < viewRangeBtmElevNotOffset)
              {
                minZ = viewRangeBtmElevNotOffset;
              }
            }
            else
            {
              if (maxZ > viewRangeTopElev)
              {
                maxZ = viewRangeTopElev;
              }
              if (minZ < viewRangeBtmElev)
              {
                minZ = viewRangeBtmElev;
              }
            }
          }

          // ビュー外
          if (maxZ < minZ || maxZ == minZ)
          {
            continue;
          }

          Revit.DB.XYZ pXY = cmpGeometry.GetPipeLine(pipe).GetEndPoint(0);

          Revit.DB.XYZ pnt1 = new Revit.DB.XYZ(pXY.X, pXY.Y, maxZ);
          Revit.DB.XYZ pnt2 = new Revit.DB.XYZ(pXY.X, pXY.Y, minZ);

          linePnts.Add(pnt1);
          linePnts.Add(pnt2);
        }

        #endregion

        // 計算結果線分
        Collections.Generic.IList<Revit.DB.Line> calLines = new Collections.Generic.List<Revit.DB.Line>();

        for (int i = 0; i < linePnts.Count; ++i)
        {
          if (i == linePnts.Count - 1)
          {
            continue;
          }

          Revit.DB.XYZ ep0 = linePnts[i];
          Revit.DB.XYZ ep1 = linePnts[i + 1];

          if (rvtSrvcApp.ShortCurveTolerance < cmpGeometry.Distance(ep0, ep1))
          {
            Revit.DB.Line line = Revit.DB.Line.CreateBound(ep0, ep1);

            calLines.Add(line);
          }
        }

        foreach (Revit.DB.Line calLine in calLines)
        {
          // スペースとの交点
          Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> crossSpaceAndPoint = cmpService.GetCrossingSpaceAndPoint(calLine);

          foreach (Revit.DB.ElementId elemId in crossSpaceAndPoint.Keys)
          {
            // スペース
            Revit.DB.Mechanical.Space space = rvtDbDoc.GetElement(elemId) as Revit.DB.Mechanical.Space;
            
            // スペースごとの交点座標
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = crossSpaceAndPoint[elemId];

            //CSV出力パラメータセット
            foreach (Collections.Generic.IList<Revit.DB.XYZ> listP in listPnts)
            {
              Components.OutPutParam oparam = cmpService.SetOutPutParameter(1, space, pipe, listP);
              if (oparam != null)
              {
                cmpParameters._OutPutPipeList.Add(oparam);
              }
            }

          }

        }


      }

      #endregion

      // アンダーレイ再表示
      cmpParameters.UnHideUnderLay(viewPlan);

      String csvName = "Pipe_" + viewPlan.Name  + ".csv";
      string errMsg = cmpService.OutPutData(csvName, cmpParameters._OutPutPipeList, 1);

      if (errMsg != "")
      {
        System.Windows.Forms.MessageBox.Show(errMsg);

        transGroup.RollBack();
        return retCmd;
      }


      // 終了メッセージ
      System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_FINISH"),
                                           cmpAttribute.ResourceText("IDS_TXT_QUANTITYEXPORT"),
                                           System.Windows.Forms.MessageBoxButtons.OK,
                                           System.Windows.Forms.MessageBoxIcon.Information,
                                           System.Windows.Forms.MessageBoxDefaultButton.Button1,
                                           System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);

      transGroup.Assimilate();
      retCmd = Revit.UI.Result.Succeeded;
      return retCmd;
    }
    #endregion
  }
}
