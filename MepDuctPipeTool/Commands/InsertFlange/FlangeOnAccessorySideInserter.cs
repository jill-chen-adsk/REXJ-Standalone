using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using MepDuctPipeTool.Geometry;
using MepDuctPipeTool.RevitDBAccess;
using MepDuctPipeTool.Utils;


namespace MepDuctPipeTool.Commands.InsertFlange
{
  public class FlangeOnAccessorySideInserter
  {
    private readonly Document _document;
    private readonly FamilySymbol _flangeSymbol;

    public FlangeOnAccessorySideInserter( Document document, FamilySymbol flangeSymbol )
    {
      _flangeSymbol = flangeSymbol;
      _document = document;
    }

    public void AttachTo( FamilyInstance accessory )
    {
      using var trans = new Transaction( _document, Resources.TRANSACTION_NAME_INSERT_FLANGE );
      trans.Start();

      // TODO レデューサーとつながっている場合もある

      // ① 付属品の使用済みコネクタで配管とつながっているものを取得
      var accessoryToPipeConnectorMap = AccessoryConnectionMap.Create( accessory );

      if ( AreBothSidePipeSlopesDifferent( accessoryToPipeConnectorMap ) ) throw new BothSidePipeSlopesDifferentException();

      // ② 配管とつながっている場合は配管を分離
      accessoryToPipeConnectorMap.Disconnect( _document );

      // ③ 付属品の未使用コネクタを取得
      var unusedConnectors = accessory.GetUnusedConnectors();

      // ④ 未使用コネクタの隣にフランジを配置
      foreach ( var accessoryConnector in unusedConnectors )
      {
        var connectorOutputDirection = accessoryConnector.GetOutputDirection();
        var radius = accessoryConnector.Radius.RevitUnitsToLength();

        // ④-1 コネクタ位置にフランジを配置・回転・径変更
        var connectorPosition = accessoryConnector.Origin;
        var flangeCreator = new FlangeCreator( _document, _flangeSymbol );
        var flange = flangeCreator.PlaceFlange( connectorPosition, connectorOutputDirection, radius );
        _document.Regenerate();

        // ④-2 分離した配管を移動・接続とフランジを接続
        if ( accessoryToPipeConnectorMap.TryGetValue( accessoryConnector, out var pipeConnector ) )
        {
          var a2PConnectorMap = new KeyValuePair<Connector, Connector>( accessoryConnector, pipeConnector! );
          AdjustPipeLengthToFitFlange( flange, a2PConnectorMap );
          _document.Regenerate();

          // 配管コネクタとフランジコネクタを接続
          // 配管 + フランジ → フランジ + 付属品の順で接続する必要あり。逆だと配管の経路指定時の優先設定によってフランジが消えてしまう。
          ConnectorUtils.ConnectPipeAndFlange( pipeConnector!, flange );
        }

        // ④-3 フランジと付属品を接続
        ConnectorUtils.ConnectAccessoryAndFlange( new[] { accessoryConnector }, flange );
      }

      trans.Commit();
    }

    private static bool AreBothSidePipeSlopesDifferent( AccessoryConnectionMap map )
    {
      var pipes = map.Pipes;
      if ( pipes.Count != 2 ) return false;

      const double tolerance = 1e-6; // 経験的に決めた
      return pipes.First().GetSlope().IsDefinitelyDifferTo( pipes.Last().GetSlope(), tolerance );
    }

    // TODO 共通化
    // TODO レデューサーの場合の考慮
    private static void AdjustPipeLengthToFitFlange( FamilyInstance flange, KeyValuePair<Connector, Connector> accessoryToPipeConnectorMap )
    {
      var accessoryConnector = accessoryToPipeConnectorMap.Key;
      var pipeConnector = accessoryToPipeConnectorMap.Value;
      if ( accessoryConnector.Owner is not FamilyInstance accessory ) throw new ArgumentException();
      if ( pipeConnector.Owner is not Pipe pipe ) throw new ArgumentException();

      var length = CalcShorteningLength( flange );
      PipeStretcher.ShortenPipe( pipe, pipeConnector, length );

      return;

      static Length CalcShorteningLength( FamilyInstance flange )
      {
        return PipeFittingSizeCalculator.CalcAxisLength( flange );
      }
    }
  }

  /// <summary>
  /// 配管付属品の両端の勾配が異なる場合、配管付属品とフランジ・配管の角度が揃わないため、例外にする。
  /// </summary>
  // TODO コードが複雑化してきたら外部ファイルに出す
  internal class BothSidePipeSlopesDifferentException : Exception
  {
    public BothSidePipeSlopesDifferentException()
    {
    }

    public BothSidePipeSlopesDifferentException( string message ) : base( message )
    {
    }
  }

  /// <summary>
  /// 配管付属品のコネクタとそれに接続されている配管・配管継手との対応を管理する
  /// </summary>
  internal class AccessoryConnectionMap
  {
    private readonly Dictionary<Connector, Connector> _map;

    internal IReadOnlyCollection<Pipe> Pipes => _map.Values.Select( con => con.Owner ).OfType<Pipe>().ToArray();

    private AccessoryConnectionMap( Dictionary<Connector, Connector> map )
    {
      _map = map;
    }

    public static AccessoryConnectionMap Create( FamilyInstance accessory )
    {
      var accessoryUsedConnectors = accessory.GetConnectors().Where( c => c.IsConnected );

      Dictionary<Connector, Connector> accessoryToConnectedElemConnectorMap = new();
      foreach ( var accessoryConnector in accessoryUsedConnectors )
      {
        var connected = accessoryConnector.GetConnectedConnector();
        if ( connected?.Owner is not Pipe or FamilyInstance { MEPModel: MechanicalFitting } ) continue;
        accessoryToConnectedElemConnectorMap.Add( accessoryConnector, connected );
      }

      return new AccessoryConnectionMap( accessoryToConnectedElemConnectorMap );
    }

    internal void Disconnect( Document document )
    {
      foreach ( var connectorPair in _map )
      {
        connectorPair.Key.DisconnectFrom( connectorPair.Value );
      }

      document.Regenerate();
    }

    internal bool TryGetValue( Connector key, out Connector? value )
    {
      // Ownerが同じ && 位置が同じなら、同じコネクタとみなす
      var samePosConnector = _map.FirstOrDefault( map => ConnectorEquals( map.Key, key ) );
      value = samePosConnector.Value;

      return value is not null;
    }

    private static bool ConnectorEquals( Connector accessoryConnector, Connector comparingConnector )
    {
      var isSameOwner = accessoryConnector.Owner.Id == comparingConnector.Owner.Id;
      var isSamePosition = accessoryConnector.Origin.IsAlmostEqualTo( comparingConnector.Origin );
      return isSameOwner && isSamePosition;
    }
  }
}