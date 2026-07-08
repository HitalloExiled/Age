using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age;

public static class Input
{
    private static readonly Dictionary<MouseButton, ulong> pressedMouseButtons = [];
    private static readonly Dictionary<Key, ulong>         pressedKeys  = [];
    private static readonly HashSet<Key>                   releasedKeys = [];

    private static ulong         iteration;
    private static Point<ushort> mousePosition;
    private static float         mouseWheel;
    private static MouseButton   mousePressedButtons;
    private static Point<ushort> previousMousePosition;
    private static MouseButton   releasedMouseButtons;

    public static bool LeftHanded { get; private set; }

    private static void OnKeyDown(in WindowKeyEvent windowKeyEvent)
    {
        if (!windowKeyEvent.Echo)
        {
            pressedKeys.TryAdd(windowKeyEvent.Key, iteration);
        }
    }

    private static void OnKeyUp(in WindowKeyEvent windowKeyEvent)
    {
        if (!windowKeyEvent.Echo)
        {
            pressedKeys.Remove(windowKeyEvent.Key);
            releasedKeys.Add(windowKeyEvent.Key);
        }
    }

    private static void OnMouseDown(in WindowMouseEvent mouseEvent)
    {
        LeftHanded          = mouseEvent.LeftHanded;
        mousePressedButtons = mouseEvent.PressedButtons;

        pressedMouseButtons.TryAdd(mouseEvent.Button, iteration);
    }

    private static void OnMouseMove(in WindowMouseEvent mouseEvent)
    {
        previousMousePosition = mousePosition;
        mousePosition         = new(mouseEvent.X, mouseEvent.Y);
    }

    private static void OnMouseUp(in WindowMouseEvent mouseEvent)
    {
        mousePressedButtons = mouseEvent.PressedButtons;

        pressedMouseButtons.Remove(mouseEvent.Button);

        releasedMouseButtons |= mouseEvent.Button;
    }

    private static void OnMouseWheel(in WindowMouseEvent mouseEvent) =>
        mouseWheel = mouseEvent.ScrollDelta;

    internal static void ListenInputEvents(Window window)
    {
        window.MouseDown  += OnMouseDown;
        window.MouseMove  += OnMouseMove;
        window.MouseUp    += OnMouseUp;
        window.KeyDown    += OnKeyDown;
        window.KeyUp      += OnKeyUp;
        window.MouseWheel += OnMouseWheel;
    }

    internal static void UnlistenInputEvents(Window window)
    {
        window.MouseDown  -= OnMouseDown;
        window.MouseMove  -= OnMouseMove;
        window.MouseUp    -= OnMouseUp;
        window.KeyDown    -= OnKeyDown;
        window.KeyUp      -= OnKeyUp;
        window.MouseWheel -= OnMouseWheel;
    }

    internal static void Update()
    {
        iteration++;
        mouseWheel = 0;

        releasedKeys.Clear();
        releasedMouseButtons = default;
    }

    public static Modifier GetModifiers()
    {
        Modifier modifiers = default;

        if (pressedKeys.ContainsKey(Key.Shift))
        {
            modifiers |= Modifier.Shift;
        }

        if (pressedKeys.ContainsKey(Key.Ctrl))
        {
            modifiers |= Modifier.Ctrl;
        }

        if (pressedKeys.ContainsKey(Key.Alt))
        {
            modifiers |= Modifier.Alt;
        }

        if (pressedKeys.ContainsKey(Key.Meta))
        {
            modifiers |= Modifier.Meta;
        }

        return modifiers;
    }

    public static Point<ushort> GetMousePosition() =>
        mousePosition;

    public static MouseButton GetMousePressedButtons() =>
        mousePressedButtons;

    public static Point<short> GetMouseDeltaPosition() =>
        mousePosition.Cast<short>() - previousMousePosition.Cast<short>();

    public static float GetMouseWheel() =>
        mouseWheel;

    public static bool IsKeyJustPressed(Key key) =>
        pressedKeys.TryGetValue(key, out var keyIteration) && keyIteration == iteration;

    public static bool IsKeyJustReleased(Key key) =>
        releasedKeys.Contains(key);

    public static bool IsKeyPressed(Key key) =>
        pressedKeys.ContainsKey(key);

    public static bool IsMouseButtonPressed(MouseButton mouseButton) =>
        pressedMouseButtons.ContainsKey(mouseButton);

    public static bool IsMouseButtonJustPressed(MouseButton mouseButton) =>
        pressedMouseButtons.TryGetValue(mouseButton, out var mouseButtonIteration) && mouseButtonIteration == iteration;

    public static bool IsMouseButtonJustReleased(MouseButton mouseButton) =>
        releasedMouseButtons.HasFlags(mouseButton);
}
