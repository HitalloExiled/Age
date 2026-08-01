using System.Runtime.CompilerServices;
using Age.Platforms.Display;

namespace Age.Tests.Age.Acessors;

internal static class DisplayWindowAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<State>k__BackingField")]
    internal static unsafe extern ref WindowState* GetStateBackingField(Platforms.Display.Window window);
}

