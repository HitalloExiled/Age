using Age.Core.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Collections;

internal unsafe struct UnsafeBuffer
{
    public void* Pointer;

    public int Length;
    public int Stride;
    public int Dynamic;

    internal readonly ReadOnlySpan<byte> Bytes      => new(this.Pointer, this.Length * this.Stride);
    internal readonly int                BytesCount => this.Length * this.Stride;

    public static void Copy(UnsafeBuffer source, int sourceIndex, UnsafeBuffer destination, int destinationIndex, int count)
    {
        Debug.Assert(source.Pointer != null);
        Debug.Assert(source.Pointer != destination.Pointer);
        Debug.Assert(source.Stride == destination.Stride);
        Debug.Assert(source.Stride > 0);
        Debug.Assert(destination.Pointer != null);
        Debug.Assert(source.Length >= sourceIndex + count);
        Debug.Assert(destination.Length >= destinationIndex + count);

        NativeMemory.Copy((byte*)destination.Pointer + (destinationIndex * source.Stride), (byte*)source.Pointer + (sourceIndex * source.Stride), (nuint)(count * source.Stride));
    }

    public static void CopyFrom<T>(void* source, int sourceIndex, UnsafeBuffer destination, int destinationIndex, int count) where T : unmanaged
    {
        Debug.Assert(source != null);
        Debug.Assert(source != destination.Pointer);
        Debug.Assert(destination.Pointer != null);
        Debug.Assert(destination.Stride > 0);
        Debug.Assert(destination.Length >= destinationIndex + count);

        NativeMemory.Copy((T*)destination.Pointer + destinationIndex, (T*)source + sourceIndex, (nuint)(count * sizeof(T)));
    }

    public static void CopyTo<T>(UnsafeBuffer source, int sourceIndex, void* destination, int destinationIndex, int count) where T : unmanaged
    {
        Debug.Assert(source.Pointer != null);
        Debug.Assert(source.Pointer != destination);
        Debug.Assert(source.Stride > 0);
        Debug.Assert(destination != null);
        Debug.Assert(source.Length >= sourceIndex + count);

        NativeMemory.Copy((T*)destination + destinationIndex, (T*)source.Pointer + sourceIndex, (nuint)(count * sizeof(T)));
    }

    public static void Clear(UnsafeBuffer* buffer) =>
        NativeMemory.Clear(buffer->Pointer, (nuint)(buffer->Length * buffer->Stride));

    public static void Free(UnsafeBuffer* buffer)
    {
        if (buffer == null)
        {
            return;
        }

        if (buffer->Dynamic == 0)
        {
            throw new InvalidOperationException("Can't free a fixed buffer");
        }

        Debug.Assert(buffer->Pointer != null);

        NativeMemory.AlignedFree(buffer->Pointer);

        *buffer = default;
    }

    public static int IndexOf<T>(UnsafeBuffer buffer, T item, int startIndex, int count)
    where T : unmanaged, IEquatable<T>
    {
        Debug.Assert(buffer.Pointer != null);

        if (buffer.Length == 0)
        {
            return -1;
        }

        Debug.Assert(startIndex > -1);
        Debug.Assert(count > -1);
        Debug.Assert(buffer.Length >= startIndex + count);
        Debug.Assert(buffer.Stride == sizeof(T));

        return new Span<T>((T*)buffer.Pointer + startIndex, count).IndexOf(item);
    }

    public static void InitDynamic<T>(UnsafeBuffer* buffer, int length) where T : unmanaged =>
        InitDynamic(buffer, length, sizeof(T));

    public static void InitDynamic(UnsafeBuffer* buffer, int length, int stride)
    {
        Debug.Assert(buffer  != null);

        Debug.Assert(length > 0);
        Debug.Assert(stride > 0);

        buffer->Pointer = NativeMemory.AlignedAllocZeroed((nuint)(length * stride), (nuint)Marshal.GetAlignment(stride));
        buffer->Length  = length;
        buffer->Stride  = stride;
        buffer->Dynamic = 1;
    }

    public static void InitFixed<T>(UnsafeBuffer* buffer, void* pointer, int length) where T : unmanaged =>
        InitFixed(buffer, pointer, length, sizeof(T));

    public static void InitFixed(UnsafeBuffer* buffer, void* pointer, int length, int stride)
    {
        Debug.Assert(buffer  != null);
        Debug.Assert(pointer != null);

        Debug.Assert(length > 0);
        Debug.Assert(stride > 0);

        Debug.Assert(((nint)pointer).ToInt64() % Marshal.GetAlignment(stride) == 0);

        buffer->Pointer = pointer;
        buffer->Length  = length;
        buffer->Stride  = stride;
        buffer->Dynamic = 0;
    }

    public static int LastIndexOf<T>(UnsafeBuffer buffer, T item, int startIndex, int count)
    where T : unmanaged, IEquatable<T>
    {
        Debug.Assert(startIndex > -1);
        Debug.Assert(count > 0);
        Debug.Assert(buffer.Length >= startIndex);
        Debug.Assert(startIndex + 1 >= count);
        Debug.Assert(buffer.Stride == sizeof(T));

        return new Span<T>((T*)buffer.Pointer + startIndex, count).LastIndexOf(item);
    }

    public static void Move(UnsafeBuffer buffer, int fromIndex, int toIndex, int count)
    {
        Debug.Assert(buffer.Pointer != null);

        var size = count * buffer.Stride;

        var from = fromIndex * buffer.Stride;
        var to   = toIndex   * buffer.Stride;

        var source      = new Span<byte>((byte*)buffer.Pointer + from, size);
        var destination = new Span<byte>((byte*)buffer.Pointer + to, size);

        source.CopyTo(destination);
    }

    public static void ResizeDynamic(UnsafeBuffer* buffer, int length, bool zeroed)
    {
        Debug.Assert(buffer != null);
        Debug.Assert(buffer->Dynamic == 1);
        Debug.Assert(buffer->Stride > 0);

        var alignment = Marshal.GetAlignment(buffer->Stride);

        var bytesCount = length * buffer->Stride;
        var newBuffer  = zeroed
            ? NativeMemory.AlignedAllocZeroed((nuint)bytesCount, (nuint)alignment)
            : NativeMemory.AlignedAlloc((nuint)bytesCount, (nuint)alignment);

        var source = buffer->BytesCount < bytesCount
            ? buffer->Bytes
            : buffer->Bytes[..bytesCount];

        var destination = new Span<byte>(newBuffer, bytesCount);

        source.CopyTo(destination);

        NativeMemory.AlignedFree(buffer->Pointer);

        buffer->Pointer = newBuffer;

        buffer->Length = length;
    }

    public readonly void Clear() =>
        NativeMemory.Clear(this.Pointer, (nuint)(this.Length * this.Stride));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void* Element(int index)
    {
        Debug.Assert(index <= this.Length);
        return (byte*)this.Pointer + (index * this.Stride);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T* Element<T>(int index) where T : unmanaged
    {
        Debug.Assert(index <= this.Length);

        return (T*)((byte*)this.Pointer + (index * this.Stride));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Element(void* bufferPtr, int index, int stride) =>
        (byte*)bufferPtr + (index * stride);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void* Element(int index, int offset)
    {
        Debug.Assert(index <= this.Length);

        return (byte*)this.Pointer + (index * this.Stride) + offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T ElementRef<T>(int index) where T : unmanaged =>
        ref Unsafe.AsRef<T>(this.Element<T>(index));
}
