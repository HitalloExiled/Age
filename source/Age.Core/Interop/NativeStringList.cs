using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Core.Interop;

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

    public NativeStringList(Span<string?> values) : this(values.Length)
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

    protected override void Disposed(bool disposing) =>
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

    public void Remove(int index)
    {
        this.CheckIndex(index);
        this.ThrowIfDisposed();

        this.Count -= 1;

        for (var i = index; i < this.Count; i++)
        {
            this.handles[i] = this.handles[i + 1];
        }

        Marshal.FreeHGlobal(this.Count);

        this.handles[this.Count] = default;
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
