using ADSK.JExtRAC.AutoCreateDimension.UI;
using Autodesk.Revit.DB;
using ResText = ADSK.JExtRAC.AutoCreateDimension.Resources.Text;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Form = System.Windows.Forms.Form;
using RvtExtApp = ADSK.JExtRAC.AutoCreateDimension;

namespace ADSK.JExtRAC.AutoCreateDimension.Screen
{
    /// ================================================================================
    /// <summary>画面 寸法作成</summary>
    /// ================================================================================
    public partial class AutoDimension : Form
    {
        /// <summary>
        /// コマンドデータ
        /// </summary>
        public static ExternalCommandData CommandData;

        /// <summary>
        /// Revitアプリケーション
        /// </summary>
        public static UIApplication UiApp;

        /// <summary>
        /// アプリケーション
        /// </summary>
        public static Autodesk.Revit.ApplicationServices.Application App;

        /// <summary>
        /// アクティブなUIドキュメント
        /// </summary>
        public static UIDocument UiDoc;

        /// <summary>
        /// Revitドキュメント
        /// </summary>
        public static Document Doc;

        /// <summary>
        /// 要素リスト(LocationCurveがnullでない、かつ、FamilyInstance要素以外、および、通り芯)
        /// </summary>
        private List<Element> elementList = new List<Element>();

        /// <summary>
        /// 点配置要素リスト(LocationCurveがnull、または、FamilyInstance要素)
        /// </summary>
        private List<Element> pointSetList = new List<Element>();

        /// <summary>
        /// 方向リスト
        /// </summary>
        private List<XYZ> dirListXY = new List<XYZ>();

        /// <summary>
        /// 要素セットリスト(同じ方向ごとにまとめた要素セットのリスト)
        /// </summary>
        private List<HashSet<Element>> elementSetList = new List<HashSet<Element>>();

        /// <summary>
        /// 要素方向ディクショナリー
        /// </summary>
        private Dictionary<XYZ, HashSet<Element>> elementDirDic = new Dictionary<XYZ, HashSet<Element>>();

        /// <summary>
        /// ビュー方向のフラグ
        /// </summary>
        public XYZ dimensionPoint = new XYZ(0, 0, 0);

        /// <summary>
        /// 数値許容誤差
        /// </summary>
        private const double _eps = 1.0e-9;

        /// <summary>
        /// XYZリスト
        /// </summary>
        public static List<XYZ> xyzList = new List<XYZ>();

        /// <summary>
        /// エラーID
        /// </summary>
        private string errorId = "";

        /// <summary>
        /// エラーIDセット
        /// </summary>
        private HashSet<string> errorIdSet = new HashSet<string>();

        /// <summary>
        /// 寸法作成成功IDセット
        /// </summary>
        private HashSet<string> successIdSet = new HashSet<string>();

        /// <summary>
        /// 作成線分リスト
        /// </summary>
        private List<Line> createLineList = new List<Line>();

        /// <summary>
        /// 作成線分リスト
        /// </summary>
        private List<ElementId> elementIdListXY = new List<ElementId>();

        /// <summary>
        /// 選択処理制御
        /// </summary>
        public bool _isSelectPoint;

        /// <summary>
        /// 点指定制御
        /// </summary>
        public bool _isPoint;

        /// ================================================================================
        /// <summary>コンストラクト</summary>
        /// <param name="commandData">コマンドデータ</param>
        /// <param name="elementIdList">要素IDリスト</param>
        /// ================================================================================
        public AutoDimension(ExternalCommandData commandData, ICollection<ElementId> elementIdList)
        {
            InitializeComponent();
            ApplyLocalizedFormText();

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;
            View activeView = Doc.ActiveView;
            XYZ viewDirection = activeView.ViewDirection;
            xyzList = new List<XYZ>();

            // アクティブビューが3Dビューの場合、詳細線分作成チェックボックスを非活性
            if (activeView.ViewType == ViewType.ThreeD) {
                lineCheck.Enabled = false;
            }
            XYZ viewDir = activeView.ViewDirection;
            // XY平面の場合
            if (Math.Abs(viewDir.Z) == 1) {
                foreach (ElementId id in elementIdList) {
                    elementIdListXY.Add(id); ;
                }
            }
            // XY平面以外の場合
            else {
                elementList = new List<Element>();

                // 対象となる要素を抽出
                foreach (ElementId id in elementIdList) {
                    Element ele = Doc.GetElement(id);
                    if (ele.GetType().Name == "Group") {
                        continue;
                    }
                    if (ele.Category.Name == "文字注記") {
                        continue;
                    }
                    if (ele.Category.Name == "ビュー") {
                        continue;
                    }
                    if (ele.Category.Name == "立面図") {
                        continue;
                    }
                    if (ele.Category.Name == "プロジェクト基準点") {
                        continue;
                    }
                    if (ele.Category.Name == "寸法") {
                        continue;
                    }
                    if (ele.Category.Name == "参照図") {
                        continue;
                    }
                    if (ele.GetType().Name == "MultiSegmentGrid") {
                        // 単体の通り芯として扱う
                        MultiSegmentGrid grids = (MultiSegmentGrid)ele;
                        ICollection<ElementId> ids = grids.GetGridIds();
                        foreach (ElementId gridId in ids) {
                            Element grid = Doc.GetElement(gridId);
                            elementList.Add(grid);
                        }
                    }
                    else if (ele.GetType().Name == "Grid") {
                        bool singleFlag = true;
                        string gridName = ele.Name;
                        List<MultiSegmentGrid> multiGridList = GetElements<MultiSegmentGrid>(Doc);
                        foreach (MultiSegmentGrid multi in multiGridList) {
                            if (multi.Name == gridName) {
                                singleFlag = false;
                                ICollection<ElementId> ids = multi.GetGridIds();
                                foreach (ElementId gridId in ids) {
                                    Element grid = Doc.GetElement(gridId);
                                    elementList.Add(grid);
                                }
                            }
                        }
                        if (singleFlag) {
                            elementList.Add(ele);
                        }
                    }
                    else {
                        if (ele.Location as LocationCurve == null || ele.GetType().Name == "FamilyInstance") {
                            pointSetList.Add(ele);
                        }
                        else {
                            elementList.Add(ele);
                        }
                    }
                }

                // 処理対象要素セットに追加済みの要素を一時的に格納する要素セット
                HashSet<ElementId> continueSet = new HashSet<ElementId>();

                // 要素リスト(LocationCurveがnullでない、かつ、FamilyInstance要素以外、および、通り芯)について処理対象要素を確認
                foreach (Element element1 in elementList) {
                    // 処理対象とする要素セット
                    HashSet<Element> elementSet = new HashSet<Element>();
                    // 処理対象要素セットに追加済みの要素セットに含まれていたらスキップ
                    if (continueSet.Contains(element1.Id)) {
                        continue;
                    }
                    // あらかじめひとつ目の要素(element1)は処理対象とする処理対象とする要素セットに追加しておく
                    else {
                        elementSet.Add(element1);
                    }

                    // ひとつ目の要素(element1)と総当たり
                    foreach (Element element2 in elementList) {
                        if (element1.Id == element2.Id) {
                            continue;
                        }

                        // 方向リストを作成
                        LocationCurve locationCurve1 = element1.Location as LocationCurve;
                        Curve curve1 = null;
                        List<XYZ> directionList1 = new List<XYZ>();
                        List<XYZ> directionList2 = new List<XYZ>();

                        ///// ひとつ目の要素(element1)について /////
                        // LocationCurveがあればそのCurveをチェック
                        if (locationCurve1 != null) {
                            curve1 = locationCurve1.Curve;
                        }
                        // GridならそのCurveをチェック
                        else if (element1.GetType().Name == "Grid") {
                            Grid grid = (Grid)element1;
                            curve1 = grid.Curve;
                        }
                        // FamilyInstanceなら要素のXYZ軸を方向リスト1に追加
                        else if (element1.GetType().Name == "FamilyInstance") {
                            FamilyInstance instance = (FamilyInstance)element1;
                            Transform transform = instance.GetTransform();
                            XYZ zero = new XYZ(0, 0, 0);
                            XYZ baseX = transform.BasisX;
                            Line lineX = Line.CreateBound(zero, baseX);
                            directionList1.Add(lineX.Direction);
                            XYZ baseY = transform.BasisY;
                            Line lineY = Line.CreateBound(zero, baseY);
                            directionList1.Add(lineY.Direction);
                            XYZ baseZ = transform.BasisZ;
                            Line lineZ = Line.CreateBound(zero, baseZ);
                            directionList1.Add(lineZ.Direction);
                        }

                        // チェックしてLine(直線)であれば、方向リスト1に追加
                        Line line1 = null;
                        if (curve1 != null && curve1.GetType().Name == "Line") {
                            line1 = (Line)curve1;
                            directionList1.Add(line1.Direction);
                        }

                        ///// ふたつ目の要素(element2)について /////
                        LocationCurve locationCurve2 = element2.Location as LocationCurve;
                        Curve curve2 = null;
                        // LocationCurveがあればそのCurveをチェック
                        if (locationCurve2 != null) {
                            curve2 = locationCurve2.Curve;
                        }
                        // GridならそのCurveをチェック
                        else if (element2.GetType().Name == "Grid") {
                            Grid grid = (Grid)element2;
                            curve2 = grid.Curve;
                        }
                        // FamilyInstanceなら要素のXYZ軸を方向リスト2に追加
                        else if (element2.GetType().Name == "FamilyInstance") {
                            FamilyInstance instance = (FamilyInstance)element2;
                            Transform transform = instance.GetTransform();
                            XYZ zero = new XYZ(0, 0, 0);
                            XYZ baseX = transform.BasisX;
                            Line lineX = Line.CreateBound(zero, baseX);
                            directionList2.Add(lineX.Direction);
                            XYZ baseY = transform.BasisY;
                            Line lineY = Line.CreateBound(zero, baseY);
                            directionList2.Add(lineY.Direction);
                            XYZ baseZ = transform.BasisZ;
                            Line lineZ = Line.CreateBound(zero, baseZ);
                            directionList2.Add(lineZ.Direction);
                        }

                        // チェックしてLine(直線)であれば、方向リスト2に追加
                        Line line2 = null;
                        if (curve2 != null && curve2.GetType().Name == "Line") {
                            line2 = (Line)curve2;
                            directionList2.Add(line2.Direction);
                        }

                        // 方向リスト1と方向リスト2の中身を比較
                        foreach (XYZ lineDirection1 in directionList1) {
                            foreach (XYZ lineDirection2 in directionList2) {
                                Line l1 = Line.CreateBound(XYZ.Zero, lineDirection1);
                                Line l2 = Line.CreateBound(XYZ.Zero, lineDirection2);
                                XYZ p1 = NormalViewDirection(l1, viewDirection);
                                XYZ p2 = NormalViewDirection(l2, viewDirection);
                                // 平行かどうか
                                if (IsParallel(p1, p2)) {
                                    // 平行なものがあれば、ふたつ目の要素(element2)を処理対象とする要素セット
                                    elementSet.Add(element2);
                                    // 処理対象要素セットに追加済みの要素セットに、ふたつ目の要素(element2)を追加
                                    continueSet.Add(element2.Id);
                                }
                            }
                        }
                    }

                    // 処理対象とする要素セットに複数要素が含まれている場合
                    if (elementSet.Count > 1) {
                        // 処理対象とする要素セットに、点配置要素リスト(LocationCurveがnull、または、FamilyInstance要素)を追加
                        foreach (Element p in pointSetList) {
                            elementSet.Add(p);
                        }
                        // 処理対象とする要素セットを、要素セットリストに追加
                        elementSetList.Add(elementSet);
                    }
                    // 処理対象とする要素セットがひとつで、点配置要素リスト(LocationCurveがnull、または、FamilyInstance要素)がある場合
                    else if (elementSet.Count == 1 && pointSetList.Count > 0) {
                        // 処理対象とする要素セットに、点配置要素リスト(LocationCurveがnull、または、FamilyInstance要素)を追加
                        foreach (Element p in pointSetList) {
                            elementSet.Add(p);
                        }
                        // 処理対象とする要素セットを、要素セットリストに追加
                        elementSetList.Add(elementSet);
                    }
                    // 処理対象とする要素セットがひとつの場合
                    else if (elementSet.Count == 1) {
                        // 処理対象とする要素セットを、要素セットリストに追加
                        elementSetList.Add(elementSet);
                    }
                }
            }
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理。
        /// </summary>
        /// <param name="sender">イベントを送信したオブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            // 画面を閉じる
            Close();
        }

        /// <summary>
        /// 寸法記入ボタンクリック時の処理。
        /// </summary>
        /// <param name="sender">イベントを送信したオブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void DimensionButton_Click(object sender, EventArgs e)
        {
            // コマンド側で配置基準点の指定処理
            _isPoint = true;
            _isSelectPoint = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void ApplyLocalizedFormText()
        {
            this.Text = ResText.IDS_DIM_FORM_TITLE;
            this.groupBox1.Text = ResText.IDS_DIM_GROUPBOX_PLACEMENT_TYPE;
            this.faceCoreRadio.Text = ResText.IDS_DIM_RADIO_FACE_CORE;
            this.faceRadio.Text = ResText.IDS_DIM_RADIO_FACE_FACE;
            this.coreRadio.Text = ResText.IDS_DIM_RADIO_CORE_CORE;
            this.dimensionButton.Text = ResText.IDS_TXT_OK;
            this.cancelButton.Text = ResText.IDS_TXT_CANCEL;
            this.lineCheck.Text = ResText.IDS_DIM_CHECK_DETAIL_LINE_NOTE;
        }

        /// <summary>
        /// 寸法作成処理コマンド側での呼び出し用
        /// </summary>
        public void CreateDimension()
        {
            _isPoint = false;
            _isSelectPoint = false;
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            View activeView = Doc.ActiveView;
            XYZ viewDirection = activeView.ViewDirection;
            // XY平面について
            if (Math.Abs(viewDirection.Z) == 1) {
                CreateDimension_XY(activeView);
            }
            // XY平面以外
            else {
                CreateDimension_CrossSection();
            }
            // エラーメッセージ
            if (errorIdSet.Count > 0) {
                foreach (string id in successIdSet) {
                    errorIdSet.Remove(id);
                }
                foreach (string id in errorIdSet) {
                    if (errorId == "") {
                        errorId += id;
                    }
                    else {
                        errorId += ("; " + id);
                    }
                }
            }
            if (errorId != "") {
                // エラーメッセージの表示
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ResText.IDS_DIM_ERROR_FACES_CORES);
                sb.Append(errorId);
                RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
                FormLog frm = new FormLog(cmpAttribute, sb);
                frm.ShowDialog();
            }
            // 画面を閉じる
            Close();
        }

