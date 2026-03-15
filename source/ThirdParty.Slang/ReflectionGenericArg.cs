using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public struct ReflectionType;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct ReflectionGenericArg
{
    [FieldOffset(0)]
    public SlangReflectionType* TypeVal;

    [FieldOffset(0)]
    public int64_t IntVal;

    [FieldOffset(0)]
    public bool BoolVal;
}
