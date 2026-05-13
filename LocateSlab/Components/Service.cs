using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Components
{
    public class Service
    {
        private readonly Attribute _cmpAttribute;
        private readonly Elements _cmpElements;
        private readonly Geometry _cmpGeometry;
        private readonly Parameters _cmpParameters;
        private readonly Settings _cmpSettings;
        private readonly Entities.DtItems _entDtItems;
        private string _errMsg;

        public Service(Attribute cmpAttribute, Elements cmpElements, Geometry cmpGeometry,
            Parameters cmpParameters, Settings cmpSettings)
        {
            _cmpAttribute = cmpAttribute;
            _cmpElements = cmpElements;
            _cmpGeometry = cmpGeometry;
            _cmpParameters = cmpParameters;
            _cmpSettings = cmpSettings;
            _entDtItems = new Entities.DtItems(_cmpAttribute);
            _errMsg = "";
        }

        public string ErrMsg => _errMsg;

        public IList<Element> GetSelSetBeams()
        {
            var ret = new List<Element>();
            var sysTypes = new List<Type> { typeof(FamilyInstance) };
            var categories = new List<Category>
            {
                _cmpElements.GetCategory(BuiltInCategory.OST_StructuralFraming)
            };

            var elems = _cmpElements.GetElementsSelection(sysTypes, categories, null, true);
            foreach (var elem in elems)
            {
                int iValue = 0;
                _cmpParameters.GetValue(elem, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, ref iValue);
                if (iValue == 3 || iValue == 4)
                    ret.Add(elem);
            }
            return ret;
        }

        public bool CreateSlab(Entities.DtSlabType entDtSlabType, IList<Element> elemBeams,
            string strHeightOffset, string strDegreeAngle)
        {
            _errMsg = "";

            FloorType floorType = entDtSlabType.WorkElem as FloorType;
            if (floorType == null)
            {
                _errMsg = _cmpAttribute.ResourceText("IDS_ERR_SLABTYPE");
                return false;
            }

            double heightOffset = 0.0;
            if (double.TryParse(strHeightOffset, out double ho))
                heightOffset = ho;
            heightOffset /= _cmpGeometry.UnitCoe;

            double directionAngle = 0.0;
            if (double.TryParse(strDegreeAngle, out double da))
                directionAngle = da;
            directionAngle *= (Math.PI / 180);

            var beams = new List<FamilyInstance>();
            foreach (var elem in elemBeams)
            {
                if (elem is FamilyInstance fi) beams.Add(fi);
            }
            if (beams.Count == 0)
            {
                _errMsg = _cmpAttribute.ResourceText("IDS_ERR_BEAMS");
                return false;
            }

            Level beamLevel = null;
            var paramRefLevel = beams[0].get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
            if (paramRefLevel != null)
            {
                var elemHost = _cmpElements.GetElementDoc(
                    int.Parse(paramRefLevel.AsElementId().ToString()));
                beamLevel = elemHost as Level;
            }
            if (beamLevel == null)
            {
                _errMsg = _cmpAttribute.ResourceText("IDS_ERR_LEVELBEAMS");
                return false;
            }
            double beamHeight = beamLevel.Elevation;

            var beamsCurve = new List<Curve>();
            foreach (var beam in beams)
            {
                var curve = _cmpGeometry.GetElementLocCurve(beam);
                if (curve != null) beamsCurve.Add(curve);
            }
            if (beamsCurve.Count == 0)
            {
                _errMsg = _cmpAttribute.ResourceText("IDS_ERR_CURVBEAMS");
                return false;
            }

            _cmpGeometry.ToleranceInter = _entDtItems.ToleranceInter / _cmpGeometry.UnitCoe;

            var beamsInterPosAry = _cmpGeometry.GetInterPosCurves(beamsCurve);
            var floorsCurvesTmp = _cmpGeometry.GetPlanFaceCurveInterPos(beamsInterPosAry, beamHeight);

            for (int i = 0; i < floorsCurvesTmp.Count; ++i)
            {
                var curveLoop = new CurveLoop();
                for (int j = 0; j < floorsCurvesTmp[i].Count; ++j)
                    curveLoop.Append(floorsCurvesTmp[i][j]);

                var lstCurveLoop = new List<CurveLoop> { curveLoop };
                var elemFloor = Floor.Create(_cmpElements.RvtDBDoc, lstCurveLoop,
                    floorType.Id, beamLevel.Id, true, null, 0);

                if (elemFloor != null)
                {
                    _cmpParameters.SetValue(elemFloor,
                        BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM, heightOffset);
                    elemFloor.SpanDirectionAngle = directionAngle;
                }
            }
            return true;
        }
    }
}
