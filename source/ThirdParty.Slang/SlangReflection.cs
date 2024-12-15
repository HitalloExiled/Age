using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflection(SlangCompileRequest request) : ManagedSlang(PInvoke.spGetReflection(request.Handle))
{
    public SlangCompileRequest Request { get; } = request;
    public SlangSession Session => this.Request.Session;

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

    public SlangReflectionFunction? FindFunctionByNameInType(SlangReflectionTypeHandle reflType, string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindFunctionByNameInType(this.Handle, reflType, pName);

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

    public SlangReflectionVariable? FindVarByNameInType(SlangReflectionTypeHandle reflType, string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflection_FindVarByNameInType(this.Handle, reflType, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionEntryPoint[] GetEntryPoints()
    {
        var entryPoints = new SlangReflectionEntryPoint[this.GetEntryPointCount()];

        for (var i = 0; i < entryPoints.Length; i++)
        {
            entryPoints[i] = new(PInvoke.spReflection_getEntryPointByIndex(this.Handle, (uint)i));
        }

        return entryPoints;
    }

    public SlangReflectionEntryPoint GetEntryPointByIndex(ulong index) =>
        new(PInvoke.spReflection_getEntryPointByIndex(this.Handle, index));

    public ulong GetEntryPointCount() =>
        PInvoke.spReflection_getEntryPointCount(this.Handle);

    public ulong GetGlobalConstantBufferBinding() =>
        PInvoke.spReflection_getGlobalConstantBufferBinding(this.Handle);

    public ulong GetGlobalConstantBufferSize() =>
        PInvoke.spReflection_getGlobalConstantBufferSize(this.Handle);

    public SlangReflectionTypeLayoutHandle GetGlobalParamsTypeLayout() =>
        PInvoke.spReflection_getGlobalParamsTypeLayout(this.Handle);

    public SlangReflectionVariableLayoutHandle GetGlobalParamsVarLayout() =>
        PInvoke.spReflection_getGlobalParamsVarLayout(this.Handle);

    public string GetHashedString(ulong index, Span<ulong> outCount)
    {
        fixed (ulong* pOutCount = outCount)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflection_getHashedString(this.Handle, index, pOutCount))!;
        }
    }

    public ulong GetHashedStringCount() =>
        PInvoke.spReflection_getHashedStringCount(this.Handle);

    public SlangReflectionParameter[] GetParameters()
    {
        var parameters = new SlangReflectionParameter[this.GetParameterCount()];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = new(PInvoke.spReflection_GetParameterByIndex(this.Handle, (uint)i));
        }

        return parameters;
    }

    public SlangReflectionParameter GetParameterByIndex(uint index) =>
        new(PInvoke.spReflection_GetParameterByIndex(this.Handle, index));

    public uint GetParameterCount() =>
        PInvoke.spReflection_GetParameterCount(this.Handle);

    public SlangReflectionTypeLayout GetTypeLayout(SlangReflectionType inType, SlangLayoutRules rules) =>
        new(PInvoke.spReflection_GetTypeLayout(this.Handle, inType.Handle, rules));

    public SlangReflectionTypeParameter[] GetTypeParameters(uint index)
    {
        var typeParameters = new SlangReflectionTypeParameter[this.GetTypeParameterCount()];

        for (var i = 0; i < typeParameters.Length; i++)
        {
            typeParameters[i] = new(PInvoke.spReflection_GetTypeParameterByIndex(this.Handle, index));
        }

        return typeParameters;
    }

    public SlangReflectionTypeParameter GetTypeParameterByIndex(uint index) =>
        new(PInvoke.spReflection_GetTypeParameterByIndex(this.Handle, index));

    public uint GetTypeParameterCount() =>
        PInvoke.spReflection_GetTypeParameterCount(this.Handle);

    public bool IsSubType(SlangReflectionType subType, SlangReflectionTypeHandle superType) =>
        PInvoke.spReflection_isSubType(this.Handle, subType.Handle, superType);

    public SlangReflectionGeneric SpecializeGeneric(SlangReflectionGeneric generic, long argCount, SlangReflectionGenericArgType argTypes, SlangReflectionGenericArg args, Span<nint> outDiagnostics)
    {
        fixed (nint* pOutDiagnostics = outDiagnostics)
        {
            return new(PInvoke.spReflection_specializeGeneric(this.Handle, generic.Handle, argCount, argTypes.Handle, args.Handle, pOutDiagnostics));
        }
    }

    public SlangReflectionType SpecializeType(SlangReflectionTypeHandle inType, long specializationArgCount, SlangReflectionTypeHandle specializationArgs, Span<nint> outDiagnostics)
    {
        fixed (nint* pOutDiagnostics = outDiagnostics)
        {
            return new(PInvoke.spReflection_specializeType(this.Handle, inType, specializationArgCount, specializationArgs, pOutDiagnostics));
        }
    }
}
