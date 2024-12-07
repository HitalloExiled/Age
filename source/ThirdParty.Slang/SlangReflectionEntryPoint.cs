using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionEntryPoint
{
    internal SlangReflectionEntryPointHandle Handle { get; }

    internal SlangReflectionEntryPoint(SlangReflectionEntryPointHandle handle) =>
        this.Handle = handle;

    public nint GetFunction() =>
        PInvoke.spReflectionEntryPoint_getFunction(this.Handle);

    public string GetName() =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getName(this.Handle))!;

    public string GetNameOverride() =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getNameOverride(this.Handle))!;

    public SlangReflectionVariableLayoutHandle GetParameterByIndex(uint index) =>
        PInvoke.spReflectionEntryPoint_getParameterByIndex(this.Handle, index);

    public uint GetParameterCount() =>
        PInvoke.spReflectionEntryPoint_getParameterCount(this.Handle);

    public SlangStage GetStage() =>
        PInvoke.spReflectionEntryPoint_getStage(this.Handle);
}
