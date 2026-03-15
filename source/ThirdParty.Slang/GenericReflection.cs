using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class GenericReflection : Managed<GenericReflection>
{
    public SlangReflectionDecl AsDecl => PInvoke.spReflectionGeneric_asDecl(this.Handle);

    [field: AllowNull]
    public SlangReflectionDecl? InnerDecl => field ??= PInvoke.spReflectionGeneric_GetInnerDecl(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionGeneric_GetName(this.Handle))!;

    [field: AllowNull]
    public GenericReflection? OuterGenericContainer => field ??= PInvoke.spReflectionGeneric_GetOuterGenericContainer(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    public SlangDeclKind InnerKind           => PInvoke.spReflectionGeneric_GetInnerKind(this.Handle);
    public uint          TypeParameterCount  => PInvoke.spReflectionGeneric_GetTypeParameterCount(this.Handle);
    public uint          ValueParameterCount => PInvoke.spReflectionGeneric_GetValueParameterCount(this.Handle);

    internal GenericReflection(Session session, Handle<GenericReflection> handle) : base(session, handle)
    { }

    public GenericReflection ApplySpecializations(GenericReflection generic) =>
        new(this.Session, PInvoke.spReflectionGeneric_applySpecializations(this.Handle, generic.Handle));

    public int64_t GetConcreteIntVal(VariableReflection valueParam) =>
        PInvoke.spReflectionGeneric_GetConcreteIntVal(this.Handle, valueParam.Handle);

    public TypeReflection GetConcreteType(VariableReflection typeParam) =>
        new(this.Session, PInvoke.spReflectionGeneric_GetConcreteType(this.Handle, typeParam.Handle));
    public VariableReflection GetTypeParameter(uint index) =>
        new(this.Session, PInvoke.spReflectionGeneric_GetTypeParameter(this.Handle, index));

    public uint GetTypeParameterConstraintCount(VariableReflection typeParam) =>
        PInvoke.spReflectionGeneric_GetTypeParameterConstraintCount(this.Handle, typeParam.Handle);

    public TypeReflection GetTypeParameterConstraintType(VariableReflection typeParam, uint index) =>
        new(this.Session, PInvoke.spReflectionGeneric_GetTypeParameterConstraintType(this.Handle, typeParam.Handle, index));

    public VariableReflection GetValueParameter(uint index) =>
        new(this.Session, PInvoke.spReflectionGeneric_GetValueParameter(this.Handle, index));
}
