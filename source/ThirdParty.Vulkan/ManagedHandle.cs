namespace ThirdParty.Vulkan;

public abstract class ManagedHandle<T> where T : ManagedHandle<T>
{
    public readonly VkHandle<T> Handle;

    internal ManagedHandle() { }

    internal ManagedHandle(VkHandle<T> handle) =>
        this.Handle = handle;

    public static VkHandle<U>[] ToHandlers<U>(IList<ManagedHandle<U>> source) where U : ManagedHandle<U>
    {
        var handlers = new VkHandle<U>[source.Count];

        for (var i = 0; i < source.Count; i++)
        {
            handlers[i] = new(source[i].Handle);
        }

        return handlers;
    }
}
