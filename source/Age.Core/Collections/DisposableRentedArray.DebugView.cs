using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct DisposableRentedArray<T> where T : IDisposable
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(DisposableRentedArray<T> source)
    {
        private readonly DisposableRentedArray<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [..this.source];
    }
}
