using System.Runtime.CompilerServices;
using Age.Scenes;

namespace Age.Tests.Age.Acessors;

internal static class RenderableAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MakeSubtreeStatePristine")]
    internal static extern void MakeSubtreeStatePristine(Renderable renderable);
}
