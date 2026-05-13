using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.PrintRegion
{
    /// ================================================================================
    /// <summary>PrintSTP</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public class PrintSTP : ISettingNameOperation
    {
        //Member Variables

        #region Member Variables

        /// <summary>ExternalCommandData</summary>
        private ExternalCommandData m_commandData;

        /// <summary>PrintManager</summary>
        private PrintManager m_printMgr;

        /// <summary>Localized strings and images</summary>
        private ADSK.JExtRAC.PrintRegion.Components.Attribute _cmpAttribute;

        #endregion Member Variables

        //Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="commandData">ExternalCommandData</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public PrintSTP(ExternalCommandData commandData, ADSK.JExtRAC.PrintRegion.Components.Attribute cmpAttribute)
        {
            m_commandData = commandData;
            m_printMgr = commandData.Application.ActiveUIDocument.Document.PrintManager;
            _cmpAttribute = cmpAttribute;
        }

        #endregion Constructor

        //Member functions

        #region Member functions

        /// ================================================================================
        /// <summary>Save</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool Save()
        {
            try
            {
                return m_printMgr.PrintSetup.Save();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
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
                m_printMgr.PrintSetup.Revert();
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
                return m_printMgr.PrintSetup.Delete();
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        /// ================================================================================
        /// <summary>VerifyMarginType</summary>
        ///
        /// <param name="controlToEnableOrNot">Control</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool VerifyMarginType(System.Windows.Forms.Control controlToEnableOrNot)
        {
            // Enable terms (or):
            // 1. Paper placement is LowerLeft.
            return controlToEnableOrNot.Enabled =
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement == PaperPlacementType.LowerLeft;
        }

        /// ================================================================================
        /// <summary>VerifyUserDefinedMargin</summary>
        ///
        /// <param name="controlsToEnableOrNot">Control</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool VerifyUserDefinedMargin(Collection<System.Windows.Forms.Control> controlsToEnableOrNot)
        {
            bool enableOrNot =
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType == MarginType.UserDefined;

            foreach (System.Windows.Forms.Control control in controlsToEnableOrNot)
            {
                control.Enabled = enableOrNot;
            }

            return enableOrNot;
        }

        /// ================================================================================
        /// <summary>Print the views and sheets defined in the current local PrintManager settings.</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool SubmitPrint()
        {
            return m_printMgr.SubmitPrint();
        }

        /// ================================================================================
        /// <summary>SettingCount</summary>
        ///
        /// <param name="newName">new name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool SaveAs(string newName)
        {
            try
            {
                return m_printMgr.PrintSetup.SaveAs(newName);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        /// ================================================================================
        /// <summary>SettingCount</summary>
        ///
        /// <param name="name">new name</param>
        /// <returns></returns>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool Rename(string name)
        {
            try
            {
                return m_printMgr.PrintSetup.Rename(name);
            }
            catch (Exception ex)
            {
                PrintMgr.MyMessageBox(ex.Message);
                return false;
            }
        }

        #endregion Member functions

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>Printer Name</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string PrinterName
        {
            get
            {
                return m_printMgr.PrinterName;
            }
            set
            {
                try
                {
                    m_printMgr.SelectNewPrintDriver(value);
                }
                catch (Exception ex)
                {
                    var mess = ex.Message;
                    // un-available or exceptional printer
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
                return "Default ";
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
                return m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds().Count;
            }
        }

        /// ================================================================================
        /// <summary>PrinSettingName</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public List<string> PrintSettingNames
        {
            get
            {
                List<string> names = new List<string>();
                //foreach (Element printSetting in m_commandData.Application.ActiveUIDocument.Document.PrintSettings)
                ICollection<ElementId> printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();
                foreach (ElementId eid in printSettingIds)
                {
                    Element printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                    names.Add(printSetting.Name);
                }
                names.Add(ConstData.InSessionName);
                return names;
            }
        }

        /// ================================================================================
        /// <summary>SettingName</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string SettingName
        {
            get
            {
                IPrintSetting setting = m_printMgr.PrintSetup.CurrentPrintSetting;
                return (setting is PrintSetting) ?
                    (setting as PrintSetting).Name : ConstData.InSessionName;
            }
            set
            {
                //PrintSetting print;
                try
                {
                    if (value == ConstData.InSessionName)
                    {
                        m_printMgr.PrintSetup.CurrentPrintSetting = m_printMgr.PrintSetup.InSession;
                        return;
                    }
                    //foreach (Element printSetting in m_commandData.Application.ActiveUIDocument.Document.PrintSettings)
                    ICollection<ElementId> printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();

                    foreach (ElementId eid in printSettingIds)
                    {
                        if (eid == ElementId.InvalidElementId)
                            continue;
                        Element printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                        if (printSetting == null)
                            continue;
                        if (printSetting.Name.Equals(value))
                        {
                            m_printMgr.PrintSetup.CurrentPrintSetting = printSetting as PrintSetting;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ICollection<ElementId> printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();
                    foreach (ElementId eid in printSettingIds)
                    {
                        if (eid == ElementId.InvalidElementId)
                            continue;
                        Element printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                        if (printSetting == null)
                            continue;
                        if (printSetting.Name.Equals(value))
                        {
                            // delete print setting
                            m_commandData.Application.ActiveUIDocument.Document.Delete(eid);
                            m_commandData.Application.ActiveUIDocument.Document.Regenerate();

                            break;
                        }
                    }

                    bool isCreated = m_printMgr.PrintSetup.SaveAs(value);
                    if (isCreated)
                    {
                        printSettingIds = m_commandData.Application.ActiveUIDocument.Document.GetPrintSettingIds();
                        foreach (ElementId eid in printSettingIds)
                        {
                            if (eid == ElementId.InvalidElementId)
                                continue;
                            Element printSetting = m_commandData.Application.ActiveUIDocument.Document.GetElement(eid);
                            if (printSetting == null)
                                continue;
                            if (printSetting.Name.Equals(value))
                            {
                                m_printMgr.PrintSetup.CurrentPrintSetting = printSetting as PrintSetting;

                                break;
                            }
                        }
                    }

                    var mess = ex.Message;
                }
            }
        }

        /// ================================================================================
        /// <summary>PaperSizes</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public List<string> PaperSizes
        {
            get
            {
                List<string> names = new List<string>();
                foreach (PaperSize ps in m_printMgr.PaperSizes)
                {
                    names.Add(ps.Name);
                }
                return names;
            }
        }

        /// ================================================================================
        /// <summary>PaperSize</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string PaperSize
        {
            get
            {
                try
                {
                    return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize.Name;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                foreach (PaperSize ps in m_printMgr.PaperSizes)
                {
                    if (ps.Name.Equals(value))
                    {
                        m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize = ps;
                        break;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>PaperSources</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public List<string> PaperSources
        {
            get
            {
                List<string> names = new List<string>();
                foreach (PaperSource ps in m_printMgr.PaperSources)
                {
                    names.Add(ps.Name);
                }
                return names;
            }
        }

        /// ================================================================================
        /// <summary>PaperSource</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public string PaperSource
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSource.Name;
            }
            set
            {
                foreach (PaperSource ps in m_printMgr.PaperSources)
                {
                    if (ps.Name.Equals(value))
                    {
                        m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSource = ps;
                        break;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>PageOrientation</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public PageOrientationType PageOrientation
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation = value;
            }
        }

        /// ================================================================================
        /// <summary>PaperPlacement</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public PaperPlacementType PaperPlacement
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement = value;
            }
        }

        /// ================================================================================
        /// <summary>MarginTypes</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public Array MarginTypes
        {
            get
            {
                List<string> items = new List<string>();
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_MARGIN_NO_MARGIN"));
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_MARGIN_PRINTER_LIMIT"));
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_MARGIN_USER_DEFINED"));
                return items.ToArray();
            }
        }

        /// ================================================================================
        /// <summary>SelectedMarginType</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public MarginType SelectedMarginType
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType;
            }
            set
            {
                if (PaperPlacement == PaperPlacementType.LowerLeft)
                    m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType = value;
            }
        }

        /// ================================================================================
        /// <summary>OriginOffsetX</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public double OriginOffsetX
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetX;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetX = value;
            }
        }

        /// ================================================================================
        /// <summary>OriginOffsetY</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public double OriginOffsetY
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetY;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetY = value;
            }
        }

        /// ================================================================================
        /// <summary>HiddenLineViews</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public HiddenLineViewsType HiddenLineViews
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews = value;
            }
        }

        /// ================================================================================
        /// <summary>Zoom</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public int Zoom
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.Zoom = value;
            }
        }

        /// ================================================================================
        /// <summary>ZoomType</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public ZoomType ZoomType
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ZoomType = value;
            }
        }

        /// ================================================================================
        /// <summary>RasterQualities</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public Array RasterQualities
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(_cmpAttribute.ResourceText("IDS_TXT_RASTER_LOW"));
                list.Add(_cmpAttribute.ResourceText("IDS_TXT_RASTER_MEDIUM"));
                list.Add(_cmpAttribute.ResourceText("IDS_TXT_RASTER_HIGH"));
                list.Add(_cmpAttribute.ResourceText("IDS_TXT_RASTER_PRESENTATION"));
                return list.ToArray();
            }
        }

        /// ================================================================================
        /// <summary>RasterQualities</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public RasterQualityType RasterQuality
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality = value;
            }
        }

        /// ================================================================================
        /// <summary>Colors</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public Array Colors
        {
            get
            {
                List<string> items = new List<string>();
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_COLOR_MONOCHROME"));
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_COLOR_GRAYSCALE"));
                items.Add(_cmpAttribute.ResourceText("IDS_TXT_COLOR_COLOR"));
                return items.ToArray();
            }
        }

        /// ================================================================================
        /// <summary>Colors</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public ColorDepthType Color
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth = value;
            }
        }

        /// ================================================================================
        /// <summary>ViewLinksinBlue</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool ViewLinksinBlue
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ViewLinksinBlue;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ViewLinksinBlue = value;
            }
        }

        /// ================================================================================
        /// <summary>HideScopeBoxes</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool HideScopeBoxes
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes = value;
            }
        }

        /// ================================================================================
        /// <summary>HideReforWorkPlanes</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool HideReforWorkPlanes
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes = value;
            }
        }

        /// ================================================================================
        /// <summary>HideCropBoundaries</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool HideCropBoundaries
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries = value;
            }
        }

        /// ================================================================================
        /// <summary>HideUnreferencedViewTags</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool HideUnreferencedViewTags
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags = value;
            }
        }

        /// ================================================================================
        /// <summary>RegionEdgesMaskCoincidentLines</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool RegionEdgesMaskCoincidentLines
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MaskCoincidentLines;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.MaskCoincidentLines = value;
            }
        }

        /// ================================================================================
        /// <summary>ReplaceHaftoneWithThinLines</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public bool ReplaceHaftoneWithThinLines
        {
            get
            {
                return m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ReplaceHalftoneWithThinLines;
            }
            set
            {
                m_printMgr.PrintSetup.CurrentPrintSetting.PrintParameters.ReplaceHalftoneWithThinLines = value;
            }
        }

        #endregion Properties
    }
}