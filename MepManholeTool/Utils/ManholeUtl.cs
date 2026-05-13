using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MepManholeTool.Const;
using MepManholeTool.Models;
using MepManholeTool.Properties;
using System.Windows.Media;

namespace MepManholeTool.Utils
{
    public static class ManholeUtl
    {
        public static List<FamilySymbol> GetAnnotationSymbols( this Document doc )
        {
            var annotationSymbols = new List<FamilySymbol>() ;
            if ( doc != null ) {
                var catId = VersionExtension.NewElementIdEx( (long)BuiltInCategory.OST_PipeAccessoryTags) ;
                var collector = new FilteredElementCollector( doc ) ;
                var collection = collector.OfClass( typeof( FamilySymbol ) ).ToElements() ;
                foreach ( var e in collection )
                    if ( e is FamilySymbol symbol && symbol.Category.Id == catId && e.Name.Contains("記号") )
                        annotationSymbols.Add( symbol ) ;                
            }
            return annotationSymbols ;
        }
        
        /// <summary>
        /// Get parameters from Reference
        /// </summary>
        /// <param name="uiDoc">Revit UI Doc</param>
        /// <param name="references"></param>
        /// <returns></returns>
        public static List<MasuItem> GetManholeParameterFromRef(UIDocument uiDoc, IList<Reference> references)
        {
            var masuItems = new List<MasuItem>();
            foreach (var reference in references)
            {
                Element element = uiDoc.Document.GetElement(reference);
                if (element != null)
                {
                    if (element is FamilyInstance familyInstance)
                    {
                        //Elevation from level
                        var heightFromBase = element.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                        //Level
                        var groundLevel = element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);

                        var curMapping = GlobalMappings.Instance.Manholes.FirstOrDefault(x => x.Family == familyInstance.Symbol.Family.Name);

                        if (curMapping is not null)
                        {
                            //記号
                            var symbol = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_記号);

                            //備考
                            var biko = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_備考);

                            //桝サイズ
                            var size = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_桝サイズ);

                            //出口径
                            var output = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_出口径);

                            //桝深さ
                            var depth = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_桝深さ);

