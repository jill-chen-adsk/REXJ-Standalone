using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    public static class TopoUtils
    {
        public static bool IsNormalUpward(this MeshTriangle tri)
        {
            XYZ p0 = MeshTriangleAccess.GetVertex(tri, 0);
            XYZ p1 = MeshTriangleAccess.GetVertex(tri, 1);
            XYZ p2 = MeshTriangleAccess.GetVertex(tri, 2);

            XYZ v1 = p1 - p0;
            XYZ v2 = p2 - p0;

            XYZ normal = v1.CrossProduct(v2);

            normal = normal.Normalize();

            return normal.Z > 0;
        }

        public static bool IsPointInside(this MeshTriangle tri, XYZ p)
        {
            XYZ t0 = MeshTriangleAccess.GetVertex(tri, 0);
            XYZ t1 = MeshTriangleAccess.GetVertex(tri, 1);
            XYZ t2 = MeshTriangleAccess.GetVertex(tri, 2);

            var denominator = ((t1.Y - t2.Y) * (t0.X - t2.X) + (t2.X - t1.X) * (t0.Y - t2.Y));
            var a = ((t1.Y - t2.Y) * (p.X - t2.X) + (t2.X - t1.X) * (p.Y - t2.Y)) / denominator;
            var b = ((t2.Y - t0.Y) * (p.X - t2.X) + (t0.X - t2.X) * (p.Y - t2.Y)) / denominator;
            var c = 1 - a - b;
            return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
        }
    }
}
