using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange
{
  public class DividedPipePair
  {
    public XYZ BreakPosition { get; }
    public Pipe LeftPiece { get; } // TODO input/outputSideという名前にする
    public Pipe RightPiece { get; }
    public Connector GeneratedConnectorOfLeftPiece { get; }
    public Connector GeneratedConnectorOfRightPiece { get; }


    public static DividedPipePair Create( Document document, Pipe target, XYZ breakPosition )
    {
      var newPipeId = PlumbingUtils.BreakCurve( document, target.Id, breakPosition );
      if ( document.GetElementById<Pipe>( newPipeId ) is not { } newPipe ) throw new InvalidOperationException( Resources.ERR_CANNNOT_SPLIT_PIPE );
      return new DividedPipePair( breakPosition, target, newPipe );
    }

    private DividedPipePair( XYZ breakPosition, Pipe remainingPiece, Pipe newPiece )
    {
      BreakPosition = breakPosition;

      var isNewPipeRight = DetermineNewPipeRightSide( newPiece, remainingPiece, breakPosition );

      LeftPiece = isNewPipeRight switch
      {
        true => remainingPiece,
        false => newPiece
      };
      RightPiece = isNewPipeRight switch
      {
        true => newPiece,
        false => remainingPiece
      };

      GeneratedConnectorOfLeftPiece = FindGeneratedConnectorBySplit( LeftPiece, breakPosition );
      GeneratedConnectorOfRightPiece = FindGeneratedConnectorBySplit( RightPiece, breakPosition );
    }


    /// <remarks>切断された元pipeのDirectionに対して根本側=左, 先端側=右とする</remarks>
    static bool DetermineNewPipeRightSide( Pipe newPiece, Pipe remainingPiece, XYZ breakPosition )
    {
      // PlumbingUtils.BreakCurve()で分割されたパイプの方向は分割前と同じであることを利用
      var originalPipeDir = remainingPiece.GetLine().Direction.Normalize();

      var newPipeCenter = CalcCenter( newPiece.GetCurve() );
      var breakPosToNewPipeDir = ( newPipeCenter - breakPosition ).Normalize();
      return breakPosToNewPipeDir.IsSameDirectionTo( originalPipeDir );
    }

    private static XYZ CalcCenter( Curve curve )
    {
      var start = curve.GetEndPoint( 0 );
      var end = curve.GetEndPoint( 1 );
      return ( start + end ) * 0.5;
    }

    private static Connector FindGeneratedConnectorBySplit( Pipe pipe, XYZ breakPosition )
    {
      // pipeの終端位置がsplit時から変更されていない前提
      var unUsedConnectors = pipe.ConnectorManager.UnusedConnectors.Cast<Connector>();
      return unUsedConnectors.FirstOrDefault( c => c.Origin.IsAlmostEqualTo( breakPosition ) ) ?? throw new ArgumentException();
    }
  }
}