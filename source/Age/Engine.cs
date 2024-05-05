using System.Diagnostics;
using Age.Internal;
using Age.Numerics;
using Age.Rendering;
using Age.Rendering.Drawing;
using Age.Rendering.Services;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;

namespace Age;

public class Engine : IDisposable
{
    private const bool   FPS_LOCKED        = true;
    private const bool   DRAW_BVH          = true;
    private const ushort TARGET_FPS        = 60;
    private const double TARGET_FRAME_TIME = 1000.0 / TARGET_FPS;

    private readonly Dictionary<NodeTree, BvhTree> bvhMap = [];
    private readonly Container                     container;
    private readonly VulkanRenderer                renderer  = new();

    private bool disposed;

    public Window Window { get; }
    public bool Running { get; private set; }

    public Engine(string name, Size<uint> windowSize, Point<int> windowPosition)
    {
        Window.Register(this.renderer);

        this.Window = new Window(name, windowSize, windowPosition);

        var textureStorage   = new TextureStorage(this.renderer);
        var renderingService = new RenderingService(this.renderer, textureStorage);
        var textService      = new TextService(renderingService, textureStorage);

        this.container = new()
        {
            RenderingService = renderingService,
            TextService      = textService,
            TextureStorage   = textureStorage,
        };

        this.Window.SizeChanged += this.container.RenderingService.RequestDraw;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                Platforms.Display.Window.CloseAll();

                this.container.Dispose();
                this.renderer.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        this.Running = true;

        var previous  = 0D;
        var frameTime = 0D;
        var current   = TARGET_FRAME_TIME;

        foreach (var window in Window.Windows)
        {
            window.Tree.Initialize();
            window.SizeChanged += () => window.Tree.HasChanges = true;
        }

        var watch = Stopwatch.StartNew();

        while (this.Running)
        {
            frameTime += current - previous;

            foreach (var window in Window.Windows)
            {
                if (window.Tree.HasChanges)
                {
                    if (!this.bvhMap.TryGetValue(window.Tree, out var bvhTree))
                    {
                        this.bvhMap[window.Tree] = bvhTree = new();
                    }

                    bvhTree.Build(window.Tree);

                    if (DRAW_BVH)
                    {
                        foreach (var node in window.Tree.Enumerate<BvhDebugNode>())
                        {
                            node.Remove();
                        }

                        window.Tree.AppendChild(bvhTree.Draw());
                    }

                    window.Tree.HasChanges = false;
                }
            }

            if (!FPS_LOCKED || frameTime >= TARGET_FRAME_TIME)
            {
                var deltaTime = frameTime / 1000;

                foreach (var window in Window.Windows)
                {
                    window.DoEvents();
                    window.Tree.Update(deltaTime);
                }

                this.container.RenderingService.Render(Window.Windows);

                this.Running = Window.Windows.Any(x => !x.Closed);

                frameTime = 0;
            }

            previous = current;
            current  = watch.Elapsed.TotalMilliseconds;
        }

        watch.Stop();
    }
}
