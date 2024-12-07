using Age.Core;

namespace ThirdParty.Slang;

public class SlangReflection
{
    internal SlangReflectionHandle Handle { get; }

    public SlangReflection(SlangCompileRequest request)
    {
        if ((this.Handle = PInvoke.spGetReflection(request.Handle)) == default)
        {
            throw new InvalidOperationException();
        }
    }

    public string? FindEntryPointByName() => throw new NotImplementedException();

    public ulong GetEntryPointCount() =>
        PInvoke.spReflection_getEntryPointCount(this.Handle);

    public SlangReflectionEntryPoint? GetEntryPointByIndex(ulong index)
    {
        var handle = PInvoke.spReflection_getEntryPointByIndex(this.Handle, index);

        return handle == default ? null : new(PInvoke.spReflection_getEntryPointByIndex(this.Handle, index));
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

    public int GetGlobalConstantBufferBinding() => throw new NotImplementedException();
    public int GetGlobalConstantBufferSize() => throw new NotImplementedException();

    public uint GetParameterCount() =>
        PInvoke.spReflection_GetParameterCount(this.Handle);

    public SlangReflectionParameter GetParameterByIndex(uint index)
    {
        var handle = PInvoke.spReflection_GetParameterByIndex(this.Handle, index);

        return handle == default ? throw new InvalidOperationException() : new(handle);
    }
}
