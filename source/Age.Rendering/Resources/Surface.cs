using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public sealed class Surface : Disposable
{
    public event Action? SwapchainRecreated;

    private Size<uint> size;
    private Swapchain  swapchain;

    internal VkSurfaceKHR Value { get; }

    public static bool AllHidden => !AnyVisible;

    public static bool AnyVisible
    {
        get
        {
            foreach (var surface in Entries)
            {
                if (surface.Visible)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static ReadOnlySpan<Surface> Visibles
    {
        get
        {
            var visibles = new List<Surface>(Entries.Count);

            foreach (var surface in Entries)
            {
                if (surface.Visible)
                {
                    visibles.Add(surface);
                }
            }

            return visibles.AsSpan();
        }
    }

    public static List<Surface> Entries { get; } = [];

    public VkSemaphore[] Semaphores { get; }

    public bool IsDirty { get; private set; }

    public Swapchain Swapchain
    {
        get => this.swapchain;
        internal set
        {
            if (this.swapchain != value)
            {
                this.swapchain = value;

                this.SwapchainRecreated?.Invoke();
            }
        }
    }

    public uint CurrentBuffer { get; internal set; }

    public Size<uint> Size
    {
        get => this.size;
        set
        {
            if (this.size != value)
            {
                this.size = value;

                this.MakeDirty();
            }
        }
    }

    public bool Visible { get; set; } = true;

    internal void MakeDirty() =>
        this.IsDirty = true;

    internal void MakePristine() =>
        this.IsDirty = false;

    internal Surface(VkSurfaceKHR vkSurfaceKHR, in Size<uint> size, VkSemaphore[] semaphores, Swapchain swapchain)
    {
        this.Value      = vkSurfaceKHR;
        this.size       = size;
        this.Semaphores = semaphores;
        this.swapchain  = swapchain;

        Entries.Add(this);
    }

    protected override void OnDisposed(bool disposing)
    {
        Entries.Remove(this);

        if (disposing)
        {
            foreach (var semaphore in this.Semaphores)
            {
                semaphore.Dispose();
            }

            this.Swapchain.Dispose();
            this.Value.Dispose();
        }
    }
}
