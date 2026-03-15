using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringList))]
public partial class NativeStringList : Disposable
{
    private UnsafeStringListBuffer unsafeBuffer;

    public string this[int index]
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer[index];
        }
        set
        {
            this.ThrowIfDisposed();

            this.unsafeBuffer[index] = value;
        }
    }

    public int Capacity
    {
        get => this.unsafeBuffer.Capacity;
        set
        {
            this.ThrowIfDisposed();

            this.unsafeBuffer.Capacity = value;
        }
    }

    public unsafe byte** Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public int  Count   => this.unsafeBuffer.Count;
    public bool IsEmpty => this.unsafeBuffer.IsEmpty;

    public NativeStringList(int capacity = 0) =>
        this.unsafeBuffer = new(capacity);

    public NativeStringList(ReadOnlySpan<string?> values) =>
        this.unsafeBuffer = new(values);

    protected override void OnDisposed(bool disposing) =>
        this.unsafeBuffer.Dispose();

    public void Add(string? value)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Add(value);
    }

    public void Clear()
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Clear();
    }

    public Span<string>.Enumerator GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public void Remove(int startIndex, int count = 1)
    {
        this.ThrowIfDisposed();

        this.unsafeBuffer.Remove(startIndex, count);
    }

    public string[] ToArray()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.ToArray();
    }
}
