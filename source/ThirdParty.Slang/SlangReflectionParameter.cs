namespace ThirdParty.Slang;

public class SlangReflectionParameter : ManagedSlang<SlangReflectionParameter>
{
    public uint BindingIndex => PInvoke.spReflectionParameter_GetBindingIndex(this.Handle);
    public uint BindingSpace => PInvoke.spReflectionParameter_GetBindingSpace(this.Handle);

    internal SlangReflectionParameter(Handle<SlangReflectionParameter> handle) : base(handle)
    { }
}
