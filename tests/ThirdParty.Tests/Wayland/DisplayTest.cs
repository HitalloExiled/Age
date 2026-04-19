#if LINUX

using System.Text;
using ThirdParty.Wayland;

namespace ThirdParty.Tests.Wayland;

public class DisplayTest
{
    [Fact]
    public void Create()
    {
        using var display  = new Display();
        using var registry = display.Registry;

        Compositor? compositor = null;
        XdgWMBase?  xdgWmBase     = null;

        registry.GlobalAdded += (name, @namespace, version) =>
        {
            if (@namespace.SequenceEqual("wl_compositor"u8))
            {
                compositor = registry.BindCompositor(name, Math.Clamp(version, 1, 6));
            }
            else if (@namespace.SequenceEqual("xdg_wm_base"u8))
            {
                xdgWmBase = registry.BindWMBase(name, Math.Clamp(version, 1, 6));
            }
        };

        registry.GlobalRemoved += static (name) => Console.WriteLine($"registry.GlobalRemoved - name: {name}");

        display.RoundTrip();

        Assert.NotNull(compositor);
        Assert.NotNull(xdgWmBase);

        using var surface    = compositor.CreateSurface();
        using var xdgSurface = xdgWmBase.CreateSurface(surface);
        using var topLevel   = xdgSurface.GetTopLevel();

        var configured = false;

        xdgSurface.Configured += (serial) =>
        {
            xdgSurface.AckConfigure(serial);
            configured = true;
        };

        var closed = false;

        topLevel.Configured       += static (width, height, states) => Console.WriteLine($"topLevel.Configured - width: {width}, height: {height}, states: {toString(states)}");
        topLevel.ConfiguredBounds += static (width, height) => Console.WriteLine($"topLevel.ConfiguredBounds - width: {width}, height: {height}");
        topLevel.WMCapabilities   += static (capabilities) => Console.WriteLine($"topLevel.WMCapabilities - capabilities: {toString(capabilities)}");
        topLevel.Closed           += () => closed = true;

        topLevel.SetTitle("AGE Engine");
        topLevel.SetAppId("age.engine.bootstrap");

        surface.Commit();

        display.RoundTrip();

        while (!closed)
        {
            while (display.PrepareRead())
            {
                display.DispatchPending();
            }

            display.Flush();

            if (configured)
            {
                configured = false;

                surface.Commit();
            }

            display.ReadEvents();
            display.DispatchPending();
        }

        xdgWmBase?.Dispose();
        compositor?.Dispose();

        static string toString(ReadOnlySpan<uint> values)
        {
            var builder = new StringBuilder();

            builder.Append('[');

            for (var i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(values[i].ToString());
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}

#endif
