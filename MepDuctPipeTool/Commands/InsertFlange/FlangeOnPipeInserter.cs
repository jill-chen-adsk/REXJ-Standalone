using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange;
using MepDuctPipeTool.Geometry;
using MepDuctPipeTool.RevitDBAccess;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Commands.InsertFlange
{
  public class FlangeOnPipeInserter

  {
    private readonly Document _document;
    private readonly FamilySymbol _flangeSymbol;

    public FlangeOnPipeInserter( Document document, FamilySymbol flangeSymbol )
    {
      _document = document;
      _flangeSymbol = flangeSymbol;
    }

    public void InsertTo( Pipe pipe, XYZ position )
    {
      using var trans = new Transaction( _document, Resources.TRANSACTION_NAME_INSERT_FLANGE );
      trans.Start();

      var pipeDirection = pipe.GetLine().Direction;
      var pipeDiameter = pipe.Diameter.RevitUnitsToLength();

      // ① 配管を2つに分割
      var dividedPipePair = SplitPipe( pipe, position );

      // ② 切断位置にフランジを配置
      var flanges = PlaceFlanges( ( position, position ), pipeDirection, pipeDiameter * 0.5 );
      _document.Regenerate();

      // ③ 分割された配管の端点をフランジの端の位置まで移動
      AdjustPipeLengthToFitFlange( flanges, dividedPipePair );
      _document.Regenerate();

      // ④ 配管コネクタとフランジコネクタを接続
      ConnectorUtils.ConnectPipeAndFlange( dividedPipePair.GeneratedConnectorOfLeftPiece, flanges.inputSide );
      ConnectorUtils.ConnectPipeAndFlange( dividedPipePair.GeneratedConnectorOfRightPiece, flanges.outputSide );

      // ⑤ フランジコネクタとフランジコネクタを接続
      ConnectFlanges( flanges );

      trans.Commit();
    }


    // TODO DividedPipePairをInsertPipeAccessoryWithFlange名前空間から外に出す
    private DividedPipePair SplitPipe( Pipe pipe, XYZ breakPosition )
      => DividedPipePair.Create( _document, pipe, breakPosition );

    // TODO 重複コード PipeAccessoryWithFlangeInserter.cs
    private (FamilyInstance inputSide, FamilyInstance outputSide) PlaceFlanges( (XYZ inputSidePos, XYZ outputSidePos) positions, XYZ ioDirection, Length flangeRadius )
    {
      var flangeCreator = new FlangeCreator( _document, _flangeSymbol );
      var inputSideFlange = flangeCreator.PlaceFlange( positions.inputSidePos, -ioDirection, flangeRadius );
      var outputSideFlange = flangeCreator.PlaceFlange( positions.outputSidePos, ioDirection, flangeRadius );

      return ( inputSideFlange, outputSideFlange );
    }

    private static void AdjustPipeLengthToFitFlange( (FamilyInstance inputSide, FamilyInstance outputSide) flanges, DividedPipePair dividedPipePair )
    {
      var extendLenInputSide = PipeFittingSizeCalculator.CalcAxisLength( flanges.inputSide );
      PipeStretcher.ShortenPipe( dividedPipePair.RightPiece, dividedPipePair.GeneratedConnectorOfRightPiece, extendLenInputSide );

      var extendLenOutputSide = PipeFittingSizeCalculator.CalcAxisLength( flanges.outputSide );
      PipeStretcher.ShortenPipe( dividedPipePair.LeftPiece, dividedPipePair.GeneratedConnectorOfLeftPiece, extendLenOutputSide );
    }


    private static void ConnectFlanges( (FamilyInstance inputSide, FamilyInstance outputSide) flanges )
    {
      var connectorOfInputSide = flanges.inputSide.GetUnusedConnectors().Single();
      var connectorOfOutputSide = flanges.outputSide.GetUnusedConnectors().Single();

      connectorOfInputSide.ConnectTo( connectorOfOutputSide );
    }
  }
}