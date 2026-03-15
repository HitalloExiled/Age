using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionTypeParameter : SessionResource<SlangReflectionTypeParameter>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionTypeParameter_GetName(this.Handle))!;

    public uint ConstraintCount => PInvoke.spReflectionTypeParameter_GetConstraintCount(this.Handle);
    public uint Index           => PInvoke.spReflectionTypeParameter_GetIndex(this.Handle);

    internal SlangReflectionTypeParameter(Session session, Handle<SlangReflectionTypeParameter> handle) : base(session, handle)
    { }

    public SlangReflectionType GetConstraintByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionTypeParameter_GetConstraintByIndex(this.Handle, index));
}
