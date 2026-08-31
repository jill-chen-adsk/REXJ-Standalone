using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Printing;
using ADSK.JExtRAC.PrintRegion.Utils;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using ADSK.JExtRAC.PrintRegion.Entities;

namespace ADSK.JExtRAC.PrintRegion.Components
{
    /// ================================================================================
    /// <summary>Elements</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public class Elements
    {
        // Member variables

        #region Member variables

        /// <summary>Distance offset</summary>
        private const double distance = 1000;
        /// <summary>誤差定数</summary>
        private const double tolerance = 0.001;

        /// <summary>Unique name of view</summary>
        private const string _viewNameUnique = "RangePrintPreview";

        private readonly UIDocument _rvtUIDoc;
        public Document RvtDBDoc { get; }

        #endregion Member variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="rvtUIDoc">UIDocument</param>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================
        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
        }

        #endregion Constructor

        // Member functions

        #region Member functions

        /// ================================================================================
        /// <summary>Data scale</summary>
        ///
        /// <returns>Data table scale</returns>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================
        public System.Data.DataTable GetDataScale()
        {
            bool isImperial = ViewScaleHelper.IsImperial(RvtDBDoc);
            return ViewScaleHelper.CreateScaleDataTable(isImperial);
        }

        /// ================================================================================
        /// <summary>Set Information View</summary>
        ///
        /// <param name="uiDoc">UIDocument</param>
        /// <param name="viewPrint">View</param>
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <param name="scaleView">view scale</param>
        /// <returns></returns>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================
        public bool SetInfomationView(
            UIDocument uiDoc,
            View viewPrint,
            XYZ p1,
            XYZ p2,
            int scaleView,
            XYZ modelPickMin = null,
            XYZ modelPickMax = null)
        {
            try
            {
                if (viewPrint == null)
                    return false;

                XYZ cropMin = p1;
                XYZ cropMax = p2;
                if (modelPickMin != null && modelPickMax != null)
                    TryGetCropLocalBounds(viewPrint, modelPickMin, modelPickMax, out cropMin, out cropMax);

                TryClearViewTemplate(viewPrint);

                viewPrint.Scale = scaleView;
                viewPrint.CropBoxActive = true;
                viewPrint.CropBoxVisible = true;

                if (modelPickMin != null && modelPickMax != null)
                    TryApplyCropShape(viewPrint, modelPickMin, modelPickMax);

                ApplyCropBox(viewPrint, cropMin, cropMax);
                ApplyAnnotationCropOffsets(viewPrint);
                viewPrint.CropBoxVisible = false;

                uiDoc.Document.Regenerate();

                return true;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return false;
            }
        }

        private static void TryClearViewTemplate(View viewPrint)
        {
            try
            {
                if (viewPrint.ViewTemplateId != ElementId.InvalidElementId)
                    viewPrint.ViewTemplateId = ElementId.InvalidElementId;
            }
            catch (Exception)
            {
            }
        }

        private static void ApplyCropBox(View viewPrint, XYZ cropMin, XYZ cropMax)
        {
            BoundingBoxXYZ existingBox = viewPrint.CropBox;
            double minZ = existingBox.Min.Z;
            double maxZ = existingBox.Max.Z;
            double minX = Math.Min(cropMin.X, cropMax.X);
            double maxX = Math.Max(cropMin.X, cropMax.X);
            double minY = Math.Min(cropMin.Y, cropMax.Y);
            double maxY = Math.Max(cropMin.Y, cropMax.Y);

            viewPrint.CropBox = new BoundingBoxXYZ
            {
                Transform = existingBox.Transform,
                Min = new XYZ(minX, minY, minZ),
                Max = new XYZ(maxX, maxY, maxZ)
            };
        }

