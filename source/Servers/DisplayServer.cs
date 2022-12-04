using Age.Core.Error;
using Age.Core.Math;
using Age.Platforms.Windows;
using static Age.Core.Error.ErrorMacro;

namespace Age.Servers;

internal record DisplayServerCreate(
    string                           Name,
    DisplayServer.CreateFuncDelegate CreateFunction,
    Func<List<string>>               GetRenderingDriversFunction
);

internal abstract class DisplayServer
{
    private const int MAX_SERVERS = 64;
    public delegate DisplayServer CreateFuncDelegate(string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, out ErrorType err);
    public enum Feature
    {
		FEATURE_GLOBAL_MENU,
		FEATURE_SUBWINDOWS,
		FEATURE_TOUCHSCREEN,
		FEATURE_MOUSE,
		FEATURE_MOUSE_WARP,
		FEATURE_CLIPBOARD,
		FEATURE_VIRTUAL_KEYBOARD,
		FEATURE_CURSOR_SHAPE,
		FEATURE_CUSTOM_CURSOR_SHAPE,
		FEATURE_NATIVE_DIALOG,
		FEATURE_IME,
		FEATURE_WINDOW_TRANSPARENCY,
		FEATURE_HIDPI,
		FEATURE_ICON,
		FEATURE_NATIVE_ICON,
		FEATURE_ORIENTATION,
		FEATURE_SWAP_BUFFERS,
		FEATURE_KEEP_SCREEN_ON,
		FEATURE_CLIPBOARD_PRIMARY,
		FEATURE_TEXT_TO_SPEECH,
		FEATURE_EXTEND_TO_TITLE,
	};

    public enum MouseMode
    {
		MOUSE_MODE_VISIBLE,
		MOUSE_MODE_HIDDEN,
		MOUSE_MODE_CAPTURED,
		MOUSE_MODE_CONFINED,
		MOUSE_MODE_CONFINED_HIDDEN,
	};

    public enum ScreenOrientation
    {
        NONE = -1,
		SCREEN_LANDSCAPE,
		SCREEN_PORTRAIT,
		SCREEN_REVERSE_LANDSCAPE,
		SCREEN_REVERSE_PORTRAIT,
		SCREEN_SENSOR_LANDSCAPE,
		SCREEN_SENSOR_PORTRAIT,
		SCREEN_SENSOR,
	};

    public enum VSyncMode
    {
        NONE = -1,
		VSYNC_DISABLED,
		VSYNC_ENABLED,
		VSYNC_ADAPTIVE,
		VSYNC_MAILBOX
	};

    public enum WindowFlags
    {
		WINDOW_FLAG_RESIZE_DISABLED,
		WINDOW_FLAG_BORDERLESS,
		WINDOW_FLAG_ALWAYS_ON_TOP,
		WINDOW_FLAG_TRANSPARENT,
		WINDOW_FLAG_NO_FOCUS,
		WINDOW_FLAG_POPUP,
		WINDOW_FLAG_EXTEND_TO_TITLE,
		WINDOW_FLAG_MAX,
	};

    [Flags]
    public enum WindowFlagsBit
    {
        NONE = -1,
		WINDOW_FLAG_RESIZE_DISABLED_BIT = 1,
		WINDOW_FLAG_BORDERLESS_BIT      = 2,
		WINDOW_FLAG_ALWAYS_ON_TOP_BIT   = 4,
		WINDOW_FLAG_TRANSPARENT_BIT     = 8,
		WINDOW_FLAG_NO_FOCUS_BIT        = 16,
		WINDOW_FLAG_POPUP_BIT           = 32,
		WINDOW_FLAG_EXTEND_TO_TITLE_BIT = 64,
	};

    public enum WindowMode
    {
        NONE = -1,
		WINDOW_MODE_WINDOWED,
		WINDOW_MODE_MINIMIZED,
		WINDOW_MODE_MAXIMIZED,
		WINDOW_MODE_FULLSCREEN,
		WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
	};

    public static DisplayServer Singleton => DisplayServerWindows.Singleton;

    public abstract bool CanAnyWindowDraw { get; }

    public static List<DisplayServerCreate> ServerCreateFunctions { get; } = new()
    {
        new DisplayServerCreate("headless", DisplayServerHeadless.CreateFunc, DisplayServerHeadless.GetRenderingDriversFunc),
    };

    protected static void RegisterCreateFunction(string name, CreateFuncDelegate createFunc, Func<List<string>> getRenderingDriversFunc)
    {
        if (ERR_FAIL_COND(ServerCreateFunctions.Count == MAX_SERVERS))
        {
            return;
        }

        ServerCreateFunctions.Add(ServerCreateFunctions.Last());

        ServerCreateFunctions[^2] = (new (name, createFunc, getRenderingDriversFunc));
    }

    public static DisplayServer? Create(int index, string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, out ErrorType err)
    {
        if (ERR_FAIL_INDEX_V(index, ServerCreateFunctions.Count))
        {
            windowPosition = default;
            err            = ErrorType.FAILED;

            return null;
        }

        return ServerCreateFunctions[index].CreateFunction(
            renderingDriver,
            windowMode,
            windowVsyncMode,
            windowFlags,
            out windowPosition,
            windowSize,
            out err
        );
    }

    public abstract bool WindowCanDraw();
    public abstract void WindowSetMode(WindowMode wINDOW_MODE_MAXIMIZED);
    public abstract void WindowSetCurrentScreen(int initScreen);
    public abstract void WindowSetFlag(WindowFlags flag, bool enabled);
    public abstract void WindowSetPosition(Vector2<int> position);
    public abstract bool HasFeature(Feature feature);
    public abstract void ScreenSetOrientation(ScreenOrientation orientation);
}
