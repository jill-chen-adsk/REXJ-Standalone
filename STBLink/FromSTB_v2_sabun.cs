using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using ST_BRIDGE_V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STBLink
{
    partial class FromSTB_v2
    {
        internal class ConvertCheck
        {
            internal string Fugo { get; private set; } = "";
            internal string Guid { get; private set; } = "";
            internal int Id { get; private set; } = 0;
            internal List<ElementId> TypeId { get; private set; } = null;
            internal string NodeName { get; private set; } = "";
            internal bool Check { get; set; } = false;

            internal ConvertCheck(string f, string g, int id, List<ElementId> eid, string n)
            {
                Fugo = f;
                Guid = g;
                NodeName = n;
                Id = id;
                if (eid != null && eid.Count == 0)
                {
                    TypeId = null;
                }
                else
                {
                    TypeId = eid;
                }
                Check = false;
            }

        }

        internal static Dictionary<string, List<ConvertCheck>> SabunTarget_T = new Dictionary<string, List<ConvertCheck>>();
        internal static Dictionary<string, List<ConvertCheck>> SabunTarget_I = new Dictionary<string, List<ConvertCheck>>();

        //インポートしたときのGuid情報
        internal static IDictionary<ElementId, string> GuidData = null;

        /// <summary>
        /// STBファイルにあるGuidをチェックする。残りが差分時になくなった部材
        /// </summary>
        /// <param name="stb"></param>
        internal static void CheckGuid(ST_BRIDGE stb)
        {
            List<string> guids = new List<string>();

            guids.AddRange(stb.StbModel.StbSections.StbSecColumn_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecColumn_S?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecColumn_SRC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecColumn_CFT?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecBeam_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecBeam_S?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecBeam_SRC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecBrace_S?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecFoundation_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecOpen_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecParapet_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecPileProduct?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecPile_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecPile_S?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecSlabDeck?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecSlabPrecast?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecSlab_RC?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbSections.StbSecWall_RC?.Select(a => a.guid) ?? new List<string>());



            guids.AddRange(stb.StbModel.StbMembers.StbBeams?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbBraces?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbColumns?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbFootings?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbFoundationColumns?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbGirders?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbOpens?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbParapets?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbPiles?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbPosts?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbSlabs?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbStripFootings?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbMembers.StbWalls?.Select(a => a.guid) ?? new List<string>());



            guids.AddRange(stb.StbModel.StbStories?.Select(a => a.guid) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbAxes.StbParallelAxes?.SelectMany(a => a.StbParallelAxis.Select(b => b.guid)) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbAxes.StbParallelAxes?.SelectMany(a => a.StbParallelAxis.Select(b => b.guid)) ?? new List<string>());
            guids.AddRange(stb.StbModel.StbAxes.StbRadialAxes?.SelectMany(a => a.StbRadialAxis.Select(b => b.guid)) ?? new List<string>());


            foreach (var g in guids)
            {
                CheckGuid(g);
            }
        }

        /// <summary>
        /// 使用したGuidをチェックする。残りが差分時になくなった部材
        /// </summary>
        /// <param name="guid"></param>
        private static void CheckGuid(string guid)
        {
            if (GuidData != null)
            {
                var used_item = GuidData.Where(a => a.Value == guid).ToList();
                if (used_item.Count > 0)
                {
                    foreach (var item in used_item)
                    {
                        GuidData.Remove(item.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 新STBでなくなった要素を削除する
        /// </summary>
        internal static void DeleteElement()
        {
            if (GuidData != null && GuidData.Count > 0)
            {
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                
                List<ElementId> deleteID = new List<ElementId>();
                foreach (var g in GuidData)
                {
                    Element elm = Commons.doc.GetElement(g.Key);
                    if (elm == null) continue;

                    if (elm is FamilyInstance ||
                        elm is Wall ||
                        elm is Floor)
                    {
                        //新STBでなくなった配置情報
                        Guid guid1 = Data.Convertguid(elm.UniqueId);
                        if (Guid.TryParse(g.Value, out Guid guid2))
                        {
                            if (guid1 == guid2)
                            {
                                //Revitで手動で作ってエクスポートした部材
                                //UniqueIdから生成したGuidと拡張ストレージに登録されているGuidが一致

                                //消さない
                                continue;
                            }
                        }

                        deleteID.Add(g.Key);
                        LogData.AddLog(LogData.LogKind.Infmoation, 0, $"id={g.Key}(guid={g.Value})を削除しました");
                        
                        if (amanager.HasAssociation(elm.Id))
                        {
                            //紐づいている解析線分があれば削除
                            var id = amanager.GetAssociatedElementId(elm.Id);
                            deleteID.Add(id);
                        }
                    }
                }

                if (deleteID.Count > 0)
                {
                    Transaction tran = new Transaction(Commons.doc, "部材削除");
                    tran.Start();
                    try
                    {
                        Commons.doc.Delete(deleteID);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.RollBack();
                    }
                }
            }
        }




        internal static void UpdateSection(ST_BRIDGE stb, List<ConvertForm.Chb_class> chb)
        {
            string errmsg = "";


            try
            {
                for (int i = 0; i < chb.Count(); i++)
                {
                    if (!chb[i].chbchecked) { continue; }
                    switch (chb[i].buzai)
                    {
                        case "柱":
                        case "間柱":
                        case "基礎柱":
                            if (!UpdateColumnSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "大梁":
                        case "小梁":
                        case "片持梁":
                        case "片持小梁":
                            if (!UpdateGirderSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "RCスラブ":
                        case "デッキプレート":
                        case "既製スラブ":
                        case "基礎スラブ":
                            if (!UpdateSlabSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "Sブレース":
                            if (!UpdateBraceSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "壁":
                            if (!UpdateWallSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;
                        case "RCパラペット":
                            //対象外
                            break;

                        case "基礎・布基礎・杭":
                            if (!UpdateFoundationSection(stb, chb[i].buzai, SabunTarget_T[chb[i].buzai], out errmsg))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                    }
                }

            }
            catch
            {
                //ログ出力
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }

        }


        private static bool UpdateColumnSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                Family[][] ConvFamily = GetConvFamily_Column(syubetu);

                if (stb.StbModel.StbSections.StbSecColumn_RC != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (cols.Count > 0)
                    {
                        const string logheader = "StbSecColumn_RC";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", cols.Count);

                        foreach (var section in cols)
                        {
                            Data.ProgressPerformStep();


                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;

                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreateColumn_RC(stb, section, ConvFamily)) { ret = false; errmsg = "RC柱"; }
                            }
                            else
                            {
                                //更新

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                if (section.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect) //矩形
                                {
                                    if (ConvFamily[0][0].Name != symbol.Family.Name)
                                    {
                                        //差分の前後でファミリが違う
                                        LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (ConvFamily[0][1].Name != symbol.Family.Name)
                                    {
                                        //差分の前後でファミリが違う
                                        LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                        continue;
                                    }
                                }

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = "";

                                    if (section.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect) //矩形
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.RCClmRe.name);
                                        SetParameter_RCColumn_Rect(section, symbol);
                                    }
                                    else
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.RCClmRo.name);
                                        SetParameter_RCColumn_Circle(section, symbol);
                                    }


                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Column(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    if (!columnType[0].ContainsKey(section.id))
                                    {
                                        columnType[0].Add(section.id, symbol.Id);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }

                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecColumn_S != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_S.Where(a => targetID.Contains(a.id)).ToList();
                    if (cols.Count > 0)
                    {
                        const string logheader = "StbSecColumn_S";
                        Data.ProgressRestart($"S{syubetu}断面の更新", cols.Count);

                        foreach (var section in cols)
                        {
                            Data.ProgressPerformStep();


                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;

                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreateColumn_S(stb, section, ConvFamily)) { ret = false; errmsg = "S柱"; }
                            }
                            else
                            {
                                //更新

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                //鉄骨形状を取得
                                var fig1 = section.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_Same>().ToList();
                                var fig2 = section.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_NotSame>().OrderBy(a => a.pos).ToList();
                                var fig3 = section.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_ThreeTypes>().OrderBy(a => a.pos).ToList();
                                string steelshape = "";
                                string strength_main = "";
                                string strength_web = "";
                                if (fig1.Count > 0)
                                {
                                    steelshape = fig1.First().shape;
                                    strength_main = fig1.First().strength_main;
                                    strength_web = fig1.First().strength_web;
                                }
                                else if (fig2.Count > 0)
                                {
                                    steelshape = fig2.First().shape;
                                    strength_main = fig2.First().strength_main;
                                    strength_web = fig2.First().strength_web;
                                }
                                else if (fig3.Count > 0)
                                {
                                    steelshape = fig3.First().shape;
                                    strength_main = fig3.First().strength_main;
                                    strength_web = fig3.First().strength_web;
                                }
                                strength_web = GetStrength_web(strength_web, strength_main);

                                string shape = Check_Steel(stb, steelshape, out int shapeid);
                                int family_index = -1;
                                switch (shape)
                                {
                                    case RevitLNK.st_steel_H: family_index = 0; break;
                                    case RevitLNK.st_steel_BH: family_index = 1; break;
                                    case RevitLNK.st_steel_Box: family_index = 2; break;
                                    case RevitLNK.st_steel_BBox: family_index = 3; break;
                                    case RevitLNK.st_steel_Pipe: family_index = 4; break;
                                    case RevitLNK.st_steel_T: family_index = 5; break;
                                    case RevitLNK.st_steel_C: family_index = 6; break;
                                    case RevitLNK.st_steel_L: family_index = 7; break;

                                    default:
                                        continue;
                                }

                                if (ConvFamily[1][family_index].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                string kind_column = section.kind_column == StbSecColumn_Kind_column.COLUMN ? "Column" : "Post";

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = "";

                                    string logtxt = "";
                                    string shapename = "";

                                    switch (shape)
                                    {
                                        case RevitLNK.st_steel_H:
                                            shapename = "S柱H形鋼";
                                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeid];
                                            logtxt = Roll_H_Size_Check(steel);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmH.name);
                                            SetParameter_SColumn_H(section, steelshape, strength_main, strength_web, symbol, kind_column, steel);
                                            break;

                                        case RevitLNK.st_steel_BH:
                                            shapename = "S柱組立H形鋼";
                                            var steelBH = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeid];
                                            logtxt = Build_H_Size_Check(steelBH);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmBH.name);
                                            SetParameter_SColumn_BH(section, steelshape, strength_main, strength_web, symbol, kind_column, steelBH);
                                            break;

                                        case RevitLNK.st_steel_Box:
                                            shapename = "S柱角形鋼";
                                            var steelBox = stb.StbModel.StbSections.StbSecSteel.StbSecRollBox[shapeid];
                                            logtxt = Roll_Box_Size_Check(steelBox);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmBox.name);
                                            SetParameter_SColumn_Box(section, steelshape, strength_main, symbol, kind_column, steelBox);
                                            break;

                                        case RevitLNK.st_steel_BBox:
                                            shapename = "S柱組立角形鋼管";
                                            var steelBBox = stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox[shapeid];
                                            logtxt = Build_Box_Size_Check(steelBBox);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmBBox.name);
                                            SetParameter_SColumn_BBox(section, steelshape, strength_main, symbol, kind_column, steelBBox);
                                            break;

                                        case RevitLNK.st_steel_Pipe:
                                            shapename = "S柱円形鋼管";
                                            var steelP = stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];
                                            logtxt = Pipe_Size_Check(steelP);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmPipe.name);
                                            SetParameter_SColumn_Pipe(section, steelshape, strength_main, symbol, kind_column, steelP);
                                            break;

                                        case RevitLNK.st_steel_T:
                                            shapename = "S柱T形鋼";
                                            var steelT = stb.StbModel.StbSections.StbSecSteel.StbSecRollT[shapeid];
                                            logtxt = Roll_T_Size_Check(steelT);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmT.name);
                                            SetParameter_SColumn_T(section, steelshape, strength_main, strength_web, symbol, kind_column, steelT);
                                            break;

                                        case RevitLNK.st_steel_C:
                                            shapename = "S柱溝形鋼";
                                            var steelC = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeid];
                                            logtxt = Roll_C_Size_Check(steelC);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmC.name);
                                            SetParameter_SColumn_C(section, steelshape, strength_main, symbol, kind_column, steelC);
                                            break;

                                        case RevitLNK.st_steel_L:
                                            shapename = "S柱山形鋼";
                                            var steelL = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeid];
                                            logtxt = Roll_L_Size_Check(steelL);
                                            if (logtxt != "")
                                            {
                                                Data.MakeSizeLog(shapename, symbol.Name, section.id, logtxt, 0);
                                                continue;
                                            }
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SClmL.name);
                                            SetParameter_SColumn_L(section, steelshape, strength_main, symbol, kind_column, steelL);
                                            break;
                                    }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Column(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    if (!columnType[1].ContainsKey(section.id))
                                    {
                                        columnType[1].Add(section.id, symbol.Id);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }

                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecColumn_SRC != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_SRC.Where(a => targetID.Contains(a.id)).ToList();
                    if (cols.Count > 0)
                    {
                        const string logheader = "StbSecColumn_SRC";
                        Data.ProgressRestart($"SRC{syubetu}断面の更新", cols.Count);

                        foreach (var section in cols)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreateColumn_SRC(stb, section, ConvFamily)) { ret = false; errmsg = "SRC柱"; }
                            }
                            else
                            {
                                //更新

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                string shape = GetSRCSteelShape(section);
                                int family_index = -1;
                                if (shape == "H")
                                {
                                    if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                    {
                                        family_index = 0;
                                    }
                                    else
                                    {
                                        family_index = 3;
                                    }
                                }
                                else if (shape == "CROSS")
                                {
                                    if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                    {
                                        family_index = 1;
                                    }
                                    else
                                    {
                                        family_index = 4;
                                    }
                                }
                                else if (shape == "T")
                                {
                                    if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                    {
                                        family_index = 2;
                                    }
                                    else
                                    {
                                        family_index = 5;
                                    }
                                }
                                else
                                {
                                    //ログ表示（変換対象外）
                                    if (shape == "Box")
                                    {
                                        Data.Make_taisyougaiLog("SRC柱", section.id, section.name, "StbSecColumn_SRC_ShapeBox", "SRC柱□形断面鉄骨形状");
                                    }
                                    else
                                    {
                                        Data.Make_taisyougaiLog("SRC柱", section.id, section.name, "StbSecColumn_SRC_ShapePipe", "SRC柱○形断面鉄骨形状");
                                    }
                                    continue;
                                }

                                if (ConvFamily[2][family_index].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = "";


                                    if (shape == "H")
                                    {
                                        if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmH.name);
                                            CreateColumn_SRC_H_Rec(stb, section, symbol);
                                        }
                                        else
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmH_Rou.name);
                                            CreateColumn_SRC_H_Rou(stb, section, symbol);
                                        }
                                    }
                                    else if (shape == "CROSS")
                                    {
                                        if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmCross.name);
                                            CreateColumn_SRC_Cross_Rec(stb, section, symbol);
                                        }
                                        else
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmCross_Rou.name);
                                            CreateColumn_SRC_Cross_Rou(stb, section, symbol);
                                        }
                                    }
                                    else if (shape == "T")
                                    {
                                        if (section.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmT.name);
                                            CreateColumn_SRC_T_Rec(stb, section, symbol);
                                        }
                                        else
                                        {
                                            fugo = Data.GetParameter_string(symbol, SetFamily.SRCClmT_Rou.name);
                                            CreateColumn_SRC_T_Rou(stb, section, symbol);
                                        }
                                    }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Column(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    if (!columnType[2].ContainsKey(section.id))
                                    {
                                        columnType[2].Add(section.id, symbol.Id);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecColumn_CFT != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_CFT.Where(a => targetID.Contains(a.id)).ToList();
                    if (cols.Count > 0)
                    {
                        const string logheader = "StbSecColumn_CFT";
                        Data.ProgressRestart($"CFT{syubetu}断面の更新", cols.Count);

                        foreach (var section in cols)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreateColumn_CFT(stb, section, ConvFamily)) { ret = false; errmsg = "CFT柱"; }
                            }
                            else
                            {
                                //更新

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                //鉄骨形状を取得
                                var fig1 = section.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_Same>().ToList();
                                var fig2 = section.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_NotSame>().OrderBy(a => a.pos).ToList();
                                var fig3 = section.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>().OrderBy(a => a.pos).ToList();
                                string steelshape = "";
                                string strength_main = "";
                                if (fig1.Count > 0)
                                {
                                    steelshape = fig1.First().shape;
                                    strength_main = fig1.First().strength;
                                }
                                else if (fig2.Count > 0)
                                {
                                    steelshape = fig2.First().shape;
                                    strength_main = fig2.First().strength;
                                }
                                else if (fig3.Count > 0)
                                {
                                    steelshape = fig3.First().shape;
                                    strength_main = fig3.First().strength;
                                }

                                string shape = Check_Steel(stb, steelshape, out int shapeid);
                                int family_index = -1;
                                if (shape == RevitLNK.st_steel_Box || shape == RevitLNK.st_steel_BBox)
                                {
                                    family_index = 0;
                                }
                                else
                                {
                                    family_index = 1;
                                }

                                if (ConvFamily[3][family_index].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = "";


                                    if (shape == RevitLNK.st_steel_Box || shape == RevitLNK.st_steel_BBox)
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.CFTClmBox.name);
                                        SetParameter_CFTColumn_Box(stb, section, steelshape, strength_main, shape, shapeid, symbol);
                                    }
                                    else
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.CFTClmPipe.name);
                                        SetParameter_CFTColumn_Pipe(stb, section, steelshape, strength_main, shapeid, symbol);
                                    }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Column(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    if (!columnType[3].ContainsKey(section.id))
                                    {
                                        columnType[3].Add(section.id, symbol.Id);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }
                        }
                    }
                }


                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateGirderSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                Family[][] ConvFamily = GetConvFamily_Girder(syubetu);

                if (stb.StbModel.StbSections.StbSecBeam_RC != null)
                {
                    var beams = stb.StbModel.StbSections.StbSecBeam_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (beams.Count > 0)
                    {
                        const string logheader = "StbSecBeam_RC";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", beams.Count);

                        foreach (var section in beams)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();
                                if (section.isCanti)
                                {
                                    if (!CreateCGirder_RC(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"RC{syubetu}"; }
                                }
                                else
                                {
                                    if (!CreateGirder_RC(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"RC{syubetu}"; }
                                }
                            }
                            else
                            {
                                //更新

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                int family_index = -1;

                                //ログ用部材名
                                string logbuzai = "";
                                string kind = section.kind_beam == StbSecBeam_Kind_beam.GIRDER ? "大梁" : "小梁";

                                if (!section.isCanti)
                                {
                                    switch (section.StbSecFigureBeam_RC.FigureType)
                                    {
                                        case 1:
                                            if (section.StbSecBarArrangementBeam_RC == null)
                                            {
                                                if (section.isFoundation)
                                                {
                                                    logbuzai = "基礎" + kind;
                                                    family_index = 0;
                                                }
                                                else
                                                {
                                                    logbuzai = "RC" + kind;
                                                    family_index = 2;
                                                }
                                            }
                                            else
                                            {
                                                if (section.StbSecBarArrangementBeam_RC.Bar_ArrangementType == 1)
                                                {
                                                    if (section.isFoundation)
                                                    {
                                                        logbuzai = "基礎" + kind;
                                                        family_index = 0;
                                                    }
                                                    else
                                                    {
                                                        logbuzai = "RC" + kind;
                                                        family_index = 2;
                                                    }
                                                }
                                                else
                                                {
                                                    //ハンチ付き
                                                    if (section.isFoundation)
                                                    {
                                                        logbuzai = "ハンチ付き基礎" + kind;
                                                        family_index = 1;
                                                    }
                                                    else
                                                    {
                                                        logbuzai = "ハンチ付きRC" + kind;
                                                        family_index = 3;
                                                    }
                                                }
                                            }
                                            break;

                                        case 2:
                                            var ts = section.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                            var te = section.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                                            if (ts.depth != te.depth ||
                                                ts.width != te.width)
                                            {
                                                //ハンチ付き
                                                if (section.isFoundation)
                                                {
                                                    logbuzai = "ハンチ付き基礎" + kind;
                                                    family_index = 1;
                                                }
                                                else
                                                {
                                                    logbuzai = "ハンチ付きRC" + kind;
                                                    family_index = 3;
                                                }
                                            }
                                            else
                                            {
                                                //ハンチなし
                                                if (section.isFoundation)
                                                {
                                                    logbuzai = "基礎" + kind;
                                                    family_index = 0;
                                                }
                                                else
                                                {
                                                    logbuzai = "RC" + kind;
                                                    family_index = 2;
                                                }
                                            }
                                            break;

                                        case 3:
                                            var hs = section.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                                            var hc = section.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                            var he = section.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                                            if (hs == null) hs = hc;
                                            if (he == null) he = hc;
                                            if (hs.depth != hc.depth ||
                                                he.depth != hc.depth ||
                                                hs.width != hc.width ||
                                                he.width != hc.width)
                                            {
                                                //ハンチ付き
                                                if (section.isFoundation)
                                                {
                                                    logbuzai = "ハンチ付き基礎" + kind;
                                                    family_index = 1;
                                                }
                                                else
                                                {
                                                    logbuzai = "ハンチ付きRC" + kind;
                                                    family_index = 3;
                                                }
                                            }
                                            else
                                            {
                                                //ハンチなし
                                                if (section.isFoundation)
                                                {
                                                    logbuzai = "基礎" + kind;
                                                    family_index = 0;
                                                }
                                                else
                                                {
                                                    logbuzai = "RC" + kind;
                                                    family_index = 2;
                                                }
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    if (section.isFoundation)
                                    {
                                        logbuzai = "片持基礎" + kind;
                                        family_index = 0;
                                    }
                                    else
                                    {
                                        logbuzai = "片持" + kind;
                                        family_index = 1;
                                    }
                                }


                                if (ConvFamily[0][family_index].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }




                                //haunch_start,haunch_endの取得
                                List<double> haunch_start = new List<double>();
                                List<double> haunch_end = new List<double>();
                                List<string> kind_haunch_start = new List<string>();
                                List<string> kind_haunch_end = new List<string>();
                                Get_Haunch(stb, section.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


                                Dictionary<int, ElementId> haunch_map = new Dictionary<int, ElementId>();
                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                                    double hs = Data.GetParameter_double(symbol, Rgir.haunch_start, false);
                                    double he = Data.GetParameter_double(symbol, Rgir.haunch_end, false);

                                    for (int h = 0; h < haunch_start.Count; h++)
                                    {
                                        if (Math.Abs(haunch_start[h] - hs) < 0.01 &&
                                            Math.Abs(haunch_end[h] - he) < 0.01)
                                        {
                                            if (!haunch_map.ContainsKey(h)) haunch_map.Add(h, eid);

                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = symbol.Name,
                                                id = section.id,
                                                Length = haunch_start[h],
                                                Length2 = haunch_end[h],
                                                BHaunch1 = kind_haunch_start[h],
                                                BHaunch2 = kind_haunch_end[h],
                                                symbol = symbol,
                                            };
                                            GirderSymbols.Add(re);

                                            break;
                                        }
                                    }
                                }


                                if (t.TypeId.Count == 1 && haunch_start.Count == 1)
                                {
                                    //新旧ともにハンチ長が１種類しかないならそのまま更新

                                    symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                    string fugo = "";

                                    if (!section.isCanti)
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.RCGir.name);
                                        SetParameter_RCGirder(section, logbuzai, symbol.Name, haunch_start[0], haunch_end[0], kind_haunch_start[0], kind_haunch_end[0], symbol);
                                    }
                                    else
                                    {
                                        fugo = Data.GetParameter_string(symbol, SetFamily.RCCGir.name);
                                        SetParameter_RCCGirder(section, logbuzai, symbol.Name, haunch_start[0], haunch_end[0], kind_haunch_start[0], kind_haunch_end[0], symbol);
                                    }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Girder(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );
                                }
                                else
                                {
                                    //ハンチ長にあわせて更新

                                    List<ElementId> updateid = new List<ElementId>();

                                    for (int h = 0; h < haunch_start.Count; h++)
                                    {
                                        string fugo = "";

                                        if (haunch_map.ContainsKey(h))
                                        {
                                            var eid = haunch_map[h];

                                            symbol = Commons.doc.GetElement(eid) as FamilySymbol;


                                            if (!section.isCanti)
                                            {
                                                fugo = Data.GetParameter_string(symbol, SetFamily.RCGir.name);
                                                SetParameter_RCGirder(section, logbuzai, symbol.Name, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);
                                            }
                                            else
                                            {
                                                fugo = Data.GetParameter_string(symbol, SetFamily.RCCGir.name);
                                                SetParameter_RCCGirder(section, logbuzai, symbol.Name, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);
                                            }

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                string typename = GetTypeName_Girder(stb, section.id);
                                                ChangeTypeName(symbol, typename);
                                            }
                                            updateid.Add(eid);

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                        }
                                        else
                                        {
                                            //同じ長さのハンチ長を持つタイプがないので新しく作る
                                            string typename = GetTypeName_Girder(stb, section.id);
                                            var names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a).Name.ToUpper()).ToList();
                                            string name2 = typename;
                                            int n = 1;
                                            while (names.Contains(name2.ToUpper()))
                                            {
                                                name2 = typename + "_" + n.ToString();
                                                n++;
                                            }

                                            symbol = (FamilySymbol)symbol.Duplicate(name2);
                                            if (!section.isCanti)
                                            {
                                                SetParameter_RCGirder(section, logbuzai, symbol.Name, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);
                                            }
                                            else
                                            {
                                                SetParameter_RCCGirder(section, logbuzai, symbol.Name, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);
                                            }

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );


                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = symbol.Name,
                                                id = section.id,
                                                Length = haunch_start[h],
                                                Length2 = haunch_end[h],
                                                BHaunch1 = kind_haunch_start[h],
                                                BHaunch2 = kind_haunch_end[h],
                                                symbol = symbol,
                                            };
                                            GirderSymbols.Add(re);
                                        }
                                    }

                                    foreach (var eid in t.TypeId)
                                    {
                                        if (!updateid.Contains(eid))
                                        {
                                            //対応ハンチ長がないものはハンチ長以外を更新

                                            symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                            string fugo = "";
                                            if (!section.isCanti)
                                            {
                                                fugo = Data.GetParameter_string(symbol, SetFamily.RCGir.name);
                                                SetParameter_RCGirder(section, logbuzai, symbol.Name, -1, -1, "", "", symbol);
                                            }
                                            else
                                            {
                                                fugo = Data.GetParameter_string(symbol, SetFamily.RCCGir.name);
                                                SetParameter_RCCGirder(section, logbuzai, symbol.Name, -1, -1, "", "", symbol);
                                            }

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                string typename = GetTypeName_Girder(stb, section.id);
                                                ChangeTypeName(symbol, typename);
                                            }

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );

                                            if (!GirderSymbols.Any(a => a.symbol.Id == eid))
                                            {
                                                Data.ReNameSymbols re = new Data.ReNameSymbols
                                                {
                                                    name = symbol.Name,
                                                    id = section.id,
                                                    Length = 0,
                                                    Length2 = 0,
                                                    BHaunch1 = "",
                                                    BHaunch2 = "",
                                                    symbol = symbol,
                                                };
                                                GirderSymbols.Add(re);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecBeam_S != null)
                {
                    var beams = stb.StbModel.StbSections.StbSecBeam_S.Where(a => targetID.Contains(a.id)).ToList();
                    if (beams.Count > 0)
                    {
                        const string logheader = "StbSecBeam_S";
                        Data.ProgressRestart($"S{syubetu}断面の更新", beams.Count);

                        foreach (var section in beams)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();
                                if (section.isCanti)
                                {
                                    if (!CreateCGirder_S(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"S{syubetu}"; }
                                }
                                else
                                {
                                    if (!CreateGirder_S(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"S{syubetu}"; }
                                }
                            }
                            else
                            {
                                //更新

                                string log = section.isCanti ? "S片持梁" : "S梁";

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                int family_index = -1;

                                //鉄骨形状を取得
                                int[] shapeids = new int[3];
                                GetSteelShapeData(section.StbSecSteelFigureBeam_S.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);

                                string shape = "";
                                bool check = true;
                                for (int i = 0; i < ind.Count(); i++)
                                {
                                    if (shape == "")
                                    {
                                        shape = Check_Steel(stb, steel_shapes[i], out shapeids[i]);
                                    }
                                    else
                                    {
                                        string shape_ = Check_Steel(stb, steel_shapes[i], out shapeids[i]);

                                        //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                                        if (shape != shape_)
                                        {
                                            if ((shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) &&
                                                (shape_ == RevitLNK.st_steel_H || shape_ == RevitLNK.st_steel_BH))
                                            {
                                                //H,BHの組み合わせならOK
                                                break;
                                            }
                                            Data.MakeTekkotuLog(log, section.name, section.id);
                                            check = false;
                                            break;
                                        }
                                    }
                                }
                                if (!check) continue;


                                string shapename_J = "";
                                if (shape == RevitLNK.st_steel_H) { shapename_J = "H形鋼"; }
                                else if (shape == RevitLNK.st_steel_BH) { shapename_J = "組立H形鋼"; }
                                else if (shape == RevitLNK.st_steel_C) { shapename_J = "溝形鋼"; }
                                else if (shape == RevitLNK.st_steel_L) { shapename_J = "山形鋼"; }
                                else if (shape == RevitLNK.st_steel_LipC) { shapename_J = "リップ溝形鋼"; }
                                else if (shape != "")
                                {
                                    //ログ表示(変換対象外)
                                    switch (shape)
                                    {
                                        case RevitLNK.st_steel_Box:
                                            shapename_J = "角形鋼管";
                                            break;
                                        case RevitLNK.st_steel_BBox:
                                            shapename_J = "組立角形鋼管";
                                            break;
                                        case RevitLNK.st_steel_Pipe:
                                            shapename_J = "円形鋼管";
                                            break;
                                        case RevitLNK.st_steel_T:
                                            shapename_J = "T形鋼";
                                            break;
                                        case RevitLNK.st_steel_FB:
                                            shapename_J = "フラットバー";
                                            break;
                                        case RevitLNK.st_steel_Bar:
                                            shapename_J = "丸鋼";
                                            break;
                                    }
                                    Data.Make_taisyougaiLog(log, section.id, section.name, shape, shapename_J);
                                    continue;
                                }
                                else
                                {
                                    LogData.AddLog(LogData.LogKind.Warning, 2500, $"[{log}]" + section.name + "(断面id=" + section.id.ToString() + ")の鉄骨形状[" + steel_shapes[0] + "]");
                                    continue;
                                }


                                switch (shape)
                                {
                                    case RevitLNK.st_steel_H:
                                        if (!section.isCanti)
                                        {
                                            bool shapeflg = section.StbSecSteelFigureBeam_S.FigureType == 1;
                                            if (shapeflg)
                                            {
                                                family_index = 0;
                                            }
                                            else
                                            {
                                                family_index = 5;
                                            }
                                        }
                                        else
                                        {
                                            family_index = 0;
                                        }
                                        break;

                                    case RevitLNK.st_steel_BH:
                                        family_index = 1;
                                        break;
                                    case RevitLNK.st_steel_C:
                                        family_index = 2;
                                        break;
                                    case RevitLNK.st_steel_L:
                                        family_index = 3;
                                        break;
                                    case RevitLNK.st_steel_LipC:
                                        family_index = 4;
                                        break;
                                }

                                if (ConvFamily[1][family_index].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }




                                //haunch_start,haunch_endの取得
                                List<double> haunch_start = new List<double>();
                                List<double> haunch_end = new List<double>();
                                List<string> kind_haunch_start = new List<string>();
                                List<string> kind_haunch_end = new List<string>();
                                Get_Haunch(stb, section.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


                                Dictionary<int, ElementId> haunch_map = new Dictionary<int, ElementId>();
                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                                    double hs = Data.GetParameter_double(symbol, Rgir.haunch_start, false);
                                    double he = Data.GetParameter_double(symbol, Rgir.haunch_end, false);

                                    for (int h = 0; h < haunch_start.Count; h++)
                                    {
                                        if (Math.Abs(haunch_start[h] - hs) < 0.01 &&
                                            Math.Abs(haunch_end[h] - he) < 0.01)
                                        {
                                            if (!haunch_map.ContainsKey(h)) haunch_map.Add(h, eid);

                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = symbol.Name,
                                                id = section.id,
                                                Length = haunch_start[h],
                                                Length2 = haunch_end[h],
                                                BHaunch1 = kind_haunch_start[h],
                                                BHaunch2 = kind_haunch_end[h],
                                                symbol = symbol,
                                            };
                                            GirderSymbols.Add(re);

                                            break;
                                        }
                                    }
                                }


                                string typename = GetTypeName_Girder(stb, section.id);

                                switch (shape)
                                {
                                    case RevitLNK.st_steel_H:
                                    case RevitLNK.st_steel_BH:
                                        string fugo_paraName = "";
                                        int para_mode = 0;

                                        if (shape == RevitLNK.st_steel_H)
                                        {
                                            if (!section.isCanti)
                                            {
                                                fugo_paraName = SetFamily.SGirH.name;
                                                para_mode = 0;
                                            }
                                            else
                                            {
                                                fugo_paraName = SetFamily.SCGirH.name;
                                                para_mode = 2;
                                            }
                                        }
                                        else
                                        {
                                            if (!section.isCanti)
                                            {
                                                fugo_paraName = SetFamily.SGirBH.name;
                                                para_mode = 1;
                                            }
                                            else
                                            {
                                                fugo_paraName = SetFamily.SCGirBH.name;
                                                para_mode = 3;
                                            }
                                        }

                                        if (t.TypeId.Count == 1 && haunch_start.Count == 1)
                                        {
                                            //新旧ともにハンチ長が１種類しかないならそのまま更新

                                            symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                            string fugo = "";

                                            if (!section.isCanti)
                                            {
                                                fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                                for (int j = 0; j < 3; j++)
                                                {
                                                    string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                    SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, section,
                                                                                                shape_, shapename_J,
                                                                                                steel_shapes[j], strength_main[j], strength_web[j]);
                                                }

                                                SetParameter_SGirder_H2(para_mode, section, haunch_start[0], haunch_end[0], symbol);
                                            }
                                            else
                                            {
                                                fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                                int jj = 0;
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    //中央は使わない
                                                    if (j == 1) continue;

                                                    string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                    SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, section,
                                                                                                shape_, shapename_J,
                                                                                                steel_shapes[j], strength_main[j], strength_web[j]);
                                                    jj++;
                                                }

                                                SetParameter_SGirder_H2(para_mode, section, haunch_start[0], haunch_end[0], symbol);
                                            }

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                ChangeTypeName(symbol, typename);
                                            }

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );
                                        }
                                        else
                                        {
                                            //ハンチ長にあわせて更新

                                            List<ElementId> updateid = new List<ElementId>();

                                            for (int h = 0; h < haunch_start.Count; h++)
                                            {
                                                string fugo = "";

                                                if (haunch_map.ContainsKey(h))
                                                {
                                                    var eid = haunch_map[h];

                                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;


                                                    if (!section.isCanti)
                                                    {
                                                        fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, haunch_start[h], haunch_end[h], symbol);
                                                    }
                                                    else
                                                    {
                                                        fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                                        int jj = 0;
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            //中央は使わない
                                                            if (j == 1) continue;

                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                            jj++;
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, haunch_start[h], haunch_end[h], symbol);
                                                    }


                                                    if (fugo != section.name)
                                                    {
                                                        //符号が異なる場合はタイプ名も変える
                                                        ChangeTypeName(symbol, typename);
                                                    }

                                                    updateid.Add(eid);

                                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                                }
                                                else
                                                {
                                                    //同じ長さのハンチ長を持つタイプがないので新しく作る
                                                    var names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a).Name.ToUpper()).ToList();
                                                    string name2 = typename;
                                                    int n = 1;
                                                    while (names.Contains(name2.ToUpper()))
                                                    {
                                                        name2 = typename + n.ToString();
                                                        n++;
                                                    }

                                                    symbol = (FamilySymbol)symbol.Duplicate(name2);
                                                    if (!section.isCanti)
                                                    {
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, haunch_start[h], haunch_end[h], symbol);
                                                    }
                                                    else
                                                    {
                                                        int jj = 0;
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            //中央は使わない
                                                            if (j == 1) continue;

                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                            jj++;
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, haunch_start[h], haunch_end[h], symbol);
                                                    }

                                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                                    DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );


                                                    Data.ReNameSymbols re = new Data.ReNameSymbols
                                                    {
                                                        name = symbol.Name,
                                                        id = section.id,
                                                        Length = haunch_start[h],
                                                        Length2 = haunch_end[h],
                                                        BHaunch1 = kind_haunch_start[h],
                                                        BHaunch2 = kind_haunch_end[h],
                                                        symbol = symbol,
                                                    };
                                                    GirderSymbols.Add(re);
                                                }
                                            }

                                            foreach (var eid in t.TypeId)
                                            {
                                                if (!updateid.Contains(eid))
                                                {
                                                    //対応ハンチ長がないものはハンチ長以外を更新
                                                    string fugo = "";
                                                    if (!section.isCanti)
                                                    {
                                                        fugo = Data.GetParameter_string(symbol, fugo_paraName);
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, -1, -1, symbol);
                                                    }
                                                    else
                                                    {
                                                        fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                                        int jj = 0;
                                                        for (int j = 0; j < 3; j++)
                                                        {
                                                            //中央は使わない
                                                            if (j == 1) continue;

                                                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                                                            SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, section,
                                                                                                        shape_, shapename_J,
                                                                                                        steel_shapes[j], strength_main[j], strength_web[j]);
                                                            jj++;
                                                        }

                                                        SetParameter_SGirder_H2(para_mode, section, -1, -1, symbol);
                                                    }

                                                    if (fugo != section.name)
                                                    {
                                                        //符号が異なる場合はタイプ名も変える
                                                        ChangeTypeName(symbol, typename);
                                                    }

                                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );


                                                    if (!GirderSymbols.Any(a => a.symbol.Id == eid))
                                                    {
                                                        Data.ReNameSymbols re = new Data.ReNameSymbols
                                                        {
                                                            name = symbol.Name,
                                                            id = section.id,
                                                            Length = 0,
                                                            Length2 = 0,
                                                            BHaunch1 = "",
                                                            BHaunch2 = "",
                                                            symbol = symbol,
                                                        };
                                                        GirderSymbols.Add(re);
                                                    }
                                                }
                                            }

                                        }

                                        break;


                                    case RevitLNK.st_steel_C:
                                    case RevitLNK.st_steel_L:
                                    case RevitLNK.st_steel_LipC:
                                        //ハンチなし

                                        //どの断面で変換したかログを出力
                                        if (section.StbSecSteelFigureBeam_S.FigureType != 1)
                                        {
                                            if (ind[1] == 0)
                                            { Data.MakeDanmenLog(log, typename, section.id, shape, shapename_J, "始端"); }
                                            else if (ind[1] == 1)
                                            { Data.MakeDanmenLog(log, typename, section.id, shape, shapename_J, "中央"); }
                                            else
                                            { Data.MakeDanmenLog(log, typename, section.id, shape, shapename_J, "終端"); }
                                        }


                                        foreach (var eid in t.TypeId)
                                        {
                                            symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                            if (shape == RevitLNK.st_steel_C)
                                            {
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel_C = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeids[j]];

                                                    SetParameter_Girder_and_CGirder_C(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                                                           steel_C, section, shapename_J, steel_shapes[j], strength_main[j]);
                                                }
                                            }
                                            else if (shape == RevitLNK.st_steel_L)
                                            {
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel_L = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeids[j]];
                                                    SetParameter_Girder_and_CGirder_L(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                                                           steel_L, section, shapename_J, steel_shapes[j], strength_main[j]);
                                                }
                                            }
                                            else if (shape == RevitLNK.st_steel_LipC)
                                            {
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel_LipC = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[shapeids[j]];

                                                    SetParameter_Girder_and_CGirder_LipC(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                                                              steel_LipC, section, shapename_J, steel_shapes[j], strength_main[j]);
                                                }
                                            }
                                        }

                                        break;
                                }

                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecBeam_SRC != null)
                {
                    var beams = stb.StbModel.StbSections.StbSecBeam_SRC.Where(a => targetID.Contains(a.id)).ToList();
                    if (beams.Count > 0)
                    {
                        const string logheader = "StbSecBeam_SRC";
                        Data.ProgressRestart($"SRC{syubetu}断面の更新", beams.Count);

                        foreach (var section in beams)
                        {
                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();
                                if (section.isCanti)
                                {
                                    if (!CreateCGirder_SRC(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"SRC{syubetu}"; }
                                }
                                else
                                {
                                    if (!CreateGirder_SRC(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"SRC{syubetu}"; }
                                }
                            }
                            else
                            {
                                //更新

                                string log = section.isCanti ? "SRC片持梁" : "SRC梁";

                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                if (ConvFamily[2][0].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }



                                //鉄骨形状を取得
                                int[] shapeids = new int[3];
                                GetSteelShapeData(section.StbSecSteelFigureBeam_SRC.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);

                                string shape = "";
                                bool check = true;
                                for (int i = 0; i < ind.Count(); i++)
                                {
                                    if (shape == "")
                                    {
                                        shape = Check_Steel(stb, steel_shapes[i], out shapeids[i]);
                                    }
                                    else
                                    {
                                        string shape_ = Check_Steel(stb, steel_shapes[i], out shapeids[i]);

                                        //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                                        if (shape != shape_)
                                        {
                                            if ((shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) &&
                                                (shape_ == RevitLNK.st_steel_H || shape_ == RevitLNK.st_steel_BH))
                                            {
                                                //H,BHの組み合わせならOK
                                                break;
                                            }
                                            Data.MakeTekkotuLog(log, section.name, section.id);
                                            check = false;
                                            break;
                                        }
                                    }
                                }
                                if (!check) continue;

                                switch (shape)
                                {
                                    case RevitLNK.st_steel_H:
                                    case RevitLNK.st_steel_BH:
                                        break;

                                    default:
                                        //ログ表示(変換対象外)
                                        string shapename_J = "";
                                        switch (shape)
                                        {
                                            case RevitLNK.st_steel_Box:
                                                shapename_J = "角形鋼管";
                                                break;
                                            case RevitLNK.st_steel_BBox:
                                                shapename_J = "組立角形鋼管";
                                                break;
                                            case RevitLNK.st_steel_Pipe:
                                                shapename_J = "円形鋼管";
                                                break;
                                            case RevitLNK.st_steel_T:
                                                shapename_J = "T形鋼";
                                                break;
                                            case RevitLNK.st_steel_C:
                                                shapename_J = "溝形鋼";
                                                break;
                                            case RevitLNK.st_steel_L:
                                                shapename_J = "山形鋼";
                                                break;
                                            case RevitLNK.st_steel_LipC:
                                                shapename_J = "リップ溝形鋼";
                                                break;
                                            case RevitLNK.st_steel_FB:
                                                shapename_J = "フラットバー";
                                                break;
                                            case RevitLNK.st_steel_Bar:
                                                shapename_J = "丸鋼";
                                                break;
                                        }
                                        Data.Make_taisyougaiLog(log, section.id, section.name, shape, shapename_J);
                                        continue;
                                }


                                //haunch_start,haunch_endの取得
                                List<double> haunch_start = new List<double>();
                                List<double> haunch_end = new List<double>();
                                List<string> kind_haunch_start = new List<string>();
                                List<string> kind_haunch_end = new List<string>();
                                Get_Haunch(stb, section.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


                                Dictionary<int, ElementId> haunch_map = new Dictionary<int, ElementId>();
                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                                    double hs = Data.GetParameter_double(symbol, Rgir.haunch_start, false);
                                    double he = Data.GetParameter_double(symbol, Rgir.haunch_end, false);

                                    for (int h = 0; h < haunch_start.Count; h++)
                                    {
                                        if (Math.Abs(haunch_start[h] - hs) < 0.01 &&
                                            Math.Abs(haunch_end[h] - he) < 0.01)
                                        {
                                            if (!haunch_map.ContainsKey(h)) haunch_map.Add(h, eid);

                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = symbol.Name,
                                                id = section.id,
                                                Length = haunch_start[h],
                                                Length2 = haunch_end[h],
                                                BHaunch1 = kind_haunch_start[h],
                                                BHaunch2 = kind_haunch_end[h],
                                                symbol = symbol,
                                            };
                                            GirderSymbols.Add(re);

                                            break;
                                        }
                                    }
                                }

                                string typename = GetTypeName_Girder(stb, section.id);
                                string fugo_paraName = section.isCanti ? SetFamily.SRCCGirH.name : SetFamily.SRCGirH.name;

                                if (t.TypeId.Count == 1 && haunch_start.Count == 1)
                                {
                                    //新旧ともにハンチ長が１種類しかないならそのまま更新

                                    symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                    string fugo = "";

                                    fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                    SetParameter_SRCGirder_S(stb, section, steel_shapes, strength_main, strength_web, shape, symbol);
                                    SetParameter_SRCGirder_RC(section, typename, haunch_start[0], haunch_end[0], kind_haunch_start[0], kind_haunch_end[0], symbol);

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );
                                }
                                else
                                {
                                    //ハンチ長にあわせて更新

                                    List<ElementId> updateid = new List<ElementId>();

                                    for (int h = 0; h < haunch_start.Count; h++)
                                    {
                                        string fugo = "";

                                        if (haunch_map.ContainsKey(h))
                                        {
                                            var eid = haunch_map[h];

                                            symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                            fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                            SetParameter_SRCGirder_S(stb, section, steel_shapes, strength_main, strength_web, shape, symbol);
                                            SetParameter_SRCGirder_RC(section, typename, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                ChangeTypeName(symbol, typename);
                                            }
                                            updateid.Add(eid);
                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                        }
                                        else
                                        {
                                            //同じ長さのハンチ長を持つタイプがないので新しく作る
                                            var names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a).Name.ToUpper()).ToList();
                                            string name2 = typename;
                                            int n = 1;
                                            while (names.Contains(name2.ToUpper()))
                                            {
                                                name2 = typename + n.ToString();
                                                n++;
                                            }

                                            symbol = (FamilySymbol)symbol.Duplicate(name2);
                                            SetParameter_SRCGirder_S(stb, section, steel_shapes, strength_main, strength_web, shape, symbol);
                                            SetParameter_SRCGirder_RC(section, typename, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );


                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = symbol.Name,
                                                id = section.id,
                                                Length = haunch_start[h],
                                                Length2 = haunch_end[h],
                                                BHaunch1 = kind_haunch_start[h],
                                                BHaunch2 = kind_haunch_end[h],
                                                symbol = symbol,
                                            };
                                            GirderSymbols.Add(re);
                                        }
                                    }

                                    foreach (var eid in t.TypeId)
                                    {
                                        if (!updateid.Contains(eid))
                                        {
                                            //対応ハンチ長がないものはハンチ長以外を更新
                                            string fugo = Data.GetParameter_string(symbol, fugo_paraName);

                                            SetParameter_SRCGirder_S(stb, section, steel_shapes, strength_main, strength_web, shape, symbol);
                                            SetParameter_SRCGirder_RC(section, typename, -1, -1, "", "", symbol);

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                ChangeTypeName(symbol, typename);
                                            }

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );


                                            if (!GirderSymbols.Any(a => a.symbol.Id == eid))
                                            {
                                                Data.ReNameSymbols re = new Data.ReNameSymbols
                                                {
                                                    name = symbol.Name,
                                                    id = section.id,
                                                    Length = 0,
                                                    Length2 = 0,
                                                    BHaunch1 = "",
                                                    BHaunch2 = "",
                                                    symbol = symbol,
                                                };
                                                GirderSymbols.Add(re);
                                            }
                                        }
                                    }


                                }

                            }
                        }
                    }
                }

                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateBraceSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                Family[][] ConvFamily = GetConvFamily_Brace();

                if (stb.StbModel.StbSections.StbSecBrace_S != null)
                {
                    var braces = stb.StbModel.StbSections.StbSecBrace_S.Where(a => targetID.Contains(a.id)).ToList();
                    if (braces.Count > 0)
                    {
                        const string logheader = "StbSecBrace_S";
                        Data.ProgressRestart($"{syubetu}断面の更新", braces.Count);

                        foreach (var section in braces)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();
                                if (!CreateBrace_S(stb, section, ConvFamily, ref typename_list)) { ret = false; errmsg = $"{syubetu}"; }
                            }
                            else
                            {
                                //更新
                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;


                                //鉄骨形状を取得
                                int shapeid = -1;
                                string steel_shape = "";
                                string strength_main = "";
                                string strength_web = "";

                                var fig_s = section.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_Same>().ToList();
                                var fig_n = section.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_NotSame>().OrderBy(a => a.pos).ToList();
                                var fig_3 = section.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_ThreeTypes>().OrderBy(a => a.pos).ToList();

                                if (fig_s.Count > 0)
                                {
                                    steel_shape = fig_s.First().shape;
                                    strength_main = fig_s.First().strength_main;
                                    strength_web = fig_s.First().strength_web;
                                }
                                else if (fig_n.Count > 0)
                                {
                                    steel_shape = fig_n.First().shape;
                                    strength_main = fig_n.First().strength_main;
                                    strength_web = fig_n.First().strength_web;
                                }
                                else if (fig_3.Count > 0)
                                {
                                    var fig = fig_3.Find(a => a.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER);
                                    steel_shape = fig.shape;
                                    strength_main = fig.strength_main;
                                    strength_web = fig.strength_web;
                                }

                                strength_web = GetStrength_web(strength_web, strength_main);


                                string shape = Check_Steel(stb, steel_shape, out shapeid);

                                int family_index1 = -1;
                                int family_index2 = -1;
                                string shapename = "";
                                if (shape == RevitLNK.st_steel_H) { shapename = "H形鋼"; family_index1 = 0; family_index2 = 0; }
                                else if (shape == RevitLNK.st_steel_BH) { shapename = "組立H形鋼"; family_index1 = 0; family_index2 = 1; }
                                else if (shape == RevitLNK.st_steel_C) { shapename = "溝形鋼"; family_index1 = 1; family_index2 = 0; }
                                else if (shape == RevitLNK.st_steel_L) { shapename = "山形鋼"; family_index1 = 1; family_index2 = 1; }
                                else if (shape == RevitLNK.st_steel_LipC) { shapename = "溝形鋼"; family_index1 = 1; family_index2 = 2; }
                                else if (shape == RevitLNK.st_steel_Box) { shapename = "角型鋼"; family_index1 = 0; family_index2 = 2; }
                                else if (shape == RevitLNK.st_steel_BBox) { shapename = "組立角型鋼"; family_index1 = 0; family_index2 = 3; }
                                else if (shape == RevitLNK.st_steel_Pipe) { shapename = "円形鋼管"; family_index1 = 0; family_index2 = 4; }
                                else if (shape == RevitLNK.st_steel_FB) { shapename = "フラットバー"; family_index1 = 1; family_index2 = 3; }
                                else if (shape == RevitLNK.st_steel_Bar) { shapename = "丸鋼"; family_index1 = 1; family_index2 = 4; }
                                else if (shape != "")
                                {
                                    //ログ表示(変換対象外)
                                    LogData.AddLog(LogData.LogKind.Warning, 2200, syubetu + section.name + "(断面id=" + section.id.ToString() + ")");
                                    continue;
                                }
                                else
                                {
                                    if (steel_shape != "")
                                    { LogData.AddLog(LogData.LogKind.Warning, 2500, syubetu + section.name + "(断面id=" + section.id.ToString() + ")の鉄骨形状[" + steel_shape + "]"); }
                                    else
                                    { LogData.AddLog(LogData.LogKind.Warning, 3000, syubetu + section.name + "(断面id=" + section.id.ToString() + ")はブレース鉄骨情報"); }
                                    continue;
                                }

                                if (ConvFamily[family_index1][family_index2].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                string typename = GetTypeName_Brace(stb, section.id);

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;
                                    string fugo = "";

                                    if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH || shape == RevitLNK.st_steel_C || shape == RevitLNK.st_steel_L || shape == RevitLNK.st_steel_LipC)
                                    {
                                        //マッピングテーブルが梁と共用なので3断面ある

                                        //鉄骨形状を取得
                                        int[] shapeids = new int[3];
                                        GetSteelShapeData(section.StbSecSteelFigureBrace_S.Items, out int[] ind2, out string[] steel_shape2, out string[] strength_main2, out string[] strength_web2);
                                        for (int j = 0; j < ind2.Length; ++j)
                                        {
                                            Check_Steel(stb, steel_shape2[j], out shapeids[j]);
                                        }

                                        double[] A = new double[3];
                                        double[] B = new double[3];
                                        double[] t1 = new double[3];
                                        double[] t2 = new double[3];
                                        double[] r1 = new double[3];
                                        double[] r2 = new double[3];
                                        string[] type = new string[3];
                                        bool[] side = new bool[3];
                                        bool check = true;
                                        switch (shape)
                                        {
                                            case RevitLNK.st_steel_H:
                                                //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeids[j]];

                                                    string logtxt = Roll_H_Size_Check(steel);
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                        check = false;
                                                        break;
                                                    }
                                                    A[j] = steel.A;
                                                    B[j] = steel.B;
                                                    type[j] = steel.type.ToString();
                                                    t1[j] = steel.t1;
                                                    t2[j] = steel.t2;

                                                    logtxt = "";
                                                    if (steel.r < 1)
                                                    {
                                                        logtxt = "フィレット半径";
                                                        r1[j] = 1;
                                                    }
                                                    else
                                                    { r1[j] = steel.r; }
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 1);
                                                    }
                                                }
                                                if (!check) continue;

                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraH.name);
                                                SetParameter_SBrace_H(section, symbol, steel_shape2, strength_main2, strength_web2, A, B, t1, t2, r1, type);
                                                break;

                                            case RevitLNK.st_steel_BH:
                                                //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeids[j]];

                                                    string logtxt = Build_H_Size_Check(steel);
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                        check = false;
                                                        break;
                                                    }
                                                    A[j] = steel.A;
                                                    B[j] = steel.B;
                                                    t1[j] = steel.t1;
                                                    t2[j] = steel.t2;
                                                }
                                                if (!check) continue;

                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraBH.name);
                                                SetParameter_SBrace_BH(section, symbol, steel_shape2, strength_main2, strength_web2, A, B, t1, t2);
                                                break;

                                            case RevitLNK.st_steel_C:
                                                //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeids[j]];

                                                    string logtxt = Roll_C_Size_Check(steel);
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                        check = false;
                                                        break;
                                                    }
                                                    A[j] = steel.A;
                                                    B[j] = steel.B;
                                                    t1[j] = steel.t1;
                                                    t2[j] = steel.t2;
                                                    r1[j] = steel.r1;
                                                    r2[j] = steel.r2;
                                                    type[j] = steel.type.ToString();
                                                    side[j] = steel.type != StbSecRollCType.SINGLE;
                                                    logtxt = "";
                                                    if (steel.r1 < 1)
                                                    {
                                                        logtxt = "フィレット半径";
                                                        r1[j] = 1;
                                                    }
                                                    if (steel.r2 < 1)
                                                    {
                                                        if (logtxt == "")
                                                        { logtxt = "フランジ先端半径"; }
                                                        else
                                                        { logtxt += ",フランジ先端半径"; }
                                                        r2[j] = 1;
                                                    }
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 1);
                                                    }
                                                }
                                                if (!check) continue;

                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraC.name);
                                                SetParameter_SBrace_C(section, typename, symbol, steel_shape2, strength_main2, A, B, t1, t2, r1, r2, type, side);
                                                break;

                                            case RevitLNK.st_steel_L:
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeids[j]];

                                                    string logtxt = Roll_L_Size_Check(steel);
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                        check = false;
                                                        break;
                                                    }
                                                    if (steel.r1 < 1)
                                                    {
                                                        if (logtxt == "")
                                                        { logtxt = "フィレット半径"; }
                                                        else
                                                        { logtxt += ",フィレット半径"; }
                                                        r1[j] = 1;
                                                    }
                                                    if (steel.r2 < 1)
                                                    {
                                                        if (logtxt == "")
                                                        { logtxt = "先端半径"; }
                                                        else
                                                        { logtxt += ",先端半径"; }
                                                        r2[j] = 1;
                                                    }
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 1);
                                                    }
                                                    A[j] = steel.A;
                                                    B[j] = steel.B;
                                                    t1[j] = steel.t1;
                                                    t2[j] = steel.t2;
                                                    r1[j] = steel.r1;
                                                    r2[j] = steel.r2;
                                                    type[j] = steel.type.ToString();
                                                    side[j] = steel.type != StbSecRollLType.SINGLE;
                                                    logtxt = "";
                                                    if (steel.r1 < 1)
                                                    {
                                                        logtxt = "フィレット半径";
                                                        r1[j] = 1;
                                                    }
                                                    if (steel.r2 < 1)
                                                    {
                                                        if (logtxt == "")
                                                        { logtxt = "先端半径"; }
                                                        else
                                                        { logtxt += ",先端半径"; }
                                                        r2[j] = 1;
                                                    }
                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 1);
                                                    }
                                                }
                                                if (!check) continue;

                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraL.name);
                                                SetParameter_SBrace_L(section, typename, symbol, steel_shape2, strength_main2, A, B, t1, t2, r1, r2, type, side);
                                                break;

                                            case RevitLNK.st_steel_LipC:
                                                double[] H = new double[3];
                                                double[] C = new double[3];
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[shapeids[j]];

                                                    string logtxt = Rool_LipC_Size_Check(steel);

                                                    if (logtxt != "")
                                                    {
                                                        Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                        check = false;
                                                        break;
                                                    }
                                                    H[j] = steel.H;
                                                    A[j] = steel.A;
                                                    C[j] = steel.C;
                                                    t1[j] = steel.t;
                                                    side[j] = steel.type != StbSecLipCType.SINGLE;
                                                    type[j] = steel.type.ToString();
                                                }
                                                if (!check) continue;

                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraLipC.name);
                                                SetParameter_SBrace_LipC(section, typename, symbol, steel_shape2, strength_main2, H, A, C, t1, type, side);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        string logtxt = "";
                                        switch (shape)
                                        {
                                            case RevitLNK.st_steel_Box:
                                                var steel_B = stb.StbModel.StbSections.StbSecSteel.StbSecRollBox[shapeid];
                                                logtxt = Roll_Box_Size_Check(steel_B);
                                                if (logtxt != "")
                                                {
                                                    Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                    continue;
                                                }
                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraBox.name);
                                                SetParameter_SBrace_Box(section, steel_shape, strength_main, symbol, steel_B);
                                                break;

                                            case RevitLNK.st_steel_BBox:
                                                var steel_BB = stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox[shapeid];
                                                logtxt = Build_Box_Size_Check(steel_BB);
                                                if (logtxt != "")
                                                {
                                                    Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                    continue;
                                                }
                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraBBox.name);
                                                SetParameter_SBrace_BBox(section, steel_shape, strength_main, symbol, steel_BB);
                                                break;

                                            case RevitLNK.st_steel_Pipe:
                                                var steel_P = stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];
                                                logtxt = Pipe_Size_Check(steel_P);
                                                if (logtxt != "")
                                                {
                                                    Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                    continue;
                                                }
                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraPipe.name);
                                                SetParameter_SBrace_Pipe(section, steel_shape, strength_main, symbol, steel_P);
                                                break;

                                            case RevitLNK.st_steel_FB:
                                                var steel_FB = stb.StbModel.StbSections.StbSecSteel.StbSecFlatBar[shapeid];
                                                if (steel_FB.B == 0)
                                                { logtxt = "幅"; }
                                                if (steel_FB.t == 0)
                                                {
                                                    if (logtxt == "")
                                                    { logtxt = "板厚"; }
                                                    else
                                                    { logtxt += ",板厚"; }
                                                }
                                                if (logtxt != "")
                                                {
                                                    Data.MakeSizeLog(syubetu + shapename, typename, section.id, logtxt, 0);
                                                    continue;
                                                }
                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraFB.name);
                                                SetParameter_SBrace_FB(section, steel_shape, strength_main, symbol, steel_FB);
                                                break;

                                            case RevitLNK.st_steel_Bar:
                                                var steel_Bar = stb.StbModel.StbSections.StbSecSteel.StbSecRoundBar[shapeid];
                                                if (steel_Bar.R == 0)
                                                {
                                                    Data.MakeSizeLog(syubetu + shapename, typename, section.id, "直径", 0);
                                                    continue;
                                                }
                                                fugo = Data.GetParameter_string(symbol, SetFamily.SBraRollBar.name);
                                                SetParameter_SBrace_Bar(section, steel_shape, strength_main, symbol, steel_Bar);
                                                break;
                                        }
                                    }


                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    if (!braceType[0].ContainsKey(section.id))
                                    {
                                        braceType[0].Add(section.id, symbol.Id);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );

                                }


                            }
                        }
                    }
                }

                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateSlabSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                //変換ファミリの取得
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                List<FloorType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FloorType>().ToList();


                if (stb.StbModel.StbSections.StbSecSlab_RC != null)
                {
                    var slabs = stb.StbModel.StbSections.StbSecSlab_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (slabs.Count > 0)
                    {
                        const string logheader = "StbSecSlab_RC";
                        Data.ProgressRestart($"{syubetu}断面の更新", slabs.Count);

                        foreach (var section in slabs)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            string typename = GetTypeName_Slab(stb, section.id);
                            if (t.TypeId == null)
                            {
                                //新規断面
                                var names = symbols.Select(a => a.Name.ToUpper()).ToList();
                                string name2 = typename;
                                int ascii = 97;
                                while (names.Contains(name2.ToUpper()))
                                {
                                    name2 = Data.ReName(typename, ascii);
                                    ascii++;
                                }

                                FloorType symbol = (FloorType)symbols[0].Duplicate(name2);
                                symbols.Add(symbol);
                                if (!CreateRCSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }
                                
                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を作成しました");
                            }
                            else
                            {
                                //更新
                                foreach (var eid in t.TypeId)
                                {
                                    FloorType symbol = Commons.doc.GetElement(eid) as FloorType;
                                    
                                    string fugo = Data.GetParameter_string(symbol, SetFamily.Slab.name);

                                    if (!CreateRCSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }

                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecSlabDeck != null)
                {
                    var slabs = stb.StbModel.StbSections.StbSecSlabDeck.Where(a => targetID.Contains(a.id)).ToList();
                    if (slabs.Count > 0)
                    {
                        const string logheader = "StbSecSlabDeck";
                        Data.ProgressRestart($"{syubetu}断面の更新", slabs.Count);

                        foreach (var section in slabs)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            string typename = GetTypeName_Slab(stb, section.id);
                            if (t.TypeId == null)
                            {
                                //新規断面
                                var names = symbols.Select(a => a.Name.ToUpper()).ToList();
                                string name2 = typename;
                                int ascii = 97;
                                while (names.Contains(name2.ToUpper()))
                                {
                                    name2 = Data.ReName(typename, ascii);
                                    ascii++;
                                }

                                FloorType symbol = (FloorType)symbols[0].Duplicate(name2);
                                symbols.Add(symbol);
                                if (!CreateDeckSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を作成しました");
                            }
                            else
                            {
                                //更新
                                foreach (var eid in t.TypeId)
                                {
                                    FloorType symbol = Commons.doc.GetElement(eid) as FloorType;

                                    string fugo = Data.GetParameter_string(symbol, SetFamily.Slab.name);

                                    if (!CreateDeckSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecSlabPrecast != null)
                {
                    var slabs = stb.StbModel.StbSections.StbSecSlabPrecast.Where(a => targetID.Contains(a.id)).ToList();
                    if (slabs.Count > 0)
                    {
                        const string logheader = "StbSecSlabPrecast";
                        Data.ProgressRestart($"{syubetu}断面の更新", slabs.Count);

                        foreach (var section in slabs)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            string typename = GetTypeName_Slab(stb, section.id);
                            if (t.TypeId == null)
                            {
                                //新規断面
                                var names = symbols.Select(a => a.Name.ToUpper()).ToList();
                                string name2 = typename;
                                int ascii = 97;
                                while (names.Contains(name2.ToUpper()))
                                {
                                    name2 = Data.ReName(typename, ascii);
                                    ascii++;
                                }

                                FloorType symbol = (FloorType)symbols[0].Duplicate(name2);
                                symbols.Add(symbol);
                                if (!CreateProductSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を作成しました");
                            }
                            else
                            {
                                //更新
                                foreach (var eid in t.TypeId)
                                {
                                    FloorType symbol = Commons.doc.GetElement(eid) as FloorType;

                                    string fugo = Data.GetParameter_string(symbol, SetFamily.Slab.name);

                                    if (!CreateProductSlab(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }
                        }
                    }
                }


                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateWallSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                //変換ファミリの取得
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                List<WallType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<WallType>().Where(a => a.Kind == WallKind.Basic).ToList();


                if (stb.StbModel.StbSections.StbSecWall_RC != null)
                {
                    var walls = stb.StbModel.StbSections.StbSecWall_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (walls.Count > 0)
                    {
                        const string logheader = "StbSecWall_RC";
                        Data.ProgressRestart($"{syubetu}断面の更新", walls.Count);

                        foreach (var section in walls)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;


                            string typename = GetTypeName_Wall(stb, section.id);
                            if (t.TypeId == null)
                            {
                                //新規断面
                                var names = symbols.Select(a => a.Name.ToUpper()).ToList();
                                string name2 = typename;
                                int ascii = 97;
                                while (names.Contains(name2.ToUpper()))
                                {
                                    name2 = Data.ReName(typename, ascii);
                                    ascii++;
                                }

                                WallType symbol = (WallType)symbols[0].Duplicate(name2);
                                symbols.Add(symbol);
                                if (!CreateRCWall(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を作成しました");
                            }
                            else
                            {
                                //更新
                                foreach (var eid in t.TypeId)
                                {
                                    WallType symbol = Commons.doc.GetElement(eid) as WallType;

                                    string fugo = Data.GetParameter_string(symbol, SetFamily.Slab.name);

                                    if (!CreateRCWall(stb, section, symbol)) { ret = false; errmsg = $"{syubetu}"; }

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        ChangeTypeName(symbol, typename);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }
                            }

                        }
                    }
                }


                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateFoundationSection(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            var targetID = target.Where(a => a.Check).Select(a => a.Id).ToList();
            if (targetID.Count == 0) return true;

            bool ret = true;

            Transaction tran = new Transaction(Commons.doc, syubetu + "断面の更新");
            tran.Start();
            try
            {
                Family[][] ConvFamily = GetConvFamily_Foundation();


                //基礎
                if (stb.StbModel.StbSections.StbSecFoundation_RC != null)
                {
                    var foundations = stb.StbModel.StbSections.StbSecFoundation_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (foundations.Count > 0)
                    {
                        const string logheader = "StbSecFoundation_RC";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", foundations.Count);

                        foreach (var section in foundations)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreateFoundation_RC(stb, section, ConvFamily)) { ret = false; errmsg = "基礎"; }
                            }
                            else
                            {
                                //更新
                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;
                                int family_index1 = 0;
                                int family_index2 = -1;
                                string fugo_paraName = "";
                                switch (section.StbSecFigureFoundation_RC.FigureType)
                                {
                                    case 1: family_index1 = 0; family_index2 = 0; fugo_paraName = SetFamily.FRect.name; break;
                                    case 2: family_index1 = 0; family_index2 = 1; fugo_paraName = SetFamily.FTRect.name; break;
                                    case 3: family_index1 = 0; family_index2 = 2; fugo_paraName = SetFamily.FTri.name; break;
                                    case 4: family_index1 = 0; family_index2 = 3; fugo_paraName = SetFamily.FETriangle.name; break;
                                    case 5: family_index1 = 0; family_index2 = 4; fugo_paraName = SetFamily.FOct.name; break;
                                    case 6: family_index1 = 1; family_index2 = 0; fugo_paraName = SetFamily.FConti.name; break;

                                    default: continue;
                                }

                                if (ConvFamily[family_index1][family_index2].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }


                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = Data.GetParameter_string(symbol, fugo_paraName);
                                    switch (section.StbSecFigureFoundation_RC.FigureType)
                                    {
                                        case 1: SetParameter_RCFoundation_Rect(section, symbol); break;
                                        case 2: SetParameter_RCFoundation_Taper(section, symbol); break;
                                        case 3: SetParameter_RCFoundation_Triangle(section, symbol); break;
                                        case 4: SetParameter_RCFoundation_EquiTriangle(section, symbol); break;
                                        case 5: SetParameter_RCFoundation_Octagon(section, symbol); break;
                                        case 6:
                                            var strip_fo = stb.StbModel.StbMembers.StbStripFootings.Find(a => a.id_section == section.id);
                                            double t_B = Get_Girder_B(stb, strip_fo.id_node_start, strip_fo.id_node_end);
                                            SetParameter_RCFoundation_Continuous(section, symbol, t_B);

                                            Data.ReNameSymbols re = new Data.ReNameSymbols
                                            {
                                                name = section.name,
                                                Length = t_B,
                                                symbol = symbol,
                                                id = section.id
                                            };
                                            FContiSymbols.Add(re);
                                            break;
                                    }

                                    if (section.StbSecFigureFoundation_RC.FigureType != 6)
                                    {
                                        if (!footingType[0].ContainsKey(section.id))
                                        {
                                            footingType[0].Add(section.id, symbol.Id);
                                        }
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }

                            }

                        }
                    }
                }


                //杭
                if (stb.StbModel.StbSections.StbSecPile_RC != null)
                {
                    var piles = stb.StbModel.StbSections.StbSecPile_RC.Where(a => targetID.Contains(a.id)).ToList();
                    if (piles.Count > 0)
                    {
                        const string logheader = "StbSecPile_RC";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", piles.Count);

                        foreach (var section in piles)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreatePile_RC(stb, section, ConvFamily)) { ret = false; errmsg = "RC杭"; }
                            }
                            else
                            {
                                //更新
                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                if (ConvFamily[2][0].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                var length = GetPile_length(stb, section.id);

                                Dictionary<int, ElementId> length_map = new Dictionary<int, ElementId>();
                                FamilyStructure.Pile Rpile = SetFamily.CastinPile;
                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    double len0 = Data.GetParameter_double(symbol, Rpile.length_all, false);
                                    double len1 = Data.GetParameter_double(symbol, Rpile.length_head, false);
                                    double len2 = Data.GetParameter_double(symbol, Rpile.length_foot, false);

                                    if (Data.GetParameter_int(symbol, "拡頭") == 0) len1 = 0;
                                    if (Data.GetParameter_int(symbol, "拡底") == 0) len2 = 0;

                                    for (int L = 0; L < length.Count; ++L)
                                    {
                                        if (Math.Abs(length[L][0] - len0) < 1 &&
                                            Math.Abs(length[L][1] - len1) < 1 &&
                                            Math.Abs(length[L][2] - len2) < 1)
                                        {
                                            length_map.Add(L, eid);
                                            break;
                                        }
                                    }
                                }

                                if (t.TypeId.Count == 1 && length.Count == 1)
                                {
                                    //新旧ともに杭長が１種類しかないならそのまま更新
                                    symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                    string fugo = Data.GetParameter_string(symbol, SetFamily.CastinPile.name);
                                    SetParameter_RCPile(section, symbol, length[0]);

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Pile(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    Data.ReNameSymbols s = new Data.ReNameSymbols
                                    {
                                        symbol = symbol,
                                        id = section.id,
                                        name = section.name,
                                        Length = length[0][0],
                                        Length2 = length[0][1],
                                        Length3 = length[0][2],
                                        BHaunch1 = StbPileKind_structure.RC.ToString(),
                                    };
                                    PilesSymbols.Add(s);

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );
                                }
                                else
                                {
                                    //杭長にあわせて更新

                                    List<ElementId> updateid = new List<ElementId>();

                                    for (int L = 0; L < length.Count; ++L)
                                    {
                                        string fugo = "";

                                        if (length_map.ContainsKey(L))
                                        {
                                            var eid = length_map[L];
                                            symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                            fugo = Data.GetParameter_string(symbol, SetFamily.CastinPile.name);
                                            SetParameter_RCPile(section, symbol, length[L]);

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                string typename = GetTypeName_Pile(stb, section.id);
                                                ChangeTypeName(symbol, typename);
                                            }
                                            updateid.Add(eid);

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                        }
                                        else
                                        {
                                            //同じ長さの杭長を持つタイプがないので新しく作る
                                            string typename = GetTypeName_Pile(stb, section.id);
                                            var names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a).Name.ToUpper()).ToList();
                                            string name2 = typename;
                                            int n = 1;
                                            while (names.Contains(name2.ToUpper()))
                                            {
                                                name2 = typename + n.ToString();
                                                n++;
                                            }

                                            symbol = (FamilySymbol)symbol.Duplicate(name2);
                                            SetParameter_RCPile(section, symbol, length[L]);
                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( symbol, section.id, logheader, symbol.Name );
                                        }

                                        Data.ReNameSymbols s = new Data.ReNameSymbols
                                        {
                                            symbol = symbol,
                                            id = section.id,
                                            name = section.name,
                                            Length = length[L][0],
                                            Length2 = length[L][1],
                                            Length3 = length[L][2],
                                            BHaunch1 = StbPileKind_structure.RC.ToString(),
                                        };
                                        PilesSymbols.Add(s);
                                    }

                                    foreach (var eid in t.TypeId)
                                    {
                                        if (!updateid.Contains(eid))
                                        {
                                            //対応杭長がないものは杭長以外を更新
                                            string fugo = Data.GetParameter_string(symbol, SetFamily.CastinPile.name);
                                            SetParameter_RCPile(section, symbol, null);

                                            if (fugo != section.name)
                                            {
                                                //符号が異なる場合はタイプ名も変える
                                                string typename = GetTypeName_Pile(stb, section.id);
                                                ChangeTypeName(symbol, typename);
                                            }

                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                            DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecPile_S != null)
                {
                    var piles = stb.StbModel.StbSections.StbSecPile_S.Where(a => targetID.Contains(a.id)).ToList();
                    if (piles.Count > 0)
                    {
                        const string logheader = "StbSecPile_S";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", piles.Count);

                        foreach (var section in piles)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreatePile_S(stb, section, ConvFamily)) { ret = false; errmsg = "鋼管杭"; }
                            }
                            else
                            {
                                //更新
                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                if (ConvFamily[2][2].Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = Data.GetParameter_string(symbol, SetFamily.Pile_S.name);
                                    SetParameter_SPile(section, symbol);

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Pile(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    Data.ReNameSymbols s = new Data.ReNameSymbols
                                    {
                                        symbol = symbol,
                                        id = section.id,
                                        name = section.name,
                                        Length = 0,
                                        Length2 = 0,
                                        Length3 = 0,
                                        BHaunch1 = StbPileKind_structure.S.ToString(),
                                    };
                                    PilesSymbols.Add(s);

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }

                            }
                        }
                    }
                }

                if (stb.StbModel.StbSections.StbSecPileProduct != null)
                {
                    var piles = stb.StbModel.StbSections.StbSecPileProduct.Where(a => targetID.Contains(a.id)).ToList();
                    if (piles.Count > 0)
                    {
                        const string logheader = "StbSecPileProduct";
                        Data.ProgressRestart($"RC{syubetu}断面の更新", piles.Count);

                        foreach (var section in piles)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (section.guid == null || section.guid == "") continue;


                            //CheckをOFFにしたものは対象外
                            var t = target.Find(a => a.Id == section.id);
                            if (t == null) continue;
                            if (!t.Check) continue;

                            if (t.TypeId == null)
                            {
                                //新規断面
                                if (!CreatePile_Product(stb, section, ConvFamily)) { ret = false; errmsg = "既製杭"; }
                            }
                            else
                            {
                                //更新
                                FamilySymbol symbol = Commons.doc.GetElement(t.TypeId[0]) as FamilySymbol;

                                CheckConvertFamily_PileProduct(section, ConvFamily, out Family fam, out string type);

                                if (fam.Name != symbol.Family.Name)
                                {
                                    //差分の前後でファミリが違う
                                    LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={section.id}) ファミリが異なるため変換できません");
                                    continue;
                                }

                                string fugo_paraName = "";
                                if (type == "PHC杭") { fugo_paraName = SetFamily.Pile_PHC.name; }
                                else if (type == "ST杭") { fugo_paraName = SetFamily.Pile_ST.name; }
                                else if (type == "SC杭") { fugo_paraName = SetFamily.Pile_SC.name; }
                                else if (type == "PRC杭") { fugo_paraName = SetFamily.Pile_PRC.name; }
                                else if (type == "CPRC杭") { fugo_paraName = SetFamily.Pile_CPRC.name; }
                                else if (type == "節付PHC杭") { fugo_paraName = SetFamily.Pile_PHC.name; }
                                else if (type == "節付PRC杭") { fugo_paraName = SetFamily.Pile_PRC.name; }
                                else if (type == "節付CPRC杭") { fugo_paraName = SetFamily.Pile_CPRC.name; }


                                foreach (var eid in t.TypeId)
                                {
                                    symbol = Commons.doc.GetElement(eid) as FamilySymbol;

                                    string fugo = Data.GetParameter_string(symbol, fugo_paraName);
                                    SetParameter_PileProduct(section, symbol, type);

                                    if (fugo != section.name)
                                    {
                                        //符号が異なる場合はタイプ名も変える
                                        string typename = GetTypeName_Pile(stb, section.id);
                                        ChangeTypeName(symbol, typename);
                                    }

                                    Data.ReNameSymbols s = new Data.ReNameSymbols
                                    {
                                        symbol = symbol,
                                        id = section.id,
                                        name = section.name,
                                        Length = 0,
                                        Length2 = 0,
                                        Length3 = 0,
                                        BHaunch1 = StbPileKind_structure.PC.ToString(),
                                    };
                                    PilesSymbols.Add(s);

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={section.id}) {symbol.Name}を更新しました");
                                    DebugLogToCommentParam( Commons.doc.GetElement(eid), section.id, logheader, symbol.Name );
                                }

                            }
                        }
                    }
                }


                tran.Commit();
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }



        private static void ChangeTypeName(FamilySymbol symbol, string typename)
        {
            var names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a).Name.ToUpper()).ToList();
            if (names.Contains(typename.ToUpper()))
            {
                //同一ファミリ内に既に同名のタイプ名がある

                string name2 = typename;
                int ascii = 97;
                while (names.Contains(name2.ToUpper()))
                {
                    name2 = Data.ReName(typename, ascii);
                    ascii++;
                }
                typename = name2;
            }
            symbol.Name = typename;
        }
        private static void ChangeTypeName(FloorType symbol, string typename)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            List<FloorType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FloorType>().ToList();

            var names = symbols.Select(a => a.Name.ToUpper()).ToList();
            if (names.Contains(typename.ToUpper()))
            {
                //同一ファミリ内に既に同名のタイプ名がある

                string name2 = typename;
                int ascii = 97;
                while (names.Contains(name2.ToUpper()))
                {
                    name2 = Data.ReName(typename, ascii);
                    ascii++;
                }
                typename = name2;
            }
            symbol.Name = typename;
        }
        private static void ChangeTypeName(WallType symbol, string typename)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<WallType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<WallType>().Where(a => a.Kind == WallKind.Basic).ToList();

            var names = symbols.Select(a => a.Name.ToUpper()).ToList();
            if (names.Contains(typename.ToUpper()))
            {
                //同一ファミリ内に既に同名のタイプ名がある

                string name2 = typename;
                int ascii = 97;
                while (names.Contains(name2.ToUpper()))
                {
                    name2 = Data.ReName(typename, ascii);
                    ascii++;
                }
                typename = name2;
            }
            symbol.Name = typename;
        }


        private static Family[][] GetConvFamily_Column(string syubetu)
        {
            //変換ファミリの取得
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilySymbol> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            Family[][] ConvFamily;
            if (syubetu == "基礎柱")
            {
                ConvFamily = new Family[RevitLNK.FClmText.Length][];
                for (int i = 0; i < RevitLNK.FClmText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.FClmText[i].Length);
                }
            }
            else
            {
                ConvFamily = new Family[RevitLNK.ClmText.Length][];
                for (int i = 0; i < RevitLNK.ClmText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.ClmText[i].Length);
                }
            }

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (var familysymbol in symbols)
                    {
                        if (syubetu == "柱")
                        {
                            if (!SetFamily.ClmFName.flg[i][j]) { continue; }
                            if (!SetFamily.ClmFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.ClmFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                        else if (syubetu == "基礎柱")
                        {
                            if (!SetFamily.BClmFName.flg[i][j]) { continue; }
                            if (!SetFamily.BClmFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.BClmFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                        else
                        {
                            if (!SetFamily.PClmFName.flg[i][j]) { continue; }
                            if (!SetFamily.PClmFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.PClmFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                    }
                }
            }

            return ConvFamily;
        }

        private static Family[][] GetConvFamily_Girder(string syubetu)
        {
            //変換ファミリの取得
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilySymbol> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            Family[][] ConvFamily = null;
            if (syubetu == "大梁" || syubetu == "小梁")
            {
                ConvFamily = new Family[RevitLNK.GirText.Length][];
                for (int i = 0; i < RevitLNK.GirText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.GirText[i].Length);
                }
            }
            else
            {
                ConvFamily = new Family[RevitLNK.CGirText.Length][];
                for (int i = 0; i < RevitLNK.CGirText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.CGirText[i].Length);
                }
            }


            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (var familysymbol in symbols)
                    {
                        if (syubetu == "大梁")
                        {
                            if (!SetFamily.GirFName.flg[i][j]) { continue; }
                            if (!SetFamily.GirFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.GirFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                        else if (syubetu == "小梁")
                        {
                            if (!SetFamily.BeamFName.flg[i][j]) { continue; }
                            if (!SetFamily.BeamFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.BeamFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                        else if (syubetu == "片持梁")
                        {
                            if (!SetFamily.CGirFName.flg[i][j]) { continue; }
                            if (!SetFamily.CGirFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.CGirFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                        else if (syubetu == "片持小梁")
                        {
                            if (!SetFamily.CBeamFName.flg[i][j]) { continue; }
                            if (!SetFamily.CBeamFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.CBeamFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                            }
                        }
                    }
                }
            }

            return ConvFamily;
        }

        private static Family[][] GetConvFamily_Brace()
        {
            //変換ファミリの取得
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilySymbol> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            Family[][] ConvFamily = new Family[RevitLNK.SBraText.Length][];
            for (int i = 0; i < RevitLNK.SBraText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.SBraText[i].Length);
            }

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.SBraFName.flg[i][j]) { continue; }
                    if (!SetFamily.SBraFName.convflg[i][j]) { continue; }

                    foreach (var familysymbol in symbols)
                    {
                        if (familysymbol.FamilyName == SetFamily.SBraFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            break;
                        }
                    }
                }
            }

            return ConvFamily;
        }

        private static Family[][] GetConvFamily_Foundation()
        {
            //変換ファミリの取得
            Family[][] ConvFamily = new Family[RevitLNK.BaseText.Length][];
            for (int i = 0; i < RevitLNK.BaseText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.BaseText[i].Length);
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<FamilySymbol> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (var familysymbol in symbols)
                    {
                        if (!SetFamily.FoFName.flg[i][j]) { continue; }
                        if (!SetFamily.FoFName.convflg[i][j]) { continue; }

                        if (familysymbol.FamilyName == SetFamily.FoFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            break;
                        }
                    }
                }
            }

            return ConvFamily;
        }



        internal static void UpdateMember(ST_BRIDGE stb, List<ConvertForm.Chb_class> chb, bool convertOffset)
        {
            string errmsg = "";

            Transaction tran = null;
            try
            {
                if (!UpdateLevel(stb))
                {
                    return;
                }

                if (!UpdateGrid(stb))
                {
                    return;
                }


                //部材の更新
                tran = new Transaction(Commons.doc, "部材の更新");
                tran.Start();

                for (int i = 0; i < chb.Count(); i++)
                {
                    if (!chb[i].chbchecked) { continue; }
                    switch (chb[i].buzai)
                    {
                        case "柱":
                        case "間柱":
                            bool convBase = chb.Find(a => a.buzai == "柱脚")?.chbchecked ?? false;
                            if (!UpdateColumn(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset, convBase))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "基礎柱":
                            if (!UpdateFoundationColumn(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "大梁":
                        case "小梁":
                        case "片持梁":
                        case "片持小梁":
                            if (!UpdateGirder(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "RCスラブ":
                        case "デッキプレート":
                        case "既製スラブ":
                        case "基礎スラブ":
                            if (!UpdateSlab(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "Sブレース":
                            if (!UpdateBrace(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                        case "壁":
                            if (!UpdateWall(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;
                        case "RCパラペット":
                            if (stb.StbModel.StbMembers.StbParapets != null &&
                                stb.StbModel.StbMembers.StbParapets.Count > 0)
                            {
                                LogData.AddLog(LogData.LogKind.Warning, 0, "パラペットは対象外です");
                            }
                            break;

                        case "基礎・布基礎・杭":
                            if (!UpdateFoooting(stb, chb[i].buzai, SabunTarget_I[chb[i].buzai], out errmsg, convertOffset))
                            {
                                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
                            }
                            break;

                    }
                }

            }
            catch
            {
                //ログ出力
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }
            finally
            {
                if (tran != null && tran.HasStarted())
                {
                    tran.Commit();
                }
            }

        }


        private static bool UpdateLevel(ST_BRIDGE stb)
        {
            Levels = new List<Level>();

            if (stb.StbModel.StbStories != null && stb.StbModel.StbStories.Count > 0)
            {
                const string logheader = "StbStories";
                Data.ProgressRestart("レベルの更新", stb.StbModel.StbStories.Count);

                Transaction tran = new Transaction(Commons.doc, "レベルの更新");
                tran.Start();
                try
                {
                    FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                    ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
                    List<Level> levels = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Level>().ToList();
                    var names = levels.Select(a => a.Name).ToList();

                    foreach (var story in stb.StbModel.StbStories)
                    {
                        Data.ProgressPerformStep();

                        //GUIDのないものは対象外
                        if (story.guid == null || story.guid == "") continue;



                        var eid = Data.GetStorageElementId(story.guid);
                        if (eid == null)
                        {
                            //新規
                            Level lv = Level.Create(Commons.doc, Commons.mm2ft(story.height));

                            string name = story.name;
                            if (Data.Name_Check(names, name))
                            {
                                int ascii = 97;
                                string rename = "";
                                do
                                {
                                    rename = name + "_" + (char)ascii;
                                    ascii++;

                                } while (Data.Name_Check(names, rename));
                                name = rename;
                                names.Add(name);
                            }

                            lv.Name = name;

                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={story.id}) {lv.Name}を作成しました");
                            Levels.Add(lv);
                        }
                        else
                        {
                            //更新
                            if (!(Commons.doc.GetElement(eid[0]) is Level lv)) continue;

                            double elevation = Commons.mm2ft(story.height);
                            if (Math.Abs(elevation - lv.Elevation) > 0.001)
                            {
                                lv.Elevation = Commons.mm2ft(story.height);
                            }

                            if (lv.Name != story.name)
                            {
                                string name = story.name;
                                if (Data.Name_Check(names, name))
                                {
                                    int ascii = 97;
                                    string rename = "";
                                    do
                                    {
                                        rename = name + "_" + (char)ascii;
                                        ascii++;

                                    } while (Data.Name_Check(names, rename));
                                    name = rename;
                                    names.Add(name);
                                }

                                lv.Name = name;
                            }

                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={story.id}) {lv.Name}を更新しました");
                            DebugLogToCommentParam( lv, story.id, logheader, lv.Name );
                            Levels.Add(lv);
                        }
                    }

                    tran.Commit();

                    //高い順に並び替える
                    Levels = Levels.OrderByDescending(a => a.Elevation).ToList();
                }
                catch (Exception ex)
                {
                    tran.RollBack();
                    LogData.AddLog(LogData.LogKind.Error, 0, "レベル更新 " + ex.Message);
                    return false;
                }
                finally
                {
                    Data.ProgressClose();
                }
            }

            return true;
        }

        private static bool UpdateGrid(ST_BRIDGE stb)
        {
            Transaction tran = new Transaction(Commons.doc, "通り軸の更新");
            tran.Start();
            try
            {
                double entyou = Commons.mm2ft(3000); //グリッドを延長する（始点側)


                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
                List<Element> grids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().ToList();
                var names = grids.Select(a => a.Name).ToList();

                //グリッドのタイプの設定
                collector = new FilteredElementCollector(Commons.doc);
                List<GridType> elems = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<GridType>().ToList();
                GridType gt = null;
                if (elems != null && elems.Count != 0)
                {
                    foreach (var el in elems)
                    {
                        gt = el;
                        if (gt.Name == "通り心記号_始端")
                        { break; }
                    }
                }


                int count = stb.StbModel.StbAxes.StbParallelAxes?.Sum(a => a.StbParallelAxis.Count) ?? 0;
                count += stb.StbModel.StbAxes.StbArcAxes?.Sum(a => a.StbArcAxis.Count) ?? 0;
                count += stb.StbModel.StbAxes.StbRadialAxes?.Sum(a => a.StbRadialAxis.Count) ?? 0;

                Data.ProgressRestart("通り軸の更新", count);

                //平行軸
                if (stb.StbModel.StbAxes.StbParallelAxes != null)
                {
                    const string logheader = "StbParallelAxis";

                    //作図面
                    XYZ normal2 = new XYZ(0, 0, 1);
                    Plane p = Plane.CreateByNormalAndOrigin(normal2, new XYZ(0, 0, 0));
                    SketchPlane skp = SketchPlane.Create(Commons.doc, p);

                    foreach (var axisGroup in stb.StbModel.StbAxes.StbParallelAxes)
                    {
                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X + alloffsetX, axisGroup.Y + alloffsetY, 0));
                        double rad = axisGroup.angle * Math.PI / 180;
                        XYZ vec1 = new XYZ(Math.Cos(rad), Math.Sin(rad), 0);
                        XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();

                        //直交軸
                        var orthogonal = stb.StbModel.StbAxes.StbParallelAxes.Find(a => Math.Abs(Math.Abs(a.angle - axisGroup.angle) - 90) < 0.01 ||
                                                                                        Math.Abs(Math.Abs(a.angle - axisGroup.angle) - 270) < 0.01);
                        XYZ orthogonal_origin = null;
                        XYZ orthogonal_vec1 = null;
                        XYZ orthogonal_vec2 = null;
                        if (orthogonal != null)
                        {
                            //直行軸が存在していれば、始点に近い軸位置に始点を移動する
                            double orthogonal_rad = orthogonal.angle * Math.PI / 180;
                            orthogonal_origin = Commons.mm2ft(new XYZ(orthogonal.X, orthogonal.Y, 0));
                            orthogonal_vec1 = new XYZ(Math.Cos(orthogonal_rad), Math.Sin(orthogonal_rad), 0);
                            orthogonal_vec2 = XYZ.BasisZ.CrossProduct(orthogonal_vec1).Normalize();
                        }

                        foreach (var axis in axisGroup.StbParallelAxis)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (axis.guid == null || axis.guid == "") continue;


                            bool create_flag = false;

                            var eid = Data.GetStorageElementId(axis.guid);
                            if (eid == null)
                            {
                                create_flag = true;
                            }
                            else
                            {
                                MultiSegmentGrid m_grid = Commons.doc.GetElement(eid[0]) as MultiSegmentGrid;

                                if (m_grid == null)
                                {
                                    create_flag = true;
                                }
                                else
                                {
                                    var grid_ids = m_grid.GetGridIds().ToList();
                                    Grid grid = Commons.doc.GetElement(grid_ids[0]) as Grid;
                                    XYZ p1 = grid.Curve.GetEndPoint(0);

                                    Grid grid2 = Commons.doc.GetElement(grid_ids.Last()) as Grid;
                                    XYZ p2 = grid2.Curve.GetEndPoint(1);

                                    //名称
                                    if (m_grid.Name != axis.name)
                                    {
                                        string name = axis.name;
                                        if (Data.Name_Check(names, name))
                                        {
                                            int ascii = 97;
                                            string rename = "";
                                            do
                                            {
                                                rename = name + "_" + (char)ascii;
                                                ascii++;

                                            } while (Data.Name_Check(names, rename));
                                            name = rename;
                                            names.Add(name);
                                        }

                                        m_grid.Name = name;
                                    }

                                    //折れ曲がりは変更できないので始点位置のみ動かす
                                    double distance1 = Commons.mm2ft(axis.distance);
                                    double distance2 = vec2.DotProduct(p1 - origin);
                                    double distance3 = vec2.DotProduct(p2 - origin);
                                    if (Math.Abs(distance1 - distance2) > 0.001 && Math.Abs(distance1 - distance3) > 0.001)
                                    {
                                        //軸の始端と終端、両方がずれている場合のみ移動。片方でも代表距離と一致している場合は動かさない。
                                        //始端側が折れ曲がっていると移動してしまうため。
                                        m_grid.Location.Move(vec2 * (distance1 - distance2));
                                    }


                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={axis.id}) {grid.Name}を更新しました");
                                    DebugLogToCommentParam( grid, axis.id, logheader, grid.Name );
                                }
                            }

                            if (create_flag)
                            {
                                //新規
                                Create_ParallelAxis(stb, entyou, $"平行軸({axisGroup.group_name})", gt, names, skp, origin, vec1, vec2, orthogonal, orthogonal_origin, orthogonal_vec2, axis);
                            }

                        }
                    }
                }

                //円弧軸
                if (stb.StbModel.StbAxes.StbArcAxes != null)
                {
                    const string logheader = "StbArcAxis";

                    foreach (var axisGroup in stb.StbModel.StbAxes.StbArcAxes)
                    {
                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X, axisGroup.Y, 0));
                        double rad_s = axisGroup.start_angle * Math.PI / 180;
                        double rad_e = axisGroup.end_angle * Math.PI / 180;
                        if (rad_s < rad_e)
                        {
                            //例: S=30 ～ E=150 -> 第三、第四象限を通る円弧
                            //Revitで作れないので2π加算する
                            rad_s += Math.PI * 2;
                        }
                        else
                        {
                            //例: S=150 ～ E=30 -> OK
                        }

                        foreach (var axis in axisGroup.StbArcAxis)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (axis.guid == null || axis.guid == "") continue;



                            double radius = Commons.mm2ft(axis.radius);
                            bool create_flag = false;

                            var eid = Data.GetStorageElementId(axis.guid);
                            if (eid == null)
                            {
                                create_flag = true;
                            }
                            else
                            {
                                Grid grid = Commons.doc.GetElement(eid[0]) as Grid;

                                if (grid == null)
                                {
                                    create_flag = true;
                                }
                                else
                                {
                                    var curve = grid.Curve as Arc;
                                    if (curve == null)
                                    {
                                        create_flag = true;
                                    }
                                    else
                                    {
                                        if (Math.Abs(curve.Radius - radius) > 0.001)
                                        {
                                            //半径が異なる場合は更新できない
                                            create_flag = true;
                                        }
                                        else
                                        {
                                            //更新

                                            //名称
                                            if (grid.Name != axis.name)
                                            {
                                                string name = axis.name;
                                                if (Data.Name_Check(names, name))
                                                {
                                                    int ascii = 97;
                                                    string rename = "";
                                                    do
                                                    {
                                                        rename = name + "_" + (char)ascii;
                                                        ascii++;

                                                    } while (Data.Name_Check(names, rename));
                                                    name = rename;
                                                    names.Add(name);
                                                }

                                                grid.Name = name;
                                            }

                                            //角度
                                            //curve.GetEndParameter(0)で角度取れるが、
                                            //2π超えていたり、マイナスだったりするので、チェックが面倒。常に更新する。
                                            Arc newArc = Arc.Create(curve.Center, curve.Radius, rad_e, rad_s, curve.XDirection, curve.YDirection);
                                            grid.SetCurveInView(DatumExtentType.Model, Commons.doc.ActiveView, newArc);


                                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={axis.id}) {grid.Name}を更新しました");
                                            DebugLogToCommentParam( grid, axis.id, logheader, grid.Name );
                                        }
                                    }
                                }
                            }

                            if (create_flag)
                            {
                                //新規
                                Create_ArclAxis($"円弧軸({axisGroup.group_name})", names, origin, rad_s, rad_e, axis);
                            }

                        }
                    }
                }

                //放射軸
                if (stb.StbModel.StbAxes.StbRadialAxes != null)
                {
                    const string logheader = "StbRadialAxes";

                    double length1 = entyou;
                    double length2 = Commons.mm2ft(10000);

                    if (stb.StbModel.StbAxes.StbArcAxes != null && stb.StbModel.StbAxes.StbArcAxes.Count > 0)
                    {
                        //円弧軸がある場合は、放射軸長さを円弧軸から決める
                        length1 = Commons.mm2ft(stb.StbModel.StbAxes.StbArcAxes.Min(a => a.StbArcAxis.Min(b => b.radius))) - entyou;
                        length2 = Commons.mm2ft(stb.StbModel.StbAxes.StbArcAxes.Max(a => a.StbArcAxis.Max(b => b.radius))) + entyou;
                    }

                    foreach (var axisGroup in stb.StbModel.StbAxes.StbRadialAxes)
                    {
                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X, axisGroup.Y, 0));

                        foreach (var axis in axisGroup.StbRadialAxis)
                        {
                            Data.ProgressPerformStep();

                            //GUIDのないものは対象外
                            if (axis.guid == null || axis.guid == "") continue;



                            double rad = axis.angle * Math.PI / 180;
                            XYZ vec1 = new XYZ(Math.Cos(rad), Math.Sin(rad), 0);
                            XYZ p1 = origin + vec1 * length1;
                            XYZ p2 = origin + vec1 * length2;

                            bool create_flag = false;


                            var eid = Data.GetStorageElementId(axis.guid);
                            if (eid == null)
                            {
                                create_flag = true;
                            }
                            else
                            {
                                Grid grid = Commons.doc.GetElement(eid[0]) as Grid;
                                if (grid == null)
                                {
                                    create_flag = true;
                                }
                                else
                                {
                                    var curve = grid.Curve as Line;
                                    if (curve == null)
                                    {
                                        create_flag = true;
                                    }
                                    else
                                    {
                                        //更新

                                        //名称
                                        if (grid.Name != axis.name)
                                        {
                                            string name = axis.name;
                                            if (Data.Name_Check(names, name))
                                            {
                                                int ascii = 97;
                                                string rename = "";
                                                do
                                                {
                                                    rename = name + "_" + (char)ascii;
                                                    ascii++;

                                                } while (Data.Name_Check(names, rename));
                                                name = rename;
                                                names.Add(name);
                                            }

                                            grid.Name = name;
                                        }


                                        XYZ p3 = curve.GetEndPoint(0);
                                        XYZ p4 = curve.GetEndPoint(1);
                                        XYZ v1 = (p4 - p3).Normalize();
                                        XYZ v2 = XYZ.BasisZ.CrossProduct(v1).Normalize();

                                        //長さ
                                        double length = p1.DistanceTo(p2);
                                        if (Math.Abs(length - curve.Length) > 0.001)
                                        {
                                            Line newLine = Line.CreateBound(p2, p1);
                                            grid.SetCurveInView(DatumExtentType.Model, Commons.doc.ActiveView, newLine);
                                        }

                                        //座標
                                        if (p3.DistanceTo(p2) > 0.001)
                                        {
                                            grid.Location.Move(p2 - p3);
                                        }

                                        //角度
                                        double angle = XYZ.BasisX.AngleOnPlaneTo(v1, XYZ.BasisZ);
                                        if (Math.Abs(angle - rad) > 0.001)
                                        {
                                            grid.Location.Rotate(Line.CreateBound(p3, p3 + XYZ.BasisZ), rad - angle);
                                        }


                                        LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={axis.id}) {grid.Name}を更新しました");
                                        DebugLogToCommentParam( grid, axis.id, logheader, grid.Name );
                                    }
                                }
                            }


                            if (create_flag)
                            {
                                //新規
                                Create_RadialAxis($"放射軸({axisGroup.group_name})", names, length1, length2, origin, axis);
                            }
                        }
                    }

                }


                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.RollBack();
                LogData.AddLog(LogData.LogKind.Error, 0, "通り軸更新 " + ex.Message);
                return false;
            }
            finally
            {
                Data.ProgressClose();
            }


            return true;
        }



        private static bool UpdateColumn(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset, bool convBase)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                Family[][] ConvFamily = GetConvFamily_Column(syubetu);

                //柱脚ファミリの取得
                GetBaseFamily(out Dictionary<string, Family> BClmFamily, out Dictionary<string, string> mappingTypeName);


                string logheader = "StbColumns";
                List<StbColumn> columns = null;
                if (syubetu == "柱")
                {
                    if (stb.StbModel.StbMembers.StbColumns == null) return true;

                    columns = stb.StbModel.StbMembers.StbColumns;
                }
                else
                {
                    if (stb.StbModel.StbMembers.StbPosts == null) return true;

                    columns = stb.StbModel.StbMembers.StbPosts.OfType<StbColumn>().ToList();
                    logheader = "StbPosts";
                }
                if (columns.Count == 0) return true;


                Data.ProgressRestart($"{syubetu}の更新", columns.Count);
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

                foreach (var member in columns)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //CheckをOFFにしたものは対象外
                    var t = target.Find(a => a.Id == member.id_section);
                    if (t == null) continue;
                    if (!t.Check) continue;

                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);


                    Data.BaseClass newb = null;
                    switch (member.kind_structure)
                    {
                        case StbColumnKind_structure.RC:
                            break;
                        case StbColumnKind_structure.S:
                            var sec2 = stb.StbModel.StbSections.StbSecColumn_S.Find(a => a.id == member.id_section);
                            if (sec2.Item != null && sec2.Item is StbSecBaseProduct_S baseProduct)
                            {
                                newb = new Data.BaseClass
                                {
                                    id_section = sec2.id,
                                    clmname = sec2.name,
                                    clm_structure = "S",
                                    product_company = baseProduct.product_company,
                                    product_code = baseProduct.product_code
                                };
                                BClm.Add(newb);
                            }
                            break;
                        case StbColumnKind_structure.SRC:
                            var sec3 = stb.StbModel.StbSections.StbSecColumn_SRC.Find(a => a.id == member.id_section);
                            if (sec3.Item != null && sec3.Item is StbSecBaseProduct_SRC baseProduct3)
                            {
                                newb = new Data.BaseClass
                                {
                                    id_section = sec3.id,
                                    clmname = sec3.name,
                                    clm_structure = "SRC",
                                    product_company = baseProduct3.product_company,
                                    product_code = baseProduct3.product_code
                                };
                                BClm.Add(newb);
                            }
                            break;
                        case StbColumnKind_structure.CFT:
                            var sec4 = stb.StbModel.StbSections.StbSecColumn_CFT.Find(a => a.id == member.id_section);
                            if (sec4.Item != null && sec4.Item is StbSecBaseProduct_CFT baseProduct4)
                            {
                                newb = new Data.BaseClass
                                {
                                    id_section = sec4.id,
                                    clmname = sec4.name,
                                    clm_structure = "CFT",
                                    product_company = baseProduct4.product_company,
                                    product_code = baseProduct4.product_code
                                };
                                BClm.Add(newb);
                            }
                            break;
                    }


                    if (ids == null)
                    {
                        //新規インスタンス
                        if (!CreateColumn_instance(stb, member, ConvFamily)) { ret = false; errmsg = $"{syubetu}"; }

                        //柱脚
                        if (!convBase) continue;


                        if (newb == null) continue;
                        if (!mappingTypeName.ContainsKey(newb.product_code)) continue;
                        if (!BClmFamily.ContainsKey(newb.product_code)) continue;

                        string typename = mappingTypeName[newb.product_code]; //マッピングテーブルで指定されたタイプ名
                        Family baseFamily = BClmFamily[newb.product_code];    //マッピングテーブルで指定されたファミリ名
                        if (baseFamily == null) continue;

                        FamilySymbol symbol = null;

                        if (typename == "")
                        { typename = baseFamily.Name; }

                        if (!Data.SearchFamilySymbol(baseFamily, typename, ref symbol))
                        {
                            symbol = (FamilySymbol)symbol.Duplicate(typename);
                        }

                        if (!symbol.IsActive)
                        {
                            symbol.Activate();
                        }

                        CreateClmBase(stb, typename, symbol, member);

                    }
                    else
                    {
                        //更新

                        string typename = GetTypeName_Column(stb, member.id_section);
                        bool isReferenceDirection = true;
                        string guid = "";
                        StbSecColumn_Kind_column kind_column = StbSecColumn_Kind_column.COLUMN;
                        switch (member.kind_structure)
                        {
                            case StbColumnKind_structure.RC:
                                var sec1 = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == member.id_section);
                                guid = sec1.guid;
                                kind_column = sec1.kind_column;
                                break;
                            case StbColumnKind_structure.S:
                                var sec2 = stb.StbModel.StbSections.StbSecColumn_S.Find(a => a.id == member.id_section);
                                isReferenceDirection = sec2.isReferenceDirection;
                                guid = sec2.guid;
                                kind_column = sec2.kind_column;
                                break;
                            case StbColumnKind_structure.SRC:
                                var sec3 = stb.StbModel.StbSections.StbSecColumn_SRC.Find(a => a.id == member.id_section);
                                guid = sec3.guid;
                                kind_column = sec3.kind_column;
                                break;
                            case StbColumnKind_structure.CFT:
                                var sec4 = stb.StbModel.StbSections.StbSecColumn_CFT.Find(a => a.id == member.id_section);
                                guid = sec4.guid;
                                kind_column = sec4.kind_column;
                                break;
                        }


                        //配置座標の取得
                        XYZ Pt = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_top, 0, 0, 0);
                        XYZ Pb = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_bottom, 0, 0, 0);
                        XYZ offset_t = new XYZ();
                        XYZ offset_b = new XYZ();
                        if (convOffset)
                        {
                            offset_t = Commons.mm2ft(new XYZ(member.offset_top_X, member.offset_top_Y, member.offset_top_Z));
                            offset_b = Commons.mm2ft(new XYZ(member.offset_bottom_X, member.offset_bottom_Y, member.offset_bottom_Z));
                        }


                        double angle2 = member.rotate * Math.PI / 180;
                        if (!isReferenceDirection)
                        {
                            angle2 += Math.PI / 2;
                        }


                        //柱に紐づく柱脚も同じGuidで登録してある
                        bool check = false;
                        foreach (var elmid in ids)
                        {
                            FamilyInstance instance = Commons.doc.GetElement(elmid) as FamilyInstance;

                            if (instance.Category.Id.Value() == (long)BuiltInCategory.OST_StructuralColumns)
                            {
                                //柱

                                if (guid != "" && Data.GetStorageGuid(instance.Symbol.Id) != guid)
                                {
                                    //タイプが異なる場合は差し替え
                                    foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                                    {
                                        if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                        {
                                            if (Data.GetStorageGuid(symbol.Id) == guid)
                                            {
                                                instance.ChangeTypeId(id);
                                                break;
                                            }
                                        }
                                    }
                                }

                                //解析線分
                                AnalyticalMember am = null;
                                if (amanager.HasAssociation(instance.Id))
                                {
                                    am = Commons.doc.GetElement(amanager.GetAssociatedElementId(instance.Id)) as AnalyticalMember;
                                }

                                if (instance.Location is LocationPoint locp)
                                {
                                    //座標
                                    XYZ p1 = null;
                                    if (convOffset)
                                    {
                                        //寄りありの場合、柱基点との差分を動かす
                                        p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                    }
                                    else
                                    {
                                        if (am != null)
                                        {
                                            //寄りなしの場合、解析線分座標との差分を動かす
                                            //基点と解析線分の相対位置は変わらない
                                            p1 = am.GetCurve().GetEndPoint(0);
                                            p1 = new XYZ(p1.X, p1.Y, 0);
                                        }
                                        else
                                        {
                                            p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                        }
                                    }

                                    var p2 = new XYZ(Pb.X, Pb.Y, 0) + new XYZ(offset_b.X, offset_b.Y, 0);
                                    if (p1.DistanceTo(p2) > 0.001)
                                    {
                                        instance.Location.Move(p2 - p1);
                                        if (am != null)
                                        {
                                            am.SetCurve(Line.CreateBound(Pb, Pt));
                                        }
                                    }


                                    //上下レベルオフセット：節点とレベルに差があっても位置のときは更新しない。最初のオフセットがなくなるから。
                                    if (convOffset)
                                    {
                                        ElementId eid = instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                                        Level lv1 = Commons.doc.GetElement(eid) as Level;
                                        double offsetZb = Pb.Z - lv1.Elevation;

                                        eid = instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();
                                        Level lv2 = Commons.doc.GetElement(eid) as Level;
                                        double offsetZt = Pt.Z - lv2.Elevation;

                                        if (convOffset)
                                        {
                                            if (member.offset_bottom_Z == 0) //柱脚Z方向オフセット値が0以外の時はその値を優先する
                                            {
                                                Search_Girder_Offset_Z_bottom(stb, member.id_node_bottom, lv1, member.kind_structure, out var gir_offset_Z_bottom);
                                                offsetZb += Commons.mm2ft(gir_offset_Z_bottom);
                                            }
                                            else
                                            {
                                                offsetZb += Commons.mm2ft(member.offset_bottom_Z);
                                            }

                                            if (member.offset_top_Z == 0) //柱頭Z方向オフセット値が0以外の時はその値を優先する
                                            {
                                                Search_Girder_Offset_Z_top(stb, member.id_node_top, lv2, member.kind_structure, out var gir_offset_Z_top, kind_column);
                                                offsetZt += Commons.mm2ft(gir_offset_Z_top);
                                            }
                                            else
                                            {
                                                offsetZt += Commons.mm2ft(member.offset_top_Z);
                                            }
                                        }

                                        double z1 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, true);
                                        double z2 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, true);

                                        if (Math.Abs(z1 - offsetZb) > 0.001)
                                        {
                                            Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, offsetZb, false);
                                        }
                                        if (Math.Abs(z2 - offsetZt) > 0.001)
                                        {
                                            Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, offsetZt, false);
                                        }
                                    }


                                    //回転
                                    //instance.HandOrientation無回転なら (1,0,0)
                                    double angle1 = XYZ.BasisX.AngleOnPlaneTo(instance.HandOrientation, XYZ.BasisZ);
                                    if (Math.Abs(angle2 - angle1) > 0.001)
                                    {
                                        instance.Location.Rotate(Line.CreateBound(p2, p2 + BasisZ), angle2 - angle1);
                                    }

                                }
                                else if (instance.Location is LocationCurve locc)
                                {
                                    var p0 = locc.Curve.GetEndPoint(0);
                                    var p1 = locc.Curve.GetEndPoint(1);
                                    XYZ Pt2 = Pt;
                                    XYZ Pb2 = Pb;
                                    if (convOffset)
                                    {
                                        Pb2 += offset_b;
                                        Pt2 += offset_t;
                                    }
                                    else
                                    {
                                        if (am != null)
                                        {
                                            p0 = am.GetCurve().GetEndPoint(0);
                                            p1 = am.GetCurve().GetEndPoint(1);
                                        }
                                    }


                                    XYZ vec1 = (p1 - p0).Normalize();
                                    XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();
                                    double angle1 = vec2.AngleOnPlaneTo(instance.HandOrientation, vec1);

                                    //座標（上下オフセット込み）
                                    if (p0.DistanceTo(Pb2) > 0.001 ||
                                        p1.DistanceTo(Pt2) > 0.001)
                                    {
                                        locc.Curve = Line.CreateBound(Pb2, Pt2);
                                        if (am != null)
                                        {
                                            am.SetCurve(Line.CreateBound(Pb, Pt));
                                        }
                                    }


                                    //回転
                                    if (Math.Abs(angle2 - angle1) > 0.001)
                                    {
                                        XYZ vec3 = (Pt2 - Pb2).Normalize();
                                        instance.Location.Rotate(Line.CreateBound(Pb, Pb + vec3), angle2 - angle1);
                                    }
                                }


                                //インスタンスパラメータ更新
                                SetInstanceParameter_Column(stb, member, instance);


                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                DebugLogToCommentParam( instance, member.id, logheader, member.name );
                            }
                            else
                            {
                                //柱脚
                                check = true;

                                if (!convBase) continue;

                                if (instance.Location is LocationPoint locp)
                                {
                                    //座標
                                    var p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                    var p2 = new XYZ(Pb.X, Pb.Y, 0);
                                    if (p1.DistanceTo(p2) > 0.001)
                                    {
                                        instance.Location.Move(p2 - p1);
                                    }

                                    //オフセット
                                    if (convOffset)
                                    {
                                        Data.SetParameter(instance, BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM, Pb.Z, false);
                                    }

                                    //回転
                                    double angle1 = XYZ.BasisX.AngleOnPlaneTo(instance.HandOrientation, XYZ.BasisZ);
                                    if (Math.Abs(angle2 - angle1) > 0.001)
                                    {
                                        instance.Location.Rotate(Line.CreateBound(p2, p2 + BasisZ), angle2 - angle1);
                                    }

                                    LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                    DebugLogToCommentParam( instance, member.id, logheader, member.name );
                                }
                            }
                        }


                        if (newb != null && !check)
                        {
                            //元が柱脚なしなので、柱脚が更新できていない。新たに作る
                            if (!mappingTypeName.ContainsKey(newb.product_code)) continue;
                            if (!BClmFamily.ContainsKey(newb.product_code)) continue;

                            typename = mappingTypeName[newb.product_code]; //マッピングテーブルで指定されたタイプ名
                            Family baseFamily = BClmFamily[newb.product_code];    //マッピングテーブルで指定されたファミリ名
                            if (baseFamily == null) continue;

                            FamilySymbol symbol = null;

                            if (typename == "")
                            { typename = baseFamily.Name; }

                            if (!Data.SearchFamilySymbol(baseFamily, typename, ref symbol))
                            {
                                symbol = (FamilySymbol)symbol.Duplicate(typename);
                            }

                            if (!symbol.IsActive)
                            {
                                symbol.Activate();
                            }

                            CreateClmBase(stb, typename, symbol, member);
                        }
                    }
                }


            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateFoundationColumn(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                if (stb.StbModel.StbMembers.StbFoundationColumns == null) return true;
                if (stb.StbModel.StbMembers.StbFoundationColumns.Count == 0) return true;


                //変換ファミリの取得
                Family[][] ConvFamily = new Family[RevitLNK.FClmText.Length][];
                for (int i = 0; i < RevitLNK.FClmText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.FClmText[i].Length);
                }

                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
                var elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

                for (int i = 0; i < ConvFamily.Length; i++)
                {
                    for (int j = 0; j < ConvFamily[i].Length; j++)
                    {
                        foreach (var familysymbol in elements)
                        {
                            if (familysymbol.FamilyName == SetFamily.BClmFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;
                                break;
                            }
                        }
                    }
                }

                const string logheader = "StbFoundationColumns";

                Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbFoundationColumns.Count);

                foreach (var member in stb.StbModel.StbMembers.StbFoundationColumns)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);


                    double angle2 = member.rotate * Math.PI / 180;



                    int kk = 0;
                    for (int k = 0; k <= 1; ++k)
                    {
                        //CheckをOFFにしたものは対象外
                        int id_section = k == 0 ? member.id_section_FD : member.id_section_WR;
                        var t = target.Find(a => a.Id == id_section);
                        if (t == null) continue;
                        if (!t.Check) continue;

                        double height = k == 0 ? member.length_FD : member.length_WR;
                        if (height < 1)
                        {
                            continue;
                        }

                        var sec = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == id_section);
                        if (sec == null)
                        {
                            continue;
                        }


                        if (ids == null)
                        {
                            //新規インスタンス
                            if (!CreateFoundationColumn_instance(stb, member, ConvFamily)) { ret = false; errmsg = $"{syubetu}"; }
                        }
                        else if (ids.Count > kk)
                        {
                            //更新
                            FamilyInstance instance = Commons.doc.GetElement(ids[kk]) as FamilyInstance;
                            kk++;

                            //string typename = GetTypeName_Column(stb, sec.id);
                            if (Data.GetStorageGuid(instance.Symbol.Id) != sec.guid)
                            {
                                //タイプが異なる場合は差し替え
                                foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                                {
                                    if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                    {
                                        if (Data.GetStorageGuid(symbol.Id) == sec.guid)
                                        {
                                            instance.ChangeTypeId(id);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (instance.Location is LocationPoint locp)
                            {
                                //配置座標の取得
                                XYZ Pt = new XYZ();
                                XYZ Pb = new XYZ();
                                if (k == 0)
                                {
                                    if (convOffset)
                                    {
                                        Pt = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_FD_X, member.offset_FD_Y, member.offset_Z);
                                        Pb = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_FD_X, member.offset_FD_Y, -height + member.offset_Z);
                                    }
                                    else
                                    {
                                        Pt = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, 0, 0, 0);
                                        Pb = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, 0, 0, -height);
                                    }
                                }
                                else
                                {
                                    if (convOffset)
                                    {
                                        Pt = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_WR_X, member.offset_WR_Y, member.offset_Z + height);
                                        Pb = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_WR_X, member.offset_WR_Y, member.offset_Z);
                                    }
                                    else
                                    {
                                        Pt = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, 0, 0, height);
                                        Pb = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, 0, 0, 0);
                                    }
                                }

                                //座標
                                var p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                var p2 = new XYZ(Pb.X, Pb.Y, 0);
                                if (p1.DistanceTo(p2) > 0.001)
                                {
                                    instance.Location.Move(p2 - p1);
                                }


                                //上下レベルオフセット：節点とレベルに差があっても位置のときは更新しない。最初のオフセットがなくなるから。
                                if (convOffset)
                                {
                                    ElementId eid = instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                                    Level lv = Commons.doc.GetElement(eid) as Level;
                                    double offsetZb = Pb.Z - lv.Elevation;
                                    
                                    eid = instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();
                                    lv = Commons.doc.GetElement(eid) as Level;
                                    double offsetZt = Pt.Z - lv.Elevation;

                                    // Console.WriteLine($"*00: Zt:{Commons.ft2mm(offsetZt).ToString("F1")} Zb:{Commons.ft2mm(offsetZb).ToString("F1")}" );
                                    //
                                    // if (convOffset)
                                    // {
                                    //     offsetZb += Commons.mm2ft(member.offset_Z);
                                    //     offsetZt += Commons.mm2ft(member.offset_Z);
                                    // }

                                    double z1 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, true);
                                    double z2 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, true);

                                    if (Math.Abs(z1 - offsetZb) > 0.001)
                                    {
                                        Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, offsetZb, false);
                                        // Console.WriteLine($"*01: Zt:{Commons.ft2mm(offsetZt).ToString("F1")} Zb:{Commons.ft2mm(offsetZb).ToString("F1")}" );
                                    }
                                    if (Math.Abs(z2 - offsetZt) > 0.001)
                                    {
                                        Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, offsetZt, false);
                                        // Console.WriteLine($"*02: Zt:{Commons.ft2mm(offsetZt).ToString("F1")} Zb:{Commons.ft2mm(offsetZb).ToString("F1")}" );
                                    }
                                }
                                else
                                {
                                    //柱高さはオフセットパラメータで設定しているので高さだけ調節
                                    ElementId eid = instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                                    Level lv1 = Commons.doc.GetElement(eid) as Level;
                                    double offset1 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, true);
                                    double z1 = offset1 + lv1.Elevation;

                                    eid = instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();
                                    Level lv2 = Commons.doc.GetElement(eid) as Level;
                                    double offset2 = Data.GetParameter_double(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, true);
                                    double z2 = offset2 + lv2.Elevation;


                                    double height1 = Commons.mm2ft(height);
                                    double height2 = z2 - z1;
                                    if (Math.Abs(height2 - height1) > 0.001)
                                    {
                                        if (k == 0)
                                        {
                                            //基礎柱：下側を移動
                                            Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, z1 + height2 - height1, false);
                                        }
                                        else
                                        {
                                            //根巻：上側を移動
                                            Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, z2 + height2 - height1, false);
                                        }
                                    }
                                }


                                //回転
                                double angle1 = XYZ.BasisX.AngleOnPlaneTo(instance.HandOrientation, XYZ.BasisZ);
                                if (Math.Abs(angle2 - angle1) > 0.001)
                                {
                                    instance.Location.Rotate(Line.CreateBound(p2, p2 + BasisZ), angle2 - angle1);
                                }


                                SetInstanceParameter_FoundationColumn(member, k, sec, instance);


                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                DebugLogToCommentParam( instance, member.id, logheader, member.name );
                            }

                        }
                        else
                        {
                            //根巻だけ新規
                            //（元が根巻だけ、差分時は基礎柱、根巻の両方になった場合は判別できない）
                            if (!CreateFoundationColumn_instance(stb, member, ConvFamily, k)) { ret = false; errmsg = $"{syubetu}"; }
                        }
                    }

                }
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateGirder(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                Family[][] ConvFamily = GetConvFamily_Girder(syubetu);

                string logheader = "StbGirders";
                StbSecBeam_Kind_beam kind = StbSecBeam_Kind_beam.GIRDER;
                List<StbGirder> girders = null;
                if (syubetu.Contains("小梁"))
                {
                    if (stb.StbModel.StbMembers.StbBeams == null) return true;

                    girders = stb.StbModel.StbMembers.StbBeams.OfType<StbGirder>().ToList();
                    logheader = "StbBeams";
                    kind = StbSecBeam_Kind_beam.BEAM;
                }
                else
                {
                    if (stb.StbModel.StbMembers.StbGirders == null) return true;

                    girders = stb.StbModel.StbMembers.StbGirders.ToList();
                }
                if (girders.Count == 0) return true;


                bool isCanti = syubetu.Contains("片持");

                for (int i = girders.Count - 1; i >= 0; --i)
                {
                    if (girders[i].kind_structure == StbGirderKind_structure.RC)
                    {
                        var sec = stb.StbModel.StbSections.StbSecBeam_RC.Find(a => a.id == girders[i].id_section);
                        if (sec == null || sec.isCanti != isCanti)
                        {
                            girders.RemoveAt(i);
                        }
                    }
                    else if (girders[i].kind_structure == StbGirderKind_structure.S)
                    {
                        var sec = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == girders[i].id_section);
                        if (sec == null || sec.isCanti != isCanti)
                        {
                            girders.RemoveAt(i);
                        }
                    }
                    else if (girders[i].kind_structure == StbGirderKind_structure.SRC)
                    {
                        var sec = stb.StbModel.StbSections.StbSecBeam_SRC.Find(a => a.id == girders[i].id_section);
                        if (sec == null || sec.isCanti != isCanti)
                        {
                            girders.RemoveAt(i);
                        }
                    }
                }

                if (girders.Count == 0) return true;


                Data.ProgressRestart($"{syubetu}の更新", girders.Count);
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

                foreach (var member in girders)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //CheckをOFFにしたものは対象外
                    var t = target.Find(a => a.Id == member.id_section);
                    if (t == null) continue;
                    if (!t.Check) continue;

                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);

                    if (ids == null)
                    {
                        //新規インスタンス
                        if (!CreateGirder_instance(stb, member, ConvFamily, ConvFamily, kind)) { ret = false; errmsg = $"{syubetu}"; }
                    }
                    else
                    {
                        //更新
                        FamilyInstance instance = Commons.doc.GetElement(ids[0]) as FamilyInstance;
                        Level btmLevel = Commons.doc.GetElement(instance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level;


                        //string typename = GetTypeName_Girder(stb, member.id_section);
                        string guid = "";
                        switch (member.kind_structure)
                        {
                            case StbGirderKind_structure.RC:
                                var sec1 = stb.StbModel.StbSections.StbSecBeam_RC.Find(a => a.id == member.id_section);
                                guid = sec1.guid;
                                break;
                            case StbGirderKind_structure.S:
                                var sec2 = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == member.id_section);
                                guid = sec2.guid;
                                break;
                            case StbGirderKind_structure.SRC:
                                var sec3 = stb.StbModel.StbSections.StbSecBeam_SRC.Find(a => a.id == member.id_section);
                                guid = sec3.guid;
                                break;
                        }


                        //配置座標の取得（オフセット・レベルを考慮していない節点の位置）
                        XYZ Ps_org = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_start, 0, 0, 0);
                        XYZ Pe_org = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_end, 0, 0, 0);
                        if (Ps_org.DistanceTo(Pe_org) < Commons.mm2ft(1))
                        {
                            return ret; //falseは変換失敗
                        }
                        XYZ vecU = (Pe_org - Ps_org).Normalize();


                        //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
                        XYZ offsetstart = new XYZ();
                        XYZ offsetend = new XYZ();
                        XYZ offsetstart2 = new XYZ();
                        XYZ offsetend2 = new XYZ();

                        XYZ Ps_xy = new XYZ();
                        XYZ Pe_xy = new XYZ();

                        if (member.offset_start_X != 0 || member.offset_start_Y != 0 || member.offset_end_Z != 0 ||
                            member.offset_end_X != 0 || member.offset_end_Y != 0 || member.offset_end_Z != 0)
                        {
                            offsetstart = Data.TransformCoord(Ps_org, Pe_org, member.offset_start_X, member.offset_start_Y, member.offset_start_Z, -member.rotate);
                            offsetend = Data.TransformCoord(Ps_org, Pe_org, member.offset_end_X, member.offset_end_Y, member.offset_end_Z, -member.rotate);

                            Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(member.offset_start_X), Ps_org.Y + Commons.mm2ft(member.offset_start_Y), Ps_org.Z);
                            Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(member.offset_end_X), Pe_org.Y + Commons.mm2ft(member.offset_end_Y), Pe_org.Z);
                        }
                        else
                        {
                            offsetstart = Search_Offset_gir(stb, member.id_node_start, ref Ps_org, ref Pe_org, "start", vecU, member.id, btmLevel, -member.rotate, out offsetstart2);
                            offsetend = Search_Offset_gir(stb, member.id_node_end, ref Ps_org, ref Pe_org, "end", vecU, member.id, btmLevel, -member.rotate, out offsetend2);

                            Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
                            Pe_xy = Pe_org + Commons.mm2ft(offsetend2);
                        }

                        //梁描画用節点（部材方向のオフセットだけ考慮、それ以外のオフセットはパラメータに入力）
                        XYZ Ps = Ps_org;
                        XYZ Pe = Pe_org;
                        if (convOffset)
                        {
                            Ps = Data.Set_offset(Ps_org, offsetstart, vecU);
                            Pe = Data.Set_offset(Pe_org, offsetend, vecU);
                        }

                        //継手距離は柱中心からの距離なので、梁端部と柱中心の距離ぶん補正する
                        if ( member.joint_start != 0 ) {
                            var jointOffset = Commons.ft2mm( Ps.DistanceTo( Ps_org ) ) ;
                            member.joint_start -= jointOffset ;
                        }
                        if ( member.joint_end != 0 ) {
                            var jointOffset = Commons.ft2mm( Pe.DistanceTo( Pe_org ) ) ;
                            member.joint_end -= jointOffset ;
                        }
                        

                        if (guid != "" && Data.GetStorageGuid(instance.Symbol.Id) != guid)
                        {
                            //タイプが異なる場合は差し替え
                            foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                            {
                                if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                {
                                    if (Data.GetStorageGuid(symbol.Id) == guid)
                                    {
                                        instance.ChangeTypeId(id);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var gs in GirderSymbols)
                            {
                                if (gs.id == member.id_section &&
                                    Math.Abs(member.haunch_start - gs.Length) < 0.001 &&
                                    Math.Abs(member.haunch_end - gs.Length2) < 0.001 &&
                                    member.kind_haunch_start.ToString() == gs.BHaunch1 &&
                                    member.kind_haunch_end.ToString() == gs.BHaunch2)
                                {
                                    if (instance.Symbol.Id != gs.symbol.Id)
                                    {
                                        //ハンチ長が異なるので差し替え
                                        instance.ChangeTypeId(gs.symbol.Id);
                                    }
                                    break;
                                }
                            }
                        }


                        if (instance.Location is LocationCurve locc)
                        {
                            var p0 = locc.Curve.GetEndPoint(0);
                            var p1 = locc.Curve.GetEndPoint(1);

                            AnalyticalMember am = null;
                            if (amanager.HasAssociation(instance.Id))
                            {
                                am = Commons.doc.GetElement(amanager.GetAssociatedElementId(instance.Id)) as AnalyticalMember;
                            }

                            //座標
                            if (p0.DistanceTo(Ps) > 0.001 ||
                                p1.DistanceTo(Pe) > 0.001)
                            {
                                locc.Curve = Line.CreateBound(Ps, Pe);
                                if (am != null)
                                {
                                    am.SetCurve(Line.CreateBound(Ps_org, Pe_org));
                                }
                            }

                            if (convOffset)
                            {
                                Data.SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                                Data.SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                                Data.SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                                Data.SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                                Data.SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                            }

                            //断面回転
                            Data.SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-member.rotate * Math.PI) / 180);

                            //タイプ変更後の再生成
                            Commons.doc.Regenerate();

                            //インスタンスパラメータ更新
                            SetInstanceParameter_Girder(stb, member, kind, "", isCanti, Ps_org, Pe_org, Ps_xy, Pe_xy, instance);


                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                            DebugLogToCommentParam( instance, member.id, logheader, member.name );

                        }

                    }

                }

            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateBrace(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                if (stb.StbModel.StbMembers.StbBraces == null) return true;
                if (stb.StbModel.StbMembers.StbBraces.Count == 0) return true;

                Family[][] ConvFamily = GetConvFamily_Brace();

                const string logheader = "StbBraces";

                Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbBraces.Count);
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

                foreach (var member in stb.StbModel.StbMembers.StbBraces)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //CheckをOFFにしたものは対象外
                    var t = target.Find(a => a.Id == member.id_section);
                    if (t == null) continue;
                    if (!t.Check) continue;

                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);

                    if (ids == null)
                    {
                        //新規インスタンス
                        if (!CreateBrace_instance(stb, member, ConvFamily)) { ret = false; errmsg = $"{syubetu}"; }
                    }
                    else
                    {
                        //更新
                        FamilyInstance instance = Commons.doc.GetElement(ids[0]) as FamilyInstance;
                        Level btmLevel = Commons.doc.GetElement(instance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level;


                        var sec = stb.StbModel.StbSections.StbSecBrace_S.Find(a => a.id == member.id_section);
                        //string typename = GetTypeName_Brace(stb, sec.id);

                        var kind_brace = sec.kind_brace;


                        //配置座標の取得
                        XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_start, 0, 0, 0);
                        XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_end, 0, 0, 0);
                        XYZ Ps_org = Ps;
                        XYZ Pe_org = Pe;
                        if (Ps.DistanceTo(Pe) < Commons.mm2ft(1))
                        {
                            return ret; //falseは変換失敗
                        }

                        XYZ vecU = (Pe - Ps).Normalize();


                        //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
                        XYZ offsetstart = new XYZ();
                        if (member.offset_start_X != 0 || member.offset_start_Y != 0 || member.offset_start_Z != 0)
                        {
                            offsetstart = Data.TransformCoord(Ps, Pe, member.offset_start_X, member.offset_start_Y, member.offset_start_Z, -member.rotate);
                        }
                        else
                        {
                            offsetstart = Search_Offset_bra(stb, member.id_node_start, Ps, Pe, "start", kind_brace, -member.rotate);
                        }

                        XYZ offsetend = new XYZ();
                        if (member.offset_end_X != 0 || member.offset_end_Y != 0 || member.offset_end_Z != 0)
                        {
                            offsetend = Data.TransformCoord(Ps, Pe, member.offset_end_X, member.offset_end_Y, member.offset_end_Z, -member.rotate);
                        }
                        else
                        {
                            offsetend = Search_Offset_bra(stb, member.id_node_end, Ps, Pe, "end", kind_brace, -member.rotate);
                        }


                        if (convOffset)
                        {
                            Ps = Data.Set_offset(Ps, offsetstart, vecU, true);
                            Pe = Data.Set_offset(Pe, offsetend, vecU, true);
                        }


                        if (Data.GetStorageGuid(instance.Symbol.Id) != sec.guid)
                        {
                            //タイプが異なる場合は差し替え
                            foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                            {
                                if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                {
                                    if (Data.GetStorageGuid(symbol.Id) == sec.guid)
                                    {
                                        instance.ChangeTypeId(id);
                                        break;
                                    }
                                }
                            }
                        }


                        if (instance.Location is LocationCurve locc)
                        {
                            var p0 = locc.Curve.GetEndPoint(0);
                            var p1 = locc.Curve.GetEndPoint(1);

                            AnalyticalMember am = null;
                            if (amanager.HasAssociation(instance.Id))
                            {
                                am = Commons.doc.GetElement(amanager.GetAssociatedElementId(instance.Id)) as AnalyticalMember;
                            }

                            //座標
                            if (p0.DistanceTo(Ps) > 0.001 ||
                                p1.DistanceTo(Pe) > 0.001)
                            {
                                locc.Curve = Line.CreateBound(Ps, Pe);
                                if (am != null)
                                {
                                    am.SetCurve(Line.CreateBound(Ps_org, Pe_org));
                                }
                            }

                            if (convOffset)
                            {
                                Data.SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                                Data.SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                                Data.SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                                Data.SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                                Data.SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                            }

                            //断面回転
                            Data.SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-member.rotate * Math.PI) / 180);


                            //インスタンスパラメータ更新
                            SetInstanceParameter_Brace(member, instance);


                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                            DebugLogToCommentParam( instance, member.id, logheader, member.name );
                        }


                    }
                }

            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateFoooting(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                Family[][] ConvFamily = GetConvFamily_Foundation();

                if (stb.StbModel.StbMembers.StbFootings != null && stb.StbModel.StbMembers.StbFootings.Count > 0)
                {
                    const string logheader = "StbFootings";

                    Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbFootings.Count);

                    foreach (var member in stb.StbModel.StbMembers.StbFootings)
                    {
                        Data.ProgressPerformStep();

                        //GUIDのないものは対象外
                        if (member.guid == null || member.guid == "") continue;


                        //CheckをOFFにしたものは対象外
                        var t = target.Find(a => a.Id == member.id_section);
                        if (t == null) continue;
                        if (!t.Check) continue;

                        //Guidに紐づくインスタンスを取得
                        var ids = Data.GetStorageElementId(member.guid);

                        if (ids == null)
                        {
                            //新規インスタンス
                            if (!CreateFoundation_instance(stb, member, ConvFamily)) { ret = false; errmsg = "基礎"; }
                        }
                        else
                        {
                            FamilyInstance instance = Commons.doc.GetElement(ids[0]) as FamilyInstance;

                            //string typename = GetTypeName_Footing(stb, member.id_section);
                            var sec = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == member.id_section);
                            if (Data.GetStorageGuid(instance.Symbol.Id) != sec.guid)
                            {
                                //タイプが異なる場合は差し替え
                                foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                                {
                                    if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                    {
                                        if (Data.GetStorageGuid(symbol.Id) == sec.guid)
                                        {
                                            instance.ChangeTypeId(id);
                                            break;
                                        }
                                    }
                                }
                            }


                            //配置座標の取得
                            XYZ P;
                            if (convOffset)
                            {
                                P = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_X, member.offset_Y, member.level_bottom);
                            }
                            else
                            {
                                P = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, 0, 0, member.level_bottom);
                            }

                            if (instance.Location is LocationPoint locp)
                            {
                                var p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                var p2 = new XYZ(P.X, P.Y, 0);
                                if (p1.DistanceTo(p2) > 0.001)
                                {
                                    instance.Location.Move(p2 - p1);
                                }

                                //レベルからの高さに換算
                                var para = instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
                                Level level = Commons.doc.GetElement(para.AsElementId()) as Level;
                                double nodeZ = stb.StbModel.StbNodes.First( x => x.id == member.id_node ).Z ;
                                double level_bottom = Commons.mm2ft(member.level_bottom + nodeZ) - level.Elevation;

                                //レベルからの高さオフセット
                                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), level_bottom, false);

                                //回転
                                double angle1 = XYZ.BasisX.AngleOnPlaneTo(instance.HandOrientation, XYZ.BasisZ);
                                double angle2 = member.rotate * Math.PI / 180;
                                if (Math.Abs(angle2 - angle1) > 0.001)
                                {
                                    instance.Location.Rotate(Line.CreateBound(p2, p2 + BasisZ), angle2 - angle1);
                                }


                                SetInstanceParameter_Footing(stb, member, instance);

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                DebugLogToCommentParam( instance, member.id, logheader, member.name );
                            }

                        }
                    }

                }

                if (stb.StbModel.StbMembers.StbStripFootings != null && stb.StbModel.StbMembers.StbStripFootings.Count > 0)
                {
                    const string logheader = "StbStripFootings";

                    Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbStripFootings.Count);

                    foreach (var member in stb.StbModel.StbMembers.StbStripFootings)
                    {
                        Data.ProgressPerformStep();

                        //GUIDのないものは対象外
                        if (member.guid == null || member.guid == "") continue;


                        //CheckをOFFにしたものは対象外
                        var t = target.Find(a => a.Id == member.id_section);
                        if (t == null) continue;
                        if (!t.Check) continue;

                        //Guidに紐づくインスタンスを取得
                        var ids = Data.GetStorageElementId(member.guid);

                        if (ids == null)
                        {
                            //新規インスタンス
                            if (!CreateStripFooting_instance(stb, member, ConvFamily)) { ret = false; errmsg = "布基礎"; }
                        }
                        else
                        {
                            FamilyInstance instance = Commons.doc.GetElement(ids[0]) as FamilyInstance;

                            //string typename = GetTypeName_Footing(stb, member.id_section);
                            var sec = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == member.id_section);
                            if (Data.GetStorageGuid(instance.Symbol.Id) != sec.guid)
                            {
                                //タイプが異なる場合は差し替え
                                foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                                {
                                    if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                    {
                                        if (Data.GetStorageGuid(symbol.Id) == sec.guid)
                                        {
                                            instance.ChangeTypeId(id);
                                            break;
                                        }
                                    }
                                }
                            }


                            //配置座標の取得
                            XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_start, 0, 0, 0);
                            XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, member.id_node_end, 0, 0, 0);

                            if (convOffset)
                            {
                                //オフセット
                                XYZ vec1 = (Pe - Ps).Normalize();
                                XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();
                                double offset = Commons.mm2ft(member.offset);
                                Ps += vec2 * offset;
                                Pe += vec2 * offset;
                            }

                            if (instance.Location is LocationCurve locc)
                            {
                                var p0 = locc.Curve.GetEndPoint(0);
                                var p1 = locc.Curve.GetEndPoint(1);

                                //配置平面上にするためZを差し替え
                                Ps = new XYZ(Ps.X, Ps.Y, p0.Z);
                                Pe = new XYZ(Pe.X, Pe.Y, p0.Z);

                                //座標
                                if (p0.DistanceTo(Ps) > 0.001 ||
                                    p1.DistanceTo(Pe) > 0.001)
                                {
                                    locc.Curve = Line.CreateBound(Ps, Pe);
                                }

                                //STBのレベルは節点から布基礎下端までの長さ。インスタンスに設定するのは配置面からのオフセット
                                double f_level = Ps.Z + Commons.mm2ft(member.level) - p0.Z;

                                Data.SetParameter(instance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM), f_level, false); //レベルからの高さオフセット


                                SetInstanceParameter_StripFooting(member, instance);

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                DebugLogToCommentParam( instance, member.id, logheader, member.name );
                            }

                        }
                    }
                }

                if (stb.StbModel.StbMembers.StbPiles != null && stb.StbModel.StbMembers.StbPiles.Count > 0)
                {
                    const string logheader = "StbPiles";

                    Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbPiles.Count);

                    foreach (var member in stb.StbModel.StbMembers.StbPiles)
                    {
                        Data.ProgressPerformStep();

                        //GUIDのないものは対象外
                        if (member.guid == null || member.guid == "") continue;


                        //CheckをOFFにしたものは対象外
                        var t = target.Find(a => a.Id == member.id_section);
                        if (t == null) continue;
                        if (!t.Check) continue;

                        //Guidに紐づくインスタンスを取得
                        var ids = Data.GetStorageElementId(member.guid);

                        if (ids == null)
                        {
                            //新規インスタンス
                            if (!CreatePile_instance(stb, member, ConvFamily)) { ret = false; errmsg = "杭"; }
                        }
                        else
                        {
                            GetPileData(stb, member, ConvFamily, out _, out string typename, out double length_all, out int index);

                            FamilyInstance instance = Commons.doc.GetElement(ids[0]) as FamilyInstance;
                            Level btmlevel = Commons.doc.GetElement(instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId()) as Level;
                            if (btmlevel == null)
                            {
                                btmlevel = Commons.doc.GetElement(instance.LevelId) as Level;
                            }


                            //タイプが異なる場合は差し替え（杭は長さ違いがあるのでチェックする）
                            FamilyStructure.Pile Rpile = SetFamily.CastinPile;
                            foreach (var id in instance.Symbol.Family.GetFamilySymbolIds())
                            {
                                if (Commons.doc.GetElement(id) is FamilySymbol symbol)
                                {
                                    string fugo = Data.GetParameter_string(symbol, Rpile.name);
                                    if (typename != fugo) continue;

                                    double len0 = Data.GetParameter_double(symbol, Rpile.length_all, false);
                                    double len1 = Data.GetParameter_double(symbol, Rpile.length_head, false);
                                    double len2 = Data.GetParameter_double(symbol, Rpile.length_foot, false);

                                    if (Data.GetParameter_int(symbol, "拡頭") == 0) len1 = 0;
                                    if (Data.GetParameter_int(symbol, "拡底") == 0) len2 = 0;

                                    if (Math.Abs(len0 - member.length_all) < 1 &&
                                        Math.Abs(len1 - member.length_head) < 1 &&
                                        Math.Abs(len2 - member.length_foot) < 1)
                                    {
                                        if (instance.Symbol.Id != id)
                                        {
                                            instance.ChangeTypeId(id);
                                        }
                                        break;
                                    }
                                }
                            }


                            //配置座標の取得
                            //杭はオフセットを加えないと全て同じところにできてしまうので、断面＋位置でもオフセット見る
                            XYZ P = Get_Node_Position(stb.StbModel.StbNodes, member.id_node, member.offset_X, member.offset_Y, 0);

                            if (instance.Location is LocationPoint locp)
                            {
                                var p1 = new XYZ(locp.Point.X, locp.Point.Y, 0);
                                var p2 = new XYZ(P.X, P.Y, 0);
                                if (p1.DistanceTo(p2) > 0.001)
                                {
                                    instance.Location.Move(p2 - p1);
                                }

                                //レベルからの相対高さを設定する
                                double level_top = P.Z + Commons.mm2ft(member.level_top) - btmlevel.Elevation;

                                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), level_top, false); //レベルからの高さオフセット


                                SetInstanceParameter_Pile(member, length_all, index, instance);

                                LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                                DebugLogToCommentParam( instance, member.id, logheader, member.name );
                            }

                        }
                    }
                }

            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateWall(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                if (stb.StbModel.StbMembers.StbWalls == null) return true;
                if (stb.StbModel.StbMembers.StbWalls.Count == 0) return true;


                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                List<WallType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<WallType>().Where(a => a.Kind == WallKind.Basic).ToList();


                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_SWallRectOpening);
                List<Opening> opens = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList();



                const string logheader = "StbWalls";

                Data.ProgressRestart($"{syubetu}の更新", stb.StbModel.StbMembers.StbWalls.Count);
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

                foreach (var member in stb.StbModel.StbMembers.StbWalls)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //CheckをOFFにしたものは対象外
                    var t = target.Find(a => a.Id == member.id_section);
                    if (t == null) continue;
                    if (!t.Check) continue;

                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);

                    if (ids == null)
                    {
                        //新規インスタンス
                        var symbol = symbols.Find(a => a.Name == member.name);
                        if (!CreateWall_instance(stb, member, symbol, ref errmsg)) { ret = false; errmsg = $"{syubetu}"; }
                    }
                    else
                    {
                        //更新
                        Wall instance = Commons.doc.GetElement(ids[0]) as Wall;
                        Level btmLevel = Commons.doc.GetElement(instance.LevelId) as Level;

                        //string typename = GetTypeName_Wall(stb, member.id_section);
                        var sec = stb.StbModel.StbSections.StbSecWall_RC.Find(a => a.id == member.id_section);
                        if (Data.GetStorageGuid(instance.WallType.Id) != sec.guid)
                        {
                            //タイプが異なる場合は差し替え
                            foreach (var id in instance.GetValidTypes())
                            {
                                if (Commons.doc.GetElement(id) is WallType symbol)
                                {
                                    if (Data.GetStorageGuid(symbol.Id) == sec.guid)
                                    {
                                        instance.ChangeTypeId(id);
                                        break;
                                    }
                                }
                            }
                        }


                        var nodes = member.StbNodeIdOrderList.Distinct().ToList();

                        //節点から配置位置を取得
                        List<XYZ> Point0 = new List<XYZ>();
                        List<XYZ> Point1 = new List<XYZ>();
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            XYZ Pa = Get_Node_Position(stb.StbModel.StbNodes, nodes[i], 0, 0, 0);
                            XYZ Pb = Pa;
                            if (convOffset && member.StbWallOffsetList != null)
                            {
                                var offset = member.StbWallOffsetList.Find(a => a.id_node == nodes[i]);
                                if (offset != null)
                                {
                                    Pa = Get_Node_Position(stb.StbModel.StbNodes, nodes[i], offset.offset_X, offset.offset_Y, offset.offset_Z);
                                }
                            }

                            if (Point0.Count > 0)
                            {
                                if (Pa.DistanceTo(Point0.Last()) < gosa)
                                {
                                    continue;
                                }
                            }
                            Point0.Add(Pa);
                            Point1.Add(Pb);
                        }

                        if (!CheckWall(member, Point0))
                        {
                            continue;
                        }


                        if (instance.Location is LocationCurve locc)
                        {
                            var p0 = locc.Curve.GetEndPoint(0);
                            var p1 = locc.Curve.GetEndPoint(1);

                            AnalyticalPanel ap = null;
                            if (amanager.HasAssociation(instance.Id))
                            {
                                ap = Commons.doc.GetElement(amanager.GetAssociatedElementId(instance.Id)) as AnalyticalPanel;
                            }

                            XYZ Ps = new XYZ(Point0[0].X, Point0[0].Y, p0.Z);
                            XYZ Pe = new XYZ(Point0[1].X, Point0[1].Y, p0.Z);
                            if (Ps.DistanceTo(p0) > Ps.DistanceTo(p1))
                            {
                                //locationcurveが逆になっていることがある
                                Pe = new XYZ(Point0[0].X, Point0[0].Y, p0.Z);
                                Ps = new XYZ(Point0[1].X, Point0[1].Y, p0.Z);
                            }

                            //座標
                            if (p0.DistanceTo(Ps) > 0.001 ||
                                p1.DistanceTo(Pe) > 0.001)
                            {
                                locc.Curve = Line.CreateBound(Ps, Pe);
                                if (ap != null)
                                {
                                    List<Curve> lines = new List<Curve>();
                                    Point1.Add(Point1[0]);
                                    for (int i = 0; i < Point1.Count - 1; ++i)
                                    {
                                        if (Point1[i].DistanceTo(Point1[i + 1]) <= Commons.doc.Application.ShortCurveTolerance)
                                        {
                                            continue;
                                        }

                                        lines.Add(Line.CreateBound(Point1[i], Point1[i + 1]));
                                    }
                                    CurveLoop curves = CurveLoop.Create(lines);
                                    ap.SetOuterContour(curves);
                                }
                            }

                            var base_offset1 = Point0[0].Z - btmLevel.Elevation;
                            var base_offset2 = Data.GetParameter_double(instance, BuiltInParameter.WALL_BASE_OFFSET, true);
                            if (Math.Abs(base_offset1 - base_offset2) > 0.001)
                            {
                                Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), base_offset1, false);
                            }

                            Parameter param = instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
                            ElementId topLV = param.AsElementId();
                            if (topLV.Value() == -1)
                            {
                                //高さ指定
                                var height1 = Point0.Last().Z - Point0[0].Z;
                                var height2 = Data.GetParameter_double(instance, BuiltInParameter.WALL_USER_HEIGHT_PARAM, true);
                                if (Math.Abs(height1 - height2) > 0.001)
                                {
                                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM), height1, false);
                                }
                            }
                            else
                            {
                                Level topLevel = Commons.doc.GetElement(topLV) as Level;
                                var top_offset1 = Point0.Last().Z - topLevel.Elevation;
                                var top_offset2 = Data.GetParameter_double(instance, BuiltInParameter.WALL_TOP_OFFSET, true);
                                if (Math.Abs(top_offset1 - top_offset2) > 0.001)
                                {
                                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET), top_offset1, false);
                                }
                            }


                            //インスタンスパラメータ更新
                            SetInstanceParameter_Wall(stb, member, instance);

                            LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                            DebugLogToCommentParam( instance, member.id, logheader, instance.WallType.Name );


                            //開口：パラメータを持てないので、残したい情報はないはず。一度削除して作り直す
                            var opens2 = opens.Where(a => a.Host.Id == instance.Id).Select(a => a.Id).ToList();
                            if (opens2.Count > 0)
                            {
                                opens.RemoveAll(a => opens2.Contains(a.Id));
                                Commons.doc.Delete(opens2);
                            }
                            if (member.StbOpenIdList != null && member.StbOpenIdList.Count > 0)
                            {
                                //開口を作る前に、一度Regenerateしないとエラーが出る
                                Commons.doc.Regenerate();
                                errmsg = "開口";

                                XYZ v1 = (Point0[1] - Point0[0]).Normalize();
                                XYZ v2 = (Point0[Point0.Count - 1] - Point0[0]).Normalize();
                                XYZ normal = (v2.CrossProduct(v1)).Normalize();

                                Wall_Open(stb, member, Point0[0], v1, normal, instance);
                            }
                        }
                    }

                }

            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        private static bool UpdateSlab(ST_BRIDGE stb, string syubetu, List<ConvertCheck> target, out string errmsg, bool convOffset)
        {
            errmsg = "";
            if (target == null) return true;
            if (target.Count == 0) return true;

            if (!target.Any(a => a.Check)) return true;


            bool ret = true;

            try
            {
                if (stb.StbModel.StbMembers.StbSlabs == null) return true;
                if (stb.StbModel.StbMembers.StbSlabs.Count == 0) return true;

                List<int> id_section = new List<int>();
                switch (syubetu)
                {
                    case "RCスラブ":
                        id_section = stb.StbModel.StbSections.StbSecSlab_RC.Where(a => !a.isFoundation).Select(a => a.id).ToList();
                        break;
                    case "基礎スラブ":
                        id_section = stb.StbModel.StbSections.StbSecSlab_RC.Where(a => a.isFoundation).Select(a => a.id).ToList();
                        break;
                    case "デッキプレート":
                        id_section = stb.StbModel.StbSections.StbSecSlabDeck.Select(a => a.id).ToList();
                        break;
                    case "既製スラブ":
                        id_section = stb.StbModel.StbSections.StbSecSlabPrecast.Select(a => a.id).ToList();
                        break;
                }

                var slabs = stb.StbModel.StbMembers.StbSlabs.Where(a => id_section.Contains(a.id_section)).ToList();
                if (slabs.Count == 0) return true;

                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                List<FloorType> symbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FloorType>().ToList();

                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_FloorOpening);
                List<Opening> opens = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Opening>().ToList();


                const string logheader = "StbSlabs";

                Data.ProgressRestart($"{syubetu}の更新", slabs.Count);
                var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);

                foreach (var member in slabs)
                {
                    Data.ProgressPerformStep();

                    //GUIDのないものは対象外
                    if (member.guid == null || member.guid == "") continue;


                    //CheckをOFFにしたものは対象外
                    var t = target.Find(a => a.Id == member.id_section);
                    if (t == null) continue;
                    if (!t.Check) continue;


                    //Guidに紐づくインスタンスを取得
                    var ids = Data.GetStorageElementId(member.guid);

                    if (ids == null)
                    {
                        //新規インスタンス
                        var symbol = symbols.Find(a => a.Name == member.name);
                        if (!CreateSlab_instance(stb, member, symbol, syubetu, ref errmsg, syubetu == "基礎スラブ")) { ret = false; errmsg = $"{syubetu}"; }
                    }
                    else
                    {
                        //更新
                        Floor instance = Commons.doc.GetElement(ids[0]) as Floor;

                        //string typename = GetTypeName_Slab(stb, member.id_section);
                        string guid = "";
                        switch (member.kind_structure)
                        {
                            case StbSlabKind_structure.RC:
                                var sec1 = stb.StbModel.StbSections.StbSecSlab_RC.Find(a => a.id == member.id_section);
                                guid = sec1.guid;
                                break;
                            case StbSlabKind_structure.DECK:
                                var sec2 = stb.StbModel.StbSections.StbSecSlabDeck.Find(a => a.id == member.id_section);
                                guid = sec2.guid;
                                break;
                            case StbSlabKind_structure.PRECAST:
                                var sec3 = stb.StbModel.StbSections.StbSecSlabPrecast.Find(a => a.id == member.id_section);
                                guid = sec3.guid;
                                break;
                        }

                        if (guid != "" && Data.GetStorageGuid(instance.FloorType.Id) != guid)
                        {
                            //タイプが異なる場合は差し替え
                            foreach (var id in instance.GetValidTypes())
                            {
                                if (Commons.doc.GetElement(id) is FloorType symbol)
                                {
                                    if (Data.GetStorageGuid(symbol.Id) == guid)
                                    {
                                        instance.ChangeTypeId(id);
                                        break;
                                    }
                                }
                            }
                        }


                        if (!GetSlabCoords(stb, member, syubetu, out _, out List<XYZ> Point0, out _, out XYZ vec1, out XYZ normal, convOffset))
                        {
                            continue;
                        }

                        //解析線分用にオフセットのない座標取得
                        GetSlabCoords(stb, member, syubetu, out _, out List<XYZ> Point1, out _, out _, out _, false);
                        Point1.Add(Point1[0]);

                        // SubTransaction st = new SubTransaction(Commons.doc);
                        // st.Start();
                        // var partsId = Commons.doc.Delete(instance.Id);
                        // st.RollBack();

                        List<ModelLine> mLine = new List<ModelLine>();
                        ModelLine slope = null;
                        
                        if (Commons.doc.GetElement(instance.SketchId) is Sketch sketch)
                        {
                            var partsId = sketch.GetAllElements();
                            foreach (var id in partsId)
                            {
                                var elm = Commons.doc.GetElement(id);
                                if (elm is ModelLine m)
                                {
                                    if (m.get_Parameter(BuiltInParameter.SLOPE_ARROW_LEVEL_START) != null)
                                    {
                                        //「矢尻 レベル」パラメータがあれば勾配矢印とする
                                        slope = m;
                                    }
                                    else if (m.get_Parameter(BuiltInParameter.CURVE_LEVEL) == null)
                                    {
                                        //「レベル」パラメータがない→スパン方向エッジ
                                        continue;
                                    }
                                    else
                                    {
                                        mLine.Add(m);
                                    }
                                }
                            }
                        }

                        /*
                        foreach (var id in partsId)
                        {
                            var elm = Commons.doc.GetElement(id);
                            if (elm is ModelLine m)
                            {
                                if (m.get_Parameter(BuiltInParameter.SLOPE_ARROW_LEVEL_START) != null)
                                {
                                    //「矢尻 レベル」パラメータがあれば勾配矢印とする
                                    slope = m;
                                }
                                else if (m.get_Parameter(BuiltInParameter.CURVE_LEVEL) == null)
                                {
                                    //「レベル」パラメータがない→スパン方向エッジ
                                    continue;
                                }
                                else
                                {
                                    mLine.Add(m);
                                }
                            }
                        }
                        //*/

                        if (mLine.Count < Point0.Count)
                        {
                            LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()} 頂点数の増えた床は更新できません");
                            continue;
                        }
                        else if (mLine.Count < Point0.Count)
                        {
                            //辺の数が減った分削除する
                            int n = Point0.Count - mLine.Count;
                            for (int i = 0; i < n; ++i)
                            {
                                mLine.RemoveAt(mLine.Count - 1);
                            }
                        }


                        double zz = Point0[0].Z;
                        for (int i = 0; i < Point0.Count; ++i)
                        {
                            int ii = i + 1;
                            if (ii >= Point0.Count) ii = 0;

                            var locc = mLine[i].Location as LocationCurve;
                            XYZ p0;
                            XYZ p1;
                            if (convOffset)
                            {
                                p0 = new XYZ(Point0[i].X, Point0[i].Y, zz);
                                p1 = new XYZ(Point0[ii].X, Point0[ii].Y, zz);
                            }
                            else
                            {
                                XYZ pp = locc.Curve.GetEndPoint(0);
                                p0 = new XYZ(Point0[i].X, Point0[i].Y, pp.Z);
                                p1 = new XYZ(Point0[ii].X, Point0[ii].Y, pp.Z);
                            }

                            locc.Curve = Line.CreateBound(p0, p1);
                        }

                        bool keisyaflg = false;
                        if (normal.CrossProduct(XYZ.BasisZ).GetLength() < 0.001)
                        {
                            //傾斜なし
                            if (slope != null)
                            {
                                //傾斜を0°にする
                                Data.SetParameter(instance.get_Parameter(BuiltInParameter.ROOF_SLOPE), 0);
                            }
                        }
                        else
                        {
                            //傾斜
                            if (slope != null)
                            {
                                //傾斜軸
                                XYZ v1 = XYZ.BasisZ.CrossProduct(normal).Normalize();
                                //傾斜方向
                                XYZ v2 = normal.CrossProduct(v1).Normalize();
                                //傾斜方向からZ成分を取り除いたもの
                                XYZ v3 = new XYZ(v2.X, v2.Y, 0).Normalize();

                                double tan = Math.Tan(v2.AngleTo(v3));
                                if (v2.Z < 0)
                                {
                                    //下がる床なら角度反転
                                    tan = -tan;
                                }

                                var locc = slope.Location as LocationCurve;
                                XYZ p0 = locc.Curve.GetEndPoint(0);
                                XYZ p1 = p0 + v3 * locc.Curve.Length;
                                locc.Curve = Line.CreateBound(p0, p1);

                                //勾配：tangentをセットする
                                Data.SetParameter(slope.get_Parameter(BuiltInParameter.ROOF_SLOPE), tan);

                                keisyaflg = true;
                            }
                            else
                            {
                                //傾斜なし→あり は変更できない。スケッチに勾配矢印を追加できない。
                                LogData.AddLog(LogData.LogKind.Warning, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()} 勾配を追加することはできません");
                            }
                        }

                        if (convOffset)
                        {
                            Level btmLevel = Commons.doc.GetElement(instance.LevelId) as Level;
                            Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), zz - btmLevel.Elevation);
                        }

                        AnalyticalPanel ap = null;
                        if (amanager.HasAssociation(instance.Id))
                        {
                            ap = Commons.doc.GetElement(amanager.GetAssociatedElementId(instance.Id)) as AnalyticalPanel;
                            if (ap != null)
                            {
                                List<Curve> lines = new List<Curve>();
                                for (int i = 0; i < Point1.Count - 1; ++i)
                                {
                                    if (Point1[i].DistanceTo(Point1[i + 1]) <= Commons.doc.Application.ShortCurveTolerance)
                                    {
                                        continue;
                                    }

                                    lines.Add(Line.CreateBound(Point1[i], Point1[i + 1]));
                                }
                                CurveLoop curves = CurveLoop.Create(lines);
                                ap.SetOuterContour(curves);
                            }
                        }


                        SetInstanceParameter_Slab(stb, member, instance);


                        LogData.AddLog(LogData.LogKind.Infmoation, 0, $"{logheader}(id={member.id}) Revit-id={instance.Id.Value()}を更新しました");
                        DebugLogToCommentParam( instance, member.id, logheader, member.name );


                        //開口：パラメータを持てないので、残したい情報はないはず。一度削除して作り直す
                        var opens2 = opens.Where(a => a.Host.Id == instance.Id).Select(a => a.Id).ToList();
                        if (opens2.Count > 0)
                        {
                            opens.RemoveAll(a => opens2.Contains(a.Id));
                            Commons.doc.Delete(opens2);
                        }
                        if (member.StbOpenIdList != null && member.StbOpenIdList.Count > 0)
                        {
                            //開口を作る前に、一度Regenerateしないとエラーが出る
                            Commons.doc.Regenerate();
                            errmsg = "開口";

                            Slab_Open(stb, member, Point0[0], vec1, normal, instance, keisyaflg);
                        }

                    }

                }
            }
            catch
            {
                errmsg = syubetu;
                ret = false;
            }

            Data.ProgressClose();

            return ret;
        }

        
        internal static void DebugLogToCommentParam( Object obj, int id, string logname, string typename )
        {
            if ( ! ShouldOutputCommentDebugLog ) return ;
            
            if ( obj is FamilyInstance ) {
                var instance = obj as FamilyInstance ;
                //デバッグ用
                var commentParam = instance.LookupParameter( "コメント" ) ;
                var val = commentParam.AsValueString() ;
                commentParam?.Set( $"{val} + [{id}:{logname} {typename}]" ) ;
                return;
            }

            if ( obj is Wall ) {
                var instance = obj as Wall ;
                var commentParam = instance.LookupParameter( "コメント" ) ;
                commentParam?.Set( $"[{id}:{logname} {typename}]" ) ;
                return;
            }
            if ( obj is Floor ) {
                var instance = obj as Wall ;
                var commentParam = instance.LookupParameter( "コメント" ) ;
                commentParam?.Set( $"[{id}:{logname} {typename}]" ) ;
                return;
            }
            return;

        }
        
    }
}
