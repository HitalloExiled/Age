using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;

namespace Age;

public class Window : Age.Platforms.Display.Window, IWindow
{
    public new static IEnumerable<Window> Windows = windows.Values.Cast<Window>();

    public Content Content { get; } = new();

    private static VulkanRenderer renderer = null!;

    public SurfaceContext Context { get; private set; } = null!;

    public Window(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent = null) : base(title, size, position, parent)
    { }

    public static void Register(VulkanRenderer renderer)
    {
        PlatformRegister("Age.Window");

        Window.renderer = renderer;
    }

    protected override void PlatformCreate(string title, Size<uint> size, Point<int> position, Platforms.Display.Window? parent)
    {
        base.PlatformCreate(title, size, position, parent);

        this.Context = renderer.Context.CreateSurfaceContext(this.Handle, this.ClientSize);
    }

    protected override void PlatformClose()
    {
        base.PlatformClose();

        renderer.Context.DestroySurfaceContext(this.Context);
    }
}
