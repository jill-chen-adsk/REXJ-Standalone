using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using MepManholeTool.Utils;
using MepManholeTool.ViewModel;
using MepManholeTool.Views;
using Revit = Autodesk.Revit;

namespace MepManholeTool.Tools
{
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [JournalingAttribute(JournalingMode.NoCommandData)]
    public class MepModelLineCommand : IExternalCommand, IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try {
                // 初期化 
                var rvtUiApp = commandData.Application ;
                var rvtUiDoc = rvtUiApp.ActiveUIDocument ;
                var rvtDoc = rvtUiDoc.Document ;
                var app = rvtUiApp.Application ;
                var pipAsFilter = new PipeAccessorySelectionFilter() ;
                var folder =
                    System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;

                if (rvtDoc.ActiveView.ViewType != ViewType.FloorPlan)
                {
                    System.Windows.MessageBox.Show("This command can only be executed in a floor plan view.", "Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return Result.Succeeded;
                }
                
                app.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>( HandleFailures! ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026" ), "RJ_2D_凡_配管付属品_桝_断面") ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "桝詳細項目タグ" ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "00_RJ_タグ_配管付属品_CL15" ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "08070_公共桝" ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "08060_ため桝_RC" ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "08050_インバート桝_SC" ) ;
                LoadSymbolAndTag( rvtDoc, Path.Combine( folder, "Resources", "rfa2026"), "08060_トラップ桝" ) ;
                if ( ! IsDetailFamilyLoaded( rvtDoc, "RJ_2D_凡_配管付属品_桝_断面" ) )
                    return Result.Cancelled ;
                var topoSolids = new FilteredElementCollector( rvtDoc ).OfClass( typeof( Toposolid ) )
                    .OfType<Toposolid>().ToList() ;
                var topoSurfaces = new FilteredElementCollector( rvtDoc ).OfClass( typeof( TopographySurface ) )
                    .OfType<TopographySurface>().ToList() ;

                IList<MeshTriangle> topoTriMeshes = new List<MeshTriangle>() ;
                if ( topoSolids.Count > 0 )
                    foreach ( var tp in topoSolids )
                        GetTriMeshTopoSolid( app, tp, ref topoTriMeshes ) ;
                else {
                    if ( topoSurfaces.Count > 0 )
                        foreach ( var ts in topoSurfaces )
                            GetTriMeshTopoSurface( app, ts, ref topoTriMeshes ) ;
                }

                IList<Reference> pickedObj = new List<Reference>() ;
                List<Reference> filteredReferences = new List<Reference>() ;
                Category targetCategory = rvtDoc.Settings.Categories.get_Item( BuiltInCategory.OST_PipeAccessory ) ;

                pickedObj = rvtUiDoc.Selection.GetReferences() ;
                
                foreach ( Reference reference in pickedObj ) {
                    Element element = rvtDoc.GetElement( reference.ElementId ) ;
                    if ( element.Category != null && element.Category.Id == targetCategory.Id ) {
                        filteredReferences.Add( reference ) ;
                    }
                }
                //複数桝を選択したかどうかチェックする
                pickedObj = filteredReferences ;
                if ( pickedObj.Count <= 1 )
                    pickedObj = rvtUiDoc.Selection.PickObjects( Revit.UI.Selection.ObjectType.Element, pipAsFilter, "Select manholes.") ;
                
                //桝パラメータを取得する
                var masus = ManholeUtl.GetManholeParameterFromRef( rvtUiDoc, pickedObj ) ;

                //パラメータを取得できなかった場合、処理を終了する
                if(masus.Count <= 1 )
                {
                    System.Windows.MessageBox.Show("Failed to retrieve target parameters. Please check the mapping.", "Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return Result.Succeeded;
                }

                using ( TransactionGroup tx = new TransactionGroup( rvtDoc, "REXJ MEP Manhole Tool: Tag Creation") ) {
                    tx.Start() ;

                    //Create tag on first time
                    if ( !ManholeUtl.SetSymbol( rvtUiDoc, masus, null ) ) 
                    {
                        tx.Assimilate();
                        return Result.Succeeded; 
                    }

                    //桝２件以上選択したのみ、処理を進める
                    if ( masus.Count >= 2 ) {
                        var masuView = new ListMasuView() ;
                        var masuViewModel = new ManholeViewModel( rvtUiDoc, masus ) ;
                        var mainWin = MainWindowHelper.GetRevitMainWindow( rvtUiApp ) ;
                        MepModelLineCommandHandler mepHandler = new MepModelLineCommandHandler() ;
                        ExternalEvent exEvent = ExternalEvent.Create(mepHandler);
                        
                        masuViewModel.RvtDoc = rvtDoc ;
                        masuViewModel.RvtUiDoc = rvtUiDoc ;
                        masuViewModel.UiApp = rvtUiApp ;
                        masuViewModel.TopoTriMeshes = topoTriMeshes ;
                        masuViewModel.MepHandler = mepHandler ;
                        masuViewModel.ExEvent = exEvent ;
                        masuView.DataContext = masuViewModel ;
                        masuView.Owner = mainWin ;
                        masuView.Show() ;
                        //実行後に親画面に戻す
                        mainWin.Show() ;
                    }

                    tx.Assimilate() ;
                }

                return Result.Succeeded ;
            }
            catch (Exception ex)
            {
                if (ex is Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Succeeded;
                }
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }
        
        /// <summary>
        /// Inaccurate Line 警告を削除する 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFailures(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor fa = e.GetFailuresAccessor();
            
            IList<FailureMessageAccessor> failList = fa.GetFailureMessages();
            foreach (FailureMessageAccessor failure in failList)
            { 
                FailureDefinitionId failId = failure.GetFailureDefinitionId();
                if (failId == BuiltInFailures.InaccurateFailures.InaccurateLine || failId == BuiltInFailures.DimensionFailures.LinearConstraintNotParallel || failId == BuiltInFailures.GeneralFailures.ErrorInSymbolFamilyResolved)
                {
                    fa.DeleteWarning(failure);
                }
            }
        }

        /// <summary>
        /// 地形ソリッドから三角メッシュを取得
        /// </summary>
        /// <param name="toposolid"></param>
        /// <param name="triMeshes"></param>
        public void GetTriMeshTopoSolid(Application application, Toposolid toposolid, ref IList<Revit.DB.MeshTriangle> triMeshes)
        {
            var options = application.Create.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            var geoElement = toposolid.get_Geometry(options);

            // Get geometry object
            foreach (GeometryObject geoObject in geoElement)
            {
                Solid? solid = geoObject as Solid;
                if (solid != null)
                {
                    foreach (Face face in solid.Faces)
                    {
                        Mesh mesh = face.Triangulate();
                        

                        if (mesh != null)
                        {
                            // Triangles
                            for (int i = 0; i < mesh.NumTriangles; ++i)
                            {
                                MeshTriangle triangle = mesh.get_Triangle(i);
                                if (triangle != null)
                                {
                                    triMeshes.Add(triangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetTriMeshTopoSurface(Application application, TopographySurface toposurface,
            ref IList<Revit.DB.MeshTriangle> triMeshes)
        {
            var options = application.Create.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            var geoElement = toposurface.get_Geometry(options);
            
            // Get geometry object
            foreach (GeometryObject geoObject in geoElement)
            {
                Solid? solid = geoObject as Solid;
                if (solid != null)
                {
                    foreach (Face face in solid.Faces)
                    {
                        Mesh mesh = face.Triangulate();
                        

                        if (mesh != null)
                        {
                            // Triangles
                            for (int i = 0; i < mesh.NumTriangles; ++i)
                            {
                                MeshTriangle triangle = mesh.get_Triangle(i);
                                if (triangle != null)
                                {
                                    triMeshes.Add(triangle);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static bool IsDetailFamilyLoaded(Document doc, string familyName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Family));
            var family = collector.Cast<Family>().FirstOrDefault(f => f.Name == familyName);
            return family != null;
        }

        private static void LoadSymbolAndTag(Document doc, string folder, string familyFileName)
        {
            string familyFile = folder + "\\" + familyFileName + ".rfa";
            
            string familyName = System.IO.Path.GetFileNameWithoutExtension(familyFile);
            var lstFamilyInProject = new FilteredElementCollector(doc).OfClass(typeof(Family))
                .Cast<Family>().Where(x => x.Name == familyName).ToList();
            if (lstFamilyInProject.Count == 0)
            {
                using (Transaction tran = new Transaction(doc, familyFileName))
                {
                    tran.Start();
                    Family family = null;
                    doc.LoadFamily(familyFile, out family);
                    tran.Commit();
                }
            }
        }
    }
}