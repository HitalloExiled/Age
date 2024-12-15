namespace ThirdParty.Slang;

public class SlangReflectionParameter : ManagedSlang
{
    internal SlangReflectionParameter(nint handle) : base(handle)
    { }

    public uint GetBindingIndex() =>
        PInvoke.spReflectionParameter_GetBindingIndex(this.Handle);

    public uint GetBindingSpace() =>
        PInvoke.spReflectionParameter_GetBindingSpace(this.Handle);
}
