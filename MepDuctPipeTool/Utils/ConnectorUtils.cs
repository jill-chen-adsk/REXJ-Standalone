using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class ConnectorUtils
  {
    internal static Connector? FindConnectorAtSamePosition( FamilyInstance target, XYZ position )
      => target.GetConnectors().FirstOrDefault( c => c.Origin.IsAlmostEqualTo( position ) );

    internal static void ConnectPipeAndFlange( Connector pipeConnector, FamilyInstance flange )
    {
      if ( FindConnectorAtSamePosition( flange, pipeConnector.Origin ) is not { } flangeConnector ) throw new ConnectorPositionMismatchException();
      pipeConnector.ConnectTo( flangeConnector );
    }

    internal static void ConnectAccessoryAndFlange( IEnumerable<Connector> accessoryConnectors, FamilyInstance flange )
    {
      foreach ( var connector in accessoryConnectors )
      {
        if ( FindConnectorAtSamePosition( flange, connector.Origin ) is not { } flangeConnector ) continue;
        connector.ConnectTo( flangeConnector );
        return;
      }

      throw new InvalidOperationException( Resources.ERR_COMPATIBLE_CONNECTOR_NOT_FOUND );
    }
  }

  internal class ConnectorPositionMismatchException : Exception
  {
  }
}