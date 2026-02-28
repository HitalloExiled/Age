using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringArray))]
public unsafe partial class NativeStringArray : Disposable
{
    private UnsafeStringArrayBuffer unsafeBuffer;

    public string? this[int index]
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

    public byte** Buffer
    {
        get
        {
            this.ThrowIfDisposed();

            return this.unsafeBuffer.Buffer;
        }
    }

    public bool IsEmpty => this.unsafeBuffer.IsEmpty;
    public int  Length  => this.unsafeBuffer.Length;

    public NativeStringArray(int size) =>
        this.unsafeBuffer = new(size);

    public NativeStringArray(ReadOnlySpan<string> values) =>
        this.unsafeBuffer = new(values);

    protected override void OnDisposed(bool disposing) =>
        this.unsafeBuffer.Dispose();

    public Span<string>.Enumerator GetEnumerator()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.GetEnumerator();
    }

    public string[] ToArray()
    {
        this.ThrowIfDisposed();

        return this.unsafeBuffer.ToArray();
    }

    public static implicit operator byte**(NativeStringArray value) => value.Buffer;
}
