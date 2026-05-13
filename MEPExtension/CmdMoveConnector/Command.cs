/* ko-mimura
 *  エルボをまたいだ？コネクタ移動コマンド（サンプル。上手くいったらこのまま納品予定)
 *  使用方法
 *  １．Tなど接続部材を選択する。
 *  ２．移動先のパイプを選択する。
 *  ３．移動先のパイプが分割され、Tなどの接続部材と接続される。
 *  ４．Tともともと接続されていた部材は可能であれば、ひとつのパイプにマージされる。
 */
using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using System.Windows.Forms;
using Autodesk.Revit.DB.Plumbing;
namespace CmdMoveConnector
{
	public abstract class Node
	{
		public Node m_par                  = null;
		public Connector m_start         = null;
		public List<Node> m_children  = new List<Node>();
		public void GetReversePath(ref List<Node> nodes) 
		{
			nodes.Add(this);
			if (m_par != null)
				m_par.GetReversePath(ref nodes);
		}
		public void Find(Element e, ref List<Node>ret) 
		{
			if (element.Id == e.Id)
				ret.Add(this);
			foreach (var node in m_children) 
				node.Find(e, ref ret);
		}
		public Node FindRev(Element e) 
		{
			if (element.Id == e.Id)
				return this;
			if (m_par != null)
				return m_par.FindRev(e);
			return null;
		}
		public abstract ConnectorSet connectorSet { get; }
		public abstract Element element { get; }
		public abstract string Tag { get; }
		public Node(Node par, Connector start) 
		{
			m_par = par;
			m_start = start;
			if (m_par != null) 
				m_par.m_children.Add(this);
		}
	};
	public class NodeFamilyInstance  :  Node 
	{
		public FamilyInstance m_familyInstance = null;
		public override ConnectorSet connectorSet 
		{ 
			get {
				return m_familyInstance.MEPModel.ConnectorManager.Connectors;
			}
		}
		public override string Tag 
		{
			 get {
				var param = m_familyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
				var comments = param.AsString();
				return comments;
			}
		}
		public override Element element 
		{
			get {
				return m_familyInstance;
			}
		}
		public NodeFamilyInstance(Node par, Connector start, FamilyInstance inst) 
		:base(par, start)
		{
			m_familyInstance = inst;
		}
	}
	public class NodeMepCurve : Node
	{
		public MEPCurve m_mepCurve = null;
		public override ConnectorSet connectorSet 
		{
			get {
				return m_mepCurve.ConnectorManager.Connectors;
			}
		}
		public override string Tag 
		{
			get {
				var param = m_mepCurve.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
				var comments = param.AsString();
				return comments;
			}
		}
		public override Element element 
		{
			get {
				return m_mepCurve;
			}
		}
		public NodeMepCurve(Node par, Connector start, MEPCurve mepCurve)
		: base(par, start) 
		{
			m_mepCurve = mepCurve;
		}
	}
	// ---------------------------------------
	[Transaction(TransactionMode.Manual)]
	public class MoveConnector : IExternalCommand
	{
		enum PIPE_TYPE : int {
			PIPE = 0,
			DUCT = 1,
		};
		private readonly List<string> targetCategories = new List<string>(){ "配管継手" , "配管付属品", "ダクト継手" };
		private const double tol = 10e-6;
		private const string debugPointFamilyInstance = "C:\\dev\\RevitExtensionForMEP\\trunk\\10_ダクト・配管レベル移動\\30_PG開発\\32_ソース\\RevitMEPAddin2018\\CmdMoveConnector\\DebugXyz.rfa";
		private const string debugPointFamilyInstanceName = "DebugXyz";
		class SrcPipe
		{
			public MEPCurve m_mepCurve;
			public Connector m_FamilyInstanceConnector;
			public Connector m_mepCurveConnector;
			public SrcPipe(MEPCurve mepCurve, Connector familyInstanceConnector, Connector mepCurveConnector) 
			{
				m_mepCurve = mepCurve;
				m_mepCurveConnector = mepCurveConnector;
				m_FamilyInstanceConnector = familyInstanceConnector;
			}
		};
		class DestPipe
		{
			public MEPCurve m_mepCurve;
			public MEPCurve m_mepCurveCopy;
			public XYZ          m_pickPoint;
			public DestPipe(MEPCurve pipe, XYZ pickPoint) 
			{
				m_mepCurve = pipe;
				m_pickPoint = pickPoint;
			}
		};
		private static FamilySymbol debugSymbol = null;
		private UIDocument         m_uidoc;
		private UIApplication        m_uiapp;
		private Document            m_doc;
		private PIPE_TYPE            m_pipeType;
		private DestPipe               m_destPipe;
		private List<SrcPipe>       m_FamilyInstanceMepCurves;
		private FamilyInstance      m_familyInstance;
		private List<Node>          m_path;
		private Autodesk.Revit.ApplicationServices.Application m_app;
		private void ASSERT(bool b) 
		{
			System.Diagnostics.Debug.Assert(b);
		}
		private void messageBox(string msg) 
		{
			MessageBox.Show(msg);
		}
		private void DebugPt(XYZ pt) 
		{
			if (debugSymbol == null) {
				try {
					if (!m_doc.LoadFamilySymbol(debugPointFamilyInstance, debugPointFamilyInstanceName, out debugSymbol)) 
						ASSERT(false);
					debugSymbol.Activate();
				} catch (Exception) {
					ASSERT(false);
				}
			}
			m_doc.Create.NewFamilyInstance(pt, debugSymbol, StructuralType.NonStructural);
		}
		private string GetPipeTypeStr() 
		{
			if (m_pipeType == PIPE_TYPE.PIPE)
				return "Pipe";
			return "Duct";
		}
		private bool CheckConnector(FamilyInstance inst) 
		{
			var name = inst.Category.Name;
			foreach (var name2 in targetCategories) {
				if (name == name2)
					return true;
			}
			var msg = "Wrong category name. Pick a connector from one of these categories [";
			foreach (var name2 in targetCategories) {
				msg += name2;
				msg += ",";
			}
			msg += "]";
			messageBox(msg);
			return false;
		}
		
