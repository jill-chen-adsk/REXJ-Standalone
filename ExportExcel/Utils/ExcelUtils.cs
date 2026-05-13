using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ADSK.JExtRAC.ExportExcel.Entities;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Parameter = Autodesk.Revit.DB.Parameter;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using System.IO;
using System.Text;
using Range = Microsoft.Office.Interop.Excel.Range;
using RvtExtApp = ADSK.JExtRAC.ExportExcel;

namespace ADSK.JExtRAC.ExportExcel.Utils
{
    public class ExcelUtils
    {
        private static Dictionary<int, sDataType> _dataTypeOfColumn = null;

        public static Workbook GetExcelWorkBook(bool isAlwaysNewBook = true)
        {
            Application application = (Application)Interaction.CreateObject("Excel.Application", "");
            application.IgnoreRemoteRequests = true;
            application.Visible = false;

            try
            {
                Workbook workbook = !isAlwaysNewBook ? application.ActiveWorkbook ?? application.Workbooks.Add(Missing.Value) : application.Workbooks.Add(Missing.Value);
                return workbook;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                return null;
            }
        }

        public static void FormatTitleCell(int iRow, int iCol, string title, float width, Worksheet sh)
        {
            Range cell = sh.Cells[iRow, iCol];
            cell.Value = title;
            cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            cell.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            cell.Font.Size = (float)10.5;
            cell.Font.ColorIndex = 2;
            cell.Interior.ColorIndex = 54;
            cell.Interior.Pattern = Microsoft.Office.Interop.Excel.Constants.xlBoth;
            cell.Interior.PatternColorIndex = Microsoft.Office.Interop.Excel.Constants.xlAutomatic;
            cell.ColumnWidth = width;
        }

        private static int ExportParamHeader(Worksheet sh, List<string> parameterNames)
        {
            sh.Rows[1].RowHeight = 35.0;

            int flag = GetTypeOrInstanceFlag(parameterNames);
            if (flag == 0)
                FormatTitleCell(1, 1, "T:UID", 15f, sh);
            else
                FormatTitleCell(1, 1, "I:UID", 15f, sh);

            FormatTitleCell(1, 2, "Family\r\nCategory", 15f, sh);
            FormatTitleCell(1, 3, "Type", 15f, sh);
            int iCol = 4;
            int num1 = 0;
            int num2 = checked(parameterNames.Count - 1);
            int index = num1;
            while (index <= num2)
            {
                string parameter = parameterNames[index];
                FormatCell(1, iCol, parameter, 10f, sh);
                checked { ++iCol; }
                checked { ++index; }
            }

            sh.Application.ActiveWindow.SplitRow = 1;
            sh.Application.ActiveWindow.FreezePanes = true;

            return checked(iCol - 1);
        }

        public static void FormatCell(int iRow, int iCol, string value, float width, Worksheet sh)
        {
            Range cell = sh.Cells[iRow, iCol];
            cell.Value = value;
            cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            cell.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            cell.Font.Size = (float)10.5;
            cell.Font.ColorIndex = 54;
            cell.ColumnWidth = width;
        }

