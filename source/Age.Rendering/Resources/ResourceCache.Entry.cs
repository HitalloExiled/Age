using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public partial class ResourceCache<TKey, TValue>
where TKey   : notnull, Resource
where TValue : notnull, Resource
{
    internal readonly struct Entry(int hash, ResourceCache<TKey, TValue> resourceCache)
    {
        public readonly ResourceCache<TKey, TValue> Cache = resourceCache;

        public readonly int Hash = hash;

        public readonly void Dispose()
        {
            VulkanRenderer.Singleton.DeferredDispose(this.Cache.entries[this.Hash]);
            this.Cache.entries.Remove(this.Hash);
        }
    }
}
