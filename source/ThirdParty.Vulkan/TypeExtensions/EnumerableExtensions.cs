namespace ThirdParty.Vulkan.TypeExtensions;

public static class EnumerableExtensions
{
    public static nint[] ToHandlers(this IEnumerable<NativeHandle> source) =>
        source.Select(static x => (nint)x).ToArray();

    public static U[] ToNatives<U>(this IEnumerable<NativeReference<U>> source) where U : unmanaged =>
        source.Select(static x => (U)x).ToArray();
}
