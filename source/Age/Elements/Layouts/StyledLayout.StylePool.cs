using Age.Core;
using Age.Styling;

namespace Age.Elements.Layouts;

internal abstract partial class StyledLayout
{
    private sealed class StylePool : ObjectPool<Style>
    {
        protected override Style Create() => new();
    }
}
