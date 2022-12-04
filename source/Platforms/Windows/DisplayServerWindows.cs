#define VULKAN_ENABLED
#define GLES3_ENABLED

using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Error;
using Age.Core.Math;
using Age.Core.OS;
using Age.Platforms.Windows.Interop;
using Age.Servers;
using static Age.Core.Config.Macros;

namespace Age.Platforms.Windows;

#pragma warning disable IDE0051,CS0169,IDE0044,CS0414,CS0649,IDE0052,IDE0060 // TODO - Remove

internal record struct WindowData
{
    public bool Minimized { get; set; }
}

internal partial class DisplayServerWindows : DisplayServer
{
    private const int MAIN_WINDOW_ID    = 0;
    private const int INVALID_WINDOW_ID = -1;

    public static new DisplayServerWindows Singleton { get; private set; } = null!;

    private readonly bool                         metaMem;
    private readonly MouseMode                    mouseMode;
    private readonly string                       renderingDriver;
    private readonly Dictionary<Guid, WindowData> windows = new();
    private int      keyEventPos;

    private bool      altMem;
    private bool      controlMem;
    private bool      dropEvents;
    private bool      grMem;
    private bool      oldInvalid = true;
    private int       pressrc;
    private bool      shiftMem;
    private int?      hInstance;
    private bool      keepScreenOn;
    private int powerRequest;
    private VulkanContextWindows? contextVulkan;

    public override bool CanAnyWindowDraw => this.windows.Any(x => !x.Value.Minimized);

    public DisplayServerWindows(string renderingDriver, WindowMode mode, VSyncMode vsyncMode, WindowFlagsBit flags, out Vector2<int> position, Vector2<int> resolution, out ErrorType error)
    {
        position = default;
        error    = ErrorType.FAILED;

        this.mouseMode = MouseMode.MOUSE_MODE_VISIBLE;

        this.renderingDriver = renderingDriver;

        // TODO - platform\windows\display_server_windows.cpp[3798]

        this.ScreenSetKeepOn(GLOBAL_GET<bool>("display/window/energy_saving/keep_screen_on"));

        // TODO - platform\windows\display_server_windows.cpp[3808:3873]

        var hInstance = 0L;

        using (var process = Process.GetCurrentProcess())
        using (var module = process.MainModule!)
        {
            hInstance = this.hInstance ?? module.EntryPointAddress.ToInt64();
        }

        var wc = new WndClassEx
        {
            cbSize        = (uint)Marshal.SizeOf<WndClassEx>(),
            style         = User32.CS_HREDRAW | User32.CS_VREDRAW | User32.CS_OWNDC | User32.CS_DBLCLKS,
            lpfnWndProc   = WndProc,
            cbClsExtra    = 0,
            cbWndExtra    = 0,
            hInstance     = hInstance,
            // hIcon         = LoadIcon(nullptr, IDI_WINLOGO),
            hCursor       = null,
            hbrBackground = null,
            lpszMenuName  = null,
            lpszClassName = "Engine"
        };

        if (User32.RegisterClassExW(wc) == 0)
        {
            User32.MessageBox(default, "Failed To Register The Window Class.", "ERROR", User32.MB_OK | User32.MB_ICONEXCLAMATION);

            error = ErrorType.ERR_UNAVAILABLE;

            return;
        }

        // TODO - platform\windows\display_server_windows.cpp[3894]

        #if VULKAN_ENABLED
        // if (renderingDriver == "vulkan")
        {
            this.contextVulkan = new VulkanContextWindows();

            if (this.contextVulkan.Initialize() != ErrorType.OK)
            {
                this.contextVulkan = null;

                error = ErrorType.ERR_UNAVAILABLE;

                return;
            }
        }
        #endif

        // TODO ...

        Singleton = this;
    }

