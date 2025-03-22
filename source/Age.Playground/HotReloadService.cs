//#if DEBUG
using Age.Playground;
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(HotReloadService))]

namespace Age.Playground;

public static class HotReloadService
{
    public static event Action? ApplicationUpdated;

    public static void ClearCache(Type[]? _) =>
        Console.WriteLine("ClearCache was called");

    public static void UpdateApplication(Type[]? _)
    {
        Console.WriteLine("UpdateApplication was called");
        ApplicationUpdated?.Invoke();
    }
}
//#endif
