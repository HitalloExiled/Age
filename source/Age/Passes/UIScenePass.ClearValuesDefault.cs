using Age.Numerics;
using Age.Rendering.Resources;
using System.Runtime.CompilerServices;

namespace Age.Passes;

public abstract partial class UIScenePass
{
    [InlineArray(2)]
    private struct ClearValuesDefault
    {
#pragma warning disable RCS1213
        private ClearValue _;
#pragma warning restore RCS1213

        public ClearValuesDefault()
        {
            this[0] = new(Color.Black);
            this[1] = new(1, 0);
        }
    }
}
