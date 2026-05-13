using ADSK.JExtRAC.AutoCreateRoomView;
using ADSK.JExtRAC.AutoCreateRoomView.Common;
using Autodesk.Revit.DB;
using RoomSettings = ADSK.JExtRAC.AutoCreateRoomView.Components.Settings;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;
using TaskDialogIcon = Autodesk.Revit.UI.TaskDialogIcon ;
using View = Autodesk.Revit.DB.View;

namespace ADSK.JExtRAC.AutoCreateRoomView.Screen
{
    /// ================================================================================
    /// <summary>画面 各室ビュー作成</summary>
    /// ================================================================================
    public partial class FormAutoCreateRoomView : Form
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
        /// 平面図タイプ名セット
        /// </summary>
        public SortedSet<string> floorTypeSet;

        /// <summary>
        /// 天井伏図タイプ名セット
        /// </summary>
        public SortedSet<string> ceilingTypeSet;

        /// <summary>
        /// 部屋タグセット
        /// </summary>
        public SortedSet<string> roomTagSet;

        /// <summary>
        /// レベルセット
        /// </summary>
        public List<string> levelSet;

        /// <summary>
        /// ルームセット
        /// </summary>
        public SortedSet<string> roomSet;

        /// <summary>
        /// テンプレート＜なし＞
        /// </summary>
        private string noneTemplate;

        private readonly ADSK.JExtRAC.AutoCreateRoomView.Components.Attribute _cmpAttribute;

        /// <summary>
        /// 平面図タイプ別テンプレート
        /// </summary>
        private Dictionary<ViewFamilyType, string> floorTypeTemplateDic;

        /// <summary>
        /// 天井伏図タイプ別テンプレート
        /// </summary>
        public static Dictionary<ViewFamilyType, string> ceilingTypeTemplateDic;

        /// <summary>
        /// テンプレートディクショナリー
        /// </summary>
        public static Dictionary<ViewPlan, string> viewPlanTemplateDic;

        /// <summary>
        /// 部屋別レベル
        /// </summary>
        public static Dictionary<Room, Level> roomLevelDic;

        /// <summary>
        //　ビューファミリタイプ
        /// </summary>
        public static List<ViewFamilyType> viewFamilyTypes;

        /// <summary>
        /// レベル
        /// </summary>
        public static List<Level> levels;

        /// <summary>
        /// ルームタグ
        /// </summary>
        public static IList<Element> roomTags;

        /// <summary>
        /// 平面図テンプレート名
        /// </summary>
        public static SortedSet<string> floorTemplateNameList = new SortedSet<string>();

        /// <summary>
        /// 天井伏図テンプレート名
        /// </summary>
        public static SortedSet<string> ceilingTemplateNameList = new SortedSet<string>();

        /// <summary>
        /// 選択タイプID
        /// </summary>
        public static ElementId typeId;

        /// <summary>
        /// 選択タグID
        /// </summary>
        public static ElementId tagId;

        /// <summary>
        /// 選択タイプインデックス
        /// </summary>
        public static int typeIndex;

        /// <summary>
        /// 選択タグインデックス
        /// </summary>
        public static int tagIndex;

        /// <summary>
        /// 変更前選択タイプインデックス数
        /// </summary>
        private int preTypeIndexCount;

        /// <summary>
        /// 変更前選択タグインデックス数
        /// </summary>
        private int preTagIndexCount;

        /// <summary>
        /// 変更前選択タイプアイテム
        /// </summary>
        private object preTypeItem;

        /// <summary>
        /// 変更前選択タグインデックス
        /// </summary>
        private object preTagItem;

        /// <summary>
        /// OKフラグ
        /// </summary>
        private bool okFlag;

        /// <summary>
        /// 新しいビューに適用されるテンプレート名
        /// </summary>
        private string newViewApplyTemplate;

        /// <summary>
        /// エラールームリスト
        /// </summary>
        public List<Room> errorRoomList;

        // ビュープランクラス取得
        private List<ViewPlan> viewPlan = new List<ViewPlan>();

        /// <summary>
        /// 外部イベント
        /// </summary>
        private Autodesk.Revit.UI.ExternalEvent m_ExEvent;

        /// <summary>
        /// イベントハンドラー
        /// </summary>
        private ExternalViewCreate m_Handler;

        /// <summary>
        /// 画面のクラス
        /// </summary>
        public static FormAutoCreateRoomView MainWindow;

        /// <summary>
        /// 一時要素ID
        /// </summary>
        public ElementId temId = null;

        /// <summary>
        /// 作成ビューリスト
        /// </summary>
        public List<View> createViews = new List<View>();

        /// <summary>
        /// 初期ビュー
        /// </summary>
        public View preView;

