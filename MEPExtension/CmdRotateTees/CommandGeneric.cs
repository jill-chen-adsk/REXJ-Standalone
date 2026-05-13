/* ko-mimura
 *  チーズの回転コマンド(ジェネリックパーツ)
 *  使用方法
 *  １．チーズを選択する。
 *  ２．ダイアログが表示されるので、回転角度をインクリメントする。
 *  ３．OKボタン押下で回転確定される。
 */
//#define _TEST_
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using MEPCommon;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
namespace CmdRotateTees
{
	public class RotateTeesGeneric : RotateTeesA, MainWindowIF
	{
		private readonly List<string> m_targetCategories = new List<string>(){ "配管継手" , "配管付属品", "ダクト継手", "ダクト付属品" };
		private readonly List<BuiltInCategory> _mTargetBuiltInCategories = new List<BuiltInCategory> {
			BuiltInCategory.OST_PipeFitting, BuiltInCategory.OST_PipeAccessory, BuiltInCategory.OST_DuctFitting, BuiltInCategory.OST_DuctAccessory
		};
	private FamilyInstance      m_familyInstance;
	private ElementId m_backupInsulationTypeId = ElementId.InvalidElementId;
	private double m_backupInsulationThickness = 0.0;
	private List<ElementId> m_insulationElementIds = new List<ElementId>();
	private bool m_isInsulationBackedUp = false;
		public override BoundingBoxXYZ getBoundingBoxInWCS() 
		{
			return MEPCommon.MepCommon.GetBoundingBoxInWcs(m_familyInstance);
		}
		private void Rotate1(double angle)
		{
			var pt = GetAxisPt();
			m_familyInstance.Location.Move(pt.Negate());
			var dir = GetAxisZ();
			m_familyInstance.Location.Rotate(Line.CreateBound(XYZ.Zero, dir), angle);
			m_familyInstance.Location.Move(pt);
		}
		private void SwapWidthAndHeightOfTheCurrentConnector()
		{
			{
				const double tol2= 1e-5;
				var cons = new List<Connector>();
				{
					var con1 = GetCurrentConnector();
					Connector con2 = null;
					{
						foreach (Connector c in m_connectors) {
							if (con1.Id.Equals(c.Id))
								continue;
							if (!(c.Shape == ConnectorProfileType.Rectangular || c.Shape == ConnectorProfileType.Oval))
								continue;
							if (MepCommon.Equal(con1.Width, c.Width) && MepCommon.Equal(con1.Height, c.Height))
								continue;
							var dot = con1.CoordinateSystem.BasisZ.DotProduct(c.CoordinateSystem.BasisZ);
							if (System.Math.Abs(dot) < (1.0 + tol2)) {
								var line1 = Line.CreateUnbound(con1.CoordinateSystem.Origin, con1.CoordinateSystem.BasisZ);
								var dist = line1.Distance(c.CoordinateSystem.Origin);
								if (dist < tol2) {
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
		private void Rotate2(double angle) 
		{
			var pt = GetAxisPt();
			m_familyInstance.Location.Move(pt.Negate());
			var dir = GetAxisZ();
			m_familyInstance.Location.Rotate(Line.CreateBound(XYZ.Zero, dir), angle);
			m_familyInstance.Location.Move(pt);
			try {
				SwapWidthAndHeightOfTheCurrentConnector();
			} catch (Exception e) {
				var str = e.Message;
			}
		}
	public void MainWindowIF_BackupInsulationParameters()
	{
		// Only backup once, before Disconnect() is called
		if (m_isInsulationBackedUp)
			return;
			
		try {
			if (m_familyInstance != null) {
				// Find all insulation elements attached to this family instance
				FindAndBackupInsulation();
				m_isInsulationBackedUp = true;
			}
		} catch (Exception e) {
			// Ignore errors if parameters don't exist
			var str = e.Message;
		}
	}

	private void FindAndBackupInsulation()
	{
		try {
			// Get insulation elements attached to this family instance
			FilteredElementCollector collector = new FilteredElementCollector(MepCommon.m_doc);
			var insulationElements = collector.OfClass(typeof(InsulationLiningBase))
				.Cast<InsulationLiningBase>()
				.Where(ins => ins.HostElementId.Value == m_familyInstance.Id.Value)
				.ToList();

			foreach (var insulation in insulationElements) {
				// Backup insulation type and thickness
				m_backupInsulationTypeId = insulation.GetTypeId();
				m_backupInsulationThickness = insulation.Thickness;
				m_insulationElementIds.Add(insulation.Id);
			}

			// If no insulation elements found, try to get from parameters
			if (m_insulationElementIds.Count == 0) {
				var paramType = m_familyInstance.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE);
				if (paramType != null && paramType.HasValue) {
					m_backupInsulationTypeId = paramType.AsElementId();
				}
				
				var paramThickness = m_familyInstance.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS);
				if (paramThickness != null && paramThickness.HasValue) {
					m_backupInsulationThickness = paramThickness.AsDouble();
				}
			}
		} catch (Exception e) {
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
		} catch (Exception e) {
			var str = e.Message;
		}
	}

	public void MainWindowIF_RestoreInsulationParameters()
	{
		try {
			if (m_familyInstance != null && m_backupInsulationTypeId != ElementId.InvalidElementId && m_backupInsulationThickness > 0.0) {
				// Recreate insulation with backed up information
				RecreateInsulation();
			}
		} catch (Exception e) {
			// Ignore errors if insulation can't be recreated
			var str = e.Message;
		}
	}

	private void RecreateInsulation()
	{
		try {
			// Try to create insulation using Revit API
			var category = m_familyInstance.Category;
			
			long categoryIdValue = category.Id.Value;
			
			if (categoryIdValue == (int)BuiltInCategory.OST_PipeFitting ||
			    categoryIdValue == (int)BuiltInCategory.OST_PipeAccessory) {
				// For pipe fittings, try PipeInsulation.Create
				try {
					PipeInsulation.Create(MepCommon.m_doc, m_familyInstance.Id, m_backupInsulationTypeId, m_backupInsulationThickness);
				} catch {
					// May not work for all types, ignore error
				}
			}
			else if (categoryIdValue == (int)BuiltInCategory.OST_DuctFitting ||
			         categoryIdValue == (int)BuiltInCategory.OST_DuctAccessory) {
				// For duct fittings, try DuctInsulation.Create
				try {
					DuctInsulation.Create(MepCommon.m_doc, m_familyInstance.Id, m_backupInsulationTypeId, m_backupInsulationThickness);
				} catch {
					// May not work for all types, ignore error
				}
			}
		} catch (Exception e) {
			var str = e.Message;
		}
	}

		public void MainWindowIF_Rotate(double angle)
		{
			// Rotate1(angle);
			Rotate2(angle);
		}
		private List<Connector> GetConnectors(FamilyInstance inst)
		{
			List<Connector> list = new List<Connector>();
			var connector_cons = inst.MEPModel.ConnectorManager.Connectors;
			foreach (Connector con in connector_cons) {
				list.Add(con);
			}
			return list;
		}
		private bool IsElbow(FamilyInstance inst)
		{
			var cons = GetConnectors(inst);
			if (cons.Count != 2)
				return false;
			var dir1 = cons[0].CoordinateSystem.BasisZ;
			var dir2 = cons[1].CoordinateSystem.BasisZ;
			if (dir1.CrossProduct(dir2).GetLength() < MepCommon.tol)
				return false;
			return true;
		}
		private bool CheckPickedObject()
		{
			bool bFind = false; {
				foreach (var cat2 in _mTargetBuiltInCategories) {
					if (m_familyInstance.Category.BuiltInCategory == cat2) {
						bFind = true;
						break;
					}
				}
				

			}
			if (!bFind) {
				var msg = "Invalid category. Please select a pipe accessory from the following categories: [";
				foreach (var name2 in m_targetCategories) {
					msg += name2;
					msg += ",";
				}
				msg += "]";
				messageBox(msg);
				return false;
			}
			return true;
		}
		protected override ConnectorManager GetConnectorManager()
		{
			return this.m_familyInstance.MEPModel.ConnectorManager;
		}
		public override bool IsTargetElement(Element element)
		{
			if (!(element is FamilyInstance))
				return false;
			m_familyInstance = element as FamilyInstance;
			if (m_familyInstance == null)
				return false;
#if _TEST_
			System.Windows.MessageBox.Show(m_familyInstance.Category.Name);
#endif
			m_connectedPipes = GetConnectedPipes(GetConnectorManager().Connectors);
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
		public RotateTeesGeneric(TransactionGroup transGroup)
		{
			m_transGroup = transGroup;
			m_pipeType = PIPE_TYPE.PIPE;
			m_connectedPipes = null;
			m_familyInstance = null;
		}
	}
}
