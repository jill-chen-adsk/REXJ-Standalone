using System ;
using System.Collections.Generic ;
using System.Linq ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Mechanical ;
using Autodesk.Revit.DB.Plumbing ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.UI ;

namespace MEPConnectTool.Utils ;

public static class DuctUtils
{
  public enum DuctFamilySymbolType
  {
    DuctFamilySymbolTypeJointRectangle,
    DuctFamilySymbolTypeJointRectangleS,
    DuctFamilySymbolTypeJointRectangleElbow1R,
    DuctFamilySymbolTypeJointRectangleElbowAtypical1R,
    DuctFamilySymbolTypeJointRectangleElbowInnerR,
    DuctFamilySymbolTypeJointRectangleElbowAtypicalInnerR,
    DuctFamilySymbolTypeJointRoundElbow,
    DuctFamilySymbolTypeRound,
    DuctFamilySymbolTypeRoundS
  }

  public enum RefPlane
  {
    Xy,
    Xz,
    Yz
  }

  // private enum RoundJointType
  // {
  //   Joint90,Joint45,JointS
  // }


  public const double AngleToleranceDegree = 1e-6 ;

  // public static (Connector?, Connector?) NearestConnectors( Duct duct0, Duct duct1 )
  // {
  //   var connectors0 = duct0.ConnectorManager.UnusedConnectors.Cast<Connector>() ;
  //   var connectors1 = duct1.ConnectorManager.UnusedConnectors.Cast<Connector>() ;
  //
  //   Connector? closestConnector0 = null ;
  //   Connector? closestConnector1 = null ;
  //   var minimumDistance = double.MaxValue ;
  //
  //   foreach ( var c0 in connectors0 ) {
  //     foreach ( var c1 in connectors1 ) {
  //       var distance = c0.Origin.DistanceTo( c1.Origin ) ;
  //       // Console.WriteLine($"c0: {c0.Origin.ToF2()} c1:{c1.Origin} dist:{distance}");
  //       if ( distance < minimumDistance ) {
  //         // Console.WriteLine($"**Min** c0: {c0.Origin.ToF2()} c1:{c1.Origin} dist:{distance}");
  //         minimumDistance = distance ;
  //         closestConnector0 = c0 ;
  //         closestConnector1 = c1 ;
  //       }
  //     }
  //   }
  //
  //   return ( closestConnector0, closestConnector1 ) ;
  // }

  public static (Connector?, Connector?) NearestConnectors( MEPCurve curve0, MEPCurve curve1 )
  {
    var connectors0 = curve0.ConnectorManager.UnusedConnectors.Cast<Connector>() ;
    var connectors1 = curve1.ConnectorManager.UnusedConnectors.Cast<Connector>() ;

    Connector? closestConnector0 = null ;
    Connector? closestConnector1 = null ;
    var minimumDistance = double.MaxValue ;

    foreach ( var c0 in connectors0 ) {
      foreach ( var c1 in connectors1 ) {
        var distance = c0.Origin.DistanceTo( c1.Origin ) ;
        if ( distance < minimumDistance ) {
          minimumDistance = distance ;
          closestConnector0 = c0 ;
          closestConnector1 = c1 ;
        }
      }
    }

    return ( closestConnector0, closestConnector1 ) ;
  }


  public static FamilySymbol? RectDuctFamilySymbol( this Document doc, DuctFamilySymbolType symbolType )
  {
    var familySymbolName = symbolType switch
    {
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangle => "041_角_ホッパー_長さ",
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleS => "051_角_S管_垂直",

      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleElbow1R => "011_角_エルボ_1R",
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleElbowAtypical1R => "011_角_エルボ_異形_1R",
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleElbowInnerR => "011_角_エルボ_内R設定",
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleElbowAtypicalInnerR => "011_角_エルボ_異形_内R指定",
      DuctFamilySymbolType.DuctFamilySymbolTypeJointRoundElbow => "011_丸_エルボ",
      DuctFamilySymbolType.DuctFamilySymbolTypeRound => "00_丸ダクト",
      DuctFamilySymbolType.DuctFamilySymbolTypeRoundS => "051_丸_S管",
      _ => "",
    } ;

    return doc.RectDuctFamilySymbol( familySymbolName ) ;
  }

  public static FamilySymbol? RectDuctFamilySymbol( this Document doc, string familySymbolName )
  {
    var collector = new FilteredElementCollector( doc ) ;

    var list = collector.OfClass( typeof( Family ) ).ToArray() ;

    var family = collector.OfClass( typeof( Family ) ).FirstOrDefault( q => q.Name == familySymbolName ) as Family ;
    if ( family is null ) {
      Console.WriteLine( "[Error] Missing family" ) ;

      TaskDialog.Show( "Missing Family", $"The family \"{familySymbolName}\" is required to execute this command." ) ;

      return null ;
    }

    var familySymbolId = family.GetFamilySymbolIds().FirstOrDefault() ;
    var familySymbol = doc.GetElement( familySymbolId ) as FamilySymbol ;
    if ( familySymbol is null ) return null ;

    if ( ! familySymbol.IsActive ) {
      familySymbol.Activate() ;
      doc.Regenerate() ;
    }

    return familySymbol ;
  }


  // public static XYZ EndDirectionAtPoint( this Duct duct, XYZ point )
  // {
  //   var curve = ( (LocationCurve)duct.Location ).Curve ;
  //   var ep0 = curve.GetEndPoint( 0 ) ;
  //   var ep1 = curve.GetEndPoint( 1 ) ;
  //
  //   //Console.WriteLine($"pt:{point} ep0:{ep0.ToF2()} ep1:{ep1.ToF2()} //{point.IsAlmostEqualTo( ep0 )}");
  //
  //   var dir = point.IsAlmostEqualTo( ep0 ) ? ep0 - ep1 : ep1 - ep0 ;
  //
  //   return dir.Normalize() ;
  // }

  public static XYZ EndDirectionAtPoint( this MEPCurve mepCurve, XYZ point )
  {
    var curve = ( (LocationCurve)mepCurve.Location ).Curve ;
    var ep0 = curve.GetEndPoint( 0 ) ;
    var ep1 = curve.GetEndPoint( 1 ) ;

    //Console.WriteLine($"pt:{point} ep0:{ep0.ToF2()} ep1:{ep1.ToF2()} //{point.IsAlmostEqualTo( ep0 )}");

    var dir = point.IsAlmostEqualTo( ep0 ) ? ep0 - ep1 : ep1 - ep0 ;

    return dir.Normalize() ;
  }


  public static double MmToFt( this double value ) => value * 0.00328083989501312 ;
  public static double FtToMm( this double value ) => value * 304.8 ;

  public static string ToF1( this double value ) => value.ToString( "F1" ) ;
  public static string ToF2( this double value ) => value.ToString( "F2" ) ;


  public static (double, double, double) DecomposeVector( this XYZ inputDir, XYZ dir )
  {
    var orthoDirX = dir.CrossProduct( XYZ.BasisZ ).Normalize() ;
    var orthoDirZ = dir.CrossProduct( XYZ.BasisX ).Normalize() ;

    var l = inputDir.DotProduct( dir ) ;
    var lOrthoX = -inputDir.DotProduct( orthoDirX ) ;
    var lOrthoZ = -inputDir.DotProduct( orthoDirZ ) ;

    return ( l, lOrthoX, lOrthoZ ) ;
  }

  /// <summary>
  /// ベクトルを投影ベクトルの要素に分解
  /// </summary>
  /// <param name="inputDir"></param>
  /// <param name="dir"></param>
  /// <returns>l, lOrthoX, lOrthoY, lOrthoZ</returns>
  public static (double, double, double, double) DecomposeVector2( this XYZ inputDir, XYZ dir )
  {
    var orthoDirX = dir.CrossProduct( XYZ.BasisZ ).Normalize() ;
    var orthoDirY = dir.CrossProduct( XYZ.BasisY ).Normalize() ;
    var orthoDirZ = dir.CrossProduct( XYZ.BasisX ).Normalize() ;

    var l = inputDir.DotProduct( dir ) ;
    var lOrthoX = -inputDir.DotProduct( orthoDirX ) ;
    var lOrthoY = -inputDir.DotProduct( orthoDirY ) ;
    var lOrthoZ = -inputDir.DotProduct( orthoDirZ ) ;

    return ( l, lOrthoX, lOrthoY, lOrthoZ ) ;
  }


  public static bool AreCollinear( XYZ point1, XYZ point2, XYZ vector )
  {
    var direction = point2 - point1 ;
    var crossProduct = direction.CrossProduct( vector ) ;
    return crossProduct.IsZeroLength() ;
  }

  public static bool IsAlmostEqualToZero( this double value )
  {
    return value is > -0.00001 and < 0.00001 ;
  }

  public static (Duct?, Duct?) PickDuctPair( this UIDocument uiDoc )
  {
    var elem0 = uiDoc.PickElement() ;
    if ( elem0 is null ) throw new OperationCanceledException() ;
    if ( elem0 is not Duct duct0 ) return ( null, null ) ;
    var elem1 = uiDoc.PickElement() ;
    if ( elem1 is null ) throw new OperationCanceledException() ;
    if ( elem1 is not Duct duct1 ) return ( null, null ) ;
    if ( duct0 == duct1 ) return ( null, null ) ;
    return ( duct0, duct1 ) ;
  }

  public static (Element?, Element?) PickElementPair( this UIDocument uiDoc )
  {
    var elem0 = uiDoc.PickElement() ;
    if ( elem0 is null ) throw new OperationCanceledException() ;
    if ( elem0 is not Duct && elem0 is not Pipe ) return ( null, null ) ;
    var elem1 = uiDoc.PickElement() ;
    if ( elem1 is not Duct && elem1 is not Pipe ) return ( null, null ) ;
    if ( elem1 is null ) throw new OperationCanceledException() ;
    if ( elem0 == elem1 ) return ( null, null ) ;
    return ( elem0, elem1 ) ;
  }

