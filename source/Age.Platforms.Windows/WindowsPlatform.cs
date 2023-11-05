using Age.Numerics;
using Age.Platforms.Windows.Display;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;
using Age.Platforms.Windows.Vulkan;
using Age.Rendering.Vulkan.Handlers;

namespace Age.Platforms.Windows;

public class WindowsPlatform : Platform, IDisposable
{
    private static WindowsPlatform singleton = null!;

    private readonly Dictionary<HWND, Window> windows = [];
    private readonly WindowsVulkanLoader windowsVulkanLoader = new();

    private bool disposed;

    private readonly Window mainWindow;
    private readonly WindowHandler mainWindowHandler;

    public override WindowsVulkanRenderer Renderer { get; }
    public override bool CanDraw => !this.mainWindow.Minimized && !this.mainWindow.Closed;

    public unsafe WindowsPlatform()
    {
        singleton = this;

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

            this.mainWindow = this.CreateWindow("Age", 600, 400, 800, 300);

            this.Renderer = new WindowsVulkanRenderer(new(this.windowsVulkanLoader));

            this.mainWindowHandler = this.Renderer.CreateWindow(this.mainWindow.Handle, this.mainWindow.ClientSize, true);

            this.mainWindow.SizeChanged += () => this.mainWindowHandler.Size = this.mainWindow.ClientSize;
        }
    }

    private static Size<uint> GetClientSize(HWND hwnd)
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

                        var size = GetClientSize(hwnd);

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

                    if (singleton.mainWindow.Closed)
                    {
                        singleton.QuitRequested = true;
                    }

                    return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
                default:
                    break;
            }
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
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

                this.Renderer.FreeWindow(this.mainWindowHandler);

                this.Renderer.Dispose();
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

    public Window CreateWindow(string title, uint width, uint height, int x, int y, Window? parent = null)
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

        var window = new Window(hwnd, title, new(width, height), new(x, y), parent);

        window.Destroyed += this.DestroyWindow;

        this.windows[hwnd] = window;

        return window;
    }

    public void DestroyWindow(Window window)
    {
        window.Destroyed -= this.DestroyWindow;

        window.Close();

        this.windows.Remove(window.Handle);

        User32.DestroyWindow(window.Handle);
    }

    public override void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
