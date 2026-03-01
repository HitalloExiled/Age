using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public partial class NativeStringList
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(NativeStringList source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[] Elements => source.ToArray();
    }
}
