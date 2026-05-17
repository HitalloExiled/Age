using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeStringList
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly struct DebugView(NativeStringList source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[]? Elements => source.IsCreated ? source.ToArray() : null;
    }
}
