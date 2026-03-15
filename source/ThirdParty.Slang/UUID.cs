using System.Runtime.CompilerServices;

namespace ThirdParty.Slang;

public struct UUID
{
    public uint32_t              Data1;
    public uint16_t              Data2;
    public uint16_t              Data3;
    public InlineArray8<uint8_t> Data4;
}
