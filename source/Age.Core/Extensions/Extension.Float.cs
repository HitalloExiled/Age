namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(float value)
    {
        public float ClampSubtract(float other) =>
            value > other ? value - other : 0;
    }
}
