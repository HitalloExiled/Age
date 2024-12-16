namespace ThirdParty.Slang;

public class SlangReflectionParameter : ManagedSlang
{
    public uint BindingIndex => PInvoke.spReflectionParameter_GetBindingIndex(this.Handle);
    public uint BindingSpace => PInvoke.spReflectionParameter_GetBindingSpace(this.Handle);

    internal SlangReflectionParameter(nint handle) : base(handle)
    { }
}
