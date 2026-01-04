using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public partial struct InlineList4<T>
{
    [InlineArray(CAPACITY)]
    private struct Buffer
    {
        #pragma warning disable RCS1169, RCS1213
        private T element0;
        #pragma warning restore RCS1169, RCS1213
    }
}
