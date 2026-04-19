using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(NativeMemory)
    {
        public static unsafe T* Alloc<T>() where T : unmanaged =>
            (T*)NativeMemory.Alloc((nuint)sizeof(T));

        public static unsafe T* AllocSet<T>(in T value) where T : unmanaged
        {
            var pointer = (T*)NativeMemory.Alloc((nuint)sizeof(T));

            pointer[0] = value;

            return pointer;
        }

        public static unsafe T* AllocZeroed<T>() where T : unmanaged =>
            (T*)NativeMemory.AllocZeroed((nuint)sizeof(T));
    }
}
