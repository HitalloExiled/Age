using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionUserAttribute : ManagedSlang
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetName(this.Handle))!;

    public uint ArgumentCount => PInvoke.spReflectionUserAttribute_GetArgumentCount(this.Handle);

    internal SlangReflectionUserAttribute(nint handle) : base(handle)
    { }

    public SlangResult GetArgumentValueFloat(uint index, Span<float> rs)
    {
        fixed (float* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueFloat(this.Handle, index, pRs);
        }
    }

    public SlangResult GetArgumentValueInt(uint index, Span<int> rs)
    {
        fixed (int* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueInt(this.Handle, index, pRs);
        }
    }

    public string GetArgumentValueString(uint index, Span<ulong> bufLen)
    {
        fixed (ulong* pBufLen = bufLen)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetArgumentValueString(this.Handle, index, pBufLen))!;
        }
    }
}
