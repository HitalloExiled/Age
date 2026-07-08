using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class ShaderReflection : Managed<ShaderReflection>
{
    [field: AllowNull]
    public EntryPointReflection[] EntryPoints
    {
        get
        {
            if (field == null)
            {
                field = new EntryPointReflection[this.EntryPointCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetEntryPointByIndex((uint)i);
                }
            }

            return field;
        }
    }

    [field: AllowNull]
    public TypeLayoutReflection GlobalParamsTypeLayout => field ??= new(this.Session, PInvoke.spReflection_getGlobalParamsTypeLayout(this.Handle));

    [field: AllowNull]
    public VariableLayoutReflection GlobalParamsVarLayout => field ??= new(this.Session, PInvoke.spReflection_getGlobalParamsVarLayout(this.Handle));

    [field: AllowNull]
    public VariableLayoutReflection[] Parameters
    {
        get
        {
            if (field == null)
            {
                field = new VariableLayoutReflection[this.ParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetParameterByIndex((uint)i);
                }
            }

            return field;
        }
    }

    [field: AllowNull]
    public TypeParameterReflection[] TypeParameters
    {
        get
        {
            if (field == null)
            {
                field = new TypeParameterReflection[this.TypeParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetTypeParameterByIndex((uint)i);
                }
            }

            return field;
        }
    }

    public ulong EntryPointCount             => PInvoke.spReflection_getEntryPointCount(this.Handle);
    public ulong GlobalConstantBufferBinding => PInvoke.spReflection_getGlobalConstantBufferBinding(this.Handle);
    public ulong GlobalConstantBufferSize    => PInvoke.spReflection_getGlobalConstantBufferSize(this.Handle);
    public ulong HashedStringCount           => PInvoke.spReflection_getHashedStringCount(this.Handle);
    public uint  ParameterCount              => PInvoke.spReflection_GetParameterCount(this.Handle);
    public uint  TypeParameterCount          => PInvoke.spReflection_GetTypeParameterCount(this.Handle);
    public long  BindlessSpaceIndex          => PInvoke.spReflection_getBindlessSpaceIndex(this.Handle);

    internal ShaderReflection(Session session, Handle<ShaderReflection> handle) : base(session, handle) { }

    public TypeReflection GetTypeFromDecl(SlangReflectionDecl decl) =>
        new(this.Session, PInvoke.spReflection_getTypeFromDecl(decl));

    public EntryPointReflection? FindEntryPointByName(string name)
    {
        using var pName = new NativeString(name);
        {
            var handle = PInvoke.spReflection_findEntryPointByName(this.Handle, pName);

            return handle == default ? null : new(this.Session, handle);
        }
    }

    public FunctionReflection? FindFunctionByName(string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflection_FindFunctionByName(this.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public FunctionReflection? FindFunctionByNameInType(TypeReflection reflType, string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflection_FindFunctionByNameInType(this.Handle, reflType.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public TypeReflection? FindTypeByName(string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflection_FindTypeByName(this.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public TypeParameterReflection? FindTypeParameter(string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflection_FindTypeParameter(this.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public VariableReflection? FindVarByNameInType(TypeReflection type, string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflection_FindVarByNameInType(this.Handle, type.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public EntryPointReflection GetEntryPointByIndex(ulong index) =>
        new(this.Session, PInvoke.spReflection_getEntryPointByIndex(this.Handle, index));

    public string GetHashedString(ulong index, ReadOnlySpan<ulong> outCount)
    {
        fixed (ulong* pOutCount = outCount)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflection_getHashedString(this.Handle, index, pOutCount))!;
        }
    }

    public VariableLayoutReflection GetParameterByIndex(uint index) =>
        new(this.Session, PInvoke.spReflection_GetParameterByIndex(this.Handle, index));

    public TypeLayoutReflection GetTypeLayout(TypeReflection inType, SlangLayoutRules rules) =>
        new(this.Session, PInvoke.spReflection_GetTypeLayout(this.Handle, inType.Handle, rules));

    public TypeParameterReflection GetTypeParameterByIndex(uint index) =>
        new(this.Session, PInvoke.spReflection_GetTypeParameterByIndex(this.Handle, index));

    public bool IsSubType(TypeReflection subType, TypeReflection superType) =>
        PInvoke.spReflection_isSubType(this.Handle, subType.Handle, superType.Handle);

    public GenericReflection SpecializeGeneric(GenericReflection generic, ReadOnlySpan<SlangReflectionGenericArgType> argTypes, ReadOnlySpan<ReflectionGenericArg> args)
    {
        var pArgTypes = stackalloc SlangReflectionGenericArgType[args.Length];
        var pArgs     = stackalloc ReflectionGenericArg[args.Length];

        return new(this.Session, PInvoke.spReflection_specializeGeneric(this.Handle, generic.Handle, argTypes.Length, pArgTypes, pArgs, null));
    }

    public TypeReflection SpecializeType(TypeReflection inType, long specializationArgCount, TypeReflection specializationArgs) =>
        new(this.Session, PInvoke.spReflection_specializeType(this.Handle, inType.Handle, specializationArgCount, specializationArgs.Handle, null));
}
