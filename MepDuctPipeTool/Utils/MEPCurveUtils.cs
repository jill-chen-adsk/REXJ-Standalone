using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class MepCurveUtils
  {
    public static XYZ ProjectToAxis( MEPCurve mepCurve, XYZ point )
    {
      var axis = mepCurve.GetCurve();
      return axis.Project( point ).XYZPoint;
    }
  }
}