using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Extensions;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeStringArray))]
public unsafe partial struct NativeStringArray(int size) : IDisposable
{
    public byte** Buffer { get; private set; } = (byte**)NativeMemory.AllocZeroed((uint)(sizeof(byte*) * size));

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

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Length == 0;
    }

    public readonly bool IsCreated  => this.Buffer != null;
    public readonly bool IsDisposed => this.Buffer == null;

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => size;
    }

    public NativeStringArray(ReadOnlySpan<string> values) : this(values.Length)
    {
        for (var i = 0; i < values.Length; i++)
        {
            this.Buffer[i] = MemoryMarshal.CreateUTF8StringBuffer(values[i]);
        }
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
        if (this.Buffer == null)
        {
            return;
        }

        for (var i = 0; i < this.Length; i++)
        {
            NativeMemory.Free(this.Buffer[i]);
        }

        NativeMemory.Free(this.Buffer);

        this.Buffer = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<string>.Enumerator GetEnumerator() =>
        this.ToArray().AsSpan().GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string[] ToArray() =>
        Array.ToUTF8StringArray(this.Buffer, (uint)this.Length);

    public override readonly string ToString() =>
        this.IsCreated ? $"Length = {this.Length}" : "";

    public static implicit operator byte**(NativeStringArray value) => value.Buffer;
}
