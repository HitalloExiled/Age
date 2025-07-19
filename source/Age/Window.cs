using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Scene;

using PlatformWindow = Age.Platforms.Display.Window;

namespace Age;

public sealed partial class Window : PlatformWindow
{
    private static VulkanRenderer renderer = null!;

    public new static IEnumerable<Window> Windows => new WindowCastEnumerator(PlatformWindow.Windows);

    public Surface Surface { get; private set; } = null!;
    public RenderTree Tree { get; }

    public Window(string title, Size<uint> size, Point<int> position, PlatformWindow? parent = null) : base(title, size, position, parent) =>
        this.Tree = new(this);

    protected override void PlatformClose()
    {
        base.PlatformClose();

        renderer.WaitIdle();

        this.Surface.Dispose();
    }

    protected override void PlatformCreate(string title, Size<uint> size, Point<int> position, PlatformWindow? parent)
    {
        base.PlatformCreate(title, size, position, parent);

        this.Surface = renderer.CreateSurface(this.Handle, this.ClientSize);

        this.Resized += () =>
        {
            this.Surface.Size = this.ClientSize;
            this.Surface.Hidden = this.IsMinimized || !this.IsVisible;
        };
    }

    public static void Register(VulkanRenderer renderer)
    {
        PlatformRegister("Age.Window");

        Window.renderer = renderer;
    }
}
