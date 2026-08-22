using System.Runtime.CompilerServices;
using Age.Scenes;

namespace Age.Tests.Age.Acessors;

internal static class RenderTreeAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "sceneGraphCache")]
    internal static extern ref SceneGraphCache GetSceneGraphCache(RenderTree renderTree);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "updatesQueue")]
    internal static extern ref Queue<Action> GetUpdatesQueue(RenderTree renderTree);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Timers>k__BackingField")]
    internal static extern ref List<global::Age.Scenes.Timer> GetTimers(RenderTree renderTree);
}
