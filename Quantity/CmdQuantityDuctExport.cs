using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using System.Windows.Documents;
using System.Linq;

namespace Quantity
{
  /// ================================================================================
  /// <summary>コマンド 拾い書書き出し - ダクト</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdQuantityDuctExport : Revit.UI.IExternalCommand
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
    /// <history><p>2015/12/18 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/04/18 Modified GSA,Inc. Ryo Kuroda</p></history>
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
      transGroup.Start("Duct");

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

      // ビュー範囲高さ
      double viewRangeTopElev = cmpParameters.ViewRangeTopElevation;
      double viewRangeBtmElev = cmpParameters.ViewRangeBottomElevation;

      // オフセット抜き高さ
      double viewRangeTopElevNotOffset = viewRangeTopElev - cmpParameters.ViewRangeTopOffset;
      double viewRangeBtmElevNotOffset = viewRangeBtmElev - cmpParameters.ViewRangeBottomOffset;

      // ビュー内ダクト
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> inViewDucts = cmpService.InViewDuct(viewPlan, rvtDbDoc);

      // ビュー内フレキシブルダクト
      Collections.Generic.IList<Revit.DB.Mechanical.FlexDuct> inViewFlexDucts = cmpService.InViewFlexDuct(viewPlan, rvtDbDoc);

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
        Collections.Generic.IList<Revit.DB.Mechanical.Duct> selDucts = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();
        Collections.Generic.IList<Revit.DB.Mechanical.FlexDuct> selFlexDucts = new Collections.Generic.List<Revit.DB.Mechanical.FlexDuct>();

