namespace ThirdParty.Vulkan;

public unsafe partial class VkDescriptorSet : DisposableManagedHandle<VkDescriptorSet>
{
    private readonly VkDescriptorPool descriptorPool;

    internal VkDescriptorSet(VkHandle<VkDescriptorSet> handle, VkDescriptorPool descriptorPool) : base(handle) =>
        this.descriptorPool = descriptorPool;

    protected override void OnDispose()
    {
        VkHandle<VkDescriptorSet> handle = this;

        VkException.Check(PInvoke.vkFreeDescriptorSets(this.descriptorPool.Device, this.descriptorPool, 1, &handle));
    }
}
