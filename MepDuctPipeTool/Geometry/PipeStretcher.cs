using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Geometry
{
  public static class PipeStretcher
  {
    public static void ShortenPipe( Pipe pipe, Connector connectorOnTargetEnd, Length shortenLength )
    {
      if ( shortenLength < Length.Zero ) throw new ArgumentException();
      ReplaceLine( pipe, connectorOnTargetEnd, shortenLength, false );
    }

    public static void ExtendPipe( Pipe pipe, Connector connectorOnTargetEnd, Length extendLength )
    {
      if ( extendLength < Length.Zero ) throw new ArgumentException();
      ReplaceLine( pipe, connectorOnTargetEnd, extendLength, true );
    }

    private static void ReplaceLine( Pipe pipe, Connector connectorOnTargetEnd, Length moveLength, bool isExtention )
    {
      var line = pipe.GetLine();

      // 移動するEndPointを取得
      var endPoints = new List<XYZ> { line.GetEndPoint( 0 ), line.GetEndPoint( 1 ) };
      var targetEndIdx = endPoints.FindIndex( ep => ep.IsAlmostEqualTo( connectorOnTargetEnd.Origin ) );
      if ( targetEndIdx < 0 ) throw new InvalidOperationException();

      // 新Line作成
      var isStartPointMoved = targetEndIdx == 0;

      var moveDir = ( isStartPointMoved, isExtention ) switch
      {
        (true, true) => -1,
        (true, false) => 1,
        (false, true) => 1,
        (false, false) => -1
      };

      var newLine = CreateExtendedLine( endPoints[0], endPoints[1], line.Direction, moveDir * moveLength, isStartPointMoved );

      if ( pipe.Location is not LocationCurve locationCurve ) throw new InvalidOperationException();
      locationCurve.Curve = newLine;
    }

    private static Line CreateExtendedLine( XYZ startPoint, XYZ endPoint, XYZ direction, Length extension, bool isStartPointMoved )
      => isStartPointMoved switch
      {
        true => Line.CreateBound( MovePoint( startPoint, direction, extension ), endPoint ),
        false => Line.CreateBound( startPoint, MovePoint( endPoint, direction, extension ) )
      };

    private static XYZ MovePoint( XYZ point, XYZ direction, Length extension )
      => point + direction * extension.LengthToRevitUnits();
  }
}