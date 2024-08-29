using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThirdParty.SpirvCross;

[DebuggerDisplay("{Value}")]
public record struct ParsedSpirv(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ParsedSpirv(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(ParsedSpirv value) => value.Value;
}