        /// <summary>
        /// ドキュメント内の要素を、クラスでフィルタリングして取得する。
        /// </summary>
        /// <typeparam name="T">フィルタリングするクラス</typeparam>
        /// <param name="doc">対象のドキュメント</param>
        /// <returns>クラスリスト</returns>
        public static List<T> GetElements<T>(Document doc)
        {
            Type type = typeof(T);

            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// XY平面の寸法作成前処理
        /// </summary>
        private void CreateDimension_XY(View activeView)
        {
            elementList = new List<Element>();
            // 要素リストの設定
            foreach (ElementId id in elementIdListXY) {
                Element ele = Doc.GetElement(id);
                if (ele.GetType().Name == "Group") {
                    continue;
                }
                if (ele.Category.Name == "文字注記") {
                    continue;
                }
                if (ele.Category.Name == "ビュー") {
                    continue;
                }
                if (ele.Category.Name == "立面図") {
                    continue;
                }
                if (ele.Category.Name == "プロジェクト基準点") {
                    continue;
                }
                if (ele.Category.Name == "寸法") {
                    continue;
                }
                if (ele.Category.Name == "参照面") {
                    continue;
                }
                if (ele.GetType().Name == "MultiSegmentGrid") {
                    // 単体の通り芯として扱う
                    MultiSegmentGrid grids = (MultiSegmentGrid)ele;
                    ICollection<ElementId> ids = grids.GetGridIds();
                    foreach (ElementId gridId in ids) {
                        Element grid = Doc.GetElement(gridId);
                        elementList.Add(grid);
                    }
                }
                else if (ele.GetType().Name == "Grid") {
                    // 単体のグリッドかどうか
                    bool singleFlag = true;
                    string gridName = ele.Name;
                    List<MultiSegmentGrid> multiGridList = GetElements<MultiSegmentGrid>(Doc);
                    foreach (MultiSegmentGrid multi in multiGridList) {
                        if (multi.Name == gridName) {
                            singleFlag = false;
                            ICollection<ElementId> ids = multi.GetGridIds();
                            foreach (ElementId gridId in ids) {
                                Element grid = Doc.GetElement(gridId);
                                elementList.Add(grid);
                            }
                        }
                    }
                    if (singleFlag) {
                        elementList.Add(ele);
                    }
                }
                else {
                    elementList.Add(ele);
                }
            }
            // 全方向取得
            List<XYZ> directionList = GetDirectionXY(elementList, activeView);
            dirListXY = directionList;
            foreach (XYZ directionXYZ in directionList) {
                HashSet<Element> elementSet = new HashSet<Element>();
                foreach (Element element1 in elementList) {
                    LocationCurve locationCurve1 = element1.Location as LocationCurve;
                    Curve curve1 = null;
                    List<XYZ> directionList1 = new List<XYZ>();
                    if (locationCurve1 != null) {
                        curve1 = locationCurve1.Curve;
                    }
                    else if (element1.GetType().Name == "Grid") {
                        Grid grid = (Grid)element1;
                        curve1 = grid.Curve;
                    }
                    else if (element1.GetType().Name == "FamilyInstance" && locationCurve1 == null && coreRadio.Checked) {
                        FamilyInstance ins1 = (FamilyInstance)element1;
                        // 中心正面/背面
                        IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                        // 中心左/右
                        IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                        // 中心立面図
                        IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                        if (frontBack.Count > 0) {
                            foreach (Reference r in frontBack) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                directionList1.Add(v);
                            }
                        }
                        if (leftRight.Count > 0) {
                            foreach (Reference r in leftRight) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                directionList1.Add(v);
                            }
                        }
                        if (centerElevation.Count > 0) {
                            foreach (Reference r in centerElevation) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                directionList1.Add(v);
                            }
                        }
                    }
                    Line line1 = null;
                    if (curve1 != null && curve1.GetType().Name == "Line") {
                        line1 = (Line)curve1;
                        XYZ v = NormalViewDirection(line1, Doc.ActiveView.ViewDirection);
                        if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            directionList1.Add(line1.Direction);
                        }
                        else {
                            XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                            directionList1.Add(Line.CreateBound(XYZ.Zero, v2).Direction);
                        }
                    }
                    // 芯-芯以外を選択肢、ロケーションカーブがnullの場合
                    if (locationCurve1 == null && !coreRadio.Checked) {
                        // フェイスもしくはエッジを全て取得
                        HashSet<XYZ> faceDirectionSet = new HashSet<XYZ>();
                        HashSet<XYZ> edgeDirectionSet = new HashSet<XYZ>();
                        List<KeyValuePair<double, double>> kvpList = new List<KeyValuePair<double, double>>();
                        HashSet<XYZ> deleteDirectionSet = new HashSet<XYZ>();
                        List<KeyValuePair<Face, Element>> allFaces = new List<KeyValuePair<Face, Element>>();
                        List<KeyValuePair<Edge, Element>> allEdges = new List<KeyValuePair<Edge, Element>>();
                        Options opt = new Options();
                        opt.ComputeReferences = true;
                        opt.View = Doc.ActiveView;
                        opt.IncludeNonVisibleObjects = false;
                        List<Face> faces = GetInstanceAllFaceList(element1, opt);
                        foreach (Face face in faces) {
                            PlanarFace pf = face as PlanarFace;
                            if (Math.Round(pf.FaceNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                if (!kvpList.Contains(new KeyValuePair<double, double>(Math.Round(pf.FaceNormal.X, 4, MidpointRounding.AwayFromZero), Math.Round(pf.FaceNormal.Y, 4, MidpointRounding.AwayFromZero)))) {
                                    faceDirectionSet.Add(pf.FaceNormal);
                                    kvpList.Add(new KeyValuePair<double, double>(Math.Round(pf.FaceNormal.X, 4, MidpointRounding.AwayFromZero), Math.Round(pf.FaceNormal.Y, 4, MidpointRounding.AwayFromZero)));
                                }
                            }
                        }
                        List<XYZ> checkList = new List<XYZ>();
                        foreach (XYZ dir in faceDirectionSet) {
                            List<PlanarFace> pFaceList = new List<PlanarFace>();
                            bool checkFlag = false;
                            foreach (XYZ checkDir in checkList) {
                                // 平行かどうか
                                if (IsParallel(checkDir, dir)) {
                                    checkFlag = true;
                                }
                            }
                            if (checkFlag) {
                                continue;
                            }
                            foreach (Face face in faces) {
                                PlanarFace pf = face as PlanarFace;
                                // 平行かどうか
                                if (IsParallel(dir, pf.FaceNormal)) {
                                    pFaceList.Add(pf);
                                }
                            }
                            XYZ xyz = dir;
                            double x = 0;
                            double y = 0;
                            double slo = xyz.Y / xyz.X;
                            if (double.IsInfinity(slo)) {
                                x = 1;
                                y = 0;
                            }
                            else if (Math.Round(slo, 3, MidpointRounding.AwayFromZero) == 0) {
                                x = 0;
                                y = 1;
                            }
                            else {
                                double vs = -1 / slo;
                                x = 1;
                                y = 1 * vs;
                            }
                            xyz = new XYZ(x, y, xyz.Z).Normalize();
                            List<Face> faceList = FaceSortPointByDirection(dir, pFaceList.Cast<Face>().ToList());
                            allFaces.Add(new KeyValuePair<Face, Element>(faceList.First(), element1));
                            allFaces.Add(new KeyValuePair<Face, Element>(faceList.Last(), element1));
                            checkList.Add(dir);
                        }

                        if (allFaces.Count == 0) {
                            List<Edge> edges = GetAllInstanceEdgeList(element1, opt);
                            foreach (Edge edge in edges) {
                                Line edgeLine = edge.AsCurve() as Line;
                                if (edgeLine != null) {
                                    if (!kvpList.Contains(new KeyValuePair<double, double>(edgeLine.Direction.X, edgeLine.Direction.Y))) {
                                        edgeDirectionSet.Add(edgeLine.Direction);
                                        kvpList.Add(new KeyValuePair<double, double>(edgeLine.Direction.X, edgeLine.Direction.Y));
                                    }
                                }
                            }
                            checkList = new List<XYZ>();
                            foreach (XYZ dir in edgeDirectionSet) {
                                List<Edge> edgeList = new List<Edge>();
                                bool checkFlag = false;
                                foreach (XYZ checkDir in checkList) {
                                    // 平行かどうか
                                    if (IsParallel(checkDir, dir)) {
                                        checkFlag = true;
                                    }
                                }
                                if (checkFlag) {
                                    continue;
                                }
                                foreach (Edge edge in edges) {
                                    Line edgeLine = edge.AsCurve() as Line;
                                    // 平行かどうか
                                    if (IsParallel(dir, edgeLine.Direction)) {
                                        edgeList.Add(edge);
                                    }
                                }
                                XYZ xyz = dir;
                                double x = 0;
                                double y = 0;
                                double slo = xyz.Y / xyz.X;
                                if (double.IsInfinity(slo)) {
                                    x = 1;
                                    y = 0;
                                }
                                else if (Math.Round(slo, 3, MidpointRounding.AwayFromZero) == 0) {
                                    x = 0;
                                    y = 1;
                                }
                                else {
                                    double vs = -1 / slo;
                                    x = 1;
                                    y = 1 * vs;
                                }
                                xyz = new XYZ(x, y, xyz.Z).Normalize();
                                edgeList = EdgeSortPointByDirection(dir, edgeList);
                                allEdges.Add(new KeyValuePair<Edge, Element>(edgeList.First(), element1));
                                allEdges.Add(new KeyValuePair<Edge, Element>(edgeList.Last(), element1));
                                checkList.Add(dir);
                            }
                        }
                        int count1 = 1;
                        foreach (XYZ xyz in faceDirectionSet) {
                            int count2 = 1;
                            foreach (XYZ xyz2 in faceDirectionSet) {
                                if (count2 <= count1) {
                                    count2++;
                                    continue;
                                }
                                // 平行かどうか
                                if (IsParallel(xyz, xyz2)) {
                                    deleteDirectionSet.Add(xyz2);
                                }
                                count2++;
                            }
                            count1++;
                        }
                        // 重複する方向を削除
                        foreach (XYZ xyz in deleteDirectionSet) {
                            faceDirectionSet.Remove(xyz);
                        }
                        deleteDirectionSet = new HashSet<XYZ>();
                        foreach (XYZ xyz in edgeDirectionSet) {
                            count1 = 1;
                            int count2 = 1;
                            foreach (XYZ xyz2 in edgeDirectionSet) {
                                if (count2 > count1) {
                                    continue;
                                }
                                //　平行かどうか
                                if (IsParallel(xyz, xyz2)) {
                                    deleteDirectionSet.Add(xyz2);
                                }
                                count2++;
                            }
                            count1++;
                        }
                        // 重複する方向を削除
                        foreach (XYZ xyz in deleteDirectionSet) {
                            edgeDirectionSet.Remove(xyz);
                        }
                        HashSet<List<KeyValuePair<PlanarFace, Element>>> faceListSet = new HashSet<List<KeyValuePair<PlanarFace, Element>>>();
                        foreach (XYZ xyz in faceDirectionSet) {
                            List<KeyValuePair<PlanarFace, Element>> facesList = new List<KeyValuePair<PlanarFace, Element>>();
                            foreach (KeyValuePair<Face, Element> kvp in allFaces) {
                                Face face = kvp.Key;
                                PlanarFace pf = face as PlanarFace;
                                // 平行かどうか
                                if (IsParallel(xyz, pf.FaceNormal)) {
                                    if (!facesList.Contains(new KeyValuePair<PlanarFace, Element>(pf, kvp.Value))) {
                                        facesList.Add(new KeyValuePair<PlanarFace, Element>(pf, kvp.Value));
                                    }
                                }
                            }
                            if (facesList.Count > 0) {
                                faceListSet.Add(facesList);
                            }
                        }
                        HashSet<List<KeyValuePair<Edge, Element>>> edgeListSet = new HashSet<List<KeyValuePair<Edge, Element>>>();
                        foreach (XYZ xyz in edgeDirectionSet) {
                            List<KeyValuePair<Edge, Element>> edges = new List<KeyValuePair<Edge, Element>>();
                            foreach (KeyValuePair<Edge, Element> kvp in allEdges) {
                                Edge edge = kvp.Key;
                                Line edgeLine = edge.AsCurve() as Line;
                                // 平行かどうか
                                if (IsParallel(xyz, edgeLine.Direction)) {
                                    if (!edges.Contains(new KeyValuePair<Edge, Element>(edge, kvp.Value))) {
                                        edges.Add(new KeyValuePair<Edge, Element>(edge, kvp.Value));
                                    }
                                }
                            }
                            if (edges.Count > 0) {
                                edgeListSet.Add(edges);
                            }
                        }
                        List<XYZ> xyzList = new List<XYZ>();
                        foreach (XYZ fDirection in faceDirectionSet) {
                            XYZ direction = NormalViewDirection(Line.CreateBound(XYZ.Zero, fDirection), Doc.ActiveView.ViewDirection);
                            directionList1.Add(direction);
                        }
                        if (faceDirectionSet.Count == 0) {
                            foreach (XYZ fDirection in edgeDirectionSet) {
                                directionList1.Add(fDirection);
                            }
                        }
                        if (faceDirectionSet.Count == 0 && edgeDirectionSet.Count == 0) {
                            List<Line> lineList = GetLine(element1);
                            foreach (Line line in lineList) {
                                directionList1.Add(line.Direction);
                            }
                        }
                    }
                    foreach (XYZ xyz in directionList1) {
                        // 平行かどうか
                        if (IsParallel(directionXYZ, xyz)) {
                            elementSet.Add(element1);
                        }
                    }
                }
                elementDirDic.Add(directionXYZ, elementSet);
            }
            // 要素方向ディクショナリーでループ
            foreach (KeyValuePair<XYZ, HashSet<Element>> kvp in elementDirDic) {
                XYZ direction = kvp.Key;
                XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, direction), Doc.ActiveView.ViewDirection);
                List<Element> newElementList = new List<Element>();
                foreach (Element ele in kvp.Value) {
                    newElementList.Add(ele);
                }
                XYZ sortDirection = vDirection;
                // ソート
                newElementList = ElementSortPointByDirection(sortDirection, newElementList);
                // 芯-芯を選択した場合
                if (coreRadio.Checked) {
                    List<TmpElement> elems = new List<TmpElement>();
                    int count = 0;
                    foreach (Element element in newElementList) {
                        elems.Add(new TmpElement(element, count));
                        count++;
                    }
                    List<TmpElement> newList = CreateDimension(elems, direction, activeView);
                    newElementList.Clear();
                    foreach (TmpElement tmp in newList) {
                        newElementList.Add(tmp.TmpEle);
                    }
                    // ソート
                    newElementList = ElementSortPointByDirection(vDirection, newElementList);
                    // 寸法作成
                    CreateXY(newElementList, direction, Doc.ActiveView);
                }
                else {
                    // 寸法作成
                    CreateXY(newElementList, direction, Doc.ActiveView);
                }
            }
        }

        /// <summary>
        /// XY平面以外の寸法作成前処理
        /// </summary>
        private void CreateDimension_CrossSection()
        {
            View activeView = Doc.ActiveView;
            XYZ elementDirection = new XYZ(0, 0, 0);
            XYZ rightDirection = activeView.RightDirection;

            // 要素セットリストが1以上の場合
            if (elementSetList.Count > 0) {
                // 要素セットリスト(同じ方向ごとにまとめた要素セットのリスト)でループ
                foreach (HashSet<Element> elementSet in elementSetList) {
                    // 要素の方向を取得
                    LocationCurve locationCurve = null;
                    Curve curve = null;

                    // 要素セットに含まれる要素をリスト化
                    List<Element> newElementList = new List<Element>();
                    foreach (Element element in elementSet) {
                        newElementList.Add(element);
                    }

                    foreach (Element element in newElementList) {
                        locationCurve = element.Location as LocationCurve;
                        // LocationCurveがあればそのCurveを要素の方向とする
                        if (locationCurve != null) {
                            curve = locationCurve.Curve;
                            break;
                        }
                        // GridであればそのCurveを要素の方向とする
                        if (element.GetType().Name == "Grid") {
                            Grid grid = (Grid)element;
                            curve = grid.Curve;
                            break;
                        }
                    }

                    // 要素の方向がLine(直線)の場合
                    if (curve != null && curve.GetType().Name == "Line") {
                        Line line = (Line)curve;
                        XYZ vDirection = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            elementDirection = new XYZ(0, 0, 1);
                        }
                        else if (Math.Abs(Math.Round(line.Direction.X, 3, MidpointRounding.AwayFromZero)) != 1
                            && Math.Abs(Math.Round(line.Direction.Y, 3, MidpointRounding.AwayFromZero)) != 1) {
                            if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Abs(Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero)) == 1) {
                                elementDirection = new XYZ(0, 0, 1);
                            }
                            else {
                                elementDirection = line.Direction;
                            }
                        }
                        else {
                            elementDirection = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                        }
                    }

                    XYZ vDir = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                    // ソート
                    newElementList = ElementSortPointByDirection(vDir, newElementList);
                    // 芯-芯を選択した場合
                    if (coreRadio.Checked) {
                        List<TmpElement> elems = new List<TmpElement>();
                        int count = 0;
                        foreach (Element element in newElementList) {
                            elems.Add(new TmpElement(element, count));
                            count++;
                        }
                        List<TmpElement> newList = CreateDimension(elems, elementDirection, activeView);
                        newElementList.Clear();
                        foreach (TmpElement tmp in newList) {
                            newElementList.Add(tmp.TmpEle);
                        }
                        // ソート
                        newElementList = ElementSortPointByDirection(Doc.ActiveView.RightDirection, newElementList);
                        // 寸法作成
                        CreateCrossSection(newElementList, elementDirection, activeView);
                    }
                    else {
                        // 寸法作成
                        CreateCrossSection(newElementList, elementDirection, activeView);
                    }
                    return;
                }
            }

            // ポイントセットリストが1以上、芯-芯または面-芯を選択した場合
            else if (pointSetList.Count > 0 && coreRadio.Checked || faceCoreRadio.Checked) {
                List<Element> newElementList = new List<Element>();
                List<Line> lineList = new List<Line>();
                foreach (Element element in pointSetList) {
                    // 芯を取得
                    if (element.GetType().Name == "FamilyInstance") {
                        Line line = null;
                        LocationCurve locationCurve = null;
                        locationCurve = element.Location as LocationCurve;
                        if (locationCurve != null) {
                            Curve curve = null;
                            curve = locationCurve.Curve;
                            if (curve != null && curve.GetType().Name == "Line") {
                                line = (Line)curve;
                                XYZ mLine = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(mLine.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(mLine.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(mLine.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    line = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                                    lineList.Add(line);
                                }
                                else {
                                    XYZ mLine2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, mLine), Doc.ActiveView.ViewDirection);
                                    lineList.Add(Line.CreateBound(XYZ.Zero, mLine2));
                                }
                            }
                        }
                        FamilyInstance ins = (FamilyInstance)element;
                        // 中心正面/背面
                        IList<Reference> frontBack = ins.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                        // 中心左/右
                        IList<Reference> leftRight = ins.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                        // 中心立面図
                        IList<Reference> centerElevation = ins.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                        if (frontBack.Count > 0 && line == null) {
                            foreach (Reference r in frontBack) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                Line li = Line.CreateBound(XYZ.Zero, xyz);
                                XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                                li = Line.CreateBound(XYZ.Zero, v);
                                lineList.Add(li);
                            }
                        }
                        if (leftRight.Count > 0 && line == null) {
                            foreach (Reference r in leftRight) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                Line li = Line.CreateBound(XYZ.Zero, xyz);
                                XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                                li = Line.CreateBound(XYZ.Zero, v);
                                lineList.Add(li);
                            }
                        }
                        if (centerElevation.Count > 0 && line == null) {
                            foreach (Reference r in centerElevation) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                Line li = Line.CreateBound(XYZ.Zero, xyz);
                                XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                                li = Line.CreateBound(XYZ.Zero, v);
                                lineList.Add(li);
                            }
                        }
                    }
                }
                List<Line> newLineList = new List<Line>();
                foreach (Line line in lineList) {
                    bool checkFlag = true;
                    foreach (Line line2 in newLineList) {
                        // 平行かどうか
                        if (IsParallel(line.Direction, line2.Direction)) {
                            checkFlag = false;
                        }
                    }
                    if (checkFlag) {
                        newLineList.Add(line);
                    }
                }
                newElementList = pointSetList;
                // ライン（方向）でループ
                foreach (Line line in newLineList) {
                    XYZ direction = line.Direction;
                    XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, direction), Doc.ActiveView.ViewDirection);
                    if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                        direction = new XYZ(0, 0, 1);
                        vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, direction), Doc.ActiveView.ViewDirection);
                    }
                    XYZ sortDirection = vDirection;
                    // 平行かどうか
                    if (IsParallel(sortDirection, Doc.ActiveView.RightDirection)) {
                        sortDirection = Doc.ActiveView.RightDirection;
                    }
                    // 平行かどうか
                    if (IsParallel(sortDirection, Doc.ActiveView.UpDirection)) {
                        sortDirection = Doc.ActiveView.UpDirection;
                    }
                    // ソート
                    newElementList = ElementSortPointByDirection(sortDirection, newElementList);
                    if (coreRadio.Checked) {
                        List<TmpElement> elems = new List<TmpElement>();
                        int count = 0;
                        foreach (Element element in newElementList) {
                            elems.Add(new TmpElement(element, count));
                            count++;
                        }
                        List<TmpElement> newList = CreateDimension(elems, direction, activeView);
                        pointSetList.Clear();
                        foreach (TmpElement tmp in newList) {
                            pointSetList.Add(tmp.TmpEle);
                        }
                        // ソート
                        pointSetList = ElementSortPointByDirection(sortDirection, pointSetList);
                        // 寸法作成
                        CreateCrossSection(pointSetList, direction, activeView);
                    }
                    else {
                        // 寸法作成
                        CreateCrossSection(newElementList, direction, activeView);
                    }
                }
            }
            // ポイントセットリストが1以上、面-面を選択した場合
            else if (pointSetList.Count > 0 && faceRadio.Checked) {
                // 全フェイス取得
                HashSet<XYZ> faceDirectionSet = new HashSet<XYZ>();
                HashSet<XYZ> checkSet = new HashSet<XYZ>();
                bool checkFlag = false;
                HashSet<XYZ> deleteDirectionSet = new HashSet<XYZ>();
                List<KeyValuePair<Face, Element>> allFaces = new List<KeyValuePair<Face, Element>>();
                foreach (Element element in pointSetList) {
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    List<Face> faces = GetInstanceAllFaceList(element, opt);
                    foreach (Face face in faces) {
                        checkFlag = true;
                        PlanarFace pf = face as PlanarFace;
                        foreach (XYZ xyz in checkSet) {
                            if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == Math.Round(pf.FaceNormal.X, 3, MidpointRounding.AwayFromZero)
                                && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == Math.Round(pf.FaceNormal.Y, 3, MidpointRounding.AwayFromZero)
                                && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == Math.Round(pf.FaceNormal.Z, 3, MidpointRounding.AwayFromZero)) {
                                checkFlag = false;
                                break;
                            }
                        }
                        if (checkFlag) {
                            faceDirectionSet.Add(pf.FaceNormal);
                            checkSet.Add(pf.FaceNormal);
                        }
                    }
                }
                int count1 = 1;
                foreach (XYZ xyz in faceDirectionSet) {
                    int count2 = 1;
                    foreach (XYZ xyz2 in faceDirectionSet) {
                        if (count2 <= count1) {
                            count2++;
                            continue;
                        }
                        // 平行かどうか
                        if (IsParallel(xyz, xyz2)) {
                            deleteDirectionSet.Add(xyz2);
                        }
                        count2++;
                    }
                    count1++;
                }
                foreach (XYZ xyz in deleteDirectionSet) {
                    faceDirectionSet.Remove(xyz);
                }
                List<XYZ> xyzList = new List<XYZ>();
                if (faceDirectionSet.Count > 1) {
                    foreach (XYZ xyz in faceDirectionSet) {
                        Line line = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ vDirection = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            continue;
                        }
                        XYZ sortDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, vDirection), Doc.ActiveView.ViewDirection);
                        XYZ direction = vDirection;
                        bool check = true;
                        foreach (XYZ x in xyzList) {
                            if (IsParallel(x, direction)) {
                                // 平行かどうか
                                check = false;
                            }
                        }
                        if (check) {
                            // ソート
                            pointSetList = ElementSortPointByDirection(sortDirection, pointSetList);
                            xyzList.Add(direction);
                            // 寸法作成
                            CreateCrossSection(pointSetList, direction, activeView);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// XY平面以外の寸法作成
        /// </summary>
        /// <param name="newElementList">要素リスト</param>
        /// <param name="elementDirection">要素の方向</param>
        /// <param name="activeView">アクティブビュー</param>
        private void CreateCrossSection(List<Element> newElementList, XYZ elementDirection, View activeView)
        {
            Element e1 = null;
            Element e2 = null;
            int countCross = 0;
            for (int i = 0; i < newElementList.Count - 1; i++) {
                try {
                    Element element1 = newElementList[i];
                    Element element2 = newElementList[i + 1];
                    if (element1.Id.ToString() == element2.Id.ToString()) {
                        continue;
                    }
                    e1 = element1;
                    e2 = element2;

                    // 面-面を選択した場合
                    if (faceRadio.Checked) {
                        CreateCrossSectionFaceToFace(element1, element2, elementDirection, activeView, ref countCross);
                    }
                    // 芯-芯、または、面-芯を選択した場合
                    else
                    {
                        // ラインリストを取得
                        List<Line> lineList1 = GetLineList(element1);
                        List<Line> lineList2 = GetLineList(element2);
                        // 芯用ディクショナリーを取得
                        Dictionary<Reference, XYZ> refDic1 = GetCoreReferenceDictionary(element1, elementDirection, activeView, lineList1);
                        Dictionary<Reference, XYZ> refDic2 = GetCoreReferenceDictionary(element2, elementDirection, activeView, lineList2);

                        // 芯-芯を選択した場合
                        if (coreRadio.Checked)
                        {
                            XYZ midPoint1 = new XYZ(0, 0, 0);
                            XYZ midPoint2 = new XYZ(0, 0, 0);

                            // 2要素間の芯-芯寸法方向を算出
                            XYZ diagonalDirection = null;
                            if (element1.GetType().Name == "FamilyInstance" && element1.Location is LocationCurve)
                            {
                                diagonalDirection = CalcDiagonalDirection(element1, element2, activeView);
                            }

                            foreach (KeyValuePair<Reference, XYZ> kvp in refDic1)
                            {
                                foreach (KeyValuePair<Reference, XYZ> kvp2 in refDic2)
                                {
                                    // 平行かどうか
                                    if (IsParallel(kvp.Value, kvp2.Value))
                                    {
                                        // 平行でない場合はコンティニュー
                                        if (!IsParallel(kvp.Value, elementDirection))
                                        {
                                            continue;
                                        }
                                        XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0)
                                        {
                                            vDirection = new XYZ(0, 0, 1);
                                        }
                                        if (diagonalDirection == null)
                                        {
                                            midPoint1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                                            midPoint2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                                        }
                                        else
                                        {
                                            midPoint1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                                            midPoint2 = new XYZ(dimensionPoint.X + diagonalDirection.X, dimensionPoint.Y + diagonalDirection.Y, dimensionPoint.Z + diagonalDirection.Z);
                                        }
                                        // 寸法作成処理
                                        CreateDimensionElement(Doc.ActiveView, midPoint1, kvp.Key, midPoint2, kvp2.Key, element1, element2);
                                    }
                                }
                            }
                        }
                        // 面-芯を選択した場合
                        else
                        {
                            XYZ midPoint1 = new XYZ(0, 0, 0);
                            XYZ midPoint2 = new XYZ(0, 0, 0);
                            BoundingBoxXYZ box1 = element1.get_BoundingBox(activeView);
                            BoundingBoxXYZ box2 = element2.get_BoundingBox(activeView);
                            if (box1 != null)
                            {
                                midPoint1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, 0);
                            }
                            if (box2 != null)
                            {
                                midPoint2 = new XYZ((box2.Max.X + box2.Min.X) / 2, (box2.Max.Y + box2.Min.Y) / 2, 0);
                            }

                            XYZ point1 = XYZ.Zero;
                            XYZ point2 = XYZ.Zero;
                            XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                            point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                            if (Math.Round(vDirection.X, 0, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(vDirection.Y, 0, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(vDirection.Z, 0, MidpointRounding.AwayFromZero) == 0)
                            {
                                point2 = new XYZ(dimensionPoint.X + 1, dimensionPoint.Y, dimensionPoint.Z);
                                vDirection = new XYZ(0, 0, 1);
                            }
                            else if (Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1 ||
                                Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == -1)
                            {
                                point2 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z + 1);
                            }
                            else
                            {
                                point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                            }
                            XYZ verticalNormal = vDirection;

                            // 自オブジェクト間の寸法
                            CreateCrossSectionFaceToCoreBetweenOwn(element1, elementDirection, activeView, refDic1, verticalNormal, point1, point2, e1, countCross == 0, true, 
                                                                   out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl1);
                            CreateCrossSectionFaceToCoreBetweenOwn(element2, elementDirection, activeView, refDic2, verticalNormal, point1, point2, e2, true, false, 
                                                                   out List<Face> faceList2, out List<Edge> edgeList2, out DetailCurve mDl2);
                            countCross++;

                            // 自他オブジェクト間の寸法
                            Reference ref1 = null;
                            Reference ref2 = null;
                            if (edgeList.Count > 0)
                            {
                                // ライトダイレクションと平行の場合
                                if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.RightDirection))
                                {
                                    ref1 = edgeList.Last().Reference;
                                }
                                // アップダイレクションと平行の場合
                                else if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.UpDirection))
                                {
                                    ref1 = edgeList.Last().Reference;
                                }
                                else
                                {
                                    ref1 = edgeList.Last().Reference;
                                }
                            }
                            else if (faceList.Count > 0)
                            {
                                ref1 = faceList.Last().Reference;
                            }
                            else if (mDl1 != null)
                            {
                                ref1 = mDl1.GeometryCurve.Reference;
                            }
                            if (element1.GetType().Name == "Grid")
                            {
                                ref1 = new Reference(element1);
                            }
                            if (edgeList2.Count > 0)
                            {
                                // ライトダイレクションと平行の場合
                                if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.RightDirection))
                                {
                                    ref2 = edgeList2.First().Reference;
                                }
                                // アップダイレクションと平行の場合
                                else if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.UpDirection))
                                {
                                    ref2 = edgeList2.First().Reference;
                                }
                                else
                                {
                                    ref2 = edgeList2.First().Reference;
                                }
                            }
                            else if (faceList2.Count > 0)
                            {
                                ref2 = faceList2.First().Reference;
                            }
                            else if (mDl2 != null)
                            {
                                ref2 = mDl2.GeometryCurve.Reference;
                            }
                            if (element2.GetType().Name == "Grid")
                            {
                                ref2 = new Reference(element2);
                            }
                            if (ref1 != null && ref2 != null)
                            {
                                // 寸法作成処理
                                CreateDimensionElement(Doc.ActiveView,
                                point1, ref1,
                                point2, ref2, element1, element2);
                            }
                        }
                    }
                }
                catch (Exception) {
                    if (e1 != null) {
                        errorIdSet.Add(e1.Id.ToString());
                    }
                    if (e2 != null) {
                        errorIdSet.Add(e2.Id.ToString());
                    }
                }
            }
            // 単体の場合
            if (newElementList.Count == 1) {
                try {
                    Element element1 = newElementList[0];
                    e1 = element1;
                    // 面-面を選択した場合
                    if (faceRadio.Checked)
                    {
                        XYZ point1 = XYZ.Zero;
                        XYZ point2 = XYZ.Zero;
                        XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                        point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        if (Math.Round(vDirection.X, 0, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 0, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 0, MidpointRounding.AwayFromZero) == 0)
                        {
                            point2 = new XYZ(dimensionPoint.X + 1, dimensionPoint.Y, dimensionPoint.Z);
                            vDirection = new XYZ(0, 0, 1);
                        }
                        else if (Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1 ||
                            Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == -1)
                        {
                            point2 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z + 1);
                        }
                        else
                        {
                            point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                        }
                        XYZ verticalNormal = vDirection;

                        // 自オブジェクト間の寸法
                        CreateCrossSectionFaceToFaceBetweenOwn(element1, elementDirection, activeView, verticalNormal, point1, point2, true, true,
                                                               out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl1);
                    }
                    // 面-芯を選択した場合
                    else
                    {
                        // ラインリストを取得
                        List<Line> lineList1 = new List<Line>();
                        LocationCurve locationCurve1 = element1.Location as LocationCurve;
                        Curve curve1 = null;
                        if (locationCurve1 != null)
                        {
                            curve1 = locationCurve1.Curve;
                        }
                        else if (element1.GetType().Name == "Grid")
                        {
                            return;
                        }
                        else if (element1.GetType().Name == "FamilyInstance")
                        {
                            FamilyInstance instance = (FamilyInstance)element1;
                            Transform t = instance.GetTransform();
                            XYZ zero = new XYZ(0, 0, 0);
                            XYZ baseZ = t.BasisZ;
                            Line lineX = Line.CreateBound(zero, baseZ);
                            curve1 = lineX;
                        }
                        Line line1 = (Line)curve1;
                        if (curve1 != null)
                        {
                            Line li1 = (Line)curve1;
                            lineList1.Add(li1);
                        }
                        XYZ lineDirection1 = line1.Direction;

                        // 芯用ディクショナリーを取得
                        Dictionary<Reference, XYZ> refDic1 = GetCoreReferenceDictionary(element1, elementDirection, activeView, lineList1);

                        XYZ point1 = XYZ.Zero;
                        XYZ point2 = XYZ.Zero;
                        XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                        point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        if (Math.Round(vDirection.X, 0, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 0, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 0, MidpointRounding.AwayFromZero) == 0)
                        {
                            point2 = new XYZ(dimensionPoint.X + 1, dimensionPoint.Y, dimensionPoint.Z);
                            vDirection = new XYZ(0, 0, 1);
                        }
                        else if (Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1 ||
                            Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == -1)
                        {
                            point2 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z + 1);
                        }
                        else
                        {
                            point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                        }
                        XYZ verticalNormal = vDirection;

                        // 自オブジェクト間の寸法
                        CreateCrossSectionFaceToCoreBetweenOwn(element1, elementDirection, activeView, refDic1, verticalNormal, point1, point2, e1, true, true,
                                                               out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl1);
                    }
                }
                catch (Exception) {
                    if (e1 != null) {
                        errorIdSet.Add(e1.Id.ToString());
                    }
                    if (e2 != null) {
                        errorIdSet.Add(e2.Id.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// XY平面以外の面-芯寸法作成(自オブジェクト間)
        /// </summary>
        /// <param name="element">自オブジェクト要素</param>
        /// <param name="elementDirection">要素方向</param>
        /// <param name="activeView">アクティブビュー</param>
        /// <param name="refDic">Referenceディクショナリー</param>
        /// <param name="verticalNormal">要素直行方向単位ベクトル</param>
        /// <param name="point1">寸法作成位置(ユーザ選択)</param>
        /// <param name="point2">寸法作成位置から寸法作成方向にずれた点</param>
        /// <param name="e">エラーログ出力用の要素ID</param>
        /// <param name="needDimention">寸法作成要否</param>
        /// <param name="isStartSide">2要素間の寸法作成時、寸法作成順で手前側のオブジェクトである</param>
        /// <param name="faceList">オブジェクトのFaceリスト</param>
        /// <param name="edgeList">オブジェクトのEdgeリスト</param>
        /// <param name="mDl">作成した詳細線分(より隣の要素に近いもの)</param>
        private void CreateCrossSectionFaceToCoreBetweenOwn(Element element, XYZ elementDirection, View activeView, Dictionary<Reference, XYZ> refDic,
                                                            XYZ verticalNormal, XYZ point1, XYZ point2, Element e, bool needDimention, bool isStartSide,
                                                            out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl)
        {
            faceList = new List<Face>();
            edgeList = new List<Edge>();
            mDl = null;

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.View = activeView;

            // エッジ取得
            if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
            {
                edgeList = GetInstanceEdgeListXZ(element, elementDirection, opt);
            }
            else
            {
                edgeList = GetEdgeListXZ(element, elementDirection, opt);
            }

            // フェイス取得
            if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
            {
                List<Face> list = GetInstanceClosestFaceList(element, verticalNormal, opt);
                List<int> intList = new List<int>();
                foreach (Face face in list)
                {
                    intList.Add(face.Id);
                }
                faceList = GetSymbolClosestFaceList(element, intList, opt);
            }
            else
            {
                faceList = GetClosestFaceList(element, verticalNormal, opt);
                if (faceList.Count > 0)
                {
                    // ソート
                    faceList = FaceSortPointByDirection(verticalNormal, faceList);
                }
            }
            Reference ref1 = null;
            Reference ref2 = null;
            if (edgeList.Count > 1)
            {
                ref1 = edgeList.First().Reference;
                ref2 = edgeList.Last().Reference;
            }
            if (edgeList.Count == 1)
            {
                ref1 = edgeList.First().Reference;
                ref2 = null;
            }
            else if (faceList.Count > 1)
            {
                ref1 = faceList.First().Reference;
                ref2 = faceList.Last().Reference;
            }
            else if (faceList.Count == 1)
            {
                ref1 = faceList.First().Reference;
                ref2 = null;
            }
            else if(needDimention)
            {
                // 詳細線分作成
                LocationCurve locationCurve = element.Location as LocationCurve;
                // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                if (locationCurve != null && lineCheck.Checked)
                {
                    Line line = (Line)locationCurve.Curve;
                    // 平行かどうか
                    if (IsParallel(line.Direction, elementDirection))
                    {
                        List<Solid> solidList = new List<Solid>();
                        if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
                        {
                            solidList = GetInstanceSolid(element);
                        }
                        else
                        {
                            solidList = GetSolid(element);
                        }
                        if (solidList == null)
                        {
                            return;
                        }
                        Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                        List<XYZ> xyzList = new List<XYZ>();
                        foreach (KeyValuePair<XYZ, int> kvp in cornerDic)
                        {
                            xyzList.Add(kvp.Key);
                        }
                        List<XYZ> cornerList = GetCoordinateXY(xyzList, line.Direction);
                        cornerList.Sort((a, b) => Math.Sign(a.X - b.X));
                        XYZ maxX = cornerList.Last();
                        cornerList.Sort((a, b) => Math.Sign(b.X - a.X));
                        XYZ minX = cornerList.First();
                        cornerList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                        XYZ maxY = cornerList.Last();
                        if (maxY.X == minX.X)
                        {
                            maxY = cornerList[cornerList.Count - 2];
                        }
                        XYZ minY = cornerList.First();
                        if (minY.X == maxX.X)
                        {
                            minY = cornerList[1];
                        }
                        using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE))
                        {
                            t.Start();
                            Line l1 = Line.CreateBound(minY, maxX);
                            DetailCurve dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                            Line l2 = Line.CreateBound(minX, maxY);
                            DetailCurve dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                            t.Commit();
                            ref1 = dl1.GeometryCurve.Reference;
                            ref2 = dl2.GeometryCurve.Reference;
                            List<DetailCurve> dlList = new List<DetailCurve>();
                            dlList.Add(dl1);
                            dlList.Add(dl2);
                            XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                            XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                            double distance1 = GetSpecificDirectionDistance(c1, verticalNormal);
                            double distance2 = GetSpecificDirectionDistance(c2, verticalNormal);
                            if (isStartSide && distance1 < distance2 || !isStartSide && distance1 > distance2)
                            {
                                mDl = dl2;
                            }
                            else
                            {
                                mDl = dl1;
                            }
                        }
                    }
                }
            }
            if (ref1 != null && needDimention)
            {
                foreach (KeyValuePair<Reference, XYZ> kvp in refDic)
                {
                    // 芯と平行の場合、寸法作成
                    if (IsParallel(kvp.Value, elementDirection))
                    {
                        // 寸法作成処理
                        CreateDimensionElement(Doc.ActiveView,
                        point1, ref1,
                        point2, kvp.Key, element, element);
                    }
                }
            }
            if (ref2 != null && needDimention)
            {
                foreach (KeyValuePair<Reference, XYZ> kvp in refDic)
                {
                    // 芯と平行の場合、寸法作成
                    if (IsParallel(kvp.Value, elementDirection))
                    {
                        // 寸法作成処理
                        CreateDimensionElement(Doc.ActiveView,
                        point1, kvp.Key,
                        point2, ref2, element, element);
                    }
                }
            }
            if (ref1 == null && ref2 == null)
            {
                e = element;
                if (e != null)
                {
                    errorIdSet.Add(e.Id.ToString());
                }
            }
        }

        /// <summary>
        /// 2要素間の芯-芯寸法方向を算出
        /// </summary>
        /// <param name="element1">要素1</param>
        /// <param name="element2">要素2</param>
        /// <param name="activeView">アクティブビュー</param>
        /// <returns></returns>
        private XYZ CalcDiagonalDirection(Element element1, Element element2, View activeView)
        {
            BoundingBoxXYZ box1 = element1.get_BoundingBox(activeView);
            BoundingBoxXYZ box2 = element2.get_BoundingBox(activeView);
            XYZ point1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, (box1.Max.Z + box1.Min.Z) / 2);
            XYZ point2 = new XYZ((box2.Max.X + box2.Min.X) / 2, (box2.Max.Y + box2.Min.Y) / 2, (box2.Max.Z + box2.Min.Z) / 2);
            // ビュー断面Planeを作成
            Plane plane1 = Plane.CreateByNormalAndOrigin(activeView.ViewDirection, activeView.Origin);
            // ビュー断面Planeに投影
            plane1.Project(point1, out UV uv, out double distance);
            // 投影点を取得
            XYZ projectPt1 = plane1.Origin + uv.U * plane1.XVec + uv.V * plane1.YVec;
            // ビュー断面Planeを作成
            Plane plane2 = Plane.CreateByNormalAndOrigin(activeView.ViewDirection, activeView.Origin);
            // ビュー断面Planeに投影
            plane2.Project(point2, out UV uv2, out double distance2);
            // 投影点を取得
            XYZ projectPt2 = plane2.Origin + uv2.U * plane2.XVec + uv2.V * plane2.YVec;
            if (Math.Round(projectPt1.X, 3, MidpointRounding.AwayFromZero) == Math.Round(projectPt2.X, 3, MidpointRounding.AwayFromZero)
            && Math.Round(projectPt1.Y, 3, MidpointRounding.AwayFromZero) == Math.Round(projectPt2.Y, 3, MidpointRounding.AwayFromZero))
            {
                return null;
            }
            else
            {
                Line line = Line.CreateBound(projectPt1, projectPt2);
                return line.Direction;
            }
        }

        /// <summary>
        /// 芯ディクショナリーを取得
        /// </summary>
        /// <param name="element">オブジェクト要素</param>
        /// <param name="elementDirection">要素方向</param>
        /// <param name="activeView">アクティブビュー</param>
        /// <param name="lineList">ラインリスト</param>
        /// <returns></returns>
        private Dictionary<Reference, XYZ> GetCoreReferenceDictionary(Element element, XYZ elementDirection, View activeView, List<Line> lineList)
        {
            Dictionary<Reference, XYZ> refDic = new Dictionary<Reference, XYZ>();
            LocationCurve locationCurve = element.Location as LocationCurve;
            // 芯を取得
            if (element.GetType().Name == "FamilyInstance" && locationCurve == null)
            {
                FamilyInstance ins = (FamilyInstance)element;
                // 中心正面/背面
                IList<Reference> frontBack = ins.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0)
                {
                    foreach (Reference r in frontBack)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        refDic.Add(r, li.Direction);
                    }
                }
                if (leftRight.Count > 0)
                {
                    foreach (Reference r in leftRight)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        refDic.Add(r, li.Direction);
                    }
                }
                if (centerElevation.Count > 0)
                {
                    foreach (Reference r in centerElevation)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        refDic.Add(r, li.Direction);
                    }
                }
            }
            else if (element.GetType().Name == "FamilyInstance" && locationCurve != null)
            {
                Curve curve = locationCurve.Curve;
                Line loc = (Line)curve;
                XYZ mLine = NormalViewDirection(loc, Doc.ActiveView.ViewDirection);
                if (Math.Round(mLine.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(mLine.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(mLine.Z, 3, MidpointRounding.AwayFromZero) == 0)
                {
                    loc = Line.CreateBound(XYZ.Zero, XYZ.BasisZ);
                }
                else
                {
                    XYZ mLine2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, mLine), Doc.ActiveView.ViewDirection);
                    loc = Line.CreateBound(XYZ.Zero, mLine2);
                }

                FamilyInstance ins = (FamilyInstance)element;
                // 中心正面/背面
                IList<Reference> frontBack = ins.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0)
                {
                    foreach (Reference r in frontBack)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        // 平行かどうか
                        if (IsParallel(loc.Direction, li.Direction))
                        {
                            refDic.Add(r, li.Direction);
                        }
                    }
                }
                if (leftRight.Count > 0)
                {
                    foreach (Reference r in leftRight)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        // 平行かどうか
                        if (IsParallel(loc.Direction, li.Direction))
                        {
                            refDic.Add(r, li.Direction);
                        }
                    }
                }
                if (centerElevation.Count > 0)
                {
                    foreach (Reference r in centerElevation)
                    {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0)
                        {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        lineList.Add(li);
                        // 平行かどうか
                        if (IsParallel(loc.Direction, li.Direction))
                        {
                            refDic.Add(r, li.Direction);
                        }
                    }
                }
            }
            else if (element.GetType().Name == "FabricationPart")
            {
                int lineInt = GetInstanceLine(element, elementDirection);
                Line li = GetSymbolLine(element, lineInt);
                lineList.Add(li);
                refDic.Add(li.Reference, li.Direction);
            }
            // ファミリインスタンス、ファブリケーションパーツ以外の場合
            else
            {
                Curve curve = null;
                if (locationCurve != null)
                {
                    curve = locationCurve.Curve;
                }
                else if (element.GetType().Name == "Grid")
                {
                    Grid grid = (Grid)element;
                    curve = grid.Curve;
                }
                Line li = null;
                XYZ lineDirection = new XYZ(0, 0, 0);
                if (curve != null)
                {
                    li = (Line)curve;
                    XYZ vDirection = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                    if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0)
                    {
                        li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                        lineDirection = li.Direction;
                    }
                    else if (Math.Round(li.Direction.X, 3, MidpointRounding.AwayFromZero) != 1
                        && Math.Round(li.Direction.Y, 3, MidpointRounding.AwayFromZero) != 1)
                    {
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1)
                        {
                            li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                            lineDirection = li.Direction;
                        }
                        else
                        {
                            lineDirection = li.Direction;
                        }
                    }
                    else
                    {
                        lineDirection = li.Direction;
                    }
                }
                refDic.Add(new Reference(element), lineDirection);
            }
            return refDic;
        }

        /// <summary>
        /// ラインリストを取得
        /// </summary>
        /// <param name="element">オブジェクト要素</param>
        /// <returns></returns>
        private List<Line> GetLineList(Element element)
        {
            List<Line> lineList = new List<Line>();
            LocationCurve locationCurve1 = element.Location as LocationCurve;
            Curve curve = null;
            if (locationCurve1 != null)
            {
                curve = locationCurve1.Curve;
            }
            else if (element.GetType().Name == "Grid")
            {
                Grid grid = (Grid)element;
                curve = grid.Curve;
            }
            else if (element.GetType().Name == "FamilyInstance")
            {
                FamilyInstance instance = (FamilyInstance)element;
                Transform t = instance.GetTransform();
                XYZ zero = new XYZ(0, 0, 0);
                XYZ baseZ = t.BasisZ;
                Line lineX = Line.CreateBound(zero, baseZ);
                curve = lineX;
            }
            if (curve != null)
            {
                Line li = (Line)curve;
                XYZ vDirection = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0)
                {
                    li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                    lineList.Add(li);
                }
                else if (Math.Round(li.Direction.X, 3, MidpointRounding.AwayFromZero) != 1
                    && Math.Round(li.Direction.Y, 3, MidpointRounding.AwayFromZero) != 1)
                {
                    if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1)
                    {
                        li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                        lineList.Add(li);
                    }
                    else
                    {
                        lineList.Add(li);
                    }
                }
                else
                {
                    lineList.Add(li);
                }
            }
            return lineList;
        }

        /// <summary>
        /// XY平面以外の面-面寸法作成
        /// </summary>
        private void CreateCrossSectionFaceToFace(Element element1, Element element2, XYZ elementDirection, View activeView, ref int countCross)
        {
            XYZ point1 = XYZ.Zero;
            XYZ point2 = XYZ.Zero;

            XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
            point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
            if (Math.Round(vDirection.X, 0, MidpointRounding.AwayFromZero) == 0
                && Math.Round(vDirection.Y, 0, MidpointRounding.AwayFromZero) == 0
                && Math.Round(vDirection.Z, 0, MidpointRounding.AwayFromZero) == 0)
            {
                point2 = new XYZ(dimensionPoint.X + 1, dimensionPoint.Y, dimensionPoint.Z);
                vDirection = new XYZ(0, 0, 1);
            }
            else if (Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1 ||
                Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == -1)
            {
                point2 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z + 1);
            }
            else
            {
                point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
            }
            XYZ verticalNormal = vDirection;

            // 自オブジェクト間の寸法
            CreateCrossSectionFaceToFaceBetweenOwn(element1, elementDirection, activeView, verticalNormal, point1, point2, countCross == 0, true,
                                                   out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl1);
            CreateCrossSectionFaceToFaceBetweenOwn(element2, elementDirection, activeView, verticalNormal, point1, point2, true, false,
                                                   out List<Face> faceList2, out List<Edge> edgeList2, out DetailCurve mDl2);
            countCross++;

            // 自他オブジェクト間の寸法
            Reference ref1 = null;
            Reference ref2 = null;
            if (edgeList.Count > 0)
            {
                // ライトダイレクションと平行かどうか
                if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.RightDirection))
                {
                    ref1 = edgeList.Last().Reference;
                }
                // アップダイレクションと 平行かどうか
                else if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.UpDirection))
                {
                    ref1 = edgeList.Last().Reference;
                }
                else
                {
                    ref1 = edgeList.Last().Reference;
                }
            }
            else if (faceList.Count > 0)
            {
                ref1 = faceList.Last().Reference;
            }
            else if (mDl1 != null)
            {
                ref1 = mDl1.GeometryCurve.Reference;
            }
            if (element1.GetType().Name == "Grid")
            {
                ref1 = new Reference(element1);
            }
            if (edgeList2.Count > 0)
            {
                // ライトダイレクションと平行かどうか
                if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.RightDirection))
                {
                    ref2 = edgeList2.First().Reference;
                }
                // アップダイレクションと平行かどうか
                else if (IsParallel(NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection), Doc.ActiveView.UpDirection))
                {
                    ref2 = edgeList2.First().Reference;
                }
                else
                {
                    ref2 = edgeList2.First().Reference;
                }
            }
            else if (faceList2.Count > 0)
            {
                ref2 = faceList2.First().Reference;
            }
            else if (mDl2 != null)
            {
                ref2 = mDl2.GeometryCurve.Reference;
            }
            if (element2.GetType().Name == "Grid")
            {
                ref2 = new Reference(element2);
            }
            if (ref1 != null && ref2 != null)
            {
                // 寸法作成処理
                CreateDimensionElement(Doc.ActiveView,
                point1, ref1,
                point2, ref2, element1, element2);
            }
        }

        /// <summary>
        /// XY平面以外の面-面寸法作成(自オブジェクト間)
        /// </summary>
        /// <param name="element">自オブジェクト要素</param>
        /// <param name="elementDirection">要素方向</param>
        /// <param name="activeView">アクティブビュー</param>
        /// <param name="verticalNormal">要素直行方向単位ベクトル</param>
        /// <param name="point1">寸法作成位置(ユーザ選択)</param>
        /// <param name="point2">寸法作成位置から寸法作成方向にずれた点</param>
        /// <param name="needDimention">寸法作成要否</param>
        /// <param name="isStartSide">2要素間の寸法作成時、寸法作成順で手前側のオブジェクトである</param>
        /// <param name="faceList">オブジェクトのFaceリスト</param>
        /// <param name="edgeList">オブジェクトのEdgeリスト</param>
        /// <param name="mDl">作成した詳細線分(より外側のもの)</param>
        private void CreateCrossSectionFaceToFaceBetweenOwn(Element element, XYZ elementDirection, View activeView, XYZ verticalNormal, XYZ point1, XYZ point2, bool needDimention, bool isStartSide,
                                                            out List<Face> faceList, out List<Edge> edgeList, out DetailCurve mDl)
        {
            faceList = new List<Face>();
            edgeList = new List<Edge>();
            mDl = null;

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.View = activeView;

            // エッジ取得
            if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
            {
                edgeList = GetInstanceEdgeListXZ(element, elementDirection, opt);
            }
            else
            {
                edgeList = GetEdgeListXZ(element, elementDirection, opt);
            }
            // フェイス取得
            if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
            {
                List<Face> list = GetInstanceClosestFaceList(element, verticalNormal, opt);
                List<int> intList = new List<int>();
                foreach (Face face in list)
                {
                    intList.Add(face.Id);
                }
                faceList = GetSymbolClosestFaceList(element, intList, opt);
            }
            else
            {
                faceList = GetClosestFaceList(element, verticalNormal, opt);
                if (faceList.Count > 0)
                {
                    // ソート
                    faceList = FaceSortPointByDirection(verticalNormal, faceList);
                }
            }
            Reference ref1 = null;
            Reference ref2 = null;
            if (edgeList.Count > 1 && needDimention)
            {
                for (int j = 0; j < edgeList.Count - 1; j++)
                {
                    ref1 = edgeList[j].Reference;
                    ref2 = edgeList[j + 1].Reference;
                    if (ref1 != null && ref2 != null)
                    {
                        CreateDimensionElement(Doc.ActiveView,
                        point1, ref1,
                        point2, ref2, element, element);
                    }
                }
            }
            else if (faceList.Count > 1 && needDimention)
            {
                for (int j = 0; j < faceList.Count - 1; j++)
                {
                    ref1 = faceList[j].Reference;
                    ref2 = faceList[j + 1].Reference;
                    if (ref1 != null && ref2 != null)
                    {
                        CreateDimensionElement(Doc.ActiveView,
                        point1, ref1,
                        point2, ref2, element, element);
                    }
                }
            }
            else
            {
                // 詳細線分作成
                LocationCurve locationCurve = element.Location as LocationCurve;
                // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                if (locationCurve != null && lineCheck.Checked)
                {
                    Line line = (Line)locationCurve.Curve;

                    // 平行かどうか
                    if (IsParallel(line.Direction, elementDirection))
                    {
                        List<Solid> solidList = new List<Solid>();
                        if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
                        {
                            solidList = GetInstanceSolid(element);
                        }
                        else
                        {
                            solidList = GetSolid(element);
                        }
                        if (solidList == null)
                        {
                            return;
                        }
                        Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                        List<XYZ> xyzList = new List<XYZ>();
                        foreach (KeyValuePair<XYZ, int> kvp in cornerDic)
                        {
                            xyzList.Add(kvp.Key);
                        }
                        List<XYZ> cornerList = GetCoordinateXY(xyzList, line.Direction);
                        cornerList.Sort((a, b) => Math.Sign(a.X - b.X));
                        XYZ maxX = cornerList.Last();
                        cornerList.Sort((a, b) => Math.Sign(b.X - a.X));
                        XYZ minX = cornerList.First();
                        cornerList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                        XYZ maxY = cornerList.Last();
                        if (maxY.X == minX.X)
                        {
                            maxY = cornerList[cornerList.Count - 2];
                        }
                        XYZ minY = cornerList.First();
                        if (minY.X == maxX.X)
                        {
                            minY = cornerList[1];
                        }
                        using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE))
                        {
                            t.Start();
                            Line l1 = Line.CreateBound(minY, maxX);
                            DetailCurve dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                            Line l2 = Line.CreateBound(minX, maxY);
                            DetailCurve dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                            t.Commit();
                            ref1 = dl1.GeometryCurve.Reference;
                            ref2 = dl2.GeometryCurve.Reference;
                            List<DetailCurve> dlList = new List<DetailCurve>();
                            dlList.Add(dl1);
                            dlList.Add(dl2);
                            XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                            XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                            double distance1 = GetSpecificDirectionDistance(c1, verticalNormal);
                            double distance2 = GetSpecificDirectionDistance(c2, verticalNormal);
                            if (isStartSide && distance1 < distance2 || !isStartSide && distance1 > distance2)
                            {
                                mDl = dl2;
                            }
                            else
                            {
                                mDl = dl1;
                            }
                        }
                    }
                }
                // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                else if (lineCheck.Checked && element.GetType().Name != "Grid")
                {
                    Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                    xyzList = GetPoint(element, line);
                    if (xyzList.Count == 0)
                    {
                        List<Solid> solidList = new List<Solid>();
                        if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart")
                        {
                            solidList = GetInstanceSolid(element);
                        }
                        else
                        {
                            solidList = GetSolid(element);
                        }
                        if (solidList == null)
                        {
                            return;
                        }
                        Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                        List<XYZ> xyzList = new List<XYZ>();
                        foreach (KeyValuePair<XYZ, int> kvp in cornerDic)
                        {
                            xyzList.Add(kvp.Key);
                        }
                    }
                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                    if (cornerList.Count == 0)
                    {
                        return;
                    }
                    double dz = cornerList[0].Z;
                    List<XYZ> cornerList2 = new List<XYZ>();
                    foreach (XYZ xyz in cornerList)
                    {
                        if (xyz.Z == dz)
                        {
                            cornerList2.Add(xyz);
                        }
                    }
                    cornerList = cornerList2;
                    XYZ p1 = cornerList[0];
                    XYZ p2 = cornerList[0];
                    foreach (XYZ xyz in cornerList)
                    {
                        if (p1 == xyz)
                        {
                            continue;
                        }
                        Line l = Line.CreateBound(p1, xyz);
                        // 平行かどうか
                        if (IsParallel(elementDirection, l.Direction))
                        {
                            p2 = xyz;
                        }
                    }
                    cornerList.Remove(p1);
                    cornerList.Remove(p2);
                    XYZ p3 = cornerList.First();
                    XYZ p4 = cornerList.Last();
                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE))
                    {
                        t.Start();
                        FilteredElementCollector detailLineCollection =
                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                        .OfCategory(BuiltInCategory.OST_Lines);
                        List<CurveElement> lineList = new List<CurveElement>();
                        int co = detailLineCollection.Count();
                        foreach (CurveElement dc in detailLineCollection)
                        {
                            if (dc.GetType().Name != "DetailLine")
                            {
                                continue;
                            }
                            if (dc.OwnerViewId == Doc.ActiveView.Id)
                            {
                                lineList.Add(dc);
                            }
                        }
                        DetailCurve dl1 = null;
                        DetailCurve dl2 = null;
                        Line l1 = Line.CreateBound(p1, p2);
                        foreach (CurveElement dc in lineList)
                        {
                            if (dc.GetType().Name != "DetailLine")
                            {
                                continue;
                            }
                            DetailLine detailLine = (DetailLine)dc;
                            Line dcLine = detailLine.GeometryCurve as Line;
                            if (dcLine == null)
                            {
                                continue;
                            }
                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                && dcLine.Direction == l1.Direction)
                            {
                                dl1 = (DetailLine)dc;
                                break;
                            }
                        }
                        // 重複するかどうかの判定用フラグ
                        bool breakFlag = true;
                        foreach (Line dcLine in createLineList)
                        {
                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                && dcLine.Direction == l1.Direction)
                            {
                                breakFlag = false;
                                break;
                            }
                        }
                        if (dl1 == null && breakFlag)
                        {
                            createLineList.Add(l1);
                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                        }
                        Line l2 = Line.CreateBound(p3, p4);

                        foreach (CurveElement dc in lineList)
                        {
                            if (dc.GetType().Name != "DetailLine")
                            {
                                continue;
                            }
                            LocationCurve lc = dc.Location as LocationCurve;
                            Line dcLine = lc.Curve as Line;
                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                && dcLine.Direction == l2.Direction)
                            {
                                dl2 = (DetailLine)dc;
                                break;
                            }
                        }
                        breakFlag = true;
                        foreach (Line dcLine in createLineList)
                        {
                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                && dcLine.Direction == l2.Direction)
                            {
                                breakFlag = false;
                                break;
                            }
                        }
                        if (dl2 == null && breakFlag)
                        {
                            createLineList.Add(l2);
                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                        }
                        t.Commit();
                        ref1 = dl1.GeometryCurve.Reference;
                        ref2 = dl2.GeometryCurve.Reference;
                        List<DetailCurve> dlList = new List<DetailCurve>();
                        dlList.Add(dl1);
                        dlList.Add(dl2);
                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                        double distance1 = GetSpecificDirectionDistance(c1, verticalNormal);
                        double distance2 = GetSpecificDirectionDistance(c2, verticalNormal);
                        if (isStartSide && distance1 < distance2 || !isStartSide && distance1 > distance2)
                        {
                            mDl = dl2;
                        }
                        else
                        {
                            mDl = dl1;
                        }
                    }
                }
                if (ref1 != null && ref2 != null && needDimention)
                {
                    // 寸法作成処理
                    CreateDimensionElement(Doc.ActiveView,
                    point1, ref1,
                    point2, ref2, element, element);
                }
            }
        }

        /// <summary>
        /// 寸法作成XY
        /// </summary>
        /// <param name=" newElementList">要素リスト</param>
        /// <param name="elementDirection">要素の方向</param>
        /// <param name="activeView">アクティブビュー</param>
        private void CreateXY(List<Element> newElementList, XYZ elementDirection, View activeView)
        {
            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(elementDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                return;
            }
            int countXY = 0;
            Element e1 = null;
            Element e2 = null;
            for (int i = 0; i < newElementList.Count - 1; i++) {
                try {
                    Element element1 = newElementList[i];
                    Element element2 = newElementList[i + 1];
                    if (element1.Id.ToString() == element2.Id.ToString()) {
                        continue;
                    }
                    e1 = element1;
                    e2 = element2;

                    LocationCurve locationCurve1 = element1.Location as LocationCurve;
                    Curve curve1 = null;
                    if (locationCurve1 != null) {
                        curve1 = locationCurve1.Curve;
                    }
                    else if (element1.GetType().Name == "Grid") {
                        Grid grid = (Grid)element1;
                        curve1 = grid.Curve;
                    }

                    XYZ lineDirection1 = new XYZ(0, 0, 0);
                    if (curve1 != null) {
                        Line line1 = (Line)curve1;
                        lineDirection1 = line1.Direction;
                    }

                    LocationCurve locationCurve2 = element2.Location as LocationCurve;
                    Curve curve2 = null;
                    if (locationCurve2 != null) {
                        curve2 = locationCurve2.Curve;
                    }
                    else if (element2.GetType().Name == "Grid") {
                        Grid grid = (Grid)element2;
                        curve2 = grid.Curve;
                    }
                    XYZ lineDirection2 = new XYZ(0, 0, 0);
                    if (curve2 != null) {
                        Line line2 = (Line)curve2;
                        lineDirection2 = line2.Direction;
                    }
                    Reference r1 = new Reference(element1);
                    Reference r2 = new Reference(element2);
                    // 芯用ディクショナリー
                    Dictionary<Reference, XYZ> refDic1 = new Dictionary<Reference, XYZ>();
                    Dictionary<Reference, XYZ> refDic2 = new Dictionary<Reference, XYZ>();
                    // 芯を取得
                    if (element1.GetType().Name == "FamilyInstance") {
                        FamilyInstance ins1 = (FamilyInstance)element1;
                        // 中心正面/背面
                        IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                        // 中心左/右
                        IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                        // 中心立面図
                        IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                        if (frontBack.Count > 0) {
                            foreach (Reference r in frontBack) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                        if (leftRight.Count > 0) {
                            foreach (Reference r in leftRight) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                        if (centerElevation.Count > 0) {
                            foreach (Reference r in centerElevation) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                    }
                    else if (element1.GetType().Name == "FabricationPart") {
                        int lineInt = GetInstanceLine(element1, elementDirection);
                        Line line = GetSymbolLine(element1, lineInt);
                        if (line != null) {
                            refDic1.Add(line.Reference, elementDirection);
                        }
                    }
                    // ファミリインスタンス、ファブリケーションパーツ以外の場合
                    else {
                        r1 = new Reference(element1);
                        locationCurve1 = element1.Location as LocationCurve;
                        curve1 = null;
                        if (locationCurve1 != null) {
                            curve1 = locationCurve1.Curve;
                        }
                        else if (element1.GetType().Name == "Grid") {
                            Grid grid = (Grid)element1;
                            curve1 = grid.Curve;
                        }
                        Line line1 = null;
                        lineDirection1 = new XYZ(0, 0, 0);
                        if (curve1 != null) {
                            line1 = (Line)curve1;
                            lineDirection1 = line1.Direction;
                        }
                        refDic1.Add(r1, lineDirection1);
                    }
                    if (element2.GetType().Name == "FamilyInstance") {
                        FamilyInstance ins2 = (FamilyInstance)element2;
                        // 中心正面/背面
                        IList<Reference> frontBack = ins2.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                        // 中心左/右
                        IList<Reference> leftRight = ins2.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                        // 中心立面図
                        IList<Reference> centerElevation = ins2.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                        if (frontBack.Count > 0) {
                            foreach (Reference r in frontBack) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic2.Add(r, v);
                            }
                        }
                        if (leftRight.Count > 0) {
                            foreach (Reference r in leftRight) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic2.Add(r, v);
                            }
                        }
                        if (centerElevation.Count > 0) {
                            foreach (Reference r in centerElevation) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic2.Add(r, v);
                            }
                        }
                    }
                    else if (element2.GetType().Name == "FabricationPart") {
                        int lineInt = GetInstanceLine(element2, elementDirection);
                        Line line = GetSymbolLine(element2, lineInt);
                        if (line != null) {
                            refDic2.Add(line.Reference, elementDirection);
                        }
                    }
                    // ファミリインスタンス、ファブリケーションパーツ以外の場合
                    else {
                        r2 = new Reference(element2);
                        locationCurve2 = element2.Location as LocationCurve;
                        curve2 = null;
                        if (locationCurve2 != null) {
                            curve2 = locationCurve2.Curve;
                        }
                        else if (element2.GetType().Name == "Grid") {
                            Grid grid = (Grid)element2;
                            curve2 = grid.Curve;
                        }
                        lineDirection2 = new XYZ(0, 0, 0);
                        if (curve2 != null) {
                            Line line2 = (Line)curve2;
                            lineDirection2 = line2.Direction;
                        }
                        refDic2.Add(r2, lineDirection2);
                    }
                    XYZ midPoint1 = new XYZ(1, 0, 0);
                    XYZ midPoint2 = new XYZ(2, 0, 0);
                    // 面-面を選択した場合
                    if (faceRadio.Checked) {
                        BoundingBoxXYZ box1 = element1.get_BoundingBox(null);
                        BoundingBoxXYZ box2 = element2.get_BoundingBox(null);
                        if (box1 != null) {
                            midPoint1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, 0);
                        }
                        if (box2 != null) {
                            midPoint2 = new XYZ((box2.Max.X + box2.Min.X) / 2, (box2.Max.Y + box2.Min.Y) / 2, 0);
                        }
                        XYZ point1 = XYZ.Zero;
                        XYZ point2 = XYZ.Zero;
                        Options opt = new Options();
                        opt.ComputeReferences = true;
                        opt.View = activeView;
                        opt.IncludeNonVisibleObjects = false;
                        Line elementLine = Line.CreateBound(XYZ.Zero, elementDirection);
                        XYZ elementDirectionNormal = NormalViewDirection(elementLine, activeView.ViewDirection);
                        point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisX);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisY);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisZ);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                        }
                        else {
                            point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        }
                        XYZ normal1 = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), activeView.ViewDirection);
                        XYZ normal2 = normal1;
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisX);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisX);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisY);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisY);
                            }
                            else {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisZ);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisZ);
                            }
                        }
                        XYZ verticalNormal = new XYZ(0, 0, 0);
                        DetailCurve dl1 = null;
                        DetailCurve dl2 = null;
                        DetailCurve dl3 = null;
                        DetailCurve dl4 = null;
                        DetailCurve mDl1 = null;
                        DetailCurve mDl2 = null;
                        List<Face> faceList = new List<Face>();
                        // 自オブジェクト間の寸法
                        // フェイス取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            List<Face> list = GetInstanceClosestFaceList(element1, normal1, opt);
                            List<int> intList = new List<int>();
                            foreach (Face face in list) {
                                intList.Add(face.Id);
                            }
                            faceList = GetSymbolClosestFaceList(element1, intList, opt);
                        }
                        else {
                            faceList = GetClosestFaceList(element1, normal1, opt);
                            if (faceList.Count > 0) {
                                XYZ sortDirection = normal1;
                                faceList = FaceSortPointByDirection(sortDirection, faceList);
                            }
                        }
                        List<Edge> edgeList = new List<Edge>();
                        // エッジ取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            edgeList = GetInstanceEdgeListXY(element1, elementDirection, opt);
                        }
                        else {
                            edgeList = GetEdgeListXY(element1, elementDirection, opt, midPoint2);
                        }
                        Reference ref1 = null;
                        Reference ref2 = null;
                        if (faceList.Count > 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = faceList.Last().Reference;
                        }
                        else if (faceList.Count == 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = null;
                        }
                        else if (edgeList.Count > 1) {
                            ref1 = edgeList.First().Reference;
                            ref2 = edgeList.Last().Reference;
                        }
                        else {
                            // 詳細線分作成
                            LocationCurve locationCurve = element1.Location as LocationCurve;
                            // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                            if (locationCurve != null && lineCheck.Checked) {
                                Line line = (Line)locationCurve1.Curve;
                                XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                }
                                else {
                                    XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                    line = Line.CreateBound(XYZ.Zero, v2);
                                }
                                // 平行かどうか
                                if (IsParallel(line.Direction, elementDirection)) {
                                    List<Solid> solidList = new List<Solid>();
                                    if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                        solidList = GetInstanceSolid(element1);
                                    }
                                    else {
                                        solidList = GetSolid(element1);
                                    }
                                    if (solidList == null) {
                                        break;
                                    }
                                    Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                    List<XYZ> xyzList = new List<XYZ>();
                                    foreach (KeyValuePair<XYZ, int> kvp in cornerDic) {
                                        XYZ n = new XYZ(kvp.Key.X, kvp.Key.Y, 0);
                                        xyzList.Add(n);
                                    }
                                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                            // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                            else if (lineCheck.Checked && element1.GetType().Name != "Grid") {
                                Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                xyzList = GetPoint(element1, line);
                                if (xyzList.Count > 0) {
                                    List<XYZ> cornerList = new List<XYZ>();
                                    xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                    double minX = xyzList.First().X;
                                    double maxX = xyzList.Last().X;
                                    xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                    double minY = xyzList.First().Y;
                                    double maxY = xyzList.Last().Y;
                                    XYZ po1 = new XYZ(minX, minY, 0);
                                    XYZ po2 = new XYZ(maxX, minY, 0);
                                    XYZ po3 = new XYZ(minX, maxY, 0);
                                    XYZ po4 = new XYZ(maxX, maxY, 0);
                                    cornerList.Add(po1);
                                    cornerList.Add(po2);
                                    cornerList.Add(po3);
                                    cornerList.Add(po4);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    double z = cornerList[0].Z;
                                    List<XYZ> cornerList2 = new List<XYZ>();
                                    foreach (XYZ xyz in cornerList) {
                                        if (xyz.Z == z) {
                                            cornerList2.Add(xyz);
                                        }
                                    }
                                    cornerList = cornerList2;
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                        }
                        if (ref1 != null && ref2 != null && countXY == 0) {
                            // 寸法作成処理
                            CreateDimensionElement(Doc.ActiveView,
                            point1, ref1,
                            point2, ref2, element1, element1);
                        }
                        else if (ref1 == null && ref2 == null) {
                            e1 = element1;
                            if (e1 != null) {
                                errorIdSet.Add(e1.Id.ToString());
                            }
                        }
                        countXY++;
                        ref1 = null;
                        ref2 = null;
                        List<Face> faceList2 = GetClosestFaceList(element2, normal2, opt);
                        List<Edge> edgeList2 = new List<Edge>();
                        // エッジ取得
                        if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                            edgeList2 = GetInstanceEdgeListXY(element2, elementDirection, opt);
                        }
                        else {
                            edgeList2 = GetEdgeListXY(element2, elementDirection, opt, midPoint1);
                        }
                        // フェイス取得
                        if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                            List<Face> list = GetInstanceClosestFaceList(element2, normal1, opt);
                            List<int> intList = new List<int>();
                            foreach (Face face in list) {
                                intList.Add(face.Id);
                            }
                            faceList2 = GetSymbolClosestFaceList(element2, intList, opt);
                        }
                        else {
                            faceList2 = GetClosestFaceList(element2, normal2, opt);
                            if (faceList2.Count > 0) {
                                XYZ sortDirection = normal2;
                                // ソート
                                faceList2 = FaceSortPointByDirection(sortDirection, faceList2);
                            }
                        }
                        if (faceList2.Count > 1) {
                            ref1 = faceList2.First().Reference;
                            ref2 = faceList2.Last().Reference;
                        }
                        else if (faceList2.Count == 1) {
                            ref1 = faceList2.First().Reference;
                            ref2 = null;
                        }
                        else if (edgeList2.Count > 1) {
                            ref1 = edgeList2.First().Reference;
                            ref2 = edgeList2.Last().Reference;
                        }
                        else {
                            // 詳細線分作成
                            LocationCurve locationCurve = element2.Location as LocationCurve;
                            // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                            if (locationCurve != null && lineCheck.Checked) {
                                Line line = (Line)locationCurve1.Curve;
                                XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                }
                                else {
                                    XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                    line = Line.CreateBound(XYZ.Zero, v2);
                                }
                                // 平行かどうか
                                if (IsParallel(line.Direction, elementDirection)) {
                                    List<Solid> solidList = new List<Solid>();
                                    if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                                        solidList = GetInstanceSolid(element2);
                                    }
                                    else {
                                        solidList = GetSolid(element2);
                                    }
                                    if (solidList == null) {
                                        break;
                                    }
                                    Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                    List<XYZ> xyzList = new List<XYZ>();
                                    foreach (KeyValuePair<XYZ, int> kvp in cornerDic) {
                                        XYZ n = new XYZ(kvp.Key.X, kvp.Key.Y, 0);
                                        xyzList.Add(n);
                                    }
                                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        List<CurveElement> lineList = GetElements<CurveElement>(Doc);
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                dl3 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl3 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl3 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (dcLine.Origin.X == l2.Origin.X && dcLine.Origin.Y == l2.Origin.Y) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl4 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl4 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl3.GeometryCurve.Reference;
                                        ref2 = dl4.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl3);
                                        dlList.Add(dl4);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint1, c1);
                                        double distance2 = GetDistance(midPoint1, c2);
                                        if (distance1 > distance2) {
                                            mDl2 = dl4;
                                        }
                                        else {
                                            mDl2 = dl3;
                                        }
                                    }
                                }
                            }
                            // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                            else if (lineCheck.Checked && element2.GetType().Name != "Grid") {
                                Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                List<XYZ> cornerList = new List<XYZ>();
                                xyzList = GetPoint(element2, line);
                                if (xyzList.Count > 0) {
                                    xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                    double minX = xyzList.First().X;
                                    double maxX = xyzList.Last().X;
                                    xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                    double minY = xyzList.First().Y;
                                    double maxY = xyzList.Last().Y;
                                    XYZ po1 = new XYZ(minX, minY, 0);
                                    XYZ po2 = new XYZ(maxX, minY, 0);
                                    XYZ po3 = new XYZ(minX, maxY, 0);
                                    XYZ po4 = new XYZ(maxX, maxY, 0);
                                    cornerList.Add(po1);
                                    cornerList.Add(po2);
                                    cornerList.Add(po3);
                                    cornerList.Add(po4);

                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl3 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl3 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl3 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                 && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl4 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl4 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl3.GeometryCurve.Reference;
                                        ref2 = dl4.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl3);
                                        dlList.Add(dl4);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl2 = dl4;
                                        }
                                        else {
                                            mDl2 = dl3;
                                        }
                                    }
                                }
                            }
                        }
                        if (ref1 != null && ref2 != null) {
                            // 寸法作成処理
                            CreateDimensionElement(Doc.ActiveView,
                            point1, ref1,
                            point2, ref2, element2, element2);
                        }
                        else if (ref1 == null && ref2 == null) {
                            e2 = element2;
                            if (e2 != null) {
                                errorIdSet.Add(e2.Id.ToString());
                            }
                        }

                        // 自他オブジェクト間の寸法
                        ref1 = null;
                        ref2 = null;
                        Face face1 = null;
                        Face face2 = null;
                        if (faceList.Count > 0) {
                            face1 = faceList.Last();
                        }
                        if (faceList2.Count > 0) {
                            face2 = faceList2.First();
                        }
                        if (face1 != null) {
                            ref1 = face1.Reference;
                        }
                        else if (edgeList.Count > 0) {
                            ref1 = edgeList.Last().Reference;
                        }
                        else if (mDl1 != null) {
                            ref1 = mDl1.GeometryCurve.Reference;
                        }
                        if (element1.GetType().Name == "Grid") {
                            ref1 = r1;
                        }
                        if (face2 != null) {
                            ref2 = face2.Reference;
                        }
                        else if (edgeList2.Count > 0) {
                            ref2 = edgeList2.First().Reference;
                        }
                        else if (mDl2 != null) {
                            ref2 = mDl2.GeometryCurve.Reference;
                        }
                        if (element2.GetType().Name == "Grid") {
                            ref2 = r2;
                        }
                        if (ref1 != null && ref2 != null) {
                            CreateDimensionElement(Doc.ActiveView,
                            point1, ref1,
                            point2, ref2, element1, element2);
                        }
                    }
                    // 芯-芯を選択した場合
                    else if (coreRadio.Checked) {
                        Line elementLine = Line.CreateBound(XYZ.Zero, elementDirection);
                        XYZ elementDirectionNormal = NormalViewDirection(elementLine, activeView.ViewDirection);

                        foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                            foreach (KeyValuePair<Reference, XYZ> kvp2 in refDic2) {
                                Line l1 = Line.CreateBound(XYZ.Zero, kvp.Value);
                                Line l2 = Line.CreateBound(XYZ.Zero, kvp2.Value);
                                XYZ p1 = NormalViewDirection(l1, Doc.ActiveView.ViewDirection);
                                XYZ p2 = NormalViewDirection(l2, Doc.ActiveView.ViewDirection);
                                // 平行かどうか
                                if (IsParallel(p1, p2)) {
                                    XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                                    // 平行でない場合コンティニュー
                                    if (!IsParallel(p1, vDirection) || !IsParallel(p2, vDirection)) {
                                        continue;
                                    }
                                    midPoint1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                                    if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                        if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                            XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisX);
                                            midPoint2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                        }
                                        else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                            XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisY);
                                            midPoint2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                        }
                                        else {
                                            XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisZ);
                                            midPoint2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                        }
                                    }
                                    else {
                                        midPoint2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                                    }
                                    // 寸法作成処理
                                    CreateDimensionElement(Doc.ActiveView, midPoint1, kvp.Key, midPoint2, kvp2.Key, element1, element2);
                                }
                            }
                        }
                    }
                    // 面-芯を選択した場合
                    else {
                        BoundingBoxXYZ box1 = element1.get_BoundingBox(null);
                        BoundingBoxXYZ box2 = element2.get_BoundingBox(null);
                        if (box1 != null) {
                            midPoint1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, 0);
                        }
                        if (box2 != null) {
                            midPoint2 = new XYZ((box2.Max.X + box2.Min.X) / 2, (box2.Max.Y + box2.Min.Y) / 2, 0);
                        }
                        XYZ point1 = XYZ.Zero;
                        XYZ point2 = XYZ.Zero;
                        Options opt = new Options();
                        opt.ComputeReferences = true;
                        opt.View = activeView;
                        opt.IncludeNonVisibleObjects = false;
                        Line elementLine = Line.CreateBound(XYZ.Zero, elementDirection);
                        XYZ elementDirectionNormal = NormalViewDirection(elementLine, activeView.ViewDirection);
                        point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisX);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisY);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisZ);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                        }
                        else {
                            point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        }
                        XYZ normal1 = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), activeView.ViewDirection);
                        XYZ normal2 = normal1;
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisX);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisX);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisY);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisY);
                            }
                            else {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisZ);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisZ);
                            }
                        }
                        XYZ verticalNormal = new XYZ(0, 0, 0);
                        DetailCurve dl1 = null;
                        DetailCurve dl2 = null;
                        DetailCurve dl3 = null;
                        DetailCurve dl4 = null;
                        DetailCurve mDl1 = null;
                        DetailCurve mDl2 = null;
                        List<Face> faceList = new List<Face>();
                        // 自オブジェクト間の寸法
                        // フェイス取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            List<Face> list = GetInstanceClosestFaceList(element1, normal1, opt);
                            List<int> intList = new List<int>();
                            foreach (Face face in list) {
                                intList.Add(face.Id);
                            }
                            faceList = GetSymbolClosestFaceList(element1, intList, opt);
                        }
                        else {
                            faceList = GetClosestFaceList(element1, normal1, opt);
                            if (faceList.Count > 0) {
                                XYZ sortDirection = normal1;
                                // ソート
                                faceList = FaceSortPointByDirection(sortDirection, faceList);
                            }
                        }
                        List<Edge> edgeList = new List<Edge>();
                        // エッジ取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            edgeList = GetInstanceEdgeListXY(element1, elementDirection, opt);
                        }
                        else {
                            edgeList = GetEdgeListXY(element1, elementDirection, opt, midPoint2);
                        }
                        Reference ref1 = null;
                        Reference ref2 = null;
                        if (faceList.Count > 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = faceList.Last().Reference;
                        }
                        else if (faceList.Count == 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = null;
                        }
                        else if (edgeList.Count > 1) {
                            ref1 = edgeList.First().Reference;
                            ref2 = edgeList.Last().Reference;
                        }
                        else {
                            // 詳細線分作成
                            LocationCurve locationCurve = element1.Location as LocationCurve;
                            // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                            if (locationCurve != null && lineCheck.Checked) {
                                Line line = (Line)locationCurve1.Curve;
                                XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                }
                                else {
                                    XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                    line = Line.CreateBound(XYZ.Zero, v2);
                                }
                                // 平行かどうか
                                if (IsParallel(line.Direction, elementDirection)) {
                                    List<Solid> solidList = new List<Solid>();
                                    if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                        solidList = GetInstanceSolid(element1);
                                    }
                                    else {
                                        solidList = GetSolid(element1);
                                    }
                                    if (solidList == null) {
                                        break;
                                    }
                                    Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                    List<XYZ> xyzList = new List<XYZ>();
                                    foreach (KeyValuePair<XYZ, int> kvp in cornerDic) {
                                        XYZ n = new XYZ(kvp.Key.X, kvp.Key.Y, 0);
                                        xyzList.Add(n);
                                    }
                                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                            // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                            else if (lineCheck.Checked && element1.GetType().Name != "Grid") {
                                Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                xyzList = GetPoint(element1, line);
                                if (xyzList.Count > 0) {
                                    List<XYZ> cornerList = new List<XYZ>();
                                    xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                    double minX = xyzList.First().X;
                                    double maxX = xyzList.Last().X;
                                    xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                    double minY = xyzList.First().Y;
                                    double maxY = xyzList.Last().Y;
                                    XYZ po1 = new XYZ(minX, minY, 0);
                                    XYZ po2 = new XYZ(maxX, minY, 0);
                                    XYZ po3 = new XYZ(minX, maxY, 0);
                                    XYZ po4 = new XYZ(maxX, maxY, 0);
                                    cornerList.Add(po1);
                                    cornerList.Add(po2);
                                    cornerList.Add(po3);
                                    cornerList.Add(po4);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    double z = cornerList[0].Z;
                                    List<XYZ> cornerList2 = new List<XYZ>();
                                    foreach (XYZ xyz in cornerList) {
                                        if (xyz.Z == z) {
                                            cornerList2.Add(xyz);
                                        }
                                    }
                                    cornerList = cornerList2;
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                        }
                        if (ref1 != null && ref2 != null && countXY == 0) {
                            foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                                // 芯と平行なものに対し、寸法を作成
                                if (IsParallel(kvp.Value, elementDirection)) {
                                    // 寸法作成処理
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, ref1,
                                    point2, kvp.Key, element1, element1);
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, kvp.Key,
                                    point2, ref2, element1, element1);
                                }
                            }
                        }
                        else if (ref1 != null && ref2 == null && countXY == 0) {
                            foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                                // 芯と平行なものに対し、寸法を作成
                                if (IsParallel(kvp.Value, elementDirection)) {
                                    // 寸法作成処理
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, ref1,
                                    point2, kvp.Key, element1, element1);
                                }
                            }
                        }
                        else if (ref1 == null && ref2 == null) {
                            e1 = element1;
                            if (e1 != null) {
                                errorIdSet.Add(e1.Id.ToString());
                            }
                        }
                        countXY++;
                        ref1 = null;
                        ref2 = null;
                        List<Face> faceList2 = GetClosestFaceList(element2, normal2, opt);
                        List<Edge> edgeList2 = new List<Edge>();
                        // エッジ取得
                        if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                            edgeList2 = GetInstanceEdgeListXY(element2, elementDirection, opt);
                        }
                        else {
                            edgeList2 = GetEdgeListXY(element2, elementDirection, opt, midPoint1);
                        }
                        // フェイス取得
                        if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                            List<Face> list = GetInstanceClosestFaceList(element2, normal1, opt);
                            List<int> intList = new List<int>();
                            foreach (Face face in list) {
                                intList.Add(face.Id);
                            }
                            faceList2 = GetSymbolClosestFaceList(element2, intList, opt);
                        }
                        else {
                            faceList2 = GetClosestFaceList(element2, normal2, opt);
                            if (faceList2.Count > 0) {
                                XYZ sortDirection = normal2;
                                // ソート
                                faceList2 = FaceSortPointByDirection(sortDirection, faceList2);
                            }
                        }
                        if (faceList2.Count > 1) {
                            ref1 = faceList2.First().Reference;
                            ref2 = faceList2.Last().Reference;
                        }
                        else if (faceList2.Count == 1) {
                            ref1 = faceList2.First().Reference;
                            ref2 = null;
                        }
                        else if (edgeList2.Count > 1) {
                            ref1 = edgeList2.First().Reference;
                            ref2 = edgeList2.Last().Reference;
                        }
                        else {
                            // 詳細線分作成
                            LocationCurve locationCurve = element2.Location as LocationCurve;
                            // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                            if (locationCurve != null && lineCheck.Checked) {
                                Line line = (Line)locationCurve1.Curve;
                                XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                }
                                else {
                                    XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                    line = Line.CreateBound(XYZ.Zero, v2);
                                }
                                // 平行かどうか
                                if (IsParallel(line.Direction, elementDirection)) {
                                    List<Solid> solidList = new List<Solid>();
                                    if (element2.GetType().Name == "FamilyInstance" || element2.GetType().Name == "FabricationPart") {
                                        solidList = GetInstanceSolid(element2);
                                    }
                                    else {
                                        solidList = GetSolid(element2);
                                    }
                                    if (solidList == null) {
                                        break;
                                    }
                                    Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                    List<XYZ> xyzList = new List<XYZ>();
                                    foreach (KeyValuePair<XYZ, int> kvp in cornerDic) {
                                        XYZ n = new XYZ(kvp.Key.X, kvp.Key.Y, 0);
                                        xyzList.Add(n);
                                    }
                                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        List<CurveElement> lineList = GetElements<CurveElement>(Doc);
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                dl3 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl3 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl3 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (dcLine.Origin.X == l2.Origin.X && dcLine.Origin.Y == l2.Origin.Y) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                ) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl4 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl4 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl3.GeometryCurve.Reference;
                                        ref2 = dl4.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl3);
                                        dlList.Add(dl4);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint1, c1);
                                        double distance2 = GetDistance(midPoint1, c2);
                                        if (distance1 > distance2) {
                                            mDl2 = dl4;
                                        }
                                        else {
                                            mDl2 = dl3;
                                        }
                                    }
                                }
                            }
                            // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                            else if (lineCheck.Checked && element2.GetType().Name != "Grid") {
                                Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                List<XYZ> cornerList = new List<XYZ>();
                                xyzList = GetPoint(element2, line);
                                if (xyzList.Count > 0) {
                                    xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                    double minX = xyzList.First().X;
                                    double maxX = xyzList.Last().X;
                                    xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                    double minY = xyzList.First().Y;
                                    double maxY = xyzList.Last().Y;
                                    XYZ po1 = new XYZ(minX, minY, 0);
                                    XYZ po2 = new XYZ(maxX, minY, 0);
                                    XYZ po3 = new XYZ(minX, maxY, 0);
                                    XYZ po4 = new XYZ(maxX, maxY, 0);
                                    cornerList.Add(po1);
                                    cornerList.Add(po2);
                                    cornerList.Add(po3);
                                    cornerList.Add(po4);

                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl3 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl3 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl3 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                dl4 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                 && Math.Round(dcLine.Direction.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Y, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Direction.Z, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Direction.Z, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl4 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl4 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl3.GeometryCurve.Reference;
                                        ref2 = dl4.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl3);
                                        dlList.Add(dl4);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint2, c1);
                                        double distance2 = GetDistance(midPoint2, c2);
                                        if (distance1 > distance2) {
                                            mDl2 = dl4;
                                        }
                                        else {
                                            mDl2 = dl3;
                                        }
                                    }
                                }
                            }
                        }
                        if (ref1 != null && ref2 != null) {
                            foreach (KeyValuePair<Reference, XYZ> kvp in refDic2) {
                                // 芯と平行なものに対し、寸法を作成
                                if (IsParallel(kvp.Value, elementDirection)) {
                                    // 寸法作成処理
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, ref1,
                                    point2, kvp.Key, element2, element2);
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, kvp.Key,
                                    point2, ref2, element2, element2);
                                }
                            }
                        }
                        else if (ref1 != null && ref2 == null) {
                            foreach (KeyValuePair<Reference, XYZ> kvp in refDic2) {
                                // 芯と平行なものに対し、寸法を作成
                                if (IsParallel(kvp.Value, elementDirection)) {
                                    // 寸法作成処理
                                    CreateDimensionElement(Doc.ActiveView,
                                    point1, ref1,
                                    point2, kvp.Key, element2, element2);
                                }
                            }
                        }
                        else if (ref1 == null && ref2 == null) {
                            e2 = element2;
                            if (e2 != null) {
                                errorIdSet.Add(e2.Id.ToString());
                            }
                        }

                        // 自他オブジェクト間の寸法
                        ref1 = null;
                        ref2 = null;
                        Face face1 = null;
                        Face face2 = null;
                        if (faceList.Count > 0) {
                            face1 = faceList.Last();
                        }
                        if (faceList2.Count > 0) {
                            face2 = faceList2.First();
                        }
                        if (face1 != null) {
                            ref1 = face1.Reference;
                        }
                        else if (edgeList.Count > 0) {
                            ref1 = edgeList.Last().Reference;
                        }
                        else if (mDl1 != null) {
                            ref1 = mDl1.GeometryCurve.Reference;
                        }
                        if (element1.GetType().Name == "Grid") {
                            ref1 = r1;
                        }
                        if (face2 != null) {
                            ref2 = face2.Reference;
                        }
                        else if (edgeList2.Count > 0) {
                            ref2 = edgeList2.First().Reference;
                        }
                        else if (mDl2 != null) {
                            ref2 = mDl2.GeometryCurve.Reference;
                        }
                        if (element2.GetType().Name == "Grid") {
                            ref2 = r2;
                        }
                        if (ref1 != null && ref2 != null) {
                            // 寸法作成処理
                            CreateDimensionElement(Doc.ActiveView,
                            point1, ref1,
                            point2, ref2, element1, element2);
                        }
                    }
                }
                catch (Exception ex) {
                    string exM = ex.Message;
                    if (e1 != null) {
                        errorIdSet.Add(e1.Id.ToString());
                    }
                    if (e2 != null) {
                        errorIdSet.Add(e2.Id.ToString());
                    }
                }
            }
            // 単体の場合
            if (newElementList.Count == 1) {
                try {
                    Element element1 = newElementList[0];
                    List<Line> lineList1 = new List<Line>();

                    LocationCurve locationCurve1 = element1.Location as LocationCurve;
                    Curve curve1 = null;
                    if (locationCurve1 != null) {
                        curve1 = locationCurve1.Curve;
                    }
                    else if (element1.GetType().Name == "Grid") {
                        return;
                    }

                    XYZ lineDirection1 = new XYZ(0, 0, 0);
                    if (curve1 != null) {
                        Line line1 = (Line)curve1;
                        lineDirection1 = line1.Direction;
                        lineList1.Add(line1);
                    }
                    Reference r1 = new Reference(element1);
                    // 芯用ディクショナリー
                    Dictionary<Reference, XYZ> refDic1 = new Dictionary<Reference, XYZ>();
                    // 芯を取得
                    if (element1.GetType().Name == "FamilyInstance") {
                        FamilyInstance ins1 = (FamilyInstance)element1;
                        // 中心正面/背面
                        IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                        // 中心左/右
                        IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                        // 中心立面図
                        IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                        if (frontBack.Count > 0) {
                            foreach (Reference r in frontBack) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                        if (leftRight.Count > 0) {
                            foreach (Reference r in leftRight) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                        if (centerElevation.Count > 0) {
                            foreach (Reference r in centerElevation) {
                                XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                                if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    break;
                                }
                                XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                                refDic1.Add(r, v);
                            }
                        }
                    }
                    else if (element1.GetType().Name == "FabricationPart") {
                        int lineInt = GetInstanceLine(element1, elementDirection);
                        Line line = GetSymbolLine(element1, lineInt);
                        lineList1.Add(line);
                        refDic1.Add(line.Reference, line.Direction);
                    }
                    // ファミリインスタンス、ファブリケーションパーツ以外の場合
                    else {
                        r1 = new Reference(element1);
                        locationCurve1 = element1.Location as LocationCurve;
                        curve1 = null;
                        if (locationCurve1 != null) {
                            curve1 = locationCurve1.Curve;
                        }
                        else if (element1.GetType().Name == "Grid") {
                            Grid grid = (Grid)element1;
                            curve1 = grid.Curve;
                        }
                        Line line1 = null;
                        lineDirection1 = new XYZ(0, 0, 0);
                        if (curve1 != null) {
                            line1 = (Line)curve1;
                            lineDirection1 = line1.Direction;
                        }
                        refDic1.Add(r1, lineDirection1);
                    }
                    XYZ midPoint1 = new XYZ(1, 0, 0);

                    // 傾き
                    double slope = elementDirection.Y / elementDirection.X;
                    double verticalSlop = -1 / slope;
                    // 切片
                    double modifyX = 1;
                    double modifyY = 0;
                    if (Double.IsNaN(verticalSlop)) {
                        modifyY = verticalSlop * modifyX;
                    }
                    // 面-面を選択した場合
                    if (faceRadio.Checked) {
                        BoundingBoxXYZ box1 = element1.get_BoundingBox(null);
                        midPoint1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, 0);
                        XYZ point1 = XYZ.Zero;
                        XYZ point2 = XYZ.Zero;
                        Options opt = new Options();
                        opt.ComputeReferences = true;
                        opt.View = activeView;
                        opt.IncludeNonVisibleObjects = false;
                        Line elementLine = Line.CreateBound(XYZ.Zero, elementDirection);
                        XYZ elementDirectionNormal = NormalViewDirection(elementLine, activeView.ViewDirection);
                        point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisX);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisY);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                            else {
                                XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisZ);
                                point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                            }
                        }
                        else {
                            point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                        }
                        XYZ normal1 = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), activeView.ViewDirection);
                        XYZ normal2 = normal1;
                        if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisX);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisX);
                            }
                            else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisY);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisY);
                            }
                            else {
                                normal1 = NormalViewDirection(elementLine, XYZ.BasisZ);
                                normal2 = NormalViewDirection(elementLine, XYZ.BasisZ);
                            }
                        }
                        XYZ verticalNormal = new XYZ(0, 0, 0);
                        DetailCurve dl1 = null;
                        DetailCurve dl2 = null;
                        DetailCurve mDl1 = null;
                        List<Face> faceList = new List<Face>();
                        // 自オブジェクト間の寸法
                        // フェイス取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            List<Face> list = GetInstanceClosestFaceList(element1, normal1, opt);
                            List<int> intList = new List<int>();
                            foreach (Face face in list) {
                                intList.Add(face.Id);
                            }
                            faceList = GetSymbolClosestFaceList(element1, intList, opt);
                        }
                        else {
                            faceList = GetClosestFaceList(element1, normal1, opt);
                            if (faceList.Count > 0) {
                                XYZ sortDirection = normal1;
                                // ソート
                                faceList = FaceSortPointByDirection(sortDirection, faceList);
                            }
                        }
                        List<Edge> edgeList = new List<Edge>();
                        // エッジ取得
                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                            edgeList = GetInstanceEdgeListXY(element1, elementDirection, opt);
                        }
                        else {
                            edgeList = GetEdgeListXY(element1, elementDirection, opt, midPoint1);
                        }
                        Reference ref1 = null;
                        Reference ref2 = null;
                        if (faceList.Count > 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = faceList.Last().Reference;
                        }
                        else if (faceList.Count == 1) {
                            ref1 = faceList.First().Reference;
                            ref2 = null;
                        }
                        else if (edgeList.Count > 1) {
                            ref1 = edgeList.First().Reference;
                            ref2 = edgeList.Last().Reference;
                        }
                        else {
                            // 詳細線分作成
                            LocationCurve locationCurve = element1.Location as LocationCurve;
                            // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                            if (locationCurve != null && lineCheck.Checked) {
                                Line line = (Line)locationCurve1.Curve;
                                XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                    && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                }
                                else {
                                    XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                    line = Line.CreateBound(XYZ.Zero, v2);
                                }
                                // 平行かどうか
                                if (IsParallel(line.Direction, elementDirection)) {
                                    List<Solid> solidList = new List<Solid>();
                                    if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                        solidList = GetInstanceSolid(element1);
                                    }
                                    else {
                                        solidList = GetSolid(element1);
                                    }
                                    if (solidList == null) {
                                        return;
                                    }
                                    Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                    List<XYZ> xyzList = new List<XYZ>();
                                    foreach (KeyValuePair<XYZ, int> kvp in cornerDic) {
                                        XYZ n = new XYZ(kvp.Key.X, kvp.Key.Y, 0);
                                        xyzList.Add(n);
                                    }
                                    List<XYZ> cornerList = GetCoordinateXY(xyzList, elementDirection);
                                    if (cornerList.Count == 0) {
                                        return;
                                    }
                                    elementDirection = new XYZ(elementDirection.X, elementDirection.Y, 0);
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint1, c1);
                                        double distance2 = GetDistance(midPoint1, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                            // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                            else if (lineCheck.Checked && element1.GetType().Name != "Grid") {
                                Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                xyzList = GetPoint(element1, line);
                                List<XYZ> cornerList = new List<XYZ>();
                                xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                double minX = xyzList.First().X;
                                double maxX = xyzList.Last().X;
                                xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                double minY = xyzList.First().Y;
                                double maxY = xyzList.Last().Y;
                                XYZ po1 = new XYZ(minX, minY, 0);
                                XYZ po2 = new XYZ(maxX, minY, 0);
                                XYZ po3 = new XYZ(minX, maxY, 0);
                                XYZ po4 = new XYZ(maxX, maxY, 0);
                                cornerList.Add(po1);
                                cornerList.Add(po2);
                                cornerList.Add(po3);
                                cornerList.Add(po4);
                                if (cornerList.Count == 0) {
                                    return;
                                }
                                double z = cornerList[0].Z;
                                List<XYZ> cornerList2 = new List<XYZ>();
                                foreach (XYZ xyz in cornerList) {
                                    if (xyz.Z == z) {
                                        cornerList2.Add(xyz);
                                    }
                                }
                                cornerList = cornerList2;
                                XYZ p1 = cornerList[0];
                                XYZ p2 = cornerList[0];
                                foreach (XYZ xyz in cornerList) {
                                    if (p1 == xyz) {
                                        continue;
                                    }
                                    Line l = Line.CreateBound(p1, xyz);
                                    // 平行かどうか
                                    if (IsParallel(elementDirection, l.Direction)) {
                                        p2 = xyz;
                                    }
                                }
                                cornerList.Remove(p1);
                                cornerList.Remove(p2);
                                XYZ p3 = cornerList.First();
                                XYZ p4 = cornerList.Last();
                                using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                    t.Start();
                                    FilteredElementCollector detailLineCollection =
                                    new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                    .OfCategory(BuiltInCategory.OST_Lines);
                                    List<CurveElement> lineList = new List<CurveElement>();
                                    int co = detailLineCollection.Count();
                                    foreach (CurveElement dc in detailLineCollection) {
                                        if (dc.GetType().Name != "DetailLine") {
                                            continue;
                                        }
                                        if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                            lineList.Add(dc);
                                        }
                                    }
                                    int count = lineList.Count();
                                    Line l1 = Line.CreateBound(p1, p2);
                                    foreach (CurveElement dc in lineList) {
                                        if (dc.GetType().Name != "DetailLine") {
                                            continue;
                                        }
                                        DetailLine detailLine = (DetailLine)dc;
                                        Line dcLine = detailLine.GeometryCurve as Line;
                                        if (dcLine == null) {
                                            continue;
                                        }
                                        if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                            && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                            && dcLine.Direction == l1.Direction) {
                                            dl1 = (DetailLine)dc;
                                            break;
                                        }
                                    }
                                    // 重複するかどうかの判定用フラグ
                                    bool breakFlag = true;
                                    foreach (Line dcLine in createLineList) {
                                        if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                            && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                            && dcLine.Direction == l1.Direction) {
                                            breakFlag = false;
                                            break;
                                        }
                                    }
                                    if (dl1 == null && breakFlag) {
                                        createLineList.Add(l1);
                                        dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                    }
                                    Line l2 = Line.CreateBound(p3, p4);

                                    foreach (CurveElement dc in lineList) {
                                        if (dc.GetType().Name != "DetailLine") {
                                            continue;
                                        }
                                        LocationCurve lc = dc.Location as LocationCurve;
                                        Line dcLine = lc.Curve as Line;
                                        if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                            && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                            && dcLine.Direction == l2.Direction) {
                                            dl2 = (DetailLine)dc;
                                            break;
                                        }
                                    }
                                    breakFlag = true;
                                    foreach (Line dcLine in createLineList) {
                                        if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                            && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                            && dcLine.Direction == l2.Direction) {
                                            breakFlag = false;
                                            break;
                                        }
                                    }
                                    if (dl2 == null && breakFlag) {
                                        createLineList.Add(l2);
                                        dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                    }
                                    t.Commit();
                                    ref1 = dl1.GeometryCurve.Reference;
                                    ref2 = dl2.GeometryCurve.Reference;
                                    List<DetailCurve> dlList = new List<DetailCurve>();
                                    dlList.Add(dl1);
                                    dlList.Add(dl2);
                                    XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                    XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                    double distance1 = GetDistance(midPoint1, c1);
                                    double distance2 = GetDistance(midPoint1, c2);
                                    if (distance1 > distance2) {
                                        mDl1 = dl2;
                                    }
                                    else {
                                        mDl1 = dl1;
                                    }
                                }
                            }
                        }
                        if (ref1 != null && ref2 != null) {
                            CreateDimensionElement(Doc.ActiveView,
                            point1, ref1,
                            point2, ref2, element1, element1);
                        }
                        else if (ref1 == null && ref2 == null) {
                            e1 = element1;
                            if (e1 != null) {
                                errorIdSet.Add(e1.Id.ToString());
                            }
                        }
                    }
                    // 芯-芯を選択した場合
                    else if (coreRadio.Checked) {
                        // 処理無し
                    }
                    // 面- 芯を選択した場合
                    else {
                        foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                            BoundingBoxXYZ box1 = element1.get_BoundingBox(activeView);
                            midPoint1 = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, (box1.Max.Z + box1.Min.Z) / 2);
                            Options opt = new Options();
                            opt.ComputeReferences = true;
                            opt.View = activeView;
                            opt.IncludeNonVisibleObjects = false;
                            XYZ point1 = new XYZ(0, 0, 0);
                            XYZ point2 = new XYZ(0, 0, 0);
                            // 平行でない場合コンティニュー
                            if (!IsParallel(kvp.Value, elementDirection)) {
                                continue;
                            }
                            Line elementLine = Line.CreateBound(XYZ.Zero, elementDirection);
                            XYZ elementDirectionNormal = NormalViewDirection(elementLine, activeView.ViewDirection);
                            point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                            point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                            if (Math.Round(elementDirectionNormal.X, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Y, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(elementDirectionNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                    XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisX);
                                    point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                }
                                else if (Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                    XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisY);
                                    point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                }
                                else {
                                    XYZ xyz = NormalViewDirection(elementLine, XYZ.BasisZ);
                                    point2 = new XYZ(dimensionPoint.X + xyz.X, dimensionPoint.Y + xyz.Y, dimensionPoint.Z + xyz.Z);
                                }
                            }
                            else {
                                point2 = new XYZ(dimensionPoint.X + elementDirectionNormal.X, dimensionPoint.Y + elementDirectionNormal.Y, dimensionPoint.Z + elementDirectionNormal.Z);
                            }
                            XYZ verticalNormal = new XYZ(0, 0, 0);
                            verticalNormal = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), activeView.ViewDirection); ;
                            DetailCurve dl1 = null;
                            DetailCurve dl2 = null;

                            DetailCurve mDl1 = null;
                            List<Face> faceList = new List<Face>();
                            // 自オブジェクト間の寸法
                            // フェイス取得
                            if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                List<Face> list = GetInstanceClosestFaceList(element1, verticalNormal, opt);
                                List<int> intList = new List<int>();
                                foreach (Face face in list) {
                                    intList.Add(face.Id);
                                }
                                faceList = GetSymbolClosestFaceList(element1, intList, opt);
                            }
                            else {
                                faceList = GetClosestFaceList(element1, verticalNormal, opt);
                                if (faceList.Count > 0) {
                                    XYZ sortDirection = verticalNormal;
                                    // ソート
                                    faceList = FaceSortPointByDirection(sortDirection, faceList);
                                }
                            }
                            List<Edge> edgeList = new List<Edge>();
                            // エッジ取得
                            if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                edgeList = GetInstanceEdgeListXY(element1, elementDirection, opt);
                            }
                            else {
                                edgeList = GetEdgeListXY(element1, elementDirection, opt, midPoint1);
                            }
                            Reference ref1 = null;
                            Reference ref2 = null;
                            if (faceList.Count > 1) {
                                ref1 = faceList.First().Reference;
                                ref2 = faceList.Last().Reference;
                            }
                            else if (faceList.Count == 1) {
                                ref1 = faceList.First().Reference;
                                ref2 = null;
                            }
                            else if (edgeList.Count > 1) {
                                ref1 = edgeList.First().Reference;
                                ref2 = edgeList.Last().Reference;
                            }
                            else if (edgeList.Count > 1) {
                                ref1 = edgeList.First().Reference;
                                ref2 = null;
                            }
                            else {
                                // 詳細線分作成
                                LocationCurve locationCurve = element1.Location as LocationCurve;
                                // ロケーションカーブがnullではなく、詳細線分作成にチェックを入れた場合
                                if (locationCurve != null && lineCheck.Checked) {
                                    Line line = (Line)locationCurve1.Curve;
                                    XYZ v = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                                    if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                                        && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                                        && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                    }
                                    else {
                                        XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                                        line = Line.CreateBound(XYZ.Zero, v2);
                                    }
                                    // 平行かどうか
                                    if (IsParallel(line.Direction, elementDirection)) {
                                        List<Solid> solidList = new List<Solid>();
                                        if (element1.GetType().Name == "FamilyInstance" || element1.GetType().Name == "FabricationPart") {
                                            solidList = GetInstanceSolid(element1);
                                        }
                                        else {
                                            solidList = GetSolid(element1);
                                        }
                                        if (solidList == null) {
                                            break;
                                        }
                                        Dictionary<XYZ, int> cornerDic = GetCorners(solidList);
                                        List<XYZ> xyzList = new List<XYZ>();
                                        foreach (KeyValuePair<XYZ, int> kv in cornerDic) {
                                            XYZ n = new XYZ(kv.Key.X, kv.Key.Y, 0);
                                            xyzList.Add(n);
                                        }
                                        List<XYZ> cornerList = GetCoordinateXY(xyzList, kvp.Value);
                                        if (cornerList.Count == 0) {
                                            break;
                                        }
                                        XYZ p1 = cornerList[0];
                                        XYZ p2 = cornerList[0];
                                        foreach (XYZ xyz in cornerList) {
                                            if (p1 == xyz) {
                                                continue;
                                            }
                                            Line l = Line.CreateBound(p1, xyz);
                                            // 平行かどうか
                                            if (IsParallel(elementDirection, l.Direction)) {
                                                p2 = xyz;
                                            }
                                        }
                                        cornerList.Remove(p1);
                                        cornerList.Remove(p2);
                                        XYZ p3 = cornerList.First();
                                        XYZ p4 = cornerList.Last();
                                        using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                            t.Start();
                                            FilteredElementCollector detailLineCollection =
                                            new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                            .OfCategory(BuiltInCategory.OST_Lines);
                                            List<CurveElement> lineList = new List<CurveElement>();
                                            int co = detailLineCollection.Count();
                                            foreach (CurveElement dc in detailLineCollection) {
                                                if (dc.GetType().Name != "DetailLine") {
                                                    continue;
                                                }
                                                if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                    lineList.Add(dc);
                                                }
                                            }
                                            int count = lineList.Count();
                                            Line l1 = Line.CreateBound(p1, p2);
                                            foreach (CurveElement dc in lineList) {
                                                if (dc.GetType().Name != "DetailLine") {
                                                    continue;
                                                }
                                                DetailLine detailLine = (DetailLine)dc;
                                                Line dcLine = detailLine.GeometryCurve as Line;
                                                if (dcLine == null) {
                                                    continue;
                                                }
                                                if (dcLine.Origin.X == l1.Origin.X && dcLine.Origin.Y == l1.Origin.Y) {
                                                    dl1 = (DetailLine)dc;
                                                    break;
                                                }
                                            }
                                            if (dl1 == null) {
                                                createLineList.Add(l1);
                                                dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                            }
                                            Line l2 = Line.CreateBound(p3, p4);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                            foreach (CurveElement dc in lineList) {
                                                if (dc.GetType().Name != "DetailLine") {
                                                    continue;
                                                }
                                                LocationCurve lc = dc.Location as LocationCurve;
                                                Line dcLine = lc.Curve as Line;
                                                if (dcLine.Origin.X == l2.Origin.X && dcLine.Origin.Y == l2.Origin.Y) {
                                                    dl2 = (DetailLine)dc;
                                                    break;
                                                }
                                            }
                                            if (dl2 == null) {
                                                dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                            }
                                            t.Commit();
                                            ref1 = dl1.GeometryCurve.Reference;
                                            ref2 = dl2.GeometryCurve.Reference;
                                            List<DetailCurve> dlList = new List<DetailCurve>();
                                            dlList.Add(dl1);
                                            dlList.Add(dl2);
                                            XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                            XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                            double distance1 = GetDistance(midPoint1, c1);
                                            double distance2 = GetDistance(midPoint1, c2);
                                            if (distance1 > distance2) {
                                                mDl1 = dl2;
                                            }
                                            else {
                                                mDl1 = dl1;
                                            }
                                        }
                                    }
                                }
                                // ロケーションカーブがnullで、詳細線分作成にチェックを入れた場合
                                else if (lineCheck.Checked && element1.GetType().Name != "Grid") {
                                    List<XYZ> cornerList = new List<XYZ>();
                                    Line line = Line.CreateBound(XYZ.Zero, elementDirection);
                                    xyzList = GetPoint(element1, line);
                                    xyzList.Sort((a, b) => Math.Sign(a.X - b.X));
                                    double minX = xyzList.First().X;
                                    double maxX = xyzList.Last().X;
                                    xyzList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                                    double minY = xyzList.First().Y;
                                    double maxY = xyzList.Last().Y;
                                    XYZ po1 = new XYZ(minX, minY, 0);
                                    XYZ po2 = new XYZ(maxX, minY, 0);
                                    XYZ po3 = new XYZ(minX, maxY, 0);
                                    XYZ po4 = new XYZ(maxX, maxY, 0);
                                    cornerList.Add(po1);
                                    cornerList.Add(po2);
                                    cornerList.Add(po3);
                                    cornerList.Add(po4);
                                    if (cornerList.Count == 0) {
                                        break;
                                    }
                                    double z = cornerList[0].Z;
                                    List<XYZ> cornerList2 = new List<XYZ>();
                                    foreach (XYZ xyz in cornerList) {
                                        if (xyz.Z == z) {
                                            cornerList2.Add(xyz);
                                        }
                                    }
                                    cornerList = cornerList2;
                                    XYZ p1 = cornerList[0];
                                    XYZ p2 = cornerList[0];
                                    foreach (XYZ xyz in cornerList) {
                                        if (p1 == xyz) {
                                            continue;
                                        }
                                        Line l = Line.CreateBound(p1, xyz);
                                        // 平行かどうか
                                        if (IsParallel(elementDirection, l.Direction)) {
                                            p2 = xyz;
                                        }
                                    }
                                    cornerList.Remove(p1);
                                    cornerList.Remove(p2);
                                    XYZ p3 = cornerList.First();
                                    XYZ p4 = cornerList.Last();
                                    using (Transaction t = new Transaction(Doc, ResText.IDS_TRN_DETAIL_LINE)) {
                                        t.Start();
                                        FilteredElementCollector detailLineCollection =
                                        new FilteredElementCollector(Doc).OfClass(typeof(CurveElement))
                                        .OfCategory(BuiltInCategory.OST_Lines);
                                        List<CurveElement> lineList = new List<CurveElement>();
                                        int co = detailLineCollection.Count();
                                        foreach (CurveElement dc in detailLineCollection) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            if (dc.OwnerViewId == Doc.ActiveView.Id) {
                                                lineList.Add(dc);
                                            }
                                        }
                                        int count = lineList.Count();
                                        Line l1 = Line.CreateBound(p1, p2);
                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            DetailLine detailLine = (DetailLine)dc;
                                            Line dcLine = detailLine.GeometryCurve as Line;
                                            if (dcLine == null) {
                                                continue;
                                            }
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && dcLine.Direction == l1.Direction) {
                                                dl1 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        // 重複するかどうかの判定用フラグ
                                        bool breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l1.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && dcLine.Direction == l1.Direction) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl1 == null && breakFlag) {
                                            createLineList.Add(l1);
                                            dl1 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l1);
                                        }
                                        Line l2 = Line.CreateBound(p3, p4);

                                        foreach (CurveElement dc in lineList) {
                                            if (dc.GetType().Name != "DetailLine") {
                                                continue;
                                            }
                                            LocationCurve lc = dc.Location as LocationCurve;
                                            Line dcLine = lc.Curve as Line;
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && dcLine.Direction == l2.Direction) {
                                                dl2 = (DetailLine)dc;
                                                break;
                                            }
                                        }
                                        breakFlag = true;
                                        foreach (Line dcLine in createLineList) {
                                            if (Math.Round(dcLine.Origin.X, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.X, 4, MidpointRounding.AwayFromZero)
                                                && Math.Round(dcLine.Origin.Y, 4, MidpointRounding.AwayFromZero) == Math.Round(l2.Origin.Y, 4, MidpointRounding.AwayFromZero)
                                                && dcLine.Direction == l2.Direction) {
                                                breakFlag = false;
                                                break;
                                            }
                                        }
                                        if (dl2 == null && breakFlag) {
                                            createLineList.Add(l2);
                                            dl2 = Doc.Create.NewDetailCurve(UiDoc.ActiveView, l2);
                                        }
                                        t.Commit();
                                        ref1 = dl1.GeometryCurve.Reference;
                                        ref2 = dl2.GeometryCurve.Reference;
                                        List<DetailCurve> dlList = new List<DetailCurve>();
                                        dlList.Add(dl1);
                                        dlList.Add(dl2);
                                        XYZ c1 = new XYZ(((l1.GetEndPoint(0).X + l1.GetEndPoint(1).X) / 2), ((l1.GetEndPoint(0).Y + l1.GetEndPoint(1).Y) / 2), ((l1.GetEndPoint(0).Z + l1.GetEndPoint(1).Z) / 2));
                                        XYZ c2 = new XYZ(((l2.GetEndPoint(0).X + l2.GetEndPoint(1).X) / 2), ((l2.GetEndPoint(0).Y + l2.GetEndPoint(1).Y) / 2), ((l2.GetEndPoint(0).Z + l2.GetEndPoint(1).Z) / 2));
                                        double distance1 = GetDistance(midPoint1, c1);
                                        double distance2 = GetDistance(midPoint1, c2);
                                        if (distance1 > distance2) {
                                            mDl1 = dl2;
                                        }
                                        else {
                                            mDl1 = dl1;
                                        }
                                    }
                                }
                            }
                            if (ref1 != null) {
                                // 寸法作成処理
                                CreateDimensionElement(Doc.ActiveView,
                                point1, ref1,
                                point2, kvp.Key, element1, element1);
                            }
                            if (ref2 != null) {
                                // 寸法作成処理
                                CreateDimensionElement(Doc.ActiveView,
                                point1, ref2,
                                point2, kvp.Key, element1, element1);
                            }
                            if (ref1 == null && ref2 == null) {
                                e1 = element1;
                                if (e1 != null) {
                                    errorIdSet.Add(e1.Id.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    string exMes = ex.Message;
                    if (e1 != null) {
                        errorIdSet.Add(e1.Id.ToString());
                    }
                    if (e2 != null) {
                        errorIdSet.Add(e2.Id.ToString());
                    }
                }
            }
        }


        /// <summary>
        /// インスタンス面リスト取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="normal">正規化ベクトル</param>
        /// <param name="opt">オプション</param>
        /// <returns>フェイスリスト</returns>
        private List<Face> GetInstanceClosestFaceList(
            Element e,
            XYZ normal,
            Options opt)
        {
            List<int> ids = new List<int>();
            List<Face> face = new List<Face>();
            GeometryElement geomElem = e.get_Geometry(opt);
            foreach (GeometryObject geomObj in geomElem)
            {
                //Solidであるか判定
                if (geomObj is Solid solid){
                    if (solid != null && 0 < solid.Volume){
                        FaceArray facearr = solid.Faces;
                        foreach (Face f in facearr){
                            if (f.GetType().Name != "PlanarFace" || f.Reference == null){
                                continue;
                            }
                            PlanarFace pf = f as PlanarFace;
                            // 平行かどうか
                            if (null != pf && IsParallel(normal, pf.FaceNormal)) {
                                face.Add(f);
                            }
                        }
                    }
                }
                //GeometryInstanceであるか判定
                else if (geomObj is GeometryInstance gi) {
                    foreach (GeometryObject go in gi.GetInstanceGeometry()){
                        Solid sol = go as Solid;
                        if (sol != null && 0 < sol.Volume){
                            FaceArray fa = sol.Faces;
                            foreach (Face f in fa){
                                if (f.GetType().Name != "PlanarFace" || f.Reference == null){
                                    continue;
                                }
                                PlanarFace pf = f as PlanarFace;
                                // 平行かどうか
                                if (null != pf && IsParallel(normal, pf.FaceNormal)){
                                    face.Add(f);
                                }
                            }
                        }
                    }
                }
            }
            XYZ sortDirection = normal;
            // ソート
            face = FaceSortPointByDirection(sortDirection, face);

            return face;
        }

        /// <summary>
        /// インスタンス全面リスト取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="opt">オプション</param>
        /// <returns>フェイスリスト</returns>
        private static List<Face> GetInstanceAllFaceList(
            Element e,
            Options opt)
        {
            List<Face> face = new List<Face>();
            List<int> intList = new List<int>();
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                if (obj.GetType().Name == "GeometryInstance") {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        Solid solid = obj2 as Solid;
                        if (solid != null) {
                            FaceArray fa = solid.Faces;
                            foreach (Face f in fa) {
                                if (f.GetType().Name != "PlanarFace" || f.Reference == null) {
                                    continue;
                                }
                                PlanarFace pf = f as PlanarFace;
                                if (null != pf) {
                                    face.Add(f);
                                }
                            }
                        }
                    }
                }
                else if (obj.GetType().Name == "Solid") {
                    Solid solid = obj as Solid;
                    if (solid != null) {
                        FaceArray fa = solid.Faces;
                        foreach (Face f in fa) {
                            if (f.GetType().Name != "PlanarFace" || f.Reference == null) {
                                continue;
                            }
                            PlanarFace pf = f as PlanarFace;
                            if (null != pf) {
                                face.Add(f);
                            }
                        }
                    }
                }
            }
            
            return face;
        }


        /// <summary>
        /// シンボル面リスト取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="ids">IDリスト</param>
        /// <param name="opt">オプション</param>
        /// <returns>フェイスリスト</returns>
        private static List<Face> GetSymbolClosestFaceList(
            Element e,
            List<int> ids,
            Options opt)
        {
            List<Face> face = new List<Face>();
            foreach (int i in ids) {
                GeometryElement geomElem = e.get_Geometry(opt);
                foreach (GeometryObject geomObj in geomElem){
                    //Solidであるか判定
                    if (geomObj is Solid solid){
                        if (solid != null && 0 < solid.Volume){
                            FaceArray facearr = solid.Faces;
                            foreach (Face f in facearr){
                                if (f.GetType().Name != "PlanarFace" || f.Reference == null){
                                    continue;
                                }
                                if (i == f.Id) {
                                    face.Add(f);
                                }
                            }
                        }
                    }
                    //GeometryInstanceであるか判定
                    else if (geomObj is GeometryInstance gi){
                        foreach (GeometryObject go in gi.GetSymbolGeometry()){
                            Solid sol = go as Solid;
                            if (sol != null && 0 < sol.Volume){
                                FaceArray fa = sol.Faces;
                                foreach (Face f in fa){
                                    if (f.GetType().Name != "PlanarFace" || f.Reference == null){
                                        continue;
                                    }
                                    if (i == f.Id){
                                        face.Add(f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return face;
        }

        /// <summary>
        /// 面リスト取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="normal">正規化ベクトル</param>
        /// <param name="opt">オプション</param>
        /// <returns>フェイスリスト</returns>
        private static List<Face> GetClosestFaceList(
            Element e,
            XYZ normal,
            Options opt)
        {
            List<Face> face = new List<Face>();
            GeometryElement geo = e.get_Geometry(opt);
            if (geo != null) {
                foreach (GeometryObject obj in geo) {
                    //Solidであるか判定
                    if (obj is Solid solid){
                        if (solid != null){
                            FaceArray fa = solid.Faces;
                            foreach (Face f in fa){
                                if (f.GetType().Name != "PlanarFace" || f.Reference == null){
                                    continue;
                                }
                                PlanarFace pf = f as PlanarFace;
                                // 平行かどうか
                                if (null != pf && IsParallel(normal, pf.FaceNormal)){
                                    face.Add(pf);
                                }
                            }
                        }
                    }
                    //GeometryInstanceであるか判定
                    else if (obj is GeometryInstance gi){
                        foreach (GeometryObject go in gi.GetInstanceGeometry()){
                            Solid sol = go as Solid;
                            if (sol != null){
                                FaceArray facearr = sol.Faces;
                                foreach (Face fac in facearr){
                                    if (fac.GetType().Name != "PlanarFace" || fac.Reference == null){
                                        continue;
                                    }
                                    PlanarFace pf = fac as PlanarFace;
                                    // 平行かどうか
                                    if (null != pf && IsParallel(normal, pf.FaceNormal)){
                                        face.Add(pf);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return face;
        }

        /// <summary>
        /// 距離取得
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns>距離の値</returns>
        private Double GetDistance(
            XYZ p1,
            XYZ p2)
        {
            double distance = Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2)));
            return distance;
        }

        /// <summary>
        /// 中心点取得
        /// </summary>
        /// <param name="element">要素</param>
        /// <param name="activeView">アクティブビュー</param>
        /// <returns>中心点<returns>
        private XYZ GetCenter(
            Element element, View activeView)
        {
            XYZ point = new XYZ(0, 0, 0);
            BoundingBoxXYZ box1 = element.get_BoundingBox(activeView);
            point = new XYZ((box1.Max.X + box1.Min.X) / 2, (box1.Max.Y + box1.Min.Y) / 2, (box1.Max.Z + box1.Min.Z) / 2);
            return point;
        }

        /// <summary>
        /// 中心点取得
        /// </summary>
        /// <param name="edge">エッジ</param>
        /// <returns>中心点<returns>
        private XYZ GetEdgeCenter(
            Edge edge)
        {
            XYZ point = new XYZ(0, 0, 0);
            Line line = edge.AsCurve() as Line;
            XYZ endPoint1 = line.GetEndPoint(0);
            XYZ endPoint2 = line.GetEndPoint(1);
            point = new XYZ((endPoint1.X + endPoint2.X) / 2, (endPoint1.Y + endPoint2.Y) / 2, (endPoint1.Z + endPoint2.Z) / 2);
            return point;
        }

        /// <summary>
        /// ソリッド取得
        /// </summary>
        /// <param name="element">要素</param>
        /// <returns>ソリッドリスト<returns>
        private List<Solid> GetSolid(
            Element element)
        {
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geo = element.get_Geometry(opt);
            List<Solid> solidList = new List<Solid>();
            foreach (GeometryObject obj in geo) {
                Solid solid = obj as Solid;
                solidList.Add(solid);
            }
            return solidList;
        }

        /// <summary>
        /// インスタンスソリッド取得
        /// </summary>
        /// <param name="element">要素</param>
        /// <returns>ソリッドリスト<returns>
        private List<Solid> GetInstanceSolid(
            Element element)
        {
            Options opt = new Options();
            opt.View = Doc.ActiveView;
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = false;
            List<Solid> solidList = new List<Solid>();
            GeometryElement geo = element.get_Geometry(opt);

            foreach (GeometryObject obj in geo) {
                if (obj.GetType().Name == "GeometryInstance") {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        Solid solid = obj2 as Solid;
                        if (solid != null) {
                            solidList.Add(solid);
                        }
                    }
                }
                else if (obj.GetType().Name == "Solid") {
                    Solid solid = obj as Solid;
                    solidList.Add(solid);
                }
            }
            return solidList;
        }

        /// <summary>
        /// ラインからのポイント取得
        /// </summary>
        /// <param name="element">要素</param>
        /// <param name="li">ライン</param>
        /// <returns>XYZリスト<returns>
        private List<XYZ> GetPoint(
            Element element, Line li)
        {
            Options opt = new Options();
            opt.View = Doc.ActiveView;
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = false;
            List<XYZ> xyzList = new List<XYZ>();
            GeometryElement geo = element.get_Geometry(opt);

            foreach (GeometryObject obj in geo) {
                if (obj.GetType().Name == "Line") {
                    Line line = obj as Line;
                    // 平行かどうか
                    if (IsParallel(li.Direction, line.Direction) && line.Id != -1) {
                        xyzList.Add(line.GetEndPoint(0));
                        xyzList.Add(line.GetEndPoint(1));
                    }
                }
                if (obj.GetType().Name == "GeometryInstance") {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        if (obj2.GetType().Name == "Line") {
                            Line line = obj2 as Line;
                            // 平行かどうか
                            if (IsParallel(li.Direction, line.Direction) && line.Id != -1) {
                                xyzList.Add(line.GetEndPoint(0));
                                xyzList.Add(line.GetEndPoint(1));
                            }
                        }
                    }
                }
            }
            //サブコンポーネント
            if (element.GetType().Name == "FamilyInstance") {
                FamilyInstance fi = (FamilyInstance)element;
                if (fi.GetSubComponentIds().Count > 0) {
                    foreach (ElementId eId in fi.GetSubComponentIds()) {
                        Element ele = Doc.GetElement(eId);
                        GeometryElement geo4 = ele.get_Geometry(opt);
                        foreach (GeometryObject obj in geo4) {
                            //Solidであるか判定
                            if (obj is Solid solid){
                                EdgeArray ea = solid.Edges;
                                foreach (Edge edge in ea){
                                    Curve curve = edge.AsCurve();
                                    if (curve.GetType().Name != "Line"){
                                        continue;
                                    }
                                    Line line = obj as Line;
                                    // 平行、リファレンスがnullでない場合
                                    if (IsParallel(line.Direction, line.Direction) && line.Id != -1){
                                        xyzList.Add(line.GetEndPoint(0));
                                        xyzList.Add(line.GetEndPoint(1));
                                    }
                                }
                            }
                            //GeometryInstanceであるか判定
                            else if (obj is GeometryInstance gi){
                                foreach (GeometryObject go in gi.GetInstanceGeometry()){
                                    if (go.GetType().Name == "Line"){
                                        Line line = go as Line;
                                        // 平行、リファレンスがnullでない場合
                                        if (IsParallel(line.Direction, line.Direction) && line.Id != -1){
                                            xyzList.Add(line.GetEndPoint(0));
                                            xyzList.Add(line.GetEndPoint(1));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return xyzList;
        }

        /// <summary>
        /// ライン取得
        /// </summary>
        /// <param name="element">要素</param>
        /// <returns>ラインリスト<returns>
        private List<Line> GetLine(
            Element element)
        {
            Options opt = new Options();
            opt.View = Doc.ActiveView;
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = false;
            List<Line> lineList = new List<Line>();
            GeometryElement geo = element.get_Geometry(opt);

            foreach (GeometryObject obj in geo) {
                if (obj.GetType().Name == "Line") {
                    Line line = obj as Line;
                    lineList.Add(line);
                }
                if (obj.GetType().Name == "GeometryInstance") {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        if (obj2.GetType().Name == "Line") {
                            Line line = obj2 as Line;
                            lineList.Add(line);
                        }
                    }
                }
            }
            //サブコンポーネント
            if (element.GetType().Name == "FamilyInstance") {
                FamilyInstance fi = (FamilyInstance)element;
                if (fi.GetSubComponentIds().Count > 0) {
                    foreach (ElementId eId in fi.GetSubComponentIds()) {
                        Element ele = Doc.GetElement(eId);
                        GeometryElement geo4 = ele.get_Geometry(opt);
                        foreach (GeometryObject obj in geo4) {
                            //Solidであるか判定
                            if (obj is Solid solid){
                                EdgeArray ea = solid.Edges;
                                foreach (Edge edge in ea){
                                    Curve curve = edge.AsCurve();
                                    if (curve.GetType().Name != "Line"){
                                        Line line = obj as Line;
                                        lineList.Add(line);
                                    }
                                }
                            }
                            //GeometryInstanceであるか判定
                            else if (obj is GeometryInstance gi){
                                foreach (GeometryObject go in gi.GetInstanceGeometry()){
                                    if (go.GetType().Name == "Line"){
                                        Line line = go as Line;
                                        lineList.Add(line);
                                    }
                                }
                            }                 
                        }
                    }
                }
            }

            return lineList;
        }

        /// <summary>
        /// 座標取得(XY平面)
        /// </summary>
        /// <param name="corner">コーナーリスト</param>
        /// <param name="direction">方向</param>
        /// <returns>XYZリスト</returns>
        private List<XYZ> GetCoordinateXY(
            List<XYZ> corner, XYZ direction)
        {
            double slope = direction.Y / direction.X;
            HashSet<XYZ> cordinateSet = new HashSet<XYZ>();
            List<XYZ> cordinateList = new List<XYZ>();
            List<XYZ> cornerList = corner;
            cornerList.Sort((a, b) => Math.Sign(a.X - b.X));
            XYZ maxCordinate = cornerList.Last();
            foreach (XYZ xyz1 in cornerList) {
                foreach (XYZ xyz2 in cornerList) {
                    if (xyz1 == xyz2) {
                        continue;
                    }
                    Line line = Line.CreateBound(xyz1, xyz2);
                    // 平行かどうか
                    if (IsParallel(line.Direction, direction)) {
                        cordinateSet.Add(xyz1);
                    }
                }
            }
            foreach (XYZ xyz in cordinateSet) {
                cordinateList.Add(xyz);
            }

            return cordinateList;
        }

        /// <summary>
        /// エッジ取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="eleDirection">方向</param>
        /// <param name="opt">オプション</param>
        /// <param name="point">点</param>
        /// <returns>エッジリスト</returns>
        private List<Edge> GetEdgeListXY(
        Element e,
        XYZ eleDirection,
        Options opt,
            XYZ point)
        {
            List<XYZ> endPointList = new List<XYZ>();
            List<Edge> edgeList = new List<Edge>();
            GeometryElement geo = e.get_Geometry(opt);
            if (geo != null) {
                foreach (GeometryObject obj in geo) {
                    if (obj is Solid solid){
                        if (solid != null){
                            EdgeArray ea = solid.Edges;
                            foreach (Edge edge in ea){
                                Curve curve = edge.AsCurve();
                                if (curve.GetType().Name != "Line"){
                                    continue;
                                }
                                Line line = (Line)curve;
                                // 平行かどうか
                                if (IsParallel(line.Direction, eleDirection)){
                                    edgeList.Add(edge);
                                    endPointList.Add(edge.AsCurve().GetEndPoint(0));
                                }
                            }
                        }
                    }
                    //GeometryInstanceであるか判定
                    else if (obj is GeometryInstance gi){
                        foreach (GeometryObject go in gi.GetInstanceGeometry()){
                            Solid sol = go as Solid;
                            if (sol != null){
                                EdgeArray eda = sol.Edges;
                                foreach (Edge edge in eda){
                                    Curve cv = edge.AsCurve();
                                    if (cv.GetType().Name != "Line"){
                                        continue;
                                    }
                                    Line line = (Line)cv;
                                    // 平行かどうか
                                    if (IsParallel(line.Direction, eleDirection)){
                                        edgeList.Add(edge);
                                        endPointList.Add(edge.AsCurve().GetEndPoint(0));
                                    }
                                }
                            }
                        }
                    }
                }
                XYZ sortDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, eleDirection), Doc.ActiveView.ViewDirection);
                // ソート
                edgeList = EdgeSortPointByDirection(sortDirection, edgeList);
            }
            return edgeList;
        }

        /// <summary>
        /// エッジ取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="eleDirection">方向</param>
        /// <param name="opt">オプション</param>
        /// <returns>エッジリスト</returns>
        private List<Edge> GetInstanceEdgeListXY(
        Element e,
        XYZ eleDirection,
        Options opt)
        {
            List<Edge> edgeList = new List<Edge>();
            List<double> xList = new List<double>();
            List<int> ids = new List<int>();
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                //Solidであるか判定
                if (obj is Solid solid){
                    if (solid != null){
                        EdgeArray ea = solid.Edges;
                        foreach (Edge edge in ea) {
                            Curve curve = edge.AsCurve();
                            if (curve.GetType().Name != "Line"){
                                continue;
                            }
                            Line line = (Line)curve;
                            // 平行かどうか
                            if (IsParallel(line.Direction, eleDirection) && edge.Reference != null){
                                edgeList.Add(edge);
                            }
                        }
                    }                     
                }
                //GeometryInstanceであるか判定
                else if (obj is GeometryInstance gi){
                    foreach (GeometryObject go in gi.GetInstanceGeometry()){
                        Solid sol = go as Solid;
                        if (sol != null){
                            EdgeArray ea = sol.Edges;
                            foreach (Edge edge in ea){
                                Curve curve = edge.AsCurve();
                                if (curve.GetType().Name != "Line") {
                                    continue;
                                }
                                Line line = (Line)curve;
                                // 平行かどうか
                                if (IsParallel(line.Direction, eleDirection) && edge.Reference != null) {
                                    edgeList.Add(edge);
                                }
                            }
                        }
                    }
                }
            }
            XYZ sortDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, eleDirection), Doc.ActiveView.ViewDirection);
            // ソート
            edgeList = EdgeSortPointByDirection(sortDirection, edgeList);
            foreach (Edge edge in edgeList) {
                ids.Add(edge.Id);
            }
            edgeList.Clear();

            foreach (int i in ids) {
                foreach (GeometryObject obj in geo) {
                    if (obj.GetType().Name == "GeometryInstance") {
                        GeometryInstance gi = obj as GeometryInstance;
                        GeometryElement geo2 = gi.GetSymbolGeometry();
                        foreach (GeometryObject obj2 in geo2) {
                            Solid solid = obj2 as Solid;
                            if (solid != null) {
                                EdgeArray ea = solid.Edges;
                                foreach (Edge edge in ea) {
                                    Curve curve = edge.AsCurve();
                                    if (curve.GetType().Name != "Line") {
                                        continue;
                                    }
                                    Line line = (Line)curve;
                                    // ID一致、平行、リファレンスがnullでない場合
                                    if (i == edge.Id && IsParallel(line.Direction, eleDirection) && edge.Reference != null) {
                                        edgeList.Add(edge);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return edgeList;
        }

        /// <summary>
        /// エッジ取得XZ
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="eleDirection">方向</param>
        /// <param name="opt">オプション</param>
        /// <returns>エッジリスト</returns>
        private List<Edge> GetInstanceEdgeListXZ(
        Element e,
        XYZ eleDirection,
        Options opt)
        {
            List<Edge> edgeList = new List<Edge>();
            List<double> xList = new List<double>();
            List<int> ids = new List<int>();
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                //Solidであるか判定
                if (obj is Solid solid){
                    if (solid != null){
                        EdgeArray ea = solid.Edges;
                        foreach (Edge edge in ea){
                            Curve curve = edge.AsCurve();
                            if (curve.GetType().Name != "Line"){
                                continue;
                            }
                            Line line = (Line)curve;
                            // 平行、リファレンスがnullでない場合
                            if (IsParallel(line.Direction, eleDirection) && edge.Reference != null){
                                edgeList.Add(edge);
                            }
                        }
                    }
                }
                //GeometryInstanceであるか判定
                else if (obj is GeometryInstance gi){
                    Solid sol = obj as Solid;
                    if (sol != null){
                        EdgeArray ea = sol.Edges;
                        foreach (Edge edge in ea){
                            Curve curve = edge.AsCurve();
                            if (curve.GetType().Name != "Line") {
                                continue;
                            }
                            Line line = (Line)curve;
                            // 平行、リファレンスがnullでない場合
                            if (IsParallel(line.Direction, eleDirection) && edge.Reference != null) {
                                edgeList.Add(edge);
                            }
                        }
                    }
                }
            }
            List<XYZ> endPointList = new List<XYZ>();
            foreach (Edge edge in edgeList) {
                endPointList.Add(GetEdgeCenter(edge));
            }
            XYZ sortDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, eleDirection), Doc.ActiveView.ViewDirection);
            // ソート
            edgeList = EdgeSortPointByDirection(sortDirection, edgeList);
            endPointList = new List<XYZ>();
            foreach (Edge edge in edgeList) {
                endPointList.Add(GetEdgeCenter(edge));
            }
            foreach (Edge edge in edgeList) {
                ids.Add(edge.Id);
            }
            edgeList = new List<Edge>();
            foreach (int i in ids) {
                foreach (GeometryObject obj in geo) {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetSymbolGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        Solid solid = obj2 as Solid;
                        if (solid != null) {
                            EdgeArray ea = solid.Edges;
                            foreach (Edge edge in ea) {
                                if (i == edge.Id) {
                                    edgeList.Add(edge);
                                }
                            }
                        }
                    }
                }
            }
            return edgeList;
        }

        /// <summary>
        /// エッジ取得XZ
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="eleDirection">方向</param>
        /// <param name="opt">オプション</param>
        /// <returns>エッジリスト</returns>
        private List<Edge> GetEdgeListXZ(
        Element e,
        XYZ eleDirection,
        Options opt)
        {
            List<XYZ> endPointList = new List<XYZ>();
            List<Edge> edgeList = new List<Edge>();
            GeometryElement geo = e.get_Geometry(opt);
            if (geo != null) {
                foreach (GeometryObject obj in geo) {
                    //Solidであるか判定
                    if (obj is Solid solid){
                        EdgeArray ea = solid.Edges;
                        foreach (Edge edge in ea) {
                            Curve curve = edge.AsCurve();
                            if (curve.GetType().Name != "Line") {
                                continue;
                            }
                            Line line = (Line)curve;
                            XYZ xyzX = line.GetEndPoint(0);
                            // 平行、リファレンスがnullでない場合
                            if (IsParallel(line.Direction, eleDirection) && edge.Reference != null) {
                                edgeList.Add(edge);
                            }
                        }
                    }
                    //GeometryInstanceであるか判定
                    else if (obj is GeometryInstance gi) {
                        foreach (GeometryObject go in gi.GetInstanceGeometry()){
                            Solid sol = go as Solid;
                            if (sol != null){
                                EdgeArray ea = sol.Edges;
                                foreach (Edge edge in ea){
                                    Curve curve = edge.AsCurve();
                                    if (curve.GetType().Name != "Line"){
                                        continue;
                                    }
                                    Line line = (Line)curve;
                                    XYZ xyzX = line.GetEndPoint(0);
                                    // 平行、リファレンスがnullでない場合
                                    if (IsParallel(line.Direction, eleDirection) && edge.Reference != null){
                                        edgeList.Add(edge);
                                    }
                                }
                            }       
                        }
                    }
                }
            }

            if (e.GetType().Name == "FamilyInstance") {
                FamilyInstance fi = (FamilyInstance)e;
                // サブコンポーネント
                if (fi.GetSubComponentIds().Count > 0) {
                    foreach (ElementId eId in fi.GetSubComponentIds()) {
                        Element ele = Doc.GetElement(eId);
                        GeometryElement geo4 = e.get_Geometry(opt);
                        foreach (GeometryObject obj in geo4) {
                            //Solidであるか判定
                            if (obj is Solid solid){
                                EdgeArray ea = solid.Edges;
                                foreach (Edge edge in ea) {
                                    Curve curve = edge.AsCurve();
                                    if (curve.GetType().Name != "Line") {
                                        continue;
                                    }
                                    Line line = (Line)curve;
                                    XYZ xyzX = line.GetEndPoint(0);
                                    // 平行、リファレンスがnullでない場合
                                    if (IsParallel(line.Direction, eleDirection) && edge.Reference != null) {
                                        edgeList.Add(edge);
                                        endPointList.Add(edge.AsCurve().GetEndPoint(0));
                                    }
                                }
                            }
                            //GeometryInstanceであるか判定
                            else if (obj is GeometryInstance gi){
                                foreach (GeometryObject go in gi.GetInstanceGeometry()){
                                    Solid sol = go as Solid;
                                    EdgeArray ea = sol.Edges;
                                    foreach (Edge edge in ea){
                                        Curve curve = edge.AsCurve();
                                        if (curve.GetType().Name != "Line"){
                                            continue;
                                        }
                                        Line line = (Line)curve;
                                        XYZ xyzX = line.GetEndPoint(0);
                                        // 平行、リファレンスがnullでない場合
                                        if (IsParallel(line.Direction, eleDirection) && edge.Reference != null){
                                            edgeList.Add(edge);
                                            endPointList.Add(edge.AsCurve().GetEndPoint(0));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (Edge edge in edgeList) {
                endPointList.Add(GetEdgeCenter(edge));
            }
            XYZ sortDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, eleDirection), Doc.ActiveView.ViewDirection);
            // ソート
            edgeList = EdgeSortPointByDirection(sortDirection, edgeList);

            return edgeList;
        }

        /// <summary>
        /// インスタンスエッジ取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="opt">オプション</param>
        /// <returns>エッジ</returns>
        private static List<Edge> GetAllInstanceEdgeList(
         Element e,
        Options opt)
        {
            List<Edge> edgeList = new List<Edge>();
            List<double> xList = new List<double>();
            List<int> intList = new List<int>();
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                if (obj.GetType().Name == "GeometryInstance") {
                    GeometryInstance gi = obj as GeometryInstance;
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2) {
                        Solid solid = obj2 as Solid;
                        if (solid != null) {
                            EdgeArray ea = solid.Edges;
                            foreach (Edge edge in ea) {
                                Curve curve = edge.AsCurve();
                                if (curve.GetType().Name != "Line" || edge.Reference == null) {
                                    continue;
                                }
                                Line line = (Line)curve;
                                edgeList.Add(edge);
                            }
                        }
                    }
                }
                else if (obj.GetType().Name == "Solid") {
                    Solid solid = obj as Solid;
                    if (solid != null) {
                        EdgeArray ea = solid.Edges;
                        foreach (Edge edge in ea) {
                            Curve curve = edge.AsCurve();
                            if (curve.GetType().Name != "Line" || edge.Reference == null) {
                                continue;
                            }
                            Line line = (Line)curve;
                            edgeList.Add(edge);
                        }
                    }
                }
            }
            return edgeList;
        }

        /// <summary>
        /// インスタンス中心線取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="direction">方向</param>
        /// <returns>ID</returns>
        private static int GetInstanceLine(
         Element e, XYZ direction)
        {
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = true;
            int lineId = int.MinValue;
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                GeometryInstance gi = obj as GeometryInstance;
                GeometryElement geo2 = gi.GetInstanceGeometry();
                foreach (GeometryObject obj2 in geo2) {
                    Line line = obj2 as Line;
                    // ラインがnullでない、平行の場合
                    if (line != null && IsParallel(direction, line.Direction)) {
                        lineId = line.Id;
                    }
                }
            }

            return lineId;
        }

        /// <summary>
        /// インスタンス中心線取得
        /// </summary>
        /// <param name="e">要素</param>
        /// <param name="lineId">ID</param>
        /// <returns>ライン</returns>
        private static Line GetSymbolLine(Element e,
         int lineId)
        {
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = true;
            Line line = null;
            GeometryElement geo = e.get_Geometry(opt);
            foreach (GeometryObject obj in geo) {
                GeometryInstance gi = obj as GeometryInstance;
                GeometryElement geo2 = gi.GetSymbolGeometry();
                foreach (GeometryObject obj2 in geo2) {
                    Line line2 = obj2 as Line;
                    if (line2 != null) {
                        if (line2.Id == lineId) {
                            line = line2;
                        }
                    }
                }
            }

            return line;
        }

        /// <summary>
        /// 参照の方向取得
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="reference">リファレンス</param>
        /// <returns>方向</returns>
        private XYZ GetReferenceDirection(Document doc, XYZ viewRightDirection, Reference reference)
        {
            bool deleteFlag = true;
            XYZ res = XYZ.Zero;
            if (reference.ElementId == ElementId.InvalidElementId) {
                return res;
            }

            Element elem = doc.GetElement(reference.ElementId);
            if (elem == null) {
                return res;
            }
            if (reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE || reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_LINEAR) {
                XYZ bEnd = new XYZ(10, 10, 10);
                ReferenceArray refArr = new ReferenceArray();
                refArr.Append(reference);
                Dimension dim = null;
                using (Transaction t = new Transaction(doc, "Direction")) {
                    t.Start();
                    using (SubTransaction st = new SubTransaction(doc)) {
                        st.Start();

                        ReferencePlane refPlane = doc.Create.NewReferencePlane(XYZ.Zero, bEnd, bEnd.CrossProduct(XYZ.BasisZ).Normalize(), doc.ActiveView);
                        ModelCurve mc = doc.Create.NewModelCurve(Line.CreateBound(XYZ.Zero, new XYZ(10, 10, 10)), SketchPlane.Create(doc, refPlane.Id));
                        refArr.Append(mc.GeometryCurve.GetEndPointReference(0));
                        dim = doc.Create.NewDimension(doc.ActiveView, Line.CreateBound(XYZ.Zero, viewRightDirection * 10), refArr);
                        if (dim.ValueString.Contains("-")) {
                            doc.Delete(dim.Id);
                            deleteFlag = false;
                        }
                        else {
                            ElementTransformUtils.MoveElement(doc, dim.Id, new XYZ(0, 0.1, 0));
                        }
                        st.Commit();
                    }
                    if (dim != null && deleteFlag) {
                        Curve cv = dim.Curve;
                        cv.MakeBound(0, 1);
                        XYZ pt1 = cv.GetEndPoint(0);
                        XYZ pt2 = cv.GetEndPoint(1);
                        res = pt2.Subtract(pt1).Normalize();
                    }
                    t.RollBack();
                }
            }
            return res;
        }

        /// <summary>
        /// 平行かどうかの判定
        /// </summary>
        /// <param name="a">方向1</param>
        /// <param name="b">方向2</param>
        /// <returns>真偽</returns>
        static public bool IsParallel(XYZ a, XYZ b)
        {
            return a.IsAlmostEqualTo(b, App.AngleTolerance) || a.IsAlmostEqualTo(-b, App.AngleTolerance);
        }

        /// <summary>
        /// 等号判定
        /// </summary>
        /// <param name="a">数値1</param>
        /// <param name="b">数値2</param>
        /// <returns>真偽</returns>
        static public bool IsEqual(double a, double b)
        {
            return Math.Abs(a - b) < _eps;
        }

        /// <summary>
        /// 寸法作成処理
        /// </summary>
        /// <param name="view">ビュー</param>
        /// <param name="p1">点1</param>
        /// <param name="r1">リファレンス1</param>
        /// <param name="p2">点2</param>
        /// <param name="r2">リファレンス2</param>
        /// <param name="e1">要素1</param>
        /// <param name="e2">要素2</param>
        private void CreateDimensionElement(
         View view,
         XYZ p1,
         Reference r1,
         XYZ p2,
         Reference r2,
         Element e1,
         Element e2)
        {
            try {
                ReferenceArray ra = new ReferenceArray();

                ra.Append(r1);
                ra.Append(r2);

                Line line = Line.CreateBound(p1, p2);

                using (Transaction transaction = new Transaction(Doc, "寸法作成")) {
                    transaction.Start();
                    Dimension dim = Doc.Create.NewDimension(
                    Doc.ActiveView, line, ra);
                    if (dim.ValueString.Contains("-")) {
                        if (e1 != null) {
                            errorIdSet.Add(e1.Id.ToString());
                        }
                        if (e2 != null) {
                            errorIdSet.Add(e2.Id.ToString());
                        }
                        Doc.Delete(dim.Id);
                    }
                    else {
                        if (e1 != null) {
                            successIdSet.Add(e1.Id.ToString());
                        }
                        if (e2 != null) {
                            successIdSet.Add(e2.Id.ToString());
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex) {
                string err = ex.Message;
                if (e1 != null) {
                    errorIdSet.Add(e1.Id.ToString());
                }
                if (e2 != null) {
                    errorIdSet.Add(e2.Id.ToString());
                }
            }
        }

        /// <summary>
        /// 中点取得
        /// </summary>
        /// <param name="line">ライン</param>
        /// <returns>中点</returns>
        private XYZ Midpoint(Line line)
        {
            return Midpoint(line.GetEndPoint(0),
              line.GetEndPoint(1));
        }

        /// <summary>
        /// 中点取得
        /// </summary>
        /// <param name="p">点1</param>
        /// <param name="q">点2</param>
        /// <returns>中点</returns>
        private XYZ Midpoint(XYZ p, XYZ q)
        {
            return p + 0.5 * (q - p);
        }

        /// <summary>
        /// Z軸との外積を求める
        /// </summary>
        /// <param name="line">ライン</param>
        /// <returns>ベクトル</returns>
        private XYZ Normal(Line line)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;

            return v.CrossProduct(XYZ.BasisZ).Normalize();
        }

        /// <summary>
        /// ビュー方向との外積を求める
        /// </summary>
        /// <param name="line">ライン</param>
        /// <param name="viewDirection">ビューの方向</param>
        /// <returns>ベクトル</returns>
        private XYZ NormalViewDirection(Line line, XYZ viewDirection)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;

            return v.CrossProduct(viewDirection).Normalize();
        }

        /// <summary>
        /// Y軸との外積を求める
        /// </summary>
        /// <param name="line">ライン</param>
        /// <returns>ベクトル</returns>
        private XYZ NormalXZ(Line line)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;

            return v.CrossProduct(XYZ.BasisY).Normalize();
        }

        /// <summary>
        /// コーナー取得
        /// </summary>
        /// <param name="corners">コーナーディクショナリー</param>
        /// <param name="solid">ソリッド</param>
        private static void GetCorners(
          Dictionary<XYZ, int> corners,
          Solid solid)
        {
            foreach (Face f in solid.Faces) {
                foreach (EdgeArray ea in f.EdgeLoops) {
                    foreach (Edge e in ea) {
                        XYZ p = e.AsCurveFollowingFace(f)
                          .GetEndPoint(0);

                        if (!corners.ContainsKey(p)) {
                            corners[p] = 0;
                        }
                        ++corners[p];
                    }
                }
            }
        }

        /// <summary>
        /// コーナー取得
        /// </summary>
        /// <param name="solids">ソリッドリスト</param>
        /// <returns>ディクショナリー</returns>
        public static Dictionary<XYZ, int> GetCorners(
          List<Solid> solids)
        {
            Dictionary<XYZ, int> corners
              = new Dictionary<XYZ, int>(
                new XyzEqualityComparer());

            foreach (Solid solid in solids) {
                GetCorners(corners, solid);
            }
            return corners;
        }

        /// <summary>
        /// 等式定義クラス
        /// </summary>
        private class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private const double _sixteenthInchInFeet
              = 1.0 / (16.0 * 12.0);

            public bool Equals(XYZ p, XYZ q)
            {
                return p.IsAlmostEqualTo(q,
                  _sixteenthInchInFeet);
            }

            public int GetHashCode(XYZ p)
            {
                return PointString(p).GetHashCode();
            }
        }

        /// <summary>
        /// XYZからstringを返す
        /// </summary>
        /// <param name="p">点</param>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        /// <summary>
        /// 数値からstringを返す
        /// </summary>
        /// <param name="a">数値</param>
        private static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// 投影
        /// </summary>
        /// <param name="vector">ベクトル</param>
        /// <param name="axis">軸</param>
        /// <returns>ベクトル</returns>
        public static XYZ Project(XYZ vector, XYZ axis)
        {
            XYZ n = axis.Normalize();
            return vector.DotProduct(n) * n;
        }

        /// <summary>
        /// 点を指定方向順にソート(要素)
        /// </summary>
        /// <param name="sortDirection">ソート方向(正規化済)</param>
        /// <param name="edges">ソート対象のエッジリスト</param>
        /// <returns>エッジリスト</returns>
        private List<Edge> EdgeSortPointByDirection(XYZ sortDirection, List<Edge> edges)
        {
            if (Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) < 0 ||
                Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(sortDirection.AngleTo(XYZ.BasisY), 3, MidpointRounding.AwayFromZero) > Math.Round(Math.PI / 2, 3, MidpointRounding.AwayFromZero) ||
                sortDirection.IsAlmostEqualTo(-XYZ.BasisX))
            {
                sortDirection = sortDirection.Negate();
            }

            // 回転の必要性(ソート方向がX軸であれば回転の必要なし)
            bool needRotation = false;
            // 回転軸
            XYZ rotateAxis = XYZ.BasisZ;
            // X軸とソート方向の成す角度(回転角度)
            double angleRadianBetweenBasisXAndSortDir = 0;
            if (!sortDirection.IsAlmostEqualTo(XYZ.BasisX)) {
                needRotation = true;

                // 回転軸を求める(X軸とソート方向の外積方向)
                rotateAxis = XYZ.BasisX.CrossProduct(sortDirection).Normalize();
                if (rotateAxis.IsZeroLength())
                    rotateAxis = XYZ.BasisZ;

                // X軸とソート方向の成す角度(A)を求める
                angleRadianBetweenBasisXAndSortDir = XYZ.BasisX.AngleTo(sortDirection);
            }

            // 要素と、配置点を回転させた点のDictionaryを作成
            Dictionary<Edge, XYZ> convertPtDic = new Dictionary<Edge, XYZ>();
            foreach (Edge edge in edges) {
                // 要素配置点を取得
                XYZ locationPt = GetEdgeCenter(edge);

                // X軸とソート方向の外積を軸、ビュー原点を中心点として、対象点を角度(A)だけ回転させる
                if (needRotation) {
                    locationPt = GetRotationPoint(rotateAxis, locationPt, -angleRadianBetweenBasisXAndSortDir, XYZ.Zero);
                }

                // エッジと、配置点(必要があれば回転済)を、Dictionaryに追加
                convertPtDic.Add(edge, locationPt);
            }

            // Dictionaryを、回転させた点のX軸順にソートして、元のエッジをソート方向順にソートしたリストを作成
            List<Edge> sortedElems = new List<Edge>();
            foreach (KeyValuePair<Edge, XYZ> target in convertPtDic.OrderBy(pair => pair.Value.X)) {
                sortedElems.Add(target.Key);
            }

            return sortedElems;
        }

        /// <summary>
        /// 点を指定方向順にソート（要素）
        /// </summary>
        /// <param name="sortDirection">ソート方向(正規化済)</param>
        /// <param name="elems">ソート対象の要素リスト</param>
        /// <returns>要素リスト</returns>
        private List<Element> ElementSortPointByDirection(XYZ sortDirection, List<Element> eles)
        {
            if (Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) < 0 ||
                Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(sortDirection.AngleTo(XYZ.BasisY), 3, MidpointRounding.AwayFromZero) > Math.Round(Math.PI / 2, 3, MidpointRounding.AwayFromZero) ||
                sortDirection.IsAlmostEqualTo(-XYZ.BasisX))
            {
                sortDirection = sortDirection.Negate();
            }

            // 回転の必要性(ソート方向がX軸であれば回転の必要なし)
            bool needRotation = false;
            // 回転軸
            XYZ rotateAxis = XYZ.BasisZ;
            // X軸とソート方向の成す角度(回転角度)
            double angleRadianBetweenBasisXAndSortDir = 0;
            if (!sortDirection.IsAlmostEqualTo(XYZ.BasisX)) {
                needRotation = true;

                // 回転軸を求める(X軸とソート方向の外積方向)
                rotateAxis = XYZ.BasisX.CrossProduct(sortDirection).Normalize();
                if (rotateAxis.IsZeroLength())
                    rotateAxis = XYZ.BasisZ;

                // X軸とソート方向の成す角度(A)を求める
                angleRadianBetweenBasisXAndSortDir = XYZ.BasisX.AngleTo(sortDirection);
            }

            // 要素と、配置点を回転させた点のDictionaryを作成
            Dictionary<Element, XYZ> convertPtDic = new Dictionary<Element, XYZ>();
            foreach (Element ele in eles) {
                // 要素配置点を取得
                XYZ locationPt = new XYZ(0, 0, 0);
                if (ele.Location.GetType().Name == "LocationCurve") {
                    LocationCurve lc = ele.Location as LocationCurve;
                    if (lc.Curve.GetType().Name == "Line") {
                        Line line = lc.Curve as Line;
                        XYZ point1 = line.GetEndPoint(0);
                        XYZ point2 = line.GetEndPoint(1);
                        XYZ centerPoint = new XYZ((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                        locationPt = centerPoint;
                    }
                    else {
                        locationPt = GetCenter(ele, Doc.ActiveView);
                    }
                }
                else if (ele.Location.GetType().Name == "LocationPoint") {
                    LocationPoint lp = ele.Location as LocationPoint;
                    locationPt = lp.Point;
                }
                else {
                    locationPt = GetCenter(ele, Doc.ActiveView);
                }

                // X軸とソート方向の外積を軸、ビュー原点を中心点として、対象点を角度(A)だけ回転させる
                if (needRotation) {
                    locationPt = GetRotationPoint(rotateAxis, locationPt, -angleRadianBetweenBasisXAndSortDir, XYZ.Zero);
                }

                // 要素と、配置点(必要があれば回転済)を、Dictionaryに追加
                convertPtDic.Add(ele, locationPt);
            }

            // Dictionaryを、回転させた点のX軸順にソートして、元の要素をソート方向順にソートしたリストを作成
            List<Element> sortedElems = new List<Element>();
            foreach (KeyValuePair<Element, XYZ> target in convertPtDic.OrderBy(pair => pair.Value.X)) {
                sortedElems.Add(target.Key);
            }

            return sortedElems;
        }

        /// <summary>
        /// 点を指定方向順にソート（フェイス）
        /// </summary>
        /// <param name="sortDirection">ソート方向(正規化済)</param>
        /// <param name="faces">ソート対象のファイスリスト</param>
        /// <returns>ファイスリスト</returns>
        private List<Face> FaceSortPointByDirection(XYZ sortDirection, List<Face> faces)
        {
            if (Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) < 0 ||
                Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) == 0 && Math.Round(sortDirection.AngleTo(XYZ.BasisY), 3, MidpointRounding.AwayFromZero) > Math.Round(Math.PI / 2, 3, MidpointRounding.AwayFromZero) ||
                sortDirection.IsAlmostEqualTo(-XYZ.BasisX))
            {
                sortDirection = sortDirection.Negate();
            }

            // 回転の必要性(ソート方向がX軸であれば回転の必要なし)
            bool needRotation = false;
            // 回転軸
            XYZ rotateAxis = XYZ.BasisZ;
            // X軸とソート方向の成す角度(回転角度)
            double angleRadianBetweenBasisXAndSortDir = 0;
            if (!sortDirection.IsAlmostEqualTo(XYZ.BasisX)) {
                needRotation = true;

                // 回転軸を求める(X軸とソート方向の外積方向)
                rotateAxis = XYZ.BasisX.CrossProduct(sortDirection).Normalize();
                if (rotateAxis.IsZeroLength())
                    rotateAxis = XYZ.BasisZ;

                // X軸とソート方向の成す角度(A)を求める
                angleRadianBetweenBasisXAndSortDir = XYZ.BasisX.AngleTo(sortDirection);
            }

            // フェイスと、配置点を回転させた点のDictionaryを作成
            Dictionary<Face, XYZ> convertPtDic = new Dictionary<Face, XYZ>();
            foreach (Face face in faces) {
                // 要素配置点を取得
                PlanarFace pf = (PlanarFace)face;
                XYZ point = new XYZ(0, 0, 0);
                IList<CurveLoop> loop = face.GetEdgesAsCurveLoops();
                foreach (CurveLoop cl in loop) {
                    foreach (Curve curve in cl) {
                        if (curve.GetType().Name != "Line") {
                            continue;
                        }
                        Line line = (Line)curve;
                        XYZ vDirection = NormalViewDirection(line, Doc.ActiveView.ViewDirection);
                        // 平行かどうか
                        if (IsParallel(vDirection, pf.FaceNormal)) {
                            point = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                        }
                    }
                }
                XYZ locationPt = point;

                // X軸とソート方向の外積を軸、ビュー原点を中心点として、対象点を角度(A)だけ回転させる
                if (needRotation) {
                    locationPt = GetRotationPoint(rotateAxis, locationPt, -angleRadianBetweenBasisXAndSortDir, XYZ.Zero);
                }

                // 要素と、配置点(必要があれば回転済)を、Dictionaryに追加
                convertPtDic.Add(face, locationPt);
            }

            // Dictionaryを、回転させた点のX軸順にソートして、元のフェイスをソート方向順にソートしたリストを作成
            List<Face> sortedElems = new List<Face>();
            foreach (KeyValuePair<Face, XYZ> target in convertPtDic.OrderBy(pair => pair.Value.X)) {
                sortedElems.Add(target.Key);
            }

            return sortedElems;
        }

        /// <summary>
        /// 原点から指定された点までの、指定方向の距離を算出
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="sortDirection">距離計測方向</param>
        /// <returns></returns>
        private double GetSpecificDirectionDistance(XYZ point, XYZ sortDirection)
        {
            if (Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) < 0 ||
                Math.Round(sortDirection.Z, 3, MidpointRounding.AwayFromZero) == 0 && sortDirection.AngleTo(XYZ.BasisY) > Math.PI ||
                sortDirection.IsAlmostEqualTo(-XYZ.BasisZ))
            {
                sortDirection = sortDirection.Negate();
            }

            // 回転の必要性(ソート方向がX軸であれば回転の必要なし)
            bool needRotation = false;
            // 回転軸
            XYZ rotateAxis = XYZ.BasisZ;
            // X軸とソート方向の成す角度(回転角度)
            double angleRadianBetweenBasisXAndSortDir = 0;
            if (!sortDirection.IsAlmostEqualTo(XYZ.BasisX))
            {
                needRotation = true;

                // 回転軸を求める(X軸とソート方向の外積方向)
                rotateAxis = XYZ.BasisX.CrossProduct(sortDirection).Normalize();
                if (rotateAxis.IsZeroLength())
                    rotateAxis = XYZ.BasisZ;

                // X軸とソート方向の成す角度(A)を求める
                angleRadianBetweenBasisXAndSortDir = XYZ.BasisX.AngleTo(sortDirection);
            }

            // X軸とソート方向の外積を軸、ビュー原点を中心点として、対象点を角度(A)だけ回転させる
            XYZ rotatePt = point;
            if (needRotation)
            {
                rotatePt = GetRotationPoint(rotateAxis, rotatePt, -angleRadianBetweenBasisXAndSortDir, XYZ.Zero);
            }

            return rotatePt.X;
        }

        /// <summary>
        /// 指定の回転軸方向を軸とし、basePointを中心にpointをangleRadianだけ回転する
        /// </summary>
        /// <param name="rotationAxis">回転軸方向</param>
        /// <param name="point">回転させる点</param>
        /// <param name="angleRadian">回転角度</param>
        /// <param name="basePoint">回転の中心点</param>
        /// <returns>XYZ</returns>
        public static XYZ GetRotationPoint(XYZ rotationAxis, XYZ point, double angleRadian, XYZ basePoint)
        {
            Transform rotation = Transform.CreateRotationAtPoint(rotationAxis, angleRadian, basePoint);
            return rotation.OfPoint(point);
        }

        /// <summary>
        /// 簡易的なElement要素クラス(bool値のReferenceを持つ想定)
        /// </summary>
        public class TmpElement
        {
            // 要素
            public Element TmpEle { get; set; }

            // ID(連番)
            public int ID { get; set; }

            // コンストラクタ
            public TmpElement(Element element, int id)
            {
                TmpEle = element;
                ID = id;
            }
        }

        /// <summary>
        /// 位置でソートされた要素リストから、順に「芯-芯」寸法を作成
        /// </summary>
        private List<TmpElement> CreateDimension(List<TmpElement> elems, XYZ elementDirection, View activeView)
        {
            List<TmpElement> returnList = new List<TmpElement>();
            Debug.WriteLine("寸法作成開始");

            // ひとつでも寸法が作成できたか否かフラグ(初めて寸法が作成できるまでは要素を総当たり)
            bool couldCreateDim = false;

            // 寸法の始端側・終端側の要素Index
            int indexSt = 0;
            int indexEd = 1;

            while (indexEd < elems.Count) {
                // 寸法始端側・終端側の要素で寸法を作成 ※実際の作成処理に書き換えてください
                bool dimRes = false;
                if (Math.Abs(Doc.ActiveView.ViewDirection.Z) == 1) {
                    dimRes = TmpCreateDimensionXY(elems[indexSt].TmpEle, elems[indexEd].TmpEle, elementDirection, activeView);
                }
                else {
                    dimRes = TmpCreateDimension(elems[indexSt].TmpEle, elems[indexEd].TmpEle, elementDirection, activeView);
                }

                // 寸法が作成できた場合
                if (dimRes) {
                    Debug.WriteLine(string.Format("{0}-{1}", indexSt, indexEd));

                    bool checkFlag = true;
                    foreach (TmpElement tmp in returnList) {
                        if (tmp.ID == elems[indexSt].ID) {
                            checkFlag = false;
                        }
                    }
                    if (checkFlag) {
                        returnList.Add(elems[indexSt]);
                    }
                    checkFlag = true;
                    foreach (TmpElement tmp in returnList) {
                        if (tmp.ID == elems[indexEd].ID) {
                            checkFlag = false;
                        }
                    }
                    if (checkFlag) {
                        returnList.Add(elems[indexEd]);
                    }

                    // 寸法が作成できたフラグ→ON
                    couldCreateDim = true;

                    // 次の始端 = 今の終端に、次の終端 = 次の始端+1に更新
                    indexSt = indexEd;
                    indexEd = indexSt + 1;
                }
                // 寸法が作成できない、かつ、寸法作成済の場合
                else if (couldCreateDim) {
                    // 終端のみひとつずらす
                    ++indexEd;
                }
                // 寸法が作成できない、かつ、寸法未作成、かつ、始端と終端が1つだけ離れている場合
                else if (indexEd - indexSt == 1) {
                    // 始端は0に、終端はひとつずらす
                    indexSt = 0;
                    ++indexEd;
                }
                // 寸法が作成できない、かつ、寸法未作成、かつ、始端と終端が2つ以上離れている場合
                else {
                    // 始端のみひとつずらす
                    ++indexSt;
                }
            }

            Debug.WriteLine("寸法作成終了");

            return returnList;
        }

        /// <summary>
        /// 一時寸法の作成
        /// </summary>
        /// <param name="elem1">要素1</param>
        /// <param name="elem2">要素2</param>
        /// <param name="elementDirection">方向</param>
        /// <returns>真偽</returns>
        private bool TmpCreateDimension(Element elem1, Element elem2, XYZ elementDirection, View activeView)
        {
            XYZ ve = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
            if (Math.Round(ve.X, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(ve.Y, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(ve.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                elementDirection = new XYZ(0, 0, 1);
            }
            else {
                XYZ vDirection2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, ve), Doc.ActiveView.ViewDirection);
                elementDirection = vDirection2;
            }
            bool checkFlag = false;
            Element element1 = elem1;
            Element element2 = elem2;
            XYZ checkDirection1 = new XYZ(0, 0, 0);
            XYZ checkDirection2 = new XYZ(0, 0, 0);

            LocationCurve locationCurve1 = element1.Location as LocationCurve;
            Curve curve1 = null;
            if (locationCurve1 != null) {
                curve1 = locationCurve1.Curve;
            }
            else if (element1.GetType().Name == "Grid") {
                Grid grid = (Grid)element1;
                curve1 = grid.Curve;
            }
            else if (element1.GetType().Name == "FamilyInstance") {
                FamilyInstance instance = (FamilyInstance)element1;
                Transform t = instance.GetTransform();
                XYZ zero = new XYZ(0, 0, 0);
                XYZ baseZ = t.BasisZ;
                Line lineX = Line.CreateBound(zero, baseZ);
                curve1 = lineX;
            }
            Line line1 = (Line)curve1;
            if (curve1 != null) {
                Line li1 = (Line)curve1;
                XYZ vDirection = NormalViewDirection(li1, Doc.ActiveView.ViewDirection);
                if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                    checkDirection1 = new XYZ(0, 0, 1);
                }
                else {
                    XYZ vDirection2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, vDirection), Doc.ActiveView.ViewDirection);
                    checkDirection1 = vDirection2;
                }
            }
            LocationCurve locationCurve2 = element2.Location as LocationCurve;
            Curve curve2 = null;
            if (locationCurve2 != null) {
                curve2 = locationCurve2.Curve;
            }
            else if (element2.GetType().Name == "Grid") {
                Grid grid = (Grid)element2;
                curve2 = grid.Curve;
            }
            else if (element1.GetType().Name == "FamilyInstance") {
                FamilyInstance instance = (FamilyInstance)element2;
                Transform t = instance.GetTransform();
                XYZ zero = new XYZ(0, 0, 0);
                XYZ baseZ = t.BasisZ;
                Line lineX = Line.CreateBound(zero, baseZ);
                curve2 = lineX;
            }
            Line line2 = (Line)curve2;
            if (curve2 != null) {
                Line li2 = (Line)curve2;
                XYZ vDirection = NormalViewDirection(li2, Doc.ActiveView.ViewDirection);
                if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                    checkDirection2 = new XYZ(0, 0, 1);
                }
                else {
                    XYZ vDirection2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, vDirection), Doc.ActiveView.ViewDirection);
                    checkDirection2 = vDirection2;
                }
            }
            if (locationCurve1 != null && locationCurve2 != null) {
                // 平行かどうか
                if (IsParallel(checkDirection1, checkDirection2)) {
                    if (!IsParallel(checkDirection1, elementDirection)) {
                        return checkFlag;
                    }
                }
                else {
                    return checkFlag;
                }
            }
            Reference r1 = new Reference(element1);
            Reference r2 = new Reference(element2);
            // 芯用ディクショナリー
            Dictionary<Reference, XYZ> refDic1 = new Dictionary<Reference, XYZ>();
            Dictionary<Reference, XYZ> refDic2 = new Dictionary<Reference, XYZ>();
            // 芯を取得
            if (element1.GetType().Name == "FamilyInstance") {
                FamilyInstance ins1 = (FamilyInstance)element1;
                // 中心正面/背面
                IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0) {
                    foreach (Reference r in frontBack) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic1.Add(r, li.Direction);
                    }
                }
                if (leftRight.Count > 0) {
                    foreach (Reference r in leftRight) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic1.Add(r, li.Direction);
                    }
                }
                if (centerElevation.Count > 0) {
                    foreach (Reference r in centerElevation) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic1.Add(r, li.Direction);
                    }
                }
            }
            else if (element1.GetType().Name == "FabricationPart") {
                int lineInt = GetInstanceLine(element1, elementDirection);
                Line li = GetSymbolLine(element1, lineInt);
                refDic1.Add(li.Reference, li.Direction);
            }
            // ファミリインスタンス、ファブリケーションパーツ以外の場合
            else {
                r1 = new Reference(element1);
                locationCurve1 = element1.Location as LocationCurve;
                curve1 = null;
                if (locationCurve1 != null) {
                    curve1 = locationCurve1.Curve;
                }
                else if (element1.GetType().Name == "Grid") {
                    Grid grid = (Grid)element1;
                    curve1 = grid.Curve;
                }
                Line li = null;
                XYZ lineDirection = new XYZ(0, 0, 0);
                if (curve1 != null) {
                    li = (Line)curve1;
                    XYZ vDirection = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                    if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                        li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                        lineDirection = li.Direction;
                    }
                    else if (Math.Round(li.Direction.X, 3, MidpointRounding.AwayFromZero) != 1
                        && Math.Round(li.Direction.Y, 3, MidpointRounding.AwayFromZero) != 1) {
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1) {
                            li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                            lineDirection = li.Direction;
                        }
                        else {
                            lineDirection = li.Direction;
                        }
                    }
                    else {
                        lineDirection = li.Direction;
                    }
                }
                if (Math.Round(lineDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                }
                else {
                    refDic1.Add(r1, lineDirection);
                }
            }
            if (element2.GetType().Name == "FamilyInstance") {
                FamilyInstance ins2 = (FamilyInstance)element2;
                // 中心正面/背面
                IList<Reference> frontBack = ins2.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins2.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins2.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0) {
                    foreach (Reference r in frontBack) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic2.Add(r, li.Direction);
                    }
                }
                if (leftRight.Count > 0) {
                    foreach (Reference r in leftRight) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic2.Add(r, li.Direction);
                    }
                }
                if (centerElevation.Count > 0) {
                    foreach (Reference r in centerElevation) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        Line li = Line.CreateBound(XYZ.Zero, xyz);
                        XYZ v = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                        li = Line.CreateBound(XYZ.Zero, v);
                        refDic2.Add(r, li.Direction);
                    }
                }
            }
            else if (element2.GetType().Name == "FabricationPart") {
                int lineInt = GetInstanceLine(element2, elementDirection);
                Line li = GetSymbolLine(element2, lineInt);
                refDic2.Add(li.Reference, li.Direction);
            }
            // ファミリインスタンス、ファブリケーションパーツ以外の場合
            else {
                r2 = new Reference(element2);
                locationCurve2 = element2.Location as LocationCurve;
                curve2 = null;
                if (locationCurve2 != null) {
                    curve2 = locationCurve2.Curve;
                }
                else if (element2.GetType().Name == "Grid") {
                    Grid grid = (Grid)element2;
                    curve2 = grid.Curve;
                }
                XYZ lineDirection = new XYZ(0, 0, 0);
                if (curve2 != null) {
                    Line li = (Line)curve2;
                    XYZ vDirection = NormalViewDirection(li, Doc.ActiveView.ViewDirection);
                    if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                        li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                        lineDirection = li.Direction;
                    }
                    else if (Math.Round(li.Direction.X, 3, MidpointRounding.AwayFromZero) != 1
                        && Math.Round(li.Direction.Y, 3, MidpointRounding.AwayFromZero) != 1) {
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 1) {
                            li = Line.CreateBound(XYZ.Zero, new XYZ(0, 0, 1));
                            lineDirection = li.Direction;
                        }
                        else {
                            lineDirection = li.Direction;
                        }
                    }
                    else {
                        lineDirection = li.Direction;
                    }
                }
                if (Math.Round(lineDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                    refDic2.Add(r2, lineDirection);
                }
                else {
                    refDic2.Add(r2, lineDirection);
                }
            }
            foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                foreach (KeyValuePair<Reference, XYZ> kvp2 in refDic2) {
                    // 平行かどうか
                    if (IsParallel(kvp.Value, kvp2.Value)) {
                        XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                        if (Math.Round(vDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(vDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            vDirection = new XYZ(0, 0, 1);
                        }
                        XYZ point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        XYZ point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                        try {
                            ReferenceArray ra = new ReferenceArray();

                            ra.Append(kvp.Key);
                            ra.Append(kvp2.Key);

                            Line line = Line.CreateBound(point1, point2);

                            using (Transaction transaction = new Transaction(Doc, "寸法作成")) {
                                transaction.Start();
                                Dimension dim = Doc.Create.NewDimension(
                                Doc.ActiveView, line, ra);
                                if (dim.ValueString.Contains("-")) {
                                    checkFlag = false;
                                }
                                else {
                                    checkFlag = true;
                                    return checkFlag;
                                }
                                transaction.RollBack();
                            }
                        }
                        catch (Exception ex) {
                            checkFlag = false;
                            string err = ex.Message;
                            if (element1 != null) {
                                errorIdSet.Add(element1.Id.ToString());
                            }
                            if (element2 != null) {
                                errorIdSet.Add(element2.Id.ToString());
                            }
                        }
                    }
                }
            }

            return checkFlag;
        }

        /// <summary>
        /// 一時寸法の作成XY
        /// </summary>
        /// <param name="elem1">要素1</param>
        /// <param name="elem2">要素2</param>
        /// <param name="elementDirection">方向</param>
        /// <returns>真偽</returns>
        private bool TmpCreateDimensionXY(Element elem1, Element elem2, XYZ elementDirection, View activeView)
        {
            bool checkFlag = false;
            if (Math.Round(elementDirection.X, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(elementDirection.Y, 3, MidpointRounding.AwayFromZero) == 0
                && Math.Round(elementDirection.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                return checkFlag;
            }
            Element element1 = elem1;
            Element element2 = elem2;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();

            LocationCurve locationCurve1 = element1.Location as LocationCurve;
            Curve curve1 = null;
            if (locationCurve1 != null) {
                if (locationCurve1.Curve.GetType().Name == "Line") {
                    curve1 = locationCurve1.Curve;
                }
            }
            else if (element1.GetType().Name == "Grid") {
                Grid grid = (Grid)element1;
                curve1 = grid.Curve;
            }

            XYZ lineDirection1 = new XYZ(0, 0, 0);
            if (curve1 != null && curve1.GetType().Name == "Line") {
                Line line1 = (Line)curve1;
                lineDirection1 = line1.Direction;
                lineList1.Add(line1);
            }

            LocationCurve locationCurve2 = element2.Location as LocationCurve;
            Curve curve2 = null;
            if (locationCurve2 != null) {
                if (locationCurve2.Curve.GetType().Name == "Line") {
                    curve2 = locationCurve2.Curve;
                }
            }
            else if (element2.GetType().Name == "Grid") {
                Grid grid = (Grid)element2;
                curve2 = grid.Curve;
            }
            XYZ lineDirection2 = new XYZ(0, 0, 0);
            if (curve2 != null && curve2.GetType().Name == "Line") {
                Line line2 = (Line)curve2;
                lineDirection2 = line2.Direction;
                lineList2.Add(line2);
            }
            Reference r1 = new Reference(element1);
            Reference r2 = new Reference(element2);
            // 芯用ディクショナリー
            Dictionary<Reference, XYZ> refDic1 = new Dictionary<Reference, XYZ>();
            Dictionary<Reference, XYZ> refDic2 = new Dictionary<Reference, XYZ>();
            // 芯を取得
            if (element1.GetType().Name == "FamilyInstance") {
                FamilyInstance ins1 = (FamilyInstance)element1;
                // 中心正面/背面
                IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0) {
                    foreach (Reference r in frontBack) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic1.Add(r, v);
                    }
                }
                if (leftRight.Count > 0) {
                    foreach (Reference r in leftRight) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic1.Add(r, v);
                    }
                }
                if (centerElevation.Count > 0) {
                    foreach (Reference r in centerElevation) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic1.Add(r, v);
                    }
                }
            }
            else if (element1.GetType().Name == "FabricationPart") {
                int lineInt = GetInstanceLine(element1, elementDirection);
                Line line = GetSymbolLine(element1, lineInt);
                if (line != null) {
                    refDic1.Add(line.Reference, elementDirection);
                }
            }
            // ファミリインスタンス、ファブリケーションパーツ以外の場合
            else {
                r1 = new Reference(element1);
                locationCurve1 = element1.Location as LocationCurve;
                curve1 = null;
                if (locationCurve1 != null) {
                    if (locationCurve1.Curve.GetType().Name == "Line") {
                        curve1 = locationCurve1.Curve;
                    }
                }
                else if (element1.GetType().Name == "Grid") {
                    Grid grid = (Grid)element1;
                    curve1 = grid.Curve;
                }
                Line line1 = null;
                lineDirection1 = new XYZ(0, 0, 0);
                if (curve1 != null && curve1.GetType().Name == "Line") {
                    line1 = (Line)curve1;
                    lineDirection1 = line1.Direction;
                }
                if (Math.Round(lineDirection1.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection1.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection1.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                }
                else {
                    refDic1.Add(r1, lineDirection1);
                }
            }
            if (element2.GetType().Name == "FamilyInstance") {
                FamilyInstance ins2 = (FamilyInstance)element2;
                // 中心正面/背面
                IList<Reference> frontBack = ins2.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                // 中心左/右
                IList<Reference> leftRight = ins2.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                // 中心立面図
                IList<Reference> centerElevation = ins2.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                if (frontBack.Count > 0) {
                    foreach (Reference r in frontBack) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic2.Add(r, v);
                    }
                }
                if (leftRight.Count > 0) {
                    foreach (Reference r in leftRight) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic2.Add(r, v);
                    }
                }
                if (centerElevation.Count > 0) {
                    foreach (Reference r in centerElevation) {
                        XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                        if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                            && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            break;
                        }
                        XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                        refDic2.Add(r, v);
                    }
                }
            }
            else if (element2.GetType().Name == "FabricationPart") {
                int lineInt = GetInstanceLine(element2, elementDirection);
                Line line = GetSymbolLine(element2, lineInt);
                if (line != null) {
                    refDic2.Add(line.Reference, elementDirection);
                }
            }
            // ファミリインスタンス、ファブリケーションパーツ以外の場合
            else {
                r2 = new Reference(element2);
                locationCurve2 = element2.Location as LocationCurve;
                curve2 = null;
                if (locationCurve2 != null) {
                    if (locationCurve2.Curve.GetType().Name == "Line") {
                        curve2 = locationCurve2.Curve;
                    }
                }
                else if (element2.GetType().Name == "Grid") {
                    Grid grid = (Grid)element2;
                    curve2 = grid.Curve;
                }
                lineDirection2 = new XYZ(0, 0, 0);
                if (curve2 != null && curve2.GetType().Name == "Line") {
                    Line line2 = (Line)curve2;
                    lineDirection2 = line2.Direction;
                }
                if (Math.Round(lineDirection2.X, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection2.Y, 3, MidpointRounding.AwayFromZero) == 0
                    && Math.Round(lineDirection2.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                }
                else {
                    refDic2.Add(r2, lineDirection2);
                }
            }
            foreach (KeyValuePair<Reference, XYZ> kvp in refDic1) {
                foreach (KeyValuePair<Reference, XYZ> kvp2 in refDic2) {
                    Line l1 = Line.CreateBound(XYZ.Zero, kvp.Value);
                    Line l2 = Line.CreateBound(XYZ.Zero, kvp2.Value);
                    XYZ p1 = NormalViewDirection(l1, Doc.ActiveView.ViewDirection);
                    XYZ p2 = NormalViewDirection(l2, Doc.ActiveView.ViewDirection);
                    // 平行かどうか
                    if (IsParallel(p1, p2)) {
                        XYZ vDirection = NormalViewDirection(Line.CreateBound(XYZ.Zero, elementDirection), Doc.ActiveView.ViewDirection);
                        // 平行でない場合はコンティニュー
                        if (!IsParallel(p1, vDirection) || !IsParallel(p2, vDirection)) {
                            continue;
                        }
                        XYZ point1 = new XYZ(dimensionPoint.X, dimensionPoint.Y, dimensionPoint.Z);
                        XYZ point2 = new XYZ(dimensionPoint.X + vDirection.X, dimensionPoint.Y + vDirection.Y, dimensionPoint.Z + vDirection.Z);
                        try {
                            ReferenceArray ra = new ReferenceArray();

                            ra.Append(kvp.Key);
                            ra.Append(kvp2.Key);

                            Line line = Line.CreateBound(point1, point2);

                            using (Transaction transaction = new Transaction(Doc, "寸法作成")) {
                                transaction.Start();
                                Dimension dim = Doc.Create.NewDimension(
                                Doc.ActiveView, line, ra);
                                if (dim.ValueString.Contains("-")) {
                                    checkFlag = false;
                                }
                                else {
                                    checkFlag = true;
                                    transaction.RollBack();
                                    return checkFlag;
                                }
                                transaction.RollBack();
                            }
                        }
                        catch (Exception) {
                            checkFlag = false;
                            if (element1 != null) {
                                errorIdSet.Add(element1.Id.ToString());
                            }
                            if (element2 != null) {
                                errorIdSet.Add(element2.Id.ToString());
                            }
                        }
                    }
                }
            }

            return checkFlag;
        }

        /// <summary>
        /// 方向取得XY
        /// </summary>
        /// <param name="elementList">要素リスト</param>
        /// <returns>方向リスト</returns>
        private List<XYZ> GetDirectionXY(List<Element> elementList, View activeView)
        {
            List<XYZ> directionList = new List<XYZ>();
            List<XYZ> checkXYZ = new List<XYZ>();

            foreach (Element element1 in elementList) {
                LocationCurve locationCurve1 = element1.Location as LocationCurve;
                Curve curve1 = null;
                List<XYZ> directionList1 = new List<XYZ>();
                if (locationCurve1 != null) {
                    curve1 = locationCurve1.Curve;
                }
                else if (element1.GetType().Name == "Grid") {
                    Grid grid = (Grid)element1;
                    curve1 = grid.Curve;
                }
                else if (element1.GetType().Name == "FamilyInstance" && locationCurve1 == null && !faceRadio.Checked) {
                    FamilyInstance ins1 = (FamilyInstance)element1;
                    // 中心正面/背面
                    IList<Reference> frontBack = ins1.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                    // 中心左/右
                    IList<Reference> leftRight = ins1.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                    // 中心立面図
                    IList<Reference> centerElevation = ins1.GetReferences(FamilyInstanceReferenceType.CenterElevation);

                    if (frontBack.Count > 0) {
                        foreach (Reference r in frontBack) {
                            XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                            if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                break;
                            }
                            XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                            directionList1.Add(v);
                        }
                    }
                    if (leftRight.Count > 0) {
                        foreach (Reference r in leftRight) {
                            XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                            if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                break;
                            }
                            XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                            directionList1.Add(v);
                        }
                    }
                    if (centerElevation.Count > 0) {
                        foreach (Reference r in centerElevation) {
                            XYZ xyz = GetReferenceDirection(Doc, activeView.RightDirection, r);
                            if (Math.Round(xyz.X, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Y, 3, MidpointRounding.AwayFromZero) == 0
                                && Math.Round(xyz.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                                break;
                            }
                            XYZ v = NormalViewDirection(Line.CreateBound(XYZ.Zero, xyz), Doc.ActiveView.ViewDirection);
                            directionList1.Add(v);
                        }
                    }
                }
                Line line1 = null;
                if (curve1 != null && curve1.GetType().Name == "Line") {
                    line1 = (Line)curve1;
                    XYZ v = NormalViewDirection(line1, Doc.ActiveView.ViewDirection);
                    if (Math.Round(v.X, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(v.Y, 3, MidpointRounding.AwayFromZero) == 0
                        && Math.Round(v.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                        directionList1.Add(line1.Direction);
                    }
                    else {
                        XYZ v2 = NormalViewDirection(Line.CreateBound(XYZ.Zero, v), Doc.ActiveView.ViewDirection);
                        directionList1.Add(Line.CreateBound(XYZ.Zero, v2).Direction);
                    }
                }
                if (locationCurve1 == null && faceRadio.Checked) {
                    // 面もしくはエッジの方向を取得
                    HashSet<XYZ> faceDirectionSet = new HashSet<XYZ>();
                    HashSet<XYZ> edgeDirectionSet = new HashSet<XYZ>();
                    List<KeyValuePair<double, double>> kvpList = new List<KeyValuePair<double, double>>();
                    HashSet<XYZ> deleteDirectionSet = new HashSet<XYZ>();
                    List<KeyValuePair<Face, Element>> allFaces = new List<KeyValuePair<Face, Element>>();
                    List<KeyValuePair<Edge, Element>> allEdges = new List<KeyValuePair<Edge, Element>>();
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.View = Doc.ActiveView;
                    opt.IncludeNonVisibleObjects = false;
                    List<Face> faces = GetInstanceAllFaceList(element1, opt);
                    foreach (Face face in faces) {
                        PlanarFace pf = face as PlanarFace;
                        if (Math.Round(pf.FaceNormal.Z, 3, MidpointRounding.AwayFromZero) == 0) {
                            if (!kvpList.Contains(new KeyValuePair<double, double>(Math.Round(pf.FaceNormal.X, 4, MidpointRounding.AwayFromZero), Math.Round(pf.FaceNormal.Y, 4, MidpointRounding.AwayFromZero)))) {
                                faceDirectionSet.Add(pf.FaceNormal);
                                kvpList.Add(new KeyValuePair<double, double>(Math.Round(pf.FaceNormal.X, 4, MidpointRounding.AwayFromZero), Math.Round(pf.FaceNormal.Y, 4, MidpointRounding.AwayFromZero)));
                            }
                        }
                    }
                    List<XYZ> checkList = new List<XYZ>();
                    foreach (XYZ dir in faceDirectionSet) {
                        List<PlanarFace> pFaceList = new List<PlanarFace>();
                        bool checkFlag = false;
                        foreach (XYZ checkDir in checkList) {
                            // 平行かどうか
                            if (IsParallel(checkDir, dir)) {
                                checkFlag = true;
                            }
                        }
                        if (checkFlag) {
                            continue;
                        }
                        foreach (Face face in faces) {
                            PlanarFace pf = face as PlanarFace;
                            // 平行かどうか
                            if (IsParallel(dir, pf.FaceNormal)) {
                                pFaceList.Add(pf);
                            }
                        }
                        XYZ xyz = dir;
                        double x = 0;
                        double y = 0;
                        double slo = xyz.Y / xyz.X;
                        if (double.IsInfinity(slo)) {
                            x = 1;
                            y = 0;
                        }
                        else if (Math.Round(slo, 3, MidpointRounding.AwayFromZero) == 0) {
                            x = 0;
                            y = 1;
                        }
                        else {
                            double vs = -1 / slo;
                            x = 1;
                            y = 1 * vs;
                        }
                        xyz = new XYZ(x, y, xyz.Z).Normalize();
                        List<Face> faceList = FaceSortPointByDirection(dir, pFaceList.Cast<Face>().ToList());
                        allFaces.Add(new KeyValuePair<Face, Element>(faceList.First(), element1));
                        allFaces.Add(new KeyValuePair<Face, Element>(faceList.Last(), element1));
                        checkList.Add(dir);
                    }

                    if (allFaces.Count == 0) {
                        List<Edge> edges = GetAllInstanceEdgeList(element1, opt);
                        foreach (Edge edge in edges) {
                            Line edgeLine = edge.AsCurve() as Line;
                            if (edgeLine != null) {
                                if (!kvpList.Contains(new KeyValuePair<double, double>(edgeLine.Direction.X, edgeLine.Direction.Y))) {
                                    edgeDirectionSet.Add(edgeLine.Direction);
                                    kvpList.Add(new KeyValuePair<double, double>(edgeLine.Direction.X, edgeLine.Direction.Y));
                                }
                            }
                        }
                        checkList = new List<XYZ>();
                        foreach (XYZ dir in edgeDirectionSet) {
                            List<Edge> edgeList = new List<Edge>();
                            bool checkFlag = false;
                            foreach (XYZ checkDir in checkList) {
                                // 平行かどうか
                                if (IsParallel(checkDir, dir)) {
                                    checkFlag = true;
                                }
                            }
                            if (checkFlag) {
                                continue;
                            }
                            foreach (Edge edge in edges) {
                                Line edgeLine = edge.AsCurve() as Line;
                                // 平行かどうか
                                if (IsParallel(dir, edgeLine.Direction)) {
                                    edgeList.Add(edge);
                                }
                            }
                            XYZ xyz = dir;
                            double x = 0;
                            double y = 0;
                            double slo = xyz.Y / xyz.X;
                            if (double.IsInfinity(slo)) {
                                x = 1;
                                y = 0;
                            }
                            else if (Math.Round(slo, 3, MidpointRounding.AwayFromZero) == 0) {
                                x = 0;
                                y = 1;
                            }
                            else {
                                double vs = -1 / slo;
                                x = 1;
                                y = 1 * vs;
                            }
                            xyz = new XYZ(x, y, xyz.Z).Normalize();
                            edgeList = EdgeSortPointByDirection(dir, edgeList);
                            allEdges.Add(new KeyValuePair<Edge, Element>(edgeList.First(), element1));
                            allEdges.Add(new KeyValuePair<Edge, Element>(edgeList.Last(), element1));
                            checkList.Add(dir);
                        }
                    }
                    int count1 = 1;
                    foreach (XYZ xyz in faceDirectionSet) {
                        int count2 = 1;
                        foreach (XYZ xyz2 in faceDirectionSet) {
                            if (count2 <= count1) {
                                count2++;
                                continue;
                            }
                            // 平行かどうか
                            if (IsParallel(xyz, xyz2)) {
                                deleteDirectionSet.Add(xyz2);
                            }
                            count2++;
                        }
                        count1++;
                    }
                    foreach (XYZ xyz in deleteDirectionSet) {
                        faceDirectionSet.Remove(xyz);
                    }
                    deleteDirectionSet = new HashSet<XYZ>();
                    foreach (XYZ xyz in edgeDirectionSet) {
                        count1 = 1;
                        int count2 = 1;
                        foreach (XYZ xyz2 in edgeDirectionSet) {
                            if (count2 > count1) {
                                continue;
                            }
                            // 平行かどうか
                            if (IsParallel(xyz, xyz2)) {
                                deleteDirectionSet.Add(xyz2);
                            }
                            count2++;
                        }
                        count1++;
                    }
                    foreach (XYZ xyz in deleteDirectionSet) {
                        edgeDirectionSet.Remove(xyz);
                    }
                    HashSet<List<KeyValuePair<PlanarFace, Element>>> faceListSet = new HashSet<List<KeyValuePair<PlanarFace, Element>>>();
                    foreach (XYZ xyz in faceDirectionSet) {
                        List<KeyValuePair<PlanarFace, Element>> facesList = new List<KeyValuePair<PlanarFace, Element>>();
                        foreach (KeyValuePair<Face, Element> kvp in allFaces) {
                            Face face = kvp.Key;
                            PlanarFace pf = face as PlanarFace;
                            // 平行かどうかどうか
                            if (IsParallel(xyz, pf.FaceNormal)) {
                                // 重複チェック
                                if (!facesList.Contains(new KeyValuePair<PlanarFace, Element>(pf, kvp.Value))) {
                                    facesList.Add(new KeyValuePair<PlanarFace, Element>(pf, kvp.Value));
                                }
                            }
                        }
                        if (facesList.Count > 0) {
                            faceListSet.Add(facesList);
                        }
                    }
                    HashSet<List<KeyValuePair<Edge, Element>>> edgeListSet = new HashSet<List<KeyValuePair<Edge, Element>>>();
                    foreach (XYZ xyz in edgeDirectionSet) {
                        List<KeyValuePair<Edge, Element>> edges = new List<KeyValuePair<Edge, Element>>();
                        foreach (KeyValuePair<Edge, Element> kvp in allEdges) {
                            Edge edge = kvp.Key;
                            Line edgeLine = edge.AsCurve() as Line;
                            // 平行かどうか
                            if (IsParallel(xyz, edgeLine.Direction)) {
                                if (!edges.Contains(new KeyValuePair<Edge, Element>(edge, kvp.Value))) {
                                    edges.Add(new KeyValuePair<Edge, Element>(edge, kvp.Value));
                                }
                            }
                        }
                        if (edges.Count > 0) {
                            edgeListSet.Add(edges);
                        }
                    }
                    List<XYZ> xyzList = new List<XYZ>();
                    // リストに設定
                    foreach (XYZ fDirection in faceDirectionSet) {
                        XYZ direction = NormalViewDirection(Line.CreateBound(XYZ.Zero, fDirection), Doc.ActiveView.ViewDirection);
                        directionList1.Add(direction);
                    }
                    if (faceDirectionSet.Count == 0) {
                        foreach (XYZ fDirection in edgeDirectionSet) {
                            directionList1.Add(fDirection);
                        }
                    }
                    if (faceDirectionSet.Count == 0 && edgeDirectionSet.Count == 0) {
                        List<Line> lineList = GetLine(element1);
                        foreach (Line line in lineList) {
                            directionList1.Add(line.Direction);
                        }
                    }
                }
                foreach (XYZ xyz in directionList1) {
                    bool check = true;
                    foreach (XYZ ch in checkXYZ) {
                        // 平行なものは除外
                        if (IsParallel(ch, xyz)) {
                            check = false;
                        }
                    }
                    if (check) {
                        checkXYZ.Add(xyz);
                        directionList.Add(xyz);
                    }
                }
            }
            return directionList;
        }
    }
}