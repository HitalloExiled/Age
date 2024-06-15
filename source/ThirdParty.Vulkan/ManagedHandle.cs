namespace ThirdParty.Vulkan;

#pragma warning disable IDE1006

public abstract class ManagedHandle<T> where T : ManagedHandle<T>
{
    protected readonly VkHandle<T> handle;

    public VkHandle<T> Handle => this.handle;

    internal ManagedHandle() { }

    internal ManagedHandle(VkHandle<T> handle) =>
        this.handle = handle;
}
