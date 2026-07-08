using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(NativeMemory)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* Alloc<T>() where T : unmanaged =>
            (T*)NativeMemory.Alloc((nuint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* Alloc<T>(nuint count) where T : unmanaged =>
            (T*)NativeMemory.Alloc(count, (nuint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* Alloc<T>(T value) where T : unmanaged
        {
            var pointer = (T*)NativeMemory.Alloc((nuint)sizeof(T));

            pointer[0] = value;

            return pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* Alloc<T>(ReadOnlySpan<T> values) where T : unmanaged
        {
            var pointer = (T*)NativeMemory.Alloc((nuint)values.Length, (nuint)sizeof(T));

            var span = new Span<T>(pointer, values.Length);

            values.CopyTo(span);

            return pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* AllocZeroed<T>() where T : unmanaged =>
            (T*)NativeMemory.AllocZeroed((nuint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* AllocZeroed<T>(nuint count) where T : unmanaged =>
            (T*)NativeMemory.AllocZeroed(count, (nuint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* AlignedAllocZeroed(nuint byteCount, nuint alignment)
        {
            var pointer = NativeMemory.AlignedAlloc(byteCount, alignment);

            NativeMemory.Fill(pointer, byteCount, 0);

            return pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* AlignedAllocZeroed<T>() where T : unmanaged =>
            (T*)AlignedAllocZeroed((nuint)sizeof(T), (nuint)Marshal.GetAlignment(sizeof(T)));
    }
}
