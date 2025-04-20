using Age.Core;
using Age.Core.Extensions;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public sealed class DescriptorPool : Disposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL = 64;

    private static readonly Dictionary<DescriptorPoolKey, List<DescriptorPool>> typeEntries = [];

    public required DescriptorPoolKey Key   { get; init; }
    public required VkDescriptorPool  Value { get; init; }

    public uint Usage { get; private set; }

    private static List<VkDescriptorPoolSize> GetPoolSize(in DescriptorPoolKey key)
    {
        Span<VkDescriptorType> types =
        [
            VkDescriptorType.UniformBuffer,
            VkDescriptorType.CombinedImageSampler,
        ];

        var poolSizes = new List<VkDescriptorPoolSize>();

        for (var i = 0; i < types.Length; i++)
        {
            var count = key[types[i]];

            if (count > 0)
            {
                poolSizes.Add(new() { Type = types[i], DescriptorCount = count * MAX_DESCRIPTORS_PER_POOL });
            }
        }

        return poolSizes;
    }

    private static void RemoveFromDescriptorPool(DescriptorPool descriptorPool)
    {
        descriptorPool.Usage--;

        if (descriptorPool.Usage == 0)
        {
            descriptorPool.Value.Dispose();

            var entries = typeEntries[descriptorPool.Key];

            entries.Remove(descriptorPool);

            if (entries.Count == 0)
            {
                typeEntries.Remove(descriptorPool.Key);
            }
        }
    }

    public unsafe static DescriptorPool CreateDescriptorPool(DescriptorPoolKey key)
    {
        if (typeEntries.TryGetValue(key, out var entries))
        {
            if (entries.FirstOrDefault(x => x.Usage < MAX_DESCRIPTORS_PER_POOL) is DescriptorPool descriptorPool)
            {
                descriptorPool.Usage++;

                return descriptorPool;
            }
        }
        else
        {
            typeEntries[key] = entries = [];
        }

        var poolSizes = GetPoolSize(key);

        fixed (VkDescriptorPoolSize* pPoolSizes = poolSizes.AsSpan())
        {
            var descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                Flags         = VkDescriptorPoolCreateFlags.FreeDescriptorSet,
                MaxSets       = MAX_DESCRIPTORS_PER_POOL,
                PoolSizeCount = (uint)poolSizes.Count,
                PPoolSizes    = pPoolSizes,
            };

            var descriptorPool = VulkanRenderer.Singleton.Context.Device.CreateDescriptorPool(descriptorPoolCreateInfo);

            var descriptorPoolValue = new DescriptorPool
            {
                Key   = key,
                Value = descriptorPool,
                Usage = 1,
            };

            entries.Add(descriptorPoolValue);

            return descriptorPoolValue;
        }
    }

    public static void Clear()
    {
        foreach (var pools in typeEntries.Values)
        {
            foreach (var descriptorPool in pools)
            {
                descriptorPool.Dispose();
            }

            pools.Clear();
        }

        typeEntries.Clear();
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Value.Dispose();
        }
    }

    public void FreeDescriptorSets(scoped ReadOnlySpan<VkDescriptorSet> descriptorSets)
    {
        foreach (var descriptorSet in descriptorSets)
        {
            descriptorSet.Dispose();
        }

        RemoveFromDescriptorPool(this);
    }
}
