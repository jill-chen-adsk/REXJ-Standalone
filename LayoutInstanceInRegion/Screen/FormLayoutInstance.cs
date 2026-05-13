using ADSK.JExtRAC.LayoutInstanceInRegion.Commands;
using ADSK.JExtRAC.LayoutInstanceInRegion.Common;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using ComboBox = System.Windows.Forms.ComboBox;
using Control = System.Windows.Forms.Control;
using Form = System.Windows.Forms.Form;
using TaskDialogIcon = Autodesk.Revit.UI.TaskDialogIcon ;
using TextBox = System.Windows.Forms.TextBox;
using View = Autodesk.Revit.DB.View;
using ADSK.JExtRAC.LayoutInstanceInRegion.Resources;
using Resources = ADSK.JExtRAC.LayoutInstanceInRegion.Resources;

namespace ADSK.JExtRAC.LayoutInstanceInRegion.Screen
{
    public partial class FormLayoutInstance : Form
    {
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
        /// 要素リスト
        /// </summary>
        private List<Element> elementList = new List<Element>();

        /// <summary>
        /// レベル
        /// </summary>
        private List<Level> levelList = new List<Level>();

        /// <summary>
        /// カテゴリーセット
        /// </summary>
        private SortedSet<string> categorySet = new SortedSet<string>();

        /// <summary>
        /// ファミリ名セット
        /// </summary>
        private SortedSet<string> familySet = new SortedSet<string>();

        /// <summary>
        /// ファミリーディクショナリー
        /// </summary>
        private Dictionary<Element, Category> familyDic = new Dictionary<Element, Category>();

        /// <summary>
        /// タイプディクショナリー
        /// </summary>
        private Dictionary<string, string> typeDic = new Dictionary<string, string>();

        /// <summary>
        /// Revitドキュメント
        /// </summary>
        private Document Doc;

        /// <summary>
        /// 選択要素
        /// </summary>
        public Element selectElement;

        /// <summary>
        /// ピックボックス
        /// </summary>
        public PickedBox pickedBox;

        /// <summary>
        /// 詳細線分
        /// </summary>
        public DetailCurve detailLine;

        /// <summary> is select object</summary>
        public bool _isSelectObject;

        /// <summary> is object</summary>
        public bool _isObject;

        /// <summary> is object</summary>
        public bool _isRegion;

        public bool _isAngle;

        /// <summary>
        /// OKフラグ
        /// t</summary>
        private bool okFlag;

        public FormLayoutInstance(ExternalCommandData commandData)
        {
            InitializeComponent();

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;

            List<BuiltInCategory> builtInCategoryList = new List<BuiltInCategory>(){
            BuiltInCategory.OST_Sprinklers,
            BuiltInCategory.OST_DataDevices,
            BuiltInCategory.OST_DuctTerminal,
            BuiltInCategory.OST_Site,
            BuiltInCategory.OST_Furniture,
            BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_Planting,
            BuiltInCategory.OST_StructuralFoundation,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_LightingFixtures,
            BuiltInCategory.OST_LightingDevices,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_DetailComponents,
            BuiltInCategory.OST_ElectricalFixtures,
            BuiltInCategory.OST_ElectricalEquipment,
            BuiltInCategory.OST_TelephoneDevices,
            BuiltInCategory.OST_Parking,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_Casework,
            BuiltInCategory.OST_NurseCallDevices,
            BuiltInCategory.OST_FireAlarmDevices,
            BuiltInCategory.OST_SecurityDevices,
            BuiltInCategory.OST_CommunicationDevices
            };
            ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(builtInCategoryList);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Doc);
            elementList = filteredElementCollector.OfClass(typeof(FamilySymbol)).WherePasses(elementMulticategoryFilter).ToList();

            foreach (Element element in elementList) {
                categorySet.Add(element.Category.Name);
                familyDic.Add(element, element.Category);
            }

            int count = 0;
            string firstName = null;
            foreach (string name in categorySet) {
                categoryListBox.Items.Add(name);
                if (count == 0) {
                    firstName = name;
                }
                count++;
            }

            foreach (KeyValuePair<Element, Category> kvp in familyDic) {
                ElementType elementType = (ElementType)kvp.Key;
                if (kvp.Value.Name == firstName) {
                    familySet.Add(elementType.FamilyName);
                }
            }

            foreach (string name in familySet) {
                familyListBox.Items.Add(name);
            }

            string firstFamily = familyListBox.Items[0].ToString();
            foreach (KeyValuePair<Element, Category> kvp in familyDic) {
                ElementType elementType = (ElementType)kvp.Key;
                if (elementType.FamilyName == firstFamily) {
                    typeDic.Add(kvp.Key.Name, elementType.FamilyName);
                }
            }
            // ソート
            IOrderedEnumerable<KeyValuePair<string, string>> typeDic2 = typeDic.OrderBy(selector => { return selector.Key; });
            foreach (KeyValuePair<string, string> kvp in typeDic2) {
                if (kvp.Value == firstFamily) {
                    typeListBox.Items.Add(kvp.Key);
                }
            }

            categoryListBox.SelectedIndex = 0;
            familyListBox.SelectedIndex = 0;
            typeListBox.SelectedIndex = 0;
            setPatternX.SelectedIndex = 0;
            setPatternY.SelectedIndex = 0;
            objectLabel.Show();
            regionLabel.Show();
            pickObjectButton.Enabled = true;
            pickRegionButton.Enabled = false;
            countComboX.Enabled = true;
            countComboX.SelectedIndex = 0;
            minIntervalX.Enabled = false;
            maxIntervalX.Enabled = false;
            countComboY.Enabled = true;
            countComboY.SelectedIndex = 0;
            minIntervalY.Enabled = false;
            maxIntervalY.Enabled = false;
            familyAngleCombo.SelectedIndex = 4;
            axisAngleCombo.SelectedIndex = 2;
            okButton.Enabled = false;
            applyButton.Enabled = false;

            leftMarginText.KeyPress += TextBoxPrice_PreviewTextInput;
            rightMarginText.KeyPress += TextBoxPrice_PreviewTextInput;
            frontMarginText.KeyPress += TextBoxPrice_PreviewTextInput;
            backMarginText.KeyPress += TextBoxPrice_PreviewTextInput;
            offsetText.KeyPress += TextBoxPrice_PreviewTextInput;
            familyAngleCombo.KeyPress += FamilyAngleTextBoxPrice_PreviewTextInput;
            axisAngleCombo.KeyPress += FamilyAngleTextBoxPrice_PreviewTextInput;
            countComboX.KeyPress += Count_PreviewTextInput;
            countComboY.KeyPress += Count_PreviewTextInput;
            minIntervalX.KeyPress += TextBoxPrice_PreviewTextInput;
            maxIntervalX.KeyPress += TextBoxPrice_PreviewTextInput;
            minIntervalY.KeyPress += TextBoxPrice_PreviewTextInput;
            maxIntervalY.KeyPress += TextBoxPrice_PreviewTextInput;

            leftMarginText.MaxLength = 7;
            rightMarginText.MaxLength = 7;
            frontMarginText.MaxLength = 7;
            backMarginText.MaxLength = 7;
            offsetText.MaxLength = 7;
            familyAngleCombo.MaxLength = 4;
            axisAngleCombo.MaxLength = 4;
            countComboX.MaxLength = 7;
            countComboY.MaxLength = 7;
            minIntervalX.MaxLength = 7;
            maxIntervalX.MaxLength = 7;
            minIntervalY.MaxLength = 7;
            maxIntervalY.MaxLength = 7;
        }

