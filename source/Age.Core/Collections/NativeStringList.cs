using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DebugView))]
public unsafe partial class NativeStringList : Disposable, IEnumerable<string>
{
    private nint* handles;

    public int Capacity
    {
        get;
        set
        {
            if (value == 0)
            {
                NativeMemory.Free(this.handles);

                this.Count = 0;
            }
            else
            {
                this.handles = (nint*)NativeMemory.Realloc(this.handles, (uint)(sizeof(nint) * value));

                if (value < this.Count)
                {
                    this.Count = value;
                }
            }

            field = value;
        }
    }

    public int Count { get; private set; }

    public NativeStringList(int capacity = 0)
    {
        if (capacity > 0)
        {
            this.Capacity = capacity;
        }
    }

    public NativeStringList(ReadOnlySpan<string?> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.handles[i] = Marshal.StringToHGlobalAnsi(values[i]);
        }

        this.Count = values.Length;
    }

    public string this[int index]
    {
        get
        {
            this.ThrowIfDisposed();
            this.CheckIndex(index);

            return Marshal.PtrToStringAnsi(this.handles[index])!;
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

    protected override void OnDisposed(bool disposing) =>
        this.Clear();

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    public void Add(string? value)
    {
        this.ThrowIfDisposed();
        this.EnsureCapacity();

        this.handles[this.Count] = Marshal.StringToHGlobalAnsi(value);

        this.Count++;
    }

    public Span<nint> AsSpan() =>
        new(this.handles, this.Count);

    public ReadOnlySpan<nint> AsReadOnlySpan() =>
        new(this.handles, this.Count);

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

    public void Remove(int startIndex, int count = 1)
    {
        this.ThrowIfDisposed();
        this.CheckIndex(startIndex);

        var endIndex = startIndex + count;
        var length   = this.Count - endIndex;

        for (var i = startIndex; i < endIndex; i++)
        {
            NativeMemory.Free((void*)this.handles[i]);
        }

        if (length > 0)
        {
            var source      = new Span<nint>(this.handles + endIndex,   length);
            var destination = new Span<nint>(this.handles + startIndex, length);

            source.CopyTo(destination);
        }

        this.Count = int.Max(this.Count - count, 0);
    }

    public IEnumerator<string> GetEnumerator()
    {
        var span = this.AsSpan().ToArray();

        for (var i = 0; i < this.Count; i++)
        {
            yield return Marshal.PtrToStringAnsi(span[i])!;
        }
    }
}
