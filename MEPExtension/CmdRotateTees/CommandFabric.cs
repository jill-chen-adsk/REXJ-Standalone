/* ko-mimura(2019/06/27)
 *  チーズの回転コマンド(製造パーツ用)
 *  使用方法
 *  １．チーズを選択する。
 *  ２．ダイアログが表示されるので、回転角度をインクリメントする。
 *  ３．OKボタン押下で回転確定される。
 */
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using MEPCommon;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
namespace CmdRotateTees
{
	public class RotateTeesFabric : RotateTeesA, MainWindowIF
	{
		protected FabricationPart m_fabricInstance;
		//protected readonly List<string> m_targetCategories = new List<string>() { "配管継手", "配管付属品", "ダクト継手", "ダクト付属品" };
		private ElementId m_backupInsulationTypeId = ElementId.InvalidElementId;
		private double m_backupInsulationThickness = 0.0;
		private List<ElementId> m_insulationElementIds = new List<ElementId>();
		private bool m_isInsulationBackedUp = false;

		public override BoundingBoxXYZ getBoundingBoxInWCS()
		{
			return MEPCommon.MepCommon.GetBoundingBoxInWcs(m_fabricInstance);
		}
		public void MainWindowIF_BackupInsulationParameters()
		{
			// Only backup once, before Disconnect() is called
			if (m_isInsulationBackedUp)
				return;
			
			try {
				if (m_fabricInstance != null) {
					// Find all insulation elements attached to this fabrication part
					FindAndBackupInsulation();
					m_isInsulationBackedUp = true;
				}
			} catch (System.Exception e) {
				// Ignore errors if parameters don't exist
				var str = e.Message;
			}
		}

