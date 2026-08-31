using System;
using System.Threading;
using Autodesk.Revit.UI;

namespace REXJ.McpHost.Core;

/// <summary>
/// Marshals work onto Revit's main thread via ExternalEvent.
/// </summary>
public sealed class RevitApiExecutor : IExternalEventHandler
{
    private readonly ExternalEvent _externalEvent;
    private Func<UIApplication, object>? _work;
    private object? _result;
    private Exception? _error;
    private volatile bool _pending;

    public RevitApiExecutor()
    {
        _externalEvent = ExternalEvent.Create(this);
    }

    public void Execute(UIApplication app)
    {
        try
        {
            if (_work == null)
            {
                throw new InvalidOperationException("No work scheduled for Revit API executor.");
            }

            _result = _work(app);
        }
        catch (Exception ex)
        {
            _error = ex;
        }
        finally
        {
            _pending = false;
        }
    }

    public string GetName() => "REXJ MCP Host API Executor";

    public object Invoke(Func<UIApplication, object> work, int timeoutMs = 120000)
    {
        if (work == null)
        {
            throw new ArgumentNullException(nameof(work));
        }

        _work = work;
        _result = null;
        _error = null;
        _pending = true;

        var raiseResult = _externalEvent.Raise();
        if (raiseResult != ExternalEventRequest.Accepted)
        {
            throw new InvalidOperationException($"Revit rejected ExternalEvent raise: {raiseResult}");
        }

        var deadline = Environment.TickCount64 + timeoutMs;
        while (_pending)
        {
            if (Environment.TickCount64 > deadline)
            {
                throw new TimeoutException("Timed out waiting for Revit API executor.");
            }

            Thread.Sleep(15);
        }

        if (_error != null)
        {
            throw _error;
        }

        return _result ?? new object();
    }
}
