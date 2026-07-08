using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct RentedArray<T>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly struct DebugView(RentedArray<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [..source];
    }
}
