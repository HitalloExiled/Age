using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflection(SlangCompileRequest request) : ManagedSlang<SlangReflection>(PInvoke.spGetReflection(request.Handle))
{
    public SlangCompileRequest Request { get; } = request;

    [field: AllowNull]
    public SlangReflectionEntryPoint[] EntryPoints
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionEntryPoint[this.EntryPointCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetEntryPointByIndex((uint)i);
                }
            }

            return field;
        }
    }

    [field: AllowNull]
    public SlangReflectionTypeLayout GlobalParamsTypeLayout => field ??= new(PInvoke.spReflection_getGlobalParamsTypeLayout(this.Handle));

    [field: AllowNull]
    public SlangReflectionVariableLayout GlobalParamsVarLayout => field ??= new(PInvoke.spReflection_getGlobalParamsVarLayout(this.Handle));

    [field: AllowNull]
    public SlangReflectionParameter[] Parameters
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionParameter[this.ParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetParameterByIndex((uint)i);
                }
            }

            return field;
        }
    }

    [field: AllowNull]
    public SlangReflectionTypeParameter[] TypeParameters
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionTypeParameter[this.TypeParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetTypeParameterByIndex((uint)i);
                }
            }

            return field;
        }
    }

    public ulong        EntryPointCount             => PInvoke.spReflection_getEntryPointCount(this.Handle);
    public ulong        GlobalConstantBufferBinding => PInvoke.spReflection_getGlobalConstantBufferBinding(this.Handle);
    public ulong        GlobalConstantBufferSize    => PInvoke.spReflection_getGlobalConstantBufferSize(this.Handle);
    public ulong        HashedStringCount           => PInvoke.spReflection_getHashedStringCount(this.Handle);
    public uint         ParameterCount              => PInvoke.spReflection_GetParameterCount(this.Handle);
    public SlangSession Session                     => this.Request.Session;
    public uint         TypeParameterCount          => PInvoke.spReflection_GetTypeParameterCount(this.Handle);

    public static SlangReflectionType GetTypeFromDecl(SlangReflectionDecl decl) =>
        new(PInvoke.spReflection_getTypeFromDecl(decl.Handle));

    public SlangReflectionEntryPoint? FindEntryPointByName(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_findEntryPointByName(this.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionFunction? FindFunctionByName(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindFunctionByName(this.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionFunction? FindFunctionByNameInType(SlangReflectionType reflType, string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindFunctionByNameInType(this.Handle, reflType.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionType? FindTypeByName(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindTypeByName(this.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionTypeParameter? FindTypeParameter(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindTypeParameter(this.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionVariable? FindVarByNameInType(SlangReflectionType reflType, string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindVarByNameInType(this.Handle, reflType.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionEntryPoint GetEntryPointByIndex(ulong index) =>
        new(PInvoke.spReflection_getEntryPointByIndex(this.Handle, index));

    public string GetHashedString(ulong index, scoped ReadOnlySpan<ulong> outCount)
    {
        fixed (ulong* pOutCount = outCount)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflection_getHashedString(this.Handle, index, pOutCount))!;
        }
    }

    public SlangReflectionParameter GetParameterByIndex(uint index) =>
        new(PInvoke.spReflection_GetParameterByIndex(this.Handle, index));

    public SlangReflectionTypeLayout GetTypeLayout(SlangReflectionType inType, SlangLayoutRules rules) =>
        new(PInvoke.spReflection_GetTypeLayout(this.Handle, inType.Handle, rules));

    public SlangReflectionTypeParameter GetTypeParameterByIndex(uint index) =>
        new(PInvoke.spReflection_GetTypeParameterByIndex(this.Handle, index));

    public bool IsSubType(SlangReflectionType subType, SlangReflectionType superType) =>
        PInvoke.spReflection_isSubType(this.Handle, subType.Handle, superType.Handle);

    public SlangReflectionGeneric SpecializeGeneric(SlangReflectionGeneric generic, long argCount, SlangReflectionGenericArgType argTypes, SlangReflectionGenericArg args, scoped ReadOnlySpan<nint> outDiagnostics)
    {
        fixed (nint* pOutDiagnostics = outDiagnostics)
        {
            return new(PInvoke.spReflection_specializeGeneric(this.Handle, generic.Handle, argCount, argTypes.Handle, args.Handle, pOutDiagnostics));
        }
    }

    public SlangReflectionType SpecializeType(SlangReflectionType inType, long specializationArgCount, SlangReflectionType specializationArgs, scoped ReadOnlySpan<nint> outDiagnostics)
    {
        fixed (nint* pOutDiagnostics = outDiagnostics)
        {
            return new(PInvoke.spReflection_specializeType(this.Handle, inType.Handle, specializationArgCount, specializationArgs.Handle, pOutDiagnostics));
        }
    }
}
