namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(int value)
    {
        public int ClampSubtract(int other) =>
            value > other ? value - other : 0;
    }
}
