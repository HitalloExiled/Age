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
    private static float mouseWheel;

    private static uint currentIteration;

    private static void OnKeyDown(Key key) =>
        keys.TryAdd(key, currentIteration);

    private static void OnKeyUp(Key key) =>
        keys.Remove(key);

    private static void OnMouseDown(in MouseEvent eventArgs) =>
        mouseButtons.TryAdd(eventArgs.Button, currentIteration);

    private static void OnClickUp(in MouseEvent eventArgs) =>
        mouseButtons.Remove(eventArgs.Button);

    private static void OnMouseWheel(in MouseEvent eventArgs) =>
        mouseWheel = eventArgs.Delta;

    internal static void ListenInputEvents(Window window)
    {
        window.MouseDown  += OnMouseDown;
        window.MouseUp    += OnClickUp;
        window.KeyDown    += OnKeyDown;
        window.KeyUp      += OnKeyUp;
        window.MouseWhell += OnMouseWheel;
    }

    internal static void UnlistenInputEvents(Window window)
    {
        window.MouseDown  -= OnMouseDown;
        window.MouseUp    -= OnClickUp;
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

    public static float GetMouseWheel() => mouseWheel;

}
