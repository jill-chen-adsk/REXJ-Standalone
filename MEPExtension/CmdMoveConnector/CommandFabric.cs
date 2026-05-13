/* ko-mimura
 *  エルボをまたいだ？コネクタ移動コマンド（サンプル。上手くいったらこのまま納品予定)
 *  (ファブリック)
 *  使用方法
 *  １．Tなど配管付属品を選択する。
 *  ２．移動先のパイプを選択する。
 *  ３．移動先のパイプが分割され、Tなどの配管付属品と接続される。
 *  ４．Tともともと接続されていた部材は可能であれば、ひとつのパイプにマージされる。
 */
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using MEPCommon;
namespace CmdMoveConnector
{
	// -----------------------------------------
	// 移動物（付属品)ファブリック
	// -----------------------------------------
	public class FittingFabric : FittingA
	{
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
		public override bool IsSkipper()
		{
			return false;
		}
		public override List<FittingA> GetSkippers()
		{
			return new List<FittingA>();
		}
		public override bool GetSkipperOutConnector(Connector conFitting, out FittingGeneric outSkipper, out Connector outSkipperC)
		{
			outSkipper = null;
			outSkipperC = null;
			return false;
		}
#endif
		// protected:
		protected FabricationPart m_fab;
		protected override bool IsPipe(Element element) 
		{
			var ff = element as FabricationPart;
			if (ff == null)	
				return false;
			return ff.IsAStraight();
		}
		// public:
		public override Element element
		{
			get {
				return m_fab;
			}
		}
		public override BoundingBoxXYZ boundingBox {
			get {
				var bbox = m_fab.get_BoundingBox(null);
				return bbox;
			}
		}
		public override BoundingBoxXYZ boundingBoxLocal
		{
			get {
				return MepCommon.GetBoundingBoxLocal(m_fab);
			}
		}
		public override Location location
		{
			get {
				return m_fab.Location;
			}
		}
		public override UnitXYZ dir
		{
			get {
				return new UnitXYZ(m_fab.GetTransform().BasisX);
			}
		}
		public override Transform transform
		{
			get {
				return m_fab.GetTransform();
			}
		}
		protected override ConnectorSet connectorSet
		{
			get {
				return m_fab.ConnectorManager.Connectors;
			}
		}
		public override bool IsGuage()
		{

			return false;
		}
		public override bool IsAElbow()
		{
			///const double tol = 1e-3;
			var cons = new List<Connector>();
			{
				foreach (Connector con in connectorSet)
					cons.Add(con);
			}
			if (cons.Count != 2)
				return false;
			var dir1 = cons[0].CoordinateSystem.BasisZ;
			var dir2 = cons[1].CoordinateSystem.BasisZ;
			if (dir1.CrossProduct(dir2).GetLength() < MepCommon.m_ShortCurveTolerance)// tol)
				return false;
			return true;
		}
		public override bool IsATap()
		{
			return m_fab.IsATap();
		}
		public bool _Equals(double a, double b, double tol)
		{
			return System.Math.Abs(a - b) < tol;
		}
		public override bool IsATransision()
		{
			///const double tol = 1e-3;
			var cons = new List<Connector>();
			{
				foreach (Connector con in connectorSet)
					cons.Add(con);
			}
			if (cons.Count != 2)
				return false;
			var con1 = cons[0];
			var con2 = cons[1];
			if (con1.Shape != con2.Shape)
				return false;
			if (con1.Shape == ConnectorProfileType.Oval || con1.Shape == ConnectorProfileType.Rectangular) {
				if (MepCommon.Equal(con1.Width, con2.Width))
					return false;
				if (MepCommon.Equal(con1.Height, con2.Height))
					return false;
			} else if (con1.Shape == ConnectorProfileType.Round) {
				if (MepCommon.Equal(con1.Radius, con2.Radius))
					return false;
			}
			return true;
		}
		public override void Validate()
		{
			base.Validate();
			if (m_fab.IsAStraight())
				throw new System.Exception("Please select a pipe or duct accessory.");
		}
		public FittingFabric(FabricationPart fab, bool bCheck)
			:base(fab)
		{
			m_fab = fab;
			if (bCheck)
				Validate();
			dirOrg = dir;
			transformOrg = transform;
		}
	}
	// -----------------------------------------
	// 移動先(直管)ファブリック
	// -----------------------------------------
	class DestPipeFabric : DestPipeA
	{
		FabricationPart m_fab;
		public override Element element
		{
			get {
				return m_fab;
			}
		}
		public override Curve curve
		{
			get {
				var loc = m_fab.Location as LocationCurve;
				return loc.Curve;
			}
		}
		public override bool isPipe
		{
			get {
				if (m_fab.Size.IndexOf('x') > -1)
					return false;
				return true;
			}
		}
		protected override ConnectorSet connectorSet
		{
			get {
				return m_fab.ConnectorManager.Connectors;
			}
		}
		private double mmToFeet(double mm) {
			return UnitUtils.Convert(mm, UnitTypeId.Millimeters, UnitTypeId.Feet);
		}
		public DestPipeFabric(FabricationPart fab, XYZ pickPoint) 
			:base(pickPoint)
		{
			m_fab = fab;
		}
	};
	// ---------------------------------
	public class NodeFabricStraight : NodeStraightA
	{
		public FabricationPart m_fab = null;
		public override Curve curve { 
			get {
				var loc = m_fab.Location as LocationCurve;
				return loc.Curve;
			}
		}
		protected override ConnectorSet connectorSet { get { return m_fab.ConnectorManager.Connectors; } }
		public override string Tag { get { return MepCommon.GetTag(m_fab); } }
		public override Element element { get { return m_fab; } }
		public NodeFabricStraight(NodeA par, Connector start, Connector prev, FabricationPart fab)
			: base(par, start, prev)
		{
			m_fab = fab;
			MepCommon.ASSERT(fab.IsAStraight(), "Not a fabrication straight.");
		}
	}
	// ---------------------------------
	public class NodeFabricFitting : NodeFittingA
	{
		public FabricationPart m_fab = null;
		protected override ConnectorSet connectorSet { get { return m_fab.ConnectorManager.Connectors; } }
		public override string Tag { get { return MepCommon.GetTag(m_fab); } }
		public override Element element { get { return m_fab; } }
		public override BoundingBoxXYZ boundingBoxLocal { get {	return MepCommon.GetBoundingBoxLocal(m_fab);}}
		public override Transform transform { get { return m_fab.GetTransform(); } }
		public NodeFabricFitting(NodeA par, Connector start, Connector prev, FabricationPart fab)
			: base(par, start, prev)
		{
			m_fab = fab;
			MepCommon.ASSERT(!fab.IsAStraight(), "Expected a fabrication fitting, not a straight.");
		}
	}
	// ---------------------------------------
	// エルボを超えた移動コマンド(ファブリック)
	// ---------------------------------------
	public class MoveConnectorFabric : MoveConnectorA
	{
		override protected List<Element> GetStraights() 
		{
			var elements = new List<Element>();
			var fabs = MepCommon._findElementsByType(typeof(FabricationPart));
			foreach (FabricationPart fab in fabs)
				if (fab.IsAStraight())
					elements.Add(fab);
			return elements;
		}
		protected override void StickToOtherFitting(CollisionException e) 
		{
			var fab = m_fitting as FittingFabric;
			FittingA K = FittingA.Create(e.m_e, false);
			Connector cB = null;
			{
				var dic = new SortedDictionary<double, Connector>();
				{
					XYZ pt = null;
					{
						//var lp = K.location as LocationPoint;
						//pt = lp.Point;
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
			//var tB = new Transform(cB.CoordinateSystem);
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
			if (cD == null)
				return;
			ASSERT(cD != null);
			{
				// 4
				var v = cB.Origin - cD.Origin;
				m_fitting.location.Move(v);
			}
			if (true) {
				// 5
				MepCommon.Disconnect(cB);
				cD.Origin = cF.Origin;
				cD.ConnectTo(cF);
				//TEST FabricationPart.ConnectAndCouple(MepCommon.m_doc, cD, cF);
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
				//TEST FabricationPart.ConnectAndCouple(MepCommon.m_doc, cB, cC);

			}
		}
		protected void SplitPipeForPipe()
		{
			FabricationPart[] straights = new FabricationPart[2] { null, null}; {
				var orgStraight = m_destPipe.element as FabricationPart;
				FabricationPart newStraight = null; {
					var pt = m_destPipe.projectedPickPoint;
					var ele = MepCommon.m_doc.GetElement(orgStraight.SplitStraight(pt));
					ASSERT(ele is FabricationPart, "Not a fabrication part.");
					newStraight = ele as FabricationPart;
				}
				straights[0] = orgStraight;
				straights[1] = newStraight;
			}
			var con_orgs = new List<KeyValuePair<Connector, Connector >> ();
			foreach (FabricationPart straight in straights) {
				{
					// 直管のすべての接続を外しておかないとFabricationPart.AlignPartByConnectorsでエラーとなる
					// FabricationPart.AlignPartByConnectorsを先に呼ばないと
					// FabricationPart.ConnectAndCoupleがエラーとなる。
					var list = MepCommon.GetEndConnectors(straight.ConnectorManager.Connectors);
					foreach (Connector c in list) {
						foreach (Connector cc in c.AllRefs) {
							c.DisconnectFrom(cc);
							con_orgs.Add(MepCommon.make_pair(c, cc));
						}
					}
				}
				Connector conStraight = null; {
					var list = new List<Connector>(); {
						var list2 = MepCommon.GetEndConnectors(straight.ConnectorManager.Connectors);
						foreach (Connector con in list2)
							if (con.IsConnected == false)
								list.Add(con);
					}
					if (list.Count == 0)
						continue;
					double max_dist = double.MaxValue;
					foreach (Connector con in list) {
						var f = m_fitting.element as FabricationPart;
						var dist = con.Origin.DistanceTo(f.Origin);
						if (dist < max_dist) {
							conStraight = con;
							max_dist = dist;
						}
					}
				}
				Connector conFittng = null; {
					var fabFitting = m_fitting.element as FabricationPart;
					foreach (Connector con in fabFitting.ConnectorManager.Connectors) {
						if (con.ConnectorType != ConnectorType.End)
							continue;
						if (con.IsConnected)
							continue;
						bool bConnect = false; {
							var v1 = conStraight.CoordinateSystem.BasisZ;
							var v2 = con.CoordinateSystem.BasisZ;
							var dot = v1.DotProduct(v2);
							bConnect = MepCommon.Equal(dot, -1);
						}
						if (bConnect) {
							conFittng = con;
							break;
						}
					}
				}
				if (conFittng != null) {
					FabricationPart.AlignPartByConnectors(MepCommon.m_doc, conStraight, conFittng, 0);
					FabricationPart.ConnectAndCouple(MepCommon.m_doc, conStraight, conFittng);
				}
			}
			foreach (var x in con_orgs) {
				x.Key.Origin = x.Value.Origin;
				x.Key.ConnectTo(x.Value);
			}
		}
		protected void SplitPipeForDuct() {
			FabricationPart[] straights = new FabricationPart[2] { null, null };
			{
				var orgStraight = m_destPipe.element as FabricationPart;
				FabricationPart newStraight = null;
				{
					var pt = m_destPipe.projectedPickPoint;
					var id = orgStraight.SplitStraight(pt);
					var ele = MepCommon.m_doc.GetElement(id);
					ASSERT(ele is FabricationPart, "Not a fabrication part.");
					newStraight = ele as FabricationPart;
				}
				straights[0] = orgStraight;
				straights[1] = newStraight;
			}
			foreach (FabricationPart straight in straights) 
			{
				Connector conStraight = null; {
					var list = new List<Connector>(); {
						var list2 = MepCommon.GetEndConnectors(straight.ConnectorManager.Connectors);
						foreach (Connector con in list2)
							if (con.IsConnected == false)
								list.Add(con);
					}
					if (list.Count == 0)
						continue;
					double max_dist = double.MaxValue;
					foreach (Connector con in list) {
						var f = m_fitting.element as FabricationPart;
						var dist = con.Origin.DistanceTo(f.Origin);
						if (dist < max_dist) {
							conStraight = con;
							max_dist = dist;
						}
					}
				}
				Connector conFittng = null; {
					var fabFitting = m_fitting.element as FabricationPart;
					foreach (Connector con in fabFitting.ConnectorManager.Connectors) {
						if (con.ConnectorType != ConnectorType.End)
							continue;
						if (con.IsConnected)
							continue;
						bool bConnect = false; {
							var v1 = conStraight.CoordinateSystem.BasisZ;
							var v2 = con.CoordinateSystem.BasisZ;
							var dot = v1.DotProduct(v2);
							bConnect = MepCommon.Equal(dot, -1);
						}
						if (bConnect) {
							conFittng = con;
							break;
						}
					}
				}
				if (conFittng != null) {
					conStraight.Origin = conFittng.Origin;
					conStraight.ConnectTo(conFittng);
				}
			}
		}
		protected bool IsSmallPartLikeCoupling(FabricationPart f)
		{
			ASSERT(f.IsValidObject);
			ConnectorDomainType cdType;	{
				var fabFitting = m_fitting.element as FabricationPart;
				cdType = fabFitting.DomainType;
			}
			if (cdType == ConnectorDomainType.Piping) {
				foreach (Parameter p in f.Parameters) {
					if (p.Definition.Name == "ファミリ") {
						var str = p.AsValueString();
						if (str == null)
							continue;
						// パイプのカップリングとかジョイントとかは無視(現行のプログラム上の動作に合うように)
						str = str.ToLower();
						if (str.IndexOf("coupling") > -1)
							return true;
						if (str.IndexOf("joint") > -1)
							return true;
					}
				}
			}
			return false;
		}
		protected override bool IsMovingToOtherPipe()
		{
			var nss = m_fitting.FindNodeStraights;
			foreach (var ns in nss) {
				if (ns.element.Id == m_destPipe.element.Id) {
					var rpath = new List<NodeA>();
					ns.GetReversePath(ref rpath);
					for (int i =1; i < (rpath.Count-1); i++) {
						var f = rpath[i].element as FabricationPart;
						if (!f.IsValidObject)
							continue;
						if (!IsSmallPartLikeCoupling(f))
							return true;
					}
					return false;
				}
			}
			return true;
		}
		NodeA FindPipeOrFitting(NodeA x) 
		{
			var f = x.element as FabricationPart;
			if (f.IsValidObject &&  !IsSmallPartLikeCoupling(f)) {
				return x;				
			}
			foreach (NodeA y in x.m_children) {
				var z = FindPipeOrFitting(y);
				if (z != null)
					return z;
			}
			return null;
		}
		NodeA FindPipeOrFittingR(NodeA x) {
			var f = x.element as FabricationPart;
			if (!IsSmallPartLikeCoupling(f)) {
				return x;
			}
			if (x.m_par != null) {
				var z = FindPipeOrFitting(x.m_par);
				if (z != null)
					return z;
			}
			return null;
		}
		NodeA FindNext(NodeA node) 
		{
			if (node.element.IsValidObject) {
				return node;
			}
			foreach (var x in node.m_children) {
				var y = FindNext(x);
				if (y != null)
					return y;
			}
			return null;
		}
		protected FabricationPart CopyFab(FabricationPart fab) 
		{
			FabricationPart copyFab = null; {
				var list = MepCommon.Copy(MepCommon.m_doc, fab.Id);
				ASSERT(list.Count == 1);
				copyFab = list[0] as FabricationPart;
			}
			return copyFab;
		}
		protected void MoveWithInConnectedPipe_2_A() {
			var destPoint = m_destPipe.projectedPickPoint;
			var nodeFittings = m_fitting.FindNodeFittings; 
			var nodeStraights = m_fitting.FindNodeStraights;
			var t = new Transform(m_fitting.transform); {
				t.Origin = destPoint;
			}
			Connector[] startEnd = { m_destPipe.StartConnector, m_destPipe.EndConnector };
			Connector cFit = null; {
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
				var v = new UnitXYZ(cFit.Origin - cFitOpposite.Origin); {
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
			foreach (var n in nodeStraights) {
				bool bDisConnect = false; {
					var v1 = m_destPipe.dir;
					var v2 = n.m_c.CoordinateSystem.BasisZ;
					var dot = v1.DotProduct(v2);
					if (!MepCommon.Equal(Math.Abs(dot), 1.0))
						bDisConnect = true;
				}
				if (bDisConnect)
					MepCommon.Disconnect(n.m_c);
			}
			var orgs = new Dictionary<NodeFittingA, XYZ>(); {
				foreach (var n in nodeFittings) 
					orgs.Add(n, n.transform.Origin);
			}
			foreach (var n in nodeFittings) {
				if (!n.element.IsValidObject)
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
			if (cFit != null) 
				MepCommon.m_doc.Delete(m_destPipe.element.Id); // 伸ばして削除する。
		}
		protected void AlignPartByConnectors(Connector partConn, Connector toConn)
		{
			FabricationPart.AlignPartByConnectors(MepCommon.m_doc, partConn, toConn, 0);
			try {
				FabricationPart.ConnectAndCouple(MepCommon.m_doc, partConn, toConn);
			} catch {

			}
		}
		protected List<KeyValuePair<Connector, Connector>>  DisConnectAll(FabricationPart fabPipe)
		{
			// 直管のすべての接続を外しておかないとFabricationPart.AlignPartByConnectorsでエラーとなる
			// FabricationPart.AlignPartByConnectorsを先に呼ばないと
			// FabricationPart.ConnectAndCoupleがエラーとなる。
			var con_orgs = new List<KeyValuePair<Connector, Connector>>();
			foreach (Connector c in MepCommon.GetEndConnectors(fabPipe.ConnectorManager.Connectors)) {
				foreach (Connector r in c.AllRefs) {
					c.DisconnectFrom(r);
					con_orgs.Add(MepCommon.make_pair(c, r));
				}
			}
			return con_orgs;
		}
		protected NodeA FindPipeNode(List<NodeA> nextNodes)
		{
			foreach (var nextNode in nextNodes) {
				var fab2 = nextNode.element as FabricationPart;
				if (!fab2.IsValidObject)
					continue;
				if (fab2.IsAStraight())
					return nextNode;
			}
			return null;
		}
		protected NodeA FindFittingNode(List<NodeA> nextNodes) 
		{
			foreach (var nextNode in nextNodes) {
				var fab2 = nextNode.element as FabricationPart;
				if (!fab2.IsValidObject)
					continue;
				if (!fab2.IsAStraight())
					return nextNode;
			}
			return null;
		}
		protected void MoveF()
		{
			var fab = m_fitting.element as FabricationPart;
			var loc = fab.Location;
			var pt1 = fab.GetTransform().Origin.Negate();
			var v1 = m_fitting.dir;
			var v2 = m_destPipe.dir;
			var pt2 = m_destPipe.projectedPickPoint;
			Move(loc, pt1, pt2, v1, v2);
		}
		protected bool _Equal(double a, double b)
		{
			return MepCommon.Equal(a, b);
		}
		protected void MoveWithInConnectedPipe_2_B() 
		{
			var nextNodes = new List<NodeA>();	var mapNextToChildNodes = new Dictionary<NodeA, NodeA>();
			{
				foreach (var childNode2 in m_fitting.m_rootNode.m_children) {
					var nextNode = FindPipeOrFitting(childNode2);
					if (nextNode != null) {
						if (!MepCommon.IsBiParallel(m_fitting.dir, nextNode.m_c.CoordinateSystem.BasisZ))
							continue;
						nextNodes.Add(nextNode);
						mapNextToChildNodes.Add(nextNode, childNode2);
					}
				}
			}
			foreach (var nextNode in nextNodes)
				MepCommon.Disconnect(nextNode.m_c);
			foreach (Connector c in m_fitting.endConnectors)
				MepCommon.Disconnect(c);
			NodeA pipeNode = FindPipeNode(nextNodes);
			ASSERT(pipeNode != null);
			// 附属品を移動
			{
				MoveF();
			}
			var con_orgs = DisConnectAll(pipeNode.element as FabricationPart);
			AlignPartByConnectors(pipeNode.m_c, mapNextToChildNodes[pipeNode].m_prev);
			foreach (var x in con_orgs) {
				x.Key.Origin = x.Value.Origin;
				x.Key.ConnectTo(x.Value);
			}
			// パイプを作って移動する付属品に引っ付ける。
			var copyPipe = CopyFab(m_destPipe.element as FabricationPart); {
				foreach (Connector c in MepCommon.GetEndConnectors(copyPipe.ConnectorManager.Connectors)) {
					foreach (Connector r in c.AllRefs) {
						c.DisconnectFrom(r);
						con_orgs.Add(MepCommon.make_pair(c, r));
					}
				}
			}
			// (移動する付属品と引っ付いていた付属品と新しく作ったパイプの節点を引っ付ける)
			Connector cCopyPipe1 = null; {
				NodeA fittingNode = FindFittingNode(nextNodes); 
				if (fittingNode != null) {
					foreach (Connector c in MepCommon.GetEndConnectors(copyPipe.ConnectorManager.Connectors)) {
						var vv1 = fittingNode.m_c.CoordinateSystem.BasisZ;
						var dot = c.CoordinateSystem.BasisZ.DotProduct(vv1);
						if (MepCommon.Equal(dot,  -1.0, 10e-3)) {
							cCopyPipe1 = c;
							break;
						}
					}
					ASSERT(cCopyPipe1 != null);
					AlignPartByConnectors(cCopyPipe1, fittingNode.m_c);
				}
			}
			// (移動する付属品の節点と新しく作ったパイプの節点を引っ付ける)
			Connector cCopyPipe2 = null; {
				var list2 = MepCommon.GetEndConnectors(copyPipe.ConnectorManager.Connectors);
				if (cCopyPipe1 != null) {
					foreach (Connector c in list2) {
						if (c.Id != cCopyPipe1.Id) {
							cCopyPipe2 = c;
							break;
						}
					}
				} else {
					cCopyPipe2 = list2[0];
				}
			}
			ASSERT(cCopyPipe2 != null);
			Connector cFitting_For_cCopyPipe2 = null; {
				foreach (Connector c in m_fitting.endConnectors) {
					var dot = c.CoordinateSystem.BasisZ.DotProduct(cCopyPipe2.CoordinateSystem.BasisZ);
					if (MepCommon.Equal(dot, -1.0, 10e-3)) {
						cFitting_For_cCopyPipe2 = c;
						break;
					}
				}
			}
			ASSERT(cFitting_For_cCopyPipe2 != null);
			var con_orgs2 = DisConnectAll(copyPipe);
			AlignPartByConnectors(cCopyPipe2, cFitting_For_cCopyPipe2);
			foreach (var x in con_orgs2) {
				x.Key.Origin = x.Value.Origin;
				x.Key.ConnectTo(x.Value);
			}
		}
		protected override void MoveWithInConnectedPipe()
		{
			CollisionException collide = null;	{
#if FIX_SKIPPER // (ko-mimura 2020/01/31)
				var a1 = GetFittingSolidInLocal(); {
#else
				var a1 = MepCommon.CreateBox(m_fitting.boundingBoxLocal); {
#endif
					var destPoint = m_destPipe.projectedPickPoint;
					var t = new Transform(m_fitting.transform); {
						///t.Origin = destPoint;
						XYZ pt = null; {
							///ここらへんを修正
							var box = MepCommon.GetBoundingBoxInWcs(m_fitting.element);
							var cen = box.Min+(box.Max-box.Min)*0.5;
							var perp = MepCommon.GetPerp(m_fitting.transform.Origin, new UnitXYZ(m_fitting.transform.BasisX) ,cen);
							var v =  m_fitting.transform.Origin - perp;
							MepCommon.CreateModelCurve(Line.CreateBound(XYZ.Zero, v));
							pt = destPoint + v;
						}
						t.Origin = pt;
					}
					a1 = SolidUtils.CreateTransformed(a1, t);
					//MepCommon.DrawSolidAsWires(a1);
					//return;
				}
				try {
					CheckCollisionDetection(a1);
				} catch (CollisionException e) {
					collide = e;
				}
			}
			if (collide != null) {
				DisconnectPipes();
				StickToOtherFitting(collide);
				MergePipes();
			} else  {
				int connectFittingCnt = 0; {
					var nextNodes = new List<NodeA>();
					foreach (var childNode in m_fitting.m_rootNode.m_children) {
						var nextNode = FindPipeOrFitting(childNode);
						if (nextNode != null) {
							if (!MepCommon.IsBiParallel(m_fitting.dir, nextNode.m_c.CoordinateSystem.BasisZ))
								continue;
							nextNodes.Add(nextNode);
						}
					}
					foreach (var nextNode in nextNodes) {
						var fab = nextNode.element as FabricationPart;
						if (!fab.IsAStraight()) {
							connectFittingCnt++;
							break;
						}
					}
				}
				if (connectFittingCnt == 0) {
					// 今までどおり
					MoveWithInConnectedPipe_2_A();
				} else if (connectFittingCnt == 1) {
					// 附属品に引っ付いている。
					MoveWithInConnectedPipe_2_B();
				} else if (connectFittingCnt == 2) {
					// 無視
				}
			}
		}
	
		protected override void SplitPipe()
		{
			ConnectorDomainType cdType; {
				var fabFitting = m_fitting.element as FabricationPart;
				cdType = fabFitting.DomainType;
			}
			if (cdType == ConnectorDomainType.Piping) {
				SplitPipeForPipe();
			} else {
				SplitPipeForDuct();
			}
		}
		protected override void CheckPathCheeze()
		{
			// コネクタが3つ以上は分岐コネクタとみなす
			foreach (var node in m_path) {
				if (object.ReferenceEquals(node, m_path[0]))
					continue;
				if (node is NodeFabricFitting) {
					var nodeFabricationParFitting = node as NodeFabricFitting;
					int endCnt = 0; {
						foreach (Connector con in nodeFabricationParFitting.targetConnectors)
							if (con.ConnectorType == ConnectorType.End)
								endCnt++;
					}
					if (endCnt > 2)
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
				if (node is NodeFabricFitting) {
					var nodeFabricationParFitting = node as NodeFabricFitting;
					if (nodeFabricationParFitting.m_fab.IsATap())
						throw new RetrySelectiongPipe("Cannot move across a tap.");
				}
			}
		}
		protected override void CheckPathReducer() {
			var fabFitting = this.m_fitting.element as FabricationPart;
			var domainType = fabFitting.DomainType;
			if (domainType == ConnectorDomainType.Piping) {
				CheckPathReducerSub();
				return;
			}
			if (m_bCHECK_REDUCER_DUCT) {
				if (domainType == ConnectorDomainType.Hvac) {
					CheckPathReducerSub();
					return;
				}
			}
		}
		protected void CheckPathReducerSub()
		{
			Func<ConnectorProfileType, bool> ShapeType = (t) =>{
				foreach (var node in m_path) {
					if (node is NodeFabricStraight) {
						var nodeStraight = node as NodeFabricStraight;
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
					if (node is NodeFabricStraight) {
						var nodeStraight = node as NodeFabricStraight;
						var con = nodeStraight.m_c;
						if (con.Shape != ConnectorProfileType.Round)
							throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
						if (!(w == double.MaxValue)) {
							if (!(MepCommon.Equal(w, con.Radius)))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
						}
						w = con.Radius;
					}
				}
				return;
			}
			if (ShapeType(ConnectorProfileType.Rectangular)) {
				double w = double.MaxValue;
				double h = double.MaxValue;
				foreach (var node in m_path) {
					if (node is NodeFabricStraight) {
						var nodeStraight = node as NodeFabricStraight;
						var con = nodeStraight.m_c;
						if (con.Shape != ConnectorProfileType.Rectangular)
							throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
						if (!(w == double.MaxValue && h == double.MaxValue)) {
							if (!(MepCommon.Equal(w, con.Width)  && MepCommon.Equal(h, con.Height)))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
						}
						w = con.Width;
						h = con.Height;
					}
				}
				return;
			}
			if (ShapeType(ConnectorProfileType.Oval)) {
				double w = double.MaxValue;
				double h = double.MaxValue;
				foreach (var node in m_path) {
					if (node is NodeFabricStraight) {
						var nodeStraight = node as NodeFabricStraight;
						var con = nodeStraight.m_c;
						if (con.Shape != ConnectorProfileType.Oval)
							throw new RetrySelectiongPipe("Cannot move between straight ducts of different shapes.");
						if (!(w == double.MaxValue && h == double.MaxValue)) {
							if (!(MepCommon.Equal(w, con.Width) && MepCommon.Equal(h, con.Height)))
								throw new RetrySelectiongPipe("Cannot move between straight ducts of different sizes.");
						}
						w = con.Width;
						h = con.Height;
					}
				}
				return;
			}
		}
		protected override void FitConnect(Connector c1, Connector c2)
		{
			FabricationPart.ConnectAndCouple(MepCommon.m_doc, c1, c2);
		}
	}
}
