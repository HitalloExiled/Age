namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(ushort value)
    {
        public ushort ClampSubtract(ushort other) =>
            (ushort)(value > other ? value - other : 0);
    }
}
