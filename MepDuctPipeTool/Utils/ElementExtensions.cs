using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class ElementExtensions
  {
    public static Curve GetCurve( this Element element )
    {
      if ( element.Location is not LocationCurve locationCurve ) throw new ArgumentException();
      return locationCurve.Curve;
    }

    public static Line GetLine( this Element element )
    {
      return element.GetCurve() as Line ?? throw new ArgumentException();
    }

    public static LocationPoint GetLocationPoint( this Element element )
      => element.Location as LocationPoint ?? throw new ArgumentException();

    public static ConnectorManager? GetConnectorManager( this Element element )
    {
      return element switch
      {
        FamilyInstance fi => fi.MEPModel?.ConnectorManager,
        MEPSystem sys => sys.ConnectorManager,
        MEPCurve crv => crv.ConnectorManager,
        _ => null,
      };
    }

    public static IEnumerable<Connector> GetConnectors( this Element element )
    {
      var connectorSet = element.GetConnectorManager()?.Connectors;
      return connectorSet?.OfType<Connector>() ?? Array.Empty<Connector>();
    }

    public static IEnumerable<Connector> GetUnusedConnectors( this Element element )
    {
      var unusedConnectorSet = element.GetConnectorManager()?.UnusedConnectors;
      return unusedConnectorSet?.OfType<Connector>() ?? Array.Empty<Connector>();
    }
  }
}