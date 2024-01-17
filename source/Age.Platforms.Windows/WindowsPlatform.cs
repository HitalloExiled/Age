using Age.Numerics;
using Age.Platforms.Windows.Display;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;
using Age.Platforms.Windows.Vulkan;
using Age.Rendering.Vulkan;
using Age.Vulkan.Native;

namespace Age.Platforms.Windows;

public class WindowsPlatform : Platform, IDisposable
{
    private static WindowsPlatform singleton = null!;

    private readonly Dictionary<HWND, Window> windows = [];
    private readonly WindowsVulkanLoader windowsVulkanLoader = new();

    private bool disposed;

    private readonly WindowsVulkanContext rendererContext;

    public override VulkanRenderer Renderer { get; }

    public override bool CanDraw => this.windows.Values.Any(x => !x.Closed && !x.Minimized);

    public unsafe WindowsPlatform()
    {
        singleton = this;

        var vk = new Vk(this.windowsVulkanLoader);

        this.rendererContext = new WindowsVulkanContext(vk);
        this.Renderer        = new(vk, this.rendererContext);

        fixed (char* lpszClassName = "Engine")
        {
            var windowClass = new User32.WNDCLASSEXW
            {
                cbSize        = (uint)sizeof(User32.WNDCLASSEXW),
                hbrBackground = default,
                hCursor       = User32.LoadCursorW(default, User32.IDC_STANDARD_CURSORS.IDC_ARROW),
                hIcon         = default,
                hIconSm       = default,
                hInstance     = default,
                lpszClassName = lpszClassName,
                lpszMenuName  = null,
                style         = 0,
                lpfnWndProc   = new(WndProc),
            };

            if (User32.RegisterClassExW(windowClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }
        }
    }

    private static Size<uint> GetClientSize(HWND hwnd)
    {
        User32.GetClientRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    private static Size<uint> GetWindowSize(HWND hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    private static LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (singleton.windows.TryGetValue(hwnd, out var window))
        {
            switch (msg)
            {
                case User32.WINDOW_MESSAGE.WM_SIZE:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.SetMaximized(placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMAXIMIZED);
                        window.SetMinimized(placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMINIMIZED);

                        var size = GetWindowSize(hwnd);

                        if (size.Width != window.Size.Width || size.Height != window.Size.Height)
                        {
                            window.Size = size;

                            window.NotifySizeChanged();
                        }
                    }

                    return 0;
                case User32.WINDOW_MESSAGE.WM_MOVING:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.Position = new(placement.rcNormalPosition.left, placement.rcNormalPosition.top);
                    }

                    return 0;
                case User32.WINDOW_MESSAGE.WM_CLOSE:
                    window.Close();

                    return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
                default:
                    break;
            }
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    private Window CreateWindow(string title, uint width, uint height, int x, int y, Window? parent = null)
    {
        var hwnd = User32.CreateWindowExW(
            User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
            "Engine",
            title,
            User32.WINDOW_STYLES.WS_VISIBLE | User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW,
            x,
            y,
            (int)width,
            (int)height,
            parent?.Handle ?? default,
            default,
            default,
            0
        );

        if (hwnd == default)
        {
            throw new Exception("Failed to create window on Windows OS.");
		}

        var clientSize = GetClientSize(hwnd);

        var windowContext = this.rendererContext.CreateWindow(hwnd, clientSize);

        var window = new Window(hwnd, windowContext, title, new(width, height), new(x, y), parent);

        window.SizeChanged += () => window.Context.Size = window.ClientSize;

        window.Destroyed += this.DestroyWindow;

        this.windows[hwnd] = window;

        return window;
    }

    private void DestroyWindow(Window window)
    {
        window.Destroyed -= this.DestroyWindow;

        window.Close();

        this.windows.Remove(window.Handle);
        this.rendererContext.DestroyWindow(window.Context);

        User32.DestroyWindow(window.Handle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var window in this.windows.Values)
                {
                    window.Close();
                }

                this.windows.Clear();

                this.Renderer.Dispose();
                this.rendererContext.Dispose();
                this.windowsVulkanLoader.Dispose();
            }

            this.disposed = true;
        }
    }

    public override void DoEvents()
    {
        foreach (var window in this.windows.Values)
        {
            while (User32.PeekMessageW(out var msg, window.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !window.Closed)
            {
                User32.TranslateMessage(msg);
                User32.DispatchMessageW(msg);
            }
        }
    }

    public override void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override Rendering.Display.Window CreateWindow(string title, uint width, uint height, int x, int y, Rendering.Display.Window? parent = null) =>
        this.CreateWindow(title, width, height, x, y, (Window?)parent);

    public override void DestroyWindow(Rendering.Display.Window window) =>
        this.DestroyWindow((Window)window);
}
