using System;
using Autodesk.Revit.UI;

namespace REXJ.McpHost.Core;

/// <summary>
/// Holds the active Revit UIApplication for MCP capability handlers.
/// </summary>
public static class RevitContext
{
    private static UIApplication? _uiApp;

    public static void SetUIApplication(UIApplication uiApp)
    {
        _uiApp = uiApp;
    }

    public static UIApplication GetUIApplication()
    {
        if (_uiApp == null)
        {
            throw new InvalidOperationException("Revit is not ready. Open a project in Revit first.");
        }

        return _uiApp;
    }

    public static bool IsAvailable => _uiApp != null;
}
