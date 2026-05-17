using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeStringArray
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly struct DebugView(NativeStringArray source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[]? Elements => source.IsCreated ? source.ToArray() : null;
    }
}
