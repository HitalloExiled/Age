using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionUserAttribute : ManagedSlang<SlangReflectionUserAttribute>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetName(this.Handle))!;

    public uint ArgumentCount => PInvoke.spReflectionUserAttribute_GetArgumentCount(this.Handle);

    internal SlangReflectionUserAttribute(Handle<SlangReflectionUserAttribute> handle) : base(handle)
    { }

    public SlangResult GetArgumentValueFloat(uint index, scoped ReadOnlySpan<float> rs)
    {
        fixed (float* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueFloat(this.Handle, index, pRs);
        }
    }

    public SlangResult GetArgumentValueInt(uint index, scoped ReadOnlySpan<int> rs)
    {
        fixed (int* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueInt(this.Handle, index, pRs);
        }
    }

    public string GetArgumentValueString(uint index, scoped ReadOnlySpan<ulong> bufLen)
    {
        fixed (ulong* pBufLen = bufLen)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetArgumentValueString(this.Handle, index, pBufLen))!;
        }
    }
}
