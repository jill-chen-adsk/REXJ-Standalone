using ADSK.JExtRAC.ExportExcel.Entities;
using Autodesk.Revit.DB;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Document = Autodesk.Revit.DB.Document;

namespace ADSK.JExtRAC.ExportExcel.Utils
{
    public class Setting
    {
        public static string _Prefix_Type = "T:";
        public static string _Prefix_Instance = "I:";
        public static string _Prefix_Category = "C:";
        public static string _DashLine = "-----";
        public static string _ExportSetting = "ExportSetting";

        public static bool SaveSettingFile(string fileName, string value)
        {
            try
            {
                File.WriteAllText(fileName, value, Encoding.GetEncoding("Shift_JIS"));
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        public static List<CategoryItem> ReadSettingFile(Document doc, string fileName)
        {
            if (fileName == null || fileName == string.Empty)
                return null;

            string[] strArray = null;

            if (File.Exists(fileName) == true)
            {
                strArray = File.ReadAllLines(fileName, Encoding.GetEncoding("Shift_JIS"));
            }
            else
            {
                string[] stringSeparators = new string[] { "\r\n" };
                strArray = fileName.Split(stringSeparators, StringSplitOptions.None);
            }

            return ReadConfig(doc, strArray);
        }

        public static List<CategoryItem> ReadSettingText(Document doc, string text)
        {
            if (text == null || text == string.Empty)
                return null;

            string[] stringSeparators = new string[] { "\r\n" };
            string[] strArray = text.Split(stringSeparators, StringSplitOptions.None);

            return ReadConfig(doc, strArray);
        }

        private static List<CategoryItem> ReadConfig(Document doc, string[] strArray)
        {
            if (strArray == null)
                return null;

            List<CategoryItem> loaded_categories = new List<CategoryItem>();
            CategoryItem categoryItem = null;

            int export_index = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                string line = strArray[i].Trim();

                if (line == string.Empty || line == _DashLine)
                    continue;

                var symbol = line.Substring(0, _Prefix_Category.Length);

                if (symbol.ToUpper() == _Prefix_Category.ToUpper())
                {
                    var categoryName = line.Substring(_Prefix_Category.Length);
                    categoryItem = new CategoryItem(categoryName, ElementId.InvalidElementId);
                    loaded_categories.Add(categoryItem);
                }
                else
                {
                    if (categoryItem == null)
                        continue;

                    string parameterName = string.Empty;

                    var prefix = line.Substring(0, _Prefix_Type.Length);
                    if (prefix.ToUpper() == _Prefix_Type.ToUpper())
                    {
                        parameterName = line.Substring(_Prefix_Type.Length);
                    }
                    else
                    {
                        prefix = line.Substring(0, _Prefix_Instance.Length);
                        if (prefix.ToUpper() == _Prefix_Instance.ToUpper())
                        {
                            parameterName = line.Substring(_Prefix_Instance.Length);
                        }
                        else
                            parameterName = line;
                    }

                    if (parameterName == string.Empty)
                        continue;

                    var parameterData = new ParameterData(parameterName, line);
                    parameterData._IndexExport = export_index++;

                    categoryItem._Parameters.Add(parameterData);
                }
            }

            return loaded_categories;
        }

        public static bool SaveSetting(string section, string key, string value)
        {
            try
            {
                string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                Interaction.SaveSetting(appName, section, key, value);
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        public static bool LoadSetting(string section, string key, ref string value)
        {
            try
            {
                string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                value = Interaction.GetSetting(appName, section, key, value);
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        public static string GetDataConfig(List<CategoryItem> categoryList)
        {
            var export_categories = categoryList.Where(item => item._IsChecked == true).ToList();
            if (export_categories.Count == 0)
                return null;

            StringBuilder strBuilder = new StringBuilder();

            Dictionary<CategoryItem, List<ParameterData>> dic_exports = new Dictionary<CategoryItem, List<ParameterData>>();
            foreach (CategoryItem categoryItem in export_categories)
            {
                var export_parameters = categoryItem._Parameters.Where(item => item._IndexExport != ParameterData._NotExport).ToList();

                if (export_parameters.Count == 0)
                    continue;

                dic_exports.Add(categoryItem, export_parameters);
            }

            var list = dic_exports.Keys.ToList();

            list.Sort(delegate (CategoryItem c1, CategoryItem c2)
            {
                var max1 = c1._Parameters.Max(item => item._IndexExport);
                var max2 = c2._Parameters.Max(item => item._IndexExport);
                return max1.CompareTo(max2);
            });

            foreach (CategoryItem categoryItem in list)
            {
                var export_parameters = dic_exports[categoryItem];

                export_parameters.Sort(delegate (ParameterData p1, ParameterData p2)
                {
                    return p1._IndexExport.CompareTo(p2._IndexExport);
                });

                strBuilder.AppendLine(Setting._Prefix_Category + categoryItem.ToString());

                foreach (ParameterData parameterData in export_parameters)
                {
                    strBuilder.AppendLine(parameterData.ToString());
                }

                strBuilder.AppendLine(Setting._DashLine);
            }
            if (strBuilder.ToString() == string.Empty)
                return null;

            return strBuilder.ToString();
        }
    }
}
