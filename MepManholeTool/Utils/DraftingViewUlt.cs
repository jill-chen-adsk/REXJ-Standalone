using System ;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using MepManholeTool.Const ;
using MepManholeTool.Models;

namespace MepManholeTool.Utils
{
    public static class DraftingViewUlt
    {
        /// <summary>
        /// Create drafting view for selected element
        /// </summary>
        /// <param name="document">Revit doc</param>
        /// <param name="dftView">Drafting view</param>
        /// <param name="parameters">Selected element's parameters</param>
        public static void DrawDraftingView(Document document, View dftView, IList<RoutingParameter> parameters, int length = 0)
        {
            double baseLineLength = 1000;
            if (length <= 0)
                baseLineLength = parameters.Sum(parameter => parameter.PipeLength);
            else
                baseLineLength = parameters.Count() * length;
            DrawBaseLevel(document, dftView, new(0, 0, 0), new(UnitUtils.ConvertToInternalUnits(baseLineLength, UnitTypeId.Millimeters), 0, 0));
            XYZ startPoint = new XYZ(UnitUtils.ConvertToInternalUnits(100, UnitTypeId.Millimeters), 0, 0);
            bool drawCon = false;
            MasuViewItem fromMasu = null;
            MasuViewItem toMasu = null;

            foreach (var parameter in parameters)
            {
                toMasu = DrawMasu(document, dftView, parameter, ref startPoint, length);
                if (drawCon)
                {
                    DrawConnection(document, dftView, fromMasu, toMasu);
                }

                fromMasu = toMasu;
                drawCon = true;
            }
        }

        private static void DrawBaseLevel(Document document, View dftView, XYZ startPoint, XYZ endPoint)
        {
            DrawLine(document, dftView, startPoint, endPoint);
        }
        
        /// <summary>
        /// 桝確認ビューにREXJ_詳細_桝を追加する
        /// 桝深さ　＝　桝底　＋　当該桝地盤レベル
        /// 桝ファミリ     設定管底   当該桝地盤レベル  設定管底＋泥だまり
        /// 詳細桝ファミリ　REXJ管底  REXJ地盤高　　　　REXJ桝底
        /// </summary>
        /// <param name="document"></param>
        /// <param name="view"></param>
        /// <param name="parameter"></param>
        /// <param name="startPoint"></param>
        /// <param name="fixedLength">桝間</param>
        /// <returns></returns>
        public static MasuViewItem DrawMasu(Document document, View view, RoutingParameter parameter, ref XYZ startPoint, int fixedLength)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(Family));
            var family = collector.Cast<Family>().FirstOrDefault(f => f.Name == "RJ_2D_凡_配管付属品_桝_断面");
            var masuviewItem = new MasuViewItem(startPoint, parameter, parameter.MasuItem!.Size);
            
            if (family != null)
            {
                FamilySymbol familySymbol = family.GetFamilySymbolIds()
                    .Select(id => family.Document.GetElement(id))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault(symbol => symbol.Name == "RJ_2D_凡_配管付属品_桝_断面");
                
                using (Transaction tx = new Transaction(document, $"Create Family Instance {familySymbol?.Id}"))
                {
                   tx.Start();

                    if (familySymbol?.IsActive == false)
                    {
                        familySymbol.Activate();
                    }
                    //桝深さ　＝　桝底　＋　当該桝地盤レベル
                    //桝ファミリ     設定管底   当該桝地盤レベル  設定管底＋泥だまり
                    //詳細桝ファミリ　REXJ管底  REXJ地盤高　　　　REXJ桝底　 
                    
                    var kanteiValue = Math.Abs( parameter.BottomHeight.Value ) ;
                    var jibanVal = parameter.MasuItem!.HeightFromGroundLevel ;
                    
                    //REXJ_詳細_桝インスタンスを追加する
                    FamilyInstance familyInstance =
                        document.Create.NewFamilyInstance( startPoint, familySymbol, view ) ;
                    
                    //パラメータ設定：記号 
                    Parameter symbol = familyInstance.LookupParameter(MepModelLineConst.桝パラメータ_記号);
                    if ( symbol?.IsReadOnly == false ) {
                        symbol.Set( parameter.MasuItem!.MasuSymbol ) ;
                    }
                    
                    //パラメータ設定：幅
                    Parameter haba = familyInstance.LookupParameter("桝の幅");
                    if (haba?.IsReadOnly == false && parameter.MasuItem!.Size.HasValue)
                    {
                        haba.Set(UnitUtils.ConvertToInternalUnits(parameter.MasuItem!.Size.Value, UnitTypeId.Millimeters));
                    }
                    
                    //パラメータ更新：地盤高
                    Parameter jiban = familyInstance.LookupParameter("当該桝地盤レベル");
                    if (jiban?.IsReadOnly == false)
                    {
                        jiban.Set(UnitUtils.ConvertToInternalUnits(jibanVal, UnitTypeId.Millimeters));
                    }
                    
                    //パラメータ更新：流出管底
                    Parameter kantei = familyInstance.LookupParameter("流出管底");
                    if (kantei?.IsReadOnly == false)
                    {
                        kantei.Set(UnitUtils.ConvertToInternalUnits(kanteiValue, UnitTypeId.Millimeters));
                    }
                    
                    //パラメータ更新：泥だまり
                    Parameter mudPuddle = familyInstance.LookupParameter("泥だまり");
                    var puddle = parameter.MasuItem!.MudPuddle.HasValue ? Math.Abs(parameter.MasuItem.MudPuddle.Value) : 0;
                    if (mudPuddle?.IsReadOnly == false)
                    {
                        mudPuddle.Set(UnitUtils.ConvertToInternalUnits(puddle, UnitTypeId.Millimeters));
                    }
                    
                    //パラメータ更新：桝底（設定管底＋泥だまり）
                    Parameter bottom = familyInstance.LookupParameter("桝底");
                    if (bottom?.IsReadOnly == false )
                    {
                        bottom.Set(UnitUtils.ConvertToInternalUnits(kanteiValue + puddle, UnitTypeId.Millimeters));
                    }

                    //タグ付け
                    var tmpSize = parameter.MasuItem!.Size.HasValue ? Math.Abs(parameter.MasuItem!.Size.Value) : 300;
                    var tmpPuddle = parameter.MasuItem.MudPuddle.HasValue ? Math.Abs(parameter.MasuItem.MudPuddle.Value) : 0;
                    IndependentTag tag = IndependentTag.Create(document, view.Id, new Reference(familyInstance),
                        false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal,
                        startPoint + new XYZ(
                            UnitUtils.ConvertToInternalUnits(tmpSize / 2,
                                UnitTypeId.Millimeters),
                            UnitUtils.ConvertToInternalUnits(-kanteiValue - tmpPuddle,
                                UnitTypeId.Millimeters), 0));

                    tx.Commit();
                }
                
                startPoint += new XYZ(UnitUtils.ConvertToInternalUnits(fixedLength <= 0 ? parameter.PipeLength : fixedLength, UnitTypeId.Millimeters), 0, 0);
            }
            
