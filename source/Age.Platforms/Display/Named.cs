#if LINUX
using System.Diagnostics.CodeAnalysis;

namespace Age.Platforms.Display;

internal unsafe struct Named<T>(uint name, T* value) : IEquatable<Named<T>> where T : unmanaged
{
    public T* Value = value;

    public uint Name = name;

    public readonly bool Equals(Named<T> other) =>
        this.Name == other.Name && this.Value == other.Value;

    public override readonly bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Named<T> named && this.Equals(named);

    public override readonly int GetHashCode() =>
        HashCode.Combine(this.Name, (nint)this.Value);

    public static implicit operator T*(Named<T> named) => named.Value;

    public static bool operator ==(Named<T> left, Named<T> right) => left.Equals(right);
    public static bool operator !=(Named<T> left, Named<T> right) => !left.Equals(right);
}
#endif
