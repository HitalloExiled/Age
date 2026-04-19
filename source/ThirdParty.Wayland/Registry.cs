using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace ThirdParty.Wayland;

public delegate void GlobalAddedHandler(uint name, ReadOnlySpan<byte> @interface, uint version);
public delegate void GlobalRemovedHandler(uint name);

public unsafe class Registry : DisposableManaged<wl_registry>
{
    private const uint WL_REGISTRY_BIND = 0;

    private event GlobalAddedHandler? globalAdded;
    private event GlobalRemovedHandler? globalRemoved;

    private wl_registry_listener* nativeListener;

    private static Registry Singleton { get; set; } = null!;

    public event GlobalAddedHandler? GlobalAdded
    {
        add
        {
            globalAdded += value;

            this.EnsureNativeListener();
        }
        remove
        {
            globalAdded -= value;

            this.TryDetachNativeListener();
        }
    }

    public event GlobalRemovedHandler? GlobalRemoved
    {
        add
        {
            globalRemoved += value;

            this.EnsureNativeListener();
        }
        remove
        {
            globalRemoved -= value;

            this.TryDetachNativeListener();
        }
    }

    internal Registry(Handle<wl_registry> handle) : base(handle)
    {
        Debug.Assert(Singleton == null);

        Singleton = this;
    }

    [UnmanagedCallersOnly]
    private static void OnGlobalCallback(void* data, wl_registry* pRegistry, uint name, byte* pInterface, uint version)
    {
        var @interface = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pInterface);

        Singleton.globalAdded?.Invoke(name, @interface, version);
    }

    [UnmanagedCallersOnly]
    private static void OnGlobalRemoveCallback(void* data, wl_registry* pRegistry, uint name) =>
        Singleton.globalRemoved?.Invoke(name);

    protected override void OnDisposed(bool disposing)
    {
        Singleton = null!;

        NativeMemory.Free(this.nativeListener);

        PInvoke.wl_proxy_destroy((wl_proxy*)this.Handle.Value);
    }

    private void EnsureNativeListener()
    {
        if (this.nativeListener == null)
        {
            this.nativeListener = NativeMemory.AllocSet<wl_registry_listener>(
                new()
                {
                    global        = &OnGlobalCallback,
                    global_remove = &OnGlobalRemoveCallback,
                }
            );

            WaylandException.Check(PInvoke.wl_proxy_add_listener(this.Handle, this.nativeListener, null), "Failed to add listener");
        }
    }

    private void TryDetachNativeListener()
    {
        if (this.nativeListener != null && this.globalAdded == null && this.globalRemoved == null)
        {
            WaylandException.Check(PInvoke.wl_proxy_add_listener(this.Handle, null, null), "Failed to remove listener");

            NativeMemory.Free(this.nativeListener);

            this.nativeListener = null;
        }
    }

    private T* Bind<T>(uint32_t name, wl_interface* @interface, uint32_t version) where T : unmanaged =>
        (T*)PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            WL_REGISTRY_BIND,
            @interface,
            version,
            0,
            name,
            @interface->name,
            version,
            null
        );

    public Compositor BindCompositor(uint name, uint version) =>
        new(this.Bind<wl_compositor>(name, ClientProtocol.wl_compositor_interface, version));

    public XdgWMBase BindWMBase(uint name, uint version) =>
        new(this.Bind<xdg_wm_base>(name, XdgShellProtocol.xdg_wm_base_interface, version));
}
