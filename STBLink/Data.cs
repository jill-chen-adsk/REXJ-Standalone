using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STBLink
{
    class Data
    {
        //STB1.4,2.0共通で使えるクラス等のデータ

        internal const string schemaName_StbCommon = "STBridgeLink_StbCommon";
        internal const string schemaName_Guid = "STBridgeLink_Guid";
        internal const string FieldName = "Guid_List";


        #region Progressbar

        //STB2.0用
        private static ProgressBar2 progress = null;
        private static Task progressTask = null;
        internal static IntPtr RevitHandle = new IntPtr();

        /// <summary>
        /// ウィンドウハンドルからParentを指定するためのラッパークラス
        /// </summary>
        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public IntPtr Handle { get; private set; }

            public WindowWrapper(IntPtr _h)
            {
                this.Handle = _h;
            }
        }

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="msg">ラベルに表示するメッセージ</param>
        /// <param name="maximum">bar.最大値</param>
        internal static void ProgressStart(string msg, int maximum)
        {
            progress = new ProgressBar2(msg, maximum);

            //Revit WindowにCenterParentで表示するため、ハンドルを指定する。
            WindowWrapper wrapper = new WindowWrapper(RevitHandle);

            progressTask = Task.Run(() => progress.ShowDialog(wrapper));
        }

        /// <summary>
        /// Step分増加
        /// </summary>
        internal static void ProgressPerformStep()
        {
            progress.PerformStep();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        internal static void ProgressClose()
        {
            if (progress != null)
            {
                progress.ProgressClose();
                progressTask.Wait();
                progress = null;
                progressTask = null;
            }
        }

        internal static void ProgressRestart(string msg, int maximum)
        {
            if (progress != null)
            {
                progress.Restart(msg, maximum);
            }
            else
            {
                ProgressStart(msg, maximum);
            }
        }

        #endregion



        /// <summary>
        /// プロジェクト情報
        /// </summary>
        internal static readonly List<string> projectParams = new List<string>()
        {
            "STBファイル名",                 //0
            "STBファイル更新日時",           //1
            "STBレベルマッピング設定",       //2
            "STB基点位置設定",               //3
            "STBコンクリート設定",           //4
            "STB鉄骨設定",                   //5
            "STBグローバルID",               //6
            "STBプロジェクト名",             //7
            "STBアプリケーション名",         //8
            "STB建物全体のコンクリート強度", //9
            "STB鉄骨規格",                   //10
            "STB径別鉄筋強度情報"            //11
        };


        /// <summary>
        /// マッピングテーブルで設定したオフセット一覧
        /// </summary>
        internal class OffsetZ
        {
            internal double offset = 0;
            internal Level lev = null;
            internal int stbid = 0;
        }

        /// <summary>
        /// 基礎梁・布基礎グループ化
        /// </summary>
        internal class CGroup
        {
            internal int start_node = 0;
            internal int end_node = 0;
            internal List<ElementId> elId = new List<ElementId>();
            internal List<Element> elem = new List<Element>();
        }

        internal class TypeName_Data
        {
            internal string typename = "";
            internal int id = 0;
            internal string shapename = "";
        }

        /// <summary>
        /// 名称変更用
        /// </summary>
        internal class ReNameSymbols
        {
            internal string name = "";
            internal double Length = 0;
            internal double Length2 = 0;
            internal double Length3 = 0;
            internal string BHaunch1 = "";
            internal string BHaunch2 = "";
            internal int id = 0;
            internal FamilySymbol symbol = null;
        }

        internal class IsOutin_Girder
        {
            internal int id = 0;
            internal FamilySymbol symbol = null;
            internal string section_io_start = "";
            internal string section_io_end = "";
        }


        /// <summary>
        /// 柱脚一覧
        /// </summary>
        internal class BaseClass
        {
            internal int id_section = 0;
            internal string clmname = "";
            internal string clm_structure = "";
            internal string product_company = "";
            internal string product_code = "";
        }



        /// <summary>
        /// 勝ち負け判定優先順位
        /// </summary>
        internal enum Joinorder
        {
            pile = 0,
            foundation,
            column,
            girder,
            beam,
            wall,
            brace,
            slab
        }



        #region ログ生成

        internal static void MakeGridLog(string stage, string name, XYZ ps, XYZ pe, int mode)
        {
            XYZ[] pos = new XYZ[0];
            if (pe != null)
            {
                Array.Resize(ref pos, 2);
                pos[0] = ps;
                pos[1] = pe;
            }
            else
            {
                Array.Resize(ref pos, 1);
                pos[0] = ps;
            }

            string log = "";
            log += stage + "\t";
            log += name + ",[";
            log += MakeLog_Coord(mode, pos);
            log += "]";

            LogData.AddLog(0, 0, log);
        }

        /// <summary>
        /// 節点IDで表示するログ
        /// </summary>
        /// <param name="stage">生成された部材</param>
        /// <param name="name">タイプ名(符号)</param>
        /// <param name="nodeid">配置ID</param>
        /// <param name="mode"></param>
        internal static void MakeNodeLog(string stage, string name, int[] nodeid, int mode, ElementId elementId)
        {
            string log = "";
            log += stage + "\t";
            log += name + ",[節点Id";
            log += MakeLog_Coord(mode, nodeid);
            log += "]";
            log += $" 要素Id{elementId}" ;

            LogData.AddLog(0, 0, log);
        }

        /// <summary>
        /// 節点IDリストで表示するログ
        /// </summary>
        /// <param name="stage">生成された部材</param>
        /// <param name="name">タイプ名(符号)</param>
        /// <param name="nodeid">配置IDリスト</param>
        /// <param name="mode"></param>
        internal static void MakeNodeLog(string stage, string name, List<STBclass.StbNodeid> nodes, int mode, ElementId elementId)
        {
            string log = "";
            log += stage + "\t";
            log += name + ",[節点Id(";
            for (int i = 0; i < nodes.Count(); i++)
            {
                log += nodes[i].id.ToString();
                if (i != nodes.Count() - 1)
                {
                    log += ",";
                }
            }
            log += ")]";
            log += $" 要素Id{elementId}" ;

            LogData.AddLog(0, 0, log);
        }

        /// <summary>
        /// 座標値で表示するログ
        /// </summary>
        /// <param name="mode">0：Z座標のみ 1：X,Y座標 2：X,Y,Z座標</param>
        /// <param name="coord"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal static string MakeLog_Coord(int mode, XYZ[] coord, string format = "0.0")
        {
            string ret = "";

            for (int i = 0; i < coord.Length; i++)
            {
                if (coord[i] == null) continue;
                ret += "(";
                switch (mode)
                {
                    case 0:
                        ret += Commons.ft2mm(coord[i].Z).ToString(format);
                        break;
                    case 1:
                        ret += Commons.ft2mm(coord[i].X).ToString(format) + ", ";
                        ret += Commons.ft2mm(coord[i].Y).ToString(format);
                        break;
                    case 2:
                        ret += Commons.ft2mm(coord[i].X).ToString(format) + ", ";
                        ret += Commons.ft2mm(coord[i].Y).ToString(format) + ", ";
                        ret += Commons.ft2mm(coord[i].Z).ToString(format);
                        break;
                }
                ret += ")";
                if (i != coord.Length - 1)
                { ret += "-"; }
            }
            return ret;
        }

        public static string MakeLog_Coord(int mode, int[] coord, string format = "0.0")
        {
            string ret = "";

            ret += "(";
            for (int i = 0; i < coord.Length; i++)
            {

                ret += coord[i].ToString();
                if (i != coord.Length - 1)
                { ret += ","; }
            }
            ret += ")";
            return ret;
        }

        /// <summary>
        /// 変換不可ログ（各断面の鉄骨種類が異なる）
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        internal static void MakeTekkotuLog(string kind, string name, int id)
        {
            LogData.AddLog(LogData.LogKind.Warning, 0, "[" + kind + "]" + name + "(断面id=" + id.ToString() + ")" + "異なる鋼材で構成された部材は変換できません。");
        }

        /// <summary>
        /// 断面ログ（どの断面で変換したか）
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="shape"></param>
        /// <param name="shapename"></param>
        /// <param name="danmen"></param>
        internal static void MakeDanmenLog(string kind, string name, int id, string shape, string shapename, string danmen)
        {
            LogData.AddLog(LogData.LogKind.Warning, 0, "[" + kind + "]" + name + "(断面id=" + id.ToString() + ")は" + shape + "(" + shapename + ")のため" + danmen + "断面で変換しました。");
        }

        /// <summary>
        /// 鉄骨サイズログ
        /// </summary>
        /// <param name="shapename"></param>
        /// <param name="typename"></param>
        /// <param name="id"></param>
        /// <param name="logtxt"></param>
        /// <param name="flg">0：変換対象外（せい・幅・厚さなど） 1：1mmに設定（フィレット半径）</param>
        internal static void MakeSizeLog(string shapename, string typename, int id, string logtxt, int flg)
        {
            switch (flg)
            {
                case 0:
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[" + shapename + "]" + typename + "(断面id=" + id.ToString() + ")は" + logtxt +
                                   "が0mmのため変換できません。");
                    break;
                case 1:
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[" + shapename + "]" + typename + "(断面id=" + id.ToString() + ")は" + logtxt +
                                   "が1mm未満のため値を1mmに設定しました。");
                    break;
            }

        }

        /// <summary>
        /// 変換対象外ログ
        /// </summary>
        /// <param name="kind_structure"></param>
        /// <param name="id"></param>
        /// <param name="typename"></param>
        /// <param name="shapename"></param>
        /// <param name="shapename_J"></param>
        internal static void Make_taisyougaiLog(string kind_structure, int id, string typename, string shapename, string shapename_J)
        {
            LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + kind_structure + "]" + typename + "(断面id=" + id.ToString() + ")は" +
                            shapename + "(" + shapename_J + ")");
        }

        /// <summary>
        /// 水平・鉛直ハンチ種類のログ
        /// </summary>
        /// <param name="logname"></param>
        /// <param name="typename"></param>
        /// <param name="id"></param>
        internal static void Make_haunchLog(string logname, string typename, int id)
        {
            //switch(logname)
            //{
            //    case "DROP":
            //        LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(配置id=" + id.ToString() + ")のハンチ種類はDROPで変換されました。");
            //        break;
            //    case "SLOPE":
            //        LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(配置id=" + id.ToString() + ")のハンチ種類はSLOPEで変換されました。");
            //        break;
            //    case "BOTH_H":
            //        LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(配置id=" + id.ToString() + ")の水平ハンチ形状はBOTHで変換されました。");
            //        break;
            //    case "BOTH_V":
            //        LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(配置id=" + id.ToString() + ")の鉛直ハンチ形状はBOTHで変換されました。");
            //        break;
            //    case "TOP":
            //        LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(配置id=" + id.ToString() + ")の鉛直ハンチ形状はTOPで変換されました。");
            //        break;
            //}
        }

        /// <summary>
        /// 2丁扱いログ
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="id"></param>
        /// <param name="shape"></param>
        /// <param name="shapename_J"></param>
        /// <param name="flg">true：梁 false：ブレース</param>
        internal static void Make_typeLog(string typename, int id, string shape, string shapename_J, bool flg = true)
        {
            if (flg)
            { LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁]" + typename + "(断面id=" + id.ToString() + ")" + shape + "(" + shapename_J + ")2丁扱いは単材として変換されました。"); }
            else
            { LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + id.ToString() + ")" + shape + "(" + shapename_J + ")2丁扱いは単材として変換されました。"); }
        }

        internal static void Make_TekkinkeiLog(string typename, int id, string pos, string buzai, int flg)
        {
            switch (flg)
            {
                case 1:
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[" + buzai + "]" + typename + "(断面id=" + id.ToString() + ")" + pos + "の鉄筋径が空欄のため鉄筋径は変換されませんでした。");
                    break;
                case 2:
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[" + buzai + "]" + typename + "(断面id=" + id.ToString() + ")" + pos + "の鉄筋径の書式が正しくないため鉄筋径は変換されませんでした。");
                    break;
            }

        }

        #endregion


        /// <summary>
        /// STBの鉄骨材料情報
        /// </summary>
        /// <param name="matname"></param>
        internal static void MateData_Add(string matname)
        {
            if (matname == null || matname == "") return;
            if (!RevitLNK.MateData.Any(a => a.stbmatName == matname))
            {
                RevitLNK.Materialdata md = new RevitLNK.Materialdata
                {
                    stbmatName = matname
                };
                RevitLNK.MateData.Add(md);
            }
        }


        /// <summary>
        /// 名前の重複チェック
        /// </summary>
        /// <param name="existingname">プロジェクト内で使用されている名前</param>
        /// <param name="newname">新しくつけようとしている名前</param>
        /// <returns></returns>
        internal static bool Name_Check(List<string> existingname, string newname)
        {
            bool ret = false;
            for (int i = 0; i < existingname.Count(); i++)
            {
                if (existingname[i].Equals(newname, StringComparison.CurrentCultureIgnoreCase))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }



        /// <summary>
        /// FamilySymbolの検索
        /// </summary>
        /// <param name="fam"></param>
        /// <param name="typename"></param>
        /// <param name="symbol"></param>
        /// <param name="mid"></param>
        /// <param name="mid_name"></param>
        /// <returns></returns>
        internal static bool SearchFamilySymbol(Family fam, string typename, ref FamilySymbol symbol, int mid = -1, string mid_name = "")
        {
            bool sameflg = false;
            ISet<ElementId> slist = fam.GetFamilySymbolIds();
            IList<ElementId> elist = slist.ToList<ElementId>();

            List<FamilySymbol> symbols = new List<FamilySymbol>();

            foreach (ElementId e in elist)
            {
                FamilySymbol s = (FamilySymbol)Commons.doc.GetElement(e);
                if (s != null)
                {
                    if (mid_name != "")
                    {
                        Parameter p = s.LookupParameter(mid_name);
                        if (p != null)
                        {
                            symbols.Add(s);

                            if (p.AsInteger() != mid)
                            { continue; }
                        }
                    }
                    if (s.Name.ToUpper() == typename.ToUpper())
                    {
                        symbol = s;
                        sameflg = true;
                        break;
                    }
                    else if (symbol == null)
                    {
                        symbol = s;
                    }
                }
            }

            if (!sameflg)
            {
                //多層にまたがる柱の場合、サフィックスが付いているかもしれないのでチェック
                foreach (var s in symbols)
                {
                    Parameter p = s.LookupParameter(mid_name);
                    if (p != null && p.AsInteger() == mid)
                    {
                        if (s.Name.Contains(typename))
                        {
                            //断面IDが同じで、指定名称を含んでいるならOKとする。
                            symbol = s;
                            sameflg = true;
                            break;
                        }
                    }
                }
            }


            return sameflg;
        }



        /// <summary>
        /// 名前の付け替え
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="ascii"></param>
        /// <returns></returns>
        internal static string ReName(string typename, int ascii)
        {
            string newname = "";
            if (ascii < 123)
            { newname = typename + "_" + (char)ascii; }
            else
            {
                int ascii2 = 97; // =a
                int s = 0;
                int aaa = ascii;
                do
                {
                    aaa = aaa - 26;
                    s++;
                } while (aaa > 122);
                ascii2 = 96 + s;
                newname = typename + "_" + (char)ascii2 + (char)(ascii - 26 * s);
            }
            return newname;
        }
        internal static string ReName2(Family fami, string typename)
        {
            int ascii = 97;
            string newname;
            FamilySymbol symbol = null;
            do
            {
                newname = ReName(typename, ascii);
                ascii++;
            } while (SearchFamilySymbol(fami, newname, ref symbol));

            return newname;
        }





        /// <summary>
        /// 文字列から数字を抜き出す
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static int Get_Num(string s)
        {
            string name = "";
            int num = 0;
            if (s == "" || s == null) { return num; }

            bool getflg = false;
            StringBuilder buf = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                //数字であればStringBuilderに追加する
                if (char.IsDigit(c))
                {
                    buf.Append(c);
                    getflg = true;
                }
                else
                {
                    if (getflg)
                        break;
                }
            }
            name = buf.ToString();
            int.TryParse(name, out num);
            return num;
        }

        /// <summary> 鉄筋強度の大きい方を返す
        /// </summary>
        /// <param name="strength1"></param>
        /// <param name="strength2"></param>
        /// <returns></returns>
        internal static string Compare_strength(string strength1, string strength2)
        {
            string get_st = "";
            int s1 = Get_Num(strength1);
            int s2 = Get_Num(strength2);
            if (s1 > s2)
            { get_st = strength1; }
            else
            { get_st = strength2; }
            return get_st;
        }


        /// <summary>
        /// マテリアルのElementIdを取得
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cate"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        internal static bool SearchMaterial(string name, string cate, ref ElementId eid)
        {
            bool sameflg = false;
            Material basemat = null;
            switch (cate)
            {
                case "メタル":
                    basemat = RevitLNK.LoFa.RevitMatName[0];
                    FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                    ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
                    IList<Element> elements = collector.WherePasses(filter).ToElements();

                    if (elements == null || elements.Count() == 0)
                    {
                        return false;
                    }
                    foreach (Element el in elements)
                    {
                        Material mate = el as Material;
                        if (mate.Name == name)
                        {
                            sameflg = true;
                            eid = mate.Id;
                        }
                    }
                    //for (int i = 0; i < RevitLNK.LoFa.RevitMatName.Count(); i++)
                    //{
                    //    if (RevitLNK.LoFa.RevitMatName[i].Name == name)
                    //    {
                    //        sameflg = true;
                    //        eid = RevitLNK.LoFa.RevitMatName[i].Id;                            
                    //        break;
                    //    }
                    //}
                    break;
                case "コンクリート":
                    basemat = RevitLNK.LoFa.RevitConcName[0];
                    for (int i = 0; i < RevitLNK.LoFa.RevitConcName.Count(); i++)
                    {
                        if (RevitLNK.LoFa.RevitConcName[i].Name == name)
                        {
                            sameflg = true;
                            eid = RevitLNK.LoFa.RevitConcName[i].Id;
                            break;
                        }
                    }

                    break;
            }

            if (!sameflg && name != "")
            {
                eid = Material.Create(Commons.doc, name);
                Material mat = (Material)Commons.doc.GetElement(eid);
                mat.MaterialCategory = cate;
                mat.MaterialClass = cate;
                if (basemat != null)
                {
                    mat.Color = basemat.Color;
                    mat.CutForegroundPatternColor = basemat.CutForegroundPatternColor;
                }
                if (cate == "コンクリート")
                {
                    RevitLNK.LoFa.RevitConcName.Add(mat);
                }
                else
                {
                    RevitLNK.LoFa.RevitMatName.Add(mat);
                }
            }
            return true;
        }


        /// <summary>
        /// マテリアルファミリの生成
        /// </summary>
        /// <param name="val"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        internal static bool SetMaterial(ref Object val, ref ElementId eid, bool cftflg = false)
        {
            bool ret = true;
            bool getflg = false;
            string cate = "";
            for (int i = 0; i < RevitLNK.MateData.Count(); i++)
            {
                if (RevitLNK.MateData[i].stbmatName == (string)val)
                {
                    val = (object)RevitLNK.MateData[i].RevitmatName;
                    getflg = true;
                    cate = "メタル";
                    break;
                }
            }
            if (!getflg)
            {
                for (int i = 0; i < RevitLNK.ConcData.Count(); i++)
                {
                    if (cftflg)
                    {
                        if (RevitLNK.ConcData[i].kouzou != "CFT") { continue; }
                    }
                    if (RevitLNK.ConcData[i].STBstrength == (string)val)
                    {
                        val = (object)RevitLNK.ConcData[i].Revitname;
                        cate = "コンクリート";
                        break;
                    }
                }
            }

            //マテリアルの作成
            SearchMaterial((string)val, cate, ref eid);
            return ret;
        }


        /// <summary>
        /// フィート換算が必要なパラメータタイプ
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private static bool ConvFeet(ForgeTypeId datatype)
        {
            if (datatype == SpecTypeId.Length) return true;
            if (datatype == SpecTypeId.SectionProperty) return true;
            if (datatype == SpecTypeId.SectionDimension) return true;
            if (datatype == SpecTypeId.ReinforcementSpacing) return true;
            if (datatype == SpecTypeId.ReinforcementLength) return true;
            if (datatype == SpecTypeId.ReinforcementCover) return true;


            return false;
        }

        /// <summary>
        /// パラメータのセット
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        /// <param name="mm2ftflg">true：mm→feetに直す</param>
        /// <returns></returns>
        internal static bool SetParameter(Parameter p, object val, bool mm2ftflg = false, bool cftflg = false)
        {
            bool ret = true;

            if (p == null)
            {
                ret = false;
                return ret;
            }
            if (p.IsReadOnly == true)
            {
                ret = false;
                return ret;
            }


            try
            {
                var datatype = p.Definition.GetDataType();
                if (datatype == SpecTypeId.Int.Integer)
                {
                    p.Set(Convert.ToInt32(val));
                }
                else if (datatype == SpecTypeId.Boolean.YesNo)
                {
                    int obj = 0;
                    if (Convert.ToBoolean(val) == true)
                    { obj = 1; }
                    p.Set(obj);
                }
                else if (datatype == SpecTypeId.String.Text && p.StorageType == StorageType.String)
                {
                    p.Set(val?.ToString() ?? "");
                }
                else if (ConvFeet(datatype))
                {
                    if (mm2ftflg)
                    { p.Set(Commons.mm2ft(Convert.ToDouble(val))); }
                    else
                    { p.Set(Convert.ToDouble(val)); }
                }
                else if (datatype == SpecTypeId.Number || datatype == SpecTypeId.Angle)
                {
                    p.Set(Convert.ToDouble(val));
                }
                else if (datatype == SpecTypeId.Reference.Material)
                {
                    ElementId eid = val as ElementId;
                    if (eid == null)
                    { SetMaterial(ref val, ref eid, cftflg); }
                    if (eid == null) {
                        eid = ElementId.InvalidElementId ;
                    }
                    p.Set(eid);
                }
                else
                {
                    switch (p.StorageType)
                    {
                        case StorageType.ElementId:
                            ElementId seid = val as ElementId;
                            if (seid != null)
                            {
                                p.Set(seid);
                            }
                            break;
                        case StorageType.Double:
                            p.Set(Convert.ToDouble(val));
                            break;
                        case StorageType.Integer:
                            p.Set(Convert.ToInt32(val));
                            break;
                        case StorageType.String:
                            p.Set(val?.ToString() ?? "");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        /// <summary>
        /// インスタンスパラメータのセット
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="binp"></param>
        /// <param name="val"></param>
        /// <param name="mm2ftflg"></param>
        /// <returns></returns>
        internal static bool SetParameter(FamilyInstance instance, BuiltInParameter binp, Object val, bool mm2ftflg = false)
        {
            bool ret = true;

            Parameter p = instance.get_Parameter(binp);
            if (p == null)
            {
                ret = false;
                return ret;
            }
            else if (p.IsReadOnly)
            {
                ret = false;
                return ret;
            }

            try
            {
                var datatype = p.Definition.GetDataType();
                if (datatype == SpecTypeId.Int.Integer)
                {
                    p.Set(Convert.ToInt32(val));
                }
                else if (datatype == SpecTypeId.Boolean.YesNo)
                {
                    int obj = 0;
                    if (Convert.ToBoolean(val) == true)
                    { obj = 1; }
                    p.Set(obj);
                }
                else if (datatype == SpecTypeId.String.Text && p.StorageType == StorageType.String)
                {
                    p.Set(val?.ToString() ?? "");
                }
                else if (ConvFeet(datatype))
                {
                    if (mm2ftflg)
                    { p.Set(Commons.mm2ft(Convert.ToDouble(val))); }
                    else
                    { p.Set(Convert.ToDouble(val)); }
                }
                else if (datatype == SpecTypeId.Number || datatype == SpecTypeId.Angle)
                {
                    p.Set(Convert.ToDouble(val));
                }
                else if (datatype == SpecTypeId.Reference.Material)
                {
                    ElementId eid = val as ElementId;
                    if (eid == null)
                    { SetMaterial(ref val, ref eid, false); }
                    if (eid == null) {
                        eid = ElementId.InvalidElementId ;
                    }
                    p.Set(eid);
                }
                else
                {
                    switch (p.StorageType)
                    {
                        case StorageType.ElementId:
                            ElementId seid = val as ElementId;
                            if (seid != null)
                            {
                                p.Set(seid);
                            }
                            break;
                        case StorageType.Double:
                            p.Set(Convert.ToDouble(val));
                            break;
                        case StorageType.Integer:
                            p.Set(Convert.ToInt32(val));
                            break;
                        case StorageType.String:
                            p.Set(val?.ToString() ?? "");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        /// <summary>
        /// パラメータが複数あり、Textの方へ値を入れる
        /// </summary>
        /// <param name="paraname"></param>
        /// <param name="set"></param>
        /// <param name="symbol"></param>
        /// <param name="floor"></param>
        /// <param name="wall"></param>
        internal static void Parameter_Select_Set(string paraname, string set, FamilySymbol symbol = null, FloorType floor = null, WallType wall = null)
        {
            IList<Parameter> paras = null;
            if (symbol != null)
            { paras = symbol.GetParameters(paraname); }
            if (floor != null)
            { paras = floor.GetParameters(paraname); }
            if (wall != null)
            { paras = wall.GetParameters(paraname); }
            for (int i = 0; i < paras.Count(); i++)
            {
                if (paras[i].StorageType != StorageType.String) { continue; }
                SetParameter(paras[i], set);
            }
        }



        /// <summary>
        /// オフセットの計算
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="offsetx"></param>
        /// <param name="offsety"></param>
        /// <param name="offsetz"></param>
        /// <returns></returns>
        internal static XYZ TransformCoord(XYZ start, XYZ end, double offsetx, double offsety, double offsetz, double rotate)
        {
            XYZ Puvw = null;

            XYZ vecU = (end - start).Normalize();
            XYZ vecV = new XYZ();
            XYZ vecW = new XYZ();
            Get_Vector(vecU, rotate, ref vecV, ref vecW);

            double u = vecU.X * offsetx + vecU.Y * offsety + vecU.Z * offsetz;
            double v = vecV.X * offsetx + vecV.Y * offsety + vecV.Z * offsetz;
            double w = vecW.X * offsetx + vecW.Y * offsety + vecW.Z * offsetz;
            Puvw = new XYZ(u, v, w);

            return Puvw;
        }

        /// <summary>梁のオフセット方向のベクトルとレベル方向のベクトルを求める
        /// </summary>
        /// <param name="vecU">梁の方向ベクトル</param>
        /// <param name="rotate">断面回転角度（度）</param>
        /// <param name="vecV">梁のオフセット方向ベクトル</param>
        /// <param name="vecW">梁のレベル方向ベクトル</param>
        internal static void Get_Vector(XYZ vecU, double rotate, ref XYZ vecV, ref XYZ vecW)
        {
            XYZ vecV_r = (XYZ.BasisZ.CrossProduct(vecU)).Normalize();
            vecV = new XYZ();
            Commons.AxisRotate(vecV_r, vecU, new XYZ(), rotate, ref vecV);
            vecW = (vecU.CrossProduct(vecV)).Normalize();
        }

        /// <summary>
        /// 部材方向のオフセット
        /// </summary>
        /// <param name="P">全体座標系の座標</param>
        /// <param name="offset">部材座標系での移動量</param>
        /// <param name="vecU">部材座標径での部材方向ベクトル</param>
        /// <param name="z_flg">T:Z座標も考慮</param>
        /// <returns></returns>
        internal static XYZ Set_offset(XYZ P, XYZ offset, XYZ vecU, bool z_flg = false)
        {
            double x = 0, y = 0, z = 0;
            x = P.X + vecU.X * Commons.mm2ft(offset.X);
            y = P.Y + vecU.Y * Commons.mm2ft(offset.X);
            if (z_flg)
            { z = P.Z + vecU.Z * Commons.mm2ft(offset.X); }
            else
            { z = P.Z; }

            return new XYZ(x, y, z);
        }





        internal static bool Search_Same_FoundationFamily(string typename, double B)
        {
            bool ret = false;

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<FamilySymbol> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().ToList();

            if (elements == null || elements.Count() == 0)
            {
                return ret;
            }

            foreach (FamilySymbol symbol in elements)
            {
                if (symbol.Name == typename)
                {
                    Parameter para = symbol.LookupParameter(SetFamily.FConti.t_B); //テーパー_幅
                    if (para != null)
                    {
                        double paraB = para.AsDouble();
                        if (ConvFeet(para.Definition.GetDataType()))
                        {
                            paraB = Commons.ft2mm(paraB);
                        }

                        if (Math.Abs(B - paraB) > 1)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            return ret;
        }



        /// <summary>
        /// プロジェクト情報のSTB基点位置設定を取得する
        /// </summary>
        internal static void ReadKiten()
        {
            //プロジェクト情報の読み込み
            ProjectInfo pinfo = Commons.doc.ProjectInformation;
            Parameter p = pinfo.LookupParameter("STB基点位置設定");
            if (p != null)
            {
                string str = p.AsString();
                string[] split;
                string[] jouken = { "," };
                int spnum = 0;
                if (str != null && str != "")
                {
                    split = str.Split(jouken, StringSplitOptions.None);

                    ConvertForm.LMD.flg = true;
                    int.TryParse(split[spnum], out ConvertForm.LMD.rdb);
                    spnum++;
                    ConvertForm.LMD.STB_X = split[spnum];
                    spnum++;
                    ConvertForm.LMD.STB_Y = split[spnum];
                    spnum++;
                    ConvertForm.LMD.RVT_X = split[spnum];
                    spnum++;
                    ConvertForm.LMD.RVT_Y = split[spnum];
                    spnum++;
                    double.TryParse(split[spnum], out ConvertForm.LMD.Offset_X1);
                    spnum++;
                    double.TryParse(split[spnum], out ConvertForm.LMD.Offset_Y1);
                    spnum++;
                    double.TryParse(split[spnum], out ConvertForm.LMD.Offset_X2);
                    spnum++;
                    double.TryParse(split[spnum], out ConvertForm.LMD.Offset_Y2);
                }
            }
        }








        #region Export


        /// <summary>
        /// 杭長の出力設定
        /// </summary>
        internal enum ExportPileSetting
        {
            input = 0,
            none,
        }
        internal static ExportPileSetting pileSetting = ExportPileSetting.input;



        internal class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            public bool Equals(XYZ a, XYZ b)
            {
                return (a.DistanceTo(b) < 0.0001);
            }

            public int GetHashCode(XYZ a)
            {
                return a.ToString().GetHashCode();
            }
        }


        /// <summary>
        /// 軸に所属する節点判別用に情報を格納する
        /// </summary>
        internal class GridInformation
        {
            internal int stb_id = 0;
            internal Grid gr = null;
            internal ElementId multiGridID = null;
            internal XYZ ps = null;
            internal XYZ pe = null;
        }


        /// <summary>
        /// 杭長0_Flagの取得
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal static bool GetPile0Length(FamilySymbol symbol)
        {
            string familyname = symbol.Family.Name;
            string pname = "";
            if (familyname == SetFamily.CastinPile.FamilyName)
            {
                pname = SetFamily.CastinPile.zeroLength;
            }
            else if (familyname == SetFamily.PrecastPile.FamilyName)
            {
                pname = SetFamily.PrecastPile.zeroLength;
            }

            return GetParameter_bool(symbol, pname);
        }

        /// <summary>
        /// Import時に「杭長0_Flag」ONとした杭があるか探す
        /// </summary>
        /// <returns>=1:あり</returns>
        internal static int Check_PileZeroLength()
        {
            List<string> PileFamilyName = SetFamily.FoFName.FamilyName.Last().Where(x => x != "").ToList();

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<FamilyInstance> piles = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements().OfType<FamilyInstance>().Where(x => PileFamilyName.Contains(x.Symbol.Family.Name)).ToList();

            collector = new FilteredElementCollector(Commons.doc);
            List<FamilySymbol> pileSymbols = collector.WherePasses(filter).WhereElementIsElementType().ToElements().OfType<FamilySymbol>().Where(x => PileFamilyName.Contains(x.Family.Name)).ToList();


            for (int i = 0; i < pileSymbols.Count; ++i)
            {
                //使われていないタイプは除外
                if (!piles.Any(x => x.Symbol.Id == pileSymbols[i].Id)) continue;

                if (GetPile0Length(pileSymbols[i]))
                {
                    return 1;
                }
            }

            return 0;
        }


        #region パラメータ取得

        internal static string GetParameter_string(Element elm, string n)
        {
            Parameter p = elm.LookupParameter(n ?? "");
            if (p != null)
            {
                switch (p.StorageType)
                {
                    case StorageType.String:
                        return p.AsString() ?? "";
                    case StorageType.Integer:
                        return p.AsInteger().ToString();
                    case StorageType.Double:
                        return p.AsDouble().ToString();
                    case StorageType.ElementId:
                        ElementId eid = p.AsElementId();
                        if (eid != null)
                        {
                            return Commons.doc.GetElement(eid)?.Name ?? "";
                        }
                        break;
                }

                return p.AsString() ?? "";
            }

            return "";
        }
        internal static string GetParameter_string(Element elm, BuiltInParameter bip)
        {
            Parameter p = elm.get_Parameter(bip);
            if (p != null)
            {
                switch (p.StorageType)
                {
                    case StorageType.String:
                        return p.AsString() ?? "";
                    case StorageType.Integer:
                        return p.AsInteger().ToString();
                    case StorageType.Double:
                        return p.AsDouble().ToString();
                    case StorageType.ElementId:
                        ElementId eid = p.AsElementId();
                        if (eid != null)
                        {
                            return Commons.doc.GetElement(eid)?.Name ?? "";
                        }
                        break;
                }

                return p.AsString() ?? "";
            }

            return "";
        }
        internal static double GetParameter_double(Element elm, string n, bool ft = false)
        {
            Parameter p = elm.LookupParameter(n ?? "");
            if (p != null)
            {
                var datatype = p.Definition.GetDataType();
                if (ConvFeet(datatype))
                {
                    if (ft)
                    {
                        return p.AsDouble();
                    }
                    else
                    {
                        return Commons.ft2mm(p.AsDouble(), 0, 3);
                    }
                }
                else if (datatype == SpecTypeId.String.Text)
                {
                    string s = p.AsString();
                    double.TryParse(s, out double d);
                    return d;
                }
                else if (datatype == SpecTypeId.Int.Integer)
                {
                    return p.AsInteger();
                }

                return p.AsDouble();
            }

            return 0;
        }
        internal static double GetParameter_double(Element elm, BuiltInParameter bip, bool ft = false)
        {
            Parameter p = elm.get_Parameter(bip);
            if (p != null)
            {
                var datatype = p.Definition.GetDataType();
                if (ConvFeet(datatype))
                {
                    if (ft)
                    {
                        return p.AsDouble();
                    }
                    else
                    {
                        return Commons.ft2mm(p.AsDouble(), 0, 3);
                    }
                }
                else if (datatype == SpecTypeId.String.Text)
                {
                    string s = p.AsString();
                    double.TryParse(s, out double d);
                    return d;
                }
                else if (datatype == SpecTypeId.Int.Integer)
                {
                    return p.AsInteger();
                }

                return p.AsDouble();
            }

            return 0;
        }
        internal static int GetParameter_int(Element elm, string n)
        {
            Parameter p = elm.LookupParameter(n ?? "");
            if (p != null)
            {
                var datatype = p.Definition.GetDataType();
                if (datatype == SpecTypeId.String.Text)
                {
                    string s = p.AsString();
                    int.TryParse(s, out int d);
                    return d;
                }
                else if (datatype == SpecTypeId.Int.Integer)
                {
                    return p.AsInteger();
                }
            }

            return 0;
        }
        internal static int GetParameter_int(Element elm, BuiltInParameter bip)
        {
            Parameter p = elm.get_Parameter(bip);
            if (p != null)
            {
                return p.AsInteger();
            }

            return 0;
        }
        internal static bool GetParameter_bool(Element elm, string n)
        {
            Parameter p = elm.LookupParameter(n ?? "");
            if (p != null)
            {
                var datatype = p.Definition.GetDataType();
                if (datatype == SpecTypeId.Int.Integer || datatype == SpecTypeId.Boolean.YesNo)
                {
                    return p.AsInteger() == 1;
                }
                else if (datatype == SpecTypeId.String.Text)
                {
                    return p.AsString().ToUpper() == "TRUE";
                }
            }

            return false;
        }


        /// <summary>
        /// 鉄筋径パラメータの取得
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="d1">径1パラメータ名</param>
        /// <param name="d2">径2パラメータ名</param>
        /// <returns>径1径2(D10D13)</returns>
        internal static string GetParameter_D(Element elm, string d1, string d2)
        {
            return GetParameter_string(elm, d1) + GetParameter_string(elm, d2);
        }

        /// <summary>
        /// 角度パラメータの取得
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="bip"></param>
        /// <returns>0～360°</returns>
        internal static double GetParameter_Angle(Element elm, BuiltInParameter bip)
        {
            double angle = -(GetParameter_double(elm, bip) * 180 / Math.PI);
            while (angle < 0)
            {
                angle += 360;
            }
            while (360 < angle)
            {
                angle -= 360;
            }

            return angle;
        }

        #endregion



        #region ログ関連


        internal enum LogCode : int
        {
            grid = 0,
            level,
            column,
            girder,
            beam,
            wall,
            slab,
            brace,
            footing,
            pile,
        }

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="code"></param>
        /// <param name="elm"></param>
        /// <param name="stbid">STBの配置ID</param>
        /// <param name="stbsecid">STBの断面ID</param>
        internal static void AddLog(LogCode code, Element elm, int stbid, int stbsecid)
        {
            string msg = "";

            FamilyInstance ins = elm as FamilyInstance;
            Wall wal = elm as Wall;
            Floor sla = elm as Floor;

            const string RID = "Revit ID:";
            const string SID = " → STB ID:";

            switch (code)
            {
                case LogCode.grid:
                    msg = "[通芯] ";
                    msg += RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    break;
                case LogCode.level:
                    msg = "[レベル] ";
                    msg += RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    break;
                case LogCode.column:
                    msg = "[柱] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.girder:
                    msg = "[大梁] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.beam:
                    msg = "[小梁] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.wall:
                    msg = "[壁] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + wal.WallType.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.slab:
                    msg = "[床] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + sla.FloorType.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.brace:
                    msg = "[ブレース] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.footing:
                    msg = "[基礎] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
                case LogCode.pile:
                    msg = "[杭] ";
                    msg += "(配置)" + RID + elm.Id.Value().ToString() + SID + stbid.ToString();
                    msg += " / ";
                    msg += "(断面)" + RID + ins.Symbol.Id.Value().ToString() + SID + stbsecid.ToString();
                    break;
            }

            if (msg != "")
            {
                LogData.AddLog(LogData.LogKind.Infmoation, 0, msg);
            }
        }
        internal static void AddWarning(int code, Element elm)
        {
            const string RID = "Revit ID:";
            string msg = RID + elm.Id.Value().ToString();

            switch (code)
            {
                case -1:
                    msg += " 座標が取得できないので変換できません";
                    break;
                case -2:
                    msg += " 解析モデルが無効のため変換できません";
                    break;
                case -3:
                    msg += " 符号が空欄のため変換できません";
                    break;
                case -4:
                    msg += " 解析線分の頂点数と躯体の頂点数が一致しないため変換できません";
                    break;
            }

            if (msg != "")
            {
                LogData.AddLog(LogData.LogKind.Warning, 0, msg);
            }
        }


        #endregion



        /// <summary>
        /// コンクリート - FcXX 等のマテリアル名からFcXXのみを取り出す
        /// </summary>
        /// <param name="s">マテリアル名称</param>
        /// <returns></returns>
        internal static string GetConcreteFC(string s)
        {
            const string conc_pattern = "(Fc|FC|Lc|LC)[0-9]+";
            const string conc_pattern_rep1 = "(^.*)(" + conc_pattern + ")(.*$)";
            const string conc_pattern_rep2 = "$2";

            string FC = "";
            if (Regex.IsMatch(s, conc_pattern))
            {
                FC = Regex.Replace(s, conc_pattern_rep1, conc_pattern_rep2);
            }

            return FC;
        }


        /// <summary>
        /// 構造フレームの座標を取得
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="SE">0:始端, 1:終端</param>
        /// <returns></returns>
        internal static XYZ GetFramingCoordinate(FamilyInstance ins, int SE)
        {
            LocationCurve locC = ins.Location as LocationCurve;
            XYZ p = locC.Curve.GetEndPoint(SE);

            int yz_justification = GetParameter_int(ins, BuiltInParameter.YZ_JUSTIFICATION);
            double y = 0;
            double z = 0;
            switch (yz_justification)
            {
                case (int)YZJustificationOption.Uniform: //同一
                    y = GetParameter_double(ins, BuiltInParameter.Y_OFFSET_VALUE, true);
                    z = GetParameter_double(ins, BuiltInParameter.Z_OFFSET_VALUE, true);

                    //躯体に平行に動くので、躯体の持つベクトル方向に加算
                    p = p + ins.FacingOrientation * y;
                    p = p + ins.HandOrientation.CrossProduct(ins.FacingOrientation).Normalize() * z;
                    break;

                case (int)YZJustificationOption.Independent: //個別
                    y = GetParameter_double(ins, (SE == 0 ? BuiltInParameter.START_Y_OFFSET_VALUE : BuiltInParameter.END_Y_OFFSET_VALUE), true);
                    z = GetParameter_double(ins, (SE == 0 ? BuiltInParameter.START_Z_OFFSET_VALUE : BuiltInParameter.END_Z_OFFSET_VALUE), true);

                    //各点がLocationCurveに直行する方向に動く（躯体の持つベクトルは動いたあとの状態でのベクトルなので使えない）
                    XYZ v1 = (locC.Curve.GetEndPoint(1) - locC.Curve.GetEndPoint(0)).Normalize();
                    XYZ v2 = XYZ.BasisZ.CrossProduct(v1).Normalize();
                    p = p + v2 * y;
                    p = p + v1.CrossProduct(v2).Normalize() * z;
                    break;
            }


            return p;
        }



        /// <summary>
        /// 解析モデルが有効であるかチェックする
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal static bool Check_Analytical_Model(Element element)
        {
            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
            return amanager.HasAssociation(element.Id);
        }


        /// <summary>
        /// 床の外周座標の取得
        /// </summary>
        /// <param name="s">床</param>
        /// <param name="op">開口</param>
        /// <returns>座標[mm]</returns>
        internal static List<XYZ> GetSlabCoord(Floor s, List<Opening> op)
        {
            List<XYZ> points = new List<XYZ>();

            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
            if (Commons.doc.GetElement(amanager.GetAssociatedElementId(s.Id)) is AnalyticalPanel panel)
            {
                foreach (Line line in panel.GetOuterContour())
                {
                    points.Add(line.GetEndPoint(0));
                }
            }
            else
            {
                return new List<XYZ>();
            }

            XYZ pp = XYZ.Zero;
            double mindist = points.Min(x => x.DistanceTo(pp));
            int index = points.FindIndex(x => Math.Abs(x.DistanceTo(pp) - mindist) < 0.001);
            index = Math.Max(index, 0);

            bool reverse = Commons.CalcMenseki(points) < 0;

            List<XYZ> points2 = new List<XYZ>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                int ii = i + index;
                if (ii >= points.Count) ii = ii - points.Count;

                //床は傾いているのに、解析線分が水平、というケースがあるので、床上面の座標を取得する。
                XYZ p = s.GetVerticalProjectionPoint(points[ii], FloorFace.Top);
                if (p == null) p = points[ii];

                p = Commons.ft2mm(p);
                if (i == 0 || !reverse)
                {
                    points2.Add(p);
                }
                else
                {
                    points2.Insert(1, p);
                }
            }

            return points2;
        }

        /// <summary>
        /// 床の外周座標の取得[mm]
        /// </summary>
        /// <param name="s"></param>
        /// <returns>1:解析線分, 2:躯体</returns>
        internal static (List<XYZ>, List<XYZ>) GetSlabCoord2(Floor s)
        {
            List<XYZ> points1 = new List<XYZ>();
            List<XYZ> points2 = new List<XYZ>();

            var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
            if (Commons.doc.GetElement(amanager.GetAssociatedElementId(s.Id)) is AnalyticalPanel panel)
            {
                foreach (var item in panel.GetOuterContour())
                {
                    points1.Add(Commons.ft2mm(item.GetEndPoint(0)));
                    if(item is Arc arc) Console.WriteLine($"Id:{arc.Id}");
                }
            }

            if (Commons.doc.GetElement(s.SketchId) is Sketch sketch)
            {
                foreach (CurveArray curveArray in sketch.Profile)
                {
                    foreach (Curve curve in curveArray)
                    {
                        points2.Add(curve.GetEndPoint(0));
                    }
                    break;
                }
                if (points2.Count > 0)
                {
                    points2 = points2.Select(a => s.GetVerticalProjectionPoint(a, FloorFace.Top) ?? a).ToList();
                    points2 = points2.Select(a => Commons.ft2mm(a)).ToList();
                }
            }

            //Deleteすると解析線分とのリンクが切れる。RollBackしても戻らない。削除しなくてもSketchIdが取れるようになった。
            /*
            SubTransaction st = new SubTransaction(Commons.doc);
            st.Start();
            var ids = Commons.doc.Delete(s.Id);
            st.RollBack();

            double offset = s.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();

            List<ModelLine> mLine = new List<ModelLine>();
            ModelLine slope = null;
            foreach (var id in ids)
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

            if (slope != null)
            {
                var p1 = slope.get_Parameter(BuiltInParameter.SPECIFY_SLOPE_OR_OFFSET);
                var p2 = slope.get_Parameter(BuiltInParameter.SLOPE_START_HEIGHT); //矢尻 レベル オフセット
                var p3 = slope.get_Parameter(BuiltInParameter.SLOPE_END_HEIGHT);   //矢先 レベル オフセット
                XYZ normal = new XYZ(0, 0, 1);

                XYZ slope_pos0 = slope.GeometryCurve.GetEndPoint(0) + new XYZ(0, 0, p2.AsDouble());
                XYZ slope_pos1 = slope.GeometryCurve.GetEndPoint(1) + new XYZ(0, 0, p3.AsDouble());

                if (p1.AsInteger() == 1)
                {
                    //勾配
                    double tan = slope.get_Parameter(BuiltInParameter.ROOF_SLOPE).AsDouble();
                    double rad = Math.Atan(tan);
                    double deg = rad * 180 / Math.PI;

                    XYZ pos0 = slope.GeometryCurve.GetEndPoint(0);
                    XYZ pos1 = slope.GeometryCurve.GetEndPoint(1);
                    XYZ vec1 = (pos1 - pos0).Normalize();
                    XYZ vec2 = new XYZ();
                    XYZ vec3 = new XYZ(0, 0, 1).CrossProduct(vec1).Normalize();
                    Commons.AxisRotate(vec1, new XYZ(), vec3, -deg, ref vec2);

                    normal = vec2.CrossProduct(vec3).Normalize();
                }
                else
                {
                    //端部
                    XYZ v = new XYZ(0, 0, 1).CrossProduct(slope_pos1 - slope_pos0).Normalize();
                    normal = (slope_pos1 - slope_pos0).CrossProduct(v).Normalize();
                }

                double kd = -normal.DotProduct(slope_pos0);

                foreach (var m in mLine)
                {
                    XYZ pos = m.GeometryCurve.GetEndPoint(0);
                    double z = -(normal.X * pos.X + normal.Y * pos.Y + kd) / normal.Z;

                    XYZ pos2 = Commons.ft2mm(new XYZ(pos.X, pos.Y, z + offset));
                    points2.Add(pos2);
                }
            }
            else
            {
                foreach (var m in mLine)
                {
                    XYZ pos = m.GeometryCurve.GetEndPoint(0);
                    XYZ pos2 = Commons.ft2mm(new XYZ(pos.X, pos.Y, pos.Z + offset));
                    points2.Add(pos2);
                }
            }
            //*/

            if (points1.Count == 0)
            {
                //解析線分がないときは躯体と同じ扱い
                points1.AddRange(points2);
            }
            else if (points1.Count != points2.Count)
            {
                //解析線分の頂点数と躯体の頂点数が不一致のときは出力しない
                return (null, null);
            }


            SortPoints(ref points1);
            SortPoints(ref points2);


            return (points1, points2);
        }

        /// <summary>
        /// 床座標のソート
        /// </summary>
        /// <param name="points"></param>
        private static void SortPoints(ref List<XYZ> points)
        {
            if (points.Count == 0) return;

            if (Commons.CalcMenseki(points) < 0)
            {
                points.Reverse();
            }

            double x = points.Min(a => a.X);
            double y = points.Min(a => a.Y);
            double z = points.Min(a => a.Z);

            XYZ min = new XYZ(x, y, z);
            double dist = min.DistanceTo(points[0]);
            int index = 0;
            for (int i = 1; i < points.Count; ++i)
            {
                double d = min.DistanceTo(points[i]);
                if (d < dist)
                {
                    dist = d;
                    index = i;
                }
            }

            if (index > 0)
            {
                var p2 = points.Take(index);
                points = points.Skip(index).ToList();
                points.AddRange(p2);
            }
        }


        #endregion






        /// <summary>
        /// UniqueID（ハイフン除いて40桁） を Guid に変換
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        internal static Guid Convertguid(string uniqueID)
        {
            //Guid episodeId = new Guid(uniqueID.Substring(0, 36));

            //Revit UniqueID
            //8桁-4桁-4桁-4桁-12桁-8桁
            //"Guid              "-elementID("x8") 

            //末尾の8桁からelementidを取得
            int elementId = int.Parse(uniqueID.Substring(37), System.Globalization.NumberStyles.AllowHexSpecifier);

            //12桁部分の後ろ8桁を取得
            int last_32_bits = int.Parse(uniqueID.Substring(28, 8), System.Globalization.NumberStyles.AllowHexSpecifier);

            int xor = last_32_bits ^ elementId;

            //12桁部分の後ろ8桁をxorしたものに置き換える
            var uniqueID2 = uniqueID.Substring(0, 28) + xor.ToString("x8");


            return new Guid(uniqueID2);
        }

        /// <summary>
        /// Guid を UniqueID に変換
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        internal static string ConvertUniqueID(string guid, ElementId eid)
        {
            List<string> g2 = new List<string>(6);
            if (guid.Contains("-"))
            {
                //ハイフンで分割
                g2 = guid.Split('-').ToList();
            }
            else
            {
                //各桁ごとに分割
                g2.Add(guid.Substring(0, 8));
                g2.Add(guid.Substring(8, 4));
                g2.Add(guid.Substring(12, 4));
                g2.Add(guid.Substring(16, 4));
                g2.Add(guid.Substring(20));
            }

            //12桁部分の後ろ8桁を取得
            var last_32_bits = long.Parse(g2.Last().Substring(4), System.Globalization.NumberStyles.AllowHexSpecifier);
            var xor = last_32_bits ^ eid.Value();

            //12桁部分の後ろ8桁をxorしたものに置き換える
            g2[g2.Count - 1] = g2.Last().Substring(0, 4) + xor.ToString("x8");

            //elementidを追加
            g2.Add(eid.Value().ToString("x8"));

            return string.Join("-", g2);
        }


        /// <summary>
        /// Schemaの取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static Schema GetSchema(string name)
        {
            var schema1 = Schema.ListSchemas().Where(a => a.SchemaName == name);

            //Revit起動してから開いたプロジェクトにあるスキーマが全部取れるみたい
            //このプロジェクトのものか（編集できるか）チェックする
            foreach (var s in schema1)
            {
                var entity = Commons.doc.ProjectInformation.GetEntity(s);
                if (entity != null)
                {
                    if (entity.IsValid() && entity.ReadAccessGranted())
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Guid を拡張ストレージに保存する
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="eid"></param>
        internal static void SaveGuid(string guid, ElementId eid)
        {
            if (guid == null || guid.Trim() == "") return;
            if (eid == null) return;
            if (eid.Value() == -1) return;



            var schema1 = Data.GetSchema(Data.schemaName_Guid);
            Entity entity = null;

            if (schema1 == null)
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(Guid.NewGuid());
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetSchemaName(Data.schemaName_Guid);

                //field作成
                schemaBuilder.AddMapField(FieldName, typeof(ElementId), typeof(string));

                schema1 = schemaBuilder.Finish();

                entity = new Entity(schema1);
            }
            else
            {
                entity = Commons.doc.ProjectInformation.GetEntity(schema1);
            }


            var field = schema1.GetField(FieldName);

            var data1 = entity.Get<IDictionary<ElementId, string>>(field);
            if (data1.ContainsKey(eid))
            {
                //既に存在している場合は上書き
                data1[eid] = guid.Trim();
            }
            else
            {
                data1.Add(eid, guid.Trim());
            }

            entity.Set(field, data1);

            //確認用
            //var data2 = entity.Get<IDictionary<ElementId, string>>(field);
            //foreach (var d in data2)
            //{
            //}


            //プロジェクト情報にセット
            Commons.doc.ProjectInformation.SetEntity(entity);
        }

        /// <summary>
        /// 拡張ストレージに保持されているGuidを取得する
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        internal static string GetStorageGuid(ElementId eid)
        {
            if (eid == null) return "";
            if (eid.Value() == -1) return "";

            var schema1 = Data.GetSchema(Data.schemaName_Guid);
            if (schema1 == null) return "";

            Entity entity = Commons.doc.ProjectInformation.GetEntity(schema1);
            var field = schema1.GetField(FieldName);
            if (field == null) return "";
            if (!entity.RecognizedField(field)) return "";

            var data1 = entity.Get<IDictionary<ElementId, string>>(field);

            if (data1.ContainsKey(eid))
            {
                return data1[eid];
            }

            return "";
        }

        /// <summary>
        /// 拡張ストレージに保持されているElementIdを取得する
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static List<ElementId> GetStorageElementId(string guid)
        {
            if (guid == null || guid == "") return null;

            var schema1 = Data.GetSchema(Data.schemaName_Guid);
            if (schema1 == null) return null;

            Entity entity = Commons.doc.ProjectInformation.GetEntity(schema1);
            var field = schema1.GetField(FieldName);
            if (field == null) return null;
            if (!entity.RecognizedField(field)) return null;

            var data1 = entity.Get<IDictionary<ElementId, string>>(field);

            List<ElementId> id = new List<ElementId>();
            foreach (var d in data1)
            {
                if (d.Value == guid)
                {
                    id.Add(d.Key);
                }
            }

            if (id.Count == 0)
            {
                return null;
            }
            else
            {
                return id;
            }
        }

        /// <summary>
        /// 拡張ストレージに登録されているIdで既に削除されたものを取り除く
        /// </summary>
        internal static void DeleteStoageElementId()
        {
            var schema1 = Data.GetSchema(Data.schemaName_Guid);
            if (schema1 == null) return;

            Entity entity = Commons.doc.ProjectInformation.GetEntity(schema1);
            var field = schema1.GetField(FieldName);
            if (field == null) return;
            if (!entity.RecognizedField(field)) return;

            var data1 = entity.Get<IDictionary<ElementId, string>>(field);

            //対象の要素が消えると、拡張ストレージに登録されているIDも消えているみたい？
            //確証はないので一応チェックする
            List<ElementId> del_id = new List<ElementId>();
            foreach (var d in data1)
            {
                if (Commons.doc.GetElement(d.Key) == null)
                {
                    //ないデータなのでストレージから削除
                    del_id.Add(d.Key);
                }
            }

            if (del_id.Count > 0)
            {
                foreach (var id in del_id)
                {
                    data1.Remove(id);
                }

                entity.Set(field, data1);

                //プロジェクト情報にセット
                Commons.doc.ProjectInformation.SetEntity(entity);
            }
        }

    }
}


internal static class Extention
{
    internal static double ToDouble(this string s)
    {
        double.TryParse(s, out double d);
        return d;
    }
    internal static int ToInt(this string s)
    {
        int.TryParse(s, out int d);
        return d;
    }
    internal static bool ToBool(this string s)
    {
        bool.TryParse(s, out bool d);
        return d;
    }
}



