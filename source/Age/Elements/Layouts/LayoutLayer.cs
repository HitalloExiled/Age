using Age.Core.Collections;

namespace Age.Elements.Layouts;

[KeyedListKey]
public enum LayoutLayer : byte
{
    None   = 0,
    Layer1 = BoxLayout.LayoutCommand.Layer1,
    Layer2 = BoxLayout.LayoutCommand.Layer2,
    Layer3 = BoxLayout.LayoutCommand.Layer3,
}
