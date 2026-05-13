using System ;
using Autodesk.Revit.DB ;

namespace SectionListRC.Utils
{
  public static class ParamUtil
  {
    public static double AsDoubleMm( this Parameter parameter )
    {
      if ( parameter is null ) return 0 ;
      if ( parameter.StorageType != StorageType.Double ) return 0 ;
        //throw new InvalidOperationException( "パラメータはDouble型ではありません。" ) ;
      var value = parameter.AsDouble() ;
      var unitTypeId = parameter.GetUnitTypeId() ;

      if(UnitUtils.IsMeasurableSpec( unitTypeId )) return UnitUtils.ConvertFromInternalUnits( parameter.AsDouble(), UnitTypeId.Millimeters ) ;

      //旧RUGファミリの鉄筋ピッチ、幅止筋ピッチの場合の対応
      //単位変換できない場合、鉄筋ピッチ扱いする
      var doc = parameter.Element.Document ;
      var reinforcementSpacingId = new ForgeTypeId( "autodesk.spec.aec.structural:reinforcementSpacing-2.0.0" ) ;
      var formatOption = doc.GetUnits().GetFormatOptions( reinforcementSpacingId ) ;

      return UnitUtils.Convert( value, formatOption.GetUnitTypeId(), UnitTypeId.Millimeters ) ;
    }
  }
}