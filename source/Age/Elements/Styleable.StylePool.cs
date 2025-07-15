using Age.Core;
using Age.Styling;

namespace Age.Elements;

public abstract partial class Styleable
{
    private sealed class StylePool : ObjectPool<Style>
    {
        protected override Style Create() => new();
    }
}
