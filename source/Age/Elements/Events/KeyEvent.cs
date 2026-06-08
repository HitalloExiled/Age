using Age.Platforms.Display;
using Key = Age.Platforms.Display.Key;

namespace Age.Elements.Events;

public struct KeyEvent
{
    public Key      Key;
    public Modifier Modifiers;
    public bool     Holding;
}
