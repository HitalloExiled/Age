using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionTypeParameter : ManagedSlang
{
    internal SlangReflectionTypeParameter(nint handle) : base(handle)
    { }

    public nint GetConstraintByIndex(uint index) =>
        PInvoke.spReflectionTypeParameter_GetConstraintByIndex(this.Handle, index);

    public uint GetConstraintCount() =>
        PInvoke.spReflectionTypeParameter_GetConstraintCount(this.Handle);

    public uint GetIndex() =>
        PInvoke.spReflectionTypeParameter_GetIndex(this.Handle);

    public string GetName() =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionTypeParameter_GetName(this.Handle))!;
}
