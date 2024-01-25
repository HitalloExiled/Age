namespace ThirdParty.Vulkan;

public abstract class NativeHandle
{
    internal readonly nint Handle;

    internal NativeHandle() { }

    internal NativeHandle(nint handle) =>
        this.Handle = handle;

    public static implicit operator nint(NativeHandle? handle) => handle == null ? default : handle.Handle;
}
