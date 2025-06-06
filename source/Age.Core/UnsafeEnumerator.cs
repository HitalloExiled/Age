using System.Collections;

namespace Age.Core;

public unsafe struct UnsafeEnumerator<T>(T* span, int length) : IEnumerator<T> where T : unmanaged
{
    private int index = -1;
    private readonly T* span = span;

    public readonly T Current => this.span[this.index];

    readonly object IEnumerator.Current => this.Current;

    public readonly void Dispose() { }
    public bool MoveNext() => ++this.index < length;
    public void Reset() => this.index = -1;
}
