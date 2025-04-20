using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Core;

public unsafe class NativeStringList : Disposable, IEnumerable<string?>
{
    private nint[] handles = [];

    public int Capacity
    {
        get;
        set
        {
            if (field != value)
            {
                if (this.Count > value)
                {
                    for (var i = value; i < this.Count; i++)
                    {
                        Marshal.FreeHGlobal(this.handles[i]);
                    }

                    this.Count = value;
                }

                Array.Resize(ref this.handles, value);

                field = value;
            }
        }
    }

    public int Count { get; private set; }

    public NativeStringList(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.handles = new nint[capacity];

            this.Capacity = capacity;
        }
    }

    public NativeStringList(scoped ReadOnlySpan<string?> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.handles[i] = Marshal.StringToHGlobalAnsi(values[i]);
        }

        this.Count = values.Length;
    }

    public string? this[int index]
    {
        get
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            return Marshal.PtrToStringAnsi(this.handles[index]);
        }
        set
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            Marshal.FreeHGlobal(this.handles[index]);

            this.handles[index] = Marshal.StringToHGlobalAnsi(value);
        }
    }

    private void CheckIndex(int index)
    {
        if (index >= this.Count)
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

    private void ShiftElements(int sourceIndex, int destinationIndex, int elementCount)
    {
        if (elementCount <= 0)
        {
            return;
        }

        Array.Copy(this.handles, sourceIndex, this.handles, destinationIndex, elementCount);
    }

    protected override void OnDisposed(bool disposing) =>
        this.Clear();

    public void Add(string? value)
    {
        this.ThrowIfDisposed();
        this.EnsureCapacity();

        this.handles[this.Count] = Marshal.StringToHGlobalAnsi(value);

        this.Count++;
    }

    public Span<nint> AsSpan() => this.handles.AsSpan();

    public void Clear()
    {
        this.ThrowIfDisposed();

        for (var i = 0; i < this.Count; i++)
        {
            Marshal.FreeHGlobal(this.handles[i]);
        }

        this.Count = 0;
    }

    public nint GetHandle(int index)
    {
        this.CheckIndex(index);
        this.ThrowIfDisposed();

        return this.handles[index];
    }

    public void Remove(int startIndex, int length = 1)
    {
        this.CheckIndex(startIndex);
        this.ThrowIfDisposed();

        var endIndex = startIndex + length;

        var shiftCount = this.Count - endIndex;

        for (var i = startIndex; i < endIndex; i++)
        {
            Marshal.FreeHGlobal(this.handles[i]);
        }

        if (shiftCount > 0)
        {
            this.ShiftElements(endIndex, startIndex, shiftCount);
        }

        this.Count = int.Max(this.Count - length, 0);
    }

    public IEnumerator<string?> GetEnumerator()
    {
        var span = this.AsSpan().ToArray();

        for (var i = 0; i < this.Count; i++)
        {
            yield return Marshal.PtrToStringAnsi(span[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();
}
