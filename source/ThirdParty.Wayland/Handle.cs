namespace ThirdParty.Wayland;

internal readonly unsafe record struct Handle<T> where T : unmanaged
{
    public readonly nint Value { get; }

    public Handle(T* value) =>
        this.Value = (nint)value;

    public override readonly string ToString() =>
        "0x" + this.Value.ToString("x");

    public static Handle<T> EnsureNotNull(Handle<T> value, string message) =>
        value == default ? throw new NullReferenceException(message) : value;

    public static implicit operator T*(Handle<T> handle) => (T*)handle.Value;
    public static implicit operator Handle<T>(T* pointer) => new(pointer);
}
