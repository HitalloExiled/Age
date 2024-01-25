using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAllocationCallbacks.html">VkAllocationCallbacks</see>
/// </summary>
public unsafe partial record AllocationCallbacks : NativeReference<VkAllocationCallbacks>
{
    private AllocationFunction?             allocation;
    private FreeFunction?                   free;
    private InternalAllocationNotification? internalAllocation;
    private InternalFreeNotification?       internalFree;
    private Ptr<Callbacks>                  pCallbacks = Alloc(new Callbacks());
    private ReallocationFunction?           reallocation;

    public AllocationFunction? Allocation
    {
        get => this.allocation;
        set
        {
            this.pCallbacks.Value->Allocation = ToPointer(ref this.allocation, value);
            this.PNative->pfnAllocation = Marshal.GetFunctionPointerForDelegate(AllocationFunctionCallback);
        }
    }

    public new FreeFunction? Free
    {
        get => this.free;
        set
        {
            this.pCallbacks.Value->Free = ToPointer(ref this.free, value);
            this.PNative->pfnFree = Marshal.GetFunctionPointerForDelegate(FreeFunctionCallback);
        }
    }

    public InternalAllocationNotification? InternalAllocation
    {
        get => this.internalAllocation;
        set
        {
            this.pCallbacks.Value->InternalAllocation = ToPointer(ref this.internalAllocation, value);
            this.PNative->pfnInternalAllocation = Marshal.GetFunctionPointerForDelegate(InternalAllocationNotificationCallback);
        }
    }

    public InternalFreeNotification? InternalFree
    {
        get => this.internalFree;
        set
        {
            this.pCallbacks.Value->InternalFree = ToPointer(ref this.internalFree, value);
            this.PNative->pfnInternalFree = Marshal.GetFunctionPointerForDelegate(InternalFreeNotificationCallback);
        }
    }

    public ReallocationFunction? Reallocation
    {
        get => this.reallocation;
        set
        {
            this.pCallbacks.Value->Reallocation = ToPointer(ref this.reallocation, value);
            this.PNative->pfnReallocation = Marshal.GetFunctionPointerForDelegate(InternalFreeNotificationCallback);
        }
    }

    public AllocationCallbacks() =>
        this.PNative->pUserData = this.pCallbacks.Value;

    private static void* AllocationFunctionCallback(void* pUserData, size_t size, size_t alignment, VkSystemAllocationScope allocationScope)
    {
        var callback = Marshal.GetDelegateForFunctionPointer<AllocationFunction>(((Callbacks*)pUserData)->Allocation);

        return callback(size, alignment, allocationScope).ToPointer();
    }

    private static void FreeFunctionCallback(void* pUserData, void* pMemory)
    {
        var callback = Marshal.GetDelegateForFunctionPointer<FreeFunction>(((Callbacks*)pUserData)->Allocation);

        callback.Invoke((nint)pMemory);
    }

    private static void InternalAllocationNotificationCallback(void* pUserData, size_t size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope)
    {
        var callback = Marshal.GetDelegateForFunctionPointer<InternalAllocationNotification>(((Callbacks*)pUserData)->Allocation);

        callback.Invoke(size, allocationType, allocationScope);
    }

    private static void InternalFreeNotificationCallback(void* pUserData, size_t size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope)
    {
        var callback = Marshal.GetDelegateForFunctionPointer<InternalFreeNotification>(((Callbacks*)pUserData)->Allocation);

        callback.Invoke(size, allocationType, allocationScope);
    }

    private static void* ReallocationFunctionCallback(void* pUserData, void* pOriginal, size_t size, size_t alignment, VkSystemAllocationScope allocationScope)
    {
        var callback = Marshal.GetDelegateForFunctionPointer<ReallocationFunction>(((Callbacks*)pUserData)->Allocation);

        return callback.Invoke((nint)pOriginal, size, alignment, allocationScope).ToPointer();
    }

    protected override void OnFinalize() =>
        NativeReference.Free(ref this.pCallbacks);
}
