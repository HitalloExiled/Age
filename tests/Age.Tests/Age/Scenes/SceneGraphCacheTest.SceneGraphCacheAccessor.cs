using System.Runtime.CompilerServices;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private static class SceneGraphCacheAccessor
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "dirtySubtrees")]
        internal static extern ref List<Renderable> GetDirtTrees(SceneGraphCache cache);
    }
}
