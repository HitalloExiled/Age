using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Core.Extensions;

namespace Age.Rendering.Resources;

public partial class ResourceCache<TKey, TValue>
where TKey   : notnull, Resource
where TValue : notnull, Resource
{
    private readonly Dictionary<TKey, TValue> entries = [];

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? resource)
    {
        ref var entry = ref this.entries.GetValueRefOrNullRef(key);

        if (!Unsafe.IsNullRef(ref entry))
        {
            resource = entry;
            return true;
        }

        resource = null;
        return false;
    }

    public void Set(TKey key, TValue resource)
    {
        this.entries[key] = resource;

        var entry = new Entry(key, this);

        key.Dependencies.Add(Unsafe.As<Entry, ResourceCache.Entry>(ref entry));
    }
}

public sealed class ResourceCache : ResourceCache<Resource, Resource>;
