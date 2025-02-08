using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core;

public sealed class StringHandler : Disposable, IEnumerable<char>
{
    private char[] buffer = [];

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
        get => field;
        set
        {
            if (field != value)
            {
                if (value > this.buffer.Length)
                {
                    if (value > int.MaxValue)
                    {
                        throw new InvalidOperationException("Capacity exceeds maximum allowed size");
                    }

                    var newBuffer = ArrayPool<char>.Shared.Rent(value);

                    this.buffer.AsSpan(0, this.Length).CopyTo(newBuffer);

                    ArrayPool<char>.Shared.Return(this.buffer);

                    this.buffer = newBuffer;
                }

                field = value;
            }
        }
    }

    public int Length { get; private set; }

    public StringHandler(int capacity = 32) =>
        this.Capacity = capacity;

    public StringHandler(scoped ReadOnlySpan<char> value) =>
        this.Append(value);

    private void CheckIndex(int index)
    {
        if (index > this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void ShiftElements(int sourceIndex, int destinationIndex, int elementCount)
    {
        if (elementCount <= 0)
        {
            return;
        }

        Array.Copy(this.buffer, sourceIndex, this.buffer, destinationIndex, elementCount);
    }

    protected override void Disposed(bool disposing) =>
        ArrayPool<char>.Shared.Return(this.buffer);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public ReadOnlySpan<char> AsSpan() =>
        new(this.buffer, 0, this.Length);

    public void Append(scoped ReadOnlySpan<char> value)
    {
        this.ThrowIfDisposed();

        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);

        value.CopyTo(this.buffer.AsSpan(this.Length));

        this.Length += value.Length;
    }

    public void AppendLine(scoped ReadOnlySpan<char> value)
    {
        this.Append(value);
        this.Append(['\n']);
    }

    public StringHandler Concat(StringHandler other)
    {
        this.ThrowIfDisposed();
        other.ThrowIfDisposed();

        var handler = new StringHandler(this.Length + other.Length);

        handler.Append(this.buffer.AsSpan(0, this.Length));
        handler.Append(other.buffer.AsSpan(0, other.Length));

        return handler;
    }

    public void Clear()
    {
        this.ThrowIfDisposed();
        this.Length = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        this.ThrowIfDisposed();
        if (capacity > this.Capacity)
        {
            this.Capacity = int.Min(int.Max(this.Capacity * 2, capacity), int.MaxValue);
        }
    }

    public IEnumerator<char> GetEnumerator()
    {
        for (var i = 0; i < this.Length; i++)
        {
            yield return this.buffer[i];
        }
    }

    public void Insert(scoped ReadOnlySpan<char> value, int index)
    {
        this.ThrowIfDisposed();
        this.CheckIndex(index);

        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);
        this.ShiftElements(index, index + value.Length, this.Length - index);

        value.CopyTo(this.buffer.AsSpan(index));

        this.Length += value.Length;
    }

    public void Remove(int index, int length = 1)
    {
        this.ThrowIfDisposed();
        this.CheckIndex(index);

        if (length == 0)
        {
            return;
        }

        var endIndex   = index + length;
        var shiftCount = this.Length - endIndex;

        if (shiftCount > 0)
        {
            this.ShiftElements(endIndex, index, shiftCount);

        }

        this.Length = int.Max(this.Length - length, 0);
    }

    public void Set(scoped ReadOnlySpan<char> value)
    {
        this.ThrowIfDisposed();
        this.Clear();
        this.Append(value);
    }

    public override string ToString() =>
        new(this.buffer, 0, this.Length);
}
