using System ;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using MepDuctPipeTool.Geometry;
using MepDuctPipeTool.RevitDBAccess;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange
{
  public class PipeAccessoryWithFlangeInserter
  {
    private readonly Document _document;
    private readonly FamilySymbol _accessorySymbol;
    private readonly FamilySymbol _flangeSymbol;

    public PipeAccessoryWithFlangeInserter( Document document, FamilySymbol accessorySymbol, FamilySymbol flangeSymbol )
    {
      _document = document;
      _accessorySymbol = accessorySymbol;
      _flangeSymbol = flangeSymbol;

      FamilySymbolUtils.ActivateSymbolIfNeeded( _document, _accessorySymbol );
    }


    public void InsertTo( Pipe pipe, XYZ position )
    {
      //シンボルの有効化、※トランザクションが走る処理なのでトランザクション外で行う必要あり
      FamilySymbolUtils.ActivateSymbolIfNeeded( _document, _flangeSymbol );
      
      using var trans = new Transaction( _document, Resources.TRANSACTION_NAME_INSERT_PIPE_ACCESSORY_WITH_FLANGE );
      trans.Start();

      var pipeDirection = pipe.GetLine().Direction;
      var pipeDiameter = pipe.Diameter.RevitUnitsToLength();

      // ① 配管を2つに分割
      var dividedPipePair = SplitPipe( pipe, position );

      // ② 切断位置に付属品配置
      var accessory = PlaceAccessory( position, pipeDirection, pipeDiameter );
      _document.Regenerate();

      // ③ 付属品の両端点にフランジを配置
      var flanges = PlaceFlanges( ( accessory.InputPosition, accessory.OutputPosition ), pipeDiameter * 0.5 );
      _document.Regenerate();

      // ④ 分割された配管の端点を付属品+フランジの端の位置まで移動
      AdjustPipeLengthToFitFlange( accessory, flanges, dividedPipePair );
      _document.Regenerate();

      // ⑤ 配管コネクタとフランジコネクタを接続
      // 配管 + フランジ → フランジ + 付属品の順で接続する必要あり。逆だと配管の経路指定時の優先設定によってフランジが消えてしまう。
      ConnectorUtils.ConnectPipeAndFlange( dividedPipePair.GeneratedConnectorOfLeftPiece, flanges.inputSide );
      ConnectorUtils.ConnectPipeAndFlange( dividedPipePair.GeneratedConnectorOfRightPiece, flanges.outputSide );

      // ⑥ フランジコネクタと付属品コネクタを接続
      ConnectorUtils.ConnectAccessoryAndFlange( accessory.Connectors, flanges.inputSide );
      ConnectorUtils.ConnectAccessoryAndFlange( accessory.Connectors, flanges.outputSide );

      trans.Commit();
    }

    private static void AdjustPipeLengthToFitFlange( AccessoryPlacementResult accessory, (FamilyInstance inputSide, FamilyInstance outputSide) flanges, DividedPipePair dividedPipePair )
    {
      var accessoryLength = PipeAccessorySizeCalculator.CalcLengthFromOriginToEnd( accessory );

      var extendLenInputSide = CalcExtendLength( flanges.inputSide, accessoryLength.inputToOriginLength );
      PipeStretcher.ShortenPipe( dividedPipePair.RightPiece, dividedPipePair.GeneratedConnectorOfRightPiece, extendLenInputSide );

      var extendLenOutputSide = CalcExtendLength( flanges.outputSide, accessoryLength.outputToOriginLength );
      PipeStretcher.ShortenPipe( dividedPipePair.LeftPiece, dividedPipePair.GeneratedConnectorOfLeftPiece, extendLenOutputSide );
      return;

      static Length CalcExtendLength( FamilyInstance flange, Length targetAccessoryLength )
      {
        var flangeLength = PipeFittingSizeCalculator.CalcAxisLength( flange );
        return targetAccessoryLength + flangeLength;
      }
    }

    private DividedPipePair SplitPipe( Pipe pipe, XYZ breakPosition )
      => DividedPipePair.Create( _document, pipe, breakPosition );

    private AccessoryPlacementResult PlaceAccessory( XYZ position, XYZ direction, Length diameter )
    {
      // 配管付属品の配置
      var accessory = _document.Create.NewFamilyInstance( position, _accessorySymbol, StructuralType.NonStructural );
      _document.Regenerate();

      // 配置した付属品の方向を配管に合わせるように補正
      accessory.RotateToReverseAlign( direction, _document );
      _document.Regenerate();

      // 取っ手がZ+方向を向くように補正
      if ( ! direction.IsParallelTo( XYZ.BasisZ ) )
      {
        accessory.RotateTopToAlignWith( XYZ.BasisZ, _document );
      }
      else
      {
        // 配置先配管が竪管の場合は”前”方向を向くように補正
        accessory.RotateTopToAlignWith( -XYZ.BasisY, _document );
      }

      // 配置した付属品の径を変更
      // アクセサリの径指定パラメータには半径と直径のどちらもあるのでそれを考慮
      var fittingDiameterOrRadiusParam = ParameterGetter.GetAccessoryDiameterParameter( _document, accessory ) ;

      var diameterOrRadiusValue = ParameterGetter.IsConnectorDimensionTypeDiameter( accessory ) ? diameter.LengthToRevitUnits() : diameter.LengthToRevitUnits() * 0.5 ;

      Console.WriteLine(fittingDiameterOrRadiusParam.Definition.Name);
      if ( fittingDiameterOrRadiusParam.IsReadOnly ) {
        //入口半径 (既定値)、入口径 (既定値)で決め打ちで対応
        var radiusParam = accessory.LookupParameter( "入口半径" ) ;
        var diameterParam = accessory.LookupParameter( "入口径" ) ;
        if ( radiusParam?.IsReadOnly == false ) {
          radiusParam?.Set( diameter.LengthToRevitUnits() * 0.5 ) ;
        }else if ( diameterParam?.IsReadOnly == false ) {
          diameterParam?.Set( diameter.LengthToRevitUnits() ) ;
        }
      }
      else {
        fittingDiameterOrRadiusParam.Set( diameterOrRadiusValue ) ;
      }
      

      return new AccessoryPlacementResult( accessory, direction );
    }

    private (FamilyInstance inputSide, FamilyInstance outputSide) PlaceFlanges( (XYZ inputSidePos, XYZ outputSidePos) positions, Length radius )
    {
      var ioDirection = ( positions.outputSidePos - positions.inputSidePos ).Normalize(); // TODO 配管の方向に合わせるようにする

      var flangeCreator = new FlangeCreator( _document, _flangeSymbol );
      var inputSideFlange = flangeCreator.PlaceFlange( positions.inputSidePos, -ioDirection, radius );
      var outputSideFlange = flangeCreator.PlaceFlange( positions.outputSidePos, ioDirection, radius );

      return ( inputSideFlange, outputSideFlange );
    }
  }
}