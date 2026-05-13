using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using Autodesk;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using static STBLink.Data;
using System.Security.Cryptography;

namespace STBLink
{
    class fromSTB
    {
        private System.Windows.Forms.Form form;
        private XYZ ConvOrigin = new XYZ();
        private XYZ BasisX = new XYZ(1, 0, 0);
        private XYZ BasisY = new XYZ(0, 1, 0);
        private XYZ BasisZ = new XYZ(0, 0, 1);
        private double alloffsetX = 0;
        private double alloffsetY = 0;
        private List<Level> Levels = new List<Level>();
        private const double gosa = 0.001;

       
        

        private List<BaseClass> BClm = new List<BaseClass>();


        private List<OffsetZ> alloffsetZ = new List<OffsetZ>();


        private List<ReNameSymbols> GirderSymbols = new List<ReNameSymbols>();
        private List<ReNameSymbols> FContiSymbols = new List<ReNameSymbols>();
        private List<ReNameSymbols> PilesSymbols = new List<ReNameSymbols>();



        private List<IsOutin_Girder> isOutin_G = new List<IsOutin_Girder>();

        /// <summary>基礎・杭グループ化
        /// </summary>
        private Dictionary<int, List<ElementId>> FGroup = new Dictionary<int, List<ElementId>>();

        private List<CGroup> CGrp = new List<CGroup>();

        public bool ShouldOutputCommentDebugLog = false ; 



        public fromSTB(System.Windows.Forms.Form form)
        {
            this.form = form;
            
        }

        ///ProgressBarFormの設定
        private void ProgressBar_Show(ProgressBarForm pform, string labtext)
        {
            gaugeForm = pform;
            pform.lab.Text = labtext;
            pform.lab.Visible = true;
            GaugePositionSet(true, pform.panelFooter.Left, pform.panelFooter.Top, pform.panelFooter.Width, pform.panelFooter.Height);          
            GaugeShow();
        }

        internal void Initialize()
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
        }

        /// <summary> プロジェクト情報への追加
        /// </summary>
        /// <param name="pform"></param>
        /// <param name="labtext"></param>
        internal void AddProjectParameter(STBclass stb, ProgressBarForm pform)
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
               
                foreach (string mappingsParam in projectParams)
                {
                    information = informationCollection.Definitions.get_Item(mappingsParam);

                    if(information == null)
                    {
                        ExternalDefinitionCreationOptions edco = new ExternalDefinitionCreationOptions(mappingsParam, SpecTypeId.String.Text);
                        edco.Visible = true;
                        informationCollection.Definitions.Create(edco);
                        information = informationCollection.Definitions.get_Item(mappingsParam);
                    }

                    //ドキュメントにパラメータを追加
                    Commons.doc.ParameterBindings.Insert(information, binding);
                }

                Commons.doc.Regenerate();

                //プログレスバーの準備
                //Stopwatch stopw = new Stopwatch();
                //stopw.Start();
                ProgressBar_Show(pform, "プロジェクト情報の追加");
                GaugePercent("プロジェクト情報の設定", (int)(0));

                //プロジェクト情報に設定
                ProjectInfo pinfo = Commons.doc.ProjectInformation;
                Parameter p = null;
                for (int i =0; i< projectParams.Count();i++)
                {
                    GaugePercent("プロジェクト情報の設定", (int)((double)i / (double)projectParams.Count * 100));

                    p = pinfo.LookupParameter(projectParams[i]);
                    if(p == null) { continue; }
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
                            p.Set(stb.StbCommon.globalID);
                            break;
                        case 7:
                            p.Set(stb.StbCommon.project_name);
                            break;
                        case 8:
                            p.Set(stb.StbCommon.app_name);
                            break;
                        case 9:
                            p.Set(stb.StbCommon.concrete_strength);
                            break;
                        case 10:
                            p.Set(stb.StbCommon.steel_standard_code);
                            break;
                        case 11:
                            string pset = "";
                            if (stb.StbCommon.StbReinforcement_Strength_List == null) { break; }
                            for (int j = 0; j < stb.StbCommon.StbReinforcement_Strength_List.Count(); j++)
                            {
                                pset += stb.StbCommon.StbReinforcement_Strength_List[j].D + "," + stb.StbCommon.StbReinforcement_Strength_List[j].SD;
                                if (j != stb.StbCommon.StbReinforcement_Strength_List.Count() - 1)
                                {
                                    pset += ",";
                                }
                            }
                            p.Set(pset);
                            break;
                    }
                }               
              
                pform.TopMost = false;
                tran1.Commit();
                pform.TopMost = true;
            }
            catch(Exception)
            {
                tran1.RollBack();
            }
        }

        /// <summary>レベルの生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="Lpare"></param>
        internal void Level_Convert(STBclass stb, ProgressBarForm pform, List<RevitLNK.LevelPare> Lpare)
        {
            Stopwatch stopw = new Stopwatch();
            stopw.Start();
            ProgressBar_Show(pform, "レベルの変換");
            GaugePercent("レベルの生成", (int)(0));

            Transaction tran = new Transaction(Commons.doc, "レベルの生成");
            tran.Start();
           
            try
            {
                //変換情報ログのための変数
                XYZ ps = null, pe = null;
                string stage = "";
                string levelname = "";
               

                //平面図のファミリタイプ
                System.Collections.Generic.IEnumerable<ViewFamilyType> viewFamilyTypes = from elem in new FilteredElementCollector(Commons.doc).OfClass(typeof(ViewFamilyType)) let type = elem as ViewFamilyType where type.ViewFamily == ViewFamily.StructuralPlan select type;
                for (int i = 0; i < Lpare.Count(); i++)
                {
                    bool logflg = true;
                    GaugePercent("レベルの生成", (int)((double)i/(double)Lpare.Count * 100));

                    for (int  s = 0; s< stb.StbModel.StbStories.Count(); s++)
                    {
                        STBclass.StbModelClass.StbStory story = stb.StbModel.StbStories[s];
                        if(Lpare[i].stbStrory == story.name)
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
                                            if(lv.Name == story.name)
                                            {
                                                lv.Name = lv.Name + "_";
                                                break;
                                            }
                                        }
                                    }
                                    Level newlev = Level.Create(Commons.doc, Commons.mm2ft(story.height + Lpare[i].offset));
                                    newlev.Name = story.name;
                                    Levels.Add(newlev);
                                    //平面図の生成
                                    ViewPlan newvp = ViewPlan.Create(Commons.doc, viewFamilyTypes.First().Id, newlev.Id);

                                    
                                    ps = newlev.Elevation * XYZ.BasisZ;
                                    pe = null;
                                    stage = "レベルの新規生成：";
                                    levelname = newlev.Name;
                                    if (Lpare[i].offset != 0)
                                    {
                                        OffsetZ newz = new OffsetZ();
                                        newz.lev = newlev;
                                        newz.offset = Lpare[i].offset;
                                        newz.stbid = s;
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

                                            ps = lv.Elevation * XYZ.BasisZ;
                                            pe = null;
                                            stage = "レベルの生成：";
                                            levelname = story.name + "→" + lv.Name;
                                            if (Lpare[i].offset != 0)
                                            {
                                                OffsetZ newz = new OffsetZ
                                                {
                                                    lev = lv,
                                                    offset = Lpare[i].offset,
                                                    stbid = s
                                                };
                                                alloffsetZ.Add(newz);
                                            }


                                            bool vplanaddflg = true;
                                            for (int vp = 0; vp < RevitLNK.LoFa.VPlan.Count(); vp++)
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
                                MakeGridLog(stage, levelname, ps, pe, 0);
                            }
                            break;                           
                        }
                    }
                }
              
                // 進捗ゲージの消去
                if (form != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    pform.lab.Visible = false;
                    GaugeClose();
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();               
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception e)
            {
                e.ToString();
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }

            
        }

        /// <summary>軸の生成(条件設定) 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="radb1"></param>
        /// <param name="radb2"></param>
        /// <param name="pform"></param>
        internal void Kiten_Convert(STBclass stb, ProgressBarForm pform, bool radb1, bool radb2, RevitLNK.AxisPare XPare, RevitLNK.AxisPare YPare)
        {
            double hoseiZ_min = Commons.mm2ft(5000);
            double hoseiZ_max = Commons.mm2ft(2000);
            Stopwatch stopw = new Stopwatch();
            stopw.Start();
            ProgressBar_Show(pform, "軸の生成");

            try
            {
                if (radb1)
                {
                    double kitenX = 0, kitenY = 0;
                    double revitX = 0, revitY = 0;
                    for(int i = 0; i < RevitLNK.LoFa.GridX.Count(); i++)
                    {
                        if(RevitLNK.LoFa.GridX[i].Name == XPare.RevitGrid)
                        {
                            revitX = Commons.ft2mm(RevitLNK.LoFa.GridX[i].Curve.GetEndPoint(0).X);
                            break;
                        }
                    }
                    for (int i = 0; i < RevitLNK.LoFa.GridY.Count(); i++)
                    {
                        if (RevitLNK.LoFa.GridY[i].Name == YPare.RevitGrid)
                        {
                            revitY = Commons.ft2mm(RevitLNK.LoFa.GridY[i].Curve.GetEndPoint(0).Y);
                            break;
                        }
                    }
                    for(int i = 0; i < stb.StbModel.StbAxes.StbX_Axis.Count();i++)
                    {
                        if(stb.StbModel.StbAxes.StbX_Axis[i].name == XPare.stbAxis)
                        {
                            kitenX = revitX - stb.StbModel.StbAxes.StbX_Axis[i].distance;
                            break;
                        }
                    }
                    for (int i = 0; i < stb.StbModel.StbAxes.StbY_Axis.Count(); i++)
                    {
                        if (stb.StbModel.StbAxes.StbY_Axis[i].name == YPare.stbAxis)
                        {
                            kitenY = revitY - stb.StbModel.StbAxes.StbY_Axis[i].distance;
                            break;
                        }
                    }
                   if(! Kiten_Convert_XY(stb, XPare.offset, YPare.offset, pform, kitenX, kitenY))
                    {
                        LogData.AddLog(LogData.LogKind.Error, 0, "軸の生成");
                    }
                }
                else
                {
                    //元々ある通り芯の削除
                    //Delete_Grid();
                    Kiten_Convert_XY(stb, XPare.offset, YPare.offset, pform);                   
                }

             
                
                // 進捗ゲージの消去
                if (form != null)
                {
                    do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                    stopw.Stop();
                    GaugeClose();

                }
            }
            catch (Exception e)
            {
                e.ToString();
                return;
            }
            
        }

        /// <summary>軸の生成(実際の処理)
        /// 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="kitenX"></param>
        /// <param name="kitenY"></param>
        private bool Kiten_Convert_XY(STBclass stb, double offsetX, double offsetY, ProgressBarForm pform, double kitenX = 0, double kitenY = 0)
        {
            bool ret = true;
            double entyou = Commons.mm2ft(3000); //グリッドを延長する（始点側)
            string logname = ""; //ログ出力用

            //建物全体の移動距離         
            alloffsetX = offsetX + kitenX;
            alloffsetY = offsetY + kitenY;

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

            //通り芯の名前が重複しないように
            List<string> name = new List<string>();

            STBclass.StbModelClass.StbAxesClass stbaxes = stb.StbModel.StbAxes;
            //X軸
            //プロジェクト内のX軸名をリストにしておく
            for (int i = 0; i < RevitLNK.LoFa.GridX.Count(); i++)
            {
                name.Add(RevitLNK.LoFa.GridX[i].Name);
            }

            Transaction tran = new Transaction(Commons.doc, "通り芯の生成");
            try
            {
                tran.Start();
                
                logname = "X軸";
                for (int i = 0; i < stbaxes.StbX_Axis.Count(); i++)
                {
                    STBclass.StbModelClass.StbAxesClass.Stb_Axis axis = stbaxes.StbX_Axis[i];
                    
                    //そもそも節点リストが無い→軸を生成しない
                    if(axis.StbNodeid_List.Count() == 0) { continue; }

                    //プログレスバーの表示
                    GaugePercent("X軸の生成", (int)((double)i / (double)stbaxes.StbX_Axis.Count * 100));

                    List<Curve> cur = new List<Curve>();
                    XYZ beforend = new XYZ();

                    //節点リストを整理する
                    List<STBclass.StbNodeid> newL;
                    if (Commons.GridMode == 0)
                    {
                        newL = Narabekae_Node(stb, axis.StbNodeid_List, "X");
                    }
                    else
                    {
                        //基準距離
                        newL = new List<STBclass.StbNodeid>();
                    }

                    //節点リスト内に該当する節点が無いとき→代表距離をもとに軸を生成する
                    if (newL.Count == 0)
                    {
                        XYZ p0 = new XYZ(Commons.mm2ft(axis.distance + kitenX), 0, 0);
                        XYZ p1;
                        XYZ p2;

                        if (stb.StbModel.StbAxes.StbY_Axis != null &&
                            stb.StbModel.StbAxes.StbY_Axis.Count > 0)
                        {
                            double distance1 = Commons.mm2ft(stb.StbModel.StbAxes.StbY_Axis.Min(a => a.distance));
                            double distance2 = Commons.mm2ft(stb.StbModel.StbAxes.StbY_Axis.Max(a => a.distance));
                            p1 = p0 + new XYZ(0, distance1 - entyou, 0);
                            p2 = p0 + new XYZ(0, distance2 + entyou, 0);
                        }
                        else
                        {
                            p1 = p0 - new XYZ(0, entyou, 0);
                            p2 = p0 + new XYZ(0, entyou, 0);
                        }

                        cur.Add(Line.CreateBound(p1, p2));
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
                                start = Get_Node_Position(stb, newL[0].id, offsetX + kitenX, offsetY + kitenY) - entyou * BasisY;
                                end = Get_Node_Position(stb, newL[0].id, offsetX + kitenX, offsetY + kitenY);

                                double entyouStart = Commons.mm2ft(stbaxes.StbY_Axis[0].distance + kitenY + offsetY) - entyou - start.Y;
                                start = start + entyouStart * BasisY;
                            }
                            else
                            {
                                start = beforend;
                                end = Get_Node_Position(stb, newL[j + 1].id, offsetX + kitenX, offsetY + kitenY);                                
                            }


                            //始点と終点の距離が1mm未満→次の点へ
                            double kyori = Commons.ft2mm(Math.Pow(((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y) + (end.Z - start.Z) * (end.Z - start.Z)), 0.5));
                            if (kyori < 1) { continue; }

                            Curve cr = null;
                            cr = Line.CreateBound(start, end);
                            cur.Add(cr);
                            beforend = end;
                        }

                        //通り芯の最後を延長
                        double entyouend = Commons.mm2ft(stbaxes.StbY_Axis[stbaxes.StbY_Axis.Count() - 1].distance + kitenY + offsetY) + entyou - beforend.Y;
                        Curve cre = Line.CreateBound(beforend, beforend + Math.Max(entyouend, entyou) * BasisY);
                        cur.Add(cre);
                    }


                    CurveLoop cloop = CurveLoop.Create(cur);

                    //作図面
                    XYZ normal = new XYZ(1, 1, 0);
                    XYZ normal2 = new XYZ(0, 0, 1);
                    Plane p = Plane.CreateByNormalAndOrigin(normal2, new XYZ(0, 0, 0));
                    SketchPlane skp = SketchPlane.Create(Commons.doc, p);

                    //複数セグメントの通り芯
                    MultiSegmentGrid mgr = (MultiSegmentGrid)(Commons.doc.GetElement(MultiSegmentGrid.Create(Commons.doc, gt.Id, cloop, skp.Id)));

                    //通り芯の名前の重複チェック
                    if (Name_Check(name, axis.name))
                    {
                        string rename = axis.name;
                        int ascii = 97;
                        do
                        {
                            rename += "_" + (char)ascii;
                            ascii++;

                        } while (Name_Check(name, rename));
                        axis.name = rename;
                    }
                    mgr.Name = axis.name;
                    name.Add(mgr.Name);

                    //通り芯生成ログ
                    MakeGridLog("X軸の生成", mgr.Name, cur[0].GetEndPoint(0), cur[cur.Count - 1].GetEndPoint(1), 1);

                }


                //Y軸
                //プロジェクト内のY軸名をリストにしておく
                for (int i = 0; i < RevitLNK.LoFa.GridY.Count(); i++)
                {
                    name.Add(RevitLNK.LoFa.GridY[i].Name);
                }
                logname = "Y軸";
                for (int i = 0; i < stbaxes.StbY_Axis.Count(); i++)
                {
                    STBclass.StbModelClass.StbAxesClass.Stb_Axis axis = stbaxes.StbY_Axis[i];

                    //そもそも節点リストが無い→軸を生成しない
                    if (axis.StbNodeid_List.Count() == 0) { continue; }

                    GaugePercent("Y軸の生成", (int)((double)i / (double)stbaxes.StbY_Axis.Count * 100));

                    List<Curve> cur = new List<Curve>();
                    XYZ beforend = new XYZ();

                    //節点リストを整理する
                    List<STBclass.StbNodeid> newL;
                    if (Commons.GridMode == 0)
                    {
                        newL = Narabekae_Node(stb, axis.StbNodeid_List, "Y");
                    }
                    else
                    {
                        //基準距離
                        newL = new List<STBclass.StbNodeid>();
                    }

                    //節点リスト内に該当する節点が無いとき→代表距離をもとに軸を生成する
                    if (newL.Count == 0)
                    {
                        XYZ p0 = new XYZ(0, Commons.mm2ft(axis.distance + kitenY), 0);
                        XYZ p1;
                        XYZ p2;

                        if (stb.StbModel.StbAxes.StbX_Axis != null &&
                            stb.StbModel.StbAxes.StbX_Axis.Count > 0)
                        {
                            double distance1 = Commons.mm2ft(stb.StbModel.StbAxes.StbX_Axis.Min(a => a.distance));
                            double distance2 = Commons.mm2ft(stb.StbModel.StbAxes.StbX_Axis.Max(a => a.distance));
                            p1 = p0 + new XYZ(distance1 - entyou, 0, 0);
                            p2 = p0 + new XYZ(distance2 + entyou, 0, 0);
                        }
                        else
                        {
                            p1 = p0 - new XYZ(entyou, 0, 0);
                            p2 = p0 + new XYZ(entyou, 0, 0);
                        }

                        cur.Add(Line.CreateBound(p1, p2));
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
                                start = Get_Node_Position(stb, newL[0].id, offsetX + kitenX, offsetY + kitenY) - entyou * BasisX;
                                end = Get_Node_Position(stb, newL[0].id, offsetX + kitenX, offsetY + kitenY);

                                double entyouStart = Commons.mm2ft(stbaxes.StbX_Axis[0].distance + kitenX + offsetX) - entyou - start.X;
                                start = start + entyouStart * BasisX;
                            }
                            else
                            {
                                start = beforend;
                                end = Get_Node_Position(stb, newL[j + 1].id, offsetX + kitenX, offsetY + kitenY);
                            }


                            //始点と終点の距離が1mm未満→次の点へ
                            double kyori = Commons.ft2mm(Math.Pow(((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y) + (end.Z - start.Z) * (end.Z - start.Z)), 0.5));
                            if (kyori < 1) { continue; }

                            Curve cr = null;
                            cr = Line.CreateBound(start, end);
                            cur.Add(cr);
                            beforend = end;
                        }

                        //通り芯の最後を延長
                        double entyouend = Commons.mm2ft(stbaxes.StbX_Axis[stbaxes.StbX_Axis.Count() - 1].distance + kitenX + offsetX) + entyou - beforend.X;
                        Curve cre = Line.CreateBound(beforend, beforend + Math.Max(entyouend, entyou) * BasisX);
                        cur.Add(cre);
                    }


                    CurveLoop cloop = CurveLoop.Create(cur);
                    
                    //作図面
                    XYZ normal = new XYZ(1, 1, 0);
                    XYZ normal2 = new XYZ(0, 0, 1);
                    Plane p = Plane.CreateByNormalAndOrigin(normal2, new XYZ(0, 0, 0));
                    SketchPlane skp = SketchPlane.Create(Commons.doc, p);

                    //複数セグメントの通り芯
                    MultiSegmentGrid mgr = (MultiSegmentGrid)(Commons.doc.GetElement(MultiSegmentGrid.Create(Commons.doc, gt.Id, cloop, skp.Id)));

                    //通り芯の名前の重複チェック
                    if (Name_Check(name, axis.name))
                    {
                        string rename = axis.name;
                        int ascii = 97;
                        do
                        {
                            rename += "_" + (char)ascii;
                            ascii++;

                        } while (Name_Check(name, rename));
                        axis.name = rename;
                    }
                    mgr.Name = axis.name;                    
                    name.Add(mgr.Name);

                    //通り芯生成ログ
                    MakeGridLog("Y軸の生成", mgr.Name, cur[0].GetEndPoint(0), cur[cur.Count - 1].GetEndPoint(1), 1);
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                LogData.AddLog(LogData.LogKind.Error, 0, logname);
            }

            tran.SetName("作図用軸の生成");
            try
            {
                tran.Start();

                logname = "作図用軸";
                if (stb.StbModel.StbAxes.StbDrawingAxis != null)
                {
                    for (int i = 0; i < stb.StbModel.StbAxes.StbDrawingAxis.Count(); i++)
                    {
                        STBclass.StbModelClass.StbAxesClass.Stb_DrawingAxis axis = stb.StbModel.StbAxes.StbDrawingAxis[i];
                        if(axis == null) { continue; }

                        XYZ start = new XYZ();
                        XYZ end = new XYZ();

                        start = new XYZ(Commons.mm2ft(axis.start_x + alloffsetX), Commons.mm2ft(axis.start_y + alloffsetY), 0);
                        end = new XYZ(Commons.mm2ft(axis.end_x + alloffsetX), Commons.mm2ft(axis.end_y + alloffsetY), 0);
                        Line newl = Line.CreateBound(end, start);
                        Grid g = Grid.Create(Commons.doc, newl);
                        //通り芯の名前の重複チェック
                        if (Name_Check(name, axis.name))
                        {
                            string rename = axis.name;
                            int ascii = 97;
                            do
                            {
                                rename += "_" + (char)ascii;
                                ascii++;

                            } while (Name_Check(name, rename));
                            axis.name = rename;
                        }
                        g.Name = axis.name;
                        name.Add(g.Name);
                    }
                }

                pform.TopMost = false;
                tran.Commit();
                pform.TopMost = true;
            }
            catch(Exception)
            {
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                LogData.AddLog(LogData.LogKind.Error, 0, logname);
            }

            return ret;
        }

        /// <summary>部材の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="chb"></param>
        internal void CreateBuzai(STBclass stb, ProgressBarForm pform, List<ConvertForm.Chb_class> chb)
        {
            string errmsg = "";
            Node_Set(stb); //節点でx,y,zが同じものをsub_idで探せるようにする

            //各部材ファミリを取得
            //梁
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            IList<Element> girders = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
            //柱
            collector = new FilteredElementCollector(Commons.doc);
            filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            IList<Element> columns = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            
            try
            {
                TransactionGroup trang = new TransactionGroup(Commons.doc, "変換開始");
                trang.Start();
                for (int i = 0; i < chb.Count(); i++)
                {
                    errmsg = "";

                    if (!chb[i].chbchecked) { continue; }
                    switch (chb[i].buzai)
                    {
                        case "柱":
                        case "間柱":
                            if (!CreateColumn(stb, pform, chb[i].buzai, columns, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }                           
                            break;
                        case "基礎柱":
                            if (!CreateFoundationColumn(stb, pform, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "大梁":
                        case "小梁":
                        case "片持梁":
                        case "片持小梁":
                            if (!CreateGirder(stb, pform, chb[i].buzai, girders, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "RCスラブ":
                        case "デッキプレート":
                        case "既製スラブ":
                        case "基礎スラブ":
                            if (!CreateSlab(stb, pform, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "Sブレース":
                            if (!CreateBrace(stb, pform, chb[i].buzai, girders, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "壁":
                        case "RCパラペット":
                            if (!CreateWall(stb, pform, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "基礎・布基礎・杭":
                            if (!CreateFoundation(stb, pform, chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                        case "柱脚":
                            if(!CreateClmBase(stb,pform,chb[i].buzai, ref errmsg))
                            { LogData.AddLog(LogData.LogKind.Error, 0, errmsg); }
                            break;
                    }
                }
                trang.Assimilate();
                //結合順序の入れ替え
                ChangeOrder(pform);

                Transaction tran4 = new Transaction(Commons.doc, "基礎のグループ化");
                try
                {
                    tran4.Start();
                    
                    //基礎梁と布基礎のグループ化
                    errmsg = "梁と布基礎のグループ化";
                    for (int i = 0; i < CGrp.Count(); i++)
                    {
                        if (CGrp[i].elId.Count() > 1)
                        {
                            Commons.doc.Create.NewGroup(CGrp[i].elId);
                            pform.TopMost = false;
                            Commons.doc.Regenerate();
                            pform.TopMost = true;
                        }
                    }
                    //基礎と杭のグループ化（グループ化前にRegenerateが必要）
                    errmsg = "基礎と杭のグループ化";
                    foreach (var k in FGroup.Keys)
                    {
                        if (FGroup[k].Count() > 1)
                        { Commons.doc.Create.NewGroup(FGroup[k]); }
                    }
                    pform.TopMost = false;
                    tran4.Commit();
                    pform.TopMost = true;
                }
                catch (Exception)
                {
                    
                    pform.TopMost = false;
                    tran4.RollBack();
                    pform.TopMost = true;
                }
            }
            catch(Exception)
            {
                //ログ出力
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
               
            }
        }

        /// <summary>重複節点を若い番号にまとめる
        /// </summary>
        /// <param name="stb"></param>
        private void Node_Set(STBclass stb)
        {
            for(int i =0; i < stb.StbModel.StbNodes.Count() - 1; i++)
            {
                STBclass.StbModelClass.StbNode node1 = stb.StbModel.StbNodes[i];
                for(int j = i + 1; j < stb.StbModel.StbNodes.Count(); j++)
                {
                    STBclass.StbModelClass.StbNode node2 = stb.StbModel.StbNodes[j];
                    if(node2.sub_id != 0) { continue; }
                    if (node1.x == node2.x && node1.y == node2.y && node1.z == node2.z)
                    { node2.sub_id = node1.id; }
                }
            }
            for(int i =0; i < stb.StbModel.StbNodes.Count(); i++)
            {
                STBclass.StbModelClass.StbNode node = stb.StbModel.StbNodes[i];
                if(node.sub_id == 0) { node.sub_id = node.id; }
            }
        }

     
       
        #region 各部材の生成        
        #region 柱
        /// <summary>柱の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <returns></returns>
        private bool CreateColumn(STBclass stb, ProgressBarForm pform, string syubetu, IList<Element> elements, ref string errmsg)
        {
            bool ret = true;

            Stopwatch stopw = new Stopwatch();
            stopw.Start();
            string kind = "";
            switch(syubetu)
            {
                case "柱":
                    kind = "COLUMN";
                                      
                    break;
                case "間柱":
                    kind = "POST";
                    break;
            }
            

            ProgressBar_Show(pform, syubetu + "の生成");
            if(kind == "COLUMN")
            {
                Clm_Parameter_Set(pform, SetFamily.ClmFName, elements);
            }
            else
            {
                Clm_Parameter_Set(pform, SetFamily.PClmFName, elements);
            }
            
           

            //変換ファミリの取得
            Family[][] ConvFamily = new Family[RevitLNK.ClmText.Length][];
            for (int i = 0; i < RevitLNK.ClmText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.ClmText[i].Length);
            }

            //FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            //ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            //IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }
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
            
            Transaction tran = new Transaction(Commons.doc, syubetu + "タイプパラメータの生成");
            try
            {
                tran.Start();
                
                //柱タイプパラメータの設定
                //RC柱
                if (stb.StbModel.StbSections.StbSecColumns_RC != null) //Clm[0][0](矩形),Clm[0][1](円形)
                {
                    int numCount = stb.StbModel.StbSections.StbSecColumns_RC.Count();                   
                    
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC clm = stb.StbModel.StbSections.StbSecColumns_RC[i];
                        if (kind != clm.kind_column) continue;

                        //プログレスバーの表示
                        GaugePercent("RC柱の生成", (int)((double)i / (double)numCount * 100));
                       
                        if (!CreateColumn_RC(stb, clm, pform, ConvFamily)) { ret = false; errmsg = "RC柱"; }
                    }
                   
                }
                //S柱
                if (stb.StbModel.StbSections.StbSecColumns_S != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecColumns_S.Count();
                    
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_S clm = stb.StbModel.StbSections.StbSecColumns_S[i];
                        if (kind != clm.kind_column) continue;

                        //プログレスバーの表示
                        GaugePercent("S柱の生成", (int)((double)i / (double)numCount * 100));
                        
                        if (!CreateColumn_S(stb, clm, pform, ConvFamily)) { ret = false; errmsg = "S柱"; }
                    }
                }
                //SRC柱
                if (stb.StbModel.StbSections.StbSecColumns_SRC != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecColumns_SRC.Count();
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm = stb.StbModel.StbSections.StbSecColumns_SRC[i];
                        if (kind != clm.kind_column) continue;

                        //プログレスバーの表示
                        GaugePercent("SRC柱の生成", (int)((double)i / (double)numCount * 100));

                        if (!CreateColumn_SRC(stb, clm, pform, ConvFamily)) { ret = false; errmsg = "SRC柱"; }
                    }
                }
                //CFT柱
                if (stb.StbModel.StbSections.StbSecColumns_CFT != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecColumns_CFT.Count();
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_CFT clm = stb.StbModel.StbSections.StbSecColumns_CFT[i];
                        if (kind != clm.kind_column) continue;

                        //プログレスバーの表示
                        GaugePercent("CFT柱の生成", (int)((double)i / (double)numCount * 100));
                                                
                        if (!CreateColumn_CFT(stb, clm, pform, ConvFamily)) { ret = false; errmsg = "CFT柱"; }
                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                ret = false;
            }
            tran.SetName ("インスタンスパラメータの生成");
            try
            {
                tran.Start();
                switch (kind)
                {
                    case "COLUMN":
                        //柱インスタンスパラメータの設定
                        if (stb.StbModel.StbMembers.StbColumns != null)
                        {
                            int numCount = stb.StbModel.StbMembers.StbColumns.Count();

                            for (int i = 0; i < numCount; i++)
                            {
                                STBclass.StbModelClass.StbMembersClass.StbColumn clm = stb.StbModel.StbMembers.StbColumns[i];
                                int sclmind = Get_SectionColumn(stb, clm.id_section, clm.kind_structure);

                                //プログレスバーの表示
                                GaugePercent("柱の生成", (int)((double)i / (double)numCount * 100));

                                
                                if (!CreateColumn_instance(stb, clm, sclmind, pform, ConvFamily)) { ret = false; errmsg = "柱インスタンス"; }
                            }
                        }
                        break;
                    case "POST":
                        if (stb.StbModel.StbMembers.StbPosts != null)
                        {
                            int numCount = stb.StbModel.StbMembers.StbPosts.Count();

                            for (int i = 0; i < numCount; i++)
                            {
                                STBclass.StbModelClass.StbMembersClass.StbPost clm = stb.StbModel.StbMembers.StbPosts[i];
                                int sclmind = Get_SectionColumn(stb, clm.id_section, clm.kind_structure);

                                //プログレスバーの表示
                                GaugePercent("間柱の生成", (int)((double)i / (double)numCount * 100));

                                if (!CreatePost_instance(stb, clm, sclmind, pform, ConvFamily)) { ret = false; errmsg = "間柱インスタンス"; }
                            }
                        }
                        break;
                }

                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran.Commit();
                pform.TopMost = true;

            }
            catch (Exception)
            {
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }



            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }

           
            return ret;
        }

        /// <summary> 基礎柱の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateFoundationColumn(STBclass stb, ProgressBarForm pform, ref string errmsg)
        {
            bool ret = true;

            Stopwatch stopw = new Stopwatch();
            stopw.Start();
            ProgressBar_Show(pform, "基礎柱の生成");



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



            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.BClmFName.flg[i][j]) { continue; }
                    if (!SetFamily.BClmFName.convflg[i][j]) { continue; }
                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }
                        if (familysymbol.FamilyName == SetFamily.BClmFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {  //プログレスバーの表示
                                GaugePercent("パラメータ追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.BClmFName.FamilyName, familysymbol.FamilyName, i, j);

                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
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
                if(stb.StbModel.StbSections.StbSecColumns_RC != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbFoundationColumns.Count();
                    List<int> ind = new List<int>();
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbFoundationColumn clm = stb.StbModel.StbMembers.StbFoundationColumns[i];
                        int sclmind = Get_SectionColumn(stb, clm.id_section, clm.kind_structure);
                        bool flg = false;
                        for(int j = 0; j < ind.Count(); j++)
                        {
                            if(ind[j] == sclmind)
                            {
                                flg = true;
                                break;
                            }
                        }
                        if (flg) { continue; }
                        ind.Add(sclmind);
                        //プログレスバーの表示
                        GaugePercent("基礎柱の生成", (int)((double)i / (double)numCount * 100));

                        if (!CreateColumn_RC(stb, stb.StbModel.StbSections.StbSecColumns_RC[sclmind], pform, ConvFamily)) { ret = false; errmsg = "基礎柱タイプ"; }
                    }
                }
                if (stb.StbModel.StbMembers.StbFoundationColumns != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbFoundationColumns.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbFoundationColumn clm = stb.StbModel.StbMembers.StbFoundationColumns[i];
                        int sclmind = Get_SectionColumn(stb, clm.id_section, clm.kind_structure);                        

                        //プログレスバーの表示
                        GaugePercent("基礎柱の生成", (int)((double)i / (double)numCount * 100));
                       
                        if (!CreateFoundationColumn_instance(stb, clm, sclmind, pform, ConvFamily)) { ret = false; errmsg = "基礎柱インスタンス"; }
                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();               
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }
            return ret;
        }

        /// <summary>柱・間柱パラメータセット
        /// </summary>
        /// <param name="pform"></param>
        /// <param name="ClmFName"></param>
        private void Clm_Parameter_Set(ProgressBarForm pform, FamilyStructure.ClmFamilyName ClmFName, IList<Element> elements)
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
            //FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            //ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            //IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            int numendadd = 0;
            for (int i = 0; i < ClmFName.convflg.Count(); i++)
            {
                for (int j = 0; j < ClmFName.convflg[i].Count(); j++)
                {
                    if (!ClmFName.flg[i][j]) { continue; }
                    if (!ClmFName.convflg[i][j]) { continue; }

                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }

                        if (familysymbol.FamilyName == ClmFName.FamilyName[i][j])
                        {

                            //プログレスバーの表示
                            numendadd++;
                            GaugePercent("パラメータ追加", (int)((double)numendadd / (double)numfamily * 100));
                            Document doc = Commons.doc.EditFamily(familysymbol.Family);
                            Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ClmFName.FamilyName + "パラメータ追加");
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
                                pform.TopMost = false;
                                tran1.Commit();
                                pform.TopMost = true;
                                doc.Close(false);
                                break;
                            }
                            catch (Exception)
                            {
                                pform.TopMost = false;
                                tran1.RollBack();
                                pform.TopMost = true;
                                doc.Close(false);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 複数層に配置されている断面をチェックし複製する
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="floor">断面.floor</param>
        /// <param name="id_section">断面Id</param>
        /// <param name="name">符号</param>
        /// <param name="symbol"></param>
        private void CheckMultiFloor_Column(STBclass stb, string floor, int id_section, string name, FamilySymbol symbol)
        {
            if (symbol != null)
            {
                if (Get_stbFloor_index(stb, floor) == -1)
                {
                    //断面の配置フロアが見つからない場合は節点の所属層を全部取得する
                    List<int> node_bottom = new List<int>();
                    if (stb.StbModel.StbMembers.StbColumns != null)
                    {
                        node_bottom.AddRange(stb.StbModel.StbMembers.StbColumns.Where(a => a.id_section == id_section).Select(a => a.idNode_bottom).ToList());
                    }
                    if (stb.StbModel.StbMembers.StbPosts != null)
                    {
                        node_bottom.AddRange(stb.StbModel.StbMembers.StbPosts.Where(a => a.id_section == id_section).Select(a => a.idNode_bottom).ToList());
                    }

                    node_bottom = node_bottom.Distinct().ToList();
                    node_bottom.Sort();

                    List<string> typename2 = new List<string>();
                    for (int i = 0; i < node_bottom.Count; ++i)
                    {
                        int find = Get_stbFloor_index(stb, node_bottom[i]);
                        if (find != -1)
                        {
                            typename2.Add(stb.StbModel.StbStories[find].name + name);
                        }
                    }
                    typename2 = typename2.Distinct().Where(a => a != symbol.Name).ToList();

                    var symbol_names = symbol.Family.GetFamilySymbolIds().Select(a => Commons.doc.GetElement(a)).OfType<FamilySymbol>().Select(a => a.Name.ToUpper()).ToList();

                    for (int i = 0; i < typename2.Count; ++i)
                    {
                        //異なる層で同じ断面が使用されているので複製する
                        //symbol.Duplicate(typename2[i]);
                        string name2 = typename2[i];
                        int ascii = 97;
                        while (symbol_names.Contains(name2.ToUpper()))
                        {
                            name2 = ReName(typename2[i], ascii);
                            ascii++;
                        }

                        symbol.Duplicate(name2);
                        symbol_names.Add(name2);
                    }
                }
            }
        }

        /// <summary>
        /// 柱のタイプ名取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm_floor">断面.floor</param>
        /// <param name="clm_id">断面.id</param>
        /// <param name="clm_name">断面.name</param>
        /// <returns>タイプ名</returns>
        private string GetTypeName_Column(STBclass stb, string clm_floor, int clm_id, string clm_name)
        {
            string typename = "";
            string floor = clm_floor;

            int find = Get_stbFloor_index(stb, floor);
            if (find == -1)
            {
                find = Get_stbFloor_index_Clm(stb, clm_id);
            }
            if (find != -1)
            {
                typename = stb.StbModel.StbStories[find].name;
            }

            typename += clm_name;

            return typename;
        }

        /// <summary>RC柱タイプパラメータ設定
        /// </summary>
        /// <param name="clm"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateColumn_RC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC clm, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;
            string typename = GetTypeName_Column(stb, clm.floor, clm.id, clm.name);

            //2017/05/19 鉄筋のタグが無いとき→ログ出力
            string logbuzai = "";
            if(clm.StbSecFigure.StbSecFigureType == 1) { logbuzai = "RC矩形柱"; }
            else { logbuzai = "RC円柱"; }
            if(clm.StbSecBar_Arrangement == null)
            {
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[RC柱]" + typename + "(断面id=" + clm.id.ToString() + ")");
            }


            FamilySymbol symbol = null;
            if (clm.StbSecFigure.StbSecFigureType == 1)
            {
                if(ConvFamily[0][0] == null)
                {
                    //ログ表示（ファミリ未ロード)
                    LogData.AddLog(LogData.LogKind.Warning, 2100, logbuzai);
                    return ret;
                }

                if (!SearchFamilySymbol(ConvFamily[0][0], typename, ref symbol))
                { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                else
                {
                    typename = Data.ReName2(ConvFamily[0][0], typename);
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                FamilyStructure.RC_Clm_Re Rclm = SetFamily.RCClmRe;

                //鉄筋径のチェック
                Get_D("RC柱", ref clm.D_reinforcement_main, "主筋", typename, clm.id);
                Get_D("RC柱", ref clm.D_reinforcement_2nd_main, "副主筋", typename, clm.id);
                Get_D("RC柱", ref clm.D_reinforcement_axial, "軸筋", typename, clm.id);
                Get_D("RC柱", ref clm.D_reinforcement_band, "帯筋", typename, clm.id);
                Get_D("RC柱", ref clm.D_bar_spacing, "巾止筋", typename, clm.id);

                SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
                if(clm.kind_column == "COLUMN")
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
                else
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
                SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
                SetParameter(symbol.LookupParameter(Rclm.DX), clm.StbSecFigure.StbSecRect.DX, true);
                SetParameter(symbol.LookupParameter(Rclm.DY), clm.StbSecFigure.StbSecRect.DY, true);
                SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
               
                SetParameter(symbol.LookupParameter(Rclm.center_reinforcement_start_X), clm.center_reinforcement_start_X);
                SetParameter(symbol.LookupParameter(Rclm.center_reinforcement_start_Y), clm.center_reinforcement_start_Y);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), clm.strength_reinforcement_2nd_main);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_axial), clm.strength_reinforcement_axial);
                SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
                SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
                SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.kind_reinforcement_corner);
                SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.kind_reinforcement_corner);
                SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.interval_reinforcement);
                SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.depth_cover_start_X);
                SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.depth_cover_end_X);
                SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.depth_cover_start_Y);
                SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.depth_cover_end_Y);

                SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_axial), clm.D_reinforcement_axial);

                if (clm.StbSecBar_Arrangement != null)
                {
                    if (clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced != null)
                    { SetParameter(symbol.LookupParameter(Rclm.count_main_total_X), clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced.count_main_total); }
                    if (clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same != null)
                    {
                        for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same.Count(); j++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass bar =
                                clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[j];
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x = 
                                clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                            if (x != null)
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                            else
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                            if (x != null)
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                            else
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                            SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);

                            SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar.count_axial);
                            SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar.count_axial);
                        }
                    }
                    else if (clm.StbSecBar_Arrangement.StbSecRect_Column_Same != null)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass bar =
                               clm.StbSecBar_Arrangement.StbSecRect_Column_Same;
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                               clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                            if (x != null)
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                            else
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                            if (x != null)
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                            else
                            { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                            SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[j]), clm.kind_reinforcement_corner);
                            SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);

                            SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar.count_axial);
                            SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar.count_axial);
                        }
                    }
                }
            }
            else //円形
            {
                if (ConvFamily[0][1] == null)
                {
                    //ログ表示（ファミリ未ロード）
                    LogData.AddLog(LogData.LogKind.Warning, 2100, logbuzai);
                    return ret;
                }

                if (!SearchFamilySymbol(ConvFamily[0][1], typename, ref symbol))
                { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                else
                {
                    typename = Data.ReName2(ConvFamily[0][1], typename);
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                FamilyStructure.RC_Clm_Ro Rclm = SetFamily.RCClmRo;

                SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
                SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
                if (clm.kind_column == "COLUMN")
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
                else
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
                SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
                SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_axial), clm.D_reinforcement_axial);
                SetParameter(symbol.LookupParameter(Rclm.D), clm.StbSecFigure.StbSecCircle.D, true);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_axial), clm.strength_reinforcement_axial);
                SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
                SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);                
                SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
                SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.depth_cover_start_X);
                if (clm.StbSecBar_Arrangement != null)
                {
                    if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same != null)
                    {
                        for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same.Count(); j++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass bar =
                                clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[j];
                            SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar.count_axial);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                            SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                            SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);                        
                            SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);

                            SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar.count_axial);
                        }
                    }
                    else if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Same != null)
                    {
                        for (int j = 0; j < Rclm.D_reinforcement_main.Count(); j++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_RC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass bar =
                                clm.StbSecBar_Arrangement.StbSecCircle_Column_Same;
                            SetParameter(symbol.LookupParameter(Rclm.count_axial[j]), bar.count_axial);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                            SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                            SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                            SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                            SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                            SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);

                            SetParameter(symbol.LookupParameter(Rclm.count_axial_list), bar.count_axial);
                        }
                    }
                }
            }

            if (symbol != null)
            {
                CheckMultiFloor_Column(stb, clm.floor, clm.id, clm.name, symbol);
            }


            return ret;
        }
      
        /// <summary>S柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateColumn_S(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_S clm, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;
            string typename = GetTypeName_Column(stb, clm.floor, clm.id, clm.name);

            //鉄骨形状を取得
            int shapeid = -1;
            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_S.StbSecSteelColumnClass secsteel = null;
            int steelind = 0;
            if(clm.StbSecSteelColumn[0] != null)
            {
                secsteel = clm.StbSecSteelColumn[0];
                steelind = 0;
            }
            else if(clm.StbSecSteelColumn[2] != null)
            {
                secsteel = clm.StbSecSteelColumn[2];
                steelind = 2;
            }
            else if(clm.StbSecSteelColumn[1] != null)
            {
                secsteel = clm.StbSecSteelColumn[1];
                steelind = 1;
            }
            if(secsteel == null)
            {
                LogData.AddLog(LogData.LogKind.Warning, 3000, "[S柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")は柱鉄骨情報");
                return ret;
            }

            string shape = Check_Steel(stb, secsteel.shape, ref shapeid);

            FamilySymbol symbol = null;

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

                    FamilyStructure.S_Clm_H Rclm = SetFamily.SClmH;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeid];
                    string logtxt0 = Roll_H_Size_Check(steel);
                    if (logtxt0 != "")
                    {
                        MakeSizeLog(shapename0, symbol.Name, clm.id, logtxt0, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][0], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(Rclm.strength_web), GetStrength_web(secsteel.strength_web, secsteel.strength_main));
                    SetParameter(symbol.LookupParameter(Rclm.strength_main), secsteel.strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(Rclm.B), steel.B, true);
                    SetParameter(symbol.LookupParameter(Rclm.A), steel.A, true);
                    SetParameter(symbol.LookupParameter(Rclm.t1), steel.t1, true);
                    SetParameter(symbol.LookupParameter(Rclm.t2), steel.t2, true);
                    SetParameter(symbol.LookupParameter(Rclm.r), steel.r, true);
                    SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
                    SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(Rclm.type), steel.type);
                    SetParameter(symbol.LookupParameter(Rclm.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_BH:
                    string shapename1 = "S柱組立H形鋼";
                    if (ConvFamily[1][1] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename1);
                        return ret;
                    }
                    FamilyStructure.S_Clm_BH RclmBH = SetFamily.SClmBH;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steelBH =
                       stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeid];

                    string logtxt1 = Build_H_Size_Check(steelBH);
                    if (logtxt1 != "")
                    {
                        MakeSizeLog(shapename1, symbol.Name, clm.id, logtxt1, 0);
                        return ret;
                    }

                    if (!SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][1], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmBH.strength_web), GetStrength_web(secsteel.strength_web, secsteel.strength_main));
                    SetParameter(symbol.LookupParameter(RclmBH.strength_main), secsteel.strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmBH.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmBH.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmBH.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmBH.B), steelBH.B, true);
                    SetParameter(symbol.LookupParameter(RclmBH.A), steelBH.A, true);
                    SetParameter(symbol.LookupParameter(RclmBH.t1), steelBH.t1, true);
                    SetParameter(symbol.LookupParameter(RclmBH.t2), steelBH.t2, true);
                    SetParameter(symbol.LookupParameter(RclmBH.r), 0.0, true);
                    SetParameter(symbol.LookupParameter(RclmBH.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmBH.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmBH.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmBH.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_Box:
                    string shapename2 = "S柱角形鋼";
                    if (ConvFamily[1][2] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "S柱角形鋼管");
                        return ret;
                    }

                    FamilyStructure.S_Clm_Box RclmBox = SetFamily.SClmBox;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_BOX_Class steelBox =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_BOX[shapeid];

                    string logtxt2 = Roll_Box_Size_Check(steelBox);
                    if (logtxt2 != "")
                    {
                        MakeSizeLog(shapename2, symbol.Name, clm.id, logtxt2, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][2], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmBox.strength_main), secsteel.strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmBox.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmBox.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmBox.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmBox.B), steelBox.B, true);
                    SetParameter(symbol.LookupParameter(RclmBox.A), steelBox.A, true);
                    SetParameter(symbol.LookupParameter(RclmBox.t1), steelBox.t, true);
                    SetParameter(symbol.LookupParameter(RclmBox.r), steelBox.R, true);
                    SetParameter(symbol.LookupParameter(RclmBox.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmBox.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmBox.type), steelBox.type);
                    SetParameter(symbol.LookupParameter(RclmBox.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmBox.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_BBox:
                    string shapename3 = "S柱組立角形鋼管";
                    if (ConvFamily[1][3] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename3);
                        return ret;
                    }

                    FamilyStructure.S_Clm_BBox RclmBBox = SetFamily.SClmBBox;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_BOX_Class steelBBox =
                       stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX[shapeid];

                    string logtxt3 = Build_Box_Size_Check(steelBBox);
                    if (logtxt3 != "")
                    {
                        MakeSizeLog(shapename3, symbol.Name, clm.id, logtxt3, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][3], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmBBox.strength_main), secsteel.strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmBBox.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmBBox.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmBBox.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmBBox.B), steelBBox.B, true);
                    SetParameter(symbol.LookupParameter(RclmBBox.A), steelBBox.A, true);
                    SetParameter(symbol.LookupParameter(RclmBBox.size_imput), true, true);
                    SetParameter(symbol.LookupParameter(RclmBBox.t1), steelBBox.t1, true);
                    SetParameter(symbol.LookupParameter(RclmBBox.t2), steelBBox.t2, true);
                    SetParameter(symbol.LookupParameter(RclmBBox.r), 0.0, true);
                    //if(ConvFamily[1][3].Name == "Steel_Column_BBox")
                    //{
                    //    SetParameter(symbol.LookupParameter("BBOX 板厚 別サイズ入力"), true);
                    //    SetParameter(symbol.LookupParameter("フィレット"), 0.0, true);
                    //}
                    SetParameter(symbol.LookupParameter(RclmBBox.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmBBox.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmBBox.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmBBox.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_Pipe:
                    string shapename4 = "S柱円形鋼管";
                    if (ConvFamily[1][4] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename4);
                        return ret;
                    }

                    FamilyStructure.S_Clm_Pipe RclmP = SetFamily.SClmPipe;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecPipe_Class steelP =
                        stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];

                    string logtxt4 = Pipe_Size_Check(steelP);
                    if (logtxt4 != "")
                    {
                        MakeSizeLog(shapename4, symbol.Name, clm.id, logtxt4, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                    {
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][4], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmP.strength_main), secsteel.strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmP.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmP.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmP.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmP.D), steelP.D, true);
                    SetParameter(symbol.LookupParameter(RclmP.t), steelP.t, true);
                    SetParameter(symbol.LookupParameter(RclmP.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmP.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmP.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmP.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_T:
                    string shapename5 = "S柱T形鋼";
                    if (ConvFamily[1][5] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename5);
                        return ret;
                    }

                    FamilyStructure.S_Clm_T RclmT = SetFamily.SClmT;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_T_Class steelT =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_T[shapeid];

                    string logtxt5 = Roll_T_Size_Check(steelT);
                    if (logtxt5 != "")
                    {
                        MakeSizeLog(shapename5, symbol.Name, clm.id, logtxt5, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][5], typename, ref symbol))
                    {
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][5], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmT.strength_web), GetStrength_web(clm.StbSecSteelColumn[0].strength_web, clm.StbSecSteelColumn[0].strength_main));
                    SetParameter(symbol.LookupParameter(RclmT.strength_main), clm.StbSecSteelColumn[0].strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmT.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmT.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmT.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmT.B), steelT.B, true);
                    SetParameter(symbol.LookupParameter(RclmT.A), steelT.A, true);
                    SetParameter(symbol.LookupParameter(RclmT.t1), steelT.t1, true);
                    SetParameter(symbol.LookupParameter(RclmT.t2), steelT.t2, true);
                    SetParameter(symbol.LookupParameter(RclmT.r), steelT.r, true);
                    SetParameter(symbol.LookupParameter(RclmT.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmT.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmT.type), steelT.type);
                    SetParameter(symbol.LookupParameter(RclmT.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmT.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_C:
                    string shapename6 = "S柱溝形鋼";
                    if (ConvFamily[1][6] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename6);
                        return ret;
                    }

                    FamilyStructure.S_Clm_C RclmC = SetFamily.SClmC;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steelC =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C[shapeid];

                    string logtxt6 = Roll_C_Size_Check(steelC);
                    if (logtxt6 != "")
                    {
                        MakeSizeLog(shapename6, symbol.Name, clm.id, logtxt6, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][6], typename, ref symbol))
                    {
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][6], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmC.strength_main), clm.StbSecSteelColumn[0].strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmC.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmC.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmC.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmC.B), steelC.B, true);
                    SetParameter(symbol.LookupParameter(RclmC.A), steelC.A, true);
                    SetParameter(symbol.LookupParameter(RclmC.t1), steelC.t1, true);
                    SetParameter(symbol.LookupParameter(RclmC.t2), steelC.t2, true);
                    SetParameter(symbol.LookupParameter(RclmC.r1), steelC.r1, true);
                    SetParameter(symbol.LookupParameter(RclmC.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmC.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmC.r2), steelC.r2, true);
                    SetParameter(symbol.LookupParameter(RclmC.side), steelC.side);
                    SetParameter(symbol.LookupParameter(RclmC.type), steelC.type);
                    SetParameter(symbol.LookupParameter(RclmC.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmC.base_type), clm.base_type);
                    break;
                case RevitLNK.st_steel_L:
                    string shapename7 = "S柱山形鋼";
                    if (ConvFamily[1][7] == null)
                    {
                        //ログ表示（ファミリ未ロード）
                        LogData.AddLog(LogData.LogKind.Warning, 2100, shapename7);
                        return ret;
                    }

                    FamilyStructure.S_Clm_L RclmL = SetFamily.SClmL;
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steelL =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L[shapeid];

                    string logtxt7 = Roll_L_Size_Check(steelL);
                    if (logtxt7 != "")
                    {
                        MakeSizeLog(shapename7, symbol.Name, clm.id, logtxt7, 0);
                        return ret;
                    }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[1][7], typename, ref symbol))
                    {
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[1][7], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    SetParameter(symbol.LookupParameter(RclmL.strength_main), clm.StbSecSteelColumn[0].strength_main);
                    if (clm.kind_column == "COLUMN")
                    { SetParameter(symbol.LookupParameter(RclmL.kind_column), "Column"); }
                    else
                    { SetParameter(symbol.LookupParameter(RclmL.kind_column), "Post"); }
                    SetParameter(symbol.LookupParameter(RclmL.kind_column2), clm.kind_column);
                    SetParameter(symbol.LookupParameter(RclmL.B), steelL.B, true);
                    SetParameter(symbol.LookupParameter(RclmL.A), steelL.A, true);
                    SetParameter(symbol.LookupParameter(RclmL.t1), steelL.t1, true);
                    SetParameter(symbol.LookupParameter(RclmL.t2), steelL.t2, true);
                    SetParameter(symbol.LookupParameter(RclmL.r1), steelL.r1, true);
                    SetParameter(symbol.LookupParameter(RclmL.name), clm.name);
                    SetParameter(symbol.LookupParameter(RclmL.SecId), clm.id);
                    SetParameter(symbol.LookupParameter(RclmL.r2), steelL.r2, true);
                    SetParameter(symbol.LookupParameter(RclmL.side), steelL.side);
                    SetParameter(symbol.LookupParameter(RclmL.type), steelL.type);
                    SetParameter(symbol.LookupParameter(RclmL.type_name), secsteel.shape);
                    SetParameter(symbol.LookupParameter(RclmL.base_type), clm.base_type);
                    break;
                default:
                    string shapename = "";
                    if (shape == "")
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 2500, "[S柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + clm.StbSecSteelColumn[steelind].shape + "]");
                        return ret;
                    }
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
                    Make_taisyougaiLog("[S柱]", clm.id, typename, shape, shapename);
                    break;
            }

            if (symbol != null)
            {
                CheckMultiFloor_Column(stb, clm.floor, clm.id, clm.name, symbol);
            }


            return ret;
        }

        /// <summary>SRC柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateColumn_SRC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            string typename = GetTypeName_Column(stb, clm.floor, clm.id, clm.name);

            FamilySymbol symbol = null;

            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class src = null;
            if (clm.StbSecSteelColumn_SRC[0] != null)
            { src = clm.StbSecSteelColumn_SRC[0]; }
            else if (clm.StbSecSteelColumn_SRC[1] != null)
            { src = clm.StbSecSteelColumn_SRC[1]; }
            else
            { src = clm.StbSecSteelColumn_SRC[2]; }

            //鉄骨形状を取得
            string shape = "";
            //Revitの鉄骨断面は1断面なので、柱脚⇒中央⇒柱頭の優先順
            if (clm.StbSecSteelColumn_SRC[0] != null)
            {
                if (clm.StbSecSteelColumn_SRC[0].pos == "ALL")
                { shape = clm.StbSecSteelColumn_SRC[0].build_up_shape; }
                else
                {
                    //ログ
                    if (clm.StbSecSteelColumn_SRC[1] != null)
                    { shape = clm.StbSecSteelColumn_SRC[1].build_up_shape; }
                    else
                    {
                        if (clm.StbSecSteelColumn_SRC[2] != null)
                        { shape = clm.StbSecSteelColumn_SRC[2].build_up_shape; }
                    }
                }
            }
            if (shape == "H")
            {
                if (clm.StbSecFigure.StbSecFigureType == 1)
                {
                    if (ConvFamily[2][0] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][0], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }

                    if (!CreateColumn_SRC_H_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][3] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][3], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][3], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }


                    if (!CreateColumn_SRC_H_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else if (shape == "CROSS")
            {
                if (clm.StbSecFigure.StbSecFigureType == 1)
                {
                    if (ConvFamily[2][1] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][1], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][1], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }


                    if (!CreateColumn_SRC_Cross_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][4] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][4], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][4], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }


                    if (!CreateColumn_SRC_Cross_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else if (shape == "T")
            {
                if (clm.StbSecFigure.StbSecFigureType == 1)
                {
                    if (ConvFamily[2][2] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][2], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][2], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }


                    if (!CreateColumn_SRC_T_Rec(stb, clm, symbol))
                    { return ret; }
                }
                else
                {
                    if (ConvFamily[2][5] == null) { ret = false; return ret; }

                    symbol = null;
                    if (!SearchFamilySymbol(ConvFamily[2][5], typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    else
                    {
                        typename = Data.ReName2(ConvFamily[2][5], typename);
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }


                    if (!CreateColumn_SRC_T_Rou(stb, clm, symbol))
                    { return ret; }
                }
            }
            else
            {
                //ログ表示（変換対象外）
                if(shape == "BOX")
                {
                    Make_taisyougaiLog("SRC柱", clm.id, clm.name, "StbSecColumn_SRC_ShapeBox", "SRC柱□形断面鉄骨形状");
                }
                else
                {
                    Make_taisyougaiLog("SRC柱", clm.id, clm.name, "StbSecColumn_SRC_ShapePipe", "SRC柱○形断面鉄骨形状");
                }
            }

            if(clm.StbSecBar_Arrangement == null)
            {  
                //ログ表示
                LogData.AddLog(LogData.LogKind.Warning, 2400, "[SRC柱]" + typename + "(断面id=" + clm.id.ToString() + ")");
            }

            if (symbol != null)
            {
                CheckMultiFloor_Column(stb, clm.floor, clm.id, clm.name, symbol);
            }


            return ret;
        }

        private bool CreateColumn_SRC_H_Rec(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;

            FamilyStructure.SRC_Clm_H Rclm = SetFamily.SRCClmH;

            string shapename = "SRC柱H形断面鉄骨形状";
            string logtxt = "";
            string shape = "";
            double B = 0, A = 0, t1 = 0, t2 = 0, r = 0;
            string type = "";
            int shapeidX = -1;
            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeH_Class shapeH = null;
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    shapeH = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeH;
                    shape = Check_Steel(stb, shapeH.shape, ref shapeidX);
                    if (shape != "")
                    { break; }
                }
            }

            if(shape == "")
            {
                if (shapeH != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + shapeH.shape + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱H形断面鉄骨形状"); }
                return ret;
            }
            else if (shape == RevitLNK.st_steel_H)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                type = steel.type;
            }
            else 
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                   stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
            }
            SetParameter(symbol.LookupParameter(Rclm.strength_web), GetStrength_web(shapeH.strength_web, shapeH.strength_main));
            SetParameter(symbol.LookupParameter(Rclm.strength_main), shapeH.strength_main);
            SetParameter(symbol.LookupParameter(Rclm.direction_type), shapeH.direction_type);
            SetParameter(symbol.LookupParameter(Rclm.offset_X), shapeH.offset_X, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_Y), shapeH.offset_Y, true);
            SetParameter(symbol.LookupParameter(Rclm.type), type);
            SetParameter(symbol.LookupParameter(Rclm.typename), shapeH.shape);
            if (shapeH.direction_type == "H")
            {
                SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
            }
            SetParameter(symbol.LookupParameter(Rclm.H), A, true);
            SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            SetParameter(symbol.LookupParameter(Rclm.r), r, true);



            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);

            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), clm.strength_reinforcement_2nd_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.depth_cover_end_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.depth_cover_start_Y);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.depth_cover_end_Y);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.interval_reinforcement);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.DX), clm.StbSecFigure.StbSecRect.DX, true);
            SetParameter(symbol.LookupParameter(Rclm.DY), clm.StbSecFigure.StbSecRect.DY, true);

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[j];
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecRect_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_Same;
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[j]), clm.kind_reinforcement_corner);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced != null)
                { SetParameter(symbol.LookupParameter(Rclm.count_main_total_X), clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced.count_main_total); }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }

            return ret;
        }
        private bool CreateColumn_SRC_H_Rou(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;
            string logtxt = "";
            string shapename = "SRC柱H形断面鉄骨形状";
            FamilyStructure.SRC_Clm_H_Rou Rclm = SetFamily.SRCClmH_Rou;
            double B = 0, A = 0, t1 = 0, t2 = 0, r = 0;
            string type = "";
            int shapeidX = -1;
            string shape = "";
            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeH_Class shapeH = null;
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    shapeH = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeH;
                    shape = Check_Steel(stb, shapeH.shape, ref shapeidX);
                    if(shape != "")
                    { break; }
                }
            }
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                type = steel.type;
            }
            else
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                   stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                B = steel.B;
                A = steel.A;
                t1 = steel.t1;
                t2 = steel.t2;
            }
        
            
            if (shapeH.direction_type == "H")
            {
                SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
            }
            SetParameter(symbol.LookupParameter(Rclm.H), A, true);
            SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            SetParameter(symbol.LookupParameter(Rclm.r), r, true);

            SetParameter(symbol.LookupParameter(Rclm.strength_web), GetStrength_web(shapeH.strength_web, shapeH.strength_main));
            SetParameter(symbol.LookupParameter(Rclm.strength_main), shapeH.strength_main);
            SetParameter(symbol.LookupParameter(Rclm.direction_type), shapeH.direction_type);
            SetParameter(symbol.LookupParameter(Rclm.offset_X), shapeH.offset_X, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_Y), shapeH.offset_Y, true);
            SetParameter(symbol.LookupParameter(Rclm.type), type);
            SetParameter(symbol.LookupParameter(Rclm.typename), shapeH.shape);



            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);
            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.D), clm.StbSecFigure.StbSecCircle.D, true);
            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[j];

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecCircle_Column_Same;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }
            return ret;
        }
        private bool CreateColumn_SRC_Cross_Rec(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;
            string logtxt = "";
            string shapename = "SRC柱＋形断面鉄骨形状";

            FamilyStructure.SRC_Clm_Cross Rclm = SetFamily.SRCClmCross;
            //鉄骨形状のindex
            int shapeidX = -1, shapeidY = -1;            
            string shapetypeX = "";
            string shapename_X = "";
            string shapename_Y = "";
            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeCross_Class Cross = null;
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    Cross = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeCross;
                    shapename_X = Cross.shape_X;
                    shapename_Y = Cross.shape_Y;
                    shapetypeX = Check_Steel(stb, Cross.shape_X, ref shapeidX);
                    if(shapename_X != "" && shapename_Y != "")
                    { break; }
                }
            }
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
                Xr = steel.r;
                type_X = steel.type;
            }
            else if (shapetypeX == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                      stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
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

            string shapetypeY = Check_Steel(stb, Cross.shape_Y, ref shapeidY);
            if (shapetypeY == "")
            {
                if (Cross != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")の鉄骨形状[" + Cross.shape_Y + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[SRC柱]" + clm.name + "(断面id=" + clm.id.ToString() + ")はSRC柱＋形断面鉄骨形状"); }
                return ret;
            }
            else if(shapetypeY == RevitLNK.st_steel_H)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidY];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
                Yr = steel.r;
                type_Y = steel.type;
            }
            else if (shapetypeY == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                      stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidY];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
            }

            SetParameter(symbol.LookupParameter(Rclm.strength_main_X), Cross.strength_main_X);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_X), GetStrength_web(Cross.strength_web_X, Cross.strength_main_X));
            SetParameter(symbol.LookupParameter(Rclm.strength_main_Y), Cross.strength_main_Y);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_Y), GetStrength_web(Cross.strength_web_Y, Cross.strength_main_Y));
            SetParameter(symbol.LookupParameter(Rclm.XH), XH, true);
            SetParameter(symbol.LookupParameter(Rclm.XB), XB, true);
            SetParameter(symbol.LookupParameter(Rclm.Xt1), Xt1, true);
            SetParameter(symbol.LookupParameter(Rclm.Xt2), Xt2, true);
            SetParameter(symbol.LookupParameter(Rclm.Xr), Xr, true);
            SetParameter(symbol.LookupParameter(Rclm.YH), YH, true);
            SetParameter(symbol.LookupParameter(Rclm.YB), YB, true);
            SetParameter(symbol.LookupParameter(Rclm.Yt1), Yt1, true);
            SetParameter(symbol.LookupParameter(Rclm.Yt2), Yt2, true);
            SetParameter(symbol.LookupParameter(Rclm.Yr), Yr, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_XX), Cross.offset_XX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_XY), Cross.offset_XY, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_YX), Cross.offset_YX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_YY), Cross.offset_YY, true);
            SetParameter(symbol.LookupParameter(Rclm.type_X), type_X);
            SetParameter(symbol.LookupParameter(Rclm.type_Y), type_Y);
            SetParameter(symbol.LookupParameter(Rclm.typename_X), shapename_X);
            SetParameter(symbol.LookupParameter(Rclm.typename_Y), shapename_Y);

            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);
            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), clm.strength_reinforcement_2nd_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.depth_cover_end_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.depth_cover_start_Y);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.depth_cover_end_Y);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.interval_reinforcement);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.DX), clm.StbSecFigure.StbSecRect.DX, true);
            SetParameter(symbol.LookupParameter(Rclm.DY), clm.StbSecFigure.StbSecRect.DY, true);

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[j];
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecRect_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_Same;
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[j]), clm.kind_reinforcement_corner);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced != null)
                { SetParameter(symbol.LookupParameter(Rclm.count_main_total_X), clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced.count_main_total); }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }


            return ret;
        }
        private bool CreateColumn_SRC_Cross_Rou(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;
            string logtxt = "";
            string shapename = "SRC柱＋形断面鉄骨形状";
            FamilyStructure.SRC_Clm_Cross_Rou Rclm = SetFamily.SRCClmCross_Rou;

            //鉄骨形状のindex
            int shapeidX = -1, shapeidY = -1;
            string shapetypeX = "";
            string shapename_X = "";
            string shapename_Y = "";
            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeCross_Class Cross = null;
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    Cross = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeCross;
                    shapename_X = Cross.shape_X;
                    shapename_Y = Cross.shape_Y;
                    shapetypeX = Check_Steel(stb, Cross.shape_X, ref shapeidX);
                    if (shapename_X != "" && shapename_Y != "")
                    { break; }
                }
            }
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                XB = steel.B;
                XH = steel.A;
                Xt1 = steel.t1;
                Xt2 = steel.t2;
                Xr = steel.r;
                type_X = steel.type;
            }
            else if (shapetypeX == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                      stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
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

            string shapetypeY = Check_Steel(stb, Cross.shape_Y, ref shapeidY);
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidY];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
                Yr = steel.r;
                type_Y = steel.type;
            }
            else if (shapetypeY == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                      stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidY];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                YB = steel.B;
                YH = steel.A;
                Yt1 = steel.t1;
                Yt2 = steel.t2;
            }

            SetParameter(symbol.LookupParameter(Rclm.strength_main_X), Cross.strength_main_X);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_X), GetStrength_web(Cross.strength_web_X, Cross.strength_main_X));
            SetParameter(symbol.LookupParameter(Rclm.strength_main_Y), Cross.strength_main_Y);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_Y), GetStrength_web(Cross.strength_web_Y, Cross.strength_main_Y));
            SetParameter(symbol.LookupParameter(Rclm.XH), XH, true);
            SetParameter(symbol.LookupParameter(Rclm.XB), XB, true);
            SetParameter(symbol.LookupParameter(Rclm.Xt1), Xt1, true);
            SetParameter(symbol.LookupParameter(Rclm.Xt2), Xt2, true);
            SetParameter(symbol.LookupParameter(Rclm.Xr), Xr, true);
            SetParameter(symbol.LookupParameter(Rclm.YH), YH, true);
            SetParameter(symbol.LookupParameter(Rclm.YB), YB, true);
            SetParameter(symbol.LookupParameter(Rclm.Yt1), Yt1, true);
            SetParameter(symbol.LookupParameter(Rclm.Yt2), Yt2, true);
            SetParameter(symbol.LookupParameter(Rclm.Yr), Yr, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_XX), Cross.offset_XX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_XY), Cross.offset_XY, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_YX), Cross.offset_YX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_YY), Cross.offset_YY, true);
            SetParameter(symbol.LookupParameter(Rclm.type_X), type_X);
            SetParameter(symbol.LookupParameter(Rclm.type_Y), type_Y);
            SetParameter(symbol.LookupParameter(Rclm.typename_X), shapename_X);
            SetParameter(symbol.LookupParameter(Rclm.typename_Y), shapename_Y);

            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);
            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.D), clm.StbSecFigure.StbSecCircle.D, true);
            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[j];

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecCircle_Column_Same;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }

            return ret;
        }
        private bool CreateColumn_SRC_T_Rec(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;
            string logtxt = "";
            string shapename = "SRC柱T形断面鉄骨形状";

            FamilyStructure.SRC_Clm_T Rclm = SetFamily.SRCClmT;

            int shapeidX = 0, shapeidY = 0;

            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeT_Class shape_T = null;           
            string shapetypeH = "", shapetypeT = "";
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    if (clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT != null)
                    {
                        shape_T = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT;
                        shapetypeH = Check_Steel(stb, clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT.shape_H, ref shapeidX);
                        shapetypeT = Check_Steel(stb, clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT.shape_T, ref shapeidY);
                        if(shapetypeH != "" && shapetypeT != "")
                        { break; }
                    }
                }
            }
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                       stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                typeH = steel.type;
            }
            else if (shapetypeH == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                        stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_T_Class steel =
                       stb.StbModel.StbSections.StbSecSteel.StbSecRoll_T[shapeidY];

                logtxt = Roll_T_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                CT_A = steel.A;
                CT_B = steel.B;
                CT_t1 = steel.t1;
                CT_t2 = steel.t2;
                CT_r = steel.r;
                typeT = steel.type;

            }
            switch (shape_T.direction_type)
            {
                case "T1":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
                    break;
                case "T3":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 270 * Math.PI / 180);
                    break;
                case "T4":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 180 * Math.PI / 180);
                    break;
            }
            SetParameter(symbol.LookupParameter(Rclm.H), H, true);
            SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            SetParameter(symbol.LookupParameter(Rclm.r), r, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_A), CT_A, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_B), CT_B, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_t1), CT_t1, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_t2), CT_t2, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_r), CT_r, true);
            SetParameter(symbol.LookupParameter(Rclm.type_H), typeH);
            SetParameter(symbol.LookupParameter(Rclm.type_T), typeT);
            SetParameter(symbol.LookupParameter(Rclm.direction_type), shape_T.direction_type);
            SetParameter(symbol.LookupParameter(Rclm.typename_H), shape_T.shape_H);
            SetParameter(symbol.LookupParameter(Rclm.typename_T), shape_T.shape_T);
            SetParameter(symbol.LookupParameter(Rclm.strength_main_H), shape_T.strength_main_H);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_H), GetStrength_web(shape_T.strength_web_H, shape_T.strength_main_H));
            SetParameter(symbol.LookupParameter(Rclm.strength_main_T), shape_T.strength_main_T);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_T), GetStrength_web(shape_T.strength_web_T, shape_T.strength_main_T));
            SetParameter(symbol.LookupParameter(Rclm.offset_HX), shape_T.offset_HX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_HY), shape_T.offset_HY, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_T), shape_T.offset_T, true);

            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);
            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_2nd_main), clm.strength_reinforcement_2nd_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[0]), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X[1]), clm.depth_cover_end_X);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[0]), clm.depth_cover_start_Y);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_Y[1]), clm.depth_cover_end_Y);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[0]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[1]), clm.kind_reinforcement_corner);
            SetParameter(symbol.LookupParameter(Rclm.interval_reinforcement), clm.interval_reinforcement);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.DX), clm.StbSecFigure.StbSecRect.DX, true);
            SetParameter(symbol.LookupParameter(Rclm.DY), clm.StbSecFigure.StbSecRect.DY, true);

            double pitch_bar_spacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_Not_Same[j];
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                            clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecRect_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_Same;
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecRect_Column_XReinforcedClass x =
                           clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_2nd_main[j]), clm.D_reinforcement_2nd_main);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_X_1st[j]), bar.count_main_X_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_1st[j]), bar.count_2nd_main_X_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_X_2nd[j]), bar.count_main_X_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_X_2nd[j]), bar.count_2nd_main_X_2nd);
                        if (x != null)
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st + x.count_main_X); }
                        else
                        { SetParameter(symbol.LookupParameter(Rclm.count_main_Y_1st[j]), bar.count_main_Y_1st); }
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_1st[j]), bar.count_2nd_main_Y_1st);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_Y_2nd[j]), bar.count_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.count_2nd_main_Y_2nd[j]), bar.count_2nd_main_Y_2nd);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_X[j]), bar.count_band_dir_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_band_dir_Y[j]), bar.count_band_dir_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        SetParameter(symbol.LookupParameter(Rclm.kind_reinforcement_corner[j]), clm.kind_reinforcement_corner);
                        SetParameter(symbol.LookupParameter(Rclm.count_main_total), bar.count_main_total);
                        if (j == 0)
                        { pitch_bar_spacing_list = bar.pitch_bar_spacing; }
                    }
                }
                if (clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced != null)
                { SetParameter(symbol.LookupParameter(Rclm.count_main_total_X), clm.StbSecBar_Arrangement.StbSecRect_Column_XReinforced.count_main_total); }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_spacing_list, true);
            }
            return ret;
        }
        private bool CreateColumn_SRC_T_Rou(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm, FamilySymbol symbol)
        {
            bool ret = true;
            string logtxt = "";
            string shapename = "SRC柱T形断面鉄骨形状";

            FamilyStructure.SRC_Clm_T_Rou Rclm = SetFamily.SRCClmT_Rou;

            int shapeidX = 0, shapeidY = 0;

            STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class.StbSecColumn_SRC_ShapeT_Class shape_T = null;
            string shapetypeH = "", shapetypeT = "";
            for (int i = 0; i < clm.StbSecSteelColumn_SRC.Count(); i++)
            {
                if (clm.StbSecSteelColumn_SRC[i] != null)
                {
                    if (clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT != null)
                    {
                        shape_T = clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT;
                        shapetypeH = Check_Steel(stb, clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT.shape_H, ref shapeidX);
                        shapetypeT = Check_Steel(stb, clm.StbSecSteelColumn_SRC[i].StbSecColumn_SRC_ShapeT.shape_T, ref shapeidY);
                        if (shapetypeH != "" && shapetypeT != "")
                        { break; }
                    }
                }
            }
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                       stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeidX];

                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                H = steel.A;
                B = steel.B;
                t1 = steel.t1;
                t2 = steel.t2;
                r = steel.r;
                typeH = steel.type;
            }
            else if (shapetypeH == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                        stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeidX];

                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
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
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_T_Class steel =
                       stb.StbModel.StbSections.StbSecSteel.StbSecRoll_T[shapeidY];

                logtxt = Roll_T_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename, symbol.Name, clm.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    return false;
                }

                CT_A = steel.A;
                CT_B = steel.B;
                CT_t1 = steel.t1;
                CT_t2 = steel.t2;
                CT_r = steel.r;
                typeT = steel.type;

            }
            switch (shape_T.direction_type)
            {
                case "T1":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 90 * Math.PI / 180);
                    break;
                case "T3":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 270 * Math.PI / 180);
                    break;
                case "T4":
                    SetParameter(symbol.LookupParameter(Rclm.angle), 180 * Math.PI / 180);
                    break;
            }
            SetParameter(symbol.LookupParameter(Rclm.H), H, true);
            SetParameter(symbol.LookupParameter(Rclm.B), B, true);
            SetParameter(symbol.LookupParameter(Rclm.t1), t1, true);
            SetParameter(symbol.LookupParameter(Rclm.t2), t2, true);
            SetParameter(symbol.LookupParameter(Rclm.r), r, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_A), CT_A, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_B), CT_B, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_t1), CT_t1, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_t2), CT_t2, true);
            SetParameter(symbol.LookupParameter(Rclm.CT_r), CT_r, true);
            SetParameter(symbol.LookupParameter(Rclm.type_H), typeH);
            SetParameter(symbol.LookupParameter(Rclm.type_T), typeT);
            SetParameter(symbol.LookupParameter(Rclm.direction_type), shape_T.direction_type);
            SetParameter(symbol.LookupParameter(Rclm.typename_H), shape_T.shape_H);
            SetParameter(symbol.LookupParameter(Rclm.typename_T), shape_T.shape_T);
            SetParameter(symbol.LookupParameter(Rclm.strength_main_H), shape_T.strength_main_H);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_H), GetStrength_web(shape_T.strength_web_H, shape_T.strength_main_H));
            SetParameter(symbol.LookupParameter(Rclm.strength_main_T), shape_T.strength_main_T);
            SetParameter(symbol.LookupParameter(Rclm.strength_web_T), GetStrength_web(shape_T.strength_web_T, shape_T.strength_main_T));
            SetParameter(symbol.LookupParameter(Rclm.offset_HX), shape_T.offset_HX, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_HY), shape_T.offset_HY, true);
            SetParameter(symbol.LookupParameter(Rclm.offset_T), shape_T.offset_T, true);

            //コンクリート 
            //鉄筋径のチェック
            Get_D("SRC柱", ref clm.D_reinforcement_main, "主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_2nd_main, "副主筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_reinforcement_band, "帯筋", symbol.Name, clm.id);
            Get_D("SRC柱", ref clm.D_bar_spacing, "巾止筋", symbol.Name, clm.id);
            SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
            SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
            if (clm.kind_column == "COLUMN")
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
            else
            { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
            SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
            SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete);
            SetParameter(symbol.LookupParameter(Rclm.D_bar_spacing), clm.D_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_main), clm.strength_reinforcement_main);
            SetParameter(symbol.LookupParameter(Rclm.strength_reinforcement_band), clm.strength_reinforcement_band);
            SetParameter(symbol.LookupParameter(Rclm.strength_bar_spacing), clm.strength_bar_spacing);
            SetParameter(symbol.LookupParameter(Rclm.depth_cover_X), clm.depth_cover_start_X);
            SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
            SetParameter(symbol.LookupParameter(Rclm.D), clm.StbSecFigure.StbSecCircle.D, true);
            double pitch_bar_cpacing_list = 0;
            if (clm.StbSecBar_Arrangement != null)
            {
                if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same != null)
                {

                    for (int j = 0; j < clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same.Count(); j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_Not_SameClass bar =
                            clm.StbSecBar_Arrangement.StbSecCircle_Column_Not_Same[j];

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                else if (clm.StbSecBar_Arrangement.StbSecCircle_Column_Same != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecBar_ArrangementClass.StbSecCircle_Column_SameClass bar =
                           clm.StbSecBar_Arrangement.StbSecCircle_Column_Same;

                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_main[j]), clm.D_reinforcement_main);
                        SetParameter(symbol.LookupParameter(Rclm.count_main[j]), bar.count_main);
                        SetParameter(symbol.LookupParameter(Rclm.D_reinforcement_band[j]), clm.D_reinforcement_band);
                        SetParameter(symbol.LookupParameter(Rclm.count_band[j]), bar.count_band);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_band[j]), bar.pitch_band, true);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_X[j]), bar.count_bar_spacing_X);
                        SetParameter(symbol.LookupParameter(Rclm.count_bar_spacing_Y[j]), bar.count_bar_spacing_Y);
                        SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing[j]), bar.pitch_bar_spacing, true);
                        if (j == 0)
                        { pitch_bar_cpacing_list = bar.pitch_bar_spacing; }
                    }
                }
                SetParameter(symbol.LookupParameter(Rclm.pitch_bar_spacing_list), pitch_bar_cpacing_list, true);
            }

            return ret;
        }

        /// <summary>CFT柱タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateColumn_CFT(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecColumn_CFT clm, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            string typename = GetTypeName_Column(stb, clm.floor, clm.id, clm.name);

            //鉄骨形状を取得
            int shapeid = -1;
            string shapename = "";
            string shape = "";
            if(clm.StbSecSteelColumn_CFT[0] != null)
            {
                shape = Check_Steel(stb, clm.StbSecSteelColumn_CFT[0].shape, ref shapeid);
                shapename = clm.StbSecSteelColumn_CFT[0].shape;
            }
            else if(clm.StbSecSteelColumn_CFT[1] != null)
            {
                shape = Check_Steel(stb, clm.StbSecSteelColumn_CFT[1].shape, ref shapeid);
                shapename = clm.StbSecSteelColumn_CFT[1].shape;
            }
            else if (clm.StbSecSteelColumn_CFT[2] != null)
            {
                shape = Check_Steel(stb, clm.StbSecSteelColumn_CFT[2].shape, ref shapeid);
                shapename = clm.StbSecSteelColumn_CFT[2].shape;
            }

            FamilySymbol symbol = null;
            if (shape == RevitLNK.st_steel_Box || shape == RevitLNK.st_steel_BBox)
            {
                if (ConvFamily[3][0] == null) { ret = false; return ret; }

                if (!SearchFamilySymbol(ConvFamily[3][0], typename, ref symbol))
                { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                else
                {
                    typename = Data.ReName2(ConvFamily[3][0], typename);
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }


                FamilyStructure.CFT_Clm_Box Rclm = SetFamily.CFTClmBox;
                double B = 0, A = 0, t = 0, r1 = 0;
                string type = "";
                if (shape == RevitLNK.st_steel_Box)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_BOX_Class steel =
                        stb.StbModel.StbSections.StbSecSteel.StbSecRoll_BOX[shapeid];
                    B = steel.B;
                    A = steel.A;
                    t = steel.t;
                    r1 = steel.R;
                    type = steel.type;
                }
                else
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_BOX_Class steel =
                       stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX[shapeid];
                    B = steel.B;
                    A = steel.A;
                    t = steel.t1;                    
                }
                SetParameter(symbol.LookupParameter(Rclm.strength_main), clm.StbSecSteelColumn_CFT[0].strength_main);
                SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete, false, true);
                if (clm.kind_column == "COLUMN")
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
                else
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
                SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
                SetParameter(symbol.LookupParameter(Rclm.direction_type), clm.direction);               
                SetParameter(symbol.LookupParameter(Rclm.type), type);
                SetParameter(symbol.LookupParameter(Rclm.typename), shapename);
                SetParameter(symbol.LookupParameter(Rclm.B), B, true);
                SetParameter(symbol.LookupParameter(Rclm.A), A, true);
                SetParameter(symbol.LookupParameter(Rclm.t), t, true);
                SetParameter(symbol.LookupParameter(Rclm.r1), r1, true);
                SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
                SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
                SetParameter(symbol.LookupParameter(Rclm.enbedded_length), clm.enbedded_length, true);
            }
            else if (shape == RevitLNK.st_steel_Pipe)
            {
                if (ConvFamily[3][1] == null) { ret = false; return ret; }

                if (!SearchFamilySymbol(ConvFamily[3][1], typename, ref symbol))
                {
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }
                else
                {
                    typename = Data.ReName2(ConvFamily[3][1], typename);
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }


                FamilyStructure.CFT_Clm_Pipe Rclm = SetFamily.CFTClmPipe;

                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecPipe_Class steel =
                    stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];

                SetParameter(symbol.LookupParameter(Rclm.strength_main), clm.StbSecSteelColumn_CFT[0].strength_main);
                SetParameter(symbol.LookupParameter(Rclm.strength_concrete), clm.strength_concrete, false , true);
                if (clm.kind_column == "COLUMN")
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Column"); }
                else
                { SetParameter(symbol.LookupParameter(Rclm.kind_column), "Post"); }
                SetParameter(symbol.LookupParameter(Rclm.kind_column2), clm.kind_column);
                SetParameter(symbol.LookupParameter(Rclm.typename), shapename);
                SetParameter(symbol.LookupParameter(Rclm.D), steel.D, true);
                SetParameter(symbol.LookupParameter(Rclm.t), steel.t, true);
                SetParameter(symbol.LookupParameter(Rclm.name), clm.name);
                SetParameter(symbol.LookupParameter(Rclm.SecId), clm.id);
                SetParameter(symbol.LookupParameter(Rclm.base_type), clm.base_type);
                SetParameter(symbol.LookupParameter(Rclm.enbedded_length), clm.enbedded_length, true);
            }

            if (symbol != null)
            {
                CheckMultiFloor_Column(stb, clm.floor, clm.id, clm.name, symbol);
            }


            return ret;
        }

        /// <summary>柱インスタンスパラメータ設定（柱）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateColumn_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbColumn clm, int sclmind, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;            

            //2017/05/23 回転角が360度以上→-360度する
            if(clm.rotate >= 360) { clm.rotate = clm.rotate - 360; }

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            string floor = "";
            Family fami = null;
            string shape = "";
            int ind = 0;
            //タイプ名
            string typename = "";
            int mid = -1;
            string mid_name = "";
            switch (clm.kind_structure)
            {
                case "RC":
                    mid = section.StbSecColumns_RC[sclmind].id;                    
                    floor = section.StbSecColumns_RC[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_RC[sclmind].name);

                    if (section.StbSecColumns_RC[sclmind].StbSecFigure.StbSecFigureType == 1)
                    {
                        fami = ConvFamily[0][0];
                        mid_name = SetFamily.RCClmRe.SecId;
                    }
                    else
                    {
                        fami = ConvFamily[0][1];
                        mid_name = SetFamily.RCClmRo.SecId;
                    }
                    break;

                case "S":
                    mid = section.StbSecColumns_S[sclmind].id;
                    floor = section.StbSecColumns_S[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_S[sclmind].name);

                    shape = Check_Steel(stb, section.StbSecColumns_S[sclmind].StbSecSteelColumn[0].shape, ref ind);
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            mid_name = SetFamily.SClmH.SecId;
                            fami = ConvFamily[1][0];
                            break;
                        case RevitLNK.st_steel_BH:
                            mid_name = SetFamily.SClmBH.SecId;
                            fami = ConvFamily[1][1];
                            break;
                        case RevitLNK.st_steel_Box:
                            mid_name = SetFamily.SClmBox.SecId;
                            fami = ConvFamily[1][2];
                            break;
                        case RevitLNK.st_steel_BBox:
                            mid_name = SetFamily.SClmBBox.SecId;
                            fami = ConvFamily[1][3];
                            break;
                        case RevitLNK.st_steel_Pipe:
                            mid_name = SetFamily.SClmPipe.SecId;
                            fami = ConvFamily[1][4];
                            break;
                        case RevitLNK.st_steel_T:
                            mid_name = SetFamily.SClmT.SecId;
                            fami = ConvFamily[1][5];
                            break;
                        case RevitLNK.st_steel_C:
                            mid_name = SetFamily.SClmC.SecId;
                            fami = ConvFamily[1][6];
                            break;
                        case RevitLNK.st_steel_L:
                            mid_name = SetFamily.SClmL.SecId;
                            fami = ConvFamily[1][7];
                            break;
                        default:                            
                            return ret;
                    }
                    if(fami == null) { return ret; }
                    break;

                case "SRC":
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class src = section.StbSecColumns_SRC[sclmind].StbSecSteelColumn_SRC[0];
                    mid = section.StbSecColumns_SRC[sclmind].id;
                    floor = section.StbSecColumns_SRC[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_SRC[sclmind].name);

                    if (section.StbSecColumns_SRC[sclmind].StbSecFigure.StbSecFigureType == 1)
                    {
                        if (src.StbSecColumn_SRC_ShapeH != null)
                        {
                            mid_name = SetFamily.SRCClmH.SecId;
                            fami = ConvFamily[2][0];
                        }
                        else if (src.StbSecColumn_SRC_ShapeCross != null)
                        {
                            mid_name = SetFamily.SRCClmCross.SecId;
                            fami = ConvFamily[2][1];
                        }
                        else if (src.StbSecColumn_SRC_ShapeT != null)
                        {
                            mid_name = SetFamily.SRCClmT.SecId;
                            fami = ConvFamily[2][2];
                        }
                    }
                    else
                    {
                        if (src.StbSecColumn_SRC_ShapeH != null)
                        {
                            mid_name = SetFamily.SRCClmH_Rou.SecId;
                            fami = ConvFamily[2][3];
                        }
                        else if (src.StbSecColumn_SRC_ShapeCross != null)
                        {
                            mid_name = SetFamily.SRCClmCross_Rou.SecId;
                            fami = ConvFamily[2][4];
                        }
                        else if (src.StbSecColumn_SRC_ShapeT != null)
                        {
                            mid_name = SetFamily.SRCClmT_Rou.SecId;
                            fami = ConvFamily[2][5];
                        }
                    }
                    if(fami == null) { return ret; }
                    break;

                case "CFT":
                    mid = section.StbSecColumns_CFT[sclmind].id;
                    floor = section.StbSecColumns_CFT[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_CFT[sclmind].name);

                    shape = Check_Steel(stb, section.StbSecColumns_CFT[sclmind].StbSecSteelColumn_CFT[0].shape, ref ind);
                    switch (shape)
                    {
                        case RevitLNK.st_steel_Box:
                        case RevitLNK.st_steel_BBox:
                            mid_name = SetFamily.CFTClmBox.SecId;
                            fami = ConvFamily[3][0];
                            break;
                        case RevitLNK.st_steel_Pipe:
                            mid_name = SetFamily.CFTClmPipe.SecId;
                            fami = ConvFamily[3][1];
                            break;
                    }
                    if(fami == null) { return ret; }
                    break;
            }

            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, clm.idNode_bottom);
            Level btmLevel = null;
            int index = indb;
            do
            {
                btmLevel = SearchLevel(stb, index);
                index--;
                if(index < 0) { break; }
            } while (btmLevel == null);
            if(btmLevel == null)
            {
                index = indb;
                do
                {
                    btmLevel = SearchLevel(stb, index);
                    index++;
                    if (index == stb.StbModel.StbStories.Count()) { break; }
                } while (btmLevel == null);               
            }
            if(btmLevel == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")は基準レベルが取得できないため変換できません。");
                return ret;
            }

            int indt = Get_stbFloor_index(stb, clm.idNode_top);
            Level topLevel = null;
            index = indt;
            do
            {
                topLevel = SearchLevel(stb, index);
                index++;
                if(index == stb.StbModel.StbStories.Count()) { break; }
            } while (topLevel == null);
            if(topLevel == null)
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
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")は上部レベルが取得できないため変換できません。");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (!SearchFamilySymbol(fami, typename, ref symbol, mid, mid_name))
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得 
            XYZ Pt = new XYZ();
            XYZ Pb = new XYZ();
            if (clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0||
                clm.offset_top_X != 0 || clm.offset_top_Y != 0)
            {
                Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_top_X, clm.offset_top_Y, 0);
                Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, 0);
            }
            else
            {
                Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_X, clm.offset_Y, 0);
                Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_X, clm.offset_Y, 0);
            }      

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
            { Search_Girder_Offset_Z_bottom(stb, clm.idNode_bottom, btmLevel, clm.kind_structure, ref gir_offset_Z_bottom); }
            if (clm.offset_top_Z == 0) //柱頭Z方向オフセット値が0以外の時はその値を優先する
            { Search_Girder_Offset_Z_top(stb, clm.idNode_top, topLevel, ref gir_offset_Z_top); }

            XYZ Pt_offset = new XYZ(Pt.X, Pt.Y, Pt.Z + Commons.mm2ft( gir_offset_Z_top + clm.offset_top_Z));
            XYZ Pb_offset = new XYZ(Pb.X, Pb.Y, Pb.Z + Commons.mm2ft( gir_offset_Z_bottom + clm.offset_bottom_Z));
            double length = Commons.PointPointDist3D(Pt_offset, Pb_offset);
            if (length <= 1)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + clm.name + "(配置id=" + clm.id.ToString() + ")は長さが0mmのため変換できません。");
                return ret;
            }
            if(Pt_offset.Z < Pb_offset.Z)
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
                    if(symbol.FamilyName == "Steel_Column_Box" || symbol.FamilyName == "Steel_Column_Pipe") //このファミリで傾斜柱のときダイアフラムを非表示にする
                    {
                        SetParameter(instance.LookupParameter("Diaphragm"), false);
                    }             
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Pb, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);
                }

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ) 
                //回転 ※ラジアンに直して
                double rotate = clm.rotate * Math.PI / 180;
                if (clm.kind_structure == "S")
                {
                    if (stb.StbModel.StbSections.StbSecColumns_S[sclmind].direction)
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


                SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, topLevel.Id); //上部レベル

                double top = Pt.Z +  Commons.mm2ft(clm.offset_top_Z + gir_offset_Z_top + offset_t);
               

                if (offset_t <= 0 && offset_b <= 0)
                {                   
                    //柱脚レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                    //柱頭レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                }
                else
                {
                    //柱頭レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                    //柱脚レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                }


                //解析線分作成
                Commons.doc.Regenerate();
                XYZ Pb_org = Get_Node_Position(stb, clm.idNode_bottom, 0, 0, 0);
                XYZ Pt_org = Get_Node_Position(stb, clm.idNode_top, 0, 0, 0);
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

                SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_BASE_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle); //下部のカットスタイル
                SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_TOP_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle);  //上部のカットスタイル               

                //ver1.4の継手距離は節点からなので換算する必要がある
                double joint_t = 0;
                double joint_b = 0;
                if (Math.Abs(clm.joint_top) > 0.1) joint_t = clm.joint_top + (clm.offset_top_Z + gir_offset_Z_top + offset_t);
                if (Math.Abs(clm.joint_bottom) > 0.1) joint_b = clm.joint_bottom - (clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b);

                switch (clm.kind_structure)
                {
                    case "RC":
                        FamilyStructure.RC_Clm_Re RCclm = SetFamily.RCClmRe;
                        SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                        SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), clm.thickness_ex_start_X, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_X), clm.thickness_ex_end_X, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_Y), clm.thickness_ex_start_Y, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_Y), clm.thickness_ex_end_Y, true);
                        break;
                    case "S":
                        switch (shape)
                        {
                            case RevitLNK.st_steel_H:
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmH.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_BH:
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBH.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_Box:
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBox.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_BBox:
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmBBox.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_Pipe:
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmPipe.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_T:
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmT.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_C:
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmC.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                            case RevitLNK.st_steel_L:
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.MemId), clm.id);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.NameMembers), clm.name);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.condition_bottom), clm.condition_bottom);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.condition_top), clm.condition_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.joint_top), joint_t, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.joint_bottom), joint_b, true);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.kind_joint_top), clm.kind_joint_top);
                                SetParameter(instance.LookupParameter(SetFamily.SClmL.kind_joint_bottom), clm.kind_joint_bottom);
                                break;
                        }
                        break;
                    case "SRC":
                        if (stb.StbModel.StbSections.StbSecColumns_SRC[sclmind].StbSecFigure.StbSecFigureType == 1)
                        {
                            FamilyStructure.SRC_Clm_H SRCclm = SetFamily.SRCClmH;
                            SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_ex_start_X, true);
                            SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_X), clm.thickness_ex_end_X, true);
                            SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_Y), clm.thickness_ex_start_Y, true);
                            SetParameter(instance.LookupParameter(SRCclm.thickness_ex_end_Y), clm.thickness_ex_end_Y, true);
                            SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            SetParameter(instance.LookupParameter(SRCclm.joint_top), joint_t, true);
                            SetParameter(instance.LookupParameter(SRCclm.joint_bottom), joint_b, true);
                            SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        else
                        {
                            FamilyStructure.SRC_Clm_H_Rou SRCclm = SetFamily.SRCClmH_Rou;
                            SetParameter(instance.LookupParameter(SRCclm.MemId), clm.id);
                            SetParameter(instance.LookupParameter(SRCclm.NameMembers), clm.name);
                            SetParameter(instance.LookupParameter(SRCclm.thickness_ex_start_X), clm.thickness_ex_start_X, true);
                            SetParameter(instance.LookupParameter(SRCclm.condition_bottom), clm.condition_bottom);
                            SetParameter(instance.LookupParameter(SRCclm.condition_top), clm.condition_top);
                            SetParameter(instance.LookupParameter(SRCclm.joint_top), joint_t, true);
                            SetParameter(instance.LookupParameter(SRCclm.joint_bottom), joint_b, true);
                            SetParameter(instance.LookupParameter(SRCclm.kind_joint_top), clm.kind_joint_top);
                            SetParameter(instance.LookupParameter(SRCclm.kind_joint_bottom), clm.kind_joint_bottom);
                        }
                        break;
                    case "CFT":
                        FamilyStructure.CFT_Clm_Box CFTclm = SetFamily.CFTClmBox;
                        SetParameter(instance.LookupParameter(CFTclm.MemId), clm.id);
                        SetParameter(instance.LookupParameter(CFTclm.NameMembers), clm.name);
                        SetParameter(instance.LookupParameter(CFTclm.condition_bottom), clm.condition_bottom);
                        SetParameter(instance.LookupParameter(CFTclm.condition_top), clm.condition_top);
                        SetParameter(instance.LookupParameter(CFTclm.joint_top), joint_t, true);
                        SetParameter(instance.LookupParameter(CFTclm.joint_bottom), joint_b, true);
                        SetParameter(instance.LookupParameter(CFTclm.kind_joint_top), clm.kind_joint_top);
                        SetParameter(instance.LookupParameter(CFTclm.kind_joint_bottom), clm.kind_joint_bottom);
                        break;
                }

                //変換情報ログの出力
                var nodeIds = new int[] { clm.idNode_bottom, clm.idNode_top } ;
                MakeNodeLog( "柱の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, clm.id, "柱", typename, nodeIds );
            }
            catch (Exception)
            {
                ret = false;
            }
            

            return ret;
        }

        /// <summary> 柱インスタンスパラメータ設定（間柱）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreatePost_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbPost clm, int sclmind, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //2017/05/23 回転角が360度以上→-360度する
            if (clm.rotate >= 360) { clm.rotate = clm.rotate - 360; }

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            string floor = "";
            Family fami = null;
            string shape = "";
            int ind = 0;
            //タイプ名
            string typename = "";
            int mid = -1;
            string mid_name = "";
            switch (clm.kind_structure)
            {
                case "RC":
                    mid = section.StbSecColumns_RC[sclmind].id;
                    floor = section.StbSecColumns_RC[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_RC[sclmind].name);

                    if (section.StbSecColumns_RC[sclmind].StbSecFigure.StbSecFigureType == 1)
                    {
                        fami = ConvFamily[0][0];
                        mid_name = SetFamily.RCClmRe.SecId;
                    }
                    else
                    {
                        fami = ConvFamily[0][1];
                        mid_name = SetFamily.RCClmRo.SecId;
                    }
                    break;

                case "S":
                    mid = section.StbSecColumns_S[sclmind].id;
                    floor = section.StbSecColumns_S[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_S[sclmind].name);

                    shape = Check_Steel(stb, section.StbSecColumns_S[sclmind].StbSecSteelColumn[0].shape, ref ind);
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            mid_name = SetFamily.SClmH.SecId;
                            fami = ConvFamily[1][0];
                            break;
                        case RevitLNK.st_steel_BH:
                            mid_name = SetFamily.SClmBH.SecId;
                            fami = ConvFamily[1][1];
                            break;
                        case RevitLNK.st_steel_Box:
                            mid_name = SetFamily.SClmBox.SecId;
                            fami = ConvFamily[1][2];
                            break;
                        case RevitLNK.st_steel_BBox:
                            mid_name = SetFamily.SClmBBox.SecId;
                            fami = ConvFamily[1][3];
                            break;
                        case RevitLNK.st_steel_Pipe:
                            mid_name = SetFamily.SClmPipe.SecId;
                            fami = ConvFamily[1][4];
                            break;
                        case RevitLNK.st_steel_T:
                            mid_name = SetFamily.SClmT.SecId;
                            fami = ConvFamily[1][5];
                            break;
                        case RevitLNK.st_steel_C:
                            mid_name = SetFamily.SClmC.SecId;
                            fami = ConvFamily[1][6];
                            break;
                        case RevitLNK.st_steel_L:
                            mid_name = SetFamily.SClmL.SecId;
                            fami = ConvFamily[1][7];
                            break;
                        default:
                            return ret;
                    }
                    if (fami == null) { return ret; }
                    break;

                case "SRC":
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC.StbSecSteelColumn_SRC_Class src = section.StbSecColumns_SRC[sclmind].StbSecSteelColumn_SRC[0];
                    mid = section.StbSecColumns_SRC[sclmind].id;
                    floor = section.StbSecColumns_SRC[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_SRC[sclmind].name);

                    if (section.StbSecColumns_SRC[sclmind].StbSecFigure.StbSecFigureType == 1)
                    {
                        if (src.StbSecColumn_SRC_ShapeH != null)
                        {
                            mid_name = SetFamily.SRCClmH.SecId;
                            fami = ConvFamily[2][0];
                        }
                        else if (src.StbSecColumn_SRC_ShapeCross != null)
                        {
                            mid_name = SetFamily.SRCClmCross.SecId;
                            fami = ConvFamily[2][1];
                        }
                        else if (src.StbSecColumn_SRC_ShapeT != null)
                        {
                            mid_name = SetFamily.SRCClmT.SecId;
                            fami = ConvFamily[2][2];
                        }
                    }
                    else
                    {
                        if (src.StbSecColumn_SRC_ShapeH != null)
                        {
                            mid_name = SetFamily.SRCClmH_Rou.SecId;
                            fami = ConvFamily[2][3];
                        }
                        else if (src.StbSecColumn_SRC_ShapeCross != null)
                        {
                            mid_name = SetFamily.SRCClmCross_Rou.SecId;
                            fami = ConvFamily[2][4];
                        }
                        else if (src.StbSecColumn_SRC_ShapeT != null)
                        {
                            mid_name = SetFamily.SRCClmT_Rou.SecId;
                            fami = ConvFamily[2][5];
                        }
                    }
                    if (fami == null) { return ret; }
                    break;

                case "CFT":
                    mid = section.StbSecColumns_CFT[sclmind].id;
                    floor = section.StbSecColumns_CFT[sclmind].floor;
                    typename = GetTypeName_Column(stb, floor, mid, section.StbSecColumns_CFT[sclmind].name);

                    shape = Check_Steel(stb, section.StbSecColumns_CFT[sclmind].StbSecSteelColumn_CFT[0].shape, ref ind);
                    switch (shape)
                    {
                        case RevitLNK.st_steel_Box:
                        case RevitLNK.st_steel_BBox:
                            mid_name = SetFamily.CFTClmBox.SecId;
                            fami = ConvFamily[3][0];
                            break;
                        case RevitLNK.st_steel_Pipe:
                            mid_name = SetFamily.CFTClmPipe.SecId;
                            fami = ConvFamily[3][1];
                            break;
                    }
                    if (fami == null) { return ret; }
                    break;
            }

            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, clm.idNode_bottom);
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

            int indt = Get_stbFloor_index(stb, clm.idNode_top);
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
                LogData.AddLog(LogData.LogKind.Warning, 0, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")は上部レベルが取得できないため変換できません。");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (!SearchFamilySymbol(fami, typename, ref symbol, mid, mid_name))
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得 
            XYZ Pt = new XYZ();
            XYZ Pb = new XYZ();
            if (clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0 ||
                clm.offset_top_X != 0 || clm.offset_top_Y != 0)
            {
                Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_top_X, clm.offset_top_Y, 0);
                Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, 0);
            }
            else
            {
                Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_X, clm.offset_Y, 0);
                Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_X, clm.offset_Y, 0);
            }

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
            { Search_Girder_Offset_Z_bottom(stb, clm.idNode_bottom, btmLevel, clm.kind_structure, ref gir_offset_Z_bottom); }
            if (clm.offset_top_Z == 0) //柱頭Z方向オフセット値が0以外の時はその値を優先する
            { Search_Girder_Offset_Z_top(stb, clm.idNode_top, topLevel, ref gir_offset_Z_top); }

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
            if (Math.Abs(Pt.X - Pb.X) > gosa || Math.Abs(Pt.Y - Pb.Y) > gosa)
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
                        SetParameter(instance.LookupParameter("Diaphragm"), false);
                    }
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Pb, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);
                }

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ) 
                //回転 ※ラジアンに直して
                double rotate = clm.rotate * Math.PI / 180;
                if (clm.kind_structure == "S")
                {
                    if (stb.StbModel.StbSections.StbSecColumns_S[sclmind].direction)
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

                SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, topLevel.Id); //上部レベル

                double top = Pt.Z + Commons.mm2ft(clm.offset_top_Z + gir_offset_Z_top + offset_t);


                if (offset_t <= 0 && offset_b <= 0)
                {
                    //柱脚レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                    //柱頭レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                }
                else
                {
                    //柱頭レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_top_Z + gir_offset_Z_top + offset_t, true);
                    //柱脚レベル
                    SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b, true);
                }

                //解析線分作成
                Commons.doc.Regenerate();
                XYZ Pb_org = Get_Node_Position(stb, clm.idNode_bottom, 0, 0, 0);
                XYZ Pt_org = Get_Node_Position(stb, clm.idNode_top, 0, 0, 0);
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

                SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_BASE_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle); //下部のカットスタイル
                SetParameter(instance, BuiltInParameter.SLANTED_COLUMN_TOP_CUT_STYLE, SlantedOrVerticalColumnType.CT_Angle);  //上部のカットスタイル               

                //ver1.4の継手距離は節点からなので換算する必要がある
                double joint_t = 0;
                double joint_b = 0;
                if (Math.Abs(clm.joint_top) > 0.1) joint_t = clm.joint_top + (clm.offset_top_Z + gir_offset_Z_top + offset_t);
                if (Math.Abs(clm.joint_bottom) > 0.1) joint_b = clm.joint_bottom - (clm.offset_bottom_Z + gir_offset_Z_bottom + offset_b);

                switch (clm.kind_structure)
                {
                    case "RC":
                        FamilyStructure.RC_Clm_Re RCclm = SetFamily.RCClmRe;
                        SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                        SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), clm.thickness_ex_start_X, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_X), clm.thickness_ex_end_X, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_Y), clm.thickness_ex_start_Y, true);
                        SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_Y), clm.thickness_ex_end_Y, true);
                        break;
                    case "S":
                        //インスタンスはH形・山形・溝型ともに同じパラメータ
                        FamilyStructure.S_Clm_H Sclm = SetFamily.SClmH;
                        SetParameter(instance.LookupParameter(Sclm.MemId), clm.id);
                        SetParameter(instance.LookupParameter(Sclm.NameMembers), clm.name);
                        SetParameter(instance.LookupParameter(Sclm.condition_bottom), clm.condition_bottom);
                        SetParameter(instance.LookupParameter(Sclm.condition_top), clm.condition_top);
                        SetParameter(instance.LookupParameter(Sclm.joint_top), joint_t, true);
                        SetParameter(instance.LookupParameter(Sclm.joint_bottom), joint_b, true);
                        SetParameter(instance.LookupParameter(Sclm.kind_joint_top), clm.kind_joint_top);
                        SetParameter(instance.LookupParameter(Sclm.kind_joint_bottom), clm.kind_joint_bottom);
                        break;
                    case "SRC":
                        break;
                    case "CFT":
                        FamilyStructure.CFT_Clm_Box CFTclm = SetFamily.CFTClmBox;
                        SetParameter(instance.LookupParameter(CFTclm.MemId), clm.id);
                        SetParameter(instance.LookupParameter(CFTclm.NameMembers), clm.name);
                        SetParameter(instance.LookupParameter(CFTclm.condition_bottom), clm.condition_bottom);
                        SetParameter(instance.LookupParameter(CFTclm.condition_top), clm.condition_top);
                        SetParameter(instance.LookupParameter(CFTclm.joint_top), joint_t, true);
                        SetParameter(instance.LookupParameter(CFTclm.joint_bottom), joint_b, true);
                        SetParameter(instance.LookupParameter(CFTclm.kind_joint_top), clm.kind_joint_top);
                        SetParameter(instance.LookupParameter(CFTclm.kind_joint_bottom), clm.kind_joint_bottom);
                        break;
                }

                //変換情報ログの出力
                var nodeIds = new int[] { clm.idNode_bottom, clm.idNode_top } ;
                MakeNodeLog( "間柱の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, clm.id, "間柱", typename, nodeIds );
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;

        }

        /// <summary>柱インスタンスパラメータ設定（基礎柱）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateFoundationColumn_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbFoundationColumn clm, int sclmind, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            Family fami = null;
            //タイプ名
            string typename = GetTypeName_Column(stb, section.StbSecColumns_RC[sclmind].floor, section.StbSecColumns_RC[sclmind].id, section.StbSecColumns_RC[sclmind].name);

            if (section.StbSecColumns_RC[sclmind].StbSecFigure.StbSecFigureType == 1)
            { fami = ConvFamily[0][0]; }
            else
            { fami = ConvFamily[0][1]; }
           

            //配置レベルの取得           
            int indt = Get_stbFloor_index(stb, clm.idNode);
            Level bottomLevel = SearchLevel(stb,indt);
            Level topLevel = SearchLevel(stb,indt+1);

            //基準レベルのオフセットの設定（基礎柱高さ）
            double height = 0;
             height = clm.length; 

            //配置座標の取得
            XYZ Pt = Get_Node_Position(stb, clm.idNode, clm.offset_X, clm.offset_Y, clm.offset_Z);
            XYZ Pb = Get_Node_Position(stb, clm.idNode, clm.offset_X, clm.offset_Y, -height + clm.offset_Z);

            if(clm.length == 0 && clm.offset_Z == 0)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 0, "[基礎柱]" + clm.name + "(id=" + clm.id.ToString() + ")は基礎柱高さが0mmのため変換できません。");
                return ret;
            }
           

            //ファミリがロードされているか           
            if (fami == null)
            {
                LogData.AddLog(LogData.LogKind.Warning, 2100, clm.kind_structure + "柱");
                return ret;
            }
            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (!SearchFamilySymbol(fami, typename, ref symbol))
            {
                //ログ表示(タイプが無い)⇒RC柱の変換が行われていないかも
                CreateColumn_RC(stb, stb.StbModel.StbSections.StbSecColumns_RC[sclmind], pform, ConvFamily);
                if (!SearchFamilySymbol(fami, typename, ref symbol))
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + clm.kind_structure + "柱]" + typename + "(配置Id=" + clm.id.ToString() + ")");
                    return ret;
                }
            }


            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;

                //stbで指定されている座標は柱頭→とりあえず柱頭から上に柱を生成
                instance = Commons.doc.Create.NewFamilyInstance(Pt, symbol, bottomLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);
                //上部レベルを基点レベルと同じレベルに設定
                SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_PARAM, bottomLevel.Id);

                //基準レベルのオフセット
                SetParameter(instance, BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM, -clm.length, true);
                SetParameter(instance, BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM, clm.offset_Z, true);

                //回転 ※ラジアンに直して
                instance.Location.Rotate(Line.CreateBound(Pb, Pb + (Pt - Pb).Normalize()), (clm.rotate * Math.PI) / 180);


                FamilyStructure.RC_Clm_Re RCclm = SetFamily.RCClmRe;
                SetParameter(instance.LookupParameter(RCclm.MemId), clm.id);
                SetParameter(instance.LookupParameter(RCclm.NameMembers), clm.name);
                SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_X), clm.thickness_ex_start_X);
                SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_X), clm.thickness_ex_end_X);
                SetParameter(instance.LookupParameter(RCclm.thickness_ex_start_Y), clm.thickness_ex_start_Y);
                SetParameter(instance.LookupParameter(RCclm.thickness_ex_end_Y), clm.thickness_ex_end_Y);

                //変換情報ログの出力
                var nodeIds = new int[] { clm.idNode } ;
                MakeNodeLog( "基礎柱の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, clm.id, "基礎柱", typename, nodeIds );
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }
        #endregion
        #region 梁
        /// <summary> 梁の生成 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <returns></returns>
        private bool CreateGirder(STBclass stb, ProgressBarForm pform, string buzai, IList<Element> elements, ref string errmsg)
        {
            bool ret = true;

            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            string kind = "";
            bool isCanti = false;
            switch(buzai)
            {
                case "大梁":
                    kind = "GIRDER";
                    break;
                case "小梁":
                    kind = "BEAM";
                    break;
                case "片持梁":
                    kind = "GIRDER";
                    isCanti = true;
                    break;
                case "片持小梁":
                    kind = "BEAM";
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

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            //IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            ProgressBar_Show(pform, buzai + "の生成");
            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }
                        if (buzai == "大梁")
                        {
                            if (!SetFamily.GirFName.flg[i][j]) { continue; }
                            if (!SetFamily.GirFName.convflg[i][j]) { continue; }

                            if (familysymbol.FamilyName == SetFamily.GirFName.FamilyName[i][j])
                            {
                               
                                ConvFamily[i][j] = familysymbol.Family;

                                //プログレスバーの表示
                                GaugePercent("パラメータの追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                               
                                try
                                {
                                    tran1.Start();
                                   
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.GirFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
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

                                //プログレスバーの表示
                                GaugePercent("パラメータの追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                  
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.BeamFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
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

                                //プログレスバーの表示
                                GaugePercent("パラメータの追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                   
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.CGirFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
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

                                //プログレスバーの表示
                                GaugePercent("パラメータの追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(familysymbol.Family);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                   
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.CBeamFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
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
                List<TypeName_Data> typename_list = new List<TypeName_Data>();
                tran.Start();
              
                //梁タイプパラメータの設定
                //RC梁
                if (stb.StbModel.StbSections.StbSecBeams_RC != null) //Gir[0][0](RC梁)
                {
                    int numCount = stb.StbModel.StbSections.StbSecBeams_RC.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC gir = stb.StbModel.StbSections.StbSecBeams_RC[i];
                        if (gir.isCanti != isCanti) { continue; }
                        if (kind != gir.kind_beam) { continue; }

                        //プログレスバーの表示
                        GaugePercent("RC梁の生成", (int)((double)i / (double)numCount * 100));


                        if (gir.isCanti == true)
                        {
                            if (!CreateCGirder_RC(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                        else
                        {
                            if (!CreateGirder_RC(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                    }
                }
                //S梁
                if (stb.StbModel.StbSections.StbSecBeams_S != null) //Gir[1][0](H形),Gir[1][1](組立H形),Gir[1][2](溝形),Gir[1][3](山形),Gir[1][4](リップ溝形)
                {
                    int numCount = stb.StbModel.StbSections.StbSecBeams_S.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir = stb.StbModel.StbSections.StbSecBeams_S[i];
                        if (gir.isCanti != isCanti) { continue; }
                        if (kind != gir.kind_beam) { continue; }

                        //プログレスバーの表示
                        GaugePercent("S梁の生成", (int)((double)i / (double)numCount * 100));

                        if (gir.isCanti == true)
                        {
                            if (!CreateCGirder_S(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                        else
                        {
                            if (!CreateGirder_S(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                    }
                }
                //SRC梁
                if (stb.StbModel.StbSections.StbSecBeams_SRC != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecBeams_SRC.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC gir = stb.StbModel.StbSections.StbSecBeams_SRC[i];
                        if (gir.isCanti != isCanti) { continue; }                       
                        if (kind != gir.kind_beam) { continue; }

                        //プログレスバーの表示
                        GaugePercent("SRC梁の生成", (int)((double)i / (double)numCount * 100));

                        if (gir.isCanti == true)
                        {
                            if (!CreateCGirder_SRC(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                        else
                        {
                            if (!CreateGirder_SRC(stb, gir, pform, ConvFamily, ref typename_list)) { ret = false; }
                        }
                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                pform.TopMost = true;

                //梁インスタンスパラメータの設定 
                if (kind != "BEAM")
                {
                    if (stb.StbModel.StbMembers.StbGirders != null)
                    {
                        int numCount = stb.StbModel.StbMembers.StbGirders.Count();

                        for (int i = 0; i < numCount; i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                            bool cantiflg = false;
                            int sgirind = Get_SectionGirder(stb, gir.id_section, gir.kind_structure, ref cantiflg);
                            if (sgirind == -1) { continue; }
                            if (cantiflg != isCanti) { continue; }
                            //プログレスバーの表示
                            GaugePercent(buzai + "の生成", (int)((double)i / (double)numCount * 100));

                            if (!CreateGirder_instance(stb, gir, sgirind, pform, ConvFamily, ConvFamily)) { ret = false; }
                        }
                    }
                }
                if (kind != "GIRDER")
                {
                    if (stb.StbModel.StbMembers.StbBeams != null)
                    {
                        int numCount = stb.StbModel.StbMembers.StbBeams.Count();

                        for (int i = 0; i < numCount; i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbBeam gir = stb.StbModel.StbMembers.StbBeams[i];
                            bool cantiflg = false;
                            int sgirind = Get_SectionGirder(stb, gir.id_section, gir.kind_structure, ref cantiflg);
                            if (sgirind == -1) { continue; }
                            if (cantiflg != isCanti) { continue; }

                            //プログレスバーの表示
                            GaugePercent(buzai + "の生成", (int)((double)i / (double)numCount * 100));


                            if (!CreateBeam_instance(stb, gir, sgirind, pform, ConvFamily, ConvFamily)) { ret = false; }
                        }
                    }
                }
                pform.TopMost = false;
                tran.Commit();
                pform.TopMost = true;

                IList<Element> elements_end = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                for (int i = 0; i < typename_list.Count(); i++)
                {
                    bool flg = false;
                    foreach (Element el in elements_end)
                    {
                        FamilySymbol symbol = el as FamilySymbol;
                        if (symbol == null) { continue; }
                        if (symbol.Name == typename_list[i].typename)
                        {
                            flg = true;
                            break;
                        }
                    }
                    if (!flg)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + typename_list[i].shapename + "]" + typename_list[i].typename +
                                       "(断面id=" + typename_list[i].id.ToString() + ")を生成できませんでした。寸法値またはファミリの設定を確認してください。");
                    }

                }
            }
            catch (Exception e)
            {
                e.ToString();
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }


            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }

            if (ret == false)
            {
                errmsg = buzai;
            }
            return ret;
        }
       
        /// <summary> RC梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateGirder_RC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
        {
            bool ret = true;
            //変換に使用するファミリ
            Family fami = null;
            //ログ用部材名
            string logbuzai = "";
            string kind = "";
            if (gir.kind_beam == "GIRDER")
            { kind = "大梁"; }
            else { kind = "小梁"; }
            //タイプ名            
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                {
                    //typename = (find + 1).ToString();
                    typename = stb.StbModel.StbStories[find].name;
                }
            }
            typename += gir.name;

            if (!gir.isCanti)
            {
                switch (gir.StbSecFigure.StbSecFigureType) //2016/11/07ファミリを詳細化⇒ハンチ付か3断面同一かを判断する
                {
                    case 1:
                        if (gir.StbSecBar_Arrangement == null) //2017/09/14 鉄筋情報が無い場合は全断面として変換
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
                            if (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType == 1)
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
                        if (gir.StbSecFigure.StbSecTaper.depth_start != gir.StbSecFigure.StbSecTaper.depth_end ||
                            gir.StbSecFigure.StbSecTaper.width_start != gir.StbSecFigure.StbSecTaper.width_end)
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
                        if (gir.StbSecFigure.StbSecHaunch.depth_start != gir.StbSecFigure.StbSecHaunch.depth_center ||
                            gir.StbSecFigure.StbSecHaunch.depth_end != gir.StbSecFigure.StbSecHaunch.depth_center ||
                            gir.StbSecFigure.StbSecHaunch.width_start != gir.StbSecFigure.StbSecHaunch.width_center ||
                            gir.StbSecFigure.StbSecHaunch.width_end != gir.StbSecFigure.StbSecHaunch.width_center)
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

            if(fami == null)
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
                if (SearchFamilySymbol(fami, typename, ref symbol))                
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(fami, typename, ref symbol));
                }

                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }
                else
                {
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[h];
                re.Length2 = haunch_end[h];
                re.BHaunch1 = kind_haunch_start[h];
                re.BHaunch2 = kind_haunch_end[h];
                re.symbol = symbol;
                GirderSymbols.Add(re);

                //2017/05/19 鉄筋タグが無いとき→ログ
                if(gir.StbSecBar_Arrangement == null)
                {
                    //ログ表示
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[" + logbuzai + "]" + typename + "(断面id=" + gir.id.ToString() + ")");
                }

                FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;

                //鉄筋径のチェック
                Get_D(logbuzai, ref gir.D_reinforcement_main, "主筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_reinforcement_2nd_main, "副主筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_stirrup, "あばら筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_reinforcement_web, "腹筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_bar_spacing, "巾止筋", typename, gir.id);

                SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.isFoundation) { canti += "Foundation-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }                
                SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);
                SetParameter(symbol.LookupParameter(Rgir.name), gir.name);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), gir.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), gir.strength_reinforcement_2nd_main);
                SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), gir.strength_stirrup);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), gir.strength_reinforcement_web);
                SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), gir.strength_bar_spacing);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.depth_cover_left);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.depth_cover_right);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.depth_cover_top);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.depth_cover_bottom);
                SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.interval_reinforcement);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.center_reinforcement_top);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.center_reinforcement_bottom);
                SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start[h], true);
                SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end[h], true);
                SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutIn);
                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start[h] == "DROP") 
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if(kind_haunch_end[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }
                SetParameter(symbol.LookupParameter(Rgir.bar_length_start), gir.bar_length_start, true);
                SetParameter(symbol.LookupParameter(Rgir.bar_length_end), gir.bar_length_end, true);

                
                for (int i = 0; i < 3; i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), gir.D_stirrup);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), gir.D_reinforcement_web);
                    SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), gir.D_bar_spacing);
                }
                if (gir.StbSecFigure != null)
                {

                    switch (gir.StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecStraight.depth, true);
                            break;
                        case 2:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecTaper.width_end, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecTaper.depth_end, true);
                            break;
                        case 3:
                            if (gir.StbSecFigure.StbSecHaunch.width_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_start = gir.StbSecFigure.StbSecHaunch.width_center; }
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecHaunch.width_start, true);
                            if (gir.StbSecFigure.StbSecHaunch.width_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_center = gir.StbSecFigure.StbSecHaunch.width_start; }
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecHaunch.width_center, true);
                            if(gir.StbSecFigure.StbSecHaunch.width_end ==0)
                            { gir.StbSecFigure.StbSecHaunch.width_end = gir.StbSecFigure.StbSecHaunch.width_center; }
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecHaunch.width_end, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_start = gir.StbSecFigure.StbSecHaunch.depth_center; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecHaunch.depth_start, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_center = gir.StbSecFigure.StbSecHaunch.depth_start; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecHaunch.depth_center, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_end == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_end = gir.StbSecFigure.StbSecHaunch.depth_center; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecHaunch.depth_end, true);
                            break;
                    }
                }

                if (gir.StbSecBar_Arrangement != null)
                {
                    int N_topX = 0;
                    int N_btmX = 0;
                    if (gir.StbSecBar_Arrangement.StbSecBeam_XReinforced != null)
                    {
                        N_topX = gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_top;
                        N_btmX = gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_bottom;
                    }

                    switch (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass secbar =
                                gir.StbSecBar_Arrangement.StbSecBeam_Same_Section;

                            for (int i = 0; i < 3; i++)
                            {
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), secbar.count_main_top_1st + N_topX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), secbar.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), secbar.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), secbar.count_main_bottom_1st + N_btmX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), secbar.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), secbar.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), secbar.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), secbar.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), secbar.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), secbar.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), secbar.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), secbar.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), secbar.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), secbar.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), secbar.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), secbar.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), secbar.pitch_bar_spacing, true);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Count(); i++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass sec3 =
                                    gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i];

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec3.count_main_top_1st + N_topX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec3.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec3.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec3.count_main_bottom_1st + N_btmX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec3.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec3.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec3.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec3.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec3.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec3.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec3.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec3.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec3.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec3.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec3.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec3.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec3.pitch_bar_spacing, true);

                            }
                            break;
                        case 3:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Count() + 1; i++)
                            {
                                if (i == 1) { continue; } //断面中央には値を入れない
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass sec2 = null;
                                if (i == 0)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[0]; }
                                else if (i == 2)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[1]; }

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec2.count_main_top_1st + N_topX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec2.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec2.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec2.count_main_bottom_1st + N_btmX);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec2.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec2.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec2.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec2.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec2.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec2.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec2.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec2.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec2.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec2.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec2.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec2.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec2.pitch_bar_spacing, true);
                            }
                            break;
                    }

                    //if (gir.StbSecBar_Arrangement.StbSecBeam_XReinforced != null)
                    //{
                    //    for (int i = 0; i < 3; i++)
                    //    {
                    //        Rgir.count_main_top_1st[i] = Rgir.count_main_top_1st[i] + gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_top;
                    //        Rgir.count_main_bottom_1st[i] = Rgir.count_main_bottom_1st[i] + gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_bottom;
                    //    }
                    //}
                }
                if (symbol != null)
                {
                    TypeName_Data td = new TypeName_Data();
                    td.typename = symbol.Name;
                    td.id = gir.id;
                    td.shapename = "RC梁";
                    typename_list.Add(td);
                }
            }
           
            return ret;
        }
        /// <summary> RC片持ち梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateCGirder_RC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
        {
            bool ret = true;
            Family fami = null;
            string logbuzai = "";
            string kind = "";
            if (gir.kind_beam == "GIRDER")
            { kind = "梁"; }
            else
            { kind = "小梁"; }

            //タイプ名
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                { 
                    //typename = (find + 1).ToString();
                    typename = stb.StbModel.StbStories[find].name;
                }
            }
            typename += gir.name;

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

            if (gir.StbSecFigure == null)
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
                if (SearchFamilySymbol(fami, typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(fami, typename, ref symbol));
                }

                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }
                else
                {
                    symbol = (FamilySymbol)symbol.Duplicate(typename);
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[h];
                re.Length2 = haunch_end[h];
                re.BHaunch1 = kind_haunch_start[h];
                re.BHaunch2 = kind_haunch_end[h];
                re.symbol = symbol;
                GirderSymbols.Add(re);


                FamilyStructure.RC_CGir Rgir = SetFamily.RCCGir;

                //鉄筋径のチェック
                Get_D(logbuzai, ref gir.D_reinforcement_main, "主筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_reinforcement_2nd_main, "副主筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_stirrup, "あばら筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_reinforcement_web, "腹筋", typename, gir.id);
                Get_D(logbuzai, ref gir.D_bar_spacing, "巾止筋", typename, gir.id);

                SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
                SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);
                SetParameter(symbol.LookupParameter(Rgir.name), gir.name);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), gir.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), gir.strength_reinforcement_2nd_main);
                SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), gir.strength_stirrup);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), gir.strength_reinforcement_web);
                SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), gir.strength_bar_spacing);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.depth_cover_left);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.depth_cover_right);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.depth_cover_top);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.depth_cover_bottom);
                SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.interval_reinforcement);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.center_reinforcement_top);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.center_reinforcement_bottom);
                SetParameter(symbol.LookupParameter(Rgir.bar_length_start), gir.bar_length_start);
                SetParameter(symbol.LookupParameter(Rgir.bar_length_end), gir.bar_length_end);
                SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start[h], true);
                SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end[h], true);
                SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutIn);
                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }
                for (int i = 0; i < 2; i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), gir.D_stirrup);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), gir.D_reinforcement_web);
                    SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), gir.D_bar_spacing);
                }


                switch (gir.StbSecFigure.StbSecFigureType)
                {
                    case 1:
                        SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecStraight.width, true);
                        SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecStraight.width, true);
                        SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecStraight.depth, true);
                        SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecStraight.depth, true);
                        break;
                    case 2:
                        SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecTaper.width_start, true);
                        SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecTaper.width_end, true);
                        SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecTaper.depth_start, true);
                        SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecTaper.depth_end, true);
                        break;
                    case 3:
                        if(gir.StbSecFigure.StbSecHaunch.width_start ==0)
                        { gir.StbSecFigure.StbSecHaunch.width_start = gir.StbSecFigure.StbSecHaunch.width_center; }
                        SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecHaunch.width_start, true);
                        if(gir.StbSecFigure.StbSecHaunch.width_end == 0)
                        { gir.StbSecFigure.StbSecHaunch.width_end = gir.StbSecFigure.StbSecHaunch.width_center; }
                        SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecHaunch.width_end, true);
                        if (gir.StbSecFigure.StbSecHaunch.depth_start == 0)
                        { gir.StbSecFigure.StbSecHaunch.depth_start = gir.StbSecFigure.StbSecHaunch.depth_center; }
                        SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecHaunch.depth_start, true);
                        if (gir.StbSecFigure.StbSecHaunch.depth_end == 0)
                        { gir.StbSecFigure.StbSecHaunch.depth_end = gir.StbSecFigure.StbSecHaunch.depth_center; }
                        SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecHaunch.depth_end, true);
                        return ret;
                }


                if (gir.StbSecBar_Arrangement == null)
                {
                    //ログ表示
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[" + logbuzai + "]" + typename + "(断面id=" + gir.id.ToString() + ")");
                }
                else
                {

                    switch (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass secbar =
                                gir.StbSecBar_Arrangement.StbSecBeam_Same_Section;

                            for (int i = 0; i < 2; i++)
                            {
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), secbar.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), secbar.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), secbar.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), secbar.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), secbar.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), secbar.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), secbar.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), secbar.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), secbar.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), secbar.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), secbar.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), secbar.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), secbar.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), secbar.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), secbar.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), secbar.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), secbar.pitch_bar_spacing, true);

                            }
                            break;
                        case 2:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Count(); i++)
                            {
                                if (i == 1) continue;
                                int n = 0;
                                if (i == 2) { n = 1; }
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass sec3 =
                                    gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i];

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[n]), sec3.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[n]), sec3.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[n]), sec3.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[n]), sec3.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[n]), sec3.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[n]), sec3.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[n]), sec3.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[n]), sec3.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[n]), sec3.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[n]), sec3.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[n]), sec3.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[n]), sec3.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[n]), sec3.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[n]), sec3.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[n]), sec3.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[n]), sec3.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[n]), sec3.pitch_bar_spacing, true);

                            }
                            break;
                        case 3:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Count(); i++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass sec2 =
                                    gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[i];

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec2.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec2.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec2.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec2.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec2.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec2.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec2.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec2.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec2.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec2.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec2.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec2.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec2.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec2.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec2.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec2.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec2.pitch_bar_spacing, true);

                            }
                            break;
                    }

                    //if (gir.StbSecBar_Arrangement.StbSecBeam_XReinforced != null)
                    //{
                    //    for (int i = 0; i < 3; i++)
                    //    {
                    //        Rgir.count_main_top_1st[i] = Rgir.count_main_top_1st[i] + gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_top;
                    //        Rgir.count_main_bottom_1st[i] = Rgir.count_main_bottom_1st[i] + gir.StbSecBar_Arrangement.StbSecBeam_XReinforced.count_main_bottom;
                    //    }
                    //}
                }

                if (symbol != null)
                {
                    TypeName_Data td = new TypeName_Data();
                    td.typename = symbol.Name;
                    td.id = gir.id;
                    td.shapename = "RC片持梁";
                    typename_list.Add(td);
                }
            }
            return ret;
        }

        /// <summary> S梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateGirder_S(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
        {
            bool ret = true;
            
            //タイプ名
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                { typename = stb.StbModel.StbStories[find].name; }
            }
            typename += gir.name;

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            //鉄骨形状を取得
            int shapeid = -1;                    

            FamilySymbol symbol = null;

            int[] ind = new int[3];
            int[] shapeids = new int[3];
            for (int i = 0; i < gir.StbSecSteelBeam.Count(); i++)
            {
                if (i == 3 || i == 4) { continue; }
                if (gir.StbSecSteelBeam[i] == null || gir.StbSecSteelBeam[i].shape == "")
                {
                    switch (i)
                    {
                        case 0:
                            if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { ind[i] = 1; }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { ind[i] = 2; }
                            break;
                        case 1:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { ind[i] = 0; }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { ind[i] = 2; }
                            break;
                        case 2:
                            if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { ind[i] = 0; }
                            else if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { ind[i] = 1; }
                            break;
                    }
                }
                else
                { ind[i] = i; }
            }

            string shape = "";
            for(int i = 0; i < ind.Count(); i++)
            {
                if(shape == "")
                { shape = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]); }
                else
                {
                    string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]);
                    
                    //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                    if (shape != shape_)
                    {
                        if ((shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) || (shape_ == RevitLNK.st_steel_H || shape_ == RevitLNK.st_steel_BH)) { continue; }
                        MakeTekkotuLog("S梁", gir.name, gir.id);
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
                Make_taisyougaiLog("S梁", gir.id, gir.name, shape, shapename_J);
                return ret;
            }
            else
            {
                LogData.AddLog(LogData.LogKind.Warning, 2500, "[S梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")の鉄骨形状[" + gir.StbSecSteelBeam[ind[0]].shape + "]");
                return ret;
            }

            switch (shape)
            {
                case RevitLNK.st_steel_H:
                    bool shapeflg = true;
                    for (int i = 1; i < gir.StbSecSteelBeam.Count(); i++)
                    {
                        if (gir.StbSecSteelBeam[i] == null) { continue; }
                        if (gir.StbSecSteelBeam[0].shape != gir.StbSecSteelBeam[i].shape)
                        {
                            shapeflg = false;
                            break;
                        }
                    }
                    Family fami = null;
                    string logbuzai = "";
                    string kind = "";
                    if (gir.kind_beam == "GIRDER")
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
                    if (SearchFamilySymbol(fami, typename, ref symbol))
                    {
                        do
                        {
                            int ascii = 97;
                            typename = ReName(typename, ascii);
                            ascii++;
                        } while (SearchFamilySymbol(fami, typename, ref symbol));
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
                            string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[j]].shape, ref shapeids[j]);
                            if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, typename, haunch_start[i], haunch_end[i], shapeids[j], ind[j], j,
                                                                   shape_, gir, shapename_J, shapeflg)) { return false; }
                        }

                        FamilyStructure.S_Gir_H Rgir_H = SetFamily.SGirH;
                        SetParameter(symbol.LookupParameter(Rgir_H.SecId), gir.id);
                        string canti = "";
                        if (gir.isCanti)
                        { canti = "Cantilever-"; }
                        if (gir.kind_beam == "GIRDER")
                        { SetParameter(symbol.LookupParameter(Rgir_H.kind_beam), canti + "Girder"); }
                        else
                        { SetParameter(symbol.LookupParameter(Rgir_H.kind_beam), canti + "Beam"); }
                        SetParameter(symbol.LookupParameter(Rgir_H.kind_beam2), gir.kind_beam);
                        SetParameter(symbol.LookupParameter(Rgir_H.isOutIn), gir.isOutIn);
                        SetParameter(symbol.LookupParameter(Rgir_H.haunch_start), haunch_start[i], true);
                        SetParameter(symbol.LookupParameter(Rgir_H.haunch_end), haunch_end[i], true);
                        SetParameter(symbol.LookupParameter(Rgir_H.name), gir.name);
                        SetParameter(symbol.LookupParameter(Rgir_H.isOutIn), gir.isOutIn);

                        ReNameSymbols re = new ReNameSymbols();
                        re.name = typename;
                        re.id = gir.id;
                        re.Length = haunch_start[i];
                        re.Length2 = haunch_end[i];
                        re.BHaunch1 = kind_haunch_start[i];
                        re.BHaunch2 = kind_haunch_start[i];
                        re.symbol = symbol;
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
                    if (SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                    {
                        do
                        {
                            int ascii = 97;
                            typename = ReName(typename, ascii);
                            ascii++;
                        } while (SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol));
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
                            string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[j]].shape, ref shapeids[j]);
                            if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, typename, haunch_start[i], haunch_end[i], shapeids[j], ind[j], j,
                                                                   shape_, gir, shapename_J)) { return false; }
                        }

                        FamilyStructure.S_Gir_BH Rgir_BH = SetFamily.SGirBH;
                        SetParameter(symbol.LookupParameter(Rgir_BH.SecId), gir.id);
                        string canti = "";
                        if (gir.isCanti)
                        { canti = "Cantilever-"; }
                        if (gir.kind_beam == "GIRDER")
                        { SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam), canti + "Girder"); }
                        else
                        { SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam), canti + "Beam"); }
                        SetParameter(symbol.LookupParameter(Rgir_BH.name), gir.name);
                        SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam2), gir.kind_beam);
                        SetParameter(symbol.LookupParameter(Rgir_BH.haunch_start), haunch_start[i], true);
                        SetParameter(symbol.LookupParameter(Rgir_BH.haunch_end), haunch_end[i], true);
                        SetParameter(symbol.LookupParameter(Rgir_BH.SecId), gir.id);
                        SetParameter(symbol.LookupParameter(Rgir_BH.isOutIn), gir.isOutIn);


                        ReNameSymbols re = new ReNameSymbols();
                        re.name = typename;
                        re.id = gir.id;
                        re.Length = haunch_start[i];
                        re.Length2 = haunch_end[i];
                        re.BHaunch1 = kind_haunch_start[i];
                        re.BHaunch2 = kind_haunch_start[i];
                        re.symbol = symbol;
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
                    if (SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                    {
                        do
                        {
                            int ascii = 97;
                            typename = ReName(typename, ascii);
                            ascii++;
                        } while (SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                    {
                        if (ind[1] == 0)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steel_C =
                            stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C[shapeids[j]];

                        if (!SetParameter_Girder_and_CGirder_C(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_C, gir.StbSecSteelBeam[ind[j]], gir, shapename_J))
                        { return ret; }
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
                    if (SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                    {
                        do
                        {
                            int ascii = 97;
                            typename = ReName(typename, ascii);
                            ascii++;
                        } while (SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                    {
                        if (ind[1] == 0)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        Check_Steel(stb, gir.StbSecSteelBeam[j].shape, ref shapeid);
                        STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steel_L =
                            stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L[shapeids[j]];

                        if (!SetParameter_Girder_and_CGirder_L(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_L, gir.StbSecSteelBeam[ind[j]], gir, shapename_J))
                        { return ret; }
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
                    if (SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                    {
                        do
                        {
                            int ascii = 97;
                            typename = ReName(typename, ascii);
                            ascii++;
                        } while (SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol));
                    }
                    symbol = (FamilySymbol)symbol.Duplicate(typename);

                    //どの断面で変換したかログを出力
                    if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                    {
                        if (ind[1] == 0)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "始端"); }
                        else if (ind[1] == 1)
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "中央"); }
                        else
                        { MakeDanmenLog("S梁", typename, gir.id, shape, shapename_J, "終端"); }
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        Check_Steel(stb, gir.StbSecSteelBeam[j].shape, ref shapeid);
                        STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_LipC_Class steel_LipC =
                          stb.StbModel.StbSections.StbSecSteel.StbSecRoll_LipC[shapeids[j]];

                        SetParameter_Girder_and_CGirder_LipC(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_LipC, gir.StbSecSteelBeam[ind[j]], gir, shapename_J);
                    }
                    break;
            }
            if (symbol != null)
            {
                TypeName_Data td = new TypeName_Data();
                td.typename = symbol.Name;
                td.id = gir.id;
                td.shapename = "S梁";
                typename_list.Add(td);
            }
            return ret;
        }
        /// <summary> S片持ち梁タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateCGirder_S(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
        {
            bool ret = true;
            //タイプ名
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                {typename = stb.StbModel.StbStories[find].name;; }
            }
            typename += gir.name;

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            //鉄骨形状を取得
            int[] ind = new int[3];
            int[] shapeids = new int[3];

            for (int i = 0; i < gir.StbSecSteelBeam.Count(); i++)
            {
                if (i == 3 || i == 4) { continue; }
                if (gir.StbSecSteelBeam[i] == null || gir.StbSecSteelBeam[i].shape == "")
                {
                    switch (i)
                    {
                        case 0:
                            if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            {
                                ind[i] = 2;
                            }
                            else if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            {
                                ind[i] = 1;
                            }
                            break;
                        case 1:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            {
                                ind[i] = 0;
                            }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            {
                                ind[i] = 2;
                            }
                            break;
                        case 2:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            {
                                ind[i] = 0;
                            }
                            else if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            {
                                ind[i] = 1;
                            }
                            break;
                    }
                }
                else
                {
                    ind[i] = i;
                }
            }

            string shape = "";
            for (int i = 0; i < ind.Count(); i++)
            {
                if (shape == "")
                { shape = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]); }
                else
                {
                    string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]);

                    //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                    if (shape != shape_)
                    {
                        if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) { continue; }
                        MakeTekkotuLog("S片持梁", gir.name, gir.id);
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
            else if(shape != "")
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
                Make_taisyougaiLog("S片持梁", gir.id, gir.name, shape, shapename_J);
                return ret;
            }
            else
            {
                LogData.AddLog(LogData.LogKind.Warning, 2500, "[S片持梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")の鉄骨形状[" + gir.StbSecSteelBeam[ind[0]].shape + "]");
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
                if (SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

               

                for (int i = 0; i < haunch_start.Count(); i++)
                {

                    if (i != 0)
                    {
                        string newtypename = typename + "_" + i.ToString();
                        symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[j]].shape, ref shapeids[j]);
                        if (!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, typename, haunch_start[i], haunch_end[i], shapeids[j], ind[j], j,
                                                               shape_, gir, shapename_J)) { return false; }
                    }

                    FamilyStructure.S_CGir_H Rgir_H = SetFamily.SCGirH;
                    SetParameter(symbol.LookupParameter(Rgir_H.SecId), gir.id);
                    string canti = "";
                    if (gir.isCanti)
                    { canti = "Cantilever-"; }
                    if (gir.kind_beam == "GIRDER")
                    { SetParameter(symbol.LookupParameter(Rgir_H.kind_beam), canti + "Girder"); }
                    else
                    { SetParameter(symbol.LookupParameter(Rgir_H.kind_beam), canti + "Beam"); }
                    SetParameter(symbol.LookupParameter(Rgir_H.kind_beam2), gir.kind_beam);
                    SetParameter(symbol.LookupParameter(Rgir_H.isOutIn), gir.isOutIn);
                    SetParameter(symbol.LookupParameter(Rgir_H.haunch_start), haunch_start[i], true);
                    SetParameter(symbol.LookupParameter(Rgir_H.haunch_end), haunch_end[i], true);
                    SetParameter(symbol.LookupParameter(Rgir_H.name), gir.name);
                    SetParameter(symbol.LookupParameter(Rgir_H.isOutIn), gir.isOutIn);

                    ReNameSymbols re = new ReNameSymbols();
                    re.name = typename;
                    re.id = gir.id;
                    re.Length = haunch_start[i];
                    re.Length2 = haunch_end[i];
                    re.BHaunch1 = kind_haunch_start[i];
                    re.BHaunch2 = kind_haunch_start[i];
                    re.symbol = symbol;
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
                if (SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

               

                for (int i = 0; i < haunch_start.Count(); i++)
                {
                    if (i != 0)
                    {
                        typename = typename + "_" + i.ToString();
                        symbol = (FamilySymbol)symbol.Duplicate(typename);
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        string shape_ = Check_Steel(stb, gir.StbSecSteelBeam[ind[j]].shape, ref shapeids[j]);
                        if(!SetParameter_Girder_and_CGirder_HandBH(stb, symbol, typename, haunch_start[i], haunch_end[i], shapeids[j], ind[j], j,
                                                               shape_, gir, shapename_J)) { return false; }
                    }

                    FamilyStructure.S_CGir_H Rgir_BH = SetFamily.SCGirBH;
                    SetParameter(symbol.LookupParameter(Rgir_BH.SecId), gir.id);
                    string canti = "";
                    if (gir.isCanti)
                    { canti = "Cantilever-"; }
                    if (gir.kind_beam == "GIRDER")
                    { SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam), canti + "Girder"); }
                    else
                    { SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam), canti + "Beam"); }
                    SetParameter(symbol.LookupParameter(Rgir_BH.name), gir.name);
                    SetParameter(symbol.LookupParameter(Rgir_BH.kind_beam2), gir.kind_beam);
                    SetParameter(symbol.LookupParameter(Rgir_BH.haunch_start), haunch_start[i], true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.haunch_end), haunch_end[i], true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.SecId), gir.id);
                    SetParameter(symbol.LookupParameter(Rgir_BH.isOutIn), gir.isOutIn);


                    ReNameSymbols re = new ReNameSymbols();
                    re.name = typename;
                    re.id = gir.id;
                    re.Length = haunch_start[i];
                    re.Length2 = haunch_end[i];
                    re.BHaunch1 = kind_haunch_start[i];
                    re.BHaunch2 = kind_haunch_start[i];
                    re.symbol = symbol;
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
                if (SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                {
                    if (ind[1] == 0)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                for (int j = 0; j < 3; j++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steel_C =
                             stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C[shapeids[j]];
                    if (!SetParameter_Girder_and_CGirder_C(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_C, gir.StbSecSteelBeam[ind[j]], gir, shapename_J))
                    { return ret; }
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
                if (SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                {
                    if (ind[1] == 0)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                for (int j = 0; j < 3; j++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steel_L =
                         stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L[shapeids[j]];

                    if (!SetParameter_Girder_and_CGirder_L(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_L, gir.StbSecSteelBeam[ind[j]], gir, shapename_J))
                    { return ret; }
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

                if (SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                {
                    do
                    {
                        int ascii = 97;
                        typename = ReName(typename, ascii);
                        ascii++;
                    } while (SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol));
                }
                symbol = (FamilySymbol)symbol.Duplicate(typename);

                //どの断面で変換したかログを出力
                if (gir.StbSecSteelBeam[ind[0]].pos != "ALL")
                {
                    if (ind[1] == 0)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "始端"); }
                    else if (ind[1] == 1)
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "中央"); }
                    else
                    { MakeDanmenLog("S片持梁", typename, gir.id, shape, shapename_J, "終端"); }
                }

                FamilyStructure.S_Gir_LipC Rgir_Lip = SetFamily.SGirLipC;
                for (int j = 0; j < 3; j++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_LipC_Class steel_LipC =
                      stb.StbModel.StbSections.StbSecSteel.StbSecRoll_LipC[shapeids[j]];

                    if (!SetParameter_Girder_and_CGirder_LipC(j, symbol, typename, haunch_start, haunch_end, kind_haunch_start, kind_haunch_end, steel_LipC, gir.StbSecSteelBeam[ind[j]], gir, shapename_J))
                    { return ret; }
                }
            }
            if (symbol != null)
            {
                TypeName_Data td = new TypeName_Data();
                td.typename = symbol.Name;
                td.id = gir.id;
                td.shapename = "S片持梁";
                typename_list.Add(td);
            }
            return ret;
        }

        /// <summary> SRC梁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateGirder_SRC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
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
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                {typename = stb.StbModel.StbStories[find].name;; }
            }
            typename += gir.name;

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            //鉄骨形状を取得
            int[] ind = new int[3];
            int[] shapeids = new int[3]; 
            for (int i = 0; i < gir.StbSecSteelBeam.Count(); i++)
            {
                if (i == 3 || i == 4) { continue; }
                if (gir.StbSecSteelBeam[i] == null || gir.StbSecSteelBeam[i].shape == "")
                {
                    switch (i)
                    {
                        case 0:
                            if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { ind[i] = 1; }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { ind[i] = 2; }
                            break;
                        case 1:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { ind[i] = 0; }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { ind[i] = 2; }
                            break;
                        case 2:
                            if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { ind[i] = 0; }
                            else if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { ind[i] = 1; }
                            break;
                    }
                }
                else
                { ind[i] = i; }
            }

            string shape = "";
            for (int i = 0; i < ind.Count(); i++)
            {
                if (shape == "")
                { shape = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]); }
                else
                {
                    //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                    if (shape != Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]))
                    {
                        if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) { continue; }
                        MakeTekkotuLog("SRC梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }

            FamilySymbol symbol = null;
            if (SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol))
            {
                do
                {
                    int ascii = 97;
                    typename = ReName(typename, ascii);
                    ascii++;
                } while (SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol));
            }
            symbol = (FamilySymbol)symbol.Duplicate(typename);

            FamilyStructure.SRC_Gir Rgir = SetFamily.SRCGirH;
            for (int h = 0; h < haunch_start.Count(); h++)
            {
                if (h != 0)
                {
                    string newtypename = typename + "_" + h.ToString();
                    symbol = (FamilySymbol)symbol.Duplicate(newtypename);
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[h];
                re.Length2 = haunch_end[h];
                re.BHaunch1 = kind_haunch_start[h];
                re.BHaunch2 = kind_haunch_end[h];
                re.symbol = symbol;
                GirderSymbols.Add(re);
                switch (shape)
                {
                    case RevitLNK.st_steel_H:
                    case RevitLNK.st_steel_BH:

                        double steel_size = 0;
                        for (int j = 0; j < 3; j++)
                        {
                            var steel_H = stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H?.Find(a => a.name == gir.StbSecSteelBeam[ind[j]].shape);
                            if (steel_H != null)
                            {
                                string logtxt = Roll_H_Size_Check(steel_H);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("SRC梁" + shape, symbol.Name, gir.id, logtxt, 0);
                                    Commons.doc.Delete(symbol.Id);
                                    return ret;
                                }
                                SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind[j]].strength_web, gir.StbSecSteelBeam[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), gir.StbSecSteelBeam[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rgir.shape[j]), gir.StbSecSteelBeam[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_H.A, true);
                                SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_H.B, true);
                                SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_H.t1, true);
                                SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_H.t2, true);
                                SetParameter(symbol.LookupParameter(Rgir.type[j]), steel_H.type);

                                double r = steel_H.r;
                                if (steel_H.r < 1)
                                {
                                    r = 1;
                                    MakeSizeLog("SRC梁" + shape, symbol.Name, gir.id, "フィレット半径", 1);
                                }
                                SetParameter(symbol.LookupParameter(Rgir.r[j]), r, true);

                                if (j == 1)
                                {
                                    steel_size = steel_H.A;
                                }
                            }
                            else
                            {
                                var steel_BH = stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H?.Find(a => a.name == gir.StbSecSteelBeam[ind[j]].shape);

                                string logtxt = Build_H_Size_Check(steel_BH);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("SRC梁" + shape, symbol.Name, gir.id, logtxt, 0);
                                    Commons.doc.Delete(symbol.Id);
                                    return ret;
                                }
                                SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind[j]].strength_web, gir.StbSecSteelBeam[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), gir.StbSecSteelBeam[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rgir.shape[j]), gir.StbSecSteelBeam[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_BH.A, true);
                                SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_BH.B, true);
                                SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_BH.t1, true);
                                SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_BH.t2, true);
                                SetParameter(symbol.LookupParameter(Rgir.r[j]), 0.0, true);

                                if (j == 1)
                                {
                                    steel_size = steel_BH.A;
                                }
                            }
                        }

                        if (gir.level > 0 && steel_size > 1)
                        {
                            //1.4では通常≧0、プラス値が入っている
                            double rc_size = 0;
                            switch (gir.StbSecFigure.StbSecFigureType)
                            {
                                case 1:
                                    rc_size = gir.StbSecFigure.StbSecStraight.depth;
                                    break;

                                case 2:
                                    rc_size = gir.StbSecFigure.StbSecTaper.depth_start;
                                    break;

                                case 3:
                                    rc_size = gir.StbSecFigure.StbSecHaunch.depth_center;
                                    break;
                            }

                            //RCとSの寸法差
                            double d2 = (rc_size - steel_size) / 2;
                            //中心からの距離に換算（＋なら上に鉄骨が移動、－なら下に鉄骨が移動）
                            double d3 = d2 - gir.level;
                            SetParameter(symbol.LookupParameter(Rgir.level), d3, true);
                        }

                        SetParameter(symbol.LookupParameter(Rgir.offset), gir.offset, true);
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
                        Make_taisyougaiLog("SRC梁", gir.id, gir.name, shape, shapename_J);
                        return ret;
                }
                SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start[h], true);
                SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end[h], true);
                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }

                //RC部
                //鉄筋径のチェック
                Get_D("SRC梁", ref gir.D_reinforcement_main, "主筋", typename, gir.id);
                Get_D("SRC梁", ref gir.D_reinforcement_2nd_main, "副主筋", typename, gir.id);
                Get_D("SRC梁", ref gir.D_stirrup, "あばら筋", typename, gir.id);
                Get_D("SRC梁", ref gir.D_reinforcement_web, "腹筋", typename, gir.id);
                Get_D("SRC梁", ref gir.D_bar_spacing, "巾止筋", typename, gir.id);

                SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
                SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutIn);

                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
                SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);
                SetParameter(symbol.LookupParameter(Rgir.name), gir.name);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), gir.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), gir.strength_reinforcement_2nd_main);
                SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), gir.strength_stirrup);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), gir.strength_reinforcement_web);
                SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), gir.strength_bar_spacing);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.depth_cover_left);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.depth_cover_right);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.depth_cover_top);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.depth_cover_bottom);
                SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.interval_reinforcement);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.center_reinforcement_top);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.center_reinforcement_bottom);
                for (int i = 0; i < Rgir.D_reinforcement_main_top.Count(); i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), gir.D_reinforcement_2nd_main);

                }
                for (int i = 0; i < Rgir.D_stirrup.Count(); i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), gir.D_stirrup);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), gir.D_reinforcement_web);
                    SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), gir.D_bar_spacing);
                }
                if (gir.StbSecFigure != null)
                {

                    switch (gir.StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecStraight.depth, true);
                            break;
                        case 2:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecTaper.width_end, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecTaper.depth_end, true);
                            break;
                        case 3:
                            if (gir.StbSecFigure.StbSecHaunch.width_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_start = gir.StbSecFigure.StbSecHaunch.width_center; }                            
                            if (gir.StbSecFigure.StbSecHaunch.width_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_center = gir.StbSecFigure.StbSecHaunch.width_start; }                           
                            if (gir.StbSecFigure.StbSecHaunch.width_end == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_end = gir.StbSecFigure.StbSecHaunch.width_center; }                           
                            if (gir.StbSecFigure.StbSecHaunch.depth_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_start = gir.StbSecFigure.StbSecHaunch.depth_center; }                            
                            if (gir.StbSecFigure.StbSecHaunch.depth_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_center = gir.StbSecFigure.StbSecHaunch.depth_start; }                           
                            if (gir.StbSecFigure.StbSecHaunch.depth_end == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_end = gir.StbSecFigure.StbSecHaunch.depth_center; }

                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecHaunch.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecHaunch.width_center, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecHaunch.width_end, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecHaunch.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecHaunch.depth_center, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecHaunch.depth_end, true);
                            break;
                    }


                    Parameter p_height = symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT);
                    Parameter p_half = symbol.LookupParameter("Half");
                    if (p_height != null && p_half != null)
                    {
                        //SRC鉄骨の芯ずれ。Halfパラメータで中心位置を割り出しているみたい。
                        //数式がセットされていないので梁せい/2をいれておく。
                        //(高さは数式がセットされている)
                        SetParameter(p_half, p_height.AsDouble() / 2, false);
                    }
                }

                if (gir.StbSecBar_Arrangement != null)
                {
                    switch (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass secbar =
                                gir.StbSecBar_Arrangement.StbSecBeam_Same_Section;

                            for (int i = 0; i < 3; i++)
                            {
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), secbar.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), secbar.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), secbar.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), secbar.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), secbar.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), secbar.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), secbar.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), secbar.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), secbar.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), secbar.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), secbar.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), secbar.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), secbar.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), secbar.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), secbar.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), secbar.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), secbar.pitch_bar_spacing, true);

                            }
                            break;
                        case 2:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Count(); i++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass sec3 =
                                    gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i];

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec3.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec3.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec3.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec3.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec3.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec3.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec3.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec3.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec3.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec3.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec3.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec3.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec3.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec3.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec3.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec3.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec3.pitch_bar_spacing, true);

                            }
                            break;
                        case 3:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Count() + 1; i++)
                            {
                                if (i == 1) { continue; } //断面中央には値を入れない
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass sec2 = null;
                                if (i == 0)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[0]; }
                                else if (i == 2)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[1]; }

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec2.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec2.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec2.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec2.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec2.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec2.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec2.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec2.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec2.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec2.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec2.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec2.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec2.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec2.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec2.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec2.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec2.pitch_bar_spacing, true);

                            }
                            break;
                    }
                }
                //2017/05/19 鉄筋タグが無いとき→ログ
                if (gir.StbSecBar_Arrangement == null)
                {
                    //ログ表示
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[SRC梁]" + typename + "(断面id=" + gir.id.ToString() + ")");
                }

                if (symbol != null)
                {
                    TypeName_Data td = new TypeName_Data();
                    td.typename = symbol.Name;
                    td.id = gir.id;
                    td.shapename = "SRC梁";
                    typename_list.Add(td);
                }
            }

            return ret;
        }
        /// <summary> SRC片持梁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <param name="typename_list"></param>
        /// <returns></returns>
        private bool CreateCGirder_SRC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC gir, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
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
            string typename = "";
            string floor = gir.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, gir.id); }
                if (find != -1)
                {typename = stb.StbModel.StbStories[find].name;; }
            }
            typename += gir.name;

            //haunch_start,haunch_endの取得
            List<double> haunch_start = new List<double>();
            List<double> haunch_end = new List<double>();
            List<string> kind_haunch_start = new List<string>();
            List<string> kind_haunch_end = new List<string>();
            Get_Haunch(stb, gir.id, ref haunch_start, ref haunch_end, ref kind_haunch_start, ref kind_haunch_end);

            //鉄骨形状を取得
            int[] ind = new int[3];
            int[] shapeids = new int[3];
            for (int i = 0; i < gir.StbSecSteelBeam.Count(); i++)
            {
                if (i == 3 || i == 4) { continue; }
                if (gir.StbSecSteelBeam[i] == null || gir.StbSecSteelBeam[i].shape == "")
                {
                    switch (i)
                    {
                        case 0:
                            if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            {
                                ind[i] = 2;
                                Check_Steel(stb, gir.StbSecSteelBeam[2].shape, ref shapeids[i]);
                            }
                            else if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            {
                                ind[i] = 1;
                                Check_Steel(stb, gir.StbSecSteelBeam[1].shape, ref shapeids[i]);
                            }
                            break;
                        case 1:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            {
                                ind[i] = 0;
                                Check_Steel(stb, gir.StbSecSteelBeam[0].shape, ref shapeids[i]);
                            }
                            else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            {
                                ind[i] = 2;
                                Check_Steel(stb, gir.StbSecSteelBeam[2].shape, ref shapeids[i]);
                            }
                            break;
                        case 2:
                            if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            {
                                ind[i] = 0;
                                Check_Steel(stb, gir.StbSecSteelBeam[0].shape, ref shapeids[i]);
                            }
                            else if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            {
                                ind[i] = 1;
                                Check_Steel(stb, gir.StbSecSteelBeam[1].shape, ref shapeids[i]);
                            }
                            break;
                    }
                }
                else
                {
                    ind[i] = i;                   
                    Check_Steel(stb, gir.StbSecSteelBeam[i].shape, ref shapeids[i]);
                }
            }

            string shape = "";
            for (int i = 0; i < ind.Count(); i++)
            {
                if (shape == "")
                { shape = Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]); }
                else
                {
                    //鉄骨断面の種別が1つでも違ったらログを出して変換しない
                    if (shape != Check_Steel(stb, gir.StbSecSteelBeam[ind[i]].shape, ref shapeids[i]))
                    {
                        if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH) { continue; }
                        MakeTekkotuLog("SRC片持梁", gir.name, gir.id);
                        return ret;
                    }
                }
            }
            
            FamilySymbol symbol = null;
            if (SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol))
            {
                do
                {
                    int ascii = 97;
                    typename = ReName(typename, ascii);
                    ascii++;
                } while (SearchFamilySymbol(ConvFamily[2][0], typename, ref symbol));
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

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[h];
                re.Length2 = haunch_end[h];
                re.BHaunch1 = kind_haunch_start[h];
                re.BHaunch2 = kind_haunch_end[h];
                re.symbol = symbol;
                GirderSymbols.Add(re);
                switch (shape)
                {
                    case RevitLNK.st_steel_H:
                    case RevitLNK.st_steel_BH:
                        double steel_size = 0;
                        for (int j = 0; j < 3; j++)
                        {
                            var steel_H = stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H?.Find(a => a.name == gir.StbSecSteelBeam[ind[j]].shape);
                            if (steel_H != null)
                            {
                                string logtxt = Roll_H_Size_Check(steel_H);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("SRC片持梁" + shape, symbol.Name, gir.id, logtxt, 0);
                                    Commons.doc.Delete(symbol.Id);
                                    return ret;
                                }
                                SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind[j]].strength_web, gir.StbSecSteelBeam[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), gir.StbSecSteelBeam[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rgir.shape[j]), gir.StbSecSteelBeam[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_H.A, true);
                                SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_H.B, true);
                                SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_H.t1, true);
                                SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_H.t2, true);
                                SetParameter(symbol.LookupParameter(Rgir.type[j]), steel_H.type);

                                double r = steel_H.r;
                                if (steel_H.r < 1)
                                {
                                    r = 1;
                                    MakeSizeLog("SRC片持梁" + shape, symbol.Name, gir.id, "フィレット半径", 1);
                                }
                                SetParameter(symbol.LookupParameter(Rgir.r[j]), r, true);

                                if (j == 1)
                                {
                                    steel_size = steel_H.A;
                                }
                            }
                            else
                            {
                                var steel_BH = stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H?.Find(a => a.name == gir.StbSecSteelBeam[ind[j]].shape);

                                string logtxt = Build_H_Size_Check(steel_BH);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("SRC片持梁" + shape, symbol.Name, gir.id, logtxt, 0);
                                    Commons.doc.Delete(symbol.Id);
                                    return ret;
                                }
                                SetParameter(symbol.LookupParameter(Rgir.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind[j]].strength_web, gir.StbSecSteelBeam[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rgir.strength_main[j]), gir.StbSecSteelBeam[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rgir.shape[j]), gir.StbSecSteelBeam[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rgir.A[j]), steel_BH.A, true);
                                SetParameter(symbol.LookupParameter(Rgir.B[j]), steel_BH.B, true);
                                SetParameter(symbol.LookupParameter(Rgir.t1[j]), steel_BH.t1, true);
                                SetParameter(symbol.LookupParameter(Rgir.t2[j]), steel_BH.t2, true);
                                SetParameter(symbol.LookupParameter(Rgir.r[j]), 0.0, true);

                                if (j == 1)
                                {
                                    steel_size = steel_BH.A;
                                }
                            }
                        }

                        if (gir.level > 0 && steel_size > 1)
                        {
                            //1.4では通常≧0、プラス値が入っている
                            double rc_size = 0;
                            switch (gir.StbSecFigure.StbSecFigureType)
                            {
                                case 1:
                                    rc_size = gir.StbSecFigure.StbSecStraight.depth;
                                    break;

                                case 2:
                                    rc_size = gir.StbSecFigure.StbSecTaper.depth_start;
                                    break;

                                case 3:
                                    rc_size = gir.StbSecFigure.StbSecHaunch.depth_center;
                                    break;
                            }

                            //RCとSの寸法差
                            double d2 = (rc_size - steel_size) / 2;
                            //中心からの距離に換算（＋なら上に鉄骨が移動、－なら下に鉄骨が移動）
                            double d3 = gir.level - d2;
                            SetParameter(symbol.LookupParameter(Rgir.level), d3, true);
                        }

                        SetParameter(symbol.LookupParameter(Rgir.offset), gir.offset, true);
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
                        Make_taisyougaiLog("SRC片持梁", gir.id, gir.name, shape, shapename_J);
                        return ret;
                }
                SetParameter(symbol.LookupParameter(Rgir.haunch_start), haunch_start[h], true);
                SetParameter(symbol.LookupParameter(Rgir.haunch_end), haunch_end[h], true);
                //DROPならボックスハンチチェックボックスをtrueにする
                if (kind_haunch_start[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[0]), false); }
                if (kind_haunch_end[h] == "DROP")
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), true); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.BHaunch[1]), false); }

                //RC部
                //鉄筋径のチェック
                Get_D("SRC片持梁", ref gir.D_reinforcement_main, "主筋", typename, gir.id);
                Get_D("SRC片持梁", ref gir.D_reinforcement_2nd_main, "副主筋", typename, gir.id);
                Get_D("SRC片持梁", ref gir.D_stirrup, "あばら筋", typename, gir.id);
                Get_D("SRC片持梁", ref gir.D_reinforcement_web, "腹筋", typename, gir.id);
                Get_D("SRC片持梁", ref gir.D_bar_spacing, "巾止筋", typename, gir.id);

                SetParameter(symbol.LookupParameter(Rgir.strength_concrete), gir.strength_concrete);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol.LookupParameter(Rgir.kind_beam), canti + "Beam"); }
                SetParameter(symbol.LookupParameter(Rgir.kind_beam2), gir.kind_beam);
                SetParameter(symbol.LookupParameter(Rgir.name), gir.name);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_main), gir.strength_reinforcement_main);
                SetParameter(symbol.LookupParameter(Rgir.SecId), gir.id);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_2nd_main), gir.strength_reinforcement_2nd_main);
                SetParameter(symbol.LookupParameter(Rgir.strength_stirrup), gir.strength_stirrup);
                SetParameter(symbol.LookupParameter(Rgir.strength_reinforcement_web), gir.strength_reinforcement_web);
                SetParameter(symbol.LookupParameter(Rgir.strength_bar_spacing), gir.strength_bar_spacing);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_left), gir.depth_cover_left);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_right), gir.depth_cover_right);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_top), gir.depth_cover_top);
                SetParameter(symbol.LookupParameter(Rgir.depth_cover_bottom), gir.depth_cover_bottom);
                SetParameter(symbol.LookupParameter(Rgir.interval_reinforcement), gir.interval_reinforcement);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_top), gir.center_reinforcement_top);
                SetParameter(symbol.LookupParameter(Rgir.center_reinforcement_bottom), gir.center_reinforcement_bottom);
                SetParameter(symbol.LookupParameter(Rgir.isOutIn), gir.isOutIn);
                for (int i = 0; i < Rgir.D_reinforcement_main_top.Count(); i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]), gir.D_reinforcement_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]), gir.D_reinforcement_2nd_main);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]), gir.D_reinforcement_2nd_main);
                }
                for (int i = 0; i < Rgir.D_stirrup.Count(); i++)
                {
                    SetParameter(symbol.LookupParameter(Rgir.D_stirrup[i]), gir.D_stirrup);
                    SetParameter(symbol.LookupParameter(Rgir.D_reinforcement_web[i]), gir.D_reinforcement_web);
                    SetParameter(symbol.LookupParameter(Rgir.D_bar_spacing[i]), gir.D_bar_spacing);
                }
                if (gir.StbSecFigure != null)
                {

                    switch (gir.StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecStraight.width, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecStraight.depth, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecStraight.depth, true);
                            break;
                        case 2:
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecTaper.width_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecTaper.width_end, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecTaper.depth_start, true);
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecTaper.depth_end, true);
                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[SRC片持梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")は形状タイプがテーパーのため中央断面を始端断面で変換しました。");
                            break;
                        case 3:
                            if (gir.StbSecFigure.StbSecHaunch.width_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_start = gir.StbSecFigure.StbSecHaunch.width_center; }
                            SetParameter(symbol.LookupParameter(Rgir.width_start), gir.StbSecFigure.StbSecHaunch.width_start, true);
                            if (gir.StbSecFigure.StbSecHaunch.width_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_center = gir.StbSecFigure.StbSecHaunch.width_start; }
                            SetParameter(symbol.LookupParameter(Rgir.width_center), gir.StbSecFigure.StbSecHaunch.width_center, true);
                            if (gir.StbSecFigure.StbSecHaunch.width_end == 0)
                            { gir.StbSecFigure.StbSecHaunch.width_end = gir.StbSecFigure.StbSecHaunch.width_center; }
                            SetParameter(symbol.LookupParameter(Rgir.width_end), gir.StbSecFigure.StbSecHaunch.width_end, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_start == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_start = gir.StbSecFigure.StbSecHaunch.depth_center; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_start), gir.StbSecFigure.StbSecHaunch.depth_start, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_center == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_center = gir.StbSecFigure.StbSecHaunch.depth_start; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_center), gir.StbSecFigure.StbSecHaunch.depth_center, true);
                            if (gir.StbSecFigure.StbSecHaunch.depth_end == 0)
                            { gir.StbSecFigure.StbSecHaunch.depth_end = gir.StbSecFigure.StbSecHaunch.depth_center; }
                            SetParameter(symbol.LookupParameter(Rgir.depth_end), gir.StbSecFigure.StbSecHaunch.depth_end, true);
                            break;
                    }
                }

                if (gir.StbSecBar_Arrangement != null)
                {
                    switch (gir.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Same_SectionClass secbar =
                                gir.StbSecBar_Arrangement.StbSecBeam_Same_Section;

                            for (int i = 0; i < 3; i++)
                            {
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), secbar.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), secbar.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), secbar.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), secbar.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), secbar.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), secbar.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), secbar.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), secbar.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), secbar.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), secbar.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), secbar.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), secbar.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), secbar.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), secbar.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), secbar.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), secbar.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), secbar.pitch_bar_spacing, true);

                            }
                            break;
                        case 2:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section.Count(); i++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_Center_End_SectionClass sec3 =
                                    gir.StbSecBar_Arrangement.StbSecBeam_Start_Center_End_Section[i];

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec3.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec3.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec3.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec3.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec3.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec3.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec3.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec3.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec3.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec3.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec3.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec3.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec3.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec3.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec3.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec3.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec3.pitch_bar_spacing, true);

                            }
                            break;
                        case 3:
                            for (int i = 0; i < gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section.Count() + 1; i++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC.StbSecBar_ArrangementClass.StbSecBeam_Start_End_SectionClass sec2 = null;
                                if (i == 0)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[0]; }
                                else if (i == 1)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[0]; }
                                else if (i == 2)
                                { sec2 = gir.StbSecBar_Arrangement.StbSecBeam_Start_End_Section[1]; }

                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_1st[i]), sec2.count_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_2nd[i]), sec2.count_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_top_3rd[i]), sec2.count_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_1st[i]), sec2.count_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]), sec2.count_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]), sec2.count_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]), sec2.count_2nd_main_top_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]), sec2.count_2nd_main_top_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]), sec2.count_2nd_main_top_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]), sec2.count_2nd_main_bottom_1st);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]), sec2.count_2nd_main_bottom_2nd);
                                SetParameter(symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]), sec2.count_2nd_main_bottom_3rd);
                                SetParameter(symbol.LookupParameter(Rgir.count_stirrup[i]), sec2.count_stirrup);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_stirrup[i]), sec2.pitch_stirrup, true);
                                SetParameter(symbol.LookupParameter(Rgir.count_web[i]), sec2.count_web);
                                SetParameter(symbol.LookupParameter(Rgir.count_bar_spacing[i]), sec2.count_bar_spacing);
                                SetParameter(symbol.LookupParameter(Rgir.pitch_bar_spacing[i]), sec2.pitch_bar_spacing, true);
                            }
                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[SRC片持梁]" + gir.name + "(断面id=" + gir.id.ToString() + ")は梁断面始端終端別配筋のため中央配筋を始端配筋で変換しました。");
                            break;
                    }
                }
                else //2017/05/19 鉄筋タグが無いとき→ログ                
                {
                    //ログ表示
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[SRC片持梁]" + typename + "(断面id=" + gir.id.ToString() + ")");
                }

                if (symbol != null)
                {
                    TypeName_Data td = new TypeName_Data();
                    td.typename = symbol.Name;
                    td.id = gir.id;
                    td.shapename = "SRC片持梁";
                    typename_list.Add(td);
                }
            }

            return ret;
        }

        private bool SetParameter_Girder_and_CGirder_HandBH(STBclass stb, FamilySymbol symbol, string typename, double haunch_start, double haunch_end, int steel_ind, int ind, int j, string shape,
                                                            STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, string shapename_J, bool shapeflg = true)
        {
            bool ret = true;

            string logtxt = "";
            if (shape == RevitLNK.st_steel_H)
            {
               

                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel = stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[steel_ind];
                logtxt = Roll_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename_J, symbol.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol.Id);
                    ret = false;
                    return ret;
                }
                if (!gir.isCanti)
                {
                    FamilyStructure.S_Gir_H Rgir_H = SetFamily.SGirH;
                    SetParameter(symbol.LookupParameter(Rgir_H.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind].strength_web, gir.StbSecSteelBeam[ind].strength_main));
                    SetParameter(symbol.LookupParameter(Rgir_H.strength_main[j]), gir.StbSecSteelBeam[ind].strength_main);
                    SetParameter(symbol.LookupParameter(Rgir_H.A[j]), steel.A, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.B[j]), steel.B, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.t1[j]), steel.t1, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.t2[j]), steel.t2, true);
                    double r = steel.r;
                    //2017/07/06  <変更>6/7版ファミリよりフィレットが0でも変換できるようになったため処理を除外
                    //if(!shapeflg)
                    //{
                    //    //r = 0;
                    //    //LogData.AddLog(LogData.LogKind.Warning, 0, "[S梁" + shapename_J + "]" + symbol.Name + "(断面id=" + gir.id.ToString() + ")は" + 
                    //    //    　　　　　"ハンチ付き梁なのでフィレット半径を0mmで変換しました。");
                    //}
                    //else if (steel.r < 1 && shapeflg)
                    //{
                    //    //r = 1;
                    //    //MakeSizeLog("S梁" + shapename_J, symbol.Name, gir.id, "フィレット半径", 1);
                    //}
                    
                    SetParameter(symbol.LookupParameter(Rgir_H.r[j]), r, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.shape[j]), gir.StbSecSteelBeam[ind].shape);
                    SetParameter(symbol.LookupParameter(Rgir_H.type[j]), steel.type);
                }
                else
                {
                    FamilyStructure.S_CGir_H Rgir_H = SetFamily.SCGirH;
                    SetParameter(symbol.LookupParameter(Rgir_H.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind].strength_web, gir.StbSecSteelBeam[ind].strength_main));
                    SetParameter(symbol.LookupParameter(Rgir_H.strength_main[j]), gir.StbSecSteelBeam[ind].strength_main);
                    SetParameter(symbol.LookupParameter(Rgir_H.A[j]), steel.A, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.B[j]), steel.B, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.t1[j]), steel.t1, true);
                    SetParameter(symbol.LookupParameter(Rgir_H.t2[j]), steel.t2, true);                 
                    SetParameter(symbol.LookupParameter(Rgir_H.r[j]), steel.r, true); //片持梁用ファミリはフィット半径が0でも変換できる
                    SetParameter(symbol.LookupParameter(Rgir_H.shape[j]), gir.StbSecSteelBeam[ind].shape);
                    SetParameter(symbol.LookupParameter(Rgir_H.type[j]), steel.type);
                }
            }
            else if (shape == RevitLNK.st_steel_BH)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel = stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[steel_ind];
                logtxt = Build_H_Size_Check(steel);
                if (logtxt != "")
                {
                    MakeSizeLog(shapename_J, symbol.Name, gir.id, logtxt, 0); 
                    Commons.doc.Delete(symbol.Id);
                    return ret;
                }
                if (!gir.isCanti)
                {
                    FamilyStructure.S_Gir_BH Rgir_BH = SetFamily.SGirBH;
                    SetParameter(symbol.LookupParameter(Rgir_BH.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind].strength_web, gir.StbSecSteelBeam[ind].strength_main));
                    SetParameter(symbol.LookupParameter(Rgir_BH.strength_main[j]), gir.StbSecSteelBeam[ind].strength_main);
                    SetParameter(symbol.LookupParameter(Rgir_BH.A[j]), steel.A, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.B[j]), steel.B, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.t1[j]), steel.t1, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.t2[j]), steel.t2, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.r[j]), 0.0, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.shape[j]), gir.StbSecSteelBeam[ind].shape);
                }
                else
                {
                    FamilyStructure.S_CGir_H Rgir_BH = SetFamily.SCGirBH;
                    SetParameter(symbol.LookupParameter(Rgir_BH.strength_web[j]), GetStrength_web(gir.StbSecSteelBeam[ind].strength_web, gir.StbSecSteelBeam[ind].strength_main));
                    SetParameter(symbol.LookupParameter(Rgir_BH.strength_main[j]), gir.StbSecSteelBeam[ind].strength_main);
                    SetParameter(symbol.LookupParameter(Rgir_BH.A[j]), steel.A, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.B[j]), steel.B, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.t1[j]), steel.t1, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.t2[j]), steel.t2, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.r[j]), 0.0, true);
                    SetParameter(symbol.LookupParameter(Rgir_BH.shape[j]), gir.StbSecSteelBeam[ind].shape);
                }
            }
            return ret;
        }


        /// <summary>大梁・小梁・片持梁溝形鋼SetParameter
        /// </summary>
        /// <param name="symbol_C"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="steel_C"></param>
        /// <param name="Sbeam"></param>
        /// <param name="gir"></param>
        /// <returns></returns>
        private bool SetParameter_Girder_and_CGirder_C(int j, FamilySymbol symbol_C, string typename, List<double> haunch_start, List<double> haunch_end, List<string> kind_haunch_start, 
                                                       List<string>kind_haunch_end, STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steel_C,
                                                       STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S.StbSecSteelBeamClass Sbeam, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, string shapename_J)
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
                if(logtxt != "")
                {
                    MakeSizeLog(shapename_J, symbol_C.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol_C.Id);               
                    ret = false;
                    return ret;
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[i];
                re.Length2 = haunch_end[i];
                re.BHaunch1 = kind_haunch_start[i];
                re.BHaunch2 = kind_haunch_end[i];
                re.symbol = symbol_C;
                GirderSymbols.Add(re);

                SetParameter(symbol_C.LookupParameter(Rgir_C.SecId), gir.id);
                SetParameter(symbol_C.LookupParameter(Rgir_C.name), gir.name);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam), canti + "Beam"); }
                SetParameter(symbol_C.LookupParameter(Rgir_C.kind_beam2), gir.kind_beam);
                SetParameter(symbol_C.LookupParameter(Rgir_C.strength), Sbeam.strength_main);
                SetParameter(symbol_C.LookupParameter(Rgir_C.shape[j]), Sbeam.shape);
                SetParameter(symbol_C.LookupParameter(Rgir_C.type[j]), steel_C.type);
                if (steel_C.type == "2C")
                { Make_typeLog(typename, gir.id, RevitLNK.st_steel_C, shapename_J); }
                SetParameter(symbol_C.LookupParameter(Rgir_C.H[j]), steel_C.A, true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.B[j]), steel_C.B, true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.t1[j]), steel_C.t1, true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.t2[j]), steel_C.t2, true);

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
               
                SetParameter(symbol_C.LookupParameter(Rgir_C.r1[j]), r1, true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.r2[j]), r2, true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.haunch_start), haunch_start[i], true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.haunch_end), haunch_end[i], true);
                SetParameter(symbol_C.LookupParameter(Rgir_C.isOutIn), gir.isOutIn);
            }
            return ret;
        }

        /// <summary>大梁・小梁・片持梁山形鋼SetParameter
        /// </summary>
        /// <param name="symbol_L"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="steel_L"></param>
        /// <param name="Sbeam"></param>
        /// <param name="gir"></param>
        /// <returns></returns>
        private bool SetParameter_Girder_and_CGirder_L(int j, FamilySymbol symbol_L, string typename, List<double> haunch_start, List<double> haunch_end, List<string>kind_haunch_start, List<string>kind_haunch_end,
                                                       STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steel_L,
                                                       STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S.StbSecSteelBeamClass Sbeam, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, string shapename_J)
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
                if(logtxt != "")
                {
                    MakeSizeLog(shapename_J, symbol_L.Name, gir.id, logtxt, 0);
                    ret = false;
                    return ret;
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[i];
                re.Length2 = haunch_end[i];
                re.BHaunch1 = kind_haunch_start[i];
                re.BHaunch2 = kind_haunch_end[i];
                re.symbol = symbol_L;
                GirderSymbols.Add(re);

                SetParameter(symbol_L.LookupParameter(Rgir_L.SecId), gir.id);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam), canti + "Beam"); }
                SetParameter(symbol_L.LookupParameter(Rgir_L.kind_beam2), gir.kind_beam);
                SetParameter(symbol_L.LookupParameter(Rgir_L.name), gir.name);
                SetParameter(symbol_L.LookupParameter(Rgir_L.strength), Sbeam.strength_main);
                SetParameter(symbol_L.LookupParameter(Rgir_L.shape[j]), Sbeam.shape);
                SetParameter(symbol_L.LookupParameter(Rgir_L.type[j]), steel_L.type);
                if (steel_L.type == "2L")
                { Make_typeLog(typename, gir.id, RevitLNK.st_steel_L, shapename_J); }
                SetParameter(symbol_L.LookupParameter(Rgir_L.A[j]), steel_L.A, true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.B[j]), steel_L.B, true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.t1[j]), steel_L.t1, true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.t2[j]), steel_L.t2, true);

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
                    MakeSizeLog(shapename_J, symbol_L.Name, gir.id, logtxt, 1);
                }
               
                SetParameter(symbol_L.LookupParameter(Rgir_L.r1[j]), r1, true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.r2[j]), r2, true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.haunch_start), haunch_start[i], true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.haunch_end), haunch_end[i], true);
                SetParameter(symbol_L.LookupParameter(Rgir_L.isOutIn), gir.isOutIn);
            }
            
            return ret;
        }

        /// <summary> 大梁・小梁・片持梁リップ溝形鋼SetParameter
        /// </summary>
        /// <param name="j"></param>
        /// <param name="symbol_LipC"></param>
        /// <param name="typename"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="steel_LipC"></param>
        /// <param name="Sbeam"></param>
        /// <param name="gir"></param>
        /// <param name="shapename_J"></param>
        /// <returns></returns>
        private bool SetParameter_Girder_and_CGirder_LipC(int j, FamilySymbol symbol_LipC, string typename, List<double> haunch_start, List<double> haunch_end, List<string>kind_haunch_start, List<string>kind_haunch_end,
                                                          STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_LipC_Class steel_LipC,
                                                          STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S.StbSecSteelBeamClass Sbeam, STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir, string shapename_J)
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
                if(logtxt != "")
                {
                    MakeSizeLog(shapename_J, symbol_LipC.Name, gir.id, logtxt, 0);
                    Commons.doc.Delete(symbol_LipC.Id);
                    ret = false;
                    return ret;
                }

                ReNameSymbols re = new ReNameSymbols();
                re.name = typename;
                re.id = gir.id;
                re.Length = haunch_start[i];
                re.Length2 = haunch_end[i];
                re.BHaunch1 = kind_haunch_start[i];
                re.BHaunch2 = kind_haunch_end[i];
                re.symbol = symbol_LipC;
                GirderSymbols.Add(re);

                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.SecId), gir.id);
                string canti = "";
                if (gir.isCanti)
                { canti = "Cantilever-"; }
                if (gir.kind_beam == "GIRDER")
                { SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam), canti + "Girder"); }
                else
                { SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam), canti + "Beam"); }
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.kind_beam2), gir.kind_beam);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.name), gir.name);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.strength), Sbeam.strength_main);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.shape[j]), Sbeam.shape);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.type[j]), steel_LipC.type);
                if (steel_LipC.type == "2C")
                { Make_typeLog(typename, gir.id, RevitLNK.st_steel_LipC, shapename_J); }
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.H[j]), steel_LipC.H, true);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.A[j]), steel_LipC.A, true);
                //if (steel_LipC.C < 0.5)
                //{ steel_LipC.C = 0.5; }
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.C[j]), steel_LipC.C, true);
                //if (steel_LipC.t < 0.5)
                //{ steel_LipC.t = 0.5; }
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.t[j]), steel_LipC.t, true);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.haunch_start), haunch_start[i], true);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.haunch_end), haunch_end[i], true);
                SetParameter(symbol_LipC.LookupParameter(Rgir_LipC.isOutIn), gir.isOutIn);
            }
            return ret;
        }
              
        /// <summary>梁インスタンスパラメータ設定（大梁）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="gir"></param>
        /// <param name="sgirind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily">大梁のファミリ</param>
        /// <param name="ConvCFamily">片持ち大梁のファミリ</param>
        /// <returns></returns>
        private bool CreateGirder_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbGirder gir, int sgirind, ProgressBarForm pform, Family[][] ConvFamily, Family[][] ConvCFamily)
        {
            bool ret = true;

            //2017/05/23 回転角が360度以上→-360度する
            if (gir.rotate >= 360) { gir.rotate = gir.rotate - 360; }

            //梁断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            string floor = "";
            Family fami = null;
            string shape = "";
            int ind = 0;
            //タイプ名
            string typename = "";
            //所属層のindex
            int find = -1;
            //片持ちか否か
            bool isCanti = false;
            //S・SRCの時の鉄骨形状名
            string shapename = "";
            bool isOutin = false;
            //断面id
            int sid = -1;
            string sid_name = "";
            //使用するファミリの取得
            switch (gir.kind_structure)
            {
                case "RC":
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC rcgir = section.StbSecBeams_RC[sgirind];
                    sid = rcgir.id;
                    sid_name = SetFamily.RCGir.SecId;
                    floor = section.StbSecBeams_RC[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_RC[sgirind].name;

                    isCanti = rcgir.isCanti;
                    isOutin = rcgir.isOutIn;
                    if (isCanti)
                    {
                        if (rcgir.isFoundation)
                        { fami = ConvCFamily[0][0]; }
                        else
                        { fami = ConvCFamily[0][1]; }
                    }
                    else
                    {
                        switch (rcgir.StbSecFigure.StbSecFigureType) //2016/11/07ファミリを詳細化⇒ハンチ付か3断面同一かを判断する
                        {
                            case 1:
                                if (rcgir.StbSecBar_Arrangement == null) //2017/09/14 鉄筋が入力されていなければ全断面として変換
                                {
                                    if (rcgir.isFoundation)
                                    { fami = ConvFamily[0][0]; }
                                    else
                                    { fami = ConvFamily[0][2]; }
                                }
                                else
                                {
                                    if (rcgir.StbSecBar_Arrangement.StbSecBar_ArrangementType == 1)
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
                                if (rcgir.StbSecFigure.StbSecTaper.depth_start != rcgir.StbSecFigure.StbSecTaper.depth_end ||
                                    rcgir.StbSecFigure.StbSecTaper.width_start != rcgir.StbSecFigure.StbSecTaper.width_end)
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
                                if (rcgir.StbSecFigure.StbSecHaunch.depth_start != rcgir.StbSecFigure.StbSecHaunch.depth_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.depth_end != rcgir.StbSecFigure.StbSecHaunch.depth_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.width_start != rcgir.StbSecFigure.StbSecHaunch.width_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.width_end != rcgir.StbSecFigure.StbSecHaunch.width_center)
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
                case "S":
                    sid = section.StbSecBeams_S[sgirind].id;

                    floor = section.StbSecBeams_S[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_S[sgirind].name;

                    shape = Check_Steel(stb, section.StbSecBeams_S[sgirind].StbSecSteelBeam[0].shape, ref ind);

                    isCanti = section.StbSecBeams_S[sgirind].isCanti;
                    isOutin = section.StbSecBeams_S[sgirind].isOutIn;
                    bool shapeflg = false;
                    for (int i = 0; i < section.StbSecBeams_S[sgirind].StbSecSteelBeam.Count(); i++)
                    {
                        if (section.StbSecBeams_S[sgirind].StbSecSteelBeam[i] == null) { continue; }
                        if (section.StbSecBeams_S[sgirind].StbSecSteelBeam[0].shape != section.StbSecBeams_S[sgirind].StbSecSteelBeam[i].shape)
                        {
                            shapeflg = true;
                            break;
                        }
                    }
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            shapename = "H形鋼";
                            sid_name = SetFamily.SGirH.SecId;
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
                            sid_name = SetFamily.SGirBH.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][1]; }
                            else
                            { fami = ConvFamily[1][1]; }
                            break;
                        case RevitLNK.st_steel_C:
                            shapename = "溝形鋼";
                            sid_name = SetFamily.SGirC.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][2]; }
                            else
                            { fami = ConvFamily[1][2]; }
                            break;
                        case RevitLNK.st_steel_L:
                            shapename = "山形鋼";
                            sid_name = SetFamily.SGirL.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][3]; }
                            else
                            { fami = ConvFamily[1][3]; }
                            break;
                        case RevitLNK.st_steel_LipC:
                            shapename = "リップ溝形鋼";
                            sid_name = SetFamily.SGirLipC.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][4]; }
                            else
                            { fami = ConvFamily[1][4]; }
                            break;
                        default:
                            return ret;
                    }
                    break;
                case "SRC":
                    shapename = "H形鋼";
                    sid_name = SetFamily.SRCGirH.SecId;
                    sid = section.StbSecBeams_SRC[sgirind].id;

                    floor = section.StbSecBeams_SRC[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_SRC[sgirind].name;

                    isCanti = section.StbSecBeams_SRC[sgirind].isCanti;
                    isOutin = section.StbSecBeams_SRC[sgirind].isOutIn;

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
                LogData.AddLog(LogData.LogKind.Warning, 2100, gir.kind_structure + "梁" + shapename);
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
                if (!SearchFamilySymbol(fami, typename, ref symbol, sid, sid_name))
                {
                    //ReNameされているとき
                    symbol = null;
                    for (int i = 0; i < GirderSymbols.Count(); i++)
                    {
                        if (GirderSymbols[i].id != gir.id_section) { continue; }
                        if (GirderSymbols[i].Length == gir.haunch_start && GirderSymbols[i].Length2 == gir.haunch_end)
                        {
                            symbol = GirderSymbols[i].symbol;
                            break;
                        }
                    }
                    if (symbol == null)
                    {
                        //ログ表示(タイプが無い)
                        LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + gir.kind_structure + "大梁]" + typename + "(配置id=" + gir.id + ")");
                        return ret;
                    }
                }
            }


            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, gir.idNode_end, false);
            int indt = Get_stbFloor_index(stb, gir.idNode_start, false);
            Level btmLevel = null;
            if (indb == -1 && indt == -1)
            {
                btmLevel = SearchLevel_height(stb, gir.idNode_start, gir.idNode_end);
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
            XYZ Ps_org = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
            XYZ Pe_org = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
            if (Ps_org.DistanceTo(Pe_org) < Commons.mm2ft(1))
            {
                string log = "大梁の生成：" + "\t" + "[配置Id " + gir.id.ToString() + "]" + typename + ",[節点Id";
                log += MakeLog_Coord(0, new int[] { gir.idNode_start, gir.idNode_end });
                log += "] ";

                LogData.AddLog(LogData.LogKind.Warning, 3100, log);
                return ret; //falseは変換失敗
            }

            XYZ vecU = (Pe_org - Ps_org).Normalize();

            //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
            bool se_offset_flg = false; //始端・終端にオフセットが入っている⇒true

            XYZ offsetstart = new XYZ();
            XYZ offsetend = new XYZ();
            XYZ offsetstart2 = new XYZ();
            XYZ offsetend2 = new XYZ();

            XYZ Ps_xy = new XYZ();
            XYZ Pe_xy = new XYZ();

            if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_end_Z != 0 ||
                gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0)
            {
                offsetstart = TransformCoord(Ps_org, Pe_org, gir.offset_start_X, gir.offset_start_Y, gir.offset_start_Z, -gir.rotate);
                offsetend = TransformCoord(Ps_org, Pe_org, gir.offset_end_X, gir.offset_end_Y, gir.offset_end_Z, -gir.rotate);

                se_offset_flg = true;

                Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(gir.offset_start_X), Ps_org.Y + Commons.mm2ft(gir.offset_start_Y), Ps_org.Z);
                Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(gir.offset_end_X), Pe_org.Y + Commons.mm2ft(gir.offset_end_Y), Pe_org.Z);
            }
            else
            {
                offsetstart = Search_Offset_gir(stb, gir.idNode_start, ref Ps_org, ref Pe_org, "start", vecU, gir.id, btmLevel, -gir.rotate, out offsetstart2);
                offsetend = Search_Offset_gir(stb, gir.idNode_end, ref Ps_org, ref Pe_org, "end", vecU, gir.id, btmLevel, -gir.rotate, out offsetend2);

                Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
                Pe_xy = Pe_org + Commons.mm2ft(offsetend2);
            }

            //梁描画用節点（部材方向のオフセットだけ考慮、それ以外のオフセットはパラメータに入力）
            XYZ Ps = Set_offset(Ps_org, offsetstart, vecU);
            XYZ Pe = Set_offset(Pe_org, offsetend, vecU);

            //継手計算用に全てのオフセットを考慮した節点を求める　2017/08/23
            //XYZ Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(gir.offset_start_X), Ps_org.Y + Commons.mm2ft(gir.offset_start_Y), Ps_org.Z);
            //XYZ Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(gir.offset_end_X), Pe_org.Y + Commons.mm2ft(gir.offset_end_Y), Pe_org.Z);
            //XYZ Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
            //XYZ Pe_xy = Pe_org + Commons.mm2ft(offsetend2);


            //端点オフセットのZ座標が入っていないとき
            if (gir.offset_start_Z == 0 && gir.offset_end_Z == 0)
            {
                //レベル方向のオフセットはSTART/END_Z_OFFSET_VALUEに統一
                //Ps = new XYZ(Ps.X, Ps.Y, Ps.Z + Commons.mm2ft(gir.level));
                //Pe = new XYZ(Pe.X, Pe.Y, Pe.Z + Commons.mm2ft(gir.level));
                offsetstart = offsetstart + new XYZ(0, 0, gir.level);
                offsetend = offsetend + new XYZ(0, 0, gir.level);
            }

            Line gir_L = Line.CreateBound(Ps, Pe);
            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (isOutin)
                {
                    FamilySymbol newsymbol = Create_newsymbol_isOutin(stb, symbol, gir.id, "GIRDER", isCanti);
                    if (newsymbol != null)
                    { instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam); }
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                }

                //ジオメトリ：各オフセット
                SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                if (se_offset_flg)
                {
                    SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                    SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                    SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                    SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                }
                else
                {
                    SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, gir.offset, true);
                    SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, gir.offset, true);
                    SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                    SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                }

                //断面回転
                SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-gir.rotate * Math.PI) / 180);

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ)
                SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.Girder); //構造用途（大梁）

                SetParameter(instance, BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM, btmLevel.Id);

                switch (gir.kind_structure)
                {
                    case "RC":
                        FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                        SetParameter(instance.LookupParameter(Rgir.MemId), gir.id);
                        SetParameter(instance.LookupParameter(Rgir.NameMembers), gir.name);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_top), gir.thickness_ex_top);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_right), gir.thickness_ex_right);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_left), gir.thickness_ex_left);
                        SetParameter(instance.LookupParameter(Rgir.kind_haunch_start), gir.kind_haunch_start);
                        SetParameter(instance.LookupParameter(Rgir.kind_haunch_end), gir.kind_haunch_end);
                        SetParameter(instance.LookupParameter(Rgir.type_haunch_H), gir.type_haunch_H);
                        SetParameter(instance.LookupParameter(Rgir.type_haunch_V), gir.type_haunch_V);
                        break;
                    case "S":
                        if (!isCanti)
                        {
                            Create_GirderandBeam_S_instance(stb, shape, instance, Ps_xy, Pe_xy, Ps_org, Pe_org, gir, null);
                        }
                        else
                        {
                            FamilyStructure.S_CGir_H Hgir = SetFamily.SCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                            if (fami.Name == "Steel_CG_H")
                            {
                                bool joint = false;
                                double joint_start = gir.joint_start;
                                if (gir.joint_start != 0)
                                { joint = true; }
                                else
                                {
                                    joint = false;
                                }
                                SetParameter(instance.LookupParameter("継手"), joint);
                                Commons.doc.Regenerate();
                                if (joint_start == 0)
                                { joint_start = 1; }
                                SetParameter(instance.LookupParameter(Hgir.joint_start), joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                            else
                            {
                                SetParameter(instance.LookupParameter(Hgir.joint_start), gir.joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                        }
                        break;
                    case "SRC":
                        if (!isCanti)
                        {
                            FamilyStructure.SRC_Gir Hgir = SetFamily.SRCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_ex_top);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_ex_right);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_ex_left);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            int numjoint = 0;
                            if (gir.joint_start != 0)
                            { numjoint++; }
                            if (gir.joint_end != 0)
                            { numjoint++; }
                            SetParameter(instance.LookupParameter("継手数"), numjoint);
                            Commons.doc.Regenerate();
                            if (numjoint != 0)
                            {
                                double joint_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps_xy, Pe_xy, gir.idNode_start);
                                SetParameter(instance.LookupParameter(Hgir.joint_start), joint_s);
                                double joint_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe_xy, Ps_xy, gir.idNode_end);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), joint_e);
                            }
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                        }
                        else
                        {
                            FamilyStructure.SRC_CGir Hgir = SetFamily.SRCCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_ex_top);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_ex_right);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_ex_left);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            int numjoint = 0;
                            if (gir.joint_start != 0)
                            { numjoint++; }
                            if (gir.joint_end != 0)
                            { numjoint++; }
                            SetParameter(instance.LookupParameter("継手数"), numjoint);
                            Commons.doc.Regenerate();
                            if (numjoint != 0)
                            {
                                SetParameter(instance.LookupParameter(Hgir.joint_start), gir.joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                        }
                        break;

                }

                //解析線分作成
                Commons.doc.Regenerate();
                AnalyticalMember member = AnalyticalMember.Create(Commons.doc, Line.CreateBound(Ps_org, Pe_org));
                if (member != null)
                {
                    //構造の役割
                    var p = member.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleGirder);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(member.Id, instance.Id);
                }


                //変換情報ログの出力
                var nodeIds = new int[] { gir.idNode_start, gir.idNode_end } ;
                MakeNodeLog( "大梁の生成：", "[配置Id " + gir.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, gir.id, "大梁", typename, nodeIds );

                Commons.doc.Regenerate();
                CGrp_Add(stb, gir.idNode_start, gir.idNode_end, instance.Id, instance);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        /// <summary>梁インスタンスパラメータ設定（小梁）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateBeam_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbBeam gir, int sgirind, ProgressBarForm pform, Family[][] ConvFamily, Family[][] ConvCFamily)
        {
            bool ret = true;

            //2017/05/23 回転角が360度以上→-360度する
            if (gir.rotate >= 360) { gir.rotate = gir.rotate - 360; }

            //梁断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            string floor = "";
            Family fami = null;
            string shape = "";
            int ind = 0;
            //タイプ名
            string typename = "";
            //所属層のindex
            int find = -1;
            //片持ちか否か
            bool isCanti = false;
            //S・SRCの時の鉄骨形状名
            string shapename = "";
            bool isOutin = false;
            //断面id
            int sid = -1;
            string sid_name = "";
            //使用するファミリの取得
            switch (gir.kind_structure)
            {
                case "RC":
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC rcgir = section.StbSecBeams_RC[sgirind];
                    sid = rcgir.id;
                    sid_name = SetFamily.RCGir.SecId;
                    floor = section.StbSecBeams_RC[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_RC[sgirind].name;

                    isCanti = rcgir.isCanti;
                    isOutin = rcgir.isOutIn;
                    if (isCanti)
                    {
                        if (rcgir.isFoundation)
                        { fami = ConvCFamily[0][0]; }
                        else
                        { fami = ConvCFamily[0][1]; }
                    }
                    else
                    {
                        switch (rcgir.StbSecFigure.StbSecFigureType) //2016/11/07ファミリを詳細化⇒ハンチ付か3断面同一かを判断する
                        {
                            case 1:
                                if (rcgir.StbSecBar_Arrangement == null) //2017/09/14 鉄筋が入力されていなければ全断面として変換
                                {
                                    if (rcgir.isFoundation)
                                    { fami = ConvFamily[0][0]; }
                                    else
                                    { fami = ConvFamily[0][2]; }
                                }
                                else
                                {
                                    if (rcgir.StbSecBar_Arrangement.StbSecBar_ArrangementType == 1)
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
                                if (rcgir.StbSecFigure.StbSecTaper.depth_start != rcgir.StbSecFigure.StbSecTaper.depth_end ||
                                    rcgir.StbSecFigure.StbSecTaper.width_start != rcgir.StbSecFigure.StbSecTaper.width_end)
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
                                if (rcgir.StbSecFigure.StbSecHaunch.depth_start != rcgir.StbSecFigure.StbSecHaunch.depth_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.depth_end != rcgir.StbSecFigure.StbSecHaunch.depth_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.width_start != rcgir.StbSecFigure.StbSecHaunch.width_center ||
                                    rcgir.StbSecFigure.StbSecHaunch.width_end != rcgir.StbSecFigure.StbSecHaunch.width_center)
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
                case "S":
                    sid = section.StbSecBeams_S[sgirind].id;

                    floor = section.StbSecBeams_S[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_S[sgirind].name;

                    shape = Check_Steel(stb, section.StbSecBeams_S[sgirind].StbSecSteelBeam[0].shape, ref ind);

                    isCanti = section.StbSecBeams_S[sgirind].isCanti;
                    isOutin = section.StbSecBeams_S[sgirind].isOutIn;
                    bool shapeflg = false;
                    for (int i = 0; i < section.StbSecBeams_S[sgirind].StbSecSteelBeam.Count(); i++)
                    {
                        if (section.StbSecBeams_S[sgirind].StbSecSteelBeam[i] == null) { continue; }
                        if (section.StbSecBeams_S[sgirind].StbSecSteelBeam[0].shape != section.StbSecBeams_S[sgirind].StbSecSteelBeam[i].shape)
                        {
                            shapeflg = true;
                            break;
                        }
                    }
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            shapename = "H形鋼";
                            sid_name = SetFamily.SGirH.SecId;
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
                            sid_name = SetFamily.SGirBH.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][1]; }
                            else
                            { fami = ConvFamily[1][1]; }
                            break;
                        case RevitLNK.st_steel_C:
                            shapename = "溝形鋼";
                            sid_name = SetFamily.SGirC.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][2]; }
                            else
                            { fami = ConvFamily[1][2]; }
                            break;
                        case RevitLNK.st_steel_L:
                            shapename = "山形鋼";
                            sid_name = SetFamily.SGirL.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][3]; }
                            else
                            { fami = ConvFamily[1][3]; }
                            break;
                        case RevitLNK.st_steel_LipC:
                            shapename = "リップ溝形鋼";
                            sid_name = SetFamily.SGirLipC.SecId;
                            if (isCanti)
                            { fami = ConvCFamily[1][4]; }
                            else
                            { fami = ConvFamily[1][4]; }
                            break;
                        default:
                            return ret;
                    }
                    break;
                case "SRC":
                    shapename = "H形鋼";
                    sid_name = SetFamily.SRCGirH.SecId;
                    sid = section.StbSecBeams_SRC[sgirind].id;

                    floor = section.StbSecBeams_SRC[sgirind].floor;
                    find = Get_stbFloor_index(stb, floor);
                    if (find == -1)
                    { find = Get_stbFloor_index(stb, gir.idNode_start); }
                    if (find != -1)
                    {
                        //typename = (find + 1).ToString();
                        typename = stb.StbModel.StbStories[find].name;
                    }
                    typename += section.StbSecBeams_SRC[sgirind].name;

                    isCanti = section.StbSecBeams_SRC[sgirind].isCanti;
                    isOutin = section.StbSecBeams_SRC[sgirind].isOutIn;

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
                LogData.AddLog(LogData.LogKind.Warning, 2100, gir.kind_structure + "梁" + shapename);
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
                if (!SearchFamilySymbol(fami, typename, ref symbol, sid, sid_name))
                {
                    //ReNameされているとき
                    symbol = null;
                    for (int i = 0; i < GirderSymbols.Count(); i++)
                    {
                        if (GirderSymbols[i].id != gir.id_section) { continue; }
                        if (GirderSymbols[i].Length == gir.haunch_start && GirderSymbols[i].Length2 == gir.haunch_end)
                        {
                            symbol = GirderSymbols[i].symbol;
                            break;
                        }
                    }
                    if (symbol == null)
                    {
                        //ログ表示(タイプが無い)
                        LogData.AddLog(LogData.LogKind.Warning, 2300, "[" + gir.kind_structure + "小梁]" + typename + "(配置id=" + gir.id + ")");
                        return ret;
                    }
                }
            }

            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, gir.idNode_end, false);
            int indt = Get_stbFloor_index(stb, gir.idNode_start, false);
            Level btmLevel = null;
            if (indb == -1 && indt == -1)
            {
                btmLevel = SearchLevel_height(stb, gir.idNode_start, gir.idNode_end);
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
            XYZ Ps_org = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
            XYZ Pe_org = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
            if (Ps_org.DistanceTo(Pe_org) < Commons.mm2ft(1))
            {
                string log = "小梁の生成：" + "\t" + "[配置Id " + gir.id.ToString() + "]" + typename + ",[節点Id";
                log += MakeLog_Coord(0, new int[] { gir.idNode_start, gir.idNode_end });
                log += "] ";

                LogData.AddLog(LogData.LogKind.Warning, 3100, log);
                return ret; //falseは変換失敗
            }

            XYZ vecU = (Pe_org - Ps_org).Normalize();

            //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
            bool se_offset_flg = false; //始端・終端にオフセットが入っている⇒true

            XYZ offsetstart = new XYZ();
            XYZ offsetend = new XYZ();
            XYZ offsetstart2 = new XYZ();
            XYZ offsetend2 = new XYZ();

            XYZ Ps_xy = new XYZ();
            XYZ Pe_xy = new XYZ();

            if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_end_Z != 0 ||
                gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0)
            {
                offsetstart = TransformCoord(Ps_org, Pe_org, gir.offset_start_X, gir.offset_start_Y, gir.offset_start_Z, -gir.rotate);
                offsetend = TransformCoord(Ps_org, Pe_org, gir.offset_end_X, gir.offset_end_Y, gir.offset_end_Z, -gir.rotate);

                se_offset_flg = true;

                Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(gir.offset_start_X), Ps_org.Y + Commons.mm2ft(gir.offset_start_Y), Ps_org.Z);
                Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(gir.offset_end_X), Pe_org.Y + Commons.mm2ft(gir.offset_end_Y), Pe_org.Z);
            }
            else
            {
                offsetstart = Search_Offset_gir(stb, gir.idNode_start, ref Ps_org, ref Pe_org, "start", vecU, gir.id, btmLevel, -gir.rotate, out offsetstart2);
                offsetend = Search_Offset_gir(stb, gir.idNode_end, ref Ps_org, ref Pe_org, "end", vecU, gir.id, btmLevel, -gir.rotate, out offsetend2);

                Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
                Pe_xy = Pe_org + Commons.mm2ft(offsetend2);
            }

            //梁描画用節点（部材方向のオフセットだけ考慮、それ以外のオフセットはパラメータに入力）
            XYZ Ps = Set_offset(Ps_org, offsetstart, vecU);
            XYZ Pe = Set_offset(Pe_org, offsetend, vecU);

            //継手計算用に全てのオフセットを考慮した節点を求める　2017/08/23
            //XYZ Ps_xy = new XYZ(Ps_org.X + Commons.mm2ft(gir.offset_start_X), Ps_org.Y + Commons.mm2ft(gir.offset_start_Y), Ps_org.Z);
            //XYZ Pe_xy = new XYZ(Pe_org.X + Commons.mm2ft(gir.offset_end_X), Pe_org.Y + Commons.mm2ft(gir.offset_end_Y), Pe_org.Z);
            //XYZ Ps_xy = Ps_org + Commons.mm2ft(offsetstart2);
            //XYZ Pe_xy = Pe_org + Commons.mm2ft(offsetend2);


            //端点オフセットのZ座標が入っていないとき
            if (gir.offset_start_Z == 0 && gir.offset_end_Z == 0)
            {
                //レベル方向のオフセットはSTART/END_Z_OFFSET_VALUEに統一
                //Ps = new XYZ(Ps.X, Ps.Y, Ps.Z + Commons.mm2ft(gir.level));
                //Pe = new XYZ(Pe.X, Pe.Y, Pe.Z + Commons.mm2ft(gir.level));
                offsetstart = offsetstart + new XYZ(0, 0, gir.level);
                offsetend = offsetend + new XYZ(0, 0, gir.level);
            }

            Line gir_L = Line.CreateBound(Ps, Pe);
            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (isOutin)
                {
                    FamilySymbol newsymbol = Create_newsymbol_isOutin(stb, symbol, gir.id, "GIRDER", isCanti);
                    if (newsymbol != null)
                    { instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam); }
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(gir_L, symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                }

                //ジオメトリ：各オフセット
                SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                if (se_offset_flg)
                {
                    SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                    SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                    SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                    SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                }
                else
                {
                    SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, gir.offset, true);
                    SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, gir.offset, true);
                    SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                    SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);
                }

                //断面回転
                SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-gir.rotate * Math.PI) / 180);

                //RC・S・SRC・CFTに共通のパラメータ(元々あるパラメータ)
                SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.Joist); //構造用途（小梁）

                SetParameter(instance, BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM, btmLevel.Id);

                switch (gir.kind_structure)
                {
                    case "RC":
                        FamilyStructure.RC_Gir Rgir = SetFamily.RCGir;
                        SetParameter(instance.LookupParameter(Rgir.MemId), gir.id);
                        SetParameter(instance.LookupParameter(Rgir.NameMembers), gir.name);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_top), gir.thickness_ex_top);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_right), gir.thickness_ex_right);
                        SetParameter(instance.LookupParameter(Rgir.thickness_ex_left), gir.thickness_ex_left);
                        SetParameter(instance.LookupParameter(Rgir.kind_haunch_start), gir.kind_haunch_start);
                        SetParameter(instance.LookupParameter(Rgir.kind_haunch_end), gir.kind_haunch_end);
                        SetParameter(instance.LookupParameter(Rgir.type_haunch_H), gir.type_haunch_H);
                        SetParameter(instance.LookupParameter(Rgir.type_haunch_V), gir.type_haunch_V);
                        break;
                    case "S":
                        if (!isCanti)
                        {
                            Create_GirderandBeam_S_instance(stb, shape, instance, Ps_xy, Pe_xy, Ps_org, Pe_org, null, gir);
                        }
                        else
                        {
                            FamilyStructure.S_CGir_H Hgir = SetFamily.SCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                            if (fami.Name == "Steel_CG_H")
                            {
                                bool joint = false;
                                double joint_start = gir.joint_start;
                                if (gir.joint_start != 0)
                                { joint = true; }
                                else
                                {
                                    joint = false;
                                }
                                SetParameter(instance.LookupParameter("継手"), joint);
                                Commons.doc.Regenerate();
                                if (joint_start == 0)
                                { joint_start = 1; }
                                SetParameter(instance.LookupParameter(Hgir.joint_start), joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                            else
                            {
                                SetParameter(instance.LookupParameter(Hgir.joint_start), gir.joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                        }
                        break;
                    case "SRC":
                        if (!isCanti)
                        {
                            FamilyStructure.SRC_Gir Hgir = SetFamily.SRCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_ex_top);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_ex_right);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_ex_left);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            int numjoint = 0;
                            if (gir.joint_start != 0)
                            { numjoint++; }
                            if (gir.joint_end != 0)
                            { numjoint++; }
                            SetParameter(instance.LookupParameter("継手数"), numjoint);
                            Commons.doc.Regenerate();
                            if (numjoint != 0)
                            {
                                double joint_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps_xy, Pe_xy, gir.idNode_start);
                                SetParameter(instance.LookupParameter(Hgir.joint_start), joint_s);
                                double joint_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe_xy, Ps_xy, gir.idNode_end);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), joint_e);
                            }
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                        }
                        else
                        {
                            FamilyStructure.SRC_CGir Hgir = SetFamily.SRCCGirH;
                            SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                            SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_top), gir.thickness_ex_top);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_bottom), gir.thickness_ex_bottom);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_right), gir.thickness_ex_right);
                            SetParameter(instance.LookupParameter(Hgir.thickness_ex_left), gir.thickness_ex_left);
                            SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                            SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                            SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                            int numjoint = 0;
                            if (gir.joint_start != 0)
                            { numjoint++; }
                            if (gir.joint_end != 0)
                            { numjoint++; }
                            SetParameter(instance.LookupParameter("継手数"), numjoint);
                            Commons.doc.Regenerate();
                            if (numjoint != 0)
                            {
                                SetParameter(instance.LookupParameter(Hgir.joint_start), gir.joint_start, true);
                                SetParameter(instance.LookupParameter(Hgir.joint_end), gir.joint_end, true);
                            }
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                            SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                        }
                        break;

                }

                //解析線分作成
                Commons.doc.Regenerate();
                AnalyticalMember member = AnalyticalMember.Create(Commons.doc, Line.CreateBound(Ps_org, Pe_org));
                if (member != null)
                {
                    //構造の役割
                    var p = member.get_Parameter(BuiltInParameter.ANALYTICAL_ELEMENT_STRUCTURAL_ROLE);
                    if (p != null)
                    {
                        p.Set((int)AnalyticalStructuralRole.StructuralRoleBeam);
                    }

                    var amanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(Commons.doc);
                    amanager.AddAssociation(member.Id, instance.Id);
                }


                //変換情報ログの出力
                var nodeIds = new int[] { gir.idNode_start, gir.idNode_end } ;
                MakeNodeLog( "小梁の生成：", "[配置Id " + gir.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, gir.id, "小梁", typename, nodeIds );

                Commons.doc.Regenerate();
                CGrp_Add(stb, gir.idNode_start, gir.idNode_end, instance.Id, instance);
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }
        
        /// <summary> 大梁・小梁S造インスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="shape"></param>
        /// <param name="instance"></param>
        /// <param name="gir"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        private void Create_GirderandBeam_S_instance(STBclass stb, string shape, FamilyInstance instance, XYZ Ps, XYZ Pe, XYZ Ps_org, XYZ Pe_org,
                                                STBclass.StbModelClass.StbMembersClass.StbGirder gir = null, STBclass.StbModelClass.StbMembersClass.StbBeam beam = null)
        {
            //継手距離はSTBの値そのままでなく、計算値をセットする（2017/08/21）                    
            double join_s = 0;
            double join_e = 0;
            int numjoint = 0;
            if (gir != null)
            {
                join_s = Get_Joint(stb, gir.joint_start, Ps_org, Ps, Pe, gir.idNode_start);
                join_e = Get_Joint(stb, gir.joint_end, Pe_org, Pe, Ps, gir.idNode_end);
                if (gir.joint_start != 0) { numjoint++; }
                if (gir.joint_end != 0) { numjoint++; }
            }
            else if (beam != null)
            {
                join_s = Get_Joint(stb, beam.joint_start, Ps_org, Ps, Pe, beam.idNode_start);
                join_e = Get_Joint(stb, beam.joint_end, Pe_org, Pe, Ps, beam.idNode_end);
                if (beam.joint_start != 0) { numjoint++; }
                if (beam.joint_end != 0) { numjoint++; }
            }


            if (shape == RevitLNK.st_steel_H)
            {
                FamilyStructure.S_Gir_H Hgir = (gir != null ? SetFamily.SGirH : SetFamily.SBeamH);
                if (gir != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                    if (instance.Symbol.FamilyName == "Steel_Girder_H" || instance.Symbol.FamilyName == "Steel_Beam_H")
                    {
                        //ハンチ種類
                        if (gir.kind_haunch_start == "SLOPE" || gir.kind_haunch_end == "SLOPE")
                        {
                            Make_haunchLog("DROP", instance.Symbol.Name, gir.id);
                        }
                        //水平ハンチ形状
                        if (gir.type_haunch_H != "BOTH" || gir.type_haunch_H != "")
                        {
                            Make_haunchLog("BOTH_H", instance.Symbol.Name, gir.id);
                        }
                        //鉛直ハンチ形状
                        if (gir.type_haunch_V != "BOTH" || gir.type_haunch_V != "")
                        {
                            Make_haunchLog("BOTH_V", instance.Symbol.Name, gir.id);
                        }
                    }
                }
                if (beam != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), beam.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), beam.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), beam.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), beam.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), beam.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), beam.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), beam.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), beam.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), beam.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), beam.kind_joint_end);
                    if (instance.Symbol.FamilyName == "Steel_Girder_H" || instance.Symbol.FamilyName == "Steel_Beam_H")
                    {
                        //ハンチ種類
                        if (beam.kind_haunch_start == "SLOPE" || beam.kind_haunch_end == "SLOPE")
                        {
                            Make_haunchLog("DROP", instance.Symbol.Name, beam.id);
                        }
                        //水平ハンチ形状
                        if (beam.type_haunch_H != "BOTH" || beam.type_haunch_H != "")
                        {
                            Make_haunchLog("BOTH_H", instance.Symbol.Name, beam.id);
                        }
                        //鉛直ハンチ形状
                        if (beam.type_haunch_V != "BOTH" || beam.type_haunch_V != "")
                        {
                            Make_haunchLog("BOTH_V", instance.Symbol.Name, beam.id);
                        }
                    }
                }
            }
            else if (shape == RevitLNK.st_steel_BH)
            {
                FamilyStructure.S_Gir_BH Hgir = (gir != null ? SetFamily.SGirBH : SetFamily.SBeamBH);
                if (gir != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                    if (instance.Symbol.FamilyName == "Steel_Girder_BH")
                    {
                        //ハンチ種類
                        if (gir.kind_haunch_start == "DROP" || gir.kind_haunch_end == "DROP")
                        {
                            Make_haunchLog("SLOPE", instance.Symbol.Name, gir.id);
                        }
                        //水平ハンチ形状
                        if (gir.type_haunch_H != "BOTH" || gir.type_haunch_H != "")
                        {
                            Make_haunchLog("BOTH_H", instance.Symbol.Name, gir.id);
                        }
                        //鉛直ハンチ形状
                        if (gir.type_haunch_V != "TOP" || gir.type_haunch_V != "")
                        {
                            Make_haunchLog("TOP", instance.Symbol.Name, gir.id);
                        }
                    }
                }
                if (beam != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), beam.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), beam.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), beam.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), beam.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), beam.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), beam.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), beam.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), beam.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), beam.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), beam.kind_joint_end);
                    if (instance.Symbol.FamilyName == "Steel_Girder_BH")
                    {
                        //ハンチ種類
                        if (beam.kind_haunch_start == "DROP" || beam.kind_haunch_end == "DROP")
                        {
                            Make_haunchLog("SLOPE", instance.Symbol.Name, beam.id);
                        }
                        //水平ハンチ形状
                        if (beam.type_haunch_H != "BOTH" || beam.type_haunch_H != "")
                        {
                            Make_haunchLog("BOTH_H", instance.Symbol.Name, beam.id);
                        }
                        //鉛直ハンチ形状
                        if (beam.type_haunch_V != "TOP" || beam.type_haunch_V != "")
                        {
                            Make_haunchLog("TOP", instance.Symbol.Name, beam.id);
                        }
                    }
                }
            }
            else if (shape == RevitLNK.st_steel_C)
            {
                FamilyStructure.S_Gir_C Hgir = (gir != null ? SetFamily.SGirC : SetFamily.SBeamC);
                if (gir != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                }
                if (beam != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), beam.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), beam.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), beam.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), beam.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), beam.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), beam.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), beam.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), beam.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), beam.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), beam.kind_joint_end);
                }
            }
            else if (shape == RevitLNK.st_steel_L)
            {
                FamilyStructure.S_Gir_L Hgir = (gir != null ? SetFamily.SGirL : SetFamily.SBeamL);
                if (gir != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                }
                if (beam != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), beam.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), beam.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), beam.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), beam.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), beam.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), beam.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), beam.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), beam.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), beam.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), beam.kind_joint_end);
                }
            }
            else if (shape == RevitLNK.st_steel_LipC)
            {
                FamilyStructure.S_Gir_LipC Hgir = (gir != null ? SetFamily.SGirLipC : SetFamily.SBeamLipC);
                if (gir != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), gir.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), gir.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), gir.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), gir.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), gir.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), gir.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), gir.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), gir.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), gir.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), gir.kind_joint_end);
                }
                if (beam != null)
                {
                    SetParameter(instance.LookupParameter(Hgir.MemId), beam.id);
                    SetParameter(instance.LookupParameter(Hgir.NameMembers), beam.name);
                    SetParameter(instance.LookupParameter(Hgir.condition_start), beam.condition_start);
                    SetParameter(instance.LookupParameter(Hgir.condition_end), beam.condition_end);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_start), beam.kind_haunch_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_haunch_end), beam.kind_haunch_end);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_H), beam.type_haunch_H);
                    SetParameter(instance.LookupParameter(Hgir.type_haunch_V), beam.type_haunch_V);
                    SetParameter(instance.LookupParameter("継手数"), numjoint);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Hgir.joint_start), join_s);
                    SetParameter(instance.LookupParameter(Hgir.joint_end), join_e);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_start), beam.kind_joint_start);
                    SetParameter(instance.LookupParameter(Hgir.kind_joint_end), beam.kind_joint_end);
                }
            }
        }

        private FamilySymbol Create_newsymbol_isOutin(STBclass stb, FamilySymbol symbol, int mid, string kind, bool isCanti)
        {
            string section_io_start = "", section_io_end = "";
            int secid = 0;
            string kind_structure = "";
            for(int i =0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
            {
                if(kind != "GIRDER") { break; }
                if(mid == stb.StbModel.StbMembers.StbGirders[i].id)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                    section_io_start = gir.section_io_start;
                    section_io_end = gir.section_io_end;
                    secid = gir.id_section;
                    kind_structure = stb.StbModel.StbMembers.StbGirders[i].kind_structure;
                    break;
                }
            }
            for(int i =0; i < stb.StbModel.StbMembers.StbBeams.Count();i++)
            {
                if (kind != "BEAM") { break; }
                if(mid == stb.StbModel.StbMembers.StbBeams[i].id)
                {
                    STBclass.StbModelClass.StbMembersClass.StbBeam gir = stb.StbModel.StbMembers.StbBeams[i];
                    section_io_start = gir.section_io_start;
                    section_io_end = gir.section_io_end;
                    secid = gir.id_section;
                    kind_structure = gir.kind_structure;
                    break;
                }
            }

            FamilySymbol newsymbol = null;
            if(section_io_start == "") { section_io_start = "OUT"; }
            if(section_io_end == "") { section_io_end = "IN"; }

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

                string newtypename = ReName(symbol.Name, 97);
                if (!SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
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
                string newtypename = ReName(symbol.Name, 97);
                if (!SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
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
                string newtypename = ReName(symbol.Name, 97);
                if (!SearchFamilySymbol(symbol.Family, newtypename, ref newsymbol))
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
                            SetParameter(newsymbol.LookupParameter(Rgir.bar_length_start), symbol.LookupParameter(Rgir.bar_length_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if(section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.bar_length_end), symbol.LookupParameter(Rgir.bar_length_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                        //配筋
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i]).AsDouble());
                        }
                    }
                    else
                    {
                        FamilyStructure.RC_CGir Rgir = SetFamily.RCCGir;
                        //形状
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.bar_length_start), symbol.LookupParameter(Rgir.bar_length_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.bar_length_end), symbol.LookupParameter(Rgir.bar_length_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                        //配筋
                        for (int i = 0; i < 2; i++)
                        {
                            int k = j[i];
                            if(i == 1)
                            {
                                k = j[2];
                            }
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[k]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[k]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[k]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[k]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[k]), symbol.LookupParameter(Rgir.count_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[k]), symbol.LookupParameter(Rgir.count_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[k]), symbol.LookupParameter(Rgir.count_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[k]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[k]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[k]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[k]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[k]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[k]), symbol.LookupParameter(Rgir.D_stirrup[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[k]), symbol.LookupParameter(Rgir.count_stirrup[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[k]), symbol.LookupParameter(Rgir.pitch_stirrup[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[k]), symbol.LookupParameter(Rgir.D_reinforcement_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_web[k]), symbol.LookupParameter(Rgir.count_web[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[k]), symbol.LookupParameter(Rgir.D_bar_spacing[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[k]), symbol.LookupParameter(Rgir.count_bar_spacing[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[k]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i]).AsDouble());
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
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SCGirBH.FamilyName || newsymbol.FamilyName == SetFamily.SCGirH.FamilyName)
                    {
                        FamilyStructure.S_CGir_H Rgir = SetFamily.SCGirH;
                        //鉄骨形状
                        for (int i = 0; i < 2; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirBH.FamilyName)
                    {
                        FamilyStructure.S_Gir_BH Rgir = SetFamily.SGirBH;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirC.FamilyName || newsymbol.FamilyName == SetFamily.SCGirC.FamilyName)
                    {
                        FamilyStructure.S_Gir_C Rgir = SetFamily.SGirC;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.H[j[i]]), symbol.LookupParameter(Rgir.H[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r1[j[i]]), symbol.LookupParameter(Rgir.r1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r2[j[i]]), symbol.LookupParameter(Rgir.r2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i]).AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirLipC.FamilyName || newsymbol.FamilyName == SetFamily.SCGirLipC.FamilyName)
                    {
                        FamilyStructure.S_Gir_LipC Rgir = SetFamily.SGirLipC;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.H[j[i]]), symbol.LookupParameter(Rgir.H[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.C[j[i]]), symbol.LookupParameter(Rgir.C[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t[j[i]]), symbol.LookupParameter(Rgir.t[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i]).AsString());
                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SGirL.FamilyName || newsymbol.FamilyName == SetFamily.SCGirL.FamilyName)
                    {
                        FamilyStructure.S_Gir_L Rgir = SetFamily.SGirL;
                        //鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r1[j[i]]), symbol.LookupParameter(Rgir.r1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r2[j[i]]), symbol.LookupParameter(Rgir.r2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.side[j[i]]), symbol.LookupParameter(Rgir.side[i]).AsString());

                        }
                        //ハンチ
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
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
                            SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                        //配筋・鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i]).AsDouble());

                            SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());

                        }
                    }
                    else if (newsymbol.FamilyName == SetFamily.SRCCGirH.FamilyName)
                    {
                        FamilyStructure.SRC_CGir Rgir = SetFamily.SRCCGirH;
                        //形状
                        if (section_io_start == "IN")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.width_start), symbol.LookupParameter(Rgir.width_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_start), symbol.LookupParameter(Rgir.depth_end).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[0]), symbol.LookupParameter(Rgir.BHaunch[1]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_start), symbol.LookupParameter(Rgir.haunch_end).AsDouble());
                        }
                        if (section_io_end == "OUT")
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.width_end), symbol.LookupParameter(Rgir.width_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.depth_end), symbol.LookupParameter(Rgir.depth_start).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.BHaunch[1]), symbol.LookupParameter(Rgir.BHaunch[0]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.haunch_end), symbol.LookupParameter(Rgir.haunch_start).AsDouble());
                        }
                        //配筋・鉄骨形状
                        for (int i = 0; i < 3; i++)
                        {
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_top[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_2nd_main_bottom[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_top_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_top_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_1st[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_2nd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[j[i]]), symbol.LookupParameter(Rgir.count_2nd_main_bottom_3rd[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_stirrup[j[i]]), symbol.LookupParameter(Rgir.D_stirrup[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_stirrup[j[i]]), symbol.LookupParameter(Rgir.count_stirrup[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_stirrup[j[i]]), symbol.LookupParameter(Rgir.pitch_stirrup[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_reinforcement_web[j[i]]), symbol.LookupParameter(Rgir.D_reinforcement_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_web[j[i]]), symbol.LookupParameter(Rgir.count_web[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.D_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.D_bar_spacing[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.count_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.count_bar_spacing[i]).AsInteger());
                            SetParameter(newsymbol.LookupParameter(Rgir.pitch_bar_spacing[j[i]]), symbol.LookupParameter(Rgir.pitch_bar_spacing[i]).AsDouble());

                            SetParameter(newsymbol.LookupParameter(Rgir.strength_web[j[i]]), symbol.LookupParameter(Rgir.strength_web[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.strength_main[j[i]]), symbol.LookupParameter(Rgir.strength_main[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.A[j[i]]), symbol.LookupParameter(Rgir.A[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.B[j[i]]), symbol.LookupParameter(Rgir.B[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t1[j[i]]), symbol.LookupParameter(Rgir.t1[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.t2[j[i]]), symbol.LookupParameter(Rgir.t2[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.r[j[i]]), symbol.LookupParameter(Rgir.r[i]).AsDouble());
                            SetParameter(newsymbol.LookupParameter(Rgir.shape[j[i]]), symbol.LookupParameter(Rgir.shape[i]).AsString());
                            SetParameter(newsymbol.LookupParameter(Rgir.type[j[i]]), symbol.LookupParameter(Rgir.type[i]).AsString());

                        }
                    }
                    break;
            }

            IsOutin_Girder newOIG = new IsOutin_Girder();
            newOIG.id = secid;
            newOIG.section_io_start = section_io_start;
            newOIG.section_io_end = section_io_end;
            newOIG.symbol = newsymbol;
            isOutin_G.Add(newOIG);
            return newsymbol;
        }

       
        #endregion
        #region ブレース
        /// <summary> ブレースの生成 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <returns></returns>
        private bool CreateBrace(STBclass stb, ProgressBarForm pform, string syubetu, IList<Element> elements, ref string errmsg)
        {
            bool ret = true;

            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            ProgressBar_Show(pform, syubetu + "の生成");

            //変換ファミリの取得              
            Family[][] ConvFamily = new Family[RevitLNK.SBraText.Length][];
            for (int i = 0; i < RevitLNK.SBraText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.SBraText[i].Length);
            }

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            //IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }

            int numfamily = 0; //変換するファミリの数


            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.SBraFName.flg[i][j]) { continue; }
                    if (!SetFamily.SBraFName.convflg[i][j]) { continue; }

                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }

                        if (familysymbol.FamilyName == SetFamily.SBraFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {
                                //プログレスバーの表示
                                GaugePercent("パラメータ追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name + "パラメータ追加");
                                
                                try
                                {
                                    tran1.Start();
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;

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
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.SBraFName.FamilyName, familysymbol.FamilyName, i, j);

                                    break;
                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
                                    doc.Close(false);
                                }
                            }
                            numfamily++;
                        }
                    }
                }
            }

           
            Transaction tran = new Transaction(Commons.doc, syubetu + "の生成");
            try
            {
                tran.Start();
                
                //作ったタイプリスト
                List<TypeName_Data> typename_list = new List<TypeName_Data>();
                if (stb.StbModel.StbSections.StbSecBraces_S != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecBraces_S.Count();

                    for (int i = 0; i < numCount; i++)
                    {

                        STBclass.StbModelClass.StbSectionsClass.StbSecBrace_S bra = stb.StbModel.StbSections.StbSecBraces_S[i];

                        //プログレスバーの表示
                        GaugePercent("Sブレースの生成", (int)((double)i / (double)numCount * 100));
                        if (!CreateBrace_S(stb, bra, pform, ConvFamily, ref typename_list)) { ret = false; errmsg = "Sブレース"; }
                    }
                }



                if (stb.StbModel.StbMembers.StbBraces != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbBraces.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbBrace bra = stb.StbModel.StbMembers.StbBraces[i];
                        int sbraind = -1;
                        for (int j = 0; j < stb.StbModel.StbSections.StbSecBraces_S.Count(); j++)
                        {
                            if (stb.StbModel.StbSections.StbSecBraces_S[j].id == bra.id_section)
                            {
                                sbraind = j;
                                break;
                            }
                        }
                        if (sbraind == -1) { continue; }
                        //プログレスバーの表示
                        GaugePercent("ブレースの生成", (int)((double)i / (double)numCount * 100));
                        if (!CreateBrace_instance(stb, bra, sbraind, pform, ConvFamily)) { ret = false; errmsg = "Sブレースインスタンス"; }
                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran.Commit();
                pform.TopMost = true;

                //タイプができているかチェック
                IList<Element> elements_end = collector.WherePasses(filter).WhereElementIsElementType().ToElements();                
                for(int i = 0; i < typename_list.Count(); i++)
                {
                    bool flg = false;
                    foreach (Element el in elements_end)
                    {
                        FamilySymbol symbol = el as FamilySymbol;
                        if (symbol == null) { continue; }
                        if (symbol.Name == typename_list[i].typename)
                        {
                            flg = true;
                            break;
                        }
                    }
                    if(!flg)
                    {
                        LogData.AddLog(LogData.LogKind.Warning, 0, "[" + typename_list[i].shapename + "]" + typename_list[i].typename + 
                                       "(断面id=" + typename_list[i].id.ToString() + ")を生成できませんでした。寸法値またはファミリの設定を確認してください。" );
                    }
                    
                }
            }
            catch (Exception e)
            {
                e.ToString();
                ret = false;
                errmsg = "Sブレース";
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }


            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }
           
            return ret;
        }
        /// <summary> Sブレースタイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateBrace_S(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecBrace_S bra, ProgressBarForm pform, Family[][] ConvFamily, ref List<TypeName_Data> typename_list)
        {
            bool ret = true;
            string shapename = "";

            //タイプ名
            string typename = "";
            string floor = bra.floor;
            if (floor != "")
            {
                int find = Get_stbFloor_index(stb, floor);
                if (find == -1)
                { find = Get_stbFloor_index_Gir(stb, bra.id); }
                if (find != -1)
                { typename = stb.StbModel.StbStories[find].name; ; }
            }
            typename += bra.name;

            //鉄骨形状を取得
            STBclass.StbModelClass.StbSectionsClass.StbSecBrace_S.StbSecSteelBraceClass Sbra = null;
            int shapeid = -1;
            string shape = "";
            //shape = Check_Steel(stb, bra.StbSecSteelBrace[1].shape, ref shapeid);
            if (bra.StbSecSteelBrace[1] != null)
            {
                Sbra = bra.StbSecSteelBrace[1];
                shape = Check_Steel(stb, bra.StbSecSteelBrace[1].shape, ref shapeid);
            }
            else if (bra.StbSecSteelBrace[0] != null)
            {
                Sbra = bra.StbSecSteelBrace[0];
                shape = Check_Steel(stb, bra.StbSecSteelBrace[0].shape, ref shapeid);
            }
            else if (bra.StbSecSteelBrace[2] != null)
            {
                Sbra = bra.StbSecSteelBrace[2];
                shape = Check_Steel(stb, bra.StbSecSteelBrace[2].shape, ref shapeid);
            }

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
                if (Sbra != null)
                { LogData.AddLog(LogData.LogKind.Warning, 2500, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")の鉄骨形状[" + Sbra.shape + "]"); }
                else
                { LogData.AddLog(LogData.LogKind.Warning, 3000, "[Sブレース]" + bra.name + "(断面id=" + bra.id.ToString() + ")はブレース鉄骨情報"); }                
                return ret;
            }

            if (shape == RevitLNK.st_steel_H || shape == RevitLNK.st_steel_BH || shape == RevitLNK.st_steel_C || shape == RevitLNK.st_steel_L || shape == RevitLNK.st_steel_LipC)
            {
                int[] ind = new int[3];
                int[] shapeids = new int[3];
                for (int j = 0; j < bra.StbSecSteelBrace.Count(); j++)
                {
                    if (bra.StbSecSteelBrace[j] == null)
                    {
                        switch (j)
                        {
                            case 0:
                                if (bra.StbSecSteelBrace[1] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[1].shape, ref shapeids[j]);
                                    ind[0] = 1;
                                }
                                else if (bra.StbSecSteelBrace[2] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[2].shape, ref shapeids[j]);
                                    ind[0] = 2;
                                }
                                break;
                            case 1:
                                if (bra.StbSecSteelBrace[0] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[0].shape, ref shapeids[j]);
                                    ind[1] = 0;
                                }
                                else if (bra.StbSecSteelBrace[2] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[2].shape, ref shapeids[j]);
                                    ind[1] = 2;
                                }
                                break;
                            case 2:
                                if (bra.StbSecSteelBrace[1] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[1].shape, ref shapeids[j]);
                                    ind[2] = 1;
                                }
                                else if (bra.StbSecSteelBrace[0] != null)
                                {
                                    Check_Steel(stb, bra.StbSecSteelBrace[0].shape, ref shapeids[j]);
                                    ind[2] = 0;
                                }
                                break;
                        }
                    }
                    else
                    {
                        Check_Steel(stb, bra.StbSecSteelBrace[j].shape, ref shapeids[j]);
                        ind[j] = j;
                    }
                }

                //H形鋼・組立H形鋼以外のファミリは、断面が1断面しか入力できない⇒全断面でないときは中央、始端、終端の優先順 
                if (shape != RevitLNK.st_steel_H && shape != RevitLNK.st_steel_BH)
                {
                    if (bra.StbSecSteelBrace[0].pos != "ALL")
                    {
                        if (bra.StbSecSteelBrace[1] != null)
                        {
                            Sbra = bra.StbSecSteelBrace[1];
                            shape = Check_Steel(stb, bra.StbSecSteelBrace[1].shape, ref shapeid);

                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + bra.id.ToString() + ")は" + shape + "(" + shapename + ")のため中央断面で変換しました。");
                        }
                        else if (bra.StbSecSteelBrace[0] != null)
                        {
                            Sbra = bra.StbSecSteelBrace[0];
                            shape = Check_Steel(stb, bra.StbSecSteelBrace[0].shape, ref shapeid);

                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + bra.id.ToString() + ")は" + shape + "(" + shapename + ")のため中央断面で変換しました。");
                        }
                        else if (bra.StbSecSteelBrace[2] != null)
                        {
                            Sbra = bra.StbSecSteelBrace[2];
                            shape = Check_Steel(stb, bra.StbSecSteelBrace[2].shape, ref shapeid);

                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[Sブレース]" + typename + "(断面id=" + bra.id.ToString() + ")は" + shape + "(" + shapename + ")のため中央断面で変換しました。");
                        }
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
                                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel =
                                                                                                stb.StbModel.StbSections.StbSecSteel.StbSecRoll_H[shapeids[j]];

                                string logtxt = Roll_H_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                A[j] = steel.A;
                                B[j] = steel.B;
                                type[j] = steel.type;
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
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }
                            }
                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[0][0], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                            FamilyStructure.S_Bra_H Rbra_H = SetFamily.SBraH;
                            SetParameter(symbol.LookupParameter(Rbra_H.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra_H.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra_H.name), bra.name);
                            for (int j = 0; j < 3; j++)
                            {
                                SetParameter(symbol.LookupParameter(Rbra_H.strength_web[j]), GetStrength_web(bra.StbSecSteelBrace[ind[j]].strength_web, bra.StbSecSteelBrace[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rbra_H.strength_main[j]), bra.StbSecSteelBrace[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rbra_H.shape[j]), bra.StbSecSteelBrace[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rbra_H.type[j]), type);
                                SetParameter(symbol.LookupParameter(Rbra_H.A[j]), A[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_H.B[j]), B[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_H.t1[j]), t1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_H.t2[j]), t2[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_H.r[j]), r[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_H.type[j]), type[j]);
                            }
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
                                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel =
                                                                                                  stb.StbModel.StbSections.StbSecSteel.StbSecBuild_H[shapeids[j]];

                                string logtxt = Build_H_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                A[j] = steel.A;
                                B[j] = steel.B;
                                t1[j] = steel.t1;
                                t2[j] = steel.t2;

                            }
                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[0][1], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_BH Rbra_BH = SetFamily.SBraBH;

                            SetParameter(symbol.LookupParameter(Rbra_BH.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra_BH.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra_BH.name), bra.name);
                            for (int j = 0; j < 3; j++)
                            {
                                SetParameter(symbol.LookupParameter(Rbra_BH.strength_web[j]), GetStrength_web(bra.StbSecSteelBrace[ind[j]].strength_web, bra.StbSecSteelBrace[ind[j]].strength_main));
                                SetParameter(symbol.LookupParameter(Rbra_BH.strength_main[j]), bra.StbSecSteelBrace[j].strength_main);
                                SetParameter(symbol.LookupParameter(Rbra_BH.shape[j]), bra.StbSecSteelBrace[j].shape);
                                SetParameter(symbol.LookupParameter(Rbra_BH.A[j]), A[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_BH.B[j]), B[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_BH.t1[j]), t1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_BH.t2[j]), t2[j], true);
                            }
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
                                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steel =
                                  stb.StbModel.StbSections.StbSecSteel.StbSecRoll_C[shapeids[j]];

                                string logtxt = Roll_C_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                A[j] = steel.A;
                                B[j] = steel.B;
                                t1[j] = steel.t1;
                                t2[j] = steel.t2;
                                r1[j] = steel.r1;
                                r2[j] = steel.r2;
                                type[j] = steel.type;
                                side[j] = steel.side;
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
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }

                            }
                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_C Rbra_C = SetFamily.SBraC;

                            SetParameter(symbol.LookupParameter(Rbra_C.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra_C.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra_C.name), bra.name);
                            for (int j = 0; j < 3; j++)
                            {
                                SetParameter(symbol.LookupParameter(Rbra_C.strength), bra.StbSecSteelBrace[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rbra_C.shape[j]), bra.StbSecSteelBrace[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rbra_C.type[j]), type[j]);
                                SetParameter(symbol.LookupParameter(Rbra_C.side[j]), side[j]);
                                SetParameter(symbol.LookupParameter(Rbra_C.H[j]), A[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_C.B[j]), B[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_C.t1[j]), t1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_C.t2[j]), t2[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_C.r1[j]), r1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_C.r2[j]), r2[j], true);
                            }
                            if (type[0] == "2C")
                            { Make_typeLog(typename, bra.id, RevitLNK.st_steel_C, "溝形鋼", false); }
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
                                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steel =
                                 stb.StbModel.StbSections.StbSecSteel.StbSecRoll_L[shapeids[j]];

                                string logtxt = Roll_L_Size_Check(steel);
                                if (logtxt != "")
                                {
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
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
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }
                                A[j] = steel.A;
                                B[j] = steel.B;
                                t1[j] = steel.t1;
                                t2[j] = steel.t2;
                                r1[j] = steel.r1;
                                r2[j] = steel.r2;
                                type[j] = steel.type;
                                side[j] = steel.side;
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
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 1);
                                }
                            }
                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[1][1], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_L Rbra_L = SetFamily.SBraL;

                            SetParameter(symbol.LookupParameter(Rbra_L.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra_L.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra_L.kind_brace), bra.kind_brace);
                            for (int j = 0; j < 3; j++)
                            {
                                SetParameter(symbol.LookupParameter(Rbra_L.strength), bra.StbSecSteelBrace[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rbra_L.shape[j]), bra.StbSecSteelBrace[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rbra_L.type[j]), type[j]);
                                SetParameter(symbol.LookupParameter(Rbra_L.side[j]), side[j]);
                                SetParameter(symbol.LookupParameter(Rbra_L.A[j]), A[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_L.B[j]), B[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_L.t1[j]), t1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_L.t2[j]), t2[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_L.r1[j]), r1[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_L.r2[j]), r2[j], true);
                            }
                            if (type[0] == "2L")
                            { Make_typeLog(typename, bra.id, RevitLNK.st_steel_L, "山形鋼", false); }
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
                                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_LipC_Class steel =
                                  stb.StbModel.StbSections.StbSecSteel.StbSecRoll_LipC[shapeids[j]];

                                string logtxt = Rool_LipC_Size_Check(steel);

                                if (logtxt != "")
                                {
                                    MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                    return ret;
                                }
                                H[j] = steel.H;
                                A[j] = steel.A;
                                C[j] = steel.C;
                                t[j] = steel.t;
                                side[j] = steel.side;
                                type[j] = steel.type;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[1][2], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_LipC Rbra_LipC = SetFamily.SBraLipC;

                            SetParameter(symbol.LookupParameter(Rbra_LipC.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra_LipC.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra_LipC.kind_brace), bra.kind_brace);
                            for (int j = 0; j < 3; j++)
                            {
                                SetParameter(symbol.LookupParameter(Rbra_LipC.strength), bra.StbSecSteelBrace[ind[j]].strength_main);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.shape[j]), bra.StbSecSteelBrace[ind[j]].shape);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.type[j]), type[j]);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.side[j]), side[j]);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.H[j]), H[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.A[j]), A[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.C[j]), C[j], true);
                                SetParameter(symbol.LookupParameter(Rbra_LipC.t[j]), t[j], true);
                            }
                            if (type[0] == "2C")
                            { Make_typeLog(typename, bra.id, RevitLNK.st_steel_LipC, "リップ溝形鋼", false); }
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
                            STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_BOX_Class steel =
                                 stb.StbModel.StbSections.StbSecSteel.StbSecRoll_BOX[shapeid];
                            string logtxt = Roll_Box_Size_Check(steel);
                            if (logtxt != "")
                            {
                                MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[0][2], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_Box Rbra = SetFamily.SBraBox;

                            SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra.strength), Sbra.strength_main);
                            SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra.shape), Sbra.shape);
                            SetParameter(symbol.LookupParameter(Rbra.H), steel.A, true);
                            SetParameter(symbol.LookupParameter(Rbra.B), steel.B, true);
                            SetParameter(symbol.LookupParameter(Rbra.t1), steel.t, true);
                            SetParameter(symbol.LookupParameter(Rbra.t2), steel.t, true);
                            SetParameter(symbol.LookupParameter(Rbra.r), steel.R, true);
                            SetParameter(symbol.LookupParameter(Rbra.type), steel.type);
                            SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
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
                            STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_BOX_Class steel =
                                stb.StbModel.StbSections.StbSecSteel.StbSecBuild_BOX[shapeid];
                            string logtxt = Build_Box_Size_Check(steel);
                            if (logtxt != "")
                            {
                                MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[0][3], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_BBox Rbra = SetFamily.SBraBBox;

                            SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
                            SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra.strength), Sbra.strength_main);
                            SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra.shape), Sbra.shape);
                            SetParameter(symbol.LookupParameter(Rbra.H), steel.A, true);
                            SetParameter(symbol.LookupParameter(Rbra.B), steel.B, true);
                            SetParameter(symbol.LookupParameter(Rbra.t1), steel.t1, true);
                            SetParameter(symbol.LookupParameter(Rbra.t2), steel.t2, true);
                            SetParameter(symbol.LookupParameter("フィレット"), 0.0, true);
                            SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
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
                            STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecPipe_Class steel =
                                 stb.StbModel.StbSections.StbSecSteel.StbSecPipe[shapeid];
                            string logtxt = Pipe_Size_Check(steel);
                            if (logtxt != "")
                            {
                                MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[0][4], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_Pipe Rbra = SetFamily.SBraPipe;

                            SetParameter(symbol.LookupParameter(Rbra.strength), Sbra.strength_main);
                            SetParameter(symbol.LookupParameter(Rbra.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra.shape), Sbra.shape);
                            SetParameter(symbol.LookupParameter(Rbra.D), steel.D, true);
                            SetParameter(symbol.LookupParameter(Rbra.t), steel.t, true);
                            SetParameter(symbol.LookupParameter(Rbra.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra.SecId), bra.id);
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
                            STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_FB_Class steel =
                                     stb.StbModel.StbSections.StbSecSteel.StbSecRoll_FB[shapeid];
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
                                MakeSizeLog("Sブレース" + shapename, typename, bra.id, logtxt, 0);
                                return ret;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[1][3], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_FB Rbra_FB = SetFamily.SBraFB;


                            SetParameter(symbol.LookupParameter(Rbra_FB.strength_main), Sbra.strength_main);
                            SetParameter(symbol.LookupParameter(Rbra_FB.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra_FB.shape), Sbra.shape);
                            SetParameter(symbol.LookupParameter(Rbra_FB.B), steel.B, true);
                            SetParameter(symbol.LookupParameter(Rbra_FB.t), steel.t, true);
                            SetParameter(symbol.LookupParameter(Rbra_FB.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra_FB.SecId), bra.id);

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
                            STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_Bar_Class steel =
                                  stb.StbModel.StbSections.StbSecSteel.StbSecRoll_Bar[shapeid];

                            if (steel.R == 0)
                            {
                                MakeSizeLog("Sブレース" + shapename, typename, bra.id, "直径", 0);
                                return ret;
                            }

                            FamilySymbol symbol = null;
                            if (!SearchFamilySymbol(ConvFamily[1][4], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.S_Bra_RollBar Rbra_Bar = SetFamily.SBraRollBar;
                            SetParameter(symbol.LookupParameter(Rbra_Bar.strength_main), Sbra.strength_main);
                            SetParameter(symbol.LookupParameter(Rbra_Bar.kind_brace), bra.kind_brace);
                            SetParameter(symbol.LookupParameter(Rbra_Bar.shape), Sbra.shape);
                            SetParameter(symbol.LookupParameter(Rbra_Bar.D), steel.R, true);
                            SetParameter(symbol.LookupParameter(Rbra_Bar.name), bra.name);
                            SetParameter(symbol.LookupParameter(Rbra_Bar.SecId), bra.id);
                        }
                        break;
                    default:
                        //ログ（変換対象外）
                        Make_taisyougaiLog("Sブレース", bra.id, bra.name, shape, "T形鋼");
                        return ret;
                }
            }

            TypeName_Data td = new TypeName_Data();
            td.typename = typename;
            td.id = bra.id;
            td.shapename = "Sブレース";
            typename_list.Add(td);
            return ret;
        }
        
        /// <summary>ブレースインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="sgirind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily">大梁のファミリ</param>
        /// <param name="ConvCFamily">片持ち大梁のファミリ</param>
        /// <returns></returns>
        private bool CreateBrace_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbBrace bra, int sbraind, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            string floor = "";
            Family fami = null;
            string shape = "";
            int ind = 0;
            //タイプ名
            string typename = "";
            //所属層のindex
            int find = 0;


            //使用するファミリの取

            floor = section.StbSecBraces_S[sbraind].floor;
            find = Get_stbFloor_index(stb, floor);
            if (find != -1)
            {typename = stb.StbModel.StbStories[find].name;  }
             typename += section.StbSecBraces_S[sbraind].name; 

            shape = Check_Steel(stb, section.StbSecBraces_S[sbraind].StbSecSteelBrace[0].shape, ref ind);
            
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
            if (!SearchFamilySymbol(fami, typename, ref symbol))
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[Sブレース]" + bra.name + "(配置Id=" + bra.id.ToString() + ")");
                return ret;
            }
            
            //配置レベルの取得
            int indb = Get_stbFloor_index(stb, bra.idNode_end, false);
            int indt = Get_stbFloor_index(stb, bra.idNode_start, false);
            Level btmLevel = null;
            if (indb == -1 && indt == -1)
            {
                btmLevel = SearchLevel_height(stb, bra.idNode_start, bra.idNode_end);
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
            string kind_brace = section.StbSecBraces_S[sbraind].kind_brace;
            if(kind_brace == "")
            {
                int s_ind = Get_stbFloor_index(stb, bra.idNode_start);
                int e_ind = Get_stbFloor_index(stb, bra.idNode_end);
                if(s_ind != e_ind) { kind_brace = "VERTICAL"; }
                else { kind_brace = "HORIZONTAL"; }
            }
            //配置座標の取得
            XYZ Ps = Get_Node_Position(stb, bra.idNode_start, 0, 0, 0);
            XYZ Pe = Get_Node_Position(stb, bra.idNode_end, 0, 0, 0);
            if (Ps.DistanceTo(Pe) < Commons.mm2ft(1))
            {
                string log = "ブレースの生成：" + "\t" + "[配置Id " + bra.id.ToString() + "]" + typename + ",[節点Id";
                log += MakeLog_Coord(0, new int[] { bra.idNode_start, bra.idNode_end });
                log += "] ";

                LogData.AddLog(LogData.LogKind.Warning, 3100, log);
                return ret; //falseは変換失敗
            }

            XYZ vecU = (Pe - Ps).Normalize();

            //オフセット（設定画面で設定したレベルのオフセットは、梁の始端・終端とbtmLevelのElevationの差が自動で入力されるので計算に含まない）
            XYZ offsetstart = new XYZ();
            if (bra.offset_start_X != 0 || bra.offset_start_Y != 0 || bra.offset_start_Z != 0)
            { offsetstart = TransformCoord(Ps, Pe, bra.offset_start_X, bra.offset_start_Y, bra.offset_start_Z, -bra.rotate); }
            else
            { offsetstart = Search_Offset_bra(stb, bra.idNode_start, Ps, Pe, "start", kind_brace, -bra.rotate); }
            XYZ offsetend = new XYZ();
            if (bra.offset_end_X != 0 || bra.offset_end_Y != 0 || bra.offset_end_Z != 0)
            { offsetend = TransformCoord(Ps, Pe, bra.offset_end_X, bra.offset_end_Y, bra.offset_end_Z, -bra.rotate); }
            else
            { offsetend = Search_Offset_bra(stb, bra.idNode_end, Ps, Pe, "end", kind_brace, -bra.rotate); }

            Ps = Set_offset(Ps, offsetstart, vecU, true);
            Pe = Set_offset(Pe, offsetend, vecU, true);
            //インスタンスの生成
            try
            {
                FamilyInstance instance = null;
                if (kind_brace == "HORIZONTAL")
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Line.CreateBound(Ps, Pe), symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                    SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.HorizontalBracing); //構造用途 
                }
                else
                {
                    instance = Commons.doc.Create.NewFamilyInstance(Line.CreateBound(Ps, Pe), symbol, btmLevel, Autodesk.Revit.DB.Structure.StructuralType.Brace);
                    SetParameter(instance, BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM, StructuralInstanceUsage.Brace); //構造用途 
                }

                //解析線分作成
                Commons.doc.Regenerate();
                XYZ Ps_org = Get_Node_Position(stb, bra.idNode_start, 0, 0, 0);
                XYZ Pe_org = Get_Node_Position(stb, bra.idNode_end, 0, 0, 0);
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
                SetParameter(instance, BuiltInParameter.YZ_JUSTIFICATION, 1);
                SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);

                //断面回転
                SetParameter(instance, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE, (-bra.rotate * Math.PI) / 180);

                              
                SetParameter(instance, BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM, btmLevel.Id); //参照レベル
                


                SetParameter(instance, BuiltInParameter.START_Y_OFFSET_VALUE, offsetstart.Y, true);
                SetParameter(instance, BuiltInParameter.END_Y_OFFSET_VALUE, offsetend.Y, true);
                SetParameter(instance, BuiltInParameter.START_Z_OFFSET_VALUE, offsetstart.Z, true);
                SetParameter(instance, BuiltInParameter.END_Z_OFFSET_VALUE, offsetend.Z, true);

                if (fami.Name == SetFamily.SBraH.FamilyName)
                {
                    FamilyStructure.S_Bra_H Rbra = SetFamily.SBraH;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    int joint_num = 0;
                    if (bra.joint_start != 0) { joint_num++; }
                    if (bra.joint_end != 0) { joint_num++; }
                    SetParameter(instance.LookupParameter("継手数"), joint_num);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraBH.FamilyName)
                {
                    FamilyStructure.S_Bra_BH Rbra = SetFamily.SBraBH;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    int joint_num = 0;
                    if (bra.joint_start != 0) { joint_num++; }
                    if (bra.joint_end != 0) { joint_num++; }
                    SetParameter(instance.LookupParameter("継手数"), joint_num);
                    Commons.doc.Regenerate();
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraBox.FamilyName)
                {
                    FamilyStructure.S_Bra_Box Rbra = SetFamily.SBraBox;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraBBox.FamilyName)
                {
                    FamilyStructure.S_Bra_BBox Rbra = SetFamily.SBraBBox;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraPipe.FamilyName)
                {
                    FamilyStructure.S_Bra_Pipe Rbra = SetFamily.SBraPipe;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraC.FamilyName)
                {
                    FamilyStructure.S_Bra_C Rbra = SetFamily.SBraC;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraL.FamilyName)
                {
                    FamilyStructure.S_Bra_L Rbra = SetFamily.SBraL;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraLipC.FamilyName)
                {
                    FamilyStructure.S_Bra_LipC Rbra = SetFamily.SBraLipC;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraFB.FamilyName)
                {
                    FamilyStructure.S_Bra_FB Rbra = SetFamily.SBraFB;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }
                else if (fami.Name == SetFamily.SBraRollBar.FamilyName)
                {
                    FamilyStructure.S_Bra_RollBar Rbra = SetFamily.SBraRollBar;
                    SetParameter(instance.LookupParameter(Rbra.MemId), bra.id);
                    SetParameter(instance.LookupParameter(Rbra.NameMembers), bra.name);
                    SetParameter(instance.LookupParameter(Rbra.condition_start), bra.condition_start);
                    SetParameter(instance.LookupParameter(Rbra.condition_end), bra.condition_end);
                    SetParameter(instance.LookupParameter(Rbra.joint_start), bra.joint_start, true);
                    SetParameter(instance.LookupParameter(Rbra.joint_end), bra.joint_end, true);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_start), bra.kind_joint_start);
                    SetParameter(instance.LookupParameter(Rbra.kind_joint_end), bra.kind_joint_end);
                    SetParameter(instance.LookupParameter(Rbra.future_brace), bra.future_brace);
                }

                //変換情報ログの出力
                var nodeIds = new int[] { bra.idNode_start, bra.idNode_end } ;
                MakeNodeLog( "ブレースの生成：", "[配置Id " + bra.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, bra.id, "ブレース", typename, nodeIds ) ;
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }
        #endregion
        #region スラブ
        /// <summary> スラブの生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateSlab(STBclass stb, ProgressBarForm pform, string buzai, ref string errmsg)
        {
            bool ret = true;

            bool isFoundation = false;
            string catename = ""; //使用するファミリカテゴリ名
            if (buzai == "基礎スラブ")
            { isFoundation = true; }

            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            ProgressBar_Show(pform, "スラブの生成");

          
            List<int> ids = new List<int>();
            List<string> typenames = new List<string>();
            List<FloorType> symbols = new List<FloorType>();

            bool paraflg = true; //パラメータを追加した⇒false
            if (!isFoundation)
            {
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

                catename = "構造床";

                if (elements == null || elements.Count() == 0)
                {
                    ret = false;
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "構造床");
                    return ret;
                }
                else
                {
                    foreach (Element el in elements)
                    {
                        FloorType symbol = el as FloorType;
                        if (symbol == null) { continue; }

                        if (paraflg)
                        {
                            GaugePercent("パラメータの追加", (int)((double)1 / (double)1 * 100));
                            ParaSet.SetPara_Slab("床", el, SetFamily.Slab);
                            paraflg = false;
                        }

                        symbols.Add(symbol);
                    }
                }
            }
            else
            {
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

                catename = "基礎床";

                if (elements == null || elements.Count() == 0)
                {
                    ret = false;
                    LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎床");
                    return ret;
                }
                else
                {
                    foreach (Element el in elements)
                    {
                        FloorType symbol = el as FloorType;
                        if (symbol == null) { continue; }

                        if (paraflg)
                        {
                            GaugePercent("パラメータの追加", (int)((double)1 / (double)1 * 100));
                            ParaSet.SetPara_Slab("構造基礎", el, SetFamily.Slab);
                            paraflg = false;
                        }

                        symbols.Add(symbol);
                    }
                }
            }
            if (symbols.Count() == 0)
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
                        if (stb.StbModel.StbSections.StbSecSlabs_RC != null)
                        {
                            
                            int numCount = stb.StbModel.StbSections.StbSecSlabs_RC.Count();
                            for (int i = 0; i < numCount; i++)
                            {
                                GaugePercent(buzai + "の生成", (int)((double)i / (double)numCount * 100));

                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC sla = stb.StbModel.StbSections.StbSecSlabs_RC[i];

                                if (sla.isFoundation != isFoundation) { continue; }

                                string typename = sla.name;
                                //もし、名前がかぶっていたらReNameする
                                bool nameflg = true;
                                int ascii = 97;
                                string oldname = typename;
                                do
                                {
                                    nameflg = true;
                                    for (int j = 0; j < symbols.Count(); j++)
                                    {
                                        if(symbols[j].Name.Equals(typename, StringComparison.CurrentCultureIgnoreCase))
                                        //if (symbols[j].Name == typename)
                                        {
                                            typename = ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                FloorType symbol = null;
                                symbol = (FloorType)symbols[0].Duplicate(typename);
                                symbols.Add(symbol);

                                if (!CreateRCSlab(stb, sla, pform, symbol)) { errmsg = buzai; }
                                ids.Add(sla.id);
                                typenames.Add(typename);
                            }
                        }
                        break;
                    case "デッキプレート":
                        if (stb.StbModel.StbSections.StbSecSlabs_Deck != null)
                        {
                            int numCount = stb.StbModel.StbSections.StbSecSlabs_Deck.Count();
                            for (int i = 0; i < numCount; i++)
                            {
                                GaugePercent("デッキスラブの生成", (int)((double)i / (double)numCount * 100));

                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck sla = stb.StbModel.StbSections.StbSecSlabs_Deck[i];

                                string typename = sla.name;

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
                                        //if (symbols[j].Name == typename)
                                        {
                                            typename = ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                FloorType symbol = null;
                                symbol = (FloorType)symbols[0].Duplicate(typename);
                                symbols.Add(symbol);

                                if (!CreateDeckSlab(stb, sla, pform, symbol)) { errmsg = "デッキスラブ"; }
                                ids.Add(sla.id);
                                typenames.Add(typename);
                            }
                        }
                        break;
                    case "既製スラブ":
                        if (stb.StbModel.StbSections.StbSecSlabs_Precast != null)
                        {
                            int numCount = stb.StbModel.StbSections.StbSecSlabs_Precast.Count();
                            for (int i = 0; i < numCount; i++)
                            {
                                GaugePercent("既製スラブの生成", (int)((double)i / (double)numCount * 100));

                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast sla = stb.StbModel.StbSections.StbSecSlabs_Precast[i];

                                string typename = sla.name;

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
                                        //if (symbols[j].Name == typename)
                                        {
                                            typename = ReName(oldname, ascii);
                                            ascii++;
                                            nameflg = false;
                                            break;
                                        }
                                    }
                                }
                                while (!nameflg);

                                FloorType symbol = null;
                                symbol = (FloorType)symbols[0].Duplicate(typename);
                                symbols.Add(symbol);

                                if (!CreateProductSlab(stb, sla, pform, symbol)) { errmsg = "既製スラブ"; }
                                ids.Add(sla.id);
                                typenames.Add(typename);
                            }
                        }
                        break;
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();                
                tran.Commit();
                pform.TopMost = true;
                
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }

            Transaction tran2 = new Transaction(Commons.doc, "スラブインスタンスパラメータの設定");
            
            try
            {
                tran2.Start();
               
                errmsg = "スラブインスタンス";
                if (stb.StbModel.StbMembers.StbSlabs != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbSlabs.Count();
                    for (int i = 0; i < numCount; i++)
                    {

                        STBclass.StbModelClass.StbMembersClass.StbSlab sla = stb.StbModel.StbMembers.StbSlabs[i];

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

                        GaugePercent("スラブの生成", (int)((double)i / (double)numCount * 100));

                        if (!CreateSlab_instance(stb, sla, pform, symbol, buzai, ref errmsg, isFoundation))
                        {                           
                            if (errmsg == "")
                            { errmsg = "スラブインスタンス"; }
                        }
                        Commons.doc.Regenerate();

                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran2.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran2.RollBack();
                pform.TopMost = true;
                LogData.AddLog(LogData.LogKind.Error, 0, errmsg);
            }



            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }
            return ret;
        }
       
        /// <summary> RCスラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool CreateRCSlab(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC sla, ProgressBarForm pform, FloorType symbol)
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
                    val = stb.StbCommon.concrete_strength;
                }
                SetMaterial(ref val, ref eid);
                double depth = 0;
                if (sla.StbSecFigure != null)
                {
                    switch (sla.StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecFigureClass.StbSecStraightClass stra = sla.StbSecFigure.StbSecStraight;
                            depth = stra.depth;
                            SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_base), 0, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_tip), 0, true);
                            SetParameter(symbol.LookupParameter(Rsla.length_haunch), 0, true);
                            break;
                        case 2:
                            STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecFigureClass.StbSecTaperClass tap = sla.StbSecFigure.StbSecTaper;
                            depth = (tap.depth_base + tap.depth_tip) / 2;
                            SetParameter(symbol.LookupParameter(Rsla.depth_base), tap.depth_base, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_tip), tap.depth_tip, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_center), 0.0, true);
                            SetParameter(symbol.LookupParameter(Rsla.length_haunch), 0, true);
                            break;
                        case 3:
                            STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecFigureClass.StbSecHaunchClass hau = sla.StbSecFigure.StbSecHaunch;
                            depth = hau.depth_center;
                            SetParameter(symbol.LookupParameter(Rsla.depth_base), hau.depth_base, true);
                            SetParameter(symbol.LookupParameter(Rsla.length_haunch), hau.length_haunch, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);
                            SetParameter(symbol.LookupParameter(Rsla.depth_tip), 0, true);
                            break;
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
               
                SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                SetParameter(symbol.LookupParameter(Rsla.isEarthen), sla.isEarthen);
                if (sla.isCanti)
                {
                    SetParameter(symbol.LookupParameter(Rsla.isCanti), "片持ち");
                }
                else
                {
                    SetParameter(symbol.LookupParameter(Rsla.isCanti), "一般");
                }
                SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.depth_cover_top, true);
                SetParameter(symbol.LookupParameter(Rsla.depth_cover_bottom), sla.depth_cover_bottom, true);

                if (sla.StbSecBar_Arrangement != null)
                {
                    string strength = "";
                    string[] D = new string[2];
                    switch (sla.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            if(sla.StbSecBar_Arrangement.StbSecStandard_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSecStandard_Slab.Count(); j++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class sec = sla.StbSecBar_Arrangement.StbSecStandard_Slab[j];
                                if (sec == null) { continue; }

                                int keiflg = Get_D(sec.D, ref D);
                                SetParameter(symbol.LookupParameter(Rsla.D1[j]), D[0]);
                                SetParameter(symbol.LookupParameter(Rsla.D2[j]), D[1]);
                                SetParameter(symbol.LookupParameter(Rsla.pitch[j]), sec.pitch, true);

                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(1, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "RCスラブ", keiflg);
                                }

                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 2:
                            if(sla.StbSecBar_Arrangement.StbSec2Way_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec2Way_Slab.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class sec = sla.StbSecBar_Arrangement.StbSec2Way_Slab[j];
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(2, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "RCスラブ", keiflg);
                                }
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
                                Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch);
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 3:
                            if(sla.StbSecBar_Arrangement.StbSec1Way_Slab_1 == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ1配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec1Way_Slab_1.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_1_Class sec = sla.StbSecBar_Arrangement.StbSec1Way_Slab_1[j];
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(3, j);                                   
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "RCスラブ", keiflg);
                                }
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
                                Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch);
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 4:
                            if(sla.StbSecBar_Arrangement.StbSec1Way_Slab_2 == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ2配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec1Way_Slab_2.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_RC.StbSecBar_ArrangementClass.StbSec1Way_Slab_2_Class sec = sla.StbSecBar_Arrangement.StbSec1Way_Slab_2[j];
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(4, j);                                  
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "RCスラブ", keiflg);
                                }
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
                                Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch);
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                    }

                    Parameter_Select_Set(Rsla.strength, strength, floor:symbol);                    
                }
                else //2017/05/19 鉄筋タグが無い→ログ出力
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RCスラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                }
            }
            catch(Exception)
            { ret = false; }
           
            return ret;
        }
        /// <summary> デッキスラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool CreateDeckSlab(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck sla, ProgressBarForm pform, FloorType symbol)
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
                    val = stb.StbCommon.concrete_strength;
                }
                SetMaterial(ref val, ref eid);

                //構造のスラブ厚・コンクリート強度を設定
                CompoundStructure csSlab = symbol.GetCompoundStructure();
                double depth = 0;
                if (sla.depth_concrete < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[デッキスラブ]" + sla.name + "(断面id=" + sla.id.ToString() + ")" + "床厚が0mmなので1mmとして変換します。");
                }
                else
                {
                    depth = sla.depth_concrete;
                }
                csSlab.SetLayerWidth(0, Commons.mm2ft(depth));
                if (eid != null)
                { csSlab.SetMaterialId(0, eid); }
                symbol.SetCompoundStructure(csSlab);

                SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                SetParameter(symbol.LookupParameter(Rsla.product_type), sla.product_type);
                SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.depth_cover_top, true);
                SetParameter(symbol.LookupParameter(Rsla.depth_cover_bottom), sla.depth_cover_bottom, true);
                SetParameter(symbol.LookupParameter(Rsla.isCanti), "一般");
                
                if (sla.StbSecBar_Arrangement != null)
                {
                    string strength = "";
                    string[] D = new string[2];
                    switch (sla.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            if(sla.StbSecBar_Arrangement.StbSecStandard_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSecStandard_Slab.Count(); j++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class sec = sla.StbSecBar_Arrangement.StbSecStandard_Slab[j];
                                if (sec == null) { continue; }


                                int keiflg = Get_D(sec.D, ref D);
                                SetParameter(symbol.LookupParameter(Rsla.D1[j]), D[0]);
                                SetParameter(symbol.LookupParameter(Rsla.D2[j]), D[1]);
                                SetParameter(symbol.LookupParameter(Rsla.pitch[j]), sec.pitch, true);

                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(1, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "デッキスラブ", keiflg);
                                }

                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 2:
                            if(sla.StbSecBar_Arrangement.StbSec2Way_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec2Way_Slab.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class sec = sla.StbSecBar_Arrangement.StbSec2Way_Slab[j];
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(2, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "デッキスラブ", keiflg);
                                }
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
                                Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch);
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 3:
                            if(sla.StbSecBar_Arrangement.StbSec1Way_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec1Way_Slab.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Deck.StbSecBar_ArrangementClass.StbSec1Way_Slab_Class sec = sla.StbSecBar_Arrangement.StbSec1Way_Slab[j];
                                if(sec == null) { continue; }
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(5, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "デッキスラブ", keiflg);
                                }
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
                                        SetParameter(symbol.LookupParameter(Rsla.addD), D[0]);
                                        SetParameter(symbol.LookupParameter(Rsla.addpitch), sec.pitch, true);
                                        break;
                                }
                                if (ind != null)
                                { Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch); }
                                
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                    }

                    Parameter_Select_Set(Rsla.strength, strength, floor: symbol);
                }
                else //2017/05/19 鉄筋タグが無い→ログ出力
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[デッキスラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                }
                if (sla.StbSecDeck_Product != null)
                {
                   
                    SetParameter(symbol.LookupParameter(Rsla.product_company), sla.StbSecDeck_Product.product_company);
                    SetParameter(symbol.LookupParameter(Rsla.product_code), sla.StbSecDeck_Product.product_code);
                    SetParameter(symbol.LookupParameter(Rsla.depth_center), sla.StbSecDeck_Product.deck_depth, true);
                }
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
        /// <summary> 既製スラブタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool CreateProductSlab(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast sla, ProgressBarForm pform, FloorType symbol)
        {
            bool ret = true;

            FamilyStructure.Slab Rsla = SetFamily.Slab;

            try
            {
                //コンクリート強度のElementIdを取得
                ElementId eid = null;
                Object val = sla.strength_concrete;
                if(sla.strength_concrete == "")
                {
                    val = stb.StbCommon.concrete_strength;
                }
                SetMaterial(ref val, ref eid);
                //構造のスラブ厚・コンクリート強度を設定
                CompoundStructure csSlab = symbol.GetCompoundStructure();
                //床厚が0だと変換できない⇒1mmに設定する
                double depth = 0;
                if(sla.depth_concrete < 1)
                {
                    depth = 1;
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[既製スラブ]" + sla.name + "(断面id=" + sla.id.ToString() + ")" + "床厚が0mmなので1mmとして変換します。");
                }
                else
                {
                    depth = sla.depth_concrete;
                }
                csSlab.SetLayerWidth(0, Commons.mm2ft(depth));
                if (eid != null)
                { csSlab.SetMaterialId(0, eid); }
                symbol.SetCompoundStructure(csSlab);

                SetParameter(symbol.LookupParameter(Rsla.SecId), sla.id);
                SetParameter(symbol.LookupParameter(Rsla.name), sla.name);
                SetParameter(symbol.LookupParameter(Rsla.depth_cover_top), sla.depth_cover_top, true);
                SetParameter(symbol.LookupParameter(Rsla.depth_center), depth, true);
                if (sla.StbSecBar_Arrangement != null)
                {
                    string strength = "";
                    string[] D = new string[2];
                    switch (sla.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            if(sla.StbSecBar_Arrangement.StbSecStandard_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "標準スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSecStandard_Slab.Count(); j++)
                            {
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast.StbSecBar_ArrangementClass.StbSecStandard_Slab_Class sec = sla.StbSecBar_Arrangement.StbSecStandard_Slab[j];
                                if (sec == null) { continue; }

                                int keiflg = Get_D(sec.D, ref D);
                                SetParameter(symbol.LookupParameter(Rsla.D1[j]), D[0]);
                                SetParameter(symbol.LookupParameter(Rsla.D2[j]), D[1]);
                                SetParameter(symbol.LookupParameter(Rsla.pitch[j]), sec.pitch, true);

                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(1, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "既製スラブ", keiflg);
                                }

                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 2:
                            if(sla.StbSecBar_Arrangement.StbSec2Way_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "2方向スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec2Way_Slab.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast.StbSecBar_ArrangementClass.StbSec2Way_Slab_Class sec = sla.StbSecBar_Arrangement.StbSec2Way_Slab[j];
                                if(sec == null) { continue; }
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(2, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "既製スラブ", keiflg);
                                }
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
                                Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch);
                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                        case 3:
                            if(sla.StbSecBar_Arrangement.StbSec1Way_Slab == null) { break; }
                            SetParameter(symbol.LookupParameter(Rsla.ArrengementType), "1方向スラブ配筋");
                            for (int j = 0; j < sla.StbSecBar_Arrangement.StbSec1Way_Slab.Count(); j++)
                            {
                                int[] ind = new int[0];
                                STBclass.StbModelClass.StbSectionsClass.StbSecSlab_Precast.StbSecBar_ArrangementClass.StbSec1Way_Slab_Class sec = sla.StbSecBar_Arrangement.StbSec1Way_Slab[j];
                                if(sec == null) { continue; }
                                int keiflg = Get_D(sec.D, ref D);
                                //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                if (keiflg != 0)
                                {
                                    string pos = SlabTekkinkei_PosName(5, j);
                                    Make_TekkinkeiLog(symbol.Name, sla.id, pos, "既製スラブ", keiflg);
                                }
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
                                if (ind != null)
                                { Set_Slab_Bar_Arrangement(symbol, Rsla, ind, D, sec.pitch); }

                                if (strength == "")
                                {
                                    strength = sec.strength;
                                }
                                else
                                {
                                    strength = Compare_strength(strength, sec.strength);
                                }
                            }
                            break;
                    }

                    Parameter_Select_Set(Rsla.strength, strength, floor: symbol);
                }
                else //2017/05/19 鉄筋タグが無い→ログ出力
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[既製スラブ]" + symbol.Name + "(断面id=" + sla.id.ToString() + ")");
                }
                if (sla.StbSecSlabP_Precast != null)
                {
                    SetParameter(symbol.LookupParameter(Rsla.product_company), sla.StbSecSlabP_Precast.product_company);
                    SetParameter(symbol.LookupParameter(Rsla.product_name), sla.StbSecSlabP_Precast.product_name);
                    SetParameter(symbol.LookupParameter(Rsla.product_code), sla.StbSecSlabP_Precast.product_code);
                    SetParameter(symbol.LookupParameter(Rsla.product_depth), sla.StbSecSlabP_Precast.deck_depth, true);
                }
              
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slabtype">1：標準 2：2方向 3：1方向1 4:1方向2 5：1方向</param>
        /// <param name="j"></param>
        /// <returns></returns>
        private string SlabTekkinkei_PosName(int slabtype, int j)
        {
            string pos = "";
            switch(slabtype)
            {
                case 1:
                    switch (j)
                    {
                        case 0:
                            pos = "SHORT_TOP_COLUMN";
                            break;
                        case 1:
                            pos = "SHORT_TOP_MID_END";
                            break;
                        case 2:
                            pos = "SHORT_TOP_MID_CENTER";
                            break;
                        case 3:
                            pos = "SHORT_BOTTOM_COLUMN";
                            break;
                        case 4:
                            pos = "SHORT_BOTTOM_MID_END";
                            break;
                        case 5:
                            pos = "SHORT_BOTTOM_MID_CENTER";
                            break;
                        case 6:
                            pos = "LONG_TOP_COLUMN";
                            break;
                        case 7:
                            pos = "LONG_TOP_MID_END";
                            break;
                        case 8:
                            pos = "LONG_TOP_MID_CENTER";
                            break;
                        case 9:
                            pos = "LONG_BOTTOM_COLUMN";
                            break;
                        case 10:
                            pos = "LONG_BOTTOM_MID_END";
                            break;
                        case 11:
                            pos = "LONG_BOTTOM_MID_CENTER";
                            break;
                    }
                    break;
                case 2:
                    switch (j)
                    {
                        case 0:
                            pos = "SHORT_TOP";
                            break;
                        case 1:
                            pos = "SHORT_BOTTOM";
                            break;
                        case 2:
                            pos = "LONG_TOP";
                            break;
                        case 3:
                            pos = "LONG_BOTTOM";
                            break;
                    }
                    break;
                case 3:
                    switch (j)
                    {
                        case 0:
                            pos = "MAINT_TOP";
                            break;
                        case 1:
                            pos = "MAIN_BOTTOM";
                            break;
                        case 2:
                            pos = "TRANSVERS_TOP";
                            break;
                        case 3:
                            pos = "TRANSVERS_BOTTOM";
                            break;
                    }
                    break;
                case 4:
                    switch (j)
                    {
                        case 0:
                            pos = "MAINT_BASE_TOP";
                            break;
                        case 1:
                            pos = "MAIN_BASE_BOTTOM";
                            break;
                        case 2:
                            pos = "MAINT_TIP_TOP";
                            break;
                        case 3:
                            pos = "MAIN_TIP_BOTTOM";
                            break;
                        case 4:
                            pos = "TRANSVERS_TOP";
                            break;
                        case 5:
                            pos = "TRANSVERS_BOTTOM";
                            break;
                    }
                    break;
                case 5:
                    switch (j)
                    {
                        case 0:
                            pos = "MAINT_TOP";
                            break;
                        case 1:
                            pos = "MAIN_BOTTOM";
                            break;
                        case 2:
                            pos = "TRANSVERS_TOP";
                            break;
                        case 3:
                            pos = "TRANSVERS_BOTTOM";
                            break;
                        case 4:
                            pos = "REFRACTORY";
                            break;
                    }
                    break;
            }
            return pos;

        }
        /// <summary> スラブインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <param name="isFoundaion"></param>
        /// <returns></returns>
        private bool CreateSlab_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbSlab sla, ProgressBarForm pform, FloorType symbol, string buzai, ref string errmsg, bool isFoundaion = false)
        {
            bool ret = true;

            List<Curve> profile = new List<Curve>();
            Level btmLevel = null;
            try
            {
                //同じ節点idが含まれているときがある→同じものは消去
                for (int i = 0; i < sla.StbNodeid_List.Count();i++)
                {
                    for(int j = i + 1; j < sla.StbNodeid_List.Count(); j++)
                    {
                        if(sla.StbNodeid_List[i].id == sla.StbNodeid_List[j].id)
                        {
                            sla.StbNodeid_List.RemoveAt(j);
                            j--;
                        }
                    }
                }
                if (sla.StbNodeid_List.Count < 3)
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + buzai + "]" + sla.name + "(配置Id=" + sla.id.ToString() + "節点数が3未満のスラブ");
                    return ret;
                }

                //節点をオフセットを考慮した値に直す
                List<XYZ> Point0 = new List<XYZ>();
                List<XYZ> Point1 = new List<XYZ>();
                List<int> stbfloorid = new List<int>();
                for(int i = 0; i < sla.StbNodeid_List.Count(); i++)
                {
                    XYZ Pa = null;
                    //設定画面で設定したレベルのオフセットは自動で計算してくれるので、個々の計算には含まない
                    if(sla.StbSlabOffset_List != null)
                    { Pa = Get_Node_Position(stb, sla.StbNodeid_List[i].id, sla.StbSlabOffset_List[i].offset_X, sla.StbSlabOffset_List[i].offset_Y, sla.level); }
                    else
                    { Pa = Get_Node_Position(stb, sla.StbNodeid_List[i].id, 0, 0, sla.level); }
                     
                    if(Point0.Count() > 0)
                    {
                        if(Pa.DistanceTo(Point0[Point0.Count() - 1] ) < gosa)
                        {
                            continue;
                        }
                    }
                    Point0.Add(Pa);
                    Point1.Add(Get_Node_Position(stb, sla.StbNodeid_List[i].id, 0, 0, 0));
                    int flid = Get_stbFloor_index(stb, sla.StbNodeid_List[i].id);
                    stbfloorid.Add(flid);
                }

                //閉じた図形になっているか確認
                for(int i =0; i < Point0.Count(); i++)
                {
                    XYZ Cc = new XYZ();
                    int cs = -1;
                    if (i == Point0.Count() - 1)
                    {
                        for(int j = 1; j < Point0.Count() - 2; j++)
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
                    if(cs == 0)
                    {
                        //ログ
                        LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + buzai + "]" + sla.name + "(配置Id=" + sla.id.ToString() + ")構成する辺が交差しているスラブ");
                        return ret;
                    }                    
                }



                //法線ベクトル
                XYZ v1 = (Point0[1] - Point0[0]).Normalize();
                XYZ v2 = new XYZ();                
                v2 = (Point0[Point0.Count() - 1] - Point0[0]).Normalize();
                XYZ normal = (v2.CrossProduct(v1)).Normalize();
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
                XYZ cross = (normal.CrossProduct(new XYZ(0, 0, 1))).Normalize();

                //同一平面上にすべての節点があるか⇒同一平面上でない場合は変換対象外
                if (!Commons.CalcPlane(normal, Point0))
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[RCスラブ]" + sla.name + "(配置Id=" + sla.id.ToString() + ")節点が同一面上に無いスラブ");
                    return ret;
                }

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
                    if(j >= Point0.Count()) { j = 0; }
                    XYZ Pi = Point0[i];
                    XYZ Pj = Point0[j];

                    if(Pi.X == Pj.X && Pi.Y == Pj.Y && Pi.Z == Pj.Z)
                    { continue; }

                    //配置レベルの取得
                    Level newlv = null;
                    int index = stbfloorid[i];
                    do
                    {
                        newlv = SearchLevel(stb, index);
                        index--;
                        if(index < 0) { break; }
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
                   
                    if(newlv == null)
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


                FamilyStructure.Slab Rsla = SetFamily.Slab;
                SetParameter(instance.LookupParameter(Rsla.MemId), sla.id);
                SetParameter(instance.LookupParameter(Rsla.NameMembers), sla.name);
                SetParameter(instance.LookupParameter(Rsla.thickness_ex_upper), sla.thickness_ex_upper);
                SetParameter(instance.LookupParameter(Rsla.thickness_ex_bottom), sla.thickness_ex_bottom);
                SetParameter(instance.LookupParameter(Rsla.dir_load), sla.dir_load);
                SetParameter(instance.LookupParameter(Rsla.angle_load), sla.angle_load);
                SetParameter(instance.LookupParameter(Rsla.isFoundation), sla.isFoundation);
                SetParameter(instance.LookupParameter(Rsla.type_haunch), sla.type_haunch);
                SetParameter(instance.LookupParameter(Rsla.kind_structure), sla.kind_structure);
                SetParameter(instance.LookupParameter(Rsla.kind_slab), sla.kind_slab);


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

                if (sla.StbOpens != null)
                {
                    //開口を作る前に、一度Regenerateしないとエラーが出る
                    pform.TopMost = false;
                    Commons.doc.Regenerate();
                    pform.TopMost = true;
                    errmsg = "開口";
                    if (!Slab_Open(stb, sla, Point0[0], v1, normal, pform, instance, keisyaflg)) { ret = false; }
                }

                //変換情報ログの出力
                var nodeIds = sla.StbNodeid_List.Select( x => x.id ).ToArray() ;
                MakeNodeLog("スラブの生成：", "[配置Id " + sla.id.ToString() + "]" + symbol.Name, sla.StbNodeid_List, 0, instance.Id);
                OutputDebubCommentLog( instance, sla.id, "スラブ", symbol.Name, nodeIds ) ;
            }
            catch (Exception)
            {
                ret = false;
            }
           
            return ret;
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


        /// <summary> 開口の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="sla"></param>
        /// <param name="Ps"></param>
        /// <param name="Vx">X方向単位ベクトル</param>
        /// <param name="N">法線ベクトル</param>
        /// <param name="pform"></param>
        /// <param name="instance"></param>
        /// <param name="keisyaflg"></param>
        /// <returns></returns>
        private bool Slab_Open(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbSlab sla, XYZ Ps,  XYZ Vx, XYZ N, ProgressBarForm pform, Floor instance, bool keisyaflg)
        {
            bool ret = true;

            try
            {
                for (int i = 0; i < sla.StbOpens.Count(); i++)
                {
                    XYZ Vy = -N.CrossProduct(Vx).Normalize(); 
                                     
                    XYZ Vz = -N;

                    XYZ Pb = Ps + Commons.mm2ft(sla.StbOpens[i].position_X) * Vx + Commons.mm2ft(sla.StbOpens[i].position_Y) * Vy;

                    //回転
                    Commons.AxisRotate(Vx, new XYZ(0, 0, 0), Vz, sla.StbOpens[i].rotate, ref Vx);
                    Commons.AxisRotate(Vy, new XYZ(0, 0, 0), Vz, sla.StbOpens[i].rotate, ref Vy);

                    CurveArray profile = new CurveArray();
                    XYZ Pn1 = Pb + Vx * Commons.mm2ft(sla.StbOpens[i].length_X);
                    profile.Append(Line.CreateBound(Pb, Pn1));
                    XYZ Pn2 = Pn1 + Vy * Commons.mm2ft(sla.StbOpens[i].length_Y);
                    profile.Append(Line.CreateBound(Pn1, Pn2));
                    XYZ Pn3 = Pn2 - Vx * Commons.mm2ft(sla.StbOpens[i].length_X);
                    profile.Append(Line.CreateBound(Pn2, Pn3));
                    profile.Append(Line.CreateBound(Pn3, Pb));
                    Commons.doc.Create.NewOpening(instance, profile, keisyaflg);

                   
                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "スラブ開口の生成：\t[配置Id" + sla.StbOpens[i].id.ToString() + "]" + sla.StbOpens[i].name);
                    
                    OutputDebubCommentLog( instance, sla.StbOpens[i].id, "スラブ開口", sla.StbOpens[i].name, new int[]{} ) ;
                    
                }
            }
            catch(Exception)
            {
                LogData.AddLog(LogData.LogKind.Error, 0, "[スラブ開口]" + sla.name + "(断面id=" + sla.id.ToString() + ")");
            }
            

            return ret;
        }
        #endregion
        #region 壁
        /// <summary> 壁の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateWall(STBclass stb, ProgressBarForm pform, string buzai, ref string errmsg)
        {
            bool ret = true;

          
            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            ProgressBar_Show(pform, "壁の生成");

           
            List<int> ids = new List<int>();
            List<string> typenames = new List<string>();
            List<WallType> symbols = new List<WallType>();

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();



            if (elements == null || elements.Count() == 0)
            {
                ret = false;
                LogData.AddLog(LogData.LogKind.Warning, 2100, "構造壁");
                return ret;
            }
            else
            {
                foreach (Element el in elements)
                {
                    WallType symbol = el as WallType;
                    if (symbol == null) { continue; }
                    Parameter p = symbol.LookupParameter("断面id");
                    if (p == null)
                    {
                        GaugePercent("壁パラメータの追加", (int)((double)1 / (double)1 * 100));
                        ParaSet.SetPara_Wall("壁", el, SetFamily.Wall);
                    }
                    symbols.Add(symbol);
                }
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
                        if (stb.StbModel.StbSections.StbSecWalls_RC != null)
                        {
                            int numCount = stb.StbModel.StbSections.StbSecWalls_RC.Count();
                            for (int i = 0; i < numCount; i++)
                            {
                                GaugePercent("RC壁の生成", (int)((double)i / (double)numCount * 100));

                                STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC wal = stb.StbModel.StbSections.StbSecWalls_RC[i];
                                
                                string typename = wal.name;
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
                                            typename = ReName(oldname, ascii);
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

                                if (!CreateRCWall(stb, wal, pform, symbol)) { ret = false; errmsg = buzai; }
                                ids.Add(wal.id);
                                typenames.Add(typename);
                            }
                        }
                        break;
                    case "RCパラペット":
                        if (stb.StbModel.StbSections.StbSecParapets_RC != null)
                        {
                            int numCount = stb.StbModel.StbSections.StbSecParapets_RC.Count();
                            for (int i = 0; i < numCount; i++)
                            {
                                GaugePercent("RCパラペットの生成", (int)((double)i / (double)numCount * 100));

                                STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC wal = stb.StbModel.StbSections.StbSecParapets_RC[i];

                                string typename = wal.name;

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
                                        //if (symbols[j].Name == typename)
                                        {
                                            typename = ReName(oldname, ascii);
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
                                if (!CreateParapet(stb, wal, pform, symbol)) { ret = false; errmsg = buzai; }

                                ids.Add(wal.id);
                                typenames.Add(typename);
                            }
                        }
                        break;

                }
                pform.TopMost = false;
                Commons.doc.Regenerate();
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                ret = false;
                errmsg = buzai;
            }

            tran.SetName("壁インスタンスパラメータの設定");
            try
            {
                tran.Start();

                if (stb.StbModel.StbMembers.StbWalls != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbWalls.Count();
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbWall wal = stb.StbModel.StbMembers.StbWalls[i];
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

                        GaugePercent("壁の生成", (int)((double)i / (double)numCount * 100));
                        if (!CreateWall_instance(stb, wal, pform, symbol, ref errmsg)) { ret = false; errmsg = buzai; }
                    }
                }
                if (stb.StbModel.StbMembers.StbParapets != null)
                {
                    int numCount = stb.StbModel.StbMembers.StbParapets.Count();
                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbParapet wal = stb.StbModel.StbMembers.StbParapets[i];
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

                        GaugePercent("パラペットの生成", (int)((double)i / (double)numCount * 100));
                        if (!CreateParapet_instance(stb, wal, pform, symbol, ref errmsg)) { ret = false; errmsg = buzai; }
                    }
                }
                pform.TopMost = false;
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
                ret = false;
                errmsg = buzai;
            }



            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }
            return ret;
        }
        /// <summary> RC壁タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool CreateRCWall(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC wal, ProgressBarForm pform, WallType symbol)
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
                    val = stb.StbCommon.concrete_strength;
                }
                SetMaterial(ref val, ref eid);
                double depth = 0;
                depth = wal.depth;

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

                SetParameter(symbol.LookupParameter(Rwal.SecId), wal.id);
                SetParameter(symbol.LookupParameter(Rwal.name), wal.name);
                SetParameter(symbol.LookupParameter(Rwal.depth_cover_outside), wal.depth_cover_outside);
                SetParameter(symbol.LookupParameter(Rwal.depth_cover_inside), wal.depth_cover_inside);

                //配筋
                if (wal.StbSecBar_Arrangement != null)
                {
                    string strength = "";
                    switch (wal.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:                       
                            if (wal.StbSecBar_Arrangement.StbSecSingle != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "シングル配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecSingle_Class bar = wal.StbSecBar_Arrangement.StbSecSingle[i];
                                    if(bar == null) { continue; }
                                    string[] D = new string[2];
                                    int keiflg = Get_D(bar.D, ref D);
                                    //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                    if (keiflg != 0)
                                    {
                                        string pos = WallTekkinkei_PosName(1, i);
                                        Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                                    }
                                    else
                                    {
                                        SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                        SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    }
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (wal.StbSecBar_Arrangement.StbSecZigzag != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "千鳥配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecZigzag_Class bar = wal.StbSecBar_Arrangement.StbSecZigzag[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    int keiflg = Get_D(bar.D, ref D);
                                    //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                    if (keiflg != 0)
                                    {
                                        string pos = WallTekkinkei_PosName(1, i);
                                        Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                                    }
                                    else
                                    {
                                        SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                        SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    }
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                        case 3:
                            if (wal.StbSecBar_Arrangement.StbSecDouble_Net != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecDouble_Net_Class bar = wal.StbSecBar_Arrangement.StbSecDouble_Net[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    int keiflg = Get_D(bar.D, ref D);
                                    //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                    if (keiflg != 0)
                                    {
                                        string pos = WallTekkinkei_PosName(1, i);
                                        Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                                    }
                                    else
                                    {
                                        SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                        SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    }
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                        case 4:
                            if (wal.StbSecBar_Arrangement.StbSecInside_And_Outside != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋（内外異なる）");
                                for (int i = 0; i < wal.StbSecBar_Arrangement.StbSecInside_And_Outside.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecInside_And_Outside_Class bar = wal.StbSecBar_Arrangement.StbSecInside_And_Outside[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    int keiflg = Get_D(bar.D, ref D);
                                    //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                    if (keiflg != 0)
                                    {
                                        string pos = WallTekkinkei_PosName(2, i);
                                        Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                                    }
                                    else
                                    {
                                        SetParameter(symbol.LookupParameter(Rwal.D_inout[i]), D[0]);
                                        SetParameter(symbol.LookupParameter(Rwal.D2_inout[i]), D[1]);
                                    }
                                    SetParameter(symbol.LookupParameter(Rwal.pitch_inout[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                    }
                    //鉄筋種別の方がTextとMaterialとあるため、Textの方へ値を入れる
                    IList<Parameter> paras = symbol.GetParameters(Rwal.strength);
                    for (int i = 0; i < paras.Count(); i++)
                    {
                        if (paras[i].StorageType != StorageType.String) { continue; }
                        SetParameter(paras[i], strength);
                    }
                    //端部補強筋
                    if (wal.StbSecBar_Arrangement.StbSecWallEdge != null)
                    {
                        for (int i = 0; i < wal.StbSecBar_Arrangement.StbSecWallEdge.Count(); i++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecWallEdge_Class bar = wal.StbSecBar_Arrangement.StbSecWallEdge[i];
                            if (bar == null) { continue; }
                            string[] D = new string[2];
                            int keiflg = Get_D(bar.D, ref D);
                            //2017/05/12 鉄筋径の書式が違うor空欄のとき
                            if (keiflg != 0)
                            {
                                string pos = WallTekkinkei_PosName(3, i);
                                Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                            }
                            else
                            {
                                SetParameter(symbol.LookupParameter(Rwal.D_Edge[i]), D[0]); 
                            }
                            
                            SetParameter(symbol.LookupParameter(Rwal.count_Edge[i]), bar.count);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Compare_strength(strength, bar.strength);
                            }
                        }
                    }                  
                    //開口配筋
                    if(wal.StbSecBar_Arrangement.StbSecOpen_Wall != null)
                    {
                        for(int i =0; i < wal.StbSecBar_Arrangement.StbSecOpen_Wall.Count(); i++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecWall_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class bar = wal.StbSecBar_Arrangement.StbSecOpen_Wall[i];
                            if (bar == null) { continue; }
                            SetParameter(symbol.LookupParameter(Rwal.D_op[i]), bar.D);
                            SetParameter(symbol.LookupParameter(Rwal.count_op[i]), bar.count);
                            SetParameter(symbol.LookupParameter(Rwal.length_op[i]), bar.length);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Compare_strength(strength, bar.strength);
                            }
                           
                        }
                    }
                    if(stb.StbModel.StbSections.StbSecOpens_RC != null)
                    {
                        List<int> ind_open = new List<int>();
                        for(int i = 0; i < stb.StbModel.StbMembers.StbWalls.Count(); i++)
                        {
                            if(stb.StbModel.StbMembers.StbWalls[i].id_section == wal.id)
                            {
                                if(stb.StbModel.StbMembers.StbWalls[i].StbOpens != null)
                                {
                                    for(int j = 0; j < stb.StbModel.StbMembers.StbWalls[i].StbOpens.Count(); j++)
                                    {
                                        ind_open.Add(stb.StbModel.StbMembers.StbWalls[i].StbOpens[j].id_section);
                                    }
                                    break;
                                }
                            }                            
                        }
                        bool copyflg = false;
                        for (int i = 0; i < ind_open.Count(); i++)
                        {
                            for(int j = 0; j < stb.StbModel.StbSections.StbSecOpens_RC.Count(); j++)
                            {
                                if(ind_open[i] == stb.StbModel.StbSections.StbSecOpens_RC[j].id)
                                {
                                    if(!copyflg)
                                    { copyflg = true; }
                                    else
                                    {
                                        string newtypename = symbol.Name + "_" + i.ToString();
                                        symbol = (WallType)symbol.Duplicate(newtypename);
                                    }
                                    STBclass.StbModelClass.StbSectionsClass.StbSecOpen_RC open_rc = stb.StbModel.StbSections.StbSecOpens_RC[j];
                                    if (open_rc == null) { continue; }
                                    if(open_rc.StbSecBar_Arrangement.StbSecBar_ArrangementType != 2) { continue; }
                                    
                                    for (int k = 0; k < open_rc.StbSecBar_Arrangement.StbSecOpen_Wall.Count(); k++)
                                    {
                                        STBclass.StbModelClass.StbSectionsClass.StbSecOpen_RC.StbSecBar_ArrangementClass.StbSecOpen_Wall_Class bar = open_rc.StbSecBar_Arrangement.StbSecOpen_Wall[k];
                                        if(bar == null) { continue; }
                                        string[] D = new string[2];
                                        int keiflg = Get_D(bar.D, ref D);
                                        //2017/05/12 鉄筋径の書式が違うor空欄のとき
                                        if (keiflg != 0)
                                        {
                                            string pos = WallTekkinkei_PosName(1, i);
                                            Make_TekkinkeiLog(symbol.Name, wal.id, pos, "RC壁", keiflg);
                                        }
                                        else
                                        {
                                            SetParameter(symbol.LookupParameter(Rwal.D_op[k]), D[0]);
                                        }
                                        SetParameter(symbol.LookupParameter(Rwal.count_op[k]), bar.count);
                                        SetParameter(symbol.LookupParameter(Rwal.length_op[k]), bar.length);
                                        if (strength == "")
                                        {
                                            strength = bar.strength;
                                        }
                                        else
                                        {
                                            strength = Compare_strength(strength, bar.strength);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Parameter_Select_Set(Rwal.strength, strength, wall: symbol);
                }
                else //2017/05/19 鉄筋タグが無い→ログ出力
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RC壁]" + symbol.Name + "(断面id=" + wal.id.ToString() + ")");
                }
            }
            catch (Exception)
            { ret = false; }

            return ret;
        }
        /// <summary> RCパラペットタイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool CreateParapet(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC wal, ProgressBarForm pform, WallType symbol)
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
                    val = stb.StbCommon.concrete_strength;
                }
                SetMaterial(ref val, ref eid);
                double depth = 0;
                depth = wal.depth_T;

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

                SetParameter(symbol.LookupParameter(Rwal.SecId), wal.id);
                SetParameter(symbol.LookupParameter(Rwal.name), wal.name);
                SetParameter(symbol.LookupParameter(Rwal.depth_cover_outside), wal.depth_cover_outside);
                SetParameter(symbol.LookupParameter(Rwal.depth_cover_inside), wal.depth_cover_inside);
                SetParameter(symbol.LookupParameter(Rwal.kind_form), wal.kind_form);
                SetParameter(symbol.LookupParameter(Rwal.isTip_line), wal.isTip_line);
                SetParameter(symbol.LookupParameter(Rwal.depth_T), wal.depth_T, true);
                SetParameter(symbol.LookupParameter(Rwal.depth_H), wal.depth_H,true);
                SetParameter(symbol.LookupParameter(Rwal.depth_T1), wal.depth_T1,true);
                SetParameter(symbol.LookupParameter(Rwal.depth_H1), wal.depth_H1,true);
                SetParameter(symbol.LookupParameter(Rwal.depth_H2), wal.depth_H2,true);
                SetParameter(symbol.LookupParameter(Rwal.depth_H3), wal.depth_H3,true);

                //配筋
                if (wal.StbSecBar_Arrangement != null)
                {
                    string strength = "";
                    switch (wal.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                    {
                        case 1:
                            if (wal.StbSecBar_Arrangement.StbSecSingle != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "シングル配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC.StbSecBar_ArrangementClass.StbSecSingle_Class bar = wal.StbSecBar_Arrangement.StbSecSingle[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    Get_D(bar.D, ref D);
                                    SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                    SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (wal.StbSecBar_Arrangement.StbSecZigzag != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "千鳥配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC.StbSecBar_ArrangementClass.StbSecZigzag_Class bar = wal.StbSecBar_Arrangement.StbSecZigzag[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    Get_D(bar.D, ref D);
                                    SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                    SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;
                        case 3:
                            if (wal.StbSecBar_Arrangement.StbSecDouble_Net != null)
                            {
                                SetParameter(symbol.LookupParameter(Rwal.ArrengementType), "ダブル配筋");
                                for (int i = 0; i < 2; i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC.StbSecBar_ArrangementClass.StbSecDouble_Net_Class bar = wal.StbSecBar_Arrangement.StbSecDouble_Net[i];
                                    if (bar == null) { continue; }
                                    string[] D = new string[2];
                                    Get_D(bar.D, ref D);
                                    SetParameter(symbol.LookupParameter(Rwal.D[i]), D[0]);
                                    SetParameter(symbol.LookupParameter(Rwal.D2[i]), D[1]);
                                    SetParameter(symbol.LookupParameter(Rwal.pitch[i]), bar.pitch, true);
                                    if (strength == "")
                                    {
                                        strength = bar.strength;
                                    }
                                    else
                                    {
                                        strength = Compare_strength(strength, bar.strength);
                                    }
                                }
                            }
                            break;                           
                    }                    

                    //先端補強筋（アゴ筋）
                    if (wal.StbSecBar_Arrangement.StbSecParapetTip != null)
                    {
                        for (int i = 0; i < wal.StbSecBar_Arrangement.StbSecParapetTip.Count(); i++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC.StbSecBar_ArrangementClass.StbSecParapetTip_Class bar = wal.StbSecBar_Arrangement.StbSecParapetTip[i];
                            if (bar == null) { continue; }
                            SetParameter(symbol.LookupParameter(Rwal.D_Tip[i]), bar.D);
                            SetParameter(symbol.LookupParameter(Rwal.pitch_Tip[i]), bar.pitch, true);
                            SetParameter(symbol.LookupParameter(Rwal.count_Tip[i]), bar.count);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Compare_strength(strength, bar.strength);
                            }
                        }
                    }

                    //端部補強筋
                    if (wal.StbSecBar_Arrangement.StbSecParapetEdge != null)
                    {
                        for (int i = 0; i < wal.StbSecBar_Arrangement.StbSecParapetEdge.Count(); i++)
                        {
                            STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC.StbSecBar_ArrangementClass.StbSecParapetEdge_Class bar = wal.StbSecBar_Arrangement.StbSecParapetEdge[i];
                            if (bar == null) { continue; }
                            SetParameter(symbol.LookupParameter(Rwal.D_Edge_Para[i]), bar.D);
                            SetParameter(symbol.LookupParameter(Rwal.count_Edge_Para[i]), bar.count);
                            if (strength == "")
                            {
                                strength = bar.strength;
                            }
                            else
                            {
                                strength = Compare_strength(strength, bar.strength);
                            }
                        }
                    }

                    Parameter_Select_Set(Rwal.strength, strength, wall: symbol);
                }
                else //2017/05/19 鉄筋タグが無い→ログ出力
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2400, "[RCパラペット]" + symbol.Name + "(断面id=" + wal.id.ToString() + ")");
                }
            }
            catch (Exception)
            { ret = false; }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="walltype">1：シングル・ダブル・千鳥・開口 2：ダブル（内外異なる） 3：端部補強</param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string WallTekkinkei_PosName(int walltype, int i)
        {
            string pos = "";

            switch(walltype)
            {
                case 1:
                    switch(i)
                    {
                        case 0:
                            pos = "VERTICAL";
                            break;
                        case 1:
                            pos = "HORIZONTAL";
                            break;
                        case 2:
                            pos = "DIAGONAL";
                            break;
                    }
                    break;
                case 2:
                    switch(i)
                    {
                        case 0:
                            pos = "VERTICAL_OUTSIDE(TOP_START)";
                            break;
                        case 1:
                            pos = "VERTICAL_OUTSIDE(MIDDLE)";
                            break;
                        case 2:
                            pos = "VERTICAL_OUTSIDE(BOTTOM_END)";
                            break;
                        case 3:
                            pos = "VERTICAL_INSIDE(TOP_START)";
                            break;
                        case 4:
                            pos = "VERTICAL_INSIDE(MIDDLE)";
                            break;
                        case 5:
                            pos = "VERTICAL_INSIDE(BOTTOM_END)";
                            break;
                        case 6:
                            pos = "HORIZONTAL_OUTSIDE(TOP_START)";
                            break;
                        case 7:
                            pos = "HORIZONTAL_OUTSIDE(MIDDLE)";
                            break;
                        case 8:
                            pos = "HORIZONTAL_OUTSIDE(BOTTOM_END)";
                            break;
                        case 9:
                            pos = "HORIZONTAL_INSIDE(TOP_START)";
                            break;
                        case 10:
                            pos = "HORIZONTAL_INSIDE(MIDDLE)";
                            break;
                        case 11:
                            pos = "HORIZONTAL_INSIDE(BOTTOM_END)";
                            break;
                    }
                    break;
                case 3:
                    switch(i)
                    {
                        case 0:
                            pos = "VERTICAL_START";
                            break;
                        case 1:
                            pos = "VERTICAL_END";
                            break;
                        case 2:
                            pos = "HORIZONTAL_BOTTOM";
                            break;
                        case 3:
                            pos = "HORIZONTAL_TOP";
                            break;
                    }
                    break;
            }

            return pos;
        }
        /// <summary> 壁インスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <param name="isFoundaion"></param>
        /// <returns></returns>
        private bool CreateWall_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbWall wal, ProgressBarForm pform, WallType symbol, ref string errmsg)
        {
            bool ret = true;
            FamilyStructure.Wall Rwal = SetFamily.Wall;
            IList<Curve> profile = new List<Curve>();

            try
            {
                //同じ節点idが含まれているときがある→同じものは消去
                for (int i = 0; i < wal.StbNodeid_List.Count(); i++)
                {
                    for (int j = i + 1; j < wal.StbNodeid_List.Count(); j++)
                    {
                        if (wal.StbNodeid_List[i].id == wal.StbNodeid_List[j].id)
                        {
                            wal.StbNodeid_List.RemoveAt(j);
                            j--;
                        }
                    }
                }

                //節点の数が3未満⇒変換対象外
                if (wal.StbNodeid_List.Count < 3)
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id="　+  wal.id.ToString() + ")節点数が3未満の壁" );
                    return ret;
                }

                //節点から配置位置を取得
                List<XYZ> Point0 = new List<XYZ>();
                List<XYZ> Point1 = new List<XYZ>();
                List<int> stbfloorid = new List<int>();
                for (int i = 0; i < wal.StbNodeid_List.Count(); i++)
                {
                    XYZ Pa = null;
                    Pa = Get_Node_Position(stb, wal.StbNodeid_List[i].id, 0, 0, 0);

                    if (Point0.Count() > 0)
                    {
                        if (Pa.DistanceTo(Point0[Point0.Count() - 1]) < gosa)
                        {
                            continue;
                        }
                    }
                    Point0.Add(Pa);
                    Point1.Add(Pa);
                    int flid = Get_stbFloor_index(stb, wal.StbNodeid_List[i].id, false);
                    stbfloorid.Add(flid);
                }
               
                //法線ベクトル
                XYZ v1 = (Point0[1] - Point0[0]).Normalize();
                XYZ v2 = (Point0[Point0.Count() - 1] - Point0[0]).Normalize();
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
                if (normal.GetLength() < 0.01)
                {
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")法線ベクトルが計算できないため変換できません。");
                    return ret;
                }

                var wh = Point0.Max(a => a.Z) - Point0.Min(a => a.Z);
                if (wh <= Commons.mm2ft(1))
                {
                    LogData.AddLog(LogData.LogKind.Warning, 0, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")高さが1mm以下のため変換できません。");
                    return ret;
                }


                //同一平面上にすべての節点があるか⇒同一平面上でない場合は変換対象外
                if (!Commons.CalcPlane(normal, Point0))
                {
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")節点が同一面上に無い壁");
                    return ret;
                }


                //傾斜フラグ
                if (Math.Abs(normal.Z) > gosa)
                {
                    //ログ（傾斜壁は生成しない）
                    LogData.AddLog(LogData.LogKind.Warning, 2200, "[RC壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")傾斜壁");
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
                        for(int j = 1; j < Point0.Count() - 2; j++)
                        {
                            double Pj_X = Commons.Get_Point_Vec(v1, Point0[j]);
                            double Pj_Y = Commons.Get_Point_Vec(v2, Point0[j]);
                            double Pj1_X = Commons.Get_Point_Vec(v1, Point0[j + 1]);
                            double Pj1_Y = Commons.Get_Point_Vec(v2, Point0[j + 1]);
                            cs = Commons.CalcCross(Pi_X, Pi_Y, P0_X, P0_Y, Pj_X, Pj_Y, Pj1_X, Pj1_Y, out Xx, out Yy);
                            if(Xx == Pj_X && Yy == Pj_Y) { continue; }
                            if(cs == 0) { break; }
                        }
                    }
                    else
                    {
                        for (int j = i + 2; j < Point0.Count() - 1; j++)
                        {
                            if(j == i || j == i + 1) { continue; }
                            double Pj_X = Commons.Get_Point_Vec(v1, Point0[j]);
                            double Pj_Y = Commons.Get_Point_Vec(v2, Point0[j]);
                            double Pj1_X = Commons.Get_Point_Vec(v1, Point0[j + 1]);
                            double Pj1_Y = Commons.Get_Point_Vec(v2, Point0[j + 1]);
                            double Pi1_X = Commons.Get_Point_Vec(v1, Point0[i + 1]);
                            double Pi1_Y = Commons.Get_Point_Vec(v2, Point0[i + 1]);
                            cs = Commons.CalcCross(Pi_X, Pi_Y, Pi1_X, Pi1_Y, Pj_X, Pj_Y, Pj1_X, Pj1_Y, out Xx, out Yy);
                            if(Xx == Pj_X && Yy == Pj_Y) { continue; }
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
                for (int i = 0; i < Point0.Count(); i++)
                {
                    int j = i + 1;
                    if (j >= Point0.Count()) { j = 0; }
                    double offset = Commons.mm2ft(wal.offset);

                    XYZ Pi = Point0[i] + normal * offset;
                    XYZ Pj = Point0[j] + normal * offset;

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
                            newlv = SearchLevel_height(stb, wal.StbNodeid_List[i].id, wal.StbNodeid_List[i].id);
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

                Wall instance = Wall.Create(Commons.doc, profile, symbol.Id, btmLevel.Id, true, normal);

                var pz = Point0.Select(a => a.Z).Distinct().OrderBy(a => a).ToList();
                if (pz.Count <= 2)
                {
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), topLevel.Id);
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET), offset_t);
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), offset_b);
                }
                else
                {
                    //台形形状のときは高さ指定にする。（形状がプロファイルの座標で作れない）
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), ElementId.InvalidElementId);
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM), pz.Max() - pz.Min());
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), offset_b);
                }

                SetParameter(instance.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_USAGE_PARAM), StructuralInstanceUsage.Wall);              

                SetParameter(instance.LookupParameter(Rwal.MemId), wal.id);
                SetParameter(instance.LookupParameter(Rwal.NameMembers), wal.name);
                SetParameter(instance.LookupParameter(Rwal.kind_structure), wal.kind_structure);
                SetParameter(instance.LookupParameter(Rwal.kind_layout), wal.kind_layout);
                SetParameter(instance.LookupParameter(Rwal.thickness_ex_right), wal.thickness_ex_right);
                SetParameter(instance.LookupParameter(Rwal.thickness_ex_left), wal.thickness_ex_left);
                SetParameter(instance.LookupParameter(Rwal.kind_wall), wal.kind_wall);
                SetParameter(instance.LookupParameter(Rwal.slit_upper), wal.slit_upper);
                SetParameter(instance.LookupParameter(Rwal.slit_bottom), wal.slit_bottom);
                SetParameter(instance.LookupParameter(Rwal.slit_left), wal.slit_left);
                SetParameter(instance.LookupParameter(Rwal.slit_right), wal.slit_right);
                SetParameter(instance.LookupParameter(Rwal.type_outside), wal.type_outside);
                SetParameter(instance.LookupParameter(Rwal.isPress), wal.isPress);


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
                if (wal.StbOpens != null)
                {
                    //開口を作る前に、一度Regenerateしないとエラーが出る
                    pform.TopMost = false;
                    Commons.doc.Regenerate();
                    pform.TopMost = true;
                    errmsg = "開口";
                    if (!Wall_Open(stb, wal, Point0[0], v1, normal, pform, instance)) { ret = false; }
                }

                //変換情報ログの出力
                var nodeIds = wal.StbNodeid_List.Select( x => x.id ).ToArray() ;
                MakeNodeLog( "壁の生成：", "[配置Id " + wal.id.ToString() + "]" + symbol.Name, wal.StbNodeid_List, 0, instance.Id) ;
                OutputDebubCommentLog( instance, wal.id, "壁", symbol.Name, nodeIds ) ;
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
        /// <summary> パラペットインスタンスパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="pform"></param>
        /// <param name="symbol"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateParapet_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbParapet wal, ProgressBarForm pform, WallType symbol, ref string errmsg)
        {
            bool ret = true;

            FamilyStructure.Wall Rwal = SetFamily.Wall;
            IList<Curve> profile = new List<Curve>();

            try
            {
                double H = 0;
                for (int i = 0; i < stb.StbModel.StbSections.StbSecParapets_RC.Count(); i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecParapet_RC para = stb.StbModel.StbSections.StbSecParapets_RC[i];
                    if (wal.id_section == para.id)
                    {
                        H = Commons.mm2ft(para.depth_H);
                        break;
                    }
                }


                XYZ P1 = Get_Node_Position(stb, wal.idNode_start, 0, 0, 0);
                XYZ P2 = Get_Node_Position(stb, wal.idNode_end, 0, 0, 0);

                XYZ v1 = (P2 - P1).Normalize();
                XYZ v2 = XYZ.BasisZ;
                XYZ normal = (v2.CrossProduct(v1)).Normalize();

                //配置レベルの取得
                int floorind = Get_stbFloor_index(stb, wal.idNode_start);
                Level newlv = SearchLevel(stb, floorind);
                int floorinde = Get_stbFloor_index(stb, wal.idNode_end);
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

                P1 += normal * Commons.mm2ft(wal.offset);
                P2 += normal * Commons.mm2ft(wal.offset);

                profile.Add(Line.CreateBound(P1, P2));
                XYZ P3 = P2 + H * BasisZ;
                profile.Add(Line.CreateBound(P2, P3));
                XYZ P4 = P1 + H * BasisZ;
                profile.Add(Line.CreateBound(P3, P4));
                profile.Add(Line.CreateBound(P4, P1));

                Wall instance = Wall.Create(Commons.doc, profile, symbol.Id, btmLevel.Id, true, normal);
                if (instance != null)
                {
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE), ElementId.InvalidElementId);
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM), H);
                    SetParameter(instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET), Math.Min(P1.Z, P2.Z) - btmLevel.Elevation);
                }
            }
            catch (Exception)
            { ret = false; }

            return ret;
        }
        /// <summary> 開口の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="wal"></param>
        /// <param name="Ps"></param>
        /// <param name="Vx">X方向単位ベクトル</param>
        /// <param name="N">法線ベクトル</param>
        /// <param name="pform"></param>
        /// <param name="instance"></param>
        /// <param name="keisyaflg"></param>
        /// <returns></returns>
        private bool Wall_Open(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbWall wal, XYZ Ps, XYZ Vx, XYZ N, ProgressBarForm pform, Wall instance)
        {
            bool ret = true;

            try
            {
                for (int i = 0; i < wal.StbOpens.Count(); i++)
                {
                    if(wal.StbOpens[i].rotate != 0)
                    {
                        //ログ出力
                        LogData.AddLog(LogData.LogKind.Warning, 2200, "[" + wal.kind_structure + "壁]" + wal.name + "(配置Id=" + wal.id.ToString() + ")壁開口の回転");
                    }

                    XYZ Vy = -N.CrossProduct(Vx).Normalize();

                    XYZ Pb = Ps + Commons.mm2ft(wal.StbOpens[i].position_X) * Vx + Commons.mm2ft(wal.StbOpens[i].position_Y) * Vy;   
                    XYZ Pn1 = Pb + Vx * Commons.mm2ft(wal.StbOpens[i].length_X);
                    XYZ Pn2 = Pn1 + Vy * Commons.mm2ft(wal.StbOpens[i].length_Y);
                    Commons.doc.Create.NewOpening(instance, Pb, Pn2);

                    LogData.AddLog(LogData.LogKind.Infmoation, 0, "壁開口の生成：\t[配置Id" + wal.StbOpens[i].id.ToString() + "]" + wal.StbOpens[i].name);
                    
                    OutputDebubCommentLog( instance, wal.StbOpens[i].id, "壁開口", wal.StbOpens[i].name, new int[]{} ) ;
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
        private bool CreateFoundation(STBclass stb, ProgressBarForm pform, string syubetu, ref string errmsg)
        {
            bool ret = true;

            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            ProgressBar_Show(pform, syubetu + "の生成");

            //変換ファミリ配列
            Family[][] ConvFamily = new Family[RevitLNK.BaseText.Length][];
            for (int i = 0; i < RevitLNK.BaseText.Length; i++)
            {
                Array.Resize(ref ConvFamily[i], RevitLNK.BaseText[i].Length);
            }

            //変換ファミリの取得
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }
            int numfamily = 0; //変換するファミリの数


            for (int i = 0; i < ConvFamily.Length; i++)
            {
                for (int j = 0; j < ConvFamily[i].Length; j++)
                {
                    if (!SetFamily.FoFName.flg[i][j]) { continue; }
                    if (!SetFamily.FoFName.convflg[i][j]) { continue; }

                    foreach (Element el in elements)
                    {
                        FamilySymbol familysymbol = el as FamilySymbol;
                        if (familysymbol == null) { continue; }
                        if (familysymbol.FamilyName == SetFamily.FoFName.FamilyName[i][j])
                        {
                            ConvFamily[i][j] = familysymbol.Family;
                            Parameter p = familysymbol.LookupParameter("断面id");
                            if (p == null)
                            {  
                                //プログレスバーの表示
                                GaugePercent("パラメータ追加", (int)((double)i / (double)ConvFamily.Count() * 100));

                                Autodesk.Revit.DB.Document doc = Commons.doc.EditFamily(ConvFamily[i][j]);
                                Autodesk.Revit.DB.Transaction tran1 = new Autodesk.Revit.DB.Transaction(doc, ConvFamily[i][j].Name+"パラメータ追加");
                                try
                                {
                                    tran1.Start();
                                   
                                    Autodesk.Revit.DB.FamilyManager fmg = doc.FamilyManager;
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
                                            ParaSet.SetPara_Castinpile(fmg, SetFamily.CastinPile);
                                            break;
                                    }
                                    //プロジェクトにパラメータを追加したファミリをロードする
                                    FamilyOption famop = new FamilyOption();
                                    ConvFamily[i][j] = doc.LoadFamily(Commons.doc, famop);
                                    pform.TopMost = false;
                                    tran1.Commit();
                                    pform.TopMost = true;
                                    doc.Close(false);

                                    SetConvertFamily(ref ConvFamily, SetFamily.FoFName.FamilyName, familysymbol.FamilyName, i, j);

                                }
                                catch (Exception)
                                {
                                    pform.TopMost = false;
                                    tran1.RollBack();
                                    pform.TopMost = true;
                                    doc.Close();
                                }
                            }
                            numfamily++;
                        }
                    }
                }
            }

            Transaction tran = new Transaction(Commons.doc, "基礎の生成");
            errmsg = "RC基礎";
            try
            {
                tran.Start();               

                if (stb.StbModel.StbSections.StbSecFoundations_RC != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecFoundations_RC.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC fou = stb.StbModel.StbSections.StbSecFoundations_RC[i];

                        //プログレスバーの表示
                        GaugePercent("RC基礎断面の生成", (int)((double)i / (double)numCount * 100));
                        if (!CreateFoundation_RC(stb, fou, pform, ConvFamily)) { ret = false; errmsg = "RC基礎断面"; }

                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();              
                tran.Commit();
                pform.TopMost = true;

            }
            catch(Exception)
            {
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }

            //変換ファミリ配列
           
             tran.SetName("杭の生成");
            errmsg = "杭";
            try
            {
                tran.Start();
               
                if (stb.StbModel.StbSections.StbSecPiles_RC != null)
                {
                    int numCount = stb.StbModel.StbSections.StbSecPiles_RC.Count();

                    for (int i = 0; i < numCount; i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecPile_RC pile = stb.StbModel.StbSections.StbSecPiles_RC[i];

                        //プログレスバーの表示
                        GaugePercent("RC杭断面の生成", (int)((double)i / (double)numCount * 100));
                       if(!CreatePile_RC(stb, pile, pform, ConvFamily)) { ret = false;  errmsg = "RC杭断面"; }

                    }
                }
                pform.TopMost = false;
                Commons.doc.Regenerate();                
                tran.Commit();
                pform.TopMost = true;

            }
            catch (Exception)
            {
                ret = false;
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }

            if (ret)
            {
                tran.SetName("基礎インスタンスパラメータの生成");
                try
                {
                    tran.Start();

                    if (stb.StbModel.StbMembers.StbFootings != null)
                    {
                        int numCount = stb.StbModel.StbMembers.StbFootings.Count();

                        for (int i = 0; i < numCount; i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbFooting foo = stb.StbModel.StbMembers.StbFootings[i];

                            //プログレスバーの表示
                            GaugePercent(syubetu + "の生成", (int)((double)i / (double)numCount * 100));
                            if (!CreateFoundation_instance(stb, foo, pform, ConvFamily)) { ret = false; errmsg = "フーチング情報"; }
                        }
                    }
                    if(stb.StbModel.StbMembers.StbStrip_Footings != null)
                    {
                        int numCount = stb.StbModel.StbMembers.StbStrip_Footings.Count();

                        for (int i = 0; i < numCount; i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbStrip_Footing foo = stb.StbModel.StbMembers.StbStrip_Footings[i];

                            //プログレスバーの表示
                            GaugePercent(syubetu + "の生成", (int)((double)i / (double)numCount * 100));
                            if (!CreateStripFooting_instance(stb, foo, pform, ConvFamily)) { ret = false; errmsg = "布基礎情報"; }
                        }
                    }
                    pform.TopMost = false;
                    Commons.doc.Regenerate();
                    pform.TopMost = true;

                    pform.TopMost = false;
                    tran.Commit();
                    pform.TopMost = true;
                }
                catch (Exception)
                {
                    ret = false;
                    pform.TopMost = false;
                    tran.RollBack();
                    pform.TopMost = true;
                }

                tran.SetName("杭インスタンスパラメータの生成");
                errmsg = "杭インスタンス";
                try
                {
                    tran.Start();

                    if(stb.StbModel.StbMembers.StbPiles != null)
                    {
                        int numCount = stb.StbModel.StbMembers.StbPiles.Count();
                        for(int i =0; i <numCount;i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbPile pile = stb.StbModel.StbMembers.StbPiles[i];

                            //プログレスバーの表示
                            GaugePercent(syubetu + "の生成", (int)((double)i / (double)numCount * 100));
                            if (!CreatePile_instance(stb, pile, pform, ConvFamily)) { ret = false;  errmsg = "杭インスタンス"; }
                        }
                    }

                    pform.TopMost = false;
                    Commons.doc.Regenerate();
                    pform.TopMost = true;

                   

                    pform.TopMost = false;
                    tran.Commit();
                    pform.TopMost = true;
                }
                catch(Exception)
                {
                    ret = false;
                    pform.TopMost = false;
                    tran.RollBack();
                    pform.TopMost = true;
                }

               
            }



            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();

            }

            if (ret == false)
            {
                errmsg = "基礎";
            }

            return ret;
        }
        /// <summary> RC基礎タイプパラメータ設定 
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="bra"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateFoundation_RC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC fo, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //タイプ名
            string typename = "";
            typename = fo.name;

            FamilySymbol symbol = null;

            switch (fo.StbSecFigure.StbSecFigureType)
            {
                case 1:
                    if (fo.StbSecFigure.StbSecRect != null)
                    {
                        if (ConvFamily[0][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "RC基礎矩形");
                        }
                        else
                        {
                            //タイプの生成                        
                            if (!SearchFamilySymbol(ConvFamily[0][0], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.Foundation_Rect Rfo = SetFamily.FRect;
                            SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                            SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                            SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                            SetParameter(symbol.LookupParameter(Rfo.DX), fo.StbSecFigure.StbSecRect.DX, true);
                            SetParameter(symbol.LookupParameter(Rfo.DY), fo.StbSecFigure.StbSecRect.DY, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth), fo.StbSecFigure.StbSecRect.depth, true);

                            //配筋
                            if (fo.StbSecBar_Arrangement.StbSecRect != null)
                            {
                                string strength = "";
                                for (int i = 0; i < fo.StbSecBar_Arrangement.StbSecRect.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass bar = fo.StbSecBar_Arrangement.StbSecRect[i];
                                    if (bar != null)
                                    {
                                        SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                                        SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.count);
                                        if (i == 0)
                                        { strength = bar.strength; }
                                        else
                                        { strength = Compare_strength(bar.strength, strength); }                                                                                    
                                    }
                                }

                                Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                            }
                        }
                    }
                    break;
                case 2:
                    if(fo.StbSecFigure.StbSecTapered_Rect != null)
                    {
                        if (ConvFamily[0][1] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "RC基礎矩形テーパー");
                        }
                        else
                        {
                            //タイプの生成                        
                            if (!SearchFamilySymbol(ConvFamily[0][1], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                            
                            FamilyStructure.Foundation_Tapered_Rect Rfo = SetFamily.FTRect;
                            SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                            SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                            SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_base), fo.StbSecFigure.StbSecTapered_Rect.depth_base, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_tip), fo.StbSecFigure.StbSecTapered_Rect.depth_tip, true);
                            SetParameter(symbol.LookupParameter(Rfo.DX), fo.StbSecFigure.StbSecTapered_Rect.DX, true);
                            SetParameter(symbol.LookupParameter(Rfo.DY), fo.StbSecFigure.StbSecTapered_Rect.DY, true);
                            SetParameter(symbol.LookupParameter(Rfo.t_DX), fo.StbSecFigure.StbSecTapered_Rect.DX / 2, true);
                            SetParameter(symbol.LookupParameter(Rfo.t_DY), fo.StbSecFigure.StbSecTapered_Rect.DY / 2, true);
                            
                            //配筋
                            if (fo.StbSecBar_Arrangement.StbSecRect != null)
                            {
                                string strength = "";
                                for (int i = 0; i < fo.StbSecBar_Arrangement.StbSecRect.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass bar = fo.StbSecBar_Arrangement.StbSecRect[i];
                                    if (bar != null)
                                    {
                                        SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                                        SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.count);
                                        if(strength == "")
                                        { strength = bar.strength; }
                                        else
                                        { strength = Compare_strength(strength, bar.strength); }
                                    }
                                }

                                Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                            }
                        }
                    }
                    break;
                case 3:
                    if (fo.StbSecFigure.StbSecTriangle != null)
                    {
                        if (ConvFamily[0][2] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎三角");
                        }
                        else
                        {
                            //タイプの生成                        
                            if (!SearchFamilySymbol(ConvFamily[0][2], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.Foundation_Triangle Rfo = SetFamily.FTri;
                            SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                            SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                            SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth), fo.StbSecFigure.StbSecTriangle.depth, true);
                            SetParameter(symbol.LookupParameter(Rfo.DX), fo.StbSecFigure.StbSecTriangle.DX, true);
                            SetParameter(symbol.LookupParameter(Rfo.DY), fo.StbSecFigure.StbSecTriangle.DY, true);

                            //配筋
                            if (fo.StbSecBar_Arrangement.StbSecTriangle != null)
                            {
                                string strength = "";
                                for (int i = 0; i < fo.StbSecBar_Arrangement.StbSecTriangle.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecTriangleClass bar = fo.StbSecBar_Arrangement.StbSecTriangle[i];
                                    if (bar != null)
                                    {
                                        SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                                        SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.count);
                                        if(strength == "")
                                        { strength = bar.strength; }
                                        else
                                        { strength = Compare_strength(bar.strength, strength); }
                                    }
                                }

                                Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                            }
                        }
                    }
                    break;
                case 4:
                    if (fo.StbSecFigure.StbSecEqiTriangle != null)
                    {
                        if (ConvFamily[0][3] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎正三角形");
                        }
                        else
                        {
                            //タイプの生成                        
                            if (!SearchFamilySymbol(ConvFamily[0][3], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.Foundation_Equi_Triangle Rfo = SetFamily.FETriangle;
                            SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                            SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                            SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth), fo.StbSecFigure.StbSecEqiTriangle.depth, true);
                            SetParameter(symbol.LookupParameter(Rfo.B), fo.StbSecFigure.StbSecEqiTriangle.B,true);
                            SetParameter(symbol.LookupParameter(Rfo.C), fo.StbSecFigure.StbSecEqiTriangle.C,true);

                            //配筋
                            if (fo.StbSecBar_Arrangement.StbSecThreeWay != null)
                            {
                                string strength = "";
                                for (int i = 0; i < fo.StbSecBar_Arrangement.StbSecThreeWay.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecThreeWayClass bar = fo.StbSecBar_Arrangement.StbSecThreeWay[i];
                                    if (bar != null)
                                    {
                                        SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                                        SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.count);
                                        if (strength == "")
                                        { strength = bar.strength; }
                                        else
                                        { strength = Compare_strength(bar.strength, strength); }
                                    }
                                }

                                Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                            }
                        }
                    }
                    break;
                case 5:
                    if (fo.StbSecFigure.StbSecOctagon != null)
                    {
                        if (ConvFamily[0][4] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎八角形");
                        }
                        else
                        {
                            //タイプの生成                        
                            if (!SearchFamilySymbol(ConvFamily[0][4], typename, ref symbol))
                            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                            FamilyStructure.Foundation_Octagon Rfo = SetFamily.FOct;
                            SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                            SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                            SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                            SetParameter(symbol.LookupParameter(Rfo.depth), fo.StbSecFigure.StbSecOctagon.depth, true);
                            SetParameter(symbol.LookupParameter(Rfo.DX), fo.StbSecFigure.StbSecOctagon.DX,true);
                            SetParameter(symbol.LookupParameter(Rfo.DY), fo.StbSecFigure.StbSecOctagon.DY,true);
                            SetParameter(symbol.LookupParameter(Rfo.CX1), fo.StbSecFigure.StbSecOctagon.CX1,true);
                            SetParameter(symbol.LookupParameter(Rfo.CY1), fo.StbSecFigure.StbSecOctagon.CY1,true);
                            SetParameter(symbol.LookupParameter(Rfo.CX2), fo.StbSecFigure.StbSecOctagon.CX2,true);
                            SetParameter(symbol.LookupParameter(Rfo.CY2), fo.StbSecFigure.StbSecOctagon.CY2,true);
                            SetParameter(symbol.LookupParameter(Rfo.CX3), fo.StbSecFigure.StbSecOctagon.CX3,true);
                            SetParameter(symbol.LookupParameter(Rfo.CY3), fo.StbSecFigure.StbSecOctagon.CY3,true);
                            SetParameter(symbol.LookupParameter(Rfo.CX4), fo.StbSecFigure.StbSecOctagon.CX4,true);
                            SetParameter(symbol.LookupParameter(Rfo.CY4), fo.StbSecFigure.StbSecOctagon.CY4,true);


                            //配筋
                            if (fo.StbSecBar_Arrangement.StbSecRect != null)
                            {
                                string strength = "";
                                for (int i = 0; i < fo.StbSecBar_Arrangement.StbSecRect.Count(); i++)
                                {
                                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC.StbSecBar_ArrangementClass.StbSecRectClass bar = fo.StbSecBar_Arrangement.StbSecRect[i];
                                    if (bar != null)
                                    {
                                        SetParameter(symbol.LookupParameter(Rfo.D[i]), bar.D);
                                        SetParameter(symbol.LookupParameter(Rfo.count[i]), bar.count); if (strength == "")
                                        { strength = bar.strength; }
                                        else
                                        { strength = Compare_strength(bar.strength, strength); }
                                    }
                                }

                                Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                            }
                        }
                    }
                    break;
                case 6:
                    if (fo.StbSecFigure.StbSecContinuous != null)
                    {
                        if (ConvFamily[1][0] == null)
                        {
                            //ログ表示（ファミリ未ロード)
                            LogData.AddLog(LogData.LogKind.Warning, 2100, "布基礎");
                        }
                        else
                        {                            
                            for (int i = 0; i < stb.StbModel.StbMembers.StbStrip_Footings.Count(); i++)
                            {
                                STBclass.StbModelClass.StbMembersClass.StbStrip_Footing strip_fo = stb.StbModel.StbMembers.StbStrip_Footings[i];
                                if (strip_fo.id_section != fo.id) { continue; }
                               ElementId eid = null;
                                double t_B = Get_Girder_B(stb, strip_fo.idNode_start, strip_fo.idNode_end, ref eid);

                                if (Search_Same_FoundationFamily(typename, t_B))
                                {
                                    int ascii = 97;
                                    do
                                    {
                                        typename = ReName(typename, ascii);
                                        ascii++;
                                    } while (Search_Same_FoundationFamily(typename, t_B));
                                }
                                //タイプの生成                        
                                if (!SearchFamilySymbol(ConvFamily[1][0], typename, ref symbol))
                                { symbol = (FamilySymbol)symbol.Duplicate(typename); }

                                ReNameSymbols re = new ReNameSymbols();
                                re.name = fo.name;
                                re.Length = t_B;
                                re.symbol = symbol;
                                re.id = fo.id;
                                FContiSymbols.Add(re);


                                FamilyStructure.Foundation_Continuous Rfo = SetFamily.FConti;
                                SetParameter(symbol.LookupParameter(Rfo.SecId), fo.id);
                                SetParameter(symbol.LookupParameter(Rfo.name), fo.name);
                                SetParameter(symbol.LookupParameter(Rfo.strength_concrete), fo.strength_concrete);
                                SetParameter(symbol.LookupParameter(Rfo.depth_cover_top), fo.depth_cover_top, true);
                                SetParameter(symbol.LookupParameter(Rfo.depth_cover_bottom), fo.depth_cover_bottom, true);
                                SetParameter(symbol.LookupParameter(Rfo.depth_cover_side), fo.depth_cover_side, true);
                                SetParameter(symbol.LookupParameter(Rfo.t_B), t_B, true);
                                if (fo.StbSecFigure.StbSecFigureType == 6)
                                {
                                    SetParameter(symbol.LookupParameter(Rfo.B), fo.StbSecFigure.StbSecContinuous.B, true);
                                    SetParameter(symbol.LookupParameter(Rfo.depth_base), fo.StbSecFigure.StbSecContinuous.depth_base, true);
                                    SetParameter(symbol.LookupParameter(Rfo.depth_tip), fo.StbSecFigure.StbSecContinuous.depth_tip, true);
                                    switch(fo.StbSecFigure.StbSecContinuous.type)
                                    {
                                        case "RIGHT_L":
                                            SetParameter(symbol.LookupParameter(Rfo.type_right), true);
                                            break;
                                        case "LEFT_L":
                                            SetParameter(symbol.LookupParameter(Rfo.type_left), true);
                                            break;
                                    }
                                    SetParameter(symbol.LookupParameter(Rfo.type), fo.StbSecFigure.StbSecContinuous.type);
                                }
                                if(fo.StbSecBar_Arrangement != null)
                                {
                                    string strength = "";
                                    if(fo.StbSecBar_Arrangement.StbSecBar_ArrangementType == 4)
                                    {
                                        for (int b = 0; b < fo.StbSecBar_Arrangement.StbSecContinuous.Count(); b++)
                                        {
                                            if(fo.StbSecBar_Arrangement.StbSecContinuous[b] == null) { continue; }
                                            SetParameter(symbol.LookupParameter(Rfo.D[b]), fo.StbSecBar_Arrangement.StbSecContinuous[b].D);
                                            SetParameter(symbol.LookupParameter(Rfo.count[b]), fo.StbSecBar_Arrangement.StbSecContinuous[b].count);
                                            SetParameter(symbol.LookupParameter(Rfo.pitch[b]), fo.StbSecBar_Arrangement.StbSecContinuous[b].pitch, true);
                                            if (strength == "")
                                            { strength = fo.StbSecBar_Arrangement.StbSecContinuous[b].strength; }
                                            else
                                            { strength = Compare_strength(fo.StbSecBar_Arrangement.StbSecContinuous[b].strength, strength); }
                                        }
                                    }

                                    Parameter_Select_Set(Rfo.strength, strength, symbol: symbol);
                                }
                            }
                        }
                    }                    
                    break;
            }


            return ret;
        }
        /// <summary> RC杭タイプパラメータ設定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pile"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreatePile_RC(STBclass stb, STBclass.StbModelClass.StbSectionsClass.StbSecPile_RC pile, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //タイプ名
            string typename = "";
            typename = pile.name;

            Family fam = null;
            FamilySymbol symbol = null;
            string type = "";
            
            if (ConvFamily[2][0] != null)
            {
                fam = ConvFamily[2][0];
                type = "場所打ち杭";
            }
            else if(ConvFamily[2][1] != null)
            {
                fam = ConvFamily[2][1];
                type = "既製杭";
            }
            if(fam == null)
            {
                //ログ表示（ファミリ未ロード)
                LogData.AddLog(LogData.LogKind.Warning, 2100, type);
                return ret;
            }
            //タイプの生成                        
            if (!SearchFamilySymbol(fam, typename, ref symbol))
            { symbol = (FamilySymbol)symbol.Duplicate(typename); }

            double[][] length = new double[0][];
            if (stb.StbModel.StbMembers.StbPiles != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbPiles.Count(); i++)
                {
                    if (stb.StbModel.StbMembers.StbPiles[i].id_section == pile.id)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbPile stbp = stb.StbModel.StbMembers.StbPiles[i];
                        bool addflg = true;
                        for (int j = 0; j <length.Count(); j++)
                        {
                           
                            if(length[j][0] == stbp.length_all && length[j][1] == stbp.length_head && length[j][2] == stbp.length_foot)
                            {
                                addflg = false;
                                break;
                            }
                        }
                        if(addflg)
                        {
                            Array.Resize(ref length, length.Count() + 1);
                            Array.Resize(ref length[length.Count() - 1], 3);
                            length[length.Count() - 1][0] = stbp.length_all;
                            length[length.Count() - 1][1] = stbp.length_head;
                            length[length.Count() - 1][2] = stbp.length_foot;
                        }
                    }
                }
            }

           
            for (int i = 0; i < length.Count(); i++)
            {
                if (i != 0)
                {
                    symbol = (FamilySymbol)symbol.Duplicate(typename + i.ToString());
                    ReNameSymbols s = new ReNameSymbols();
                    s.symbol = symbol;
                    s.name = pile.name;
                        
                    s.Length = length[i][0];
                    s.Length2 = length[i][1];
                    s.Length3 = length[i][2];
                    PilesSymbols.Add(s);
                }

                //形状
                bool length_Log = false;
                switch (type)
                {
                    case "場所打ち杭":
                        FamilyStructure.Pile Rpile = SetFamily.CastinPile;
                        SetParameter(symbol.LookupParameter(Rpile.strength_concrete), pile.strength_concrete);                       
                        switch (pile.StbSecFigure.StbSecFigureType)
                        {
                            case 1:
                                SetParameter(symbol.LookupParameter(Rpile.D), pile.StbSecFigure.StbSecStraight.D, true);
                                if (length[i][0] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_all), length[i][0], true); }
                                else
                                { length_Log = true; }
                                break;
                            case 2:
                                SetParameter(symbol.LookupParameter("拡底"), true);
                                SetParameter(symbol.LookupParameter(Rpile.D), pile.StbSecFigure.StbSecExtended_Foot.D_axial, true);
                                SetParameter(symbol.LookupParameter(Rpile.D_extended_foot), pile.StbSecFigure.StbSecExtended_Foot.D_extended_foot, true);
                                if (length[i][0] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_all), length[i][0], true); }
                                else
                                { length_Log = true; }
                                if (length[i][2] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_foot), length[i][2], true); }
                                else
                                { length_Log = true; }
                                SetParameter(symbol.LookupParameter(Rpile.length_foot_taper), 1000, true);
                                SetParameter(symbol.LookupParameter(Rpile.length_foot_Revit), 100, true);
                                break;
                            case 3:
                                SetParameter(symbol.LookupParameter("拡頭"), true);
                                SetParameter(symbol.LookupParameter(Rpile.D), pile.StbSecFigure.StbSecExtended_Top.D_axial, true);
                                SetParameter(symbol.LookupParameter(Rpile.D_extended_top), pile.StbSecFigure.StbSecExtended_Top.D_extended_top, true);
                                if (length[i][0] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_all), length[i][0], true); }
                                else
                                { length_Log = true; }
                                if (length[i][1] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_head), length[i][1], true); }
                                else
                                { length_Log = true; }
                                SetParameter(symbol.LookupParameter(Rpile.length_head_taper), length[i][1] / 2, true);
                                break;
                            case 4:
                                SetParameter(symbol.LookupParameter("拡底"), true);
                                SetParameter(symbol.LookupParameter("拡頭"), true);
                                SetParameter(symbol.LookupParameter(Rpile.D), pile.StbSecFigure.StbSecExtended_Top_Foot.D_axial, true);
                                SetParameter(symbol.LookupParameter(Rpile.D_extended_top), pile.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top, true);
                                SetParameter(symbol.LookupParameter(Rpile.D_extended_foot), pile.StbSecFigure.StbSecExtended_Top_Foot.D_extended_foot, true);
                                if (length[i][0] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_all), length[i][0], true); }
                                else
                                { length_Log = true; }
                                if (length[i][1] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_head), length[i][1], true); }
                                else
                                { length_Log = true; }
                                if (length[i][2] != 0)
                                { SetParameter(symbol.LookupParameter(Rpile.length_head_taper), length[i][1] / 2, true); }
                                else
                                { length_Log = true; }
                                SetParameter(symbol.LookupParameter(Rpile.length_foot), length[i][2], true);
                                SetParameter(symbol.LookupParameter(Rpile.length_foot_taper), 1000, true);
                                SetParameter(symbol.LookupParameter(Rpile.length_foot_Revit), 100, true);
                                break;
                        }
                        if (length_Log == true)
                        {
                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "杭長さが0mmのためファミリのデフォルト値で変換しました。");
                            SetParameter(symbol.LookupParameter(Rpile.zeroLength), true);
                        }
                        else
                        {
                            SetParameter(symbol.LookupParameter(Rpile.zeroLength), false);
                        }

                        //配筋
                        switch (pile.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                        {
                            case 1:
                                for (int j = 0; j < 3; j++)
                                {
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_band);
                                    SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_band);
                                break;
                            case 2:
                                for (int j = 0; j < 3; j++)
                                {
                                    int newj = 0;
                                    if (j == 1)
                                    { newj = 0; }
                                    else if (j == 2)
                                    { newj = 1; }
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_band);
                                    SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_band);
                                break;
                            case 3:
                                for (int j = 0; j < 3; j++)
                                {
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_band);
                                    SetParameter(symbol.LookupParameter(Rpile.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_band);
                                break;
                        }
                        SetParameter(symbol.LookupParameter(Rpile.name), pile.name);
                        SetParameter(symbol.LookupParameter(Rpile.depth_cover), pile.depth_cover);
                        SetParameter(symbol.LookupParameter(Rpile.depth_cover_top), pile.depth_cover_top);
                        SetParameter(symbol.LookupParameter(Rpile.SecId), pile.id);
                        break;
                    case "既製杭":
                        FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                        ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                        IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();
                        Family ko_fam = null; //子ファミリ
                        foreach (Element el in elements)
                        {
                            FamilySymbol familySymbol = el as FamilySymbol;

                            if (familySymbol != null)
                            {
                                if (familySymbol.FamilyName == "PCa")
                                {
                                    ko_fam = familySymbol.Family;
                                    break;
                                }
                            }
                        }
                        if(ko_fam == null)
                        {
                            //ログ
                            LogData.AddLog(LogData.LogKind.Warning, 0, "[既製杭]既製杭ファミリが不正のため変換できません。");
                            Commons.doc.Delete(symbol.Id);
                            return ret;
                        }
                        FamilyStructure.Pile_2 Rpile_2 = SetFamily.PrecastPile;

                        string typename1 = "";
                        string typename2 = "";
                        string typename3 = "";
                        switch (pile.StbSecFigure.StbSecFigureType)
                        {
                            case 1:
                                //子ファミリタイプの生成   
                                FamilySymbol symbol_s = null;
                                typename1 = typename + "_φ" + pile.StbSecFigure.StbSecStraight.D.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename1, ref symbol_s))
                                { symbol_s = (FamilySymbol)symbol_s.Duplicate(typename1); }
                                SetParameter(symbol_s.LookupParameter("D"), pile.StbSecFigure.StbSecStraight.D, true);

                                SetParameter(symbol.LookupParameter(Rpile_2.straight_D), symbol_s.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.straight_length), length[i][0],true);
                                break;
                            case 2:
                                //子ファミリタイプの生成   
                                FamilySymbol symbol_f1 = null;
                                typename1 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Foot.D_axial.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename1, ref symbol_f1))
                                { symbol_f1 = (FamilySymbol)symbol_f1.Duplicate(typename1); }
                                FamilySymbol symbol_f2 = null;
                                typename2 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Foot.D_extended_foot.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename2, ref symbol_f2))
                                { symbol_f2 = (FamilySymbol)symbol_f2.Duplicate(typename2); }
                                SetParameter(symbol.LookupParameter("拡底"), true);
                                SetParameter(symbol_f1.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Foot.D_axial, true);
                                SetParameter(symbol_f2.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Foot.D_extended_foot, true);

                                SetParameter(symbol.LookupParameter(Rpile_2.ef_D_axial), symbol_f1.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.ef_D_extended_foot), symbol_f2.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.ef_length_axial), length[i][0] - length[i][2], true);
                                SetParameter(symbol.LookupParameter(Rpile_2.ef_length_foot), length[i][2], true);
                                break;
                            case 3:
                                //子ファミリタイプの生成   
                                FamilySymbol symbol_t1 = null;
                                typename1 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Top.D_extended_top.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename1, ref symbol_t1))
                                { symbol_t1 = (FamilySymbol)symbol_t1.Duplicate(typename1); }
                                FamilySymbol symbol_t2 = null;
                                typename2 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Top.D_axial.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename2, ref symbol_t2))
                                { symbol_t2 = (FamilySymbol)symbol_t2.Duplicate(typename2); }
                                SetParameter(symbol.LookupParameter("拡頭"), true);
                                SetParameter(symbol_t1.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Top.D_extended_top, true);
                                SetParameter(symbol_t2.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Top.D_axial, true);

                                SetParameter(symbol.LookupParameter(Rpile_2.et_D_extended_top), symbol_t1.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.et_D_axial), symbol_t2.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.et_length_head), length[i][1], true);
                                SetParameter(symbol.LookupParameter(Rpile_2.et_length_axial), length[i][0] - length[i][1], true);
                                break;
                            case 4:
                                //子ファミリタイプの生成   
                                FamilySymbol symbol_tf1 = null;
                                typename1 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename1, ref symbol_tf1))
                                { symbol_tf1 = (FamilySymbol)symbol_tf1.Duplicate(typename1); }
                                FamilySymbol symbol_tf2 = null;
                                typename2 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Top_Foot.D_axial.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename2, ref symbol_tf2))
                                { symbol_tf2 = (FamilySymbol)symbol_tf2.Duplicate(typename2); }
                                FamilySymbol symbol_tf3 = null;
                                typename3 = typename + "_φ" + pile.StbSecFigure.StbSecExtended_Top_Foot.D_axial.ToString();
                                if (!SearchFamilySymbol(ko_fam, typename3, ref symbol_tf3))
                                { symbol_tf3 = (FamilySymbol)symbol_tf3.Duplicate(typename3); }
                                SetParameter(symbol.LookupParameter("拡底"), true);
                                SetParameter(symbol.LookupParameter("拡頭"), true);
                                SetParameter(symbol_tf1.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Top_Foot.D_extended_top, true);
                                SetParameter(symbol_tf2.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Top_Foot.D_axial, true);
                                SetParameter(symbol_tf3.LookupParameter("D"), pile.StbSecFigure.StbSecExtended_Top_Foot.D_extended_foot, true);

                                SetParameter(symbol.LookupParameter(Rpile_2.etf_D_extended_top), symbol_tf1.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.etf_D_axial), symbol_tf2.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.etf_D_extended_foot), symbol_tf3.Id);
                                SetParameter(symbol.LookupParameter(Rpile_2.etf_length_head), length[i][1], true);
                                SetParameter(symbol.LookupParameter(Rpile_2.etf_length_axial), length[i][0] - length[i][1] - length[i][2], true);
                                SetParameter(symbol.LookupParameter(Rpile_2.etf_length_foot), length[i][2], true);
                                break;
                        }
                        //配筋
                        switch (pile.StbSecBar_Arrangement.StbSecBar_ArrangementType)
                        {
                            case 1:
                                for (int j = 0; j < 3; j++)
                                {
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.D_band);
                                    SetParameter(symbol.LookupParameter(Rpile_2.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Same.pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Same.strength_band);
                                break;
                            case 2:
                                for (int j = 0; j < 3; j++)
                                {
                                    int newj = 0;
                                    if (j == 1)
                                    { newj = 0; }
                                    else if (j == 2)
                                    { newj = 1; }
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].D_band);
                                    SetParameter(symbol.LookupParameter(Rpile_2.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[newj].pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Top_Bottom[0].strength_band);
                                break;
                            case 3:
                                for (int j = 0; j < 3; j++)
                                {
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_circumference_1st[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].count_main_circumference_1st);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.count_main_core[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].count_main_core);
                                    SetParameter(symbol.LookupParameter(Rpile_2.D_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].D_band);
                                    SetParameter(symbol.LookupParameter(Rpile_2.pitch_band[j]), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[j].pitch_band, true);
                                }
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_circumference_1st), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_main_circumference_1st);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_main_core), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_main_core);
                                SetParameter(symbol.LookupParameter(Rpile_2.strength_band), pile.StbSecBar_Arrangement.StbSecPile_Top_Center_Bottom[0].strength_band);
                                break;
                        }
                        SetParameter(symbol.LookupParameter(Rpile_2.name), pile.name);
                        SetParameter(symbol.LookupParameter(Rpile_2.depth_cover), pile.depth_cover);
                        SetParameter(symbol.LookupParameter(Rpile_2.depth_cover_top), pile.depth_cover_top);
                        SetParameter(symbol.LookupParameter(Rpile_2.SecId), pile.id);

                        break;
                }
                
                
            }
            return ret;
        }

        /// <summary>基礎インスタンスパラメータ設定（基礎矩形・基礎矩形テーパー）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateFoundation_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbFooting footing, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;          
            Family fami = null;
            //タイプ名
            string typename = "";

            STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC secf = null;
            for (int i =0; i <stb.StbModel.StbSections.StbSecFoundations_RC.Count(); i++)
            {
                if(stb.StbModel.StbSections.StbSecFoundations_RC[i].id == footing.id_section)
                {
                    secf = stb.StbModel.StbSections.StbSecFoundations_RC[i];
                    break;
                }
            }

            if(secf == null)
            {
                return ret;
            }
            
            typename = secf.name;
           

            switch(secf.StbSecFigure.StbSecFigureType)
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

            if(fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (!SearchFamilySymbol(fami, typename, ref symbol))
            {
                //ログ表示(タイプが無い)
                LogData.AddLog(LogData.LogKind.Warning, 2300, "[基礎]" + typename + "(配置Id=" + footing.id.ToString() + ")");
                return ret;
            }

            //配置座標の取得
            double depth = Get_Foundation_depth(stb, footing.id_section);
            XYZ P = Get_Node_Position(stb, footing.idNode, footing.offset_X, footing.offset_Y, footing.level_bottom);

            //所属層
            int ind = Get_stbFloor_index(stb, footing.idNode);
            Level btmlevel = SearchLevel(stb, ind);

            //インスタンスの生成
            try
            {
                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, btmlevel, StructuralType.Footing);

                //レベルからの高さオフセット
                SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), P.Z - btmlevel.Elevation, false);

                //断面回転
                instance.Location.Rotate(Line.CreateBound(P, P + 5 * BasisZ.Normalize()), (footing.rotate * Math.PI) / 180);


                List<string> pnames = new List<string>();
                switch (secf.StbSecFigure.StbSecFigureType)
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

                SetParameter(instance.LookupParameter(pnames[0]), footing.id);
                SetParameter(instance.LookupParameter(pnames[1]), footing.name);
                SetParameter(instance.LookupParameter(pnames[2]), footing.thickness_ex_start_X);
                SetParameter(instance.LookupParameter(pnames[3]), footing.thickness_ex_end_X);
                SetParameter(instance.LookupParameter(pnames[4]), footing.thickness_ex_start_Y);
                SetParameter(instance.LookupParameter(pnames[5]), footing.thickness_ex_end_Y);
                SetParameter(instance.LookupParameter(pnames[6]), footing.thickness_ex_top);
                SetParameter(instance.LookupParameter(pnames[7]), footing.thickness_ex_bottom);


                if (!FGroup.ContainsKey(footing.idNode))
                {
                    FGroup.Add(footing.idNode, new List<ElementId>());
                }
                FGroup[footing.idNode].Add(instance.Id);
                
                LogData.AddLog( LogData.LogKind.Infmoation, 0, $"基礎の生成：\t[配置Id {footing.idNode}]{symbol.Name} 要素ID{instance.Id}" ) ;
                OutputDebubCommentLog( instance, footing.idNode, "基礎", symbol.Name, new int[]{} ) ;
                
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }
        /// <summary>基礎インスタンスパラメータ設定（布基礎）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreateStripFooting_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbStrip_Footing footing, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            Family fami = null;
            //タイプ名
            string typename = "";

            STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC secf = null;
            for (int i = 0; i < stb.StbModel.StbSections.StbSecFoundations_RC.Count(); i++)
            {
                if (stb.StbModel.StbSections.StbSecFoundations_RC[i].id == footing.id_section)
                {
                    secf = stb.StbModel.StbSections.StbSecFoundations_RC[i];
                    break;
                }
            }

            if (secf == null)
            {
                return ret;
            }

            typename = secf.name;

            fami = ConvFamily[1][0];

            if (fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "基礎");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            ElementId eid = null;
            double t_B = Get_Girder_B(stb, footing.idNode_start, footing.idNode_end, ref eid);
            for(int i =0; i < FContiSymbols.Count(); i++)
            {
                if(FContiSymbols[i].Length == t_B && FContiSymbols[i].id == footing.id_section)
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
            XYZ Ps = Get_Node_Position(stb, footing.idNode_start, 0, 0, 0);
            XYZ Pe = Get_Node_Position(stb, footing.idNode_end, 0, 0, 0);

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
                for(int i = 0; i < CGrp.Count(); i++)
                {
                    if(CGrp[i].start_node == footing.idNode_start && CGrp[i].end_node == footing.idNode_end)
                    {
                        ins2 = CGrp[i].elem[0] as FamilyInstance;
                        cgrp_ind = i;
                    }
                }
                if(ins2 == null)
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
                        GeometryInstance　gins = genum.Current as GeometryInstance;
                        if(gins == null) { continue; }
                        //GeometryInstanceをもう一度取得して、そこからSolidを取る
                        ge = gins.SymbolGeometry;
                        IEnumerator<GeometryObject>  genum2 = ge.GetEnumerator();
                        genum2.Reset();
                        while(genum2.MoveNext())
                        {
                            sld = genum2.Current as Solid;
                            if(sld == null) { continue; }
                            if (sld.Volume <= 0) { continue; }
                            if (sld.Faces.Size == 0) { continue; }
                            tf = gins.Transform;
                            break;
                        }
                    }
                    if(sld == null) { continue; }
                    if (sld.Volume <= 0) { continue; }
                    if (sld.Faces.Size == 0) { continue; }

                    for (int i = 0; i < sld.Faces.Size; i++)
                    {
                        PlanarFace pface = sld.Faces.get_Item(i) as PlanarFace;
                        if (pface == null) { continue; }
                        if (pface.Reference == null) { continue; }

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

                FamilyStructure.Foundation_Continuous Rfo = SetFamily.FConti;

                SetParameter(instance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM), f_level, false); //レベルからの高さオフセット
                SetParameter(instance.get_Parameter(BuiltInParameter.INSTANCE_MOVES_WITH_GRID_PARAM), false); //通心に沿ってい移動
                SetParameter(instance.LookupParameter(Rfo.MemId), footing.id);
                SetParameter(instance.LookupParameter(Rfo.NameMembers), footing.name);
                SetParameter(instance.LookupParameter(Rfo.length_ex_start), footing.length_ex_start);
                SetParameter(instance.LookupParameter(Rfo.length_ex_end), footing.length_ex_end);

                //変換情報ログの出力
                var nodeIds = new int[] { footing.idNode_start, footing.idNode_end } ;
                MakeNodeLog( "布基礎の生成：", "[配置Id " + footing.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                OutputDebubCommentLog( instance, footing.id, "布基礎", typename, nodeIds ) ;

                if(cgrp_ind != -1)
                {
                    CGrp[cgrp_ind].elId.Add(instance.Id);
                }

            }
            catch (Exception)
            {
                
                ret = false;
            }


            return ret;
        }
        /// <summary>基礎インスタンスパラメータ設定（杭）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="clm"></param>
        /// <param name="sclmind"></param>
        /// <param name="pform"></param>
        /// <param name="ConvFamily"></param>
        /// <returns></returns>
        private bool CreatePile_instance(STBclass stb, STBclass.StbModelClass.StbMembersClass.StbPile pile, ProgressBarForm pform, Family[][] ConvFamily)
        {
            bool ret = true;

            //柱断面情報から使用するファミリなどを取得
            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            Family fami = null;
            //タイプ名
            string typename = "";

            STBclass.StbModelClass.StbSectionsClass.StbSecPile_RC secp = null;
            for (int i = 0; i < stb.StbModel.StbSections.StbSecPiles_RC.Count(); i++)
            {
                if (stb.StbModel.StbSections.StbSecPiles_RC[i].id == pile.id_section)
                {
                    secp = stb.StbModel.StbSections.StbSecPiles_RC[i];
                    break;
                }
            }

            if (secp == null)
            {
                return ret;
            }

            typename = secp.name;


            if (ConvFamily[2][0] != null)
            { fami = ConvFamily[2][0]; }
            else
            { fami = ConvFamily[2][1]; }
            
                    

            if (fami == null)
            {
                //ログ
                LogData.AddLog(LogData.LogKind.Warning, 2100, "杭");
                return ret;
            }

            //タイプがすでに生成されているか
            FamilySymbol symbol = null;
            if (!SearchFamilySymbol(fami, typename, ref symbol))
            {
                //ReNameされているとき
                symbol = null;
                for (int i = 0; i < PilesSymbols.Count(); i++)
                {
                    if (PilesSymbols[i].name == typename && PilesSymbols[i].Length == pile.length_all && 
                        PilesSymbols[i].Length2 == pile.length_head && PilesSymbols[i].Length3 == pile.length_foot)
                    {
                        symbol = PilesSymbols[i].symbol;
                        break;
                    }
                }
                if (symbol == null)
                {
                    //ログ表示(タイプが無い)
                    LogData.AddLog(LogData.LogKind.Warning, 2300, "[杭]" + typename + "(配置id=" + pile.id + ")");
                    return ret;
                }
                return ret;
            }

            //配置座標の取得
            XYZ P = Get_Node_Position(stb, pile.idNode, pile.offset_X, pile.offset_Y, 0);

            //インスタンスの生成
            try
            {
                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, StructuralType.Footing);
                double level_top = 0;
                if (ConvFamily[2][0] != null) {level_top = pile.level_top + Commons.ft2mm(P.Z); }
                else if(ConvFamily[2][1] != null) { level_top = pile.level_top; }
                SetParameter(instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM), level_top, true); //レベルからの高さオフセット

                if (ConvFamily[2][0] != null)
                {
                    var Rfo = SetFamily.CastinPile;
                    SetParameter(instance.LookupParameter(Rfo.MemId), pile.id);
                    SetParameter(instance.LookupParameter(Rfo.NameMembers), pile.name);
                }
                else
                {
                    var Rfo = SetFamily.PrecastPile;
                    SetParameter(instance.LookupParameter(Rfo.MemId), pile.id);
                    SetParameter(instance.LookupParameter(Rfo.NameMembers), pile.name);
                }

                if (!FGroup.ContainsKey(pile.idNode))
                {
                    FGroup.Add(pile.idNode, new List<ElementId>());
                }
                FGroup[pile.idNode].Add( instance.Id);
                
                LogData.AddLog( LogData.LogKind.Infmoation, 0, $"杭の生成：\t[配置Id {pile.idNode}]{symbol.Name} 要素ID{instance.Id}" ) ;
                OutputDebubCommentLog( instance, pile.idNode, "杭", symbol.Name, new int[]{} ) ;
                
            }
            catch (Exception)
            {
                ret = false;
            }


            return ret;
        }

        #endregion
        /// <summary> 柱脚の生成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="pform"></param>
        /// <param name="syubetu"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool CreateClmBase(STBclass stb, ProgressBarForm pform, string syubetu, ref string errmsg)
        {
            bool ret = true;
            //プロジェクトにロードさているファミリをもう一度確認する
            LoadFamily lofa = new LoadFamily();
            lofa.LoadFfamily_fromProject();
            //柱脚ファミリの取得
            Dictionary<string, Family> BClmFamily = new Dictionary<string, Family>();
            Dictionary<string, string> mappingTypeName = new Dictionary<string, string>();
            for (int i = 0; i < RevitLNK.BClm.Count(); i++)
            {
                if (!RevitLNK.BClm[i].flg) { continue; }
               
                for (int j = 0; j < LoadFamily.ProFami.Count(); j++)
                {
                    if(LoadFamily.ProFami[j] == null) { continue; }
                    string rfaname = System.IO.Path.GetFileNameWithoutExtension(RevitLNK.BClm[i].rfa_pass);
                    if (rfaname == LoadFamily.ProFami[j].Name)
                    {
                        BClmFamily.Add(RevitLNK.BClm[i].product_code, LoadFamily.ProFami[j]);
                        mappingTypeName.Add(RevitLNK.BClm[i].product_code, RevitLNK.BClm[i].typename);
                    }
                }
            }

            //S柱
            if (stb.StbModel.StbSections.StbSecColumns_S != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumns_S.Count();

                for (int i = 0; i < numCount; i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_S clm = stb.StbModel.StbSections.StbSecColumns_S[i];
                   
                    if (clm.StbSecBaseProduct != null)
                    {
                        BaseClass newb = new BaseClass();
                        newb.id_section = clm.id;
                        newb.clmname = clm.name;
                        newb.clm_structure = "S";
                        newb.product_company = clm.StbSecBaseProduct.product_company;
                        newb.product_code = clm.StbSecBaseProduct.product_code;
                        BClm.Add(newb);
                    }
                }
            }
            //SRC柱
            if (stb.StbModel.StbSections.StbSecColumns_SRC != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumns_SRC.Count();

                for (int i = 0; i < numCount; i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_SRC clm = stb.StbModel.StbSections.StbSecColumns_SRC[i];
                    if (clm.StbSecBaseProduct != null)
                    {
                        BaseClass newb = new BaseClass();
                        newb.id_section = clm.id;
                        newb.clmname = clm.name;
                        newb.clm_structure = "SRC";
                        newb.product_company = clm.StbSecBaseProduct.product_company;
                        newb.product_code = clm.StbSecBaseProduct.product_code;
                        BClm.Add(newb);
                    }
                }
            }
            //CFT柱
            if (stb.StbModel.StbSections.StbSecColumns_CFT != null)
            {
                int numCount = stb.StbModel.StbSections.StbSecColumns_CFT.Count();

                for (int i = 0; i < numCount; i++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecColumn_CFT clm = stb.StbModel.StbSections.StbSecColumns_CFT[i];
                   

                    if (clm.StbSecBaseProduct != null)
                    {
                        BaseClass newb = new BaseClass();
                        newb.id_section = clm.id;
                        newb.clmname = clm.name;
                        newb.clm_structure = "CFT";
                        newb.product_company = clm.StbSecBaseProduct.product_company;
                        newb.product_code = clm.StbSecBaseProduct.product_code;
                        BClm.Add(newb);
                    }
                }
            }

            Stopwatch stopw = new Stopwatch();
            stopw.Start();

            ProgressBar_Show(pform, "柱脚の生成");

            Transaction tran = new Transaction(Commons.doc, "柱脚タイプ");
            try
            {
                tran.Start();

                for (int i = 0; i < BClm.Count(); i++)
                {
                    GaugePercent("柱脚の生成", (int)((double)i / (double)BClm.Count() * 100));
                    string typename = mappingTypeName[BClm[i].product_code]; //マッピングテーブルで指定されたタイプ名
                    Family ConvFamily = BClmFamily[BClm[i].product_code];    //マッピングテーブルで指定されたファミリ名                    

                    if (ConvFamily == null)
                    {
                        //ログ表示（ファミリ未ロード)
                        LogData.AddLog(LogData.LogKind.Warning, 2100, "柱脚[" + BClm[i].product_code + "]");
                        pform.TopMost = false;
                        tran.RollBack();
                        pform.TopMost = true;
                        return ret;
                    }
                                                        
                    FamilySymbol symbol = null;
                   
                    if(typename == "")
                    { typename = ConvFamily.Name; }

                    if (!SearchFamilySymbol(ConvFamily, typename, ref symbol))
                    { symbol = (FamilySymbol)symbol.Duplicate(typename); }
                    symbol.Activate();
                    SetParameter(symbol.LookupParameter("符号"), typename);
                    //インスタンス
                    int id_member = 0;
                    int node_id = 0;
                    if (stb.StbModel.StbMembers.StbColumns != null)
                    {
                        for (int j = 0; j < stb.StbModel.StbMembers.StbColumns.Count(); j++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbColumn clm = stb.StbModel.StbMembers.StbColumns[j];
                            if (clm.id_section == BClm[i].id_section)
                            {
                                id_member = clm.id;
                                node_id = clm.idNode_bottom;

                                //XYZ P = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_bottom_X + clm.offset_X, clm.offset_bottom_Y + clm.offset_Y, clm.offset_bottom_Z);
                                XYZ P = new XYZ();
                                if (clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0 ||
                                    clm.offset_top_X != 0 || clm.offset_top_Y != 0)
                                {
                                    P = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, clm.offset_bottom_Z);
                                }
                                else
                                {
                                    P = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_X, clm.offset_Y, 0);
                                }

                                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                                //配置レベルの取得
                                int indb = Get_stbFloor_index(stb, clm.idNode_bottom);
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
                                //double offset_b = 0;
                                //if (btmLevel != null)
                                //{
                                //    //柱脚のZ座標と基準レベルの代表値の差
                                //     offset_b = Commons.ft2mm(P.Z - btmLevel.Elevation);
                                //}

                                //ホスト指定していないのでZ座標をそのまま使用
                                double offset_b = Commons.ft2mm(P.Z);

                                double gir_offset_Z_bottom = 0;
                                if (clm.offset_bottom_Z == 0) //柱脚Z方向オフセット値が0以外の時はその値を優先する
                                { Search_Girder_Offset_Z_bottom(stb, clm.idNode_bottom, btmLevel, clm.kind_structure, ref gir_offset_Z_bottom); }
                                SetParameter(instance, BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM, offset_b + gir_offset_Z_bottom, true);

                                ////回転
                                //XYZ Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_X + clm.offset_top_X, clm.offset_Y + clm.offset_top_Y, 0);
                                //XYZ Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_X + clm.offset_bottom_X, clm.offset_Y + clm.offset_bottom_Y, 0);
                                //instance.Location.Rotate(Line.CreateBound(Pb, Pb + (Pt - Pb).Normalize()), (clm.rotate * Math.PI) / 180);

                                //回転
                                instance.Location.Rotate(Line.CreateBound(P, P + XYZ.BasisZ), (clm.rotate * Math.PI) / 180);

                                //変換情報ログの出力
                                var nodeIds = new int[] { clm.idNode_bottom } ;
                                MakeNodeLog( "柱脚の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                                OutputDebubCommentLog( instance, clm.id, "柱脚", typename, nodeIds ) ;
                            }
                        }
                    }
                    if (stb.StbModel.StbMembers.StbPosts != null)
                    {
                        for (int j = 0; j < stb.StbModel.StbMembers.StbPosts.Count(); j++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbPost clm = stb.StbModel.StbMembers.StbPosts[j];
                            if (clm.id_section == BClm[i].id_section)
                            {
                                id_member = clm.id;
                                node_id = clm.idNode_bottom;
                                XYZ P = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_bottom_X, clm.offset_bottom_Y, clm.offset_bottom_Z);
                                FamilyInstance instance = Commons.doc.Create.NewFamilyInstance(P, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                //回転
                                XYZ Pt = Get_Node_Position(stb, clm.idNode_top, clm.offset_X + clm.offset_top_X, clm.offset_Y + clm.offset_top_Y, 0);
                                XYZ Pb = Get_Node_Position(stb, clm.idNode_bottom, clm.offset_X + clm.offset_bottom_X, clm.offset_Y + clm.offset_bottom_Y, 0);
                                instance.Location.Rotate(Line.CreateBound(Pb, Pb + (Pt - Pb).Normalize()), -(clm.rotate * Math.PI) / 180);
                                //変換情報ログの出力
                                var nodeIds = new int[] { clm.idNode_bottom } ;
                                MakeNodeLog( "柱脚の生成：", "[配置Id " + clm.id.ToString() + "]" + typename, nodeIds, 0, instance.Id ) ;
                                OutputDebubCommentLog( instance, clm.id, "柱脚", typename, nodeIds ) ;
                            }
                        }
                    }
                    
                }


                pform.TopMost = false;
                tran.Commit();
                pform.TopMost = true;
            }
            catch (Exception)
            {
                ret = false;
                errmsg = "柱脚";
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }

            // 進捗ゲージの消去
            if (form != null)
            {
                do { Application.DoEvents(); } while (stopw.ElapsedMilliseconds <= 1000); ;
                stopw.Stop();
                GaugeClose();
            }
            return ret;
        }
        #endregion 部材の生成

        

        /// <summary>基礎梁・布基礎グループ化
        /// </summary>
        /// <param name="start_node"></param>
        /// <param name="end_node"></param>
        /// <param name="el"></param>
        /// <param name="elem"></param>
        private void CGrp_Add(STBclass stb, int start_node, int end_node, ElementId el, Element elem = null)
        {
            bool addflg = true;
            for (int i = 0; i < CGrp.Count(); i++)
            {
                if(Node_Check(stb, CGrp[i].start_node, start_node)&& Node_Check(stb, CGrp[i].end_node, end_node))
                {
                    bool sameflg = false;
                    for (int j = 0; j < CGrp[i].elId.Count(); j++)
                    {
                        if(CGrp[i].elId[j] == el)
                        {
                            sameflg = true;
                            addflg = false;
                            break;
                        }
                    }
                    if(sameflg)
                    {
                        CGrp[i].elId.Add(el);
                        if(elem != null)
                        { CGrp[i].elem.Add(elem); }
                        addflg = false;
                    }
                }
            }
            if (addflg)
            {
                CGroup cgp = new CGroup();
                cgp.start_node = start_node;
                cgp.end_node = end_node;
                cgp.elId.Add(el);
                if (elem != null)
                {
                    cgp.elem.Add(elem);
                }
                CGrp.Add(cgp);
            }
        }
        /// <summary>true：パラメータSecIdに0以外の数字が追加されている⇒変換で生成されたタイプ
        /// </summary>
        /// <param name="paraname"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        private bool Check_SecID(string paraname, FamilySymbol fs)
        {
            bool ret = false;
            Parameter p = fs.LookupParameter(SetFamily.RCGir_F.SecId);
            if (p != null)
            {
                int p_id = p.AsInteger();
                if (p_id != 0)//変換によって作られたタイプ
                { ret = true; }
            }
            return ret;
        }


        /// <summary>勝ち負け判定
        /// </summary>
        /// <param name="pform"></param>
        /// <returns></returns>
        private int ChangeOrder(ProgressBarForm pform)
        {
            int ret = 0;

            Transaction tran = new Transaction(Commons.doc, "結合順序切り替え");

            try
            {
                tran.Start();

                Dictionary<int, List<ElementId>> JoinedElement = new Dictionary<int, List<ElementId>>();
                JoinedElement.Add((int)Joinorder.pile, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.foundation, new List<ElementId>());
                JoinedElement.Add ((int)Joinorder.column, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.girder, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.beam, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.wall, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.brace, new List<ElementId>());
                JoinedElement.Add((int)Joinorder.slab, new List<ElementId>());


                //基礎
                FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
                IList<Element> elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    FamilyInstance ins = elems[i] as FamilyInstance;
                    if (ins == null) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        if (elems[i].Name.Contains("pile") || elems[i].Name.Contains("Pile"))
                        {
                            JoinedElement[(int)Joinorder.pile].Add(elems[i].Id);
                        }
                        else
                        {
                            JoinedElement[(int)Joinorder.foundation].Add(elems[i].Id);
                        }
                    }
                }

                //柱
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for(int i = 0; i < elems.Count(); i++)
                {
                    FamilyInstance ins = elems[i] as FamilyInstance;
                    if (ins == null) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Joinorder.column].Add(elems[i].Id);
                    }
                }

                //大梁小梁ブレース
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    FamilyInstance ins = elems[i] as FamilyInstance;
                    if (ins == null) { continue; }

                    //SRCの始端の接合部カットバック・終端の接合部カットバックを0に設定
                    if (ins.Symbol.FamilyName == "SRC_Girder_icj")
                    {
                        SetParameter(ins, BuiltInParameter.START_JOIN_CUTBACK, 1.0, true);
                        SetParameter(ins, BuiltInParameter.END_JOIN_CUTBACK, 1.0, true);
                    }
                    else
                    {
                        SetParameter(ins, BuiltInParameter.START_JOIN_CUTBACK, 0.0, true);
                        SetParameter(ins, BuiltInParameter.END_JOIN_CUTBACK, 0.0, true);
                    }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        StructuralInstanceUsage usage = (StructuralInstanceUsage)(ins.get_Parameter(BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM).AsInteger());
                        switch (usage)
                        {
                            case StructuralInstanceUsage.Girder:
                                JoinedElement[(int)Joinorder.girder].Add(elems[i].Id);                                
                                break;
                            case StructuralInstanceUsage.Joist:
                                JoinedElement[(int)Joinorder.beam].Add(elems[i].Id);
                                break;
                            case StructuralInstanceUsage.Brace:
                            case StructuralInstanceUsage.HorizontalBracing:
                            case StructuralInstanceUsage.KickerBracing:
                            case StructuralInstanceUsage.Other:
                                JoinedElement[(int)Joinorder.brace].Add(elems[i].Id);
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
                    Wall ins = elems[i] as Wall;
                    if (ins == null) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Joinorder.wall].Add(elems[i].Id);
                    }
                }

                //床
                collector = new FilteredElementCollector(Commons.doc);
                filter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
                elems = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                for (int i = 0; i < elems.Count(); i++)
                {
                    Floor ins = elems[i] as Floor;
                    if (ins == null) { continue; }

                    ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, ins);
                    if (joined.Count > 0)
                    {
                        JoinedElement[(int)Joinorder.slab].Add(elems[i].Id);
                    }
                }

                Stopwatch stopw = new Stopwatch();
                stopw.Start();

                ProgressBar_Show(pform, "結合順序切り替え");

                //切り替え開始
                for (int j1 = 0; j1 < JoinedElement.Count() - 1; j1++)
                {
                    //プログレスバーの表示
                    GaugePercent("結合順序切り替え", (int)((double)j1 / (double)JoinedElement.Count() * 100));

                    for (int e1 = 0; e1 < JoinedElement[j1].Count(); e1++)
                    {
                        ElementId eid1 = JoinedElement[j1][e1];
                        Element elm1 = Commons.doc.GetElement(eid1);
                        ICollection<ElementId> joined = JoinGeometryUtils.GetJoinedElements(Commons.doc, elm1);

                        for (int e2 = 0; e2 < joined.Count(); e2++)
                        {                            
                            List<ElementId> dd = new List<ElementId>();
                            dd = joined.ToList();
                            ElementId eid2 = dd[e2];
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
                pform.TopMost = false;
                tran.Commit();
                pform.TopMost = true;
            }
            catch(Exception)
            {
                pform.TopMost = false;
                tran.RollBack();
                pform.TopMost = true;
            }

            return ret;
        }

        #region 鉄骨サイズチェック⇒成・幅・厚さ0なら変換対象外
        private string Roll_H_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_H_Class steel)
        {
            string txt = "";

            if(steel.A == 0)
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
        private string Build_H_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_H_Class steel)
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
        private string Roll_Box_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_BOX_Class steel)
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
        private string Build_Box_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecBuild_BOX_Class steel)
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
        private string Pipe_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecPipe_Class steel)
        {
            string txt = "";

            if(steel.D == 0)
            {
                txt = "直径";
            }
            if(steel.t == 0)
            {
                if(txt == "")
                { txt = "板厚"; }
                else
                { txt += ",板厚"; }
            }
            return txt;
        }
        private string Roll_T_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_T_Class steel)
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
        private string Roll_C_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_C_Class steel)
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
        private string Roll_L_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_L_Class steel)
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
        private string Rool_LipC_Size_Check(STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class.StbSecRoll_LipC_Class steel)
        {
            string txt = "";

            if(steel.H == 0)
            { txt += "成"; }
            if(steel.A == 0)
            {
                if(txt == "")
                { txt += "幅"; }
                else
                { txt += ",幅"; }
            }
            if(steel.C == 0)
            {
                if(txt == "")
                { txt += "リップ長"; }
                else
                { txt += ",リップ長"; }
            }
            if(steel.t == 0)
            {
                if(txt == "")
                { txt += "板厚"; }
                else
                { txt += ",板厚"; }
            }
            return txt;
        }
        #endregion



       



        


        /// <summary> 壁・スラブの鉄筋径(二つ入力されていることがある) 
        /// </summary>
        /// <param name="temp2"></param>
        /// <param name="Diameter"></param>
        private int Get_D( string temp2, ref string[] Diameter)
        {
            int d = -1;
            int retcode = 0;
            if (temp2 == "") { retcode = 1; }
            else if (!temp2.Contains("D") && !temp2.Contains("S") && !temp2.Contains("R")) { retcode = 2; }
            else
            {
                for (int i = 0; i < Diameter.Length; i++)
                {
                    Diameter[i] = "";
                }

                for (int i = 0; i < temp2.Length; i++)
                {
                    if (temp2[i] == 'D' || temp2[i] == 'S' || temp2[i] == 'R')
                    {
                        if(i + 1 > temp2.Length - 1) { retcode = 2; break; }
                        if (!int.TryParse(temp2[i + 1].ToString(), out int n))
                        {
                            retcode = 2;
                            break;
                        }
                        if (n == 0) { retcode = 2; break; }
                        else { d++; }
                    }
                    Diameter[d] += temp2[i];
                }
            }
            return retcode;
        }

       
        private void Get_D(string buzai, ref string kei_str, string tekkin_name, string typename, int id)
        {
            if (kei_str == "")
            {
                switch (tekkin_name)
                {
                    case "主筋":
                    case "帯筋":
                    case "あばら筋":
                        Make_TekkinkeiLog(typename, id, tekkin_name, buzai, 1);
                        break;
                }
            }
            else if (!kei_str.Contains("D") && !kei_str.Contains("S") && !kei_str.Contains("R"))
            {
                Make_TekkinkeiLog(typename, id, tekkin_name, buzai, 2);
                kei_str = "";
            }
            else
            {
                int n = Get_Num(kei_str);
                if(n <= 0)
                {
                    Make_TekkinkeiLog(typename, id, tekkin_name, buzai, 2);
                    kei_str = "";
                }
            }
        }




        /// <summary> スラブの配筋を指定したindexに設定
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="Rsla"></param>
        /// <param name="ind"></param>
        /// <param name="D"></param>
        /// <param name="pitch"></param>
        private void Set_Slab_Bar_Arrangement(FloorType symbol, FamilyStructure.Slab Rsla, int[] ind, string[] D, double pitch)
        {
            for(int i =0; i < ind.Count(); i++)
            {
                SetParameter(symbol.LookupParameter(Rsla.D1[ind[i]]), D[0]);
                SetParameter(symbol.LookupParameter(Rsla.D2[ind[i]]), D[1]);
                SetParameter(symbol.LookupParameter(Rsla.pitch[ind[i]]), pitch, true);
            }
        }


        /// <summary>節点群から1層に載っているものを選択し、X（またはY）の小さい順に並べ替える
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="nodeL"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        private List<STBclass.StbNodeid> Narabekae_Node(STBclass stb, List<STBclass.StbNodeid> nodeL, string way)
        {
            List<STBclass.StbNodeid> newL = new List<STBclass.StbNodeid>(); //条件に合致する節点リスト

            for (int s = 0; s < stb.StbModel.StbStories.Count(); s++)
            {
                if (newL.Count() > 0) break;
                for (int i = 0; i < nodeL.Count(); i++)
                {
                    bool flg = false; //1層目の節点→true

                    for (int l = 0; l < stb.StbModel.StbStories[s].StbNodeid_List.Count(); l++)
                    {
                        if (stb.StbModel.StbStories[s].StbNodeid_List[l].id == nodeL[i].id)
                        {
                            flg = true;
                            break;
                        }
                    }
                    if (!flg) continue;


                    if (newL.Count() == 0)
                    {
                        newL.Add(nodeL[i]);
                    }
                    else
                    {
                        double x = 0, y = 0, z = 0;             //節点群のx,y,z
                        double newx0 = 0, newy0 = 0, newz0 = 0; //条件に合致する節点リストの1つめのx,y,zの値
                        Get_Node_Position(stb, newL[0].id, ref newx0, ref newy0, ref newz0);
                        Get_Node_Position(stb, nodeL[i].id, ref x, ref y, ref z);
                        //①条件に合致する節点リストの1つ目よりxまたはyが小さい→節点リストの1番目に追加
                        if (way == "Y")
                        {
                            if(x == newx0)
                            { continue; }
                            else if (x < newx0)
                            {
                                newL.Insert(0, nodeL[i]);
                                continue;
                            }
                        }
                        else
                        {
                            if (y == newy0)
                            { continue; }
                            else if (y < newy0)
                            {
                                newL.Insert(0, nodeL[i]);
                                continue;
                            }
                        }

                        //①以外の時
                        bool addflg = false;
                        for (int j = 1; j < newL.Count(); j++)
                        {
                            if (addflg) break;
                            double newx1 = 0, newy1 = 0, newz1 = 0, newx2 = 0, newy2 = 0, newz2 = 0;


                            Get_Node_Position(stb, newL[j - 1].id, ref newx1, ref newy1, ref newz1);
                            Get_Node_Position(stb, newL[j].id, ref newx2, ref newy2, ref newz2);

                            if (way == "Y")
                            {
                                if(x == newx2 || x == newx1)
                                {
                                    addflg = true;
                                    break;
                                }
                                else if (x < newx2 && newx1 < x)
                                {
                                    newL.Insert(j, nodeL[i]);
                                    addflg = true;
                                    break;
                                }
                            }
                            else if (way == "X")
                            {
                                if(y == newy2 || y == newy1)
                                {
                                    addflg = true;
                                    break;
                                }
                                else if (y < newy2 && newy1 < y)
                                {
                                    newL.Insert(j, nodeL[i]);
                                    addflg = true;
                                    break;
                                }
                            }
                        }
                        if (!addflg)
                        {
                            newL.Add(nodeL[i]);
                        }
                    }
                }
            }
            return newL;
        }

        /// <summary>STB内での座標を取得(double x, double y, double z)
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void Get_Node_Position(STBclass stb, int id, ref double x, ref double y, ref double z)
        {
            for (int i = 0; i < stb.StbModel.StbNodes.Count();i++)
            {
                STBclass.StbModelClass.StbNode node = stb.StbModel.StbNodes[i];
                if(node.id == id)
                {
                    x = node.x;
                    y = node.y;
                    z = node.z;
                    break;
                }
            }
        }

        /// <summary>Revit内での座標を取得(XYのみ)※軸の作成に使用
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="alloffsetx">基点位置offset_X</param>
        /// <param name="alloffsety">基点位置offset_Y</param>
        /// <returns></returns>
        private XYZ Get_Node_Position(STBclass stb, int id, double alloffsetx, double alloffsety)
        {
            XYZ position = new XYZ();
            for(int i= 0; i < stb.StbModel.StbNodes.Count(); i++)
            {
                STBclass.StbModelClass.StbNode node = stb.StbModel.StbNodes[i];
                if (id == node.id)
                {
                    position = new XYZ(Commons.mm2ft(node.x + alloffsetx), Commons.mm2ft(node.y + alloffsety), 0);
                    break;
                }
            }
            return position;
        }

        /// <summary> Revit内での座標を取得(XYZ)
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="offsetx">部材のX方向offset（全体座標系）</param>
        /// <param name="offsety">部材のY方向offset（全体座標系）</param>
        /// <param name="offsetz">部材のZ方向offset（全体座標系）</param>
        /// <returns></returns>
        private XYZ Get_Node_Position(STBclass stb, int id, double offsetx, double offsety, double offsetz)
        {
            XYZ position = new XYZ();
            for (int i = 0; i < stb.StbModel.StbNodes.Count(); i++)
            {
                STBclass.StbModelClass.StbNode node = stb.StbModel.StbNodes[i];
                if (id == node.id)
                {
                    position = new XYZ(Commons.mm2ft(node.x + offsetx + alloffsetX), Commons.mm2ft(node.y + offsety+ alloffsetY), Commons.mm2ft(node.z + offsetz));
                    break;
                }
            }
            return position;
        }

        /// <summary>柱断面のindexを取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="kind_structure"></param>
        /// <returns></returns>
        private int Get_SectionColumn(STBclass stb, int id, string kind_structure)
        {
            int ind = 0;

            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            switch(kind_structure)
            {
                case "RC":
                    for(int i = 0; i < section.StbSecColumns_RC.Count(); i++)
                    {
                        if(section.StbSecColumns_RC[i].id == id)
                        {
                            ind = i;
                            break;
                        }
                    }
                    break;
                case "S":
                    for (int i = 0; i < section.StbSecColumns_S.Count(); i++)
                    {
                        if (section.StbSecColumns_S[i].id == id)
                        {
                            ind = i;
                            break;
                        }
                    }
                    break;
                case "SRC":
                    for (int i = 0; i < section.StbSecColumns_SRC.Count(); i++)
                    {
                        if (section.StbSecColumns_SRC[i].id == id)
                        {
                            ind = i;
                            break;
                        }
                    }
                    break;
                case "CFT":
                    for (int i = 0; i < section.StbSecColumns_CFT.Count(); i++)
                    {
                        if (section.StbSecColumns_CFT[i].id == id)
                        {
                            ind = i;
                            break;
                        }
                    }
                    break;

            }

            return ind;
        }
        /// <summary>梁断面のindexを取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="kind_structure"></param>
        /// <returns></returns>
        private int Get_SectionGirder(STBclass stb, int id, string kind_structure, ref bool isCanti)
        {
            int ind = 0;

            STBclass.StbModelClass.StbSectionsClass section = stb.StbModel.StbSections;
            switch (kind_structure)
            {
                case "RC":
                    for (int i = 0; i < section.StbSecBeams_RC.Count(); i++)
                    {
                        if (section.StbSecBeams_RC[i].id == id)
                        {
                            ind = i;
                            isCanti = section.StbSecBeams_RC[i].isCanti;
                            break;
                        }
                    }
                    break;
                case "S":
                    for (int i = 0; i < section.StbSecBeams_S.Count(); i++)
                    {
                        if (section.StbSecBeams_S[i].id == id)
                        {
                            ind = i;
                            isCanti = section.StbSecBeams_S[i].isCanti;
                            break;
                        }
                    }
                    break;
                case "SRC":
                    for (int i = 0; i < section.StbSecBeams_SRC.Count(); i++)
                    {
                        if (section.StbSecBeams_SRC[i].id == id)
                        {
                            ind = i;
                            isCanti = section.StbSecBeams_SRC[i].isCanti;
                            break;
                        }
                    }
                    break;

            }

            return ind;
        }
    　　/// <summary>STBの層のindexを層名から取得
     　 /// </summary>
     　 /// <param name="stb"></param>
     　 /// <param name="floor"></param>
     　 /// <returns></returns>
        private int Get_stbFloor_index(STBclass stb, string floor)
        {
            int ret = -1;
            if (floor == "") return ret;

            for(int i = 0; i < stb.StbModel.StbStories.Count(); i++)
            {
                if(stb.StbModel.StbStories[i].name == floor)
                {
                    ret = i;
                    break;
                }
            }

            return ret;
        }
        private int Get_stbFloor_index_Clm(STBclass stb, int sec_id)
        {
            int ret = -1;

            if(stb.StbModel.StbMembers.StbColumns != null)
            {
                foreach(STBclass.StbModelClass.StbMembersClass.StbColumn clm in stb.StbModel.StbMembers.StbColumns)
                {
                    if(clm.id_section == sec_id)
                    {
                        ret = Get_stbFloor_index(stb, clm.idNode_bottom);
                        break;
                    }
                }
            }
            if(ret == -1)
            {
                if(stb.StbModel.StbMembers.StbPosts != null)
                {
                    foreach (STBclass.StbModelClass.StbMembersClass.StbPost clm in stb.StbModel.StbMembers.StbPosts)
                    {
                        if (clm.id_section == sec_id)
                        {
                            ret = Get_stbFloor_index(stb, clm.idNode_bottom);
                            break;
                        }
                    }
                }
            }

            return ret;
        }
        private int Get_stbFloor_index_Gir(STBclass stb, int id_section)
        {
            int ret = -1;

            if(stb.StbModel.StbMembers.StbGirders != null)
            {
                foreach(STBclass.StbModelClass.StbMembersClass.StbGirder gir in stb.StbModel.StbMembers.StbGirders)
                {
                    if(gir.id_section == id_section)
                    {
                        ret = Get_stbFloor_index(stb, gir.idNode_start);
                        break;
                    }
                }
            }
            if(ret == -1)
            {
                if (stb.StbModel.StbMembers.StbBeams != null)
                {
                    foreach (STBclass.StbModelClass.StbMembersClass.StbBeam gir in stb.StbModel.StbMembers.StbBeams)
                    {
                        if (gir.id_section == id_section)
                        {
                            ret = Get_stbFloor_index(stb, gir.idNode_start);
                            break;
                        }
                    }
                }
            }

            return ret;
        }
        /// <summary>STBの層のindexを節点idから取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private int Get_stbFloor_index(STBclass stb, int id, bool flg = true)
        {
            int ret = 0;
            if (!flg) { ret = -1; }
            bool getflg = false;
            for (int i = 0; i < stb.StbModel.StbStories.Count(); i++)
            {
                if (getflg) break;
                for (int j = 0; j < stb.StbModel.StbStories[i].StbNodeid_List.Count(); j++)
                {
                    if(id == stb.StbModel.StbStories[i].StbNodeid_List[j].id)
                    {
                        ret = i;
                        getflg = true;
                        break;
                    }
                }
            }
            return ret;
        }
        /// <summary>STBの所属層からRevitでの所属層を取得
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        private Level SearchLevel(STBclass stb, int ind)
        {
            Level lv = null;
            double offset = 0;

            for(int i =0; i < alloffsetZ.Count();i++)
            {
                if(alloffsetZ[i].stbid == ind)
                {
                    offset = alloffsetZ[i].offset;
                    break;
                }
            }
            if(ind == -1) { return lv; }
            double interval = 0;
            for (int i = 0; i < Levels.Count(); i++)
            {
                double mm2ft = Commons.mm2ft(stb.StbModel.StbStories[ind].height + offset);                
                if(Math.Abs(mm2ft - Levels[i].Elevation) < 1)
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
                        if(interval > sa)
                        {
                            interval = sa;
                            lv = Levels[i];
                        }
                    }
                    
                }

            }

            return lv;
        }

        private Level SearchLevel_height(STBclass stb, int id_start, int id_end)
        {
            Level lv = null;

            //中点を求める
            double xs = 0, ys = 0, zs = 0, xe = 0, ye = 0, ze = 0;
            Get_Node_Position(stb, id_start, ref xs, ref ys, ref zs);
            Get_Node_Position(stb, id_end, ref xe, ref ye, ref ze);

            double zc = 0; //中点座標（フィート）
            zc = Commons.mm2ft((zs + ze) / 2);

            for(int i = 0; i < Levels.Count() - 1; i++) //Levelsは高い方から順に入っている
            {
                if(Levels[i].Elevation > zc && Levels[i+1].Elevation <= zc)
                {
                    lv = Levels[i + 1];
                    break;
                }
            }
            if(lv == null)
            {
                lv = Levels[0];
            }
            return lv;
        }

        /// <summary>梁の幅
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="s_id"></param>
        /// <param name="e_id"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private double Get_Girder_B(STBclass stb, int s_id, int e_id, ref ElementId id, bool flg = true)
        {
            double ret = 0;

            int id_section = -1;
            int id_member = -1;
            string kind_structure = "";
            bool isCanti = false;

            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];

                    if (gir.idNode_start == s_id && gir.idNode_end == e_id)
                    {
                        id_section = gir.id_section;
                        kind_structure = gir.kind_structure;
                        id_member = gir.id;
                        if (flg)
                        { Get_Girder_insElementId(stb, id_member); }
                        break;
                    }
                }
            }
            if (stb.StbModel.StbMembers.StbBeams != null && id_section == -1)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbBeams.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbBeam gir = stb.StbModel.StbMembers.StbBeams[i];

                    if (gir.idNode_start == s_id && gir.idNode_end == e_id)
                    {
                        id_section = gir.id_section;
                        kind_structure = gir.kind_structure;
                        id_member = gir.id;
                        if (flg)
                        { Get_Girder_insElementId(stb, id_member); }
                        break;
                    }
                }
            }


            int girind = Get_SectionGirder(stb, id_section, kind_structure, ref isCanti);
            switch (kind_structure)
            {
                case "RC":
                    switch (stb.StbModel.StbSections.StbSecBeams_RC[girind].StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            ret = stb.StbModel.StbSections.StbSecBeams_RC[girind].StbSecFigure.StbSecStraight.width;
                            break;
                        case 2:
                            ret = stb.StbModel.StbSections.StbSecBeams_RC[girind].StbSecFigure.StbSecTaper.width_start;
                            break;
                        case 3:
                            ret = stb.StbModel.StbSections.StbSecBeams_RC[girind].StbSecFigure.StbSecHaunch.width_center;
                            break;
                    }
                    break;
                case "S":
                    int sind = 0;
                    string shape = "";
                    STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir = stb.StbModel.StbSections.StbSecBeams_S[girind];
                    if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                    { shape = Check_Steel(stb, stb.StbModel.StbSections.StbSecBeams_S[girind].StbSecSteelBeam[1].shape, ref sind); }
                    if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                    { shape = Check_Steel(stb, gir.StbSecSteelBeam[0].shape, ref sind); }
                    else if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                    { shape = Check_Steel(stb, gir.StbSecSteelBeam[2].shape, ref sind); }

                    STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class steel = stb.StbModel.StbSections.StbSecSteel;
                    switch (shape)
                    {
                        case RevitLNK.st_steel_H:
                            ret = steel.StbSecRoll_H[sind].B;
                            break;
                        case RevitLNK.st_steel_BH:
                            ret = steel.StbSecBuild_H[sind].B;
                            break;
                        case RevitLNK.st_steel_C:
                            ret = steel.StbSecRoll_C[sind].B;
                            break;
                        case RevitLNK.st_steel_L:
                            ret = steel.StbSecRoll_L[sind].B;
                            break;
                        case RevitLNK.st_steel_LipC:
                            ret = steel.StbSecRoll_LipC[sind].A;
                            break;
                    }
                    break;
                case "SRC":
                    switch (stb.StbModel.StbSections.StbSecBeams_SRC[girind].StbSecFigure.StbSecFigureType)
                    {
                        case 1:
                            ret = stb.StbModel.StbSections.StbSecBeams_SRC[girind].StbSecFigure.StbSecStraight.width;
                            break;
                        case 2:
                            ret = stb.StbModel.StbSections.StbSecBeams_SRC[girind].StbSecFigure.StbSecTaper.width_start;
                            break;
                        case 3:
                            ret = stb.StbModel.StbSections.StbSecBeams_SRC[girind].StbSecFigure.StbSecHaunch.width_center;
                            break;
                    }
                    break;
            }

            return ret;
        }

        /// <summary> 梁の成
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id"></param>
        /// <param name="kind"></param>
        /// <param name="start_end"></param>
        /// <returns></returns>
        private double Get_Girder_depth(STBclass stb, int id, string kind, string start_end)
        {
            double depth = 0;
            switch(kind)
            {
                case "RC":
                    for(int i =0;i<stb.StbModel.StbSections.StbSecBeams_RC.Count(); i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_RC gir = stb.StbModel.StbSections.StbSecBeams_RC[i];
                        if(gir.id != id) { continue; }
                        switch(gir.StbSecFigure.StbSecFigureType)
                        {
                            case 1:
                                depth = gir.StbSecFigure.StbSecStraight.depth;
                                break;
                            case 2:
                                if (start_end == "start")
                                { depth = gir.StbSecFigure.StbSecTaper.depth_start; }
                                else if (start_end == "center")
                                { depth = (gir.StbSecFigure.StbSecTaper.depth_start + gir.StbSecFigure.StbSecTaper.depth_end) / 2; }
                                else if (start_end == "end")
                                { depth = gir.StbSecFigure.StbSecTaper.depth_end; } 
                                break;
                            case 3:
                                if (start_end == "start")
                                { depth = gir.StbSecFigure.StbSecHaunch.depth_start; }
                                else if(start_end == "center")
                                { depth = gir.StbSecFigure.StbSecHaunch.depth_center; }
                                else if(start_end == "end")
                                { depth = gir.StbSecFigure.StbSecHaunch.depth_end; }
                                break;
                        }
                        break;
                    }
                    break;
                case "S":
                    for (int i = 0; i < stb.StbModel.StbSections.StbSecBeams_S.Count(); i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_S gir = stb.StbModel.StbSections.StbSecBeams_S[i];
                        if(gir.id != id) { continue; }
                        int sind = 0;
                        string shape = "";
                        if(start_end == "start")
                        {
                            if(gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[0].shape, ref sind); }
                            else if(gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[1].shape, ref sind); }
                            else if(gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[2].shape, ref sind); }
                        }
                        else
                        {
                            if (gir.StbSecSteelBeam[2] != null && gir.StbSecSteelBeam[2].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[2].shape, ref sind); }
                            else if (gir.StbSecSteelBeam[1] != null && gir.StbSecSteelBeam[1].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[1].shape, ref sind); }
                            else if (gir.StbSecSteelBeam[0] != null && gir.StbSecSteelBeam[0].shape != "")
                            { shape = Check_Steel(stb, gir.StbSecSteelBeam[0].shape, ref sind); }
                        }
                        STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class steel = stb.StbModel.StbSections.StbSecSteel;
                        switch(shape)
                        {                            
                            case RevitLNK.st_steel_H:
                                depth = steel.StbSecRoll_H[sind].A;
                                break;
                            case RevitLNK.st_steel_BH:
                                depth = steel.StbSecBuild_H[sind].A;
                                break;
                            case RevitLNK.st_steel_C:
                                depth = steel.StbSecRoll_C[sind].A;
                                break;
                            case RevitLNK.st_steel_L:
                                depth = steel.StbSecRoll_L[sind].A;
                                break;
                            case RevitLNK.st_steel_LipC:
                                depth = steel.StbSecRoll_LipC[sind].H;
                                break;
                        }
                        break;
                    }
                    break;
                case "SRC":
                    for (int i = 0; i < stb.StbModel.StbSections.StbSecBeams_SRC.Count(); i++)
                    {
                        STBclass.StbModelClass.StbSectionsClass.StbSecBeam_SRC gir = stb.StbModel.StbSections.StbSecBeams_SRC[i];
                        if (gir.id != id) { continue; }
                        switch (gir.StbSecFigure.StbSecFigureType)
                        {
                            case 1:
                                depth = gir.StbSecFigure.StbSecStraight.depth;
                                break;
                            case 2:
                                if (start_end == "start")
                                { depth = gir.StbSecFigure.StbSecTaper.depth_start; }
                                else
                                { depth = gir.StbSecFigure.StbSecTaper.depth_end; }
                                break;
                            case 3:
                                if (start_end == "start")
                                { depth = gir.StbSecFigure.StbSecHaunch.depth_start; }
                                else
                                { depth = gir.StbSecFigure.StbSecHaunch.depth_end; }
                                break;
                        }
                        break;
                    }
                    break;
            }
            return depth;
        }
       
        private double Get_Foundation_depth(STBclass stb, int id)
        {
            double depth = 0;
            if (stb.StbModel.StbSections.StbSecFoundations_RC != null)
            {
                for (int j = 0; j < stb.StbModel.StbSections.StbSecFoundations_RC.Count(); j++)
                {
                    STBclass.StbModelClass.StbSectionsClass.StbSecFoundation_RC fo = stb.StbModel.StbSections.StbSecFoundations_RC[j];
                    if (fo.id != id) { continue; }
                    if (fo.StbSecFigure != null)
                    {
                        switch (fo.StbSecFigure.StbSecFigureType)
                        {
                            case 1:
                                depth = fo.StbSecFigure.StbSecRect.depth;
                                break;
                            case 2:
                                depth = fo.StbSecFigure.StbSecTapered_Rect.depth_base;
                                break;
                            case 3:
                                depth = fo.StbSecFigure.StbSecTriangle.depth;
                                break;
                            case 4:
                                depth = fo.StbSecFigure.StbSecEqiTriangle.depth;
                                break;
                            case 5:
                                depth = fo.StbSecFigure.StbSecOctagon.depth;
                                break;
                        }
                        break;
                    }
                }
            }
            return depth;
        }
        /// <summary>梁インスタンスファミリのElementId
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id_section"></param>
        /// <returns></returns>
        private ElementId Get_Girder_insElementId(STBclass stb, int id_section)
        {
            ElementId ret = null;

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            IList<Element> elements = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            if (elements == null || elements.Count() == 0)
            {
                //ファミリが無いログ
                return ret;
            }
            List<FamilyInstance> symbols = new List<FamilyInstance>();

            foreach (Element el in elements)
            {
                FamilyInstance ins = el as FamilyInstance;
                if (ins == null) { continue; }
                StructuralInstanceUsage usage = (StructuralInstanceUsage)(ins.get_Parameter(BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM).AsInteger());
                if (usage == StructuralInstanceUsage.Girder || usage == StructuralInstanceUsage.Joist)
                { symbols.Add(ins); }
            }

            for(int i =0; i < symbols.Count(); i++)
            {
                Parameter para = symbols[i].LookupParameter("配置ID");
                if(para == null) { continue; }
                if(id_section == para.AsInteger())
                {
                    ret = para.Id;
                    break;
                }
            }

            return ret;

        }

        /// <summary> ハンチ長・ハンチ種類を配置から取得
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="id_section"></param>
        /// <param name="haunch_start"></param>
        /// <param name="haunch_end"></param>
        /// <param name="kind_haunch_start"></param>
        /// <param name="kind_haunch_end"></param>
        private void Get_Haunch(STBclass stb, int id_section, ref List<double> haunch_start, ref List<double> haunch_end, ref List<string> kind_haunch_start, ref List<string> kind_haunch_end)
        {
            if (stb.StbModel.StbMembers.StbBeams != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbBeams.Count(); i++)
                {
                    if (stb.StbModel.StbMembers.StbBeams[i].id_section == id_section)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbBeam beam = stb.StbModel.StbMembers.StbBeams[i];
                        bool sameflg = false;
                        for (int j = 0; j < haunch_start.Count(); j++)
                        {
                            if (haunch_start[j] == beam.haunch_start && haunch_end[j] == beam.haunch_end &&
                                kind_haunch_start[j] == beam.kind_haunch_start && kind_haunch_end[j] == beam.kind_haunch_end)
                            {
                                sameflg = true;
                                break;
                            }
                        }
                        if (!sameflg)
                        {
                            haunch_start.Add(beam.haunch_start);
                            haunch_end.Add(beam.haunch_end);
                            kind_haunch_start.Add(beam.kind_haunch_start);
                            kind_haunch_end.Add(beam.kind_haunch_end);
                        }
                    }
                }
            }
            if (stb.StbModel.StbMembers.StbGirders == null) { return; }
            for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
            {
                if (stb.StbModel.StbMembers.StbGirders[i].id_section == id_section)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder sgir = stb.StbModel.StbMembers.StbGirders[i];
                    bool sameflg = false;
                    for (int j = 0; j < haunch_start.Count(); j++)
                    {
                        if (haunch_start[j] == sgir.haunch_start && haunch_end[j] == sgir.haunch_end &&
                             kind_haunch_start[j] == sgir.kind_haunch_start && kind_haunch_end[j] == sgir.kind_haunch_end)
                        {
                            sameflg = true;
                            break;
                        }
                    }
                    if (!sameflg)
                    {
                        haunch_start.Add(sgir.haunch_start);
                        haunch_end.Add(sgir.haunch_end);
                        kind_haunch_start.Add(sgir.kind_haunch_start);
                        kind_haunch_end.Add(sgir.kind_haunch_end);
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
        private double Get_Joint(STBclass stb, double j_stb, XYZ pos, XYZ Ps, XYZ Pe, int idnode)
        {
            if (j_stb == 0) { return 0; }

            //柱のオフセット
            XYZ clm_offset = new XYZ();
            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                var clm = stb.StbModel.StbMembers.StbColumns.Find(c => c.idNode_top == idnode);
                if (clm != null)
                {
                    if (clm.offset_top_X != 0 || clm.offset_top_Y != 0)
                    {
                        clm_offset = new XYZ(Commons.mm2ft(clm.offset_top_X), Commons.mm2ft(clm.offset_top_Y), 0);
                    }
                    else
                    {
                        clm_offset = new XYZ(Commons.mm2ft(clm.offset_X), Commons.mm2ft(clm.offset_Y), 0);
                    }
                }
                else
                {
                    clm = stb.StbModel.StbMembers.StbColumns.Find(c => c.idNode_bottom == idnode);
                    if (clm != null)
                    {
                        if (clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0)
                        {
                            clm_offset = new XYZ(Commons.mm2ft(clm.offset_bottom_X), Commons.mm2ft(clm.offset_bottom_Y), 0);
                        }
                        else
                        {
                            clm_offset = new XYZ(Commons.mm2ft(clm.offset_X), Commons.mm2ft(clm.offset_Y), 0);
                        }
                    }
                }
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



        /// <summary>同じ座標があるか（値が同じでもnodeIdが違う節点があった時用）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private bool Node_Check(STBclass stb, int node1, int node2)
        {
            bool ret = false;
            if(node1 == node2)
            {
                ret = true;
                return ret;
            }
            int node1_sub = -1, node2_sub = -1;
            for(int i = 0; i <stb.StbModel.StbNodes.Count(); i++)
            {
                if(node1_sub != -1 && node2_sub != -1) { break; }
                if(stb.StbModel.StbNodes[i].id == node1)
                {
                    node1_sub = stb.StbModel.StbNodes[i].sub_id;
                }
                else if(stb.StbModel.StbNodes[i].id == node2)
                {
                    node2_sub = stb.StbModel.StbNodes[i].sub_id;
                }
            }
            if(node1 == node2 || node1_sub == node2 || node1 == node2_sub || node1_sub == node2_sub)
            { ret = true; }
            return ret;
        }

        /// <summary> 柱脚の伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="offset_Z"></param>
        private void Search_Girder_Offset_Z_bottom(STBclass stb, int node, Level btmlevel, string kind, ref double offset_Z)
        {
            bool getflg = false;
            //共有する節点があるとき
            //基礎
            if (stb.StbModel.StbMembers.StbFootings != null)
            {
                double depth = 0;
                for (int i = 0; i < stb.StbModel.StbMembers.StbFootings.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbFooting footing = stb.StbModel.StbMembers.StbFootings[i];
                    if (Node_Check(stb, footing.idNode, node))
                    {
                        getflg = true;
                        double level_offset = Commons.ft2mm(Get_Node_Position(stb, footing.idNode, 0, 0, 0).Z);
                        depth = Get_Foundation_depth(stb, footing.id_section);
                        //梁よりも基礎のオフセット値が優先度高い
                        offset_Z = footing.level_bottom + depth;
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
                    STBclass.StbModelClass.StbMembersClass.StbColumn clm = stb.StbModel.StbMembers.StbColumns[i];
                    if (Node_Check(stb, node, clm.idNode_bottom) || Node_Check(stb, node, clm.idNode_top)) { clmnum++; }
                }
            }
            if (stb.StbModel.StbMembers.StbPosts != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbPosts.Count(); i++)
                {
                    if (clmnum > 1) { break; }
                    STBclass.StbModelClass.StbMembersClass.StbPost clm = stb.StbModel.StbMembers.StbPosts[i];
                    if (Node_Check(stb, node, clm.idNode_bottom) || Node_Check(stb, node, clm.idNode_top)) { clmnum++; }
                }
            }
            if (clmnum < 2 && kind != "S") { btmflg = true; }
            if (!getflg)
            {
                //節点を共有する大梁を探す            
                if (stb.StbModel.StbMembers.StbGirders != null)
                {
                    double depth = 0;
                    for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                        bool osflg = false;
                        if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_start_Z != 0 ||
                           gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0) { osflg = true; }
                        XYZ vecU = new XYZ();
                        XYZ vecV = new XYZ();
                        XYZ vecW = new XYZ();
                        if (!osflg)
                        {
                            XYZ Ps = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                            XYZ Pe = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                            vecU = (Pe - Ps).Normalize();
                            Get_Vector(vecU, gir.rotate, ref vecV, ref vecW);
                        }
                        if (Node_Check(stb, gir.idNode_start, node))
                        {
                            if (btmflg)
                            { depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "start"); }

                            if (!getflg)
                            {
                                if (osflg)
                                { offset_Z = gir.offset_start_Z + depth; }
                                else
                                {
                                    offset_Z = gir.level * vecW.Z + depth;
                                }
                                getflg = true;
                            }
                            else
                            {
                                double _offset = 0;
                                if (osflg)
                                {
                                    _offset = gir.offset_start_Z + depth;
                                }
                                else
                                {
                                    _offset = gir.level * vecW.Z + depth;
                                }
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
                        else if (Node_Check(stb, gir.idNode_end, node))
                        {
                            if (btmflg)
                            { depth = -Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "end"); }

                            if (!getflg)
                            {
                                if (osflg)
                                { offset_Z = gir.offset_end_Z + depth; }
                                else
                                {
                                    offset_Z = gir.level * vecW.Z + depth;
                                }
                                getflg = true;
                            }
                            else
                            {
                                double _offset = 0;
                                if (osflg)
                                {
                                    _offset = gir.offset_end_Z + depth;
                                }
                                else
                                {
                                    _offset = gir.level * vecW.Z + depth;
                                }
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
            if(!getflg)
            {
                //節点を共有する大梁が無い→大梁に乗っていたらその大梁のレベルに合わせる
                if (stb.StbModel.StbMembers.StbGirders != null)
                {
                    double depth = 0;
                    for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                    {
                        STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                        int floor = Get_stbFloor_index(stb, gir.idNode_start);
                        Level lv = SearchLevel(stb, floor);
                        if (lv == null) { lv = SearchLevel_height(stb, gir.idNode_start, gir.idNode_end); }

                        if(btmlevel != lv) { continue; } //同じレベルの梁だけチェックする
                       

                        XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                        XYZ P = Get_Node_Position(stb, node, 0, 0, 0);

                        
                        if(Ps_gir.X > P.X || P.X > Pe_gir.X || Ps_gir.Y > P.Y || P.Y > Pe_gir.Y) { continue; }

                        XYZ vec1 = Pe_gir - Ps_gir;
                        XYZ vec2 = P - Ps_gir;
                        double length = Commons.LinePointDist(vec1, vec2);
                        if (length < Commons.mm2ft(50))
                        {
                            bool osflg = false;
                            if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_start_Z != 0 ||
                               gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0) { osflg = true; }

                            XYZ vecV = null;
                            XYZ vecW = null;
                            if (!osflg) { Get_Vector(vec1.Normalize(), gir.rotate, ref vecV, ref vecW); }
                            if (btmflg)
                            {
                                depth = (-Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "start") -
                                          Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "end")) / 2;
                            }

                            double _offset = 0;
                            if (osflg)
                            {
                                _offset = (gir.offset_start_Z + gir.offset_end_Z) + depth;
                            }
                            else
                            {
                                _offset = gir.level * vecW.Z + depth;
                            }
                            if (!getflg)
                            {
                                offset_Z = _offset + length;
                                getflg = true;
                            }
                            else
                            {
                                if (btmflg)
                                {
                                    if (offset_Z > _offset)
                                    { offset_Z = _offset + length; }
                                }
                                else
                                {
                                    if (offset_Z < _offset)
                                    { offset_Z = _offset + length; }
                                }
                            }
                        }
                        
                    }
                }
                        
            }
        }
        /// <summary> 柱頭の伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="offset_Z"></param>
        private void Search_Girder_Offset_Z_top(STBclass stb, int node, Level toplevel, ref double offset_Z)
        {
            int ind = Get_stbFloor_index(stb, node);
            //大梁
            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                bool getflg = false;
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                    bool offset_flg = true; //false⇒levelを使う
                    if (gir.offset_start_Z == 0 && gir.offset_end_Z == 0) { offset_flg = false; }
                    if (Node_Check(stb, gir.idNode_start, node))
                    {
                        if (!getflg)
                        {
                            if (!offset_flg)
                            {
                                offset_Z = gir.level;
                            }
                            else
                            {
                                offset_Z = gir.offset_start_Z;
                            }
                            getflg = true;
                        }
                        else
                        {
                            if (!offset_flg)
                            {
                                if (offset_Z < gir.level)
                                {
                                    offset_Z = gir.level;
                                }
                            }
                            else
                            {
                                if (offset_Z < gir.offset_start_Z)
                                {
                                    offset_Z = gir.offset_start_Z;
                                }
                            }
                        }
                    }
                    if (Node_Check(stb, gir.idNode_end, node))
                    {
                        if (!getflg)
                        {
                            if (!offset_flg)
                            {
                                offset_Z = gir.level;
                            }
                            else
                            {
                                offset_Z = gir.offset_end_Z;
                            }
                            getflg = true;
                        }
                        else
                        {
                            if (!offset_flg)
                            {
                                if (offset_Z < gir.level)
                                {
                                    offset_Z = gir.level;
                                }
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
                }

                if (!getflg)
                {
                    //節点を共有する大梁が無い→大梁に乗っていたらその大梁のレベルに合わせる
                    if (stb.StbModel.StbMembers.StbGirders != null)
                    {
                        double depth = 0;
                        for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                        {
                            STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                            int floor = Get_stbFloor_index(stb, gir.idNode_start);
                            Level lv = SearchLevel(stb, floor);
                            if (lv == null) { lv = SearchLevel_height(stb, gir.idNode_start, gir.idNode_end); }

                            if (toplevel != lv) { continue; } //同じレベルの梁だけチェックする


                            XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                            XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                            XYZ P = Get_Node_Position(stb, node, 0, 0, 0);


                            if (Ps_gir.X > P.X || P.X > Pe_gir.X || Ps_gir.Y > P.Y || P.Y > Pe_gir.Y) { continue; }

                            XYZ vec1 = Pe_gir - Ps_gir;
                            XYZ vec2 = P - Ps_gir;
                            double length = Commons.LinePointDist(vec1, vec2);
                            if (length < Commons.mm2ft(50))
                            {
                                bool osflg = false;
                                if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_start_Z != 0 ||
                                   gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0) { osflg = true; }

                                XYZ vecV = null;
                                XYZ vecW = null;
                                if (!osflg) { Get_Vector(vec1.Normalize(), gir.rotate, ref vecV, ref vecW); }
                                
                                    depth = (-Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "start") -
                                              Get_Girder_depth(stb, gir.id_section, gir.kind_structure, "end")) / 2;
                                

                                double _offset = 0;
                                if (osflg)
                                {
                                    _offset = (gir.offset_start_Z + gir.offset_end_Z) + depth;
                                }
                                else
                                {
                                    _offset = gir.level * vecW.Z + depth;
                                }
                                if (!getflg)
                                {
                                    offset_Z = _offset + length;
                                    getflg = true;
                                }
                                else
                                {
                                    if (offset_Z < _offset)
                                    { offset_Z = _offset + length; }
                                }
                            }

                        }
                    }
                }
            }
        }
        /// <summary> ブレース伸縮用梁のオフセット値（全体座標系）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="ind"></param>
        /// <param name="offset_X"></param>
        /// <param name="offset_Y"></param>
        /// <param name="offset_Z"></param>
        /// <param name="start_end"></param>
        private void Search_Girder_Offset_XYZ(STBclass stb, int id, ref double offset_X, ref double offset_Y, ref double offset_Z, string start_end)
        {
            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                    if(gir.id != id) { continue; }
                    XYZ Ps = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                    XYZ Pe = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                    XYZ vecU = (Pe - Ps).Normalize();
                    XYZ vecV = new XYZ();
                    XYZ vecW = new XYZ();
                    Get_Vector(vecU, gir.rotate, ref vecV, ref vecW);
                    bool offsetflg = false;
                    if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_start_Z != 0 ||
                        gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0) { offsetflg = true; }
                    if(offsetflg)
                    {
                        if(start_end == "start")
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
                    }
                    else
                    {
                        offset_X = vecW.X * gir.level;
                        offset_Y = vecW.Y * gir.level;
                        offset_Z = vecW.Z * gir.level;
                    }                    
                }
            }
            if (stb.StbModel.StbMembers.StbBeams != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbBeams.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbBeam gir = stb.StbModel.StbMembers.StbBeams[i];
                    if(gir.id != id) { continue; }
                    XYZ Ps = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                    XYZ Pe = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                    XYZ vecU = (Pe - Ps).Normalize();
                    XYZ vecV = new XYZ();
                    XYZ vecW = new XYZ();
                    Get_Vector(vecU, gir.rotate, ref vecV, ref vecW);
                    bool offsetflg = false;
                    if (gir.offset_start_X != 0 || gir.offset_start_Y != 0 || gir.offset_start_Z != 0 ||
                        gir.offset_end_X != 0 || gir.offset_end_Y != 0 || gir.offset_end_Z != 0) { offsetflg = true; }
                    if (offsetflg)
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
                    }
                    else
                    {
                        offset_X = vecW.X * gir.level;
                        offset_Y = vecW.Y * gir.level;
                        offset_Z = vecW.Z * gir.level;
                    }
                }
            }
        }
        /// <summary> 柱のオフセット値（全体座標系）
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="offset_X"></param>
        /// <param name="offset_Y"></param>
        /// <param name="offset_Z"></param>
        private bool Search_Column_Offset_XYZ(STBclass stb, int node, ref double offset_X, ref double offset_Y, ref double offset_Z)
        {
            bool ret = false;
            if (stb.StbModel.StbMembers.StbColumns != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbColumns.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbColumn clm = stb.StbModel.StbMembers.StbColumns[i];
                    bool offset_flg = false; //true⇒端点のオフセットを使用
                    if(clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0 || 
                       clm.offset_top_X != 0 || clm.offset_top_Y != 0 ) { offset_flg = true; }
                    if (Node_Check(stb, clm.idNode_bottom, node))
                    {
                        if (offset_flg)
                        {
                            offset_X = clm.offset_bottom_X;
                            offset_Y = clm.offset_bottom_Y;
                        }
                        else
                        {
                            offset_X = clm.offset_X;
                            offset_Y = clm.offset_Y;
                        }
                        offset_Z = clm.offset_bottom_Z;
                        ret = true;
                        break;
                    }
                    if (Node_Check(stb, clm.idNode_top, node))
                    {
                        if (offset_flg)
                        {
                            offset_X = clm.offset_top_X;
                            offset_Y = clm.offset_top_Y;
                        }
                        else
                        {
                            offset_X = clm.offset_X;
                            offset_Y = clm.offset_Y;
                        }
                        offset_Z = clm.offset_top_Z;
                        ret = true;
                        break;
                    }
                }
            }
            if (stb.StbModel.StbMembers.StbPosts != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbPosts.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbPost clm = stb.StbModel.StbMembers.StbPosts[i];
                    bool offset_flg = false; //true⇒端点のオフセットを使用
                    if (clm.offset_bottom_X != 0 || clm.offset_bottom_Y != 0 ||
                        clm.offset_top_X != 0 || clm.offset_top_Y != 0) { offset_flg = true; }
                    if (Node_Check(stb, clm.idNode_bottom, node))
                    {
                        if (offset_flg)
                        {
                            offset_X = clm.offset_bottom_X;
                            offset_Y = clm.offset_bottom_Y;
                        }
                        else
                        {
                            offset_X = clm.offset_X;
                            offset_Y = clm.offset_Y;
                        }
                        offset_Z = clm.offset_bottom_Z;
                        ret = true;
                        break;
                    }
                    if (Node_Check(stb, clm.idNode_top, node))
                    {
                        if (offset_flg)
                        {
                            offset_X = clm.offset_top_X;
                            offset_Y = clm.offset_top_Y;
                        }
                        else
                        {
                            offset_X = clm.offset_X;
                            offset_Y = clm.offset_Y;
                        }
                        offset_Z = clm.offset_top_Z;
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }
        /// <summary>梁の伸縮
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
        private XYZ Search_Offset_gir(STBclass stb, int node, ref XYZ Ps, ref XYZ Pe, string start_end, XYZ vecU, int id, Level btmlevel, double rotate, out XYZ offset2)
        {
            XYZ offset = new XYZ(); //梁の方向をX軸としたSTBの部材座標系
            offset2 = new XYZ();

            double clm_offset_X = 0, clm_offset_Y = 0, clm_offset_Z = 0;
            //①節点を共有する柱がある
            if (Search_Column_Offset_XYZ(stb, node, ref clm_offset_X, ref clm_offset_Y, ref clm_offset_Z))
            {
                offset2 = new XYZ(clm_offset_X, clm_offset_Y, 0);
                offset = TransformCoord(Ps, Pe, clm_offset_X, clm_offset_Y, 0, rotate);
            }           
            else
            {
                //伸縮する梁の方向
                XYZ VecU = (Pe - Ps).Normalize();
                //②節点を共有する大梁があるとき
                bool getflg_1 = false;
                double B = 0;
                double gir_offset_X = 0, gir_offset_Y = 0;  //全体座標系
                for (int i =0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    if (getflg_1) { break; }
                   
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb, node, gir.idNode_start))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0);
                        XYZ VecU_gir = (Pe - Ps).Normalize();
                        if (vecU.X == VecU_gir.X && vecU.Y == VecU_gir.Y) { continue; } //XY平面で同じ方向なら無視する
                        if(Search_Girder_Samevec(stb, node, gir.id, VecU_gir)) { continue; } //同じ方向の梁があるときは考慮しない（伸縮する梁との交点がT字型のとき）
                        
                        if (gir.offset_start_X != 0 | gir.offset_start_Y != 0)
                        {
                            gir_offset_X = gir.offset_start_X;
                            gir_offset_Y = gir.offset_start_Y;
                        }
                        else
                        {
                            XYZ vecV_r = (BasisZ.CrossProduct(vecU)).Normalize();
                            XYZ vecV = new XYZ();
                            Commons.AxisRotate(vecV_r, vecU, new XYZ(), rotate, ref vecV);
                            gir_offset_X = gir.offset * vecV.X;
                            gir_offset_Y = gir.offset * vecV.Y;
                        }
                        getflg_1 = true;
                    }
                    else if(Node_Check(stb, node, gir.idNode_end))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0);
                        XYZ VecU_gir = (Pe - Ps).Normalize();
                        if (vecU.X == VecU_gir.X && vecU.Y == VecU_gir.Y) { continue; } //XY平面で同じ方向なら無視する
                        if (Search_Girder_Samevec(stb, node, gir.id, VecU_gir)) { continue; } //同じ方向の梁があるときは考慮しない（伸縮する梁との交点がT字型のとき）
                        if (gir.offset_end_X != 0 | gir.offset_end_Y != 0)
                        {
                            gir_offset_X = gir.offset_end_X;
                            gir_offset_Y = gir.offset_end_Y;
                        }
                        else
                        {
                            XYZ vecV_r = (BasisZ.CrossProduct(vecU)).Normalize();
                            XYZ vecV = new XYZ();
                            Commons.AxisRotate(vecV_r, vecU, new XYZ(), rotate, ref vecV);
                            gir_offset_X = gir.offset * vecV.X;
                            gir_offset_Y = gir.offset * vecV.Y;
                        }
                        getflg_1 = true;
                    }
                    if (getflg_1 && !Search_Girder_Samevec(stb, node, id, vecU))
                    {
                        ElementId eid = null;
                        B = Get_Girder_B(stb, gir.idNode_start, gir.idNode_end, ref eid);
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
                if(getflg_1)
                {
                    offset2 = new XYZ(gir_offset_X, gir_offset_Y, 0);
                    offset = TransformCoord(Ps, Pe, gir_offset_X, gir_offset_Y, 0, rotate);
                }
            }


            return offset;
        }
        /// <summary>VecU_girと同じ向きの梁がある→true
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="id"></param>
        /// <param name="vecU"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        private bool Search_Girder_Samevec(STBclass stb, int node, int id, XYZ vecU)
        {
            bool ret = false;
            if (stb.StbModel.StbMembers.StbGirders != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbGirder gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (gir.id == id) { continue; }
                    if (Node_Check(stb, gir.idNode_start, node) || Node_Check(stb, gir.idNode_end, node))
                    {
                        XYZ Ps = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                        XYZ Pe = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                        XYZ vec = (Pe - Ps).Normalize();
                        if (Math.Abs(vec.X) == Math.Abs(vecU.X) &&
                            Math.Abs(vec.Y) == Math.Abs(vecU.Y) &&
                            Math.Abs(vec.Z) == Math.Abs(vecU.Z))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            if (!ret && stb.StbModel.StbMembers.StbBeams != null)
            {
                for (int i = 0; i < stb.StbModel.StbMembers.StbBeams.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbBeam gir = stb.StbModel.StbMembers.StbBeams[i];
                    if (gir.id == id) { continue; }
                    if (Node_Check(stb, gir.idNode_start, node) || Node_Check(stb, gir.idNode_end, node))
                    {
                        XYZ Ps = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                        XYZ Pe = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                        XYZ vec = (Pe - Ps).Normalize();
                        if (Math.Abs(vec.X) == Math.Abs(vecU.X) &&
                            Math.Abs(vec.Y) == Math.Abs(vecU.Y) &&
                            Math.Abs(vec.Z) == Math.Abs(vecU.Z))
                        {
                            ret = true;
                            break;
                        }
                    }

                }
            }
            return ret;
        }
        private bool Search_Column(STBclass stb, int node)
        {
            bool ret = false;
            if(stb.StbModel.StbMembers.StbColumns != null)
            {
                XYZ P = Get_Node_Position(stb, node, 0, 0, 0);
                for(int i =0; i < stb.StbModel.StbMembers.StbColumns.Count(); i++)
                {
                    STBclass.StbModelClass.StbMembersClass.StbColumn clm = stb.StbModel.StbMembers.StbColumns[i];
                    
                    XYZ Pb = Get_Node_Position(stb, clm.idNode_bottom, 0, 0, 0);
                    XYZ Pt = Get_Node_Position(stb, clm.idNode_top, 0, 0, 0);
                    if(Pb.Z > P.Z || Pt.Z < P.Z) { continue; }
                    double kyori_x = Commons.LinePointDist(Pb.X, Pb.Z, Pt.X, Pt.Z, P.X, P.Z);
                    double kyori_y = Commons.LinePointDist(Pb.Y, Pb.Z, Pt.Y, Pt.Z, P.Y, P.Z);
                    if(Math.Abs(kyori_x) < Commons.mm2ft(50) && Math.Abs(kyori_y) < Commons.mm2ft(50))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }
        /// <summary> ブレースの伸縮
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="node"></param>
        /// <param name="Ps"></param>
        /// <param name="Pe"></param>
        /// <param name="start_end"></param>
        /// <param name="kind_brace"></param>
        /// <returns></returns>
        private XYZ Search_Offset_bra(STBclass stb, int node,  XYZ Ps,  XYZ Pe, string start_end, string kind_brace, double rotate)
        {
            XYZ offset = new XYZ();
            XYZ vecU = new XYZ();
            if(start_end == "start")
            { vecU = (Pe - Ps).Normalize(); }
            else
            { vecU = (Ps - Pe).Normalize(); }
            double gir_offset_X = 0, gir_offset_Y = 0, gir_offset_Z = 0;
            double clm_offset_X = 0, clm_offset_Y = 0, clm_offset_Z = 0;
            if (kind_brace == "VERTICAL")
            {
                double angle = 0;
                int id = 0;
                string s_e = "";
                STBclass.StbModelClass.StbMembersClass.StbGirder gir = null;
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb, gir.idNode_start, node) || Node_Check(stb, gir.idNode_end, node))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                        XYZ vecU_gir = new XYZ();
                        if (Node_Check(stb, gir.idNode_start, node))
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
                { Search_Girder_Offset_XYZ(stb, id, ref gir_offset_X, ref gir_offset_Y, ref gir_offset_Z, s_e); }
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
                STBclass.StbModelClass.StbMembersClass.StbGirder gir = null;
                for (int i = 0; i < stb.StbModel.StbMembers.StbGirders.Count(); i++)
                {
                    gir = stb.StbModel.StbMembers.StbGirders[i];
                    if (Node_Check(stb, gir.idNode_start, node) || Node_Check(stb, gir.idNode_end, node))
                    {
                        XYZ Ps_gir = Get_Node_Position(stb, gir.idNode_start, 0, 0, 0);
                        XYZ Pe_gir = Get_Node_Position(stb, gir.idNode_end, 0, 0, 0);
                        XYZ vecU_gir = new XYZ();
                        if (Node_Check(stb, gir.idNode_start, node))
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
                    double x = 0, y = 0, z = 0;
                    Search_Girder_Offset_XYZ(stb, id_min, ref x, ref y, ref z, s_e_min);
                    gir_offset_X += x;
                    gir_offset_Y += y;
                    gir_offset_Z += z;
                }
                if (s_e_max != "")
                {
                    double x = 0, y = 0, z = 0;
                    Search_Girder_Offset_XYZ(stb, id_max, ref x, ref y, ref z, s_e_max);
                    gir_offset_X += x;
                    gir_offset_Y += y;
                    gir_offset_Z += z;
                }
                if (s_e_max != "" && s_e_min != "")
                { gir_offset_Z = gir_offset_Z / 2; }
            }
               
                
            if(Search_Column_Offset_XYZ(stb, node, ref clm_offset_X, ref clm_offset_Y, ref clm_offset_Z))
            {
                offset = TransformCoord(Ps, Pe, clm_offset_X , clm_offset_Y, gir_offset_Z, rotate);
            }
            else
            {
                offset = TransformCoord(Ps, Pe, gir_offset_X, gir_offset_Y, gir_offset_Z, rotate);
            }

              
           
            
            //offset = new XYZ(clm_offset_X, clm_offset_Y, gir_offset_Z);
            return offset;
        }

       
        /// <summary>鉄骨形状判定
        /// </summary>
        /// <param name="stb"></param>
        /// <param name="shape"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        internal static string Check_Steel(STBclass stb, string shape, ref int ind)
        {
            string shapetype = "";
            if(stb.StbModel.StbSections.StbSecSteel != null)
            {
                STBclass.StbModelClass.StbSectionsClass.StbSecSteel_Class steel = stb.StbModel.StbSections.StbSecSteel;

                if (steel.StbSecRoll_H != null)
                {
                    for(int i = 0; i < steel.StbSecRoll_H.Count(); i++)
                    {
                        if(steel.StbSecRoll_H[i].name == shape)
                        {
                            shapetype = RevitLNK.st_steel_H;
                            ind = i;
                            break;
                        }
                    }
                }
                if(shapetype == "")
                {
                    if(steel.StbSecBuild_H != null)
                    {
                        for(int i =0; i < steel.StbSecBuild_H.Count(); i++)
                        {
                            if(steel.StbSecBuild_H[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_BH;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_BOX != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_BOX.Count(); i++)
                        {
                            if (steel.StbSecRoll_BOX[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_Box;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecBuild_BOX != null)
                    {
                        for (int i = 0; i < steel.StbSecBuild_BOX.Count(); i++)
                        {
                            if (steel.StbSecBuild_BOX[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_BBox;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecPipe != null)
                    {
                        for (int i = 0; i < steel.StbSecPipe.Count(); i++)
                        {
                            if (steel.StbSecPipe[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_Pipe;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_T != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_T.Count(); i++)
                        {
                            if (steel.StbSecRoll_T[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_T;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_C != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_C.Count(); i++)
                        {
                            if (steel.StbSecRoll_C[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_C;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_L != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_L.Count(); i++)
                        {
                            if (steel.StbSecRoll_L[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_L;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_LipC != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_LipC.Count(); i++)
                        {
                            if (steel.StbSecRoll_LipC[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_LipC;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_FB != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_FB.Count(); i++)
                        {
                            if (steel.StbSecRoll_FB[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_FB;
                                ind = i;
                                break;
                            }
                        }
                    }
                }
                if (shapetype == "")
                {
                    if (steel.StbSecRoll_Bar != null)
                    {
                        for (int i = 0; i < steel.StbSecRoll_Bar.Count(); i++)
                        {
                            if (steel.StbSecRoll_Bar[i].name == shape)
                            {
                                shapetype = RevitLNK.st_steel_Bar;
                                ind = i;
                                break;
                            }
                        }
                    }
                }

            }
            
            return shapetype;
        }


        /// <summary>
        /// 鉄骨材料（ウェブ）の取得
        /// </summary>
        /// <param name="strength_web">ウェブ</param>
        /// <param name="strength_main">主</param>
        /// <returns>ウェブがブランクなら主を返す</returns>
        internal static string GetStrength_web(string strength_web, string strength_main)
        {
            return strength_web != "" ? strength_web : strength_main;
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





        #region 進捗状況表示

        /// <summary>進捗状況を表示するForm
        /// </summary>
        internal static System.Windows.Forms.Form gaugeForm = null;

        private static string preGaugeText = "";
        private static int preGaugePer = 0;
        private static int RefreshPer = 0;

        private static bool gaugePosSet = false;
        private static int preGaugeLeft;
        private static int preGaugeTop;
        private static int preGaugeHeight;
        private static int preGaugeWidth;

        /// <summary>進捗ゲージの表示位置サイズの設定
        /// </summary>
        /// <param name="setFlag">true=位置サイズを指定, false=初期化</param>
        internal static void GaugePositionSet(bool setFlag, int left, int top, int width, int height)
        {
            if (setFlag)
            {
                preGaugeLeft = left;
                preGaugeTop = top;
                preGaugeWidth = width;
                preGaugeHeight = height;
            }
            else
            {
                left = 0;
                top = 0;
                width = 300;
                height = 38;
                preGaugeLeft = left;
                preGaugeTop = top;
                preGaugeWidth = width;
                preGaugeHeight = height;
            }
            gaugePosSet = setFlag;
        }
        internal static void GaugeShow()
        {
            if (gaugeForm == null) return;


            int GaugeWidth = 362;
            int GaugeHeight = 33;
            int GaugeLeft = 0;
            int GaugeTop = 0;

            System.Windows.Forms.Control [] Ctr;
            gaugeForm.SuspendLayout();
            Ctr = gaugeForm.Controls.Find("Gauge", false);
            if (Ctr.Length != 0) return;

            if (GaugeWidth > gaugeForm.ClientSize.Width) GaugeWidth = gaugeForm.ClientSize.Width - 10;
            GaugeLeft = (int)((gaugeForm.ClientSize.Width - GaugeWidth) / 2);
            GaugeTop = (int)((gaugeForm.ClientSize.Height - GaugeHeight) / 2);

            if (gaugePosSet)
            {
                GaugeLeft = preGaugeLeft;
                GaugeTop = preGaugeTop;
                GaugeWidth = preGaugeWidth;
                GaugeHeight = preGaugeHeight;
            }

            PictureBox Gauge = new PictureBox();
            Gauge.Name = "Gauge";
            Gauge.Width = GaugeWidth;
            Gauge.Height = GaugeHeight;
            Gauge.Visible = false;
            gaugeForm.Controls.Add(Gauge);
            Gauge.Visible = false;
            Gauge.BringToFront();
            Gauge.Left = GaugeLeft;
            Gauge.Top = GaugeTop;
            Gauge.BorderStyle = BorderStyle.Fixed3D;
            Gauge.BackColor = System.Drawing.Color.White;
            gaugeForm.ResumeLayout(true);
            Gauge.Visible = true;
            Application.DoEvents();

            preGaugeText = "";
            preGaugePer = 0;
            RefreshPer = 0;
        }
        internal static void GaugeClose()
        {
            System.Windows.Forms.Control[] Ctr;
            gaugeForm.SuspendLayout();
            Ctr = gaugeForm.Controls.Find("Gauge", false);
            if (Ctr.Length == 0) return;
            gaugeForm.Controls.Remove(Ctr[0]);
            gaugeForm.ResumeLayout(false);
        }
        internal static void GaugePercent(string Txt, int Per)
        {
            if (gaugeForm == null) return;

            System.Windows.Forms.Control[] Ctr;
            PictureBox Gauge;

            Ctr = gaugeForm.Controls.Find("Gauge", false);
            if (Ctr.Length == 0) return;
            Gauge = (PictureBox)Ctr[0];

            bool GoFlag;
            int p;
            string TxtMoji;
            Single MW, MH, sX, sY;
            System.Drawing.Point sp = new System.Drawing.Point();
            System.Drawing.Point ep = new System.Drawing.Point();
            System.Drawing.Color BC, FC;
            Brush FBrsh; //ステータスバーの色
            Brush MBrsh; //文字の色
            Brush BBrsh;
            Gauge.Image = new Bitmap(Gauge.ClientRectangle.Width, Gauge.ClientRectangle.Height);
            Graphics g = Graphics.FromImage(Gauge.Image);
            Font ft = new Font(SystemInformation.MenuFont, FontStyle.Regular);
            StringFormat sf = StringFormat.GenericDefault;

            if (preGaugeText != Txt) { preGaugeText = ""; preGaugePer = -1; }
            p = Per;
            if (Per < 0) p = 0;
            if (Per > 100) p = 100;
            TxtMoji = Txt;
            if (TxtMoji == "") TxtMoji = p.ToString() + "%";

            GoFlag = false;
            if (preGaugeText != TxtMoji || p > preGaugePer)
            { GoFlag = true; }
            else
            { if (Txt == "") GoFlag = true; }
            if (GoFlag)
            {
                BC = System.Drawing.Color.White;
                FC = System.Drawing.Color.Yellow;
                BBrsh = Brushes.White;
                FBrsh = Brushes.DeepSkyBlue;
                MBrsh = Brushes.DarkOliveGreen;
                SizeF StringSize = g.MeasureString(TxtMoji, ft, 1000, sf);
                MW = StringSize.Width;
                MH = StringSize.Height;
                sX = (Gauge.Width - MW) / 2;
                sY = (Gauge.Height - MH) / 2;
                sp.X = (int)sX; sp.Y = (int)sY;
                ep.X = (int)MW; ep.Y = (int)MH;
                sp.X = 0;
                sp.Y = 0;
                ep.X = Gauge.Width;
                ep.Y = Gauge.Height;
                g.FillRectangle(BBrsh, sp.X, sp.Y, (ep.X - sp.X), (ep.Y - sp.Y));
                g.DrawString(TxtMoji, ft, MBrsh, sX, sY);
                if (p != -1)
                {
                    sp.X = 0;
                    sp.Y = 0;
                    ep.X = (int)(Gauge.Width * Per / 100);
                    ep.Y = Gauge.Height;
                    g.FillRectangle(FBrsh, sp.X, sp.Y, ep.X, ep.Y);
                    g.DrawString(TxtMoji, ft, MBrsh, sX, sY);
                }
                Gauge.Refresh();
                preGaugeText = TxtMoji;
                preGaugePer = Per;

                // 5%ずつDoEventsを発生させて画面を更新する
                if (preGaugePer - RefreshPer > 5)
                {
                    Application.DoEvents();
                    RefreshPer = preGaugePer;
                }
                Application.DoEvents();
            }
            g.Dispose();
            
        }
        #endregion

        
        internal void OutputDebubCommentLog<T>(T obj, int id, string logname, string typename, int[] nodeids ){
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

