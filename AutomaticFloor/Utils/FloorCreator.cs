using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Utils
{
    public class FloorCreator
    {
        public Result CreateFloor(ExternalCommandData cmdData, eFloorType efloorType)
        {
            CultureHelper.InitializeCulture();

            UIApplication rvtUIApp = cmdData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;

            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);

            Result retExtCom = Result.Cancelled;
            TransactionGroup transGroup = new TransactionGroup(cmpElements.RvtDBDoc);
            transGroup.Start("CreateFloor");
            Transaction trans = new Transaction(cmpElements.RvtDBDoc);

            try
            {
                if (rvtUIDoc.ActiveView.ViewType != ViewType.FloorPlan &&
                    rvtUIDoc.ActiveView.ViewType != ViewType.CeilingPlan &&
                    rvtUIDoc.ActiveView.ViewType != ViewType.AreaPlan &&
                    rvtUIDoc.ActiveView.ViewType != ViewType.EngineeringPlan)
                {
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_INFO_ACTIVE_VIEW"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                List<Element> elementList = cmpService.GetElements(rvtUIDoc.ActiveView, efloorType);
                if (elementList.Count == 0)
                {
                    if (efloorType == eFloorType.Arch)
                        MessageBox.Show(cmpAttribute.ResourceText("IDS_INFO_NO_EXIST_WALLS"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    else
                        MessageBox.Show(cmpAttribute.ResourceText("IDS_INFO_NO_EXIST_BEAMS"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                IList<Element> elemFloorTypes = cmpElements.GetFloorTypes(efloorType);
                if (elemFloorTypes.Count == 0)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Start("SetCommand");

                RvtExtApp.Entities.DtSlabType entDtSlabType = new RvtExtApp.Entities.DtSlabType(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                if (entDtSlabType.ErrMsg != "")
                {
                    MessageBox.Show(entDtSlabType.ErrMsg, cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }
                entDtSlabType.GetData(elemFloorTypes);

                ProjectInfo elemProjInfo = cmpElements.ProjectInfo;
                RvtExtApp.Entities.DtCmd entDtCmd = new RvtExtApp.Entities.DtCmd(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings,
                    elemProjInfo, cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_LOCATESLAB"), 4);
                if (entDtCmd.ErrMsg != "")
                {
                    MessageBox.Show(entDtCmd.ErrMsg, cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                RvtExtApp.Config.FormConfig form = new RvtExtApp.Config.FormConfig(cmpAttribute, entDtSlabType, entDtCmd, efloorType);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                Level level = null;
                FloorType floorType = null;
                IList<IList<Curve>> floorsCurves = null;
                Dictionary<int, List<int>> dic_indexs = null;
                List<ElementId> floorIds = new List<ElementId>();
                bool pickPoint_end = false;

                while (!pickPoint_end)
                {
                    if (rvtUIDoc.Document.ActiveView.SketchPlane == null)
                    {
                        trans.Start("Temporarily set work plane");
                        Plane plane = Plane.CreateByNormalAndOrigin(rvtUIDoc.Document.ActiveView.ViewDirection, rvtUIDoc.Document.ActiveView.Origin);
                        SketchPlane sp = SketchPlane.Create(rvtUIDoc.Document, plane);
                        rvtUIDoc.Document.ActiveView.SketchPlane = sp;
                        rvtUIDoc.Document.ActiveView.ShowActiveWorkPlane();
                    }

                    Selection sel = cmdData.Application.ActiveUIDocument.Selection;
                    XYZ pickPoint;
                    try
                    {
                        pickPoint = sel.PickPoint(ObjectSnapTypes.None, cmpAttribute.ResourceText("IDS_INFO_PICK_POINT"));
                    }
                    catch
                    {
                        pickPoint_end = true;
                        break;
                    }
                    finally
                    {
                        if (trans.HasStarted()) trans.RollBack();
                    }

                    if (floorsCurves == null || level == null || floorType == null)
                    {
                        if (!cmpService.GetData(entDtSlabType, elementList, efloorType, out level, out floorType, out floorsCurves, out dic_indexs))
                            continue;
                    }

                    trans.Start("CreateFloor");
                    var floor = cmpService.CreateFloor(floorsCurves, dic_indexs, level, efloorType, floorType, pickPoint);
                    if (floor == null)
                    {
                        MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_NO_CREATE_FLOOR"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                        trans.RollBack();
                        continue;
                    }
                    trans.Commit();

                    bool isLock = entDtCmd.Data[2] == "true";
                    if (isLock)
                    {
                        trans.Start("Lock");
                        LockFloor(cmdData.Application.ActiveUIDocument.Document, floor, elementList);
                        trans.Commit();
                    }

                    trans.Start("FloorParam");
                    if (efloorType == eFloorType.Arch)
                        cmpParameters.SetValue(floor, BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL, 0);
                    else
                        cmpParameters.SetValue(floor, BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL, 1);
                    trans.Commit();

                    trans.Start("SetHeight");
                    double heightOffset = 0.0;
                    if (double.TryParse(entDtCmd.Data[0], out double parsed))
                        heightOffset = parsed;
                    heightOffset /= cmpGeometry.UnitCoe;
                    if (floor != null)
                        cmpParameters.SetValue(floor, BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM, heightOffset);
                    trans.Commit();

                    trans.Start("SetValueSlabDirectionAngle");
                    double directionAngle = 0.0;
                    if (entDtCmd.DegreeAngle != 0) directionAngle = entDtCmd.DegreeAngle;
                    directionAngle *= (Math.PI / 180);
                    if (floor != null) floor.SpanDirectionAngle = directionAngle;
                    trans.Commit();

                    trans.Start("SetParamValue");
                    entDtCmd.SetData();
                    trans.Commit();

                    floorIds.Add(floor.Id);
                }

                if (floorIds.Count == 0)
                    retExtCom = Result.Cancelled;
                else
                {
                    cmdData.Application.ActiveUIDocument.Selection.SetElementIds(floorIds);
                    retExtCom = Result.Succeeded;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                if (trans.GetStatus() != TransactionStatus.Committed)
                    trans.RollBack();
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }

        private List<ModelCurve> GetModelCurveOfFloor(Document doc, Floor floor)
        {
            List<ModelCurve> modelCurveList = new List<ModelCurve>();
            ICollection<ElementId> ids = null;
            using (SubTransaction subTran = new SubTransaction(doc))
            {
                if (subTran.Start() == TransactionStatus.Started)
                {
                    ids = doc.Delete(floor.Id);
                    subTran.RollBack();
                }
            }
            if (ids == null) return modelCurveList;
            foreach (ElementId mLineId in ids)
            {
                var modelCurve = doc.GetElement(mLineId) as ModelCurve;
                if (modelCurve != null) modelCurveList.Add(modelCurve);
            }
            return modelCurveList;
        }

        private Dictionary<ModelCurve, List<Element>> DetectMatchedElements(Document doc, Floor floor, List<Element> elements)
        {
            var modelCurves = GetModelCurveOfFloor(doc, floor);
            if (modelCurves == null || modelCurves.Count == 0) return null;

            Dictionary<ModelCurve, List<Element>> lines = new Dictionary<ModelCurve, List<Element>>();
            foreach (ModelCurve modelCurve in modelCurves)
            {
                foreach (Element familyInstance in elements)
                {
                    var locationCurve = (familyInstance.Location as LocationCurve)?.Curve;
                    if (locationCurve == null) continue;

                    bool flag = IsSameCurve(modelCurve.GeometryCurve, locationCurve) || IsSameCurve2D(modelCurve.GeometryCurve, locationCurve);
                    if (flag)
                    {
                        if (!lines.ContainsKey(modelCurve))
                            lines.Add(modelCurve, new List<Element>());
                        lines[modelCurve].Add(familyInstance);
                    }
                }
            }
            return lines;
        }

        private bool IsSameCurve2D(Curve curve1, Curve curve2)
        {
            var p0 = curve1.GetEndPoint(0);
            var p1 = curve1.GetEndPoint(1);
            var curveTemp1 = Line.CreateBound(new XYZ(p0.X, p0.Y, 0), new XYZ(p1.X, p1.Y, 0));
            var p2 = curve2.GetEndPoint(0);
            var p3 = curve2.GetEndPoint(1);
            var curveTemp2 = Line.CreateBound(new XYZ(p2.X, p2.Y, 0), new XYZ(p3.X, p3.Y, 0));

            var result1 = curveTemp2.Project(p0);
            var result2 = curveTemp2.Project(p1);
            if (result1 != null && result2 != null && result1.Distance < 1e-6 && result2.Distance < 1e-6)
                return true;

            result1 = curveTemp1.Project(p2);
            result2 = curveTemp1.Project(p3);
            if (result1 != null && result2 != null && result1.Distance < 1e-6 && result2.Distance < 1e-6)
                return true;

            return false;
        }

        private bool IsSameCurve(Curve curve1, Curve curve2)
        {
            return (IsNearlyEqual(curve1.GetEndPoint(0), curve2.GetEndPoint(0)) && IsNearlyEqual(curve1.GetEndPoint(1), curve2.GetEndPoint(1))) ||
                   (IsNearlyEqual(curve1.GetEndPoint(0), curve2.GetEndPoint(1)) && IsNearlyEqual(curve1.GetEndPoint(1), curve2.GetEndPoint(0)));
        }

        private bool IsNearlyEqual(XYZ pos1, XYZ pos2)
        {
            return IsNearlyEqual(pos1.X, pos2.X) && IsNearlyEqual(pos1.Y, pos2.Y) && IsNearlyEqual(pos1.Z, pos2.Z);
        }

        private bool IsNearlyEqual(double val1, double val2, int digit = 3)
        {
            int ival1 = (int)(val1 * Math.Pow(10, digit));
            int ival2 = (int)(val2 * Math.Pow(10, digit));
            return ival1 == ival2;
        }

        private void LockFloor(Document doc, Floor floor, List<Element> elements)
        {
            var matchElements = DetectMatchedElements(doc, floor, elements);
            if (matchElements == null || matchElements.Count == 0) return;

            foreach (KeyValuePair<ModelCurve, List<Element>> keyPair in matchElements)
            {
                foreach (Element element in keyPair.Value)
                {
                    try
                    {
                        if (element is FamilyInstance fi)
                        {
                            IList<Reference> refList = fi.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                            if (refList.Count != 0)
                            {
                                try { doc.Create.NewAlignment(doc.ActiveView, refList[0], new Reference(keyPair.Key)); }
                                catch { }
                            }
                        }
                        else
                        {
                            try { doc.Create.NewAlignment(doc.ActiveView, new Reference(element), new Reference(keyPair.Key)); }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