            return masuviewItem;
        }

        private static void DrawConnection(Document document, View view, MasuViewItem fromMasu, MasuViewItem toMasu)
        {
            XYZ startPoint = new XYZ(fromMasu.StartPoint.X, -UnitUtils.ConvertToInternalUnits(- fromMasu.Parameter.BottomHeight.Value, UnitTypeId.Millimeters), 0);
            XYZ endPoint = new XYZ(toMasu.StartPoint.X, -UnitUtils.ConvertToInternalUnits( - toMasu.Parameter.BottomHeight.Value - (toMasu.Parameter.MasuItem!.PipeStep ?? 0), UnitTypeId.Millimeters), 0);
            
            DrawLine(document, view, startPoint, endPoint);
        }

        static bool IsPointInsideFamilyInstance(FamilyInstance instance, XYZ point)
        {
            BoundingBoxXYZ bbox = instance.get_BoundingBox(null);
            if (bbox == null) return false;

            Transform transform = bbox.Transform;

            XYZ localPoint = transform.Inverse.OfPoint(point);

            XYZ min = bbox.Min;
            XYZ max = bbox.Max;

            return localPoint.X >= min.X && localPoint.X <= max.X &&
                   localPoint.Y >= min.Y && localPoint.Y <= max.Y &&
                   localPoint.Z >= min.Z && localPoint.Z <= max.Z;
        }
        
        private static void DrawLine(Document doc, View dftView, XYZ startPoint, XYZ endPoint)
        {
            using (Transaction tx = new Transaction(doc, "Create Masu Line"))
            {
                try
                {
                    tx.Start();
                    
                    Line line = Line.CreateBound(startPoint, endPoint);
                    doc.Create.NewDetailCurve(dftView, line);
                    
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }

        private static void CreateDimension(Document document, View dftView, Line line, ReferenceArray references)
        {
            using (Transaction tx = new Transaction(document, "Create Dimension"))
            {
                try
                {
                    tx.Start();
                    
                    var dim = document.Create.NewDimension(dftView, line, references);
                    if (dim.DimensionType.get_Parameter(BuiltInParameter.TEXT_SIZE)?.IsReadOnly == false) dim.DimensionType.get_Parameter(BuiltInParameter.TEXT_SIZE).SetValueString("0.5");
                    if (dim.DimensionType.get_Parameter(BuiltInParameter.DIM_WITNS_LINE_EXTENSION_BELOW)?.IsReadOnly == false) dim.DimensionType.get_Parameter(BuiltInParameter.DIM_WITNS_LINE_EXTENSION_BELOW)?.SetValueString("1.0");
                    if (dim.DimensionType.get_Parameter(BuiltInParameter.LINE_PEN).IsReadOnly == false) dim.DimensionType.get_Parameter(BuiltInParameter.LINE_PEN).Set(1);
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }
    }

    public class MasuViewItem
    {
        /// <summary>
        /// 桝描くスタートポイント
        /// </summary>
        public XYZ StartPoint { get; set; }
        
        /// <summary>
        /// 桝プロパティ
        /// </summary>
        public RoutingParameter Parameter { get; set; }

        /// <summary>
        /// 桝サイズ
        /// </summary>
        public double? Size { get; set; }

        public MasuViewItem() { }
        public MasuViewItem(XYZ startPoint, RoutingParameter parameter, double? size)
        {
            StartPoint = startPoint;
            Parameter = parameter;
            Size = size;
        }
    }
}