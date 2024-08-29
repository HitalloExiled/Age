using System.Diagnostics;

namespace ThirdParty.Vulkan;

[DebuggerDisplay("{Value}")]
public readonly struct VkHandle<T>(nint value) where T : ManagedHandle<T>
{
    public readonly nint Value = value;

    public static implicit operator nint(VkHandle<T> handle) => handle.Value;
}

public static class VkHandle
{
    public static VkHandle<T>[] GetHandles<T>(Span<T> managedHandles) where T : ManagedHandle<T>
    {
        var handles = new VkHandle<T>[managedHandles.Length];

        for (var i = 0; i < managedHandles.Length; i++)
        {
            handles[i] = managedHandles[i].Handle;
        }

        return handles;
    }
}
