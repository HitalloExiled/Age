using System.Runtime.CompilerServices;
using Age.Elements;

namespace Age.Tests.Age.Scenes;

public static class ElementAcessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "commandsSeparator")]
    public static extern ref byte GetCommandsSeparator(Element element);
}
