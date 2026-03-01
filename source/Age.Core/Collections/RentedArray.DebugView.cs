using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct RentedArray<T>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(RentedArray<T> source)
    {
        private readonly RentedArray<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [..this.source];
    }
}
