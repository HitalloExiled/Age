using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(InlineList8<>.DebugView))]
[CollectionBuilder(typeof(InlineListBuilder), nameof(InlineListBuilder.InlineList8))]
public partial struct InlineList8<T> : IEquatable<InlineList8<T>>
{
    private const int CAPACITY = 8;

    private Buffer buffer;

    public int Length { get; private set; }

    public T this[int index]
    {
        readonly get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Length);

            return this.buffer[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Length);

            this.buffer[index] = value;
        }
    }

    public T this[Index index]
    {
        readonly get => this.buffer[index.GetOffset(this.Length)];
        set => this.buffer[index.GetOffset(this.Length)] = value;
    }

    public InlineList8(int size)
    {
        InlineListException.ThrowsIfExceeds(size, CAPACITY);

        this.Length = size;
    }

    public InlineList8(params ReadOnlySpan<T> elements) : this(elements.Length)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(elements.Length, CAPACITY);

        elements.CopyTo(this.buffer);
    }

    public void Add(T item)
    {
        InlineListException.ThrowsIfExceeds(this.Length + 1, CAPACITY);

        this.buffer[this.Length++] = item;
    }

    public void Remove(T item) =>
        this.RemoveAt(this.AsSpan().IndexOf(item), 1);

    public void RemoveAt(int startIndex, int count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, this.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan(count - startIndex, 0);

        var endIndex = startIndex + count;
        var length   = this.Length - endIndex;

        var span = this.AsSpan();

        if (length > 0)
        {
            var source      = span.Slice(endIndex, length);
            var destination = span.Slice(startIndex, length);

            source.CopyTo(destination);
        }

        this.Length = int.Max(this.Length - count, 0);
    }

    public Span<T> AsSpan() =>
        MemoryMarshal.CreateSpan(ref this.buffer[0], this.Length);

    public void CopyTo(ref InlineList8<T> other)
    {
        MemoryMarshal.CreateSpan(ref this.buffer[0], CAPACITY).CopyTo(MemoryMarshal.CreateSpan(ref other.buffer[0], CAPACITY));

        other.Length = this.Length;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is InlineList8<T> other && this.Equals(other);

    public bool Equals(InlineList8<T> other) =>
        this.AsSpan().SequenceEqual(other);

    public Span<T>.Enumerator GetEnumerator() =>
        this.AsSpan().GetEnumerator();

    public override int GetHashCode() =>
        Span<T>.CombineHashCode(this.AsSpan());

    public static implicit operator InlineList8<T>(Span<T> elements) => new(elements);
    public static implicit operator Span<T>(InlineList8<T> inlineArray) => inlineArray.AsSpan();

    public static bool operator ==(InlineList8<T> left, InlineList8<T> right) => left.Equals(right);
    public static bool operator !=(InlineList8<T> left, InlineList8<T> right) => !(left == right);
}
