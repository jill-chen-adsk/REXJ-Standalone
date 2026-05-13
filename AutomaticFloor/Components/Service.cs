using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using ADSK.JExtRAC.AutomaticFloor.Utils;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Components
{
    public class Service
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Components.Elements _CmpElements;
        private RvtExtApp.Components.Geometry _CmpGeometry;
        private RvtExtApp.Components.Parameters _CmpParameters;
        private RvtExtApp.Components.Settings _CmpSettings;
        private RvtExtApp.Entities.DtItems _EntDtItems;
        private string _ErrMsg;

        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _EntDtItems = new RvtExtApp.Entities.DtItems(_CmpAttribute);
            _ErrMsg = "";
        }

        public List<Element> GetElements(View view, eFloorType eFloorType)
        {
            List<Element> ret = new List<Element>();

            ElementCategoryFilter catFilter;
            if (eFloorType == eFloorType.Arch)
                catFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            else
                catFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);

            FilteredElementCollector collector = new FilteredElementCollector(view.Document, view.Id)
                .WhereElementIsNotElementType()
                .WherePasses(catFilter);

            foreach (Element elem in collector.ToElements())
            {
                if (eFloorType == eFloorType.Arch)
                {
                    int iValue = 0;
                    if (_CmpParameters.GetValue(elem, BuiltInParameter.WALL_ATTR_ROOM_BOUNDING, ref iValue) == 0)
                    {
                        if (iValue == 1)
                            ret.Add(elem);
                    }
                }
                else
                {
                    int iValue = 0;
                    _CmpParameters.GetValue(elem, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, ref iValue);
                    if (iValue == 3 || iValue == 4)
                        ret.Add(elem);
                }
            }
            return ret;
        }

        public bool GetData(RvtExtApp.Entities.DtSlabType entDtSlabType, List<Element> elementList, eFloorType eFloorType,
            out Level level, out FloorType floorType,
            out IList<IList<Curve>> floorsCurves, out Dictionary<int, List<int>> dic_bounds)
        {
            _ErrMsg = "";
            floorsCurves = null;
            dic_bounds = null;
            level = null;
            floorType = null;

            Element elem = entDtSlabType.WorkElem;
            if (elem != null)
                floorType = elem as FloorType;
            if (floorType == null)
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_SLABTYPE");
                return false;
            }
            if (elementList.Count == 0)
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_BEAMS");
                return false;
            }

            level = elem.Document.ActiveView.GenLevel;
            var firstElement = elementList[0];

            if (eFloorType == eFloorType.Arch)
            {
                if (level == null && firstElement.LevelId != ElementId.InvalidElementId)
                    level = _CmpElements.RvtDBDoc.GetElement(firstElement.LevelId) as Level;
            }
            else
            {
                Parameter paramRefLevel = firstElement.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                if (paramRefLevel != null)
                {
                    Element elemHost = _CmpElements.GetElementDoc(int.Parse(paramRefLevel.AsElementId().ToString()));
                    if (elemHost != null)
                        level = elemHost as Level;
                }
            }

            if (level == null)
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_LEVELBEAMS");
                return false;
            }

            double elementHeight = level.Elevation;
            IList<Curve> elementsCurve = new List<Curve>();
            foreach (Element element in elementList)
            {
                Curve curve = _CmpGeometry.GetElementLocCurve(element);
                elementsCurve.Add(curve);
            }
            if (elementsCurve.Count == 0)
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_CURVBEAMS");
                return false;
            }

            _CmpGeometry.ToleranceInter = _EntDtItems.ToleranceInter / _CmpGeometry.UnitCoe;
            IList<IList<XYZ>> beamsInterPosAry = _CmpGeometry.GetInterPosCurves(elementsCurve);
            _CmpGeometry.GetPlanFaceCurveInterPos(beamsInterPosAry, elementHeight, out floorsCurves, out dic_bounds);
            return true;
        }

        public Floor CreateFloor(IList<IList<Curve>> floorsCurvesTmp, Dictionary<int, List<int>> dic_indexs,
            Level level, eFloorType eFloorType, FloorType floorType, XYZ pickPoint)
        {
            foreach (KeyValuePair<int, List<int>> keyPair in dic_indexs)
            {
                var bounds = floorsCurvesTmp[keyPair.Key];
                CurveLoop curveLoop = new CurveLoop();
                List<XYZ> points = new List<XYZ>();

                for (int j = 0; j < bounds.Count; j++)
                {
                    curveLoop.Append(bounds[j]);
                    points.AddRange(bounds[j].Tessellate());
                }

                if (!_CmpGeometry.isPointInPolyline(points, pickPoint))
                    continue;

                List<CurveLoop> lstCurveLoop = new List<CurveLoop>() { curveLoop };
                var elemFloor = Floor.Create(_CmpElements.RvtDBDoc, lstCurveLoop, floorType.Id, level.Id);

                if (keyPair.Value.Count != 0 && elemFloor != null)
                {
                    _CmpElements.RvtDBDoc.Regenerate();
                    foreach (int index in keyPair.Value)
                    {
                        var bound2s = floorsCurvesTmp[index];
                        CurveArray curveAry2 = new CurveArray();
                        for (int j = 0; j < bound2s.Count; j++)
                            curveAry2.Append(bound2s[j]);
                        try { _CmpElements.RvtDBDoc.Create.NewOpening(elemFloor, curveAry2, false); }
                        catch { }
                    }
                }
                return elemFloor;
            }
            return null;
        }

        public string ErrMsg => _ErrMsg;
    }
}
