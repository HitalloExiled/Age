namespace ThirdParty.Vulkan;

public sealed unsafe class VkQueue : ManagedHandle<VkQueue>
{
    public uint FamilyIndex { get; }
    public uint Index       { get; }

    internal VkQueue(VkDevice device, uint familyIndex, uint index)
    {
        this.FamilyIndex = familyIndex;
        this.Index       = index;

        fixed (VkHandle<VkQueue>* pHandle = &this.handle)
        {
            PInvoke.vkGetDeviceQueue(device.Handle, familyIndex, index, pHandle);
        }
    }

    public void Submit(in VkSubmitInfo submitInfo, VkFence? fence = null)
    {
        fixed (VkSubmitInfo* pSubmitInfo = &submitInfo)
        {
            VkException.Check(PInvoke.vkQueueSubmit(this.handle, 1, pSubmitInfo, fence?.Handle ?? default));
        }
    }

    public void Submit(VkSubmitInfo[] submits, VkFence? fence = null)
    {
        fixed (VkSubmitInfo* pSubmits = submits)
        {
            VkException.Check(PInvoke.vkQueueSubmit(this.handle, (uint)submits.Length, pSubmits, fence?.Handle ?? default));
        }
    }

    public void WaitIdle() =>
        VkException.Check(PInvoke.vkQueueWaitIdle(this.handle));
}
