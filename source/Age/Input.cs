using Age.Numerics;
using Age.Platforms.Display;

namespace Age;

file struct KeyState(Key key, ulong iteration)
{
    public Key   Key       = key;
    public ulong Iteration = iteration;
}

public static class Input
{
    private static readonly Dictionary<Key, ulong> keys = [];
    private static readonly Dictionary<MouseButton, ulong> mouseButtons = [];

    private static uint          currentIteration;
    private static Point<ushort> mousePosition;
    private static float         mouseWheel;

    private static void OnKeyDown(Key key) =>
        keys.TryAdd(key, currentIteration);

    private static void OnKeyUp(Key key) =>
        keys.Remove(key);

    private static void OnMouseDown(in MouseEvent mouseEvent) =>
        mouseButtons.TryAdd(mouseEvent.Button, currentIteration);

    private static void OnMouseMove(in MouseEvent mouseEvent) =>
        mousePosition = new(mouseEvent.X, mouseEvent.Y);

    private static void OnMouseUp(in MouseEvent mouseEvent) =>
        mouseButtons.Remove(mouseEvent.Button);

    private static void OnMouseWheel(in MouseEvent mouseEvent) =>
        mouseWheel = mouseEvent.Delta;

    internal static void ListenInputEvents(Window window)
    {
        window.MouseDown  += OnMouseDown;
        window.MouseMove  += OnMouseMove;
        window.MouseUp    += OnMouseUp;
        window.KeyDown    += OnKeyDown;
        window.KeyUp      += OnKeyUp;
        window.MouseWhell += OnMouseWheel;
    }

    internal static void UnlistenInputEvents(Window window)
    {
        window.MouseDown  -= OnMouseDown;
        window.MouseMove  -= OnMouseMove;
        window.MouseUp    -= OnMouseUp;
        window.KeyDown    -= OnKeyDown;
        window.KeyUp      -= OnKeyUp;
        window.MouseWhell -= OnMouseWheel;
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

    public static bool IsKeyJustPressed(Key key) =>
        keys.TryGetValue(key, out var iteration) && currentIteration - iteration == 0;

    public static bool IsKeyPressed(Key key) =>
        keys.TryGetValue(key, out var iteration) && iteration > 0;

    public static bool IsKeyReleased(Key key) =>
        keys.TryGetValue(key, out var iteration) && iteration == 0;

    public static bool IsMouseButtonJustPressed(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && currentIteration - iteration == 0;

    public static bool IsMouseButtonPressed(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && iteration > 0;

    public static bool IsMouseButtonReleased(MouseButton mouseButton) =>
        mouseButtons.TryGetValue(mouseButton, out var iteration) && iteration == 0;

    public static Point<ushort> GetMousePosition() =>
        mousePosition;

    public static float GetMouseWheel() =>
        mouseWheel;
}
