using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(InlineList4<>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.InlineList4))]
public partial struct InlineList4<T> : IEquatable<InlineList4<T>>
{
    private const int CAPACITY = 4;

    private InlineArray4<T> buffer;

    private int count;

    public readonly int Count => this.count;

    public T this[int index]
    {
        readonly get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Count);

            return this.buffer[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Count);

            this.buffer[index] = value;
        }
    }

    public T this[Index index]
    {
        readonly get => this.buffer[index.GetOffset(this.Count)];
        set => this.buffer[index.GetOffset(this.Count)] = value;
    }

    public InlineList4(int size)
    {
        InlineListException.ThrowsIfExceeds(size, CAPACITY);

        this.count = size;
    }

    public InlineList4(params ReadOnlySpan<T> elements) : this(elements.Length) =>
        elements.CopyTo(this.buffer);

    public void Add(T item) =>
        InlineListHelper<T>.Add(this.buffer, item, CAPACITY, ref this.count);

    public Span<T> AsSpan() =>
        MemoryMarshal.CreateSpan(ref this.buffer[0], this.Count);

    public void Clear() =>
        InlineListHelper<T>.Clear(this.buffer, ref this.count);

    public readonly void CopyTo(ref InlineList4<T> other) =>
        InlineListHelper<T>.CopyTo(this, other, ref other.count);

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is InlineList4<T> other && this.Equals(other);

    public bool Equals(InlineList4<T> other) =>
        this.AsSpan().SequenceEqual(other);

    public Span<T>.Enumerator GetEnumerator() =>
        this.AsSpan().GetEnumerator();

    public override int GetHashCode() =>
        Span<T>.CombineHashCode(this.AsSpan());

    public void Remove(T item) =>
        InlineListHelper<T>.Remove(this.buffer, item, ref this.count);

    public void RemoveAt(int index) =>
        InlineListHelper<T>.RemoveAt(this.buffer, index, ref this.count);

    public void RemoveAt(int startIndex, int count) =>
        InlineListHelper<T>.RemoveAt(this.buffer, startIndex, count, ref this.count);

    public static implicit operator InlineList4<T>(Span<T> elements) => new(elements);
    public static implicit operator Span<T>(InlineList4<T> inlineArray) => inlineArray.AsSpan();

    public static bool operator ==(InlineList4<T> left, InlineList4<T> right) => left.Equals(right);
    public static bool operator !=(InlineList4<T> left, InlineList4<T> right) => !(left == right);
}
