using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using SectionListRC.Utils;

namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>図形</summary>
  /// ================================================================================
  public class Geometry : SectionListRC.JExtComCompat.RvtGeometry
  {
    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
    /// 
    /// <history>2012/11/13 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Geometry(Revit.UI.UIDocument rvtUIDoc)
      : base(rvtUIDoc)
    {
    }
    #endregion


    //メンバ関数
    #region Member Functions
    /// ================================================================================
    /// <summary>正円</summary>
    /// <history>2013/04/24 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Curve CrvCircle(double diameter,
                             Revit.DB.XYZ center)
    {
      if (diameter == 0)
      {
        return null;
      }

      double radius = diameter / 2;
      double startAngle = 0;
      double endAngle = 2 * System.Math.PI;
      Revit.DB.XYZ xAxis = new Revit.DB.XYZ(1, 0, 0);
      Revit.DB.XYZ yAxis = new Revit.DB.XYZ(0, 1, 0);

      return Revit.DB.Arc.Create(center, radius, startAngle, endAngle, xAxis, yAxis);
    }

    /// ================================================================================
    /// <summary>半円</summary>
    /// <history>2013/05/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Curve CrvHalfCircle(double diameter,
                                 Revit.DB.XYZ center,
                                 int mode)
    {
      Revit.DB.Arc ret = null;

      double radius = diameter / 2;
      double startAngle = 0;
      double endAngle = System.Math.PI;

      // 上
      if (mode == 0)
      {
      }
      // 下
      if (mode == 1)
      {
        startAngle = System.Math.PI;
        endAngle = System.Math.PI * 2;
      }
      // 右
      if (mode == 2)
      {
        startAngle = System.Math.PI * 3 / 2;
        endAngle = System.Math.PI * 5 / 2;
      }
      // 左
      if (mode == 3)
      {
        startAngle = System.Math.PI / 2;
        endAngle = System.Math.PI * 3 / 2;
      }

      Revit.DB.XYZ xAxis = new Revit.DB.XYZ(1, 0, 0);
      Revit.DB.XYZ yAxis = new Revit.DB.XYZ(0, 1, 0);

      try
      {
        ret = Revit.DB.Arc.Create(center, radius, startAngle, endAngle, xAxis, yAxis);
      }
      catch (Revit.Exceptions.ArgumentsInconsistentException)
      {
        
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>同心円上の点</summary>
    /// <history>2013/05/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> PointOnCircle(double diameter,
                                                          Revit.DB.XYZ center,
                                                          int pointNum)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 半径
      double radius = diameter / 2;

      double round = System.Math.PI * 2;
      double oneRad = round / pointNum;

      // 現在の角度
      double currentRad = 0;

      for (int i = 0; i < pointNum; ++i)
      {
        currentRad = oneRad * i;

        double x = System.Math.Cos(currentRad) * radius;
        double y = System.Math.Sin(currentRad) * radius;

        Revit.DB.XYZ newPoint = center + new Revit.DB.XYZ(x, y, center.Z);

        ret.Add(newPoint);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>長方形</summary>
    /// <history>2013/03/26 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public 
    Collections.Generic.IList<Revit.DB.Curve> CrvRectangle(double width,
                                                           double height,
                                                           Revit.DB.XYZ center)
    {
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      if (width <= 0d || height <= 0d)
      {
        return ret;
      }

      Revit.DB.XYZ p0 = new Revit.DB.XYZ(center.X - width / 2, center.Y + height / 2, center.Z);
      Revit.DB.XYZ p1 = new Revit.DB.XYZ(center.X + width / 2, center.Y + height / 2, center.Z);
      Revit.DB.XYZ p2 = new Revit.DB.XYZ(center.X + width / 2, center.Y - height / 2, center.Z);
      Revit.DB.XYZ p3 = new Revit.DB.XYZ(center.X - width / 2, center.Y - height / 2, center.Z);

      ret.Add(Revit.DB.Line.CreateBound(p0, p1));
      ret.Add(Revit.DB.Line.CreateBound(p1, p2));
      ret.Add(Revit.DB.Line.CreateBound(p2, p3));
      ret.Add(Revit.DB.Line.CreateBound(p3, p0));

      return ret;
    }

    /// ================================================================================
    /// <summary>長方形の頂点</summary>
    /// 
    /// <param name="width" >幅</param>
    /// <param name="height">高さ</param>
    /// <param name="center">中心</param>
    /// 
    /// <returns><p>[0] = 左上</p>
    ///           <p>[1] = 右上</p>
    ///           <p>[2] = 右下</p>
    ///           <p>[3] = 左下</p></returns>
    /// 
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> RectanglePoints(double width,
                                                            double height,
                                                            Revit.DB.XYZ center)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 左上
      Revit.DB.XYZ p0 = new Revit.DB.XYZ(center.X - width / 2, center.Y + height / 2, center.Z);
      // 右上
      Revit.DB.XYZ p1 = new Revit.DB.XYZ(center.X + width / 2, center.Y + height / 2, center.Z);
      // 右下
      Revit.DB.XYZ p2 = new Revit.DB.XYZ(center.X + width / 2, center.Y - height / 2, center.Z);
      // 左下
      Revit.DB.XYZ p3 = new Revit.DB.XYZ(center.X - width / 2, center.Y - height / 2, center.Z);

      ret.Add(p0);
      ret.Add(p1);
      ret.Add(p2);
      ret.Add(p3);

      return ret;
    }

    /// ================================================================================
    /// <summary>かぶり厚分内の長方形の頂点</summary>
    /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void RectanglePointsInsideKaburi(Revit.DB.XYZ center,
                                            double x,
                                            double y,
                                            double kaburi,
                                            ref Revit.DB.XYZ leftTop,
                                            ref Revit.DB.XYZ leftBottom,
                                            ref Revit.DB.XYZ rightTop,
                                            ref Revit.DB.XYZ rightBottom)
    {
      double halfX = x / 2;
      double halfY = y / 2;

      leftTop     = center + new Revit.DB.XYZ(-(halfX - kaburi), halfY - kaburi, 0);
      leftBottom  = center + new Revit.DB.XYZ(-(halfX - kaburi), -(halfY - kaburi), 0);
      rightTop    = center + new Revit.DB.XYZ(halfX - kaburi, halfY - kaburi, 0);
      rightBottom = center + new Revit.DB.XYZ(halfX - kaburi, -(halfY - kaburi), 0);
    }

    /// ================================================================================
    /// <summary>長方形の下、左に寸法線線作成</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void CreateDimensionsRectangleBottomLeft(Collections.Generic.IList<Revit.DB.DetailLine> dLines,
                                                    Revit.DB.DimensionType dimType,
                                                    Revit.DB.View view)
    {
      if (dLines.Count != 4)
      {
        return;
      }

      // 済み
      Collections.Generic.IList<Revit.DB.Element> elems = new Collections.Generic.List<Revit.DB.Element>();

      for (int i = 0; i < dLines.Count; ++i)
      {
        Revit.DB.Element elem = dLines[i];
        elems.Add(elem);

        foreach (Revit.DB.Element e in dLines)
        {
          if (e.Id.Value != elem.Id.Value && elems.Contains(e) == false)
          {
            bool sameDirection = sameDirectionLines(elem, e);

            if (sameDirection == true)
            {

              Revit.DB.Reference ref1 = new Revit.DB.Reference(elem);
              Revit.DB.Reference ref2 = new Revit.DB.Reference(e);

              Revit.DB.Line dimLine = null;

              Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();

              if (elem.Id.Value < e.Id.Value)
              {
                //ref1 = new Revit.DB.Reference(elem);
                //ref2 = new Revit.DB.Reference(e);

                dimLine = OrthogonalLineOnPoint(base.Center2Point(BottomLeftEndPnt(elem, dimType), BottomLeftEndPnt(e, dimType)), elem, e);

                refAry.Append(ref1);
                refAry.Append(ref2);
              }
              else
              {
                //ref1 = new Revit.DB.Reference(e);
                //ref2 = new Revit.DB.Reference(elem);

                dimLine = OrthogonalLineOnPoint(base.Center2Point(BottomLeftEndPnt(e, dimType), BottomLeftEndPnt(elem, dimType)), elem, e);

                refAry.Append(ref2);
                refAry.Append(ref1);
              }

              Revit.DB.Dimension newDim = RvtDBDoc.Create.NewDimension(view, dimLine, refAry, dimType);
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>長方形の下に寸法線線作成</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void CreateDimensionRectBottom(Collections.Generic.IList<Revit.DB.DetailLine> dLines,
                                   Revit.DB.DimensionType dimType)
    {
      if (dLines.Count != 4)
      {
        return;
      }

      // 済み
      Collections.Generic.IList<Revit.DB.Element> elems = new Collections.Generic.List<Revit.DB.Element>();

      for (int i = 0; i < dLines.Count; ++i)
      {
        Revit.DB.Element elem = dLines[i];
        elems.Add(elem);

        Revit.DB.XYZ bottomPoint = null;

        bool haveBottom = BottomEndPoint(elem, dimType, ref bottomPoint);
        if (haveBottom == false)
        {
          continue;
        }

        foreach (Revit.DB.Element e in dLines)
        {
          if (e.Id.Value != elem.Id.Value && elems.Contains(e) == false)
          {
            bool sameDirection = sameDirectionLines(elem, e);

            if (sameDirection == true)
            {

              Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();

              Revit.DB.Reference ref1 = new Revit.DB.Reference(elem);
              Revit.DB.Reference ref2 = new Revit.DB.Reference(e);

              Revit.DB.Line dimLine = null;

              Revit.DB.XYZ bottomPoint2 = null;
              bool haveBottom2 = BottomEndPoint(e, dimType, ref bottomPoint2);

              if (haveBottom2 == false)
              {
                continue;
              }

              refAry.Append(ref1);
              refAry.Append(ref2);

              dimLine = OrthogonalLineOnPoint(base.Center2Point(bottomPoint, bottomPoint2), elem, e);  

              Revit.DB.Dimension newDim = RvtDBDoc.Create.NewDimension(RvtDBDoc.ActiveView, dimLine, refAry, dimType);
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>長方形の左に寸法線線作成</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void CreateDimensionRectLeft(Collections.Generic.IList<Revit.DB.DetailLine> dLines,
                                   Revit.DB.DimensionType dimType)
    {
      if (dLines.Count != 4)
      {
        return;
      }

      // 済み
      Collections.Generic.IList<Revit.DB.Element> elems = new Collections.Generic.List<Revit.DB.Element>();

      for (int i = 0; i < dLines.Count; ++i)
      {
        Revit.DB.Element elem = dLines[i];
        elems.Add(elem);

        Revit.DB.XYZ leftPoint = null;

        bool haveLeft = LeftEndPoint(elem, dimType, ref leftPoint);
        if (haveLeft == false)
        {
          continue;
        }

        foreach (Revit.DB.Element e in dLines)
        {
          if (e.Id.Value != elem.Id.Value && elems.Contains(e) == false)
          {
            bool sameDirection = sameDirectionLines(elem, e);

            if (sameDirection == true)
            {

              Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();

              Revit.DB.Reference ref1 = new Revit.DB.Reference(elem);
              Revit.DB.Reference ref2 = new Revit.DB.Reference(e);

              Revit.DB.Line dimLine = null;

              Revit.DB.XYZ leftPoint2 = null;
              bool haveLeft2 = LeftEndPoint(e, dimType, ref leftPoint2);

              if (haveLeft2 == false)
              {
                continue;
              }

              refAry.Append(ref1);
              refAry.Append(ref2);

              dimLine = OrthogonalLineOnPoint(base.Center2Point(leftPoint, leftPoint2), elem, e);

              Revit.DB.Dimension newDim = RvtDBDoc.Create.NewDimension(RvtDBDoc.ActiveView, dimLine, refAry, dimType);
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>円の下に寸法線線作成</summary>
    /// <history>2013/10/01 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void CreateDimensionCircleBottom(Collections.Generic.IList<Revit.DB.DetailCurve> dCrvs,
                                     Revit.DB.DimensionType dimType,
                                     Revit.DB.View actView)
    {
      foreach (Revit.DB.DetailCurve dc in dCrvs)
      {
        Revit.DB.Curve crv = dc.GeometryCurve;

        Revit.DB.Arc arc = crv as Revit.DB.Arc;

        double radius = arc.Radius;

        Revit.DB.XYZ center = arc.Center;

        Revit.DB.XYZ p0 = center + new Revit.DB.XYZ(radius , 0, 0);
        Revit.DB.XYZ p1 = center + new Revit.DB.XYZ(-radius, 0, 0);

        // 1mm
        double length = 1 / 304.8;
        double textSize = dimType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble() * RvtDBDoc.ActiveView.Scale;

        Revit.DB.XYZ p2 = p0 + new Revit.DB.XYZ(0, -length, 0);
        Revit.DB.XYZ p3 = p1 + new Revit.DB.XYZ(0, -length, 0);

        Revit.DB.Line line0 = Revit.DB.Line.CreateBound(p0, p2);
        Revit.DB.Line line1 = Revit.DB.Line.CreateBound(p1, p3);

        Revit.DB.Line horizLine = Revit.DB.Line.CreateBound(p0 + new Revit.DB.XYZ(0, -(radius + 2 * textSize), 0), p1 + new Revit.DB.XYZ(0, -(radius + 2 * textSize), 0));

        Revit.DB.DetailCurve dc0 = RvtDBDoc.Create.NewDetailCurve(actView, line0);
        Revit.DB.DetailCurve dc1 = RvtDBDoc.Create.NewDetailCurve(actView, line1);

        Revit.DB.Reference ref0 = new Revit.DB.Reference(dc0);
        Revit.DB.Reference ref1 = new Revit.DB.Reference(dc1);

        Revit.DB.ReferenceArray refAry = new Revit.DB.ReferenceArray();
        refAry.Append(ref0);
        refAry.Append(ref1);

        try
        {
          RvtDBDoc.Create.NewDimension(actView, horizLine, refAry, dimType);
        }
        catch
        {
          continue;
        }
      }
    }


    /// ================================================================================
    /// <summary>線分要素からラジアン角取得(始点->終点)</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    double GetRadian(Revit.DB.Element elem)
    {
      Revit.DB.Curve crv = null;

      if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Grids))
      {
        Revit.DB.Grid grid = elem as Revit.DB.Grid;
        crv = grid.Curve;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_CLines))
      {
        Revit.DB.ReferencePlane refPlane = elem as Revit.DB.ReferencePlane;
        Revit.DB.XYZ p1 = refPlane.BubbleEnd;
        Revit.DB.XYZ p2 = refPlane.FreeEnd;
        Revit.DB.Line line = Revit.DB.Line.CreateBound(p1, p2);
        crv = line;
      }
      else
      {
        Revit.DB.CurveElement crvElem = elem as Revit.DB.CurveElement;
        crv = crvElem.GeometryCurve;
      }

      // 始点、終点
      Revit.DB.XYZ startPoint = crv.GetEndPoint(0);
      Revit.DB.XYZ endPoint = crv.GetEndPoint(1);

      // 戻り値
      double rad = System.Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);

      return rad;
    }

    /// ================================================================================
    /// <summary>同じ方向の線分要素判定</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool sameDirectionLines(Revit.DB.Element elem,
                            Revit.DB.Element elem2)
    {
      bool ret = false;

      double rad = GetRadian(elem);

      double r = GetRadian(elem2);

      // +180°
      double plusPI = ToHalfAdjust(r + System.Math.PI, -9);
      // -180°
      double minusPI = ToHalfAdjust(r - System.Math.PI, -9);

      rad = ToHalfAdjust(rad, -9);
      r = ToHalfAdjust(r, -9);

      if (rad == r || rad == plusPI || rad == minusPI)
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>寸法用線分要素の左端または下端</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ BottomLeftEndPnt(Revit.DB.Element elem,
                                  Revit.DB.DimensionType dimType)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.Curve crv = null;

      if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Grids))
      {
        Revit.DB.Grid grid = elem as Revit.DB.Grid;
        crv = grid.Curve;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_CLines))
      {
        Revit.DB.ReferencePlane refPlane = elem as Revit.DB.ReferencePlane;
        Revit.DB.XYZ p1 = refPlane.BubbleEnd;
        Revit.DB.XYZ p2 = refPlane.FreeEnd;
        Revit.DB.Line line = Revit.DB.Line.CreateBound(p1, p2);
        crv = line;
      }
      else
      {
        Revit.DB.CurveElement crvElem = elem as Revit.DB.CurveElement;
        crv = crvElem.GeometryCurve;
      }

      // 寸法線の足の長さ外へ
      double offSet = dimType.get_Parameter(Revit.DB.BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST).AsDouble();
      int scale = RvtDBDoc.ActiveView.Scale;
      Revit.DB.XYZ p = new Revit.DB.XYZ(offSet * scale, offSet* scale, 0);

      // 始点、終点
      Revit.DB.XYZ startPoint = crv.GetEndPoint(0);
      Revit.DB.XYZ endPoint = crv.GetEndPoint(1);

      if (ToHalfAdjust(startPoint.X, -9) < ToHalfAdjust(endPoint.X, -9))
      {
        ret = startPoint - p;
      }
      else if (ToHalfAdjust(startPoint.X, -9) > ToHalfAdjust(endPoint.X, -9))
      {
        ret = endPoint - p;
      }

      else if (ToHalfAdjust(startPoint.X, -9) == ToHalfAdjust(endPoint.X, -9))
      {
        if (ToHalfAdjust(startPoint.Y, -9) < ToHalfAdjust(endPoint.Y, -9))
        {
          ret = startPoint - p;
        }
        else
        {
          ret = endPoint - p;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>寸法用線分要素の左端</summary>
    /// <history>2013/05/01 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool LeftEndPoint(Revit.DB.Element elem,
                      Revit.DB.DimensionType dimType,
                      ref Revit.DB.XYZ leftPoint)
    {
      bool ret = false;

      Revit.DB.Curve crv = null;

      if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Grids))
      {
        Revit.DB.Grid grid = elem as Revit.DB.Grid;
        crv = grid.Curve;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_CLines))
      {
        Revit.DB.ReferencePlane refPlane = elem as Revit.DB.ReferencePlane;
        Revit.DB.XYZ p1 = refPlane.BubbleEnd;
        Revit.DB.XYZ p2 = refPlane.FreeEnd;
        Revit.DB.Line line = Revit.DB.Line.CreateBound(p1, p2);
        crv = line;
      }
      else
      {
        Revit.DB.CurveElement crvElem = elem as Revit.DB.CurveElement;
        crv = crvElem.GeometryCurve;
      }

      // 寸法線の足の長さ
      double offSet = dimType.get_Parameter(Revit.DB.BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST).AsDouble();
      int scale = RvtDBDoc.ActiveView.Scale;
      Revit.DB.XYZ p = new Revit.DB.XYZ(offSet * scale, 0, 0);

      // 始点、終点
      Revit.DB.XYZ startPoint = crv.GetEndPoint(0);
      Revit.DB.XYZ endPoint = crv.GetEndPoint(1);

      if (ToHalfAdjust(startPoint.X, -9) < ToHalfAdjust(endPoint.X, -9))
      {
        leftPoint = startPoint - p;
        ret = true;
      }
      else if (ToHalfAdjust(startPoint.X, -9) > ToHalfAdjust(endPoint.X, -9))
      {
        leftPoint = endPoint - p;
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>寸法用線分要素の下端</summary>
    /// <history>2013/05/01 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool BottomEndPoint(Revit.DB.Element elem,
                      Revit.DB.DimensionType dimType,
                      ref Revit.DB.XYZ bottomPoint)
    {
      bool ret = false;

      Revit.DB.Curve crv = null;

      if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Grids))
      {
        Revit.DB.Grid grid = elem as Revit.DB.Grid;
        crv = grid.Curve;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_CLines))
      {
        Revit.DB.ReferencePlane refPlane = elem as Revit.DB.ReferencePlane;
        Revit.DB.XYZ p1 = refPlane.BubbleEnd;
        Revit.DB.XYZ p2 = refPlane.FreeEnd;
        Revit.DB.Line line = Revit.DB.Line.CreateBound(p1, p2);
        crv = line;
      }
      else
      {
        Revit.DB.CurveElement crvElem = elem as Revit.DB.CurveElement;
        crv = crvElem.GeometryCurve;
      }

      // 寸法線の足の長さ
      double offSet = dimType.get_Parameter(Revit.DB.BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST).AsDouble();
      int scale = RvtDBDoc.ActiveView.Scale;
      Revit.DB.XYZ p = new Revit.DB.XYZ(0, offSet * scale, 0);

      // 始点、終点
      Revit.DB.XYZ startPoint = crv.GetEndPoint(0);
      Revit.DB.XYZ endPoint = crv.GetEndPoint(1);

      if (ToHalfAdjust(startPoint.Y, -9) < ToHalfAdjust(endPoint.Y, -9))
      {
        bottomPoint = startPoint - p;
        ret = true;
      }
      else if (ToHalfAdjust(startPoint.Y, -9) > ToHalfAdjust(endPoint.Y, -9))
      {
        bottomPoint = endPoint - p;
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ライン取得</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line LineElem(Revit.DB.Element elem)
    {
      Revit.DB.Line line = null;

      if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Walls))
      {
        Revit.DB.Wall wall = elem as Revit.DB.Wall;
        Revit.DB.LocationCurve locCrv = wall.Location as Revit.DB.LocationCurve;
        line = locCrv.Curve as Revit.DB.Line;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Grids))
      {
        Revit.DB.Grid grid = elem as Revit.DB.Grid;
        line = grid.Curve as Revit.DB.Line;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_CLines))
      {
        Revit.DB.ReferencePlane refPlane = elem as Revit.DB.ReferencePlane;
        Revit.DB.XYZ p1 = refPlane.BubbleEnd;
        Revit.DB.XYZ p2 = refPlane.FreeEnd;
        line = Revit.DB.Line.CreateBound(p1, p2);
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Walls))
      {
        Revit.DB.Wall wall = elem as Revit.DB.Wall;
        Revit.DB.LocationCurve locCrv = wall.Location as Revit.DB.LocationCurve;
        Revit.DB.Curve crv = locCrv.Curve;

        line = crv as Revit.DB.Line;
      }
      else if (elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_Lines) ||
               elem.Category.Id.Value.Equals((long)Revit.DB.BuiltInCategory.OST_AreaSchemeLines))
      {
        Revit.DB.CurveElement crvElem = elem as Revit.DB.CurveElement;
        if (crvElem != null)
        {
          line = crvElem.GeometryCurve as Revit.DB.Line;
        }
      }

      return line;
    }

    /// ================================================================================
    /// <summary>任意の点を通り2線分と直交する線分</summary>
    /// <history>2013/04/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line OrthogonalLineOnPoint(Revit.DB.XYZ anyPoint,
                                        Revit.DB.Element elem1,
                                        Revit.DB.Element elem2)
    {
      // 戻り値
      Revit.DB.Line line = null;

      double rad = GetRadian(elem1);

      // 任意点を通り、線分に直交する仮想の線分の通り芯との交点を求める
      double kakudo = rad + System.Math.PI / 2;
      // X成分Y成分
      double x = System.Math.Cos(kakudo);
      double y = System.Math.Sin(kakudo);
      double a = 10;
      // 仮の端点
      Revit.DB.XYZ virtualPnt = new Revit.DB.XYZ(anyPoint.X + a * x, anyPoint.Y + a * y, anyPoint.Z);

      double f = virtualPnt.X - anyPoint.X;
      double g = virtualPnt.Y - anyPoint.Y;
      // 先の線分との交点
      double f1 = LineElem(elem1).GetEndPoint(1).X - LineElem(elem1).GetEndPoint(0).X;
      double g1 = LineElem(elem1).GetEndPoint(1).Y - LineElem(elem1).GetEndPoint(0).Y;
      double det1 = f1 * g - f * g1;
      double dx1 = LineElem(elem1).GetEndPoint(0).X - anyPoint.X;
      double dy1 = LineElem(elem1).GetEndPoint(0).Y - anyPoint.Y;
      double t1 = (f1 * dy1 - g1 * dx1) / det1;
      Revit.DB.XYZ firstPoint = new Revit.DB.XYZ(anyPoint.X + f * t1,
                                                 anyPoint.Y + g * t1,
                                                 LineElem(elem1).GetEndPoint(0).Z);
      // 後の線分との交点
      double f2 = LineElem(elem2).GetEndPoint(1).X - LineElem(elem2).GetEndPoint(0).X;
      double g2 = LineElem(elem2).GetEndPoint(1).Y - LineElem(elem2).GetEndPoint(0).Y;
      double det2 = f2 * g - f * g2;
      double dx2 = LineElem(elem2).GetEndPoint(0).X - anyPoint.X;
      double dy2 = LineElem(elem2).GetEndPoint(0).Y - anyPoint.Y;
      double t2 = (f2 * dy2 - g2 * dx2) / det2;
      Revit.DB.XYZ secondPoint = new Revit.DB.XYZ(anyPoint.X + f * t2,
                                                  anyPoint.Y + g * t2,
                                                  LineElem(elem1).GetEndPoint(0).Z);

      line = Revit.DB.Line.CreateBound(firstPoint, secondPoint);
      return line;
    }

    /// ================================================================================
    /// <summary>指定の点に近い順</summary>
    /// 
    /// <history>2013/06/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> NearPointsOrder(Revit.DB.XYZ pnt,
                                                            Collections.Generic.IList<Revit.DB.XYZ> pnts,
                                                            bool vertical)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> buffer = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.XYZ p in pnts)
      {
        buffer.Add(p);
      }

      while (buffer.Count > 0)
      {
        Revit.DB.XYZ near = null;
        double distance = 0;

        for (int i = 0; i < buffer.Count; ++i)
        {
          if (i == 0)
          {
            near = buffer[0];
            //distance = base.Distance2D(pnt, near);

            if (vertical == true)
            {
              distance = System.Math.Abs(pnt.Y - near.Y);
            }
            else
            {
              distance = System.Math.Abs(pnt.X - near.X);
            }

            continue;
          }

          Revit.DB.XYZ p = buffer[i];
          double dis = 0; //base.Distance2D(pnt, p);
          if (vertical == true)
          {
            dis = System.Math.Abs(pnt.Y - p.Y);
          }
          else
          {
            dis = System.Math.Abs(pnt.X - p.X);
          }

          distance = ToHalfAdjust(distance, -9);
          dis = ToHalfAdjust(dis, -9);

          if (distance > dis)
          {
            near = p;
            distance = dis;
          }
          else if (distance == dis)
          {
            if (vertical == true)
            {
              if (pnt.Y > p.Y)
              {
                near = p;
                distance = dis;
              }
            }
            else
            {
              if (pnt.X > p.X)
              {
                near = p;
                distance = dis;
              }
            }
          }
        }

        ret.Add(near);
        buffer.Remove(near);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2段筋位置の調整</summary>
    /// 
    /// <param name="vertical">true = 縦方向(X2段筋)</param>
    /// 
    /// <history>2013/05/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void Move2ndRebar(ref Revit.DB.XYZ point,
                      Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                      bool vertical)
    {
      bool onaji = false;

      // 縦方向(X2段筋)の調整
      if (vertical == true)
      {
        int smallCount = 0;
        int bigCount = 0;

        // 差が小さい点があるか
        foreach (Revit.DB.XYZ p in defaultPoints)
        {
          if (ToHalfAdjust(point.Y, -9) - ToHalfAdjust(p.Y, -9) == 0)
          {
            onaji = true;
            break;
          }
          else if (ToHalfAdjust(point.Y, -9) > ToHalfAdjust(p.Y, -9))
          {
            smallCount += 1;
          }
          else if (ToHalfAdjust(point.Y, -9) < ToHalfAdjust(p.Y, -9))
          {
            bigCount += 1;
          }
        }

        if (onaji == true)
        {
          return;
        }

        // 自分より小さい点が1つだけならその点
        if (smallCount == 1)
        {
          foreach (Revit.DB.XYZ p in defaultPoints)
          {
            if (ToHalfAdjust(point.X, -9) > ToHalfAdjust(p.X, -9))
            {
              point = new Revit.DB.XYZ(p.X, point.Y, point.Z);
              return;
            }
          }
        }
        // 自分より大きい点が1つだけならその点
        if (bigCount == 1)
        {
          foreach (Revit.DB.XYZ p in defaultPoints)
          {
            if (ToHalfAdjust(point.X, -9) < ToHalfAdjust(p.X, -9))
            {
              point = new Revit.DB.XYZ(p.X, point.Y, point.Z);
              return;
            }
          }
        }


        Revit.DB.XYZ nearPoint = defaultPoints[0];
        double distance = base.Distance2D(point, nearPoint);

        // 自分より小さく、差が最小な点を求める
        for (int i = 0; i < defaultPoints.Count; ++i)
        {
          Revit.DB.XYZ p = defaultPoints[i];

          if (p.Y < point.Y)
          {
            double dis = base.Distance2D(point, p);

            if (ToHalfAdjust(dis, -9) < ToHalfAdjust(distance, -9))
            {
              distance = dis;

              nearPoint = p;
            }
          }
        }

        if (nearPoint != null)
        {
          point = new Revit.DB.XYZ(point.X, nearPoint.Y, point.Z);
        }
      }
      // 横方向(Y2段筋)の調整
      else
      {
        int smallCount = 0;
        int bigCount = 0;

        // 差が小さい点(ほぼ同じ点)があるか
        foreach (Revit.DB.XYZ p in defaultPoints)
        {
          if (ToHalfAdjust(point.X, -9) - ToHalfAdjust(p.X, -9) == 0)
          {
            onaji = true;
            break;
          }
          else if (ToHalfAdjust(point.X, -9) > ToHalfAdjust(p.X, -9))
          {
            smallCount += 1;
          }
          else if (ToHalfAdjust(point.X, -9) < ToHalfAdjust(p.X, -9))
          {
            bigCount += 1;
          }
        }
        if (onaji == true)
        {
          return;
        }

        // 自分より小さい点が1つだけならその点
        if (smallCount == 1)
        {
          foreach (Revit.DB.XYZ p in defaultPoints)
          {
            if (ToHalfAdjust(point.X, -9) > ToHalfAdjust(p.X, -9))
            {
              point = new Revit.DB.XYZ(p.X, point.Y, point.Z);
              return;
            }
          }
        }
        // 自分より大きい点が1つだけならその点
        if (bigCount == 1)
        {
          foreach (Revit.DB.XYZ p in defaultPoints)
          {
            if (ToHalfAdjust(point.X, -9) < ToHalfAdjust(p.X, -9))
            {
              point = new Revit.DB.XYZ(p.X, point.Y, point.Z);
              return;
            }
          }
        }


        Revit.DB.XYZ nearPoint = defaultPoints[0];
        double distance = base.Distance2D(point, nearPoint);

        // 自分より小さく、差が最小な点を求める
        for (int i = 0; i < defaultPoints.Count; ++i)
        {
          Revit.DB.XYZ p = defaultPoints[i];

          if (p.X < point.X)
          {
            if (i == 0)
            {
              distance = base.Distance2D(point, p);
            }

            double dis = base.Distance2D(point, p);

            if (ToHalfAdjust(dis, -9) < ToHalfAdjust(distance, -9))
            {
              distance = dis;

              nearPoint = p;
            }
          }
        }

        if (nearPoint != null)
        {
          point = new Revit.DB.XYZ(nearPoint.X, point.Y, point.Z);
        }
      }
    }

    /// ================================================================================
    /// <summary>2段筋太径位置の調整</summary>
    /// 
    /// <param name="defaultPoints">鉄筋の左から右の並び</param>
    /// 
    /// <history>2013/06/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SetNextRowHutoRebar(int rebarNum,
                                                                Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                                                                Revit.DB.XYZ move)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      if (defaultPoints.Count < 1)
      {
        return ret;
      }
      else if (defaultPoints.Count < 2)
      {
        ret.Add(defaultPoints[0] + move);

        return ret;
      }
      else if (defaultPoints.Count == rebarNum)
      {
        for (int i = 0; i < defaultPoints.Count; ++i)
        {
          ret.Add(defaultPoints[i] + move);
        }
      }
      else
      {
        // 端
        ret.Add(defaultPoints[0] + move);
        if (ret.Count == rebarNum)
        {
          return ret;
        }

        ret.Add(defaultPoints[defaultPoints.Count - 1] + move);
        if (ret.Count == rebarNum)
        {
          return ret;
        }

        // 中央から左、右、左、右、、の順
        //Collections.Generic.IList<Revit.DB.XYZ> pntsFromCenter = new Collections.Generic.List<Revit.DB.XYZ>();

        //// 偶数本
        //if (defaultPoints.Count % 2 == 0)
        //{
        //  for (int i = 0; i < defaultPoints.Count / 2; ++i)
        //  {
        //    ret.Add(defaultPoints[defaultPoints.Count / 2 - 1 - i] + move);

        //    if (ret.Count == rebarNum)
        //    {
        //      break;
        //    }

        //    ret.Add(defaultPoints[defaultPoints.Count / 2 + i] + move);

        //    if (ret.Count == rebarNum)
        //    {
        //      break;
        //    }
        //  }
        //}
        //// 奇数本
        //else
        //{
        //  double d = defaultPoints.Count;
        //  double d2 = d / 2;
        //  int center = (int)System.Math.Floor(d2);

        //  ret.Add(defaultPoints[center] + move);

        //  if (ret.Count == rebarNum)
        //  {
        //    return ret;
        //  }

        //  for (int i = 0; i < defaultPoints.Count / 2; ++i)
        //  {
        //    ret.Add(defaultPoints[center - 1 - i] + move);
        //    if (ret.Count == rebarNum)
        //    {
        //      break;
        //    }

        //    ret.Add(defaultPoints[center + 1 + i] + move);

        //    if (ret.Count == rebarNum)
        //    {
        //      break;
        //    }
        //  }
        //}


        // 2014/01/09

        // 左端
        Revit.DB.XYZ left = defaultPoints[0];
        // 右端
        Revit.DB.XYZ right = defaultPoints[defaultPoints.Count - 1];

        double distance = Distance2D(left, right);

        double pitch = 0;
        if (rebarNum > 1)
        {
          pitch = distance / (rebarNum - 1);
        }

        for (int i = 0; i < rebarNum - 1; ++i)
        {
          if (i == 0 || i == rebarNum)
          {
            continue; ;
          }

          Revit.DB.XYZ p = new Revit.DB.XYZ(left.X + pitch * i, left.Y, left.Z);

          ret.Add(NearXPoint(p, defaultPoints) + move);
        }

      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2段筋細径位置の調整</summary>
    /// 
    /// <param name="defaultPoints">鉄筋の左から右の並び</param>
    /// 
    /// <history>2013/06/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SetNextRowHosoRebar(int rebarNum,
                                                                Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                                                                Collections.Generic.IList<Revit.DB.XYZ> used,
                                                                Revit.DB.XYZ center,
                                                                Revit.DB.XYZ move,
                                                                bool vertical)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      if (defaultPoints.Count < 2)
      {
        return ret;
      }

      // 太径を配置していない点
      Collections.Generic.IList<Revit.DB.XYZ> unUsed = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.XYZ p in defaultPoints)
      {
        bool isNear = false;

        Revit.DB.XYZ p2 = p + move;

        foreach (Revit.DB.XYZ pnt in used)
        {
          if (base.Distance2D(p2, pnt) < 1 / 304.8)
          {
            isNear = true;
          }
        }

        if (isNear == false)
        {
          unUsed.Add(p2);
        }
      }

      if (unUsed.Count == 0)
      {
        return ret;
      }

      // 細径は中央に近い順に
      Collections.Generic.IList<Revit.DB.XYZ> nearPntOrder = NearPointsOrder(center, unUsed, vertical);

      for (int i = 0; i < rebarNum; ++i)
      {
        ret.Add(nearPntOrder[i]);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2段筋位置の調整</summary>
    /// 
    /// <param name="defaultPoints">鉄筋の左から右の並び</param>
    /// 
    /// <history>2014/01/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void SetNextRowRebar(Collections.Generic.IList<int> rebarOrder,
                         Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                         ref Collections.Generic.IList<Revit.DB.XYZ> hutoPnts,
                         ref Collections.Generic.IList<Revit.DB.XYZ> hosoPnts,
                         Revit.DB.XYZ move)
    {
      // 左端
      Revit.DB.XYZ left = defaultPoints[0];
      // 右端
      Revit.DB.XYZ right = defaultPoints[defaultPoints.Count - 1];

      double distance = Distance2D(left, right);

      double pitch = 0;
      if (rebarOrder.Count > 1)
      {
        pitch = distance / (rebarOrder.Count - 1);
      }

      for (int i = 0; i < rebarOrder.Count; ++i)
      {
        int hutohoso = rebarOrder[i];

        Revit.DB.XYZ p = new Revit.DB.XYZ(left.X + pitch * i, left.Y, left.Z);
        p = NearXPoint(p, defaultPoints) + move;

        if (hutohoso == 0)
        {
          hutoPnts.Add(p);
        }
        else
        {
          hosoPnts.Add(p);
        }
      }
    }

    /// ================================================================================
    /// <summary>2段筋太径位置の調整</summary>
    /// 
    /// <param name="defaultPoints">鉄筋の左から右の並び</param>
    /// 
    /// <history>2013/06/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public 
    Collections.Generic.IList<Revit.DB.XYZ> Set2ndHutoRebar(int rebarNum,
                                                            Collections.Generic.IList<Revit.DB.XYZ> defaultPoints)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      if (defaultPoints.Count < 1)
      {
        return ret;
      }
      else if (defaultPoints.Count < 2)
      {
        ret.Add(defaultPoints[0]);

        return ret;
      }
      else
      {
        // 中央から左(下)、右(上)、左(下)、右(上)、、の順
        Collections.Generic.IList<Revit.DB.XYZ> pntsFromCenter = new Collections.Generic.List<Revit.DB.XYZ>();

        // 偶数本
        if (defaultPoints.Count % 2 == 0)
        {
          for (int i = 0; i < defaultPoints.Count / 2; ++i)
          {
            pntsFromCenter.Add(defaultPoints[defaultPoints.Count / 2 - 1 - i]);
            pntsFromCenter.Add(defaultPoints[defaultPoints.Count / 2 + i]);
          }
        }
        // 奇数本
        else
        {
          double d = defaultPoints.Count;
          double d2 = d / 2;
          int center = (int)System.Math.Floor(d2);

          pntsFromCenter.Add(defaultPoints[center]);

          for (int i = 0; i < defaultPoints.Count / 2; ++i)
          {
            pntsFromCenter.Add(defaultPoints[center - 1 - i]);
            pntsFromCenter.Add(defaultPoints[center + 1 + i]);
          }
        }


        for (int i = 0; i < rebarNum - 2; ++i)
        {
          ret.Add(pntsFromCenter[i]);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2段筋太径位置</summary>
    /// 
    /// <param name="defaultPoints">太径の端部を含む鉄筋の左から右の並び</param>
    /// 
    /// <history>2013/10/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> Set2ndHutoRebar(int rebarNum,
                                                            Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                                                            bool vertical)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 同数
      if (rebarNum == defaultPoints.Count)
      {
        for (int i = 0; i < rebarNum; ++i)
        {
          if (i == 0 || i == rebarNum - 1)
          {
            continue;
          }

          ret.Add(defaultPoints[i]);
        }
      }
      else if(defaultPoints.Count > 0)
      {
        int num = defaultPoints.Count;
        Revit.DB.XYZ startPnt = defaultPoints[0];
        Revit.DB.XYZ endPnt = defaultPoints[num - 1];

        if (vertical == true)
        {
          double distance = endPnt.Y - startPnt.Y;
          double pitch = 0;
          if (rebarNum > 1)
          {
            pitch = distance / (rebarNum - 1);
          }

          double sumPitch = 0;

          for (int i = 0; i < rebarNum; ++i)
          {
            if (i == 0 || i == rebarNum - 1)
            {
              continue;
            }

            sumPitch += pitch;
            Revit.DB.XYZ p = new Revit.DB.XYZ(0, sumPitch, 0) + startPnt;

            p = NearYPoint(p, defaultPoints);

            ret.Add(p);
          }
        }
        else
        {
          double distance = endPnt.X - startPnt.X;
          double pitch = 0;
          if (rebarNum > 1)
          {
            pitch = distance / (rebarNum - 1);
          }

          double sumPitch = 0;

          for (int i = 0; i < rebarNum; ++i)
          {
            if (i == 0 || i == rebarNum - 1)
            {
              continue;
            }

            sumPitch += pitch;
            Revit.DB.XYZ p = new Revit.DB.XYZ(sumPitch, 0, 0) + startPnt;

            p = NearXPoint(p, defaultPoints);

            ret.Add(p);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2段筋細径位置の調整</summary>
    /// 
    /// <param name="defaultPoints">太径細径合わせた鉄筋の左から右の並び</param>
    /// 
    /// <history>2013/06/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> Set2ndHosoRebar(int rebarNum,
                                                            Collections.Generic.IList<Revit.DB.XYZ> defaultPoints,
                                                            Collections.Generic.IList<Revit.DB.XYZ> used,
                                                            Revit.DB.XYZ center,
                                                            bool vertical)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      if (defaultPoints.Count < 2)
      {
        return ret;
      }

      // 太径を配置していない点
      Collections.Generic.IList<Revit.DB.XYZ> unUsed = new Collections.Generic.List<Revit.DB.XYZ>();

      foreach (Revit.DB.XYZ p in defaultPoints)
      {
        bool isNear = false;

        foreach (Revit.DB.XYZ pnt in used)
        {
          if (base.Distance2D(p, pnt) < 1 / 304.8)
          {
            isNear = true;
          }
        }

        if (isNear == false)
        {
          unUsed.Add(p);
        }
      }

      if (unUsed.Count == 0)
      {
        return ret;
      }

      // 細径は中央に近い順に
      Collections.Generic.IList<Revit.DB.XYZ> nearPntOrder = NearPointsOrder(center, unUsed, vertical);

      for (int i = 0; i < rebarNum; ++i)
      {
        ret.Add(nearPntOrder[i]);
      }



      // 
      int num = defaultPoints.Count;
      // 前
      Revit.DB.XYZ startPnt = defaultPoints[0];
      // 後
      Revit.DB.XYZ endPnt = defaultPoints[num - 1];

      if (vertical == true)
      {
        double distance = endPnt.Y - startPnt.Y;
        double pitch = 0;
        if (rebarNum > 1)
        {
          pitch = distance / (rebarNum - 1);
        }


      }
      else
      {
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>1段筋に細径がないときの2段筋位置</summary>
    /// 
    /// <history><p>2013/10/15 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/02/15 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public void Set2ndRebar_No1danHoso(Collections.Generic.IList<int> order2dan,
                                       Collections.Generic.IList<Revit.DB.XYZ> all1danPoints,
                                       ref Collections.Generic.IList<Revit.DB.XYZ> hutoPoints,
                                       ref Collections.Generic.IList<Revit.DB.XYZ> hosoPoints,
                                       Revit.DB.XYZ center,
                                       bool vertical,
                                       bool is2ndCorner)
    {
      // コーナー配筋なしの場合、隅部座標を含んでいない
      if (all1danPoints.Count == 0)
      {
        return;
      }

      int num = order2dan.Count;
      Revit.DB.XYZ startPnt = all1danPoints[0];
      Revit.DB.XYZ endPnt = all1danPoints[all1danPoints.Count - 1];

      if (vertical == true)
      {
        double distance = endPnt.Y - startPnt.Y;
        double pitch = 0;
        if (num > 1)
        {
          pitch = distance / (num - 1);
        }
        if (is2ndCorner == true)
        {
          // コーナー配筋なしの場合、端部座標を含んでいない
          pitch = distance / (num - 3);
        }
        double sumPitch = 0;

        for (int i = 0; i < num; ++i)
        {
          if (i == 0 || i == num - 1)
          {
            continue;
          }

          if (is2ndCorner == true)
          {
            if (i == 1 || i == num - 2)
            {
              continue;
            }
          }

          sumPitch += pitch;
          Revit.DB.XYZ p = new Revit.DB.XYZ(0, sumPitch, 0) + startPnt;

          p = NearYPoint(p, all1danPoints);

          int hutohoso = order2dan[i];

          // 太径
          if (hutohoso == 0)
          {
            hutoPoints.Add(p);
          }
          // 細径
          else
          {
            hosoPoints.Add(p);
          }
        }
      }
      else
      {
        double distance = endPnt.X - startPnt.X;
        double pitch = 0;
        if (num > 1)
        {
          pitch = distance / (num - 1);
        }
        if (is2ndCorner == true)
        {
          // コーナー配筋なしの場合、端部座標を含んでいない
          pitch = distance / (num - 3);
        }
        double sumPitch = 0;

        for (int i = 0; i < num; ++i)
        {
          if (i == 0 || i == num - 1)
          {
            continue;
          }

          if (is2ndCorner == true)
          {
            if (i == 1 || i == num - 2)
            {
              continue;
            }
          }

          sumPitch += pitch;
          Revit.DB.XYZ p = new Revit.DB.XYZ(sumPitch, 0, 0) + startPnt;

          p = NearXPoint(p, all1danPoints);

          int hutohoso = order2dan[i];

          // 太径
          if (hutohoso == 0)
          {
            hutoPoints.Add(p);
          }
          // 細径
          else
          {
            hosoPoints.Add(p);
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>X方向の点または1つ外の点</summary>
    /// 
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.XYZ NearXPoint(Revit.DB.XYZ point,
                            Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      Revit.DB.XYZ ret = null;

      Collections.Generic.IList<Revit.DB.XYZ> sortX = SortByXAry(points);

      for (int i = 0; i < sortX.Count; ++i)
      {
        Revit.DB.XYZ p = sortX[i];

        // 1mm以下のずれならOK
        if (System.Math.Abs(point.X - p.X) <= 1 / 304.8)
        {
          ret = p;

          double count = sortX.Count;
          double half = count / 2;

          break;
        }
      }
      if (ret == null)
      {
        Revit.DB.XYZ min = sortX[0];
        Revit.DB.XYZ max = sortX[sortX.Count - 1];

        double distance = Distance2D(min, max);
        double halfDist = distance / 2;

        // 中間より前
        if (point.X <= min.X + halfDist)
        {
          for (int i = 0; i < sortX.Count; ++i)
          {
            if (point.X < sortX[i].X)
            {
              ret = sortX[i - 1];

              break;
            }
          }
        }
        // 中間より後
        else
        {
          for (int i = 0; i < sortX.Count; ++i)
          {
            if (point.X < sortX[i].X)
            {
              ret = sortX[i];

              break;
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>X方向の近似点判定</summary>
    /// 
    /// <history><p>2013/10/16 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool isNearXPoint(Revit.DB.XYZ point,
                      Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      bool ret = false;

      // 1mm
      double limit = 1 / 304.8;

      foreach (Revit.DB.XYZ p in points)
      {
        // 差
        double distance = System.Math.Abs(p.X - point.X);

        if (distance <= limit)
        {
          ret = true;

          break;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>Y方向の点または1つ外の点</summary>
    /// 
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.XYZ NearYPoint(Revit.DB.XYZ point,
                            Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      Revit.DB.XYZ ret = null;
      Collections.Generic.IList<Revit.DB.XYZ> sortY = SortByYAry(points);

      for (int i = 0; i < sortY.Count; ++i)
      {
        Revit.DB.XYZ p = sortY[i];

        // 1mm以下のずれならOK
        if (System.Math.Abs(point.Y - p.Y) <= 1 / 304.8)
        {
          ret = p;

          double count = sortY.Count;
          double half = count / 2;

          break;
        }
      }
      if (ret == null)
      {
        Revit.DB.XYZ min = sortY[0];
        Revit.DB.XYZ max = sortY[sortY.Count - 1];

        double distance = Distance2D(min, max);
        double halfDist = distance / 2;

        // 中間より前
        if (point.Y <= min.Y + halfDist)
        {
          for (int i = 0; i < sortY.Count; ++i)
          {
            if (point.Y < sortY[i].Y)
            {
              ret = sortY[i - 1];

              break;
            }
          }
        }
        // 中間より後
        else
        {
          for (int i = 0; i < sortY.Count; ++i)
          {
            if (point.Y < sortY[i].Y)
            {
              ret = sortY[i];

              break;
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>Y方向の近似点判定</summary>
    /// 
    /// <history><p>2013/10/16 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool isNearYPoint(Revit.DB.XYZ point,
                      Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      bool ret = false;

      // 1mm
      double limit = 1 / 304.8;

      foreach (Revit.DB.XYZ p in points)
      {
        // 差
        double distance = System.Math.Abs(p.Y - point.Y);

        if (distance <= limit)
        {
          ret = true;

          break;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>Y座標が小さい順に並び替え</summary>
    /// 
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SortByYAry(Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> list = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.XYZ p in points)
      {
        list.Add(p);
      }

      while (list.Count > 0)
      {
        Revit.DB.XYZ point = null;

        foreach (Revit.DB.XYZ p in list)
        {
          if (point == null)
          {
            point = p;
            continue;
          }

          // 一番小さい点
          if (point.Y > p.Y)
          {
            point = p;
          }
        }

        ret.Add(point);
        list.Remove(point);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>X座標が小さい順に並び替え</summary>
    /// 
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SortByXAry(Collections.Generic.IList<Revit.DB.XYZ> points)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> list = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.XYZ p in points)
      {
        list.Add(p);
      }

      while (list.Count > 0)
      {
        Revit.DB.XYZ point = null;

        foreach (Revit.DB.XYZ p in list)
        {
          if (point == null)
          {
            point = p;
            continue;
          }

          // 一番小さい点
          if (point.X > p.X)
          {
            point = p;
          }
        }

        ret.Add(point);
        list.Remove(point);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>中心からのX座標逆位置</summary>
    /// 
    /// <history>2013/07/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ ReversePoint_X(Revit.DB.XYZ center, Revit.DB.XYZ pnt)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ distance = pnt - center;

      ret = new Revit.DB.XYZ((center - distance).X, pnt.Y, pnt.Z);

      return ret;
    }

    /// ================================================================================
    /// <summary>直線と円の交点座標</summary>
    /// 
    /// <history>2013/10/04 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> CrossPoint(Revit.DB.Line line,
                                                       Revit.DB.XYZ center,
                                                       double radius)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Revit.DB.XYZ p1 = line.GetEndPoint(0);
      Revit.DB.XYZ p2 = line.GetEndPoint(1);

      // 定数
      double a = p2.Y - p1.Y;
      double b = p1.X - p2.X;
      double c = -(a * p1.X + b * p1.Y);

      // 単位ベクトル
      Revit.DB.XYZ unitVec = base.UnitVector(p1, p2);

      double unitX = unitVec.X;
      double unitY = unitVec.Y;

      // 直角な単位ベクトル
      double verUnitX = -unitY;
      double verUnitY = unitX;

      // 円の中心から直線への垂線の足の長さ
      double k = -(a * center.X + b * center.Y + c) / (a * verUnitX + b * verUnitY);

      // 交点なし
      if (k > radius)
      {
        return ret;
      }

      // 垂線の交点
      double crossX = center.X + k * verUnitX;
      double crossY = center.Y + k * verUnitY;

      // 同じ
      if (ToHalfAdjust(k, -9) == ToHalfAdjust(radius, -9))
      {
        Revit.DB.XYZ p = new Revit.DB.XYZ(crossX, crossY, 0);
        ret.Add(p);
        return ret;
      }

      // 垂線の足と円と直線との交点の距離
      double s = 0;
      try
      {
        s = System.Math.Sqrt(radius * radius - k * k);

        if (double.IsNaN(s))
        {
          return ret;
        }
      }
      catch
      {
        return ret;
      }

      // 垂線の足から直線の単位ベクトル方向にs移動した位置
      double x1 = crossX + s * unitX;
      double y1 = crossY + s * unitY;

      double x2 = crossX - s * unitX;
      double y2 = crossY - s * unitY;

      Revit.DB.XYZ pnt1 = new Revit.DB.XYZ(x1, y1, 0);
      Revit.DB.XYZ pnt2 = new Revit.DB.XYZ(x2, y2, 0);

      ret.Add(pnt1);
      ret.Add(pnt2);

      return ret;
    }

    /// ================================================================================
    /// <summary>n位(10^digits)に四捨五入</summary>
    /// ================================================================================
    public
    double ToHalfAdjust(double value, int digits)
    {
      digits = digits * -1;

      // 10のべき乗
      double dCoef = System.Math.Pow(10, digits);

      // 指定位を1の位にする
      // 値が0より大きい場合
      // 0.5を足した値以下の最大の整数を10のべき乗で割る
      // 値が0以下の場合
      // 0.5を引いた値以上の最小の整数を10のべき乗で割る
      return value > 0 ? System.Math.Floor((value * dCoef) + 0.5) / dCoef :
                         System.Math.Ceiling((value * dCoef) - 0.5) / dCoef;

      // 例 1
      // value = 10.56, digits = 0
      // 10^digits = 10^0 = 1
      // value > 0 == true
      // Floor(10.56 * 1 + 0.5) = Floor(10.56 + 0.5) = Floor(11.06) = 11
      // return 11

      // 例 2
      // value = 0.014, digits = -2
      // digits = -2 * -1
      // 10^digits = 10^2 = 100
      // value > 0 == false
      // Ceiling(0.014 * 100 - 0.5) = Ceiling(1.4 -0.5) = Ceiling(0.9) = 1
      // 1 / 100 = 0.01
      // return 0.01
    }

    #endregion

  }
}
