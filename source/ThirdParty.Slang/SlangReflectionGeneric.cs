using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionGeneric : ManagedSlang<SlangReflectionGeneric>
{
    [field: AllowNull]
    public SlangReflectionDecl? AsDecl => field ??= PInvoke.spReflectionGeneric_asDecl(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionDecl? InnerDecl => field ??= PInvoke.spReflectionGeneric_GetInnerDecl(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionGeneric_GetName(this.Handle))!;

    [field: AllowNull]
    public SlangReflectionGeneric? OuterGenericContainer => field ??= PInvoke.spReflectionGeneric_GetOuterGenericContainer(this.Handle) is var x && x != default ? new(x) : null;

    public SlangDeclKind InnerKind           => PInvoke.spReflectionGeneric_GetInnerKind(this.Handle);
    public uint          TypeParameterCount  => PInvoke.spReflectionGeneric_GetTypeParameterCount(this.Handle);
    public uint          ValueParameterCount => PInvoke.spReflectionGeneric_GetValueParameterCount(this.Handle);

    internal SlangReflectionGeneric(Handle<SlangReflectionGeneric> handle) : base(handle)
    { }

    public SlangReflectionGeneric applySpecializations(SlangReflectionGeneric generic) =>
        new(PInvoke.spReflectionGeneric_applySpecializations(this.Handle, generic.Handle));

    public int64_t GetConcreteIntVal(SlangReflectionVariable valueParam) =>
        PInvoke.spReflectionGeneric_GetConcreteIntVal(this.Handle, valueParam.Handle);

    public SlangReflectionType GetConcreteType(SlangReflectionVariable typeParam) =>
        new(PInvoke.spReflectionGeneric_GetConcreteType(this.Handle, typeParam.Handle));
    public SlangReflectionVariable GetTypeParameter(uint index) =>
        new(PInvoke.spReflectionGeneric_GetTypeParameter(this.Handle, index));

    public uint GetTypeParameterConstraintCount(SlangReflectionVariable typeParam) =>
        PInvoke.spReflectionGeneric_GetTypeParameterConstraintCount(this.Handle, typeParam.Handle);

    public SlangReflectionType GetTypeParameterConstraintType(SlangReflectionVariable typeParam, uint index) =>
        new(PInvoke.spReflectionGeneric_GetTypeParameterConstraintType(this.Handle, typeParam.Handle, index));

    public SlangReflectionVariable GetValueParameter(uint index) =>
        new(PInvoke.spReflectionGeneric_GetValueParameter(this.Handle, index));
}
