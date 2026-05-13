/* ko-mimura
 *  エルボをまたいだ？コネクタ移動コマンド（サンプル。上手くいったらこのまま納品予定)
 *  (基本クラス)
 *  使用方法
 *  １．Tなど配管付属品を選択する。
 *  ２．移動先のパイプを選択する。
 *  ３．移動先のパイプが分割され、Tなどの配管付属品と接続される。
 *  ４．Tともともと接続されていた部材は可能であれば、ひとつのパイプにマージされる。
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using MEPCommon;
using System.Linq;

namespace CmdMoveConnector
{
	// ---------------------------------
	// 節点情報(基本)
	// ---------------------------------
	public abstract class NodeA
	{
		public int m_level = 0;
		public NodeA        m_par           = null;
		public Connector   m_c      = null;
		public Connector m_prev = null;
		public List<NodeA>  m_children  = new List<NodeA>();
		public UnitXYZ connecctorDir { get { return new UnitXYZ(m_c.CoordinateSystem.BasisZ); } }
		public void GetReversePath(ref List<NodeA> nodes)
		{
			nodes.Add(this);
			if (m_par != null)
				m_par.GetReversePath(ref nodes);
		}
		public void Find(Element e, ref List<NodeA>ret)
		{
			if (element.Id == e.Id)
				ret.Add(this);
			foreach (var node in m_children)
				node.Find(e, ref ret);
		}
		public void FindFirstConnectednodeStraight(ref List<NodeStraightA> ret) 
		{
			if (this is NodeStraightA) {
				ret.Add(this as NodeStraightA);
			} else {
				foreach (var node in m_children)
					node.FindFirstConnectednodeStraight(ref ret);
			}
		}
		public void FindFittingsByFirstConnectedStraight(ref List<NodeFittingA> ret) {
			if (this is NodeFittingA)
				ret.Add(this as NodeFittingA);
			foreach (var node in m_children) {
				if (node is NodeFittingA)
					node.FindFittingsByFirstConnectedStraight(ref ret);
			}
		}
		public NodeA FindRev(Element e)
		{
			if (element.Id == e.Id)
				return this;
			if (m_par != null) {
				return m_par.FindRev(e);
			}
			return null;
		}
		protected abstract ConnectorSet connectorSet { get; }
		public static bool IsTargrt(Connector c) 
		{
			if (c.ConnectorType == ConnectorType.End || c.ConnectorType == ConnectorType.Surface || c.ConnectorType == ConnectorType.Curve) 
				return true;
			return false;
		}
		public List<Connector> targetConnectors {
			get {
				var list = new List<Connector>();
				foreach (Connector c in connectorSet)
					if (IsTargrt(c))
						list.Add(c);
				return list;
			}
		}
		public abstract Element element { get; }
		public abstract string Tag { get; }
		public NodeA(NodeA par, Connector start, Connector prev)
		{
			m_par = par;
			m_c = start;
			m_prev = prev;
			if (m_par != null) {
				m_level = m_par.m_level + 1;
				m_par.m_children.Add(this);
			}
		}
		public void DrawBoundingBox(bool bInclueSub) 
		{
			var ele = this.element;
			if (ele != null) {
				var solids = MepCommon.GetSolids(ele);
				foreach (var a1 in solids)
					MepCommon.DrawSolidAsWires(a1);
			}
			if (bInclueSub)
				foreach (var x in m_children)
					x.DrawBoundingBox(bInclueSub);
		}

		public static NodeA CreateNode(NodeA par, Connector start, Connector prev, Element ele)
		{
			if (ele is FamilyInstance)
				return new NodeGenericFitting(par, start, prev, ele as FamilyInstance);
			else if (ele is MEPCurve)
				return new NodeGenericStraight(par, start, prev, ele as MEPCurve);
			else if (ele is FabricationPart) {
				var fab = ele as FabricationPart;
				if (fab.IsAStraight())
					return new NodeFabricStraight(par, start, prev, fab);
				else
					return new NodeFabricFitting(par, start, prev, fab);
			}
			return null;
		}
	};
	public abstract class NodeFittingA : NodeA
	{
		public abstract BoundingBoxXYZ boundingBoxLocal { get; }
		public abstract Transform transform { get; }
#if FIX_SKIPPER // (ko-mimura 20206/01/31)
		public FittingA fitting {
			get {
				var f = FittingA.Create(element, false);
				return f;
			}
		}
#endif
		public void DrawBondingBox() 
		{
			var a1 = MepCommon.CreateBox(boundingBoxLocal);
			a1 = SolidUtils.CreateTransformed(a1, transform);
			MepCommon.DrawSolidAsWires(a1);
		}
		public NodeFittingA(NodeA par, Connector start, Connector prev)
			: base(par, start, prev) {
		}
	}
	public abstract class NodeStraightA : NodeA
	{
		public abstract Curve curve { get; }
		public Connector OppositeConnector 
		{ 
			get {
				var cons = MepCommon.GetEndConnectors(connectorSet);
				foreach (Connector c in cons) 
					if (this.m_c.Id != c.Id)
						return c;
				return null;
			}
		}
		public NodeStraightA(NodeA par, Connector start, Connector prev) :base(par, start, prev){}
	}
	// -----------------------------------------
	// 移動物（付属品)基本クラス
	// -----------------------------------------
	public abstract class FittingA
	{
		public NodeA m_rootNode = null;
		protected abstract ConnectorSet connectorSet { get; }
		protected abstract bool IsPipe(Element element);
		protected List<Element> m_nodes = new List<Element>();
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		protected Ray GetRay(Connector c)
		{
			return new Ray(c.Origin, new UnitXYZ(c.CoordinateSystem.BasisZ));
		}
		public abstract bool  IsSkipper();
		public abstract List<FittingA> GetSkippers();
		public abstract bool GetSkipperOutConnector(Connector conFitting, out FittingGeneric outSkipper, out Connector outSkipperC);
#endif
		protected bool FindProNode(Element ele)
		{
			foreach (Element e in m_nodes) {
				if (e.Id == ele.Id)
					return true;
			}
			return false;
		}
		protected void CreateTreeNodeSub(NodeA node) 
		{
			if (node.m_level > 50)
				return;
			foreach (Connector c in node.targetConnectors) {
				foreach (Connector r in c.AllRefs) {
					if (NodeA.IsTargrt(r) == false)
						continue;
					var ele = r.Owner;
					 if (node.FindRev(ele) != null)
					       continue;
					if (FindProNode(ele))
						continue;
					NodeA newNode = NodeA.CreateNode(node, r, c, ele);
					if (newNode == null)
						continue;
					m_nodes.Add(ele);
					CreateTreeNodeSub(newNode);
				}
			}
		}
		public void DrawNodeAll()
		{
			m_rootNode.DrawBoundingBox(true);
		}
		void CreateTreeNode(Element ele)
		{
			m_nodes.Clear();
			m_rootNode = NodeA.CreateNode(null, null, null, ele);
			CreateTreeNodeSub(m_rootNode);
			//TEST
//TEST			DrawNodeAll();
			//TEST
		}
		public List<NodeStraightA> FindNodeStraights 
		{ 
			get {
				var nodes = new List<NodeStraightA>();
				m_rootNode.FindFirstConnectednodeStraight(ref nodes);
				return nodes;
			} 
		}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		public void FindNodeFittingsSub(NodeA node, ref List<NodeFittingA> vec) 
		{
			var nodeFittingA = node as NodeFittingA;
			if (nodeFittingA != null) {
				if (!nodeFittingA.fitting.IsSkipper()) {
					vec.Add(nodeFittingA);
					return;
				}
			}
			foreach (var child in node.m_children)
				FindNodeFittingsSub(child, ref vec);
		}
		public List<NodeFittingA> FindNodeFittings2
		{
			get {
				var vec = new List<NodeFittingA>(); {
					var rootF = m_rootNode as NodeFittingA;
					vec.Add(rootF);
					foreach (var child in m_rootNode.m_children)
						FindNodeFittingsSub(child, ref vec);
				}
				return vec;
			}
		}
#endif
		public List<NodeFittingA> FindNodeFittings
		{
			get {
				var nodes = new List<NodeFittingA>();
				m_rootNode.FindFittingsByFirstConnectedStraight(ref nodes);
				return nodes;
			}
		}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		public List<Connector> rowEndConnectors {
			get {
				return MepCommon.GetEndConnectors(connectorSet);
			}
		}
#endif
		
		public List<Connector> endConnectors {
			get {
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				if (IsSkipper()) {
					return rowEndConnectors;
				}
				var vec = new List<Connector>();
				foreach (Connector c in rowEndConnectors) {
					FittingGeneric outSkipper;
					Connector outSkipperC;
					if (GetSkipperOutConnector(c, out outSkipper, out outSkipperC)) 
						vec.Add(outSkipperC);
					else
						vec.Add(c);
				}
				return vec;
#else
				return MepCommon.GetEndConnectors(connectorSet);
#endif
			}
		}
		public abstract Element element { get; }
		public abstract BoundingBoxXYZ boundingBox { get; }
		public abstract BoundingBoxXYZ boundingBoxLocal { get; }
		public abstract Location location { get; }
		public abstract UnitXYZ dir { get; }
		public UnitXYZ dirOrg;
		public abstract Transform transform { get; }
		public Transform transformOrg;
		public static FittingA Create(Element ele, bool bCheck = true)
		{
			if (ele is FamilyInstance)
				return new FittingGeneric(ele as FamilyInstance, bCheck);
			else if (ele is FabricationPart)
				return new FittingFabric(ele as FabricationPart, bCheck);
			return null;
		}
		protected void ThrowException(string str)
		{
			throw new System.Exception(str);
		}
		public abstract bool IsGuage();
		public abstract bool IsAElbow();
		public abstract bool IsATransision();
		public abstract bool IsATap();
		public virtual bool IsAHiroidasi()
		{
			return connectorSet.Size == 0;
		}
		public virtual void Validate()
		{
			/*
			以下の部材を除外
			 a.エルボ(移動先は原則直管です。)
			 b.置換部材(移動先の左右の形状および、サイズが異なることは想定していません。）
			 c.タップ(移動先は原則直管のため。)
			 d.拾い出し（コネクタ無し。）
			 */
			if (IsGuage()) {
				ThrowException("Gages cannot be moved.");
			}
			if (IsAElbow()) {
				ThrowException("Elbows cannot be moved. (Destination is normally straight duct.)");
			}
			if (IsATransision()) {
				ThrowException("Transitions cannot be moved. (Left/right shape and size at destination are assumed to match.)");
			}
			if (IsATap()) {
				ThrowException("Tap fittings cannot be moved. (Destination is normally straight duct.)");
			}
			if (IsAHiroidasi()) {
				ThrowException("Takeouts cannot be moved. (No connectors.)");
			}
		}
		public FittingA(Element ele)
		{
			CreateTreeNode(ele);
		}
	}
	// -----------------------------------------
	// 移動先(直管)基本クラス
	// -----------------------------------------
	public abstract class DestPipeA
	{
		private XYZ m_pickPoint;
		private Connector GetConnector(XYZ pt)
		{
			var dic = new SortedDictionary<double, Connector>();{
				foreach (Connector con in connectors)
					dic.Add(con.Origin.DistanceTo(pt), con);
			}
			var it = dic.GetEnumerator();
			if (it.MoveNext())
				return it.Current.Value;
			return null;
		}
		public XYZ projectedPickPoint {
			get {
				// その都度計算しないと、1e-10レベルで数値が変わっている。
				// 1e-10レベルの誤差で、Fabrication.SplitStraight()が分割失敗する。
				var proj = curve.Project(m_pickPoint);
				return proj.XYZPoint;
			}
		}
		public UnitXYZ dir{get {return MepCommon.GetDir(curve);}}
		public abstract bool isPipe { get; }
		public abstract Element element { get; }
		public abstract Curve curve { get; }
		protected abstract ConnectorSet connectorSet { get; }
		public List<Connector> connectors {
			get {
				return MepCommon.GetEndConnectors(connectorSet);
			}
		}
		public Connector StartConnector
		{
			get {
				return GetConnector(curve.Evaluate(0.0, true));
			}
		}
		public Connector EndConnector
		{
			get {
				return GetConnector(curve.Evaluate(1.0, true));
			}
		}
		public static DestPipeA Create(Element ele, XYZ pickPoint)
		{
			if (ele is MEPCurve) {
				return new DestPipeGeneric(ele as MEPCurve, pickPoint);
			} else if (ele is FabricationPart) {
				var fab = ele as FabricationPart;
				if (fab.IsAStraight()) {
					return new DestPipeFabric(fab, pickPoint);
				}
			}
			return null;
		}
		public DestPipeA(XYZ pickPoint)
		{
			m_pickPoint = pickPoint;
		}
	};
	// ---------------------------------------
	// 例外
	// ---------------------------------------
	public class RetrySelectiongFitting : System.Exception
	{
		public RetrySelectiongFitting(string msg)
			:base(msg)
		{
		}
	};
	public class RetrySelectiongPipe : System.Exception
	{
		public RetrySelectiongPipe(string msg)
			:base(msg)
		{
		}
	};
	public class CollisionException : System.Exception
	{
		public Element m_e;
		public CollisionException(Element e) {
			m_e = e;
		}
	}
	// ---------------------------------------
	// エルボを超えた移動コマンド
	// ---------------------------------------
	public abstract class MoveConnectorA : SelectionUtilIF
	{
		// --------------------------------
		protected const bool m_bCHECK_REDUCER_DUCT = true;
		// --------------------------------
		protected DestPipeA       m_destPipe;
		protected List<NodeA>  m_path;
		protected FittingA          m_fitting;
		protected double tol {	get{	return MepCommon.tol;}}
		protected double AngleTolerance { get { return MepCommon.m_AngleTolerance; } }
		protected double VertexTolerance { get { return MepCommon.m_VertexTolerance; } }
		protected double ShortCurveTolerance { get { return MepCommon.m_ShortCurveTolerance; } }
		protected void ASSERT(bool b, string message = "")
		{
			MepCommon.ASSERT(b, message);
		}
		protected void GetFittingStartAndEnd(ref Connector conFittingStart, ref Connector conFittingEnd)
		{
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
			foreach (Connector con1 in m_fitting.rowEndConnectors) {
#else
			foreach (Connector con1 in m_fitting.endConnectors) {
#endif
				if (con1.ConnectorType != ConnectorType.End)
					continue;
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				foreach (Connector con2 in m_fitting.rowEndConnectors) {
#else
				foreach (Connector con2 in m_fitting.endConnectors) {
#endif
					if (con1.Id == con2.Id)
						continue;
					if (con2.ConnectorType != ConnectorType.End)
						continue;
					var dir = GetDir(con1, con2);
					var dot = m_destPipe.dir.DotProduct(dir);
					if (MepCommon.Equal(dot, 1.0)) {
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
						Func<Connector, Connector> _skipperConnector = (c) => {
							FittingGeneric outSkipper;
							Connector outSkipperC;
							if (m_fitting.GetSkipperOutConnector(c, out outSkipper, out outSkipperC))
								return outSkipperC;
								return c;
						};
						conFittingEnd = _skipperConnector(con1);
						conFittingStart =_skipperConnector(con2);
#else
						conFittingEnd = con1;
						conFittingStart = con2;
#endif
						break;
					}
				}
			}
		}
		protected Element PickObject(string msg, out XYZ pickPoint)
		{
			var reference = MepCommon.m_uidoc.Selection.PickObject(ObjectType.Element, msg);
			var ele = MepCommon.m_uidoc.Document.GetElement(reference);
			pickPoint = reference.GlobalPoint;
			if (ele == null)
				MessageBox.Show(msg);
			return ele;
		}
		abstract protected List<Element> GetStraights();
		public List<Element> SelectionUtilIF_GetStraights() 
		{
			return GetStraights();
		}
		protected void DisconnectPipes()
		{
			foreach (Connector c in m_fitting.endConnectors) 
				MepCommon.Disconnect(c);
		}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		protected Solid GetFittingSolidInLocal()
		{
			var aFitting = MepCommon.CreateBox(m_fitting.boundingBoxLocal);
			var skippers = m_fitting.GetSkippers();
			foreach (var skipper in skippers) {
				var aSkipper = MepCommon.CreateBox(skipper.boundingBoxLocal); {
					Transform trans = null; {
						var tFittingInv = m_fitting.transform.Inverse;
						trans = skipper.transform;
						trans = tFittingInv.Multiply(trans);
					}
					aSkipper = SolidUtils.CreateTransformed(aSkipper, trans);
				}
				aFitting = BooleanOperationsUtils.ExecuteBooleanOperation(aFitting, aSkipper, BooleanOperationsType.Union);
			}
			//TEST
			// MepCommon.DrawSolidAsWires(aFitting);
			//TEST
			return aFitting;
		}
#endif
		protected void PickPipe()
		{
			m_destPipe = null;
			while (true) {
				var msg = "Select destination straight duct or fabrication straight.";
				XYZ pickPoint = null; Element element = null; {
					if (MepCommon.m_uidoc.ActiveView is View3D) {
						element = PickObject(msg, out pickPoint);
					} else {
						var selectionUtil = new SelectionUtil(this);
						element = selectionUtil.PickPoint(msg, out pickPoint);
					}
				}
				m_destPipe = DestPipeA.Create(element, pickPoint);
				if (m_destPipe == null) {
					MessageBox.Show(msg);
					continue;
				}
				break;
			}
		}
		protected Curve GetCurve(MEPCurve pipe)
		{
			var loc = pipe.Location as LocationCurve;
			var srcCurve = loc.Curve;
			return srcCurve;
		}
		protected Curve GetCurve(FabricationPart fab)
		{
			ASSERT(fab.IsAStraight(), "Selection is not a straight fabrication segment.");
			var loc = fab.Location as LocationCurve;
			return loc.Curve;
		}
		protected Curve GetCurve(Element ele)
		{
			if (ele is FabricationPart)
				return GetCurve(ele as FabricationPart);
			if (ele is MEPCurve)
				return GetCurve(ele as MEPCurve);
			return null;
		}
		protected UnitXYZ GetDir(Curve curve) {return MepCommon.GetDir(curve);}
		protected UnitXYZ GetDir(MEPCurve pipe){return GetDir(GetCurve(pipe));}
		protected UnitXYZ GetDir(Connector a, Connector b) {return new UnitXYZ(a.Origin - b.Origin);}
		protected string GetComments(Element e)
		{
			var param = e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			var comments = param.AsString();
			return comments;
		}
		protected void SetComments(Element e, string msg)
		{
			var param = e.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
			param.Set(msg);
		}
		protected void FindPath()
		{
			m_path = null;
			var paths = new SortedDictionary<int, List<NodeA>>(); {
				var nEnds = new List<NodeA>();	{
					var target = m_destPipe.element;
					m_fitting.m_rootNode.Find(target, ref nEnds);
				}
				foreach (var nEnd in nEnds) {
					var paths2 = new List<NodeA>(); {
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
		protected void CheckCollisionDetection(Solid a1)
		{
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
			var skippers = m_fitting.GetSkippers();
#endif
			var elements = new List<Element>(); {
				Connector[] cStartEnd = { m_destPipe.StartConnector, m_destPipe.EndConnector };
				foreach (Connector c in cStartEnd) {
					var refs = MepCommon.GetEndConnectors(c.AllRefs);
					foreach (Connector r in refs) {
						if (r.Owner.Id == m_fitting.element.Id)
							continue;
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
						bool bSkip = false; {
							foreach (var skipper in skippers) {
								if (r.Owner.Id.ToString() == skipper.element.Id.ToString()) {
									bSkip = true;
									break;
								}
							}
						}
						if (bSkip)
							continue;
#endif
						if (r.Owner is FamilyInstance) {
							elements.Add(r.Owner);
						} else if (r.Owner is FabricationPart) {
							var fab = r.Owner as FabricationPart;
							if (!fab.IsAStraight()) {
								elements.Add(r.Owner);
							}
						}
					}
				}
			}
			foreach (Element e in elements) {
				var solids = MepCommon.GetSolids(e);
				foreach (var a2 in solids) {
					if (MepCommon.hasIntersection(a1, a2)) {
						throw new CollisionException(e);
						// throw new RetrySelectiongPipe("配管付属品から離れた位置に配置してください。");
					}
				}
			}
		}
		protected void Move(Location location, XYZ pt1, XYZ pt2, UnitXYZ v1, UnitXYZ v2) 
		{
			location.Move(pt1);
			var cross = v1.CrossProduct(v2);
			if (cross.GetLength() > this.ShortCurveTolerance) {// tol) {
				cross = cross.Normalize();
				double angle = 0.0; {
					angle = v1.AngleOnPlaneTo(v2, cross);
				}
				location.Rotate(Line.CreateBound(XYZ.Zero, cross), angle);
			}
			location.Move(pt2);
		}
		protected void MoveFitting(XYZ pt2, UnitXYZ v1, UnitXYZ v2) 
		{
#if FIX_SKIPPER //(ko-mimura 2020/01/31)
			var fittings = m_fitting.GetSkippers(); {
				fittings.Add(m_fitting);
			}
			foreach (var f in fittings)
				Move(f.location, f.transform.Origin.Negate(), pt2, v1, v2);
#else
			Move(m_fitting.location, m_fitting.transform.Origin.Negate(), pt2, v1, v2);
#endif
		
		}
		protected abstract void FitConnect(Connector c1, Connector c2);
		protected bool Fit()
		{
			Connector cFit = null; {
				var boundingBoxLocal = m_fitting.boundingBoxLocal;
				var transInv = m_fitting.transform.Inverse;
				Connector[] startEnd = { m_destPipe.StartConnector, m_destPipe.EndConnector };
				foreach (Connector c in startEnd) {
					if (c.IsConnected)
						continue;
					var ptInv = transInv.OfPoint(c.Origin);
					if (MepCommon.IsIn(boundingBoxLocal, ptInv)) {
						cFit = c;
						break;
					}
				}
			}
			if (cFit != null) {
				var dir1 = new UnitXYZ(cFit.CoordinateSystem.BasisZ);
				foreach (Connector c in m_fitting.endConnectors) {
					var dir2 = new UnitXYZ(c.CoordinateSystem.BasisZ);
					var dot = dir1.DotProduct(dir2);
					if (MepCommon.Equal(dot, -1)) {
						var pt = cFit.Origin + (m_fitting.transform.Origin - c.Origin);
						m_fitting.location.Move(m_fitting.transform.Origin.Negate());
						m_fitting.location.Move(pt);
						FitConnect(cFit, c);
						break;
					}
				}
				return true;
			}
			return false;
		}
		protected void MoveFittingByPath()
		{
			var nodes = new List<NodeStraightA>(); {
				foreach (var node in m_path) {
					if (node is NodeStraightA)
						nodes.Add(node as NodeStraightA);
				}
			}
			Func<NodeStraightA, UnitXYZ> _GetDir = (nodeline) => {
				var curve = nodeline.curve;
				var pt1 = curve.Evaluate(0, true);
				var pt2 = curve.Evaluate(1, true);
				var dir = pt2 - pt1;
				ASSERT(nodeline.m_c != null, "No node connector.");
				if (pt2.DistanceTo(nodeline.m_c.Origin) < pt1.DistanceTo(nodeline.m_c.Origin))
					dir = dir.Negate();
				return new UnitXYZ(dir);
			};
			var v1 = m_fitting.dir;
			foreach (NodeStraightA node in nodes) {
				var v2 = _GetDir(node);
				XYZ pt = null; {
					if (object.ReferenceEquals(node, nodes[nodes.Count - 1])) {
						pt = m_destPipe.projectedPickPoint;
					} else {
						pt = node.curve.Evaluate(0.5, true);
					}
				}
				MoveFitting(pt, v1, v2);
				v1 = v2;
			}
		}
		protected abstract void StickToOtherFitting(CollisionException e);
		protected void MoveFitting()
		{
			if (m_path != null)
				MoveFittingByPath();
			else
				MoveFitting(m_destPipe.projectedPickPoint, m_fitting.dir, m_destPipe.dir);
			bool bFitted = Fit();
			try {
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				var a1 = GetFittingSolidInLocal();
#else
				var a1 = MepCommon.CreateBox(m_fitting.boundingBoxLocal);
#endif
				{
					a1 = SolidUtils.CreateTransformed(a1, m_fitting.transform);
				}
				CheckCollisionDetection(a1);
			} catch (CollisionException e) {
				StickToOtherFitting(e);
				return;
			}
			if (!bFitted)
				SplitPipe();
		}
		protected void CreateGuideLine(Line line, Plane plane, Color col = null)
		{
			var sketchPlane = SketchPlane.Create(MepCommon.m_doc, plane);
			try {
				var modelCurve = MepCommon.m_doc.Create.NewModelCurve(line, sketchPlane);
				{
					GraphicsStyle gs = modelCurve.LineStyle as GraphicsStyle;
					var cat = gs.GraphicsStyleCategory;
					if (col == null)
						col = new Color(255, 0, 0);
					cat.LineColor = col;
				}
			} catch {
			}
		}
		protected void CreateGuideLine(Line line, Color col = null)
		{
			Plane plane = null;
			{
				XYZ normal = null;
				{
					var vLine = new UnitXYZ(line.Direction);
					if (XYZ.BasisZ.CrossProduct(vLine).GetLength() < this.ShortCurveTolerance)// tol)
						normal = XYZ.BasisX;
					else
						normal = XYZ.BasisZ.CrossProduct(vLine).Normalize();
				}
				plane = Plane.CreateByNormalAndOrigin(normal, line.Evaluate(0, true));
			}
			CreateGuideLine(line, plane, col);
		}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		NodeA SkipSkipperAndDeletedPartsLikeFlange(NodeA x) 
		{
			//ファブリケーションパーツの削除された部品（フランジなど）をスキップする。
			if (x.element.IsValidObject) {
				bool bSkipper = false; {
					NodeFittingA nodeFittingA = x as NodeFittingA;
					if (nodeFittingA != null) {
						if (nodeFittingA.fitting.IsSkipper()) 
							bSkipper = true;
					}
				}
				if (!bSkipper) {
					if (MepCommon.Equal(Math.Abs(m_fitting.dirOrg.DotProduct(x.connecctorDir)), 1.0))
						return x;
				}
			}
			foreach (var y in x.m_children) {
				var z = SkipSkipperAndDeletedPartsLikeFlange(y);
				if (z != null)
					return z;
			}
			return null;
		}
#else
		NodeA SkipDeletedPartsLikeFlange(NodeA x) 
		{
			//ファブリケーションパーツの削除された部品（フランジなど）をスキップする。
			bool b = x.element.IsValidObject;
			if (b) {
				if (MepCommon.Equal(Math.Abs(m_fitting.dirOrg.DotProduct(x.connecctorDir)), 1.0))
					return x;
			}
			foreach (var y in x.m_children) {
				var z = SkipDeletedPartsLikeFlange(y);
				if (z != null)
					return z;
			}
			return null;
		}
#endif
		protected void MergePipes() 
		{
			// パイプの再接続.xlsx
			var dels = new List<int>();
			var nodes = m_fitting.m_rootNode.m_children;
			for (int i = 0; i < nodes.Count; i++) {
				bool bFind = false;
				{
					foreach (var del in dels)
						if (del == i) {
							bFind = true;
							break;
						}
				}
				if (bFind)
					continue;
				var x = nodes[i];
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				x = SkipSkipperAndDeletedPartsLikeFlange(x);
#else
				x = SkipDeletedPartsLikeFlange(x);
#endif
				if (x == null)
					continue;
				for (int j = i + 1; j < nodes.Count; j++) {
					bFind = false;
					{
						foreach (var del in dels)
							if (del == j) {
								bFind = true;
								break;
							}
					}
					if (bFind)
						continue;
					var y = nodes[j];
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
					y = SkipSkipperAndDeletedPartsLikeFlange(y);
#else
					y = SkipDeletedPartsLikeFlange(y);
#endif
					if (y == null)
						continue;
					if (x is NodeFittingA && y is NodeFittingA) {
						// type1
						var xFitting = x as NodeFittingA;
						var yFitting = y as NodeFittingA;
						MEPCurve mepNew = null;
						{
							var list = MepCommon.Copy(MepCommon.m_doc, m_destPipe.element.Id);
							ASSERT(list.Count == 1);
							mepNew = list[0] as MEPCurve;
						}
						var c = GetCurve(mepNew);
						var pt1 = c.Evaluate(0.0, true);
						var pt2 = c.Evaluate(1.0, true);
						var loc = mepNew.Location as LocationCurve;
						loc.Move(pt1.Negate());
						double ang = 0.0; XYZ axis = XYZ.Zero;
						{
							var v1 = (pt2 - pt1).Normalize();
							var v2 = (yFitting.m_c.Origin - xFitting.m_c.Origin).Normalize();
							axis = v1.Normalize().CrossProduct(v2);
							var dot = v1.DotProduct(v2);
							{
								if (MepCommon.Equal(dot, 1.0))
									dot = 1.0;
								if (MepCommon.Equal(dot, -1.0))
									dot = -1.0;
							}
							ang = Math.Acos(dot);
						}
						if (axis.GetLength() > MepCommon.m_ShortCurveTolerance) {
							loc.Rotate(Line.CreateUnbound(XYZ.Zero, axis), -ang);
						}
						loc.Move(xFitting.m_c.Origin);
						{
							c = GetCurve(mepNew);
							pt1 = c.Evaluate(0.0, true);
							pt2 = c.Evaluate(1.0, true);
							var cons = MepCommon.GetEndConnectors(mepNew.ConnectorManager.Connectors);
							ASSERT(cons.Count == 2);
							var dist1 = cons[0].Origin.DistanceTo(pt1);
							var dist2 = cons[0].Origin.DistanceTo(pt2);
							Connector c1 = null;
							Connector c2 = null;
							if (dist1 < dist2) {
								c1 = cons[0];
								c2 = cons[1];
							} else {
								c1 = cons[1];
								c2 = cons[0];
							}
							c1.Origin = xFitting.m_c.Origin;
							xFitting.m_c.ConnectTo(c1);
							c2.Origin = yFitting.m_c.Origin;
							yFitting.m_c.ConnectTo(c2);
						}
					} else if (x is NodeStraightA && y is NodeFittingA) {
						// type2
						var xStraight = x as NodeStraightA;
						var yFitting = y as NodeFittingA;
						xStraight.m_c.Origin = yFitting.m_c.Origin;
						xStraight.m_c.ConnectTo(yFitting.m_c);
					} else if (x is NodeFittingA && y is NodeStraightA) {
						// type3
						var xFitting = x as NodeFittingA;
						var yStraight = y as NodeStraightA;
						yStraight.m_c.Origin = xFitting.m_c.Origin;
						yStraight.m_c.ConnectTo(xFitting.m_c);
					} else if (x is NodeStraightA && y is NodeStraightA) {
						// type4
						var xStraight = x as NodeStraightA;
						var yStraight = y as NodeStraightA;
						var oppositeCon = yStraight.OppositeConnector;
						xStraight.m_c.Origin = oppositeCon.Origin;
						if (oppositeCon.IsConnected) {
							var ownerId = oppositeCon.Owner.Id;
							foreach (Connector con in oppositeCon.AllRefs) {
								var ownerId2 = con.Owner.Id;
								if (!ownerId.Equals(ownerId2)) {
									xStraight.m_c.ConnectTo(con);
									break;
								}
							}
						}
						MepCommon.m_doc.Delete(yStraight.element.Id);
						dels.Add(j);
					} else {
						throw new System.Exception("MergePipes error.");
					}
				}
			}
		}
		protected abstract void SplitPipe();
		protected bool IsEndCap()
		{
			return m_fitting.endConnectors.Count == 1;
		}
		protected abstract bool IsMovingToOtherPipe();
		//												ダクト	配管
		// チーズをまたぐ移動（分岐コネクタ）						NG	NG
		protected abstract void CheckPathCheeze();
		//												ダクト	配管
		// 十字継ぎ手をまたぐ移動（分岐コネクタ）					NG	NG
		protected virtual void CheckPathCross()
		{
			CheckPathCheeze();
		}
		//												ダクト	配管
		// エルボをまたぐ移動									OK	OK
		protected virtual void CheckPathElbow()
		{
			// 無視
		}
		//												ダクト	配管
		// タップをまたぐ？移動								NG	NG
		protected abstract void CheckPathTap();
		//												ダクト	配管
		// ストレーナをまたぐ移動								OK	OK
		protected virtual void CheckPathStriiner()
		{
			// 無視
		}
		//												ダクト	配管
		// レデューサをまたぐ移動								OK	NG
		// 異なるサイズへの移動								OK	（NG）※1
		protected abstract void CheckPathReducer();
		protected void CheckPath()
		{
			if (m_path == null)
				throw new RetrySelectiongPipe("Could not detect a route.");
			//												ダクト	配管
			// チーズをまたぐ移動（分岐コネクタ）						NG	NG
			CheckPathCheeze();
			// 十字継ぎ手をまたぐ移動（分岐コネクタ）					NG	NG
			CheckPathCross();
			// エルボをまたぐ移動									OK	OK
			CheckPathElbow();
			// タップをまたぐ？移動								OK	OK
			CheckPathTap();
			// ストレーナをまたぐ移動								OK	OK
			CheckPathStriiner();
			// レデューサをまたぐ移動								OK	NG
			// 異なるサイズへの移動								OK	（NG）※1
			CheckPathReducer();
		}
		protected void MoveToOtherPipe()
		{
			FindPath();
			CheckPath();
			DisconnectPipes();
			MoveFitting();
			MergePipes();
		}
		protected abstract void MoveWithInConnectedPipe();
		protected void ReShapeConnector(Connector dest, Connector src)
		{
			ASSERT(dest.Shape == src.Shape, "Connector shapes do not match.");
			try {
				if (dest.Shape == ConnectorProfileType.Round) {
					dest.Radius = src.Radius;
				} else if (dest.Shape == ConnectorProfileType.Rectangular) {
					dest.Width = src.Width;
					dest.Height = src.Height;
				} else if (dest.Shape == ConnectorProfileType.Oval) {
					dest.Width = src.Width;
					dest.Height = src.Height;
				}
			} catch (Exception e) {
			}
		}
		protected void MoveEndCap()
		{
			Connector con = null; {
				ASSERT(m_fitting.endConnectors.Count== 1, "Expected a single connector for end cap.");
				con = m_fitting.endConnectors[0];
			}
			var cons2 = m_destPipe.connectors;
			foreach (Connector con2 in cons2) {
				if (!con2.IsConnected) {
					DisconnectPipes();
					MoveFitting(con2.CoordinateSystem.Origin, 
							new UnitXYZ(con.CoordinateSystem.BasisZ),
							new UnitXYZ(con2.CoordinateSystem.BasisZ.Negate()));
					con.ConnectTo(con2);
					if (con.Shape == con2.Shape) {
						ReShapeConnector(con, con2);
					}
					return;
				}
			}
			throw new RetrySelectiongPipe("Failed to move the cap.");
		}
		public void Execute(FittingA fitting)
		{
			m_fitting = fitting;
			if (m_destPipe == null)
				PickPipe();
			if (IsEndCap()) {
				MoveEndCap();
			} else {
				if (IsMovingToOtherPipe())
					MoveToOtherPipe();
				else
					MoveWithInConnectedPipe();
			}
		}
		public void Execute(FittingA fitting, DestPipeA destPipeA) 
		{
			m_destPipe = destPipeA;
			Execute(fitting);
		}
	}
	// ---------------------------------------
	// エルボを超えた移動コマンド
	// ---------------------------------------
	[Transaction(TransactionMode.Manual)]
	public class MoveConnector : IExternalCommand
	{
		protected void DisableWarning(Transaction tran)
		{
			FailureHandlingOptions failureHandlingOptions
				= tran.GetFailureHandlingOptions();
			FailureHandler failureHandler
				= new FailureHandler();
			failureHandlingOptions.SetFailuresPreprocessor(
				failureHandler);
			failureHandlingOptions.SetClearAfterRollback(
				true);
			tran.SetFailureHandlingOptions(
				failureHandlingOptions);
		}
		public static Transaction m_tran;
		public Result ExecuteSub(ref string message) 
		{
			Element element = null;
			while (true) {
				// ESCキーが押されるまで処理を繰り返す。
				using (TransactionGroup transGroup = new TransactionGroup(MepCommon.m_doc)) {
					transGroup.Start("Move Connector Group");
					using (var tran = new Transaction(MepCommon.m_doc, "Move Connector")) {
						try {
							m_tran = tran;
							DisableWarning(tran);
							tran.Start();
							if (element == null) {
								var elms = MepCommon.ConvCollectionToList(MepCommon.m_uidoc.Selection.GetElementIds());
								if (elms.Count == 1) {
									element = MepCommon.m_doc.GetElement(elms[0]);
									MepCommon.m_uidoc.Selection.SetElementIds(new List<ElementId>());
								} else {
									XYZ pickPoint = null;
									var reference = MepCommon.m_uidoc.Selection.PickObject(ObjectType.Element, "Please select a pipe or duct accessory.");
									element = MepCommon.m_uidoc.Document.GetElement(reference);
									pickPoint = reference.GlobalPoint;
								}
							}
							if (element == null) {
								MessageBox.Show("Please select a pipe or duct accessory.");
							} else {
								var fitting = FittingA.Create(element);
								var fittingGeneric = fitting as FittingGeneric;
								var fittingFabric = fitting as FittingFabric;
								if (fittingGeneric != null)
									(new MoveConnectorGeneric()).Execute(fittingGeneric);
								else if (fittingFabric != null)
									(new MoveConnectorFabric()).Execute(fittingFabric);
								else
									throw new System.Exception("Please select a pipe or duct accessory.");
							}
							tran.Commit();
							transGroup.Commit();
							element = null;
						} catch (RetrySelectiongFitting e) {
							if (e.Message != "")
								MessageBox.Show(e.Message);
							transGroup.RollBack();
						} catch (RetrySelectiongPipe e) {
							if (e.Message != "")
								MessageBox.Show(e.Message);
							transGroup.RollBack();
						} catch (CollisionException) {
							MessageBox.Show("Place farther from the accessory.");
							transGroup.RollBack();
						} catch (Autodesk.Revit.Exceptions.OperationCanceledException) {
							transGroup.RollBack();
							return Result.Succeeded;
						} catch (Exception e) {
							message = e.Message;
							MessageBox.Show(e.Message);
							transGroup.RollBack();
							element = null;
						}
					}
				}
			}
			return Result.Succeeded;
		}
		private List<FITTING> TestCollectFittings<FITTING, INST>(string markPipe, string markDuct) 
			where FITTING : FittingA  
			where INST : Element
		{
			var fittings = new List<FITTING>();	{
				var insts = MepCommon._findElementsByType(typeof(INST));
				foreach (INST inst in insts) {
					var tag = MepCommon.GetTag(inst);
					if (tag == null)
						continue;
					if (Regex.IsMatch(tag, markPipe) || Regex.IsMatch(tag, markDuct)) {
						try {
							var fitting = FittingA.Create(inst) as FITTING;
							if (fitting != null)
								fittings.Add(fitting);
						} catch (Exception ) {

						}
					}
				}
			}
			return fittings;
		}
		private List<DESTPIPE> TestCollectPipes<DESTPIPE, INST>(string markPipe, string markDuct) 
			where DESTPIPE : DestPipeA 
			where INST : Element
		{
			var destPipeGenerics = new List<DESTPIPE>(); {
				var lines = MepCommon._findElementsByType(typeof(CurveElement));
				var insts = MepCommon._findElementsByType(typeof(INST));
				foreach (INST inst in insts) {
					var tag = MepCommon.GetTag(inst);
					if (tag == null)
						continue;
					if (Regex.IsMatch(tag, markPipe) || Regex.IsMatch(tag, markDuct)) {
						XYZ pickPoint = null;	{
							var loc = inst.Location as LocationCurve;
							if (loc != null) {
								foreach (var line in lines) {
									SetComparisonResult x;
									IList<CurveOverlapPoint> overlaps = null; {
										var loc2 = line.Location as LocationCurve;
										var pt1 = loc2.Curve.Evaluate(0, true);
										var pt2 = loc2.Curve.Evaluate(1, true);
										var z1 = loc.Curve.Evaluate(0, true).Z;
										var z2 = loc.Curve.Evaluate(1, true).Z;
										MepCommon.ASSERT(MepCommon.Equal(z1, z2));
										var line2 = Line.CreateBound(new XYZ(pt1.X, pt1.Y, z1), new XYZ(pt2.X, pt2.Y, z2));
										var intersectResult = loc.Curve.Intersect(line2, CurveIntersectResultOption.Detailed);
										x = intersectResult.Result;
										overlaps = intersectResult.GetOverlaps();
									}
									if (x == SetComparisonResult.Overlap && overlaps != null) {
										foreach (var overlapPt in overlaps) {
											pickPoint = overlapPt.Point;
										}
									}
									if (pickPoint != null)
										break;
								}
								if (pickPoint == null) {
									pickPoint = loc.Curve.Evaluate(0.5, true);
								}
							} else {
								continue;
							}
						}
						MepCommon.ASSERT(pickPoint != null);
						var dp = DestPipeA.Create(inst, pickPoint) as DESTPIPE;
						if (dp != null)
							destPipeGenerics.Add(dp);
					}
				}
			}
			return destPipeGenerics;
		}
		private void TestMoveConnectorA<FITTING, DESTPIPE, MOVEFUNC>(List<FITTING> fittings, List<DESTPIPE> pipes) 
			where FITTING : FittingA 
			where DESTPIPE : DestPipeA 
			where MOVEFUNC : MoveConnectorA ,new()
		{
			foreach (var fitting in fittings) {
				var tagFitting = MepCommon.GetTag(fitting.element);
				DESTPIPE pipe = null;{
					foreach (var p in pipes) {
						try {
							var tagPipe = MepCommon.GetTag(p.element);
							if (tagFitting == tagPipe) {
								pipe = p;
								break;
							}
						} catch (Exception) {

						}
					}
				}
				MepCommon.ASSERT(pipe != null);
				if (pipe == null)
					continue;
				using (var tran = new Transaction(MepCommon.m_doc, "Move Connector")) {
					try {
						DisableWarning(tran);
						tran.Start(); {
							var cmd = new MOVEFUNC();
							cmd.Execute(fitting, pipe);
						}
						tran.Commit();
					} catch (Exception e) {
						MessageBox.Show(e.Message);
						tran.RollBack();
					}
				}
			}
		}
		public void ExecuteTest() 
		{
			string TestFileName = "附属品の移動.rvt";
			string fileName; {
				var path = MepCommon.m_doc.PathName;
				fileName = System.IO.Path.GetFileName(path);
			}
			if (fileName != TestFileName) {
				MessageBox.Show("Please open test project [" + TestFileName + "].");
				return;
			}
			{
				string[] merks = { "GENERIC_PIPE_TEST_", "GENERIC_DUCT_TEST_" };
				var fittings = TestCollectFittings<FittingGeneric, FamilyInstance>(merks[0], merks[1]);
				var pipes = TestCollectPipes<DestPipeGeneric, MEPCurve>(merks[0], merks[1]);
				TestMoveConnectorA<FittingGeneric, DestPipeGeneric, MoveConnectorGeneric>(fittings, pipes);
			}
			{
				string[] merks = { "FABRIC_PIPE_TEST_", "FABRIC_DUCT_TEST_" };
				var fittings = TestCollectFittings<FittingFabric, FabricationPart>(merks[0], merks[1]);
				var pipes = TestCollectPipes<DestPipeFabric, FabricationPart>(merks[0], merks[1]);
				TestMoveConnectorA<FittingFabric, DestPipeFabric, MoveConnectorFabric>(fittings, pipes);
			}
		}
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
///UnitTest.Execute();
			MepCommon.Init(commandData);
			bool bTest = false; {
				var MoveConnectorTest = Environment.GetEnvironmentVariable("MoveConnectorTest");
				if (MoveConnectorTest != null)
					bTest = true;
			}
			if (bTest) {
				ExecuteTest();
				return Result.Succeeded;
			}
			return ExecuteSub(ref message);
		}
	}
}

