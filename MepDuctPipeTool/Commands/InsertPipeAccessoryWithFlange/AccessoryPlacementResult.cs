using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MepDuctPipeTool.Utils;

// TODO 適切な名前空間を検討する
namespace MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange
{
  public class AccessoryPlacementResult
  {
    public FamilyInstance Accessory { get; }
    public Connector InputSideConnector { get; }

    public Connector OutputSideConnector { get; }
    public Connector? BranchSideConnector { get; }
    public XYZ InputPosition => InputSideConnector.Origin;
    public XYZ OutputPosition => OutputSideConnector.Origin;
    public XYZ BranchPosition => BranchSideConnector?.Origin ?? throw new InvalidOperationException();
    public IEnumerable<Connector> Connectors { get; }

    public AccessoryPlacementResult( FamilyInstance accessory, XYZ pipeDirection )
    {
      Accessory = accessory;
      var ioConnectors = GetAccessoryIoConnector( accessory, pipeDirection );
      InputSideConnector = ioConnectors.inputSide;
      OutputSideConnector = ioConnectors.outputSide;
      BranchSideConnector = ioConnectors.branchSide;
      Connectors = ( BranchSideConnector is null ) switch
      {
        true => new[] { InputSideConnector, OutputSideConnector },
        false => new[] { InputSideConnector, OutputSideConnector, BranchSideConnector }
      };
    }


    private static (Connector inputSide, Connector outputSide, Connector? branchSide) GetAccessoryIoConnector( FamilyInstance accessory, XYZ pipeDirection )
    {
      var connectors = accessory.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToArray();
      switch ( connectors.Length )
      {
        case 2:
          var a = Identify2ConnectorsDirection( connectors, pipeDirection );
          return ( a.inputSide, a.outputSide, null );
        case 3:
          return Identify3ConnectorsDirection( connectors, pipeDirection, accessory );
        default:
          throw new InvalidOperationException( Resources.ERR_CONNECTOR_NUM_IS_UNSUPPORTED );
      }
    }


    private static (Connector inputSide, Connector outputSide) Identify2ConnectorsDirection( Connector[] connectors, XYZ pipeDirection )
    {
      return IsInputSide( connectors[0], connectors[1], pipeDirection ) switch
      {
        true => ( connectors[0], connectors[1] ),
        false => ( connectors[1], connectors[0] )
      };

      static bool IsInputSide( Connector target, Connector another, XYZ direction )
      {
        var connectorsDirection = another.Origin - target.Origin;
        return connectorsDirection.IsSameDirectionTo( direction );
      }
    }

    private static (Connector inputSide, Connector outputSide, Connector branchSide) Identify3ConnectorsDirection( Connector[] connectors, XYZ pipeDirection, FamilyInstance accessory )
    {
      Connector? sameDirCon = null;
      Connector? reverseDirCon = null;
      Connector? branchDirCon = null;

      foreach ( var connector in connectors )
      {
        var dir = GetDirFromAccessoryOrigin( connector, accessory );
        if ( dir.IsSameDirectionTo( pipeDirection ) )
        {
          sameDirCon = connector;
          continue;
        }

        if ( dir.IsOppositeDirectionTo( pipeDirection ) )
        {
          reverseDirCon = connector;
          continue;
        }

        branchDirCon = connector;
      }

      if ( sameDirCon is null || reverseDirCon is null || branchDirCon is null ) throw new InvalidOperationException( Resources.ERR_CANNOT_IDENTIFY_CONNECTOR_DIRECTIONS );
      return ( sameDirCon, reverseDirCon, branchDirCon );
    }


    private static XYZ GetDirFromAccessoryOrigin( Connector connector, FamilyInstance accessory )
      => accessory.GetLocationPoint().Point - connector.Origin;
  }
}