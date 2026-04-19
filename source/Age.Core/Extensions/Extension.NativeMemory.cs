using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(NativeMemory)
    {
        public static unsafe T* Alloc<T>() where T : unmanaged =>
            (T*)NativeMemory.Alloc((nuint)sizeof(T));

        public static unsafe T* Alloc<T>(uint count) where T : unmanaged =>
            (T*)NativeMemory.Alloc((nuint)sizeof(T) * count);

        public static unsafe T* AllocSet<T>(T value) where T : unmanaged =>
            AllocSet([value]);

        public static unsafe T* AllocSet<T>(ReadOnlySpan<T> values) where T : unmanaged
        {
            var pointer = (T*)NativeMemory.Alloc((nuint)(sizeof(T) * values.Length));

            for (var i = 0; i < values.Length; i++)
            {
                pointer[0] = values[i];
            }

            return pointer;
        }

        public static unsafe T* AllocZeroed<T>() where T : unmanaged =>
            (T*)NativeMemory.AllocZeroed((nuint)sizeof(T));

        public static unsafe T* AllocZeroed<T>(uint count) where T : unmanaged =>
            (T*)NativeMemory.AllocZeroed((nuint)sizeof(T) * count);
    }
}