        private static void ApplyAnnotationCropOffsets(View viewPrint)
        {
            viewPrint.get_Parameter(BuiltInParameter.VIEWER_ANNOTATION_CROP_ACTIVE).Set(1);
            ViewCropRegionShapeManager manager = viewPrint.GetCropRegionShapeManager();
            double minOffset = UnitUtils.ConvertToInternalUnits(0.125, UnitTypeId.Inches);
            manager.BottomAnnotationCropOffset = minOffset;
            manager.TopAnnotationCropOffset = minOffset;
            manager.LeftAnnotationCropOffset = minOffset;
            manager.RightAnnotationCropOffset = minOffset;
        }

        private static void TryApplyCropShape(View viewPrint, XYZ modelPickMin, XYZ modelPickMax)
        {
            try
            {
                if (!TryBuildModelCropLoop(viewPrint, modelPickMin, modelPickMax, out CurveLoop cropLoop))
                    return;

                viewPrint.GetCropRegionShapeManager().SetCropShape(cropLoop);
            }
            catch (Exception)
            {
            }
        }

        private static XYZ ProjectToViewPlane(View view, XYZ point)
        {
            XYZ normal = view.ViewDirection;
            double distanceToPlane = (point - view.Origin).DotProduct(normal);
            return point - distanceToPlane * normal;
        }

        private static bool TryBuildModelCropLoop(View view, XYZ pickMin, XYZ pickMax, out CurveLoop loop)
        {
            loop = null;

            const double alpha = 1e5;
            XYZ p1 = ProjectToViewPlane(view, pickMin);
            XYZ p2 = ProjectToViewPlane(view, pickMax);

            Line horizontal = Line.CreateBound(
                p1 - alpha * view.RightDirection,
                p1 + alpha * view.RightDirection);
            XYZ corner3 = horizontal.Project(p2).XYZPoint;

            Line vertical = Line.CreateBound(
                p1 - alpha * view.UpDirection,
                p1 + alpha * view.UpDirection);
            XYZ corner4 = vertical.Project(p2).XYZPoint;

            loop = new CurveLoop();
            loop.Append(Line.CreateBound(p1, corner3));
            loop.Append(Line.CreateBound(corner3, p2));
            loop.Append(Line.CreateBound(p2, corner4));
            loop.Append(Line.CreateBound(corner4, p1));
            return true;
        }

        /// <summary>
        /// 指定領域からはみ出す通り芯を指定領域境界までトリム
        /// </summary>
        /// <param name="uiDoc">UIドキュメント</param>
        /// <param name="view">複製ビュー</param>
        /// <param name="min">指定領域のmin頂点位置(ビュー断面に投影済)</param>
        /// <param name="max">指定領域のmax頂点位置(ビュー断面に投影済)</param>
        public void TrimGrid(UIDocument uiDoc, View view, XYZ min, XYZ max)
        {
            // ビューに表示されている通り芯を全て取得
            Document doc = uiDoc.Document;
            LogicalOrFilter filter = new LogicalOrFilter(new List<ElementFilter>()
            {
                new ElementCategoryFilter(BuiltInCategory.OST_Grids),
                new ElementCategoryFilter(BuiltInCategory.OST_GridChains),
            });
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id).WherePasses(filter).WhereElementIsNotElementType();
            IList<Element> elements = collector.ToElements();

            // 指定領域の4辺を表すLineを作成(ビュー断面に投影済)
            Line top = Line.CreateBound(new XYZ(min.X, max.Y, min.Z), new XYZ(max.X, max.Y, max.Z));
            Line bottom = Line.CreateBound(new XYZ(min.X, min.Y, min.Z), new XYZ(max.X, min.Y, max.Z));
            Line left = Line.CreateBound(new XYZ(min.X, min.Y, min.Z), new XYZ(min.X, max.Y, max.Z));
            Line right = Line.CreateBound(new XYZ(max.X, min.Y, min.Z), new XYZ(max.X, max.Y, max.Z));

