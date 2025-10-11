using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public static class RenderableAcessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_DirtState")]
    public static extern void SetDirtState(Renderable node, DirtState dirtState);
}

public static class RenderableAcessor<T> where T : Command
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "AddCommand")]
    public static extern void AddCommand(Renderable<T> node, T command);
}
