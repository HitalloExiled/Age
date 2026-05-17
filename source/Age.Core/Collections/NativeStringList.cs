using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Extensions;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringList))]
public unsafe partial struct NativeStringList : IDisposable
{
    public byte** Buffer { get; private set; }

    public readonly string this[int index]
    {
        get
        {
            this.CheckIndex(index);

            var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(this.Buffer[index]);

            return Encoding.UTF8.GetString(span);
        }
        set
        {
            this.CheckIndex(index);

            NativeMemory.Free(this.Buffer[index]);

            this.Buffer[index] = MemoryMarshal.CreateUTF8StringBuffer(value);
        }
    }

    public int Capacity
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, this.Count);

            this.Buffer = (byte**)NativeMemory.Realloc(this.Buffer, (nuint)(sizeof(byte*) * value));

            field = value;
        }
    }

    public int Count { get; private set; }

    public readonly bool IsCreated  => this.Buffer != null;
    public readonly bool IsDisposed => this.Buffer == null;

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Count == 0;
    }

    public NativeStringList(int capacity) =>
        this.Capacity = capacity;

    public NativeStringList() : this(4)
    { }

    public NativeStringList(ReadOnlySpan<string?> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.Buffer[i] = MemoryMarshal.CreateUTF8StringBuffer(values[i]);
        }

        this.Count = values.Length;
    }

    private readonly void CheckIndex(int index)
    {
        if (index < 0 || index >= this.Count)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void EnsureCapacity()
    {
        if (this.Count + 1 > this.Capacity)
        {
            this.Capacity = int.Min(this.Capacity == 0 ? 4 : this.Capacity * 2, int.MaxValue);
        }
    }

    public void Add(string? value)
    {
        this.EnsureCapacity();

        this.Buffer[this.Count] = MemoryMarshal.CreateUTF8StringBuffer(value);

        this.Count++;
    }

    public void Clear()
    {
        for (var i = 0; i < this.Count; i++)
        {
            NativeMemory.Free(this.Buffer[i]);
            this.Buffer[i] = null;
        }

        this.Count = 0;
    }

    public void Dispose()
    {
        this.Clear();

        NativeMemory.Free(this.Buffer);
    }

    public readonly Span<string>.Enumerator GetEnumerator() =>
        this.ToArray().AsSpan().GetEnumerator();

    public void Remove(int startIndex, int count = 1)
    {
        this.CheckIndex(startIndex);

        var endIndex = startIndex + count;
        var length   = this.Count - endIndex;

        for (var i = startIndex; i < endIndex; i++)
        {
            NativeMemory.Free(this.Buffer[i]);
        }

        if (length > 0)
        {
            var source      = new Span<nint>(this.Buffer + endIndex,   length);
            var destination = new Span<nint>(this.Buffer + startIndex, length);

            source.CopyTo(destination);
        }

        this.Count = int.Max(this.Count - count, 0);
    }

    public readonly string[] ToArray() =>
        Array.ToUTF8StringArray(this.Buffer, (uint)this.Count);

    public override readonly string ToString() =>
        this.IsCreated ? $"Count = {this.Count}" : "";
}
