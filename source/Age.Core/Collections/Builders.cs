using System.ComponentModel;

namespace Age.Core.Collections;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class Builders
{
    public static InlineList16<T> InlineList16<T>(ReadOnlySpan<T> values) => new(values);
    public static InlineList4<T> InlineList4<T>(ReadOnlySpan<T> values) => new(values);
    public static InlineList8<T> InlineList8<T>(ReadOnlySpan<T> values) => new(values);
    public static NativeArray<T> NativeArray<T>(ReadOnlySpan<T> values) where T : unmanaged => new(values);
    public static NativeList<T> NativeList<T>(ReadOnlySpan<T> values) where T : unmanaged => new(values);
    public static NativeRefArray<T> NativeRefArray<T>(ReadOnlySpan<T> values) where T : unmanaged => new(values);
    public static NativeRefList<T> NativeRefList<T>(ReadOnlySpan<T> values) where T : unmanaged => new(values);
    public static NativeStringArray NativeStringArray(ReadOnlySpan<string> values) => new(values);
    public static NativeStringList NativeStringList(ReadOnlySpan<string> values) => new(values);
    public static NativeStringRefArray NativeStringRefArray(ReadOnlySpan<string> values) => new(values);
    public static NativeStringRefList NativeStringRefList(ReadOnlySpan<string> values) => new(values);
}
