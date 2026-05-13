using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace MepDuctPipeTool.RevitDBAccess
{
  public static class ParameterGetter
  {
    /// <summary>
    /// FamilyInstanceのコネクタ径を決めるパラメータを取得する。
    /// </summary>
    /// <remarks>
    /// ほとんどのFamilyInstanceで入口径のパラメータとコネクタ直径or半径のパラメータが関連付けられていることを利用している。<br/>
    /// 参考: https://forums.autodesk.com/t5/revit-api-forum/set-pipe-fittings-diameter/td-p/9258474
    /// </remarks>
    public static Parameter GetAccessoryDiameterParameter( Document document, FamilyInstance accessory )
    {
      return GetSizeParameter( document, accessory );
    }

    public static Parameter GetUnionRadiusParameter( Document document, FamilyInstance union )
    {
      var mechanicalFitting = union.MEPModel as MechanicalFitting;
      if ( mechanicalFitting?.PartType is not PartType.Union ) throw new ArgumentException();

      return GetSizeParameter( document, union );
    }

    public static Parameter GetFlangeDiameterParameter( Document document, FamilyInstance flange )
    {
      // フランジの径はReadonlyParameterなので変更不可なことに注意
      // https://forums.autodesk.com/t5/revit-api-forum/incorrect-pipe-flange-radius-through-api/td-p/11585993
      var mechanicalFitting = flange.MEPModel as MechanicalFitting;
      if ( mechanicalFitting?.PartType is not PartType.PipeFlange ) throw new ArgumentException();
      return GetSizeParameter( document, flange );
    }

    private static Parameter GetSizeParameter( Document document, FamilyInstance familyInstance )
    {
      return IsConnectorDimensionTypeDiameter( familyInstance ) switch
      {
        true => GetSizeParameterAssociatedToConnector( document, familyInstance, BuiltInParameter.CONNECTOR_DIAMETER ),
        false => GetSizeParameterAssociatedToConnector( document, familyInstance, BuiltInParameter.CONNECTOR_RADIUS )
      };
    }

    public static bool IsConnectorDimensionTypeDiameter( FamilyInstance familyInstance )
    {
      // FAMILY_ROUNDCONNECTOR_DIMENSIONTYPE の次の仕様を利用する
      // dimensionType.AsInteger == 0 => 半径を使用
      // dimensionType.AsInteger == 1 => 直径を使用

      var dimensionType = familyInstance.Symbol.Family.get_Parameter( BuiltInParameter.FAMILY_ROUNDCONNECTOR_DIMENSIONTYPE );
      return dimensionType.AsInteger() == 1;
    }


    private static Parameter GetSizeParameterAssociatedToConnector( Document document, FamilyInstance familyInstance, BuiltInParameter parameter )
    {
      var mepModel = familyInstance.MEPModel ?? throw new ArgumentException();
      var connector = mepModel.ConnectorManager.Connectors.Cast<Connector>().First();
      var connectorInfo = (MEPFamilyConnectorInfo)connector.GetMEPConnectorInfo();
      var parameterId = connectorInfo.GetAssociateFamilyParameterId( new ElementId( parameter ) );
      var parameterElement = (ParameterElement)document.GetElement( parameterId );
      return familyInstance.get_Parameter( parameterElement.GetDefinition() );
    }
  }
}