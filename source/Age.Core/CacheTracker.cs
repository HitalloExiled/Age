namespace Age.Core;

public static class CacheTracker
{
    public static uint Version { get; private set; } = 1;

    public static void Invalidate() =>
        Version++;
}
