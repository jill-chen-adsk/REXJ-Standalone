using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace MepDuctPipeTool.Utils
{
  public static class PipeTypeUtil
  {
    public static IEnumerable<PipeType> GetAllPipeTypes( Document document )
    {
      var collector = new FilteredElementCollector( document );
      return collector.OfClass( typeof( PipeType ) ).ToArray().Cast<PipeType>();
    }
  }
}