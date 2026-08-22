using System.Runtime.CompilerServices;
using Age.Graphs;
using Age.Rendering.Resources;
using Age.Scenes;

namespace Age.Tests.Age.Acessors;

internal static class WindowAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "window")]
    internal static extern ref Platforms.Display.Window GetDisplayWindow(Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "renderTargets")]
    internal static extern ref RenderTarget[] GetRenderTargets(Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "uiSceneSlot")]
    internal static extern ref Empty GetUiSceneSlot(Viewport window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<RenderTree>k__BackingField")]
    internal static extern ref RenderTree GetRenderTree(Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<RenderContext>k__BackingField")]
    internal static extern ref RenderContext GetRenderContext(Viewport window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Surface>k__BackingField")]
    internal static extern ref Surface GetSurface(Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<RenderGraph>k__BackingField")]
    internal static extern ref RenderGraph GetRenderGraph(Window window);
}
