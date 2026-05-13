using System.Linq;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class ConnectorExtensions
  {
    public static Connector? GetConnectedConnector( this Connector connector )
    {
      var id = connector.Owner.Id;
      return connector.AllRefs.OfType<Connector>().FirstOrDefault( c => c.Owner.Id != id );
    }

    public static XYZ GetOutputDirection( this Connector connector )
    {
      return connector.CoordinateSystem.BasisZ;
    }
  }
}