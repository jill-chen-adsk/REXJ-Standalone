using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class Geometry
    {
        private UIDocument _rvtUIDoc;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public double UnitCoe => 304.8;

        public double Approx0Len => 0.001;

        public Transform GetElemTransform(FamilyInstance familyInstance)
        {
            return familyInstance.GetTransform();
        }

        public double Distance2D(XYZ p1, XYZ p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void GetPoints(GeometryElement geomElem, ref IList<XYZ> points)
        {
            if (geomElem == null) return;
            foreach (GeometryObject gObj in geomElem)
            {
                if (gObj is Solid solid)
                {
                    GetPoints(solid, ref points);
                }
                else if (gObj is GeometryInstance gInst)
                {
                    GetPoints(gInst.SymbolGeometry, ref points);
                }
                else if (gObj is Mesh mesh)
                {
                    GetPoints(mesh, ref points);
                }
                else if (gObj is Curve curve)
                {
                    GetPoints(curve, ref points);
                }
                else if (gObj is Edge edge)
                {
                    GetPoints(edge, ref points);
                }
            }
        }

        private void GetPoints(Solid solid, ref IList<XYZ> points)
        {
            if (solid == null) return;
            foreach (Face face in solid.Faces)
            {
                GetPoints(face, ref points);
            }
            foreach (Edge edge in solid.Edges)
            {
                GetPoints(edge, ref points);
            }
        }

        private void GetPoints(Face face, ref IList<XYZ> points)
        {
            if (face == null) return;
            var mesh = face.Triangulate();
            if (mesh != null)
                GetPoints(mesh, ref points);
        }

        private void GetPoints(Mesh mesh, ref IList<XYZ> points)
        {
            if (mesh == null) return;
            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                points.Add(triangle.get_Vertex(0));
                points.Add(triangle.get_Vertex(1));
                points.Add(triangle.get_Vertex(2));
            }
        }

        private void GetPoints(Curve curve, ref IList<XYZ> points)
        {
            if (curve == null) return;
            var pts = curve.Tessellate();
            foreach (var pt in pts)
                points.Add(pt);
        }

        private void GetPoints(Edge edge, ref IList<XYZ> points)
        {
            if (edge == null) return;
            var pts = edge.Tessellate();
            foreach (var pt in pts)
                points.Add(pt);
        }
    }
}
