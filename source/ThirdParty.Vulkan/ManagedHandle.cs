namespace ThirdParty.Vulkan;

#pragma warning disable IDE1006

public abstract class ManagedHandle<T> where T : ManagedHandle<T>
{
    protected readonly VkHandle<T> handle;

    public VkHandle<T> Handle => this.handle;

    internal ManagedHandle() { }

    internal ManagedHandle(VkHandle<T> handle) =>
        this.handle = handle;

    public static VkHandle<U>[] ToHandlers<U>(IList<ManagedHandle<U>> source) where U : ManagedHandle<U>
    {
        var handlers = new VkHandle<U>[source.Count];

        for (var i = 0; i < source.Count; i++)
        {
            handlers[i] = new(source[i].handle);
        }

        return handlers;
    }
}
