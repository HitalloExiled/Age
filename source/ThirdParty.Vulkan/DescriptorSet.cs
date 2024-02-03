namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorSet : DisposableNativeHandle
{
    private readonly DescriptorPool descriptorPool;

    internal DescriptorSet(VkDescriptorSet handle, DescriptorPool descriptorPool) : base(handle) =>
        this.descriptorPool = descriptorPool;

    protected override void OnDispose()
    {
        nint handle = this;

        VulkanException.Check(PInvoke.vkFreeDescriptorSets(this.descriptorPool.Device, this.descriptorPool, 1, &handle));
    }
}
