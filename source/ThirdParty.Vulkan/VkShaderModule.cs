using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe partial class VkShaderModule : VkDeviceResource<VkShaderModule>
{
    internal VkShaderModule(VkDevice device, in VkShaderModuleCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkShaderModule>* pHandle     = &this.handle)
        fixed (VkShaderModuleCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*    pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateShaderModule(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyShaderModule(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
