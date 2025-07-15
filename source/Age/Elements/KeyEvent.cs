using Age.Platforms.Display;
using Key = Age.Platforms.Display.Key;

namespace Age.Elements;

public struct KeyEvent
{
    public Key       Key;
    public KeyStates Modifiers;
    public bool      Holding;
}
