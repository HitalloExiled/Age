using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkQueue : ManagedHandle<VkQueue>
{
    public uint FamilyIndex { get; }
    public uint Index       { get; }

    internal VkQueue(VkDevice device, uint familyIndex, uint index)
    {
        this.FamilyIndex = familyIndex;
        this.Index       = index;

        fixed (VkHandle<VkQueue>* pHandle = &this.Handle)
        {
            PInvoke.vkGetDeviceQueue(device, familyIndex, index, pHandle);
        }
    }

    public void Submit(in VkSubmitInfo submitInfo, VkFence? fence = null)
    {
        fixed (VkSubmitInfo* pSubmitInfo = &submitInfo)
        {
            VkException.Check(PInvoke.vkQueueSubmit(this, 1, pSubmitInfo, fence));
        }
    }

    public void Submit(VkSubmitInfo[] submits, VkFence? fence = null)
    {
        fixed (VkSubmitInfo* pSubmits = submits)
        {
            VkException.Check(PInvoke.vkQueueSubmit(this, (uint)submits.Length, pSubmits, fence));
        }
    }

    public void WaitIdle() =>
        VkException.Check(PInvoke.vkQueueWaitIdle(this));
}
