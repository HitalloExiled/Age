using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public partial class ResourceCache<TKey, TValue>
where TKey   : notnull, Resource
where TValue : notnull, Resource
{
    internal readonly struct Entry(TKey key, ResourceCache<TKey, TValue> resourceCache)
    {
        public readonly ResourceCache<TKey, TValue> Cache = resourceCache;

        public readonly TKey Key = key;

        public readonly void Dispose()
        {
            VulkanRenderer.Singleton.DeferredDispose(this.Cache.entries[this.Key]);
            this.Cache.entries.Remove(this.Key);
        }
    }
}
