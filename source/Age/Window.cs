using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age;

public class Window(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent = null) : Platforms.Display.Window(title, size, position, parent), IWindow
{
    public new static IEnumerable<Window> Windows => Platforms.Display.Window.Windows.Cast<Window>();

    public Content Content { get; } = new();

    private static VulkanRenderer renderer = null!;

    public Surface Surface { get; private set; } = null!;

    public static void Register(VulkanRenderer renderer)
    {
        PlatformRegister("Age.Window");

        Window.renderer = renderer;
    }

    protected override void PlatformCreate(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent)
    {
        base.PlatformCreate(title, size, position, parent);

        this.Surface = renderer.Context.CreateSurface(this.Handle, this.ClientSize);

        this.SizeChanged += () =>
        {
            this.Surface.Size   = this.ClientSize;
            this.Surface.Hidden = this.Minimized || !this.Visible;
        };
    }

    protected override void PlatformClose()
    {
        base.PlatformClose();

        renderer.WaitIdle();

        this.Surface.Dispose();
    }
}