                            //泥だまり
                            var mudPuddle = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_泥だまり);

                            //配管段差
                            var pipeStep = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_配管段差);

                            //流出管底
                            var baseBottomHeight = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_管底高);

                            //当該桝地盤レベル
                            var heightGroundLevel = element.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_当該桝地盤レベル);
                            
                            //コネクター
                            var mepModel = familyInstance.MEPModel;
                            ConnectorManager connectorManager = mepModel.ConnectorManager;
                            var connectorSet = connectorManager.Connectors;
                            IList<Connector> connectors = connectorSet.Cast<Connector>().ToList();

                            //必要なパラメータを取得できなかった場合飛ばす
                            if (new[] { symbol, biko, depth, baseBottomHeight, heightGroundLevel }.Any(p => p == null))
                            {
                                continue;
                            }

                            // Convert heightFromBase to mm (from feet)
                            double heightFromBaseMm = UnitUtils.ConvertFromInternalUnits(heightFromBase.AsDouble(), UnitTypeId.Millimeters);
                            
                            // Helper function to convert nullable parameter to mm
                            double? ConvertToMm(Parameter param)
                            {
                                if (param == null) return null;
                                return UnitUtils.ConvertFromInternalUnits(param.AsDouble(), UnitTypeId.Millimeters);
                            }

                            masuItems.Add(new MasuItem(
                                element, 
                                familyInstance.Symbol.Family.Name, 
                                biko.AsValueString(), 
                                symbol.AsValueString(), 
                                heightFromBaseMm, 
                                ConvertToMm(size), 
                                ConvertToMm(output),
                                ConvertToMm(mudPuddle),
                                ConvertToMm(pipeStep), 
                                ConvertToMm(baseBottomHeight) ?? 0,
                                ConvertToMm(depth) ?? 0, 
                                groundLevel.AsValueString(),
                                ConvertToMm(heightGroundLevel) ?? 0, 
                                connectors, 
                                null, 
                                null));
                        }
                    }
                }
            }
            return masuItems;
        }

        /// <summary>
        /// マッピング情報でパラメータを取得する
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="_familyName"></param>
        /// <param name="_fromParameter"></param>
        /// <returns></returns>
        public static Parameter LookupManholeParameter(this Element _element, string _familyName, string _fromParameter)
        {
            //グローバルパラメータマッピングから対象
            var familyMapping = GlobalMappings.Instance.Manholes.FirstOrDefault(x => x.Family == _familyName);
            if(familyMapping is not null)
            {
                var targetParm = familyMapping.Mapping.FirstOrDefault(x => x.FromParameter == _fromParameter);
                if(targetParm is not null)
                {
                    return _element.LookupParameter(targetParm.ToParameter);
                }
            }

            return null;
        }

        /// <summary>
        /// Add tag for item
        /// </summary>
        /// <param name="uiDoc">Revit UI Doc</param>
        /// <param name="masus">items</param>
        public static bool SetSymbol(UIDocument uiDoc, List<MasuItem> masus, ElementId symbol )
        {
            var doc = uiDoc.Document;
            var tmpIndex = 1;
            
            using (Transaction trans = new Transaction(doc, "Set symbol"))
            {
                try
                {
                    trans.Start();

                    if (symbol == null)
                    {
                        var annotationSymbols = doc.GetAnnotationSymbols();
                        if (annotationSymbols != null && annotationSymbols.Count > 0)
                        {
                            var preferredSymbol = annotationSymbols.FirstOrDefault(fs => 
                                fs.Family.Name == "00_RJ_タグ_配管付属品_CL15");
                            
                            symbol = (preferredSymbol ?? annotationSymbols.FirstOrDefault())?.Id;
                        }
                    }

                    foreach (var masu in masus)
                    {
                        var tagExists = TagExistsForElement(doc, doc.ActiveView, masu.MasuElement);

                        try
                        {
                            if ( symbol != null ) {
                                if ( tagExists == null ) {
                                    IndependentTag tag = IndependentTag.Create( doc, symbol, doc.ActiveView.Id,
                                        new Reference( masu.MasuElement ), false, TagOrientation.Horizontal,
                                        ( masu.MasuElement.Location as LocationPoint )!.Point ) ;
                                    tag.LeaderEndCondition = LeaderEndCondition.Attached ;
                                    tag.HasLeader = false ;
                                }
                                else {
                                    //delete current tag
                                    doc.Delete( tagExists ) ;
                                    
                                    IndependentTag tag = IndependentTag.Create( doc, symbol, doc.ActiveView.Id,
                                        new Reference( masu.MasuElement ), false, TagOrientation.Horizontal,
                                        ( masu.MasuElement.Location as LocationPoint )!.Point ) ;
                                    tag.LeaderEndCondition = LeaderEndCondition.Attached ;
                                    tag.HasLeader = false ;
                                }
                            }
                            else {
                                if ( tagExists == null ) {
                                    IndependentTag tag = IndependentTag.Create( doc, doc.ActiveView.Id,
                                        new Reference( masu.MasuElement ), false, TagMode.TM_ADDBY_CATEGORY,
                                        TagOrientation.Horizontal, ( masu.MasuElement.Location as LocationPoint )!.Point ) ;
                                    tag.LeaderEndCondition = LeaderEndCondition.Attached ;
                                    tag.HasLeader = false ;                                
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.RollBack();
                            string categoryName = masu.MasuElement.Category?.Name ?? "Unknown Category";
                            TaskDialog.Show("Tag Creation Error", $"Tag for {categoryName} is not loaded.");
                            return false;
                        }

                        var foundParameter = masu.MasuElement.LookupParameter("記号");
                        if (foundParameter != null && string.IsNullOrEmpty(foundParameter.AsString()))
                        {
                            masu.MasuSymbol = MepModelLineConst.記号先頭文字 + tmpIndex;
                            foundParameter.Set(masu.MasuSymbol);
                            tmpIndex++;
                        }
                    }
                
                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.RollBack();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Element has tag?
        /// </summary>
        /// <param name="doc">Revit Doc</param>
        /// <param name="view">Active View</param>
        /// <param name="element">Element</param>
        /// <returns></returns>
        private static ElementId TagExistsForElement(Document doc, View view, Element element)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);
            collector.OfClass(typeof(IndependentTag));

            foreach (Element tagElement in collector)
            {
                IndependentTag? tag = tagElement as IndependentTag;
                if (tag != null)
                {
                    Reference tagReference = tag.GetTaggedReferences().First();
                    if (tagReference != null && tagReference.ElementId == element.Id) {
                        return tagElement.Id ;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Create parameters for creating model line and drafting view
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="masus">Items</param>
        /// <returns>List of parameter can bind in WPF view</returns>
        public static ObservableCollection<RoutingParameter> GetRoutingParameters(Document doc, IList<MasuItem> masus, MasuToolUnit masuToolUnit)
        {
            var masuSettings = new ObservableCollection<RoutingParameter>();
            foreach (var masu in masus)
            {
                //必要管底　＝　流出管底　＋　基準レベルから高さ
                masuSettings.Add(new RoutingParameter {MasuItem = masu, GradientDenominator = 100, Depth = masu.Depth, BaseBottomHeight = masu.BaseBottomHeight, BottomHeight = new MasuToolHeight(masuToolUnit, 0, 2), RequiredBottomHeight = new MasuToolHeight(masuToolUnit, masu.BaseBottomHeight, 2), CheckBottomHeight = new MasuToolHeight(masuToolUnit, 0, 2), PipeLength = 0});
                //masuSettings.Add(new RoutingParameter {MasuItem = masu, GradientDenominator = 100, Depth = masu.Depth, BaseBottomHeight = masu.BaseBottomHeight, BottomHeight = new MasuToolHeight(masuToolUnit, 0, 2), RequiredBottomHeight = new MasuToolHeight(masuToolUnit, ignoreLevel ? masu.BaseBottomHeight : masu.BaseBottomHeight + masu.HeightFromBase, 2), CheckBottomHeight = new MasuToolHeight(masuToolUnit, 0, 2), PipeLength = 0});
            }
            return masuSettings;
        }

        /// <summary>
        /// 桝のパラメータを更新する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parameter"></param>
        /// <param name="overrideLevelHeight">当該桝地盤レベルを上書きする</param>
        public static void UpdateParameters(Document doc, RoutingParameter parameter, bool overrideLevelHeight = false)
        {
            if ( parameter.MasuItem != null && parameter.MasuItem!.MasuElement is FamilyInstance familyInstance ) {
                var curMapping = GlobalMappings.Instance.Manholes.FirstOrDefault(x => x.Family == familyInstance.Symbol.Family.Name);
                if ( curMapping is not null ) {
                    //記号再設定
                    var foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter(familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_記号);
                    if ( foundParameter is { IsReadOnly: false } ) foundParameter.Set( parameter.MasuItem.MasuSymbol ) ;

                    //備考
                    foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_備考 ) ;
                    if ( foundParameter is { IsReadOnly: false } ) foundParameter.Set( parameter.MasuItem.Biko ) ;

                    //当該桝地盤レベル
                    foundParameter =
                        parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_当該桝地盤レベル ) ;
                    if ( overrideLevelHeight && foundParameter is { IsReadOnly: false } ) {
                        parameter.MasuItem.HeightFromGroundLevel = parameter.MasuItem.HeightToTopo ;
                        foundParameter.Set( UnitUtils.ConvertToInternalUnits( parameter.MasuItem.HeightFromGroundLevel,
                            UnitTypeId.Millimeters ) ) ;
                    }

                    //基準レベルからの管底高 ＋ 基準レベルからの高さ
                    foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_管底高 ) ;
                    if ( foundParameter is { IsReadOnly: false } ) {
                        parameter.BottomHeight.Value = parameter.BottomHeight.MmValue ;
                        foundParameter.Set(
                            UnitUtils.ConvertToInternalUnits( Math.Abs( parameter.BottomHeight.MmValue ) + parameter.MasuItem.HeightFromBase ,
                                UnitTypeId.Millimeters ) ) ;
                    }

                    // foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_管底高_丸 ) ;
                    // if ( foundParameter is { IsReadOnly: false } ) {
                    //     parameter.BottomHeight.Value = parameter.BottomHeight.MmValue ;
                    //     foundParameter.Set(
                    //         UnitUtils.ConvertToInternalUnits( Math.Abs( parameter.BottomHeight.MmValue ),
                    //             UnitTypeId.Millimeters ) ) ;
                    // }

                    //配管段差
                    foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_配管段差 ) ;
                    if ( foundParameter == null || foundParameter.IsReadOnly ) {
                        foundParameter =
                            parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_配管段差_丸 ) ;
                    }

                    if ( foundParameter is { IsReadOnly: false } && parameter.MasuItem.PipeStep.HasValue )
                        foundParameter.Set( UnitUtils.ConvertToInternalUnits( parameter.MasuItem.PipeStep.Value,
                            UnitTypeId.Millimeters ) ) ;

                    //出口
                    foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_出口径 ) ;
                    if ( foundParameter is { IsReadOnly: false } && parameter.MasuItem.OutputDiameter.HasValue )
                        foundParameter.Set( UnitUtils.ConvertToInternalUnits( parameter.MasuItem.OutputDiameter.Value,
                            UnitTypeId.Millimeters ) ) ;

                    //泥だまり
                    foundParameter = parameter.MasuItem.MasuElement.LookupManholeParameter( familyInstance.Symbol.Family.Name, MepModelLineConst.桝パラメータ_泥だまり ) ;
                    if ( foundParameter is { IsReadOnly: false } && parameter.MasuItem.MudPuddle.HasValue )
                        foundParameter.Set( UnitUtils.ConvertToInternalUnits( parameter.MasuItem.MudPuddle.Value,
                            UnitTypeId.Millimeters ) ) ;
                }
            }
            
        }

        /// <summary>
        /// Set element's parameter value by name
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="masu">Item</param>
        /// <param name="symbol">Parameter name</param>
        /// <param name="val">Value to set</param>
        public static void SetParameterStringValue(Document doc, Element masu, string parmName, string val)
        {
            var foundParameter = masu.LookupParameter(parmName);
            if (foundParameter != null && !string.IsNullOrEmpty(val))
            {
                using (Transaction tx = new Transaction(doc))
                {
                    try
                    {
                        tx.Start();
                    
                        foundParameter.Set(val);
                    
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
        
        /// <summary>
        /// Set element's parameter value by name
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="masu">Item</param>
        /// <param name="symbol">Parameter name</param>
        /// <param name="val">Value to set</param>
        public static void SetParameterDoubleValue(Document doc, Element masu, string symbol, double val)
        {
            var foundParameter = masu.LookupParameter(symbol);
            if (foundParameter != null)
            {
                using (Transaction tx = new Transaction(doc))
                {
                    try
                    {
                        tx.Start();
                    
                        foundParameter.Set(UnitUtils.ConvertToInternalUnits(val, UnitTypeId.Millimeters));
                    
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
        
        /// <summary>
        /// 過去モデル線分を削除する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <param name="parameters"></param>
        public static void ClearConnection( Document doc, View view, IList<RoutingParameter> parameters)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);
            var detailLines = collector
                .OfCategory(BuiltInCategory.OST_Lines)
                .WhereElementIsNotElementType()
                .Where(e => e is ModelCurve)
                .ToList();
            List<XYZ> points = new List<XYZ>();
            foreach ( var routingParameter in parameters ) {
                points.Add( ( routingParameter.MasuItem!.MasuElement as FamilyInstance )?.GetTotalTransform().Origin ) ;
            }
            var oldLines = FilterLinesNearPoints( detailLines, points) ;
            if ( oldLines?.Count > 0 ) {
                using Transaction tx = new Transaction(doc, "Clear Masu Line") ;
                try
                {
                    tx.Start();
                    foreach ( var oldLine in oldLines ) {
                        doc.Delete(oldLine.Id);
                    }
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }

        static List<Element> FilterLinesNearPoints(IEnumerable<Element> lines, List<XYZ> points, double threshold = 0.01)
        {
            List<Element> result = new List<Element>();

            foreach (Element element in lines)
            {
                if ( element.Location is LocationCurve locCurve ) {
                    XYZ start = locCurve.Curve.GetEndPoint(0);
                    XYZ end = locCurve.Curve.GetEndPoint(1);
                    
                    bool startClose = points.Any(p => GeometryUtl.Distance2D(start, p) < threshold);
                    bool endClose = points.Any(p => GeometryUtl.Distance2D(end, p) < threshold);

                    if (startClose && endClose)
                        result.Add(element);
                }
            }

            return result;
        }
        
        /// <summary>
        /// Create line from two points
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="startPoint">Start point location</param>
        /// <param name="endPoint">End point location</param>
        public static void CreateLine(Document doc, XYZ startPoint, XYZ endPoint)
        {
            using (Transaction tx = new Transaction(doc, $"Create line {startPoint.X},{startPoint.Y},{startPoint.Z} to {endPoint.X},{endPoint.Y},{endPoint.Z}"))
            {
                try
                {
                    tx.Start();
                    
                    var normal = startPoint.CrossProduct(endPoint).Normalize();
                    Plane plane = Plane.CreateByNormalAndOrigin(normal, startPoint);
                    SketchPlane sp = SketchPlane.Create(doc, plane);
                    Line line = Line.CreateBound(startPoint, endPoint);
                    ModelCurve modelCurve = doc.Create.NewModelCurve(line, sp);
                    
                    if (modelCurve.Location is LocationCurve locationCurve)
                    {
                        Line actualLine = Line.CreateBound(startPoint, endPoint);
                        locationCurve.Curve = actualLine;
                    }
                    
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }

        /// <summary>
        /// Rotate element: create line -> Rotate
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="id">Element id</param>
        /// <param name="fromPoint">From point location</param>
        /// <param name="toPoint">To point location</param>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="orgRotation">Original rotation angle</param>
        public static void RotateElement(Document doc, ElementId id, XYZ fromPoint, XYZ toPoint, double angle, double orgRotation)
        {
            using (Transaction tx = new Transaction(doc, $"Rotate element {id}"))
            {
                try
                {
                    ResetRotationBasis(doc, id, fromPoint, XYZ.BasisZ, orgRotation);
                    
                    tx.Start();
                    
                    Line axis2 = Line.CreateBound(fromPoint, fromPoint + XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, id, axis2, angle);
                    
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }

        /// <summary>
        /// Reset rotation base on Basis-X/Y/Z
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="id">Element id</param>
        /// <param name="fromPoint">Element location</param>
        /// <param name="basis">Rotate based on basis</param>
        /// <param name="orgRotation">Original rotation angle</param>
        public static void ResetRotationBasis(Document doc, ElementId id, XYZ fromPoint, XYZ basis, double orgRotation)
        {
            if ( orgRotation != 0 ) {
                using ( Transaction tx = new Transaction( doc, $"Reset rotation {id}" ) ) {
                    try {
                        tx.Start() ;

                        Line axis1 = Line.CreateBound( fromPoint, fromPoint + basis ) ;
                        ElementTransformUtils.RotateElement( doc, id, axis1, -orgRotation ) ;

                        tx.Commit() ;
                    }
                    catch {
                        tx.RollBack() ;
                        throw ;
                    }
                }
            }
        }

        /// <summary>
        /// Adjust out connector angle based-on 3 points
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="routingParameter">Routing parameter</param>
        /// <param name="startPoint">Start point of line</param>
        /// <param name="middlePoint">Middle point of line</param>
        /// <param name="endPoint">End point of line</param>
        /// <param name="orgRotation">Origin rotation</param>
        public static void AdjOutAngle(Document doc, RoutingParameter routingParameter, XYZ startPoint, XYZ middlePoint, XYZ endPoint, double orgRotation)
        {
            var param = routingParameter.MasuItem!.MasuElement.LookupParameter("流出角度");
            if (param != null && param.StorageType == StorageType.Double)
            { 
              Transaction tx = new Transaction(doc, $"Adjust out angle {routingParameter.MasuItem.MasuElement.Id}");
              var inVector = middlePoint - startPoint;
              var outVector = middlePoint - endPoint;
              double angleRad = outVector.AngleOnPlaneTo(inVector, XYZ.BasisZ) + orgRotation ;
              
              try
              {
                  tx.Start();
                  // Normalize angle: ensure it's between 0 and 2*PI
                  double twoPi = 2 * Math.PI;
                  if (angleRad >= twoPi)
                  {
                      angleRad = angleRad % twoPi;
                  }
                  
                  param.Set(angleRad);
                  tx.Commit();
              }
              catch
              {
                  tx.RollBack();
                  throw;
              }
            }
        }
        
        /// <summary>
        /// Adjust out connector angle based-on 3 points
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="routingParameter">Routing parameter</param>
        /// <param name="startPoint">Start point of line</param>
        /// <param name="middlePoint">Middle point of line</param>
        /// <param name="endPoint">End point of line</param>
        public static void AdjOutAngle(Document doc, RoutingParameter routingParameter, XYZ startPoint, XYZ middlePoint, XYZ endPoint)
        {
            var param = routingParameter.MasuItem!.MasuElement.LookupParameter("流出角度");
            if (param != null && param.StorageType == StorageType.Double)
            { 
                Transaction tx = new Transaction(doc, $"Adjust out angle {routingParameter.MasuItem.MasuElement.Id}");
                var inVector = middlePoint - startPoint;
                var outVector = middlePoint - endPoint;
                double angleRad = outVector.AngleOnPlaneTo(inVector, XYZ.BasisZ);
              
                try
                {
                    tx.Start();
                    param.Set(angleRad);
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Adjust out connector angle based-on 3 points
        /// </summary>
        /// <param name="doc">Revit doc</param>
        /// <param name="routingParameter">Routing parameter</param>
        /// <param name="startPoint">Start point of line</param>
        /// <param name="middlePoint">Middle point of line</param>
        /// <param name="endPoint">End point of line</param>
        /// <param name="orgRotation">Origin rotation</param>
        public static void AdjInAngle(Document doc, RoutingParameter routingParameter, XYZ startPoint, XYZ middlePoint, XYZ endPoint, double orgRotation)
        {
            var param = routingParameter.MasuItem!.MasuElement.LookupParameter("流入角度");
            if (param != null && param.StorageType == StorageType.Double)
            { 
                Transaction tx = new Transaction(doc, $"Adjust Input Angle {routingParameter.MasuItem.MasuElement.Id}");
                var inVector = middlePoint - startPoint;
                var outVector = middlePoint - endPoint;
                double angleRad = outVector.AngleOnPlaneTo(inVector, XYZ.BasisZ) + orgRotation ;
              
                try
                {
                    tx.Start();
                    // Normalize angle: ensure it's between 0 and 2*PI
                    double twoPi = 2 * Math.PI;
                    if (angleRad >= twoPi)
                    {
                        angleRad = angleRad % twoPi;
                    }
                    
                    param.Set(angleRad);
                    tx.Commit();
                }
                catch
                {
                    tx.RollBack();
                    throw;
                }
            }
        }
        
        public static List<Connector> GetConnectors(Element element)
        {
            List<Connector> connectors = new List<Connector>();

            if (element is FamilyInstance familyInstance)
            {
                MEPModel mepModel = familyInstance.MEPModel;
                if (mepModel != null)
                {
                    // Retrieve the connector manager
                    ConnectorManager connectorManager = mepModel.ConnectorManager;

                    // Loop through each connector and add it to the list
                    foreach (Connector connector in connectorManager.Connectors)
                    {
                        connectors.Add(connector);
                    }
                }
            }
            return connectors;
        }

        /// <summary>
        /// Calculate the required pipe bottom height
        /// </summary>
        /// <param name="masuSetting"></param>
        /// <param name="forceUpd"></param>
        public static void CalcBottomHeight(ObservableCollection<RoutingParameter> masuSetting, bool forceUpd, MasuToolUnit masuToolUnit, bool init = false)
        {
            // var curBaseHeight = Math.Abs(masuSetting.First().BaseBottomHeight);
            var curPipeLength = masuSetting.First().PipeLength;
            var curPipeStep = masuSetting.First().MasuItem!.PipeStep ?? 0;
            var curBottomHeight = init ? Math.Abs(masuSetting.First().BaseBottomHeight) : Math.Abs(masuSetting.First().BottomHeight.MmValue) ;
            
            for (int i = 1; i < masuSetting.Count; i++)
            {
                //勾配分母 # 0 => - （上）設定管底高 - （上）配管長／勾配分母 - （上）配管段差 - 基準レベルからの高さ
                //勾配分母 = 0 => - （上）設定管底高 - （上）配管長 - 基準レベルからの高さ
                masuSetting[i].RequiredBottomHeight = new MasuToolHeight(masuToolUnit, Math.Round(masuSetting[i - 1].GradientDenominator != 0 ? - curBottomHeight - curPipeLength/masuSetting[i - 1].GradientDenominator - curPipeStep : - curBottomHeight - curPipeStep, 0), 2);
                masuSetting[ i ].CheckBottomHeight.MasuToolUnit = masuToolUnit ;
                masuSetting[ i ].CheckBottomHeight.Value =
                    new MasuToolHeight( masuToolUnit, masuSetting[ i ].CheckBottomHeight.Value, 2 ).Value ;
                if ( masuSetting[ i ].GradientDenominator != 0 ) {
                    masuSetting[ i ].GradientDenominatorDifference =
                        masuSetting[ i ].PipeLength / masuSetting[ i ].GradientDenominator ;
                }
                else {
                    masuSetting[ i ].GradientDenominatorDifference = 0 ;
                }
                
                if (masuSetting[i].CheckBottomHeight?.Value == 0) masuSetting[i].BottomHeight = new MasuToolHeight(masuToolUnit, masuSetting[i].RequiredBottomHeight.MmValue, 2);

                // curBaseHeight = Math.Abs( masuSetting[ i ].BottomHeight.MmValue ) ;
                curPipeLength = masuSetting[i].PipeLength;
                curPipeStep = masuSetting[i].MasuItem!.PipeStep ?? 0;
                curBottomHeight = Math.Abs(masuSetting[i].BottomHeight.MmValue) ;
            }
            
            masuSetting.First().RequiredBottomHeight = new MasuToolHeight(masuToolUnit, - Math.Abs(masuSetting.First().RequiredBottomHeight.MmValue), 2);
            masuSetting.First().CheckBottomHeight.MasuToolUnit = masuToolUnit ;
            masuSetting.First().CheckBottomHeight.Value = new MasuToolHeight( masuToolUnit, masuSetting.First().CheckBottomHeight.Value, 2 ).Value ;
            if ( masuSetting.First().GradientDenominator != 0 ) {
                masuSetting.First().GradientDenominatorDifference =
                    masuSetting.First().PipeLength / masuSetting.First().GradientDenominator ;
            }
            else {
                masuSetting.First().GradientDenominatorDifference = 0 ;
            }
            if (masuSetting.First().CheckBottomHeight?.Value == 0) masuSetting.First().BottomHeight = new MasuToolHeight(masuToolUnit, masuSetting.First().RequiredBottomHeight.MmValue, 2);
        }
        
        /// <summary>
        /// Calculate the required pipe bottom height top to bottom
        /// </summary>
        /// <param name="masuSetting"></param>
        public static void CalcBottomHeightTopDown(ObservableCollection<RoutingParameter> masuSetting, MasuToolUnit masuToolUnit)
        {
            var curPipeLength = masuSetting.First().PipeLength;
            var curPipeStep = masuSetting.First().MasuItem!.PipeStep ?? 0;
            var curBottomHeight = Math.Abs(masuSetting.First().BottomHeight.MmValue) ;
            
            for (int i = 1; i < masuSetting.Count; i++)
            {
                //勾配分母 # 0 => - （上）設定管底高 - （上）配管長／勾配分母 - （上）配管段差 - 基準レベルからの高さ
                //勾配分母 = 0 => - （上）設定管底高 - （上）配管長 - 基準レベルからの高さ
                masuSetting[i].RequiredBottomHeight = new MasuToolHeight(masuToolUnit, Math.Round(masuSetting[i - 1].GradientDenominator != 0 ? - curBottomHeight - curPipeLength/masuSetting[i - 1].GradientDenominator - curPipeStep : - curBottomHeight - curPipeStep, 0), 2);
                if(masuSetting[i].CheckBottomHeight?.Value == 0) masuSetting[i].BottomHeight = new MasuToolHeight(masuToolUnit, masuSetting[i].RequiredBottomHeight.MmValue, 2);

                curPipeLength = masuSetting[i].PipeLength;
                curPipeStep = masuSetting[i].MasuItem!.PipeStep ?? 0;
                curBottomHeight = Math.Abs(masuSetting[i].BottomHeight.MmValue) ;
            }
            
            masuSetting.First().RequiredBottomHeight = new MasuToolHeight(masuToolUnit, -Math.Abs(masuSetting.First().RequiredBottomHeight.MmValue), 2);
        }
        
        /// <summary>
        /// Calculate the required pipe bottom height bottom to top
        /// </summary>
        /// <param name="masuSetting"></param>
        public static void CalcBottomHeightBottomUp(ObservableCollection<RoutingParameter> masuSetting, MasuToolUnit masuToolUnit)
        {
            var curBottomHeight = Math.Abs(masuSetting.Last().BottomHeight.MmValue) ;
            for (int i = masuSetting.Count - 2; i >= 0; i--)
            {
                var curPipeLength = masuSetting[i].PipeLength;
                var curPipeStep = masuSetting[i].MasuItem!.PipeStep ?? 0;
                var curGradient = masuSetting[ i ].GradientDenominator ;
                
                //勾配分母 # 0 => - （上）設定管底高 - （上）配管長／勾配分母 - （上）配管段差 - 基準レベルからの高さ
                //勾配分母 = 0 => - （上）設定管底高 - （上）配管長 - 基準レベルからの高さ
                masuSetting[i].RequiredBottomHeight = new MasuToolHeight(masuToolUnit, Math.Round(curGradient != 0 ? - curBottomHeight + curPipeLength/curGradient + curPipeStep : - curBottomHeight + curPipeStep, 0), 3);
                if(masuSetting[i].CheckBottomHeight?.Value == 0) masuSetting[i].BottomHeight = new MasuToolHeight(masuToolUnit, masuSetting[i].RequiredBottomHeight.MmValue, 3);
                curBottomHeight = Math.Abs(masuSetting[i].BottomHeight.MmValue) ;
            }
        }

        public static void HighLightElements(UIDocument uiDoc, IList<MasuItem> masuItems, string level)
        {
            uiDoc.Selection.SetElementIds(masuItems.Where(x => x.GroundLevel == level).Select(x => x.MasuElement.Id).ToList());
        }
    }
} 