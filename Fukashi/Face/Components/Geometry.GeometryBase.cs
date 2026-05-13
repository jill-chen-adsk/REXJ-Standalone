using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cl = System.Collections.Generic;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// <summary>JExtCom の基幾何メソッドのスタンドイン。</summary>
  public partial class Geometry
  {
    private const double GeomVertexTol = 1e-6;
    private readonly UIDocument _rvtUidoc;
    private readonly Attribute _CmpAttribute;

    public UIDocument RvtUIDoc => _rvtUidoc;

    public Document RvtDBDoc => _rvtUidoc.Document;

    public double Approx0Len => 1.0e-9;

    public double Approx0Ang => 1.0e-6;

    public Geometry(UIDocument rvtUiDoc, Attribute cmpAttribute)
    {
      _rvtUidoc = rvtUiDoc ?? throw new ArgumentNullException(nameof(rvtUiDoc));
      _CmpAttribute = cmpAttribute ?? throw new ArgumentNullException(nameof(cmpAttribute));
    }

    public double Distance(XYZ p, XYZ q) => p.DistanceTo(q);

    public static XYZ UnitVector(XYZ v)
    {
      double len = v.GetLength();
      if (len < 1e-9)
        return XYZ.Zero;
      return v / len;
    }

    public XYZ UnitVector(XYZ from, XYZ to) => UnitVector(to - from);

    public XYZ CrossProduct(XYZ origin, XYZ p1, XYZ p2) =>
      Gaiseki(p1 - origin, p2 - origin);

    public double DotProduct(XYZ origin, XYZ p1, XYZ p2) =>
      Naiseki(p1 - origin, p2 - origin);

    public Cl.IList<Curve> OptimizeLineVertexNoConvLine(Cl.IList<Curve> curves)
    {
      if (curves == null || curves.Count == 0)
        return curves;

      var output = new Cl.List<Curve>();

      foreach (Curve c in curves)
      {
        if (c == null)
          continue;

        if (output.Count > 0
            && output[output.Count - 1] is Line prev
            && c is Line next
            && SamePoint(prev.GetEndPoint(1), next.GetEndPoint(0))
            && DirectionsColinear(prev.Direction, next.Direction))
        {
          try
          {
            output[output.Count - 1] = Line.CreateBound(prev.GetEndPoint(0), next.GetEndPoint(1));
          }
          catch
          {
            output.Add(c);
          }
        }
        else
        {
          output.Add(c);
        }
      }

      return output;
    }

    private static bool DirectionsColinear(XYZ a, XYZ b)
    {
      if (a == null || b == null || a.IsAlmostEqualTo(XYZ.Zero) || b.IsAlmostEqualTo(XYZ.Zero))
        return false;

      XYZ ua = UnitVector(a);
      XYZ ub = UnitVector(b);
      double dp = Math.Abs(ua.DotProduct(ub));

      return Math.Abs(dp - 1.0) < 1e-8;
    }

    private bool SamePoint(XYZ a, XYZ b)
    {
      return Distance(a, b) < Math.Max(GeomVertexTol, Approx0Len * 1000);
    }

    public Curve GetLeftBottomCurve(Cl.IList<Curve> curves, int mode)
    {
      if (curves == null || curves.Count == 0)
        return null;

      XYZ anchor = AnchorMinXYZ(curves);

      foreach (Curve c in curves)
      {
        if (SamePoint(c.GetEndPoint(0), anchor))
          return c;

        if (SamePoint(c.GetEndPoint(1), anchor))
        {
          try
          {
            return c.CreateReversed();
          }
          catch
          {
          }
        }
      }

      return curves[0];
    }

    private XYZ AnchorMinXYZ(Cl.IList<Curve> curves)
    {
      XYZ anchor = curves[0].GetEndPoint(0);
      foreach (Curve c in curves)
      {
        for (int k = 0; k <= 1; k++)
        {
          XYZ px = c.GetEndPoint(k);

          bool xLess = px.X < anchor.X - Approx0Len;
          bool xyTieLexY = Math.Abs(px.X - anchor.X) < Approx0Len && px.Y < anchor.Y - Approx0Len;
          bool xyzTieLexZ =
            Math.Abs(px.X - anchor.X) < Approx0Len &&
            Math.Abs(px.Y - anchor.Y) < Approx0Len &&
            px.Z < anchor.Z - Approx0Len;

          if (xLess || xyTieLexY || xyzTieLexZ)
          {
            anchor = px;
          }
        }
      }

      return anchor;
    }

    public void GetContinuousCurves(
      Cl.IList<Curve> curves,
      Curve seed,
      ref Cl.IList<Curve> sorted)
    {
      sorted ??= new Cl.List<Curve>();

      sorted.Clear();

      if (curves == null || curves.Count == 0 || seed == null)
        return;

      if (TryRebuildChain(curves, seed, out Cl.List<Curve> builtChain))
      {
        CopyChain(sorted, builtChain);
        return;
      }

      Curve reversedSeed = SeedReversed(seed);
      if (reversedSeed != null && TryRebuildChain(curves, reversedSeed, out builtChain))
      {
        CopyChain(sorted, builtChain);
        return;
      }

      foreach (Curve curve in curves)
      {
        sorted.Add(curve);
      }
    }

    private static void CopyChain(Cl.IList<Curve> sorted, Cl.List<Curve> built)
    {
      foreach (Curve curve in built)
      {
        sorted.Add(curve);
      }
    }

    private bool TryRebuildChain(Cl.IList<Curve> pool, Curve starter, out Cl.List<Curve> chain)
    {
      chain = new Cl.List<Curve>();

      Cl.List<Curve> roster = new();

      foreach (Curve c in pool)
        roster.Add(c);

      if (!ConsumeStarter(roster, starter, out Curve walker))
      {
        return false;
      }

      chain.Add(walker);

      XYZ tip = walker.GetEndPoint(1);

      while (roster.Count > 0)
      {
        Curve continuation = AttachNext(roster, tip);
        if (continuation == null)
        {
          chain.Clear();
          return false;
        }

        roster.Remove(continuation);
        chain.Add(continuation);

        tip = continuation.GetEndPoint(1);
      }

      return chain.Count == pool.Count;
    }

    private Curve AttachNext(Cl.List<Curve> roster, XYZ tip)
    {
      foreach (Curve curve in roster)
      {
        if (SamePoint(tip, curve.GetEndPoint(0)))
          return curve;

        if (SamePoint(tip, curve.GetEndPoint(1)))
        {
          try
          {
            return curve.CreateReversed();
          }
          catch
          {
            return null;
          }
        }
      }

      return null;
    }

    private bool ConsumeStarter(Cl.List<Curve> roster, Curve starter, out Curve oriented)
    {
      for (int idx = 0; idx < roster.Count; ++idx)
      {
        Curve raw = roster[idx];
        Curve orientedMatch = OrientTemplate(starter, raw);
        if (orientedMatch == null)
          continue;

        roster.RemoveAt(idx);
        oriented = orientedMatch;
        return true;
      }

      oriented = null;
      return false;
    }

    private Curve OrientTemplate(Curve needle, Curve raw)
    {
      if (!(needle is Line) || !(raw is Line))
      {
        return ReferenceEquals(needle, raw) ? raw : null;
      }

      Line ln = raw as Line;
      Line na = needle as Line;

      bool forward =
        SamePoint(na.GetEndPoint(0), ln.GetEndPoint(0)) &&
        SamePoint(na.GetEndPoint(1), ln.GetEndPoint(1));
      bool reverse =
        SamePoint(na.GetEndPoint(0), ln.GetEndPoint(1)) &&
        SamePoint(na.GetEndPoint(1), ln.GetEndPoint(0));

      if (!(forward || reverse))
        return null;

      if (forward)
        return raw;

      Curve rev = ReverseCurve(raw);

      return rev ?? raw;
    }

    private static Curve ReverseCurve(Curve c)
    {
      try
      {
        return c.CreateReversed();
      }
      catch
      {
        return null;
      }
    }

    private Curve SeedReversed(Curve seed)
    {
      try
      {
        return seed.CreateReversed();
      }
      catch
      {
        return null;
      }
    }

    public void IntersecCurve(
      Curve curve1,
      Curve curve2,
      ref Cl.IList<XYZ> intersections)
    {
      intersections ??= new Cl.List<XYZ>();

      intersections.Clear();

      if (curve1 == null || curve2 == null)
        return;

      foreach (Curve sa in TessellateToLineSegmentsFace(curve1))
      {
        foreach (Curve sb in TessellateToLineSegmentsFace(curve2))
        {
          if (!(sa is Line la) || !(sb is Line lb))
            continue;

          AddBoundedLineIntersect2DFace(la, lb, intersections);
        }
      }
    }

    private static Cl.IList<Curve> TessellateToLineSegmentsFace(Curve curve)
    {
      if (curve is Line ln)
      {
        return new Cl.List<Curve> { ln };
      }

      Cl.IList<XYZ> tes = curve.Tessellate();
      var list = new Cl.List<Curve>();
      for (int i = 0; i + 1 < tes.Count; i++)
      {
        list.Add(Line.CreateBound(tes[i], tes[i + 1]));
      }
      return list;
    }

    private static void AddBoundedLineIntersect2DFace(Line la, Line lb, Cl.IList<XYZ> intersections)
    {
      XYZ xy = LineIntersectInfinite2DFace(la, lb);

      if (xy == null)
        return;

      if (PointOnBoundedLine2DFace(la, xy) && PointOnBoundedLine2DFace(lb, xy))
      {
        intersections.Add(xy);
      }
    }

    private static XYZ LineIntersectInfinite2DFace(Line line1, Line line2)
    {
      XYZ pos10 = line1.GetEndPoint(0);
      XYZ pos11 = line1.GetEndPoint(1);
      XYZ pos20 = line2.GetEndPoint(0);
      XYZ pos21 = line2.GetEndPoint(1);

      double x1 = pos10.X, y1 = pos10.Y;
      double x2 = pos11.X, y2 = pos11.Y;
      double x3 = pos20.X, y3 = pos20.Y;
      double x4 = pos21.X, y4 = pos21.Y;

      double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

      if (Math.Abs(denom) < GeomVertexTol)
      {
        return null;
      }

      double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom;
      double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom;

      double z = pos10.Z;

      return new XYZ(px, py, z);
    }

    private static bool PointOnBoundedLine2DFace(Line line, XYZ pt)
    {
      XYZ p0 = line.GetEndPoint(0);
      XYZ p1 = line.GetEndPoint(1);
      double xmin = Math.Min(p0.X, p1.X) - 1e-6;
      double xmax = Math.Max(p0.X, p1.X) + 1e-6;
      double ymin = Math.Min(p0.Y, p1.Y) - 1e-6;
      double ymax = Math.Max(p0.Y, p1.Y) + 1e-6;

      return pt.X >= xmin && pt.X <= xmax && pt.Y >= ymin && pt.Y <= ymax;
    }

    public bool IsEqualCurve(Curve curve1, Curve curve2)
    {
      if (curve1 == null || curve2 == null)
        return false;

      XYZ curve1EndPt0 = curve1.GetEndPoint(0),
        curve1EndPt1 = curve1.GetEndPoint(1),
        curve2EndPt0 = curve2.GetEndPoint(0),
        curve2EndPt1 = curve2.GetEndPoint(1);

      bool fwd = SamePoint(curve1EndPt0, curve2EndPt0)
                 && SamePoint(curve1EndPt1, curve2EndPt1);

      bool rev = SamePoint(curve1EndPt0, curve2EndPt1)
                 && SamePoint(curve1EndPt1, curve2EndPt0);

      if (!(fwd || rev))
        return false;

      return Math.Abs(curve1.Length - curve2.Length) < GeomVertexTol;
    }

    public bool IsPointInPolygon(
      Cl.IList<Curve> curves,
      XYZ polygonRefIgnored,
      XYZ testPt,
      int mode)
    {
      _ = polygonRefIgnored;
      _ = mode;

      if (curves == null || curves.Count < 3 || testPt == null)
        return false;

      var verts = new Cl.List<XYZ>();

      foreach (Curve c in curves)
        verts.Add(c.GetEndPoint(0));

      if (verts.Count < 3)
        return false;

      XYZ planeOrigin = verts[0];
      XYZ normalAcc = XYZ.Zero;

      int n = verts.Count;
      for (int i = 1; i + 1 < n; ++i)
      {
        XYZ vi = verts[i] - planeOrigin;
        XYZ vj = verts[i + 1] - planeOrigin;

        XYZ cross = vi.CrossProduct(vj);
        if (cross.GetLength() > GeomVertexTol)
          normalAcc = cross;
      }

      if (normalAcc.GetLength() < GeomVertexTol && n >= 3)
      {
        normalAcc = Gaiseki(verts[2] - verts[0], verts[1] - verts[0]);
      }

      XYZ uz = UnitVector(normalAcc);
      if (uz.IsAlmostEqualTo(XYZ.Zero))
      {
        return false;
      }

      XYZ ux = UnitVector(verts[1] - planeOrigin);
      if (ux.IsAlmostEqualTo(XYZ.Zero))
      {
        return false;
      }

      XYZ uy = uz.CrossProduct(ux);
      uy = UnitVector(uy);
      if (uy.IsAlmostEqualTo(XYZ.Zero))
      {
        return false;
      }

      double tx = Naiseki(testPt - planeOrigin, ux);
      double ty = Naiseki(testPt - planeOrigin, uy);

      double[] uxArray = new double[n],
        uyArray = new double[n];

      for (int i = 0; i < n; ++i)
      {
        XYZ rel = verts[i] - planeOrigin;

        uxArray[i] = Naiseki(rel, ux);
        uyArray[i] = Naiseki(rel, uy);
      }

      bool inside = false;

      for (int ii = 0, jj = n - 1; ii < n; jj = ii++)
      {
        double yi = uyArray[ii],
          yj = uyArray[jj],
          xi = uxArray[ii],
          xj = uxArray[jj];
        double denom = yj - yi;

        if (Math.Abs(denom) < 1e-12)
          denom = denom >= 0 ? 1e-12 : -1e-12;

        if (((yi > ty) != (yj > ty)) && tx < ((xj - xi) * (ty - yi)) / denom + xi)
        {
          inside = !inside;
        }
      }

      return inside;
    }

    public void GetSolidElem(Element elem, ref Cl.IList<Solid> solidList)
    {
      GetElemSolid(elem, ref solidList);
    }
  }
}
