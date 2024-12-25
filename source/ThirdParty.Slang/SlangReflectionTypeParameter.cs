using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionTypeParameter : ManagedSlang<SlangReflectionTypeParameter>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionTypeParameter_GetName(this.Handle))!;

    public uint ConstraintCount => PInvoke.spReflectionTypeParameter_GetConstraintCount(this.Handle);
    public uint Index           => PInvoke.spReflectionTypeParameter_GetIndex(this.Handle);

    internal SlangReflectionTypeParameter(Handle<SlangReflectionTypeParameter> handle) : base(handle)
    { }

    public SlangReflectionType GetConstraintByIndex(uint index) =>
        new(PInvoke.spReflectionTypeParameter_GetConstraintByIndex(this.Handle, index));
}
