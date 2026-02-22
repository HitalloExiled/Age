using System.ComponentModel;

namespace Age.Core.Collections;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class InlineListBuilder
{
    public static InlineList4<T> InlineList4<T>(ReadOnlySpan<T> values) => new(values);
    public static InlineList8<T> InlineList8<T>(ReadOnlySpan<T> values) => new(values);
    public static InlineList16<T> InlineList16<T>(ReadOnlySpan<T> values) => new(values);
}
