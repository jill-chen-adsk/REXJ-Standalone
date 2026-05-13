using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.PrintRegion
{
    /// ================================================================================
    /// <summary>VisibleType</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public enum VisibleType
    {
        VT_ViewOnly,
        VT_SheetOnly,
        VT_BothViewAndSheet,
        VT_None
    }

    /// ================================================================================
    /// <summary>Interface ISettingNameOperation</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public interface ISettingNameOperation
    {
        /// ================================================================================
        /// <summary>SettingName </summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        string SettingName
        {
            get;
            set;
        }

        /// ================================================================================
        /// <summary>Prefix</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        string Prefix
        {
            get;
        }

        /// ================================================================================
        /// <summary>SettingCount</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        int SettingCount
        {
            get;
        }

        /// ================================================================================
        /// <summary>Rename</summary>
        ///
        /// <param name="name">string name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        bool Rename(string name);

        /// ================================================================================
        /// <summary>Save as</summary>
        ///
        /// <param name="newName">string name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        bool SaveAs(string newName);
    }

    /// ================================================================================
    /// <summary>Class SettingName </summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public static class ConstData
    {
        /// <summary>InSessionName</summary>
        public const string InSessionName = "<In-Session>";
    }

    /// ================================================================================
    /// <summary>Exposes the View/Sheet Set interfaces just like the View/Sheet Set Dialog (File->Print...; selected views/sheets->Select...) in UI.</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public class ViewSheets : ISettingNameOperation
    {
        //Member variables

        #region Member variables

        /// <summary>Document</summary>
        private Document m_doc;

        /// <summary>ViewSheetSetting</summary>
        private ViewSheetSetting m_viewSheetSetting;

        #endregion Member variables

        //Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///<param name = "doc" > document</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public ViewSheets(Document doc)
        {
            m_doc = doc;
            m_viewSheetSetting = doc.PrintManager.ViewSheetSetting;
        }

        #endregion Constructor

        // Member function

        #region Member function

        /// ================================================================================
        /// <summary>Save</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool Save()
        {
            try
            {
                return m_viewSheetSetting.Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Revert</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public void Revert()
        {
            try
            {
                m_viewSheetSetting.Revert();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
            }
        }

        /// ================================================================================
        /// <summary>Delete</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool Delete()
        {
            try
            {
                return m_viewSheetSetting.Delete();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        /// ================================================================================
        /// <summary>AvailableViewSheetSet</summary>
        ///
        /// <param name="visibleType">VisibleType</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public List<Autodesk.Revit.DB.View> AvailableViewSheetSet(VisibleType visibleType)
        {
            if (visibleType == VisibleType.VT_None)
                return null;

            List<Autodesk.Revit.DB.View> views = new List<Autodesk.Revit.DB.View>();
            foreach (Autodesk.Revit.DB.View view in m_viewSheetSetting.AvailableViews)
            {
                if (view.ViewType == Autodesk.Revit.DB.ViewType.DrawingSheet
                    && visibleType == VisibleType.VT_ViewOnly)
                {
                    continue;   // filter out sheets.
                }
                if (view.ViewType != Autodesk.Revit.DB.ViewType.DrawingSheet
                    && visibleType == VisibleType.VT_SheetOnly)
                {
                    continue;   // filter out views.
                }

                views.Add(view);
            }

            return views;
        }

        /// ================================================================================
        /// <summary>IsSelected</summary>
        ///
        /// <param name="viewName">view name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool IsSelected(string viewName)
        {
            foreach (Autodesk.Revit.DB.View view in m_viewSheetSetting.CurrentViewSheetSet.Views)
            {
                if (viewName.Equals(view.ViewType.ToString() + ": " + view.Name))
                {
                    return true;
                }
            }

            return false;
        }

        /// ================================================================================
        /// <summary>ChangeCurrentViewSheetSet</summary>
        ///
        /// <param name="names"> list name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public void ChangeCurrentViewSheetSet(List<string> names)
        {
            ViewSet selectedViews = new ViewSet();

            if (null != names && 0 < names.Count)
            {
                foreach (Autodesk.Revit.DB.View view in m_viewSheetSetting.AvailableViews)
                {
                    if (names.Contains(view.ViewType.ToString() + ": " + view.Name))
                    {
                        selectedViews.Insert(view);
                    }
                }
            }

            IViewSheetSet viewSheetSet = m_viewSheetSetting.CurrentViewSheetSet;
            viewSheetSet.Views = selectedViews;
            Save();
        }

        #endregion Member function

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>SettingName</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string SettingName
        {
            get
            {
                IViewSheetSet theSet = m_viewSheetSetting.CurrentViewSheetSet;
                return (theSet is ViewSheetSet) ?
                    (theSet as ViewSheetSet).Name : ConstData.InSessionName;
            }
            set
            {
                if (value == ConstData.InSessionName)
                {
                    m_viewSheetSetting.CurrentViewSheetSet = m_viewSheetSetting.InSession;
                    return;
                }
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc);
                filteredElementCollector.OfClass(typeof(ViewSheetSet));
                IEnumerable<ViewSheetSet> viewSheetSets = filteredElementCollector.Cast<ViewSheetSet>().Where<ViewSheetSet>(viewSheetSet => viewSheetSet.Name.Equals(value as string));
                if (viewSheetSets.Count<ViewSheetSet>() > 0)
                {
                    m_viewSheetSetting.CurrentViewSheetSet = viewSheetSets.First<ViewSheetSet>();
                }
            }
        }

        /// ================================================================================
        /// <summary>Prefix</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string Prefix
        {
            get
            {
                return "Set ";
            }
        }

        /// ================================================================================
        /// <summary>SettingCount</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public int SettingCount
        {
            get
            {
                return (new FilteredElementCollector(m_doc)).OfClass(typeof(ViewSheetSet)).ToElementIds().Count;
            }
        }

        /// ================================================================================
        /// <summary>SaveAs</summary>
        ///<param name="newName">string name</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool SaveAs(string newName)
        {
            try
            {
                return m_viewSheetSetting.SaveAs(newName);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Rename</summary>
        /// <param name="name">string name</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool Rename(string name)
        {
            try
            {
                return m_viewSheetSetting.Rename(name);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        /// ================================================================================
        /// <summary>ViewSheetSetNames</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public List<string> ViewSheetSetNames
        {
            get
            {
                List<string> names = new List<string>();
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc);
                filteredElementCollector.OfClass(typeof(ViewSheetSet));
                foreach (Element element in filteredElementCollector)
                {
                    ViewSheetSet viewSheetSet = element as ViewSheetSet;
                    names.Add(viewSheetSet.Name);
                }
                names.Add(ConstData.InSessionName);

                return names;
            }
        }

        #endregion Properties
    }
}
