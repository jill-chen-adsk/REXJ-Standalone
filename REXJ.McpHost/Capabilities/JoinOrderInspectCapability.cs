using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace REXJ.McpHost.Capabilities;

/// <summary>
/// MCP wrapper for rexj.rac.diagnose.join-order (pilot #2).
/// Extracted from JoinOrderInspector analysis logic — read-only by default.
/// </summary>
public sealed class JoinOrderInspectCapability : Core.ICapability
{
    public string Name => "rexj.rac.diagnose.join-order";

    public string Description =>
        "Inspect join order for an element: which joined elements cut it vs are cut by it.";

    public bool SideEffects => false;

    public object Execute(JsonElement arguments, Core.RevitApiExecutor executor)
    {
        if (!arguments.TryGetProperty("elementId", out JsonElement elementIdProp)
            || !elementIdProp.TryGetInt64(out long elementIdValue))
        {
            throw new ArgumentException("elementId (integer) is required.");
        }

        bool applyViewOverrides = ReadBool(arguments, "applyViewOverrides", false);

        return executor.Invoke(uiApp =>
        {
            UIDocument uidoc = uiApp.ActiveUIDocument
                ?? throw new InvalidOperationException("No active Revit document.");

            Document doc = uidoc.Document;
            ElementId elementId = new ElementId(elementIdValue);
            Element selected = doc.GetElement(elementId)
                ?? throw new InvalidOperationException($"Element not found: {elementIdValue}");

            ICollection<ElementId> joinElemIds =
                JoinGeometryUtils.GetJoinedElements(doc, selected);

            var cuttingElements = new List<Element>();
            var cutByElements = new List<Element>();

            foreach (ElementId joinId in joinElemIds)
            {
                Element joined = doc.GetElement(joinId)
                    ?? throw new InvalidOperationException($"Joined element not found: {joinId.Value}");

                if (JoinGeometryUtils.IsCuttingElementInJoin(doc, selected, joined))
                {
                    cutByElements.Add(joined);
                }
                else
                {
                    cuttingElements.Add(joined);
                }
            }

            if (applyViewOverrides)
            {
                using Transaction transaction = new Transaction(doc, "REXJ MCP Join Order Overrides");
                transaction.Start();
                ApplyColorCoding(doc, selected, cuttingElements, cutByElements);
                transaction.Commit();
            }

            return new
            {
                selectedElementId = selected.Id.Value,
                selectedCategory = selected.Category?.Name,
                selectedName = selected.Name,
                joinCount = joinElemIds.Count,
                cuttingElements = cuttingElements.Select(ToElementSummary).ToList(),
                cutByElements = cutByElements.Select(ToElementSummary).ToList(),
                viewOverridesApplied = applyViewOverrides,
            };
        });
    }

    private static object ToElementSummary(Element element)
    {
        return new
        {
            elementId = element.Id.Value,
            category = element.Category?.Name,
            name = element.Name,
        };
    }

    private static void ApplyColorCoding(
        Document doc,
        Element selectedElem,
        IList<Element> cuttingElements,
        IList<Element> cutByElements)
    {
        View? activeView = doc.ActiveView;
        if (activeView == null)
        {
            return;
        }

        ResetViewOverrides(doc, activeView);

        FillPatternElement? fillPattern = new FilteredElementCollector(doc)
            .OfClass(typeof(FillPatternElement))
            .Cast<FillPatternElement>()
            .FirstOrDefault(fp =>
            {
                FillPattern? pattern = fp.GetFillPattern();
                return pattern != null && pattern.IsSolidFill;
            });

        if (fillPattern == null)
        {
            return;
        }

        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
        ogs.SetSurfaceForegroundPatternId(fillPattern.Id);
        ogs.SetCutForegroundPatternId(fillPattern.Id);

        ogs.SetSurfaceForegroundPatternColor(new Color(255, 255, 101));
        ogs.SetCutForegroundPatternColor(new Color(221, 221, 0));
        activeView.SetElementOverrides(selectedElem.Id, ogs);

        ogs.SetSurfaceForegroundPatternColor(new Color(255, 153, 153));
        ogs.SetCutForegroundPatternColor(new Color(206, 89, 89));
        foreach (Element element in cuttingElements)
        {
            activeView.SetElementOverrides(element.Id, ogs);
        }

        ogs.SetSurfaceForegroundPatternColor(new Color(101, 178, 255));
        ogs.SetCutForegroundPatternColor(new Color(48, 109, 255));
        foreach (Element element in cutByElements)
        {
            activeView.SetElementOverrides(element.Id, ogs);
        }
    }

    private static void ResetViewOverrides(Document doc, View view)
    {
        IList<Element> viewElems = new FilteredElementCollector(doc, view.Id).ToElements();
        OverrideGraphicSettings clear = new OverrideGraphicSettings();
        foreach (Element element in viewElems)
        {
            view.SetElementOverrides(element.Id, clear);
        }
    }

    private static bool ReadBool(JsonElement arguments, string propertyName, bool defaultValue)
    {
        if (arguments.TryGetProperty(propertyName, out JsonElement value))
        {
            if (value.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (value.ValueKind == JsonValueKind.False)
            {
                return false;
            }
        }

        return defaultValue;
    }
}
