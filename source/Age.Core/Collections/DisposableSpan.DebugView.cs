using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public readonly ref partial struct DisposableSpan<T> where T : IDisposable
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(DisposableSpan<T> source)
    {
        private readonly DisposableSpan<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [..this.source];
    }
}
