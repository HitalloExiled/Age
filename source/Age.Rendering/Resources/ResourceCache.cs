using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Core.Extensions;

namespace Age.Rendering.Resources;

public partial class ResourceCache<TKey, TValue>
where TKey   : notnull, Resource
where TValue : notnull, Resource
{
    private readonly Dictionary<int, TValue> entries = [];

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? resource)
    {
        var hash = key.GetHashCode();

        ref var entry = ref this.entries.GetValueRefOrNullRef(hash);

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
        var hash = key.GetHashCode();

        this.entries[hash] = resource;

        var entry = new Entry(hash, this);

        key.Dependencies.Add(Unsafe.As<Entry, ResourceCache.Entry>(ref entry));
    }
}

public class ResourceCache : ResourceCache<Resource, Resource>;
