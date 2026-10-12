using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ADSK.JExtRAC.JoinOrderInspector.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfGrid = System.Windows.Controls.Grid;

namespace ADSK.JExtRAC.JoinOrderInspector.UI
{
    public partial class JoinOrderInspectWindow : Window, IWeaveChromeWindow
    {
        private readonly UIDocument _uidoc;
        private readonly Document _doc;
        private readonly ResourceManager _resourceMan;

        public JoinOrderInspectWindow(UIApplication uiapp)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _uidoc = uiapp.ActiveUIDocument;
            _doc = _uidoc.Document;
            _resourceMan = new ResourceManager(
                "ADSK.JExtRAC.JoinOrderInspector.Resources.Text",
                Assembly.GetExecutingAssembly());

            SetLocalizedText();
            WeaveTheme.Apply(this, this, Title, Close);

            AnalyzeAndDisplayResults();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public WpfGrid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private string Res(string key)
        {
            return _resourceMan.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }

        private void SetLocalizedText()
        {
            string title = Res("IDS_WINDOW_TITLE");
            Title = title;
            WeaveWindowChrome.SetTitle(this, this, title);
            legendSelected.Text = Res("IDS_LEGEND_SELECTED");
            legendCutting.Text = Res("IDS_LEGEND_CUTTING");
            legendCutBy.Text = Res("IDS_LEGEND_CUT_BY");
            btnReanalyze.Content = Res("IDS_BTN_REANALYZE");
        }

        private void AnalyzeAndDisplayResults()
        {
            elementListBox.Items.Clear();

            try
            {
                Reference selectElemRef = _uidoc.Selection.PickObject(
                    ObjectType.Element, Res("IDS_PICK_PROMPT"));
                Element selectElem = _doc.GetElement(selectElemRef);
                ICollection<ElementId> joinElemIds =
                    JoinGeometryUtils.GetJoinedElements(_doc, selectElem);

                List<Element> cuttingElements = new List<Element>();
                List<Element> cutByElements = new List<Element>();

                foreach (ElementId id in joinElemIds)
                {
                    Element elem = _doc.GetElement(id);
                    if (JoinGeometryUtils.IsCuttingElementInJoin(_doc, selectElem, elem))
                        cutByElements.Add(elem);
                    else
                        cuttingElements.Add(elem);
                }

                using (Transaction t = new Transaction(_doc, "Join Order Inspector - VG Override"))
                {
                    t.Start();
                    ResetAllElementOverrides();
                    ApplyColorCoding(selectElem, cuttingElements, cutByElements);
                    t.Commit();
                }

                DisplayResults(selectElem, cuttingElements, cutByElements);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                elementListBox.Items.Add(new ListItem("", Res("IDS_NO_SELECTION")));
            }
            catch (Exception ex)
            {
                elementListBox.Items.Add(new ListItem("",
                    Res("IDS_ERROR_PREFIX") + ex.Message));
            }
        }

        private void ResetAllElementOverrides()
        {
            IList<Element> viewElems =
                new FilteredElementCollector(_doc, _doc.ActiveView.Id).ToElements();
            foreach (var e in viewElems)
            {
                _doc.ActiveView.SetElementOverrides(e.Id, new OverrideGraphicSettings());
            }
        }

        private FillPatternElement FindSolidFillPattern()
        {
            var allPatterns = new FilteredElementCollector(_doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>();

            foreach (var fp in allPatterns)
            {
                FillPattern pattern = fp.GetFillPattern();
                if (pattern != null && pattern.IsSolidFill)
                    return fp;
            }

            return allPatterns.FirstOrDefault();
        }

        private void ApplyColorCoding(Element selectedElem,
            List<Element> cuttingElements, List<Element> cutByElements)
        {
            FillPatternElement fillPattern = FindSolidFillPattern();
            if (fillPattern == null) return;

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceForegroundPatternId(fillPattern.Id);
            ogs.SetCutForegroundPatternId(fillPattern.Id);

            SetFillColor(ogs, "Weave.Color.Legend.Selected");
            _doc.ActiveView.SetElementOverrides(selectedElem.Id, ogs);

            SetFillColor(ogs, "Weave.Color.Legend.Cutting");
            foreach (Element elem in cuttingElements)
                _doc.ActiveView.SetElementOverrides(elem.Id, ogs);

            SetFillColor(ogs, "Weave.Color.Legend.CutBy");
            foreach (Element elem in cutByElements)
                _doc.ActiveView.SetElementOverrides(elem.Id, ogs);
        }

        private void SetFillColor(OverrideGraphicSettings ogs, string legendColorKey)
        {
            var legend = (System.Windows.Media.Color)FindResource(legendColorKey);
            var color = new Color(legend.R, legend.G, legend.B);
            ogs.SetSurfaceForegroundPatternColor(color);
            ogs.SetCutForegroundPatternColor(color);
        }

        private void DisplayResults(Element selectElem,
            List<Element> cuttingElements, List<Element> cutByElements)
        {
            AddCategoryToOutput(Res("IDS_CATEGORY_CUTTING"), cuttingElements);
            AddElementToOutput(Res("IDS_CATEGORY_SELECTED"), selectElem);
            AddCategoryToOutput(Res("IDS_CATEGORY_CUT_BY"), cutByElements);
        }

        private void AddCategoryToOutput(string category, List<Element> elements)
        {
            elementListBox.Items.Add(
                new ListItem("", $"--------------- {category} ---------------"));
            if (elements.Count > 0)
            {
                foreach (var elem in elements)
                    AddElementToOutput(null, elem);
            }
            else
            {
                elementListBox.Items.Add(new ListItem("", Res("IDS_NONE")));
            }
        }

        private void AddElementToOutput(string category, Element elem)
        {
            if (category != null)
            {
                elementListBox.Items.Add(
                    new ListItem("", $"--------------- {category} ---------------"));
            }

            string elementInfo = $"{elem.Category?.Name} - {elem.Name}";
            elementListBox.Items.Add(new ListItem(elem.Id.ToString(), elementInfo));
        }

        private void Element_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string content = button.Content?.ToString();
                if (!string.IsNullOrEmpty(content) && long.TryParse(content, out long idValue))
                {
                    ElementId elementId = new ElementId(idValue);
                    _uidoc.Selection.SetElementIds(new List<ElementId> { elementId });
                    _uidoc.ShowElements(elementId);
                }
            }
        }

        private void BtnReanalyze_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeAndDisplayResults();
        }
    }

    public class ListItem
    {
        public string Id { get; }
        public string Info { get; }

        public ListItem(string id, string info)
        {
            Id = id;
            Info = info;
        }
    }
}
