using Age.Numerics;
using Age.Platforms.Display;

namespace Age;

file struct KeyState(Key key, ulong iteration)
{
    public ulong Iteration = iteration;
    public Key   Key       = key;
}

public static class Input
{
    private static readonly Dictionary<Key, ulong>         keys         = [];
    private static readonly Dictionary<MouseButton, ulong> mouseButtons = [];

    private static uint          currentIteration;
    private static Point<ushort> mousePosition;
    private static float         mouseWheel;
    private static Point<ushort> previousMousePosition;
    public static MouseButton PrimaryButton { get; private set; }

    private static void OnKeyDown(Key key) =>
        keys.TryAdd(key, currentIteration);

    private static void OnKeyUp(Key key) =>
        keys.Remove(key);

    private static void OnMouseDown(in WindowMouseEvent mouseEvent)
    {
        PrimaryButton = mouseEvent.PrimaryButton;

        mouseButtons.TryAdd(mouseEvent.Button, currentIteration);
    }

    private static void OnMouseMove(in WindowMouseEvent mouseEvent)
    {
        previousMousePosition = mousePosition;
        mousePosition = new(mouseEvent.X, mouseEvent.Y);
    }

    private static void OnMouseUp(in WindowMouseEvent mouseEvent) =>
        mouseButtons.Remove(mouseEvent.Button);

    private static void OnMouseWheel(in WindowMouseEvent mouseEvent) =>
        mouseWheel = mouseEvent.Delta;

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
        currentIteration++;
        mouseWheel = 0;
    }

    public static KeyStates GetModifiers()
    {
        KeyStates modifiers = default;

        if (keys.ContainsKey(Key.Shift))
        {
            modifiers |= KeyStates.Shift;
        }

        if (keys.ContainsKey(Key.Control))
        {
            modifiers |= KeyStates.Control;
        }

        // if (keys.ContainsKey(Key.Alt))
        // {
        //     modifiers |= KeyStates.Alt;
        // }

        return modifiers;
    }

    public static Point<ushort> GetMousePosition() =>
        mousePosition;

    public static Point<short> GetMouseDeltaPosition() =>
        mousePosition.Cast<short>() - previousMousePosition.Cast<short>();

    public static float GetMouseWheel() =>
        mouseWheel;

    public static bool IsKeyJustPressed(Key key) =>
        keys.TryGetValue(key, out var iteration) && currentIteration == iteration;

    public static bool IsKeyPressed(Key key) =>
        keys.TryGetValue(key, out var iteration) && iteration > 0;

    public static bool IsKeyReleased(Key key) =>
        keys.TryGetValue(key, out var iteration) && iteration == 0;

    public static bool IsMouseButtonJustPressed(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && currentIteration == iteration;

    public static bool IsMouseButtonPressed(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && iteration > 0;

    public static bool IsMouseButtonReleased(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && iteration == 0;
}