            // ビューに表示されている全ての通り芯IDを取得(複数セグメント通り芯は、中身の単一セグメント通り芯のIDを取得)
            List<ElementId> ids = new List<ElementId>();
            foreach (Element element in elements)
            {
                if (element.Category.Id.ToString() == ((int)BuiltInCategory.OST_Grids).ToString())
                {
                    // 単一セグメント通り芯の場合、親となる複数セグメント通り芯を検索
                    Grid grid = element as Grid;
                    ElementId segmentId = MultiSegmentGrid.GetMultiSegementGridId(grid);
                    if (segmentId != ElementId.InvalidElementId)
                    {
                        Element segment = doc.GetElement(segmentId);
                        MultiSegmentGrid multiGrid = segment as MultiSegmentGrid;
                        ICollection<ElementId> gridIds = multiGrid.GetGridIds();
                        foreach (ElementId id in gridIds)
                        {
                            Grid childGrid = doc.GetElement(id) as Grid;
                            if (childGrid.CanBeVisibleInView(view) && !ids.Contains(id))
                                ids.Add(id);
                        }
                    }
                    else
                    {
                        if (grid.CanBeVisibleInView(view) && !ids.Contains(grid.Id))
                            ids.Add(grid.Id);
                    }
                }
                else
                {
                    MultiSegmentGrid multiGrid = element as MultiSegmentGrid;
                    ICollection<ElementId> gridIds = multiGrid.GetGridIds();
                    foreach (ElementId id in gridIds)
                    {
                        Grid childGrid = doc.GetElement(id) as Grid;
                        if (childGrid.CanBeVisibleInView(view) && !ids.Contains(id))
                            ids.Add(id);
                    }
                }
            }
            foreach (ElementId id in ids)
            {
                Element element = doc.GetElement(id);
                Grid grid = element as Grid;
                TrimGridByCurve(doc, view, grid, top, isTop: true);
                TrimGridByCurve(doc, view, grid, bottom, isBottom: true);
                TrimGridByCurve(doc, view, grid, left, isLeft: true);
                TrimGridByCurve(doc, view, grid, right, isRight: true);
            }
        }

        public void HideElements(UIDocument uiDoc, View view)
        {
            Document doc = uiDoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_Viewers).WhereElementIsNotElementType();
            IList<Element> ids = collector.Where(p => !p.IsHidden(view) && p.CanBeHidden(view)).ToList();
            if (ids.Count > 0)
                view.HideElements(ids.Select(p => p.Id).ToList());
        }

        private double SignedDistanceTo(Plane plane, XYZ p)
        {
            XYZ v = p - plane.Origin;
            return plane.Normal.DotProduct(v);
        }
        private XYZ ProjectPointOntoPlane(Plane plane, XYZ p)
        {
            double d = SignedDistanceTo(plane, p);
            return p - d * plane.Normal;
        }

        /// <summary>
        /// Lineに合わせて通り芯をトリム
        /// </summary>
        /// <param name="doc">Revitドキュメント</param>
        /// <param name="view">現在のビュー</param>
        /// <param name="grid">通り芯</param>
        /// <param name="line">トリム境界線Line(ビュー断面に投影済)</param>
        /// <param name="isTop">指定領域の上端</param>
        /// <param name="isBottom">指定領域の下端</param>
        /// <param name="isLeft">指定領域の左端</param>
        /// <param name="isRight">指定領域の右端</param>
        /// <returns></returns>
        private bool TrimGridByCurve(Document doc, View view, Grid grid, Line line,
            bool isTop = false, bool isBottom = false, bool isLeft = false, bool isRight = false)
        {
            // トリミング必要
            bool needTrim = false;

            Plane plane = Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);

            Curve bdCrv = line as Curve;
            DatumPlane datum = doc.GetElement(grid.Id) as DatumPlane;
            Curve gdCrv = datum.GetCurvesInView(DatumExtentType.ViewSpecific, view)?.First();
            if (gdCrv == null)
                return false;

            Transform gdTrf;
            if (view is ViewPlan)
            {
                gdCrv = Project(gdCrv, plane, out gdTrf);
                bdCrv = Project(bdCrv, plane);
            }
            else
            {
                TransformCurveByCropbox(view, gdCrv, out gdCrv, out gdTrf);
            }
            Transform backTrf = gdTrf.Inverse; // 新たな終点を3D原点座標に変換する用

            // トリム境界線と通り芯の交点を求める（2つの線が重ならない場合や、交点が2つ以上の場合、トリム処理をスキップ）
            Curve gdTestCrv = gdCrv.Clone();
            //gdTestCrv.MakeUnbound();
            using (CurveIntersectResult intersectResult = gdTestCrv.Intersect(bdCrv, CurveIntersectResultOption.Detailed))
            {
                IList<CurveOverlapPoint> overlaps = intersectResult.GetOverlaps();
                if (intersectResult.Result == SetComparisonResult.Overlap &&
                    overlaps != null && overlaps.Count == 1)
                {
                CurveOverlapPoint overlapPt = overlaps[0];
                // トリム境界線と通り芯の交点を、通り芯の新たな終点とする
                XYZ newEnd = overlapPt.Point;
                double paramStart = gdTestCrv.GetEndParameter(0);
                double paramEnd = gdTestCrv.GetEndParameter(1);
                double paramNewEnd = overlapPt.FirstParameter;
                // paramStart >= paramNewEnd >= paramEnd OR paramEnd >= paramNewEnd >= paramStart
                needTrim = (paramStart - paramNewEnd >= 0 && paramNewEnd - paramEnd >= 0) || (paramEnd - paramNewEnd >= 0 && paramNewEnd - paramStart >= 0);

                // 通り芯の方向とビューのUpDirection, RightDirectionから算出
                // トリミングする側
                // trueの場合、始点-交差点
                // falseの場合,交差点-終点
                bool trimStartSide = false;
                XYZ gdDir = GetGridDir(doc, grid, overlapPt.Point);
                TransformPoint(view, gdDir, out XYZ gdDirOnView);
                if (isTop    && IsMoreThanOrEqualTo(0, gdDir.X * view.UpDirection.X    + gdDir.Y * view.UpDirection.Y    + gdDir.Z * view.UpDirection.Z   )    || // 通り芯方向に、ビューのUpDirectionのマイナス成分を含む
                    isBottom && IsMoreThanOrEqualTo(   gdDir.X * view.UpDirection.X    + gdDir.Y * view.UpDirection.Y    + gdDir.Z * view.UpDirection.Z   , 0) || // 通り芯方向に、ビューのUpDirectionのプラス成分を含む
                    isLeft   && IsMoreThanOrEqualTo(   gdDir.X * view.RightDirection.X + gdDir.Y * view.RightDirection.Y + gdDir.Z * view.RightDirection.Z, 0) || // 通り芯方向に、ビューのRightDirectionのマイナス成分を含む
                    isRight  && IsMoreThanOrEqualTo(0, gdDir.X * view.RightDirection.X + gdDir.Y * view.RightDirection.Y + gdDir.Z * view.RightDirection.Z))      // 通り芯方向に、ビューのRightDirectionのプラス成分を含む
                {
                    trimStartSide = paramStart - paramNewEnd >= 0 && paramNewEnd - paramEnd >= 0;
                }
                else
                {
                    trimStartSide = paramEnd - paramNewEnd >= 0 && paramNewEnd - paramStart >= 0;
                }

                if (needTrim)
                {
                    Curve newCrv = null;
                    DatumEnds dEnds = 0;
                    if (trimStartSide)
                    {
                        XYZ crvStart = gdCrv.GetEndPoint(0);
                        XYZ crvEnd = newEnd;
                        dEnds = DatumEnds.End1;
                        if (gdCrv is Arc)
                        {
                            double paramMid = (paramNewEnd + paramStart) / 2;
                            XYZ midPnt = gdTestCrv.Evaluate(paramMid, false);
                            newCrv = Arc.Create(crvStart, crvEnd, midPnt);
                            newCrv = newCrv.CreateTransformed(backTrf);
                        }
                        else
                        {
                            newCrv = Line.CreateBound(crvStart, crvEnd);
                            newCrv = newCrv.CreateTransformed(backTrf);
                        }
                    }
                    else
                    {
                        XYZ crvStart = newEnd;
                        XYZ crvEnd = gdCrv.GetEndPoint(1);
                        dEnds = 0;
                        if (gdCrv is Arc)
                        {
                            double paramMid = (paramEnd + paramNewEnd) / 2;
                            XYZ midPnt = gdTestCrv.Evaluate(paramMid, false);
                            newCrv = Arc.Create(crvStart, crvEnd, midPnt);
                            newCrv = newCrv.CreateTransformed(backTrf);
                        }
                        else
                        {
                            newCrv = Line.CreateBound(crvStart, crvEnd);
                            newCrv = newCrv.CreateTransformed(backTrf);
                        }
                    }
                    if (grid.GetDatumExtentTypeInView(dEnds, view) != DatumExtentType.ViewSpecific)
                    {
                        grid.SetDatumExtentType(dEnds, view, DatumExtentType.ViewSpecific);
                    }
                    if (newCrv != null)
                        grid.SetCurveInView(DatumExtentType.ViewSpecific, view, newCrv);
                }
                }
            }
            return needTrim;
        }

        /// <summary>
        /// 通り芯の方向を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="grid">通り芯</param>
        /// <param name="newEndPt">通り芯と印刷範囲の交点</param>
        /// <returns></returns>
        private XYZ GetGridDir(Document doc, Grid grid, XYZ newEndPt)
        {
            if (grid.Curve is Line line)
            {
                GeometryElement ge = grid.get_Geometry(new Options() { ComputeReferences = true, View = doc.ActiveView });
                foreach (GeometryObject go in ge)
                {
                    if (go is Line li)
                        return li.Direction;
                }
                return line.Direction;
            }
            else
            {
                // 曲線の場合は、始端と交点を結んだ直線の方向を返却
                return Line.CreateBound(grid.Curve.GetEndPoint(0), newEndPt).Direction;
            }
        }

        /// <summary>
        /// 誤差の許容範囲
        /// </summary>
        public static double EPSILON = 1.0e-9;

        /// <summary>
        /// double型の値が等しいとみなせるか確認
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualTo(double d1, double d2)
        {
            double diiference = Math.Abs(d1 - d2);

            if (diiference < EPSILON)
                return true;
            else
                return false;
        }

        /// <summary>
        /// double型の値d1がd2以上とみなせるか確認
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsMoreThanOrEqualTo(double d1, double d2)
        {
            if (IsAlmostEqualTo(d1, d2))
                return true;

            return d1 > d2;
        }

        private Curve Project(Curve crv, Plane pl)
        {
            return Project(crv, pl, out Transform trf);
        }

        private Curve Project(Curve crv, Plane pl, out Transform trf)
        {
            XYZ end = crv.GetEndPoint(0);
            Project(end, pl, out XYZ dir);
            trf = Transform.CreateTranslation(dir);
            return crv.CreateTransformed(trf);
        }

        private XYZ Project(XYZ pnt, Plane pl, out XYZ dir)
        {
            Line ln = Line.CreateUnbound(pl.Origin, pl.Normal);
            XYZ pntPrj = ln.Project(pnt).XYZPoint;
            dir = pl.Origin - pntPrj;
            return pnt + dir;
        }

        /// <summary>
        /// ビューのCropBoxのtransform.InverseでCurveを変換
        /// </summary>
        /// <param name="view"></param>
        /// <param name="crvIn"></param>
        /// <param name="crvOut"></param>
        /// <param name="trf"></param>
        /// <returns></returns>
        private bool TransformCurveByCropbox(View view, Curve crvIn, out Curve crvOut, out Transform trf)
        {
            crvOut = null;
            trf = null;

            // Get the view crop box
            BoundingBoxXYZ bb = view.CropBox;
            if (bb != null)
            {
                Transform transform = bb.Transform;
                trf = transform.Inverse;
                crvOut = crvIn.CreateTransformed(trf);
            }
            return null != crvOut;
        }

        /// <summary>
        /// Delete view has been created for preview and set preview active view
        /// </summary>
        /// <param name="_entData">View created</param>
        /// <returns></returns>
        public bool DeleteViewCreatedAndSetActiveView(EntitiesData _entData)
        {
            if (_entData == null || _entData._viewDuplicate == null || _entData._viewDuplicate.IsValidObject == false || _entData._rvtUIApp == null)
                return false;

            try
            {
                // Set active view
                _entData._rvtUIApp.ActiveUIDocument.ActiveView = _entData._viewCurrent;

                // Start transaction
                Transaction tr = new Transaction(_entData._rvtUIApp.ActiveUIDocument.Document);
                tr.Start("Delete view created");

                // Delete duplicate view
                _entData._rvtUIApp.ActiveUIDocument.Document.Delete(_entData._viewDuplicate.Id);

                // Commit transaction
                tr.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }

            return false;
        }

        /// <summary>
        /// Duplicate current view
        /// </summary>
        /// <param name="rvtUIDoc"></param>
        /// <returns></returns>
        public View DuplicateView(UIDocument rvtUIDoc)
        {
            // Get current view
            View currentView = rvtUIDoc.Document.ActiveView;
            if (currentView == null)
                return null;

            // Create new view
            View createdView = rvtUIDoc.Document.GetElement(currentView.Duplicate(ViewDuplicateOption.WithDetailing)) as View;
            if (createdView == null)
                return null;

            // Get new name of view
            var newNameView = GetUniqueNameView(rvtUIDoc.Document, _viewNameUnique);
            if (createdView.Name != newNameView)
                createdView.Name = newNameView;

            return createdView;
        }

        public string GetUniqueNameView(Document doc, string viewName)
        {
            string Name = string.Empty;
            var lstViewName = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)).Select(x => x.Name).ToList();

            string realName = viewName;
            string nameTemp = string.Empty;

            if (lstViewName.Contains(viewName) == false)
                return viewName;

            int numFamily = 1;
            while (true)
            {
                nameTemp = string.Format("{0}{1}", viewName, numFamily);
                if (lstViewName.Contains(nameTemp))
                    numFamily++;
                else
                {
                    realName = nameTemp;
                    break;
                }
            }

            return realName;
        }

        /// ================================================================================
        /// <summary>PickBox Region Print</summary>
        ///
        /// <param name="cmpAttribute">Attribute</param>
        /// <param name="uiDoc">UIDocument</param>
        /// <param name="p1">out value first point(ビュー断面に投影済)</param>
        /// <param name="p2">out value second point(ビュー断面に投影済)</param>
        /// <returns></returns>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================
        public bool PickPoints(
            Attribute cmpAttribute,
            UIDocument uiDoc,
            out XYZ p1,
            out XYZ p2,
            out XYZ modelPickMin,
            out XYZ modelPickMax)
        {
            p1 = XYZ.Zero;
            p2 = XYZ.Zero;
            modelPickMin = XYZ.Zero;
            modelPickMax = XYZ.Zero;

            Transaction t = new Transaction(uiDoc.Document);
            try
            {
                //3Dビューの場合は、作業面を強制的にビュー断面に合わせる
                if (uiDoc.Document.ActiveView.ViewType == ViewType.ThreeD) {
                    t.Start("Temporarily set work plane");
                    Plane plane = Plane.CreateByNormalAndOrigin(uiDoc.Document.ActiveView.ViewDirection, uiDoc.Document.ActiveView.Origin);
                    SketchPlane sp = SketchPlane.Create(uiDoc.Document, plane);
                    uiDoc.Document.ActiveView.SketchPlane = sp;
                    t.Commit();
                }

                // 印刷領域を2点指定(ビュー方向に垂直な作業面上)
                //pick box region
                PickedBox pickedBox = uiDoc.Selection.PickBox(PickBoxStyle.Directional, cmpAttribute.ResourceText("IDS_TXT_PICK_AREA"));
                if (pickedBox == null)
                    return false;

                // 印刷領域のXY最小・最大を算出(ビュー方向に垂直な作業面上)
                if ((pickedBox.Min.X == pickedBox.Max.X) && (pickedBox.Min.Y == pickedBox.Max.Y) && (pickedBox.Min.Z == pickedBox.Max.Z))
                {
                    //Can not create view with same point
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SAME_POINT"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    return false;
                }

                modelPickMin = pickedBox.Min;
                modelPickMax = pickedBox.Max;
                View activeView = uiDoc.Document.ActiveView;

                return TryGetCropLocalBounds(activeView, modelPickMin, modelPickMax, out p1, out p2);
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                return false;
            }
            finally
            {
                if (t.HasStarted())
                    t.RollBack();
            }
        }

        /// <summary>
        /// Converts a screen pick box to crop-local min/max using all four view-oriented corners.
        /// </summary>
        public bool TryGetCropLocalBounds(View view, XYZ pickMin, XYZ pickMax, out XYZ cropMin, out XYZ cropMax)
        {
            cropMin = XYZ.Zero;
            cropMax = XYZ.Zero;

            const double alpha = 1e5;

            Line horizontal = Line.CreateBound(
                pickMin - alpha * view.RightDirection,
                pickMin + alpha * view.RightDirection);
            XYZ pickCorner3 = horizontal.Project(pickMax).XYZPoint;

            Line vertical = Line.CreateBound(
                pickMin - alpha * view.UpDirection,
                pickMin + alpha * view.UpDirection);
            XYZ pickCorner4 = vertical.Project(pickMax).XYZPoint;

            if (!TransformPoint(view, pickMin, out XYZ local1) ||
                !TransformPoint(view, pickMax, out XYZ local2) ||
                !TransformPoint(view, pickCorner3, out XYZ local3) ||
                !TransformPoint(view, pickCorner4, out XYZ local4))
            {
                return false;
            }

            double minX = Math.Min(Math.Min(local1.X, local2.X), Math.Min(local3.X, local4.X));
            double maxX = Math.Max(Math.Max(local1.X, local2.X), Math.Max(local3.X, local4.X));
            double minY = Math.Min(Math.Min(local1.Y, local2.Y), Math.Min(local3.Y, local4.Y));
            double maxY = Math.Max(Math.Max(local1.Y, local2.Y), Math.Max(local3.Y, local4.Y));

            cropMin = new XYZ(minX, minY, -1000);
            cropMax = new XYZ(maxX, maxY, 0);
            return true;
        }

        /// <summary>Transform a point by crop box of view</summary>
        /// <param name="view">View</param>
        /// <param name="pointIn">Point input</param>
        /// <param name="pointOut">Point output</param>
        /// <returns></returns>
        private bool TransformPoint(View view, XYZ pointIn, out XYZ pointOut)
        {
            pointOut = null;

            // Get the view crop box
            BoundingBoxXYZ bb = view.CropBox;
            if (bb != null) {
                Transform transform = bb.Transform;
                Transform transformInverse = transform.Inverse;
                pointOut = transformInverse.OfPoint(pointIn);
            }
            return null != pointOut;            
        }

        #endregion Member functions
    }
}