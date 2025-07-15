using Age.Core.Collections;

namespace Age.Elements;

[KeyedListKey]
public enum LayoutLayer : byte
{
    None   = 0,
    Layer1 = Element.LayoutCommand.Layer1,
    Layer2 = Element.LayoutCommand.Layer2,
    Layer3 = Element.LayoutCommand.Layer3,
}
