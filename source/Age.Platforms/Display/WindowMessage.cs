using System.Runtime.InteropServices;

namespace Age.Platforms.Display;

internal enum MessageKind : byte
{
    Click,
    Closed,
    Context,
    CursorChanged,
    DoubleClick,
    Input,
    KeyDown,
    KeyPress,
    KeyUp,
    MouseDown,
    MouseMove,
    MouseUp,
    MouseWheel,
    Resized,
}

[StructLayout(LayoutKind.Explicit)]
internal struct MessageUnion
{
    [FieldOffset(0)]
    public Key Key;

    [FieldOffset(0)]
    public char Input;

    [FieldOffset(0)]
    public WindowMouseEvent MouseEvent;

    [FieldOffset(0)]
    public WindowContextEvent WindowContextEvent;
}

internal readonly struct WindowMessage
{
    public readonly MessageKind  Kind;
    public readonly MessageUnion Value;

    private WindowMessage(MessageKind kind) =>
        this.Kind = kind;

    private WindowMessage(MessageKind kind, Key key) : this(kind) =>
        this.Value = new() { Key = key };

    private WindowMessage(MessageKind kind, char input) : this(kind) =>
        this.Value = new() { Input = input };

    private WindowMessage(MessageKind kind, in WindowMouseEvent mouseEvent) : this(kind) =>
        this.Value = new() { MouseEvent = mouseEvent };

    private WindowMessage(MessageKind kind, in WindowContextEvent windowContextEvent) : this(kind) =>
        this.Value = new() { WindowContextEvent = windowContextEvent };

    public static WindowMessage Click(WindowMouseEvent mouseEvent) =>
        new(MessageKind.Click, mouseEvent);

    public static WindowMessage Context(WindowContextEvent mouseEvent) =>
        new(MessageKind.Context, mouseEvent);

    public static WindowMessage Closed() =>
        new(MessageKind.Closed);

    internal static WindowMessage CursorChanged() =>
        new(MessageKind.CursorChanged);

    public static WindowMessage DoubleClick(WindowMouseEvent mouseEvent) =>
        new(MessageKind.DoubleClick, mouseEvent);

    public static WindowMessage Input(char input) =>
        new(MessageKind.Input, input);

    public static WindowMessage KeyDown(Key key) =>
        new(MessageKind.KeyDown, key);

    public static WindowMessage KeyPress(Key key) =>
        new(MessageKind.KeyPress, key);

    public static WindowMessage KeyUp(Key key) =>
        new(MessageKind.KeyUp, key);

    public static WindowMessage MouseDown(in WindowMouseEvent mouseEvent) =>
        new(MessageKind.MouseDown, mouseEvent);

    public static WindowMessage MouseMove(in WindowMouseEvent mouseEvent) =>
        new(MessageKind.MouseMove, mouseEvent);

    public static WindowMessage MouseUp(in WindowMouseEvent mouseEvent) =>
        new(MessageKind.MouseUp, mouseEvent);

    public static WindowMessage MouseWheel(in WindowMouseEvent mouseEvent) =>
        new(MessageKind.MouseWheel, mouseEvent);

    public static WindowMessage Resized() =>
        new(MessageKind.Resized);

    public override string ToString() =>
        this.Kind.ToString();
}
