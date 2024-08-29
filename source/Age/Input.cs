using Age.Platforms.Display;

namespace Age;

internal struct KeyState(Key key, ulong iteration)
{
    public Key   Key       = key;
    public ulong Iteration = iteration;
}

public static class Input
{
    private static readonly Dictionary<Key, ulong> keyStates = [];
    private static readonly Dictionary<MouseButton, ulong> mouseButtonsStates = [];
    private static float mouseWheel;

    private static uint currentIteration;

    private static void OnKeyDown(Key key) =>
        keyStates.TryAdd(key, currentIteration);

    private static void OnKeyUp(Key key) =>
        keyStates[key] = 0;

    private static void OnMouseDown(in MouseEvent eventArgs) =>
        mouseButtonsStates.TryAdd(eventArgs.Button, currentIteration);

    private static void OnClickUp(in MouseEvent eventArgs) =>
        mouseButtonsStates[eventArgs.Button] = 0;

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
        foreach (var (key, value) in keyStates.ToArray())
        {
            if (value == 0)
            {
                keyStates.Remove(key);
            }
        }

        foreach (var (key, value) in mouseButtonsStates.ToArray())
        {
            if (value == 0)
            {
                mouseButtonsStates.Remove(key);
            }
        }

        currentIteration++;
        mouseWheel = 0;
    }

    public static bool IsKeyJustPressed(Key key) =>
        keyStates.TryGetValue(key, out var iteration) && currentIteration - iteration == 0;

    public static bool IsKeyPressed(Key key) =>
        keyStates.TryGetValue(key, out var iteration) && iteration > 0;

    public static bool IsKeyReleased(Key key) =>
        keyStates.TryGetValue(key, out var iteration) && iteration == 0;

    public static bool IsMouseButtonJustPressed(MouseButton mouseButton) =>
        mouseButtonsStates.TryGetValue(mouseButton, out var iteration) && currentIteration - iteration == 0;

    public static bool IsMouseButtonPressed(MouseButton mouseButton) =>
        mouseButtonsStates.TryGetValue(mouseButton, out var iteration) && iteration > 0;

    public static bool IsMouseButtonReleased(MouseButton mouseButton) =>
        mouseButtonsStates.TryGetValue(mouseButton, out var iteration) && iteration == 0;

    public static float GetMouseWheel() => mouseWheel;

}
