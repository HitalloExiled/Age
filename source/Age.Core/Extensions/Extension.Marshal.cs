using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

file struct AlignmentHelper<T> where T : unmanaged
{
    public byte Sentinel;
    public T    Target;
}

public static partial class Extension
{
    extension(Marshal)
    {
        public static int GetAlignment<T>() where T : unmanaged
        {
            AlignmentHelper<T> helper = default;

            ref var origin = ref Unsafe.As<AlignmentHelper<T>, byte>(ref helper);
            ref var target = ref Unsafe.As<T, byte>(ref helper.Target);

            return (int)Unsafe.ByteOffset(ref origin, ref target);
        }
    }
}
