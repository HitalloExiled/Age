using System.Runtime.CompilerServices;

namespace Age.Core;

public sealed class StringHandler
{
    public event Action? Changed;
    public event Action? Modified;

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

                    var newBuffer = GC.AllocateUninitializedArray<char>(value);

                    this.buffer.AsSpan(0, this.Length).CopyTo(newBuffer);

                    // ArrayPool<char>.Shared.Return(this.buffer);

                    this.buffer = newBuffer;
                }

                field = value;
            }
        }
    }

    public int Length { get; private set; }

    public bool IsEmpty => this.Length == 0;

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

    private void EnsureCapacity(int capacity)
    {
        if (capacity > this.Capacity)
        {
            this.Capacity = int.Min(int.Max(this.Capacity * 2, capacity), int.MaxValue);
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

    public ReadOnlySpan<char> AsSpan() =>
        new(this.buffer, 0, this.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Append(scoped ReadOnlySpan<char> value, bool dispatchEvent)
    {
        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);

        value.CopyTo(this.buffer.AsSpan(this.Length));

        this.Length += value.Length;

        if (dispatchEvent)
        {
            this.Modified?.Invoke();
        }
    }

    public void Append(scoped ReadOnlySpan<char> value) =>
        this.Append(value, true);

    public void AppendLine(scoped ReadOnlySpan<char> value)
    {
        this.Append(value, false);
        this.Append(['\n'], false);

        this.Modified?.Invoke();
    }

    public StringHandler Concat(StringHandler other)
    {
        var handler = new StringHandler(this.Length + other.Length);

        handler.Append(this.buffer.AsSpan(0, this.Length), false);
        handler.Append(other.buffer.AsSpan(0, other.Length), false);

        return handler;
    }

    public void Clear()
    {
        this.Length = 0;

        this.Modified?.Invoke();
    }

    public bool Equals(ReadOnlySpan<char> value) =>
        value.SequenceEqual(this.AsSpan());

    public IEnumerator<char> GetEnumerator()
    {
        for (var i = 0; i < this.Length; i++)
        {
            yield return this.buffer[i];
        }
    }

    public void Insert(scoped ReadOnlySpan<char> value, int index)
    {
        this.CheckIndex(index);

        if (value.IsEmpty)
        {
            return;
        }

        this.EnsureCapacity(this.Length + value.Length);
        this.ShiftElements(index, index + value.Length, this.Length - index);

        value.CopyTo(this.buffer.AsSpan(index));

        this.Length += value.Length;

        this.Modified?.Invoke();
    }

    public void Remove(int index, int length = 1)
    {
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

        this.Modified?.Invoke();
    }

    public void Set(scoped ReadOnlySpan<char> value)
    {
        if (this.Equals(value))
        {
            return;
        }

        this.Length = 0;
        this.Append(value, true);

        this.Changed?.Invoke();
    }

    public override string ToString() =>
        new(this.buffer, 0, this.Length);

    public ReadOnlySpan<char> Substring(int start, int length) =>
        this.AsSpan().Slice(start, length);
}
