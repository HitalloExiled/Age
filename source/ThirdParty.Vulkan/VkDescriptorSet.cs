
namespace ThirdParty.Vulkan;

public unsafe partial class VkDescriptorSet : DisposableManagedHandle<VkDescriptorSet>
{
    private readonly VkDescriptorPool descriptorPool;

    internal VkDescriptorSet(VkHandle<VkDescriptorSet> handle, VkDescriptorPool descriptorPool) : base(handle) =>
        this.descriptorPool = descriptorPool;

    protected override void OnDispose()
    {
        var handle = this.handle;

        VkException.Check(PInvoke.vkFreeDescriptorSets(this.descriptorPool.Device.Handle, this.descriptorPool.Handle, 1, &handle));
    }
}