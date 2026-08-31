using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace REXJ.McpHost.Core;

/// <summary>
/// Lightweight localhost HTTP server exposing MCP-style tools/list and tools/call.
/// </summary>
public sealed class McpHttpServer : IDisposable
{
    private const int DefaultPort = 28733;

    private readonly HttpListener _listener;
    private readonly CapabilityRegistry _registry;
    private readonly RevitApiExecutor _executor;
    private readonly CancellationTokenSource _cts = new();
    private Task? _listenTask;

    public McpHttpServer(CapabilityRegistry registry, RevitApiExecutor executor, int port = DefaultPort)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));

        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        _listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public int Port => DefaultPort;

    public void Start()
    {
        if (_listenTask != null)
        {
            return;
        }

        _listener.Start();
        _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
    }

    public void Stop()
    {
        _cts.Cancel();
        if (_listener.IsListening)
        {
            _listener.Stop();
        }
    }

    public void Dispose()
    {
        Stop();
        _listener.Close();
        _cts.Dispose();
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await _listener.GetContextAsync().ConfigureAwait(false);
            }
            catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            _ = Task.Run(() => HandleRequestAsync(context), cancellationToken);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            if (context.Request.HttpMethod == "GET" && context.Request.Url?.AbsolutePath == "/health")
            {
                await WriteJsonAsync(context.Response, 200, new
                {
                    status = "ok",
                    revitReady = RevitContext.IsAvailable,
                    port = Port,
                }).ConfigureAwait(false);
                return;
            }

            if (context.Request.HttpMethod != "POST" || context.Request.Url?.AbsolutePath != "/mcp")
            {
                await WriteJsonAsync(context.Response, 404, new { error = "Not found. Use POST /mcp or GET /health." })
                    .ConfigureAwait(false);
                return;
            }

            using StreamReader reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            string body = await reader.ReadToEndAsync().ConfigureAwait(false);

            JsonDocument document = JsonDocument.Parse(body);
            JsonElement root = document.RootElement;

            string? method = root.TryGetProperty("method", out JsonElement methodProp)
                && methodProp.ValueKind == JsonValueKind.String
                ? methodProp.GetString()
                : null;

            JsonElement id = root.TryGetProperty("id", out JsonElement idProp) ? idProp : default;
            JsonElement parameters = root.TryGetProperty("params", out JsonElement paramsProp)
                ? paramsProp
                : default;

            object result = method switch
            {
                "tools/list" => BuildToolsList(),
                "tools/call" => HandleToolsCall(parameters),
                "initialize" => new
                {
                    protocolVersion = "2024-11-05",
                    serverInfo = new { name = "REXJ.McpHost", version = "0.1.0-spike" },
                    capabilities = new { tools = new { } },
                },
                _ => throw new InvalidOperationException($"Unknown method: {method}"),
            };

            var envelope = new Dictionary<string, object?>
            {
                ["jsonrpc"] = "2.0",
                ["result"] = result,
            };

            if (id.ValueKind != JsonValueKind.Undefined)
            {
                envelope["id"] = JsonSerializer.Deserialize<object>(id.GetRawText());
            }

            await WriteJsonAsync(context.Response, 200, envelope).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await WriteJsonAsync(context.Response, 500, new
            {
                jsonrpc = "2.0",
                error = new { code = -32000, message = ex.Message },
            }).ConfigureAwait(false);
        }
    }

    private object BuildToolsList()
    {
        var tools = new List<object>();
        foreach (ICapability capability in _registry.All)
        {
            tools.Add(new
            {
                name = capability.Name,
                description = capability.Description,
                sideEffects = capability.SideEffects,
            });
        }

        return new { tools };
    }

    private object HandleToolsCall(JsonElement parameters)
    {
        if (!RevitContext.IsAvailable)
        {
            throw new InvalidOperationException("Revit is not ready. Open a project in Revit first.");
        }

        if (!parameters.TryGetProperty("name", out JsonElement nameProp)
            || nameProp.ValueKind != JsonValueKind.String)
        {
            throw new ArgumentException("params.name is required.");
        }

        string toolName = nameProp.GetString() ?? string.Empty;
        JsonElement arguments = parameters.TryGetProperty("arguments", out JsonElement argsProp)
            ? argsProp
            : default;

        ICapability capability = _registry.Get(toolName);
        object content = capability.Execute(
            arguments.ValueKind == JsonValueKind.Undefined ? default : arguments,
            _executor);

        return new
        {
            content = new[]
            {
                new
                {
                    type = "text",
                    text = JsonSerializer.Serialize(content, JsonOptions),
                },
            },
            isError = false,
        };
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private static async Task WriteJsonAsync(HttpListenerResponse response, int statusCode, object payload)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json; charset=utf-8";
        response.Headers.Add("Access-Control-Allow-Origin", "*");

        byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, JsonOptions));
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes).ConfigureAwait(false);
        response.OutputStream.Close();
    }
}
