namespace ThirdParty.Vulkan;

public readonly struct VkHandle<T>(nint value) where T : ManagedHandle<T>
{
    public readonly nint Value = value;

    public static implicit operator nint(VkHandle<T> handle) => handle.Value;
}