        /// ================================================================================
        /// <summary>コンストラクト</summary>
        /// <param name="commandData">コマンドデータ</param>
        /// <param name="exEvent">外部イベント</param>
        /// <param name="handler">イベントハンドラー</param>
        /// ================================================================================
        public FormAutoCreateRoomView(ExternalCommandData commandData, Autodesk.Revit.UI.ExternalEvent exEvent, ExternalViewCreate handler)
        {
            _cmpAttribute = new ADSK.JExtRAC.AutoCreateRoomView.Components.Attribute();
            noneTemplate = _cmpAttribute.ResourceText("IDS_TXT_NONE_TEMPLATE");
            InitializeComponent();
            SetLocalizedText();

            this.Activate();
            m_ExEvent = exEvent;
            m_Handler = handler;

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;
            App.DocumentChanged += Document_Changed;
            TransactionGroup transGroup = new TransactionGroup(Doc, "配置");

            RoomSettings settings = new RoomSettings(UiDoc);
            SpatialElementBoundaryLocation test = settings.GetRoomAreaComputation();

            // 初期化
            floorTypeSet = new SortedSet<string>();
            ceilingTypeSet = new SortedSet<string>();
            roomTagSet = new SortedSet<string>();
            levelSet = new List<string>();
            roomSet = new SortedSet<string>();
            roomLevelDic = new Dictionary<Room, Level>();
            floorTypeTemplateDic = new Dictionary<ViewFamilyType, string>();
            ceilingTypeTemplateDic = new Dictionary<ViewFamilyType, string>();
            okButton.Click += OkButton_Click;
            offsetText.KeyPress += TextBoxPrice_PreviewTextInput;
            offsetText.MaxLength = 7;
            tagTypeButton.Click += RoomTagTypeEditButton_Click;
            okButton.Enabled = false;
            applyButton.Enabled = false;
            templateButton.Click += TemplateButton_Click;
            viewPlanTemplateDic = new Dictionary<ViewPlan, string>();

            // ビューファミリタイプクラス取得
            viewFamilyTypes = GetElements<ViewFamilyType>(Doc);

            // ビュープランクラス取得
            viewPlan = GetElements<ViewPlan>(Doc);

            // レベルクラスを取得
            levels = GetElements<Level>(Doc);

            //  部屋クラスを取得
            List<SpatialElement> rooms = GetElements<SpatialElement>(Doc);
            rooms.Sort((a, b) => string.Compare(a.Name, b.Name));

            // テンプレート名
            string templateName = "";

            //新しいビューに適用されるビュー テンプレートのID。
            var viewTemplateTypeId = new ForgeTypeId( "autodesk.revit.parameter:defaultViewTemplate-1.0.0" ) ;
            
            // ビュータイプ取得
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {
                // 平面図のタイプを取得
                if (viewFamilyType.ViewFamily == ViewFamily.FloorPlan) {
                    ElementId id = viewFamilyType.DefaultTemplateId;
                    Element ele = Doc.GetElement(id);
                    Element element = (Element)viewFamilyType;
                    floorTypeSet.Add(viewFamilyType.Name);
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                            temId = elementId;
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        floorTypeTemplateDic.Add(viewFamilyType, Doc.GetElement(elementId).Name);
                    }
                    else {
                        floorTypeTemplateDic.Add(viewFamilyType, noneTemplate);
                    }
                }
                // 天井伏図のタイプを取得
                if (viewFamilyType.ViewFamily == ViewFamily.CeilingPlan) {
                    ceilingTypeSet.Add(viewFamilyType.Name);
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        ceilingTypeTemplateDic.Add(viewFamilyType, Doc.GetElement(elementId).Name);
                    }
                    else {
                        ceilingTypeTemplateDic.Add(viewFamilyType, noneTemplate);
                    }
                }
            }

            // タイプコンボボックスにセット
            if (floorTypeSet.Count > 0) {
                foreach (string typeName in floorTypeSet) {
                    viewTypeCombo.Items.Add(typeName);
                }
                viewTypeCombo.SelectedIndex = 0;
            }

            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {

                // 平面図のタイプを取得
                if (viewFamilyType.Name == viewTypeCombo.SelectedItem.ToString()) {
                    ElementId id = viewFamilyType.DefaultTemplateId;
                    Element ele = Doc.GetElement(id);
                    Element element = (Element)viewFamilyType;
                    floorTypeSet.Add(viewFamilyType.Name);
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if ( para.GetTypeId() == viewTemplateTypeId ) {
                            elementId = para.AsElementId();
                            temId = elementId;
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        templateName = Doc.GetElement(elementId).Name;
                    }
                    else {
                        templateName = noneTemplate;
                    }
                }
            }

            // テンプレートをセット
            newViewApplyTemplate = templateName;
            templateCombo.Items.Clear();
            foreach (ViewPlan plan in viewPlan) {
                if (plan.ViewType == ViewType.FloorPlan && plan.Origin == null) {
                    floorTemplateNameList.Add(plan.Name);
                }
                if (plan.ViewType == ViewType.CeilingPlan && plan.Origin == null) {
                    ceilingTemplateNameList.Add(plan.Name);
                }
                if ((plan.ViewType == ViewType.FloorPlan || plan.ViewType == ViewType.CeilingPlan) && plan.Origin == null) {
                    viewPlanTemplateDic.Add(plan, plan.Name);
                }
            }

            floorTemplateNameList.Add(noneTemplate);
            foreach (string name in floorTemplateNameList) {
                templateCombo.Items.Add(name);
            }
            int index = templateCombo.Items.IndexOf(templateName);
            templateCombo.SelectedIndex = index;

            levels.Sort((a, b) => Math.Sign(a.Elevation - b.Elevation));
            foreach (Level level in levels) {
                levelSet.Add(level.Name);
            }

            // タグを取得
            FilteredElementCollector collector = new FilteredElementCollector(UiDoc.Document);
            IList<Element> collection = collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_RoomTags).ToElements();
            roomTags = collection;
            foreach (Element tag in collection) {
                roomTagSet.Add(tag.Name);
            }

            foreach (string name in roomTagSet) {
                tagCombo.Items.Add(name);
            }
            if (roomTagSet.Count > 0) {
                tagCombo.SelectedIndex = 0;
            }
            else {
                // タイプ編集ボタンを非活性
                tagTypeButton.Enabled = false;
            }

            // レベル
            levelListView.Columns.Clear();
            levelListView.Items.Clear();
            levelListView.View = System.Windows.Forms.View.Details;
            levelListView.CheckBoxes = true;
            levelListView.Columns.Add(_cmpAttribute.ResourceText("IDS_TXT_LEVEL"), 200, HorizontalAlignment.Left);
            levelListView.ItemChecked += LevelListItemsChecked_Change;

            foreach (string name in levelSet) {
                levelListView.Items.Add(name);
            }
            levelListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // 部屋
            foreach (SpatialElement room in rooms) {
                if (room.GetType().Name.ToString() == "Room") {
                    Room castRoom = (Room)room;
                    roomSet.Add(room.Name);
                    roomLevelDic.Add(castRoom, room.Level);
                }
            }

            roomListView.Columns.Clear();
            roomListView.Items.Clear();
            roomListView.View = System.Windows.Forms.View.Details;
            roomListView.CheckBoxes = true;
            roomListView.Columns.Add(_cmpAttribute.ResourceText("IDS_TXT_ROOMNAME"), 200, HorizontalAlignment.Left);
            roomListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            roomListView.ItemChecked += RoomListItemsChecked_Change;
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
        /// ビューカテゴリラジオボタンチェンジ時の処理。
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ViewRadio_Change(object sender, EventArgs e)
        {
            //新しいビューに適用されるビュー テンプレートのID。
            var viewTemplateTypeId = new ForgeTypeId( "autodesk.revit.parameter:defaultViewTemplate-1.0.0" ) ;
            
            viewTypeCombo.Items.Clear();
            // 平面図の場合
            if (floorRadio.Checked) {
                if (floorTypeSet.Count > 0) {
                    foreach (string typeName in floorTypeSet) {
                        viewTypeCombo.Items.Add(typeName);
                    }
                    viewTypeCombo.SelectedIndex = 0;
                }
            }
            // 天井伏図の場合
            else {
                if (ceilingTypeSet.Count > 0) {
                    foreach (string typeName in ceilingTypeSet) {
                        viewTypeCombo.Items.Add(typeName);
                    }
                    viewTypeCombo.SelectedIndex = 0;
                }
            }
            // チェックフラグ
            bool checkFlag = false;
            // チェック取得
            foreach (ListViewItem item in roomListView.Items) {
                if (item.Checked) {
                    checkFlag = true;
                }
            }
            // タイプコンボボックスインデックス数
            if (viewTypeCombo.Items.Count == 0) {
                checkFlag = false;
            }
            if (checkFlag) {
                // 活性化
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                // 非活性化
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
            // ビュープランクラス取得
            viewPlan = GetElements<ViewPlan>(Doc);

            // テンプレート名
            string templateName = "";

            // ビュータイプ取得
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {
                var viewFamily = viewFamilyType.ViewFamily ;
                // 平面図のタイプを取得
                if (viewFamily == ViewFamily.FloorPlan && floorRadio.Checked) {
                    ElementId id = viewFamilyType.DefaultTemplateId;
                    Element ele = Doc.GetElement(id);
                    Element element = (Element)viewFamilyType;
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                            temId = elementId;
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        templateName = Doc.GetElement(elementId).Name;
                    }
                    else {
                        templateName = noneTemplate;
                    }
                }
                // 天井伏図のタイプを取得
                if (viewFamily == ViewFamily.CeilingPlan && ceilingRadio.Checked) {
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        templateName = Doc.GetElement(elementId).Name;
                    }
                    else {
                        templateName = noneTemplate;
                    }
                }
            }

            // テンプレートを取得
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {
            }

            // テンプレートをセット
            templateCombo.Items.Clear();
            floorTemplateNameList.Clear();
            ceilingTemplateNameList.Clear();
            viewPlanTemplateDic.Clear();
            foreach (ViewPlan plan in viewPlan) {
                if (plan.ViewType == ViewType.FloorPlan && plan.Origin == null) {
                    floorTemplateNameList.Add(plan.Name);
                }
                if (plan.ViewType == ViewType.CeilingPlan && plan.Origin == null) {
                    ceilingTemplateNameList.Add(plan.Name);
                }
                if ((plan.ViewType == ViewType.FloorPlan || plan.ViewType == ViewType.CeilingPlan) && plan.Origin == null) {
                    viewPlanTemplateDic.Add(plan, plan.Name);
                }
            }

            if (floorRadio.Checked) {
                floorTemplateNameList.Add(noneTemplate);
                foreach (string name in floorTemplateNameList) {
                    templateCombo.Items.Add(name);
                }
                int index = templateCombo.Items.IndexOf(templateName);
                templateCombo.SelectedIndex = index;
            }
            else {
                ceilingTemplateNameList.Add(noneTemplate);
                foreach (string name in ceilingTemplateNameList) {
                    templateCombo.Items.Add(name);
                }
                int index = templateCombo.Items.IndexOf(templateName);
                templateCombo.SelectedIndex = index;
            }
            newViewApplyTemplate = templateName;
        }

        /// <summary>
        /// ビュータイプコンボチェンジ時の処理。
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ViewTypeCombo_Change(object sender, EventArgs e)
        {
            // ビュータイプ名取得
            string typeName = viewTypeCombo.SelectedItem.ToString();
            // テンプレート
            string template = "";
            // 平面図の場合
            if (floorRadio.Checked) {
                foreach (KeyValuePair<ViewFamilyType, string> kvp in floorTypeTemplateDic) {
                    if (kvp.Key.Name == typeName) {
                        template = kvp.Value;
                    }
                }
                int index = templateCombo.Items.IndexOf(template);
                if (index != -1) {
                    templateCombo.SelectedIndex = index;
                }
            }
            // 天井伏図の場合
            else {
                foreach (KeyValuePair<ViewFamilyType, string> kvp in ceilingTypeTemplateDic) {
                    if (kvp.Key.Name == typeName) {
                        template = kvp.Value;
                    }
                }
                int index = templateCombo.Items.IndexOf(template);
                if (index != -1) {
                    templateCombo.SelectedIndex = index;
                }
            }
        }

        /// <summary>
        /// レベルリストチェックチェンジ時の処理。
        /// </summary>
        private void LevelListItemsChecked_Change(object sender, ItemCheckedEventArgs e)
        {
            // チェック済み部屋リスト
            List<string> checkedList = new List<string>();
            foreach (ListViewItem item in roomListView.Items) {
                if (item.Checked) {
                    checkedList.Add(item.Text);
                }
            }
            List<Room> roomList = new List<Room>();
            foreach (ListViewItem listItem in levelListView.Items) {
                if (listItem.Checked) {
                    string text = listItem.Text;
                    foreach (KeyValuePair<Room, Level> kvp in roomLevelDic) {
                        if (kvp.Value.Name == text) {
                            roomList.Add(kvp.Key);
                        }
                    }
                }
            }
            roomListView.Items.Clear();
            foreach (Room room in roomList) {
                string name = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString();
                string number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER)?.AsString();
                if (name != "" && name != null && number != "" && number != null) {
                    roomListView.Items.Add(name + " " + number);
                }
                else if (name != "" && name != null) {
                    roomListView.Items.Add(name);
                }
                else if (number != "" && number != null) {
                    roomListView.Items.Add(number);
                }
                else {
                    roomListView.Items.Add(_cmpAttribute.ResourceText("IDS_TXT_NO_ROOM_NAME"));
                }
            }
            if (roomList.Count > 0) {
                roomListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else {
                roomListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            // 再度チェック
            foreach (ListViewItem item in roomListView.Items) {
                foreach (string str in checkedList) {
                    if (item.Text == str) {
                        item.Checked = true;
                    }
                }
            }
            // チェックフラグ
            bool checkFlag = false;
            // チェック取得
            foreach (ListViewItem item in roomListView.Items) {
                if (item.Checked) {
                    checkFlag = true;
                }
            }
            // タイプコンボボックスインデックス数
            if (viewTypeCombo.Items.Count == 0) {
                checkFlag = false;
            }
            if (checkFlag) {
                // 活性化
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                // 非活性化
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
        }

        /// <summary>
        /// 部屋リストチェックチェンジ時の処理。
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.ItemCheckedEventArgs"/> instance containing the event data.</param>
        private void RoomListItemsChecked_Change(object sender, ItemCheckedEventArgs e)
        {
            // チェックフラグ
            bool checkFlag = false;
            // チェック取得
            foreach (ListViewItem item in roomListView.Items) {
                if (item.Checked) {
                    checkFlag = true;
                }
            }
            // タイプコンボボックスインデックス数
            if (viewTypeCombo.Items.Count == 0) {
                checkFlag = false;
            }
            if (checkFlag) {
                // 活性化
                okButton.Enabled = true;
                applyButton.Enabled = true;
            }
            else {
                // 非活性化
                okButton.Enabled = false;
                applyButton.Enabled = false;
            }
        }

        /// <summary>
        /// OKボタンクリック時の処理。
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            okFlag = true;
            // 外部イベント呼び出し
            string eName = m_Handler.GetName();
            m_ExEvent.Raise();
        }

        /// <summary>
        /// 適用ボタンクリック時の処理。
        /// </summary>
        private void ApplicationButton_Click(object sender, EventArgs e)
        {
            okFlag = false;
            // 外部イベント呼び出し
            string eName = m_Handler.GetName();
            m_ExEvent.Raise();
        }

        /// <summary>
        /// ビュー作成
        /// </summary>
        public void View_Create()
        {
            preView = Doc.ActiveView;
            createViews.Clear();
            errorRoomList = new List<Room>();
            // ビューテンプレート
            string selectTemplateName = templateCombo.SelectedItem.ToString();
            ViewPlan selectViewPlan = null;
            foreach (KeyValuePair<ViewPlan, string> kvp in viewPlanTemplateDic) {
                if (selectTemplateName == kvp.Value) {
                    selectViewPlan = kvp.Key;
                    break;
                }
            }
            // ビューID
            ElementId viewId = null;
            // レベル
            Level level = null;
            // レベルID
            ElementId levelId = null;
            levelId = levels.First().Id;
            //　タイプ名
            string typeName = viewTypeCombo.SelectedItem.ToString();
            // 矩形フラグ
            bool rectangleFlag = true;
            if (shapeCombo.SelectedItem.ToString() == _cmpAttribute.ResourceText("IDS_TXT_RECTANGLE")) {
                rectangleFlag = true;
            }
            else {
                rectangleFlag = false;
            }
            // オフセット
            double offset = 0;
            if (offsetText.Text != "" && rectangleFlag) {
                offset = int.Parse(offsetText.Text);
            }
            else if (offsetText.Text != "" && !rectangleFlag) {
                offset = int.Parse(offsetText.Text) * -1;
            }
            offset = UnitUtils.ConvertToInternalUnits(offset, UnitTypeId.Millimeters);
            // 作成済み時フラグ
            double createdFlag = 0;
            if (skipRadio.Checked) {
                createdFlag = 1;
            }
            else if (recreateRadio.Checked) {
                createdFlag = 2;
            }
            else if (copyRadio.Checked) {
                createdFlag = 3;
            }
            // 作成済みビュー一覧
            List<View> preViewList = GetElements<Autodesk.Revit.DB.View>(Doc);
            List<string> nameList = new List<string>();
            foreach (View vie in preViewList)
            {
                nameList.Add(vie.Name);
            }
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes)
            {
                var viewFamily = viewFamilyType.ViewFamily ;
                // 平面図・天井伏図のIDを取得
                if (((floorRadio.Checked && viewFamily == ViewFamily.FloorPlan) || (ceilingRadio.Checked && viewFamily == ViewFamily.CeilingPlan)) && viewFamilyType.Name == typeName)
                {
                    viewId = viewFamilyType.Id;
                    break;
                }
            }
            foreach (ListViewItem item in roomListView.Items) {
                if (item.Checked) {
                    // 作成済みかどうかのフラグ
                    bool alreadyFlag = false;
                    // 連番
                    int changeNumber = 1;
                    // レベル取得
                    foreach (KeyValuePair<Room, Level> kvp in roomLevelDic) {
                        if (kvp.Key.Name == item.Text) {
                            level = kvp.Value;
                        }
                    }
                    levelId = level.Id;

                    string orgName = typeName + "_" + level.Name + "_" + item.Text;
                    string newName = orgName + "_" + changeNumber;
                    View foundView = preViewList.Find(x => x.Name == orgName);
                    if (null != foundView)
                        alreadyFlag = true;
                    if (alreadyFlag) {
                        if (createdFlag == 1) {
                            continue;
                        }
                        else if (createdFlag == 2) {
                            if (UiDoc.ActiveView.Id == foundView.Id)
                                UiDoc.ActiveView = preView;
                            Doc.Delete(foundView.Id);
                        }
                        else if (createdFlag == 3) {
                            while (true) {
                                if (nameList.Contains(newName))
                                {
                                    changeNumber++;
                                    newName = orgName + "_" + changeNumber;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    ViewPlan createdView = ViewPlan.Create(Doc, viewId, levelId);
                    createdView.Name = (alreadyFlag && createdFlag == 3) ? newName : orgName;
                    List<View> viewList = GetElements<Autodesk.Revit.DB.View>(Doc);
                    View view = viewList.Find(x => x.Name == createdView.Name);
                    if (null != view)
                    {
                        Parameter param = null;
                        // 「ビューをトリミング」にチェック
                        param = view.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION);
                        if (null != param)
                            param.Set(1);
                        // 「トリミング領域を表示」にチェック
                        param = view.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION_VISIBLE);
                        if (null != param)
                            param.Set(1);
                        // ビューテンプレート設定                            
                        param = view.get_Parameter(BuiltInParameter.VIEW_TEMPLATE);
                        if (null != param)
                            param.Set(null == selectViewPlan ? ElementId.InvalidElementId : selectViewPlan.Id);
                    }
                    List<SpatialElement> rooms = GetElements<SpatialElement>(Doc);
                    Room room = rooms.OfType<Room>().First();
                    foreach (SpatialElement spa in rooms) {
                        if (spa.Name == item.Text) {
                            room = (Room)spa;
                            break;
                        }
                    }
                    // 部屋のセグメント取得
                    SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
                    RoomSettings roomSettings = new RoomSettings(UiDoc);
                    opt.SpatialElementBoundaryLocation = roomSettings.GetRoomAreaComputation();
                    IList<IList<Autodesk.Revit.DB.BoundarySegment>> segments = room.GetBoundarySegments(opt);
                    if (segments.Count == 0) {
                        errorRoomList.Add(room);
                        Doc.Delete(view.Id);
                        continue;
                    }
                    // セグメントが取得でき、矩形でない場合
                    if (null != segments && rectangleFlag == false) {
                        createViews.Add(view);
                        foreach (IList<Autodesk.Revit.DB.BoundarySegment> segmentList in segments) {
                            CurveLoop loop = new CurveLoop();
                            CurveLoop loop2 = new CurveLoop();
                            foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in segmentList) {
                                Curve curve = boundarySegment.GetCurve();
                                if (curve.GetType().Name == "Arc") {
                                    rectangleFlag = true;
                                }
                                XYZ p1 = curve.GetEndPoint(0);
                                XYZ p2 = curve.GetEndPoint(1);
                                loop.Append(boundarySegment.GetCurve());
                            }
                            XYZ normal = loop.GetPlane().Normal;
                            loop2 = CurveLoop.CreateViaOffset(loop, offset, normal);
                            ViewCropRegionShapeManager vcrShapeMgr = view.GetCropRegionShapeManager();
                            try {
                                if (rectangleFlag == false) {
                                    vcrShapeMgr.SetCropShape(loop2);
                                }
                            }
                            catch (Exception) {
                            }
                            break;
                        }
                    }
                    // 矩形の場合
                    if (rectangleFlag) {
                        Doc.Regenerate();
                        BoundingBoxXYZ rB = room.get_BoundingBox(view);
                        if (rB == null) {
                            errorRoomList.Add(room);
                            Doc.Delete(view.Id);
                            continue;
                        }
                        createViews.Add(view);
                        XYZ maxXYZ = new XYZ(rB.Max.X + offset, rB.Max.Y + offset, 0);
                        XYZ minXYZ = new XYZ(rB.Min.X - offset, rB.Min.Y - offset, 0);
                        XYZ vertexA = new XYZ(minXYZ.X, maxXYZ.Y, 0);
                        XYZ vertexB = new XYZ(maxXYZ.X, minXYZ.Y, 0);
                        Curve curve1 = Line.CreateBound(
                            new XYZ(minXYZ.X, minXYZ.Y, 0),
                            new XYZ(vertexA.X, vertexA.Y, 0));
                        Curve curve2 = Line.CreateBound(
                            new XYZ(vertexA.X, vertexA.Y, 0),
                            new XYZ(maxXYZ.X, maxXYZ.Y, 0));
                        Curve curve3 = Line.CreateBound(
                            new XYZ(maxXYZ.X, maxXYZ.Y, 0),
                            new XYZ(vertexB.X, vertexB.Y, 0));
                        Curve curve4 = Line.CreateBound(
                            new XYZ(vertexB.X, vertexB.Y, 0),
                            new XYZ(minXYZ.X, minXYZ.Y, 0));
                        CurveLoop loop = new CurveLoop();
                        loop.Append(curve1);
                        loop.Append(curve2);
                        loop.Append(curve3);
                        loop.Append(curve4);
                        ViewCropRegionShapeManager vcrShapeMgr = view.GetCropRegionShapeManager();
                        vcrShapeMgr.SetCropShape(loop);
                        // オフセット設定
                        vcrShapeMgr.TopAnnotationCropOffset = offset;
                        vcrShapeMgr.BottomAnnotationCropOffset = offset;
                        vcrShapeMgr.RightAnnotationCropOffset = offset;
                        vcrShapeMgr.LeftAnnotationCropOffset = offset;
                    }
                    // ルームタグ
                    XYZ cen = GetRoomCenter(room, view);
                    UV center = new UV(cen.X, cen.Y);
                    RoomTag tag = Doc.Create.NewRoomTag(new LinkElementId(room.Id), center, view.Id);
                    foreach (Element element in roomTags) {
                        string tagName = tagCombo.SelectedItem.ToString();
                        if (tagName == element.Name) {
                            tag.ChangeTypeId(element.Id);
                            break;
                        }
                    }
                }
            }
            // エラーメッセージ
            string errorMes = _cmpAttribute.ResourceText("IDS_ERR_ROOM_NOT_ENCLOSED");
            foreach (Room eRoom in errorRoomList) {
                errorMes += (Environment.NewLine + eRoom.Name);
            }
            if (errorRoomList.Count > 0) {
                ComDialog.ShowDialog(_cmpAttribute.ResourceText("IDS_TXT_WARNING"), TaskDialogIcon.TaskDialogIconWarning, errorMes, false);
            }
            if (okFlag) {
                // 画面を閉じる
                Close();
            }
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            // 画面を閉じる
            Close();
        }

        /// <summary>
        /// タイプ編集ボタンクリック時の処理
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TypeEditButton_Click(object sender, EventArgs e)
        {
            typeId = null;
            // コンボボックスの選択状態を取得
            string typeName = viewTypeCombo.SelectedItem.ToString();
            // ビューファミリタイプクラス取得
            viewFamilyTypes = GetElements<ViewFamilyType>(Doc);
            // ビュータイプ取得
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {
                if (viewFamilyType.Name == typeName) {
                    typeId = viewFamilyType.Id;
                }
            }
            if (typeId == null) {
                return;
            }
            UiDoc.Selection.SetElementIds(new[] { typeId });
            RevitCommandId commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.TypeProperties);
            if (UiApp.CanPostCommand(commandId)) {
                try {
                    UiApp.PostCommand(commandId);
                }
                catch (Exception) {
                }
            }
        }

        /// <summary>
        /// タイプ編集ボタンクリック時の処理
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RoomTagTypeEditButton_Click(object sender, EventArgs e)
        {
            tagId = null;
            // コンボボックスの選択状態を取得
            string tagName = tagCombo.SelectedItem.ToString();
            // タグを取得
            FilteredElementCollector collector = new FilteredElementCollector(UiDoc.Document);
            IList<Element> collection = collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_RoomTags).ToElements();
            roomTags = collection;
            foreach (Element tag in collection) {
                if (tagName == tag.Name) {
                    tagId = tag.Id;
                }
            }
            if (tagId == null) {
                return;
            }
            // タイププロパティを表示
            UiDoc.Selection.SetElementIds(new[] { tagId });
            RevitCommandId commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.TypeProperties);
            if (UiApp.CanPostCommand(commandId)) {
                try {
                    UiApp.PostCommand(commandId);
                }
                catch (Exception) {
                }
            }
            this.Activate();
        }

        /// <summary>
        /// テンプレート管理ボタンクリック時の処理
        /// </summary>
        /// <param name="sender">イベントを送信したオブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void TemplateButton_Click(object sender, EventArgs e)
        {
            // ビューテンプレート管理画面を表示
            RevitCommandId commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.ManageViewTemplates);
            if (UiApp.CanPostCommand(commandId)) {
                try {
                    UiApp.PostCommand(commandId);
                }
                catch (Exception) {
                }
            }
        }

        /// <summary>
        /// ドキュメント変更時の処理
        /// </summary>
        /// <param name="sender">イベントを送信したオブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Document_Changed(object sender, EventArgs e)
        {
            // 変更前タイプ名
            List<string> preTypeNameList = new List<string>();
            foreach (object item in viewTypeCombo.Items) {
                preTypeNameList.Add(item.ToString());
            }
            // 変更前テンプレート名
            List<string> preTemplateList = new List<string>();
            foreach (object item in templateCombo.Items) {
                preTemplateList.Add(item.ToString());
            }
            // 変更前タグ名
            List<string> preTagList = new List<string>();
            foreach (object item in tagCombo.Items) {
                preTagList.Add(item.ToString());
            }
            // テンプレート名
            string templateName = "";
            // 変更後タイプ名
            string newTypeName = "";
            // 変更後タイプ名
            string newTemplateName = "";
            // 変更後タグ名
            string newTagName = "";
            object senr = sender;
            // 選択インデックスを取得
            typeIndex = viewTypeCombo.SelectedIndex;
            tagIndex = tagCombo.SelectedIndex;
            int temIn = templateCombo.SelectedIndex;
            // 変更前インデックス数
            preTypeIndexCount = viewTypeCombo.Items.Count;
            preTagIndexCount = tagCombo.Items.Count;
            // 変更前アイテム
            preTypeItem = viewTypeCombo.SelectedItem;
            object preTemplateName = templateCombo.SelectedItem;
            preTagItem = tagCombo.SelectedItem;
            // ビューファミリタイプクラス取得
            viewFamilyTypes = GetElements<ViewFamilyType>(Doc);

            // ビュープランクラス取得
            List<ViewPlan> viewPlan = GetElements<ViewPlan>(Doc);

            //新しいビューに適用されるビュー テンプレートのID。
            var viewTemplateTypeId = new ForgeTypeId( "autodesk.revit.parameter:defaultViewTemplate-1.0.0" ) ;
            
            // 初期化
            floorTypeSet = new SortedSet<string>();
            ceilingTypeSet = new SortedSet<string>();
            floorTypeTemplateDic = new Dictionary<ViewFamilyType, string>();
            ceilingTypeTemplateDic = new Dictionary<ViewFamilyType, string>();
            roomTagSet = new SortedSet<string>();
            tagCombo.Items.Clear();
            viewTypeCombo.Items.Clear();
            floorTemplateNameList = new SortedSet<string>();
            ceilingTemplateNameList = new SortedSet<string>();

            // ビュータイプ取得
            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {
                var viewFamily = viewFamilyType.ViewFamily ;
                // 平面図のタイプを取得
                if (viewFamily == ViewFamily.FloorPlan) {
                    ElementId id = viewFamilyType.DefaultTemplateId;
                    Element ele = Doc.GetElement(id);
                    Element element = (Element)viewFamilyType;
                    floorTypeSet.Add(viewFamilyType.Name);
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                            temId = elementId;
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        floorTypeTemplateDic.Add(viewFamilyType, Doc.GetElement(elementId).Name);
                    }
                    else {
                        floorTypeTemplateDic.Add(viewFamilyType, noneTemplate);
                    }
                }
                // 天井伏図のタイプを取得
                if (viewFamily == ViewFamily.CeilingPlan) {
                    ceilingTypeSet.Add(viewFamilyType.Name);
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        ceilingTypeTemplateDic.Add(viewFamilyType, Doc.GetElement(elementId).Name);
                    }
                    else {
                        ceilingTypeTemplateDic.Add(viewFamilyType, noneTemplate);
                    }
                }
            }

            // タイプコンボボックスにセット
            if (floorRadio.Checked) {
                foreach (string name in floorTypeSet) {
                    if (!preTypeNameList.Contains(name)) {
                        newTypeName = name;
                    }
                }
                foreach (string typeName in floorTypeSet) {
                    viewTypeCombo.Items.Add(typeName);
                }
                if (viewTypeCombo.Items.IndexOf(preTypeItem) != -1) {
                    int ind = viewTypeCombo.Items.IndexOf(preTypeItem);
                    viewTypeCombo.SelectedIndex = ind;
                }
                else if (viewTypeCombo.Items.IndexOf(newTypeName) != -1) {
                    int ind = viewTypeCombo.Items.IndexOf(newTypeName);
                    viewTypeCombo.SelectedIndex = ind;
                }
                else {
                    viewTypeCombo.SelectedIndex = 0;
                }
            }
            else {
                foreach (string name in ceilingTypeSet) {
                    if (!preTypeNameList.Contains(name)) {
                        newTypeName = name;
                    }
                }
                foreach (string typeName in ceilingTypeSet) {
                    viewTypeCombo.Items.Add(typeName);
                }
                if (viewTypeCombo.Items.IndexOf(preTypeItem) != -1) {
                    int ind = viewTypeCombo.Items.IndexOf(preTypeItem);
                    viewTypeCombo.SelectedIndex = ind;
                }
                else if (viewTypeCombo.Items.IndexOf(newTypeName) != -1) {
                    int ind = viewTypeCombo.Items.IndexOf(newTypeName);
                    viewTypeCombo.SelectedIndex = ind;
                }
                else {
                    viewTypeCombo.SelectedIndex = 0;
                }
            }

            foreach (ViewFamilyType viewFamilyType in viewFamilyTypes) {

                if (viewFamilyType.Name == viewTypeCombo.SelectedItem.ToString()) {
                    ElementId id = viewFamilyType.DefaultTemplateId;
                    Element ele = Doc.GetElement(id);
                    Element element = (Element)viewFamilyType;
                    ParameterSet parameters = viewFamilyType.Parameters;
                    ElementId elementId = null;
                    foreach (Parameter para in parameters) {
                        if (para.GetTypeId() == viewTemplateTypeId) {
                            elementId = para.AsElementId();
                            temId = elementId;
                        }
                    }
                    if (Doc.GetElement(elementId) != null) {
                        templateName = Doc.GetElement(elementId).Name;
                    }
                    else {
                        templateName = noneTemplate;
                    }
                }
            }

            // テンプレートをセット
            templateCombo.Items.Clear();
            foreach (ViewPlan plan in viewPlan) {
                if (plan.ViewType == ViewType.FloorPlan && plan.Origin == null) {
                    floorTemplateNameList.Add(plan.Name);
                }
                if (plan.ViewType == ViewType.CeilingPlan && plan.Origin == null) {
                    ceilingTemplateNameList.Add(plan.Name);
                }
                if ((plan.ViewType == ViewType.FloorPlan || plan.ViewType == ViewType.CeilingPlan) && plan.Origin == null) {
                    viewPlanTemplateDic.Add(plan, plan.Name);
                }
            }
            if (floorRadio.Checked) {
                floorTemplateNameList.Add(noneTemplate);
                foreach (string name in floorTemplateNameList) {
                    templateCombo.Items.Add(name);
                }
                if (newViewApplyTemplate != templateName) {
                    int index = templateCombo.Items.IndexOf(templateName);
                    if (index == -1) {
                        index = 0;
                    }
                    templateCombo.SelectedIndex = index;
                }
                else {
                    foreach (string name in floorTemplateNameList) {
                        if (!preTemplateList.Contains(name)) {
                            newTemplateName = name;
                        }
                    }
                    if (templateCombo.Items.IndexOf(preTemplateName) != -1) {
                        int index = templateCombo.Items.IndexOf(preTemplateName);
                        templateCombo.SelectedIndex = index;
                    }
                    else if (templateCombo.Items.IndexOf(newTemplateName) != -1) {
                        int index = templateCombo.Items.IndexOf(newTemplateName);
                        templateCombo.SelectedIndex = index;
                    }
                    else {
                        templateCombo.SelectedIndex = 0;
                    }
                }
            }
            else {
                ceilingTemplateNameList.Add(noneTemplate);
                foreach (string name in ceilingTemplateNameList) {
                    templateCombo.Items.Add(name);
                }
                if (newViewApplyTemplate != templateName) {
                    int index = templateCombo.Items.IndexOf(templateName);
                    if (index == -1) {
                        index = 0;
                    }
                    templateCombo.SelectedIndex = index;
                }
                else {
                    foreach (string name in ceilingTemplateNameList) {
                        if (!preTemplateList.Contains(name)) {
                            newTemplateName = name;
                        }
                    }
                    if (templateCombo.Items.IndexOf(preTemplateName) != -1) {
                        int index = templateCombo.Items.IndexOf(preTemplateName);
                        templateCombo.SelectedIndex = index;
                    }
                    else if (templateCombo.Items.IndexOf(newTemplateName) != -1) {
                        int index = templateCombo.Items.IndexOf(newTemplateName);
                        templateCombo.SelectedIndex = index;
                    }
                    else {
                        templateCombo.SelectedIndex = 0;
                    }
                }
            }
            newViewApplyTemplate = templateName;

            // タグを取得
            FilteredElementCollector collector = new FilteredElementCollector(UiDoc.Document);
            IList<Element> collection = collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_RoomTags).ToElements();
            roomTags = collection;
            foreach (Element tag in collection) {
                roomTagSet.Add(tag.Name);
            }
            foreach (string name in roomTagSet) {
                if (!preTagList.Contains(name)) {
                    newTagName = name;
                }
            }

            foreach (string name in roomTagSet) {
                tagCombo.Items.Add(name);
            }
            if (roomTagSet.Count > 0) {
                if (tagCombo.Items.IndexOf(preTagItem) != -1) {
                    int index = tagCombo.Items.IndexOf(preTagItem);
                    tagCombo.SelectedIndex = index;
                }
                else if (tagCombo.Items.IndexOf(newTagName) != -1) {
                    int index = tagCombo.Items.IndexOf(newTagName);
                    tagCombo.SelectedIndex = index;
                }
                else {
                    tagCombo.SelectedIndex = 0;
                }
            }
            else {
                // タイプ編集ボタンを非活性
                tagTypeButton.Enabled = false;
            }
        }

        /// <summary>
        /// 部屋の中心を取得
        /// </summary>
        /// <param name="room">部屋</param>
        /// <param name="view">ビュー</param>
        /// <returns>部屋の中心</returns>
        public static XYZ GetRoomCenter(Room room, View view)
        {
            XYZ boundCenter = GetElementCenter(room, view);
            LocationPoint locPt = (LocationPoint)room.Location;
            XYZ roomCenter = new XYZ(boundCenter.X, boundCenter.Y, locPt.Point.Z);
            return roomCenter;
        }

        /// <summary>
        /// 要素の中心を取得
        /// </summary>
        /// <returns>要素の中心</returns>
        public static XYZ GetElementCenter(Element elem, View view)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(view);
            XYZ center = (bounding.Max + bounding.Min) * 0.5;
            return center;
        }

        /// <summary>
        /// 数値のみ入力を許可する
        /// </summary>
        /// <param name="sender">イベントを送信したオブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void TextBoxPrice_PreviewTextInput(object sender, KeyPressEventArgs e)
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

        private void SetLocalizedText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_FORM_TITLE");
            label1.Text = _cmpAttribute.ResourceText("IDS_TXT_SELECT_VIEW_CATEGORY");
            floorRadio.Text = _cmpAttribute.ResourceText("IDS_TXT_FLOOR_PER_ROOM");
            ceilingRadio.Text = _cmpAttribute.ResourceText("IDS_TXT_CEILING_PER_ROOM");
            groupBox3.Text = _cmpAttribute.ResourceText("IDS_TXT_VIEW_TYPE");
            viewTypeButton.Text = _cmpAttribute.ResourceText("IDS_TXT_TYPE_EDIT");
            groupBox1.Text = _cmpAttribute.ResourceText("IDS_TXT_TAG");
            tagTypeButton.Text = _cmpAttribute.ResourceText("IDS_TXT_TYPE_EDIT");
            cancelButton.Text = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            label2.Text = _cmpAttribute.ResourceText("IDS_TXT_OFFSET") + "(" + _cmpAttribute.ResourceText("IDS_TXT_MM") + ")";
            label3.Text = _cmpAttribute.ResourceText("IDS_TXT_SHAPE");
            shapeCombo.Items.Clear();
            shapeCombo.Items.Add(_cmpAttribute.ResourceText("IDS_TXT_RECTANGLE"));
            shapeCombo.Items.Add(_cmpAttribute.ResourceText("IDS_TXT_ROOM_BOUNDARY"));
            shapeCombo.SelectedIndex = 0;
            okButton.Text = _cmpAttribute.ResourceText("IDS_TXT_OK");
            applyButton.Text = _cmpAttribute.ResourceText("IDS_TXT_APPLY");
            groupBox4.Text = _cmpAttribute.ResourceText("IDS_TXT_DUPLICATE_NAME");
            copyRadio.Text = _cmpAttribute.ResourceText("IDS_TXT_COPY_CREATE");
            recreateRadio.Text = _cmpAttribute.ResourceText("IDS_TXT_OVERWRITE");
            skipRadio.Text = _cmpAttribute.ResourceText("IDS_TXT_SKIP");
            groupBox2.Text = _cmpAttribute.ResourceText("IDS_TXT_TRIM");
            groupBox5.Text = _cmpAttribute.ResourceText("IDS_TXT_VIEW_TEMPLATE");
            templateButton.Text = _cmpAttribute.ResourceText("IDS_TXT_MANAGE_TEMPLATES");
        }
    }
}