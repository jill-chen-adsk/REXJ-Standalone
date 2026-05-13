using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Geometry
{
  public static class PipeFittingSizeCalculator
  {
    public static Length CalcAxisLength( FamilyInstance familyInstance )
    {
      // コネクタが2つのものが対象
      if ( familyInstance.MEPModel is not MechanicalFitting mechanicalFitting ) throw new ArgumentException();

      var connectors = mechanicalFitting.ConnectorManager.Connectors;
      if ( connectors.Size != 2 ) throw new ArgumentException();

      var origins = connectors.Cast<Connector>().Select( c => c.Origin ).ToArray();
      return ( origins[1] - origins[0] ).GetLength().RevitUnitsToLength();
    }
  }
}