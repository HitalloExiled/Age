using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Scene;

namespace Age;

public sealed class Window : Platforms.Display.Window
{
    public new static IEnumerable<Window> Windows => Platforms.Display.Window.Windows.Cast<Window>();

    public RenderTree Tree { get; }

    private static VulkanRenderer renderer = null!;

    public Window(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent = null) : base(title, size, position, parent) =>
        this.Tree = new(this);

    public Surface Surface { get; private set; } = null!;

    public static void Register(VulkanRenderer renderer)
    {
        PlatformRegister("Age.Window");

        Window.renderer = renderer;
    }

    protected override void PlatformCreate(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent)
    {
        base.PlatformCreate(title, size, position, parent);

        this.Surface = renderer.CreateSurface(this.Handle, this.ClientSize);

        this.Resized += () =>
        {
            this.Surface.Size   = this.ClientSize;
            this.Surface.Hidden = this.IsMinimized || !this.IsVisible;
        };
    }

    protected override void PlatformClose()
    {
        base.PlatformClose();

        renderer.WaitIdle();

        this.Surface.Dispose();
    }
}
