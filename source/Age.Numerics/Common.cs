namespace Age.Numerics;

public static class Common
{
    public static void ThrowIfOutOfRange(int min, int max, int value)
    {
        if (value < min || value > max)
        {
            throw new IndexOutOfRangeException();
        }
    }
}
