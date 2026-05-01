using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe static class NativeListExtensions
{
    extension<T>(NativeList<T> list) where T : unmanaged, IEquatable<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) =>
            UnsafeList.Contains(list.GetUnsafeList(), item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) =>
            UnsafeList.IndexOf(list.GetUnsafeList(), item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T item) =>
            UnsafeList.LastIndexOf(list.GetUnsafeList(), item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item) =>
            UnsafeList.Remove(list.GetUnsafeList(), item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveUnordered(T item) =>
            UnsafeList.RemoveUnordered(list.GetUnsafeList(), item);
    }
}
