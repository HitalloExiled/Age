using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkShaderModule : DeviceResource<VkShaderModule>
{
    internal VkShaderModule(VkDevice device, in VkShaderModuleCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkShaderModule>* pHandle     = &this.Handle)
        fixed (VkShaderModuleCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*    pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateShaderModule(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyShaderModule(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
