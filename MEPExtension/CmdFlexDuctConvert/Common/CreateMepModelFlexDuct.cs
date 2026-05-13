using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;

namespace FlexibleDuctMaking
{
    class CreateMepModelFlexDuct
    {
        public FlexDuct flexduct { get; private set; } = null;

        public Result createmepmodel(Document doc, Level lvl,List<XYZ>points,
                                     XYZ starttangent,XYZ endtangent,
                                     ElementId ductSysTypeId)
        {
            // find a pipe type
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FlexDuctType));
            //ElementId ductTypeId = collector.FirstElementId();
            ElementId ductTypeId = null;
            foreach(Element ele in collector)
            {
                if ( ele is FlexDuctType ductType ) {
                    //ConnectorProfileType.Round == 0
                    if ( ductType.Shape == 0) {
                        ductTypeId = ele.Id ;
                    }
                }
            }

            //DuctのシステムIDを既存情報から取得するから以下の処理を不要
            /*
            // find a pipe system type
            FilteredElementCollector sysCollector = new FilteredElementCollector(doc);
            sysCollector.OfClass(typeof(MechanicalSystemType));
            ElementId ductSysTypeId = sysCollector.FirstElementId();
            */

            if (ductTypeId != ElementId.InvalidElementId && ductSysTypeId != ElementId.InvalidElementId)
            {
                //create flex pipe with 3 points
                //double dx = 10.0;
                //double dy = 10.0;

                //List<XYZ> pointsInternal = new List<XYZ>();
                /*
                points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 0.00000000000000));
                points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 3.03477690300000));//中間点
                points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 6.06955380600000));
                points.Add(new XYZ(dx + 2.952755906, dy + 0.0, 9.02230971128609));
                points.Add(new XYZ(dx + 6.397637795, dy + 0.0, 9.02230971128609));//中間点
                points.Add(new XYZ(dx + 9.842519685, dy + 0.0, 9.02230971128609));
                points.Add(new XYZ(dx + 9.842519685 + 20.0, dy + 0.0, 9.02230971128609));
                */

                //points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 0.00000000000000));
                //points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 3.03477690300000));//中間点
                //points.Add(new XYZ(dx + 0.000000000, dy + 0.0, 6.06955380600000));
                //points.Add(new XYZ(dx + 2.952755906, dy + 0.0, 9.02230971128609));
                //points.Add(new XYZ(dx + 6.397637795, dy + 0.0, 9.02230971128609));//中間点
                //points.Add(new XYZ(dx + 9.842519685, dy + 0.0, 9.02230971128609));
                //points.Add(new XYZ(dx + 9.842519685 + 20.0, dy + 0.0, 9.02230971128609));
                //points.Add(new XYZ(dx + 9.842519685 + 23.0, dy + 3.0, 9.02230971128609));
                //points.Add(new XYZ(dx + 9.842519685 + 23.0, dy + 6.0, 9.02230971128609));
                //points.Add(new XYZ(dx + 9.842519685 + 23.0, dy + 12.0, 9.02230971128609));

                //単位変換
                /*
                foreach(XYZ point in points)
                {
                    pointsInternal.Add(new XYZ(
                        UnitUtils.ConvertToInternalUnits(point.X, UnitTypeId.Millimeters),
                        UnitUtils.ConvertToInternalUnits(point.Y, UnitTypeId.Millimeters),
                        UnitUtils.ConvertToInternalUnits(point.Z, UnitTypeId.Millimeters)));
                }
                */

                //XYZ starttangent = new XYZ(0.0, 0.0, 1.0);
                //XYZ endtangent = new XYZ(1.0, 0.0, 0.0);


                //pipe = FlexPipe.Create(doc, pipeSysTypeId, pipeTypeId, lvl.Id, points);
                //duct = FlexDuct.Create(doc, ductSysTypeId, ductTypeId, lvl.Id, starttangent, endtangent, pointsInternal);
                flexduct = FlexDuct.Create(doc, ductSysTypeId, ductTypeId, lvl.Id, starttangent, endtangent, points);
            }

            return Result.Succeeded;
        }
    }
}
