using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(KeyedList<,>.DebugView))]
public unsafe partial class KeyedList<TKey, TValue>(int capacity = 0) : IEnumerable<KeyValuePair<TKey, TValue>>
where TKey   : unmanaged, Enum
where TValue : notnull
{
    private static readonly int maxSize = sizeof(TKey) * 8;

    private TValue[] values = [];
    private TKey     keys;

    public int Capacity
    {
        get;
        set
        {
            if (field != value)
            {
                if (value > this.values.Length)
                {
                    if (value > int.MaxValue)
                    {
                        throw new InvalidOperationException($"Requested capacity ({value}) exceeds the maximum allowed size for {typeof(TKey).Name} ({maxSize}).");
                    }

                    var newBuffer = ArrayPool<TValue>.Shared.Rent(value);

                    this.values.AsSpan(0, this.Count).CopyTo(newBuffer);

                    ArrayPool<TValue>.Shared.Return(this.values, true);

                    this.values = newBuffer;
                }

                field = value;
            }
        }
    } = capacity;

    public int Count { get; private set; }

    public TValue this[TKey key]
    {
        get => this.Get(key);
        set => this.Add(key, value);
    }

    private static void ThrowIfNotSingleFlag(TKey key)
    {
        if (!key.HasSingleBit())
        {
            throw new InvalidOperationException("Removing multiple flags at once is not supported. Please provide a key with a single flag set.");
        }
    }

    private static TKey ToKey(long value) =>
        sizeof(TKey) switch
        {
            1 => Unsafe.As<byte,  TKey>(ref Unsafe.As<long, byte>(ref value)),
            2 => Unsafe.As<short, TKey>(ref Unsafe.As<long, short>(ref value)),
            4 => Unsafe.As<int,   TKey>(ref Unsafe.As<long, int>(ref value)),
            8 => Unsafe.As<long,  TKey>(ref value),
            _ => throw new NotSupportedException($"Enum with size of {maxSize} is not supported")
        };

    private void EnsureCapacity()
    {
        if (this.Count + 1 > this.Capacity)
        {
            this.Capacity = int.Min(this.Capacity == 0 ? 4 : this.Count * 2, int.MaxValue);
        }
    }

    private int GetIndex(long key)
    {
        var mask = key - 1;

        return (int)long.PopCount(this.keys.GetValue() & mask);
    }

    private int GetIndex(TKey key) =>
        this.GetIndex(key.GetValue());

    private void Insert(int index, TValue item)
    {
        if (index > this.Count)
        {
            throw new IndexOutOfRangeException();
        }

        this.EnsureCapacity();

        this.Count++;

        var length = this.Count - index - 1;

        if (length > 0)
        {
            var source      = new Span<TValue>(this.values, index, length);
            var destination = new Span<TValue>(this.values, index + 1, length);

            source.CopyTo(destination);
        }

        this.values[index] = item;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    public void Add(TKey key, TValue value)
    {
        ThrowIfNotSingleFlag(key);

        var index = this.GetIndex(key);

        if (!this.keys.HasFlags(key))
        {
            this.keys = this.keys.CombineFlag(key);
            this.EnsureCapacity();
            this.Insert(index, value);
        }
        else
        {
            this.values[index] = value;
        }
    }

    public Span<TValue> AsSpan() =>
        new(this.values, 0, this.Count);

    public ReadOnlySpan<TValue> AsReadOnlySpan() =>
        new(this.values, 0, this.Count);

    public bool ContainsAnyKey(TKey key) =>
        this.keys.HasAnyFlag(key);

    public bool ContainsKey(TKey key) =>
        this.keys.HasFlags(key);

    public void EnsureCapacity(int capacity)
    {
        if (capacity > this.Capacity)
        {
            this.Capacity = capacity;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        var keys = this.keys.GetValue();

        for (var i = 0; i < maxSize; i++)
        {
            var flag = 1L << i;

            if ((keys & flag) != 0)
            {
                yield return new(ToKey(flag), this.values[this.GetIndex(flag)]);
            }
        }
    }

    public TValue Get(TKey key) =>
        this.TryGet(key, out var value) ? value : throw new InvalidOperationException($"The key '{key}' does not exist in the collection.");

    public bool Remove(TKey key)
    {
        ThrowIfNotSingleFlag(key);

        if (!this.keys.HasFlags(key))
        {
            return false;
        }

        var index = this.GetIndex(key);

        var endIndex = index + 1;
        var length   = this.Count - endIndex;

        if (length > 0)
        {
            var source      = new Span<TValue>(this.values, endIndex, length);
            var destination = new Span<TValue>(this.values, index,    length);

            source.CopyTo(destination);
        }

        this.Count              = int.Max(this.Count - 1, 0);
        this.keys               = this.keys.RemoveFlag(key);
        this.values[this.Count] = default!;

        return true;
    }

    public bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        ThrowIfNotSingleFlag(key);

        if (this.keys.HasFlags(key))
        {
            value = this.values[this.GetIndex(key)];

            return true;
        }

        value = default;
        return false;
    }
}
