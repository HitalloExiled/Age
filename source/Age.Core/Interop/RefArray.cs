using System.Runtime.InteropServices;

namespace Age.Core.Interop;

public unsafe ref struct RefArray<T>(int lenght = 1) : IDisposable where T : unmanaged
{
    private T* buffer = (T*)NativeMemory.Alloc((uint)(sizeof(T) * lenght));

    public readonly int Length => lenght;

    public readonly T this[int index]
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

    public RefArray(scoped ReadOnlySpan<T> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.buffer[i] = values[i];
        }
    }

    private readonly void CheckIndex(int index)
    {
        if (index >= lenght)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public readonly Span<T> AsSpan() => new(this.buffer, lenght);

    public void Dispose()
    {
        NativeMemory.Free(this.buffer);
        this.buffer = default;
    }

    public static implicit operator Span<T>(in RefArray<T> refArray) =>
        refArray.AsSpan();
}