        /// <summary>
        /// ドキュメント内の要素を、クラスでフィルタリングして取得する。
        /// </summary>
        /// <typeparam name="T">フィルタリングするクラス</typeparam>
        /// <param name="doc">対象のドキュメント</param>
        /// <returns></returns>
        public static List<T> GetElements<T>(Document doc)
        {
            Type type = typeof(T);

            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 部屋/スペースボタンクリック
        /// </summary>
        private void PickObjectButton_Click(object sender, System.EventArgs e)
        {
            _isSelectObject = true;
            _isObject = true;
            _isRegion = false;
            _isAngle = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// 領域指定ボタンクリック
        /// </summary>
        private void PickRegionButton_Click(object sender, System.EventArgs e)
        {
            _isSelectObject = true;
            _isObject = false;
            _isRegion = true;
            _isAngle = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// 選択範囲ラジオボタンチェンジ
        /// </summary>
        private void SetRegionRadio_Change(object sender, System.EventArgs e)
        {
            bool enableFlag = true;
            if (objectRadio.Checked) {
                pickObjectButton.Enabled = true;
                pickRegionButton.Enabled = false;
                if (objectLabel.Text == Resources.Text.LabelNotSelected) {
                    enableFlag = false;
                }
            }
            else {
                pickObjectButton.Enabled = false;
                pickRegionButton.Enabled = true;
                if (regionLabel.Text == Resources.Text.LabelNotSpecified) {
                    enableFlag = false;
                }
            }
            if (countSetRadioX.Checked) {
                if (countComboX.Text == "") {
                    enableFlag = false;
                }
                else if (countComboX.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalX.Text == "" && maxIntervalX.Text == "") {
                    enableFlag = false;
                }
            }
            if (countSetRadioY.Checked) {
                if (countComboY.Text == "") {
                    enableFlag = false;
                }
                else if (countComboY.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalY.Text == "" && maxIntervalY.Text == "") {
                    enableFlag = false;
                }
            }
            if (!int.TryParse(familyAngleCombo.Text, out int fa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(axisAngleCombo.Text, out int aa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(leftMarginText.Text, out int lm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(rightMarginText.Text, out int rm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(frontMarginText.Text, out int fm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(backMarginText.Text, out int bm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(offsetText.Text, out int ot)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(minIntervalX.Text, out int minX)) {
                enableFlag = false;
            }
            else if (minIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(minIntervalY.Text, out int minY)) {
                enableFlag = false;
            }
            else if (maxIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(maxIntervalX.Text, out int maxX)) {
                enableFlag = false;
            }
            else if (maxIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(maxIntervalY.Text, out int maxY)) {
                enableFlag = false;
            }
            if (enableFlag) {
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
            Preview_Change(sender, e);
        }

        /// <summary>
        /// カテゴリーチェンジ
        /// </summary>
        private void Category_Change(object sender, System.EventArgs e)
        {
            string selectCategory = categoryListBox.SelectedItem.ToString();

            familySet.Clear();
            foreach (KeyValuePair<Element, Category> kvp in familyDic) {
                ElementType elementType = (ElementType)kvp.Key;
                if (kvp.Value.Name == selectCategory) {
                    familySet.Add(elementType.FamilyName);
                }
            }

            familyListBox.Items.Clear();
            foreach (string name in familySet) {
                familyListBox.Items.Add(name);
            }
            familyListBox.SelectedIndex = 0;
        }

        /// <summary>
        /// カテゴリーチェンジ
        /// </summary>
        private void Family_Change(object sender, System.EventArgs e)
        {
            string selectFamily = familyListBox.SelectedItem.ToString();

            typeDic.Clear();
            foreach (KeyValuePair<Element, Category> kvp in familyDic) {
                ElementType elementType = (ElementType)kvp.Key;
                if (elementType.FamilyName == selectFamily) {
                    typeDic.Add(kvp.Key.Name, elementType.FamilyName);
                }
            }
            typeListBox.Items.Clear();
            IOrderedEnumerable<KeyValuePair<string, string>> typeDic2 = typeDic.OrderBy(selector => { return selector.Key; });
            foreach (KeyValuePair<string, string> kvp in typeDic2) {
                if (kvp.Value == selectFamily) {
                    typeListBox.Items.Add(kvp.Key);
                }
            }
            typeListBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 配置方法X変更
        /// </summary>
        private void SetMethodX_Change(object sender, System.EventArgs e)
        {
            // 活性非活性制御
            if (countSetRadioX.Checked) {
                countComboX.Enabled = true;
                minIntervalX.Enabled = false;
                maxIntervalX.Enabled = false;
                Preview_Change(sender, e);
            }
            else {
                countComboX.Enabled = false;
                minIntervalX.Enabled = true;
                maxIntervalX.Enabled = true;
                Preview_Change(sender, e);
            }
        }

        /// <summary>
        /// 配置方法Y変更
        /// </summary>
        private void SetMethodY_Change(object sender, System.EventArgs e)
        {
            // 活性非活性制御
            if (countSetRadioY.Checked) {
                countComboY.Enabled = true;
                minIntervalY.Enabled = false;
                maxIntervalY.Enabled = false;
                Preview_Change(sender, e);
            }
            else {
                countComboY.Enabled = false;
                minIntervalY.Enabled = true;
                maxIntervalY.Enabled = true;
                Preview_Change(sender, e);
            }
        }

        /// <summary>
        /// プレビューチェンジ
        /// </summary>
        private void Preview_Change(object sender, System.EventArgs e)
        {
            // 活性非活性制御
            bool enableFlag = true;
            if (objectRadio.Checked) {
                pickObjectButton.Enabled = true;
                pickRegionButton.Enabled = false;
                if (objectLabel.Text == Resources.Text.LabelNotSelected) {
                    enableFlag = false;
                }
            }
            else {
                pickObjectButton.Enabled = false;
                pickRegionButton.Enabled = true;
                if (regionLabel.Text == Resources.Text.LabelNotSpecified) {
                    enableFlag = false;
                }
            }
            if (countSetRadioX.Checked) {
                if (countComboX.Text == "") {
                    enableFlag = false;
                }
                else if (countComboX.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalX.Text == "" && maxIntervalX.Text == "") {
                    enableFlag = false;
                }
            }
            if (countSetRadioY.Checked) {
                if (countComboY.Text == "") {
                    enableFlag = false;
                }
                else if (countComboY.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalY.Text == "" && maxIntervalY.Text == "") {
                    enableFlag = false;
                }
            }
            if (!int.TryParse(familyAngleCombo.Text, out int fa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(axisAngleCombo.Text, out int aa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(leftMarginText.Text, out int lm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(rightMarginText.Text, out int rm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(frontMarginText.Text, out int fm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(backMarginText.Text, out int bm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(offsetText.Text, out int ot)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(minIntervalX.Text, out int minX)) {
                enableFlag = false;
            }
            else if (minIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(minIntervalY.Text, out int minY)) {
                enableFlag = false;
            }
            else if (maxIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(maxIntervalX.Text, out int maxX)) {
                enableFlag = false;
            }
            else if (maxIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(maxIntervalY.Text, out int maxY)) {
                enableFlag = false;
            }
            if (enableFlag) {
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
            // ペイント
            Control panel = previewPanel;
            PaintEventArgs pe = new PaintEventArgs(panel.CreateGraphics(), panel.ClientRectangle);
            PaintEvent(sender, pe);
        }

        /// <summary>
        /// 2点指定
        /// </summary>
        private void Point_Pick(object sender, System.EventArgs e)
        {
            // コマンド側で2点指定を実行
            _isSelectObject = true;
            _isObject = false;
            _isRegion = false;
            _isAngle = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// 終了ボタンクリック
        /// </summary>
        private void Cancel_Button(object sender, System.EventArgs e)
        {
            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                tran.Start();
                if (detailLine != null) {
                    Doc.Delete(detailLine.Id);
                    detailLine = null;
                }
                tran.Commit();
            }
            Close();
        }

        /// <summary>
        /// ペイントイベント
        /// </summary>
        private void PaintEvent(object sender, PaintEventArgs e)
        {
            bool enableFlag = true;
            if (objectRadio.Checked) {
                pickObjectButton.Enabled = true;
                pickRegionButton.Enabled = false;
                if (objectLabel.Text == Resources.Text.LabelNotSelected) {
                    enableFlag = false;
                }
            }
            else {
                pickObjectButton.Enabled = false;
                pickRegionButton.Enabled = true;
                if (regionLabel.Text == Resources.Text.LabelNotSpecified) {
                    enableFlag = false;
                }
            }
            if (countSetRadioX.Checked) {
                if (countComboX.Text == "") {
                    enableFlag = false;
                }
                else if (countComboX.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalX.Text == "" && maxIntervalX.Text == "") {
                    enableFlag = false;
                }
            }
            if (countSetRadioY.Checked) {
                if (countComboY.Text == "") {
                    enableFlag = false;
                }
                else if (countComboY.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalY.Text == "" && maxIntervalY.Text == "") {
                    enableFlag = false;
                }
            }
            if (!int.TryParse(familyAngleCombo.Text, out int fa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(axisAngleCombo.Text, out int aa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(leftMarginText.Text, out int lm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(rightMarginText.Text, out int rm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(frontMarginText.Text, out int fm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(backMarginText.Text, out int bm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(offsetText.Text, out int ot)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(minIntervalX.Text, out int minX)) {
                enableFlag = false;
            }
            else if (minIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(minIntervalY.Text, out int minY)) {
                enableFlag = false;
            }
            else if (maxIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(maxIntervalX.Text, out int maxX)) {
                enableFlag = false;
            }
            else if (maxIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(maxIntervalY.Text, out int maxY)) {
                enableFlag = false;
            }
            if (enableFlag) {
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
            bool minasFlag = false;
            if (selectElement == null && pickedBox == null) {
                return;
            }
            List<PointF> pointList = new List<PointF>();
            // ビュー
            View activeView = UiDoc.ActiveView;
            // X方向メッセージフラグ
            bool xMesFlag = false;
            // X方向メッセージフラグ
            bool yMesFlag = false;

            Level selectLevel = activeView.GenLevel;
            // オフセット
            double.TryParse(offsetText.Text, out double offset);
            offset = UnitUtils.Convert(offset, UnitTypeId.Millimeters, UnitTypeId.Feet);
            // 配置ファミリの取得
            string selectCategory = categoryListBox.SelectedItem.ToString();
            string selectFamily = familyListBox.SelectedItem.ToString();
            string selectType = typeListBox.SelectedItem.ToString();
            FamilySymbol setSymbol = null;
            foreach (Element element in elementList) {
                ElementType elementType = (ElementType)element;
                if (element.Category.Name == selectCategory && elementType.FamilyName == selectFamily && element.Name == selectType) {
                    setSymbol = (FamilySymbol)element;
                }
            }
            // ファミリ角度
            if (double.TryParse(familyAngleCombo.Text, out double fAngle)) {
                fAngle = double.Parse(familyAngleCombo.Text);
            }
            // 頂点
            HashSet<XYZ> pointSet = new HashSet<XYZ>();
            if (selectElement != null && objectRadio.Checked && selectElement.GetType().Name == "Room") {
                Room room = (Room)selectElement;
                SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings settings = new ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings(UiDoc);
                opt.SpatialElementBoundaryLocation = settings.GetRoomAreaComputation();
                IList<IList<BoundarySegment>> segList = room.GetBoundarySegments(opt);
                foreach (IList<BoundarySegment> list in segList) {
                    foreach (BoundarySegment seg in list) {
                        bool zeroFlag = true;
                        bool oneFlag = true;
                        Curve curve = seg.GetCurve();
                        if (pointSet.Count > 0) {
                            foreach (XYZ point in pointSet) {
                                double zeroX = Math.Round(curve.GetEndPoint(0).X, 3, MidpointRounding.AwayFromZero);
                                double zeroY = Math.Round(curve.GetEndPoint(0).Y, 3, MidpointRounding.AwayFromZero);
                                double zeroZ = Math.Round(curve.GetEndPoint(0).Z, 3, MidpointRounding.AwayFromZero);
                                double oneX = Math.Round(curve.GetEndPoint(1).X, 3, MidpointRounding.AwayFromZero);
                                double oneY = Math.Round(curve.GetEndPoint(1).Y, 3, MidpointRounding.AwayFromZero);
                                double oneZ = Math.Round(curve.GetEndPoint(1).Z, 3, MidpointRounding.AwayFromZero);
                                double pointX = Math.Round(point.X, 3, MidpointRounding.AwayFromZero);
                                double pointY = Math.Round(point.Y, 3, MidpointRounding.AwayFromZero);
                                double pointZ = Math.Round(point.Z, 3, MidpointRounding.AwayFromZero);
                                if (pointX == zeroX
                                    && pointY == zeroY && pointZ == zeroZ) {
                                    zeroFlag = false;
                                }
                                if (pointX == oneX
                                    && pointY == oneY && pointZ == oneZ) {
                                    oneFlag = false;
                                }
                            }
                        }
                        if (zeroFlag) {
                            pointSet.Add(curve.GetEndPoint(0));
                        }
                        if (oneFlag) {
                            pointSet.Add(curve.GetEndPoint(1));
                        }
                    }
                }
            }
            else if (selectElement != null && objectRadio.Checked && selectElement.GetType().Name == "Space") {
                Space space = (Space)selectElement;
                SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings settings = new ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings(UiDoc);
                opt.SpatialElementBoundaryLocation = settings.GetRoomAreaComputation();
                IList<IList<BoundarySegment>> segList = space.GetBoundarySegments(opt);
                foreach (IList<BoundarySegment> list in segList) {
                    foreach (BoundarySegment seg in list) {
                        bool zeroFlag = true;
                        bool oneFlag = true;
                        Curve curve = seg.GetCurve();
                        if (pointSet.Count > 0) {
                            foreach (XYZ point in pointSet) {
                                double zeroX = Math.Round(curve.GetEndPoint(0).X, 3, MidpointRounding.AwayFromZero);
                                double zeroY = Math.Round(curve.GetEndPoint(0).Y, 3, MidpointRounding.AwayFromZero);
                                double zeroZ = Math.Round(curve.GetEndPoint(0).Z, 3, MidpointRounding.AwayFromZero);
                                double oneX = Math.Round(curve.GetEndPoint(1).X, 3, MidpointRounding.AwayFromZero);
                                double oneY = Math.Round(curve.GetEndPoint(1).Y, 3, MidpointRounding.AwayFromZero);
                                double oneZ = Math.Round(curve.GetEndPoint(1).Z, 3, MidpointRounding.AwayFromZero);
                                double pointX = Math.Round(point.X, 3, MidpointRounding.AwayFromZero);
                                double pointY = Math.Round(point.Y, 3, MidpointRounding.AwayFromZero);
                                double pointZ = Math.Round(point.Z, 3, MidpointRounding.AwayFromZero);
                                if (pointX == zeroX
                                    && pointY == zeroY && pointZ == zeroZ) {
                                    zeroFlag = false;
                                }
                                if (pointX == oneX
                                    && pointY == oneY && pointZ == oneZ) {
                                    oneFlag = false;
                                }
                            }
                        }
                        if (zeroFlag) {
                            pointSet.Add(curve.GetEndPoint(0));
                        }
                        if (oneFlag) {
                            pointSet.Add(curve.GetEndPoint(1));
                        }
                    }
                }
            }
            // 配置基準点
            XYZ setPoint = new XYZ(0, 0, 0);
            // 頂点リスト
            List<XYZ> vertexList = new List<XYZ>();
            // 傾きのない矩形フラグ
            bool rectFlag = false;
            // 長さ
            double lengthX = 0;
            double lengthY = 0;
            BoundingBoxXYZ box = null;
            // 選択オブジェクトラジオボタンを選択
            if (objectRadio.Checked && selectElement != null) {
                //　バインディングBOX
                box = selectElement.get_BoundingBox(UiDoc.ActiveView);
                lengthX = box.Max.X - box.Min.X;
                lengthY = box.Max.Y - box.Min.Y;
                if (pointSet.Count != 4) {
                    box = selectElement.get_BoundingBox(UiDoc.ActiveView);
                    lengthX = box.Max.X - box.Min.X;
                    lengthY = box.Max.Y - box.Min.Y;
                    XYZ xyz1 = new XYZ(box.Min.X, box.Min.Y, 0);
                    XYZ xyz2 = new XYZ(box.Min.X, box.Max.Y, 0);
                    XYZ xyz3 = new XYZ(box.Max.X, box.Min.Y, 0);
                    XYZ xyz4 = new XYZ(box.Max.X, box.Max.Y, 0);
                    vertexList.Add(xyz1);
                    vertexList.Add(xyz2);
                    vertexList.Add(xyz3);
                    vertexList.Add(xyz4);
                    rectFlag = true;
                }
                else {
                    foreach (XYZ point in pointSet) {
                        vertexList.Add(point);
                    }
                    vertexList.Sort((a, b) => Math.Sign(a.X - b.X));
                    double x0 = Math.Round(vertexList[0].X, 3, MidpointRounding.AwayFromZero);
                    double x1 = Math.Round(vertexList[1].X, 3, MidpointRounding.AwayFromZero);
                    double y0 = Math.Round(vertexList[0].Y, 3, MidpointRounding.AwayFromZero);
                    double y2 = Math.Round(vertexList[2].Y, 3, MidpointRounding.AwayFromZero);
                    double y3 = Math.Round(vertexList[3].Y, 3, MidpointRounding.AwayFromZero);
                    double x2 = Math.Round(vertexList[2].X, 3, MidpointRounding.AwayFromZero);
                    double x3 = Math.Round(vertexList[3].X, 3, MidpointRounding.AwayFromZero);
                    if (x0 == x1 && (y0 == y2 || y0 == 3) && x2 == x3) {
                        rectFlag = true;
                    }
                }
            }
            // 領域指定ラジオボタンを選択
            else if (regionRadio.Checked && pickedBox != null) {
                XYZ min = pickedBox.Min;
                XYZ max = pickedBox.Max;
                double maxX = max.X > min.X ? max.X : min.X;
                double minX = max.X > min.X ? min.X : max.X;
                double maxY = max.Y > min.Y ? max.Y : min.Y;
                double minY = max.Y > min.Y ? min.Y : max.Y;
                box = new BoundingBoxXYZ();
                box.Max = new XYZ(maxX, maxY, max.Z);
                box.Min = new XYZ(minX, minY, min.Z);
                XYZ xyz1 = new XYZ(box.Min.X, box.Min.Y, 0);
                XYZ xyz2 = new XYZ(box.Min.X, box.Max.Y, 0);
                XYZ xyz3 = new XYZ(box.Max.X, box.Min.Y, 0);
                XYZ xyz4 = new XYZ(box.Max.X, box.Max.Y, 0);
                vertexList.Add(xyz1);
                vertexList.Add(xyz2);
                vertexList.Add(xyz3);
                vertexList.Add(xyz4);
                rectFlag = true;
            }
            if (vertexList.Count == 0) {
                KeyValuePair<PointF, PointF>[] ps = { };
                KeyValuePair<PointF, PointF>[] ds = { };
                PointF[] p = pointList.ToArray();
                Control pane = previewPanel;
                Image image = Properties.Resources.arrow_in;
                PaintEventArgs pa = new PaintEventArgs(pane.CreateGraphics(), pane.ClientRectangle);
                Execute(pane, pa, ds, p, image, -fAngle, ps);
                return;
            }
            // 頂点
            XYZ dLeftBottom;
            XYZ dLeftTop;
            XYZ dRightBottom;
            XYZ dRightTop;
            // 頂点
            XYZ leftBottom;
            XYZ leftTop;
            XYZ rightBottom;
            XYZ rightTop;
            if (vertexList[0].Y < vertexList[1].Y) {
                leftBottom = vertexList[0];
                leftTop = vertexList[1];
            }
            else {
                leftBottom = vertexList[1];
                leftTop = vertexList[0];
            }
            if (vertexList[2].Y < vertexList[3].Y) {
                rightBottom = vertexList[2];
                rightTop = vertexList[3];
            }
            else {
                rightBottom = vertexList[3];
                rightTop = vertexList[2];
            }
            // デフォルト
            dLeftBottom = leftBottom;
            dLeftTop = leftTop;
            dRightBottom = rightBottom;
            dRightTop = rightTop;
            // マージン
            double.TryParse(leftMarginText.Text, out double leftMargin);
            leftMargin = UnitUtils.Convert(leftMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(rightMarginText.Text, out double rightMargin);
            rightMargin = UnitUtils.Convert(rightMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(frontMarginText.Text, out double frontMargin);
            frontMargin = UnitUtils.Convert(frontMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(backMarginText.Text, out double backMargin);
            backMargin = UnitUtils.Convert(backMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            if (rectFlag) {
                // 傾いていない場合
                leftBottom = new XYZ(leftBottom.X + leftMargin, leftBottom.Y + frontMargin, leftBottom.Z);
                leftTop = new XYZ(leftTop.X + leftMargin, leftTop.Y - backMargin, leftTop.Z);
                rightBottom = new XYZ(rightBottom.X - rightMargin, rightBottom.Y + frontMargin, rightBottom.Z);
                rightTop = new XYZ(rightTop.X - rightMargin, rightTop.Y - backMargin, leftTop.Z);
                lengthX = rightBottom.X - leftBottom.X;
                lengthY = leftTop.Y - leftBottom.Y;
                setPoint = leftBottom;
            }
            else {
                // 傾いている場合
                XYZ zero = new XYZ(1, 0, 0);
                // 左下について
                // 左下および右下のなす角度
                double rad = zero.AngleTo(new XYZ(rightBottom.X - leftBottom.X, rightBottom.Y - leftBottom.Y, 0));
                double angle = rad / Math.PI * 180;
                double mAngle = 180 - 90 - angle;
                double mRad = mAngle * Math.PI / 180;
                // 左マージン
                double xLeftMargin = Math.Cos(rad) * leftMargin;
                double yLeftMargin = Math.Sin(rad) * leftMargin;
                // 前マージン
                double xFrontMargin = Math.Cos(mRad) * frontMargin;
                double yFrontMargin = Math.Sin(mRad) * frontMargin;
                // 右マージン
                double xRightMargin = Math.Cos(rad) * rightMargin;
                double yRightMargin = Math.Sin(rad) * rightMargin;
                // 後マージン
                double xBackMargin = Math.Cos(mRad) * backMargin;
                double yBackMargin = Math.Sin(mRad) * backMargin;
                if (leftBottom.Y < rightBottom.Y) {
                    // 座標移動
                    leftBottom = new XYZ(leftBottom.X + xLeftMargin - xFrontMargin, leftBottom.Y + yLeftMargin + yFrontMargin, leftBottom.Z);
                    leftTop = new XYZ(leftTop.X + xLeftMargin + xBackMargin, leftTop.Y + yLeftMargin - yBackMargin, leftTop.Z);
                    rightBottom = new XYZ(rightBottom.X - xRightMargin - xFrontMargin, rightBottom.Y - yRightMargin + yFrontMargin, rightBottom.Z);
                    rightTop = new XYZ(rightTop.X - xRightMargin + xBackMargin, rightTop.Y - yRightMargin - yBackMargin, rightTop.Z);
                }
                else {
                    // 座標移動
                    leftBottom = new XYZ(leftBottom.X + xLeftMargin + xFrontMargin, leftBottom.Y - yLeftMargin + yFrontMargin, leftBottom.Z);
                    leftTop = new XYZ(leftTop.X + xLeftMargin - xBackMargin, leftTop.Y - yLeftMargin - yBackMargin, leftTop.Z);
                    rightBottom = new XYZ(rightBottom.X - xRightMargin + xFrontMargin, rightBottom.Y + yRightMargin + yFrontMargin, rightBottom.Z);
                    rightTop = new XYZ(rightTop.X - xRightMargin - xBackMargin, rightTop.Y + yRightMargin - yBackMargin, rightTop.Z);
                }
            }

            // ラジアン
            double setRad = 0;
            // 配置軸
            if (double.TryParse(axisAngleCombo.Text, out double setAngle)) {
                setAngle = double.Parse(axisAngleCombo.Text);
            }
            if (setAngle != 0) {
                if (setAngle == 90) {
                    setAngle = 89.999;
                }
                if (setAngle == -90) {
                    setAngle = -89.999;
                }
                // 傾き
                setRad = setAngle * Math.PI / 180;
                double tilt = Math.Tan(setRad);
                double mTilt = -(1 / tilt);
                double mTilt2 = tilt + (1 / tilt);
                // 左下について
                double interceptLeftBottom = 0;
                // 左上について
                double interceptLeftTop = 0;
                // 右下について
                double interceptRightBottom = 0;
                // 右上について
                double interceptRightTop = 0;
                // 左下および左上についての交点
                XYZ leftIntersection1 = new XYZ(0, 0, 0);

                // 左下および右下についての交点
                XYZ leftIntersection2 = new XYZ(0, 0, 0);

                // 右下および右上についての交点
                XYZ rightIntersection1 = new XYZ(0, 0, 0);

                // 右上および左上についての交点
                XYZ rightIntersection2 = new XYZ(0, 0, 0);
                if (Math.Round(leftBottom.Y, 3, MidpointRounding.AwayFromZero) == Math.Round(rightBottom.Y, 3, MidpointRounding.AwayFromZero)) {
                    // 各頂点を通る直線の切片
                    if (tilt > 0) {
                        // 左下について
                        interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                        // 左上について
                        interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                        // 右下について
                        interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                        // 右上について
                        interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                        // 左下および左上についての交点
                        double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                        double y1 = tilt * x1 + interceptLeftTop;
                        leftIntersection1 = new XYZ(x1, y1, 0);
                        // 左下および右下についての交点
                        double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                        double y2 = tilt * x2 + interceptRightBottom;
                        leftIntersection2 = new XYZ(x2, y2, 0);
                        setPoint = leftIntersection2;
                        // 右下および右上についての交点
                        double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                        double y3 = tilt * x3 + interceptRightBottom;
                        rightIntersection1 = new XYZ(x3, y3, 0);
                        // 右上および左上についての交点
                        double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                        double y4 = tilt * x4 + interceptLeftTop;
                        rightIntersection2 = new XYZ(x4, y4, 0);
                        // 交点間の長さ
                        double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                        double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                        lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                        double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                        double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                        lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                    }
                    else if (tilt < 0) {
                        // 左下について
                        interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                        // 左上について
                        interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                        // 右下について
                        interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                        // 右上について
                        interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                        // 左下および左上についての交点
                        double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                        double y1 = mTilt * x1 + interceptLeftTop;
                        leftIntersection1 = new XYZ(x1, y1, 0);
                        setPoint = leftIntersection1;
                        // 左下および右下についての交点
                        double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                        double y2 = mTilt * x2 + interceptRightBottom;
                        leftIntersection2 = new XYZ(x2, y2, 0);
                        // 右下および右上についての交点
                        double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                        double y3 = mTilt * x3 + interceptRightBottom;
                        rightIntersection1 = new XYZ(x3, y3, 0);
                        // 右上および左上についての交点
                        double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                        double y4 = mTilt * x4 + interceptLeftTop;
                        rightIntersection2 = new XYZ(x4, y4, 0);
                        // 交点間の長さ
                        double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                        double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                        lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                        double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                        double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                        lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                    }
                }
                else if (leftBottom.Y < rightBottom.Y) {
                    if (tilt > 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180;
                        if (angle2 >= setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                    else if (tilt < 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180;
                        double angle3 = 90 - angle2;
                        double setAngle2 = setAngle * -1;
                        if (angle3 >= setAngle2) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);

                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            setPoint = rightIntersection2;
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                }
                else {
                    if (tilt > 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = 90 - (rad2 / Math.PI * 180);
                        if (angle2 < setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            setPoint = rightIntersection1;
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                    else if (tilt < 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180 * -1;
                        if (angle2 >= setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                }
            }
            else if (setAngle == 0) {
                if (!rectFlag) {
                    List<double> xList = new List<double>();
                    xList.Add(leftBottom.X);
                    xList.Add(leftTop.X);
                    xList.Add(rightBottom.X);
                    xList.Add(rightTop.X);
                    List<double> yList = new List<double>();
                    yList.Add(leftBottom.Y);
                    yList.Add(leftTop.Y);
                    yList.Add(rightBottom.Y);
                    yList.Add(rightTop.Y);

                    XYZ mLeftBottom = new XYZ(xList.Min(), yList.Min(), 0);
                    XYZ mLeftTop = new XYZ(xList.Min(), yList.Max(), 0);
                    XYZ mRightBottom = new XYZ(xList.Max(), yList.Min(), 0);
                    XYZ mRightTop = new XYZ(xList.Max(), yList.Max(), 0);

                    lengthX = mRightBottom.X - mLeftBottom.X;
                    lengthY = mLeftTop.Y - mLeftBottom.Y;
                    setPoint = mLeftBottom;
                }
            }
            // 配置設定取得
            double bottomX = 0;
            double heightX = 0;
            double bottomY = 0;
            double heightY = 0;
            double increaseX = 0;
            double increaseY = 0;
            if (countSetRadioX.Checked) {
                int.TryParse(countComboX.Text.ToString(), out int countX);
                if (countX > 400) {
                    countX = 0;
                }
                double mLengthX = 0;
                double intervalX = 0;
                if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countX > 0) {
                    mLengthX = lengthX / (countX + 1);
                }
                else if (countX > 0) {
                    mLengthX = lengthX / countX;
                    intervalX = setPoint.X + (mLengthX / 2);
                }
                if (countSetRadioY.Checked) {
                    int.TryParse(countComboY.Text.ToString(), out int countY);
                    if (countY > 400) {
                        countY = 0;
                    }
                    double mLengthY = 0;
                    double intervalY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countY > 0) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else if (countY > 0) {
                        mLengthY = lengthY / countY;
                        intervalY = box.Min.Y + (mLengthY / 2);
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    if (countX * countY > 400) {
                        countX = 0;
                        countY = 0;
                    }
                    if (minasFlag) {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                }
                // Yが間隔指定配置
                else {
                    double intervalY = 0;
                    int countY = 0;
                    double widthY = lengthY;
                    // 入力値チェック
                    int.TryParse(minIntervalY.Text, out int minY);
                    int.TryParse(maxIntervalY.Text, out int maxY);
                    double milliY = UnitUtils.Convert(widthY, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    // チェックフラグ
                    bool checkFlag = false;
                    if (minY != 0 && maxY == 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                        }
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                        } while (intervalY >= mMinY);
                        countY--;
                    }
                    else if (minY == 0 && maxY != 0) {
                        countY = 1;
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                        } while (intervalY > mMaxY);
                    }
                    else if (minY != 0 && maxY != 0) {
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                        }
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                            if ((intervalY <= mMaxY && intervalY >= mMinY)) {
                                checkFlag = true;
                            }
                        } while (intervalY > mMaxY || (intervalY <= mMaxY && intervalY >= mMinY));
                        countY--;
                        if (!checkFlag) {
                            countY = 0;
                        }
                    }
                    if (yMesFlag) {
                        countY = 0;
                    }
                    countY--;
                    if (countY < 0) {
                        countY = 0;
                    }
                    double mLengthY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countY > 0) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else if (countY > 0) {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    if (countX * countY > 400) {
                        countX = 0;
                        countY = 0;
                    }
                    if (minasFlag) {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                }
            }
            // Xが間隔指定配置
            else {
                double intervalX = 0;
                int countX = 0;
                double widthX = lengthX;
                double milliX = UnitUtils.Convert(widthX, UnitTypeId.Feet, UnitTypeId.Millimeters);
                // 入力値チェック
                int.TryParse(minIntervalX.Text, out int minX);
                int.TryParse(maxIntervalX.Text, out int maxX);
                // チェックフラグ
                bool checkFlag = false;
                if (minX != 0 && maxX == 0) {
                    if (milliX < minX * 2) {
                        xMesFlag = true;
                    }
                    countX = 1;
                    double mMinX = UnitUtils.Convert(minX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    intervalX = 0;
                    do {
                        countX++;
                        if (countX > 400) {
                            countX = 0;
                            break;
                        }
                        intervalX = widthX / countX;
                    } while (intervalX >= mMinX);
                    countX--;
                }
                else if (minX == 0 && maxX != 0) {
                    countX = 1;
                    double mMaxX = UnitUtils.Convert(maxX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    intervalX = 0;
                    do {
                        countX++;
                        if (countX > 400) {
                            countX = 0;
                            break;
                        }
                        intervalX = widthX / countX;
                    } while (intervalX > mMaxX);
                }
                else if (minX != 0 && maxX != 0) {
                    if (milliX < minX * 2) {
                        xMesFlag = true;
                    }
                    countX = 1;
                    double mMinX = UnitUtils.Convert(minX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    double mMaxX = UnitUtils.Convert(maxX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    intervalX = 0;
                    do {
                        countX++;
                        if (countX > 400) {
                            countX = 0;
                            break;
                        }
                        intervalX = widthX / countX;
                        if ((intervalX <= mMaxX && intervalX >= mMinX)) {
                            checkFlag = true;
                        }
                    } while (intervalX > mMaxX || (intervalX <= mMaxX && intervalX >= mMinX));
                    countX--;
                    if (!checkFlag) {
                        countX = 0;
                    }
                }
                if (xMesFlag) {
                    countX = 0;
                }
                countX--;
                if (countX < 0) {
                    countX = 0;
                }
                double mLengthX = 0;
                if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countX > 0) {
                    mLengthX = lengthX / (countX + 1);
                    intervalX = box.Min.X + mLengthX;
                }
                else if (countX > 0) {
                    mLengthX = lengthX / countX;
                    intervalX = box.Min.X + (mLengthX / 2);
                }

                if (countSetRadioY.Checked) {
                    int.TryParse(countComboY.Text.ToString(), out int countY);
                    if (countY > 400) {
                        countY = 0;
                    }
                    double mLengthY = 0;
                    double intervalY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countY > 0) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else if (countY > 0) {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    if (countX * countY > 400) {
                        countX = 0;
                        countY = 0;
                    }
                    if (minasFlag) {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                }
                // Yが間隔指定配置
                else {
                    double intervalY = 0;
                    int countY = 0;
                    double widthY = lengthY;
                    double milliY = UnitUtils.Convert(widthY, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    // 入力値チェック
                    int.TryParse(minIntervalY.Text, out int minY);
                    int.TryParse(maxIntervalY.Text, out int maxY);
                    // チェックフラグ
                    checkFlag = false;
                    if (minY != 0 && maxY == 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                        }
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                        } while (intervalY >= mMinY);
                        countY--;
                    }
                    else if (minY == 0 && maxY != 0) {
                        countY = 1;
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                        } while (intervalY > mMaxY);
                    }
                    else if (minY != 0 && maxY != 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                        }
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            if (countY > 400) {
                                countY = 0;
                                break;
                            }
                            intervalY = widthY / countY;
                            if ((intervalY <= mMaxY && intervalY >= mMinY)) {
                                checkFlag = true;
                            }
                        } while (intervalY > mMaxY || (intervalY <= mMaxY && intervalY >= mMinY));
                        countY--;
                        if (!checkFlag) {
                            countY = 0;
                        }
                    }
                    if (yMesFlag) {
                        countY = 0;
                    }
                    countY--;
                    if (countY < 0) {
                        countY = 0;
                    }
                    double mLengthY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing && countY > 0) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else if (countY > 0) {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    if (countX * countY > 400) {
                        countX = 0;
                        countY = 0;
                    }
                    if (minasFlag) {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < countX; i++) {
                            for (int j = 0; j < countY; j++) {
                                XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                            }
                        }
                    }
                }
            }
            PointF d1 = new PointF((float)dLeftBottom.X, (float)dLeftBottom.Y);
            PointF d2 = new PointF((float)dLeftTop.X, (float)dLeftTop.Y);
            PointF d3 = new PointF((float)dRightTop.X, (float)dRightTop.Y);
            PointF d4 = new PointF((float)dRightBottom.X, (float)dRightBottom.Y);
            PointF p1 = new PointF((float)leftBottom.X, (float)leftBottom.Y);
            PointF p2 = new PointF((float)leftTop.X, (float)leftTop.Y);
            PointF p3 = new PointF((float)rightTop.X, (float)rightTop.Y);
            PointF p4 = new PointF((float)rightBottom.X, (float)rightBottom.Y);
            KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
            KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
            KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
            KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
            KeyValuePair<PointF, PointF> dPare1 = new KeyValuePair<PointF, PointF>(d1, d2);
            KeyValuePair<PointF, PointF> dPare2 = new KeyValuePair<PointF, PointF>(d2, d3);
            KeyValuePair<PointF, PointF> dPare3 = new KeyValuePair<PointF, PointF>(d3, d4);
            KeyValuePair<PointF, PointF> dPare4 = new KeyValuePair<PointF, PointF>(d4, d1);

            KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
            KeyValuePair<PointF, PointF>[] dPares = { dPare1, dPare2, dPare3, dPare4 };
            PointF[] points = pointList.ToArray();
            Control panel = previewPanel;
            Image imageIn = Properties.Resources.arrow_in;
            PaintEventArgs pe = new PaintEventArgs(panel.CreateGraphics(), panel.ClientRectangle);
            Execute(panel, pe, dPares, points, imageIn, -fAngle, pares);
        }

        /// <summary>
        /// 配置して終了ボタンクリック
        /// </summary>
        private void OkButton_Click(object sender, System.EventArgs e)
        {
            okFlag = true;
            SetInstance();
            if (okFlag) {
                Close();
            }
        }

        /// <summary>
        /// 配置ボタンクリック
        /// </summary>
        private void ApplyButton_Click(object sender, System.EventArgs e)
        {
            SetInstance();
            CmdLayoutInstance.transGroup.Assimilate();
            CmdLayoutInstance.transGroup.Start();
        }

        /// <summary>
        /// 角度の最大値
        /// </summary>
        private const int MaxDegree = 360;

        /// <summary>
        /// 余白
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public new int Margin { get; set; } = 8;

        /// <summary>
        /// 背景を塗るためのブラシ
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public SolidBrush BGBrush { get; set; } = new SolidBrush(Color.White);

        /// <summary>
        /// 範囲線を描画するためのペン
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Pen LinePen { get; set; } = new Pen(Color.Black);

        /// <summary>
        /// 点線用のペン
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Pen DotPen { get; set; } = new Pen(Color.Black);

        /// <summary>
        /// 描画を実行する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        /// <param name="lines"></param>
        /// <param name="points"></param>
        /// <param name="iconInside"></param>
        /// <param name="iconOutside"></param>
        /// <param name="iconAngle"></param>
        public void Execute(Control control, PaintEventArgs e, KeyValuePair<PointF, PointF>[] lines, PointF[] points, Image iconInside, double iconAngle, KeyValuePair<PointF, PointF>[] marginLines)
        {
            Execute(control, e, lines.Select(line => new LineData(line)).ToArray(), points.Select(p => new PointData(p)).ToArray(), iconInside, iconAngle, marginLines.Select(marginLine => new LineData(marginLine)).ToArray());
        }

        /// <summary>
        /// 描画を実行する
        /// </summary>
        /// <param control="e">コントロール</param>
        /// <param name="e">OnPaintのEventArgs</param>
        /// <param name="lines">範囲を示すための線</param>
        /// <param name="points">座標点</param>
        /// <param name="iconInside">範囲内の点を描画するためのアイコン</param>
        /// <param name="iconOutside">範囲外の点を描画するためのアイコン</param>
        /// <param name="iconAngle">点の角度</param>
        public void Execute(Control control, PaintEventArgs e, Line[] lines, XYZ[] points, Image iconInside, double iconAngle, Line[] marginLines)
        {
            Execute(control, e, lines.Select(line => new LineData(line)).ToArray(), points.Select(p => new PointData(p)).ToArray(), iconInside, iconAngle, marginLines.Select(marginLine => new LineData(marginLine)).ToArray());
        }

        /// <summary>
        /// 描画の実行
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        /// <param name="lines"></param>
        /// <param name="points"></param>
        /// <param name="iconInside"></param>
        /// <param name="iconOutside"></param>
        /// <param name="iconAngle"></param>
        private void Execute(Control control, PaintEventArgs e, LineData[] lines, PointData[] points, Image iconInside, double iconAngle, LineData[] marginLines)
        {
            //更新停止
            control.SuspendLayout();
            //背景色で塗りつぶす
            e.Graphics.FillRectangle(BGBrush, e.ClipRectangle);
            //線が存在しない場合は描画しない
            if (marginLines == null || marginLines.Length == 0) {
                control.ResumeLayout();
                return;
            }
            //最大・最小の値を計算する
            double minX, minY, maxX, maxY;
            minX = minY = double.MaxValue;
            maxX = maxY = double.MinValue;
            //各範囲線
            foreach (LineData line in lines) {
                //開始座標と終了座標を取得
                CalculateMinMax(line.Begin, ref minX, ref minY, ref maxX, ref maxY);
                CalculateMinMax(line.End, ref minX, ref minY, ref maxX, ref maxY);
            }
            //配置場所が存在している場合は追加する
            if (points != null) {
                foreach (PointData point in points) {
                    CalculateMinMax(point, ref minX, ref minY, ref maxX, ref maxY);
                }
            }
            //Controlの描画範囲を計算する
            int cw = e.ClipRectangle.Width - Margin * 2;
            int ch = e.ClipRectangle.Height - Margin * 2;
            //長さを計算する
            double dx = (maxX - minX);
            double dy = (maxY - minY);
            //縦横の比率を計算する
            double rw = cw / dx;
            double rh = ch / dy;
            //より低い方を割合として正とする
            double rate = Math.Min(rw, rh);
            //マージンを計算する（中央に描画するようにする）
            int xMargin = Margin + (int)(cw - dx * rate) / 2;
            int yMargin = Margin + (int)(ch - dy * rate) / 2;
            //マージン線を引く
            foreach (LineData line in marginLines) {
                //開始地点と終了地点を取得する
                PointF begin = ConvertToPoint(line.Begin, minX, maxY, rate, xMargin, yMargin);
                PointF end = ConvertToPoint(line.End, minX, maxY, rate, xMargin, yMargin);
                DotPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawLine(DotPen, begin, end);
            }
            //デフォルト線を引く
            foreach (LineData line in lines) {
                //開始地点と終了地点を取得する
                PointF begin = ConvertToPoint(line.Begin, minX, maxY, rate, xMargin, yMargin);
                PointF end = ConvertToPoint(line.End, minX, maxY, rate, xMargin, yMargin);
                e.Graphics.DrawLine(LinePen, begin, end);
            }
            //配置ポイントの描画
            if (points != null) {
                //ラジアンに変換
                double radian = iconAngle * Math.PI / 180;
                float cos = (float)Math.Cos(radian);
                float sin = (float)Math.Sin(radian);
                float mx = -cos + sin;
                float my = -cos - sin;
                foreach (PointData point in points) {
                    //アイコンを決定
                    Image icon = iconInside;
                    //開始座標を計算
                    PointF p0 = ConvertToPoint(point, minX, maxY, rate, xMargin, yMargin);
                    p0.X += mx * icon.Width / 2;
                    p0.Y += my * icon.Height / 2;
                    //右上
                    float x1 = p0.X + icon.Width * cos;
                    float y1 = p0.Y + icon.Width * sin;
                    //左下
                    float x2 = p0.X - icon.Height * sin;
                    float y2 = p0.Y + icon.Height * cos;
                    //描画
                    e.Graphics.DrawImage(icon, new PointF[] { p0, new PointF(x1, y1), new PointF(x2, y2) });
                }
            }
            //描画を実行
            control.ResumeLayout();
        }

        /// <summary>
        /// 最小値と最大値の計算処理
        /// </summary>
        /// <param name="point"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        private void CalculateMinMax(PointData point, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        /// <summary>
        /// 座標をポイントに変換（二次元）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="minX"></param>
        /// <param name="maxY"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        private PointF ConvertToPoint(PointData point, double minX, double maxY, double rate, int xMargin, int yMargin)
        {
            float x = (float)((point.X - minX) * rate) + xMargin;
            float y = (float)((maxY - point.Y) * rate) + yMargin;
            return new PointF(x, y);
        }

        /// <summary>
        /// 範囲内に存在するか判定（二次元）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool IsRange(PointData point, params LineData[] lines)
        {
            //各線と点のなす角を合算する
            double degree = 0;
            foreach (LineData line in lines) {
                degree += GetDegree(point, line);
            }
            //合計して360度であった場合は範囲内に存在すると判断する
            return (int)Math.Round(degree) == MaxDegree;
        }

        /// <summary>
        /// ある点と線の頂点のなす角を求める（二次元）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private double GetDegree(PointData point, LineData line)
        {
            //判定するポイントからのベクトルに変換
            PointData v1 = line.Begin - point;
            PointData v2 = line.End - point;
            //cosを計算
            double cos = (v1.X * v2.X + v1.Y * v2.Y) / (Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y) * Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y));
            //degreeに変換
            return Math.Acos(cos) * 180 / Math.PI;
        }

        /// <summary>
        /// 線分情報
        /// </summary>
        private struct LineData
        {
            public PointData Begin;
            public PointData End;

            public LineData(Line line)
            {
                Begin = new PointData(line.Origin);
                End = new PointData(line.Origin + line.Direction * line.Length);
            }

            public LineData(KeyValuePair<PointF, PointF> line)
            {
                Begin = new PointData(line.Key);
                End = new PointData(line.Value);
            }
        }

        /// <summary>
        /// 点情報
        /// </summary>
        private struct PointData
        {
            public double X;
            public double Y;

            public PointData(PointF point)
            {
                X = point.X;
                Y = point.Y;
            }

            public PointData(XYZ point)
            {
                X = point.X;
                Y = point.Y;
            }

            public PointData(double x, double y)
            {
                X = x;
                Y = y;
            }

            public static PointData operator -(PointData p1, PointData p2)
            {
                return new PointData(p1.X - p2.X, p1.Y - p2.Y);
            }
        }

        /// <summary>
        /// Return all the "corner" vertices of a given solid
        /// by adding them to the dictionary passed in.
        /// Note that a circle in Revit consists of two arcs
        /// and will return a "corner" at each of the two arc
        /// end points.
        /// </summary>
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
        /// Return all the "corner" vertices of a given solid.
        /// Note that a circle in Revit consists of two arcs
        /// and will return a "corner" at each of the two arc
        /// end points.
        /// </summary>
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
        /// Define equality for Revit XYZ points.
        /// Very rough tolerance, as used by Revit itself.
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
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        private static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// 要素取得
        /// </summary>
        private List<Element> GetElement(Level level)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Doc);
            List<Element> fElementList = filteredElementCollector.WhereElementIsNotElementType().ToList();
            List<Element> returnList = new List<Element>();
            foreach (Element element in fElementList) {
                if (element.LevelId == level.Id && element.GetType().Name != "Room"
                    && element.GetType().Name != "Space" && element.GetType().Name != "Slab" && element.GetType().Name != "Ceiling") {
                    returnList.Add(element);
                }
            }
            return returnList;
        }

        /// <summary>
        /// 要素取得(天井、床)
        /// </summary>
        private List<Element> GetSlabCeiling(Level level)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Doc);
            List<Element> fElementList = filteredElementCollector.WhereElementIsNotElementType().ToList();
            List<Element> returnList = new List<Element>();
            foreach (Element element in fElementList) {
                if (element.LevelId == level.Id && (element.GetType().Name == "Floor" || element.LevelId == level.Id && element.GetType().Name == "Slab" || element.GetType().Name == "Ceiling")) {
                    returnList.Add(element);
                }
            }
            return returnList;
        }

        /// <summary>
        /// ホスト要素候補取得(XY)
        /// </summary>
        private List<Element> GetHostElementCandidateXY(XYZ setPoint, List<Element> elements, View activeView)
        {
            List<Element> cList = new List<Element>();
            foreach (Element element in elements) {
                BoundingBoxXYZ box = element.get_BoundingBox(activeView);
                if (box == null) {
                    continue;
                }
                XYZ max = box.Max;
                XYZ min = box.Min;
                PointF setP = new PointF((float)setPoint.X, (float)setPoint.Y);
                PointF p1 = new PointF((float)max.X, (float)max.Y);
                PointF p2 = new PointF((float)max.X, (float)min.Y);
                PointF p3 = new PointF((float)min.X, (float)min.Y);
                PointF p4 = new PointF((float)min.X, (float)max.Y);
                KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
                KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
                KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
                KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
                KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
                LineData[] lineData = pares.Select(line => new LineData(line)).ToArray();
                PointF[] points = { setP };
                PointData[] pointData = points.Select(p => new PointData(p)).ToArray();

                foreach (PointData point in pointData) {
                    if (IsRange(point, lineData)) {
                        cList.Add(element);
                    }
                }
            }

            return cList;
        }

        /// <summary>
        /// フェイス取得
        /// </summary>
        private Face GetFace(XYZ setPoint, List<Element> elements)
        {
            Face face = null;
            double distance = 1;
            double min_distance = 0.05;
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            foreach (Element element in elements) {
                if (element.GetType().Name == "FamilyInstance") {
                }
                else {
                    GeometryElement geo = element.get_Geometry(opt);
                    foreach (GeometryObject obj in geo) {
                        Solid solid = obj as Solid;
                        if (solid != null) {
                            FaceArray fa = solid.Faces;
                            foreach (Face f in fa) {
                                IList<CurveLoop> edgeCurve = f.GetEdgesAsCurveLoops();
                                XYZ xyz = f.ComputeNormal(new UV(0, 0));
                                if (Math.Abs(xyz.Z) != 0) {
                                    continue;
                                }
                                foreach (CurveLoop curveLoop in edgeCurve) {
                                    foreach (Curve curve in curveLoop) {
                                        try {
                                            double z = 0;
                                            if (curve.GetType().Name == "Arc") {
                                                Arc arc = curve as Arc;
                                                z = arc.GetEndPoint(0).Z;
                                            }
                                            else if (curve.GetType().Name == "Line") {
                                                Line line = curve as Line;
                                                z = line.GetEndPoint(0).Z;
                                            }
                                            setPoint = new XYZ(setPoint.X, setPoint.Y, z);
                                            distance = curve.Distance(setPoint);
                                        }
                                        catch (Exception ex) {
                                            continue;
                                        }
                                        if (distance < min_distance) {
                                            if (f.Reference == null) {
                                                continue;
                                            }
                                            face = f as Face;
                                            break;
                                        }
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
        /// フェイス取得(天井、床)
        /// </summary>
        private Face GetFaceSlabCeiling(XYZ setPoint, List<Element> elements, View activeView)
        {
            Face face = null;
            double distance = 1;
            double min_distance = double.MaxValue;
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            foreach (Element element in elements) {
                BoundingBoxXYZ box = element.get_BoundingBox(activeView);
                if (box.Min.Z > setPoint.Z || setPoint.Z > box.Max.Z) {
                    continue;
                }
                GeometryElement geo = element.get_Geometry(opt);
                foreach (GeometryObject obj in geo) {
                    Solid solid = obj as Solid;
                    if (solid != null) {
                        XYZ point1 = new XYZ(setPoint.X, setPoint.Y, setPoint.Z - 1);
                        XYZ point2 = new XYZ(setPoint.X, setPoint.Y, setPoint.Z + 1);
                        Line line = Line.CreateBound(point1, point2);
                        SolidCurveIntersectionOptions option = new SolidCurveIntersectionOptions();
                        SolidCurveIntersection section = solid.IntersectWithCurve(line, option);
                        if (section.SegmentCount == 0) {
                            continue;
                        }
                        FaceArray fa = solid.Faces;
                        foreach (Face f in fa) {
                            IList<CurveLoop> edgeCurve = f.GetEdgesAsCurveLoops();
                            foreach (CurveLoop curveLoop in edgeCurve) {
                                foreach (Curve curve in curveLoop) {
                                    XYZ xyz = f.ComputeNormal(new UV(0, 0));
                                    if (Math.Abs(xyz.X) != 0) {
                                        continue;
                                    }
                                    if (Math.Abs(xyz.Y) != 0) {
                                        continue;
                                    }
                                    try {
                                        distance = curve.Distance(setPoint);
                                    }
                                    catch (Exception ex) {
                                        continue;
                                    }

                                    if (distance < min_distance) {
                                        if (f.Reference == null) {
                                            continue;
                                        }
                                        min_distance = distance;
                                        face = f as Face;
                                        PlanarFace pf = (PlanarFace)f;
                                        XYZ v = setPoint - pf.Origin;
                                        double d = v.DotProduct(-pf.FaceNormal);
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
        /// 数値のみ入力を許可する
        /// </summary>
        private void TextBoxPrice_PreviewTextInput(object sender, KeyPressEventArgs e)
        {
            //バックスペースが押された時は有効（Deleteキーも有効）
            if (e.KeyChar == '\b') {
                return;
            }
            //マイナスと小数点は有効
            if (e.KeyChar == '-') {
                return;
            }
            //数値0～9以外が押された時はイベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar)) {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 配置数入力制限
        /// </summary>
        private void Count_PreviewTextInput(object sender, KeyPressEventArgs e)
        {
            //バックスペースが押された時は有効（Deleteキーも有効）
            if (e.KeyChar == '\b') {
                return;
            }
            //数値0～9以外が押された時はイベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar)) {
                e.Handled = true;
            }
        }

        /// <summary>
        /// ファミリ角度の入力制限
        /// </summary>
        private void FamilyAngleTextBoxPrice_PreviewTextInput(object sender, KeyPressEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            //バックスペースが押された時は有効（Deleteキーも有効）
            if (e.KeyChar == '\b') {
                return;
            }
            //if (comboBox.Text.Count() == 0) {
            //マイナスと小数点は有効
            if (e.KeyChar == '-') {
                return;
            }
            //数値0～9以外が押された時はイベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar)) {
                e.Handled = true;
            }
        }

        /// <summary>
        /// テキストロストフォーカス時のイベント
        /// </summary>
        private void TextLeave(object sender, EventArgs e)
        {
            bool enableFlag = true;
            if (objectRadio.Checked) {
                pickObjectButton.Enabled = true;
                pickRegionButton.Enabled = false;
                if (objectLabel.Text == Resources.Text.LabelNotSelected) {
                    enableFlag = false;
                }
            }
            else {
                pickObjectButton.Enabled = false;
                pickRegionButton.Enabled = true;
                if (regionLabel.Text == Resources.Text.LabelNotSpecified) {
                    enableFlag = false;
                }
            }
            if (countSetRadioX.Checked) {
                if (countComboX.Text == "") {
                    enableFlag = false;
                }
                else if (countComboX.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalX.Text == "" && maxIntervalX.Text == "") {
                    enableFlag = false;
                }
            }
            if (countSetRadioY.Checked) {
                if (countComboY.Text == "") {
                    enableFlag = false;
                }
                else if (countComboY.Text == "0") {
                    enableFlag = false;
                }
            }
            else {
                if (minIntervalY.Text == "" && maxIntervalY.Text == "") {
                    enableFlag = false;
                }
            }
            if (!int.TryParse(familyAngleCombo.Text, out int fa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(axisAngleCombo.Text, out int aa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(leftMarginText.Text, out int lm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(rightMarginText.Text, out int rm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(frontMarginText.Text, out int fm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(backMarginText.Text, out int bm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(offsetText.Text, out int ot)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(minIntervalX.Text, out int minX)) {
                enableFlag = false;
            }
            else if (minIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(minIntervalY.Text, out int minY)) {
                enableFlag = false;
            }
            else if (maxIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(maxIntervalX.Text, out int maxX)) {
                enableFlag = false;
            }
            else if (maxIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(maxIntervalY.Text, out int maxY)) {
                enableFlag = false;
            }
            if (enableFlag) {
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
        }

        /// <summary>
        /// ファミリ角度の変更時の処理
        /// </summary>
        private void FamilyAngleTextChange(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.Text.Count() > 0) {
                bool flag = int.TryParse(comboBox.Text, out int value);
                if (flag == false) {
                    return;
                }
                if (value > 180 || value <= -180) {
                    int changeValue = CalcProperAngle180(value);
                    comboBox.Text = changeValue.ToString();
                }
            }
            Preview_Change(sender, e);
        }

        /// <summary>
        /// 配置軸角度の変更時の処理
        /// </summary>
        private void AxisAngleTextChange(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.Text.Count() > 0) {
                bool flag = int.TryParse(comboBox.Text, out int value);
                if (flag == false) {
                    return;
                }
                if (value > 90 || value < -90) {
                    int changeValue = CalcProperAngle90(value);
                    comboBox.Text = changeValue.ToString();
                }
            }
            Preview_Change(sender, e);
        }

        /// <summary>
        /// 角度の変更時の処理
        /// </summary>
        private void AngleTextChange(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.Text.Count() > 0) {
                bool flag = int.TryParse(comboBox.Text, out int value);
                if (flag == false) {
                    return;
                }
            }
            Preview_Change(sender, e);
        }

        /// <summary>
        /// ヴァリデーション
        /// </summary>
        private void Text_Validation(object sender, CancelEventArgs e)
        {
            bool enableFlag = true;
            if (sender.GetType().Name == "TextBox") {
                TextBox text = (TextBox)sender;
                if (!int.TryParse(text.Text, out int t)) {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, Resources.Text.MsgInvalidInput);
                    okButton.Enabled = false;
                    applyButton.Enabled = false;
                }
                else {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, "");
                }
            }
            else {
                ComboBox combo = (ComboBox)sender;
                if (!int.TryParse(combo.Text, out int t)) {
                    e.Cancel = false;
                    errorProviderApp.SetError(combo, Resources.Text.MsgInvalidInput);
                    okButton.Enabled = false;
                    applyButton.Enabled = false;
                }
                else {
                    e.Cancel = false;
                    errorProviderApp.SetError(combo, "");
                }
            }
            if (objectRadio.Checked) {
                if (objectLabel.Text == Resources.Text.LabelNotSelected) {
                    enableFlag = false;
                }
            }
            else {
                if (regionLabel.Text == Resources.Text.LabelNotSpecified) {
                    enableFlag = false;
                }
            }
            if (!int.TryParse(familyAngleCombo.Text, out int fa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(axisAngleCombo.Text, out int aa)) {
                enableFlag = false;
            }
            else if (!int.TryParse(leftMarginText.Text, out int lm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(rightMarginText.Text, out int rm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(frontMarginText.Text, out int fm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(backMarginText.Text, out int bm)) {
                enableFlag = false;
            }
            else if (!int.TryParse(offsetText.Text, out int ot)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(minIntervalX.Text, out int minX)) {
                enableFlag = false;
            }
            else if (minIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(minIntervalY.Text, out int minY)) {
                enableFlag = false;
            }
            else if (maxIntervalX.Text != "" && intervalRadioX.Checked && !int.TryParse(maxIntervalX.Text, out int maxX)) {
                enableFlag = false;
            }
            else if (maxIntervalY.Text != "" && intervalRadioY.Checked && !int.TryParse(maxIntervalY.Text, out int maxY)) {
                enableFlag = false;
            }
            else if (minIntervalX.Text == "" && maxIntervalX.Text == "" && intervalRadioX.Checked) {
                enableFlag = false;
            }
            else if (minIntervalY.Text == "" && maxIntervalY.Text == "" && intervalRadioY.Checked) {
                enableFlag = false;
            }
            else if (countComboX.Text == "" && countSetRadioX.Checked) {
                enableFlag = false;
            }
            else if (countComboY.Text == "" && countSetRadioY.Checked) {
                enableFlag = false;
            }
            if (enableFlag) {
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
            if (minIntervalX.Text == "") {
                errorProviderApp.SetError(minIntervalX, "");
            }
            if (maxIntervalX.Text == "") {
                errorProviderApp.SetError(maxIntervalX, "");
            }
            if (minIntervalY.Text == "") {
                errorProviderApp.SetError(minIntervalY, "");
            }
            if (maxIntervalY.Text == "") {
                errorProviderApp.SetError(maxIntervalY, "");
            }
        }

        /// <summary>
        /// フォームを閉じる際のイベント
        /// </summary>
        private void FormClosedEvent(object sender, FormClosingEventArgs e)
        {
            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionDeleteLine)) {
                tran.Start();
                if (detailLine != null) {
                    Doc.Delete(detailLine.Id);
                    detailLine = null;
                }
                tran.Commit();
            }
        }

        /// <summary>
        /// 配列配置処理
        /// </summary>
        private void SetInstance()
        {
            bool minasFlag = false;
            List<PointF> pointList = new List<PointF>();
            // ビュー
            View activeView = UiDoc.ActiveView;

            // レベル
            Level selectLevel = activeView.GenLevel;
            // レベルに属する要素
            List<Element> levelElement = GetElement(selectLevel);
            // 床天井
            List<Element> scElement = GetSlabCeiling(selectLevel);
            // オフセット
            double.TryParse(offsetText.Text, out double offset);
            offset = UnitUtils.Convert(offset, UnitTypeId.Millimeters, UnitTypeId.Feet);
            // 面配置フラグ
            bool faceFlag = false;
            // X方向メッセージ
            string xMessage = "";
            // X方向メッセージフラグ
            bool xMesFlag = false;
            // Y方向メッセージ
            string yMessage = "";
            // X方向メッセージフラグ
            bool yMesFlag = false;

            // 配置ファミリの取得
            string selectCategory = categoryListBox.SelectedItem.ToString();
            string selectFamily = familyListBox.SelectedItem.ToString();
            string selectType = typeListBox.SelectedItem.ToString();
            FamilySymbol setSymbol = null;
            foreach (Element element in elementList) {
                ElementType elementType = (ElementType)element;
                if (element.Category.Name == selectCategory && elementType.FamilyName == selectFamily && element.Name == selectType) {
                    setSymbol = (FamilySymbol)element;
                }
            }
            // ファミリ角度
            double fAngle = 0;
            if (familyAngleCombo.Text != "" && familyAngleCombo.Text != null) {
                fAngle = double.Parse(familyAngleCombo.Text) * Math.PI / 180;
            }
            // 頂点
            HashSet<XYZ> pointSet = new HashSet<XYZ>();
            if (objectRadio.Checked && selectElement.GetType().Name == "Room") {
                Room room = (Room)selectElement;
                SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings settings = new ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings(UiDoc);
                opt.SpatialElementBoundaryLocation = settings.GetRoomAreaComputation();
                IList<IList<BoundarySegment>> segList = room.GetBoundarySegments(opt);
                foreach (IList<BoundarySegment> list in segList) {
                    foreach (BoundarySegment seg in list) {
                        bool zeroFlag = true;
                        bool oneFlag = true;
                        Curve curve = seg.GetCurve();
                        if (pointSet.Count > 0) {
                            foreach (XYZ point in pointSet) {
                                double zeroX = Math.Round(curve.GetEndPoint(0).X, 3, MidpointRounding.AwayFromZero);
                                double zeroY = Math.Round(curve.GetEndPoint(0).Y, 3, MidpointRounding.AwayFromZero);
                                double zeroZ = Math.Round(curve.GetEndPoint(0).Z, 3, MidpointRounding.AwayFromZero);
                                double oneX = Math.Round(curve.GetEndPoint(1).X, 3, MidpointRounding.AwayFromZero);
                                double oneY = Math.Round(curve.GetEndPoint(1).Y, 3, MidpointRounding.AwayFromZero);
                                double oneZ = Math.Round(curve.GetEndPoint(1).Z, 3, MidpointRounding.AwayFromZero);
                                double pointX = Math.Round(point.X, 3, MidpointRounding.AwayFromZero);
                                double pointY = Math.Round(point.Y, 3, MidpointRounding.AwayFromZero);
                                double pointZ = Math.Round(point.Z, 3, MidpointRounding.AwayFromZero);
                                if (pointX == zeroX
                                    && pointY == zeroY && pointZ == zeroZ) {
                                    zeroFlag = false;
                                }
                                if (pointX == oneX
                                    && pointY == oneY && pointZ == oneZ) {
                                    oneFlag = false;
                                }
                            }
                        }
                        if (zeroFlag) {
                            pointSet.Add(curve.GetEndPoint(0));
                        }
                        if (oneFlag) {
                            pointSet.Add(curve.GetEndPoint(1));
                        }
                    }
                }
            }
            else if (objectRadio.Checked && selectElement.GetType().Name == "Space") {
                Space space = (Space)selectElement;
                SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings settings = new ADSK.JExtRAC.LayoutInstanceInRegion.Components.Settings(UiDoc);
                opt.SpatialElementBoundaryLocation = settings.GetRoomAreaComputation();
                IList<IList<BoundarySegment>> segList = space.GetBoundarySegments(opt);
                foreach (IList<BoundarySegment> list in segList) {
                    foreach (BoundarySegment seg in list) {
                        bool zeroFlag = true;
                        bool oneFlag = true;
                        Curve curve = seg.GetCurve();
                        if (pointSet.Count > 0) {
                            foreach (XYZ point in pointSet) {
                                double zeroX = Math.Round(curve.GetEndPoint(0).X, 3, MidpointRounding.AwayFromZero);
                                double zeroY = Math.Round(curve.GetEndPoint(0).Y, 3, MidpointRounding.AwayFromZero);
                                double zeroZ = Math.Round(curve.GetEndPoint(0).Z, 3, MidpointRounding.AwayFromZero);
                                double oneX = Math.Round(curve.GetEndPoint(1).X, 3, MidpointRounding.AwayFromZero);
                                double oneY = Math.Round(curve.GetEndPoint(1).Y, 3, MidpointRounding.AwayFromZero);
                                double oneZ = Math.Round(curve.GetEndPoint(1).Z, 3, MidpointRounding.AwayFromZero);
                                double pointX = Math.Round(point.X, 3, MidpointRounding.AwayFromZero);
                                double pointY = Math.Round(point.Y, 3, MidpointRounding.AwayFromZero);
                                double pointZ = Math.Round(point.Z, 3, MidpointRounding.AwayFromZero);
                                if (pointX == zeroX
                                    && pointY == zeroY && pointZ == zeroZ) {
                                    zeroFlag = false;
                                }
                                if (pointX == oneX
                                    && pointY == oneY && pointZ == oneZ) {
                                    oneFlag = false;
                                }
                            }
                        }
                        if (zeroFlag) {
                            pointSet.Add(curve.GetEndPoint(0));
                        }
                        if (oneFlag) {
                            pointSet.Add(curve.GetEndPoint(1));
                        }
                    }
                }
            }
            // 配置基準点
            XYZ setPoint = new XYZ(0, 0, 0);
            // 頂点リスト
            List<XYZ> vertexList = new List<XYZ>();
            // 傾きのない矩形フラグ
            bool rectFlag = false;
            // 長さ
            double lengthX = 0;
            double lengthY = 0;
            BoundingBoxXYZ box = null;
            // 選択オブジェクトラジオボタンを選択
            if (objectRadio.Checked) {
                //　バインディングBOX
                box = selectElement.get_BoundingBox(UiDoc.ActiveView);
                lengthX = box.Max.X - box.Min.X;
                lengthY = box.Max.Y - box.Min.Y;
                if (pointSet.Count != 4) {
                    box = selectElement.get_BoundingBox(UiDoc.ActiveView);
                    lengthX = box.Max.X - box.Min.X;
                    lengthY = box.Max.Y - box.Min.Y;
                    XYZ xyz1 = new XYZ(box.Min.X, box.Min.Y, 0);
                    XYZ xyz2 = new XYZ(box.Min.X, box.Max.Y, 0);
                    XYZ xyz3 = new XYZ(box.Max.X, box.Min.Y, 0);
                    XYZ xyz4 = new XYZ(box.Max.X, box.Max.Y, 0);
                    vertexList.Add(xyz1);
                    vertexList.Add(xyz2);
                    vertexList.Add(xyz3);
                    vertexList.Add(xyz4);
                    rectFlag = true;
                }
                else {
                    foreach (XYZ point in pointSet) {
                        vertexList.Add(point);
                    }
                    vertexList.Sort((a, b) => Math.Sign(a.X - b.X));
                    double x0 = Math.Round(vertexList[0].X, 3, MidpointRounding.AwayFromZero);
                    double x1 = Math.Round(vertexList[1].X, 3, MidpointRounding.AwayFromZero);
                    double y0 = Math.Round(vertexList[0].Y, 3, MidpointRounding.AwayFromZero);
                    double y2 = Math.Round(vertexList[2].Y, 3, MidpointRounding.AwayFromZero);
                    double y3 = Math.Round(vertexList[3].Y, 3, MidpointRounding.AwayFromZero);
                    double x2 = Math.Round(vertexList[2].X, 3, MidpointRounding.AwayFromZero);
                    double x3 = Math.Round(vertexList[3].X, 3, MidpointRounding.AwayFromZero);
                    if (x0 == x1 && (y0 == y2 || y0 == 3) && x2 == x3) {
                        rectFlag = true;
                    }
                }
            }
            // 領域指定ラジオボタンを選択
            else if (regionRadio.Checked) {
                XYZ min = pickedBox.Min;
                XYZ max = pickedBox.Max;
                double maxX = max.X > min.X ? max.X : min.X;
                double minX = max.X > min.X ? min.X : max.X;
                double maxY = max.Y > min.Y ? max.Y : min.Y;
                double minY = max.Y > min.Y ? min.Y : max.Y;
                box = new BoundingBoxXYZ();
                box.Max = new XYZ(maxX, maxY, max.Z);
                box.Min = new XYZ(minX, minY, min.Z);
                XYZ xyz1 = new XYZ(box.Min.X, box.Min.Y, 0);
                XYZ xyz2 = new XYZ(box.Min.X, box.Max.Y, 0);
                XYZ xyz3 = new XYZ(box.Max.X, box.Min.Y, 0);
                XYZ xyz4 = new XYZ(box.Max.X, box.Max.Y, 0);
                vertexList.Add(xyz1);
                vertexList.Add(xyz2);
                vertexList.Add(xyz3);
                vertexList.Add(xyz4);
                rectFlag = true;
            }

            // 頂点
            XYZ leftBottom;
            XYZ leftTop;
            XYZ rightBottom;
            XYZ rightTop;
            if (vertexList[0].Y < vertexList[1].Y) {
                leftBottom = vertexList[0];
                leftTop = vertexList[1];
            }
            else {
                leftBottom = vertexList[1];
                leftTop = vertexList[0];
            }
            if (vertexList[2].Y < vertexList[3].Y) {
                rightBottom = vertexList[2];
                rightTop = vertexList[3];
            }
            else {
                rightBottom = vertexList[3];
                rightTop = vertexList[2];
            }
            // マージン
            double.TryParse(leftMarginText.Text, out double leftMargin);
            leftMargin = UnitUtils.Convert(leftMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(rightMarginText.Text, out double rightMargin);
            rightMargin = UnitUtils.Convert(rightMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(frontMarginText.Text, out double frontMargin);
            frontMargin = UnitUtils.Convert(frontMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            double.TryParse(backMarginText.Text, out double backMargin);
            backMargin = UnitUtils.Convert(backMargin, UnitTypeId.Millimeters, UnitTypeId.Feet);
            if (rectFlag) {
                // 傾いていない場合
                leftBottom = new XYZ(leftBottom.X + leftMargin, leftBottom.Y + frontMargin, leftBottom.Z);
                leftTop = new XYZ(leftTop.X + leftMargin, leftTop.Y - backMargin, leftTop.Z);
                rightBottom = new XYZ(rightBottom.X - rightMargin, rightBottom.Y + frontMargin, rightBottom.Z);
                rightTop = new XYZ(rightTop.X - rightMargin, rightTop.Y - backMargin, leftTop.Z);
                lengthX = rightBottom.X - leftBottom.X;
                lengthY = leftTop.Y - leftBottom.Y;
                setPoint = leftBottom;
            }
            else {
                // 傾いている場合
                XYZ zero = new XYZ(1, 0, 0);
                // 左下について
                // 左下および右下のなす角度
                double rad = zero.AngleTo(new XYZ(rightBottom.X - leftBottom.X, rightBottom.Y - leftBottom.Y, 0));
                double angle = rad / Math.PI * 180;
                double mAngle = 180 - 90 - angle;
                double mRad = mAngle * Math.PI / 180;
                // 左マージン
                double xLeftMargin = Math.Cos(rad) * leftMargin;
                double yLeftMargin = Math.Sin(rad) * leftMargin;
                // 前マージン
                double xFrontMargin = Math.Cos(mRad) * frontMargin;
                double yFrontMargin = Math.Sin(mRad) * frontMargin;
                // 右マージン
                double xRightMargin = Math.Cos(rad) * rightMargin;
                double yRightMargin = Math.Sin(rad) * rightMargin;
                // 後マージン
                double xBackMargin = Math.Cos(mRad) * backMargin;
                double yBackMargin = Math.Sin(mRad) * backMargin;
                if (leftBottom.Y < rightBottom.Y) {
                    // 座標移動
                    leftBottom = new XYZ(leftBottom.X + xLeftMargin - xFrontMargin, leftBottom.Y + yLeftMargin + yFrontMargin, leftBottom.Z);
                    leftTop = new XYZ(leftTop.X + xLeftMargin + xBackMargin, leftTop.Y + yLeftMargin - yBackMargin, leftTop.Z);
                    rightBottom = new XYZ(rightBottom.X - xRightMargin - xFrontMargin, rightBottom.Y - yRightMargin + yFrontMargin, rightBottom.Z);
                    rightTop = new XYZ(rightTop.X - xRightMargin + xBackMargin, rightTop.Y - yRightMargin - yBackMargin, rightTop.Z);
                }
                else {
                    // 座標移動
                    leftBottom = new XYZ(leftBottom.X + xLeftMargin + xFrontMargin, leftBottom.Y - yLeftMargin + yFrontMargin, leftBottom.Z);
                    leftTop = new XYZ(leftTop.X + xLeftMargin - xBackMargin, leftTop.Y - yLeftMargin - yBackMargin, leftTop.Z);
                    rightBottom = new XYZ(rightBottom.X - xRightMargin + xFrontMargin, rightBottom.Y + yRightMargin + yFrontMargin, rightBottom.Z);
                    rightTop = new XYZ(rightTop.X - xRightMargin - xBackMargin, rightTop.Y + yRightMargin - yBackMargin, rightTop.Z);
                }
            }

            // ラジアン
            double setRad = 0;
            // 配置軸
            double setAngle = 0;
            if (axisAngleCombo.Text != "") {
                setAngle = double.Parse(axisAngleCombo.Text);
            }
            if (setAngle != 0) {
                if (setAngle == 90) {
                    setAngle = 89.999;
                }
                if (setAngle == -90) {
                    setAngle = -89.999;
                }
                // 傾き
                setRad = setAngle * Math.PI / 180;
                double tilt = Math.Tan(setRad);
                double mTilt = -(1 / tilt);
                double mTilt2 = tilt + (1 / tilt);
                // 左下について
                double interceptLeftBottom = 0;
                // 左上について
                double interceptLeftTop = 0;
                // 右下について
                double interceptRightBottom = 0;
                // 右上について
                double interceptRightTop = 0;
                // 左下および左上についての交点
                XYZ leftIntersection1 = new XYZ(0, 0, 0);

                // 左下および右下についての交点
                XYZ leftIntersection2 = new XYZ(0, 0, 0);

                // 右下および右上についての交点
                XYZ rightIntersection1 = new XYZ(0, 0, 0);

                // 右上および左上についての交点
                XYZ rightIntersection2 = new XYZ(0, 0, 0);
                if (Math.Round(leftBottom.Y, 3, MidpointRounding.AwayFromZero) == Math.Round(rightBottom.Y, 3, MidpointRounding.AwayFromZero)) {
                    // 各頂点を通る直線の切片
                    if (tilt > 0) {
                        // 左下について
                        interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                        // 左上について
                        interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                        // 右下について
                        interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                        // 右上について
                        interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                        // 左下および左上についての交点
                        double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                        double y1 = tilt * x1 + interceptLeftTop;
                        leftIntersection1 = new XYZ(x1, y1, 0);
                        // 左下および右下についての交点
                        double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                        double y2 = tilt * x2 + interceptRightBottom;
                        leftIntersection2 = new XYZ(x2, y2, 0);
                        setPoint = leftIntersection2;
                        // 右下および右上についての交点
                        double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                        double y3 = tilt * x3 + interceptRightBottom;
                        rightIntersection1 = new XYZ(x3, y3, 0);
                        // 右上および左上についての交点
                        double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                        double y4 = tilt * x4 + interceptLeftTop;
                        rightIntersection2 = new XYZ(x4, y4, 0);
                        // 交点間の長さ
                        double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                        double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                        lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                        double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                        double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                        lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                    }
                    else if (tilt < 0) {
                        // 左下について
                        interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                        // 左上について
                        interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                        // 右下について
                        interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                        // 右上について
                        interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                        // 左下および左上についての交点
                        double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                        double y1 = mTilt * x1 + interceptLeftTop;
                        leftIntersection1 = new XYZ(x1, y1, 0);
                        setPoint = leftIntersection1;
                        // 左下および右下についての交点
                        double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                        double y2 = mTilt * x2 + interceptRightBottom;
                        leftIntersection2 = new XYZ(x2, y2, 0);
                        // 右下および右上についての交点
                        double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                        double y3 = mTilt * x3 + interceptRightBottom;
                        rightIntersection1 = new XYZ(x3, y3, 0);
                        // 右上および左上についての交点
                        double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                        double y4 = mTilt * x4 + interceptLeftTop;
                        rightIntersection2 = new XYZ(x4, y4, 0);
                        // 交点間の長さ
                        double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                        double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                        lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                        double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                        double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                        lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                    }
                }
                else if (leftBottom.Y < rightBottom.Y) {
                    if (tilt > 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180;
                        if (angle2 >= setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                    else if (tilt < 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180;
                        double angle3 = 90 - angle2;
                        double setAngle2 = setAngle * -1;
                        if (angle3 >= setAngle2) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);

                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            setPoint = rightIntersection2;
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                }
                else {
                    if (tilt > 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = 90 - (rad2 / Math.PI * 180);
                        if (angle2 < setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            setPoint = rightIntersection1;
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                    else if (tilt < 0) {
                        Line line = Line.CreateBound(leftBottom, rightBottom);
                        XYZ zero = new XYZ(1, 0, 0);
                        double rad2 = zero.AngleTo(line.Direction);
                        double angle2 = rad2 / Math.PI * 180 * -1;
                        if (angle2 >= setAngle) {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (tilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (mTilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (mTilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (tilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftTop - interceptLeftBottom) / mTilt2;
                            double y1 = mTilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            setPoint = leftIntersection1;
                            // 左下および右下についての交点
                            double x2 = (interceptRightBottom - interceptLeftBottom) / mTilt2;
                            double y2 = mTilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            // 右下および右上についての交点
                            double x3 = (interceptRightBottom - interceptRightTop) / mTilt2;
                            double y3 = mTilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptLeftTop - interceptRightTop) / mTilt2;
                            double y4 = mTilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                        else {
                            // 左下について
                            interceptLeftBottom = leftBottom.Y - (mTilt * leftBottom.X);
                            // 左上について
                            interceptLeftTop = leftTop.Y - (tilt * leftTop.X);
                            // 右下について
                            interceptRightBottom = rightBottom.Y - (tilt * rightBottom.X);
                            // 右上について
                            interceptRightTop = rightTop.Y - (mTilt * rightTop.X);
                            // 左下および左上についての交点
                            double x1 = (interceptLeftBottom - interceptLeftTop) / mTilt2;
                            double y1 = tilt * x1 + interceptLeftTop;
                            leftIntersection1 = new XYZ(x1, y1, 0);
                            // 左下および右下についての交点
                            double x2 = (interceptLeftBottom - interceptRightBottom) / mTilt2;
                            double y2 = tilt * x2 + interceptRightBottom;
                            leftIntersection2 = new XYZ(x2, y2, 0);
                            setPoint = leftIntersection2;
                            // 右下および右上についての交点
                            double x3 = (interceptRightTop - interceptRightBottom) / mTilt2;
                            double y3 = tilt * x3 + interceptRightBottom;
                            rightIntersection1 = new XYZ(x3, y3, 0);
                            // 右上および左上についての交点
                            double x4 = (interceptRightTop - interceptLeftTop) / mTilt2;
                            double y4 = tilt * x4 + interceptLeftTop;
                            rightIntersection2 = new XYZ(x4, y4, 0);
                            // 交点間の長さ
                            double bottom1 = leftIntersection1.X > leftIntersection2.X ? leftIntersection1.X - leftIntersection2.X : leftIntersection2.X - leftIntersection1.X;
                            double height1 = leftIntersection1.Y > leftIntersection2.Y ? leftIntersection1.Y - leftIntersection2.Y : leftIntersection2.Y - leftIntersection1.Y;
                            lengthY = Math.Sqrt(Math.Pow(bottom1, 2) + Math.Pow(height1, 2));
                            double bottom2 = leftIntersection2.X > rightIntersection1.X ? leftIntersection2.X - rightIntersection1.X : leftIntersection2.X - rightIntersection1.X;
                            double height2 = leftIntersection2.Y > rightIntersection1.Y ? leftIntersection2.Y - rightIntersection1.Y : leftIntersection2.Y - rightIntersection1.Y;
                            lengthX = Math.Sqrt(Math.Pow(bottom2, 2) + Math.Pow(height2, 2));
                        }
                    }
                }
            }
            else if (setAngle == 0) {
                if (!rectFlag) {
                    List<double> xList = new List<double>();
                    xList.Add(leftBottom.X);
                    xList.Add(leftTop.X);
                    xList.Add(rightBottom.X);
                    xList.Add(rightTop.X);
                    List<double> yList = new List<double>();
                    yList.Add(leftBottom.Y);
                    yList.Add(leftTop.Y);
                    yList.Add(rightBottom.Y);
                    yList.Add(rightTop.Y);

                    XYZ mLeftBottom = new XYZ(xList.Min(), yList.Min(), 0);
                    XYZ mLeftTop = new XYZ(xList.Min(), yList.Max(), 0);
                    XYZ mRightBottom = new XYZ(xList.Max(), yList.Min(), 0);
                    XYZ mRightTop = new XYZ(xList.Max(), yList.Max(), 0);

                    lengthX = mRightBottom.X - mLeftBottom.X;
                    lengthY = mLeftTop.Y - mLeftBottom.Y;
                    setPoint = mLeftBottom;
                }
            }
            // 配置設定取得
            double bottomX = 0;
            double heightX = 0;
            double bottomY = 0;
            double heightY = 0;
            double increaseX = 0;
            double increaseY = 0;
            if (countSetRadioX.Checked) {
                int.TryParse(countComboX.Text.ToString(), out int countX);
                double mLengthX = 0;
                double intervalX = 0;
                if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                    mLengthX = lengthX / (countX + 1);
                }
                else {
                    mLengthX = lengthX / countX;
                    intervalX = setPoint.X + (mLengthX / 2);
                }
                // Xが個数指定配置
                if (countSetRadioY.Checked) {
                    int.TryParse(countComboY.Text.ToString(), out int countY);
                    double mLengthY = 0;
                    double intervalY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else {
                        mLengthY = lengthY / countY;
                        intervalY = box.Min.Y + (mLengthY / 2);
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }

                    for (int i = 0; i < countX; i++) {
                        for (int j = 0; j < countY; j++) {
                            faceFlag = false;
                            // 配置範囲
                            PointF p1 = new PointF((float)leftBottom.X, (float)leftBottom.Y);
                            PointF p2 = new PointF((float)leftTop.X, (float)leftTop.Y);
                            PointF p3 = new PointF((float)rightTop.X, (float)rightTop.Y);
                            PointF p4 = new PointF((float)rightBottom.X, (float)rightBottom.Y);
                            KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
                            KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
                            KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
                            KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
                            XYZ xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                            if (minasFlag) {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            else {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            FamilyInstance instance = null;
                            List<Element> candidateList = GetHostElementCandidateXY(xyz, levelElement, activeView);
                            Face face = GetFace(xyz, candidateList);
                            List<Element> scCandidate = GetHostElementCandidateXY(xyz, scElement, activeView);
                            Face scFace = GetFaceSlabCeiling(xyz, scCandidate, activeView);
                            if (scFace != null) {
                                face = scFace;
                            }
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                tran.Start();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                setSymbol.Activate();
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.WorkPlaneBased) {
                                    XYZ dir = new XYZ(0, 1, 0);
                                    if (face == null) {
                                        continue;
                                    }
                                    if (face.GetType().Name == "PlanarFace") {
                                        PlanarFace pF = face as PlanarFace;
                                        XYZ fN = pF.FaceNormal;
                                        double x = 0;
                                        double y = 0;
                                        double sl = fN.Y / fN.X;
                                        if (double.IsInfinity(sl)) {
                                            x = 1;
                                            y = 0;
                                        }
                                        else if (sl == 0) {
                                            x = 0;
                                            y = 1;
                                        }
                                        else {
                                            double vs = -1 / sl;
                                            x = 1;
                                            y = 1 * vs;
                                        }
                                        if (double.IsNaN(y)) {
                                            y = 0;
                                        }
                                        dir = new XYZ(x, y, 0).Normalize();
                                    }
                                    else {
                                        UV uv = new UV(activeView.ViewDirection.X, activeView.ViewDirection.Y);
                                        XYZ fN = face.ComputeNormal(uv);
                                        double x = 0;
                                        double y = 0;
                                        double sl = fN.Y / fN.X;
                                        if (double.IsInfinity(sl)) {
                                            x = 1;
                                            y = 0;
                                        }
                                        else if (sl == 0) {
                                            x = 0;
                                            y = 1;
                                        }
                                        else {
                                            double vs = -1 / sl;
                                            x = 1;
                                            y = 1 * vs;
                                        }
                                        if (double.IsNaN(y)) {
                                            y = 0;
                                        }
                                        dir = new XYZ(x, y, 0).Normalize();
                                    }
                                    // dir = new XYZ(0, 0, 0);
                                    instance = Doc.Create.NewFamilyInstance(face, xyz, dir, setSymbol);
                                    faceFlag = true;
                                }
                                else {
                                    xyz = new XYZ(xyz.X, xyz.Y, offset);
                                    instance = Doc.Create.NewFamilyInstance(xyz, setSymbol, selectLevel, StructuralType.NonStructural);
                                }
                                tran.Commit();
                            }
                            XYZ iLeftBottom = new XYZ(0, 0, 0);
                            XYZ iLeftTop = new XYZ(0, 0, 0);
                            XYZ iRightBottom = new XYZ(0, 0, 0);
                            XYZ iRightTop = new XYZ(0, 0, 0);
                            XYZ center = new XYZ(0, 0, 0);

                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionRotateFamily)) {
                                tran.Start();
                                if (fAngle != 0) {
                                    if (!faceFlag) {
                                        Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                        ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                    }
                                    else {
                                        if (scFace != null) {
                                            if (scFace.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)scFace;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                        else {
                                            if (face.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)face;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, xyz.Z + pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                double modifyX = 0;
                                                double modifyY = 0;
                                                XYZ direction = instance.HandOrientation;
                                                double x = 0;
                                                double y = 0;
                                                double sl = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 1;
                                                    y = 0;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 0;
                                                    y = 1;
                                                }
                                                else {
                                                    double vs = -1 / sl;
                                                    x = 1;
                                                    y = 1 * vs;
                                                }
                                                direction = new XYZ(x, y, direction.Z).Normalize();
                                                double sl2 = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 0;
                                                    modifyY = 1;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 1;
                                                    modifyY = 0;
                                                }
                                                else {
                                                    modifyX = 1;
                                                    modifyY = 1 * sl2;
                                                }
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + modifyX, xyz.Y + modifyY, xyz.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // オフセット
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionOffset)) {
                                tran.Start();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                setSymbol.Activate();
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.TwoLevelsBased) {
                                    ParameterSet paraSet = instance.Parameters;
                                    foreach (Parameter para in paraSet) {
                                        if (para.Definition.Name == Resources.Text.ParamBaseOffset) {
                                            para.Set(offset);
                                        }
                                        if (para.Definition.Name == Resources.Text.ParamTopOffset) {
                                            para.Set(offset);
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // 範囲外の判定
                            bool deleteFlag = false;
                            // 配置点での判定
                            KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
                            LineData[] lineData = pares.Select(line => new LineData(line)).ToArray();

                            PointF m1 = new PointF((float)xyz.X, (float)xyz.Y);
                            PointF[] points = { m1 };
                            PointData[] pointData = points.Select(p => new PointData(p)).ToArray();
                            foreach (PointData point in pointData) {
                                if (!IsRange(point, lineData)) {
                                    deleteFlag = true;
                                }
                            }
                            // インスタンスのSolidの取得
                            Options opt = new Options();
                            opt.ComputeReferences = true;
                            opt.DetailLevel = ViewDetailLevel.Fine;
                            GeometryElement geo = instance.get_Geometry(opt);
                            foreach (GeometryObject obj in geo) {
                                GeometryInstance geoInstance = obj as GeometryInstance;
                                if (geoInstance != null) {
                                    GeometryElement geo2 = geoInstance.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            BoundingBoxXYZ instanceBox = instance.get_BoundingBox(activeView);
                                            double z = (instanceBox.Max.Z + instanceBox.Min.Z) / 2;
                                            XYZ mLeftBottom = new XYZ(leftBottom.X - 0.001, leftBottom.Y - 0.001, z);
                                            XYZ mLeftTop = new XYZ(leftTop.X - 0.001, leftTop.Y + 0.001, z);
                                            XYZ mRightBottom = new XYZ(rightBottom.X + 0.001, rightBottom.Y - 0.001, z);
                                            XYZ mRightTop = new XYZ(rightTop.X + 0.001, rightTop.Y + 0.001, z);
                                            Line line1 = Line.CreateBound(mLeftTop, mLeftBottom);
                                            Line line2 = Line.CreateBound(mLeftBottom, mRightBottom);
                                            Line line3 = Line.CreateBound(mRightBottom, mRightTop);
                                            Line line4 = Line.CreateBound(mRightTop, mLeftTop);
                                            List<Line> lineList = new List<Line>();
                                            lineList.Add(line1);
                                            lineList.Add(line2);
                                            lineList.Add(line3);
                                            lineList.Add(line4);
                                            SolidCurveIntersectionOptions option = new SolidCurveIntersectionOptions();
                                            foreach (Line line in lineList) {
                                                SolidCurveIntersection section = solid.IntersectWithCurve(line, option);
                                                if (section.Count() > 0) {
                                                    deleteFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                Solid solid2 = obj as Solid;
                                if (solid2 != null) {
                                    deleteFlag = true;
                                }
                            }
                            if (!deleteCheck.Checked) {
                                deleteFlag = false;
                            }

                            if (deleteFlag) {
                                using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                    tran.Start();
                                    Doc.Delete(instance.Id);
                                    tran.Commit();
                                }
                            }
                        }
                    }
                }
                // Yが間隔指定配置
                else {
                    double intervalY = 0;
                    int countY = 0;
                    double widthY = lengthY;
                    double milliY = UnitUtils.Convert(widthY, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    // 入力値チェック
                    int.TryParse(minIntervalY.Text, out int minY);
                    int.TryParse(maxIntervalY.Text, out int maxY);
                    // チェックフラグ
                    bool checkFlag = false;
                    if (minY != 0 && maxY == 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                            yMessage = Resources.Text.MsgYIntervalMinError;
                        }
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            intervalY = widthY / countY;
                        } while (intervalY >= mMinY);
                        countY--;
                    }
                    else if (minY == 0 && maxY != 0) {
                        countY = 1;
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            intervalY = widthY / countY;
                        } while (intervalY > mMaxY);
                    }
                    else if (minY != 0 && maxY != 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                            yMessage = Resources.Text.MsgYIntervalMinError;
                        }
                        if (!yMesFlag) {
                            countY = 1;
                            double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                            double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                            intervalY = 0;
                            do {
                                countY++;
                                intervalY = widthY / countY;
                                if ((intervalY <= mMaxY && intervalY >= mMinY)) {
                                    checkFlag = true;
                                }
                            } while (intervalY > mMaxY || (intervalY <= mMaxY && intervalY >= mMinY));
                            countY--;
                            if (!checkFlag) {
                                yMesFlag = true;
                                yMessage = Resources.Text.MsgYDirectionError;
                            }
                        }
                    }
                    if (yMesFlag) {
                        okFlag = false;
                        ComDialog.ShowDialog(Resources.Text.DialogWarning, TaskDialogIcon.TaskDialogIconError, yMessage, false);
                        return;
                    }
                    countY--;
                    double mLengthY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    for (int i = 0; i < countX; i++) {
                        for (int j = 0; j < countY; j++) {
                            XYZ xyz = new XYZ(intervalX + mLengthX * i, intervalY + mLengthY * j, selectLevel.Elevation + offset);
                            if (minasFlag) {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            else {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            faceFlag = false;
                            // 配置範囲
                            PointF p1 = new PointF((float)leftBottom.X, (float)leftBottom.Y);
                            PointF p2 = new PointF((float)leftTop.X, (float)leftTop.Y);
                            PointF p3 = new PointF((float)rightTop.X, (float)rightTop.Y);
                            PointF p4 = new PointF((float)rightBottom.X, (float)rightBottom.Y);
                            KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
                            KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
                            KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
                            KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
                            FamilyInstance instance = null;
                            List<Element> candidateList = GetHostElementCandidateXY(xyz, levelElement, activeView);
                            Face face = GetFace(xyz, candidateList);
                            List<Element> scCandidate = GetHostElementCandidateXY(xyz, scElement, activeView);
                            Face scFace = GetFaceSlabCeiling(xyz, scCandidate, activeView);
                            if (scFace != null) {
                                face = scFace;
                            }
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                tran.Start();
                                setSymbol.Activate();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.WorkPlaneBased) {
                                    XYZ dir = new XYZ(0, 1, 0);
                                    if (face == null) {
                                        continue;
                                    }
                                    dir = new XYZ(0, 0, 0);
                                    instance = Doc.Create.NewFamilyInstance(face, xyz, dir, setSymbol);
                                    faceFlag = true;
                                }
                                else {
                                    xyz = new XYZ(xyz.X, xyz.Y, offset);
                                    instance = Doc.Create.NewFamilyInstance(xyz, setSymbol, selectLevel, StructuralType.NonStructural);
                                }
                                tran.Commit();
                            }
                            BoundingBoxXYZ box2 = instance.get_BoundingBox(activeView);
                            double lx = box2.Max.X - box2.Min.X;
                            double ly = box2.Max.Y - box2.Min.Y;

                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionRotateFamily)) {
                                tran.Start();
                                if (fAngle != 0) {
                                    if (!faceFlag) {
                                        Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                        ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                    }
                                    else {
                                        if (scFace != null) {
                                            if (scFace.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)scFace;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                        else {
                                            if (face.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)face;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, xyz.Z + pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                double modifyX = 0;
                                                double modifyY = 0;
                                                XYZ direction = instance.HandOrientation;
                                                double x = 0;
                                                double y = 0;
                                                double sl = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 1;
                                                    y = 0;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 0;
                                                    y = 1;
                                                }
                                                else {
                                                    double vs = -1 / sl;
                                                    x = 1;
                                                    y = 1 * vs;
                                                }
                                                direction = new XYZ(x, y, direction.Z).Normalize();
                                                double sl2 = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 0;
                                                    modifyY = 1;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 1;
                                                    modifyY = 0;
                                                }
                                                else {
                                                    modifyX = 1;
                                                    modifyY = 1 * sl2;
                                                }
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + modifyX, xyz.Y + modifyY, xyz.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // オフセット
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionOffset)) {
                                tran.Start();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                setSymbol.Activate();
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.TwoLevelsBased) {
                                    ParameterSet paraSet = instance.Parameters;
                                    foreach (Parameter para in paraSet) {
                                        if (para.Definition.Name == Resources.Text.ParamBaseOffset) {
                                            para.Set(offset);
                                        }
                                        if (para.Definition.Name == Resources.Text.ParamTopOffset) {
                                            para.Set(offset);
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // 範囲外の判定
                            bool deleteFlag = false;
                            // 配置点での判定
                            KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
                            LineData[] lineData = pares.Select(line => new LineData(line)).ToArray();

                            PointF m1 = new PointF((float)xyz.X, (float)xyz.Y);
                            PointF[] points = { m1 };
                            PointData[] pointData = points.Select(p => new PointData(p)).ToArray();
                            foreach (PointData point in pointData) {
                                if (!IsRange(point, lineData)) {
                                    deleteFlag = true;
                                }
                            }
                            // インスタンスのSolidの取得
                            Options opt = new Options();
                            opt.ComputeReferences = true;
                            opt.DetailLevel = ViewDetailLevel.Fine;
                            GeometryElement geo = instance.get_Geometry(opt);
                            foreach (GeometryObject obj in geo) {
                                GeometryInstance geoInstance = obj as GeometryInstance;
                                if (geoInstance != null) {
                                    GeometryElement geo2 = geoInstance.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            BoundingBoxXYZ instanceBox = instance.get_BoundingBox(activeView);
                                            double z = (instanceBox.Max.Z + instanceBox.Min.Z) / 2;
                                            XYZ mLeftBottom = new XYZ(leftBottom.X - 0.001, leftBottom.Y - 0.001, z);
                                            XYZ mLeftTop = new XYZ(leftTop.X - 0.001, leftTop.Y + 0.001, z);
                                            XYZ mRightBottom = new XYZ(rightBottom.X + 0.001, rightBottom.Y - 0.001, z);
                                            XYZ mRightTop = new XYZ(rightTop.X + 0.001, rightTop.Y + 0.001, z);
                                            Line line1 = Line.CreateBound(mLeftTop, mLeftBottom);
                                            Line line2 = Line.CreateBound(mLeftBottom, mRightBottom);
                                            Line line3 = Line.CreateBound(mRightBottom, mRightTop);
                                            Line line4 = Line.CreateBound(mRightTop, mLeftTop);
                                            List<Line> lineList = new List<Line>();
                                            lineList.Add(line1);
                                            lineList.Add(line2);
                                            lineList.Add(line3);
                                            lineList.Add(line4);
                                            SolidCurveIntersectionOptions option = new SolidCurveIntersectionOptions();
                                            foreach (Line line in lineList) {
                                                SolidCurveIntersection section = solid.IntersectWithCurve(line, option);
                                                if (section.Count() > 0) {
                                                    deleteFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                Solid solid2 = obj as Solid;
                                if (solid2 != null) {
                                    deleteFlag = true;
                                }
                            }
                            if (!deleteCheck.Checked) {
                                deleteFlag = false;
                            }

                            if (deleteFlag) {
                                using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                    tran.Start();
                                    Doc.Delete(instance.Id);
                                    tran.Commit();
                                }
                            }
                        }
                    }
                }
            }
            // Xが間隔指定配置
            else {
                double intervalX = 0;
                int countX = 0;
                double widthX = lengthX;
                double milliX = UnitUtils.Convert(widthX, UnitTypeId.Feet, UnitTypeId.Millimeters);
                // 入力値チェック
                int.TryParse(minIntervalX.Text, out int minX);
                int.TryParse(maxIntervalX.Text, out int maxX);
                // チェックフラグ
                bool checkFlag = false;
                if (minX != 0 && maxX == 0) {
                    if (milliX < minX * 2) {
                        xMesFlag = true;
                        xMessage = Resources.Text.MsgXIntervalMinError;
                    }
                    countX = 1;
                    double mMinX = UnitUtils.Convert(minX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    intervalX = 0;
                    do {
                        countX++;
                        intervalX = widthX / countX;
                    } while (intervalX >= mMinX);
                    countX--;
                }
                else if (minX == 0 && maxX != 0) {
                    countX = 1;
                    double mMaxX = UnitUtils.Convert(maxX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    intervalX = 0;
                    do {
                        countX++;
                        intervalX = widthX / countX;
                    } while (intervalX > mMaxX);
                }
                else if (minX != 0 && maxX != 0) {
                    countX = 1;
                    double mMinX = UnitUtils.Convert(minX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    double mMaxX = UnitUtils.Convert(maxX, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    if (milliX < minX * 2) {
                        xMesFlag = true;
                        xMessage = Resources.Text.MsgXIntervalMinError;
                    }
                    if (!xMesFlag) {
                        intervalX = 0;
                        do {
                            countX++;
                            intervalX = widthX / countX;
                            if ((intervalX <= mMaxX && intervalX >= mMinX)) {
                                checkFlag = true;
                            }
                        } while (intervalX > mMaxX || (intervalX <= mMaxX && intervalX >= mMinX));
                        countX--;
                        if (!checkFlag) {
                            xMesFlag = true;
                            xMessage = Resources.Text.MsgXDirectionError;
                        }
                    }
                }
                countX--;
                double mLengthX = 0;
                if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                    mLengthX = lengthX / (countX + 1);
                    intervalX = box.Min.X + mLengthX;
                }
                else {
                    mLengthX = lengthX / countX;
                    intervalX = box.Min.X + (mLengthX / 2);
                }

                if (countSetRadioY.Checked) {
                    if (xMesFlag) {
                        okFlag = false;
                        ComDialog.ShowDialog(Resources.Text.DialogWarning, TaskDialogIcon.TaskDialogIconError, xMessage, false);
                        return;
                    }
                    int.TryParse(countComboY.Text.ToString(), out int countY);
                    double mLengthY = 0;
                    double intervalY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    for (int i = 0; i < countX; i++) {
                        for (int j = 0; j < countY; j++) {
                            XYZ xyz = new XYZ(intervalX + mLengthX * i, intervalY + mLengthY * j, selectLevel.Elevation + offset);
                            if (minasFlag) {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            else {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            faceFlag = false;
                            // 配置範囲
                            PointF p1 = new PointF((float)leftBottom.X, (float)leftBottom.Y);
                            PointF p2 = new PointF((float)leftTop.X, (float)leftTop.Y);
                            PointF p3 = new PointF((float)rightTop.X, (float)rightTop.Y);
                            PointF p4 = new PointF((float)rightBottom.X, (float)rightBottom.Y);
                            KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
                            KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
                            KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
                            KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
                            List<Element> candidateList = GetHostElementCandidateXY(xyz, levelElement, activeView);
                            Face face = GetFace(xyz, candidateList);
                            List<Element> scCandidate = GetHostElementCandidateXY(xyz, scElement, activeView);
                            Face scFace = GetFaceSlabCeiling(xyz, scCandidate, activeView);
                            if (scFace != null) {
                                face = scFace;
                            }
                            FamilyInstance instance = null;
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                tran.Start();
                                setSymbol.Activate();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.WorkPlaneBased) {
                                    XYZ dir = new XYZ(0, 1, 0);
                                    if (face == null) {
                                        continue;
                                    }
                                    dir = new XYZ(0, 0, 0);
                                    instance = Doc.Create.NewFamilyInstance(face, xyz, dir, setSymbol);
                                    faceFlag = true;
                                }
                                else {
                                    xyz = new XYZ(xyz.X, xyz.Y, offset);
                                    instance = Doc.Create.NewFamilyInstance(xyz, setSymbol, selectLevel, StructuralType.NonStructural);
                                }
                                tran.Commit();
                            }
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionRotateFamily)) {
                                tran.Start();
                                if (fAngle != 0) {
                                    if (!faceFlag) {
                                        Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                        ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                    }
                                    else {
                                        if (scFace != null) {
                                            if (scFace.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)scFace;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                        else {
                                            if (face.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)face;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, xyz.Z + pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                double modifyX = 0;
                                                double modifyY = 0;
                                                XYZ direction = instance.HandOrientation;
                                                double x = 0;
                                                double y = 0;
                                                double sl = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 1;
                                                    y = 0;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 0;
                                                    y = 1;
                                                }
                                                else {
                                                    double vs = -1 / sl;
                                                    x = 1;
                                                    y = 1 * vs;
                                                }
                                                direction = new XYZ(x, y, direction.Z).Normalize();
                                                double sl2 = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 0;
                                                    modifyY = 1;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 1;
                                                    modifyY = 0;
                                                }
                                                else {
                                                    modifyX = 1;
                                                    modifyY = 1 * sl2;
                                                }
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + modifyX, xyz.Y + modifyY, xyz.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // オフセット
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionOffset)) {
                                tran.Start();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                setSymbol.Activate();
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.TwoLevelsBased) {
                                    ParameterSet paraSet = instance.Parameters;
                                    foreach (Parameter para in paraSet) {
                                        if (para.Definition.Name == Resources.Text.ParamBaseOffset) {
                                            para.Set(offset);
                                        }
                                        if (para.Definition.Name == Resources.Text.ParamTopOffset) {
                                            para.Set(offset);
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // 範囲外の判定
                            bool deleteFlag = false;
                            // 配置点での判定
                            KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
                            LineData[] lineData = pares.Select(line => new LineData(line)).ToArray();

                            PointF m1 = new PointF((float)xyz.X, (float)xyz.Y);
                            PointF[] points = { m1 };
                            PointData[] pointData = points.Select(p => new PointData(p)).ToArray();
                            foreach (PointData point in pointData) {
                                if (!IsRange(point, lineData)) {
                                    deleteFlag = true;
                                }
                            }
                            // インスタンスのSolidの取得
                            Options opt = new Options();
                            opt.ComputeReferences = true;
                            opt.DetailLevel = ViewDetailLevel.Fine;
                            GeometryElement geo = instance.get_Geometry(opt);
                            foreach (GeometryObject obj in geo) {
                                GeometryInstance geoInstance = obj as GeometryInstance;
                                if (geoInstance != null) {
                                    GeometryElement geo2 = geoInstance.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            BoundingBoxXYZ instanceBox = instance.get_BoundingBox(activeView);
                                            double z = (instanceBox.Max.Z + instanceBox.Min.Z) / 2;
                                            XYZ mLeftBottom = new XYZ(leftBottom.X - 0.001, leftBottom.Y - 0.001, z);
                                            XYZ mLeftTop = new XYZ(leftTop.X - 0.001, leftTop.Y + 0.001, z);
                                            XYZ mRightBottom = new XYZ(rightBottom.X + 0.001, rightBottom.Y - 0.001, z);
                                            XYZ mRightTop = new XYZ(rightTop.X + 0.001, rightTop.Y + 0.001, z);
                                            Line line1 = Line.CreateBound(mLeftTop, mLeftBottom);
                                            Line line2 = Line.CreateBound(mLeftBottom, mRightBottom);
                                            Line line3 = Line.CreateBound(mRightBottom, mRightTop);
                                            Line line4 = Line.CreateBound(mRightTop, mLeftTop);
                                            List<Line> lineList = new List<Line>();
                                            lineList.Add(line1);
                                            lineList.Add(line2);
                                            lineList.Add(line3);
                                            lineList.Add(line4);
                                            SolidCurveIntersectionOptions option = new SolidCurveIntersectionOptions();
                                            foreach (Line line in lineList) {
                                                SolidCurveIntersection section = solid.IntersectWithCurve(line, option);
                                                if (section.Count() > 0) {
                                                    deleteFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                Solid solid2 = obj as Solid;
                                if (solid2 != null) {
                                    deleteFlag = true;
                                }
                            }
                            if (!deleteCheck.Checked) {
                                deleteFlag = false;
                            }

                            if (deleteFlag) {
                                using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                    tran.Start();
                                    Doc.Delete(instance.Id);
                                    tran.Commit();
                                }
                            }
                        }
                    }
                }
                // Yが間隔指定配置
                else {
                    double intervalY = 0;
                    int countY = 0;
                    double widthY = lengthY;
                    double milliY = UnitUtils.Convert(widthY, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    // 入力値チェック
                    int.TryParse(minIntervalY.Text, out int minY);
                    int.TryParse(maxIntervalY.Text, out int maxY);
                    // チェックフラグ
                    checkFlag = false;
                    if (minY != 0 && maxY == 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                            yMessage = Resources.Text.MsgYIntervalMinError;
                        }
                        countY = 1;
                        double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            intervalY = widthY / countY;
                        } while (intervalY >= mMinY);
                        countY--;
                    }
                    else if (minY == 0 && maxY != 0) {
                        countY = 1;
                        double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                        intervalY = 0;
                        do {
                            countY++;
                            intervalY = widthY / countY;
                        } while (intervalY > mMaxY);
                    }
                    else if (minY != 0 && maxY != 0) {
                        if (milliY < minY * 2) {
                            yMesFlag = true;
                            yMessage = Resources.Text.MsgYIntervalMinError;
                        }
                        if (!yMesFlag) {
                            countY = 1;
                            double mMinY = UnitUtils.Convert(minY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                            double mMaxY = UnitUtils.Convert(maxY, UnitTypeId.Millimeters, UnitTypeId.Feet);
                            intervalY = 0;
                            do {
                                countY++;
                                intervalY = widthY / countY;
                                if ((intervalY <= mMaxY && intervalY >= mMinY)) {
                                    checkFlag = true;
                                }
                            } while (intervalY > mMaxY || (intervalY <= mMaxY && intervalY >= mMinY));
                            countY--;
                            if (!checkFlag) {
                                yMesFlag = true;
                                yMessage = Resources.Text.MsgYDirectionError;
                            }
                        }
                    }
                    if (xMesFlag && !yMesFlag) {
                        okFlag = false;
                        ComDialog.ShowDialog(Resources.Text.DialogWarning, TaskDialogIcon.TaskDialogIconError, xMessage, false);
                        return;
                    }
                    else if (!xMesFlag && yMesFlag) {
                        okFlag = false;
                        ComDialog.ShowDialog(Resources.Text.DialogWarning, TaskDialogIcon.TaskDialogIconError, yMessage, false);
                        return;
                    }
                    else if (xMesFlag && yMesFlag) {
                        okFlag = false;
                        ComDialog.ShowDialog(Resources.Text.DialogWarning, TaskDialogIcon.TaskDialogIconError, xMessage + Environment.NewLine + yMessage, false);
                        return;
                    }

                    countY--;
                    double mLengthY = 0;
                    if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                        mLengthY = lengthY / (countY + 1);
                    }
                    else {
                        mLengthY = lengthY / countY;
                    }
                    if (setRad > 0) {
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = 180 - 90 - setAngle;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad);
                        heightX = mLengthX * Math.Sin(setRad);
                        bottomY = mLengthY * Math.Cos(setRad2);
                        heightY = mLengthY * Math.Sin(setRad2);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX - bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX - mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y + mHeightX + mHeightY;
                        }
                    }
                    else if (setRad < 0) {
                        minasFlag = true;
                        increaseX = 0;
                        increaseY = 0;
                        double setAngle2 = setAngle * -1;
                        double setAngle3 = 90 - setAngle2;
                        double setRad2 = setAngle2 * Math.PI / 180;
                        double setRad3 = setAngle3 * Math.PI / 180;
                        bottomX = mLengthX * Math.Cos(setRad2);
                        heightX = mLengthX * Math.Sin(setRad2);
                        bottomY = mLengthY * Math.Cos(setRad3);
                        heightY = mLengthY * Math.Sin(setRad3);
                        double mBottomX = (mLengthX / 2) * Math.Cos(setRad);
                        double mHeightX = (mLengthX / 2) * Math.Sin(setRad);
                        double mBottomY = (mLengthY / 2) * Math.Cos(setRad2);
                        double mHeightY = (mLengthY / 2) * Math.Sin(setRad2);
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + bottomX + bottomY;
                        }
                        else {
                            intervalX = setPoint.X + mBottomX + mBottomY;
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y - heightX + heightY;
                        }
                        else {
                            intervalY = setPoint.Y - mHeightX + mHeightY;
                        }
                    }
                    else {
                        increaseX = mLengthX;
                        increaseY = mLengthY;
                        if (setPatternX.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalX = setPoint.X + increaseX;
                        }
                        else {
                            intervalX = setPoint.X + (mLengthX / 2);
                        }
                        if (setPatternY.SelectedItem.ToString() == Resources.Text.PatternEqualSpacing) {
                            intervalY = setPoint.Y + increaseY;
                        }
                        else {
                            intervalY = setPoint.Y + (mLengthY / 2);
                        }
                    }
                    for (int i = 0; i < countX; i++) {
                        for (int j = 0; j < countY; j++) {
                            XYZ xyz = new XYZ(intervalX + mLengthX * i, intervalY + mLengthY * j, selectLevel.Elevation + offset);
                            if (minasFlag) {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i + bottomY * j, intervalY + increaseY * j - heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            else {
                                xyz = new XYZ(intervalX + increaseX * i + bottomX * i - bottomY * j, intervalY + increaseY * j + heightX * i + heightY * j, selectLevel.Elevation + offset);
                            }
                            FamilyInstance instance = null;
                            faceFlag = false;
                            // 配置範囲
                            PointF p1 = new PointF((float)leftBottom.X, (float)leftBottom.Y);
                            PointF p2 = new PointF((float)leftTop.X, (float)leftTop.Y);
                            PointF p3 = new PointF((float)rightTop.X, (float)rightTop.Y);
                            PointF p4 = new PointF((float)rightBottom.X, (float)rightBottom.Y);
                            KeyValuePair<PointF, PointF> pare1 = new KeyValuePair<PointF, PointF>(p1, p2);
                            KeyValuePair<PointF, PointF> pare2 = new KeyValuePair<PointF, PointF>(p2, p3);
                            KeyValuePair<PointF, PointF> pare3 = new KeyValuePair<PointF, PointF>(p3, p4);
                            KeyValuePair<PointF, PointF> pare4 = new KeyValuePair<PointF, PointF>(p4, p1);
                            List<Element> candidateList = GetHostElementCandidateXY(xyz, levelElement, activeView);
                            Face face = GetFace(xyz, candidateList);
                            List<Element> scCandidate = GetHostElementCandidateXY(xyz, scElement, activeView);
                            Face scFace = GetFaceSlabCeiling(xyz, scCandidate, activeView);
                            if (scFace != null) {
                                face = scFace;
                            }
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                tran.Start();
                                setSymbol.Activate();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.WorkPlaneBased) {
                                    XYZ dir = new XYZ(0, 1, 0);
                                    if (face == null) {
                                        continue;
                                    }
                                    dir = new XYZ(0, 0, 0);
                                    instance = Doc.Create.NewFamilyInstance(face, xyz, dir, setSymbol);
                                    faceFlag = true;
                                }
                                else {
                                    xyz = new XYZ(xyz.X, xyz.Y, offset);
                                    instance = Doc.Create.NewFamilyInstance(xyz, setSymbol, selectLevel, StructuralType.NonStructural);
                                }
                                tran.Commit();
                            }
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionRotateFamily)) {
                                tran.Start();
                                if (fAngle != 0) {
                                    if (!faceFlag) {
                                        Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                        ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                    }
                                    else {
                                        if (scFace != null) {
                                            if (scFace.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)scFace;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, 0), new XYZ(xyz.X, xyz.Y, 1));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                        else {
                                            if (face.GetType().Name == "PlanarFace") {
                                                PlanarFace pf = (PlanarFace)face;
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + pf.FaceNormal.X, xyz.Y + pf.FaceNormal.Y, xyz.Z + pf.FaceNormal.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                            else {
                                                double modifyX = 0;
                                                double modifyY = 0;
                                                XYZ direction = instance.HandOrientation;
                                                double x = 0;
                                                double y = 0;
                                                double sl = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 1;
                                                    y = 0;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    x = 0;
                                                    y = 1;
                                                }
                                                else {
                                                    double vs = -1 / sl;
                                                    x = 1;
                                                    y = 1 * vs;
                                                }
                                                direction = new XYZ(x, y, direction.Z).Normalize();
                                                double sl2 = direction.Y / direction.X;
                                                if (Math.Round(direction.X, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 0;
                                                    modifyY = 1;
                                                }
                                                else if (Math.Round(direction.Y, 3, MidpointRounding.AwayFromZero) == 0) {
                                                    modifyX = 1;
                                                    modifyY = 0;
                                                }
                                                else {
                                                    modifyX = 1;
                                                    modifyY = 1 * sl2;
                                                }
                                                Line axisLine = Line.CreateBound(new XYZ(xyz.X, xyz.Y, xyz.Z), new XYZ(xyz.X + modifyX, xyz.Y + modifyY, xyz.Z));
                                                ElementTransformUtils.RotateElement(Doc, instance.Id, axisLine, fAngle);
                                            }
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // オフセット
                            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionOffset)) {
                                tran.Start();
                                pointList.Add(new PointF((float)xyz.X, (float)xyz.Y));
                                setSymbol.Activate();
                                Family family = setSymbol.Family;
                                FamilyPlacementType placeType = family.FamilyPlacementType;
                                if (placeType == FamilyPlacementType.TwoLevelsBased) {
                                    ParameterSet paraSet = instance.Parameters;
                                    foreach (Parameter para in paraSet) {
                                        if (para.Definition.Name == Resources.Text.ParamBaseOffset) {
                                            para.Set(offset);
                                        }
                                        if (para.Definition.Name == Resources.Text.ParamTopOffset) {
                                            para.Set(offset);
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            // 範囲外の判定
                            bool deleteFlag = false;
                            // 配置点での判定
                            KeyValuePair<PointF, PointF>[] pares = { pare1, pare2, pare3, pare4 };
                            LineData[] lineData = pares.Select(line => new LineData(line)).ToArray();

                            PointF m1 = new PointF((float)xyz.X, (float)xyz.Y);
                            PointF[] points = { m1 };
                            PointData[] pointData = points.Select(p => new PointData(p)).ToArray();
                            foreach (PointData point in pointData) {
                                if (!IsRange(point, lineData)) {
                                    deleteFlag = true;
                                }
                            }
                            // インスタンスのSolidの取得
                            Options opt = new Options();
                            opt.ComputeReferences = true;
                            opt.DetailLevel = ViewDetailLevel.Fine;
                            GeometryElement geo = instance.get_Geometry(opt);
                            foreach (GeometryObject obj in geo) {
                                GeometryInstance geoInstance = obj as GeometryInstance;
                                if (geoInstance != null) {
                                    GeometryElement geo2 = geoInstance.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            BoundingBoxXYZ instanceBox = instance.get_BoundingBox(activeView);
                                            double z = (instanceBox.Max.Z + instanceBox.Min.Z) / 2;
                                            XYZ mLeftBottom = new XYZ(leftBottom.X - 0.001, leftBottom.Y - 0.001, z);
                                            XYZ mLeftTop = new XYZ(leftTop.X - 0.001, leftTop.Y + 0.001, z);
                                            XYZ mRightBottom = new XYZ(rightBottom.X + 0.001, rightBottom.Y - 0.001, z);
                                            XYZ mRightTop = new XYZ(rightTop.X + 0.001, rightTop.Y + 0.001, z);
                                            Line line1 = Line.CreateBound(mLeftTop, mLeftBottom);
                                            Line line2 = Line.CreateBound(mLeftBottom, mRightBottom);
                                            Line line3 = Line.CreateBound(mRightBottom, mRightTop);
                                            Line line4 = Line.CreateBound(mRightTop, mLeftTop);
                                            List<Line> lineList = new List<Line>();
                                            lineList.Add(line1);
                                            lineList.Add(line2);
                                            lineList.Add(line3);
                                            lineList.Add(line4);
                                            SolidCurveIntersectionOptions option = new SolidCurveIntersectionOptions();
                                            foreach (Line line in lineList) {
                                                SolidCurveIntersection section = solid.IntersectWithCurve(line, option);
                                                if (section.Count() > 0) {
                                                    deleteFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                Solid solid2 = obj as Solid;
                                if (solid2 != null) {
                                    deleteFlag = true;
                                }
                            }
                            if (!deleteCheck.Checked) {
                                deleteFlag = false;
                            }

                            if (deleteFlag) {
                                using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionArrayLayout)) {
                                    tran.Start();
                                    Doc.Delete(instance.Id);
                                    tran.Commit();
                                }
                            }
                        }
                    }
                }
            }
            // ファミリ位置
            using (Transaction tran = new Transaction(Doc, Resources.Text.TransactionDeleteLine)) {
                tran.Start();
                if (detailLine != null) {
                    Doc.Delete(detailLine.Id);
                    detailLine = null;
                }
                tran.Commit();
            }
        }

        /// <summary>
        /// -180°より大きく180°以下の角度に変換
        /// </summary>
        /// <param name="angle">変換したい角度</param>
        /// <returns></returns>
        private int CalcProperAngle180(int angle)
        {
            // 指定角度を360°で割った余りを求める
            int result = angle % 360;

            // 余りが180°より大きかったら、360°を差し引いて-180～0°になるよう調整
            if (result > 180)
                result -= 360;

            // 余りが-180°以下だったら、360°を足して0～180°になるよう調整
            else if (result <= -180)
                result += 360;

            return result;
        }

        /// <summary>
        /// -90°より大きく90°以下の角度に変換
        /// </summary>
        /// <param name="angle">変換したい角度</param>
        /// <returns></returns>
        private int CalcProperAngle90(int angle)
        {
            // 指定角度を180°で割った余りを求める
            int result = angle % 180;

            // 余りが90°より大きかったら、180°を差し引いて-90～0°になるよう調整
            if (result > 90)
                result -= 180;

            // 余りが-90°以下だったら、180°を足して0～90°になるよう調整
            else if (result <= -90)
                result += 180;

            return result;
        }
    }
}