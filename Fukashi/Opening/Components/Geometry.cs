using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Components
{
  /// ================================================================================
  /// <summary>図形</summary>
  /// ================================================================================
  public partial class Geometry
  {
    // メンバ変数
    #region Member Variables

    /// <summary>頂点</summary>
    private Collections.Generic.IList<Revit.DB.XYZ> _Pnts;

    /// <summary>接点誤差</summary>
    private double _ToleranceInter;

    /// <summary>面</summary>
    private Revit.DB.Plane _Plane;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>要素</summary>
    ///
    /// <param name="rvtUiDoc">UIドキュメント</param>
    ///
    /// <history>2016/11/17 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public Geometry(Revit.UI.UIDocument rvtUiDoc)
    {
      RvtUIDoc = rvtUiDoc ?? throw new System.ArgumentNullException(nameof(rvtUiDoc));
      _Pnts = new Collections.Generic.List<Revit.DB.XYZ>();

      _ToleranceInter = Approx0Len;
    }
    #endregion

    // メンバ関数
    #region Member Functions
      
    /// ================================================================================
    /// <summary>カーブ形状判定</summary>
    /// 
    /// <param name="curves">カーブ</param>
    /// 
    /// <returns><p>1 = 三角形</p>
    ///           <p>2 = 台形</p>
    ///           <p>3 = 正方形</p>
    ///           <p>4 = 長方形</p>
    ///           <p>5 = ひし形</p>
    ///           <p>6 = 平行四辺形</p>
    ///           <p>7 = L字形</p>
    ///           <p>8 = 凸形</p>
    ///           <p>9 = 凹型</p>
    ///           <p>10 = その他</p></returns>
    /// 
    /// <history>2016/12/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    int CurvesGeometryMode(Collections.Generic.IList<Revit.DB.Curve> curves)
    {
      // 戻り値
      int ret = 0;
      
      // 曲線を含むか
      bool isCyclic = false;

      Collections.Generic.IList<Revit.DB.Curve> _Curves = new Collections.Generic.List<Revit.DB.Curve>();

      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);

        if (curve.IsCyclic)
        {
          isCyclic = true;
        }
      }

      if (isCyclic)
      {
        // その他
        ret = 10;
        return ret;
      }

      // 調整
      _Curves = OptimizeLineVertexNoConvLine(_Curves);

      // 三角形
      if (_Curves.Count == 3)
      {
        ret = 1;
      }
      // 四角形
      else if (_Curves.Count == 4)
      {
        #region 四角形

        // 対角線
        Collections.Generic.IList<Revit.DB.Line> diagonals = GetDiagonal(_Curves);

        if (diagonals.Count == 2)
        {
          Revit.DB.Line line1 = diagonals[0];
          Revit.DB.Line line2 = diagonals[1];

          // 対角線の交点
          Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();
          IntersecCurve(line1, line2, ref intersects);

          if (intersects.Count == 1)
          {
            Revit.DB.XYZ p0 = line1.GetEndPoint(0);
            Revit.DB.XYZ p1 = line1.GetEndPoint(1);
            Revit.DB.XYZ p2 = line2.GetEndPoint(0);
            Revit.DB.XYZ p3 = line2.GetEndPoint(1);

            Revit.DB.XYZ intersect = intersects[0];

            // お互いに2等分
            if (System.Math.Abs(Distance(p0, intersect) - Distance(p1, intersect)) < Approx0Len &&
                System.Math.Abs(Distance(p2, intersect) - Distance(p3, intersect)) < Approx0Len)
            {
              // 対角線同士の長さが等しい
              if (System.Math.Abs(line1.Length - line2.Length) < Approx0Len)
              {
                // 交点からのそれぞれの対角線始点への単位ベクトル
                Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
                Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

                double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

                // ラジアン角度
                double rad = System.Math.Acos(cosSita);

                // 直角に交差
                if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                {
                  // 正方形
                  ret = 3;
                }
                // 直角に交差ではない
                else
                {
                  // 長方形
                  ret = 4;
                }
              }
              else
              {
                // 交点からのそれぞれの対角線始点への単位ベクトル
                Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
                Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

                double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

                // ラジアン角度
                double rad = System.Math.Acos(cosSita);

                // 直角に交差
                if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                {
                  // ひし形
                  ret = 5;
                }
                // 直角に交差ではない
                else
                {
                  // 平行四辺形
                  ret = 6;
                }
              }
            }
            // 2等分ではない
            else
            {
              // どちらかの対辺が平行
              Revit.DB.Line l0 = _Curves[0] as Revit.DB.Line;
              Revit.DB.Line l1 = _Curves[1] as Revit.DB.Line;
              Revit.DB.Line l2 = _Curves[2] as Revit.DB.Line;
              Revit.DB.Line l3 = _Curves[3] as Revit.DB.Line;

              if (IsParallelLine(l0, l2) ||
                  IsParallelLine(l1, l3))
              {
                // 台形
                ret = 2;
              }
            }
          }
        }

        #endregion
      }
      // 六角形
      else if (_Curves.Count == 6)
      {
        #region 六角形

        bool isRect = true;

        for (int i = 0; i < _Curves.Count; ++i)
        {
          Revit.DB.Curve curve1 = _Curves[i];
          Revit.DB.Curve curve2 = null;

          if (i == _Curves.Count - 1)
          {
            curve2 = _Curves[0];
          }
          else
          {
            curve2 = _Curves[i + 1];
          }

          Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
          Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
          Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
          Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

          Revit.DB.XYZ uv1 = UnitVector(p0, p1);
          Revit.DB.XYZ uv2 = UnitVector(p2, p3);

          double cosSita = CosSita(uv1, uv2);

          // ラジアン角度
          double rad = System.Math.Acos(cosSita);

          // 頂点が直角
          if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
          {
            isRect = true;
          }
          else
          {
            isRect = false;
            break;
          }
        }

        // すべての頂点が直角
        if (isRect)
        {
          // L字形
          ret = 7;
        }

        #endregion
      }
      // 八角形
      else if (_Curves.Count == 8)
      {
        #region 八角形

        bool isRect = true;

        for (int i = 0; i < _Curves.Count; ++i)
        {
          Revit.DB.Curve curve1 = _Curves[i];
          Revit.DB.Curve curve2 = null;

          if (i == _Curves.Count - 1)
          {
            curve2 = _Curves[0];
          }
          else
          {
            curve2 = _Curves[i + 1];
          }

          Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
          Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
          Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
          Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

          Revit.DB.XYZ uv1 = UnitVector(p0, p1);
          Revit.DB.XYZ uv2 = UnitVector(p2, p3);

          double cosSita = CosSita(uv1, uv2);

          // ラジアン角度
          double rad = System.Math.Acos(cosSita);

          // 頂点が直角
          if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
          {
            isRect = true;
          }
          else
          {
            isRect = false;
            break;
          }
        }

        // すべての頂点が直角
        if (isRect)
        {
          // 凸凹並び順(正負で判定)
          Collections.Generic.IList<Collections.Generic.IList<int>> unevenness = new Collections.Generic.List<Collections.Generic.IList<int>>();

          for (int i = 0; i < _Curves.Count; ++i)
          {
            Revit.DB.Curve curve1 = _Curves[i];
            Revit.DB.Curve curve2 = null;

            if (i == _Curves.Count - 1)
            {
              curve2 = _Curves[0];
            }
            else
            {
              curve2 = _Curves[i + 1];
            }

            Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
            Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
            Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
            Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

            Revit.DB.XYZ uv1 = UnitVector(p0, p1);
            Revit.DB.XYZ uv2 = UnitVector(p2, p3);

            Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

            // 符号 正を1、負を0
            // (XY面上であれば、Z軸が1なら凸、-1なら凹)
            Collections.Generic.IList<int> ary = new Collections.Generic.List<int>();
            ary.Add(cross.X > 0 ? 1 : 0);
            ary.Add(cross.Y > 0 ? 1 : 0);
            ary.Add(cross.Z > 0 ? 1 : 0);

            unevenness.Add(ary);
          }

          // 最初の頂点
          // 最初の頂点は凸でも凹でもどちらでもいい
          Collections.Generic.IList<int> ary0 = unevenness[0];

          // 最初の頂点と同一の凸凹か
          Collections.Generic.IList<int> sameAry = new Collections.Generic.List<int>();

          int sameCount = 0;
          int diffeCount = 0;

          foreach (Collections.Generic.IList<int> ary1 in unevenness)
          {
            if (ary0[0] == ary1[0] &&
                ary0[1] == ary1[1] &&
                ary0[2] == ary1[2])
            {
              sameAry.Add(1);
              sameCount += 1;
            }
            else
            {
              sameAry.Add(0);
              diffeCount += 1;
            }
          }

          string order = "";
          foreach (int same in sameAry)
          {
            order += same.ToString();
          }
          foreach (int same in sameAry)
          {
            order += same.ToString();
          }

          // 凸形は凸の頂点が6個、凹の頂点が2個で
          // 頂点の並び順が凸凸凸凸凹凸凸凹
          if (sameCount == 6 &&
              diffeCount == 2)
          {
            // 多い方を1、少ない方を0
            if (order.Contains("11110110"))
            {
              // 凸形
              ret = 8;
            }
          }
          else if (sameCount == 2 &&
                   diffeCount == 6)
          {
            // 多い方を0、少ない方を1
            if (order.Contains("00001001"))
            {
              // 凸形
              ret = 8;
            }
          }

          // 凹形は凸の頂点が6個、凹の頂点が2個で
          // 頂点の並び順が凸凸凸凸凸凸凹凹
          if (sameCount == 6 &&
              diffeCount == 2)
          {
            // 多い方を1、少ない方を0
            if (order.Contains("11111100"))
            {
              // 凹形
              ret = 9;
            }
          }
          else if (sameCount == 2 &&
                   diffeCount == 6)
          {
            // 多い方を0、少ない方を1
            if (order.Contains("00000011"))
            {
              // 凹形
              ret = 9;
            }
          }
        }

        #endregion
      }

      // 該当なし
      if (ret == 0)
      {
        // その他
        ret = 10;
      }

      return ret;
    }


    /// ================================================================================
    /// <summary>平行判定</summary>
    /// 
    /// <param name="line1">線分1</param>
    /// <param name="line2">線分2</param>
    /// 
    /// <history>2016/12/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsParallelLine(Revit.DB.Line line1,
                        Revit.DB.Line line2)
    {
      // 戻り値
      bool ret = false;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      Revit.DB.XYZ uv1 = UnitVector(p1, p0);
      Revit.DB.XYZ uv2 = UnitVector(p3, p2);

      double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

      // ラジアン角度
      double rad = System.Math.Acos(cosSita);
      
      // 0か180度
      if (System.Math.Abs(rad) < Approx0Ang ||
          System.Math.Abs(System.Math.Abs(rad) - System.Math.PI) < 0.001)
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>対角線取得</summary>
    /// 
    /// <param name="polygonCurves">多角形線分</param>
    /// 
    /// <history>2016/12/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> GetDiagonal(Collections.Generic.IList<Revit.DB.Curve> polygonCurves)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
      foreach (Revit.DB.Curve curve in polygonCurves)
      {
        Revit.DB.XYZ pos = curve.GetEndPoint(0);
        posAry.Add(pos);
      }

      for (int i = 0; i < posAry.Count; ++i)
      {
        // 最後の点
        if (i == posAry.Count - 1)
        {
          continue;
        }

        Revit.DB.XYZ p0 = posAry[i];

        for (int j = 0; j < posAry.Count; ++j)
        {
          // 先の点に限定
          if (i + 1 >= j)
          {
            continue;
          }
          // 例外 最初の点
          if (i == 0)
          {
            // 最後の点が前の点
            if (j == posAry.Count - 1)
            {
              continue;
            }
          }

          Revit.DB.XYZ p1 = posAry[j];

          if (Distance(p0, p1) > Approx0Len)
          {
            Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);
            ret.Add(line);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>L形ファミリ始点に合わせたソート</summary>
    /// 
    /// <param name="curves">カーブ</param>
    /// 
    /// <history>2016/12/26 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> SortLshapeCurves(Collections.Generic.IList<Revit.DB.Curve> curves)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();
      
      int concave = 0;

      for (int i = 0; i < curves.Count; ++i)
      {
        Revit.DB.Curve curve1 = curves[i];
        Revit.DB.Curve curve2 = null;

        if (i == curves.Count - 1)
        {
          curve2 = curves[0];
        }
        else
        {
          curve2 = curves[i + 1];
        }

        Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

        // XY面上の左回りであれば、Z軸が1なら凸、-1なら凹
        if (cross.Z < 0)
        {
          concave = i;
          break;
        }
      }

      Collections.Generic.IList<Revit.DB.Curve> _Curves = new Collections.Generic.List<Revit.DB.Curve>();
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }

      for (int i = 0; i < _Curves.Count; ++i)
      {
        if (i > concave + 4)
        {
          if (i > concave + 10)
          {
            break;
          }

          Revit.DB.Curve curve = _Curves[i];
          ret.Add(curve);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>凸形ファミリ始点に合わせたソート</summary>
    /// 
    /// <param name="curves">カーブ</param>
    /// 
    /// <history>2016/12/26 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> SortConvexCurves(Collections.Generic.IList<Revit.DB.Curve> curves)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      Collections.Generic.IList<int> unevenness = new Collections.Generic.List<int>();
      
      for (int i = 0; i < curves.Count; ++i)
      {
        Revit.DB.Curve curve1 = curves[i];
        Revit.DB.Curve curve2 = null;

        if (i == curves.Count - 1)
        {
          curve2 = curves[0];
        }
        else
        {
          curve2 = curves[i + 1];
        }

        Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

        // XY面上の左回りであれば、Z軸が1なら凸、-1なら凹
        unevenness.Add(cross.Z > 0 ? 1 : 0);
      }

      int concave = 0;

      string order = "";

      foreach (int val in unevenness)
      {
        order += val.ToString();

        if (order.Contains("0110"))
        {
          break;
        }

        concave += 1;
      }
      if (order.Contains("0110") == false)
      {
        foreach (int val in unevenness)
        {
          order += val.ToString();

          if (order.Contains("0110"))
          {
            break;
          }

          concave += 1;
        }
      }

      Collections.Generic.IList<Revit.DB.Curve> _Curves = new Collections.Generic.List<Revit.DB.Curve>();
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }

      for (int i = 0; i < _Curves.Count; ++i)
      {
        if (i > concave + 2)
        {
          if (i > concave + 10)
          {
            break;
          }

          Revit.DB.Curve curve = _Curves[i];
          ret.Add(curve);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>凹形ファミリ始点に合わせたソート</summary>
    /// 
    /// <param name="curves">カーブ</param>
    /// 
    /// <history>2017/01/11 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> SortConcaveCurves(Collections.Generic.IList<Revit.DB.Curve> curves)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      Collections.Generic.IList<int> unevenness = new Collections.Generic.List<int>();

      for (int i = 0; i < curves.Count; ++i)
      {
        Revit.DB.Curve curve1 = curves[i];
        Revit.DB.Curve curve2 = null;

        if (i == curves.Count - 1)
        {
          curve2 = curves[0];
        }
        else
        {
          curve2 = curves[i + 1];
        }

        Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

        // XY面上の左回りであれば、Z軸が1なら凸、-1なら凹
        unevenness.Add(cross.Z > 0 ? 1 : 0);
      }

      int convex = 0;

      string order = "";

      foreach (int val in unevenness)
      {
        order += val.ToString();

        if (order.Contains("1001"))
        {
          break;
        }

        convex += 1;
      }
      if (order.Contains("1001") == false)
      {
        foreach (int val in unevenness)
        {
          order += val.ToString();

          if (order.Contains("1001"))
          {
            break;
          }

          convex += 1;
        }
      }

      Collections.Generic.IList<Revit.DB.Curve> _Curves = new Collections.Generic.List<Revit.DB.Curve>();
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }
      foreach (Revit.DB.Curve curve in curves)
      {
        _Curves.Add(curve);
      }

      for (int i = 0; i < _Curves.Count; ++i)
      {
        if (i > convex + 2)
        {
          if (i > convex + 10)
          {
            break;
          }

          Revit.DB.Curve curve = _Curves[i];
          ret.Add(curve);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ファミリインスタンスのソリッド取得</summary>
    /// 
    /// <param name="famIns">ファミリインスタンス</param>
    /// 
    /// <history>2016/12/27 Created CST,Co.Ltd.Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Solid> GetFamInsSolid(Revit.DB.FamilyInstance famIns)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Solid> ret = new Collections.Generic.List<Revit.DB.Solid>();

      Revit.DB.Options opt = RvtDBDoc.Application.Create.NewGeometryOptions();
      opt.View = RvtDBDoc.ActiveView;

      Revit.DB.GeometryElement geomElem = famIns.get_Geometry(opt);

      if (geomElem == null)
      {
        return ret;
      }

      Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
      geoObjEnum.Reset();

      while (geoObjEnum.MoveNext())
      {
        Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
        Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

        if (geoIns != null)
        {
          Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
          Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
          goEnum.Reset();

          while (goEnum.MoveNext())
          {
            Revit.DB.GeometryObject go = goEnum.Current;

            Revit.DB.Solid solid = go as Revit.DB.Solid;

            if (solid != null && solid.Volume > 0)
            {
              ret.Add(solid);
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>エッジカーブ取得</summary>
    /// 
    /// <history><p>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/01/05 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetAllEdgeCurves()
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      // ビュー内の要素
      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc, RvtDBDoc.ActiveView.Id);
      fec.WhereElementIsNotElementType();

      Collections.Generic.IList<Revit.DB.Element> allElemeAry = new Collections.Generic.List<Revit.DB.Element>();

      foreach (Revit.DB.Element elem in fec)
      {
        if (elem.Category != null)
        {
          allElemeAry.Add(elem);
        }
      }

      // ビュー
      Revit.DB.View view = RvtDBDoc.ActiveView;
      Revit.DB.ViewPlan viewPlan = view as Revit.DB.ViewPlan;

      // ビュー範囲
      Revit.DB.PlanViewRange viewRange = viewPlan.GetViewRange();

      // 下部レベル
      Revit.DB.Level btmLevel = null;
      // 切断面レベル
      Revit.DB.Level cutLevel = null;

      Revit.DB.ElementId btmLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.BottomClipPlane);
      Revit.DB.ElementId cutLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.CutPlane);

      if (btmLvlId != null)
      {
        btmLevel = RvtDBDoc.GetElement(btmLvlId) as Revit.DB.Level;
      }
      if (cutLvlId != null)
      {
        cutLevel = RvtDBDoc.GetElement(cutLvlId) as Revit.DB.Level;
      }

      // オフセット
      double btmOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
      double topOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);

      // 面取得
      #region 面取得
      Collections.Generic.IList<Revit.DB.Face> allFaces = new Collections.Generic.List<Revit.DB.Face>();

      Revit.DB.Options opt = RvtDBDoc.Application.Create.NewGeometryOptions();
      //opt.View = RvtDBDoc.ActiveView;

      foreach (Revit.DB.Element elem in allElemeAry)
      {
        Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

        if (geomElem == null)
        {
          continue;
        }

        Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
        geoObjEnum.Reset();

        while (geoObjEnum.MoveNext())
        {
          Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
          Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

          if (geoIns != null)
          {
            Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
            if (famIns != null)
            {
              Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
              Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
              goEnum.Reset();

              while (goEnum.MoveNext())
              {
                Revit.DB.GeometryObject go = goEnum.Current;

                Revit.DB.Solid solid = go as Revit.DB.Solid;

                if (solid != null)
                {
                  foreach (Revit.DB.Face face in solid.Faces)
                  {
                    // 表示判定
                    if (IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                    {
                      Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                      if (pf == null)
                      {
                        continue;
                      }

                      allFaces.Add(face);
                    }
                  }
                }
              }
            }
          }
          else
          {
            Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

            if (solid != null)
            {
              foreach (Revit.DB.Face face in solid.Faces)
              {
                // 表示判定
                if (IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                {
                  Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                  if (pf == null)
                  {
                    continue;
                  }

                  allFaces.Add(face);
                }
              }
            }
          }
        }
      }
      #endregion

      // 作業面
      Revit.DB.SketchPlane sktPln = view.SketchPlane;
      
      if (sktPln != null)
      {
        _Plane = sktPln.GetPlane();
        Revit.DB.XYZ plnOrigin = _Plane.Origin;
        Revit.DB.XYZ xVec = _Plane.XVec;
        Revit.DB.XYZ yVec = _Plane.YVec;

        Revit.DB.XYZ posPln1 = plnOrigin + xVec * 1000000;
        Revit.DB.XYZ posPln2 = plnOrigin + yVec * 1000000;
        Revit.DB.XYZ posPln3 = plnOrigin - xVec * 1000000;
        Revit.DB.XYZ posPln4 = plnOrigin - yVec * 1000000;

        Revit.DB.Line linePln1 = Revit.DB.Line.CreateBound(posPln1, posPln2);
        Revit.DB.Line linePln2 = Revit.DB.Line.CreateBound(posPln2, posPln3);
        Revit.DB.Line linePln3 = Revit.DB.Line.CreateBound(posPln3, posPln4);
        Revit.DB.Line linePln4 = Revit.DB.Line.CreateBound(posPln4, posPln1);

        Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();
        crvLoop.Append(linePln1);
        crvLoop.Append(linePln2);
        crvLoop.Append(linePln3);
        crvLoop.Append(linePln4);

        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
        crvLoops.Add(crvLoop);

        // 面ソリッド
        Revit.DB.Solid plnSolid = null;

        try
        {
          // ビューと逆向きに押し出し作成
          plnSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops, -view.ViewDirection, Approx0Len);
        }
        catch
        {
          return ret;
        }

        Revit.DB.Face orgFace = null;
        foreach (Revit.DB.Face plnFace in plnSolid.Faces)
        {
          Revit.DB.PlanarFace pf = plnFace as Revit.DB.PlanarFace;

          // ビューと同じ向きの面
          if (ToHalfAdjust(pf.FaceNormal.Z, -9) == ToHalfAdjust(view.ViewDirection.Z, -9))
          {
            orgFace = plnFace;
          }
        }

        foreach (Revit.DB.Face face in allFaces)
        {
          try
          {
            Revit.DB.Curve intersectCrv = null;

            // 面と面の交差カーブ
            if (orgFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
            {
              if (intersectCrv.IsCyclic == false)
              {
                ret.Add(intersectCrv);
              }
              else
              {
                Collections.Generic.IList<Revit.DB.Line> convertLines = CurveConvertToLine(intersectCrv);

                foreach (Revit.DB.Line line in convertLines)
                {
                  ret.Add(line);
                }
              }
            }
          }
          catch
          {
            continue;
          }

        }
      }
      
      return ret;
    }

    /// ================================================================================
    /// <summary>面の表示判定</summary>
    /// 
    /// <param name="face"        >面</param>
    /// <param name="bottomLevel" >下部レベル</param>
    /// <param name="topLevel"    >上部レベル</param>
    /// <param name="bottomOffset">下部オフセット</param>
    /// <param name="topOffset"   >上部オフセット</param>
    /// 
    /// <history><p>2016/12/28 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/01/10 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool IsVisibleFaceInHeight(Revit.DB.Face face,
                               Revit.DB.Level bottomLevel,
                               Revit.DB.Level topLevel,
                               double bottomOffset,
                               double topOffset)
    {
      // 戻り値
      bool ret = false;

      double btmHeight = 0;
      double topHeight = 0;

      if (bottomLevel != null)
      {
        btmHeight = bottomLevel.Elevation + bottomOffset;
      }

      if (topLevel != null)
      {
        topHeight = topLevel.Elevation + topOffset;
      }

      foreach (Revit.DB.CurveLoop crvLoop in face.GetEdgesAsCurveLoops())
      {
        foreach (Revit.DB.Curve curve in crvLoop)
        {
          Revit.DB.XYZ p0 = curve.GetEndPoint(0);
          Revit.DB.XYZ p1 = curve.GetEndPoint(1);

          // 上下とも無制限
          if (bottomLevel == null &&
              topLevel == null)
          {
            ret = true;
            return ret;
          }
          // 下部レベルだけ無制限の場合、上部レベルだけ比較
          else if (bottomLevel == null)
          {
            if (topHeight >= p0.Z ||
                topHeight >= p1.Z)
            {
              ret = true;
              return ret;
            }
          }
          // 上部レベルだけ無制限の場合、下部レベルだけ比較
          else if (topLevel == null)
          {
            if (btmHeight <= p0.Z ||
                btmHeight <= p1.Z)
            {
              ret = true;
              return ret;
            }
          }
          // 上下とも制限
          else
          {
            // エッジ端部のどちらかが範囲内
            // または両端部が上下どちらとも範囲外(ビュー範囲を貫通)
            if ((topHeight >= p0.Z && btmHeight <= p0.Z) ||
                (topHeight >= p1.Z && btmHeight <= p1.Z) ||
                (topHeight <= p0.Z && btmHeight >= p1.Z) ||
                (topHeight <= p1.Z && btmHeight >= p0.Z))
            {
              ret = true;
              return ret;
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>線分の表示判定</summary>
    /// 
    /// <param name="curve"       >カーブ</param>
    /// <param name="bottomLevel" >下部レベル</param>
    /// <param name="topLevel"    >上部レベル</param>
    /// <param name="bottomOffset">下部オフセット</param>
    /// <param name="topOffset"   >上部オフセット</param>
    /// 
    /// <history>2017/01/12 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsVisibleCurveInHeight(Revit.DB.Curve curve,
                                Revit.DB.Level bottomLevel,
                                Revit.DB.Level topLevel,
                                double bottomOffset,
                                double topOffset)
    {
      // 戻り値
      bool ret = false;

      double btmHeight = 0;
      double topHeight = 0;

      if (bottomLevel != null)
      {
        btmHeight = bottomLevel.Elevation + bottomOffset;
      }

      if (topLevel != null)
      {
        topHeight = topLevel.Elevation + topOffset;
      }

      Revit.DB.XYZ p0 = curve.GetEndPoint(0);
      Revit.DB.XYZ p1 = curve.GetEndPoint(1);

      // 上下とも無制限
      if (bottomLevel == null &&
          topLevel == null)
      {
        ret = true;
      }
      // 下部レベルだけ無制限の場合、上部レベルだけ比較
      else if (bottomLevel == null)
      {
        if (topHeight >= p0.Z ||
            topHeight >= p1.Z)
        {
          ret = true;
        }
      }
      // 上部レベルだけ無制限の場合、下部レベルだけ比較
      else if (topLevel == null)
      {
        if (btmHeight <= p0.Z ||
            btmHeight <= p1.Z)
        {
          ret = true;
        }
      }
      // 上下とも制限
      else
      {
        // エッジ端部のどちらかが範囲内
        // または両端部が上下どちらとも範囲外(ビュー範囲を貫通)
        if ((topHeight >= p0.Z && btmHeight <= p0.Z) ||
            (topHeight >= p1.Z && btmHeight <= p1.Z) ||
            (topHeight <= p0.Z && btmHeight >= p1.Z) ||
            (topHeight <= p1.Z && btmHeight >= p0.Z))
        {
          ret = true;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>面と線分の関連確認</summary>
    /// 
    /// <param name="plnFace" >平面</param>
    /// <param name="line"    >線分</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    int CheckRelationCurve(Revit.DB.PlanarFace plnFace,
                            Revit.DB.Line line)
    {
      // 戻り値
      int ret = 0;

      Revit.DB.XYZ pA = plnFace.Origin;
      Revit.DB.XYZ pB = pA + plnFace.XVector;
      Revit.DB.XYZ pC = pA + plnFace.YVector;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 面と交差
      Revit.DB.XYZ crossPos = GetCrossPoint(line, pA, pB, pC, 0);

      if (crossPos != null)
      {
        ret = 1;
        return ret;
      }

      // 面上
      bool onPlane0 = IsPointOnPlane(p0, pA, pB, pC);
      bool onPlane1 = IsPointOnPlane(p1, pA, pB, pC);

      if (onPlane0 || onPlane1)
      {
        ret = 2;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>範囲内線分取得</summary>
    /// 
    /// <param name="curve"       >カーブ</param>
    /// <param name="bottomLevel" >下部レベル</param>
    /// <param name="topLevel"    >上部レベル</param>
    /// <param name="bottomOffset">下部オフセット</param>
    /// <param name="topOffset"   >上部オフセット</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line GetLineInHeight(Revit.DB.Curve curve,
                                  Revit.DB.Level bottomLevel,
                                  Revit.DB.Level topLevel,
                                  double bottomOffset,
                                  double topOffset)
    {
      // 戻り値
      Revit.DB.Line ret = null;

      if (IsVisibleCurveInHeight(curve, bottomLevel, topLevel, bottomOffset, topOffset))
      {
        Revit.DB.XYZ p0 = curve.GetEndPoint(0);
        Revit.DB.XYZ p1 = curve.GetEndPoint(1);

        Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);

        double btmHeight = 0;
        double topHeight = 0;

        if (bottomLevel != null)
        {
          btmHeight = bottomLevel.Elevation + bottomOffset;

          if (btmHeight > p0.Z)
          {
            Revit.DB.XYZ p = GetMidPointOnLine(line, btmHeight);

            if (p != null)
            {
              p0 = p;
            }
          }
          if (btmHeight > p1.Z)
          {
            Revit.DB.XYZ p = GetMidPointOnLine(line, btmHeight);

            if (p != null)
            {
              p1 = p;
            }
          }
        }

        if (topLevel != null)
        {
          topHeight = topLevel.Elevation + topOffset;

          if (topHeight < p0.Z)
          {
            Revit.DB.XYZ p = GetMidPointOnLine(line, topHeight);

            if (p != null)
            {
              p0 = p;
            }
          }
          if (topHeight < p1.Z)
          {
            Revit.DB.XYZ p = GetMidPointOnLine(line, topHeight);

            if (p != null)
            {
              p1 = p;
            }
          }
        }

        if (Distance(p0, p1) > Approx0Len)
        {
          ret = Revit.DB.Line.CreateBound(p0, p1);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>基準面と対する面の関連確認</summary>
    /// 
    /// <param name="plnFace1">基準平面</param>
    /// <param name="plnFace2">対象平面</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool CheckRelationFace(Revit.DB.PlanarFace plnFace1,
                           Revit.DB.PlanarFace plnFace2)
    {
      // 戻り値
      bool ret = false;

      if (plnFace1 == null || plnFace2 == null)
      {
        return ret;
      }

      Collections.Generic.IList<Revit.DB.CurveLoop> loops = plnFace2.GetEdgesAsCurveLoops();

      foreach (Revit.DB.CurveLoop loop in loops)
      {
        foreach (Revit.DB.Curve curve in loop)
        {
          Revit.DB.Line line = Revit.DB.Line.CreateBound(curve.GetEndPoint(0), curve.GetEndPoint(1));

          // 面と交差
          if (CheckRelationCurve(plnFace1, line) == 1)
          {
            ret = true;
            return ret;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>面への投影線分</summary>
    /// 
    /// <param name="face">平面</param>
    /// <param name="line">線分</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line GetShadowLine(Revit.DB.PlanarFace face,
                                Revit.DB.Line line)
    {
      // 戻り値
      Revit.DB.Line ret = null;

      Revit.DB.XYZ pA = face.Origin;
      Revit.DB.XYZ pB = pA + face.XVector;
      Revit.DB.XYZ pC = pA + face.YVector;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // 平面の定数
      double a = 0;
      double b = 0;
      double c = 0;
      double d = 0;

      // 平面の方程式
      GetPlaneEquation(pA,
                       pB,
                       pC,
                       ref a,
                       ref b,
                       ref c,
                       ref d);

      // 平面の法線
      Revit.DB.XYZ normal = new Revit.DB.XYZ(a, b, c);

      Revit.DB.Line shadowLine0 = Revit.DB.Line.CreateBound(p0, p0 + normal);
      Revit.DB.Line shadowLine1 = Revit.DB.Line.CreateBound(p1, p1 + normal);

      Revit.DB.XYZ shadowPos0 = GetCrossPoint(shadowLine0, pA, pB, pC, 1);
      Revit.DB.XYZ shadowPos1 = GetCrossPoint(shadowLine1, pA, pB, pC, 1);

      if (Distance(shadowPos0, shadowPos1) > Approx0Len)
      {
        ret = Revit.DB.Line.CreateBound(shadowPos0, shadowPos1);
      }

      return ret;
    }
    
    /// ================================================================================
    /// <summary>指定高さの線分上点</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="midZ">指定高さ</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
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
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
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
    /// <summary>カーブの直線化</summary>
    /// 
    /// <param name="curve">カーブ</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Line> CurveConvertToLine(Revit.DB.Curve curve)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

      Collections.Generic.IList<Revit.DB.XYZ> tesselated = curve.Tessellate();

      for (int i = 0; i < tesselated.Count; ++i)
      {
        if (i == tesselated.Count - 1)
        {
          break;
        }

        Revit.DB.XYZ p0 = tesselated[i];
        Revit.DB.XYZ p1 = tesselated[i + 1];

        Revit.DB.Line newLine = Revit.DB.Line.CreateBound(p0, p1);

        // 最初
        if (i == 0)
        {
          ret.Add(newLine);
          continue;
        }

        Revit.DB.Line beforeLine = ret[ret.Count - 1];

        // 平行判定
        bool isParallel = IsParallel(newLine, beforeLine);

        // ほぼ平行な場合
        if (isParallel)
        {
          // 終点を変更
          beforeLine = Revit.DB.Line.CreateBound(beforeLine.GetEndPoint(0), p1);

          ret[ret.Count - 1] = beforeLine;
        }
        // 平行ではない場合
        else
        {
          ret.Add(newLine);
        }
      }


      return ret;
    }

    /// ================================================================================
    /// <summary>2直線の交差判定</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsParallel(Revit.DB.Line line1,
                    Revit.DB.Line line2)
    {
      bool ret = false;

      Revit.DB.XYZ p0 = line1.GetEndPoint(0);
      Revit.DB.XYZ p1 = line1.GetEndPoint(1);
      Revit.DB.XYZ p2 = line2.GetEndPoint(0);
      Revit.DB.XYZ p3 = line2.GetEndPoint(1);

      double f1 = p1.X - p0.X;
      double f2 = p3.X - p2.X;

      double g1 = p1.Y - p0.Y;
      double g2 = p3.Y - p2.Y;

      double det = f2 * g1 - f1 * g2;

      if (0.0001 > System.Math.Abs(det))
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>点からの垂線と直線の交点</summary>
    /// 
    /// <param name="line">直線</param>
    /// <param name="pnt"   >点</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetVertical(Revit.DB.Line line,
                             Revit.DB.XYZ pnt)
    {
      // 戻り値
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      if (ToHalfAdjust(Distance(p0, p1), -9) != 0)
      {
        double k = ((pnt.X - p0.X) * (p1.X - p0.X) + (pnt.Y - p0.Y) * (p1.Y - p0.Y) + (pnt.Z - p0.Z) * (p1.Z - p0.Z)) /
                   ((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y) + (p1.Z - p0.Z) * (p1.Z - p0.Z));

        ret = new Revit.DB.XYZ(k * p1.X + (1 - k) * p0.X,
                               k * p1.Y + (1 - k) * p0.Y,
                               k * p1.Z + (1 - k) * p0.Z);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>平行な2直線間の距離</summary>
    /// 
    /// <param name="line1">直線1</param>
    /// <param name="line2">直線2</param>
    /// 
    /// <history>2016/12/26 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    double ParallelLineDistance(Revit.DB.Line line1,
                                Revit.DB.Line line2)
    {
      // 戻り値
      double ret = 0;

      if (IsParallelLine(line1, line2))
      {
        Revit.DB.XYZ p0 = line1.GetEndPoint(0);
        Revit.DB.XYZ pos = GetVertical(line2, p0);

        ret = Distance(p0, pos);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>点位置のカーブ内補正</summary>
    /// 
    /// <param name="pnt"     >点</param>
    /// <param name="curveAry">カーブ</param>
    /// 
    /// <hisotry>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</hisotry>
    /// ================================================================================
    public
    Revit.DB.XYZ PointAdjustInCurves(Revit.DB.XYZ pnt,
                                     Collections.Generic.IList<Revit.DB.Curve> curveAry)
    {
      Revit.DB.XYZ ret = null;

      if (pnt == null || curveAry.Count < 3)
      {
        return ret;
      }

      // 最適化
      curveAry = OptimizeLineVertexConvLine(curveAry);

      try
      {
        // 三角形分割
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> triAreas = GetTriangelAreas(curveAry);

        // 1つ目の三角形重心
        Revit.DB.XYZ p = TriangleGravity2D(triAreas[0][0], triAreas[0][1], triAreas[0][2]);

        // 多角形内外判定
        if (PointInCurves(curveAry, p, pnt, 0))
        {
          ret = pnt;
        }
        else
        {
          // 一番近い重心
          double distance = 0;

          for (int i = 0; i < triAreas.Count; ++i)
          {
            Collections.Generic.IList<Revit.DB.XYZ> triArea = triAreas[i];
            Revit.DB.XYZ grav = TriangleGravity2D(triArea[0], triArea[1], triArea[2]);
            double dis = Distance2D(pnt, grav);

            if (i == 0)
            {
              ret = grav;
              distance = dis;
            }
            else
            {
              if (distance > dis)
              {
                ret = grav;
                distance = dis;
              }
            }
          }
        }
      }
      catch
      {
        ret = null;
      }

      if (ret != null)
      {
        ret = new Revit.DB.XYZ(ret.X, ret.Y, pnt.Z);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>点のカーブ内判定</summary>
    /// 
    /// <param name="curves">カーブ</param>
    /// <param name="inPos" >カーブ内点</param>
    /// <param name="pos"   >対象点</param>
    /// <param name="mode"  ><p>辺上</p>
    ///                       <p>0 = 辺上含まない</p>
    ///                       <p>1 = 辺上含む</p></param>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool PointInCurves(Collections.Generic.IList<Revit.DB.Curve> curves,
                       Revit.DB.XYZ inPos,
                       Revit.DB.XYZ pos,
                       int mode)
    {
      // 戻り値
      bool ret = false;

      if (Distance(inPos, pos) < Approx0Len)
      {
        ret = true;
        return ret;
      }

      // 2点を結ぶ線分
      Revit.DB.Curve inPosCurve = Revit.DB.Line.CreateBound(pos, inPos);

      int flag = 0;
      Collections.Generic.IList<Revit.DB.XYZ> retPosAry = new Collections.Generic.List<Revit.DB.XYZ>();

      for (int i = 0; i < curves.Count; ++i)
      {
        double dist0 = Distance(pos, curves[i].GetEndPoint(0));

        if (dist0 > Approx0Len)
        {
          Revit.DB.XYZ crossPnt = CrossPoint(inPosCurve, curves[i]);

          if (crossPnt != null)
          {
            // 同一点は除外
            bool containPos = false;

            foreach (Revit.DB.XYZ retPos in retPosAry)
            {
              if (Distance2D(retPos, crossPnt) < Approx0Len)
              {
                containPos = true;
                break;
              }
            }

            if (containPos == false)
            {
              retPosAry.Add(crossPnt);
            }
          }

        }
        // 頂点上
        else
        {
          // 辺上含む
          if (mode == 1)
          {
            flag = 1;
            break;
          }
        }
      }

      if (flag == 1)
      {
        ret = true;
      }
      else
      {
        bool flag2 = false;

        for (int i = 0; i < retPosAry.Count; ++i)
        {
          double dist = Distance(pos, retPosAry[i]);

          // 交点上
          if (dist < Approx0Len)
          {
            flag2 = true;
          }
        }

        if (flag2)
        {
          // 辺上
          if (mode == 1)
          {
            ret = true;
          }
          else
          {
            ret = false;
          }
        }
        else
        {
          // 偶数回交差
          if ((retPosAry.Count % 2) == 0)
          {
            ret = true;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>多角形の三角形分割</summary>
    /// 
    /// <param name="curveAry">カーブ</param>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> GetTriangelAreas(Collections.Generic.IList<Revit.DB.Curve> curveAry)
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      SetPnts2D(curveAry);

      Revit.DB.XYZ p0 = _Pnts[0];

      Collections.Generic.IList<Revit.DB.XYZ> triPnts = new Collections.Generic.List<Revit.DB.XYZ>();

      // 一番遠い点
      int farIndex = 0;
      double farDistance = 0;

      for (int i = 0; i < _Pnts.Count; ++i)
      {
        Revit.DB.XYZ p = _Pnts[i];
        double distance = Distance2D(p0, p);

        if (distance > farDistance)
        {
          farIndex = i;
          farDistance = distance;
        }
      }

      // 一番遠い点と両隣の点 = 凸になる
      Revit.DB.XYZ farPnt = _Pnts[farIndex];
      Revit.DB.XYZ sidePnt1 = GetBefore(farPnt);
      Revit.DB.XYZ sidePnt2 = GetAfter(farPnt);

      // 基準の外積
      double baseGaiseki = CrossProduct2D(farPnt, sidePnt1, sidePnt2);

      bool firstFlag = true;

      while (_Pnts.Count > 0)
      {
        if (_Pnts.Count == 3)
        {
          triPnts = new Collections.Generic.List<Revit.DB.XYZ>();

          triPnts.Add(_Pnts[0]);
          triPnts.Add(_Pnts[1]);
          triPnts.Add(_Pnts[2]);

          ret.Add(triPnts);

          break;
        }

        // 三角形の重心
        Revit.DB.XYZ triGrav = TriangleGravity2D(farPnt, sidePnt1, sidePnt2);

        // 三角形内に他の点があるか
        Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
        curves.Add(Revit.DB.Line.CreateBound(farPnt, sidePnt1));
        curves.Add(Revit.DB.Line.CreateBound(sidePnt1, sidePnt2));
        curves.Add(Revit.DB.Line.CreateBound(sidePnt2, farPnt));
        bool inTriangle = false;
        foreach (Revit.DB.XYZ p in _Pnts)
        {
          if (IsPointOnPoint(triGrav, p) ||
              IsPointOnPoint(farPnt, p) ||
              IsPointOnPoint(sidePnt1, p) ||
              IsPointOnPoint(sidePnt2, p))
          {
            continue;
          }

          inTriangle = PointInCurves(curves, triGrav, p, 1);

          if (inTriangle)
          {
            break;
          }
        }

        if (firstFlag)
        {
          double gaiseki = CrossProduct2D(farPnt, sidePnt1, sidePnt2);

          // 同じ方向(凹凸)
          if ((baseGaiseki > 0 && gaiseki > 0) ||
              (baseGaiseki == 0 && gaiseki == 0) ||
              (baseGaiseki < 0 && gaiseki < 0))
          {
          }
          else
          {
            inTriangle = true;
          }

          firstFlag = false;
        }

        // 内点あり
        if (inTriangle)
        {
          // 次の点
          bool next = false;

          while (next == false)
          {
            farIndex += 1;
            if (farIndex > _Pnts.Count - 1)
            {
              farIndex = 0;
            }

            farPnt = _Pnts[farIndex];
            sidePnt1 = GetBefore(farPnt);
            sidePnt2 = GetAfter(farPnt);

            double gaiseki = CrossProduct2D(farPnt, sidePnt1, sidePnt2);

            // 同じ方向(凹凸)
            if ((baseGaiseki > 0 && gaiseki > 0) ||
                (baseGaiseki == 0 && gaiseki == 0) ||
                (baseGaiseki < 0 && gaiseki < 0))
            {
              next = true;
            }
          }
        }
        // 内点なし
        else
        {
          triPnts = new Collections.Generic.List<Revit.DB.XYZ>();

          triPnts.Add(farPnt);
          triPnts.Add(sidePnt1);
          triPnts.Add(sidePnt2);

          // 三角形取得
          ret.Add(triPnts);

          // 頂点を除外
          RemovePnt(farPnt);

          if (farIndex > _Pnts.Count - 1)
          {
            farIndex = 0;
          }

          farPnt = _Pnts[farIndex];
          sidePnt1 = GetBefore(farPnt);
          sidePnt2 = GetAfter(farPnt);

          firstFlag = true;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>頂点セット</summary>
    /// 
    /// <param name="curveAry">カーブ</param>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void SetPnts2D(Collections.Generic.IList<Revit.DB.Curve> curveAry)
    {
      _Pnts = new Collections.Generic.List<Revit.DB.XYZ>();

      foreach (Revit.DB.Curve crv in curveAry)
      {
        Revit.DB.XYZ p = crv.GetEndPoint(0);
        p = new Revit.DB.XYZ(p.X, p.Y, 0);
        _Pnts.Add(p);
      }
    }

    /// ================================================================================
    /// <summary>先の点取得</summary>
    /// 
    /// <param name="pnt">点</param>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetBefore(Revit.DB.XYZ pnt)
    {
      Revit.DB.XYZ ret = null;

      int index = 0;
      double distance = 0;

      for (int i = 0; i < _Pnts.Count; ++i)
      {
        if (i == 0)
        {
          distance = Distance2D(pnt, _Pnts[i]);
        }
        else
        {
          double d = Distance2D(pnt, _Pnts[i]);

          if (d < distance)
          {
            index = i;
            distance = d;
          }
        }
      }

      int beforeIndex = index - 1;
      if (beforeIndex < 0)
      {
        beforeIndex = _Pnts.Count - 1;
      }

      ret = _Pnts[beforeIndex];
      return ret;
    }

    /// ================================================================================
    /// <summary>後の点取得</summary>
    /// 
    /// <param name="pnt">点</param>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetAfter(Revit.DB.XYZ pnt)
    {
      Revit.DB.XYZ ret = null;

      int index = 0;
      double distance = 0;

      for (int i = 0; i < _Pnts.Count; ++i)
      {
        if (i == 0)
        {
          distance = Distance2D(pnt, _Pnts[i]);
        }
        else
        {
          double d = Distance2D(pnt, _Pnts[i]);

          if (d < distance)
          {
            index = i;
            distance = d;
          }
        }
      }

      int afterIndex = index + 1;
      if (afterIndex > _Pnts.Count - 1)
      {
        afterIndex = 0;
      }

      ret = _Pnts[afterIndex];
      return ret;
    }

    /// ================================================================================
    /// <summary>点の除外</summary>
    /// 
    /// <param name="pnt">点</param>
    /// 
    /// <history>2016/05/25 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void RemovePnt(Revit.DB.XYZ pnt)
    {
      int index = 0;
      double distance = 0;

      for (int i = 0; i < _Pnts.Count; ++i)
      {
        if (i == 0)
        {
          distance = Distance2D(pnt, _Pnts[i]);
        }
        else
        {
          double d = Distance2D(pnt, _Pnts[i]);

          if (d < distance)
          {
            index = i;
            distance = d;
          }
        }
      }

      _Pnts.RemoveAt(index);
    }

    /// ================================================================================
    /// <summary>カーブ交点</summary>
    /// 
    /// <history>2016/10/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ CrossPoint(Revit.DB.Curve crv1, Revit.DB.Curve crv2)
    {
      Revit.DB.XYZ ret = null;

      Revit.DB.XYZ pnt1 = crv1.GetEndPoint(0);
      Revit.DB.XYZ pnt2 = crv1.GetEndPoint(1);
      Revit.DB.XYZ pnt3 = crv2.GetEndPoint(0);
      Revit.DB.XYZ pnt4 = crv2.GetEndPoint(1);

      double x1 = pnt1.X;
      double y1 = pnt1.Y;
      double x2 = pnt2.X;
      double y2 = pnt2.Y;
      double x3 = pnt3.X;
      double y3 = pnt3.Y;
      double x4 = pnt4.X;
      double y4 = pnt4.Y;

      //double judge1 = ((x3 - x1) * (y2 - y1) - (x2 - x1) * (y3 - y1)) * ((x4 - x1) * (y2 - y1) - (x2 - x1) * (y4 - y1));
      //double judge2 = ((x1 - x3) * (y4 - y3) - (x4 - x3) * (y1 - y3)) * ((x2 - x3) * (y4 - y3) - (x4 - x3) * (y2 - y3));

      double f1 = x2 - x1;
      double f2 = x4 - x3;

      double g1 = y2 - y1;
      double g2 = y4 - y3;

      double det = f2 * g1 - f1 * g2;

      // 平行で交点なし
      if (ToHalfAdjust(det, -9) == 0)
      {
        return ret;
      }

      double dx = x3 - x1;
      double dy = y3 - y1;

      double t1 = (f2 * dy - g2 * dx) / det;
      double t2 = (f1 * dy - g1 * dx) / det;

      t1 = ToHalfAdjust(t1, -9);
      t2 = ToHalfAdjust(t2, -9);

      // 範囲外
      if (t1 < 0 || t1 > 1 || t2 < 0 || t2 > 1)
      {
        return ret;
      }

      double x = x1 + f1 * t1;
      double y = y1 + g1 * t1;

      ret = new Revit.DB.XYZ(x, y, pnt1.Z);

      return ret;
    }


    /// ================================================================================
    /// <summary>カーブの交点位置取得</summary>
    /// 
    /// <param name="curveAry">カーブ</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2016/10/28 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>> GetInterPosCurvesDic(Collections.Generic.IList<Revit.DB.Curve> curveAry)
    {
      // 戻り値
      Collections.Generic.IDictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>> ret = new Collections.Generic.Dictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>>();

      // カーブ
      for (int i = 0; i < curveAry.Count; ++i)
      {
        Revit.DB.Curve curve1 = curveAry[i];
        Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp1 = new Collections.Generic.List<Revit.DB.XYZ>();
        for (int j = 0; j < curveAry.Count; ++j)
        {
          if (i == j)
          {
            continue;
          }
          Revit.DB.Curve curve2 = curveAry[j];

          // 交点検索
          Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp = GetInterPosCurves2D(curve1, curve2);
          for (int k = 0; k < interPosAryTmp.Count; ++k)
          {
            interPosAryTmp1.Add(interPosAryTmp[k]);
          }
        }

        // 並び替え
        Collections.Generic.IList<int> sortedIdxAry = new Collections.Generic.List<int>();
        Collections.Generic.IList<Revit.DB.XYZ> sortedPosAryTmp1 = new Collections.Generic.List<Revit.DB.XYZ>();
        SortXYPos(interPosAryTmp1, 1, ref sortedIdxAry, ref sortedPosAryTmp1);

        // 重複除外
        Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp2 = new Collections.Generic.List<Revit.DB.XYZ>();
        for (int j = 0; j < sortedPosAryTmp1.Count; ++j)
        {
          bool flag = false;
          for (int k = 0; k < interPosAryTmp2.Count; ++k)
          {
            if (Distance2D(sortedPosAryTmp1[j], interPosAryTmp2[k]) < Approx0Len)
            {
              flag = true;
              break;
            }
          }
          if (flag == false)
          {
            interPosAryTmp2.Add(sortedPosAryTmp1[j]);
          }
        }

        ret.Add(curve1, interPosAryTmp2);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブ交点の平面カーブ取得</summary>
    /// 
    /// <param name="basePos"             >基準点</param>
    /// <param name="relaPos"             >関連点</param>
    /// <param name="dicCurveInterPosAryAry" >カーブ交点</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history><p>2011/12/02 Created  GSA,Inc. Shinichi Ishii</p>
    ///           <p>2016/10/28 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetPlanFaceCurveInterPos_Test(
        Revit.DB.XYZ basePos,
        Revit.DB.XYZ relaPos,
        Collections.Generic.IDictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>> dicCurveInterPosAryAry)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      Revit.DB.XYZ posF = basePos;
      Revit.DB.XYZ posB = basePos;
      Revit.DB.XYZ posA = null;
      Revit.DB.XYZ posN = null;

      int cntNum = -1;
      int numMax = 1000;
      bool flagEnd = false;


      Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
      posAry.Add(posF);

      // 隣接点取得
      Collections.Generic.IList<Revit.DB.XYZ> nextPosAry = GetRelatedPos(posF, null, dicCurveInterPosAryAry);

      // 反時計回り点取得
      int idxPos = GetPosReClockwise(posF, relaPos, nextPosAry, true);
      if (idxPos == -1)
      {
        return ret;
      }
      else
      {
        posN = nextPosAry[idxPos];
      }
      posAry.Add(posN);
      posB = posN;
      posA = posF;


      // 次点の候補
      while (flagEnd == false)
      {
        // 制限値越え
        cntNum++;
        if (cntNum > numMax)
        {
          break;
        }

        // 隣接点取得
        posN = null;
        nextPosAry = GetRelatedPos(posB, posA, dicCurveInterPosAryAry);

        // 反時計回り点取得
        idxPos = GetPosReClockwise(posB, posA, nextPosAry, false);
        if (idxPos > -1)
        {
          posN = nextPosAry[idxPos];
        }

        // 次点判定
        if (posN != null)
        {
          posAry.Add(posN);
          posA = posB;
          posB = posN;
          if (Distance(posF, posN) < Approx0Len)
          {
            flagEnd = true;
          }
        }
        else
        {
          break;
        }
      }

      // 点を確認
      if (posAry.Count < 3)
      {
        return ret;
      }
      if (Distance(posAry[0], posAry[posAry.Count - 1]) > Approx0Len)
      {
        return ret;
      }

      // 線分作成
      bool flagRet = true;
      Collections.Generic.IList<Revit.DB.Curve> retCurves = new Collections.Generic.List<Revit.DB.Curve>();
      for (int i = 1; i < posAry.Count; ++i)
      {
        // 線分距離確認
        Revit.DB.XYZ pos1 = posAry[i - 1];
        Revit.DB.XYZ pos2 = posAry[i];
        if (Distance(pos1, pos2) < Approx0Len)
        {
          flagRet = false;
          break;
        }

        // 他線分の交差確認
        Revit.DB.Curve curve = GetCurveBasePoint(dicCurveInterPosAryAry.Keys, pos1, pos2);// Revit.DB.Line.CreateBound(pos1, pos2);
        if (curve == null)
        {
          continue;
        }

        bool flag = true;
        for (int j = 0; j < retCurves.Count; ++j)
        {
          Collections.Generic.IList<Revit.DB.XYZ> interPosAry = new Collections.Generic.List<Revit.DB.XYZ>();
          IntersecCurve(curve, retCurves[j], ref interPosAry);
          for (int k = 0; k < interPosAry.Count; ++k)
          {
            if ((Distance(pos1, interPosAry[k]) > Approx0Len) &&
                (Distance(pos2, interPosAry[k]) > Approx0Len))
            {
              flag = false;
              break;
            }
          }
          if (flag == false)
          {
            break;
          }
        }
        if (flag == true)
        {
          retCurves.Add(curve);
        }
        else
        {
          flagRet = false;
          break;
        }
      }
      if (flagRet == true)
      {
        ret = retCurves;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブ交点の平面カーブ取得</summary>
    /// 
    /// <param name="dicCurveInterPosAryAry" >カーブ交点</param>
    /// <param name="height"              >高さ</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history><p>2011/12/02 Created  GSA,Inc. Shinichi Ishii</p>
    ///           <p>2016/10/28 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> GetPlanFaceCurveInterPos_Test(
        Collections.Generic.IDictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>> dicCurveInterPosAryAry)
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> ret =
          new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

      // カーブ
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> pFaceCurveAryAry =
          new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

      Collections.Generic.IList<Revit.DB.XYZ> pFaceGravityAry = new Collections.Generic.List<Revit.DB.XYZ>();

      foreach (Revit.DB.Curve curve in dicCurveInterPosAryAry.Keys)
      //for (int i = 0; i < dicCurveInterPosAryAry.Count; ++i)
      {
        // 交点位置
        Collections.Generic.IList<Revit.DB.XYZ> curveInterPosAry = dicCurveInterPosAryAry[curve];
        for (int j = 0; j < curveInterPosAry.Count; ++j)
        {
          Revit.DB.XYZ curveInterPos = curveInterPosAry[j];
          Revit.DB.XYZ basePos = curveInterPos;

          int idxN = j + 1;
          if (idxN > curveInterPosAry.Count - 1)
          {
            idxN = j - 1;
          }
          if (idxN < 0)
          {
            continue;
          }
          Revit.DB.XYZ relaPos = curveInterPosAry[idxN];

          // カーブ交点の平面
          Collections.Generic.IList<Revit.DB.Curve> planCurves = GetPlanFaceCurveInterPos_Test(basePos,
                                                                                               relaPos,
                                                                                               dicCurveInterPosAryAry);
          if (planCurves.Count > 0)
          {
            Revit.DB.XYZ gravity = PolygonGravity2D(planCurves);

            // 位置補正(重心が多角形の外の場合)
            gravity = PointAdjustInCurves(gravity, planCurves);

            if (gravity == null)
            {
              continue;
            }
            // 同じ重心かチェック
            bool flag = true;
            for (int k = 0; k < pFaceCurveAryAry.Count; ++k)
            {
              if (Distance2D(gravity, pFaceGravityAry[k]) < Approx0Len)
              {
                flag = false;
                break;
              }
            }
            if (flag == true)
            {
              pFaceCurveAryAry.Add(planCurves);
              pFaceGravityAry.Add(gravity);
            }
          }
        }
      }

      // 形状が重複しているかチェック
      for (int i = 0; i < pFaceCurveAryAry.Count; ++i)
      {
        // カーブ
        Collections.Generic.IList<Revit.DB.Curve> pFaceCurveAry = pFaceCurveAryAry[i];
        Revit.DB.XYZ pFaceGravity = pFaceGravityAry[i];

        // 面積
        double area = System.Math.Abs(GetPolygonArea(pFaceCurveAry));

        // 比較
        bool flag = true;
        for (int j = 0; j < pFaceCurveAryAry.Count; ++j)
        {
          if (i == j)
          {
            continue;
          }
          Collections.Generic.IList<Revit.DB.Curve> pFaceCurveAryTmp = pFaceCurveAryAry[j];
          Revit.DB.XYZ pFaceGravityTmp = pFaceGravityAry[j];
          double areaTmp = System.Math.Abs(GetPolygonArea(pFaceCurveAryTmp));

          // 重心内外判定
          bool isIn = IsPointInPolygon(pFaceCurveAryTmp, pFaceGravityTmp, pFaceGravity, 1);
          if (isIn == true)
          {
            if ((area - areaTmp) > 0.0)
            {
              flag = false;
              break;
            }
          }
        }

        if (flag == true)
        {
          ret.Add(pFaceCurveAry);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>隣接点取得</summary>
    /// 
    /// <param name="basePos"             >基準点</param>
    /// <param name="exclPos"             >除外点</param>
    /// <param name="dicCurveInterPosAryAry" >カーブ交点</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history><p>2011/12/02 Created  GSA,Inc. Shinichi Ishii</p>
    ///           <p>2016/10/28 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetRelatedPos(Revit.DB.XYZ basePos,
                                                          Revit.DB.XYZ exclPos,
                                                          Collections.Generic.IDictionary<Revit.DB.Curve, Collections.Generic.IList<Revit.DB.XYZ>> dicCurveInterPosAryAry)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> nextPosAry = new Collections.Generic.List<Revit.DB.XYZ>();

      foreach (Revit.DB.Curve curve in dicCurveInterPosAryAry.Keys)
      // for (int i = 0; i < dicCurveInterPosAryAry.Count; ++i)
      {
        for (int j = 0; j < dicCurveInterPosAryAry[curve].Count; ++j)
        {
          // 基準点を持つ組み合わせ
          if (Distance2D(basePos, dicCurveInterPosAryAry[curve][j]) < Approx0Len)
          {
            // 基準点ではない方の点
            int k = j + 1;
            if (k < dicCurveInterPosAryAry[curve].Count)
            {
              nextPosAry.Add(dicCurveInterPosAryAry[curve][k]);
            }

            k = j - 1;
            if (k > -1)
            {
              nextPosAry.Add(dicCurveInterPosAryAry[curve][k]);
            }
          }
        }
      }

      // 次点
      if (nextPosAry.Count > 0)
      {
        for (int i = 0; i < nextPosAry.Count; ++i)
        {
          if (exclPos == null)
          {
            ret.Add(nextPosAry[i]);
          }
          else
          {
            if (Distance2D(exclPos, nextPosAry[i]) > Approx0Len)
            {
              ret.Add(nextPosAry[i]);
            }
          }
        }
      }

      return ret;
    }


    public
    Revit.DB.Curve GetCurveBasePoint(Collections.Generic.ICollection<Revit.DB.Curve> crvAry,
                                     Revit.DB.XYZ p0,
                                     Revit.DB.XYZ p1)
    {
      Revit.DB.Curve ret = null;

      foreach (Revit.DB.Curve curve in crvAry)
      {
        Revit.DB.Line line = curve as Revit.DB.Line;
        Revit.DB.Arc arc = curve as Revit.DB.Arc;

        if (line != null)
        {
          Revit.DB.Line getLine = GetLineBasePoint(line, p0, p1);
          if (getLine != null)
          {
            ret = getLine;
            break;
          }
        }
        else if (arc != null)
        {
          Revit.DB.Arc getArc = GetArcBasePoint(arc, p0, p1);
          if (getArc != null)
          {
            ret = getArc;
            break;
          }
        }
      }

      return ret;
    }


    public
    Revit.DB.Line GetLineBasePoint(Revit.DB.Line line,
                                   Revit.DB.XYZ p0,
                                   Revit.DB.XYZ p1)
    {
      Revit.DB.Line ret = null;

      if (IsPointOnLine(line, p0) == false ||
          IsPointOnLine(line, p1) == false)
      {
        return ret;
      }

      ret = Revit.DB.Line.CreateBound(p0, p1);

      return ret;
    }

    public
    Revit.DB.Arc GetArcBasePoint(Revit.DB.Arc arc,
                                 Revit.DB.XYZ p0,
                                 Revit.DB.XYZ p1)
    {

      Revit.DB.Arc ret = null;

      if (IsPointOnArc(arc, p0) == false ||
          IsPointOnArc(arc, p1) == false)
      {
        return ret;
      }

      Revit.DB.XYZ center = arc.Center;
      double radius = arc.Radius;

      double angle0 = GetRadian(center, p0);
      double angle1 = GetRadian(center, p1);

      ret = Revit.DB.Arc.Create(BasePlane, radius, angle0, angle1);

      return ret;
    }

    /// ================================================================================
    /// <summary>点の直線上判定</summary>
    /// 
    /// <param name="line">直線</param>
    /// <param name="p"   >点</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
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
      if (ToHalfAdjust(Distance2D(p0, p), -9) == 0)
      {
        ret = true;
        return ret;
      }
      // 終点上
      if (ToHalfAdjust(Distance2D(p1, p), -9) == 0)
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
    /// <summary>点の円弧上判定</summary>
    /// 
    /// <param name="arc" >円弧</param>
    /// <param name="p"   >点</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsPointOnArc(Revit.DB.Arc arc,
                      Revit.DB.XYZ p)
    {
      bool ret = false;

      Revit.DB.XYZ center = arc.Center;

      // 半径と同じ距離
      if (System.Math.Abs(Distance2D(center, p) - arc.Radius) > Approx0Len)
      {
        return ret;
      }

      ret = true;
      return ret;
    }

    /// ================================================================================
    /// <summary>2点のラジアン角</summary>
    /// 
    /// <param name="start" >始点</param>
    /// <param name="end"   >終点</param>
    /// 
    /// <history>2016/10/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    double GetRadian(Revit.DB.XYZ start,
                     Revit.DB.XYZ end)
    {
      double ret = 0;

      ret = System.Math.Atan2(end.Y - start.Y,
                              end.X - start.X);

      return ret;
    }



    /// ================================================================================
    /// <summary>直線上のポイント(2D)</summary>
    /// 
    /// <param name="curve" >カーブ</param>
    /// <param name="pos"   >ポイント</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    bool GetCurveOnPos2D(Revit.DB.Curve curve, Revit.DB.XYZ pos)
    {
      // 戻り値
      bool ret = false;

      // 端点
      Revit.DB.XYZ pos0 = curve.GetEndPoint(0);
      Revit.DB.XYZ pos1 = curve.GetEndPoint(1);

      // 端点比較
      if (ret == false)
      {
        if (Distance2D(pos0, pos) < Approx0Len)
        {
          ret = true;
        }
        else if (Distance2D(pos1, pos) < Approx0Len)
        {
          ret = true;
        }
      }

      // 中間点角度比較
      if (ret == false)
      {
        double rad0 = System.Math.Abs(Angle2D(pos0, pos1, pos));
        double rad1 = System.Math.Abs(Angle2D(pos1, pos0, pos));
        if ((rad0 < Approx0Ang) && (rad1 < Approx0Ang))
        {
          ret = true;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブの交点位置取得(2D)</summary>
    /// 
    /// <param name="curve1">カーブ</param>
    /// <param name="curve2">カーブ</param>
    /// 
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetInterPosCurves2D(Revit.DB.Curve curve1, Revit.DB.Curve curve2)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 端点
      Revit.DB.XYZ pos10 = curve1.GetEndPoint(0);
      Revit.DB.XYZ pos11 = curve1.GetEndPoint(1);
      Revit.DB.XYZ pos20 = curve2.GetEndPoint(0);
      Revit.DB.XYZ pos21 = curve2.GetEndPoint(1);
      Revit.DB.XYZ interPos = null;

      // ベクトル平行
      bool isPalla = false;
      Revit.DB.XYZ vec1 = UnitVector(pos10, pos11);
      Revit.DB.XYZ vec2 = UnitVector(pos20, pos21);
      Revit.DB.XYZ vec1Rev = new Revit.DB.XYZ(vec1.X * -1.0, vec1.Y * -1.0, 0.0);

      if (Distance2D(vec1, vec2) < Approx0Len)
      {
        isPalla = true;
      }
      if (Distance2D(vec1Rev, vec2) < Approx0Len)
      {
        isPalla = true;
      }

      // 交点判定(平行）
      if (isPalla == true)
      {
        interPos = null;
        if (Distance2D(pos10, pos20) < Approx0Len)
        {
          interPos = pos10;
        }
        else if (Distance2D(pos10, pos21) < Approx0Len)
        {
          interPos = pos10;
        }
        else if (Distance2D(pos11, pos20) < Approx0Len)
        {
          interPos = pos11;
        }
        else if (Distance2D(pos11, pos21) < Approx0Len)
        {
          interPos = pos11;
        }

        if (interPos != null)
        {
          ret.Add(interPos);
        }
      }

      // 交点判定(平行以外）
      else
      {
        // 交点判定
        bool flagInter = false;
        interPos = IntersecVector2D(curve1, curve2);
        if (interPos != null)
        {
          interPos = new Revit.DB.XYZ(interPos.X, interPos.Y, curve1.GetEndPoint(0).Z);

          if (Distance2D(interPos, pos10) < ToleranceInter)
          {
            flagInter = true;
          }
          else if (Distance2D(interPos, pos11) < ToleranceInter)
          {
            flagInter = true;
          }
          else if (Distance2D(interPos, pos20) < ToleranceInter)
          {
            if (GetCurveOnPos2D(curve1, interPos) == true)
            {
              flagInter = true;
            }
          }
          else if (Distance2D(interPos, pos21) < ToleranceInter)
          {
            if (GetCurveOnPos2D(curve1, interPos) == true)
            {
              flagInter = true;
            }
          }
        }
        if (flagInter == true)
        {
          ret.Add(interPos);
        }

        // 交点(平行以外）
        Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp = new Collections.Generic.List<Revit.DB.XYZ>();
        IntersecCurve2D(curve1, curve2, ref interPosAryTmp);
        for (int i = 0; i < interPosAryTmp.Count; ++i)
        {
          Revit.DB.XYZ interPosTmp = new Revit.DB.XYZ(interPosAryTmp[i].X, interPosAryTmp[i].Y, curve1.GetEndPoint(0).Z);
          bool flag = true;
          if (flagInter == true)
          {
            if (Distance2D(interPos, interPosTmp) < ToleranceInter)
            {
              flag = false;
            }
          }
          if (flag == true)
          {
            ret.Add(interPosTmp);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブの交点位置取得</summary>
    /// 
    /// <param name="curveAry">カーブ</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> GetInterPosCurves(Collections.Generic.IList<Revit.DB.Curve> curveAry)
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> ret =
          new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      // カーブ
      for (int i = 0; i < curveAry.Count; ++i)
      {
        Revit.DB.Curve curve1 = curveAry[i];
        Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp1 = new Collections.Generic.List<Revit.DB.XYZ>();
        for (int j = 0; j < curveAry.Count; ++j)
        {
          if (i == j)
          {
            continue;
          }
          Revit.DB.Curve curve2 = curveAry[j];

          // 交点検索
          Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp = GetInterPosCurves2D(curve1, curve2);
          for (int k = 0; k < interPosAryTmp.Count; ++k)
          {
            interPosAryTmp1.Add(interPosAryTmp[k]);
          }
        }

        // 並び替え
        Collections.Generic.IList<int> sortedIdxAry = new Collections.Generic.List<int>();
        Collections.Generic.IList<Revit.DB.XYZ> sortedPosAryTmp1 = new Collections.Generic.List<Revit.DB.XYZ>();
        SortXYPos(interPosAryTmp1, 1, ref sortedIdxAry, ref sortedPosAryTmp1);

        // 重複除外
        Collections.Generic.IList<Revit.DB.XYZ> interPosAryTmp2 = new Collections.Generic.List<Revit.DB.XYZ>();
        for (int j = 0; j < sortedPosAryTmp1.Count; ++j)
        {
          bool flag = false;
          for (int k = 0; k < interPosAryTmp2.Count; ++k)
          {
            if (Distance2D(sortedPosAryTmp1[j], interPosAryTmp2[k]) < Approx0Len)
            {
              flag = true;
              break;
            }
          }
          if (flag == false)
          {
            interPosAryTmp2.Add(sortedPosAryTmp1[j]);
          }
        }

        ret.Add(interPosAryTmp2);
      }
      return ret;
    }

    /// ================================================================================
    /// <summary>反時計回り点取得</summary>
    /// 
    /// <param name="basePos" >基準点</param>
    /// <param name="compPos" >比較点</param>
    /// <param name="posAry"  >点</param>
    /// <param name="isComp"  ><p>比較点</p>
    ///                           <p>True  = 含む</p>
    ///                           <p>False = 含まない</p>
    ///
    /// <returns>点インデックス</returns>
    ///           
    /// <history>2011/12/02 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    int GetPosReClockwise(Revit.DB.XYZ basePos,
                          Revit.DB.XYZ compPos,
                          Collections.Generic.IList<Revit.DB.XYZ> posAry,
                          bool isComp)
    {
      // 戻り値
      int ret = -1;

      // 初期化
      Revit.DB.XYZ pos = null;
      double rad = 0.0;
      Collections.Generic.IList<int> idxPosAry = null;

      // 基準点確認
      if (basePos == null)
      {
        return ret;
      }

      // 点確認
      if (posAry == null)
      {
        return ret;
      }

      // 比較点確認
      if (compPos == null)
      {
        return ret;
      }

      // 点を検索(比較点含まない)
      int idxPi = -1;
      idxPosAry = new Collections.Generic.List<int>();
      for (int i = 0; i < posAry.Count; ++i)
      {
        pos = posAry[i];

        if (Distance2D(basePos, pos) < Approx0Len)
        {
          continue;
        }
        if (Distance2D(compPos, pos) < Approx0Len)
        {
          continue;
        }

        rad = Angle2D(basePos, compPos, pos);
        if (System.Math.Abs(System.Math.PI - System.Math.Abs(rad)) < Approx0Ang)
        {
          idxPi = i;
        }
        else
        {
          if (System.Math.Abs(rad) > Approx0Ang)
          {
            if (rad < 0)
            {
              idxPosAry.Add(i);
            }
          }
        }
      }
      double minRad = 0.0;
      int minIdx = -1;
      for (int i = 0; i < idxPosAry.Count; ++i)
      {
        int idx = idxPosAry[i];
        rad = System.Math.Abs(Angle2D(basePos, compPos, posAry[idx]));
        if (i == 0)
        {
          minRad = rad;
          minIdx = idx;
        }
        else
        {
          if (rad < minRad)
          {
            minRad = rad;
            minIdx = idx;
          }
        }
      }
      ret = minIdx;


      // 点を検索(比較点含む)
      if (ret == -1)
      {
        if (isComp == true)
        {
          // 比較点
          int idxCmp = -1;
          for (int i = 0; i < posAry.Count; ++i)
          {
            pos = posAry[i];

            if (Distance2D(basePos, pos) < Approx0Len)
            {
              continue;
            }
            if (Distance2D(compPos, pos) < Approx0Len)
            {
              idxCmp = i;
              break;
            }
          }
          if (idxCmp > -1)
          {
            bool flag = false;
            for (int i = 0; i < posAry.Count; ++i)
            {
              if (i == idxCmp)
              {
                continue;
              }
              pos = posAry[i];

              if (Distance2D(basePos, pos) < Approx0Len)
              {
                continue;
              }

              rad = Angle2D(basePos, pos, compPos);
              if (System.Math.Abs(System.Math.PI - System.Math.Abs(rad)) < Approx0Ang)
              {
                flag = true;
                break;
              }
              else
              {
                if (System.Math.Abs(rad) > Approx0Ang)
                {
                  if (rad < 0)
                  {
                    flag = true;
                    break;
                  }
                }
              }
            }
            if (flag == true)
            {
              ret = idxCmp;
            }
          }
        }
      }

      // 180°
      if (ret == -1)
      {
        if (idxPi > -1)
        {
          ret = idxPi;
        }
        else
        {
          if (posAry.Count == 1)
          {
            ret = 0;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>隣接点取得</summary>
    /// 
    /// <param name="basePos"             >基準点</param>
    /// <param name="exclPos"             >除外点</param>
    /// <param name="curveInterPosAryAry" >カーブ交点</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2011/12/02 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetRelatedPos(Revit.DB.XYZ basePos,
                                                          Revit.DB.XYZ exclPos,
                                                          Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> curveInterPosAryAry)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      Collections.Generic.IList<Revit.DB.XYZ> nextPosAry = new Collections.Generic.List<Revit.DB.XYZ>();
      for (int i = 0; i < curveInterPosAryAry.Count; ++i)
      {
        for (int j = 0; j < curveInterPosAryAry[i].Count; ++j)
        {
          // 基準点を持つ組み合わせ
          if (Distance2D(basePos, curveInterPosAryAry[i][j]) < Approx0Len)
          {
            // 基準点ではない方の点
            int k = j + 1;
            if (k < curveInterPosAryAry[i].Count)
            {
              nextPosAry.Add(curveInterPosAryAry[i][k]);
            }

            k = j - 1;
            if (k > -1)
            {
              nextPosAry.Add(curveInterPosAryAry[i][k]);
            }
          }
        }
      }
      // 次点
      if (nextPosAry.Count > 0)
      {
        for (int i = 0; i < nextPosAry.Count; ++i)
        {
          if (exclPos == null)
          {
            ret.Add(nextPosAry[i]);
          }
          else
          {
            if (Distance2D(exclPos, nextPosAry[i]) > Approx0Len)
            {
              ret.Add(nextPosAry[i]);
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブ交点の平面カーブ取得</summary>
    /// 
    /// <param name="basePos"             >基準点</param>
    /// <param name="relaPos"             >関連点</param>
    /// <param name="curveInterPosAryAry" >カーブ交点</param>
    /// <param name="height"              >高さ</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history>2011/12/02 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetPlanFaceCurveInterPos(
        Revit.DB.XYZ basePos,
        Revit.DB.XYZ relaPos,
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> curveInterPosAryAry,
        double height)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

      Revit.DB.XYZ posF = basePos;
      Revit.DB.XYZ posB = basePos;
      Revit.DB.XYZ posA = null;
      Revit.DB.XYZ posN = null;

      int cntNum = -1;
      int numMax = 1000;
      bool flagEnd = false;


      Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
      posAry.Add(posF);

      // 隣接点取得
      Collections.Generic.IList<Revit.DB.XYZ> nextPosAry = GetRelatedPos(posF, null, curveInterPosAryAry);

      // 反時計回り点取得
      int idxPos = GetPosReClockwise(posF, relaPos, nextPosAry, true);
      if (idxPos == -1)
      {
        return ret;
      }
      else
      {
        posN = nextPosAry[idxPos];
      }
      posAry.Add(posN);
      posB = posN;
      posA = posF;


      // 次点の候補
      while (flagEnd == false)
      {
        // 制限値越え
        cntNum++;
        if (cntNum > numMax)
        {
          break;
        }

        // 隣接点取得
        posN = null;
        nextPosAry = GetRelatedPos(posB, posA, curveInterPosAryAry);

        // 反時計回り点取得
        idxPos = GetPosReClockwise(posB, posA, nextPosAry, false);
        if (idxPos > -1)
        {
          posN = nextPosAry[idxPos];
        }

        // 次点判定
        if (posN != null)
        {
          posAry.Add(posN);
          posA = posB;
          posB = posN;
          if (Distance2D(posF, posN) < Approx0Len)
          {
            flagEnd = true;
          }
        }
        else
        {
          break;
        }
      }

      // 点を確認
      if (posAry.Count < 3)
      {
        return ret;
      }
      if (Distance2D(posAry[0], posAry[posAry.Count - 1]) > Approx0Len)
      {
        return ret;
      }

      // 線分作成
      bool flagRet = true;
      Collections.Generic.IList<Revit.DB.Curve> retCurves = new Collections.Generic.List<Revit.DB.Curve>();
      for (int i = 1; i < posAry.Count; ++i)
      {
        // 線分距離確認
        Revit.DB.XYZ pos1 = new Revit.DB.XYZ(posAry[i - 1].X, posAry[i - 1].Y, height);
        Revit.DB.XYZ pos2 = new Revit.DB.XYZ(posAry[i].X, posAry[i].Y, height);
        if (Distance2D(pos1, pos2) < Approx0Len)
        {
          flagRet = false;
          break;
        }

        // 他線分の交差確認
        Revit.DB.Curve curve = Revit.DB.Line.CreateBound(pos1, pos2);
        bool flag = true;
        for (int j = 0; j < retCurves.Count; ++j)
        {
          Collections.Generic.IList<Revit.DB.XYZ> interPosAry = new Collections.Generic.List<Revit.DB.XYZ>();
          IntersecCurve2D(curve, retCurves[j], ref interPosAry);
          for (int k = 0; k < interPosAry.Count; ++k)
          {
            if ((Distance2D(pos1, interPosAry[k]) > Approx0Len) &&
                (Distance2D(pos2, interPosAry[k]) > Approx0Len))
            {
              flag = false;
              break;
            }
          }
          if (flag == false)
          {
            break;
          }
        }
        if (flag == true)
        {
          retCurves.Add(curve);
        }
        else
        {
          flagRet = false;
          break;
        }
      }
      if (flagRet == true)
      {
        ret = retCurves;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>カーブ交点の平面カーブ取得</summary>
    /// 
    /// <param name="curveInterPosAryAry" >カーブ交点</param>
    /// <param name="height"              >高さ</param>
    ///
    /// <returns>結果</returns>
    ///           
    /// <history><p>2011/12/02 Created  GSA,Inc. Shinichi Ishii</p>
    ///           <p>2016/05/27 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> GetPlanFaceCurveInterPos(
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> curveInterPosAryAry,
        double height)
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> ret =
          new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

      // カーブ
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> pFaceCurveAryAry =
          new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

      Collections.Generic.IList<Revit.DB.XYZ> pFaceGravityAry = new Collections.Generic.List<Revit.DB.XYZ>();

      for (int i = 0; i < curveInterPosAryAry.Count; ++i)
      {
        // 交点位置
        Collections.Generic.IList<Revit.DB.XYZ> curveInterPosAry = curveInterPosAryAry[i];
        for (int j = 0; j < curveInterPosAry.Count; ++j)
        {
          Revit.DB.XYZ curveInterPos = curveInterPosAry[j];
          Revit.DB.XYZ basePos = curveInterPos;

          int idxN = j + 1;
          if (idxN > curveInterPosAry.Count - 1)
          {
            idxN = j - 1;
          }
          if (idxN < 0)
          {
            continue;
          }
          Revit.DB.XYZ relaPos = curveInterPosAry[idxN];

          // カーブ交点の平面
          Collections.Generic.IList<Revit.DB.Curve> planCurves = GetPlanFaceCurveInterPos(basePos,
                                                                                          relaPos,
                                                                                          curveInterPosAryAry,
                                                                                          height);
          if (planCurves.Count > 0)
          {
            Revit.DB.XYZ gravity = PolygonGravity2D(planCurves);

            // 位置補正(重心が多角形の外の場合)
            gravity = PointAdjustInCurves(gravity, planCurves);

            if (gravity == null)
            {
              continue;
            }
            // 同じ重心かチェック
            bool flag = true;
            for (int k = 0; k < pFaceCurveAryAry.Count; ++k)
            {
              if (Distance2D(gravity, pFaceGravityAry[k]) < Approx0Len)
              {
                flag = false;
                break;
              }
            }
            if (flag == true)
            {
              pFaceCurveAryAry.Add(planCurves);
              pFaceGravityAry.Add(gravity);
            }
          }
        }
      }

      // 形状が重複しているかチェック
      for (int i = 0; i < pFaceCurveAryAry.Count; ++i)
      {
        // カーブ
        Collections.Generic.IList<Revit.DB.Curve> pFaceCurveAry = pFaceCurveAryAry[i];
        Revit.DB.XYZ pFaceGravity = pFaceGravityAry[i];

        // 面積
        double area = System.Math.Abs(GetPolygonArea(pFaceCurveAry));

        // 比較
        bool flag = true;
        for (int j = 0; j < pFaceCurveAryAry.Count; ++j)
        {
          if (i == j)
          {
            continue;
          }
          Collections.Generic.IList<Revit.DB.Curve> pFaceCurveAryTmp = pFaceCurveAryAry[j];
          Revit.DB.XYZ pFaceGravityTmp = pFaceGravityAry[j];
          double areaTmp = System.Math.Abs(GetPolygonArea(pFaceCurveAryTmp));

          // 重心内外判定
          bool isIn = IsPointInPolygon(pFaceCurveAryTmp, pFaceGravityTmp, pFaceGravity, 1);
          if (isIn == true)
          {
            if ((area - areaTmp) > 0.0)
            {
              flag = false;
              break;
            }
          }
        }

        if (flag == true)
        {
          ret.Add(pFaceCurveAry);
        }
      }

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

    /// ================================================================================
    /// <summary>点の3点のなす面上判定</summary>
    /// 
    /// <param name="p" >点</param>
    /// <param name="pA">点A</param>
    /// <param name="pB">点B</param>
    /// <param name="pC">点C</param>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsPointOnPlane(Revit.DB.XYZ p,
                        Revit.DB.XYZ pA,
                        Revit.DB.XYZ pB,
                        Revit.DB.XYZ pC)
    {
      // 戻り値
      bool ret = false;

      // 平面の定数
      double a = 0;
      double b = 0;
      double c = 0;
      double d = 0;

      // 平面の方程式
      GetPlaneEquation(pA,
                       pB,
                       pC,
                       ref a,
                       ref b,
                       ref c,
                       ref d);

      // 平面の法線
      Revit.DB.XYZ normal = new Revit.DB.XYZ(a, b, c);

      // 中心点(重心)
      Revit.DB.XYZ pO = new Revit.DB.XYZ((pA.X + pB.X + pC.X) / 3,
                                         (pA.Y + pB.Y + pC.Y) / 3,
                                         (pA.Z + pB.Z + pC.Z) / 3);

      // 中心点から点へのベクトル
      Revit.DB.XYZ vecO = new Revit.DB.XYZ(p.X - pO.X,
                                           p.Y - pO.Y,
                                           p.Z - pO.Z);

      // 法線との内積
      // 0は面上、正または負は離れている
      double dotN = Naiseki(vecO, normal);

      // 誤差吸収（処理する桁は適宜調整）
      if (System.Math.Abs(dotN) < 0.000001)
      {
        dotN = 0.0;
      }

      // 点が平面上
      if (dotN == 0.0)
      {
        ret = true;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>3点のなす面と線分の交点</summary>
    /// 
    /// <param name="line">線分</param>
    /// <param name="pA"  >点A</param>
    /// <param name="pB"  >点B</param>
    /// <param name="pC"  >点C</param>
    /// <param name="modeExtend">線分延長</param>
    /// 
    /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ GetCrossPoint(Revit.DB.Line line,
                               Revit.DB.XYZ pA,
                               Revit.DB.XYZ pB,
                               Revit.DB.XYZ pC,
                               int modeExtend)
    {
      // 戻り値
      Revit.DB.XYZ ret = null;

      // 平面の定数
      double a = 0;
      double b = 0;
      double c = 0;
      double d = 0;

      // 平面の方程式
      GetPlaneEquation(pA,
                       pB,
                       pC,
                       ref a,
                       ref b,
                       ref c,
                       ref d);

      // 平面の法線
      Revit.DB.XYZ normal = new Revit.DB.XYZ(a, b, c);

      // 中心点(重心)
      Revit.DB.XYZ pO = new Revit.DB.XYZ((pA.X + pB.X + pC.X) / 3,
                                         (pA.Y + pB.Y + pC.Y) / 3,
                                         (pA.Z + pB.Z + pC.Z) / 3);

      // 始点
      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      // 終点
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      if (modeExtend == 1)
      {
        Revit.DB.XYZ vec = new Revit.DB.XYZ(p1.X - p0.X,
                                            p1.Y - p0.Y,
                                            p1.Z - p0.Z);

        p0 = p0 + vec * 1000000;
        p1 = p1 + -vec * 1000000;
      }

      // 中心点から始点へのベクトル
      Revit.DB.XYZ vecO0 = new Revit.DB.XYZ(p0.X - pO.X,
                                            p0.Y - pO.Y,
                                            p0.Z - pO.Z);
      // 中心点から終点へのベクトル
      Revit.DB.XYZ vecO1 = new Revit.DB.XYZ(p1.X - pO.X,
                                            p1.Y - pO.Y,
                                            p1.Z - pO.Z);

      // 法線との内積
      // 0は面上(同じ方向への働きなし)、正は同じ向き、負は逆向き
      double dotN0 = Naiseki(vecO0, normal);
      double dotN1 = Naiseki(vecO1, normal);

      // 誤差吸収（処理する桁は適宜調整）
      if (System.Math.Abs(dotN0) < 0.000001)
      {
        dotN0 = 0.0;
      }
      if (System.Math.Abs(dotN1) < 0.000001)
      {
        dotN1 = 0.0;
      }

      // 両端が平面上
      if (dotN0 == 0.0 && dotN1 == 0.0)
      {
        return ret;
      }
      else
      {
        // 端点が平面の片側ずつ
        if ((dotN0 >= 0.0 && dotN1 <= 0.0) || (dotN0 <= 0.0 && dotN1 >= 0.0))
        {
          // 始点から終点へ
          Revit.DB.XYZ vec01 = new Revit.DB.XYZ(p1.X - p0.X,
                                                p1.Y - p0.Y,
                                                p1.Z - p0.Z);

          // 内積の比 = 距離の比
          double dotRate = System.Math.Abs(dotN0) / (System.Math.Abs(dotN0) + System.Math.Abs(dotN1));

          // 平面との交点
          Revit.DB.XYZ pCross = new Revit.DB.XYZ(p0.X + vec01.X * dotRate,
                                                 p0.Y + vec01.Y * dotRate,
                                                 p0.Z + vec01.Z * dotRate);

          ret = pCross;
        }
        // 両点が平面の同じ側
        else
        {
          return ret;
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>平面の方程式</summary>
    /// 
    /// <param name="pA">点A</param>
    /// <param name="pB">点B</param>
    /// <param name="pC">点C</param>
    /// <param name="a" >定数a</param>
    /// <param name="b" >定数b</param>
    /// <param name="c" >定数c</param>
    /// <param name="d" >定数d</param>
    /// 
    /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetPlaneEquation(Revit.DB.XYZ pA,
                          Revit.DB.XYZ pB,
                          Revit.DB.XYZ pC,
                          ref double a,
                          ref double b,
                          ref double c,
                          ref double d)
    {
      // ax + by + cz + d = 0

      a = (pB.Y - pA.Y) * (pC.Z - pA.Z) - (pC.Y - pA.Y) * (pB.Z - pA.Z);
      b = (pB.Z - pA.Z) * (pC.X - pA.X) - (pC.Z - pA.Z) * (pB.X - pA.X);
      c = (pB.X - pA.X) * (pC.Y - pA.Y) - (pC.X - pA.X) * (pB.Y - pA.Y);
      d = -(a * pA.X + b * pA.Y + c * pA.Z);
    }

    /// ================================================================================
    /// <summary>Cosシータ</summary>
    /// 
    /// <param name="vec1">ベクトル1</param>
    /// <param name="vec2">ベクトル2</param>
    /// 
    /// <history>2016/11/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    double CosSita(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
    {
      // 戻り値
      double ret = 0;

      // 2ベクトルの内積
      double naiseki = Naiseki(vec1, vec2);

      // ベクトルの長さ
      double length1 = System.Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y + vec1.Z * vec1.Z);
      double length2 = System.Math.Sqrt(vec2.X * vec2.X + vec2.Y * vec2.Y + vec2.Z * vec2.Z);

      ret = naiseki / (length1 * length2);

      return ret;
    }

    /// ================================================================================
    /// <summary>内積</summary>
    /// 
    /// <param name="vec1">ベクトル1</param>
    /// <param name="vec2">ベクトル2</param>
    /// 
    /// <history>2016/11/18 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    double Naiseki(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
    {
      return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
    }

    /// ================================================================================
    /// <summary>外積</summary>
    /// 
    /// <param name="vec1">ベクトル1</param>
    /// <param name="vec2">ベクトル2</param>
    /// 
    /// <history>2016/11/18 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.XYZ Gaiseki(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
    {
      return new Revit.DB.XYZ(vec1.Y * vec2.Z - vec1.Z * vec2.Y,
                              vec1.Z * vec2.X - vec1.X * vec2.Z,
                              vec1.X * vec2.Y - vec1.Y * vec2.X);
    }

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>面</summary>
    /// <history>2017/01/10 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Plane BasePlane
    {
      get
      {
        return _Plane;
      }
      set
      {
        _Plane = value;
      }
    }

    /// ================================================================================
    /// <summary>接点誤差</summary>
    /// <history>2011/12/13 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public
    double ToleranceInter
    {
      get
      {
        return _ToleranceInter;
      }
      set
      {
        _ToleranceInter = value;
      }
    }

    #endregion
  }
}
