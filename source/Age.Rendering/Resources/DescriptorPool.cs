using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public record DescriptorPool : Disposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL = 64;

    private static readonly Dictionary<VkDescriptorType, List<DescriptorPool>> typeEntries = [];

    public required VkDescriptorType DescriptorType { get; init; }
    public required VkDescriptorPool Value          { get; init; }

    public uint Usage { get; private set; }

    private static void RemoveFromDescriptorPool(DescriptorPool descriptorPool)
    {
        descriptorPool.Usage--;

        if (descriptorPool.Usage == 0)
        {
            descriptorPool.Value.Dispose();

            var entries = typeEntries[descriptorPool.DescriptorType];

            entries.Remove(descriptorPool);

            if (entries.Count == 0)
            {
                typeEntries.Remove(descriptorPool.DescriptorType);
            }
        }
    }

    public unsafe static DescriptorPool CreateDescriptorPool(VkDevice device, VkDescriptorType descriptorType)
    {
        if (typeEntries.TryGetValue(descriptorType, out var entries))
        {
            if (entries.FirstOrDefault(x => x.Usage < MAX_DESCRIPTORS_PER_POOL) is DescriptorPool descriptorPool)
            {
                descriptorPool.Usage++;

                return descriptorPool;
            }
        }
        else
        {
            typeEntries[descriptorType] = entries = [];
        }

        var sizes = new List<VkDescriptorPoolSize>();

        if (descriptorType.HasFlag(VkDescriptorType.UniformBuffer))
        {
            sizes.Add(new() { Type = VkDescriptorType.UniformBuffer, DescriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        if (descriptorType.HasFlag(VkDescriptorType.CombinedImageSampler))
        {
            sizes.Add(new() { Type = VkDescriptorType.CombinedImageSampler, DescriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        fixed (VkDescriptorPoolSize* pPoolSizes = sizes.ToArray())
        {
            var descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                Flags         = VkDescriptorPoolCreateFlags.FreeDescriptorSet,
                MaxSets       = MAX_DESCRIPTORS_PER_POOL,
                PoolSizeCount = (uint)sizes.Count,
                PPoolSizes    = pPoolSizes,
            };

            var descriptorPool = device.CreateDescriptorPool(descriptorPoolCreateInfo);

            var descriptorPoolValue = new DescriptorPool
            {
                DescriptorType = descriptorType,
                Value          = descriptorPool,
                Usage          = 1,
            };

            entries.Add(descriptorPoolValue);

            return descriptorPoolValue;
        }
    }

    protected override void OnDispose() =>
        this.Value.Dispose();

    public void FreeDescriptorSets(VkDescriptorSet[] descriptorSets)
    {
        this.Value.FreeDescriptorSets(descriptorSets);

        RemoveFromDescriptorPool(this);
    }
}
