namespace Age.Core.Collections;

public unsafe static class NativeStackExtensions
{
    extension<T>(NativeStack<T> stack) where T : unmanaged, IEquatable<T>
    {
        public bool Contains(T item) =>
            UnsafeStack.Contains(stack.GetUnsafeStack(), item);
    }
}
