using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public class Geometry
    {
        protected readonly UIDocument _rvtUIDoc;

        private const double Approx0LenDefault = 1e-6;
        private const double PlanElevationZ = 0.0;

        public Geometry(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
        }

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public double Approx0Len => Approx0LenDefault;

        public double UnitCoe
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public double UnitCoeM2
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public double UnitCoeM3
        {
            get
            {
                var fmt = RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Volume).GetUnitTypeId();
                return UnitUtils.ConvertFromInternalUnits(1.0, fmt);
            }
        }

        public bool IsImperial
        {
            get
            {
                try { return RvtDBDoc.DisplayUnitSystem == DisplayUnit.IMPERIAL; }
                catch { return false; }
            }
        }

        public ForgeTypeId LengthUnitTypeId =>
            RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();

        public ForgeTypeId AreaUnitTypeId =>
            RvtDBDoc.GetUnits().GetFormatOptions(SpecTypeId.Area).GetUnitTypeId();

        /// <summary>Convert Revit internal length (feet) to project length display units.</summary>
        public double ConvertInternalLengthToDisplay(double internalLength) =>
            UnitUtils.ConvertFromInternalUnits(internalLength, LengthUnitTypeId);

        /// <summary>
        /// Align a length with project display units when it was stored in meters (e.g. room volume/area)
        /// while a reference length such as head height is already in display units (e.g. mm).
        /// </summary>
        public double ResolveDisplayLength(double length, double referenceLength)
        {
            if (length <= 0.0 || referenceLength <= 0.0)
                return length;

            double oneMeterInDisplay = UnitUtils.ConvertFromInternalUnits(
                UnitUtils.ConvertToInternalUnits(1.0, UnitTypeId.Meters), LengthUnitTypeId);

            if (referenceLength > 200.0
                && length < 50.0 * oneMeterInDisplay
                && referenceLength / length > 50.0)
            {
                return UnitUtils.ConvertFromInternalUnits(
                    UnitUtils.ConvertToInternalUnits(length, UnitTypeId.Meters), LengthUnitTypeId);
            }

            return length;
        }

        /// <summary>Convert a millimeter value (Japanese code constants) to project length display units.</summary>
        public double FromMillimeters(double millimeters) =>
            UnitUtils.ConvertFromInternalUnits(
                UnitUtils.ConvertToInternalUnits(millimeters, UnitTypeId.Millimeters),
                LengthUnitTypeId);

        /// <summary>Convert project display length to meters (Japanese code coefficients use meters).</summary>
        public double DisplayLengthToMeters(double displayLength)
        {
            double internalFeet = UnitUtils.ConvertToInternalUnits(displayLength, LengthUnitTypeId);
            return UnitUtils.ConvertFromInternalUnits(internalFeet, UnitTypeId.Meters);
        }

        /// <summary>Convert width×height in project display length units to project display area units.</summary>
        public double DisplayLengthProductToDisplayArea(double widthDisplay, double heightDisplay)
        {
            double areaFactor = UnitCoeM2 / (UnitCoe * UnitCoe);
            return widthDisplay * heightDisplay * areaFactor;
        }

        public string LengthUnitLabel => GetUnitLabel(LengthUnitTypeId);

        public string AreaUnitLabel => GetUnitLabel(AreaUnitTypeId);

        private static string GetUnitLabel(ForgeTypeId unitTypeId)
        {
            if (unitTypeId == UnitTypeId.Millimeters) return "mm";
            if (unitTypeId == UnitTypeId.Centimeters) return "cm";
            if (unitTypeId == UnitTypeId.Meters) return "m";
            if (unitTypeId == UnitTypeId.Feet) return "ft";
            if (unitTypeId == UnitTypeId.FeetFractionalInches) return "ft";
            if (unitTypeId == UnitTypeId.Inches) return "in";
            if (unitTypeId == UnitTypeId.SquareMeters) return "m\u00B2";
            if (unitTypeId == UnitTypeId.SquareFeet) return "ft\u00B2";
            if (unitTypeId == UnitTypeId.SquareMillimeters) return "mm\u00B2";
            return unitTypeId.TypeId;
        }

        public Transform GetElemTransform(FamilyInstance familyInstance)
        {
            try { return familyInstance.GetTotalTransform(); }
            catch { return Transform.Identity; }
        }

        public double Distance2D(XYZ p1, XYZ p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public double Distance(XYZ a, XYZ b)
        {
            return a.DistanceTo(b);
        }

        public void IntersecCurve2D(Curve c1, Curve c2, ref XYZ interPos)
        {
            if (!TryIntersectSegments2D(c1, c2, out XYZ hit))
                return;

            interPos = hit;
        }

        /// <summary>
        /// Line-segment intersection in plan (XY). Returns false when segments are parallel or do not cross.
        /// </summary>
        public bool TryIntersectSegments2D(Curve c1, Curve c2, out XYZ intersection)
        {
            intersection = null;
            if (c1 == null || c2 == null)
                return false;

            XYZ p1 = c1.GetEndPoint(0);
            XYZ p2 = c1.GetEndPoint(1);
            XYZ p3 = c2.GetEndPoint(0);
            XYZ p4 = c2.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y, x4 = p4.X, y4 = p4.Y;
            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < Approx0Len)
                return false;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / denom;
            if (t < -Approx0Len || t > 1.0 + Approx0Len || u < -Approx0Len || u > 1.0 + Approx0Len)
                return false;

            intersection = new XYZ(x1 + t * (x2 - x1), y1 + t * (y2 - y1), p1.Z);
            return true;
        }

        /// <summary>
        /// Tessellate a boundary curve into plan-view line segments at a fixed elevation.
        /// </summary>
        public IList<Line> TessellatePlanSegments(Curve curve, double z)
        {
            var segments = new List<Line>();
            if (curve == null)
                return segments;

            IList<XYZ> points = curve.Tessellate();
            if (points == null || points.Count < 2)
                return segments;

            for (int i = 0; i < points.Count - 1; ++i)
            {
                XYZ start = new XYZ(points[i].X, points[i].Y, z);
                XYZ end = new XYZ(points[i + 1].X, points[i + 1].Y, z);
                if (start.DistanceTo(end) <= Approx0Len)
                    continue;

                segments.Add(Line.CreateBound(start, end));
            }

            return segments;
        }

        /// <summary>
        /// Ray/segment intersection in plan (XY). Ray is infinite in the forward direction.
        /// </summary>
        public bool TryIntersectRaySegment2D(XYZ rayOrigin, XYZ rayDir, Curve segment, out XYZ intersection)
        {
            intersection = null;
            if (rayOrigin == null || rayDir == null || segment == null)
                return false;

            double rayDx = rayDir.X;
            double rayDy = rayDir.Y;
            if (Math.Abs(rayDx) < Approx0Len && Math.Abs(rayDy) < Approx0Len)
                return false;

            XYZ p1 = rayOrigin;
            XYZ p2 = new XYZ(rayOrigin.X + rayDx, rayOrigin.Y + rayDy, rayOrigin.Z);
            XYZ p3 = segment.GetEndPoint(0);
            XYZ p4 = segment.GetEndPoint(1);

            double x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            double x3 = p3.X, y3 = p3.Y, x4 = p4.X, y4 = p4.Y;
            double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denom) < Approx0Len)
                return false;

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
            double u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / denom;
            if (t < -Approx0Len || u < -Approx0Len || u > 1.0 + Approx0Len)
                return false;

            intersection = new XYZ(x1 + t * (x2 - x1), y1 + t * (y2 - y1), rayOrigin.Z);
            return true;
        }

        /// <summary>
        /// Nearest plan-view hit from a ray origin toward rayEnd across property-line curves.
        /// Uses 2D segment intersection so arcs and Z offsets on the boundary are handled safely.
        /// </summary>
        public double GetNearestPlanRayIntersection(XYZ origin, XYZ rayEnd, IList<Curve> curves)
        {
            if (origin == null || rayEnd == null || curves == null || curves.Count == 0)
                return 0.0;

            double z = PlanElevationZ;
            double rayDx = rayEnd.X - origin.X;
            double rayDy = rayEnd.Y - origin.Y;
            double rayLen = Math.Sqrt(rayDx * rayDx + rayDy * rayDy);
            if (rayLen <= Approx0Len)
                return 0.0;

            XYZ rayDir = new XYZ(rayDx / rayLen, rayDy / rayLen, 0.0);
            XYZ rayOrigin = new XYZ(origin.X, origin.Y, z);

            double minDist = 0.0;
            foreach (Curve curve in curves)
            {
                foreach (Line segment in TessellatePlanSegments(curve, z))
                {
                    if (!TryIntersectRaySegment2D(rayOrigin, rayDir, segment, out XYZ hit))
                        continue;

                    double dist = Distance2D(rayOrigin, hit);
                    if (dist <= Approx0Len)
                        continue;

                    if (minDist == 0.0 || dist < minDist)
                        minDist = dist;
                }
            }

            return minDist;
        }

        /// <summary>
        /// All plan-view boundary curves for a property line (full loop, not a single LocationCurve).
        /// </summary>
        public IList<Curve> GetPropertyLineCurves(PropertyLine propLine)
        {
            var list = new List<Curve>();
            if (propLine == null)
                return list;

            try
            {
                IList<CurveLoop> boundaries = propLine.GetBoundary();
                if (boundaries != null)
                {
                    foreach (CurveLoop loop in boundaries)
                    {
                        if (loop == null)
                            continue;

                        foreach (Curve curve in loop)
                        {
                            if (curve != null)
                                list.Add(curve);
                        }
                    }
                }
            }
            catch
            {
                // Fall back to LocationCurve when GetBoundary is unavailable.
            }

            if (list.Count == 0 && propLine.Location is LocationCurve lc && lc.Curve != null)
                list.Add(lc.Curve);

            if (list.Count == 0)
                AppendCurvesFromElementGeometry(propLine, list);

            return list;
        }

        /// <summary>
        /// Collect property-line boundary curves from the host document and any loaded links.
        /// </summary>
        public IList<Curve> GetAllPropertyLineCurves()
        {
            return GetAllPropertyLineCurves(RvtDBDoc);
        }

        public IList<Curve> GetAllPropertyLineCurves(Document doc)
        {
            var list = new List<Curve>();
            if (doc == null)
                return list;

            AppendPropertyLinesFromDocument(doc, Transform.Identity, list);

            foreach (RevitLinkInstance link in new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>())
            {
                Document linkDoc = link.GetLinkDocument();
                if (linkDoc == null)
                    continue;

                AppendPropertyLinesFromDocument(linkDoc, link.GetTotalTransform(), list);
            }

            return list;
        }

        private void AppendPropertyLinesFromDocument(Document doc, Transform toHost, IList<Curve> list)
        {
            foreach (PropertyLine propLine in new FilteredElementCollector(doc)
                .OfClass(typeof(PropertyLine))
                .Cast<PropertyLine>())
            {
                IList<Curve> curves = GetPropertyLineCurves(propLine);
                for (int i = 0; i < curves.Count; ++i)
                {
                    Curve curve = curves[i];
                    if (curve == null)
                        continue;

                    list.Add(toHost.IsIdentity ? curve : curve.CreateTransformed(toHost));
                }
            }
        }

        private static void AppendCurvesFromElementGeometry(Element element, IList<Curve> list)
        {
            var options = new Options
            {
                ComputeReferences = false,
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = true
            };

            GeometryElement geometry = element.get_Geometry(options);
            if (geometry == null)
                return;

            AppendGeometryCurves(geometry, list);
        }

        private static void AppendGeometryCurves(GeometryElement geometry, IList<Curve> list)
        {
            foreach (GeometryObject geometryObject in geometry)
            {
                if (geometryObject is Curve curve && curve.Length > Approx0LenDefault)
                {
                    list.Add(curve);
                    continue;
                }

                if (geometryObject is GeometryInstance geometryInstance)
                {
                    GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();
                    if (instanceGeometry != null)
                        AppendGeometryCurves(instanceGeometry, list);
                }
            }
        }

        /// <summary>
        /// Merge boundary curves from every property line in the project.
        /// </summary>
        public IList<Curve> GetPropertyLineCurves(IEnumerable<PropertyLine> propLines)
        {
            var list = new List<Curve>();
            if (propLines == null)
                return list;

            foreach (PropertyLine propLine in propLines)
            {
                IList<Curve> curves = GetPropertyLineCurves(propLine);
                for (int i = 0; i < curves.Count; ++i)
                    list.Add(curves[i]);
            }

            return list;
        }

        public IList<Curve> GetCurveElem(PropertyLine propLine) => GetPropertyLineCurves(propLine);

        public void GetCurvesBound(IList<Curve> curves, ref BoundingBoxXYZ bbox)
        {
            if (curves == null || curves.Count == 0)
            {
                bbox = null;
                return;
            }

            double minx = double.MaxValue, miny = double.MaxValue, minz = double.MaxValue;
            double maxx = double.MinValue, maxy = double.MinValue, maxz = double.MinValue;

            foreach (var c in curves)
            {
                for (int i = 0; i <= 1; i++)
                {
                    var p = c.GetEndPoint(i);
                    minx = Math.Min(minx, p.X); miny = Math.Min(miny, p.Y); minz = Math.Min(minz, p.Z);
                    maxx = Math.Max(maxx, p.X); maxy = Math.Max(maxy, p.Y); maxz = Math.Max(maxz, p.Z);
                }
            }

            bbox = new BoundingBoxXYZ
            {
                Min = new XYZ(minx, miny, minz),
                Max = new XYZ(maxx, maxy, maxz)
            };
        }
    }
}
