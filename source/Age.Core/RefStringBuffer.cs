using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core;

public unsafe ref struct RefStringBuffer : IDisposable
{
    private char* buffer;

    public char this[int index]
    {
        get
        {
            this.CheckIndex(index);

            return this.buffer[index];
        }
        set
        {
            this.CheckIndex(index);

            this.buffer[index] = value;
        }
    }

    public int Capacity
    {
        readonly get;
        set
        {
            if (value == 0)
            {
                NativeMemory.Free(this.buffer);

                this.Length = 0;
            }
            else
            {
                this.buffer = (char*)NativeMemory.Realloc(this.buffer, (uint)(sizeof(char) * value));

                if (value < this.Length)
                {
                    this.Length = value;
                }
            }

            field = value;
        }
    }

    public int Length { get; private set; }

    public readonly bool IsEmpty => this.Length == 0;

    public RefStringBuffer(int capacity) =>
        this.Capacity = capacity;

    public RefStringBuffer(ReadOnlySpan<char> value) =>
        this.Append(value);

    private readonly void CheckIndex(int index)
    {
        if (index > this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public readonly Span<char> AsSpan() =>
        new(this.buffer, this.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);

        value.CopyTo(new Span<char>(this.buffer + this.Length, value.Length));

        this.Length += value.Length;
    }

    public void AppendLine(ReadOnlySpan<char> value)
    {
        this.Append(value);
        this.Append(['\n']);
    }

    public readonly RefStringBuffer Concat(in RefStringBuffer other)
    {
        var handler = new RefStringBuffer(this.Length + other.Length);

        handler.Append(this.AsSpan());
        handler.Append(other.AsSpan());

        return handler;
    }

    public void Clear() =>
        this.Length = 0;

    public void Dispose()
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
        this.Length = 0;
    }

    public readonly bool Equals(ReadOnlySpan<char> value) =>
        value.SequenceEqual(this.AsSpan());

    public void EnsureCapacity(int capacity)
    {
        if (capacity > this.Capacity)
        {
            this.Capacity = int.Min(int.Max(this.Capacity * 2, capacity), int.MaxValue);
        }
    }

    public readonly Span<char>.Enumerator GetEnumerator() =>
        this.AsSpan().GetEnumerator();

    public void Insert(int index, ReadOnlySpan<char> value)
    {
        this.CheckIndex(index);

        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);

        this.Length += value.Length;

        var length = this.Length - index - value.Length;

        if (length > 0)
        {
            var source      = new Span<char>(this.buffer + index, length);
            var destination = new Span<char>(this.buffer + index + value.Length, length);

            source.CopyTo(destination);
        }

        value.CopyTo(new Span<char>(this.buffer + index, value.Length));
    }

    public void Remove(int index, int count)
    {
        this.CheckIndex(index);

        var offset = index + count;
        var length = this.Length - offset;

        if (length > 0)
        {
            var source      = new Span<char>(this.buffer + offset, length);
            var destination = new Span<char>(this.buffer + index,  length);

            source.CopyTo(destination);
        }

        this.Length = int.Max(this.Length - count, 0);
    }

    public void Set(ReadOnlySpan<char> value)
    {
        if (this.Equals(value))
        {
            return;
        }

        this.Length = 0;
        this.Append(value);
    }

    public override readonly string ToString() =>
        new(this.buffer, 0, this.Length);

    public readonly ReadOnlySpan<char> Substring(int start, int length) =>
        this.AsSpan().Slice(start, length);
}