  // ==============================================================

  #region ConnectParallelRectDuct

  public static Result BeginConnectParallelRectDuct( this UIApplication uiApp, bool isFixedLength, bool isHopperPreferred, double length )
  {
    //Console.WriteLine($"== BeginConnectParallelRectDuct isFixedLength:{isFixedLength} Length:{length}");

    try {
      var uiDoc = uiApp.ActiveUIDocument ;
      var doc = uiDoc.Document ;

      while ( true ) {
        var (duct0, duct1) = uiDoc.PickDuctPair() ;
        if ( duct0 is null || duct1 is null ) return Result.Cancelled ;
        //if ( duct0 == duct1 ) return Result.Cancelled ;

        var sizeStr0 = duct0.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;
        var sizeStr1 = duct1.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;

        //近傍コネクタの検出
        var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
        if ( connector0 is null || connector1 is null ) return Result.Cancelled ;

        var dir0 = duct0.EndDirectionAtPoint( connector0.Origin ) ;
        var dir1 = duct1.EndDirectionAtPoint( connector1.Origin ) ;
        var crossProduct = dir0.CrossProduct( connector1.Origin - connector0.Origin ) ;
        var areCollinear = crossProduct.IsZeroLength() ;
        var areCoplanar = crossProduct.DotProduct( dir1 ).IsAlmostEqualToZero() ;
        var areSameSize = sizeStr0 == sizeStr1 ;
        var areCoplanarZ = XYZ.BasisZ.DotProduct( dir0 ).IsAlmostEqualToZero() && XYZ.BasisZ.DotProduct( dir1 ).IsAlmostEqualToZero() ;

        //平行の場合crossProductが0ベクトル
        Console.WriteLine( $"Parallel:{dir0.CrossProduct( dir1 ).IsZeroLength()} Opposite:{( dir0 + dir1 ).IsZeroLength()}" ) ;
        if ( ! dir0.CrossProduct( dir1 ).IsZeroLength() || ! ( dir0 + dir1 ).IsZeroLength() ) return Result.Cancelled ;


        //bool isFixedLength, double lengthをつけて処理

        if ( isHopperPreferred ) return ConnectHopperDuct( doc, duct0, duct1, isFixedLength, length ) ? Result.Succeeded : Result.Cancelled ;

        var result = areCollinear switch
        {
          true when areSameSize => ConnectHopperDuct( doc, duct0, duct1, isFixedLength, length ) ? Result.Succeeded : Result.Cancelled,
          false when areCoplanar && areSameSize && areCoplanarZ => ConnectRectangleSDuct( doc, duct0, duct1, isFixedLength, length ) ? Result.Succeeded : Result.Cancelled,
          _ => ConnectHopperDuct( doc, duct0, duct1, isFixedLength, length ) ? Result.Succeeded : Result.Cancelled
        } ;
      }
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
    }

    return Result.Succeeded ;
  }


  /// <summary>
  /// S管接続
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct0"></param>
  /// <param name="duct1"></param>
  /// <returns></returns>
  private static bool ConnectRectangleSDuct( Document doc, Duct duct0, Duct duct1, bool isFixedLength, double jointLength )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;

