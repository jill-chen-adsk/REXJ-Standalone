using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using System.Linq;

namespace Quantity
{
  /// ================================================================================
  /// <summary>コマンド 拾い図</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdQuantityReason : Revit.UI.IExternalCommand
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
    /// <history><p>2014/07/14 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/09/29 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
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
      Quantity.Components.Geometry   cmpGeometry = new Quantity.Components.Geometry(rvtUiDoc,
                                                                                                          cmpAttribute);
      Quantity.Components.Parameters cmpParameters = new Quantity.Components.Parameters(rvtUiDoc,
                                                                                                              cmpAttribute);
      Quantity.Components.Settings   cmpSettings = new Quantity.Components.Settings(rvtUiDoc,
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

      // プロジェクトブラウザ
      if (actView.ViewType == Revit.DB.ViewType.ProjectBrowser)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_PRJECTVIEW"));
        return retCmd;
      }

      // 平面図
      Revit.DB.ViewPlan vp = actView as Revit.DB.ViewPlan;

      if (vp == null)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTVIEWPLAN"));
        return retCmd;
      }

      // 選択配管
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> selPipes = cmpElements.SelectPipeAry;

      // 選択ダクト
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> selDucts = cmpElements.SelectDuctAry;

      // 選択なし
      if (selPipes.Count < 1 &&
          selDucts.Count < 1)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOSELECT"));
        return retCmd;
      }

      Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(rvtDbDoc);
      transGroup.Start("Quantity Diagram");

      Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDbDoc);

      // ワークフロー
      trans.Start("Workflow");
      string retMsg = cmpService.WorkFlow();
      trans.Commit();
      if (retMsg != null)
      {
        System.Windows.Forms.MessageBox.Show(retMsg);
        retCmd = Revit.UI.Result.Failed;
      }

      // 寸法タイプ
      Revit.DB.DimensionType dimType = null;
      // 文字タイプ
      Revit.DB.TextNoteType textType = null;

      // 詳細線分グラフィックススタイル
      Revit.DB.GraphicsStyle dcGraphicsStyle = null;
      
      // タイプ取得
      cmpService.GetTypes(ref dimType, ref textType);

      // グラフィックススタイル取得
      cmpService.GetDetailCurveGraphicsStyle(ref dcGraphicsStyle, vp, trans);

      // ビュー範囲取得
      cmpParameters.GetViewPlanRange(vp);

      // ビュー範囲高さ
      double viewRangeTopElev = cmpParameters.ViewRangeTopElevation;
      double viewRangeBtmElev = cmpParameters.ViewRangeBottomElevation;

      // オフセット抜き高さ
      double viewRangeTopElevNotOffset = viewRangeTopElev - cmpParameters.ViewRangeTopOffset;
      double viewRangeBtmElevNotOffset = viewRangeBtmElev - cmpParameters.ViewRangeBottomOffset;

      // ビュー内スペース
      Collections.Generic.IList<Revit.DB.Mechanical.Space> allSpaces = new Collections.Generic.List<Revit.DB.Mechanical.Space>();
      allSpaces = cmpService.InViewPlanSpaceAry(vp);
      cmpElements.Spaces = allSpaces;

      // スペース境界線分
      cmpGeometry.GetSpacesBndryCrv(allSpaces, 1);
      // スペース高さ
      cmpParameters.GetSpaceElev(allSpaces);

      // 現在ビュー配管
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> inViewPipes = cmpService.InViewPipe(vp, rvtDbDoc);
      // ビュー内配管ID
      Collections.Generic.IList<string> inViewPipeId = new Collections.Generic.List<string>();
      foreach (Revit.DB.Plumbing.Pipe p in inViewPipes)
      {
        inViewPipeId.Add(p.Id.ToString());
      }

      // ビュー内ダクト
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> inViewDucts = cmpService.InViewDuct(vp, rvtDbDoc);
      // ビュー内ダクトID
      Collections.Generic.IList<string> inViewDuctId = new Collections.Generic.List<string>();
      foreach (Revit.DB.Mechanical.Duct d in inViewDucts)
      {
        inViewDuctId.Add(d.Id.ToString());
      }

      // 使用済み
      Collections.Generic.IList<string> usedIds = new Collections.Generic.List<string>();

      // ビュー範囲形状
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = vp.GetCropRegionShapeManager();

      Collections.Generic.IList<Revit.DB.Curve> crvCrop = new Collections.Generic.List<Revit.DB.Curve>();
      Revit.DB.XYZ inAreaPoint = null;

      // 形状設定 = トリミング
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
      // トリミングなし
      else
      {
        // ビュー表示範囲 = クロップボックス
        Revit.DB.BoundingBoxXYZ bbXYZ = vp.CropBox;

        Revit.DB.XYZ pntTopRight  = bbXYZ.Max;
        Revit.DB.XYZ pntBtmLeft   = bbXYZ.Min;
        Revit.DB.XYZ pntTopLeft   = new Revit.DB.XYZ(pntBtmLeft.X, pntTopRight.Y, pntTopRight.Z);
        Revit.DB.XYZ pntBtmRight  = new Revit.DB.XYZ(pntTopRight.X, pntBtmLeft.Y, pntTopRight.Z);

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

      // 配管寸法
      #region 配管寸法

      foreach (Revit.DB.Plumbing.Pipe pipe in selPipes)
      {
        // 
        if (usedIds.Contains(pipe.Id.ToString()))
        {
          continue;
        }
        else
        {
          usedIds.Add(pipe.Id.ToString());
        }

        // 選択配管だがビュー外
        if (inViewPipeId.Contains(pipe.Id.ToString()) == false)
        {
          continue;
        }
        
        // 配管と連続する配管の端点
        Revit.DB.XYZ farPnt1 = null;
        Revit.DB.XYZ farPnt2 = null;
        Revit.DB.Plumbing.Pipe p1 = null;
        Revit.DB.Plumbing.Pipe p2 = null;

        // 横管
        #region 横管

        if (cmpGeometry.IsVerticalSinglePipe(pipe) == false)
        {
          // 横管、斜め管
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> horizontalPipes = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

          // 連続する配管
          cmpService.StraightConnectPipesXY(pipe,
                                            ref farPnt1,
                                            ref farPnt2,
                                            ref p1,
                                            ref p2,
                                            ref horizontalPipes,
                                            inViewPipeId);

          // 使用済み
          foreach (Revit.DB.Plumbing.Pipe p in horizontalPipes)
          {
            usedIds.Add(p.Id.ToString());
          }

          // 配管ソート
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sortedPipes = cmpService.SortPipes(horizontalPipes, farPnt1);

          // 全配管端点
          Collections.Generic.IList<Revit.DB.XYZ> linePnts = cmpGeometry.GetPoints(sortedPipes, farPnt1);

          if (linePnts.Count < 2)
          {
            continue;
          }

          if (p1 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[0];
            Revit.DB.XYZ pnt1 = linePnts[1];
            trans.Start("Create detail lines");
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);
            trans.Commit();

            Revit.DB.Line _l = cmpGeometry.GetPipeLine(p1);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cmpGeometry.IsVerticalSinglePipe(p1))
            {
              cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
            }

            if (cross != null)
            {
              // 近い方を修正
              if (cmpGeometry.Distance2D(pnt0, cross) < cmpGeometry.Distance2D(pnt1, cross))
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt1))
                {
                  linePnts[0] = cross;// new Revit.DB.XYZ(cross.X, cross.Y, lineEndPnts[0].Z);
                }
              }
              else
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(pnt0, cross))
                {
                  linePnts[1] = cross;// new Revit.DB.XYZ(cross.X, cross.Y, lineEndPnts[1].Z);
                }
              }
            }
          }

          if (p2 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[linePnts.Count - 1];
            Revit.DB.XYZ pnt1 = linePnts[linePnts.Count - 2];
            trans.Start("Create detail lines");
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);
            trans.Commit();

            Revit.DB.Line _l = cmpGeometry.GetPipeLine(p2);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cmpGeometry.IsVerticalSinglePipe(p2))
            {
              cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
            }

            if (cross != null)
            {
              // 近い方を修正
              if (cmpGeometry.Distance2D(pnt0, cross) < cmpGeometry.Distance2D(pnt1, cross))
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(cross, pnt1))
                {
                  linePnts[linePnts.Count - 1] = cross;// new Revit.DB.XYZ(cross.X, cross.Y, lineEndPnts[lineEndPnts.Count - 1].Z);
                }
              }
              else
              {
                // 短くなる場合はそのまま
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(pnt0, cross))
                {
                  linePnts[linePnts.Count - 2] = cross;// new Revit.DB.XYZ(cross.X, cross.Y, lineEndPnts[lineEndPnts.Count - 2].Z);
                }
              }
            }
          }

          linePnts = cmpGeometry.SortNearPoints(linePnts, farPnt1);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);

          //// 始点終点
          //Revit.DB.XYZ start = linePnts[0];
          //Revit.DB.XYZ end = linePnts[linePnts.Count - 1];

          //linePnts.Clear();
          //linePnts.Add(start);
          //linePnts.Add(end);

          // スペース境界との交差判定で点を追加
          linePnts = cmpService.GetSpaceBndryCrossingLine(linePnts, cmpElements.Spaces);//.CurrentViewSpaceAry);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);
          //linePnts = cmpGeometry.SetDoublePnts(linePnts);

          // 配管の分岐点追加
          linePnts = cmpService.GetPipeJunctionPoint(linePnts, horizontalPipes);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);
          //linePnts = cmpGeometry.SetDoublePnts(linePnts);

          // トリミング領域内
          linePnts = cmpGeometry.GetLineEndPointInLinesArea(linePnts, crvCrop, inAreaPoint);
          if (linePnts.Count < 2)
          {
            continue;
          }

          trans.Start("Create pipe detail lines");
          Collections.Generic.IList<Revit.DB.Line> pipeDetailLineLines = cmpGeometry.CreatePipeDetailLines(linePnts, vp.GenLevel.Elevation);
          trans.Commit();
          trans.Start("Create pipe lines");
          Collections.Generic.IList<Revit.DB.Line> pipeLines = cmpGeometry.CreatePipeLines(linePnts);
          trans.Commit();

          //連続する配管の詳細線分
          Collections.Generic.IList<Revit.DB.DetailCurve> dcAry = new Collections.Generic.List<Revit.DB.DetailCurve>();

          // 詳細線分作成
          trans.Start("Create detail lines");
          foreach (Revit.DB.Line l in pipeDetailLineLines)
          {
            if (l == null)
            {
              dcAry.Add(null);
              continue;
            }

            Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, l);
            if (dcGraphicsStyle != null)
            {
              dc.LineStyle = dcGraphicsStyle;
            }
            dcAry.Add(dc);
          }
          trans.Commit();

          // 寸法作成
          for (int i = 0; i < dcAry.Count; ++i)
          {
            Revit.DB.DetailCurve dc = dcAry[i];

            if (dc == null)
            {
              continue;
            }

            Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();
            refAry.Append(dc.GeometryCurve.GetEndPointReference(0));
            refAry.Append(dc.GeometryCurve.GetEndPointReference(1));

            Revit.DB.XYZ ep0 = dc.GeometryCurve.GetEndPoint(0);
            Revit.DB.XYZ ep1 = dc.GeometryCurve.GetEndPoint(1);

            trans.Start("Create detail lines");
            Revit.DB.Line line1 = Revit.DB.Line.CreateBound(ep0, ep1);
            trans.Commit();
            double length = cmpGeometry.ToMetricFromFeet(line1.Length, -3);
            //double length = cmpGeometry.ToHalfAdjust(line1.Length, -9);

            Revit.DB.XYZ _ep0 = linePnts[i];
            Revit.DB.XYZ _ep1 = linePnts[i + 1];
            //Revit.DB.XYZ _ep0 = linePnts[i * 2];
            //Revit.DB.XYZ _ep1 = linePnts[i * 2 + 1];

            //Revit.DB.Line line2 = Revit.DB.Line.CreateBound(_ep0, _ep1);
            //line2 = cmpGeometry.GetLineInLinesArea(line2, crvCrop, inAreaPoint);
            //if (line2 == null)
            //{
            //  continue;
            //}

            double distance = cmpGeometry.ToMetricFromFeet(cmpGeometry.Distance(_ep0, _ep1), -3);
            //double distance = cmpGeometry.ToHalfAdjust(cmpGeometry.Distance(_ep0, _ep1), -9);

            // 作成
            trans.Start("Create dimensions");
            Revit.DB.Dimension dim = cmpElements.CreateDimension(vp, line1, refAry);

            if (dim != null)
            {
              if (dimType != null)
              {
                try
                {
                  dim.ChangeTypeId(dimType.Id);
                }
                catch
                {
                  dimType = rvtDbDoc.GetElement(dim.GetTypeId()) as Revit.DB.DimensionType;
                }
              }

              // 平面寸法 != 端点距離
              if (length != distance)
              {
                // メートル単位
                double metric = cmpGeometry.ToHalfAdjust(distance, -1);
                //double metric = cmpGeometry.ToHalfAdjust(distance * 304.8 / 1000, -1);

                // 最小値
                if (metric < 0.1)
                {
                  metric = 0.1;
                }
                string strMetric = metric.ToString();

                // 小数点以下
                if (strMetric.Contains("."))
                {
                  strMetric = strMetric.Substring(0, strMetric.LastIndexOf(".") + 2);
                }
                else
                {
                  strMetric += ".0";
                }

                // 寸法上書き
                dim.ValueOverride = "∠" + strMetric;
              }
              // 0.1以下
              else
              {
                double? dimVal = dim.Value;

                if (dimVal == null)
                {
                  // 寸法上書き
                  dim.ValueOverride = "0.1";
                }
                else
                {
                  dimVal = cmpGeometry.ToMetricFromFeet((double)dimVal, -1);

                  if (dimVal < 0.1)
                  {
                    // 寸法上書き
                    dim.ValueOverride = "0.1";
                  }
                }
              }
              trans.Commit();
            }
            else
            {
                  trans.RollBack();
            }
          }
        }

        #endregion

        // 縦管
        #region 縦管

        else // if (cmpGeometry.IsVerticalSinglePipe(pipe))
        {
          // 連続する配管
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cnctVerticalPipes = cmpService.ConnectVerticalPipe(pipe,
                                                                                                               inViewPipes,
                                                                                                               ref farPnt1,
                                                                                                               ref farPnt2,
                                                                                                               ref p1,
                                                                                                               ref p2,
                                                                                                               inViewPipeId);

          // 使用済み
          foreach (Revit.DB.Plumbing.Pipe p in cnctVerticalPipes)
          {
            usedIds.Add(p.Id.ToString());
          }

          // 最大高さ、最小高さ
          double maxZ = 0;
          double minZ = 0;

          cmpGeometry.GetVerticalPipesTopBtm(cnctVerticalPipes, ref maxZ, ref minZ, farPnt1, farPnt2);


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

          Revit.DB.Line line = cmpGeometry.GetPipeLine(pipe);

          // 範囲内判定
          line = cmpGeometry.GetLineInLinesArea(line, crvCrop, inAreaPoint);
          if (line == null)
          {
            continue;
          }

          Revit.DB.XYZ ep0 = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, minZ);
          Revit.DB.XYZ ep1 = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, maxZ);

          if (cmpGeometry.Distance(ep0, ep1) <= cmpParameters.LineMinLength)
          {
            continue;
          }

          trans.Start("Create detail lines");
          Revit.DB.Line l = Revit.DB.Line.CreateBound(ep0, ep1);
          trans.Commit();

          // スペースとの交差端点
          Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> crossSpaceAndPoint = cmpService.GetCrossingSpaceAndPoint(l);

          foreach (Revit.DB.ElementId elemId in crossSpaceAndPoint.Keys)
          {
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> dicPnts = crossSpaceAndPoint[elemId];

            foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in dicPnts)
            {
              if (cmpParameters.LineMinLength < cmpGeometry.Distance(pnts[0], pnts[1]))
              {
                double length = System.Math.Abs(pnts[0].Z - pnts[1].Z);

                // ミリメートル単位
                double milliMetric = length * 304.8;
                
                // 最小値
                if (milliMetric < 1)
                {
                  milliMetric = 1;
                }

                string strMilliMetric = Math.Round(milliMetric, 0, MidpointRounding.AwayFromZero).ToString();// milliMetric.ToString();
                
                // 長さ文字
                string strVal = "\u00D8" + strMilliMetric;


                Revit.DB.XYZ origin = line.GetEndPoint(0);
                Revit.DB.XYZ baseVec = vp.RightDirection;
                Revit.DB.HorizontalTextAlignment horizontal = Revit.DB.HorizontalTextAlignment.Center;
                Revit.DB.VerticalTextAlignment vertical = Revit.DB.VerticalTextAlignment.Middle;

                // 文字作成
                Revit.DB.TextNote textNote = cmpService.CreateTextNoteXYPosRotateSet(trans,
                                                                                     vp,
                                                                                     origin,
                                                                                     baseVec,
                                                                                     horizontal,
                                                                                     vertical,
                                                                                     strVal);

                if (textType != null)
                {
                  trans.Start("Change Type");
                  textNote.ChangeTypeId(textType.Id);
                  trans.Commit();
                }
              }
            }
          }
        }

        #endregion
      }

      #endregion

      // ダクト寸法
      #region ダクト寸法

      foreach (Revit.DB.Mechanical.Duct duct in selDucts)
      {
        // 使用済み
        if (usedIds.Contains(duct.Id.ToString()))
        {
          continue;
        }
        else
        {
          usedIds.Add(duct.Id.ToString());
        }

        // 選択ダクトだがビュー外
        if (inViewDuctId.Contains(duct.Id.ToString()) == false)
        {
          continue;
        }

        // ダクトと連続するダクトの端点
        Revit.DB.XYZ farPnt1 = null;
        Revit.DB.XYZ farPnt2 = null;
        Revit.DB.Mechanical.Duct d1 = null;
        Revit.DB.Mechanical.Duct d2 = null;

        // 横ダクト
        #region 横ダクト

        if (cmpGeometry.IsVerticalSingleDuct(duct) == false)
        {
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

          ducts = cmpService.StraightConnectDuctsXY(duct,
                                                    ref farPnt1,
                                                    ref farPnt2,
                                                    ref d1,
                                                    ref d2,
                                                    inViewDuctId,
                                                    false);

          foreach (Revit.DB.Mechanical.Duct d in ducts)
          {
            usedIds.Add(d.Id.ToString());
          }

          // ダクトソート
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sortedDucts = cmpService.SortDucts(ducts, farPnt1);

          // ダクトライン端点
          Collections.Generic.IList<Revit.DB.XYZ> linePnts = cmpGeometry.GetPoints(sortedDucts, farPnt1);

          if (linePnts.Count < 2)
          {
            continue;
          }

          if (d1 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[0];
            Revit.DB.XYZ pnt1 = linePnts[1];
            trans.Start("Create detail lines");
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);
            trans.Commit();

            Revit.DB.Line _l = cmpGeometry.GetDuctLine(d1);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cmpGeometry.IsVerticalSingleDuct(d1))
            {
              cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
            }

            if (cross != null)
            {
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
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(pnt0, cross))
                {
                  linePnts[1] = cross;
                }
              }
            }
          }

          if (d2 != null)
          {
            Revit.DB.XYZ pnt0 = linePnts[linePnts.Count - 1];
            Revit.DB.XYZ pnt1 = linePnts[linePnts.Count - 2];
            trans.Start("Create detail lines");
            Revit.DB.Line l = Revit.DB.Line.CreateBound(pnt0, pnt1);
            trans.Commit();

            Revit.DB.Line _l = cmpGeometry.GetDuctLine(d2);

            // 交点
            Revit.DB.XYZ cross = cmpGeometry.TwoLineCrossPnt(l, _l);

            if (cmpGeometry.IsVerticalSingleDuct(d2))
            {
              cross = new Revit.DB.XYZ(_l.GetEndPoint(0).X, _l.GetEndPoint(0).Y, cross.Z);
            }

            if (cross != null)
            {
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
                if (cmpGeometry.Distance(pnt0, pnt1) < cmpGeometry.Distance(pnt0, cross))
                {
                  linePnts[linePnts.Count - 2] = cross;
                }
              }
            }
          }

          linePnts = cmpGeometry.SortNearPoints(linePnts, farPnt1);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);

          // スペース境界との交差判定で点を追加
          linePnts = cmpService.GetSpaceBndryCrossingLine(linePnts, cmpElements.Spaces);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);
          //linePnts = cmpGeometry.SetDoublePnts(linePnts);

          // ダクトの分岐点追加
          linePnts = cmpService.GetDuctJunctionPoint(linePnts, ducts);
          linePnts = cmpGeometry.SameXYPointRemove(linePnts);
          //linePnts = cmpGeometry.SetDoublePnts(linePnts);

          linePnts = cmpGeometry.GetLineEndPointInLinesArea(linePnts, crvCrop, inAreaPoint);
          if (linePnts.Count < 2)
          {
            continue;
          }


          trans.Start("Create duct detail lines");
          Collections.Generic.IList<Revit.DB.Line> ductDetailLines = cmpGeometry.CreateDuctDetailLines(linePnts, vp.GenLevel.Elevation);
          trans.Commit();

          trans.Start("Create duct lines");
          Collections.Generic.IList<Revit.DB.Line> ductLines = cmpGeometry.CreateDuctLines(linePnts);
          trans.Commit();

          Collections.Generic.IList<Revit.DB.DetailCurve> dcAry = new Collections.Generic.List<Revit.DB.DetailCurve>();

          // 詳細線分作成
          trans.Start("Create detail lines");
          foreach (Revit.DB.Line l in ductDetailLines)
          {
            if (l == null)
            {
              dcAry.Add(null);
              continue;
            }

            Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, l);
            if (dcGraphicsStyle != null)
            {
              dc.LineStyle = dcGraphicsStyle;
            }
            dcAry.Add(dc);
          }
          trans.Commit();

          // 寸法作成
          for (int i = 0; i < dcAry.Count; ++i)
          {
            Revit.DB.DetailCurve dc = dcAry[i];

            if (dc == null)
            {
              continue;
            }

            Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();
            refAry.Append(dc.GeometryCurve.GetEndPointReference(0));
            refAry.Append(dc.GeometryCurve.GetEndPointReference(1));

            Revit.DB.XYZ ep0 = dc.GeometryCurve.GetEndPoint(0);
            Revit.DB.XYZ ep1 = dc.GeometryCurve.GetEndPoint(1);

            trans.Start("Create detail lines");
            Revit.DB.Line line1 = Revit.DB.Line.CreateBound(ep0, ep1);
            trans.Commit();
            double length = cmpGeometry.ToMetricFromFeet(line1.Length, -3);
            //double length = cmpGeometry.ToHalfAdjust(line1.Length, -9);

            Revit.DB.XYZ _ep0 = linePnts[i];
            Revit.DB.XYZ _ep1 = linePnts[i + 1];
            //Revit.DB.XYZ _ep0 = linePnts[i * 2];
            //Revit.DB.XYZ _ep1 = linePnts[i * 2 + 1];

            //Revit.DB.Line line2 = Revit.DB.Line.CreateBound(_ep0, _ep1);
            //line2 = cmpGeometry.GetLineInLinesArea(line2, crvCrop, inAreaPoint);
            //if (line2 == null)
            //{
            //  continue;
            //}

            double distance = cmpGeometry.ToMetricFromFeet(cmpGeometry.Distance(_ep0, _ep1), -3);
                        //double distance = cmpGeometry.ToHalfAdjust(cmpGeometry.Distance(_ep0, _ep1), -9);

            // 作成
            trans.Start("Create dimensions");
            Revit.DB.Dimension dim = cmpElements.CreateDimension(vp, line1, refAry);

            if (dim != null)
            {
              if (dimType != null)
              {
                try
                {
                  dim.ChangeTypeId(dimType.Id);
                }
                catch
                {
                  dimType = rvtDbDoc.GetElement(dim.GetTypeId()) as Revit.DB.DimensionType;
                }
              }

              // 平面寸法 != 端点距離
              if (length != distance)
              {
                // メートル単位
                double metric = cmpGeometry.ToHalfAdjust(distance, -1);
                //double metric = cmpGeometry.ToHalfAdjust(distance * 304.8 / 1000, -1);

                // 最小値
                if (metric < 0.1)
                {
                  metric = 0.1;
                }
                string strMetric = metric.ToString();

                // 小数点以下1位
                if (strMetric.Contains("."))
                {
                  strMetric = strMetric.Substring(0, strMetric.LastIndexOf(".") + 2);
                }
                else
                {
                  strMetric += ".0";
                }

                // 寸法上書き
                dim.ValueOverride = "∠" + strMetric;
              }
              // 0.1以下
              else
              {
                double? dimVal = dim.Value;

                if (dimVal == null)
                {
                  // 寸法上書き
                  dim.ValueOverride = "0.1";
                }
                else
                {
                  dimVal = cmpGeometry.ToMetricFromFeet((double)dimVal, -1);

                  if (dimVal < 0.1)
                  {
                    // 寸法上書き
                    dim.ValueOverride = "0.1";
                  }
                }
              }
              trans.Commit();
            }
            else
            {
                  trans.RollBack();
            }
          }
        }

        #endregion

        // 縦ダクト
        #region 縦ダクト

        else // if (cmpGeometry.IsVerticalSingleDuct(duct))
        {
          // 連続するダクト
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> cnctVerticalDucts = cmpService.ConnectVerticalDuct(duct,
                                                                                                                 inViewDucts,
                                                                                                                 ref farPnt1,
                                                                                                                 ref farPnt2,
                                                                                                                 ref d1,
                                                                                                                 ref d2,
                                                                                                                 inViewDuctId,
                                                                                                                 false);

          // 使用済み
          foreach (Revit.DB.Mechanical.Duct d in cnctVerticalDucts)
          {
            usedIds.Add(d.Id.ToString());
          }

          // 最大高さ、最小高さ
          double maxZ = 0;
          double minZ = 0;

          cmpGeometry.GetVerticalDuctTopBbtm(cnctVerticalDucts, ref maxZ, ref minZ, farPnt1, farPnt2);


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

          Revit.DB.Line line = cmpGeometry.GetDuctLine(duct);

          // 範囲内判定
          line = cmpGeometry.GetLineInLinesArea(line, crvCrop, inAreaPoint);
          if (line == null)
          {
            continue;
          }

          Revit.DB.XYZ ep0 = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, minZ);
          Revit.DB.XYZ ep1 = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, maxZ);

          if (cmpGeometry.Distance(ep0, ep1) <= cmpParameters.LineMinLength)
          {
            continue;
          }

          trans.Start("Create detail lines");
          Revit.DB.Line l = Revit.DB.Line.CreateBound(ep0, ep1);
          trans.Commit();


          // スペースとの交差端点
          Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> crossSpaceAndPoint = cmpService.GetCrossingSpaceAndPoint(l);

          foreach (Revit.DB.ElementId elemId in crossSpaceAndPoint.Keys)
          {
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> dicPnts = crossSpaceAndPoint[elemId];

            foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in dicPnts)
            {
              if (cmpParameters.LineMinLength < cmpGeometry.Distance(pnts[0], pnts[1]))
              {
                double length = System.Math.Abs(pnts[0].Z - pnts[1].Z);

                // ミリメートル単位
                double milliMetric = length * 304.8;

                // 最小値
                if (milliMetric < 1)
                {
                  milliMetric = 1;
                }

                string strMilliMetric = Math.Round(milliMetric, 0, MidpointRounding.AwayFromZero).ToString();// milliMetric.ToString();

                // 長さ文字
                string strVal = "\u00D8" + strMilliMetric;

                Revit.DB.XYZ origin = line.GetEndPoint(0);
                Revit.DB.XYZ baseVec = vp.RightDirection;
                Revit.DB.HorizontalTextAlignment horizontal = Revit.DB.HorizontalTextAlignment.Center;
                Revit.DB.VerticalTextAlignment vertical = Revit.DB.VerticalTextAlignment.Middle;

                // 文字作成
                Revit.DB.TextNote textNote = cmpService.CreateTextNoteXYPosRotateSet(trans,
                                                                                     vp,
                                                                                     origin,
                                                                                     baseVec,
                                                                                     horizontal,
                                                                                     vertical,
                                                                                     strVal);

                if (textType != null)
                {
                  trans.Start("Change Type");
                  textNote.ChangeTypeId(textType.Id);
                  trans.Commit();
                }
              }
            }
          }
        }

        #endregion
      }

      #endregion

      trans.Start("Save settings");
      cmpService.Set(dimType, textType);
      trans.Commit();

      // 終了メッセージ

      transGroup.Assimilate();
      retCmd = Revit.UI.Result.Succeeded;
      return retCmd;
    }
    #endregion
  }
}
