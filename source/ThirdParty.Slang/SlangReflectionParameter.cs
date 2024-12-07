using Age.Core;

namespace ThirdParty.Slang;

public class SlangReflectionParameter : Disposable
{
    public SlangReflectionParameterHandle Handle { get; }

    internal SlangReflectionParameter(SlangReflectionParameterHandle handle) =>
        this.Handle = handle;

    protected override void Disposed(bool disposing) => throw new NotImplementedException();

    public uint GetBindingIndex() =>
        PInvoke.spReflectionParameter_GetBindingIndex(this.Handle);

    public uint GetBindingSpace() =>
        PInvoke.spReflectionParameter_GetBindingSpace(this.Handle);
}
