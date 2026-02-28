using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct NativeStringRefList
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(NativeStringList source)
    {
        private readonly NativeStringList source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[] Elements => this.source.ToArray();
    }
}
