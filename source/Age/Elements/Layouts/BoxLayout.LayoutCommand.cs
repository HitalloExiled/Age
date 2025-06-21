using Age.Core.Collections;

namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    [KeyedListKey]
    public enum LayoutCommand : byte
    {
        None     = 0,
        Box      = 1 << 0,
        Image    = 1 << 1,
        ScrollX  = 1 << 2,
        ScrollY  = 1 << 3,
        Layer1   = 1 << 4,
        Layer2   = 1 << 5,
        Layer3   = 1 << 6,
    }
}
