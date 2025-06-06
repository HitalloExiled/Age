namespace Age.Styling;

[Flags]
public enum Overflow : byte
{
    None     = 0,
    Clipping = 1 << 0,
    ScrollX  = 1 << 1 | Clipping,
    ScrollY  = 1 << 2 | Clipping,
    Scroll   = ScrollX | ScrollY,
}
