using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public abstract unsafe partial record NativeReference
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Init<M, U>(ref U* destination, M[] value, Func<M[], U[]> converter) where U : unmanaged =>
        destination = PointerHelper.Alloc(converter.Invoke(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Init<M, U>(ref M[] field, ref U* destination, M[] value, Func<M[], U[]> converter)
    where U : unmanaged
    {
        field = value;

        Init(ref destination, value, converter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Init<M, N, U>(ref M[] field, ref U* destination, ref N length, M[] value, Func<M[], U[]> converter)
    where N : INumber<N>
    where U : unmanaged
    {
        field  = value;
        length = N.CreateChecked(value.Length);

        Init(ref destination, value, converter);
    }

    private static void Init<M, U>(M[] field, U* destination, uint fixedLength, M[] value, string propertyName, Func<M, U> converter)
    where U : unmanaged
    {
        if (value.Length != fixedLength)
        {
            throw new InvalidOperationException($"Property {propertyName} must have the fixed size of {fixedLength}");
        }

        for (var i = 0; i < fixedLength; i++)
        {
            field[i]       = value[i];
            destination[i] = converter.Invoke(value[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nint ToHandler<M>(M value) where M : NativeHandle =>
        (nint)value;

    private static nint[] ToHandler<M>(M[] values) where M : NativeHandle
    {
        var result = new nint[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            result[i] = (nint)values[i];
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static U ToNative<M, U>(M value)
    where M : NativeReference<U>
    where U : unmanaged =>
        value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static U[] ToNative<M, U>(M[] values)
    where M : NativeReference<U>
    where U : unmanaged
    {
        var result = new U[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            result[i] = (U)values[i];
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static U ToSelf<U>(U value) where U : unmanaged =>
        value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static U[] ToSelf<U>(U[] values) where U : unmanaged =>
        values;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected static T* Alloc<T>(in T value) where T : unmanaged =>
        PointerHelper.Alloc(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Free<U>(ref U* pointer) where U : unmanaged
    {
        PointerHelper.Free(pointer);

        pointer = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Free(ref byte** pointer, uint length)
    {
        PointerHelper.Free(pointer, length);

        pointer = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static string? Get(ref string? field, byte* value)
    {
        if (field == null && value != null)
        {
            field = Marshal.PtrToStringAnsi((nint)value);
        }

        return field;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static U[] Get<U>(ref U[] field, U* value, uint length) where U : unmanaged
    {
        if (field.Length != length)
        {
            field = PointerHelper.ToArray(value, length);
        }

        return field;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static M[] Get<M, U>(ref M[] field, U* value, uint length, Func<U, M> factory)
    where M : NativeReference<U>
    where U : unmanaged
    {
        if (field.Length != length)
        {
            field = new M[length];

            for (var i = 0; i < length; i++)
            {
                field[i] = factory.Invoke(value[i]);
            }
        }

        return field;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<U>(ref U* destination, U[] value) where U : unmanaged =>
        Init(ref destination, value, ToSelf);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M, U>(ref M[] field, ref U* destination, M[] value) where M : NativeReference<U> where U : unmanaged =>
        Init(ref field, ref destination, value, ToNative<M, U>);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M>(ref M[] field, ref nint* destination, M[] value) where M : NativeHandle =>
        Init(ref field, ref destination, value, ToHandler);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M, N>(ref M[] field, ref nint* destination, ref N length, M[] value)
    where M : NativeHandle
    where N : INumber<N> =>
        Init(ref field, ref destination, ref length, value, ToHandler);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M, N, U>(ref M[] field, ref U* destination, ref N length, M[] value)
    where M : NativeReference<U>
    where N : INumber<N>
    where U : unmanaged =>
        Init(ref field, ref destination, ref length, value, ToNative<M, U>);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<U>(ref U[] field, ref U* destination, U[] value) where U : unmanaged =>
        Init(ref field, ref destination, value, ToSelf);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<U>(U[] field, U* destination, uint fixedLength, U[] value, string propertyName) where U : unmanaged =>
        Init(field, destination, fixedLength, value, propertyName, ToSelf);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M, U>(M[] field, nint* destination, uint fixedLength, M[] value, string propertyName)
    where M : NativeHandle
    where U : unmanaged =>
        Init(field, destination, fixedLength, value, propertyName, ToHandler);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<M, U>(M[] field, U* destination, uint fixedLength, M[] value, string propertyName)
    where M : NativeReference<U>
    where U : unmanaged =>
        Init(field, destination, fixedLength, value, propertyName, ToNative<M, U>);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<U>(byte[] field, U* destination, uint fixedLength, U[] value, string propertyName) where U : unmanaged
    {
        if (value.Length != fixedLength)
        {
            throw new InvalidOperationException($"Property {propertyName} must have the fixed size of {fixedLength}");
        }

        PointerHelper.Copy(value, destination);
        MemoryMarshal.Cast<U, byte>(value).CopyTo(field.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init<U, N>(ref U[] field, ref U* destination, ref N length, U[] value)
    where U : unmanaged
    where N : INumber<N>
    {
        if (value.Length > 0)
        {
            destination = PointerHelper.Alloc(value);
            field       = value;
            length      = N.CreateChecked(value.Length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init(ref string? field, ref byte* destination, string? value)
    {
        field       = value;
        destination = PointerHelper.Alloc(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void Init(ref string[] field, ref byte** destination, ref uint length, string[] value)
    {
        field       = value;
        destination = PointerHelper.Alloc(value);
        length      = (uint)value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected static nint ToPointer<U>(ref U? field, U? value) where U : Delegate
    {
        field = value;
        return ToPointer(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected static nint ToPointer<U>(U? value) where U : Delegate =>
        value == null ? default : Marshal.GetFunctionPointerForDelegate(value);
}

#pragma warning disable CA1001
public unsafe abstract record NativeReference<T> : NativeReference where T : unmanaged
{
    private readonly Ref<T> pNative;

    protected T* PNative => this.pNative;

    public NativeReference(NativeReference<T> self) : base(self) =>
        this.pNative = new(*self.pNative.Handle);

    internal NativeReference(in T value) =>
        this.pNative = new(value);

    public NativeReference() =>
        this.pNative = new();

    ~NativeReference()
    {
        this.OnFinalize();
        this.pNative.Free();
    }

    protected virtual void OnFinalize() { }

    public static implicit operator T(NativeReference<T>? value) => value == null ? default : *value.PNative;
    public static implicit operator T*(NativeReference<T>? value) => value == null ? default : value.PNative;
    public static implicit operator nint(NativeReference<T>? value) => value == null ? default : (nint)value.PNative;
}