        public static int ExportElements(Worksheet wh, RvtExtApp.Components.Parameters cmpParameters, List<Element> elementList, List<string> parameterNames)
        {
            if (elementList.Count == 0)
                return -1;

            _dataTypeOfColumn = new Dictionary<int, sDataType>();

            int flag = GetTypeOrInstanceFlag(parameterNames);
            string pseudoTypeGuid = Resources.Text.IDS_PSEUDO_TYPE_GUID;
            string pseudoCount = Resources.Text.IDS_PSEUDO_COUNT;

            List<ElementId> elementTypes = new List<ElementId>();
            Document document = elementList[0].Document;

            int beginRow = 2;
            int num1 = beginRow;
            int rowIndex = 0;
            try
            {
                int rowCount = elementList.ToList().Count;
                int colCount = 3 + parameterNames.Count;
                object[,] arr = new object[rowCount, colCount];

                List<Element>.Enumerator enumerator = elementList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Element current = enumerator.Current;
                    try
                    {
                        Element elementType = document.GetElement(current.GetTypeId());

                        if (flag == 0)
                        {
                            ElementId typeId = Common.GetFamilyType(current);
                            if (typeId == ElementId.InvalidElementId)
                            {
                                typeId = current.Id;
                                elementType = current;
                            }

                            if (typeId != ElementId.InvalidElementId)
                            {
                                if (elementTypes.Contains(typeId) == true)
                                    continue;
                                elementTypes.Add(typeId);
                            }
                        }

                        int colIndex = 0;
                        object value = string.Empty;
                        if (flag != 0)
                            value = current.UniqueId;
                        else
                            value = elementType.UniqueId;
                        arr[rowIndex, colIndex++] = value;

                        if (current is FamilyInstance)
                        {
                            string name = ((FamilyInstance)current).Symbol.Family.Name;
                            if (!string.IsNullOrEmpty(name)) { value = name; }
                        }
                        else if (current is FamilySymbol)
                        {
                            string name = ((FamilySymbol)current).Family.Name;
                            if (!string.IsNullOrEmpty(name)) { value = name; }
                        }
                        else if (current is View)
                        {
                            View view = (View)current;
                            ViewFamilyType element2 = (ViewFamilyType)document.GetElement(view.GetTypeId());
                            value = element2.Name;
                        }
                        else
                        {
                            value = current.Category.Name;
                        }

                        arr[rowIndex, colIndex++] = value;
                        arr[rowIndex, colIndex++] = current.Name;

                        int num3 = parameterNames.Count - 1;
                        int index = 0;
                        while (index <= num3)
                        {
                            string parameterName = parameterNames[index];
                            Parameter parameter1 = (Parameter)null;
                            value = string.Empty;

                            if (parameterName.StartsWith(Setting._Prefix_Type) & !Information.IsNothing(elementType))
                            {
                                string str = parameterName.Remove(0, 2);

                                if (Operators.CompareString(str, pseudoTypeGuid, false) == 0)
                                {
                                    value = elementType.UniqueId;
                                }
                                else
                                    parameter1 = cmpParameters.FindParameter(elementType, str);
                            }
                            else if (parameterName.StartsWith(Setting._Prefix_Instance))
                            {
                                string str = parameterName.Remove(0, 2);
                                parameter1 = cmpParameters.FindParameter(current, str);
                            }

                            if (parameter1 == null)
                            {
                                if (parameterName.StartsWith(Setting._Prefix_Type) || parameterName.StartsWith(Setting._Prefix_Instance))
                                {
                                    parameterName = parameterName.Remove(0, 2);
                                }

                                if (Operators.CompareString(parameterName, pseudoCount, false) == 0)
                                {
                                    value = string.Empty;
                                    if (flag != 0)
                                        value = 1;
                                    else
                                    {
                                        int count = 0;
                                        if (!Information.IsNothing(elementType))
                                        {
                                            FilteredElementCollector elementCollector = new FilteredElementCollector(document).WhereElementIsNotElementType();
                                            elementCollector.OfCategory((BuiltInCategory)Int32.Parse(elementType.Category.Id.ToString()));
                                            foreach (Element element in elementCollector.ToElements())
                                            {
                                                ElementId typeId = Common.GetFamilyType(element);
                                                if (typeId != elementType.Id)
                                                    continue;
                                                count++;
                                            }
                                        }
                                        value = count;
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "ToRoom", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Room toRoom = familyInstance.ToRoom;
                                        if (toRoom != null) { value = toRoom.UniqueId; }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "FromRoom", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Room fromRoom = familyInstance.FromRoom;
                                        if (fromRoom != null) { value = fromRoom.UniqueId; }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "Room", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Room room = familyInstance.Room;
                                        if (room != null) { value = room.UniqueId; }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "Space", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Space space = familyInstance.Space;
                                        if (space != null) { value = space.UniqueId; }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "Host", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Element host = familyInstance.Host;
                                        if (host != null) { value = host.UniqueId; }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "SpaceName", false) == 0)
                                {
                                    FamilyInstance familyInstance = current as FamilyInstance;
                                    if (familyInstance != null)
                                    {
                                        Space space = familyInstance.Space;
                                        if (space != null)
                                        {
                                            Parameter parameter2 = space.get_Parameter(BuiltInParameter.ROOM_NAME);
                                            string str = string.Empty;
                                            if (!Information.IsNothing(parameter2))
                                                str = parameter2.AsString();
                                            value = str;
                                        }
                                    }
                                }
                                else if (Operators.CompareString(parameterName, "ID", false) == 0)
                                {
                                    value = current.Id.Value.ToString();
                                }
                                else
                                    parameter1 = cmpParameters.FindParameter(current, parameterName);
                            }

                            if (parameter1 != null)
                            {
                                string str = "";
                                object parameterValue = Common.GetParameterValue(document, parameter1);
                                if (parameterValue != null)
                                    str = parameterValue.ToString();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    value = str;
                                }
                            }

                            sDataType _sDataType = sDataType.General;
                            if (_sDataType == sDataType.General && parameter1 != null)
                            {
                                if (parameter1.Definition.GetDataType() == SpecTypeId.String.Text || parameter1.Definition.GetDataType() == SpecTypeId.String.Url || parameter1.Definition.GetDataType() == SpecTypeId.String.MultilineText)
                                    _sDataType = sDataType.Text;
                            }

                            if (_dataTypeOfColumn.ContainsKey(colIndex + 1) == false)
                            {
                                _dataTypeOfColumn.Add(colIndex + 1, _sDataType);
                            }

                            arr[rowIndex, colIndex++] = value;
                            ++index;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMsg = ex.Message;
                        continue;
                    }

                    ++num1;
                    ++rowIndex;
                }

                if (_dataTypeOfColumn.Count != 0)
                {
                    foreach (KeyValuePair<int, sDataType> kvp in _dataTypeOfColumn)
                    {
                        Range range = wh.Range[wh.Cells[beginRow, kvp.Key], wh.Cells[rowCount + beginRow, kvp.Key]];
                        if (kvp.Value == sDataType.General)
                            range.NumberFormat = "";
                        else if (kvp.Value == sDataType.Text)
                            range.NumberFormat = "@";
                    }
                }

                var cell1 = wh.Cells[beginRow, 1];
                var cell2 = wh.Cells[rowCount + (beginRow - 1), colCount];
                wh.Range[cell1, cell2].Value2 = arr;
            }
            finally
            {
            }

