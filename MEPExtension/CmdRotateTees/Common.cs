// ko-mimura (2019/09/26)
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace MEPCommon
{
	// ----------------------------------
	// Unit Vector (2d)
	// ----------------------------------
	public class UnitUV : UV
	{
		private static UV _normal(UV v) {
			if (v.GetLength() < MepCommon.m_ShortCurveTolerance)
				throw new System.Exception("UnitXYZ::UnitXYZ() error");
			return v.Normalize();
		}
		public UnitUV(UV v)
			: base(_normal(v).U, _normal(v).V) {
		}
		public UnitUV(double u, double v)
			: base(_normal(new UV(u, v)).U, _normal(new UV(u, v)).V) {
		}
	}
	// ----------------------------------
	// Unit Vector (3d)
	// ----------------------------------
	public class UnitXYZ : XYZ
	{
		private static XYZ _normal(XYZ v)
		{
			if (v.GetLength() < MepCommon.m_ShortCurveTolerance)
				throw new System.Exception("UnitXYZ::UnitXYZ() error");
			return v.Normalize();
		}
		public UnitXYZ(XYZ v)
			:base(_normal(v).X, _normal(v).Y, _normal(v).Z)
		{
		}
		public UnitXYZ(double x, double y, double z)
			: base(_normal(new XYZ(x, y, z)).X, _normal(new XYZ(x, y, z)).Y, _normal(new XYZ (x, y, z)).Z) {
			}
	}
	// ----------------------------------
	// Ray
	// ----------------------------------
	public class Ray
	{
		public UnitXYZ m_v;
		public XYZ m_pt;
		public double GetDistance(XYZ pt)
		{
			var perp = MepCommon.GetPerp(m_pt, m_v, pt);
			return perp.DistanceTo(pt);
		}
		public bool equalTo(Ray ray)
		{
			const double tol = 10e-7;
			double dist = GetDistance(ray.m_pt);
			if (dist > tol)
				return false;
			var dot = m_v.DotProduct(ray.m_v);
			return MepCommon.Equal(dot, 1.0);
		}
		public Ray(XYZ pt, UnitXYZ v)
		{
			m_v = v;
			m_pt = pt;
		}
	}
	// ----------------------------------
	// ExtentUV
	// ----------------------------------
	public class ExtentUV {
		public double m_min_u;
		public double m_min_v;
		public double m_max_u;
		public double m_max_v;
		public BoundingBoxUV GetBbox() 
		{
			var bbox = new BoundingBoxUV(); {
				bbox.Min = Min;
				bbox.Max = Max;
			}
			return bbox;
		}
		public List<UV> GetPts() 
		{
			var pts = new List<UV>();
			{
				var diff = Max - Min;
				pts.Add(Min);
				pts.Add(Min + new UV(diff.U, 0));
				pts.Add(Min + new UV(diff.U, diff.V));
				pts.Add(Min + new UV(0, diff.V));
			}
			return pts;
		}
		public bool IsIn(UV pt) 
		{
			var x = Min.U <= pt.U && pt.U <= Max.U;
			var y = Min.V <= pt.V && pt.V <= Max.V;
			return x && y;
		}
		public double GetW() 
		{
			return Math.Abs(m_max_u - m_min_u);
		}
		public double GetH() 
		{
			return Math.Abs(m_max_v - m_min_v);
		}
		public UV Min 
		{
			get {
				return new UV(m_min_u, m_min_v);
			}
		}
		public UV Max 
		{
			get {
				return new UV(m_max_u, m_max_v);
			}
		}
		public void Add(UV uv) 
		{
			if (uv.U < m_min_u)
				m_min_u = uv.U;
			if (uv.V < m_min_v)
				m_min_v = uv.V;
			if (uv.U > m_max_u)
				m_max_u = uv.U;
			if (uv.V > m_max_v)
				m_max_v = uv.V;
		}
		public void Init()
		{
			m_min_u = double.MaxValue;
			m_min_v = double.MaxValue;
			m_max_u = double.MinValue;
			m_max_v = double.MinValue;
		}
		public ExtentUV(BoundingBoxUV bbox) 
		{
			Init();
			Add(bbox.Min);
			Add(bbox.Max);
		}
		public ExtentUV() 
		{
			Init();
		}
	}
	// ----------------------------------
	// ExtentXYZ
	// ----------------------------------
	public class ExtentXYZ
	{
		public double m_min_x;
		public double m_min_y;
		public double m_min_z;
		public double m_max_x;
		public double m_max_y;
		public double m_max_z;
		public BoundingBoxXYZ GetBbox() 
		{
			var bbox = new BoundingBoxXYZ(); {
				bbox.Min = Min;
				bbox.Max = Max;
			}
			return bbox;
		}
		public bool IsIn(XYZ pt) 
		{
			var x = Min.X <= pt.X && pt.X <= Max.X;
			var y = Min.Y <= pt.Y && pt.Y <= Max.Y;
			var z = Min.Z <= pt.Z && pt.Z <= Max.Z;
			return x && y && z;
		}
		public List<XYZ> GetPts() 
		{
			var pts = new List<XYZ>(); {
				var diff = Max - Min;
				pts.Add(Min);
				pts.Add(Min + new XYZ(diff.X, 0, 0));
				pts.Add(Min + new XYZ(diff.X, diff.Y, 0));
				pts.Add(Min + new XYZ(0, diff.Y, 0));
				pts.Add(Min + new XYZ(0, 0, diff.Z));
				pts.Add(Min + new XYZ(diff.X, 0, diff.Z));
				pts.Add(Min + new XYZ(diff.X, diff.Y, diff.Z));
				pts.Add(Min + new XYZ(0, diff.Y, diff.Z));
			}
			return pts;
		}
		public double GetW() 
		{
			return Math.Abs(m_max_x - m_min_x);
		}
		public double GetH() 
		{
			return Math.Abs(m_max_y - m_min_y);
		}
		public double GetD() 
		{
			return Math.Abs(m_max_z - m_min_z);
		}
		public XYZ Min 
		{
			get {
				return new XYZ(m_min_x, m_min_y, m_min_z);
			}
		}
		public XYZ Max 
		{
			get {
				return new XYZ(m_max_x, m_max_y, m_max_z);
			}
		}
		public void Add(XYZ pt) 
		{
			if (pt.X < m_min_x)
				m_min_x = pt.X;
			if (pt.Y < m_min_y)
				m_min_y = pt.Y;
			if (pt.Z < m_min_z)
				m_min_z = pt.Z;
			if (pt.X > m_max_x)
				m_max_x = pt.X;
			if (pt.Y > m_max_y)
				m_max_y = pt.Y;
			if (pt.Z > m_max_z)
				m_max_z = pt.Z;
		}
		public void Init()
		{
			m_min_x = double.MaxValue;
			m_min_y = double.MaxValue;
			m_min_z = double.MaxValue;
			m_max_x = double.MinValue;
			m_max_y = double.MinValue;
			m_max_z = double.MinValue;
		}
		public ExtentXYZ(BoundingBoxXYZ bbox) 
		{
			Init();
			Add(bbox.Min);
			Add(bbox.Max);
		}
		public ExtentXYZ() 
		{
			Init();
		}
	}
	// ----------------------------------
	// いろいろ雑多
	// ----------------------------------
	public class MepCommon
	{
		public const double tol = 1e-6;
		public const double m_far = 1000.0;
		public static double m_AngleTolerance = 1e-6;
		public static double m_VertexTolerance = 1e-6;
		public static double m_ShortCurveTolerance = 1e-6;
		public static ExternalCommandData m_commandData;
		public static UIDocument m_uidoc;
		public static UIApplication m_uiapp;
		public static Document m_doc;
		public static Autodesk.Revit.ApplicationServices.Application m_app;
		protected static List<Solid> GetSolids(FamilyInstance inst) 
		{
			ASSERT(inst != null);
			List<Solid> solids = new List<Solid>();
			List<GeometryObject> list = null;
			{
				Options options = new Options();
				{
					options.DetailLevel = ViewDetailLevel.Fine;
				}
				var geo = inst.GetOriginalGeometry(options);
				list = ConvEnumeratorToList(geo.GetEnumerator());
			}
			foreach (GeometryObject gObj in list)
			{
				if (gObj is Solid)
				{
					Solid s = gObj as Solid;
					if (s.Volume > 0)
						solids.Add(s);
				}
			}
			return solids;
		}
		protected static List<Solid> GetSolids(FabricationPart fab) 
		{
			ASSERT(fab != null);
			List<Solid> solids = new List<Solid>();
			var trans = GetTransform(fab);
			List<GeometryObject> list = null;
			{
				Options options = new Options();
				var geoElement = fab.get_Geometry(options);
				geoElement = geoElement.GetTransformed(trans.Inverse);
				list = ConvEnumeratorToList(geoElement.GetEnumerator());
			}
			foreach (GeometryObject gObj in list)
			{
				if (gObj is Solid)
				{
					Solid s = gObj as Solid;
					if (s.Volume > 0)
						solids.Add(s);
				}
				else if (gObj is GeometryInstance)
				{
					var gI = gObj as GeometryInstance;
					var geoSet = gI.GetInstanceGeometry();
					var geoSetList = ConvEnumeratorToList(geoSet.GetEnumerator());
					foreach (GeometryObject obj2 in geoSetList)
					{
						if (obj2 is Solid)
						{
							Solid s = obj2 as Solid;
							if (s.Volume > 0)
								solids.Add(s);
						}
					}
				}
			}
			return solids;
		}
		public static List<Solid> GetSolids(Element ele) 
		{
			var inst = ele as FamilyInstance;
			var fab = ele as FabricationPart;
			if (inst != null) {
				return MepCommon.GetSolids(inst);
			} else if (fab != null) {
				return MepCommon.GetSolids(fab);
			}
			return new List<Solid>();
		}
		static public bool IsParallel(UnitXYZ v1, UnitXYZ v2) {
			var dot = v1.DotProduct(v2);
			return MepCommon.Equal(dot, 1.0);
		}
		static public bool IsParallel(XYZ v1, XYZ v2) {
			return IsParallel(new UnitXYZ(v1), new UnitXYZ(v2));
		}
		static public bool IsBiParallel(XYZ v1, XYZ v2) 
		{
			return IsBiParallel(new UnitXYZ(v1), new UnitXYZ(v2));
		}
		static public bool IsBiParallel(UnitXYZ v1, UnitXYZ v2) 
		{
			var dot = v1.DotProduct(v2);
			return MepCommon.Equal(Math.Abs(dot), 1.0);
		}
		static public XYZ GetPerp(XYZ ptRay, UnitXYZ dirRay, XYZ pt)
		{
			var dir = pt - ptRay;
			var dot = dirRay.DotProduct(dir);
			return ptRay + dirRay * dot;
		}
		public static void Init(ExternalCommandData commandData) {
			m_commandData = commandData;
			m_uiapp = commandData.Application;
			m_uidoc = m_uiapp.ActiveUIDocument;
			m_app = m_uiapp.Application;
			m_doc = m_uidoc.Document;
			m_AngleTolerance = m_app.AngleTolerance;
			m_VertexTolerance = m_app.VertexTolerance;
			m_ShortCurveTolerance = m_app.ShortCurveTolerance;
		}
		static public bool Equal(double t1, double t2, double tol2 = MepCommon.tol) {
			return Math.Abs(t1 - t2) < tol2;
		}
		static public bool Equal(ElementId t1, ElementId t2) {
			return t1.Equals(t2);
		}
		static public bool Equal(UV t1, UV t2, double tol2) {
			return t1.DistanceTo(t2) < tol2;
		}
		static public bool Equal(UV t1, UV t2) {
			return Equal(t1, t2, m_VertexTolerance);
		}
		static public bool Equal(XYZ t1, XYZ t2, double tol2) {
			return t1.DistanceTo(t2) < tol2;
		}
		static public bool Equal(XYZ t1, XYZ t2) {
			return Equal(t1, t2, m_VertexTolerance);
		}

		static public void ASSERT(bool b, string message = "") {
			if (message == "")
				System.Diagnostics.Debug.Assert(b);
			else
				System.Diagnostics.Debug.Assert(b, message);
		}
		static public List<T> ConvEnumeratorToList<T>(IEnumerator<T> er) {
			var list = new List<T>(); {
				while (er.MoveNext())
					list.Add(er.Current);
			}
			return list;
		}
		static public List<T> ConvCollectionToList<T>(ICollection<T> ids) {
			var list = new List<T>(); {
				foreach (T id in ids)
					list.Add(id);
			}
			return list;
		}
		static public double D2R(double angle) {
			return (double)(angle * System.Math.PI / 180.0);
		}
		public static View3D GetActiveViewAs3D() {
			return MepCommon.m_uidoc.ActiveView as View3D;
		}
		public static ViewSection GetActiveViewAsSection() {
			return MepCommon.m_uidoc.ActiveView as ViewSection;
		}
		public static ViewPlan GetActiveViewAsPlan() {
			return MepCommon.m_uidoc.ActiveView as ViewPlan;
		}
		public static XYZ GetNearestPt(List<XYZ>pts, XYZ target) {
			ASSERT(pts.Count > 0);
			XYZ ret = null;
			double max_dist = double.MaxValue;
			foreach (XYZ pt in pts) {
				double dist = pt.DistanceTo(target);
				if (dist < max_dist) {
					max_dist = dist;
					ret = pt;
				}
			}
			return ret;
		}
		public static XYZ GetFartestPt(List<XYZ> pts, XYZ target) {
			ASSERT(pts.Count > 0);
			XYZ ret = null;
			double min_dist = double.MinValue;
			foreach (XYZ pt in pts) {
				double dist = pt.DistanceTo(target);
				if (dist > min_dist) {
					min_dist = dist;
					ret = pt;
				}
			}
			return ret;
		}
		static public int Find(List<Connector> list, Connector target) {
			for (int i = 0; i < list.Count; i++) {
				if (list[i].Id == target.Id)
					return i;
			}
			return -1;
		}
		public static List<Element> _findElementsByType(Type type) {
			FilteredElementCollector col = null; {
				col = new FilteredElementCollector(MepCommon.m_doc).OfClass(type);
			}
			List<Element> lists = new List<Element>();
			foreach (var ele in col.ToElements())
				lists.Add(ele);
			return lists;
		}
		static public KeyValuePair<Type1, Type2> make_pair<Type1, Type2>(Type1 p1, Type2 p2) {
			return new KeyValuePair<Type1, Type2>(p1, p2);
		}
		static public int FindByKey<Type1, Type2>(List<KeyValuePair<Type1, Type2>> list, Type1 target) {
			for (int i = 0; i < list.Count; i++)
				if (object.Equals(list[i].Key, target))
					return i;
			return -1;
		}
		static public int FindByValue<Type1, Type2>(List<KeyValuePair<Type1, Type2>> list, Type2 target) {
			for (int i = 0; i < list.Count; i++)
				if (object.Equals(list[i].Value, target))
					return i;
			return -1;
		}
		static public UnitXYZ GetDir(Curve c) {
			var deriv = c.ComputeDerivatives(0, true);
			return new UnitXYZ(deriv.BasisX.Normalize());
		}
		static public Transform GetTransform(Element e) {
			if (e is FamilyInstance) {
				var f = e as FamilyInstance;
				return f.GetTransform();
			}
			if (e is FabricationPart) {
				var f = e as FabricationPart;
				return f.GetTransform();
			}
			MepCommon.ASSERT(false, "Common::GetTransform()");
			return null;
		}
		static public string GetTag(FabricationPart fab) {
			var param = fab.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			var comments = param.AsString();
			return comments;
		}
		static public string GetTag(FamilyInstance inst) {
			var param = inst.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			var comments = param.AsString();
			return comments;
		}
		static public string GetTag(MEPCurve mepCurve) {
			var param = mepCurve.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			var comments = param.AsString();
			return comments;
		}
		static public string GetTag(Element e) {
			var inst = e as FamilyInstance;
			var mc = e as MEPCurve;
			var fab = e as FabricationPart;
			if (inst != null) {
				return GetTag(inst);
			} else if (mc != null) {
				return GetTag(mc);
			} else if (fab != null) {
				return GetTag(fab);
			}
			ASSERT(false);
			return "";
		}
		public static PartType GetPartType(FamilyInstance inst) {
			Parameter partTypeParam = inst.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
			if (partTypeParam == null) {
				throw new System.Exception("GetPartType Error!");
			}
			PartType partType = (PartType)partTypeParam.AsInteger();
			return partType;
		}
		static public bool hasIntersection(Solid a, Solid b) {
			Solid s = null;
			try {
				s = BooleanOperationsUtils.ExecuteBooleanOperation(a, b, BooleanOperationsType.Intersect);
			} catch {
				return false;
			}
			if (s == null)
				return false;
			var vol = s.Volume;
			return vol > tol;
		}
		static public ModelCurve CreateModelCurve(Curve line, Plane plane)
		{
			ASSERT(line.Length > m_ShortCurveTolerance, "CreateModelCurve() 線分が短すぎます。");
			var sketchPlane = SketchPlane.Create(m_doc, plane);
			try {
				var modelCurve = m_doc.Create.NewModelCurve(line, sketchPlane);
				{
					GraphicsStyle gs = modelCurve.LineStyle as GraphicsStyle;
					var cat = gs.GraphicsStyleCategory;
					cat.LineColor = new Color(255, 0, 0);
				}
				return modelCurve;
			} catch {
			}
			return null;
		}
		static public DetailCurve NewDetailCurve(Curve line)
		{
			Autodesk.Revit.DB.View view = MepCommon.m_uidoc.ActiveView;
			return m_doc.Create.NewDetailCurve(view, line);
		}
		static public ModelCurve CreateModelCurve(Curve line)
		{
			Plane plane = null; {
				XYZ normal = null; {
					XYZ vLine; {
						var dv = line.ComputeDerivatives(0, true);
						vLine = dv.BasisX;
					}
					if (XYZ.BasisZ.CrossProduct(vLine).GetLength() < m_ShortCurveTolerance)// tol)
						normal = XYZ.BasisX;
					else
						normal = XYZ.BasisZ.CrossProduct(vLine).Normalize();
				}
				plane = Plane.CreateByNormalAndOrigin(normal, line.Evaluate(0, true));
			}
			return CreateModelCurve(line, plane);
		}
		static public ModelCurve CreateModelCurve(XYZ pt1, XYZ pt2) {
			return CreateModelCurve(Line.CreateBound(pt1, pt2));
		}
		static public List<Line> CreateLineSegments(List<XYZ> pts)
		{
			var lines = new List<Line>();
			for (int i = 0; i < pts.Count-1; i++) {
				Line line = null;	{
					var start = pts[i];
					var end = pts[(i + 1) % pts.Count];
					if (start.DistanceTo(end) > MepCommon.m_ShortCurveTolerance)//tol)
						line = Line.CreateBound(start, end);
				}
				if (line != null)
					lines.Add(line);
			}
			return lines;
		}
		static public List<ModelCurve> DrawSolidAsWires(Solid a) {
			var mcs = new List<ModelCurve>();
			foreach (Edge e in a.Edges) {
				var c = e.AsCurve();
				var mc = CreateModelCurve(c);
				if (mc != null)
					mcs.Add(mc);
			}
			return mcs;
		}
		static public List<ModelCurve> DrawSolidAsTriangle(Solid a)
		{
			var mcs = new List<ModelCurve>();
			foreach (Face f in a.Faces) {
				var mesh = f.Triangulate();
				for (int i = 0; i < mesh.NumTriangles; i++) {
					var tri = mesh.get_Triangle(i);
					for (int j = 0; j < 3; j++) {
						var v1 = tri.get_Vertex(j);
						var v2 = tri.get_Vertex((j + 1) % 3);
						var c = Line.CreateBound(v1, v2);
						var mc = CreateModelCurve(c);
						if (mc != null)
							mcs.Add(mc);
					}
				}
			}
			return mcs;
		}
		public class SolidAndTrans
		{
			public List<Solid> m_solids = new List<Solid>();
			public Transform m_trans;
		};
		public class ExceptionTooThin : System.Exception
		{ }
		static public Solid CreateBox(BoundingBoxXYZ bbox)
		{
			return CreateBox(new ExtentXYZ(bbox));
		}
		static public Solid CreateBox(ExtentXYZ bbox) {
			var diff = bbox.Max - bbox.Min;
			if (Equal(diff.X, 0) || MepCommon.Equal(diff.Y, 0) || Equal(diff.Z, 0))
				throw new ExceptionTooThin();
			List<Curve> profile = new List<Curve>();
			{
				var w = Math.Abs(diff.X);
				var h = Math.Abs(diff.Y);
				XYZ profile00 = new XYZ(0, 0, 0);
				XYZ profile01 = new XYZ(w, 0, 0);
				XYZ profile11 = new XYZ(w, h, 0);
				XYZ profile10 = new XYZ(0, h, 0);
				profile.Add(Line.CreateBound(profile00, profile01));
				profile.Add(Line.CreateBound(profile01, profile11));
				profile.Add(Line.CreateBound(profile11, profile10));
				profile.Add(Line.CreateBound(profile10, profile00));
			}
			var d = Math.Abs(diff.Z);
			CurveLoop curveLoop = CurveLoop.Create(profile);
			var a = CreateExtrude(curveLoop, XYZ.BasisZ, d);
			return SolidUtils.CreateTransformed(a, Transform.CreateTranslation(bbox.Min));
		}
		static public Solid CreateSphereAt(XYZ centre, double radius) 
		{
			// Use the standard global coordinate system
			// as a frame, translated to the sphere centre.
			Frame frame = new Frame(centre, XYZ.BasisX,
						XYZ.BasisY, XYZ.BasisZ);
			// Create a vertical half-circle loop
			// that must be in the frame location.
			Arc arc = Arc.Create(
				centre - radius * XYZ.BasisZ,
				centre + radius * XYZ.BasisZ,
				centre + radius * XYZ.BasisX);
			Line line = Line.CreateBound(
				arc.GetEndPoint(1),
				arc.GetEndPoint(0));
			CurveLoop halfCircle = new CurveLoop();
			halfCircle.Append(arc);
			halfCircle.Append(line);
			List<CurveLoop> loops = new List<CurveLoop>(1);
			loops.Add(halfCircle);
			return GeometryCreationUtilities.CreateRevolvedGeometry(frame, loops, 0, 2 * Math.PI);
		}
		static public List<Curve> CreateLineRect(double w, double h) 
		{
			List<Curve> profile = new List<Curve>(); {
				XYZ profile00 = new XYZ(0, 0, 0);
				XYZ profile01 = new XYZ(w, 0, 0);
				XYZ profile11 = new XYZ(w, h, 0);
				XYZ profile10 = new XYZ(0, h, 0);
				profile.Add(Line.CreateBound(profile00, profile01));
				profile.Add(Line.CreateBound(profile01, profile11));
				profile.Add(Line.CreateBound(profile11, profile10));
				profile.Add(Line.CreateBound(profile10, profile00));
				var t = Transform.CreateTranslation(new XYZ(-w * 0.5, -h * 0.5, 0));
				for (int i =0; i < profile.Count; i++)
					profile[i] = profile[i].CreateTransformed(t);
			}
			return profile;
		}
		static public CurveLoop CreateLoopRect(double w, double h) 
		{
			List<Curve> profile = CreateLineRect(w, h);
			return CurveLoop.Create(profile);
		}
		static public CurveLoop CreateLoopCircle(double rad) 
		{
			List<Curve> profile = new List<Curve>();
			{
				profile.Add(Arc.Create(XYZ.BasisX * rad, -XYZ.BasisX * rad, XYZ.BasisY * rad));
				profile.Add(Arc.Create(-XYZ.BasisX * rad, XYZ.BasisX * rad, -XYZ.BasisY * rad));
			}
			return CurveLoop.Create(profile);
		}
		static public CurveLoop CreateLoopEllipse(double w, double h) 
		{
			List<Curve> profile = new List<Curve>();
			{
				profile.Add(Ellipse.CreateCurve(XYZ.Zero, w * 0.5, h * 0.5, XYZ.BasisX, XYZ.BasisY, 0, Math.PI));
				profile.Add(Ellipse.CreateCurve(XYZ.Zero, w * 0.5, h * 0.5, XYZ.BasisX, XYZ.BasisY, Math.PI, Math.PI * 2.0));
			}
			return CurveLoop.Create(profile);
		}
		static public Solid CreateExtrude(CurveLoop loop, XYZ extrudeDirection, double len) 
		{
			SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
			return GeometryCreationUtilities.CreateExtrusionGeometry(new CurveLoop[] { loop }, extrudeDirection, len, options);
		}
		static public Solid CreateRectangularPrism(double d1, double d2, double d3) 
		{
			CurveLoop curveLoop = CreateLoopRect(d1, d2);
			return CreateExtrude(curveLoop, XYZ.BasisZ, d3);
		}
		public static bool IsIn(BoundingBoxUV bbox, UV pt) 
		{
			return new ExtentUV(bbox).IsIn(pt);
		}
		public static bool IsIn(BoundingBoxXYZ bbox, XYZ pt) 
		{
			return new ExtentXYZ(bbox).IsIn(pt);
		}
		public static List<Connector> GetEndConnectors(ConnectorSet cons) 
		{
			var list = new List<Connector>();
			{
				foreach (Connector c in cons)
					if (c.ConnectorType == ConnectorType.End)
						list.Add(c);
			}
			return list;
		}
		public static BoundingBoxXYZ GetBoundingBoxInWcs(Element ele) 
		{
			var op = new Options();
			{
				op.DetailLevel = ViewDetailLevel.Fine;
				op.ComputeReferences = true;
			}
			var geo = ele.get_Geometry(op);
			return geo.GetBoundingBox();
		}
		public static void TransformBy(Location location, Transform t) {
			var axisAngle = QuaternionUtil.GetAxisAndAngle(t);
			if (axisAngle.m_axis != null)
				location.Rotate(Line.CreateBound(XYZ.Zero, axisAngle.m_axis), axisAngle.m_angle);
			location.Move(t.Origin);
		}
		public static BoundingBoxXYZ GetBoundingBoxLocal(FamilyInstance inst) {
			var t = inst.GetTransform();
			var tInv = t.Inverse;
			TransformBy(inst.Location, tInv);
			var bbox = inst.get_BoundingBox(null);
			TransformBy(inst.Location, t);
			return bbox;
		}
		public static BoundingBoxXYZ GetBoundingBoxLocal(FabricationPart inst) {
			var t = inst.GetTransform();
			var tInv = t.Inverse;
			TransformBy(inst.Location, tInv);
			var bbox = inst.get_BoundingBox(null);
			TransformBy(inst.Location, t);
			return bbox;
		}
		public static List<Element> Copy(Document doc, ElementId id) {
			var list = new List<Element>();
			{
				var ids = ElementTransformUtils.CopyElement(doc, id, XYZ.Zero);
				foreach (var id2 in ids)
					list.Add(doc.GetElement(id2));
			}
			return list;
		}
		public static void Connect(Connector a, Connector b) {
			a.Origin = b.Origin;
			a.ConnectTo(b);
		}
		public static void Disconnect(Connector c) {
			foreach (Connector r in c.AllRefs)
				r.DisconnectFrom(c);
		}
		public static XYZ GetProjectedPointOnPlane(Plane plane, XYZ pt)
		{
			double dot = 0.0; {
				var v = pt - plane.Origin;
				dot = plane.Normal.DotProduct(v);
			}
			return pt - plane.Normal * dot;
		}
		public static double GetSignedPolygonArea(List<UV> p) {
			//https://thebuildingcoder.typepad.com/blog/2008/12/2d-polygon-areas-and-outer-loop.html
			int n = p.Count;
			double sum = p[0].U * (p[1].V - p[n - 1].V);
			for (int i = 1; i < n - 1; ++i) {
				sum += p[i].U * (p[i + 1].V - p[i - 1].V);
			}
			sum += p[n - 1].U * (p[0].V - p[n - 2].V);
			return 0.5 * sum;
		}
		public static void ArbitraryAxisAlgorithm(XYZ N, out XYZ Ax, out XYZ Ay) {
			// https://knowledge.autodesk.com/search-result/caas/CloudHelp/cloudhelp/2017/ENU/AutoCAD-DXF/files/GUID-E19E5B42-0CC7-4EBA-B29F-5E1D595149EE-htm.html
			/*
			任意の軸アルゴリズムは、AutoCAD が内部的に使用するものです。これにより、オブジェクト座標を使用するすべての図形に対する任意で定型のオブジェクト座標系が生成されます。
			座標系の Z 軸として使用される単位長のベクトルが与えられると、任意の軸アルゴリズムは、その座標系に適合する X 軸を生成します。その後、右手の法則によって Y 軸が生成されます。
			この方法では、与えられた Z 軸( 「法線ベクトル」とも呼ばれます)が調べられます。それがワールド座標の Z 軸の正または負方向に十分に接近していれば、ワールド座標の Y 軸と与えられた Z 軸との外積が計算され、これが任意の X 軸となります。十分に接近していない場合は、ワールド座標の Z 軸と与えられた Z 軸との外積が計算され、これが任意の X 軸となります。十分に接近しているかどうかの決定には、境界が使用されます。これは、計算を容易にし、ハードウェアに依存しないためです。これは、一種の "正方形" の極キャップによって実現されます。境界の値は 1/64 で、これは 10 進数では小数点以下 6 桁、2 進数では 6 ビットで正確に指定することが可能です。
			アルゴリズムは次のようになります(すべてのベクトルは 3D 空間にあり、ワールド座標系で指定されているものとします)。
			Let the given normal vector be called N.
			Let the world Y axis be called Wy, which is always (0,1,0).
			Let the world Z axis be called Wz, which is always (0,0,1).
			これから、法線 N に対する任意の X 軸および Y 軸を求めます。これを、Ax および Ay とします。次に示すように、N は Az(任意の Z 軸)と呼ばれることもあります。
			If (abs (Nx) < 1/64) and (abs (Ny) < 1/64) then
			     Ax = Wy X N (where “X” is the cross-product operator).
			Otherwise,
			     Ax = Wz X N.
			Scale Ax to unit length.
			Ay ベクトルを求める方法は、次のとおりです。
			Ay = N X Ax. Scale Ay to unit length.
			 * */
			N.Normalize();
			const double tol = 1.0 / 64.0;
			var Wy = XYZ.BasisY;
			var Wz = XYZ.BasisZ;
			if (Math.Abs(N.X) < tol && Math.Abs(N.Y) < tol) {
				Ax = Wy.CrossProduct(N);
			} else {
				Ax = Wz.CrossProduct(N);
			}
			Ax = Ax.Normalize();
			Ay = N.CrossProduct(Ax);
			Ay = Ay.Normalize();
		}
		static public double mmToFeet(double mm)
		{
			return UnitUtils.Convert(mm, UnitTypeId.Millimeters, UnitTypeId.Feet);
		}
		static public bool IsPointAbovePlane(Plane plane, XYZ pt)
		{
			var v = pt - plane.Origin;
			var dot = plane.Normal.DotProduct(v);
			return dot > 0.0;
		}
		static public List<Level> GetSortedPlanLevels() {
			var sorted_levels = new SortedDictionary<double, Level>();
			var viewPlans = _findElementsByType(typeof(ViewPlan));
			foreach (ViewPlan viewPlan in viewPlans) {
				var id = viewPlan.LevelId;
				var ele = m_doc.GetElement(id);
				if (ele == null)
					continue;
				var level = ele as Level;
				ASSERT(level != null);
				sorted_levels.Add(level.Elevation, level);
			}
			var levels = new List<Level>();
			foreach (var x in sorted_levels)
				levels.Add(x.Value);
			return levels;
		}
		/*
		Vector3 Intersect(Vector3 planeP, Vector3 planeN, Vector3 rayP, Vector3 rayD)
		{
		    var d = Vector3.Dot(planeP, -planeN);
		    var t = -(d + rayP.z * planeN.z + rayP.y * planeN.y + rayP.x * planeN.x) / (rayD.z * planeN.z + rayD.y * planeN.y + rayD.x * planeN.x);
		    return rayP + t * rayD;
		}
		 * */
		static public XYZ IntersectPlaneAndRay(XYZ planeP, UnitXYZ planeN, XYZ rayP, UnitXYZ rayD)
		{
			// https://stackoverflow.com/questions/23975555/how-to-do-ray-plane-intersection
			var cross = planeN.CrossProduct(rayD);
			if (MepCommon.Equal(cross.GetLength(), 1.0))
				throw new System.Exception("平面とRayの方向が平行です。交点を計算できません。");
			var d = planeP.DotProduct(-planeN);
			var t = -(d + rayP.Z * planeN.Z + rayP.Y * planeN.Y + rayP.X * planeN.X) / (rayD.Z * planeN.Z + rayD.Y * planeN.Y + rayD.X * planeN.X);
			return rayP + t * rayD;
		}
		static public XYZ IntersectPlaneAndRay(Plane plane, Ray ray)
		{
			return IntersectPlaneAndRay(plane.Origin, new UnitXYZ(plane.Normal), ray.m_pt, ray.m_v);
		}
		public static UIView GetActiveUiView()
		{
			Autodesk.Revit.DB.View view = m_doc.ActiveView;
			IList<UIView> uiviews = m_uidoc.GetOpenUIViews();
			UIView uiview = null;
			foreach (UIView uv in uiviews) {
				if (uv.ViewId.Equals(view.Id)) {
					uiview = uv;
					break;
				}
			}
			return uiview;
		}
	}
	// ---------------------------------
	// デバッグ用図形作図ユーティリティー
	// ---------------------------------
	class DebugUtil
	{
		protected const string        m_familyName = "C:\\dev\\RevitExtensionForMEP\\trunk\\10_ダクト・配管レベル移動\\30_PG開発\\32_ソース\\RevitMEPAddin2018\\CmdMoveConnector\\DebugXyz.rfa";
		protected const string        m_inst = "DebugXyz";
		protected static FamilySymbol m_symbol = null;
		public static void DrawPoint(XYZ pt)
		{
			Document doc = MepCommon.m_doc;
			if (m_symbol == null) {
				try {
					if (!doc.LoadFamilySymbol(m_familyName, m_inst, out m_symbol))
						MepCommon.ASSERT(false, "Family["+m_familyName+"]が存在しません。");
					m_symbol.Activate();
				} catch (Exception e) {
					MepCommon.ASSERT(false, e.Message);
				}
			}
			var inst = doc.Create.NewFamilyInstance(pt, m_symbol, StructuralType.NonStructural);
		}
		public static void DrawViewClippingPlane()
		{
			var view = MepCommon.m_uidoc.ActiveView;
			var view3d = view as View3D;
			UIView uiView = MepCommon.GetActiveUiView();
			IList<XYZ> corners = uiView.GetZoomCorners();
			Transform tView = null; Transform tViewInv = null; {
				var pt = (corners[0] + corners[1]) * 0.5;
				tView = Transform.CreateTranslation(pt);
				{
					tView.BasisX = view.RightDirection;
					tView.BasisY = view.UpDirection;
					tView.BasisZ = view.ViewDirection;
				}
				tViewInv = tView.Inverse;
			}
			for (int i = 0; i < corners.Count; i++)
				corners[i] = tViewInv.OfPoint(corners[i]);
			var w = Math.Abs(corners[1].X - corners[0].X);
			var h = Math.Abs(corners[1].Y - corners[0].Y);
			var lines = MepCommon.CreateLineRect(w, h); {
				var c1 = Line.CreateBound(corners[0], corners[1]);
				lines.Add(c1);
			}
			foreach (Line line in lines) {
				var ll = line.CreateTransformed(tView);
				MepCommon.CreateModelCurve(ll);
			}
		}
		public static void DawEyeBeam()
		{
			// 視線を描画(ちょっとズレている。)
			var view = MepCommon.m_uidoc.ActiveView;
			var view3d = view as View3D;
			if (view3d != null) {
				var orientation = view3d.GetOrientation();
				double far = 0;	{
					var param_far = view3d.get_Parameter(BuiltInParameter.VIEWER_BOUND_OFFSET_FAR);
					MepCommon.ASSERT(param_far.StorageType == StorageType.Double);
					far = param_far.AsDouble();
				}
				{
					var start = orientation.EyePosition;
					{
						UIView uiView = MepCommon.GetActiveUiView();
						IList<XYZ> corners = uiView.GetZoomCorners();
						start = (corners[0] + corners[1]) * 0.5;
					}
					var end = start + orientation.ForwardDirection * far;
					Line line = Line.CreateBound(start, end);
					MepCommon.CreateModelCurve(line);
				}
				if (false) {
					var start = orientation.EyePosition;
					var end = start + orientation.ForwardDirection * far;
					Line line = Line.CreateBound(start, end);
					MepCommon.CreateModelCurve(line);
				}
			}
		}
	}
	// ---------------------------------
	// 四元数ユーティリティー
	// ---------------------------------
	public class QuaternionUtil
	{
		private static double SQRT(double x) { return Math.Sqrt(x); }
		private static double SIGN(double x) { return (x >= 0.0f) ? +1.0f : -1.0f; }
		private static double NORM(double a, double b, double c, double d) { return SQRT(a * a + b * b + c * c + d * d); }
		// http://www.cg.info.hiroshima-cu.ac.jp/~miyazaki/knowledge/tech52.html
		// 回転行列からクォータニオンを取得する。
		public static void ConvertMatrixToQuaternion(out double q0, out double q1, out double q2, out double q3,
							     double r11, double r12, double r13,
							     double r21, double r22, double r23,
							     double r31, double r32, double r33
							     )
		{
			double d = 1.0 / 4.0;
			q0 = (r11 + r22 + r33 + 1.0f) * d;
			q1 = (r11 - r22 - r33 + 1.0f) * d;
			q2 = (-r11 + r22 - r33 + 1.0f) * d;
			q3 = (-r11 - r22 + r33 + 1.0f) * d;
			if (q0 < 0.0f) q0 = 0.0f;
			if (q1 < 0.0f) q1 = 0.0f;
			if (q2 < 0.0f) q2 = 0.0f;
			if (q3 < 0.0f) q3 = 0.0f;
			q0 = SQRT(q0);
			q1 = SQRT(q1);
			q2 = SQRT(q2);
			q3 = SQRT(q3);
			if (q0 >= q1 && q0 >= q2 && q0 >= q3) {
				q0 *= +1.0f;
				q1 *= SIGN(r32 - r23);
				q2 *= SIGN(r13 - r31);
				q3 *= SIGN(r21 - r12);
			} else if (q1 >= q0 && q1 >= q2 && q1 >= q3) {
				q0 *= SIGN(r32 - r23);
				q1 *= +1.0f;
				q2 *= SIGN(r21 + r12);
				q3 *= SIGN(r13 + r31);
			} else if (q2 >= q0 && q2 >= q1 && q2 >= q3) {
				q0 *= SIGN(r13 - r31);
				q1 *= SIGN(r21 + r12);
				q2 *= +1.0f;
				q3 *= SIGN(r32 + r23);
			} else if (q3 >= q0 && q3 >= q1 && q3 >= q2) {
				q0 *= SIGN(r21 - r12);
				q1 *= SIGN(r31 + r13);
				q2 *= SIGN(r32 + r23);
				q3 *= +1.0f;
			} else {
				throw new Exception("coding error\n");
			}
			double r = NORM(q0, q1, q2, q3);
			q0 /= r;
			q1 /= r;
			q2 /= r;
			q3 /= r;
		}
		public static void InvertQuaternion(ref double q0, ref double q1, ref double q2, ref double q3)
		{
			var s = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
			s = s * s;
			q0 = q0 / s;
			q1 = -q1 / s;
			q2 = -q2 / s;
			q3 = -q3 / s;
		}
		public class AxisAngle
		{
			public UnitXYZ m_axis;
			public double m_angle;
			public AxisAngle(UnitXYZ axis, double angle)
			{
				m_axis = axis;
				m_angle = angle;
			}
		};
		public static AxisAngle GetAxisAndAngle(Transform C)
		{
			UnitXYZ axis = null; double angle = 0; {
				double q0, q1, q2, q3 = 0; {
					ConvertMatrixToQuaternion(
						out q0, out q1, out q2, out q3,
						C.BasisX.X, C.BasisY.X, C.BasisZ.X,
						C.BasisX.Y, C.BasisY.Y, C.BasisZ.Y,
						C.BasisX.Z, C.BasisY.Z, C.BasisZ.Z
						);
				}
				angle = 2.0 * Math.Acos(q0);
				if (!MepCommon.Equal(angle,0)) {
					axis = new UnitXYZ(q1 / Math.Sin(angle * 0.5), q2 / Math.Sin(angle * 0.5), q3 / Math.Sin(angle * 0.5));
				}
			}
			return new AxisAngle(axis, angle);
		}
	}
	// ---------------------------------
	// 不要なワーニングを表示させないようにする。
	// ---------------------------------
	public class FailureHandler : IFailuresPreprocessor
	{
		public string ErrorMessage { set; get; }
		public string ErrorSeverity { set; get; }
		public FailureHandler() {
			ErrorMessage = "";
			ErrorSeverity = "";
		}
		public FailureProcessingResult PreprocessFailures(
			FailuresAccessor failuresAccessor) {
			IList<FailureMessageAccessor> failureMessages
				= failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor
				 failureMessageAccessor in failureMessages) {
				FailureDefinitionId id = failureMessageAccessor
					.GetFailureDefinitionId();
				try {
					ErrorMessage = failureMessageAccessor
						.GetDescriptionText();
				} catch {
					ErrorMessage = "Unknown Error";
				}
				System.Windows.MessageBox.Show("Failed to move.");
				try {
					FailureSeverity failureSeverity
						= failureMessageAccessor.GetSeverity();
					ErrorSeverity = failureSeverity.ToString();
					if (failureSeverity == FailureSeverity.Warning) {
						failuresAccessor.DeleteWarning(
							failureMessageAccessor);
					} else {
						return FailureProcessingResult
							.ProceedWithRollBack;
					}
				} catch {
				}
			}
			return FailureProcessingResult.Continue;
		}
	}
	// ----------------------------------
	// セレクション関係
	// ----------------------------------
	public interface SelectionUtilIF
	{
		List<Element> SelectionUtilIF_GetStraights();
	}
	public class SelectionUtil
	{
		public class INVALID_VIEW : System.Exception
		{
		}
		SelectionUtilIF m_IF;
		static public Plane GetProjectPlaneIso()
		{
			const double cUnlimitedViewDepth = MepCommon.m_far;
			var view = MepCommon.m_uidoc.ActiveView;
			var viewPlan = view as ViewPlan;
			var viewSection = view as ViewSection;
			var view3d = view as View3D;
			if (viewPlan != null) {
				//  https://forums.autodesk.com/t5/revit-api-forum/getting-levelid-for-level-below-from-viewrange-returns-4/td-p/6681011
				double z = 0; {
					Func<ViewPlan, PlanViewPlane, double> GetZ = (view_plan, plane) => {
						var view_range = view_plan.GetViewRange();
						var id = view_range.GetLevelId(plane);
						double elevation = 0;	{
							var ele = MepCommon.m_doc.GetElement(id);
							var level = ele as Level;
							elevation = level.Elevation;
						}
						var offset = view_range.GetOffset(plane);
						z = elevation + offset;
						return z;
					};
					var viewRange = viewPlan.GetViewRange();
					var levelId = viewRange.GetLevelId(PlanViewPlane.TopClipPlane);
					if (levelId.ToString() == "-1") {
						// 'unlimited view depth
						z = cUnlimitedViewDepth;
					} else if (levelId.ToString() == "-2") {
						// 'the level above
						var levels = MepCommon.GetSortedPlanLevels();
						Level level = null;
						{
							var id = viewRange.GetLevelId(PlanViewPlane.CutPlane);
							var ele = MepCommon.m_doc.GetElement(id);
							level = ele as Level;
						}
						int i = 0;
						for (; i < levels.Count; i++) {
							if (MepCommon.Equal(levels[i].Id, level.Id))
								break;
						}
						if (i < (levels.Count - 1)) {
							var above = levels[i + 1];
							// ここには来ないと思われる。
							MepCommon.ASSERT(false, "PickPoint error");
						} else {
							z = cUnlimitedViewDepth;
						}
					} else if (levelId.ToString() == "-3") {
						// 'the same as the plan level
						z = GetZ(viewPlan, PlanViewPlane.CutPlane);
					} else {
						z = GetZ(viewPlan, PlanViewPlane.TopClipPlane);
					}
				}
				return Plane.CreateByOriginAndBasis(new XYZ(0, 0, z), view.RightDirection, view.UpDirection);
			} else if (viewSection != null) {
				var org = viewSection.Origin;
				return Plane.CreateByOriginAndBasis(org, view.RightDirection, view.UpDirection);
			} else if (view3d != null) {
				MepCommon.ASSERT(!view3d.IsPerspective);
				XYZ org = null;	{
					IList<XYZ> corners; {
						UIView uiView = MepCommon.GetActiveUiView();
						corners = uiView.GetZoomCorners();
					}
					org = (corners[0] + corners[1]) * 0.5;
					var orientation = view3d.GetOrientation();
					double advance = -cUnlimitedViewDepth;
					org += orientation.ForwardDirection * advance;
				}
				return Plane.CreateByOriginAndBasis(org, view.RightDirection, view.UpDirection);
			} else {
				throw new INVALID_VIEW();
			}
			return null;
		}
		static public Plane GetProjectPlaneParse(double advance)
		{
			var view = MepCommon.m_uidoc.ActiveView;
			var view3d = view as View3D;
			MepCommon.ASSERT(view3d != null);
			MepCommon.ASSERT(view3d.IsPerspective);
			XYZ org = null; {
				IList<XYZ> corners;
				{
					UIView uiView = MepCommon.GetActiveUiView();
					corners = uiView.GetZoomCorners();
				}
				org = (corners[0] + corners[1]) * 0.5;
				var orientation = view3d.GetOrientation();
				org += orientation.ForwardDirection * advance;
			}
			return Plane.CreateByOriginAndBasis(org, view.RightDirection, view.UpDirection);
		}
		/*
			double near = 0; {
				var param_near = view3d.get_Parameter(BuiltInParameter.VIEWER_BOUND_OFFSET_NEAR);
				MepCommon.ASSERT(param_near.StorageType == StorageType.Double);
				near = param_near.AsDouble();
			}
		 * */
		public Solid GetSolidFromConnectorAndCurve(Connector cc, Curve curve, double fuzz)
		{
			MepCommon.ASSERT(cc != null);
			double len = 0;
			{
				MepCommon.ASSERT(curve.Length > MepCommon.m_VertexTolerance);
				var pt1 = curve.Evaluate(0, true);
				var pt2 = curve.Evaluate(1, true);
				len = pt1.DistanceTo(pt2);
			}
			CurveLoop curveLoop = null; {
				if (cc.Shape == ConnectorProfileType.Rectangular) {
					var w = cc.Width + fuzz;
					var h = cc.Height + fuzz;
					curveLoop = MepCommon.CreateLoopRect(w, h);
				} else if (cc.Shape == ConnectorProfileType.Round) {
					var rad = cc.Radius + fuzz * 0.5;
					curveLoop = MepCommon.CreateLoopCircle(rad);
				} else if (cc.Shape == ConnectorProfileType.Oval) {
					var w = cc.Width + fuzz;
					var h = cc.Height + fuzz;
					curveLoop = MepCommon.CreateLoopEllipse(w, h);
				} else {
					MepCommon.ASSERT(false);
				}
			}
			var solid = MepCommon.CreateExtrude(curveLoop, -XYZ.BasisZ, len);
			solid = SolidUtils.CreateTransformed(solid, cc.CoordinateSystem);
			return solid;
		}
		public Solid GetSolidFromDuctOrPipe(Element straight)
		{
			double fuzz = MepCommon.mmToFeet(10);
			if (straight is Duct || straight is Pipe) {
				var mep = straight as MEPCurve;
				Connector cc1 = null;
				{
					foreach (Connector c in mep.ConnectorManager.Connectors) {
						if (c.ConnectorType == ConnectorType.End) {
							cc1 = c;
							break;
						}
					}
				}
				Curve curve1 = null;
				{
					var loc = mep.Location as LocationCurve;
					curve1 = loc.Curve;
				}
				return GetSolidFromConnectorAndCurve(cc1, curve1, fuzz);
			}
			FabricationPart fabs = straight as FabricationPart;
			MepCommon.ASSERT(fabs != null);
			MepCommon.ASSERT(fabs.IsAStraight());
			Connector cc = null; {
				foreach (Connector c in fabs.ConnectorManager.Connectors) {
					if (c.ConnectorType == ConnectorType.End) {
						cc = c;
						break;
					}
				}
			}
			Curve curve = null; {
				var loc = fabs.Location as LocationCurve;
				curve = loc.Curve;
			}
			return GetSolidFromConnectorAndCurve(cc, curve, fuzz);
		}
		protected bool IsInViewFrustum(XYZ pt, Plane nearClipingPlane)
		{
			if (MepCommon.IsPointAbovePlane(nearClipingPlane, pt))
				return false;
			return true;
		}
		public Element PickPoint(string msg, out XYZ ptOnSurface)
		{
			Element ret = null;
			ptOnSurface = null;
			var view = MepCommon.m_uidoc.ActiveView;
			var sketchPlaneOrg = view.SketchPlane;
			var nearClipingPlane = GetProjectPlaneIso();
			XYZ pickPoint = null;	{
				var sketchPlane = SketchPlane.Create(MepCommon.m_doc, nearClipingPlane);
				view.SketchPlane = sketchPlane;
				var snap = ObjectSnapTypes.Endpoints |
					ObjectSnapTypes.Midpoints |
						ObjectSnapTypes.Nearest |
							ObjectSnapTypes.WorkPlaneGrid |
								ObjectSnapTypes.Intersections |
									ObjectSnapTypes.Centers |
										ObjectSnapTypes.Perpendicular |
											ObjectSnapTypes.Tangents |
												ObjectSnapTypes.Quadrants |
													ObjectSnapTypes.Points;
				pickPoint = MepCommon.m_uidoc.Selection.PickPoint(snap, msg);
			}
			var straights = m_IF.SelectionUtilIF_GetStraights();
			double max_dist = double.MaxValue;
			Line eyeDir = null;
			{
				double far = MepCommon.m_far;
				var pt1 = pickPoint - nearClipingPlane.Normal * far;
				var pt2 = pickPoint + nearClipingPlane.Normal * far;
				eyeDir = Line.CreateBound(pt1, pt2);
			}
			foreach (Element straight in straights) {
				var solid = GetSolidFromDuctOrPipe(straight);
				if (solid == null)
					continue;
				SolidCurveIntersection sci = null; {
					var op = new SolidCurveIntersectionOptions();
					{
						op.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
					}
					sci = solid.IntersectWithCurve(eyeDir, op);
				}
				MepCommon.ASSERT(sci != null);
				for (int segment = 0; segment <= sci.SegmentCount - 1; segment++) {
					Curve curveInside = sci.GetCurveSegment(segment);
					XYZ[] pts = new XYZ[2] { curveInside.Evaluate(0, true), curveInside.Evaluate(1, true) };
					foreach (var pt in pts) {
						if (!IsInViewFrustum(pt, nearClipingPlane)) // スケッチプレーンがViewに設定されていたら不要か。
							continue;
						double dist = pt.DistanceTo(pickPoint);
						if (dist < max_dist) {
							max_dist = dist;
							ret = straight;
							ptOnSurface = pt;
						}
					}
				}
			}
			if (sketchPlaneOrg != null)
				view.SketchPlane = sketchPlaneOrg;
			return ret;
		}
		public SelectionUtil(SelectionUtilIF IF)
		{
			m_IF = IF;
		}
	}
	// ----------------------------------
	// ユニットテストコード
	// ----------------------------------
	class UnitTest
	{
		static private bool Equal(double t1, double t2, double tol2 = MepCommon.tol)
		{
			return MepCommon.Equal(t1, t2, tol2);
		}
		static private bool Equal(ElementId t1, ElementId t2)
		{
			return MepCommon.Equal(t1, t2);
		}
		static private bool Equal(UV t1, UV t2)
		{
			return MepCommon.Equal(t1, t2);
		}
		static private bool Equal(XYZ t1, XYZ t2)
		{
			return MepCommon.Equal(t1, t2);
		}
		static private void ASSERT(bool b, string message = "") {
			MepCommon.ASSERT(b, message);
		}
		static private void MepCommonTest()
		{
			// static public bool Equal(double t1, double t2, double tol2 = MepCommon.tol) {
			{
				ASSERT(MepCommon.Equal(1.1,1.1));
				ASSERT(!MepCommon.Equal(1.1,1.2));
			}
			// static public bool Equal(ElementId t1, ElementId t2) {
			{
			}
			// static public bool Equal(UV t1, UV t2) {
			{
				ASSERT(MepCommon.Equal(new UV(1,1), new UV(1,1)));
				ASSERT(!MepCommon.Equal(new UV(1,1), new UV(2,1)));
				ASSERT(!MepCommon.Equal(new UV(1,1), new UV(1,2)));
			}
			// static public bool Equal(XYZ t1, XYZ t2) {
			{
				ASSERT(MepCommon.Equal(new XYZ(1,1,1), new XYZ(1,1,1)));
				ASSERT(!MepCommon.Equal(new XYZ(1,1,1), new XYZ(2,1,1)));
				ASSERT(!MepCommon.Equal(new XYZ(1,1,1), new XYZ(1,2,1)));
				ASSERT(!MepCommon.Equal(new XYZ(1,1,1), new XYZ(1,1,2)));
			}
			// static public List<T> ConvEnumeratorToList<T>(IEnumerator<T> er) {
			// static public int Find(List<Connector> list, Connector target) {
			// public static List<Element> _findElementsByType(Type type) {
			// static public KeyValuePair<Type1, Type2> make_pair<Type1, Type2>(Type1 p1, Type2 p2) {
			{
				var x = MepCommon.make_pair(1,2);
				ASSERT(MepCommon.Equal(x.Key, 1));
				ASSERT(MepCommon.Equal(x.Value, 2));
			}
			// static public int FindByKey<Type1, Type2>(List<KeyValuePair<Type1, Type2>> list, Type1 target) {
			{
				{
					// int等　タイプ
					var list = new List<KeyValuePair<int, int>>();
					list.Add(MepCommon.make_pair(1, 2));
					list.Add(MepCommon.make_pair(2, 3));
					list.Add(MepCommon.make_pair(3, 4));
					{
						var idx = MepCommon.FindByKey(list, 2);
						ASSERT(MepCommon.Equal(list[idx].Key, 2));
						ASSERT(MepCommon.Equal(list[idx].Value, 3));
					}
					{
						var idx = MepCommon.FindByKey(list, 10);
						ASSERT(MepCommon.Equal(idx, -1));
					}
				}
				{
					// クラス
					var p1 = new UV(1,2);
					var p2 = new UV(2,3);
					var p3 = new UV(3,4);
					var p4 = new UV(4, 5);
					var p5 = new UV(5, 6);
					var list = new List<KeyValuePair<UV, UV>>(); {
						list.Add(MepCommon.make_pair(p1, p2));
						list.Add(MepCommon.make_pair(p2, p3));
						list.Add(MepCommon.make_pair(p3, p4));
					}
					{
						var idx = MepCommon.FindByKey(list, p2);
						ASSERT(MepCommon.Equal(list[idx].Key, p2));
						ASSERT(MepCommon.Equal(list[idx].Value, p3));
					}
					{
						var idx = MepCommon.FindByKey(list, p5);
						ASSERT(MepCommon.Equal(idx, -1));
					}
				}
			}
			// static public int FindByValue<Type1, Type2>(List<KeyValuePair<Type1, Type2>> list, Type2 target) {
			{
				{
					// int等　タイプ
					var list = new List<KeyValuePair<int, int>>();
					list.Add(MepCommon.make_pair(1, 2));
					list.Add(MepCommon.make_pair(2, 3));
					list.Add(MepCommon.make_pair(3, 4));
					{
						var idx = MepCommon.FindByValue(list, 2);
						ASSERT(MepCommon.Equal(list[idx].Key, 1));
						ASSERT(MepCommon.Equal(list[idx].Value, 2));
					}
					{
						var idx = MepCommon.FindByValue(list, 10);
						ASSERT(MepCommon.Equal(idx, -1));
					}
				}
				{
					// クラス
					var p1 = new UV(1, 2);
					var p2 = new UV(2, 3);
					var p3 = new UV(3, 4);
					var p4 = new UV(4, 5);
					var p5 = new UV(5, 6);
					var list = new List<KeyValuePair<UV, UV>>();
					{
						list.Add(MepCommon.make_pair(p1, p2));
						list.Add(MepCommon.make_pair(p2, p3));
						list.Add(MepCommon.make_pair(p3, p4));
					}
					{
						var idx = MepCommon.FindByValue(list, p2);
						ASSERT(MepCommon.Equal(list[idx].Key, p1));
						ASSERT(MepCommon.Equal(list[idx].Value, p2));
					}
					{
						var idx = MepCommon.FindByValue(list, p5);
						ASSERT(MepCommon.Equal(idx, -1));
					}
				}
			}
			// static public UnitXYZ GetDir(Curve c) {
			{
				{
					var line = Line.CreateBound(new XYZ(-1,-1,-1), new XYZ(2,2,2));
					var dir = MepCommon.GetDir(line);
					ASSERT(MepCommon.Equal(dir, new UnitXYZ(1, 1, 1)));
				}
			}
			//static public Transform GetTransform(Element e) {
			//static public string GetTag(FabricationPart fab) {
			//static public string GetTag(FamilyInstance inst) {
			//static public string GetTag(MEPCurve mepCurve) {
			//static public string GetTag(Element e) {
			//public static PartType GetPartType(FamilyInstance inst) {
			//static public bool hasIntersection(Solid a, Solid b) {
			{
				{
					Solid solid1 = null; {
						BoundingBoxXYZ bbox = new BoundingBoxXYZ(); {
							bbox.Min = new XYZ(0, 0, 0);
							bbox.Max = new XYZ(100, 100, 100);
						}
						solid1 = MepCommon.CreateBox(bbox);
					}
					var bbox1 = solid1.GetBoundingBox();
					Solid solid2 = null; {
						BoundingBoxXYZ bbox = new BoundingBoxXYZ(); {
							bbox.Min = new XYZ(200, 200, 200);
							bbox.Max = new XYZ(300, 300, 300);
						}
						solid2 = MepCommon.CreateBox(bbox);
					}
					var bbox2 = solid2.GetBoundingBox();
					var b = MepCommon.hasIntersection(solid1, solid2);
					ASSERT(!b);
				}
				{
					Solid solid1 = null; {
						BoundingBoxXYZ bbox = new BoundingBoxXYZ(); {
							bbox.Min = new XYZ(0, 0, 0);
							bbox.Max = new XYZ(100, 100, 100);
						}
						solid1 = MepCommon.CreateBox(bbox);
					}
					var bbox1 = solid1.GetBoundingBox();
					Solid solid2 = null; {
						BoundingBoxXYZ bbox = new BoundingBoxXYZ(); {
							bbox.Min = new XYZ(50, 50, 50);
							bbox.Max = new XYZ(150, 150, 150);
						}
						solid2 = MepCommon.CreateBox(bbox);
					}
					var bbox2 = solid2.GetBoundingBox();
					var b = MepCommon.hasIntersection(solid1, solid2);
					ASSERT(b);
				}

			}
			//static public ModelCurve CreateModelCurve(Curve line, Plane plane)
			//static public DetailCurve NewDetailCurve(Curve line)
			//static public ModelCurve CreateModelCurve(Curve line)
			//static public ModelCurve CreateModelCurve(XYZ pt1, XYZ pt2) {
			//static public List<Line> CreateLineSegments(List<XYZ> pts)
			{
				var pts = new List<XYZ>(); {
					pts.Add(new XYZ(0,0,0));
					pts.Add(new XYZ(1,0,0));
					pts.Add(new XYZ(1,1,0));
				}
				var lines = MepCommon.CreateLineSegments(pts);
				ASSERT(lines.Count == 2);
				ASSERT(Equal(lines[0].Evaluate(0, true),new XYZ(0, 0, 0)));
				ASSERT(Equal(lines[0].Evaluate(1, true), new XYZ(1, 0, 0)));
				ASSERT(Equal(lines[1].Evaluate(0, true), new XYZ(1, 0, 0)));
				ASSERT(Equal(lines[1].Evaluate(1, true), new XYZ(1, 1, 0)));
			}
			//static public List<ModelCurve> DrawSolidAsWires(Solid a) {
			//static public List<ModelCurve> DrawSolidAsTriangle(Solid a)
			//public class SolidAndTrans
			//public class ExceptionTooThin : System.Exception
			//static public Solid CreateBox(BoundingBoxXYZ bbox)
			{
				{
					BoundingBoxXYZ bbox = new BoundingBoxXYZ();
					{
						bbox.Min = new XYZ(-100, -100, -100);
						bbox.Max = new XYZ(100, 100, 100);
					}
					var solid = MepCommon.CreateBox(bbox);
					ASSERT(MepCommon.Equal(solid.Volume, 200 * 200 * 200));
					ASSERT(MepCommon.Equal(solid.ComputeCentroid(), XYZ.Zero));
				}
				{
					BoundingBoxXYZ bbox = new BoundingBoxXYZ();
					{
						bbox.Min = new XYZ(0, 0, 0);
						bbox.Max = new XYZ(100, 100, 100);
					}
					var solid = MepCommon.CreateBox(bbox);
					ASSERT(MepCommon.Equal(solid.Volume, 100 * 100 * 100));
					var cen = solid.ComputeCentroid();
					ASSERT(MepCommon.Equal(cen, new XYZ(50,50,50)));
				}
			}
			//static public Solid CreateSphereAt(XYZ centre, double radius) {
			{
				var s = MepCommon.CreateSphereAt(new XYZ(100,100,100 ), 200);
				double tol = 20.0;
				ASSERT(MepCommon.Equal(s.Volume, 4.0/3.0 * Math.PI * Math.Pow(200.0, 3.0), tol)); // 誤差大きいがまあいいか
				var cen = s.ComputeCentroid();
				ASSERT(MepCommon.Equal(cen, new XYZ(100, 100, 100), 1e-3));
			}
			//static public List<Curve> CreateLineRect(double w, double h) {
			{
				var curves = MepCommon.CreateLineRect(100, 200);
				ASSERT(curves.Count == 4);
				ASSERT(Equal(curves[0].Evaluate(0, true), new XYZ(-50, -100, 0)));
				ASSERT(Equal(curves[0].Evaluate(1, true), new XYZ(50, -100, 0)));
				ASSERT(Equal(curves[1].Evaluate(0, true), new XYZ(50, -100, 0)));
				ASSERT(Equal(curves[1].Evaluate(1, true), new XYZ(50, 100, 0)));
				ASSERT(Equal(curves[2].Evaluate(0, true), new XYZ(50, 100, 0)));
				ASSERT(Equal(curves[2].Evaluate(1, true), new XYZ(-50, 100, 0)));
				ASSERT(Equal(curves[3].Evaluate(0, true), new XYZ(-50, 100, 0)));
				ASSERT(Equal(curves[3].Evaluate(1, true), new XYZ(-50, -100, 0)));
			}
			//static public CurveLoop CreateLoopRect(double w, double h) {
			{
				var loop = MepCommon.CreateLoopRect(100, 200);
				ASSERT(loop.IsOpen() == false);
				var curves = MepCommon.ConvEnumeratorToList(loop.GetEnumerator());
				ASSERT(curves.Count == 4);
				ASSERT(Equal(curves[0].Evaluate(0, true), new XYZ(-50, -100, 0)));
				ASSERT(Equal(curves[0].Evaluate(1, true), new XYZ(50, -100, 0)));

				ASSERT(Equal(curves[1].Evaluate(0, true), new XYZ(50, -100, 0)));
				ASSERT(Equal(curves[1].Evaluate(1, true), new XYZ(50, 100, 0)));

				ASSERT(Equal(curves[2].Evaluate(0, true), new XYZ(50, 100, 0)));
				ASSERT(Equal(curves[2].Evaluate(1, true), new XYZ(-50, 100, 0)));

				ASSERT(Equal(curves[3].Evaluate(0, true), new XYZ(-50, 100, 0)));
				ASSERT(Equal(curves[3].Evaluate(1, true), new XYZ(-50, -100, 0)));
			}
			//static public CurveLoop CreateLoopCircle(double rad) {
			{
				var loop = MepCommon.CreateLoopCircle(50);
				ASSERT(loop.IsOpen() == false);
				var curves = MepCommon.ConvEnumeratorToList(loop.GetEnumerator());
				ASSERT(curves.Count == 2);
				var a1 = curves[0] as Arc;
				var a2 = curves[1] as Arc;
				ASSERT(Equal(a1.Center, new XYZ(0, 0, 0)));
				ASSERT(Equal(a2.Center, new XYZ(0, 0, 0)));
				ASSERT(Equal(a1.Radius, 50));
				ASSERT(Equal(a2.Radius, 50));
				ASSERT(Equal(a1.Evaluate(0, true), new XYZ(50, 0, 0)));
				ASSERT(Equal(a1.Evaluate(1, true), new XYZ(-50, 0, 0)));
				ASSERT(Equal(a2.Evaluate(0, true), new XYZ(-50, 0, 0)));
				ASSERT(Equal(a2.Evaluate(1, true), new XYZ(50, 0, 0)));
			}
			//static public CurveLoop CreateLoopEllipse(double w, double h) {
			{
				var loop = MepCommon.CreateLoopEllipse(200, 100);
				ASSERT(loop.IsOpen() == false);
				var curves = MepCommon.ConvEnumeratorToList(loop.GetEnumerator());
				ASSERT(curves.Count == 2);
				var a1 = curves[0] as Ellipse;
				var a2 = curves[1] as Ellipse;
				ASSERT(Equal(a1.Evaluate(0, true), new XYZ(100, 0, 0)));
				ASSERT(Equal(a1.Evaluate(0.5, true), new XYZ(0, 50, 0)));
				ASSERT(Equal(a1.Evaluate(1, true), new XYZ(-100, 0, 0)));

				ASSERT(Equal(a2.Evaluate(0, true), new XYZ(-100, 0, 0)));
				ASSERT(Equal(a2.Evaluate(0.5, true), new XYZ(0, -50, 0)));
				ASSERT(Equal(a2.Evaluate(1, true), new XYZ(100, 0, 0)));
			}
			//static public Solid CreateExtrude(CurveLoop loop, XYZ extrudeDirection, double len) {
			{
				var loop = MepCommon.CreateLoopRect(100, 200);
				var solid = MepCommon.CreateExtrude(loop, XYZ.BasisZ, 100);
				ASSERT(Equal(solid.Volume, 100 * 200 * 100));
				ASSERT(Equal(solid.SurfaceArea, 100 * 200 * 2 + 100 * 100 * 2 + 200*100*2  ));
			}
			//static public Solid CreateRectangularPrism(double d1, double d2, double d3) {
			{
				var s = MepCommon.CreateRectangularPrism(100, 100, 100);
				var cen = s.ComputeCentroid();
				ASSERT(Equal(cen, new XYZ(0,0,50)));
			}
			//public static bool IsIn(BoundingBoxUV bbox, XYZ pt) {
			{
				{
					BoundingBoxUV bbox = null; {
						var ext = new ExtentUV(); {
							ext.Add(new UV(-100, -100));
							ext.Add(new UV(100, 100));
						}
						bbox = ext.GetBbox();
					}
					{
						ASSERT(MepCommon.IsIn(bbox, new UV(0, 0)));
						ASSERT(MepCommon.IsIn(bbox, new UV(100, 0)));
						ASSERT(MepCommon.IsIn(bbox, new UV(0, 100)));
					}
					{
						ASSERT(!MepCommon.IsIn(bbox, new UV(101, 0)));
						ASSERT(!MepCommon.IsIn(bbox, new UV(0, 101)));
					}
				}
			}
			//public static bool IsIn(BoundingBoxXYZ bbox, XYZ pt) {
			{
				{
					BoundingBoxXYZ bbox = null; {
						var ext = new ExtentXYZ(); {
							ext.Add(new XYZ(-100, -100, -100));
							ext.Add(new XYZ(100, 100, 100));
						}
						bbox = ext.GetBbox();
					}
					{
						ASSERT(MepCommon.IsIn(bbox, new XYZ(0, 0, 0)));
						ASSERT(MepCommon.IsIn(bbox, new XYZ(100, 0, 0)));
						ASSERT(MepCommon.IsIn(bbox, new XYZ(0, 100, 0)));
						ASSERT(MepCommon.IsIn(bbox, new XYZ(0, 0, 100)));
					}
					{
						ASSERT(!MepCommon.IsIn(bbox, new XYZ(101, 0, 0)));
						ASSERT(!MepCommon.IsIn(bbox, new XYZ(0, 101, 0)));
						ASSERT(!MepCommon.IsIn(bbox, new XYZ(0, 0, 101)));
					}
				}
			}
			{
				// D2R
				{
					var x = MepCommon.D2R(0);
					ASSERT(Equal(x, 0));
				}
				{
					var x = MepCommon.D2R(90);
					ASSERT(Equal(x, Math.PI * 0.5));
					ASSERT(Equal(-x, -Math.PI * 0.5));
				}
				{
					var x = MepCommon.D2R(180);
					ASSERT(Equal(x, Math.PI));
					ASSERT(Equal(-x, -Math.PI));
				}
				{
					var x = MepCommon.D2R(270);
					ASSERT(Equal(x, Math.PI * 1.5));
					ASSERT(Equal(-x, -Math.PI * 1.5));
				}
				{
					var x = MepCommon.D2R(360);
					ASSERT(Equal(x, Math.PI * 2.0));
					ASSERT(Equal(-x, -Math.PI * 2.0));
				}
			}
			{
				// mmToFeet
				{
					var x = MepCommon.mmToFeet(1.0);
					ASSERT(Equal(x, 0.00328084, 1e-7));
				}
			}
			// public static XYZ GetNearestPt(List<XYZ> pts, XYZ target);
			{
				var pts = new List<XYZ>(); {
					pts.Add(new XYZ(100, 100, 100));
					pts.Add(new XYZ(0, 0, 0));
					pts.Add(new XYZ(-100,-100,-100));
					pts.Add(new XYZ(0,0,0));
					pts.Add(new XYZ(100,100,100));
					pts.Add(new XYZ(-100, -100, -100));
				}
				{
					var x = MepCommon.GetNearestPt(pts, new XYZ(0, 0, 0));
					ASSERT(Equal(x, new XYZ(0,0,0)));
				}
				{
					var x = MepCommon.GetNearestPt(pts, new XYZ(100, 100, 100));
					ASSERT(Equal(x, new XYZ(100,100,100)));
				}
				{
					var x = MepCommon.GetNearestPt(pts, new XYZ(-100, -100, -100));
					ASSERT(Equal(x, new XYZ(-100,-100,-100)));
				}
				{
					var x = MepCommon.GetNearestPt(pts, new XYZ(49, 49, 49));
					ASSERT(Equal(x, new XYZ(0, 0, 0)));
				}
				{
					var x = MepCommon.GetNearestPt(pts, new XYZ(51, 51, 51));
					ASSERT(Equal(x, new XYZ(100, 100, 100)));
				}
			}
			// public static XYZ GetFartestPt(List<XYZ> pts, XYZ target);
			{
				var pts = new List<XYZ>();
				{
					pts.Add(new XYZ(101, 101, 101));
					pts.Add(new XYZ(0, 0, 0));
					pts.Add(new XYZ(-100, -100, -100));
					pts.Add(new XYZ(0, 0, 0));
					pts.Add(new XYZ(100, 100, 100));
					pts.Add(new XYZ(-100, -100, -100));
				}
				{
					var x = MepCommon.GetFartestPt(pts, new XYZ(0, 0, 0));
					ASSERT(Equal(x, new XYZ(101, 101, 101)));
				}
				{
					var x = MepCommon.GetFartestPt(pts, new XYZ(-100, -100, -100));
					ASSERT(Equal(x, new XYZ(101, 101, 101)));
				}
				{
					var x = MepCommon.GetFartestPt(pts, new XYZ(50, 50, 50));
					ASSERT(Equal(x, new XYZ(-100, -100, -100)));
				}
			}
			{
				// GetProjectedPointOnPlane
				{
					// Z
					var normal = new XYZ(0,0,1);
					var origin = new XYZ(100,100,100);
					var pt = new XYZ(300, 300, 300);
					var plane = Plane.CreateByNormalAndOrigin(normal, origin);
					var projPt = MepCommon.GetProjectedPointOnPlane(plane, pt);
					ASSERT(Equal(projPt, new XYZ(300, 300, 100)));
				}
				{
					// X
					var normal = new XYZ(1, 0, 0);
					var origin = new XYZ(100, 100, 100);
					var pt = new XYZ(300, 300, 300);
					var plane = Plane.CreateByNormalAndOrigin(normal, origin);
					var projPt = MepCommon.GetProjectedPointOnPlane(plane, pt);
					ASSERT(Equal(projPt, new XYZ(100, 300, 300)));
				}
				{
					// Y
					var normal = new XYZ(0, 1, 0);
					var origin = new XYZ(100, 100, 100);
					var pt = new XYZ(300, 300, 300);
					var plane = Plane.CreateByNormalAndOrigin(normal, origin);
					var projPt = MepCommon.GetProjectedPointOnPlane(plane, pt);
					ASSERT(Equal(projPt, new XYZ(300, 100, 300)));
				}
				{
					// XYZ

					var normal = (new XYZ(1, 1, 1)).Normalize();
					var origin = new XYZ(0, 0, 0);
					var pt = new XYZ(300, 300, 300);
					var plane = Plane.CreateByNormalAndOrigin(normal, origin);
					var projPt = MepCommon.GetProjectedPointOnPlane(plane, pt);
					ASSERT(Equal(projPt, new XYZ(0, 0, 0)));
				}
			}
			{
				//static public XYZ IntersectPlaneAndRay(XYZ planeP, XYZ planeN, XYZ rayP, XYZ rayD) {
				{
					XYZ planeP = new XYZ(100,100,100);
					XYZ planeN = new XYZ(0,0,1);
					XYZ rayP = new XYZ(-200,-200,-200);
					XYZ rayD = new XYZ(0,0,-1);
					var pt = MepCommon.IntersectPlaneAndRay(planeP, new UnitXYZ(planeN), rayP, new UnitXYZ(rayD));
					ASSERT(Equal(pt, new XYZ(-200, -200, 100)));
				}
				{
					XYZ planeP = new XYZ(100, 100, 100);
					XYZ planeN = new XYZ(0, 0, 1);
					XYZ rayP = new XYZ(-200, -200, -200);
					XYZ rayD = new XYZ(1, 0, 0);
					try {
						var pt = MepCommon.IntersectPlaneAndRay(planeP, new UnitXYZ(planeN), rayP, new UnitXYZ(rayD));
						ASSERT(false, "平面の法線とRayの方向が同じにも関わらず。例外が発生しない。");
					} catch  {
						// 正解
					}
				}
				{
					XYZ planeP = new XYZ(100, 100, 100);
					XYZ planeN = new XYZ(1, 0, 0);
					XYZ rayP = new XYZ(-200, -200, -200);
					XYZ rayD = new XYZ(1, 0, 0);
					var pt = MepCommon.IntersectPlaneAndRay(planeP, new UnitXYZ(planeN), rayP, new UnitXYZ(rayD));
					ASSERT(Equal(pt, new XYZ(100, -200, -200)));
				}
			}
			{
				// public static bool IsIn(BoundingBoxXYZ bbox, XYZ pt) {
				var bbox = new BoundingBoxXYZ(); {
					bbox.Min = new XYZ(-1, -1, -1);
					bbox.Max = new XYZ(1, 1, 1);
				}
				{
					bool b = MepCommon.IsIn(bbox, new XYZ(0,0,0));
					ASSERT(b);
				}
				{
					// Z
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(0, 0, 1));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(0, 0, 1.000001));
						ASSERT(!b2);
					}
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(0, 0, -1));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(0, 0, -1.0000001));
						ASSERT(!b2);
					}
				}
				{
					// X
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(1, 0, 0));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(1.000001, 0, 0));
						ASSERT(!b2);
					}
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(-1, 0, 0));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(-1.000001, 0, 0));
						ASSERT(!b2);
					}
				}
				{
					// Y
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(0, 1, 0));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(0, 1.000001, 0));
						ASSERT(!b2);
					}
					{
						bool b = MepCommon.IsIn(bbox, new XYZ(0, -1, 0));
						ASSERT(b);
						bool b2 = MepCommon.IsIn(bbox, new XYZ(0, -1.000001, 0));
						ASSERT(!b2);
					}
				}
			}
			{
				//GetSignedPolygonArea
				{
					var pts = new List<UV>(); {
						pts.Add(new UV(0,0));
						pts.Add(new UV(1,0));
						pts.Add(new UV(1,1));
						pts.Add(new UV(0,1));
					}
					double a = MepCommon.GetSignedPolygonArea(pts);
					ASSERT(Equal(a, 1.0));
				}
				{
					var pts = new List<UV>();{
						pts.Add(new UV(0, 0));
						pts.Add(new UV(1, 0));
						pts.Add(new UV(1, 1));
						pts.Add(new UV(0, 1));
						pts.Add(new UV(0, 0));
					}
					double a = MepCommon.GetSignedPolygonArea(pts);
					ASSERT(Equal(a, 1.0));
				}
				{
					var pts = new List<UV>();	{
						pts.Add(new UV(0, 0));
						pts.Add(new UV(2, 0));
						pts.Add(new UV(2, 2));
						pts.Add(new UV(0, 2));
					}
					double a = MepCommon.GetSignedPolygonArea(pts);
					ASSERT(Equal(a, 4.0));
				}
				{
					var pts = new List<UV>(); {
						var tmp = new List<XYZ>(); {
							tmp.Add(new XYZ(0, 0, 0));
							tmp.Add(new XYZ(2, 0, 0));
							tmp.Add(new XYZ(2, 1, 0));
							tmp.Add(new XYZ(1,1 , 0));
							tmp.Add(new XYZ(1,2 , 0));
							tmp.Add(new XYZ(0, 2, 0));
							var t = Transform.CreateRotation(XYZ.BasisZ, 1.0);
							var t2 = Transform.CreateTranslation(new XYZ(1,1,0));
							t = t * t2;
							foreach (XYZ pt in tmp) {
								var pt2 = t.OfPoint(pt);
								pts.Add(new UV(pt2.X, pt2.Y));
							}
						}
					};
					double a = MepCommon.GetSignedPolygonArea(pts);
					ASSERT(Equal(a, 3.0));
				}
			}
			{
				// ArbitraryAxisAlgorithm
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm(new XYZ(0, 0, 1), out x, out y);
					ASSERT(Equal(x, new XYZ(1, 0, 0)));
					ASSERT(Equal(y, new XYZ(0, 1, 0)));
				}
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm(new XYZ(1, 0, 0), out x, out y);
					ASSERT(Equal(x, new XYZ(0, 1, 0)));
					ASSERT(Equal(y, new XYZ(0, 0, 1)));
				}
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm(new XYZ(0, 1, 0), out x, out y);
					ASSERT(Equal(x, new XYZ(-1, 0, 0)));
					ASSERT(Equal(y, new XYZ(0, 0,1)));
				}
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm((new XYZ(1, 1, 0)).Normalize(), out x, out y);
					ASSERT(Equal(x, (new XYZ(-1, 1, 0)).Normalize()   ));
					ASSERT(Equal(y, new XYZ(0, 0, 1)));
				}
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm((new XYZ(0, 1, 1)).Normalize(), out x, out y);
					ASSERT(Equal(x, new XYZ(-1, 0, 0)));
					ASSERT(Equal(y, (new XYZ(0, -1, 1)).Normalize()   ));
				}
				{
					XYZ x, y = null;
					MepCommon.ArbitraryAxisAlgorithm((new XYZ(1, 0, 1)).Normalize(), out x, out y);
					ASSERT(Equal(x, new XYZ(0, 1, 0)));
					ASSERT(Equal(y, (new XYZ(-1, 0, 1)).Normalize()  ));
				}
			}
			{
				//IsPointAbovePlane
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);
					XYZ pt = new XYZ(1000000, 1000000, 0.0000001);
					ASSERT(MepCommon.IsPointAbovePlane(plane, pt));
				}
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);
					XYZ pt = new XYZ(1000000, 1000000, -0.00000001);
					ASSERT(!MepCommon.IsPointAbovePlane(plane, pt));
				}
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisX, XYZ.Zero);
					XYZ pt = new XYZ(0.0000001, 1000000, 1000000);
					ASSERT(MepCommon.IsPointAbovePlane(plane, pt));
				}
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisX, XYZ.Zero);
					XYZ pt = new XYZ(-0.0000001, 1000000, 1000000);
					ASSERT(!MepCommon.IsPointAbovePlane(plane, pt));
				}
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);
					XYZ pt = new XYZ(1000000, 0.0000001, 1000000);
					ASSERT(MepCommon.IsPointAbovePlane(plane, pt));
				}
				{
					Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);
					XYZ pt = new XYZ(1000000, -0.0000001, 1000000);
					ASSERT(!MepCommon.IsPointAbovePlane(plane, pt));
				}
			}
			// static public Solid CreateBox(BoundingBoxXYZ bbox)
			{
				var bbox = new BoundingBoxXYZ();
				bbox.Min = new XYZ(0, 0, 0);
				bbox.Max = new XYZ(10, 10, 10);
				var solid = MepCommon.CreateBox(bbox);
				var bbox2 = solid.GetBoundingBox();
				ASSERT(Equal(bbox.Min, bbox2.Transform.OfPoint(bbox2.Min)));
				ASSERT(Equal(bbox.Max, bbox2.Transform.OfPoint(bbox2.Max)));
			}
			{
				var bbox = new BoundingBoxXYZ();
				bbox.Min = new XYZ(-10, -10, -10);
				bbox.Max = new XYZ(10, 10, 10);
				var solid = MepCommon.CreateBox(bbox);
				var bbox2 = solid.GetBoundingBox();
				ASSERT(Equal(bbox.Min, bbox2.Transform.OfPoint(bbox2.Min)));
				ASSERT(Equal(bbox.Max, bbox2.Transform.OfPoint(bbox2.Max)));
			}
		}
		static private void ExtentXYZTest()
		{
			// ExtentXYZ
			{
				var b = new BoundingBoxXYZ(); {
					b.Min = new XYZ(-1, -2, -3);
					b.Max = new XYZ(1, 2, 3);
				}
				var bbox = new ExtentXYZ(b);
				ASSERT(Equal(bbox.m_min_x, -1));
				ASSERT(Equal(bbox.m_min_y, -2));
				ASSERT(Equal(bbox.m_min_z, -3));
				ASSERT(Equal(bbox.m_max_x, 1));
				ASSERT(Equal(bbox.m_max_y, 2));
				ASSERT(Equal(bbox.m_max_z, 3));
			}
			{
				var bbox = new ExtentXYZ();
				ASSERT(Equal(bbox.m_min_x, double.MaxValue));
				ASSERT(Equal(bbox.m_min_y, double.MaxValue));
				ASSERT(Equal(bbox.m_min_z, double.MaxValue));
				ASSERT(Equal(bbox.m_max_x, double.MinValue));
				ASSERT(Equal(bbox.m_max_y, double.MinValue));
				ASSERT(Equal(bbox.m_max_z, double.MinValue));
			}
			{
				var bbox = new ExtentXYZ();
				bbox.Add(new XYZ(0, 0, 0));
				ASSERT(Equal(bbox.m_max_x, 0));
				ASSERT(Equal(bbox.m_max_y, 0));
				ASSERT(Equal(bbox.m_max_z, 0));
				ASSERT(Equal(bbox.m_min_x, 0));
				ASSERT(Equal(bbox.m_min_y, 0));
				ASSERT(Equal(bbox.m_min_z, 0));
			}
			{
				var bbox = new ExtentXYZ();
				bbox.Add(new XYZ(0, 0, 0));
				bbox.Add(new XYZ(10, 11, 12));
				ASSERT(Equal(bbox.m_max_x, 10));
				ASSERT(Equal(bbox.m_max_y, 11));
				ASSERT(Equal(bbox.m_max_z, 12));
				ASSERT(Equal(bbox.m_min_x, 0));
				ASSERT(Equal(bbox.m_min_y, 0));
				ASSERT(Equal(bbox.m_min_z, 0));
				{
					bbox.Add(new XYZ(5, 5, 5));
					ASSERT(Equal(bbox.m_max_x, 10));
					ASSERT(Equal(bbox.m_max_y, 11));
					ASSERT(Equal(bbox.m_max_z, 12));
					ASSERT(Equal(bbox.m_min_x, 0));
					ASSERT(Equal(bbox.m_min_y, 0));
				}
				{
					bbox.Add(new XYZ(20, 22, 23));
					ASSERT(Equal(bbox.m_max_x, 20));
					ASSERT(Equal(bbox.m_max_y, 22));
					ASSERT(Equal(bbox.m_max_z, 23));
					ASSERT(Equal(bbox.m_min_x, 0));
					ASSERT(Equal(bbox.m_min_y, 0));
					ASSERT(Equal(bbox.m_min_z, 0));
				}
			}
			////
			{
				var bbox = new ExtentXYZ();
				bbox.Add(new XYZ(0, 0, 0));
				bbox.Add(new XYZ(-10, -11, -12));
				ASSERT(Equal(bbox.m_max_x, 0));
				ASSERT(Equal(bbox.m_max_y, 0));
				ASSERT(Equal(bbox.m_max_z, 0));
				ASSERT(Equal(bbox.m_min_x, -10));
				ASSERT(Equal(bbox.m_min_y, -11));
				ASSERT(Equal(bbox.m_min_z, -12));
				{
					bbox.Add(new XYZ(-5, -5, -5));
					ASSERT(Equal(bbox.m_max_x, 0));
					ASSERT(Equal(bbox.m_max_y, 0));
					ASSERT(Equal(bbox.m_max_z, 0));
					ASSERT(Equal(bbox.m_min_x, -10));
					ASSERT(Equal(bbox.m_min_y, -11));
					ASSERT(Equal(bbox.m_min_z, -12));
				}
				{
					bbox.Add(new XYZ(-20, -22, -23));
					ASSERT(Equal(bbox.m_max_x, 0));
					ASSERT(Equal(bbox.m_max_y, 0));
					ASSERT(Equal(bbox.m_max_z, 0));
					ASSERT(Equal(bbox.m_min_x, -20));
					ASSERT(Equal(bbox.m_min_y, -22));
					ASSERT(Equal(bbox.m_min_z, -23));
				}
			}
			{
				var bbox = new ExtentXYZ();
				bbox.Add(new XYZ(-10, -10, -10));
				bbox.Add(new XYZ(10, 10, 10));
				bbox.Add(new XYZ(-10, -10, -10));
				bbox.Add(new XYZ(10, 10, 10));
				ASSERT(Equal(bbox.m_max_x, 10));
				ASSERT(Equal(bbox.m_max_y, 10));
				ASSERT(Equal(bbox.m_max_z, 10));
				ASSERT(Equal(bbox.m_min_x, -10));
				ASSERT(Equal(bbox.m_min_y, -10));
				ASSERT(Equal(bbox.m_min_z, -10));
			}
			{
				var bbox = new ExtentXYZ();
				bbox.Add(new XYZ(-11, -10, -11));
				bbox.Add(new XYZ(10, 10, 10));
				bbox.Add(new XYZ(-10, -10,-10));
				bbox.Add(new XYZ(10, 10,10));
				ASSERT(Equal(bbox.GetW(), 21));
				ASSERT(Equal(bbox.GetH(), 20));
				ASSERT(Equal(bbox.GetD(), 21));
				ASSERT(Equal(bbox.Min, new XYZ(-11,-10,-11)));
				ASSERT(Equal(bbox.Max, new XYZ(10,10,10)));
			}
			{
				// public static List<XYZ> GetPts(BoundingBoxXYZ bbox)
				{
					ExtentXYZ bbox = null; {
						var b = new BoundingBoxXYZ();	{
							b.Min = new XYZ(-1, -1, -1);
							b.Max = new XYZ(1, 1, 1);
						}
						bbox = new ExtentXYZ(b);
					}
					var pts = bbox.GetPts();
					ASSERT(pts.Count == 8);
					ASSERT(Equal(pts[0], new XYZ(-1, -1, -1)));
					ASSERT(Equal(pts[1], new XYZ(1, -1, -1)));
					ASSERT(Equal(pts[2], new XYZ(1, 1, -1)));
					ASSERT(Equal(pts[3], new XYZ(-1, 1, -1)));
					ASSERT(Equal(pts[4], new XYZ(-1, -1, 1)));
					ASSERT(Equal(pts[5], new XYZ(1, -1, 1)));
					ASSERT(Equal(pts[6], new XYZ(1, 1, 1)));
					ASSERT(Equal(pts[7], new XYZ(-1, 1, 1)));

				}
			}
		}
		static private void ExtentUVTest()
		{
			{
				// ExtentUV
				{
					var b = new BoundingBoxUV(-1,-2,1,2);
					var bbox = new ExtentUV(b);
					ASSERT(Equal(bbox.m_min_u, -1));
					ASSERT(Equal(bbox.m_min_v, -2));
					ASSERT(Equal(bbox.m_max_u, 1));
					ASSERT(Equal(bbox.m_max_v, 2));
				}
				{
					var bbox = new ExtentUV();
					ASSERT(Equal(bbox.m_min_u, double.MaxValue));
					ASSERT(Equal(bbox.m_min_v, double.MaxValue));
					ASSERT(Equal(bbox.m_max_u, double.MinValue));
					ASSERT(Equal(bbox.m_max_v, double.MinValue));
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(0, 0));
					ASSERT(Equal(bbox.m_max_u, 0));
					ASSERT(Equal(bbox.m_max_v, 0));
					ASSERT(Equal(bbox.m_min_u, 0));
					ASSERT(Equal(bbox.m_min_v, 0));
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(0, 0));
					bbox.Add(new UV(10, 11));
					ASSERT(Equal(bbox.m_max_u, 10));
					ASSERT(Equal(bbox.m_max_v, 11));
					ASSERT(Equal(bbox.m_min_u, 0));
					ASSERT(Equal(bbox.m_min_v, 0));
					{
						bbox.Add(new UV(5, 5));
						ASSERT(Equal(bbox.m_max_u, 10));
						ASSERT(Equal(bbox.m_max_v, 11));
						ASSERT(Equal(bbox.m_min_u, 0));
						ASSERT(Equal(bbox.m_min_v, 0));
					}
					{
						bbox.Add(new UV(20, 22));
						ASSERT(Equal(bbox.m_max_u, 20));
						ASSERT(Equal(bbox.m_max_v, 22));
						ASSERT(Equal(bbox.m_min_u, 0));
						ASSERT(Equal(bbox.m_min_v, 0));
					}
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(-1,-1));
					bbox.Add(new UV(1,1));
					var pts = bbox.GetPts();
					ASSERT(Equal(pts[0], new UV(-1,-1)));
					ASSERT(Equal(pts[1], new UV(1,-1)));
					ASSERT(Equal(pts[2], new UV(1,1)));
					ASSERT(Equal(pts[3], new UV(-1,1)));
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(0, 0));
					bbox.Add(new UV(-10, -11));
					ASSERT(Equal(bbox.m_max_u, 0));
					ASSERT(Equal(bbox.m_max_v, 0));
					ASSERT(Equal(bbox.m_min_u, -10));
					ASSERT(Equal(bbox.m_min_v, -11));
					{
						bbox.Add(new UV(-5, -5));
						ASSERT(Equal(bbox.m_max_u, 0));
						ASSERT(Equal(bbox.m_max_v, 0));
						ASSERT(Equal(bbox.m_min_u, -10));
						ASSERT(Equal(bbox.m_min_v, -11));
					}
					{
						bbox.Add(new UV(-20, -22));
						ASSERT(Equal(bbox.m_max_u, 0));
						ASSERT(Equal(bbox.m_max_v, 0));
						ASSERT(Equal(bbox.m_min_u, -20));
						ASSERT(Equal(bbox.m_min_v, -22));
					}
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(-10, -10));
					bbox.Add(new UV(10, 10));
					bbox.Add(new UV(-10, -10));
					bbox.Add(new UV(10, 10));
					ASSERT(Equal(bbox.m_max_u, 10));
					ASSERT(Equal(bbox.m_max_v, 10));
					ASSERT(Equal(bbox.m_min_u, -10));
					ASSERT(Equal(bbox.m_min_v, -10));
				}
				{
					var bbox = new ExtentUV();
					bbox.Add(new UV(-11, -10));
					bbox.Add(new UV(10, 10));
					bbox.Add(new UV(-10, -10));
					bbox.Add(new UV(10, 10));
					ASSERT(Equal(bbox.GetW(), 21));
					ASSERT(Equal(bbox.GetH(), 20));
					ASSERT(Equal(bbox.Min, new UV(-11, -10)));
					ASSERT(Equal(bbox.Max, new UV(10, 10)));
				}
			}
		}
		static private void UnitUVTest() 
		{
			{
				var v1 = new UnitUV(1, 1);
				var v2 = new UV(1, 1).Normalize();
				ASSERT(Equal(v1, v2));
			}
			{
				bool b = false;
				try {
					var v1 = new UnitUV(0, 0);
				} catch {
					b = true;
				}
				ASSERT(b);
			}
		}
		static private void UnitXYZTest()
		{
			{
				var v1 = new UnitXYZ(1, 1, 1);
				var v2 = new XYZ(1, 1, 1).Normalize();
				ASSERT(Equal(v1, v2));
			}
			{
				bool b = false;
				try {
					var v1 = new UnitXYZ(0, 0, 0);
				} catch {
					b = true;
				}
				ASSERT(b);
			}
		}
		static void QuaternionUtilTest()
		{
			{
				var t = Transform.CreateRotation(XYZ.BasisZ, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, XYZ.BasisZ));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var t = Transform.CreateRotation(-XYZ.BasisZ, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, -XYZ.BasisZ));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var t = Transform.CreateRotation(XYZ.BasisX, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, XYZ.BasisX));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var t = Transform.CreateRotation(-XYZ.BasisX, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, -XYZ.BasisX));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var t = Transform.CreateRotation(XYZ.BasisY, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, XYZ.BasisY));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var t = Transform.CreateRotation(-XYZ.BasisY, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, -XYZ.BasisY));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var v = (new XYZ(1, 1, 1)).Normalize();
				var t = Transform.CreateRotation(v, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, v));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
			{
				var v = (new XYZ(1, 1, 1)).Negate().Normalize();
				var t = Transform.CreateRotation(v, 1);
				var aa = QuaternionUtil.GetAxisAndAngle(t);
				ASSERT(Equal(aa.m_axis, v));
				ASSERT(Equal(aa.m_angle, 1.0));
			}
		}
		static public void Execute()
		{
			UnitUVTest();
			UnitXYZTest();
			ExtentUVTest();
			ExtentXYZTest();
			QuaternionUtilTest();
			MepCommonTest();
		}
	}
}

