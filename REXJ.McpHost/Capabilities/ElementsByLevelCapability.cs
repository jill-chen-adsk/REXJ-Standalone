using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace REXJ.McpHost.Capabilities;

/// <summary>
/// MCP wrapper for rexj.rac.query.elements-by-level (pilot #1).
/// Read-only query — no model changes.
/// </summary>
public sealed class ElementsByLevelCapability : Core.ICapability
{
    public string Name => "rexj.rac.query.elements-by-level";

    public string Description =>
        "Query Revit element IDs filtered by level name(s) and optional category names.";

    public bool SideEffects => false;

    public object Execute(JsonElement arguments, Core.RevitApiExecutor executor)
    {
        var levelNames = ReadStringArray(arguments, "levelNames");
        var levelIds = ReadLongArray(arguments, "levelIds");
        var categoryNames = ReadStringArray(arguments, "categoryNames");
        var maxResults = ReadInt(arguments, "maxResults", 5000);

        return executor.Invoke(uiApp =>
        {
            UIDocument uidoc = uiApp.ActiveUIDocument
                ?? throw new InvalidOperationException("No active Revit document.");

            Document doc = uidoc.Document;

            IList<ElementId> targetLevelIds = ResolveLevelIds(doc, levelNames, levelIds);
            if (targetLevelIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "Provide at least one level via levelNames or levelIds.");
            }

            ElementFilter levelFilter = BuildLevelFilter(targetLevelIds);
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(levelFilter);

            if (categoryNames.Count > 0)
            {
                IList<ElementFilter> categoryFilters = new List<ElementFilter>();
                foreach (string categoryName in categoryNames)
                {
                    Category? category = doc.Settings.Categories
                        .Cast<Category>()
                        .FirstOrDefault(c =>
                            string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));

                    if (category != null)
                    {
                        categoryFilters.Add(new ElementCategoryFilter(category.Id));
                    }
                }

                if (categoryFilters.Count > 0)
                {
                    ElementFilter categoryOr = new LogicalOrFilter(categoryFilters);
                    collector = collector.WherePasses(categoryOr);
                }
            }

            IList<ElementId> elementIds = collector
                .ToElementIds()
                .Take(maxResults)
                .ToList();

            var groupedByCategory = new Dictionary<string, List<long>>();
            foreach (ElementId id in elementIds)
            {
                Element? element = doc.GetElement(id);
                string categoryLabel = element?.Category?.Name ?? "(none)";
                if (!groupedByCategory.TryGetValue(categoryLabel, out List<long>? ids))
                {
                    ids = new List<long>();
                    groupedByCategory[categoryLabel] = ids;
                }

                ids.Add(id.Value);
            }

            return new
            {
                count = elementIds.Count,
                levelIds = targetLevelIds.Select(id => id.Value).ToList(),
                elementIds = elementIds.Select(id => id.Value).ToList(),
                groupedByCategory,
                truncated = elementIds.Count >= maxResults,
            };
        });
    }

    private static IList<ElementId> ResolveLevelIds(
        Document doc,
        IList<string> levelNames,
        IList<long> levelIds)
    {
        var resolved = new List<ElementId>();

        foreach (long id in levelIds)
        {
            resolved.Add(new ElementId(id));
        }

        if (levelNames.Count == 0)
        {
            return resolved;
        }

        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToList();

        foreach (string levelName in levelNames)
        {
            Level? match = levels.FirstOrDefault(l =>
                string.Equals(l.Name, levelName, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                resolved.Add(match.Id);
            }
        }

        return resolved.Distinct(new ElementIdComparer()).ToList();
    }

    private static ElementFilter BuildLevelFilter(IList<ElementId> levelIds)
    {
        if (levelIds.Count == 1)
        {
            return new ElementLevelFilter(levelIds[0]);
        }

        IList<ElementFilter> filters = levelIds
            .Select(id => (ElementFilter)new ElementLevelFilter(id))
            .ToList();

        return new LogicalOrFilter(filters);
    }

    private static IList<string> ReadStringArray(JsonElement arguments, string propertyName)
    {
        if (!arguments.TryGetProperty(propertyName, out JsonElement value)
            || value.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<string>();
        }

        return value.EnumerateArray()
            .Where(item => item.ValueKind == JsonValueKind.String)
            .Select(item => item.GetString() ?? string.Empty)
            .Where(s => s.Length > 0)
            .ToList();
    }

    private static IList<long> ReadLongArray(JsonElement arguments, string propertyName)
    {
        if (!arguments.TryGetProperty(propertyName, out JsonElement value)
            || value.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<long>();
        }

        var ids = new List<long>();
        foreach (JsonElement item in value.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Number && item.TryGetInt64(out long id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    private static int ReadInt(JsonElement arguments, string propertyName, int defaultValue)
    {
        if (arguments.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt32(out int parsed))
        {
            return parsed;
        }

        return defaultValue;
    }

    private sealed class ElementIdComparer : IEqualityComparer<ElementId>
    {
        public bool Equals(ElementId? x, ElementId? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Value == y.Value;
        }

        public int GetHashCode(ElementId obj) => obj.Value.GetHashCode();
    }
}
