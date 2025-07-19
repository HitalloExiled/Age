namespace Age.Extensions;

public static partial class Extension
{
    extension(uint value)
    {
        public uint ClampSubtract(uint other) =>
            value > other ? value - other : 0;
    }
}