		private void PickFamilyInstance() 
		{
			Func<bool> _setPipeType = () => {
				if (m_FamilyInstanceMepCurves[0].m_mepCurve is Pipe) {
					m_pipeType = PIPE_TYPE.PIPE;
				} else {
					m_pipeType = PIPE_TYPE.DUCT;
				}
				return true;
			};
			var elms = m_uidoc.Selection.GetElementIds();
			if (elms.Count == 1) {
				var it = elms.GetEnumerator();
				if (it.MoveNext()) {
					var inst = m_doc.GetElement(it.Current) as FamilyInstance;
					if (inst != null) {
						m_FamilyInstanceMepCurves = GetConnectedPipes(inst);
						if (m_FamilyInstanceMepCurves.Count > 0) {
							m_familyInstance = inst;
							_setPipeType();
							return;
						}
					}
				}
			}
			while (true) {
				var msg = "Select connecting fitting or accessory.";
				var reference = m_uidoc.Selection.PickObject(ObjectType.Element, msg);
				var element = m_uidoc.Document.GetElement(reference);
				var inst = element as FamilyInstance;
				if (inst == null) {
					messageBox(msg);
				} else {
					if (!CheckConnector(inst))
						continue;
					m_FamilyInstanceMepCurves = GetConnectedPipes(inst);
					if (m_FamilyInstanceMepCurves.Count > 0) {
						m_familyInstance = inst;
						_setPipeType();
						return;
					} else {
						messageBox("Not connected to " + GetPipeTypeStr() + ". Pick another element.");
					}
				}
			}
		}
		private List<SrcPipe> GetConnectedPipes(FamilyInstance inst) 
		{
			var pipe_srcs = new List<SrcPipe>(); {
				var connector_cons = inst.MEPModel.ConnectorManager.Connectors;
				foreach (Connector connector_con in connector_cons) {
					if (connector_con.AllRefs.Size != 1) 
						continue;
					foreach (Connector pipe_con in connector_con.AllRefs) 
						if (pipe_con.Owner is MEPCurve) 
							pipe_srcs.Add(new SrcPipe(pipe_con.Owner as MEPCurve, connector_con, pipe_con));
				}
			}
			return pipe_srcs;
		}
		private void DisconnectPipes() 
		{
			var connector_cons = m_familyInstance.MEPModel.ConnectorManager.Connectors;
			foreach (Connector connector_con in connector_cons) {
				foreach (Connector pipe_con in connector_con.AllRefs) 
					pipe_con.DisconnectFrom(connector_con);
			}
		}
		private bool PickObject2(ObjectType type, string msg, out Element element, out XYZ pt)
		{
			element = null;
			pt = null;
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
			pt = m_uidoc.Selection.PickPoint(snap, msg);
			var sorted_list = new SortedDictionary<double, MEPCurve>(); {
				IList<Element> mepCurves = null; {
					FilteredElementCollector col = null; {
						col = new FilteredElementCollector(m_doc).OfClass(typeof(MEPCurve));
						mepCurves = col.ToElements();
					}
				}
				foreach (MEPCurve mepCurve in mepCurves) {
					var c = GetCurve(mepCurve);
					double dist = c.Distance(pt);
					if (sorted_list.ContainsKey(dist) == false) {
						sorted_list.Add(dist, mepCurve);
					}
				}
			}
			if (sorted_list.Count > 0) {
				var enumerator = sorted_list.GetEnumerator();
				enumerator.MoveNext();
				element = enumerator.Current.Value;
				return true;
			}
			return false;
		}
		private void PickDestPipeSub() 
		{
			m_destPipe = null;
			MEPCurve pipe_dest = null; XYZ pickPoint = null; 	
			{
				while (true) {
					var msg = "Pick destination " + GetPipeTypeStr() + ".";
					Element element;
					bool b = PickObject2(ObjectType.Element, msg, out element, out pickPoint);
					if (!b)
						continue;

					if (element is MEPCurve) {
						bool bFindInSrcPipes = false; {
							foreach (var srcPipe in m_FamilyInstanceMepCurves) {
								if (srcPipe.m_mepCurve.Id == element.Id) {
									bFindInSrcPipes = true;
									break;
								}
							}
						}
						if (bFindInSrcPipes) {
							messageBox("That element is connected to the fitting. Pick a different "+GetPipeTypeStr()+".");
							continue;
						}
						pipe_dest = element as MEPCurve;
						var curve = (pipe_dest.Location as LocationCurve).Curve;
						var proj = curve.Project(pickPoint);
						pickPoint = proj.XYZPoint;
						break;
					} else {
						messageBox(msg);
					}
				}
			}
			m_destPipe = new DestPipe(pipe_dest, pickPoint);
		}
		private void PickMepCurve() 
		{
			while (true) {
				PickDestPipeSub();
				if (m_pipeType == PIPE_TYPE.PIPE && !(m_destPipe.m_mepCurve is Pipe) ||
					m_pipeType == PIPE_TYPE.DUCT && !(m_destPipe.m_mepCurve is Duct)) {
					messageBox("Select a " + GetPipeTypeStr() + ".");
					continue;
				}
				break;
			}
		}
		private bool _Equal(double t1, double t2) {
			return Math.Abs(t1 - t2) < tol;
 		}
		private bool _Equal(ElementId t1, ElementId t2) {
			return t1.Equals(t2);
		}
		private Curve GetCurve(MEPCurve pipe) {
			var loc = pipe.Location as LocationCurve;
			var srcCurve = loc.Curve;
			return srcCurve;
		}
		private XYZ GetDir(MEPCurve pipe) 
		{
			var curve = GetCurve(pipe);
			var deriv = curve.ComputeDerivatives(0, true);
			var v = deriv.BasisX;
			return v.Normalize();
		}
		private XYZ GetDir(Connector a, Connector b) {
			var v = a.Origin - b.Origin;
			return v.Normalize();
		}
		private string GetComments(Element e) 
		{
			var param = e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			var comments = param.AsString();
			return comments;
		}
		private void SetComments(Element e, string msg) {
			var param = e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			param.Set(msg);
		}
		private void FindPathSub(Node node, Element target) 
		{
			//var tag = node.Tag;
			foreach (Connector con in node.connectorSet) {
				foreach (Connector conCounter in con.AllRefs) {
					if (node.FindRev(conCounter.Owner) != null)
						continue;
					//var tag2 = GetComments(owner);
					if (conCounter.Owner is MEPCurve) {
						var newNode = new NodeMepCurve(node, conCounter, conCounter.Owner as MEPCurve);
						if (newNode.element.Id == target.Id)
							return;
						FindPathSub(newNode, target);
					} else if (conCounter.Owner is FamilyInstance) {
						var newNode = new NodeFamilyInstance(node, conCounter, conCounter.Owner as FamilyInstance);
						FindPathSub(newNode, target);
					} else if (conCounter.Owner is MEPSystem) {
						continue;
					} else {
						continue;
					}
				}
			}
		}
		private void FindPath() 
		{
			m_path = null;
			var paths = new SortedDictionary<int, List<Node>>(); {
				var nEnds = new List<Node>();	{
					MEPCurve target = m_destPipe.m_mepCurve;
					var nodeRoot = new NodeFamilyInstance(null, null, m_familyInstance); {
						FindPathSub(nodeRoot, target);
					}
					nodeRoot.Find(target, ref nEnds);
				}
				foreach (var nEnd in nEnds) {
					var paths2 = new List<Node>(); {
						nEnd.GetReversePath(ref paths2);
					}
					var cnt = paths2.Count;
					if (paths.ContainsKey(cnt) == false) {
						paths.Add(cnt, paths2);
					}
				}
			}
			if (paths.Count > 0) {
				var x = paths.GetEnumerator();
				x.MoveNext();
				m_path = x.Current.Value;
				m_path.Reverse();
			}
		}
		private void MoveFamilyInstance(XYZ pt, XYZ v1, XYZ v2)
		{
			m_familyInstance.Location.Move(m_familyInstance.GetTransform().Origin.Negate());
			var cross = v1.CrossProduct(v2);
			if (cross.GetLength() > tol) {
				cross = cross.Normalize();
				double angle = 0.0; {
					angle = v1.AngleOnPlaneTo(v2, cross);
				}
				m_familyInstance.Location.Rotate(Line.CreateBound(XYZ.Zero, cross), angle);
			}
			m_familyInstance.Location.Move(pt);
		}
		private void MoveFamilyInstanceByPath() 
		{
			var modelCurveNodes = new List<NodeMepCurve>(); {
				foreach (var node in m_path) {
					if (node is NodeMepCurve) 
						modelCurveNodes.Add(node as NodeMepCurve);
				}
			}
			Func<NodeMepCurve, XYZ> _GetDir = (node) => {
				var curve = GetCurve(node.m_mepCurve);
				var pt1 = curve.Evaluate(0, true);
				var pt2 = curve.Evaluate(1, true);
				var dir = pt2 - pt1;
				ASSERT(node.m_start != null);
				if (pt2.DistanceTo(node.m_start.Origin) < pt1.DistanceTo(node.m_start.Origin)) 
					dir = dir.Negate();
				return dir;
			};
			for (int i = 1; i < modelCurveNodes.Count; i++) {
				XYZ v1 = null; {
					var src = modelCurveNodes[i-1];
					v1 = _GetDir(src);
				}
				XYZ pt = null; XYZ v2 = null; {
					var dest = modelCurveNodes[i];
					v2 = _GetDir(dest);
					var curve = GetCurve(dest.m_mepCurve);
					pt = curve.Evaluate(0.5, true); {
						if (i == (modelCurveNodes.Count-1)) {
							pt = m_destPipe.m_pickPoint;
						}
					}
				}
				MoveFamilyInstance(pt, v1, v2);
			}
		}
		private void MoveFamilyInstanceDirect() 
		{
			XYZ v1 = null; {
				MEPCurve srcPipe = m_FamilyInstanceMepCurves[0].m_mepCurve; {
					foreach (var x in m_FamilyInstanceMepCurves) {
						var p1dir = GetDir(x.m_mepCurve);
						bool bFind = false;
						foreach (var y in m_FamilyInstanceMepCurves) {
							if (x == y)
								continue;
							var p2dir = GetDir(y.m_mepCurve);
							var normal = p1dir.CrossProduct(p2dir);
							if (normal.GetLength() < tol) {
								srcPipe = x.m_mepCurve;
								bFind = true;
								break;
							}
						}
						if (bFind)
							break;
					}
				}
				if (srcPipe == null) {
					messageBox("No "+GetPipeTypeStr()+" connects to this fitting.");
					return;
				}
				v1 = GetDir(srcPipe);
			}
			ASSERT(v1 != null);
			XYZ v2 = GetDir(m_destPipe.m_mepCurve);
			MoveFamilyInstance(m_destPipe.m_pickPoint, v1, v2);
		}
		private void MoveFamilyInstance() 
		{
			if (m_path != null)
				MoveFamilyInstanceByPath();
			else
				MoveFamilyInstanceDirect();
		}
		private void MergeFamilyInstanceMepCurves()
		{
			var targetMepCurves = new List<KeyValuePair<SrcPipe, SrcPipe> >(); {
				foreach (var p1 in m_FamilyInstanceMepCurves) {
					var v1 = GetDir(p1.m_mepCurve);
					foreach (var p2 in m_FamilyInstanceMepCurves) {
						if (p1 == p2)
							continue;
						bool bFind = false; {
							foreach (var p in targetMepCurves) {
								if (p2 == p.Key) {
									bFind = true;
									break;
								}
							}
						}
						if (bFind)
							continue;
						var v2 = GetDir(p2.m_mepCurve);
						var dot = v1.DotProduct(v2);
						if (_Equal(Math.Abs(dot), 1.0) ) 
							targetMepCurves.Add(new KeyValuePair<SrcPipe, SrcPipe>(p1, p2));
					}
				}
			}
			foreach (var targetMepCurve in targetMepCurves) {
				var key = targetMepCurve.Key;
				var value = targetMepCurve.Value;
				var socket = m_doc.Create.NewUnionFitting(key.m_mepCurveConnector, value.m_mepCurveConnector);
				m_doc.Delete(socket.Id);
			}
		}
		private Connector GetStartOrEndConnector(MEPCurve mepCurve, bool bStart) 
		{
			var c = GetCurve(mepCurve);
			double param = 0.0; {
				if (!bStart)
					param = 1.0;
			}
			var pt = c.Evaluate(param, true);
			var dic = new SortedDictionary<double, Connector>();
			foreach (Connector con  in mepCurve.ConnectorManager.Connectors) {
				var dist = con.Origin.DistanceTo(pt);
				dic.Add(dist, con);
			}
			var it = dic.GetEnumerator();
			if (it.MoveNext())
				return it.Current.Value;
			return null;
		}
		private Connector GetStartConnector(MEPCurve pipe) 
		{
			return GetStartOrEndConnector(pipe, true);
		}
		private Connector GetEndConnector(MEPCurve pipe)
		{
			return GetStartOrEndConnector(pipe, false);
		}
		private List<Element> _copy(ElementId id) 
		{
			var list = new List<Element>(); {
				var ids = ElementTransformUtils.CopyElement(m_doc, id, XYZ.Zero);
				foreach (var id2 in ids) {
					list.Add(m_doc.GetElement(id2));
				}
			}
			return list;
		}
		private void ResizeFittingPipeSub() 
		{
			double rad = 0; {
				Pipe pipe = m_destPipe.m_mepCurve as Pipe;
				rad = pipe.LookupParameter("直径").AsDouble();
				rad *= 0.5;
			}
			m_familyInstance.LookupParameter("呼び半径").Set(rad);
		}
		private void ResizeFittingDuctRectSub()
		{
			// 未実装
		}
		private void ResizeFittingDuctCircleSub() 
		{
			// 未実装
		}
		private void ResizeFittingDuctOvalSub() 
		{
			// 未実装
		}
		private void ResizeFittingDuctSub() 
		{
			// 未実装
		}
		private void ResizeFamilyInstance()
		{
			if (m_destPipe.m_mepCurve is Pipe) {
				if (m_familyInstance.Category.Name == "配管継手") {
					ResizeFittingPipeSub();
				}
			} else if (m_destPipe.m_mepCurve is Duct) {
				if (m_familyInstance.Category.Name == "ダクト継手") {
					ResizeFittingDuctSub();
				}
			} else {
				ASSERT(false);
			}
		}
		private void SplitDestMepCurve() 
		{
			Curve curveDestPipe = null; {
				curveDestPipe = GetCurve(m_destPipe.m_mepCurve);
				curveDestPipe = curveDestPipe.Clone();
			}
			var conPipeStart = GetStartConnector(m_destPipe.m_mepCurve);
			var conPipeEnd = GetEndConnector(m_destPipe.m_mepCurve);
			Connector conPipeEndCounterPart = null; {
				foreach (Connector con in conPipeEnd.AllRefs) {
					var owner = con.Owner;
					if (_Equal(m_destPipe.m_mepCurve.Id, owner.Id))
						continue;
					if (owner is MEPCurve) {
						continue;
					} else if (owner is FamilyInstance) {
						conPipeEndCounterPart = con;
						break;
					} else if (owner is MEPSystem) {
						continue;
					} else {
						continue;
					}
				}
			}
			var pipeDir = GetDir(conPipeEnd, conPipeStart);
			Connector conFittingStart = null; Connector conFittingEnd = null; {
				foreach (Connector con1 in m_familyInstance.MEPModel.ConnectorManager.Connectors) {
					var pt1 = con1.Origin;
					foreach (Connector con2 in m_familyInstance.MEPModel.ConnectorManager.Connectors) {
						if (_Equal(con1.Id, con2.Id))
							continue;
						var dir = GetDir(con1, con2);
						var dot = pipeDir.DotProduct(dir);
						if (_Equal(dot, 1.0)) {
							conFittingEnd = con1;
							conFittingStart = con2;
							break;
						}
					}
				}
			}
			if (conFittingStart == null)
				return;
			Func<Connector, Connector, bool> _connect = (a, b) => {
				a.Origin = b.Origin;
				a.ConnectTo(b);
				return true;
			};
			bool fitsOnCurveEndpoints = false; {
				var pt1 = conFittingStart.Origin;
				var pt2 = conFittingEnd.Origin;
				var dist1 = curveDestPipe.Distance(pt1);
				var dist2 = curveDestPipe.Distance(pt2);
				if (dist1 < tol && dist2 < tol)
					fitsOnCurveEndpoints = true;					
			}
			if (fitsOnCurveEndpoints) {
				MEPCurve newPipe = null;	{
					var list = _copy(m_destPipe.m_mepCurve.Id);
					ASSERT(list.Count == 1);
					newPipe = list[0] as MEPCurve;
				}
				_connect(conPipeEnd, conFittingStart);
				var newPipeConStart = GetStartConnector(newPipe);
				var newPipeConEnd = GetEndConnector(newPipe);
				_connect(newPipeConStart, conFittingEnd);
				if (conPipeEndCounterPart != null) {
					_connect(newPipeConEnd, conPipeEndCounterPart);
				}
			} else {
				Line line = Line.CreateBound(conFittingStart.Origin, conFittingEnd.Origin);
				var distEnd = line.Distance(conPipeEnd.Origin);
				if (distEnd < tol) {
					_connect(conPipeEnd, conFittingStart);
				} else {
					_connect(conPipeStart, conFittingEnd);
				}
			}
		}
		private void Init()
		{
			m_pipeType = PIPE_TYPE.PIPE;
			m_FamilyInstanceMepCurves = null;
			m_familyInstance = null;
			m_path = null;
		}
		private void ExecuteSub() 
		{
			Init();
			PickFamilyInstance();
			PickMepCurve();
			FindPath();
			DisconnectPipes();
			MoveFamilyInstance();
			MergeFamilyInstanceMepCurves();
			ResizeFamilyInstance();
			SplitDestMepCurve();
		}
		public Result Execute(	ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			m_uiapp = commandData.Application;
			m_uidoc = m_uiapp.ActiveUIDocument;
			m_app = m_uiapp.Application;
			m_doc = m_uidoc.Document;
			while (true) {
				// ESCキーが押されるまで処理を繰り返す。
				using (TransactionGroup transGroup  = new TransactionGroup(m_doc)) {
					transGroup.Start("Move Connector Group");
					using (var tran = new Transaction(m_doc, "Move Connector")) {
						try {
							tran.Start();
							ExecuteSub();
							tran.Commit();
							transGroup.Commit();
						} catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
							transGroup.RollBack();
							return Result.Succeeded;
						} catch (Exception e){
							message = e.Message;
							transGroup.RollBack();
							return Result.Failed;
						}
					}
				}
			}
			return Result.Succeeded;            
		}
	}
}
