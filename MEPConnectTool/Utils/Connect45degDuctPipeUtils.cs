using System ;
using System.Collections.Generic ;
using System.Linq ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Mechanical ;
using Autodesk.Revit.DB.Plumbing ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.UI ;


namespace MEPConnectTool.Utils ;

public static class Connect45degDuctPipeUtils
{
  #region Connect45degDuctPipe

  public static void BeginConnect45degDuctPipe( this UIApplication uiApp )
  {
    var uiDoc = uiApp.ActiveUIDocument ;
    var doc = uiDoc.Document ;


    try {
      while ( true ) {
        var (elem0, elem1) = uiDoc.PickElementPair() ;

        if ( elem0 is MEPCurve curve0 && elem1 is MEPCurve curve1 ) {
          var (connector0, connector1) = DuctUtils.NearestConnectors( curve0, curve1 ) ;
          var dir0 = curve0.EndDirectionAtPoint( connector0.Origin ) ;
          var dir1 = curve1.EndDirectionAtPoint( connector1.Origin ) ;
          if ( ! dir0.DotProduct( dir1 ).IsAlmostEqualToZero() ) return ;
          Connect45degOrthogonalDuctB( doc, curve0, curve1 ) ;
        }
      }
    }
    catch ( Exception e ) {
      if ( e is OperationCanceledException ) return ; //Escで抜けた場合

      Console.WriteLine( e ) ;
    }
  }


