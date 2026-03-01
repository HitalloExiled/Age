using System.Diagnostics;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DisposableSpan<>.DebugView))]
public readonly ref partial struct DisposableSpan<T>(ReadOnlySpan<T> values) where T : IDisposable
{
    public ReadOnlySpan<T> Values { get; } = values;

    public T this[int index] => this.Values[index];

    public int Length => this.Values.Length;

    public void Dispose()
    {
        foreach (var value in this.Values)
        {
            value.Dispose();
        }
    }

    public ReadOnlySpan<T>.Enumerator GetEnumerator() =>
        this.Values.GetEnumerator();

    public static implicit operator ReadOnlySpan<T>(DisposableSpan<T> disposibleSequence) => disposibleSequence.Values;
}
