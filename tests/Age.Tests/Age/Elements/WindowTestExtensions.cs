using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scenes;

namespace Age.Tests.Age.Elements;

internal static class WindowTestExtensions
{
    public static Window CreateTestWindow()
    {
        var window = (Window)RuntimeHelpers.GetUninitializedObject(typeof(Window));

        WindowAccessor.MakeSubtreeStatePristine(window);

        SetupWindow(window);

        return window;
    }

    private static unsafe void SetupWindow(Window window)
    {
        var displayWindow = (Platforms.Display.Window)RuntimeHelpers.GetUninitializedObject(typeof(Platforms.Display.Window));

        WindowAccessor.GetStateBackingField(displayWindow) = WindowState.Allocate(null, new Size<int>(800, 600));

        var renderTree = CreateMockRenderTree();

        WindowAccessor.GetDisplayWindow(window) = displayWindow;
        WindowAccessor.GetRenderTargets(window) = [];
        WindowAccessor.GetRenderTree(window)    = renderTree;
    }

    private static RenderTree CreateMockRenderTree()
    {
        var renderTree = (RenderTree)RuntimeHelpers.GetUninitializedObject(typeof(RenderTree));
        WindowAccessor.GetUpdatesQueue(renderTree)    = [];
        WindowAccessor.GetSceneGraphCache(renderTree) = new();
        return renderTree;
    }
}
