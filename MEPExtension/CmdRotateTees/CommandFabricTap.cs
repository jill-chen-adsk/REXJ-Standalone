/* ko-mimura(2019/06/27)
 *  チーズの回転コマンド(製造パーツ用)(フジツボ型)
 *  使用方法
 *  １．チーズを選択する。
 *  ２．ダイアログが表示されるので、回転角度をインクリメントする。
 *  ３．OKボタン押下で回転確定される。
 */
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using MEPCommon;
namespace CmdRotateTees
{
	public class RotateTeesFabricTap : RotateTeesFabric , MainWindowIF
	{
		FabricationPart m_AttachedFab = null;
		protected override void Disconnect() {
			return;
		}
		private void Rotate1(double angle)
		{
			var axis = GetAxisZ();
			var ptPorjToLine = GetAxisPt();
			var ptConnector = base.GetAxisPt();
			var locPoint = m_fabricInstance.Location;
			locPoint.Move(ptPorjToLine.Negate());
			locPoint.Rotate(Line.CreateBound(XYZ.Zero, axis), angle);
			locPoint.Move(ptPorjToLine);
		}
		private void Rotate2(double angle) 
		{
			FabricationPart.RotateConnectedTap(MepCommon.m_doc, m_fabricInstance, angle, 0);
		}
		override protected void UserControl1_IF_RotateSub(double angle)
		{
			try {
				Rotate2(angle);
				//Rotate1(angle);
			} catch (System.Exception e) {
				var str = e.Message;
			}
		}
		public override void ExecuteSub() 
		{
			DispDialog( this);
		}
		protected void FindAttachedFab() 
		{
			for (int i =0; i < m_connectors.Count; i++) {
				Connector con = m_connectors[i];
				foreach (Connector conRef in con.AllRefs) {
					if (conRef.ConnectorType != ConnectorType.Curve)
						continue;
					if (conRef.Owner is FabricationPart) {
						var fab = conRef.Owner as FabricationPart;
						m_connectorIndex = i - 1;
						m_AttachedFab = fab;
						return;
					}
				}
			}
			m_connectorIndex = -1;
			m_AttachedFab = null;
			return;
		}
		override protected XYZ GetAxisZ() 
		{
			if (m_AttachedFab == null)
				return base.GetAxisZ();
			return m_Axis.BasisX;
		}
		override protected XYZ GetAxisX() 
		{
			if (m_AttachedFab == null)
				return base.GetAxisX();
			return m_Axis.BasisY;
		}
		override protected XYZ GetAxisY() 
		{
			if (m_AttachedFab == null)
				return base.GetAxisY();
			return m_Axis.BasisZ;
		}
		override protected XYZ GetAxisPt() 
		{
			if (m_AttachedFab == null)
				return base.GetAxisPt();
			var curve = GetCurve(m_AttachedFab);
			var pt = base.GetAxisPt();
			var ptProj = curve.Project(pt).XYZPoint;
			return ptProj;
		}
		public override bool IsTargetElement(Element element) 
		{
			if (base.IsTargetElement(element) == false)
				return false;
			if (m_fabricInstance.IsATap() == false)
				return false;
			FindAttachedFab();
			return true;
		}
		public RotateTeesFabricTap(TransactionGroup transGroup)
			:base(transGroup)
		{
		}
	}
}