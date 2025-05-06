namespace Age.Extensions;

public static partial class Extension
{
    public static ushort ClampSubtract(this ushort value, ushort other) =>
        (ushort)(value > other ? value - other : 0);
}
