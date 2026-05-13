using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Parameter = Autodesk.Revit.DB.Parameter;
using Range = Microsoft.Office.Interop.Excel.Range ;
using RvtExtApp = ADSK.JExtRAC.ExportSchedule;

namespace ADSK.JExtRAC.ExportSchedule.Utils
{
    public class ScheduleExporter
    {
        // メンバ変数

        #region Member Variables

        //Dictionary contains all data for body schedule
        private List<RowTable> _Body_Datas = null;

        //Begin data
        private int _EndHeader = -1;

        //Name for UID column
        private string _UID = "UID";

        //Fix column for sheet type and instance
        private string _FamilyAndType = "I:ファミリとタイプ";

        //Sheet name
        public string _SheetScheduleName = "集計表";

        //Sheet type name
        public string _SheetTypeName = "タイプ内訳";

        //Sheet instance name
        public string _SheetInstanceName = "インスタンス内訳";

        //Parameter components
        private RvtExtApp.Components.Parameters _CmpParameters = null;

        //Element list in the schedule
        private List<Element> _Elements = null;

        //Detect schedule has group by Family and Type
        public bool _GroupByFamilyType = false;

        //Dictionary contains data type of columns
        private Dictionary<int, sDataType> _DataTypeOf_Column = null;

        private Autodesk.Revit.ApplicationServices.Application _Application = null;

        //error log
        private System.Text.StringBuilder strLog = new System.Text.StringBuilder();

        // Attribute
        private RvtExtApp.Components.Attribute _CmpAttribute = null;

        //UID Flag Export
        public enum UIDFlagExport
        {
            None = 0,
            Type,
            Instance
        }

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        public ScheduleExporter(RvtExtApp.Components.Attribute cmpAttribute, Autodesk.Revit.ApplicationServices.Application application, UIDocument uiDoc, ViewSchedule schedule, bool forImport)
        {
            _CmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, uiDoc);
            _CmpAttribute = cmpAttribute;

            // Set header is show
            bool isShowHeader = schedule.Definition.ShowHeaders;
            if (isShowHeader == false && forImport == true)
                schedule.Definition.ShowHeaders = true;

            _Application = application;
            _Elements = GetElementsInSchedule(uiDoc.Document, schedule);

            if (schedule.Definition.IsKeySchedule == false)
            {
                //Add parameter UID
                CreateParameterUID(uiDoc.Document, schedule);

                //Add fix col which family and type
                CreateParameterFamilyType(uiDoc.Document, schedule);
            }

            GetData(uiDoc.Document, schedule, forImport);

            // Rollback show header
            schedule.Definition.ShowHeaders = isShowHeader;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>Get element list in schedule</summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">Schedule</param>
        /// <returns>Element list</returns>
        ///
        /// <history>2018/10/24 Created Applied Technology</history>
        /// ================================================================================
        public List<Element> GetElementsInSchedule(Document doc, ViewSchedule schedule)
        {
            List<Element> elements = new List<Element>();
            //Get all elements in a schedule
            FilteredElementCollector elementCollector = new FilteredElementCollector(doc, schedule.Id).WhereElementIsNotElementType();

            //Iterator all elements
            using (IEnumerator<Element> enumeratorElements = elementCollector.GetEnumerator())
            {
                while (((IEnumerator)enumeratorElements).MoveNext())
                {
                    Element currentElement = enumeratorElements.Current;
                    if (currentElement.IsHidden(schedule) == true)
                        continue;

                    if (Int32.Parse(schedule.Definition.CategoryId.ToString()) != (int)BuiltInCategory.INVALID)
                    {
                        if (currentElement.Category == null || currentElement.Category.Id != schedule.Definition.CategoryId)
                            continue;
                    }

                    elements.Add(currentElement);
                }
            }

            return elements;
        }

