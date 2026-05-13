using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

using static STBclass;
using static STBclass.StbCommonClass;
using static STBclass.StbModelClass;
using static STBclass.StbModelClass.StbSectionsClass;
using static STBclass.StbModelClass.StbMembersClass;

using static STBLink.Data;


namespace STBLink
{
    class ToSTB
    {

        private static STBclass stb = null;

        //private static XYZ origin = new XYZ();
        private static List<Level> Levels = null;

        private static int id = 0;
        private static int id_sect = 0;



        private static List<GridInformation>[] GridInfo = null;







        /// <summary>
        /// Export
        /// </summary>
        internal static void ExportSTB(string savepath)
        {
            if (Check_PileZeroLength() == 1)
            {
                ExportForm ef = new ExportForm();
                if (ef.ShowDialog() != DialogResult.OK) return;
            }

            stb = new STBclass()
            {
                version = "1.4.00",
            };

            #region 初期化

            stb.StbModel.StbSections.StbSecSteel = new StbSecSteel_Class()
            {
                StbSecRoll_H = new List<StbSecSteel_Class.StbSecRoll_H_Class>(),
                StbSecBuild_H = new List<StbSecSteel_Class.StbSecBuild_H_Class>(),
                StbSecRoll_BOX = new List<StbSecSteel_Class.StbSecRoll_BOX_Class>(),
                StbSecBuild_BOX = new List<StbSecSteel_Class.StbSecBuild_BOX_Class>(),
                StbSecPipe = new List<StbSecSteel_Class.StbSecPipe_Class>(),
                StbSecRoll_T = new List<StbSecSteel_Class.StbSecRoll_T_Class>(),
                StbSecRoll_C = new List<StbSecSteel_Class.StbSecRoll_C_Class>(),
                StbSecRoll_L = new List<StbSecSteel_Class.StbSecRoll_L_Class>(),
                StbSecRoll_LipC = new List<StbSecSteel_Class.StbSecRoll_LipC_Class>(),
                StbSecRoll_FB = new List<StbSecSteel_Class.StbSecRoll_FB_Class>(),
                StbSecRoll_Bar = new List<StbSecSteel_Class.StbSecRoll_Bar_Class>(),
            };
            stb.StbModel.StbSections.StbSecColumns_RC = new List<StbSecColumn_RC>();
            stb.StbModel.StbSections.StbSecColumns_S = new List<StbSecColumn_S>();
            stb.StbModel.StbSections.StbSecColumns_SRC = new List<StbSecColumn_SRC>();
            stb.StbModel.StbSections.StbSecColumns_CFT = new List<StbSecColumn_CFT>();

            stb.StbModel.StbSections.StbSecBeams_RC = new List<StbSecBeam_RC>();
            stb.StbModel.StbSections.StbSecBeams_S = new List<StbSecBeam_S>();
            stb.StbModel.StbSections.StbSecBeams_SRC = new List<StbSecBeam_SRC>();

            stb.StbModel.StbSections.StbSecBraces_S = new List<StbSecBrace_S>();

            stb.StbModel.StbSections.StbSecSlabs_RC = new List<StbSecSlab_RC>();
            stb.StbModel.StbSections.StbSecSlabs_Deck = new List<StbSecSlab_Deck>();

            stb.StbModel.StbSections.StbSecWalls_RC = new List<StbSecWall_RC>();

            stb.StbModel.StbSections.StbSecFoundations_RC = new List<StbSecFoundation_RC>();
            stb.StbModel.StbSections.StbSecPiles_RC = new List<StbSecPile_RC>();

            stb.StbModel.StbSections.StbSecOpens_RC = new List<StbSecOpen_RC>();
            stb.StbModel.StbSections.StbSecParapets_RC = new List<StbSecParapet_RC>();


            stb.StbModel.StbMembers.StbColumns = new List<StbColumn>();
            stb.StbModel.StbMembers.StbPosts = new List<StbPost>();
            stb.StbModel.StbMembers.StbGirders = new List<StbGirder>();
            stb.StbModel.StbMembers.StbBeams = new List<StbBeam>();
            stb.StbModel.StbMembers.StbBraces = new List<StbBrace>();
            stb.StbModel.StbMembers.StbSlabs = new List<StbSlab>();
            stb.StbModel.StbMembers.StbWalls = new List<StbWall>();
            stb.StbModel.StbMembers.StbFootings = new List<StbFooting>();
            stb.StbModel.StbMembers.StbStrip_Footings = new List<StbStrip_Footing>();
            stb.StbModel.StbMembers.StbPiles = new List<StbPile>();
            stb.StbModel.StbMembers.StbParapets = new List<StbParapet>();

            #endregion



            #region StbCommon

            stb.StbCommon.app_name = RevitLNK.formtitle + " " + RevitLNK.RevitVersion;

            ProjectInfo pinfo = Commons.doc.ProjectInformation;
            for (int i = 0; i < Data.projectParams.Count(); ++i)
            {
                Parameter p = pinfo.LookupParameter(Data.projectParams[i]);
                if (p == null) continue;

                switch (i)
                {
                    case 0:  //"STBファイル名"
                        break;
                    case 1:  //"STBファイル更新日時"
                        break;
                    case 2:  //"STBレベルマッピング設定"
                        break;
                    case 3:  //"STB基点位置設定"
                        break;
                    case 4:  //"STBコンクリート設定"
                        break;
                    case 5:  //"STB鉄骨設定"
                        break;
                    case 6:  //"STBグローバルID"
                        break;
                    case 7:  //"STBプロジェクト名"
                        stb.StbCommon.project_name = p.AsString();
                        break;
                    case 8:  //"STBアプリケーション名"
                        break;
                    case 9:  //"STB建物全体のコンクリート強度"
                        stb.StbCommon.concrete_strength = p.AsString();
                        break;
                    case 10: //"STB鉄骨規格"
                        break;
                    case 11: //"STB径別鉄筋強度情報"
                        if (p.AsString() != null)
                        {
                            string[] Reinforcement = p.AsString().Split(',');
                            for (int j = 0; j < Reinforcement.Length - 1; j += 2)
                            {
                                stb.StbCommon.StbReinforcement_Strength_List.Add(new StbReinforcement_Strength() { D = Reinforcement[j].Trim(), SD = Reinforcement[j + 1].Trim() });
                            }
                        }
                        break;
                }
            }

            if (stb.StbCommon.project_name == "")
            {
                stb.StbCommon.project_name = Path.GetFileNameWithoutExtension(Commons.doc.PathName);
            }


            #endregion


            id = 0;
            GridInfo = new List<GridInformation>[2];
            GridInfo[0] = new List<GridInformation>();
            GridInfo[1] = new List<GridInformation>();

            Export_Grid();
            Export_Level();


            id = 0;
            id_sect = 0;
            Export_Column();
            Export_Girder(StructuralInstanceUsage.Girder);
            Export_Girder(StructuralInstanceUsage.Joist);

            Export_Wall();
            Export_Slab();
            Export_Brace();
            Export_Footing();

            SetNodeKind();

            stb.WriteFile(savepath);
        }










