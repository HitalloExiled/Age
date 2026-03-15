using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class TypeParameterReflection : Managed<TypeParameterReflection>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionTypeParameter_GetName(this.Handle))!;

    public uint ConstraintCount => PInvoke.spReflectionTypeParameter_GetConstraintCount(this.Handle);
    public uint Index           => PInvoke.spReflectionTypeParameter_GetIndex(this.Handle);

    internal TypeParameterReflection(Session session, Handle<TypeParameterReflection> handle) : base(session, handle)
    { }

    public TypeReflection GetConstraintByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionTypeParameter_GetConstraintByIndex(this.Handle, index));
}
