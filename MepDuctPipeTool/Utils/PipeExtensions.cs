using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace MepDuctPipeTool.Utils
{
  internal static class PipeExtensions
  {
    /// <summary>
    /// 勾配(単位長さあたりに変化する高さ)を返す
    /// </summary>
    internal static double GetSlope( this Pipe pipe )
    {
      return pipe.get_Parameter( BuiltInParameter.RBS_PIPE_SLOPE ).AsDouble();
    }
  }
}