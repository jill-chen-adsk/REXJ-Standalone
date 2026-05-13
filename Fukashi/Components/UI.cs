using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using AdWindows = Autodesk.Windows;
using RvtExtApp = ADSK.Ext.Fukashi;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace ADSK.Ext.Fukashi.Components
{
    internal class UI
    {
        private const string RibbonTabName = "REXJ Standalone";
        private const string RibbonPanelName = "Fukashi";

        private RvtExtApp.Components.Attribute _CmpAttribute;

        private string _AssemblyFolderName;

        private string _OffsetValue;

        private readonly Revit.UI.UIControlledApplication _rvtUICtrlApp;

        internal UI(RvtExtApp.Components.Attribute cmpAttribute,
            Revit.UI.UIControlledApplication rvtUICtrlApp)
        {
            _CmpAttribute = cmpAttribute;

            _AssemblyFolderName = _CmpAttribute.ExecuteFolder + "\\";
            _rvtUICtrlApp = rvtUICtrlApp;
        }

        internal void SetRibbon()
        {
            string assembly = "";
            PushButtonData pushBtnData = null;
            ComboBoxData cmbBoxData = null;
            TextBoxData txtBoxData = null;
            IList<RibbonItemData> itemDatas = new List<RibbonItemData>();

            ContextualHelp contHelp = null;
            string contHelpPath = _AssemblyFolderName +
                                  "Resources" + "\\" +
                                  _CmpAttribute.ResourceText("IDS_TXT_FUKASHIHELPHTM");
            if (System.IO.File.Exists(contHelpPath))
            {
                contHelp = new ContextualHelp(ContextualHelpType.Url, contHelpPath);
            }

            IList<Revit.UI.RibbonPanel> ribbonPanels = GetRibbonPanel(RibbonTabName);
            if (ribbonPanels.Count == 0)
            {
                CreateRibbonTab(RibbonTabName);
            }

            Revit.UI.RibbonPanel ribbonPanel = CreateRibbonPanel(RibbonTabName, RibbonPanelName);

            assembly = _AssemblyFolderName + _CmpAttribute.ResourceText("IDS_BTN_FACE_ASSEMBLYNAME");
            if (System.IO.File.Exists(assembly))
            {
                pushBtnData = CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_FACE_NAME"),
                    _CmpAttribute.ResourceText("IDS_BTN_FACE_TEXT"),
                    ResImage("IDI_BTN_FACE_S"),
                    ResImage("IDI_BTN_FACE_L"),
                    _CmpAttribute.ResourceText("IDS_BTN_FACE_TOOLTIP_S"),
                    _CmpAttribute.ResourceText("IDS_BTN_FACE_TOOLTIP_L"),
                    ResImage("IDI_BTN_FACE_L"),
                    assembly,
                    _CmpAttribute.ResourceText("IDS_BTN_FACE_CLASSNAME"),
                    "");

                itemDatas.Add(pushBtnData);

                if (contHelp != null)
                {
                    pushBtnData.SetContextualHelp(contHelp);
                }
            }

            assembly = _AssemblyFolderName + _CmpAttribute.ResourceText("IDS_BTN_OPENING_ASSEMBLYNAME");
            if (System.IO.File.Exists(assembly))
            {
                pushBtnData = CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_OPENING_NAME"),
                    _CmpAttribute.ResourceText("IDS_BTN_OPENING_TEXT"),
                    ResImage("IDI_BTN_OPENING_S"),
                    ResImage("IDI_BTN_OPENING_L"),
                    _CmpAttribute.ResourceText("IDS_BTN_OPENING_TOOLTIP_S"),
                    _CmpAttribute.ResourceText("IDS_BTN_OPENING_TOOLTIP_L"),
                    ResImage("IDI_BTN_OPENING_L"),
                    assembly,
                    _CmpAttribute.ResourceText("IDS_BTN_OPENING_CLASSNAME"),
                    "");

                itemDatas.Add(pushBtnData);

                if (contHelp != null)
                {
                    pushBtnData.SetContextualHelp(contHelp);
                }
            }

            int limit = 1;
            SetStackItems(ribbonPanel, itemDatas, limit);

            if (itemDatas.Count > 0)
            {
                itemDatas.Clear();

                ribbonPanel.AddSeparator();

                pushBtnData = CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_MATERIAL_NAME"),
                    _CmpAttribute.ResourceText("IDS_BTN_MATERIAL_TEXT"),
                    ResImage(""),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_BTN_MATERIAL_TOOLTIP_S"),
                    _CmpAttribute.ResourceText(""),
                    ResImage(""),
                    assembly,
                    _CmpAttribute.ResourceText("IDS_RBN_RIBBON_CLASSNAME"),
                    "");
                itemDatas.Add(pushBtnData);

                if (contHelp != null)
                {
                    pushBtnData.SetContextualHelp(contHelp);
                }

                pushBtnData = CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_UPPERLEVEL_NAME"),
                    _CmpAttribute.ResourceText("IDS_BTN_UPPERLEVEL_TEXT"),
                    ResImage(""),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_BTN_UPPERLEVEL_TOOLTIP_S"),
                    _CmpAttribute.ResourceText(""),
                    ResImage(""),
                    assembly,
                    _CmpAttribute.ResourceText("IDS_RBN_RIBBON_CLASSNAME"),
                    "");
                itemDatas.Add(pushBtnData);

                if (contHelp != null)
                {
                    pushBtnData.SetContextualHelp(contHelp);
                }

                pushBtnData = CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_OFFSET_NAME"),
                    _CmpAttribute.ResourceText("IDS_BTN_OFFSET_TEXT"),
                    ResImage(""),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_BTN_OFFSET_TOOLTIP_S"),
                    _CmpAttribute.ResourceText(""),
                    ResImage(""),
                    assembly,
                    _CmpAttribute.ResourceText("IDS_RBN_RIBBON_CLASSNAME"),
                    "");
                itemDatas.Add(pushBtnData);

                if (contHelp != null)
                {
                    pushBtnData.SetContextualHelp(contHelp);
                }

                limit = 3;
                SetStackItems(ribbonPanel, itemDatas, limit);

                itemDatas.Clear();

                cmbBoxData = CreateComboBoxData(_CmpAttribute.ResourceText("IDS_CMBBOX_MATERIAL_NAME"),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_CMBBOX_MATERIAL_TOOLTIP_S"),
                    "",
                    ResImage(""));
                itemDatas.Add(cmbBoxData);

                cmbBoxData = CreateComboBoxData(_CmpAttribute.ResourceText("IDS_CMBBOX_UPPERLEVEL_NAME"),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_CMBBOX_UPPERLEVEL_TOOLTIP_S"),
                    "",
                    ResImage(""));
                itemDatas.Add(cmbBoxData);

                txtBoxData = CreateTextBoxData(_CmpAttribute.ResourceText("IDS_TXTBOX_OFFSET_NAME"),
                    ResImage(""),
                    _CmpAttribute.ResourceText("IDS_TXTBOX_OFFSET_TOOLTIP_S"),
                    _CmpAttribute.ResourceText("IDS_TXTBOX_OFFSET_TOOLTIP_L"),
                    ResImage(""));
                itemDatas.Add(txtBoxData);

                limit = 3;
                SetStackItems(ribbonPanel, itemDatas, limit);

                foreach (Revit.UI.RibbonItem item in ribbonPanel.GetItems())
                {
                    if (item.Name == _CmpAttribute.ResourceText("IDS_BTN_MATERIAL_NAME") ||
                        item.Name == _CmpAttribute.ResourceText("IDS_BTN_UPPERLEVEL_NAME") ||
                        item.Name == _CmpAttribute.ResourceText("IDS_BTN_OFFSET_NAME"))
                    {
                        item.Enabled = false;
                    }

                    if (item.Name == _CmpAttribute.ResourceText("IDS_TXTBOX_OFFSET_NAME"))
                    {
                        Revit.UI.TextBox txtBox = item as Revit.UI.TextBox;
                        txtBox.EnterPressed += new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetOffsetValueEvent);

                        if (contHelp != null)
                        {
                            item.SetContextualHelp(contHelp);
                        }
                    }

                    if (item.Name == _CmpAttribute.ResourceText("IDS_CMBBOX_UPPERLEVEL_NAME") ||
                        item.Name == _CmpAttribute.ResourceText("IDS_CMBBOX_MATERIAL_NAME"))
                    {
                        if (contHelp != null)
                        {
                            item.SetContextualHelp(contHelp);
                        }
                    }
                }
            }
        }

        internal void SetLastTimeValues(RvtExtApp.Components.Elements cmpElements)
        {
            Revit.DB.TransactionGroup txGrp = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            txGrp.Start(_CmpAttribute.ResourceText("IDS_TXT_SETMATERIAL"));

            Collections.Generic.IList<Revit.DB.Material> materials = cmpElements.GetMaterials();

            string tabName = RibbonTabName;
            string pnlName = RibbonPanelName;

            AdWindows.RibbonCombo cmbBox = null;
            AdWindows.RibbonTextBox txtBox = null;

            AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
            AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

            foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
            {
                if (rbnTab.AutomationName == tabName)
                {
                    AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

                    foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
                    {
                        if (rbnPanel.Source.AutomationName == pnlName)
                        {
                            AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

                            if (item != null)
                            {
                                cmbBox = item as AdWindows.RibbonCombo;
                            }

                            item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_TXTBOX_OFFSET"), true);

                            if (item != null)
                            {
                                txtBox = item as AdWindows.RibbonTextBox;
                            }
                        }
                    }

                    break;
                }
            }

            if (cmbBox == null)
            {
                txGrp.Assimilate();
                return;
            }

            using (Revit.DB.Transaction tx = new Revit.DB.Transaction(cmpElements.RvtDBDoc, _CmpAttribute.ResourceText("IDS_TXT_SETMATERIAL")))
            {
                if (tx.Start() == Autodesk.Revit.DB.TransactionStatus.Started)
                {
                    try
                    {
                        cmbBox.Items.Clear();

                        foreach (Revit.DB.Material material in materials)
                        {
                            ComboBoxMemberData memberData = new ComboBoxMemberData(material.Id.ToString(),
                                material.Name);

                            cmbBox.Items.Add(memberData);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    tx.Commit();
                }
            }

            Revit.DB.ProjectInfo prjInfo = cmpElements.RvtDBDoc.ProjectInformation;

            Revit.DB.Parameter paramLastTimeValues = prjInfo.LookupParameter(_CmpAttribute.ResourceText("IDS_SHPARAM_DEF"));

            if (paramLastTimeValues == null)
            {
                if (cmbBox.Items.Count > 0)
                {
                    cmbBox.Current = cmbBox.Items[0];
                }

                txGrp.Commit();
                return;
            }

            string sValue = paramLastTimeValues.AsString();

            Collections.Generic.IList<string> valueSplit =
                ADSK.Ext.Fukashi.Utils.UtilValue.SplitString(sValue, ",");

            string materialId = "";
            if (valueSplit.Count > 0)
            {
                materialId = valueSplit[0];
            }

            string offset = "";
            if (valueSplit.Count > 1)
            {
                offset = valueSplit[1];
            }

            if (offset == "")
            {
                offset = "0";
            }

            foreach (object item in cmbBox.Items)
            {
                ComboBoxMemberData memberData = item as ComboBoxMemberData;

                if (memberData.Name == materialId)
                {
                    cmbBox.Current = memberData;
                    break;
                }
            }

            if (cmbBox.Current == null)
            {
                if (cmbBox.Items.Count > 0)
                {
                    cmbBox.Current = cmbBox.Items[0];
                }
            }

            txtBox.Value = offset;
            _OffsetValue = offset;

            txGrp.Commit();
        }

        internal void SetUpperLevels(RvtExtApp.Components.Elements cmpElements)
        {
            Revit.DB.TransactionGroup txGrp = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            txGrp.Start(_CmpAttribute.ResourceText("IDS_TXT_SETMATERIAL"));

            Revit.DB.View view = cmpElements.RvtDBDoc.ActiveView;

            Revit.DB.Level level = view.GenLevel;

            if (level == null)
            {
                txGrp.Assimilate();
                return;
            }

            Collections.Generic.IList<Revit.DB.Level> levels = cmpElements.GetUpperLevels(level);

            string tabName = RibbonTabName;
            string pnlName = RibbonPanelName;

            AdWindows.RibbonCombo cmbBox = null;

            AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
            AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

            foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
            {
                if (rbnTab.AutomationName == tabName)
                {
                    AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

                    foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
                    {
                        if (rbnPanel.Source.AutomationName == pnlName)
                        {
                            AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_UPPERLEVEL"), true);

                            if (item != null)
                            {
                                cmbBox = item as AdWindows.RibbonCombo;

                                break;
                            }
                        }
                    }

                    break;
                }
            }

            if (cmbBox == null)
            {
                txGrp.Assimilate();
                return;
            }

            using (Revit.DB.Transaction tx = new Revit.DB.Transaction(cmpElements.RvtDBDoc, _CmpAttribute.ResourceText("IDS_TXT_SETLEVEL")))
            {
                if (tx.Start() == Autodesk.Revit.DB.TransactionStatus.Started)
                {
                    try
                    {
                        cmbBox.Items.Clear();

                        foreach (Revit.DB.Level lvl in levels)
                        {
                            ComboBoxMemberData memberData = new ComboBoxMemberData(lvl.Id.ToString(),
                                lvl.Name);

                            cmbBox.Items.Add(memberData);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    tx.Commit();
                }
            }

            if (cmbBox.Current == null)
            {
                if (cmbBox.Items.Count > 0)
                {
                    cmbBox.Current = cmbBox.Items[0];
                }

                if (cmbBox.Items.Count > 1)
                {
                    cmbBox.Current = cmbBox.Items[1];
                }
            }

            txGrp.Commit();
        }

        internal void AddComboChangedEvent()
        {
            string tabName = RibbonTabName;
            string pnlName = RibbonPanelName;

            AdWindows.RibbonCombo cmbBox = null;

            AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
            AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

            foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
            {
                if (rbnTab.AutomationName == tabName)
                {
                    AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

                    foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
                    {
                        if (rbnPanel.Source.AutomationName == pnlName)
                        {
                            AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

                            if (item != null)
                            {
                                cmbBox = item as AdWindows.RibbonCombo;

                                break;
                            }
                        }
                    }

                    break;
                }
            }

            if (cmbBox != null)
            {
                cmbBox.CurrentChanged += new EventHandler<Autodesk.Windows.RibbonPropertyChangedEventArgs>(SetRibbonPropertyChangedEvent);
            }
        }

        internal void RemoveComboChangedEvent()
        {
            string tabName = RibbonTabName;
            string pnlName = RibbonPanelName;

            AdWindows.RibbonCombo cmbBox = null;

            AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
            AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

            foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
            {
                if (rbnTab.AutomationName == tabName)
                {
                    AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

                    foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
                    {
                        if (rbnPanel.Source.AutomationName == pnlName)
                        {
                            AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

                            if (item != null)
                            {
                                cmbBox = item as AdWindows.RibbonCombo;

                                break;
                            }
                        }
                    }

                    break;
                }
            }

            if (cmbBox != null)
            {
                cmbBox.CurrentChanged -= SetRibbonPropertyChangedEvent;
            }
        }

        internal void SetRibbonPropertyChangedEvent(object obj, AdWindows.RibbonPropertyChangedEventArgs args)
        {
            if (obj is AdWindows.RibbonCombo)
            {
                object oldValue = args.OldValue;
                object newValue = args.NewValue;

                ComboBoxMemberData memberDataOld = oldValue as ComboBoxMemberData;
                ComboBoxMemberData memberDataNew = newValue as ComboBoxMemberData;

                if (memberDataOld != null &&
                    memberDataNew != null)
                {
                }
            }
        }

        internal void SetOffsetValueEvent(object obj, Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs args)
        {
            Revit.UI.TextBox txtBox = obj as Revit.UI.TextBox;

            object objTxt = txtBox.Value;
            string val = objTxt.ToString();

            if ((!ADSK.Ext.Fukashi.Utils.UtilValue.IsInteger(val)) &&
                (!ADSK.Ext.Fukashi.Utils.UtilValue.IsNumber(val)))
            {
                txtBox.Value = _OffsetValue;
            }
            else
            {
                _OffsetValue = val;
            }
        }

        private IList<Revit.UI.RibbonPanel> GetRibbonPanel(string tabName)
        {
            return _rvtUICtrlApp.GetRibbonPanels(tabName);
        }

        private void CreateRibbonTab(string tabName)
        {
            _rvtUICtrlApp.CreateRibbonTab(tabName);
        }

        private Revit.UI.RibbonPanel CreateRibbonPanel(string tabName, string panelName)
        {
            return _rvtUICtrlApp.CreateRibbonPanel(tabName, panelName);
        }

        private PushButtonData CreatePushButtonData(string name, string text, BitmapImage smallImg,
            BitmapImage largeImg, string tooltipS, string tooltipL, BitmapImage tooltipImg, string assembly,
            string className, string availClassName)
        {
            PushButtonData pbd = new PushButtonData(name, text, assembly, className);
            if (smallImg != null && smallImg.UriSource != null)
            {
                pbd.Image = smallImg;
            }

            if (largeImg != null && largeImg.UriSource != null)
            {
                pbd.LargeImage = largeImg;
            }

            if (!string.IsNullOrEmpty(tooltipS))
            {
                pbd.ToolTip = tooltipS;
            }

            if (!string.IsNullOrEmpty(tooltipL))
            {
                pbd.LongDescription = tooltipL;
            }

            if (tooltipImg != null && tooltipImg.UriSource != null)
            {
                pbd.ToolTipImage = tooltipImg;
            }

            if (!string.IsNullOrEmpty(availClassName))
            {
                pbd.AvailabilityClassName = availClassName;
            }

            return pbd;
        }

        private ComboBoxData CreateComboBoxData(string name, BitmapImage img, string tooltip, string longDesc,
            BitmapImage tooltipImg)
        {
            ComboBoxData cbd = new ComboBoxData(name);
            if (!string.IsNullOrEmpty(tooltip))
            {
                cbd.ToolTip = tooltip;
            }

            return cbd;
        }

        private TextBoxData CreateTextBoxData(string name, BitmapImage img, string tooltip, string longDesc,
            BitmapImage tooltipImg)
        {
            TextBoxData tbd = new TextBoxData(name);
            if (!string.IsNullOrEmpty(tooltip))
            {
                tbd.ToolTip = tooltip;
            }

            return tbd;
        }

        private void SetStackItems(Revit.UI.RibbonPanel panel, IList<RibbonItemData> items, int limit)
        {
            if (items.Count == 0)
            {
                return;
            }

            if (items.Count == 1)
            {
                panel.AddItem(items[0]);
            }
            else if (items.Count == 2)
            {
                panel.AddStackedItems(items[0], items[1]);
            }
            else if (items.Count >= 3)
            {
                panel.AddStackedItems(items[0], items[1], items[2]);
            }
        }

        private BitmapImage ResImage(string path)
        {
            var location = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (path == "")
            {
                return new BitmapImage();
            }

            var bmp = new BitmapImage(new Uri($@"{location}\Res\{path}.png", UriKind.Absolute));
            if (!System.IO.File.Exists($@"{location}\Res\{path}.png"))
            {
                throw new Exception();
            }

            return bmp;
        }
    }
}
