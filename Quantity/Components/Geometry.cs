using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace Quantity.Components
{
  /// ================================================================================
  /// <summary>図形</summary>
  /// ================================================================================
  public class Geometry
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private Quantity.Components.Attribute _CmpAttribute;
    private readonly double _shortCurveTol;

    /// <summary>スペース境界</summary>
    private Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>> _SpaceBndryCrv;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="rvtUIDoc"    >Revit UIドキュメント</param>
    /// <param name="cmpAttribute">属性</param>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Geometry(Revit.UI.UIDocument rvtUIDoc, Quantity.Components.Attribute cmpAttribute)
    {
      _CmpAttribute = cmpAttribute;
      _shortCurveTol = rvtUIDoc.Document.Application.ShortCurveTolerance;
      _SpaceBndryCrv = new Collections.Generic.Dictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>>();
    }
    #endregion

    // メンバ関数
    #region
    private static double GeomDistance2D(Revit.DB.XYZ p0, Revit.DB.XYZ p1)
    {
      double dx = p1.X - p0.X;
      double dy = p1.Y - p0.Y;
      return System.Math.Sqrt(dx * dx + dy * dy);
    }

    private static double GeomDistance3D(Revit.DB.XYZ p0, Revit.DB.XYZ p1) => p0.DistanceTo(p1);

    public double Distance2D(Revit.DB.XYZ p0, Revit.DB.XYZ p1) => GeomDistance2D(p0, p1);

    public double Distance(Revit.DB.XYZ p0, Revit.DB.XYZ p1) => GeomDistance3D(p0, p1);

    public Revit.DB.XYZ Center2Point(Revit.DB.XYZ p0, Revit.DB.XYZ p1) => p0 + (p1 - p0) * 0.5;

    public Revit.DB.XYZ PolygonGravity2D(Collections.Generic.IList<Revit.DB.Curve> curves)
    {
      if (curves == null || curves.Count < 1)
        return Revit.DB.XYZ.Zero;
      Revit.DB.XYZ acc = Revit.DB.XYZ.Zero;
      double zSum = 0;
      foreach (Revit.DB.Curve curve in curves)
      {
        Revit.DB.XYZ pt = curve.Evaluate(0.5, true);
        acc += new Revit.DB.XYZ(pt.X, pt.Y, 0);
        zSum += pt.Z;
      }
      double inv = 1.0 / curves.Count;
      return new Revit.DB.XYZ(acc.X * inv, acc.Y * inv, zSum * inv);
    }

    public double DotProduct2D(Revit.DB.XYZ p0, Revit.DB.XYZ p1, Revit.DB.XYZ p2)
    {
      double x1 = p1.X - p0.X;
      double y1 = p1.Y - p0.Y;
      double x2 = p2.X - p0.X;
      double y2 = p2.Y - p0.Y;
      return x1 * x2 + y1 * y2;
    }

    public double CrossProduct2D(Revit.DB.XYZ p0, Revit.DB.XYZ p1, Revit.DB.XYZ p2)
    {
      double x1 = p1.X - p0.X;
      double y1 = p1.Y - p0.Y;
      double x2 = p2.X - p0.X;
      double y2 = p2.Y - p0.Y;
      return x1 * y2 - y1 * x2;
    }

    /// ================================================================================
    /// <summary>スペースの境界線分</summary>
    /// 
    /// <param name="spaces"  >スペース</param>
    /// <param name="bndryNum"><p>境界条件 : </p>
    ///                         <p>0 = 壁仕上げ、</p>
    ///                         <p>1 = 壁中心、</p>
    ///                         <p>2 = 躯体中心、</p>
    ///                         <p>3 = 躯体境界</p></param>
    /// 
    /// <history>2014/11/12 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetSpacesBndryCrv(Collections.Generic.IList<Revit.DB.Mechanical.Space> spaces,
                           int bndryNum)
    {
      foreach (Revit.DB.Mechanical.Space space in spaces)
      {
        Collections.Generic.IList<Revit.DB.Curve> spaceBndryCrv = GetSpaceBndryCrv(space, bndryNum);

        _SpaceBndryCrv.Add(space, spaceBndryCrv);
      }
    }

    /// ================================================================================
    /// <summary>スペースの境界線分</summary>
    /// 
    /// <param name="space"   >スペース</param>
    /// <param name="bndryNum"><p>境界条件 : </p>
    ///                         <p>0 = 壁仕上げ、</p>
    ///                         <p>1 = 壁中心、</p>
    ///                         <p>2 = 躯体中心、</p>
    ///                         <p>3 = 躯体境界</p></param>
    /// 
    /// <history><p>2014/09/08 Created GSA, Inc. Ryo Kuroda</p>
    ///           <p>2015/09/28 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetSpaceBndryCrv(Revit.DB.Mechanical.Space space,
                                                               int bndryNum)
    {
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      Revit.DB.SpatialElementBoundaryOptions opt = new Revit.DB.SpatialElementBoundaryOptions();

      // 境界条件
      switch (bndryNum)
      {
        case 0:
          opt.SpatialElementBoundaryLocation = Revit.DB.SpatialElementBoundaryLocation.Finish;
          break;

        case 1:
          opt.SpatialElementBoundaryLocation = Revit.DB.SpatialElementBoundaryLocation.Center;
          break;

        case 2:
          opt.SpatialElementBoundaryLocation = Revit.DB.SpatialElementBoundaryLocation.CoreCenter;
          break;

        case 3:
          opt.SpatialElementBoundaryLocation = Revit.DB.SpatialElementBoundaryLocation.CoreBoundary;
          break;

        default:
          opt.SpatialElementBoundaryLocation = Revit.DB.SpatialElementBoundaryLocation.Finish;
          break;
      }

      // スペース境界線取得
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.BoundarySegment>> bndrySgmtAryAry = space.GetBoundarySegments(opt);

      foreach (Collections.Generic.IList<Revit.DB.BoundarySegment> bndrySgmtAry in bndrySgmtAryAry)
      {
        foreach (Revit.DB.BoundarySegment bndrySgmt in bndrySgmtAry)
        {
          Revit.DB.Curve crv = bndrySgmt.GetCurve();

          if (crv != null)
          {
            ret.Add(crv);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線のXY交点</summary>
    /// 
    /// <param name="p0"  >直線1 始点</param>
    /// <param name="p1"  >直線1 終点</param>
    /// <param name="p2"  >直線2 始点</param>
    /// <param name="p3"  >直線2 終点</param>
    /// <param name="mode"><p>モード : </p>
    ///                     <p>mode = 0 完全交差, </p>
    ///                     <p>mode = 1 端点交差含む, </p>
    ///                     <p>mode = 2 延長線上交差含む</p></param>
    /// 
    /// <history>2014/11/10 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ CrossPointXY(Revit.DB.XYZ p0,
                              Revit.DB.XYZ p1,
                              Revit.DB.XYZ p2,
                              Revit.DB.XYZ p3,
                              int mode)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p0, p1);

      double f1 = p1.X - p0.X;
      double g1 = p1.Y - p0.Y;
      double f2 = p3.X - p2.X;
      double g2 = p3.Y - p2.Y;

      double det = f2 * g1 - f1 * g2;

      // 平行
      if (ToHalfAdjust(det, -9) == 0)
      {
        return ret;
      }

      double dx = p2.X - p0.X;
      double dy = p2.Y - p0.Y;

      double t1 = (f2 * dy - g2 * dx) / det;
      double t2 = (f1 * dy - g1 * dx) / det;

      if (mode == 0)
      {
        if (ToHalfAdjust(t1, -9) <= 0 ||
            ToHalfAdjust(t1, -9) >= 1 ||
            ToHalfAdjust(t2, -9) <= 0 ||
            ToHalfAdjust(t2, -9) >= 1)
        {
          return ret;
        }
      }
      if (mode == 1)
      {
        if (ToHalfAdjust(t1, -9) < 0 ||
            ToHalfAdjust(t1, -9) > 1 ||
            ToHalfAdjust(t2, -9) < 0 ||
            ToHalfAdjust(t2, -9) > 1)
        {
          return ret;
        }
      }

      double x = p0.X + f1 * t1;
      double y = p0.Y + g1 * t1;
      double z = 0;

      Revit.DB.XYZ p = new Revit.DB.XYZ(x, y, z);

      // 線上
      if (IsPointOnLine(l1, p))
      {
        z = GetZPointOnLine(l1, x, y);
      }
      else
      {
        z = GetExtLineZPoint(l1, x, y);
      }

      ret = new Revit.DB.XYZ(x, y, z);

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線のXY交点</summary>
    /// 
    /// <param name="l1"  >直線1</param>
    /// <param name="l2"  >直線2</param>
    /// <param name="mode"><p>モード : </p>
    ///                     <p>mode = 0 完全交差, </p>
    ///                     <p>mode = 1 端点交差含む, </p>
    ///                     <p>mode = 2 延長線上交差含む</p></param>
    /// 
    /// <history>2014/04/28 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ CrossPointXY(Revit.DB.Line l1,
                              Revit.DB.Line l2,
                              int mode)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = l1.GetEndPoint(0);
      Revit.DB.XYZ p1 = l1.GetEndPoint(1);
      Revit.DB.XYZ p2 = l2.GetEndPoint(0);
      Revit.DB.XYZ p3 = l2.GetEndPoint(1);

      double f1 = p1.X - p0.X;
      double g1 = p1.Y - p0.Y;
      double f2 = p3.X - p2.X;
      double g2 = p3.Y - p2.Y;

      double det = f2 * g1 - f1 * g2;

      // 平行
      if (ToHalfAdjust(det, -9) == 0)
      {
        return ret;
      }

      double dx = p2.X - p0.X;
      double dy = p2.Y - p0.Y;

      double t1 = (f2 * dy - g2 * dx) / det;
      double t2 = (f1 * dy - g1 * dx) / det;

      if (mode == 0)
      {
        if (ToHalfAdjust(t1, -9) <= 0 ||
            ToHalfAdjust(t1, -9) >= 1 ||
            ToHalfAdjust(t2, -9) <= 0 ||
            ToHalfAdjust(t2, -9) >= 1)
        {
          return ret;
        }
      }
      if (mode == 1)
      {
        if (ToHalfAdjust(t1, -9) < 0 ||
            ToHalfAdjust(t1, -9) > 1 ||
            ToHalfAdjust(t2, -9) < 0 ||
            ToHalfAdjust(t2, -9) > 1)
        {
          return ret;
        }
      }

      double x = p0.X + f1 * t1;
      double y = p0.Y + g1 * t1;
      double z = 0;

      Revit.DB.XYZ p = new Revit.DB.XYZ(x, y, z);

      // 線上
      if (IsPointOnLine(l1, p))
      {
        z = GetZPointOnLine(l1, x, y);
      }
      else
      {
        z = GetExtLineZPoint(l1, x, y);
      }

      ret = new Revit.DB.XYZ(x, y, z);

      return ret;
    }

    /// ================================================================================
    /// <summary>直線と円弧のXY交点</summary>
    /// 
    /// <param name="line">直線</param>
    /// <param name="arc" >円弧</param>
    /// 
    /// <history>2014/08/21 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetXYCrossPoint(Revit.DB.Line line,
                                                            Revit.DB.Arc arc)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> tessellate = arc.Tessellate();

      for (int i = 0; i < tessellate.Count; ++i)
      {
        if (i == tessellate.Count - 1)
        {
          break;
        }

        Revit.DB.XYZ tp0 = line.GetEndPoint(0);
        Revit.DB.XYZ tp1 = line.GetEndPoint(1);

        Revit.DB.XYZ tp2 = tessellate[i];
        Revit.DB.XYZ tp3 = tessellate[i + 1];

        Revit.DB.XYZ cross = CrossPointXY(tp0, tp1, tp2, tp3, 1);

        if (cross != null)
        {
          ret.Add(cross);
        }
      }

      if (ret.Count > -1)
      {
        return ret;
      }


      // 三次元の場合は
      // 円弧から面を求めて、
      // その平面上に線分があるか確かめ、
      // その平面上での交差判定を行う？


      // 直線端点
      Revit.DB.XYZ lp0 = line.GetEndPoint(0);
      Revit.DB.XYZ lp1 = line.GetEndPoint(1);

      // 円弧中心点
      Revit.DB.XYZ center = arc.Center;
      double xc = center.X;
      double yc = center.Y;

      // 円弧半径
      double r = arc.Radius / 2;

      // 直線の方程式 (ax + by + c = 0)
      double a = lp1.Y - lp0.Y;
      double b = lp0.X - lp1.X;
      double c = -(a * lp0.X + b * lp0.Y);

      // 距離
      double l = line.Length;

      // 直線方向の単位ベクトル
      double ex = -(lp1.X - lp0.X) / l;
      double ey = -(lp1.Y - lp0.Y) / l;

      // 直角な単位ベクトル
      double vx = -ey;
      double vy = ex;

      // 垂線の足の長さ
      double k = -(a * xc + b * yc + c) / (a * vx + b * vy);

      if (double.IsInfinity(k) || double.IsNaN(k) || k < 0)
      {
        return ret;
      }

      // 円弧中心点から垂線の端点
      double xp = xc + k * vx;
      double yp = yc + k * vy;

      // 半径より足が長い = 交点なし
      if (ToHalfAdjust(r, -9) < ToHalfAdjust(k, -9))
      {
        return ret;
      }

      // 半径^2 = 垂線の足の長さ^2 + 垂線の足と交点の距離^2 ・・・(三平方の定理)を変形
      // 垂線の足と交点の距離 = √半径^2 - 垂線の足の長さ^2
      double s = System.Math.Sqrt(r * r - k * k);

      // 垂線の端点から直線の単位ベクトル方向に±sが直線と円弧の交点

      // 交点1の座標
      double x1 = xp + s * ex;
      double y1 = yp + s * ey;

      Revit.DB.XYZ p0 = new Revit.DB.XYZ(x1, y1, 0);

      // 点が直線上か
      // (交点は"線分と円弧"ではなく"直線と円弧"なので線上判定を行う)
      if (IsPointOnLine(line, p0))
      {
        ret.Add(p0);
      }

      // 交点2の座標
      double x2 = xp - s * ex;
      double y2 = yp - s * ey;

      Revit.DB.XYZ p1 = new Revit.DB.XYZ(x2, y2, 0);

      if (IsPointOnLine(line, p1))
      {
        ret.Add(p1);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>点の直線上判定</summary>
    /// 
    /// <param name="line">直線</param>
    /// <param name="p"   >点</param>
    /// 
    /// <history>2014/08/21 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsPointOnLine(Revit.DB.Line line,
                       Revit.DB.XYZ p)
    {
      bool ret = false;

      // 始点
      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      // 終点
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 端点上

      // 始点上
      if (ToHalfAdjust(GeomDistance2D(p0, p), -9) == 0)
      {
        ret = true;
        return ret;
      }
      // 終点上
      if (ToHalfAdjust(GeomDistance2D(p1, p), -9) == 0)
      {
        ret = true;
        return ret;
      }

      // 端点より内側

      // 始点から終点のラジアン
      double rad1 = GetRadian(p0, p1);

      // 終点から始点のラジアン
      double rad2 = GetRadian(p1, p0);

      // 始点から点のラジアン
      double rad3 = GetRadian(p0, p);

      // 終点から点のラジアン
      double rad4 = GetRadian(p1, p);

      // 始点からのラジアンが同じかつ、終点からのラジアンが同じ
      if (ToHalfAdjust(rad1, -9) == ToHalfAdjust(rad3, -9) &&
          ToHalfAdjust(rad2, -9) == ToHalfAdjust(rad4, -9))
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>直線分XY指定点のZ座標</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="midX">中間X座標</param>
    /// <param name="midY">中間Y座標</param>
    /// 
    /// <history>2014/09/25 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    double GetZPointOnLine(Revit.DB.Line line, double midX, double midY)
    {
      double ret = 0;

      // 始点終点
      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 中間点
      Revit.DB.XYZ midP = new Revit.DB.XYZ(midX, midY, 0);

      // 平面距離
      double distance2d = GeomDistance2D(p0, p1);
      double disMid = GeomDistance2D(p0, midP);

      // 高さ差分
      double z = p1.Z - p0.Z;

      // 単位距離あたりの高さ
      double unitZ = z / distance2d;

      ret = unitZ * disMid + p0.Z;

      return ret;
    }

    /// ================================================================================
    /// <summary>直線分XY延長時のZ座標</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="extX">延長X座標</param>
    /// <param name="extY">延長Y座標</param>
    /// 
    /// <history>2014/08/13 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    double GetExtLineZPoint(Revit.DB.Line line,
                            double extX,
                            double extY)
    {
      // 戻り値
      double ret = 0;

      Revit.DB.XYZ p = new Revit.DB.XYZ(extX, extY, 0);

      double dis0 = GeomDistance2D(p, line.GetEndPoint(0));
      double dis1 = GeomDistance2D(p, line.GetEndPoint(1));

      // 終点からの延長座標
      // 遠い方がp0 = 始点, 近い方がp1 = 終点
      if (dis0 > dis1)
      {
        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        double z0 = p0.Z;
        double z1 = p1.Z;

        // XY平面での距離
        double disXY0 = GeomDistance2D(p0, p1);
        double disXY1 = GeomDistance2D(p0, p);

        // 線分のZ差分
        double disZ = z1 - z0;

        // 傾き(高さ÷平面距離)
        double rate = disZ / disXY0;

        // Y = aX + b
        ret = rate * disXY1 + z0;
      }
      else if (dis0 < dis1)
      {
        Revit.DB.XYZ p0 = line.GetEndPoint(1);
        Revit.DB.XYZ p1 = line.GetEndPoint(0);

        double z0 = p0.Z;
        double z1 = p1.Z;

        // XY平面での距離
        double disXY0 = GeomDistance2D(p0, p1);
        double disXY1 = GeomDistance2D(p0, p);

        // 線分のZ差分
        double disZ = z1 - z0;

        // 傾き(高さ÷平面距離)
        double rate = disZ / disXY0;

        // Y = aX + b
        ret = rate * disXY1 + z0;
      }
      else
      {
        // 今回は端点の中点はないはず
        // この場合は指定位置が正しくない(今回は)

        // 今回の用途
        // 連続する横管のラインに縦管のXY座標を与え、交差座標を求める
        // Z = aXY + b
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>基準配管と1番90度近く傾いている配管</summary>
    /// 
    /// <param name="pipe"></param>
    /// <param name="pipes"></param>
    /// 
    /// <history>2015/01/21 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Plumbing.Pipe GetMostAnglePipe(Revit.DB.Plumbing.Pipe pipe,
                                            Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipes)
    {
      // 戻り値
      Revit.DB.Plumbing.Pipe ret = null;

      double rad = 0;

      Revit.DB.Line l1 = GetPipeLine(pipe);
      double rad1 = GetRadian(l1.GetEndPoint(0), l1.GetEndPoint(1));

      // 0以上に
      while (rad1 < 0)
      {
        rad1 += System.Math.PI;
      }

      // 180以下に
      while (rad1 > System.Math.PI)
      {
        rad1 -= System.Math.PI;
      }


      foreach (Revit.DB.Plumbing.Pipe p in pipes)
      {
        if (pipe.Id.ToString() == p.Id.ToString())
        {
          continue;
        }


        Revit.DB.Line l2 = GetPipeLine(p);
        double rad2 = GetRadian(l2.GetEndPoint(0), l2.GetEndPoint(1));

        // 0以上に
        while (rad2 < 0)
        {
          rad2 += System.Math.PI;
        }

        // 180以下に
        while (rad2 > System.Math.PI)
        {
          rad2 -= System.Math.PI;
        }


        double sa = System.Math.Abs(rad2 - rad1);

        if (ret == null)
        {
          ret = p;
          rad = sa;
        }
        else
        {
          if (System.Math.Abs(sa - System.Math.PI / 2) < System.Math.Abs(rad - System.Math.PI / 2))
          {
            ret = p;
            rad = sa;
          }
        }
      }


      return ret;
    }

    /// ================================================================================
    /// <summary>基準ダクトと1番90度近く傾いているダクト</summary>
    /// 
    /// <param name="duct"></param>
    /// <param name="ducts"></param>
    /// 
    /// <history>2015/01/21 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Mechanical.Duct GetMostAngleDuct(Revit.DB.Mechanical.Duct duct,
                                              Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts)
    {
      // 戻り値
      Revit.DB.Mechanical.Duct ret = null;

      double rad = 0;

      Revit.DB.Line l1 = GetDuctLine(duct);
      double rad1 = GetRadian(l1.GetEndPoint(0), l1.GetEndPoint(1));

      // 0以上に
      while (rad1 < 0)
      {
        rad1 += System.Math.PI;
      }

      // 180以下に
      while (rad1 > System.Math.PI)
      {
        rad1 -= System.Math.PI;
      }


      foreach (Revit.DB.Mechanical.Duct d in ducts)
      {
        if (duct.Id.ToString() == d.Id.ToString())
        {
          continue;
        }


        Revit.DB.Line l2 = GetDuctLine(d);
        double rad2 = GetRadian(l2.GetEndPoint(0), l2.GetEndPoint(1));

        // 0以上に
        while (rad2 < 0)
        {
          rad2 += System.Math.PI;
        }

        // 180以下に
        while (rad2 > System.Math.PI)
        {
          rad2 -= System.Math.PI;
        }


        double sa = System.Math.Abs(rad2 - rad1);

        if (ret == null)
        {
          ret = d;
          rad = sa;
        }
        else
        {
          if (System.Math.Abs(sa - System.Math.PI / 2) < System.Math.Abs(rad - System.Math.PI / 2))
          {
            ret = d;
            rad = sa;
          }
        }
      }


      return ret;
    }

    /// ================================================================================
    /// <summary>指定点に一番近い端点を持つ配管</summary>
    /// 
    /// <param name="pipe"  >対象外配管</param>
    /// <param name="pnt"   >指定点</param>
    /// <param name="pipes" >配管</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Plumbing.Pipe GetNearEndPntPipe(Revit.DB.Plumbing.Pipe pipe,
                                             ref Revit.DB.XYZ pnt,
                                             Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipes)
    {
      Revit.DB.Plumbing.Pipe ret = null;

      Revit.DB.XYZ endPnt = null;

      foreach (Revit.DB.Plumbing.Pipe p in pipes)
      {
        if (pipe != null)
        {
          if (pipe.Id.ToString() == p.Id.ToString())
          {
            continue;
          }
        }

        Revit.DB.Line line = GetPipeLine(p);


        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (endPnt == null)
        {
          endPnt = p0;

          ret = p;
        }

        if (GeomDistance2D(pnt, endPnt) > GeomDistance2D(pnt, p0))
        {
          endPnt = p0;

          ret = p;
        }
        if (GeomDistance2D(pnt, endPnt) > GeomDistance2D(pnt, p1))
        {
          endPnt = p1;

          ret = p;
        }
      }

      pnt = endPnt;

      return ret;
    }

    /// ================================================================================
    /// <summary>指定点に一番近い端点を持つダクト</summary>
    /// 
    /// <param name="pipe"  >対象外ダクト</param>
    /// <param name="pnt"   >指定点</param>
    /// <param name="ducts" >配管</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Mechanical.Duct GetNearEndPntDuct(Revit.DB.Mechanical.Duct duct,
                                             ref Revit.DB.XYZ pnt,
                                             Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts)
    {
      Revit.DB.Mechanical.Duct ret = null;

      Revit.DB.XYZ endPnt = null;

      foreach (Revit.DB.Mechanical.Duct d in ducts)
      {
        if (duct.Id.ToString() == d.Id.ToString())
        {
          continue;
        }

        Revit.DB.Line line = GetDuctLine(d);


        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (endPnt == null)
        {
          endPnt = p0;

          ret = d;
        }

        if (GeomDistance2D(pnt, endPnt) > GeomDistance2D(pnt, p0))
        {
          endPnt = p0;

          ret = d;
        }
        if (GeomDistance2D(pnt, endPnt) > GeomDistance2D(pnt, p1))
        {
          endPnt = p1;

          ret = d;
        }
      }

      pnt = endPnt;

      return ret;
    }

    /// ================================================================================
    /// <summary>配管端点</summary>
    /// 
    /// <param name="sortedDucts" >ソート済み配管</param>
    /// <param name="pnt"         >始点</param>
    /// 
    /// <history><p>2014/07/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/03/17 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetPoints(Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sortedPipes,
                                                      Revit.DB.XYZ pnt)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Revit.DB.XYZ p0 = null;
      Revit.DB.XYZ p1 = null;

      for (int i = 0; i < sortedPipes.Count; ++i)
      {
        Revit.DB.Plumbing.Pipe pipe = sortedPipes[i];
        Revit.DB.Line line = GetPipeLine(pipe);

        // 初回
        if (p0 == null && p1 == null)
        {
          p0 = pnt;

          if (GeomDistance2D(p0, line.GetEndPoint(0)) > GeomDistance2D(p0, line.GetEndPoint(1)))
          {
            p1 = line.GetEndPoint(0);
          }
          else
          {
            p1 = line.GetEndPoint(1);
          }

          ret.Add(p0);
          ret.Add(p1);
        }
        else
        {
          // 始点に近い方の点から
          if (GeomDistance2D(p1, line.GetEndPoint(0)) < GeomDistance2D(p1, line.GetEndPoint(1)))
          {
            p0 = line.GetEndPoint(0);
            p1 = line.GetEndPoint(1);
          }
          else
          {
            p0 = line.GetEndPoint(1);
            p1 = line.GetEndPoint(0);
          }

          ret.Add(p0);
          ret.Add(p1);
        }
      }

      if (ret.Count > 2)
      {
        int count = ret.Count;

        for (int i = 0; i < count; ++i)
        {
          if (i == 0)
          {
            continue;
          }

          if (i % 2 == 0)
          {
            continue;
          }

          if (i == count - 1)
          {
            continue;
          }

          // 2点の中間点に変更
          Revit.DB.XYZ retP1 = ret[i];
          Revit.DB.XYZ retP2 = ret[i + 1];

          Revit.DB.XYZ centerP = (retP1 + retP2) / 2;

          ret[i] = centerP;
          ret[i + 1] = centerP;
        }
      }

      ret = SameXYPointRemove(ret);

      return ret;
    }

    /// ================================================================================
    /// <summary>ダクト端点</summary>
    /// 
    /// <param name="sortedPipes" >ソート済みダクト</param>
    /// <param name="pnt"         >始点</param>
    /// 
    /// <history><p>2014/07/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/03/17 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetPoints(Collections.Generic.IList<Revit.DB.Mechanical.Duct> sortedDucts,
                                                      Revit.DB.XYZ pnt)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Revit.DB.XYZ p0 = null;
      Revit.DB.XYZ p1 = null;

      for (int i = 0; i < sortedDucts.Count; ++i)
      {
        Revit.DB.Mechanical.Duct duct = sortedDucts[i];
        Revit.DB.Line line = GetDuctLine(duct);

        if (p0 == null && p1 == null)
        {
          p0 = pnt;

          if (GeomDistance2D(p0, line.GetEndPoint(0)) > GeomDistance2D(p0, line.GetEndPoint(1)))
          {
            p1 = line.GetEndPoint(0);
          }
          else
          {
            p1 = line.GetEndPoint(1);
          }

          ret.Add(p0);
          ret.Add(p1);
        }
        else
        {
          if (GeomDistance2D(p1, line.GetEndPoint(0)) < GeomDistance2D(p1, line.GetEndPoint(1)))
          {
            p0 = line.GetEndPoint(0);
            p1 = line.GetEndPoint(1);
          }
          else
          {
            p0 = line.GetEndPoint(1);
            p1 = line.GetEndPoint(0);
          }

          ret.Add(p0);
          ret.Add(p1);
        }
      }

      if (ret.Count > 2)
      {
        int count = ret.Count;

        for (int i = 0; i < count; ++i)
        {
          if (i == 0)
          {
            continue;
          }

          if (i % 2 == 0)
          {
            continue;
          }

          if (i == count - 1)
          {
            continue;
          }

          // 2点の中間点に変更
          Revit.DB.XYZ retP1 = ret[i];
          Revit.DB.XYZ retP2 = ret[i + 1];

          Revit.DB.XYZ centerP = (retP1 + retP2) / 2;

          ret[i] = centerP;
          ret[i + 1] = centerP;
        }
      }

      ret = SameXYPointRemove(ret);

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線の交点</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/03/27 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.XYZ TwoLineCrossPnt(Revit.DB.Line line1,
                                 Revit.DB.Line line2)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      double f1 = p1.X - p0.X;
      double f2 = p3.X - p2.X;
      double g1 = p1.Y - p0.Y;
      double g2 = p3.Y - p2.Y;

      double det = ToHalfAdjust(f2 * g1 - f1 * g2, -9);

      
      if (det == 0 || IsTwoLineParallelZeroDIstance_NoSlope(line1, line2))
      {
        // 両方が傾いているまたはどちらも傾いていない
        if ((ToHalfAdjust(p0.Z - p1.Z, -9) != 0 && ToHalfAdjust(p2.Z - p3.Z, -9) != 0) ||
            (ToHalfAdjust(p0.Z - p1.Z, -9) == 0 && ToHalfAdjust(p2.Z - p3.Z, -9) == 0))
        {
          Revit.DB.XYZ xyz1 = null;
          Revit.DB.XYZ xyz2 = null;

          // 近い点
          GetNearLinesPoints(line1, line2, ref xyz1, ref xyz2);

          // 中点
          Revit.DB.XYZ center = Center2Point(xyz1, xyz2);

          // 基準線の延長点Z座標
          double extz = GetExtLineZPoint(line1, center.X, center.Y);

          ret = new Revit.DB.XYZ(center.X, center.Y, extz);
        }
        // 一方が傾いている
        else
        {
          // 傾いている方
          // line1
          if (ToHalfAdjust(p0.Z - p1.Z, -9) != 0)
          {
            ret = GetOverPointOnLine(line1, p2.Z);
          }
          // line2
          if (ToHalfAdjust(p2.Z - p3.Z, -9) != 0)
          {
            ret = GetOverPointOnLine(line2, p0.Z);
          }
        }

        if (ret != null)
        {
          return ret;
        }
      }


      double dx = p2.X - p0.X;
      double dy = p2.Y - p0.Y;

      double t1 = (f2 * dy - g2 * dx) / det;

      double x = p0.X + f1 * t1;
      double y = p0.Y + g1 * t1;
      double z = p0.Z;
      // XZ、YZをもとめZ座標とする
      Revit.DB.XYZ xz = CrossPntXZ(line1, line2);
      Revit.DB.XYZ yz = CrossPntYZ(line1, line2);
      if (xz != null)
      {
        if (IsXYParallelTwoLine(line1, line2))
        {
          if (IsEqualSlopeTwoLine(line1, line2) == false)
          {
            x = xz.X;
          }
        }

        z = xz.Z;
      }
      if (yz != null)
      {
        if (IsXYParallelTwoLine(line1, line2))
        {
          if (IsEqualSlopeTwoLine(line1, line2) == false)
          {
            y = yz.Y;
          }
        }

        z = yz.Z;
      }

      z = GetExtLineZPoint(line1, x, y);

      ret = new Revit.DB.XYZ(x, y, z);

      return ret;
    }

    /// ================================================================================
    /// <summary>点からの垂線と直線の交点</summary>
    /// 
    /// <param name="line">直線</param>
    /// <param name="pnt"   >点</param>
    /// 
    /// <history><p>2014/07/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.XYZ CrossPntXY(Revit.DB.Line line,
                            Revit.DB.XYZ pnt)
    {
      // 戻り値
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      if (ToHalfAdjust(GeomDistance2D(p0, p1), -9) != 0)
      {
        double k = ((pnt.X - p0.X) * (p1.X - p0.X) + (pnt.Y - p0.Y) * (p1.Y - p0.Y) + 0) /
                   ((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y) + 0);

        ret = new Revit.DB.XYZ(k * p1.X + (1 - k) * p0.X,
                               k * p1.Y + (1 - k) * p0.Y,
                               0);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線のXZ交点</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ===============================================================================
    public
    Revit.DB.XYZ CrossPntXZ(Revit.DB.Line line1,
                            Revit.DB.Line line2)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      // XZ面に対して垂直な場合
      if ((ToHalfAdjust(p0.X, -9) == ToHalfAdjust(p1.X, -9) &&
          ToHalfAdjust(p0.Z, -9) == ToHalfAdjust(p1.Z, -9)) ||
          ToHalfAdjust(p2.X, -9) == ToHalfAdjust(p3.X, -9) &&
          ToHalfAdjust(p2.Z, -9) == ToHalfAdjust(p3.Z, -9))
      {
        return ret;
      }

      double f1 = p1.X - p0.X;
      double f2 = p3.X - p2.X;
      double g1 = p1.Z - p0.Z;
      double g2 = p3.Z - p2.Z;

      double det = ToHalfAdjust(f2 * g1 - f1 * g2, -9);

      if (det == 0)
      {
        return ret;
      }

      double dx = p2.X - p0.X;
      double dz = p2.Z - p0.Z;

      double t1 = (f2 * dz - g2 * dx) / det;

      double x = p0.X + f1 * t1;
      double z = p0.Z + g1 * t1;

      ret = new Revit.DB.XYZ(x, p0.Y, z);

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線のYZ交点</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ===============================================================================
    public
    Revit.DB.XYZ CrossPntYZ(Revit.DB.Line line1,
                            Revit.DB.Line line2)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      // YZ面に対して垂直な場合
      if ((ToHalfAdjust(p0.Y, -9) == ToHalfAdjust(p1.Y, -9) &&
           ToHalfAdjust(p0.Z, -9) == ToHalfAdjust(p1.Z, -9)) ||
           ToHalfAdjust(p2.Y, -9) == ToHalfAdjust(p3.Y, -9) &&
           ToHalfAdjust(p2.Z, -9) == ToHalfAdjust(p3.Z, -9))
      {
        return ret;
      }

      double f1 = p1.Y - p0.Y;
      double f2 = p3.Y - p2.Y;
      double g1 = p1.Z - p0.Z;
      double g2 = p3.Z - p2.Z;

      double det = ToHalfAdjust(f2 * g1 - f1 * g2, -9);

      if (det == 0)
      {
        return ret;
      }

      double dy = p2.Y - p0.Y;
      double dz = p2.Z - p0.Z;

      double t1 = (f2 * dz - g2 * dy) / det;

      double y = p0.Y + f1 * t1;
      double z = p0.Z + g1 * t1;

      ret = new Revit.DB.XYZ(p0.X, y, z);

      return ret;
    }

    /// ================================================================================
    /// <summary>指定高さの線分上点</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="midZ">指定高さ</param>
    /// 
    /// <history>2014/09/25 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetMidPointOnLine(Revit.DB.Line line,
                                   double midZ)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 両端が同じ高さ
      if (ToHalfAdjust(p0.Z, -9) == ToHalfAdjust(p1.Z, -9))
      {
        return ret;
      }

      // zが範囲内か
      if ((p0.Z <= midZ && midZ <= p1.Z) ||
          (p0.Z >= midZ && midZ >= p1.Z))
      {
        double x = p1.X - p0.X;
        double y = p1.Y - p0.Y;
        double z = p1.Z - p0.Z;

        if (z == 0)
        {
          return ret;
        }

        double unitX = x / z;
        double unitY = y / z;

        double midX = p0.X + unitX * (midZ - p0.Z);
        double midY = p0.Y + unitY * (midZ - p0.Z);

        ret = new Revit.DB.XYZ(midX, midY, midZ);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>指定高さの直線上点</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="midZ">指定高さ</param>
    /// 
    /// <history>2014/11/10 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetOverPointOnLine(Revit.DB.Line line,
                                    double midZ)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 差分
      double x = p1.X - p0.X;
      double y = p1.Y - p0.Y;
      double z = p1.Z - p0.Z;

      if (z == 0)
      {
        return ret;
      }

      // 単位高さ当たり
      double unitX = x / z;
      double unitY = y / z;

      double midX = p0.X + unitX * (midZ - p0.Z);
      double midY = p0.Y + unitY * (midZ - p0.Z);

      ret = new Revit.DB.XYZ(midX, midY, midZ);

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線がXY面上で平行かつ最短距離がゼロ(Z軸の傾き判定付き)</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool IsTwoLineParallelZeroDIstance(Revit.DB.Line line1,
                                       Revit.DB.Line line2)
    {
      // 戻り値
      bool ret = false;

      // 2直線ともにXY成分があるか
      if (ToHalfAdjust(GeomDistance2D(line1.GetEndPoint(0), line1.GetEndPoint(1)), -9) > 0 &&
          ToHalfAdjust(GeomDistance2D(line2.GetEndPoint(0), line2.GetEndPoint(1)), -9) > 0)
      {
        // XY平行判定
        if (IsXYParallelTwoLine(line1, line2))
        {
          // 距離
          if (TwoXYParallelLineDistance(line1, line2) == 0)
          {
            // 傾き判定
            if (IsEqualSlopeTwoLine(line1, line2))
            {
              ret = true;
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線がXY面上で平行かつ最短距離がゼロ(Z軸の傾き判定なし)</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool IsTwoLineParallelZeroDIstance_NoSlope(Revit.DB.Line line1,
                                               Revit.DB.Line line2)
    {
      // 戻り値
      bool ret = false;

      // 2直線ともにXY成分があるか
      if (ToHalfAdjust(GeomDistance2D(line1.GetEndPoint(0), line1.GetEndPoint(1)), -9) > 0 &&
          ToHalfAdjust(GeomDistance2D(line2.GetEndPoint(0), line2.GetEndPoint(1)), -9) > 0)
      {
        // XY平行判定
        if (IsXYParallelTwoLine(line1, line2))
        {
          // 距離
          if (TwoXYParallelLineDistance(line1, line2) == 0)
          {
            ret = true;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線のXY面上での平行判定</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool IsXYParallelTwoLine(Revit.DB.Line line1,
                             Revit.DB.Line line2)
    {
      // 戻り値
      bool ret = false;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      double f1 = p1.X - p0.X;
      double f2 = p3.X - p2.X;
      double g1 = p1.Y - p0.Y;
      double g2 = p3.Y - p2.Y;

      double det = ToHalfAdjust(f2 * g1 - f1 * g2, -2);

      // 交点なし = 平行
      if (det == 0)
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線の傾き判定</summary>
    /// 
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/09/22 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool IsEqualSlopeTwoLine(Revit.DB.Line line1,
                             Revit.DB.Line line2)
    {
      bool ret = false;

      Revit.DB.XYZ p1 = line1.GetEndPoint(0);
      Revit.DB.XYZ p2 = line1.GetEndPoint(1);
      Revit.DB.XYZ p3 = line2.GetEndPoint(0);
      Revit.DB.XYZ p4 = line2.GetEndPoint(1);

      Revit.DB.XYZ far1 = null;
      Revit.DB.XYZ far2 = null;

      GetFarLinesPoints(line1, line2, ref far1, ref far2);

      double xy1 = ToHalfAdjust(GeomDistance2D(p1, p2), -3);
      double xy2 = ToHalfAdjust(GeomDistance2D(p3, p4), -3);
      double xy3 = ToHalfAdjust(GeomDistance2D(far1, far2), -3);

      double z1 = ToHalfAdjust(System.Math.Abs(p1.Z - p2.Z), -3);
      double z2 = ToHalfAdjust(System.Math.Abs(p3.Z - p4.Z), -3);
      double z3 = ToHalfAdjust(System.Math.Abs(far1.Z - far2.Z), -3);

      if (double.IsInfinity(z1) || double.IsNaN(z1))
      {
        z1 = 0;
      }
      if (double.IsInfinity(z2) || double.IsNaN(z2))
      {
        z2 = 0;
      }
      if (double.IsInfinity(z3) || double.IsNaN(z3))
      {
        z3 = 0;
      }

      if (z1 == 0 && z2 == 0 && z3 == 0)
      {
        ret = true;
        return ret;
      }
      else if (z1 == 0 || z2 == 0 || z3 == 0)
      {
        ret = false;
        return ret;
      }

      // XY成分÷Z成分
      double rateXY1 = xy1 / z1;
      double rateXY2 = xy2 / z2;
      double rateXY3 = xy3 / z3;

      // A = B, B = C, よってA = C
      if (ToHalfAdjust(rateXY1, -2) == ToHalfAdjust(rateXY2, -2) &&
          ToHalfAdjust(rateXY2, -2) == ToHalfAdjust(rateXY3, -2))
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2直線の最近端点</summary>
    /// 
    /// <param name="line1" >直線1</param>
    /// <param name="line2" >直線2</param>
    /// <param name="xyz1"  >端点1</param>
    /// <param name="xyz2"  >端点2</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetNearLinesPoints(Revit.DB.Line line1,
                            Revit.DB.Line line2,
                            ref Revit.DB.XYZ xyz1,
                            ref Revit.DB.XYZ xyz2)
    {
      // 端点
      Revit.DB.XYZ p1 = line1.GetEndPoint(0);
      Revit.DB.XYZ p2 = line1.GetEndPoint(1);
      Revit.DB.XYZ p3 = line2.GetEndPoint(0);
      Revit.DB.XYZ p4 = line2.GetEndPoint(1);


      double distance = 0;

      double distance1 = GeomDistance2D(p1, p3);
      double distance2 = GeomDistance2D(p1, p4);
      double distance3 = GeomDistance2D(p2, p3);
      double distance4 = GeomDistance2D(p2, p4);

      xyz1 = p1;
      xyz2 = p3;
      distance = distance1;

      if (distance > distance2)
      {
        xyz1 = p1;
        xyz2 = p4;
        distance = distance2;
      }
      if (distance > distance3)
      {
        xyz1 = p2;
        xyz2 = p3;
        distance = distance3;
      }
      if (distance > distance4)
      {
        xyz1 = p2;
        xyz2 = p4;
        distance = distance4;
      }
    }

    /// ================================================================================
    /// <summary>2直線の最遠端点</summary>
    /// 
    /// <param name="line1" >直線1</param>
    /// <param name="line2" >直線2</param>
    /// <param name="xyz1"  >端点1</param>
    /// <param name="xyz2"  >端点2</param>
    /// 
    /// <history>2014/09/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetFarLinesPoints(Revit.DB.Line line1,
                           Revit.DB.Line line2,
                           ref Revit.DB.XYZ xyz1,
                           ref Revit.DB.XYZ xyz2)
    {
      // 端点
      Revit.DB.XYZ p1 = line1.GetEndPoint(0);
      Revit.DB.XYZ p2 = line1.GetEndPoint(1);
      Revit.DB.XYZ p3 = line2.GetEndPoint(0);
      Revit.DB.XYZ p4 = line2.GetEndPoint(1);


      double distance = 0;

      double distance1 = GeomDistance2D(p1, p3);
      double distance2 = GeomDistance2D(p1, p4);
      double distance3 = GeomDistance2D(p2, p3);
      double distance4 = GeomDistance2D(p2, p4);

      xyz1 = p1;
      xyz2 = p3;
      distance = distance1;

      if (distance < distance2)
      {
        xyz1 = p1;
        xyz2 = p4;
        distance = distance2;
      }
      if (distance < distance3)
      {
        xyz1 = p2;
        xyz2 = p3;
        distance = distance3;
      }
      if (distance < distance4)
      {
        xyz1 = p2;
        xyz2 = p4;
        distance = distance4;
      }
    }

    /// ================================================================================
    /// <summary>XY面上で平行な2直線の距離</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history><p>2014/07/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    double TwoXYParallelLineDistance(Revit.DB.Line line1,
                                     Revit.DB.Line line2)
    {
      // 戻り値
      double ret = 0;

      Revit.DB.XYZ p = line1.GetEndPoint(0);

      // 点からの垂線と直線の交点
      Revit.DB.XYZ crossP = CrossPntXY(line2, p);

      if (crossP != null)
      {
        // 2点の距離
        double dis = GeomDistance2D(p, crossP);

        ret = ToHalfAdjust(dis, -2);
      }

      return ret;
    }

    
    /// <summary>
    /// 線分の端部がカーブの外部にあるかを判定する（部分外を含む）
    /// </summary>
    /// <param name="line">判定する線分</param>
    /// <param name="crvs">カーブのリスト</param>
    /// <returns>端部がカーブの外部にある場合は true、それ以外の場合は false</returns>
    public bool IsOutCurves(Revit.DB.Line line, Collections.Generic.IList<Revit.DB.Curve> crvs)
    {
      // 交差カウントを初期化
      var count = 0;

      // すべてのカーブについて交差判定
      foreach (var crv in crvs)
      {
        count += IsCrossing(line, crv);
      }

      // 交差が偶数回の場合、両端は内部 / 奇数回の場合、片方の端点が外部
      return count % 2 != 0;
    }
    
    // /// ================================================================================
    // /// <summary>線分端部のカーブ外判定(部分外含む)</summary>
    // /// 
    // /// <param name="line">線分</param>
    // /// <param name="crvs">カーブ</param>
    // /// 
    // /// <history>2014/09/25 Created GSA, Inc. Ryo Kuroda</history>
    // /// ================================================================================
    // public
    // bool IsOutCurves(Revit.DB.Line line,
    //                  Collections.Generic.IList<Revit.DB.Curve> crvs)
    // {
    //   bool ret = false;
    //
    //   int count = 0;
    //
    //   foreach (Revit.DB.Curve crv in crvs)
    //   {
    //     count += IsCrossing(line, crv);
    //
    //     if (ret)
    //     {
    //       ret = true;
    //     }
    //   }
    //
    //   //if (count > 0)
    //   {
    //     // 交差が偶数回は両端内部
    //     if (count % 2 == 0)
    //     {
    //       ret = false;
    //     }
    //     else
    //     {
    //       ret = true;
    //     }
    //   }
    //   //else
    //   //{
    //   //  Revit.DB.XYZ gra2d = PolygonGravity2D(crvs);
    //
    //   //  if (gra2d != null)
    //   //  {
    //   //    Revit.DB.XYZ p0 = line.GetEndPoint(0);
    //   //    Revit.DB.XYZ p1 = line.GetEndPoint(1);
    //
    //   //    Revit.DB.Line l0 = Revit.DB.Line.CreateBound(gra2d, p0);
    //   //    Revit.DB.Line l1 = Revit.DB.Line.CreateBound(gra2d, p1);
    //
    //   //    foreach (Revit.DB.Curve c in crvs)
    //   //    {
    //   //      if (IsCrossing(l0, c) > 0)
    //   //      {
    //   //        return true;
    //   //      }
    //   //      if (IsCrossing(l1, c) > 0)
    //   //      {
    //   //        return true;
    //   //      }
    //   //    }
    //
    //   //  }
    //   //}
    //
    //   return ret;
    // }

    /// ================================================================================
    /// <summary>線分とカーブの交差判定</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="crv" >カーブ</param>
    /// 
    /// <history>2014/09/25 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    int IsCrossing(Revit.DB.Line line,
                    Revit.DB.Curve crv)
    {
      int ret = 0;

      // 直線
      if (crv.IsCyclic == false)
      {
        Revit.DB.Line l = crv as Revit.DB.Line;
        Revit.DB.XYZ cross = CrossPointXY(line, l, 1);

        if (cross != null)
        {
          ret += 1;
        }
      }
      // 曲線
      else
      {
        Revit.DB.Arc arc = crv as Revit.DB.Arc;
        Collections.Generic.IList<Revit.DB.XYZ> crosses = GetXYCrossPoint(line, arc);

        foreach (Revit.DB.XYZ cross in crosses)
        {
          ret += 1;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>線上不使用範囲点</summary>
    /// 
    /// <param name="line"    >線分</param>
    /// <param name="pntsAry" >使用点</param>
    /// 
    /// <history>2015/01/20 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> GetNotIncludeLinePoint(Revit.DB.Line line,
                                                                                              Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> pntsAry)
    {
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      Collections.Generic.IList<Revit.DB.XYZ> linePnts = new Collections.Generic.List<Revit.DB.XYZ>();
      linePnts.Add(line.GetEndPoint(0));
      linePnts.Add(line.GetEndPoint(1));

      //  2点の基準に近い方から
      pntsAry = ResetNearBasePoint(line.GetEndPoint(0), pntsAry);

      // 始点が基準に近い方から
      pntsAry = SorttNearBasePoint(line.GetEndPoint(0), pntsAry);

      // 統合
      pntsAry = MergeOverlapPoint(pntsAry);

      // 不使用部分
      pntsAry = NotUsedPoint(linePnts, pntsAry);

      // 最小距離
      foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in pntsAry)
      {
        if (GeomDistance3D(pnts[0], pnts[1]) > _shortCurveTol)
        {
          ret.Add(pnts);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>2点の基準に近い方から</summary>
    /// 
    /// <history>2015/01/20 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ResetNearBasePoint(Revit.DB.XYZ basePnt,
                                                                                          Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> pntsAry)
    {
      var ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in pntsAry)
      {
        Collections.Generic.IList<Revit.DB.XYZ> _Pnts = new Collections.Generic.List<Revit.DB.XYZ>();

        if (GeomDistance3D(basePnt, pnts[0]) <= GeomDistance3D(basePnt, pnts[1]))
        {
          _Pnts.Add(pnts[0]);
          _Pnts.Add(pnts[1]);
        }
        else
        {
          _Pnts.Add(pnts[1]);
          _Pnts.Add(pnts[0]);
        }

        ret.Add(_Pnts);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>始点が基準に近い方から並び替え</summary>
    /// 
    /// <history>2015/01/20 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> SorttNearBasePoint(Revit.DB.XYZ basePnt,
                                                                                          Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> pntsAry)
    {
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      //var usedNum = new Collections.Generic.List<int>();

      //Collections.Generic.IList<Revit.DB.XYZ> ary0 = new Collections.Generic.List<Revit.DB.XYZ>();

      //for (int i = 0; i < pntsAry.Count; ++i)
      //{
      //  if (usedNum.Contains(i))
      //  {
      //    continue;
      //  }

      //  ary0 = pntsAry[i];

      //  int num = 0;

      //  for (int j = 0; j < pntsAry.Count; ++j)
      //  {
      //    if (i == j)
      //    {
      //      continue;
      //    }

      //    if (GeomDistance3D(basePnt, ary0[0]) > GeomDistance3D(basePnt, pntsAry[j][0]))
      //    {
      //      ary0 = pntsAry[j];
      //      num = j;
      //    }
      //  }

      //  usedNum.Add(num);

      //  ret.Add(ary0);
      //}



      ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in pntsAry)
      {
        bool isIn = false;

        for (int i = 0; i < ret.Count; ++i)
        {
          Collections.Generic.IList<Revit.DB.XYZ> ary = ret[i];

          if (GeomDistance3D(basePnt, pnts[0]) < GeomDistance3D(basePnt, ary[0]))
          {
            ret.Insert(i, pnts);

            isIn = true;

            break;
          }
        }

        if (isIn == false)
        {
          ret.Add(pnts);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>統合</summary>
    /// 
    /// <history><p>2015/01/20 Created GSA, Inc. Ryo Kuroda</p>
    ///           <p>2015/01/26 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> MergeOverlapPoint(Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> pntsAry)
    {
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      // 使用番号
      Collections.Generic.IList<int> usedNum = new Collections.Generic.List<int>();

      for (int i = 0; i < pntsAry.Count; ++i)
      {
        Collections.Generic.IList<Revit.DB.XYZ> ary0 = pntsAry[i];

        if (usedNum.Contains(i))
        {
          continue;
        }
        usedNum.Add(i);

        for (int j = 0; j < pntsAry.Count; ++j)
        {
          if (i == j)
          {
            continue;
          }

          if (usedNum.Contains(j))
          {
            continue;
          }

          Collections.Generic.IList<Revit.DB.XYZ> ary1 = pntsAry[j];

          if (GeomDistance3D(ary0[0], ary0[1]) > GeomDistance3D(ary0[0], ary1[0]))
          {
            ary0[1] = ary1[1];

            usedNum.Add(j);
          }
        }

        ret.Add(ary0);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>不使用部分</summary>
    /// 
    /// <history><p>2015/01/20 Created GSA, Inc. Ryo Kuroda</p>
    ///           <p>2015/01/26 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> NotUsedPoint(Collections.Generic.IList<Revit.DB.XYZ> basePnts,
                                                                                    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> usedPnts)
    {
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      if (usedPnts.Count == 0)
      {
        ret.Add(basePnts);

        return ret;
      }

      for (int i = 0; i < usedPnts.Count; ++i)
      {
        Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>();

        if (i == 0)
        {
          pnts.Add(basePnts[0]);
          pnts.Add(usedPnts[i][0]);

          if (GeomDistance3D(pnts[0], pnts[1]) > _shortCurveTol)
          {
            ret.Add(pnts);
          }
        }
        else
        {
          pnts.Add(usedPnts[i - 1][1]);
          pnts.Add(usedPnts[i][0]);

          if (GeomDistance3D(pnts[0], pnts[1]) > _shortCurveTol)
          {
            ret.Add(pnts);
          }


        }

        // 
        if (i == usedPnts.Count - 1)
        {
          pnts = new Collections.Generic.List<Revit.DB.XYZ>();

          pnts.Add(usedPnts[i][1]);
          pnts.Add(basePnts[1]);

          if (GeomDistance3D(pnts[0], pnts[1]) > _shortCurveTol)
          {
            ret.Add(pnts);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>線分領域内の線分取得</summary>
    /// 
    /// <history>2014/11/13 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetLineEndPointInLinesArea(Collections.Generic.IList<Revit.DB.XYZ> linePoints,
                                                                       Collections.Generic.IList<Revit.DB.Curve> lines,
                                                                       Revit.DB.XYZ inAreaPoint)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      for (int i = 0; i < linePoints.Count; ++i)
      {
        if (i == linePoints.Count - 1)
        {
          break;
        }

        Revit.DB.XYZ p0 = linePoints[i];
        Revit.DB.XYZ p1 = linePoints[i + 1];

        if (_shortCurveTol < GeomDistance3D(p0, p1))
        {
          Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);

          Revit.DB.Line inLinesArea = GetLineInLinesArea(line, lines, inAreaPoint);

          if (inLinesArea != null)
          {
            ret.Add(inLinesArea.GetEndPoint(0));
            ret.Add(inLinesArea.GetEndPoint(1));
          }
        }
      }

      if (ret.Count > 1)
      {
        ret = SameXYPointRemove(ret);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>線分領域内の線分取得</summary>
    /// 
    /// <history>2014/11/13 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line GetLineInLinesArea(Revit.DB.Line line,
                                     Collections.Generic.IList<Revit.DB.Curve> lines,
                                     Revit.DB.XYZ inAreaPoint)
    {
      Revit.DB.Line ret = null;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 端点と領域内点を結ぶ線分
      Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, inAreaPoint);
      Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, inAreaPoint);

      bool isOut0 = IsOutCurves(l0, lines);
      bool isOut1 = IsOutCurves(l1, lines);

      // 完全内部
      if (isOut0 == false && isOut1 == false)
      {
        return line;
      }
      // 完全外部
      else if (isOut0 && isOut1)
      {
        return ret;
      }
      // 一部内部
      else
      {
        foreach (Revit.DB.Curve crv in lines)
        {
          Revit.DB.Line l = crv as Revit.DB.Line;

          Revit.DB.XYZ cross = CrossPointXY(line, l, 0);

          if (cross != null)
          {
            if (isOut0 == false)
            {
              if (GeomDistance2D(p0, cross) > _shortCurveTol)
              {
                ret = Revit.DB.Line.CreateBound(p0, cross);

                break;
              }
            }
            else if (isOut1 == false)
            {
              if (GeomDistance2D(p1, cross) > _shortCurveTol)
              {
                ret = Revit.DB.Line.CreateBound(cross, p1);

                break;
              }
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ソート座標</summary>
    /// 
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SortNearPoints(Collections.Generic.IList<Revit.DB.XYZ> pnts,
                                                           Revit.DB.XYZ pnt)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Revit.DB.XYZ pnt1 = null;
      Revit.DB.XYZ pnt2 = null;
      MostDifferentXYPoints(pnts, ref pnt1, ref pnt2);

      Revit.DB.XYZ startPnt = null;

      if (GeomDistance2D(pnt, pnt1) < GeomDistance2D(pnt, pnt2))
      {
        startPnt = pnt1;
      }
      else
      {
        startPnt = pnt2;
      }


      Collections.Generic.IList<Revit.DB.XYZ> _Pnts = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.XYZ p in pnts)
      {
        _Pnts.Add(p);
      }


      while (ret.Count < pnts.Count)
      {
        Revit.DB.XYZ _P = null;

        foreach (Revit.DB.XYZ p in _Pnts)
        {
          if (_P == null)
          {
            _P = p;
            continue;
          }

          if (GeomDistance2D(startPnt, p) < GeomDistance2D(startPnt, _P))
          {
            _P = p;
          }
        }

        ret.Add(_P);
        _Pnts.Remove(_P);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>XY面上で離れた2点</summary>
    /// 
    /// <param name="pnts"></param>
    /// <param name="pnt1"></param>
    /// <param name="pnt2"></param>
    /// 
    /// <hisstory>2014/07/30 Created GSA,Inc. Ryo Kuroda</hisstory>
    /// ================================================================================
    public
    void MostDifferentXYPoints(Collections.Generic.IList<Revit.DB.XYZ> pnts,
                               ref Revit.DB.XYZ pnt1,
                               ref Revit.DB.XYZ pnt2)
    {
      pnt1 = null;
      pnt2 = null;

      foreach (Revit.DB.XYZ p1 in pnts)
      {
        foreach (Revit.DB.XYZ p2 in pnts)
        {
          double dis = GeomDistance2D(p1, p2);

          if (ToHalfAdjust(dis, -9) > 0)
          {
            if (pnt1 == null && pnt2 == null)
            {
              pnt1 = p1;
              pnt2 = p2;
            }
            else
            {
              if (GeomDistance2D(pnt1, pnt2) < GeomDistance2D(p1, p2))
              {
                pnt1 = p1;
                pnt2 = p2;
              }
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>XY面上で同一点の除外</summary>
    /// 
    /// <param name="sorted">ソート済み点</param>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/08/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> SameXYPointRemove(Collections.Generic.IList<Revit.DB.XYZ> sorted)
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      foreach (Revit.DB.XYZ p in sorted)
      {
        bool isNear = false;

        foreach (Revit.DB.XYZ pnt in ret)
        {
          // 線分作成不可長さ
          if(_shortCurveTol > GeomDistance2D(p,pnt))
          {
            isNear = true;

            break;
          }
        }

        if (isNear == false)
        {
          ret.Add(p);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>縦管の最大最小高さ座標</summary>
    /// 
    /// <param name="pipes" >配管</param>
    /// <param name="max"   >最大</param>
    /// <param name="min"   >最小</param>
    /// 
    /// <history>2014/09/24 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetVerticalPipesTopBtm(Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipes,
                                ref double max,
                                ref double min,
                                Revit.DB.XYZ pnt1,
                                Revit.DB.XYZ pnt2)
    {
      for (int i = 0; i < pipes.Count; ++i)
      {
        // 配管
        Revit.DB.Plumbing.Pipe pipe = pipes[i];

        // 線分
        Revit.DB.Line line = GetPipeLine(pipe);

        // 端点
        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (i == 0)
        {
          max = p0.Z;
          min = p0.Z;

          if (max < p1.Z)
          {
            max = p1.Z;
          }
          if (min > p1.Z)
          {
            min = p1.Z;
          }
        }
        else
        {
          if (max < p0.Z)
          {
            max = p0.Z;
          }
          if (min > p0.Z)
          {
            min = p0.Z;
          }

          if (max < p1.Z)
          {
            max = p1.Z;
          }
          if (min > p1.Z)
          {
            min = p1.Z;
          }
        }
      }

      if (pnt1 != null)
      {
        if (max < pnt1.Z)
        {
          max = pnt1.Z;
        }
        if (min > pnt1.Z)
        {
          min = pnt1.Z;
        }
      }
      if (pnt2 != null)
      {
        if (max < pnt2.Z)
        {
          max = pnt2.Z;
        }
        if (min > pnt2.Z)
        {
          min = pnt2.Z;
        }
      }
    }

    /// ================================================================================
    /// <summary>縦ダクト最大最小高さ座標</summary>
    /// 
    /// <param name="ducts" >配管</param>
    /// <param name="max"   >最大</param>
    /// <param name="min"   >最小</param>
    /// 
    /// <history>2014/09/24 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetVerticalDuctTopBbtm(Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts,
                                ref double max,
                                ref double min,
                                Revit.DB.XYZ pnt1,
                                Revit.DB.XYZ pnt2)
    {
      for (int i = 0; i < ducts.Count; ++i)
      {
        // ダクト
        Revit.DB.Mechanical.Duct duct = ducts[i];

        // 線分
        Revit.DB.Line line = GetDuctLine(duct);

        // 端点
        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (i == 0)
        {
          max = p0.Z;
          min = p0.Z;

          if (max < p1.Z)
          {
            max = p1.Z;
          }
          if (min > p1.Z)
          {
            min = p1.Z;
          }
        }
        else
        {
          if (max < p0.Z)
          {
            max = p0.Z;
          }
          if (min > p0.Z)
          {
            min = p0.Z;
          }

          if (max < p1.Z)
          {
            max = p1.Z;
          }
          if (min > p1.Z)
          {
            min = p1.Z;
          }
        }
      }

      if (pnt1 != null)
      {
        if (max < pnt1.Z)
        {
          max = pnt1.Z;
        }
        if (min > pnt1.Z)
        {
          min = pnt1.Z;
        }
      }
      if (pnt2 != null)
      {
        if (max < pnt2.Z)
        {
          max = pnt2.Z;
        }
        if (min > pnt2.Z)
        {
          min = pnt2.Z;
        }
      }
    }

    /// ================================================================================
    /// <summary>詳細線分用配管作成</summary>
    /// 
    /// <param name="pipeLinePnts">配管線分端点</param>
    /// <param name="elevation"   >高さ</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> CreatePipeDetailLines(Collections.Generic.IList<Revit.DB.XYZ> pipeLinePnts,
                                                                   double elevation)
    {
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Revit.DB.XYZ center1 = null;
      Revit.DB.XYZ center2 = null;

      // 線が書けない
      if (pipeLinePnts.Count < 2)
      {
        return ret;
      }
      else if (pipeLinePnts.Count >= 2)
      {
        for (int i = 0; i < pipeLinePnts.Count; ++i)
        {
          if (i == pipeLinePnts.Count - 1)
          {
            break;
          }

          Revit.DB.XYZ p0 = pipeLinePnts[i];
          Revit.DB.XYZ p1 = pipeLinePnts[i + 1];

          if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                         new Revit.DB.XYZ(p1.X, p1.Y, elevation)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                        new Revit.DB.XYZ(p1.X, p1.Y, elevation));
            ret.Add(l);
          }
          else
          {
            ret.Add(null);
          }
        }

        return ret;
      }

      if (pipeLinePnts.Count == 2)
      {
        Revit.DB.XYZ p0 = pipeLinePnts[0];
        Revit.DB.XYZ p1 = pipeLinePnts[1];

        Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                    new Revit.DB.XYZ(p1.X, p1.Y, elevation));
        ret.Add(l);
        return ret;
      }

      for (int i = 0; i < pipeLinePnts.Count; ++i)
      {
        // 初回
        if (center1 == null)
        {
          Revit.DB.XYZ p0 = pipeLinePnts[i];
          Revit.DB.XYZ p1 = pipeLinePnts[i + 1];
          Revit.DB.XYZ p2 = pipeLinePnts[i + 2];
          center2 = Center2Point(p1, p2);

          if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                      new Revit.DB.XYZ(center2.X, center2.Y, elevation)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                        new Revit.DB.XYZ(center2.X, center2.Y, elevation));
            ret.Add(l);

            center1 = center2;
          }
          else
          {
            ret.Add(null);
          }
        }
        else if (center1 != null)// && center2 != null)
        {
          center1 = center2;

          if (i < pipeLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = pipeLinePnts[i + 1];
            Revit.DB.XYZ p2 = pipeLinePnts[i + 2];
            center2 = Center2Point(p1, p2);

            if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                           new Revit.DB.XYZ(center2.X, center2.Y, elevation)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                          new Revit.DB.XYZ(center2.X, center2.Y, elevation));
              ret.Add(l);
            }
            else
            {
              ret.Add(null);
            }
          }
          // 最後(IndexがCount - 2)
          else// if (i == pipeLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = pipeLinePnts[i + 1];

            if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                        new Revit.DB.XYZ(p1.X, p1.Y, elevation)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                          new Revit.DB.XYZ(p1.X, p1.Y, elevation));
              ret.Add(l);
            }
            else
            {
              ret.Add(null);
            }

            break;
          }
        }

        i += 1;

        if (i >= pipeLinePnts.Count - 1)
        {
          break;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>詳細線分用ダクト作成</summary>
    /// 
    /// <param name="ductLinePnts">ダクト線分端点</param>
    /// <param name="elevation"   >高さ</param>
    /// 
    /// <history><p>2014/07/28 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> CreateDuctDetailLines(Collections.Generic.IList<Revit.DB.XYZ> ductLinePnts,
                                                                   double elevation)
    {
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Revit.DB.XYZ center1 = null;
      Revit.DB.XYZ center2 = null;

      if (ductLinePnts.Count < 2)
      {
        return ret;
      }
      else if (ductLinePnts.Count >= 2)
      {
        for (int i = 0; i < ductLinePnts.Count; ++i)
        {
          if (i == ductLinePnts.Count - 1)
          {
            break;
          }

          Revit.DB.XYZ p0 = ductLinePnts[i];
          Revit.DB.XYZ p1 = ductLinePnts[i + 1];

          if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                         new Revit.DB.XYZ(p1.X, p1.Y, elevation)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                        new Revit.DB.XYZ(p1.X, p1.Y, elevation));
            ret.Add(l);
          }
          else
          {
            ret.Add(null);
          }
        }

        return ret;
      }

      if (ductLinePnts.Count == 2)
      {
        Revit.DB.XYZ p0 = ductLinePnts[0];
        Revit.DB.XYZ p1 = ductLinePnts[ductLinePnts.Count - 1];

        Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                    new Revit.DB.XYZ(p1.X, p1.Y, elevation));
        ret.Add(l);
        return ret;
      }

      for (int i = 0; i < ductLinePnts.Count; ++i)
      {
        // 初回
        if (center1 == null)
        {
          Revit.DB.XYZ p0 = ductLinePnts[i];
          Revit.DB.XYZ p1 = ductLinePnts[i + 1];
          Revit.DB.XYZ p2 = ductLinePnts[i + 2];
          center2 = Center2Point(p1, p2);

          if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                         new Revit.DB.XYZ(center2.X, center2.Y, elevation)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, elevation),
                                                        new Revit.DB.XYZ(center2.X, center2.Y, elevation));
            ret.Add(l);

            center1 = center2;
          }
          else
          {
            ret.Add(null);
          }
          //center1 = center2;
        }
        else if (center1 != null)// && center2 != null)
        {
          center1 = center2;

          if (i < ductLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = ductLinePnts[i + 1];
            Revit.DB.XYZ p2 = ductLinePnts[i + 2];
            center2 = Center2Point(p1, p2);

            if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                           new Revit.DB.XYZ(center2.X, center2.Y, elevation)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                          new Revit.DB.XYZ(center2.X, center2.Y, elevation));
              ret.Add(l);
            }
            else
            {
              ret.Add(null);
            }
          }
          else
          {
            Revit.DB.XYZ p1 = ductLinePnts[i + 1];

            if (_shortCurveTol < GeomDistance2D(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                           new Revit.DB.XYZ(p1.X, p1.Y, elevation)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, elevation),
                                                          new Revit.DB.XYZ(p1.X, p1.Y, elevation));
              ret.Add(l);
            }
            else
            {
              ret.Add(null);
            }

            break;
          }
        }

        i += 1;

        if (i >= ductLinePnts.Count - 1)
        {
          break;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管線分作成</summary>
    /// 
    /// <param name="pipeLinePnts">配管線分端点</param>
    /// <param name="elevation"   >高さ</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> CreatePipeLines(Collections.Generic.IList<Revit.DB.XYZ> pipeLinePnts)
    {
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Revit.DB.XYZ center1 = null;
      Revit.DB.XYZ center2 = null;

      // 線が書けない
      if (pipeLinePnts.Count < 2)
      {
        return ret;
      }
      else if (pipeLinePnts.Count >= 2)
      {
        for (int i = 0; i < pipeLinePnts.Count; ++i)
        {
          if (i == pipeLinePnts.Count - 1)
          {
            break;
          }

          Revit.DB.XYZ p0 = pipeLinePnts[i];
          Revit.DB.XYZ p1 = pipeLinePnts[i + 1];

          if (_shortCurveTol < GeomDistance3D(p0, p1))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(p0, p1);
            ret.Add(l);
          }
          else
          {
            ret.Add(null);
          }
        }

        return ret;
      }

      if (pipeLinePnts.Count == 2)
      {
        Revit.DB.XYZ p0 = pipeLinePnts[0];
        Revit.DB.XYZ p1 = pipeLinePnts[1];

        Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                    new Revit.DB.XYZ(p1.X, p1.Y, p1.Z));
        ret.Add(l);
        return ret;
      }

      for (int i = 0; i < pipeLinePnts.Count; ++i)
      {
        // 初回
        if (center1 == null)
        {
          Revit.DB.XYZ p0 = pipeLinePnts[i];
          Revit.DB.XYZ p1 = pipeLinePnts[i + 1];
          Revit.DB.XYZ p2 = pipeLinePnts[i + 2];
          center2 = Center2Point(p1, p2);

          if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                       new Revit.DB.XYZ(center2.X, center2.Y, center2.Z)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                        new Revit.DB.XYZ(center2.X, center2.Y, center2.Z));
            ret.Add(l);

            center1 = center2;
          }

          center1 = center2;
        }
        else if (center1 != null)// && center2 != null)
        {
          center1 = center2;

          if (i < pipeLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = pipeLinePnts[i + 1];
            Revit.DB.XYZ p2 = pipeLinePnts[i + 2];
            center2 = Center2Point(p1, p2);

            if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                         new Revit.DB.XYZ(center2.X, center2.Y, center2.Z)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                          new Revit.DB.XYZ(center2.X, center2.Y, center2.Z));
              ret.Add(l);
            }
          }
          // 最後(IndexがCount - 2)
          else if (i == pipeLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = pipeLinePnts[i + 1];

            if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                         new Revit.DB.XYZ(p1.X, p1.Y, p1.Z)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                          new Revit.DB.XYZ(p1.X, p1.Y, p1.Z));
              ret.Add(l);
            }

            break;
          }
        }

        i += 1;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ダクト線分作成</summary>
    /// 
    /// <param name="ductLinePnts">ダクト線分端点</param>
    /// 
    /// <history><p>2014/07/28 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> CreateDuctLines(Collections.Generic.IList<Revit.DB.XYZ> ductLinePnts)
    {
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Revit.DB.XYZ center1 = null;
      Revit.DB.XYZ center2 = null;

      if (ductLinePnts.Count < 2)
      {
        return ret;
      }
      else if (ductLinePnts.Count >= 2)
      {
        for (int i = 0; i < ductLinePnts.Count; ++i)
        {
          if (i == ductLinePnts.Count - 1)
          {
            break;
          }

          Revit.DB.XYZ p0 = ductLinePnts[i];
          Revit.DB.XYZ p1 = ductLinePnts[i + 1];

          if (_shortCurveTol < GeomDistance3D(p0, p1))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(p0, p1);
            ret.Add(l);
          }
          else
          {
            ret.Add(null);
          }
        }

        return ret;
      }

      if (ductLinePnts.Count == 2)
      {
        Revit.DB.XYZ p0 = ductLinePnts[0];
        Revit.DB.XYZ p1 = ductLinePnts[ductLinePnts.Count - 1];

        Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                    new Revit.DB.XYZ(p1.X, p1.Y, p1.Z));
        ret.Add(l);
        return ret;
      }

      for (int i = 0; i < ductLinePnts.Count; ++i)
      {
        // 初回
        if (center1 == null)
        {
          Revit.DB.XYZ p0 = ductLinePnts[i];
          Revit.DB.XYZ p1 = ductLinePnts[i + 1];
          Revit.DB.XYZ p2 = ductLinePnts[i + 2];
          center2 = Center2Point(p1, p2);

          if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                       new Revit.DB.XYZ(center2.X, center2.Y, center2.Z)))
          {
            Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(p0.X, p0.Y, p0.Z),
                                                        new Revit.DB.XYZ(center2.X, center2.Y, center2.Z));
            ret.Add(l);

            center1 = center2;
          }

          center1 = center2;
        }
        else if (center1 != null)// && center2 != null)
        {
          center1 = center2;

          if (i < ductLinePnts.Count - 2)
          {
            Revit.DB.XYZ p1 = ductLinePnts[i + 1];
            Revit.DB.XYZ p2 = ductLinePnts[i + 2];
            center2 = Center2Point(p1, p2);

            if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                         new Revit.DB.XYZ(center2.X, center2.Y, center2.Z)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                          new Revit.DB.XYZ(center2.X, center2.Y, center2.Z));
              ret.Add(l);
            }
          }
          else
          {
            Revit.DB.XYZ p1 = ductLinePnts[i + 1];

            if (_shortCurveTol < GeomDistance3D(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                         new Revit.DB.XYZ(p1.X, p1.Y, p1.Z)))
            {
              Revit.DB.Line l = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(center1.X, center1.Y, center1.Z),
                                                          new Revit.DB.XYZ(p1.X, p1.Y, p1.Z));
              ret.Add(l);
            }

            break;
          }
        }

        i += 1;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管の始終端線分</summary>
    /// 
    /// <param name="pipe">配管</param>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line GetPipeLine(Revit.DB.Plumbing.Pipe pipe)
    {
      // 戻り値
      Revit.DB.Line ret = null;

      if (pipe == null)
      {
        return ret;
      }

      Revit.DB.LocationCurve locCrv = pipe.Location as Revit.DB.LocationCurve;
      Revit.DB.Curve crv = locCrv.Curve;

      if (crv.IsCyclic == false)
      {
        ret = crv as Revit.DB.Line;
      }
      // 円弧の場合
      else
      {
        ret = Revit.DB.Line.CreateBound(crv.GetEndPoint(0), crv.GetEndPoint(1));
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ダクトの始終端線分</summary>
    /// 
    /// <param name="duct">ダクト</param>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line GetDuctLine(Revit.DB.Mechanical.Duct duct)
    {
      // 戻り値
      Revit.DB.Line ret = null;

      if (duct == null)
      {
        return ret;
      }

      Revit.DB.LocationCurve locCrv = duct.Location as Revit.DB.LocationCurve;
      Revit.DB.Curve crv = locCrv.Curve;

      if (crv.IsCyclic == false)
      {
        ret = crv as Revit.DB.Line;
      }
      // 円弧の場合
      else
      {
        ret = Revit.DB.Line.CreateBound(crv.GetEndPoint(0), crv.GetEndPoint(1));
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>縦配管判定</summary>
    /// 
    /// <param name="pipe">配管</param>
    /// 
    /// <history>2014/08/08 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsVerticalSinglePipe(Revit.DB.Plumbing.Pipe pipe)
    {
      bool ret = true;

      Revit.DB.Line line = GetPipeLine(pipe);

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // XY成分の差分 1cm単位
      double distance2d = ToHalfAdjust(GeomDistance2D(p0, p1) * 304.8 / 1000, -2);

      if (distance2d > 0)
      {
        ret = false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>縦ダクト判定</summary>
    /// 
    /// <param name="duct">ダクト</param>
    /// 
    /// <history>2014/09/18 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsVerticalSingleDuct(Revit.DB.Mechanical.Duct duct)
    {
      bool ret = true;

      Revit.DB.Line line = GetDuctLine(duct);

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // XY成分の差分 1cm単位
      double distance2d = ToHalfAdjust(GeomDistance2D(p0, p1) * 304.8 / 1000, -2);

      if (distance2d > 0)
      {
        ret = false;
      }

      return ret;
    }
    
    /// ================================================================================
    /// <summary>2点のラジアン角</summary>
    /// 
    /// <param name="start" >始点</param>
    /// <param name="end"   >終点</param>
    /// 
    /// <history>2014/08/21 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    double
    GetRadian(Revit.DB.XYZ start,
              Revit.DB.XYZ end)
    {
      double ret = 0;

      ret = System.Math.Atan2(end.Y - start.Y,
                              end.X - start.X);

      return ret;
    }

    /// ================================================================================
    /// <summary>フレキシブルダクト 単位メートル長さ分割</summary>
    /// 
    /// <param name="flexDuct">フレキシブルダクト</param>
    /// 
    /// <history>2016/02/12 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> DivideMeterLength(Revit.DB.Mechanical.FlexDuct flexDuct)
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      Revit.DB.LocationCurve locCrv = flexDuct.Location as Revit.DB.LocationCurve;
      Revit.DB.Curve crv = locCrv.Curve;

      Revit.DB.XYZ p0 = crv.GetEndPoint(0);

      // 長さ
      double length = crv.Length;
      // メートル化
      double lengthMeter = length * 304.8 / 1000;

      // 単位メートル
      double unitMeter = 1;

      // 切り上げ
      lengthMeter = System.Math.Ceiling(lengthMeter / unitMeter);

      Revit.DB.XYZ p1 = p0;
      Revit.DB.XYZ p2 = null;

      double unitLength = unitMeter * 1000 / 304.8;

      for (int i = 0; i < lengthMeter; ++i)
      {
        Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>();

        p2 = p1 + new Revit.DB.XYZ(unitLength, 0, 0);

        pnts.Add(p1);
        pnts.Add(p2);

        ret.Add(pnts);

        p1 = p2;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>メートル化</summary>
    /// 
    /// <param name="feetVal" >フィート値</param>
    /// <param name="digits"  >表示桁</param>
    /// 
    /// <history>2014/11/14 Created GSA, Inc. Ryo Kurods</history>
    /// ================================================================================
    public
    double ToMetricFromFeet(double feetVal, int digits)
    {
      double ret = 0;

      // mm単位
      double milliMetric = feetVal * 304.8;

      // 整数切り上げ
      milliMetric = System.Math.Ceiling(milliMetric);

      // m単位
      double metric = milliMetric / 1000;

      // 四捨五入
      ret = ToHalfAdjust(metric, digits);

      return ret;
    }

    /// ================================================================================
    /// <summary>ミリメートル化</summary>
    /// 
    /// <param name="feetVal" >フィート値</param>
    /// <param name="digits"  >表示桁</param>
    /// 
    /// <history>2017/07/21 Created CST,Co.Ltd. Ryo Kurods</history>
    /// ================================================================================
    public
    double ToMilliMetricFromFeet(double feetVal, int digits)
    {
      double ret = 0;

      // mm単位
      double milliMetric = feetVal * 304.8;
      
      // 四捨五入
      ret = ToHalfAdjust(milliMetric, digits);

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

      return value > 0 ? System.Math.Floor((value * dCoef) + 0.5) / dCoef :
                         System.Math.Ceiling((value * dCoef) - 0.5) / dCoef;
    }

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>スペース境界</summary>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>> SpaceBndryCrv
    {
      get
      {
        return _SpaceBndryCrv;
      }
    }

    #endregion
  }
}
