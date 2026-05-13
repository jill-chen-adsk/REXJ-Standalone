using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Structure;
using ST_BRIDGE_V2;

namespace STBLink
{
    partial class FromSTB_v2
    {
        //private変数はファイル内で完結しているのでそれぞれで持つ

        private const double gosa = 0.001;

        private static readonly XYZ BasisX = new XYZ(1, 0, 0);
        private static readonly XYZ BasisY = new XYZ(0, 1, 0);
        private static readonly XYZ BasisZ = new XYZ(0, 0, 1);

        private static double alloffsetX = 0;
        private static double alloffsetY = 0;
        private static List<Level> Levels = new List<Level>();
        private static List<Data.OffsetZ> alloffsetZ = new List<Data.OffsetZ>();

        private static List<Data.BaseClass> BClm = new List<Data.BaseClass>();

        private static List<Data.ReNameSymbols> GirderSymbols = new List<Data.ReNameSymbols>();
        private static List<Data.ReNameSymbols> FContiSymbols = new List<Data.ReNameSymbols>();
        private static List<Data.ReNameSymbols> PilesSymbols = new List<Data.ReNameSymbols>();

        private static List<Data.IsOutin_Girder> isOutin_G = new List<Data.IsOutin_Girder>();

        //基礎・杭グループ化
        private static Dictionary<int, List<ElementId>> FGroup = new Dictionary<int, List<ElementId>>();
        //基礎グループ化
        private static List<Data.CGroup> CGrp = new List<Data.CGroup>();


        private static Dictionary<int, ElementId>[] columnType = new Dictionary<int, ElementId>[4];
        private static Dictionary<int, ElementId>[] braceType = new Dictionary<int, ElementId>[1];
        private static Dictionary<int, ElementId>[] footingType = new Dictionary<int, ElementId>[1];


        public static bool ShouldOutputCommentDebugLog = false ; 

        internal static void Initialize()
        {
            alloffsetX = 0;
            alloffsetY = 0;
            Levels = new List<Level>();
            alloffsetZ = new List<Data.OffsetZ>();
            BClm = new List<Data.BaseClass>();
            GirderSymbols = new List<Data.ReNameSymbols>();
            FContiSymbols = new List<Data.ReNameSymbols>();
            PilesSymbols = new List<Data.ReNameSymbols>();
            isOutin_G = new List<Data.IsOutin_Girder>();
            FGroup = new Dictionary<int, List<ElementId>>();
            CGrp = new List<Data.CGroup>();

            columnType = new Dictionary<int, ElementId>[4];
            columnType[0] = new Dictionary<int, ElementId>(); //RC
            columnType[1] = new Dictionary<int, ElementId>(); //S
            columnType[2] = new Dictionary<int, ElementId>(); //SRC
            columnType[3] = new Dictionary<int, ElementId>(); //CFT

            braceType = new Dictionary<int, ElementId>[1];
            braceType[0] = new Dictionary<int, ElementId>();

            footingType = new Dictionary<int, ElementId>[1];
            footingType[0] = new Dictionary<int, ElementId>();
        }
        internal static void SetAllOffset()
        {
            //移動量をプロジェクト情報に保持するため、先に計算しておく方法に変更したので、どちらの指定方法でも値を読むだけ

            //if (ConvertForm.LMD.rdb == 1)
            //{
            //    //マッピング指定
            //    FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            //    ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            //    List<Grid> grids = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<Grid>().ToList();

            //    var gx = grids.Find(a => a.Name == ConvertForm.LMD.RVT_X);
            //    if (gx != null)
            //    {
            //        alloffsetX = Commons.ft2mm(gx.Curve.GetEndPoint(0).X) + ConvertForm.LMD.Offset_X1;
            //    }

            //    var gy = grids.Find(a => a.Name == ConvertForm.LMD.RVT_Y);
            //    if (gy != null)
            //    {
            //        alloffsetY = Commons.ft2mm(gy.Curve.GetEndPoint(0).Y) + ConvertForm.LMD.Offset_Y1;
            //    }
            //}
            //else
            {
                //オフセット指定
                alloffsetX = ConvertForm.LMD.Offset_X2;
                alloffsetY = ConvertForm.LMD.Offset_Y2;
            }
        }


        #region データチェック

        /// <summary>
        /// STBのコンクリート情報
        /// </summary>
        /// <param name="stb2"></param>
        /// <param name="floor"></param>
        /// <param name="kouzou"></param>
        /// <param name="conc"></param>
        /// <returns></returns>
        private static string ConcData_Add(ST_BRIDGE stb2, string floor, string kouzou, ref string conc)
        {
            if (conc == null)
            {
                conc = "";
            }

            if (conc == "") //部材ごとのコンクリート強度が空欄の時
            {
                //所属階のコンクリート強度を求める
                if (floor != "")
                {
                    conc = stb2.StbModel.StbStories.Find(a => a.name == floor)?.strength_concrete ?? "";
                }

                //所属階のコンクリート強度が空欄の時
                if (conc == "")
                {
                    //共通情報の建物全体のコンクリート強度を求める
                    conc = stb2.StbCommon.strength_concrete;
                }
            }

            if (conc == "") return conc; //階情報にも共通情報にもコンクリート強度が無い

            string conc2 = conc;
            if (!RevitLNK.ConcData.Any(a => a.kouzou == kouzou && a.STBstrength == conc2))
            {
                RevitLNK.Concredata cd = new RevitLNK.Concredata
                {
                    kouzou = kouzou,
                    STBstrength = conc,
                };
                RevitLNK.ConcData.Add(cd);
            }

            return conc;
        }




        internal static void CheckSTB_Column(ST_BRIDGE stb2, ConvertForm f)
        {
            if (stb2.StbModel.StbSections.StbSecColumn_RC != null)
            {
                foreach (var clm in stb2.StbModel.StbSections.StbSecColumn_RC)
                {
                    try
                    {
                        ConvertForm.STBLoadflg nf = new ConvertForm.STBLoadflg()
                        {
                            kind = clm.kind_column.ToString(),
                            flg = true,
                        };

                        if (nf.kind == "")
                        {
                            nf.kind = StbSecColumn_Kind_column.COLUMN.ToString();
                        }

                        if (clm.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect)
                        {
                            nf.name = RevitLNK.ClmText[0][0];
                        }
                        else
                        {
                            nf.name = RevitLNK.ClmText[0][1];
                        }

                        f.STBload_Add(nf);

                        string conc = clm.strength_concrete;
                        clm.strength_concrete = ConcData_Add(stb2, clm.floor, "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC柱(StbSecColumn_RC)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecColumn_S != null)
            {
                foreach (var clm in stb2.StbModel.StbSections.StbSecColumn_S)
                {
                    try
                    {
                        foreach (var fig in clm.StbSecSteelFigureColumn_S.Items)
                        {
                            string shape = "";

                            if (fig is StbSecSteelColumn_S_NotSame notSame)
                            {
                                Data.MateData_Add(notSame.strength_main);
                                Data.MateData_Add(notSame.strength_web);
                                shape = notSame.shape;
                            }
                            else if (fig is StbSecSteelColumn_S_Same same)
                            {
                                Data.MateData_Add(same.strength_main);
                                Data.MateData_Add(same.strength_web);
                                shape = same.shape;
                            }
                            else if (fig is StbSecSteelColumn_S_ThreeTypes threeTypes)
                            {
                                Data.MateData_Add(threeTypes.strength_main);
                                Data.MateData_Add(threeTypes.strength_web);
                                shape = threeTypes.shape;
                            }

                            string shapetype = Check_Steel(stb2, shape, out int ind);

                            ConvertForm.STBLoadflg nf = new ConvertForm.STBLoadflg
                            {
                                kind = clm.kind_column.ToString(),
                                flg = true,
                            };

                            if (nf.kind == "")
                            {
                                nf.kind = StbSecColumn_Kind_column.COLUMN.ToString();
                            }

                            if (shapetype == RevitLNK.st_steel_H)
                            {
                                nf.name = RevitLNK.ClmText[1][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_BH)
                            {
                                nf.name = RevitLNK.ClmText[1][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_Box)
                            {
                                nf.name = RevitLNK.ClmText[1][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.ClmText[1][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.ClmText[1][4];
                            }
                            else if (shapetype == RevitLNK.st_steel_T)
                            {
                                nf.name = RevitLNK.ClmText[1][5];
                            }
                            else if (shapetype == RevitLNK.st_steel_C)
                            {
                                nf.name = RevitLNK.ClmText[1][6];
                            }
                            else if (shapetype == RevitLNK.st_steel_L)
                            {
                                nf.name = RevitLNK.ClmText[1][7];
                            }
                            else if (shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S柱(StbSecColumn_S)");
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + clm.name + "(断面id=" + clm.id.ToString() + ") shape=\"" + shape + "\" ");
                            }

                            f.STBload_Add(nf);
                        }

                        if (clm.StbSecSteelFigureColumn_S.base_type == StbSecSteelFigureColumn_SBase_type.EXPOSE)
                        {
                            if (clm.Item != null && clm.Item is StbSecBaseProduct_S baseProduct)
                            {
                                var lf = new ConvertForm.STBLoadflg
                                {
                                    kind = "柱脚",
                                    flg = true,
                                    name = "柱脚"
                                };
                                f.STBload_Add(lf);

                                var bc = new ConvertForm.BClmData
                                {
                                    company = baseProduct.product_company,
                                    product_code = baseProduct.product_code
                                };
                                f.BClm_Add(bc);

                                for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                {
                                    if (RevitLNK.BClm[b].product_company == baseProduct.product_company &&
                                        RevitLNK.BClm[b].product_code == baseProduct.product_code)
                                    {
                                        RevitLNK.BClm[b].flg = true;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S柱(StbSecColumn_S)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecColumn_SRC != null)
            {
                foreach (var clm in stb2.StbModel.StbSections.StbSecColumn_SRC)
                {
                    try
                    {
                        foreach (var fig in clm.StbSecSteelFigureColumn_SRC.Items)
                        {
                            var nf = new ConvertForm.STBLoadflg
                            {
                                kind = clm.kind_column.ToString(),
                                flg = true,
                            };

                            if (nf.kind == "")
                            {
                                nf.kind = StbSecColumn_Kind_column.COLUMN.ToString();
                            }

                            if (clm.StbSecSteelFigureColumn_SRC == null) { continue; }

                            string shape = "";
                            List<string> strength = new List<string>();

                            if (fig is StbSecSteelColumn_SRC_NotSame notSame)
                            {
                                if (notSame.Item is StbSecColumn_SRC_NotSameShapeH h)
                                {
                                    shape = "H";
                                    strength.Add(h.strength_main);
                                    strength.Add(h.strength_web);
                                }
                                else if (notSame.Item is StbSecColumn_SRC_NotSameShapeBox b)
                                {
                                    shape = "Box";
                                    strength.Add(b.strength);
                                }
                                else if (notSame.Item is StbSecColumn_SRC_NotSameShapePipe p)
                                {
                                    shape = "Pipe";
                                    strength.Add(p.strength);
                                }
                                else if (notSame.Item is StbSecColumn_SRC_NotSameShapeCross c)
                                {
                                    shape = "CROSS";
                                    strength.Add(c.strength_main_X);
                                    strength.Add(c.strength_main_Y);
                                    strength.Add(c.strength_web_X);
                                    strength.Add(c.strength_web_Y);
                                }
                                else if (notSame.Item is StbSecColumn_SRC_NotSameShapeT t)
                                {
                                    shape = "T";
                                    strength.Add(t.strength_main_H);
                                    strength.Add(t.strength_main_T);
                                    strength.Add(t.strength_web_H);
                                    strength.Add(t.strength_web_T);
                                }
                            }
                            else if (fig is StbSecSteelColumn_SRC_Same same)
                            {
                                if (same.Item is StbSecColumn_SRC_SameShapeH h)
                                {
                                    shape = "H";
                                    strength.Add(h.strength_main);
                                    strength.Add(h.strength_web);
                                }
                                else if (same.Item is StbSecColumn_SRC_SameShapeBox b)
                                {
                                    shape = "Box";
                                    strength.Add(b.strength);
                                }
                                else if (same.Item is StbSecColumn_SRC_SameShapePipe p)
                                {
                                    shape = "Pipe";
                                    strength.Add(p.strength);
                                }
                                else if (same.Item is StbSecColumn_SRC_SameShapeCross c)
                                {
                                    shape = "CROSS";
                                    strength.Add(c.strength_main_X);
                                    strength.Add(c.strength_main_Y);
                                    strength.Add(c.strength_web_X);
                                    strength.Add(c.strength_web_Y);
                                }
                                else if (same.Item is StbSecColumn_SRC_SameShapeT t)
                                {
                                    shape = "T";
                                    strength.Add(t.strength_main_H);
                                    strength.Add(t.strength_main_T);
                                    strength.Add(t.strength_web_H);
                                    strength.Add(t.strength_web_T);
                                }
                            }
                            else if (fig is StbSecSteelColumn_SRC_ThreeTypes threeTypes)
                            {
                                if (threeTypes.Item is StbSecColumn_SRC_ThreeTypesShapeH h)
                                {
                                    shape = "H";
                                    strength.Add(h.strength_main);
                                    strength.Add(h.strength_web);
                                }
                                else if (threeTypes.Item is StbSecColumn_SRC_ThreeTypesShapeBox b)
                                {
                                    shape = "Box";
                                    strength.Add(b.strength);
                                }
                                else if (threeTypes.Item is StbSecColumn_SRC_ThreeTypesShapePipe p)
                                {
                                    shape = "Pipe";
                                    strength.Add(p.strength);
                                }
                                else if (threeTypes.Item is StbSecColumn_SRC_ThreeTypesShapeCross c)
                                {
                                    shape = "CROSS";
                                    strength.Add(c.strength_main_X);
                                    strength.Add(c.strength_main_Y);
                                    strength.Add(c.strength_web_X);
                                    strength.Add(c.strength_web_Y);
                                }
                                else if (threeTypes.Item is StbSecColumn_SRC_ThreeTypesShapeT t)
                                {
                                    shape = "T";
                                    strength.Add(t.strength_main_H);
                                    strength.Add(t.strength_main_T);
                                    strength.Add(t.strength_web_H);
                                    strength.Add(t.strength_web_T);
                                }
                            }

                            int FigureType = 1;
                            if (clm.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                            {
                                FigureType = 1;
                            }
                            else
                            {
                                FigureType = 2;
                            }

                            if (shape == "H")
                            {
                                if (FigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][0]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][3]; }
                            }
                            else if (shape == "CROSS")
                            {
                                if (FigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][1]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][4]; }
                            }
                            else if (shape == "T")
                            {
                                if (FigureType == 1)
                                { nf.name = RevitLNK.ClmText[2][2]; }
                                else
                                { nf.name = RevitLNK.ClmText[2][5]; }
                            }
                            else if (shape == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC柱(StbSecColumn_SRC)");
                            }

                            foreach (var s in strength)
                            {
                                Data.MateData_Add(s);
                            }


                            f.STBload_Add(nf);

                        }

                        if (clm.StbSecSteelFigureColumn_SRC.base_type == StbSecSteelFigureColumn_SRCBase_type.UNEMBEDDED)
                        {
                            if (clm.Item != null && clm.Item is StbSecBaseProduct_SRC baseProduct)
                            {
                                var lf = new ConvertForm.STBLoadflg
                                {
                                    kind = "柱脚",
                                    flg = true,
                                    name = "柱脚"
                                };
                                f.STBload_Add(lf);

                                var bc = new ConvertForm.BClmData
                                {
                                    company = baseProduct.product_company,
                                    product_code = baseProduct.product_code
                                };
                                f.BClm_Add(bc);

                                for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                {
                                    if (RevitLNK.BClm[b].product_company == baseProduct.product_company &&
                                        RevitLNK.BClm[b].product_code == baseProduct.product_code)
                                    {
                                        RevitLNK.BClm[b].flg = true;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC柱(StbSecColumn_SRC)");
                    }


                    string conc = clm.strength_concrete;
                    clm.strength_concrete = ConcData_Add(stb2, clm.floor, "SRC", ref conc);

                }
            }

            if (stb2.StbModel.StbSections.StbSecColumn_CFT != null)
            {
                foreach (var clm in stb2.StbModel.StbSections.StbSecColumn_CFT)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = clm.strength_concrete;
                        clm.strength_concrete = ConcData_Add(stb2, clm.floor, "CFT", ref conc);

                        foreach (var fig in clm.StbSecSteelFigureColumn_CFT.Items)
                        {
                            //鉄骨材料情報

                            string shape = "";
                            if (fig is StbSecSteelColumn_CFT_NotSame notSame)
                            {
                                Data.MateData_Add(notSame.strength);
                                shape = notSame.shape;
                            }
                            else if (fig is StbSecSteelColumn_CFT_Same same)
                            {
                                Data.MateData_Add(same.strength);
                                shape = same.shape;
                            }
                            else if (fig is StbSecSteelColumn_CFT_ThreeTypes threeTypes)
                            {
                                Data.MateData_Add(threeTypes.strength);
                                shape = threeTypes.shape;
                            }

                            string shapetype = Check_Steel(stb2, shape, out int ind);
                            var nf = new ConvertForm.STBLoadflg
                            {
                                kind = clm.kind_column.ToString(),
                                flg = true,
                            };

                            if (nf.kind == "")
                            {
                                nf.kind = StbSecColumn_Kind_column.COLUMN.ToString();
                            }

                            if (shapetype == RevitLNK.st_steel_Box || shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.ClmText[3][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.ClmText[3][1];
                            }
                            else if (shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "CFT柱(StbSecColumn_CFT)");
                            }
                            f.STBload_Add(nf);
                        }

                        if (clm.StbSecSteelFigureColumn_CFT.base_type == StbSecSteelFigureColumn_CFTBase_type.EXPOSE)
                        {
                            if (clm.Item != null && clm.Item is StbSecBaseProduct_CFT baseProduct)
                            {
                                var lf = new ConvertForm.STBLoadflg
                                {
                                    kind = "柱脚",
                                    flg = true,
                                    name = "柱脚"
                                };
                                f.STBload_Add(lf);

                                var bc = new ConvertForm.BClmData
                                {
                                    company = baseProduct.product_company,
                                    product_code = baseProduct.product_code
                                };
                                f.BClm_Add(bc);

                                for (int b = 0; b < RevitLNK.BClm.Count(); b++)
                                {
                                    if (RevitLNK.BClm[b].product_company == baseProduct.product_company &&
                                        RevitLNK.BClm[b].product_code == baseProduct.product_code)
                                    {
                                        RevitLNK.BClm[b].flg = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "CFT柱(StbSecColumn_CFT)");
                    }
                }
            }

        }

        internal static void CheckSTB_Girder(ST_BRIDGE stb2, ConvertForm f)
        {
            if (stb2.StbModel.StbSections.StbSecBeam_RC != null)
            {
                foreach (var gir in stb2.StbModel.StbSections.StbSecBeam_RC)
                {
                    try
                    {
                        var nf = new ConvertForm.STBLoadflg
                        {
                            kind = gir.kind_beam.ToString(),
                            flg = true,
                        };

                        if (nf.kind == "")
                        {
                            nf.kind = StbSecBeam_Kind_beam.GIRDER.ToString();
                        }

                        if (gir.isCanti)
                        {
                            if (gir.isFoundation)
                            {
                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                { nf.name = RevitLNK.CGirText[0][0]; }
                                else
                                { nf.name = RevitLNK.CBeamText[0][0]; }
                            }
                            else
                            {
                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                { nf.name = RevitLNK.CGirText[0][1]; }
                                else
                                { nf.name = RevitLNK.CBeamText[0][1]; }
                            }
                        }
                        else
                        {
                            var fig = gir.StbSecFigureBeam_RC;
                            switch (fig.FigureType)
                            {
                                case 1:
                                    if (gir.StbSecBarArrangementBeam_RC == null) //鉄筋タグが無ければ全断面として変換
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.StbSecBarArrangementBeam_RC.Bar_ArrangementType == 1)
                                        {
                                            if (gir.isFoundation)
                                            {
                                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                                { nf.name = RevitLNK.GirText[0][0]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][0]; }
                                            }
                                            else
                                            {
                                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                                { nf.name = RevitLNK.GirText[0][2]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][2]; }
                                            }
                                        }
                                        else
                                        {
                                            if (gir.isFoundation)
                                            {
                                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                                { nf.name = RevitLNK.GirText[0][1]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][1]; }
                                            }
                                            else
                                            {
                                                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                                { nf.name = RevitLNK.GirText[0][3]; }
                                                else
                                                { nf.name = RevitLNK.BeamText[0][3]; }
                                            }
                                        }
                                    }
                                    break;

                                case 2:
                                    var taper0 = (StbSecBeam_RC_Taper)fig.Items[0];
                                    var taper1 = (StbSecBeam_RC_Taper)fig.Items[1];
                                    if (taper0.depth != taper1.depth ||
                                        taper0.width != taper1.width)
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][1]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][1]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][3]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][3]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    break;

                                case 3:
                                    var haunchS = fig.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                                    var haunchC = fig.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                    var haunchE = fig.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                                    if (haunchS == null) haunchS = haunchC;
                                    if (haunchE == null) haunchE = haunchC;

                                    if (haunchS.depth != haunchC.depth || haunchE.depth != haunchC.depth ||
                                        haunchS.width != haunchC.width || haunchE.width != haunchC.width)
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][1]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][1]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][3]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][3]; }
                                        }
                                    }
                                    else
                                    {
                                        if (gir.isFoundation)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][0]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[0][2]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[0][2]; }
                                        }
                                    }
                                    break;
                            }
                        }
                        f.STBload_Add(nf);

                        //コンクリート情報
                        string conc = gir.strength_concrete;
                        gir.strength_concrete = ConcData_Add(stb2, gir.floor, "RC", ref conc);

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC梁(StbSecBeam_RC)");
                    }

                }
            }

            if (stb2.StbModel.StbSections.StbSecBeam_S != null)
            {
                foreach (var gir in stb2.StbModel.StbSections.StbSecBeam_S)
                {
                    try
                    {
                        foreach (var fig in gir.StbSecSteelFigureBeam_S.Items)
                        {
                            string shape = "";

                            //鉄骨材料情報
                            if (fig is StbSecSteelBeam_S_Straight s)
                            {
                                Data.MateData_Add(s.strength_main);
                                Data.MateData_Add(s.strength_web);
                                shape = s.shape;
                            }
                            else if (fig is StbSecSteelBeam_S_Taper t)
                            {
                                Data.MateData_Add(t.strength_main);
                                Data.MateData_Add(t.strength_web);
                                shape = t.shape;
                            }
                            else if (fig is StbSecSteelBeam_S_Joint j)
                            {
                                Data.MateData_Add(j.strength_main);
                                Data.MateData_Add(j.strength_web);
                                shape = j.shape;
                            }
                            else if (fig is StbSecSteelBeam_S_Haunch h)
                            {
                                Data.MateData_Add(h.strength_main);
                                Data.MateData_Add(h.strength_web);
                                shape = h.shape;
                            }
                            else if (fig is StbSecSteelBeam_S_FiveTypes five)
                            {
                                Data.MateData_Add(five.strength_main);
                                Data.MateData_Add(five.strength_web);
                                shape = five.shape;
                            }

                            //鉄骨形状
                            string shapetype = Check_Steel(stb2, shape, out int ind);

                            var nf = new ConvertForm.STBLoadflg
                            {
                                kind = gir.kind_beam.ToString(),
                                flg = true,
                            };

                            if (nf.kind == "")
                            {
                                nf.kind = StbSecBeam_Kind_beam.GIRDER.ToString();
                            }

                            switch (shapetype)
                            {
                                case RevitLNK.st_steel_H:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.CGirText[1][0]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][0]; }
                                    }
                                    else
                                    {
                                        //ハンチ付か3断面同一かを判断する
                                        //ストレートの場合のみ全断面（継手ありは板厚は変えることができるので3断面として扱う）
                                        if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[1][5]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[1][5]; }
                                        }
                                        else
                                        {
                                            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                            { nf.name = RevitLNK.GirText[1][0]; }
                                            else
                                            { nf.name = RevitLNK.BeamText[1][0]; }
                                        }
                                    }
                                    break;
                                case RevitLNK.st_steel_BH:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.CGirText[1][1]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][1]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.GirText[1][1]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][1]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_C:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.CGirText[1][2]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][2]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.GirText[1][2]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][2]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_L:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.CGirText[1][3]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][3]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.GirText[1][3]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][3]; }
                                    }
                                    break;
                                case RevitLNK.st_steel_LipC:
                                    if (gir.isCanti)
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.CGirText[1][4]; }
                                        else
                                        { nf.name = RevitLNK.CBeamText[1][4]; }
                                    }
                                    else
                                    {
                                        if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                        { nf.name = RevitLNK.GirText[1][4]; }
                                        else
                                        { nf.name = RevitLNK.BeamText[1][4]; }
                                    }
                                    break;
                                case "":
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S梁(StbSecBeam_S)");
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + gir.name + "(断面id=" + gir.id.ToString() + ") shape=\"" + shape + "\" ");
                                    break;

                            }

                            f.STBload_Add(nf);
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "S梁(StbSecBeam_S)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecBeam_SRC != null)
            {
                foreach (var gir in stb2.StbModel.StbSections.StbSecBeam_SRC)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = gir.strength_concrete;
                        gir.strength_concrete = ConcData_Add(stb2, gir.floor, "SRC", ref conc);

                        foreach (var fig in gir.StbSecSteelFigureBeam_SRC.Items)
                        {
                            string shape = "";

                            //鉄骨材料情報
                            if (fig is StbSecSteelBeam_SRC_Straight s)
                            {
                                Data.MateData_Add(s.strength_main);
                                Data.MateData_Add(s.strength_web);
                                shape = s.shape;
                            }
                            else if (fig is StbSecSteelBeam_SRC_Taper t)
                            {
                                Data.MateData_Add(t.strength_main);
                                Data.MateData_Add(t.strength_web);
                                shape = t.shape;
                            }
                            else if (fig is StbSecSteelBeam_SRC_Joint j)
                            {
                                Data.MateData_Add(j.strength_main);
                                Data.MateData_Add(j.strength_web);
                                shape = j.shape;
                            }
                            else if (fig is StbSecSteelBeam_SRC_Haunch h)
                            {
                                Data.MateData_Add(h.strength_main);
                                Data.MateData_Add(h.strength_web);
                                shape = h.shape;
                            }
                            else if (fig is StbSecSteelBeam_SRC_FiveTypes five)
                            {
                                Data.MateData_Add(five.strength_main);
                                Data.MateData_Add(five.strength_web);
                                shape = five.shape;
                            }

                            //鉄骨形状
                            string shapetype = Check_Steel(stb2, shape, out int ind);

                            if (shapetype == "") { LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC梁(StbSecBeam_SRC)"); }

                            var nf = new ConvertForm.STBLoadflg
                            {
                                kind = gir.kind_beam.ToString(),
                                flg = true,
                            };

                            if (nf.kind == "")
                            {
                                nf.kind = StbSecBeam_Kind_beam.GIRDER.ToString();
                            }

                            if (shapetype == RevitLNK.st_steel_H || shapetype == RevitLNK.st_steel_BH)
                            {
                                if (gir.isCanti)
                                {
                                    if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                    { nf.name = RevitLNK.CGirText[2][0]; }
                                    else
                                    { nf.name = RevitLNK.CBeamText[2][0]; }
                                }
                                else
                                {
                                    if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                                    { nf.name = RevitLNK.GirText[2][0]; }
                                    else
                                    { nf.name = RevitLNK.BeamText[2][0]; }
                                }
                            }
                            else if (shapetype == "")
                            {
                                LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC梁(StbSecBeam_SRC)");
                            }
                            f.STBload_Add(nf);
                        }

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "SRC梁(StbSecBeam_SRC)");
                    }

                }
            }

        }

        internal static void CheckSTB_Brace(ST_BRIDGE stb2, ConvertForm f)
        {
            if (stb2.StbModel.StbSections.StbSecBrace_S != null)
            {
                foreach (var bra in stb2.StbModel.StbSections.StbSecBrace_S)
                {
                    try
                    {
                        foreach (var fig in bra.StbSecSteelFigureBrace_S.Items)
                        {
                            string shape = "";

                            //鉄骨材料情報
                            if (fig is StbSecSteelBrace_S_Same same)
                            {
                                Data.MateData_Add(same.strength_main);
                                Data.MateData_Add(same.strength_web);
                                shape = same.shape;
                            }
                            else if (fig is StbSecSteelBrace_S_NotSame notsame)
                            {
                                Data.MateData_Add(notsame.strength_main);
                                Data.MateData_Add(notsame.strength_web);
                                shape = notsame.shape;
                            }
                            else if (fig is StbSecSteelBrace_S_ThreeTypes three)
                            {
                                Data.MateData_Add(three.strength_main);
                                Data.MateData_Add(three.strength_web);
                                shape = three.shape;
                            }

                            string shapetype = Check_Steel(stb2, shape, out int ind);

                            var nf = new ConvertForm.STBLoadflg
                            {
                                flg = true
                            };

                            if (shapetype == RevitLNK.st_steel_H)
                            {
                                nf.name = RevitLNK.SBraText[0][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_BH)
                            {
                                nf.name = RevitLNK.SBraText[0][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_Box)
                            {
                                nf.name = RevitLNK.SBraText[0][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_BBox)
                            {
                                nf.name = RevitLNK.SBraText[0][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Pipe)
                            {
                                nf.name = RevitLNK.SBraText[0][4];
                            }
                            else if (shapetype == RevitLNK.st_steel_C)
                            {
                                nf.name = RevitLNK.SBraText[1][0];
                            }
                            else if (shapetype == RevitLNK.st_steel_L)
                            {
                                nf.name = RevitLNK.SBraText[1][1];
                            }
                            else if (shapetype == RevitLNK.st_steel_LipC)
                            {
                                nf.name = RevitLNK.SBraText[1][2];
                            }
                            else if (shapetype == RevitLNK.st_steel_FB)
                            {
                                nf.name = RevitLNK.SBraText[1][3];
                            }
                            else if (shapetype == RevitLNK.st_steel_Bar)
                            {
                                nf.name = RevitLNK.SBraText[1][4];
                            }
                            else
                            {
                                //ログ表示(変換対象外)
                                if (shapetype == "")
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "Sブレース(StbSecBrace_S)");
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "  " + bra.name + "(断面id=" + bra.id.ToString() + ") shape=\"" + shape + "\" ");
                                }
                                else
                                { LogData.AddLog(LogData.LogKind.Warning, 2200, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")はT形鋼(" + shapetype + ")"); }
                            }
                            f.STBload_Add(nf);
                        }
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "Sブレース(StbSecBrace_S)");
                    }

                }
            }

        }

        internal static void CheckSTB_Foundation(ST_BRIDGE stb2, ConvertForm f)
        {
            if (stb2.StbModel.StbSections.StbSecFoundation_RC != null)
            {
                foreach (var frc in stb2.StbModel.StbSections.StbSecFoundation_RC)
                {
                    try
                    {
                        var nf = new ConvertForm.STBLoadflg
                        {
                            flg = true
                        };

                        if (frc.StbSecFigureFoundation_RC != null)
                        {
                            switch (frc.StbSecFigureFoundation_RC.FigureType)
                            {
                                case 1:
                                    nf.name = RevitLNK.BaseText[0][0];
                                    break;
                                case 2:
                                    nf.name = RevitLNK.BaseText[0][1];
                                    break;
                                case 3:
                                    nf.name = RevitLNK.BaseText[0][2];
                                    break;
                                case 4:
                                    nf.name = RevitLNK.BaseText[0][3];
                                    break;
                                case 5:
                                    nf.name = RevitLNK.BaseText[0][4];
                                    break;
                                case 6:
                                    nf.name = RevitLNK.BaseText[1][0];
                                    break;
                            }
                        }
                        f.STBload_Add(nf);

                        //コンクリート情報
                        string conc = frc.strength_concrete;
                        frc.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC基礎(StbSecFoundations_RC)");
                    }

                }
            }

            if (stb2.StbModel.StbSections.StbSecPile_RC != null)
            {
                foreach (var prc in stb2.StbModel.StbSections.StbSecPile_RC)
                {
                    try
                    {
                        bool logflg = false;
                        if (prc.StbSecFigurePile_RC != null)
                        {
                            switch (prc.StbSecFigurePile_RC.FigureType)
                            {
                                case 1:
                                    if (((StbSecPile_RC_Straight)prc.StbSecFigurePile_RC.Item).D == 0)
                                    { logflg = true; }
                                    break;
                                case 2:
                                    var foot = (StbSecPile_RC_ExtendedFoot)prc.StbSecFigurePile_RC.Item;
                                    if (foot.D_axial == 0 || foot.D_extended_foot == 0)
                                    { logflg = true; }
                                    break;
                                case 3:
                                    var top = (StbSecPile_RC_ExtendedTop)prc.StbSecFigurePile_RC.Item;
                                    if (top.D_axial == 0 || top.D_extended_top == 0)
                                    { logflg = true; }
                                    break;
                                case 4:
                                    var top_foot = (StbSecPile_RC_ExtendedTopFoot)prc.StbSecFigurePile_RC.Item;
                                    if (top_foot.D_axial == 0 ||
                                        top_foot.D_extended_foot == 0 ||
                                        top_foot.D_extended_top == 0)
                                    { logflg = true; }
                                    break;
                            }
                        }
                        if (logflg == true)
                        {
                            LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPiles_RC)");
                            break;
                        }

                        var nf = new ConvertForm.STBLoadflg
                        {
                            flg = true,
                            name = RevitLNK.BaseText[2][0]
                        };
                        f.STBload_Add(nf);

                        //コンクリート情報
                        string conc = prc.strength_concrete;
                        prc.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC杭(StbSecPiles_RC)");
                    }

                }
            }

            if (stb2.StbModel.StbSections.StbSecPile_S != null)
            {
                foreach (var prc in stb2.StbModel.StbSections.StbSecPile_S)
                {
                    try
                    {
                        bool logflg = false;
                        if (prc.StbSecFigurePile_S != null)
                        {
                            if (prc.StbSecFigurePile_S.StbSecPile_S_Straight.Count > 0)
                            {
                                foreach (var p in prc.StbSecFigurePile_S.StbSecPile_S_Straight)
                                {
                                    if (p.D < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (logflg == true)
                        {
                            LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPile_S)");
                            break;
                        }

                        var nf = new ConvertForm.STBLoadflg
                        {
                            flg = true,
                            name = RevitLNK.BaseText[2][2]
                        };
                        f.STBload_Add(nf);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "鋼管杭(StbSecPile_S)");
                    }

                }
            }

            if (stb2.StbModel.StbSections.StbSecPileProduct != null)
            {
                foreach (var prc in stb2.StbModel.StbSections.StbSecPileProduct)
                {
                    try
                    {
                        if (prc.StbSecFigurePileProduct.StbSecPileProduct_PHC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProduct_PHC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProduct_PHC)
                                {
                                    if (p.D < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][3]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProduct_ST != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProduct_ST.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProduct_ST)
                                {
                                    if (p.D1 < 1 || p.D2 < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][4]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProduct_SC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProduct_SC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProduct_SC)
                                {
                                    if (p.D < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][5]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProduct_PRC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProduct_PRC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProduct_PRC)
                                {
                                    if (p.D < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][6]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProduct_CPRC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProduct_CPRC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProduct_CPRC)
                                {
                                    if (p.D < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][7]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_PHC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_PHC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProductNodular_PHC)
                                {
                                    if (p.D1 < 1 || p.D2 < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][3]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_PRC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_PRC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProductNodular_PRC)
                                {
                                    if (p.D1 < 1 || p.D2 < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][6]
                                };
                                f.STBload_Add(nf);
                            }
                        }


                        if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC != null)
                        {
                            if (prc.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC.Count > 0)
                            {
                                bool logflg = false;

                                foreach (var p in prc.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC)
                                {
                                    if (p.D1 < 1 || p.D2 < 1)
                                    {
                                        logflg = true;
                                        break;
                                    }
                                }

                                if (logflg == true)
                                {
                                    LogData.AddSTBLog(LogData.LogKind.Error, 3100, "杭基礎(StbSecPileProduct)");
                                    break;
                                }

                                var nf = new ConvertForm.STBLoadflg
                                {
                                    flg = true,
                                    name = RevitLNK.BaseText[2][7]
                                };
                                f.STBload_Add(nf);
                            }
                        }

                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "既製杭(StbSecPileProduct)");
                    }

                }
            }



        }

        internal static void CheckSTB_Sonota(ST_BRIDGE stb2, ConvertForm f)
        {
            if (stb2.StbModel.StbMembers.StbFoundationColumns != null)
            {
                bool recflg = false, rouflg = false;
                foreach (var col in stb2.StbModel.StbMembers.StbFoundationColumns)
                {
                    if (recflg && rouflg) { break; }

                    try
                    {
                        for (int k = 0; k <= 1; ++k)
                        {
                            int id_section = k == 0 ? col.id_section_FD : col.id_section_WR;
                            var secCol = stb2.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == id_section);
                            if (secCol != null)
                            {
                                if (secCol.StbSecFigureColumn_RC.Item != null)
                                {
                                    if (secCol.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect)
                                    {
                                        var nf = new ConvertForm.STBLoadflg
                                        {
                                            flg = true,
                                            name = RevitLNK.FClmText[0][0]
                                        };
                                        f.STBload_Add(nf);
                                        recflg = true;
                                    }
                                    else if (secCol.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Circle)
                                    {
                                        var nf = new ConvertForm.STBLoadflg
                                        {
                                            flg = true,
                                            name = RevitLNK.FClmText[0][1]
                                        };
                                        f.STBload_Add(nf);
                                        rouflg = true;
                                    }
                                }
                            }
                        }

                    }
                    catch
                    {
                        //形状はRC柱からとるのですでにチェック済み
                    }

                }
                //コンクリート情報はすでにColumn_RCで取得済み
            }

            if (stb2.StbModel.StbSections.StbSecSlab_RC != null && stb2.StbModel.StbSections.StbSecSlab_RC.Count > 0)
            {
                var nf = new ConvertForm.STBLoadflg
                {
                    kind = "RCスラブ",
                    flg = true,
                    name = "スラブ、デッキプレート"
                };
                f.STBload_Add(nf);

                foreach (var sla in stb2.StbModel.StbSections.StbSecSlab_RC)
                {
                    try
                    {
                        if (sla.isFoundation)
                        {
                            var nf2 = new ConvertForm.STBLoadflg
                            {
                                kind = "基礎スラブ",
                                flg = true,
                                name = "基礎スラブ"
                            };
                            f.STBload_Add(nf2);
                        }

                        //コンクリート情報
                        string conc = sla.strength_concrete;
                        sla.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RCスラブ(StbSecSlabs_RC)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecSlabDeck != null && stb2.StbModel.StbSections.StbSecSlabDeck.Count > 0)
            {
                var nf = new ConvertForm.STBLoadflg
                {
                    kind = "デッキプレート",
                    flg = true,
                    name = "スラブ、デッキプレート"
                };
                f.STBload_Add(nf);

                foreach (var deck in stb2.StbModel.StbSections.StbSecSlabDeck)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = deck.strength_concrete;
                        deck.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "デッキスラブ(StbSecSlabs_Deck)");
                    }

                }
            }

            if (stb2.StbModel.StbSections.StbSecSlabPrecast != null && stb2.StbModel.StbSections.StbSecSlabPrecast.Count > 0)
            {
                var nf = new ConvertForm.STBLoadflg
                {
                    kind = "既製スラブ",
                    flg = true,
                    name = "スラブ、デッキプレート"
                };
                f.STBload_Add(nf);

                foreach (var prod in stb2.StbModel.StbSections.StbSecSlabPrecast)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = prod.strength_concrete;
                        prod.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "既製スラブ(StbSecSlabs_Precast)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecWall_RC != null && stb2.StbModel.StbSections.StbSecWall_RC.Count > 0)
            {
                var nf = new ConvertForm.STBLoadflg
                {
                    kind = "壁",
                    flg = true,
                    name = "壁、RCパラペット"
                };
                f.STBload_Add(nf);
                foreach (var wall in stb2.StbModel.StbSections.StbSecWall_RC)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = wall.strength_concrete;
                        wall.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RC壁(StbSecWalls_RC)");
                    }
                }
            }

            if (stb2.StbModel.StbSections.StbSecParapet_RC != null && stb2.StbModel.StbSections.StbSecParapet_RC.Count > 0)
            {
                var nf = new ConvertForm.STBLoadflg
                {
                    kind = "RCパラペット",
                    flg = true,
                    name = "壁、RCパラペット"
                };
                f.STBload_Add(nf);

                foreach (var wall in stb2.StbModel.StbSections.StbSecParapet_RC)
                {
                    try
                    {
                        //コンクリート情報
                        string conc = wall.strength_concrete;
                        wall.strength_concrete = ConcData_Add(stb2, "", "RC", ref conc);
                    }
                    catch
                    {
                        LogData.AddSTBLog(LogData.LogKind.Error, 3100, "RCパラペット(StbSecParapets_RC)");
                    }
                }
            }

        }


        #endregion



        private static IDictionary<string, string> GetPropertyName_and_Value<T>(T data) where T : class
        {
            if (data == null) return null;

            IDictionary<string, string> dic = new Dictionary<string, string>();

            var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                dic.Add(property.Name, property.GetValue(data)?.ToString() ?? "");
            }

            return dic;
        }



        /// <summary>
        /// StbCommonの情報を拡張ストレージに保持する
        /// </summary>
        /// <param name="stb"></param>
        private static void AddCommonData(ST_BRIDGE stb)
        {
            if (stb.StbCommon == null) return;
            if (stb.StbCommon.StbApplyConditionsList == null) return;

            try
            {
                var schema1 = Data.GetSchema(Data.schemaName_StbCommon);
                if (schema1 != null)
                {
                    //既にある場合は削除して作り直す
                    Commons.doc.EraseSchemaAndAllEntities(schema1);
                }

                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetSchemaName(Data.schemaName_StbCommon);


                //field作成
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply)  , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply)     , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply) , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply)    , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply)    , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply)           , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply)       , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply)   , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply)          , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply)      , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply)      , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply)      , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply), typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply)      , typeof(string), typeof(string));
                schemaBuilder.AddMapField(nameof(stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply)   , typeof(string), typeof(string));

                Schema schema = schemaBuilder.Finish();
                Entity entity = new Entity(schema);

                var apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply ?? new StbColumn_RC_RebarPositionApply());
                var field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_RebarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply ?? new StbColumn_RC_BarSpacingApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_RC_BarSpacingApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply ?? new StbColumn_SRC_RebarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_RebarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply ?? new StbColumn_SRC_BarSpacingApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbColumn_SRC_BarSpacingApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply ?? new StbBeam_RC_RebarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_RebarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply ?? new StbBeam_RC_BarWebApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarWebApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply ?? new StbBeam_RC_BarSpacingApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_RC_BarSpacingApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply ?? new StbBeam_SRC_RebarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_RebarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply ?? new StbBeam_SRC_BarWebApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarWebApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply ?? new StbBeam_SRC_BarSpacingApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbBeam_SRC_BarSpacingApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply ?? new StbSlab_RC_BarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbSlab_RC_BarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply ?? new StbWall_RC_BarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbWall_RC_BarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply ?? new StbFoundation_RC_BarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbFoundation_RC_BarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply ?? new StbPile_RC_BarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbPile_RC_BarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);

                apply_data = GetPropertyName_and_Value(stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply ?? new StbParapet_RC_BarPositionApply());
                field = schema.GetField(nameof(stb.StbCommon.StbApplyConditionsList.StbParapet_RC_BarPositionApply));
                if (apply_data != null & field != null) entity.Set(field, apply_data);


                //プロジェクト情報にセット
                Commons.doc.ProjectInformation.SetEntity(entity);
            }
            catch //(Exception ex)
            {
            }
        }



        /// <summary>
        /// プロジェクト情報への追加
        /// </summary>
        /// <param name="stb"></param>
        internal static void AddProjectParameter(ST_BRIDGE stb)
        {
            DefinitionFile infomationFile = Commons.doc.Application.OpenSharedParameterFile();
            DefinitionGroups infomationCollections = infomationFile.Groups;
            DefinitionGroup informationCollection = infomationCollections.get_Item(RevitLNK.groupName);

            Definition information = null;

            //プロジェクト情報にパラメータを追加
            CategorySet mappingCategories = Commons.doc.Application.Create.NewCategorySet();
            mappingCategories.Insert(Commons.doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation));
            InstanceBinding binding = Commons.doc.Application.Create.NewInstanceBinding(mappingCategories);



            Transaction tran1 = new Transaction(Commons.doc, "プロジェクトパラメータの追加");

            try
            {
                tran1.Start();

                foreach (string mappingsParam in Data.projectParams)
                {
                    information = informationCollection.Definitions.get_Item(mappingsParam);

                    if (information == null)
                    {
                        ExternalDefinitionCreationOptions edco = new ExternalDefinitionCreationOptions(mappingsParam, SpecTypeId.String.Text)
                        {
                            Visible = true
                        };
                        informationCollection.Definitions.Create(edco);
                        information = informationCollection.Definitions.get_Item(mappingsParam);
                    }

                    //ドキュメントにパラメータを追加
                    Commons.doc.ParameterBindings.Insert(information, binding);
                }

                Commons.doc.Regenerate();

                //プログレスバーの準備
                Data.ProgressStart("プロジェクト情報の追加", Data.projectParams.Count);

                //プロジェクト情報に設定
                ProjectInfo pinfo = Commons.doc.ProjectInformation;
                Parameter p = null;
                for (int i = 0; i < Data.projectParams.Count; i++)
                {
                    Data.ProgressPerformStep();

                    p = pinfo.LookupParameter(Data.projectParams[i]);
                    if (p == null) { continue; }
                    string lp = "";
                    switch (i)
                    {
                        case 0:
                            p.Set(RevitLNK.openfilename);
                            break;
                        case 1:
                            p.Set(RevitLNK.filedata);
                            break;
                        case 2:
                            lp = "";
                            if (ConvertForm.LMD.RevitLevel != null)
                            {
                                for (int j = 0; j < ConvertForm.LMD.RevitLevel.Count(); j++)
                                {
                                    if (ConvertForm.LMD.RevitLevel[j] == null) { continue; }
                                    lp += ConvertForm.LMD.RevitLevel[j];
                                    lp += ",";
                                    if (ConvertForm.LMD.RevitOffset[j] == null) { continue; }
                                    lp += ConvertForm.LMD.RevitOffset[j];
                                    lp += ",";
                                }
                                p.Set(lp);
                            }
                            break;
                        case 3:
                            lp = "";
                            lp += ConvertForm.LMD.rdb.ToString();
                            lp += ",";
                            lp += ConvertForm.LMD.STB_X;
                            lp += ",";
                            lp += ConvertForm.LMD.STB_Y;
                            lp += ",";
                            lp += ConvertForm.LMD.RVT_X;
                            lp += ",";
                            lp += ConvertForm.LMD.RVT_Y;
                            lp += ",";
                            lp += ConvertForm.LMD.Offset_X1.ToString();
                            lp += ",";
                            lp += ConvertForm.LMD.Offset_Y1.ToString();
                            lp += ",";
                            lp += ConvertForm.LMD.Offset_X2.ToString();
                            lp += ",";
                            lp += ConvertForm.LMD.Offset_Y2.ToString();
                            p.Set(lp);
                            break;
                        case 4:
                            //string conc = "";
                            //for (int j = 0; j < ConvertForm.Concname.Count(); j++)
                            //{
                            //    if (ConvertForm.Concname[j] == null) { continue; }
                            //    conc += ConvertForm.Concname[j];
                            //    if (j != RevitLNK.ConcData.Count() - 1)
                            //    {
                            //        conc += ",";
                            //    }
                            //}
                            //p.Set(conc);
                            break;
                        case 5:
                            //string mat = "";
                            //for (int j = 0; j < ConvertForm.TekkotuPare.Count(); j++)
                            //{
                            //    if (ConvertForm.TekkotuPare[j] == null) { continue; }

                            //    mat += ConvertForm.TekkotuPare[j].STB;
                            //    mat += ",";
                            //    mat += ConvertForm.TekkotuPare[j].RVT;
                            //    if (j != ConvertForm.TekkotuPare.Count() - 1)
                            //    {
                            //        mat += ",";
                            //    }
                            //}
                            //p.Set(mat);
                            break;
                        case 6:
                            p.Set(stb.StbCommon.guid);
                            break;
                        case 7:
                            p.Set(stb.StbCommon.project_name);
                            break;
                        case 8:
                            p.Set(stb.StbCommon.app_name);
                            break;
                        case 9:
                            p.Set(stb.StbCommon.strength_concrete);
                            break;
                        case 10:
                            //鉄骨の規格 STB2.0にない
                            p.Set("");
                            break;
                        case 11:
                            if (stb.StbCommon.StbReinforcementStrengthList == null) { break; }
                            if (stb.StbCommon.StbReinforcementStrengthList.Count == 0) { break; }

                            string pset = string.Join(",", stb.StbCommon.StbReinforcementStrengthList.Select(a => a.D + "," + a.strength));
                            p.Set(pset);
                            break;
                    }
                }


                AddCommonData(stb);


                tran1.Commit();
            }
            catch (Exception)
            {
                tran1.RollBack();
            }

            Data.ProgressClose();
        }

        /// <summary>
        /// レベルの生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="Lpare">レベルマッピングの情報</param>
        internal static void Level_Convert(ST_BRIDGE stb, List<RevitLNK.LevelPare> Lpare)
        {
            Levels = new List<Level>();
            alloffsetZ = new List<Data.OffsetZ>();


            Data.ProgressStart("レベルの生成", stb.StbModel.StbStories.Count);

            Transaction tran = new Transaction(Commons.doc, "レベルの生成");
            tran.Start();

            try
            {
                //変換情報ログのための変数
                XYZ ps = null, pe = null;
                string stage = "";
                string levelname = "";


                //平面図のファミリタイプ
                var viewFamilyTypes = from elem in new FilteredElementCollector(Commons.doc).OfClass(typeof(ViewFamilyType)) let type = elem as ViewFamilyType where type.ViewFamily == ViewFamily.StructuralPlan select type;
                for (int i = 0; i < Lpare.Count; i++)
                {
                    bool logflg = true;
                    Data.ProgressPerformStep();

                    for (int s = 0; s < stb.StbModel.StbStories.Count; ++s)
                    {
                        var story = stb.StbModel.StbStories[s];

                        if (Lpare[i].stbStrory == story.name)
                        {
                            switch (Lpare[i].RevitLevel)
                            {
                                case "レベルを新規生成":
                                    //レベルの生成
                                    ElementCategoryFilter LVfilter1 = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
                                    FilteredElementCollector LVcollector1 = new FilteredElementCollector(Commons.doc);
                                    IList<Element> LVlist1 = LVcollector1.WherePasses(LVfilter1).WhereElementIsNotElementType().ToElements();
                                    if (LVlist1 != null && LVlist1.Count() != 0)
                                    {
                                        foreach (Element el in LVlist1)
                                        {
                                            Level lv = el as Level;
                                            if (lv.Name == story.name)
                                            {
                                                lv.Name = lv.Name + "_";
                                                break;
                                            }
                                        }
                                    }

                                    Level newlev = Level.Create(Commons.doc, Commons.mm2ft(story.height + Lpare[i].offset));
                                    newlev.Name = story.name;
                                    Levels.Add(newlev);

                                    Data.SaveGuid(story.guid, newlev.Id);

                                    //平面図の生成
                                    ViewPlan newvp = ViewPlan.Create(Commons.doc, viewFamilyTypes.First().Id, newlev.Id);

                                    ps = newlev.Elevation * XYZ.BasisZ;
                                    pe = null;
                                    stage = "レベルの新規生成：";
                                    levelname = newlev.Name;
                                    if (Lpare[i].offset != 0)
                                    {
                                        Data.OffsetZ newz = new Data.OffsetZ()
                                        {
                                            lev = newlev,
                                            offset = Lpare[i].offset,
                                            stbid = s
                                        };
                                        alloffsetZ.Add(newz);
                                    }
                                    break;

                                case "レベルを生成しない":
                                    logflg = false;
                                    break;

                                default:
                                    for (int r = 0; r < RevitLNK.LoFa.LevelNameList.Count(); r++)
                                    {
                                        LoadFamily.LevelList loll = RevitLNK.LoFa.LevelNameList[r];
                                        if (Lpare[i].RevitLevel == loll.name)
                                        {
                                            loll.elevation = Commons.mm2ft(story.height + Lpare[i].offset);

                                            Level lv = Commons.doc.GetElement(loll.id) as Level;

                                            lv.Elevation = loll.elevation;
                                            Levels.Add(lv);

                                            Data.SaveGuid(story.guid, lv.Id);

                                            ps = lv.Elevation * XYZ.BasisZ;
                                            pe = null;
                                            stage = "レベルの生成：";
                                            levelname = story.name + "→" + lv.Name;
                                            if (Lpare[i].offset != 0)
                                            {
                                                Data.OffsetZ newz = new Data.OffsetZ()
                                                {
                                                    lev = lv,
                                                    offset = Lpare[i].offset,
                                                    stbid = s
                                                };
                                                alloffsetZ.Add(newz);
                                            }


                                            bool vplanaddflg = true;
                                            for (int vp = 0; vp < RevitLNK.LoFa.VPlan.Count; vp++)
                                            {
                                                if (RevitLNK.LoFa.VPlan[vp].Name == loll.name)
                                                {
                                                    vplanaddflg = false;
                                                    break;
                                                }
                                            }
                                            if (vplanaddflg)
                                            {
                                                //平面図の生成
                                                ViewPlan newvp2 = ViewPlan.Create(Commons.doc, viewFamilyTypes.First().Id, loll.id);
                                            }
                                            break;
                                        }
                                    }
                                    break;
                            }
                            //変換情報ログの出力
                            if (logflg)
                            {
                                Data.MakeGridLog(stage, levelname, ps, pe, 0);
                            }
                            break;
                        }
                    }
                }


                Commons.doc.Regenerate();
                tran.Commit();
            }
            catch //(Exception e)
            {
                tran.RollBack();
            }

            Data.ProgressClose();

        }

        /// <summary>
        /// 軸の生成(条件設定) 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="radb1">マッピング指定ならtrue, オフセット指定ならfalse</param>
        /// <param name="XPare">X軸オフセットの情報</param>
        /// <param name="YPare">Y軸オフセットの情報</param>
        internal static void Kiten_Convert(ST_BRIDGE stb, bool radb1, RevitLNK.AxisPare XPare, RevitLNK.AxisPare YPare)
        {
            try
            {
                //移動量をプロジェクト情報に保持するため、先に計算しておく方法に変更

                //if (radb1)
                //{
                //    //マッピング指定
                //    double kitenX = 0, kitenY = 0;
                //    double revitX = 0, revitY = 0;
                //    for (int i = 0; i < RevitLNK.LoFa.GridX.Count(); i++)
                //    {
                //        if (RevitLNK.LoFa.GridX[i].Name == XPare.RevitGrid)
                //        {
                //            revitX = Commons.ft2mm(RevitLNK.LoFa.GridX[i].Curve.GetEndPoint(0).X);
                //            break;
                //        }
                //    }
                //    for (int i = 0; i < RevitLNK.LoFa.GridY.Count(); i++)
                //    {
                //        if (RevitLNK.LoFa.GridY[i].Name == YPare.RevitGrid)
                //        {
                //            revitY = Commons.ft2mm(RevitLNK.LoFa.GridY[i].Curve.GetEndPoint(0).Y);
                //            break;
                //        }
                //    }

                //    foreach (var axisGroup in stb.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("X")))
                //    {
                //        var axis = axisGroup.StbParallelAxis.Find(a => a.name == XPare.stbAxis);
                //        if (axis != null)
                //        {
                //            kitenX = revitX - axis.distance;
                //            break;
                //        }
                //    }
                //    foreach (var axisGroup in stb.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("Y")))
                //    {
                //        var axis = axisGroup.StbParallelAxis.Find(a => a.name == YPare.stbAxis);
                //        if (axis != null)
                //        {
                //            kitenY = revitY - axis.distance;
                //            break;
                //        }
                //    }

                //    if (!Kiten_Convert_XY(stb, XPare.offset, YPare.offset, kitenX, kitenY))
                //    {
                //        LogData.AddLog(LogData.LogKind.Error, 0, "軸の生成");
                //    }
                //}
                //else
                {
                    Kiten_Convert_XY(stb, XPare.offset, YPare.offset, 0, 0);
                }

            }
            catch //(Exception e)
            {
            }

        }

        /// <summary>
        /// 軸の生成(実際の処理)
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="offsetX">X軸オフセット</param>
        /// <param name="offsetY">Y軸オフセット</param>
        /// <param name="kitenX">マッピング指定時のRevit軸基点 X座標</param>
        /// <param name="kitenY">マッピング指定時のRevit軸基点 Y座標</param>
        private static bool Kiten_Convert_XY(ST_BRIDGE stb, double offsetX, double offsetY, double kitenX, double kitenY)
        {
            bool ret = true;
            double entyou = Commons.mm2ft(3000); //グリッドを延長する（始点側)
            string logname = ""; //ログ出力用

            //建物全体の移動距離
            alloffsetX = offsetX + kitenX;
            alloffsetY = offsetY + kitenY;
            XYZ offset_mm = new XYZ(alloffsetX, alloffsetY, 0);
            XYZ offset_ft = Commons.mm2ft(offset_mm);


            //グリッドのタイプの設定
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filtergt = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            IList<Element> elems = collector.WherePasses(filtergt).WhereElementIsElementType().ToElements();
            GridType gt = null;
            if (elems != null && elems.Count() != 0)
            {
                foreach (Element el in elems)
                {
                    gt = el as GridType;
                    if (gt.Name == "通り心記号_始端")
                    { break; }
                }
            }


            var stbaxes = stb.StbModel.StbAxes;

            //通り芯の名前が重複しないようにプロジェクト内の軸名をリストにしておく
            List<string> name = new List<string>();
            for (int i = 0; i < RevitLNK.LoFa.GridX.Count(); i++)
            {
                name.Add(RevitLNK.LoFa.GridX[i].Name);
            }
            for (int i = 0; i < RevitLNK.LoFa.GridY.Count(); i++)
            {
                name.Add(RevitLNK.LoFa.GridY[i].Name);
            }


            Transaction tran = new Transaction(Commons.doc, "通り芯の生成");
            tran.Start();
            try
            {
                int count = stbaxes.StbParallelAxes?.Sum(a => a.StbParallelAxis.Count) ?? 0;
                count += stbaxes.StbArcAxes?.Sum(a => a.StbArcAxis.Count) ?? 0;
                count += stbaxes.StbRadialAxes?.Sum(a => a.StbRadialAxis.Count) ?? 0;

                Data.ProgressStart("軸の生成", count);

                //平行軸
                if (stbaxes.StbParallelAxes != null)
                {
                    //作図面
                    XYZ normal2 = new XYZ(0, 0, 1);
                    Plane p = Plane.CreateByNormalAndOrigin(normal2, new XYZ(0, 0, 0));
                    SketchPlane skp = SketchPlane.Create(Commons.doc, p);


                    var axisX = stb.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("X")).ToList();
                    var axisY = stb.StbModel.StbAxes.StbParallelAxes.Where(a => a.group_name.StartsWith("Y")).ToList();

                    foreach (var axisGroup in stbaxes.StbParallelAxes)
                    {
                        logname = $"平行軸({axisGroup.group_name})";

                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X, axisGroup.Y, 0)) + offset_ft;
                        double rad = axisGroup.angle * Math.PI / 180;
                        XYZ vec1 = new XYZ(Math.Cos(rad), Math.Sin(rad), 0);
                        XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();

                        //直交軸
                        var orthogonal = stb.StbModel.StbAxes.StbParallelAxes.Find(a => Math.Abs(Math.Abs(a.angle - axisGroup.angle) -  90) < 0.01 ||
                                                                                        Math.Abs(Math.Abs(a.angle - axisGroup.angle) - 270) < 0.01);
                        XYZ orthogonal_origin = null;
                        XYZ orthogonal_vec1 = null;
                        XYZ orthogonal_vec2 = null;
                        if (orthogonal != null)
                        {
                            //直行軸が存在していれば、始点に近い軸位置に始点を移動する
                            double orthogonal_rad = orthogonal.angle * Math.PI / 180;
                            orthogonal_origin = Commons.mm2ft(new XYZ(orthogonal.X, orthogonal.Y, 0)) + offset_ft;
                            orthogonal_vec1 = new XYZ(Math.Cos(orthogonal_rad), Math.Sin(orthogonal_rad), 0);
                            orthogonal_vec2 = XYZ.BasisZ.CrossProduct(orthogonal_vec1).Normalize();
                        }


                        foreach (var axis in axisGroup.StbParallelAxis)
                        {
                            Data.ProgressPerformStep();

                            ////そもそも節点リストが無い→軸を生成しない
                            //if (axis.StbNodeIdList.Count == 0) { continue; }

                            Create_ParallelAxis(stb, entyou, logname, gt, name, skp, origin, vec1, vec2, orthogonal, orthogonal_origin, orthogonal_vec2, axis);
                        }
                    }
                }

                //円弧軸
                if (stbaxes.StbArcAxes != null)
                {
                    foreach (var axisGroup in stbaxes.StbArcAxes)
                    {
                        logname = $"円弧軸({axisGroup.group_name})";

                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X, axisGroup.Y, 0)) + offset_ft;

                        //STBの角度は時計回りで指定されている。RevitでArcを作るときはSTBの終了角度→開始角度の方向（反時計回りで角度を指定）
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

                            Create_ArclAxis(logname, name, origin, rad_s, rad_e, axis);
                        }
                    }
                }

                //放射軸
                if (stbaxes.StbRadialAxes != null)
                {
                    double length1 = entyou;
                    double length2 = Commons.mm2ft(10000);
                    if (stbaxes.StbArcAxes != null && stbaxes.StbArcAxes.Count > 0)
                    {
                        //円弧軸がある場合は、放射軸長さを円弧軸から決める
                        length1 = Commons.mm2ft(stbaxes.StbArcAxes.Min(a => a.StbArcAxis.Min(b => b.radius))) - entyou;
                        length2 = Commons.mm2ft(stbaxes.StbArcAxes.Max(a => a.StbArcAxis.Max(b => b.radius))) + entyou;
                    }

                    foreach (var axisGroup in stbaxes.StbRadialAxes)
                    {
                        logname = $"放射軸({axisGroup.group_name})";

                        XYZ origin = Commons.mm2ft(new XYZ(axisGroup.X, axisGroup.Y, 0)) + offset_ft;

                        foreach (var axis in axisGroup.StbRadialAxis)
                        {
                            Data.ProgressPerformStep();

                            Create_RadialAxis(logname, name, length1, length2, origin, axis);
                        }
                    }
                }

                Commons.doc.Regenerate();
                tran.Commit();
            }
            catch (Exception)
            {
                ret = false;
                tran.RollBack();
                LogData.AddLog(LogData.LogKind.Error, 0, logname);
            }

            Data.ProgressClose();



            //作業用軸
            if (stbaxes.StbDrawingAxes != null)
            {
                int count = stbaxes.StbDrawingAxes.StbDrawingLineAxis?.Count ?? 0;
                count += stbaxes.StbDrawingAxes.StbDrawingArcAxis?.Count ?? 0;

                if (count > 0)
                {
                    Data.ProgressStart("作図用軸の生成", count);


                    tran.SetName("作図用軸の生成");
                    tran.Start();
                    try
                    {
                        if (stbaxes.StbDrawingAxes.StbDrawingLineAxis.Count > 0)
                        {
                            logname = "作図用直線軸";

                            foreach (var axis in stbaxes.StbDrawingAxes.StbDrawingLineAxis)
                            {
                                Data.ProgressPerformStep();

                                XYZ p1 = Commons.mm2ft(new XYZ(axis.start_X, axis.start_Y, 0)) + offset_ft;
                                XYZ p2 = Commons.mm2ft(new XYZ(axis.end_X, axis.end_Y, 0)) + offset_ft;

                                Line curve = Line.CreateBound(p2, p1);
                                Grid grid = Grid.Create(Commons.doc, curve);


                                //通り芯の名前の重複チェック
                                if (Data.Name_Check(name, axis.name))
                                {
                                    string rename = axis.name;
                                    int ascii = 97;
                                    do
                                    {
                                        rename += "_" + (char)ascii;
                                        ascii++;
                                    } while (Data.Name_Check(name, rename));
                                    axis.name = rename;
                                }
                                grid.Name = axis.name;
                                name.Add(grid.Name);

                                //通り芯生成ログ
                                Data.MakeGridLog($"{logname}の生成", grid.Name, curve.GetEndPoint(0), curve.GetEndPoint(1), 1);
                            }
                        }

                        if (stbaxes.StbDrawingAxes.StbDrawingArcAxis.Count > 0)
                        {
                            logname = "作図用円弧軸";

                            foreach (var axis in stbaxes.StbDrawingAxes.StbDrawingArcAxis)
                            {
                                Data.ProgressPerformStep();

                                XYZ origin = Commons.mm2ft(new XYZ(axis.X, axis.Y, 0)) + offset_ft;
                                double rad_s = axis.start_angle * Math.PI / 180;
                                double rad_e = axis.end_angle * Math.PI / 180;
                                double radius = Commons.mm2ft(axis.radius);
                                Arc curve = Arc.Create(origin, radius, Math.Min(rad_s, rad_e), Math.Max(rad_s, rad_e), XYZ.BasisX, XYZ.BasisY);
                                Grid grid = Grid.Create(Commons.doc, curve);


                                //通り芯の名前の重複チェック
                                if (Data.Name_Check(name, axis.name))
                                {
                                    string rename = axis.name;
                                    int ascii = 97;
                                    do
                                    {
                                        rename += "_" + (char)ascii;
                                        ascii++;
                                    } while (Data.Name_Check(name, rename));
                                    axis.name = rename;
                                }
                                grid.Name = axis.name;
                                name.Add(grid.Name);

                                //通り芯生成ログ
                                Data.MakeGridLog($"{logname}の生成", grid.Name, curve.GetEndPoint(0), curve.GetEndPoint(1), 1);
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        ret = false;
                        tran.RollBack();
                        LogData.AddLog(LogData.LogKind.Error, 0, logname);
                    }

                    Data.ProgressClose();
                }
            }


            return ret;
        }

        /// <summary>
        /// 放射軸の作成
        /// </summary>
        /// <param name="logname"></param>
        /// <param name="name"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="origin"></param>
        /// <param name="axis"></param>
        private static void Create_RadialAxis(string logname, List<string> name, double length1, double length2, XYZ origin, StbRadialAxis axis)
        {
            double rad = axis.angle * Math.PI / 180;
            XYZ vec1 = new XYZ(Math.Cos(rad), Math.Sin(rad), 0);
            XYZ p1 = origin + vec1 * length1;
            XYZ p2 = origin + vec1 * length2;

            Line curve = Line.CreateBound(p2, p1);
            Grid grid = Grid.Create(Commons.doc, curve);


            //通り芯の名前の重複チェック
            if (Data.Name_Check(name, axis.name))
            {
                string rename = axis.name;
                int ascii = 97;
                do
                {
                    rename += "_" + (char)ascii;
                    ascii++;

                } while (Data.Name_Check(name, rename));
                axis.name = rename;
            }
            grid.Name = axis.name;
            name.Add(grid.Name);

            //通り芯生成ログ
            Data.MakeGridLog($"{logname}の生成", grid.Name, curve.GetEndPoint(0), curve.GetEndPoint(1), 1);

            Data.SaveGuid(axis.guid, grid.Id);
        }

        /// <summary>
        /// 円弧軸の作成
        /// </summary>
        /// <param name="logname"></param>
        /// <param name="name"></param>
        /// <param name="origin"></param>
        /// <param name="rad_s"></param>
        /// <param name="rad_e"></param>
        /// <param name="axis"></param>
        private static void Create_ArclAxis(string logname, List<string> name, XYZ origin, double rad_s, double rad_e, StbArcAxis axis)
        {
            double radius = Commons.mm2ft(axis.radius);
            Arc curve = Arc.Create(origin, radius, rad_e, rad_s, XYZ.BasisX, XYZ.BasisY);
            Grid grid = Grid.Create(Commons.doc, curve);


            //通り芯の名前の重複チェック
            if (Data.Name_Check(name, axis.name))
            {
                string rename = axis.name;
                int ascii = 97;
                do
                {
                    rename += "_" + (char)ascii;
                    ascii++;

                } while (Data.Name_Check(name, rename));
                axis.name = rename;
            }
            grid.Name = axis.name;
            name.Add(grid.Name);

            //通り芯生成ログ
            Data.MakeGridLog($"{logname}の生成", grid.Name, curve.GetEndPoint(0), curve.GetEndPoint(1), 1);

            Data.SaveGuid(axis.guid, grid.Id);
        }

        /// <summary>
        /// 平行軸の作成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="entyou"></param>
        /// <param name="logname"></param>
        /// <param name="gt"></param>
        /// <param name="name"></param>
        /// <param name="skp"></param>
        /// <param name="origin"></param>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <param name="orthogonal"></param>
        /// <param name="orthogonal_origin"></param>
        /// <param name="orthogonal_vec2"></param>
        /// <param name="axis"></param>
        private static void Create_ParallelAxis(ST_BRIDGE stb, double entyou, string logname, GridType gt, List<string> name, SketchPlane skp, XYZ origin, XYZ vec1, XYZ vec2, StbParallelAxes orthogonal, XYZ orthogonal_origin, XYZ orthogonal_vec2, StbParallelAxis axis)
        {
            List<XYZ> points = new List<XYZ>();

            //節点リストを整理する
            List<StbNodeId> newL;
            if (Commons.GridMode == 0 && axis.StbNodeIdList.Count > 0)
            {
                newL = Narabekae_Node(stb, axis.StbNodeIdList, vec1);
            }
            else
            {
                //基準距離
                newL = new List<StbNodeId>();
            }

            //節点リスト内に該当する節点が無いとき→代表距離をもとに軸を生成する
            if (newL.Count == 0)
            {
                XYZ p = origin + vec2 * Commons.mm2ft(axis.distance);

                if (orthogonal != null)
                {
                    //直行軸が存在していれば、始点に近い軸位置に始点を移動する
                    List<XYZ> orthogonal_point = orthogonal.StbParallelAxis.Select(a => orthogonal_origin + orthogonal_vec2 * Commons.mm2ft(a.distance)).ToList();

                    double distance1 = orthogonal_point.Min(a => vec1.DotProduct(a - p));
                    double distance2 = orthogonal_point.Max(a => vec1.DotProduct(a - p));
                    points.Add(p + (distance1 - entyou) * vec1);
                    points.Add(p + (distance2 + entyou) * vec1);
                }
                else
                {
                    points.Add(p - entyou * vec1);
                    points.Add(p + entyou * vec1);
                }
            }
            else
            {
                for (int j = -1; j < newL.Count() - 1; j++)
                {
                    XYZ start = new XYZ();
                    XYZ end = new XYZ();
                    if (j == -1)
                    {
                        //最初の時だけ延長
                        end = Get_Node_Position(stb.StbModel.StbNodes, newL[0].id, alloffsetX, alloffsetY);
                        start = end;

                        if (orthogonal != null)
                        {
                            //直行軸が存在していれば、始点に近い軸位置に始点を移動する
                            List<XYZ> orthogonal_point = orthogonal.StbParallelAxis.Select(a => orthogonal_origin + orthogonal_vec2 * Commons.mm2ft(a.distance)).ToList();
                            double distance = orthogonal_point.Min(a => vec1.DotProduct(a - start));
                            start = start + distance * vec1;
                        }

                        //延長
                        start = start - entyou * vec1;
                        points.Add(start);
                    }
                    else
                    {
                        start = points.Last();
                        end = Get_Node_Position(stb.StbModel.StbNodes, newL[j + 1].id, alloffsetX, alloffsetY);
                    }

                    //始点と終点の距離が短い→次の点へ
                    if (start.DistanceTo(end) <= Commons.doc.Application.ShortCurveTolerance)
                    {
                        continue;
                    }

                    points.Add(end);
                }


                //通り芯の最後を延長
                XYZ end2 = points.Last();
                if (orthogonal != null)
                {
                    //直行軸が存在していれば、始点に近い軸位置に始点を移動する
                    List<XYZ> orthogonal_point = orthogonal.StbParallelAxis.Select(a => orthogonal_origin + orthogonal_vec2 * Commons.mm2ft(a.distance)).ToList();
                    double distance = orthogonal_point.Min(a => -vec1.DotProduct(a - end2));
                    XYZ end3 = end2 - distance * vec1;
                    if ((end3 - end2).Normalize().DotProduct(vec1) > 0)
                    {
                        //進行方向なら置き換える
                        end2 = end3;
                    }
                }

                //延長
                end2 = end2 + entyou * vec1;
                points.Add(end2);
            }



            XYZ gridVec = (points.Last() - points.First());
            double angle = XYZ.BasisX.AngleOnPlaneTo(gridVec, XYZ.BasisZ);
            if (Math.PI * 5 / 4 <= angle && angle <= Math.PI * 7 / 4)
            {
                //225-315ならひっくり返す
                points.Reverse();
            }

            List<Curve> cur = new List<Curve>();
            for (int i = 0; i < points.Count - 1; ++i)
            {
                cur.Add(Line.CreateBound(points[i], points[i + 1]));
            }


            CurveLoop cloop = CurveLoop.Create(cur);

            //複数セグメントの通り芯
            MultiSegmentGrid mgr = (MultiSegmentGrid)(Commons.doc.GetElement(MultiSegmentGrid.Create(Commons.doc, gt.Id, cloop, skp.Id)));


            //通り芯の名前の重複チェック
            if (Data.Name_Check(name, axis.name))
            {
                string rename = axis.name;
                int ascii = 97;
                do
                {
                    rename += "_" + (char)ascii;
                    ascii++;

                } while (Data.Name_Check(name, rename));
                axis.name = rename;
            }
            mgr.Name = axis.name;
            name.Add(mgr.Name);

            //通り芯生成ログ
            Data.MakeGridLog($"{logname}の生成", mgr.Name, cur[0].GetEndPoint(0), cur[cur.Count - 1].GetEndPoint(1), 1);

            Data.SaveGuid(axis.guid, mgr.Id);
        }








        /// <summary>
        /// 部材の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="chb">変換要否の情報</param>
        internal static void CreateBuzai(ST_BRIDGE stb, List<ConvertForm.Chb_class> chb)
        {
            string errmsg = "";


            //各部材ファミリを取得
            //梁
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilySymbol> girders = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            //柱
            collector = new FilteredElementCollector(Commons.doc);
            filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilySymbol> columns = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();


            TransactionGroup trang = new TransactionGroup(Commons.doc, "変換開始");
            trang.Start();
            try
            {
                for (int i = 0; i < chb.Count(); i++)
                {
                    errmsg = "";

                    if (!chb[i].chbchecked) { continue; }
                    switch (chb[i].buzai)
                    {
                        case "柱":
                        case "間柱":
                            if (!CreateColumn(stb, chb[i].buzai, columns, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "基礎柱":
                            if (!CreateFoundationColumn(stb, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "大梁":
                        case "小梁":
                        case "片持梁":
                        case "片持小梁":
                            if (!CreateGirder(stb, chb[i].buzai, girders, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "RCスラブ":
                        case "デッキプレート":
                        case "既製スラブ":
                        case "基礎スラブ":
                            if (!CreateSlab(stb, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "Sブレース":
                            if (!CreateBrace(stb, chb[i].buzai, girders, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "壁":
                        case "RCパラペット":
                            if (!CreateWall(stb, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "基礎・布基礎・杭":
                            if (!CreateFoundation(stb, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;

                        case "柱脚":
                            if (!CreateClmBase(stb, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                    }
                }



                //結合順序の入れ替え
                ChangeOrder();


                //基礎のグループ化
                Transaction tran4 = new Transaction(Commons.doc, "基礎のグループ化");
                try
                {
                    tran4.Start();

                    //基礎梁と布基礎のグループ化
                    errmsg = "梁と布基礎のグループ化";
                    for (int i = 0; i < CGrp.Count; i++)
                    {
                        if (CGrp[i].elId.Count > 1)
                        {
                            Commons.doc.Create.NewGroup(CGrp[i].elId);
                            //Commons.doc.Regenerate();
                        }
                    }

                    //基礎と杭のグループ化（グループ化前にRegenerateが必要）
                    errmsg = "基礎と杭のグループ化";
                    foreach (var k in FGroup.Keys)
                    {
                        if (FGroup[k].Count > 1)
                        {
                            Commons.doc.Create.NewGroup(FGroup[k]);
                        }
                    }
                    tran4.Commit();
                }
                catch (Exception)
                {
                    tran4.RollBack();
                }

            }
            catch (Exception)
            {
                //ログ出力
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }

            trang.Assimilate();


        }



        #region 各部材の生成


        #region 柱

        /// <summary>
        /// 柱の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="syubetu">"柱", "間柱"</param>
        /// <param name="symbols"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateColumn(ST_BRIDGE stb, string syubetu, List<FamilySymbol> symbols, ref string errmsg)
        {
            bool ret = true;

            if (symbols == null || symbols.Count == 0)
            {
                //ファミリが無いログ
                return ret;
            }


            StbSecColumn_Kind_column kind = StbSecColumn_Kind_column.COLUMN;
            switch (syubetu)
            {
                case "柱":
                    kind = StbSecColumn_Kind_column.COLUMN;
                    break;
                case "間柱":
                    kind = StbSecColumn_Kind_column.POST;
                    break;
            }

            //パラメータの追加
            if (kind == StbSecColumn_Kind_column.COLUMN)
            {
                Clm_Parameter_Set(SetFamily.ClmFName, symbols, syubetu);
            }
            else
            {
                Clm_Parameter_Set(SetFamily.PClmFName, symbols, syubetu);
            }


            //変換ファミリの取得
            Family[][] ConvFamily = new Family[RevitLNK.ClmText.Length][];
            for (int i = 0; i < RevitLNK.ClmText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.ClmText[i].Length);
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



            //柱タイプパラメータの設定
            Transaction tran = new Transaction(Commons.doc, syubetu + "タイプパラメータの生成");
            try
            {
                tran.Start();

                //RC柱
                if (stb.StbModel.StbSections.StbSecColumn_RC != null)
                {
                    //Clm[0][0](矩形),Clm[0][1](円形)

                    var cols = stb.StbModel.StbSections.StbSecColumn_RC.Where(a => a.kind_column == kind).ToList();
                    if (cols.Count > 0)
                    {
                        Data.ProgressRestart($"RC{syubetu}の生成", cols.Count);

                        foreach (var clm in cols)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateColumn_RC(stb, clm, ConvFamily)) { ret = false; errmsg = "RC柱"; }
                        }
                    }
                }

                //S柱
                if (stb.StbModel.StbSections.StbSecColumn_S != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_S.Where(a => a.kind_column == kind).ToList();
                    if (cols.Count > 0)
                    {
                        Data.ProgressRestart($"S{syubetu}の生成", cols.Count);

                        foreach (var clm in cols)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateColumn_S(stb, clm, ConvFamily)) { ret = false; errmsg = "S柱"; }
                        }
                    }
                }

                //SRC柱
                if (stb.StbModel.StbSections.StbSecColumn_SRC != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_SRC.Where(a => a.kind_column == kind).ToList();
                    if (cols.Count > 0)
                    {
                        Data.ProgressRestart($"SRC{syubetu}の生成", cols.Count);

                        foreach (var clm in cols)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateColumn_SRC(stb, clm, ConvFamily)) { ret = false; errmsg = "SRC柱"; }
                        }
                    }
                }

                //CFT柱
                if (stb.StbModel.StbSections.StbSecColumn_CFT != null)
                {
                    var cols = stb.StbModel.StbSections.StbSecColumn_CFT.Where(a => a.kind_column == kind).ToList();
                    if (cols.Count > 0)
                    {
                        Data.ProgressRestart($"CFT{syubetu}の生成", cols.Count);

                        foreach (var clm in cols)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateColumn_CFT(stb, clm, ConvFamily)) { ret = false; errmsg = "CFT柱"; }
                        }
                    }
                }


                Data.ProgressClose();

                Commons.doc.Regenerate();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.RollBack();
                ret = false;
            }

            Data.ProgressClose();



            //柱インスタンスパラメータの設定
            tran.SetName("インスタンスパラメータの生成");
            try
            {
                tran.Start();
                switch (kind)
                {
                    case StbSecColumn_Kind_column.COLUMN:
                        if (stb.StbModel.StbMembers.StbColumns != null && stb.StbModel.StbMembers.StbColumns.Count > 0)
                        {
                            Data.ProgressStart("柱の生成", stb.StbModel.StbMembers.StbColumns.Count);

                            foreach (var clm in stb.StbModel.StbMembers.StbColumns)
                            {
                                Data.ProgressPerformStep();

                                if (!CreateColumn_instance(stb, clm, ConvFamily)) { ret = false; errmsg = "柱インスタンス"; }
                            }

                            Data.ProgressClose();
                        }
                        break;

                    case StbSecColumn_Kind_column.POST:
                        if (stb.StbModel.StbMembers.StbPosts != null && stb.StbModel.StbMembers.StbPosts.Count > 0)
                        {
                            Data.ProgressStart("間柱の生成", stb.StbModel.StbMembers.StbPosts.Count);

                            foreach (var clm in stb.StbModel.StbMembers.StbPosts)
                            {
                                Data.ProgressPerformStep();

                                if (!CreateColumn_instance(stb, clm, ConvFamily)) { ret = false; errmsg = "間柱インスタンス"; }
                            }

                            Data.ProgressClose();
                        }
                        break;
                }

                Commons.doc.Regenerate();
                tran.Commit();

            }
            catch (Exception)
            {
                ret = false;
                tran.RollBack();
            }

            if (ret == false)
            {
                errmsg = syubetu;
            }

            Data.ProgressClose();


            return ret;
        }


        /// <summary>
        /// 基礎柱の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateFoundationColumn(ST_BRIDGE stb, ref string errmsg)
        {
            bool ret = true;

            //変換ファミリの取得
            Family[][] ConvFamily = new Family[RevitLNK.FClmText.Length][];
            for (int i = 0; i < RevitLNK.FClmText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.FClmText[i].Length);
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            int numfamily = 0; //変換するファミリの数


            Data.ProgressStart("基礎柱パラメータ追加", ConvFamily.Count());

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                Data.ProgressPerformStep();

                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.BClmFName.flg[i][j]) { continue; }
                    if (!SetFamily.BClmFName.convflg[i][j]) { continue; }

                    foreach (Element el in elements)
                    {
                        if (!(el is FamilySymbol familysymbol)) { continue; }

                        if (familysymbol.FamilyName == SetFamily.BClmFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {
                                Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                    FamilyManager fmg = doc.FamilyManager;

                                    switch (j)
                                    {
                                        case 0:
                                            ParaSet.SetPara_RCClmRe(fmg, SetFamily.RCClmRe);
                                            break;
                                        case 1:
                                            ParaSet.SetPara_RCClmRo(fmg, SetFamily.RCClmRo);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.BClmFName.FamilyName, familysymbol.FamilyName, i, j);

                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                            numfamily++;
                        }
                    }
                }
            }



            Transaction tran = new Transaction(Commons.doc, "基礎柱インスタンスパラメータの生成");
            try
            {
                tran.Start();
                if (stb.StbModel.StbSections.StbSecColumn_RC != null)
                {
                    List<int> ind = new List<int>();
                    ind.AddRange(stb.StbModel.StbMembers.StbFoundationColumns.Select(a => a.id_section_FD));
                    ind.AddRange(stb.StbModel.StbMembers.StbFoundationColumns.Select(a => a.id_section_WR));
                    ind.Distinct();

                    var cols = stb.StbModel.StbSections.StbSecColumn_RC.Where(a => ind.Contains(a.id)).ToList();
                    Data.ProgressRestart("基礎柱の生成", cols.Count);
                    foreach (var sec in cols)
                    {
                        Data.ProgressPerformStep();

                        if (!CreateColumn_RC(stb, sec, ConvFamily)) { ret = false; errmsg = "基礎柱タイプ"; }
                    }
                }

                if (stb.StbModel.StbMembers.StbFoundationColumns != null)
                {
                    Data.ProgressRestart("基礎柱の生成", stb.StbModel.StbMembers.StbFoundationColumns.Count);
                    foreach (var col in stb.StbModel.StbMembers.StbFoundationColumns)
                    {
                        Data.ProgressPerformStep();

                        if (!CreateFoundationColumn_instance(stb, col, ConvFamily)) { ret = false; errmsg = "基礎柱インスタンス"; }
                    }
                }

                Data.ProgressClose();


                Commons.doc.Regenerate();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.RollBack();
            }

            Data.ProgressClose();

            return ret;
        }



        /// <summary>
        /// 柱・間柱パラメータセット
        /// </summary>
        /// <param name="ClmFName"></param>
        /// <param name="elements"></param>
        private static void Clm_Parameter_Set(FamilyStructure.ClmFamilyName ClmFName, List<FamilySymbol> elements, string syubetu)
        {
            int numfamily = 0; //変換するファミリの数
            for (int i = 0; i < ClmFName.convflg.Count(); i++)
            {
                for (int j = 0; j < ClmFName.convflg[i].Count(); j++)
                {
                    if (!ClmFName.flg[i][j]) { continue; }
                    if (!ClmFName.convflg[i][j]) { continue; }
                    numfamily++;
                }
            }

            Data.ProgressStart($"{syubetu}パラメータ追加", numfamily);

            for (int i = 0; i < ClmFName.convflg.Count(); i++)
            {
                for (int j = 0; j < ClmFName.convflg[i].Count(); j++)
                {
                    if (!ClmFName.flg[i][j]) { continue; }
                    if (!ClmFName.convflg[i][j]) { continue; }

                    foreach (FamilySymbol familysymbol in elements)
                    {
                        if (familysymbol.FamilyName == ClmFName.FamilyName[i][j])
                        {
                            //プログレスバーの表示
                            Data.ProgressPerformStep();

                            Document doc = Commons.doc.EditFamily(familysymbol.Family);
                            Transaction tran1 = new Transaction(doc, ClmFName.FamilyName + "パラメータ追加");
                            try
                            {
                                tran1.Start();
                                Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
                                string famname = familysymbol.FamilyName;

                                if (famname == SetFamily.RCClmRe.FamilyName)
                                {
                                    ParaSet.SetPara_RCClmRe(fmg, SetFamily.RCClmRe);
                                }
                                if (famname == SetFamily.RCClmRo.FamilyName)
                                {
                                    ParaSet.SetPara_RCClmRo(fmg, SetFamily.RCClmRo);
                                }
                                if (famname == SetFamily.SClmH.FamilyName)
                                {
                                    ParaSet.SetPara_SClmH(fmg, SetFamily.SClmH);
                                }
                                if (famname == SetFamily.SClmBH.FamilyName)
                                {
                                    ParaSet.SetPara_SClmBH(fmg, SetFamily.SClmBH);
                                }
                                if (famname == SetFamily.SClmBox.FamilyName)
                                {
                                    ParaSet.SetPara_SClmBox(fmg, SetFamily.SClmBox);
                                }
                                if (famname == SetFamily.SClmBBox.FamilyName)
                                {
                                    ParaSet.SetPara_SClmBBox(fmg, SetFamily.SClmBBox);
                                }
                                if (famname == SetFamily.SClmPipe.FamilyName)
                                {
                                    ParaSet.SetPara_SClmPipe(fmg, SetFamily.SClmPipe);
                                }
                                if (famname == SetFamily.SClmT.FamilyName)
                                {
                                    ParaSet.SetPara_SClmT(fmg, SetFamily.SClmT);
                                }
                                if (famname == SetFamily.SClmC.FamilyName)
                                {
                                    ParaSet.SetPara_SClmC(fmg, SetFamily.SClmC);
                                }
                                if (famname == SetFamily.SClmL.FamilyName)
                                {
                                    ParaSet.SetPara_SClmL(fmg, SetFamily.SClmL);
                                }
                                if (famname == SetFamily.SRCClmH.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmH(fmg, SetFamily.SRCClmH);
                                }
                                if (famname == SetFamily.SRCClmCross.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmCross(fmg, SetFamily.SRCClmCross);
                                }
                                if (famname == SetFamily.SRCClmT.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmT(fmg, SetFamily.SRCClmT);
                                }
                                if (famname == SetFamily.SRCClmH_Rou.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmH_Rou(fmg, SetFamily.SRCClmH_Rou);
                                }
                                if (famname == SetFamily.SRCClmCross_Rou.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmCross_Rou(fmg, SetFamily.SRCClmCross_Rou);
                                }
                                if (famname == SetFamily.SRCClmT_Rou.FamilyName)
                                {
                                    ParaSet.SetPara_SRCClmT_Rou(fmg, SetFamily.SRCClmT_Rou);
                                }
                                if (famname == SetFamily.CFTClmBox.FamilyName)
                                {
                                    ParaSet.SetPara_CFTClmBox(fmg, SetFamily.CFTClmBox);
                                }
                                if (famname == SetFamily.CFTClmPipe.FamilyName)
                                {
                                    ParaSet.SetPara_CFTClmPipe(fmg, SetFamily.CFTClmPipe);
                                }


                                //プロジェクトにパラメータを追加したファミリをロードする
                                FamilyOption famop = new FamilyOption();
                                doc.LoadFamily(Commons.doc, famop);
                                tran1.Commit();
                                doc.Close(false);
                                break;
                            }
                            catch (Exception)
                            {
                                tran1.RollBack();
                                doc.Close(false);
                            }
                        }
                    }
                }
            }

            Data.ProgressClose();
        }





        /// <summary>
        /// 柱のタイプ名取得（Member.nameをそのまま使用する方式に変更）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm_id">断面id</param>
        /// <returns></returns>
        private static string GetTypeName_Column(ST_BRIDGE stb, int clm_id)
        {
            string typename = stb.StbModel.StbMembers.StbColumns.Find(a => a.id_section == clm_id)?.name;
            if (typename == null || typename == "")
            {
                typename = stb.StbModel.StbMembers.StbPosts.Find(a => a.id_section == clm_id)?.name;
            }
            if (typename == null || typename == "")
            {
                typename = stb.StbModel.StbMembers.StbFoundationColumns.Find(a => a.id_section_FD == clm_id || a.id_section_WR == clm_id)?.name;
            }

            return typename;
        }

        private static FamilySymbol GetFamilySymbol_Column(Family family, ref string typename)
        {
            FamilySymbol symbol = null;
            if (!Data.SearchFamilySymbol(family, typename, ref symbol))
            {
                symbol = (FamilySymbol)symbol.Duplicate(typename);
            }
            else
            {
                //idが異なるのに名前が同じ。枝番付ける
                typename = Data.ReName2(family, typename);
                symbol = (FamilySymbol)symbol.Duplicate(typename);
            }

            return symbol;
        }


        /// <summary>
        /// RC柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateColumn_RC(ST_BRIDGE stb, StbSecColumn_RC clm, Family[][] ConvFamily)
        {
            bool ret = true;
            string typename = GetTypeName_Column(stb, clm.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[RC柱](断面id=" + clm.id.ToString() + ")");
                return ret;
            }


            string logbuzai = "";
            if (clm.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect) { logbuzai = "RC矩形柱"; }
            else { logbuzai = "RC円柱"; }

            //鉄筋のタグが無いとき→ログ出力
            if (clm.StbSecBarArrangementColumn_RC == null)
            {
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[RC柱]" + typename + "(断面id=" + clm.id.ToString() + ")");
            }


            FamilySymbol symbol = null;
            int n = clm.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect ? 0 : 1;
            if (ConvFamily[0][n] == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, logbuzai);
                return ret;
            }
            if (columnType[0].ContainsKey(clm.id))
            {
                symbol = Commons.doc.GetElement(columnType[0][clm.id]) as FamilySymbol;
            }
            else
            {
                symbol = GetFamilySymbol_Column(ConvFamily[0][n], ref typename);
            }


            if (n == 0) //矩形
            {
                SetParameter_RCColumn_Rect(clm, symbol);
            }
            else //円形
            {
                SetParameter_RCColumn_Circle(clm, symbol);
            }

            if (symbol != null)
            {
                if (!columnType[0].ContainsKey(clm.id))
                {
                    columnType[0].Add(clm.id, symbol.Id);
                }


                Data.SaveGuid(clm.guid, symbol.Id);
            }


            return ret;
        }

        private static void SetParameter_RCColumn_Rect(StbSecColumn_RC clm, FamilySymbol symbol)
        {
            FamilyStructure.RC_Clm_Re Rclm = SetFamily.RCClmRe;

            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column.ToString());

            StbSecColumn_RC_Rect fig = (StbSecColumn_RC_Rect)clm.StbSecFigureColumn_RC.Item;
            Data.SetParameter(symbol.LookupParameter(Rclm.DX), fig.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.DY), fig.width_Y, true);

            if (clm.StbSecBarArrangementColumn_RC == null)
            {
                clm.StbSecBarArrangementColumn_RC = new StbSecBarArrangementColumn_RC();
                clm.StbSecBarArrangementColumn_RC.Items = new List<object>() { new StbSecBarColumn_RC_RectSame() };
            }

            if (clm.StbSecBarArrangementColumn_RC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_RC_RectNotSame bar = clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_RectNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_RC_RectNotSame(clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_RectSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                //STB2.0ではDRS以外もOKなのでチェックしない
                //if (bar != null)
                //{
                //    //鉄筋径のチェック
                //    bar.D_main = Data.Get_D("RC柱", bar.D_main, "主筋", typename, clm.id);
                //    bar.D_2nd_main = Data.Get_D("RC柱", bar.D_2nd_main, "副主筋", typename, clm.id);
                //    bar.D_axial = Data.Get_D("RC柱", bar.D_axial, "軸筋", typename, clm.id);
                //    bar.D_band = Data.Get_D("RC柱", bar.D_band, "帯筋", typename, clm.id);
                //    bar.D_bar_spacing = Data.Get_D("RC柱", bar.D_bar_spacing, "巾止筋", typename, clm.id);
                //}

                Data.SetParameter(symbol.LookupParameter(Rclm.center_reinforcement_start_X), clm.StbSecBarArrangementColumn_RC.center_start_X, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.center_reinforcement_start_Y), clm.StbSecBarArrangementColumn_RC.center_start_Y, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.StbSecBarArrangementColumn_RC.center_interval, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.StbSecBarArrangementColumn_RC.depth_cover_start_X, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.StbSecBarArrangementColumn_RC.depth_cover_end_X, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.StbSecBarArrangementColumn_RC.depth_cover_start_Y, true);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.StbSecBarArrangementColumn_RC.depth_cover_end_Y, true);

                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar?.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), bar?.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_axial), bar?.strength_axial);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar?.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar?.strength_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_axial), bar?.D_axial);
                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar?.D_bar_spacing);

                var barX = clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumnXReinforced>().FirstOrDefault();
                if (barX != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_total_X), barX.N_main_total);
                }


                List<StbSecBarColumn_RC_RectNotSame> bar2 = new List<StbSecBarColumn_RC_RectNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_RectNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), bar2[j].D_2nd_main);
                    if (barX != null)
                    { Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar2[j].N_main_X_1st + barX.N_main_X); }
                    else
                    { Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar2[j].N_main_X_1st); }

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar2[j].N_2nd_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar2[j].N_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar2[j].N_2nd_main_X_2nd);
                    if (barX != null)
                    { Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar2[j].N_main_Y_1st + barX.N_main_X); }
                    else
                    { Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar2[j].N_main_Y_1st); }
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar2[j].N_2nd_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar2[j].N_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar2[j].N_2nd_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar2[j].N_band_direction_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar2[j].N_band_direction_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[j]), clm.StbSecBarArrangementColumn_RC.kind_corner.ToString());
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar2[j].N_main_total);

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar2[j].N_axial);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar2[j].N_axial);

                    if (j == 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), bar2[j].pitch_bar_spacing, true);
                    }
                }

            }
        }
        private static void SetParameter_RCColumn_Circle(StbSecColumn_RC clm, FamilySymbol symbol)
        {
            FamilyStructure.RC_Clm_Ro Rclm = SetFamily.RCClmRo;

            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(Rclm.D), ((StbSecColumn_RC_Circle)clm.StbSecFigureColumn_RC.Item).D, true);


            if (clm.StbSecBarArrangementColumn_RC == null)
            {
                clm.StbSecBarArrangementColumn_RC = new StbSecBarArrangementColumn_RC();
                clm.StbSecBarArrangementColumn_RC.Items = new List<object>() { new StbSecBarColumn_RC_CircleSame() };
            }

            if (clm.StbSecBarArrangementColumn_RC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_RC_CircleNotSame bar = clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_CircleNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_RC_CircleNotSame(clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_CircleSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar?.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_axial), bar?.strength_axial);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar?.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar?.strength_bar_spacing);

                Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_axial), bar?.D_axial);
                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar?.D_bar_spacing);

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.StbSecBarArrangementColumn_RC.depth_cover_start_X);


                List<StbSecBarColumn_RC_CircleNotSame> bar2 = new List<StbSecBarColumn_RC_CircleNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_RC.Items.OfType<StbSecBarColumn_RC_CircleNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < Rclm.D_reinforcement_main.Count(); j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar2[j].N_axial);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar2[j].N_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar2[j].N_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar2[j].N_axial);

                    if (j == 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), bar2[j].pitch_bar_spacing, true);
                    }
                }
            }

        }



        /// <summary>
        /// S柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateColumn_S(ST_BRIDGE stb, StbSecColumn_S clm, Family[][] ConvFamily)
        {
            bool ret = true;
            string typename = GetTypeName_Column(stb, clm.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[S柱](断面id=" + clm.id.ToString() + ")");
                return ret;
            }


            if (clm.StbSecSteelFigureColumn_S.Items == null || clm.StbSecSteelFigureColumn_S.Items.Count == 0)
            {
                LogData.AddLog(LogData.LogKind.Warning, 3000, "[S柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")は柱鉄骨情報");
                return ret;
            }

            //鉄骨形状を取得
            var fig1 = clm.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_ThreeTypes>().OrderBy(a => a.pos).ToList();
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

            FamilySymbol symbol = null;

            string kind_column = clm.kind_column == StbSecColumn_Kind_column.COLUMN ? "Column" : "Post";

            switch (shape)
            {
                case RevitLNK.st_steel_H:
                    string shapename0 = "S柱H形鋼";
                    if (ConvFamily[1][0] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename0);
                        return ret;
                    }

                    var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeid];
                    string logtxt0 = Roll_H_Size_Check(steel);
                    if (logtxt0 != "")
                    {
                        Data.MakeSizeLog(shapename0, typename, clm.id, logtxt0, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][0], ref typename);
                    }

                    SetParameter_SColumn_H(clm, steelshape, strength_main, strength_web, symbol, kind_column, steel);
                    break;

                case RevitLNK.st_steel_BH:
                    string shapename1 = "S柱組立H形鋼";
                    if (ConvFamily[1][1] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename1);
                        return ret;
                    }

                    var steelBH = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeid];
                    string logtxt1 = Build_H_Size_Check(steelBH);
                    if (logtxt1 != "")
                    {
                        Data.MakeSizeLog(shapename1, typename, clm.id, logtxt1, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][1], ref typename);
                    }

                    SetParameter_SColumn_BH(clm, steelshape, strength_main, strength_web, symbol, kind_column, steelBH);
                    break;

                case RevitLNK.st_steel_Box:
                    string shapename2 = "S柱角形鋼";
                    if (ConvFamily[1][2] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S柱角形鋼管");
                        return ret;
                    }

                    var steelBox = stb.StbModel.StbSections.StbSecSteel.StbSecRollBox[shapeid];

                    string logtxt2 = Roll_Box_Size_Check(steelBox);
                    if (logtxt2 != "")
                    {
                        Data.MakeSizeLog(shapename2, typename, clm.id, logtxt2, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][2], ref typename);
                    }

                    SetParameter_SColumn_Box(clm, steelshape, strength_main, symbol, kind_column, steelBox);
                    break;

                case RevitLNK.st_steel_BBox:
                    string shapename3 = "S柱組立角形鋼管";
                    if (ConvFamily[1][3] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename3);
                        return ret;
                    }

                    var steelBBox = stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox[shapeid];

                    string logtxt3 = Build_Box_Size_Check(steelBBox);
                    if (logtxt3 != "")
                    {
                        Data.MakeSizeLog(shapename3, typename, clm.id, logtxt3, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][3], ref typename);
                    }

                    SetParameter_SColumn_BBox(clm, steelshape, strength_main, symbol, kind_column, steelBBox);
                    break;

                case RevitLNK.st_steel_Pipe:
                    string shapename4 = "S柱円形鋼管";
                    if (ConvFamily[1][4] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename4);
                        return ret;
                    }

                    var steelP = stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];

                    string logtxt4 = Pipe_Size_Check(steelP);
                    if (logtxt4 != "")
                    {
                        Data.MakeSizeLog(shapename4, typename, clm.id, logtxt4, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][4], ref typename);
                    }

                    SetParameter_SColumn_Pipe(clm, steelshape, strength_main, symbol, kind_column, steelP);
                    break;

                case RevitLNK.st_steel_T:
                    string shapename5 = "S柱T形鋼";
                    if (ConvFamily[1][5] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename5);
                        return ret;
                    }

                    var steelT = stb.StbModel.StbSections.StbSecSteel.StbSecRollT[shapeid];

                    string logtxt5 = Roll_T_Size_Check(steelT);
                    if (logtxt5 != "")
                    {
                        Data.MakeSizeLog(shapename5, typename, clm.id, logtxt5, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][5], ref typename);
                    }

                    SetParameter_SColumn_T(clm, steelshape, strength_main, strength_web, symbol, kind_column, steelT);
                    break;

                case RevitLNK.st_steel_C:
                    string shapename6 = "S柱溝形鋼";
                    if (ConvFamily[1][6] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename6);
                        return ret;
                    }

                    var steelC = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeid];

                    string logtxt6 = Roll_C_Size_Check(steelC);
                    if (logtxt6 != "")
                    {
                        Data.MakeSizeLog(shapename6, typename, clm.id, logtxt6, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][6], ref typename);
                    }

                    SetParameter_SColumn_C(clm, steelshape, strength_main, symbol, kind_column, steelC);
                    break;

                case RevitLNK.st_steel_L:
                    string shapename7 = "S柱山形鋼";
                    if (ConvFamily[1][7] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename7);
                        return ret;
                    }

                    var steelL = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeid];

                    string logtxt7 = Roll_L_Size_Check(steelL);
                    if (logtxt7 != "")
                    {
                        Data.MakeSizeLog(shapename7, typename, clm.id, logtxt7, 0);
                        return ret;
                    }

                    if (columnType[1].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[1][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[1][7], ref typename);
                    }

                    SetParameter_SColumn_L(clm, steelshape, strength_main, symbol, kind_column, steelL);
                    break;

                default:
                    if (shape == "")
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 2500, "[S柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + steelshape + "]");
                        return ret;
                    }

                    string shapename = "";
                    switch (shape)
                    {
                        case RevitLNK.st_steel_LipC:
                            shapename = "リップ溝形鋼";
                            break;
                        case RevitLNK.st_steel_FB:
                            shapename = "フラットバー";
                            break;
                        case RevitLNK.st_steel_Bar:
                            shapename = "丸鋼";
                            break;
                    }

                    //ログ表示(変換対象外)
                    Data.Make_taisyougaiLog("[S柱]", clm.id, typename, shape, shapename);
                    break;
            }


            if (symbol != null)
            {
                if (!columnType[1].ContainsKey(clm.id))
                {
                    columnType[1].Add(clm.id, symbol.Id);
                }

                Data.SaveGuid(clm.guid, symbol.Id);
            }


            return ret;
        }

        private static void SetParameter_SColumn_L(StbSecColumn_S clm, string steelshape, string strength_main, FamilySymbol symbol, string kind_column, StbSecRollL steelL)
        {
            FamilyStructure.S_Clm_L RclmL = SetFamily.SClmL;
            Data.SetParameter(symbol.LookupParameter(RclmL.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmL.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmL.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmL.B), steelL.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.A), steelL.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.t1), steelL.t1, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.t2), steelL.t2, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.r1), steelL.r1, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmL.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmL.r2), steelL.r2, true);
            Data.SetParameter(symbol.LookupParameter(RclmL.side), steelL.type == StbSecRollLType.BACKTOBACK);
            Data.SetParameter(symbol.LookupParameter(RclmL.type), steelL.type.ToString());
            Data.SetParameter(symbol.LookupParameter(RclmL.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmL.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_C(StbSecColumn_S clm, string steelshape, string strength_main, FamilySymbol symbol, string kind_column, StbSecRollC steelC)
        {
            FamilyStructure.S_Clm_C RclmC = SetFamily.SClmC;
            Data.SetParameter(symbol.LookupParameter(RclmC.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmC.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmC.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmC.B), steelC.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.A), steelC.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.t1), steelC.t1, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.t2), steelC.t2, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.r1), steelC.r1, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmC.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmC.r2), steelC.r2, true);
            Data.SetParameter(symbol.LookupParameter(RclmC.side), steelC.type == StbSecRollCType.BACKTOBACK);
            Data.SetParameter(symbol.LookupParameter(RclmC.type), steelC.type.ToString());
            Data.SetParameter(symbol.LookupParameter(RclmC.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmC.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_T(StbSecColumn_S clm, string steelshape, string strength_main, string strength_web, FamilySymbol symbol, string kind_column, StbSecRollT steelT)
        {
            FamilyStructure.S_Clm_T RclmT = SetFamily.SClmT;
            Data.SetParameter(symbol.LookupParameter(RclmT.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmT.strength_web), strength_web);
            Data.SetParameter(symbol.LookupParameter(RclmT.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmT.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmT.B), steelT.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmT.A), steelT.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmT.t1), steelT.t1, true);
            Data.SetParameter(symbol.LookupParameter(RclmT.t2), steelT.t2, true);
            Data.SetParameter(symbol.LookupParameter(RclmT.r), steelT.r, true);
            Data.SetParameter(symbol.LookupParameter(RclmT.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmT.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmT.type), steelT.type.ToString());
            Data.SetParameter(symbol.LookupParameter(RclmT.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmT.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_Pipe(StbSecColumn_S clm, string steelshape, string strength_main, FamilySymbol symbol, string kind_column, StbSecPipe steelP)
        {
            FamilyStructure.S_Clm_Pipe RclmP = SetFamily.SClmPipe;
            Data.SetParameter(symbol.LookupParameter(RclmP.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmP.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmP.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmP.D), steelP.D, true);
            Data.SetParameter(symbol.LookupParameter(RclmP.t), steelP.t, true);
            Data.SetParameter(symbol.LookupParameter(RclmP.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmP.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmP.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmP.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_BBox(StbSecColumn_S clm, string steelshape, string strength_main, FamilySymbol symbol, string kind_column, StbSecBuildBox steelBBox)
        {
            FamilyStructure.S_Clm_BBox RclmBBox = SetFamily.SClmBBox;
            Data.SetParameter(symbol.LookupParameter(RclmBBox.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmBBox.B), steelBBox.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.A), steelBBox.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.size_imput), true, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.t1), steelBBox.t1, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.t2), steelBBox.t2, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.r), 0.0, true);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmBBox.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_Box(StbSecColumn_S clm, string steelshape, string strength_main, FamilySymbol symbol, string kind_column, StbSecRollBox steelBox)
        {
            FamilyStructure.S_Clm_Box RclmBox = SetFamily.SClmBox;
            Data.SetParameter(symbol.LookupParameter(RclmBox.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmBox.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmBox.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmBox.B), steelBox.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmBox.A), steelBox.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmBox.t1), steelBox.t, true);
            Data.SetParameter(symbol.LookupParameter(RclmBox.r), steelBox.r, true);
            Data.SetParameter(symbol.LookupParameter(RclmBox.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmBox.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmBox.type), steelBox.type.ToString());
            Data.SetParameter(symbol.LookupParameter(RclmBox.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmBox.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_BH(StbSecColumn_S clm, string steelshape, string strength_main, string strength_web, FamilySymbol symbol, string kind_column, StbSecBuildH steelBH)
        {
            FamilyStructure.S_Clm_BH RclmBH = SetFamily.SClmBH;
            Data.SetParameter(symbol.LookupParameter(RclmBH.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(RclmBH.strength_web), strength_web);
            Data.SetParameter(symbol.LookupParameter(RclmBH.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(RclmBH.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(RclmBH.B), steelBH.B, true);
            Data.SetParameter(symbol.LookupParameter(RclmBH.A), steelBH.A, true);
            Data.SetParameter(symbol.LookupParameter(RclmBH.t1), steelBH.t1, true);
            Data.SetParameter(symbol.LookupParameter(RclmBH.t2), steelBH.t2, true);
            Data.SetParameter(symbol.LookupParameter(RclmBH.r), 0.0, true);
            Data.SetParameter(symbol.LookupParameter(RclmBH.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(RclmBH.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(RclmBH.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(RclmBH.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }

        private static void SetParameter_SColumn_H(StbSecColumn_S clm, string steelshape, string strength_main, string strength_web, FamilySymbol symbol, string kind_column, StbSecRollH steel)
        {
            FamilyStructure.S_Clm_H Rclm = SetFamily.SClmH;
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web), strength_web);
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), kind_column);
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column.ToString());

            Data.SetParameter(symbol.LookupParameter(Rclm.B), steel.B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.A), steel.A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t1), steel.t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t2), steel.t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r), steel.r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.type), steel.type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rclm.type_name), steelshape);
            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_S.base_type.ToString());
        }




        /// <summary>
        /// SRC柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateColumn_SRC(ST_BRIDGE stb, StbSecColumn_SRC clm, Family[][] ConvFamily)
        {
            bool ret = true;

            string typename = GetTypeName_Column(stb, clm.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[SRC柱](断面id=" + clm.id.ToString() + ")");
                return ret;
            }

            string shape = GetSRCSteelShape(clm);

            FamilySymbol symbol = null;
            if (shape == "H")
            {
                if (clm.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                {
                    if (ConvFamily[2][0] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][0], ref typename);
                    }

                    if (!CreateColumn_SRC_H_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][3] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][3], ref typename);
                    }

                    if (!CreateColumn_SRC_H_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else if (shape == "CROSS")
            {
                if (clm.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                {
                    if (ConvFamily[2][1] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][1], ref typename);
                    }

                    if (!CreateColumn_SRC_Cross_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][4] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][4], ref typename);
                    }

                    if (!CreateColumn_SRC_Cross_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else if (shape == "T")
            {
                if (clm.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                {
                    if (ConvFamily[2][2] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][2], ref typename);
                    }

                    if (!CreateColumn_SRC_T_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][5] == null) { ret = false; return ret; }

                    symbol = null;
                    if (columnType[2].ContainsKey(clm.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[2][clm.id]) as FamilySymbol;
                    }
                    else
                    {
                        symbol = GetFamilySymbol_Column(ConvFamily[2][5], ref typename);
                    }

                    if (!CreateColumn_SRC_T_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else
            {
                //ログ表示（変換対象外）
                if (shape == "Box")
                {
                    Data.Make_taisyougaiLog("SRC柱", clm.id, clm.name, "StbSecColumn_SRC_ShapeBox", "SRC柱□形断面鉄骨形状");
                }
                else
                {
                    Data.Make_taisyougaiLog("SRC柱", clm.id, clm.name, "StbSecColumn_SRC_ShapePipe", "SRC柱○形断面鉄骨形状");
                }
            }

            if (clm.StbSecBarArrangementColumn_SRC == null)
            {
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[SRC柱]" + typename + "(断面id=" + clm.id.ToString() + ")");
            }

            if (symbol != null)
            {
                if (!columnType[2].ContainsKey(clm.id))
                {
                    columnType[2].Add(clm.id, symbol.Id);
                }

                Data.SaveGuid(clm.guid, symbol.Id);
            }


            return ret;
        }

        private static string GetSRCSteelShape(StbSecColumn_SRC clm)
        {
            //鉄骨形状を取得
            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            string shape = "";
            if (fig1.Count > 0)
            {
                if (fig1.First().Item is StbSecColumn_SRC_SameShapeH h)
                {
                    shape = "H";
                }
                else if (fig1.First().Item is StbSecColumn_SRC_SameShapeBox b)
                {
                    shape = "Box";
                }
                else if (fig1.First().Item is StbSecColumn_SRC_SameShapePipe p)
                {
                    shape = "Pipe";
                }
                else if (fig1.First().Item is StbSecColumn_SRC_SameShapeCross c)
                {
                    shape = "CROSS";
                }
                else if (fig1.First().Item is StbSecColumn_SRC_SameShapeT t)
                {
                    shape = "T";
                }
            }
            else if (fig2.Count > 0)
            {
                if (fig2.First().Item is StbSecColumn_SRC_NotSameShapeH h)
                {
                    shape = "H";
                }
                else if (fig2.First().Item is StbSecColumn_SRC_NotSameShapeBox b)
                {
                    shape = "Box";
                }
                else if (fig2.First().Item is StbSecColumn_SRC_NotSameShapePipe p)
                {
                    shape = "Pipe";
                }
                else if (fig2.First().Item is StbSecColumn_SRC_NotSameShapeCross c)
                {
                    shape = "CROSS";
                }
                else if (fig2.First().Item is StbSecColumn_SRC_NotSameShapeT t)
                {
                    shape = "T";
                }
            }
            else if (fig3.Count > 0)
            {
                if (fig3.First().Item is StbSecColumn_SRC_ThreeTypesShapeH h)
                {
                    shape = "H";
                }
                else if (fig3.First().Item is StbSecColumn_SRC_ThreeTypesShapeBox b)
                {
                    shape = "Box";
                }
                else if (fig3.First().Item is StbSecColumn_SRC_ThreeTypesShapePipe p)
                {
                    shape = "Pipe";
                }
                else if (fig3.First().Item is StbSecColumn_SRC_ThreeTypesShapeCross c)
                {
                    shape = "CROSS";
                }
                else if (fig3.First().Item is StbSecColumn_SRC_ThreeTypesShapeT t)
                {
                    shape = "T";
                }
            }

            return shape;
        }

        private static bool CreateColumn_SRC_H_Rec(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_H Rclm = SetFamily.SRCClmH;

            string shapename = "SRC柱H形断面鉄骨形状";
            string logtxt = "";
            double B = 0, A = 0, t1 = 0, t2 = 0, r = 0;
            string type = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeH shapeH = null;
            if (fig1.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig3.First().Item;
            }

            string shape = Check_Steel(stb, shapeH?.shape, out int shapeidX);
            if (shape == "")
            {
                if (shapeH != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shapeH.shape + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱H形断面鉄骨形状"); }
                return ret;
            }
            else if (shape == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //差分でもこの関数を使うので消さない
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                type = steel.type.ToString();
            }
            else
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
            }

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main), shapeH.strength_main);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web), GetStrength_web(shapeH.strength_web, shapeH.strength_main));
            Data.SetParameter(symbol.LookupParameter(Rclm.direction_type), shapeH.direction_type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_X), shapeH.offset_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_Y), shapeH.offset_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type), type);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename), shapeH.shape);
            if (shapeH.direction_type == StbSecColumn_SRC_SameShapeHDirection_type.H)
            {
                Data.SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
            }
            Data.SetParameter(symbol.LookupParameter(Rclm.H), A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r), r, true);



            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            StbSecColumn_SRC_Rect fig = (StbSecColumn_SRC_Rect)clm.StbSecFigureColumn_SRC.Item;
            Data.SetParameter(symbol.LookupParameter(Rclm.DX), fig.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.DY), fig.width_Y, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_RectNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_RectNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.StbSecBarArrangementColumn_SRC.interval);
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.StbSecBarArrangementColumn_SRC.kind_corner.ToString());
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.StbSecBarArrangementColumn_SRC.kind_corner.ToString());

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);


                List<StbSecBarColumn_SRC_RectNotSame> bar2 = new List<StbSecBarColumn_SRC_RectNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), bar2[j].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar2[j].N_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar2[j].N_2nd_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar2[j].N_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar2[j].N_2nd_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar2[j].N_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar2[j].N_2nd_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar2[j].N_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar2[j].N_2nd_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar2[j].N_band_direction_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar2[j].N_band_direction_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar2[j].N_main_total);

                    if (j == 0)
                    {
                        pitch_bar_spacing_list = bar2[j].pitch_bar_spacing;
                    }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }

            return ret;
        }
        private static bool CreateColumn_SRC_H_Rou(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_H_Rou Rclm = SetFamily.SRCClmH_Rou;

            string shapename = "SRC柱H形断面鉄骨形状";
            string logtxt = "";
            double B = 0, A = 0, t1 = 0, t2 = 0, r = 0;
            string type = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeH shapeH = null;
            if (fig1.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                shapeH = (StbSecColumn_SRC_SameShapeH)fig3.First().Item;
            }

            string shape = Check_Steel(stb, shapeH?.shape, out int shapeidX);
            if (shape == "")
            {
                if (shapeH != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shapeH.shape + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱H形断面鉄骨形状"); }
                return ret;
            }
            else if (shape == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                type = steel.type.ToString();
            }
            else
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
            }


            if (shapeH.direction_type == StbSecColumn_SRC_SameShapeHDirection_type.H)
            {
                Data.SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
            }
            Data.SetParameter(symbol.LookupParameter(Rclm.H), A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r), r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main), shapeH.strength_main);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web), GetStrength_web(shapeH.strength_web, shapeH.strength_main));
            Data.SetParameter(symbol.LookupParameter(Rclm.direction_type), shapeH.direction_type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_X), shapeH.offset_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_Y), shapeH.offset_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type), type);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename), shapeH.shape);


            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            Data.SetParameter(symbol.LookupParameter(Rclm.D), ((StbSecColumn_SRC_Circle)clm.StbSecFigureColumn_SRC.Item).D, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());

            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_CircleNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_CircleNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);


                List<StbSecBarColumn_SRC_CircleNotSame> bar2 = new List<StbSecBarColumn_SRC_CircleNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar2[j].N_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar2[j].N_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);

                    if (j == 0)
                    {
                        pitch_bar_cpacing_list = bar2[j].pitch_bar_spacing;
                    }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }

            return ret;
        }

        private static bool CreateColumn_SRC_Cross_Rec(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_Cross Rclm = SetFamily.SRCClmCross;

            string shapename = "SRC柱＋形断面鉄骨形状";
            string logtxt = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeCross Cross = null;
            if (fig1.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig3.First().Item;
            }

            //鉄骨形状のindex
            string shapename_X = Cross?.shape_X;
            string shapename_Y = Cross?.shape_Y;
            string shapetypeX = Check_Steel(stb, Cross?.shape_X, out int shapeidX);
            string shapetypeY = Check_Steel(stb, Cross?.shape_Y, out int shapeidY);


            //X方向H形鋼
            double XB = 0, XH = 0, Xt1 = 0, Xt2 = 0, Xr = 0;
            string type_X = "";
            if (shapetypeX == "")
            {
                if (Cross != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + Cross.shape_X + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱＋形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeX == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
                Xr = steel.r;
                type_X = steel.type.ToString();
            }
            else if (shapetypeX == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
            }

            //Y方向H形鋼
            double YB = 0, YH = 0, Yt1 = 0, Yt2 = 0, Yr = 0;
            string type_Y = "";
            if (shapetypeY == "")
            {
                if (Cross != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + Cross.shape_Y + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱＋形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeY == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidY];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
                Yr = steel.r;
                type_Y = steel.type.ToString();
            }
            else if (shapetypeY == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidY];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
            }


            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_X), Cross.strength_main_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_X), GetStrength_web(Cross.strength_web_X, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_Y), GetStrength_web(Cross.strength_main_Y, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_Y), GetStrength_web(Cross.strength_web_Y, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.XH), XH, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.XB), XB, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xt1), Xt1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xt2), Xt2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xr), Xr, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.YH), YH, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.YB), YB, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yt1), Yt1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yt2), Yt2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yr), Yr, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_XX), Cross.offset_XX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_XY), Cross.offset_XY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_YX), Cross.offset_YX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_YY), Cross.offset_YY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_X), type_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_Y), type_Y);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_X), shapename_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_Y), shapename_Y);

            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            StbSecColumn_SRC_Rect fig = (StbSecColumn_SRC_Rect)clm.StbSecFigureColumn_SRC.Item;
            Data.SetParameter(symbol.LookupParameter(Rclm.DX), fig.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.DY), fig.width_Y, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_RectNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_RectNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.StbSecBarArrangementColumn_SRC.interval);
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.StbSecBarArrangementColumn_SRC.kind_corner.ToString());
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.StbSecBarArrangementColumn_SRC.kind_corner.ToString());

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);

                List<StbSecBarColumn_SRC_RectNotSame> bar2 = new List<StbSecBarColumn_SRC_RectNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), bar2[j].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar2[j].N_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar2[j].N_2nd_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar2[j].N_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar2[j].N_2nd_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar2[j].N_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar2[j].N_2nd_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar2[j].N_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar2[j].N_2nd_main_Y_2nd);

                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar2[j].N_band_direction_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar2[j].N_band_direction_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar2[j].N_main_total);

                    if (j == 0)
                    {
                        pitch_bar_spacing_list = bar2[j].pitch_bar_spacing;
                    }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }


            return ret;
        }
        private static bool CreateColumn_SRC_Cross_Rou(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_Cross_Rou Rclm = SetFamily.SRCClmCross_Rou;

            string shapename = "SRC柱＋形断面鉄骨形状";
            string logtxt = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeCross Cross = null;
            if (fig1.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                Cross = (StbSecColumn_SRC_SameShapeCross)fig3.First().Item;
            }

            //鉄骨形状のindex
            string shapename_X = Cross?.shape_X;
            string shapename_Y = Cross?.shape_Y;
            string shapetypeX = Check_Steel(stb, Cross?.shape_X, out int shapeidX);
            string shapetypeY = Check_Steel(stb, Cross?.shape_Y, out int shapeidY);


            //X方向H形鋼
            double XB = 0, XH = 0, Xt1 = 0, Xt2 = 0, Xr = 0;
            string type_X = "";
            if (shapetypeX == "")
            {
                if (Cross != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + Cross.shape_X + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱＋形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeX == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
                Xr = steel.r;
                type_X = steel.type.ToString();
            }
            else if (shapetypeX == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
            }

            //Y方向H形鋼
            double YB = 0, YH = 0, Yt1 = 0, Yt2 = 0, Yr = 0;
            string type_Y = "";
            if (shapetypeY == "")
            {
                if (Cross != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + Cross.shape_Y + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱＋形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeY == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidY];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
                Yr = steel.r;
                type_Y = steel.type.ToString();
            }
            else if (shapetypeY == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidY];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
            }


            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_X), Cross.strength_main_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_X), GetStrength_web(Cross.strength_web_X, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_Y), GetStrength_web(Cross.strength_main_Y, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_Y), GetStrength_web(Cross.strength_web_Y, Cross.strength_main_X));
            Data.SetParameter(symbol.LookupParameter(Rclm.XH), XH, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.XB), XB, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xt1), Xt1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xt2), Xt2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Xr), Xr, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.YH), YH, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.YB), YB, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yt1), Yt1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yt2), Yt2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.Yr), Yr, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_XX), Cross.offset_XX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_XY), Cross.offset_XY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_YX), Cross.offset_YX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_YY), Cross.offset_YY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_X), type_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_Y), type_Y);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_X), shapename_X);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_Y), shapename_Y);


            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            Data.SetParameter(symbol.LookupParameter(Rclm.D), ((StbSecColumn_SRC_Circle)clm.StbSecFigureColumn_SRC.Item).D, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());


            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_CircleNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_CircleNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);


                List<StbSecBarColumn_SRC_CircleNotSame> bar2 = new List<StbSecBarColumn_SRC_CircleNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar2[j].N_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar2[j].N_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);

                    if (j == 0)
                    { pitch_bar_cpacing_list = bar2[j].pitch_bar_spacing; }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }

            return ret;
        }

        private static bool CreateColumn_SRC_T_Rec(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_T Rclm = SetFamily.SRCClmT;

            string shapename = "SRC柱T形断面鉄骨形状";
            string logtxt = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeT shape_T = null;
            if (fig1.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig3.First().Item;
            }

            string shapename_H = shape_T?.shape_H;
            string shapename_T = shape_T?.shape_T;
            string shapetypeH = Check_Steel(stb, shape_T?.shape_H, out int shapeidX);
            string shapetypeT = Check_Steel(stb, shape_T?.shape_T, out int shapeidY);

            //H形鋼
            double H = 0, B = 0, t1 = 0, t2 = 0, r = 0;
            string typeH = "";
            if (shapetypeH == "")
            {
                if (shape_T != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shape_T.shape_H + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱T形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeH == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                typeH = steel.type.ToString();
            }
            else if (shapetypeH == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
            }

            //T形鋼 
            double CT_A = 0, CT_B = 0, CT_t1 = 0, CT_t2 = 0, CT_r = 0;
            string typeT = "";
            if (shapetypeT == "")
            {
                if (shape_T != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shape_T.shape_T + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱T形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeT == RevitLNK.st_steel_T)
            {
                shapename = "SRC柱T形断面鉄骨形状";
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollT[shapeidY];

                logtxt = Roll_T_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                CT_A = steel.A;
                CT_B = steel.B;
                CT_t1 = steel.t1;
                CT_t2 = steel.t2;
                CT_r = steel.r;
                typeT = steel.type.ToString();

            }

            switch (shape_T.direction_type)
            {
                case StbSecColumn_SRC_SameShapeTDirection_type.T1:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
                    break;
                case StbSecColumn_SRC_SameShapeTDirection_type.T3:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 270 * Math.PI / 180);
                    break;
                case StbSecColumn_SRC_SameShapeTDirection_type.T4:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 180 * Math.PI / 180);
                    break;
            }



            Data.SetParameter(symbol.LookupParameter(Rclm.H), H, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r), r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_A), CT_A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_B), CT_B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_t1), CT_t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_t2), CT_t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_r), CT_r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_H), typeH);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_T), typeT);
            Data.SetParameter(symbol.LookupParameter(Rclm.direction_type), shape_T.direction_type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_H), shape_T.shape_H);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_T), shape_T.shape_T);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_H), shape_T.strength_main_H);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_H), GetStrength_web(shape_T.strength_web_H, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_T), GetStrength_web(shape_T.strength_main_T, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_T), GetStrength_web(shape_T.strength_web_T, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_HX), shape_T.offset_HX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_HY), shape_T.offset_HY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_T), shape_T.offset_T, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            StbSecColumn_SRC_Rect fig = (StbSecColumn_SRC_Rect)clm.StbSecFigureColumn_SRC.Item;
            Data.SetParameter(symbol.LookupParameter(Rclm.DX), fig.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.DY), fig.width_Y, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_RectNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_RectNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_X);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.StbSecBarArrangementColumn_SRC.depth_cover_end_Y);
                Data.SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.StbSecBarArrangementColumn_SRC.interval);
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.StbSecBarArrangementColumn_SRC.kind_corner);
                Data.SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.StbSecBarArrangementColumn_SRC.kind_corner);

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);

                List<StbSecBarColumn_SRC_RectNotSame> bar2 = new List<StbSecBarColumn_SRC_RectNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_RectNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), bar2[j].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar2[j].N_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar2[j].N_2nd_main_X_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar2[j].N_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar2[j].N_2nd_main_X_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar2[j].N_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar2[j].N_2nd_main_Y_1st);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar2[j].N_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar2[j].N_2nd_main_Y_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar2[j].N_band_direction_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar2[j].N_band_direction_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar2[j].N_main_total);

                    if (j == 0)
                    { pitch_bar_spacing_list = bar2[j].pitch_bar_spacing; }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }

            return ret;
        }
        private static bool CreateColumn_SRC_T_Rou(ST_BRIDGE stb, StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_T_Rou Rclm = SetFamily.SRCClmT_Rou;

            string shapename = "SRC柱T形断面鉄骨形状";
            string logtxt = "";

            var fig1 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_SRC.Items.OfType<StbSecSteelColumn_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
            StbSecColumn_SRC_SameShapeT shape_T = null;
            if (fig1.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig1.First().Item;
            }
            else if (fig2.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig2.First().Item;
            }
            else if (fig3.Count > 0)
            {
                shape_T = (StbSecColumn_SRC_SameShapeT)fig3.First().Item;
            }

            string shapename_H = shape_T?.shape_H;
            string shapename_T = shape_T?.shape_T;
            string shapetypeH = Check_Steel(stb, shape_T?.shape_H, out int shapeidX);
            string shapetypeT = Check_Steel(stb, shape_T?.shape_T, out int shapeidY);

            //H形鋼
            double H = 0, B = 0, t1 = 0, t2 = 0, r = 0;
            string typeH = "";
            if (shapetypeH == "")
            {
                if (shape_T != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shape_T.shape_H + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱T形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeH == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                typeH = steel.type.ToString();
            }
            else if (shapetypeH == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
            }

            //T形鋼 
            double CT_A = 0, CT_B = 0, CT_t1 = 0, CT_t2 = 0, CT_r = 0;
            string typeT = "";
            if (shapetypeT == "")
            {
                if (shape_T != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shape_T.shape_T + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱T形断面鉄骨形状"); }
                return ret;
            }
            else if (shapetypeT == RevitLNK.st_steel_T)
            {
                shapename = "SRC柱T形断面鉄骨形状";
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollT[shapeidY];

                logtxt = Roll_T_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    //Commons.doc.Delete(symbol.Id);
                    return false;
                }

                CT_A = steel.A;
                CT_B = steel.B;
                CT_t1 = steel.t1;
                CT_t2 = steel.t2;
                CT_r = steel.r;
                typeT = steel.type.ToString();

            }

            switch (shape_T.direction_type)
            {
                case StbSecColumn_SRC_SameShapeTDirection_type.T1:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
                    break;
                case StbSecColumn_SRC_SameShapeTDirection_type.T3:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 270 * Math.PI / 180);
                    break;
                case StbSecColumn_SRC_SameShapeTDirection_type.T4:
                    Data.SetParameter(symbol.LookupParameter(Rclm.angle), 180 * Math.PI / 180);
                    break;
            }



            Data.SetParameter(symbol.LookupParameter(Rclm.H), H, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r), r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_A), CT_A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_B), CT_B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_t1), CT_t1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_t2), CT_t2, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.CT_r), CT_r, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_H), typeH);
            Data.SetParameter(symbol.LookupParameter(Rclm.type_T), typeT);
            Data.SetParameter(symbol.LookupParameter(Rclm.direction_type), shape_T.direction_type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_H), shape_T.shape_H);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename_T), shape_T.shape_T);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_H), shape_T.strength_main_H);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_H), GetStrength_web(shape_T.strength_web_H, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main_T), GetStrength_web(shape_T.strength_main_T, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_web_T), GetStrength_web(shape_T.strength_web_T, shape_T.strength_main_H));
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_HX), shape_T.offset_HX, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_HY), shape_T.offset_HY, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.offset_T), shape_T.offset_T, true);


            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);

            Data.SetParameter(symbol.LookupParameter(Rclm.D), ((StbSecColumn_SRC_Circle)clm.StbSecFigureColumn_SRC.Item).D, true);

            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_SRC.base_type.ToString());

            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBarArrangementColumn_SRC != null)
            {
                bool sameflg = false;
                StbSecBarColumn_SRC_CircleNotSame bar = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>()?.FirstOrDefault(a => a.pos == StbSecBarColumn_RC_NotSamePos.BASE);
                if (bar == null)
                {
                    bar = new StbSecBarColumn_SRC_CircleNotSame(clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleSame>().FirstOrDefault())
                    {
                        pos = StbSecBarColumn_RC_NotSamePos.BASE
                    };
                    sameflg = true;
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.StbSecBarArrangementColumn_SRC.depth_cover_start_X);

                Data.SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), bar.D_bar_spacing);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), bar.strength_band);
                Data.SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), bar.strength_bar_spacing);


                List<StbSecBarColumn_SRC_CircleNotSame> bar2 = new List<StbSecBarColumn_SRC_CircleNotSame>();
                if (sameflg)
                {
                    bar2.Add(bar);
                    bar2.Add(bar);
                }
                else
                {
                    bar2 = clm.StbSecBarArrangementColumn_SRC.Items.OfType<StbSecBarColumn_SRC_CircleNotSame>().OrderBy(a => a.pos).ToList();
                }

                for (int j = 0; j < 2; j++)
                {
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), bar2[j].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar2[j].N_main);
                    Data.SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), bar2[j].D_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar2[j].N_band);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar2[j].pitch_band, true);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar2[j].N_bar_spacing_X);
                    Data.SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar2[j].N_bar_spacing_Y);
                    Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar2[j].pitch_bar_spacing, true);

                    if (j == 0)
                    { pitch_bar_cpacing_list = bar2[j].pitch_bar_spacing; }
                }

                Data.SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }

            return ret;
        }
        

        /// <summary>
        /// CFT柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateColumn_CFT(ST_BRIDGE stb, StbSecColumn_CFT clm, Family[][] ConvFamily)
        {
            bool ret = true;

            string typename = GetTypeName_Column(stb, clm.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[CFT柱](断面id=" + clm.id.ToString() + ")");
                return ret;
            }

            if (clm.StbSecSteelFigureColumn_CFT.Items == null || clm.StbSecSteelFigureColumn_CFT.Items.Count == 0)
            {
                LogData.AddLog(LogData.LogKind.Warning, 3000, "[CFT柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")は柱鉄骨情報");
                return ret;
            }

            //鉄骨形状を取得
            var fig1 = clm.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_Same>().ToList();
            var fig2 = clm.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_NotSame>().OrderBy(a => a.pos).ToList();
            var fig3 = clm.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>().OrderBy(a => a.pos).ToList();
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


            FamilySymbol symbol = null;
            if (shape == RevitLNK.st_steel_Box || shape == RevitLNK.st_steel_BBox)
            {
                if (ConvFamily[3][0] == null) { ret = false; return ret; }

                if (columnType[3].ContainsKey(clm.id))
                {
                    symbol = Commons.doc.GetElement(columnType[3][clm.id]) as FamilySymbol;
                }
                else
                {
                    symbol = GetFamilySymbol_Column(ConvFamily[3][0], ref typename);
                }

                SetParameter_CFTColumn_Box(stb, clm, steelshape, strength_main, shape, shapeid, symbol);
            }
            else if (shape == RevitLNK.st_steel_Pipe)
            {
                if (ConvFamily[3][1] == null) { ret = false; return ret; }

                if (columnType[3].ContainsKey(clm.id))
                {
                    symbol = Commons.doc.GetElement(columnType[3][clm.id]) as FamilySymbol;
                }
                else
                {
                    symbol = GetFamilySymbol_Column(ConvFamily[3][1], ref typename);
                }

                SetParameter_CFTColumn_Pipe(stb, clm, steelshape, strength_main, shapeid, symbol);
            }

            if (symbol != null)
            {
                if (!columnType[3].ContainsKey(clm.id))
                {
                    columnType[3].Add(clm.id, symbol.Id);
                }

                Data.SaveGuid(clm.guid, symbol.Id);
            }


            return ret;
        }

        private static void SetParameter_CFTColumn_Pipe(ST_BRIDGE stb, StbSecColumn_CFT clm, string steelshape, string strength_main, int shapeid, FamilySymbol symbol)
        {
            FamilyStructure.CFT_Clm_Pipe Rclm = SetFamily.CFTClmPipe;

            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];

            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete, false, true);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename), steelshape);
            Data.SetParameter(symbol.LookupParameter(Rclm.D), steel.D, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t), steel.t, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_CFT.base_type.ToString());
            //Data.SetParameter(symbol.LookupParameter(Rclm.enbedded_length), clm.enbedded_length, true);
        }

        private static void SetParameter_CFTColumn_Box(ST_BRIDGE stb, StbSecColumn_CFT clm, string steelshape, string strength_main, string shape, int shapeid, FamilySymbol symbol)
        {
            double B = 0, A = 0, t = 0, r1 = 0;
            string type = "";
            if (shape == RevitLNK.st_steel_Box)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollBox[shapeid];
                B = steel.B;
                A = steel.A;
                t = steel.t;
                r1 = steel.r;
                type = steel.type.ToString();
            }
            else
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox[shapeid];
                B = steel.B;
                A = steel.A;
                t = steel.t1;
            }

            FamilyStructure.CFT_Clm_Box Rclm = SetFamily.CFTClmBox;
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete, false, true);
            if (clm.kind_column == StbSecColumn_Kind_column.COLUMN)
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            Data.SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            Data.SetParameter(symbol.LookupParameter(Rclm.direction_type), clm.isReferenceDirection);
            Data.SetParameter(symbol.LookupParameter(Rclm.type), type);
            Data.SetParameter(symbol.LookupParameter(Rclm.typename), steelshape);
            Data.SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.A), A, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.t), t, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.r1), r1, true);
            Data.SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            Data.SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            Data.SetParameter(symbol.LookupParameter(Rclm.base_type), clm.StbSecSteelFigureColumn_CFT.base_type.ToString());
            //Data.SetParameter(symbol.LookupParameter(Rclm.enbedded_length), clm.enbedded_length, true);
        }





        /// <summary>
        /// 柱の生成とインスタンスパラメータ設定（柱,間柱）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateColumn_instance(ST_BRIDGE stb, StbColumn clm, Family[][] ConvFamily)
        {
            bool ret = true;

            //回転角が360度以上→-360度する
            if (clm.rotate >= 360) { clm.rotate = clm.rotate - 360; }

            string typename = GetTypeName_Column(stb, clm.id_section);
            StbSecColumn_Kind_column kind_column = StbSecColumn_Kind_column.COLUMN;
            int st_number = 0;

            switch (clm.kind_structure)
            {
                case StbColumnKind_structure.RC:
                    var sec1 = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == clm.id_section);
                    kind_column = sec1.kind_column;
                    st_number = 0;
                    break;

                case StbColumnKind_structure.S:
                    var sec2 = stb.StbModel.StbSections.StbSecColumn_S.Find(a => a.id == clm.id_section);
                    kind_column = sec2.kind_column;
                    st_number = 1;
                    break;

                case StbColumnKind_structure.SRC:
                    var sec3 = stb.StbModel.StbSections.StbSecColumn_SRC.Find(a => a.id == clm.id_section);
                    kind_column = sec3.kind_column;
                    st_number = 2;
                    break;

                case StbColumnKind_structure.CFT:
                    var sec4 = stb.StbModel.StbSections.StbSecColumn_CFT.Find(a => a.id == clm.id_section);
                    kind_column = sec4.kind_column;
                    st_number = 3;
                    break;
            }


            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, clm.id_node_bottom);
            Level btmLevel = null;
            int index = indb;
            do
            {
                btmLevel = SearchLevel(stb, index);
                index--;
                if (index < 0) { break; }
            } while (btmLevel == null);
            if (btmLevel == null)
            {
                index = indb;
                do
                {
                    btmLevel = SearchLevel(stb, index);
                    index++;
                    if (index == stb.StbModel.StbStories.Count()) { break; }
                } while (btmLevel == null);
            }
            if (btmLevel == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")は基準レベルが取得できないため変換できません。");
                return ret;
            }

            int indt = Get_stbFloor_index(stb, clm.id_node_top);
            Level topLevel = null;
            index = indt;
            do
            {
                topLevel = SearchLevel(stb, index);
                index++;
                if (index == stb.StbModel.StbStories.Count()) { break; }
            } while (topLevel == null);
            if (topLevel == null)
            {
                index = indt;
                do
                {
                    topLevel = SearchLevel(stb, index);
                    index--;
                    if (index < 0) { break; }
                } while (topLevel == null);

            }
            if (topLevel == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure.ToString() + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")は上部レベルが取得できないため変換できません。");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (columnType[st_number].ContainsKey(clm.id_section))
            {
                symbol = Commons.doc.GetElement(columnType[st_number][clm.id_section]) as FamilySymbol;
            }
            else
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + clm.kind_structure.ToString() + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得 
            XYZ Pt = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node_top, clm.offset_top_X, clm.offset_top_Y, 0);
            XYZ Pb = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, 0);

            //オフセット
            //柱頭のZ座標と上部レベルの代表値の差
            double offset_t = Commons.ft2mm(Pt.Z - topLevel.Elevation);
            //柱脚のZ座標と基準レベルの代表値の差
            double offset_b = Commons.ft2mm(Pb.Z - btmLevel.Elevation);

            //柱脚に接続する梁のZ方向オフセット
            double gir_offset_Z_bottom = 0;
            //柱頭に接続する梁のZ方向オフセット
            double gir_offset_Z_top = 0;

            if (clm.offset_bottom_Z == 0) //柱脚Z方向オフセット値が0以外の時はその値を優先する
            { Search_Girder_Offset_Z_bottom(stb, clm.id_node_bottom, btmLevel, clm.kind_structure, out gir_offset_Z_bottom); }

            if (clm.offset_top_Z == 0) //柱頭Z方向オフセット値が0以外の時はその値を優先する
            { Search_Girder_Offset_Z_top(stb, clm.id_node_top, topLevel, clm.kind_structure, out gir_offset_Z_top, kind_column); }

            XYZ Pt_offset = new XYZ(Pt.X, Pt.Y, Pt.Z + Commons.mm2ft(gir_offset_Z_top + clm.offset_top_Z));
            XYZ Pb_offset = new XYZ(Pb.X, Pb.Y, Pb.Z + Commons.mm2ft(gir_offset_Z_bottom + clm.offset_bottom_Z));
            double length = Commons.PointPointDist3D(Pt_offset, Pb_offset);
            if (length <= 1)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + clm.name + "(配置id=" + clm.id.ToString() + ")は長さが0mmのため変換できません。");
                return ret;
            }
            if (Pt_offset.Z < Pb_offset.Z)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + clm.name + "(配置id=" + clm.id.ToString() + ")は柱頭の位置が柱脚の位置より低いため変換できません。");
                return ret;
            }

            //傾斜柱チェック
            bool IsSlant = false;
            XYZ vecU = (Pt_offset - Pb_offset).Normalize();
            if (Math.Abs(1 - vecU.Z) > gosa)
            {
                IsSlant = true;
            }

            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (IsSlant)
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Line.CreateBound(Pb, Pt), symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);
                    if (symbol.FamilyName == "Steel_Column_Box" || symbol.FamilyName == "Steel_Column_Pipe") //このファミリで傾斜柱のときダイアフラムを非表示にする
                    {
                        Data.SetParameter(instance.LookupParameter("Diaphragm"), false);
                    }
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Pb, symbol, btmLevel, StructuralType.Column);
                }

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ) 
                //回転 ※ラジアンに直して
                double rotate = clm.rotate * Math.PI / 180;
                if (clm.kind_structure == StbColumnKind_structure.S)
                {
                    var sec2 = stb.StbModel.StbSections.StbSecColumn_S.Find(a => a.id == clm.id_section);
                    if (!sec2.isReferenceDirection)
                    {
                        rotate += Math.PI / 2;
                    }
                }
                if (Pb.DistanceTo(Pt) < Commons.doc.Application.ShortCurveTolerance)
                {
                    instance.Location.Rotate(Line.CreateBound(Pb, Pb + XYZ.BasisZ), rotate);
                }
                else
                {
                    instance.Location.Rotate(Line.CreateBound(Pb, Pb + (Pt - Pb).Normalize()), rotate);
                }

                Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, topLevel.Id); //上部レベル

                double top = Pt.Z + Commons.mm2ft(clm.offset_top_Z + gir_offset_Z_top + offset_t);


                if (offset_t <= 0 && offset_b <= 0)
                {
                    //柱脚レベル
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                    //柱頭レベル
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                }
                else
                {
                    //柱頭レベル
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                    //柱脚レベル
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                }


                //解析線分作成
                Commons.doc.Regenerate();
                XYZ Pb_org = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node_bottom, 0, 0, 0);
                XYZ Pt_org = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node_top, 0, 0, 0);
                AnalyticalMember member = AnalyticalMember.Create(Commons.doc, Line.CreateBound(Pb_org, Pt_org));
                if (member != null)
                {
                    //構造の役割
                    var p = member.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleColumn);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(member.Id, instance.Id);
                }

                Data.SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_BASE_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle); //下部のカットスタイル
                Data.SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_TOP_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle);  //上部のカットスタイル               

                SetInstanceParameter_Column(stb, clm, instance);

                //変換情報ログの出力
                string col = kind_column == StbSecColumn_Kind_column.POST ? "間柱" : "柱";
                var nodeIds = new int[] { clm.id_node_bottom, clm.id_node_top } ;
                Data.MakeNodeLog( $"{col}の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, clm.id, col, typename, nodeIds );
                Data.SaveGuid(clm.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        private static void SetInstanceParameter_Column(ST_BRIDGE stb, StbColumn clm, FamilyInstance instance)
        {
            string steelshape = "";
            string shape = "";

            switch (clm.kind_structure)
            {
                case StbColumnKind_structure.RC:
                    var sec1 = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == clm.id_section);
                    if (sec1.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect)
                    {
                        FamilyStructure.RC_Clm_Re RCclm = SetFamily.RCClmRe;
                        Data.SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                        Data.SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                        Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                        Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_X), clm.thickness_add_end_X, true);
                        Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_Y), clm.thickness_add_start_Y, true);
                        Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_Y), clm.thickness_add_end_Y, true);
                    }
                    else
                    {
                        FamilyStructure.RC_Clm_Ro RCclm = SetFamily.RCClmRo;
                        Data.SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                        Data.SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                        Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                    }
                    break;

                case StbColumnKind_structure.S:
                    var sec2 = stb.StbModel.StbSections.StbSecColumn_S.Find(a => a.id == clm.id_section);
                    var fig21 = sec2.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_Same>().ToList();
                    var fig22 = sec2.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_NotSame>().OrderBy(a => a.pos).ToList();
                    var fig23 = sec2.StbSecSteelFigureColumn_S.Items.OfType<StbSecSteelColumn_S_ThreeTypes>().OrderBy(a => a.pos).ToList();
                    if (fig21.Count > 0)
                    {
                        steelshape = fig21.First().shape;
                    }
                    else if (fig22.Count > 0)
                    {
                        steelshape = fig22.First().shape;
                    }
                    else if (fig23.Count > 0)
                    {
                        steelshape = fig23.First().shape;
                    }

                    shape = Check_Steel(stb, steelshape, out _);
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmH.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_BH:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBH.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_Box:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBox.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_BBox:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmBBox.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_Pipe:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmPipe.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_T:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmT.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_C:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmC.kind_joint_bottom), clm.kind_joint_bottom);
                            break;

                        case RevitLNK.st_steel_L:
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SetFamily.SClmL.kind_joint_bottom), clm.kind_joint_bottom);
                            break;
                    }
                    break;

                case StbColumnKind_structure.SRC:
                    var sec3 = stb.StbModel.StbSections.StbSecColumn_SRC.Find(a => a.id == clm.id_section);
                    shape = GetSRCSteelShape(sec3);
                    if (sec3.StbSecFigureColumn_SRC.Item is StbSecColumn_SRC_Rect)
                    {
                        if (shape == "CROSS")
                        {
                            var SRCclm = SetFamily.SRCClmCross;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_X), clm.thickness_add_end_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_Y), clm.thickness_add_start_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_Y), clm.thickness_add_end_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        else if (shape == "T")
                        {
                            var SRCclm = SetFamily.SRCClmT;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_X), clm.thickness_add_end_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_Y), clm.thickness_add_start_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_Y), clm.thickness_add_end_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        else
                        {
                            var SRCclm = SetFamily.SRCClmH;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_X), clm.thickness_add_end_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_Y), clm.thickness_add_start_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_Y), clm.thickness_add_end_Y, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                    }
                    else
                    {
                        if (shape == "CROSS")
                        {
                            var SRCclm = SetFamily.SRCClmCross_Rou;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        else if (shape == "T")
                        {
                            var SRCclm = SetFamily.SRCClmT_Rou;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        else
                        {
                            var SRCclm = SetFamily.SRCClmH_Rou;
                            Data.SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            Data.SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            Data.SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_add_start_X, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            Data.SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_top), clm.joint_top, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.joint_bottom), clm.joint_bottom, true);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            Data.SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                    }
                    break;

                case StbColumnKind_structure.CFT:
                    var sec4 = stb.StbModel.StbSections.StbSecColumn_CFT.Find(a => a.id == clm.id_section);
                    var fig41 = sec4.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_Same>().ToList();
                    var fig42 = sec4.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_NotSame>().OrderBy(a => a.pos).ToList();
                    var fig43 = sec4.StbSecSteelFigureColumn_CFT.Items.OfType<StbSecSteelColumn_CFT_ThreeTypes>().OrderBy(a => a.pos).ToList();
                    if (fig41.Count > 0)
                    {
                        steelshape = fig41.First().shape;
                    }
                    else if (fig42.Count > 0)
                    {
                        steelshape = fig42.First().shape;
                    }
                    else if (fig43.Count > 0)
                    {
                        steelshape = fig43.First().shape;
                    }

                    shape = Check_Steel(stb, steelshape, out _);

                    if (shape == RevitLNK.st_steel_Box || shape == RevitLNK.st_steel_BBox)
                    {
                        var CFTclm = SetFamily.CFTClmBox;
                        Data.SetParameter(instance.LookupParameter(CFTclm.MemId), clm.id);
                        Data.SetParameter(instance.LookupParameter(CFTclm.NameMembers), clm.name);
                        Data.SetParameter(instance.LookupParameter(CFTclm.condition_bottom), clm.condition_bottom);
                        Data.SetParameter(instance.LookupParameter(CFTclm.condition_top), clm.condition_top);
                        Data.SetParameter(instance.LookupParameter(CFTclm.joint_top), clm.joint_top, true);
                        Data.SetParameter(instance.LookupParameter(CFTclm.joint_bottom), clm.joint_bottom, true);
                        Data.SetParameter(instance.LookupParameter(CFTclm.kind_joint_top), clm.kind_joint_top);
                        Data.SetParameter(instance.LookupParameter(CFTclm.kind_joint_bottom), clm.kind_joint_bottom);
                    }
                    else if (shape == RevitLNK.st_steel_Pipe)
                    {
                        var CFTclm = SetFamily.CFTClmPipe;
                        Data.SetParameter(instance.LookupParameter(CFTclm.MemId), clm.id);
                        Data.SetParameter(instance.LookupParameter(CFTclm.NameMembers), clm.name);
                        Data.SetParameter(instance.LookupParameter(CFTclm.condition_bottom), clm.condition_bottom);
                        Data.SetParameter(instance.LookupParameter(CFTclm.condition_top), clm.condition_top);
                        Data.SetParameter(instance.LookupParameter(CFTclm.joint_top), clm.joint_top, true);
                        Data.SetParameter(instance.LookupParameter(CFTclm.joint_bottom), clm.joint_bottom, true);
                        Data.SetParameter(instance.LookupParameter(CFTclm.kind_joint_top), clm.kind_joint_top);
                        Data.SetParameter(instance.LookupParameter(CFTclm.kind_joint_bottom), clm.kind_joint_bottom);
                    }
                    break;
            }
        }

        /// <summary>柱インスタンスパラメータ設定（基礎柱）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="ConvFamily"></param>
        /// <param name="flag">差分時の作成フラグ 0なら基礎柱だけ作る、1なら根巻だけ作る</param>
        /// <returns></returns>
        private static bool CreateFoundationColumn_instance(ST_BRIDGE stb, StbFoundationColumn clm, Family[][] ConvFamily, int flag = -1)
        {
            bool ret = true;

            //基礎柱
            var secFD = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == clm.id_section_FD);
            //根巻
            var secWR = stb.StbModel.StbSections.StbSecColumn_RC.Find(a => a.id == clm.id_section_WR);

            for (int k = 0; k <= 1; ++k)
            {
                if (flag >= 0)
                {
                    if (flag != k) continue;
                }

                string kind = k == 0 ? "基礎柱" : "根巻柱";
                var sec = k == 0 ? secFD : secWR;
                if (sec == null)
                {
                    continue;
                }


                //タイプ名
                string typename = GetTypeName_Column(stb, sec.id);


                Family fami = null;
                if (sec.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect)
                { fami = ConvFamily[0][0]; }
                else
                { fami = ConvFamily[0][1]; }


                //配置レベルの取得           
                int indt = Get_stbFloor_index(stb, clm.id_node);
                Level bottomLevel = SearchLevel(stb, indt);
                Level topLevel = SearchLevel(stb, indt + 1);

                //基準レベルのオフセットの設定（基礎柱高さ）
                double height = k == 0 ? clm.length_FD : clm.length_WR;
                if (height <= 1)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 0, $"[{kind}]" + clm.name + "(id=" + clm.id.ToString() + $")は{kind}高さが0mmのため変換できません。");
                    continue;
                }

                //配置座標の取得
                XYZ Pt = new XYZ();
                XYZ Pb = new XYZ();
                if (k == 0)
                {
                    Pt = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node, clm.offset_FD_X, clm.offset_FD_Y, clm.offset_Z);
                    Pb = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node, clm.offset_FD_X, clm.offset_FD_Y, -height + clm.offset_Z);
                }
                else
                {
                    Pt = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node, clm.offset_WR_X, clm.offset_WR_Y, clm.offset_Z + height);
                    Pb = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node, clm.offset_WR_X, clm.offset_WR_Y, clm.offset_Z);
                }



                //ファミリがロードされているか           
                if (fami == null)
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2100, clm.kind_structure + "柱");
                    return ret;
                }

                //タイプがすでに生成されているか
                FamilySymbol symbol = null;
                if (columnType[0].ContainsKey(sec.id))
                {
                    symbol = Commons.doc.GetElement(columnType[0][sec.id]) as FamilySymbol;
                }
                else
                {
                    CreateColumn_RC(stb, sec, ConvFamily);
                    if (columnType[0].ContainsKey(sec.id))
                    {
                        symbol = Commons.doc.GetElement(columnType[0][sec.id]) as FamilySymbol;
                    }
                    else
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")");
                        continue;
                    }
                }


                //インスタンスの生成
                try
                {
                    //stbで指定されている座標は柱頭→とりあえず柱頭から上に柱を生成
                    FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(Pt, symbol, bottomLevel, StructuralType.Column);
                    //上部レベルを基点レベルと同じレベルに設定
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, bottomLevel.Id);

                    //基準レベルのオフセット（feet換算済み）
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, Pb.Z - bottomLevel.Elevation);
                    Data.SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, Pt.Z - bottomLevel.Elevation);


                    //回転 ※ラジアンに直して
                    instance.Location.Rotate(Line.CreateBound(Pb, Pb + (Pt - Pb).Normalize()), (clm.rotate * Math.PI) / 180);


                    SetInstanceParameter_FoundationColumn(clm, k, sec, instance);

                    //変換情報ログの出力
                    var nodeIds = new int[] { clm.id_node } ;
                    Data.MakeNodeLog( $"{kind}の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                    OutputDebubCommentLog( instance, clm.id, kind, typename, nodeIds ) ;

                    Data.SaveGuid(clm.guid, instance.Id);
                }
                catch (Exception)
                {
                    ret = false;
                }
            }


            return ret;
        }

        /// <summary>
        /// 基礎柱インスタンスパラメータ設定
        /// </summary>
        /// <param name="clm"></param>
        /// <param name="k">0:基礎柱, 1:根巻</param>
        /// <param name="sec"></param>
        /// <param name="instance"></param>
        private static void SetInstanceParameter_FoundationColumn(StbFoundationColumn clm, int k, StbSecColumn_RC sec, FamilyInstance instance)
        {
            if (sec.StbSecFigureColumn_RC.Item is StbSecColumn_RC_Rect)
            {
                FamilyStructure.RC_Clm_Re RCclm = SetFamily.RCClmRe;
                Data.SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                Data.SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), k == 0 ? clm.thickness_add_FD_start_X : clm.thickness_add_WR_start_X, true);
                Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_X), k == 0 ? clm.thickness_add_FD_end_X : clm.thickness_add_WR_end_X, true);
                Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_Y), k == 0 ? clm.thickness_add_FD_start_Y : clm.thickness_add_WR_start_Y, true);
                Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_Y), k == 0 ? clm.thickness_add_FD_end_Y : clm.thickness_add_WR_end_Y, true);
            }
            else
            {
                FamilyStructure.RC_Clm_Ro RCclm = SetFamily.RCClmRo;
                Data.SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                Data.SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                Data.SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), k == 0 ? clm.thickness_add_FD_start_X : clm.thickness_add_WR_start_X, true);
            }
        }


        #endregion


        #region 梁

        /// <summary>
        /// 梁の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="buzai"></param>
        /// <param name="elements"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateGirder(ST_BRIDGE stb, string buzai, List<FamilySymbol> elements, ref string errmsg)
        {
            bool ret = true;

            StbSecBeam_Kind_beam kind = StbSecBeam_Kind_beam.GIRDER;
            bool isCanti = false;
            switch (buzai)
            {
                case "大梁":
                    kind = StbSecBeam_Kind_beam.GIRDER;
                    break;
                case "小梁":
                    kind = StbSecBeam_Kind_beam.BEAM;
                    break;
                case "片持梁":
                    kind = StbSecBeam_Kind_beam.GIRDER;
                    isCanti = true;
                    break;
                case "片持小梁":
                    kind = StbSecBeam_Kind_beam.BEAM;
                    isCanti = true;
                    break;
            }

            //変換ファミリの取得
            Family[][] ConvFamily = new Family[RevitLNK.GirText.Length][];
            if (!isCanti)
            {
                for (int i = 0; i < RevitLNK.GirText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.GirText[i].Length);
                }
            }
            else
            {
                for (int i = 0; i < RevitLNK.CGirText.Length; i++)
                {
                    Array.Resize(ref ConvFamily[i], RevitLNK.CGirText[i].Length);
                }
            }

            if (elements == null || elements.Count == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            //パラメータの追加
            Data.ProgressStart("梁パラメータ追加", ConvFamily.Count());
            for (int i = 0; i < ConvFamily.Length; i++)
            {
                Data.ProgressPerformStep();

                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (FamilySymbol familysymbol in elements)
                    {
                        if (buzai == "大梁")
                        {
                            if (!SetFamily.GirFName.flg[i][j]) { continue; }
                            if (!SetFamily.GirFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.GirFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;

                                Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");

                                try
                                {
                                    tran1.Start();

                                    FamilyManager fmg = doc.FamilyManager;
                                    switch (i)
                                    {
                                        case 0:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_F);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_F_Haunch);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCGir_Haunch);
                                                    break;
                                            }
                                            break;
                                        case 1:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SGirH(fmg, SetFamily.SGirH);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SGirBH(fmg, SetFamily.SGirBH);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SGirC(fmg, SetFamily.SGirC);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SGirL(fmg, SetFamily.SGirL);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SGirLipC);
                                                    break;
                                                case 5:
                                                    ParaSet.SetPara_SGirH(fmg, SetFamily.SGirH_Haunch);
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            ParaSet.SetPara_SRCGirH(fmg, SetFamily.SRCGirH);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.GirFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                        }
                        else if (buzai == "小梁")
                        {
                            if (!SetFamily.BeamFName.flg[i][j]) { continue; }
                            if (!SetFamily.BeamFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.BeamFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;

                                Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();

                                    FamilyManager fmg = doc.FamilyManager;
                                    switch (i)
                                    {
                                        case 0:

                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_F);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_F_Haunch);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_RCGir(fmg, SetFamily.RCBeam_Haunch);
                                                    break;
                                            }

                                            break;
                                        case 1:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SGirH(fmg, SetFamily.SBeamH);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SGirBH(fmg, SetFamily.SBeamBH);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SGirC(fmg, SetFamily.SBeamC);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SGirL(fmg, SetFamily.SBeamL);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SBeamLipC);
                                                    break;
                                                case 5:
                                                    ParaSet.SetPara_SGirH(fmg, SetFamily.SBeamH_Haunch);
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            ParaSet.SetPara_SRCGirH(fmg, SetFamily.SRCBeamH);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.BeamFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                        }
                        else if (buzai == "片持梁")
                        {
                            if (!SetFamily.CGirFName.flg[i][j]) { continue; }
                            if (!SetFamily.CGirFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.CGirFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;

                                Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();

                                    FamilyManager fmg = doc.FamilyManager;
                                    switch (i)
                                    {
                                        case 0:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir_F);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir);
                                                    break;

                                            }
                                            break;
                                        case 1:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirH);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirBH);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SGirC(fmg, SetFamily.SCGirC);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SGirL(fmg, SetFamily.SCGirL);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SCGirLipC);
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            ParaSet.SetPara_SRCCGirH(fmg, SetFamily.SRCCGirH);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.CGirFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                        }
                        else if (buzai == "片持小梁")
                        {
                            if (!SetFamily.CBeamFName.flg[i][j]) { continue; }
                            if (!SetFamily.CBeamFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.CBeamFName.FamilyName[i][j])
                            {
                                ConvFamily[i][j] = familysymbol.Family;

                                Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();

                                    FamilyManager fmg = doc.FamilyManager;
                                    switch (i)
                                    {
                                        case 0:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir_F);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_RCCGir(fmg, SetFamily.RCCGir);
                                                    break;

                                            }
                                            break;
                                        case 1:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirH);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SCGirH(fmg, SetFamily.SCGirBH);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SGirC(fmg, SetFamily.SCGirC);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SGirL(fmg, SetFamily.SCGirL);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SGirLipC(fmg, SetFamily.SCGirLipC);
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            ParaSet.SetPara_SRCCGirH(fmg, SetFamily.SRCCBeamH);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.CBeamFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                        }
                    }
                }
            }



            Transaction tran = new Transaction(Commons.doc, buzai + "の生成");
            try
            {
                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();
                tran.Start();

                //梁タイプパラメータの設定
                //RC梁
                if (stb.StbModel.StbSections.StbSecBeam_RC != null) //Gir[0][0](RC梁)
                {
                    var girders = stb.StbModel.StbSections.StbSecBeam_RC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).ToList();
                    if (girders.Count > 0)
                    {
                        Data.ProgressRestart("RC梁の生成", girders.Count);
                        foreach (var gir in girders)
                        {
                            Data.ProgressPerformStep();

                            if (gir.isCanti)
                            {
                                if (!CreateCGirder_RC(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                            else
                            {
                                if (!CreateGirder_RC(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                        }
                    }
                }

                //S梁
                if (stb.StbModel.StbSections.StbSecBeam_S != null) //Gir[1][0](H形),Gir[1][1](組立H形),Gir[1][2](溝形),Gir[1][3](山形),Gir[1][4](リップ溝形)
                {
                    var girders = stb.StbModel.StbSections.StbSecBeam_S.Where(a => a.isCanti == isCanti && a.kind_beam == kind).ToList();
                    if (girders.Count > 0)
                    {
                        Data.ProgressRestart("S梁の生成", girders.Count);
                        foreach (var gir in girders)
                        {
                            Data.ProgressPerformStep();

                            if (gir.isCanti)
                            {
                                if (!CreateCGirder_S(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                            else
                            {
                                if (!CreateGirder_S(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                        }
                    }
                }

                //SRC梁
                if (stb.StbModel.StbSections.StbSecBeam_SRC != null)
                {
                    var girders = stb.StbModel.StbSections.StbSecBeam_SRC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).ToList();
                    if (girders.Count > 0)
                    {
                        Data.ProgressRestart("SRC梁の生成", girders.Count);
                        foreach (var gir in girders)
                        {
                            Data.ProgressPerformStep();

                            if (gir.isCanti)
                            {
                                if (!CreateCGirder_SRC(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                            else
                            {
                                if (!CreateGirder_SRC(stb, gir, ConvFamily, ref typename_list)) { ret = false; }
                            }
                        }
                    }
                }

                Data.ProgressClose();
                Commons.doc.Regenerate();


                //梁インスタンスパラメータの設定 
                if (kind == StbSecBeam_Kind_beam.GIRDER)
                {
                    if (stb.StbModel.StbMembers.StbGirders != null)
                    {
                        List<int> id_section = new List<int>();
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_RC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_S.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_SRC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));

                        var girders = stb.StbModel.StbMembers.StbGirders.Where(a => id_section.Contains(a.id_section)).ToList();
                        if (girders.Count > 0)
                        {
                            Data.ProgressRestart($"{buzai}の生成", girders.Count);
                            foreach (var gir in girders)
                            {
                                Data.ProgressPerformStep();

                                if (!CreateGirder_instance(stb, gir, ConvFamily, ConvFamily, kind))
                                {
                                    ret = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (stb.StbModel.StbMembers.StbBeams != null)
                    {
                        List<int> id_section = new List<int>();
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_RC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_S.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));
                        id_section.AddRange(stb.StbModel.StbSections.StbSecBeam_SRC.Where(a => a.isCanti == isCanti && a.kind_beam == kind).Select(a => a.id));

                        var girders = stb.StbModel.StbMembers.StbBeams.Where(a => id_section.Contains(a.id_section)).ToList();
                        if (girders.Count > 0)
                        {
                            Data.ProgressRestart($"{buzai}の生成", girders.Count);
                            foreach (var gir in girders)
                            {
                                Data.ProgressPerformStep();

                                if (!CreateGirder_instance(stb, gir, ConvFamily, ConvFamily, kind))
                                {
                                    ret = false;
                                }
                            }
                        }
                    }
                }

                Data.ProgressClose();
                tran.Commit();


                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                List<FamilySymbol> elements_end = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();
                for (int i = 0; i < typename_list.Count(); i++)
                {
                    bool flg = elements_end.Any(a => a.Name == typename_list[i].typename);
                    if (!flg)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + typename_list[i].shapename + "]" + typename_list[i].typename +
                                       "(断面id=" + typename_list[i].id.ToString() + ")を生成できませんでした。寸法値またはファミリの設定を確認してください。");
                    }

                }
            }
            catch //(Exception e)
            {
                ret = false;
                tran.RollBack();
            }

            if (ret == false)
            {
                errmsg = buzai;
            }

            Data.ProgressClose();

            return ret;
        }


        /// <summary>
        /// RC梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateGirder_RC(ST_BRIDGE stb, StbSecBeam_RC gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;
            //変換に使用するファミリ
            Family fami = null;
            //ログ用部材名
            string logbuzai = "";
            string kind = "";
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { kind = "大梁"; }
            else { kind = "小梁"; }

            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[RC梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }


            if (!gir.isCanti)
            {
                switch (gir.StbSecFigureBeam_RC.FigureType) //ファミリを詳細化⇒ハンチ付か3断面同一かを判断する
                {
                    case 1:
                        if (gir.StbSecBarArrangementBeam_RC == null) //鉄筋情報が無い場合は全断面として変換
                        {
                            //ハンチなし
                            if (gir.isFoundation)
                            {
                                logbuzai = "基礎" + kind;
                                fami = ConvFamily[0][0];
                            }
                            else
                            {
                                logbuzai = "RC" + kind;
                                fami = ConvFamily[0][2];
                            }
                        }
                        else
                        {
                            if (gir.StbSecBarArrangementBeam_RC.Bar_ArrangementType == 1)
                            {
                                //ハンチなし
                                if (gir.isFoundation)
                                {
                                    logbuzai = "基礎" + kind;
                                    fami = ConvFamily[0][0];
                                }
                                else
                                {
                                    logbuzai = "RC" + kind;
                                    fami = ConvFamily[0][2];
                                }
                            }
                            else
                            {
                                //ハンチ付き
                                if (gir.isFoundation)
                                {
                                    logbuzai = "ハンチ付き基礎" + kind;
                                    fami = ConvFamily[0][1];
                                }
                                else
                                {
                                    logbuzai = "ハンチ付きRC" + kind;
                                    fami = ConvFamily[0][3];
                                }
                            }
                        }
                        break;

                    case 2:
                        var ts = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                        var te = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                        if (ts.depth != te.depth ||
                            ts.width != te.width)
                        {
                            //ハンチ付き
                            if (gir.isFoundation)
                            {
                                logbuzai = "ハンチ付き基礎" + kind;
                                fami = ConvFamily[0][1];
                            }
                            else
                            {
                                logbuzai = "ハンチ付きRC" + kind;
                                fami = ConvFamily[0][3];
                            }
                        }
                        else
                        {
                            //ハンチなし
                            if (gir.isFoundation)
                            {
                                logbuzai = "基礎" + kind;
                                fami = ConvFamily[0][0];
                            }
                            else
                            {
                                logbuzai = "RC" + kind;
                                fami = ConvFamily[0][2];
                            }
                        }
                        break;

                    case 3:
                        var hs = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                        var hc = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                        var he = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                        if (hs == null) hs = hc;
                        if (he == null) he = hc;
                        if (hs.depth != hc.depth ||
                            he.depth != hc.depth ||
                            hs.width != hc.width ||
                            he.width != hc.width)
                        {
                            //ハンチ付き
                            if (gir.isFoundation)
                            {
                                logbuzai = "ハンチ付き基礎" + kind;
                                fami = ConvFamily[0][1];
                            }
                            else
                            {
                                logbuzai = "ハンチ付きRC" + kind;
                                fami = ConvFamily[0][3];
                            }
                        }
                        else
                        {
                            //ハンチなし
                            if (gir.isFoundation)
                            {
                                logbuzai = "基礎" + kind;
                                fami = ConvFamily[0][0];
                            }
                            else
                            {
                                logbuzai = "RC" + kind;
                                fami = ConvFamily[0][2];
                            }
                        }
                        break;
                }
            }
            else
            {
                if (gir.isFoundation)
                {
                    logbuzai = "片持基礎" + kind;
                    fami = ConvFamily[0][0];
                }
                else
                {
                    logbuzai = "片持" + kind;
                    fami = ConvFamily[0][1];
                }
            }

            if (fami == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, logbuzai);
                return ret;
            }

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            FamilySymbol symbol = null;
            for (int h = 0; h < haunch_start.Count(); h++)
            {
                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }
                else
                {
                    if (Data.SearchFamilySymbol(fami, typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(fami, typename, ref symbol));
                    }

                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[h],
                    Length2 = haunch_end[h],
                    BHaunch1 = kind_haunch_start[h],
                    BHaunch2 = kind_haunch_end[h],
                    symbol = symbol
                };
                GirderSymbols.Add(re);

                SetParameter_RCGirder(gir, logbuzai, typename, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);

                if (symbol != null)
                {
                    Data.TypeName_Data td = new Data.TypeName_Data
                    {
                        typename = symbol.Name,
                        id = gir.id,
                        shapename = "RC梁"
                    };
                    typename_list.Add(td);

                    Data.SaveGuid(gir.guid, symbol.Id);
                }
            }

            return ret;
        }

        private static void SetParameter_RCGirder(StbSecBeam_RC gir, string logbuzai, string typename, double haunch_start, double haunch_end, string kind_haunch_start, string kind_haunch_end, FamilySymbol symbol)
        {
            FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;

            string canti = "";
            if (gir.isCanti)
            { canti = "Cantilever-"; }
            if (gir.isFoundation) { canti += "Foundation-"; }
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
            Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);

            Data.SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
            Data.SetParameter(symbol.LookupParameter(Rgir.name), gir.name);

            Data.SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
            Data.SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutin);

            if (haunch_start < 0 && haunch_end < 0)
            {
                //マイナス値ならハンチパラメータだけ更新しない。差分用
            }
            else
            {
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start, true);
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end, true);

                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }
            }


            if (gir.StbSecFigureBeam_RC != null)
            {
                switch (gir.StbSecFigureBeam_RC.FigureType)
                {
                    case 1:
                        var fig1 = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Straight>().FirstOrDefault();
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig1.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig1.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig1.depth, true);
                        break;

                    case 2:
                        var fig2s = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                        var fig2e = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig2s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig2s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig2e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig2s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig2s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig2e.depth, true);
                        break;

                    case 3:
                        var fig3s = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                        var fig3c = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                        var fig3e = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                        if (fig3s == null) fig3s = fig3c;
                        if (fig3e == null) fig3e = fig3c;
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig3s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig3c.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig3e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig3s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig3c.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig3e.depth, true);
                        break;
                }
            }

            if (gir.StbSecBarArrangementBeam_RC == null)
            {
                //鉄筋タグが無いとき→ログ
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[" + logbuzai + "]" + typename + "(断面id=" + gir.id.ToString() + ")");
            }
            else
            {
                StbSecBarBeam_RC_ThreeTypes bar = null;
                List<StbSecBarBeam_RC_ThreeTypes> bar2 = new List<StbSecBarBeam_RC_ThreeTypes>();
                switch (gir.StbSecBarArrangementBeam_RC.Bar_ArrangementType)
                {
                    case 1:
                        bar = new StbSecBarBeam_RC_ThreeTypes(gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_Same>().FirstOrDefault());
                        bar2.Add(bar);
                        bar2.Add(bar);
                        bar2.Add(bar);
                        break;
                    case 2:
                        bar = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>()?.FirstOrDefault(a => a.pos == StbSecBarBeam_RC_ThreeTypesPos.CENTER);
                        bar2 = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>().OrderBy(a => a.pos).ToList();
                        break;
                    case 3:
                        bar = new StbSecBarBeam_RC_ThreeTypes(gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().FirstOrDefault());
                        bar2 = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().OrderBy(a => a.pos).Select(a => new StbSecBarBeam_RC_ThreeTypes(a)).ToList();
                        //中央はなし
                        bar2.Insert(1, null);
                        break;
                }

                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.StbSecBarArrangementBeam_RC.depth_cover_left);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.StbSecBarArrangementBeam_RC.depth_cover_right);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.StbSecBarArrangementBeam_RC.depth_cover_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.StbSecBarArrangementBeam_RC.depth_cover_bottom);
                Data.SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.StbSecBarArrangementBeam_RC.interval);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.StbSecBarArrangementBeam_RC.center_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.StbSecBarArrangementBeam_RC.center_bottom);
                Data.SetParameter(symbol.LookupParameter(Rgir.bar_length_start), gir.StbSecBarArrangementBeam_RC.length_bar_start, true);
                Data.SetParameter(symbol.LookupParameter(Rgir.bar_length_end), gir.StbSecBarArrangementBeam_RC.length_bar_end, true);

                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), bar.strength_stirrup);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), bar.strength_web);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), bar.strength_bar_spacing);


                int N_topX = gir.StbSecBarArrangementBeam_RC.StbSecBarBeamXReinforced?.N_main_top ?? 0;
                int N_btmX = gir.StbSecBarArrangementBeam_RC.StbSecBarBeamXReinforced?.N_main_bottom ?? 0;

                for (int i = 0; i < 3; i++)
                {
                    if (bar2[i] == null) continue;

                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), bar2[i].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), bar2[i].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), bar2[i].D_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), bar2[i].D_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), bar2[i].D_bar_spacing);

                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), bar2[i].N_main_top_1st + N_topX);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), bar2[i].N_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), bar2[i].N_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), bar2[i].N_main_bottom_1st + N_btmX);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), bar2[i].N_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), bar2[i].N_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), bar2[i].N_2nd_main_top_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), bar2[i].N_2nd_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), bar2[i].N_2nd_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), bar2[i].N_2nd_main_bottom_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), bar2[i].N_2nd_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), bar2[i].N_2nd_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), bar2[i].N_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), bar2[i].pitch_stirrup, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_web[i]), bar2[i].N_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), bar2[i].N_bar_spacing);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), bar2[i].pitch_bar_spacing, true);
                }

            }
        }

        /// <summary>
        /// RC片持ち梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateCGirder_RC(ST_BRIDGE stb, StbSecBeam_RC gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;
            Family fami = null;
            string logbuzai = "";
            string kind = "";
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { kind = "梁"; }
            else
            { kind = "小梁"; }

            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[RC梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }


            if (gir.isFoundation)
            {
                logbuzai = "片持基礎" + kind;
                fami = ConvFamily[0][0];
            }
            else
            {
                logbuzai = "RC片持" + kind;
                fami = ConvFamily[0][1];
            }
            if (fami == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2200, logbuzai);
                return ret;
            }

            if (gir.StbSecFigureBeam_RC == null)
            {
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + logbuzai + "]" + typename + "(断面id=" + gir.id.ToString() + ")形状が入力されていない梁");
                return ret;
            }


            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            FamilySymbol symbol = null;
            for (int h = 0; h < haunch_start.Count(); h++)
            {
                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }
                else
                {
                    if (Data.SearchFamilySymbol(fami, typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(fami, typename, ref symbol));
                    }

                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[h],
                    Length2 = haunch_end[h],
                    BHaunch1 = kind_haunch_start[h],
                    BHaunch2 = kind_haunch_end[h],
                    symbol = symbol
                };
                GirderSymbols.Add(re);

                if (!Data.SearchFamilySymbol(fami, typename, ref symbol))
                { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                SetParameter_RCCGirder(gir, logbuzai, symbol.Name, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);


                if (symbol != null)
                {
                    Data.TypeName_Data td = new Data.TypeName_Data
                    {
                        typename = symbol.Name,
                        id = gir.id,
                        shapename = "RC片持梁"
                    };
                    typename_list.Add(td);
                    
                    Data.SaveGuid(gir.guid, symbol.Id);
                }
            }
            return ret;
        }

        private static void SetParameter_RCCGirder(StbSecBeam_RC gir, string logbuzai, string typename, double haunch_start, double haunch_end, string kind_haunch_start, string kind_haunch_end, FamilySymbol symbol)
        {
            FamilyStructure.RC_CGir Rgir = SetFamily.RCCGir;


            string canti = "";
            if (gir.isCanti)
            { canti = "Cantilever-"; }
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
            Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);

            Data.SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
            Data.SetParameter(symbol.LookupParameter(Rgir.name), gir.name);

            Data.SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
            Data.SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutin);

            if (haunch_start < 0 && haunch_end < 0)
            {
                //マイナス値ならハンチパラメータだけ更新しない。差分用
            }
            else
            {
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start, true);
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end, true);

                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }
            }


            if (gir.StbSecFigureBeam_RC != null)
            {
                switch (gir.StbSecFigureBeam_RC.FigureType)
                {
                    case 1:
                        var fig1 = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Straight>().FirstOrDefault();
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig1.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig1.depth, true);
                        break;

                    case 2:
                        var fig2s = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                        var fig2e = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig2s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig2e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig2s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig2e.depth, true);
                        break;

                    case 3:
                        var fig3s = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                        var fig3c = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                        var fig3e = gir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                        if (fig3s == null) fig3s = fig3c;
                        if (fig3e == null) fig3e = fig3c;
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig3s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig3e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig3s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig3e.depth, true);
                        break;
                }
            }


            if (gir.StbSecBarArrangementBeam_RC == null)
            {
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[" + logbuzai + "]" + typename + "(断面id=" + gir.id.ToString() + ")");
            }
            else
            {
                StbSecBarBeam_RC_ThreeTypes bar = null;
                List<StbSecBarBeam_RC_ThreeTypes> bar2 = new List<StbSecBarBeam_RC_ThreeTypes>();
                switch (gir.StbSecBarArrangementBeam_RC.Bar_ArrangementType)
                {
                    case 1:
                        bar = new StbSecBarBeam_RC_ThreeTypes(gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_Same>().FirstOrDefault());
                        bar2.Add(bar);
                        bar2.Add(bar);
                        break;
                    case 2:
                        //中央なし
                        bar = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>()?.FirstOrDefault(a => a.pos == StbSecBarBeam_RC_ThreeTypesPos.START);
                        bar2 = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_ThreeTypes>().Where(a => a.pos != StbSecBarBeam_RC_ThreeTypesPos.CENTER).OrderBy(a => a.pos).ToList();
                        break;
                    case 3:
                        bar = new StbSecBarBeam_RC_ThreeTypes(gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().FirstOrDefault());
                        bar2 = gir.StbSecBarArrangementBeam_RC.Items.OfType<StbSecBarBeam_RC_StartEnd>().OrderBy(a => a.pos).Select(a => new StbSecBarBeam_RC_ThreeTypes(a)).ToList();
                        break;
                }

                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.StbSecBarArrangementBeam_RC.depth_cover_left);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.StbSecBarArrangementBeam_RC.depth_cover_right);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.StbSecBarArrangementBeam_RC.depth_cover_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.StbSecBarArrangementBeam_RC.depth_cover_bottom);
                Data.SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.StbSecBarArrangementBeam_RC.interval);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.StbSecBarArrangementBeam_RC.center_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.StbSecBarArrangementBeam_RC.center_bottom);
                Data.SetParameter(symbol.LookupParameter(Rgir.bar_length_start), gir.StbSecBarArrangementBeam_RC.length_bar_start);
                Data.SetParameter(symbol.LookupParameter(Rgir.bar_length_end), gir.StbSecBarArrangementBeam_RC.length_bar_end);

                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), bar.strength_stirrup);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), bar.strength_web);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), bar.strength_bar_spacing);


                int N_topX = gir.StbSecBarArrangementBeam_RC.StbSecBarBeamXReinforced?.N_main_top ?? 0;
                int N_btmX = gir.StbSecBarArrangementBeam_RC.StbSecBarBeamXReinforced?.N_main_bottom ?? 0;

                for (int i = 0; i < 2; i++)
                {
                    if (bar2[i] == null) continue;

                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), bar2[i].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), bar2[i].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), bar2[i].D_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), bar2[i].D_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), bar2[i].D_bar_spacing);

                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), bar2[i].N_main_top_1st + N_topX);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), bar2[i].N_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), bar2[i].N_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), bar2[i].N_main_bottom_1st + N_btmX);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), bar2[i].N_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), bar2[i].N_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), bar2[i].N_2nd_main_top_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), bar2[i].N_2nd_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), bar2[i].N_2nd_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), bar2[i].N_2nd_main_bottom_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), bar2[i].N_2nd_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), bar2[i].N_2nd_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), bar2[i].N_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), bar2[i].pitch_stirrup, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_web[i]), bar2[i].N_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), bar2[i].N_bar_spacing);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), bar2[i].pitch_bar_spacing, true);
                }

            }

        }





        /// <summary>
        /// S梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateGirder_S(ST_BRIDGE stb, StbSecBeam_S gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;

            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[S梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


            //鉄骨形状を取得
            int[] shapeids = new int[3];
            GetSteelShapeData(gir.StbSecSteelFigureBeam_S.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);

            string shape = "";
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
                            continue;
                        }
                        Data.MakeTekkotuLog("S梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }

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
                Data.Make_taisyougaiLog("S梁", gir.id, gir.name, shape, shapename_J);
                return ret;
            }
            else
            {
                LogData.AddLog(LogData.LogKind.Warning, 2500, "[S梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")の鉄骨形状[" + steel_shapes[0] + "]");
                return ret;
            }


            FamilySymbol symbol = null;
            switch (shape)
            {
                case RevitLNK.st_steel_H:
                    bool shapeflg = gir.StbSecSteelFigureBeam_S.FigureType == 1;
                    Family fami = null;
                    string logbuzai = "";
                    string kind = "";
                    if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                    {
                        kind = "大梁";
                    }
                    else
                    {
                        kind = "小梁";
                    }

                    if (shapeflg)
                    {
                        logbuzai = "S" + kind;
                        fami = ConvFamily[1][0];
                    }
                    else
                    {
                        logbuzai = "ハンチ付きS" + kind;
                        fami = ConvFamily[1][5];
                    }

                    if (fami == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2100, logbuzai);
                        return ret;
                    }

                    symbol = null;
                    if (Data.SearchFamilySymbol(fami, typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(fami, typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);


                    for (int i = 0; i < haunch_start.Count(); i++)
                    {
                        if (i != 0)
                        {
                            string newtypename = typename + "_" + i.ToString();
                            symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                        }

                        for (int j = 0; j < 3; j++)
                        {
                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                            if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, gir,
                                                                        shape_, shapename_J,
                                                                        steel_shapes[j], strength_main[j], strength_web[j]))
                            {
                                return false;
                            }
                        }

                        SetParameter_SGirder_H2(0, gir, haunch_start[i], haunch_end[i], symbol);

                        Data.ReNameSymbols re = new Data.ReNameSymbols
                        {
                            name = typename,
                            id = gir.id,
                            Length = haunch_start[i],
                            Length2 = haunch_end[i],
                            BHaunch1 = kind_haunch_start[i],
                            BHaunch2 = kind_haunch_start[i],
                            symbol = symbol
                        };
                        GirderSymbols.Add(re);
                    }
                    break;

                case RevitLNK.st_steel_BH:

                    if (ConvFamily[1][1] == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S梁組立H形鋼");
                        return ret;
                    }
                    symbol = null;
                    if (Data.SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    for (int i = 0; i < haunch_start.Count(); i++)
                    {
                        if (i != 0)
                        {
                            typename = typename + "_" + i.ToString();
                            symbol = (FamilySymbol)symbol.Duplicate(typename);
                        }
                        for (int j = 0; j < 3; j++)
                        {
                            string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                            if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], j, gir,
                                                                        shape_, shapename_J,
                                                                        steel_shapes[j], strength_main[j], strength_web[j]))
                            {
                                return false;
                            }
                        }

                        SetParameter_SGirder_H2(1, gir, haunch_start[i], haunch_end[i], symbol);

                        Data.ReNameSymbols re = new Data.ReNameSymbols
                        {
                            name = typename,
                            id = gir.id,
                            Length = haunch_start[i],
                            Length2 = haunch_end[i],
                            BHaunch1 = kind_haunch_start[i],
                            BHaunch2 = kind_haunch_start[i],
                            symbol = symbol
                        };
                        GirderSymbols.Add(re);
                    }
                    break;

                case RevitLNK.st_steel_C:
                    if (ConvFamily[1][2] == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S梁溝形鋼");
                        return ret;
                    }
                    symbol = null;
                    if (Data.SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                    {
                        if (ind[1] == 0)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        var steel_C = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeids[j]];

                        if (!SetParameter_Girder_and_CGirder_C(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                               steel_C, gir, shapename_J, steel_shapes[j], strength_main[j]))
                        {
                            return ret;
                        }
                    }
                    break;

                case RevitLNK.st_steel_L:
                    if (ConvFamily[1][3] == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S梁山形鋼");
                        return ret;
                    }
                    symbol = null;
                    if (Data.SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                    {
                        if (ind[1] == 0)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        var steel_L = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeids[j]];
                        if (!SetParameter_Girder_and_CGirder_L(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                               steel_L, gir, shapename_J, steel_shapes[j], strength_main[j]))
                        {
                            return ret;
                        }
                    }
                    break;

                case RevitLNK.st_steel_LipC:
                    if (ConvFamily[1][4] == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S梁リップ溝形鋼");
                        return ret;
                    }
                    symbol = null;
                    if (Data.SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                    {
                        int ascii = 97;
                        string oldname = typename;
                        do
                        {
                            typename = Data.ReName(oldname, ascii);
                            ascii++;
                        } while (Data.SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                    {
                        if (ind[1] == 0)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { Data.MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        var steel_LipC = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[shapeids[j]];

                        if (!SetParameter_Girder_and_CGirder_LipC(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                                  steel_LipC, gir, shapename_J, steel_shapes[j], strength_main[j]))
                        {
                            return ret;
                        }
                    }
                    break;
            }

            if (symbol != null)
            {
                Data.TypeName_Data td = new Data.TypeName_Data
                {
                    typename = symbol.Name,
                    id = gir.id,
                    shapename = "S梁"
                };
                typename_list.Add(td);
                
                Data.SaveGuid(gir.guid, symbol.Id);
            }

            return ret;
        }


        /// <summary>
        /// S片持ち梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateCGirder_S(ST_BRIDGE stb, StbSecBeam_S gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;

            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[S梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }


            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


            //鉄骨形状を取得
            int[] shapeids = new int[3];
            GetSteelShapeData(gir.StbSecSteelFigureBeam_S.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);

            string shape = "";
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
                            continue;
                        }
                        Data.MakeTekkotuLog("S片持梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }

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
                Data.Make_taisyougaiLog("S片持梁", gir.id, gir.name, shape, shapename_J);
                return ret;
            }
            else
            {
                LogData.AddLog(LogData.LogKind.Warning, 2500, "[S片持梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")の鉄骨形状[" + steel_shapes[0] + "]");
                return ret;
            }


            FamilySymbol symbol = null;
            if (shape == RevitLNK.st_steel_H)
            {
                if (ConvFamily[1][0] == null)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "S片持梁" + shapename_J);
                    return ret;
                }

                symbol = null;
                if (Data.SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                {
                    int ascii = 97;
                    string oldname = typename;
                    do
                    {
                        typename = Data.ReName(oldname, ascii);
                        ascii++;
                    } while (Data.SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);


                for (int i = 0; i < haunch_start.Count(); i++)
                {
                    if (i != 0)
                    {
                        string newtypename = typename + "_" + i.ToString();
                        symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                    }

                    int jj = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        //中央は使わない
                        if (j == 1) continue;

                        string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                        if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, gir,
                                                                    shape_, shapename_J,
                                                                    steel_shapes[j], strength_main[j], strength_web[j]))
                        {
                            return false;
                        }

                        jj++;
                    }

                    SetParameter_SGirder_H2(2, gir, haunch_start[i], haunch_end[i], symbol);

                    Data.ReNameSymbols re = new Data.ReNameSymbols
                    {
                        name = typename,
                        id = gir.id,
                        Length = haunch_start[i],
                        Length2 = haunch_end[i],
                        BHaunch1 = kind_haunch_start[i],
                        BHaunch2 = kind_haunch_start[i],
                        symbol = symbol
                    };
                    GirderSymbols.Add(re);
                }
            }
            else if (shape == RevitLNK.st_steel_BH)
            {
                if (ConvFamily[1][1] == null)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "S片持梁組立H形鋼");
                    return ret;
                }

                symbol = null;
                if (Data.SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                {
                    int ascii = 97;
                    string oldname = typename;
                    do
                    {
                        typename = Data.ReName(oldname, ascii);
                        ascii++;
                    } while (Data.SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);


                for (int i = 0; i < haunch_start.Count(); i++)
                {
                    if (i != 0)
                    {
                        typename = typename + "_" + i.ToString();
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    int jj = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        //中央は使わない
                        if (j == 1) continue;

                        string shape_ = Check_Steel(stb, steel_shapes[j], out shapeids[j]);
                        if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, shapeids[j], jj, gir,
                                                                    shape_, shapename_J,
                                                                    steel_shapes[j], strength_main[j], strength_web[j]))
                        {
                            return false;
                        }

                        jj++;
                    }

                    SetParameter_SGirder_H2(3, gir, haunch_start[i], haunch_end[i], symbol);


                    Data.ReNameSymbols re = new Data.ReNameSymbols
                    {
                        name = typename,
                        id = gir.id,
                        Length = haunch_start[i],
                        Length2 = haunch_end[i],
                        BHaunch1 = kind_haunch_start[i],
                        BHaunch2 = kind_haunch_start[i],
                        symbol = symbol
                    };
                    GirderSymbols.Add(re);
                }
            }
            else if (shape == RevitLNK.st_steel_C)
            {
                if (ConvFamily[1][2] == null)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "S片持梁溝形鋼");
                    return ret;
                }
                symbol = null;
                if (Data.SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                {
                    int ascii = 97;
                    string oldname = typename;
                    do
                    {
                        typename = Data.ReName(oldname, ascii);
                        ascii++;
                    } while (Data.SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                {
                    if (ind[1] == 0)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                for (int j = 0; j < 3; j++)
                {
                    var steel_C = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeids[j]];

                    if (!SetParameter_Girder_and_CGirder_C(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                           steel_C, gir, shapename_J, steel_shapes[j], strength_main[j]))
                    {
                        return ret;
                    }
                }
            }
            else if (shape == RevitLNK.st_steel_L)
            {
                if (ConvFamily[1][3] == null)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "S片持梁山形鋼");
                    return ret;
                }
                symbol = null;
                if (Data.SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                {
                    int ascii = 97;
                    string oldname = typename;
                    do
                    {
                        typename = Data.ReName(oldname, ascii);
                        ascii++;
                    } while (Data.SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                {
                    if (ind[1] == 0)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                for (int j = 0; j < 3; j++)
                {
                    var steel_L = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeids[j]];
                    if (!SetParameter_Girder_and_CGirder_L(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                           steel_L, gir, shapename_J, steel_shapes[j], strength_main[j]))
                    {
                        return ret;
                    }
                }
            }
            else if (shape == RevitLNK.st_steel_LipC)
            {
                if (ConvFamily[1][4] == null)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "S片持梁リップ溝形鋼");
                    return ret;
                }

                if (Data.SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                {
                    int ascii = 97;
                    string oldname = typename;
                    do
                    {
                        typename = Data.ReName(oldname, ascii);
                        ascii++;
                    } while (Data.SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelFigureBeam_S.FigureType != 1)
                {
                    if (ind[1] == 0)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { Data.MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                for (int j = 0; j < 3; j++)
                {
                    var steel_LipC = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[shapeids[j]];

                    if (!SetParameter_Girder_and_CGirder_LipC(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end,
                                                              steel_LipC, gir, shapename_J, steel_shapes[j], strength_main[j]))
                    {
                        return ret;
                    }
                }
            }

            if (symbol != null)
            {
                Data.TypeName_Data td = new Data.TypeName_Data
                {
                    typename = symbol.Name,
                    id = gir.id,
                    shapename = "S片持梁"
                };
                typename_list.Add(td);
                
                Data.SaveGuid(gir.guid, symbol.Id);
            }

            return ret;
        }

        /// <summary>
        /// S梁H,BH,片持ちH,BH のパラメータ設定2
        /// </summary>
        /// <param name="m"></param>
        /// <param name="gir"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="symbol"></param>
        private static void SetParameter_SGirder_H2(int m, StbSecBeam_S gir, double haunch_start, double haunch_end, FamilySymbol symbol)
        {
            string[] p_names = null;
            switch (m)
            {
                case 0: //H
                    p_names = new string[]
                    {
                        SetFamily.SGirH.SecId,
                        SetFamily.SGirH.kind_beam,
                        SetFamily.SGirH.kind_beam2,
                        SetFamily.SGirH.haunch_start,
                        SetFamily.SGirH.haunch_end,
                        SetFamily.SGirH.name,
                        SetFamily.SGirH.isOutIn,
                    };
                    break;
                case 1: //BH
                    p_names = new string[]
                    {
                        SetFamily.SGirBH.SecId,
                        SetFamily.SGirBH.kind_beam,
                        SetFamily.SGirBH.kind_beam2,
                        SetFamily.SGirBH.haunch_start,
                        SetFamily.SGirBH.haunch_end,
                        SetFamily.SGirBH.name,
                        SetFamily.SGirBH.isOutIn,
                    };
                    break;
                case 2: //H 片持ち
                    p_names = new string[]
                    {
                        SetFamily.SCGirH.SecId,
                        SetFamily.SCGirH.kind_beam,
                        SetFamily.SCGirH.kind_beam2,
                        SetFamily.SCGirH.haunch_start,
                        SetFamily.SCGirH.haunch_end,
                        SetFamily.SCGirH.name,
                        SetFamily.SCGirH.isOutIn,
                    };
                    break;
                case 3: //BH 片持ち
                    p_names = new string[]
                    {
                        SetFamily.SCGirBH.SecId,
                        SetFamily.SCGirBH.kind_beam,
                        SetFamily.SCGirBH.kind_beam2,
                        SetFamily.SCGirBH.haunch_start,
                        SetFamily.SCGirBH.haunch_end,
                        SetFamily.SCGirBH.name,
                        SetFamily.SCGirBH.isOutIn,
                    };
                    break;
            }


            FamilyStructure.S_Gir_H sgir = SetFamily.SGirH;
            Data.SetParameter(symbol.LookupParameter(p_names[0]), gir.id);
            string canti = "";
            if (gir.isCanti)
            { canti = "Cantilever-"; }
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { Data.SetParameter(symbol.LookupParameter(p_names[1]), canti + "Girder"); }
            else
            { Data.SetParameter(symbol.LookupParameter(p_names[1]), canti + "Beam"); }
            Data.SetParameter(symbol.LookupParameter(p_names[2]), gir.kind_beam);
            if (haunch_start < 0 && haunch_end < 0)
            {
                //マイナス値ならハンチパラメータだけ更新しない。差分用
            }
            else
            {
                Data.SetParameter(symbol.LookupParameter(p_names[3]), haunch_start, true);
                Data.SetParameter(symbol.LookupParameter(p_names[4]), haunch_end, true);
            }
            Data.SetParameter(symbol.LookupParameter(p_names[5]), gir.name);
            Data.SetParameter(symbol.LookupParameter(p_names[6]), gir.isOutin);
        }



        /// <summary>
        /// S梁,Sブレースの鉄骨形状,強度を取得
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="ind">ItemsのIndex</param>
        /// <param name="steel_shapes">鉄骨形状名称</param>
        /// <param name="strength_main">鉄骨強度</param>
        /// <param name="strength_web">鉄骨強度</param>
        private static void GetSteelShapeData(List<object> Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web)
        {
            ind = new int[3];
            steel_shapes = new string[3];
            strength_main = new string[3];
            strength_web = new string[3];

            if (Items.First() is StbSecSteelBeam_S_Straight)
            {
                var fig_s = Items.OfType<StbSecSteelBeam_S_Straight>().FirstOrDefault();
                ind[0] = 0;
                ind[1] = 0;
                ind[2] = 0;
                steel_shapes[0] = fig_s.shape;
                steel_shapes[1] = fig_s.shape;
                steel_shapes[2] = fig_s.shape;
                strength_main[0] = fig_s.strength_main;
                strength_main[1] = fig_s.strength_main;
                strength_main[2] = fig_s.strength_main;
                strength_web[0] = fig_s.strength_web;
                strength_web[1] = fig_s.strength_web;
                strength_web[2] = fig_s.strength_web;
            }
            else if (Items.First() is StbSecSteelBeam_S_Taper)
            {
                var fig_t = Items.OfType<StbSecSteelBeam_S_Taper>().ToList();
                ind[0] = fig_t.FindIndex(a => a.pos == StbSecSteelBeam_S_TaperPos.START);
                ind[1] = ind[0];
                ind[2] = fig_t.FindIndex(a => a.pos == StbSecSteelBeam_S_TaperPos.END);
                steel_shapes[0] = fig_t[ind[0]].shape;
                steel_shapes[1] = fig_t[ind[1]].shape;
                steel_shapes[2] = fig_t[ind[2]].shape;
                strength_main[0] = fig_t[ind[0]].strength_main;
                strength_main[1] = fig_t[ind[1]].strength_main;
                strength_main[2] = fig_t[ind[2]].strength_main;
                strength_web[0] = fig_t[ind[0]].strength_web;
                strength_web[1] = fig_t[ind[1]].strength_web;
                strength_web[2] = fig_t[ind[2]].strength_web;
            }
            else if (Items.First() is StbSecSteelBeam_S_Joint)
            {
                var fig_j = Items.OfType<StbSecSteelBeam_S_Joint>().ToList();
                ind[0] = fig_j.FindIndex(a => a.pos == StbSecSteelBeam_S_JointPos.START);
                ind[1] = fig_j.FindIndex(a => a.pos == StbSecSteelBeam_S_JointPos.CENTER);
                ind[2] = fig_j.FindIndex(a => a.pos == StbSecSteelBeam_S_JointPos.END);
                if (ind[0] == -1) ind[0] = ind[1];
                if (ind[2] == -1) ind[2] = ind[1];
                steel_shapes[0] = fig_j[ind[0]].shape;
                steel_shapes[1] = fig_j[ind[1]].shape;
                steel_shapes[2] = fig_j[ind[2]].shape;
                strength_main[0] = fig_j[ind[0]].strength_main;
                strength_main[1] = fig_j[ind[1]].strength_main;
                strength_main[2] = fig_j[ind[2]].strength_main;
                strength_web[0] = fig_j[ind[0]].strength_web;
                strength_web[1] = fig_j[ind[1]].strength_web;
                strength_web[2] = fig_j[ind[2]].strength_web;
            }
            else if (Items.First() is StbSecSteelBeam_S_Haunch)
            {
                var fig_h = Items.OfType<StbSecSteelBeam_S_Haunch>().ToList();
                ind[0] = fig_h.FindIndex(a => a.pos == StbSecSteelBeam_S_HaunchPos.START);
                ind[1] = fig_h.FindIndex(a => a.pos == StbSecSteelBeam_S_HaunchPos.CENTER);
                ind[2] = fig_h.FindIndex(a => a.pos == StbSecSteelBeam_S_HaunchPos.END);
                if (ind[0] == -1) ind[0] = ind[1];
                if (ind[2] == -1) ind[2] = ind[1];
                steel_shapes[0] = fig_h[ind[0]].shape;
                steel_shapes[1] = fig_h[ind[1]].shape;
                steel_shapes[2] = fig_h[ind[2]].shape;
                strength_main[0] = fig_h[ind[0]].strength_main;
                strength_main[1] = fig_h[ind[1]].strength_main;
                strength_main[2] = fig_h[ind[2]].strength_main;
                strength_web[0] = fig_h[ind[0]].strength_web;
                strength_web[1] = fig_h[ind[1]].strength_web;
                strength_web[2] = fig_h[ind[2]].strength_web;
            }
            else if (Items.First() is StbSecSteelBeam_S_FiveTypes)
            {
                var fig_5 = Items.OfType<StbSecSteelBeam_S_FiveTypes>().ToList();
                ind[0] = fig_5.FindIndex(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.START);
                ind[1] = fig_5.FindIndex(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER);
                ind[2] = fig_5.FindIndex(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.END);
                if (ind[0] == -1) ind[0] = ind[1];
                if (ind[2] == -1) ind[2] = ind[1];
                steel_shapes[0] = fig_5[ind[0]].shape;
                steel_shapes[1] = fig_5[ind[1]].shape;
                steel_shapes[2] = fig_5[ind[2]].shape;
                strength_main[0] = fig_5[ind[0]].strength_main;
                strength_main[1] = fig_5[ind[1]].strength_main;
                strength_main[2] = fig_5[ind[2]].strength_main;
                strength_web[0] = fig_5[ind[0]].strength_web;
                strength_web[1] = fig_5[ind[1]].strength_web;
                strength_web[2] = fig_5[ind[2]].strength_web;
            }

            else if (Items.First() is StbSecSteelBrace_S_Same)
            {
                var fig_s = Items.OfType<StbSecSteelBrace_S_Same>().FirstOrDefault();
                ind[0] = 0;
                ind[1] = 0;
                ind[2] = 0;
                steel_shapes[0] = fig_s.shape;
                steel_shapes[1] = fig_s.shape;
                steel_shapes[2] = fig_s.shape;
                strength_main[0] = fig_s.strength_main;
                strength_main[1] = fig_s.strength_main;
                strength_main[2] = fig_s.strength_main;
                strength_web[0] = fig_s.strength_web;
                strength_web[1] = fig_s.strength_web;
                strength_web[2] = fig_s.strength_web;
            }
            else if (Items.First() is StbSecSteelBrace_S_NotSame)
            {
                var fig_n = Items.OfType<StbSecSteelBrace_S_NotSame>().ToList();
                ind[0] = fig_n.FindIndex(a => a.pos == StbSecSteelBrace_S_NotSamePos.BOTTOM);
                ind[1] = ind[0];
                ind[2] = fig_n.FindIndex(a => a.pos == StbSecSteelBrace_S_NotSamePos.TOP);
                steel_shapes[0] = fig_n[ind[0]].shape;
                steel_shapes[1] = fig_n[ind[1]].shape;
                steel_shapes[2] = fig_n[ind[2]].shape;
                strength_main[0] = fig_n[ind[0]].strength_main;
                strength_main[1] = fig_n[ind[1]].strength_main;
                strength_main[2] = fig_n[ind[2]].strength_main;
                strength_web[0] = fig_n[ind[0]].strength_web;
                strength_web[1] = fig_n[ind[1]].strength_web;
                strength_web[2] = fig_n[ind[2]].strength_web;
            }
            else if (Items.First() is StbSecSteelBrace_S_ThreeTypes)
            {
                var fig_3 = Items.OfType<StbSecSteelBrace_S_ThreeTypes>().ToList();
                ind[0] = fig_3.FindIndex(a => a.pos == StbSecSteelBrace_S_ThreeTypesPos.BOTTOM);
                ind[1] = fig_3.FindIndex(a => a.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER);
                ind[2] = fig_3.FindIndex(a => a.pos == StbSecSteelBrace_S_ThreeTypesPos.TOP);
                if (ind[0] == -1) ind[0] = ind[1];
                if (ind[2] == -1) ind[2] = ind[1];
                steel_shapes[0] = fig_3[ind[0]].shape;
                steel_shapes[1] = fig_3[ind[1]].shape;
                steel_shapes[2] = fig_3[ind[2]].shape;
                strength_main[0] = fig_3[ind[0]].strength_main;
                strength_main[1] = fig_3[ind[1]].strength_main;
                strength_main[2] = fig_3[ind[2]].strength_main;
                strength_web[0] = fig_3[ind[0]].strength_web;
                strength_web[1] = fig_3[ind[1]].strength_web;
                strength_web[2] = fig_3[ind[2]].strength_web;
            }


            strength_web[0] = GetStrength_web(strength_web[0], strength_main[0]);
            strength_web[1] = GetStrength_web(strength_web[1], strength_main[1]);
            strength_web[2] = GetStrength_web(strength_web[2], strength_main[2]);

        }

        /// <summary>
        /// S梁,Sブレースの鉄骨形状を取得
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="steel_shapes">鉄骨形状名称</param>
        private static void GetSteelShapeData(object Item, out string steel_shapes)
        {
            steel_shapes = "";

            if (Item is StbSecSteelBeam_S_Straight straight)
            {
                steel_shapes = straight.shape;
            }
            else if (Item is StbSecSteelBeam_S_Taper taper)
            {
                steel_shapes = taper.shape;
            }
            else if (Item is StbSecSteelBeam_S_Joint joint)
            {
                steel_shapes = joint.shape;
            }
            else if (Item is StbSecSteelBeam_S_Haunch haunch)
            {
                steel_shapes = haunch.shape;
            }
            else if (Item is StbSecSteelBeam_S_FiveTypes five)
            {
                steel_shapes = five.shape;
            }

            else if (Item is StbSecSteelBrace_S_Same same)
            {
                steel_shapes = same.shape;
            }
            else if (Item is StbSecSteelBrace_S_NotSame notsame)
            {
                steel_shapes = notsame.shape;
            }
            else if (Item is StbSecSteelBrace_S_ThreeTypes three)
            {
                steel_shapes = three.shape;
            }


        }



        /// <summary>
        /// SRC梁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateGirder_SRC(ST_BRIDGE stb, StbSecBeam_SRC gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;

            string shapename = "SRC梁";
            if (ConvFamily[2][0] == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, shapename);
                return ret;
            }


            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[SRC梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


            //鉄骨形状を取得
            int[] shapeids = new int[3];
            GetSteelShapeData(gir.StbSecSteelFigureBeam_SRC.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);


            string shape = "";
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
                            continue;
                        }
                        Data.MakeTekkotuLog("SRC梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }


            FamilySymbol symbol = null;
            if (Data.SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol))
            {
                int ascii = 97;
                string oldname = typename;
                do
                {
                    typename = Data.ReName(oldname, ascii);
                    ascii++;
                } while (Data.SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol));
            }
            symbol = (FamilySymbol)symbol.Duplicate(typename);

            FamilyStructure.SRC_Gir Rgir = SetFamily.SRCGirH;
            for (int h = 0; h < haunch_start.Count; h++)
            {
                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[h],
                    Length2 = haunch_end[h],
                    BHaunch1 = kind_haunch_start[h],
                    BHaunch2 = kind_haunch_end[h],
                    symbol = symbol,
                };
                GirderSymbols.Add(re);


                //鉄骨
                switch (shape)
                {
                    case RevitLNK.st_steel_H:
                    case RevitLNK.st_steel_BH:
                        SetParameter_SRCGirder_S(stb, gir, steel_shapes, strength_main, strength_web, shape, symbol);
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
                        Data.Make_taisyougaiLog("SRC梁", gir.id, gir.name, shape, shapename_J);
                        return ret;
                }

                //RC部
                SetParameter_SRCGirder_RC(gir, typename, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);

                if (symbol != null)
                {
                    Data.TypeName_Data td = new Data.TypeName_Data
                    {
                        typename = symbol.Name,
                        id = gir.id,
                        shapename = "SRC梁"
                    };
                    typename_list.Add(td);

                    Data.SaveGuid(gir.guid, symbol.Id);
                }
            }

            return ret;
        }



        /// <summary> SRC片持梁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="ConvFamily"></param>
        /// <param name="typename_list"></param>
        /// <returns></returns>
        private static bool CreateCGirder_SRC(ST_BRIDGE stb, StbSecBeam_SRC gir, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;

            string shapename = "SRC片持梁";
            if (ConvFamily[2][0] == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, shapename);
                return ret;
            }

            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[SRC梁](断面id=" + gir.id.ToString() + ")");
                return ret;
            }


            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);


            //鉄骨形状を取得
            int[] shapeids = new int[3];
            GetSteelShapeData(gir.StbSecSteelFigureBeam_SRC.Items, out int[] ind, out string[] steel_shapes, out string[] strength_main, out string[] strength_web);


            string shape = "";
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
                            continue;
                        }
                        Data.MakeTekkotuLog("SRC片持梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }


            FamilySymbol symbol = null;
            if (Data.SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol))
            {
                int ascii = 97;
                string oldname = typename;
                do
                {
                    typename = Data.ReName(oldname, ascii);
                    ascii++;
                } while (Data.SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol));
            }
            symbol = (FamilySymbol)symbol.Duplicate(typename);

            FamilyStructure.SRC_CGir Rgir = SetFamily.SRCCGirH;
            for (int h = 0; h < haunch_start.Count(); h++)
            {
                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[h],
                    Length2 = haunch_end[h],
                    BHaunch1 = kind_haunch_start[h],
                    BHaunch2 = kind_haunch_end[h],
                    symbol = symbol
                };
                GirderSymbols.Add(re);


                //鉄骨
                switch (shape)
                {
                    case RevitLNK.st_steel_H:
                    case RevitLNK.st_steel_BH:
                        SetParameter_SRCGirder_S(stb, gir, steel_shapes, strength_main, strength_web, shape, symbol);
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
                        Data.Make_taisyougaiLog("SRC片持梁", gir.id, gir.name, shape, shapename_J);
                        return ret;
                }


                //RC部
                SetParameter_SRCGirder_RC(gir, typename, haunch_start[h], haunch_end[h], kind_haunch_start[h], kind_haunch_end[h], symbol);


                if (symbol != null)
                {
                    Data.TypeName_Data td = new Data.TypeName_Data
                    {
                        typename = symbol.Name,
                        id = gir.id,
                        shapename = "SRC片持梁"
                    };
                    typename_list.Add(td);
                    
                    Data.SaveGuid(gir.guid, symbol.Id);
                }
            }

            return ret;
        }


        private static void SetParameter_SRCGirder_S(ST_BRIDGE stb, StbSecBeam_SRC gir, string[] steel_shapes, string[] strength_main, string[] strength_web, string shape, FamilySymbol symbol)
        {
            FamilyStructure.SRC_Gir Rgir = null;

            if (gir.isCanti)
            {
                Rgir = new FamilyStructure.SRC_Gir()
                {
                    strength_main = SetFamily.SRCCGirH.strength_main,
                    strength_web = SetFamily.SRCCGirH.strength_web,
                    shape = SetFamily.SRCCGirH.shape,
                    A = SetFamily.SRCCGirH.A,
                    B = SetFamily.SRCCGirH.B,
                    t1 = SetFamily.SRCCGirH.t1,
                    t2 = SetFamily.SRCCGirH.t2,
                    type = SetFamily.SRCCGirH.type,
                    r = SetFamily.SRCCGirH.r,
                };
            }
            else
            {
                Rgir = SetFamily.SRCGirH;
            }

            string log = gir.isCanti ? "SRC片持梁" : "SRC梁";

            double steel_size = 0;
            for (int j = 0; j < 3; j++)
            {
                var steel_H = stb.StbModel.StbSections.StbSecSteel.StbSecRollH?.Find(a => a.name == steel_shapes[j]);
                if (steel_H != null)
                {
                    string logtxt = Roll_H_Size_Check(steel_H);
                    if (logtxt != "")
                    {
                        Data.MakeSizeLog(log + shape, symbol.Name, gir.id, logtxt, 0);
                        continue;
                    }

                    Data.SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), strength_main[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), strength_web[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.shape[j]), steel_shapes[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_H.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_H.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_H.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_H.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.type[j]), steel_H.type.ToString());

                    double r = steel_H.r;
                    if (steel_H.r < 1)
                    {
                        r = 1;
                        Data.MakeSizeLog(log + shape, symbol.Name, gir.id, "フィレット半径", 1);
                    }
                    Data.SetParameter(symbol.LookupParameter(Rgir.r[j]), r, true);

                    if (j == 1)
                    {
                        steel_size = steel_H.A;
                    }
                }
                else
                {
                    var steel_BH = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH?.Find(a => a.name == steel_shapes[j]);

                    string logtxt = Build_H_Size_Check(steel_BH);
                    if (logtxt != "")
                    {
                        Data.MakeSizeLog(log + shape, symbol.Name, gir.id, logtxt, 0);
                        continue;
                    }

                    Data.SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), strength_main[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), strength_web[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.shape[j]), steel_shapes[j]);
                    Data.SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_BH.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_BH.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_BH.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_BH.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.r[j]), 0.0, true);

                    if (j == 1)
                    {
                        steel_size = steel_BH.A;
                    }
                }
            }

            if (gir.StbSecSteelFigureBeam_SRC.level < 0 && steel_size > 1)
            {
                //2.0では部材座標系の正方向が正、天端から下がるのでマイナス値が入っている
                double rc_size = 0;
                switch (gir.StbSecFigureBeam_SRC.FigureType)
                {
                    case 1:
                        var fig1 = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Straight>().FirstOrDefault();
                        rc_size = fig1.depth;
                        break;

                    case 2:
                        var fig2s = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault();
                        rc_size = fig2s.depth;
                        break;

                    case 3:
                        var fig3c = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                        rc_size = fig3c.depth;
                        break;
                }

                //RCとSの寸法差
                double d2 = (rc_size - steel_size) / 2;
                //中心からの距離に換算（＋なら上に鉄骨が移動、－なら下に鉄骨が移動）
                double d3 = d2 + gir.StbSecSteelFigureBeam_SRC.level;
                Data.SetParameter(symbol.LookupParameter(Rgir.level), d3, true);
            }

            Data.SetParameter(symbol.LookupParameter(Rgir.offset), gir.StbSecSteelFigureBeam_SRC.offset, true);


        }

        private static void SetParameter_SRCGirder_RC(StbSecBeam_SRC gir, string typename, double haunch_start, double haunch_end, string kind_haunch_start, string kind_haunch_end, FamilySymbol symbol)
        {
            FamilyStructure.SRC_Gir Rgir = null;

            if (gir.isCanti)
            {
                Rgir = new FamilyStructure.SRC_Gir()
                {
                    kind_beam = SetFamily.SRCCGirH.kind_beam,
                    kind_beam2 = SetFamily.SRCCGirH.kind_beam2,
                    SecId = SetFamily.SRCCGirH.SecId,
                    name = SetFamily.SRCCGirH.name,
                    strength_concrete = SetFamily.SRCCGirH.strength_concrete,
                    isOutIn = SetFamily.SRCCGirH.isOutIn,
                    haunch_start = SetFamily.SRCCGirH.haunch_start,
                    haunch_end = SetFamily.SRCCGirH.haunch_end,
                    BHaunch = SetFamily.SRCCGirH.BHaunch,

                    width_start = SetFamily.SRCCGirH.width_start,
                    width_center = SetFamily.SRCCGirH.width_center,
                    width_end = SetFamily.SRCCGirH.width_end,
                    depth_start = SetFamily.SRCCGirH.depth_start,
                    depth_center = SetFamily.SRCCGirH.depth_center,
                    depth_end = SetFamily.SRCCGirH.depth_end,

                    depth_cover_left = SetFamily.SRCCGirH.depth_cover_left,
                    depth_cover_right = SetFamily.SRCCGirH.depth_cover_right,
                    depth_cover_top = SetFamily.SRCCGirH.depth_cover_top,
                    depth_cover_bottom = SetFamily.SRCCGirH.depth_cover_bottom,
                    center_reinforcement_top = SetFamily.SRCCGirH.center_reinforcement_top,
                    center_reinforcement_bottom = SetFamily.SRCCGirH.center_reinforcement_bottom,

                    strength_reinforcement_main = SetFamily.SRCCGirH.strength_reinforcement_main,
                    strength_reinforcement_2nd_main = SetFamily.SRCCGirH.strength_reinforcement_2nd_main,
                    strength_stirrup = SetFamily.SRCCGirH.strength_stirrup,
                    strength_reinforcement_web = SetFamily.SRCCGirH.strength_reinforcement_web,
                    strength_bar_spacing = SetFamily.SRCCGirH.strength_bar_spacing,

                    D_reinforcement_main_top = SetFamily.SRCCGirH.D_reinforcement_main_top,
                    D_reinforcement_main_bottom = SetFamily.SRCCGirH.D_reinforcement_main_bottom,
                    D_reinforcement_2nd_main_top = SetFamily.SRCCGirH.D_reinforcement_2nd_main_top,
                    D_reinforcement_2nd_main_bottom = SetFamily.SRCCGirH.D_reinforcement_2nd_main_bottom,
                    D_stirrup = SetFamily.SRCCGirH.D_stirrup,
                    D_reinforcement_web = SetFamily.SRCCGirH.D_reinforcement_web,
                    D_bar_spacing = SetFamily.SRCCGirH.D_bar_spacing,

                    count_main_top_1st = SetFamily.SRCCGirH.count_main_top_1st,
                    count_main_top_2nd = SetFamily.SRCCGirH.count_main_top_2nd,
                    count_main_top_3rd = SetFamily.SRCCGirH.count_main_top_3rd,
                    count_main_bottom_1st = SetFamily.SRCCGirH.count_main_bottom_1st,
                    count_main_bottom_2nd = SetFamily.SRCCGirH.count_main_bottom_2nd,
                    count_main_bottom_3rd = SetFamily.SRCCGirH.count_main_bottom_3rd,
                    count_2nd_main_top_1st = SetFamily.SRCCGirH.count_2nd_main_top_1st,
                    count_2nd_main_top_2nd = SetFamily.SRCCGirH.count_2nd_main_top_2nd,
                    count_2nd_main_top_3rd = SetFamily.SRCCGirH.count_2nd_main_top_3rd,
                    count_2nd_main_bottom_1st = SetFamily.SRCCGirH.count_2nd_main_bottom_1st,
                    count_2nd_main_bottom_2nd = SetFamily.SRCCGirH.count_2nd_main_bottom_2nd,
                    count_2nd_main_bottom_3rd = SetFamily.SRCCGirH.count_2nd_main_bottom_3rd,

                    count_stirrup = SetFamily.SRCCGirH.count_stirrup,
                    pitch_stirrup = SetFamily.SRCCGirH.pitch_stirrup,
                    count_web = SetFamily.SRCCGirH.count_web,
                    count_bar_spacing = SetFamily.SRCCGirH.count_bar_spacing,
                    pitch_bar_spacing = SetFamily.SRCCGirH.pitch_bar_spacing,
                };
            }
            else
            {
                Rgir = SetFamily.SRCGirH;
            }


            //RC部
            string canti = "";
            if (gir.isCanti)
            { canti = "Cantilever-"; }
            if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
            else
            { Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
            Data.SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);

            Data.SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
            Data.SetParameter(symbol.LookupParameter(Rgir.name), gir.name);

            Data.SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
            Data.SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutin);

            if (haunch_start < 0 && haunch_end < 0)
            {
            }
            else
            {
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start, true);
                Data.SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end, true);

                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end == "DROP")
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { Data.SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }
            }


            if (gir.StbSecFigureBeam_SRC != null)
            {
                switch (gir.StbSecFigureBeam_SRC.FigureType)
                {
                    case 1:
                        var fig1 = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Straight>().FirstOrDefault();
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig1.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig1.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig1.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig1.depth, true);
                        break;

                    case 2:
                        var fig2s = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                        var fig2e = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig2s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig2s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig2e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig2s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig2s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig2e.depth, true);
                        break;

                    case 3:
                        var fig3s = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                        var fig3c = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                        var fig3e = gir.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                        if (fig3s == null) fig3s = fig3c;
                        if (fig3e == null) fig3e = fig3c;
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_start), fig3s.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_center), fig3c.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.width_end), fig3e.width, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_start), fig3s.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_center), fig3c.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rgir.depth_end), fig3e.depth, true);
                        break;
                }


                Parameter p_height = symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT);
                Parameter p_half = symbol.LookupParameter("Half");
                if (p_height != null && p_half != null)
                {
                    //SRC鉄骨の芯ずれ。Halfパラメータで中心位置を割り出しているみたい。
                    //数式がセットされていないので梁せい/2をいれておく。
                    //(高さは数式がセットされている)
                    Data.SetParameter(p_half, p_height.AsDouble() / 2, false);
                }
            }

            if (gir.StbSecBarArrangementBeam_SRC == null)
            {
                //鉄筋タグが無いとき→ログ
                string log = gir.isCanti ? "SRC片持梁" : "SRC梁";
                LogData.AddLog(LogData.LogKind.Warning, 2400, $"[{log}]" + typename + "(断面id=" + gir.id.ToString() + ")");
            }
            else
            {
                StbSecBarBeam_SRC_ThreeTypes bar = null;
                List<StbSecBarBeam_SRC_ThreeTypes> bar2 = new List<StbSecBarBeam_SRC_ThreeTypes>();
                switch (gir.StbSecBarArrangementBeam_SRC.Bar_ArrangementType)
                {
                    case 1:
                        bar = new StbSecBarBeam_SRC_ThreeTypes(gir.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_Same>().FirstOrDefault());
                        bar2.Add(bar);
                        bar2.Add(bar);
                        bar2.Add(bar);
                        break;
                    case 2:
                        bar = gir.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>()?.FirstOrDefault(a => a.pos == StbSecBarBeam_RC_ThreeTypesPos.CENTER);
                        bar2 = gir.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_ThreeTypes>().OrderBy(a => a.pos).ToList();
                        break;
                    case 3:
                        bar = new StbSecBarBeam_SRC_ThreeTypes(gir.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_StartEnd>().FirstOrDefault());
                        bar2 = gir.StbSecBarArrangementBeam_SRC.Items.OfType<StbSecBarBeam_SRC_StartEnd>().OrderBy(a => a.pos).Select(a => new StbSecBarBeam_SRC_ThreeTypes(a)).ToList();
                        //中央はなし
                        bar2.Insert(1, null);
                        break;
                }

                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.StbSecBarArrangementBeam_SRC.depth_cover_left);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.StbSecBarArrangementBeam_SRC.depth_cover_right);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.StbSecBarArrangementBeam_SRC.depth_cover_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.StbSecBarArrangementBeam_SRC.depth_cover_bottom);
                Data.SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.StbSecBarArrangementBeam_SRC.interval);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.StbSecBarArrangementBeam_SRC.center_top);
                Data.SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.StbSecBarArrangementBeam_SRC.center_bottom);

                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), bar.strength_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), bar.strength_2nd_main);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), bar.strength_stirrup);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), bar.strength_web);
                Data.SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), bar.strength_bar_spacing);

                for (int i = 0; i < 3; i++)
                {
                    if (bar2[i] == null) continue;

                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), bar2[i].D_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), bar2[i].D_2nd_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), bar2[i].D_2nd_main);

                    Data.SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), bar2[i].D_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), bar2[i].D_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), bar2[i].D_bar_spacing);

                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), bar2[i].N_main_top_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), bar2[i].N_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), bar2[i].N_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), bar2[i].N_main_bottom_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), bar2[i].N_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), bar2[i].N_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), bar2[i].N_2nd_main_top_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), bar2[i].N_2nd_main_top_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), bar2[i].N_2nd_main_top_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), bar2[i].N_2nd_main_bottom_1st);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), bar2[i].N_2nd_main_bottom_2nd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), bar2[i].N_2nd_main_bottom_3rd);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), bar2[i].N_stirrup);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), bar2[i].pitch_stirrup, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_web[i]), bar2[i].N_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), bar2[i].N_bar_spacing);
                    Data.SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), bar2[i].pitch_bar_spacing, true);
                }
            }
        }




        /// <summary>
        /// 大梁・小梁・片持梁 H形鋼 パラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="symbol"></param>
        /// <param name="steel_ind">StbSteelのIndex</param>
        /// <param name="j">0:始端, 1:中央, 2:終端</param>
        /// <param name="gir"></param>
        /// <param name="shapename">形状</param>
        /// <param name="shapename_J">形状(ログ用)</param>
        /// <param name="shape">鉄骨形状名称</param>
        /// <param name="strength_main">鉄骨強度 main</param>
        /// <param name="strength_web">鉄骨強度 web</param>
        /// <returns></returns>
        private static bool SetParameter_Girder_and_CGirder_HandBH(ST_BRIDGE stb, FamilySymbol symbol, int steel_ind, int j,
                                                                   StbSecBeam_S gir,
                                                                   string shapename, string shapename_J,
                                                                   string shape, string strength_main, string strength_web)
        {
            bool ret = true;

            string logtxt = "";
            if (shapename == RevitLNK.st_steel_H)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[steel_ind];
                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    ret = false;
                    return ret;
                }

                if (!gir.isCanti)
                {
                    FamilyStructure.S_Gir_H Rgir_H = SetFamily.SGirH;
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.strength_main[j]), strength_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.strength_web[j]), strength_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.A[j]), steel.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.B[j]), steel.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.t1[j]), steel.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.t2[j]), steel.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.r[j]), steel.r, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.shape[j]), shape);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.type[j]), steel.type.ToString());
                }
                else
                {
                    FamilyStructure.S_CGir_H Rgir_H = SetFamily.SCGirH;
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.strength_main[j]), strength_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.strength_web[j]), strength_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.A[j]), steel.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.B[j]), steel.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.t1[j]), steel.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.t2[j]), steel.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.r[j]), steel.r, true); //片持梁用ファミリはフィット半径が0でも変換できる
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.shape[j]), shape);
                    Data.SetParameter(symbol.LookupParameter(Rgir_H.type[j]), steel.type.ToString());
                }
            }
            else if (shapename == RevitLNK.st_steel_BH)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[steel_ind];
                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return ret;
                }
                if (!gir.isCanti)
                {
                    FamilyStructure.S_Gir_BH Rgir_BH = SetFamily.SGirBH;
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.strength_main[j]), strength_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.strength_web[j]), strength_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.A[j]), steel.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.B[j]), steel.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.t1[j]), steel.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.t2[j]), steel.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.r[j]), 0.0, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.shape[j]), shape);
                }
                else
                {
                    FamilyStructure.S_CGir_H Rgir_BH = SetFamily.SCGirBH;
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.strength_main[j]), strength_main);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.strength_web[j]), strength_web);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.A[j]), steel.A, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.B[j]), steel.B, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.t1[j]), steel.t1, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.t2[j]), steel.t2, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.r[j]), 0.0, true);
                    Data.SetParameter(symbol.LookupParameter(Rgir_BH.shape[j]), shape);
                }
            }

            return ret;
        }

        /// <summary>
        /// 大梁・小梁・片持梁溝形鋼 パラメータ設定
        /// </summary>
        /// <param name="j">0:始端, 1:中央, 2:終端</param>
        /// <param name="symbol_C"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start">ハンチ長</param>
        /// <param name="haunch_end">ハンチ長</param>
        /// <param name="kind_haunch_start">ハンチ種類</param>
        /// <param name="kind_haunch_end">ハンチ種類</param>
        /// <param name="steel_C"></param>
        /// <param name="gir"></param>
        /// <param name="shapename_J">形状(ログ用)</param>
        /// <param name="shape">鉄骨形状名称</param>
        /// <param name="strength_main">鉄骨強度</param>
        /// <returns></returns>
        private static bool SetParameter_Girder_and_CGirder_C(int j, FamilySymbol symbol_C, string typename,
                                                              List<double> haunch_start, List<double> haunch_end,
                                                              List<string> kind_haunch_start, List<string> kind_haunch_end,
                                                              StbSecRollC steel_C, StbSecBeam_S gir, string shapename_J,
                                                              string shape, string strength_main)
        {
            bool ret = true;
            FamilyStructure.S_Gir_C Rgir_C = SetFamily.SGirC;


            for (int i = 0; i < haunch_start.Count(); i++)
            {
                if (i != 0)
                {
                    typename = typename + "_" + i.ToString();
                    symbol_C = (FamilySymbol)symbol_C.Duplicate(typename);
                }

                string logtxt = Roll_C_Size_Check(steel_C);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol_C.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol_C.Id);
                    ret = false;
                    return ret;
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[i],
                    Length2 = haunch_end[i],
                    BHaunch1 = kind_haunch_start[i],
                    BHaunch2 = kind_haunch_end[i],
                    symbol = symbol_C
                };
                GirderSymbols.Add(re);

                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.SecId), gir.id);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.name), gir.name);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                { Data.SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam), canti + "Girder"); }
                else
                { Data.SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam), canti + "Beam"); }
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam2), gir.kind_beam);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.strength), strength_main);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.shape[j]), shape);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.type[j]), steel_C.type);
                if (steel_C.type != StbSecRollCType.SINGLE)
                { Data.Make_typeLog(typename, gir.id, RevitLNK.st_steel_C, shapename_J); }
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.H[j]), steel_C.A, true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.B[j]), steel_C.B, true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.t1[j]), steel_C.t1, true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.t2[j]), steel_C.t2, true);

                logtxt = "";
                double r1 = steel_C.r1, r2 = steel_C.r2;
                if (steel_C.r1 < 1)
                {
                    if (logtxt == "")
                    { logtxt = "フィレット半径"; }
                    else
                    { logtxt += ",フィレット半径"; }
                    r1 = 1;
                }
                if (steel_C.r2 < 1)
                {
                    if (logtxt == "")
                    { logtxt = "先端半径"; }
                    else
                    { logtxt += ",先端半径"; }
                    r2 = 1;
                }
                if (logtxt != "")
                {
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[" + shapename_J + "]" + typename + "(断面id=" + gir.id.ToString() + ")は" + logtxt +
                                   "が1mm未満のため値を1mmに設定しました。");
                }

                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.r1[j]), r1, true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.r2[j]), r2, true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.haunch_start), haunch_start[i], true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.haunch_end), haunch_end[i], true);
                Data.SetParameter(symbol_C.LookupParameter(Rgir_C.isOutIn), gir.isOutin);
            }
            return ret;
        }

        /// <summary>
        /// 大梁・小梁・片持梁山形鋼 パラメータ設定
        /// </summary>
        /// <param name="j">0:始端, 1:中央, 2:終端</param>
        /// <param name="symbol_L"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start">ハンチ長</param>
        /// <param name="haunch_end">ハンチ長</param>
        /// <param name="kind_haunch_start">ハンチ種類</param>
        /// <param name="kind_haunch_end">ハンチ種類</param>
        /// <param name="steel_L"></param>
        /// <param name="gir"></param>
        /// <param name="shapename_J">形状(ログ用)</param>
        /// <param name="shape">鉄骨形状名称</param>
        /// <param name="strength_main">鉄骨強度</param>
        /// <returns></returns>
        private static bool SetParameter_Girder_and_CGirder_L(int j, FamilySymbol symbol_L, string typename,
                                                              List<double> haunch_start, List<double> haunch_end,
                                                              List<string> kind_haunch_start, List<string> kind_haunch_end,
                                                              StbSecRollL steel_L, StbSecBeam_S gir, string shapename_J,
                                                              string shape, string strength_main)
        {
            bool ret = true;
            FamilyStructure.S_Gir_L Rgir_L = SetFamily.SGirL;


            for (int i = 0; i < haunch_start.Count(); i++)
            {
                if (i != 0)
                {
                    typename = typename + "_" + i.ToString();
                    symbol_L = (FamilySymbol)symbol_L.Duplicate(typename);
                }

                string logtxt = Roll_L_Size_Check(steel_L);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol_L.Name, gir.id, logtxt, 0);
                    ret = false;
                    return ret;
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[i],
                    Length2 = haunch_end[i],
                    BHaunch1 = kind_haunch_start[i],
                    BHaunch2 = kind_haunch_end[i],
                    symbol = symbol_L
                };
                GirderSymbols.Add(re);

                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.SecId), gir.id);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                { Data.SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam), canti + "Girder"); }
                else
                { Data.SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam), canti + "Beam"); }
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam2), gir.kind_beam);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.name), gir.name);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.strength), strength_main);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.shape[j]), shape);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.type[j]), steel_L.type);
                if (steel_L.type != StbSecRollLType.SINGLE)
                { Data.Make_typeLog(typename, gir.id, RevitLNK.st_steel_L, shapename_J); }
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.A[j]), steel_L.A, true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.B[j]), steel_L.B, true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.t1[j]), steel_L.t1, true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.t2[j]), steel_L.t2, true);

                logtxt = "";
                double r1 = steel_L.r1, r2 = steel_L.r2;
                if (steel_L.r1 < 1)
                {
                    if (logtxt == "")
                    { logtxt = "フィレット半径"; }
                    else
                    { logtxt += ",フィレット半径"; }
                    r1 = 1;
                }
                if (steel_L.r2 < 1)
                {
                    if (logtxt == "")
                    { logtxt = "先端半径"; }
                    else
                    { logtxt += ",先端半径"; }
                    r2 = 1;
                }
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol_L.Name, gir.id, logtxt, 1);
                }

                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.r1[j]), r1, true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.r2[j]), r2, true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.haunch_start), haunch_start[i], true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.haunch_end), haunch_end[i], true);
                Data.SetParameter(symbol_L.LookupParameter(Rgir_L.isOutIn), gir.isOutin);
            }

            return ret;
        }

        /// <summary>
        /// 大梁・小梁・片持梁リップ溝形鋼 パラメータ設定
        /// </summary>
        /// <param name="j">0:始端, 1:中央, 2:終端</param>
        /// <param name="symbol_LipC"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start">ハンチ長</param>
        /// <param name="haunch_end">ハンチ長</param>
        /// <param name="kind_haunch_start">ハンチ種類</param>
        /// <param name="kind_haunch_end">ハンチ種類</param>
        /// <param name="steel_LipC"></param>
        /// <param name="gir"></param>
        /// <param name="shapename_J">形状(ログ用)</param>
        /// <param name="shape">鉄骨形状名称</param>
        /// <param name="strength_main">鉄骨強度</param>
        /// <returns></returns>
        private static bool SetParameter_Girder_and_CGirder_LipC(int j, FamilySymbol symbol_LipC, string typename,
                                                                 List<double> haunch_start, List<double> haunch_end,
                                                                 List<string> kind_haunch_start, List<string> kind_haunch_end,
                                                                 StbSecLipC steel_LipC, StbSecBeam_S gir, string shapename_J,
                                                                 string shape, string strength_main)
        {
            bool ret = true;
            FamilyStructure.S_Gir_LipC Rgir_LipC = SetFamily.SGirLipC;


            for (int i = 0; i < haunch_start.Count(); i++)
            {
                if (i != 0)
                {
                    typename = typename + "_" + i.ToString();
                    symbol_LipC = (FamilySymbol)symbol_LipC.Duplicate(typename);
                }

                string logtxt = Rool_LipC_Size_Check(steel_LipC);
                if (logtxt != "")
                {
                    Data.MakeSizeLog(shapename_J, symbol_LipC.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol_LipC.Id);
                    ret = false;
                    return ret;
                }

                Data.ReNameSymbols re = new Data.ReNameSymbols
                {
                    name = typename,
                    id = gir.id,
                    Length = haunch_start[i],
                    Length2 = haunch_end[i],
                    BHaunch1 = kind_haunch_start[i],
                    BHaunch2 = kind_haunch_end[i],
                    symbol = symbol_LipC
                };
                GirderSymbols.Add(re);

                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.SecId), gir.id);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == StbSecBeam_Kind_beam.GIRDER)
                { Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam), canti + "Girder"); }
                else
                { Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam), canti + "Beam"); }
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam2), gir.kind_beam);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.name), gir.name);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.strength), strength_main);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.shape[j]), shape);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.type[j]), steel_LipC.type);
                if (steel_LipC.type != StbSecLipCType.SINGLE)
                { Data.Make_typeLog(typename, gir.id, RevitLNK.st_steel_LipC, shapename_J); }
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.H[j]), steel_LipC.H, true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.A[j]), steel_LipC.A, true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.C[j]), steel_LipC.C, true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.t[j]), steel_LipC.t, true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.haunch_start), haunch_start[i], true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.haunch_end), haunch_end[i], true);
                Data.SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.isOutIn), gir.isOutin);
            }

            return ret;
        }





        /// <summary>
        /// 梁インスタンスパラメータ設定（大梁）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily">大梁のファミリ</param>
        /// <param name="ConvCFamily">片持ち大梁のファミリ</param>
        /// <returns></returns>
        private static bool CreateGirder_instance(ST_BRIDGE stb, StbGirder gir, Family[][] ConvFamily, Family[][] ConvCFamily, StbSecBeam_Kind_beam kind_Beam)
        {
            bool ret = true;

            string logname = kind_Beam == StbSecBeam_Kind_beam.GIRDER ? "大梁" : "小梁";

            //回転角が360度以上→-360度する
            if (gir.rotate >= 360) { gir.rotate = gir.rotate - 360; }

            Family fami = null;
            string shape = "";
            //タイプ名
            string typename = GetTypeName_Girder(stb, gir.id_section);
            //片持ちか否か
            bool isCanti = false;
            //S・SRCの時の鉄骨形状名
            string shapename = "";
            bool isOutin = false;


            //使用するファミリの取得
            switch (gir.kind_structure)
            {
                case StbGirderKind_structure.RC:
                    var rcgir = stb.StbModel.StbSections.StbSecBeam_RC.Find(a => a.id == gir.id_section);
                    isCanti = rcgir.isCanti;
                    isOutin = rcgir.isOutin;
                    if (isCanti)
                    {
                        if (rcgir.isFoundation)
                        { fami = ConvCFamily[0][0]; }
                        else
                        { fami = ConvCFamily[0][1]; }
                    }
                    else
                    {
                        switch (rcgir.StbSecFigureBeam_RC.FigureType) //ファミリを詳細化⇒ハンチ付か3断面同一かを判断する
                        {
                            case 1:
                                if (rcgir.StbSecBarArrangementBeam_RC == null) //鉄筋が入力されていなければ全断面として変換
                                {
                                    if (rcgir.isFoundation)
                                    { fami = ConvFamily[0][0]; }
                                    else
                                    { fami = ConvFamily[0][2]; }
                                }
                                else
                                {
                                    if (rcgir.StbSecBarArrangementBeam_RC.Bar_ArrangementType == 1)
                                    {
                                        if (rcgir.isFoundation)
                                        { fami = ConvFamily[0][0]; }
                                        else
                                        { fami = ConvFamily[0][2]; }
                                    }
                                    else
                                    {
                                        if (rcgir.isFoundation)
                                        { fami = ConvFamily[0][1]; }
                                        else
                                        { fami = ConvFamily[0][3]; }
                                    }
                                }
                                break;

                            case 2:
                                var ts = rcgir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                var te = rcgir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                                if (ts.depth != te.depth ||
                                    ts.width != te.width)
                                {
                                    if (rcgir.isFoundation)
                                    { fami = ConvFamily[0][1]; }
                                    else
                                    { fami = ConvFamily[0][3]; }
                                }
                                else
                                {
                                    if (gir.isFoundation)
                                    { fami = ConvFamily[0][0]; }
                                    else
                                    { fami = ConvFamily[0][2]; }
                                }
                                break;

                            case 3:
                                var hs = rcgir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                                var hc = rcgir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                var he = rcgir.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                                if (hs == null) hs = hc;
                                if (he == null) he = hc;
                                if (hs.depth != hc.depth ||
                                    he.depth != hc.depth ||
                                    hs.width != hc.width ||
                                    he.width != hc.width)
                                {
                                    if (gir.isFoundation)
                                    { fami = ConvFamily[0][1]; }
                                    else
                                    { fami = ConvFamily[0][3]; }
                                }
                                else
                                {
                                    if (gir.isFoundation)
                                    { fami = ConvFamily[0][0]; }
                                    else
                                    { fami = ConvFamily[0][2]; }
                                }
                                break;
                        }
                    }
                    break;

                case StbGirderKind_structure.S:
                    var s_gir = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == gir.id_section);

                    GetSteelShapeData(s_gir.StbSecSteelFigureBeam_S.Items.First(), out string shape1);
                    shape = Check_Steel(stb, shape1, out _);

                    isCanti = s_gir.isCanti;
                    isOutin = s_gir.isOutin;
                    bool shapeflg = false;
                    foreach (var item in s_gir.StbSecSteelFigureBeam_S.Items)
                    {
                        if (item == null) { continue; }

                        GetSteelShapeData(s_gir.StbSecSteelFigureBeam_S.Items.First(), out string shape2);

                        if (shape1 != shape2)
                        {
                            shapeflg = true;
                            break;
                        }
                    }

                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            shapename = "H形鋼";
                            if (isCanti)
                            { fami = ConvCFamily[1][0]; }
                            else
                            {
                                if (shapeflg)
                                {
                                    fami = ConvFamily[1][5];
                                }
                                else
                                {
                                    fami = ConvFamily[1][0];
                                }
                            }
                            break;
                        case RevitLNK.st_steel_BH:
                            shapename = "H組立形鋼";
                            if (isCanti)
                            { fami = ConvCFamily[1][1]; }
                            else
                            { fami = ConvFamily[1][1]; }
                            break;
                        case RevitLNK.st_steel_C:
                            shapename = "溝形鋼";
                            if (isCanti)
                            { fami = ConvCFamily[1][2]; }
                            else
                            { fami = ConvFamily[1][2]; }
                            break;
                        case RevitLNK.st_steel_L:
                            shapename = "山形鋼";
                            if (isCanti)
                            { fami = ConvCFamily[1][3]; }
                            else
                            { fami = ConvFamily[1][3]; }
                            break;
                        case RevitLNK.st_steel_LipC:
                            shapename = "リップ溝形鋼";
                            if (isCanti)
                            { fami = ConvCFamily[1][4]; }
                            else
                            { fami = ConvFamily[1][4]; }
                            break;
                        default:
                            return ret;
                    }
                    break;

                case StbGirderKind_structure.SRC:
                    var src_gir = stb.StbModel.StbSections.StbSecBeam_SRC.Find(a => a.id == gir.id_section);
                    shapename = "H形鋼";

                    isCanti = src_gir.isCanti;
                    isOutin = src_gir.isOutin;

                    if (isCanti)
                    { fami = ConvCFamily[2][0]; }
                    else
                    { fami = ConvFamily[2][0]; }
                    break;
            }

            //ファミリがロードされているか           
            if (fami == null)
            {
                //ログ表示(ファミリがロードされていない)
                LogData.AddLog(LogData.LogKind.Warning, 2100, gir.kind_structure.ToString() + "梁" + shapename);
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            var symbols = GirderSymbols.Where(a => a.id == gir.id_section);
            if (symbols.Count() == 1)
            {
                symbol = symbols.First().symbol;
            }
            else if (symbols.Count() > 1)
            {
                foreach (var s in symbols)
                {
                    if (s.Length == gir.haunch_start && s.Length2 == gir.haunch_end)
                    {
                        symbol = s.symbol;
                        break;
                    }
                }
            }

            if (symbol == null)
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + gir.kind_structure.ToString() + logname + "]" + typename + "(配置id=" + gir.id + ")");
                return ret;
            }


            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, gir.id_node_end, false);
            int indt = Get_stbFloor_index(stb, gir.id_node_start, false);
            Level btmLevel = null;
            if (indb == -1 && indt == -1)
            {
                btmLevel = SearchLevel_height(stb, gir.id_node_start, gir.id_node_end);
            }
            else
            {
                btmLevel = SearchLevel(stb, (indb != -1 ? indb : indt));
            }

            //配置層が取得できない時
            if (btmLevel == null)
            {
                return ret;
            }

            //配置座標の取得（オフセット・レベルを考慮していない節点の位置）
            XYZ Ps_org = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, 0);
            XYZ Pe_org = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, 0);
            if (Ps_org.DistanceTo(Pe_org) < Commons.mm2ft(1))
            {
                string log = logname + "の生成：" + "\t" + "[配置Id " + gir.id.ToString() + "]" + typename + ",[節点Id";
                log += Data.MakeLog_Coord(0, new int[] { gir.id_node_start, gir.id_node_end });
                log += "] ";

                LogData.AddLog(LogData.LogKind.Warning, 3100, log);
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

            if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_end_Z != 0 ||
                gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0)
            {
                offsetstart = Data.TransformCoord(Ps_org, Pe_org, gir.offset_start_X, gir.offset_start_Y, gir.offset_start_Z, -gir.rotate);
                offsetend = Data.TransformCoord(Ps_org, Pe_org, gir.offset_end_X, gir.offset_end_Y, gir.offset_end_Z, -gir.rotate);

                Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(gir.offset_start_X), Ps_org.Y + Commons.mm2ft(gir.offset_start_Y), Ps_org.Z);
                Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(gir.offset_end_X), Pe_org.Y + Commons.mm2ft(gir.offset_end_Y), Pe_org.Z);
            }
            else
            {
                offsetstart = Search_Offset_gir(stb, gir.id_node_start, ref Ps_org, ref Pe_org, "start", vecU, gir.id, btmLevel, -gir.rotate, out offsetstart2);
                offsetend = Search_Offset_gir(stb, gir.id_node_end, ref Ps_org, ref Pe_org, "end", vecU, gir.id, btmLevel, -gir.rotate, out offsetend2);

                Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
                Pe_xy = Pe_org + Commons.mm2ft(offsetend2);
            }

            //梁描画用節点（部材方向のオフセットだけ考慮、それ以外のオフセットはパラメータに入力）
            XYZ Ps = Data.Set_offset(Ps_org, offsetstart, vecU);
            XYZ Pe = Data.Set_offset(Pe_org, offsetend, vecU);

            Line gir_L = Line.CreateBound(Ps, Pe);

            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (isOutin)
                {
                    FamilySymbol newsymbol = Create_newsymbol_isOutin(stb, symbol, gir.id, "GIRDER", isCanti);
                    if (newsymbol != null)
                    { instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, StructuralType.Beam); }
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, StructuralType.Beam);
                }

                //ジオメトリ：各オフセット
                Data.SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                Data.SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                Data.SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                Data.SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                Data.SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);

                //断面回転
                Data.SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-gir.rotate * Math.PI) / 180);

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ)
                //構造用途
                StructuralInstanceUsage usage = kind_Beam == StbSecBeam_Kind_beam.GIRDER ? StructuralInstanceUsage.Girder : StructuralInstanceUsage.Joist;
                Data.SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, usage);

                Data.SetParameter(instance, BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM, btmLevel.Id);


                SetInstanceParameter_Girder(stb, gir, kind_Beam, shape, isCanti, Ps_org, Pe_org, Ps_xy, Pe_xy, instance);



                //解析線分作成
                Commons.doc.Regenerate();
                AnalyticalMember member = AnalyticalMember.Create(Commons.doc, Line.CreateBound(Ps_org, Pe_org));
                if (member != null)
                {
                    //構造の役割
                    var p = member.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set(kind_Beam == StbSecBeam_Kind_beam.GIRDER ? (int)AnalyticalStructuralRole.StructuralRoleGirder : (int)AnalyticalStructuralRole.StructuralRoleBeam);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(member.Id, instance.Id);
                }

                //変換情報ログの出力
                var nodeIds = new int[] { gir.id_node_start, gir.id_node_end } ;
                Data.MakeNodeLog( logname + "の生成：", "[配置Id " + gir.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, gir.id, logname,typename, nodeIds);
                Data.SaveGuid(gir.guid, instance.Id);

                Commons.doc.Regenerate();
                CGrp_Add(stb, gir.id_node_start, gir.id_node_end, instance.Id, instance);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        private static void SetInstanceParameter_Girder(ST_BRIDGE stb, StbGirder gir, StbSecBeam_Kind_beam kind_Beam, string shape, bool isCanti, XYZ Ps_org, XYZ Pe_org, XYZ Ps_xy, XYZ Pe_xy, FamilyInstance instance)
        {
            switch (gir.kind_structure)
            {
                case StbGirderKind_structure.RC:
                    FamilyStructure.RC_Gir Rgir = isCanti ? SetFamily.RCCGir : SetFamily.RCGir;
                    Data.SetParameter(instance.LookupParameter(Rgir.MemId), gir.id);
                    Data.SetParameter(instance.LookupParameter(Rgir.NameMembers), gir.name);
                    Data.SetParameter(instance.LookupParameter(Rgir.thickness_ex_top), gir.thickness_add_top);
                    Data.SetParameter(instance.LookupParameter(Rgir.thickness_ex_bottom), gir.thickness_add_bottom);
                    Data.SetParameter(instance.LookupParameter(Rgir.thickness_ex_right), gir.thickness_add_right);
                    Data.SetParameter(instance.LookupParameter(Rgir.thickness_ex_left), gir.thickness_add_left);
                    Data.SetParameter(instance.LookupParameter(Rgir.kind_haunch_start), gir.kind_haunch_start);
                    Data.SetParameter(instance.LookupParameter(Rgir.kind_haunch_end), gir.kind_haunch_end);
                    Data.SetParameter(instance.LookupParameter(Rgir.type_haunch_H), gir.type_haunch_H);
                    Data.SetParameter(instance.LookupParameter(Rgir.type_haunch_V), gir.type_haunch_V);
                    break;

                case StbGirderKind_structure.S:
                    if (!isCanti)
                    {
                        if (shape == "")
                        {
                            var s_gir = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == gir.id_section);
                            GetSteelShapeData(s_gir.StbSecSteelFigureBeam_S.Items.First(), out string shape1);
                            shape = Check_Steel(stb, shape1, out int ind);
                        }
                        Create_GirderandBeam_S_instance(stb, shape, instance, Ps_xy, Pe_xy, Ps_org, Pe_org, gir, kind_Beam);
                    }
                    else
                    {
                        FamilyStructure.S_CGir_H Hgir = SetFamily.SCGirH;
                        Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                        Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                        if (instance.Symbol.FamilyName == "Steel_CG_H")
                        {
                            bool joint = false;
                            double joint_start = gir.joint_start;
                            if (gir.joint_start != 0)
                            { joint = true; }
                            else
                            {
                                joint = false;
                            }
                            Data.SetParameter(instance.LookupParameter("継手"), joint);
                            Commons.doc.Regenerate();
                            if (joint_start == 0)
                            { joint_start = 1; }
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_start), joint_start, true);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                        }
                        else
                        {
                            double joint_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps_xy, Pe_xy, gir.id_node_start);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_start), Commons.ft2mm( joint_s ));
                            double joint_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe_xy, Ps_xy, gir.id_node_end);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_end), Commons.ft2mm( joint_e ));
                        }
                    }
                    break;

                case StbGirderKind_structure.SRC:
                    if (!isCanti)
                    {
                        FamilyStructure.SRC_Gir Hgir = SetFamily.SRCGirH;
                        Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                        Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_add_top);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_add_bottom);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_add_right);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_add_left);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                        int numjoint = 0;
                        if (gir.joint_start != 0)
                        { numjoint++; }
                        if (gir.joint_end != 0)
                        { numjoint++; }
                        Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                        Commons.doc.Regenerate();
                        if (numjoint != 0)
                        {
                            double joint_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps_xy, Pe_xy, gir.id_node_start);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_start), joint_s);
                            double joint_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe_xy, Ps_xy, gir.id_node_end);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_end), joint_e);
                        }
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                    }
                    else
                    {
                        FamilyStructure.SRC_CGir Hgir = SetFamily.SRCCGirH;
                        Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                        Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_add_top);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_add_bottom);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_add_right);
                        Data.SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_add_left);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                        Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                        int numjoint = 0;
                        if (gir.joint_start != 0)
                        { numjoint++; }
                        if (gir.joint_end != 0)
                        { numjoint++; }
                        Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                        Commons.doc.Regenerate();
                        if (numjoint != 0)
                        {
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_start), gir.joint_start, true);
                            Data.SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                        }
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                        Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                    }
                    break;

            }
        }


        /// <summary>
        /// 大梁・小梁S造インスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="shape"></param>
        /// <param name="instance"></param>
        /// <param name="gir"></param>
        /// <returns></returns>
        private static void Create_GirderandBeam_S_instance(ST_BRIDGE stb, string shape, FamilyInstance instance, XYZ Ps, XYZ Pe, XYZ Ps_org, XYZ Pe_org, StbGirder gir, StbSecBeam_Kind_beam kind_Beam)
        {
            //継手距離はSTBの値そのままでなく、計算値をセットする
            double join_s = 0;
            double join_e = 0;
            int numjoint = 0;

            join_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps, Pe, gir.id_node_start);
            join_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe, Ps, gir.id_node_end);
            if (gir.joint_start != 0) { numjoint++; }
            if (gir.joint_end != 0) { numjoint++; }

            if (shape == RevitLNK.st_steel_H)
            {
                FamilyStructure.S_Gir_H Hgir = (kind_Beam == StbSecBeam_Kind_beam.GIRDER ? SetFamily.SGirH : SetFamily.SBeamH);

                Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                Data.SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                if (instance.Symbol.FamilyName == "Steel_Girder_H" || instance.Symbol.FamilyName == "Steel_Beam_H")
                {
                    //ハンチ種類
                    if (gir.kind_haunch_start == StbGirderKind_haunch.SLOPE || gir.kind_haunch_end == StbGirderKind_haunch.SLOPE)
                    {
                        //↓中身なし
                        Data.Make_haunchLog("DROP", instance.Symbol.Name, gir.id);
                    }

                    //水平ハンチ形状
                    if (gir.type_haunch_H != StbGirderType_haunch_H.BOTH)
                    {
                        Data.Make_haunchLog("BOTH_H", instance.Symbol.Name, gir.id);
                    }

                    //鉛直ハンチ形状
                    if (gir.type_haunch_V != StbGirderType_haunch_V.BOTH)
                    {
                        Data.Make_haunchLog("BOTH_V", instance.Symbol.Name, gir.id);
                    }
                }

            }
            else if (shape == RevitLNK.st_steel_BH)
            {
                FamilyStructure.S_Gir_BH Hgir = (kind_Beam == StbSecBeam_Kind_beam.GIRDER ? SetFamily.SGirBH : SetFamily.SBeamBH);

                Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                Data.SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                if (instance.Symbol.FamilyName == "Steel_Girder_BH")
                {
                    //ハンチ種類
                    if (gir.kind_haunch_start == StbGirderKind_haunch.DROP || gir.kind_haunch_end == StbGirderKind_haunch.DROP)
                    {
                        Data.Make_haunchLog("SLOPE", instance.Symbol.Name, gir.id);
                    }
                    //水平ハンチ形状
                    if (gir.type_haunch_H != StbGirderType_haunch_H.BOTH)
                    {
                        Data.Make_haunchLog("BOTH_H", instance.Symbol.Name, gir.id);
                    }
                    //鉛直ハンチ形状
                    if (gir.type_haunch_V != StbGirderType_haunch_V.TOP)
                    {
                        Data.Make_haunchLog("TOP", instance.Symbol.Name, gir.id);
                    }
                }

            }
            else if (shape == RevitLNK.st_steel_C)
            {
                FamilyStructure.S_Gir_C Hgir = (kind_Beam == StbSecBeam_Kind_beam.GIRDER ? SetFamily.SGirC : SetFamily.SBeamC);

                Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                Data.SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);

            }
            else if (shape == RevitLNK.st_steel_L)
            {
                FamilyStructure.S_Gir_L Hgir = (kind_Beam == StbSecBeam_Kind_beam.GIRDER ? SetFamily.SGirL : SetFamily.SBeamL);

                Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                Data.SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
            }
            else if (shape == RevitLNK.st_steel_LipC)
            {
                FamilyStructure.S_Gir_LipC Hgir = (kind_Beam == StbSecBeam_Kind_beam.GIRDER ? SetFamily.SGirLipC : SetFamily.SBeamLipC);

                Data.SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                Data.SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                Data.SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                Data.SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                Data.SetParameter(instance.LookupParameter("継手数"), numjoint);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                Data.SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
            }
        }

        private static FamilySymbol Create_newsymbol_isOutin(ST_BRIDGE stb, FamilySymbol symbol, int mid, string kind, bool isCanti)
        {
            string section_io_start = "", section_io_end = "";
            int secid = 0;
            string kind_structure = "";
            for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
            {
                if (kind != "GIRDER") { break; }
                if (mid == stb.StbModel.StbMembers.StbGirders[i].id)
                {
                    var gir = stb.StbModel.StbMembers.StbGirders[i];
                    section_io_start = gir.section_io_start.ToString();
                    section_io_end = gir.section_io_end.ToString();
                    secid = gir.id_section;
                    kind_structure = gir.kind_structure.ToString();
                    break;
                }
            }
            for (int i = 0; i < stb.StbModel.StbMembers.StbBeams.Count; i++)
            {
                if (kind != "BEAM") { break; }
                if (mid == stb.StbModel.StbMembers.StbBeams[i].id)
                {
                    var gir = stb.StbModel.StbMembers.StbBeams[i];
                    section_io_start = gir.section_io_start.ToString();
                    section_io_end = gir.section_io_end.ToString();
                    secid = gir.id_section;
                    kind_structure = gir.kind_structure.ToString();
                    break;
                }
            }

            FamilySymbol newsymbol = null;
            if (section_io_start == "" || section_io_start == "NONE") { section_io_start = "OUT"; }
            if (section_io_end == "" || section_io_end == "NONE") { section_io_end = "IN"; }

            //すでに外端・内端で変更されているタイプがあればそれを使用
            for (int i = 0; i < isOutin_G.Count(); i++)
            {
                if (isOutin_G[i].id == secid && isOutin_G[i].section_io_start == section_io_start && isOutin_G[i].section_io_end == section_io_end)
                {
                    newsymbol = isOutin_G[i].symbol;
                    return newsymbol;
                }
            }

            int[] j = new int[3];
            if (section_io_start == "OUT" && section_io_end == "IN")
            {
                newsymbol = symbol;
                return newsymbol;
            }
            else if (section_io_start == "OUT" && section_io_end == "OUT")
            {
                string newtypename = Data.ReName(symbol.Name, 97);
                if (!Data.SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
                { newsymbol = (FamilySymbol)symbol.Duplicate(newtypename); }
                for (int i = 0; i < 3; i++)
                {
                    j[i] = i;
                    if (i == 2)
                    { j[i] = 0; }
                }
            }
            else if (section_io_start == "IN" && section_io_end == "IN")
            {
                string newtypename = Data.ReName(symbol.Name, 97);
                if (!Data.SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
                { newsymbol = (FamilySymbol)symbol.Duplicate(newtypename); }
                for (int i = 0; i < 3; i++)
                {
                    j[i] = i;
                    if (i == 0)
                    { j[i] = 2; }
                }
            }
            else if (section_io_start == "IN" && section_io_end == "OUT")
            {
                string newtypename = Data.ReName(symbol.Name, 97);
                if (!Data.SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
                { newsymbol = (FamilySymbol)symbol.Duplicate(newtypename); }
                for (int i = 0; i < 3; i++)
                {
                    j[i] = i;
                    if (i == 0)
                    { j[i] = 2; }
                    else if (i == 2)
                    { j[i] = 0; }
                }
            }
            switch (kind_structure)
            {
                case "RC":
                    if (!isCanti)
                    {
                        FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                        //形状
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.bar_length_start), symbol.LookupParameter(Rgir.bar_length_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.bar_length_end), symbol.LookupParameter(Rgir.bar_length_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                        //配筋
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i])?.AsDouble());
                        }
                    }
                    else
                    {
                        FamilyStructure.RC_CGir Rgir = SetFamily.RCCGir;
                        //形状
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.bar_length_start), symbol.LookupParameter(Rgir.bar_length_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.bar_length_end), symbol.LookupParameter(Rgir.bar_length_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                        //配筋
                        for (int i = 0; i < 2; i++)
                        {
                            int k = j[i];
                            if (i == 1)
                            {
                                k = j[2];
                            }
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[k]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[k]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[k]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[k]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[k]), symbol.LookupParameter(Rgir.count_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[k]), symbol.LookupParameter(Rgir.count_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[k]), symbol.LookupParameter(Rgir.count_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[k]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[k]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[k]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[k]), symbol.LookupParameter(Rgir.D_stirrup[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[k]), symbol.LookupParameter(Rgir.count_stirrup[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[k]), symbol.LookupParameter(Rgir.pitch_stirrup[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[k]), symbol.LookupParameter(Rgir.D_reinforcement_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_web[k]), symbol.LookupParameter(Rgir.count_web[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[k]), symbol.LookupParameter(Rgir.D_bar_spacing[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[k]), symbol.LookupParameter(Rgir.count_bar_spacing[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[k]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i])?.AsDouble());
                        }
                    }
                    break;
                case "S":
                    if (newsymbol.FamilyName == SetFamily.SGirH.FamilyName || newsymbol.FamilyName == SetFamily.SGirH_Haunch.FamilyName)
                    {
                        FamilyStructure.S_Gir_H Rgir = SetFamily.SGirH;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SCGirBH.FamilyName || newsymbol.FamilyName == SetFamily.SCGirH.FamilyName)
                    {
                        FamilyStructure.S_CGir_H Rgir = SetFamily.SCGirH;
                        //鉄骨形状
                        for (int i = 0; i < 2; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirBH.FamilyName)
                    {
                        FamilyStructure.S_Gir_BH Rgir = SetFamily.SGirBH;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirC.FamilyName || newsymbol.FamilyName == SetFamily.SCGirC.FamilyName)
                    {
                        FamilyStructure.S_Gir_C Rgir = SetFamily.SGirC;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.H[j[i]]), symbol.LookupParameter(Rgir.H[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r1[j[i]]), symbol.LookupParameter(Rgir.r1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r2[j[i]]), symbol.LookupParameter(Rgir.r2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i])?.AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirLipC.FamilyName || newsymbol.FamilyName == SetFamily.SCGirLipC.FamilyName)
                    {
                        FamilyStructure.S_Gir_LipC Rgir = SetFamily.SGirLipC;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.H[j[i]]), symbol.LookupParameter(Rgir.H[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.C[j[i]]), symbol.LookupParameter(Rgir.C[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t[j[i]]), symbol.LookupParameter(Rgir.t[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i])?.AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirL.FamilyName || newsymbol.FamilyName == SetFamily.SCGirL.FamilyName)
                    {
                        FamilyStructure.S_Gir_L Rgir = SetFamily.SGirL;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r1[j[i]]), symbol.LookupParameter(Rgir.r1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r2[j[i]]), symbol.LookupParameter(Rgir.r2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i])?.AsString());

                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                    }
                    break;
                case "SRC":
                    if (newsymbol.FamilyName == SetFamily.SRCGirH.FamilyName)
                    {
                        FamilyStructure.SRC_Gir Rgir = SetFamily.SRCGirH;
                        //形状
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                        //配筋・鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i])?.AsDouble());

                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());

                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SRCCGirH.FamilyName)
                    {
                        FamilyStructure.SRC_CGir Rgir = SetFamily.SRCCGirH;
                        //形状
                        if (section_io_start == "IN")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end)?.AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start)?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start)?.AsDouble());
                        }
                        //配筋・鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i])?.AsInteger());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i])?.AsDouble());

                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i])?.AsDouble());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i])?.AsString());
                            Data.SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i])?.AsString());

                        }
                    }
                    break;
            }

            Data.IsOutin_Girder newOIG = new Data.IsOutin_Girder
            {
                id = secid,
                section_io_start = section_io_start,
                section_io_end = section_io_end,
                symbol = newsymbol
            };
            isOutin_G.Add(newOIG);

            string guid = stb.StbModel.StbSections.StbSecBeam_RC?.Find(a => a.id == secid)?.guid;
            if (guid == null) guid = stb.StbModel.StbSections.StbSecBeam_S?.Find(a => a.id == secid)?.guid;
            if (guid == null) guid = stb.StbModel.StbSections.StbSecBeam_SRC?.Find(a => a.id == secid)?.guid;

            Data.SaveGuid(guid, newsymbol.Id);

            return newsymbol;
        }



        /// <summary>
        /// 梁のタイプ名取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="g_floor">断面.floor</param>
        /// <param name="g_id">断面.id</param>
        /// <param name="g_name">断面.name</param>
        /// <returns>タイプ名</returns>
        private static string GetTypeName_Girder(ST_BRIDGE stb, string g_floor, int g_id, string g_name)
        {
            string typename = "";
            string floor = g_floor;

            if (floor != "")
            {
                int find = Get_stbFloor_index(stb.StbModel.StbStories, floor);
                if (find == -1)
                {
                    find = Get_stbFloor_index_Gir(stb, g_id);
                }
                if (find != -1)
                {
                    typename = stb.StbModel.StbStories[find].name;
                }
            }

            typename += g_name;

            return typename;
        }

        private static string GetTypeName_Girder(ST_BRIDGE stb, int g_id)
        {
            string typename = stb.StbModel.StbMembers.StbGirders.Find(a => a.id_section == g_id)?.name;
            if (typename == null || typename == "")
            {
                typename = stb.StbModel.StbMembers.StbBeams.Find(a => a.id_section == g_id)?.name;
            }

            return typename;
        }


        #endregion


        #region ブレース

        /// <summary>
        /// ブレースの生成 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="syubetu"></param>
        /// <param name="elements"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateBrace(ST_BRIDGE stb, string syubetu, List<FamilySymbol> elements, ref string errmsg)
        {
            bool ret = true;


            //変換ファミリの取得              
            Family[][] ConvFamily = new Family[RevitLNK.SBraText.Length][];
            for (int i = 0; i < RevitLNK.SBraText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.SBraText[i].Length);
            }

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            int numfamily = 0; //変換するファミリの数


            //パラメータの追加
            Data.ProgressStart("ブレースパラメータ追加", ConvFamily.Count());

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                Data.ProgressPerformStep();

                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.SBraFName.flg[i][j]) { continue; }
                    if (!SetFamily.SBraFName.convflg[i][j]) { continue; }

                    foreach (FamilySymbol familysymbol in elements)
                    {
                        if (familysymbol.FamilyName == SetFamily.SBraFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {
                                Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");

                                try
                                {
                                    tran1.Start();
                                    FamilyManager fmg = doc.FamilyManager;

                                    switch (i)
                                    {
                                        case 0:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SBraH(fmg, SetFamily.SBraH);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SBraBH(fmg, SetFamily.SBraBH);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SBraBox(fmg, SetFamily.SBraBox);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SBraBBox(fmg, SetFamily.SBraBBox);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SBraPipe(fmg, SetFamily.SBraPipe);
                                                    break;
                                            }
                                            break;
                                        case 1:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_SBraC(fmg, SetFamily.SBraC);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_SBraL(fmg, SetFamily.SBraL);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_SBraLipC(fmg, SetFamily.SBraLipC);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_SBraFB(fmg, SetFamily.SBraFB);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_SBraRollBar(fmg, SetFamily.SBraRollBar);
                                                    break;
                                            }
                                            break;
                                    }

                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.SBraFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close(false);
                                }
                            }
                            numfamily++;
                        }
                    }
                }
            }

            Data.ProgressClose();


            Transaction tran = new Transaction(Commons.doc, syubetu + "の生成");
            try
            {
                tran.Start();

                //作ったタイプリスト
                List<Data.TypeName_Data> typename_list = new List<Data.TypeName_Data>();

                //ブレースタイプパラメータの設定
                if (stb.StbModel.StbSections.StbSecBrace_S != null && stb.StbModel.StbSections.StbSecBrace_S.Count > 0)
                {
                    Data.ProgressRestart("Sブレースの生成", stb.StbModel.StbSections.StbSecBrace_S.Count);

                    foreach (var bra in stb.StbModel.StbSections.StbSecBrace_S)
                    {
                        Data.ProgressPerformStep();

                        if (!CreateBrace_S(stb, bra, ConvFamily, ref typename_list)) { ret = false; errmsg = "Sブレース"; }
                    }
                }

                Data.ProgressClose();



                //ブレース生成、インスタンスパラメータの設定 
                if (stb.StbModel.StbMembers.StbBraces != null && stb.StbModel.StbMembers.StbBraces.Count > 0)
                {
                    Data.ProgressRestart("ブレースの生成", stb.StbModel.StbMembers.StbBraces.Count);
                    foreach (var bra in stb.StbModel.StbMembers.StbBraces)
                    {
                        Data.ProgressPerformStep();

                        if (!CreateBrace_instance(stb, bra, ConvFamily))
                        {
                            ret = false;
                            errmsg = "Sブレースインスタンス";
                        }
                    }
                }

                Data.ProgressClose();
                Commons.doc.Regenerate();
                tran.Commit();


                //タイプができているかチェック
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                List<FamilySymbol> elements_end = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();
                for (int i = 0; i < typename_list.Count(); i++)
                {
                    bool flg = elements_end.Any(a => a.Name == typename_list[i].typename);
                    if (!flg)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + typename_list[i].shapename + "]" + typename_list[i].typename +
                                       "(断面id=" + typename_list[i].id.ToString() + ")を生成できませんでした。寸法値またはファミリの設定を確認してください。");
                    }
                }

            }
            catch //(Exception e)
            {
                ret = false;
                errmsg = "Sブレース";
                tran.RollBack();
            }

            Data.ProgressClose();

            return ret;
        }


        /// <summary>
        /// Sブレースタイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateBrace_S(ST_BRIDGE stb, StbSecBrace_S bra, Family[][] ConvFamily, ref List<Data.TypeName_Data> typename_list)
        {
            bool ret = true;
            string shapename = "";

            //タイプ名
            string typename = GetTypeName_Brace(stb, bra.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[Sブレース](断面id=" + bra.id.ToString() + ")");
                return ret;
            }

            //鉄骨形状を取得
            int shapeid = -1;
            string steel_shape = "";
            string strength_main = "";
            string strength_web = "";

            var fig_s = bra.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_Same>().ToList();
            var fig_n = bra.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_NotSame>().OrderBy(a => a.pos).ToList();
            var fig_3 = bra.StbSecSteelFigureBrace_S.Items.OfType<StbSecSteelBrace_S_ThreeTypes>().OrderBy(a => a.pos).ToList();

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

            if (shape == RevitLNK.st_steel_H) { shapename = "H形鋼"; }
            else if (shape == RevitLNK.st_steel_BH) { shapename = "組立H形鋼"; }
            else if (shape == RevitLNK.st_steel_C) { shapename = "溝形鋼"; }
            else if (shape == RevitLNK.st_steel_L) { shapename = "山形鋼"; }
            else if (shape == RevitLNK.st_steel_LipC) { shapename = "溝形鋼"; }
            else if (shape == RevitLNK.st_steel_Box) { shapename = "角型鋼"; }
            else if (shape == RevitLNK.st_steel_BBox) { shapename = "組立角型鋼"; }
            else if (shape == RevitLNK.st_steel_Pipe) { shapename = "円形鋼管"; }
            else if (shape == RevitLNK.st_steel_FB) { shapename = "フラットバー"; }
            else if (shape == RevitLNK.st_steel_Bar) { shapename = "丸鋼"; }
            else if (shape != "")
            {
                //ログ表示(変換対象外)
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")");
                return ret;
            }
            else
            {
                if (steel_shape != "")
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")の鉄骨形状[" + steel_shape + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")はブレース鉄骨情報"); }
                return ret;
            }

            FamilySymbol symbol = null;

            if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH || shape == RevitLNK.st_steel_C || shape == RevitLNK.st_steel_L || shape == RevitLNK.st_steel_LipC)
            {
                //鉄骨形状を取得
                int[] shapeids = new int[3];
                GetSteelShapeData(bra.StbSecSteelFigureBrace_S.Items, out int[] ind2, out string[] steel_shape2, out string[] strength_main2, out string[] strength_web2);
                for (int j = 0; j < ind2.Length; ++j)
                {
                    Check_Steel(stb, steel_shape2[j], out shapeids[j]);
                }


                //H形鋼・組立H形鋼以外のファミリは、断面が1断面しか入力できない⇒全断面でないときは中央、始端、終端の優先順 
                if (shape != RevitLNK.st_steel_H && shape != RevitLNK.st_steel_BH)
                {
                    if (fig_s.Count > 0)
                    {
                        //全断面
                    }
                    else if (fig_n.Count > 0)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + bra.id.ToString() + ")は" + shape + "(" + shapename + ")のため脚部断面で変換しました。");
                    }
                    else if (fig_3.Count > 0)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + bra.id.ToString() + ")は" + shape + "(" + shapename + ")のため中央断面で変換しました。");
                    }
                }


                switch (shape)
                {
                    case RevitLNK.st_steel_H:
                        if (ConvFamily[0][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "SブレースH形鋼");
                        }
                        else
                        {
                            //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                            double[] A = new double[3];
                            double[] B = new double[3];
                            double[] t1 = new double[3];
                            double[] t2 = new double[3];
                            double[] r = new double[3];
                            string[] type = new string[3];
                            for (int j = 0; j < 3; j++)
                            {
                                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[shapeids[j]];

                                string logtxt = Roll_H_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
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
                                    r[j] = 1;
                                }
                                else
                                { r[j] = steel.r; }
                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][0], ref typename);
                            }

                            SetParameter_SBrace_H(bra, symbol, steel_shape2, strength_main2, strength_web2, A, B, t1, t2, r, type);
                        }
                        break;

                    case RevitLNK.st_steel_BH:
                        if (ConvFamily[0][1] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース組立H形鋼");
                        }
                        else
                        {
                            //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                            double[] A = new double[3];
                            double[] B = new double[3];
                            double[] t1 = new double[3];
                            double[] t2 = new double[3];
                            for (int j = 0; j < 3; j++)
                            {
                                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[shapeids[j]];

                                string logtxt = Build_H_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                A[j] = steel.A;
                                B[j] = steel.B;
                                t1[j] = steel.t1;
                                t2[j] = steel.t2;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][1], ref typename);
                            }

                            SetParameter_SBrace_BH(bra, symbol, steel_shape2, strength_main2, strength_web2, A, B, t1, t2);
                        }
                        break;

                    case RevitLNK.st_steel_C:
                        if (ConvFamily[1][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース溝形鋼");
                        }
                        else
                        {
                            //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                            double[] A = new double[3];
                            double[] B = new double[3];
                            double[] t1 = new double[3];
                            double[] t2 = new double[3];
                            double[] r1 = new double[3];
                            double[] r2 = new double[3];
                            string[] type = new string[3];
                            bool[] side = new bool[3];
                            for (int j = 0; j < 3; j++)
                            {
                                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[shapeids[j]];

                                string logtxt = Roll_C_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
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
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }

                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[1][0], ref typename);
                            }

                            SetParameter_SBrace_C(bra, typename, symbol, steel_shape2, strength_main2, A, B, t1, t2, r1, r2, type, side);
                        }
                        break;

                    case RevitLNK.st_steel_L:
                        if (ConvFamily[1][1] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース山形鋼");
                        }
                        else
                        {
                            //鉄骨サイズをチェック ①成・幅・板厚のうちどれかが0なら変換しない ②半径が0の時は1を入れて変換する
                            double[] A = new double[3];
                            double[] B = new double[3];
                            double[] t1 = new double[3];
                            double[] t2 = new double[3];
                            double[] r1 = new double[3];
                            double[] r2 = new double[3];
                            string[] type = new string[3];
                            bool[] side = new bool[3];
                            for (int j = 0; j < 3; j++)
                            {
                                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[shapeids[j]];

                                string logtxt = Roll_L_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
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
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
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
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[1][1], ref typename);
                            }

                            SetParameter_SBrace_L(bra, typename, symbol, steel_shape2, strength_main2, A, B, t1, t2, r1, r2, type, side);
                        }
                        break;

                    case RevitLNK.st_steel_LipC:
                        if (ConvFamily[1][2] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレースリップ溝形鋼");
                        }
                        else
                        {
                            double[] H = new double[3];
                            double[] A = new double[3];
                            double[] C = new double[3];
                            double[] t = new double[3];
                            string[] type = new string[3];
                            bool[] side = new bool[3];
                            for (int j = 0; j < 3; j++)
                            {
                                var steel = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[shapeids[j]];

                                string logtxt = Rool_LipC_Size_Check(steel);

                                if (logtxt != "")
                                {
                                    Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                H[j] = steel.H;
                                A[j] = steel.A;
                                C[j] = steel.C;
                                t[j] = steel.t;
                                side[j] = steel.type != StbSecLipCType.SINGLE;
                                type[j] = steel.type.ToString();
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[1][2], ref typename);
                            }

                            SetParameter_SBrace_LipC(bra, typename, symbol, steel_shape2, strength_main2, H, A, C, t, type, side);
                        }
                        break;
                }
            }
            else
            {
                switch (shape)
                {
                    case RevitLNK.st_steel_Box:
                        if (ConvFamily[0][2] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース角形鋼");
                        }
                        else
                        {
                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRollBox[shapeid];
                            string logtxt = Roll_Box_Size_Check(steel);
                            if (logtxt != "")
                            {
                                Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][2], ref typename);
                            }

                            SetParameter_SBrace_Box(bra, steel_shape, strength_main, symbol, steel);
                        }
                        break;

                    case RevitLNK.st_steel_BBox:
                        if (ConvFamily[0][3] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース組立角形鋼");
                        }
                        else
                        {
                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuildBox[shapeid];
                            string logtxt = Build_Box_Size_Check(steel);
                            if (logtxt != "")
                            {
                                Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][3], ref typename);
                            }

                            SetParameter_SBrace_BBox(bra, steel_shape, strength_main, symbol, steel);
                        }
                        break;

                    case RevitLNK.st_steel_Pipe:
                        if (ConvFamily[0][4] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース円形鋼管");
                        }
                        else
                        {
                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];
                            string logtxt = Pipe_Size_Check(steel);
                            if (logtxt != "")
                            {
                                Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][4], ref typename);
                            }

                            SetParameter_SBrace_Pipe(bra, steel_shape, strength_main, symbol, steel);
                        }
                        break;

                    case RevitLNK.st_steel_FB:
                        if (ConvFamily[1][3] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレースフラットバー");
                        }
                        else
                        {
                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecFlatBar[shapeid];
                            string logtxt = "";
                            if (steel.B == 0)
                            { logtxt = "幅"; }
                            if (steel.t == 0)
                            {
                                if (logtxt == "")
                                { logtxt = "板厚"; }
                                else
                                { logtxt += ",板厚"; }
                            }
                            if (logtxt != "")
                            {
                                Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[1][3], ref typename);
                            }

                            SetParameter_SBrace_FB(bra, steel_shape, strength_main, symbol, steel);

                        }
                        break;

                    case RevitLNK.st_steel_Bar:
                        if (ConvFamily[1][4] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "Sブレース丸鋼");
                        }
                        else
                        {
                            var steel = stb.StbModel.StbSections.StbSecSteel.StbSecRoundBar[shapeid];

                            if (steel.R == 0)
                            {
                                Data.MakeSizeLog("Sブレース" + shapename, typename, bra.id, "直径", 0);
                                return ret;
                            }

                            if (braceType[0].ContainsKey(bra.id))
                            {
                                symbol = Commons.doc.GetElement(braceType[0][bra.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[1][4], ref typename);
                            }

                            SetParameter_SBrace_Bar(bra, steel_shape, strength_main, symbol, steel);
                        }
                        break;

                    default:
                        //ログ（変換対象外）
                        Data.Make_taisyougaiLog("Sブレース", bra.id, bra.name, shape, "T形鋼");
                        return ret;
                }
            }


            Data.TypeName_Data td = new Data.TypeName_Data
            {
                typename = typename,
                id = bra.id,
                shapename = "Sブレース"
            };
            typename_list.Add(td);

            if (!braceType[0].ContainsKey(bra.id))
            {
                braceType[0].Add(bra.id, symbol.Id);
            }

            Data.SaveGuid(bra.guid, symbol?.Id);

            return ret;
        }

        private static void SetParameter_SBrace_Bar(StbSecBrace_S bra, string steel_shape, string strength_main, FamilySymbol symbol, StbSecRoundBar steel)
        {
            FamilyStructure.S_Bra_RollBar Rbra_Bar = SetFamily.SBraRollBar;
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.shape), steel_shape);
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.D), steel.R, true);
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra_Bar.SecId), bra.id);
        }

        private static void SetParameter_SBrace_FB(StbSecBrace_S bra, string steel_shape, string strength_main, FamilySymbol symbol, StbSecFlatBar steel)
        {
            FamilyStructure.S_Bra_FB Rbra_FB = SetFamily.SBraFB;


            Data.SetParameter(symbol.LookupParameter(Rbra_FB.strength_main), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.shape), steel_shape);
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.B), steel.B, true);
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.t), steel.t, true);
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra_FB.SecId), bra.id);
        }

        private static void SetParameter_SBrace_Pipe(StbSecBrace_S bra, string steel_shape, string strength_main, FamilySymbol symbol, StbSecPipe steel)
        {
            FamilyStructure.S_Bra_Pipe Rbra = SetFamily.SBraPipe;

            Data.SetParameter(symbol.LookupParameter(Rbra.strength), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra.shape), steel_shape);
            Data.SetParameter(symbol.LookupParameter(Rbra.D), steel.D, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.t), steel.t, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
        }

        private static void SetParameter_SBrace_BBox(StbSecBrace_S bra, string steel_shape, string strength_main, FamilySymbol symbol, StbSecBuildBox steel)
        {
            FamilyStructure.S_Bra_BBox Rbra = SetFamily.SBraBBox;

            Data.SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra.strength), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rbra.shape), steel_shape);
            Data.SetParameter(symbol.LookupParameter(Rbra.H), steel.A, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.B), steel.B, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.t1), steel.t1, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.t2), steel.t2, true);
            Data.SetParameter(symbol.LookupParameter("フィレット"), 0.0, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
        }

        private static void SetParameter_SBrace_Box(StbSecBrace_S bra, string steel_shape, string strength_main, FamilySymbol symbol, StbSecRollBox steel)
        {
            FamilyStructure.S_Bra_Box Rbra = SetFamily.SBraBox;

            Data.SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra.strength), strength_main);
            Data.SetParameter(symbol.LookupParameter(Rbra.shape), steel_shape);
            Data.SetParameter(symbol.LookupParameter(Rbra.H), steel.A, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.B), steel.B, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.t1), steel.t, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.t2), steel.t, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.r), steel.r, true);
            Data.SetParameter(symbol.LookupParameter(Rbra.type), steel.type.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
        }

        private static void SetParameter_SBrace_LipC(StbSecBrace_S bra, string typename, FamilySymbol symbol, string[] steel_shape2, string[] strength_main2, double[] H, double[] A, double[] C, double[] t, string[] type, bool[] side)
        {
            FamilyStructure.S_Bra_LipC Rbra_LipC = SetFamily.SBraLipC;

            Data.SetParameter(symbol.LookupParameter(Rbra_LipC.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra_LipC.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra_LipC.kind_brace), bra.kind_brace.ToString());
            for (int j = 0; j < 3; j++)
            {
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.strength), strength_main2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.shape[j]), steel_shape2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.type[j]), type[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.side[j]), side[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.H[j]), H[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.A[j]), A[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.C[j]), C[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_LipC.t[j]), t[j], true);
            }
            if (type[0] == "2C")
            { Data.Make_typeLog(typename, bra.id, RevitLNK.st_steel_LipC, "リップ溝形鋼", false); }
        }

        private static void SetParameter_SBrace_L(StbSecBrace_S bra, string typename, FamilySymbol symbol, string[] steel_shape2, string[] strength_main2, double[] A, double[] B, double[] t1, double[] t2, double[] r1, double[] r2, string[] type, bool[] side)
        {
            FamilyStructure.S_Bra_L Rbra_L = SetFamily.SBraL;

            Data.SetParameter(symbol.LookupParameter(Rbra_L.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra_L.name), bra.name);
            Data.SetParameter(symbol.LookupParameter(Rbra_L.kind_brace), bra.kind_brace.ToString());
            for (int j = 0; j < 3; j++)
            {
                Data.SetParameter(symbol.LookupParameter(Rbra_L.strength), strength_main2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.shape[j]), steel_shape2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.type[j]), type[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.side[j]), side[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.A[j]), A[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.B[j]), B[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.t1[j]), t1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.t2[j]), t2[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.r1[j]), r1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_L.r2[j]), r2[j], true);
            }
            if (type[0] == "2L")
            { Data.Make_typeLog(typename, bra.id, RevitLNK.st_steel_L, "山形鋼", false); }
        }

        private static void SetParameter_SBrace_C(StbSecBrace_S bra, string typename, FamilySymbol symbol, string[] steel_shape2, string[] strength_main2, double[] A, double[] B, double[] t1, double[] t2, double[] r1, double[] r2, string[] type, bool[] side)
        {
            FamilyStructure.S_Bra_C Rbra_C = SetFamily.SBraC;

            Data.SetParameter(symbol.LookupParameter(Rbra_C.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra_C.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra_C.name), bra.name);
            for (int j = 0; j < 3; j++)
            {
                Data.SetParameter(symbol.LookupParameter(Rbra_C.strength), strength_main2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.shape[j]), steel_shape2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.type[j]), type[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.side[j]), side[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.H[j]), A[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.B[j]), B[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.t1[j]), t1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.t2[j]), t2[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.r1[j]), r1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_C.r2[j]), r2[j], true);
            }
            if (type[0] == "2C")
            { Data.Make_typeLog(typename, bra.id, RevitLNK.st_steel_C, "溝形鋼", false); }
        }

        private static void SetParameter_SBrace_BH(StbSecBrace_S bra, FamilySymbol symbol, string[] steel_shape2, string[] strength_main2, string[] strength_web2, double[] A, double[] B, double[] t1, double[] t2)
        {
            FamilyStructure.S_Bra_BH Rbra_BH = SetFamily.SBraBH;

            Data.SetParameter(symbol.LookupParameter(Rbra_BH.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra_BH.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra_BH.name), bra.name);
            for (int j = 0; j < 3; j++)
            {
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.strength_main[j]), strength_main2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.strength_web[j]), strength_web2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.shape[j]), steel_shape2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.A[j]), A[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.B[j]), B[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.t1[j]), t1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_BH.t2[j]), t2[j], true);
            }
        }

        private static void SetParameter_SBrace_H(StbSecBrace_S bra, FamilySymbol symbol, string[] steel_shape2, string[] strength_main2, string[] strength_web2, double[] A, double[] B, double[] t1, double[] t2, double[] r, string[] type)
        {
            FamilyStructure.S_Bra_H Rbra_H = SetFamily.SBraH;
            Data.SetParameter(symbol.LookupParameter(Rbra_H.SecId), bra.id);
            Data.SetParameter(symbol.LookupParameter(Rbra_H.kind_brace), bra.kind_brace.ToString());
            Data.SetParameter(symbol.LookupParameter(Rbra_H.name), bra.name);
            for (int j = 0; j < 3; j++)
            {
                Data.SetParameter(symbol.LookupParameter(Rbra_H.strength_main[j]), strength_main2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.strength_web[j]), strength_web2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.shape[j]), steel_shape2[j]);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.type[j]), type);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.A[j]), A[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.B[j]), B[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.t1[j]), t1[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.t2[j]), t2[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.r[j]), r[j], true);
                Data.SetParameter(symbol.LookupParameter(Rbra_H.type[j]), type[j]);
            }
        }

        private static string GetTypeName_Brace(ST_BRIDGE stb, StbSecBrace_S bra)
        {
            string typename = "";
            string floor = bra.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb.StbModel.StbStories, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, bra.id); }
                if (find != -1)
                { typename = stb.StbModel.StbStories[find].name; ; }
            }
            typename += bra.name;
            return typename;
        }

        private static string GetTypeName_Brace(ST_BRIDGE stb, int id)
        {
            string typename = stb.StbModel.StbMembers.StbBraces.Find(a => a.id_section == id)?.name;
            return typename;
        }


        /// <summary>ブレースインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="ConvFamily">大梁のファミリ</param>
        /// <returns></returns>
        private static bool CreateBrace_instance(ST_BRIDGE stb, StbBrace bra, Family[][] ConvFamily)
        {
            bool ret = true;

            var sec = stb.StbModel.StbSections.StbSecBrace_S.Find(a => a.id == bra.id_section);

            Family fami = null;

            //タイプ名
            string typename = GetTypeName_Brace(stb, sec.id);

            GetSteelShapeData(sec.StbSecSteelFigureBrace_S.Items.First(), out string steel_shape);
            string shape = Check_Steel(stb, steel_shape, out int ind);

            switch (shape)
            {
                case RevitLNK.st_steel_H:
                    fami = ConvFamily[0][0];
                    break;
                case RevitLNK.st_steel_BH:
                    fami = ConvFamily[0][1];
                    break;
                case RevitLNK.st_steel_Box:
                    fami = ConvFamily[0][2];
                    break;
                case RevitLNK.st_steel_BBox:
                    fami = ConvFamily[0][3];
                    break;
                case RevitLNK.st_steel_Pipe:
                    fami = ConvFamily[0][4];
                    break;
                case RevitLNK.st_steel_C:
                    fami = ConvFamily[1][0];
                    break;
                case RevitLNK.st_steel_L:
                    fami = ConvFamily[1][1];
                    break;
                case RevitLNK.st_steel_LipC:
                    fami = ConvFamily[1][2];
                    break;
                case RevitLNK.st_steel_FB:
                    fami = ConvFamily[1][3];
                    break;
                case RevitLNK.st_steel_Bar:
                    fami = ConvFamily[1][4];
                    break;
                default:
                    return ret;
            }

            //ファミリがロードされているか           
            if (fami == null)
            {
                //ログ表示(ファミリがロードされていない)
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[Sブレース]" + bra.name + "(配置Id=" + bra.id.ToString() + ")");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (braceType[0].ContainsKey(bra.id_section))
            {
                symbol = Commons.doc.GetElement(braceType[0][bra.id_section]) as FamilySymbol;
            }
            else
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[Sブレース]" + bra.name + "(配置Id=" + bra.id.ToString() + ")");
                return ret;
            }


            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, bra.id_node_end, false);
            int indt = Get_stbFloor_index(stb, bra.id_node_start, false);
            Level btmLevel = null;
            if (indb == -1 && indt == -1)
            {
                btmLevel = SearchLevel_height(stb, bra.id_node_start, bra.id_node_end);
            }
            else if (indb == -1 || indt == -1)
            {
                btmLevel = SearchLevel(stb, (indb != -1 ? indb : indt));
            }
            else
            {
                btmLevel = SearchLevel(stb, Math.Min(indb, indt));
            }

            //水平ブレースか鉛直ブレースか
            var kind_brace = sec.kind_brace;

            //配置座標の取得
            XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, bra.id_node_start, 0, 0, 0);
            XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, bra.id_node_end, 0, 0, 0);
            if (Ps.DistanceTo(Pe) < Commons.mm2ft(1))
            {
                string log = "ブレースの生成：" + "\t" + "[配置Id " + bra.id.ToString() + "]" + typename + ",[節点Id";
                log += Data.MakeLog_Coord(0, new int[] { bra.id_node_start, bra.id_node_end });
                log += "] ";

                LogData.AddLog(LogData.LogKind.Warning, 3100, log);
                return ret; //falseは変換失敗
            }

            XYZ vecU = (Pe - Ps).Normalize();

            //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
            XYZ offsetstart = new XYZ();
            if (bra.offset_start_X != 0 || bra.offset_start_Y != 0 || bra.offset_start_Z != 0)
            {
                offsetstart = Data.TransformCoord(Ps, Pe, bra.offset_start_X, bra.offset_start_Y, bra.offset_start_Z, -bra.rotate);
            }
            else
            {
                offsetstart = Search_Offset_bra(stb, bra.id_node_start, Ps, Pe, "start", kind_brace, -bra.rotate);
            }

            XYZ offsetend = new XYZ();
            if (bra.offset_end_X != 0 || bra.offset_end_Y != 0 || bra.offset_end_Z != 0)
            {
                offsetend = Data.TransformCoord(Ps, Pe, bra.offset_end_X, bra.offset_end_Y, bra.offset_end_Z, -bra.rotate);
            }
            else
            {
                offsetend = Search_Offset_bra(stb, bra.id_node_end, Ps, Pe, "end", kind_brace, -bra.rotate);
            }

            Ps = Data.Set_offset(Ps, offsetstart, vecU, true);
            Pe = Data.Set_offset(Pe, offsetend, vecU, true);

            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (kind_brace == StbSecBrace_SKind_brace.HORIZONTAL)
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Line.CreateBound(Ps, Pe), symbol, btmLevel, StructuralType.Beam);
                    Data.SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.HorizontalBracing); //構造用途 
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Line.CreateBound(Ps, Pe), symbol, btmLevel, StructuralType.Brace);
                    Data.SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.Brace); //構造用途 
                }

                //解析線分作成
                Commons.doc.Regenerate();
                XYZ Ps_org = Get_Node_Position(stb.StbModel.StbNodes, bra.id_node_start, 0, 0, 0);
                XYZ Pe_org = Get_Node_Position(stb.StbModel.StbNodes, bra.id_node_end, 0, 0, 0);
                AnalyticalMember member = AnalyticalMember.Create(Commons.doc, Line.CreateBound(Ps_org, Pe_org));
                if (member != null)
                {
                    //構造の役割
                    var p = member.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleMember);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(member.Id, instance.Id);
                }

                //ジオメトリ：各オフセット
                Data.SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                Data.SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                Data.SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                Data.SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                Data.SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);

                //断面回転
                Data.SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-bra.rotate * Math.PI) / 180);


                Data.SetParameter(instance, BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM, btmLevel.Id); //参照レベル



                Data.SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                Data.SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                Data.SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                Data.SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);

                SetInstanceParameter_Brace(bra, instance);

                //変換情報ログの出力
                var nodeIds = new int[] { bra.id_node_start, bra.id_node_end } ;
                Data.MakeNodeLog( "ブレースの生成：", "[配置Id " + bra.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, bra.id, "ブレース", typename, nodeIds ) ;

                Data.SaveGuid(bra.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        private static void SetInstanceParameter_Brace(StbBrace bra, FamilyInstance instance)
        {
            if (instance.Symbol.FamilyName == SetFamily.SBraH.FamilyName)
            {
                FamilyStructure.S_Bra_H Rbra = SetFamily.SBraH;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                int joint_num = 0;
                if (bra.joint_start != 0) { joint_num++; }
                if (bra.joint_end != 0) { joint_num++; }
                Data.SetParameter(instance.LookupParameter("継手数"), joint_num);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraBH.FamilyName)
            {
                FamilyStructure.S_Bra_BH Rbra = SetFamily.SBraBH;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                int joint_num = 0;
                if (bra.joint_start != 0) { joint_num++; }
                if (bra.joint_end != 0) { joint_num++; }
                Data.SetParameter(instance.LookupParameter("継手数"), joint_num);
                Commons.doc.Regenerate();
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraBox.FamilyName)
            {
                FamilyStructure.S_Bra_Box Rbra = SetFamily.SBraBox;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraBBox.FamilyName)
            {
                FamilyStructure.S_Bra_BBox Rbra = SetFamily.SBraBBox;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraPipe.FamilyName)
            {
                FamilyStructure.S_Bra_Pipe Rbra = SetFamily.SBraPipe;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraC.FamilyName)
            {
                FamilyStructure.S_Bra_C Rbra = SetFamily.SBraC;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraL.FamilyName)
            {
                FamilyStructure.S_Bra_L Rbra = SetFamily.SBraL;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraLipC.FamilyName)
            {
                FamilyStructure.S_Bra_LipC Rbra = SetFamily.SBraLipC;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraFB.FamilyName)
            {
                FamilyStructure.S_Bra_FB Rbra = SetFamily.SBraFB;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
            else if (instance.Symbol.FamilyName == SetFamily.SBraRollBar.FamilyName)
            {
                FamilyStructure.S_Bra_RollBar Rbra = SetFamily.SBraRollBar;
                Data.SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                Data.SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                Data.SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                Data.SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                Data.SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                Data.SetParameter(instance.LookupParameter(Rbra.future_brace), bra.feature_brace.ToString());
            }
        }


        #endregion


        #region スラブ

        /// <summary>
        /// スラブの生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateSlab(ST_BRIDGE stb, string buzai, ref string errmsg)
        {
            bool ret = true;

            bool isFoundation = false;
            string catename = ""; //使用するファミリカテゴリ名
            if (buzai == "基礎スラブ")
            { isFoundation = true; }



            List<int> ids = new List<int>();
            List<string> typenames = new List<string>();
            List<FloorType> symbols = new List<FloorType>();

            bool paraflg = true; //パラメータを追加した⇒false
            if (!isFoundation)
            {
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                List<FloorType> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FloorType>().ToList();

                catename = "構造床";

                if (elements == null || elements.Count == 0)
                {
                    ret = false;
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "構造床");
                    return ret;
                }
                else
                {
                    Data.ProgressStart("床パラメータの追加", 1);
                    foreach (FloorType symbol in elements)
                    {
                        if (paraflg)
                        {
                            Data.ProgressPerformStep();

                            ParaSet.SetPara_Slab("床", symbol, SetFamily.Slab);
                            paraflg = false;
                        }

                        symbols.Add(symbol);
                    }
                    Data.ProgressClose();
                }
            }
            else
            {
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                List<FloorType> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FloorType>().ToList();

                catename = "基礎床";

                if (elements == null || elements.Count == 0)
                {
                    ret = false;
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎床");
                    return ret;
                }
                else
                {
                    Data.ProgressStart("基礎床パラメータの追加", 1);
                    foreach (FloorType symbol in elements)
                    {
                        if (paraflg)
                        {
                            Data.ProgressPerformStep();
                            ParaSet.SetPara_Slab("構造基礎", symbol, SetFamily.Slab);
                            paraflg = false;
                        }

                        symbols.Add(symbol);
                    }
                    Data.ProgressClose();
                }
            }

            if (symbols.Count == 0)
            {
                ret = false;
                LogData.AddLog(LogData.LogKind.Warning, 2100, catename);
                return ret;
            }


            Transaction tran = new Transaction(Commons.doc, "スラブの生成");
            FamilyStructure.Slab Rsla = SetFamily.Slab;
            try
            {
                tran.Start();

                errmsg = buzai;
                switch (buzai)
                {
                    case "RCスラブ":
                    case "基礎スラブ":
                        if (stb.StbModel.StbSections.StbSecSlab_RC != null)
                        {
                            var slabs = stb.StbModel.StbSections.StbSecSlab_RC.Where(a => a.isFoundation == isFoundation).ToList();
                            if (slabs.Count > 0)
                            {
                                Data.ProgressRestart(buzai + "の生成", slabs.Count);

                                foreach (var sla in stb.StbModel.StbSections.StbSecSlab_RC)
                                {
                                    Data.ProgressPerformStep();

                                    string typename = GetTypeName_Slab(stb, sla.id);
                                    if (typename == null || typename == "")
                                    {
                                        LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{buzai}](断面id=" + sla.id.ToString() + ")");
                                        continue;
                                    }

                                    //もし、名前がかぶっていたらReNameする
                                    bool nameflg = true;
                                    int ascii = 97;
                                    string oldname = typename;
                                    do
                                    {
                                        nameflg = true;
                                        for (int j = 0; j < symbols.Count(); j++)
                                        {
                                            if (symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                typename = Data.ReName(oldname, ascii);
                                                ascii++;
                                                nameflg = false;
                                                break;
                                            }
                                        }
                                    }
                                    while (!nameflg);

                                    FloorType symbol = (FloorType)symbols[0].Duplicate(typename);
                                    symbols.Add(symbol);

                                    if (!CreateRCSlab(stb, sla, symbol))
                                    {
                                        errmsg = buzai;
                                    }
                                    ids.Add(sla.id);
                                    typenames.Add(typename);
                                }
                            }
                        }
                        break;

                    case "デッキプレート":
                        if (stb.StbModel.StbSections.StbSecSlabDeck != null && stb.StbModel.StbSections.StbSecSlabDeck.Count > 0)
                        {
                            Data.ProgressRestart("デッキスラブの生成", stb.StbModel.StbSections.StbSecSlabDeck.Count);
                            foreach (var sla in stb.StbModel.StbSections.StbSecSlabDeck)
                            {
                                Data.ProgressPerformStep();

                                string typename = GetTypeName_Slab(stb, sla.id);
                                if (typename == null || typename == "")
                                {
                                    LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{buzai}](断面id=" + sla.id.ToString() + ")");
                                    continue;
                                }

                                //もし、名前がかぶっていたらReNameする
                                bool nameflg = true;
                                int ascii = 97;
                                string oldname = typename;
                                do
                                {
                                    nameflg = true;
                                    for (int j = 0; j < symbols.Count(); j++)
                                    {
                                        if (symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            typename = Data.ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                FloorType symbol = (FloorType)symbols[0].Duplicate(typename);
                                symbols.Add(symbol);

                                if (!CreateDeckSlab(stb, sla, symbol))
                                {
                                    errmsg = "デッキスラブ";
                                }
                                ids.Add(sla.id);
                                typenames.Add(typename);
                            }
                        }
                        break;

                    case "既製スラブ":
                        if (stb.StbModel.StbSections.StbSecSlabPrecast != null && stb.StbModel.StbSections.StbSecSlabPrecast.Count > 0)
                        {
                            Data.ProgressRestart("既製スラブの生成", stb.StbModel.StbSections.StbSecSlabPrecast.Count);
                            foreach (var sla in stb.StbModel.StbSections.StbSecSlabPrecast)
                            {
                                Data.ProgressPerformStep();

                                string typename = GetTypeName_Slab(stb, sla.id);
                                if (typename == null || typename == "")
                                {
                                    LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{buzai}](断面id=" + sla.id.ToString() + ")");
                                    continue;
                                }

                                //もし、名前がかぶっていたらReNameする
                                bool nameflg = true;
                                int ascii = 97;
                                string oldname = typename;
                                do
                                {
                                    nameflg = true;
                                    for (int j = 0; j < symbols.Count(); j++)
                                    {
                                        if (symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            typename = Data.ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                FloorType symbol = (FloorType)symbols[0].Duplicate(typename);
                                symbols.Add(symbol);

                                if (!CreateProductSlab(stb, sla, symbol))
                                {
                                    errmsg = "既製スラブ";
                                }
                                ids.Add(sla.id);
                                typenames.Add(typename);
                            }
                        }
                        break;
                }

                Data.ProgressClose();
                Commons.doc.Regenerate();
                tran.Commit();

            }
            catch (Exception)
            {
                tran.RollBack();
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }

            Data.ProgressClose();



            Transaction tran2 = new Transaction(Commons.doc, "スラブインスタンスパラメータの設定");
            try
            {
                tran2.Start();

                errmsg = "スラブインスタンス";
                if (stb.StbModel.StbMembers.StbSlabs != null && stb.StbModel.StbMembers.StbSlabs.Count > 0)
                {
                    List<int> id_section = new List<int>();
                    switch (buzai)
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
                    if (slabs.Count > 0)
                    {
                        Data.ProgressRestart($"{buzai}の生成", slabs.Count);
                        foreach (var sla in slabs)
                        {
                            Data.ProgressPerformStep();

                            bool secflg = false;
                            FloorType symbol = null;
                            for (int j = 0; j < ids.Count(); j++)
                            {
                                if (sla.id_section == ids[j])
                                {
                                    secflg = true;
                                    for (int k = 0; k < symbols.Count(); k++)
                                    {
                                        if (symbols[k].Name == typenames[j])
                                        {
                                            symbol = symbols[k];
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!secflg) { continue; }


                            if (!CreateSlab_instance(stb, sla, symbol, buzai, ref errmsg, isFoundation))
                            {
                                if (errmsg == "")
                                { errmsg = "スラブインスタンス"; }
                            }

                            //Commons.doc.Regenerate();

                        }
                    }
                }

                Data.ProgressClose();
                Commons.doc.Regenerate();
                tran2.Commit();
            }
            catch (Exception)
            {
                tran2.RollBack();
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }


            Data.ProgressClose();

            return ret;
        }


        private static string GetTypeName_Slab(ST_BRIDGE stb, int id)
        {
            string typename = stb.StbModel.StbMembers.StbSlabs.Find(a => a.id_section == id)?.name;
            return typename;
        }

        /// <summary>
        /// RCスラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static bool CreateRCSlab(ST_BRIDGE stb, StbSecSlab_RC sla, FloorType symbol)
        {
            bool ret = true;

            FamilyStructure.Slab Rsla = SetFamily.Slab;

            try
            {
                //構造の設定（床厚、コンクリート情報)
                ElementId eid = null;
                Object val = sla.strength_concrete;
                if (sla.strength_concrete == "")
                {
                    val = stb.StbCommon.strength_concrete;
                }
                Data.SetMaterial(ref val, ref eid);

                double depth = 0;
                if (sla.StbSecFigureSlab_RC != null)
                {
                    if (sla.StbSecFigureSlab_RC.Items.First() is StbSecSlab_RC_Straight)
                    {
                        var fig_s = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Straight>().FirstOrDefault();
                        depth = fig_s.depth;
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_base), 0, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_tip), 0, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.length_haunch), 0, true);
                    }
                    else if (sla.StbSecFigureSlab_RC.Items.First() is StbSecSlab_RC_Taper)
                    {
                        var fig_tb = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Taper>().FirstOrDefault(a => a.pos == StbSecSlab_RC_TaperPos.BASE);
                        var fig_tt = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Taper>().FirstOrDefault(a => a.pos == StbSecSlab_RC_TaperPos.TIP);
                        depth = (fig_tb.depth + fig_tt.depth) / 2;
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_base), fig_tb.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_tip), fig_tt.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_center), 0.0, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.length_haunch), 0, true);
                    }
                    else if (sla.StbSecFigureSlab_RC.Items.First() is StbSecSlab_RC_Haunch)
                    {
                        var fig_hb = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecSlab_RC_HaunchPos.BASE);
                        var fig_hc = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecSlab_RC_HaunchPos.CENTER);
                        var fig_hh = sla.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecSlab_RC_HaunchPos.HAUNCH);
                        depth = fig_hc.depth;
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_base), fig_hb.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.length_haunch), fig_hh.depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);
                        Data.SetParameter(symbol.LookupParameter(Rsla.depth_tip), 0, true);
                    }
                }

                //構造のスラブ厚・コンクリート強度を設定
                CompoundStructure csSlab = symbol.GetCompoundStructure();
                if (depth < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[RCスラブ]" + sla.name + "(断面id=" + sla.id.ToString() + ")" + "床厚が0mmなので1mmとして変換します。");
                }
                csSlab.SetLayerWidth(0, Commons.mm2ft(depth));

                if (eid != null)
                {
                    csSlab.SetMaterialId(0, eid);
                }
                symbol.SetCompoundStructure(csSlab);

                Data.SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                Data.SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                Data.SetParameter(symbol.LookupParameter(Rsla.isEarthen), sla.isEarthen);
                if (sla.isCanti)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.isCanti), "片持ち");
                }
                else
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.isCanti), "一般");
                }

                if (sla.StbSecBarArrangementSlab_RC != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.StbSecBarArrangementSlab_RC.depth_cover_top, true);
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_cover_bottom), sla.StbSecBarArrangementSlab_RC.depth_cover_bottom, true);

                    string strength = "";

                    var bar_st = sla.StbSecBarArrangementSlab_RC.Items.OfType<StbSecBarSlab_RC_Standard>().OrderBy(a => a.pos).ToList();
                    var bar_2w = sla.StbSecBarArrangementSlab_RC.Items.OfType<StbSecBarSlab_RC_2Way>().OrderBy(a => a.pos).ToList();
                    var bar_1w1 = sla.StbSecBarArrangementSlab_RC.Items.OfType<StbSecBarSlab_RC_1Way1>().OrderBy(a => a.pos).ToList();
                    var bar_1w2 = sla.StbSecBarArrangementSlab_RC.Items.OfType<StbSecBarSlab_RC_1Way2>().OrderBy(a => a.pos).ToList();
                    if (bar_st.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                        for (int j = 0; j < bar_st.Count; ++j)
                        {
                            string[] d = Get_D2(bar_st[j].D);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D1[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.pitch[j]), bar_st[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_st[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_st[j].strength);
                            }
                        }
                    }
                    else if (bar_2w.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                        for (int j = 0; j < bar_2w.Count; ++j)
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
                            }

                            string[] d = Get_D2(bar_2w[j].D);
                            for (int k = 0; k < ind.Length; ++k)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_2w[j].pitch, true);
                            }

                            if (strength == "")
                            {
                                strength = bar_2w[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_2w[j].strength);
                            }
                        }
                    }
                    else if (bar_1w1.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ1配筋");
                        for (int j = 0; j < bar_1w1.Count; ++j)
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
                            }

                            string[] d = Get_D2(bar_1w1[j].D);
                            for (int k = 0; k < ind.Length; ++k)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_1w1[j].pitch, true);
                            }

                            if (strength == "")
                            {
                                strength = bar_1w1[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_1w1[j].strength);
                            }
                        }
                    }
                    else if (bar_1w2.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ2配筋");
                        for (int j = 0; j < bar_1w2.Count; ++j)
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

                            string[] d = Get_D2(bar_1w2[j].D);
                            for (int k = 0; k < ind.Length; ++k)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_1w2[j].pitch, true);
                            }

                            if (strength == "")
                            {
                                strength = bar_1w2[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_1w2[j].strength);
                            }
                        }
                    }


                    Data.Parameter_Select_Set(Rsla.strength, strength, floor: symbol);
                }
                else
                {
                    //鉄筋タグが無い→ログ出力
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RCスラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                }

                Data.SaveGuid(sla.guid, symbol.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
        
        /// <summary>
        /// デッキスラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static bool CreateDeckSlab(ST_BRIDGE stb, StbSecSlabDeck sla, FloorType symbol)
        {
            bool ret = true;

            FamilyStructure.Slab Rsla = SetFamily.Slab;

            try
            {
                //コンクリート強度のElementIdを取得
                ElementId eid = null;
                Object val = sla.strength_concrete;
                if (sla.strength_concrete == "")
                {
                    val = stb.StbCommon.strength_concrete;
                }
                Data.SetMaterial(ref val, ref eid);

                //構造のスラブ厚・コンクリート強度を設定
                CompoundStructure csSlab = symbol.GetCompoundStructure();
                double depth = 0;
                if (sla.StbSecFigureSlabDeck.StbSecSlabDeckStraight.depth < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[デッキスラブ]" + sla.name + "(断面id=" + sla.id.ToString() + ")" + "床厚が0mmなので1mmとして変換します。");
                }
                else
                {
                    depth = sla.StbSecFigureSlabDeck.StbSecSlabDeckStraight.depth;
                }
                csSlab.SetLayerWidth(0, Commons.mm2ft(depth));
                if (eid != null)
                { csSlab.SetMaterialId(0, eid); }
                symbol.SetCompoundStructure(csSlab);

                Data.SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                Data.SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                Data.SetParameter(symbol.LookupParameter(Rsla.product_type), sla.product_type);
                Data.SetParameter(symbol.LookupParameter(Rsla.isCanti), "一般");

                if (sla.StbSecBarArrangementSlabDeck != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.StbSecBarArrangementSlabDeck.depth_cover_top, true);
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_cover_bottom), sla.StbSecBarArrangementSlabDeck.depth_cover_bottom, true);

                    string strength = "";

                    var bar_st = sla.StbSecBarArrangementSlabDeck.Items.OfType<StbSecBarSlabDeckStandard>().OrderBy(a => a.pos).ToList();
                    var bar_2w = sla.StbSecBarArrangementSlabDeck.Items.OfType<StbSecBarSlabDeck2Way>().OrderBy(a => a.pos).ToList();
                    var bar_1w = sla.StbSecBarArrangementSlabDeck.Items.OfType<StbSecBarSlabDeck1Way>().OrderBy(a => a.pos).ToList();
                    if (bar_st.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                        for (int j = 0; j < bar_st.Count; ++j)
                        {
                            string[] d = Get_D2(bar_st[j].D);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D1[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.pitch[j]), bar_st[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_st[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_st[j].strength);
                            }
                        }
                    }
                    else if (bar_2w.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                        for (int j = 0; j < bar_2w.Count; ++j)
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
                            }

                            string[] d = Get_D2(bar_2w[j].D);
                            for (int k = 0; k < ind.Length; ++k)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_2w[j].pitch, true);
                            }

                            if (strength == "")
                            {
                                strength = bar_2w[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_2w[j].strength);
                            }
                        }
                    }
                    else if (bar_1w.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ配筋");
                        for (int j = 0; j < bar_1w.Count; ++j)
                        {
                            int[] ind = new int[0];
                            string[] d = Get_D2(bar_1w[j].D);
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
                                    //耐火補強筋
                                    ind = null;
                                    Data.SetParameter(symbol.LookupParameter(Rsla.addD), d[0]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.addpitch), bar_1w[j].pitch, true);
                                    break;
                            }

                            if (ind != null)
                            {
                                for (int k = 0; k < ind.Length; ++k)
                                {
                                    Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_1w[j].pitch, true);
                                }
                            }

                            if (strength == "")
                            {
                                strength = bar_1w[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_1w[j].strength);
                            }
                        }
                    }


                    Data.Parameter_Select_Set(Rsla.strength, strength, floor: symbol);
                }
                else
                {
                    //鉄筋タグが無い→ログ出力
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[デッキスラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                }

                if (sla.StbSecProductSlabDeck != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_company), sla.StbSecProductSlabDeck.product_company);
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_code), sla.StbSecProductSlabDeck.product_code);
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_center), sla.StbSecProductSlabDeck.depth_deck, true);
                }

                Data.SaveGuid(sla.guid, symbol.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
        
        /// <summary>
        /// 既製スラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static bool CreateProductSlab(ST_BRIDGE stb, StbSecSlabPrecast sla, FloorType symbol)
        {
            bool ret = true;

            FamilyStructure.Slab Rsla = SetFamily.Slab;

            try
            {
                //コンクリート強度のElementIdを取得
                ElementId eid = null;
                Object val = sla.strength_concrete;
                if (sla.strength_concrete == "")
                {
                    val = stb.StbCommon.strength_concrete;
                }
                Data.SetMaterial(ref val, ref eid);

                //構造のスラブ厚・コンクリート強度を設定
                CompoundStructure csSlab = symbol.GetCompoundStructure();

                //床厚が0だと変換できない⇒1mmに設定する
                double depth = 0;
                if (sla.StbSecFigureSlabPrecast != null)
                {
                    //precast_type == FULL の場合は StbSecFigureSlabPrecastなし
                    depth = sla.StbSecFigureSlabPrecast.StbSecSlabPrecastStraight.depth_concrete;
                }
                if (depth < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[既製スラブ]" + sla.name + "(断面id=" + sla.id.ToString() + ")" + "床厚が0mmなので1mmとして変換します。");
                }

                csSlab.SetLayerWidth(0, Commons.mm2ft(depth));
                if (eid != null)
                { csSlab.SetMaterialId(0, eid); }
                symbol.SetCompoundStructure(csSlab);

                Data.SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                Data.SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                Data.SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);

                if (sla.StbSecBarArrangementSlabPrecast != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.StbSecBarArrangementSlabPrecast.depth_cover_top, true);

                    string strength = "";

                    var bar_st = sla.StbSecBarArrangementSlabPrecast.Items.OfType<StbSecBarSlabPrecastStandard>().OrderBy(a => a.pos).ToList();
                    var bar_2w = sla.StbSecBarArrangementSlabPrecast.Items.OfType<StbSecBarSlabPrecast2Way>().OrderBy(a => a.pos).ToList();
                    var bar_1w = sla.StbSecBarArrangementSlabPrecast.Items.OfType<StbSecBarSlabPrecast1Way>().OrderBy(a => a.pos).ToList();
                    if (bar_st.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                        for (int j = 0; j < bar_st.Count; ++j)
                        {
                            string[] d = Get_D2(bar_st[j].D);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D1[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rsla.pitch[j]), bar_st[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_st[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_st[j].strength);
                            }
                        }
                    }
                    else if (bar_2w.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                        for (int j = 0; j < bar_2w.Count; ++j)
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
                            }

                            string[] d = Get_D2(bar_2w[j].D);
                            for (int k = 0; k < ind.Length; ++k)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_2w[j].pitch, true);
                            }

                            if (strength == "")
                            {
                                strength = bar_2w[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_2w[j].strength);
                            }
                        }
                    }
                    else if (bar_1w.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ配筋");
                        for (int j = 0; j < bar_1w.Count; ++j)
                        {
                            int[] ind = new int[0];
                            string[] d = Get_D2(bar_1w[j].D);
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
                                    //耐火補強筋
                                    ind = null;
                                    Data.SetParameter(symbol.LookupParameter(Rsla.addD), d[0]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.addpitch), bar_1w[j].pitch, true);
                                    break;
                            }

                            if (ind != null)
                            {
                                for (int k = 0; k < ind.Length; ++k)
                                {
                                    Data.SetParameter(symbol.LookupParameter(Rsla.D1[ind[k]]), d[0]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.D2[ind[k]]), d[1]);
                                    Data.SetParameter(symbol.LookupParameter(Rsla.pitch[ind[k]]), bar_1w[j].pitch, true);
                                }
                            }

                            if (strength == "")
                            {
                                strength = bar_1w[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_1w[j].strength);
                            }
                        }
                    }


                    Data.Parameter_Select_Set(Rsla.strength, strength, floor: symbol);
                }
                else
                {
                    if (sla.precast_type == StbSecSlabPrecastPrecast_type.FULL)
                    {
                        //FULLの場合は配筋記述しないのでログも出さない
                    }
                    else
                    {
                        //鉄筋タグが無い→ログ出力
                        LogData.AddLog(LogData.LogKind.Warning, 2400, "[既製スラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                    }
                }

                if (sla.StbSecProductSlabPrecast != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_company), sla.StbSecProductSlabPrecast.product_company);
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_name), sla.StbSecProductSlabPrecast.product_name);
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_code), sla.StbSecProductSlabPrecast.product_code);
                    Data.SetParameter(symbol.LookupParameter(Rsla.product_depth), sla.StbSecProductSlabPrecast.depth, true);
                }

                Data.SaveGuid(sla.guid, symbol.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }



        /// <summary>
        /// スラブインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <param name="isFoundaion"></param>
        /// <returns></returns>
        private static bool CreateSlab_instance(ST_BRIDGE stb, StbSlab sla, FloorType symbol, string buzai, ref string errmsg, bool isFoundaion = false)
        {
            bool ret = true;

            List<Curve> profile = new List<Curve>();
            Level btmLevel = null;
            try
            {
                if (!GetSlabCoords(stb, sla, buzai, out List<int> nodes, out List<XYZ> Point0, out List<int> stbfloorid, out XYZ v, out XYZ normal, true))
                {
                    return ret;
                }

                //解析線分用にオフセットのない座標取得
                GetSlabCoords(stb, sla, buzai, out _, out List<XYZ> Point1, out _, out _, out _, false);
                Point1.Add(Point1[0]);

                XYZ cross = (normal.CrossProduct(new XYZ(0, 0, 1))).Normalize();


                //傾斜フラグ
                bool keisyaflg = true;
                if (cross.GetLength() < gosa)
                {
                    keisyaflg = false;
                }

                //傾斜床の生成に使用
                Line arrow = null; //傾斜方向
                double slope = 0;  //傾斜角度
                XYZ KP = Point0[0];//基準の高さ
                if (keisyaflg)
                {
                    //傾斜軸
                    XYZ vec1 = XYZ.BasisZ.CrossProduct(normal).Normalize();
                    //傾斜方向
                    XYZ vec2 = normal.CrossProduct(vec1).Normalize();
                    //傾斜方向からZ成分を取り除いたもの
                    XYZ vec3 = new XYZ(vec2.X, vec2.Y, 0).Normalize();

                    slope = Math.Tan(vec2.AngleTo(vec3));
                    if (vec2.Z < 0)
                    {
                        //下がる床なら角度反転
                        slope = -slope;
                    }

                    arrow = Line.CreateBound(KP, KP + vec3); //傾斜方向
                }

                for (int i = 0; i < Point0.Count(); i++)
                {
                    int j = i + 1;
                    if (j >= Point0.Count()) { j = 0; }
                    XYZ Pi = Point0[i];
                    XYZ Pj = Point0[j];

                    if (Pi.X == Pj.X && Pi.Y == Pj.Y && Pi.Z == Pj.Z)
                    { continue; }

                    //配置レベルの取得
                    Level newlv = null;
                    int index = stbfloorid[i];
                    do
                    {
                        newlv = SearchLevel(stb, index);
                        index--;
                        if (index < 0) { break; }
                    } while (newlv == null);
                    if (newlv == null)
                    {
                        index = stbfloorid[i];
                        do
                        {
                            newlv = SearchLevel(stb, index);
                            index++;
                            if (index == stb.StbModel.StbStories.Count()) { break; }
                        } while (newlv == null);
                    }

                    if (newlv == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + sla.kind_structure + "スラブ]" + sla.name + "(配置Id=" + sla.id.ToString() + ")は基準レベルが取得できないため変換できません。");
                        return ret;
                    }

                    if (btmLevel == null)
                    { btmLevel = newlv; }
                    if (newlv.Elevation < btmLevel.Elevation)
                    { btmLevel = newlv; }

                    //傾斜床
                    if (keisyaflg)
                    {
                        //傾斜床の場合は傾斜方向と同じ高さでプロファイルを作成する
                        XYZ Pi2 = new XYZ(Pi.X, Pi.Y, KP.Z);
                        XYZ Pj2 = new XYZ(Pj.X, Pj.Y, KP.Z);

                        profile.Add(Line.CreateBound(Pi2, Pj2));

                    }
                    //平行
                    else
                    {
                        profile.Add(Line.CreateBound(Pi, Pj));
                    }
                }

                Floor instance = null;
                List<CurveLoop> prof2 = new List<CurveLoop>() { CurveLoop.Create(profile) };
                instance = Floor.Create(Commons.doc, prof2, symbol.Id, btmLevel.Id, true, arrow, slope);

                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), KP.Z - btmLevel.Elevation);

                SetInstanceParameter_Slab(stb, sla, instance);


                //解析線分
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
                AnalyticalPanel panel = AnalyticalPanel.Create(Commons.doc, curves);
                if (panel != null)
                {
                    //構造の役割
                    var p = panel.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleFloor);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(panel.Id, instance.Id);
                }


                if (sla.StbOpenIdList != null && sla.StbOpenIdList.Count > 0)
                {
                    //開口を作る前に、一度Regenerateしないとエラーが出る
                    Commons.doc.Regenerate();
                    errmsg = "開口";
                    if (!Slab_Open(stb, sla, Point0[0], v, normal, instance, keisyaflg))
                    {
                        ret = false;
                    }
                }

                //変換情報ログの出力
                var nodeIds = nodes.ToArray() ;
                Data.MakeNodeLog( "スラブの生成：", "[配置Id " + sla.id.ToString() + "]" + symbol.Name, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, sla.id, "スラブ", symbol.Name, nodeIds ) ;

                Data.SaveGuid(sla.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        private static void SetInstanceParameter_Slab(ST_BRIDGE stb, StbSlab sla, Floor instance)
        {
            FamilyStructure.Slab Rsla = SetFamily.Slab;
            Data.SetParameter(instance.LookupParameter(Rsla.MemId), sla.id);
            Data.SetParameter(instance.LookupParameter(Rsla.NameMembers), sla.name);
            Data.SetParameter(instance.LookupParameter(Rsla.thickness_ex_upper), sla.thickness_add_top);
            Data.SetParameter(instance.LookupParameter(Rsla.thickness_ex_bottom), sla.thickness_add_bottom);
            if (sla.direction_load == StbSlabDirection_load.Item1WAY)
            {
                Data.SetParameter(instance.LookupParameter(Rsla.dir_load), "1WAY");
            }
            else if (sla.direction_load == StbSlabDirection_load.Item2WAY)
            {
                Data.SetParameter(instance.LookupParameter(Rsla.dir_load), "2WAY");
            }
            Data.SetParameter(instance.LookupParameter(Rsla.angle_load), sla.angle_load);
            Data.SetParameter(instance.LookupParameter(Rsla.isFoundation), sla.isFoundation);
            Data.SetParameter(instance.LookupParameter(Rsla.kind_structure), sla.kind_structure);
            Data.SetParameter(instance.LookupParameter(Rsla.kind_slab), sla.kind_slab);

            string type_haunch = "";
            var sec_slab = stb.StbModel.StbSections.StbSecSlab_RC.Find(a => a.id == sla.id_section);
            if (sec_slab != null)
            {
                if (sec_slab.StbSecFigureSlab_RC != null)
                {
                    if (sec_slab.StbSecFigureSlab_RC.Items.OfType<StbSecSlab_RC_Haunch>().Count() > 0)
                    {
                        type_haunch = sla.type_haunch.ToString();
                    }
                }
            }
            Data.SetParameter(instance.LookupParameter(Rsla.type_haunch), type_haunch);
        }

        /// <summary>
        /// スラブ座標の取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="buzai"></param>
        /// <param name="nodes"></param>
        /// <param name="Point0">座標[ft]</param>
        /// <param name="stbfloorid"></param>
        /// <param name="v1"></param>
        /// <param name="normal"></param>
        /// <param name="convOffset">オフセット変換するか（差分用）</param>
        /// <returns></returns>
        private static bool GetSlabCoords(ST_BRIDGE stb, StbSlab sla, string buzai, out List<int> nodes, out List<XYZ> Point0, out List<int> stbfloorid, out XYZ v1, out XYZ normal, bool convOffset)
        {
            Point0 = new List<XYZ>();
            stbfloorid = new List<int>();
            v1 = new XYZ();
            normal = new XYZ();


            //同じ節点idが含まれているときがある→同じものは消去
            nodes = sla.StbNodeIdOrderList.Distinct().ToList();
            if (nodes.Count < 3)
            {
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + buzai + "]" + sla.name + "(配置Id=" + sla.id.ToString() + "節点数が3未満のスラブ");
                return false;
            }

            //節点をオフセットを考慮した値に直す
            for (int i = 0; i < nodes.Count; i++)
            {
                //設定画面で設定したレベルのオフセットは自動で計算してくれるので、個々の計算には含まない
                XYZ Pa = Get_Node_Position(stb.StbModel.StbNodes, nodes[i], 0, 0, 0);
                if (sla.StbSlabOffsetList != null && convOffset)
                {
                    int n = nodes[i];
                    var offset = sla.StbSlabOffsetList.Find(a => a.id_node == n);
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

                int flid = Get_stbFloor_index(stb, nodes[i]);
                stbfloorid.Add(flid);
            }

            //閉じた図形になっているか確認
            for (int i = 0; i < Point0.Count(); i++)
            {
                XYZ Cc = new XYZ();
                int cs = -1;
                if (i == Point0.Count() - 1)
                {
                    for (int j = 1; j < Point0.Count() - 2; j++)
                    {
                        cs = Commons.CalcCross(Point0[i], Point0[0], Point0[j], Point0[j + 1], out Cc);
                        if (cs == 0) { break; }
                    }

                }
                else
                {
                    for (int j = i + 2; j < Point0.Count() - 1; j++)
                    {

                        cs = Commons.CalcCross(Point0[i], Point0[i + 1], Point0[j], Point0[j + 1], out Cc);
                        if (cs == 0) { break; }
                    }
                }
                if (cs == 0)
                {
                    //ログ
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + buzai + "]" + sla.name + "(配置Id=" + sla.id.ToString() + ")構成する辺が交差しているスラブ");
                    return false;
                }
            }


            //法線ベクトル
            v1 = (Point0[1] - Point0[0]).Normalize();
            XYZ v2 = new XYZ();
            v2 = (Point0[Point0.Count() - 1] - Point0[0]).Normalize();
            normal = (v2.CrossProduct(v1)).Normalize();
            if (normal.X == 0 && normal.Y == 0 && normal.Z == 0)
            {
                int i = Point0.Count() - 1;
                do
                {
                    i--;
                    if (i < 1) { break; }
                    v2 = (Point0[i] - Point0[0]).Normalize();
                    normal = v2.CrossProduct(v1).Normalize();

                } while (normal.X == 0 && normal.Y == 0 && normal.Z == 0);
            }

            //同一平面上にすべての節点があるか⇒同一平面上でない場合は変換対象外
            if (!Commons.CalcPlane(normal, Point0))
            {
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[RCスラブ]" + sla.name + "(配置Id=" + sla.id.ToString() + ")節点が同一面上に無いスラブ");
                return false;
            }

            return true;
        }

        /// <summary>
        /// ねじれスラブの生成
        /// </summary>
        /// <param name="points">座標[ft]</param>
        /// <param name="symbol"></param>
        /// <param name="lv"></param>
        /// <returns></returns>
        private Floor CreateTwistSlab(List<XYZ> points, FloorType symbol, Level lv)
        {
            //最初の点のZ座標でprofileを作る
            List<Curve> profile = new List<Curve>();
            for (int p = 0; p < points.Count; ++p)
            {
                int q = p + 1;
                if (q >= points.Count) q = 0;

                XYZ p1 = new XYZ(points[p].X, points[p].Y, points[0].Z);
                XYZ p2 = new XYZ(points[q].X, points[q].Y, points[0].Z);

                profile.Add(Line.CreateBound(p1, p2));
            }

            List<CurveLoop> prof2 = new List<CurveLoop>() { CurveLoop.Create(profile) };

            //フラットな床を生成
            Floor instance = Floor.Create(Commons.doc, prof2, symbol.Id, lv.Id, true, null, 0);
            if (instance != null)
            {
                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), points[0].Z - lv.Elevation);

                Commons.doc.Regenerate();

                //サブ要素で各頂点のZ座標を調整
                if (instance.SlabShapeEditor() != null)
                {
                    for (int p = 0; p < points.Count; ++p)
                    {
                        instance.SlabShapeEditor().AddPoint(points[p]);
                    }
                }
            }

            return instance;
        }


        /// <summary>
        /// 開口の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="Ps"></param>
        /// <param name="Vx">X方向単位ベクトル</param>
        /// <param name="N">法線ベクトル</param>
        /// <param name="instance"></param>
        /// <param name="keisyaflg"></param>
        /// <returns></returns>
        private static bool Slab_Open(ST_BRIDGE stb, StbSlab sla, XYZ Ps, XYZ Vx, XYZ N, Floor instance, bool keisyaflg)
        {
            bool ret = true;

            try
            {
                for (int i = 0; i < sla.StbOpenIdList.Count; i++)
                {
                    var open = stb.StbModel.StbMembers.StbOpens.Find(a => a.id == sla.StbOpenIdList[i].id);
                    if (open == null) continue;


                    XYZ Vy = -N.CrossProduct(Vx).Normalize();
                    XYZ Vz = -N;

                    //回転
                    Commons.AxisRotate(Vx, new XYZ(0, 0, 0), Vz, open.rotate, ref Vx);
                    Commons.AxisRotate(Vy, new XYZ(0, 0, 0), Vz, open.rotate, ref Vy);

                    XYZ Pb = Ps + Commons.mm2ft(open.position_X) * Vx + Commons.mm2ft(open.position_Y) * Vy;

                    CurveArray profile = new CurveArray();
                    XYZ Pn1 = Pb + Vx * Commons.mm2ft(open.length_X);
                    profile.Append(Line.CreateBound(Pb, Pn1));
                    XYZ Pn2 = Pn1 + Vy * Commons.mm2ft(open.length_Y);
                    profile.Append(Line.CreateBound(Pn1, Pn2));
                    XYZ Pn3 = Pn2 - Vx * Commons.mm2ft(open.length_X);
                    profile.Append(Line.CreateBound(Pn2, Pn3));
                    profile.Append(Line.CreateBound(Pn3, Pb));
                    var op = Commons.doc.Create.NewOpening(instance, profile, keisyaflg);

                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "スラブ開口の生成：\t[配置Id" + open.id.ToString() + "]" + open.name);
                    OutputDebubCommentLog( instance, open.id, "スラブ開口", open.name, new int[]{} ) ;
                
                    Data.SaveGuid(open.guid, op?.Id);
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 0, "[スラブ開口]" + sla.name + "(断面id=" + sla.id.ToString() + ")");
            }


            return ret;
        }

        #endregion


        #region 壁

        /// <summary>
        /// 壁の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateWall(ST_BRIDGE stb, string buzai, ref string errmsg)
        {
            bool ret = true;


            List<int> ids = new List<int>();
            List<string> typenames = new List<string>();
            List<WallType> symbols = new List<WallType>();

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<WallType> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<WallType>().ToList();
            if (elements == null || elements.Count == 0)
            {
                ret = false;
                LogData.AddLog(LogData.LogKind.Warning, 2100, "構造壁");
                return ret;
            }
            else
            {
                Data.ProgressStart("壁パラメータの追加", 1);
                Data.ProgressPerformStep();
                foreach (WallType symbol in elements)
                {
                    Parameter p = symbol.LookupParameter("断面id");
                    if (p == null)
                    {
                        ParaSet.SetPara_Wall("壁", symbol, SetFamily.Wall);
                    }
                    symbols.Add(symbol);
                }
                Data.ProgressClose();
            }

            if (symbols.Count() == 0)
            {
                ret = false;
                LogData.AddLog(LogData.LogKind.Warning, 2100, "構造壁");
                return ret;
            }



            Transaction tran = new Transaction(Commons.doc, "壁の生成");
            try
            {
                tran.Start();

                switch (buzai)
                {
                    case "壁":
                        if (stb.StbModel.StbSections.StbSecWall_RC != null && stb.StbModel.StbSections.StbSecWall_RC.Count > 0)
                        {
                            Data.ProgressStart("RC壁の生成", stb.StbModel.StbSections.StbSecWall_RC.Count);

                            foreach (var wal in stb.StbModel.StbSections.StbSecWall_RC)
                            {
                                Data.ProgressPerformStep();

                                string typename = GetTypeName_Wall(stb, wal.id);
                                if (typename == null || typename == "")
                                {
                                    LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{buzai}](断面id=" + wal.id.ToString() + ")");
                                    continue;
                                }

                                //もし、名前がかぶっていたらReNameする
                                bool nameflg = true;
                                int ascii = 97;
                                string oldname = typename;
                                do
                                {
                                    nameflg = true;
                                    for (int j = 0; j < symbols.Count(); j++)
                                    {
                                        if (symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            typename = Data.ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                WallType symbol = null;
                                for (int s = 0; s < symbols.Count(); s++)
                                {
                                    if (symbols[s].Kind == WallKind.Basic)
                                    {
                                        symbol = (WallType)symbols[s].Duplicate(typename);
                                        break;
                                    }
                                }
                                symbols.Add(symbol);

                                if (!CreateRCWall(stb, wal, symbol))
                                {
                                    ret = false;
                                    errmsg = buzai;
                                }
                                ids.Add(wal.id);
                                typenames.Add(typename);
                            }
                        }
                        break;

                    case "RCパラペット":
                        if (stb.StbModel.StbSections.StbSecParapet_RC != null && stb.StbModel.StbSections.StbSecParapet_RC.Count > 0)
                        {
                            Data.ProgressStart("RCパラペットの生成", stb.StbModel.StbSections.StbSecParapet_RC.Count);

                            foreach (var wal in stb.StbModel.StbSections.StbSecParapet_RC)
                            {
                                Data.ProgressPerformStep();

                                string typename = GetTypeName_Wall(stb, wal.id);
                                if (typename == null || typename == "")
                                {
                                    LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{buzai}](断面id=" + wal.id.ToString() + ")");
                                    continue;
                                }

                                //もし、名前がかぶっていたらReNameする
                                bool nameflg = true;
                                int ascii = 97;
                                string oldname = typename;
                                do
                                {
                                    nameflg = true;
                                    for (int j = 0; j < symbols.Count(); j++)
                                    {
                                        if (symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            typename = Data.ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                WallType symbol = null;
                                for (int s = 0; s < symbols.Count(); s++)
                                {
                                    if (symbols[s].Kind == WallKind.Basic)
                                    {
                                        symbol = (WallType)symbols[s].Duplicate(typename);
                                        break;
                                    }
                                }
                                symbols.Add(symbol);
                                if (!CreateParapet(stb, wal, symbol)) { ret = false; errmsg = buzai; }

                                ids.Add(wal.id);
                                typenames.Add(typename);
                            }
                        }
                        break;

                }

                Data.ProgressClose();
                Commons.doc.Regenerate();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.RollBack();
                ret = false;
                errmsg = buzai;
            }

            Data.ProgressClose();




            tran.SetName("壁インスタンスパラメータの設定");
            try
            {
                tran.Start();

                switch (buzai)
                {
                    case "壁":
                        if (stb.StbModel.StbMembers.StbWalls != null && stb.StbModel.StbMembers.StbWalls.Count > 0)
                        {
                            Data.ProgressRestart("壁の生成", stb.StbModel.StbMembers.StbWalls.Count);

                            foreach (var wal in stb.StbModel.StbMembers.StbWalls)
                            {
                                Data.ProgressPerformStep();

                                bool secflg = false;
                                WallType symbol = null;
                                for (int j = 0; j < ids.Count(); j++)
                                {
                                    if (wal.id_section == ids[j])
                                    {
                                        secflg = true;
                                        for (int k = 0; k < symbols.Count(); k++)
                                        {
                                            if (symbols[k].Name == typenames[j])
                                            {
                                                symbol = symbols[k];
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (!secflg) { continue; }

                                if (!CreateWall_instance(stb, wal, symbol, ref errmsg))
                                {
                                    ret = false;
                                    errmsg = buzai;
                                }
                            }
                        }

                        break;

                    case "RCパラペット":
                        if (stb.StbModel.StbMembers.StbParapets != null && stb.StbModel.StbMembers.StbParapets.Count > 0)
                        {
                            Data.ProgressRestart("パラペットの生成", stb.StbModel.StbMembers.StbParapets.Count);

                            foreach (var wal in stb.StbModel.StbMembers.StbParapets)
                            {
                                Data.ProgressPerformStep();

                                bool secflg = false;
                                WallType symbol = null;
                                for (int j = 0; j < ids.Count(); j++)
                                {
                                    if (wal.id_section == ids[j])
                                    {
                                        secflg = true;
                                        for (int k = 0; k < symbols.Count(); k++)
                                        {
                                            if (symbols[k].Name == typenames[j])
                                            {
                                                symbol = symbols[k];
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (!secflg) { continue; }

                                if (!CreateParapet_instance(stb, wal, symbol, ref errmsg))
                                {
                                    ret = false;
                                    errmsg = buzai;
                                }
                            }
                        }
                        break;
                }


                Data.ProgressClose();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.RollBack();
                ret = false;
                errmsg = buzai;
            }


            Data.ProgressClose();

            return ret;
        }

        private static string GetTypeName_Wall(ST_BRIDGE stb, int id)
        {
            string typename = stb.StbModel.StbMembers.StbWalls.Find(a => a.id_section == id)?.name;
            if (typename == null || typename == "")
            {
                typename = stb.StbModel.StbMembers.StbParapets.Find(a => a.id_section == id)?.name;
            }

            return typename;
        }


        /// <summary>
        /// RC壁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static bool CreateRCWall(ST_BRIDGE stb, StbSecWall_RC wal, WallType symbol)
        {
            bool ret = true;

            FamilyStructure.Wall Rwal = SetFamily.Wall;

            try
            {
                //構造の設定（壁厚、コンクリート情報)
                ElementId eid = null;
                Object val = wal.strength_concrete;
                if (wal.strength_concrete == "")
                {
                    val = stb.StbCommon.strength_concrete;
                }
                Data.SetMaterial(ref val, ref eid);

                double depth = wal.StbSecFigureWall_RC.StbSecWall_RC_Straight.t;

                //構造の壁厚・コンクリート強度を設定
                CompoundStructure csWall = symbol.GetCompoundStructure();
                if (depth < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[RC壁]" + wal.name + "(断面id=" + wal.id.ToString() + ")" + "は壁厚が0mmなので1mmとして変換します。");
                }
                csWall.SetLayerWidth(0, Commons.mm2ft(depth));
                if (eid != null)
                { csWall.SetMaterialId(0, eid); }
                symbol.SetCompoundStructure(csWall);

                Data.SetParameter(symbol.LookupParameter(Rwal.SecId), wal.id);
                Data.SetParameter(symbol.LookupParameter(Rwal.name), wal.name);

                //配筋
                if (wal.StbSecBarArrangementWall_RC != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rwal.depth_cover_outside), wal.StbSecBarArrangementWall_RC.depth_cover_outside);
                    Data.SetParameter(symbol.LookupParameter(Rwal.depth_cover_inside), wal.StbSecBarArrangementWall_RC.depth_cover_inside);

                    string strength = "";

                    var bar_s = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_Single>().OrderBy(a => a.pos).ToList();
                    var bar_z = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_Zigzag>().OrderBy(a => a.pos).ToList();
                    var bar_d = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_DoubleNet>().OrderBy(a => a.pos).ToList();
                    var bar_io = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_InsideAndOutside>().OrderBy(a => a.pos).ThenBy(a => a.pos2).ToList();
                    if (bar_s.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "シングル配筋");
                        for (int j = 0; j < bar_s.Count; ++j)
                        {
                            string[] d = Get_D2(bar_s[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_s[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_s[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_s[j].strength);
                            }
                        }
                    }
                    else if (bar_z.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "千鳥配筋");
                        for (int j = 0; j < bar_z.Count; ++j)
                        {
                            string[] d = Get_D2(bar_z[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_z[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_z[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_z[j].strength);
                            }
                        }
                    }
                    else if (bar_d.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋");
                        for (int j = 0; j < bar_d.Count; ++j)
                        {
                            string[] d = Get_D2(bar_d[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_d[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_d[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_d[j].strength);
                            }
                        }
                    }
                    else if (bar_io.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋（内外異なる）");
                        for (int j = 0; j < Rwal.D_inout.Length; ++j)
                        {
                            StbSecBarWall_RC_InsideAndOutsidePos pos1 = StbSecBarWall_RC_InsideAndOutsidePos.VERTICAL_OUTSIDE;
                            StbSecBarWall_RC_InsideAndOutsidePos2 pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.ALL;

                            switch (j)
                            {
                                case 0:
                                case 1:
                                case 2:
                                    pos1 = StbSecBarWall_RC_InsideAndOutsidePos.VERTICAL_OUTSIDE;
                                    break;
                                case 3:
                                case 4:
                                case 5:
                                    pos1 = StbSecBarWall_RC_InsideAndOutsidePos.VERTICAL_INSIDE;
                                    break;
                                case 6:
                                case 7:
                                case 8:
                                    pos1 = StbSecBarWall_RC_InsideAndOutsidePos.HORIZONTAL_OUTSIDE;
                                    break;
                                case 9:
                                case 10:
                                case 11:
                                    pos1 = StbSecBarWall_RC_InsideAndOutsidePos.HORIZONTAL_INSIDE;
                                    break;
                            }

                            switch (j)
                            {
                                case 0:
                                case 3:
                                case 6:
                                case 9:
                                    pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.TOP_START;
                                    break;
                                case 1:
                                case 4:
                                case 7:
                                case 10:
                                    pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.MIDDLE;
                                    break;
                                case 2:
                                case 5:
                                case 8:
                                case 11:
                                    pos2 = StbSecBarWall_RC_InsideAndOutsidePos2.BOTTOM_END;
                                    break;
                            }

                            var b = bar_io.Find(a => a.pos == pos1 && (a.pos2 == pos2 || a.pos2 == StbSecBarWall_RC_InsideAndOutsidePos2.ALL));
                            if (b != null)
                            {
                                string[] d = Get_D2(b.D);
                                Data.SetParameter(symbol.LookupParameter(Rwal.D_inout[j]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rwal.D2_inout[j]), d[1]);
                                Data.SetParameter(symbol.LookupParameter(Rwal.pitch_inout[j]), b.pitch, true);

                                if (strength == "")
                                {
                                    strength = b.strength;
                                }
                                else
                                {
                                    strength = Data.Compare_strength(strength, b.strength);
                                }
                            }
                        }
                    }


                    //鉄筋種別の方がTextとMaterialとあるため、Textの方へ値を入れる
                    IList<Parameter> paras = symbol.GetParameters(Rwal.strength);
                    for (int i = 0; i < paras.Count(); i++)
                    {
                        if (paras[i].StorageType != StorageType.String) { continue; }
                        Data.SetParameter(paras[i], strength);
                    }


                    //端部補強筋
                    var bar_edge = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_Edge>().OrderBy(a => a.pos).ToList();
                    if (bar_edge.Count > 0)
                    {
                        for (int j = 0; j < Rwal.D_Edge.Length; ++j)
                        {
                            StbSecBarWall_RC_EdgePos pos = (StbSecBarWall_RC_EdgePos)j;
                            var b = bar_edge.Find(a => a.pos == pos);
                            if (b != null)
                            {
                                string[] d = Get_D2(b.D);
                                Data.SetParameter(symbol.LookupParameter(Rwal.D_Edge[j]), d[0]);
                                Data.SetParameter(symbol.LookupParameter(Rwal.count_Edge[j]), b.N);

                                if (strength == "")
                                {
                                    strength = b.strength;
                                }
                                else
                                {
                                    strength = Data.Compare_strength(strength, b.strength);
                                }
                            }
                        }
                    }


                    //開口配筋
                    var bar_open = wal.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_Open>().OrderBy(a => a.pos).ToList();
                    if (bar_open.Count > 0)
                    {
                        for (int j = 0; j < Rwal.D_op.Length; ++j)
                        {
                            StbSecBarWall_RC_OpenPos pos = (StbSecBarWall_RC_OpenPos)j;
                            var b = bar_open.Find(a => a.pos == pos);
                            if (b != null)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rwal.D_op[j]), b.D);
                                Data.SetParameter(symbol.LookupParameter(Rwal.count_op[j]), b.N);
                                Data.SetParameter(symbol.LookupParameter(Rwal.length_op[j]), b.length);

                                if (strength == "")
                                {
                                    strength = b.strength;
                                }
                                else
                                {
                                    strength = Data.Compare_strength(strength, b.strength);
                                }
                            }
                        }
                    }

                    if (stb.StbModel.StbSections.StbSecOpen_RC != null)
                    {
                        List<int> ind_open = new List<int>();

                        //この壁断面を使用して、開口を持っている壁
                        var walls = stb.StbModel.StbMembers.StbWalls.Where(a => a.id_section == wal.id && a.StbOpenIdList != null && a.StbOpenIdList.Count > 0).ToList();
                        foreach (var w in walls)
                        {
                            foreach (var op in w.StbOpenIdList)
                            {
                                var open = stb.StbModel.StbMembers.StbOpens.Find(a => a.id == op.id);
                                if (open != null)
                                {
                                    ind_open.Add(open.id_section);
                                }
                            }
                        }
                        ind_open = ind_open.Distinct().ToList();

                        bool copyflg = false;
                        for (int i = 0; i < ind_open.Count; i++)
                        {
                            var open_rc = stb.StbModel.StbSections.StbSecOpen_RC.Find(a => a.id == ind_open[i]);
                            if (open_rc != null)
                            {
                                if (open_rc.StbSecBarArrangementOpen_RC == null) continue;

                                var bar = open_rc.StbSecBarArrangementOpen_RC.Items.OfType<StbSecBarOpen_RC_Wall>().OrderBy(a => a.pos).ToList();
                                if (bar.Count == 0)
                                {
                                    continue;
                                }

                                if (!copyflg)
                                { copyflg = true; }
                                else
                                {
                                    string newtypename = symbol.Name + "_" + i.ToString();
                                    symbol = (WallType)symbol.Duplicate(newtypename);
                                }

                                for (int k = 0; k < bar.Count; k++)
                                {
                                    StbSecBarWall_RC_OpenPos pos = (StbSecBarWall_RC_OpenPos)k;
                                    var b = bar_open.Find(a => a.pos == pos);
                                    if (b != null)
                                    {
                                        string[] d = Get_D2(b.D);

                                        Data.SetParameter(symbol.LookupParameter(Rwal.D_op[k]), d[0]);
                                        Data.SetParameter(symbol.LookupParameter(Rwal.count_op[k]), b.N);
                                        Data.SetParameter(symbol.LookupParameter(Rwal.length_op[k]), b.length);

                                        if (strength == "")
                                        {
                                            strength = b.strength;
                                        }
                                        else
                                        {
                                            strength = Data.Compare_strength(strength, b.strength);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rwal.strength, strength, wall: symbol);
                }
                else
                {
                    //鉄筋タグが無い→ログ出力
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RC壁]" + symbol.Name + "(断面id=" + wal.id.ToString() + ")");
                }

                Data.SaveGuid(wal.guid, symbol.Id);
            }
            catch (Exception)
            { ret = false; }

            return ret;
        }

        /// <summary>
        /// RCパラペットタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private static bool CreateParapet(ST_BRIDGE stb, StbSecParapet_RC wal, WallType symbol)
        {
            bool ret = true;

            FamilyStructure.Wall Rwal = SetFamily.Wall;

            try
            {
                //構造の設定（壁厚、コンクリート情報)
                ElementId eid = null;
                Object val = wal.strength_concrete;
                if (wal.strength_concrete == "")
                {
                    val = stb.StbCommon.strength_concrete;
                }
                Data.SetMaterial(ref val, ref eid);

                double depth = 0;
                double depth_H = 0;
                double depth_T1 = 0;
                double depth_H1 = 0;
                double depth_H2 = 0;
                double depth_H3 = 0;
                string kind_form = "I";
                if (wal.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeL typeL)
                {
                    depth = typeL.t_T;
                    depth_T1 = typeL.t_T1;
                    depth_H = typeL.depth_H;
                    depth_H1 = typeL.depth_H1;
                    depth_H2 = typeL.depth_H2;
                    kind_form = "L";
                }
                else if (wal.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeT typeT)
                {
                    depth = typeT.t_T;
                    depth_T1 = typeT.t_T1;
                    depth_H = typeT.depth_H;
                    depth_H1 = typeT.depth_H1;
                    depth_H2 = typeT.depth_H2;
                    depth_H3 = typeT.depth_H3;
                    kind_form = "T";
                }
                else if (wal.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeI typeI)
                {
                    depth = typeI.t_T;
                    depth_H = typeI.depth_H;
                    kind_form = "I";
                }

                double depth_T = depth;

                //構造の壁厚・コンクリート強度を設定
                CompoundStructure csWall = symbol.GetCompoundStructure();
                if (depth < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[RCパラペット]" + wal.name + "(断面id=" + wal.id.ToString() + ")" + "は壁厚が0mmなので1mmとして変換します。");
                }
                csWall.SetLayerWidth(0, Commons.mm2ft(depth));
                csWall.SetMaterialId(0, eid);
                symbol.SetCompoundStructure(csWall);

                Data.SetParameter(symbol.LookupParameter(Rwal.SecId), wal.id);
                Data.SetParameter(symbol.LookupParameter(Rwal.name), wal.name);
                Data.SetParameter(symbol.LookupParameter(Rwal.kind_form), kind_form);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_T), depth_T, true);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_H), depth_H, true);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_T1), depth_T1, true);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_H1), depth_H1, true);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_H2), depth_H2, true);
                Data.SetParameter(symbol.LookupParameter(Rwal.depth_H3), depth_H3, true);


                //配筋
                if (wal.StbSecBarArrangementParapet_RC != null)
                {
                    Data.SetParameter(symbol.LookupParameter(Rwal.depth_cover_outside), wal.StbSecBarArrangementParapet_RC.depth_cover_outside);
                    Data.SetParameter(symbol.LookupParameter(Rwal.depth_cover_inside), wal.StbSecBarArrangementParapet_RC.depth_cover_inside);
                    Data.SetParameter(symbol.LookupParameter(Rwal.isTip_line), wal.StbSecBarArrangementParapet_RC.isTipline);

                    string strength = "";

                    var bar_s = wal.StbSecBarArrangementParapet_RC.Items.OfType<StbSecBarParapet_RC_Single>().OrderBy(a => a.pos).ToList();
                    var bar_z = wal.StbSecBarArrangementParapet_RC.Items.OfType<StbSecBarParapet_RC_Zigzag>().OrderBy(a => a.pos).ToList();
                    var bar_d = wal.StbSecBarArrangementParapet_RC.Items.OfType<StbSecBarParapet_RC_DoubleNet>().OrderBy(a => a.pos).ToList();
                    if (bar_s.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "シングル配筋");
                        for (int j = 0; j < bar_s.Count; ++j)
                        {
                            string[] d = Get_D2(bar_s[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_s[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_s[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_s[j].strength);
                            }
                        }
                    }
                    else if (bar_z.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "千鳥配筋");
                        for (int j = 0; j < bar_z.Count; ++j)
                        {
                            string[] d = Get_D2(bar_z[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_z[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_z[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_z[j].strength);
                            }
                        }
                    }
                    else if (bar_d.Count > 0)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋");
                        for (int j = 0; j < bar_d.Count; ++j)
                        {
                            string[] d = Get_D2(bar_d[j].D);

                            Data.SetParameter(symbol.LookupParameter(Rwal.D[j]), d[0]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.D2[j]), d[1]);
                            Data.SetParameter(symbol.LookupParameter(Rwal.pitch[j]), bar_d[j].pitch, true);

                            if (strength == "")
                            {
                                strength = bar_d[j].strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(strength, bar_d[j].strength);
                            }
                        }
                    }


                    //先端補強筋（アゴ筋）
                    var bar_tip = wal.StbSecBarArrangementParapet_RC.Items.OfType<StbSecBarParapet_RC_Tip>().OrderBy(a => a.pos).ToList();
                    if (bar_tip.Count > 0)
                    {
                        for (int j = 0; j < Rwal.D_Tip.Length; ++j)
                        {
                            StbSecBarParapet_RC_TipPos pos = (StbSecBarParapet_RC_TipPos)j;
                            var b = bar_tip.Find(a => a.pos == pos);
                            if (b != null)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rwal.D_Tip[j]), b.D);
                                Data.SetParameter(symbol.LookupParameter(Rwal.pitch_Tip[j]), b.pitch, true);
                                Data.SetParameter(symbol.LookupParameter(Rwal.count_Tip[j]), b.N);

                                if (strength == "")
                                {
                                    strength = b.strength;
                                }
                                else
                                {
                                    strength = Data.Compare_strength(strength, b.strength);
                                }
                            }
                        }
                    }

                    //端部補強筋
                    var bar_edge = wal.StbSecBarArrangementParapet_RC.Items.OfType<StbSecBarParapet_RC_Edge>().OrderBy(a => a.pos).ToList();
                    if (bar_edge.Count > 0)
                    {
                        for (int j = 0; j < Rwal.D_Edge_Para.Length; ++j)
                        {
                            StbSecBarParapet_RC_EdgePos pos = (StbSecBarParapet_RC_EdgePos)j;
                            var b = bar_edge.Find(a => a.pos == pos);
                            if (b != null)
                            {
                                Data.SetParameter(symbol.LookupParameter(Rwal.D_Edge_Para[j]), b.D);
                                Data.SetParameter(symbol.LookupParameter(Rwal.count_Edge_Para[j]), b.N);

                                if (strength == "")
                                {
                                    strength = b.strength;
                                }
                                else
                                {
                                    strength = Data.Compare_strength(strength, b.strength);
                                }
                            }
                        }
                    }


                    Data.Parameter_Select_Set(Rwal.strength, strength, wall: symbol);
                }
                else
                {
                    //鉄筋タグが無い→ログ出力
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RCパラペット]" + symbol.Name + "(断面id=" + wal.id.ToString() + ")");
                }

                Data.SaveGuid(wal.guid, symbol.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }


        /// <summary>
        /// 壁インスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateWall_instance(ST_BRIDGE stb, StbWall wal, WallType symbol, ref string errmsg)
        {
            bool ret = true;
            IList<Curve> profile = new List<Curve>();

            try
            {
                //同じ節点idが含まれているときがある→同じものは消去
                var nodes = wal.StbNodeIdOrderList.Distinct().ToList();
                //節点の数が3未満⇒変換対象外
                if (nodes.Count < 3)
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")節点数が3未満の壁");
                    return ret;
                }

                //節点から配置位置を取得
                List<XYZ> Point0 = new List<XYZ>();
                List<XYZ> Point1 = new List<XYZ>();
                List<int> stbfloorid = new List<int>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    XYZ Pa = Get_Node_Position(stb.StbModel.StbNodes, nodes[i], 0, 0, 0);
                    XYZ Pb = Pa;
                    if (wal.StbWallOffsetList != null)
                    {
                        var offset = wal.StbWallOffsetList.Find(a => a.id_node == nodes[i]);
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

                    int flid = Get_stbFloor_index(stb, nodes[i], false);
                    stbfloorid.Add(flid);
                }

                //法線ベクトル
                XYZ v1 = (Point0[1] - Point0[0]).Normalize();
                XYZ v2 = (Point0[Point0.Count - 1] - Point0[0]).Normalize();
                XYZ normal = (v2.CrossProduct(v1)).Normalize();
                if (normal.GetLength() < 0.01)
                {
                    //法線がゼロベクトル
                    for (int i = Point0.Count - 2; i >= 2; --i)
                    {
                        v2 = (Point0[i] - Point0[0]).Normalize();
                        normal = (v2.CrossProduct(v1)).Normalize();
                        if (normal.GetLength() < 0.01)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (!CheckWall(wal, Point0))
                {
                    return ret;
                }


                //閉じた図形になっているか確認
                double P0_X = Commons.Get_Point_Vec(v1, Point0[0]);
                double P0_Y = Commons.Get_Point_Vec(v2, Point0[0]);
                for (int i = 0; i < Point0.Count(); i++)
                {
                    double Xx = 0, Yy = 0;
                    int cs = -1;
                    double Pi_X = Commons.Get_Point_Vec(v1, Point0[i]);
                    double Pi_Y = Commons.Get_Point_Vec(v2, Point0[i]);

                    if (i == Point0.Count() - 1)
                    {
                        for (int j = 1; j < Point0.Count() - 2; j++)
                        {
                            double Pj_X = Commons.Get_Point_Vec(v1, Point0[j]);
                            double Pj_Y = Commons.Get_Point_Vec(v2, Point0[j]);
                            double Pj1_X = Commons.Get_Point_Vec(v1, Point0[j + 1]);
                            double Pj1_Y = Commons.Get_Point_Vec(v2, Point0[j + 1]);
                            cs = Commons.CalcCross(Pi_X, Pi_Y, P0_X, P0_Y, Pj_X, Pj_Y, Pj1_X, Pj1_Y, out Xx, out Yy);
                            if (Xx == Pj_X && Yy == Pj_Y) { continue; }
                            if (cs == 0) { break; }
                        }
                    }
                    else
                    {
                        for (int j = i + 2; j < Point0.Count() - 1; j++)
                        {
                            if (j == i || j == i + 1) { continue; }
                            double Pj_X = Commons.Get_Point_Vec(v1, Point0[j]);
                            double Pj_Y = Commons.Get_Point_Vec(v2, Point0[j]);
                            double Pj1_X = Commons.Get_Point_Vec(v1, Point0[j + 1]);
                            double Pj1_Y = Commons.Get_Point_Vec(v2, Point0[j + 1]);
                            double Pi1_X = Commons.Get_Point_Vec(v1, Point0[i + 1]);
                            double Pi1_Y = Commons.Get_Point_Vec(v2, Point0[i + 1]);
                            cs = Commons.CalcCross(Pi_X, Pi_Y, Pi1_X, Pi1_Y, Pj_X, Pj_Y, Pj1_X, Pj1_Y, out Xx, out Yy);
                            if (Xx == Pj_X && Yy == Pj_Y) { continue; }
                            if (cs == 0) { break; }
                        }
                    }

                    if (cs == 0)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2200, "[壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")構成する辺が交差している壁");
                        return ret;
                    }
                }

                //同一直線上の点を除外
                for (int i1 = Point0.Count() - 1; i1 >= 0; --i1)
                {
                    int i2 = i1 + 1;
                    if (i2 >= Point0.Count()) { i2 = 0; }

                    int i3 = i1 - 1;
                    if (i3 < 0) i3 = Point0.Count - 1;

                    XYZ va = (Point0[i2] - Point0[i1]).Normalize();
                    XYZ vb = (Point0[i3] - Point0[i1]).Normalize();
                    if (va.CrossProduct(vb).GetLength() < 0.01)
                    {
                        Point0.RemoveAt(i1);
                        Point1.RemoveAt(i1);
                    }
                }

                Level btmLevel = null, topLevel = null;
                double offset_t = 0, offset_b = 0; //設定した上部レベルからのオフセット値(ft)
                for (int i = 0; i < Point0.Count; i++)
                {
                    int j = i + 1;
                    if (j >= Point0.Count) { j = 0; }

                    XYZ Pi = Point0[i];
                    XYZ Pj = Point0[j];

                    //配置レベルの取得
                    Level newlv = null;
                    int index = stbfloorid[i];
                    do
                    {
                        if (index >= 0)
                        {
                            newlv = SearchLevel(stb, index);
                        }
                        else
                        {
                            //節点未登録の場合は高さからレベルを探す
                            newlv = SearchLevel_height(stb, nodes[i], nodes[i]);
                        }
                        index--;
                        if (index < 0) { break; }
                    } while (newlv == null);

                    if (newlv == null)
                    {
                        index = stbfloorid[i];
                        do
                        {
                            newlv = SearchLevel(stb, index);
                            index++;
                            if (index == stb.StbModel.StbStories.Count()) { break; }
                        } while (newlv == null);
                    }

                    if (newlv == null)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + wal.kind_structure + "壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")は基準レベルが取得できないため変換できません。");
                        return ret;
                    }
                    if (btmLevel == null)
                    {
                        btmLevel = newlv;
                        if (btmLevel != null)
                        {
                            if (Pi.Z > Pj.Z)
                            {
                                offset_b = Pj.Z - btmLevel.Elevation;
                            }
                            else
                            {
                                offset_b = Pi.Z - btmLevel.Elevation;
                            }
                        }
                    }
                    if (newlv.Elevation < btmLevel.Elevation)
                    {
                        btmLevel = newlv;
                        if (btmLevel != null)
                        {
                            if (Pi.Z > Pj.Z)
                            {
                                offset_b = Pj.Z - btmLevel.Elevation;
                            }
                            else
                            {
                                offset_b = Pi.Z - btmLevel.Elevation;
                            }
                        }
                    }
                    if (topLevel == null)
                    {
                        topLevel = newlv;
                        if (topLevel != null)
                        {
                            if (Pi.Z > Pj.Z)
                            {
                                offset_t = Pi.Z - topLevel.Elevation;
                            }
                            else
                            {
                                offset_t = Pj.Z - topLevel.Elevation;
                            }
                        }
                    }
                    if (newlv.Elevation > topLevel.Elevation)
                    {
                        topLevel = newlv;
                        if (topLevel != null)
                        {
                            if (Pi.Z > Pj.Z)
                            {
                                offset_t = Pi.Z - topLevel.Elevation;
                            }
                            else
                            {
                                offset_t = Pj.Z - topLevel.Elevation;
                            }
                        }
                    }

                    profile.Add(Line.CreateBound(Pi, Pj));
                }

                //下端のレベルが見つからない→暫定で最上階→上端レベルが見つかる→レベル、オフセットが上書き
                if (btmLevel != null)
                {
                    offset_b = Point0.Min(a => a.Z) - btmLevel.Elevation;
                }
                if (topLevel != null)
                {
                    offset_t = Point0.Max(a => a.Z) - topLevel.Elevation;
                }


                Wall instance = Wall.Create(Commons.doc, profile, symbol.Id, btmLevel.Id, true, normal);

                var pz = Point0.Select(a => a.Z).Distinct().OrderBy(a => a).ToList();
                if (pz.Count <= 2)
                {
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), topLevel.Id);
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET), offset_t);
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), offset_b);
                }
                else
                {
                    //台形形状のときは高さ指定にする。（形状がプロファイルの座標で作れない）
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), ElementId.InvalidElementId);
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM), pz.Max() - pz.Min());
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), offset_b);
                }
                Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_USAGE_PARAM), StructuralInstanceUsage.Wall);

                SetInstanceParameter_Wall(stb, wal, instance);


                //解析線分
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
                AnalyticalPanel panel = AnalyticalPanel.Create(Commons.doc, curves);
                if (panel != null)
                {
                    //構造の役割
                    var p = panel.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleWall);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(panel.Id, instance.Id);
                }


                //開口
                if (wal.StbOpenIdList != null && wal.StbOpenIdList.Count > 0)
                {
                    //開口を作る前に、一度Regenerateしないとエラーが出る
                    Commons.doc.Regenerate();
                    errmsg = "開口";
                    if (!Wall_Open(stb, wal, Point0[0], v1, normal, instance))
                    {
                        ret = false;
                    }
                }

                //変換情報ログの出力
                var nodeIds = nodes.ToArray() ;
                Data.MakeNodeLog( "壁の生成：", "[配置Id " + wal.id.ToString() + "]" + symbol.Name, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, wal.id, "壁", symbol.Name, nodeIds ) ;
                
                Data.SaveGuid(wal.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        private static void SetInstanceParameter_Wall(ST_BRIDGE stb, StbWall wal, Wall instance)
        {
            FamilyStructure.Wall Rwal = SetFamily.Wall;

            Data.SetParameter(instance.LookupParameter(Rwal.MemId), wal.id);
            Data.SetParameter(instance.LookupParameter(Rwal.NameMembers), wal.name);
            Data.SetParameter(instance.LookupParameter(Rwal.kind_structure), wal.kind_structure);
            Data.SetParameter(instance.LookupParameter(Rwal.kind_layout), wal.kind_layout);
            Data.SetParameter(instance.LookupParameter(Rwal.thickness_ex_right), wal.thickness_add_right);
            Data.SetParameter(instance.LookupParameter(Rwal.thickness_ex_left), wal.thickness_add_left);
            Data.SetParameter(instance.LookupParameter(Rwal.kind_wall), wal.kind_wall);
            Data.SetParameter(instance.LookupParameter(Rwal.slit_upper), wal.slit_upper);
            Data.SetParameter(instance.LookupParameter(Rwal.slit_bottom), wal.slit_bottom);
            Data.SetParameter(instance.LookupParameter(Rwal.slit_left), wal.slit_left);
            Data.SetParameter(instance.LookupParameter(Rwal.slit_right), wal.slit_right);
            Data.SetParameter(instance.LookupParameter(Rwal.isPress), wal.isPress);


            string type_outside = "";
            var sec_wall = stb.StbModel.StbSections.StbSecWall_RC.Find(a => a.id == wal.id_section);
            if (sec_wall != null)
            {
                if (sec_wall.StbSecBarArrangementWall_RC != null)
                {
                    if (sec_wall.StbSecBarArrangementWall_RC.Items.OfType<StbSecBarWall_RC_InsideAndOutside>().Count() > 0)
                    {
                        //配筋がダブル（内外異なる）の場合は必須。それ以外は無効。
                        type_outside = wal.type_outside.ToString();
                    }
                }
            }
            Data.SetParameter(instance.LookupParameter(Rwal.type_outside), type_outside);
        }

        private static bool CheckWall(StbWall wal, List<XYZ> Point0)
        {
            //法線ベクトル
            XYZ v1 = (Point0[1] - Point0[0]).Normalize();
            XYZ v2 = (Point0[Point0.Count - 1] - Point0[0]).Normalize();
            XYZ normal = v2.CrossProduct(v1).Normalize();

            if (normal.GetLength() < 0.01)
            {
                //法線がゼロベクトル
                for (int i = Point0.Count - 2; i >= 2; --i)
                {
                    v2 = (Point0[i] - Point0[0]).Normalize();
                    normal = (v2.CrossProduct(v1)).Normalize();
                    if (normal.GetLength() < 0.01)
                    {
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (normal.GetLength() < 0.01)
            {
                LogData.AddLog(LogData.LogKind.Warning, 0, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")法線ベクトルが計算できないため変換できません。");
                return false;
            }

            var wh = Point0.Max(a => a.Z) - Point0.Min(a => a.Z);
            if (wh <= Commons.mm2ft(1))
            {
                LogData.AddLog(LogData.LogKind.Warning, 0, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")高さが1mm以下のため変換できません。");
                return false;
            }

            //同一平面上にすべての節点があるか⇒同一平面上でない場合は変換対象外
            if (!Commons.CalcPlane(normal, Point0))
            {
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")節点が同一面上に無い壁");
                return false;
            }


            //傾斜フラグ
            if (Math.Abs(normal.Z) > gosa)
            {
                //ログ（傾斜壁は生成しない）
                LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")傾斜壁");
                return false;
            }

            return true;
        }

        /// <summary>
        /// パラペットインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateParapet_instance(ST_BRIDGE stb, StbParapet wal, WallType symbol, ref string errmsg)
        {
            bool ret = true;

            FamilyStructure.Wall Rwal = SetFamily.Wall;
            IList<Curve> profile = new List<Curve>();

            try
            {
                double H = 0;
                var sec_para = stb.StbModel.StbSections.StbSecParapet_RC.Find(a => a.id == wal.id_section);
                if (sec_para != null)
                {
                    if (sec_para.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeL typeL)
                    {
                        H = typeL.depth_H;
                    }
                    else if (sec_para.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeT typeT)
                    {
                        H = typeT.depth_H;
                    }
                    else if (sec_para.StbSecFigureParapet_RC.Item is StbSecParapet_RC_TypeI typeI)
                    {
                        H = typeI.depth_H;
                    }
                }
                H = Commons.mm2ft(H);


                XYZ P1 = Get_Node_Position(stb.StbModel.StbNodes, wal.id_node_start, 0, 0, 0);
                XYZ P2 = Get_Node_Position(stb.StbModel.StbNodes, wal.id_node_end, 0, 0, 0);

                XYZ v1 = (P2 - P1).Normalize();
                XYZ v2 = XYZ.BasisZ;
                XYZ normal = (v2.CrossProduct(v1)).Normalize();

                //配置レベルの取得
                int floorind = Get_stbFloor_index(stb, wal.id_node_start);
                Level newlv = SearchLevel(stb, floorind);
                int floorinde = Get_stbFloor_index(stb, wal.id_node_end);
                Level newlve = SearchLevel(stb, floorinde);
                Level btmLevel = null;
                if (newlv.Elevation < newlve.Elevation)
                {
                    btmLevel = newlv;
                }
                else
                {
                    btmLevel = newlve;
                }

                P1 += normal * Commons.mm2ft( wal.offset ) + v2 * Commons.mm2ft( wal.level ) ;
                P2 += normal * Commons.mm2ft( wal.offset ) + v2 * Commons.mm2ft( wal.level ) ;

                profile.Add(Line.CreateBound(P1, P2));
                XYZ P3 = P2 + H * v2;
                profile.Add(Line.CreateBound(P2, P3));
                XYZ P4 = P1 + H * v2;
                profile.Add(Line.CreateBound(P3, P4));
                profile.Add(Line.CreateBound(P4, P1));

                Wall instance = Wall.Create(Commons.doc, profile, symbol.Id, btmLevel.Id, true, normal);
                if (instance != null)
                {
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), ElementId.InvalidElementId);
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM), H);
                    Data.SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), Math.Min(P1.Z, P2.Z) - btmLevel.Elevation);

                    Data.SaveGuid(wal.guid, instance?.Id);
                }
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 開口の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="Ps"></param>
        /// <param name="Vx">X方向単位ベクトル</param>
        /// <param name="N">法線ベクトル</param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static bool Wall_Open(ST_BRIDGE stb, StbWall wal, XYZ Ps, XYZ Vx, XYZ N, Wall instance)
        {
            bool ret = true;

            try
            {
                for (int i = 0; i < wal.StbOpenIdList.Count; i++)
                {
                    var open = stb.StbModel.StbMembers.StbOpens.Find(a => a.id == wal.StbOpenIdList[i].id);
                    if (open == null) continue;

                    if (open.rotate != 0)
                    {
                        //ログ出力
                        LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + wal.kind_structure + "壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")壁開口の回転");
                    }

                    XYZ Vy = -N.CrossProduct(Vx).Normalize();

                    XYZ Pb = Ps + Commons.mm2ft(open.position_X) * Vx + Commons.mm2ft(open.position_Y) * Vy;
                    XYZ Pn1 = Pb + Vx * Commons.mm2ft(open.length_X);
                    XYZ Pn2 = Pn1 + Vy * Commons.mm2ft(open.length_Y);
                    var op = Commons.doc.Create.NewOpening(instance, Pb, Pn2);
                    if (op != null)
                    {
                        LogData.AddLog(LogData.LogKind.Infmoation, 0, "壁開口の生成：\t[配置Id" + open.id.ToString() + "]" + open.name);
                        OutputDebubCommentLog( instance, open.id, "壁開口", open.name, new int[]{} ) ;
                        Data.SaveGuid(open.guid, op.Id);
                    }
                }
            }
            catch (Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 0, "[壁開口]" + wal.name + "(配置Id=" + wal.id.ToString() + ")");
            }


            return ret;
        }


        #endregion


        #region 基礎

        /// <summary>
        /// 基礎の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="syubetu"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateFoundation(ST_BRIDGE stb, string syubetu, ref string errmsg)
        {
            bool ret = true;


            //変換ファミリ配列
            Family[][] ConvFamily = new Family[RevitLNK.BaseText.Length][];
            for (int i = 0; i < RevitLNK.BaseText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.BaseText[i].Length);
            }

            //変換ファミリの取得
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<FamilySymbol> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }


            //パラメータの追加
            Data.ProgressStart("基礎パラメータ追加", ConvFamily.Length);
            for (int i = 0; i < ConvFamily.Length; i++)
            {
                Data.ProgressPerformStep();

                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.FoFName.flg[i][j]) { continue; }
                    if (!SetFamily.FoFName.convflg[i][j]) { continue; }

                    foreach (FamilySymbol familysymbol in elements)
                    {
                        if (familysymbol.FamilyName == SetFamily.FoFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {
                                Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Transaction tran1 = new Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();

                                    FamilyManager fmg = doc.FamilyManager;
                                    switch (i)
                                    {
                                        case 0:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_Foundation_Rect(fmg, SetFamily.FRect);
                                                    break;
                                                case 1:
                                                    ParaSet.SetPara_Foundation_Tapered_Rect(fmg, SetFamily.FTRect);
                                                    break;
                                                case 2:
                                                    ParaSet.SetPara_Foundation_Triangle(fmg, SetFamily.FTri);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_Foundation_ETriangle(fmg, SetFamily.FETriangle);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_Foundation_Octagon(fmg, SetFamily.FOct);
                                                    break;
                                            }
                                            break;
                                        case 1:
                                            ParaSet.SetPara_Foundation_Continuous(fmg, SetFamily.FConti);
                                            break;
                                        case 2:
                                            switch (j)
                                            {
                                                case 0:
                                                    ParaSet.SetPara_Castinpile(fmg, SetFamily.CastinPile);
                                                    break;

                                                case 2:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_S);
                                                    break;
                                                case 3:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PHC);
                                                    break;
                                                case 4:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_ST);
                                                    break;
                                                case 5:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_SC);
                                                    break;
                                                case 6:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_PRC);
                                                    break;
                                                case 7:
                                                    ParaSet.SetPara_Pile(fmg, SetFamily.Pile_CPRC);
                                                    break;
                                            }
                                            break;
                                    }

                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    tran1.Commit();
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.FoFName.FamilyName, familysymbol.FamilyName, i, j);

                                }
                                catch (Exception)
                                {
                                    tran1.RollBack();
                                    doc.Close();
                                }
                            }
                            break;
                        }
                    }
                }
            }

            Data.ProgressClose();


            Transaction tran = new Transaction(Commons.doc, "基礎の生成");

            //基礎タイプ
            if (stb.StbModel.StbSections.StbSecFoundation_RC != null && stb.StbModel.StbSections.StbSecFoundation_RC.Count > 0)
            {
                errmsg = "RC基礎";

                try
                {
                    tran.Start();

                    Data.ProgressRestart("RC基礎断面の生成", stb.StbModel.StbSections.StbSecFoundation_RC.Count);
                    foreach (var fou in stb.StbModel.StbSections.StbSecFoundation_RC)
                    {
                        Data.ProgressPerformStep();

                        if (!CreateFoundation_RC(stb, fou, ConvFamily))
                        {
                            ret = false;
                            errmsg = "RC基礎断面";
                        }
                    }

                    Data.ProgressClose();
                    Commons.doc.Regenerate();
                    tran.Commit();
                }
                catch (Exception)
                {
                    ret = false;
                    tran.RollBack();
                }
            }


            //杭タイプ
            if ((stb.StbModel.StbSections.StbSecPile_RC != null && stb.StbModel.StbSections.StbSecPile_RC.Count > 0) ||
                (stb.StbModel.StbSections.StbSecPile_S != null && stb.StbModel.StbSections.StbSecPile_S.Count > 0) ||
                (stb.StbModel.StbSections.StbSecPileProduct != null && stb.StbModel.StbSections.StbSecPileProduct.Count > 0))
            {
                tran.SetName("杭の生成");
                errmsg = "杭";
                try
                {
                    tran.Start();

                    if (stb.StbModel.StbSections.StbSecPile_RC != null && stb.StbModel.StbSections.StbSecPile_RC.Count > 0)
                    {
                        Data.ProgressRestart("RC杭断面の生成", stb.StbModel.StbSections.StbSecPile_RC.Count);
                        foreach (var pile in stb.StbModel.StbSections.StbSecPile_RC)
                        {
                            Data.ProgressPerformStep();

                            if (!CreatePile_RC(stb, pile, ConvFamily))
                            {
                                ret = false;
                                errmsg = "RC杭断面";
                            }
                        }
                    }
                    if (stb.StbModel.StbSections.StbSecPile_S != null && stb.StbModel.StbSections.StbSecPile_S.Count > 0)
                    {
                        Data.ProgressRestart("鋼管杭断面の生成", stb.StbModel.StbSections.StbSecPile_S.Count);
                        foreach (var pile in stb.StbModel.StbSections.StbSecPile_S)
                        {
                            Data.ProgressPerformStep();

                            if (!CreatePile_S(stb, pile, ConvFamily))
                            {
                                ret = false;
                                errmsg = "鋼管杭断面";
                            }
                        }
                    }
                    if (stb.StbModel.StbSections.StbSecPileProduct != null && stb.StbModel.StbSections.StbSecPileProduct.Count > 0)
                    {
                        Data.ProgressRestart("既製杭断面の生成", stb.StbModel.StbSections.StbSecPileProduct.Count);
                        foreach (var pile in stb.StbModel.StbSections.StbSecPileProduct)
                        {
                            Data.ProgressPerformStep();

                            if (!CreatePile_Product(stb, pile, ConvFamily))
                            {
                                ret = false;
                                errmsg = "既製杭断面";
                            }
                        }
                    }

                    Data.ProgressClose();
                    Commons.doc.Regenerate();
                    tran.Commit();
                }
                catch (Exception)
                {
                    ret = false;
                    tran.RollBack();
                }
            }

            Data.ProgressClose();




            if (ret)
            {
                tran.SetName("基礎インスタンスパラメータの生成");
                try
                {
                    tran.Start();

                    if (stb.StbModel.StbMembers.StbFootings != null && stb.StbModel.StbMembers.StbFootings.Count > 0)
                    {
                        Data.ProgressRestart("基礎の生成", stb.StbModel.StbMembers.StbFootings.Count);
                        foreach (var foo in stb.StbModel.StbMembers.StbFootings)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateFoundation_instance(stb, foo, ConvFamily))
                            {
                                ret = false;
                                errmsg = "フーチング情報";
                            }
                        }
                    }

                    if (stb.StbModel.StbMembers.StbStripFootings != null && stb.StbModel.StbMembers.StbStripFootings.Count > 0)
                    {
                        Data.ProgressRestart("布基礎の生成", stb.StbModel.StbMembers.StbStripFootings.Count);
                        foreach (var foo in stb.StbModel.StbMembers.StbStripFootings)
                        {
                            Data.ProgressPerformStep();

                            if (!CreateStripFooting_instance(stb, foo, ConvFamily))
                            {
                                ret = false;
                                errmsg = "布基礎情報";
                            }
                        }
                    }

                    Data.ProgressClose();
                    Commons.doc.Regenerate();
                    tran.Commit();
                }
                catch (Exception)
                {
                    ret = false;
                    tran.RollBack();
                }
                Data.ProgressClose();




                tran.SetName("杭インスタンスパラメータの生成");
                errmsg = "杭インスタンス";
                try
                {
                    tran.Start();

                    if (stb.StbModel.StbMembers.StbPiles != null && stb.StbModel.StbMembers.StbPiles.Count > 0)
                    {
                        Data.ProgressRestart("杭の生成", stb.StbModel.StbMembers.StbPiles.Count);

                        foreach (var pile in stb.StbModel.StbMembers.StbPiles)
                        {
                            Data.ProgressPerformStep();

                            if (!CreatePile_instance(stb, pile, ConvFamily))
                            {
                                ret = false;
                                errmsg = "杭インスタンス";
                            }
                        }
                    }

                    Data.ProgressClose();
                    Commons.doc.Regenerate();
                    tran.Commit();
                }
                catch (Exception)
                {
                    ret = false;
                    tran.RollBack();
                }
            }

            Data.ProgressClose();

            if (ret == false)
            {
                errmsg = "基礎";
            }

            return ret;
        }
        

        /// <summary>
        /// RC基礎タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateFoundation_RC(ST_BRIDGE stb, StbSecFoundation_RC fo, Family[][] ConvFamily)
        {
            bool ret = true;

            //タイプ名
            string typename = GetTypeName_Footing(stb, fo.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, "[RC基礎](断面id=" + fo.id.ToString() + ")");
                return ret;
            }


            FamilySymbol symbol = null;

            switch (fo.StbSecFigureFoundation_RC.FigureType)
            {
                case 1:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_Rect)
                    {
                        if (ConvFamily[0][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "RC基礎矩形");
                        }
                        else
                        {
                            //タイプの生成
                            if (footingType[0].ContainsKey(fo.id))
                            {
                                symbol = Commons.doc.GetElement(footingType[0][fo.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][0], ref typename);
                                footingType[0].Add(fo.id, symbol?.Id);
                            }

                            SetParameter_RCFoundation_Rect(fo, symbol);
                        }
                    }
                    break;

                case 2:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_TaperedRect)
                    {
                        if (ConvFamily[0][1] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "RC基礎矩形テーパー");
                        }
                        else
                        {
                            //タイプの生成
                            if (footingType[0].ContainsKey(fo.id))
                            {
                                symbol = Commons.doc.GetElement(footingType[0][fo.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][1], ref typename);
                                footingType[0].Add(fo.id, symbol?.Id);
                            }

                            SetParameter_RCFoundation_Taper(fo, symbol);
                        }
                    }
                    break;

                case 3:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_Triangle)
                    {
                        if (ConvFamily[0][2] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎三角");
                        }
                        else
                        {
                            //タイプの生成
                            if (footingType[0].ContainsKey(fo.id))
                            {
                                symbol = Commons.doc.GetElement(footingType[0][fo.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][2], ref typename);
                                footingType[0].Add(fo.id, symbol?.Id);
                            }

                            SetParameter_RCFoundation_Triangle(fo, symbol);
                        }
                    }
                    break;

                case 4:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_EquiTriangle)
                    {
                        if (ConvFamily[0][3] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎正三角形");
                        }
                        else
                        {
                            //タイプの生成
                            if (footingType[0].ContainsKey(fo.id))
                            {
                                symbol = Commons.doc.GetElement(footingType[0][fo.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][3], ref typename);
                                footingType[0].Add(fo.id, symbol?.Id);
                            }

                            SetParameter_RCFoundation_EquiTriangle(fo, symbol);
                        }
                    }
                    break;

                case 5:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_Octagon)
                    {
                        if (ConvFamily[0][4] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎八角形");
                        }
                        else
                        {
                            //タイプの生成
                            if (footingType[0].ContainsKey(fo.id))
                            {
                                symbol = Commons.doc.GetElement(footingType[0][fo.id]) as FamilySymbol;
                            }
                            else
                            {
                                symbol = GetFamilySymbol_Column(ConvFamily[0][4], ref typename);
                                footingType[0].Add(fo.id, symbol?.Id);
                            }

                            SetParameter_RCFoundation_Octagon(fo, symbol);
                        }
                    }
                    break;

                case 6:
                    if (fo.StbSecFigureFoundation_RC.Item is StbSecFoundation_RC_Continuous)
                    {
                        if (ConvFamily[1][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "布基礎");
                        }
                        else
                        {
                            for (int i = 0; i < stb.StbModel.StbMembers.StbStripFootings.Count; i++)
                            {
                                var strip_fo = stb.StbModel.StbMembers.StbStripFootings[i];
                                if (strip_fo.id_section != fo.id) { continue; }

                                double t_B = Get_Girder_B(stb, strip_fo.id_node_start, strip_fo.id_node_end);

                                if (Data.Search_Same_FoundationFamily(typename, t_B))
                                {
                                    int ascii = 97;
                                    string oldname = typename;
                                    do
                                    {
                                        typename = Data.ReName(oldname, ascii);
                                        ascii++;
                                    } while (Data.Search_Same_FoundationFamily(typename, t_B));
                                }

                                //タイプの生成                        
                                if (!Data.SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                                {
                                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                                }


                                Data.ReNameSymbols re = new Data.ReNameSymbols
                                {
                                    name = fo.name,
                                    Length = t_B,
                                    symbol = symbol,
                                    id = fo.id
                                };
                                FContiSymbols.Add(re);

                                SetParameter_RCFoundation_Continuous(fo, symbol, t_B);
                            }
                        }
                    }
                    break;
            }


            Data.SaveGuid(fo.guid, symbol?.Id);

            return ret;
        }

        private static string GetTypeName_Footing(ST_BRIDGE stb, int id)
        {
            string typename = stb.StbModel.StbMembers.StbFootings.Find(a => a.id_section == id)?.name;
            if (typename == null || typename == "")
            {
                typename = stb.StbModel.StbMembers.StbStripFootings.Find(a => a.id_section == id)?.name;
            }

            return typename;
        }



        private static void SetParameter_RCFoundation_Continuous(StbSecFoundation_RC fo, FamilySymbol symbol, double t_B)
        {
            FamilyStructure.Foundation_Continuous Rfo = SetFamily.FConti;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            Data.SetParameter(symbol.LookupParameter(Rfo.t_B), t_B, true);

            StbSecFoundation_RC_Continuous continuous = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_Continuous;
            Data.SetParameter(symbol.LookupParameter(Rfo.B), continuous.width, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.depth_base), continuous.depth_base, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.depth_tip), continuous.depth_tip, true);
            switch (continuous.type)
            {
                case StbSecFoundation_RC_ContinuousType.RIGHT_L:
                    Data.SetParameter(symbol.LookupParameter(Rfo.type_right), true);
                    break;
                case StbSecFoundation_RC_ContinuousType.LEFT_L:
                    Data.SetParameter(symbol.LookupParameter(Rfo.type_left), true);
                    break;
            }
            Data.SetParameter(symbol.LookupParameter(Rfo.type), continuous.type.ToString());


            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_Continuous>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";

                    for (int b = 0; b < Rfo.D.Length; b++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == b);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[b]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[b]), bar.N);
                            Data.SetParameter(symbol.LookupParameter(Rfo.pitch[b]), bar.pitch, true);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        private static void SetParameter_RCFoundation_Octagon(StbSecFoundation_RC fo, FamilySymbol symbol)
        {
            FamilyStructure.Foundation_Octagon Rfo = SetFamily.FOct;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            StbSecFoundation_RC_Octagon octagon = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_Octagon;
            Data.SetParameter(symbol.LookupParameter(Rfo.depth), octagon.depth, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DX), octagon.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DY), octagon.width_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CX1), octagon.width_chamfer1_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CY1), octagon.width_chamfer1_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CX2), octagon.width_chamfer2_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CY2), octagon.width_chamfer2_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CX3), octagon.width_chamfer3_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CY3), octagon.width_chamfer3_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CX4), octagon.width_chamfer4_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.CY4), octagon.width_chamfer4_Y, true);

            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_Rect>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";

                    for (int i = 0; i < Rfo.D.Length; i++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == i);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.N);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        private static void SetParameter_RCFoundation_EquiTriangle(StbSecFoundation_RC fo, FamilySymbol symbol)
        {
            FamilyStructure.Foundation_Equi_Triangle Rfo = SetFamily.FETriangle;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            StbSecFoundation_RC_EquiTriangle equiTriangle = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_EquiTriangle;
            Data.SetParameter(symbol.LookupParameter(Rfo.depth), equiTriangle.depth, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.B), equiTriangle.width_base, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.C), equiTriangle.width_chamfer, true);

            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_ThreeWay>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";
                    for (int i = 0; i < Rfo.D.Length; i++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == i);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.N);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        private static void SetParameter_RCFoundation_Triangle(StbSecFoundation_RC fo, FamilySymbol symbol)
        {
            FamilyStructure.Foundation_Triangle Rfo = SetFamily.FTri;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            StbSecFoundation_RC_Triangle triangle = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_Triangle;
            Data.SetParameter(symbol.LookupParameter(Rfo.depth), triangle.depth, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DX), triangle.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DY), triangle.width_Y, true);

            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_Triangle>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";
                    for (int i = 0; i < Rfo.D.Length; i++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == i);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.N);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        private static void SetParameter_RCFoundation_Taper(StbSecFoundation_RC fo, FamilySymbol symbol)
        {
            FamilyStructure.Foundation_Tapered_Rect Rfo = SetFamily.FTRect;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            StbSecFoundation_RC_TaperedRect taper = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_TaperedRect;
            Data.SetParameter(symbol.LookupParameter(Rfo.DX), taper.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DY), taper.width_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.t_DX), taper.width_X / 2, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.t_DY), taper.width_Y / 2, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.depth_base), taper.depth_base, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.depth_tip), taper.depth_tip, true);

            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_Rect>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";

                    for (int i = 0; i < Rfo.D.Length; i++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == i);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.N);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        private static void SetParameter_RCFoundation_Rect(StbSecFoundation_RC fo, FamilySymbol symbol)
        {
            FamilyStructure.Foundation_Rect Rfo = SetFamily.FRect;
            Data.SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
            Data.SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
            Data.SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);

            StbSecFoundation_RC_Rect rect = fo.StbSecFigureFoundation_RC.Item as StbSecFoundation_RC_Rect;
            Data.SetParameter(symbol.LookupParameter(Rfo.DX), rect.width_X, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.DY), rect.width_Y, true);
            Data.SetParameter(symbol.LookupParameter(Rfo.depth), rect.depth, true);

            //配筋
            if (fo.StbSecBarArrangementFoundation_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.StbSecBarArrangementFoundation_RC.depth_cover_top, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.StbSecBarArrangementFoundation_RC.depth_cover_bottom, true);
                Data.SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.StbSecBarArrangementFoundation_RC.depth_cover_side, true);

                var bar0 = fo.StbSecBarArrangementFoundation_RC.Items.OfType<StbSecBarFoundation_RC_Rect>().OrderBy(a => a.pos).ToList();
                if (bar0.Count > 0)
                {
                    string strength = "";

                    for (int i = 0; i < Rfo.D.Length; i++)
                    {
                        var bar = bar0.Find(a => (int)a.pos == i);
                        if (bar != null)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                            Data.SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.N);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Data.Compare_strength(bar.strength, strength);
                            }
                        }
                    }

                    Data.Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                }
            }
        }

        /// <summary>
        /// RC杭タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pile"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreatePile_RC(ST_BRIDGE stb, StbSecPile_RC pile, Family[][] ConvFamily)
        {
            bool ret = true;
            string type = "RC杭";

            //タイプ名
            string typename = GetTypeName_Pile(stb, pile.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{type}](断面id=" + pile.id.ToString() + ")");
                return ret;
            }


            Family fam = null;
            FamilySymbol symbol = null;

            if (ConvFamily[2][0] != null)
            {
                fam = ConvFamily[2][0];
            }

            if (fam == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, type);
                return ret;
            }

            //タイプの生成
            if (!Data.SearchFamilySymbol(fam, typename, ref symbol))
            {
                symbol = (FamilySymbol)symbol.Duplicate(typename);
            }
            else
            {
                symbol = GetFamilySymbol_Column(fam, ref typename);
            }
            Data.SaveGuid(pile.guid, symbol.Id);

            List<double[]> length = GetPile_length(stb, pile.id);

            for (int i = 0; i < length.Count; i++)
            {
                if (i != 0)
                {
                    //同一符号、長さ違い
                    symbol = (FamilySymbol)symbol.Duplicate(typename + "_" + i.ToString());
                    Data.SaveGuid(pile.guid, symbol.Id);
                }

                Data.ReNameSymbols s = new Data.ReNameSymbols
                {
                    symbol = symbol,
                    id = pile.id,
                    name = pile.name,
                    Length = length[i][0],
                    Length2 = length[i][1],
                    Length3 = length[i][2],
                    BHaunch1 = StbPileKind_structure.RC.ToString(),
                };
                PilesSymbols.Add(s);


                //形状
                switch (type)
                {
                    case "RC杭":
                        SetParameter_RCPile(pile, symbol, length[i]);
                        break;

                }

            }

            return ret;
        }

        private static List<double[]> GetPile_length(ST_BRIDGE stb, int id)
        {
            List<double[]> length = new List<double[]>();

            if (stb.StbModel.StbMembers.StbPiles != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbPiles.Count; i++)
                {
                    if (stb.StbModel.StbMembers.StbPiles[i].id_section == id)
                    {
                        var stbp = stb.StbModel.StbMembers.StbPiles[i];
                        bool addflg = true;
                        for (int j = 0; j < length.Count(); j++)
                        {
                            if (Math.Abs(length[j][0] - stbp.length_all) < 1 &&
                                Math.Abs(length[j][1] - stbp.length_head) < 1 &&
                                Math.Abs(length[j][2] - stbp.length_foot) < 1)
                            {
                                addflg = false;
                                break;
                            }
                        }
                        if (addflg)
                        {
                            length.Add(new double[] { stbp.length_all, stbp.length_head, stbp.length_foot });
                        }
                    }
                }
            }

            return length;
        }

        private static void SetParameter_RCPile(StbSecPile_RC pile, FamilySymbol symbol, double[] length)
        {
            bool length_Log = false;

            FamilyStructure.Pile Rpile = SetFamily.CastinPile;
            Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
            Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
            Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), pile.strength_concrete);

            double L_foot = 0;
            double L_top = 0;

            switch (pile.StbSecFigurePile_RC.FigureType)
            {
                case 1:
                    var fig_s = (StbSecPile_RC_Straight)pile.StbSecFigurePile_RC.Item;
                    Data.SetParameter(symbol.LookupParameter(Rpile.D), fig_s.D, true);
                    if (length != null)
                    {
                        if (length[0] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_all), length[0], true);
                        }
                        else
                        {
                            length_Log = true;
                        }
                    }
                    break;

                case 2:
                    var fig_f = (StbSecPile_RC_ExtendedFoot)pile.StbSecFigurePile_RC.Item;
                    Data.SetParameter(symbol.LookupParameter("拡底"), true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D), fig_f.D_axial, true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D_extended_foot), fig_f.D_extended_foot, true);
                    if (length != null)
                    {
                        if (length[0] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_all), length[0], true);
                        }
                        else
                        {
                            length_Log = true;
                        }
                        if (length[2] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_foot), length[2], true);
                        }
                        else
                        {
                            length_Log = true;
                        }
                    }

                    if (0 < fig_f.angle_extended_foot_taper && fig_f.angle_extended_foot_taper < 90)
                    {
                        double Dfoot_diff = (fig_f.D_extended_foot - fig_f.D_axial) / 2;
                        L_foot = Dfoot_diff / Math.Tan(fig_f.angle_extended_foot_taper * Math.PI / 180);
                    }
                    L_foot = L_foot > 1 ? L_foot : 1000;

                    Data.SetParameter(symbol.LookupParameter(Rpile.length_foot_taper), L_foot, true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.length_foot_Revit), fig_f.length_extended_foot, true);
                    break;

                case 3:
                    var fig_t = (StbSecPile_RC_ExtendedTop)pile.StbSecFigurePile_RC.Item;
                    Data.SetParameter(symbol.LookupParameter("拡頭"), true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D), fig_t.D_axial, true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D_extended_top), fig_t.D_extended_top, true);
                    if (length != null)
                    {
                        if (length[0] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_all), length[0], true);
                        }
                        else
                        {
                            length_Log = true;
                        }
                        if (length[1] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_head), length[1], true);
                        }
                        else
                        {
                            length_Log = true;
                        }

                        if (0 < fig_t.angle_extended_top_taper && fig_t.angle_extended_top_taper < 90)
                        {
                            double Dtop_diff = (fig_t.D_extended_top - fig_t.D_axial) / 2;
                            L_top = Dtop_diff / Math.Tan(fig_t.angle_extended_top_taper * Math.PI / 180);
                        }
                        L_top = L_top > 1 ? L_top : length[1] / 2;

                        Data.SetParameter(symbol.LookupParameter(Rpile.length_head_taper), L_top, true);
                    }
                    break;

                case 4:
                    var fig_tf = (StbSecPile_RC_ExtendedTopFoot)pile.StbSecFigurePile_RC.Item;
                    Data.SetParameter(symbol.LookupParameter("拡底"), true);
                    Data.SetParameter(symbol.LookupParameter("拡頭"), true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D), fig_tf.D_axial, true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D_extended_top), fig_tf.D_extended_top, true);
                    Data.SetParameter(symbol.LookupParameter(Rpile.D_extended_foot), fig_tf.D_extended_foot, true);
                    if (length != null)
                    {
                        if (length[0] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_all), length[0], true);
                        }
                        else
                        {
                            length_Log = true;
                        }
                        if (length[1] != 0)
                        {
                            Data.SetParameter(symbol.LookupParameter(Rpile.length_head), length[1], true);
                        }
                        else
                        {
                            length_Log = true;
                        }


                        if (0 < fig_tf.angle_extended_foot_taper && fig_tf.angle_extended_foot_taper < 90)
                        {
                            double Dfoot_diff2 = (fig_tf.D_extended_foot - fig_tf.D_axial) / 2;
                            L_foot = Dfoot_diff2 / Math.Tan(fig_tf.angle_extended_foot_taper * Math.PI / 180);
                        }
                        L_foot = L_foot > 1 ? L_foot : 1000;

                        if (0 < fig_tf.angle_extended_top_taper && fig_tf.angle_extended_top_taper < 90)
                        {
                            double Dtop_diff2 = (fig_tf.D_extended_top - fig_tf.D_axial) / 2;
                            L_top = Dtop_diff2 / Math.Tan(fig_tf.angle_extended_top_taper * Math.PI / 180);
                        }
                        L_top = L_top > 1 ? L_top : length[1] / 2;

                        Data.SetParameter(symbol.LookupParameter(Rpile.length_foot), length[2], true);
                        Data.SetParameter(symbol.LookupParameter(Rpile.length_foot_taper), L_foot, true);
                        Data.SetParameter(symbol.LookupParameter(Rpile.length_head_taper), L_top, true);
                    }
                    Data.SetParameter(symbol.LookupParameter(Rpile.length_foot_Revit), fig_tf.length_extended_foot, true);

                    break;
            }

            if (length_Log == true)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "杭長さが0mmのためファミリのデフォルト値で変換しました。");
                Data.SetParameter(symbol.LookupParameter(Rpile.zeroLength), true);
            }
            else
            {
                Data.SetParameter(symbol.LookupParameter(Rpile.zeroLength), false);
            }

            //配筋
            if (pile.StbSecBarArrangementPile_RC != null)
            {
                Data.SetParameter(symbol.LookupParameter(Rpile.depth_cover), pile.StbSecBarArrangementPile_RC.depth_cover);
                Data.SetParameter(symbol.LookupParameter(Rpile.depth_cover_top), pile.StbSecBarArrangementPile_RC.depth_cover_top);

                var bar_same = pile.StbSecBarArrangementPile_RC.Items.OfType<StbSecBarPile_RC_Same>().ToList();
                var bar_tb = pile.StbSecBarArrangementPile_RC.Items.OfType<StbSecBarPile_RC_TopBottom>().OrderBy(a => a.pos).ToList();
                var bar_tcb = pile.StbSecBarArrangementPile_RC.Items.OfType<StbSecBarPile_RC_TopCenterBottom>().OrderBy(a => a.pos).ToList();
                if (bar_same.Count > 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), bar_same.First().D_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), bar_same.First().N_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), bar_same.First().D_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), bar_same.First().N_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_band[j]), bar_same.First().D_band);
                        Data.SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), bar_same.First().pitch_band, true);
                    }
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), bar_same.First().strength_main_circumference_1st);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_core), bar_same.First().strength_main_core);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_band), bar_same.First().strength_band);
                }
                else if (bar_tb.Count > 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int newj = 0;
                        if (j == 1)
                        { newj = 0; }
                        else if (j == 2)
                        { newj = 1; }
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), bar_tb[newj].D_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), bar_tb[newj].N_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), bar_tb[newj].D_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), bar_tb[newj].N_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_band[j]), bar_tb[newj].D_band);
                        Data.SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), bar_tb[newj].pitch_band, true);
                    }
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), bar_tb[0].strength_main_circumference_1st);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_core), bar_tb[0].strength_main_core);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_band), bar_tb[0].strength_band);
                }
                else if (bar_tcb.Count > 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), bar_tcb[j].D_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), bar_tcb[j].N_main_circumference_1st);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), bar_tcb[j].D_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), bar_tcb[j].N_main_core);
                        Data.SetParameter(symbol.LookupParameter(Rpile.D_band[j]), bar_tcb[j].D_band);
                        Data.SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), bar_tcb[j].pitch_band, true);
                    }
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), bar_tcb[0].strength_main_circumference_1st);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_main_core), bar_tcb[0].strength_main_core);
                    Data.SetParameter(symbol.LookupParameter(Rpile.strength_band), bar_tcb[0].strength_band);
                }
            }

        }

        /// <summary>
        /// 鋼管杭タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pile"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreatePile_S(ST_BRIDGE stb, StbSecPile_S pile, Family[][] ConvFamily)
        {
            bool ret = true;
            string type = "鋼管杭";

            //タイプ名
            string typename = GetTypeName_Pile(stb, pile.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{type}](断面id=" + pile.id.ToString() + ")");
                return ret;
            }

            Family fam = null;
            FamilySymbol symbol = null;

            if (ConvFamily[2][2] != null)
            {
                fam = ConvFamily[2][2];
            }

            if (fam == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, type);
                return ret;
            }

            //タイプの生成
            if (!Data.SearchFamilySymbol(fam, typename, ref symbol))
            {
                symbol = (FamilySymbol)symbol.Duplicate(typename);
            }
            else
            {
                symbol = GetFamilySymbol_Column(fam, ref typename);
            }
            Data.SaveGuid(pile.guid, symbol.Id);

            //形状
            switch (type)
            {
                case "鋼管杭":
                    SetParameter_SPile(pile, symbol);
                    break;
            }

            Data.ReNameSymbols s = new Data.ReNameSymbols
            {
                symbol = symbol,
                id = pile.id,
                name = pile.name,
                Length = 0,
                Length2 = 0,
                Length3 = 0,
                BHaunch1 = StbPileKind_structure.S.ToString(),
            };
            PilesSymbols.Add(s);


            //タイプパラメータに長さがないので、長さ別にする必要ない
            /*
            List<double[]> length = GetPile_length(stb, pile.id);
            for (int i = 0; i < length.Count; i++)
            {
                if (i != 0)
                {
                    //同一符号、長さ違い
                    symbol = (FamilySymbol)symbol.Duplicate(typename + i.ToString());
                    Data.ReNameSymbols s = new Data.ReNameSymbols
                    {
                        symbol = symbol,
                        name = pile.name,
                        Length = length[i][0],
                        Length2 = length[i][1],
                        Length3 = length[i][2]
                    };
                    PilesSymbols.Add(s);

                    Data.SaveGuid(pile.guid, symbol.Id);
                }


                //形状
                switch (type)
                {
                    case "鋼管杭":
                        SetParameter_SPile(pile, symbol);
                        break;
                }
            }
            //*/

            return ret;
        }

        private static void SetParameter_SPile(StbSecPile_S pile, FamilySymbol symbol)
        {
            var Rpile = SetFamily.Pile_S;
            Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
            Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);

            if (pile.StbSecFigurePile_S.StbSecPile_S_Straight != null &&
                pile.StbSecFigurePile_S.StbSecPile_S_Straight.Count > 0)
            {
                var fig = pile.StbSecFigurePile_S.StbSecPile_S_Straight.OrderBy(a => a.id_order).FirstOrDefault();
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t), fig.t, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength), fig.strength);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (pile.StbSecFigurePile_S.StbSecPile_S_Rotational != null &&
                     pile.StbSecFigurePile_S.StbSecPile_S_Rotational.Count > 0)
            {
                //ストレートとして変換
                var fig = pile.StbSecFigurePile_S.StbSecPile_S_Rotational.OrderBy(a => a.id_order).FirstOrDefault();
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t), fig.t, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength), fig.strength);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (pile.StbSecFigurePile_S.StbSecPile_S_Taper != null &&
                     pile.StbSecFigurePile_S.StbSecPile_S_Taper.Count > 0)
            {
                //ストレートとして変換
                var fig = pile.StbSecFigurePile_S.StbSecPile_S_Taper.OrderBy(a => a.id_order).FirstOrDefault();
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t), fig.t, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength), fig.strength);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
        }

        private static bool CreatePile_Product(ST_BRIDGE stb, StbSecPileProduct pile, Family[][] ConvFamily)
        {
            bool ret = true;

            CheckConvertFamily_PileProduct(pile, ConvFamily, out Family fam, out string type);

            //タイプ名
            string typename = GetTypeName_Pile(stb, pile.id);
            if (typename == null || typename == "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 3200, $"[{type}](断面id=" + pile.id.ToString() + ")");
                return ret;
            }

            if (fam == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, type);
                return ret;
            }

            //タイプの生成
            FamilySymbol symbol = null;
            if (!Data.SearchFamilySymbol(fam, typename, ref symbol))
            {
                symbol = (FamilySymbol)symbol.Duplicate(typename);
            }
            else
            {
                symbol = GetFamilySymbol_Column(fam, ref typename);
            }
            Data.SaveGuid(pile.guid, symbol.Id);

            SetParameter_PileProduct(pile, symbol, type);

            Data.ReNameSymbols s = new Data.ReNameSymbols
            {
                symbol = symbol,
                id = pile.id,
                name = pile.name,
                Length = 0,
                Length2 = 0,
                Length3 = 0,
                BHaunch1 = StbPileKind_structure.PC.ToString(),
            };
            PilesSymbols.Add(s);

            /*
            List<double[]> length = GetPile_length(stb, pile.id);

            for (int i = 0; i < length.Count; i++)
            {
                if (i != 0)
                {
                    //同一符号、長さ違い
                    symbol = (FamilySymbol)symbol.Duplicate(typename + i.ToString());
                    Data.ReNameSymbols s = new Data.ReNameSymbols
                    {
                        symbol = symbol,
                        name = pile.name,
                        Length = length[i][0],
                        Length2 = length[i][1],
                        Length3 = length[i][2]
                    };
                    PilesSymbols.Add(s);

                    Data.SaveGuid(pile.guid, symbol.Id);
                }

                SetParameter_PileProduct(pile, symbol, type);
            }
            //*/

            return ret;
        }

        private static void CheckConvertFamily_PileProduct(StbSecPileProduct pile, Family[][] ConvFamily, out Family fam, out string type)
        {
            fam = null;
            type = "";

            if (pile.StbSecFigurePileProduct.StbSecPileProduct_PHC != null &&
                pile.StbSecFigurePileProduct.StbSecPileProduct_PHC.Count > 0)
            {
                type = "PHC杭";

                if (ConvFamily[2][3] != null)
                {
                    fam = ConvFamily[2][3];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProduct_ST != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProduct_ST.Count > 0)
            {
                type = "ST杭";

                if (ConvFamily[2][4] != null)
                {
                    fam = ConvFamily[2][4];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProduct_SC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProduct_SC.Count > 0)
            {
                type = "SC杭";

                if (ConvFamily[2][5] != null)
                {
                    fam = ConvFamily[2][5];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProduct_PRC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProduct_PRC.Count > 0)
            {
                type = "PRC杭";

                if (ConvFamily[2][6] != null)
                {
                    fam = ConvFamily[2][6];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProduct_CPRC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProduct_CPRC.Count > 0)
            {
                type = "CPRC杭";

                if (ConvFamily[2][7] != null)
                {
                    fam = ConvFamily[2][7];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProductNodular_PHC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProductNodular_PHC.Count > 0)
            {
                type = "節付PHC杭";

                if (ConvFamily[2][3] != null)
                {
                    //節なしで変換する
                    fam = ConvFamily[2][3];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProductNodular_PRC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProductNodular_PRC.Count > 0)
            {
                type = "節付PRC杭";

                if (ConvFamily[2][6] != null)
                {
                    //節なしで変換する
                    fam = ConvFamily[2][6];
                }
            }
            else if (pile.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC != null &&
                     pile.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC.Count > 0)
            {
                type = "節付CPRC杭";

                if (ConvFamily[2][7] != null)
                {
                    //節なしで変換する
                    fam = ConvFamily[2][7];
                }
            }
        }

        private static void SetParameter_PileProduct(StbSecPileProduct pile, FamilySymbol symbol, string type)
        {
            //形状
            if (type == "PHC杭")
            {
                var Rpile = SetFamily.Pile_PHC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProduct_PHC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t), fig.t, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "ST杭")
            {
                var Rpile = SetFamily.Pile_ST;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProduct_ST.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D1), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.D2), fig.D2, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t1), fig.t1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t2), fig.t2, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "SC杭")
            {
                var Rpile = SetFamily.Pile_SC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProduct_SC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.tc), fig.tc, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.ts), fig.ts, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_pipe), fig.strength_pipe);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "PRC杭")
            {
                var Rpile = SetFamily.Pile_PRC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProduct_PRC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.tc), fig.tc, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_bar), fig.D_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_bar), fig.N_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_bar), fig.strength_bar);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "CPRC杭")
            {
                var Rpile = SetFamily.Pile_CPRC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProduct_CPRC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.tc), fig.tc, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_bar), fig.D_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_bar), fig.N_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_bar), fig.strength_bar);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "節付PHC杭")
            {
                var Rpile = SetFamily.Pile_PHC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProductNodular_PHC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.t), fig.t, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "節付PRC杭")
            {
                var Rpile = SetFamily.Pile_PRC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProductNodular_PRC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.tc), fig.tc, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_bar), fig.D_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_bar), fig.N_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_bar), fig.strength_bar);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
            else if (type == "節付CPRC杭")
            {
                var Rpile = SetFamily.Pile_CPRC;
                var fig = pile.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC.OrderBy(a => a.id_order).FirstOrDefault();

                Data.SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                Data.SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                Data.SetParameter(symbol.LookupParameter(Rpile.length_pile), fig.length_pile, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.kind), fig.kind);
                Data.SetParameter(symbol.LookupParameter(Rpile.D), fig.D1, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.tc), fig.tc, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_concrete), fig.strength_concrete);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_PC), fig.D_PC, true);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_PC), fig.N_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_PC), fig.strength_PC);
                Data.SetParameter(symbol.LookupParameter(Rpile.D_bar), fig.D_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.N_bar), fig.N_bar);
                Data.SetParameter(symbol.LookupParameter(Rpile.strength_bar), fig.strength_bar);

                //製造元
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER), fig.product_company);
                //モデル
                Data.SetParameter(symbol.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL), fig.product_code);
            }
        }

        private static string GetTypeName_Pile(ST_BRIDGE stb, int id)
        {
            string typename = stb.StbModel.StbMembers.StbPiles.Find(a => a.id_section == id)?.name;
            return typename;
        }


        /// <summary>
        /// 基礎インスタンスパラメータ設定（布基礎以外）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="footing"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateFoundation_instance(ST_BRIDGE stb, StbFooting footing, Family[][] ConvFamily)
        {
            bool ret = true;

            Family fami = null;

            var secf = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == footing.id_section);
            if (secf == null)
            {
                return ret;
            }

            //タイプ名
            string typename = GetTypeName_Footing(stb, footing.id_section);


            switch (secf.StbSecFigureFoundation_RC.FigureType)
            {
                case 1:
                    fami = ConvFamily[0][0];
                    break;
                case 2:
                    fami = ConvFamily[0][1];
                    break;
                case 3:
                    fami = ConvFamily[0][2];
                    break;
                case 4:
                    fami = ConvFamily[0][3];
                    break;
                case 5:
                    fami = ConvFamily[0][4];
                    break;
            }

            if (fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (footingType[0].ContainsKey(footing.id_section))
            {
                symbol = Commons.doc.GetElement(footingType[0][footing.id_section]) as FamilySymbol;
            }
            else
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[基礎]" + typename + "(配置Id=" + footing.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得
            double depth = Get_Foundation_depth(stb, footing.id_section);
            XYZ P = Get_Node_Position(stb.StbModel.StbNodes, footing.id_node, footing.offset_X, footing.offset_Y, footing.level_bottom);

            //所属層
            int ind = Get_stbFloor_index(stb, footing.id_node);
            Level btmlevel = SearchLevel(stb, ind);

            //インスタンスの生成
            try
            {
                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, btmlevel, StructuralType.Footing);

                //レベルからの高さオフセット
                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), P.Z - btmlevel.Elevation, false);

                //断面回転
                instance.Location.Rotate(Line.CreateBound(P, P + 5 * BasisZ.Normalize()), (footing.rotate * Math.PI) / 180);

                SetInstanceParameter_Footing(stb, footing, instance);

                if (!FGroup.ContainsKey(footing.id_node))
                {
                    FGroup.Add(footing.id_node, new List<ElementId>());
                }
                FGroup[footing.id_node].Add(instance.Id);

                LogData.AddLog( LogData.LogKind.Infmoation, 0, $"基礎の生成：\t[配置Id {footing.id_node}]{symbol.Name} 要素ID{instance.Id}" ) ;
                OutputDebubCommentLog( instance, footing.id_node, "基礎", symbol.Name, new int[]{} ) ;
                
                Data.SaveGuid(footing.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        private static void SetInstanceParameter_Footing(ST_BRIDGE stb, StbFooting footing, FamilyInstance instance)
        {
            var secf = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == footing.id_section);
            List<string> pnames = new List<string>();
            switch (secf.StbSecFigureFoundation_RC.FigureType)
            {
                case 1:
                    pnames.Add(SetFamily.FRect.MemId);
                    pnames.Add(SetFamily.FRect.NameMembers);
                    pnames.Add(SetFamily.FRect.thickness_ex_start_X);
                    pnames.Add(SetFamily.FRect.thickness_ex_end_X);
                    pnames.Add(SetFamily.FRect.thickness_ex_start_Y);
                    pnames.Add(SetFamily.FRect.thickness_ex_end_Y);
                    pnames.Add(SetFamily.FRect.thickness_ex_top);
                    pnames.Add(SetFamily.FRect.thickness_ex_bottom);
                    break;
                case 2:
                    pnames.Add(SetFamily.FTRect.MemId);
                    pnames.Add(SetFamily.FTRect.NameMembers);
                    pnames.Add(SetFamily.FTRect.thickness_ex_start_X);
                    pnames.Add(SetFamily.FTRect.thickness_ex_end_X);
                    pnames.Add(SetFamily.FTRect.thickness_ex_start_Y);
                    pnames.Add(SetFamily.FTRect.thickness_ex_end_Y);
                    pnames.Add(SetFamily.FTRect.thickness_ex_top);
                    pnames.Add(SetFamily.FTRect.thickness_ex_bottom);
                    break;
                case 3:
                    pnames.Add(SetFamily.FTri.MemId);
                    pnames.Add(SetFamily.FTri.NameMembers);
                    pnames.Add(SetFamily.FTri.thickness_ex_start_X);
                    pnames.Add(SetFamily.FTri.thickness_ex_end_X);
                    pnames.Add(SetFamily.FTri.thickness_ex_start_Y);
                    pnames.Add(SetFamily.FTri.thickness_ex_end_Y);
                    pnames.Add(SetFamily.FTri.thickness_ex_top);
                    pnames.Add(SetFamily.FTri.thickness_ex_bottom);
                    break;
                case 4:
                    pnames.Add(SetFamily.FETriangle.MemId);
                    pnames.Add(SetFamily.FETriangle.NameMembers);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_start_X);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_end_X);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_start_Y);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_end_Y);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_top);
                    pnames.Add(SetFamily.FETriangle.thickness_ex_bottom);
                    break;
                case 5:
                    pnames.Add(SetFamily.FOct.MemId);
                    pnames.Add(SetFamily.FOct.NameMembers);
                    pnames.Add(SetFamily.FOct.thickness_ex_start_X);
                    pnames.Add(SetFamily.FOct.thickness_ex_end_X);
                    pnames.Add(SetFamily.FOct.thickness_ex_start_Y);
                    pnames.Add(SetFamily.FOct.thickness_ex_end_Y);
                    pnames.Add(SetFamily.FOct.thickness_ex_top);
                    pnames.Add(SetFamily.FOct.thickness_ex_bottom);
                    break;
            }

            Data.SetParameter(instance.LookupParameter(pnames[0]), footing.id);
            Data.SetParameter(instance.LookupParameter(pnames[1]), footing.name);
            Data.SetParameter(instance.LookupParameter(pnames[2]), footing.thickness_add_start_X, true);
            Data.SetParameter(instance.LookupParameter(pnames[3]), footing.thickness_add_end_X, true);
            Data.SetParameter(instance.LookupParameter(pnames[4]), footing.thickness_add_start_Y, true);
            Data.SetParameter(instance.LookupParameter(pnames[5]), footing.thickness_add_end_Y, true);
            Data.SetParameter(instance.LookupParameter(pnames[6]), footing.thickness_add_top, true);
            Data.SetParameter(instance.LookupParameter(pnames[7]), footing.thickness_add_bottom, true);
        }

        /// <summary>
        /// 基礎インスタンスパラメータ設定（布基礎）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="footing"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreateStripFooting_instance(ST_BRIDGE stb, StbStripFooting footing, Family[][] ConvFamily)
        {
            bool ret = true;

            Family fami = null;

            var secf = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == footing.id_section);
            if (secf == null)
            {
                return ret;
            }

            //タイプ名
            string typename = secf.name;

            fami = ConvFamily[1][0];

            if (fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            double t_B = Get_Girder_B(stb, footing.id_node_start, footing.id_node_end);
            for (int i = 0; i < FContiSymbols.Count(); i++)
            {
                if (FContiSymbols[i].Length == t_B && FContiSymbols[i].id == footing.id_section)
                {
                    symbol = FContiSymbols[i].symbol;
                    break;
                }
            }
            if (symbol == null)
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[基礎]" + typename + "(配置Id=" + footing.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得
            XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, footing.id_node_start, 0, 0, 0);
            XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, footing.id_node_end, 0, 0, 0);

            //オフセット
            XYZ vec1 = (Pe - Ps).Normalize();
            XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();
            double offset = Commons.mm2ft(footing.offset);
            Ps = Ps + vec2 * offset;
            Pe = Pe + vec2 * offset;


            //インスタンスの生成
            try
            {
                //STBのレベルは節点から布基礎下端までの長さ。インスタンスに設定するのは配置面からのオフセット
                double f_level = Commons.mm2ft(footing.level);

                //配置面の取得
                FamilyInstance instance = null;
                FamilyInstance ins2 = null;
                int cgrp_ind = -1;
                for (int i = 0; i < CGrp.Count(); i++)
                {
                    if (CGrp[i].start_node == footing.id_node_start && CGrp[i].end_node == footing.id_node_end)
                    {
                        ins2 = CGrp[i].elem[0] as FamilyInstance;
                        cgrp_ind = i;
                    }
                }

                if (ins2 == null)
                {
                    //ログ表示(配置面が無い)
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[基礎]" + typename + "(配置Id=" + footing.id.ToString() + ")" + "は配置面が見つからないため配置できません。");
                    return ret;
                }

                //床勝ちだと上面が取れない。取得した基礎梁と床の勝ち負けだけここで入れ替えておく。
                ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins2);
                if (joined.Count > 0)
                {
                    bool check = false;
                    foreach (var id in joined)
                    {
                        var el = Commons.doc.GetElement(id);
                        if (el is Floor)
                        {
                            if (!JoinGeometryUtils.AreElementsJoined(Commons.doc, ins2, el)) { continue; }
                            if (JoinGeometryUtils.IsCuttingElementInJoin(Commons.doc, ins2, el)) { continue; }
                            try
                            {
                                JoinGeometryUtils.SwitchJoinOrder(Commons.doc, ins2, el);
                                check = true;
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (check) { Commons.doc.Regenerate(); }
                }

                Options opt = new Options
                {
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                };
                GeometryElement ge = ins2.get_Geometry(opt);
                IEnumerator<GeometryObject> genum = ge.GetEnumerator();
                genum.Reset();
                bool makeflg = false;
                //何故か一回でSolidが取れないことがある
                while (genum.MoveNext())
                {
                    Transform tf = null;
                    Solid sld = genum.Current as Solid;
                    if (sld == null)
                    {
                        GeometryInstance gins = genum.Current as GeometryInstance;
                        if (gins == null) { continue; }
                        //GeometryInstanceをもう一度取得して、そこからSolidを取る
                        ge = gins.SymbolGeometry;
                        IEnumerator<GeometryObject> genum2 = ge.GetEnumerator();
                        genum2.Reset();
                        while (genum2.MoveNext())
                        {
                            sld = genum2.Current as Solid;
                            if (sld == null) { continue; }
                            if (sld.Volume <= 0) { continue; }
                            if (sld.Faces.Size == 0) { continue; }
                            tf = gins.Transform;
                            break;
                        }
                    }
                    if (sld == null) { continue; }
                    if (sld.Volume <= 0) { continue; }
                    if (sld.Faces.Size == 0) { continue; }

                    for (int i = 0; i < sld.Faces.Size; i++)
                    {
                        PlanarFace pface = sld.Faces.get_Item(i) as PlanarFace;
                        if (pface == null) { continue; }

                        XYZ normal = pface.FaceNormal;

                        if (normal.DistanceTo(XYZ.BasisZ) < 0.001)
                        {
                            //上面

                            //Line座標は平面上にする必要がある。上向き法線をチェックしているので斜めはない。Z座標だけ入れ替える
                            double z = pface.Origin.Z;
                            if (tf != null)
                            {
                                z = tf.OfPoint(pface.Origin).Z;
                            }

                            XYZ ps2 = new XYZ(Ps.X, Ps.Y, z);
                            XYZ pe2 = new XYZ(Pe.X, Pe.Y, z);

                            //配置面からのオフセットに換算
                            f_level = Ps.Z + f_level - z;

                            instance = Commons.doc.Create.NewFamilyInstance(pface, Line.CreateBound(ps2, pe2), symbol);
                            makeflg = true;
                            break;
                        }
                    }
                    if (makeflg) { break; }
                }


                if (instance == null)
                {
                    //ログ表示(インスタンス生成に失敗)
                    LogData.AddLog(LogData.LogKind.Error, 0, "[基礎]" + typename + "(配置Id=" + footing.id.ToString() + ")");
                    return ret;
                }


                Data.SetParameter(instance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM), f_level, false); //レベルからの高さオフセット
                Data.SetParameter(instance.get_Parameter(BuiltInParameter.INSTANCE_MOVES_WITH_GRID_PARAM), false); //通心に沿ってい移動


                SetInstanceParameter_StripFooting(footing, instance);

                //変換情報ログの出力
                var nodeIds = new int[] { footing.id_node_start, footing.id_node_end } ;
                Data.MakeNodeLog( "布基礎の生成：", "[配置Id " + footing.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, footing.id, "布基礎", typename, nodeIds ) ;

                if (cgrp_ind != -1)
                {
                    CGrp[cgrp_ind].elId.Add(instance.Id);
                }

                Data.SaveGuid(footing.guid, instance.Id);

            }
            catch (Exception)
            {

                ret = false;
            }


            return ret;
        }

        private static void SetInstanceParameter_StripFooting(StbStripFooting footing, FamilyInstance instance)
        {
            FamilyStructure.Foundation_Continuous Rfo = SetFamily.FConti;
            Data.SetParameter(instance.LookupParameter(Rfo.MemId), footing.id);
            Data.SetParameter(instance.LookupParameter(Rfo.NameMembers), footing.name);
            Data.SetParameter(instance.LookupParameter(Rfo.length_ex_start), footing.length_ex_start);
            Data.SetParameter(instance.LookupParameter(Rfo.length_ex_end), footing.length_ex_end);
        }

        /// <summary>
        /// 基礎インスタンスパラメータ設定（杭）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pile"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private static bool CreatePile_instance(ST_BRIDGE stb, StbPile pile, Family[][] ConvFamily)
        {
            bool ret = true;

            GetPileData(stb, pile, ConvFamily, out Family fami, out string typename, out double length_all, out int index);

            if (index == -1)
            {
                return ret;
            }


            if (fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "杭");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            var syms = PilesSymbols.Where(a => a.BHaunch1 == pile.kind_structure.ToString() && a.id == pile.id_section);
            if (syms.Count() == 1)
            {
                symbol = syms.First().symbol;
            }
            else
            {
                foreach (var sym in syms)
                {
                    if (sym.id == pile.id_section &&
                        sym.Length == pile.length_all &&
                        sym.Length2 == pile.length_head &&
                        sym.Length3 == pile.length_foot)
                    {
                        symbol = sym.symbol;
                        break;
                    }
                }
            }

            if (symbol == null)
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[杭]" + typename + "(配置id=" + pile.id + ")");
                return ret;
            }


            //配置座標の取得
            XYZ P = Get_Node_Position(stb.StbModel.StbNodes, pile.id_node, pile.offset_X, pile.offset_Y, 0);

            //インスタンスの生成
            try
            {
                //所属層
                int ind = Get_stbFloor_index(stb, pile.id_node);
                Level btmlevel = SearchLevel(stb, ind);

                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, btmlevel, StructuralType.Footing);

                //レベルからの相対高さを設定する
                double level_top = P.Z + Commons.mm2ft(pile.level_top) - btmlevel.Elevation;

                Data.SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), level_top, false); //レベルからの高さオフセット

                SetInstanceParameter_Pile(pile, length_all, index, instance);

                if (!FGroup.ContainsKey(pile.id_node))
                {
                    FGroup.Add(pile.id_node, new List<ElementId>());
                }
                FGroup[pile.id_node].Add(instance.Id);
                
                LogData.AddLog( LogData.LogKind.Infmoation, 0, $"杭の生成：\t[配置Id {pile.id_node}]{symbol.Name} 要素ID{instance.Id}" ) ;
                OutputDebubCommentLog( instance, pile.id_node, "杭", symbol.Name, new int[]{} ) ;
                
                Data.SaveGuid(pile.guid, instance.Id);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        private static void GetPileData(ST_BRIDGE stb, StbPile pile, Family[][] ConvFamily, out Family fami, out string typename, out double length_all, out int index)
        {
            fami = null;
            typename = GetTypeName_Pile(stb, pile.id_section);
            index = -1;

            //杭全長
            //RC（場所打ち杭）の場合、記述は必須とする。
            //鋼管杭と既製コンクリート杭は継ぎ杭本数と継ぎ杭長さで定義する。
            length_all = 0;


            var secp_RC = stb.StbModel.StbSections.StbSecPile_RC.Find(a => a.id == pile.id_section);
            var secp_S = stb.StbModel.StbSections.StbSecPile_S.Find(a => a.id == pile.id_section);
            var secp_Pr = stb.StbModel.StbSections.StbSecPileProduct.Find(a => a.id == pile.id_section);

            if (secp_RC != null)
            {
                fami = ConvFamily[2][0];
                index = 0;
                length_all = pile.length_all;
            }
            else if (secp_S != null)
            {
                index = 2;
                fami = ConvFamily[2][2];

                if (secp_S.StbSecFigurePile_S.StbSecPile_S_Straight != null &&
                    secp_S.StbSecFigurePile_S.StbSecPile_S_Straight.Count > 0)
                {
                    length_all = secp_S.StbSecFigurePile_S.StbSecPile_S_Straight[0].length_pile * secp_S.StbSecFigurePile_S.StbSecPile_S_Straight.Count;
                }
                else if (secp_S.StbSecFigurePile_S.StbSecPile_S_Rotational != null &&
                         secp_S.StbSecFigurePile_S.StbSecPile_S_Rotational.Count > 0)
                {
                    length_all = secp_S.StbSecFigurePile_S.StbSecPile_S_Rotational[0].length_pile * secp_S.StbSecFigurePile_S.StbSecPile_S_Rotational.Count;
                }
                else if (secp_S.StbSecFigurePile_S.StbSecPile_S_Taper != null &&
                         secp_S.StbSecFigurePile_S.StbSecPile_S_Taper.Count > 0)
                {
                    length_all = secp_S.StbSecFigurePile_S.StbSecPile_S_Taper[0].length_pile * secp_S.StbSecFigurePile_S.StbSecPile_S_Taper.Count;
                }
            }
            else if (secp_Pr != null)
            {
                if (secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PHC != null &&
                    secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PHC.Count > 0)
                {
                    fami = ConvFamily[2][3];
                    index = 3;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PHC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PHC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_ST != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_ST.Count > 0)
                {
                    fami = ConvFamily[2][4];
                    index = 4;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_ST[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_ST.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_SC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_SC.Count > 0)
                {
                    fami = ConvFamily[2][5];
                    index = 5;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_SC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_SC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PRC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PRC.Count > 0)
                {
                    fami = ConvFamily[2][6];
                    index = 6;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PRC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_PRC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_CPRC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_CPRC.Count > 0)
                {
                    fami = ConvFamily[2][7];
                    index = 7;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_CPRC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProduct_CPRC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PHC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PHC.Count > 0)
                {
                    //節なしで変換する
                    fami = ConvFamily[2][3];
                    index = 3;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PHC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PHC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PRC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PRC.Count > 0)
                {
                    //節なしで変換する
                    fami = ConvFamily[2][6];
                    index = 6;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PRC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_PRC.Count;
                }
                else if (secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC != null &&
                         secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC.Count > 0)
                {
                    //節なしで変換する
                    fami = ConvFamily[2][7];
                    index = 7;
                    length_all = secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC[0].length_pile * secp_Pr.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC.Count;
                }
            }
        }

        private static void SetInstanceParameter_Pile(StbPile pile, double length_all, int index, FamilyInstance instance)
        {
            if (index == 0)
            {
                var Rpile = SetFamily.CastinPile;
                Data.SetParameter(instance.LookupParameter(Rpile.MemId), pile.id);
                Data.SetParameter(instance.LookupParameter(Rpile.NameMembers), pile.name);
            }
            else
            {
                List<string> pnames = new List<string>();
                if (index == 2)
                {
                    var Rpile = SetFamily.Pile_S;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else if (index == 3)
                {
                    var Rpile = SetFamily.Pile_PHC;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else if (index == 4)
                {
                    var Rpile = SetFamily.Pile_ST;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else if (index == 5)
                {
                    var Rpile = SetFamily.Pile_SC;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else if (index == 6)
                {
                    var Rpile = SetFamily.Pile_PRC;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else if (index == 7)
                {
                    var Rpile = SetFamily.Pile_CPRC;
                    pnames.Add(Rpile.MemId);
                    pnames.Add(Rpile.NameMembers);
                    pnames.Add(Rpile.length_all);
                }
                else
                {
                    return;
                }

                Data.SetParameter(instance.LookupParameter(pnames[0]), pile.id);
                Data.SetParameter(instance.LookupParameter(pnames[1]), pile.name);
                Data.SetParameter(instance.LookupParameter(pnames[2]), length_all, true);
            }
        }


        #endregion



        /// <summary>
        /// 柱脚の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="syubetu"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private static bool CreateClmBase(ST_BRIDGE stb, string syubetu, ref string errmsg)
        {
            bool ret = true;

            //プロジェクトにロードさているファミリをもう一度確認する
            LoadFamily lofa = new LoadFamily();
            lofa.LoadFfamily_fromProject();

            //柱脚ファミリの取得
            GetBaseFamily(out Dictionary<string, Family> BClmFamily, out Dictionary<string, string> mappingTypeName);


            //S柱
            if (stb.StbModel.StbSections.StbSecColumn_S != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumn_S.Count;

                for (int i = 0; i < numCount; i++)
                {
                    var clm = stb.StbModel.StbSections.StbSecColumn_S[i];

                    if (clm.Item != null && clm.Item is StbSecBaseProduct_S baseProduct)
                    {
                        Data.BaseClass newb = new Data.BaseClass
                        {
                            id_section = clm.id,
                            clmname = clm.name,
                            clm_structure = "S",
                            product_company = baseProduct.product_company,
                            product_code = baseProduct.product_code
                        };
                        BClm.Add(newb);
                    }
                }
            }

            //SRC柱
            if (stb.StbModel.StbSections.StbSecColumn_SRC != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumn_SRC.Count;

                for (int i = 0; i < numCount; i++)
                {
                    var clm = stb.StbModel.StbSections.StbSecColumn_SRC[i];
                    if (clm.Item != null && clm.Item is StbSecBaseProduct_SRC baseProduct)
                    {
                        Data.BaseClass newb = new Data.BaseClass
                        {
                            id_section = clm.id,
                            clmname = clm.name,
                            clm_structure = "SRC",
                            product_company = baseProduct.product_company,
                            product_code = baseProduct.product_code
                        };
                        BClm.Add(newb);
                    }
                }
            }

            //CFT柱
            if (stb.StbModel.StbSections.StbSecColumn_CFT != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumn_CFT.Count;

                for (int i = 0; i < numCount; i++)
                {
                    var clm = stb.StbModel.StbSections.StbSecColumn_CFT[i];
                    if (clm.Item != null && clm.Item is StbSecBaseProduct_CFT baseProduct)
                    {
                        Data.BaseClass newb = new Data.BaseClass
                        {
                            id_section = clm.id,
                            clmname = clm.name,
                            clm_structure = "CFT",
                            product_company = baseProduct.product_company,
                            product_code = baseProduct.product_code
                        };
                        BClm.Add(newb);
                    }
                }
            }



            Transaction tran = new Transaction(Commons.doc, "柱脚タイプ");
            try
            {
                tran.Start();

                Data.ProgressStart("柱脚の生成", BClm.Count);

                for (int i = 0; i < BClm.Count; i++)
                {
                    Data.ProgressPerformStep();

                    if (!mappingTypeName.ContainsKey(BClm[i].product_code)) continue;
                    if (!BClmFamily.ContainsKey(BClm[i].product_code)) continue;

                    string typename = mappingTypeName[BClm[i].product_code]; //マッピングテーブルで指定されたタイプ名
                    Family ConvFamily = BClmFamily[BClm[i].product_code];    //マッピングテーブルで指定されたファミリ名

                    if (ConvFamily == null)
                    {
                        //ログ表示（ファミリ未ロード)
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "柱脚[" + BClm[i].product_code + "]");
                        tran.RollBack();
                        return ret;
                    }

                    FamilySymbol symbol = null;

                    if (typename == "")
                    { typename = ConvFamily.Name; }

                    if (!Data.SearchFamilySymbol(ConvFamily, typename, ref symbol))
                    {
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }


                    Data.SetParameter(symbol.LookupParameter("符号"), typename);

                    //インスタンス
                    if (stb.StbModel.StbMembers.StbColumns != null)
                    {
                        var columns = stb.StbModel.StbMembers.StbColumns.Where(a => a.id_section == BClm[i].id_section).ToList();
                        for (int j = 0; j < columns.Count; j++)
                        {
                            var clm = columns[j];
                            if (clm.id_section == BClm[i].id_section)
                            {
                                CreateClmBase(stb, typename, symbol, clm);
                            }
                        }
                    }

                    if (stb.StbModel.StbMembers.StbPosts != null)
                    {
                        var columns = stb.StbModel.StbMembers.StbPosts.Where(a => a.id_section == BClm[i].id_section).ToList();
                        for (int j = 0; j < columns.Count; j++)
                        {
                            var clm = columns[j];
                            if (clm.id_section == BClm[i].id_section)
                            {
                                CreateClmBase(stb, typename, symbol, clm);
                            }
                        }
                    }

                }

                Data.ProgressClose();
                tran.Commit();
            }
            catch (Exception)
            {
                ret = false;
                errmsg = "柱脚";
                tran.RollBack();
            }


            Data.ProgressClose();

            return ret;
        }

        private static void GetBaseFamily(out Dictionary<string, Family> BClmFamily, out Dictionary<string, string> mappingTypeName)
        {
            BClmFamily = new Dictionary<string, Family>();
            mappingTypeName = new Dictionary<string, string>();
            for (int i = 0; i < RevitLNK.BClm.Count; i++)
            {
                if (!RevitLNK.BClm[i].flg) { continue; }

                for (int j = 0; j < LoadFamily.ProFami.Count; j++)
                {
                    if (LoadFamily.ProFami[j] == null) { continue; }
                    string rfaname = System.IO.Path.GetFileNameWithoutExtension(RevitLNK.BClm[i].rfa_pass);
                    if (rfaname == LoadFamily.ProFami[j].Name)
                    {
                        BClmFamily.Add(RevitLNK.BClm[i].product_code, LoadFamily.ProFami[j]);
                        mappingTypeName.Add(RevitLNK.BClm[i].product_code, RevitLNK.BClm[i].typename);
                    }
                }
            }
        }

        private static void CreateClmBase(ST_BRIDGE stb, string typename, FamilySymbol symbol, StbColumn clm)
        {
            XYZ P = Get_Node_Position(stb.StbModel.StbNodes, clm.id_node_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, clm.offset_bottom_Z);

            FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, StructuralType.NonStructural);

            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, clm.id_node_bottom);
            Level btmLevel = null;
            int index = indb;
            do
            {
                btmLevel = SearchLevel(stb, index);
                index--;
                if (index < 0) { break; }
            } while (btmLevel == null);
            if (btmLevel == null)
            {
                index = indb;
                do
                {
                    btmLevel = SearchLevel(stb, index);
                    index++;
                    if (index == stb.StbModel.StbStories.Count()) { break; }
                } while (btmLevel == null);
            }

            //ホスト指定していないのでZ座標をそのまま使用
            double offset_b = Commons.ft2mm(P.Z);

            double gir_offset_Z_bottom = 0;

            //柱脚Z方向オフセット値が0以外の時はその値を優先する
            if (clm.offset_bottom_Z == 0)
            {
                Search_Girder_Offset_Z_bottom(stb, clm.id_node_bottom, btmLevel, clm.kind_structure, out gir_offset_Z_bottom);
            }

            Data.SetParameter(instance, BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM, offset_b + gir_offset_Z_bottom, true);

            //回転
            instance.Location.Rotate(Line.CreateBound(P, P + XYZ.BasisZ), (clm.rotate * Math.PI) / 180);

            //変換情報ログの出力
            var nodeIds = new int[] { clm.id_node_bottom } ;
            Data.MakeNodeLog("柱脚の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id);
            OutputDebubCommentLog( instance, clm.id, "柱脚", typename, nodeIds ) ;

            Data.SaveGuid(clm.guid, instance.Id);
        }


        #endregion


        /// <summary
        /// >基礎梁・布基礎グループ化
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="start_node"></param>
        /// <param name="end_node"></param>
        /// <param name="el"></param>
        /// <param name="elem"></param>
        private static void CGrp_Add(ST_BRIDGE stb, int start_node, int end_node, ElementId el, Element elem = null)
        {
            bool addflg = true;
            for (int i = 0; i < CGrp.Count; i++)
            {
                if (Node_Check(stb.StbModel.StbNodes, CGrp[i].start_node, start_node) &&
                    Node_Check(stb.StbModel.StbNodes, CGrp[i].end_node, end_node))
                {
                    bool sameflg = false;
                    for (int j = 0; j < CGrp[i].elId.Count; j++)
                    {
                        if (CGrp[i].elId[j] == el)
                        {
                            sameflg = true;
                            addflg = false;
                            break;
                        }
                    }
                    if (sameflg)
                    {
                        CGrp[i].elId.Add(el);
                        if (elem != null)
                        { CGrp[i].elem.Add(elem); }
                        addflg = false;
                    }
                }
            }

            if (addflg)
            {
                Data.CGroup cgp = new Data.CGroup
                {
                    start_node = start_node,
                    end_node = end_node
                };
                cgp.elId.Add(el);
                if (elem != null)
                {
                    cgp.elem.Add(elem);
                }
                CGrp.Add(cgp);
            }
        }



        /// <summary>
        /// 勝ち負け判定
        /// </summary>
        /// <returns></returns>
        internal static int ChangeOrder()
        {
            int ret = 0;

            Transaction tran = new Transaction(Commons.doc, "結合順序切り替え");

            try
            {
                tran.Start();

                Dictionary<int, List<ElementId>> JoinedElement = new Dictionary<int, List<ElementId>>
                {
                    { (int)Data.Joinorder.pile, new List<ElementId>() },
                    { (int)Data.Joinorder.foundation, new List<ElementId>() },
                    { (int)Data.Joinorder.column, new List<ElementId>() },
                    { (int)Data.Joinorder.girder, new List<ElementId>() },
                    { (int)Data.Joinorder.beam, new List<ElementId>() },
                    { (int)Data.Joinorder.wall, new List<ElementId>() },
                    { (int)Data.Joinorder.brace, new List<ElementId>() },
                    { (int)Data.Joinorder.slab, new List<ElementId>() }
                };


                //基礎
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                IList<Element> elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    if (!(elems[i] is FamilyInstance ins)) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        if (elems[i].Name.Contains("pile") || elems[i].Name.Contains("Pile"))
                        {
                            JoinedElement[(int)Data.Joinorder.pile].Add(elems[i].Id);
                        }
                        else
                        {
                            JoinedElement[(int)Data.Joinorder.foundation].Add(elems[i].Id);
                        }
                    }
                }

                //柱
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    if (!(elems[i] is FamilyInstance ins)) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Data.Joinorder.column].Add(elems[i].Id);
                    }
                }

                //大梁小梁ブレース
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    if (!(elems[i] is FamilyInstance ins)) { continue; }

                    //SRCの始端の接合部カットバック・終端の接合部カットバックを0に設定
                    if (ins.Symbol.FamilyName == "SRC_Girder_icj")
                    {
                        Data.SetParameter(ins, BuiltInParameter.START_JOIN_CUTBACK, 1.0, true);
                        Data.SetParameter(ins, BuiltInParameter.END_JOIN_CUTBACK, 1.0, true);
                    }
                    else
                    {
                        Data.SetParameter(ins, BuiltInParameter.START_JOIN_CUTBACK, 0.0, true);
                        Data.SetParameter(ins, BuiltInParameter.END_JOIN_CUTBACK, 0.0, true);
                    }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        StructuralInstanceUsage usage = (StructuralInstanceUsage)(ins.get_Parameter(BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM).AsInteger());
                        switch (usage)
                        {
                            case StructuralInstanceUsage.Girder:
                                JoinedElement[(int)Data.Joinorder.girder].Add(elems[i].Id);
                                break;
                            case StructuralInstanceUsage.Joist:
                                JoinedElement[(int)Data.Joinorder.beam].Add(elems[i].Id);
                                break;
                            case StructuralInstanceUsage.Brace:
                            case StructuralInstanceUsage.HorizontalBracing:
                            case StructuralInstanceUsage.KickerBracing:
                            case StructuralInstanceUsage.Other:
                                JoinedElement[(int)Data.Joinorder.brace].Add(elems[i].Id);
                                break;
                        }
                    }
                }


                //壁
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    if (!(elems[i] is Wall ins)) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Data.Joinorder.wall].Add(elems[i].Id);
                    }
                }

                //床
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    if (!(elems[i] is Floor ins)) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Data.Joinorder.slab].Add(elems[i].Id);
                    }
                }


                Data.ProgressStart("結合順序切り替え", JoinedElement.Count() - 1);

                //切り替え開始
                for (int j1 = 0; j1 < JoinedElement.Count() - 1; j1++)
                {
                    Data.ProgressPerformStep();

                    for (int e1 = 0; e1 < JoinedElement[j1].Count(); e1++)
                    {
                        ElementId eid1 = JoinedElement[j1][e1];
                        Element elm1 = Commons.doc.GetElement(eid1);
                        ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, elm1);

                        foreach (var eid2 in joined)
                        {
                            Element elm2 = Commons.doc.GetElement(eid2);
                            if (elm1.Id == elm2.Id) { continue; }
                            bool check = false;
                            for (int j2 = 0; j2 <= j1; j2++)
                            {
                                if (JoinedElement[j2].Contains(eid2))
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check) { continue; }

                            if (!JoinGeometryUtils.AreElementsJoined(Commons.doc, elm1, elm2)) { continue; }

                            //1st>2nd：正しく設定されているので切り替え不要
                            if (JoinGeometryUtils.IsCuttingElementInJoin(Commons.doc, elm1, elm2)) { continue; }

                            //切り替え実行
                            JoinGeometryUtils.SwitchJoinOrder(Commons.doc, elm1, elm2);
                        }
                    }
                }

                Data.ProgressClose();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.RollBack();
            }

            Data.ProgressClose();

            return ret;
        }





        /// <summary>
        /// 節点群から1層に載っているものを選択し、X（またはY）の小さい順に並べ替える
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="nodeL">対象Node</param>
        /// <param name="vec">方向</param>
        /// <returns></returns>
        private static List<StbNodeId> Narabekae_Node(ST_BRIDGE stb, List<StbNodeId> nodeL, XYZ vec)
        {
            //条件に合致する節点リスト
            List<StbNodeId> newL = new List<StbNodeId>();

            for (int s = 0; s < stb.StbModel.StbStories.Count(); s++)
            {
                if (newL.Count() > 0) break;

                var nodeS = stb.StbModel.StbStories[s].StbNodeIdList.Select(a => a.id).ToList();
                var nodeL2 = nodeL.Where(a => nodeS.Contains(a.id)).ToList();

                for (int i = 0; i < nodeL2.Count(); i++)
                {
                    XYZ p0 = Get_Node_Position(stb.StbModel.StbNodes, nodeL2[i].id);

                    bool add = true;
                    for (int j = 0; j < newL.Count; ++j)
                    {
                        XYZ p1 = Get_Node_Position(stb.StbModel.StbNodes, newL[j].id);
                        XYZ v0 = (p0 - p1).Normalize();
                        if (v0.DotProduct(vec) < 0)
                        {
                            add = false;
                            newL.Insert(j, nodeL2[i]);
                            break;
                        }
                    }
                    if (add)
                    {
                        newL.Add(nodeL2[i]);
                    }
                }
            }

            return newL;
        }

        /// <summary>
        /// STB内での座標を取得 [mm]
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <returns>[mm]</returns>
        private static XYZ Get_Node_Position(List<StbNode> StbNodes, int id)
        {
            XYZ position = new XYZ();
            var node = StbNodes.Find(a => a.id == id);
            if (node != null)
            {
                position = new XYZ(node.X, node.Y, node.Z);
            }

            return position;
        }

        /// <summary>
        /// Revit内での座標を取得(XYのみ)※軸の作成に使用 [ft]
        /// </summary>
        /// <param name="StbNodes"></param>
        /// <param name="id"></param>
        /// <param name="alloffsetx">基点位置offset_X</param>
        /// <param name="alloffsety">基点位置offset_Y</param>
        /// <returns>[ft]</returns>
        private static XYZ Get_Node_Position(List<StbNode> StbNodes, int id, double alloffsetx, double alloffsety)
        {
            XYZ position = new XYZ();
            var node = StbNodes.Find(a => a.id == id);
            if (node != null)
            {
                position = Commons.mm2ft(new XYZ(node.X + alloffsetx, node.Y + alloffsety, 0));
            }

            return position;
        }

        /// <summary>
        /// Revit内での座標を取得(XYZ) [ft]
        /// </summary>
        /// <param name="StbNodes"></param>
        /// <param name="id"></param>
        /// <param name="offsetx">部材のX方向offset（全体座標系）</param>
        /// <param name="offsety">部材のY方向offset（全体座標系）</param>
        /// <param name="offsetz">部材のZ方向offset（全体座標系）</param>
        /// <returns>[ft]</returns>
        private static XYZ Get_Node_Position(List<StbNode> StbNodes, int id, double offsetx, double offsety, double offsetz)
        {
            XYZ position = new XYZ();
            var node = StbNodes.Find(a => a.id == id);
            if (node != null)
            {
                position = Commons.mm2ft(new XYZ(node.X + offsetx + alloffsetX, node.Y + offsety + alloffsetY, node.Z + offsetz));
            }

            return position;
        }



        /// <summary>
        /// STBの層のindexを層名から取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        private static int Get_stbFloor_index(List<StbStory> StbStories, string floor)
        {
            int ret = -1;
            if (floor == "") return ret;

            ret = StbStories.FindIndex(a => a.name == floor);

            return ret;
        }

        private static int Get_stbFloor_index_Clm(ST_BRIDGE stb, int sec_id)
        {
            int ret = -1;

            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                foreach (var clm in stb.StbModel.StbMembers.StbColumns)
                {
                    if (clm.id_section == sec_id)
                    {
                        ret = Get_stbFloor_index(stb, clm.id_node_bottom);
                        break;
                    }
                }
            }
            if (ret == -1)
            {
                if (stb.StbModel.StbMembers.StbPosts != null)
                {
                    foreach (var clm in stb.StbModel.StbMembers.StbPosts)
                    {
                        if (clm.id_section == sec_id)
                        {
                            ret = Get_stbFloor_index(stb, clm.id_node_bottom);
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        private static int Get_stbFloor_index_Gir(ST_BRIDGE stb, int id_section)
        {
            int ret = -1;

            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                foreach (var gir in stb.StbModel.StbMembers.StbGirders)
                {
                    if (gir.id_section == id_section)
                    {
                        ret = Get_stbFloor_index(stb, gir.id_node_start);
                        break;
                    }
                }
            }

            if (ret == -1)
            {
                if (stb.StbModel.StbMembers.StbBeams != null)
                {
                    foreach (var gir in stb.StbModel.StbMembers.StbBeams)
                    {
                        if (gir.id_section == id_section)
                        {
                            ret = Get_stbFloor_index(stb, gir.id_node_start);
                            break;
                        }
                    }
                }
            }

            return ret;
        }


        /// <summary>
        /// STBの層のindexを節点idから取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id">節点id</param>
        /// <param name="flg">F:見つからない場合は-1を返す</param>
        /// <returns></returns>
        private static int Get_stbFloor_index(ST_BRIDGE stb, int id, bool flg = true)
        {
            int ret = 0;
            if (!flg) { ret = -1; }

            for (int i = 0; i < stb.StbModel.StbStories.Count(); i++)
            {
                for (int j = 0; j < stb.StbModel.StbStories[i].StbNodeIdList.Count; j++)
                {
                    if (id == stb.StbModel.StbStories[i].StbNodeIdList[j].id)
                    {
                        ret = i;
                        return ret;
                    }
                }
            }
            return ret;
        }


        /// <summary>
        /// STBの所属層からRevitでの所属層を取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="ind">StbStoriesのIndex</param>
        /// <returns></returns>
        private static Level SearchLevel(ST_BRIDGE stb, int ind)
        {
            Level lv = null;
            double offset = 0;

            for (int i = 0; i < alloffsetZ.Count(); i++)
            {
                if (alloffsetZ[i].stbid == ind)
                {
                    offset = alloffsetZ[i].offset;
                    break;
                }
            }
            if (ind == -1) { return lv; }

            double interval = 0;
            for (int i = 0; i < Levels.Count(); i++)
            {
                double mm2ft = Commons.mm2ft(stb.StbModel.StbStories[ind].height + offset);
                if (Math.Abs(mm2ft - Levels[i].Elevation) < 1)
                {
                    lv = Levels[i];
                    break;
                }
                else
                {
                    double sa = Math.Abs(mm2ft - Levels[i].Elevation);
                    if (i == 0)
                    {
                        interval = sa;
                        lv = Levels[i];
                    }
                    else
                    {
                        if (interval > sa)
                        {
                            interval = sa;
                            lv = Levels[i];
                        }
                    }

                }

            }

            return lv;
        }

        private static Level SearchLevel_height(ST_BRIDGE stb, int id_start, int id_end)
        {
            Level lv = null;

            //中点を求める
            XYZ ps = Get_Node_Position(stb.StbModel.StbNodes, id_start);
            XYZ pe = Get_Node_Position(stb.StbModel.StbNodes, id_end);

            //中点座標（フィート）
            double zc = Commons.mm2ft((ps.Z + pe.Z) / 2);

            for (int i = 0; i < Levels.Count() - 1; i++) //Levelsは高い方から順に入っている
            {
                if (Levels[i].Elevation > zc && Levels[i + 1].Elevation <= zc)
                {
                    lv = Levels[i + 1];
                    break;
                }
            }
            if (lv == null)
            {
                lv = Levels[0];
            }
            return lv;
        }


        /// <summary>
        /// 梁の幅
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="s_id">id_node_start</param>
        /// <param name="e_id">id_node_end</param>
        /// <returns></returns>
        private static double Get_Girder_B(ST_BRIDGE stb, int s_id, int e_id)
        {
            double ret = 0;

            int id_section = -1;
            int id_member = -1;
            StbGirderKind_structure kind_structure = StbGirderKind_structure.RC;

            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                var gir = stb.StbModel.StbMembers.StbGirders.Find(a => a.id_node_start == s_id && a.id_node_end == e_id);
                if (gir != null)
                {
                    id_section = gir.id_section;
                    kind_structure = gir.kind_structure;
                    id_member = gir.id;
                }
            }

            if (stb.StbModel.StbMembers.StbBeams != null && id_section == -1)
            {
                var gir = stb.StbModel.StbMembers.StbBeams.Find(a => a.id_node_start == s_id && a.id_node_end == e_id);
                if (gir != null)
                {
                    id_section = gir.id_section;
                    kind_structure = gir.kind_structure;
                    id_member = gir.id;
                }
            }


            switch (kind_structure)
            {
                case StbGirderKind_structure.RC:
                    var secRC = stb.StbModel.StbSections.StbSecBeam_RC.Find(a => a.id == id_section);
                    if (secRC != null)
                    {
                        switch (secRC.StbSecFigureBeam_RC.FigureType)
                        {
                            case 1:
                                var rc_s = secRC.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Straight>().FirstOrDefault();
                                ret = rc_s.width;
                                break;
                            case 2:
                                var rc_t = secRC.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                ret = rc_t.width;
                                break;
                            case 3:
                                var rc_h = secRC.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                ret = rc_h.width;
                                break;
                        }
                    }
                    break;

                case StbGirderKind_structure.S:
                    var gir_s = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == id_section);
                    if (gir_s != null)
                    {
                        string shape = "";
                        switch (gir_s.StbSecSteelFigureBeam_S.FigureType)
                        {
                            case 1:
                                shape = ((StbSecSteelBeam_S_Straight)gir_s.StbSecSteelFigureBeam_S.Items[0]).shape;
                                break;
                            case 2:
                                shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Taper>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_TaperPos.START).shape;
                                break;
                            case 3:
                                shape = ((StbSecSteelBeam_S_Joint)gir_s.StbSecSteelFigureBeam_S.Items[0]).shape;
                                break;
                            case 4:
                                shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                                break;
                            case 5:
                                shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER).shape;
                                break;
                        }

                        string shape2 = Check_Steel(stb, shape, out int sind);
                        switch (shape2)
                        {
                            case RevitLNK.st_steel_H:
                                ret = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[sind].B;
                                break;
                            case RevitLNK.st_steel_BH:
                                ret = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[sind].B;
                                break;
                            case RevitLNK.st_steel_C:
                                ret = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[sind].B;
                                break;
                            case RevitLNK.st_steel_L:
                                ret = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[sind].B;
                                break;
                            case RevitLNK.st_steel_LipC:
                                ret = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[sind].A;
                                break;
                        }
                    }
                    break;

                case StbGirderKind_structure.SRC:
                    var secSRC = stb.StbModel.StbSections.StbSecBeam_SRC.Find(a => a.id == id_section);
                    if (secSRC != null)
                    {
                        switch (secSRC.StbSecFigureBeam_SRC.FigureType)
                        {
                            case 1:
                                var rc_s = secSRC.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Straight>().FirstOrDefault();
                                ret = rc_s.width;
                                break;
                            case 2:
                                var rc_t = secSRC.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                ret = rc_t.width;
                                break;
                            case 3:
                                var rc_h = secSRC.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                ret = rc_h.width;
                                break;
                        }
                    }
                    break;
            }

            return ret;
        }


        /// <summary>
        /// 梁の成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="kind"></param>
        /// <param name="start_end">"start", "end", "center"</param>
        /// <returns></returns>
        private static double Get_Girder_depth(ST_BRIDGE stb, int id, StbGirderKind_structure kind, string start_end)
        {
            double depth = 0;
            switch (kind)
            {
                case StbGirderKind_structure.RC:
                    var gir_rc = stb.StbModel.StbSections.StbSecBeam_RC.Find(a => a.id == id);
                    if (gir_rc != null)
                    {
                        switch (gir_rc.StbSecFigureBeam_RC.FigureType)
                        {
                            case 1:
                                depth = ((StbSecBeam_RC_Straight)gir_rc.StbSecFigureBeam_RC.Items[0]).depth;
                                break;

                            case 2:
                                var ts = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                var te = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                                if (start_end == "start")
                                {
                                    depth = ts.depth;
                                }
                                else if (start_end == "center")
                                {
                                    depth = (ts.depth + te.depth) / 2;
                                }
                                else if (start_end == "end")
                                {
                                    depth = te.depth;
                                }
                                break;

                            case 3:
                                if (start_end == "start")
                                {
                                    var hs = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                                    if (hs == null)
                                    {
                                        hs = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                    }

                                    depth = hs.depth;
                                }
                                else if (start_end == "center")
                                {
                                    var hc = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                    depth = hc.depth;
                                }
                                else if (start_end == "end")
                                {
                                    var he = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                                    if (he == null)
                                    {
                                        he = gir_rc.StbSecFigureBeam_RC.Items.OfType<StbSecBeam_RC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                    }

                                    depth = he.depth;
                                }
                                break;
                        }
                    }
                    break;

                case StbGirderKind_structure.S:
                    var gir_s = stb.StbModel.StbSections.StbSecBeam_S.Find(a => a.id == id);
                    if (gir_s != null)
                    {
                        string shape = "";
                        switch (gir_s.StbSecSteelFigureBeam_S.FigureType)
                        {
                            case 1:
                                shape = ((StbSecSteelBeam_S_Straight)gir_s.StbSecSteelFigureBeam_S.Items[0]).shape;
                                break;
                            case 2:
                                if (start_end == "start")
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Taper>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_TaperPos.START).shape;
                                }
                                else
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Taper>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_TaperPos.END).shape;
                                }
                                break;
                            case 3:
                                shape = ((StbSecSteelBeam_S_Joint)gir_s.StbSecSteelFigureBeam_S.Items[0]).shape;
                                break;
                            case 4:
                                if (start_end == "start")
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_HaunchPos.START)?.shape;
                                    if (shape == null)
                                    {
                                        shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                                    }
                                }
                                else
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_HaunchPos.END)?.shape;
                                    if (shape == null)
                                    {
                                        shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_Haunch>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                                    }
                                }
                                break;
                            case 5:
                                if (start_end == "start")
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.START)?.shape;
                                    if (shape == null)
                                    {
                                        shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER).shape;
                                    }
                                }
                                else
                                {
                                    shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.END)?.shape;
                                    if (shape == null)
                                    {
                                        shape = gir_s.StbSecSteelFigureBeam_S.Items.OfType<StbSecSteelBeam_S_FiveTypes>().FirstOrDefault(a => a.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER).shape;
                                    }
                                }
                                break;
                        }

                        string shape2 = Check_Steel(stb, shape, out int sind);
                        switch (shape2)
                        {
                            case RevitLNK.st_steel_H:
                                depth = stb.StbModel.StbSections.StbSecSteel.StbSecRollH[sind].A;
                                break;
                            case RevitLNK.st_steel_BH:
                                depth = stb.StbModel.StbSections.StbSecSteel.StbSecBuildH[sind].A;
                                break;
                            case RevitLNK.st_steel_C:
                                depth = stb.StbModel.StbSections.StbSecSteel.StbSecRollC[sind].A;
                                break;
                            case RevitLNK.st_steel_L:
                                depth = stb.StbModel.StbSections.StbSecSteel.StbSecRollL[sind].A;
                                break;
                            case RevitLNK.st_steel_LipC:
                                depth = stb.StbModel.StbSections.StbSecSteel.StbSecLipC[sind].H;
                                break;
                        }
                    }
                    break;

                case StbGirderKind_structure.SRC:
                    var gir_src = stb.StbModel.StbSections.StbSecBeam_SRC.Find(a => a.id == id);
                    if (gir_src != null)
                    {
                        switch (gir_src.StbSecFigureBeam_SRC.FigureType)
                        {
                            case 1:
                                depth = ((StbSecBeam_SRC_Straight)gir_src.StbSecFigureBeam_SRC.Items[0]).depth;
                                break;

                            case 2:
                                var ts = gir_src.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.START);
                                var te = gir_src.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Taper>().FirstOrDefault(a => a.pos == StbSecBeam_RC_TaperPos.END);
                                if (start_end == "start")
                                {
                                    depth = ts.depth;
                                }
                                else if (start_end == "center")
                                {
                                    depth = (ts.depth + te.depth) / 2;
                                }
                                else if (start_end == "end")
                                {
                                    depth = te.depth;
                                }
                                break;

                            case 3:
                                if (start_end == "start")
                                {
                                    var hs = gir_src.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.START);
                                    depth = hs.depth;
                                }
                                else if (start_end == "center")
                                {
                                    var hc = gir_src.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.CENTER);
                                    depth = hc.depth;
                                }
                                else if (start_end == "end")
                                {
                                    var he = gir_src.StbSecFigureBeam_SRC.Items.OfType<StbSecBeam_SRC_Haunch>().FirstOrDefault(a => a.pos == StbSecBeam_RC_HaunchPos.END);
                                    depth = he.depth;
                                }
                                break;
                        }
                    }
                    break;
            }
            return depth;
        }

        /// <summary>
        /// 基礎の高さを取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id">基礎断面id</param>
        /// <returns></returns>
        private static double Get_Foundation_depth(ST_BRIDGE stb, int id)
        {
            double depth = 0;

            if (stb.StbModel.StbSections.StbSecFoundation_RC != null)
            {
                var fo = stb.StbModel.StbSections.StbSecFoundation_RC.Find(a => a.id == id);
                if (fo != null)
                {
                    if (fo.StbSecFigureFoundation_RC != null)
                    {
                        switch (fo.StbSecFigureFoundation_RC.FigureType)
                        {
                            case 1:
                                depth = ((StbSecFoundation_RC_Rect)fo.StbSecFigureFoundation_RC.Item).depth;
                                break;
                            case 2:
                                depth = ((StbSecFoundation_RC_TaperedRect)fo.StbSecFigureFoundation_RC.Item).depth_base;
                                break;
                            case 3:
                                depth = ((StbSecFoundation_RC_Triangle)fo.StbSecFigureFoundation_RC.Item).depth;
                                break;
                            case 4:
                                depth = ((StbSecFoundation_RC_EquiTriangle)fo.StbSecFigureFoundation_RC.Item).depth;
                                break;
                            case 5:
                                depth = ((StbSecFoundation_RC_Octagon)fo.StbSecFigureFoundation_RC.Item).depth;
                                break;
                            case 6:
                                depth = ((StbSecFoundation_RC_Continuous)fo.StbSecFigureFoundation_RC.Item).depth_base;
                                break;
                        }

                    }
                }
            }

            return depth;
        }





        /// <summary>
        /// ハンチ長・ハンチ種類を配置から取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id_section"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="kind_haunch_start"></param>
        /// <param name="kind_haunch_end"></param>
        private static void Get_Haunch(ST_BRIDGE stb, int id_section,
                                       ref List<double> haunch_start, ref List<double> haunch_end,
                                       ref List<string> kind_haunch_start, ref List<string> kind_haunch_end)
        {
            //ハンチ種類は関数の呼び元で1.4共通クラスに格納するため、stringのままにする

            if (stb.StbModel.StbMembers.StbBeams != null)
            {
                var beams = stb.StbModel.StbMembers.StbBeams.Where(a => a.id_section == id_section).ToList();
                if (beams.Count > 0)
                {
                    foreach (var beam in beams)
                    {
                        bool sameflg = false;
                        for (int j = 0; j < haunch_start.Count(); j++)
                        {
                            if (haunch_start[j] == beam.haunch_start && haunch_end[j] == beam.haunch_end &&
                                kind_haunch_start[j] == beam.kind_haunch_start.ToString() && kind_haunch_end[j] == beam.kind_haunch_end.ToString())
                            {
                                sameflg = true;
                                break;
                            }
                        }
                        if (!sameflg)
                        {
                            haunch_start.Add(beam.haunch_start);
                            haunch_end.Add(beam.haunch_end);
                            kind_haunch_start.Add(beam.kind_haunch_start.ToString());
                            kind_haunch_end.Add(beam.kind_haunch_end.ToString());
                        }
                    }
                }
            }

            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                var beams = stb.StbModel.StbMembers.StbGirders.Where(a => a.id_section == id_section).ToList();
                if (beams.Count > 0)
                {
                    foreach (var beam in beams)
                    {
                        bool sameflg = false;
                        for (int j = 0; j < haunch_start.Count(); j++)
                        {
                            if (haunch_start[j] == beam.haunch_start && haunch_end[j] == beam.haunch_end &&
                                kind_haunch_start[j] == beam.kind_haunch_start.ToString() && kind_haunch_end[j] == beam.kind_haunch_end.ToString())
                            {
                                sameflg = true;
                                break;
                            }
                        }
                        if (!sameflg)
                        {
                            haunch_start.Add(beam.haunch_start);
                            haunch_end.Add(beam.haunch_end);
                            kind_haunch_start.Add(beam.kind_haunch_start.ToString());
                            kind_haunch_end.Add(beam.kind_haunch_end.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 継手距離を柱芯からの距離に換算
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="j_stb">STBに出力された継手距離</param>
        /// <param name="pos">節点座標</param>
        /// <param name="Ps">梁始点</param>
        /// <param name="Pe">梁終点</param>
        /// <param name="idnode">節点ID</param>
        /// <returns></returns>
        private static double Get_Joint(ST_BRIDGE stb, double j_stb, XYZ pos, XYZ Ps, XYZ Pe, int idnode)
        {
            if (j_stb == 0) { return 0; }

            //柱のオフセット
            XYZ clm_offset = new XYZ();
            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                var clm = stb.StbModel.StbMembers.StbColumns.Find(c => c.id_node_top == idnode);
                if (clm != null)
                {
                    clm_offset = new XYZ(Commons.mm2ft(clm.offset_top_X), Commons.mm2ft(clm.offset_top_Y), 0);
                }
                else
                {
                    clm = stb.StbModel.StbMembers.StbColumns.Find(c => c.id_node_bottom == idnode);
                    if (clm != null)
                    {
                        clm_offset = new XYZ(Commons.mm2ft(clm.offset_bottom_X), Commons.mm2ft(clm.offset_bottom_Y), 0);
                    }
                }
                if (clm == null)
                {
                    //小梁など柱に接続していない場合はそのままの値を設定
                    return Commons.mm2ft(j_stb);
                }
            }
            else
            {
                return Commons.mm2ft(j_stb);
            }

            //柱の生成座標（柱芯）
            XYZ pos_Column = pos + clm_offset;

            XYZ g_vec = (Pe - Ps).Normalize();
            double t = Commons.mm2ft(j_stb);

            //柱芯から梁方向に指定の継手距離移動した点の座標
            XYZ Q = pos_Column + g_vec * t;

            //点Qを梁芯上に移動させた点の座標
            XYZ a = Q - Ps;
            XYZ H = Ps + g_vec * a.DotProduct(g_vec);

            //点Hと梁始点との距離が設定する継手距離[ft]
            double joint = H.DistanceTo(Ps);
            return joint;
        }


        /// <summary>
        /// 同じ座標があるか（値が同じでもnodeIdが違う節点があった時用）
        /// </summary>
        /// <param name="StbNodes"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private static bool Node_Check(List<StbNode> StbNodes, int node1, int node2)
        {
            if (node1 == node2)
            {
                return true;
            }

            XYZ p1 = Get_Node_Position(StbNodes, node1);
            XYZ p2 = Get_Node_Position(StbNodes, node2);

            return p1.DistanceTo(p2) < 1;
        }


        /// <summary>
        /// 柱脚の伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="btmlevel"></param>
        /// <param name="kind"></param>
        /// <param name="offset_Z"></param>
        private static void Search_Girder_Offset_Z_bottom(ST_BRIDGE stb, int node, Level btmlevel, StbColumnKind_structure kind, out double offset_Z)
        {
            offset_Z = 0;

            bool getflg = false;

            //共有する節点があるとき
            //基礎
            if (stb.StbModel.StbMembers.StbFootings != null && (kind == StbColumnKind_structure.RC || kind == StbColumnKind_structure.SRC))
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbFootings.Count; i++)
                {
                    var footing = stb.StbModel.StbMembers.StbFootings[i];
                    if (Node_Check(stb.StbModel.StbNodes, node, footing.id_node))
                    {
                        getflg = true;
                        double level_offset = Commons.ft2mm(Get_Node_Position(stb.StbModel.StbNodes, footing.id_node, 0, 0, 0).Z);
                        double depth = Get_Foundation_depth(stb, footing.id_section);

                        //梁よりも基礎のオフセット値が優先度高い
                        offset_Z = footing.level_bottom + depth;

                        break;
                    }
                }
            }

            bool btmflg = false;
            int clmnum = 0;
            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbColumns.Count(); i++)
                {
                    if (clmnum > 1) { break; }

                    var clm = stb.StbModel.StbMembers.StbColumns[i];
                    if (Node_Check(stb.StbModel.StbNodes, node, clm.id_node_bottom) ||
                        Node_Check(stb.StbModel.StbNodes, node, clm.id_node_top))
                    {
                        clmnum++;
                    }
                }
            }

            if (stb.StbModel.StbMembers.StbPosts != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbPosts.Count(); i++)
                {
                    if (clmnum > 1) { break; }

                    var clm = stb.StbModel.StbMembers.StbPosts[i];
                    if (Node_Check(stb.StbModel.StbNodes, node, clm.id_node_bottom) ||
                        Node_Check(stb.StbModel.StbNodes, node, clm.id_node_top))
                    {
                        clmnum++;
                    }
                }
            }

            if (clmnum < 2 && kind != StbColumnKind_structure.S) { btmflg = true; }

            if (!getflg)
            {
                //節点を共有する大梁を探す            
                if (stb.StbModel.StbMembers.StbGirders != null)
                {
                    double depth = 0;
                    for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                    {
                        var gir = stb.StbModel.StbMembers.StbGirders[i];

                        if (kind == StbColumnKind_structure.RC || kind == StbColumnKind_structure.SRC)
                        {
                            if (gir.kind_structure == StbGirderKind_structure.S)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (gir.kind_structure != StbGirderKind_structure.S)
                            {
                                continue;
                            }
                        }

                        if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node))
                        {
                            if (btmflg)
                            {
                                depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "start");
                            }

                            if (!getflg)
                            {
                                offset_Z = gir.offset_start_Z + depth;
                                getflg = true;
                            }
                            else
                            {
                                double _offset = gir.offset_start_Z + depth;

                                if (btmflg)
                                {
                                    if (offset_Z > _offset)
                                    { offset_Z = _offset; }
                                }
                                else
                                {
                                    if (offset_Z < _offset)
                                    { offset_Z = _offset; }
                                }
                            }
                        }
                        else if (Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                        {
                            if (btmflg)
                            {
                                depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "end");
                            }

                            if (!getflg)
                            {
                                offset_Z = gir.offset_end_Z + depth;
                                getflg = true;
                            }
                            else
                            {
                                double _offset = gir.offset_end_Z + depth;

                                if (btmflg)
                                {
                                    if (offset_Z > _offset)
                                    { offset_Z = _offset; }
                                }
                                else
                                {
                                    if (offset_Z < _offset)
                                    { offset_Z = _offset; }
                                }
                            }
                        }
                    }
                }
            }

            if (!getflg)
            {
                //節点を共有する大梁が無い→大梁に乗っていたらその大梁のレベルに合わせる
                if (stb.StbModel.StbMembers.StbGirders != null)
                {
                    XYZ P = Get_Node_Position(stb.StbModel.StbNodes, node, 0, 0, 0);
                    XYZ P2 = new XYZ(P.X, P.Y, 0);

                    for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                    {
                        var gir = stb.StbModel.StbMembers.StbGirders[i];

                        if (kind == StbColumnKind_structure.RC || kind == StbColumnKind_structure.SRC)
                        {
                            if (gir.kind_structure == StbGirderKind_structure.S)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (gir.kind_structure != StbGirderKind_structure.S)
                            {
                                continue;
                            }
                        }

                        int floor = Get_stbFloor_index(stb, gir.id_node_start);
                        Level lv = SearchLevel(stb, floor);
                        if (lv == null) { lv = SearchLevel_height(stb, gir.id_node_start, gir.id_node_end); }

                        if (btmlevel != lv) { continue; } //同じレベルの梁だけチェックする


                        XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, gir.offset_start_Z);
                        XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, gir.offset_end_Z);


                        if (Ps_gir.X > P.X || P.X > Pe_gir.X || Ps_gir.Y > P.Y || P.Y > Pe_gir.Y) { continue; }

                        XYZ Ps_gir2 = new XYZ(Ps_gir.X, Ps_gir.Y, 0);

                        XYZ vec1 = (Pe_gir - Ps_gir).Normalize();
                        XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();
                        double length = Math.Abs(vec2.DotProduct(P2 - Ps_gir2));
                        if (length < Commons.mm2ft(50))
                        {
                            double depth = 0;
                            if (btmflg)
                            {
                                depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "center");
                            }

                            length = P2.DistanceTo(Ps_gir2);
                            double angle = Math.Asin(vec1.Z);
                            XYZ p3;
                            if (Math.Abs(angle) < 0.001)
                            {
                                //傾斜なし
                                p3 = Ps_gir + vec1 * length;
                            }
                            else
                            {
                                p3 = Ps_gir + vec1 * length / Math.Cos(angle);
                            }

                            double _offset = Commons.ft2mm(p3.Z - P.Z) + depth;

                            if (!getflg)
                            {
                                offset_Z = _offset;
                                getflg = true;
                            }
                            else
                            {
                                if (btmflg)
                                {
                                    if (offset_Z > _offset)
                                    { offset_Z = _offset; }
                                }
                                else
                                {
                                    if (offset_Z < _offset)
                                    { offset_Z = _offset; }
                                }
                            }
                        }

                    }
                }

            }
        }

        /// <summary>
        /// 柱頭の伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="offset_Z"></param>
        private static void Search_Girder_Offset_Z_top(ST_BRIDGE stb, int node, Level toplevel, StbColumnKind_structure kind, out double offset_Z, StbSecColumn_Kind_column kind_Column)
        {
            offset_Z = 0;

            int ind = Get_stbFloor_index(stb, node);

            //大梁
            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                bool getflg = false;
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                {
                    var gir = stb.StbModel.StbMembers.StbGirders[i];

                    if (kind == StbColumnKind_structure.RC || kind == StbColumnKind_structure.SRC)
                    {
                        if (gir.kind_structure == StbGirderKind_structure.S)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (gir.kind_structure != StbGirderKind_structure.S)
                        {
                            continue;
                        }
                    }

                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node))
                    {
                        if (!getflg)
                        {
                            offset_Z = gir.offset_start_Z;
                            getflg = true;
                        }
                        else
                        {
                            if (offset_Z < gir.offset_start_Z)
                            {
                                offset_Z = gir.offset_start_Z;
                            }
                        }
                    }

                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                    {
                        if (!getflg)
                        {
                            offset_Z = gir.offset_end_Z;
                            getflg = true;
                        }
                        else
                        {
                            if (offset_Z < gir.offset_end_Z)
                            {
                                offset_Z = gir.offset_end_Z;
                            }
                        }
                    }
                }

                if (!getflg)
                {
                    //節点を共有する大梁が無い→大梁に乗っていたらその大梁のレベルに合わせる
                    if (stb.StbModel.StbMembers.StbGirders != null)
                    {
                        XYZ P = Get_Node_Position(stb.StbModel.StbNodes, node, 0, 0, 0);
                        XYZ P2 = new XYZ(P.X, P.Y, 0);

                        for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                        {
                            var gir = stb.StbModel.StbMembers.StbGirders[i];

                            if (kind == StbColumnKind_structure.RC || kind == StbColumnKind_structure.SRC)
                            {
                                if (gir.kind_structure == StbGirderKind_structure.S)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (gir.kind_structure != StbGirderKind_structure.S)
                                {
                                    continue;
                                }
                            }

                            int floor = Get_stbFloor_index(stb, gir.id_node_start);
                            Level lv = SearchLevel(stb, floor);
                            if (lv == null) { lv = SearchLevel_height(stb, gir.id_node_start, gir.id_node_end); }

                            if (toplevel != lv) { continue; } //同じレベルの梁だけチェックする


                            XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, gir.offset_start_Z);
                            XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, gir.offset_end_Z);


                            if (Ps_gir.X > P.X || P.X > Pe_gir.X || Ps_gir.Y > P.Y || P.Y > Pe_gir.Y) { continue; }

                            XYZ Ps_gir2 = new XYZ(Ps_gir.X, Ps_gir.Y, 0);

                            XYZ vec1 = (Pe_gir - Ps_gir).Normalize();
                            XYZ vec2 = XYZ.BasisZ.CrossProduct(vec1).Normalize();
                            double length = Math.Abs(vec2.DotProduct(P2 - Ps_gir2));
                            if (length < Commons.mm2ft(50))
                            {
                                double depth = 0;
                                if (kind_Column == StbSecColumn_Kind_column.POST)
                                {
                                    //間柱のときは下端に合せる
                                    depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "center");
                                }

                                length = P2.DistanceTo(Ps_gir2);
                                double angle = Math.Asin(vec1.Z);
                                XYZ p3;
                                if (Math.Abs(angle) < 0.001)
                                {
                                    //傾斜なし
                                    p3 = Ps_gir + vec1 * length;
                                }
                                else
                                {
                                    p3 = Ps_gir + vec1 * length / Math.Cos(angle);
                                }

                                double _offset = Commons.ft2mm(p3.Z - P.Z) + depth;

                                if (!getflg)
                                {
                                    offset_Z = _offset;
                                    getflg = true;
                                }
                                else
                                {
                                    if (kind_Column == StbSecColumn_Kind_column.POST)
                                    {
                                        if (offset_Z > _offset)
                                        { offset_Z = _offset; }
                                    }
                                    else
                                    {
                                        if (offset_Z < _offset)
                                        { offset_Z = _offset; }
                                    }
                                }

                            }

                        }
                    }
                }
            }
        }


        /// <summary>
        /// ブレース伸縮用梁のオフセット値（全体座標系）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="offset_X"></param>
        /// <param name="offset_Y"></param>
        /// <param name="offset_Z"></param>
        /// <param name="start_end">"start", "end"</param>
        private static void Search_Girder_Offset_XYZ(ST_BRIDGE stb, int id, out double offset_X, out double offset_Y, out double offset_Z, string start_end)
        {
            offset_X = 0;
            offset_Y = 0;
            offset_Z = 0;

            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                var gir = stb.StbModel.StbMembers.StbGirders.Find(a => a.id == id);
                if (gir != null)
                {
                    if (start_end == "start")
                    {
                        offset_X = gir.offset_start_X;
                        offset_Y = gir.offset_start_Y;
                        offset_Z = gir.offset_start_Z;
                    }
                    else
                    {
                        offset_X = gir.offset_end_X;
                        offset_Y = gir.offset_end_Y;
                        offset_Z = gir.offset_end_Z;
                    }

                    return;
                }
            }

            if (stb.StbModel.StbMembers.StbBeams != null)
            {
                var gir = stb.StbModel.StbMembers.StbBeams.Find(a => a.id == id);
                if (gir != null)
                {
                    if (start_end == "start")
                    {
                        offset_X = gir.offset_start_X;
                        offset_Y = gir.offset_start_Y;
                        offset_Z = gir.offset_start_Z;
                    }
                    else
                    {
                        offset_X = gir.offset_end_X;
                        offset_Y = gir.offset_end_Y;
                        offset_Z = gir.offset_end_Z;
                    }

                    return;
                }
            }
        }


        /// <summary>
        /// 柱のオフセット値（全体座標系）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="offset_X"></param>
        /// <param name="offset_Y"></param>
        /// <param name="offset_Z"></param>
        private static bool Search_Column_Offset_XYZ(ST_BRIDGE stb, int node, out double offset_X, out double offset_Y, out double offset_Z)
        {
            bool ret = false;

            offset_X = 0;
            offset_Y = 0;
            offset_Z = 0;

            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                foreach (var clm in stb.StbModel.StbMembers.StbColumns)
                {
                    if (Node_Check(stb.StbModel.StbNodes, clm.id_node_bottom, node))
                    {
                        offset_X = clm.offset_bottom_X;
                        offset_Y = clm.offset_bottom_Y;
                        offset_Z = clm.offset_bottom_Z;
                        ret = true;
                        return ret;
                    }

                    if (Node_Check(stb.StbModel.StbNodes, clm.id_node_top, node))
                    {
                        offset_X = clm.offset_top_X;
                        offset_Y = clm.offset_top_Y;
                        offset_Z = clm.offset_top_Z;
                        ret = true;
                        return ret;
                    }
                }
            }

            if (stb.StbModel.StbMembers.StbPosts != null)
            {
                foreach (var clm in stb.StbModel.StbMembers.StbPosts)
                {
                    if (Node_Check(stb.StbModel.StbNodes, clm.id_node_bottom, node))
                    {
                        offset_X = clm.offset_bottom_X;
                        offset_Y = clm.offset_bottom_Y;
                        offset_Z = clm.offset_bottom_Z;
                        ret = true;
                        return ret;
                    }

                    if (Node_Check(stb.StbModel.StbNodes, clm.id_node_top, node))
                    {
                        offset_X = clm.offset_top_X;
                        offset_Y = clm.offset_top_Y;
                        offset_Z = clm.offset_top_Z;
                        ret = true;
                        return ret;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 梁の伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="Ps"></param>
        /// <param name="Pe"></param>
        /// <param name="start_end"></param>
        /// <param name="vecU"></param>
        /// <param name="id"></param>
        /// <param name="btmlevel"></param>
        /// <param name="rotate">伸縮する梁の断面回転</param>
        /// <param name="offset2">全体座標系でのoffset</param>
        /// <returns></returns>
        private static XYZ Search_Offset_gir(ST_BRIDGE stb, int node, ref XYZ Ps, ref XYZ Pe, string start_end, XYZ vecU, int id, Level btmlevel, double rotate, out XYZ offset2)
        {
            XYZ offset = new XYZ(); //梁の方向をX軸としたSTBの部材座標系
            offset2 = new XYZ();

            //①節点を共有する柱がある
            if (Search_Column_Offset_XYZ(stb, node, out double clm_offset_X, out double clm_offset_Y, out double clm_offset_Z))
            {
                offset2 = new XYZ(clm_offset_X, clm_offset_Y, 0);
                offset = Data.TransformCoord(Ps, Pe, clm_offset_X, clm_offset_Y, 0, rotate);
            }
            else
            {
                //伸縮する梁の方向
                XYZ VecU = (Pe - Ps).Normalize();
                //②節点を共有する大梁があるとき
                bool getflg_1 = false;
                double B = 0;
                double gir_offset_X = 0, gir_offset_Y = 0;  //全体座標系

                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                {
                    if (getflg_1) { break; }

                    var gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb.StbModel.StbNodes, node, gir.id_node_start))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0);
                        XYZ VecU_gir = (Pe - Ps).Normalize();
                        if (vecU.X == VecU_gir.X && vecU.Y == VecU_gir.Y) { continue; } //XY平面で同じ方向なら無視する
                        if (Search_Girder_Samevec(stb, node, gir.id, VecU_gir)) { continue; } //同じ方向の梁があるときは考慮しない（伸縮する梁との交点がT字型のとき）

                        gir_offset_X = gir.offset_start_X;
                        gir_offset_Y = gir.offset_start_Y;

                        getflg_1 = true;
                    }
                    else if (Node_Check(stb.StbModel.StbNodes, node, gir.id_node_end))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0);
                        XYZ VecU_gir = (Pe - Ps).Normalize();
                        if (vecU.X == VecU_gir.X && vecU.Y == VecU_gir.Y) { continue; } //XY平面で同じ方向なら無視する
                        if (Search_Girder_Samevec(stb, node, gir.id, VecU_gir)) { continue; } //同じ方向の梁があるときは考慮しない（伸縮する梁との交点がT字型のとき）

                        gir_offset_X = gir.offset_end_X;
                        gir_offset_Y = gir.offset_end_Y;

                        getflg_1 = true;
                    }

                    if (getflg_1 && !Search_Girder_Samevec(stb, node, id, vecU))
                    {
                        B = Get_Girder_B(stb, gir.id_node_start, gir.id_node_end);
                        if (start_end == "start")
                        {
                            gir_offset_X += B * VecU.X / 2;
                            gir_offset_Y += B * VecU.Y / 2;
                        }
                        else
                        {
                            gir_offset_X += -B * VecU.X / 2;
                            gir_offset_Y += -B * VecU.Y / 2;
                        }
                    }
                }
                if (getflg_1)
                {
                    offset2 = new XYZ(gir_offset_X, gir_offset_Y, 0);
                    offset = Data.TransformCoord(Ps, Pe, gir_offset_X, gir_offset_Y, 0, rotate);
                }
            }


            return offset;
        }

        /// <summary>
        /// VecU_girと同じ向きの梁がある→true
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="id"></param>
        /// <param name="vecU"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        private static bool Search_Girder_Samevec(ST_BRIDGE stb, int node, int id, XYZ vecU)
        {
            bool ret = false;
            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                foreach (var gir in stb.StbModel.StbMembers.StbGirders)
                {
                    if (gir.id == id) { continue; }

                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node) ||
                        Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                    {
                        XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, 0);
                        XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, 0);
                        XYZ vec = (Pe - Ps).Normalize();

                        if (Math.Abs(vec.X) == Math.Abs(vecU.X) &&
                            Math.Abs(vec.Y) == Math.Abs(vecU.Y) &&
                            Math.Abs(vec.Z) == Math.Abs(vecU.Z))
                        {
                            ret = true;
                            return ret;
                        }
                    }
                }
            }

            if (!ret && stb.StbModel.StbMembers.StbBeams != null)
            {
                foreach (var gir in stb.StbModel.StbMembers.StbBeams)
                {
                    if (gir.id == id) { continue; }

                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node) ||
                        Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                    {
                        XYZ Ps = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, 0);
                        XYZ Pe = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, 0);
                        XYZ vec = (Pe - Ps).Normalize();

                        if (Math.Abs(vec.X) == Math.Abs(vecU.X) &&
                            Math.Abs(vec.Y) == Math.Abs(vecU.Y) &&
                            Math.Abs(vec.Z) == Math.Abs(vecU.Z))
                        {
                            ret = true;
                            return ret;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// ブレースの伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="Ps"></param>
        /// <param name="Pe"></param>
        /// <param name="start_end">"start", "end"</param>
        /// <param name="kind_brace"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        private static XYZ Search_Offset_bra(ST_BRIDGE stb, int node, XYZ Ps, XYZ Pe, string start_end, StbSecBrace_SKind_brace kind_brace, double rotate)
        {
            XYZ offset = new XYZ();
            XYZ vecU = new XYZ();
            if (start_end == "start")
            { vecU = (Pe - Ps).Normalize(); }
            else
            { vecU = (Ps - Pe).Normalize(); }

            double gir_offset_X = 0, gir_offset_Y = 0, gir_offset_Z = 0;

            if (kind_brace == StbSecBrace_SKind_brace.VERTICAL)
            {
                double angle = 0;
                int id = 0;
                string s_e = "";
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                {
                    var gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node) ||
                        Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, 0);
                        XYZ vecU_gir = new XYZ();

                        if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node))
                        {
                            vecU_gir = (Pe_gir - Ps_gir).Normalize();
                            s_e = "start";
                        }
                        else
                        {
                            vecU_gir = (Ps_gir - Pe_gir).Normalize();
                            s_e = "end";
                        }

                        if (vecU_gir.DotProduct(vecU) <= 0) { continue; }

                        if (angle == 0)
                        {
                            angle = vecU.AngleTo(vecU_gir);
                            id = gir.id;
                        }
                        else if (angle > vecU.AngleTo(vecU_gir))
                        {
                            angle = vecU.AngleTo(vecU_gir);
                            id = gir.id;
                        }
                    }
                }

                if (s_e != "")
                {
                    Search_Girder_Offset_XYZ(stb, id, out gir_offset_X, out gir_offset_Y, out gir_offset_Z, s_e);
                }
            }
            else
            {
                double angle_min = 0;
                double angle_max = 0;
                int id_min = 0;
                int id_max = 0;
                string s_e = "";
                string s_e_min = "";
                string s_e_max = "";
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count; i++)
                {
                    var gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node) ||
                        Node_Check(stb.StbModel.StbNodes, gir.id_node_end, node))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_start, 0, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb.StbModel.StbNodes, gir.id_node_end, 0, 0, 0);
                        XYZ vecU_gir = new XYZ();

                        if (Node_Check(stb.StbModel.StbNodes, gir.id_node_start, node))
                        {
                            vecU_gir = (Pe_gir - Ps_gir).Normalize();
                            s_e = "start";
                        }
                        else
                        {
                            vecU_gir = (Ps_gir - Pe_gir).Normalize();
                            s_e = "end";
                        }

                        if (vecU_gir.DotProduct(vecU) <= 0) { continue; }

                        if (angle_min == 0)
                        {
                            angle_min = vecU.AngleOnPlaneTo(vecU_gir, BasisZ);
                            id_min = gir.id;
                            s_e_min = s_e;
                        }
                        else if (angle_min > vecU.AngleOnPlaneTo(vecU_gir, BasisZ))
                        {
                            angle_min = vecU.AngleOnPlaneTo(vecU_gir, BasisZ);
                            id_min = gir.id;
                            s_e_min = s_e;
                        }

                        if (angle_max == 0)
                        {
                            angle_max = vecU.AngleOnPlaneTo(vecU_gir, BasisZ);
                            id_max = gir.id;
                            s_e_max = s_e;
                        }
                        else if (angle_max < vecU.AngleOnPlaneTo(vecU_gir, BasisZ))
                        {
                            angle_max = vecU.AngleOnPlaneTo(vecU_gir, BasisZ);
                            id_max = gir.id;
                            s_e_max = s_e;
                        }
                    }
                }
                if (s_e_min != "")
                {
                    Search_Girder_Offset_XYZ(stb, id_min, out double x, out double y, out double z, s_e_min);
                    gir_offset_X += x;
                    gir_offset_Y += y;
                    gir_offset_Z += 0;
                }
                if (s_e_max != "")
                {
                    Search_Girder_Offset_XYZ(stb, id_max, out double x, out double y, out double z, s_e_max);
                    gir_offset_X += x;
                    gir_offset_Y += y;
                    gir_offset_Z += z;
                }
                if (s_e_max != "" && s_e_min != "")
                { gir_offset_Z = gir_offset_Z / 2; }
            }


            if (Search_Column_Offset_XYZ(stb, node, out double clm_offset_X, out double clm_offset_Y, out double clm_offset_Z))
            {
                offset = Data.TransformCoord(Ps, Pe, clm_offset_X, 0, gir_offset_Z, rotate);
            }
            else
            {
                offset = Data.TransformCoord(Ps, Pe, gir_offset_X, gir_offset_Y, gir_offset_Z, rotate);
            }

            return offset;
        }


        /// <summary>
        /// 鉄骨形状判定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="shape"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        private static string Check_Steel(ST_BRIDGE stb, string shape, out int ind)
        {
            ind = -1;

            if (stb.StbModel.StbSections.StbSecSteel != null)
            {
                var steel = stb.StbModel.StbSections.StbSecSteel;

                if (steel.StbSecRollH != null)
                {
                    ind = steel.StbSecRollH.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_H;
                    }
                }

                if (steel.StbSecBuildH != null)
                {
                    ind = steel.StbSecBuildH.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_BH;
                    }
                }

                if (steel.StbSecRollBox != null)
                {
                    ind = steel.StbSecRollBox.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_Box;
                    }
                }

                if (steel.StbSecBuildBox != null)
                {
                    ind = steel.StbSecBuildBox.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_BBox;
                    }
                }

                if (steel.StbSecPipe != null)
                {
                    ind = steel.StbSecPipe.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_Pipe;
                    }
                }

                if (steel.StbSecRollT != null)
                {
                    ind = steel.StbSecRollT.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_T;
                    }
                }

                if (steel.StbSecRollC != null)
                {
                    ind = steel.StbSecRollC.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_C;
                    }
                }

                if (steel.StbSecRollL != null)
                {
                    ind = steel.StbSecRollL.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_L;
                    }
                }

                if (steel.StbSecLipC != null)
                {
                    ind = steel.StbSecLipC.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_LipC;
                    }
                }

                if (steel.StbSecFlatBar != null)
                {
                    ind = steel.StbSecFlatBar.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_FB;
                    }
                }

                if (steel.StbSecRoundBar != null)
                {
                    ind = steel.StbSecRoundBar.FindIndex(a => a.name == shape);
                    if (ind >= 0)
                    {
                        return RevitLNK.st_steel_Bar;
                    }
                }

            }

            return "";
        }

        #region 鉄骨サイズチェック⇒成・幅・厚さ0なら変換対象外

        private static string Roll_H_Size_Check(StbSecRollH steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.t1 == 0)
            {
                if (txt == "")
                { txt += "ウェブ厚"; }
                else
                { txt += ",ウェブ厚"; }
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt += "フランジ厚"; }
                else
                { txt += ",フランジ厚"; }
            }
            return txt;
        }
        private static string Build_H_Size_Check(StbSecBuildH steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.t1 == 0)
            {
                if (txt == "")
                { txt += "ウェブ厚"; }
                else
                { txt += ",ウェブ厚"; }
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt += "フランジ厚"; }
                else
                { txt += ",フランジ厚"; }
            }
            return txt;
        }
        private static string Roll_Box_Size_Check(StbSecRollBox steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.t == 0)
            {
                if (txt == "")
                { txt += "板厚"; }
                else
                { txt += ",板厚"; }
            }
            return txt;
        }
        private static string Build_Box_Size_Check(StbSecBuildBox steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.t1 == 0)
            {
                if (txt == "")
                { txt += "成方向の板厚"; }
                else
                { txt += ",ウェブ厚"; }
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt += "幅方向の板厚"; }
                else
                { txt += ",幅方向の板厚"; }
            }
            return txt;
        }
        private static string Pipe_Size_Check(StbSecPipe steel)
        {
            string txt = "";

            if (steel.D == 0)
            {
                txt = "直径";
            }
            if (steel.t == 0)
            {
                if (txt == "")
                { txt = "板厚"; }
                else
                { txt += ",板厚"; }
            }
            return txt;
        }
        private static string Roll_T_Size_Check(StbSecRollT steel)
        {
            string txt = "";

            if (steel.t1 == 0)
            {
                txt = "ウェブ厚";
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt = "フランジ厚"; }
                else
                { txt += ",フランジ厚"; }
            }
            return txt;
        }
        private static string Roll_C_Size_Check(StbSecRollC steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "フランジ幅"; }
                else
                { txt += ",フランジ幅"; }
            }
            if (steel.t1 == 0)
            {
                if (txt == "")
                { txt += "ウェブ厚"; }
                else
                { txt += ",ウェブ厚"; }
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt += "フランジ厚"; }
                else
                { txt += ",フランジ厚"; }
            }
            return txt;
        }
        private static string Roll_L_Size_Check(StbSecRollL steel)
        {
            string txt = "";

            if (steel.A == 0)
            { txt += "成"; }
            if (steel.B == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.t1 == 0)
            {
                if (txt == "")
                { txt += "成方向の板厚厚"; }
                else
                { txt += ",成方向の板厚"; }
            }
            if (steel.t2 == 0)
            {
                if (txt == "")
                { txt += "幅方向の板厚"; }
                else
                { txt += ",幅方向の板厚"; }
            }
            return txt;
        }
        private static string Rool_LipC_Size_Check(StbSecLipC steel)
        {
            string txt = "";

            if (steel.H == 0)
            { txt += "成"; }
            if (steel.A == 0)
            {
                if (txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if (steel.C == 0)
            {
                if (txt == "")
                { txt += "リップ長"; }
                else
                { txt += ",リップ長"; }
            }
            if (steel.t == 0)
            {
                if (txt == "")
                { txt += "板厚"; }
                else
                { txt += ",板厚"; }
            }
            return txt;
        }

        #endregion


        /// <summary>
        /// 壁・スラブの鉄筋径(二つ入力されていることがある) を分解する
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static string[] Get_D2(string d)
        {
            if (Regex.IsMatch(d.Trim(), @"^([a-zA-Z]+\d{1,2}){2}$"))
            {
                var m = Regex.Match(d.Trim(), @"[a-zA-Z]+\d{1,2}");
                string d1 = m.Value;
                string d2 = m.NextMatch().Value;
                return new string[] { d1, d2 };
            }
            else
            {
                return new string[] { d.Trim(), "" };
            }
        }


        /// <summary>
        /// 鉄骨材料（ウェブ）の取得
        /// </summary>
        /// <param name="strength_web">ウェブ</param>
        /// <param name="strength_main">主</param>
        /// <returns>ウェブがブランクなら主を返す</returns>
        private static string GetStrength_web(string strength_web, string strength_main)
        {
            return (strength_web != null && strength_web != "") ? strength_web : strength_main;
        }


        /// <summary>
        /// 変換ファミリの再セット
        /// </summary>
        /// <param name="ConvFamily">[In/Out]変換ファミリ</param>
        /// <param name="familyname">ファミリ名称</param>
        /// <param name="familyname2">チェックするファミリ名称</param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        internal static void SetConvertFamily(ref Family[][] ConvFamily, string[][] familyname, string familyname2, int i, int j)
        {
            //同一名称のファミリを使用していると、LoadFamilyを実行したときにそれより前の同一Familyが消えてしまう。
            for (int i2 = 0; i2 < ConvFamily.Length; ++i2)
            {
                if (i2 > i) break;
                for (int j2 = 0; j2 < ConvFamily[i2].Length; ++j2)
                {
                    if (i2 == i && j2 >= j) break;

                    if (familyname[i2][j2] == familyname2)
                    {
                        ConvFamily[i2][j2] = ConvFamily[i][j];
                    }
                }
            }
        }

        
        internal static void OutputDebubCommentLog<T>(T obj, int id, string logname, string typename, int[] nodeids ){
            if ( ! ShouldOutputCommentDebugLog ) return ;
            if ( obj is FamilyInstance ) {
                var instance = obj as FamilyInstance ;
                var commentParam = instance.LookupParameter( "コメント" ) ;
                var nodeCoordStr = nodeids.Length == 0 ? "" : $",[節点Id{Data.MakeLog_Coord( 1, nodeids )}]" ;
                commentParam.Set( $"{logname}[配置Id{id}]{typename}{nodeCoordStr}" ) ;
                return;
            }
            if ( obj is Wall ) {
                var instance = obj as Wall ;
                var commentParam = instance.LookupParameter( "コメント" ) ;
                commentParam.Set( $"{logname}[配置Id{id}]{typename},[節点Id{Data.MakeLog_Coord( 1, nodeids )}]" ) ;
                return;
            }
            if ( obj is Floor ) {
                var instance = obj as Floor ;
                var commentParam = instance.LookupParameter( "コメント" ) ;
                commentParam.Set( $"{logname}[配置Id{id}]{typename},[節点Id{Data.MakeLog_Coord( 1, nodeids )}]" ) ;
                return;
            }
            return;
        }
        
    }
}
