using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Age.Graphs;
using Age.Platforms.Display;
using Age.Rendering.Resources;
using Age.Scenes;

namespace Age.Tests.Age.Elements;

internal static class WindowAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "window")]
    internal static extern ref global::Age.Platforms.Display.Window GetDisplayWindow(global::Age.Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "renderTargets")]
    internal static extern ref RenderTarget[] GetRenderTargets(global::Age.Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<RenderTree>k__BackingField")]
    internal static extern ref RenderTree GetRenderTree(global::Age.Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Surface>k__BackingField")]
    internal static extern ref Surface GetSurface(global::Age.Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<RenderGraph>k__BackingField")]
    internal static extern ref RenderGraph GetRenderGraph(global::Age.Window window);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "updatesQueue")]
    internal static extern ref Queue<Action> GetUpdatesQueue(RenderTree renderTree);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "sceneGraphCache")]
    internal static extern ref SceneGraphCache GetSceneGraphCache(RenderTree renderTree);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MakeSubtreeStatePristine")]
    internal static extern void MakeSubtreeStatePristine(Renderable renderable);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<State>k__BackingField")]
    internal static unsafe extern ref WindowState* GetStateBackingField(global::Age.Platforms.Display.Window window);
}
