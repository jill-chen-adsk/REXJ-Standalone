using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.RevitDBAccess
{
  public class FlangeCreator
  {
    private readonly Document _document;
    private readonly FamilySymbol _flangeSymbol;

    public FlangeCreator( Document document, FamilySymbol flangeSymbol )
    {
      _flangeSymbol = flangeSymbol;
      _document = document;
    }

    /// <summary>
    /// フランジを配置する
    /// </summary>
    /// <remarks>Transaction中に呼び出すこと</remarks>
    /// <exception cref="FlangeOriginIsNotOnConnectorException"></exception>
    internal FamilyInstance PlaceFlange( XYZ position, XYZ direction, Length radius )
    {
      // フランジの配置
      var flange = _document.Create.NewFamilyInstance( position, _flangeSymbol, StructuralType.NonStructural );
      _document.Regenerate();

      if ( ! IsOriginOnEitherConnector( flange ) ) throw new FlangeOriginIsNotOnConnectorException( );

      // 配置したフランジの方向を変更
      flange.RotateHandToAlignWith( direction, _document );

      // 配置したフランジの径を変更
      var flangeRadiusParam = ParameterGetter.GetUnionRadiusParameter( _document, flange ); // フランジのパーツタイプをユニオンにしたものを配置する前提
      flangeRadiusParam.Set( radius.LengthToRevitUnits() );
      return flange;
    }

    private bool IsOriginOnEitherConnector( FamilyInstance instance )
    {
      // フランジファミリの原点は平座面（フランジ・付属品が接続される面）のコネクタと同じ位置にあることを前提としているため、チェックする。
      var origin = instance.GetLocationPoint().Point;
      var connectors = instance.GetConnectors();
      return connectors.Any( c => c.Origin.IsAlmostEqualTo( origin ) );
    }
  }

  internal class FlangeOriginIsNotOnConnectorException : Exception
  {
    public FlangeOriginIsNotOnConnectorException()
    {
    }

    public FlangeOriginIsNotOnConnectorException( string message )
      : base( message )
    {
    }

    public FlangeOriginIsNotOnConnectorException( string message, Exception inner )
      : base( message, inner )
    {
    }
  }
}