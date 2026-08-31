using System;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using REXJ.McpHost.Capabilities;
using REXJ.McpHost.Core;

namespace REXJ.McpHost.App;

public sealed class McpHostApp : IExternalApplication
{
    private static McpHttpServer? _server;
    private static RevitApiExecutor? _executor;

    public Result OnStartup(UIControlledApplication application)
    {
        application.ControlledApplication.ApplicationInitialized += OnApplicationInitialized;
        return Result.Succeeded;
    }

    private static void OnApplicationInitialized(object? sender, ApplicationInitializedEventArgs e)
    {
        try
        {
            if (sender is not Autodesk.Revit.ApplicationServices.Application app)
            {
                throw new InvalidOperationException("Revit Application not available after initialization.");
            }

            var uiApp = new UIApplication(app);
            RevitContext.SetUIApplication(uiApp);

            _executor = new RevitApiExecutor();

            var registry = new CapabilityRegistry();
            registry.Register(new ElementsByLevelCapability());
            registry.Register(new JoinOrderInspectCapability());

            _server = new McpHttpServer(registry, _executor);
            _server.Start();
        }
        catch (Exception ex)
        {
            TaskDialog.Show("REXJ MCP Host", $"Failed to start MCP host: {ex.Message}");
        }
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        application.ControlledApplication.ApplicationInitialized -= OnApplicationInitialized;
        _server?.Dispose();
        _server = null;
        _executor = null;
        return Result.Succeeded;
    }
}
