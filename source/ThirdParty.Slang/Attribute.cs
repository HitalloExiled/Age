using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class Attribute : Managed<Attribute>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetName(this.Handle))!;

    public uint ArgumentCount => PInvoke.spReflectionUserAttribute_GetArgumentCount(this.Handle);

    internal Attribute(Session session, Handle<Attribute> handle) : base(session, handle)
    { }

    public SlangResult GetArgumentValueFloat(uint index, ReadOnlySpan<float> rs)
    {
        fixed (float* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueFloat(this.Handle, index, pRs);
        }
    }

    public SlangResult GetArgumentValueInt(uint index, ReadOnlySpan<int> rs)
    {
        fixed (int* pRs = rs)
        {
            return PInvoke.spReflectionUserAttribute_GetArgumentValueInt(this.Handle, index, pRs);
        }
    }

    public string GetArgumentValueString(uint index, ReadOnlySpan<ulong> bufLen)
    {
        fixed (ulong* pBufLen = bufLen)
        {
            return Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionUserAttribute_GetArgumentValueString(this.Handle, index, pBufLen))!;
        }
    }
}
