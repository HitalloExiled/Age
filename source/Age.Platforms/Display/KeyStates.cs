namespace Age.Platforms.Display;

[Flags]
public enum KeyStates
{
    None    = 0,
    Alt     = 1 << 0,
    Shift   = 1 << 1,
    Control = 1 << 2,
}
