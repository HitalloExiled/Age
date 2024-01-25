using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Queue : NativeHandle
{
    public uint FamilyIndex { get; }
    public uint Index       { get; }

    internal Queue(Device device, uint familyIndex, uint index)
    {
        this.FamilyIndex = familyIndex;
        this.Index       = index;

        fixed (VkQueue* pHandle = &this.Handle)
        {
            PInvoke.vkGetDeviceQueue(device, familyIndex, index, pHandle);
        }
    }

    public void Submit(SubmitInfo submitInfo, Fence? fence = null) =>
        this.Submit([submitInfo], fence);

    public void Submit(SubmitInfo[] submits, Fence? fence = null)
    {
        fixed (VkSubmitInfo* pSubmits = submits.Select(x => (VkSubmitInfo)x).ToArray())
        {
            VulkanException.Check(PInvoke.vkQueueSubmit(this.Handle, (uint)submits.Length, pSubmits, fence));
        }
    }

    public void WaitIdle() => throw new NotImplementedException();
}
