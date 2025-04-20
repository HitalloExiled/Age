using System.Runtime.CompilerServices;

namespace Age.Core;

public sealed class StringBuffer
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

                    this.buffer = newBuffer;
                }

                field = value;
            }
        }
    }

    public int Length { get; private set; }

    public bool IsEmpty => this.Length == 0;

    public StringBuffer(int capacity = 32) =>
        this.Capacity = capacity;

    public StringBuffer(scoped ReadOnlySpan<char> value) =>
        this.Append(value);

    private void CheckIndex(int index)
    {
        if (index > this.Length)
        {
            throw new IndexOutOfRangeException();
        }
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

        value.CopyTo(new Span<char>(this.buffer, this.Length, value.Length));

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

    public StringBuffer Concat(StringBuffer other)
    {
        var handler = new StringBuffer(this.Length + other.Length);

        handler.Append(this.AsSpan(), false);
        handler.Append(other.AsSpan(), false);

        return handler;
    }

    public void Clear()
    {
        this.Length = 0;

        this.Modified?.Invoke();
    }

    public bool Equals(scoped ReadOnlySpan<char> value) =>
        value.SequenceEqual(this.AsSpan());

    public void EnsureCapacity(int capacity)
    {
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

    public void Insert(int index, scoped ReadOnlySpan<char> value)
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
            var source      = new Span<char>(this.buffer, index, length);
            var destination = new Span<char>(this.buffer, index + value.Length, length);

            source.CopyTo(destination);
        }

        value.CopyTo(new Span<char>(this.buffer, index, value.Length));

        this.Modified?.Invoke();
    }

    public void Remove(int index, int count = 1)
    {
        this.CheckIndex(index);

        var offset = index + count;
        var length = this.Length - offset;

        if (length > 0)
        {
            var source      = this.buffer.AsSpan(offset, length);
            var destination = this.buffer.AsSpan(index,  length);

            source.CopyTo(destination);
        }

        this.Length = int.Max(this.Length - count, 0);

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
