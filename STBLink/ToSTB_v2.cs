using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.ExtensibleStorage ;
using Autodesk.Revit.DB.Structure ;
using ST_BRIDGE_V2 ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace STBLink
{
  class ToSTB_v2
  {
    private static ST_BRIDGE stb = null ;

    private static List<Level> Levels = null ;

    private static int id = 0 ;
    private static int id_sect = 0 ;

    private static List<Data.GridInformation>[] GridInfo = null ;


    /// <summary>
    /// Export
    /// </summary>
    /// <param name="savepath"></param>
    internal static void ExportSTB( string savepath )
    {
      if ( Data.Check_PileZeroLength() == 1 ) {
        ExportForm ef = new ExportForm() ;
        if ( ef.ShowDialog() != DialogResult.OK ) return ;
      }

      stb = new ST_BRIDGE()
      {
        version = "2.0.1",
        StbCommon = new StbCommon(),
        StbModel = new StbModel()
        {
          StbNodes = new List<StbNode>(),
          StbMembers = new StbMembers()
          {
            StbColumns = new List<StbColumn>(),
            StbPosts = new List<StbPost>(),
            StbGirders = new List<StbGirder>(),
            StbBeams = new List<StbBeam>(),
            StbBraces = new List<StbBrace>(),
            StbSlabs = new List<StbSlab>(),
            StbWalls = new List<StbWall>(),
            StbFootings = new List<StbFooting>(),
            StbStripFootings = new List<StbStripFooting>(),
            StbPiles = new List<StbPile>(),
            StbFoundationColumns = new List<StbFoundationColumn>(),
            StbParapets = new List<StbParapet>(),
            StbOpens = new List<StbOpen>(),
          },
          StbSections = new StbSections()
          {
            StbSecColumn_RC = new List<StbSecColumn_RC>(),
            StbSecColumn_S = new List<StbSecColumn_S>(),
            StbSecColumn_SRC = new List<StbSecColumn_SRC>(),
            StbSecColumn_CFT = new List<StbSecColumn_CFT>(),
            StbSecBeam_RC = new List<StbSecBeam_RC>(),
            StbSecBeam_S = new List<StbSecBeam_S>(),
            StbSecBeam_SRC = new List<StbSecBeam_SRC>(),
            StbSecBrace_S = new List<StbSecBrace_S>(),
            StbSecSlab_RC = new List<StbSecSlab_RC>(),
            StbSecSlabDeck = new List<StbSecSlabDeck>(),
            StbSecSlabPrecast = new List<StbSecSlabPrecast>(),
            StbSecWall_RC = new List<StbSecWall_RC>(),
            StbSecFoundation_RC = new List<StbSecFoundation_RC>(),
            StbSecPile_RC = new List<StbSecPile_RC>(),
            StbSecPile_S = new List<StbSecPile_S>(),
            StbSecPileProduct = new List<StbSecPileProduct>(),
            StbSecOpen_RC = new List<StbSecOpen_RC>(),
            StbSecParapet_RC = new List<StbSecParapet_RC>(),
            StbSecSteel = new StbSecSteel()
            {
              StbSecRollH = new List<StbSecRollH>(),
              StbSecBuildH = new List<StbSecBuildH>(),
              StbSecRollBox = new List<StbSecRollBox>(),
              StbSecBuildBox = new List<StbSecBuildBox>(),
              StbSecPipe = new List<StbSecPipe>(),
              StbSecRollT = new List<StbSecRollT>(),
              StbSecRollC = new List<StbSecRollC>(),
              StbSecRollL = new List<StbSecRollL>(),
              StbSecLipC = new List<StbSecLipC>(),
              StbSecFlatBar = new List<StbSecFlatBar>(),
              StbSecRoundBar = new List<StbSecRoundBar>(),
            },
          },
        },
      } ;

      Export_Common() ;


      id = 0 ;
      Export_Grid() ;
      Export_Level() ;


      id = 0 ;
      id_sect = 0 ;
      Export_Column() ;
      Export_Girder( StructuralInstanceUsage.Girder ) ;
      Export_Girder( StructuralInstanceUsage.Joist ) ;

      Export_Wall() ;
      Export_Slab() ;
      Export_Brace() ;
      Export_Footing() ;

      SetNodeKind() ;

      stb.Write( savepath ) ;
    }


    /// <summary>
    /// 節点番号の取得と登録
    /// </summary>
    /// <param name="p">座標[mm]</param>
    /// <returns>stbの節点番号</returns>
    private static int GetNodeId( XYZ p )
    {
      for ( int i = 0 ; i < stb.StbModel.StbNodes.Count ; ++i ) {
        XYZ n = new XYZ( stb.StbModel.StbNodes[ i ].X, stb.StbModel.StbNodes[ i ].Y, stb.StbModel.StbNodes[ i ].Z ) ;
        if ( n.DistanceTo( p ) < 0.001 ) {
          return stb.StbModel.StbNodes[ i ].id ;
        }
      }

      StbNode node = new StbNode()
      {
        id = stb.StbModel.StbNodes.Count + 1,
        X = p.X,
        Y = p.Y,
        Z = p.Z,
        kind = StbNodeKind.OTHER,
      } ;

      XYZ p2 = new XYZ( p.X, p.Y, 0 ) ;

      int count = 0 ;

      for ( int i = 0 ; i < GridInfo.Length ; ++i ) {
        for ( int g = 0 ; g < GridInfo[ i ].Count ; ++g ) {
          XYZ ps = GridInfo[ i ][ g ].ps ;
          XYZ pe = GridInfo[ i ][ g ].pe ;

          XYZ v1 = ( ps - p2 ).Normalize() ;
          XYZ v2 = ( pe - p2 ).Normalize() ;

          if ( ps.DistanceTo( p2 ) < 0.001 || pe.DistanceTo( p2 ) < 0.001 || Math.Abs( v1.DotProduct( v2 ) + 1 ) < 0.001 ) {
            var axes = stb.StbModel.StbAxes.StbParallelAxes[ i ] ;
            int index = axes.StbParallelAxis.FindIndex( x => x.id == GridInfo[ i ][ g ].stb_id ) ;
            if ( index >= 0 ) {
              if ( ! axes.StbParallelAxis[ index ].StbNodeIdList.Any( x => x.id == node.id ) ) {
                axes.StbParallelAxis[ index ].StbNodeIdList.Add( new StbNodeId() { id = node.id } ) ;
                //node.kind = StbNodeKind.ON_GRID;
                count++ ;
              }

              break ;
            }
          }
        }
      }

      //円弧軸
      if ( stb.StbModel.StbAxes.StbArcAxes != null && stb.StbModel.StbAxes.StbArcAxes.Count > 0 ) {
        foreach ( var axes in stb.StbModel.StbAxes.StbArcAxes ) {
          XYZ center = new XYZ( axes.X, axes.Y, 0 ) ;
          double r = center.DistanceTo( p2 ) ;
          foreach ( var axis in axes.StbArcAxis ) {
            if ( ! axis.StbNodeIdList.Any( a => a.id == node.id ) ) {
              if ( Math.Abs( axis.radius - r ) < 0.001 ) {
                axis.StbNodeIdList.Add( new StbNodeId() { id = node.id } ) ;
                //node.kind = StbNodeKind.ON_GRID;
                count++ ;
                break ;
              }
            }
          }
        }
      }

      //放射軸
      if ( stb.StbModel.StbAxes.StbRadialAxes != null && stb.StbModel.StbAxes.StbRadialAxes.Count > 0 ) {
        foreach ( var axes in stb.StbModel.StbAxes.StbRadialAxes ) {
          XYZ center = new XYZ( axes.X, axes.Y, 0 ) ;
          foreach ( var axis in axes.StbRadialAxis ) {
            if ( ! axis.StbNodeIdList.Any( a => a.id == node.id ) ) {
              double angle = axis.angle * Math.PI / 180 ;
              XYZ v1 = new XYZ( Math.Cos( angle ), Math.Sin( angle ), 0 ) ;
              XYZ v2 = XYZ.BasisZ.CrossProduct( v1 ).Normalize() ;
              if ( Math.Abs( ( p2 - center ).DotProduct( v2 ) ) < 0.001 ) {
                axis.StbNodeIdList.Add( new StbNodeId() { id = node.id } ) ;
                //node.kind = StbNodeKind.ON_GRID;
                count++ ;
                break ;
              }
            }
          }
        }
      }

      if ( count >= 2 ) {
        //XとYの2方向に所属している必要がある
        node.kind = StbNodeKind.ON_GRID ;
      }


      bool add = false ;
      for ( int i = 0 ; i < stb.StbModel.StbStories.Count ; ++i ) {
        if ( Math.Abs( stb.StbModel.StbStories[ i ].height - p.Z ) < 0.001 ) {
          stb.StbModel.StbStories[ i ].StbNodeIdList.Add( new StbNodeId() { id = node.id } ) ;
          add = true ;
          break ;
        }
      }

      if ( ! add ) {
        //層高さに合致しない場合は、一番近い層に所属させる。
        double min = stb.StbModel.StbStories.Min( x => Math.Abs( x.height - p.Z ) ) ;
        int index = stb.StbModel.StbStories.FindIndex( x => Math.Abs( Math.Abs( x.height - p.Z ) - min ) < 0.001 ) ;
        if ( index >= 0 ) {
          stb.StbModel.StbStories[ index ].StbNodeIdList.Add( new StbNodeId() { id = node.id } ) ;
        }
      }


      stb.StbModel.StbNodes.Add( node ) ;

      return node.id ;
    }

    /// <summary>
    /// StbNode.kind の設定
    /// </summary>
    private static void SetNodeKind()
    {
      foreach ( var n in stb.StbModel.StbNodes.Where( a => a.kind == StbNodeKind.OTHER ) ) {
        XYZ p = new XYZ( n.X, n.Y, n.Z ) ;

        bool isCanti = false ;
        int canti_id = 0 ;

        foreach ( var b in stb.StbModel.StbMembers.StbGirders ) {
          if ( n.id == b.id_node_start || n.id == b.id_node_end ) {
            if ( b.kind_structure == StbGirderKind_structure.RC ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_RC.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }
            else if ( b.kind_structure == StbGirderKind_structure.S ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_S.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }
            else if ( b.kind_structure == StbGirderKind_structure.SRC ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_SRC.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }

            if ( isCanti ) {
              canti_id = b.id ;
            }

            continue ;
          }

          var n1 = stb.StbModel.StbNodes.Find( a => a.id == b.id_node_start ) ;
          var n2 = stb.StbModel.StbNodes.Find( a => a.id == b.id_node_end ) ;
          XYZ p1 = new XYZ( n1.X, n1.Y, n1.Z ) ;
          XYZ p2 = new XYZ( n2.X, n2.Y, n2.Z ) ;

          if ( Math.Abs( p.DistanceTo( p1 ) + p.DistanceTo( p2 ) - p1.DistanceTo( p2 ) ) < 1 ) {
            n.kind = StbNodeKind.ON_GIRDER ;
            n.id_member = b.id ;
            break ;
          }
        }

        if ( isCanti ) {
          n.kind = StbNodeKind.ON_CANTI ;
          n.id_member = canti_id ;
        }

        if ( n.kind != StbNodeKind.OTHER ) continue ;


        foreach ( var b in stb.StbModel.StbMembers.StbBeams ) {
          if ( n.id == b.id_node_start || n.id == b.id_node_end ) {
            if ( b.kind_structure == StbGirderKind_structure.RC ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_RC.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }
            else if ( b.kind_structure == StbGirderKind_structure.S ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_S.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }
            else if ( b.kind_structure == StbGirderKind_structure.SRC ) {
              var sec = stb.StbModel.StbSections.StbSecBeam_SRC.Find( a => a.id == b.id_section ) ;
              isCanti = sec.isCanti ;
            }

            if ( isCanti ) {
              canti_id = b.id ;
            }

            continue ;
          }

          var n1 = stb.StbModel.StbNodes.Find( a => a.id == b.id_node_start ) ;
          var n2 = stb.StbModel.StbNodes.Find( a => a.id == b.id_node_end ) ;
          XYZ p1 = new XYZ( n1.X, n1.Y, n1.Z ) ;
          XYZ p2 = new XYZ( n2.X, n2.Y, n2.Z ) ;

          if ( Math.Abs( p.DistanceTo( p1 ) + p.DistanceTo( p2 ) - p1.DistanceTo( p2 ) ) < 1 ) {
            n.kind = StbNodeKind.ON_BEAM ;
            n.id_member = b.id ;
            break ;
          }
        }

        if ( isCanti ) {
          n.kind = StbNodeKind.ON_CANTI ;
          n.id_member = canti_id ;
        }

        if ( n.kind != StbNodeKind.OTHER ) continue ;


        foreach ( var c in stb.StbModel.StbMembers.StbColumns ) {
          if ( n.id == c.id_node_bottom || n.id == c.id_node_top ) {
            continue ;
          }

          var n1 = stb.StbModel.StbNodes.Find( a => a.id == c.id_node_bottom ) ;
          var n2 = stb.StbModel.StbNodes.Find( a => a.id == c.id_node_top ) ;
          XYZ p1 = new XYZ( n1.X, n1.Y, n1.Z ) ;
          XYZ p2 = new XYZ( n2.X, n2.Y, n2.Z ) ;

          if ( Math.Abs( p.DistanceTo( p1 ) + p.DistanceTo( p2 ) - p1.DistanceTo( p2 ) ) < 1 ) {
            n.kind = StbNodeKind.ON_COLUMN ;
            n.id_member = c.id ;
            break ;
          }
        }

        if ( n.kind != StbNodeKind.OTHER ) continue ;


        foreach ( var c in stb.StbModel.StbMembers.StbPosts ) {
          if ( n.id == c.id_node_bottom || n.id == c.id_node_top ) {
            continue ;
          }

          var n1 = stb.StbModel.StbNodes.Find( a => a.id == c.id_node_bottom ) ;
          var n2 = stb.StbModel.StbNodes.Find( a => a.id == c.id_node_top ) ;
          XYZ p1 = new XYZ( n1.X, n1.Y, n1.Z ) ;
          XYZ p2 = new XYZ( n2.X, n2.Y, n2.Z ) ;

          if ( Math.Abs( p.DistanceTo( p1 ) + p.DistanceTo( p2 ) - p1.DistanceTo( p2 ) ) < 1 ) {
            n.kind = StbNodeKind.ON_POST ;
            n.id_member = c.id ;
            break ;
          }
        }

        if ( n.kind != StbNodeKind.OTHER ) continue ;


        foreach ( var s in stb.StbModel.StbMembers.StbSlabs ) {
          var nodes = s.StbNodeIdOrderList ;

          if ( nodes.Contains( n.id ) ) {
            if ( s.kind_slab == StbSlabKind_slab.CANTI ) {
              n.kind = StbNodeKind.ON_CANTI ;
              n.id_member = s.id ;
              break ;
            }
          }

          var points = new List<XYZ>() ;
          foreach ( var nodeID in nodes ) {
            var n1 = stb.StbModel.StbNodes.Find( a => a.id == nodeID ) ;
            XYZ p1 = new XYZ( n1.X, n1.Y, n1.Z ) ;
            points.Add( p1 ) ;
          }

          //同一平面上チェック
          XYZ v1 = ( points[ 1 ] - points[ 0 ] ).Normalize() ;
          XYZ v2 = ( points[ 2 ] - points[ 0 ] ).Normalize() ;
          XYZ normal = v1.CrossProduct( v2 ).Normalize() ;
          if ( Math.Abs( normal.DotProduct( p - points[ 0 ] ) ) < 0.001 ) {
            if ( Commons.IntoRegion( points, p ) >= 0 ) {
              n.kind = StbNodeKind.ON_SLAB ;
              n.id_member = s.id ;
              break ;
            }
          }
        }
      }
    }

    /// <summary>
    /// 鉄骨名称の取得と登録
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="shape2">=1:十字形のY,T形のT</param>
    /// <returns></returns>
    private static string GetSteelName( FamilySymbol symbol, int shape2 = 0, int LCR = 0 )
    {
      const string format = "0" ;
      const string mark1 = "-" ;
      const string mark2 = "*" ;


      string familyname = symbol.Family.Name ;

      if ( familyname == SetFamily.SClmH.FamilyName || familyname == SetFamily.SRCClmH.FamilyName || familyname == SetFamily.SRCClmCross.FamilyName || ( familyname == SetFamily.SRCClmT.FamilyName && shape2 == 0 ) || familyname == SetFamily.SRCClmH_Rou.FamilyName || familyname == SetFamily.SRCClmCross_Rou.FamilyName || ( familyname == SetFamily.SRCClmT_Rou.FamilyName && shape2 == 0 ) || familyname == SetFamily.SBraH.FamilyName || familyname == SetFamily.SGirH.FamilyName || familyname == SetFamily.SGirH_Haunch.FamilyName || familyname == SetFamily.SBeamH.FamilyName || familyname == SetFamily.SBeamH_Haunch.FamilyName || familyname == SetFamily.SRCGirH.FamilyName || familyname == SetFamily.SRCBeamH.FamilyName || familyname == SetFamily.SCGirH.FamilyName || familyname == SetFamily.SCGirBH.FamilyName || familyname == SetFamily.SCBeamBH.FamilyName || familyname == SetFamily.SCBeamH.FamilyName || familyname == SetFamily.SRCCGirH.FamilyName || familyname == SetFamily.SRCCBeamH.FamilyName ) {
        #region H

        StbSecRollH steel = new StbSecRollH() ;
        string type_str = "" ;
        if ( familyname == SetFamily.SClmH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SClmH.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmH.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmH.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmH.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmH.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SClmH.r ) ;
        }
        else if ( familyname == SetFamily.SRCClmH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmH.type ) ;
          if ( Data.GetParameter_string( symbol, SetFamily.SRCClmH.direction_type ) == "H" ) {
            steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmH.H ) ;
            steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmH.B ) ;
          }
          else {
            steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmH.B ) ;
            steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmH.H ) ;
          }

          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmH.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmH.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmH.r ) ;
        }
        else if ( familyname == SetFamily.SRCClmCross.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, shape2 == 0 ? SetFamily.SRCClmCross.type_X : SetFamily.SRCClmCross.type_Y ) ;
          steel.A = Data.GetParameter_double( symbol, shape2 == 0 ? SetFamily.SRCClmCross.XH : SetFamily.SRCClmCross.YH ) ;
          steel.B = Data.GetParameter_double( symbol, shape2 == 0 ? SetFamily.SRCClmCross.XB : SetFamily.SRCClmCross.YB ) ;
          steel.t1 = Data.GetParameter_double( symbol, shape2 == 0 ? SetFamily.SRCClmCross.Xt1 : SetFamily.SRCClmCross.Yt1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, shape2 == 0 ? SetFamily.SRCClmCross.Xt2 : SetFamily.SRCClmCross.Yt2 ) ;
          steel.r = Data.GetParameter_double( symbol, shape2 == 0 ? SetFamily.SRCClmCross.Xr : SetFamily.SRCClmCross.Yr ) ;
        }
        else if ( familyname == SetFamily.SRCClmT.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmT.type_H ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmT.H ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmT.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmT.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmT.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmT.r ) ;
        }
        else if ( familyname == SetFamily.SRCClmH_Rou.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmH_Rou.type ) ;
          if ( Data.GetParameter_string( symbol, SetFamily.SRCClmH_Rou.direction_type ) == "H" ) {
            steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.H ) ;
            steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.B ) ;
          }
          else {
            steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.B ) ;
            steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.H ) ;
          }

          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.r ) ;
        }
        else if ( familyname == SetFamily.SRCClmCross_Rou.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.type_X : SetFamily.SRCClmCross_Rou.type_Y ) ) ;
          steel.A = Data.GetParameter_double( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.XH : SetFamily.SRCClmCross_Rou.YH ) ) ;
          steel.B = Data.GetParameter_double( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.XB : SetFamily.SRCClmCross_Rou.YB ) ) ;
          steel.t1 = Data.GetParameter_double( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xt1 : SetFamily.SRCClmCross_Rou.Yt1 ) ) ;
          steel.t2 = Data.GetParameter_double( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xt2 : SetFamily.SRCClmCross_Rou.Yt2 ) ) ;
          steel.r = Data.GetParameter_double( symbol, ( shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xr : SetFamily.SRCClmCross_Rou.Yr ) ) ;
        }
        else if ( familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.type_H ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.H ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.r ) ;
        }
        else if ( familyname == SetFamily.SBraH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBraH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBraH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBraH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SBraH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SGirH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SGirH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SGirH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SGirH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SGirH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirH_Haunch.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SGirH_Haunch.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirH_Haunch.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SGirH_Haunch.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SGirH_Haunch.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SGirH_Haunch.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SGirH_Haunch.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBeamH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBeamH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBeamH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBeamH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SBeamH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamH_Haunch.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBeamH_Haunch.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamH_Haunch.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBeamH_Haunch.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBeamH_Haunch.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBeamH_Haunch.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SBeamH_Haunch.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SRCGirH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCGirH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCGirH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCGirH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCGirH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCGirH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCGirH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SRCBeamH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCBeamH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCBeamH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCBeamH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCBeamH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCBeamH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCBeamH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCGirH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCGirH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCGirH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCGirH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCGirH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCGirH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SCGirH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCGirBH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCGirBH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCGirBH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCGirBH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCGirBH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCGirBH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SCGirBH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCBeamBH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCBeamBH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCBeamBH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCBeamBH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCBeamBH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCBeamBH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SCBeamBH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCBeamH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCBeamH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCBeamH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCBeamH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCBeamH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCBeamH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SCBeamH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SRCCGirH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCCGirH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCCGirH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCCGirH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCCGirH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCCGirH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCCGirH.r[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SRCCBeamH.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCCBeamH.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCCBeamH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCCBeamH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCCBeamH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCCBeamH.t2[ LCR ] ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCCBeamH.r[ LCR ] ) ;
        }

        Enum.TryParse( type_str, out StbSecRollHType type ) ;
        steel.type = type ;

        if ( steel.r < 0.1 ) {
          steel.name = "BH" + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) ;

          StbSecBuildH steel2 = new StbSecBuildH()
          {
            name = steel.name,
            A = steel.A,
            B = steel.B,
            t1 = steel.t1,
            t2 = steel.t2,
          } ;

          if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecBuildH.Any( x => x.name == steel.name ) ) {
            stb.StbModel.StbSections.StbSecSteel.StbSecBuildH.Add( steel2 ) ;
          }
        }
        else {
          steel.name = steel.type + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) + mark2 + steel.r.ToString( format ) ;

          if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRollH.Any( x => x.name == steel.name ) ) {
            stb.StbModel.StbSections.StbSecSteel.StbSecRollH.Add( steel ) ;
          }
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmBH.FamilyName || familyname == SetFamily.SBraBH.FamilyName || familyname == SetFamily.SGirBH.FamilyName || familyname == SetFamily.SBeamBH.FamilyName ) {
        #region BH

        StbSecBuildH steel = new StbSecBuildH() ;
        if ( familyname == SetFamily.SClmBH.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmBH.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmBH.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmBH.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmBH.t2 ) ;
        }
        else if ( familyname == SetFamily.SBraBH.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraBH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraBH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBraBH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBraBH.t2[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirBH.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirBH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SGirBH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SGirBH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SGirBH.t2[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamBH.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamBH.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBeamBH.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBeamBH.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBeamBH.t2[ LCR ] ) ;
        }

        steel.name = "BH" + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecBuildH.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecBuildH.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmBox.FamilyName || familyname == SetFamily.CFTClmBox.FamilyName || familyname == SetFamily.SBraBox.FamilyName ) {
        #region Box

        StbSecRollBox steel = new StbSecRollBox() ;
        string type_str = "" ;
        if ( familyname == SetFamily.SClmBox.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SClmBox.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmBox.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmBox.B ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SClmBox.t1 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SClmBox.r ) ;
        }
        else if ( familyname == SetFamily.CFTClmBox.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.CFTClmBox.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.CFTClmBox.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.CFTClmBox.B ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.CFTClmBox.t ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.CFTClmBox.r1 ) ;
        }
        else if ( familyname == SetFamily.SBraBox.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBraBox.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraBox.H ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraBox.B ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SBraBox.t1 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SBraBox.r ) ;
        }

        if ( type_str == "" ) type_str = "ELSE" ;

        Enum.TryParse( type_str, out StbSecRollBoxType type ) ;
        steel.type = type ;

        if ( steel.r < 0.1 ) {
          steel.name = "BB" + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t.ToString( format ) + mark2 + steel.t.ToString( format ) ;

          StbSecBuildBox steel2 = new StbSecBuildBox()
          {
            name = steel.name,
            A = steel.A,
            B = steel.B,
            t1 = steel.t,
            t2 = steel.t,
          } ;

          if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox.Any( x => x.name == steel.name ) ) {
            stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox.Add( steel2 ) ;
          }
        }
        else {
          steel.name = steel.type + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t.ToString( format ) + mark2 + steel.r.ToString( format ) ;

          if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRollBox.Any( x => x.name == steel.name ) ) {
            stb.StbModel.StbSections.StbSecSteel.StbSecRollBox.Add( steel ) ;
          }
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmBBox.FamilyName || familyname == SetFamily.SBraBBox.FamilyName ) {
        #region BBox

        StbSecBuildBox steel = new StbSecBuildBox() ;
        if ( familyname == SetFamily.SClmBBox.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmBBox.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmBBox.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmBBox.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmBBox.t2 ) ;
        }
        else if ( familyname == SetFamily.SBraBBox.FamilyName ) {
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraBBox.H ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraBBox.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBraBBox.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBraBBox.t2 ) ;
        }

        steel.name = "BB" + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmPipe.FamilyName || familyname == SetFamily.CFTClmPipe.FamilyName || familyname == SetFamily.SBraPipe.FamilyName ) {
        #region Pipe

        StbSecPipe steel = new StbSecPipe() ;

        if ( familyname == SetFamily.SClmPipe.FamilyName ) {
          steel.D = Data.GetParameter_double( symbol, SetFamily.SClmPipe.D ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SClmPipe.t ) ;
        }
        else if ( familyname == SetFamily.CFTClmPipe.FamilyName ) {
          steel.D = Data.GetParameter_double( symbol, SetFamily.CFTClmPipe.D ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.CFTClmPipe.t ) ;
        }
        else if ( familyname == SetFamily.SBraPipe.FamilyName ) {
          steel.D = Data.GetParameter_double( symbol, SetFamily.SBraPipe.D ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SBraPipe.t ) ;
        }

        steel.name = "P" + mark1 + steel.D.ToString( format ) + mark2 + steel.t.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecPipe.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecPipe.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmT.FamilyName || familyname == SetFamily.SRCClmT.FamilyName || familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
        #region T

        StbSecRollT steel = new StbSecRollT() ;
        string type_str = "" ;
        if ( familyname == SetFamily.SClmT.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SClmT.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmT.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmT.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmT.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmT.t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SClmT.r ) ;
        }
        else if ( familyname == SetFamily.SRCClmT.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmT.type_T ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmT.CT_A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmT.CT_B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmT.CT_t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmT.CT_t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmT.CT_r ) ;
        }
        else if ( familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.type_T ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.CT_A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.CT_B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.CT_t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.CT_t2 ) ;
          steel.r = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.CT_r ) ;
        }

        Enum.TryParse( type_str, out StbSecRollTType type ) ;
        steel.type = type ;

        steel.name = steel.type + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) + mark2 + steel.r.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRollT.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecRollT.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmC.FamilyName || familyname == SetFamily.SBraC.FamilyName || familyname == SetFamily.SGirC.FamilyName || familyname == SetFamily.SBeamC.FamilyName || familyname == SetFamily.SCGirC.FamilyName || familyname == SetFamily.SCBeamC.FamilyName ) {
        #region C

        StbSecRollC steel = new StbSecRollC() ;
        string type_str = "" ;
        bool side = false ;
        if ( familyname == SetFamily.SClmC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SClmC.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmC.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmC.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmC.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmC.t2 ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SClmC.r1 ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SClmC.r2 ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SClmC.side ) ;
        }
        else if ( familyname == SetFamily.SBraC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBraC.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraC.H[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraC.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBraC.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBraC.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SBraC.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SBraC.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBraC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SGirC.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirC.H[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SGirC.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SGirC.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SGirC.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SGirC.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SGirC.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SGirC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBeamC.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamC.H[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBeamC.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBeamC.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBeamC.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SBeamC.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SBeamC.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBeamC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCGirC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCGirC.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCGirC.H[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCGirC.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCGirC.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCGirC.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SCGirC.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SCGirC.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCGirC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCBeamC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCBeamC.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCBeamC.H[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCBeamC.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCBeamC.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCBeamC.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SCBeamC.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SCBeamC.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCBeamC.side[ LCR ] ) ;
        }

        string name = "C" ;
        steel.type = StbSecRollCType.SINGLE ;
        if ( type_str == StbSecRollCType.BACKTOBACK.ToString() ) {
          steel.type = StbSecRollCType.BACKTOBACK ;
          name = "2C" ;
        }
        else if ( type_str == StbSecRollCType.FACETOFACE.ToString() ) {
          steel.type = StbSecRollCType.FACETOFACE ;
          name = "2C" ;
        }
        else if ( type_str == "2C" ) {
          steel.type = side ? StbSecRollCType.BACKTOBACK : StbSecRollCType.FACETOFACE ;
          name = "2C" ;
        }


        steel.name = name + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) + mark2 + steel.r1.ToString( format ) + mark2 + steel.r2.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRollC.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecRollC.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmL.FamilyName || familyname == SetFamily.SBraL.FamilyName || familyname == SetFamily.SGirL.FamilyName || familyname == SetFamily.SBeamL.FamilyName || familyname == SetFamily.SCGirL.FamilyName || familyname == SetFamily.SCBeamL.FamilyName ) {
        #region L

        StbSecRollL steel = new StbSecRollL() ;
        string type_str = "" ;
        bool side = false ;
        if ( familyname == SetFamily.SClmL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SClmL.type ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SClmL.A ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SClmL.B ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SClmL.t1 ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SClmL.t2 ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SClmL.r1 ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SClmL.r2 ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SClmL.side ) ;
        }
        else if ( familyname == SetFamily.SBraL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBraL.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraL.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraL.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBraL.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBraL.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SBraL.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SBraL.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBraL.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SGirL.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirL.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SGirL.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SGirL.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SGirL.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SGirL.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SGirL.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SGirL.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBeamL.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamL.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBeamL.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SBeamL.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SBeamL.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SBeamL.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SBeamL.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBeamL.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCGirL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCGirL.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCGirL.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCGirL.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCGirL.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCGirL.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SCGirL.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SCGirL.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCGirL.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCBeamL.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCBeamL.type[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCBeamL.A[ LCR ] ) ;
          steel.B = Data.GetParameter_double( symbol, SetFamily.SCBeamL.B[ LCR ] ) ;
          steel.t1 = Data.GetParameter_double( symbol, SetFamily.SCBeamL.t1[ LCR ] ) ;
          steel.t2 = Data.GetParameter_double( symbol, SetFamily.SCBeamL.t2[ LCR ] ) ;
          steel.r1 = Data.GetParameter_double( symbol, SetFamily.SCBeamL.r1[ LCR ] ) ;
          steel.r2 = Data.GetParameter_double( symbol, SetFamily.SCBeamL.r2[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCBeamL.side[ LCR ] ) ;
        }


        string name = "L" ;
        steel.type = StbSecRollLType.SINGLE ;
        if ( type_str == StbSecRollLType.BACKTOBACK.ToString() ) {
          steel.type = StbSecRollLType.BACKTOBACK ;
          name = "2L" ;
        }
        else if ( type_str == StbSecRollLType.FACETOFACE.ToString() ) {
          steel.type = StbSecRollLType.FACETOFACE ;
          name = "2L" ;
        }
        else if ( type_str == "2L" ) {
          steel.type = side ? StbSecRollLType.BACKTOBACK : StbSecRollLType.FACETOFACE ;
          name = "2L" ;
        }

        steel.name = name + mark1 + steel.A.ToString( format ) + mark2 + steel.B.ToString( format ) + mark2 + steel.t1.ToString( format ) + mark2 + steel.t2.ToString( format ) + mark2 + steel.r1.ToString( format ) + mark2 + steel.r2.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRollL.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecRollL.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SBraLipC.FamilyName || familyname == SetFamily.SGirLipC.FamilyName || familyname == SetFamily.SBeamLipC.FamilyName || familyname == SetFamily.SCGirLipC.FamilyName || familyname == SetFamily.SCBeamLipC.FamilyName ) {
        #region LipC

        StbSecLipC steel = new StbSecLipC() ;
        string type_str = "" ;
        bool side = false ;
        if ( familyname == SetFamily.SBraLipC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBraLipC.type[ LCR ] ) ;
          steel.H = Data.GetParameter_double( symbol, SetFamily.SBraLipC.H[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBraLipC.A[ LCR ] ) ;
          steel.C = Data.GetParameter_double( symbol, SetFamily.SBraLipC.C[ LCR ] ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SBraLipC.t[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBraLipC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SGirLipC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SGirLipC.type[ LCR ] ) ;
          steel.H = Data.GetParameter_double( symbol, SetFamily.SGirLipC.H[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SGirLipC.A[ LCR ] ) ;
          steel.C = Data.GetParameter_double( symbol, SetFamily.SGirLipC.C[ LCR ] ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SGirLipC.t[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SGirLipC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SBeamLipC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SBeamLipC.type[ LCR ] ) ;
          steel.H = Data.GetParameter_double( symbol, SetFamily.SBeamLipC.H[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SBeamLipC.A[ LCR ] ) ;
          steel.C = Data.GetParameter_double( symbol, SetFamily.SBeamLipC.C[ LCR ] ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SBeamLipC.t[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SBeamLipC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCGirLipC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCGirLipC.type[ LCR ] ) ;
          steel.H = Data.GetParameter_double( symbol, SetFamily.SCGirLipC.H[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCGirLipC.A[ LCR ] ) ;
          steel.C = Data.GetParameter_double( symbol, SetFamily.SCGirLipC.C[ LCR ] ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SCGirLipC.t[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCGirLipC.side[ LCR ] ) ;
        }
        else if ( familyname == SetFamily.SCBeamLipC.FamilyName ) {
          type_str = Data.GetParameter_string( symbol, SetFamily.SCBeamLipC.type[ LCR ] ) ;
          steel.H = Data.GetParameter_double( symbol, SetFamily.SCBeamLipC.H[ LCR ] ) ;
          steel.A = Data.GetParameter_double( symbol, SetFamily.SCBeamLipC.A[ LCR ] ) ;
          steel.C = Data.GetParameter_double( symbol, SetFamily.SCBeamLipC.C[ LCR ] ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SCBeamLipC.t[ LCR ] ) ;
          side = Data.GetParameter_bool( symbol, SetFamily.SCBeamLipC.side[ LCR ] ) ;
        }

        string name = "C" ;
        steel.type = StbSecLipCType.SINGLE ;
        if ( type_str == StbSecLipCType.BACKTOBACK.ToString() ) {
          steel.type = StbSecLipCType.BACKTOBACK ;
          name = "2C" ;
        }
        else if ( type_str == StbSecLipCType.FACETOFACE.ToString() ) {
          steel.type = StbSecLipCType.FACETOFACE ;
          name = "2C" ;
        }
        else if ( type_str == "2C" ) {
          steel.type = side ? StbSecLipCType.BACKTOBACK : StbSecLipCType.FACETOFACE ;
          name = "2C" ;
        }

        steel.name = name + mark1 + steel.H.ToString( format ) + mark2 + steel.A.ToString( format ) + mark2 + steel.C.ToString( format ) + mark2 + steel.t.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecLipC.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecLipC.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SBraFB.FamilyName ) {
        #region FB

        StbSecFlatBar steel = new StbSecFlatBar() ;
        if ( familyname == SetFamily.SBraFB.FamilyName ) {
          steel.B = Data.GetParameter_double( symbol, SetFamily.SBraFB.B ) ;
          steel.t = Data.GetParameter_double( symbol, SetFamily.SBraFB.t ) ;
        }

        steel.name = "FB" + mark1 + steel.B.ToString( format ) + mark2 + steel.t.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecFlatBar.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecFlatBar.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }
      else if ( familyname == SetFamily.SBraRollBar.FamilyName ) {
        #region Bar

        StbSecRoundBar steel = new StbSecRoundBar() ;
        if ( familyname == SetFamily.SBraRollBar.FamilyName ) {
          steel.R = Data.GetParameter_double( symbol, SetFamily.SBraRollBar.D ) ;
        }

        steel.name = "R" + mark1 + steel.R.ToString( format ) ;

        if ( ! stb.StbModel.StbSections.StbSecSteel.StbSecRoundBar.Any( x => x.name == steel.name ) ) {
          stb.StbModel.StbSections.StbSecSteel.StbSecRoundBar.Add( steel ) ;
        }

        return steel.name ;

        #endregion
      }

      return "" ;
    }


    /// <summary>
    /// Commonの出力
    /// </summary>
    private static void Export_Common()
    {
      stb.StbCommon = new StbCommon() { app_name = RevitLNK.formtitle + " " + RevitLNK.RevitVersion, StbReinforcementStrengthList = new List<StbReinforcementStrength>(), StbApplyConditionsList = new StbApplyConditionsList(), } ;


      ProjectInfo pinfo = Commons.doc.ProjectInformation ;
      for ( int i = 0 ; i < Data.projectParams.Count() ; ++i ) {
        Parameter p = pinfo.LookupParameter( Data.projectParams[ i ] ) ;
        if ( p == null ) continue ;

        switch ( i ) {
          case 0 : //"STBファイル名"
            break ;
          case 1 : //"STBファイル更新日時"
            break ;
          case 2 : //"STBレベルマッピング設定"
            break ;
          case 3 : //"STB基点位置設定"
            break ;
          case 4 : //"STBコンクリート設定"
            break ;
          case 5 : //"STB鉄骨設定"
            break ;
          case 6 : //"STBグローバルID"
            if ( Guid.TryParse( p.AsString() ?? "", out Guid guid ) ) {
              stb.StbCommon.guid = guid.ToString( "N" ) ;
            }
            else {
              stb.StbCommon.guid = Guid.NewGuid().ToString( "N" ) ;
            }

            break ;
          case 7 : //"STBプロジェクト名"
            stb.StbCommon.project_name = p.AsString() ;
            break ;
          case 8 : //"STBアプリケーション名"
            break ;
          case 9 : //"STB建物全体のコンクリート強度"
            stb.StbCommon.strength_concrete = p.AsString() ;
            break ;
          case 10 : //"STB鉄骨規格"
            break ;
          case 11 : //"STB径別鉄筋強度情報"
            if ( p.AsString() != null ) {
              string[] Reinforcement = p.AsString().Split( ',' ) ;
              for ( int j = 0 ; j < Reinforcement.Length - 1 ; j += 2 ) {
                stb.StbCommon.StbReinforcementStrengthList.Add( new StbReinforcementStrength() { D = Reinforcement[ j ].Trim(), strength = Reinforcement[ j + 1 ].Trim() } ) ;
              }
            }

            break ;
        }
      }

      if ( stb.StbCommon.project_name == "" ) {
        stb.StbCommon.project_name = Path.GetFileNameWithoutExtension( Commons.doc.PathName ) ;
      }

      ReadApplyData() ;
    }


    /// <summary>
    /// 拡張ストレージからStbApplyConditionsListの情報を読む
    /// </summary>
    private static void ReadApplyData()
    {
      try {
        var schema = Data.GetSchema( Data.schemaName_StbCommon ) ;
        if ( schema != null && schema.ReadAccessGranted() ) {
          var entity = Commons.doc.ProjectInformation.GetEntity( schema ) ;
          if ( entity != null && entity.IsValid() && entity.ReadAccessGranted() ) {
            string name = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply ) ;
            var field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply = new StbColumn_RC_RebarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.interval ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.interval = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.center = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.length_to_center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply.length_to_center = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply = new StbColumn_RC_BarSpacingApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.D_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.D_bar_spacing = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.pitch_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.pitch_bar_spacing = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.N_bar_spacing_X ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.N_bar_spacing_X = data[ keyname ].ToInt() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.N_bar_spacing_Y ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply.N_bar_spacing_Y = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply = new StbColumn_SRC_RebarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.interval ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.interval = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.center = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.length_to_center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply.length_to_center = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply = new StbColumn_SRC_BarSpacingApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.D_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.D_bar_spacing = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.pitch_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.pitch_bar_spacing = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.N_bar_spacing_X ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.N_bar_spacing_X = data[ keyname ].ToInt() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.N_bar_spacing_Y ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply.N_bar_spacing_Y = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply = new StbBeam_RC_RebarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.depth_cover_side ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.depth_cover_side = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.depth_cover_top_bottom ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.depth_cover_top_bottom = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.interval ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.interval = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.center_side ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.center_side = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.center_top_bottom ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.center_top_bottom = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.length_to_center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply.length_to_center = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply = new StbBeam_RC_BarWebApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.D_web ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.D_web = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.N_web ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply.N_web = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply = new StbBeam_RC_BarSpacingApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.D_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.D_bar_spacing = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.pitch_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.pitch_bar_spacing = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.N_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply.N_bar_spacing = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply = new StbBeam_SRC_RebarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.depth_cover_side ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.depth_cover_side = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.depth_cover_top_bottom ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.depth_cover_top_bottom = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.interval ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.interval = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.center_side ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.center_side = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.center_top_bottom ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.center_top_bottom = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.length_to_center ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply.length_to_center = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply = new StbBeam_SRC_BarWebApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.D_web ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.D_web = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.N_web ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply.N_web = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply = new StbBeam_SRC_BarSpacingApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.D_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.D_bar_spacing = data[ keyname ] ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.pitch_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.pitch_bar_spacing = data[ keyname ].ToDouble() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.N_bar_spacing ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply.N_bar_spacing = data[ keyname ].ToInt() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply = new StbSlab_RC_BarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply = new StbWall_RC_BarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply = new StbFoundation_RC_BarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply = new StbPile_RC_BarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }
            }


            name = nameof( stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply ) ;
            field = schema.GetField( name ) ;
            if ( field != null && field.ContainerType == ContainerType.Map ) {
              var data = entity.Get<IDictionary<string, string>>( field ) ;

              stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply = new StbParapet_RC_BarPositionApply() ;
              string keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply.set_default ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply.set_default = data[ keyname ].ToBool() ;
              }

              keyname = nameof( stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply.depth_cover ) ;
              if ( data.ContainsKey( keyname ) ) {
                stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply.depth_cover = data[ keyname ].ToDouble() ;
              }
            }
          }
        }
      }
      catch {
      }
    }


    /// <summary>
    /// 通り芯の出力
    /// </summary>
    private static void Export_Grid()
    {
      stb.StbModel.StbAxes = new StbAxes() { StbParallelAxes = new List<StbParallelAxes>(), } ;

      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_GridChains ) ;
      List<MultiSegmentGrid> multiGrids = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<MultiSegmentGrid>().ToList() ;

      collector = new FilteredElementCollector( Commons.doc ) ;
      filter = new ElementCategoryFilter( BuiltInCategory.OST_Grids ) ;
      var Grids = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<Grid>().ToList() ;

      //複数セグメントの部品は除外する
      foreach ( var mg in multiGrids ) {
        var gridids = mg.GetGridIds() ;
        Grids.RemoveAll( a => gridids.Contains( a.Id ) ) ;
      }


      //GridからLineを取得するローカル関数
      Line Grid_to_Line( ElementId eid )
      {
        if ( eid != null ) {
          if ( Commons.doc.GetElement( eid ) is Grid gr ) {
            if ( ! gr.IsCurved && gr.Curve is Line ln ) {
              return ln ;
            }
          }
        }

        return null ;
      }

      //GridからLineを取得するローカル関数
      Arc Grid_to_Arc( ElementId eid )
      {
        if ( eid != null ) {
          if ( Commons.doc.GetElement( eid ) is Grid gr ) {
            if ( gr.IsCurved && gr.Curve is Arc ln ) {
              return ln ;
            }
          }
        }

        return null ;
      }


      for ( int m = multiGrids.Count - 1 ; m >= 0 ; --m ) {
        Line line = Grid_to_Line( multiGrids[ m ].GetGridIds().FirstOrDefault() ) ;
        if ( line == null ) {
          multiGrids.RemoveAt( m ) ;
        }
      }


      List<List<MultiSegmentGrid>> gridGroups = new List<List<MultiSegmentGrid>>() ;
      while ( multiGrids.Count > 0 ) {
        List<MultiSegmentGrid> gridGroup = new List<MultiSegmentGrid>() ;

        Line line0 = Grid_to_Line( multiGrids[ 0 ].GetGridIds().FirstOrDefault() ) ;
        XYZ vec1 = XYZ.BasisX ;
        if ( XYZ.BasisX.CrossProduct( line0.Direction ).GetLength() < 0.001 ) {
          vec1 = XYZ.BasisY ;
        }

        gridGroup.Add( multiGrids[ 0 ] ) ;

        for ( int m = 1 ; m < multiGrids.Count ; ++m ) {
          Line line1 = Grid_to_Line( multiGrids[ m ].GetGridIds().FirstOrDefault() ) ;
          if ( line0.Direction.CrossProduct( line1.Direction ).GetLength() < 0.001 ) {
            bool add = true ;
            for ( int m2 = 0 ; m2 < gridGroup.Count ; ++m2 ) {
              Line line2 = Grid_to_Line( multiGrids[ m2 ].GetGridIds().FirstOrDefault() ) ;
              if ( vec1.DotProduct( ( line1.GetEndPoint( 0 ) - line2.GetEndPoint( 0 ) ).Normalize() ) < 0 ) {
                add = false ;
                gridGroup.Insert( m2, multiGrids[ m ] ) ;
                break ;
              }
            }

            if ( add ) {
              gridGroup.Add( multiGrids[ m ] ) ;
            }
          }
        }

        gridGroups.Add( gridGroup ) ;
        multiGrids.RemoveAll( a => gridGroup.Select( b => b.Id ).ToList().Contains( a.Id ) ) ;
      }


      XYZ origin = new XYZ() ;
      GridInfo = new List<Data.GridInformation>[ gridGroups.Count ] ;

      for ( int i = 0 ; i < gridGroups.Count ; ++i ) {
        var grp = gridGroups[ i ] ;
        GridInfo[ i ] = new List<Data.GridInformation>() ;

        StbParallelAxes axes = new StbParallelAxes() { X = Commons.ft2mm( origin.X ), Y = Commons.ft2mm( origin.Y ), StbParallelAxis = new List<StbParallelAxis>(), } ;

        Line line0 = Grid_to_Line( grp[ 0 ].GetGridIds().FirstOrDefault() ) ;
        XYZ vec1 = XYZ.BasisX ;
        int xy = 0 ;
        if ( XYZ.BasisX.CrossProduct( line0.Direction ).GetLength() < 0.001 ) {
          vec1 = XYZ.BasisY ;
          xy = 1 ;
        }

        XYZ vec2 = vec1.CrossProduct( XYZ.BasisZ ).Normalize() ;

        var prefix = grp.Select( a => a.Name.Substring( 0, 1 ) ).Distinct().ToList() ;
        if ( prefix.Count == 1 ) {
          //最初の一文字がそろっていれば、それをグループ名にする
          axes.group_name = prefix[ 0 ] ;
        }
        else {
          if ( xy == 0 ) {
            axes.group_name = "X" ;
          }
          else {
            axes.group_name = "Y" ;
          }
        }

        axes.angle = XYZ.BasisX.AngleOnPlaneTo( vec2, XYZ.BasisZ ) * 180 / Math.PI ;

        foreach ( var gr in grp ) {
          Line line1 = Grid_to_Line( gr.GetGridIds().FirstOrDefault() ) ;

          id++ ;

          StbParallelAxis axis = new StbParallelAxis()
          {
            id = id,
            guid = GetGuid( gr, "" ),
            name = gr.Name,
            distance = Commons.ft2mm( vec1.DotProduct( line1.GetEndPoint( 0 ) - origin ) ),
            StbNodeIdList = new List<StbNodeId>(),
          } ;

          axes.StbParallelAxis.Add( axis ) ;

          Data.AddLog( Data.LogCode.grid, gr, id, 0 ) ;


          //NodeList用に各Gridの情報を収集
          foreach ( ElementId eid in gr.GetGridIds() ) {
            var grid = Commons.doc.GetElement( eid ) as Grid ;

            Data.GridInformation gi = new Data.GridInformation()
            {
              stb_id = id,
              gr = grid,
              multiGridID = gr.Id,
              ps = Commons.ft2mm( grid.Curve.GetEndPoint( 0 ) ),
              pe = Commons.ft2mm( grid.Curve.GetEndPoint( 1 ) ),
            } ;

            //Z座標は使わない
            gi.ps = new XYZ( gi.ps.X, gi.ps.Y, 0 ) ;
            gi.pe = new XYZ( gi.pe.X, gi.pe.Y, 0 ) ;

            GridInfo[ i ].Add( gi ) ;
          }
        }


        stb.StbModel.StbAxes.StbParallelAxes.Add( axes ) ;
      }


      //円弧軸、放射軸
      if ( Grids.Count > 0 ) {
        List<Grid> grids_arc = new List<Grid>() ;
        foreach ( var g in Grids ) {
          if ( Grid_to_Arc( g.Id ) != null ) {
            grids_arc.Add( g ) ;
          }
        }

        //円弧軸
        if ( grids_arc.Count > 0 ) {
          //円弧軸を除外
          Grids.RemoveAll( a => grids_arc.Select( b => b.Id ).ToList().Contains( a.Id ) ) ;

          stb.StbModel.StbAxes.StbArcAxes = new List<StbArcAxes>() ;
          foreach ( var g in grids_arc ) {
            var arc = Grid_to_Arc( g.Id ) ;
            var center = Commons.ft2mm( arc.Center ) ;
            XYZ p0 = Commons.ft2mm( arc.GetEndPoint( 0 ) ) ;
            XYZ p1 = Commons.ft2mm( arc.GetEndPoint( 1 ) ) ;
            XYZ v0 = ( p0 - center ).Normalize() ;
            XYZ v1 = ( p1 - center ).Normalize() ;
            double angle0 = XYZ.BasisX.AngleOnPlaneTo( v0, XYZ.BasisZ ) * 180 / Math.PI ;
            double angle1 = XYZ.BasisX.AngleOnPlaneTo( v1, XYZ.BasisZ ) * 180 / Math.PI ;


            id++ ;
            StbArcAxis arcaxis = new StbArcAxis()
            {
              id = id,
              guid = GetGuid( g, "" ),
              name = g.Name,
              radius = Commons.ft2mm( arc.Radius ),
              StbNodeIdList = new List<StbNodeId>(),
            } ;

            StbArcAxes arcs = null ;
            if ( stb.StbModel.StbAxes.StbArcAxes.Count > 0 ) {
              arcs = stb.StbModel.StbAxes.StbArcAxes.Find( a => new XYZ( a.X, a.Y, center.Z ).DistanceTo( center ) < 1 ) ;
            }

            if ( arcs != null ) {
              arcs.start_angle = Math.Min( arcs.start_angle, Math.Min( angle0, angle1 ) ) ;
              arcs.end_angle = Math.Max( arcs.end_angle, Math.Max( angle0, angle1 ) ) ;
              arcs.StbArcAxis.Add( arcaxis ) ;
            }
            else {
              arcs = new StbArcAxes()
              {
                group_name = g.Name.Substring( 0, 1 ),
                X = center.X,
                Y = center.Y,
                start_angle = Math.Min( angle0, angle1 ),
                end_angle = Math.Max( angle0, angle1 ),
                StbArcAxis = new List<StbArcAxis>() { arcaxis, },
              } ;

              stb.StbModel.StbAxes.StbArcAxes.Add( arcs ) ;
            }
          }
        }

        //放射軸
        if ( Grids.Count > 2 && stb.StbModel.StbAxes.StbArcAxes != null && stb.StbModel.StbAxes.StbArcAxes.Count > 0 ) {
          //放射軸は円弧軸とのペアであることを前提とする。
          foreach ( var g in Grids ) {
            var line = Grid_to_Line( g.Id ) ;
            XYZ p0 = Commons.ft2mm( line.GetEndPoint( 0 ) ) ;
            XYZ p1 = Commons.ft2mm( line.GetEndPoint( 1 ) ) ;
            foreach ( var arcAxes in stb.StbModel.StbAxes.StbArcAxes ) {
              XYZ center = new XYZ( arcAxes.X, arcAxes.Y, p0.Z ) ;
              XYZ v0 = ( p0 - center ).Normalize() ;
              XYZ v1 = ( p1 - center ).Normalize() ;
              if ( v0.CrossProduct( v1 ).GetLength() < 0.0001 ) {
                //円弧軸と同じ中心を通る
                double angle0 = XYZ.BasisX.AngleOnPlaneTo( v0, XYZ.BasisZ ) * 180 / Math.PI ;

                id++ ;
                StbRadialAxis radialaxis = new StbRadialAxis()
                {
                  id = id,
                  guid = GetGuid( g, "" ),
                  name = g.Name,
                  angle = angle0,
                  StbNodeIdList = new List<StbNodeId>(),
                } ;


                StbRadialAxes radials = null ;
                if ( stb.StbModel.StbAxes.StbRadialAxes.Count > 0 ) {
                  radials = stb.StbModel.StbAxes.StbRadialAxes.Find( a => new XYZ( a.X, a.Y, center.Z ).DistanceTo( center ) < 1 ) ;
                }

                if ( radials != null ) {
                  radials.StbRadialAxis.Add( radialaxis ) ;
                }
                else {
                  radials = new StbRadialAxes() { group_name = g.Name.Substring( 0, 1 ), X = center.X, Y = center.Y, StbRadialAxis = new List<StbRadialAxis>() { radialaxis, } } ;

                  stb.StbModel.StbAxes.StbRadialAxes.Add( radials ) ;
                }

                break ;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// レベルの出力
    /// </summary>
    private static void Export_Level()
    {
      stb.StbModel.StbStories = new List<StbStory>() ;


      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_Levels ) ;
      Levels = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<Level>().OrderBy( x => x.Elevation ).ToList() ;

      Level GL = Levels.Find( x => x.Name == "GL" ) ;

      for ( int L = 0 ; L < Levels.Count ; ++L ) {
        //GLは除外
        if ( Levels[ L ].Name == "GL" ) continue ;

        id++ ;
        StbStory s = new StbStory()
        {
          id = id,
          guid = GetGuid( Levels[ L ], "" ),
          name = Levels[ L ].Name,
          height = Commons.ft2mm( Levels[ L ].Elevation ),
          kind = StbStoryKind.GENERAL,
          StbNodeIdList = new List<StbNodeId>(),
        } ;

        if ( L == Levels.Count - 1 ) {
          s.kind = StbStoryKind.ROOF ;
        }
        else if ( Levels[ L ].Elevation - ( GL?.Elevation ?? 0 ) < -0.00001 ) {
          s.kind = StbStoryKind.BASEMENT ;
        }

        stb.StbModel.StbStories.Add( s ) ;

        Data.AddLog( Data.LogCode.level, Levels[ L ], id, 0 ) ;
      }
    }


    #region 柱

    /// <summary>
    /// 柱脚情報の取得
    /// </summary>
    /// <param name="eid">柱ID</param>
    /// <param name="ps">柱の座標</param>
    /// <returns></returns>
    private static StbSecBaseProduct_S GetBaseProduct( ElementId eid, XYZ ps )
    {
      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      XYZ p0 = Commons.mm2ft( ps ) ;
      XYZ range = new XYZ( 0.05, 0.05, 0.05 ) ;
      XYZ p1 = p0 - range ;
      XYZ p2 = p0 + range ;
      BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter( new Outline( p1, p2 ) ) ;
      List<FamilyInstance> BaseProduct = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where( x => x.Id != eid ).ToList() ;

      for ( int i = 0 ; i < BaseProduct.Count ; ++i ) {
        var bp = new StbSecBaseProduct_S() { product_company = Data.GetParameter_string( BaseProduct[ i ].Symbol, BuiltInParameter.ALL_MODEL_MANUFACTURER ), product_code = BaseProduct[ i ].Symbol.Name, height_mortar = 50, } ;

        if ( bp.product_company != "" ) {
          return bp ;
        }
      }

      return null ;
    }

    /// <summary>
    /// 柱断面の出力
    /// </summary>
    /// <param name="ins"></param>
    /// <returns>id_section</returns>
    private static int Export_SecColumn( FamilyInstance ins, XYZ ps )
    {
      FamilySymbol symbol = ins.Symbol ;
      string floor = Levels.Find( x => x.Id == ins.LevelId ).Name ;

      int retID = -1 ;

      string familyname = symbol.Family.Name ;
      if ( familyname == SetFamily.RCClmRe.FamilyName || familyname == SetFamily.RCClmRo.FamilyName ) {
        id_sect++ ;

        #region RC柱

        StbSecColumn_RC s = new StbSecColumn_RC() { id = id_sect, guid = GetGuid( symbol, "" ), floor = floor, } ;

        List<object> barItems = new List<object>() ;
        string[] paramName = new string[ 23 ] ;
        if ( familyname == SetFamily.RCClmRe.FamilyName ) {
          #region RC角柱

          paramName[ 0 ] = SetFamily.RCClmRe.name ;
          paramName[ 1 ] = SetFamily.RCClmRe.kind_column ;
          paramName[ 2 ] = SetFamily.RCClmRe.D_reinforcement_main[ 0 ] ; //柱脚太筋径
          paramName[ 3 ] = SetFamily.RCClmRe.D_reinforcement_2nd_main[ 0 ] ; //柱脚細筋径
          paramName[ 4 ] = SetFamily.RCClmRe.D_reinforcement_axial ;
          paramName[ 5 ] = SetFamily.RCClmRe.D_reinforcement_band[ 0 ] ; //柱脚
          paramName[ 6 ] = SetFamily.RCClmRe.D_bar_spacing ;
          paramName[ 7 ] = SetFamily.RCClmRe.strength_concrete ;
          paramName[ 8 ] = SetFamily.RCClmRe.strength_reinforcement_main ;
          paramName[ 9 ] = SetFamily.RCClmRe.strength_reinforcement_2nd_main ;
          paramName[ 10 ] = SetFamily.RCClmRe.strength_reinforcement_axial ;
          paramName[ 11 ] = SetFamily.RCClmRe.strength_reinforcement_band ;
          paramName[ 12 ] = SetFamily.RCClmRe.strength_bar_spacing ;
          paramName[ 13 ] = SetFamily.RCClmRe.depth_cover_X[ 0 ] ; //始
          paramName[ 14 ] = SetFamily.RCClmRe.depth_cover_X[ 1 ] ; //終
          paramName[ 15 ] = SetFamily.RCClmRe.depth_cover_Y[ 0 ] ; //始
          paramName[ 16 ] = SetFamily.RCClmRe.depth_cover_Y[ 1 ] ; //終
          paramName[ 17 ] = SetFamily.RCClmRe.kind_reinforcement_corner[ 0 ] ; //柱脚
          paramName[ 18 ] = SetFamily.RCClmRe.interval_reinforcement ;
          paramName[ 19 ] = SetFamily.RCClmRe.center_reinforcement_start_X ;
          paramName[ 20 ] = SetFamily.RCClmRe.center_reinforcement_end_X ;
          paramName[ 21 ] = SetFamily.RCClmRe.center_reinforcement_start_Y ;
          paramName[ 22 ] = SetFamily.RCClmRe.center_reinforcement_end_Y ;


          //形状
          StbSecColumn_RC_Rect rect = new StbSecColumn_RC_Rect() { width_X = Data.GetParameter_double( symbol, SetFamily.RCClmRe.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.RCClmRe.DY ), } ;
          s.StbSecFigureColumn_RC = new StbSecFigureColumn_RC() { Item = rect, } ;


          //配筋
          List<StbSecBarColumn_RC_RectSame> bar = new List<StbSecBarColumn_RC_RectSame>() ;
          for ( int b = 0 ; b < SetFamily.RCClmRe.count_main_X_1st.Length ; ++b ) {
            var bb = new StbSecBarColumn_RC_RectSame()
            {
              D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
              D_2nd_main = Data.GetParameter_string( symbol, paramName[ 3 ] ),
              D_axial = Data.GetParameter_string( symbol, paramName[ 4 ] ),
              D_band = Data.GetParameter_string( symbol, paramName[ 5 ] ),
              D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 6 ] ),
              strength_main = Data.GetParameter_string( symbol, paramName[ 8 ] ),
              strength_2nd_main = Data.GetParameter_string( symbol, paramName[ 9 ] ),
              strength_axial = Data.GetParameter_string( symbol, paramName[ 10 ] ),
              strength_band = Data.GetParameter_string( symbol, paramName[ 11 ] ),
              strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 12 ] ),
              N_main_X_1st = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_main_X_1st[ b ] ),
              N_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_main_X_2nd[ b ] ),
              N_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_main_Y_1st[ b ] ),
              N_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_main_Y_2nd[ b ] ),
              N_2nd_main_X_1st = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_2nd_main_X_1st[ b ] ),
              N_2nd_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_2nd_main_X_2nd[ b ] ),
              N_2nd_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_2nd_main_Y_1st[ b ] ),
              N_2nd_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_2nd_main_Y_2nd[ b ] ),
              N_main_total = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_main_total ),
              N_axial = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_axial[ b ] ),
              N_band_direction_X = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_band_dir_X[ b ] ),
              N_band_direction_Y = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_band_dir_Y[ b ] ),
              N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_bar_spacing_X[ b ] ),
              N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.RCClmRe.count_bar_spacing_Y[ b ] ),
              pitch_band = Data.GetParameter_double( symbol, SetFamily.RCClmRe.pitch_band[ b ] ),
              pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.RCClmRe.pitch_bar_spacing[ b ] ),
            } ;
            bar.Add( bb ) ;
          }

          bool isSame = true ;
          isSame &= bar[ 0 ].D_main == bar[ 1 ].D_main ;
          isSame &= bar[ 0 ].D_2nd_main == bar[ 1 ].D_2nd_main ;
          isSame &= bar[ 0 ].D_axial == bar[ 1 ].D_axial ;
          isSame &= bar[ 0 ].D_band == bar[ 1 ].D_band ;
          isSame &= bar[ 0 ].D_bar_spacing == bar[ 1 ].D_bar_spacing ;
          isSame &= bar[ 0 ].strength_main == bar[ 1 ].strength_main ;
          isSame &= bar[ 0 ].strength_2nd_main == bar[ 1 ].strength_2nd_main ;
          isSame &= bar[ 0 ].strength_axial == bar[ 1 ].strength_axial ;
          isSame &= bar[ 0 ].strength_band == bar[ 1 ].strength_band ;
          isSame &= bar[ 0 ].strength_bar_spacing == bar[ 1 ].strength_bar_spacing ;
          isSame &= bar[ 0 ].N_main_X_1st == bar[ 1 ].N_main_X_1st ;
          isSame &= bar[ 0 ].N_main_X_2nd == bar[ 1 ].N_main_X_2nd ;
          isSame &= bar[ 0 ].N_main_Y_1st == bar[ 1 ].N_main_Y_1st ;
          isSame &= bar[ 0 ].N_main_Y_2nd == bar[ 1 ].N_main_Y_2nd ;
          isSame &= bar[ 0 ].N_2nd_main_X_1st == bar[ 1 ].N_2nd_main_X_1st ;
          isSame &= bar[ 0 ].N_2nd_main_X_2nd == bar[ 1 ].N_2nd_main_X_2nd ;
          isSame &= bar[ 0 ].N_2nd_main_Y_1st == bar[ 1 ].N_2nd_main_Y_1st ;
          isSame &= bar[ 0 ].N_2nd_main_Y_2nd == bar[ 1 ].N_2nd_main_Y_2nd ;
          isSame &= bar[ 0 ].N_main_total == bar[ 1 ].N_main_total ;
          isSame &= bar[ 0 ].N_axial == bar[ 1 ].N_axial ;
          isSame &= bar[ 0 ].N_band_direction_X == bar[ 1 ].N_band_direction_X ;
          isSame &= bar[ 0 ].N_band_direction_Y == bar[ 1 ].N_band_direction_Y ;
          isSame &= bar[ 0 ].N_bar_spacing_X == bar[ 1 ].N_bar_spacing_X ;
          isSame &= bar[ 0 ].N_bar_spacing_Y == bar[ 1 ].N_bar_spacing_Y ;
          isSame &= Math.Abs( bar[ 0 ].pitch_band - bar[ 1 ].pitch_band ) < 0.1 ;
          isSame &= Math.Abs( bar[ 0 ].pitch_bar_spacing - bar[ 1 ].pitch_bar_spacing ) < 0.1 ;

          if ( isSame ) {
            barItems.Add( bar[ 0 ] ) ;
          }
          else {
            barItems.Add( new StbSecBarColumn_RC_RectNotSame( bar[ 0 ] ) { pos = StbSecBarColumn_RC_NotSamePos.BASE } ) ;
            barItems.Add( new StbSecBarColumn_RC_RectNotSame( bar[ 1 ] ) { pos = StbSecBarColumn_RC_NotSamePos.TOP } ) ;
          }

          #endregion
        }
        else {
          #region RC円柱

          paramName[ 0 ] = SetFamily.RCClmRo.name ;
          paramName[ 1 ] = SetFamily.RCClmRo.kind_column ;
          paramName[ 2 ] = SetFamily.RCClmRo.D_reinforcement_main[ 0 ] ; //柱脚太筋径
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = SetFamily.RCClmRo.D_reinforcement_axial ;
          paramName[ 5 ] = SetFamily.RCClmRo.D_reinforcement_band[ 0 ] ; //柱脚
          paramName[ 6 ] = SetFamily.RCClmRo.D_bar_spacing ;
          paramName[ 7 ] = SetFamily.RCClmRo.strength_concrete ;
          paramName[ 8 ] = SetFamily.RCClmRo.strength_reinforcement_main ;
          paramName[ 9 ] = "" ;
          paramName[ 10 ] = SetFamily.RCClmRo.strength_reinforcement_axial ;
          paramName[ 11 ] = SetFamily.RCClmRo.strength_reinforcement_band ;
          paramName[ 12 ] = SetFamily.RCClmRo.strength_bar_spacing ;
          paramName[ 13 ] = SetFamily.RCClmRo.depth_cover_X ;
          paramName[ 14 ] = "" ;
          paramName[ 15 ] = "" ;
          paramName[ 16 ] = "" ;
          paramName[ 17 ] = "" ;
          paramName[ 18 ] = "" ;
          paramName[ 19 ] = SetFamily.RCClmRo.center_reinforcement_start_X ;
          paramName[ 20 ] = "" ;
          paramName[ 21 ] = "" ;
          paramName[ 22 ] = "" ;


          //形状
          StbSecColumn_RC_Circle circle = new StbSecColumn_RC_Circle() { D = Data.GetParameter_double( symbol, SetFamily.RCClmRo.D ) } ;
          s.StbSecFigureColumn_RC = new StbSecFigureColumn_RC() { Item = circle, } ;


          //配筋
          List<StbSecBarColumn_RC_CircleSame> bar = new List<StbSecBarColumn_RC_CircleSame>() ;
          for ( int b = 0 ; b < SetFamily.RCClmRo.count_main.Length ; ++b ) {
            var bb = new StbSecBarColumn_RC_CircleSame()
            {
              D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
              D_axial = Data.GetParameter_string( symbol, paramName[ 4 ] ),
              D_band = Data.GetParameter_string( symbol, paramName[ 5 ] ),
              D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 6 ] ),
              strength_main = Data.GetParameter_string( symbol, paramName[ 8 ] ),
              strength_axial = Data.GetParameter_string( symbol, paramName[ 10 ] ),
              strength_band = Data.GetParameter_string( symbol, paramName[ 11 ] ),
              strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 12 ] ),
              N_main = Data.GetParameter_int( symbol, SetFamily.RCClmRo.count_main[ b ] ),
              N_axial = Data.GetParameter_int( symbol, SetFamily.RCClmRo.count_axial[ b ] ),
              N_band = Data.GetParameter_int( symbol, SetFamily.RCClmRo.count_band[ b ] ),
              N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.RCClmRo.count_bar_spacing_X[ b ] ),
              N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.RCClmRo.count_bar_spacing_Y[ b ] ),
              pitch_band = Data.GetParameter_double( symbol, SetFamily.RCClmRo.pitch_band[ b ] ),
              pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.RCClmRo.pitch_bar_spacing[ b ] ),
            } ;
            bar.Add( bb ) ;
          }

          bool isSame = true ;
          isSame &= bar[ 0 ].D_main == bar[ 1 ].D_main ;
          isSame &= bar[ 0 ].D_axial == bar[ 1 ].D_axial ;
          isSame &= bar[ 0 ].D_band == bar[ 1 ].D_band ;
          isSame &= bar[ 0 ].D_bar_spacing == bar[ 1 ].D_bar_spacing ;
          isSame &= bar[ 0 ].strength_main == bar[ 1 ].strength_main ;
          isSame &= bar[ 0 ].strength_axial == bar[ 1 ].strength_axial ;
          isSame &= bar[ 0 ].strength_band == bar[ 1 ].strength_band ;
          isSame &= bar[ 0 ].strength_bar_spacing == bar[ 1 ].strength_bar_spacing ;
          isSame &= bar[ 0 ].N_main == bar[ 1 ].N_main ;
          isSame &= bar[ 0 ].N_axial == bar[ 1 ].N_axial ;
          isSame &= bar[ 0 ].N_band == bar[ 1 ].N_band ;
          isSame &= bar[ 0 ].N_bar_spacing_X == bar[ 1 ].N_bar_spacing_X ;
          isSame &= bar[ 0 ].N_bar_spacing_Y == bar[ 1 ].N_bar_spacing_Y ;
          isSame &= Math.Abs( bar[ 0 ].pitch_band - bar[ 1 ].pitch_band ) < 0.1 ;
          isSame &= Math.Abs( bar[ 0 ].pitch_bar_spacing - bar[ 1 ].pitch_bar_spacing ) < 0.1 ;

          if ( isSame ) {
            barItems.Add( bar[ 0 ] ) ;
          }
          else {
            barItems.Add( new StbSecBarColumn_RC_CircleNotSame( bar[ 0 ] ) { pos = StbSecBarColumn_RC_NotSamePos.BASE } ) ;
            barItems.Add( new StbSecBarColumn_RC_CircleNotSame( bar[ 1 ] ) { pos = StbSecBarColumn_RC_NotSamePos.TOP } ) ;
          }

          #endregion
        }

        s.name = Data.GetParameter_string( symbol, paramName[ 0 ] ) ;
        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 1 ] ).ToUpper(), out StbSecColumn_Kind_column kind ) ;
        s.kind_column = kind ;

        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 17 ] ), out StbSecBarArrangementColumn_RCKind_corner corner ) ;


        s.StbSecBarArrangementColumn_RC = new StbSecBarArrangementColumn_RC()
        {
          depth_cover_start_X = Data.GetParameter_double( symbol, paramName[ 13 ] ),
          depth_cover_end_X = Data.GetParameter_double( symbol, paramName[ 14 ] ),
          depth_cover_start_Y = Data.GetParameter_double( symbol, paramName[ 15 ] ),
          depth_cover_end_Y = Data.GetParameter_double( symbol, paramName[ 16 ] ),
          interval = Data.GetParameter_double( symbol, paramName[ 18 ] ),
          center_start_X = Data.GetParameter_double( symbol, paramName[ 19 ] ),
          center_end_X = Data.GetParameter_double( symbol, paramName[ 20 ] ),
          center_start_Y = Data.GetParameter_double( symbol, paramName[ 21 ] ),
          center_end_Y = Data.GetParameter_double( symbol, paramName[ 22 ] ),
          kind_corner = corner,
          Items = barItems,
        } ;

        s.strength_concrete = Data.GetParameter_string( symbol, paramName[ 7 ] ) ;
        s.strength_concrete = Data.GetConcreteFC( s.strength_concrete ) ;

        stb.StbModel.StbSections.StbSecColumn_RC.Add( s ) ;
        retID = s.id ;

        #endregion
      }
      else if ( familyname == SetFamily.SClmH.FamilyName || familyname == SetFamily.SClmBH.FamilyName || familyname == SetFamily.SClmBox.FamilyName || familyname == SetFamily.SClmBBox.FamilyName || familyname == SetFamily.SClmPipe.FamilyName || familyname == SetFamily.SClmT.FamilyName || familyname == SetFamily.SClmC.FamilyName || familyname == SetFamily.SClmL.FamilyName ) {
        id_sect++ ;

        #region S柱

        StbSecColumn_S s = new StbSecColumn_S()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isReferenceDirection = true, //鉄骨向き：回転角度からでは判別できないのでfalseにしてそのまま角度を出力する
          StbSecSteelFigureColumn_S = new StbSecSteelFigureColumn_S() { base_type = StbSecSteelFigureColumn_SBase_type.NONE, Items = new List<object>(), },
        } ;

        var steel = new StbSecSteelColumn_S_Same() { shape = GetSteelName( symbol ), } ;

        if ( steel.shape == "" ) return retID ;

        string[] paramName = new string[ 5 ] ;
        if ( familyname == SetFamily.SClmH.FamilyName ) {
          #region H

          paramName[ 0 ] = SetFamily.SClmH.name ;
          paramName[ 1 ] = SetFamily.SClmH.kind_column ;
          paramName[ 2 ] = SetFamily.SClmH.base_type ;
          paramName[ 3 ] = SetFamily.SClmH.strength_main ;
          paramName[ 4 ] = SetFamily.SClmH.strength_web ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBH.FamilyName ) {
          #region BH

          paramName[ 0 ] = SetFamily.SClmBH.name ;
          paramName[ 1 ] = SetFamily.SClmBH.kind_column ;
          paramName[ 2 ] = SetFamily.SClmBH.base_type ;
          paramName[ 3 ] = SetFamily.SClmBH.strength_main ;
          paramName[ 4 ] = SetFamily.SClmBH.strength_web ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBox.FamilyName ) {
          #region Box

          paramName[ 0 ] = SetFamily.SClmBox.name ;
          paramName[ 1 ] = SetFamily.SClmBox.kind_column ;
          paramName[ 2 ] = SetFamily.SClmBox.base_type ;
          paramName[ 3 ] = SetFamily.SClmBox.strength_main ;
          paramName[ 4 ] = "" ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBBox.FamilyName ) {
          #region BBox

          paramName[ 0 ] = SetFamily.SClmBBox.name ;
          paramName[ 1 ] = SetFamily.SClmBBox.kind_column ;
          paramName[ 2 ] = SetFamily.SClmBBox.base_type ;
          paramName[ 3 ] = SetFamily.SClmBBox.strength_main ;
          paramName[ 4 ] = "" ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmPipe.FamilyName ) {
          #region Pipe

          paramName[ 0 ] = SetFamily.SClmPipe.name ;
          paramName[ 1 ] = SetFamily.SClmPipe.kind_column ;
          paramName[ 2 ] = SetFamily.SClmPipe.base_type ;
          paramName[ 3 ] = SetFamily.SClmPipe.strength_main ;
          paramName[ 4 ] = "" ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmT.FamilyName ) {
          #region T

          paramName[ 0 ] = SetFamily.SClmT.name ;
          paramName[ 1 ] = SetFamily.SClmT.kind_column ;
          paramName[ 2 ] = SetFamily.SClmT.base_type ;
          paramName[ 3 ] = SetFamily.SClmT.strength_main ;
          paramName[ 4 ] = SetFamily.SClmT.strength_web ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmC.FamilyName ) {
          #region C

          paramName[ 0 ] = SetFamily.SClmC.name ;
          paramName[ 1 ] = SetFamily.SClmC.kind_column ;
          paramName[ 2 ] = SetFamily.SClmC.base_type ;
          paramName[ 3 ] = SetFamily.SClmC.strength_main ;
          paramName[ 4 ] = "" ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmL.FamilyName ) {
          #region L

          paramName[ 0 ] = SetFamily.SClmL.name ;
          paramName[ 1 ] = SetFamily.SClmL.kind_column ;
          paramName[ 2 ] = SetFamily.SClmL.base_type ;
          paramName[ 3 ] = SetFamily.SClmL.strength_main ;
          paramName[ 4 ] = "" ;

          #endregion
        }
        else {
          return retID ;
        }

        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 1 ] ).ToUpper(), out StbSecColumn_Kind_column kind ) ;

        s.name = Data.GetParameter_string( symbol, paramName[ 0 ] ) ;
        s.kind_column = kind ;
        steel.strength_main = Data.GetParameter_string( symbol, paramName[ 3 ] ) ;
        steel.strength_web = Data.GetParameter_string( symbol, paramName[ 4 ] ) ;

        s.StbSecSteelFigureColumn_S.Items.Add( steel ) ;

        string base_type = Data.GetParameter_string( symbol, paramName[ 2 ] ) ;
        if ( base_type != "" ) {
          if ( Enum.TryParse( base_type, out StbSecSteelFigureColumn_SBase_type base_Type2 ) ) {
            if ( base_Type2 != StbSecSteelFigureColumn_SBase_type.NONE ) {
              s.StbSecSteelFigureColumn_S.base_type = base_Type2 ;
              s.Item = GetBaseProduct( ins.Id, ps ) ;
            }
          }
        }

        stb.StbModel.StbSections.StbSecColumn_S.Add( s ) ;
        retID = s.id ;

        #endregion
      }
      else if ( familyname == SetFamily.SRCClmH.FamilyName || familyname == SetFamily.SRCClmCross.FamilyName || familyname == SetFamily.SRCClmT.FamilyName || familyname == SetFamily.SRCClmH_Rou.FamilyName || familyname == SetFamily.SRCClmCross_Rou.FamilyName || familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
        id_sect++ ;

        #region SRC柱

        StbSecColumn_SRC s = new StbSecColumn_SRC()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          StbSecFigureColumn_SRC = new StbSecFigureColumn_SRC(),
          StbSecBarArrangementColumn_SRC = new StbSecBarArrangementColumn_SRC(),
          StbSecSteelFigureColumn_SRC = new StbSecSteelFigureColumn_SRC() { base_type = StbSecSteelFigureColumn_SRCBase_type.NONE, Items = new List<object>(), },
        } ;

        string shape = GetSteelName( symbol ) ;
        if ( shape == "" ) return retID ;


        List<object> barItems = new List<object>() ;
        string[] paramName = new string[ 18 ] ;
        if ( familyname == SetFamily.SRCClmH.FamilyName || familyname == SetFamily.SRCClmCross.FamilyName || familyname == SetFamily.SRCClmT.FamilyName ) {
          #region 矩形

          var bar = new List<StbSecBarColumn_SRC_RectSame>() ;

          if ( familyname == SetFamily.SRCClmH.FamilyName ) {
            #region SRC柱H形矩形

            paramName[ 0 ] = SetFamily.SRCClmH.name ;
            paramName[ 1 ] = SetFamily.SRCClmH.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmH.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = SetFamily.SRCClmH.D_reinforcement_2nd_main[ 0 ] ; //柱脚細筋径
            paramName[ 4 ] = SetFamily.SRCClmH.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmH.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmH.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmH.strength_reinforcement_main ;
            paramName[ 8 ] = SetFamily.SRCClmH.strength_reinforcement_2nd_main ;
            paramName[ 9 ] = SetFamily.SRCClmH.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmH.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmH.depth_cover_X[ 0 ] ; //始
            paramName[ 12 ] = SetFamily.SRCClmH.depth_cover_X[ 1 ] ; //終
            paramName[ 13 ] = SetFamily.SRCClmH.depth_cover_Y[ 0 ] ; //始
            paramName[ 14 ] = SetFamily.SRCClmH.depth_cover_Y[ 1 ] ; //終
            paramName[ 15 ] = SetFamily.SRCClmH.kind_reinforcement_corner[ 0 ] ; //柱脚
            paramName[ 16 ] = SetFamily.SRCClmH.interval_reinforcement ;
            paramName[ 17 ] = SetFamily.SRCClmH.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Rect() { width_X = Data.GetParameter_double( symbol, SetFamily.SRCClmH.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.SRCClmH.DY ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmH.count_main_X_1st.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_RectSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_2nd_main = Data.GetParameter_string( symbol, paramName[ 3 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_2nd_main = Data.GetParameter_string( symbol, paramName[ 8 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_main_X_1st[ b ] ),
                N_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_main_X_2nd[ b ] ),
                N_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_main_Y_1st[ b ] ),
                N_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_main_Y_2nd[ b ] ),
                N_2nd_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_2nd_main_X_1st[ b ] ),
                N_2nd_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_2nd_main_X_2nd[ b ] ),
                N_2nd_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_2nd_main_Y_1st[ b ] ),
                N_2nd_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_2nd_main_Y_2nd[ b ] ),
                N_main_total = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_main_total ),
                N_axial = 0,
                N_band_direction_X = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_band_dir_X[ b ] ),
                N_band_direction_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_band_dir_Y[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmH.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmH.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmH.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }

            //S形状
            var steel = new StbSecColumn_SRC_SameShapeH()
            {
              shape = shape,
              strength_main = Data.GetParameter_string( symbol, SetFamily.SRCClmH.strength_main ),
              strength_web = Data.GetParameter_string( symbol, SetFamily.SRCClmH.strength_web ),
              offset_X = Data.GetParameter_double( symbol, SetFamily.SRCClmH.offset_X ),
              offset_Y = Data.GetParameter_double( symbol, SetFamily.SRCClmH.offset_Y ),
            } ;

            if ( Data.GetParameter_string( symbol, SetFamily.SRCClmH.direction_type ) == "H" ) {
              steel.direction_type = StbSecColumn_SRC_SameShapeHDirection_type.H ;
            }
            else {
              steel.direction_type = StbSecColumn_SRC_SameShapeHDirection_type.I ;
            }

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else if ( familyname == SetFamily.SRCClmCross.FamilyName ) {
            #region SRC柱+形矩形

            paramName[ 0 ] = SetFamily.SRCClmCross.name ;
            paramName[ 1 ] = SetFamily.SRCClmCross.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmCross.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = SetFamily.SRCClmCross.D_reinforcement_2nd_main[ 0 ] ; //柱脚細筋径
            paramName[ 4 ] = SetFamily.SRCClmCross.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmCross.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmCross.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmCross.strength_reinforcement_main ;
            paramName[ 8 ] = SetFamily.SRCClmCross.strength_reinforcement_2nd_main ;
            paramName[ 9 ] = SetFamily.SRCClmCross.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmCross.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmCross.depth_cover_X[ 0 ] ; //始
            paramName[ 12 ] = SetFamily.SRCClmCross.depth_cover_X[ 1 ] ; //終
            paramName[ 13 ] = SetFamily.SRCClmCross.depth_cover_Y[ 0 ] ; //始
            paramName[ 14 ] = SetFamily.SRCClmCross.depth_cover_Y[ 1 ] ; //終
            paramName[ 15 ] = SetFamily.SRCClmCross.kind_reinforcement_corner[ 0 ] ; //柱脚
            paramName[ 16 ] = SetFamily.SRCClmCross.interval_reinforcement ;
            paramName[ 17 ] = SetFamily.SRCClmCross.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Rect() { width_X = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.DY ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmCross.count_main_X_1st.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_RectSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_2nd_main = Data.GetParameter_string( symbol, paramName[ 3 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_2nd_main = Data.GetParameter_string( symbol, paramName[ 8 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_main_X_1st[ b ] ),
                N_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_main_X_2nd[ b ] ),
                N_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_main_Y_1st[ b ] ),
                N_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_main_Y_2nd[ b ] ),
                N_2nd_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_2nd_main_X_1st[ b ] ),
                N_2nd_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_2nd_main_X_2nd[ b ] ),
                N_2nd_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_2nd_main_Y_1st[ b ] ),
                N_2nd_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_2nd_main_Y_2nd[ b ] ),
                N_main_total = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_main_total ),
                N_axial = 0,
                N_band_direction_X = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_band_dir_X[ b ] ),
                N_band_direction_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_band_dir_Y[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmCross.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }

            //S形状
            var steel = new StbSecColumn_SRC_SameShapeCross()
            {
              shape_X = shape,
              shape_Y = GetSteelName( symbol, 1 ),
              strength_main_X = Data.GetParameter_string( symbol, SetFamily.SRCClmCross.strength_main_X ),
              strength_main_Y = Data.GetParameter_string( symbol, SetFamily.SRCClmCross.strength_main_Y ),
              strength_web_X = Data.GetParameter_string( symbol, SetFamily.SRCClmCross.strength_web_X ),
              strength_web_Y = Data.GetParameter_string( symbol, SetFamily.SRCClmCross.strength_web_Y ),
              offset_XX = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.offset_XX ),
              offset_XY = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.offset_XY ),
              offset_YX = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.offset_YX ),
              offset_YY = Data.GetParameter_double( symbol, SetFamily.SRCClmCross.offset_YY ),
            } ;

            if ( steel.shape_Y == "" ) return retID ;

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else if ( familyname == SetFamily.SRCClmT.FamilyName ) {
            #region SRC柱T形矩形

            paramName[ 0 ] = SetFamily.SRCClmT.name ;
            paramName[ 1 ] = SetFamily.SRCClmT.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmT.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = SetFamily.SRCClmT.D_reinforcement_2nd_main[ 0 ] ; //柱脚細筋径
            paramName[ 4 ] = SetFamily.SRCClmT.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmT.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmT.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmT.strength_reinforcement_main ;
            paramName[ 8 ] = SetFamily.SRCClmT.strength_reinforcement_2nd_main ;
            paramName[ 9 ] = SetFamily.SRCClmT.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmT.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmT.depth_cover_X[ 0 ] ; //始
            paramName[ 12 ] = SetFamily.SRCClmT.depth_cover_X[ 1 ] ; //終
            paramName[ 13 ] = SetFamily.SRCClmT.depth_cover_Y[ 0 ] ; //始
            paramName[ 14 ] = SetFamily.SRCClmT.depth_cover_Y[ 1 ] ; //終
            paramName[ 15 ] = SetFamily.SRCClmT.kind_reinforcement_corner[ 0 ] ; //柱脚
            paramName[ 16 ] = SetFamily.SRCClmT.interval_reinforcement ;
            paramName[ 17 ] = SetFamily.SRCClmT.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Rect() { width_X = Data.GetParameter_double( symbol, SetFamily.SRCClmT.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.SRCClmT.DY ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmT.count_main_X_1st.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_RectSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_2nd_main = Data.GetParameter_string( symbol, paramName[ 3 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_2nd_main = Data.GetParameter_string( symbol, paramName[ 8 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_main_X_1st[ b ] ),
                N_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_main_X_2nd[ b ] ),
                N_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_main_Y_1st[ b ] ),
                N_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_main_Y_2nd[ b ] ),
                N_2nd_main_X_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_2nd_main_X_1st[ b ] ),
                N_2nd_main_X_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_2nd_main_X_2nd[ b ] ),
                N_2nd_main_Y_1st = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_2nd_main_Y_1st[ b ] ),
                N_2nd_main_Y_2nd = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_2nd_main_Y_2nd[ b ] ),
                N_main_total = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_main_total ),
                N_axial = 0,
                N_band_direction_X = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_band_dir_X[ b ] ),
                N_band_direction_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_band_dir_Y[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmT.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmT.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmT.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }


            //S形状
            var steel = new StbSecColumn_SRC_SameShapeT()
            {
              shape_H = shape,
              shape_T = GetSteelName( symbol, 1 ),
              strength_main_H = Data.GetParameter_string( symbol, SetFamily.SRCClmT.strength_main_H ),
              strength_main_T = Data.GetParameter_string( symbol, SetFamily.SRCClmT.strength_main_T ),
              strength_web_H = Data.GetParameter_string( symbol, SetFamily.SRCClmT.strength_web_H ),
              strength_web_T = Data.GetParameter_string( symbol, SetFamily.SRCClmT.strength_web_T ),
              offset_HX = Data.GetParameter_double( symbol, SetFamily.SRCClmT.offset_HX ),
              offset_HY = Data.GetParameter_double( symbol, SetFamily.SRCClmT.offset_HY ),
              offset_T = Data.GetParameter_double( symbol, SetFamily.SRCClmT.offset_T ),
            } ;

            if ( steel.shape_T == "" ) return retID ;

            Enum.TryParse( Data.GetParameter_string( symbol, SetFamily.SRCClmT.direction_type ), out StbSecColumn_SRC_SameShapeTDirection_type direction_Type ) ;
            steel.direction_type = direction_Type ;

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else {
            return retID ;
          }

          bool isSame = true ;
          isSame &= bar[ 0 ].D_main == bar[ 1 ].D_main ;
          isSame &= bar[ 0 ].D_2nd_main == bar[ 1 ].D_2nd_main ;
          isSame &= bar[ 0 ].D_axial == bar[ 1 ].D_axial ;
          isSame &= bar[ 0 ].D_band == bar[ 1 ].D_band ;
          isSame &= bar[ 0 ].D_bar_spacing == bar[ 1 ].D_bar_spacing ;
          isSame &= bar[ 0 ].strength_main == bar[ 1 ].strength_main ;
          isSame &= bar[ 0 ].strength_2nd_main == bar[ 1 ].strength_2nd_main ;
          isSame &= bar[ 0 ].strength_axial == bar[ 1 ].strength_axial ;
          isSame &= bar[ 0 ].strength_band == bar[ 1 ].strength_band ;
          isSame &= bar[ 0 ].strength_bar_spacing == bar[ 1 ].strength_bar_spacing ;
          isSame &= bar[ 0 ].N_main_X_1st == bar[ 1 ].N_main_X_1st ;
          isSame &= bar[ 0 ].N_main_X_2nd == bar[ 1 ].N_main_X_2nd ;
          isSame &= bar[ 0 ].N_main_Y_1st == bar[ 1 ].N_main_Y_1st ;
          isSame &= bar[ 0 ].N_main_Y_2nd == bar[ 1 ].N_main_Y_2nd ;
          isSame &= bar[ 0 ].N_2nd_main_X_1st == bar[ 1 ].N_2nd_main_X_1st ;
          isSame &= bar[ 0 ].N_2nd_main_X_2nd == bar[ 1 ].N_2nd_main_X_2nd ;
          isSame &= bar[ 0 ].N_2nd_main_Y_1st == bar[ 1 ].N_2nd_main_Y_1st ;
          isSame &= bar[ 0 ].N_2nd_main_Y_2nd == bar[ 1 ].N_2nd_main_Y_2nd ;
          isSame &= bar[ 0 ].N_main_total == bar[ 1 ].N_main_total ;
          isSame &= bar[ 0 ].N_axial == bar[ 1 ].N_axial ;
          isSame &= bar[ 0 ].N_band_direction_X == bar[ 1 ].N_band_direction_X ;
          isSame &= bar[ 0 ].N_band_direction_Y == bar[ 1 ].N_band_direction_Y ;
          isSame &= bar[ 0 ].N_bar_spacing_X == bar[ 1 ].N_bar_spacing_X ;
          isSame &= bar[ 0 ].N_bar_spacing_Y == bar[ 1 ].N_bar_spacing_Y ;
          isSame &= Math.Abs( bar[ 0 ].pitch_band - bar[ 1 ].pitch_band ) < 0.1 ;
          isSame &= Math.Abs( bar[ 0 ].pitch_bar_spacing - bar[ 1 ].pitch_bar_spacing ) < 0.1 ;

          if ( isSame ) {
            barItems.Add( bar[ 0 ] ) ;
          }
          else {
            barItems.Add( new StbSecBarColumn_SRC_RectNotSame( bar[ 0 ] ) { pos = StbSecBarColumn_RC_NotSamePos.BASE } ) ;
            barItems.Add( new StbSecBarColumn_SRC_RectNotSame( bar[ 1 ] ) { pos = StbSecBarColumn_RC_NotSamePos.TOP } ) ;
          }

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmH_Rou.FamilyName || familyname == SetFamily.SRCClmCross_Rou.FamilyName || familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
          #region 円形

          var bar = new List<StbSecBarColumn_SRC_CircleSame>() ;

          if ( familyname == SetFamily.SRCClmH_Rou.FamilyName ) {
            #region SRC柱H形円形

            paramName[ 0 ] = SetFamily.SRCClmH_Rou.name ;
            paramName[ 1 ] = SetFamily.SRCClmH_Rou.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmH_Rou.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = "" ;
            paramName[ 4 ] = SetFamily.SRCClmH_Rou.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmH_Rou.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmH_Rou.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmH_Rou.strength_reinforcement_main ;
            paramName[ 8 ] = "" ;
            paramName[ 9 ] = SetFamily.SRCClmH_Rou.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmH_Rou.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmH_Rou.depth_cover_X ;
            paramName[ 12 ] = "" ;
            paramName[ 13 ] = "" ;
            paramName[ 14 ] = "" ;
            paramName[ 15 ] = "" ;
            paramName[ 16 ] = "" ;
            paramName[ 17 ] = SetFamily.SRCClmH_Rou.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Circle() { D = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.D ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmH_Rou.count_main.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_CircleSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main = Data.GetParameter_int( symbol, SetFamily.SRCClmH_Rou.count_main[ b ] ),
                N_axial = 0,
                N_band = Data.GetParameter_int( symbol, SetFamily.SRCClmH_Rou.count_band[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmH_Rou.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmH_Rou.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }

            //S形状
            var steel = new StbSecColumn_SRC_SameShapeH()
            {
              shape = shape,
              strength_main = Data.GetParameter_string( symbol, SetFamily.SRCClmH_Rou.strength_main ),
              strength_web = Data.GetParameter_string( symbol, SetFamily.SRCClmH_Rou.strength_web ),
              offset_X = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.offset_X ),
              offset_Y = Data.GetParameter_double( symbol, SetFamily.SRCClmH_Rou.offset_Y ),
            } ;

            if ( Data.GetParameter_string( symbol, SetFamily.SRCClmH.direction_type ) == "H" ) {
              steel.direction_type = StbSecColumn_SRC_SameShapeHDirection_type.H ;
            }
            else {
              steel.direction_type = StbSecColumn_SRC_SameShapeHDirection_type.I ;
            }

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else if ( familyname == SetFamily.SRCClmCross_Rou.FamilyName ) {
            #region SRC柱+形円形

            paramName[ 0 ] = SetFamily.SRCClmCross_Rou.name ;
            paramName[ 1 ] = SetFamily.SRCClmCross_Rou.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmCross_Rou.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = "" ;
            paramName[ 4 ] = SetFamily.SRCClmCross_Rou.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmCross_Rou.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmCross_Rou.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmCross_Rou.strength_reinforcement_main ;
            paramName[ 8 ] = "" ;
            paramName[ 9 ] = SetFamily.SRCClmCross_Rou.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmCross_Rou.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmCross_Rou.depth_cover_X ;
            paramName[ 12 ] = "" ;
            paramName[ 13 ] = "" ;
            paramName[ 14 ] = "" ;
            paramName[ 15 ] = "" ;
            paramName[ 16 ] = "" ;
            paramName[ 17 ] = SetFamily.SRCClmCross_Rou.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Circle() { D = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.D ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmCross_Rou.count_main.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_CircleSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main = Data.GetParameter_int( symbol, SetFamily.SRCClmCross_Rou.count_main[ b ] ),
                N_axial = 0,
                N_band = Data.GetParameter_int( symbol, SetFamily.SRCClmCross_Rou.count_band[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmCross_Rou.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmCross_Rou.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }

            //S形状
            var steel = new StbSecColumn_SRC_SameShapeCross()
            {
              shape_X = shape,
              shape_Y = GetSteelName( symbol, 1 ),
              strength_main_X = Data.GetParameter_string( symbol, SetFamily.SRCClmCross_Rou.strength_main_X ),
              strength_main_Y = Data.GetParameter_string( symbol, SetFamily.SRCClmCross_Rou.strength_main_Y ),
              strength_web_X = Data.GetParameter_string( symbol, SetFamily.SRCClmCross_Rou.strength_web_X ),
              strength_web_Y = Data.GetParameter_string( symbol, SetFamily.SRCClmCross_Rou.strength_web_Y ),
              offset_XX = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.offset_XX ),
              offset_XY = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.offset_XY ),
              offset_YX = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.offset_YX ),
              offset_YY = Data.GetParameter_double( symbol, SetFamily.SRCClmCross_Rou.offset_YY ),
            } ;

            if ( steel.shape_Y == "" ) return retID ;

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else if ( familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
            #region SRC柱T形円形

            paramName[ 0 ] = SetFamily.SRCClmT_Rou.name ;
            paramName[ 1 ] = SetFamily.SRCClmT_Rou.kind_column ;
            paramName[ 2 ] = SetFamily.SRCClmT_Rou.D_reinforcement_main[ 0 ] ; //柱脚太筋径
            paramName[ 3 ] = "" ;
            paramName[ 4 ] = SetFamily.SRCClmT_Rou.D_reinforcement_band[ 0 ] ; //柱脚
            paramName[ 5 ] = SetFamily.SRCClmT_Rou.D_bar_spacing ;
            paramName[ 6 ] = SetFamily.SRCClmT_Rou.strength_concrete ;
            paramName[ 7 ] = SetFamily.SRCClmT_Rou.strength_reinforcement_main ;
            paramName[ 8 ] = "" ;
            paramName[ 9 ] = SetFamily.SRCClmT_Rou.strength_reinforcement_band ;
            paramName[ 10 ] = SetFamily.SRCClmT_Rou.strength_bar_spacing ;
            paramName[ 11 ] = SetFamily.SRCClmT_Rou.depth_cover_X ;
            paramName[ 12 ] = "" ;
            paramName[ 13 ] = "" ;
            paramName[ 14 ] = "" ;
            paramName[ 15 ] = "" ;
            paramName[ 16 ] = "" ;
            paramName[ 17 ] = SetFamily.SRCClmT_Rou.base_type ;


            //RC形状
            s.StbSecFigureColumn_SRC.Item = new StbSecColumn_SRC_Circle() { D = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.D ) } ;

            //配筋
            for ( int b = 0 ; b < SetFamily.SRCClmT_Rou.count_main.Length ; ++b ) {
              var bb = new StbSecBarColumn_SRC_CircleSame()
              {
                D_main = Data.GetParameter_string( symbol, paramName[ 2 ] ),
                D_band = Data.GetParameter_string( symbol, paramName[ 4 ] ),
                D_bar_spacing = Data.GetParameter_string( symbol, paramName[ 5 ] ),
                D_axial = "",
                strength_main = Data.GetParameter_string( symbol, paramName[ 7 ] ),
                strength_band = Data.GetParameter_string( symbol, paramName[ 9 ] ),
                strength_bar_spacing = Data.GetParameter_string( symbol, paramName[ 10 ] ),
                strength_axial = "",
                N_main = Data.GetParameter_int( symbol, SetFamily.SRCClmT_Rou.count_main[ b ] ),
                N_axial = 0,
                N_band = Data.GetParameter_int( symbol, SetFamily.SRCClmT_Rou.count_band[ b ] ),
                N_bar_spacing_X = Data.GetParameter_int( symbol, SetFamily.SRCClmT_Rou.count_bar_spacing_X[ b ] ),
                N_bar_spacing_Y = Data.GetParameter_int( symbol, SetFamily.SRCClmT_Rou.count_bar_spacing_Y[ b ] ),
                pitch_band = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.pitch_band[ b ] ),
                pitch_bar_spacing = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.pitch_bar_spacing[ b ] ),
              } ;
              bar.Add( bb ) ;
            }

            //S形状
            var steel = new StbSecColumn_SRC_SameShapeT()
            {
              shape_H = shape,
              shape_T = GetSteelName( symbol, 1 ),
              strength_main_H = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.strength_main_H ),
              strength_main_T = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.strength_main_T ),
              strength_web_H = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.strength_web_H ),
              strength_web_T = Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.strength_web_T ),
              offset_HX = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.offset_HX ),
              offset_HY = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.offset_HY ),
              offset_T = Data.GetParameter_double( symbol, SetFamily.SRCClmT_Rou.offset_T ),
            } ;

            if ( steel.shape_T == "" ) return retID ;

            Enum.TryParse( Data.GetParameter_string( symbol, SetFamily.SRCClmT_Rou.direction_type ), out StbSecColumn_SRC_SameShapeTDirection_type direction_Type ) ;
            steel.direction_type = direction_Type ;

            s.StbSecSteelFigureColumn_SRC.Items.Add( new StbSecSteelColumn_SRC_Same() { Item = steel, } ) ;

            #endregion
          }
          else {
            return retID ;
          }


          bool isSame = true ;
          isSame &= bar[ 0 ].D_main == bar[ 1 ].D_main ;
          isSame &= bar[ 0 ].D_axial == bar[ 1 ].D_axial ;
          isSame &= bar[ 0 ].D_band == bar[ 1 ].D_band ;
          isSame &= bar[ 0 ].D_bar_spacing == bar[ 1 ].D_bar_spacing ;
          isSame &= bar[ 0 ].strength_main == bar[ 1 ].strength_main ;
          isSame &= bar[ 0 ].strength_axial == bar[ 1 ].strength_axial ;
          isSame &= bar[ 0 ].strength_band == bar[ 1 ].strength_band ;
          isSame &= bar[ 0 ].strength_bar_spacing == bar[ 1 ].strength_bar_spacing ;
          isSame &= bar[ 0 ].N_main == bar[ 1 ].N_main ;
          isSame &= bar[ 0 ].N_axial == bar[ 1 ].N_axial ;
          isSame &= bar[ 0 ].N_band == bar[ 1 ].N_band ;
          isSame &= bar[ 0 ].N_bar_spacing_X == bar[ 1 ].N_bar_spacing_X ;
          isSame &= bar[ 0 ].N_bar_spacing_Y == bar[ 1 ].N_bar_spacing_Y ;
          isSame &= Math.Abs( bar[ 0 ].pitch_band - bar[ 1 ].pitch_band ) < 0.1 ;
          isSame &= Math.Abs( bar[ 0 ].pitch_bar_spacing - bar[ 1 ].pitch_bar_spacing ) < 0.1 ;

          if ( isSame ) {
            barItems.Add( bar[ 0 ] ) ;
          }
          else {
            barItems.Add( new StbSecBarColumn_SRC_CircleNotSame( bar[ 0 ] ) { pos = StbSecBarColumn_RC_NotSamePos.BASE } ) ;
            barItems.Add( new StbSecBarColumn_SRC_CircleNotSame( bar[ 1 ] ) { pos = StbSecBarColumn_RC_NotSamePos.TOP } ) ;
          }

          #endregion
        }
        else {
          return retID ;
        }


        s.name = Data.GetParameter_string( symbol, paramName[ 0 ] ) ;
        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 1 ] ).ToUpper(), out StbSecColumn_Kind_column kind ) ;
        s.kind_column = kind ;

        s.strength_concrete = Data.GetParameter_string( symbol, paramName[ 6 ] ) ;
        s.strength_concrete = Data.GetConcreteFC( s.strength_concrete ) ;

        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 15 ] ), out StbSecBarArrangementColumn_RCKind_corner corner ) ;

        s.StbSecBarArrangementColumn_SRC = new StbSecBarArrangementColumn_SRC()
        {
          depth_cover_start_X = Data.GetParameter_double( symbol, paramName[ 11 ] ),
          depth_cover_end_X = Data.GetParameter_double( symbol, paramName[ 12 ] ),
          depth_cover_start_Y = Data.GetParameter_double( symbol, paramName[ 13 ] ),
          depth_cover_end_Y = Data.GetParameter_double( symbol, paramName[ 14 ] ),
          interval = Data.GetParameter_double( symbol, paramName[ 16 ] ),
          kind_corner = corner,
          Items = barItems,
        } ;


        string base_type = Data.GetParameter_string( symbol, paramName[ 17 ] ) ;
        if ( base_type != "" ) {
          if ( Enum.TryParse( base_type, out StbSecSteelFigureColumn_SRCBase_type base_Type2 ) ) {
            if ( base_Type2 != StbSecSteelFigureColumn_SRCBase_type.NONE ) {
              s.StbSecSteelFigureColumn_SRC.base_type = base_Type2 ;

              var bp = GetBaseProduct( ins.Id, ps ) ;
              if ( bp != null ) {
                s.Item = (StbSecBaseProduct_SRC)bp ;
              }
            }
          }
        }

        stb.StbModel.StbSections.StbSecColumn_SRC.Add( s ) ;
        retID = s.id ;

        #endregion
      }
      else if ( familyname == SetFamily.CFTClmBox.FamilyName || familyname == SetFamily.CFTClmPipe.FamilyName ) {
        id_sect++ ;

        #region CFT柱

        StbSecColumn_CFT s = new StbSecColumn_CFT()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isReferenceDirection = false,
          StbSecSteelFigureColumn_CFT = new StbSecSteelFigureColumn_CFT() { Items = new List<object>(), },
        } ;

        var steel = new StbSecSteelColumn_CFT_Same() { shape = GetSteelName( symbol ), } ;

        if ( steel.shape == "" ) return retID ;


        string[] paramName = new string[ 6 ] ;
        if ( familyname == SetFamily.CFTClmBox.FamilyName ) {
          #region CFT柱角形鋼管

          paramName[ 0 ] = SetFamily.CFTClmBox.name ;
          paramName[ 1 ] = SetFamily.CFTClmBox.kind_column ;
          paramName[ 2 ] = SetFamily.CFTClmBox.strength_concrete ;
          paramName[ 3 ] = SetFamily.CFTClmBox.direction_type ;
          paramName[ 4 ] = SetFamily.CFTClmBox.base_type ;
          paramName[ 5 ] = SetFamily.CFTClmBox.enbedded_length ;

          steel.strength = Data.GetParameter_string( symbol, SetFamily.CFTClmBox.strength_main ) ;

          #endregion
        }
        else if ( familyname == SetFamily.CFTClmPipe.FamilyName ) {
          #region CFT柱鋼管

          paramName[ 0 ] = SetFamily.CFTClmPipe.name ;
          paramName[ 1 ] = SetFamily.CFTClmPipe.kind_column ;
          paramName[ 2 ] = SetFamily.CFTClmPipe.strength_concrete ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = SetFamily.CFTClmPipe.base_type ;
          paramName[ 5 ] = SetFamily.CFTClmPipe.enbedded_length ;

          steel.strength = Data.GetParameter_string( symbol, SetFamily.CFTClmPipe.strength_main ) ;

          #endregion
        }
        else {
          return retID ;
        }

        s.name = Data.GetParameter_string( symbol, paramName[ 0 ] ) ;
        Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 1 ] ).ToUpper(), out StbSecColumn_Kind_column kind ) ;
        s.kind_column = kind ;

        s.strength_concrete = Data.GetParameter_string( symbol, paramName[ 2 ] ) ;
        s.strength_concrete = Data.GetConcreteFC( s.strength_concrete ) ;

        s.isReferenceDirection = Data.GetParameter_bool( symbol, paramName[ 3 ] ) ;

        string base_type = Data.GetParameter_string( symbol, paramName[ 4 ] ) ;
        if ( base_type != "" ) {
          //1.4の非埋込→2.0の露出とする
          if ( base_type == "UNEMBEDDED" ) base_type = "EXPOSE" ;

          if ( Enum.TryParse( base_type, out StbSecSteelFigureColumn_CFTBase_type base_Type2 ) ) {
            s.StbSecSteelFigureColumn_CFT.base_type = base_Type2 ;

            var bp = GetBaseProduct( ins.Id, ps ) ;
            if ( bp != null ) {
              s.Item = (StbSecBaseProduct_CFT)bp ;
            }
          }
        }


        stb.StbModel.StbSections.StbSecColumn_CFT.Add( s ) ;
        retID = s.id ;

        #endregion
      }


      return retID ;
    }


    /// <summary>
    /// 柱配置の出力
    /// </summary>
    private static void Export_Column()
    {
      List<string> AllFamilyName = new List<string>() ;
      for ( int i = 0 ; i < SetFamily.ClmFName.FamilyName.Length ; ++i ) {
        AllFamilyName.AddRange( SetFamily.ClmFName.FamilyName[ i ] ) ;
      }

      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_StructuralColumns ) ;
      List<FamilyInstance> instances = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where( x => AllFamilyName.Contains( x.Symbol.Family.Name ) && ! x.Symbol.Family.IsInPlace ).ToList() ;

      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;
      var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager( Commons.doc ) ;

      for ( int i = 0 ; i < instances.Count ; ++i ) {
        XYZ ps1 = new XYZ() ;
        XYZ pe1 = new XYZ() ;
        XYZ ps2 = new XYZ() ;
        XYZ pe2 = new XYZ() ;

        if ( instances[ i ].Location is LocationPoint locP ) {
          Parameter param = instances[ i ].get_Parameter( BuiltInParameter.FAMILY_BASE_LEVEL_PARAM ) ;
          double z1 = Levels.Find( x => x.Id == param.AsElementId() ).ProjectElevation ;

          param = instances[ i ].get_Parameter( BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM ) ;
          z1 += param.AsDouble() ;

          param = instances[ i ].get_Parameter( BuiltInParameter.FAMILY_TOP_LEVEL_PARAM ) ;
          double z2 = Levels.Find( x => x.Id == param.AsElementId() ).ProjectElevation ;

          param = instances[ i ].get_Parameter( BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM ) ;
          z2 += param.AsDouble() ;

          ps2 = new XYZ( locP.Point.X, locP.Point.Y, z1 ) ;
          pe2 = new XYZ( locP.Point.X, locP.Point.Y, z2 ) ;
        }
        else {
          LocationCurve locC = instances[ i ].Location as LocationCurve ;
          ps2 = locC.Curve.GetEndPoint( 0 ) ;
          pe2 = locC.Curve.GetEndPoint( 1 ) ;
        }

        if ( amanager.HasAssociation( instances[ i ].Id ) && Commons.doc.GetElement( amanager.GetAssociatedElementId( instances[ i ].Id ) ) is AnalyticalMember member ) {
          ps1 = member.GetCurve().GetEndPoint( 0 ) ;
          pe1 = member.GetCurve().GetEndPoint( 1 ) ;
        }
        else {
          ps1 = ps2 ;
          pe1 = pe2 ;
        }

        ps1 = Commons.ft2mm( ps1 ) ;
        pe1 = Commons.ft2mm( pe1 ) ;
        ps2 = Commons.ft2mm( ps2 ) ;
        pe2 = Commons.ft2mm( pe2 ) ;


        int node_s = GetNodeId( ps1 ) ;
        int node_e = GetNodeId( pe1 ) ;

        var c = new StbColumn() { guid = GetGuid( instances[ i ], "" ), id_node_bottom = node_s, id_node_top = node_e, } ;

        if ( ! sect.ContainsKey( instances[ i ].Symbol.Id ) ) {
          c.id_section = Export_SecColumn( instances[ i ], ps1 ) ;
          if ( c.id_section < 0 ) continue ;

          sect.Add( instances[ i ].Symbol.Id, c.id_section ) ;
        }
        else {
          c.id_section = sect[ instances[ i ].Symbol.Id ] ;
        }


        string[] paramName = new string[ 11 ] ;
        bool isPost = false ;

        string familyname = instances[ i ].Symbol.Family.Name ;
        if ( familyname == SetFamily.RCClmRe.FamilyName ) {
          #region RC柱

          c.kind_structure = StbColumnKind_structure.RC ;

          paramName[ 0 ] = SetFamily.RCClmRe.NameMembers ;
          paramName[ 1 ] = SetFamily.RCClmRe.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.RCClmRe.thickness_ex_start_Y ;
          paramName[ 3 ] = SetFamily.RCClmRe.thickness_ex_end_X ;
          paramName[ 4 ] = SetFamily.RCClmRe.thickness_ex_end_Y ;

          isPost = stb.StbModel.StbSections.StbSecColumn_RC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.RCClmRo.FamilyName ) {
          #region RC円柱

          c.kind_structure = StbColumnKind_structure.RC ;

          paramName[ 0 ] = SetFamily.RCClmRo.NameMembers ;
          paramName[ 1 ] = SetFamily.RCClmRo.thickness_ex_start_X ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;

          isPost = stb.StbModel.StbSections.StbSecColumn_RC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmH.FamilyName ) {
          #region S柱H

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmH.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmH.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmH.condition_top ;
          paramName[ 7 ] = SetFamily.SClmH.joint_top ;
          paramName[ 8 ] = SetFamily.SClmH.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmH.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmH.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBH.FamilyName ) {
          #region S柱BH

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmBH.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmBH.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmBH.condition_top ;
          paramName[ 7 ] = SetFamily.SClmBH.joint_top ;
          paramName[ 8 ] = SetFamily.SClmBH.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmBH.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmBH.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBox.FamilyName ) {
          #region S柱Box

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmBox.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmBox.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmBox.condition_top ;
          paramName[ 7 ] = SetFamily.SClmBox.joint_top ;
          paramName[ 8 ] = SetFamily.SClmBox.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmBox.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmBox.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmBBox.FamilyName ) {
          #region S柱BBox

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmBBox.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmBBox.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmBBox.condition_top ;
          paramName[ 7 ] = SetFamily.SClmBBox.joint_top ;
          paramName[ 8 ] = SetFamily.SClmBBox.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmBBox.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmBBox.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmPipe.FamilyName ) {
          #region S柱Pipe

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmPipe.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmPipe.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmPipe.condition_top ;
          paramName[ 7 ] = SetFamily.SClmPipe.joint_top ;
          paramName[ 8 ] = SetFamily.SClmPipe.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmPipe.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmPipe.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmT.FamilyName ) {
          #region S柱T

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmT.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmT.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmT.condition_top ;
          paramName[ 7 ] = SetFamily.SClmT.joint_top ;
          paramName[ 8 ] = SetFamily.SClmT.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmT.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmT.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmC.FamilyName ) {
          #region S柱C

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmC.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmC.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmC.condition_top ;
          paramName[ 7 ] = SetFamily.SClmC.joint_top ;
          paramName[ 8 ] = SetFamily.SClmC.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmC.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmC.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SClmL.FamilyName ) {
          #region S柱L

          c.kind_structure = StbColumnKind_structure.S ;

          paramName[ 0 ] = SetFamily.SClmL.NameMembers ;

          paramName[ 5 ] = SetFamily.SClmL.condition_bottom ;
          paramName[ 6 ] = SetFamily.SClmL.condition_top ;
          paramName[ 7 ] = SetFamily.SClmL.joint_top ;
          paramName[ 8 ] = SetFamily.SClmL.joint_bottom ;
          paramName[ 9 ] = SetFamily.SClmL.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SClmL.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_S.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmH.FamilyName ) {
          #region SRC柱 H

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmH.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmH.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.SRCClmH.thickness_ex_start_Y ;
          paramName[ 3 ] = SetFamily.SRCClmH.thickness_ex_end_X ;
          paramName[ 4 ] = SetFamily.SRCClmH.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.SRCClmH.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmH.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmH.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmH.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmH.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmH.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmCross.FamilyName ) {
          #region SRC柱 +

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmCross.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmCross.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.SRCClmCross.thickness_ex_start_Y ;
          paramName[ 3 ] = SetFamily.SRCClmCross.thickness_ex_end_X ;
          paramName[ 4 ] = SetFamily.SRCClmCross.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.SRCClmCross.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmCross.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmCross.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmCross.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmCross.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmCross.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmT.FamilyName ) {
          #region SRC柱 T

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmT.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmT.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.SRCClmT.thickness_ex_start_Y ;
          paramName[ 3 ] = SetFamily.SRCClmT.thickness_ex_end_X ;
          paramName[ 4 ] = SetFamily.SRCClmT.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.SRCClmT.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmT.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmT.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmT.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmT.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmT.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmH_Rou.FamilyName ) {
          #region SRC円柱 H

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmH_Rou.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmH_Rou.thickness_ex_start_X ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = SetFamily.SRCClmH_Rou.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmH_Rou.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmH_Rou.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmH_Rou.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmH_Rou.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmH_Rou.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmCross_Rou.FamilyName ) {
          #region SRC円柱 +

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmCross_Rou.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmCross_Rou.thickness_ex_start_X ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = SetFamily.SRCClmCross_Rou.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmCross_Rou.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmCross_Rou.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmCross_Rou.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmCross_Rou.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmCross_Rou.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.SRCClmT_Rou.FamilyName ) {
          #region SRC円柱 T

          c.kind_structure = StbColumnKind_structure.SRC ;

          paramName[ 0 ] = SetFamily.SRCClmT_Rou.NameMembers ;
          paramName[ 1 ] = SetFamily.SRCClmT_Rou.thickness_ex_start_X ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = SetFamily.SRCClmT_Rou.condition_bottom ;
          paramName[ 6 ] = SetFamily.SRCClmT_Rou.condition_top ;
          paramName[ 7 ] = SetFamily.SRCClmT_Rou.joint_top ;
          paramName[ 8 ] = SetFamily.SRCClmT_Rou.joint_bottom ;
          paramName[ 9 ] = SetFamily.SRCClmT_Rou.kind_joint_top ;
          paramName[ 10 ] = SetFamily.SRCClmT_Rou.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_SRC.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.CFTClmBox.FamilyName ) {
          #region CFT柱角形鋼管

          c.kind_structure = StbColumnKind_structure.CFT ;

          paramName[ 0 ] = SetFamily.CFTClmBox.NameMembers ;
          paramName[ 1 ] = "" ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = SetFamily.CFTClmBox.condition_bottom ;
          paramName[ 6 ] = SetFamily.CFTClmBox.condition_top ;
          paramName[ 7 ] = SetFamily.CFTClmBox.joint_top ;
          paramName[ 8 ] = SetFamily.CFTClmBox.joint_bottom ;
          paramName[ 9 ] = SetFamily.CFTClmBox.kind_joint_top ;
          paramName[ 10 ] = SetFamily.CFTClmBox.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_CFT.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else if ( familyname == SetFamily.CFTClmPipe.FamilyName ) {
          #region CFT柱鋼管

          c.kind_structure = StbColumnKind_structure.CFT ;

          paramName[ 0 ] = SetFamily.CFTClmPipe.NameMembers ;
          paramName[ 1 ] = "" ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = SetFamily.CFTClmPipe.condition_bottom ;
          paramName[ 6 ] = SetFamily.CFTClmPipe.condition_top ;
          paramName[ 7 ] = SetFamily.CFTClmPipe.joint_top ;
          paramName[ 8 ] = SetFamily.CFTClmPipe.joint_bottom ;
          paramName[ 9 ] = SetFamily.CFTClmPipe.kind_joint_top ;
          paramName[ 10 ] = SetFamily.CFTClmPipe.kind_joint_bottom ;

          isPost = stb.StbModel.StbSections.StbSecColumn_CFT.Find( x => x.id == c.id_section )?.kind_column == StbSecColumn_Kind_column.POST ;

          #endregion
        }
        else {
          continue ;
        }


        id++ ;
        c.id = id ;
        c.rotate = XYZ.BasisX.AngleOnPlaneTo( instances[ i ].HandOrientation, XYZ.BasisZ ) * 180 / Math.PI ;

        XYZ v1 = ( pe1 - ps1 ).Normalize() ;
        XYZ v2 = ( pe2 - ps2 ).Normalize() ;
        XYZ offset_s = ps2 - ps1 ;
        XYZ offset_e = pe2 - pe1 ;

        c.offset_bottom_X = offset_s.X ;
        c.offset_bottom_Y = offset_s.Y ;
        c.offset_bottom_Z = offset_s.Z ;
        c.offset_top_X = offset_e.X ;
        c.offset_top_Y = offset_e.Y ;
        c.offset_top_Z = offset_e.Z ;


        c.name = instances[ i ].Symbol.Name ;
        c.thickness_add_start_X = Data.GetParameter_double( instances[ i ], paramName[ 1 ] ) ;
        c.thickness_add_start_Y = Data.GetParameter_double( instances[ i ], paramName[ 2 ] ) ;
        c.thickness_add_end_X = Data.GetParameter_double( instances[ i ], paramName[ 3 ] ) ;
        c.thickness_add_end_Y = Data.GetParameter_double( instances[ i ], paramName[ 4 ] ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 5 ] ), out StbColumnCondition condition_b ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 6 ] ), out StbColumnCondition condition_t ) ;
        c.condition_bottom = condition_b ;
        c.condition_top = condition_t ;

        c.joint_top = Data.GetParameter_double( instances[ i ], paramName[ 7 ] ) ;
        c.joint_bottom = Data.GetParameter_double( instances[ i ], paramName[ 8 ] ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 9 ] ), out StbColumnKind_joint joint_t ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 10 ] ), out StbColumnKind_joint joint_b ) ;
        c.kind_joint_top = joint_t ;
        c.kind_joint_bottom = joint_b ;

        if ( isPost ) {
          StbPost p = new StbPost( c ) ;
          stb.StbModel.StbMembers.StbPosts.Add( p ) ;
        }
        else {
          stb.StbModel.StbMembers.StbColumns.Add( c ) ;
        }


        Data.AddLog( Data.LogCode.column, instances[ i ], c.id, c.id_section ) ;
      }
    }

    #endregion


    #region 梁

    /// <summary>
    /// 梁種別から基礎梁を判定
    /// </summary>
    /// <param name="kind_beam"></param>
    /// <param name="levelid"></param>
    /// <returns></returns>
    private static bool Check_isFoundation( string kind_beam, ElementId levelid )
    {
      if ( kind_beam == "" ) {
        double GL = Levels.Find( a => a.Name == "GL" )?.Elevation ?? 0 ;
        return ( Levels.Find( a => a.Id == levelid )?.Elevation ?? 1000 ) < GL ;
      }
      else {
        return kind_beam.Contains( "Foundation" ) ;
      }
    }

    /// <summary>
    /// 梁種別から片持ち梁を判定
    /// </summary>
    /// <param name="kind_beam"></param>
    /// <returns></returns>
    private static bool Check_isCanti( string kind_beam )
    {
      if ( kind_beam == "" ) {
        return false ;
      }
      else {
        return kind_beam.Contains( "Cantilever" ) ;
      }
    }


    #region 梁の同一チェック

    /// <summary>
    /// 梁断面始端中央終端別配筋（RC）の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_RC_Bar_SCE( StbSecBarBeam_RC_ThreeTypes a, StbSecBarBeam_RC_ThreeTypes b )
    {
      bool isSame = true ;

      isSame &= a.D_main == b.D_main ;
      isSame &= a.D_2nd_main == b.D_2nd_main ;
      isSame &= a.D_stirrup == b.D_stirrup ;
      isSame &= a.D_web == b.D_web ;
      isSame &= a.D_bar_spacing == b.D_bar_spacing ;

      isSame &= a.strength_main == b.strength_main ;
      isSame &= a.strength_2nd_main == b.strength_2nd_main ;
      isSame &= a.strength_stirrup == b.strength_stirrup ;
      isSame &= a.strength_web == b.strength_web ;
      isSame &= a.strength_bar_spacing == b.strength_bar_spacing ;

      isSame &= a.N_main_top_1st == b.N_main_top_1st ;
      isSame &= a.N_main_top_2nd == b.N_main_top_2nd ;
      isSame &= a.N_main_top_3rd == b.N_main_top_3rd ;
      isSame &= a.N_main_bottom_1st == b.N_main_bottom_1st ;
      isSame &= a.N_main_bottom_2nd == b.N_main_bottom_2nd ;
      isSame &= a.N_main_bottom_3rd == b.N_main_bottom_3rd ;
      isSame &= a.N_2nd_main_top_1st == b.N_2nd_main_top_1st ;
      isSame &= a.N_2nd_main_top_2nd == b.N_2nd_main_top_2nd ;
      isSame &= a.N_2nd_main_top_3rd == b.N_2nd_main_top_3rd ;
      isSame &= a.N_2nd_main_bottom_1st == b.N_2nd_main_bottom_1st ;
      isSame &= a.N_2nd_main_bottom_2nd == b.N_2nd_main_bottom_2nd ;
      isSame &= a.N_2nd_main_bottom_3rd == b.N_2nd_main_bottom_3rd ;
      isSame &= a.N_stirrup == b.N_stirrup ;
      isSame &= a.N_web == b.N_web ;
      isSame &= a.N_bar_spacing == b.N_bar_spacing ;

      isSame &= Math.Abs( a.pitch_stirrup - b.pitch_stirrup ) < 0.01 ;
      isSame &= Math.Abs( a.pitch_bar_spacing - b.pitch_bar_spacing ) < 0.01 ;

      return isSame ;
    }

    /// <summary>
    /// 梁形状（RC）の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_StbSecFigure( StbSecFigureBeam_RC a, StbSecFigureBeam_RC b )
    {
      bool isSame = true ;

      isSame &= a.FigureType == b.FigureType ;
      if ( isSame ) {
        switch ( a.FigureType ) {
          case 1 :
            var sa = (StbSecBeam_RC_Straight)a.Items.First() ;
            var sb = (StbSecBeam_RC_Straight)b.Items.First() ;
            isSame &= Math.Abs( sa.width - sb.width ) < 0.01 ;
            isSame &= Math.Abs( sa.depth - sb.depth ) < 0.01 ;
            break ;

          case 2 :
            var ta_s = a.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.START ) ;
            var ta_e = a.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.END ) ;
            var tb_s = b.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.START ) ;
            var tb_e = b.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.END ) ;

            isSame &= Math.Abs( ta_s.width - tb_s.width ) < 0.01 ;
            isSame &= Math.Abs( ta_e.width - tb_e.width ) < 0.01 ;
            isSame &= Math.Abs( ta_s.depth - tb_s.depth ) < 0.01 ;
            isSame &= Math.Abs( ta_e.depth - tb_e.depth ) < 0.01 ;
            break ;

          case 3 :
            var ha_s = a.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.START ) ;
            var ha_c = a.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.CENTER ) ;
            var ha_e = a.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.END ) ;
            var hb_s = b.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.START ) ;
            var hb_c = b.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.CENTER ) ;
            var hb_e = b.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.END ) ;

            isSame &= Math.Abs( ha_s.width - hb_s.width ) < 0.01 ;
            isSame &= Math.Abs( ha_c.width - hb_c.width ) < 0.01 ;
            isSame &= Math.Abs( ha_e.width - hb_e.width ) < 0.01 ;
            isSame &= Math.Abs( ha_s.depth - hb_s.depth ) < 0.01 ;
            isSame &= Math.Abs( ha_c.depth - hb_c.depth ) < 0.01 ;
            isSame &= Math.Abs( ha_e.depth - hb_e.depth ) < 0.01 ;
            break ;
        }
      }

      return isSame ;
    }

    /// <summary>
    /// RC梁の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_RC( StbSecBeam_RC a, StbSecBeam_RC b )
    {
      bool isSame = true ;

      isSame &= a.name == b.name ;
      isSame &= a.kind_beam == b.kind_beam ;
      isSame &= a.isFoundation == b.isFoundation ;
      isSame &= a.isCanti == b.isCanti ;
      isSame &= a.isOutin == b.isOutin ;
      isSame &= a.strength_concrete == b.strength_concrete ;

      if ( ! isSame ) return isSame ;

      isSame &= CompareTo_StbSecBeam_StbSecFigure( a.StbSecFigureBeam_RC, b.StbSecFigureBeam_RC ) ;
      if ( ! isSame ) return isSame ;

      isSame &= a.StbSecBarArrangementBeam_RC.Bar_ArrangementType == b.StbSecBarArrangementBeam_RC.Bar_ArrangementType ;
      if ( isSame ) {
        switch ( a.StbSecBarArrangementBeam_RC.Bar_ArrangementType ) {
          case 1 :
            var sa = new StbSecBarBeam_RC_ThreeTypes( (StbSecBarBeam_RC_Same)a.StbSecBarArrangementBeam_RC.Items.First() ) ;
            var sb = new StbSecBarBeam_RC_ThreeTypes( (StbSecBarBeam_RC_Same)b.StbSecBarArrangementBeam_RC.Items.First() ) ;
            isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( sa, sb ) ;
            break ;

          case 2 :
            var ta = a.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>().OrderBy( x => x.pos ).ToList() ;
            var tb = b.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>().OrderBy( x => x.pos ).ToList() ;

            isSame &= ta.Count == tb.Count ;
            if ( isSame ) {
              for ( int i = 0 ; i < ta.Count ; ++i ) {
                isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( ta[ i ], tb[ i ] ) ;
              }
            }

            break ;

          case 3 :
            var se_a = a.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().OrderBy( x => x.pos ).ToList() ;
            var se_b = b.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().OrderBy( x => x.pos ).ToList() ;

            isSame &= se_a.Count == se_b.Count ;
            if ( isSame ) {
              for ( int i = 0 ; i < se_a.Count ; ++i ) {
                isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( se_a[ i ] ), new StbSecBarBeam_RC_ThreeTypes( se_b[ i ] ) ) ;
              }
            }

            break ;
        }
      }

      return isSame ;
    }


    /// <summary>
    /// S梁の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_S( StbSecBeam_S a, StbSecBeam_S b )
    {
      bool isSame = true ;

      isSame &= a.name == b.name ;
      isSame &= a.floor == b.floor ;
      isSame &= a.kind_beam == b.kind_beam ;
      isSame &= a.isCanti == b.isCanti ;
      isSame &= a.isOutin == b.isOutin ;

      if ( ! isSame ) return isSame ;

      isSame &= a.StbSecSteelFigureBeam_S.Items.Count == b.StbSecSteelFigureBeam_S.Items.Count ;
      isSame &= a.StbSecSteelFigureBeam_S.FigureType == b.StbSecSteelFigureBeam_S.FigureType ;
      if ( isSame ) {
        switch ( a.StbSecSteelFigureBeam_S.FigureType ) {
          case 1 :
            var s_a = (StbSecSteelBeam_S_Straight)a.StbSecSteelFigureBeam_S.Items.First() ;
            var s_b = (StbSecSteelBeam_S_Straight)b.StbSecSteelFigureBeam_S.Items.First() ;
            isSame &= s_a.shape == s_b.shape ;
            isSame &= s_a.strength_main == s_b.strength_main ;
            isSame &= s_a.strength_web == s_b.strength_web ;
            break ;

          case 2 :
            var t_a = a.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Taper>().OrderBy( x => x.pos ).ToList() ;
            var t_b = b.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Taper>().OrderBy( x => x.pos ).ToList() ;
            for ( int i = 0 ; i < t_a.Count ; ++i ) {
              isSame &= t_a[ i ].shape == t_b[ i ].shape ;
              isSame &= t_a[ i ].strength_main == t_b[ i ].strength_main ;
              isSame &= t_a[ i ].strength_web == t_b[ i ].strength_web ;
              if ( ! isSame ) return isSame ;
            }

            break ;

          case 3 :
            var j_a = a.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Joint>().OrderBy( x => x.pos ).ToList() ;
            var j_b = b.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Joint>().OrderBy( x => x.pos ).ToList() ;
            for ( int i = 0 ; i < j_a.Count ; ++i ) {
              isSame &= j_a[ i ].shape == j_b[ i ].shape ;
              isSame &= j_a[ i ].strength_main == j_b[ i ].strength_main ;
              isSame &= j_a[ i ].strength_web == j_b[ i ].strength_web ;
              if ( ! isSame ) return isSame ;
            }

            break ;

          case 4 :
            var h_a = a.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().OrderBy( x => x.pos ).ToList() ;
            var h_b = b.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().OrderBy( x => x.pos ).ToList() ;
            for ( int i = 0 ; i < h_a.Count ; ++i ) {
              isSame &= h_a[ i ].shape == h_b[ i ].shape ;
              isSame &= h_a[ i ].strength_main == h_b[ i ].strength_main ;
              isSame &= h_a[ i ].strength_web == h_b[ i ].strength_web ;
              if ( ! isSame ) return isSame ;
            }

            break ;

          case 5 :
            var f_a = a.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().OrderBy( x => x.pos ).ToList() ;
            var f_b = b.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().OrderBy( x => x.pos ).ToList() ;
            for ( int i = 0 ; i < f_a.Count ; ++i ) {
              isSame &= f_a[ i ].shape == f_b[ i ].shape ;
              isSame &= f_a[ i ].strength_main == f_b[ i ].strength_main ;
              isSame &= f_a[ i ].strength_web == f_b[ i ].strength_web ;
              if ( ! isSame ) return isSame ;
            }

            break ;
        }
      }

      return isSame ;
    }


    /// <summary>
    /// 梁形状（SRC）の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_SRC_StbSecFigure( StbSecFigureBeam_SRC a, StbSecFigureBeam_SRC b )
    {
      bool isSame = true ;

      isSame &= a.FigureType == b.FigureType ;
      if ( isSame ) {
        switch ( a.FigureType ) {
          case 1 :
            var sa = (StbSecBeam_SRC_Straight)a.Items.First() ;
            var sb = (StbSecBeam_SRC_Straight)b.Items.First() ;
            isSame &= Math.Abs( sa.width - sb.width ) < 0.01 ;
            isSame &= Math.Abs( sa.depth - sb.depth ) < 0.01 ;
            break ;

          case 2 :
            var ta_s = a.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.START ) ;
            var ta_e = a.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.END ) ;
            var tb_s = b.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.START ) ;
            var tb_e = b.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault( x => x.pos == StbSecBeam_RC_TaperPos.END ) ;

            isSame &= Math.Abs( ta_s.width - tb_s.width ) < 0.01 ;
            isSame &= Math.Abs( ta_e.width - tb_e.width ) < 0.01 ;
            isSame &= Math.Abs( ta_s.depth - tb_s.depth ) < 0.01 ;
            isSame &= Math.Abs( ta_e.depth - tb_e.depth ) < 0.01 ;
            break ;

          case 3 :
            var ha_s = a.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.START ) ;
            var ha_c = a.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.CENTER ) ;
            var ha_e = a.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.END ) ;
            var hb_s = b.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.START ) ;
            var hb_c = b.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.CENTER ) ;
            var hb_e = b.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault( x => x.pos == StbSecBeam_RC_HaunchPos.END ) ;

            isSame &= Math.Abs( ha_s.width - hb_s.width ) < 0.01 ;
            isSame &= Math.Abs( ha_c.width - hb_c.width ) < 0.01 ;
            isSame &= Math.Abs( ha_e.width - hb_e.width ) < 0.01 ;
            isSame &= Math.Abs( ha_s.depth - hb_s.depth ) < 0.01 ;
            isSame &= Math.Abs( ha_c.depth - hb_c.depth ) < 0.01 ;
            isSame &= Math.Abs( ha_e.depth - hb_e.depth ) < 0.01 ;
            break ;
        }
      }


      return isSame ;
    }

    /// <summary>
    /// SRC梁の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_StbSecBeam_SRC( StbSecBeam_SRC a, StbSecBeam_SRC b )
    {
      bool isSame = true ;

      isSame &= a.name == b.name ;
      isSame &= a.kind_beam == b.kind_beam ;
      isSame &= a.isFoundation == b.isFoundation ;
      isSame &= a.isCanti == b.isCanti ;
      isSame &= a.isOutin == b.isOutin ;
      isSame &= a.strength_concrete == b.strength_concrete ;

      if ( ! isSame ) return isSame ;

      isSame &= CompareTo_StbSecBeam_SRC_StbSecFigure( a.StbSecFigureBeam_SRC, b.StbSecFigureBeam_SRC ) ;
      if ( ! isSame ) return isSame ;

      isSame &= a.StbSecBarArrangementBeam_SRC.Bar_ArrangementType == b.StbSecBarArrangementBeam_SRC.Bar_ArrangementType ;
      if ( isSame ) {
        switch ( a.StbSecBarArrangementBeam_SRC.Bar_ArrangementType ) {
          case 1 :
            var sa = new StbSecBarBeam_SRC_ThreeTypes( (StbSecBarBeam_SRC_Same)a.StbSecBarArrangementBeam_SRC.Items.First() ) ;
            var sb = new StbSecBarBeam_SRC_ThreeTypes( (StbSecBarBeam_SRC_Same)b.StbSecBarArrangementBeam_SRC.Items.First() ) ;
            isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( sa, sb ) ;
            break ;

          case 2 :
            var ta = a.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>().OrderBy( x => x.pos ).ToList() ;
            var tb = b.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>().OrderBy( x => x.pos ).ToList() ;

            isSame &= ta.Count == tb.Count ;
            if ( isSame ) {
              for ( int i = 0 ; i < ta.Count ; ++i ) {
                isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( ta[ i ], tb[ i ] ) ;
              }
            }

            break ;

          case 3 :
            var se_a = a.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_StartEnd>().OrderBy( x => x.pos ).ToList() ;
            var se_b = b.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_StartEnd>().OrderBy( x => x.pos ).ToList() ;

            isSame &= se_a.Count == se_b.Count ;
            if ( isSame ) {
              for ( int i = 0 ; i < se_a.Count ; ++i ) {
                isSame &= CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_SRC_ThreeTypes( se_a[ i ] ), new StbSecBarBeam_SRC_ThreeTypes( se_b[ i ] ) ) ;
              }
            }

            break ;
        }
      }

      return isSame ;
    }

    #endregion


    /// <summary>
    /// 梁断面の出力
    /// </summary>
    /// <param name="ins"></param>
    /// <returns></returns>
    private static int Export_SecGirder( FamilyInstance ins )
    {
      FamilySymbol symbol = ins.Symbol ;
      string floor = Levels.Find( x => x.Id == ins.get_Parameter( BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM ).AsElementId() ).Name ;

      int retID = -1 ;

      string familyname = symbol.Family.Name ;
      if ( familyname == SetFamily.RCGir_F.FamilyName || familyname == SetFamily.RCGir_F_Haunch.FamilyName || familyname == SetFamily.RCBeam_F.FamilyName || familyname == SetFamily.RCBeam_F_Haunch.FamilyName || familyname == SetFamily.RCGir.FamilyName || familyname == SetFamily.RCGir_Haunch.FamilyName || familyname == SetFamily.RCBeam.FamilyName || familyname == SetFamily.RCBeam_Haunch.FamilyName ) {
        id_sect++ ;

        StbSecBeam_RC s = new StbSecBeam_RC()
        {
          id = id_sect, guid = GetGuid( symbol, "" ), floor = floor, isOutin = false,
        } ;

        FamilyStructure.RC_Gir RCGir = null ;
        if ( familyname == SetFamily.RCGir_F.FamilyName ) {
          RCGir = SetFamily.RCGir_F ;
        }
        else if ( familyname == SetFamily.RCGir_F_Haunch.FamilyName ) {
          RCGir = SetFamily.RCGir_F_Haunch ;
        }
        else if ( familyname == SetFamily.RCBeam_F.FamilyName ) {
          RCGir = SetFamily.RCBeam_F ;
        }
        else if ( familyname == SetFamily.RCBeam_F_Haunch.FamilyName ) {
          RCGir = SetFamily.RCBeam_F_Haunch ;
        }
        else if ( familyname == SetFamily.RCGir.FamilyName ) {
          RCGir = SetFamily.RCGir ;
        }
        else if ( familyname == SetFamily.RCGir_Haunch.FamilyName ) {
          RCGir = SetFamily.RCGir_Haunch ;
        }
        else if ( familyname == SetFamily.RCBeam.FamilyName ) {
          RCGir = SetFamily.RCBeam ;
        }
        else if ( familyname == SetFamily.RCBeam_Haunch.FamilyName ) {
          RCGir = SetFamily.RCBeam_Haunch ;
        }

        if ( RCGir == null ) return retID ;


        string kind_beam = Data.GetParameter_string( symbol, RCGir.kind_beam ) ;
        s.isFoundation = Check_isFoundation( kind_beam, ins.LevelId ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, RCGir.name ) ;
        s.strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, RCGir.strength_concrete ) ) ;


        //形状
        double ws = Data.GetParameter_double( symbol, RCGir.width_start ) ;
        double wc = Data.GetParameter_double( symbol, RCGir.width_center ) ;
        double we = Data.GetParameter_double( symbol, RCGir.width_end ) ;
        double ds = Data.GetParameter_double( symbol, RCGir.depth_start ) ;
        double dc = Data.GetParameter_double( symbol, RCGir.depth_center ) ;
        double de = Data.GetParameter_double( symbol, RCGir.depth_end ) ;
        s.StbSecFigureBeam_RC = new StbSecFigureBeam_RC() { Items = new List<object>(), } ;

        if ( Math.Abs( ws - wc ) < 0.01 && Math.Abs( we - wc ) < 0.01 && Math.Abs( ds - dc ) < 0.01 && Math.Abs( de - dc ) < 0.01 ) {
          s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Straight() { width = wc, depth = dc, } ) ;
        }
        else {
          if ( s.isCanti ) {
            if ( ws < 0.1 ) ws = wc ;
            if ( we < 0.1 ) we = wc ;
            if ( ds < 0.1 ) ds = dc ;
            if ( de < 0.1 ) de = dc ;

            s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Taper() { pos = StbSecBeam_RC_TaperPos.START, width = ws, depth = ds, } ) ;

            s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Taper() { pos = StbSecBeam_RC_TaperPos.END, width = we, depth = de, } ) ;
          }
          else {
            s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Haunch() { pos = StbSecBeam_RC_HaunchPos.START, width = ws, depth = ds, } ) ;

            s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Haunch() { pos = StbSecBeam_RC_HaunchPos.CENTER, width = wc, depth = dc, } ) ;

            s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Haunch() { pos = StbSecBeam_RC_HaunchPos.END, width = we, depth = de, } ) ;
          }
        }


        //配筋
        s.StbSecBarArrangementBeam_RC = new StbSecBarArrangementBeam_RC()
        {
          depth_cover_left = Data.GetParameter_double( symbol, RCGir.depth_cover_left ),
          depth_cover_right = Data.GetParameter_double( symbol, RCGir.depth_cover_right ),
          depth_cover_top = Data.GetParameter_double( symbol, RCGir.depth_cover_top ),
          depth_cover_bottom = Data.GetParameter_double( symbol, RCGir.depth_cover_bottom ),
          interval = Data.GetParameter_double( symbol, RCGir.interval_reinforcement ),
          center_top = Data.GetParameter_double( symbol, RCGir.center_reinforcement_top ),
          center_bottom = Data.GetParameter_double( symbol, RCGir.center_reinforcement_bottom ),
          center_side = 0,
          center_interval = 0,
          length_bar_start = Data.GetParameter_double( symbol, RCGir.bar_length_start ),
          length_bar_end = Data.GetParameter_double( symbol, RCGir.bar_length_end ),
          Items = new List<object>(),
        } ;

        List<StbSecBarBeam_RC_Same> bar = new List<StbSecBarBeam_RC_Same>() ;
        for ( int b = 0 ; b < RCGir.count_main_top_1st.Length ; ++b ) {
          var bb = new StbSecBarBeam_RC_Same()
          {
            D_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_main_top[ b ] ),
            D_2nd_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_2nd_main_top[ b ] ),
            D_stirrup = Data.GetParameter_string( symbol, RCGir.D_stirrup[ b ] ),
            D_web = Data.GetParameter_string( symbol, RCGir.D_reinforcement_web[ b ] ),
            D_bar_spacing = Data.GetParameter_string( symbol, RCGir.D_bar_spacing[ b ] ),
            strength_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_main ),
            strength_2nd_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_2nd_main ),
            strength_stirrup = Data.GetParameter_string( symbol, RCGir.strength_stirrup ),
            strength_web = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_web ),
            strength_bar_spacing = Data.GetParameter_string( symbol, RCGir.strength_bar_spacing ),
            N_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_main_top_1st[ b ] ),
            N_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_main_top_2nd[ b ] ),
            N_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_main_top_3rd[ b ] ),
            N_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_main_bottom_1st[ b ] ),
            N_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_2nd[ b ] ),
            N_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_3rd[ b ] ),
            N_2nd_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_1st[ b ] ),
            N_2nd_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_2nd[ b ] ),
            N_2nd_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_3rd[ b ] ),
            N_2nd_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_1st[ b ] ),
            N_2nd_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_2nd[ b ] ),
            N_2nd_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_3rd[ b ] ),
            N_stirrup = Data.GetParameter_int( symbol, RCGir.count_stirrup[ b ] ),
            N_web = Data.GetParameter_int( symbol, RCGir.count_web[ b ] ),
            N_bar_spacing = Data.GetParameter_int( symbol, RCGir.count_bar_spacing[ b ] ),
            pitch_stirrup = Data.GetParameter_double( symbol, RCGir.pitch_stirrup[ b ] ),
            pitch_bar_spacing = Data.GetParameter_double( symbol, RCGir.pitch_bar_spacing[ b ] ),
          } ;
          bar.Add( bb ) ;
        }


        bool isSame0 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_RC_ThreeTypes( bar[ 2 ] ) ) ;
        bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_RC_ThreeTypes( bar[ 1 ] ) ) ;

        if ( isSame0 && isSame1 ) {
          s.StbSecBarArrangementBeam_RC.Items.Add( bar[ 0 ] ) ;
        }
        else {
          s.StbSecBarArrangementBeam_RC.Items.Add( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.START } ) ;
          s.StbSecBarArrangementBeam_RC.Items.Add( new StbSecBarBeam_RC_ThreeTypes( bar[ 1 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.CENTER } ) ;
          s.StbSecBarArrangementBeam_RC.Items.Add( new StbSecBarBeam_RC_ThreeTypes( bar[ 2 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.END } ) ;
        }


        retID = stb.StbModel.StbSections.StbSecBeam_RC.Find( x => CompareTo_StbSecBeam_RC( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_RC.Add( s ) ;
          retID = s.id ;
        }
      }
      else if ( familyname == SetFamily.RCCGir_F.FamilyName || familyname == SetFamily.RCCBeam_F.FamilyName || familyname == SetFamily.RCCGir.FamilyName || familyname == SetFamily.RCCBeam.FamilyName ) {
        id_sect++ ;

        StbSecBeam_RC s = new StbSecBeam_RC()
        {
          id = id_sect, guid = GetGuid( symbol, "" ), floor = floor, isOutin = false,
        } ;

        FamilyStructure.RC_CGir RCGir = null ;
        if ( familyname == SetFamily.RCCGir_F.FamilyName ) {
          RCGir = SetFamily.RCCGir_F ;
        }
        else if ( familyname == SetFamily.RCCBeam_F.FamilyName ) {
          RCGir = SetFamily.RCCBeam_F ;
        }
        else if ( familyname == SetFamily.RCCGir.FamilyName ) {
          RCGir = SetFamily.RCCGir ;
        }
        else if ( familyname == SetFamily.RCCBeam.FamilyName ) {
          RCGir = SetFamily.RCCBeam ;
        }

        if ( RCGir == null ) return retID ;


        string kind_beam = Data.GetParameter_string( symbol, RCGir.kind_beam ) ;
        s.isFoundation = Check_isFoundation( kind_beam, ins.LevelId ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, RCGir.name ) ;
        s.strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, RCGir.strength_concrete ) ) ;


        //形状
        double ws = Data.GetParameter_double( symbol, RCGir.width_start ) ;
        double we = Data.GetParameter_double( symbol, RCGir.width_end ) ;
        double ds = Data.GetParameter_double( symbol, RCGir.depth_start ) ;
        double de = Data.GetParameter_double( symbol, RCGir.depth_end ) ;
        s.StbSecFigureBeam_RC = new StbSecFigureBeam_RC() { Items = new List<object>(), } ;

        if ( Math.Abs( ws - we ) < 0.01 && Math.Abs( ds - de ) < 0.01 ) {
          s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Straight() { width = ws, depth = ds, } ) ;
        }
        else {
          s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Taper() { pos = StbSecBeam_RC_TaperPos.START, width = ws, depth = ds, } ) ;

          s.StbSecFigureBeam_RC.Items.Add( new StbSecBeam_RC_Taper() { pos = StbSecBeam_RC_TaperPos.END, width = we, depth = de, } ) ;
        }


        //配筋
        s.StbSecBarArrangementBeam_RC = new StbSecBarArrangementBeam_RC()
        {
          depth_cover_left = Data.GetParameter_double( symbol, RCGir.depth_cover_left ),
          depth_cover_right = Data.GetParameter_double( symbol, RCGir.depth_cover_right ),
          depth_cover_top = Data.GetParameter_double( symbol, RCGir.depth_cover_top ),
          depth_cover_bottom = Data.GetParameter_double( symbol, RCGir.depth_cover_bottom ),
          interval = Data.GetParameter_double( symbol, RCGir.interval_reinforcement ),
          center_top = Data.GetParameter_double( symbol, RCGir.center_reinforcement_top ),
          center_bottom = Data.GetParameter_double( symbol, RCGir.center_reinforcement_bottom ),
          center_side = 0,
          center_interval = 0,
          length_bar_start = Data.GetParameter_double( symbol, RCGir.bar_length_start ),
          length_bar_end = Data.GetParameter_double( symbol, RCGir.bar_length_end ),
          Items = new List<object>(),
        } ;

        List<StbSecBarBeam_RC_Same> bar = new List<StbSecBarBeam_RC_Same>() ;
        for ( int b = 0 ; b < RCGir.count_main_top_1st.Length ; ++b ) {
          var bb = new StbSecBarBeam_RC_Same()
          {
            D_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_main_top[ b ] ),
            D_2nd_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_2nd_main_top[ b ] ),
            D_stirrup = Data.GetParameter_string( symbol, RCGir.D_stirrup[ b ] ),
            D_web = Data.GetParameter_string( symbol, RCGir.D_reinforcement_web[ b ] ),
            D_bar_spacing = Data.GetParameter_string( symbol, RCGir.D_bar_spacing[ b ] ),
            strength_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_main ),
            strength_2nd_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_2nd_main ),
            strength_stirrup = Data.GetParameter_string( symbol, RCGir.strength_stirrup ),
            strength_web = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_web ),
            strength_bar_spacing = Data.GetParameter_string( symbol, RCGir.strength_bar_spacing ),
            N_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_main_top_1st[ b ] ),
            N_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_main_top_2nd[ b ] ),
            N_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_main_top_3rd[ b ] ),
            N_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_main_bottom_1st[ b ] ),
            N_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_2nd[ b ] ),
            N_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_3rd[ b ] ),
            N_2nd_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_1st[ b ] ),
            N_2nd_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_2nd[ b ] ),
            N_2nd_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_3rd[ b ] ),
            N_2nd_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_1st[ b ] ),
            N_2nd_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_2nd[ b ] ),
            N_2nd_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_3rd[ b ] ),
            N_stirrup = Data.GetParameter_int( symbol, RCGir.count_stirrup[ b ] ),
            N_web = Data.GetParameter_int( symbol, RCGir.count_web[ b ] ),
            N_bar_spacing = Data.GetParameter_int( symbol, RCGir.count_bar_spacing[ b ] ),
            pitch_stirrup = Data.GetParameter_double( symbol, RCGir.pitch_stirrup[ b ] ),
            pitch_bar_spacing = Data.GetParameter_double( symbol, RCGir.pitch_bar_spacing[ b ] ),
          } ;
          bar.Add( bb ) ;
        }


        bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_RC_ThreeTypes( bar[ 1 ] ) ) ;
        if ( isSame1 ) {
          s.StbSecBarArrangementBeam_RC.Items.Add( bar[ 0 ] ) ;
        }
        else {
          s.StbSecBarArrangementBeam_RC.Items.Add( new StbSecBarBeam_RC_StartEnd( bar[ 0 ] ) { pos = StbSecBarBeam_RC_StartEndPos.START } ) ;
          s.StbSecBarArrangementBeam_RC.Items.Add( new StbSecBarBeam_RC_StartEnd( bar[ 1 ] ) { pos = StbSecBarBeam_RC_StartEndPos.END } ) ;
        }


        retID = stb.StbModel.StbSections.StbSecBeam_RC.Find( x => CompareTo_StbSecBeam_RC( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_RC.Add( s ) ;
          retID = s.id ;
        }
      }

      else if ( familyname == SetFamily.SGirH.FamilyName || familyname == SetFamily.SGirH_Haunch.FamilyName || familyname == SetFamily.SBeamH.FamilyName || familyname == SetFamily.SBeamH_Haunch.FamilyName || familyname == SetFamily.SGirBH.FamilyName || familyname == SetFamily.SBeamBH.FamilyName || familyname == SetFamily.SGirC.FamilyName || familyname == SetFamily.SBeamC.FamilyName || familyname == SetFamily.SGirL.FamilyName || familyname == SetFamily.SBeamL.FamilyName || familyname == SetFamily.SGirLipC.FamilyName || familyname == SetFamily.SBeamLipC.FamilyName || familyname == SetFamily.SCGirC.FamilyName || familyname == SetFamily.SCBeamC.FamilyName || familyname == SetFamily.SCGirL.FamilyName || familyname == SetFamily.SCBeamL.FamilyName || familyname == SetFamily.SCGirLipC.FamilyName || familyname == SetFamily.SCBeamLipC.FamilyName ) {
        id_sect++ ;

        StbSecBeam_S s = new StbSecBeam_S()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isOutin = false,
          StbSecSteelFigureBeam_S = new StbSecSteelFigureBeam_S() { Items = new List<object>(), },
        } ;

        string[][] paramName = new string[ 4 ][] ;

        if ( familyname == SetFamily.SGirH.FamilyName || familyname == SetFamily.SGirH_Haunch.FamilyName || familyname == SetFamily.SBeamH.FamilyName || familyname == SetFamily.SBeamH_Haunch.FamilyName || familyname == SetFamily.SGirBH.FamilyName || familyname == SetFamily.SBeamBH.FamilyName ) {
          FamilyStructure.S_Gir_H SGir = null ;
          if ( familyname == SetFamily.SGirH.FamilyName ) {
            SGir = SetFamily.SGirH ;
          }
          else if ( familyname == SetFamily.SGirH_Haunch.FamilyName ) {
            SGir = SetFamily.SGirH_Haunch ;
          }
          else if ( familyname == SetFamily.SBeamH.FamilyName ) {
            SGir = SetFamily.SBeamH ;
          }
          else if ( familyname == SetFamily.SBeamH_Haunch.FamilyName ) {
            SGir = SetFamily.SBeamH_Haunch ;
          }
          else if ( familyname == SetFamily.SGirBH.FamilyName ) {
            SGir = SetFamily.SGirBH ;
          }
          else if ( familyname == SetFamily.SBeamBH.FamilyName ) {
            SGir = SetFamily.SBeamBH ;
          }

          paramName[ 0 ] = new string[] { SGir.kind_beam } ;
          paramName[ 1 ] = new string[] { SGir.name } ;
          paramName[ 2 ] = SGir.strength_main ;
          paramName[ 3 ] = SGir.strength_web ;
        }
        else if ( familyname == SetFamily.SGirC.FamilyName || familyname == SetFamily.SBeamC.FamilyName || familyname == SetFamily.SCGirC.FamilyName || familyname == SetFamily.SCBeamC.FamilyName ) {
          FamilyStructure.S_Gir_C SGir = null ;
          if ( familyname == SetFamily.SGirC.FamilyName ) {
            SGir = SetFamily.SGirC ;
          }
          else if ( familyname == SetFamily.SBeamC.FamilyName ) {
            SGir = SetFamily.SBeamC ;
          }
          else if ( familyname == SetFamily.SCGirC.FamilyName ) {
            SGir = SetFamily.SCGirC ;
          }
          else if ( familyname == SetFamily.SCBeamC.FamilyName ) {
            SGir = SetFamily.SCBeamC ;
          }

          paramName[ 0 ] = new string[] { SGir.kind_beam } ;
          paramName[ 1 ] = new string[] { SGir.name } ;
          paramName[ 2 ] = new string[] { SGir.strength, SGir.strength, SGir.strength } ;
          paramName[ 3 ] = new string[] { "", "", "" } ;
        }
        else if ( familyname == SetFamily.SGirL.FamilyName || familyname == SetFamily.SBeamL.FamilyName || familyname == SetFamily.SCGirL.FamilyName || familyname == SetFamily.SCBeamL.FamilyName ) {
          FamilyStructure.S_Gir_L SGir = null ;
          if ( familyname == SetFamily.SGirL.FamilyName ) {
            SGir = SetFamily.SGirL ;
          }
          else if ( familyname == SetFamily.SBeamL.FamilyName ) {
            SGir = SetFamily.SBeamL ;
          }
          else if ( familyname == SetFamily.SCGirL.FamilyName ) {
            SGir = SetFamily.SCGirL ;
          }
          else if ( familyname == SetFamily.SCBeamL.FamilyName ) {
            SGir = SetFamily.SCBeamL ;
          }

          paramName[ 0 ] = new string[] { SGir.kind_beam } ;
          paramName[ 1 ] = new string[] { SGir.name } ;
          paramName[ 2 ] = new string[] { SGir.strength, SGir.strength, SGir.strength } ;
          paramName[ 3 ] = new string[] { "", "", "" } ;
        }
        else if ( familyname == SetFamily.SGirLipC.FamilyName || familyname == SetFamily.SBeamLipC.FamilyName || familyname == SetFamily.SCGirLipC.FamilyName || familyname == SetFamily.SCBeamLipC.FamilyName ) {
          FamilyStructure.S_Gir_LipC SGir = null ;
          if ( familyname == SetFamily.SGirLipC.FamilyName ) {
            SGir = SetFamily.SGirLipC ;
          }
          else if ( familyname == SetFamily.SBeamLipC.FamilyName ) {
            SGir = SetFamily.SBeamLipC ;
          }
          else if ( familyname == SetFamily.SCGirLipC.FamilyName ) {
            SGir = SetFamily.SCGirLipC ;
          }
          else if ( familyname == SetFamily.SCBeamLipC.FamilyName ) {
            SGir = SetFamily.SCBeamLipC ;
          }

          paramName[ 0 ] = new string[] { SGir.kind_beam } ;
          paramName[ 1 ] = new string[] { SGir.name } ;
          paramName[ 2 ] = new string[] { SGir.strength, SGir.strength, SGir.strength } ;
          paramName[ 3 ] = new string[] { "", "", "" } ;
        }


        string kind_beam = Data.GetParameter_string( symbol, paramName[ 0 ][ 0 ] ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, paramName[ 1 ][ 0 ] ) ;

        //鉄骨
        string[] shape = new string[ 3 ] ;
        string[] strength_main = new string[ 3 ] ;
        string[] strength_web = new string[ 3 ] ;
        for ( int LCR = 0 ; LCR < shape.Length ; ++LCR ) {
          shape[ LCR ] = GetSteelName( symbol, 0, LCR ) ;
          strength_main[ LCR ] = Data.GetParameter_string( symbol, paramName[ 2 ][ LCR ] ) ;
          strength_web[ LCR ] = Data.GetParameter_string( symbol, paramName[ 3 ][ LCR ] ) ;
        }

        if ( shape[ 0 ] == shape[ 1 ] && shape[ 1 ] == shape[ 2 ] && strength_main[ 0 ] == strength_main[ 1 ] && strength_main[ 1 ] == strength_main[ 2 ] && strength_web[ 0 ] == strength_web[ 1 ] && strength_web[ 1 ] == strength_web[ 2 ] ) {
          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Straight() { shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ], } ) ;
        }
        else {
          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.START, shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ],
          } ) ;

          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.CENTER, shape = shape[ 1 ], strength_main = strength_main[ 1 ], strength_web = strength_web[ 1 ],
          } ) ;

          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.END, shape = shape[ 2 ], strength_main = strength_main[ 2 ], strength_web = strength_web[ 2 ],
          } ) ;
        }

        retID = stb.StbModel.StbSections.StbSecBeam_S.Find( x => CompareTo_StbSecBeam_S( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_S.Add( s ) ;
          retID = s.id ;
        }
      }
      else if ( familyname == SetFamily.SCGirH.FamilyName || familyname == SetFamily.SCGirBH.FamilyName || familyname == SetFamily.SCBeamBH.FamilyName || familyname == SetFamily.SCBeamH.FamilyName ) {
        id_sect++ ;

        StbSecBeam_S s = new StbSecBeam_S()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isOutin = false,
          StbSecSteelFigureBeam_S = new StbSecSteelFigureBeam_S() { Items = new List<object>(), },
        } ;

        string[][] paramName = new string[ 4 ][] ;

        if ( familyname == SetFamily.SCGirH.FamilyName || familyname == SetFamily.SCGirBH.FamilyName || familyname == SetFamily.SCBeamBH.FamilyName || familyname == SetFamily.SCBeamH.FamilyName ) {
          FamilyStructure.S_CGir_H SGir = null ;
          if ( familyname == SetFamily.SCGirH.FamilyName ) {
            SGir = SetFamily.SCGirH ;
          }
          else if ( familyname == SetFamily.SCGirBH.FamilyName ) {
            SGir = SetFamily.SCGirBH ;
          }
          else if ( familyname == SetFamily.SCBeamBH.FamilyName ) {
            SGir = SetFamily.SCBeamBH ;
          }
          else if ( familyname == SetFamily.SCBeamH.FamilyName ) {
            SGir = SetFamily.SCBeamH ;
          }

          paramName[ 0 ] = new string[] { SGir.kind_beam } ;
          paramName[ 1 ] = new string[] { SGir.name } ;
          paramName[ 2 ] = SGir.strength_main ;
          paramName[ 3 ] = SGir.strength_web ;
        }

        string kind_beam = Data.GetParameter_string( symbol, paramName[ 0 ][ 0 ] ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, paramName[ 1 ][ 0 ] ) ;

        //鉄骨
        string[] shape = new string[ 2 ] ;
        string[] strength_main = new string[ 2 ] ;
        string[] strength_web = new string[ 2 ] ;
        for ( int LCR = 0 ; LCR < shape.Length ; ++LCR ) {
          shape[ LCR ] = GetSteelName( symbol, 0, LCR ) ;
          strength_main[ LCR ] = Data.GetParameter_string( symbol, paramName[ 2 ][ LCR ] ) ;
          strength_web[ LCR ] = Data.GetParameter_string( symbol, paramName[ 3 ][ LCR ] ) ;
        }

        if ( shape[ 0 ] == shape[ 1 ] && strength_main[ 0 ] == strength_main[ 1 ] && strength_web[ 0 ] == strength_web[ 1 ] ) {
          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Straight() { shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ], } ) ;
        }
        else {
          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Taper()
          {
            pos = StbSecSteelBeam_S_TaperPos.START, shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ],
          } ) ;

          s.StbSecSteelFigureBeam_S.Items.Add( new StbSecSteelBeam_S_Taper()
          {
            pos = StbSecSteelBeam_S_TaperPos.END, shape = shape[ 1 ], strength_main = strength_main[ 1 ], strength_web = strength_web[ 1 ],
          } ) ;
        }

        retID = stb.StbModel.StbSections.StbSecBeam_S.Find( x => CompareTo_StbSecBeam_S( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_S.Add( s ) ;
          retID = s.id ;
        }
      }

      else if ( familyname == SetFamily.SRCGirH.FamilyName || familyname == SetFamily.SRCBeamH.FamilyName ) {
        id_sect++ ;

        StbSecBeam_SRC s = new StbSecBeam_SRC()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isOutin = false,
          StbSecFigureBeam_SRC = new StbSecFigureBeam_SRC() { Items = new List<object>(), },
        } ;

        FamilyStructure.SRC_Gir RCGir = null ;
        if ( familyname == SetFamily.SRCGirH.FamilyName ) {
          RCGir = SetFamily.SRCGirH ;
        }
        else if ( familyname == SetFamily.SRCBeamH.FamilyName ) {
          RCGir = SetFamily.SRCBeamH ;
        }

        if ( RCGir == null ) return retID ;


        string kind_beam = Data.GetParameter_string( symbol, RCGir.kind_beam ) ;
        s.isFoundation = Check_isFoundation( kind_beam, ins.LevelId ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, RCGir.name ) ;
        s.strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, RCGir.strength_concrete ) ) ;


        //形状
        double ws = Data.GetParameter_double( symbol, RCGir.width_start ) ;
        double wc = Data.GetParameter_double( symbol, RCGir.width_center ) ;
        double we = Data.GetParameter_double( symbol, RCGir.width_end ) ;
        double ds = Data.GetParameter_double( symbol, RCGir.depth_start ) ;
        double dc = Data.GetParameter_double( symbol, RCGir.depth_center ) ;
        double de = Data.GetParameter_double( symbol, RCGir.depth_end ) ;

        if ( Math.Abs( ws - wc ) < 0.01 && Math.Abs( we - wc ) < 0.01 && Math.Abs( ds - dc ) < 0.01 && Math.Abs( de - dc ) < 0.01 ) {
          s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Straight() { width = wc, depth = dc, } ) ;
        }
        else {
          if ( s.isCanti ) {
            if ( ws < 0.1 ) ws = wc ;
            if ( we < 0.1 ) we = wc ;
            if ( ds < 0.1 ) ds = dc ;
            if ( de < 0.1 ) de = dc ;

            s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Taper() { pos = StbSecBeam_RC_TaperPos.START, width = ws, depth = ds, } ) ;

            s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Taper() { pos = StbSecBeam_RC_TaperPos.END, width = we, depth = de, } ) ;
          }
          else {
            s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Haunch() { pos = StbSecBeam_RC_HaunchPos.START, width = ws, depth = ds, } ) ;

            s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Haunch() { pos = StbSecBeam_RC_HaunchPos.CENTER, width = wc, depth = dc, } ) ;

            s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Haunch() { pos = StbSecBeam_RC_HaunchPos.END, width = we, depth = de, } ) ;
          }
        }


        //配筋
        s.StbSecBarArrangementBeam_SRC = new StbSecBarArrangementBeam_SRC()
        {
          depth_cover_left = Data.GetParameter_double( symbol, RCGir.depth_cover_left ),
          depth_cover_right = Data.GetParameter_double( symbol, RCGir.depth_cover_right ),
          depth_cover_top = Data.GetParameter_double( symbol, RCGir.depth_cover_top ),
          depth_cover_bottom = Data.GetParameter_double( symbol, RCGir.depth_cover_bottom ),
          interval = Data.GetParameter_double( symbol, RCGir.interval_reinforcement ),
          center_top = Data.GetParameter_double( symbol, RCGir.center_reinforcement_top ),
          center_bottom = Data.GetParameter_double( symbol, RCGir.center_reinforcement_bottom ),
          center_side = 0,
          center_interval = 0,
          length_bar_start = 0,
          length_bar_end = 0,
          Items = new List<object>(),
        } ;

        List<StbSecBarBeam_SRC_Same> bar = new List<StbSecBarBeam_SRC_Same>() ;
        for ( int b = 0 ; b < RCGir.count_main_top_1st.Length ; ++b ) {
          var bb = new StbSecBarBeam_SRC_Same()
          {
            D_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_main_top[ b ] ),
            D_2nd_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_2nd_main_top[ b ] ),
            D_stirrup = Data.GetParameter_string( symbol, RCGir.D_stirrup[ b ] ),
            D_web = Data.GetParameter_string( symbol, RCGir.D_reinforcement_web[ b ] ),
            D_bar_spacing = Data.GetParameter_string( symbol, RCGir.D_bar_spacing[ b ] ),
            strength_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_main ),
            strength_2nd_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_2nd_main ),
            strength_stirrup = Data.GetParameter_string( symbol, RCGir.strength_stirrup ),
            strength_web = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_web ),
            strength_bar_spacing = Data.GetParameter_string( symbol, RCGir.strength_bar_spacing ),
            N_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_main_top_1st[ b ] ),
            N_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_main_top_2nd[ b ] ),
            N_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_main_top_3rd[ b ] ),
            N_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_main_bottom_1st[ b ] ),
            N_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_2nd[ b ] ),
            N_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_3rd[ b ] ),
            N_2nd_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_1st[ b ] ),
            N_2nd_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_2nd[ b ] ),
            N_2nd_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_3rd[ b ] ),
            N_2nd_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_1st[ b ] ),
            N_2nd_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_2nd[ b ] ),
            N_2nd_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_3rd[ b ] ),
            N_stirrup = Data.GetParameter_int( symbol, RCGir.count_stirrup[ b ] ),
            N_web = Data.GetParameter_int( symbol, RCGir.count_web[ b ] ),
            N_bar_spacing = Data.GetParameter_int( symbol, RCGir.count_bar_spacing[ b ] ),
            pitch_stirrup = Data.GetParameter_double( symbol, RCGir.pitch_stirrup[ b ] ),
            pitch_bar_spacing = Data.GetParameter_double( symbol, RCGir.pitch_bar_spacing[ b ] ),
          } ;
          bar.Add( bb ) ;
        }

        bool isSame0 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_SRC_ThreeTypes( bar[ 2 ] ) ) ;
        bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_SRC_ThreeTypes( bar[ 1 ] ) ) ;

        if ( isSame0 && isSame1 ) {
          s.StbSecBarArrangementBeam_SRC.Items.Add( bar[ 0 ] ) ;
        }
        else {
          s.StbSecBarArrangementBeam_SRC.Items.Add( new StbSecBarBeam_SRC_ThreeTypes( bar[ 0 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.START } ) ;
          s.StbSecBarArrangementBeam_SRC.Items.Add( new StbSecBarBeam_SRC_ThreeTypes( bar[ 1 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.CENTER } ) ;
          s.StbSecBarArrangementBeam_SRC.Items.Add( new StbSecBarBeam_SRC_ThreeTypes( bar[ 2 ] ) { pos = StbSecBarBeam_RC_ThreeTypesPos.END } ) ;
        }


        //鉄骨
        s.StbSecSteelFigureBeam_SRC = new StbSecSteelFigureBeam_SRC() { Items = new List<object>(), } ;
        string[] shape = new string[ 3 ] ;
        string[] strength_main = new string[ 3 ] ;
        string[] strength_web = new string[ 3 ] ;
        for ( int LCR = 0 ; LCR < shape.Length ; ++LCR ) {
          shape[ LCR ] = GetSteelName( symbol, 0, LCR ) ;
          strength_main[ LCR ] = Data.GetParameter_string( symbol, RCGir.strength_main[ LCR ] ) ;
          strength_web[ LCR ] = Data.GetParameter_string( symbol, RCGir.strength_web[ LCR ] ) ;
        }

        if ( shape[ 0 ] == shape[ 1 ] && shape[ 1 ] == shape[ 2 ] && strength_main[ 0 ] == strength_main[ 1 ] && strength_main[ 1 ] == strength_main[ 2 ] && strength_web[ 0 ] == strength_web[ 1 ] && strength_web[ 1 ] == strength_web[ 2 ] ) {
          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Straight() { shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ], } ) ;
        }
        else {
          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.START, shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ],
          } ) ;

          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.CENTER, shape = shape[ 1 ], strength_main = strength_main[ 1 ], strength_web = strength_web[ 1 ],
          } ) ;

          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Haunch()
          {
            pos = StbSecSteelBeam_S_HaunchPos.END, shape = shape[ 2 ], strength_main = strength_main[ 2 ], strength_web = strength_web[ 2 ],
          } ) ;
        }


        retID = stb.StbModel.StbSections.StbSecBeam_SRC.Find( x => CompareTo_StbSecBeam_SRC( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_SRC.Add( s ) ;
          retID = s.id ;
        }
      }
      else if ( familyname == SetFamily.SRCCGirH.FamilyName || familyname == SetFamily.SRCCBeamH.FamilyName ) {
        id_sect++ ;

        StbSecBeam_SRC s = new StbSecBeam_SRC()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          floor = floor,
          isOutin = false,
          StbSecFigureBeam_SRC = new StbSecFigureBeam_SRC() { Items = new List<object>(), },
        } ;

        FamilyStructure.SRC_CGir RCGir = null ;
        if ( familyname == SetFamily.SRCCGirH.FamilyName ) {
          RCGir = SetFamily.SRCCGirH ;
        }
        else if ( familyname == SetFamily.SRCCBeamH.FamilyName ) {
          RCGir = SetFamily.SRCCBeamH ;
        }

        if ( RCGir == null ) return retID ;


        string kind_beam = Data.GetParameter_string( symbol, RCGir.kind_beam ) ;
        s.isFoundation = Check_isFoundation( kind_beam, ins.LevelId ) ;
        s.isCanti = Check_isCanti( kind_beam ) ;
        s.kind_beam = kind_beam.ToUpper().Contains( "BEAM" ) ? StbSecBeam_Kind_beam.BEAM : StbSecBeam_Kind_beam.GIRDER ;

        s.name = Data.GetParameter_string( symbol, RCGir.name ) ;
        s.strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, RCGir.strength_concrete ) ) ;


        //形状
        double ws = Data.GetParameter_double( symbol, RCGir.width_start ) ;
        double we = Data.GetParameter_double( symbol, RCGir.width_end ) ;
        double ds = Data.GetParameter_double( symbol, RCGir.depth_start ) ;
        double de = Data.GetParameter_double( symbol, RCGir.depth_end ) ;
        if ( Math.Abs( ws - we ) < 0.01 && Math.Abs( ds - de ) < 0.01 ) {
          s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Straight() { width = ws, depth = ds, } ) ;
        }
        else {
          s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Taper() { pos = StbSecBeam_RC_TaperPos.START, width = ws, depth = ds, } ) ;

          s.StbSecFigureBeam_SRC.Items.Add( new StbSecBeam_SRC_Taper() { pos = StbSecBeam_RC_TaperPos.END, width = we, depth = de, } ) ;
        }


        //配筋
        s.StbSecBarArrangementBeam_SRC = new StbSecBarArrangementBeam_SRC()
        {
          depth_cover_left = Data.GetParameter_double( symbol, RCGir.depth_cover_left ),
          depth_cover_right = Data.GetParameter_double( symbol, RCGir.depth_cover_right ),
          depth_cover_top = Data.GetParameter_double( symbol, RCGir.depth_cover_top ),
          depth_cover_bottom = Data.GetParameter_double( symbol, RCGir.depth_cover_bottom ),
          interval = Data.GetParameter_double( symbol, RCGir.interval_reinforcement ),
          center_top = Data.GetParameter_double( symbol, RCGir.center_reinforcement_top ),
          center_bottom = Data.GetParameter_double( symbol, RCGir.center_reinforcement_bottom ),
          center_side = 0,
          center_interval = 0,
          length_bar_start = 0,
          length_bar_end = 0,
          Items = new List<object>(),
        } ;

        List<StbSecBarBeam_SRC_Same> bar = new List<StbSecBarBeam_SRC_Same>() ;
        for ( int b = 0 ; b < RCGir.count_main_top_1st.Length ; ++b ) {
          var bb = new StbSecBarBeam_SRC_Same()
          {
            D_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_main_top[ b ] ),
            D_2nd_main = Data.GetParameter_string( symbol, RCGir.D_reinforcement_2nd_main_top[ b ] ),
            D_stirrup = Data.GetParameter_string( symbol, RCGir.D_stirrup[ b ] ),
            D_web = Data.GetParameter_string( symbol, RCGir.D_reinforcement_web[ b ] ),
            D_bar_spacing = Data.GetParameter_string( symbol, RCGir.D_bar_spacing[ b ] ),
            strength_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_main ),
            strength_2nd_main = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_2nd_main ),
            strength_stirrup = Data.GetParameter_string( symbol, RCGir.strength_stirrup ),
            strength_web = Data.GetParameter_string( symbol, RCGir.strength_reinforcement_web ),
            strength_bar_spacing = Data.GetParameter_string( symbol, RCGir.strength_bar_spacing ),
            N_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_main_top_1st[ b ] ),
            N_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_main_top_2nd[ b ] ),
            N_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_main_top_3rd[ b ] ),
            N_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_main_bottom_1st[ b ] ),
            N_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_2nd[ b ] ),
            N_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_main_bottom_3rd[ b ] ),
            N_2nd_main_top_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_1st[ b ] ),
            N_2nd_main_top_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_2nd[ b ] ),
            N_2nd_main_top_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_top_3rd[ b ] ),
            N_2nd_main_bottom_1st = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_1st[ b ] ),
            N_2nd_main_bottom_2nd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_2nd[ b ] ),
            N_2nd_main_bottom_3rd = Data.GetParameter_int( symbol, RCGir.count_2nd_main_bottom_3rd[ b ] ),
            N_stirrup = Data.GetParameter_int( symbol, RCGir.count_stirrup[ b ] ),
            N_web = Data.GetParameter_int( symbol, RCGir.count_web[ b ] ),
            N_bar_spacing = Data.GetParameter_int( symbol, RCGir.count_bar_spacing[ b ] ),
            pitch_stirrup = Data.GetParameter_double( symbol, RCGir.pitch_stirrup[ b ] ),
            pitch_bar_spacing = Data.GetParameter_double( symbol, RCGir.pitch_bar_spacing[ b ] ),
          } ;
          bar.Add( bb ) ;
        }

        bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE( new StbSecBarBeam_RC_ThreeTypes( bar[ 0 ] ), new StbSecBarBeam_RC_ThreeTypes( bar[ 1 ] ) ) ;
        if ( isSame1 ) {
          s.StbSecBarArrangementBeam_SRC.Items.Add( bar[ 0 ] ) ;
        }
        else {
          s.StbSecBarArrangementBeam_SRC.Items.Add( new StbSecBarBeam_SRC_StartEnd( bar[ 0 ] ) { pos = StbSecBarBeam_RC_StartEndPos.START } ) ;
          s.StbSecBarArrangementBeam_SRC.Items.Add( new StbSecBarBeam_SRC_StartEnd( bar[ 1 ] ) { pos = StbSecBarBeam_RC_StartEndPos.END } ) ;
        }


        //鉄骨
        s.StbSecSteelFigureBeam_SRC = new StbSecSteelFigureBeam_SRC() { Items = new List<object>(), } ;
        string[] shape = new string[ 2 ] ;
        string[] strength_main = new string[ 2 ] ;
        string[] strength_web = new string[ 2 ] ;
        for ( int LCR = 0 ; LCR < shape.Length ; ++LCR ) {
          shape[ LCR ] = GetSteelName( symbol, 0, LCR ) ;
          strength_main[ LCR ] = Data.GetParameter_string( symbol, RCGir.strength_main[ LCR ] ) ;
          strength_web[ LCR ] = Data.GetParameter_string( symbol, RCGir.strength_web[ LCR ] ) ;
        }

        if ( shape[ 0 ] == shape[ 1 ] && strength_main[ 0 ] == strength_main[ 1 ] && strength_web[ 0 ] == strength_web[ 1 ] ) {
          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Straight() { shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ], } ) ;
        }
        else {
          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Taper()
          {
            pos = StbSecSteelBeam_S_TaperPos.START, shape = shape[ 0 ], strength_main = strength_main[ 0 ], strength_web = strength_web[ 0 ],
          } ) ;

          s.StbSecSteelFigureBeam_SRC.Items.Add( new StbSecSteelBeam_SRC_Taper()
          {
            pos = StbSecSteelBeam_S_TaperPos.END, shape = shape[ 1 ], strength_main = strength_main[ 1 ], strength_web = strength_web[ 1 ],
          } ) ;
        }


        retID = stb.StbModel.StbSections.StbSecBeam_SRC.Find( x => CompareTo_StbSecBeam_SRC( s, x ) )?.id ?? -1 ;
        if ( retID < 0 ) {
          stb.StbModel.StbSections.StbSecBeam_SRC.Add( s ) ;
          retID = s.id ;
        }
      }


      return retID ;
    }

    /// <summary>
    /// 梁の出力
    /// </summary>
    /// <param name="usage"></param>
    private static void Export_Girder( StructuralInstanceUsage usage )
    {
      List<string> AllFamilyName = new List<string>() ;
      if ( usage == StructuralInstanceUsage.Girder ) {
        for ( int i = 0 ; i < SetFamily.GirFName.FamilyName.Length ; ++i ) {
          AllFamilyName.AddRange( SetFamily.GirFName.FamilyName[ i ] ) ;
        }

        for ( int i = 0 ; i < SetFamily.CGirFName.FamilyName.Length ; ++i ) {
          AllFamilyName.AddRange( SetFamily.CGirFName.FamilyName[ i ] ) ;
        }
      }
      else {
        for ( int i = 0 ; i < SetFamily.BeamFName.FamilyName.Length ; ++i ) {
          AllFamilyName.AddRange( SetFamily.BeamFName.FamilyName[ i ] ) ;
        }

        for ( int i = 0 ; i < SetFamily.CBeamFName.FamilyName.Length ; ++i ) {
          AllFamilyName.AddRange( SetFamily.CBeamFName.FamilyName[ i ] ) ;
        }
      }

      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter1 = new ElementCategoryFilter( BuiltInCategory.OST_StructuralFraming ) ;

      ParameterValueProvider provider = new ParameterValueProvider( new ElementId( BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM ) ) ;
      FilterNumericRuleEvaluator evaluator1 = new FilterNumericEquals() ;
      FilterRule rule2 = new FilterIntegerRule( provider, evaluator1, (int)usage ) ;
      ElementParameterFilter filter2 = new ElementParameterFilter( rule2 ) ;

      LogicalAndFilter filter = new LogicalAndFilter( filter1, filter2 ) ;

      List<FamilyInstance> instances = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where( x => AllFamilyName.Contains( x.Symbol.Family.Name ) && ! x.Symbol.Family.IsInPlace ).ToList() ;

      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;
      var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager( Commons.doc ) ;

      for ( int i = 0 ; i < instances.Count ; ++i ) {
        XYZ ps1 = new XYZ() ;
        XYZ pe1 = new XYZ() ;
        XYZ ps2 = Data.GetFramingCoordinate( instances[ i ], 0 ) ;
        XYZ pe2 = Data.GetFramingCoordinate( instances[ i ], 1 ) ;

        if ( amanager.HasAssociation( instances[ i ].Id ) && Commons.doc.GetElement( amanager.GetAssociatedElementId( instances[ i ].Id ) ) is AnalyticalMember member ) {
          ps1 = member.GetCurve().GetEndPoint( 0 ) ;
          pe1 = member.GetCurve().GetEndPoint( 1 ) ;
        }
        else {
          ps1 = ps2 ;
          pe1 = pe2 ;
        }

        ps1 = Commons.ft2mm( ps1 ) ;
        pe1 = Commons.ft2mm( pe1 ) ;
        ps2 = Commons.ft2mm( ps2 ) ;
        pe2 = Commons.ft2mm( pe2 ) ;

        var g = new StbGirder()
        {
          guid = GetGuid( instances[ i ], "" ), id_node_start = GetNodeId( ps1 ), id_node_end = GetNodeId( pe1 ), isFoundation = false,
        } ;

        if ( ! sect.ContainsKey( instances[ i ].Symbol.Id ) ) {
          g.id_section = Export_SecGirder( instances[ i ] ) ;
          if ( g.id_section < 0 ) continue ;
          sect.Add( instances[ i ].Symbol.Id, g.id_section ) ;
        }
        else {
          g.id_section = sect[ instances[ i ].Symbol.Id ] ;
        }

        string[] paramName = new string[ 18 ] ;
        string familyname = instances[ i ].Symbol.Family.Name ;
        if ( familyname == SetFamily.RCGir_F.FamilyName || familyname == SetFamily.RCGir_F_Haunch.FamilyName || familyname == SetFamily.RCBeam_F.FamilyName || familyname == SetFamily.RCBeam_F_Haunch.FamilyName || familyname == SetFamily.RCGir.FamilyName || familyname == SetFamily.RCGir_Haunch.FamilyName || familyname == SetFamily.RCBeam.FamilyName || familyname == SetFamily.RCBeam_Haunch.FamilyName || familyname == SetFamily.RCCGir_F.FamilyName || familyname == SetFamily.RCCBeam_F.FamilyName || familyname == SetFamily.RCCGir.FamilyName || familyname == SetFamily.RCCBeam.FamilyName ) {
          FamilyStructure.RC_Gir RCGir = null ;
          if ( familyname == SetFamily.RCGir_F.FamilyName ) {
            RCGir = SetFamily.RCGir_F ;
          }
          else if ( familyname == SetFamily.RCGir_F_Haunch.FamilyName ) {
            RCGir = SetFamily.RCGir_F_Haunch ;
          }
          else if ( familyname == SetFamily.RCBeam_F.FamilyName ) {
            RCGir = SetFamily.RCBeam_F ;
          }
          else if ( familyname == SetFamily.RCBeam_F_Haunch.FamilyName ) {
            RCGir = SetFamily.RCBeam_F_Haunch ;
          }
          else if ( familyname == SetFamily.RCGir.FamilyName ) {
            RCGir = SetFamily.RCGir ;
          }
          else if ( familyname == SetFamily.RCGir_Haunch.FamilyName ) {
            RCGir = SetFamily.RCGir_Haunch ;
          }
          else if ( familyname == SetFamily.RCBeam.FamilyName ) {
            RCGir = SetFamily.RCBeam ;
          }
          else if ( familyname == SetFamily.RCBeam_Haunch.FamilyName ) {
            RCGir = SetFamily.RCBeam_Haunch ;
          }
          else if ( familyname == SetFamily.RCCGir_F.FamilyName ) {
            RCGir = SetFamily.RCCGir_F ;
          }
          else if ( familyname == SetFamily.RCCBeam_F.FamilyName ) {
            RCGir = SetFamily.RCCBeam_F ;
          }
          else if ( familyname == SetFamily.RCCGir.FamilyName ) {
            RCGir = SetFamily.RCCGir ;
          }
          else if ( familyname == SetFamily.RCCBeam.FamilyName ) {
            RCGir = SetFamily.RCCBeam ;
          }

          g.kind_structure = StbGirderKind_structure.RC ;

          paramName[ 0 ] = RCGir.kind_beam ;
          paramName[ 1 ] = RCGir.NameMembers ;
          paramName[ 2 ] = RCGir.thickness_ex_top ;
          paramName[ 3 ] = RCGir.thickness_ex_bottom ;
          paramName[ 4 ] = RCGir.thickness_ex_right ;
          paramName[ 5 ] = RCGir.thickness_ex_left ;
          paramName[ 6 ] = "" ;
          paramName[ 7 ] = "" ;
          paramName[ 8 ] = RCGir.haunch_start ;
          paramName[ 9 ] = RCGir.haunch_end ;
          paramName[ 10 ] = "" ;
          paramName[ 11 ] = "" ;
          paramName[ 12 ] = RCGir.kind_haunch_start ;
          paramName[ 13 ] = RCGir.kind_haunch_end ;
          paramName[ 14 ] = RCGir.type_haunch_H ;
          paramName[ 15 ] = RCGir.type_haunch_V ;
          paramName[ 16 ] = "" ;
          paramName[ 17 ] = "" ;
        }
        else if ( familyname == SetFamily.SGirH.FamilyName || familyname == SetFamily.SGirH_Haunch.FamilyName || familyname == SetFamily.SBeamH.FamilyName || familyname == SetFamily.SBeamH_Haunch.FamilyName || familyname == SetFamily.SGirBH.FamilyName || familyname == SetFamily.SBeamBH.FamilyName || familyname == SetFamily.SCGirH.FamilyName || familyname == SetFamily.SCGirBH.FamilyName || familyname == SetFamily.SCBeamBH.FamilyName || familyname == SetFamily.SCBeamH.FamilyName ) {
          FamilyStructure.S_Gir_H SGir = null ;
          if ( familyname == SetFamily.SGirH.FamilyName ) {
            SGir = SetFamily.SGirH ;
          }
          else if ( familyname == SetFamily.SGirH_Haunch.FamilyName ) {
            SGir = SetFamily.SGirH_Haunch ;
          }
          else if ( familyname == SetFamily.SBeamH.FamilyName ) {
            SGir = SetFamily.SBeamH ;
          }
          else if ( familyname == SetFamily.SBeamH_Haunch.FamilyName ) {
            SGir = SetFamily.SBeamH_Haunch ;
          }
          else if ( familyname == SetFamily.SGirBH.FamilyName ) {
            SGir = SetFamily.SGirBH ;
          }
          else if ( familyname == SetFamily.SBeamBH.FamilyName ) {
            SGir = SetFamily.SBeamBH ;
          }
          else if ( familyname == SetFamily.SCGirH.FamilyName ) {
            SGir = SetFamily.SCGirH ;
          }
          else if ( familyname == SetFamily.SCGirBH.FamilyName ) {
            SGir = SetFamily.SCGirBH ;
          }
          else if ( familyname == SetFamily.SCBeamBH.FamilyName ) {
            SGir = SetFamily.SCBeamBH ;
          }
          else if ( familyname == SetFamily.SCBeamH.FamilyName ) {
            SGir = SetFamily.SCBeamH ;
          }

          g.kind_structure = StbGirderKind_structure.S ;

          paramName[ 0 ] = SGir.kind_beam ;
          paramName[ 1 ] = SGir.NameMembers ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = "" ;
          paramName[ 6 ] = SGir.condition_start ;
          paramName[ 7 ] = SGir.condition_end ;
          paramName[ 8 ] = SGir.haunch_start ;
          paramName[ 9 ] = SGir.haunch_end ;
          paramName[ 10 ] = SGir.joint_start ;
          paramName[ 11 ] = SGir.joint_end ;
          paramName[ 12 ] = SGir.kind_haunch_start ;
          paramName[ 13 ] = SGir.kind_haunch_end ;
          paramName[ 14 ] = SGir.type_haunch_H ;
          paramName[ 15 ] = SGir.type_haunch_V ;
          paramName[ 16 ] = SGir.kind_joint_start ;
          paramName[ 17 ] = SGir.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SGirC.FamilyName || familyname == SetFamily.SBeamC.FamilyName || familyname == SetFamily.SCGirC.FamilyName || familyname == SetFamily.SCBeamC.FamilyName ) {
          FamilyStructure.S_Gir_C SGir = null ;
          if ( familyname == SetFamily.SGirC.FamilyName ) {
            SGir = SetFamily.SGirC ;
          }
          else if ( familyname == SetFamily.SBeamC.FamilyName ) {
            SGir = SetFamily.SBeamC ;
          }
          else if ( familyname == SetFamily.SCGirC.FamilyName ) {
            SGir = SetFamily.SCGirC ;
          }
          else if ( familyname == SetFamily.SCBeamC.FamilyName ) {
            SGir = SetFamily.SCBeamC ;
          }

          g.kind_structure = StbGirderKind_structure.S ;

          paramName[ 0 ] = SGir.kind_beam ;
          paramName[ 1 ] = SGir.NameMembers ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = "" ;
          paramName[ 6 ] = SGir.condition_start ;
          paramName[ 7 ] = SGir.condition_end ;
          paramName[ 8 ] = SGir.haunch_start ;
          paramName[ 9 ] = SGir.haunch_end ;
          paramName[ 10 ] = SGir.joint_start ;
          paramName[ 11 ] = SGir.joint_end ;
          paramName[ 12 ] = SGir.kind_haunch_start ;
          paramName[ 13 ] = SGir.kind_haunch_end ;
          paramName[ 14 ] = SGir.type_haunch_H ;
          paramName[ 15 ] = SGir.type_haunch_V ;
          paramName[ 16 ] = SGir.kind_joint_start ;
          paramName[ 17 ] = SGir.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SGirL.FamilyName || familyname == SetFamily.SBeamL.FamilyName || familyname == SetFamily.SCGirL.FamilyName || familyname == SetFamily.SCBeamL.FamilyName ) {
          FamilyStructure.S_Gir_L SGir = null ;
          if ( familyname == SetFamily.SGirL.FamilyName ) {
            SGir = SetFamily.SGirL ;
          }
          else if ( familyname == SetFamily.SBeamL.FamilyName ) {
            SGir = SetFamily.SBeamL ;
          }
          else if ( familyname == SetFamily.SCGirL.FamilyName ) {
            SGir = SetFamily.SCGirL ;
          }
          else if ( familyname == SetFamily.SCBeamL.FamilyName ) {
            SGir = SetFamily.SCBeamL ;
          }

          g.kind_structure = StbGirderKind_structure.S ;

          paramName[ 0 ] = SGir.kind_beam ;
          paramName[ 1 ] = SGir.NameMembers ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = "" ;
          paramName[ 6 ] = SGir.condition_start ;
          paramName[ 7 ] = SGir.condition_end ;
          paramName[ 8 ] = SGir.haunch_start ;
          paramName[ 9 ] = SGir.haunch_end ;
          paramName[ 10 ] = SGir.joint_start ;
          paramName[ 11 ] = SGir.joint_end ;
          paramName[ 12 ] = SGir.kind_haunch_start ;
          paramName[ 13 ] = SGir.kind_haunch_end ;
          paramName[ 14 ] = SGir.type_haunch_H ;
          paramName[ 15 ] = SGir.type_haunch_V ;
          paramName[ 16 ] = SGir.kind_joint_start ;
          paramName[ 17 ] = SGir.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SGirLipC.FamilyName || familyname == SetFamily.SBeamLipC.FamilyName || familyname == SetFamily.SCGirLipC.FamilyName || familyname == SetFamily.SCBeamLipC.FamilyName ) {
          FamilyStructure.S_Gir_LipC SGir = null ;
          if ( familyname == SetFamily.SGirLipC.FamilyName ) {
            SGir = SetFamily.SGirLipC ;
          }
          else if ( familyname == SetFamily.SBeamLipC.FamilyName ) {
            SGir = SetFamily.SBeamLipC ;
          }
          else if ( familyname == SetFamily.SCGirLipC.FamilyName ) {
            SGir = SetFamily.SCGirLipC ;
          }
          else if ( familyname == SetFamily.SCBeamLipC.FamilyName ) {
            SGir = SetFamily.SCBeamLipC ;
          }

          g.kind_structure = StbGirderKind_structure.S ;

          paramName[ 0 ] = SGir.kind_beam ;
          paramName[ 1 ] = SGir.NameMembers ;
          paramName[ 2 ] = "" ;
          paramName[ 3 ] = "" ;
          paramName[ 4 ] = "" ;
          paramName[ 5 ] = "" ;
          paramName[ 6 ] = SGir.condition_start ;
          paramName[ 7 ] = SGir.condition_end ;
          paramName[ 8 ] = SGir.haunch_start ;
          paramName[ 9 ] = SGir.haunch_end ;
          paramName[ 10 ] = SGir.joint_start ;
          paramName[ 11 ] = SGir.joint_end ;
          paramName[ 12 ] = SGir.kind_haunch_start ;
          paramName[ 13 ] = SGir.kind_haunch_end ;
          paramName[ 14 ] = SGir.type_haunch_H ;
          paramName[ 15 ] = SGir.type_haunch_V ;
          paramName[ 16 ] = SGir.kind_joint_start ;
          paramName[ 17 ] = SGir.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SRCGirH.FamilyName || familyname == SetFamily.SRCBeamH.FamilyName ) {
          FamilyStructure.SRC_Gir SRCGir = null ;
          if ( familyname == SetFamily.SRCGirH.FamilyName ) {
            SRCGir = SetFamily.SRCGirH ;
          }
          else if ( familyname == SetFamily.SRCBeamH.FamilyName ) {
            SRCGir = SetFamily.SRCBeamH ;
          }

          g.kind_structure = StbGirderKind_structure.SRC ;

          paramName[ 0 ] = SRCGir.kind_beam ;
          paramName[ 1 ] = SRCGir.NameMembers ;
          paramName[ 2 ] = SRCGir.thickness_ex_top ;
          paramName[ 3 ] = SRCGir.thickness_ex_bottom ;
          paramName[ 4 ] = SRCGir.thickness_ex_right ;
          paramName[ 5 ] = SRCGir.thickness_ex_left ;
          paramName[ 6 ] = SRCGir.condition_start ;
          paramName[ 7 ] = SRCGir.condition_end ;
          paramName[ 8 ] = SRCGir.haunch_start ;
          paramName[ 9 ] = SRCGir.haunch_end ;
          paramName[ 10 ] = SRCGir.joint_start ;
          paramName[ 11 ] = SRCGir.joint_end ;
          paramName[ 12 ] = SRCGir.kind_haunch_start ;
          paramName[ 13 ] = SRCGir.kind_haunch_end ;
          paramName[ 14 ] = SRCGir.type_haunch_H ;
          paramName[ 15 ] = SRCGir.type_haunch_V ;
          paramName[ 16 ] = SRCGir.kind_joint_start ;
          paramName[ 17 ] = SRCGir.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SRCCGirH.FamilyName || familyname == SetFamily.SRCCBeamH.FamilyName ) {
          FamilyStructure.SRC_CGir SRCGir = null ;
          if ( familyname == SetFamily.SRCCGirH.FamilyName ) {
            SRCGir = SetFamily.SRCCGirH ;
          }
          else if ( familyname == SetFamily.SRCCBeamH.FamilyName ) {
            SRCGir = SetFamily.SRCCBeamH ;
          }

          g.kind_structure = StbGirderKind_structure.SRC ;

          paramName[ 0 ] = SRCGir.kind_beam ;
          paramName[ 1 ] = SRCGir.NameMembers ;
          paramName[ 2 ] = SRCGir.thickness_ex_top ;
          paramName[ 3 ] = SRCGir.thickness_ex_bottom ;
          paramName[ 4 ] = SRCGir.thickness_ex_right ;
          paramName[ 5 ] = SRCGir.thickness_ex_left ;
          paramName[ 6 ] = SRCGir.condition_start ;
          paramName[ 7 ] = SRCGir.condition_end ;
          paramName[ 8 ] = SRCGir.haunch_start ;
          paramName[ 9 ] = SRCGir.haunch_end ;
          paramName[ 10 ] = SRCGir.joint_start ;
          paramName[ 11 ] = SRCGir.joint_end ;
          paramName[ 12 ] = SRCGir.kind_haunch_start ;
          paramName[ 13 ] = SRCGir.kind_haunch_end ;
          paramName[ 14 ] = SRCGir.type_haunch_H ;
          paramName[ 15 ] = SRCGir.type_haunch_V ;
          paramName[ 16 ] = SRCGir.kind_joint_start ;
          paramName[ 17 ] = SRCGir.kind_joint_end ;
        }
        else {
          continue ;
        }

        id++ ;
        g.id = id ;
        g.rotate = Data.GetParameter_Angle( instances[ i ], BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE ) ;

        string kind_beam = Data.GetParameter_string( instances[ i ].Symbol, paramName[ 0 ] ) ;
        g.isFoundation = Check_isFoundation( kind_beam, instances[ i ].get_Parameter( BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM ).AsElementId() ) ;

        XYZ v1 = ( pe1 - ps1 ).Normalize() ;
        XYZ v2 = ( pe2 - ps2 ).Normalize() ;
        XYZ offset_s = ps2 - ps1 ;
        XYZ offset_e = pe2 - pe1 ;


        g.offset_start_X = offset_s.X ;
        g.offset_start_Y = offset_s.Y ;
        g.offset_start_Z = offset_s.Z ;
        g.offset_end_X = offset_e.X ;
        g.offset_end_Y = offset_e.Y ;
        g.offset_end_Z = offset_e.Z ;

        g.name = instances[ i ].Symbol.Name ;
        g.thickness_add_top = Data.GetParameter_double( instances[ i ], paramName[ 2 ] ) ;
        g.thickness_add_bottom = Data.GetParameter_double( instances[ i ], paramName[ 3 ] ) ;
        g.thickness_add_right = Data.GetParameter_double( instances[ i ], paramName[ 4 ] ) ;
        g.thickness_add_left = Data.GetParameter_double( instances[ i ], paramName[ 5 ] ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 6 ] ), out StbGirderCondition condition_s ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 7 ] ), out StbGirderCondition condition_e ) ;
        g.condition_start = condition_s ;
        g.condition_end = condition_e ;
        g.haunch_start = Data.GetParameter_double( instances[ i ].Symbol, paramName[ 8 ] ) ;
        g.haunch_end = Data.GetParameter_double( instances[ i ].Symbol, paramName[ 9 ] ) ;
        g.joint_start = Data.GetParameter_double( instances[ i ], paramName[ 10 ] ) ;
        g.joint_end = Data.GetParameter_double( instances[ i ], paramName[ 11 ] ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 12 ] ), out StbGirderKind_haunch haunch_s ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 13 ] ), out StbGirderKind_haunch haunch_e ) ;
        g.kind_haunch_start = haunch_s ;
        g.kind_haunch_end = haunch_e ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 14 ] ), out StbGirderType_haunch_H haunch_h ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 15 ] ), out StbGirderType_haunch_V haunch_v ) ;
        g.type_haunch_H = haunch_h ;
        g.type_haunch_V = haunch_v ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 16 ] ), out StbGirderKind_joint joint_s ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 17 ] ), out StbGirderKind_joint joint_e ) ;
        g.kind_joint_start = joint_s ;
        g.kind_joint_end = joint_e ;


        int nJoint = Data.GetParameter_int( instances[ i ], "継手数" ) ;
        if ( nJoint > 0 ) {
          //水平のみ。垂直成分は無視する
          XYZ v3 = new XYZ( instances[ i ].HandOrientation.X, instances[ i ].HandOrientation.Y, 0 ).Normalize() ;
          XYZ v4 = new XYZ( instances[ i ].FacingOrientation.X, instances[ i ].FacingOrientation.Y, 0 ).Normalize() ;

          if ( nJoint > 0 && g.joint_start > 0 ) {
            XYZ pp1 = ps1 + v3 * g.joint_start ;
            XYZ pp2 = pp1 + v4 ;
            g.joint_start = Math.Abs( Commons.LinePointDist( pp1.X, pp1.Y, pp2.X, pp2.Y, ps1.X, ps1.Y ) ) ;
          }

          if ( nJoint > 1 && g.joint_end > 0 ) {
            XYZ pp1 = pe1 - v3 * g.joint_end ;
            XYZ pp2 = pp1 + v4 ;
            g.joint_end = Math.Abs( Commons.LinePointDist( pp1.X, pp1.Y, pp2.X, pp2.Y, pe1.X, pe1.Y ) ) ;
          }
          else {
            //継手数1ならendなしとする
            g.joint_end = 0 ;
          }
        }

        if ( usage == StructuralInstanceUsage.Joist ) {
          StbBeam b = new StbBeam( g ) ;
          stb.StbModel.StbMembers.StbBeams.Add( b ) ;
          Data.AddLog( Data.LogCode.beam, instances[ i ], g.id, g.id_section ) ;
        }
        else {
          stb.StbModel.StbMembers.StbGirders.Add( g ) ;
          Data.AddLog( Data.LogCode.girder, instances[ i ], g.id, g.id_section ) ;
        }
      }
    }

    #endregion


    #region 壁

    /// <summary>
    /// 壁の外周座標の取得
    /// </summary>
    /// <param name="w">壁</param>
    /// <param name="op">開口</param>
    /// <returns>座標[mm]</returns>
    private static List<XYZ> GetWallCoord( Wall w, List<Opening> op )
    {
      List<XYZ> points = new List<XYZ>() ;

      LocationCurve locC = w.Location as LocationCurve ;
      XYZ p0 = locC.Curve.GetEndPoint( 0 ) ;
      XYZ p1 = locC.Curve.GetEndPoint( 1 ) ;
      XYZ pp = ( p0.DistanceTo( XYZ.Zero ) < p1.DistanceTo( XYZ.Zero ) ? p0 : p1 ) ;
      XYZ v1 = ( p1 - p0 ).Normalize() ;

      var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager( Commons.doc ) ;
      if ( Commons.doc.GetElement( amanager.GetAssociatedElementId( w.Id ) ) is AnalyticalPanel panel ) {
        foreach ( Line line in panel.GetOuterContour() ) {
          points.Add( line.GetEndPoint( 0 ) ) ;
        }
      }
      else {
        //LocationCurve＋Heightで四点求める

        double height = 0 ;
        Parameter param = w.get_Parameter( BuiltInParameter.WALL_HEIGHT_TYPE ) ;
        ElementId topLV = param.AsElementId() ;
        if ( topLV.Value() == -1 ) {
          param = w.get_Parameter( BuiltInParameter.WALL_USER_HEIGHT_PARAM ) ;
          height = param.AsDouble() ;
        }
        else {
          height = Levels.Find( x => x.Id == topLV ).ProjectElevation ;
          height += w.get_Parameter( BuiltInParameter.WALL_TOP_OFFSET ).AsDouble() ;

          height -= Levels.Find( x => x.Id == w.LevelId ).ProjectElevation ;
          height -= w.get_Parameter( BuiltInParameter.WALL_BASE_OFFSET ).AsDouble() ;
        }

        points.Add( p0 ) ;
        points.Add( p1 ) ;
        points.Add( p1 + XYZ.BasisZ * height ) ;
        points.Add( p0 + XYZ.BasisZ * height ) ;
      }

      double mindist = points.Min( x => x.DistanceTo( pp ) ) ;
      int index = points.FindIndex( x => Math.Abs( x.DistanceTo( pp ) - mindist ) < 0.001 ) ;
      index = Math.Max( index, 0 ) ;

      int index2 = index + 1 ;
      if ( index2 >= points.Count ) index2 = 0 ;

      XYZ v2 = ( points[ index2 ] - points[ index ] ).Normalize() ;
      bool reverse = ( v1.CrossProduct( v2 ).Normalize().GetLength() > 0.001 ) ;

      List<XYZ> points2 = new List<XYZ>( points.Count ) ;

      for ( int i = 0 ; i < points.Count ; i++ ) {
        int ii = i + index ;
        if ( ii >= points.Count ) ii = ii - points.Count ;

        if ( i == 0 || ! reverse ) {
          points2.Add( Commons.ft2mm( points[ ii ] ) ) ;
        }
        else {
          points2.Insert( 1, Commons.ft2mm( points[ ii ] ) ) ;
        }
      }

      return points2 ;
    }

    /// <summary>
    /// 壁の出力
    /// </summary>
    private static void Export_Wall()
    {
      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_Walls ) ;
      List<Wall> instances = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<Wall>().ToList() ;

      collector = new FilteredElementCollector( Commons.doc ) ;
      filter = new ElementCategoryFilter( BuiltInCategory.OST_SWallRectOpening ) ;
      List<Opening> opens = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList() ;


      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;

      for ( int i = 0 ; i < instances.Count ; ++i ) {
        if ( ! Data.Check_Analytical_Model( instances[ i ] ) ) {
          Data.AddWarning( -2, instances[ i ] ) ;
          continue ;
        }


        List<Opening> opens2 = opens.Where( x => x.Host.Id == instances[ i ].Id ).ToList() ;

        List<XYZ> points = GetWallCoord( instances[ i ], opens2 ) ;
        if ( points.Count <= 2 ) {
          Data.AddWarning( -1, instances[ i ] ) ;
          continue ;
        }

        if ( Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.name ) == "" ) {
          Data.AddWarning( -3, instances[ i ] ) ;
          continue ;
        }

        id++ ;

        StbWall w = new StbWall()
        {
          id = id,
          guid = GetGuid( instances[ i ], "" ),
          kind_structure = "RC",
          StbNodeIdOrder = string.Join( " ", points.Select( a => GetNodeId( a ).ToString() ) ),
          StbWallOffsetList = new List<StbWallOffset>(),
          StbOpenIdList = new List<StbOpenId>(),
        } ;

        LocationCurve locC = instances[ i ].Location as LocationCurve ;
        XYZ wp0 = Commons.ft2mm( locC.Curve.GetEndPoint( 0 ) ) ;
        XYZ wp1 = Commons.ft2mm( locC.Curve.GetEndPoint( 1 ) ) ;
        XYZ v0 = ( wp1 - wp0 ).Normalize() ;
        XYZ v1 = ( points[ 1 ] - points[ 0 ] ).Normalize() ;
        XYZ v2 = XYZ.BasisZ.CrossProduct( v1 ).Normalize() ;

        if ( v0.CrossProduct( v1 ).GetLength() < 0.001 ) {
          double offset = Commons.LinePointDist( points[ 0 ].X, points[ 0 ].Y, points[ 1 ].X, points[ 1 ].Y, wp0.X, wp0.Y ) ;
          if ( Math.Abs( offset ) >= 1 ) {
            //直交行列の逆行列は転置行列
            //double offset_x = v1.X * 0 + v2.X * offset + 0 * 0;
            //double offset_y = v1.Y * 0 + v2.Y * offset + 0 * 0;
            //double offset_z = v1.Z * 0 + v2.Z * offset + 1 * 0; //v3=(0,0,1)
            double offset_x = v2.X * offset ;
            double offset_y = v2.Y * offset ;
            foreach ( var id in w.StbNodeIdOrderList ) {
              w.StbWallOffsetList.Add( new StbWallOffset() { id_node = id, offset_X = offset_x, offset_Y = offset_y, offset_Z = 0 } ) ;
            }
          }
        }


        if ( ! sect.ContainsKey( instances[ i ].WallType.Id ) ) {
          #region 断面

          id_sect++ ;
          w.id_section = id_sect ;

          int secwall = 0 ;
          string arrtype = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.ArrengementType ) ;

          switch ( secwall ) {
            case 0 : //RC

              StbSecWall_RC sw = new StbSecWall_RC()
              {
                id = id_sect,
                guid = GetGuid( instances[ i ].WallType, "" ),
                name = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.name ),
                strength_concrete = Data.GetParameter_string( instances[ i ].WallType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM ),
                StbSecFigureWall_RC = new StbSecFigureWall_RC() { StbSecWall_RC_Straight = new StbSecWall_RC_Straight() { t = Commons.ft2mm( instances[ i ].WallType.Width ), }, },
                StbSecBarArrangementWall_RC = new StbSecBarArrangementWall_RC() { depth_cover_outside = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.depth_cover_outside ), depth_cover_inside = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.depth_cover_inside ), Items = new List<object>(), },
              } ;

              sw.strength_concrete = Data.GetConcreteFC( sw.strength_concrete ) ;


              //配筋
              if ( arrtype.Contains( "シングル配筋" ) ) {
                for ( int j = 0 ; j < SetFamily.Wall.D.Length ; ++j ) {
                  StbSecBarWall_RC_Single bar = new StbSecBarWall_RC_Single()
                  {
                    pos = j == 0 ? StbSecBarWall_RC_SinglePos.VERTICAL : StbSecBarWall_RC_SinglePos.HORIZONTAL, strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ), D = Data.GetParameter_D( instances[ i ].WallType, SetFamily.Wall.D[ j ], SetFamily.Wall.D2[ j ] ), pitch = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.pitch[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    sw.StbSecBarArrangementWall_RC.Items.Clear() ;
                    break ;
                  }

                  sw.StbSecBarArrangementWall_RC.Items.Add( bar ) ;
                }
              }
              else if ( arrtype.Contains( "千鳥配筋" ) ) {
                for ( int j = 0 ; j < SetFamily.Wall.D.Length ; ++j ) {
                  StbSecBarWall_RC_Zigzag bar = new StbSecBarWall_RC_Zigzag()
                  {
                    pos = j == 0 ? StbSecBarWall_RC_ZigzagPos.VERTICAL : StbSecBarWall_RC_ZigzagPos.HORIZONTAL, strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ), D = Data.GetParameter_D( instances[ i ].WallType, SetFamily.Wall.D[ j ], SetFamily.Wall.D2[ j ] ), pitch = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.pitch[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    sw.StbSecBarArrangementWall_RC.Items.Clear() ;
                    break ;
                  }

                  sw.StbSecBarArrangementWall_RC.Items.Add( bar ) ;
                }
              }
              else if ( arrtype.Contains( "ダブル配筋（内外異なる）" ) ) {
                SetFamily.Wall = new FamilyStructure.Wall() ;
                
                var bars = new List<StbSecBarWall_RC_InsideAndOutside>() ;
                for ( int j = 0 ; j < SetFamily.Wall.D_inout.Length ; ++j ) {
                  int j1 = j / 3 ;
                  int j2 = ( j % 3 ) + 1 ; //ALL=0があるため+1
                  
                  StbSecBarWall_RC_InsideAndOutside bar = new StbSecBarWall_RC_InsideAndOutside()
                  {
                    pos = (StbSecBarWall_RC_InsideAndOutsidePos)j1,
                    pos2 = (StbSecBarWall_RC_InsideAndOutsidePos2)j2,
                    strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ),
                    D = Data.GetParameter_D( instances[ i ].WallType, SetFamily.Wall.D_inout[ j ], SetFamily.Wall.D2_inout[ j ] ),
                    pitch = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.pitch_inout[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    bars.Clear() ;
                    break ;
                  }

                  bars.Add( bar ) ;
                }

                if ( bars.Count == 12 ) {
                  bool all_check = true ;
                  for ( int j = 0 ; j < 4 ; ++j ) {
                    var bar = bars.Where( a => a.pos == (StbSecBarWall_RC_InsideAndOutsidePos)j ).ToList() ;
                    for ( int k = 1 ; k < bar.Count ; ++k ) {
                      all_check &= bar[ 0 ].strength == bar[ k ].strength ;
                      all_check &= bar[ 0 ].D == bar[ k ].D ;
                      all_check &= Math.Abs( bar[ 0 ].pitch - bar[ k ].pitch ) < 0.1 ;

                      if ( ! all_check ) break ;
                    }

                    if ( ! all_check ) break ;
                  }

                  if ( all_check ) {
                    var bar0 = bars.Where( a => a.pos == StbSecBarWall_RC_InsideAndOutsidePos.VERTICAL_OUTSIDE ).First() ;
                    var bar1 = bars.Where( a => a.pos == StbSecBarWall_RC_InsideAndOutsidePos.VERTICAL_INSIDE ).First() ;
                    var bar2 = bars.Where( a => a.pos == StbSecBarWall_RC_InsideAndOutsidePos.HORIZONTAL_OUTSIDE ).First() ;
                    var bar3 = bars.Where( a => a.pos == StbSecBarWall_RC_InsideAndOutsidePos.HORIZONTAL_INSIDE ).First() ;

                    bar0.pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.ALL ;
                    bar1.pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.ALL ;
                    bar2.pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.ALL ;
                    bar3.pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.ALL ;

                    sw.StbSecBarArrangementWall_RC.Items.Add( bar0 ) ;
                    sw.StbSecBarArrangementWall_RC.Items.Add( bar1 ) ;
                    sw.StbSecBarArrangementWall_RC.Items.Add( bar2 ) ;
                    sw.StbSecBarArrangementWall_RC.Items.Add( bar3 ) ;
                  }
                  else {
                    sw.StbSecBarArrangementWall_RC.Items.AddRange( bars ) ;
                  }
                }
                else if ( bars.Count > 0 ) {
                  sw.StbSecBarArrangementWall_RC.Items.AddRange( bars ) ;
                }
              }
              else if ( arrtype.Contains( "ダブル配筋" ) ) {
                for ( int j = 0 ; j < SetFamily.Wall.D.Length ; ++j ) {
                  StbSecBarWall_RC_DoubleNet bar = new StbSecBarWall_RC_DoubleNet()
                  {
                    pos = j == 0 ? StbSecBarWall_RC_DoubleNetPos.VERTICAL : StbSecBarWall_RC_DoubleNetPos.HORIZONTAL, strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ), D = Data.GetParameter_D( instances[ i ].WallType, SetFamily.Wall.D[ j ], SetFamily.Wall.D2[ j ] ), pitch = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.pitch[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    sw.StbSecBarArrangementWall_RC.Items.Clear() ;
                    break ;
                  }

                  sw.StbSecBarArrangementWall_RC.Items.Add( bar ) ;
                }
              }



              //端部補強筋
              for ( int j = 0 ; j < SetFamily.Wall.D_Edge.Length ; ++j ) {
                StbSecBarWall_RC_Edge bar = new StbSecBarWall_RC_Edge()
                {
                  pos = (StbSecBarWall_RC_EdgePos)j, strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ), D = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.D_Edge[ j ] ), N = Data.GetParameter_int( instances[ i ].WallType, SetFamily.Wall.count_Edge[ j ] ),
                } ;

                if ( bar.D == "" || bar.N <= 0 ) {
                  //径、ピッチがないときは配筋出さない
                  //端部補強筋は必要な箇所だけ出せる
                  continue ;
                }

                sw.StbSecBarArrangementWall_RC.Items.Add( bar ) ;
              }


              //開口補強筋
              if ( opens2.Count > 0 ) {
                for ( int j = 0 ; j < SetFamily.Wall.D_op.Length ; ++j ) {
                  StbSecBarWall_RC_Open bar = new StbSecBarWall_RC_Open()
                  {
                    pos = (StbSecBarWall_RC_OpenPos)j,
                    strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ),
                    D = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.D_op[ j ] ),
                    N = Data.GetParameter_int( instances[ i ].WallType, SetFamily.Wall.count_op[ j ] ),
                    length = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.length_op[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.N <= 0 ) {
                    //径、ピッチがないときは配筋出さない
                    //開口補強筋は必要な箇所だけ出せる
                    continue ;
                  }

                  sw.StbSecBarArrangementWall_RC.Items.Add( bar ) ;
                }
              }

              stb.StbModel.StbSections.StbSecWall_RC.Add( sw ) ;
              sect.Add( instances[ i ].WallType.Id, id_sect ) ;
              break ;

            case 1 : //パラペット
              //判別できないので全て壁にする
              break ;
          }

          #endregion
        }
        else {
          w.id_section = sect[ instances[ i ].WallType.Id ] ;
        }


        w.name = instances[ i ].WallType.Name ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], SetFamily.Wall.kind_layout ), out StbWallKind_layout kind_Layout ) ;
        w.kind_layout = kind_Layout ;

        w.thickness_add_right = Data.GetParameter_double( instances[ i ], SetFamily.Wall.thickness_ex_right ) ;
        w.thickness_add_left = Data.GetParameter_double( instances[ i ], SetFamily.Wall.thickness_ex_left ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], SetFamily.Wall.kind_wall ), out StbWallKind_wall kind_Wall ) ;
        w.kind_wall = kind_Wall ;

        w.slit_upper = Data.GetParameter_double( instances[ i ], SetFamily.Wall.slit_upper ) ;
        w.slit_bottom = Data.GetParameter_double( instances[ i ], SetFamily.Wall.slit_bottom ) ;
        w.slit_right = Data.GetParameter_double( instances[ i ], SetFamily.Wall.slit_right ) ;
        w.slit_left = Data.GetParameter_double( instances[ i ], SetFamily.Wall.slit_left ) ;

        if ( ! Enum.TryParse( Data.GetParameter_string( instances[ i ], SetFamily.Wall.type_outside ), out StbWallType_outside type_Outside ) ) {
          type_Outside = StbWallType_outside.NONE ;
        }

        w.type_outside = type_Outside ;

        w.isPress = Data.GetParameter_bool( instances[ i ], SetFamily.Wall.isPress ) ;


        if ( opens2.Count > 0 ) {
          #region 開口

          //最初の点を通り壁に垂直な平面
          XYZ normal = ( points[ 1 ] - points[ 0 ] ).Normalize() ;
          double kd = -( normal.X * points[ 0 ].X + normal.Y * points[ 0 ].Y + normal.Z * points[ 0 ].Z ) ;

          for ( int op = 0 ; op < opens2.Count ; ++op ) {
            XYZ p0 = Commons.ft2mm( opens2[ op ].BoundaryRect[ 0 ] ) ;
            XYZ p1 = Commons.ft2mm( opens2[ op ].BoundaryRect[ 1 ] ) ;

            XYZ pa = new XYZ( p0.X, p0.Y, points[ 0 ].Z ) ;
            XYZ pb = new XYZ( p1.X, p1.Y, points[ 0 ].Z ) ;

            XYZ p2 = ( points[ 0 ].DistanceTo( pa ) < points[ 0 ].DistanceTo( pb ) ? pa : pb ) ;
            XYZ p3 = new XYZ( p1.X, p1.Y, p0.Z ) ;

            double position_X = Math.Abs( normal.X * p2.X + normal.Y * p2.Y + normal.Z * p2.Z + kd ) ;


            id++ ;
            StbOpen o = new StbOpen()
            {
              id = id,
              guid = GetGuid( opens2[ op ], "" ),
              name = "",
              position_X = position_X,
              position_Y = p0.Z - points[ 0 ].Z,
              length_X = p0.DistanceTo( p3 ),
              length_Y = p1.Z - p0.Z,
              rotate = 0,
            } ;

            StbSecOpen_RC so = new StbSecOpen_RC() { guid = GetGuid( null, "" ), name = "", StbSecBarArrangementOpen_RC = new StbSecBarArrangementOpen_RC() { Items = new List<object>(), }, } ;

            var wbar = new List<StbSecBarOpen_RC_Wall>() ;
            for ( int j = 0 ; j < SetFamily.Wall.D_op.Length ; ++j ) {
              StbSecBarOpen_RC_Wall bar = new StbSecBarOpen_RC_Wall()
              {
                pos = (StbSecBarOpen_RC_WallPos)j,
                strength = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.strength ),
                D = Data.GetParameter_string( instances[ i ].WallType, SetFamily.Wall.D_op[ j ] ),
                N = Data.GetParameter_int( instances[ i ].WallType, SetFamily.Wall.count_op[ j ] ),
                length = Data.GetParameter_double( instances[ i ].WallType, SetFamily.Wall.length_op[ j ] ),
              } ;

              if ( bar.D == "" || bar.N <= 0 ) {
                //径、ピッチがないときは配筋出さない
                //開口補強筋は必要な箇所だけ出せる
                continue ;
              }

              so.StbSecBarArrangementOpen_RC.Items.Add( bar ) ;
              wbar.Add( bar ) ;
            }

            o.id_section = -1 ;
            so.id = -1 ;

            //同一開口断面の有無を調べる
            for ( int k = 0 ; k < stb.StbModel.StbSections.StbSecOpen_RC.Count ; ++k ) {
              StbSecOpen_RC so2 = stb.StbModel.StbSections.StbSecOpen_RC[ k ] ;
              if ( so2.StbSecBarArrangementOpen_RC.Items == null ) continue ;

              var wbar2 = so2.StbSecBarArrangementOpen_RC.Items.OfType<StbSecBarOpen_RC_Wall>().OrderBy( a => a.pos ).ToList() ;

              if ( so.name == so2.name ) {
                if ( wbar2.Count == wbar.Count ) {
                  bool isSame2 = true ;

                  for ( int j = 0 ; j < wbar.Count ; ++j ) {
                    isSame2 &= wbar[ j ].pos == wbar2[ j ].pos ;
                    isSame2 &= wbar[ j ].D == wbar2[ j ].D ;
                    isSame2 &= wbar[ j ].N == wbar2[ j ].N ;
                    isSame2 &= Math.Abs( wbar[ j ].length - wbar2[ j ].length ) < 0.1 ;

                    if ( ! isSame2 ) {
                      break ;
                    }
                  }

                  if ( isSame2 ) {
                    o.id_section = so2.id ;
                    break ;
                  }
                }
              }
            }

            if ( o.id_section < 0 ) {
              id_sect++ ;
              so.id = id_sect ;
              o.id_section = id_sect ;

              stb.StbModel.StbSections.StbSecOpen_RC.Add( so ) ;
            }

            w.StbOpenIdList.Add( new StbOpenId() { id = o.id, } ) ;
            stb.StbModel.StbMembers.StbOpens.Add( o ) ;
          }

          #endregion
        }


        stb.StbModel.StbMembers.StbWalls.Add( w ) ;
        Data.AddLog( Data.LogCode.wall, instances[ i ], w.id, w.id_section ) ;
      }
    }

    #endregion


    #region 床

    /// <summary>
    /// 床の出力
    /// </summary>
    private static void Export_Slab()
    {
      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter1 = new ElementCategoryFilter( BuiltInCategory.OST_Floors ) ;
      ElementCategoryFilter filter2 = new ElementCategoryFilter( BuiltInCategory.OST_StructuralFoundation ) ;
      LogicalOrFilter filter = new LogicalOrFilter( filter1, filter2 ) ;
      List<Floor> instances = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<Floor>().ToList() ;

      collector = new FilteredElementCollector( Commons.doc ) ;
      filter1 = new ElementCategoryFilter( BuiltInCategory.OST_FloorOpening ) ;
      List<Opening> opens = collector.WherePasses( filter1 ).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList() ;


      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;

      for ( int i = 0 ; i < instances.Count ; ++i ) {
        if ( ! Data.Check_Analytical_Model( instances[ i ] ) ) {
          Data.AddWarning( -2, instances[ i ] ) ;
          continue ;
        }

        List<Opening> opens2 = opens.Where( x => x.Host.Id == instances[ i ].Id ).ToList() ;
        ( List<XYZ> points, List<XYZ> points2 ) = Data.GetSlabCoord2( instances[ i ] ) ;
        if ( points == null ) {
          Data.AddWarning( -4, instances[ i ] ) ;
          continue ;
        }

        if ( points.Count <= 2 ) {
          Data.AddWarning( -1, instances[ i ] ) ;
          continue ;
        }

        id++ ;

        StbSlab s = new StbSlab()
        {
          id = id,
          guid = GetGuid( instances[ i ], "" ),
          name = instances[ i ].FloorType.Name,
          kind_structure = StbSlabKind_structure.RC,
          thickness_add_top = Data.GetParameter_double( instances[ i ], SetFamily.Slab.thickness_ex_upper ),
          thickness_add_bottom = Data.GetParameter_double( instances[ i ], SetFamily.Slab.thickness_ex_bottom ),
          angle_load = Data.GetParameter_double( instances[ i ], SetFamily.Slab.angle_load ),
          angle_main_bar_direction = 0,
          isFoundation = Data.GetParameter_bool( instances[ i ], SetFamily.Slab.isFoundation ),
          StbNodeIdOrder = string.Join( " ", points.Select( a => GetNodeId( a ).ToString() ) ),
          StbSlabOffsetList = new List<StbSlabOffset>(),
          StbOpenIdList = new List<StbOpenId>(),
        } ;

        for ( int j = 0 ; j < points.Count ; ++j ) {
          XYZ offset = points2[ j ] - points[ j ] ;
          if ( offset.GetLength() > 0 ) {
            s.StbSlabOffsetList.Add( new StbSlabOffset()
            {
              id_node = GetNodeId( points[ j ] ), offset_X = offset.X, offset_Y = offset.Y, offset_Z = offset.Z,
            } ) ;
          }
        }

        Enum.TryParse( Data.GetParameter_string( instances[ i ], SetFamily.Slab.kind_slab ), out StbSlabKind_slab kind_Slab ) ;
        s.kind_slab = kind_Slab ;

        string dir_load = Data.GetParameter_string( instances[ i ], SetFamily.Slab.dir_load ) ;
        if ( Enum.TryParse( dir_load, out StbSlabDirection_load direction_Load ) ) {
          s.direction_load = direction_Load ;
        }
        else {
          if ( dir_load.ToUpper() == "1WAY" ) {
            s.direction_load = StbSlabDirection_load.Item1WAY ;
          }
          else if ( dir_load.ToUpper() == "2WAY" ) {
            s.direction_load = StbSlabDirection_load.Item2WAY ;
          }
          else {
            //set時に kind_slabによって内容を変えている
            s.direction_load = StbSlabDirection_load.NONE ;
          }
        }

        Enum.TryParse( Data.GetParameter_string( instances[ i ], SetFamily.Slab.type_haunch ), out StbSlabType_haunch type_Haunch ) ;
        s.type_haunch = type_Haunch ;


        if ( instances[ i ].Category.Id.Value() == (long)BuiltInCategory.OST_StructuralFoundation ) {
          s.isFoundation = true ;
        }


        if ( ! sect.ContainsKey( instances[ i ].FloorType.Id ) ) {
          #region 断面

          id_sect++ ;
          s.id_section = id_sect ;

          //既定の厚さ
          double depth = Data.GetParameter_double( instances[ i ].FloorType, BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM ) ;


          int secslab = 0 ;
          string product_type = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.product_type ) ;
          Enum.TryParse( product_type, out StbSecSlabDeckProduct_type product_Type2 ) ;
          if ( product_type == "FLAT" || product_type == "COMPOSITE" ) {
            //デッキ
            secslab = 1 ;
          }
          else {
            secslab = 0 ;
          }

          string arrtype = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.ArrengementType ) ;

          switch ( secslab ) {
            case 0 : //RC

              StbSecSlab_RC RC = new StbSecSlab_RC()
              {
                id = id_sect,
                guid = GetGuid( instances[ i ].FloorType, "" ),
                name = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.name ),
                //isFoundation = Data.GetParameter_bool(instances[i].FloorType, SetFamily.Slab.isFoundation), //インスタンスパラメータにしかない。STBが配置断面両方にある。
                isFoundation = false,
                isEarthen = Data.GetParameter_bool( instances[ i ].FloorType, SetFamily.Slab.isEarthen ),
                isCanti = ( Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.isCanti ) == "片持ち" ),
                strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( instances[ i ].FloorType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM ) ),
                StbSecFigureSlab_RC = new StbSecFigureSlab_RC() { Items = new List<object>(), },
                StbSecBarArrangementSlab_RC = new StbSecBarArrangementSlab_RC() { depth_cover_top = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_cover_top ), depth_cover_bottom = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_cover_bottom ), Items = new List<object>(), },
              } ;


              #region 形状

              double depth_center = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_center ) ;
              double depth_base = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_base ) ;
              double depth_tip = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_tip ) ;
              double length_haunch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.length_haunch ) ;

              if ( length_haunch > 0.001 ) {
                //ハンチ
                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Haunch() { pos = StbSecSlab_RC_HaunchPos.BASE, depth = depth_base, } ) ;

                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Haunch() { pos = StbSecSlab_RC_HaunchPos.CENTER, depth = depth_center, } ) ;

                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Haunch() { pos = StbSecSlab_RC_HaunchPos.HAUNCH, depth = length_haunch, } ) ;
              }
              else if ( depth_tip > 0.001 ) {
                //テーパー
                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Taper() { pos = StbSecSlab_RC_TaperPos.BASE, depth = depth_base, } ) ;

                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Taper() { pos = StbSecSlab_RC_TaperPos.TIP, depth = depth_tip, } ) ;
              }
              else {
                //ストレート
                RC.StbSecFigureSlab_RC.Items.Add( new StbSecSlab_RC_Straight() { depth = depth, } ) ;
              }

              #endregion


              #region 配筋

              if ( arrtype == "標準スラブ配筋" ) {
                for ( int j = 0 ; j < SetFamily.Slab.D1.Length ; ++j ) {
                  var bar = new StbSecBarSlab_RC_Standard()
                  {
                    pos = (StbSecBarSlab_RC_StandardPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ j ], SetFamily.Slab.D2[ j ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    RC.StbSecBarArrangementSlab_RC.Items = null ;
                    break ;
                  }
                  else {
                    RC.StbSecBarArrangementSlab_RC.Items.Add( bar ) ;
                  }
                }
              }
              else if ( arrtype == "2方向スラブ配筋" ) {
                for ( int j = 0 ; j < 4 ; ++j ) {
                  int[] ind = new int[ 0 ] ;
                  switch ( j ) {
                    case 0 :
                      ind = new int[] { 0, 1, 2 } ;
                      break ;
                    case 1 :
                      ind = new int[] { 3, 4, 5 } ;
                      break ;
                    case 2 :
                      ind = new int[] { 6, 7, 8 } ;
                      break ;
                    case 3 :
                      ind = new int[] { 9, 10, 11 } ;
                      break ;
                    default :
                      continue ;
                  }

                  bool check = false ;
                  for ( int k = 0 ; k < ind.Length ; ++k ) {
                    var bar = new StbSecBarSlab_RC_2Way()
                    {
                      pos = (StbSecBarSlab_RC_2WayPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ ind[ k ] ], SetFamily.Slab.D2[ ind[ k ] ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ ind[ k ] ] ),
                    } ;

                    if ( bar.D == "" || bar.pitch < 0.001 ) {
                    }
                    else {
                      RC.StbSecBarArrangementSlab_RC.Items.Add( bar ) ;
                      check = true ;
                      break ;
                    }
                  }

                  if ( ! check ) {
                    //径、ピッチがないときは配筋出さない
                    RC.StbSecBarArrangementSlab_RC.Items = null ;
                    break ;
                  }
                }
              }
              else if ( arrtype == "1方向スラブ1配筋" ) {
                for ( int j = 0 ; j < 4 ; ++j ) {
                  int[] ind = new int[ 0 ] ;
                  switch ( j ) {
                    case 0 :
                      ind = new int[] { 0, 1, 2 } ;
                      break ;
                    case 1 :
                      ind = new int[] { 3, 4, 5 } ;
                      break ;
                    case 2 :
                      ind = new int[] { 6, 7, 8 } ;
                      break ;
                    case 3 :
                      ind = new int[] { 9, 10, 11 } ;
                      break ;
                    default :
                      continue ;
                  }

                  bool check = false ;
                  for ( int k = 0 ; k < ind.Length ; ++k ) {
                    var bar = new StbSecBarSlab_RC_1Way1()
                    {
                      pos = (StbSecBarSlab_RC_1Way1Pos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ ind[ k ] ], SetFamily.Slab.D2[ ind[ k ] ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ ind[ k ] ] ),
                    } ;

                    if ( bar.D == "" || bar.pitch < 0.001 ) {
                    }
                    else {
                      RC.StbSecBarArrangementSlab_RC.Items.Add( bar ) ;
                      check = true ;
                      break ;
                    }
                  }

                  if ( ! check ) {
                    //径、ピッチがないときは配筋出さない
                    RC.StbSecBarArrangementSlab_RC.Items = null ;
                    break ;
                  }
                }
              }
              else if ( arrtype == "1方向スラブ2配筋" ) {
                for ( int j = 0 ; j < 6 ; ++j ) {
                  int[] ind = new int[ 0 ] ;
                  switch ( j ) {
                    case 1 :
                      ind = new int[] { 4 } ;
                      break ;
                    case 3 :
                      ind = new int[] { 5 } ;
                      break ;
                    case 4 :
                      ind = new int[] { 7, 8 } ;
                      break ;
                    case 5 :
                      ind = new int[] { 10, 11 } ;
                      break ;
                    default :
                      ind = new int[] { j } ;
                      break ;
                  }

                  bool check = false ;
                  for ( int k = 0 ; k < ind.Length ; ++k ) {
                    var bar = new StbSecBarSlab_RC_1Way2()
                    {
                      pos = (StbSecBarSlab_RC_1Way2Pos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ ind[ k ] ], SetFamily.Slab.D2[ ind[ k ] ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ ind[ k ] ] ),
                    } ;

                    if ( bar.D == "" || bar.pitch < 0.001 ) {
                    }
                    else {
                      RC.StbSecBarArrangementSlab_RC.Items.Add( bar ) ;
                      check = true ;
                      break ;
                    }
                  }

                  if ( ! check ) {
                    //径、ピッチがないときは配筋出さない
                    RC.StbSecBarArrangementSlab_RC.Items = null ;
                    break ;
                  }
                }
              }

              #endregion


              stb.StbModel.StbSections.StbSecSlab_RC.Add( RC ) ;
              sect.Add( instances[ i ].FloorType.Id, id_sect ) ;

              break ;

            case 1 : //デッキ

              StbSecSlabDeck deck = new StbSecSlabDeck()
              {
                id = id_sect,
                guid = GetGuid( instances[ i ].FloorType, "" ),
                name = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.name ),
                product_type = product_Type2,
                strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( instances[ i ].FloorType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM ) ),
                StbSecFigureSlabDeck = new StbSecFigureSlabDeck() { StbSecSlabDeckStraight = new StbSecSlabDeckStraight() { depth = depth, }, },
                StbSecBarArrangementSlabDeck = new StbSecBarArrangementSlabDeck() { depth_cover_top = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_cover_top ), depth_cover_bottom = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_cover_bottom ), Items = new List<object>(), },
                StbSecProductSlabDeck = new StbSecProductSlabDeck() { product_company = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.product_company ), product_code = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.product_code ), depth_deck = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.depth_center ), },
              } ;


              #region 配筋

              if ( arrtype == "標準スラブ配筋" ) {
                for ( int j = 0 ; j < SetFamily.Slab.D1.Length ; ++j ) {
                  var bar = new StbSecBarSlabDeckStandard()
                  {
                    pos = (StbSecBarSlab_RC_StandardPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ j ], SetFamily.Slab.D2[ j ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ j ] ),
                  } ;

                  if ( bar.D == "" || bar.pitch < 0.001 ) {
                    //径、ピッチがないときは配筋出さない
                    deck.StbSecBarArrangementSlabDeck.Items = null ;
                    break ;
                  }
                  else {
                    deck.StbSecBarArrangementSlabDeck.Items.Add( bar ) ;
                  }
                }
              }
              else if ( arrtype == "2方向スラブ配筋" ) {
                for ( int j = 0 ; j < 4 ; ++j ) {
                  int[] ind = new int[ 0 ] ;
                  switch ( j ) {
                    case 0 :
                      ind = new int[] { 0, 1, 2 } ;
                      break ;
                    case 1 :
                      ind = new int[] { 3, 4, 5 } ;
                      break ;
                    case 2 :
                      ind = new int[] { 6, 7, 8 } ;
                      break ;
                    case 3 :
                      ind = new int[] { 9, 10, 11 } ;
                      break ;
                    default :
                      continue ;
                  }

                  bool check = false ;
                  for ( int k = 0 ; k < ind.Length ; ++k ) {
                    var bar = new StbSecBarSlabDeck2Way()
                    {
                      pos = (StbSecBarSlab_RC_2WayPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ ind[ k ] ], SetFamily.Slab.D2[ ind[ k ] ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ ind[ k ] ] ),
                    } ;

                    if ( bar.D == "" || bar.pitch < 0.001 ) {
                      //径、ピッチがないときは配筋出さない
                    }
                    else {
                      deck.StbSecBarArrangementSlabDeck.Items.Add( bar ) ;
                      check = true ;
                      break ;
                    }
                  }

                  if ( ! check ) {
                    //径、ピッチがないときは配筋出さない
                    deck.StbSecBarArrangementSlabDeck.Items = null ;
                    break ;
                  }
                }
              }
              else if ( arrtype == "1方向スラブ配筋" ) {
                for ( int j = 0 ; j < 5 ; ++j ) {
                  int[] ind = new int[ 0 ] ;
                  switch ( j ) {
                    case 0 :
                      ind = new int[] { 0, 1, 2 } ;
                      break ;
                    case 1 :
                      ind = new int[] { 3, 4, 5 } ;
                      break ;
                    case 2 :
                      ind = new int[] { 6, 7, 8 } ;
                      break ;
                    case 3 :
                      ind = new int[] { 9, 10, 11 } ;
                      break ;

                    case 4 :
                      ind = null ;
                      break ;

                    default :
                      continue ;
                  }

                  if ( j == 4 ) {
                    //耐火補強筋
                    var bar = new StbSecBarSlabDeck1Way()
                    {
                      pos = (StbSecBarSlabDeck1WayPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.addD ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.addpitch ),
                    } ;

                    if ( bar.D == "" || bar.pitch < 0.001 ) {
                      //省略可
                    }
                    else {
                      deck.StbSecBarArrangementSlabDeck.Items.Add( bar ) ;
                    }
                  }
                  else {
                    bool check = false ;
                    for ( int k = 0 ; k < ind.Length ; ++k ) {
                      var bar = new StbSecBarSlabDeck1Way()
                      {
                        pos = (StbSecBarSlabDeck1WayPos)j, strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ), D = Data.GetParameter_D( instances[ i ].FloorType, SetFamily.Slab.D1[ ind[ k ] ], SetFamily.Slab.D2[ ind[ k ] ] ), pitch = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.pitch[ ind[ k ] ] ),
                      } ;

                      if ( bar.D == "" || bar.pitch < 0.001 ) {
                        if ( bar.pos == StbSecBarSlabDeck1WayPos.MAIN_TOP || bar.pos == StbSecBarSlabDeck1WayPos.TRANSVERSE_TOP ) {
                          //①③は省略不可
                        }
                        else {
                          //②④は省略可
                          check = true ;
                        }
                      }
                      else {
                        deck.StbSecBarArrangementSlabDeck.Items.Add( bar ) ;
                        check = true ;
                        break ;
                      }
                    }

                    if ( ! check ) {
                      //径、ピッチがないときは配筋出さない
                      deck.StbSecBarArrangementSlabDeck.Items = null ;
                      break ;
                    }
                  }
                }
              }

              #endregion


              stb.StbModel.StbSections.StbSecSlabDeck.Add( deck ) ;
              sect.Add( instances[ i ].FloorType.Id, id_sect ) ;

              break ;

            case 2 : //既製
              break ;
          }

          #endregion
        }
        else {
          s.id_section = sect[ instances[ i ].FloorType.Id ] ;
        }

        if ( opens2.Count > 0 ) {
          #region 開口

          //[0]→[1]方向をU軸とするローカル座標系への変換行列を求める
          XYZ Vu = ( points[ 1 ] - points[ 0 ] ).Normalize() ;
          XYZ Vv = ( points[ 2 ] - points[ 0 ] ).Normalize() ;
          XYZ Vw = Vu.CrossProduct( Vv ).Normalize() ;
          Vv = Vw.CrossProduct( Vu ).Normalize() ;

          double u0 = Vu.X * points[ 0 ].X + Vu.Y * points[ 0 ].Y + Vu.Z * points[ 0 ].Z ;
          double v0 = Vv.X * points[ 0 ].X + Vv.Y * points[ 0 ].Y + Vv.Z * points[ 0 ].Z ;
          double w0 = Vw.X * points[ 0 ].X + Vw.Y * points[ 0 ].Y + Vw.Z * points[ 0 ].Z ;
          XYZ p0 = new XYZ( u0, v0, w0 ) ;

          for ( int op = 0 ; op < opens2.Count ; ++op ) {
            //開口座標取得
            List<XYZ> open_points1 = new List<XYZ>() ;
            var ca = opens2[ op ].BoundaryCurves ;
            if ( ca == null ) continue ;
            if ( ca.Size != 4 ) continue ;

            foreach ( Curve c in ca ) {
              open_points1.Add( Commons.ft2mm( c.GetEndPoint( 0 ) ) ) ;
            }

            //重複除外
            open_points1 = open_points1.Distinct( new Data.XyzEqualityComparer() ).ToList() ;
            if ( open_points1.Count != 4 ) {
              //四角形のみ
              continue ;
            }

            //床平面のローカル座標に変換
            List<XYZ> open_points2 = new List<XYZ>() ;
            for ( int p = 0 ; p < open_points1.Count ; ++p ) {
              double u = Vu.X * open_points1[ p ].X + Vu.Y * open_points1[ p ].Y + Vu.Z * open_points1[ p ].Z ;
              double v = Vv.X * open_points1[ p ].X + Vv.Y * open_points1[ p ].Y + Vv.Z * open_points1[ p ].Z ;
              double w = Vw.X * open_points1[ p ].X + Vw.Y * open_points1[ p ].Y + Vw.Z * open_points1[ p ].Z ;

              open_points2.Add( new XYZ( u, v, w ) ) ;
            }

            double minY = open_points2.Min( a => a.Y ) ;
            double minX = open_points2.Where( a => Math.Abs( a.Y - minY ) < 0.001 ).Min( a => a.X ) ;
            int index = open_points2.FindIndex( a => Math.Abs( a.X - minX ) < 0.001 && Math.Abs( a.Y - minY ) < 0.001 ) ;
            index = Math.Max( index, 0 ) ;

            bool reverse = Commons.CalcMenseki( open_points2 ) < 0 ;

            //床の[0]点目に近いものから始まるように並び替える
            List<XYZ> open_points3 = new List<XYZ>( open_points2.Count ) ;
            for ( int p = 0 ; p < open_points2.Count ; ++p ) {
              int pp = p + index ;
              if ( pp >= open_points2.Count ) pp = pp - open_points2.Count ;

              if ( p == 0 || ! reverse ) {
                open_points3.Add( open_points2[ pp ] ) ;
              }
              else {
                open_points3.Insert( 1, open_points2[ pp ] ) ;
              }
            }

            XYZ vec = ( open_points3[ 1 ] - open_points3[ 0 ] ).Normalize() ;

            id++ ;
            StbOpen o = new StbOpen()
            {
              id = id,
              guid = GetGuid( opens2[ op ], "" ),
              name = "",
              position_X = ( open_points3[ 0 ].X - p0.X ),
              position_Y = ( open_points3[ 0 ].Y - p0.Y ),
              length_X = open_points3[ 0 ].DistanceTo( open_points3[ 1 ] ),
              length_Y = open_points3[ 0 ].DistanceTo( open_points3[ 3 ] ),
              rotate = XYZ.BasisX.AngleOnPlaneTo( vec, XYZ.BasisZ ) / Math.PI * 180,
            } ;

            StbSecOpen_RC so = new StbSecOpen_RC() { guid = GetGuid( null, "" ), name = "", StbSecBarArrangementOpen_RC = new StbSecBarArrangementOpen_RC() { Items = new List<object>(), }, } ;

            var sbar = new List<StbSecBarOpen_RC_Slab>() ;
            for ( int j = 0 ; j < SetFamily.Slab.D_op.Length ; ++j ) {
              var bar = new StbSecBarOpen_RC_Slab()
              {
                pos = (StbSecBarOpen_RC_SlabPos)j,
                strength = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.strength ),
                D = Data.GetParameter_string( instances[ i ].FloorType, SetFamily.Slab.D_op[ j ] ),
                N = Data.GetParameter_int( instances[ i ].FloorType, SetFamily.Slab.count_op[ j ] ),
                length = Data.GetParameter_double( instances[ i ].FloorType, SetFamily.Slab.length_op[ j ] ),
              } ;

              if ( bar.D == "" || bar.N <= 0 ) {
                continue ;
              }

              so.StbSecBarArrangementOpen_RC.Items.Add( bar ) ;
              sbar.Add( bar ) ;
            }

            o.id_section = -1 ;
            so.id = -1 ;

            //同一開口断面の有無を調べる
            for ( int k = 0 ; k < stb.StbModel.StbSections.StbSecOpen_RC.Count ; ++k ) {
              StbSecOpen_RC so2 = stb.StbModel.StbSections.StbSecOpen_RC[ k ] ;
              if ( so2.StbSecBarArrangementOpen_RC.Items == null ) continue ;

              var sbar2 = so2.StbSecBarArrangementOpen_RC.Items.OfType<StbSecBarOpen_RC_Slab>().OrderBy( a => a.pos ).ToList() ;

              if ( so.name == so2.name ) {
                if ( sbar2.Count == sbar.Count ) {
                  bool isSame2 = true ;
                  for ( int j = 0 ; j < sbar.Count ; ++j ) {
                    isSame2 &= sbar[ j ].pos == sbar2[ j ].pos ;
                    isSame2 &= sbar[ j ].D == sbar2[ j ].D ;
                    isSame2 &= sbar[ j ].N == sbar2[ j ].N ;
                    isSame2 &= Math.Abs( sbar[ j ].length - sbar2[ j ].length ) < 0.1 ;

                    if ( ! isSame2 ) {
                      break ;
                    }
                  }

                  if ( isSame2 ) {
                    o.id_section = so2.id ;
                    break ;
                  }
                }
              }
            }

            if ( o.id_section < 0 ) {
              id_sect++ ;
              so.id = id_sect ;
              o.id_section = id_sect ;

              stb.StbModel.StbSections.StbSecOpen_RC.Add( so ) ;
            }

            s.StbOpenIdList.Add( new StbOpenId() { id = o.id, } ) ;
            stb.StbModel.StbMembers.StbOpens.Add( o ) ;
          }

          #endregion
        }

        stb.StbModel.StbMembers.StbSlabs.Add( s ) ;
        Data.AddLog( Data.LogCode.slab, instances[ i ], s.id, s.id_section ) ;
      }
    }

    #endregion


    #region ブレース

    /// <summary>
    /// ブレース断面の出力
    /// </summary>
    /// <param name="ins"></param>
    /// <returns></returns>
    private static int Export_SecBrace( FamilyInstance ins )
    {
      FamilySymbol symbol = ins.Symbol ;
      ElementId lvid = ins.LevelId ;
      if ( lvid.Value() == -1 ) {
        lvid = ins.get_Parameter( BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM ).AsElementId() ;
      }

      string floor = Levels.Find( x => x.Id == lvid ).Name ;

      int retID = -1 ;

      int n = 1 ;
      string[] paramName = new string[ 2 ] ;
      string[ , ] strength = new string[ 3, 2 ] ;
      string familyname = symbol.Family.Name ;
      if ( familyname == SetFamily.SBraH.FamilyName ) {
        n = 3 ;
        paramName[ 0 ] = SetFamily.SBraH.name ;
        paramName[ 1 ] = SetFamily.SBraH.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_main[ 0 ] ) ;
        strength[ 1, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_main[ 1 ] ) ;
        strength[ 2, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_main[ 2 ] ) ;
        strength[ 0, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_web[ 0 ] ) ;
        strength[ 1, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_web[ 1 ] ) ;
        strength[ 2, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraH.strength_web[ 2 ] ) ;
      }
      else if ( familyname == SetFamily.SBraBH.FamilyName ) {
        n = 3 ;
        paramName[ 0 ] = SetFamily.SBraBH.name ;
        paramName[ 1 ] = SetFamily.SBraBH.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_main[ 0 ] ) ;
        strength[ 1, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_main[ 1 ] ) ;
        strength[ 2, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_main[ 2 ] ) ;
        strength[ 0, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_web[ 0 ] ) ;
        strength[ 1, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_web[ 1 ] ) ;
        strength[ 2, 1 ] = Data.GetParameter_string( symbol, SetFamily.SBraBH.strength_web[ 2 ] ) ;
      }
      else if ( familyname == SetFamily.SBraBox.FamilyName ) {
        paramName[ 0 ] = SetFamily.SBraBox.name ;
        paramName[ 1 ] = SetFamily.SBraBox.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraBox.strength ) ;
        strength[ 1, 0 ] = "" ;
        strength[ 2, 0 ] = "" ;
        strength[ 0, 1 ] = "" ;
        strength[ 1, 1 ] = "" ;
        strength[ 2, 1 ] = "" ;
      }
      else if ( familyname == SetFamily.SBraBBox.FamilyName ) {
        paramName[ 0 ] = SetFamily.SBraBBox.name ;
        paramName[ 1 ] = SetFamily.SBraBBox.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraBBox.strength ) ;
        strength[ 1, 0 ] = "" ;
        strength[ 2, 0 ] = "" ;
        strength[ 0, 1 ] = "" ;
        strength[ 1, 1 ] = "" ;
        strength[ 2, 1 ] = "" ;
      }
      else if ( familyname == SetFamily.SBraPipe.FamilyName ) {
        paramName[ 0 ] = SetFamily.SBraPipe.name ;
        paramName[ 1 ] = SetFamily.SBraPipe.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraPipe.strength ) ;
        strength[ 1, 0 ] = "" ;
        strength[ 2, 0 ] = "" ;
        strength[ 0, 1 ] = "" ;
        strength[ 1, 1 ] = "" ;
        strength[ 2, 1 ] = "" ;
      }
      else if ( familyname == SetFamily.SBraC.FamilyName ) {
        n = 3 ;
        paramName[ 0 ] = SetFamily.SBraC.name ;
        paramName[ 1 ] = SetFamily.SBraC.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraC.strength ) ;
        strength[ 1, 0 ] = strength[ 0, 0 ] ;
        strength[ 2, 0 ] = strength[ 0, 0 ] ;
        strength[ 0, 1 ] = strength[ 0, 0 ] ;
        strength[ 1, 1 ] = strength[ 0, 0 ] ;
        strength[ 2, 1 ] = strength[ 0, 0 ] ;
      }
      else if ( familyname == SetFamily.SBraL.FamilyName ) {
        n = 3 ;
        paramName[ 0 ] = SetFamily.SBraL.name ;
        paramName[ 1 ] = SetFamily.SBraL.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraL.strength ) ;
        strength[ 1, 0 ] = strength[ 0, 0 ] ;
        strength[ 2, 0 ] = strength[ 0, 0 ] ;
        strength[ 0, 1 ] = strength[ 0, 0 ] ;
        strength[ 1, 1 ] = strength[ 0, 0 ] ;
        strength[ 2, 1 ] = strength[ 0, 0 ] ;
      }
      else if ( familyname == SetFamily.SBraLipC.FamilyName ) {
        n = 3 ;
        paramName[ 0 ] = SetFamily.SBraLipC.name ;
        paramName[ 1 ] = SetFamily.SBraLipC.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraLipC.strength ) ;
        strength[ 1, 0 ] = strength[ 0, 0 ] ;
        strength[ 2, 0 ] = strength[ 0, 0 ] ;
        strength[ 0, 1 ] = strength[ 0, 0 ] ;
        strength[ 1, 1 ] = strength[ 0, 0 ] ;
        strength[ 2, 1 ] = strength[ 0, 0 ] ;
      }
      else if ( familyname == SetFamily.SBraFB.FamilyName ) {
        paramName[ 0 ] = SetFamily.SBraFB.name ;
        paramName[ 1 ] = SetFamily.SBraFB.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraFB.strength_main ) ;
        strength[ 1, 0 ] = "" ;
        strength[ 2, 0 ] = "" ;
        strength[ 0, 1 ] = "" ;
        strength[ 1, 1 ] = "" ;
        strength[ 2, 1 ] = "" ;
      }
      else if ( familyname == SetFamily.SBraRollBar.FamilyName ) {
        paramName[ 0 ] = SetFamily.SBraRollBar.name ;
        paramName[ 1 ] = SetFamily.SBraRollBar.kind_brace ;

        strength[ 0, 0 ] = Data.GetParameter_string( symbol, SetFamily.SBraRollBar.strength_main ) ;
        strength[ 1, 0 ] = "" ;
        strength[ 2, 0 ] = "" ;
        strength[ 0, 1 ] = "" ;
        strength[ 1, 1 ] = "" ;
        strength[ 2, 1 ] = "" ;
      }
      else {
        return retID ;
      }

      string[] shape = new string[ n ] ;
      for ( int i = 0 ; i < n ; ++i ) {
        shape[ i ] = GetSteelName( symbol, 0, i ) ;
      }

      if ( n == 3 ) {
        if ( shape[ 0 ] == "" ) shape[ 0 ] = shape[ 1 ] ;
        if ( shape[ 0 ] == "" ) shape[ 0 ] = shape[ 2 ] ;
        if ( shape[ 1 ] == "" ) shape[ 1 ] = shape[ 0 ] ;
        if ( shape[ 2 ] == "" ) shape[ 2 ] = shape[ 1 ] ;
        if ( shape[ 0 ] == "" ) return retID ;

        if ( shape[ 0 ] == shape[ 1 ] && shape[ 1 ] == shape[ 2 ] ) {
          if ( strength[ 0, 0 ] == strength[ 1, 0 ] && strength[ 1, 0 ] == strength[ 2, 0 ] ) {
            if ( strength[ 0, 1 ] == strength[ 1, 1 ] && strength[ 1, 1 ] == strength[ 2, 1 ] ) {
              //形状、材料が全て一致していればALL
              n = 1 ;
            }
          }
        }
      }
      else {
        if ( shape[ 0 ] == "" ) return retID ;
      }


      id_sect++ ;
      retID = id_sect ;

      Enum.TryParse( Data.GetParameter_string( symbol, paramName[ 1 ] ), out StbSecBrace_SKind_brace kind_Brace ) ;

      StbSecBrace_S b = new StbSecBrace_S()
      {
        id = id_sect,
        guid = GetGuid( symbol, "" ),
        name = Data.GetParameter_string( symbol, paramName[ 0 ] ),
        floor = floor,
        kind_brace = kind_Brace,
        StbSecSteelFigureBrace_S = new StbSecSteelFigureBrace_S() { Items = new List<object>(), },
      } ;

      if ( n == 3 ) {
        for ( int i = 0 ; i < n ; ++i ) {
          var steel = new StbSecSteelBrace_S_ThreeTypes()
          {
            pos = (StbSecSteelBrace_S_ThreeTypesPos)i, shape = shape[ i ], strength_main = strength[ i, 0 ], strength_web = strength[ i, 1 ],
          } ;
          b.StbSecSteelFigureBrace_S.Items.Add( steel ) ;
        }
      }
      else {
        var steel = new StbSecSteelBrace_S_Same() { shape = shape[ 0 ], strength_main = strength[ 0, 0 ], strength_web = strength[ 0, 1 ], } ;
        b.StbSecSteelFigureBrace_S.Items.Add( steel ) ;
      }


      stb.StbModel.StbSections.StbSecBrace_S.Add( b ) ;

      return retID ;
    }

    /// <summary>
    /// ブレースの出力
    /// </summary>
    private static void Export_Brace()
    {
      List<string> AllFamilyName = new List<string>() ;
      for ( int i = 0 ; i < SetFamily.SBraFName.FamilyName.Length ; ++i ) {
        AllFamilyName.AddRange( SetFamily.SBraFName.FamilyName[ i ] ) ;
      }

      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter1 = new ElementCategoryFilter( BuiltInCategory.OST_StructuralFraming ) ;

      ParameterValueProvider provider = new ParameterValueProvider( new ElementId( BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM ) ) ;
      //Aより大きい or Aより小さい = ≠A
      FilterNumericRuleEvaluator evaluator1 = new FilterNumericLess() ;
      FilterNumericRuleEvaluator evaluator2 = new FilterNumericGreater() ;

      FilterRule rule2 = new FilterIntegerRule( provider, evaluator1, (int)StructuralInstanceUsage.Girder ) ;
      FilterRule rule3 = new FilterIntegerRule( provider, evaluator2, (int)StructuralInstanceUsage.Girder ) ;
      FilterRule rule4 = new FilterIntegerRule( provider, evaluator1, (int)StructuralInstanceUsage.Joist ) ;
      FilterRule rule5 = new FilterIntegerRule( provider, evaluator2, (int)StructuralInstanceUsage.Joist ) ;

      ElementParameterFilter filter2 = new ElementParameterFilter( rule2 ) ;
      ElementParameterFilter filter3 = new ElementParameterFilter( rule3 ) ;
      ElementParameterFilter filter4 = new ElementParameterFilter( rule4 ) ;
      ElementParameterFilter filter5 = new ElementParameterFilter( rule5 ) ;


      //大梁でない
      LogicalOrFilter filter6 = new LogicalOrFilter( new List<ElementFilter> { filter2, filter3 } ) ;
      //小梁でない
      LogicalOrFilter filter7 = new LogicalOrFilter( new List<ElementFilter> { filter4, filter5 } ) ;

      //構造フレームで大梁・小梁以外のもの
      LogicalAndFilter filter = new LogicalAndFilter( new List<ElementFilter> { filter1, filter6, filter7 } ) ;
      List<FamilyInstance> instances = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where( x => AllFamilyName.Contains( x.Symbol.Family.Name ) && ! x.Symbol.Family.IsInPlace ).ToList() ;

      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;
      var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager( Commons.doc ) ;

      for ( int i = 0 ; i < instances.Count ; ++i ) {
        XYZ ps1 = new XYZ() ;
        XYZ pe1 = new XYZ() ;
        XYZ ps2 = Data.GetFramingCoordinate( instances[ i ], 0 ) ;
        XYZ pe2 = Data.GetFramingCoordinate( instances[ i ], 1 ) ;

        if ( amanager.HasAssociation( instances[ i ].Id ) && Commons.doc.GetElement( amanager.GetAssociatedElementId( instances[ i ].Id ) ) is AnalyticalMember member ) {
          ps1 = member.GetCurve().GetEndPoint( 0 ) ;
          pe1 = member.GetCurve().GetEndPoint( 1 ) ;
        }
        else {
          ps1 = ps2 ;
          pe1 = pe2 ;
        }

        ps1 = Commons.ft2mm( ps1 ) ;
        pe1 = Commons.ft2mm( pe1 ) ;
        ps2 = Commons.ft2mm( ps2 ) ;
        pe2 = Commons.ft2mm( pe2 ) ;

        StbBrace b = new StbBrace()
        {
          guid = GetGuid( instances[ i ], "" ), id_node_start = GetNodeId( ps1 ), id_node_end = GetNodeId( pe1 ), kind_structure = StbBraceKind_structure.S,
        } ;

        if ( ! sect.ContainsKey( instances[ i ].Symbol.Id ) ) {
          b.id_section = Export_SecBrace( instances[ i ] ) ;
          if ( b.id_section < 0 ) continue ;
          sect.Add( instances[ i ].Symbol.Id, b.id_section ) ;
        }
        else {
          b.id_section = sect[ instances[ i ].Symbol.Id ] ;
        }

        string[] paramName = new string[ 8 ] ;
        string familyname = instances[ i ].Symbol.Family.Name ;
        if ( familyname == SetFamily.SBraH.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraH.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraH.condition_start ;
          paramName[ 2 ] = SetFamily.SBraH.condition_end ;
          paramName[ 3 ] = SetFamily.SBraH.future_brace ;
          paramName[ 4 ] = SetFamily.SBraH.joint_start ;
          paramName[ 5 ] = SetFamily.SBraH.joint_end ;
          paramName[ 6 ] = SetFamily.SBraH.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraH.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraBH.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraBH.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraBH.condition_start ;
          paramName[ 2 ] = SetFamily.SBraBH.condition_end ;
          paramName[ 3 ] = SetFamily.SBraBH.future_brace ;
          paramName[ 4 ] = SetFamily.SBraBH.joint_start ;
          paramName[ 5 ] = SetFamily.SBraBH.joint_end ;
          paramName[ 6 ] = SetFamily.SBraBH.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraBH.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraBox.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraBox.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraBox.condition_start ;
          paramName[ 2 ] = SetFamily.SBraBox.condition_end ;
          paramName[ 3 ] = SetFamily.SBraBox.future_brace ;
          paramName[ 4 ] = SetFamily.SBraBox.joint_start ;
          paramName[ 5 ] = SetFamily.SBraBox.joint_end ;
          paramName[ 6 ] = SetFamily.SBraBox.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraBox.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraBBox.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraBBox.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraBBox.condition_start ;
          paramName[ 2 ] = SetFamily.SBraBBox.condition_end ;
          paramName[ 3 ] = SetFamily.SBraBBox.future_brace ;
          paramName[ 4 ] = SetFamily.SBraBBox.joint_start ;
          paramName[ 5 ] = SetFamily.SBraBBox.joint_end ;
          paramName[ 6 ] = SetFamily.SBraBBox.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraBBox.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraPipe.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraPipe.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraPipe.condition_start ;
          paramName[ 2 ] = SetFamily.SBraPipe.condition_end ;
          paramName[ 3 ] = SetFamily.SBraPipe.future_brace ;
          paramName[ 4 ] = SetFamily.SBraPipe.joint_start ;
          paramName[ 5 ] = SetFamily.SBraPipe.joint_end ;
          paramName[ 6 ] = SetFamily.SBraPipe.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraPipe.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraC.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraC.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraC.condition_start ;
          paramName[ 2 ] = SetFamily.SBraC.condition_end ;
          paramName[ 3 ] = SetFamily.SBraC.future_brace ;
          paramName[ 4 ] = SetFamily.SBraC.joint_start ;
          paramName[ 5 ] = SetFamily.SBraC.joint_end ;
          paramName[ 6 ] = SetFamily.SBraC.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraC.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraL.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraL.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraL.condition_start ;
          paramName[ 2 ] = SetFamily.SBraL.condition_end ;
          paramName[ 3 ] = SetFamily.SBraL.future_brace ;
          paramName[ 4 ] = SetFamily.SBraL.joint_start ;
          paramName[ 5 ] = SetFamily.SBraL.joint_end ;
          paramName[ 6 ] = SetFamily.SBraL.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraL.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraLipC.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraLipC.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraLipC.condition_start ;
          paramName[ 2 ] = SetFamily.SBraLipC.condition_end ;
          paramName[ 3 ] = SetFamily.SBraLipC.future_brace ;
          paramName[ 4 ] = SetFamily.SBraLipC.joint_start ;
          paramName[ 5 ] = SetFamily.SBraLipC.joint_end ;
          paramName[ 6 ] = SetFamily.SBraLipC.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraLipC.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraFB.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraFB.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraFB.condition_start ;
          paramName[ 2 ] = SetFamily.SBraFB.condition_end ;
          paramName[ 3 ] = SetFamily.SBraFB.future_brace ;
          paramName[ 4 ] = SetFamily.SBraFB.joint_start ;
          paramName[ 5 ] = SetFamily.SBraFB.joint_end ;
          paramName[ 6 ] = SetFamily.SBraFB.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraFB.kind_joint_end ;
        }
        else if ( familyname == SetFamily.SBraRollBar.FamilyName ) {
          paramName[ 0 ] = SetFamily.SBraRollBar.NameMembers ;
          paramName[ 1 ] = SetFamily.SBraRollBar.condition_start ;
          paramName[ 2 ] = SetFamily.SBraRollBar.condition_end ;
          paramName[ 3 ] = SetFamily.SBraRollBar.future_brace ;
          paramName[ 4 ] = SetFamily.SBraRollBar.joint_start ;
          paramName[ 5 ] = SetFamily.SBraRollBar.joint_end ;
          paramName[ 6 ] = SetFamily.SBraRollBar.kind_joint_start ;
          paramName[ 7 ] = SetFamily.SBraRollBar.kind_joint_end ;
        }
        else {
          continue ;
        }

        id++ ;
        b.id = id ;
        b.rotate = Data.GetParameter_Angle( instances[ i ], BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE ) ;

        XYZ offset_s = ps2 - ps1 ;
        XYZ offset_e = pe2 - pe1 ;
        b.offset_start_X = offset_s.X ;
        b.offset_start_Y = offset_s.Y ;
        b.offset_start_Z = offset_s.Z ;
        b.offset_end_X = offset_e.X ;
        b.offset_end_Y = offset_e.Y ;
        b.offset_end_Z = offset_e.Z ;


        b.name = instances[ i ].Symbol.Name ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 1 ] ), out StbBraceCondition condition_s ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 2 ] ), out StbBraceCondition condition_e ) ;
        b.condition_start = condition_s ;
        b.condition_end = condition_e ;

        b.feature_brace = StbBraceFeature_brace.TENSION ;
        string feature = Data.GetParameter_string( instances[ i ], paramName[ 3 ] ) ;
        if ( Enum.TryParse( feature, out StbBraceFeature_brace feature_Brace ) ) {
          b.feature_brace = feature_Brace ;
        }
        else if ( feature == "COMPRESSION" ) {
          //1.4と名称が異なる
          b.feature_brace = StbBraceFeature_brace.TENSIONANDCOMPRESSION ;
        }

        b.joint_start = Data.GetParameter_double( instances[ i ], paramName[ 4 ] ) ;
        b.joint_end = Data.GetParameter_double( instances[ i ], paramName[ 5 ] ) ;

        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 6 ] ), out StbBraceKind_joint joint_s ) ;
        Enum.TryParse( Data.GetParameter_string( instances[ i ], paramName[ 7 ] ), out StbBraceKind_joint joint_e ) ;
        b.kind_joint_start = joint_s ;
        b.kind_joint_end = joint_e ;

        stb.StbModel.StbMembers.StbBraces.Add( b ) ;
        Data.AddLog( Data.LogCode.brace, instances[ i ], b.id, b.id_section ) ;
      }
    }

    #endregion


    #region 基礎・杭

    /// <summary>
    /// 杭配筋の同一チェック
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool CompareTo_PileBar( StbSecBarPile_RC_TopCenterBottom a, StbSecBarPile_RC_TopCenterBottom b )
    {
      if ( a.D_main_circumference_1st != b.D_main_circumference_1st ) return false ;
      if ( a.D_main_circumference_2nd != b.D_main_circumference_2nd ) return false ;
      if ( a.D_main_core != b.D_main_core ) return false ;
      if ( a.D_band != b.D_band ) return false ;

      if ( a.strength_main_circumference_1st != b.strength_main_circumference_1st ) return false ;
      if ( a.strength_main_circumference_2nd != b.strength_main_circumference_2nd ) return false ;
      if ( a.strength_main_core != b.strength_main_core ) return false ;
      if ( a.strength_band != b.strength_band ) return false ;

      if ( a.N_main_circumference_1st != b.N_main_circumference_1st ) return false ;
      if ( a.N_main_circumference_2nd != b.N_main_circumference_2nd ) return false ;
      if ( a.N_main_core != b.N_main_core ) return false ;

      if ( Math.Abs( a.pitch_band - b.pitch_band ) > 0.01 ) return false ;

      if ( Math.Abs( a.length_bar - b.length_bar ) > 0.01 ) return false ;
      if ( Math.Abs( a.length_lap_bar - b.length_lap_bar ) > 0.01 ) return false ;

      return true ;
    }

    /// <summary>
    /// 杭テーパーの傾斜角度を計算
    /// </summary>
    /// <param name="diff_D">拡底径(拡頭径) と 軸径 の差</param>
    /// <param name="taper_length">テーパー部の垂直長さ</param>
    /// <returns></returns>
    private static double Calc_taper_angle( double diff_D, double taper_length )
    {
      double angle = 0 ;

      if ( diff_D > 1 ) {
        angle = Math.Atan2( diff_D, taper_length ) * 180 / Math.PI ;
        if ( angle < 0 ) {
          angle += 360 ;
        }

        if ( 360 < angle ) {
          angle -= 360 ;
        }
      }

      return angle ;
    }


    /// <summary>
    /// 基礎断面の出力
    /// </summary>
    /// <param name="ins"></param>
    /// <returns></returns>
    private static int Export_SecFoundation( FamilyInstance ins )
    {
      FamilySymbol symbol = ins.Symbol ;
      int retID = -1 ;

      string[] paramName = new string[ 5 ] ;
      string familyname = symbol.Family.Name ;
      if ( familyname == SetFamily.FRect.FamilyName ) {
        paramName[ 0 ] = SetFamily.FRect.name ;
        paramName[ 1 ] = SetFamily.FRect.strength_concrete ;
        paramName[ 2 ] = SetFamily.FRect.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FRect.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FRect.depth_cover_side ;
      }
      else if ( familyname == SetFamily.FTRect.FamilyName ) {
        paramName[ 0 ] = SetFamily.FTRect.name ;
        paramName[ 1 ] = SetFamily.FTRect.strength_concrete ;
        paramName[ 2 ] = SetFamily.FTRect.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FTRect.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FTRect.depth_cover_side ;
      }
      else if ( familyname == SetFamily.FTri.FamilyName ) {
        paramName[ 0 ] = SetFamily.FTri.name ;
        paramName[ 1 ] = SetFamily.FTri.strength_concrete ;
        paramName[ 2 ] = SetFamily.FTri.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FTri.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FTri.depth_cover_side ;
      }
      else if ( familyname == SetFamily.FETriangle.FamilyName ) {
        paramName[ 0 ] = SetFamily.FETriangle.name ;
        paramName[ 1 ] = SetFamily.FETriangle.strength_concrete ;
        paramName[ 2 ] = SetFamily.FETriangle.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FETriangle.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FETriangle.depth_cover_side ;
      }
      else if ( familyname == SetFamily.FOct.FamilyName ) {
        paramName[ 0 ] = SetFamily.FOct.name ;
        paramName[ 1 ] = SetFamily.FOct.strength_concrete ;
        paramName[ 2 ] = SetFamily.FOct.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FOct.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FOct.depth_cover_side ;
      }
      else if ( familyname == SetFamily.FConti.FamilyName ) {
        paramName[ 0 ] = SetFamily.FConti.name ;
        paramName[ 1 ] = SetFamily.FConti.strength_concrete ;
        paramName[ 2 ] = SetFamily.FConti.depth_cover_top ;
        paramName[ 3 ] = SetFamily.FConti.depth_cover_bottom ;
        paramName[ 4 ] = SetFamily.FConti.depth_cover_side ;
      }
      else {
        return retID ;
      }

      id_sect++ ;
      retID = id_sect ;

      StbSecFoundation_RC s = new StbSecFoundation_RC()
      {
        id = id_sect,
        guid = GetGuid( symbol, "" ),
        name = Data.GetParameter_string( symbol, paramName[ 0 ] ),
        strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, paramName[ 1 ] ) ),
        StbSecFigureFoundation_RC = new StbSecFigureFoundation_RC() { },
        StbSecBarArrangementFoundation_RC = new StbSecBarArrangementFoundation_RC()
        {
          depth_cover_top = Data.GetParameter_double( symbol, paramName[ 2 ] ), depth_cover_bottom = Data.GetParameter_double( symbol, paramName[ 3 ] ), depth_cover_side = Data.GetParameter_double( symbol, paramName[ 4 ] ), Items = new List<object>(),
        },
      } ;

      if ( familyname == SetFamily.FRect.FamilyName ) {
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_Rect() { width_X = Data.GetParameter_double( symbol, SetFamily.FRect.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.FRect.DY ), depth = Data.GetParameter_double( symbol, SetFamily.FRect.depth ), } ;

        for ( int i = 0 ; i < SetFamily.FRect.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_Rect()
          {
            pos = (StbSecBarFoundation_RC_RectPos)i, strength = Data.GetParameter_string( symbol, SetFamily.FRect.strength ), D = Data.GetParameter_string( symbol, SetFamily.FRect.D[ i ] ), N = Data.GetParameter_int( symbol, SetFamily.FRect.count[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_RectPos.X_BOTTOM || bar.pos == StbSecBarFoundation_RC_RectPos.Y_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }
      else if ( familyname == SetFamily.FTRect.FamilyName ) {
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_TaperedRect()
        {
          width_X = Data.GetParameter_double( symbol, SetFamily.FTRect.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.FTRect.DY ), depth_base = Data.GetParameter_double( symbol, SetFamily.FTRect.depth_base ), depth_tip = Data.GetParameter_double( symbol, SetFamily.FTRect.depth_tip ),
        } ;

        for ( int i = 0 ; i < SetFamily.FTRect.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_Rect()
          {
            pos = (StbSecBarFoundation_RC_RectPos)i, strength = Data.GetParameter_string( symbol, SetFamily.FTRect.strength ), D = Data.GetParameter_string( symbol, SetFamily.FTRect.D[ i ] ), N = Data.GetParameter_int( symbol, SetFamily.FTRect.count[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_RectPos.X_BOTTOM || bar.pos == StbSecBarFoundation_RC_RectPos.Y_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }
      else if ( familyname == SetFamily.FTri.FamilyName ) {
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_Triangle() { width_X = Data.GetParameter_double( symbol, SetFamily.FTri.DX ), width_Y = Data.GetParameter_double( symbol, SetFamily.FTri.DY ), depth = Data.GetParameter_double( symbol, SetFamily.FTri.depth ), } ;

        for ( int i = 0 ; i < SetFamily.FTri.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_Triangle()
          {
            pos = (StbSecBarFoundation_RC_TrianglePos)i, strength = Data.GetParameter_string( symbol, SetFamily.FTri.strength ), D = Data.GetParameter_string( symbol, SetFamily.FTri.D[ i ] ), N = Data.GetParameter_int( symbol, SetFamily.FTri.count[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_TrianglePos.MAIN_BOTTOM || bar.pos == StbSecBarFoundation_RC_TrianglePos.TRANSVERSE_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }
      else if ( familyname == SetFamily.FETriangle.FamilyName ) {
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_EquiTriangle() { width_base = Data.GetParameter_double( symbol, SetFamily.FETriangle.B ), width_chamfer = Data.GetParameter_double( symbol, SetFamily.FETriangle.C ), depth = Data.GetParameter_double( symbol, SetFamily.FETriangle.depth ), } ;

        for ( int i = 0 ; i < SetFamily.FETriangle.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_ThreeWay()
          {
            pos = (StbSecBarFoundation_RC_ThreeWayPos)i, strength = Data.GetParameter_string( symbol, SetFamily.FETriangle.strength ), D = Data.GetParameter_string( symbol, SetFamily.FETriangle.D[ i ] ), N = Data.GetParameter_int( symbol, SetFamily.FETriangle.count[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_ThreeWayPos.MAIN_BOTTOM || bar.pos == StbSecBarFoundation_RC_ThreeWayPos.OUTSIDE_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }
      else if ( familyname == SetFamily.FOct.FamilyName ) {
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_Octagon()
        {
          width_X = Data.GetParameter_double( symbol, SetFamily.FOct.DX ),
          width_Y = Data.GetParameter_double( symbol, SetFamily.FOct.DY ),
          width_chamfer1_X = Data.GetParameter_double( symbol, SetFamily.FOct.CX1 ),
          width_chamfer1_Y = Data.GetParameter_double( symbol, SetFamily.FOct.CY1 ),
          width_chamfer2_X = Data.GetParameter_double( symbol, SetFamily.FOct.CX2 ),
          width_chamfer2_Y = Data.GetParameter_double( symbol, SetFamily.FOct.CY2 ),
          width_chamfer3_X = Data.GetParameter_double( symbol, SetFamily.FOct.CX3 ),
          width_chamfer3_Y = Data.GetParameter_double( symbol, SetFamily.FOct.CY3 ),
          width_chamfer4_X = Data.GetParameter_double( symbol, SetFamily.FOct.CX4 ),
          width_chamfer4_Y = Data.GetParameter_double( symbol, SetFamily.FOct.CY4 ),
          depth = Data.GetParameter_double( symbol, SetFamily.FOct.depth ),
        } ;

        for ( int i = 0 ; i < SetFamily.FOct.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_Rect()
          {
            pos = (StbSecBarFoundation_RC_RectPos)i, strength = Data.GetParameter_string( symbol, SetFamily.FOct.strength ), D = Data.GetParameter_string( symbol, SetFamily.FOct.D[ i ] ), N = Data.GetParameter_int( symbol, SetFamily.FOct.count[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_RectPos.X_BOTTOM || bar.pos == StbSecBarFoundation_RC_RectPos.Y_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }
      else if ( familyname == SetFamily.FConti.FamilyName ) {
        Enum.TryParse( Data.GetParameter_string( symbol, SetFamily.FConti.type ), out StbSecFoundation_RC_ContinuousType continuousType ) ;
        s.StbSecFigureFoundation_RC.Item = new StbSecFoundation_RC_Continuous()
        {
          width = Data.GetParameter_double( symbol, SetFamily.FConti.B ), depth_base = Data.GetParameter_double( symbol, SetFamily.FConti.depth_base ), depth_tip = Data.GetParameter_double( symbol, SetFamily.FConti.depth_tip ), type = continuousType,
        } ;

        for ( int i = 0 ; i < SetFamily.FConti.D.Length ; ++i ) {
          var bar = new StbSecBarFoundation_RC_Continuous()
          {
            pos = (StbSecBarFoundation_RC_ContinuousPos)i,
            strength = Data.GetParameter_string( symbol, SetFamily.FConti.strength ),
            D = Data.GetParameter_string( symbol, SetFamily.FConti.D[ i ] ),
            N = Data.GetParameter_int( symbol, SetFamily.FConti.count[ i ] ),
            pitch = Data.GetParameter_double( symbol, SetFamily.FConti.pitch[ i ] ),
          } ;

          if ( bar.D == "" || bar.N <= 0 ) {
            if ( bar.pos == StbSecBarFoundation_RC_ContinuousPos.MAIN_BOTTOM || bar.pos == StbSecBarFoundation_RC_ContinuousPos.TRANSVERSE_BOTTOM ) {
              //省略不可
              s.StbSecBarArrangementFoundation_RC.Items.Clear() ;
              break ;
            }
            else {
              //省略可
            }
          }
          else {
            s.StbSecBarArrangementFoundation_RC.Items.Add( bar ) ;
          }
        }
      }


      stb.StbModel.StbSections.StbSecFoundation_RC.Add( s ) ;

      return retID ;
    }

    /// <summary>
    /// 杭断面の出力
    /// </summary>
    /// <param name="ins"></param>
    /// <returns></returns>
    private static int Export_SecPile( FamilyInstance ins )
    {
      FamilySymbol symbol = ins.Symbol ;
      int retID = -1 ;

      bool top = false ;
      bool foot = false ;
      string[] paramName = new string[ 12 ] ;
      string familyname = symbol.Family.Name ;
      List<double> Diameter = new List<double>() ;
      double length_taper_foot = 1000 ;
      double length_extended_foot = 100 ;
      double length_taper_top = 1000 ;
      double length_extended_top = 100 ;


      //鋼管杭、既製杭の継杭本数を算出するローカル関数
      int numpile( double length_all, double length_pile )
      {
        int n = 0 ;
        if ( length_pile > 0 ) {
          n = (int)Math.Round( length_all / length_pile, 0, MidpointRounding.AwayFromZero ) ;
        }

        if ( n < 1 ) {
          n = 1 ;
        }

        return n ;
      }

      ;


      if ( familyname == SetFamily.CastinPile.FamilyName ) {
        //RC杭

        paramName[ 0 ] = SetFamily.CastinPile.name ;
        paramName[ 1 ] = SetFamily.CastinPile.strength_concrete ;
        paramName[ 2 ] = SetFamily.CastinPile.depth_cover ;
        paramName[ 3 ] = SetFamily.CastinPile.depth_cover_top ;
        paramName[ 4 ] = SetFamily.CastinPile.D ; //ストレート
        paramName[ 5 ] = SetFamily.CastinPile.D ; //脚部軸
        paramName[ 6 ] = SetFamily.CastinPile.D_extended_foot ; //脚部拡底
        paramName[ 7 ] = SetFamily.CastinPile.D_extended_top ; //頂部拡頭
        paramName[ 8 ] = SetFamily.CastinPile.D ; //頂部軸
        paramName[ 9 ] = SetFamily.CastinPile.D_extended_top ; //両端拡頭
        paramName[ 10 ] = SetFamily.CastinPile.D ; //両端軸
        paramName[ 11 ] = SetFamily.CastinPile.D_extended_foot ; //両端拡底

        length_taper_foot = Data.GetParameter_double( symbol, SetFamily.CastinPile.length_foot_taper ) ;
        length_extended_foot = Data.GetParameter_double( symbol, SetFamily.CastinPile.length_foot_Revit ) ;
        length_taper_top = Data.GetParameter_double( symbol, SetFamily.CastinPile.length_head_taper ) ;
        length_extended_top = Data.GetParameter_double( symbol, SetFamily.CastinPile.length_head ) ;

        top = Data.GetParameter_bool( symbol, "拡頭" ) ;
        foot = Data.GetParameter_bool( symbol, "拡底" ) ;


        id_sect++ ;
        retID = id_sect ;

        StbSecPile_RC s = new StbSecPile_RC()
        {
          id = id_sect,
          guid = GetGuid( symbol, "" ),
          name = Data.GetParameter_string( symbol, paramName[ 0 ] ),
          strength_concrete = Data.GetConcreteFC( Data.GetParameter_string( symbol, paramName[ 1 ] ) ),
          StbSecFigurePile_RC = new StbSecFigurePile_RC() { },
          StbSecBarArrangementPile_RC = new StbSecBarArrangementPile_RC()
          {
            depth_cover = Data.GetParameter_double( symbol, paramName[ 2 ] ), depth_cover_top = Data.GetParameter_double( symbol, paramName[ 3 ] ), isSpiral = false, Items = new List<object>(),
          },
        } ;

        if ( ! top && ! foot ) {
          var fig = new StbSecPile_RC_Straight() ;

          if ( Diameter.Count > 0 ) {
            fig.D = Diameter[ 0 ] ;
          }
          else {
            fig.D = Data.GetParameter_double( symbol, paramName[ 4 ] ) ;
          }

          s.StbSecFigurePile_RC.Item = fig ;
        }
        else if ( top && foot ) {
          var fig = new StbSecPile_RC_ExtendedTopFoot() { angle_extended_top_taper = 0, length_extended_foot = length_extended_foot, angle_extended_foot_taper = 0, } ;

          if ( Diameter.Count > 2 ) {
            fig.D_extended_top = Diameter[ 0 ] ;
            fig.D_axial = Diameter[ 1 ] ;
            fig.D_extended_foot = Diameter[ 2 ] ;
          }
          else {
            fig.D_extended_top = Data.GetParameter_double( symbol, paramName[ 9 ] ) ;
            fig.D_axial = Data.GetParameter_double( symbol, paramName[ 10 ] ) ;
            fig.D_extended_foot = Data.GetParameter_double( symbol, paramName[ 11 ] ) ;
          }

          double diff_D = ( fig.D_extended_foot - fig.D_axial ) / 2 ;
          fig.angle_extended_foot_taper = Calc_taper_angle( diff_D, length_taper_foot ) ;

          diff_D = ( fig.D_extended_top - fig.D_axial ) / 2 ;
          fig.angle_extended_top_taper = Calc_taper_angle( diff_D, length_taper_foot ) ;

          s.StbSecFigurePile_RC.Item = fig ;
        }
        else if ( foot ) {
          var fig = new StbSecPile_RC_ExtendedFoot() { length_extended_foot = length_extended_foot, angle_extended_foot_taper = 0, } ;

          if ( Diameter.Count > 1 ) {
            fig.D_axial = Diameter[ 0 ] ;
            fig.D_extended_foot = Diameter[ 1 ] ;
          }
          else {
            fig.D_axial = Data.GetParameter_double( symbol, paramName[ 5 ] ) ;
            fig.D_extended_foot = Data.GetParameter_double( symbol, paramName[ 6 ] ) ;
          }

          double diff_D = ( fig.D_extended_foot - fig.D_axial ) / 2 ;
          fig.angle_extended_foot_taper = Calc_taper_angle( diff_D, length_taper_foot ) ;

          s.StbSecFigurePile_RC.Item = fig ;
        }
        else if ( top ) {
          var fig = new StbSecPile_RC_ExtendedTop() { angle_extended_top_taper = 0, } ;

          if ( Diameter.Count > 1 ) {
            fig.D_extended_top = Diameter[ 0 ] ;
            fig.D_axial = Diameter[ 1 ] ;
          }
          else {
            fig.D_extended_top = Data.GetParameter_double( symbol, paramName[ 7 ] ) ;
            fig.D_axial = Data.GetParameter_double( symbol, paramName[ 8 ] ) ;
          }

          double diff_D = ( fig.D_extended_top - fig.D_axial ) / 2 ;
          fig.angle_extended_top_taper = Calc_taper_angle( diff_D, length_taper_foot ) ;

          s.StbSecFigurePile_RC.Item = fig ;
        }


        //配筋
        var bars = new List<StbSecBarPile_RC_TopCenterBottom>() ;
        if ( familyname == SetFamily.CastinPile.FamilyName ) {
          for ( int i = 0 ; i < SetFamily.CastinPile.D_main_circumference_1st.Length ; ++i ) {
            var bar = new StbSecBarPile_RC_TopCenterBottom()
            {
              pos = (StbSecBarPile_RC_TopCenterBottomPos)i,
              D_main_circumference_1st = Data.GetParameter_string( symbol, SetFamily.CastinPile.D_main_circumference_1st[ i ] ),
              D_main_circumference_2nd = "",
              D_main_core = Data.GetParameter_string( symbol, SetFamily.CastinPile.D_main_core[ i ] ),
              D_band = Data.GetParameter_string( symbol, SetFamily.CastinPile.D_band[ i ] ),
              strength_main_circumference_1st = Data.GetParameter_string( symbol, SetFamily.CastinPile.strength_main_circumference_1st ),
              strength_main_circumference_2nd = "",
              strength_main_core = Data.GetParameter_string( symbol, SetFamily.CastinPile.strength_main_core ),
              strength_band = Data.GetParameter_string( symbol, SetFamily.CastinPile.strength_band ),
              N_main_circumference_1st = Data.GetParameter_int( symbol, SetFamily.CastinPile.count_main_circumference_1st[ i ] ),
              N_main_circumference_2nd = 0,
              N_main_core = Data.GetParameter_int( symbol, SetFamily.CastinPile.count_main_core[ i ] ),
              pitch_band = Data.GetParameter_double( symbol, SetFamily.CastinPile.pitch_band[ i ] ),
              length_bar = 0,
              length_lap_bar = 0,
            } ;

            if ( bar.D_main_circumference_1st == "" || bar.D_band == "" || bar.N_main_circumference_1st <= 0 || bar.pitch_band < 0.1 ) {
              //必須項目がない場合は出力しない
              bars.Clear() ;
              break ;
            }

            bars.Add( bar ) ;
          }
        }


        if ( bars.Count == 3 ) {
          bool tc = CompareTo_PileBar( bars[ 0 ], bars[ 1 ] ) ;
          bool cb = CompareTo_PileBar( bars[ 1 ], bars[ 2 ] ) ;
          if ( tc && cb ) {
            s.StbSecBarArrangementPile_RC.Items.Add( new StbSecBarPile_RC_Same()
            {
              D_main_circumference_1st = bars[ 0 ].D_main_circumference_1st,
              D_main_circumference_2nd = bars[ 0 ].D_main_circumference_2nd,
              D_main_core = bars[ 0 ].D_main_core,
              D_band = bars[ 0 ].D_band,
              strength_main_circumference_1st = bars[ 0 ].strength_main_circumference_1st,
              strength_main_circumference_2nd = bars[ 0 ].strength_main_circumference_2nd,
              strength_main_core = bars[ 0 ].strength_main_core,
              strength_band = bars[ 0 ].strength_band,
              N_main_circumference_1st = bars[ 0 ].N_main_circumference_1st,
              N_main_circumference_2nd = bars[ 0 ].N_main_circumference_2nd,
              N_main_core = bars[ 0 ].N_main_core,
              pitch_band = bars[ 0 ].pitch_band,
            } ) ;
          }
          else if ( tc || cb ) {
            s.StbSecBarArrangementPile_RC.Items.Add( new StbSecBarPile_RC_TopBottom()
            {
              pos = StbSecBarPile_RC_TopBottomPos.TOP,
              D_main_circumference_1st = bars[ 0 ].D_main_circumference_1st,
              D_main_circumference_2nd = bars[ 0 ].D_main_circumference_2nd,
              D_main_core = bars[ 0 ].D_main_core,
              D_band = bars[ 0 ].D_band,
              strength_main_circumference_1st = bars[ 0 ].strength_main_circumference_1st,
              strength_main_circumference_2nd = bars[ 0 ].strength_main_circumference_2nd,
              strength_main_core = bars[ 0 ].strength_main_core,
              strength_band = bars[ 0 ].strength_band,
              N_main_circumference_1st = bars[ 0 ].N_main_circumference_1st,
              N_main_circumference_2nd = bars[ 0 ].N_main_circumference_2nd,
              N_main_core = bars[ 0 ].N_main_core,
              pitch_band = bars[ 0 ].pitch_band,
              length_bar = bars[ 0 ].length_bar,
              length_lap_bar = bars[ 0 ].length_lap_bar,
            } ) ;

            s.StbSecBarArrangementPile_RC.Items.Add( new StbSecBarPile_RC_TopBottom()
            {
              pos = StbSecBarPile_RC_TopBottomPos.BOTTOM,
              D_main_circumference_1st = bars[ 2 ].D_main_circumference_1st,
              D_main_circumference_2nd = bars[ 2 ].D_main_circumference_2nd,
              D_main_core = bars[ 2 ].D_main_core,
              D_band = bars[ 2 ].D_band,
              strength_main_circumference_1st = bars[ 2 ].strength_main_circumference_1st,
              strength_main_circumference_2nd = bars[ 2 ].strength_main_circumference_2nd,
              strength_main_core = bars[ 2 ].strength_main_core,
              strength_band = bars[ 2 ].strength_band,
              N_main_circumference_1st = bars[ 2 ].N_main_circumference_1st,
              N_main_circumference_2nd = bars[ 2 ].N_main_circumference_2nd,
              N_main_core = bars[ 2 ].N_main_core,
              pitch_band = bars[ 2 ].pitch_band,
              length_bar = bars[ 2 ].length_bar,
              length_lap_bar = bars[ 2 ].length_lap_bar,
            } ) ;
          }
          else {
            s.StbSecBarArrangementPile_RC.Items.AddRange( bars ) ;
          }
        }


        stb.StbModel.StbSections.StbSecPile_RC.Add( s ) ;
      }
      else if ( familyname == SetFamily.Pile_PHC.FamilyName || familyname == SetFamily.Pile_ST.FamilyName || familyname == SetFamily.Pile_SC.FamilyName || familyname == SetFamily.Pile_PRC.FamilyName || familyname == SetFamily.Pile_CPRC.FamilyName ) {
        //既製杭

        StbSecFigurePileProduct fig = new StbSecFigurePileProduct() ;

        string product_company = Data.GetParameter_string( ins.Symbol, BuiltInParameter.ALL_MODEL_MANUFACTURER ) ;
        string product_code = Data.GetParameter_string( ins.Symbol, BuiltInParameter.ALL_MODEL_MODEL ) ;
        if ( product_company == "" ) {
          //メーカー名がなければ型番もなしにする
          product_code = "" ;
        }

        if ( familyname == SetFamily.Pile_PHC.FamilyName ) {
          double length_all = Data.GetParameter_double( ins, SetFamily.Pile_PHC.length_all ) ;
          double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PHC.length_pile ) ;
          int n = numpile( length_all, length_pile ) ;

          fig.StbSecPileProduct_PHC = new List<StbSecPileProduct_PHC>() ;

          paramName[ 0 ] = SetFamily.Pile_PHC.name ;

          string kind = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PHC.kind ) ;
          double D = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PHC.D ) ;
          double t = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PHC.t ) ;
          string strength_concrete = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PHC.strength_concrete ) ;
          double D_PC = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PHC.D_PC ) ;
          int N_PC = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_PHC.N_PC ) ;
          string strength_PC = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PHC.strength_PC ) ;

          for ( int i = 1 ; i <= n ; ++i ) {
            fig.StbSecPileProduct_PHC.Add( new StbSecPileProduct_PHC()
            {
              id_order = i,
              product_company = product_company,
              product_code = product_code,
              length_pile = length_pile,
              kind = kind,
              D = D,
              t = t,
              strength_concrete = strength_concrete,
              D_PC = D_PC,
              N_PC = N_PC,
              strength_PC = strength_PC,
            } ) ;
          }
        }
        else if ( familyname == SetFamily.Pile_ST.FamilyName ) {
          double length_all = Data.GetParameter_double( ins, SetFamily.Pile_ST.length_all ) ;
          double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.length_pile ) ;
          int n = numpile( length_all, length_pile ) ;

          fig.StbSecPileProduct_ST = new List<StbSecPileProduct_ST>() ;

          paramName[ 0 ] = SetFamily.Pile_ST.name ;

          string kind = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_ST.kind ) ;
          double D1 = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.D1 ) ;
          double D2 = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.D2 ) ;
          double t1 = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.t1 ) ;
          double t2 = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.t2 ) ;
          string strength_concrete = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_ST.strength_concrete ) ;
          double D_PC = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_ST.D_PC ) ;
          int N_PC = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_ST.N_PC ) ;
          string strength_PC = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_ST.strength_PC ) ;

          for ( int i = 1 ; i <= n ; ++i ) {
            fig.StbSecPileProduct_ST.Add( new StbSecPileProduct_ST()
            {
              id_order = i,
              product_company = product_company,
              product_code = product_code,
              length_pile = length_pile,
              kind = kind,
              D1 = D1,
              D2 = D2,
              t1 = t1,
              t2 = t2,
              strength_concrete = strength_concrete,
              D_PC = D_PC,
              N_PC = N_PC,
              strength_PC = strength_PC,
            } ) ;
          }
        }
        else if ( familyname == SetFamily.Pile_SC.FamilyName ) {
          double length_all = Data.GetParameter_double( ins, SetFamily.Pile_SC.length_all ) ;
          double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_SC.length_pile ) ;
          int n = numpile( length_all, length_pile ) ;

          fig.StbSecPileProduct_SC = new List<StbSecPileProduct_SC>() ;

          paramName[ 0 ] = SetFamily.Pile_SC.name ;

          string kind = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_SC.kind ) ;
          double D = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_SC.D ) ;
          double tc = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_SC.tc ) ;
          double ts = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_SC.ts ) ;
          string strength_concrete = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_SC.strength_concrete ) ;
          string strength_pile = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_SC.strength_pipe ) ;

          for ( int i = 1 ; i <= n ; ++i ) {
            fig.StbSecPileProduct_SC.Add( new StbSecPileProduct_SC()
            {
              id_order = i,
              product_company = product_company,
              product_code = product_code,
              length_pile = length_pile,
              kind = kind,
              D = D,
              tc = tc,
              ts = ts,
              strength_concrete = strength_concrete,
              strength_pipe = strength_pile,
            } ) ;
          }
        }
        else if ( familyname == SetFamily.Pile_PRC.FamilyName ) {
          double length_all = Data.GetParameter_double( ins, SetFamily.Pile_PRC.length_all ) ;
          double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PRC.length_pile ) ;
          int n = numpile( length_all, length_pile ) ;

          fig.StbSecPileProduct_PRC = new List<StbSecPileProduct_PRC>() ;

          paramName[ 0 ] = SetFamily.Pile_PRC.name ;

          string kind = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PRC.kind ) ;
          double D = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PRC.D ) ;
          double tc = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PRC.tc ) ;
          string strength_concrete = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PRC.strength_concrete ) ;
          double D_PC = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_PRC.D_PC ) ;
          int N_PC = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_PRC.N_PC ) ;
          string strength_PC = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PRC.strength_PC ) ;
          string D_bar = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PRC.D_bar ) ;
          int N_bar = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_PRC.N_bar ) ;
          string strength_bar = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_PRC.strength_bar ) ;

          for ( int i = 1 ; i <= n ; ++i ) {
            fig.StbSecPileProduct_PRC.Add( new StbSecPileProduct_PRC()
            {
              id_order = i,
              product_company = product_company,
              product_code = product_code,
              length_pile = length_pile,
              kind = kind,
              D = D,
              tc = tc,
              strength_concrete = strength_concrete,
              D_PC = D_PC,
              N_PC = N_PC,
              strength_PC = strength_PC,
              D_bar = D_bar,
              N_bar = N_bar,
              strength_bar = strength_bar,
            } ) ;
          }
        }
        else if ( familyname == SetFamily.Pile_CPRC.FamilyName ) {
          double length_all = Data.GetParameter_double( ins, SetFamily.Pile_CPRC.length_all ) ;
          double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_CPRC.length_pile ) ;
          int n = numpile( length_all, length_pile ) ;

          fig.StbSecPileProduct_CPRC = new List<StbSecPileProduct_CPRC>() ;

          paramName[ 0 ] = SetFamily.Pile_CPRC.name ;

          string kind = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_CPRC.kind ) ;
          double D = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_CPRC.D ) ;
          double tc = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_CPRC.tc ) ;
          string strength_concrete = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_CPRC.strength_concrete ) ;
          double D_PC = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_CPRC.D_PC ) ;
          int N_PC = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_CPRC.N_PC ) ;
          string strength_PC = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_CPRC.strength_PC ) ;
          string D_bar = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_CPRC.D_bar ) ;
          int N_bar = Data.GetParameter_int( ins.Symbol, SetFamily.Pile_CPRC.N_bar ) ;
          string strength_bar = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_CPRC.strength_bar ) ;

          for ( int i = 1 ; i <= n ; ++i ) {
            fig.StbSecPileProduct_CPRC.Add( new StbSecPileProduct_CPRC()
            {
              id_order = i,
              product_company = product_company,
              product_code = product_code,
              length_pile = length_pile,
              kind = kind,
              D = D,
              tc = tc,
              strength_concrete = strength_concrete,
              D_PC = D_PC,
              N_PC = N_PC,
              strength_PC = strength_PC,
              D_bar = D_bar,
              N_bar = N_bar,
              strength_bar = strength_bar,
            } ) ;
          }
        }


        id_sect++ ;
        retID = id_sect ;

        StbSecPileProduct s = new StbSecPileProduct()
        {
          id = id_sect, guid = GetGuid( symbol, "" ), name = Data.GetParameter_string( symbol, paramName[ 0 ] ), StbSecFigurePileProduct = fig,
        } ;


        stb.StbModel.StbSections.StbSecPileProduct.Add( s ) ;
      }
      else if ( familyname == SetFamily.Pile_S.FamilyName ) {
        //鋼管杭
        StbSecFigurePile_S fig = new StbSecFigurePile_S() ;

        string product_company = Data.GetParameter_string( ins.Symbol, BuiltInParameter.ALL_MODEL_MANUFACTURER ) ;
        string product_code = Data.GetParameter_string( ins.Symbol, BuiltInParameter.ALL_MODEL_MODEL ) ;
        if ( product_company == "" ) {
          //メーカー名がなければ型番もなしにする
          product_code = "" ;
        }

        double length_all = Data.GetParameter_double( ins, SetFamily.Pile_S.length_all ) ;
        double length_pile = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_S.length_pile ) ;
        int n = numpile( length_all, length_pile ) ;

        fig.StbSecPile_S_Straight = new List<StbSecPile_S_Straight>() ;

        double D = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_S.D ) ;
        double t = Data.GetParameter_double( ins.Symbol, SetFamily.Pile_S.t ) ;
        string strength = Data.GetParameter_string( ins.Symbol, SetFamily.Pile_S.strength ) ;

        for ( int i = 1 ; i <= n ; ++i ) {
          fig.StbSecPile_S_Straight.Add( new StbSecPile_S_Straight()
          {
            id_order = i,
            product_company = product_company,
            product_code = product_code,
            length_pile = length_pile,
            D = D,
            t = t,
            strength = strength,
          } ) ;
        }

        id_sect++ ;
        retID = id_sect ;

        StbSecPile_S s = new StbSecPile_S()
        {
          id = id_sect, guid = GetGuid( symbol, "" ), name = Data.GetParameter_string( symbol, SetFamily.Pile_S.name ), StbSecFigurePile_S = fig,
        } ;


        stb.StbModel.StbSections.StbSecPile_S.Add( s ) ;
      }

      return retID ;
    }

    /// <summary>
    /// 基礎・杭の出力
    /// </summary>
    private static void Export_Footing()
    {
      List<string> FootingFamilyName = SetFamily.FoFName.FamilyName[ 0 ].ToList() ;
      List<string> PileFamilyName = SetFamily.FoFName.FamilyName.Last().Where( x => x != "" ).ToList() ;

      FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
      ElementCategoryFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_StructuralFoundation ) ;
      IList<Element> elements = collector.WherePasses( filter ).WhereElementIsNotElementType().ToElements() ;
      List<FamilyInstance> instances = elements.OfType<FamilyInstance>().Where( x => FootingFamilyName.Contains( x.Symbol.Family.Name ) && ! x.Symbol.Family.IsInPlace ).ToList() ;
      List<FamilyInstance> piles = elements.OfType<FamilyInstance>().Where( x => PileFamilyName.Contains( x.Symbol.Family.Name ) && ! x.Symbol.Family.IsInPlace ).ToList() ;

      Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>() ;

      //基礎と杭の組み合わせを作る
      Dictionary<FamilyInstance, List<FamilyInstance>> instances2 = instances.ToDictionary( x => x, y => new List<FamilyInstance>() ) ;
      Dictionary<ElementId, LocationPoint> fpos = instances.ToDictionary( x => x.Id, y => y.Location as LocationPoint ) ;
      for ( int i = 0 ; i < piles.Count ; ++i ) {
        if ( piles[ i ].GroupId.Value() != -1 && instances2.Any( x => x.Key.GroupId == piles[ i ].GroupId ) ) {
          //グループ化されていれば、同じグループの基礎と組み合わせる。
          instances2.Where( x => x.Key.GroupId == piles[ i ].GroupId ).First().Value.Add( piles[ i ] ) ;
        }
        else {
          //近い基礎に組み合わせる
          if ( piles[ i ].Location is LocationPoint loc ) {
            double mindist = fpos.Where( x => x.Value != null ).Min( x => x.Value.Point.DistanceTo( loc.Point ) ) ;
            ElementId eid = fpos.Where( x => x.Value != null && Math.Abs( x.Value.Point.DistanceTo( loc.Point ) - mindist ) < 0.00001 ).First().Key ;
            instances2.Where( x => x.Key.Id == eid ).First().Value.Add( piles[ i ] ) ;
          }
        }
      }

      //フーチング
      foreach ( KeyValuePair<FamilyInstance, List<FamilyInstance>> k in instances2 ) {
        FamilyInstance ins = k.Key ;
        List<FamilyInstance> piles2 = k.Value ;

        LocationPoint loc = ins.Location as LocationPoint ;
        double height = Data.GetParameter_double( ins, BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM, true ) ;
        XYZ pos0 = loc.Point - new XYZ( 0, 0, height ) ;
        XYZ pos1 = Commons.ft2mm( pos0 ) ;

        StbFooting f = new StbFooting()
        {
          guid = GetGuid( ins, "" ),
          id_node = GetNodeId( pos1 ),
          rotate = XYZ.BasisX.AngleOnPlaneTo( ins.HandOrientation, XYZ.BasisZ ) / Math.PI * 180,
          offset_X = 0,
          offset_Y = 0,
          level_bottom = Data.GetParameter_double( ins, BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM ),
        } ;

        if ( ! sect.ContainsKey( ins.Symbol.Id ) ) {
          f.id_section = Export_SecFoundation( ins ) ;
          if ( f.id_section < 0 ) continue ;
          sect.Add( ins.Symbol.Id, f.id_section ) ;
        }
        else {
          f.id_section = sect[ ins.Symbol.Id ] ;
        }

        string[] paramName = new string[ 7 ] ;
        string familyname = ins.Symbol.Family.Name ;
        if ( familyname == SetFamily.FRect.FamilyName ) {
          paramName[ 0 ] = SetFamily.FRect.NameMembers ;
          paramName[ 1 ] = SetFamily.FRect.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.FRect.thickness_ex_end_X ;
          paramName[ 3 ] = SetFamily.FRect.thickness_ex_start_Y ;
          paramName[ 4 ] = SetFamily.FRect.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.FRect.thickness_ex_top ;
          paramName[ 6 ] = SetFamily.FRect.thickness_ex_bottom ;
        }
        else if ( familyname == SetFamily.FTRect.FamilyName ) {
          paramName[ 0 ] = SetFamily.FTRect.NameMembers ;
          paramName[ 1 ] = SetFamily.FTRect.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.FTRect.thickness_ex_end_X ;
          paramName[ 3 ] = SetFamily.FTRect.thickness_ex_start_Y ;
          paramName[ 4 ] = SetFamily.FTRect.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.FTRect.thickness_ex_top ;
          paramName[ 6 ] = SetFamily.FTRect.thickness_ex_bottom ;
        }
        else if ( familyname == SetFamily.FTri.FamilyName ) {
          paramName[ 0 ] = SetFamily.FTri.NameMembers ;
          paramName[ 1 ] = SetFamily.FTri.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.FTri.thickness_ex_end_X ;
          paramName[ 3 ] = SetFamily.FTri.thickness_ex_start_Y ;
          paramName[ 4 ] = SetFamily.FTri.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.FTri.thickness_ex_top ;
          paramName[ 6 ] = SetFamily.FTri.thickness_ex_bottom ;
        }
        else if ( familyname == SetFamily.FETriangle.FamilyName ) {
          paramName[ 0 ] = SetFamily.FETriangle.NameMembers ;
          paramName[ 1 ] = SetFamily.FETriangle.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.FETriangle.thickness_ex_end_X ;
          paramName[ 3 ] = SetFamily.FETriangle.thickness_ex_start_Y ;
          paramName[ 4 ] = SetFamily.FETriangle.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.FETriangle.thickness_ex_top ;
          paramName[ 6 ] = SetFamily.FETriangle.thickness_ex_bottom ;
        }
        else if ( familyname == SetFamily.FOct.FamilyName ) {
          paramName[ 0 ] = SetFamily.FOct.NameMembers ;
          paramName[ 1 ] = SetFamily.FOct.thickness_ex_start_X ;
          paramName[ 2 ] = SetFamily.FOct.thickness_ex_end_X ;
          paramName[ 3 ] = SetFamily.FOct.thickness_ex_start_Y ;
          paramName[ 4 ] = SetFamily.FOct.thickness_ex_end_Y ;
          paramName[ 5 ] = SetFamily.FOct.thickness_ex_top ;
          paramName[ 6 ] = SetFamily.FOct.thickness_ex_bottom ;
        }
        else {
          continue ;
        }

        id++ ;
        f.id = id ;

        f.name = ins.Symbol.Name ;
        f.thickness_add_start_X = Data.GetParameter_double( ins, paramName[ 1 ] ) ;
        f.thickness_add_end_X = Data.GetParameter_double( ins, paramName[ 2 ] ) ;
        f.thickness_add_start_Y = Data.GetParameter_double( ins, paramName[ 3 ] ) ;
        f.thickness_add_end_Y = Data.GetParameter_double( ins, paramName[ 4 ] ) ;
        f.thickness_add_top = Data.GetParameter_double( ins, paramName[ 5 ] ) ;
        f.thickness_add_bottom = Data.GetParameter_double( ins, paramName[ 6 ] ) ;

        stb.StbModel.StbMembers.StbFootings.Add( f ) ;
        Data.AddLog( Data.LogCode.footing, ins, f.id, f.id_section ) ;

        if ( piles2.Count > 0 ) {
          for ( int j = 0 ; j < piles2.Count ; ++j ) {
            ins = piles2[ j ] ;

            StbPile p = new StbPile() { guid = GetGuid( ins, "" ), id_node = f.id_node, name = ins.Symbol.Name, } ;

            if ( ! sect.ContainsKey( ins.Symbol.Id ) ) {
              p.id_section = Export_SecPile( ins ) ;
              if ( p.id_section < 0 ) continue ;
              sect.Add( ins.Symbol.Id, p.id_section ) ;
            }
            else {
              p.id_section = sect[ ins.Symbol.Id ] ;
            }

            familyname = ins.Symbol.Family.Name ;

            var sec = stb.StbModel.StbSections.StbSecPile_RC.Find( a => a.id == p.id_section ) ;
            if ( sec != null ) {
              int figtype = 0 ;
              if ( sec.StbSecFigurePile_RC.Item is StbSecPile_RC_Straight ) {
                figtype = 1 ;
              }
              else if ( sec.StbSecFigurePile_RC.Item is StbSecPile_RC_ExtendedFoot ) {
                figtype = 2 ;
              }
              else if ( sec.StbSecFigurePile_RC.Item is StbSecPile_RC_ExtendedTop ) {
                figtype = 3 ;
              }
              else if ( sec.StbSecFigurePile_RC.Item is StbSecPile_RC_ExtendedTopFoot ) {
                figtype = 4 ;
              }

              if ( familyname == SetFamily.CastinPile.FamilyName ) {
                p.kind_structure = StbPileKind_structure.RC ;

                p.length_all = Data.GetParameter_double( ins.Symbol, SetFamily.CastinPile.length_all ) ;
                p.length_head = ( figtype == 3 || figtype == 4 ? Data.GetParameter_double( ins.Symbol, SetFamily.CastinPile.length_head ) : 0 ) ;
                p.length_foot = ( figtype == 2 || figtype == 4 ? Data.GetParameter_double( ins.Symbol, SetFamily.CastinPile.length_foot ) : 0 ) ;
              }
              else {
                continue ;
              }
            }
            else {
              if ( familyname == SetFamily.Pile_PHC.FamilyName || familyname == SetFamily.Pile_ST.FamilyName || familyname == SetFamily.Pile_SC.FamilyName || familyname == SetFamily.Pile_PRC.FamilyName || familyname == SetFamily.Pile_CPRC.FamilyName ) {
                p.kind_structure = StbPileKind_structure.PC ;
              }
              else if ( familyname == SetFamily.Pile_S.FamilyName ) {
                p.kind_structure = StbPileKind_structure.S ;
              }
              else {
                continue ;
              }
            }


            if ( piles2[ j ].Location is LocationPoint pileLoc ) {
              XYZ ppos = Commons.ft2mm( pileLoc.Point ) ;
              XYZ offset = ppos - pos1 ;
              p.offset_X = offset.X ;
              p.offset_Y = offset.Y ;
              p.level_top = offset.Z ;
            }

            if ( Data.GetPile0Length( piles2[ j ].Symbol ) ) {
              switch ( Data.pileSetting ) {
                case Data.ExportPileSetting.input :
                  //そのまま値を出力
                  break ;
                case Data.ExportPileSetting.none :
                  //0にしておけば出力されない
                  p.length_all = 0 ;
                  p.length_head = 0 ;
                  p.length_foot = 0 ;
                  break ;
              }
            }

            id++ ;
            p.id = id ;
            stb.StbModel.StbMembers.StbPiles.Add( p ) ;
            Data.AddLog( Data.LogCode.pile, ins, p.id, p.id_section ) ;
          }
        }
      }


      //布基礎
      instances = elements.OfType<FamilyInstance>().Where( x => x.Symbol.Family.Name == SetFamily.FConti.FamilyName && ! x.Symbol.Family.IsInPlace ).ToList() ;
      for ( int i = 0 ; i < instances.Count ; ++i ) {
        LocationCurve loc = instances[ i ].Location as LocationCurve ;
        id++ ;

        StbStripFooting f = new StbStripFooting()
        {
          id = id,
          guid = GetGuid( instances[ i ], "" ),
          name = instances[ i ].Symbol.Name,
          id_node_start = GetNodeId( Commons.ft2mm( loc.Curve.GetEndPoint( 0 ) ) ),
          id_node_end = GetNodeId( Commons.ft2mm( loc.Curve.GetEndPoint( 1 ) ) ),
          kind_structure = "RC",
          level = Data.GetParameter_double( instances[ i ], BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM ),
          offset = 0,
          length_ex_start = Data.GetParameter_double( instances[ i ], SetFamily.FConti.length_ex_start ),
          length_ex_end = Data.GetParameter_double( instances[ i ], SetFamily.FConti.length_ex_end ),
        } ;

        if ( ! sect.ContainsKey( instances[ i ].Symbol.Id ) ) {
          f.id_section = Export_SecFoundation( instances[ i ] ) ;
          if ( f.id_section < 0 ) continue ;
          sect.Add( instances[ i ].Symbol.Id, f.id_section ) ;
        }
        else {
          f.id_section = sect[ instances[ i ].Symbol.Id ] ;
        }

        stb.StbModel.StbMembers.StbStripFootings.Add( f ) ;
        Data.AddLog( Data.LogCode.footing, instances[ i ], f.id, f.id_section ) ;
      }
    }

    #endregion


    /// <summary>
    /// Guidの取得
    /// </summary>
    /// <param name="element"></param>
    /// <param name="paraName"></param>
    /// <returns></returns>
    private static string GetGuid( Element element, string paraName )
    {
      if ( element == null ) return "" ;

      string id = Data.GetStorageGuid( element.Id ) ;

      //パラメータには保持しないことになった
      //var p = element.LookupParameter(paraName);
      //if (p != null)
      //{
      //    if (Guid.TryParse(p.AsString(), out Guid guid))
      //    {
      //        //パラメータに保持していればパラメータから
      //        id = guid.ToString("N");
      //    }
      //}

      if ( id == "" ) {
        ////なければ、UniqueIdを出力
        //id = element.UniqueId.Replace("-", "");

        //拡張ストレージにない場合はUniqueId
        //UniqueIdは40桁なので加工して出力する
        Guid guid = Data.Convertguid( element.UniqueId ) ;
        id = guid.ToString( "N" ) ;

        //拡張ストレージに作ったGuidを追加しておく
        Data.SaveGuid( id, element.Id ) ;
      }


      return id ;
    }
  }
}