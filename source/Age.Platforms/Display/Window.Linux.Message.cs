#if LINUX
using System.Runtime.InteropServices;

namespace Age.Platforms.Display;

public partial class Window
{
    private enum MessageKind : byte
    {
        Click,
        Closed,
        Context,
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

    private struct Message
    {
        public MessageKind  Kind;
        public MessageUnion Value;

        private Message(MessageKind kind) =>
            this.Kind = kind;

        private Message(MessageKind kind, in WindowMouseEvent mouseEvent) : this(kind) =>
            this.Value = new() { MouseEvent = mouseEvent };

        private Message(MessageKind kind, Key key) : this(kind) =>
            this.Value = new() { Key = key };

        private Message(MessageKind kind, char input) : this(kind) =>
            this.Value = new() { Input = input };

        public static Message Click(WindowMouseEvent mouseEvent) =>
            new(MessageKind.Click, mouseEvent);

        public static Message Context(WindowMouseEvent mouseEvent) =>
            new(MessageKind.Context, mouseEvent);

        public static Message Closed() =>
            new(MessageKind.Closed);

        public static Message DoubleClick(WindowMouseEvent mouseEvent) =>
            new(MessageKind.DoubleClick, mouseEvent);

        public static Message Input(char input) =>
            new(MessageKind.Input, input);

        public static Message KeyDown(Key key) =>
            new(MessageKind.KeyDown, key);

        public static Message KeyPress(Key key) =>
            new(MessageKind.KeyPress, key);

        public static Message KeyUp(Key key) =>
            new(MessageKind.KeyUp, key);

        public static Message MouseDown(in WindowMouseEvent mouseEvent) =>
            new(MessageKind.MouseDown, mouseEvent);

        public static Message MouseMove(in WindowMouseEvent mouseEvent) =>
            new(MessageKind.MouseMove, mouseEvent);

        public static Message MouseUp(in WindowMouseEvent mouseEvent) =>
            new(MessageKind.MouseUp, mouseEvent);

        public static Message MouseWheel(in WindowMouseEvent mouseEvent) =>
            new(MessageKind.MouseWheel, mouseEvent);

        public static Message Resized() =>
            new(MessageKind.Resized);
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct MessageUnion
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
}
#endif