        /// ================================================================================
        /// <summary>Create a project parameter </summary>
        ///
        /// <param name="name">Parameter name</param>
        /// <param name="cats">Category set</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2021/10/15 Created Applied Technology</history>
        /// ================================================================================
        public bool CreateProjectParameter(string name, CategorySet cats)
        {
            try
            {
                string oriFile = _Application.SharedParametersFilename;
                string tempFile = Path.GetTempFileName() + ".txt";
                using (File.Create(tempFile))
                {
                }
                _Application.SharedParametersFilename = tempFile;

                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(name, SpecTypeId.String.Text);
                externalDefinitionCreationOptions.Visible = true;
                ExternalDefinition def = _Application.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

                _Application.SharedParametersFilename = oriFile;
                File.Delete(tempFile);

                Autodesk.Revit.DB.Binding binding = _Application.Create.NewInstanceBinding(cats);

                BindingMap map = (new UIApplication(_Application)).ActiveUIDocument.Document.ParameterBindings;
                map.Insert(def, binding, GroupTypeId.Data);

                return true;
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Create a parameter with name UID</summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">View schedule</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2018/12/15 Created Applied Technology</history>
        /// ================================================================================
        public bool CreateParameterUID(Document doc, ViewSchedule schedule)
        {
            if (_Elements.Count == 0)
                return false;

            //Get category list
            CategorySet cats = new CategorySet();
            foreach (Element element in _Elements)
            {
                if (element.Category != null)
                {
                    if (cats.Contains(element.Category) == false)
                        cats.Insert(element.Category);
                }
            }

            if (cats.Size == 0)
                return false;

            try
            {
                //Check exist
                Parameter para = _CmpParameters.FindParameter(_Elements[0], _UID);
                if (para == null)
                {
                    CreateProjectParameter(_UID, cats);
                }

                para = null;
                foreach (Element element in _Elements)
                {
                    para = _CmpParameters.FindParameter(element, _UID);
                    if (para != null) {
                        if ( para.IsReadOnly == false ) para.Set( element.UniqueId ) ;
                    }
                }

                if (para != null)
                {
                    try
                    {
                        //Add to schedule
                        schedule.Definition.InsertField(ScheduleFieldType.Instance, para.Id, 0);
                        schedule.Definition.GetField(0).GridColumnWidth = 0.001;
                    }
                    catch (System.Exception ex)
                    {
                        string mess = ex.Message;
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Create a family and type parameter </summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">View schedule</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2018/12/15 Created Applied Technology</history>
        /// ================================================================================
        public bool CreateParameterFamilyType(Document doc, ViewSchedule schedule)
        {
            if (_Elements.Count == 0)
                return false;

            //Get category list
            CategorySet cats = new CategorySet();
            foreach (Element element in _Elements)
            {
                if (element.Category != null)
                {
                    if (cats.Contains(element.Category) == false)
                        cats.Insert(element.Category);
                }
            }

            if (cats.Size == 0)
                return false;

            try
            {
                //Check exist
                Parameter para = _CmpParameters.FindParameter(_Elements[0], _FamilyAndType);
                if (para == null)
                {
                    CreateProjectParameter(_FamilyAndType, cats);
                }

                //Set family and type (fix) for all element
                para = null;
                foreach (Element element in _Elements)
                {
                    para = _CmpParameters.FindParameter(element, _FamilyAndType);
                    if (para != null)
                    {
                        if (para.IsReadOnly == false)
                        {
                            var familyAndType = _CmpParameters.GetParameter(element, BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);
                            if (familyAndType != null)
                                para.Set(familyAndType.AsValueString());
                        }
                        else
                        {
                        }
                    }
                }

                if (para != null)
                {
                    try
                    {
                        //Add to schedule
                        schedule.Definition.InsertField(ScheduleFieldType.Instance, para.Id, 1);//After UID
                        schedule.Definition.GetField(0).GridColumnWidth = 0.001;
                    }
                    catch (System.Exception ex)
                    {
                        string mess = ex.Message;
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Create a parameter with name UID</summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">View schedule</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2018/12/15 Created Applied Technology</history>
        /// ================================================================================
        public bool CreateParameterUID1(Document doc, ViewSchedule schedule)
        {
            //Get category list
            var categories = new List<Category>();
            foreach (Element element in _Elements)
            {
                if (element.Category != null)
                {
                    if (categories.Find(item => item.Id == element.Category.Id) == null)
                        categories.Add(element.Category);
                }
            }

            if (categories.Count == 0)
                return false;

            try
            {
                //Check exist
                BindingMap bm = doc.ParameterBindings;
                DefinitionBindingMapIterator it = bm.ForwardIterator();
                while (it.MoveNext())
                {
                    Definition def = it.Key;

                    if (def.Name.Equals(_UID))
                        break;
                }

                int instanceParameter = 0;
                var result1 = _CmpParameters.SetDefinition(null, categories, _UID, SpecTypeId.String.Text, new ForgeTypeId(string.Empty), true, instanceParameter);
                if (result1 == true)
                {
                    //Set UID for all element list
                    Parameter para = null;
                    foreach (Element element in _Elements)
                    {
                        para = _CmpParameters.FindParameter(element, _UID);
                        if (para != null)
                        {
                            if (para.IsReadOnly == false)
                                para.Set(element.UniqueId.ToString());
                            else
                            {
                            }
                        }
                    }

                    if (para != null)
                    {
                        try
                        {
                            //Add to schedule
                            schedule.Definition.InsertField(ScheduleFieldType.Instance, para.Id, 0);
                            schedule.Definition.GetField(0).GridColumnWidth = 0.001;
                        }
                        catch (System.Exception ex)
                        {
                            string mess = ex.Message;
                        }
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Get headers and values data in a schedule</summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">Schedule</param>
        /// <param name="forImport">For Import</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2018/10/24 Created Applied Technology</history>
        /// ================================================================================
        public bool GetData(Document doc, ViewSchedule schedule, bool forImport)
        {
            _EndHeader = -1;
            _Body_Datas = new List<RowTable>();
            _DataTypeOf_Column = new Dictionary<int, sDataType>();

            List<ScheduleField> fieldsList = new List<ScheduleField>();
            int fieldCount = schedule.Definition.GetFieldCount();

            //Get all fields in schedule
            for (int index = 0; index < fieldCount; ++index)
            {
                ScheduleField field = schedule.Definition.GetField(index);

                if (field.IsHidden)
                    continue;

                Type type = typeof(string);
                if (field.CanTotal())
                    type = typeof(double);

                fieldsList.Add(field);

                //Check grouping
                var group = schedule.Definition.GetSortGroupFields().Where(item => item.FieldId == field.FieldId).FirstOrDefault();
                if (group != null)
                {
                    if (Int32.Parse(field.ParameterId.ToString()) == (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM ||
                        Int32.Parse(field.ParameterId.ToString()) == (int)BuiltInParameter.ELEM_TYPE_PARAM)
                    {
                        _GroupByFamilyType = true;
                    }
                }
            }

            try
            {
                // Master Data
                TableData tableData = schedule.GetTableData();

                var sType = SectionType.Body;
                if (schedule.IsValidSectionType(sType) == false)
                    return false;

                TableSectionData sectionData = tableData.GetSectionData(sType);

                int blankRow = -1;
                int index = 0;
                for (int row = sectionData.FirstRowNumber; row <= sectionData.NumberOfRows - 1; row++)
                {
                    var rowTable = new RowTable(index);

                    // Loop over the table section row.
                    bool isHeader_Footer_Row = true;
                    for (int col = sectionData.FirstColumnNumber; col <= sectionData.NumberOfColumns - 1; col++)
                    {
                        sDataType _sDataType = sDataType.General;

                        string txt = schedule.GetCellText(sType, row, col);
                        //Create Cell
                        int col_cell = col + 1;
                        var cell = new CellTable(txt, row + 1, col_cell);

                        if (col == 0 && txt != string.Empty)
                        {
                            //Get element
                            string uniqueId = txt;
                            rowTable._Element = doc.GetElement(uniqueId);
                            if (_EndHeader == -1 && rowTable._Element != null)
                                _EndHeader = rowTable._RowIndex - 1;
                        }

                        var tableMergedCell = sectionData.GetMergedCell(row, col);

                        var id = sectionData.GetCellParamId(row, col);

                        if (id != ElementId.InvalidElementId)
                        {
                            isHeader_Footer_Row = false;

                            if (Int32.Parse(id.ToString()) != (int)BuiltInParameter.VIEW_NAME)
                            {
                                if (rowTable._Element != null && col < fieldsList.Count)
                                {
                                    var parameterName = fieldsList[col].GetName();

                                    if (parameterName == _UID && _GroupByFamilyType && schedule.Definition.IsItemized == false)
                                    {
                                        var typeId = GetFamilyType(rowTable._Element);
                                        if (typeId != ElementId.InvalidElementId)
                                        {
                                            var type = doc.GetElement(typeId);
                                            cell.Text = type.UniqueId.ToString();
                                        }
                                    }
                                    else
                                    {
                                        var parameter = _CmpParameters.FindParameter(rowTable._Element, parameterName);
                                        if (parameter != null)
                                        {
                                            //Value
                                            if (forImport == true)
                                            {
                                                if (parameter.StorageType == StorageType.Integer && forImport)
                                                {
                                                    int iValue = 0;
                                                    if (int.TryParse(txt, out iValue) == false)
                                                    {
                                                        txt = parameter.AsInteger().ToString();
                                                    }
                                                }
                                                else if (parameter.StorageType == StorageType.Double)
                                                {
                                                    //Only show double value
                                                    var splits = txt.Split(' ');
                                                    if (splits.Length != 0)
                                                        txt = splits[0];
                                                }
                                            }

                                            if (cell.Text != txt)
                                                cell.Text = txt;

                                            var field = fieldsList[col];

                                            //Check data type
                                            if (_sDataType == sDataType.General)
                                            {
                                                if (parameter.Definition.GetDataType() == SpecTypeId.String.Text || parameter.Definition.GetDataType() == SpecTypeId.String.Url ||
                                                    parameter.Definition.GetDataType() == SpecTypeId.String.MultilineText)
                                                    _sDataType = sDataType.Text;
                                            }
                                        }
                                    }

                                    //Add to dictionary
                                    if (_DataTypeOf_Column.ContainsKey(col_cell) == false)
                                    {
                                        _DataTypeOf_Column.Add(col_cell, _sDataType);
                                    }
                                }
                                else if ((Int32.Parse(id.ToString()) == (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM ||
                                    Int32.Parse(id.ToString()) == (int)BuiltInParameter.ELEM_TYPE_PARAM) &&
                                    rowTable._CellTables.Count != 0 &&
                                    Int32.Parse(schedule.Definition.CategoryId.ToString()) != (int)BuiltInCategory.INVALID &&
                                    _GroupByFamilyType &&
                                    schedule.Definition.IsItemized == false)
                                {
                                    //UID cell
                                    var uid_cell = rowTable._CellTables[0];

                                    // rowTable._CellTables[ 1 ]　LSD1_2300_2400: 外付けカーム_傾斜式_引き分け_戸当り無 No. 1　をつかって、ファミリ:タイプに相当するものを探す
                                    
                                    var uid = GetUIDFromFamilyAndTypeName(schedule, rowTable._CellTables[ 1 ].Text);
                                    uid_cell.Text = uid;
                                }
                            }
                            if (_EndHeader == -1)
                                _EndHeader = rowTable._RowIndex - 1;
                        }

                        if (tableMergedCell.Left != tableMergedCell.Right || tableMergedCell.Top != tableMergedCell.Bottom)
                            cell.MergeCell = tableMergedCell;

                        rowTable._CellTables.Add(cell);

                        if (tableMergedCell.Left == 0 && tableMergedCell.Right == sectionData.NumberOfColumns - 1)
                        {
                            if (_EndHeader == -1)
                                _EndHeader = rowTable._RowIndex - 1;
                        }
                    }

                    if (rowTable._CellTables.Count != 0)
                    {
                        _Body_Datas.Add(rowTable);
                        index++;

                        if (blankRow == -1)
                        {
                            //Has blank before data
                            bool isEmpty = true;
                            foreach (CellTable cellTable in rowTable._CellTables)
                            {
                                if (cellTable.Text != string.Empty)
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }
                            if (isEmpty)
                            {
                                blankRow = rowTable._RowIndex;
                            }
                        }

                        if (blankRow != -1)
                        {
                            if (_EndHeader != -1)
                            {
                                if (blankRow <= _EndHeader)
                                    _EndHeader -= 1;
                            }
                            else
                                _EndHeader = blankRow - 1;
                        }

                        if (isHeader_Footer_Row && _EndHeader != -1)
                        {
                            rowTable._Header_Footer = isHeader_Footer_Row;
                            if (_EndHeader == -1)
                                _EndHeader = rowTable._RowIndex - 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }

            if (_EndHeader == -1 && _Body_Datas.Count != 0)
            {
                RowTable uidRow = _Body_Datas[0];
                var uidCell = uidRow._CellTables.Find(item => item.Text == _UID);
                if (uidCell != null && uidCell.MergeCell != null)
                {
                    if (uidCell.MergeCell.Top != uidCell.MergeCell.Bottom)
                    {
                        _EndHeader = uidCell.MergeCell.Bottom - uidCell.MergeCell.Top;
                    }
                }
            }

            InsertParameterRow(fieldsList);

            return true;
        }

        /// ================================================================================
        /// <summary>Get family type of an element</summary>
        ///
        /// <param name="element">Element</param>
        /// <returns>True or False</returns>
        ///
        /// <history>2018/11/15 Created Applied Technology</history>
        /// ================================================================================
        private ElementId GetFamilyType(Element element)
        {
            if (element is FamilyInstance)
            {
                FamilyInstance inst = element as FamilyInstance;
                if (null != inst.Symbol)
                {
                    return inst.Symbol.Id;
                }
            }
            else if (element.CanHaveTypeAssigned())
            {
                ElementId typeId = element.GetTypeId();
                if (null != typeId && ElementId.InvalidElementId != typeId)
                {
                    return typeId;
                }
            }

            return ElementId.InvalidElementId;
        }

        
        /// <summary>
        /// ファミリ名とタイプ名からUIDを取得します。
        /// GetUIDFamilyTypeはタイプ名重複の可能性が考慮されていなかったので
        /// 最小の修正にするために用意したメソッドに差し替えた
        /// </summary>
        /// <param name="schedule">対象のViewSchedule。</param>
        /// <param name="familyAndSymbolName">ファミリ名とシンボル名の文字列("Family:Symbol")。</param>
        /// <returns>ファミリタイプのUID。見つからなければ空文字列。</returns>
        private string GetUIDFromFamilyAndTypeName(ViewSchedule schedule, string familyAndSymbolName)
        {
            // 入力文字列をファミリ名とシンボル名に分割
            var parts = familyAndSymbolName.Split(':');
            if (parts.Length != 2) return string.Empty;

            var familyName = parts[0].Trim();
            var symbolName = parts[1].Trim();

            if (string.IsNullOrEmpty(familyName) || string.IsNullOrEmpty(symbolName))
                return string.Empty;

            // 指定されたカテゴリのすべてのタイプを収集
            var familyTypes = new FilteredElementCollector(schedule.Document)
                .OfCategoryId(schedule.Definition.CategoryId)
                .WhereElementIsElementType()
                .Cast<Element>()
                .OfType<FamilySymbol>() // FamilySymbolにキャスト
                .ToList();

            // ファミリ名とシンボル名が一致するファミリタイプを検索
            var foundFamilyType = familyTypes.FirstOrDefault(fs =>
                fs.FamilyName.Equals(familyName, StringComparison.OrdinalIgnoreCase) &&
                fs.Name.Equals(symbolName, StringComparison.OrdinalIgnoreCase));

            // UIDを返す。見つからなければ空文字列を返す
            return foundFamilyType?.UniqueId ?? string.Empty;
        }
        
        
        ///================================================================================
        /// <summary>Get UniqueId of a family type</summary>
        ///
        /// <param name="schedule">View Schedule</param>
        /// <param name="builtInParameter">BuiltInParameter</param>
        /// <param name="txt">Text</param>
        /// <returns>UniqueId or string empty</returns>
        ///
        /// <history>2018/12/24 Created Applied Technology</history>
        /// ================================================================================
        // private string GetUIDFamilyType(ViewSchedule schedule, BuiltInParameter builtInParameter, string txt)
        // {
        //     var familyTypeField = schedule.Definition.GetSchedulableFields().
        //         Where(
        //         item => Int32.Parse(item.ParameterId.ToString()) == (int)builtInParameter).FirstOrDefault();
        //
        //     if (familyTypeField == null)
        //         return string.Empty;
        //
        //     string typeName = string.Empty;
        //     if (Int32.Parse(familyTypeField.ParameterId.ToString()) == (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM)
        //     {
        //         //Get uid of family type
        //         var family_and_Type = txt;
        //         var splits = family_and_Type.Split(':');
        //         if (splits.Length == 2)
        //         {
        //             var familyName = splits[0].Trim();
        //             typeName = splits[1].Trim();
        //         }
        //     }
        //     else if (Int32.Parse(familyTypeField.ParameterId.ToString()) == (int)BuiltInParameter.ELEM_TYPE_PARAM)
        //     {
        //         typeName = txt;
        //     }
        //
        //     if (typeName != string.Empty)
        //     {
        //         FilteredElementCollector collection = new FilteredElementCollector(schedule.Document).
        //             OfCategoryId(schedule.Definition.CategoryId).
        //             WhereElementIsElementType();
        //
        //         var familyTypes = Enumerable.ToList<Element>(Enumerable.Cast<Element>((IEnumerable)collection));
        //
        //         //Find family type
        //         if (familyTypes != null && familyTypes.Count != 0)
        //         {
        //             var find = familyTypes.Find(item => item.Name == typeName);
        //             if (find != null)
        //             {
        //                 return find.UniqueId.ToString();
        //             }
        //         }
        //     }
        //     return string.Empty;
        // }

        ///================================================================================
        /// <summary>Insert parameter row</summary>
        ///
        /// <param name="fieldsList">ScheduleField list</param>
        ///
        /// <history>2018/12/24 Created Applied Technology</history>
        /// ================================================================================
        private void InsertParameterRow(List<ScheduleField> fieldsList)
        {
            if (_EndHeader != -1 && fieldsList.Count != 0 && _Body_Datas.Count != 0)
            {
                List<RowTable> headers = new List<RowTable>();
                for (int i = 0; i <= _EndHeader; i++)
                {
                    var rowTable = _Body_Datas[i];

                    headers.Add(rowTable);
                }

                Dictionary<int, string> data = new Dictionary<int, string>();
                foreach (RowTable rowTable in headers)
                {
                    if (rowTable._CellTables.Count != fieldsList.Count)
                        continue;

                    for (int j = 0; j < fieldsList.Count; j++)
                    {
                        if (data.ContainsKey(j) == true)
                            continue;

                        ScheduleField field = fieldsList[j];
                        var cellTable = rowTable._CellTables[j];
                        if (cellTable.MergeCell == null || (cellTable.MergeCell != null && cellTable.MergeCell.Left == cellTable.MergeCell.Right))
                        {
                            if (data.ContainsKey(j) == false)
                            {
                                string para = field.GetName();

                                if (_Elements.Count != 0)
                                {
                                    var firstElement = _Elements[0];

                                    var parameter = _CmpParameters.GetParameter(firstElement, para, null, new ForgeTypeId(string.Empty));
                                    if (parameter != null)
                                    {
                                        para = "I:" + para;
                                    }
                                    else
                                    {
                                        if (field.FieldType == ScheduleFieldType.CombinedParameter)
                                            para = "C:" + para;
                                        else if (field.FieldType == ScheduleFieldType.Formula)
                                            para = "F:" + para;
                                        else
                                            para = "T:" + para;
                                    }
                                }

                                data.Add(j, para);
                            }
                        }
                    }
                }
                if (data.Count != 0)
                {
                    RowTable newRow = new RowTable(_EndHeader);

                    for (int i = 0; i < fieldsList.Count; i++)
                    {
                        string text = string.Empty;

                        if (data.ContainsKey(i) == true)
                        {
                            text = data[i];
                        }
                        var cellItem = new CellTable(text, newRow._RowIndex, i + 1);
                        newRow._CellTables.Add(cellItem);
                    }
                    int index = _EndHeader + 1;
                    _Body_Datas.Insert(index, newRow);

                    _EndHeader += 1;

                    //Reset row index
                    for (int i = 0; i < _Body_Datas.Count; i++)
                    {
                        var rowTable = _Body_Datas[i];
                        rowTable._RowIndex = i;
                        foreach (CellTable cell in rowTable._CellTables)
                        {
                            cell.Row = i + 1;
                        }
                    }

                    //UID
                    var UID_cell = _Body_Datas[0]._CellTables[0];
                    if (UID_cell != null && UID_cell.Text == _UID)
                    {
                        if (UID_cell.MergeCell == null)
                        {
                            var row = UID_cell.Row - 1;
                            var col = UID_cell.Col - 1;
                            UID_cell.MergeCell = new TableMergedCell(row, col, row + _EndHeader, col);
                        }
                        else
                        {
                            UID_cell.MergeCell = new TableMergedCell(
                                UID_cell.MergeCell.Top,
                                UID_cell.MergeCell.Left,
                                UID_cell.MergeCell.Top + _EndHeader,
                                UID_cell.MergeCell.Right);
                        }
                    }
                    //Calculate merge column Family and type
                    if (_Body_Datas[0]._CellTables.Count > 1)
                    {
                        var FT_cell = _Body_Datas[0]._CellTables[1];
                        if (FT_cell != null && FT_cell.Text == _FamilyAndType)
                        {
                            if (FT_cell.MergeCell == null)
                            {
                                var row = FT_cell.Row - 1;
                                var col = FT_cell.Col - 1;
                                FT_cell.MergeCell = new TableMergedCell(row, col, row + _EndHeader, col);
                            }
                            else
                            {
                                FT_cell.MergeCell = new TableMergedCell(
                                    FT_cell.MergeCell.Top,
                                    FT_cell.MergeCell.Left,
                                    FT_cell.MergeCell.Top + _EndHeader,
                                    FT_cell.MergeCell.Right);
                            }
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary> Export a schedule to excel</summary>
        ///
        /// <param name="doc">Revit document</param>
        /// <param name="schedule">Schedule</param>
        /// <param name="worksheet">Work sheet</param>
        /// <param name="forImport">For import or schedule</param>
        /// <param name="eUIDFlagExport">UID flag export</param>
        /// <param name="errMsg">string error log</param>
        ///
        /// <history>2018/10/24 Created Applied Technology</history>
        /// ================================================================================
        public void ExportViewSchedule(Document doc, ViewSchedule schedule, Worksheet worksheet, bool forImport, UIDFlagExport eUIDFlagExport, out bool isHasError)
        {
            try
            {
                isHasError = false;
                if (_Body_Datas == null || _Body_Datas.Count == 0)
                    return;

                int headerCount = 0;
                int increase = 1; //Excel column start with index = 1

                int colCount = _Body_Datas[0]._CellTables.Count;

                Range cellBegin = null;
                Range cellEnd = null;

                if (_EndHeader == -1)
                    _EndHeader = _Body_Datas.Count - 1;

                if (_Body_Datas.Count != 0)
                {
                    int index = _EndHeader;
                    for (int row = 0; row <= index; row++)
                    {
                        var rowTable = _Body_Datas[row];

                        foreach (CellTable headerCell in rowTable._CellTables)
                        {
                            var excel_cell = worksheet.Cells[headerCell.Row, headerCell.Col];

                            if (headerCell.Text == _UID)
                            {
                                string header = "UID";
                                if (eUIDFlagExport == UIDFlagExport.Instance)
                                    header = "I:UID";
                                else if (eUIDFlagExport == UIDFlagExport.Type)
                                    header = "T:UID";
                                ExcelUtils.FormatTitleCell(headerCell.Row, headerCell.Col, header, 15f, worksheet);
                            }
                            else if (headerCell.Text == _FamilyAndType)
                            {
                                ExcelUtils.FormatTitleCell(headerCell.Row, headerCell.Col, _FamilyAndType, 15f, worksheet);
                            }
                            else
                                excel_cell.Value = headerCell.Text;

                            if (headerCell.MergeCell != null)
                            {
                                if ((headerCell.MergeCell.Left != headerCell.MergeCell.Right) || (headerCell.MergeCell.Top != headerCell.MergeCell.Bottom))
                                {
                                    cellBegin = worksheet.Cells[headerCell.MergeCell.Top + increase, headerCell.MergeCell.Left + increase];
                                    cellEnd = worksheet.Cells[headerCell.MergeCell.Bottom + increase, headerCell.MergeCell.Right + increase];

                                    worksheet.Range[cellBegin, cellEnd].Merge();
                                }
                            }
                        }

                        headerCount++;
                    }

                    //Freeze
                    if (headerCount != 0)
                    {
                        worksheet.Application.ActiveWindow.SplitRow = headerCount;
                        worksheet.Application.ActiveWindow.FreezePanes = true;
                    }
                }

                //Format titles
                if (headerCount != 0 && _Body_Datas.Count != 0)
                {
                    var header_range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[headerCount, colCount]];
                    header_range.Font.Size = 11;
                    header_range.Font.Bold = true;
                    header_range.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    header_range.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                }
                if (_Body_Datas != null && _Body_Datas.Count != 0)
                {
                    int begin = _EndHeader + 1;
                    int excel_row = headerCount + 1;

                    int count = _Body_Datas.Count;

                    //Array contains values
                    object[,] arr = new object[count, colCount];

                    for (int iRow = begin; iRow < count; iRow++)
                    {
                        var rowTable = _Body_Datas[iRow];

                        for (int iCol = 0; iCol < rowTable._CellTables.Count; iCol++)
                        {
                            CellTable cell_data = rowTable._CellTables[iCol];

                            int excel_col = iCol + increase;

                            //Increase index of excel column
                            if (rowTable._Header_Footer == true && iCol == 0)
                            {
                                excel_col += 1;
                                iCol++;
                            }

                            try
                            {
                                string value = cell_data.Text;

                                if (iCol == 0)
                                {
                                    if (eUIDFlagExport == UIDFlagExport.Type)
                                    {
                                        if (rowTable._Element != null)
                                        {
                                            var typeId = GetFamilyType(rowTable._Element);
                                            if (typeId != ElementId.InvalidElementId)
                                            {
                                                var type = doc.GetElement(typeId);
                                                value = type.UniqueId.ToString();
                                            }
                                        }
                                        else if (cell_data.Text != null && cell_data.Text != string.Empty)
                                        {
                                            string uniqueId = cell_data.Text;
                                            var type = doc.GetElement(uniqueId);
                                            if (type != null)
                                            {
                                                value = type.UniqueId.ToString();
                                            }
                                        }
                                    }
                                    else if (eUIDFlagExport == UIDFlagExport.Instance)
                                    {
                                        if (rowTable._Element != null)
                                        {
                                            value = rowTable._Element.UniqueId.ToString();
                                        }

                                        //UID
                                        if (schedule.Definition.IsItemized == false && rowTable._Header_Footer == false)
                                        {
                                            //Does not write UID with _GroupByFamilyType  = true in sheet Instance
                                            if (_GroupByFamilyType == true)
                                                value = string.Empty;
                                        }
                                    }

                                    //UID
                                    if (schedule.Definition.IsItemized == false && rowTable._Header_Footer == false)
                                    {
                                        if (_GroupByFamilyType == false)
                                            value = string.Empty;
                                    }
                                }
                                else if (iCol > 1)
                                {
                                    //Set link value
                                    if (eUIDFlagExport == UIDFlagExport.Type || eUIDFlagExport == UIDFlagExport.Instance)
                                    {
                                        string columnName = ExcelUtils.GetExcelColumnName(iCol);
                                        string cellLink = string.Format("{0}!{1}{2}", _SheetScheduleName, columnName, excel_row + 1);
                                        string formula = string.Format("=IF({0}=\"\",\"\",{1})", cellLink, cellLink);

                                        value = formula;
                                    }
                                }
                                arr[iRow - begin, iCol] = value;
                            }
                            catch (System.Exception ex)
                            {
                                string mess = ex.Message;
                            }
                        }
                        excel_row++;
                    }

                    //Format for range
                    var data_range = worksheet.Range[worksheet.Cells[headerCount + 1, 1], worksheet.Cells[excel_row, colCount]];
                    data_range.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    data_range.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;

                    //Format for column before set values
                    if (_DataTypeOf_Column.Count != 0)
                    {
                        if (eUIDFlagExport == UIDFlagExport.None)
                        {
                            foreach (KeyValuePair<int, sDataType> kvp in _DataTypeOf_Column)
                            {
                                var range = worksheet.Range[worksheet.Cells[headerCount + 1, kvp.Key], worksheet.Cells[excel_row, kvp.Key]];

                                if (kvp.Value == sDataType.General)
                                    range.NumberFormat = "";
                                else if (kvp.Value == sDataType.Text)
                                    range.NumberFormat = "@";
                            }
                        }
                    }

                    //Set values for range
                    data_range.Value2 = arr;
                }

                // Auto fit all Columns in the range
                worksheet.Columns.EntireColumn.AutoFit();

                //Schedule Name
                worksheet.Rows[1].Insert();

                var cell = worksheet.Cells[1, 1];
                cell.Value = schedule.Name;

                cell.Font.Bold = true;
                cell.Font.Size = 18;
                cell.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;

                if (_Body_Datas != null && _Body_Datas.Count != 0)
                {
                    //Merge schedule name
                    cellEnd = worksheet.Cells[1, colCount];
                    worksheet.Range[cell, cellEnd].Merge();
                }
            }
            catch (Exception ex)
            {
                isHasError = true;
                strLog.AppendLine("-----------------------");
                strLog.AppendLine(ex.Message);
                strLog.AppendLine("-----------------------");
                // show form log
                if (strLog.Length != 0)
                {
                    RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(_CmpAttribute, strLog);
                    frmLog.ShowDialog();
                }
            }
        }

        #endregion Member Functions
    }
}