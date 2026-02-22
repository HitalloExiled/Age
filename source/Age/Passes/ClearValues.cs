using Age.Numerics;
using Age.Rendering.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Passes;

[InlineArray(2)]
public struct ClearValues
{
    private ClearValue element0;

    public static ClearValues Default => new(new(Color.Black), new(1, 0));

    public ClearValues(ClearValue clearValue0, ClearValue clearValue1)
    {
        this[0] = clearValue0;
        this[1] = clearValue1;
    }

    public ReadOnlySpan<ClearValue> AsReadOnlySpan() =>
        MemoryMarshal.CreateReadOnlySpan(ref this.element0, 2);
}
