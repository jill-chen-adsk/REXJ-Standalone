using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AutoLayoutTag.UI
{
    /// ================================================================================
    /// <summary>Form setting</summary>
    /// <history>2021/12/11 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormConfig : System.Windows.Forms.Form
    {
        // Member variables

        #region Member Variables

        /// <summary>attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>attribute</summary>
        private RvtExtApp.Components.Elements _cmpElements;

        /// <summary>attribute</summary>
        private RvtExtApp.Entities.DtTag _EntDtTag;

        /// <summary> is select object</summary>
        public bool _isSelectObject;

        /// <summary> is set area</summary>
        public bool _isSetArea;

        /// <summary> is object</summary>
        public bool _isObject;

        /// <summary>list index status</summary>
        private List<int> listIndex = new List<int>();

        /// <summary>dictionary data category status</summary>
        private Collections.Generic.Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>> dicCat = new Collections.Generic.Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>>();

        /// <summary>Distance offset</summary>
        private const double distance = 1000;

        /// <summary>保存される設定情報</summary>
        private RvtExtApp.Utils.Viewtemplate savedSetting = null;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute">Attribute</param>
        /// <param name="cmpElements">Element</param>
        /// <param name="entDtTag">DtTag</param>
        /// <param name="entDtCmd">DtCmd</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public FormConfig(RvtExtApp.Components.Attribute cmpAttribute, RvtExtApp.Components.Elements cmpElements, RvtExtApp.Entities.DtTag entDtTag)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _cmpElements = cmpElements;
            _EntDtTag = entDtTag;
            SetText();
            SetData();
            _EntDtTag.NumberShow += 1;
        }

        #endregion Constructor

        // Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Set text</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_AUTOMATIC_TAG");
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            this.lblSelectionNumber.Text = "( " + _CmpAttribute.ResourceText("IDS_TXT_SELECTION_NUMBER") + _EntDtTag.LstElement.Count.ToString() + " )";

            this.btnSetTag.Text = _CmpAttribute.ResourceText("IDS_TXT_SET_TAG");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.tabSettingCondition.Text = _CmpAttribute.ResourceText("IDS_TXT_SETTING_CONDITION");
            this.tabSettingTag.Text = _CmpAttribute.ResourceText("IDS_TXT_SETTING_TAG");

            this.gpbObject.Text = _CmpAttribute.ResourceText("IDS_TXT_OBJECT");
            this.rdbSelectObject.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_OBJECT");
            this.rdbAllCategory.Text = _CmpAttribute.ResourceText("IDS_TXT_ALL_CATEGORY");
            this.btnSelectObject.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT");

            this.gpbPosittionTag.Text = _CmpAttribute.ResourceText("IDS_TXT_POSITTION_TAG");
            this.cbkLeftRight.Text = _CmpAttribute.ResourceText("IDS_TXT_LEFT_RIGHT");
            this.cbkTopBottom.Text = _CmpAttribute.ResourceText("IDS_TXT_TOP_BOTTOM");

            this.gpbLearderLine.Text = _CmpAttribute.ResourceText("IDS_TXT_LEADER");
            this.rdbHasLeader.Text = _CmpAttribute.ResourceText("IDS_TXT_HAS_LEADER");
            this.rdbNoLeader.Text = _CmpAttribute.ResourceText("IDS_TXT_NO_LEADER");

            this.gpbAreaPremises.Text = _CmpAttribute.ResourceText("IDS_TXT_AREA_PREMISES");
            this.rdbAutoJudgment.Text = _CmpAttribute.ResourceText("IDS_TXT_AUTO_JUDGMENT");
            this.rdbSetByHand.Text = _CmpAttribute.ResourceText("IDS_TXT_SET_BY_HAND");
            this.btnSetArea.Text = _CmpAttribute.ResourceText("IDS_TXT_SET_AREA");

            this.gpbHandlePresetTag.Text = _CmpAttribute.ResourceText("IDS_TXT_HAND_PRESET_TAG");
            this.rdbOnlyNewTag.Text = _CmpAttribute.ResourceText("IDS_TXT_ONLY_NEW_TAG");
            this.rdbReset.Text = _CmpAttribute.ResourceText("IDS_TXT_RESET");
            this.rdbOderMore.Text = _CmpAttribute.ResourceText("IDS_TXT_ODER_MORE");

            // Tag setting tab
            this.lblViewTemplate.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEW_TEMPLATE");
            this.btnSaveSetting.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVE_SETTING");
            ElementId viewTemplateId = _cmpElements.RvtDBDoc.ActiveView.ViewTemplateId;
            Element element = _cmpElements.RvtDBDoc.GetElement(viewTemplateId);
            if (element != null)
                this.lblValue.Text = element.Name;
            else
                this.lblValue.Text = _CmpAttribute.ResourceText("IDS_TXT_NONE");
        }

        /// ================================================================================
        /// <summary>Set Data</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetData()
        {
            // Add data to data grid view
            foreach (var data in _cmpElements.DataCategory().Rows)
            {
                System.Data.DataRow row = data as System.Data.DataRow;
                var eleId = row.ItemArray[1];
                AddDataDgvCategory(this.dgvCategory, (BuiltInCategory)eleId);
            }

            // Get data from view template
            if (_EntDtTag.NumberShow == 0)
            {
                GetDataFromViewTemplate();
            }
            else
            {
                DefaultValue();

                // Reload the form
                SetValueDgv(this.dgvCategory, _EntDtTag.LstBuiltInCategory);
            }
        }

        /// ================================================================================
        /// <summary>Set value dgvCategory previous</summary>
        ///
        /// <param name="dgv"></param>
        /// <param name="LstbuiltIns"></param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetValueDgv(DataGridView dgv, List<Revit.DB.BuiltInCategory> LstbuiltIns)
        {
            if (dgv.Rows.Count == 0 || LstbuiltIns == null || LstbuiltIns.Count == 0)
                return;

            foreach (var builtIn in LstbuiltIns)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if ((BuiltInCategory)this.dgvCategory.Rows[i].Tag == builtIn)
                    {
                        this.dgvCategory.Rows[i].Cells[0].Value = true;
                        break;
                    }
                }
            }

            // Add data to dgvSettings
            GetValueToSetting();
        }

        /// ================================================================================
        /// <summary>Get OutLine From Point</summary>
        ///
        /// <param name="point1">point first</param>
        /// <param name="point2">point second</param>
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private Outline GetOutLineFromPoint(XYZ point1, XYZ point2)
        {
            Outline retval = null;

            if (point1 == null || point2 == null || point1 == point2)
                return null;

            double minX = point1.X;
            double minY = point1.Y;

            double maxX = point2.X;
            double maxY = point2.Y;

            double sminX = System.Math.Min(minX, maxX);
            double sminY = System.Math.Min(minY, maxY);

            double smaxX = System.Math.Max(minX, maxX);
            double smaxY = System.Math.Max(minY, maxY);

            retval = new Outline(new XYZ(sminX, sminY, -distance), new XYZ(smaxX, smaxY, distance));
            return retval;
        }

        /// ================================================================================
        /// <summary>Get data from view template</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void GetDataFromViewTemplate()
        {
            try
            {
                // Read information of view template
                var stringJsonValue = ADSK.JExtRAC.AutoLayoutTag.Utils.StorageUtility.GetValue(_cmpElements.RvtDBDoc.ProjectInformation, _CmpAttribute.ResourceText("IDS_TXT_GUID"), _CmpAttribute.ResourceText("IDS_TXT_FIELD_SETTING"), typeof(string));
                if (stringJsonValue == null || string.IsNullOrEmpty(stringJsonValue.ToString()))
                {
                    DefaultValue();
                }
                else
                {
                    RvtExtApp.Utils.Root listViewTemplate = JsonConvert.DeserializeObject<RvtExtApp.Utils.Root>(stringJsonValue.ToString());
                    if (listViewTemplate == null || listViewTemplate.ViewTemplates.Count == 0)
                    {
                        DefaultValue();
                        return;
                    }

                    // Get view template of active view
                    ElementId viewTempId = _cmpElements.RvtDBDoc.ActiveView.ViewTemplateId;

                    // Get value view template from saved Json 
                    RvtExtApp.Utils.Viewtemplate viewtemplate = null;
                    if (viewTempId.ToString() == ElementId.InvalidElementId.ToString())
                        viewtemplate = listViewTemplate.ViewTemplates.FindLast(x => x.ViewTemplateId.ToString() == ElementId.InvalidElementId.ToString());
                    else
                        viewtemplate = listViewTemplate.ViewTemplates.FindLast(x => x.ViewTemplateId.ToString() == viewTempId.ToString());

                    if (viewtemplate == null)
                    {
                        savedSetting = null;
                        // Set default value
                        DefaultValue();
                        return;
                    }
                    string valueString = "";
                    savedSetting = viewtemplate;

                    // Get object
                    RdbGetObject = viewtemplate.TargetObjectType;
                    SelectObject();

                    // Set check box left right / top bottom
                    RvtExtApp.Utils.TagPosition tagPosition = viewtemplate.TagPosition;
                    valueString = tagPosition.LeftRight.ToString();
                    if (!string.IsNullOrEmpty(valueString))
                    {
                        if (valueString == "True")
                            this.cbkLeftRight.Checked = true;
                        else
                            this.cbkLeftRight.Checked = false;
                    }
                    valueString = tagPosition.TopBottom.ToString();
                    if (!string.IsNullOrEmpty(valueString))
                    {
                        if (valueString == "True")
                            this.cbkTopBottom.Checked = true;
                        else
                            this.cbkTopBottom.Checked = false;
                    }

                    // Tag leader
                    RdbTagLeader = viewtemplate.TagLeader;

                    // Option area premises
                    RdbAreaPremises = viewtemplate.TagPlacingMethod.PlacingMethodType;
                    SetAreaPremises();

                    if (RdbAreaPremises == 1)
                    {
                        // Get point user pick box
                        XYZ point1 = new XYZ();
                        XYZ point2 = new XYZ();
                        string valuePoint1 = viewtemplate.TagPlacingMethod.Point1;
                        if (!string.IsNullOrEmpty(valuePoint1))
                        {
                            point1 = GetPointFromString(valuePoint1);
                        }
                        string valuePoint2 = viewtemplate.TagPlacingMethod.Point2;
                        if (!string.IsNullOrEmpty(valuePoint2))
                        {
                            point2 = GetPointFromString(valuePoint2);
                        }
                        _EntDtTag.OutLine = GetOutLineFromPoint(point1, point2);
                    }

                    // Option handle preset tag
                    RdbHandlePresetTag = viewtemplate.ExistedTagProcessingType;
                    _EntDtTag.LstElement = new List<Element>();
                    if (RdbGetObject == 0)
                    {
                        foreach (var idEle in viewtemplate.TargetObjectIds)
                        {
                            Element ele = _cmpElements.GetElementDoc(idEle);
                            if (ele == null)
                                continue;
                            _EntDtTag.LstElement.Add(ele);
                        }

                        // Add data to dgvSaveSettings
                        List<Utils.TagFamilyType> listtagFamilyType = viewtemplate.TagFamilyType;
                        AddDataToGridViewFromJson(listtagFamilyType, false);
                        this.lblSelectionNumber.Text = "( " + _CmpAttribute.ResourceText("IDS_TXT_SELECTION_NUMBER") + _EntDtTag.LstElement.Count.ToString() + " )";
                    }
                    else
                    {
                        // Add data to dgvCategory and dgvSaveSettings
                        List<Utils.TagFamilyType> listtagFamilyType = viewtemplate.TagFamilyType;
                        AddDataToGridViewFromJson(listtagFamilyType, true);
                    }

                    // Get data
                    GetData();
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return;
            }
        }

        /// ================================================================================
        /// <summary>Get data</summary>
        ///
        /// <param name="listtagFamilyType"> list element data</param>
        /// <param name="isAddDgvCategory"></param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void AddDataToGridViewFromJson(List<Utils.TagFamilyType> listtagFamilyType, bool isAddDgvCategory)
        {
            if (listtagFamilyType == null || listtagFamilyType.Count == 0)
                return;

            int index = 0;
            try
            {
                Collections.Generic.List<FamilySymbol> listSymbol = new List<FamilySymbol>();
                for (int i = 0; i < listtagFamilyType.Count; i++)
                {
                    // Get type tag
                    Utils.TagFamilyType tagFamilyType = listtagFamilyType[i] as Utils.TagFamilyType;
                    BuiltInCategory built = tagFamilyType.CategoryId;
                    if (((int)built).ToString() == ElementId.InvalidElementId.ToString())
                        continue;

                    Category category = _cmpElements.GetCategory(built);

                    if (isAddDgvCategory)
                    {
                        for (int j = 0; j < this.dgvCategory.Rows.Count; j++)
                        {
                            if ((BuiltInCategory)this.dgvCategory.Rows[j].Tag == built)
                            {
                                this.dgvCategory.Rows[j].Cells[0].Value = true;
                                break;
                            }
                        }
                    }
                    index = dgvSaveSetting.Rows.Add();
                    dgvSaveSetting.Rows[index].Cells[0].Value = _cmpElements.GetCategoryName((BuiltInCategory)built);
                    DataGridViewComboBoxCell tagColumn = (DataGridViewComboBoxCell)dgvSaveSetting.Rows[index].Cells[1];
                    listSymbol = _cmpElements.GetAllType(_cmpElements.RvtDBDoc, category);
                    tagColumn.DataSource = listSymbol;
                    tagColumn.DisplayMember = "Name";
                    tagColumn.ValueMember = "Id";

                    Element ele = _cmpElements.GetElementDoc(tagFamilyType.TagTypeId);
                    if (ele != null)
                    {
                        ElementId typeId = ele.Id;
                        if (typeId != null)
                            tagColumn.Value = typeId;
                    }
                    else
                        tagColumn.Value = listSymbol.Select(x => x.Id).FirstOrDefault();

                    dgvSaveSetting.Rows[index].Tag = built;
                    index++;
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Get data</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void GetData()
        {
            _EntDtTag.GetObjectOpt = RdbGetObject;
            _EntDtTag.ChkLeftRight = this.cbkLeftRight.Checked;
            _EntDtTag.ChkTopBottom = this.cbkTopBottom.Checked;
            _EntDtTag.AreaPremisesOpt = RdbAreaPremises;
            _EntDtTag.HandlePresetTagOpt = RdbHandlePresetTag;
            _EntDtTag.TagLeaderOtp = RdbTagLeader;

            _EntDtTag.DicCategory.Clear();
            try
            {
                for (int i = 0; i < this.dgvSaveSetting.Rows.Count; i++)
                {
                    var builIn = this.dgvSaveSetting.Rows[i].Tag;
                    if (builIn == null)
                        continue;
                    DataGridViewComboBoxCell tagColumn = (DataGridViewComboBoxCell)this.dgvSaveSetting.Rows[i].Cells[1];

                    var eleId = tagColumn.Value;
                    if (eleId == null)
                        continue;

                    var symbolTag = _cmpElements.RvtDBDoc.GetElement((ElementId)eleId);
                    if (symbolTag == null)
                        continue;
                    if (!_EntDtTag.DicCategory.ContainsKey((BuiltInCategory)builIn))
                        _EntDtTag.DicCategory.Add((BuiltInCategory)builIn, (FamilySymbol)symbolTag);
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Set value default</summary>
        ///
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void DefaultValue()
        {
            // Set option get object
            RdbGetObject = _EntDtTag.GetObjectOpt;

            // Set check box
            this.cbkLeftRight.Checked = _EntDtTag.ChkLeftRight;
            this.cbkTopBottom.Checked = _EntDtTag.ChkTopBottom;

            // Set option area premises
            RdbAreaPremises = _EntDtTag.AreaPremisesOpt;

            // Set option tag leader
            RdbTagLeader = _EntDtTag.TagLeaderOtp;

            // Set option handle preset tag
            RdbHandlePresetTag = _EntDtTag.HandlePresetTagOpt;

            SelectObject();
            SetAreaPremises();
        }

        /// ================================================================================
        /// <summary>Get value point</summary>
        ///
        /// <param name="valueXYZ"></param>
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private XYZ GetPointFromString(string valueXYZ)
        {
            XYZ retVal = new XYZ();
            if (string.IsNullOrEmpty(valueXYZ))
                return null;
            string[] value = valueXYZ.Split(',');
            double x = 0;
            double y = 0;
            if (value.Length >= 2)
            {
                if (double.TryParse(value[0], out double parsedX))
                    x = parsedX;

                if (double.TryParse(value[1], out double parsedY))
                    y = parsedY;
            }
            else
                return null;

            retVal = new XYZ(x, y, 0);

            return retVal;
        }

        /// ================================================================================
        /// <summary>Select object option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SelectObject()
        {
            // Save the previous user input value
            var checkedRows = from DataGridViewRow r in dgvCategory.Rows
                              where Convert.ToBoolean(r.Cells[0].Value) == true
                              select r;
            Collections.Generic.List<FamilySymbol> listSymbol = new List<FamilySymbol>();

            foreach (var row in checkedRows)
            {
                int indexRow = row.Index;
                BuiltInCategory builtIn = (BuiltInCategory)this.dgvCategory.Rows[indexRow].Tag;
                Category category = _cmpElements.GetCategory((BuiltInCategory)builtIn);
                listSymbol = _cmpElements.GetAllType(_cmpElements.RvtDBDoc, category);

                if (!listIndex.Contains(indexRow))
                    listIndex.Add(indexRow);
                if (!dicCat.ContainsKey(builtIn))
                    dicCat.Add(builtIn, (listSymbol));
            }

            if (this.rdbSelectObject.Checked)
            {
                this.btnSelectObject.Enabled = true;
                this.dgvSaveSetting.Rows.Clear();

                for (int i = 0; i < this.dgvCategory.Rows.Count; i++)
                {
                    RvtExtApp.Entities.DataGridViewDisableCheckboxCell checkEnable = new RvtExtApp.Entities.DataGridViewDisableCheckboxCell();
                    checkEnable.Enabled = false;
                    this.dgvCategory.Rows[i].Cells[0] = checkEnable;
                    this.dgvCategory.Rows[i].Cells[0].Value = false;
                }
                this.dgvCategory.Columns[0].ReadOnly = true;

                // Add data to dgv settings
                AddDataDgvSettings();
            }
            else
            {
                this.btnSelectObject.Enabled = false;

                this.dgvSaveSetting.Rows.Clear();

                for (int i = 0; i < this.dgvCategory.Rows.Count; i++)
                {
                    RvtExtApp.Entities.DataGridViewDisableCheckboxCell checkEnable = new RvtExtApp.Entities.DataGridViewDisableCheckboxCell();
                    checkEnable.Enabled = true;
                    this.dgvCategory.Rows[i].Cells[0] = checkEnable;

                    if (listIndex.Any(x => x == i) == true)
                        this.dgvCategory.Rows[i].Cells[0].Value = true;
                    else
                        this.dgvCategory.Rows[i].Cells[0].Value = false;
                }

                // Add data to dgv settings
                foreach (var pair in dicCat)
                    AddDataDgvSettings(this.dgvSaveSetting, pair.Key, pair.Value);
            }
        }

        /// ================================================================================
        /// <summary>Format Json</summary>
        ///
        /// <param name="json"></param>
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private static string FormatJson(string json)
        {
            var parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        /// ================================================================================
        /// <summary> Add data dgvCategory</summary>
        ///
        /// <param name="dtgview"></param>
        /// <param name="cateItem"></param>
        /// <returns></returns>
        ///
        /// <history>2021/12/14 Created Applied Technology</history>
        /// ================================================================================
        public void AddDataDgvCategory(DataGridView dtgview, BuiltInCategory cateItem)
        {
            try
            {
                if (cateItem == BuiltInCategory.INVALID)
                    return;
                int index = dtgview.Rows.Add();
                dtgview.Rows[index].Cells[0].Value = false;
                dtgview.Rows[index].Cells[1].Value = _cmpElements.GetCategoryName((BuiltInCategory)cateItem);
                dtgview.Rows[index].Tag = cateItem;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return;
            }
        }

        /// ================================================================================
        /// <summary> Add data dgvSettings</summary>
        ///
        /// <param name="dgvsetting"></param>
        /// <param name="cateItem"></param>
        /// <param name="listSymbol"></param>
        ///
        /// <history>2021/12/14 Created Applied Technology</history>
        /// ================================================================================
        public void AddDataDgvSettings(DataGridView dgvsetting, BuiltInCategory cateItem, Collections.Generic.List<FamilySymbol> listSymbol)
        {
            try
            {
                if (cateItem == BuiltInCategory.INVALID || listSymbol == null)
                    return;

                int index = dgvsetting.Rows.Add();
                dgvsetting.Rows[index].Cells[0].Value = _cmpElements.GetCategoryName((BuiltInCategory)cateItem);

                var cbbCell = dgvsetting.Rows[index].Cells[1] as DataGridViewComboBoxCell;
                cbbCell.FlatStyle = FlatStyle.Popup;
                dgvsetting.Rows[index].Cells[1].Style.BackColor = System.Drawing.Color.White;

                cbbCell.DataSource = listSymbol;
                cbbCell.DisplayMember = "Name";
                cbbCell.ValueMember = "Id";
                if (savedSetting == null)
                    cbbCell.Value = listSymbol.Select(x => x.Id).FirstOrDefault();
                else
                {
                    // 保存した設定情報から復元
                    Utils.TagFamilyType tagType = savedSetting.TagFamilyType.Find(x => x.CategoryId == (BuiltInCategory)cateItem);
                    if (tagType != null)
                    {
                        FamilySymbol symbol = listSymbol.Find(x => x.Id.ToString() == tagType.TagTypeId.ToString() || x.Name == tagType.TagFamilyTypeName);
                        if (symbol != null )
                            cbbCell.Value = symbol.Name;
                        else
                            cbbCell.Value = listSymbol.Select(x => x.Id).FirstOrDefault();
                    }
                    else
                        cbbCell.Value = listSymbol.Select(x => x.Id).FirstOrDefault();
                }

                dgvsetting.Rows[index].Tag = cateItem;
                index++;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return;
            }
        }

        /// ================================================================================
        /// <summary>Select area premises option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetAreaPremises()
        {
            if (this.rdbAutoJudgment.Checked)
                this.btnSetArea.Enabled = false;
            else
                this.btnSetArea.Enabled = true;
        }

        /// ================================================================================
        /// <summary>Add data dgvSettings</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void AddDataDgvSettings()
        {
            try
            {
                Collections.Generic.Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>> dicCat = null;
                Collections.Generic.List<FamilySymbol> listSymbol = null;
                if (RdbGetObject == 0)
                {
                    this.dgvSaveSetting.Rows.Clear();
                    dicCat = new Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>>();
                    foreach (var ele in _EntDtTag.LstElement)
                    {
                        if (ele.Category == null)
                            continue;
                        BuiltInCategory builtIn = (BuiltInCategory)Int32.Parse(ele.Category.Id.ToString());

                        listSymbol = _cmpElements.GetAllType(_cmpElements.RvtDBDoc, ele.Category);
                        if (!dicCat.ContainsKey(builtIn))
                            dicCat.Add(builtIn, (listSymbol));
                    }
                    foreach (var pair in dicCat)
                    {
                        AddDataDgvSettings(this.dgvSaveSetting, pair.Key, pair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return;
            }
        }

        /// ================================================================================
        /// <summary>get selected check box </summary>
        /// <returns></returns>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectedCheckBox()
        {
            if (!cbkLeftRight.Checked && !cbkTopBottom.Checked)
                return true;
            return false;
        }

        /// ================================================================================
        /// <summary>Check data  input</summary>
        /// <returns></returns>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool IsError()
        {
            // Check pickbox
            if (RdbAreaPremises == 1 && _EntDtTag.OutLine == null)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NO_SPECCIFY_PLACE_TAG"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                return false;
            }

            // Check selected check box
            if (GetSelectedCheckBox())
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NO_CHECKBOX"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                return false;
            }

            // Check element
            if (this.dgvSaveSetting == null || this.dgvSaveSetting.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NO_SELECT_OBJECT"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                return false;
            }

            // Check category no element
            if (this.dgvSaveSetting != null && this.dgvSaveSetting.Rows.Count > 0)
            {
                if (!GetAllElementOfCategory())
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NO_ELEMENT"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                    return false;
                }
            }
            return true;
        }

        /// ================================================================================
        /// <summary>Check number element in active view</summary>
        ///
        /// <return><return>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private bool GetAllElementOfCategory()
        {
            List<Element> retVal = new List<Element>();

            if (this.dgvSaveSetting == null || this.dgvSaveSetting.Rows.Count == 0)
                return false;
            // List BuiltInCategory
            List<BuiltInCategory> listBuilt = new List<BuiltInCategory>();
            for (int i = 0; i < dgvSaveSetting.Rows.Count; i++)
            {
                BuiltInCategory built = (BuiltInCategory)this.dgvSaveSetting.Rows[i].Tag;
                listBuilt.Add(built);
            }
            ElementMulticategoryFilter ruleCate = new ElementMulticategoryFilter(listBuilt);

            // Get list element of category
            retVal = new FilteredElementCollector(_cmpElements.RvtDBDoc, _cmpElements.RvtDBDoc.ActiveView.Id)
                .WherePasses(ruleCate)
                .ToElements().ToList();
            if (retVal == null || retVal.Count == 0)
                return false;

            return true;
        }

        /// ================================================================================
        /// <summary>Add data from dgv data grid view to data grid view setting</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void GetValueToSetting()
        {
            // Get row check
            var checkedRows = from DataGridViewRow r in dgvCategory.Rows
                              where Convert.ToBoolean(r.Cells[0].Value) == true
                              select r;
            Collections.Generic.Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>> dicCat = new Collections.Generic.Dictionary<BuiltInCategory, Collections.Generic.List<FamilySymbol>>();
            Collections.Generic.List<FamilySymbol> listSymbol = new List<FamilySymbol>();
            foreach (var row in checkedRows)
            {
                int ide = row.Index;
                BuiltInCategory builtIn = (BuiltInCategory)this.dgvCategory.Rows[ide].Tag;
                Category category = _cmpElements.GetCategory((BuiltInCategory)builtIn);
                listSymbol = _cmpElements.GetAllType(_cmpElements.RvtDBDoc, category);

                if (!dicCat.ContainsKey(builtIn))
                    dicCat.Add(builtIn, (listSymbol));
            }
            // Clear data
            this.dgvSaveSetting.Rows.Clear();

            // Add data to dgv settings
            foreach (var pair in dicCat)
                AddDataDgvSettings(this.dgvSaveSetting, pair.Key, pair.Value);
        }

        /// ================================================================================
        /// <summary>Save view template</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SaveViewTemplate()
        {
            try
            {
                // Get the saved view template
                var stringJsonValue = ADSK.JExtRAC.AutoLayoutTag.Utils.StorageUtility.GetValue(_cmpElements.RvtDBDoc.ProjectInformation, _CmpAttribute.ResourceText("IDS_TXT_GUID"), _CmpAttribute.ResourceText("IDS_TXT_FIELD_SETTING"), typeof(string));
                RvtExtApp.Utils.Root listviewTemplates = new Utils.Root();
                if (stringJsonValue != null && !string.IsNullOrEmpty(stringJsonValue.ToString()))
                    listviewTemplates = JsonConvert.DeserializeObject<RvtExtApp.Utils.Root>(stringJsonValue.ToString());

                // Add new view template
                RvtExtApp.Utils.Viewtemplate viewtemplate = new Utils.Viewtemplate();
                {
                    ElementId viewId = _cmpElements.RvtDBDoc.ActiveView.ViewTemplateId;

                    if (viewId != null && viewId != ElementId.InvalidElementId)
                    {
                        Element viewtp = _cmpElements.RvtDBDoc.GetElement(viewId);
                        viewtemplate.ViewTemplateId = Int32.Parse(viewId.ToString());
                        viewtemplate.ViewTemplateName = viewtp.Name;
                    }
                    else
                    {
                        viewtemplate.ViewTemplateId = Int32.Parse(ElementId.InvalidElementId.ToString());
                        viewtemplate.ViewTemplateName = _CmpAttribute.ResourceText("IDS_TXT_NONE");
                    }
                    // Remove existed view template
                    Utils.Viewtemplate viewStatus = listviewTemplates.ViewTemplates.Find(x => x.ViewTemplateId == viewtemplate.ViewTemplateId);
                    if (viewStatus != null)
                        listviewTemplates.ViewTemplates.Remove(viewStatus);

                    // Data target object
                    viewtemplate.TargetObjectType = RdbGetObject;
                    {
                        if (RdbGetObject == 0)
                        {
                            // User select object by handle
                            if (_EntDtTag.LstElement != null && _EntDtTag.LstElement.Count > 0)
                            {
                                foreach (var ele in _EntDtTag.LstElement)
                                {
                                    if (ele == null)
                                        continue;
                                    viewtemplate.TargetObjectIds.Add(Int32.Parse(ele.Id.ToString()));
                                }
                            }
                        }
                        else
                        {
                            // User select object by category
                            for (int i = 0; i < this.dgvSaveSetting.Rows.Count; i++)
                            {
                                var builIn = this.dgvSaveSetting.Rows[i].Tag;
                                if (builIn == null)
                                    continue;

                                DataGridViewComboBoxCell tagColumn = (DataGridViewComboBoxCell)this.dgvSaveSetting.Rows[i].Cells[1];

                                var eleId = tagColumn.Value;
                                if (eleId == null)
                                    continue;

                                var symbolTag = _cmpElements.RvtDBDoc.GetElement((ElementId)eleId);
                                if (symbolTag == null)
                                    continue;

                                viewtemplate.TargetObjectCategories.Add(_cmpElements.GetCategoryName((BuiltInCategory)builIn));
                            }
                        }
                    }

                    viewtemplate.ExistedTagProcessingType = RdbHandlePresetTag;

                    // Data tag position
                    RvtExtApp.Utils.TagPosition tagPosition = new Utils.TagPosition();
                    tagPosition.LeftRight = cbkLeftRight.Checked.ToString();
                    tagPosition.TopBottom = cbkTopBottom.Checked.ToString();
                    viewtemplate.TagPosition = tagPosition;

                    // Data tag leader
                    viewtemplate.TagLeader = RdbTagLeader;

                    // Data tag placing method
                    RvtExtApp.Utils.TagPlacingMethod tagPlacingMethod = new RvtExtApp.Utils.TagPlacingMethod();
                    {
                        tagPlacingMethod.PlacingMethodType = RdbAreaPremises;
                        if (RdbAreaPremises == 0)
                        {
                            tagPlacingMethod.Point1 = "";
                            tagPlacingMethod.Point2 = "";
                        }
                        else
                        {
                            // Point 1
                            if (_EntDtTag.OutLine != null)
                                tagPlacingMethod.Point1 = _EntDtTag.OutLine.MinimumPoint.X.ToString() + "," + _EntDtTag.OutLine.MinimumPoint.Y.ToString() + "," + 0;
                            else
                                tagPlacingMethod.Point1 = "";

                            // Point 2
                            if (_EntDtTag.OutLine != null)
                                tagPlacingMethod.Point2 = _EntDtTag.OutLine.MaximumPoint.X.ToString() + "," + _EntDtTag.OutLine.MaximumPoint.Y.ToString() + "," + 0;
                            else
                                tagPlacingMethod.Point2 = "";
                        }
                    }
                    viewtemplate.TagPlacingMethod = tagPlacingMethod;

                    // Data tag family type
                    RvtExtApp.Utils.TagFamilyType tagFamilyType = new RvtExtApp.Utils.TagFamilyType();
                    {
                        for (int i = 0; i < this.dgvSaveSetting.Rows.Count; i++)
                        {
                            tagFamilyType = new RvtExtApp.Utils.TagFamilyType();
                            var builIn = this.dgvSaveSetting.Rows[i].Tag;
                            if (builIn == null)
                                continue;
                            // Category
                            tagFamilyType.Category = _cmpElements.GetCategoryName((BuiltInCategory)builIn);
                            tagFamilyType.CategoryId = (BuiltInCategory)builIn;

                            // Family type tag
                            DataGridViewComboBoxCell tagColumn = (DataGridViewComboBoxCell)this.dgvSaveSetting.Rows[i].Cells[1];
                            var eleId = tagColumn.Value;
                            if (eleId != null)
                            {
                                var symbolTag = _cmpElements.RvtDBDoc.GetElement((ElementId)eleId);
                                if (symbolTag == null)
                                    continue;

                                tagFamilyType.TagFamilyTypeName = symbolTag.Name;
                                tagFamilyType.TagTypeId = Int32.Parse(symbolTag.Id.ToString());
                            }
                            else
                            {
                                tagFamilyType.TagFamilyTypeName = string.Empty;
                                tagFamilyType.TagTypeId = Int32.Parse(ElementId.InvalidElementId.ToString());
                            }
                            // Add data
                            viewtemplate.TagFamilyType.Add((RvtExtApp.Utils.TagFamilyType)tagFamilyType);
                        }
                    }
                }
                listviewTemplates.ViewTemplates.Add(viewtemplate);
                // String value json
                var jsonToWrite = JsonConvert.SerializeObject(listviewTemplates, Formatting.Indented);

                // Format string value json
                string formatedData = FormatJson(jsonToWrite);

                using (SubTransaction subtran = new SubTransaction(_cmpElements.RvtDBDoc))
                {
                    subtran.Start();
                    ADSK.JExtRAC.AutoLayoutTag.Utils.StorageUtility.SetExtensibleStorage(_cmpElements.RvtDBDoc.ProjectInformation, _CmpAttribute.ResourceText("IDS_TXT_SCHEMA_NAME"),
                       _CmpAttribute.ResourceText("IDS_TXT_VENDOR_ID"), _CmpAttribute.ResourceText("IDS_TXT_FIELD_SETTING"), _CmpAttribute.ResourceText("IDS_TXT_GUID"), formatedData);
                    subtran.Commit();
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return;
            }
        }

        #endregion Member Functions

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>Get object option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private int RdbGetObject
        {
            get
            {
                int ret = 0;

                if (this.rdbSelectObject.Checked)// user select by hand
                    ret = 0;
                else
                    ret = 1;// user select by category

                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbSelectObject.Checked = true;// user select by hand
                        break;

                    case 1:
                        this.rdbAllCategory.Checked = true;// user select by category
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>Get tag leader option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private int RdbTagLeader
        {
            get
            {
                int ret = 0;

                if (this.rdbHasLeader.Checked)// user select has leader
                    ret = 0;
                else
                    ret = 1;// user select no leader

                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbHasLeader.Checked = true;// user select has leader
                        break;

                    case 1:
                        this.rdbNoLeader.Checked = true;// user select no leader
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>Area premises option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private int RdbAreaPremises
        {
            get
            {
                int ret = 0;

                if (this.rdbAutoJudgment.Checked) // automatic
                    ret = 0;
                else
                    ret = 1; // set by handle

                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbAutoJudgment.Checked = true;// automatic
                        break;

                    case 1:
                        this.rdbSetByHand.Checked = true; // set by handle
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handle preset tag option</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private int RdbHandlePresetTag
        {
            get
            {
                int ret = 0;

                if (this.rdbOnlyNewTag.Checked) // set new
                    ret = 0;
                else if (this.rdbReset.Checked) // reset
                    ret = 1;
                else
                    ret = 2;// put more tags
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbOnlyNewTag.Checked = true;// set new
                        break;

                    case 1:
                        this.rdbReset.Checked = true;// reset
                        break;

                    case 2:
                        this.rdbOderMore.Checked = true;// put more tags
                        break;
                }
            }
        }

        #endregion Properties

        // Event

        #region Event

        /// ================================================================================
        /// <summary>Handles the Click event of the rdbSelectObject control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void rdbSelectObject_Click(object sender, EventArgs e)
        {
            SelectObject();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the rdbAllCategory control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void rdbAllCategory_Click(object sender, EventArgs e)
        {
            SelectObject();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the rdbAutoJudgment control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void rdbAutoJudgment_Click(object sender, EventArgs e)
        {
            SetAreaPremises();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the rdbSetByHand control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void rdbSetByHand_Click(object sender, EventArgs e)
        {
            SetAreaPremises();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSetTag control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void btnSetTag_Click(object sender, EventArgs e)
        {
            GetData();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSelectObject control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectObject_Click(object sender, EventArgs e)
        {
            _isSelectObject = true;
            _isObject = true;
            this.DialogResult = DialogResult.OK;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSetArea control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void btnSetArea_Click(object sender, EventArgs e)
        {
            _isSetArea = true;
            _isObject = false;
            this.DialogResult = DialogResult.OK;
        }

        /// ================================================================================
        /// <summary>dgvSaveSetting_DataErrorb</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSaveSetting_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        /// ================================================================================
        /// <summary>Event when data grid view click on combobox</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSaveSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = e.RowIndex != -1 && e.ColumnIndex != -1;
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox
            if (validClick && datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                datagridview.BeginEdit(true);
                System.Windows.Forms.ComboBox combo = datagridview.EditingControl as System.Windows.Forms.ComboBox;

                if (combo != null)
                    combo.DroppedDown = true;
                else
                    datagridview.BeginEdit(true);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSaveSettings control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void btnSaveSetting_Click(object sender, EventArgs e)
        {
            // Check value
            if (!IsError())
            {
                this.tabAutomaticTag.SelectedIndex = 0;
                return;
            }
            // Save view template
            SaveViewTemplate();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the dgvCategory control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void dgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (RdbGetObject == 1)
            {
                if (e.ColumnIndex == -1 || e.RowIndex == -1)
                    return;
                // Check value
                if (dgvCategory.Rows.Count == 0)
                    return;
                var builtIn = this.dgvCategory.Rows[e.RowIndex].Tag;

                if ((bool)dgvCategory.Rows[e.RowIndex].Cells[0].Value)
                {
                    dgvCategory.Rows[e.RowIndex].Cells[0].Value = false;

                    if (dicCat.ContainsKey((Revit.DB.BuiltInCategory)builtIn))
                        dicCat.Remove((Revit.DB.BuiltInCategory)builtIn);

                    // Save the previous user input value
                    if (listIndex.Contains(e.RowIndex))
                        listIndex.Remove(e.RowIndex);

                    if (_EntDtTag.LstBuiltInCategory.Contains((Revit.DB.BuiltInCategory)builtIn))
                        _EntDtTag.LstBuiltInCategory.Remove((Revit.DB.BuiltInCategory)builtIn);
                }
                else
                {
                    dgvCategory.Rows[e.RowIndex].Cells[0].Value = true;

                    // Save the previous user input value

                    if (!listIndex.Contains(e.RowIndex))
                        listIndex.Add(e.RowIndex);

                    if (_EntDtTag.LstBuiltInCategory.Contains((Revit.DB.BuiltInCategory)builtIn))
                        _EntDtTag.LstBuiltInCategory.Remove((Revit.DB.BuiltInCategory)builtIn);

                    _EntDtTag.LstBuiltInCategory.Add((Revit.DB.BuiltInCategory)builtIn);
                }

                GetValueToSetting();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Scroll event of the dgvSaveSetting control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="ScrollEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSaveSetting_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                ScrollBarVertical.Value = e.NewValue;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the MouseWheel event of the dgvSaveSetting control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSaveSetting_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.dgvSaveSetting.Rows.Count > 1)
                {
                    if (e.Delta > 0 && dgvSaveSetting.FirstDisplayedScrollingRowIndex > 0)
                    {
                        dgvSaveSetting.FirstDisplayedScrollingRowIndex--;
                    }
                    else if (e.Delta < 0)
                    {
                        dgvSaveSetting.FirstDisplayedScrollingRowIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Scroll event of the ScrollBarVertical control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.ScrollEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void ScrollBarVertical_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.NewValue < this.dgvSaveSetting.Rows.Count)
                    dgvSaveSetting.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        #endregion Event
    }
}