		private void FindAndBackupInsulation()
		{
			try {
				// Get insulation elements attached to this fabrication part
				FilteredElementCollector collector = new FilteredElementCollector(MepCommon.m_doc);
				var insulationElements = collector.OfClass(typeof(InsulationLiningBase))
					.Cast<InsulationLiningBase>()
					.Where(ins => ins.HostElementId.Value == m_fabricInstance.Id.Value)
					.ToList();

				foreach (var insulation in insulationElements) {
					// Backup insulation type and thickness
					m_backupInsulationTypeId = insulation.GetTypeId();
					m_backupInsulationThickness = insulation.Thickness;
					m_insulationElementIds.Add(insulation.Id);
				}

				// If no insulation elements found, try to get from parameters
				if (m_insulationElementIds.Count == 0) {
					var paramType = m_fabricInstance.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE);
					if (paramType != null && paramType.HasValue) {
						m_backupInsulationTypeId = paramType.AsElementId();
					}
				
					var paramThickness = m_fabricInstance.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS);
					if (paramThickness != null && paramThickness.HasValue) {
						m_backupInsulationThickness = paramThickness.AsDouble();
					}
				}
			} catch (System.Exception e) {
				var str = e.Message;
			}
		}

		private void DeleteInsulation()
		{
			try {
				// Delete all insulation elements
				if (m_insulationElementIds.Count > 0) {
					MepCommon.m_doc.Delete(m_insulationElementIds);
				}
			} catch (System.Exception e) {
				var str = e.Message;
			}
		}

		public void MainWindowIF_RestoreInsulationParameters()
		{
			try {
				if (m_fabricInstance != null && m_backupInsulationTypeId != ElementId.InvalidElementId && m_backupInsulationThickness > 0.0) {
					// Recreate insulation with backed up information
					RecreateInsulation();
				}
			} catch (System.Exception e) {
				// Ignore errors if insulation can't be recreated
				var str = e.Message;
			}
		}

		private void RecreateInsulation()
		{
			try {
				// Try to create insulation using Revit API
				// For FabricationPart, we may need to use different approach
				var category = m_fabricInstance.Category;
			
				long categoryIdValue = category.Id.Value;
			
				if (categoryIdValue == (int)BuiltInCategory.OST_PipeFitting ||
					categoryIdValue == (int)BuiltInCategory.OST_PipeAccessory) {
					// For pipe fittings, try PipeInsulation.Create
					try {
						PipeInsulation.Create(MepCommon.m_doc, m_fabricInstance.Id, m_backupInsulationTypeId, m_backupInsulationThickness);
					} catch {
						// May not work for all types, ignore error
					}
				}
				else if (categoryIdValue == (int)BuiltInCategory.OST_DuctFitting ||
						 categoryIdValue == (int)BuiltInCategory.OST_DuctAccessory) {
					// For duct fittings, try DuctInsulation.Create
					try {
						DuctInsulation.Create(MepCommon.m_doc, m_fabricInstance.Id, m_backupInsulationTypeId, m_backupInsulationThickness);
					} catch {
						// May not work for all types, ignore error
					}
				}
			} catch (System.Exception e) {
				var str = e.Message;
			}
		}

		public void MainWindowIF_Rotate(double angle) 
		{
			UserControl1_IF_RotateSub(angle);
		}
		private void Rotate1(double angle) 
		{
			var pt = GetAxisPt();
			m_fabricInstance.Location.Move(pt.Negate());
			var dir = GetAxisZ();
			m_fabricInstance.Location.Rotate(Line.CreateBound(XYZ.Zero, dir), angle);
			m_fabricInstance.Location.Move(pt);
		}
		private void Rotate2(double angle) 
		{
			var pt = GetAxisPt();
			m_fabricInstance.Location.Move(pt.Negate());
			var dir = GetAxisZ();
			m_fabricInstance.Location.Rotate(Line.CreateBound(XYZ.Zero, dir), angle);
			m_fabricInstance.Location.Move(pt);
			{
				const double tol = 1e-5;
				var cons = new List<Connector>(); {
					var con1 = GetCurrentConnector();
					Connector con2 = null; {
						foreach (Connector c in m_connectors) {
							if (con1.Id.Equals(c.Id))
								continue;
							if (!(c.Shape == ConnectorProfileType.Rectangular || c.Shape == ConnectorProfileType.Oval)) 
								continue;
							if (MepCommon.Equal(con1.Width, c.Width) && MepCommon.Equal(con1.Height, c.Height))
								continue;
							var dot = con1.CoordinateSystem.BasisZ.DotProduct(c.CoordinateSystem.BasisZ);
							if (System.Math.Abs(dot) < (1.0+tol)) {
								var line1 = Line.CreateUnbound(con1.CoordinateSystem.Origin, con1.CoordinateSystem.BasisZ);
								var dist = line1.Distance(c.CoordinateSystem.Origin);
								if (dist < tol) {
									con2 = c;
									break;
								}
							}
						}
					}
					cons.Add(con1);
					if (con2 != null)
						cons.Add(con2);
				}
				foreach (Connector con in cons)
					if (con.Shape == ConnectorProfileType.Rectangular || con.Shape == ConnectorProfileType.Oval) {
						var w = con.Width;
						var h = con.Height;
						con.Width = h;
						con.Height = w;
					}
			}
		}
		protected override void Disconnect() 
		{
			return;
		}
		private void Rotate3(double angle) 
		{
			try {
				var con = GetCurrentConnector();
				FabricationPart.RotateConnectedPartByConnector(MepCommon.m_doc, con, angle);
			} catch (System.Exception e) {
				var str = e.Message;
				messageBox("Rotation failed.");
			}
		}
		virtual protected void UserControl1_IF_RotateSub(double angle) 
		{
			// Rotate1(angle);
			// Rotate2(angle);
			Rotate3(angle);
		}
		private List<Connector> GetConnectors(FabricationPart inst) {
			List<Connector> list = new List<Connector>(); {
				var connector_cons = inst.ConnectorManager.Connectors;
				foreach (Connector con in connector_cons) 
					list.Add(con);
			}
			return list;
		}
		private bool IsElbow(FabricationPart inst) {
			var cons = GetConnectors(inst);
			if (cons.Count != 2)
				return false;
			var dir1 = cons[0].CoordinateSystem.BasisZ;
			var dir2 = cons[1].CoordinateSystem.BasisZ;
			if (dir1.CrossProduct(dir2).GetLength() < MepCommon.m_VertexTolerance)//tol)
				return false;
			return true;
		}
		// private bool CheckConnector1() {
		// 	var name = m_fabricInstance.Category.Name;
		// 	foreach (var name2 in m_targetCategories) {
		// 		if (name == name2)
		// 			return true;
		// 	}
		// 	var msg = "カテゴリ名が違います。次のカテゴリの配管付属品を選択してください。[";
		// 	foreach (var name2 in m_targetCategories) {
		// 		msg += name2;
		// 		msg += ",";
		// 	}
		// 	msg += "]";
		// 	messageBox(msg);
		// 	return false;
		// }
		private bool CheckPickedObject() {
			return true;
		}
		protected override ConnectorManager GetConnectorManager() {
			return m_fabricInstance.ConnectorManager;
		}
		public override bool IsTargetElement(Element element) {
			if (!(element is FabricationPart))
				return false;
			m_fabricInstance = element as FabricationPart;
			if (m_fabricInstance.IsAStraight() || m_fabricInstance.IsAHanger())// || m_fabricInstance.IsATap())
				return false;
			var cons = GetConnectorManager().Connectors;
			m_connectedPipes = GetConnectedPipes(cons);
			m_pipeType = PIPE_TYPE.PIPE;
			if (!CheckPickedObject())
				return false;
			m_connectors = GetConnectorsList();
			m_connectorIndex = -1;
			return true;
		}
		public override void ExecuteSub() 
		{
			// Backup insulation information BEFORE DispDialog() calls Disconnect()
			// Disconnect() may clear the insulation parameters
			MainWindowIF_BackupInsulationParameters();
		
			// Delete insulation before starting rotation dialog
			// It will be recreated after rotation completes
			using (var tran = new Transaction(MepCommon.m_doc, "Delete Insulation")) {
				tran.Start();
				DeleteInsulation();
				tran.Commit();
			}
		
			DispDialog(this);
		}
		public RotateTeesFabric(TransactionGroup transGroup) {
			m_transGroup = transGroup;
			m_pipeType = PIPE_TYPE.PIPE;
			m_connectedPipes = null;
			m_fabricInstance = null;
		}
	}
}