  /// <summary>
  /// 丸ダクト45°接続、上から見て90°直交関係の場合
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="mepCurve0"></param>
  /// <param name="mepCurve1"></param>
  /// <returns></returns>
  private static bool Connect45degOrthogonalDuctB( Document doc, MEPCurve mepCurve0, MEPCurve mepCurve1 )
  {
    var isDuct = mepCurve0 is Duct && mepCurve1 is Duct ;
    var isPipe = mepCurve0 is Pipe && mepCurve1 is Pipe ;

    if ( isDuct && ( (Duct)mepCurve0 ).DuctType.Shape != ConnectorProfileType.Round ) return false ;
    if ( ! isDuct && ! isPipe ) return false ;

    var pipeFamilyName = $"C012021_一般配管用鋼製突合せ溶接式管継手 - 径違いＴ" ;
    var pipeElbowFamilyName = "C012011_一般配管用鋼製突合せ溶接式管継手 - ショート エルボ" ;

    var tJointFamilySymbol = doc.RectDuctFamilySymbol( isDuct ? "022_丸_T管" : pipeFamilyName ) ;
    var elbowFamilySymbol = doc.RectDuctFamilySymbol( isDuct ? "011_丸_エルボ" : pipeElbowFamilyName ) ;


    var (connector0, connector1) = DuctUtils.NearestConnectors( mepCurve0, mepCurve1 ) ;
    var level0 = mepCurve0.ReferenceLevel ;
    var level1 = mepCurve1.ReferenceLevel ;
    var pt0 = connector0.Origin ;
    var pt1 = connector1.Origin ;
    var dir0 = mepCurve0.EndDirectionAtPoint( connector0.Origin - XYZ.BasisZ * level0.Elevation ) ;
    var dir1 = mepCurve1.EndDirectionAtPoint( connector1.Origin - XYZ.BasisZ * level1.Elevation ) ;

    var (length, swingRangeX, swingRangeY, swingRangeZ) = ( pt1 - pt0 ).DecomposeVector2( dir0 ) ;
    var srZ = pt1.Z - pt0.Z ;
    
    var swingRange = new XYZ( swingRangeX, swingRangeY, swingRangeZ ) ;

    var loc0 = pt0 + dir0 * length ;

    var currentDirection = XYZ.BasisX ;
    var angle0 = dir0.AngleTo( currentDirection ) ;
    var rotAxis = currentDirection.CrossProduct( dir0 ).Normalize() ;

 
    var diameter0 = isDuct ? ( (Duct)mepCurve0 ).Diameter : ( (Pipe)mepCurve0 ).Diameter ;
    var diameter1 = isDuct ? ( (Duct)mepCurve1 ).Diameter : ( (Pipe)mepCurve1 ).Diameter ;
    var shouldUse45degElbow = Math.Abs( srZ ) > ( diameter0 * 0.5 * 1.4141592 + 0.1 ) ;

    // Console.WriteLine( $"swingRange: {swingRange.ToF2()} rotAxis:{rotAxis.ToF2()}" ) ;
    // Console.WriteLine( $"_ shouldUse45degElbow:{shouldUse45degElbow} swingRaelbowTiltAngle:geZ:{swingRangeZ} : {diameter0 * 0.5 * 1.4141592 + 0.1}" ) ;

    
    var isOnFirstDuct = IsPointOnMepCurveOfFloorPlan( mepCurve0, loc0 ) ;
    var isOnSecondDuct = IsPointOnMepCurveOfFloorPlan( mepCurve1, loc0 ) ;
    
    
    FamilyInstance tInstance ;
    Element tElement ;

    FamilyInstance elbowInstance ;
    Element elbowElement ;

    using var tx = new Transaction( doc, isDuct ? "Round Duct T-45° Connection" : "Pipe T-45° Connection" ) ;
    tx.Start() ;

    try {
      var isSuccess = false ;
      while ( ! isSuccess ) {
        // -----------------------------------------

        //ティー部分

        tInstance = doc.Create.NewFamilyInstance( loc0, tJointFamilySymbol, level0, StructuralType.NonStructural ) ;
        tElement = tInstance as Element ;

        var tiltAngle = shouldUse45degElbow ? ( swingRange.X > 0 ? 0.25 : -0.25 ) * Math.PI : 0 ;
        
        
        // Console.WriteLine($"swingRange.Z: {swingRange.Z.ToF1()}");

        if ( swingRangeZ.IsAlmostEqualToZero() ) {
          if ( srZ < 0 ) tiltAngle *= -1 ;
        }
        else {
          if (swingRange.Z < 0)
          {
            tiltAngle *= -1;
          }
          tiltAngle *= rotAxis.Z > 0 ? 1 : -1 ;
        }

        // Console.WriteLine($"tiltAngle{tiltAngle}");
        

        if ( isDuct ) {
          tElement.LookupParameter( "ダクト半径 1" )?.Set( 0.5 * diameter0 ) ;
          tElement.LookupParameter( "ダクト半径 3" )?.Set( 0.5 * diameter1 ) ;
        }
        else if ( isPipe ) {
          tElement.LookupParameter( "呼び半径 1" )?.Set( 0.5 * diameter0 ) ;
          tElement.LookupParameter( "呼び半径 2" )?.Set( 0.5 * diameter0 ) ;
          tElement.LookupParameter( "呼び半径 3" )?.Set( 0.5 * diameter1 ) ;
        }


        // ティーの向き
        if ( ! isOnSecondDuct ) {
          if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
          }
          else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + XYZ.BasisZ ), Math.PI ) ;
          }
          else {
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis ), angle0 ) ;
          }
        }
        else {
          //isOnSecondDuct 
          var rotAxis_ = currentDirection.CrossProduct( dir1 ).Normalize() ;
          // Console.WriteLine( $"\n rotAxis:{rotAxis.ToF2()} rotAxis_:{rotAxis_.ToF2()}" ) ;
          // Console.WriteLine( $"{dir0.CrossProduct( dir1 ).ToF2()} {( ( dir0.Z - dir1.Z ) * swingRange.X ) > 0}" ) ;
          if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) || dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis_ ), angle0 ) ;
            if ( ( dir0.Z - dir1.Z ) * swingRange.X > 0 ) {
              ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis_ ), Math.PI ) ;
            }
          }
          else {
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis ), angle0 ) ;
            if ( ( dir0.Z - dir1.Z ) * swingRange.X > 0 ) {
              ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis ), Math.PI ) ;
            }
          }

          if ( rotAxis_.Z < 0 ) {
            // Console.WriteLine( $"rot : {swingRange.ToF2()}" ) ;
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + rotAxis_ ), Math.PI ) ;
          }
        }

        doc.Regenerate() ;

        if ( ! isOnSecondDuct ) {
          if ( ! swingRange.X.IsAlmostEqualToZero() && swingRange.X > 0 ) {
            ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + dir0 ), Math.PI ) ;
          }

          ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + dir0 ), tiltAngle ) ;
        }
        else {
          var (lengthb, swingRangeXb, swingRangeYb, swingRangeZb) = ( pt1 - pt0 ).DecomposeVector2( dir1 ) ;

          //45°傾ける
          if ( shouldUse45degElbow ) {
            if ( ( dir0.Z - dir1.Z ) * swingRange.X > 0 ) {
              // Console.WriteLine( $"up" ) ;
              ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + dir0 ), -Math.PI * 0.25 ) ;
            }
            else {
              // Console.WriteLine( $"down" ) ;
              ElementTransformUtils.RotateElement( doc, tInstance.Id, Line.CreateBound( loc0, loc0 + dir0 ), +Math.PI * 0.25 ) ;
            }
          }
        }


        //ティーのコネクタ

        var tInstaneConnectors = tInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
        var tCon0 = tInstaneConnectors.FirstOrDefault( x => x.CoordinateSystem.BasisZ.IsAlmostEqualTo( dir0.Negate() ) ) ;
        var tCon2 = tInstaneConnectors.FirstOrDefault( x => x.CoordinateSystem.BasisZ.IsAlmostEqualTo( dir0 ) ) ;
        var tCon1 = tInstaneConnectors.FirstOrDefault( x => ! x.CoordinateSystem.BasisZ.IsAlmostEqualTo( dir0 ) && ! x.CoordinateSystem.BasisZ.IsAlmostEqualTo( dir0.Negate() ) ) ;
        // -----------------------------------------

        //伸ばすところの問題。短い方まで
        if ( isOnFirstDuct ) {
          if ( shouldUse45degElbow ) {
            doc.ExtendMEPCurve3( mepCurve0, tCon0, tCon2 ) ;
          }
          else {
            doc.InsertTJointToMEPCurve( mepCurve0, tInstance ) ;
            doc.ExtendMEPCurveToTJoint( mepCurve1, tInstance ) ;
          }
        }
        else {
          if ( shouldUse45degElbow ) {
            doc.ExtendMEPCurveToTJoint( mepCurve0, tInstance ) ;
          }
          else {
            if ( ! isOnSecondDuct ) {
              doc.ExtendMEPCurveToTJoint( mepCurve0, tInstance ) ;
              doc.ExtendMEPCurveToTJoint( mepCurve1, tInstance ) ;
            }
            else {
              doc.ExtendMEPCurveToTJoint( mepCurve0, tInstance ) ;
              doc.InsertTJointToMEPCurve( mepCurve0, tInstance ) ;
              doc.ConnectMEPCurveToTJointBranch( mepCurve1, tInstance ) ;
            }
          }
        }

        // 45°エルボ配置　------------------
        if ( shouldUse45degElbow ) {
          #region 高さが十分違う場合
        
          var loc1 = DuctUtils.GetIntersection( tCon1.Origin, tCon1.CoordinateSystem.BasisZ, pt1, dir1 ) ;
        
          if ( ! isOnSecondDuct ) {
            elbowInstance = doc.Create.NewFamilyInstance( loc1, elbowFamilySymbol, level0, StructuralType.NonStructural ) ;
          }
          else {
            //Create elbow
            elbowInstance = doc.Create.NewFamilyInstance( loc1, elbowFamilySymbol, level1, StructuralType.NonStructural ) ;
          }
        
        
          elbowElement = elbowInstance as Element ;
          if ( isDuct ) elbowElement.LookupParameter( "ダクト半径" )?.Set( 0.5 * diameter1 ) ;
          if ( isPipe ) {
            elbowElement.LookupParameter( "呼び半径" )?.Set( 0.5 * diameter1 ) ;
            elbowElement.LookupParameter( "角度" )?.Set( 0.25 * Math.PI ) ;
          }
        
          var elbowRotAngle = angle0 + ( swingRange.X > 0 ? -0.5 : 0.5 ) * Math.PI ;
          var elbowTiltAngle = ( swingRangeZ < 0 ? 0.5 : -0.5 ) * Math.PI ;
          
          if ( rotAxis.IsZeroLength() ) {
            // Console.WriteLine("ToDo:");
          }
          else {
            elbowRotAngle += rotAxis.Z < 0 ? Math.PI : 0 ;
            elbowTiltAngle *= rotAxis.Z < 0 ? -1 : 1 ;
          }
        
          // Console.WriteLine( $"elbowTiltAngle:{elbowTiltAngle.ToF2()}" ) ;
        
          if ( ! isOnSecondDuct ) {
            Console.WriteLine( $"*** 000" ) ;
            if ( ! rotAxis.IsZeroLength() ) {
              ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis ), elbowRotAngle ) ;
            }
            else {
               ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 +  XYZ.BasisZ  ), elbowRotAngle ) ;
            }
            
            elbowTiltAngle += ( srZ < 0 ? 1 : 0 ) * Math.PI ;
            // Console.WriteLine($"elbowTiltAngle:{elbowTiltAngle}");
            if(elbowTiltAngle>Math.PI) elbowTiltAngle -= Math.PI;
            // Console.WriteLine($"elbowTiltAngle:{elbowTiltAngle}");
            
            ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), elbowTiltAngle ) ;
          }
          else {
            if ( dir0.IsAlmostEqualTo( XYZ.BasisX ) ) {
              var rotAxis_ = currentDirection.CrossProduct( dir1 ).Normalize() ;
              Console.WriteLine( $"dir0.Z-dir1.Z:{dir0.Z - dir1.Z} swingRange.X{swingRange.X}" ) ;
        
              ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis_ ), elbowRotAngle ) ;
        
              if ( swingRangeX < 0 ) {
                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis_ ), Math.PI ) ;
              }
        
              if ( rotAxis_.Z < 0 ) ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), Math.PI ) ;
              ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), elbowTiltAngle ) ;
            }
            else if ( dir0.IsAlmostEqualTo( -XYZ.BasisX ) ) {
              var rotAxis_ = currentDirection.CrossProduct( dir1 ).Normalize() ;
              // Console.WriteLine( $"*** dir0.Z-dir1.Z:{dir0.Z - dir1.Z} swingRange.X{swingRange.X}" ) ;
        
              ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis_ ), elbowRotAngle ) ;
        
              if ( swingRangeX > 0 ) {
                // Console.WriteLine( $"*** A" ) ;
                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis_ ), Math.PI ) ;
                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), 0.5 * Math.PI ) ;
              }
              else {
                // Console.WriteLine( $"*** B" ) ;
                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), -0.5 * Math.PI ) ;
              }
            }
            else {
              ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + rotAxis ), elbowRotAngle ) ;
              if ( swingRangeX > 0 ) {
                // Console.WriteLine( $"*** C" ) ;

                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), -0.5 * Math.PI ) ;
              }
              else {
                // Console.WriteLine( $"*** D" ) ;

                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), 0.5 * Math.PI ) ;
              }
        
              if ( ( dir0.Z - dir1.Z ) * swingRange.X < 0 ) {
                // Console.WriteLine( $"*** E" ) ;

                ElementTransformUtils.RotateElement( doc, elbowInstance.Id, Line.CreateBound( loc1, loc1 + dir1 ), Math.PI ) ;
              }
            }
          }
        
          doc.ExtendMEPCurveToJoint( mepCurve1, elbowInstance ) ;
          
          
          if ( isDuct ) doc.ConnectInstancesWithDuct( elbowInstance, tInstance ) ;
          if ( isPipe ) doc.ConnectInstancesWithPipe( elbowInstance, tInstance, ( (Pipe)mepCurve0 ).PipeType ) ;
        
          #endregion
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

  /// <summary>
  /// MEPCurveとティーインスタンスの近い場所同士をつなぐ
  /// ToDo:ExtendMEPCurveToJointに差し替え
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="curve"></param>
  /// <param name="jointInstance"></param>
  public static void ExtendMEPCurveToTJoint( this Document doc, MEPCurve curve, FamilyInstance? jointInstance )
  {
    doc.Regenerate() ;
    if ( jointInstance is null ) return ;
    var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
    if ( connectors.ToArray().Length != 3 ) return ;

    var locCurve = curve.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
    var ptc = ( pt0 + pt1 ) * 0.5 ;

    var nearestConnector0 = connectors.OrderBy( c => c.Origin.DistanceTo( pt0 ) ).FirstOrDefault() ;
    var nearestConnector1 = connectors.OrderBy( c => c.Origin.DistanceTo( pt1 ) ).FirstOrDefault() ;
    var branchConnector = connectors.FirstOrDefault( c => c != nearestConnector0 && c != nearestConnector1 ) ;
    if ( nearestConnector0 is null || nearestConnector1 is null || branchConnector is null ) return ;

    var dirCurve = ( pt0 - pt1 ).Normalize() ;
    var dirCon = ( nearestConnector0.Origin - nearestConnector1.Origin ).Normalize() ;
    var isOnStem = dirCurve.IsAlmostEqualTo( dirCon ) || dirCurve.IsAlmostEqualTo( -dirCon ) || dirCon.IsZeroLength() ;

    // Console.WriteLine( $"isOnStem:{isOnStem}" ) ;

    if ( isOnStem ) {
      var nearestConnector = connectors.OrderBy( c => c.Origin.DistanceTo( ptc ) ).FirstOrDefault() ;
      if ( nearestConnector is null ) return ;
      var dist0JCon = nearestConnector.Origin.DistanceTo( pt0 ) ;
      var distPt1JCon = nearestConnector.Origin.DistanceTo( pt1 ) ;

      locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, nearestConnector.Origin ) ;

      var curveCon = curve.NearestConnectorAtPoint( nearestConnector.Origin ) ;
      curveCon.ConnectTo( nearestConnector ) ;
    }

    doc.Regenerate() ;
  }


  /// <summary>
  /// MEPCurve上にTを割り込ませる
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="curve0"></param>
  /// <param name="jointInstance"></param>
  private static void InsertTJointToMEPCurve( this Document doc, MEPCurve curve0, FamilyInstance? jointInstance )
  {
    if ( jointInstance is null ) return ;
    var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
    if ( connectors.ToArray().Length != 3 ) return ;

    var locCurve = curve0.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;

    var nearestConnector0 = connectors.OrderBy( c => c.Origin.DistanceTo( pt0 ) ).FirstOrDefault() ;
    var nearestConnector1 = connectors.OrderBy( c => c.Origin.DistanceTo( pt1 ) ).FirstOrDefault() ;
    var branchConnector = connectors.FirstOrDefault( c => c != nearestConnector0 && c != nearestConnector1 ) ;
    if ( nearestConnector0 is null || nearestConnector1 is null || branchConnector is null ) return ;

    var dirCurve = ( pt0 - pt1 ).Normalize() ;
    var dirCon = ( nearestConnector0.Origin - nearestConnector1.Origin ).Normalize() ;
    var isOnStem = dirCurve.IsAlmostEqualTo( dirCon ) || dirCurve.IsAlmostEqualTo( -dirCon ) ;
    // Console.WriteLine($"=== {dirCurve.ToF2()} {dirCon.ToF2()} isOnStem:{isOnStem}");

    try {
      if ( isOnStem ) {
        switch ( curve0 ) {
          case Duct duct :
            locCurve.Curve = Line.CreateBound( pt0, nearestConnector0.Origin ) ;
            curve0.NearestConnectorAtPoint( nearestConnector0.Origin ).ConnectTo( nearestConnector0 ) ;
            Duct.Create( doc, duct.DuctType.Id, nearestConnector1.Owner.LevelId, nearestConnector1, pt1 ) ;
            break ;
          case Pipe pipe :
            locCurve.Curve = Line.CreateBound( pt0, nearestConnector0.Origin ) ;
            curve0.NearestConnectorAtPoint( nearestConnector0.Origin ).ConnectTo( nearestConnector0 ) ;
            Pipe.Create( doc, pipe.PipeType.Id, pipe.ReferenceLevel.Id, nearestConnector1, pt1 ) ;
            break ;
        }
      }
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
    }


    doc.Regenerate() ;
  }

  private static void ConnectMEPCurveToTJointBranch( this Document doc, MEPCurve curve, FamilyInstance? jointInstance )
  {
    doc.Regenerate() ;
    if ( jointInstance is null ) return ;
    var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
    if ( connectors.ToArray().Length != 3 ) return ;

    var locCurve = curve.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
    var dirCurve = ( pt0 - pt1 ).Normalize() ;

    var branchConnector = connectors.FirstOrDefault( c => c.CoordinateSystem.BasisZ.IsAlmostEqualTo( dirCurve ) || c.CoordinateSystem.BasisZ.IsAlmostEqualTo( -dirCurve ) ) ;

    if ( branchConnector is null ) return ;

    var dirBranch = branchConnector.CoordinateSystem.BasisZ ;
    var dirPt01 = ( pt1 - pt0 ).Normalize() ;

    // Console.WriteLine( $"Direction: {dirBranch.ToF2()} {dirPt01.ToF2()}" ) ;

    if ( dirBranch.IsAlmostEqualTo( dirPt01 ) ) locCurve.Curve = Line.CreateBound( pt1, branchConnector.Origin ) ;
    if ( dirBranch.IsAlmostEqualTo( -dirPt01 ) ) locCurve.Curve = Line.CreateBound( pt0, branchConnector.Origin ) ;
    var curveCon = curve.NearestConnectorAtPoint( branchConnector.Origin ) ;
    curveCon.ConnectTo( branchConnector ) ;

    doc.Regenerate() ;
  }


  public static void ExtendMEPCurveToJoint( this Document doc, MEPCurve curve, FamilyInstance? jointInstance )
  {
    doc.Regenerate() ;
    if ( jointInstance is null ) return ;
    var connectors = jointInstance.MEPModel.ConnectorManager.Connectors.Cast<Connector>().ToList() ;
    if ( connectors.ToArray().Length < 2 ) return ;

    var locCurve = curve.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
    var ptc = ( pt0 + pt1 ) * 0.5 ;

    var nearestConnector0 = connectors.OrderBy( c => c.Origin.DistanceTo( pt0 ) ).FirstOrDefault() ;
    var nearestConnector1 = connectors.OrderBy( c => c.Origin.DistanceTo( pt1 ) ).FirstOrDefault() ;
    var branchConnector = connectors.FirstOrDefault( c => c != nearestConnector0 && c != nearestConnector1 ) ;
    if ( nearestConnector0 is null || nearestConnector1 is null ) return ;
    
    var dirCurve = ( pt0 - pt1 ).Normalize() ;
    var dirCon = ( nearestConnector0.Origin - nearestConnector1.Origin ).Normalize() ;
    var isOnStem = dirCurve.IsAlmostEqualTo( dirCon ) || dirCurve.IsAlmostEqualTo( -dirCon ) || dirCon.IsZeroLength() ;

    if ( branchConnector is null ) {
      //elbow
      var dirPt0To1 = ( pt1 - pt0 ).Normalize() ;
      var adoptedConnector = connectors.FirstOrDefault( c => c.CoordinateSystem.BasisZ.IsAlmostEqualTo( dirPt0To1 ) || c.CoordinateSystem.BasisZ.IsAlmostEqualTo( -dirPt0To1 ) ) ;
      if(adoptedConnector is null ) return ;
      var dirConnector = adoptedConnector.CoordinateSystem.BasisZ ;
      if ( dirConnector.IsAlmostEqualTo( dirPt0To1 ) ) locCurve.Curve = Line.CreateBound( pt1, adoptedConnector.Origin ) ;
      if ( dirConnector.IsAlmostEqualTo( -dirPt0To1 ) ) locCurve.Curve = Line.CreateBound( pt0, adoptedConnector.Origin ) ;
    }
    else {
      //T
      if ( isOnStem ) {
        var nearestConnector = connectors.OrderBy( c => c.Origin.DistanceTo( ptc ) ).FirstOrDefault() ;
        if ( nearestConnector is null ) return ;
        var dist0JCon = nearestConnector.Origin.DistanceTo( pt0 ) ;
        var distPt1JCon = nearestConnector.Origin.DistanceTo( pt1 ) ;

        locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, nearestConnector.Origin ) ;

        var curveCon = curve.NearestConnectorAtPoint( nearestConnector.Origin ) ;
        curveCon.ConnectTo( nearestConnector ) ;
      }
    }

    

    doc.Regenerate() ;
  }
  
  // /// <summary>
  // /// MEP曲線を伸ばして、指定されたコネクタと接続します。
  // /// </summary>
  // /// <param name="doc"></param>
  // /// <param name="curve"></param>
  // /// <param name="jCon"></param>
  // private static void ExtendMEPCurve2( this Document doc, MEPCurve curve, Connector jCon )
  // {
  //   var locCurve = curve.Location as LocationCurve ;
  //   if ( locCurve is null ) return ;
  //   
  //   
  //   var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
  //   var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
  //   var dist0JCon = jCon.Origin.DistanceTo( pt0 ) ;
  //   var distPt1JCon = jCon.Origin.DistanceTo( pt1 ) ;
  //
  //   var dirCon = jCon.CoordinateSystem.BasisZ ;
  //   
  //   
  //   // MEP要素の延長
  //   locCurve.Curve = Line.CreateBound( dist0JCon > distPt1JCon ? pt0 : pt1, jCon.Origin ) ;
  //
  //   // 最も近いコネクタを見つけて接続
  //   var curveCon = curve.NearestConnectorAtPoint( jCon.Origin ) ;
  //   curveCon.ConnectTo( jCon ) ;
  //
  //   // 変更を反映
  //   doc.Regenerate() ;
  // }


  /// <summary>
  /// MEP曲線（ダクトまたはパイプ）を伸ばして、指定されたコネクタと接続します。
  /// このメソッドは、現在のMEP曲線の形状を変更し、必要に応じて新しいダクトまたはパイプセグメントを作成します。
  /// </summary>
  /// <param name="doc">現在のRevitドキュメント</param>
  /// <param name="curve">伸ばすMEP曲線（ダクトまたはパイプ）</param>
  /// <param name="jCon0">接続先のコネクタ0</param>
  /// <param name="jCon1">接続先のコネクタ1</param>
  private static void ExtendMEPCurve3( this Document doc, MEPCurve curve, Connector jCon0, Connector jCon1 )
  {
    var locCurve = curve.Location as LocationCurve ;
    if ( locCurve is null ) return ;

    var pt0 = locCurve.Curve.GetEndPoint( 0 ) ;
    var pt1 = locCurve.Curve.GetEndPoint( 1 ) ;
    var distPt0JCon0 = jCon0.Origin.DistanceTo( pt0 ) ;
    var distPt1JCon0 = jCon0.Origin.DistanceTo( pt1 ) ;

    try {
      switch ( curve ) {
        case Duct duct when distPt1JCon0 < distPt0JCon0 :
          locCurve.Curve = Line.CreateBound( pt0, jCon0.Origin ) ;
          curve.NearestConnectorAtPoint( jCon0.Origin ).ConnectTo( jCon0 ) ;
          Duct.Create( doc, duct.DuctType.Id, jCon1.Owner.LevelId, jCon1, pt1 ) ;
          break ;
        case Duct duct :
          locCurve.Curve = Line.CreateBound( pt0, jCon1.Origin ) ;
          curve.NearestConnectorAtPoint( jCon1.Origin ).ConnectTo( jCon1 ) ;
          Duct.Create( doc, duct.DuctType.Id, jCon0.Owner.LevelId, jCon0, pt1 ) ;
          break ;
        case Pipe pipe when distPt1JCon0 < distPt0JCon0 :
          locCurve.Curve = Line.CreateBound( pt0, jCon0.Origin ) ;
          curve.NearestConnectorAtPoint( jCon0.Origin ).ConnectTo( jCon0 ) ;
          Pipe.Create( doc, pipe.PipeType.Id, pipe.ReferenceLevel.Id, jCon1, pt1 ) ;
          break ;
        case Pipe pipe :
          locCurve.Curve = Line.CreateBound( pt0, jCon1.Origin ) ;
          curve.NearestConnectorAtPoint( jCon1.Origin ).ConnectTo( jCon1 ) ;
          Pipe.Create( doc, pipe.PipeType.Id, pipe.ReferenceLevel.Id, jCon0, pt1 ) ;
          break ;
      }
    }
    catch ( Exception e ) {
      Console.WriteLine( e ) ;
    }

    doc.Regenerate() ;
  }


  /// <summary>
  /// インスタンス間をダクトでつなぐ
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="instance0"></param>
  /// <param name="instance1"></param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  private static void ConnectInstancesWithDuct( this Document doc, FamilyInstance instance0, FamilyInstance instance1 )
  {
    // 入力引数の検証
    if ( doc == null || instance0 == null || instance1 == null ) throw new ArgumentNullException( "Invalid document or instance." ) ;

    // コネクタを取得
    var connectors0 = instance0.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
    var connectors1 = instance1.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

    // 最短距離を測るための変数
    Connector closestConnector0 = null ;
    Connector closestConnector1 = null ;
    var minDistance = double.MaxValue ;

    // コネクタ間の距離を計測
    foreach ( var connector0 in connectors0 ) {
      foreach ( var connector1 in connectors1 ) {
        var distance = ( connector0.Origin - connector1.Origin ).GetLength() ;
        if ( ! ( distance < minDistance ) ) continue ;
        minDistance = distance ;
        closestConnector0 = connector0 ;
        closestConnector1 = connector1 ;
      }
    }

    // 最も近いコネクタが見つかった場合、ダクトを作成
    if ( closestConnector0 == null || closestConnector1 == null ) return ;

    try {
      // ダクトタイプとサイズを定義（適宜調整）
      var ductType = new FilteredElementCollector( doc ).OfClass( typeof( DuctType ) ).Cast<DuctType>().FirstOrDefault( dt => dt.Name.Equals( "00_丸ダクト" ) ) ;
      if ( ductType == null ) throw new InvalidOperationException( "Duct type not found." ) ;

      //ダクト最小長さは1/10インチ。
      var ductLength = closestConnector0.Origin.DistanceTo( closestConnector1.Origin ) ;
      if ( ductLength < 0.01 ) throw new InvalidOperationException( "Joint spacing is too short to create duct between fittings." ) ;

      // ダクトの作成
      var duct = Duct.Create( doc, ductType.Id, instance0.LevelId, closestConnector0, closestConnector1 ) ;
    }
    catch ( Exception ex ) {
      TaskDialog.Show( "Failed to create duct.", $"{ex.Message}" ) ;
    }
  }


  /// <summary>
  /// インスタンス間を配管でつなぐ
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="instance0"></param>
  /// <param name="instance1"></param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  private static void ConnectInstancesWithPipe( this Document doc, FamilyInstance instance0, FamilyInstance instance1, PipeType pipeType_ )
  {
    // 入力引数の検証
    if ( doc == null || instance0 == null || instance1 == null )
      throw new ArgumentNullException( "Invalid document or instance." ) ;

    // コネクタを取得
    var connectors0 = instance0.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;
    var connectors1 = instance1.MEPModel.ConnectorManager.Connectors.Cast<Connector>() ;

    // 最短距離を測るための変数
    Connector closestConnector0 = null ;
    Connector closestConnector1 = null ;
    var minDistance = double.MaxValue ;

    // コネクタ間の距離を計測
    foreach ( var connector0 in connectors0 ) {
      foreach ( var connector1 in connectors1 ) {
        var distance = ( connector0.Origin - connector1.Origin ).GetLength() ;
        if ( ! ( distance < minDistance ) ) continue ;
        minDistance = distance ;
        closestConnector0 = connector0 ;
        closestConnector1 = connector1 ;
      }
    }

    // 最も近いコネクタが見つかった場合、配管を作成
    if ( closestConnector0 == null || closestConnector1 == null ) return ;

    try {

      var pipeLength = closestConnector0.Origin.DistanceTo( closestConnector1.Origin ) ;
      if ( pipeLength < 0.01 ) {
        throw new InvalidOperationException( "Joint spacing is too short to create pipe between fittings." ) ;
      }

      // 配管の作成
      var pipe = Pipe.Create( doc, pipeType_.Id, instance0.LevelId, closestConnector0, closestConnector1 ) ;
    }
    catch ( Exception ex ) {
      TaskDialog.Show( "Failed to create pipe.", $"{ex.Message}" ) ;
    }
  }

  /// <summary>
  /// 指定されたXYZ点がMepCurve上に存在するかどうかを判定します。
  /// </summary>
  /// <param name="mepCurve">判定対象のMepCurve</param>
  /// <param name="point">判定するXYZ点</param>
  /// <returns>点が曲線上に存在する場合はtrue、そうでない場合はfalse</returns>
  private static bool IsPointOnMepCurve( MEPCurve mepCurve, XYZ point )
  {
    var tolerance = 0.00001 ;
    var locationCurve = mepCurve.Location as LocationCurve ;
    if ( locationCurve is null ) return false ;
    var curve = locationCurve.Curve ;
    if ( curve is null ) return false ;
    var closestPoint = curve.Project( point ).XYZPoint ;
    var distance = point.DistanceTo( closestPoint ) ;
    // Console.WriteLine($"distance:{distance}");
    return distance <= tolerance ;
  }

  private static bool IsPointOnMepCurveOfFloorPlan( MEPCurve mepCurve, XYZ pt )
  {
    var point = new XYZ(pt.X, pt.Y, 0);
    
    var tolerance = 0.00001 ;
    var locationCurve = mepCurve.Location as LocationCurve ;
    if ( locationCurve is null ) return false ;
    var curve = locationCurve.Curve ;
    if ( curve is null ) return false ;
    var closestPoint = curve.Project( point ).XYZPoint ;
    var distance = point.DistanceTo( closestPoint ) ;
    // Console.WriteLine($"distance:{distance}");
    return distance <= tolerance ;
  }

  
}