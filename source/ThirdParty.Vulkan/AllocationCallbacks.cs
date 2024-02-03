using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkAllocationCallbacks.html">VkAllocationCallbacks</see>
/// </summary>
public unsafe partial record AllocationCallbacks : NativeReference<VkAllocationCallbacks>
{
    private readonly Ref<Callbacks> callbacksRef = new();

    private AllocationFunction?             allocation;
    private FreeFunction?                   free;
    private InternalAllocationNotification? internalAllocation;
    private InternalFreeNotification?       internalFree;
    private ReallocationFunction?           reallocation;

    public AllocationFunction? Allocation
    {
        get => this.allocation;
        set
        {
            this.callbacksRef.Handle->Allocation = ToPointer(ref this.allocation, value);
            this.PNative->pfnAllocation = Marshal.GetFunctionPointerForDelegate(AllocationFunctionCallback);
        }
    }

    public new FreeFunction? Free
    {
        get => this.free;
        set
        {
            this.callbacksRef.Handle->Free = ToPointer(ref this.free, value);
            this.PNative->pfnFree = Marshal.GetFunctionPointerForDelegate(FreeFunctionCallback);
        }
    }

    public InternalAllocationNotification? InternalAllocation
    {
        get => this.internalAllocation;
        set
        {
            this.callbacksRef.Handle->InternalAllocation = ToPointer(ref this.internalAllocation, value);
            this.PNative->pfnInternalAllocation = Marshal.GetFunctionPointerForDelegate(InternalAllocationNotificationCallback);
        }
    }

    public InternalFreeNotification? InternalFree
    {
        get => this.internalFree;
        set
        {
            this.callbacksRef.Handle->InternalFree = ToPointer(ref this.internalFree, value);
            this.PNative->pfnInternalFree = Marshal.GetFunctionPointerForDelegate(InternalFreeNotificationCallback);
        }
    }

    public ReallocationFunction? Reallocation
    {
        get => this.reallocation;
        set
        {
            this.callbacksRef.Handle->Reallocation = ToPointer(ref this.reallocation, value);
            this.PNative->pfnReallocation = Marshal.GetFunctionPointerForDelegate(InternalFreeNotificationCallback);
        }
    }

    public AllocationCallbacks() =>
        this.PNative->pUserData = this.callbacksRef.Handle;

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
        this.callbacksRef.Free();
}
