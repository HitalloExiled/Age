namespace Age.Core.Unsafe;

public readonly unsafe struct Ptr<T>(T* pointer) where T : unmanaged
{
    private readonly T* pointer = pointer;

    public T Value => *this.pointer;

    public static implicit operator Ptr<T>(T* pointer) => new(pointer);
    public static implicit operator T*(Ptr<T> pointer) => pointer.pointer;
    public static implicit operator nint(Ptr<T> pointer) => (nint)pointer.pointer;
}
