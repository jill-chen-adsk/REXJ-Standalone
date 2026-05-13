/* ko-mimura
 *  エルボをまたいだ？コネクタ移動コマンド（サンプル。上手くいったらこのまま納品予定)
 *  (ジェネリック)
 *  使用方法
 *  １．Tなど配管付属品を選択する。
 *  ２．移動先のパイプを選択する。
 *  ３．移動先のパイプが分割され、Tなどの配管付属品と接続される。
 *  ４．Tともともと接続されていた部材は可能であれば、ひとつのパイプにマージされる。
 */
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using MEPCommon;
namespace CmdMoveConnector
{
	// -----------------------------------------
	// 移動物（付属品)ジェネリック
	// -----------------------------------------
	public class FittingGeneric : FittingA
	{
		// protected:
		protected readonly List<string> targetCategories = new List<string>() { "配管継手", "配管付属品", "ダクト継手", "ダクト付属品" };
		private readonly List<BuiltInCategory> TargetBuiltInCategories = new List<BuiltInCategory> {
			BuiltInCategory.OST_PipeFitting, BuiltInCategory.OST_PipeAccessory, BuiltInCategory.OST_DuctFitting, BuiltInCategory.OST_DuctAccessory
		};
		FamilyInstance m_familyInstance;
		protected override bool IsPipe(Element element)
		{
			return element is MEPCurve;
		}
		// public:
		public override Element element
		{
			get {
				return m_familyInstance;
			}
		}
		public override BoundingBoxXYZ boundingBox
		{
			get {
				var bbox = m_familyInstance.get_BoundingBox(null);
				return bbox;
			}
		}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		public FittingGeneric GetAsSkipper(FamilyInstance inst)
		{
			try {
				var sikipper = FittingA.Create(inst, false) as FittingGeneric;
				if (sikipper != null) {
					if (sikipper.IsATransision()) 
						return sikipper;
				}
			} catch {
			}
			return null;
		}
		public override bool IsSkipper()
		{
			return null != GetAsSkipper(element as FamilyInstance);
		}
		public override List<FittingA> GetSkippers()
		{
			var skippers = new List<FittingA>();
			foreach (Connector c in rowEndConnectors) {
				foreach (Connector r in c.AllRefs) {
					var inst = r.Owner as FamilyInstance;
					if (inst == null)
						continue;
					var skipper = GetAsSkipper(inst);
					if (skipper != null) 
						skippers.Add(skipper);
				}
			}
			return skippers;
		}
		public override bool GetSkipperOutConnector(Connector conFitting, out FittingGeneric outSkipper, out Connector outSkipperC)
		{
			outSkipper = null;
			outSkipperC = null;
			var rayConFitting = GetRay(conFitting);
			foreach (Connector r in conFitting.AllRefs) {
				var inst = r.Owner as FamilyInstance;
				if (inst == null)
					continue;
				var skipper = GetAsSkipper(inst);
				if (skipper == null)
					continue;
				foreach (Connector skipperC in skipper.rowEndConnectors) {
					var raySkipperC = GetRay(skipperC);
					if (rayConFitting.equalTo(raySkipperC)) {
						outSkipper = skipper;
						outSkipperC = skipperC;
						return true;
					}
				}
			}
			return false;
		}
#endif
	