        /// <summary>
        /// 節点番号の取得と登録
        /// </summary>
        /// <param name="p">座標[mm]</param>
        /// <returns>stbの節点番号</returns>
        private static int GetNodeId(XYZ p)
        {
            for (int i = 0; i < stb.StbModel.StbNodes.Count; ++i)
            {
                XYZ n = new XYZ(stb.StbModel.StbNodes[i].x, stb.StbModel.StbNodes[i].y, stb.StbModel.StbNodes[i].z);
                if (n.DistanceTo(p) < 0.001)
                {
                    return stb.StbModel.StbNodes[i].id;
                }
            }

            StbNode node = new StbNode()
            {
                id = stb.StbModel.StbNodes.Count + 1,
                x = p.X,
                y = p.Y,
                z = p.Z,
                kind = "OTHER"
            };

            XYZ p2 = new XYZ(p.X, p.Y, 0);

            int count = 0;

            for (int xy = 0; xy <= 1; ++xy)
            {
                for (int g = 0; g < GridInfo[xy].Count; ++g)
                {
                    XYZ ps = GridInfo[xy][g].ps;
                    XYZ pe = GridInfo[xy][g].pe;

                    XYZ v1 = (ps - p2).Normalize();
                    XYZ v2 = (pe - p2).Normalize();

                    if (ps.DistanceTo(p2) < 0.001 ||
                        pe.DistanceTo(p2) < 0.001 ||
                        Math.Abs(v1.DotProduct(v2) + 1) < 0.001)
                    {
                        if (xy == 0)
                        {
                            int index = stb.StbModel.StbAxes.StbX_Axis.FindIndex(x => x.id == GridInfo[xy][g].stb_id);
                            if (index >= 0)
                            {
                                if (!stb.StbModel.StbAxes.StbX_Axis[index].StbNodeid_List.Any(x => x.id == node.id))
                                {
                                    stb.StbModel.StbAxes.StbX_Axis[index].StbNodeid_List.Add(new StbNodeid() { id = node.id });
                                    //node.kind = "ON_GRID";
                                    count++;
                                }
                                break;
                            }
                        }
                        else
                        {
                            int index = stb.StbModel.StbAxes.StbY_Axis.FindIndex(x => x.id == GridInfo[xy][g].stb_id);
                            if (index >= 0)
                            {
                                if (!stb.StbModel.StbAxes.StbY_Axis[index].StbNodeid_List.Any(x => x.id == node.id))
                                {
                                    stb.StbModel.StbAxes.StbY_Axis[index].StbNodeid_List.Add(new StbNodeid() { id = node.id });
                                    //node.kind = "ON_GRID";
                                    count++;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (count >= 2)
            {
                //XとYの2方向に所属している必要がある
                node.kind = "ON_GRID";
            }


            bool add = false;
            for (int i = 0; i < stb.StbModel.StbStories.Count; ++i)
            {
                if (Math.Abs(stb.StbModel.StbStories[i].height - p.Z) < 0.001)
                {
                    stb.StbModel.StbStories[i].StbNodeid_List.Add(new StbNodeid() { id = node.id });
                    add = true;
                    break;
                }
            }
            if (!add)
            {
                double min = stb.StbModel.StbStories.Min(x => Math.Abs(x.height - p.Z));
                int index = stb.StbModel.StbStories.FindIndex(x => Math.Abs(Math.Abs(x.height - p.Z) - min) < 0.001);
                if (index >= 0)
                {
                    stb.StbModel.StbStories[index].StbNodeid_List.Add(new StbNodeid() { id = node.id });
                }
            }


            stb.StbModel.StbNodes.Add(node);

            return node.id;
        }

        /// <summary>
        /// StbNode.kind の設定
        /// </summary>
        private static void SetNodeKind()
        {
            foreach (var n in stb.StbModel.StbNodes.Where(a => a.kind == "OTHER"))
            {
                XYZ p = new XYZ(n.x, n.y, n.z);

                bool isCanti = false;
                int canti_id = 0;

                foreach (var b in stb.StbModel.StbMembers.StbGirders)
                {
                    if (n.id == b.idNode_start || n.id == b.idNode_end)
                    {
                        if (b.kind_structure == "RC")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_RC.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }
                        else if (b.kind_structure == "S")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_S.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }
                        else if (b.kind_structure == "SRC")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_SRC.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }

                        if (isCanti)
                        {
                            canti_id = b.id;
                        }

                        continue;
                    }

                    var n1 = stb.StbModel.StbNodes.Find(a => a.id == b.idNode_start);
                    var n2 = stb.StbModel.StbNodes.Find(a => a.id == b.idNode_end);
                    XYZ p1 = new XYZ(n1.x, n1.y, n1.z);
                    XYZ p2 = new XYZ(n2.x, n2.y, n2.z);

                    if (Math.Abs(p.DistanceTo(p1) + p.DistanceTo(p2) - p1.DistanceTo(p2)) < 1)
                    {
                        n.kind = "ON_BEAM";
                        n.id_member = b.id;
                        break;
                    }
                }
                if (isCanti)
                {
                    n.kind = "ON_CANTI";
                    n.id_member = canti_id;
                }
                if (n.kind != "OTHER") continue;



                foreach (var b in stb.StbModel.StbMembers.StbBeams)
                {
                    if (n.id == b.idNode_start || n.id == b.idNode_end)
                    {
                        if (b.kind_structure == "RC")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_RC.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }
                        else if (b.kind_structure == "S")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_S.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }
                        else if (b.kind_structure == "SRC")
                        {
                            var sec = stb.StbModel.StbSections.StbSecBeams_SRC.Find(a => a.id == b.id_section);
                            isCanti = sec.isCanti;
                        }

                        if (isCanti)
                        {
                            canti_id = b.id;
                        }

                        continue;
                    }

                    var n1 = stb.StbModel.StbNodes.Find(a => a.id == b.idNode_start);
                    var n2 = stb.StbModel.StbNodes.Find(a => a.id == b.idNode_end);
                    XYZ p1 = new XYZ(n1.x, n1.y, n1.z);
                    XYZ p2 = new XYZ(n2.x, n2.y, n2.z);

                    if (Math.Abs(p.DistanceTo(p1) + p.DistanceTo(p2) - p1.DistanceTo(p2)) < 1)
                    {
                        n.kind = "ON_BEAM";
                        n.id_member = b.id;
                        break;
                    }
                }
                if (isCanti)
                {
                    n.kind = "ON_CANTI";
                    n.id_member = canti_id;
                }
                if (n.kind != "OTHER") continue;




                foreach (var c in stb.StbModel.StbMembers.StbColumns)
                {
                    if (n.id == c.idNode_bottom || n.id == c.idNode_top)
                    {
                        continue;
                    }

                    var n1 = stb.StbModel.StbNodes.Find(a => a.id == c.idNode_bottom);
                    var n2 = stb.StbModel.StbNodes.Find(a => a.id == c.idNode_top);
                    XYZ p1 = new XYZ(n1.x, n1.y, n1.z);
                    XYZ p2 = new XYZ(n2.x, n2.y, n2.z);

                    if (Math.Abs(p.DistanceTo(p1) + p.DistanceTo(p2) - p1.DistanceTo(p2)) < 1)
                    {
                        n.kind = "ON_COLUMN";
                        n.id_member = c.id;
                        break;
                    }
                }
                if (n.kind != "OTHER") continue;



                foreach (var c in stb.StbModel.StbMembers.StbPosts)
                {
                    if (n.id == c.idNode_bottom || n.id == c.idNode_top)
                    {
                        continue;
                    }

                    var n1 = stb.StbModel.StbNodes.Find(a => a.id == c.idNode_bottom);
                    var n2 = stb.StbModel.StbNodes.Find(a => a.id == c.idNode_top);
                    XYZ p1 = new XYZ(n1.x, n1.y, n1.z);
                    XYZ p2 = new XYZ(n2.x, n2.y, n2.z);

                    if (Math.Abs(p.DistanceTo(p1) + p.DistanceTo(p2) - p1.DistanceTo(p2)) < 1)
                    {
                        n.kind = "ON_COLUMN";
                        n.id_member = c.id;
                        break;
                    }
                }
                if (n.kind != "OTHER") continue;



                foreach (var s in stb.StbModel.StbMembers.StbSlabs)
                {
                    var nodes = s.StbNodeid_List.Select(a => a.id);

                    if (nodes.Contains(n.id))
                    {
                        if (s.kind_slab == "CANTI")
                        {
                            n.kind = "ON_CANTI";
                            n.id_member = s.id;
                            break;
                        }
                    }

                    var points = new List<XYZ>();
                    foreach (var nodeID in nodes)
                    {
                        var n1 = stb.StbModel.StbNodes.Find(a => a.id == nodeID);
                        XYZ p1 = new XYZ(n1.x, n1.y, n1.z);
                        points.Add(p1);
                    }

                    //同一平面上チェック
                    XYZ v1 = (points[1] - points[0]).Normalize();
                    XYZ v2 = (points[2] - points[0]).Normalize();
                    XYZ normal = v1.CrossProduct(v2).Normalize();
                    if (Math.Abs(normal.DotProduct(p - points[0])) < 0.001)
                    {
                        if (Commons.IntoRegion(points, p) >= 0)
                        {
                            n.kind = "ON_SLAB";
                            n.id_member = s.id;
                            break;
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
        private static string GetSteelName(FamilySymbol symbol, int shape2 = 0, int LCR = 0)
        {
            const string format = "0";
            const string mark1 = "-";
            const string mark2 = "*";


            string familyname = symbol.Family.Name;

            if (familyname == SetFamily.SClmH.FamilyName        ||
                familyname == SetFamily.SRCClmH.FamilyName      ||
                familyname == SetFamily.SRCClmCross.FamilyName  ||
                (familyname == SetFamily.SRCClmT.FamilyName && shape2 == 0) ||
                familyname == SetFamily.SRCClmH_Rou.FamilyName  ||
                familyname == SetFamily.SRCClmCross_Rou.FamilyName ||
                (familyname == SetFamily.SRCClmT_Rou.FamilyName && shape2 == 0) ||
                familyname == SetFamily.SBraH.FamilyName ||
                familyname == SetFamily.SGirH.FamilyName ||
                familyname == SetFamily.SGirH_Haunch.FamilyName ||
                familyname == SetFamily.SBeamH.FamilyName ||
                familyname == SetFamily.SBeamH_Haunch.FamilyName ||
                familyname == SetFamily.SRCGirH.FamilyName ||
                familyname == SetFamily.SRCBeamH.FamilyName ||
                familyname == SetFamily.SCGirH.FamilyName ||
                familyname == SetFamily.SCGirBH.FamilyName ||
                familyname == SetFamily.SCBeamBH.FamilyName ||
                familyname == SetFamily.SCBeamH.FamilyName ||
                familyname == SetFamily.SRCCGirH.FamilyName ||
                familyname == SetFamily.SRCCBeamH.FamilyName)
            {
                #region H

                StbSecSteel_Class.StbSecRoll_H_Class steel = new StbSecSteel_Class.StbSecRoll_H_Class();

                if (familyname == SetFamily.SClmH.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SClmH.type);
                    steel.A = GetParameter_double(symbol, SetFamily.SClmH.A);
                    steel.B = GetParameter_double(symbol, SetFamily.SClmH.B);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SClmH.t1);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SClmH.t2);
                    steel.r = GetParameter_double(symbol, SetFamily.SClmH.r);
                }
                else if (familyname == SetFamily.SRCClmH.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SRCClmH.type);
                    if (GetParameter_string(symbol, SetFamily.SRCClmH.direction_type) == "H")
                    {
                        steel.A = GetParameter_double(symbol, SetFamily.SRCClmH.H);
                        steel.B = GetParameter_double(symbol, SetFamily.SRCClmH.B);
                    }
                    else
                    {
                        steel.A = GetParameter_double(symbol, SetFamily.SRCClmH.B);
                        steel.B = GetParameter_double(symbol, SetFamily.SRCClmH.H);
                    }
                    steel.t1 = GetParameter_double(symbol, SetFamily.SRCClmH.t1);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SRCClmH.t2);
                    steel.r = GetParameter_double(symbol, SetFamily.SRCClmH.r);
                }
                else if (familyname == SetFamily.SRCClmCross.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.type_X : SetFamily.SRCClmCross.type_Y));
                    steel.A     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.XH     : SetFamily.SRCClmCross.YH    ));
                    steel.B     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.XB     : SetFamily.SRCClmCross.YB    ));
                    steel.t1    = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.Xt1    : SetFamily.SRCClmCross.Yt1   ));
                    steel.t2    = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.Xt2    : SetFamily.SRCClmCross.Yt2   ));
                    steel.r     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross.Xr     : SetFamily.SRCClmCross.Yr    ));
                }
                else if (familyname == SetFamily.SRCClmT.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCClmT.type_H);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCClmT.H);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCClmT.B);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCClmT.t1);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCClmT.t2);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCClmT.r);
                }
                else if (familyname == SetFamily.SRCClmH_Rou.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCClmH_Rou.type);
                    if (GetParameter_string(symbol, SetFamily.SRCClmH_Rou.direction_type) == "H")
                    {
                        steel.A = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.H);
                        steel.B = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.B);
                    }
                    else
                    {
                        steel.A = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.B);
                        steel.B = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.H);
                    }
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.t1);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.t2);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.r);
                }
                else if (familyname == SetFamily.SRCClmCross_Rou.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.type_X : SetFamily.SRCClmCross_Rou.type_Y));
                    steel.A     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.XH     : SetFamily.SRCClmCross_Rou.YH    ));
                    steel.B     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.XB     : SetFamily.SRCClmCross_Rou.YB    ));
                    steel.t1    = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xt1    : SetFamily.SRCClmCross_Rou.Yt1   ));
                    steel.t2    = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xt2    : SetFamily.SRCClmCross_Rou.Yt2   ));
                    steel.r     = GetParameter_double(symbol, (shape2 == 0 ? SetFamily.SRCClmCross_Rou.Xr     : SetFamily.SRCClmCross_Rou.Yr    ));
                }
                else if (familyname == SetFamily.SRCClmT_Rou.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.type_H);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.H);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.B);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.t1);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.t2);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.r);
                }
                else if (familyname == SetFamily.SBraH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SBraH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SBraH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SBraH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SBraH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SBraH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SBraH.r[LCR]);
                }
                else if (familyname == SetFamily.SGirH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SGirH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SGirH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SGirH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SGirH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SGirH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SGirH.r[LCR]);
                }
                else if (familyname == SetFamily.SGirH_Haunch.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SGirH_Haunch.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SGirH_Haunch.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SGirH_Haunch.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SGirH_Haunch.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SGirH_Haunch.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SGirH_Haunch.r[LCR]);
                }
                else if (familyname == SetFamily.SBeamH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SBeamH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SBeamH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SBeamH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SBeamH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SBeamH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SBeamH.r[LCR]);
                }
                else if (familyname == SetFamily.SBeamH_Haunch.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SBeamH_Haunch.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SBeamH_Haunch.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SBeamH_Haunch.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SBeamH_Haunch.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SBeamH_Haunch.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SBeamH_Haunch.r[LCR]);
                }
                else if (familyname == SetFamily.SRCGirH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCGirH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCGirH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCGirH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCGirH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCGirH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCGirH.r[LCR]);
                }
                else if (familyname == SetFamily.SRCBeamH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCBeamH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCBeamH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCBeamH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCBeamH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCBeamH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCBeamH.r[LCR]);
                }
                else if (familyname == SetFamily.SCGirH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SCGirH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SCGirH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SCGirH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SCGirH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SCGirH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SCGirH.r[LCR]);
                }
                else if (familyname == SetFamily.SCGirBH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SCGirBH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SCGirBH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SCGirBH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SCGirBH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SCGirBH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SCGirBH.r[LCR]);
                }
                else if (familyname == SetFamily.SCBeamBH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SCBeamBH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SCBeamBH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SCBeamBH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SCBeamBH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SCBeamBH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SCBeamBH.r[LCR]);
                }
                else if (familyname == SetFamily.SCBeamH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SCBeamH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SCBeamH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SCBeamH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SCBeamH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SCBeamH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SCBeamH.r[LCR]);
                }
                else if (familyname == SetFamily.SRCCGirH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCCGirH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCCGirH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCCGirH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCCGirH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCCGirH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCCGirH.r[LCR]);
                }
                else if (familyname == SetFamily.SRCCBeamH.FamilyName)
                {
                    steel.type  = GetParameter_string(symbol, SetFamily.SRCCBeamH.type[LCR]);
                    steel.A     = GetParameter_double(symbol, SetFamily.SRCCBeamH.A[LCR]);
                    steel.B     = GetParameter_double(symbol, SetFamily.SRCCBeamH.B[LCR]);
                    steel.t1    = GetParameter_double(symbol, SetFamily.SRCCBeamH.t1[LCR]);
                    steel.t2    = GetParameter_double(symbol, SetFamily.SRCCBeamH.t2[LCR]);
                    steel.r     = GetParameter_double(symbol, SetFamily.SRCCBeamH.r[LCR]);
                }


                if (steel.r < 0.1)
                {
                    steel.name = "BH" + mark1 +
                                 steel.A.ToString(format) + mark2 +
                                 steel.B.ToString(format) + mark2 +
                                 steel.t1.ToString(format) + mark2 +
                                 steel.t2.ToString(format);

                    StbSecSteel_Class.StbSecBuild_H_Class steel2 = new StbSecSteel_Class.StbSecBuild_H_Class()
                    {
                        name = steel.name,
                        A = steel.A,
                        B = steel.B,
                        t1 = steel.t1,
                        t2 = steel.t2,
                    };

                    if (!stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H.Any(x => x.name == steel.name))
                    {
                        stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H.Add(steel2);
                    }
                }
                else
                {
                    steel.name = steel.type + mark1 +
                                 steel.A.ToString(format) + mark2 +
                                 steel.B.ToString(format) + mark2 +
                                 steel.t1.ToString(format) + mark2 +
                                 steel.t2.ToString(format) + mark2 +
                                 steel.r.ToString(format);

                    if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H.Any(x => x.name == steel.name))
                    {
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H.Add(steel);
                    }
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmBH.FamilyName ||
                     familyname == SetFamily.SBraBH.FamilyName ||
                     familyname == SetFamily.SGirBH.FamilyName ||
                     familyname == SetFamily.SBeamBH.FamilyName)
            {
                #region BH

                StbSecSteel_Class.StbSecBuild_H_Class steel = new StbSecSteel_Class.StbSecBuild_H_Class();
                if (familyname == SetFamily.SClmBH.FamilyName)
                {
                    steel.A  = GetParameter_double(symbol, SetFamily.SClmBH.A);
                    steel.B  = GetParameter_double(symbol, SetFamily.SClmBH.B);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SClmBH.t1);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SClmBH.t2);
                }
                else if (familyname == SetFamily.SBraBH.FamilyName)
                {
                    steel.A  = GetParameter_double(symbol, SetFamily.SBraBH.A[LCR]);
                    steel.B  = GetParameter_double(symbol, SetFamily.SBraBH.B[LCR]);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SBraBH.t1[LCR]);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SBraBH.t2[LCR]);
                }
                else if (familyname == SetFamily.SGirBH.FamilyName)
                {
                    steel.A  = GetParameter_double(symbol, SetFamily.SGirBH.A[LCR]);
                    steel.B  = GetParameter_double(symbol, SetFamily.SGirBH.B[LCR]);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SGirBH.t1[LCR]);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SGirBH.t2[LCR]);
                }
                else if (familyname == SetFamily.SBeamBH.FamilyName)
                {
                    steel.A  = GetParameter_double(symbol, SetFamily.SBeamBH.A[LCR]);
                    steel.B  = GetParameter_double(symbol, SetFamily.SBeamBH.B[LCR]);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SBeamBH.t1[LCR]);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SBeamBH.t2[LCR]);
                }

                steel.name = "BH" + mark1 +
                             steel.A.ToString(format) + mark2 +
                             steel.B.ToString(format) + mark2 +
                             steel.t1.ToString(format) + mark2 +
                             steel.t2.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmBox.FamilyName ||
                     familyname == SetFamily.CFTClmBox.FamilyName ||
                     familyname == SetFamily.SBraBox.FamilyName)
            {
                #region Box

                StbSecSteel_Class.StbSecRoll_BOX_Class steel = new StbSecSteel_Class.StbSecRoll_BOX_Class();

                if (familyname == SetFamily.SClmBox.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SClmBox.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.SClmBox.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SClmBox.B);
                    steel.t    = GetParameter_double(symbol, SetFamily.SClmBox.t1);
                    steel.R    = GetParameter_double(symbol, SetFamily.SClmBox.r);
                }
                else if (familyname == SetFamily.CFTClmBox.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.CFTClmBox.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.CFTClmBox.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.CFTClmBox.B);
                    steel.t    = GetParameter_double(symbol, SetFamily.CFTClmBox.t);
                    steel.R    = GetParameter_double(symbol, SetFamily.CFTClmBox.r1);
                }
                else if (familyname == SetFamily.SBraBox.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBraBox.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBraBox.H);
                    steel.B    = GetParameter_double(symbol, SetFamily.SBraBox.B);
                    steel.t    = GetParameter_double(symbol, SetFamily.SBraBox.t1);
                    steel.R    = GetParameter_double(symbol, SetFamily.SBraBox.r);
                }

                if (steel.type == "") steel.type = "ELSE";

                if (steel.R < 0.1)
                {
                    steel.name = "BB" + mark1 +
                                 steel.A.ToString(format) + mark2 +
                                 steel.B.ToString(format) + mark2 +
                                 steel.t.ToString(format) + mark2 +
                                 steel.t.ToString(format);

                    StbSecSteel_Class.StbSecBuild_BOX_Class steel2 = new StbSecSteel_Class.StbSecBuild_BOX_Class()
                    {
                        name = steel.name,
                        A = steel.A,
                        B = steel.B,
                        t1 = steel.t,
                        t2 = steel.t,
                    };

                    if (!stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX.Any(x => x.name == steel.name))
                    {
                        stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX.Add(steel2);
                    }
                }
                else
                {
                    steel.name = steel.type + mark1 +
                                 steel.A.ToString(format) + mark2 +
                                 steel.B.ToString(format) + mark2 +
                                 steel.t.ToString(format) + mark2 +
                                 steel.R.ToString(format);

                    if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_BOX.Any(x => x.name == steel.name))
                    {
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_BOX.Add(steel);
                    }
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmBBox.FamilyName ||
                     familyname == SetFamily.SBraBBox.FamilyName)
            {
                #region BBox

                StbSecSteel_Class.StbSecBuild_BOX_Class steel = new StbSecSteel_Class.StbSecBuild_BOX_Class();
                if (familyname == SetFamily.SClmBBox.FamilyName)
                {
                    steel.A    = GetParameter_double(symbol, SetFamily.SClmBBox.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SClmBBox.B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SClmBBox.t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SClmBBox.t2);
                }
                else if (familyname == SetFamily.SBraBBox.FamilyName)
                {
                    steel.A  = GetParameter_double(symbol, SetFamily.SBraBBox.H);
                    steel.B  = GetParameter_double(symbol, SetFamily.SBraBBox.B);
                    steel.t1 = GetParameter_double(symbol, SetFamily.SBraBBox.t1);
                    steel.t2 = GetParameter_double(symbol, SetFamily.SBraBBox.t2);
                }

                steel.name = "BB" + mark1 +
                             steel.A.ToString(format) + mark2 +
                             steel.B.ToString(format) + mark2 +
                             steel.t1.ToString(format) + mark2 +
                             steel.t2.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmPipe.FamilyName ||
                     familyname == SetFamily.CFTClmPipe.FamilyName ||
                     familyname == SetFamily.SBraPipe.FamilyName)
            {
                #region Pipe

                StbSecSteel_Class.StbSecPipe_Class steel = new StbSecSteel_Class.StbSecPipe_Class();

                if (familyname == SetFamily.SClmPipe.FamilyName)
                {
                    steel.D = GetParameter_double(symbol, SetFamily.SClmPipe.D);
                    steel.t = GetParameter_double(symbol, SetFamily.SClmPipe.t);
                }
                else if (familyname == SetFamily.CFTClmPipe.FamilyName)
                {
                    steel.D = GetParameter_double(symbol, SetFamily.CFTClmPipe.D);
                    steel.t = GetParameter_double(symbol, SetFamily.CFTClmPipe.t);
                }
                else if (familyname == SetFamily.SBraPipe.FamilyName)
                {
                    steel.D = GetParameter_double(symbol, SetFamily.SBraPipe.D);
                    steel.t = GetParameter_double(symbol, SetFamily.SBraPipe.t);
                }

                steel.name = "P" + mark1 +
                             steel.D.ToString(format) + mark2 +
                             steel.t.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecPipe.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecPipe.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmT.FamilyName ||
                     familyname == SetFamily.SRCClmT.FamilyName ||
                     familyname == SetFamily.SRCClmT_Rou.FamilyName)
            {
                #region T

                StbSecSteel_Class.StbSecRoll_T_Class steel = new StbSecSteel_Class.StbSecRoll_T_Class();

                if (familyname == SetFamily.SClmT.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SClmT.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.SClmT.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SClmT.B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SClmT.t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SClmT.t2);
                    steel.r    = GetParameter_double(symbol, SetFamily.SClmT.r);
                }
                else if (familyname == SetFamily.SRCClmT.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SRCClmT.type_T);
                    steel.A    = GetParameter_double(symbol, SetFamily.SRCClmT.CT_A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SRCClmT.CT_B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SRCClmT.CT_t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SRCClmT.CT_t2);
                    steel.r    = GetParameter_double(symbol, SetFamily.SRCClmT.CT_r);
                }
                else if (familyname == SetFamily.SRCClmT_Rou.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.type_T);
                    steel.A    = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.CT_A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.CT_B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.CT_t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.CT_t2);
                    steel.r    = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.CT_r);
                }

                steel.name = steel.type + mark1 +
                             steel.A.ToString(format) + mark2 +
                             steel.B.ToString(format) + mark2 +
                             steel.t1.ToString(format) + mark2 +
                             steel.t2.ToString(format) + mark2 +
                             steel.r.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_T.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_T.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmC.FamilyName ||
                     familyname == SetFamily.SBraC.FamilyName ||
                     familyname == SetFamily.SGirC.FamilyName ||
                     familyname == SetFamily.SBeamC.FamilyName ||
                     familyname == SetFamily.SCGirC.FamilyName ||
                     familyname == SetFamily.SCBeamC.FamilyName)
            {
                #region C

                StbSecSteel_Class.StbSecRoll_C_Class steel = new StbSecSteel_Class.StbSecRoll_C_Class();
                if (familyname == SetFamily.SClmC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SClmC.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.SClmC.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SClmC.B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SClmC.t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SClmC.t2);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SClmC.r1);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SClmC.r2);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SClmC.side);
                }
                else if (familyname == SetFamily.SBraC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBraC.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBraC.H[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SBraC.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SBraC.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SBraC.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SBraC.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SBraC.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SBraC.side[LCR]);
                }
                else if (familyname == SetFamily.SGirC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SGirC.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SGirC.H[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SGirC.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SGirC.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SGirC.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SGirC.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SGirC.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SGirC.side[LCR]);
                }
                else if (familyname == SetFamily.SBeamC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBeamC.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBeamC.H[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SBeamC.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SBeamC.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SBeamC.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SBeamC.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SBeamC.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SBeamC.side[LCR]);
                }
                else if (familyname == SetFamily.SCGirC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCGirC.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCGirC.H[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SCGirC.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SCGirC.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SCGirC.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SCGirC.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SCGirC.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCGirC.side[LCR]);
                }
                else if (familyname == SetFamily.SCBeamC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCBeamC.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCBeamC.H[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SCBeamC.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SCBeamC.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SCBeamC.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SCBeamC.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SCBeamC.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCBeamC.side[LCR]);
                }

                if (steel.type == "") steel.type = (steel.side ? "2C" : "C");

                steel.name = steel.type + mark1 +
                             steel.A.ToString(format) + mark2 +
                             steel.B.ToString(format) + mark2 +
                             steel.t1.ToString(format) + mark2 +
                             steel.t2.ToString(format) + mark2 +
                             steel.r1.ToString(format) + mark2 +
                             steel.r2.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SClmL.FamilyName ||
                     familyname == SetFamily.SBraL.FamilyName ||
                     familyname == SetFamily.SGirL.FamilyName ||
                     familyname == SetFamily.SBeamL.FamilyName ||
                     familyname == SetFamily.SCGirL.FamilyName ||
                     familyname == SetFamily.SCBeamL.FamilyName)
            {
                #region L

                StbSecSteel_Class.StbSecRoll_L_Class steel = new StbSecSteel_Class.StbSecRoll_L_Class();
                if (familyname == SetFamily.SClmL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SClmL.type);
                    steel.A    = GetParameter_double(symbol, SetFamily.SClmL.A);
                    steel.B    = GetParameter_double(symbol, SetFamily.SClmL.B);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SClmL.t1);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SClmL.t2);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SClmL.r1);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SClmL.r2);
                    steel.side = GetParameter_bool(symbol, SetFamily.SClmL.side);
                }
                else if (familyname == SetFamily.SBraL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBraL.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBraL.A[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SBraL.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SBraL.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SBraL.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SBraL.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SBraL.r2[LCR]);
                    steel.side = GetParameter_bool(symbol, SetFamily.SBraL.side[LCR]);
                }
                else if (familyname == SetFamily.SGirL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SGirL.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SGirL.A[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SGirL.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SGirL.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SGirL.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SGirL.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SGirL.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SGirL.side[LCR]);
                }
                else if (familyname == SetFamily.SBeamL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBeamL.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBeamL.A[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SBeamL.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SBeamL.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SBeamL.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SBeamL.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SBeamL.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SBeamL.side[LCR]);
                }
                else if (familyname == SetFamily.SCGirL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCGirL.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCGirL.A[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SCGirL.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SCGirL.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SCGirL.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SCGirL.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SCGirL.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCGirL.side[LCR]);
                }
                else if (familyname == SetFamily.SCBeamL.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCBeamL.type[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCBeamL.A[LCR]);
                    steel.B    = GetParameter_double(symbol, SetFamily.SCBeamL.B[LCR]);
                    steel.t1   = GetParameter_double(symbol, SetFamily.SCBeamL.t1[LCR]);
                    steel.t2   = GetParameter_double(symbol, SetFamily.SCBeamL.t2[LCR]);
                    steel.r1   = GetParameter_double(symbol, SetFamily.SCBeamL.r1[LCR]);
                    steel.r2   = GetParameter_double(symbol, SetFamily.SCBeamL.r2[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCBeamL.side[LCR]);
                }

                if (steel.type == "") steel.type = (steel.side ? "2L" : "L");

                steel.name = steel.type + mark1 +
                             steel.A.ToString(format) + mark2 +
                             steel.B.ToString(format) + mark2 +
                             steel.t1.ToString(format) + mark2 +
                             steel.t2.ToString(format) + mark2 +
                             steel.r1.ToString(format) + mark2 +
                             steel.r2.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L.Add(steel);
                }

                return steel.name;

                #endregion
            }
            else if (familyname == SetFamily.SBraLipC.FamilyName ||
                     familyname == SetFamily.SGirLipC.FamilyName ||
                     familyname == SetFamily.SBeamLipC.FamilyName ||
                     familyname == SetFamily.SCGirLipC.FamilyName ||
                     familyname == SetFamily.SCBeamLipC.FamilyName)
            {
                #region LipC

                StbSecSteel_Class.StbSecRoll_LipC_Class steel = new StbSecSteel_Class.StbSecRoll_LipC_Class();
                if (familyname == SetFamily.SBraLipC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBraLipC.type[LCR]);
                    steel.H    = GetParameter_double(symbol, SetFamily.SBraLipC.H[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBraLipC.A[LCR]);
                    steel.C    = GetParameter_double(symbol, SetFamily.SBraLipC.C[LCR]);
                    steel.t    = GetParameter_double(symbol, SetFamily.SBraLipC.t[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SBraLipC.side[LCR]);
                }
                else if (familyname == SetFamily.SGirLipC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SGirLipC.type[LCR]);
                    steel.H    = GetParameter_double(symbol, SetFamily.SGirLipC.H[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SGirLipC.A[LCR]);
                    steel.C    = GetParameter_double(symbol, SetFamily.SGirLipC.C[LCR]);
                    steel.t    = GetParameter_double(symbol, SetFamily.SGirLipC.t[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SGirLipC.side[LCR]);
                }
                else if (familyname == SetFamily.SBeamLipC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SBeamLipC.type[LCR]);
                    steel.H    = GetParameter_double(symbol, SetFamily.SBeamLipC.H[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SBeamLipC.A[LCR]);
                    steel.C    = GetParameter_double(symbol, SetFamily.SBeamLipC.C[LCR]);
                    steel.t    = GetParameter_double(symbol, SetFamily.SBeamLipC.t[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SBeamLipC.side[LCR]);
                }
                else if (familyname == SetFamily.SCGirLipC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCGirLipC.type[LCR]);
                    steel.H    = GetParameter_double(symbol, SetFamily.SCGirLipC.H[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCGirLipC.A[LCR]);
                    steel.C    = GetParameter_double(symbol, SetFamily.SCGirLipC.C[LCR]);
                    steel.t    = GetParameter_double(symbol, SetFamily.SCGirLipC.t[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCGirLipC.side[LCR]);
                }
                else if (familyname == SetFamily.SCBeamLipC.FamilyName)
                {
                    steel.type = GetParameter_string(symbol, SetFamily.SCBeamLipC.type[LCR]);
                    steel.H    = GetParameter_double(symbol, SetFamily.SCBeamLipC.H[LCR]);
                    steel.A    = GetParameter_double(symbol, SetFamily.SCBeamLipC.A[LCR]);
                    steel.C    = GetParameter_double(symbol, SetFamily.SCBeamLipC.C[LCR]);
                    steel.t    = GetParameter_double(symbol, SetFamily.SCBeamLipC.t[LCR]);
                    steel.side = GetParameter_bool  (symbol, SetFamily.SCBeamLipC.side[LCR]);
                }


                if (steel.type == "") steel.type = (steel.side ? "2C" : "C");

                steel.name = steel.type + mark1 +
                             steel.H.ToString(format) + mark2 +
                             steel.A.ToString(format) + mark2 +
                             steel.C.ToString(format) + mark2 +
                             steel.t.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_LipC.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_LipC.Add(steel);
                }

                return steel.name;
                
                #endregion
            }
            else if (familyname == SetFamily.SBraFB.FamilyName)
            {
                #region FB

                StbSecSteel_Class.StbSecRoll_FB_Class steel = new StbSecSteel_Class.StbSecRoll_FB_Class();
                if (familyname == SetFamily.SBraFB.FamilyName)
                {
                    steel.B = GetParameter_double(symbol, SetFamily.SBraFB.B);
                    steel.t = GetParameter_double(symbol, SetFamily.SBraFB.t);
                }

                steel.name = "FB" + mark1 +
                             steel.B.ToString(format) + mark2 +
                             steel.t.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_FB.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_FB.Add(steel);
                }

                return steel.name;
                
                #endregion
            }
            else if (familyname == SetFamily.SBraRollBar.FamilyName)
            {
                #region Bar

                StbSecSteel_Class.StbSecRoll_Bar_Class steel = new StbSecSteel_Class.StbSecRoll_Bar_Class();
                if (familyname == SetFamily.SBraRollBar.FamilyName)
                {
                    steel.R = GetParameter_double(symbol, SetFamily.SBraRollBar.D);
                }

                steel.name = "R" + mark1 +
                             steel.R.ToString(format);

                if (!stb.StbModel.StbSections.StbSecSteel.StbSecRoll_Bar.Any(x => x.name == steel.name))
                {
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_Bar.Add(steel);
                }

                return steel.name;

                #endregion
            }

            return "";
        }










        /// <summary>
        /// 通り芯の出力
        /// </summary>
        private static void Export_Grid()
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_GridChains);
            List<MultiSegmentGrid> multiGrids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<MultiSegmentGrid>().ToList();

            //collector = new FilteredElementCollector(Commons.doc);
            //filter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            //Grids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Grid>().ToList();

            for (int m = 0; m < multiGrids.Count; ++m)
            {
                ElementId gridid = multiGrids[m].GetGridIds().FirstOrDefault();
                if (gridid != null)
                {
                    if (Commons.doc.GetElement(gridid) is Grid gr)
                    {
                        if (gr.Curve is Line ln)
                        {
                            double angle = XYZ.BasisX.AngleOnPlaneTo(ln.Direction, XYZ.BasisZ);
                            int xy = 0; //0:X軸, 1:Y軸
                            if (angle <= Math.PI / 4)
                            {
                                xy = 1;
                            }
                            else if (angle <= Math.PI / 4 * 3)
                            {
                                xy = 0;
                            }
                            else if (angle <= Math.PI / 4 * 5)
                            {
                                xy = 1;
                            }
                            else if (angle <= Math.PI / 4 * 7)
                            {
                                xy = 0;
                            }
                            else
                            {
                                xy = 1;
                            }

                            id++;
                            XYZ p = Commons.ft2mm(ln.GetEndPoint(0));

                            StbAxesClass.Stb_Axis a = new StbAxesClass.Stb_Axis()
                            {
                                id = id,
                                name = gr.Name,
                                distance = (xy == 0 ? p.X : p.Y),
                                StbNodeid_List = new List<StbNodeid>()
                            };

                            if (xy == 0)
                            {
                                stb.StbModel.StbAxes.StbX_Axis.Add(a);
                            }
                            else
                            {
                                stb.StbModel.StbAxes.StbY_Axis.Add(a);
                            }

                            AddLog(LogCode.grid, multiGrids[m], id, 0);

                            foreach (ElementId eid in multiGrids[m].GetGridIds())
                            {
                                gr = Commons.doc.GetElement(eid) as Grid;

                                GridInformation gi = new GridInformation()
                                {
                                    stb_id = id,
                                    gr = gr,
                                    multiGridID = multiGrids[m].Id,
                                    ps = Commons.ft2mm(gr.Curve.GetEndPoint(0)),
                                    pe = Commons.ft2mm(gr.Curve.GetEndPoint(1)),
                                };

                                gi.ps = new XYZ(gi.ps.X, gi.ps.Y, 0);
                                gi.pe = new XYZ(gi.pe.X, gi.pe.Y, 0);

                                GridInfo[xy].Add(gi);
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
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
            Levels = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Level>().OrderBy(x => x.Elevation).ToList();

            ////"GL"が同じ高さにあれば除外
            //int GL = Levels.FindIndex(x => x.Name.Trim() == "GL");
            //if (GL >= 0)
            //{
            //    if (GL >= 1 && Math.Abs(Levels[GL].Elevation - Levels[GL - 1].Elevation) < 0.0001)
            //    {
            //        Levels.RemoveAt(GL);
            //    }
            //    else if (GL < Levels.Count - 1 && Math.Abs(Levels[GL].Elevation - Levels[GL + 1].Elevation) < 0.0001)
            //    {
            //        Levels.RemoveAt(GL);
            //    }
            //}

            ////GLは除外
            Level GL = Levels.Find(x => x.Name == "GL");
            //Levels.Remove(GL);

            for (int L = 0; L < Levels.Count; ++L)
            {
                //GLは除外
                if (Levels[L].Name == "GL") continue;

                id++;
                StbStory s = new StbStory()
                {
                    id = id,
                    name = Levels[L].Name,
                    height = Commons.ft2mm(Levels[L].Elevation),
                    kind = "GENERAL",
                    concrete_strength = stb.StbCommon.concrete_strength,
                    StbNodeid_List = new List<StbNodeid>(),
                };

                if (L == Levels.Count - 1)
                {
                    s.kind = "ROOF";
                }
                else if (Levels[L].Elevation - (GL?.Elevation ?? 0) < -0.00001)
                {
                    s.kind = "BASEMENT";
                }


                stb.StbModel.StbStories.Add(s);

                AddLog(LogCode.level, Levels[L], id, 0);
            }

        }




        /// <summary>
        /// 柱脚情報の取得
        /// </summary>
        /// <param name="eid">柱ID</param>
        /// <param name="ps">柱の座標</param>
        /// <returns></returns>
        private static StbSecColumn_S.StbSecBaseProductClass GetBaseProduct(ElementId eid, XYZ ps)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            XYZ p0 = Commons.mm2ft(ps);
            XYZ range = new XYZ(0.05, 0.05, 0.05);
            XYZ p1 = p0 - range;
            XYZ p2 = p0 + range;
            BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(new Outline(p1, p2));
            List<FamilyInstance> BaseProduct = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => x.Id != eid).ToList();

            for (int i = 0; i < BaseProduct.Count; ++i)
            {
                StbSecColumn_S.StbSecBaseProductClass bp = new StbSecColumn_S.StbSecBaseProductClass()
                {
                    product_company = GetParameter_string(BaseProduct[i].Symbol, BuiltInParameter.ALL_MODEL_MANUFACTURER),
                    product_code = BaseProduct[i].Symbol.Name,
                };

                if (bp.product_company != "")
                {
                    return bp;
                }
            }

            return null;
        }

        /// <summary>
        /// 柱断面の出力
        /// </summary>
        /// <param name="ins"></param>
        /// <returns>id_section</returns>
        private static int Export_SecColumn(FamilyInstance ins, XYZ ps)
        {
            FamilySymbol symbol = ins.Symbol;
            string floor = Levels.Find(x => x.Id == ins.LevelId).Name;

            int retID = -1;

            string familyname = symbol.Family.Name;
            if (familyname == SetFamily.RCClmRe.FamilyName ||
                familyname == SetFamily.RCClmRo.FamilyName)
            {
                id_sect++;

                #region RC柱

                StbSecColumn_RC s = new StbSecColumn_RC()
                {
                    id = id_sect,
                    floor = floor,
                };

                string[] paramName = new string[23];
                if (familyname == SetFamily.RCClmRe.FamilyName)
                {
                    #region RC角柱

                    paramName[ 0] = SetFamily.RCClmRe.name;
                    paramName[ 1] = SetFamily.RCClmRe.kind_column;
                    paramName[ 2] = SetFamily.RCClmRe.D_reinforcement_main[0];      //柱脚太筋径
                    paramName[ 3] = SetFamily.RCClmRe.D_reinforcement_2nd_main[0];  //柱脚細筋径
                    paramName[ 4] = SetFamily.RCClmRe.D_reinforcement_axial;
                    paramName[ 5] = SetFamily.RCClmRe.D_reinforcement_band[0];      //柱脚
                    paramName[ 6] = SetFamily.RCClmRe.D_bar_spacing;
                    paramName[ 7] = SetFamily.RCClmRe.strength_concrete;
                    paramName[ 8] = SetFamily.RCClmRe.strength_reinforcement_main;
                    paramName[ 9] = SetFamily.RCClmRe.strength_reinforcement_2nd_main;
                    paramName[10] = SetFamily.RCClmRe.strength_reinforcement_axial;
                    paramName[11] = SetFamily.RCClmRe.strength_reinforcement_band;
                    paramName[12] = SetFamily.RCClmRe.strength_bar_spacing;
                    paramName[13] = SetFamily.RCClmRe.depth_cover_X[0]; //始
                    paramName[14] = SetFamily.RCClmRe.depth_cover_X[1]; //終
                    paramName[15] = SetFamily.RCClmRe.depth_cover_Y[0]; //始
                    paramName[16] = SetFamily.RCClmRe.depth_cover_Y[1]; //終
                    paramName[17] = SetFamily.RCClmRe.kind_reinforcement_corner[0]; //柱脚
                    paramName[18] = SetFamily.RCClmRe.interval_reinforcement;
                    paramName[19] = SetFamily.RCClmRe.center_reinforcement_start_X;
                    paramName[20] = SetFamily.RCClmRe.center_reinforcement_end_X;
                    paramName[21] = SetFamily.RCClmRe.center_reinforcement_start_Y;
                    paramName[22] = SetFamily.RCClmRe.center_reinforcement_end_Y;

                    //形状
                    s.StbSecFigure = new StbSecColumn_RC.StbSecFigureClass()
                    {
                        StbSecFigureType = 1,
                        StbSecRect = new StbSecColumn_RC.StbSecFigureClass.StbSecRectClass(),
                    };

                    s.StbSecFigure.StbSecRect.DX = GetParameter_double(symbol, SetFamily.RCClmRe.DX);
                    s.StbSecFigure.StbSecRect.DY = GetParameter_double(symbol, SetFamily.RCClmRe.DY);


                    //配筋
                    StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[] bar = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[2];
                    for (int b = 0; b < bar.Length; ++b)
                    {
                        bar[b] = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass
                        {
                            pos = (b == 0 ? "BASE" : "TOP"),
                            count_main_X_1st = GetParameter_int(symbol, SetFamily.RCClmRe.count_main_X_1st[b]),
                            count_main_X_2nd = GetParameter_int(symbol, SetFamily.RCClmRe.count_main_X_2nd[b]),
                            count_main_Y_1st = GetParameter_int(symbol, SetFamily.RCClmRe.count_main_Y_1st[b]),
                            count_main_Y_2nd = GetParameter_int(symbol, SetFamily.RCClmRe.count_main_Y_2nd[b]),
                            count_2nd_main_X_1st = GetParameter_int(symbol, SetFamily.RCClmRe.count_2nd_main_X_1st[b]),
                            count_2nd_main_X_2nd = GetParameter_int(symbol, SetFamily.RCClmRe.count_2nd_main_X_2nd[b]),
                            count_2nd_main_Y_1st = GetParameter_int(symbol, SetFamily.RCClmRe.count_2nd_main_Y_1st[b]),
                            count_2nd_main_Y_2nd = GetParameter_int(symbol, SetFamily.RCClmRe.count_2nd_main_Y_2nd[b]),
                            count_main_total = GetParameter_int(symbol, SetFamily.RCClmRe.count_main_total),
                            count_axial = GetParameter_int(symbol, SetFamily.RCClmRe.count_axial[b]),
                            pitch_band = GetParameter_double(symbol, SetFamily.RCClmRe.pitch_band[b]),
                            count_band_dir_X = GetParameter_int(symbol, SetFamily.RCClmRe.count_band_dir_X[b]),
                            count_band_dir_Y = GetParameter_int(symbol, SetFamily.RCClmRe.count_band_dir_Y[b]),
                            pitch_bar_spacing = GetParameter_double(symbol, SetFamily.RCClmRe.pitch_bar_spacing[b]),
                            count_bar_spacing_X = GetParameter_int(symbol, SetFamily.RCClmRe.count_bar_spacing_X[b]),
                            count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.RCClmRe.count_bar_spacing_Y[b])
                        };
                    }

                    bool isSame = true;
                    isSame &= (bar[0].count_main_X_1st == bar[1].count_main_X_1st);
                    isSame &= (bar[0].count_main_X_2nd == bar[1].count_main_X_2nd);
                    isSame &= (bar[0].count_main_Y_1st == bar[1].count_main_Y_1st);
                    isSame &= (bar[0].count_main_Y_2nd == bar[1].count_main_Y_2nd);
                    isSame &= (bar[0].count_2nd_main_X_1st == bar[1].count_2nd_main_X_1st);
                    isSame &= (bar[0].count_2nd_main_X_2nd == bar[1].count_2nd_main_X_2nd);
                    isSame &= (bar[0].count_2nd_main_Y_1st == bar[1].count_2nd_main_Y_1st);
                    isSame &= (bar[0].count_2nd_main_Y_2nd == bar[1].count_2nd_main_Y_2nd);
                    isSame &= (bar[0].count_main_total == bar[1].count_main_total);
                    isSame &= (bar[0].count_axial == bar[1].count_axial);
                    isSame &= (bar[0].pitch_band == bar[1].pitch_band);
                    isSame &= (bar[0].count_band_dir_X == bar[1].count_band_dir_X);
                    isSame &= (bar[0].count_band_dir_Y == bar[1].count_band_dir_Y);
                    isSame &= (bar[0].pitch_bar_spacing == bar[1].pitch_bar_spacing);
                    isSame &= (bar[0].count_bar_spacing_X == bar[1].count_bar_spacing_X);
                    isSame &= (bar[0].count_bar_spacing_Y == bar[1].count_bar_spacing_Y);

                    s.StbSecBar_Arrangement = new StbSecColumn_RC.StbSecBar_ArrangementClass();
                    if (isSame)
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                        s.StbSecBar_Arrangement.StbSecRect_Column_Same = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass
                        {
                            count_main_X_1st = bar[0].count_main_X_1st,
                            count_main_X_2nd = bar[0].count_main_X_2nd,
                            count_main_Y_1st = bar[0].count_main_Y_1st,
                            count_main_Y_2nd = bar[0].count_main_Y_2nd,
                            count_2nd_main_X_1st = bar[0].count_2nd_main_X_1st,
                            count_2nd_main_X_2nd = bar[0].count_2nd_main_X_2nd,
                            count_2nd_main_Y_1st = bar[0].count_2nd_main_Y_1st,
                            count_2nd_main_Y_2nd = bar[0].count_2nd_main_Y_2nd,
                            count_main_total = bar[0].count_main_total,
                            count_axial = bar[0].count_axial,
                            pitch_band = bar[0].pitch_band,
                            count_band_dir_X = bar[0].count_band_dir_X,
                            count_band_dir_Y = bar[0].count_band_dir_Y,
                            pitch_bar_spacing = bar[0].pitch_bar_spacing,
                            count_bar_spacing_X = bar[0].count_bar_spacing_X,
                            count_bar_spacing_Y = bar[0].count_bar_spacing_Y
                        };
                    }
                    else
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[2];
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[0] = bar[0];
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[1] = bar[1];
                    }

                    #endregion
                }
                else
                {
                    #region RC円柱

                    paramName[ 0] = SetFamily.RCClmRo.name;
                    paramName[ 1] = SetFamily.RCClmRo.kind_column;
                    paramName[ 2] = SetFamily.RCClmRo.D_reinforcement_main[0];      //柱脚太筋径
                    paramName[ 3] = "";
                    paramName[ 4] = SetFamily.RCClmRo.D_reinforcement_axial;
                    paramName[ 5] = SetFamily.RCClmRo.D_reinforcement_band[0];      //柱脚
                    paramName[ 6] = SetFamily.RCClmRo.D_bar_spacing;
                    paramName[ 7] = SetFamily.RCClmRo.strength_concrete;
                    paramName[ 8] = SetFamily.RCClmRo.strength_reinforcement_main;
                    paramName[ 9] = "";
                    paramName[10] = SetFamily.RCClmRo.strength_reinforcement_axial;
                    paramName[11] = SetFamily.RCClmRo.strength_reinforcement_band;
                    paramName[12] = SetFamily.RCClmRo.strength_bar_spacing;
                    paramName[13] = SetFamily.RCClmRo.depth_cover_X;
                    paramName[14] = "";
                    paramName[15] = "";
                    paramName[16] = "";
                    paramName[17] = "";
                    paramName[18] = "";
                    paramName[19] = SetFamily.RCClmRo.center_reinforcement_start_X;
                    paramName[20] = "";
                    paramName[21] = "";
                    paramName[22] = "";

                    //形状
                    s.StbSecFigure = new StbSecColumn_RC.StbSecFigureClass()
                    {
                        StbSecFigureType = 2,
                        StbSecCircle = new StbSecColumn_RC.StbSecFigureClass.StbSecCircleClass(),
                    };

                    s.StbSecFigure.StbSecCircle.D = GetParameter_double(symbol, SetFamily.RCClmRo.D);


                    //配筋
                    StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[] bar = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[2];
                    for (int b = 0; b < bar.Length; ++b)
                    {
                        bar[b] = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass
                        {
                            pos = (b == 0 ? "BASE" : "TOP"),
                            count_main = GetParameter_int(symbol, SetFamily.RCClmRo.count_main[b]),
                            count_axial = GetParameter_int(symbol, SetFamily.RCClmRo.count_axial[b]),
                            count_band = GetParameter_int(symbol, SetFamily.RCClmRo.count_band[b]),
                            pitch_band = GetParameter_double(symbol, SetFamily.RCClmRo.pitch_band[b]),
                            pitch_bar_spacing = GetParameter_double(symbol, SetFamily.RCClmRo.pitch_bar_spacing[b]),
                            count_bar_spacing_X = GetParameter_int(symbol, SetFamily.RCClmRo.count_bar_spacing_X[b]),
                            count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.RCClmRo.count_bar_spacing_Y[b])
                        };
                    }

                    bool isSame = true;
                    isSame &= (bar[0].count_main == bar[1].count_main);
                    isSame &= (bar[0].count_axial == bar[1].count_axial);
                    isSame &= (bar[0].count_band == bar[1].count_band);
                    isSame &= (bar[0].pitch_band == bar[1].pitch_band);
                    isSame &= (bar[0].pitch_bar_spacing == bar[1].pitch_bar_spacing);
                    isSame &= (bar[0].count_bar_spacing_X == bar[1].count_bar_spacing_X);
                    isSame &= (bar[0].count_bar_spacing_Y == bar[1].count_bar_spacing_Y);

                    s.StbSecBar_Arrangement = new StbSecColumn_RC.StbSecBar_ArrangementClass();
                    if (isSame)
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Same = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass
                        {
                            count_main = bar[0].count_main,
                            count_axial = bar[0].count_axial,
                            count_band = bar[0].count_band,
                            pitch_band = bar[0].pitch_band,
                            pitch_bar_spacing = bar[0].pitch_bar_spacing,
                            count_bar_spacing_X = bar[0].count_bar_spacing_X,
                            count_bar_spacing_Y = bar[0].count_bar_spacing_Y
                        };
                    }
                    else
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 4;
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same = new StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[2];
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[0] = bar[0];
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[1] = bar[1];
                    }

                    #endregion
                }

                s.name                              = GetParameter_string(symbol, paramName[0]);
                s.kind_column                       = GetParameter_string(symbol, paramName[1]).ToUpper();
                s.D_reinforcement_main              = GetParameter_string(symbol, paramName[2]);
                s.D_reinforcement_2nd_main          = GetParameter_string(symbol, paramName[3]);
                s.D_reinforcement_axial             = GetParameter_string(symbol, paramName[4]);
                s.D_reinforcement_band              = GetParameter_string(symbol, paramName[5]);
                s.D_bar_spacing                     = GetParameter_string(symbol, paramName[6]);
                s.strength_concrete                 = GetParameter_string(symbol, paramName[7]);
                s.strength_reinforcement_main       = GetParameter_string(symbol, paramName[8]);
                s.strength_reinforcement_2nd_main   = GetParameter_string(symbol, paramName[9]);
                s.strength_reinforcement_axial      = GetParameter_string(symbol, paramName[10]);
                s.strength_reinforcement_band       = GetParameter_string(symbol, paramName[11]);
                s.strength_bar_spacing              = GetParameter_string(symbol, paramName[12]);
                s.depth_cover_start_X               = GetParameter_double(symbol, paramName[13]);
                s.depth_cover_end_X                 = GetParameter_double(symbol, paramName[14]);
                s.depth_cover_start_Y               = GetParameter_double(symbol, paramName[15]);
                s.depth_cover_end_Y                 = GetParameter_double(symbol, paramName[16]);
                s.kind_reinforcement_corner         = GetParameter_string(symbol, paramName[17]);
                s.interval_reinforcement            = GetParameter_double(symbol, paramName[18]);
                s.center_reinforcement_start_X      = GetParameter_double(symbol, paramName[19]);
                s.center_reinforcement_end_X        = GetParameter_double(symbol, paramName[20]);
                s.center_reinforcement_start_Y      = GetParameter_double(symbol, paramName[21]);
                s.center_reinforcement_end_Y        = GetParameter_double(symbol, paramName[22]);


                s.strength_concrete = GetConcreteFC(s.strength_concrete);

                stb.StbModel.StbSections.StbSecColumns_RC.Add(s);
                retID = s.id;

                #endregion
            }
            else if (familyname == SetFamily.SClmH.FamilyName    ||
                     familyname == SetFamily.SClmBH.FamilyName   ||
                     familyname == SetFamily.SClmBox.FamilyName  ||
                     familyname == SetFamily.SClmBBox.FamilyName ||
                     familyname == SetFamily.SClmPipe.FamilyName ||
                     familyname == SetFamily.SClmT.FamilyName    ||
                     familyname == SetFamily.SClmC.FamilyName    ||
                     familyname == SetFamily.SClmL.FamilyName    )
            {
                id_sect++;

                #region S柱

                StbSecColumn_S s = new StbSecColumn_S()
                {
                    id = id_sect,
                    floor = floor,
                    direction = false,
                    StbSecSteelColumn = new StbSecColumn_S.StbSecSteelColumnClass[1],
                };
                s.StbSecSteelColumn[0] = new StbSecColumn_S.StbSecSteelColumnClass()
                {
                    pos = "ALL",
                    shape = GetSteelName(symbol),
                };

                if (s.StbSecSteelColumn[0].shape == "") return retID;

                string[] paramName = new string[5];
                if (familyname == SetFamily.SClmH.FamilyName)
                {
                    #region H

                    paramName[0] = SetFamily.SClmH.name;
                    paramName[1] = SetFamily.SClmH.kind_column;
                    paramName[2] = SetFamily.SClmH.base_type;
                    paramName[3] = SetFamily.SClmH.strength_main;
                    paramName[4] = SetFamily.SClmH.strength_web;

                    #endregion
                }
                else if (familyname == SetFamily.SClmBH.FamilyName)
                {
                    #region BH

                    paramName[0] = SetFamily.SClmBH.name;
                    paramName[1] = SetFamily.SClmBH.kind_column;
                    paramName[2] = SetFamily.SClmBH.base_type;
                    paramName[3] = SetFamily.SClmBH.strength_main;
                    paramName[4] = SetFamily.SClmBH.strength_web;

                    #endregion
                }
                else if (familyname == SetFamily.SClmBox.FamilyName)
                {
                    #region Box

                    paramName[0] = SetFamily.SClmBox.name;
                    paramName[1] = SetFamily.SClmBox.kind_column;
                    paramName[2] = SetFamily.SClmBox.base_type;
                    paramName[3] = SetFamily.SClmBox.strength_main;
                    paramName[4] = "";

                    #endregion
                }
                else if (familyname == SetFamily.SClmBBox.FamilyName)
                {
                    #region BBox

                    paramName[0] = SetFamily.SClmBBox.name;
                    paramName[1] = SetFamily.SClmBBox.kind_column;
                    paramName[2] = SetFamily.SClmBBox.base_type;
                    paramName[3] = SetFamily.SClmBBox.strength_main;
                    paramName[4] = "";

                    #endregion
                }
                else if (familyname == SetFamily.SClmPipe.FamilyName)
                {
                    #region Pipe

                    paramName[0] = SetFamily.SClmPipe.name;
                    paramName[1] = SetFamily.SClmPipe.kind_column;
                    paramName[2] = SetFamily.SClmPipe.base_type;
                    paramName[3] = SetFamily.SClmPipe.strength_main;
                    paramName[4] = "";

                    #endregion
                }
                else if (familyname == SetFamily.SClmT.FamilyName)
                {
                    #region T

                    paramName[0] = SetFamily.SClmT.name;
                    paramName[1] = SetFamily.SClmT.kind_column;
                    paramName[2] = SetFamily.SClmT.base_type;
                    paramName[3] = SetFamily.SClmT.strength_main;
                    paramName[4] = SetFamily.SClmT.strength_web;

                    #endregion
                }
                else if (familyname == SetFamily.SClmC.FamilyName)
                {
                    #region C

                    paramName[0] = SetFamily.SClmC.name;
                    paramName[1] = SetFamily.SClmC.kind_column;
                    paramName[2] = SetFamily.SClmC.base_type;
                    paramName[3] = SetFamily.SClmC.strength_main;
                    paramName[4] = "";

                    #endregion
                }
                else if (familyname == SetFamily.SClmL.FamilyName)
                {
                    #region L

                    paramName[0] = SetFamily.SClmL.name;
                    paramName[1] = SetFamily.SClmL.kind_column;
                    paramName[2] = SetFamily.SClmL.base_type;
                    paramName[3] = SetFamily.SClmL.strength_main;
                    paramName[4] = "";

                    #endregion
                }
                else
                {
                    return retID;
                }

                s.name          = GetParameter_string(symbol, paramName[0]);
                s.kind_column   = GetParameter_string(symbol, paramName[1]).ToUpper();
                s.base_type     = GetParameter_string(symbol, paramName[2]);

                s.StbSecSteelColumn[0].strength_main = GetParameter_string(symbol, paramName[3]);
                s.StbSecSteelColumn[0].strength_web  = GetParameter_string(symbol, paramName[4]);


                if (s.base_type != "")
                {
                    //FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                    //XYZ p0 = Commons.mm2ft(ps);
                    //XYZ range = new XYZ(0.05, 0.05, 0.05);
                    //XYZ p1 = p0 - range;
                    //XYZ p2 = p0 + range;
                    //BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(new Outline(p1, p2));
                    //List<FamilyInstance> BaseProduct = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => x.Id != ins.Id).ToList();
                    //if (BaseProduct.Count == 1)
                    //{
                    //    s.StbSecBaseProduct = new StbSecColumn_S.StbSecBaseProductClass()
                    //    {
                    //        product_company = GetParameter_string(BaseProduct[0].Symbol, BuiltInParameter.ALL_MODEL_MANUFACTURER),
                    //        product_code = BaseProduct[0].Symbol.Name,
                    //    };
                    //}

                    s.StbSecBaseProduct = GetBaseProduct(ins.Id, ps);
                }

                stb.StbModel.StbSections.StbSecColumns_S.Add(s);
                retID = s.id;

                #endregion
            }
            else if (familyname == SetFamily.SRCClmH.FamilyName         ||
                     familyname == SetFamily.SRCClmCross.FamilyName     ||
                     familyname == SetFamily.SRCClmT.FamilyName         ||
                     familyname == SetFamily.SRCClmH_Rou.FamilyName     ||
                     familyname == SetFamily.SRCClmCross_Rou.FamilyName ||
                     familyname == SetFamily.SRCClmT_Rou.FamilyName     )
            {
                id_sect++;

                #region SRC柱

                StbSecColumn_SRC s = new StbSecColumn_SRC()
                {
                    id = id_sect,
                    floor = floor,
                };

                string shape = GetSteelName(symbol);
                if (shape == "") return retID;


                string[] paramName = new string[18];
                if (familyname == SetFamily.SRCClmH.FamilyName ||
                    familyname == SetFamily.SRCClmCross.FamilyName ||
                    familyname == SetFamily.SRCClmT.FamilyName)
                {
                    #region 矩形

                    s.StbSecFigure = new StbSecColumn_SRC.StbSecFigureClass()
                    {
                        StbSecFigureType = 1,
                        StbSecRect = new StbSecColumn_SRC.StbSecFigureClass.StbSecRectClass(),
                    };

                    StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[] bar = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[2];


                    if (familyname == SetFamily.SRCClmH.FamilyName)
                    {
                        #region SRC柱H形矩形

                        paramName[0] = SetFamily.SRCClmH.name;
                        paramName[1] = SetFamily.SRCClmH.kind_column;
                        paramName[2] = SetFamily.SRCClmH.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[3] = SetFamily.SRCClmH.D_reinforcement_2nd_main[0];  //柱脚細筋径
                        paramName[4] = SetFamily.SRCClmH.D_reinforcement_band[0];      //柱脚
                        paramName[5] = SetFamily.SRCClmH.D_bar_spacing;
                        paramName[6] = SetFamily.SRCClmH.strength_concrete;
                        paramName[7] = SetFamily.SRCClmH.strength_reinforcement_main;
                        paramName[8] = SetFamily.SRCClmH.strength_reinforcement_2nd_main;
                        paramName[9] = SetFamily.SRCClmH.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmH.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmH.depth_cover_X[0]; //始
                        paramName[12] = SetFamily.SRCClmH.depth_cover_X[1]; //終
                        paramName[13] = SetFamily.SRCClmH.depth_cover_Y[0]; //始
                        paramName[14] = SetFamily.SRCClmH.depth_cover_Y[1]; //終
                        paramName[15] = SetFamily.SRCClmH.kind_reinforcement_corner[0]; //柱脚
                        paramName[16] = SetFamily.SRCClmH.interval_reinforcement;
                        paramName[17] = SetFamily.SRCClmH.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecRect.DX = GetParameter_double(symbol, SetFamily.SRCClmH.DX);
                        s.StbSecFigure.StbSecRect.DY = GetParameter_double(symbol, SetFamily.SRCClmH.DY);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmH.count_main_X_1st[b]),
                                count_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmH.count_main_X_2nd[b]),
                                count_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmH.count_main_Y_1st[b]),
                                count_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmH.count_main_Y_2nd[b]),
                                count_2nd_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmH.count_2nd_main_X_1st[b]),
                                count_2nd_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmH.count_2nd_main_X_2nd[b]),
                                count_2nd_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmH.count_2nd_main_Y_1st[b]),
                                count_2nd_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmH.count_2nd_main_Y_2nd[b]),
                                count_main_total = GetParameter_int(symbol, SetFamily.SRCClmH.count_main_total),
                                count_axial = 0,
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmH.pitch_band[b]),
                                count_band_dir_X = GetParameter_int(symbol, SetFamily.SRCClmH.count_band_dir_X[b]),
                                count_band_dir_Y = GetParameter_int(symbol, SetFamily.SRCClmH.count_band_dir_Y[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmH.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmH.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmH.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "H",
                            StbSecColumn_SRC_ShapeH = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeH_Class(),
                        };

                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.shape = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.direction_type = GetParameter_string(symbol, SetFamily.SRCClmH.direction_type);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.strength_main = GetParameter_string(symbol, SetFamily.SRCClmH.strength_main);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.strength_web = GetParameter_string(symbol, SetFamily.SRCClmH.strength_web);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.offset_X = GetParameter_double(symbol, SetFamily.SRCClmH.offset_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.offset_Y = GetParameter_double(symbol, SetFamily.SRCClmH.offset_Y);

                        #endregion
                    }
                    else if (familyname == SetFamily.SRCClmCross.FamilyName)
                    {
                        #region SRC柱+形矩形

                        paramName[0] = SetFamily.SRCClmCross.name;
                        paramName[1] = SetFamily.SRCClmCross.kind_column;
                        paramName[2] = SetFamily.SRCClmCross.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[3] = SetFamily.SRCClmCross.D_reinforcement_2nd_main[0];  //柱脚細筋径
                        paramName[4] = SetFamily.SRCClmCross.D_reinforcement_band[0];      //柱脚
                        paramName[5] = SetFamily.SRCClmCross.D_bar_spacing;
                        paramName[6] = SetFamily.SRCClmCross.strength_concrete;
                        paramName[7] = SetFamily.SRCClmCross.strength_reinforcement_main;
                        paramName[8] = SetFamily.SRCClmCross.strength_reinforcement_2nd_main;
                        paramName[9] = SetFamily.SRCClmCross.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmCross.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmCross.depth_cover_X[0]; //始
                        paramName[12] = SetFamily.SRCClmCross.depth_cover_X[1]; //終
                        paramName[13] = SetFamily.SRCClmCross.depth_cover_Y[0]; //始
                        paramName[14] = SetFamily.SRCClmCross.depth_cover_Y[1]; //終
                        paramName[15] = SetFamily.SRCClmCross.kind_reinforcement_corner[0]; //柱脚
                        paramName[16] = SetFamily.SRCClmCross.interval_reinforcement;
                        paramName[17] = SetFamily.SRCClmCross.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecRect.DX = GetParameter_double(symbol, SetFamily.SRCClmCross.DX);
                        s.StbSecFigure.StbSecRect.DY = GetParameter_double(symbol, SetFamily.SRCClmCross.DY);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmCross.count_main_X_1st[b]),
                                count_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmCross.count_main_X_2nd[b]),
                                count_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmCross.count_main_Y_1st[b]),
                                count_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmCross.count_main_Y_2nd[b]),
                                count_2nd_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmCross.count_2nd_main_X_1st[b]),
                                count_2nd_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmCross.count_2nd_main_X_2nd[b]),
                                count_2nd_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmCross.count_2nd_main_Y_1st[b]),
                                count_2nd_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmCross.count_2nd_main_Y_2nd[b]),
                                count_main_total = GetParameter_int(symbol, SetFamily.SRCClmCross.count_main_total),
                                count_axial = 0,
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmCross.pitch_band[b]),
                                count_band_dir_X = GetParameter_int(symbol, SetFamily.SRCClmCross.count_band_dir_X[b]),
                                count_band_dir_Y = GetParameter_int(symbol, SetFamily.SRCClmCross.count_band_dir_Y[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmCross.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmCross.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmCross.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "CROSS",
                            StbSecColumn_SRC_ShapeCross = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeCross_Class(),
                        };



                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_X = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_Y = GetSteelName(symbol, 1);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_main_X = GetParameter_string(symbol, SetFamily.SRCClmCross.strength_main_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_main_Y = GetParameter_string(symbol, SetFamily.SRCClmCross.strength_main_Y);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_web_X = GetParameter_string(symbol, SetFamily.SRCClmCross.strength_web_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_web_Y = GetParameter_string(symbol, SetFamily.SRCClmCross.strength_web_Y);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_XX = GetParameter_double(symbol, SetFamily.SRCClmCross.offset_XX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_XY = GetParameter_double(symbol, SetFamily.SRCClmCross.offset_XY);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_YX = GetParameter_double(symbol, SetFamily.SRCClmCross.offset_YX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_YY = GetParameter_double(symbol, SetFamily.SRCClmCross.offset_YY);


                        if (s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_Y == "") return retID;

                        #endregion
                    }
                    else if (familyname == SetFamily.SRCClmT.FamilyName)
                    {
                        #region SRC柱T形矩形

                        paramName[0] = SetFamily.SRCClmT.name;
                        paramName[1] = SetFamily.SRCClmT.kind_column;
                        paramName[2] = SetFamily.SRCClmT.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[3] = SetFamily.SRCClmT.D_reinforcement_2nd_main[0];  //柱脚細筋径
                        paramName[4] = SetFamily.SRCClmT.D_reinforcement_band[0];      //柱脚
                        paramName[5] = SetFamily.SRCClmT.D_bar_spacing;
                        paramName[6] = SetFamily.SRCClmT.strength_concrete;
                        paramName[7] = SetFamily.SRCClmT.strength_reinforcement_main;
                        paramName[8] = SetFamily.SRCClmT.strength_reinforcement_2nd_main;
                        paramName[9] = SetFamily.SRCClmT.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmT.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmT.depth_cover_X[0]; //始
                        paramName[12] = SetFamily.SRCClmT.depth_cover_X[1]; //終
                        paramName[13] = SetFamily.SRCClmT.depth_cover_Y[0]; //始
                        paramName[14] = SetFamily.SRCClmT.depth_cover_Y[1]; //終
                        paramName[15] = SetFamily.SRCClmT.kind_reinforcement_corner[0]; //柱脚
                        paramName[16] = SetFamily.SRCClmT.interval_reinforcement;
                        paramName[17] = SetFamily.SRCClmT.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecRect.DX = GetParameter_double(symbol, SetFamily.SRCClmT.DX);
                        s.StbSecFigure.StbSecRect.DY = GetParameter_double(symbol, SetFamily.SRCClmT.DY);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmT.count_main_X_1st[b]),
                                count_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmT.count_main_X_2nd[b]),
                                count_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmT.count_main_Y_1st[b]),
                                count_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmT.count_main_Y_2nd[b]),
                                count_2nd_main_X_1st = GetParameter_int(symbol, SetFamily.SRCClmT.count_2nd_main_X_1st[b]),
                                count_2nd_main_X_2nd = GetParameter_int(symbol, SetFamily.SRCClmT.count_2nd_main_X_2nd[b]),
                                count_2nd_main_Y_1st = GetParameter_int(symbol, SetFamily.SRCClmT.count_2nd_main_Y_1st[b]),
                                count_2nd_main_Y_2nd = GetParameter_int(symbol, SetFamily.SRCClmT.count_2nd_main_Y_2nd[b]),
                                count_main_total = GetParameter_int(symbol, SetFamily.SRCClmT.count_main_total),
                                count_axial = 0,
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmT.pitch_band[b]),
                                count_band_dir_X = GetParameter_int(symbol, SetFamily.SRCClmT.count_band_dir_X[b]),
                                count_band_dir_Y = GetParameter_int(symbol, SetFamily.SRCClmT.count_band_dir_Y[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmT.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmT.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmT.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "T",
                            StbSecColumn_SRC_ShapeT = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeT_Class(),
                        };



                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_H = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_T = GetSteelName(symbol, 1);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.direction_type = GetParameter_string(symbol, SetFamily.SRCClmT.direction_type);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_main_H = GetParameter_string(symbol, SetFamily.SRCClmT.strength_main_H);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_main_T = GetParameter_string(symbol, SetFamily.SRCClmT.strength_main_T);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_web_H = GetParameter_string(symbol, SetFamily.SRCClmT.strength_web_H);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_web_T = GetParameter_string(symbol, SetFamily.SRCClmT.strength_web_T);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_HX = GetParameter_double(symbol, SetFamily.SRCClmT.offset_HX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_HY = GetParameter_double(symbol, SetFamily.SRCClmT.offset_HY);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_T = GetParameter_double(symbol, SetFamily.SRCClmT.offset_T);


                        if (s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_T == "") return retID;

                        #endregion
                    }
                    else
                    {
                        return retID;
                    }

                    bool isSame = true;
                    isSame &= (bar[0].count_main_X_1st == bar[1].count_main_X_1st);
                    isSame &= (bar[0].count_main_X_2nd == bar[1].count_main_X_2nd);
                    isSame &= (bar[0].count_main_Y_1st == bar[1].count_main_Y_1st);
                    isSame &= (bar[0].count_main_Y_2nd == bar[1].count_main_Y_2nd);
                    isSame &= (bar[0].count_2nd_main_X_1st == bar[1].count_2nd_main_X_1st);
                    isSame &= (bar[0].count_2nd_main_X_2nd == bar[1].count_2nd_main_X_2nd);
                    isSame &= (bar[0].count_2nd_main_Y_1st == bar[1].count_2nd_main_Y_1st);
                    isSame &= (bar[0].count_2nd_main_Y_2nd == bar[1].count_2nd_main_Y_2nd);
                    isSame &= (bar[0].count_main_total == bar[1].count_main_total);
                    isSame &= (bar[0].count_axial == bar[1].count_axial);
                    isSame &= (bar[0].pitch_band == bar[1].pitch_band);
                    isSame &= (bar[0].count_band_dir_X == bar[1].count_band_dir_X);
                    isSame &= (bar[0].count_band_dir_Y == bar[1].count_band_dir_Y);
                    isSame &= (bar[0].pitch_bar_spacing == bar[1].pitch_bar_spacing);
                    isSame &= (bar[0].count_bar_spacing_X == bar[1].count_bar_spacing_X);
                    isSame &= (bar[0].count_bar_spacing_Y == bar[1].count_bar_spacing_Y);

                    s.StbSecBar_Arrangement = new StbSecColumn_SRC.StbSecBar_ArrangementClass();
                    if (isSame)
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                        s.StbSecBar_Arrangement.StbSecRect_Column_Same = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass
                        {
                            count_main_X_1st = bar[0].count_main_X_1st,
                            count_main_X_2nd = bar[0].count_main_X_2nd,
                            count_main_Y_1st = bar[0].count_main_Y_1st,
                            count_main_Y_2nd = bar[0].count_main_Y_2nd,
                            count_2nd_main_X_1st = bar[0].count_2nd_main_X_1st,
                            count_2nd_main_X_2nd = bar[0].count_2nd_main_X_2nd,
                            count_2nd_main_Y_1st = bar[0].count_2nd_main_Y_1st,
                            count_2nd_main_Y_2nd = bar[0].count_2nd_main_Y_2nd,
                            count_main_total = bar[0].count_main_total,
                            count_axial = bar[0].count_axial,
                            pitch_band = bar[0].pitch_band,
                            count_band_dir_X = bar[0].count_band_dir_X,
                            count_band_dir_Y = bar[0].count_band_dir_Y,
                            pitch_bar_spacing = bar[0].pitch_bar_spacing,
                            count_bar_spacing_X = bar[0].count_bar_spacing_X,
                            count_bar_spacing_Y = bar[0].count_bar_spacing_Y
                        };
                    }
                    else
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass[2];
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[0] = bar[0];
                        s.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[1] = bar[1];
                    }

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmH_Rou.FamilyName ||
                         familyname == SetFamily.SRCClmCross_Rou.FamilyName ||
                         familyname == SetFamily.SRCClmT_Rou.FamilyName)
                {
                    #region 円形

                    s.StbSecFigure = new StbSecColumn_SRC.StbSecFigureClass()
                    {
                        StbSecFigureType = 2,
                        StbSecCircle = new StbSecColumn_SRC.StbSecFigureClass.StbSecCircleClass(),
                    };

                    StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[] bar = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[2];

                    if (familyname == SetFamily.SRCClmH_Rou.FamilyName)
                    {
                        #region SRC柱H形円形

                        paramName[ 0] = SetFamily.SRCClmH_Rou.name;
                        paramName[ 1] = SetFamily.SRCClmH_Rou.kind_column;
                        paramName[ 2] = SetFamily.SRCClmH_Rou.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[ 3] = "";
                        paramName[ 4] = SetFamily.SRCClmH_Rou.D_reinforcement_band[0];      //柱脚
                        paramName[ 5] = SetFamily.SRCClmH_Rou.D_bar_spacing;
                        paramName[ 6] = SetFamily.SRCClmH_Rou.strength_concrete;
                        paramName[ 7] = SetFamily.SRCClmH_Rou.strength_reinforcement_main;
                        paramName[ 8] = "";
                        paramName[ 9] = SetFamily.SRCClmH_Rou.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmH_Rou.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmH_Rou.depth_cover_X;
                        paramName[12] = "";
                        paramName[13] = "";
                        paramName[14] = "";
                        paramName[15] = "";
                        paramName[16] = "";
                        paramName[17] = SetFamily.SRCClmH_Rou.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecCircle.D = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.D);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main = GetParameter_int(symbol, SetFamily.SRCClmH_Rou.count_main[b]),
                                count_axial = 0,
                                count_band = GetParameter_int(symbol, SetFamily.SRCClmH_Rou.count_band[b]),
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.pitch_band[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmH_Rou.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmH_Rou.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "H",
                            StbSecColumn_SRC_ShapeH = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeH_Class(),
                        };

                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.shape = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.direction_type   = GetParameter_string(symbol, SetFamily.SRCClmH_Rou.direction_type);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.strength_main    = GetParameter_string(symbol, SetFamily.SRCClmH_Rou.strength_main);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.strength_web     = GetParameter_string(symbol, SetFamily.SRCClmH_Rou.strength_web);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.offset_X         = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.offset_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeH.offset_Y         = GetParameter_double(symbol, SetFamily.SRCClmH_Rou.offset_Y);


                        #endregion
                    }
                    else if (familyname == SetFamily.SRCClmCross_Rou.FamilyName)
                    {
                        #region SRC柱+形円形

                        paramName[ 0] = SetFamily.SRCClmCross_Rou.name;
                        paramName[ 1] = SetFamily.SRCClmCross_Rou.kind_column;
                        paramName[ 2] = SetFamily.SRCClmCross_Rou.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[ 3] = "";
                        paramName[ 4] = SetFamily.SRCClmCross_Rou.D_reinforcement_band[0];      //柱脚
                        paramName[ 5] = SetFamily.SRCClmCross_Rou.D_bar_spacing;
                        paramName[ 6] = SetFamily.SRCClmCross_Rou.strength_concrete;
                        paramName[ 7] = SetFamily.SRCClmCross_Rou.strength_reinforcement_main;
                        paramName[ 8] = "";
                        paramName[ 9] = SetFamily.SRCClmCross_Rou.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmCross_Rou.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmCross_Rou.depth_cover_X;
                        paramName[12] = "";
                        paramName[13] = "";
                        paramName[14] = "";
                        paramName[15] = "";
                        paramName[16] = "";
                        paramName[17] = SetFamily.SRCClmCross_Rou.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecCircle.D = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.D);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main = GetParameter_int(symbol, SetFamily.SRCClmCross_Rou.count_main[b]),
                                count_axial = 0,
                                count_band = GetParameter_int(symbol, SetFamily.SRCClmCross_Rou.count_band[b]),
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.pitch_band[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmCross_Rou.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmCross_Rou.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "CROSS",
                            StbSecColumn_SRC_ShapeCross = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeCross_Class(),
                        };

                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_X = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_Y = GetSteelName(symbol, 1);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_main_X = GetParameter_string(symbol, SetFamily.SRCClmCross_Rou.strength_main_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_main_Y = GetParameter_string(symbol, SetFamily.SRCClmCross_Rou.strength_main_Y);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_web_X  = GetParameter_string(symbol, SetFamily.SRCClmCross_Rou.strength_web_X);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.strength_web_Y  = GetParameter_string(symbol, SetFamily.SRCClmCross_Rou.strength_web_Y);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_XX       = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.offset_XX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_XY       = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.offset_XY);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_YX       = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.offset_YX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.offset_YY       = GetParameter_double(symbol, SetFamily.SRCClmCross_Rou.offset_YY);


                        if (s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeCross.shape_Y == "") return retID;


                        #endregion
                    }
                    else if (familyname == SetFamily.SRCClmT_Rou.FamilyName)
                    {
                        #region SRC柱T形円形

                        paramName[ 0] = SetFamily.SRCClmT_Rou.name;
                        paramName[ 1] = SetFamily.SRCClmT_Rou.kind_column;
                        paramName[ 2] = SetFamily.SRCClmT_Rou.D_reinforcement_main[0];      //柱脚太筋径
                        paramName[ 3] = "";
                        paramName[ 4] = SetFamily.SRCClmT_Rou.D_reinforcement_band[0];      //柱脚
                        paramName[ 5] = SetFamily.SRCClmT_Rou.D_bar_spacing;
                        paramName[ 6] = SetFamily.SRCClmT_Rou.strength_concrete;
                        paramName[ 7] = SetFamily.SRCClmT_Rou.strength_reinforcement_main;
                        paramName[ 8] = "";
                        paramName[ 9] = SetFamily.SRCClmT_Rou.strength_reinforcement_band;
                        paramName[10] = SetFamily.SRCClmT_Rou.strength_bar_spacing;
                        paramName[11] = SetFamily.SRCClmT_Rou.depth_cover_X;
                        paramName[12] = "";
                        paramName[13] = "";
                        paramName[14] = "";
                        paramName[15] = "";
                        paramName[16] = "";
                        paramName[17] = SetFamily.SRCClmT_Rou.base_type;


                        //RC形状
                        s.StbSecFigure.StbSecCircle.D = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.D);

                        //配筋
                        for (int b = 0; b < bar.Length; ++b)
                        {
                            bar[b] = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass
                            {
                                pos = (b == 0 ? "BASE" : "TOP"),
                                count_main = GetParameter_int(symbol, SetFamily.SRCClmT_Rou.count_main[b]),
                                count_axial = 0,
                                count_band = GetParameter_int(symbol, SetFamily.SRCClmT_Rou.count_band[b]),
                                pitch_band = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.pitch_band[b]),
                                pitch_bar_spacing = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.pitch_bar_spacing[b]),
                                count_bar_spacing_X = GetParameter_int(symbol, SetFamily.SRCClmT_Rou.count_bar_spacing_X[b]),
                                count_bar_spacing_Y = GetParameter_int(symbol, SetFamily.SRCClmT_Rou.count_bar_spacing_Y[b])
                            };
                        }

                        //S形状
                        s.StbSecSteelColumn_SRC = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class[1];
                        s.StbSecSteelColumn_SRC[0] = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class()
                        {
                            pos = "ALL",
                            build_up_shape = "T",
                            StbSecColumn_SRC_ShapeT = new StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeT_Class(),
                        };


                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_H = shape;
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_T = GetSteelName(symbol, 1);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.direction_type  = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.direction_type);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_main_H = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.strength_main_H);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_main_T = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.strength_main_T);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_web_H  = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.strength_web_H);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.strength_web_T  = GetParameter_string(symbol, SetFamily.SRCClmT_Rou.strength_web_T);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_HX       = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.offset_HX);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_HY       = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.offset_HY);
                        s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.offset_T        = GetParameter_double(symbol, SetFamily.SRCClmT_Rou.offset_T);


                        if (s.StbSecSteelColumn_SRC[0].StbSecColumn_SRC_ShapeT.shape_T == "") return retID;

                        #endregion
                    }
                    else
                    {
                        return retID;
                    }


                    bool isSame = true;
                    isSame &= (bar[0].count_main == bar[1].count_main);
                    isSame &= (bar[0].count_axial == bar[1].count_axial);
                    isSame &= (bar[0].count_band == bar[1].count_band);
                    isSame &= (bar[0].pitch_band == bar[1].pitch_band);
                    isSame &= (bar[0].pitch_bar_spacing == bar[1].pitch_bar_spacing);
                    isSame &= (bar[0].count_bar_spacing_X == bar[1].count_bar_spacing_X);
                    isSame &= (bar[0].count_bar_spacing_Y == bar[1].count_bar_spacing_Y);

                    s.StbSecBar_Arrangement = new StbSecColumn_SRC.StbSecBar_ArrangementClass();
                    if (isSame)
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Same = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass
                        {
                            count_main = bar[0].count_main,
                            count_axial = bar[0].count_axial,
                            count_band = bar[0].count_band,
                            pitch_band = bar[0].pitch_band,
                            pitch_bar_spacing = bar[0].pitch_bar_spacing,
                            count_bar_spacing_X = bar[0].count_bar_spacing_X,
                            count_bar_spacing_Y = bar[0].count_bar_spacing_Y
                        };
                    }
                    else
                    {
                        s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 4;
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same = new StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass[2];
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[0] = bar[0];
                        s.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[1] = bar[1];
                    }

                    #endregion
                }
                else
                {
                    return retID;
                }



                s.name                              = GetParameter_string(symbol, paramName[ 0]);
                s.kind_column                       = GetParameter_string(symbol, paramName[ 1]).ToUpper();
                s.D_reinforcement_main              = GetParameter_string(symbol, paramName[ 2]);
                s.D_reinforcement_2nd_main          = GetParameter_string(symbol, paramName[ 3]);
                s.D_reinforcement_band              = GetParameter_string(symbol, paramName[ 4]);
                s.D_bar_spacing                     = GetParameter_string(symbol, paramName[ 5]);
                s.strength_concrete                 = GetParameter_string(symbol, paramName[ 6]);
                s.strength_reinforcement_main       = GetParameter_string(symbol, paramName[ 7]);
                s.strength_reinforcement_2nd_main   = GetParameter_string(symbol, paramName[ 8]);
                s.strength_reinforcement_band       = GetParameter_string(symbol, paramName[ 9]);
                s.strength_bar_spacing              = GetParameter_string(symbol, paramName[10]);
                s.depth_cover_start_X               = GetParameter_double(symbol, paramName[11]);
                s.depth_cover_end_X                 = GetParameter_double(symbol, paramName[12]);
                s.depth_cover_start_Y               = GetParameter_double(symbol, paramName[13]);
                s.depth_cover_end_Y                 = GetParameter_double(symbol, paramName[14]);
                s.kind_reinforcement_corner         = GetParameter_string(symbol, paramName[15]);
                s.interval_reinforcement            = GetParameter_double(symbol, paramName[16]);
                s.base_type                         = GetParameter_string(symbol, paramName[17]);


                s.strength_concrete = GetConcreteFC(s.strength_concrete);


                if (s.base_type != "")
                {
                    StbSecColumn_S.StbSecBaseProductClass bp = GetBaseProduct(ins.Id, ps);
                    if (bp != null)
                    {
                        s.StbSecBaseProduct = new StbSecColumn_SRC.StbSecBaseProductClass()
                        {
                            product_company = bp.product_company,
                            product_code = bp.product_code,
                        };
                    }
                }

                stb.StbModel.StbSections.StbSecColumns_SRC.Add(s);
                retID = s.id;

                #endregion
            }
            else if (familyname == SetFamily.CFTClmBox.FamilyName ||
                     familyname == SetFamily.CFTClmPipe.FamilyName)
            {
                id_sect++;

                #region CFT柱

                StbSecColumn_CFT s = new StbSecColumn_CFT()
                {
                    id = id_sect,
                    floor = floor,
                };

                string shape = GetSteelName(symbol);
                if (shape == "") return retID;

                s.StbSecSteelColumn_CFT = new StbSecColumn_CFT.StbSecSteelColumn_CFT_Class[1];
                s.StbSecSteelColumn_CFT[0] = new StbSecColumn_CFT.StbSecSteelColumn_CFT_Class()
                {
                    pos = "ALL",
                    shape = shape,
                };

                string[] paramName = new string[6];
                if (familyname == SetFamily.CFTClmBox.FamilyName)
                {
                    #region CFT柱角形鋼管

                    paramName[0] = SetFamily.CFTClmBox.name;
                    paramName[1] = SetFamily.CFTClmBox.kind_column;
                    paramName[2] = SetFamily.CFTClmBox.strength_concrete;
                    paramName[3] = SetFamily.CFTClmBox.direction_type;
                    paramName[4] = SetFamily.CFTClmBox.base_type;
                    paramName[5] = SetFamily.CFTClmBox.enbedded_length;

                    s.StbSecSteelColumn_CFT[0].strength_main = GetParameter_string(symbol, SetFamily.CFTClmBox.strength_main);

                    #endregion
                }
                else if (familyname == SetFamily.CFTClmPipe.FamilyName)
                {
                    #region CFT柱鋼管

                    paramName[0] = SetFamily.CFTClmPipe.name;
                    paramName[1] = SetFamily.CFTClmPipe.kind_column;
                    paramName[2] = SetFamily.CFTClmPipe.strength_concrete;
                    paramName[3] = "";
                    paramName[4] = SetFamily.CFTClmPipe.base_type;
                    paramName[5] = SetFamily.CFTClmPipe.enbedded_length;

                    s.StbSecSteelColumn_CFT[0].strength_main = GetParameter_string(symbol, SetFamily.CFTClmPipe.strength_main);

                    #endregion
                }
                else
                {
                    return retID;
                }

                s.name              = GetParameter_string(symbol, paramName[0]);
                s.kind_column       = GetParameter_string(symbol, paramName[1]).ToUpper();
                s.strength_concrete = GetParameter_string(symbol, paramName[2]);
                s.direction         = GetParameter_bool  (symbol, paramName[3]);
                s.base_type         = GetParameter_string(symbol, paramName[4]);
                s.enbedded_length   = GetParameter_int   (symbol, paramName[5]);


                s.strength_concrete = GetConcreteFC(s.strength_concrete);


                if (s.base_type != "")
                {
                    StbSecColumn_S.StbSecBaseProductClass bp = GetBaseProduct(ins.Id, ps);
                    if (bp != null)
                    {
                        s.StbSecBaseProduct = new StbSecColumn_CFT.StbSecBaseProductClass()
                        {
                            product_company = bp.product_company,
                            product_code = bp.product_code,
                        };
                    }
                }


                stb.StbModel.StbSections.StbSecColumns_CFT.Add(s);
                retID = s.id;

                #endregion
            }


            return retID;
        }


        /// <summary>
        /// 柱配置の出力
        /// </summary>
        private static void Export_Column()
        {
            List<string> AllFamilyName = new List<string>();
            for (int i = 0; i < SetFamily.ClmFName.FamilyName.Length; ++i)
            {
                AllFamilyName.AddRange(SetFamily.ClmFName.FamilyName[i]);
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilyInstance> instances = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => AllFamilyName.Contains(x.Symbol.Family.Name) && !x.Symbol.Family.IsInPlace).ToList();

            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();
            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

            for (int i = 0; i < instances.Count; ++i)
            {
                XYZ ps1 = new XYZ();
                XYZ pe1 = new XYZ();
                XYZ ps2 = new XYZ();
                XYZ pe2 = new XYZ();

                if (instances[i].Location is LocationPoint locP)
                {
                    Parameter param = instances[i].get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                    double z1 = Levels.Find(x => x.Id == param.AsElementId()).ProjectElevation;

                    param = instances[i].get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
                    z1 += param.AsDouble();

                    param = instances[i].get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                    double z2 = Levels.Find(x => x.Id == param.AsElementId()).ProjectElevation;

                    param = instances[i].get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
                    z2 += param.AsDouble();

                    ps2 = new XYZ(locP.Point.X, locP.Point.Y, z1);
                    pe2 = new XYZ(locP.Point.X, locP.Point.Y, z2);
                }
                else
                {
                    LocationCurve locC = instances[i].Location as LocationCurve;
                    ps2 = locC.Curve.GetEndPoint(0);
                    pe2 = locC.Curve.GetEndPoint(1);
                }

                if (amanager.HasAssociation(instances[i].Id) && Commons.doc.GetElement(amanager.GetAssociatedElementId(instances[i].Id)) is AnalyticalMember member)
                {
                    ps1 = member.GetCurve().GetEndPoint(0);
                    pe1 = member.GetCurve().GetEndPoint(1);
                }
                else
                {
                    ps1 = ps2;
                    pe1 = pe2;
                }

                ps1 = Commons.ft2mm(ps1);
                pe1 = Commons.ft2mm(pe1);
                ps2 = Commons.ft2mm(ps2);
                pe2 = Commons.ft2mm(pe2);


                int node_s = GetNodeId(ps1);
                int node_e = GetNodeId(pe1);

                StbColumn c = new StbColumn()
                {
                    idNode_bottom = node_s,
                    idNode_top = node_e,
                };

                if (!sect.ContainsKey(instances[i].Symbol.Id))
                {
                    c.id_section = Export_SecColumn(instances[i], ps1);
                    if (c.id_section < 0) continue;

                    sect.Add(instances[i].Symbol.Id, c.id_section);
                }
                else
                {
                    c.id_section = sect[instances[i].Symbol.Id];
                }


                string[] paramName = new string[11];
                bool isPost = false;

                string familyname = instances[i].Symbol.Family.Name;
                if (familyname == SetFamily.RCClmRe.FamilyName)
                {
                    #region RC柱

                    c.kind_structure = "RC";

                    paramName[0] = SetFamily.RCClmRe.NameMembers;
                    paramName[1] = SetFamily.RCClmRe.thickness_ex_start_X;
                    paramName[2] = SetFamily.RCClmRe.thickness_ex_start_Y;
                    paramName[3] = SetFamily.RCClmRe.thickness_ex_end_X;
                    paramName[4] = SetFamily.RCClmRe.thickness_ex_end_Y;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_RC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.RCClmRo.FamilyName)
                {
                    #region RC円柱

                    c.kind_structure = "RC";

                    paramName[0] = SetFamily.RCClmRo.NameMembers;
                    paramName[1] = SetFamily.RCClmRo.thickness_ex_start_X;
                    paramName[2] = "";
                    paramName[3] = "";
                    paramName[4] = "";

                    isPost = (stb.StbModel.StbSections.StbSecColumns_RC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SClmH.FamilyName)
                {
                    #region S柱H

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmH.NameMembers;

                    paramName[ 5] = SetFamily.SClmH.condition_bottom;
                    paramName[ 6] = SetFamily.SClmH.condition_top;
                    paramName[ 7] = SetFamily.SClmH.joint_top;
                    paramName[ 8] = SetFamily.SClmH.joint_bottom;
                    paramName[ 9] = SetFamily.SClmH.kind_joint_top;
                    paramName[10] = SetFamily.SClmH.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SClmBH.FamilyName)
                {
                    #region S柱BH

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmBH.NameMembers;

                    paramName[ 5] = SetFamily.SClmBH.condition_bottom;
                    paramName[ 6] = SetFamily.SClmBH.condition_top;
                    paramName[ 7] = SetFamily.SClmBH.joint_top;
                    paramName[ 8] = SetFamily.SClmBH.joint_bottom;
                    paramName[ 9] = SetFamily.SClmBH.kind_joint_top;
                    paramName[10] = SetFamily.SClmBH.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");
                 
                    #endregion
                }
                else if (familyname == SetFamily.SClmBox.FamilyName)
                {
                    #region S柱Box

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmBox.NameMembers;

                    paramName[ 5] = SetFamily.SClmBox.condition_bottom;
                    paramName[ 6] = SetFamily.SClmBox.condition_top;
                    paramName[ 7] = SetFamily.SClmBox.joint_top;
                    paramName[ 8] = SetFamily.SClmBox.joint_bottom;
                    paramName[ 9] = SetFamily.SClmBox.kind_joint_top;
                    paramName[10] = SetFamily.SClmBox.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");
                 
                    #endregion
                }
                else if (familyname == SetFamily.SClmBBox.FamilyName)
                {
                    #region S柱BBox

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmBBox.NameMembers;

                    paramName[ 5] = SetFamily.SClmBBox.condition_bottom;
                    paramName[ 6] = SetFamily.SClmBBox.condition_top;
                    paramName[ 7] = SetFamily.SClmBBox.joint_top;
                    paramName[ 8] = SetFamily.SClmBBox.joint_bottom;
                    paramName[ 9] = SetFamily.SClmBBox.kind_joint_top;
                    paramName[10] = SetFamily.SClmBBox.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");
                 
                    #endregion
                }
                else if (familyname == SetFamily.SClmPipe.FamilyName)
                {
                    #region S柱Pipe

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmPipe.NameMembers;

                    paramName[ 5] = SetFamily.SClmPipe.condition_bottom;
                    paramName[ 6] = SetFamily.SClmPipe.condition_top;
                    paramName[ 7] = SetFamily.SClmPipe.joint_top;
                    paramName[ 8] = SetFamily.SClmPipe.joint_bottom;
                    paramName[ 9] = SetFamily.SClmPipe.kind_joint_top;
                    paramName[10] = SetFamily.SClmPipe.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SClmT.FamilyName)
                {
                    #region S柱T

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmT.NameMembers;

                    paramName[ 5] = SetFamily.SClmT.condition_bottom;
                    paramName[ 6] = SetFamily.SClmT.condition_top;
                    paramName[ 7] = SetFamily.SClmT.joint_top;
                    paramName[ 8] = SetFamily.SClmT.joint_bottom;
                    paramName[ 9] = SetFamily.SClmT.kind_joint_top;
                    paramName[10] = SetFamily.SClmT.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SClmC.FamilyName)
                {
                    #region S柱C

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmC.NameMembers;

                    paramName[ 5] = SetFamily.SClmC.condition_bottom;
                    paramName[ 6] = SetFamily.SClmC.condition_top;
                    paramName[ 7] = SetFamily.SClmC.joint_top;
                    paramName[ 8] = SetFamily.SClmC.joint_bottom;
                    paramName[ 9] = SetFamily.SClmC.kind_joint_top;
                    paramName[10] = SetFamily.SClmC.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SClmL.FamilyName)
                {
                    #region S柱L

                    c.kind_structure = "S";

                    paramName[0] = SetFamily.SClmL.NameMembers;

                    paramName[ 5] = SetFamily.SClmL.condition_bottom;
                    paramName[ 6] = SetFamily.SClmL.condition_top;
                    paramName[ 7] = SetFamily.SClmL.joint_top;
                    paramName[ 8] = SetFamily.SClmL.joint_bottom;
                    paramName[ 9] = SetFamily.SClmL.kind_joint_top;
                    paramName[10] = SetFamily.SClmL.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_S.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmH.FamilyName)
                {
                    #region SRC柱 H

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmH.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmH.thickness_ex_start_X;
                    paramName[ 2] = SetFamily.SRCClmH.thickness_ex_start_Y;
                    paramName[ 3] = SetFamily.SRCClmH.thickness_ex_end_X;
                    paramName[ 4] = SetFamily.SRCClmH.thickness_ex_end_Y;
                    paramName[ 5] = SetFamily.SRCClmH.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmH.condition_top;
                    paramName[ 7] = SetFamily.SRCClmH.joint_top;
                    paramName[ 8] = SetFamily.SRCClmH.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmH.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmH.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmCross.FamilyName)
                {
                    #region SRC柱 +

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmCross.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmCross.thickness_ex_start_X;
                    paramName[ 2] = SetFamily.SRCClmCross.thickness_ex_start_Y;
                    paramName[ 3] = SetFamily.SRCClmCross.thickness_ex_end_X;
                    paramName[ 4] = SetFamily.SRCClmCross.thickness_ex_end_Y;
                    paramName[ 5] = SetFamily.SRCClmCross.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmCross.condition_top;
                    paramName[ 7] = SetFamily.SRCClmCross.joint_top;
                    paramName[ 8] = SetFamily.SRCClmCross.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmCross.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmCross.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmT.FamilyName)
                {
                    #region SRC柱 T

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmT.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmT.thickness_ex_start_X;
                    paramName[ 2] = SetFamily.SRCClmT.thickness_ex_start_Y;
                    paramName[ 3] = SetFamily.SRCClmT.thickness_ex_end_X;
                    paramName[ 4] = SetFamily.SRCClmT.thickness_ex_end_Y;
                    paramName[ 5] = SetFamily.SRCClmT.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmT.condition_top;
                    paramName[ 7] = SetFamily.SRCClmT.joint_top;
                    paramName[ 8] = SetFamily.SRCClmT.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmT.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmT.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmH_Rou.FamilyName)
                {
                    #region SRC円柱 H

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmH_Rou.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmH_Rou.thickness_ex_start_X;
                    paramName[ 2] = "";
                    paramName[ 3] = "";
                    paramName[ 4] = "";
                    paramName[ 5] = SetFamily.SRCClmH_Rou.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmH_Rou.condition_top;
                    paramName[ 7] = SetFamily.SRCClmH_Rou.joint_top;
                    paramName[ 8] = SetFamily.SRCClmH_Rou.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmH_Rou.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmH_Rou.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmCross_Rou.FamilyName)
                {
                    #region SRC円柱 +

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmCross_Rou.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmCross_Rou.thickness_ex_start_X;
                    paramName[ 2] = "";
                    paramName[ 3] = "";
                    paramName[ 4] = "";
                    paramName[ 5] = SetFamily.SRCClmCross_Rou.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmCross_Rou.condition_top;
                    paramName[ 7] = SetFamily.SRCClmCross_Rou.joint_top;
                    paramName[ 8] = SetFamily.SRCClmCross_Rou.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmCross_Rou.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmCross_Rou.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.SRCClmT_Rou.FamilyName)
                {
                    #region SRC円柱 T

                    c.kind_structure = "SRC";

                    paramName[ 0] = SetFamily.SRCClmT_Rou.NameMembers;
                    paramName[ 1] = SetFamily.SRCClmT_Rou.thickness_ex_start_X;
                    paramName[ 2] = "";
                    paramName[ 3] = "";
                    paramName[ 4] = "";
                    paramName[ 5] = SetFamily.SRCClmT_Rou.condition_bottom;
                    paramName[ 6] = SetFamily.SRCClmT_Rou.condition_top;
                    paramName[ 7] = SetFamily.SRCClmT_Rou.joint_top;
                    paramName[ 8] = SetFamily.SRCClmT_Rou.joint_bottom;
                    paramName[ 9] = SetFamily.SRCClmT_Rou.kind_joint_top;
                    paramName[10] = SetFamily.SRCClmT_Rou.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_SRC.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.CFTClmBox.FamilyName)
                {
                    #region CFT柱角形鋼管

                    c.kind_structure = "CFT";

                    paramName[ 0] = SetFamily.CFTClmBox.NameMembers;
                    paramName[ 1] = "";
                    paramName[ 2] = "";
                    paramName[ 3] = "";
                    paramName[ 4] = "";
                    paramName[ 5] = SetFamily.CFTClmBox.condition_bottom;
                    paramName[ 6] = SetFamily.CFTClmBox.condition_top;
                    paramName[ 7] = SetFamily.CFTClmBox.joint_top;
                    paramName[ 8] = SetFamily.CFTClmBox.joint_bottom;
                    paramName[ 9] = SetFamily.CFTClmBox.kind_joint_top;
                    paramName[10] = SetFamily.CFTClmBox.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_CFT.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else if (familyname == SetFamily.CFTClmPipe.FamilyName)
                {
                    #region CFT柱鋼管

                    c.kind_structure = "CFT";

                    paramName[ 0] = SetFamily.CFTClmPipe.NameMembers;
                    paramName[ 1] = "";
                    paramName[ 2] = "";
                    paramName[ 3] = "";
                    paramName[ 4] = "";
                    paramName[ 5] = SetFamily.CFTClmPipe.condition_bottom;
                    paramName[ 6] = SetFamily.CFTClmPipe.condition_top;
                    paramName[ 7] = SetFamily.CFTClmPipe.joint_top;
                    paramName[ 8] = SetFamily.CFTClmPipe.joint_bottom;
                    paramName[ 9] = SetFamily.CFTClmPipe.kind_joint_top;
                    paramName[10] = SetFamily.CFTClmPipe.kind_joint_bottom;

                    isPost = (stb.StbModel.StbSections.StbSecColumns_CFT.Find(x => x.id == c.id_section)?.kind_column.ToUpper() == "POST");

                    #endregion
                }
                else
                {
                    continue;
                }


                id++;
                c.id = id;
                c.rotate = XYZ.BasisX.AngleOnPlaneTo(instances[i].HandOrientation, XYZ.BasisZ) * 180 / Math.PI;

                XYZ v1 = (pe1 - ps1).Normalize();
                XYZ v2 = (pe2 - ps2).Normalize();
                XYZ offset_s = ps2 - ps1;
                XYZ offset_e = pe2 - pe1;
                if (v1.CrossProduct(v2).Normalize().GetLength() < 0.001 &&
                    Math.Abs(offset_s.Z) < 0.1 &&
                    Math.Abs(offset_e.Z) < 0.1)
                {
                    c.offset_X = offset_s.X;
                    c.offset_Y = offset_s.Y;
                }
                else
                {
                    c.offset_bottom_X = offset_s.X;
                    c.offset_bottom_Y = offset_s.Y;
                    c.offset_bottom_Z = offset_s.Z;
                    c.offset_top_X = offset_e.X;
                    c.offset_top_Y = offset_e.Y;
                    c.offset_top_Z = offset_e.Z;
                }


                c.name                 = GetParameter_string(instances[i], paramName[0]);
                c.thickness_ex_start_X = GetParameter_double(instances[i], paramName[1]);
                c.thickness_ex_start_Y = GetParameter_double(instances[i], paramName[2]);
                c.thickness_ex_end_X   = GetParameter_double(instances[i], paramName[3]);
                c.thickness_ex_end_Y   = GetParameter_double(instances[i], paramName[4]);
                c.condition_bottom     = GetParameter_string(instances[i], paramName[5]);
                c.condition_top        = GetParameter_string(instances[i], paramName[6]);
                c.joint_top            = GetParameter_double(instances[i], paramName[7]);
                c.joint_bottom         = GetParameter_double(instances[i], paramName[8]);
                c.kind_joint_top       = GetParameter_string(instances[i], paramName[9]);
                c.kind_joint_bottom    = GetParameter_string(instances[i], paramName[10]);

                if (isPost)
                {
                    StbPost p = new StbPost()
                    {
                        id = c.id,
                        name = c.name,
                        idNode_bottom = c.idNode_bottom,
                        idNode_top = c.idNode_top,
                        rotate = c.rotate,
                        id_section = c.id_section,
                        kind_structure = c.kind_structure,
                        offset_X = c.offset_X,
                        offset_Y = c.offset_Y,
                        offset_bottom_X = c.offset_bottom_X,
                        offset_bottom_Y = c.offset_bottom_Y,
                        offset_bottom_Z = c.offset_bottom_Z,
                        offset_top_X = c.offset_top_X,
                        offset_top_Y = c.offset_top_Y,
                        offset_top_Z = c.offset_top_Z,
                        thickness_ex_start_X = c.thickness_ex_start_X,
                        thickness_ex_end_X = c.thickness_ex_end_X,
                        thickness_ex_start_Y = c.thickness_ex_start_Y,
                        thickness_ex_end_Y = c.thickness_ex_end_Y,
                        condition_bottom = c.condition_bottom,
                        condition_top = c.condition_top,
                        joint_top = c.joint_top,
                        joint_bottom = c.joint_bottom,
                        kind_joint_top = c.kind_joint_top,
                        kind_joint_bottom = c.kind_joint_bottom
                    };

                    stb.StbModel.StbMembers.StbPosts.Add(p);
                }
                else
                {
                    stb.StbModel.StbMembers.StbColumns.Add(c);
                }


                AddLog(LogCode.column, instances[i], c.id, c.id_section);

            }

        }



        /// <summary>
        /// 梁種別から基礎梁を判定
        /// </summary>
        /// <param name="kind_beam"></param>
        /// <param name="levelid"></param>
        /// <returns></returns>
        private static bool Check_isFoundation(string kind_beam, ElementId levelid)
        {
            if (kind_beam == "")
            {
                double GL = Levels.Find(a => a.Name == "GL")?.Elevation ?? 0;
                return (Levels.Find(a => a.Id == levelid)?.Elevation ?? 1000) < GL;
            }
            else
            {
                return kind_beam.Contains("Foundation");
            }
        }

        /// <summary>
        /// 梁種別から片持ち梁を判定
        /// </summary>
        /// <param name="kind_beam"></param>
        /// <returns></returns>
        private static bool Check_isCanti(string kind_beam)
        {
            if (kind_beam == "")
            {
                return false;
            }
            else
            {
                return kind_beam.Contains("Cantilever");
            }
        }


        #region 梁の同一チェック

        /// <summary>
        /// 梁断面始端中央終端別配筋（RC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_RC_Bar_SCE(StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass a,
                                                            StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁全断面同一配筋（RC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_RC_Bar_Same(StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass a,
                                                             StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁断面始端終端別配筋（RC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_RC_Bar_SE(StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass a,
                                                           StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁形状（RC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_StbSecFigure(StbSecBeam_RC.StbSecFigureClass a, StbSecBeam_RC.StbSecFigureClass b)
        {
            bool isSame = true;

            isSame &= (a.StbSecFigureType == b.StbSecFigureType);
            if (isSame)
            {
                switch (a.StbSecFigureType)
                {
                    case 1:
                        isSame &= (Math.Abs(a.StbSecStraight.width - b.StbSecStraight.width) < 0.01);
                        isSame &= (Math.Abs(a.StbSecStraight.depth - b.StbSecStraight.depth) < 0.01);
                        break;
                    case 2:
                        isSame &= (Math.Abs(a.StbSecTaper.width_start - b.StbSecTaper.width_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.width_end - b.StbSecTaper.width_end) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.depth_start - b.StbSecTaper.depth_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.depth_end - b.StbSecTaper.depth_end) < 0.01);
                        break;
                    case 3:
                        isSame &= (Math.Abs(a.StbSecHaunch.width_start - b.StbSecHaunch.width_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.width_center - b.StbSecHaunch.width_center) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.width_end   - b.StbSecHaunch.width_end) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_start - b.StbSecHaunch.depth_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_center - b.StbSecHaunch.depth_center) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_end   - b.StbSecHaunch.depth_end) < 0.01);
                        break;
                }
            }

            return isSame;
        }

        /// <summary>
        /// RC梁の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_RC(StbSecBeam_RC a, StbSecBeam_RC b)
        {
            bool isSame = true;

            isSame &= (a.name == b.name);
            isSame &= (a.kind_beam == b.kind_beam);
            isSame &= (a.isFoundation == b.isFoundation);
            isSame &= (a.isCanti == b.isCanti);
            isSame &= (a.isOutIn == b.isOutIn);
            isSame &= (a.D_reinforcement_main == b.D_reinforcement_main);
            isSame &= (a.D_reinforcement_2nd_main == b.D_reinforcement_2nd_main);
            isSame &= (a.D_stirrup == b.D_stirrup);
            isSame &= (a.D_reinforcement_web == b.D_reinforcement_web);
            isSame &= (a.D_bar_spacing == b.D_bar_spacing);
            isSame &= (a.strength_concrete == b.strength_concrete);
            isSame &= (a.strength_reinforcement_main == b.strength_reinforcement_main);
            isSame &= (a.strength_reinforcement_2nd_main == b.strength_reinforcement_2nd_main);
            isSame &= (a.strength_stirrup == b.strength_stirrup);
            isSame &= (a.strength_reinforcement_web == b.strength_reinforcement_web);
            isSame &= (a.strength_bar_spacing == b.strength_bar_spacing);

            isSame &= (Math.Abs(a.depth_cover_left - b.depth_cover_left) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_right - b.depth_cover_right) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_top - b.depth_cover_top) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_bottom - b.depth_cover_bottom) < 0.01);
            isSame &= (Math.Abs(a.interval_reinforcement - b.interval_reinforcement) < 0.01);
            isSame &= (Math.Abs(a.center_reinforcement_top - b.center_reinforcement_top) < 0.01);
            isSame &= (Math.Abs(a.center_reinforcement_bottom - b.center_reinforcement_bottom) < 0.01);
            isSame &= (Math.Abs(a.bar_length_start - b.bar_length_start) < 0.01);
            isSame &= (Math.Abs(a.bar_length_end - b.bar_length_end) < 0.01);

            if (!isSame) return isSame;

            isSame &= CompareTo_StbSecBeam_StbSecFigure(a.StbSecFigure, b.StbSecFigure);
            if (!isSame) return isSame;

            isSame &= (a.StbSecBar_Arrangement.StbSecBar_ArrangementType == b.StbSecBar_Arrangement.StbSecBar_ArrangementType);
            if (isSame)
            {
                switch (a.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                {
                    case 1:
                        isSame &= CompareTo_StbSecBeam_RC_Bar_Same(a.StbSecBar_Arrangement.StbSecBeam_Same_Section, b.StbSecBar_Arrangement.StbSecBeam_Same_Section);
                        break;
                    case 2:
                        isSame &= (a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length == b.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length);
                        if (isSame)
                        {
                            for (int i = 0; i < a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length; ++i)
                            {
                                isSame &= CompareTo_StbSecBeam_RC_Bar_SCE(a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i], b.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i]);
                            }
                        }
                        break;
                    case 3:
                        isSame &= (a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length == b.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length);
                        if (isSame)
                        {
                            for (int i = 0; i < a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length; ++i)
                            {
                                isSame &= CompareTo_StbSecBeam_RC_Bar_SE(a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[i], b.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[i]);
                            }
                        }
                        break;
                }
            }

            return isSame;
        }


        /// <summary>
        /// S梁の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_S(StbSecBeam_S a, StbSecBeam_S b)
        {
            bool isSame = true;

            isSame &= (a.name == b.name);
            isSame &= (a.floor == b.floor);
            isSame &= (a.kind_beam == b.kind_beam);
            isSame &= (a.isCanti == b.isCanti);
            isSame &= (a.isOutIn == b.isOutIn);

            if (!isSame) return isSame;

            isSame &= (a.StbSecSteelBeam.Length == b.StbSecSteelBeam.Length);
            if (isSame)
            {
                for (int i = 0; i < a.StbSecSteelBeam.Length; ++i)
                {
                    isSame &= (a.StbSecSteelBeam[i].pos == a.StbSecSteelBeam[i].pos);
                    isSame &= (a.StbSecSteelBeam[i].shape == a.StbSecSteelBeam[i].shape);
                    isSame &= (a.StbSecSteelBeam[i].strength_main == a.StbSecSteelBeam[i].strength_main);
                    isSame &= (a.StbSecSteelBeam[i].strength_web == a.StbSecSteelBeam[i].strength_web);
                    if (!isSame) return isSame;
                }
            }

            return isSame;
        }


        /// <summary>
        /// 梁断面始端中央終端別配筋（SRC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_SRC_Bar_SCE(StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass a,
                                                             StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁全断面同一配筋（SRC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_SRC_Bar_Same(StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass a,
                                                              StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁断面始端終端別配筋（SRC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_SRC_Bar_SE(StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass a,
                                                            StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass b)
        {
            bool isSame = true;

            isSame &= (a.count_main_top_1st == b.count_main_top_1st);
            isSame &= (a.count_main_top_2nd == b.count_main_top_2nd);
            isSame &= (a.count_main_top_3rd == b.count_main_top_3rd);
            isSame &= (a.count_main_bottom_1st == b.count_main_bottom_1st);
            isSame &= (a.count_main_bottom_2nd == b.count_main_bottom_2nd);
            isSame &= (a.count_main_bottom_3rd == b.count_main_bottom_3rd);
            isSame &= (a.count_2nd_main_top_1st == b.count_2nd_main_top_1st);
            isSame &= (a.count_2nd_main_top_2nd == b.count_2nd_main_top_2nd);
            isSame &= (a.count_2nd_main_top_3rd == b.count_2nd_main_top_3rd);
            isSame &= (a.count_2nd_main_bottom_1st == b.count_2nd_main_bottom_1st);
            isSame &= (a.count_2nd_main_bottom_2nd == b.count_2nd_main_bottom_2nd);
            isSame &= (a.count_2nd_main_bottom_3rd == b.count_2nd_main_bottom_3rd);
            isSame &= (a.count_stirrup == b.count_stirrup);
            isSame &= (Math.Abs(a.pitch_stirrup - b.pitch_stirrup) < 0.01);
            isSame &= (a.count_web == b.count_web);
            isSame &= (a.count_bar_spacing == b.count_bar_spacing);
            isSame &= (Math.Abs(a.pitch_bar_spacing - b.pitch_bar_spacing) < 0.01);

            return isSame;
        }

        /// <summary>
        /// 梁形状（SRC）の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_SRC_StbSecFigure(StbSecBeam_SRC.StbSecFigureClass a, StbSecBeam_SRC.StbSecFigureClass b)
        {
            bool isSame = true;

            isSame &= (a.StbSecFigureType == b.StbSecFigureType);
            if (isSame)
            {
                switch (a.StbSecFigureType)
                {
                    case 1:
                        isSame &= (Math.Abs(a.StbSecStraight.width - b.StbSecStraight.width) < 0.01);
                        isSame &= (Math.Abs(a.StbSecStraight.depth - b.StbSecStraight.depth) < 0.01);
                        break;
                    case 2:
                        isSame &= (Math.Abs(a.StbSecTaper.width_start - b.StbSecTaper.width_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.width_end - b.StbSecTaper.width_end) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.depth_start - b.StbSecTaper.depth_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecTaper.depth_end - b.StbSecTaper.depth_end) < 0.01);
                        break;
                    case 3:
                        isSame &= (Math.Abs(a.StbSecHaunch.width_start - b.StbSecHaunch.width_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.width_center - b.StbSecHaunch.width_center) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.width_end - b.StbSecHaunch.width_end) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_start - b.StbSecHaunch.depth_start) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_center - b.StbSecHaunch.depth_center) < 0.01);
                        isSame &= (Math.Abs(a.StbSecHaunch.depth_end - b.StbSecHaunch.depth_end) < 0.01);
                        break;
                }
            }

            return isSame;
        }

        /// <summary>
        /// SRC梁の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_StbSecBeam_SRC(StbSecBeam_SRC a, StbSecBeam_SRC b)
        {
            bool isSame = true;

            isSame &= (a.name == b.name);
            isSame &= (a.kind_beam == b.kind_beam);
            isSame &= (a.isFoundation == b.isFoundation);
            isSame &= (a.isCanti == b.isCanti);
            isSame &= (a.isOutIn == b.isOutIn);
            isSame &= (a.D_reinforcement_main == b.D_reinforcement_main);
            isSame &= (a.D_reinforcement_2nd_main == b.D_reinforcement_2nd_main);
            isSame &= (a.D_stirrup == b.D_stirrup);
            isSame &= (a.D_reinforcement_web == b.D_reinforcement_web);
            isSame &= (a.D_bar_spacing == b.D_bar_spacing);
            isSame &= (a.strength_concrete == b.strength_concrete);
            isSame &= (a.strength_reinforcement_main == b.strength_reinforcement_main);
            isSame &= (a.strength_reinforcement_2nd_main == b.strength_reinforcement_2nd_main);
            isSame &= (a.strength_stirrup == b.strength_stirrup);
            isSame &= (a.strength_reinforcement_web == b.strength_reinforcement_web);
            isSame &= (a.strength_bar_spacing == b.strength_bar_spacing);

            isSame &= (Math.Abs(a.depth_cover_left - b.depth_cover_left) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_right - b.depth_cover_right) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_top - b.depth_cover_top) < 0.01);
            isSame &= (Math.Abs(a.depth_cover_bottom - b.depth_cover_bottom) < 0.01);
            isSame &= (Math.Abs(a.interval_reinforcement - b.interval_reinforcement) < 0.01);
            isSame &= (Math.Abs(a.center_reinforcement_top - b.center_reinforcement_top) < 0.01);
            isSame &= (Math.Abs(a.center_reinforcement_bottom - b.center_reinforcement_bottom) < 0.01);
            isSame &= (Math.Abs(a.offset - b.offset) < 0.01);
            isSame &= (Math.Abs(a.level - b.level) < 0.01);

            if (!isSame) return isSame;

            isSame &= CompareTo_StbSecBeam_SRC_StbSecFigure(a.StbSecFigure, b.StbSecFigure);
            if (!isSame) return isSame;

            isSame &= (a.StbSecBar_Arrangement.StbSecBar_ArrangementType == b.StbSecBar_Arrangement.StbSecBar_ArrangementType);
            if (isSame)
            {
                switch (a.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                {
                    case 1:
                        isSame &= CompareTo_StbSecBeam_SRC_Bar_Same(a.StbSecBar_Arrangement.StbSecBeam_Same_Section, b.StbSecBar_Arrangement.StbSecBeam_Same_Section);
                        break;
                    case 2:
                        isSame &= (a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length == b.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length);
                        if (isSame)
                        {
                            for (int i = 0; i < a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Length; ++i)
                            {
                                isSame &= CompareTo_StbSecBeam_SRC_Bar_SCE(a.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i], b.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i]);
                            }
                        }
                        break;
                    case 3:
                        isSame &= (a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length == b.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length);
                        if (isSame)
                        {
                            for (int i = 0; i < a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Length; ++i)
                            {
                                isSame &= CompareTo_StbSecBeam_SRC_Bar_SE(a.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[i], b.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[i]);
                            }
                        }
                        break;
                }
            }

            return isSame;
        }


        #endregion


        /// <summary>
        /// 梁断面の出力
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private static int Export_SecGirder(FamilyInstance ins)
        {
            FamilySymbol symbol = ins.Symbol;
            string floor = Levels.Find(x => x.Id == ins.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()).Name;

            int retID = -1;

            string familyname = symbol.Family.Name;
            if (familyname == SetFamily.RCGir_F.FamilyName ||
                familyname == SetFamily.RCGir_F_Haunch.FamilyName ||
                familyname == SetFamily.RCBeam_F.FamilyName ||
                familyname == SetFamily.RCBeam_F_Haunch.FamilyName ||
                familyname == SetFamily.RCGir.FamilyName ||
                familyname == SetFamily.RCGir_Haunch.FamilyName ||
                familyname == SetFamily.RCBeam.FamilyName ||
                familyname == SetFamily.RCBeam_Haunch.FamilyName)
            {
                id_sect++;

                StbSecBeam_RC s = new StbSecBeam_RC()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                };

                FamilyStructure.RC_Gir RCGir = null;
                if (familyname == SetFamily.RCGir_F.FamilyName)
                {
                    RCGir = SetFamily.RCGir_F;
                }
                else if (familyname == SetFamily.RCGir_F_Haunch.FamilyName)
                {
                    RCGir = SetFamily.RCGir_F_Haunch;
                }
                else if (familyname == SetFamily.RCBeam_F.FamilyName)
                {
                    RCGir = SetFamily.RCBeam_F;
                }
                else if (familyname == SetFamily.RCBeam_F_Haunch.FamilyName)
                {
                    RCGir = SetFamily.RCBeam_F_Haunch;
                }
                else if (familyname == SetFamily.RCGir.FamilyName)
                {
                    RCGir = SetFamily.RCGir;
                }
                else if (familyname == SetFamily.RCGir_Haunch.FamilyName)
                {
                    RCGir = SetFamily.RCGir_Haunch;
                }
                else if (familyname == SetFamily.RCBeam.FamilyName)
                {
                    RCGir = SetFamily.RCBeam;
                }
                else if (familyname == SetFamily.RCBeam_Haunch.FamilyName)
                {
                    RCGir = SetFamily.RCBeam_Haunch;
                }

                if (RCGir == null) return retID;


                string kind_beam = GetParameter_string(symbol, RCGir.kind_beam);
                s.isFoundation = Check_isFoundation(kind_beam, ins.LevelId);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, RCGir.name);
                s.D_reinforcement_main = GetParameter_string(symbol, RCGir.D_reinforcement_main_top[1]);
                s.D_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.D_reinforcement_2nd_main_top[1]);
                s.D_stirrup = GetParameter_string(symbol, RCGir.D_stirrup[1]);
                s.D_reinforcement_web = GetParameter_string(symbol, RCGir.D_reinforcement_web[1]);
                s.D_bar_spacing = GetParameter_string(symbol, RCGir.D_bar_spacing[1]);
                s.strength_concrete = GetConcreteFC(GetParameter_string(symbol, RCGir.strength_concrete));
                s.strength_reinforcement_main = GetParameter_string(symbol, RCGir.strength_reinforcement_main);
                s.strength_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.strength_reinforcement_2nd_main);
                s.strength_stirrup = GetParameter_string(symbol, RCGir.strength_stirrup);
                s.strength_reinforcement_web = GetParameter_string(symbol, RCGir.strength_reinforcement_web);
                s.strength_bar_spacing = GetParameter_string(symbol, RCGir.strength_bar_spacing);
                s.depth_cover_left = GetParameter_double(symbol, RCGir.depth_cover_left);
                s.depth_cover_right = GetParameter_double(symbol, RCGir.depth_cover_right);
                s.depth_cover_top = GetParameter_double(symbol, RCGir.depth_cover_top);
                s.depth_cover_bottom = GetParameter_double(symbol, RCGir.depth_cover_bottom);
                s.interval_reinforcement = GetParameter_double(symbol, RCGir.interval_reinforcement);
                s.center_reinforcement_top = GetParameter_double(symbol, RCGir.center_reinforcement_top);
                s.center_reinforcement_bottom = GetParameter_double(symbol, RCGir.center_reinforcement_bottom);
                s.bar_length_start = GetParameter_double(symbol, RCGir.bar_length_start);
                s.bar_length_end = GetParameter_double(symbol, RCGir.bar_length_end);

                double ws = GetParameter_double(symbol, RCGir.width_start);
                double wc = GetParameter_double(symbol, RCGir.width_center);
                double we = GetParameter_double(symbol, RCGir.width_end);
                double ds = GetParameter_double(symbol, RCGir.depth_start);
                double dc = GetParameter_double(symbol, RCGir.depth_center);
                double de = GetParameter_double(symbol, RCGir.depth_end);

                //形状
                s.StbSecFigure = new StbSecBeam_RC.StbSecFigureClass();
                if (Math.Abs(ws - wc) < 0.01 && Math.Abs(we - wc) < 0.01 &&
                    Math.Abs(ds - dc) < 0.01 && Math.Abs(de - dc) < 0.01)
                {
                    s.StbSecFigure.StbSecFigureType = 1;
                    s.StbSecFigure.StbSecStraight = new StbSecBeam_RC.StbSecFigureClass.StbSecStraightClass()
                    {
                        width = wc,
                        depth = dc,
                    };
                }
                else
                {
                    if (s.isCanti)
                    {
                        if (ws < 0.1) ws = wc;
                        if (we < 0.1) we = wc;
                        if (ds < 0.1) ds = dc;
                        if (de < 0.1) de = dc;

                        s.StbSecFigure.StbSecFigureType = 2;
                        s.StbSecFigure.StbSecTaper = new StbSecBeam_RC.StbSecFigureClass.StbSecTaperClass()
                        {
                            width_start = ws,
                            width_end = we,
                            depth_start = ds,
                            depth_end = de,
                        };
                    }
                    else
                    {
                        s.StbSecFigure.StbSecFigureType = 3;
                        s.StbSecFigure.StbSecHaunch = new StbSecBeam_RC.StbSecFigureClass.StbSecHaunchClass()
                        {
                            width_start = ws,
                            width_center = wc,
                            width_end = we,
                            depth_start = ds,
                            depth_center = dc,
                            depth_end = de,
                        };
                    }
                }

                //配筋
                s.StbSecBar_Arrangement = new StbSecBeam_RC.StbSecBar_ArrangementClass();
                StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[] bar = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[3];
                for (int b = 0; b < bar.Length; ++b)
                {
                    bar[b] = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass()
                    {
                        count_main_top_1st = GetParameter_int(symbol, RCGir.count_main_top_1st[b]),
                        count_main_top_2nd = GetParameter_int(symbol, RCGir.count_main_top_2nd[b]),
                        count_main_top_3rd = GetParameter_int(symbol, RCGir.count_main_top_3rd[b]),
                        count_main_bottom_1st = GetParameter_int(symbol, RCGir.count_main_bottom_1st[b]),
                        count_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_main_bottom_2nd[b]),
                        count_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_main_bottom_3rd[b]),
                        count_2nd_main_top_1st = GetParameter_int(symbol, RCGir.count_2nd_main_top_1st[b]),
                        count_2nd_main_top_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_top_2nd[b]),
                        count_2nd_main_top_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_top_3rd[b]),
                        count_2nd_main_bottom_1st = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_1st[b]),
                        count_2nd_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_2nd[b]),
                        count_2nd_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_3rd[b]),
                        count_stirrup = GetParameter_int(symbol, RCGir.count_stirrup[b]),
                        pitch_stirrup = GetParameter_double(symbol, RCGir.pitch_stirrup[b]),
                        count_web = GetParameter_int(symbol, RCGir.count_web[b]),
                        count_bar_spacing = GetParameter_int(symbol, RCGir.count_bar_spacing[b]),
                        pitch_bar_spacing = GetParameter_double(symbol, RCGir.pitch_bar_spacing[b]),
                    };

                    switch (b)
                    {
                        case 0: bar[b].pos = "START"; break;
                        case 1: bar[b].pos = "CENTER"; break;
                        case 2: bar[b].pos = "END"; break;
                    }
                }

                bool isSame0 = CompareTo_StbSecBeam_RC_Bar_SCE(bar[0], bar[2]);
                bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE(bar[0], bar[1]);

                if (isSame0 && isSame1)
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                    s.StbSecBar_Arrangement.StbSecBeam_Same_Section = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass()
                    {
                        count_main_top_1st = bar[0].count_main_top_1st,
                        count_main_top_2nd = bar[0].count_main_top_2nd,
                        count_main_top_3rd = bar[0].count_main_top_3rd,
                        count_main_bottom_1st = bar[0].count_main_bottom_1st,
                        count_main_bottom_2nd = bar[0].count_main_bottom_2nd,
                        count_main_bottom_3rd = bar[0].count_main_bottom_3rd,
                        count_2nd_main_top_1st = bar[0].count_2nd_main_top_1st,
                        count_2nd_main_top_2nd = bar[0].count_2nd_main_top_2nd,
                        count_2nd_main_top_3rd = bar[0].count_2nd_main_top_3rd,
                        count_2nd_main_bottom_1st = bar[0].count_2nd_main_bottom_1st,
                        count_2nd_main_bottom_2nd = bar[0].count_2nd_main_bottom_2nd,
                        count_2nd_main_bottom_3rd = bar[0].count_2nd_main_bottom_3rd,
                        count_stirrup = bar[0].count_stirrup,
                        pitch_stirrup = bar[0].pitch_stirrup,
                        count_web = bar[0].count_web,
                        count_bar_spacing = bar[0].count_bar_spacing,
                        pitch_bar_spacing = bar[0].pitch_bar_spacing,
                    };
                }
                else
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                    s.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section = bar;
                }


                retID = stb.StbModel.StbSections.StbSecBeams_RC.Find(x => CompareTo_StbSecBeam_RC(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_RC.Add(s);
                    retID = s.id;
                }
            }
            else if (familyname == SetFamily.RCCGir_F.FamilyName ||
                     familyname == SetFamily.RCCBeam_F.FamilyName ||
                     familyname == SetFamily.RCCGir.FamilyName ||
                     familyname == SetFamily.RCCBeam.FamilyName)
            {
                id_sect++;

                StbSecBeam_RC s = new StbSecBeam_RC()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                };

                FamilyStructure.RC_CGir RCGir = null;
                if (familyname == SetFamily.RCCGir_F.FamilyName)
                {
                    RCGir = SetFamily.RCCGir_F;
                }
                else if (familyname == SetFamily.RCCBeam_F.FamilyName)
                {
                    RCGir = SetFamily.RCCBeam_F;
                }
                else if (familyname == SetFamily.RCCGir.FamilyName)
                {
                    RCGir = SetFamily.RCCGir;
                }
                else if (familyname == SetFamily.RCCBeam.FamilyName)
                {
                    RCGir = SetFamily.RCCBeam;
                }

                if (RCGir == null) return retID;


                string kind_beam = GetParameter_string(symbol, RCGir.kind_beam);
                s.isFoundation = Check_isFoundation(kind_beam, ins.LevelId);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, RCGir.name);
                s.D_reinforcement_main = GetParameter_string(symbol, RCGir.D_reinforcement_main_top[1]);
                s.D_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.D_reinforcement_2nd_main_top[1]);
                s.D_stirrup = GetParameter_string(symbol, RCGir.D_stirrup[1]);
                s.D_reinforcement_web = GetParameter_string(symbol, RCGir.D_reinforcement_web[1]);
                s.D_bar_spacing = GetParameter_string(symbol, RCGir.D_bar_spacing[1]);
                s.strength_concrete = GetConcreteFC(GetParameter_string(symbol, RCGir.strength_concrete));
                s.strength_reinforcement_main = GetParameter_string(symbol, RCGir.strength_reinforcement_main);
                s.strength_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.strength_reinforcement_2nd_main);
                s.strength_stirrup = GetParameter_string(symbol, RCGir.strength_stirrup);
                s.strength_reinforcement_web = GetParameter_string(symbol, RCGir.strength_reinforcement_web);
                s.strength_bar_spacing = GetParameter_string(symbol, RCGir.strength_bar_spacing);
                s.depth_cover_left = GetParameter_double(symbol, RCGir.depth_cover_left);
                s.depth_cover_right = GetParameter_double(symbol, RCGir.depth_cover_right);
                s.depth_cover_top = GetParameter_double(symbol, RCGir.depth_cover_top);
                s.depth_cover_bottom = GetParameter_double(symbol, RCGir.depth_cover_bottom);
                s.interval_reinforcement = GetParameter_double(symbol, RCGir.interval_reinforcement);
                s.center_reinforcement_top = GetParameter_double(symbol, RCGir.center_reinforcement_top);
                s.center_reinforcement_bottom = GetParameter_double(symbol, RCGir.center_reinforcement_bottom);
                s.bar_length_start = GetParameter_double(symbol, RCGir.bar_length_start);
                s.bar_length_end = GetParameter_double(symbol, RCGir.bar_length_end);


                double ws = GetParameter_double(symbol, RCGir.width_start);
                double we = GetParameter_double(symbol, RCGir.width_end);
                double ds = GetParameter_double(symbol, RCGir.depth_start);
                double de = GetParameter_double(symbol, RCGir.depth_end);

                //形状
                s.StbSecFigure = new StbSecBeam_RC.StbSecFigureClass();
                if (Math.Abs(ws - we) < 0.01 &&
                    Math.Abs(ds - de) < 0.01)
                {
                    s.StbSecFigure.StbSecFigureType = 1;
                    s.StbSecFigure.StbSecStraight = new StbSecBeam_RC.StbSecFigureClass.StbSecStraightClass()
                    {
                        width = ws,
                        depth = de,
                    };
                }
                else
                {
                    s.StbSecFigure.StbSecFigureType = 2;
                    s.StbSecFigure.StbSecTaper = new StbSecBeam_RC.StbSecFigureClass.StbSecTaperClass()
                    {
                        width_start = ws,
                        width_end = we,
                        depth_start = ds,
                        depth_end = de,
                    };
                }


                //配筋
                s.StbSecBar_Arrangement = new StbSecBeam_RC.StbSecBar_ArrangementClass();
                StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[] bar = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[2];
                for (int b = 0; b < bar.Length; ++b)
                {
                    bar[b] = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass()
                    {
                        count_main_top_1st = GetParameter_int(symbol, RCGir.count_main_top_1st[b]),
                        count_main_top_2nd = GetParameter_int(symbol, RCGir.count_main_top_2nd[b]),
                        count_main_top_3rd = GetParameter_int(symbol, RCGir.count_main_top_3rd[b]),
                        count_main_bottom_1st = GetParameter_int(symbol, RCGir.count_main_bottom_1st[b]),
                        count_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_main_bottom_2nd[b]),
                        count_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_main_bottom_3rd[b]),
                        count_2nd_main_top_1st = GetParameter_int(symbol, RCGir.count_2nd_main_top_1st[b]),
                        count_2nd_main_top_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_top_2nd[b]),
                        count_2nd_main_top_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_top_3rd[b]),
                        count_2nd_main_bottom_1st = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_1st[b]),
                        count_2nd_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_2nd[b]),
                        count_2nd_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_3rd[b]),
                        count_stirrup = GetParameter_int(symbol, RCGir.count_stirrup[b]),
                        pitch_stirrup = GetParameter_double(symbol, RCGir.pitch_stirrup[b]),
                        count_web = GetParameter_int(symbol, RCGir.count_web[b]),
                        count_bar_spacing = GetParameter_int(symbol, RCGir.count_bar_spacing[b]),
                        pitch_bar_spacing = GetParameter_double(symbol, RCGir.pitch_bar_spacing[b]),
                    };

                    switch (b)
                    {
                        case 0: bar[b].pos = "START"; break;
                        case 1: bar[b].pos = "END"; break;
                    }
                }

                bool isSame1 = CompareTo_StbSecBeam_RC_Bar_SCE(bar[0], bar[1]);

                if (isSame1)
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                    s.StbSecBar_Arrangement.StbSecBeam_Same_Section = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass()
                    {
                        count_main_top_1st = bar[0].count_main_top_1st,
                        count_main_top_2nd = bar[0].count_main_top_2nd,
                        count_main_top_3rd = bar[0].count_main_top_3rd,
                        count_main_bottom_1st = bar[0].count_main_bottom_1st,
                        count_main_bottom_2nd = bar[0].count_main_bottom_2nd,
                        count_main_bottom_3rd = bar[0].count_main_bottom_3rd,
                        count_2nd_main_top_1st = bar[0].count_2nd_main_top_1st,
                        count_2nd_main_top_2nd = bar[0].count_2nd_main_top_2nd,
                        count_2nd_main_top_3rd = bar[0].count_2nd_main_top_3rd,
                        count_2nd_main_bottom_1st = bar[0].count_2nd_main_bottom_1st,
                        count_2nd_main_bottom_2nd = bar[0].count_2nd_main_bottom_2nd,
                        count_2nd_main_bottom_3rd = bar[0].count_2nd_main_bottom_3rd,
                        count_stirrup = bar[0].count_stirrup,
                        pitch_stirrup = bar[0].pitch_stirrup,
                        count_web = bar[0].count_web,
                        count_bar_spacing = bar[0].count_bar_spacing,
                        pitch_bar_spacing = bar[0].pitch_bar_spacing,
                    };
                }
                else
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                    s.StbSecBar_Arrangement.StbSecBeam_Start_End_Section = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass[2];
                    for (int b = 0; b < bar.Length; ++b)
                    {
                        s.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[b] = new StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass()
                        {
                            pos = bar[b].pos,
                            count_main_top_1st = bar[b].count_main_top_1st,
                            count_main_top_2nd = bar[b].count_main_top_2nd,
                            count_main_top_3rd = bar[b].count_main_top_3rd,
                            count_main_bottom_1st = bar[b].count_main_bottom_1st,
                            count_main_bottom_2nd = bar[b].count_main_bottom_2nd,
                            count_main_bottom_3rd = bar[b].count_main_bottom_3rd,
                            count_2nd_main_top_1st = bar[b].count_2nd_main_top_1st,
                            count_2nd_main_top_2nd = bar[b].count_2nd_main_top_2nd,
                            count_2nd_main_top_3rd = bar[b].count_2nd_main_top_3rd,
                            count_2nd_main_bottom_1st = bar[b].count_2nd_main_bottom_1st,
                            count_2nd_main_bottom_2nd = bar[b].count_2nd_main_bottom_2nd,
                            count_2nd_main_bottom_3rd = bar[b].count_2nd_main_bottom_3rd,
                            count_stirrup = bar[b].count_stirrup,
                            pitch_stirrup = bar[b].pitch_stirrup,
                            count_web = bar[b].count_web,
                            count_bar_spacing = bar[b].count_bar_spacing,
                            pitch_bar_spacing = bar[b].pitch_bar_spacing,
                        };
                    }
                }


                retID = stb.StbModel.StbSections.StbSecBeams_RC.Find(x => CompareTo_StbSecBeam_RC(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_RC.Add(s);
                    retID = s.id;
                }
            }
            else if (familyname == SetFamily.SGirH.FamilyName ||
                     familyname == SetFamily.SGirH_Haunch.FamilyName ||
                     familyname == SetFamily.SBeamH.FamilyName ||
                     familyname == SetFamily.SBeamH_Haunch.FamilyName ||
                     familyname == SetFamily.SGirBH.FamilyName ||
                     familyname == SetFamily.SBeamBH.FamilyName ||
                     familyname == SetFamily.SGirC.FamilyName ||
                     familyname == SetFamily.SBeamC.FamilyName ||
                     familyname == SetFamily.SGirL.FamilyName ||
                     familyname == SetFamily.SBeamL.FamilyName ||
                     familyname == SetFamily.SGirLipC.FamilyName ||
                     familyname == SetFamily.SBeamLipC.FamilyName ||
                     familyname == SetFamily.SCGirC.FamilyName ||
                     familyname == SetFamily.SCBeamC.FamilyName ||
                     familyname == SetFamily.SCGirL.FamilyName ||
                     familyname == SetFamily.SCBeamL.FamilyName ||
                     familyname == SetFamily.SCGirLipC.FamilyName ||
                     familyname == SetFamily.SCBeamLipC.FamilyName)
            {
                id_sect++;

                StbSecBeam_S s = new StbSecBeam_S()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                };

                string[][] paramName = new string[4][];

                if (familyname == SetFamily.SGirH.FamilyName ||
                    familyname == SetFamily.SGirH_Haunch.FamilyName ||
                    familyname == SetFamily.SBeamH.FamilyName ||
                    familyname == SetFamily.SBeamH_Haunch.FamilyName ||
                    familyname == SetFamily.SGirBH.FamilyName ||
                    familyname == SetFamily.SBeamBH.FamilyName)
                {
                    FamilyStructure.S_Gir_H SGir = null;
                    if (familyname == SetFamily.SGirH.FamilyName)
                    {
                        SGir = SetFamily.SGirH;
                    }
                    else if (familyname == SetFamily.SGirH_Haunch.FamilyName)
                    {
                        SGir = SetFamily.SGirH_Haunch;
                    }
                    else if (familyname == SetFamily.SBeamH.FamilyName)
                    {
                        SGir = SetFamily.SBeamH;
                    }
                    else if (familyname == SetFamily.SBeamH_Haunch.FamilyName)
                    {
                        SGir = SetFamily.SBeamH_Haunch;
                    }
                    else if (familyname == SetFamily.SGirBH.FamilyName)
                    {
                        SGir = SetFamily.SGirBH;
                    }
                    else if (familyname == SetFamily.SBeamBH.FamilyName)
                    {
                        SGir = SetFamily.SBeamBH;
                    }

                    paramName[0] = new string[] { SGir.kind_beam };
                    paramName[1] = new string[] { SGir.name };
                    paramName[2] = SGir.strength_main;
                    paramName[3] = SGir.strength_web;
                }
                else if (familyname == SetFamily.SGirC.FamilyName ||
                         familyname == SetFamily.SBeamC.FamilyName ||
                         familyname == SetFamily.SCGirC.FamilyName ||
                         familyname == SetFamily.SCBeamC.FamilyName)
                {
                    FamilyStructure.S_Gir_C SGir = null;
                    if (familyname == SetFamily.SGirC.FamilyName)
                    {
                        SGir = SetFamily.SGirC;
                    }
                    else if (familyname == SetFamily.SBeamC.FamilyName)
                    {
                        SGir = SetFamily.SBeamC;
                    }
                    else if (familyname == SetFamily.SCGirC.FamilyName)
                    {
                        SGir = SetFamily.SCGirC;
                    }
                    else if (familyname == SetFamily.SCBeamC.FamilyName)
                    {
                        SGir = SetFamily.SCBeamC;
                    }

                    paramName[0] = new string[] { SGir.kind_beam };
                    paramName[1] = new string[] { SGir.name };
                    paramName[2] = new string[] { SGir.strength, SGir.strength, SGir.strength };
                    paramName[3] = new string[] { "", "", "" };
                }
                else if (familyname == SetFamily.SGirL.FamilyName ||
                         familyname == SetFamily.SBeamL.FamilyName ||
                         familyname == SetFamily.SCGirL.FamilyName ||
                         familyname == SetFamily.SCBeamL.FamilyName)
                {
                    FamilyStructure.S_Gir_L SGir = null;
                    if (familyname == SetFamily.SGirL.FamilyName)
                    {
                        SGir = SetFamily.SGirL;
                    }
                    else if (familyname == SetFamily.SBeamL.FamilyName)
                    {
                        SGir = SetFamily.SBeamL;
                    }
                    else if (familyname == SetFamily.SCGirL.FamilyName)
                    {
                        SGir = SetFamily.SCGirL;
                    }
                    else if (familyname == SetFamily.SCBeamL.FamilyName)
                    {
                        SGir = SetFamily.SCBeamL;
                    }

                    paramName[0] = new string[] { SGir.kind_beam };
                    paramName[1] = new string[] { SGir.name };
                    paramName[2] = new string[] { SGir.strength, SGir.strength, SGir.strength };
                    paramName[3] = new string[] { "", "", "" };
                }
                else if (familyname == SetFamily.SGirLipC.FamilyName ||
                         familyname == SetFamily.SBeamLipC.FamilyName ||
                         familyname == SetFamily.SCGirLipC.FamilyName ||
                         familyname == SetFamily.SCBeamLipC.FamilyName)
                {
                    FamilyStructure.S_Gir_LipC SGir = null;
                    if (familyname == SetFamily.SGirLipC.FamilyName)
                    {
                        SGir = SetFamily.SGirLipC;
                    }
                    else if (familyname == SetFamily.SBeamLipC.FamilyName)
                    {
                        SGir = SetFamily.SBeamLipC;
                    }
                    else if (familyname == SetFamily.SCGirLipC.FamilyName)
                    {
                        SGir = SetFamily.SCGirLipC;
                    }
                    else if (familyname == SetFamily.SCBeamLipC.FamilyName)
                    {
                        SGir = SetFamily.SCBeamLipC;
                    }

                    paramName[0] = new string[] { SGir.kind_beam };
                    paramName[1] = new string[] { SGir.name };
                    paramName[2] = new string[] { SGir.strength, SGir.strength, SGir.strength };
                    paramName[3] = new string[] { "", "", "" };
                }



                string kind_beam = GetParameter_string(symbol, paramName[0][0]);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, paramName[1][0]);

                //鉄骨
                string[] shape = new string[3];
                string[] strength_main = new string[3];
                string[] strength_web = new string[3];
                for (int LCR = 0; LCR < shape.Length; ++LCR)
                {
                    shape[LCR] = GetSteelName(symbol, 0, LCR);
                    strength_main[LCR] = GetParameter_string(symbol, paramName[2][LCR]);
                    strength_web[LCR] = GetParameter_string(symbol, paramName[3][LCR]);
                }

                if (shape[0] == shape[1] && shape[1] == shape[2] &&
                    strength_main[0] == strength_main[1] && strength_main[1] == strength_main[2] &&
                    strength_web[0] == strength_web[1] && strength_web[1] == strength_web[2])
                {
                    s.StbSecSteelBeam = new StbSecBeam_S.StbSecSteelBeamClass[1];
                    s.StbSecSteelBeam[0] = new StbSecBeam_S.StbSecSteelBeamClass()
                    {
                        pos = "ALL",
                        shape = shape[0],
                        strength_main = strength_main[0],
                        strength_web = strength_web[0],
                    };
                }
                else
                {
                    s.StbSecSteelBeam = new StbSecBeam_S.StbSecSteelBeamClass[3];
                    for (int LCR = 0; LCR < shape.Length; ++LCR)
                    {
                        string[] pos = new string[] { "START", "CENTER", "END" };
                        s.StbSecSteelBeam[LCR] = new StbSecBeam_S.StbSecSteelBeamClass()
                        {
                            pos = pos[LCR],
                            shape = shape[LCR],
                            strength_main = strength_main[LCR],
                            strength_web = strength_web[LCR],
                        };
                    }
                }

                retID = stb.StbModel.StbSections.StbSecBeams_S.Find(x => CompareTo_StbSecBeam_S(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_S.Add(s);
                    retID = s.id;
                }
            }
            else if (familyname == SetFamily.SCGirH.FamilyName ||
                     familyname == SetFamily.SCGirBH.FamilyName ||
                     familyname == SetFamily.SCBeamBH.FamilyName ||
                     familyname == SetFamily.SCBeamH.FamilyName)
            {
                id_sect++;

                StbSecBeam_S s = new StbSecBeam_S()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                };

                string[][] paramName = new string[4][];

                if (familyname == SetFamily.SCGirH.FamilyName ||
                    familyname == SetFamily.SCGirBH.FamilyName ||
                    familyname == SetFamily.SCBeamBH.FamilyName ||
                    familyname == SetFamily.SCBeamH.FamilyName)
                {
                    FamilyStructure.S_CGir_H SGir = null;
                    if (familyname == SetFamily.SCGirH.FamilyName)
                    {
                        SGir = SetFamily.SCGirH;
                    }
                    else if (familyname == SetFamily.SCGirBH.FamilyName)
                    {
                        SGir = SetFamily.SCGirBH;
                    }
                    else if (familyname == SetFamily.SCBeamBH.FamilyName)
                    {
                        SGir = SetFamily.SCBeamBH;
                    }
                    else if (familyname == SetFamily.SCBeamH.FamilyName)
                    {
                        SGir = SetFamily.SCBeamH;
                    }

                    paramName[0] = new string[] { SGir.kind_beam };
                    paramName[1] = new string[] { SGir.name };
                    paramName[2] = SGir.strength_main;
                    paramName[3] = SGir.strength_web;
                }

                string kind_beam = GetParameter_string(symbol, paramName[0][0]);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, paramName[1][0]);

                //鉄骨
                string[] shape = new string[2];
                string[] strength_main = new string[2];
                string[] strength_web = new string[2];
                for (int LCR = 0; LCR < shape.Length; ++LCR)
                {
                    shape[LCR] = GetSteelName(symbol, 0, LCR);
                    strength_main[LCR] = GetParameter_string(symbol, paramName[2][LCR]);
                    strength_web[LCR] = GetParameter_string(symbol, paramName[3][LCR]);
                }

                if (shape[0] == shape[1] &&
                    strength_main[0] == strength_main[1] &&
                    strength_web[0] == strength_web[1])
                {
                    s.StbSecSteelBeam = new StbSecBeam_S.StbSecSteelBeamClass[1];
                    s.StbSecSteelBeam[0] = new StbSecBeam_S.StbSecSteelBeamClass()
                    {
                        pos = "ALL",
                        shape = shape[0],
                        strength_main = strength_main[0],
                        strength_web = strength_web[0],
                    };
                }
                else
                {
                    s.StbSecSteelBeam = new StbSecBeam_S.StbSecSteelBeamClass[2];
                    for (int LCR = 0; LCR < shape.Length; ++LCR)
                    {
                        string[] pos = new string[] { "START", "END" };
                        s.StbSecSteelBeam[LCR] = new StbSecBeam_S.StbSecSteelBeamClass()
                        {
                            pos = pos[LCR],
                            shape = shape[LCR],
                            strength_main = strength_main[LCR],
                            strength_web = strength_web[LCR],
                        };
                    }
                }

                retID = stb.StbModel.StbSections.StbSecBeams_S.Find(x => CompareTo_StbSecBeam_S(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_S.Add(s);
                    retID = s.id;
                }
            }
            else if (familyname == SetFamily.SRCGirH.FamilyName ||
                     familyname == SetFamily.SRCBeamH.FamilyName)
            {
                id_sect++;

                StbSecBeam_SRC s = new StbSecBeam_SRC()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                    offset = 0,
                    level = 0,
                };

                FamilyStructure.SRC_Gir RCGir = null;
                if (familyname == SetFamily.SRCGirH.FamilyName)
                {
                    RCGir = SetFamily.SRCGirH;
                }
                else if (familyname == SetFamily.SRCBeamH.FamilyName)
                {
                    RCGir = SetFamily.SRCBeamH;
                }

                string kind_beam = GetParameter_string(symbol, RCGir.kind_beam);
                s.isFoundation = Check_isFoundation(kind_beam, ins.LevelId);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, RCGir.name);
                s.D_reinforcement_main = GetParameter_string(symbol, RCGir.D_reinforcement_main_top[1]);
                s.D_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.D_reinforcement_2nd_main_top[1]);
                s.D_stirrup = GetParameter_string(symbol, RCGir.D_stirrup[1]);
                s.D_reinforcement_web = GetParameter_string(symbol, RCGir.D_reinforcement_web[1]);
                s.D_bar_spacing = GetParameter_string(symbol, RCGir.D_bar_spacing[1]);
                s.strength_concrete = GetConcreteFC(GetParameter_string(symbol, RCGir.strength_concrete));
                s.strength_reinforcement_main = GetParameter_string(symbol, RCGir.strength_reinforcement_main);
                s.strength_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.strength_reinforcement_2nd_main);
                s.strength_stirrup = GetParameter_string(symbol, RCGir.strength_stirrup);
                s.strength_reinforcement_web = GetParameter_string(symbol, RCGir.strength_reinforcement_web);
                s.strength_bar_spacing = GetParameter_string(symbol, RCGir.strength_bar_spacing);
                s.depth_cover_left = GetParameter_double(symbol, RCGir.depth_cover_left);
                s.depth_cover_right = GetParameter_double(symbol, RCGir.depth_cover_right);
                s.depth_cover_top = GetParameter_double(symbol, RCGir.depth_cover_top);
                s.depth_cover_bottom = GetParameter_double(symbol, RCGir.depth_cover_bottom);
                s.interval_reinforcement = GetParameter_double(symbol, RCGir.interval_reinforcement);
                s.center_reinforcement_top = GetParameter_double(symbol, RCGir.center_reinforcement_top);
                s.center_reinforcement_bottom = GetParameter_double(symbol, RCGir.center_reinforcement_bottom);

                //形状
                double ws = GetParameter_double(symbol, RCGir.width_start);
                double wc = GetParameter_double(symbol, RCGir.width_center);
                double we = GetParameter_double(symbol, RCGir.width_end);
                double ds = GetParameter_double(symbol, RCGir.depth_start);
                double dc = GetParameter_double(symbol, RCGir.depth_center);
                double de = GetParameter_double(symbol, RCGir.depth_end);

                s.StbSecFigure = new StbSecBeam_SRC.StbSecFigureClass();
                if (Math.Abs(ws - wc) < 0.01 && Math.Abs(we - wc) < 0.01 &&
                    Math.Abs(ds - dc) < 0.01 && Math.Abs(de - dc) < 0.01)
                {
                    s.StbSecFigure.StbSecFigureType = 1;
                    s.StbSecFigure.StbSecStraight = new StbSecBeam_SRC.StbSecFigureClass.StbSecStraightClass()
                    {
                        width = wc,
                        depth = dc,
                    };
                }
                else
                {
                    if (s.isCanti)
                    {
                        s.StbSecFigure.StbSecFigureType = 2;
                        s.StbSecFigure.StbSecTaper = new StbSecBeam_SRC.StbSecFigureClass.StbSecTaperClass()
                        {
                            width_start = ws,
                            width_end = we,
                            depth_start = ds,
                            depth_end = de,
                        };
                    }
                    else
                    {
                        s.StbSecFigure.StbSecFigureType = 3;
                        s.StbSecFigure.StbSecHaunch = new StbSecBeam_SRC.StbSecFigureClass.StbSecHaunchClass()
                        {
                            width_start = ws,
                            width_center = wc,
                            width_end = we,
                            depth_start = ds,
                            depth_center = dc,
                            depth_end = de,
                        };
                    }
                }

                //配筋
                s.StbSecBar_Arrangement = new StbSecBeam_SRC.StbSecBar_ArrangementClass();
                StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[] bar = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[3];
                for (int b = 0; b < bar.Length; ++b)
                {
                    bar[b] = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass()
                    {
                        count_main_top_1st = GetParameter_int(symbol, RCGir.count_main_top_1st[b]),
                        count_main_top_2nd = GetParameter_int(symbol, RCGir.count_main_top_2nd[b]),
                        count_main_top_3rd = GetParameter_int(symbol, RCGir.count_main_top_3rd[b]),
                        count_main_bottom_1st = GetParameter_int(symbol, RCGir.count_main_bottom_1st[b]),
                        count_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_main_bottom_2nd[b]),
                        count_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_main_bottom_3rd[b]),
                        count_2nd_main_top_1st = GetParameter_int(symbol, RCGir.count_2nd_main_top_1st[b]),
                        count_2nd_main_top_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_top_2nd[b]),
                        count_2nd_main_top_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_top_3rd[b]),
                        count_2nd_main_bottom_1st = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_1st[b]),
                        count_2nd_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_2nd[b]),
                        count_2nd_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_3rd[b]),
                        count_stirrup = GetParameter_int(symbol, RCGir.count_stirrup[b]),
                        pitch_stirrup = GetParameter_double(symbol, RCGir.pitch_stirrup[b]),
                        count_web = GetParameter_int(symbol, RCGir.count_web[b]),
                        count_bar_spacing = GetParameter_int(symbol, RCGir.count_bar_spacing[b]),
                        pitch_bar_spacing = GetParameter_double(symbol, RCGir.pitch_bar_spacing[b]),
                    };

                    switch (b)
                    {
                        case 0: bar[b].pos = "START"; break;
                        case 1: bar[b].pos = "CENTER"; break;
                        case 2: bar[b].pos = "END"; break;
                    }
                }

                bool isSame0 = CompareTo_StbSecBeam_SRC_Bar_SCE(bar[0], bar[2]);
                bool isSame1 = CompareTo_StbSecBeam_SRC_Bar_SCE(bar[0], bar[1]);

                if (isSame0 && isSame1)
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                    s.StbSecBar_Arrangement.StbSecBeam_Same_Section = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass()
                    {
                        count_main_top_1st = bar[0].count_main_top_1st,
                        count_main_top_2nd = bar[0].count_main_top_2nd,
                        count_main_top_3rd = bar[0].count_main_top_3rd,
                        count_main_bottom_1st = bar[0].count_main_bottom_1st,
                        count_main_bottom_2nd = bar[0].count_main_bottom_2nd,
                        count_main_bottom_3rd = bar[0].count_main_bottom_3rd,
                        count_2nd_main_top_1st = bar[0].count_2nd_main_top_1st,
                        count_2nd_main_top_2nd = bar[0].count_2nd_main_top_2nd,
                        count_2nd_main_top_3rd = bar[0].count_2nd_main_top_3rd,
                        count_2nd_main_bottom_1st = bar[0].count_2nd_main_bottom_1st,
                        count_2nd_main_bottom_2nd = bar[0].count_2nd_main_bottom_2nd,
                        count_2nd_main_bottom_3rd = bar[0].count_2nd_main_bottom_3rd,
                        count_stirrup = bar[0].count_stirrup,
                        pitch_stirrup = bar[0].pitch_stirrup,
                        count_web = bar[0].count_web,
                        count_bar_spacing = bar[0].count_bar_spacing,
                        pitch_bar_spacing = bar[0].pitch_bar_spacing,
                    };
                }
                else
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                    s.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section = bar;
                }


                //鉄骨
                string[] shape = new string[3];
                string[] strength_main = new string[3];
                string[] strength_web = new string[3];
                for (int LCR = 0; LCR < shape.Length; ++LCR)
                {
                    shape[LCR] = GetSteelName(symbol, 0, LCR);
                    strength_main[LCR] = GetParameter_string(symbol, RCGir.strength_main[LCR]);
                    strength_web[LCR] = GetParameter_string(symbol, RCGir.strength_web[LCR]);
                }

                if (shape[0] == shape[1] && shape[1] == shape[2] &&
                    strength_main[0] == strength_main[1] && strength_main[1] == strength_main[2] &&
                    strength_web[0] == strength_web[1] && strength_web[1] == strength_web[2])
                {
                    s.StbSecSteelBeam = new StbSecBeam_SRC.StbSecSteelBeamClass[1];
                    s.StbSecSteelBeam[0] = new StbSecBeam_SRC.StbSecSteelBeamClass()
                    {
                        pos = "ALL",
                        shape = shape[0],
                        strength_main = strength_main[0],
                        strength_web = strength_web[0],
                    };
                }
                else
                {
                    s.StbSecSteelBeam = new StbSecBeam_SRC.StbSecSteelBeamClass[3];
                    for (int LCR = 0; LCR < shape.Length; ++LCR)
                    {
                        string[] pos = new string[] { "START", "CENTER", "END" };
                        s.StbSecSteelBeam[LCR] = new StbSecBeam_SRC.StbSecSteelBeamClass()
                        {
                            pos = pos[LCR],
                            shape = shape[LCR],
                            strength_main = strength_main[LCR],
                            strength_web = strength_web[LCR],
                        };
                    }
                }


                retID = stb.StbModel.StbSections.StbSecBeams_SRC.Find(x => CompareTo_StbSecBeam_SRC(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_SRC.Add(s);
                    retID = s.id;
                }
            }
            else if (familyname == SetFamily.SRCCGirH.FamilyName ||
                     familyname == SetFamily.SRCCBeamH.FamilyName)
            {
                id_sect++;

                StbSecBeam_SRC s = new StbSecBeam_SRC()
                {
                    id = id_sect,
                    floor = floor,
                    isOutIn = false,
                    offset = 0,
                    level = 0,
                };

                FamilyStructure.SRC_CGir RCGir = null;
                if (familyname == SetFamily.SRCCGirH.FamilyName)
                {
                    RCGir = SetFamily.SRCCGirH;
                }
                else if (familyname == SetFamily.SRCCBeamH.FamilyName)
                {
                    RCGir = SetFamily.SRCCBeamH;
                }

                string kind_beam = GetParameter_string(symbol, RCGir.kind_beam);
                s.isFoundation = Check_isFoundation(kind_beam, ins.LevelId);
                s.isCanti = Check_isCanti(kind_beam);
                s.kind_beam = (kind_beam.ToUpper().Contains("BEAM") ? "BEAM" : "GIRDER");

                s.name = GetParameter_string(symbol, RCGir.name);
                s.D_reinforcement_main = GetParameter_string(symbol, RCGir.D_reinforcement_main_top[1]);
                s.D_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.D_reinforcement_2nd_main_top[1]);
                s.D_stirrup = GetParameter_string(symbol, RCGir.D_stirrup[1]);
                s.D_reinforcement_web = GetParameter_string(symbol, RCGir.D_reinforcement_web[1]);
                s.D_bar_spacing = GetParameter_string(symbol, RCGir.D_bar_spacing[1]);
                s.strength_concrete = GetConcreteFC(GetParameter_string(symbol, RCGir.strength_concrete));
                s.strength_reinforcement_main = GetParameter_string(symbol, RCGir.strength_reinforcement_main);
                s.strength_reinforcement_2nd_main = GetParameter_string(symbol, RCGir.strength_reinforcement_2nd_main);
                s.strength_stirrup = GetParameter_string(symbol, RCGir.strength_stirrup);
                s.strength_reinforcement_web = GetParameter_string(symbol, RCGir.strength_reinforcement_web);
                s.strength_bar_spacing = GetParameter_string(symbol, RCGir.strength_bar_spacing);
                s.depth_cover_left = GetParameter_double(symbol, RCGir.depth_cover_left);
                s.depth_cover_right = GetParameter_double(symbol, RCGir.depth_cover_right);
                s.depth_cover_top = GetParameter_double(symbol, RCGir.depth_cover_top);
                s.depth_cover_bottom = GetParameter_double(symbol, RCGir.depth_cover_bottom);
                s.interval_reinforcement = GetParameter_double(symbol, RCGir.interval_reinforcement);
                s.center_reinforcement_top = GetParameter_double(symbol, RCGir.center_reinforcement_top);
                s.center_reinforcement_bottom = GetParameter_double(symbol, RCGir.center_reinforcement_bottom);

                double ws = GetParameter_double(symbol, RCGir.width_start);
                double we = GetParameter_double(symbol, RCGir.width_end);
                double ds = GetParameter_double(symbol, RCGir.depth_start);
                double de = GetParameter_double(symbol, RCGir.depth_end);

                //形状
                s.StbSecFigure = new StbSecBeam_SRC.StbSecFigureClass();
                if (Math.Abs(ws - we) < 0.01 &&
                    Math.Abs(ds - de) < 0.01)
                {
                    s.StbSecFigure.StbSecFigureType = 1;
                    s.StbSecFigure.StbSecStraight = new StbSecBeam_SRC.StbSecFigureClass.StbSecStraightClass()
                    {
                        width = ws,
                        depth = de,
                    };
                }
                else
                {
                    s.StbSecFigure.StbSecFigureType = 2;
                    s.StbSecFigure.StbSecTaper = new StbSecBeam_SRC.StbSecFigureClass.StbSecTaperClass()
                    {
                        width_start = ws,
                        width_end = we,
                        depth_start = ds,
                        depth_end = de,
                    };
                }

                //配筋
                s.StbSecBar_Arrangement = new StbSecBeam_SRC.StbSecBar_ArrangementClass();
                StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[] bar = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass[2];
                for (int b = 0; b < bar.Length; ++b)
                {
                    bar[b] = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass()
                    {
                        count_main_top_1st = GetParameter_int(symbol, RCGir.count_main_top_1st[b]),
                        count_main_top_2nd = GetParameter_int(symbol, RCGir.count_main_top_2nd[b]),
                        count_main_top_3rd = GetParameter_int(symbol, RCGir.count_main_top_3rd[b]),
                        count_main_bottom_1st = GetParameter_int(symbol, RCGir.count_main_bottom_1st[b]),
                        count_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_main_bottom_2nd[b]),
                        count_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_main_bottom_3rd[b]),
                        count_2nd_main_top_1st = GetParameter_int(symbol, RCGir.count_2nd_main_top_1st[b]),
                        count_2nd_main_top_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_top_2nd[b]),
                        count_2nd_main_top_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_top_3rd[b]),
                        count_2nd_main_bottom_1st = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_1st[b]),
                        count_2nd_main_bottom_2nd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_2nd[b]),
                        count_2nd_main_bottom_3rd = GetParameter_int(symbol, RCGir.count_2nd_main_bottom_3rd[b]),
                        count_stirrup = GetParameter_int(symbol, RCGir.count_stirrup[b]),
                        pitch_stirrup = GetParameter_double(symbol, RCGir.pitch_stirrup[b]),
                        count_web = GetParameter_int(symbol, RCGir.count_web[b]),
                        count_bar_spacing = GetParameter_int(symbol, RCGir.count_bar_spacing[b]),
                        pitch_bar_spacing = GetParameter_double(symbol, RCGir.pitch_bar_spacing[b]),
                    };

                    switch (b)
                    {
                        case 0: bar[b].pos = "START"; break;
                        case 1: bar[b].pos = "END"; break;
                    }
                }

                bool isSame1 = CompareTo_StbSecBeam_SRC_Bar_SCE(bar[0], bar[1]);
                if (isSame1)
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                    s.StbSecBar_Arrangement.StbSecBeam_Same_Section = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass()
                    {
                        count_main_top_1st = bar[0].count_main_top_1st,
                        count_main_top_2nd = bar[0].count_main_top_2nd,
                        count_main_top_3rd = bar[0].count_main_top_3rd,
                        count_main_bottom_1st = bar[0].count_main_bottom_1st,
                        count_main_bottom_2nd = bar[0].count_main_bottom_2nd,
                        count_main_bottom_3rd = bar[0].count_main_bottom_3rd,
                        count_2nd_main_top_1st = bar[0].count_2nd_main_top_1st,
                        count_2nd_main_top_2nd = bar[0].count_2nd_main_top_2nd,
                        count_2nd_main_top_3rd = bar[0].count_2nd_main_top_3rd,
                        count_2nd_main_bottom_1st = bar[0].count_2nd_main_bottom_1st,
                        count_2nd_main_bottom_2nd = bar[0].count_2nd_main_bottom_2nd,
                        count_2nd_main_bottom_3rd = bar[0].count_2nd_main_bottom_3rd,
                        count_stirrup = bar[0].count_stirrup,
                        pitch_stirrup = bar[0].pitch_stirrup,
                        count_web = bar[0].count_web,
                        count_bar_spacing = bar[0].count_bar_spacing,
                        pitch_bar_spacing = bar[0].pitch_bar_spacing,
                    };
                }
                else
                {
                    s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                    s.StbSecBar_Arrangement.StbSecBeam_Start_End_Section = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass[2];
                    for (int b = 0; b < bar.Length; ++b)
                    {
                        s.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[b] = new StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass()
                        {
                            pos = bar[b].pos,
                            count_main_top_1st = bar[b].count_main_top_1st,
                            count_main_top_2nd = bar[b].count_main_top_2nd,
                            count_main_top_3rd = bar[b].count_main_top_3rd,
                            count_main_bottom_1st = bar[b].count_main_bottom_1st,
                            count_main_bottom_2nd = bar[b].count_main_bottom_2nd,
                            count_main_bottom_3rd = bar[b].count_main_bottom_3rd,
                            count_2nd_main_top_1st = bar[b].count_2nd_main_top_1st,
                            count_2nd_main_top_2nd = bar[b].count_2nd_main_top_2nd,
                            count_2nd_main_top_3rd = bar[b].count_2nd_main_top_3rd,
                            count_2nd_main_bottom_1st = bar[b].count_2nd_main_bottom_1st,
                            count_2nd_main_bottom_2nd = bar[b].count_2nd_main_bottom_2nd,
                            count_2nd_main_bottom_3rd = bar[b].count_2nd_main_bottom_3rd,
                            count_stirrup = bar[b].count_stirrup,
                            pitch_stirrup = bar[b].pitch_stirrup,
                            count_web = bar[b].count_web,
                            count_bar_spacing = bar[b].count_bar_spacing,
                            pitch_bar_spacing = bar[b].pitch_bar_spacing,
                        };
                    }
                }


                //鉄骨
                string[] shape = new string[2];
                string[] strength_main = new string[2];
                string[] strength_web = new string[2];
                for (int LCR = 0; LCR < shape.Length; ++LCR)
                {
                    shape[LCR] = GetSteelName(symbol, 0, LCR);
                    strength_main[LCR] = GetParameter_string(symbol, RCGir.strength_main[LCR]);
                    strength_web[LCR] = GetParameter_string(symbol, RCGir.strength_web[LCR]);
                }

                if (shape[0] == shape[1] &&
                    strength_main[0] == strength_main[1] &&
                    strength_web[0] == strength_web[1])
                {
                    s.StbSecSteelBeam = new StbSecBeam_SRC.StbSecSteelBeamClass[1];
                    s.StbSecSteelBeam[0] = new StbSecBeam_SRC.StbSecSteelBeamClass()
                    {
                        pos = "ALL",
                        shape = shape[0],
                        strength_main = strength_main[0],
                        strength_web = strength_web[0],
                    };
                }
                else
                {
                    s.StbSecSteelBeam = new StbSecBeam_SRC.StbSecSteelBeamClass[2];
                    for (int LCR = 0; LCR < shape.Length; ++LCR)
                    {
                        string[] pos = new string[] { "START", "END" };
                        s.StbSecSteelBeam[LCR] = new StbSecBeam_SRC.StbSecSteelBeamClass()
                        {
                            pos = pos[LCR],
                            shape = shape[LCR],
                            strength_main = strength_main[LCR],
                            strength_web = strength_web[LCR],
                        };
                    }
                }


                retID = stb.StbModel.StbSections.StbSecBeams_SRC.Find(x => CompareTo_StbSecBeam_SRC(s, x))?.id ?? -1;
                if (retID < 0)
                {
                    stb.StbModel.StbSections.StbSecBeams_SRC.Add(s);
                    retID = s.id;
                }
            }


            return retID;
        }

        /// <summary>
        /// 梁の出力
        /// </summary>
        /// <param name="usage"></param>
        private static void Export_Girder(StructuralInstanceUsage usage)
        {
            List<string> AllFamilyName = new List<string>();
            if (usage == StructuralInstanceUsage.Girder)
            {
                for (int i = 0; i < SetFamily.GirFName.FamilyName.Length; ++i)
                {
                    AllFamilyName.AddRange(SetFamily.GirFName.FamilyName[i]);
                }
                for (int i = 0; i < SetFamily.CGirFName.FamilyName.Length; ++i)
                {
                    AllFamilyName.AddRange(SetFamily.CGirFName.FamilyName[i]);
                }
            }
            else
            {
                for (int i = 0; i < SetFamily.BeamFName.FamilyName.Length; ++i)
                {
                    AllFamilyName.AddRange(SetFamily.BeamFName.FamilyName[i]);
                }
                for (int i = 0; i < SetFamily.CBeamFName.FamilyName.Length; ++i)
                {
                    AllFamilyName.AddRange(SetFamily.CBeamFName.FamilyName[i]);
                }
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);

            ParameterValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM));
            FilterNumericRuleEvaluator evaluator1 = new FilterNumericEquals();
            FilterRule rule2 = new FilterIntegerRule(provider, evaluator1, (int)usage);
            ElementParameterFilter filter2 = new ElementParameterFilter(rule2);

            LogicalAndFilter filter = new LogicalAndFilter(filter1, filter2);

            List<FamilyInstance> instances = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => AllFamilyName.Contains(x.Symbol.Family.Name) && !x.Symbol.Family.IsInPlace).ToList();

            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();
            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

            for (int i = 0; i < instances.Count; ++i)
            {
                XYZ ps1 = new XYZ();
                XYZ pe1 = new XYZ();
                XYZ ps2 = GetFramingCoordinate(instances[i], 0);
                XYZ pe2 = GetFramingCoordinate(instances[i], 1);

                if (amanager.HasAssociation(instances[i].Id) && Commons.doc.GetElement(amanager.GetAssociatedElementId(instances[i].Id)) is AnalyticalMember member)
                {
                    ps1 = member.GetCurve().GetEndPoint(0);
                    pe1 = member.GetCurve().GetEndPoint(1);
                }
                else
                {
                    ps1 = ps2;
                    pe1 = pe2;
                }

                ps1 = Commons.ft2mm(ps1);
                pe1 = Commons.ft2mm(pe1);
                ps2 = Commons.ft2mm(ps2);
                pe2 = Commons.ft2mm(pe2);

                StbGirder g = new StbGirder()
                {
                    idNode_start = GetNodeId(ps1),
                    idNode_end = GetNodeId(pe1),
                    section_io_start = "",
                    section_io_end   = "",
                    isFoundation = false,
                };

                if (!sect.ContainsKey(instances[i].Symbol.Id))
                {
                    g.id_section = Export_SecGirder(instances[i]);
                    if (g.id_section < 0) continue;
                    sect.Add(instances[i].Symbol.Id, g.id_section);
                }
                else
                {
                    g.id_section = sect[instances[i].Symbol.Id];
                }


                string[] paramName = new string[18];
                string familyname = instances[i].Symbol.Family.Name;
                if (familyname == SetFamily.RCGir_F.FamilyName ||
                    familyname == SetFamily.RCGir_F_Haunch.FamilyName ||
                    familyname == SetFamily.RCBeam_F.FamilyName ||
                    familyname == SetFamily.RCBeam_F_Haunch.FamilyName ||
                    familyname == SetFamily.RCGir.FamilyName ||
                    familyname == SetFamily.RCGir_Haunch.FamilyName ||
                    familyname == SetFamily.RCBeam.FamilyName ||
                    familyname == SetFamily.RCBeam_Haunch.FamilyName ||
                    familyname == SetFamily.RCCGir_F.FamilyName ||
                    familyname == SetFamily.RCCBeam_F.FamilyName ||
                    familyname == SetFamily.RCCGir.FamilyName ||
                    familyname == SetFamily.RCCBeam.FamilyName)
                {
                    FamilyStructure.RC_Gir RCGir = null;
                    if (familyname == SetFamily.RCGir_F.FamilyName)
                    {
                        RCGir = SetFamily.RCGir_F;
                    }
                    else if (familyname == SetFamily.RCGir_F_Haunch.FamilyName)
                    {
                        RCGir = SetFamily.RCGir_F_Haunch;
                    }
                    else if (familyname == SetFamily.RCBeam_F.FamilyName)
                    {
                        RCGir = SetFamily.RCBeam_F;
                    }
                    else if (familyname == SetFamily.RCBeam_F_Haunch.FamilyName)
                    {
                        RCGir = SetFamily.RCBeam_F_Haunch;
                    }
                    else if (familyname == SetFamily.RCGir.FamilyName)
                    {
                        RCGir = SetFamily.RCGir;
                    }
                    else if (familyname == SetFamily.RCGir_Haunch.FamilyName)
                    {
                        RCGir = SetFamily.RCGir_Haunch;
                    }
                    else if (familyname == SetFamily.RCBeam.FamilyName)
                    {
                        RCGir = SetFamily.RCBeam;
                    }
                    else if (familyname == SetFamily.RCBeam_Haunch.FamilyName)
                    {
                        RCGir = SetFamily.RCBeam_Haunch;
                    }
                    else if (familyname == SetFamily.RCCGir_F.FamilyName)
                    {
                        RCGir = SetFamily.RCCGir_F;
                    }
                    else if (familyname == SetFamily.RCCBeam_F.FamilyName)
                    {
                        RCGir = SetFamily.RCCBeam_F;
                    }
                    else if (familyname == SetFamily.RCCGir.FamilyName)
                    {
                        RCGir = SetFamily.RCCGir;
                    }
                    else if (familyname == SetFamily.RCCBeam.FamilyName)
                    {
                        RCGir = SetFamily.RCCBeam;
                    }

                    g.kind_structure = "RC";

                    paramName[0] = RCGir.kind_beam;
                    paramName[1] = RCGir.NameMembers;
                    paramName[2] = RCGir.thickness_ex_top;
                    paramName[3] = RCGir.thickness_ex_bottom;
                    paramName[4] = RCGir.thickness_ex_right;
                    paramName[5] = RCGir.thickness_ex_left;
                    paramName[6] = "";
                    paramName[7] = "";
                    paramName[8] = RCGir.haunch_start;
                    paramName[9] = RCGir.haunch_end;
                    paramName[10] = "";
                    paramName[11] = "";
                    paramName[12] = RCGir.kind_haunch_start;
                    paramName[13] = RCGir.kind_haunch_end;
                    paramName[14] = RCGir.type_haunch_H;
                    paramName[15] = RCGir.type_haunch_V;
                    paramName[16] = "";
                    paramName[17] = "";
                }
                else if (familyname == SetFamily.SGirH.FamilyName ||
                         familyname == SetFamily.SGirH_Haunch.FamilyName ||
                         familyname == SetFamily.SBeamH.FamilyName ||
                         familyname == SetFamily.SBeamH_Haunch.FamilyName ||
                         familyname == SetFamily.SGirBH.FamilyName ||
                         familyname == SetFamily.SBeamBH.FamilyName ||
                         familyname == SetFamily.SCGirH.FamilyName ||
                         familyname == SetFamily.SCGirBH.FamilyName ||
                         familyname == SetFamily.SCBeamBH.FamilyName ||
                         familyname == SetFamily.SCBeamH.FamilyName)
                {
                    FamilyStructure.S_Gir_H SGir = null;
                    if (familyname == SetFamily.SGirH.FamilyName)
                    {
                        SGir = SetFamily.SGirH;
                    }
                    else if (familyname == SetFamily.SGirH_Haunch.FamilyName)
                    {
                        SGir = SetFamily.SGirH_Haunch;
                    }
                    else if (familyname == SetFamily.SBeamH.FamilyName)
                    {
                        SGir = SetFamily.SBeamH;
                    }
                    else if (familyname == SetFamily.SBeamH_Haunch.FamilyName)
                    {
                        SGir = SetFamily.SBeamH_Haunch;
                    }
                    else if (familyname == SetFamily.SGirBH.FamilyName)
                    {
                        SGir = SetFamily.SGirBH;
                    }
                    else if (familyname == SetFamily.SBeamBH.FamilyName)
                    {
                        SGir = SetFamily.SBeamBH;
                    }
                    else if (familyname == SetFamily.SCGirH.FamilyName)
                    {
                        SGir = SetFamily.SCGirH;
                    }
                    else if (familyname == SetFamily.SCGirBH.FamilyName)
                    {
                        SGir = SetFamily.SCGirBH;
                    }
                    else if (familyname == SetFamily.SCBeamBH.FamilyName)
                    {
                        SGir = SetFamily.SCBeamBH;
                    }
                    else if (familyname == SetFamily.SCBeamH.FamilyName)
                    {
                        SGir = SetFamily.SCBeamH;
                    }

                    g.kind_structure = "S";

                    paramName[0] = SGir.kind_beam;
                    paramName[1] = SGir.NameMembers;
                    paramName[2] = "";
                    paramName[3] = "";
                    paramName[4] = "";
                    paramName[5] = "";
                    paramName[6] = SGir.condition_start;
                    paramName[7] = SGir.condition_end;
                    paramName[8] = SGir.haunch_start;
                    paramName[9] = SGir.haunch_end;
                    paramName[10] = SGir.joint_start;
                    paramName[11] = SGir.joint_end;
                    paramName[12] = SGir.kind_haunch_start;
                    paramName[13] = SGir.kind_haunch_end;
                    paramName[14] = SGir.type_haunch_H;
                    paramName[15] = SGir.type_haunch_V;
                    paramName[16] = SGir.kind_joint_start;
                    paramName[17] = SGir.kind_joint_end;
                }
                else if (familyname == SetFamily.SGirC.FamilyName ||
                         familyname == SetFamily.SBeamC.FamilyName ||
                         familyname == SetFamily.SCGirC.FamilyName ||
                         familyname == SetFamily.SCBeamC.FamilyName)
                {
                    FamilyStructure.S_Gir_C SGir = null;
                    if (familyname == SetFamily.SGirC.FamilyName)
                    {
                        SGir = SetFamily.SGirC;
                    }
                    else if (familyname == SetFamily.SBeamC.FamilyName)
                    {
                        SGir = SetFamily.SBeamC;
                    }
                    else if (familyname == SetFamily.SCGirC.FamilyName)
                    {
                        SGir = SetFamily.SCGirC;
                    }
                    else if (familyname == SetFamily.SCBeamC.FamilyName)
                    {
                        SGir = SetFamily.SCBeamC;
                    }

                    g.kind_structure = "S";

                    paramName[0] = SGir.kind_beam;
                    paramName[1] = SGir.NameMembers;
                    paramName[2] = "";
                    paramName[3] = "";
                    paramName[4] = "";
                    paramName[5] = "";
                    paramName[6] = SGir.condition_start;
                    paramName[7] = SGir.condition_end;
                    paramName[8] = SGir.haunch_start;
                    paramName[9] = SGir.haunch_end;
                    paramName[10] = SGir.joint_start;
                    paramName[11] = SGir.joint_end;
                    paramName[12] = SGir.kind_haunch_start;
                    paramName[13] = SGir.kind_haunch_end;
                    paramName[14] = SGir.type_haunch_H;
                    paramName[15] = SGir.type_haunch_V;
                    paramName[16] = SGir.kind_joint_start;
                    paramName[17] = SGir.kind_joint_end;
                }
                else if (familyname == SetFamily.SGirL.FamilyName ||
                         familyname == SetFamily.SBeamL.FamilyName ||
                         familyname == SetFamily.SCGirL.FamilyName ||
                         familyname == SetFamily.SCBeamL.FamilyName)
                {
                    FamilyStructure.S_Gir_L SGir = null;
                    if (familyname == SetFamily.SGirL.FamilyName)
                    {
                        SGir = SetFamily.SGirL;
                    }
                    else if (familyname == SetFamily.SBeamL.FamilyName)
                    {
                        SGir = SetFamily.SBeamL;
                    }
                    else if (familyname == SetFamily.SCGirL.FamilyName)
                    {
                        SGir = SetFamily.SCGirL;
                    }
                    else if (familyname == SetFamily.SCBeamL.FamilyName)
                    {
                        SGir = SetFamily.SCBeamL;
                    }

                    g.kind_structure = "S";

                    paramName[0] = SGir.kind_beam;
                    paramName[1] = SGir.NameMembers;
                    paramName[2] = "";
                    paramName[3] = "";
                    paramName[4] = "";
                    paramName[5] = "";
                    paramName[6] = SGir.condition_start;
                    paramName[7] = SGir.condition_end;
                    paramName[8] = SGir.haunch_start;
                    paramName[9] = SGir.haunch_end;
                    paramName[10] = SGir.joint_start;
                    paramName[11] = SGir.joint_end;
                    paramName[12] = SGir.kind_haunch_start;
                    paramName[13] = SGir.kind_haunch_end;
                    paramName[14] = SGir.type_haunch_H;
                    paramName[15] = SGir.type_haunch_V;
                    paramName[16] = SGir.kind_joint_start;
                    paramName[17] = SGir.kind_joint_end;
                }
                else if (familyname == SetFamily.SGirLipC.FamilyName ||
                         familyname == SetFamily.SBeamLipC.FamilyName ||
                         familyname == SetFamily.SCGirLipC.FamilyName ||
                         familyname == SetFamily.SCBeamLipC.FamilyName)
                {
                    FamilyStructure.S_Gir_LipC SGir = null;
                    if (familyname == SetFamily.SGirLipC.FamilyName)
                    {
                        SGir = SetFamily.SGirLipC;
                    }
                    else if (familyname == SetFamily.SBeamLipC.FamilyName)
                    {
                        SGir = SetFamily.SBeamLipC;
                    }
                    else if (familyname == SetFamily.SCGirLipC.FamilyName)
                    {
                        SGir = SetFamily.SCGirLipC;
                    }
                    else if (familyname == SetFamily.SCBeamLipC.FamilyName)
                    {
                        SGir = SetFamily.SCBeamLipC;
                    }

                    g.kind_structure = "S";

                    paramName[0] = SGir.kind_beam;
                    paramName[1] = SGir.NameMembers;
                    paramName[2] = "";
                    paramName[3] = "";
                    paramName[4] = "";
                    paramName[5] = "";
                    paramName[6] = SGir.condition_start;
                    paramName[7] = SGir.condition_end;
                    paramName[8] = SGir.haunch_start;
                    paramName[9] = SGir.haunch_end;
                    paramName[10] = SGir.joint_start;
                    paramName[11] = SGir.joint_end;
                    paramName[12] = SGir.kind_haunch_start;
                    paramName[13] = SGir.kind_haunch_end;
                    paramName[14] = SGir.type_haunch_H;
                    paramName[15] = SGir.type_haunch_V;
                    paramName[16] = SGir.kind_joint_start;
                    paramName[17] = SGir.kind_joint_end;
                }
                else if (familyname == SetFamily.SRCGirH.FamilyName ||
                         familyname == SetFamily.SRCBeamH.FamilyName)
                {
                    FamilyStructure.SRC_Gir SRCGir = null;
                    if (familyname == SetFamily.SRCGirH.FamilyName)
                    {
                        SRCGir = SetFamily.SRCGirH;
                    }
                    else if (familyname == SetFamily.SRCBeamH.FamilyName)
                    {
                        SRCGir = SetFamily.SRCBeamH;
                    }

                    g.kind_structure = "SRC";

                    paramName[0] = SRCGir.kind_beam;
                    paramName[1] = SRCGir.NameMembers;
                    paramName[2] = SRCGir.thickness_ex_top;
                    paramName[3] = SRCGir.thickness_ex_bottom;
                    paramName[4] = SRCGir.thickness_ex_right;
                    paramName[5] = SRCGir.thickness_ex_left;
                    paramName[6] = SRCGir.condition_start;
                    paramName[7] = SRCGir.condition_end;
                    paramName[8] = SRCGir.haunch_start;
                    paramName[9] = SRCGir.haunch_end;
                    paramName[10] = SRCGir.joint_start;
                    paramName[11] = SRCGir.joint_end;
                    paramName[12] = SRCGir.kind_haunch_start;
                    paramName[13] = SRCGir.kind_haunch_end;
                    paramName[14] = SRCGir.type_haunch_H;
                    paramName[15] = SRCGir.type_haunch_V;
                    paramName[16] = SRCGir.kind_joint_start;
                    paramName[17] = SRCGir.kind_joint_end;
                }
                else if (familyname == SetFamily.SRCCGirH.FamilyName ||
                         familyname == SetFamily.SRCCBeamH.FamilyName)
                {
                    FamilyStructure.SRC_CGir SRCGir = null;
                    if (familyname == SetFamily.SRCCGirH.FamilyName)
                    {
                        SRCGir = SetFamily.SRCCGirH;
                    }
                    else if (familyname == SetFamily.SRCCBeamH.FamilyName)
                    {
                        SRCGir = SetFamily.SRCCBeamH;
                    }

                    g.kind_structure = "SRC";

                    paramName[0] = SRCGir.kind_beam;
                    paramName[1] = SRCGir.NameMembers;
                    paramName[2] = SRCGir.thickness_ex_top;
                    paramName[3] = SRCGir.thickness_ex_bottom;
                    paramName[4] = SRCGir.thickness_ex_right;
                    paramName[5] = SRCGir.thickness_ex_left;
                    paramName[6] = SRCGir.condition_start;
                    paramName[7] = SRCGir.condition_end;
                    paramName[8] = SRCGir.haunch_start;
                    paramName[9] = SRCGir.haunch_end;
                    paramName[10] = SRCGir.joint_start;
                    paramName[11] = SRCGir.joint_end;
                    paramName[12] = SRCGir.kind_haunch_start;
                    paramName[13] = SRCGir.kind_haunch_end;
                    paramName[14] = SRCGir.type_haunch_H;
                    paramName[15] = SRCGir.type_haunch_V;
                    paramName[16] = SRCGir.kind_joint_start;
                    paramName[17] = SRCGir.kind_joint_end;
                }
                else
                {
                    continue;
                }

                id++;
                g.id = id;
                g.rotate = GetParameter_Angle(instances[i], BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);

                string kind_beam = GetParameter_string(instances[i].Symbol, paramName[0]);
                g.isFoundation = Check_isFoundation(kind_beam, instances[i].get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId());

                XYZ v1 = (pe1 - ps1).Normalize();
                XYZ v2 = (pe2 - ps2).Normalize();
                XYZ offset_s = ps2 - ps1;
                XYZ offset_e = pe2 - pe1;

                //offset,levelは常にstart/end別で出力する
                /*
                //個別座標系のオフセット。Xが伸縮方向
                XYZ offset_s_local = instances[i].GetTransform().OfVector(offset_s);
                XYZ offset_e_local = instances[i].GetTransform().OfVector(offset_e);

                if (v1.CrossProduct(v2).Normalize().GetLength() < 0.001 &&
                    Math.Abs(offset_s_local.X) < 0.1 &&
                    Math.Abs(offset_e_local.X) < 0.1)
                {
                    XYZ vecU = instances[i].HandOrientation;
                    XYZ vecV = instances[i].FacingOrientation;
                    XYZ vecW = vecU.CrossProduct(vecV).Normalize();

                    double u = vecU.X * offset_s.X + vecU.Y * offset_s.Y + vecU.Z * offset_s.Z;
                    double v = vecV.X * offset_s.X + vecV.Y * offset_s.Y + vecV.Z * offset_s.Z;
                    double w = vecW.X * offset_s.X + vecW.Y * offset_s.Y + vecW.Z * offset_s.Z;

                    g.offset = v;
                    g.level = w;

                    g.offset_start_X = 0;
                    g.offset_start_Y = 0;
                    g.offset_start_Z = 0;
                    g.offset_end_X   = 0;
                    g.offset_end_Y   = 0;
                    g.offset_end_Z   = 0;
                }
                else
                {
                    g.offset_start_X = offset_s.X;
                    g.offset_start_Y = offset_s.Y;
                    g.offset_start_Z = offset_s.Z;
                    g.offset_end_X   = offset_e.X;
                    g.offset_end_Y   = offset_e.Y;
                    g.offset_end_Z   = offset_e.Z;

                    g.offset = 0;
                    g.level = 0;
                }
                //*/
                g.offset_start_X = offset_s.X;
                g.offset_start_Y = offset_s.Y;
                g.offset_start_Z = offset_s.Z;
                g.offset_end_X   = offset_e.X;
                g.offset_end_Y   = offset_e.Y;
                g.offset_end_Z   = offset_e.Z;

                g.offset = 0;
                g.level = 0;


                g.name                = GetParameter_string(instances[i], paramName[ 1]);
                g.thickness_ex_top    = GetParameter_double(instances[i], paramName[ 2]);
                g.thickness_ex_bottom = GetParameter_double(instances[i], paramName[ 3]);
                g.thickness_ex_right  = GetParameter_double(instances[i], paramName[ 4]);
                g.thickness_ex_left   = GetParameter_double(instances[i], paramName[ 5]);
                g.condition_start     = GetParameter_string(instances[i], paramName[ 6]);
                g.condition_end       = GetParameter_string(instances[i], paramName[ 7]);
                g.haunch_start        = GetParameter_double(instances[i].Symbol, paramName[ 8]);
                g.haunch_end          = GetParameter_double(instances[i].Symbol, paramName[ 9]);
                g.joint_start         = GetParameter_double(instances[i], paramName[10]);
                g.joint_end           = GetParameter_double(instances[i], paramName[11]);
                g.kind_haunch_start   = GetParameter_string(instances[i], paramName[12]);
                g.kind_haunch_end     = GetParameter_string(instances[i], paramName[13]);
                g.type_haunch_H       = GetParameter_string(instances[i], paramName[14]);
                g.type_haunch_V       = GetParameter_string(instances[i], paramName[15]);
                g.kind_joint_start    = GetParameter_string(instances[i], paramName[16]);
                g.kind_joint_end      = GetParameter_string(instances[i], paramName[17]);


                int nJoint = GetParameter_int(instances[i], "継手数");
                if (nJoint > 0)
                {
                    //水平のみ。垂直成分は無視する
                    XYZ v3 = new XYZ(instances[i].HandOrientation.X, instances[i].HandOrientation.Y, 0).Normalize();
                    XYZ v4 = new XYZ(instances[i].FacingOrientation.X, instances[i].FacingOrientation.Y, 0).Normalize();

                    if (nJoint > 0 && g.joint_start > 0)
                    {
                        XYZ pp1 = ps1 + v3 * g.joint_start;
                        XYZ pp2 = pp1 + v4;
                        g.joint_start = Math.Abs(Commons.LinePointDist(pp1.X, pp1.Y, pp2.X, pp2.Y, ps1.X, ps1.Y));
                    }

                    if (nJoint > 1 && g.joint_end > 0)
                    {
                        XYZ pp1 = pe1 - v3 * g.joint_end;
                        XYZ pp2 = pp1 + v4;
                        g.joint_end = Math.Abs(Commons.LinePointDist(pp1.X, pp1.Y, pp2.X, pp2.Y, pe1.X, pe1.Y));
                    }
                    else
                    {
                        //継手数1ならendなしとする
                        g.joint_end = 0;
                    }
                }

                if (usage == StructuralInstanceUsage.Joist)
                {
                    StbBeam b = new StbBeam()
                    {
                        id = g.id,
                        name = g.name,
                        idNode_start = g.idNode_start,
                        idNode_end = g.idNode_end,
                        rotate = g.rotate,
                        id_section = g.id_section,
                        section_io_start = g.section_io_start,
                        section_io_end = g.section_io_end,
                        kind_structure = g.kind_structure,
                        isFoundation = g.isFoundation,
                        offset = g.offset,
                        level = g.level,
                        offset_start_X = g.offset_start_X,
                        offset_start_Y = g.offset_start_Y,
                        offset_start_Z = g.offset_start_Z,
                        offset_end_X = g.offset_end_X,
                        offset_end_Y = g.offset_end_Y,
                        offset_end_Z = g.offset_end_Z,
                        thickness_ex_top = g.thickness_ex_top,
                        thickness_ex_bottom = g.thickness_ex_bottom,
                        thickness_ex_right = g.thickness_ex_right,
                        thickness_ex_left = g.thickness_ex_left,
                        condition_start = g.condition_start,
                        condition_end = g.condition_end,
                        haunch_start = g.haunch_start,
                        haunch_end = g.haunch_end,
                        joint_start = g.joint_start,
                        joint_end = g.joint_end,
                        kind_haunch_start = g.kind_haunch_start,
                        kind_haunch_end = g.kind_haunch_end,
                        type_haunch_H = g.type_haunch_H,
                        type_haunch_V = g.type_haunch_V,
                        kind_joint_start = g.kind_joint_start,
                        kind_joint_end = g.kind_joint_end,
                    };

                    stb.StbModel.StbMembers.StbBeams.Add(b);
                    AddLog(LogCode.beam, instances[i], g.id, g.id_section);
                }
                else
                {
                    stb.StbModel.StbMembers.StbGirders.Add(g);
                    AddLog(LogCode.girder, instances[i], g.id, g.id_section);
                }

            }
        }






        /// <summary>
        /// 壁の外周座標の取得
        /// </summary>
        /// <param name="w">壁</param>
        /// <param name="op">開口</param>
        /// <returns>座標[mm]</returns>
        private static List<XYZ> GetWallCoord(Wall w, List<Opening> op)
        {
            List<XYZ> points = new List<XYZ>();

            LocationCurve locC = w.Location as LocationCurve;
            XYZ p0 = locC.Curve.GetEndPoint(0);
            XYZ p1 = locC.Curve.GetEndPoint(1);
            XYZ pp = (p0.DistanceTo(XYZ.Zero) < p1.DistanceTo(XYZ.Zero) ? p0 : p1);
            XYZ v1 = (p1 - p0).Normalize();

            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
            if (Commons.doc.GetElement(amanager.GetAssociatedElementId(w.Id)) is AnalyticalPanel panel)
            {
                foreach (Line line in panel.GetOuterContour())
                {
                    points.Add(line.GetEndPoint(0));
                }
            }
            else
            {
                //LocationCurve＋Heightで四点求める

                double height = 0;
                Parameter param = w.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
                ElementId topLV = param.AsElementId();
                if (topLV.Value() == -1)
                {
                    param = w.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                    height = param.AsDouble();
                }
                else
                {
                    height = Levels.Find(x => x.Id == topLV).ProjectElevation;
                    height += w.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();

                    height -= Levels.Find(x => x.Id == w.LevelId).ProjectElevation;
                    height -= w.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
                }

                points.Add(p0);
                points.Add(p1);
                points.Add(p1 + XYZ.BasisZ * height);
                points.Add(p0 + XYZ.BasisZ * height);

            }

            double mindist = points.Min(x => x.DistanceTo(pp));
            int index = points.FindIndex(x => Math.Abs(x.DistanceTo(pp) - mindist) < 0.001);
            index = Math.Max(index, 0);

            int index2 = index + 1;
            if (index2 >= points.Count) index2 = 0;

            XYZ v2 = (points[index2] - points[index]).Normalize();
            bool reverse = (v1.CrossProduct(v2).Normalize().GetLength() > 0.001);

            List<XYZ> points2 = new List<XYZ>(points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                int ii = i + index;
                if (ii >= points.Count) ii = ii - points.Count;

                if (i == 0 || !reverse)
                {
                    points2.Add(Commons.ft2mm(points[ii]));
                }
                else
                {
                    points2.Insert(1, Commons.ft2mm(points[ii]));
                }
            }

            return points2;
        }

        /// <summary>
        /// 壁の出力
        /// </summary>
        private static void Export_Wall()
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<Wall> instances = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Wall>().ToList();

            collector = new FilteredElementCollector(Commons.doc);
            filter = new ElementCategoryFilter(BuiltInCategory.OST_SWallRectOpening);
            List<Opening> opens = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList();


            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();

            for (int i = 0; i < instances.Count; ++i)
            {
                if (!Check_Analytical_Model(instances[i]))
                {
                    AddWarning(-2, instances[i]);
                    continue;
                }


                List<Opening> opens2 = opens.Where(x => x.Host.Id == instances[i].Id).ToList();

                List<XYZ> points = GetWallCoord(instances[i], opens2);
                if (points.Count <= 2)
                {
                    AddWarning(-1, instances[i]);
                    continue;
                }

                if (GetParameter_string(instances[i].WallType, SetFamily.Wall.name) == "")
                {
                    AddWarning(-3, instances[i]);
                    continue;
                }

                id++;

                StbWall w = new StbWall()
                {
                    id = id,
                    kind_structure = "RC",
                    offset = 0,
                    StbNodeid_List = new List<StbNodeid>(),
                    StbOpens = new List<StbOpen>(),
                };

                LocationCurve locC = instances[i].Location as LocationCurve;
                XYZ wp0 = Commons.ft2mm(locC.Curve.GetEndPoint(0));
                XYZ wp1 = Commons.ft2mm(locC.Curve.GetEndPoint(1));
                XYZ v0 = (wp1 - wp0).Normalize();
                XYZ v1 = (points[1] - points[0]).Normalize();
                if (v0.CrossProduct(v1).GetLength() < 0.001)
                {
                    w.offset = Commons.LinePointDist(points[0].X, points[0].Y, points[1].X, points[1].Y, wp0.X, wp0.Y);
                }


                if (!sect.ContainsKey(instances[i].WallType.Id))
                {
                    #region 断面

                    id_sect++;
                    w.id_section = id_sect;

                    int secwall = 0;
                    string arrtype = GetParameter_string(instances[i].WallType, SetFamily.Wall.ArrengementType);

                    switch (secwall)
                    {
                        case 0: //RC

                            StbSecWall_RC sw = new StbSecWall_RC()
                            {
                                id = id_sect,
                                name = GetParameter_string(instances[i].WallType, SetFamily.Wall.name),
                                depth = Commons.ft2mm(instances[i].WallType.Width),
                                strength_concrete = GetParameter_string(instances[i].WallType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM),
                                depth_cover_outside = GetParameter_double(instances[i].WallType, SetFamily.Wall.depth_cover_outside),
                                depth_cover_inside = GetParameter_double(instances[i].WallType, SetFamily.Wall.depth_cover_inside),
                            };

                            sw.strength_concrete = GetConcreteFC(sw.strength_concrete);

                            sw.StbSecBar_Arrangement = new StbSecWall_RC.StbSecBar_ArrangementClass();

                            if (arrtype.Contains("シングル配筋"))
                            {
                                sw.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                                sw.StbSecBar_Arrangement.StbSecSingle = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecSingle_Class[2];
                                for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecSingle.Length; ++j)
                                {
                                    sw.StbSecBar_Arrangement.StbSecSingle[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecSingle_Class()
                                    {
                                        pos = (j == 0 ? "VERTICAL" : "HORIZONTAL"),
                                        strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                        D = GetParameter_D(instances[i].WallType, SetFamily.Wall.D[j], SetFamily.Wall.D2[j]),
                                        pitch = GetParameter_double(instances[i].WallType, SetFamily.Wall.pitch[j]),
                                    };

                                    if (sw.StbSecBar_Arrangement.StbSecSingle[j].D == "" ||
                                        sw.StbSecBar_Arrangement.StbSecSingle[j].pitch < 0.001)
                                    {
                                        sw.StbSecBar_Arrangement.StbSecSingle[j] = null;
                                    }
                                }
                            }
                            else if (arrtype.Contains("千鳥配筋"))
                            {
                                sw.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                                sw.StbSecBar_Arrangement.StbSecZigzag = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecZigzag_Class[2];
                                for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecZigzag.Length; ++j)
                                {
                                    sw.StbSecBar_Arrangement.StbSecZigzag[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecZigzag_Class()
                                    {
                                        pos = (j == 0 ? "VERTICAL" : "HORIZONTAL"),
                                        strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                        D = GetParameter_D(instances[i].WallType, SetFamily.Wall.D[j], SetFamily.Wall.D2[j]),
                                        pitch = GetParameter_double(instances[i].WallType, SetFamily.Wall.pitch[j]),
                                    };

                                    if (sw.StbSecBar_Arrangement.StbSecZigzag[j].D == "" ||
                                        sw.StbSecBar_Arrangement.StbSecZigzag[j].pitch < 0.001)
                                    {
                                        sw.StbSecBar_Arrangement.StbSecZigzag[j] = null;
                                    }
                                }
                            }
                            else if (arrtype.Contains("ダブル配筋"))
                            {
                                sw.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                                sw.StbSecBar_Arrangement.StbSecDouble_Net = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecDouble_Net_Class[2];
                                for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecDouble_Net.Length; ++j)
                                {
                                    sw.StbSecBar_Arrangement.StbSecDouble_Net[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecDouble_Net_Class()
                                    {
                                        pos = (j == 0 ? "VERTICAL" : "HORIZONTAL"),
                                        strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                        D = GetParameter_D(instances[i].WallType, SetFamily.Wall.D[j], SetFamily.Wall.D2[j]),
                                        pitch = GetParameter_double(instances[i].WallType, SetFamily.Wall.pitch[j]),
                                    };

                                    if (sw.StbSecBar_Arrangement.StbSecDouble_Net[j].D == "" ||
                                        sw.StbSecBar_Arrangement.StbSecDouble_Net[j].pitch < 0.001)
                                    {
                                        sw.StbSecBar_Arrangement.StbSecDouble_Net[j] = null;
                                    }
                                }
                            }
                            else if (arrtype.Contains("ダブル配筋（内外異なる）"))
                            {
                                sw.StbSecBar_Arrangement.StbSecBar_ArrangementType = 4;
                                sw.StbSecBar_Arrangement.StbSecInside_And_Outside = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecInside_And_Outside_Class[11];
                                for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecInside_And_Outside.Length; ++j)
                                {
                                    sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecInside_And_Outside_Class()
                                    {
                                        strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                        D = GetParameter_D(instances[i].WallType, SetFamily.Wall.D_inout[j], SetFamily.Wall.D2_inout[j]),
                                        pitch = GetParameter_double(instances[i].WallType, SetFamily.Wall.pitch[j]),
                                    };

                                    int j1 = j / 3;
                                    int j2 = j % 3;
                                    switch (j1)
                                    {
                                        case 0:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos = "VERTICAL_OUTSIDE";
                                            break;
                                        case 1:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos = "VERTICAL_INSIDE";
                                            break;
                                        case 2:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos = "HORIZONTAL_OUTSIDE";
                                            break;
                                        case 3:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos = "HORIZONTAL_INSIDE";
                                            break;
                                    }

                                    switch (j2)
                                    {
                                        case 0:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos2 = "TOP_START";
                                            break;
                                        case 1:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos2 = "MIDDLE";
                                            break;
                                        case 2:
                                            sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pos2 = "BOTTOM_END";
                                            break;
                                    }

                                    if (sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].D == "" ||
                                        sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j].pitch < 0.001)
                                    {
                                        sw.StbSecBar_Arrangement.StbSecInside_And_Outside[j] = null;
                                    }
                                }
                            }

                            sw.StbSecBar_Arrangement.StbSecWallEdge = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecWallEdge_Class[4];
                            for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecWallEdge.Length; ++j)
                            {
                                sw.StbSecBar_Arrangement.StbSecWallEdge[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecWallEdge_Class()
                                {
                                    strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                    D = GetParameter_string(instances[i].WallType, SetFamily.Wall.D_Edge[j]),
                                    count = GetParameter_int(instances[i].WallType, SetFamily.Wall.count_Edge[j]),
                                };

                                switch (j)
                                {
                                    case 0: sw.StbSecBar_Arrangement.StbSecWallEdge[j].pos = "VERTICAL_START"; break;
                                    case 1: sw.StbSecBar_Arrangement.StbSecWallEdge[j].pos = "VERTICAL_END"; break;
                                    case 2: sw.StbSecBar_Arrangement.StbSecWallEdge[j].pos = "HORIZONTAL_BOTTOM"; break;
                                    case 3: sw.StbSecBar_Arrangement.StbSecWallEdge[j].pos = "HORIZONTAL_TOP"; break;
                                }

                                if (sw.StbSecBar_Arrangement.StbSecWallEdge[j].D == "" ||
                                    sw.StbSecBar_Arrangement.StbSecWallEdge[j].count == 0)
                                {
                                    sw.StbSecBar_Arrangement.StbSecWallEdge[j] = null;
                                }
                            }

                            if (opens2.Count > 0)
                            {
                                sw.StbSecBar_Arrangement.StbSecOpen_Wall = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class[3];
                                for (int j = 0; j < sw.StbSecBar_Arrangement.StbSecOpen_Wall.Length; ++j)
                                {
                                    sw.StbSecBar_Arrangement.StbSecOpen_Wall[j] = new StbSecWall_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class()
                                    {
                                        strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength),
                                        D = GetParameter_string(instances[i].WallType, SetFamily.Wall.D_op[j]),
                                        count = GetParameter_int(instances[i].WallType, SetFamily.Wall.count_op[j]),
                                        length = GetParameter_double(instances[i].WallType, SetFamily.Wall.length_op[j]),
                                    };

                                    switch (j)
                                    {
                                        case 0: sw.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "VERTICAL"; break;
                                        case 1: sw.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "HORIZONTAL"; break;
                                        case 2: sw.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "DIAGONAL"; break;
                                    }

                                    if (sw.StbSecBar_Arrangement.StbSecOpen_Wall[j].D == "" ||
                                        sw.StbSecBar_Arrangement.StbSecOpen_Wall[j].count == 0)
                                    {
                                        sw.StbSecBar_Arrangement.StbSecOpen_Wall[j] = null;
                                    }
                                }
                            }

                            stb.StbModel.StbSections.StbSecWalls_RC.Add(sw);
                            sect.Add(instances[i].WallType.Id, id_sect);
                            break;

                        case 1: //パラペット
                            break;
                    }

                    #endregion
                }
                else
                {
                    w.id_section = sect[instances[i].WallType.Id];
                }


                w.name               = GetParameter_string(instances[i], SetFamily.Wall.NameMembers);
                w.kind_layout        = GetParameter_string(instances[i], SetFamily.Wall.kind_layout);
                w.thickness_ex_right = GetParameter_double(instances[i], SetFamily.Wall.thickness_ex_right);
                w.thickness_ex_left  = GetParameter_double(instances[i], SetFamily.Wall.thickness_ex_left);
                w.kind_wall          = GetParameter_string(instances[i], SetFamily.Wall.kind_wall);
                w.slit_upper         = GetParameter_double(instances[i], SetFamily.Wall.slit_upper);
                w.slit_bottom        = GetParameter_double(instances[i], SetFamily.Wall.slit_bottom);
                w.slit_right         = GetParameter_double(instances[i], SetFamily.Wall.slit_right);
                w.slit_left          = GetParameter_double(instances[i], SetFamily.Wall.slit_left);
                w.type_outside       = GetParameter_string(instances[i], SetFamily.Wall.type_outside);
                w.isPress = GetParameter_bool(instances[i], SetFamily.Wall.isPress);


                for (int p = 0; p < points.Count; ++p)
                {
                    w.StbNodeid_List.Add
                    (
                        new StbNodeid()
                        {
                            id = GetNodeId(points[p]),
                        }
                    );
                }

                if (opens2.Count > 0)
                {
                    #region 開口

                    //最初の点を通り壁に垂直な平面
                    XYZ normal = (points[1] - points[0]).Normalize();
                    double kd = -(normal.X * points[0].X + normal.Y * points[0].Y + normal.Z * points[0].Z);

                    for (int op = 0; op < opens2.Count; ++op)
                    {
                        XYZ p0 = Commons.ft2mm(opens2[op].BoundaryRect[0]);
                        XYZ p1 = Commons.ft2mm(opens2[op].BoundaryRect[1]);

                        XYZ pa = new XYZ(p0.X, p0.Y, points[0].Z);
                        XYZ pb = new XYZ(p1.X, p1.Y, points[0].Z);

                        XYZ p2 = (points[0].DistanceTo(pa) < points[0].DistanceTo(pb) ? pa : pb);
                        XYZ p3 = new XYZ(p1.X, p1.Y, p0.Z);

                        double position_X = Math.Abs(normal.X * p2.X + normal.Y * p2.Y + normal.Z * p2.Z + kd);


                        id++;
                        StbOpen o = new StbOpen()
                        {
                            id = id,
                            name = "",
                            position_X = position_X,
                            position_Y = p0.Z - points[0].Z,
                            length_X = p0.DistanceTo(p3),
                            length_Y = p1.Z - p0.Z,
                            rotate = 0,
                        };

                        StbSecOpen_RC so = new StbSecOpen_RC()
                        {
                            name = "",
                            StbSecBar_Arrangement = new StbSecOpen_RC.StbSecBar_ArrangementClass(),
                        };

                        so.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                        so.StbSecBar_Arrangement.StbSecOpen_Wall = new StbSecOpen_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class[3];
                        for (int j = 0; j < so.StbSecBar_Arrangement.StbSecOpen_Wall.Length; ++j)
                        {
                            so.StbSecBar_Arrangement.StbSecOpen_Wall[j] = new StbSecOpen_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class();

                            switch (j)
                            {
                                case 0:
                                    so.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "VERTICAL";
                                    break;
                                case 1:
                                    so.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "HORIZONTAL";
                                    break;
                                case 2:
                                    so.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos = "DIAGONAL";
                                    break;
                            }

                            so.StbSecBar_Arrangement.StbSecOpen_Wall[j].strength = GetParameter_string(instances[i].WallType, SetFamily.Wall.strength);
                            so.StbSecBar_Arrangement.StbSecOpen_Wall[j].D = GetParameter_string(instances[i].WallType, SetFamily.Wall.D_op[j]);
                            so.StbSecBar_Arrangement.StbSecOpen_Wall[j].count = GetParameter_int(instances[i].WallType, SetFamily.Wall.count_op[j]);
                            so.StbSecBar_Arrangement.StbSecOpen_Wall[j].length = GetParameter_double(instances[i].WallType, SetFamily.Wall.length_op[j]);

                            if (so.StbSecBar_Arrangement.StbSecOpen_Wall[j].D == "" ||
                                so.StbSecBar_Arrangement.StbSecOpen_Wall[j].count == 0)
                            {
                                so.StbSecBar_Arrangement.StbSecOpen_Wall[j] = null;
                            }
                        }

                        o.id_section = -1;
                        so.id = -1;

                        //同一開口断面の有無を調べる
                        for (int k = 0; k < stb.StbModel.StbSections.StbSecOpens_RC.Count; ++k)
                        {
                            StbSecOpen_RC so2 = stb.StbModel.StbSections.StbSecOpens_RC[k];
                            if (so2.StbSecBar_Arrangement.StbSecOpen_Wall == null) continue;

                            if (so.name == so2.name)
                            {
                                if (so.StbSecBar_Arrangement.StbSecOpen_Wall.Length == so2.StbSecBar_Arrangement.StbSecOpen_Wall.Length)
                                {
                                    bool isSame2 = true;
                                    for (int j = 0; j < so.StbSecBar_Arrangement.StbSecOpen_Wall.Length; ++j)
                                    {
                                        if (so.StbSecBar_Arrangement.StbSecOpen_Wall[j] == null &&
                                            so2.StbSecBar_Arrangement.StbSecOpen_Wall[j] == null)
                                        {
                                            continue;
                                        }
                                        else if (so.StbSecBar_Arrangement.StbSecOpen_Wall[j] == null ||
                                                 so2.StbSecBar_Arrangement.StbSecOpen_Wall[j] == null)
                                        {
                                            //片方だけnull
                                            isSame2 = false;
                                            break;
                                        }

                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos == so2.StbSecBar_Arrangement.StbSecOpen_Wall[j].pos);
                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Wall[j].D == so2.StbSecBar_Arrangement.StbSecOpen_Wall[j].D);
                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Wall[j].count == so2.StbSecBar_Arrangement.StbSecOpen_Wall[j].count);
                                        isSame2 &= (Math.Abs(so.StbSecBar_Arrangement.StbSecOpen_Wall[j].length - so2.StbSecBar_Arrangement.StbSecOpen_Wall[j].length) < 0.1);

                                        if (!isSame2)
                                        {
                                            break;
                                        }
                                    }

                                    if (isSame2)
                                    {
                                        o.id_section = so2.id;
                                        break;
                                    }
                                }
                            }
                        }
                        if (o.id_section < 0)
                        {
                            id_sect++;
                            so.id = id_sect;
                            o.id_section = id_sect;

                            stb.StbModel.StbSections.StbSecOpens_RC.Add(so);
                        }

                        w.StbOpens.Add(o);
                    }

                    #endregion
                }


                stb.StbModel.StbMembers.StbWalls.Add(w);
                AddLog(LogCode.wall, instances[i], w.id, w.id_section);
            }
        }





        /// <summary>
        /// 床の出力
        /// </summary>
        private static void Export_Slab()
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            LogicalOrFilter filter = new LogicalOrFilter(filter1, filter2);
            List<Floor> instances = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Floor>().ToList();

            collector = new FilteredElementCollector(Commons.doc);
            filter1 = new ElementCategoryFilter(BuiltInCategory.OST_FloorOpening);
            List<Opening> opens = collector.WherePasses(filter1).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList();


            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();

            for (int i = 0; i < instances.Count; ++i)
            {
                if (!Check_Analytical_Model(instances[i]))
                {
                    AddWarning(-2, instances[i]);
                    continue;
                }

                List<Opening> opens2 = opens.Where(x => x.Host.Id == instances[i].Id).ToList();
                (List<XYZ> points, List<XYZ> points2) = Data.GetSlabCoord2(instances[i]);
                if (points == null)
                {
                    Data.AddWarning(-4, instances[i]);
                    continue;
                }
                if (points.Count <= 2)
                {
                    AddWarning(-1, instances[i]);
                    continue;
                }

                id++;

                StbSlab s = new StbSlab()
                {
                    id = id,
                    name = GetParameter_string(instances[i], SetFamily.Slab.NameMembers),
                    kind_structure = "RC",
                    kind_slab = GetParameter_string(instances[i], SetFamily.Slab.kind_slab),
                    //level = GetParameter_double(instances[i], BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), //レベルからのオフセット。必要なのは節点からのオフセット
                    level = 0,
                    thickness_ex_upper = GetParameter_double(instances[i], SetFamily.Slab.thickness_ex_upper),
                    thickness_ex_bottom = GetParameter_double(instances[i], SetFamily.Slab.thickness_ex_bottom),
                    dir_load = GetParameter_string(instances[i], SetFamily.Slab.dir_load),
                    angle_load = GetParameter_double(instances[i], SetFamily.Slab.angle_load),
                    isFoundation = GetParameter_bool(instances[i], SetFamily.Slab.isFoundation),
                    type_haunch = GetParameter_string(instances[i], SetFamily.Slab.type_haunch),
                    StbNodeid_List = new List<StbNodeid>(),
                    StbOpens = new List<StbOpen>(),
                };

                if (instances[i].Category.Id.Value() == (long)BuiltInCategory.OST_StructuralFoundation)
                {
                    s.isFoundation = true;
                }

                bool offsetflag = false;
                for (int p = 0; p < points.Count; ++p)
                {
                    s.StbNodeid_List.Add
                    (
                        new StbNodeid()
                        {
                            id = GetNodeId(points[p]),
                        }
                    );

                    XYZ offset = points2[p] - points[p];
                    if (offset.GetLength() > 0)
                    {
                        offsetflag = true;
                    }
                }
                if (offsetflag)
                {
                    s.StbSlabOffset_List = new List<StbSlab.StbSlabOffset>();
                    for (int p = 0; p < points.Count; ++p)
                    {
                        XYZ offset = points2[p] - points[p];
                        s.StbSlabOffset_List.Add(new StbSlab.StbSlabOffset()
                        {
                            offset_X = offset.X,
                            offset_Y = offset.Y,
                        });

                        if (p == 0)
                        {
                            s.level = offset.Z;
                        }
                    }
                }

                if (!sect.ContainsKey(instances[i].FloorType.Id))
                {
                    #region 断面

                    id_sect++;
                    s.id_section = id_sect;

                    //既定の厚さ
                    double depth = GetParameter_double(instances[i].FloorType, BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM);


                    int secslab = 0;
                    string product_type = GetParameter_string(instances[i].FloorType, SetFamily.Slab.product_type);
                    if (product_type == "FLAT" || product_type == "COMPOSITE")
                    {
                        //デッキ
                        secslab = 1;
                    }
                    else
                    {
                        secslab = 0;
                    }

                    string arrtype = GetParameter_string(instances[i].FloorType, SetFamily.Slab.ArrengementType);

                    switch (secslab)
                    {
                        case 0: //RC

                            StbSecSlab_RC RC = new StbSecSlab_RC()
                            {
                                id = id_sect,
                                name = GetParameter_string(instances[i].FloorType, SetFamily.Slab.name),
                                //isFoundation = GetParameter_bool(instances[i].FloorType, SetFamily.Slab.isFoundation), //インスタンスパラメータにしかない。STBが配置断面両方にある。
                                isEarthen = GetParameter_bool(instances[i].FloorType, SetFamily.Slab.isEarthen),
                                isCanti = (GetParameter_string(instances[i].FloorType, SetFamily.Slab.isCanti) == "片持ち"),
                                strength_concrete = GetConcreteFC(GetParameter_string(instances[i].FloorType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM)),
                                depth_cover_top = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_cover_top),
                                depth_cover_bottom = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_cover_bottom),
                                StbSecFigure = new StbSecSlab_RC.StbSecFigureClass(),
                                StbSecBar_Arrangement = new StbSecSlab_RC.StbSecBar_ArrangementClass(),
                            };

                            #region 形状

                            double depth_center = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_center);
                            double depth_base = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_base);
                            double depth_tip = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_tip);
                            double length_haunch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.length_haunch);

                            if (length_haunch > 0.001)
                            {
                                //ハンチ
                                RC.StbSecFigure.StbSecFigureType = 3;
                                RC.StbSecFigure.StbSecHaunch = new StbSecSlab_RC.StbSecFigureClass.StbSecHaunchClass()
                                {
                                    depth_base = depth_base,
                                    depth_center = depth_center,
                                    length_haunch = length_haunch,
                                };
                            }
                            else if (depth_tip > 0.001)
                            {
                                //テーパー
                                RC.StbSecFigure.StbSecFigureType = 2;
                                RC.StbSecFigure.StbSecTaper = new StbSecSlab_RC.StbSecFigureClass.StbSecTaperClass()
                                {
                                    depth_base = depth_base,
                                    depth_tip = depth_tip,
                                };
                            }
                            else
                            {
                                //ストレート
                                RC.StbSecFigure.StbSecFigureType = 1;
                                RC.StbSecFigure.StbSecStraight = new StbSecSlab_RC.StbSecFigureClass.StbSecStraightClass()
                                {
                                    depth = depth,
                                };
                            }

                            #endregion

                            #region 配筋

                            if (arrtype == "標準スラブ配筋")
                            {
                                RC.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                                RC.StbSecBar_Arrangement.StbSecStandard_Slab = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class[12];
                                for (int j = 0; j < RC.StbSecBar_Arrangement.StbSecStandard_Slab.Length; ++j)
                                {
                                    RC.StbSecBar_Arrangement.StbSecStandard_Slab[j] = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class()
                                    {
                                        strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                        D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[j], SetFamily.Slab.D2[j]),
                                        pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[j]),
                                    };

                                    if (RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].D == "" ||
                                        RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pitch < 0.001)
                                    {
                                        RC.StbSecBar_Arrangement.StbSecStandard_Slab[j] = null;
                                    }
                                    else
                                    {
                                        switch (j)
                                        {
                                            case  0: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_TOP_COLUMN"; break;
                                            case  1: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHROT_TOP_MID_END"; break;
                                            case  2: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_TOP_MID_CENTER"; break;
                                            case  3: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_BOTTOM_COLUMN"; break;
                                            case  4: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHROT_BOTTOM_MID_END"; break;
                                            case  5: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_BOTTOM_MID_CENTER"; break;
                                            case  6: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_COLUMN"; break;
                                            case  7: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_MID_END"; break;
                                            case  8: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_MID_CENTER"; break;
                                            case  9: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_COLUMN"; break;
                                            case 10: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_MID_END"; break;
                                            case 11: RC.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_MID_CENTER"; break;
                                        }
                                    }
                                }
                            }
                            else if (arrtype == "2方向スラブ配筋")
                            {
                                RC.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                                RC.StbSecBar_Arrangement.StbSec2Way_Slab = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class[4];
                                for (int j = 0; j < RC.StbSecBar_Arrangement.StbSec2Way_Slab.Length; ++j)
                                {
                                    int[] ind = new int[0];
                                    switch (j)
                                    {
                                        case 0:
                                            ind = new int[] { 0, 1, 2 };
                                            break;
                                        case 1:
                                            ind = new int[] { 3, 4, 5 };
                                            break;
                                        case 2:
                                            ind = new int[] { 6, 7, 8 };
                                            break;
                                        case 3:
                                            ind = new int[] { 9, 10, 11 };
                                            break;
                                        default:
                                            continue;
                                    }

                                    for (int k = 0; k < ind.Length; ++k)
                                    {
                                        RC.StbSecBar_Arrangement.StbSec2Way_Slab[j] = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class()
                                        {
                                            strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                            D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[ind[k]], SetFamily.Slab.D2[ind[k]]),
                                            pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[ind[k]]),
                                        };

                                        if (RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].D == "" ||
                                            RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].pitch < 0.001)
                                        {
                                            RC.StbSecBar_Arrangement.StbSec2Way_Slab[j] = null;
                                        }
                                        else
                                        {
                                            switch (j)
                                            {
                                                case 0: RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "SHORT_TOP"; break;
                                                case 1: RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "SHORT_BOTTOM"; break;
                                                case 2: RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "LONG_TOP"; break;
                                                case 3: RC.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "LONG_BOTTOM"; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (arrtype == "1方向スラブ1配筋")
                            {
                                RC.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                                RC.StbSecBar_Arrangement.StbSec1Way_Slab_1 = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_1_Class[4];
                                for (int j = 0; j < RC.StbSecBar_Arrangement.StbSec1Way_Slab_1.Length; ++j)
                                {
                                    int[] ind = new int[0];
                                    switch (j)
                                    {
                                        case 0:
                                            ind = new int[] { 0, 1, 2 };
                                            break;
                                        case 1:
                                            ind = new int[] { 3, 4, 5 };
                                            break;
                                        case 2:
                                            ind = new int[] { 6, 7, 8 };
                                            break;
                                        case 3:
                                            ind = new int[] { 9, 10, 11 };
                                            break;
                                        default:
                                            continue;
                                    }

                                    for (int k = 0; k < ind.Length; ++k)
                                    {
                                        RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j] = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_1_Class()
                                        {
                                            strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                            D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[ind[k]], SetFamily.Slab.D2[ind[k]]),
                                            pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[ind[k]]),
                                        };

                                        if (RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].D == "" ||
                                            RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].pitch < 0.001)
                                        {
                                            RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j] = null;
                                        }
                                        else
                                        {
                                            switch (j)
                                            {
                                                case 0: RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].pos = "MAIN_TOP"; break;
                                                case 1: RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].pos = "MAIN_BOTTOM"; break;
                                                case 2: RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].pos = "TRANSVERS_TOP"; break;
                                                case 3: RC.StbSecBar_Arrangement.StbSec1Way_Slab_1[j].pos = "TRANSVERS_BOTTOM"; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (arrtype == "1方向スラブ2配筋")
                            {
                                RC.StbSecBar_Arrangement.StbSecBar_ArrangementType = 4;
                                RC.StbSecBar_Arrangement.StbSec1Way_Slab_2 = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_2_Class[6];
                                for (int j = 0; j < RC.StbSecBar_Arrangement.StbSec1Way_Slab_2.Length; ++j)
                                {
                                    int[] ind = new int[0];
                                    switch (j)
                                    {
                                        case 1:
                                            ind = new int[] { 4 };
                                            break;
                                        case 3:
                                            ind = new int[] { 5 };
                                            break;
                                        case 4:
                                            ind = new int[] { 7, 8 };
                                            break;
                                        case 5:
                                            ind = new int[] { 10, 11 };
                                            break;
                                        default:
                                            ind = new int[] { j };
                                            break;
                                    }

                                    for (int k = 0; k < ind.Length; ++k)
                                    {
                                        RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j] = new StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_2_Class()
                                        {
                                            strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                            D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[ind[k]], SetFamily.Slab.D2[ind[k]]),
                                            pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[ind[k]]),
                                        };

                                        if (RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].D == "" ||
                                            RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pitch < 0.001)
                                        {
                                            RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j] = null;
                                        }
                                        else
                                        {
                                            switch (j)
                                            {
                                                case 0: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "MAIN_BASE_TOP"; break;
                                                case 1: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "MAIN_BASE_BOTTOM"; break;
                                                case 2: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "MAIN_TIP_TOP"; break;
                                                case 3: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "MAIN_TIP_BOTTOM"; break;
                                                case 4: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "TRANSVERS_TOP"; break;
                                                case 5: RC.StbSecBar_Arrangement.StbSec1Way_Slab_2[j].pos = "TRANSVERS_BOTTOM"; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            #endregion


                            stb.StbModel.StbSections.StbSecSlabs_RC.Add(RC);
                            sect.Add(instances[i].FloorType.Id, id_sect);

                            break;

                        case 1: //デッキ

                            StbSecSlab_Deck deck = new StbSecSlab_Deck()
                            {
                                id = id_sect,
                                name = GetParameter_string(instances[i].FloorType, SetFamily.Slab.name),
                                product_type = product_type,
                                strength_concrete = GetConcreteFC(GetParameter_string(instances[i].FloorType, BuiltInParameter.STRUCTURAL_MATERIAL_PARAM)),
                                depth_concrete = depth,
                                depth_cover_top = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_cover_top),
                                depth_cover_bottom = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_cover_bottom),
                                StbSecBar_Arrangement = new StbSecSlab_Deck.StbSecBar_ArrangementClass(),
                            };

                            #region 配筋

                            if (arrtype == "標準スラブ配筋")
                            {
                                deck.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                                deck.StbSecBar_Arrangement.StbSecStandard_Slab = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class[12];
                                for (int j = 0; j < deck.StbSecBar_Arrangement.StbSecStandard_Slab.Length; ++j)
                                {
                                    deck.StbSecBar_Arrangement.StbSecStandard_Slab[j] = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class()
                                    {
                                        strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                        D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[j], SetFamily.Slab.D2[j]),
                                        pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[j]),
                                    };

                                    if (deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].D == "" ||
                                        deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pitch < 0.001)
                                    {
                                        deck.StbSecBar_Arrangement.StbSecStandard_Slab[j] = null;
                                    }
                                    else
                                    {
                                        switch (j)
                                        {
                                            case  0: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_TOP_COLUMN"; break;
                                            case  1: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHROT_TOP_MID_END"; break;
                                            case  2: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_TOP_MID_CENTER"; break;
                                            case  3: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_BOTTOM_COLUMN"; break;
                                            case  4: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHROT_BOTTOM_MID_END"; break;
                                            case  5: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "SHORT_BOTTOM_MID_CENTER"; break;
                                            case  6: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_COLUMN"; break;
                                            case  7: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_MID_END"; break;
                                            case  8: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_TOP_MID_CENTER"; break;
                                            case  9: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_COLUMN"; break;
                                            case 10: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_MID_END"; break;
                                            case 11: deck.StbSecBar_Arrangement.StbSecStandard_Slab[j].pos = "LONG_BOTTOM_MID_CENTER"; break;
                                        }
                                    }
                                }
                            }
                            else if (arrtype == "2方向スラブ配筋")
                            {
                                deck.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                                deck.StbSecBar_Arrangement.StbSec2Way_Slab = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class[4];
                                for (int j = 0; j < deck.StbSecBar_Arrangement.StbSec2Way_Slab.Length; ++j)
                                {
                                    int[] ind = new int[0];
                                    switch (j)
                                    {
                                        case 0:
                                            ind = new int[] { 0, 1, 2 };
                                            break;
                                        case 1:
                                            ind = new int[] { 3, 4, 5 };
                                            break;
                                        case 2:
                                            ind = new int[] { 6, 7, 8 };
                                            break;
                                        case 3:
                                            ind = new int[] { 9, 10, 11 };
                                            break;
                                        default:
                                            continue;
                                    }

                                    for (int k = 0; k < ind.Length; ++k)
                                    {
                                        deck.StbSecBar_Arrangement.StbSec2Way_Slab[j] = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class()
                                        {
                                            strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                            D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[ind[k]], SetFamily.Slab.D2[ind[k]]),
                                            pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[ind[k]]),
                                        };

                                        if (deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].D == "" ||
                                            deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].pitch < 0.001)
                                        {
                                            deck.StbSecBar_Arrangement.StbSec2Way_Slab[j] = null;
                                        }
                                        else
                                        {
                                            switch (j)
                                            {
                                                case 0: deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "SHORT_TOP"; break;
                                                case 1: deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "SHORT_BOTTOM"; break;
                                                case 2: deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "LONG_TOP"; break;
                                                case 3: deck.StbSecBar_Arrangement.StbSec2Way_Slab[j].pos = "LONG_BOTTOM"; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (arrtype == "1方向スラブ配筋")
                            {
                                deck.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                                deck.StbSecBar_Arrangement.StbSec1Way_Slab = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec1Way_Slab_Class[5];
                                for (int j = 0; j < deck.StbSecBar_Arrangement.StbSec1Way_Slab.Length; ++j)
                                {
                                    int[] ind = new int[0];
                                    switch (j)
                                    {
                                        case 0:
                                            ind = new int[] { 0, 1, 2 };
                                            break;
                                        case 1:
                                            ind = new int[] { 3, 4, 5 };
                                            break;
                                        case 2:
                                            ind = new int[] { 6, 7, 8 };
                                            break;
                                        case 3:
                                            ind = new int[] { 9, 10, 11 };
                                            break;

                                        case 4:
                                            ind = null;
                                            break;

                                        default:
                                            continue;
                                    }

                                    if (j == 4)
                                    {
                                        //耐火補強筋
                                        deck.StbSecBar_Arrangement.StbSec1Way_Slab[j] = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec1Way_Slab_Class()
                                        {
                                            pos = "REFRACTORY",
                                            strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                            D = GetParameter_string(instances[i].FloorType, SetFamily.Slab.addD),
                                            pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.addpitch),
                                        };

                                        if (deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].D == "" ||
                                            deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pitch < 0.001)
                                        {
                                            deck.StbSecBar_Arrangement.StbSec1Way_Slab[j] = null;
                                        }
                                    }
                                    else
                                    {
                                        for (int k = 0; k < ind.Length; ++k)
                                        {
                                            deck.StbSecBar_Arrangement.StbSec1Way_Slab[j] = new StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec1Way_Slab_Class()
                                            {
                                                strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength),
                                                D = GetParameter_D(instances[i].FloorType, SetFamily.Slab.D1[ind[k]], SetFamily.Slab.D2[ind[k]]),
                                                pitch = GetParameter_double(instances[i].FloorType, SetFamily.Slab.pitch[ind[k]]),
                                            };

                                            if (deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].D == "" ||
                                                deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pitch < 0.001)
                                            {
                                                deck.StbSecBar_Arrangement.StbSec1Way_Slab[j] = null;
                                            }
                                            else
                                            {
                                                switch (j)
                                                {
                                                    case 0: deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pos = "MAIN_TOP"; break;
                                                    case 1: deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pos = "MAIN_BOTTOM"; break;
                                                    case 2: deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pos = "TRANSVERS_TOP"; break;
                                                    case 3: deck.StbSecBar_Arrangement.StbSec1Way_Slab[j].pos = "TRANSVERS_BOTTOM"; break;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region デッキ製品

                            deck.StbSecDeck_Product = new StbSecSlab_Deck.StbSecDeck_ProductClass()
                            {
                                product_company = GetParameter_string(instances[i].FloorType, SetFamily.Slab.product_company),
                                product_code = GetParameter_string(instances[i].FloorType, SetFamily.Slab.product_code),
                                deck_depth = GetParameter_double(instances[i].FloorType, SetFamily.Slab.depth_center),
                            };

                            #endregion


                            stb.StbModel.StbSections.StbSecSlabs_Deck.Add(deck);
                            sect.Add(instances[i].FloorType.Id, id_sect);

                            break;

                        case 2: //既製
                            break;
                    }

                    #endregion
                }
                else
                {
                    s.id_section = sect[instances[i].FloorType.Id];
                }

                if (opens2.Count > 0)
                {
                    #region 開口

                    //[0]→[1]方向をU軸とするローカル座標系への変換行列を求める
                    XYZ Vu = (points[1] - points[0]).Normalize();
                    XYZ Vv = (points[2] - points[0]).Normalize();
                    XYZ Vw = Vu.CrossProduct(Vv).Normalize();
                    Vv = Vw.CrossProduct(Vu).Normalize();

                    double u0 = Vu.X * points[0].X + Vu.Y * points[0].Y + Vu.Z * points[0].Z;
                    double v0 = Vv.X * points[0].X + Vv.Y * points[0].Y + Vv.Z * points[0].Z;
                    double w0 = Vw.X * points[0].X + Vw.Y * points[0].Y + Vw.Z * points[0].Z;
                    XYZ p0 = new XYZ(u0, v0, w0);

                    for (int op = 0; op < opens2.Count; ++op)
                    {
                        //開口座標取得
                        List<XYZ> open_points1 = new List<XYZ>();
                        var ca = opens2[op].BoundaryCurves;
                        if (ca == null) continue;
                        if (ca.Size != 4) continue;

                        foreach (Curve c in ca)
                        {
                            open_points1.Add(Commons.ft2mm(c.GetEndPoint(0)));
                        }

                        //重複除外
                        open_points1 = open_points1.Distinct(new XyzEqualityComparer()).ToList();
                        if (open_points1.Count != 4)
                        {
                            //四角形のみ
                            continue;
                        }

                        //床平面のローカル座標に変換
                        List<XYZ> open_points2 = new List<XYZ>();
                        for (int p = 0; p < open_points1.Count; ++p)
                        {
                            double u = Vu.X * open_points1[p].X + Vu.Y * open_points1[p].Y + Vu.Z * open_points1[p].Z;
                            double v = Vv.X * open_points1[p].X + Vv.Y * open_points1[p].Y + Vv.Z * open_points1[p].Z;
                            double w = Vw.X * open_points1[p].X + Vw.Y * open_points1[p].Y + Vw.Z * open_points1[p].Z;

                            open_points2.Add(new XYZ(u, v, w));
                        }

                        double minY = open_points2.Min(a => a.Y);
                        double minX = open_points2.Where(a => Math.Abs(a.Y - minY) < 0.001).Min(a => a.X);
                        int index = open_points2.FindIndex(a => Math.Abs(a.X - minX) < 0.001 && Math.Abs(a.Y - minY) < 0.001);
                        index = Math.Max(index, 0);

                        bool reverse = (Commons.CalcMenseki(open_points2) < 0);

                        //床の[0]点目に近いものから始まるように並び替える
                        List<XYZ> open_points3 = new List<XYZ>(open_points2.Count);
                        for (int p = 0; p < open_points2.Count; ++p)
                        {
                            int pp = p + index;
                            if (pp >= open_points2.Count) pp = pp - open_points2.Count;

                            if (p == 0 || !reverse)
                            {
                                open_points3.Add(open_points2[pp]);
                            }
                            else
                            {
                                open_points3.Insert(1, open_points2[pp]);
                            }
                        }

                        XYZ vec = (open_points3[1] - open_points3[0]).Normalize();

                        id++;
                        StbOpen o = new StbOpen()
                        {
                            id = id,
                            name = "",
                            position_X = (open_points3[0].X - p0.X),
                            position_Y = (open_points3[0].Y - p0.Y),
                            length_X = open_points3[0].DistanceTo(open_points3[1]),
                            length_Y = open_points3[0].DistanceTo(open_points3[3]),
                            rotate = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ) / Math.PI * 180,
                        };

                        StbSecOpen_RC so = new StbSecOpen_RC()
                        {
                            name = "",
                            StbSecBar_Arrangement = new StbSecOpen_RC.StbSecBar_ArrangementClass(),
                        };

                        so.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                        so.StbSecBar_Arrangement.StbSecOpen_Slab = new StbSecOpen_RC.StbSecBar_ArrangementClass.StbSecOpen_Slab_Class[6];
                        for (int j = 0; j < so.StbSecBar_Arrangement.StbSecOpen_Slab.Length; ++j)
                        {
                            so.StbSecBar_Arrangement.StbSecOpen_Slab[j] = new StbSecOpen_RC.StbSecBar_ArrangementClass.StbSecOpen_Slab_Class();

                            switch (j)
                            {
                                case 0: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "X_TOP"; break;
                                case 1: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "X_BOTTOM"; break;
                                case 2: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "Y_TOP"; break;
                                case 3: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "Y_BOTTOM"; break;
                                case 4: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "DIAGONAL_TOP"; break;
                                case 5: so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos = "DIAGONAL_BOTTOM"; break;
                            }

                            so.StbSecBar_Arrangement.StbSecOpen_Slab[j].strength = GetParameter_string(instances[i].FloorType, SetFamily.Slab.strength);
                            so.StbSecBar_Arrangement.StbSecOpen_Slab[j].D = GetParameter_string(instances[i].FloorType, SetFamily.Slab.D_op[j]);
                            so.StbSecBar_Arrangement.StbSecOpen_Slab[j].count = GetParameter_int(instances[i].FloorType, SetFamily.Slab.count_op[j]);
                            so.StbSecBar_Arrangement.StbSecOpen_Slab[j].length = GetParameter_double(instances[i].FloorType, SetFamily.Slab.length_op[j]);

                            if (so.StbSecBar_Arrangement.StbSecOpen_Slab[j].D == "" ||
                                so.StbSecBar_Arrangement.StbSecOpen_Slab[j].count == 0)
                            {
                                so.StbSecBar_Arrangement.StbSecOpen_Slab[j] = null;
                            }
                        }

                        o.id_section = -1;
                        so.id = -1;

                        //同一開口断面の有無を調べる
                        for (int k = 0; k < stb.StbModel.StbSections.StbSecOpens_RC.Count; ++k)
                        {
                            StbSecOpen_RC so2 = stb.StbModel.StbSections.StbSecOpens_RC[k];
                            if (so2.StbSecBar_Arrangement.StbSecOpen_Slab == null) continue;

                            if (so.name == so2.name)
                            {
                                if (so.StbSecBar_Arrangement.StbSecOpen_Slab.Length == so2.StbSecBar_Arrangement.StbSecOpen_Slab.Length)
                                {
                                    bool isSame2 = true;
                                    for (int j = 0; j < so.StbSecBar_Arrangement.StbSecOpen_Slab.Length; ++j)
                                    {
                                        if (so.StbSecBar_Arrangement.StbSecOpen_Slab[j] == null &&
                                            so2.StbSecBar_Arrangement.StbSecOpen_Slab[j] == null)
                                        {
                                            continue;
                                        }
                                        else if (so.StbSecBar_Arrangement.StbSecOpen_Slab[j] == null ||
                                                 so2.StbSecBar_Arrangement.StbSecOpen_Slab[j] == null)
                                        {
                                            //片方だけnull
                                            isSame2 = false;
                                            break;
                                        }

                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos == so2.StbSecBar_Arrangement.StbSecOpen_Slab[j].pos);
                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Slab[j].D == so2.StbSecBar_Arrangement.StbSecOpen_Slab[j].D);
                                        isSame2 &= (so.StbSecBar_Arrangement.StbSecOpen_Slab[j].count == so2.StbSecBar_Arrangement.StbSecOpen_Slab[j].count);
                                        isSame2 &= (Math.Abs(so.StbSecBar_Arrangement.StbSecOpen_Slab[j].length - so2.StbSecBar_Arrangement.StbSecOpen_Slab[j].length) < 0.1);

                                        if (!isSame2)
                                        {
                                            break;
                                        }
                                    }

                                    if (isSame2)
                                    {
                                        o.id_section = so2.id;
                                        break;
                                    }
                                }
                            }
                        }
                        if (o.id_section < 0)
                        {
                            id_sect++;
                            so.id = id_sect;
                            o.id_section = id_sect;

                            stb.StbModel.StbSections.StbSecOpens_RC.Add(so);
                        }

                        s.StbOpens.Add(o);
                    }


                    #endregion
                }

                stb.StbModel.StbMembers.StbSlabs.Add(s);
                AddLog(LogCode.slab, instances[i], s.id, s.id_section);
            }
        }



        /// <summary>
        /// ブレース断面の出力
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private static int Export_SecBrace(FamilyInstance ins)
        {
            FamilySymbol symbol = ins.Symbol;
            ElementId lvid = ins.LevelId;
            if (lvid.Value() == -1)
            {
                lvid = ins.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
            }

            string floor = Levels.Find(x => x.Id == lvid).Name;

            int retID = -1;

            int n = 1;
            string[] paramName = new string[2];
            string[,] strength = new string[3, 2];
            string familyname = symbol.Family.Name;
            if (familyname == SetFamily.SBraH.FamilyName)
            {
                n = 3;
                paramName[0]   = SetFamily.SBraH.name;
                paramName[1]   = SetFamily.SBraH.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraH.strength_main[0]);
                strength[1, 0] = GetParameter_string(symbol, SetFamily.SBraH.strength_main[1]);
                strength[2, 0] = GetParameter_string(symbol, SetFamily.SBraH.strength_main[2]);
                strength[0, 1] = GetParameter_string(symbol, SetFamily.SBraH.strength_web[0]);
                strength[1, 1] = GetParameter_string(symbol, SetFamily.SBraH.strength_web[1]);
                strength[2, 1] = GetParameter_string(symbol, SetFamily.SBraH.strength_web[2]);
            }
            else if (familyname == SetFamily.SBraBH.FamilyName)
            {
                n = 3;
                paramName[0] = SetFamily.SBraBH.name;
                paramName[1] = SetFamily.SBraBH.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraBH.strength_main[0]);
                strength[1, 0] = GetParameter_string(symbol, SetFamily.SBraBH.strength_main[1]);
                strength[2, 0] = GetParameter_string(symbol, SetFamily.SBraBH.strength_main[2]);
                strength[0, 1] = GetParameter_string(symbol, SetFamily.SBraBH.strength_web[0]);
                strength[1, 1] = GetParameter_string(symbol, SetFamily.SBraBH.strength_web[1]);
                strength[2, 1] = GetParameter_string(symbol, SetFamily.SBraBH.strength_web[2]);
            }
            else if (familyname == SetFamily.SBraBox.FamilyName)
            {
                paramName[0] = SetFamily.SBraBox.name;
                paramName[1] = SetFamily.SBraBox.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraBox.strength);
                strength[1, 0] = "";
                strength[2, 0] = "";
                strength[0, 1] = "";
                strength[1, 1] = "";
                strength[2, 1] = "";
            }
            else if (familyname == SetFamily.SBraBBox.FamilyName)
            {
                paramName[0] = SetFamily.SBraBBox.name;
                paramName[1] = SetFamily.SBraBBox.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraBBox.strength);
                strength[1, 0] = "";
                strength[2, 0] = "";
                strength[0, 1] = "";
                strength[1, 1] = "";
                strength[2, 1] = "";
            }
            else if (familyname == SetFamily.SBraPipe.FamilyName)
            {
                paramName[0] = SetFamily.SBraPipe.name;
                paramName[1] = SetFamily.SBraPipe.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraPipe.strength);
                strength[1, 0] = "";
                strength[2, 0] = "";
                strength[0, 1] = "";
                strength[1, 1] = "";
                strength[2, 1] = "";
            }
            else if (familyname == SetFamily.SBraC.FamilyName)
            {
                n = 3;
                paramName[0] = SetFamily.SBraC.name;
                paramName[1] = SetFamily.SBraC.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraC.strength);
                strength[1, 0] = strength[0, 0];
                strength[2, 0] = strength[0, 0];
                strength[0, 1] = strength[0, 0];
                strength[1, 1] = strength[0, 0];
                strength[2, 1] = strength[0, 0];
            }
            else if (familyname == SetFamily.SBraL.FamilyName)
            {
                n = 3;
                paramName[0] = SetFamily.SBraL.name;
                paramName[1] = SetFamily.SBraL.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraL.strength);
                strength[1, 0] = strength[0, 0];
                strength[2, 0] = strength[0, 0];
                strength[0, 1] = strength[0, 0];
                strength[1, 1] = strength[0, 0];
                strength[2, 1] = strength[0, 0];
            }
            else if (familyname == SetFamily.SBraLipC.FamilyName)
            {
                n = 3;
                paramName[0] = SetFamily.SBraLipC.name;
                paramName[1] = SetFamily.SBraLipC.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraLipC.strength);
                strength[1, 0] = strength[0, 0];
                strength[2, 0] = strength[0, 0];
                strength[0, 1] = strength[0, 0];
                strength[1, 1] = strength[0, 0];
                strength[2, 1] = strength[0, 0];
            }
            else if (familyname == SetFamily.SBraFB.FamilyName)
            {
                paramName[0] = SetFamily.SBraFB.name;
                paramName[1] = SetFamily.SBraFB.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraFB.strength_main);
                strength[1, 0] = "";
                strength[2, 0] = "";
                strength[0, 1] = "";
                strength[1, 1] = "";
                strength[2, 1] = "";
            }
            else if (familyname == SetFamily.SBraRollBar.FamilyName)
            {
                paramName[0] = SetFamily.SBraRollBar.name;
                paramName[1] = SetFamily.SBraRollBar.kind_brace;

                strength[0, 0] = GetParameter_string(symbol, SetFamily.SBraRollBar.strength_main);
                strength[1, 0] = "";
                strength[2, 0] = "";
                strength[0, 1] = "";
                strength[1, 1] = "";
                strength[2, 1] = "";
            }
            else
            {
                return retID;
            }

            string[] shape = new string[n];
            for (int i = 0; i < n; ++i)
            {
                shape[i] = GetSteelName(symbol, 0, i);
            }

            if (n == 3)
            {
                if (shape[0] == "") shape[0] = shape[1];
                if (shape[0] == "") shape[0] = shape[2];
                if (shape[1] == "") shape[1] = shape[0];
                if (shape[2] == "") shape[2] = shape[1];
                if (shape[0] == "") return retID;

                if (shape[0] == shape[1] && shape[1] == shape[2])
                {
                    if (strength[0, 0] == strength[1, 0] && strength[1, 0] == strength[2, 0])
                    {
                        if (strength[0, 1] == strength[1, 1] && strength[1, 1] == strength[2, 1])
                        {
                            //形状、材料が全て一致していればALL
                            n = 1;
                        }
                    }
                }
            }
            else
            {
                if (shape[0] == "") return retID;
            }


            id_sect++;
            retID = id_sect;

            StbSecBrace_S b = new StbSecBrace_S()
            {
                id = id_sect,
                floor = floor,
                name       = GetParameter_string(symbol, paramName[0]),
                kind_brace = GetParameter_string(symbol, paramName[1]),
            };


            b.StbSecSteelBrace = new StbSecBrace_S.StbSecSteelBraceClass[n];
            for (int i = 0; i < b.StbSecSteelBrace.Length; ++i)
            {
                b.StbSecSteelBrace[i] = new StbSecBrace_S.StbSecSteelBraceClass()
                {
                    shape = shape[i],
                    strength_main = strength[i, 0],
                    strength_web  = strength[i, 1],
                };

                if (n == 1)
                {
                    b.StbSecSteelBrace[i].pos = "ALL";
                }
                else
                {
                    switch (i)
                    {
                        case 0: b.StbSecSteelBrace[i].pos = "BOTTOM"; break;
                        case 1: b.StbSecSteelBrace[i].pos = "CENTER"; break;
                        case 2: b.StbSecSteelBrace[i].pos = "TOP"; break;
                    }
                }
            }

            stb.StbModel.StbSections.StbSecBraces_S.Add(b);

            return retID;
        }

        /// <summary>
        /// ブレースの出力
        /// </summary>
        private static void Export_Brace()
        {
            List<string> AllFamilyName = new List<string>();
            for (int i = 0; i < SetFamily.SBraFName.FamilyName.Length; ++i)
            {
                AllFamilyName.AddRange(SetFamily.SBraFName.FamilyName[i]);
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);

            ParameterValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM));
            //Aより大きい or Aより小さい = ≠A
            FilterNumericRuleEvaluator evaluator1 = new FilterNumericLess();
            FilterNumericRuleEvaluator evaluator2 = new FilterNumericGreater();

            FilterRule rule2 = new FilterIntegerRule(provider, evaluator1, (int)StructuralInstanceUsage.Girder);
            FilterRule rule3 = new FilterIntegerRule(provider, evaluator2, (int)StructuralInstanceUsage.Girder);
            FilterRule rule4 = new FilterIntegerRule(provider, evaluator1, (int)StructuralInstanceUsage.Joist);
            FilterRule rule5 = new FilterIntegerRule(provider, evaluator2, (int)StructuralInstanceUsage.Joist);

            ElementParameterFilter filter2 = new ElementParameterFilter(rule2);
            ElementParameterFilter filter3 = new ElementParameterFilter(rule3);
            ElementParameterFilter filter4 = new ElementParameterFilter(rule4);
            ElementParameterFilter filter5 = new ElementParameterFilter(rule5);


            //大梁でない
            LogicalOrFilter filter6 = new LogicalOrFilter(new List<ElementFilter> { filter2, filter3 });
            //小梁でない
            LogicalOrFilter filter7 = new LogicalOrFilter(new List<ElementFilter> { filter4, filter5 });

            //構造フレームで大梁・小梁以外のもの
            LogicalAndFilter filter = new LogicalAndFilter(new List<ElementFilter> { filter1, filter6, filter7 });
            List<FamilyInstance> instances = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => AllFamilyName.Contains(x.Symbol.Family.Name) && !x.Symbol.Family.IsInPlace).ToList();

            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();
            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

            for (int i = 0; i < instances.Count; ++i)
            {
                XYZ ps1 = new XYZ();
                XYZ pe1 = new XYZ();
                XYZ ps2 = GetFramingCoordinate(instances[i], 0);
                XYZ pe2 = GetFramingCoordinate(instances[i], 1);

                if (amanager.HasAssociation(instances[i].Id) && Commons.doc.GetElement(amanager.GetAssociatedElementId(instances[i].Id)) is AnalyticalMember member)
                {
                    ps1 = member.GetCurve().GetEndPoint(0);
                    pe1 = member.GetCurve().GetEndPoint(1);
                }
                else
                {
                    ps1 = ps2;
                    pe1 = pe2;
                }

                ps1 = Commons.ft2mm(ps1);
                pe1 = Commons.ft2mm(pe1);
                ps2 = Commons.ft2mm(ps2);
                pe2 = Commons.ft2mm(pe2);

                StbBrace b = new StbBrace()
                {
                    idNode_start = GetNodeId(ps1),
                    idNode_end   = GetNodeId(pe1),
                    kind_structure = "S",
                };

                if (!sect.ContainsKey(instances[i].Symbol.Id))
                {
                    b.id_section = Export_SecBrace(instances[i]);
                    if (b.id_section < 0) continue;
                    sect.Add(instances[i].Symbol.Id, b.id_section);
                }
                else
                {
                    b.id_section = sect[instances[i].Symbol.Id];
                }

                string[] paramName = new string[8];
                string familyname = instances[i].Symbol.Family.Name;
                if (familyname == SetFamily.SBraH.FamilyName)
                {
                    paramName[0] = SetFamily.SBraH.NameMembers;
                    paramName[1] = SetFamily.SBraH.condition_start;
                    paramName[2] = SetFamily.SBraH.condition_end;
                    paramName[3] = SetFamily.SBraH.future_brace;
                    paramName[4] = SetFamily.SBraH.joint_start;
                    paramName[5] = SetFamily.SBraH.joint_end;
                    paramName[6] = SetFamily.SBraH.kind_joint_start;
                    paramName[7] = SetFamily.SBraH.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraBH.FamilyName)
                {
                    paramName[0] = SetFamily.SBraBH.NameMembers;
                    paramName[1] = SetFamily.SBraBH.condition_start;
                    paramName[2] = SetFamily.SBraBH.condition_end;
                    paramName[3] = SetFamily.SBraBH.future_brace;
                    paramName[4] = SetFamily.SBraBH.joint_start;
                    paramName[5] = SetFamily.SBraBH.joint_end;
                    paramName[6] = SetFamily.SBraBH.kind_joint_start;
                    paramName[7] = SetFamily.SBraBH.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraBox.FamilyName)
                {
                    paramName[0] = SetFamily.SBraBox.NameMembers;
                    paramName[1] = SetFamily.SBraBox.condition_start;
                    paramName[2] = SetFamily.SBraBox.condition_end;
                    paramName[3] = SetFamily.SBraBox.future_brace;
                    paramName[4] = SetFamily.SBraBox.joint_start;
                    paramName[5] = SetFamily.SBraBox.joint_end;
                    paramName[6] = SetFamily.SBraBox.kind_joint_start;
                    paramName[7] = SetFamily.SBraBox.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraBBox.FamilyName)
                {
                    paramName[0] = SetFamily.SBraBBox.NameMembers;
                    paramName[1] = SetFamily.SBraBBox.condition_start;
                    paramName[2] = SetFamily.SBraBBox.condition_end;
                    paramName[3] = SetFamily.SBraBBox.future_brace;
                    paramName[4] = SetFamily.SBraBBox.joint_start;
                    paramName[5] = SetFamily.SBraBBox.joint_end;
                    paramName[6] = SetFamily.SBraBBox.kind_joint_start;
                    paramName[7] = SetFamily.SBraBBox.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraPipe.FamilyName)
                {
                    paramName[0] = SetFamily.SBraPipe.NameMembers;
                    paramName[1] = SetFamily.SBraPipe.condition_start;
                    paramName[2] = SetFamily.SBraPipe.condition_end;
                    paramName[3] = SetFamily.SBraPipe.future_brace;
                    paramName[4] = SetFamily.SBraPipe.joint_start;
                    paramName[5] = SetFamily.SBraPipe.joint_end;
                    paramName[6] = SetFamily.SBraPipe.kind_joint_start;
                    paramName[7] = SetFamily.SBraPipe.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraC.FamilyName)
                {
                    paramName[0] = SetFamily.SBraC.NameMembers;
                    paramName[1] = SetFamily.SBraC.condition_start;
                    paramName[2] = SetFamily.SBraC.condition_end;
                    paramName[3] = SetFamily.SBraC.future_brace;
                    paramName[4] = SetFamily.SBraC.joint_start;
                    paramName[5] = SetFamily.SBraC.joint_end;
                    paramName[6] = SetFamily.SBraC.kind_joint_start;
                    paramName[7] = SetFamily.SBraC.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraL.FamilyName)
                {
                    paramName[0] = SetFamily.SBraL.NameMembers;
                    paramName[1] = SetFamily.SBraL.condition_start;
                    paramName[2] = SetFamily.SBraL.condition_end;
                    paramName[3] = SetFamily.SBraL.future_brace;
                    paramName[4] = SetFamily.SBraL.joint_start;
                    paramName[5] = SetFamily.SBraL.joint_end;
                    paramName[6] = SetFamily.SBraL.kind_joint_start;
                    paramName[7] = SetFamily.SBraL.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraLipC.FamilyName)
                {
                    paramName[0] = SetFamily.SBraLipC.NameMembers;
                    paramName[1] = SetFamily.SBraLipC.condition_start;
                    paramName[2] = SetFamily.SBraLipC.condition_end;
                    paramName[3] = SetFamily.SBraLipC.future_brace;
                    paramName[4] = SetFamily.SBraLipC.joint_start;
                    paramName[5] = SetFamily.SBraLipC.joint_end;
                    paramName[6] = SetFamily.SBraLipC.kind_joint_start;
                    paramName[7] = SetFamily.SBraLipC.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraFB.FamilyName)
                {
                    paramName[0] = SetFamily.SBraFB.NameMembers;
                    paramName[1] = SetFamily.SBraFB.condition_start;
                    paramName[2] = SetFamily.SBraFB.condition_end;
                    paramName[3] = SetFamily.SBraFB.future_brace;
                    paramName[4] = SetFamily.SBraFB.joint_start;
                    paramName[5] = SetFamily.SBraFB.joint_end;
                    paramName[6] = SetFamily.SBraFB.kind_joint_start;
                    paramName[7] = SetFamily.SBraFB.kind_joint_end;
                }
                else if (familyname == SetFamily.SBraRollBar.FamilyName)
                {
                    paramName[0] = SetFamily.SBraRollBar.NameMembers;
                    paramName[1] = SetFamily.SBraRollBar.condition_start;
                    paramName[2] = SetFamily.SBraRollBar.condition_end;
                    paramName[3] = SetFamily.SBraRollBar.future_brace;
                    paramName[4] = SetFamily.SBraRollBar.joint_start;
                    paramName[5] = SetFamily.SBraRollBar.joint_end;
                    paramName[6] = SetFamily.SBraRollBar.kind_joint_start;
                    paramName[7] = SetFamily.SBraRollBar.kind_joint_end;
                }
                else
                {
                    continue;
                }

                id++;
                b.id = id;
                b.rotate = GetParameter_Angle(instances[i], BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);

                //XYZ offset_s = GetFramingOffset(instances[i], 0);
                //XYZ offset_e = GetFramingOffset(instances[i], 1);
                XYZ offset_s = ps2 - ps1;
                XYZ offset_e = pe2 - pe1;

                b.offset_start_X = offset_s.X;
                b.offset_start_Y = offset_s.Y;
                b.offset_start_Z = offset_s.Z;
                b.offset_end_X   = offset_e.X;
                b.offset_end_Y   = offset_e.Y;
                b.offset_end_Z   = offset_e.Z;


                b.name             = GetParameter_string(instances[i], paramName[0]);
                b.condition_start  = GetParameter_string(instances[i], paramName[1]);
                b.condition_end    = GetParameter_string(instances[i], paramName[2]);
                b.future_brace     = GetParameter_string(instances[i], paramName[3]);
                b.joint_start      = GetParameter_double(instances[i], paramName[4]);
                b.joint_end        = GetParameter_double(instances[i], paramName[5]);
                b.kind_joint_start = GetParameter_string(instances[i], paramName[6]);
                b.kind_joint_end   = GetParameter_string(instances[i], paramName[7]);

                stb.StbModel.StbMembers.StbBraces.Add(b);
                AddLog(LogCode.brace, instances[i], b.id, b.id_section);
            }

        }



        /// <summary>
        /// 杭配筋の同一チェック
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareTo_PileBar(StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class a,
                                              StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class b)
        {
            if (a.strength_main_circumference_1st != b.strength_main_circumference_1st) return false;
            if (a.D_main_circumference_1st != b.D_main_circumference_1st) return false;
            if (a.count_main_circumference_1st != b.count_main_circumference_1st) return false;

            if (a.strength_main_core != b.strength_main_core) return false;
            if (a.D_main_core != b.D_main_core) return false;
            if (a.count_main_core != b.count_main_core) return false;

            if (a.strength_band != b.strength_band) return false;
            if (a.D_band != b.D_band) return false;
            if (Math.Abs(a.pitch_band - b.pitch_band) > 0.01) return false;

            return true;
        }

        /// <summary>
        /// 基礎断面の出力
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private static int Export_SecFoundation(FamilyInstance ins)
        {
            FamilySymbol symbol = ins.Symbol;
            int retID = -1;

            string[] paramName = new string[5];
            string familyname = symbol.Family.Name;
            if (familyname == SetFamily.FRect.FamilyName)
            {
                paramName[0] = SetFamily.FRect.name;
                paramName[1] = SetFamily.FRect.strength_concrete;
                paramName[2] = SetFamily.FRect.depth_cover_top;
                paramName[3] = SetFamily.FRect.depth_cover_bottom;
                paramName[4] = SetFamily.FRect.depth_cover_side;
            }
            else if (familyname == SetFamily.FTRect.FamilyName)
            {
                paramName[0] = SetFamily.FTRect.name;
                paramName[1] = SetFamily.FTRect.strength_concrete;
                paramName[2] = SetFamily.FTRect.depth_cover_top;
                paramName[3] = SetFamily.FTRect.depth_cover_bottom;
                paramName[4] = SetFamily.FTRect.depth_cover_side;
            }
            else if (familyname == SetFamily.FTri.FamilyName)
            {
                paramName[0] = SetFamily.FTri.name;
                paramName[1] = SetFamily.FTri.strength_concrete;
                paramName[2] = SetFamily.FTri.depth_cover_top;
                paramName[3] = SetFamily.FTri.depth_cover_bottom;
                paramName[4] = SetFamily.FTri.depth_cover_side;
            }
            else if (familyname == SetFamily.FETriangle.FamilyName)
            {
                paramName[0] = SetFamily.FETriangle.name;
                paramName[1] = SetFamily.FETriangle.strength_concrete;
                paramName[2] = SetFamily.FETriangle.depth_cover_top;
                paramName[3] = SetFamily.FETriangle.depth_cover_bottom;
                paramName[4] = SetFamily.FETriangle.depth_cover_side;
            }
            else if (familyname == SetFamily.FOct.FamilyName)
            {
                paramName[0] = SetFamily.FOct.name;
                paramName[1] = SetFamily.FOct.strength_concrete;
                paramName[2] = SetFamily.FOct.depth_cover_top;
                paramName[3] = SetFamily.FOct.depth_cover_bottom;
                paramName[4] = SetFamily.FOct.depth_cover_side;
            }
            else if (familyname == SetFamily.FConti.FamilyName)
            {
                paramName[0] = SetFamily.FConti.name;
                paramName[1] = SetFamily.FConti.strength_concrete;
                paramName[2] = SetFamily.FConti.depth_cover_top;
                paramName[3] = SetFamily.FConti.depth_cover_bottom;
                paramName[4] = SetFamily.FConti.depth_cover_side;
            }
            else
            {
                return retID;
            }

            id_sect++;
            retID = id_sect;

            StbSecFoundation_RC s = new StbSecFoundation_RC()
            {
                id = id_sect,
                name = GetParameter_string(symbol, paramName[0]),
                strength_concrete = GetConcreteFC(GetParameter_string(symbol, paramName[1])),
                depth_cover_top = GetParameter_double(symbol, paramName[2]),
                depth_cover_bottom = GetParameter_double(symbol, paramName[3]),
                depth_cover_side = GetParameter_double(symbol, paramName[4]),
                StbSecFigure = new StbSecFoundation_RC.StbSecFigureClass(),
                StbSecBar_Arrangement = new StbSecFoundation_RC.StbSecBar_ArrangementClass(),
            };

            if (familyname == SetFamily.FRect.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 1;
                s.StbSecFigure.StbSecRect = new StbSecFoundation_RC.StbSecFigureClass.StbSecRectClass()
                {
                    DX = GetParameter_double(symbol, SetFamily.FRect.DX),
                    DY = GetParameter_double(symbol, SetFamily.FRect.DY),
                    depth = GetParameter_double(symbol, SetFamily.FRect.depth),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                s.StbSecBar_Arrangement.StbSecRect = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecRect.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecRect[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FRect.strength),
                        D = GetParameter_string(symbol, SetFamily.FRect.D[i]),
                        count = GetParameter_int(symbol, SetFamily.FRect.count[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecRect[i].D == "" ||
                        s.StbSecBar_Arrangement.StbSecRect[i].count <= 0)
                    {
                        s.StbSecBar_Arrangement.StbSecRect[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecRect[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }
            else if (familyname == SetFamily.FTRect.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 2;
                s.StbSecFigure.StbSecTapered_Rect = new StbSecFoundation_RC.StbSecFigureClass.StbSecTapered_RectClass()
                {
                    DX         = GetParameter_double(symbol, SetFamily.FTRect.DX),
                    DY         = GetParameter_double(symbol, SetFamily.FTRect.DY),
                    depth_base = GetParameter_double(symbol, SetFamily.FTRect.depth_base),
                    depth_tip  = GetParameter_double(symbol, SetFamily.FTRect.depth_tip),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                s.StbSecBar_Arrangement.StbSecRect = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecRect.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecRect[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FTRect.strength),
                        D        = GetParameter_string(symbol, SetFamily.FTRect.D[i]),
                        count    = GetParameter_int   (symbol, SetFamily.FTRect.count[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecRect[i].D == "" ||
                        s.StbSecBar_Arrangement.StbSecRect[i].count <= 0)
                    {
                        s.StbSecBar_Arrangement.StbSecRect[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecRect[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }
            else if (familyname == SetFamily.FTri.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 3;
                s.StbSecFigure.StbSecTriangle = new StbSecFoundation_RC.StbSecFigureClass.StbSecTriangleClass()
                {
                    DX    = GetParameter_double(symbol, SetFamily.FTri.DX),
                    DY    = GetParameter_double(symbol, SetFamily.FTri.DY),
                    depth = GetParameter_double(symbol, SetFamily.FTri.depth),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                s.StbSecBar_Arrangement.StbSecTriangle = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecTriangleClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecTriangle.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecTriangle[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecTriangleClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FTri.strength),
                        D        = GetParameter_string(symbol, SetFamily.FTri.D[i]),
                        count    = GetParameter_int   (symbol, SetFamily.FTri.count[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecTriangle[i].D == "" ||
                        s.StbSecBar_Arrangement.StbSecTriangle[i].count <= 0)
                    {
                        s.StbSecBar_Arrangement.StbSecTriangle[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecTriangle[i].pos = "MAIN_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecTriangle[i].pos = "MAIN_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecTriangle[i].pos = "TRANSVERS_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecTriangle[i].pos = "TRANSVERS_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecTriangle[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }
            else if (familyname == SetFamily.FETriangle.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 4;
                s.StbSecFigure.StbSecEqiTriangle = new StbSecFoundation_RC.StbSecFigureClass.StbSecEqiTriangleClass()
                {
                    B     = GetParameter_double(symbol, SetFamily.FETriangle.B),
                    C     = GetParameter_double(symbol, SetFamily.FETriangle.C),
                    depth = GetParameter_double(symbol, SetFamily.FETriangle.depth),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                s.StbSecBar_Arrangement.StbSecThreeWay = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecThreeWayClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecThreeWay.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecThreeWay[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecThreeWayClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FETriangle.strength),
                        D        = GetParameter_string(symbol, SetFamily.FETriangle.D[i]),
                        count    = GetParameter_int   (symbol, SetFamily.FETriangle.count[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecThreeWay[i].D == "" ||
                        s.StbSecBar_Arrangement.StbSecThreeWay[i].count <= 0)
                    {
                        s.StbSecBar_Arrangement.StbSecThreeWay[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecThreeWay[i].pos = "MAIN_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecThreeWay[i].pos = "MAIN_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecThreeWay[i].pos = "OUTSIDE_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecThreeWay[i].pos = "OUTSIDE_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecThreeWay[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }
            else if (familyname == SetFamily.FOct.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 5;
                s.StbSecFigure.StbSecOctagon = new StbSecFoundation_RC.StbSecFigureClass.StbSecOctagonClass()
                {
                    DX    = GetParameter_double(symbol, SetFamily.FOct.DX),
                    DY    = GetParameter_double(symbol, SetFamily.FOct.DY),
                    CX1   = GetParameter_double(symbol, SetFamily.FOct.CX1),
                    CY1   = GetParameter_double(symbol, SetFamily.FOct.CY1),
                    CX2   = GetParameter_double(symbol, SetFamily.FOct.CX2),
                    CY2   = GetParameter_double(symbol, SetFamily.FOct.CY2),
                    CX3   = GetParameter_double(symbol, SetFamily.FOct.CX3),
                    CY3   = GetParameter_double(symbol, SetFamily.FOct.CY3),
                    CX4   = GetParameter_double(symbol, SetFamily.FOct.CX4),
                    CY4   = GetParameter_double(symbol, SetFamily.FOct.CY4),
                    depth = GetParameter_double(symbol, SetFamily.FOct.depth),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                s.StbSecBar_Arrangement.StbSecRect = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecRect.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecRect[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FOct.strength),
                        D        = GetParameter_string(symbol, SetFamily.FOct.D[i]),
                        count    = GetParameter_int   (symbol, SetFamily.FOct.count[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecRect[i].D == "" ||
                        s.StbSecBar_Arrangement.StbSecRect[i].count <= 0)
                    {
                        s.StbSecBar_Arrangement.StbSecRect[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecRect[i].pos = "X_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecRect[i].pos = "Y_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecRect[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }
            else if (familyname == SetFamily.FConti.FamilyName)
            {
                s.StbSecFigure.StbSecFigureType = 6;
                s.StbSecFigure.StbSecContinuous = new StbSecFoundation_RC.StbSecFigureClass.StbSecContinuousClass()
                {
                    B          = GetParameter_double(symbol, SetFamily.FConti.B),
                    depth_base = GetParameter_double(symbol, SetFamily.FConti.depth_base),
                    depth_tip  = GetParameter_double(symbol, SetFamily.FConti.depth_tip),
                    type       = GetParameter_string(symbol, SetFamily.FConti.type),
                };

                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 4;
                s.StbSecBar_Arrangement.StbSecContinuous = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecContinuousClass[5];
                for (int i = 0; i < s.StbSecBar_Arrangement.StbSecContinuous.Length; ++i)
                {
                    s.StbSecBar_Arrangement.StbSecContinuous[i] = new StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecContinuousClass()
                    {
                        strength = GetParameter_string(symbol, SetFamily.FConti.strength),
                        D        = GetParameter_string(symbol, SetFamily.FConti.D[i]),
                        count    = GetParameter_int   (symbol, SetFamily.FConti.count[i]),
                        pitch    = GetParameter_double(symbol, SetFamily.FConti.pitch[i]),
                    };

                    if (s.StbSecBar_Arrangement.StbSecContinuous[i].D == "" ||
                        (s.StbSecBar_Arrangement.StbSecContinuous[i].count <= 0 && s.StbSecBar_Arrangement.StbSecContinuous[i].pitch <= 0.01))
                    {
                        s.StbSecBar_Arrangement.StbSecContinuous[i] = null;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: s.StbSecBar_Arrangement.StbSecContinuous[i].pos = "MAIN_TOP"; break;
                            case 1: s.StbSecBar_Arrangement.StbSecContinuous[i].pos = "MAIN_BOTTOM"; break;
                            case 2: s.StbSecBar_Arrangement.StbSecContinuous[i].pos = "TRANSVERS_TOP"; break;
                            case 3: s.StbSecBar_Arrangement.StbSecContinuous[i].pos = "TRANSVERS_BOTTOM"; break;
                            case 4: s.StbSecBar_Arrangement.StbSecContinuous[i].pos = "HORIZONTAL"; break;
                        }
                    }
                }
            }

            stb.StbModel.StbSections.StbSecFoundations_RC.Add(s);

            return retID;
        }

        /// <summary>
        /// 杭断面の出力
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private static int Export_SecPile(FamilyInstance ins)
        {
            FamilySymbol symbol = ins.Symbol;
            int retID = -1;

            bool top = false;
            bool foot = false;
            string[] paramName = new string[12];
            string familyname = symbol.Family.Name;
            List<double> Diameter = new List<double>();
            if (familyname == SetFamily.CastinPile.FamilyName)
            {
                paramName[ 0] = SetFamily.CastinPile.name;
                paramName[ 1] = SetFamily.CastinPile.strength_concrete;
                paramName[ 2] = SetFamily.CastinPile.depth_cover;
                paramName[ 3] = SetFamily.CastinPile.depth_cover_top;
                paramName[ 4] = SetFamily.CastinPile.D;                 //ストレート
                paramName[ 5] = SetFamily.CastinPile.D;                 //脚部軸
                paramName[ 6] = SetFamily.CastinPile.D_extended_foot;   //脚部拡底
                paramName[ 7] = SetFamily.CastinPile.D_extended_top;    //頂部拡頭
                paramName[ 8] = SetFamily.CastinPile.D;                 //頂部軸
                paramName[ 9] = SetFamily.CastinPile.D_extended_top;    //両端拡頭
                paramName[10] = SetFamily.CastinPile.D;                 //両端軸
                paramName[11] = SetFamily.CastinPile.D_extended_foot;   //両端拡底

                top  = GetParameter_bool(symbol, "拡頭");
                foot = GetParameter_bool(symbol, "拡底");
            }
            else if (familyname == SetFamily.PrecastPile.FamilyName)
            {
                paramName[ 0] = SetFamily.PrecastPile.name;
                paramName[ 1] = SetFamily.PrecastPile.strength_concrete;
                paramName[ 2] = SetFamily.PrecastPile.depth_cover;
                paramName[ 3] = SetFamily.PrecastPile.depth_cover_top;
                paramName[ 4] = SetFamily.PrecastPile.straight_D;
                paramName[ 5] = SetFamily.PrecastPile.ef_D_axial;           //脚部軸
                paramName[ 6] = SetFamily.PrecastPile.ef_D_extended_foot;   //脚部拡底
                paramName[ 7] = SetFamily.PrecastPile.et_D_extended_top;    //頂部拡頭
                paramName[ 8] = SetFamily.PrecastPile.et_D_axial;           //頂部軸
                paramName[ 9] = SetFamily.PrecastPile.etf_D_extended_top;   //両端拡頭
                paramName[10] = SetFamily.PrecastPile.etf_D_axial;          //両端軸
                paramName[11] = SetFamily.PrecastPile.etf_D_extended_foot;  //両端拡底


                double maxD = -1;

                //拡頭・両端・拡底いずれでも杭種1～3を使う。径の大きさをチェックする
                for (int i = 0; i <= 2; ++i)
                {
                    List<ElementId> eid = new List<ElementId>(3);
                    switch (i)
                    {
                        case 0: //拡頭
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.et_D_extended_top)?.AsElementId() ?? null);
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.et_D_axial)?.AsElementId() ?? null);
                            break;
                        case 1: //両端
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.etf_D_extended_top)?.AsElementId() ?? null);
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.etf_D_axial)?.AsElementId() ?? null);
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.etf_D_extended_foot)?.AsElementId() ?? null);
                            break;
                        case 2: //拡底
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.ef_D_axial)?.AsElementId() ?? null);
                            eid.Add(symbol.LookupParameter(SetFamily.PrecastPile.ef_D_extended_foot)?.AsElementId() ?? null);
                            break;
                    }

                    List<double> D = new List<double>(eid.Count);
                    for (int j = 0; j < eid.Count; ++j)
                    {
                        if (eid[j] == null) break;
                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol sym)
                        {
                            if (sym.Name == "Undefined") break;
                            D.Add(GetParameter_double(sym, "D"));
                        }
                    }

                    if (D.Count > 0)
                    {
                        maxD = Math.Max(maxD, D.Max());

                        bool check = false;
                        if (D.Count == eid.Count)
                        {
                            switch (i)
                            {
                                case 0:
                                    if (D[0] > D[1])
                                    {
                                        check = true;
                                        top = true;
                                        foot = false;
                                        Diameter.AddRange(D);
                                    }
                                    break;
                                case 1:
                                    if (D[0] > D[1] && D[2] > D[1])
                                    {
                                        check = true;
                                        top = true;
                                        foot = true;
                                        Diameter.AddRange(D);
                                    }
                                    break;
                                case 2:
                                    if (D[0] < D[1])
                                    {
                                        check = true;
                                        top = false;
                                        foot = true;
                                        Diameter.AddRange(D);
                                    }
                                    break;
                            }
                        }
                        if (check) break;
                    }
                }

                if (Diameter.Count == 0 && maxD > 0) Diameter.Add(maxD);
            }
            else
            {
                return retID;
            }

            id_sect++;
            retID = id_sect;

            StbSecPile_RC s = new StbSecPile_RC()
            {
                id = id_sect,
                name = GetParameter_string(symbol, paramName[0]),
                strength_concrete = GetConcreteFC(GetParameter_string(symbol, paramName[1])),
                depth_cover     = GetParameter_double(symbol, paramName[2]),
                depth_cover_top = GetParameter_double(symbol, paramName[3]),
                StbSecFigure = new StbSecPile_RC.StbSecFigureClass(),
                StbSecBar_Arrangement = new StbSecPile_RC.StbSecBar_ArrangementClass(),
            };

            if (!top && !foot)
            {
                s.StbSecFigure.StbSecFigureType = 1;
                s.StbSecFigure.StbSecStraight = new StbSecPile_RC.StbSecFigureClass.StbSecStraight_Class();
                if (Diameter.Count > 0)
                {
                    s.StbSecFigure.StbSecStraight.D = Diameter[0];
                }
                else
                {
                    s.StbSecFigure.StbSecStraight.D = GetParameter_double(symbol, paramName[4]);
                }
            }
            else if (top && foot)
            {
                s.StbSecFigure.StbSecFigureType = 4;
                s.StbSecFigure.StbSecExtended_Top_Foot = new StbSecPile_RC.StbSecFigureClass.StbSecExtended_Top_Foot_Class();
                if (Diameter.Count > 2)
                {
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top = Diameter[0];
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_axial = Diameter[1];
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_extended_foot = Diameter[2];
                }
                else
                {
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top = GetParameter_double(symbol, paramName[9]);
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_axial = GetParameter_double(symbol, paramName[10]);
                    s.StbSecFigure.StbSecExtended_Top_Foot.D_extended_foot = GetParameter_double(symbol, paramName[11]);
                }
            }
            else if (foot)
            {
                s.StbSecFigure.StbSecFigureType = 2;
                s.StbSecFigure.StbSecExtended_Foot = new StbSecPile_RC.StbSecFigureClass.StbSecExtended_Foot_Class();
                if (Diameter.Count > 1)
                {
                    s.StbSecFigure.StbSecExtended_Foot.D_axial = Diameter[0];
                    s.StbSecFigure.StbSecExtended_Foot.D_extended_foot = Diameter[1];
                }
                else
                {
                    s.StbSecFigure.StbSecExtended_Foot.D_axial = GetParameter_double(symbol, paramName[5]);
                    s.StbSecFigure.StbSecExtended_Foot.D_extended_foot = GetParameter_double(symbol, paramName[6]);
                }
            }
            else if (top)
            {
                s.StbSecFigure.StbSecFigureType = 3;
                s.StbSecFigure.StbSecExtended_Top = new StbSecPile_RC.StbSecFigureClass.StbSecExtended_Top_Class();
                if (Diameter.Count > 1)
                {
                    s.StbSecFigure.StbSecExtended_Top.D_extended_top = Diameter[0];
                    s.StbSecFigure.StbSecExtended_Top.D_axial = Diameter[1];
                }
                else
                {
                    s.StbSecFigure.StbSecExtended_Top.D_extended_top = GetParameter_double(symbol, paramName[7]);
                    s.StbSecFigure.StbSecExtended_Top.D_axial = GetParameter_double(symbol, paramName[8]);
                }
            }


            StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class[] bar = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class[3];
            if (familyname == SetFamily.CastinPile.FamilyName)
            {
                for (int i = 0; i < bar.Length; ++i)
                {
                    bar[i] = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class()
                    {
                        strength_main_circumference_1st = GetParameter_string(symbol, SetFamily.CastinPile.strength_main_circumference_1st),
                        D_main_circumference_1st        = GetParameter_string(symbol, SetFamily.CastinPile.D_main_circumference_1st[i]),
                        count_main_circumference_1st    = GetParameter_int   (symbol, SetFamily.CastinPile.count_main_circumference_1st[i]),
                        strength_main_core              = GetParameter_string(symbol, SetFamily.CastinPile.strength_main_core),
                        D_main_core                     = GetParameter_string(symbol, SetFamily.CastinPile.D_main_core[i]),
                        count_main_core                 = GetParameter_int   (symbol, SetFamily.CastinPile.count_main_core[i]),
                        strength_band                   = GetParameter_string(symbol, SetFamily.CastinPile.strength_band),
                        D_band                          = GetParameter_string(symbol, SetFamily.CastinPile.D_band[i]),
                        pitch_band                      = GetParameter_double(symbol, SetFamily.CastinPile.pitch_band[i]),
                    };
                }
            }
            else if (familyname == SetFamily.PrecastPile.FamilyName)
            {
                for (int i = 0; i < bar.Length; ++i)
                {
                    bar[i] = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Center_Bottom_Class()
                    {
                        strength_main_circumference_1st = GetParameter_string(symbol, SetFamily.PrecastPile.strength_main_circumference_1st),
                        D_main_circumference_1st        = GetParameter_string(symbol, SetFamily.PrecastPile.D_main_circumference_1st[i]),
                        count_main_circumference_1st    = GetParameter_int   (symbol, SetFamily.PrecastPile.count_main_circumference_1st[i]),
                        strength_main_core              = GetParameter_string(symbol, SetFamily.PrecastPile.strength_main_core),
                        D_main_core                     = GetParameter_string(symbol, SetFamily.PrecastPile.D_main_core[i]),
                        count_main_core                 = GetParameter_int   (symbol, SetFamily.PrecastPile.count_main_core[i]),
                        strength_band                   = GetParameter_string(symbol, SetFamily.PrecastPile.strength_band),
                        D_band                          = GetParameter_string(symbol, SetFamily.PrecastPile.D_band[i]),
                        pitch_band                      = GetParameter_double(symbol, SetFamily.PrecastPile.pitch_band[i]),
                    };
                }
            }

            bool tc = CompareTo_PileBar(bar[0], bar[1]);
            bool cb = CompareTo_PileBar(bar[1], bar[2]);
            if (tc && cb)
            {
                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 1;
                s.StbSecBar_Arrangement.StbSecPile_Same = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Same_Class()
                {
                    strength_main_circumference_1st = bar[0].strength_main_circumference_1st,
                    D_main_circumference_1st        = bar[0].D_main_circumference_1st       ,
                    count_main_circumference_1st    = bar[0].count_main_circumference_1st   ,
                    strength_main_core              = bar[0].strength_main_core             ,
                    D_main_core                     = bar[0].D_main_core                    ,
                    count_main_core                 = bar[0].count_main_core                ,
                    strength_band                   = bar[0].strength_band                  ,
                    D_band                          = bar[0].D_band                         ,
                    pitch_band                      = bar[0].pitch_band                     ,
                };

                if (bar[0].D_main_circumference_1st == "" || bar[0].count_main_circumference_1st <= 0 ||
                    bar[0].D_band == "" || bar[0].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Same = null;
                }
            }
            else if (tc || cb)
            {
                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 2;
                s.StbSecBar_Arrangement.StbSecPile_Top_Bottom = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Bottom_Class[2];
                s.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0] = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Bottom_Class()
                {
                    pos = "TOP",
                    strength_main_circumference_1st = bar[0].strength_main_circumference_1st,
                    D_main_circumference_1st        = bar[0].D_main_circumference_1st       ,
                    count_main_circumference_1st    = bar[0].count_main_circumference_1st   ,
                    strength_main_core              = bar[0].strength_main_core             ,
                    D_main_core                     = bar[0].D_main_core                    ,
                    count_main_core                 = bar[0].count_main_core                ,
                    strength_band                   = bar[0].strength_band                  ,
                    D_band                          = bar[0].D_band                         ,
                    pitch_band                      = bar[0].pitch_band                     ,
                };
                s.StbSecBar_Arrangement.StbSecPile_Top_Bottom[1] = new StbSecPile_RC.StbSecBar_ArrangementClass.StbSecPile_Top_Bottom_Class()
                {
                    pos = "BOTTOM",
                    strength_main_circumference_1st = bar[2].strength_main_circumference_1st,
                    D_main_circumference_1st        = bar[2].D_main_circumference_1st       ,
                    count_main_circumference_1st    = bar[2].count_main_circumference_1st   ,
                    strength_main_core              = bar[2].strength_main_core             ,
                    D_main_core                     = bar[2].D_main_core                    ,
                    count_main_core                 = bar[2].count_main_core                ,
                    strength_band                   = bar[2].strength_band                  ,
                    D_band                          = bar[2].D_band                         ,
                    pitch_band                      = bar[2].pitch_band                     ,
                };

                if (bar[0].D_main_circumference_1st == "" || bar[0].count_main_circumference_1st <= 0 ||
                    bar[0].D_band == "" || bar[0].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0] = null;
                }
                if (bar[2].D_main_circumference_1st == "" || bar[2].count_main_circumference_1st <= 0 ||
                    bar[2].D_band == "" || bar[2].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Top_Bottom[2] = null;
                }
            }
            else
            {
                bar[0].pos = "TOP";
                bar[1].pos = "CENTER";
                bar[2].pos = "BOTTOM";
                s.StbSecBar_Arrangement.StbSecBar_ArrangementType = 3;
                s.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom = bar;

                if (bar[0].D_main_circumference_1st == "" || bar[0].count_main_circumference_1st <= 0 ||
                    bar[0].D_band == "" || bar[0].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0] = null;
                }
                if (bar[1].D_main_circumference_1st == "" || bar[1].count_main_circumference_1st <= 0 ||
                    bar[1].D_band == "" || bar[1].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[1] = null;
                }
                if (bar[2].D_main_circumference_1st == "" || bar[2].count_main_circumference_1st <= 0 ||
                    bar[2].D_band == "" || bar[2].pitch_band < 0.01)
                {
                    s.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[2] = null;
                }
            }

            stb.StbModel.StbSections.StbSecPiles_RC.Add(s);

            return retID;
        }

        /// <summary>
        /// 基礎・杭の出力
        /// </summary>
        private static void Export_Footing()
        {
            List<string> FootingFamilyName = SetFamily.FoFName.FamilyName[0].ToList();
            List<string> PileFamilyName = SetFamily.FoFName.FamilyName.Last().Where(x => x != "").ToList();

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            IList<Element> elements = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
            List<FamilyInstance> instances = elements.OfType<FamilyInstance>().Where(x => FootingFamilyName.Contains(x.Symbol.Family.Name) && !x.Symbol.Family.IsInPlace).ToList();
            List<FamilyInstance> piles = elements.OfType<FamilyInstance>().Where(x => PileFamilyName.Contains(x.Symbol.Family.Name) && !x.Symbol.Family.IsInPlace).ToList();

            Dictionary<ElementId, int> sect = new Dictionary<ElementId, int>();

            //基礎と杭の組み合わせを作る
            Dictionary<FamilyInstance, List<FamilyInstance>> instances2 = instances.ToDictionary(x => x, y => new List<FamilyInstance>());
            Dictionary<ElementId, LocationPoint> fpos = instances.ToDictionary(x => x.Id, y => y.Location as LocationPoint);
            for (int i = 0; i < piles.Count; ++i)
            {
                if (piles[i].GroupId.Value() != -1 && instances2.Any(x => x.Key.GroupId == piles[i].GroupId))
                {
                    //グループ化されていれば、同じグループの基礎と組み合わせる。
                    instances2.Where(x => x.Key.GroupId == piles[i].GroupId).First().Value.Add(piles[i]);
                }
                else
                {
                    //近い基礎に組み合わせる
                    if (piles[i].Location is LocationPoint loc)
                    {
                        double mindist = fpos.Where(x => x.Value != null).Min(x => x.Value.Point.DistanceTo(loc.Point));
                        ElementId eid = fpos.Where(x => x.Value != null && Math.Abs(x.Value.Point.DistanceTo(loc.Point) - mindist) < 0.00001).First().Key;
                        instances2.Where(x => x.Key.Id == eid).First().Value.Add(piles[i]);
                    }
                }
            }

            //フーチング
            foreach (KeyValuePair<FamilyInstance, List<FamilyInstance>> k in instances2)
            {
                FamilyInstance ins = k.Key;
                List<FamilyInstance> piles2 = k.Value;

                LocationPoint loc = ins.Location as LocationPoint;
                double height = GetParameter_double(ins, BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM, true);
                XYZ pos0 = loc.Point - new XYZ(0, 0, height);
                XYZ pos1 = Commons.ft2mm(pos0);

                StbFooting f = new StbFooting()
                {
                    idNode = GetNodeId(pos1),
                    rotate = XYZ.BasisX.AngleOnPlaneTo(ins.HandOrientation, XYZ.BasisZ) / Math.PI * 180,
                    offset_X = 0,
                    offset_Y = 0,
                    level_bottom = GetParameter_double(ins, BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM),
                };

                if (!sect.ContainsKey(ins.Symbol.Id))
                {
                    f.id_section = Export_SecFoundation(ins);
                    if (f.id_section < 0) continue;
                    sect.Add(ins.Symbol.Id, f.id_section);
                }
                else
                {
                    f.id_section = sect[ins.Symbol.Id];
                }

                string[] paramName = new string[7];
                string familyname = ins.Symbol.Family.Name;
                if (familyname == SetFamily.FRect.FamilyName)
                {
                    paramName[0] = SetFamily.FRect.NameMembers;
                    paramName[1] = SetFamily.FRect.thickness_ex_start_X;
                    paramName[2] = SetFamily.FRect.thickness_ex_end_X;
                    paramName[3] = SetFamily.FRect.thickness_ex_start_Y;
                    paramName[4] = SetFamily.FRect.thickness_ex_end_Y;
                    paramName[5] = SetFamily.FRect.thickness_ex_top;
                    paramName[6] = SetFamily.FRect.thickness_ex_bottom;
                }
                else if (familyname == SetFamily.FTRect.FamilyName)
                {
                    paramName[0] = SetFamily.FTRect.NameMembers;
                    paramName[1] = SetFamily.FTRect.thickness_ex_start_X;
                    paramName[2] = SetFamily.FTRect.thickness_ex_end_X;
                    paramName[3] = SetFamily.FTRect.thickness_ex_start_Y;
                    paramName[4] = SetFamily.FTRect.thickness_ex_end_Y;
                    paramName[5] = SetFamily.FTRect.thickness_ex_top;
                    paramName[6] = SetFamily.FTRect.thickness_ex_bottom;
                }
                else if (familyname == SetFamily.FTri.FamilyName)
                {
                    paramName[0] = SetFamily.FTri.NameMembers;
                    paramName[1] = SetFamily.FTri.thickness_ex_start_X;
                    paramName[2] = SetFamily.FTri.thickness_ex_end_X;
                    paramName[3] = SetFamily.FTri.thickness_ex_start_Y;
                    paramName[4] = SetFamily.FTri.thickness_ex_end_Y;
                    paramName[5] = SetFamily.FTri.thickness_ex_top;
                    paramName[6] = SetFamily.FTri.thickness_ex_bottom;
                }
                else if (familyname == SetFamily.FETriangle.FamilyName)
                {
                    paramName[0] = SetFamily.FETriangle.NameMembers;
                    paramName[1] = SetFamily.FETriangle.thickness_ex_start_X;
                    paramName[2] = SetFamily.FETriangle.thickness_ex_end_X;
                    paramName[3] = SetFamily.FETriangle.thickness_ex_start_Y;
                    paramName[4] = SetFamily.FETriangle.thickness_ex_end_Y;
                    paramName[5] = SetFamily.FETriangle.thickness_ex_top;
                    paramName[6] = SetFamily.FETriangle.thickness_ex_bottom;
                }
                else if (familyname == SetFamily.FOct.FamilyName)
                {
                    paramName[0] = SetFamily.FOct.NameMembers;
                    paramName[1] = SetFamily.FOct.thickness_ex_start_X;
                    paramName[2] = SetFamily.FOct.thickness_ex_end_X;
                    paramName[3] = SetFamily.FOct.thickness_ex_start_Y;
                    paramName[4] = SetFamily.FOct.thickness_ex_end_Y;
                    paramName[5] = SetFamily.FOct.thickness_ex_top;
                    paramName[6] = SetFamily.FOct.thickness_ex_bottom;
                }
                else
                {
                    continue;
                }

                id++;
                f.id = id;

                f.name                 = GetParameter_string(ins, paramName[0]);
                f.thickness_ex_start_X = GetParameter_double(ins, paramName[1]);
                f.thickness_ex_end_X   = GetParameter_double(ins, paramName[2]);
                f.thickness_ex_start_Y = GetParameter_double(ins, paramName[3]);
                f.thickness_ex_end_Y   = GetParameter_double(ins, paramName[4]);
                f.thickness_ex_top     = GetParameter_double(ins, paramName[5]);
                f.thickness_ex_bottom  = GetParameter_double(ins, paramName[6]);

                stb.StbModel.StbMembers.StbFootings.Add(f);
                AddLog(LogCode.footing, ins, f.id, f.id_section);

                if (piles2.Count > 0)
                {
                    for (int j = 0; j < piles2.Count; ++j)
                    {
                        ins = piles2[j];

                        StbPile p = new StbPile()
                        {
                            idNode = f.idNode,
                            kind_structure = "RC",
                        };

                        if (!sect.ContainsKey(ins.Symbol.Id))
                        {
                            p.id_section = Export_SecPile(ins);
                            if (p.id_section < 0) continue;
                            sect.Add(ins.Symbol.Id, p.id_section);
                        }
                        else
                        {
                            p.id_section = sect[ins.Symbol.Id];
                        }

                        int figtype = stb.StbModel.StbSections.StbSecPiles_RC.Find(a => a.id == p.id_section).StbSecFigure.StbSecFigureType;
                        familyname = ins.Symbol.Family.Name;
                        if (familyname == SetFamily.CastinPile.FamilyName)
                        {
                            p.name = GetParameter_string(ins, SetFamily.CastinPile.NameMembers);
                            p.length_all  = GetParameter_double(ins.Symbol, SetFamily.CastinPile.length_all);
                            p.length_head = (figtype == 3 || figtype == 4 ? GetParameter_double(ins.Symbol, SetFamily.CastinPile.length_head) : 0);
                            p.length_foot = (figtype == 2 || figtype == 4 ? GetParameter_double(ins.Symbol, SetFamily.CastinPile.length_foot) : 0);
                        }
                        else if (familyname == SetFamily.PrecastPile.FamilyName)
                        {
                            switch (figtype)
                            {
                                case 1:
                                    p.length_all  = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.straight_length);
                                    p.length_head = 0;
                                    p.length_foot = 0;
                                    break;
                                case 2:
                                    p.length_all  = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.ef_length_axial);
                                    p.length_head = 0;
                                    p.length_foot = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.ef_length_foot);
                                    p.length_all += p.length_foot;
                                    break;
                                case 3:
                                    p.length_all  = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.et_length_axial);
                                    p.length_head = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.et_length_head);
                                    p.length_foot = 0;
                                    p.length_all += p.length_head;
                                    break;
                                case 4:
                                    p.length_all  = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.etf_length_axial);
                                    p.length_head = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.etf_length_head);
                                    p.length_foot = GetParameter_double(ins.Symbol, SetFamily.PrecastPile.etf_length_foot);
                                    p.length_all += (p.length_head + p.length_foot);
                                    break;
                            }

                            p.name = GetParameter_string(ins, SetFamily.PrecastPile.NameMembers);
                        }
                        else
                        {
                            continue;
                        }

                        if (piles2[j].Location is LocationPoint pileLoc)
                        {
                            XYZ ppos = Commons.ft2mm(pileLoc.Point);
                            XYZ offset = ppos - pos1;
                            p.offset_X  = offset.X;
                            p.offset_Y  = offset.Y;
                            p.level_top = offset.Z;
                        }

                        if (GetPile0Length(piles2[j].Symbol))
                        {
                            switch (pileSetting)
                            {
                                case ExportPileSetting.input:
                                    //そのまま値を出力
                                    break;
                                case ExportPileSetting.none:
                                    //0にしておけば出力されない
                                    p.length_all  = 0;
                                    p.length_head = 0;
                                    p.length_foot = 0;
                                    break;
                            }
                        }

                        id++;
                        p.id = id;
                        stb.StbModel.StbMembers.StbPiles.Add(p);
                        AddLog(LogCode.pile, ins, p.id, p.id_section);
                    }
                }
            }


            //布基礎
            instances = elements.OfType<FamilyInstance>().Where(x => x.Symbol.Family.Name == SetFamily.FConti.FamilyName && !x.Symbol.Family.IsInPlace).ToList();
            for (int i = 0; i < instances.Count; ++i)
            {
                LocationCurve loc = instances[i].Location as LocationCurve;
                id++;

                StbStrip_Footing f = new StbStrip_Footing()
                {
                    id = id,
                    name = GetParameter_string(instances[i], SetFamily.FConti.NameMembers),
                    idNode_start = GetNodeId(Commons.ft2mm(loc.Curve.GetEndPoint(0))),
                    idNode_end   = GetNodeId(Commons.ft2mm(loc.Curve.GetEndPoint(1))),
                    kind_structure = "RC",
                    level = GetParameter_double(instances[i], BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM),
                    offset = 0,
                    length_ex_start = GetParameter_double(instances[i], SetFamily.FConti.length_ex_start),
                    length_ex_end   = GetParameter_double(instances[i], SetFamily.FConti.length_ex_end),
                };

                if (!sect.ContainsKey(instances[i].Symbol.Id))
                {
                    f.id_section = Export_SecFoundation(instances[i]);
                    if (f.id_section < 0) continue;
                    sect.Add(instances[i].Symbol.Id, f.id_section);
                }
                else
                {
                    f.id_section = sect[instances[i].Symbol.Id];
                }

                stb.StbModel.StbMembers.StbStrip_Footings.Add(f);
                AddLog(LogCode.footing, instances[i], f.id, f.id_section);
            }

        }





    }
}