            return num1 - 1;
        }

        public static Result Export(Document doc, RvtExtApp.Components.Parameters cmpParameters, Dictionary<CategoryItem, List<ParameterData>> dic_categories, List<Element> elements, string pathExcel)
        {
            if (dic_categories.Count == 0)
                return Result.Cancelled;

            List<string> parameterList = new List<string>();
            List<Element> elementList = new List<Element>();

            foreach (KeyValuePair<CategoryItem, List<ParameterData>> key_category in dic_categories)
            {
                foreach (ParameterData parameterData in key_category.Value)
                {
                    if (parameterList.Contains(parameterData.ToString()) == false)
                    {
                        parameterList.Add(parameterData.ToString());
                    }
                }

                var elementsIncategory = (from Element element in elements where element.Category != null && element.Category.Id == key_category.Key.ElementId select element).ToList();
                elementList.AddRange(elementsIncategory);
            }

            if (parameterList.Count == 0 || elementList.Count == 0)
                return Result.Cancelled;

            var fileExtension = Path.GetExtension(pathExcel);
            var isCsv = fileExtension.ToLower() == ".csv";
            if (isCsv)
            {
                return ExportCsv(cmpParameters, elementList, parameterList, pathExcel);
            }

            try
            {
                Workbook workbook = GetExcelWorkBook(true);
                if (workbook == null)
                {
                    return Result.Cancelled;
                }

                var application = workbook.Application;
                var worksheets = workbook.Sheets as Sheets;
                Worksheet worksheet = null;
                if (worksheets.Count == 0)
                    worksheet = (Worksheet)workbook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                else
                    worksheet = (Worksheet)workbook.Worksheets[1];

                ExportParamHeader(worksheet, parameterList);
                ExportElements(worksheet, cmpParameters, elementList, parameterList);
                worksheet.Columns.EntireColumn.AutoFit();

                string ext = Path.GetExtension(pathExcel);
                if (ext.ToUpper() == ".XLS")
                    workbook.SaveAs(pathExcel, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                else
                    workbook.SaveAs(pathExcel, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);

                workbook.Close(false);
                application.IgnoreRemoteRequests = false;
                application.Quit();

                if (worksheet != null)
                    Marshal.ReleaseComObject(worksheet);
                if (worksheets != null)
                    Marshal.ReleaseComObject(worksheets);
                if (workbook != null)
                    Marshal.ReleaseComObject(workbook);
                if (application != null)
                    Marshal.ReleaseComObject(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return Result.Failed;
            }
        }

        private static int GetTypeOrInstanceFlag(List<string> parameterNames)
        {
            string pseudoCount = Resources.Text.IDS_PSEUDO_COUNT;
            int flag = 0;

            var paraTypes = parameterNames.Where(item => item == pseudoCount || item.StartsWith("T:")).ToList();
            if (paraTypes.Count == parameterNames.Count)
                flag = 0;
            else if (paraTypes.Count == 0)
                flag = 1;
            else
                flag = 2;

            return flag;
        }

        private static Result ExportCsv(RvtExtApp.Components.Parameters cmpParameters, List<Element> elementList, List<string> parameterNames, string filePath)
        {
            if (elementList.Count == 0) return Result.Cancelled;

            string pseudoTypeGuid = Resources.Text.IDS_PSEUDO_TYPE_GUID;
            string pseudoCount = Resources.Text.IDS_PSEUDO_COUNT;

            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var str = new StringBuilder();
                var document = elementList[0].Document;

                var flag = GetTypeOrInstanceFlag(parameterNames);
                var elementTypes = new List<ElementId>();
                _dataTypeOfColumn = new Dictionary<int, sDataType>();

                var typeOrInstanceStr = flag == 0 ? "T" : "I";
                str.Append($"{typeOrInstanceStr}:UID,\"Family\nCategory\",Type,");
                for (var i = 0; i < parameterNames.Count; i++)
                {
                    str.Append(EscapedField(parameterNames[i]));
                    str.Append(i < parameterNames.Count - 1 ? "," : "\r\n");
                }

                using (var enumerator = elementList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current is null) continue;
                        try
                        {
                            var elementType = document.GetElement(current.GetTypeId());
                            if (flag == 0)
                            {
                                var typeId = Common.GetFamilyType(current);
                                if (typeId == ElementId.InvalidElementId) (typeId, elementType) = (current.Id, current);
                                if (typeId != ElementId.InvalidElementId && !elementTypes.Contains(typeId)) elementTypes.Add(typeId);
                            }

                            var uid = flag != 0 ? current.UniqueId : elementType.UniqueId;
                            str.Append(EscapedField(uid));

                            var familyCategoryName = string.Empty;
                            switch (current)
                            {
                                case FamilyInstance instance:
                                    {
                                        var name = instance.Symbol.Family.Name;
                                        if (!string.IsNullOrEmpty(name)) { familyCategoryName = name; }
                                        break;
                                    }
                                case FamilySymbol symbol:
                                    {
                                        var name = symbol.Family.Name;
                                        if (!string.IsNullOrEmpty(name)) { familyCategoryName = name; }
                                        break;
                                    }
                                case View view1:
                                    {
                                        var view = view1;
                                        var element2 = (ViewFamilyType)document.GetElement(view.GetTypeId());
                                        familyCategoryName = element2.Name;
                                        break;
                                    }
                                default:
                                    familyCategoryName = current.Category.Name;
                                    break;
                            }

                            str.Append($",{EscapedField(familyCategoryName)},{EscapedField(current.Name)}");

                            for (int i = 0; i < parameterNames.Count; i++)
                            {
                                var parameterName = parameterNames[i];
                                var parameter1 = (Parameter)null;
                                var value = string.Empty;

                                if (parameterName.StartsWith(Setting._Prefix_Type) & !Information.IsNothing(elementType))
                                {
                                    var str_ = parameterName.Remove(0, 2);
                                    if (Operators.CompareString(str_, pseudoTypeGuid, false) == 0)
                                    {
                                        value = elementType.UniqueId;
                                    }
                                    else
                                        parameter1 = cmpParameters.FindParameter(elementType, str_);
                                }
                                else if (parameterName.StartsWith(Setting._Prefix_Instance))
                                {
                                    var str_ = parameterName.Remove(0, 2);
                                    parameter1 = cmpParameters.FindParameter(current, str_);
                                }

                                if (parameter1 == null)
                                {
                                    if (parameterName.StartsWith(Setting._Prefix_Type) || parameterName.StartsWith(Setting._Prefix_Instance))
                                    {
                                        parameterName = parameterName.Remove(0, 2);
                                    }

                                    if (Operators.CompareString(parameterName, pseudoCount, false) == 0)
                                    {
                                        value = string.Empty;
                                        if (flag != 0)
                                            value = "1";
                                        else
                                        {
                                            var count = 0;
                                            if (!Information.IsNothing(elementType))
                                            {
                                                var elementCollector = new FilteredElementCollector(document).WhereElementIsNotElementType();
                                                elementCollector.OfCategory((BuiltInCategory)Int32.Parse(elementType.Category.Id.ToString()));
                                                foreach (var element in elementCollector.ToElements())
                                                {
                                                    var typeId = Common.GetFamilyType(element);
                                                    if (typeId != elementType.Id)
                                                        continue;
                                                    count++;
                                                }
                                            }
                                            value = count.ToString();
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "ToRoom", false) == 0)
                                    {
                                        var familyInstance = current as FamilyInstance;
                                        var toRoom = familyInstance?.ToRoom;
                                        if (toRoom != null) value = toRoom.UniqueId;
                                    }
                                    else if (Operators.CompareString(parameterName, "FromRoom", false) == 0)
                                    {
                                        if (current is FamilyInstance familyInstance)
                                        {
                                            var fromRoom = familyInstance.FromRoom;
                                            if (fromRoom != null) value = fromRoom.UniqueId;
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "Room", false) == 0)
                                    {
                                        if (current is FamilyInstance familyInstance)
                                        {
                                            var room = familyInstance.Room;
                                            if (room != null) value = room.UniqueId;
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "Space", false) == 0)
                                    {
                                        if (current is FamilyInstance familyInstance)
                                        {
                                            var space = familyInstance.Space;
                                            if (space != null) value = space.UniqueId;
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "Host", false) == 0)
                                    {
                                        if (current is FamilyInstance familyInstance)
                                        {
                                            var host = familyInstance.Host;
                                            if (host != null) value = host.UniqueId;
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "SpaceName", false) == 0)
                                    {
                                        if (current is FamilyInstance familyInstance)
                                        {
                                            var space = familyInstance.Space;
                                            if (space != null)
                                            {
                                                var parameter2 = space.get_Parameter(BuiltInParameter.ROOM_NAME);
                                                var str_ = string.Empty;
                                                if (!Information.IsNothing(parameter2)) str_ = parameter2.AsString();
                                                value = str_;
                                            }
                                        }
                                    }
                                    else if (Operators.CompareString(parameterName, "ID", false) == 0)
                                    {
                                        value = current.Id.Value.ToString();
                                    }
                                    else
                                        parameter1 = cmpParameters.FindParameter(current, parameterName);
                                }

                                if (parameter1 != null)
                                {
                                    var str_ = "";
                                    var parameterValue = Common.GetParameterValue(document, parameter1);
                                    if (parameterValue != null) str_ = parameterValue.ToString();
                                    if (!string.IsNullOrEmpty(str_)) value = str_;
                                }

                                str.Append($",{EscapedField(value)}");
                            }

                            str.Append("\r\n");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return Result.Failed;
                        }
                    }

                    sw.Write(str.ToString());
                }
            }

            return Result.Succeeded;
        }

        private static string EscapedField(string str)
        {
            str = str.Replace("\"", "\"\"");
            if (str.Contains(",") || str.Contains("\"") || str.Contains("\n"))
            {
                str = $"\"{str}\"";
            }
            return str;
        }
    }
}
