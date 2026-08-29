using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scenes;
using Age.Tests.Age.Acessors;

namespace Age.Tests.Age;

internal static class WindowTestExtensions
{
    private static unsafe void SetupWindow(Window window)
    {
        var displayWindow = (Platforms.Display.Window)RuntimeHelpers.GetUninitializedObject(typeof(Platforms.Display.Window));

        DisplayWindowAccessor.GetStateBackingField(displayWindow) = WindowState.Allocate(null, new Size<int>(800, 600));

        var renderTree = CreateMockRenderTree();

        WindowAccessor.GetDisplayWindow(window) = displayWindow;
        WindowAccessor.GetRenderTargets(window) = [];
        WindowAccessor.GetRenderTree(window)    = renderTree;
        WindowAccessor.GetRenderContext(window) = new();

        var sceneSlot = new Empty();

        WindowAccessor.GetSceneSlot(window) = sceneSlot;

        window.AppendChild(sceneSlot);
    }

    private static RenderTree CreateMockRenderTree()
    {
        var renderTree = (RenderTree)RuntimeHelpers.GetUninitializedObject(typeof(RenderTree));

        RenderTreeAccessor.GetSceneGraphCache(renderTree) = new();
        RenderTreeAccessor.GetTimers(renderTree)       = [];
        RenderTreeAccessor.GetUpdatesQueue(renderTree) = [];
        RenderTreeAccessor.GetViewports(renderTree)    = [];

        return renderTree;
    }

    extension(Window)
    {
        public static Window CreateMock()
        {
            var window = (Window)RuntimeHelpers.GetUninitializedObject(typeof(Window));

            RenderableAccessor.MakeSubtreeStatePristine(window);

            SetupWindow(window);

            window.Connect();

            window.Scene = new();

            window.RenderTree.Initialize();

            return window;
        }
    }
}