        foreach (Revit.DB.Element elem in elemSet)
        {
          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
            {
              Revit.DB.Mechanical.Duct duct = elem as Revit.DB.Mechanical.Duct;

              if (duct != null)
              {
                selDucts.Add(duct);
              }
            }
            else if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_FlexDuctCurves).ToString()))
            {
              Revit.DB.Mechanical.FlexDuct flexDuct = elem as Revit.DB.Mechanical.FlexDuct;

              if (flexDuct != null)
              {
                selFlexDucts.Add(flexDuct);
              }
            }
          }
        }

        inViewDucts = selDucts;
        inViewFlexDucts = selFlexDucts;
      }

      #endregion

      // ビュー内ID
      Collections.Generic.IList<string> inViewIds = new Collections.Generic.List<string>();

      foreach (Revit.DB.Mechanical.Duct d in inViewDucts)
      {
        inViewIds.Add(d.Id.ToString());
      }

      foreach (Revit.DB.Mechanical.FlexDuct fd in inViewFlexDucts)
      {
        inViewIds.Add(fd.Id.ToString());
      }

      // カウント
      int maxCount = inViewIds.Count;

      // 対象ダクトなし
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

      cmpParameters._OutPutDuctList = new Collections.Generic.List<Components.OutPutParam>();
      
      // ダクト
      #region ダクト

      foreach (Revit.DB.Mechanical.Duct duct in inViewDucts)
      {
        // システム未分類
        if (duct.MEPSystem == null)
        {
          continue;
        }

        // 連続するダクトとして計算済み
        if (usedIds.Contains(duct.Id.ToString()))
        {
          continue;
        }

        // サイズ
        Revit.DB.Parameter parCalSize = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
        if (parCalSize == null)
        {
          parCalSize = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_DUCT_CALCULATED_SIZE);
        }

        string strCalSize = parCalSize.AsString();

        // 縦ダクト判定
        bool isVerticalDuct = cmpGeometry.IsVerticalSingleDuct(duct);

        // 連続するダクト
        Revit.DB.XYZ farPnt1 = null;
        Revit.DB.XYZ farPnt2 = null;
        Revit.DB.Mechanical.Duct duct1 = null;
        Revit.DB.Mechanical.Duct duct2 = null;

        Collections.Generic.IList<Revit.DB.XYZ> linePnts = new Collections.Generic.List<Revit.DB.XYZ>();

        // 横ダクト
        #region

        if (isVerticalDuct == false)
        {
          // 連続するダクト
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> horizontalDucts = cmpService.StraightConnectDuctsXY(duct,
                                                                                                                  ref farPnt1,
                                                                                                                  ref farPnt2,
                                                                                                                  ref duct1,
                                                                                                                  ref duct2,
                                                                                                                  inViewIds,
                                                                                                                  true);

          // 使用済みに登録
          foreach (Revit.DB.Mechanical.Duct d in horizontalDucts)
          {
            usedIds.Add(d.Id.ToString());
          }

          // duct1側からの順にソート
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sortDucts = cmpService.SortDucts(horizontalDucts, farPnt1);

          // 全端点
          linePnts = cmpGeometry.GetPoints(sortDucts, farPnt1);

          if (linePnts.Count < 2)
          {
            continue;
          }

          // 延長上の交点を計算
          if (duct1 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[0];
            Revit.DB.XYZ pnt1 = linePnts[1];
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);

            Revit.DB.Line _l = cmpGeometry.GetDuctLine(duct1);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cross != null)
            {
              if (cmpGeometry.IsVerticalSingleDuct(duct1))
              {
                cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
              }

              // 近い方を修正
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

          if (duct2 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[linePnts.Count - 1];
            Revit.DB.XYZ pnt1 = linePnts[linePnts.Count - 2];
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);

            Revit.DB.Line _l = cmpGeometry.GetDuctLine(duct2);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cross != null)
            {
              if (cmpGeometry.IsVerticalSingleDuct(duct2))
              {
                cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
              }

              // 近い方を修正
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

          // ダクトの分岐点追加
          linePnts = cmpService.GetDuctJunctionPoint(linePnts, horizontalDucts);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);

          // 線分領域内に限定
          // CropBoxの状態を確認（CropBoxは設定が無い場合も初期範囲が入っているのでそのまま使うのはマズい）
          if ( viewPlan.CropBoxActive ) 
            linePnts = cmpGeometry.GetLineEndPointInLinesArea(linePnts, crvCrop, inAreaPoint);
        }

        #endregion

        // 縦ダクト
        #region

        else if (isVerticalDuct == true)
        {
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> verticalDucts = cmpService.ConnectVerticalDuct(duct,
                                                                                                             inViewDucts,
                                                                                                             ref farPnt1,
                                                                                                             ref farPnt2,
                                                                                                             ref duct1,
                                                                                                             ref duct2,
                                                                                                             inViewIds,
                                                                                                             true);

          // 使用済み
          foreach (Revit.DB.Mechanical.Duct d in verticalDucts)
          {
            usedIds.Add(d.Id.ToString());
          }

          // 最大高さ、最小高さ
          double maxZ = 0;
          double minZ = 0;

          cmpGeometry.GetVerticalDuctTopBbtm(verticalDucts, ref maxZ, ref minZ, farPnt1, farPnt2);

          // 上端が下端からのオフセットのみの場合
          if (viewRangeTopElevNotOffset == viewRangeBtmElevNotOffset)
          {
            // 上下とも(フロア)レベルの範囲外
            if (maxZ > viewRangeTopElev && minZ < viewRangeBtmElevNotOffset)
            {
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
              // オフセット含む範囲
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
          else
          {
            // 上下とも(フロア)レベルの範囲外
            if (maxZ > viewRangeTopElevNotOffset && minZ < viewRangeBtmElevNotOffset)
            {
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
              // オフセット含む範囲
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

          Revit.DB.XYZ p0 = cmpGeometry.GetDuctLine(verticalDucts[0]).GetEndPoint(0);

          Revit.DB.XYZ p1 = new Revit.DB.XYZ(p0.X, p0.Y, maxZ);
          Revit.DB.XYZ p2 = new Revit.DB.XYZ(p0.X, p0.Y, minZ);

          linePnts.Add(p1);
          linePnts.Add(p2);
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
              Components.OutPutParam oparam = cmpService.SetOutPutParameter(0, space, duct, listP);
              if (oparam != null)
              {
                cmpParameters._OutPutDuctList.Add(oparam);
              }
            }
          }
        }
      }

      #endregion

      // フレキシブルダクト
      #region フレキシブルダクト

      foreach (Revit.DB.Mechanical.FlexDuct flexDuct in inViewFlexDucts)
      {
        // システム未分類
        if (flexDuct.MEPSystem == null)
        {
          continue;
        }

        // 連続するダクトとして計算済み
        if (usedIds.Contains(flexDuct.Id.ToString()))
        {
          continue;
        }

        // サイズ
        Revit.DB.Parameter parCalSize = flexDuct.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
        if (parCalSize == null)
        {
          parCalSize = flexDuct.get_Parameter(Revit.DB.BuiltInParameter.RBS_DUCT_CALCULATED_SIZE);
        }

        string strCalSize = parCalSize.AsString();

        // スペースとの交点
        Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> crossSpaceAndPoint = cmpService.GetCrossingSpaceAndPoint(flexDuct);

        foreach (Revit.DB.ElementId elemId in crossSpaceAndPoint.Keys)
        {
          // スペース
          Revit.DB.Mechanical.Space space = rvtDbDoc.GetElement(elemId) as Revit.DB.Mechanical.Space;
          
          // スペースごとの交点
          Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = crossSpaceAndPoint[elemId];

          //CSV出力パラメータセット
          foreach (Collections.Generic.IList<Revit.DB.XYZ> listP in listPnts)
          {
            Components.OutPutParam oparam = cmpService.SetOutPutParameter(0, space, flexDuct, listP);
            if (oparam != null)
            {
              cmpParameters._OutPutDuctList.Add(oparam);
            }
          }
        }
      }

      #endregion

      #endregion

      // アンダーレイ再表示
      cmpParameters.UnHideUnderLay(viewPlan);


      String csvName = "Duct_" + viewPlan.Name + ".csv";
      string errMsg = cmpService.OutPutData(csvName, cmpParameters._OutPutDuctList, 0);

      if (errMsg != "")
      {
        System.Windows.Forms.MessageBox.Show(errMsg);

        transGroup.RollBack();
        return retCmd;
      }

      string msgFinish = cmpAttribute.ResourceText("IDS_TXT_FINISH");
      
      // 終了メッセージ
      System.Windows.Forms.MessageBox.Show(msgFinish,
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
