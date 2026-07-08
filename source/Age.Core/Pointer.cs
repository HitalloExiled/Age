using System.Runtime.CompilerServices;

namespace Age.Core;

public unsafe struct Pointer(void* value)
{
    public void* Value = value;

    public readonly bool Equals(Pointer other) =>
        this.Value == other.Value;

    public override readonly bool Equals(object? obj) =>
        obj is Pointer pointer && this.Equals(pointer);

    public override readonly int GetHashCode() =>
        ((nint)this.Value).GetHashCode();

    public static bool operator ==(Pointer left, Pointer right) => left.Equals(right);
    public static bool operator !=(Pointer left, Pointer right) => !(left == right);

    public static implicit operator void*(Pointer ptr) => ptr.Value;
    public static implicit operator Pointer(void* ptr) => new(ptr);
}

public unsafe struct Pointer<T>(T* value) : IEquatable<Pointer<T>> where T : unmanaged, allows ref struct
{
    public T* Value = value;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Value[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.Value[index] = value;
    }

    public readonly bool Equals(Pointer<T> other) =>
        this.Value == other.Value;

    public override readonly bool Equals(object? obj) =>
        obj is Pointer<T> pointer && this.Equals(pointer);

    public override readonly int GetHashCode() =>
        ((nint)this.Value).GetHashCode();

    public static bool operator ==(Pointer<T> left, Pointer<T> right) => left.Equals(right);
    public static bool operator !=(Pointer<T> left, Pointer<T> right) => !(left == right);

    public static implicit operator T*(Pointer<T> ptr) => ptr.Value;
    public static implicit operator Pointer<T>(T* ptr) => new(ptr);
}

public unsafe static class PointerHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullIfDefault<T>(T* pointer) where T : unmanaged =>
        (*pointer).Equals(default(T)) ? null : pointer;
}
