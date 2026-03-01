using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Extensions;

namespace Age.Core.Collections;

public unsafe struct UnsafeStringArrayBuffer(int size)
{
    public byte** Buffer { get; private set; } = (byte**)NativeMemory.Alloc((uint)(sizeof(byte*) * size));

    public int Length { get; }

    public readonly string? this[int index]
    {
        get
        {
            this.CheckIndex(index);

            return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(this.Buffer[index]));
        }
        set
        {
            this.CheckIndex(index);

            NativeMemory.Free(this.Buffer[index]);

            this.Buffer[index] = MemoryMarshal.CreateUTF8StringBuffer(value);
        }
    }

    public readonly bool IsEmpty => this.Length == 0;

    public UnsafeStringArrayBuffer(ReadOnlySpan<string> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.Buffer[i] = MemoryMarshal.CreateUTF8StringBuffer(values[i]);
        }

        this.Length = values.Length;
    }

    private readonly void CheckIndex(int index)
    {
        if (index < 0 || index >= this.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Dispose()
    {
        for (var i = 0; i < this.Length; i++)
        {
            NativeMemory.Free(this.Buffer[i]);
        }

        NativeMemory.Free(this.Buffer);

        this.Buffer = null;
    }

    public readonly Span<string>.Enumerator GetEnumerator() =>
        this.ToArray().AsSpan().GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string[] ToArray() =>
        Array.ToUTF8StringArray(this.Buffer, (uint)this.Length);
}