    //角ダクトS
    var familySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangleS ) ;
    if ( familySymbol is null ) return false ;

    using var tx = new Transaction( doc, "Rect Duct S-Joint Connection" ) ;
    tx.Start() ;

    try {
      FamilyInstance instance ;
      Element elem ;
      var loc = connector0.Origin ;
      var collector = new FilteredElementCollector( doc ) ;
      var level = collector.OfClass( typeof( Level ) ).FirstOrDefault( o => o.Id == duct0.Id ) as Level ;
      var dir0 = duct0.EndDirectionAtPoint( loc ) ;

      var isSuccess = false ;

      Connector? jCon0 = null ;
      Connector? jCon1 = null ;

      while ( ! isSuccess ) {
        instance = doc.Create.NewFamilyInstance( loc, familySymbol, level, StructuralType.NonStructural ) ;
        elem = instance as Element ;


        var v = connector1.Origin - connector0.Origin ;
        //var (length, swingRangeX,swingRangeZ) = v.DecomposeVector( dir0 ) ;
        var (length, swingRangeX, swingRangeY, swingRangeZ) = v.DecomposeVector2( dir0 ) ;

        var isVertical = swingRangeX.IsAlmostEqualToZero() ;

        // Console.WriteLine($"dir0.X > 0 :{dir0.X>0} length :{length.FtToMm().ToF1()} swingRangeX:{swingRangeX.FtToMm().ToF1()} {(swingRangeX + 0.5 * duct0.Width).FtToMm().ToF1()}, {(swingRangeX - 0.5 * duct0.Width).FtToMm().ToF1()}, {( - swingRangeX + 0.5 * duct0.Width).FtToMm().ToF1()},{(-swingRangeX - 0.5 * duct0.Width).FtToMm().ToF1()}");
        // Console.WriteLine($"dir0.Y > 0 :{dir0.Y>0} length :{length.FtToMm().ToF1()} swingRangeY:{swingRangeY.FtToMm().ToF1()} {(swingRangeY + 0.5 * duct0.Height).FtToMm().ToF1()}, {(swingRangeY - 0.5 * duct0.Height).FtToMm().ToF1()}, {( - swingRangeY + 0.5 * duct0.Height).FtToMm().ToF1()},{(-swingRangeY - 0.5 * duct0.Height).FtToMm().ToF1()}");
        // Console.WriteLine($"dir0.Z > 0 :{dir0.Z>0} length :{length.FtToMm().ToF1()} swingRangeZ:{swingRangeZ.FtToMm().ToF1()} {(swingRangeZ + 0.5 * duct0.Height).FtToMm().ToF1()}, {(swingRangeZ - 0.5 * duct0.Height).FtToMm().ToF1()}, {( - swingRangeZ + 0.5 * duct0.Height).FtToMm().ToF1()},{(-swingRangeZ - 0.5 * duct0.Height).FtToMm().ToF1()}");

        var currentDirection = XYZ.BasisX ;

        var rotAxis = currentDirection.CrossProduct( dir0 ).Normalize() ;

        if ( ! rotAxis.IsAlmostEqualTo( XYZ.Zero ) ) {
          // Console.WriteLine( "** notXDir" ) ;
          elem.LookupParameter( "ダクト幅" ).Set( duct0.Height ) ;
          elem.LookupParameter( "ダクト高" ).Set( duct0.Width ) ;
          elem.LookupParameter( "振れ幅設定" ).Set( Math.Abs( isVertical ? swingRangeZ : swingRangeX ) ) ;
          elem.LookupParameter( "長さ設定" ).Set( Math.Abs( length ) ) ;

          var angle = currentDirection.AngleTo( dir0 ) ;
          //Console.WriteLine($"angle:{angle.ToF2()} rotAxis{rotAxis.ToF2()} {isVertical}");
          ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + rotAxis ), angle ) ;

          var sw = new XYZ( swingRangeX, 0, swingRangeZ ) ;
          var swcross = sw.CrossProduct( dir0 ) ;
          // Console.WriteLine($"cross:{swcross.ToF2()}");

          if ( isVertical ) {
            // Console.WriteLine($"****Vert {dir0.ToF2()}");
            if ( swcross.X > 0 ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), 1 * Math.PI ) ;
            }
          }
          else {
            var isRotClockWise = v.CrossProduct( dir0 ).Z > 0 ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), ( isRotClockWise ? 0.5 : -0.5 ) * Math.PI ) ;
          }
        }
        else {
          // Console.WriteLine( "** XDir" ) ;
          // Console.WriteLine(dir0.IsAlmostEqualTo( XYZ.BasisX ));
          // Console.WriteLine(XYZ.BasisX.DotProduct( dir0 ));
          // Console.WriteLine((dir0.IsAlmostEqualTo( XYZ.BasisX ) ? XYZ.BasisX.DotProduct( dir0 ) : 0) + 1 );


          if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
            // Console.WriteLine($"X ***** {dir0.IsAlmostEqualTo( XYZ.BasisX )} {XYZ.BasisX.DotProduct( dir0 )}");
          }
          else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
            // Console.WriteLine($"-X ***** f:{dir0.IsAlmostEqualTo( XYZ.BasisX )} -1:{XYZ.BasisX.DotProduct( dir0 )}");
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + XYZ.BasisZ ), Math.PI ) ;
          }
          else {
            var angle = ( ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ? XYZ.BasisX.DotProduct( dir0 ) : 0 ) + 1 ) * Math.PI ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + XYZ.BasisZ ), angle ) ;
          }


          var sw = new XYZ( swingRangeX, 0, swingRangeZ ) ;
          var swcross = sw.CrossProduct( dir0 ) ;
          // Console.WriteLine($"cross:{swcross.ToF2()}");

          if ( isVertical ) {
            // Console.WriteLine($"****Vert {dir0.ToF2()} {Math.Abs( swingRangeY ).FtToMm()}");
            if ( dir0.X > 0 ) {
              //
            }

            elem.LookupParameter( "ダクト幅" ).Set( duct0.Width ) ;
            elem.LookupParameter( "ダクト高" ).Set( duct0.Height ) ;

            elem.LookupParameter( "振れ幅設定" ).Set( Math.Abs( swingRangeY ) ) ;
            elem.LookupParameter( "長さ設定" ).Set( Math.Abs( length ) ) ;

            //
            if ( ( dir0.IsAlmostEqualTo( XYZ.BasisX ) && swingRangeY > 0 ) || ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) && swingRangeY < 0 ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), Math.PI ) ;
            }
          }
          else {
            // Console.WriteLine($"_____");

            elem.LookupParameter( "ダクト幅" ).Set( duct0.Height ) ;
            elem.LookupParameter( "ダクト高" ).Set( duct0.Width ) ;

            elem.LookupParameter( "振れ幅設定" ).Set( Math.Abs( isVertical ? swingRangeZ : swingRangeX ) ) ;
            elem.LookupParameter( "長さ設定" ).Set( Math.Abs( length ) ) ;

            var isRotClockWise = v.CrossProduct( dir0 ).Z > 0 ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), ( isRotClockWise ? 0.5 : -0.5 ) * Math.PI ) ;
          }
        }

        var connectors = instance.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

        jCon0 = connectors.FirstOrDefault() ;
        jCon1 = connectors.Last() ;

        if ( jCon0 is null || jCon1 is null ) return false ;

        var jcz0 = jCon0.CoordinateSystem.BasisZ ;

        // Z座標向き一緒の場合、色々
        if ( connector0.CoordinateSystem.BasisZ.IsAlmostEqualTo( jcz0 ) ) {
          //接続失敗なのでやり直す
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          continue ;
        }

        connector0.ConnectTo( jCon0 ) ;
        connector1.ConnectTo( jCon1 ) ;

        //長さ再調整
        if ( isFixedLength ) {
          elem.LookupParameter( "長さ設定" )?.Set( Math.Abs( jointLength.MmToFt() ) ) ;
        }

        isSuccess = true ;
      }


      tx.Commit() ;
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
      tx.RollBack() ;
    }


    return true ;
  }


  /// <summary>
  /// ホッパー接続。
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct0"></param>
  /// <param name="duct1"></param>
  /// <returns></returns>
  public static bool ConnectHopperDuct( Document doc, Duct duct0, Duct duct1, bool isFixedLength, double jointLength )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;

    var familySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeJointRectangle ) ;
    if ( familySymbol is null ) return false ;

    using var tx = new Transaction( doc, "Rect Hopper Connection" ) ;
    tx.Start() ;
    try {
      var loc = connector0.Origin ;
      var level = duct0.ReferenceLevel ;
      loc -= XYZ.BasisZ * level.Elevation ;

      var dir0 = duct0.EndDirectionAtPoint( loc ) ;
      var isSuccess = false ;
      FamilyInstance instance ;

      Connector? jCon0 = null ;
      Connector? jCon1 = null ;

      ( connector0, connector1 ) = NearestConnectors( duct0, duct1 ) ;
      if ( connector0 is null || connector1 is null ) return false ;
      loc = connector0.Origin ;
      level = duct0.ReferenceLevel ;
      loc -= XYZ.BasisZ * level.Elevation ;
      dir0 = duct0.EndDirectionAtPoint( loc ) ;

      var v = connector1.Origin - connector0.Origin ;
      var (length, swingRangeX, swingRangeZ) = v.DecomposeVector( dir0 ) ;

      var dx = length > 0 ? swingRangeX + 0.5 * duct0.Width : -swingRangeX + 0.5 * duct0.Width ;
      var dy = dir0.Y > 0 ? swingRangeZ + 0.5 * duct0.Height : -swingRangeZ + 0.5 * duct0.Height ;

      // var tryCount = 0 ;
      while ( ! isSuccess ) {
        // Console.WriteLine( $"{tryCount++} -------------" ) ;

        instance = doc.Create.NewFamilyInstance( loc, familySymbol, level, StructuralType.NonStructural ) ;
        if ( instance is null ) return false ;
        var elem = instance as Element ;

        // Console.WriteLine($"dir0.X > 0 :{dir0.X>0} length :{length.FtToMm().ToF1()} swingRangeX:{swingRangeX.FtToMm().ToF1()} {(swingRangeX + 0.5 * duct0.Width).FtToMm().ToF1()}, {(swingRangeX - 0.5 * duct0.Width).FtToMm().ToF1()}, {( - swingRangeX + 0.5 * duct0.Width).FtToMm().ToF1()},{(-swingRangeX - 0.5 * duct0.Width).FtToMm().ToF1()}");
        // Console.WriteLine($"dir0.Y > 0 :{dir0.Y>0} length2 :{length.FtToMm().ToF1()} swingRangeZ:{swingRangeZ.FtToMm().ToF1()} {(swingRangeZ + 0.5 * duct0.Height).FtToMm().ToF1()}, {(swingRangeZ - 0.5 * duct0.Height).FtToMm().ToF1()}, {( - swingRangeZ + 0.5 * duct0.Height).FtToMm().ToF1()},{(-swingRangeZ - 0.5 * duct0.Height).FtToMm().ToF1()}");

        elem.LookupParameter( "オフセット幅" ).Set( dx ) ;
        elem.LookupParameter( "オフセット高さ" ).Set( dy ) ;
        elem.LookupParameter( "ダクト長さ" ).Set( Math.Abs( length ) ) ;

        var currentDirection = XYZ.BasisX ;

        if ( length < 0 ) {
          Console.WriteLine( $"{instance?.MEPModel.ConnectorManager.Connectors.Size}" ) ;
          Console.WriteLine( $"{instance?.Id} : {connector0.Origin.ToF2()} " ) ;
          ElementTransformUtils.RotateElement( doc, instance?.Id, Line.CreateBound( connector0.Origin, connector0.Origin + XYZ.BasisZ ), Math.PI ) ;
          Console.WriteLine( $"{instance?.MEPModel.ConnectorManager.Connectors.Size}" ) ;
          Console.WriteLine( "Connector may be lost in some cases" ) ;
        }

        var rotAxis = currentDirection.CrossProduct( dir0 ) ;
        if ( ! rotAxis.IsAlmostEqualTo( XYZ.Zero ) ) {
          rotAxis = rotAxis.Normalize() ;
          var angle = currentDirection.AngleTo( dir0 ) ;
          ElementTransformUtils.RotateElement( doc, instance?.Id, Line.CreateBound( loc, loc + rotAxis ), angle ) ;
        }
        else {
          var angle = ( ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ? XYZ.BasisX.DotProduct( dir0 ) : 0 ) + 1 ) * Math.PI ;
          ElementTransformUtils.RotateElement( doc, instance?.Id, Line.CreateBound( loc, loc + XYZ.BasisZ ), angle ) ;
        }

        var connectors = instance?.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

        //回転でコネクタが消える場合。高砂ツールでも動作しない。要確認。
        if ( connectors.Count() == 0 ) {
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          tx.RollBack() ;
          return false ;
        }

        jCon0 = connectors.FirstOrDefault() ;
        jCon1 = connectors.Last() ;

        if ( jCon0.Origin.DistanceTo( connector0.Origin ) > jCon0.Origin.DistanceTo( connector1.Origin ) ) ( jCon0, jCon1 ) = ( jCon1, jCon0 ) ;

        if ( jCon0 is null || jCon1 is null ) return false ;

        jCon0.Width = duct0.Width ;
        jCon0.Height = duct0.Height ;
        jCon1.Width = duct1.Width ;
        jCon1.Height = duct1.Height ;

        var jcz0 = jCon0.CoordinateSystem.BasisZ ;
        var jcz1 = jCon1.CoordinateSystem.BasisZ ;

        // Z座標向き一緒の場合、色々
        if ( connector0.CoordinateSystem.BasisZ.IsAlmostEqualTo( jcz0 ) ) {
          //接続失敗なのでやり直す
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          continue ;
        }

        if ( connector1.CoordinateSystem.BasisZ.IsAlmostEqualTo( jcz1 ) ) {
          //接続失敗なのでやり直す
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          continue ;
        }

        //コネクタ接続
        connector0.ConnectTo( jCon0 ) ;
        connector1.ConnectTo( jCon1 ) ;

        //長さ再調整
        if ( isFixedLength ) {
          elem.LookupParameter( "ダクト長さ" ).Set( Math.Abs( jointLength.MmToFt() ) ) ;
        }

        isSuccess = true ;
      }

      tx.Commit() ;
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
      tx.RollBack() ;
    }

    return true ;
  }

  #endregion

  // ==============================================================

  #region ConnectOrthogonalRectDucts (Elbow)

  /// <summary>
  /// 角エルボ接続
  /// </summary>
  /// <param name="uiApp"></param>
  /// <param name="familyName"></param>
  /// <param name="innnerRadius"></param>
  /// <param name="hopperLength"></param>
  public static void BeginConnectElbow( this UIApplication uiApp, string familyName, double innnerRadius, double hopperLength )
  {
    var uiDoc = uiApp.ActiveUIDocument ;
    var doc = uiDoc.Document ;

    try {
      while ( true ) {
        var (duct0, duct1) = uiDoc.PickDuctPair() ;
        if ( duct0 is null || duct1 is null ) return ;

        var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
        if ( connector0 is null || connector1 is null ) return ;

        var dir0 = duct0.EndDirectionAtPoint( connector0.Origin ) ;
        var dir1 = duct1.EndDirectionAtPoint( connector1.Origin ) ;


        //直交する場合dotProductが0
        if ( ! dir0.DotProduct( dir1 ).IsAlmostEqualToZero() ) return ;

        //Console.WriteLine( $"直交" ) ;
        ConnectOrthogonalRectDuct( doc, duct0, duct1, familyName, innnerRadius, hopperLength ) ;
      }
    }
    catch ( Exception e ) {
      if ( e is OperationCanceledException ) return ; //Escで抜けた場合
      Console.WriteLine( e ) ;
    }
  }

  /// <summary>
  /// 直交するダクトの接続
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct0"></param>
  /// <param name="duct1"></param>
  /// <param name="familyName"></param>
  /// <param name="innnerRadius"></param>
  /// <param name="hopperLength"></param>
  /// <returns></returns>
  private static bool ConnectOrthogonalRectDuct( Document doc, Duct duct0, Duct duct1, string familyName, double innnerRadius, double hopperLength )
  {
    var sizeStr0 = duct0.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;
    var sizeStr1 = duct1.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;
    var isUseHopper = sizeStr0 != sizeStr1 ;

    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;

    //座標計算
    var dir0 = duct0.EndDirectionAtPoint( connector0.Origin ) ;
    var dir1 = duct1.EndDirectionAtPoint( connector1.Origin ) ;

    var pt0 = connector0.Origin ;
    var pt1 = connector1.Origin ;
    var p0_p1 = pt1 - pt0 ;
    var crossDir0Dir1 = dir0.CrossProduct( dir1 ) ;
    var t = p0_p1.CrossProduct( dir1 ).DotProduct( crossDir0Dir1 ) / crossDir0Dir1.GetLength() ;
    var intersection = pt0 + t * dir0 ;

    var isV = dir1.IsAlmostEqualTo( -XYZ.BasisZ ) || dir1.IsAlmostEqualTo( XYZ.BasisZ ) || ( dir0.IsAlmostEqualTo( -XYZ.BasisZ ) && ( ! Math.Abs( XYZ.BasisX.DotProduct( dir1 ) - 1 ).IsAlmostEqualToZero() ) ) ;

    var ductW = isV ? duct0.Height : duct0.Width ;
    var ductH = isV ? duct1.Height : duct1.Width ;

    var end0 = intersection - dir0 * ductW ;
    var end1 = intersection - dir1 * ( ductH + ( isUseHopper ? hopperLength.MmToFt() : 0 ) ) ;

    if ( innnerRadius.IsAlmostEqualToZero() ) {
      //内Rが0の場合
      end0 = intersection - dir0 * ductW ;
      end1 = intersection - dir1 * ( ductW + ( isUseHopper ? hopperLength.MmToFt() : 0 ) ) ;
    }
    else {
      if ( hopperLength.IsAlmostEqualToZero() ) {
        // 異形
        var w = Math.Max( ductW, ductH ) ;

        end0 = intersection - dir0 * w ;
        end1 = intersection - dir1 * w ;
      }
      else {
        //内R設定があるとき、交点からの距離は
        end0 = intersection - dir0 * ( innnerRadius.MmToFt() + 0.5 * ductW ) ;
        end1 = intersection - dir1 * ( innnerRadius.MmToFt() + 0.5 * ductW + ( isUseHopper ? hopperLength.MmToFt() : 0 ) ) ;
      }
    }


    Console.WriteLine( $"{pt0.ToF2()}:{dir0.ToF2()} {pt1.ToF2()}:{dir1.ToF2()} {intersection.ToF2()}" ) ;
    // Console.WriteLine( $"{end0.ToF2()} {end1.ToF2()}" ) ;

    //サイズが一緒なら普通にエルボだけでつなぐ。
    //サイズが違う場合はホッパーを挟む。

    var familySymbol = doc.RectDuctFamilySymbol( familyName ) ;
    if ( familySymbol is null ) return false ;

    using var tx = new Transaction( doc, "Rect Duct Elbow Connection" ) ;
    tx.Start() ;

    try {
      //ファミリ配置
      FamilyInstance instance ;
      Element elem ;
      Connector? jCon0 = null ;
      Connector? jCon1 = null ;
      var isSuccess = false ;

      var collector = new FilteredElementCollector( doc ) ;
      var level = collector.OfClass( typeof( Level ) ).FirstOrDefault( o => o.Id == duct0.Id ) as Level ;


      // ダクト端部移動

      //始端側端点移動
      var curve0 = duct0.Location as LocationCurve ;
      var ep0_0 = curve0.Curve.GetEndPoint( 0 ) ;
      var ep0_1 = curve0.Curve.GetEndPoint( 1 ) ;
      if ( pt0.IsAlmostEqualTo( ep0_0 ) ) ep0_0 = end0 ;
      if ( pt0.IsAlmostEqualTo( ep0_1 ) ) ep0_1 = end0 ;
      curve0.Curve = Line.CreateBound( ep0_0, ep0_1 ) ;

      //終端側端点移動
      var curve1 = duct1.Location as LocationCurve ;
      var ep1_0 = curve1.Curve.GetEndPoint( 0 ) ;
      var ep1_1 = curve1.Curve.GetEndPoint( 1 ) ;
      if ( pt1.IsAlmostEqualTo( ep1_0 ) ) ep1_0 = end1 ;
      if ( pt1.IsAlmostEqualTo( ep1_1 ) ) ep1_1 = end1 ;
      curve1.Curve = Line.CreateBound( ep1_0, ep1_1 ) ;

      var isWHparam = true ;
      var isWHparam2 = true ;

      while ( ! isSuccess ) {
        instance = doc.Create.NewFamilyInstance( intersection, familySymbol, level, StructuralType.NonStructural ) ;
        elem = instance as Element ;

        var currentDirection0 = XYZ.BasisX ;
        var rotAxis0 = currentDirection0.CrossProduct( dir0 ).Normalize() ;
        var angle0 = currentDirection0.AngleTo( dir0 ) ;

        Console.WriteLine( $"CrossProduct: {dir0.CrossProduct( p0_p1 ).ToF2()}  rotAxis0:{rotAxis0.ToF2()}" ) ;


        //縦
        if ( dir1.IsAlmostEqualTo( -XYZ.BasisZ ) || dir1.IsAlmostEqualTo( XYZ.BasisZ ) ) {
          isWHparam = false ;

          if ( dir1.IsAlmostEqualTo( XYZ.BasisZ ) ) {
            if ( dir0.IsAlmostEqualTo( -XYZ.BasisY ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), -0.5 * Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( XYZ.BasisY ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), 0.5 * Math.PI ) ;
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisZ ), Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), 0.5 * Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), -0.5 * Math.PI ) ;
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisZ ), Math.PI ) ;
            }
          }

          else if ( dir1.IsAlmostEqualTo( -XYZ.BasisZ ) ) {
            if ( dir0.IsAlmostEqualTo( -XYZ.BasisY ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), 0.5 * Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( XYZ.BasisY ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), -0.5 * Math.PI ) ;
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisZ ), Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), -0.5 * Math.PI ) ;
            }
            else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection - dir0 ), 0.5 * Math.PI ) ;
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisZ ), Math.PI ) ;
            }
          }
        }
        else {
          if ( dir0.IsAlmostEqualTo( XYZ.BasisZ ) && ( Math.Abs( XYZ.BasisX.DotProduct( dir1 ) ) - 1 ).IsAlmostEqualToZero() ) {
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisY ), -0.5 * Math.PI ) ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), dir1.X > 0 ? 0.5 * Math.PI : -0.5 * Math.PI ) ;
            isWHparam2 = false ;
          }
          else if ( dir0.IsAlmostEqualTo( -XYZ.BasisZ ) ) {
            Console.WriteLine( $"{XYZ.BasisX.DotProduct( dir1 )}" ) ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisY ), 0.5 * Math.PI ) ;
            if ( ( Math.Abs( XYZ.BasisX.DotProduct( dir1 ) ) - 1 ).IsAlmostEqualToZero() ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), dir1.X > 0 ? -0.5 * Math.PI : 0.5 * Math.PI ) ;
              isWHparam2 = false ;
            }
            else {
              Console.WriteLine( $"{XYZ.BasisX.DotProduct( dir1 )}" ) ;
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), dir1.Y > 0 ? Math.PI : 0 ) ;
              isWHparam = false ;
            }
          }
          else if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
            //補正
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), Math.PI ) ;
            if ( dir0.CrossProduct( p0_p1 ).Z > 0 ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), Math.PI ) ;
            }
          }
          else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
            //補正
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + XYZ.BasisZ ), Math.PI ) ;
            if ( dir0.CrossProduct( p0_p1 ).Z < 0 ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), Math.PI ) ;
            }
          }
          else {
            if ( dir0.IsAlmostEqualTo( XYZ.BasisZ ) ) {
              isWHparam = false ;
            }

            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + rotAxis0 ), angle0 ) ;

            if ( dir0.CrossProduct( p0_p1 ).Z < 0 ) {
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( intersection, intersection + dir0 ), Math.PI ) ;
            }
          }
        }

        if ( isWHparam ) {
          elem.LookupParameter( "ダクト幅" )?.Set( duct0.Width ) ;
          elem.LookupParameter( "ダクト高" )?.Set( duct0.Height ) ;
          elem.LookupParameter( "ダクト幅1" )?.Set( duct0.Width ) ;
          elem.LookupParameter( "ダクト高1" )?.Set( duct0.Height ) ;

          if ( isWHparam2 ) {
            elem.LookupParameter( "ダクト幅2" )?.Set( duct1.Width ) ;
            elem.LookupParameter( "ダクト高2" )?.Set( duct1.Height ) ;
          }
          else {
            elem.LookupParameter( "ダクト幅2" )?.Set( duct1.Height ) ;
            elem.LookupParameter( "ダクト高2" )?.Set( duct1.Width ) ;
          }
        }
        else {
          elem.LookupParameter( "ダクト幅" )?.Set( duct0.Height ) ;
          elem.LookupParameter( "ダクト高" )?.Set( duct0.Width ) ;
          elem.LookupParameter( "ダクト幅1" )?.Set( duct0.Height ) ;
          elem.LookupParameter( "ダクト高1" )?.Set( duct0.Width ) ;

          if ( isWHparam2 ) {
            elem.LookupParameter( "ダクト幅2" )?.Set( duct1.Height ) ;
            elem.LookupParameter( "ダクト高2" )?.Set( duct1.Width ) ;
          }
          else {
            elem.LookupParameter( "ダクト幅2" )?.Set( duct1.Width ) ;
            elem.LookupParameter( "ダクト高2" )?.Set( duct1.Height ) ;
          }
        }


        elem.LookupParameter( "角度" )?.Set( Math.PI / 2 ) ;

        if ( innnerRadius > 0 ) elem.LookupParameter( "内R設定" )?.Set( innnerRadius.MmToFt() ) ;


        var connectors = instance.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

        jCon0 = connectors.FirstOrDefault() ;
        jCon1 = connectors.Last() ;
        if ( jCon0 is null || jCon1 is null ) return false ;


        // Z座標向きが逆でない場合接続失敗
        // Console.WriteLine($"X_{connector0.CoordinateSystem.BasisX.ToF2()}:{jCon0.CoordinateSystem.BasisX.ToF2()} Y_{connector0.CoordinateSystem.BasisY.ToF2()}:{jCon0.CoordinateSystem.BasisY.ToF2()} Z_{connector0.CoordinateSystem.BasisZ.ToF2()}:{jCon0.CoordinateSystem.BasisZ.ToF2()}");
        if ( ! connector0.CoordinateSystem.BasisZ.IsAlmostEqualTo( -jCon0.CoordinateSystem.BasisZ ) ) {
          //接続失敗なのでやり直す
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          continue ;
        }

        connector0.ConnectTo( jCon0 ) ;
        connector1.ConnectTo( jCon1 ) ;

        isSuccess = true ;
      }
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
    }

    tx.Commit() ;

    return true ;
  }

  #endregion

  #region ConnectParallelRoundDuct

  public static Result BeginConnectParallelRoundDuct( this UIApplication uiApp, bool isUse45degElbow )
  {
    try {
      var uiDoc = uiApp.ActiveUIDocument ;
      var doc = uiDoc.Document ;

      while ( true ) {
        var (duct0, duct1) = uiDoc.PickDuctPair() ;
        if ( duct0 is null || duct1 is null ) return Result.Cancelled ;
        //if ( duct0 == duct1 ) return Result.Cancelled ;

        var sizeStr0 = duct0.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;
        var sizeStr1 = duct1.get_Parameter( BuiltInParameter.RBS_REFERENCE_OVERALLSIZE )?.AsValueString() ;

        //近傍コネクタの検出
        var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
        if ( connector0 is null || connector1 is null ) return Result.Cancelled ;

        var dir0 = duct0.EndDirectionAtPoint( connector0.Origin ) ;
        var dir1 = duct1.EndDirectionAtPoint( connector1.Origin ) ;
        var crossProduct = dir0.CrossProduct( connector1.Origin - connector0.Origin ) ;
        var areCollinear = crossProduct.IsZeroLength() ;
        var areCoplanar = crossProduct.DotProduct( dir1 ).IsAlmostEqualToZero() ;
        var areSameSize = sizeStr0 == sizeStr1 ;
        var areCoplanarZ = XYZ.BasisZ.DotProduct( dir0 ).IsAlmostEqualToZero() && XYZ.BasisZ.DotProduct( dir1 ).IsAlmostEqualToZero() ;

        //平行の場合crossProductが0ベクトル
        // Console.WriteLine( $"平行:{dir0.CrossProduct( dir1 ).IsZeroLength()} 逆向き:{( dir0 + dir1 ).IsZeroLength()}" ) ;
        if ( ! dir0.CrossProduct( dir1 ).IsZeroLength() || ! ( dir0 + dir1 ).IsZeroLength() ) return Result.Cancelled ;

        var result = ConnectRoundDuct( doc, duct0, duct1, isUse45degElbow ) ? Result.Succeeded : Result.Cancelled ;
        // return result ;
      }
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
    }

    return Result.Succeeded ;
  }

  private static bool ConnectRoundDuct( Document doc, Duct duct0, Duct duct1, bool isUse45degElbow )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;
    var jointFamilySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeJointRoundElbow ) ;
    if ( jointFamilySymbol is null ) return false ;


    using var tx = new Transaction( doc, "Round Duct Connection" ) ;
    tx.Start() ;

    try {
      // FamilyInstance instance0 ;
      // FamilyInstance instance1 ;
      // Element elem0 ;
      // Element elem1 ;
      var level0 = duct0.ReferenceLevel ;
      var level1 = duct1.ReferenceLevel ;
      var dir0 = duct0.EndDirectionAtPoint( connector0.Origin - XYZ.BasisZ * level0.Elevation ) ;
      var dir1 = duct1.EndDirectionAtPoint( connector1.Origin - XYZ.BasisZ * level1.Elevation ) ;

      var diameter0 = duct0.Diameter ;
      // var diameter1 = duct0.Diameter ;

      CalcDirections( connector0.Origin, dir0, connector1.Origin, dir1 ) ;

      var isSuccess = false ;

      Connector? jCon0 = null ;
      Connector? jCon1 = null ;

      var v = connector1.Origin - connector0.Origin ;
      // var (length, swingRangeX, swingRangeZ) = v.DecomposeVector( dir0 ) ;
      var (length, swingRangeX, swingRangeY, swingRangeZ) = v.DecomposeVector2( dir0 ) ;

      var isVertical = swingRangeX.IsAlmostEqualToZero() ;


      // var dir0R = swingRangeX<0? dir0.CrossProduct( XYZ.BasisZ ) : -dir0.CrossProduct( XYZ.BasisZ ) ;
      // var jointType = RoundJointType.JointS ;

      // Console.WriteLine( $"dir0.Z > 0 :{dir0.Z > 0} length :{length.FtToMm().ToF1()} swingRangeX:{swingRangeX.FtToMm().ToF1()} swingRangeZ:{swingRangeZ.FtToMm().ToF1()} " ) ;

      // Math.Abs( swingRangeX )の値が diameter0*2 + 60.mmToFt以上ならば90°エルボ、diameter0/Math.Sqrt( 2 )までは45°エルボ、それ以下はS管

      var srX = Math.Abs( swingRangeX ) ;
      var srY = Math.Abs( swingRangeY ) ;
      var srZ = Math.Abs( swingRangeZ ) ;

      //var absSwingRange =  Math.Abs( swingRangeX );

      var absSwingRange = Math.Max( srX, Math.Max( srY, srZ ) ) ;

      if ( absSwingRange > diameter0 * 2 + 60.0.MmToFt() && ! isUse45degElbow ) {
        ConnectParallelRoundDuctWith90degJoint( doc, duct0, duct1 ) ;
      }
      else if ( absSwingRange > diameter0 / Math.Sqrt( 2 ) ) {
        ConnectParallelRoundDuctWith45degJoint( doc, duct0, duct1 ) ;
      }
      else {
        ConnectParallelRoundDuctWithSJoint( doc, duct0, duct1 ) ;
      }


      tx.Commit() ;
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
      tx.RollBack() ;
    }

    return true ;
  }


  /// <summary>
  /// 90°ジョイントでの接続
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct0"></param>
  /// <param name="duct1"></param>
  /// <returns></returns>
  private static bool ConnectParallelRoundDuctWith90degJoint( Document doc, Duct duct0, Duct duct1 )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;
    var jointFamilySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeJointRoundElbow ) ;
    if ( jointFamilySymbol is null ) return false ;

    FamilyInstance instance0 ;
    FamilyInstance instance1 ;
    Element elem0 ;
    Element elem1 ;
    var level0 = duct0.ReferenceLevel ;
    var level1 = duct1.ReferenceLevel ;
    var dir0 = duct0.EndDirectionAtPoint( connector0.Origin - XYZ.BasisZ * level0.Elevation ) ;
    // var dir1 = duct1.EndDirectionAtPoint( connector1.Origin - XYZ.BasisZ * level1.Elevation ) ;

    var diameter0 = duct0.Diameter ;
    // var diameter1 = duct0.Diameter ;


    var isSuccess = false ;

    Connector? jCon0 = null ;
    Connector? jCon1 = null ;

    var v = connector1.Origin - connector0.Origin ;
    // var (length, swingRangeX, swingRangeZ) = v.DecomposeVector( dir0 ) ;
    var (length, swingRangeX, swingRangeY, swingRangeZ) = v.DecomposeVector2( dir0 ) ;
    var isVertical = swingRangeX.IsAlmostEqualToZero() ;


    // swingRange のどれが小さいか
    var srX = Math.Abs( swingRangeX ) ;
    var srY = Math.Abs( swingRangeY ) ;
    var srZ = Math.Abs( swingRangeZ ) ;

    double sr = 0 ;
    var dir0R = XYZ.Zero ;
    var refPlane = RefPlane.Xy ;
    if ( srX > srY && srX > srZ ) {
      //平面図 rotAxis - XYZ.BasisZ
      refPlane = RefPlane.Xy ;
      dir0R = swingRangeX < 0 ? dir0.CrossProduct( XYZ.BasisZ ) : -dir0.CrossProduct( XYZ.BasisZ ) ;
      sr = srX ;
    }
    else if ( srY > srX && srY > srZ ) {
      //立面図、Y rotAxis - XYZ.BasisX
      refPlane = RefPlane.Xz ;
      dir0R = swingRangeY < 0 ? dir0.CrossProduct( XYZ.BasisY ) : -dir0.CrossProduct( XYZ.BasisY ) ;
      sr = srY ;
    }
    else {
      //立面図Z rotAxis  - XYZ.BasisY
      refPlane = RefPlane.Yz ;
      dir0R = swingRangeZ < 0 ? dir0.CrossProduct( XYZ.BasisX ) : -dir0.CrossProduct( XYZ.BasisX ) ;
      sr = srZ ;
    }

    var loc0 = connector0.Origin + dir0 * diameter0 ;
    var loc1 = loc0 + dir0R * sr ;

    var currentDirection = -XYZ.BasisY ;
    var rotAxis = currentDirection.CrossProduct( dir0 ).Normalize() ;
    var angle0 = currentDirection.AngleTo( dir0 ) ;
    var angleB = ( rotAxis.X + rotAxis.Y + rotAxis.Z ) > 0 ? angle0 : -angle0 ;


    // Console.WriteLine( $"rotAxis:{rotAxis.ToF2()} angle0:{angle0.ToF2()} angle1:{angle1.ToF2()}" ) ;

    while ( ! isSuccess ) {
      instance0 = doc.Create.NewFamilyInstance( loc0, jointFamilySymbol, level0, StructuralType.NonStructural ) ;
      instance1 = doc.Create.NewFamilyInstance( loc1, jointFamilySymbol, level1, StructuralType.NonStructural ) ;
      elem0 = instance0 as Element ;
      elem1 = instance1 as Element ;


      if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
        if ( swingRangeX < 0 ) {
          Console.WriteLine( "== Case.0" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), -0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - dir0 ), Math.PI ) ;
        }
        else {
          Console.WriteLine( "== Case.1" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), -0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
        }
      }
      else if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
        if ( swingRangeX < 0 ) {
          Console.WriteLine( "== Case.0.--" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), -0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - dir0 ), Math.PI ) ;
        }
        else {
          Console.WriteLine( "== Case.1.--" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), -0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - rotAxis ), Math.PI ) ;
        }
      }
      else if ( swingRangeX < 0 ) {
        if ( rotAxis.Z > 0 ) {
          Console.WriteLine( "== Case.2.0" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + rotAxis ), angle0 ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + rotAxis ), angleB + 0.5 * Math.PI ) ;
        }
        else {
          Console.WriteLine( "== Case.2.1" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), -angle0 - 0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - rotAxis ), angleB + 0.5 * Math.PI ) ;
        }

        ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - dir0 ), Math.PI ) ;
      }
      else {
        Console.WriteLine( $"== Case.3.0 {refPlane} {sr} {rotAxis.ToF2()} {dir0.ToF2()}" ) ;

        switch ( refPlane ) {
          case RefPlane.Xy :
            ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), angleB ) ;
            ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
            ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - rotAxis ), angleB + 0.5 * Math.PI ) ;
            break ;
          case RefPlane.Xz :
            ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), angleB ) ;
            ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + dir0 ), Math.PI ) ;
            ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - rotAxis ), angleB ) ;
            break ;
          default :
            ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 - rotAxis ), angleB ) ;
            ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + dir0 ), Math.PI ) ;
            ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - rotAxis ), angleB ) ;
            break ;
        }
      }


      var angleValue = 0.5 * Math.PI ;

      elem0.LookupParameter( "ダクト半径" ).Set( 0.5 * diameter0 ) ;
      elem1.LookupParameter( "ダクト半径" ).Set( 0.5 * diameter0 ) ;
      elem0.LookupParameter( "角度" ).Set( angleValue ) ;
      elem1.LookupParameter( "角度" ).Set( angleValue ) ;

      var connectors0 = instance0?.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
      var con0_0 = connectors0.FirstOrDefault() ;
      var connectors1 = instance1?.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
      var con1_0 = connectors1.FirstOrDefault() ;


      var isCon0Fail = ! angle0.IsAlmostEqualToZero() && con0_0?.CoordinateSystem.BasisX.IsAlmostEqualTo( dir0 ) == true ;
      var isCon1Fail = Math.Sin( con0_0?.CoordinateSystem.BasisX.AngleTo( con1_0?.CoordinateSystem.BasisX ) ?? 0 ).IsAlmostEqualToZero() ;
      if ( isCon0Fail || isCon1Fail ) {
        //接続失敗なのでやり直す
        doc.Delete( new List<ElementId> { elem0.Id, elem1.Id } ) ;
        continue ;
      }

      connector0.ConnectTo( con0_0 ) ;
      // doc.ExtendDuct( duct1, connector1, instance1 );
      doc.ExtendMEPCurve( duct1, connector1, instance1 ) ;

      // 間の部分
      var pt0 = connector0.Origin + dir0 * diameter0 + dir0R * diameter0 ;
      var pt1 = pt0 + dir0R * ( Math.Abs( swingRangeX ) - diameter0 * 2 ) ;
      var conA = connectors0.LastOrDefault( x => x.Origin.IsAlmostEqualTo( pt0 ) ) ;
      var conB = connectors1.LastOrDefault( x => x.Origin.IsAlmostEqualTo( pt1 ) ) ;
      if ( conA is null || conB is null ) return false ;
      var duct2 = Duct.Create( doc, duct0.DuctType.Id, level0.Id, conA, conB ) ;
      var elem2 = duct2 as Element ;
      elem2.LookupParameter( "直径" ).Set( diameter0 ) ;
      doc.Regenerate() ;


      isSuccess = true ;
    }

    return true ;
  }

  /// <summary>
  /// 45°ジョイントでの接続
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct0"></param>
  /// <param name="duct1"></param>
  /// <returns></returns>
  private static bool ConnectParallelRoundDuctWith45degJoint( Document doc, Duct duct0, Duct duct1 )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;
    var jointFamilySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeJointRoundElbow ) ;
    if ( jointFamilySymbol is null ) return false ;

    FamilyInstance instance0 ;
    FamilyInstance instance1 ;
    Element elem0 ;
    Element elem1 ;
    var level0 = duct0.ReferenceLevel ;
    var level1 = duct1.ReferenceLevel ;
    var dir0 = duct0.EndDirectionAtPoint( connector0.Origin - XYZ.BasisZ * level0.Elevation ) ;
    var dir1 = duct1.EndDirectionAtPoint( connector1.Origin - XYZ.BasisZ * level1.Elevation ) ;

    var diameter0 = duct0.Diameter ;
    // var diameter1 = duct0.Diameter ;

    var isSuccess = false ;

    Connector? jCon0 = null ;
    Connector? jCon1 = null ;

    var v = connector1.Origin - connector0.Origin ;
    var (length, swingRangeX, swingRangeZ) = v.DecomposeVector( dir0 ) ;

    var dir0R = swingRangeX < 0 ? dir0.CrossProduct( XYZ.BasisZ ) : -dir0.CrossProduct( XYZ.BasisZ ) ;
    var angleValue = 0.25 * Math.PI ;

    //45°の場合
    var loc0 = connector0.Origin + 0.41421356 * dir0 * diameter0 ;

    //con0の端点
    var end0 = connector0.Origin + 0.707107 * dir0 * diameter0 + 0.292893 * dir0R ;
    var endDir0 = swingRangeX < 0 ? dir0.RotateAroundAxis( XYZ.BasisZ, -angleValue ).Normalize() : dir0.RotateAroundAxis( -XYZ.BasisZ, -angleValue ).Normalize() ;

    // Console.WriteLine( $"dir0.Z > 0 :{dir0.Z > 0} length :{length.FtToMm().ToF1()} swingRangeX:{swingRangeX.FtToMm().ToF1()} swingRangeZ:{swingRangeZ.FtToMm().ToF1()} " ) ;

    var loc1 = GetIntersection( end0, endDir0, connector1.Origin, dir1 ) ;

    // Console.WriteLine($"loc0:{loc0.ToF2()} loc1:{loc1.ToF2()} end0:{end0.ToF2()} endDir0:{endDir0.ToF2()} con0:{connector0.Origin.ToF2()}  con1:{connector1.Origin.ToF2()}");

    var currentDirection = -XYZ.BasisY ;
    var rotAxis = currentDirection.CrossProduct( dir0 ).Normalize() ;
    var angle0 = currentDirection.AngleTo( dir0 ) ;

    // Console.WriteLine( $"rotAxis:{rotAxis.ToF2()} angle0:{angle0.ToF2()} angle1:{angle1.ToF2()}" ) ;

    while ( ! isSuccess ) {
      instance0 = doc.Create.NewFamilyInstance( loc0, jointFamilySymbol, level0, StructuralType.NonStructural ) ;
      instance1 = doc.Create.NewFamilyInstance( loc1, jointFamilySymbol, level1, StructuralType.NonStructural ) ;
      elem0 = instance0 as Element ;
      elem1 = instance1 as Element ;

      elem0.LookupParameter( "ダクト半径" ).Set( 0.5 * diameter0 ) ;
      elem1.LookupParameter( "ダクト半径" ).Set( 0.5 * diameter0 ) ;
      elem0.LookupParameter( "角度" ).Set( angleValue ) ;
      elem1.LookupParameter( "角度" ).Set( angleValue ) ;

      var angleB = rotAxis.Z > 0 ? angle0 : -angle0 ;

      if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
        if ( swingRangeX < 0 ) {
          // Console.WriteLine( "== Case.0" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + XYZ.BasisZ ), -0.25 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - dir0 ), Math.PI ) ;
        }
        else {
          // Console.WriteLine( "== Case.1" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + XYZ.BasisZ ), angleB - 0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + XYZ.BasisZ ), angleB + 0.5 * Math.PI ) ;
        }
      }
      else if ( swingRangeX < 0 ) {
        if ( rotAxis.Z > 0 ) {
          // Console.WriteLine( "== Case.2.0" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + rotAxis ), angle0 + 0.25 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + rotAxis ), angleB + 0.5 * Math.PI ) ;
        }
        else {
          // Console.WriteLine( "== Case.2.1" ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + XYZ.BasisZ ), -angle0 - 0.5 * Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
          ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + XYZ.BasisZ ), angleB + 0.5 * Math.PI ) ;
        }

        ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 - dir0 ), Math.PI ) ;
      }
      else {
        // Console.WriteLine( "== Case.3.0" ) ;
        ElementTransformUtils.RotateElement( doc, instance0?.Id, Line.CreateBound( loc0, loc0 + XYZ.BasisZ ), angleB - 0.5 * Math.PI ) ;
        ElementTransformUtils.RotateElement( doc, instance1?.Id, Line.CreateBound( loc1, loc1 + XYZ.BasisZ ), angleB + 0.5 * Math.PI ) ;
      }


      var connectors0 = instance0?.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
      var con0_0 = connectors0.FirstOrDefault() ;
      // var con0_1 = connectors0.Last() ;
      var connectors1 = instance1?.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
      var con1_0 = connectors1.FirstOrDefault() ;
      // var con1_1 = connectors1.Last() ;

      if ( con0_0 is null || con1_0 is null ) {
        Console.WriteLine( "con0_0 is null || con1_0 is null" ) ;
        return false ;
      }

      var isCon0Fail = ! angle0.IsAlmostEqualToZero() && con0_0?.CoordinateSystem.BasisX.IsAlmostEqualTo( dir0 ) == true ;
      var isCon1Fail = Math.Sin( con0_0?.CoordinateSystem.BasisX.AngleTo( con1_0?.CoordinateSystem.BasisX ) ?? 0 ).IsAlmostEqualToZero() ;
      if ( isCon0Fail || isCon1Fail ) {
        //接続失敗なのでやり直す
        doc.Delete( new List<ElementId> { elem0.Id, elem1.Id } ) ;
        continue ;
      }

      // doc.Regenerate() ;

      connector0.ConnectTo( con0_0 ) ;

      //伸ばす側のダクト生成
      // doc.ExtendDuct( duct1, connector1, instance1 ) ;
      doc.ExtendMEPCurve( duct1, connector1, instance1 ) ;


      // 間の部分
      var conA = connectors0.OrderBy( x => x.Origin.DistanceTo( end0 ) ).FirstOrDefault() ;
      var conB = connectors1.OrderBy( x => x.Origin.DistanceTo( end0 ) ).FirstOrDefault() ;

      if ( conA is null || conB is null ) return false ;

      var duct2 = Duct.Create( doc, duct0.DuctType.Id, level0.Id, conA, conB ) ;

      var elem2 = duct2 as Element ;
      elem2.LookupParameter( "直径" ).Set( diameter0 ) ;
      doc.Regenerate() ;


      isSuccess = true ;
    }

    return true ;
  }

  private static bool ConnectParallelRoundDuctWithSJoint( Document doc, Duct duct0, Duct duct1 )
  {
    var (connector0, connector1) = NearestConnectors( duct0, duct1 ) ;
    if ( connector0 is null || connector1 is null ) return false ;

    //角ダクトS
    var familySymbol = doc.RectDuctFamilySymbol( DuctFamilySymbolType.DuctFamilySymbolTypeRoundS ) ;
    if ( familySymbol is null ) return false ;

    // using var tx = new Transaction( doc, "丸ダクトS管接続" ) ;
    // tx.Start() ;

    try {
      FamilyInstance instance ;
      Element elem ;
      var loc = connector0.Origin ;
      var collector = new FilteredElementCollector( doc ) ;
      var level = collector.OfClass( typeof( Level ) ).FirstOrDefault( o => o.Id == duct0.Id ) as Level ;
      var dir0 = duct0.EndDirectionAtPoint( loc ) ;
      // var level0 = duct0.ReferenceLevel ;
      // var level1 = duct1.ReferenceLevel ;

      var isSuccess = false ;

      Connector? jCon0 = null ;
      Connector? jCon1 = null ;

      while ( ! isSuccess ) {
        instance = doc.Create.NewFamilyInstance( loc, familySymbol, level, StructuralType.NonStructural ) ;
        elem = instance as Element ;

        // var p = elem.get_Parameter( BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM ) ;
        // var p1 = duct1.get_Parameter( BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM ) ;


        var v = connector1.Origin - connector0.Origin ;
        var (length, swingRangeX, swingRangeY, swingRangeZ) = v.DecomposeVector2( dir0 ) ;
        var isVertical = swingRangeX.IsAlmostEqualToZero() ;
        var currentDirection = XYZ.BasisX ;
        var rotAxis = currentDirection.CrossProduct( dir0 ).Normalize() ;

        if ( ! rotAxis.IsAlmostEqualTo( XYZ.Zero ) ) {
          // Console.WriteLine("1.0");

          var angle = currentDirection.AngleTo( dir0 ) ;
          // var tan = Math.Tan( angle ) ;
          // var r = 0.5 * duct0.Diameter ;
          // var len1 = r * tan ;
          // var len2 = length - 2 * r * tan ;

          elem.LookupParameter( "半径" ).Set( 0.5 * duct0.Diameter ) ;
          elem.LookupParameter( "オフセット" ).Set( Math.Abs( isVertical ? swingRangeZ : swingRangeX ) ) ;

          ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + rotAxis ), angle ) ;

          var sw = new XYZ( swingRangeX, 0, swingRangeZ ) ;
          var swcross = sw.CrossProduct( dir0 ) ;

          if ( isVertical ) {
            // Console.WriteLine("1.1");

            if ( swcross.X > 0 ) {
              // Console.WriteLine("1.2");
              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), 1 * Math.PI ) ;
            }
          }
          else {
            // Console.WriteLine("1.3");
            var isRotClockWise = v.CrossProduct( dir0 ).Z > 0 ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), ( isRotClockWise ? 1 : 0 ) * Math.PI ) ;
          }
        }
        else {
          if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
            // Console.WriteLine($"2.0 {v.CrossProduct( dir0 ).Z > 0}");
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), 0.5 * Math.PI ) ;
          }
          else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
            // Console.WriteLine("2.1");

            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + XYZ.BasisZ ), Math.PI ) ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), 0.5 * Math.PI ) ;
          }
          else {
            // Console.WriteLine("2.2");

            var angle = ( ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ? XYZ.BasisX.DotProduct( dir0 ) : 0 ) + 1 ) * Math.PI ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + XYZ.BasisZ ), angle ) ;
          }


          var sw = new XYZ( swingRangeX, 0, swingRangeZ ) ;

          if ( isVertical ) {
            if ( dir0.X > 0 ) {
            }
            // Console.WriteLine("3.0");

            elem.LookupParameter( "半径" ).Set( 0.5 * duct0.Diameter ) ;
            elem.LookupParameter( "オフセット" ).Set( Math.Abs( swingRangeY ) ) ;

            if ( ( dir0.IsAlmostEqualTo( XYZ.BasisX ) && swingRangeY > 0 ) || ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) && swingRangeY < 0 ) ) {
              Console.WriteLine( "3.1" ) ;

              ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), Math.PI ) ;
            }
          }
          else {
            // Console.WriteLine("3.2");

            elem.LookupParameter( "半径" ).Set( 0.5 * duct0.Diameter ) ;
            elem.LookupParameter( "オフセット" ).Set( Math.Abs( isVertical ? swingRangeZ : swingRangeX ) ) ;

            var isRotClockWise = v.CrossProduct( dir0 ).Z > 0 ;
            ElementTransformUtils.RotateElement( doc, instance.Id, Line.CreateBound( loc, loc + dir0 ), ( isRotClockWise ? 0.5 : -0.5 ) * Math.PI ) ;
          }
        }

        var connectors = instance.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

        jCon0 = connectors.FirstOrDefault() ;
        jCon1 = connectors.Last() ;

        if ( jCon0 is null || jCon1 is null ) return false ;

        var jcz0 = jCon0.CoordinateSystem.BasisZ ;

        // Z座標向き一緒の場合、色々
        if ( connector0.CoordinateSystem.BasisZ.IsAlmostEqualTo( jcz0 ) ) {
          //接続失敗なのでやり直す
          doc.Delete( new List<ElementId> { elem.Id } ) ;
          continue ;
        }

        // doc.ExtendDuct( duct1, connector1, jCon0, jCon1 ) ;
        // doc.ExtendDuct( duct1, connector1, instance ) ;
        doc.ExtendMEPCurve( duct1, connector1, instance ) ;


        connector1 = duct1.NearestConnectorAtPoint( jCon1.Origin ) ;

        connector1.ConnectTo( jCon1 ) ;
        connector0.ConnectTo( jCon0 ) ;


        isSuccess = true ;
      }


      // tx.Commit() ;
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
      // tx.RollBack() ;
    }


    return true ;
  }

  #endregion


  #region Misc.Utils

  public static XYZ? GetIntersection( XYZ pt0, XYZ dir0, XYZ pt1, XYZ dir1 )
  {
    var w0 = pt0 - pt1 ;
    var a = dir0.DotProduct( dir0 ) ; // dir0の大きさの2乗
    var b = dir0.DotProduct( dir1 ) ; // dir0とdir1の内積
    var c = dir1.DotProduct( dir1 ) ; // dir1の大きさの2乗
    var d = dir0.DotProduct( w0 ) ; // dir0とw0の内積
    var e = dir1.DotProduct( w0 ) ; // dir1とw0の内積

    var denominator = a * c - b * b ; // 交差判定用の分母

    // 平行チェック
    if ( Math.Abs( denominator ) < 1e-9 ) {
      return null ; // 平行または同一直線の場合、交点なし
    }

    // パラメータt, sを計算
    var t = ( b * e - c * d ) / denominator ;
    var s = ( a * e - b * d ) / denominator ;

    // 交点を計算
    var intersectionPt0 = pt0 + t * dir0 ;
    var intersectionPt1 = pt1 + s * dir1 ;

    // 誤差を許容して交点を確認
    if ( intersectionPt0.IsAlmostEqualTo( intersectionPt1, 1e-9 ) ) {
      return intersectionPt0 ; // 交点を返す
    }

    return null ; // 交点なし
  }

  private static XYZ RotateAroundAxis( this XYZ dir, XYZ axis, double rad )
  {
    var rotationTransform = Transform.CreateRotation( axis, rad ) ;
    var rotatedDir = rotationTransform.OfPoint( dir ) ;
    return rotatedDir ;
  }

  public static bool IsOppositTo( this XYZ dir0, XYZ dir1 )
  {
    return dir0.IsAlmostEqualTo( -dir1 ) ;
  }

  public static bool IsNotOppositTo( this XYZ dir0, XYZ dir1 )
  {
    return ! dir0.IsAlmostEqualTo( -dir1 ) ;
  }

  // private static Connector NearestConnectorAtPoint( this Duct duct, XYZ? pt )
  // {
  //   pt ??= XYZ.Zero ;
  //   var connectors = duct.ConnectorManager.Connectors.Cast<Connector>() ;
  //   var con0 = connectors.FirstOrDefault() ;
  //   var con1 = connectors.LastOrDefault() ;
  //   var dist0 = con0.Origin.DistanceTo( pt ) ;
  //   var dist1 = con1.Origin.DistanceTo( pt ) ;
  //   return dist0 < dist1 ? con0 : con1 ;
  // }

  public static Connector NearestConnectorAtPoint( this MEPCurve mepCurve, XYZ? pt )
  {
    pt ??= XYZ.Zero ;
    var connectors = mepCurve.ConnectorManager.Connectors.Cast<Connector>() ;
    var con0 = connectors.FirstOrDefault() ;
    var con1 = connectors.LastOrDefault() ;
    var dist0 = con0.Origin.DistanceTo( pt ) ;
    var dist1 = con1.Origin.DistanceTo( pt ) ;
    return dist0 < dist1 ? con0 : con1 ;
  }

  // /// <summary>
  // /// ダクトの伸縮
  // /// </summary>
  // /// <param name="doc"></param>
  // /// <param name="duct"></param>
  // /// <param name="con"></param>
  // /// <param name="jointInstance"></param>
  // private static void ExtendDuct( this Document doc, Duct duct, Connector con, FamilyInstance? jointInstance )
  // {
  //   if ( jointInstance is null ) return ;
  //   var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
  //   var con0 = connectors.FirstOrDefault() ;
  //   var con1 = connectors.Last() ;
  //   if ( con0 is null || con1 is null ) return ;
  //   doc.Regenerate() ;
  //   var locCurve = duct.Location as LocationCurve ;
  //   if ( locCurve is null ) return ;
  //   var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
  //   var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
  //   // ジョイントのコネクタのうち直線の延長上に近い方のコネクタを選ぶ
  //   var dist0 = GetDistanceFromLineToPoint( pt0, pt1, con0.Origin ) ;
  //   var dist1 = GetDistanceFromLineToPoint( pt0, pt1, con1.Origin ) ;
  //   var jCon = dist0 > dist1 ? con1 : con0 ;
  //   var dist0JCon = jCon.Origin.DistanceTo( pt0 ) ;
  //   var distPt1JCon = jCon.Origin.DistanceTo( pt1 ) ;
  //   locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, jCon.Origin ) ;
  //   var ductCon = duct.NearestConnectorAtPoint( jCon.Origin ) ;
  //   ductCon.ConnectTo( jCon ) ;
  //   doc.Regenerate() ;
  // }


  public static void ExtendMEPCurve( this Document doc, MEPCurve curve, Connector con, FamilyInstance? jointInstance )
  {
    //ToDo: ExtendMEPCurve_とまとめることを検討

    if ( jointInstance is null ) return ;

    var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
    var con0 = connectors.FirstOrDefault() ;
    var con1 = connectors.Last() ;
    if ( con0 is null || con1 is null ) return ;

    doc.Regenerate() ;

    var locCurve = curve.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;

    var dist0 = GetDistanceFromLineToPoint( pt0, pt1, con0.Origin ) ;
    var dist1 = GetDistanceFromLineToPoint( pt0, pt1, con1.Origin ) ;
    var jCon = dist0 > dist1 ? con1 : con0 ;

    var dist0JCon = jCon.Origin.DistanceTo( pt0 ) ;
    var distPt1JCon = jCon.Origin.DistanceTo( pt1 ) ;

    locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, jCon.Origin ) ;

    var curveCon = curve.NearestConnectorAtPoint( jCon.Origin ) ;
    curveCon.ConnectTo( jCon ) ;

    doc.Regenerate() ;
  }


  /// <summary>
  /// ダクトの伸縮
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="duct"></param>
  /// <param name="con"></param>
  /// <param name="jCon"></param>
  // private static void ExtendDuct2( this Document doc, Duct duct, Connector con, Connector jCon )
  // {
  //   var locCurve = duct.Location as LocationCurve ;
  //   if ( locCurve is null ) return ;
  //   var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
  //   var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
  //
  //   var dist0JCon = jCon.Origin.DistanceTo( pt0 ) ;
  //   var distPt1JCon = jCon.Origin.DistanceTo( pt1 ) ;
  //
  //   locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, jCon.Origin ) ;
  //
  //   var ductCon = duct.NearestConnectorAtPoint( jCon.Origin ) ;
  //
  //   ductCon.ConnectTo( jCon ) ;
  //
  //   doc.Regenerate() ;
  // }
  //
  //
  // private static void ExtendPipe2(this Document doc, Pipe pipe, Connector con, Connector jCon)
  // {
  //   var locCurve = pipe.Location as LocationCurve;
  //   if (locCurve is null) return;
  //
  //   var pt0 = locCurve.Curve.GetEndPoint(0);
  //   var pt1 = locCurve.Curve.GetEndPoint(1);
  //   var dist0JCon = jCon.Origin.DistanceTo(pt0);
  //   var distPt1JCon = jCon.Origin.DistanceTo(pt1);
  //
  //   // 配管の延長
  //   locCurve.Curve = Line.CreateBound(dist0JCon > distPt1JCon ? pt0 : pt1, jCon.Origin);
  //
  //   // 最も近いコネクタを見つけて接続
  //   var pipeCon = pipe.NearestConnectorAtPoint(jCon.Origin);
  //   pipeCon.ConnectTo(jCon);
  //
  //   // 変更を反映
  //   doc.Regenerate();
  // }

  



  /// <summary>
  /// 点posとpt0,pt1を通る直線上の最近傍点との距離
  /// </summary>
  /// <param name="pt0"></param>
  /// <param name="pt1"></param>
  /// <param name="pos"></param>
  /// <returns></returns>
  public static double GetDistanceFromLineToPoint( XYZ pt0, XYZ pt1, XYZ pos )
  {
    var v = pt1 - pt0 ; // 線分の方向ベクトル
    var w = pos - pt0 ; // 線分の始点からposへのベクトル
    var projection = w.DotProduct( v ) / v.DotProduct( v ) ;
    var closestPoint = pt0 + projection * v ;
    return closestPoint.DistanceTo( pos ) ;
  }

  #endregion


  // 方向決めるテスト。上下左右の場合におかしい？　dirMid左右向きのときに逆。
  public static void CalcDirections( XYZ pt0, XYZ dir0, XYZ pt1, XYZ dir1 )
  {
    var v0 = dir0.Normalize() ;
    var v1 = dir1.Normalize() ;

    var isParallel = v0.IsAlmostEqualTo( v1.Negate() ) ;
    var planeNormal = dir0.CrossProduct( pt1 - pt0 ).Normalize() ;
    var toPt1 = ( pt1 - pt0 ).Normalize() ;
    var isZdir = v0.IsAlmostEqualTo( XYZ.BasisZ ) || v1.IsAlmostEqualTo( XYZ.BasisZ ) ;

    Console.WriteLine( $"pt0:{pt0.ToF2()} {v0.ToDirStr()} , pt1:{pt1.ToF2()} {v1.ToDirStr()} isParallel:{isParallel} planeNormal:{planeNormal.ToDirStr()}" ) ;
    // C#.NET Revit APIで、XYZ pt0, XYZ dir0, XYZ pt1, XYZ dir1が与えられるとき、pt0からdir0方向に向かう線と、pt1からdir1方向に向かう線が平行である場合に、pt0からpt1に向かう90°で2回曲がる経路を作成したいです。dir0,dir1の方向は上下左右前後だけでなく斜めもあります。ステップバイステップで熟考の末、経路を求めるメソッドを作成してください。

    var middlePoint1 = pt0 + dir0.CrossProduct( planeNormal ).Normalize() * dir0.GetLength() ;
    var middlePoint2 = middlePoint1 + toPt1 * ( pt1 - middlePoint1 ).DotProduct( toPt1 ) ;


    var dirMid = dir1.CrossProduct( planeNormal ).Normalize() ;
    // if ( isZdir && ( dirMid.IsAlmostEqualTo( XYZ.BasisX ) || dirMid.IsAlmostEqualTo( -XYZ.BasisX ) ) ) {
    //   dirMid = -dirMid ;
    //   Console.WriteLine("***");
    // }

    Console.WriteLine( $"{middlePoint1.ToF2()} {middlePoint2.ToF2()} {dirMid.ToF2()} {dirMid.ToDirStr()} {isZdir}" ) ;
  }

  public static string ToDirStr( this XYZ dir )
  {
    var v = dir.Normalize() ;
    if ( v.IsAlmostEqualTo( XYZ.BasisX ) ) return "R" ;
    if ( v.IsAlmostEqualTo( -XYZ.BasisX ) ) return "L" ;
    if ( v.IsAlmostEqualTo( XYZ.BasisY ) ) return "F" ;
    if ( v.IsAlmostEqualTo( -XYZ.BasisY ) ) return "B" ;
    if ( v.IsAlmostEqualTo( XYZ.BasisZ ) ) return "U" ;
    if ( v.IsAlmostEqualTo( -XYZ.BasisZ ) ) return "D" ;
    Console.WriteLine( $"v:{v.ToF2()}" ) ;
    return "?" ;
  }



}