using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Elements;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public static class RenderableAcessor<T> where T : Command
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "AddCommand")]
    public static extern void AddCommand(Renderable<T> node, T command);
}

public static class ElementAcessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "commandsSeparator")]
    public static extern ref byte GetCommandsSeparator(Element element);
}