		public override BoundingBoxXYZ boundingBoxLocal
		{
			get {
				return MepCommon.GetBoundingBoxLocal(m_familyInstance);
			}
		}
		public override Location location
		{
			get {
				return m_familyInstance.Location;
			}
		}
		public override UnitXYZ dir {
			get {
				return new UnitXYZ(m_familyInstance.GetTransform().BasisX);
			}
		}
		public override Transform transform
		{
			get {
				return m_familyInstance.GetTransform();
			}
		}
		protected override ConnectorSet connectorSet
		{
			get {
				return m_familyInstance.MEPModel.ConnectorManager.Connectors;
			}
		}
		protected PartType GetPartType()
		{
			return MepCommon.GetPartType(m_familyInstance);
		}
		public override bool IsGuage()
		{
			var type = GetPartType();
			return type == PartType.Sensor;
		}
		public override bool IsAElbow()
		{
			var type = GetPartType();
			return type == PartType.Elbow;
		}
		public override bool IsATransision()
		{
			var type = GetPartType();
			return type == PartType.Transition;
		}
		public override bool IsATap()
		{
			var type = GetPartType();
			return (type == PartType.TapAdjustable || type == PartType.TapPerpendicular);
		}
		public override void Validate()
		{
			base.Validate();
			{
				var categoryName = m_familyInstance.Category.Name;
				
				foreach ( var cat in TargetBuiltInCategories ) {
					if(cat == m_familyInstance.Category.BuiltInCategory) return;
				}
				

				
				
				var msg = "Incorrect category [" + categoryName + "]. Please select a pipe or duct accessory from one of these categories [";
				foreach (var name2 in targetCategories) {
					msg += name2;
					msg += ",";
				}
				msg += "]";
				throw new System.Exception(msg);
			}
		}
		public FittingGeneric(FamilyInstance inst, bool bCheck)
			:base(inst)
		{
			m_familyInstance = inst;
			if (bCheck)
				Validate();
			dirOrg = dir;
			transformOrg = transform;
		}
	}
	// -----------------------------------------
	// 移動先(直管)ジェネリック
	// -----------------------------------------
	class DestPipeGeneric : DestPipeA
	{
		MEPCurve m_mepCurve;
		public MEPCurve mepCurve
		{
			get {
				return m_mepCurve;
			}
		}
		private Curve GetCurve(MEPCurve pipe)
		{
			var loc = pipe.Location as LocationCurve;
			var srcCurve = loc.Curve;
			return srcCurve;
		}
		public override bool isPipe
		{
			get {
				if (m_mepCurve is Pipe)
					return true;
				return false;
			}
		}
		public override Element element {get {return m_mepCurve;}}
		public override Curve curve
		{
			get {
				var loc = m_mepCurve.Location as LocationCurve;
				return loc.Curve;
			}
		}
		protected override ConnectorSet connectorSet {get {return m_mepCurve.ConnectorManager.Connectors;}}
		public DestPipeGeneric(MEPCurve mepCurve, XYZ pickPoint)
			: base(pickPoint) 
		{
			m_mepCurve = mepCurve;
			if (!(m_mepCurve is Pipe || m_mepCurve is Duct))
				throw new System.Exception("Select pipe or duct.");
		}
	};
	// ---------------------------------
	public class NodeGenericStraight : NodeStraightA
	{
		protected MEPCurve m_mepCurve = null;
		public override Curve curve {
			get {
				var loc = m_mepCurve.Location as LocationCurve;
				return loc.Curve;
			}
		}
		protected override ConnectorSet connectorSet {get {return m_mepCurve.ConnectorManager.Connectors;}}
		public override string Tag {get {return MepCommon.GetTag(m_mepCurve);}}
		public override Element element {get {return m_mepCurve;	}}
		public NodeGenericStraight(NodeA par, Connector start, Connector prev, MEPCurve mepCurve)
			: base(par, start, prev) {
				m_mepCurve = mepCurve;
			}
	}
	// ---------------------------------
	public class NodeGenericFitting : NodeFittingA
	{
		public FamilyInstance m_familyInstance = null;
		protected override ConnectorSet connectorSet {get {return m_familyInstance.MEPModel.ConnectorManager.Connectors;}}
		public override string Tag { get {	return MepCommon.GetTag(m_familyInstance);	}}
		public override Element element {get {return m_familyInstance;}}
		public override BoundingBoxXYZ boundingBoxLocal { get {return MepCommon.GetBoundingBoxLocal(m_familyInstance);}}
		public override Transform transform { get { return m_familyInstance.GetTransform(); } }
		public NodeGenericFitting(NodeA par, Connector start, Connector prev, FamilyInstance inst)
			: base(par, start, prev) {
				m_familyInstance = inst;
			}
	}
	// ---------------------------------------
	public class MoveConnectorGeneric : MoveConnectorA
	{
		override protected List<Element> GetStraights() 
		{
			var inst = m_fitting.element as FamilyInstance;
			var categoryName = inst.Category.Name.ToLower();
			if (categoryName.Contains("配管") || categoryName.Contains("pipe")) {
				return MepCommon._findElementsByType(typeof(Pipe));
			} else if (categoryName.Contains("ダクト") || categoryName.Contains("duct")) {
				return MepCommon._findElementsByType(typeof(Duct));
			}
			MepCommon.ASSERT(false, "Selection is neither pipe nor duct.");
			return new List<Element>();
		}
		protected override void StickToOtherFitting(CollisionException e) 
		{
			var fab = m_fitting as FittingFabric;
			FittingA K = FittingA.Create(e.m_e, false);
			Connector cB = null;
			{
				var dic = new SortedDictionary<double, Connector>();
				{
					XYZ pt = null; {
						pt = K.transform.Origin;
					}
					foreach (Connector cc in m_destPipe.connectors) {
						var pt2 = cc.Origin;
						dic.Add(pt2.DistanceTo(pt), cc);
					}
				}
				var it = dic.GetEnumerator();
				if (it.MoveNext())
					cB = it.Current.Value;
			}
			ASSERT(cB != null);
			Connector cF = null;
			{
				foreach (var cc in K.endConnectors) {
					if (cB.IsConnectedTo(cc)) {
						cF = cc;
						break;
					}
				}
			}
			ASSERT(cF != null);
			Connector cD = null;
			{
				var dir1 = cF.CoordinateSystem.BasisZ;
				foreach (var cc in m_fitting.endConnectors) {
					var line = m_destPipe.curve as Line;
					line.MakeUnbound();
					var dist = line.Distance(cc.Origin);
					if (dist > 10e-5)
						continue;
					var dir2 = cc.CoordinateSystem.BasisZ;
					var dot = dir1.DotProduct(dir2);
					if (!MepCommon.Equal(dot, -1.0, 10e-5))
						continue;
					cD = cc;
					break;
				}
			}
			ASSERT(cD != null);
			{
				// 4
				var v = cB.Origin - cD.Origin;
				m_fitting.location.Move(v);
			}
			if (true) {
				// 5
				cF.ConnectTo(cD);
			}
			Connector cC = null;
			{
				var dir1 = cB.CoordinateSystem.BasisZ;
				//var dir1 = tB.BasisZ;
				foreach (var cc in m_fitting.endConnectors) {
					var line = m_destPipe.curve as Line;
					line.MakeUnbound();
					var dist = line.Distance(cc.Origin);
					if (dist > 10e-5)
						continue;
					var dir2 = cc.CoordinateSystem.BasisZ;
					var dot = dir1.DotProduct(dir2);
					if (!MepCommon.Equal(dot, -1, 10e-5))
						continue;
					cC = cc;
					break;
				}
			}
			ASSERT(cC != null);
			{
				// 7
				MepCommon.Disconnect(cB);
				cB.Origin = cC.Origin;
				cB.ConnectTo(cC);
			}
		}
		protected override void SplitPipe()
		{
			Connector conCounterPart = null; {
				foreach (Connector con in m_destPipe.EndConnector.AllRefs) {
					if (m_destPipe.element.Id.Equals(con.Owner.Id))
						continue;
					if (con.ConnectorType != ConnectorType.End)
						continue;
					if (con.Owner is FamilyInstance || con.Owner is Pipe || con.Owner is Duct) {
						conCounterPart = con;
						break;
					}
				}
			}
			Connector conFittingStart = null; Connector conFittingEnd = null;	{
				GetFittingStartAndEnd(ref conFittingStart, ref conFittingEnd);
			}
			if (conFittingStart == null)
				return;
			DestPipeA newPipe = null;	{
				Element ele = null; {
					var list = MepCommon.Copy(MepCommon.m_doc, m_destPipe.element.Id);
					ASSERT(list.Count == 1, "Expected exactly one copied element.");
					ele = list[0];
				}
				newPipe = DestPipeA.Create(ele, XYZ.Zero);
			}
			MepCommon.Connect(m_destPipe.EndConnector, conFittingStart);
			MepCommon.Connect(newPipe.StartConnector, conFittingEnd);
			if (conCounterPart != null)
				MepCommon.Connect(newPipe.EndConnector, conCounterPart);
		}
		protected override void CheckPathCheeze() 
		{
			// コネクタが3つ以上は分岐コネクタとみなす
			foreach (var node in m_path) {
				if (object.ReferenceEquals(node, m_path[0]))
					continue;
				if (node is NodeGenericFitting) {
					var nodeGenericFitting = node as NodeGenericFitting;
					if (nodeGenericFitting.targetConnectors.Count > 2)
						throw new RetrySelectiongPipe("Cannot move beyond a branch.");
				}
			}
		}
		protected override void CheckPathTap()
		{
			// タップをまたぐのはNG
			foreach (var node in m_path) {
				if (object.ReferenceEquals(node, m_path[0]))
					continue;
				if (node is NodeGenericFitting) {
					PartType partType; {
						var nodeGenericFitting = node as NodeGenericFitting;
						var f = nodeGenericFitting.m_familyInstance;
						Parameter partTypeParam = f.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
						if (partTypeParam == null)
							throw new System.Exception("GetPartType Error!");
						partType = (PartType)partTypeParam.AsInteger();
					}
					switch(partType)
					{
					case PartType.TapAdjustable:
					case PartType.TapPerpendicular:
						{
							throw new RetrySelectiongPipe("Cannot move across a tap.");
						}
					}
				}
			}
		}
		protected override bool IsMovingToOtherPipe() {
			foreach (var x in m_fitting.endConnectors) {
				foreach (Connector y in x.AllRefs) {
					if (y.Owner.Id.ToString() == m_destPipe.element.Id.ToString())
						return false;
				}
			}
			return true;
		}
		protected void CheckPathReducerPipe()
		{
			bool bPipe = false; {
				foreach (var node in m_path) {
					if (node is NodeGenericStraight) {
						if (node.element is Pipe) {
							bPipe = true;
							break;
						}
					}
				}
			}
			if (bPipe) {
				double w = double.MaxValue;
				foreach (var node in m_path) {
					if (node is NodeGenericStraight) {
						if (!(node.element is Pipe))
							throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
						var m = node.element as Pipe;
						if (!(w == double.MaxValue)) {
							if (!(MepCommon.Equal(w, m.Diameter)))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
						}
						w = m.Diameter;
					}
				}
				return;
			}


		}
		protected void CheckPathReducerDuct() 
		{
			bool bDuct = false;
			{
				foreach (var node in m_path) {
					if (node is NodeGenericStraight) {
						if (node.element is Duct) {
							bDuct = true;
							break;
						}
					}
				}
			}
			if (bDuct) {
				Func<ConnectorProfileType, bool> ShapeType = (t) => {
					foreach (var node in m_path) {
						if (node is NodeGenericStraight) {
							var nodeStraight = node as NodeGenericStraight;
							if (nodeStraight.m_c.Shape == t) {
								return true;
							}
						}
					}
					return false;
				};
				if (ShapeType(ConnectorProfileType.Round)) {
					double w = double.MaxValue;
					foreach (var node in m_path) {
						if (node is NodeGenericStraight) {
							if (!(node.element is Duct))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var con = (node as NodeGenericStraight).m_c;
							if (con.Shape != ConnectorProfileType.Round)
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var m = node.element as Duct;
							if (!(w == double.MaxValue)) {
								if (!(MepCommon.Equal(w, m.Diameter)))
									throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
							}
							w = m.Diameter;
						}
					}
				} else if (ShapeType(ConnectorProfileType.Rectangular)) {
					double w = double.MaxValue;
					double h = double.MaxValue;
					foreach (var node in m_path) {
						if (node is NodeGenericStraight) {
							if (!(node.element is Duct))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var con = (node as NodeGenericStraight).m_c;
							if (con.Shape != ConnectorProfileType.Rectangular)
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var m = node.element as Duct;
							if (!(w == double.MaxValue && h == double.MaxValue)) {
								if (!(MepCommon.Equal(w, m.Width) && MepCommon.Equal(h, m.Height)))
									throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
							}
							w = m.Width;
							h = m.Height;
						}
					}
				} else if (ShapeType(ConnectorProfileType.Oval)) {
					double w = double.MaxValue;
					double h = double.MaxValue;
					foreach (var node in m_path) {
						if (node is NodeGenericStraight) {
							if (!(node.element is Duct))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var con = (node as NodeGenericStraight).m_c;
							if (con.Shape != ConnectorProfileType.Oval)
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
							var m = node.element as Duct;
							if (!(w == double.MaxValue && h == double.MaxValue)) {
								if (!(MepCommon.Equal(w, m.Width) && MepCommon.Equal(h, m.Height)))
									throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
							}
							w = m.Width;
							h = m.Height;
						}
					}
				}
			}
		}
		protected override void MoveWithInConnectedPipe() 
		{
			var destPoint = m_destPipe.projectedPickPoint;
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
			var nodeFittings = m_fitting.FindNodeFittings2;
#else
			var nodeFittings = m_fitting.FindNodeFittings;
#endif
			var nodeStraights = m_fitting.FindNodeStraights;
			var t = new Transform(m_fitting.transform); {
				t.Origin = destPoint;
			}
			// 先に干渉チェック
			{
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				var a1 = GetFittingSolidInLocal();
#else
				var a1 = MepCommon.CreateBox(m_fitting.boundingBoxLocal);
#endif
				a1 = SolidUtils.CreateTransformed(a1, t);
				try {
					CheckCollisionDetection(a1);
				} catch (CollisionException e) {
					DisconnectPipes();
					StickToOtherFitting(e);
					MergePipes();
					return;
				}
			}
			Connector[] startEnd = { null, null };
			{
				startEnd[0] = m_destPipe.StartConnector;
				startEnd[1] = m_destPipe.EndConnector;
			}
			Connector cFit = null;
			{
				var tInv = t.Inverse;
				foreach (Connector c in startEnd) {
					if (c.IsConnected)
						continue;
					var ptInv = tInv.OfPoint(c.Origin);
					if (MepCommon.IsIn(m_fitting.boundingBoxLocal, ptInv)) {
						cFit = c;
						break;
					}
				}
			}
			XYZ cEnd = null;
			if (cFit != null) {
				Connector cFitOpposite = null; {
					if (object.ReferenceEquals(cFit, startEnd[0]))
						cFitOpposite = startEnd[1];
					else
						cFitOpposite = startEnd[0];
				}
				destPoint = cFit.Origin;
				var v = new UnitXYZ(cFit.Origin - cFitOpposite.Origin);
				{
					// パイプを伸ばしておく
					cFit.Origin += v * 100;
				}
				{
					// 端部にスナップする場合、端部のコネクタを探す
					var cEnds = new SortedDictionary<double, XYZ>();
					{
						var cc = new List<Connector>();
						{
							foreach (var c in nodeFittings)
								if (c.m_c != null)
									cc.Add(c.m_c);
							foreach (var c in m_fitting.endConnectors) {
								if (MepCommon.Find(cc, c) < 0)
									cc.Add(c);
							}
						}
						foreach (var c in cc) {
							var v2 = (c.Origin - m_fitting.transform.Origin);
							var dot = v.DotProduct(v2);
							if (dot < 0) {
								try {
									cEnds.Add(dot, c.Origin);
								} catch { }
							}
						}
					}
					if (cEnds.Count > 0) {
						var itr = cEnds.GetEnumerator();
						itr.MoveNext();
						cEnd = itr.Current.Value;
					}
				}
			}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
			Tuple<Connector, Connector>reConnect = null;
			foreach (var c in m_fitting.endConnectors) {
				foreach (Connector r in c.AllRefs) {
					var fitting = FittingA.Create(r.Owner, false);
					if (fitting != null) {
						r.DisconnectFrom(c);
						reConnect = new Tuple<Connector, Connector>(r, c);
					}
				}
			}
			MoveFitting(m_destPipe.projectedPickPoint, m_fitting.dir, m_destPipe.dir);
#else
			var orgs = new Dictionary<NodeFittingA, XYZ>();
			{
				foreach (var n in nodeFittings) {
					orgs.Add(n, n.transform.Origin);
				}
			}
			Connector reConnect1 = null;
			Connector reConnect2 = null;
			foreach (var n in nodeFittings) {
				if (n.element.Id.Value == m_fitting.element.Id.Value)
					continue;
				foreach (Connector x in n.fitting.endConnectors) {
					bool bFind = false;
					foreach (Connector y in x.AllRefs) {
						if (y.Owner.Id.Value == m_fitting.element.Id.Value) {
							x.DisconnectFrom(y);
							reConnect1 = x;
							reConnect2 = y;
							bFind = true;
						}
					}
					if (bFind) {
						break;
					}
				}
			}
			foreach (var n in nodeFittings) {
				if (n.element.Id.Value != m_fitting.element.Id.Value) 
					continue;
				var loc = n.element.Location;
				var pt1 = n.transform.Origin.Negate();
				var v1 = m_fitting.dir;
				var v2 = m_destPipe.dir;
				var pt2 = destPoint;
				{
					if (cEnd != null) {
						var o = orgs[n];
						pt2 += (o - cEnd);
					}
				}
				Move(loc, pt1, pt2, v1, v2);
			}
			if (reConnect1 != null) {
				MEPCurve mepNew = null; {
					var list = MepCommon.Copy(MepCommon.m_doc, m_destPipe.element.Id);
					ASSERT(list.Count == 1);
					mepNew = list[0] as MEPCurve;
				}
				var cc = new List<Connector>(); {
					foreach (Connector c in mepNew.ConnectorManager.Connectors)
						cc.Add(c);
				}
				var start = cc[0];
				var end = cc[1];
				start.Origin = reConnect1.Origin;
				start.ConnectTo(reConnect1);
				end.Origin = reConnect2.Origin;
				end.ConnectTo(reConnect2);
			}
#endif
			if (cFit != null) {
				MepCommon.m_doc.Delete(m_destPipe.element.Id); // 伸ばして削除する。
			}
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
			else 
			{
				if (reConnect != null) {
					MEPCurve mepNew = null; {
						var list = MepCommon.Copy(MepCommon.m_doc, m_destPipe.element.Id);
						ASSERT(list.Count == 1);
						mepNew = list[0] as MEPCurve;
					}
					var cc = new List<Connector>(); {
						foreach (Connector c in mepNew.ConnectorManager.Connectors)
							cc.Add(c);
					}
					var start = cc[0];
					var end = cc[1];
					var reConnect1 = reConnect.Item1;
					var reConnect2 = reConnect.Item2;
					start.Origin = reConnect1.Origin;
					start.ConnectTo(reConnect1);
					end.Origin = reConnect2.Origin;
					end.ConnectTo(reConnect2);
				}
			}
#endif
		}
		protected override void CheckPathReducer()
		{
			CheckPathReducerPipe();
			if (m_bCHECK_REDUCER_DUCT) {
				CheckPathReducerDuct();
			}
		}
		protected override void FitConnect(Connector c1, Connector c2) 
		{
			MepCommon.Connect(c1, c2);
		}
	}
}