    private void ScreenSetKeepOn(bool enable)
    {
        if (this.keepScreenOn == enable)
        {
            return;
        }

        if (enable)
        {
            var reason = "AGE running with display/window/energy_saving/keep_screen_on = true";

            var context = new REASON_CONTEXT
            {
                Version            = WinNT.POWER_REQUEST_CONTEXT_VERSION,
                Flags              = WinNT.POWER_REQUEST_CONTEXT_SIMPLE_STRING,
                SimpleReasonString = Marshal.StringToHGlobalUni(reason),
            };

            this.powerRequest = Kernel32.PowerCreateRequest(ref context);

            if (this.powerRequest == HandleApi.INVALID_HANDLE_VALUE)
            {
                this.PrintError("Failed to enable screen_keep_on.");
                return;
            }

            if (Kernel32.PowerSetRequest(this.powerRequest, POWER_REQUEST_TYPE.PowerRequestSystemRequired))
            {
                this.PrintError("Failed to request system sleep override.");
                return;
            }

            if (Kernel32.PowerSetRequest(this.powerRequest, POWER_REQUEST_TYPE.PowerRequestDisplayRequired))
            {
                this.PrintError("Failed to request display timeout override.");
                return;
            }
        }
        else
        {
            Kernel32.PowerClearRequest(this.powerRequest, POWER_REQUEST_TYPE.PowerRequestSystemRequired);
            Kernel32.PowerClearRequest(this.powerRequest, POWER_REQUEST_TYPE.PowerRequestDisplayRequired);
            Kernel32.CloseHandle(this.powerRequest);

            this.powerRequest = -1;
        }

        this.keepScreenOn = enable;
    }

    private void PrintError(string v) => throw new NotImplementedException();

    private static DisplayServer CreateFunc(string renderingDriver, WindowMode mode, VSyncMode vsyncMode, WindowFlagsBit flags, out Vector2<int> position, Vector2<int> resolution, out ErrorType error)
    {
        var ds = new DisplayServerWindows(renderingDriver, mode, vsyncMode, flags, out position, resolution, out error);

        if (error != ErrorType.OK)
        {
            if (renderingDriver == "vulkan")
            {
                var executableName = Path.Join(AppContext.BaseDirectory, "Age.exe");

                OS.Singleton.Alert(
                    $"""
                    Your video card driver does not support the selected Vulkan version.
                    Please try updating your GPU driver or try using the OpenGL 3 driver.
                    You can enable the OpenGL 3 driver by starting the engine from the",
                    command line with the command:'.
                    {executableName} --rendering-driver opengl3'.
                    If you have updated your graphics drivers recently, try rebooting.
                    """,
                    "Unable to initialize Video driver"
                );
            }
            else
            {
                OS.Singleton.Alert(
                    """
                    Your video card driver does not support the selected OpenGL version.
                    Please try updating your GPU driver.
                    If you have updated your graphics drivers recently, try rebooting.
                    """,
                    "Unable to initialize Video driver"
                );
            }
        }
        return ds;
    }

    private static List<string> GetRenderingDriversFunc()
    {
        var drivers = new List<string>();

        #if VULKAN_ENABLED
        drivers.Add("vulkan");
        #endif

        #if GLES3_ENABLED
        drivers.Add("opengl3");
        #endif

        return drivers;
    }

    private static nint WndProc(nint hWnd, uint uMsg, nint wParam, nint lParam)
    {
        if (DisplayServer.Singleton is DisplayServerWindows ds_win)
        {
            return ds_win.WndProcHandler(hWnd, uMsg, wParam, lParam);
        }
        else
        {
            return DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }
    }

    private nint WndProcHandler(nint hWnd, uint uMsg, nint wParam, nint lParam) => throw new NotImplementedException();

    private static nint DefWindowProcW(nint hWnd, uint uMsg, nint wParam, nint lParam) => throw new NotImplementedException();
    public static void RegisterWindowsDriver() =>
        RegisterCreateFunction("windows", CreateFunc, GetRenderingDriversFunc);

    public bool WindowCanDraw(int windowId) => throw new NotImplementedException();

    public override bool HasFeature(Feature feature) => throw new NotImplementedException();
    public override void ScreenSetOrientation(ScreenOrientation orientation) => throw new NotImplementedException();
    public override bool WindowCanDraw() => this.WindowCanDraw(MAIN_WINDOW_ID);
    public override void WindowSetCurrentScreen(int initScreen) => throw new NotImplementedException();
    public override void WindowSetFlag(WindowFlags flag, bool enabled) => throw new NotImplementedException();
    public override void WindowSetMode(WindowMode wINDOW_MODE_MAXIMIZED) => throw new NotImplementedException();
    public override void WindowSetPosition(Vector2<int> position) => throw new NotImplementedException();
}
