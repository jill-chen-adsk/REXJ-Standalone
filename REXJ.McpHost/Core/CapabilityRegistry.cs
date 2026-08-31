using System;
using System.Collections.Generic;
using System.Text.Json;

namespace REXJ.McpHost.Core;

public interface ICapability
{
    string Name { get; }

    string Description { get; }

    bool SideEffects { get; }

    object Execute(JsonElement arguments, RevitApiExecutor executor);
}

public sealed class CapabilityRegistry
{
    private readonly Dictionary<string, ICapability> _capabilities = new(StringComparer.Ordinal);

    public void Register(ICapability capability)
    {
        _capabilities[capability.Name] = capability;
    }

    public IReadOnlyCollection<ICapability> All => _capabilities.Values;

    public ICapability Get(string name)
    {
        if (!_capabilities.TryGetValue(name, out ICapability? capability))
        {
            throw new KeyNotFoundException($"Unknown capability: {name}");
        }

        return capability;
    }
}
