namespace ThirdParty.Vulkan;

public unsafe partial record AllocationCallbacks
{
    private struct Callbacks
    {
        public nint Allocation;
        public nint Free;
        public nint InternalAllocation;
        public nint InternalFree;
        public nint Reallocation;
    }
}
