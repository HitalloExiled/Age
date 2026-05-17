using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;

namespace Age.Platforms.Display;

public delegate void WindowMouseEventHandler(in WindowMouseEvent mouseEvent);
public delegate void WindowContextEventHandler(in WindowContextEvent mouseEvent);
public delegate void WindowKeyEventHandler(Key key);
public delegate void WindowInputEventHandler(char character);

public partial class Window : Disposable
{
    #region events
    public event WindowMouseEventHandler?   Click;
    public event Action?                    Closed;
    public event WindowContextEventHandler? Context;
    public event WindowMouseEventHandler?   DoubleClick;
    public event WindowInputEventHandler?   Input;
    public event WindowKeyEventHandler?     KeyDown;
    public event WindowKeyEventHandler?     KeyPress;
    public event WindowKeyEventHandler?     KeyUp;
    public event WindowMouseEventHandler?   MouseDown;
    public event WindowMouseEventHandler?   MouseMove;
    public event WindowMouseEventHandler?   MouseUp;
    public event WindowMouseEventHandler?   MouseWheel;
    public event Action?                    Resized;
    #endregion events

    private static Size<uint> defaultSize = new(800, 600);

    private readonly List<Window> children = [];

    private string title;

    internal unsafe WindowState* State { get; }

    public bool IsClosed    { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsVisible   { get; private set; } = true;

    public partial Cursor Cursor { get; set; }
    public partial string Title  { get; set; }

    public ReadOnlySpan<Window> Children => this.children.AsSpan();
    public Window?              Parent   { get; }

    public partial Size<uint> Size { get; }

    public partial Window(string? title = default, Size<uint>? size = default, Window? parent = null);

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Close();
        }
    }

    public void Close()
    {
        if (!this.IsClosed)
        {
            foreach (var child in this.children)
            {
                child.Close();
            }

            this.IsClosed = true;

            Closed?.Invoke();

            WindowManager.Instance.CloseWindow(this);

            this.Parent?.children.Remove(this);
        }
    }

    public string? GetClipboardData() =>
        WindowManager.Instance.GetClipboardData(this);

    public unsafe void DoEvents()
    {
        WindowManager.Instance.FlushWindowEvents(this);

        using var messages = this.State->GetMessages();

        if (messages.IsEmpty)
        {
            return;
        }

        WindowChanges windowChanges = default;

        foreach (var message in messages)
        {
            switch (message.Kind)
            {
                case MessageKind.Click:
                    this.Click?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.Closed:
                    this.Close();

                    return;

                case MessageKind.Context:
                    this.Context?.Invoke(message.Value.WindowContextEvent);

                    break;

                case MessageKind.CursorChanged:
                    WindowManager.Instance.UpdateCursor(this.Cursor);

                    break;

                case MessageKind.DoubleClick:
                    this.DoubleClick?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.Input:
                    this.Input?.Invoke(message.Value.Input);

                    break;

                case MessageKind.KeyPress:
                    this.KeyPress?.Invoke(message.Value.Key);

                    break;

                case MessageKind.KeyDown:
                    this.KeyDown?.Invoke(message.Value.Key);

                    break;

                case MessageKind.KeyUp:
                    this.KeyUp?.Invoke(message.Value.Key);

                    break;

                case MessageKind.MouseDown:
                    this.MouseDown?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.MouseMove:
                    this.MouseMove?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.MouseUp:
                    this.MouseUp?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.MouseWheel:
                    this.MouseWheel?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.Resized:
                    windowChanges |= WindowChanges.Size;

                    break;
            }
        }

        this.State->ClearMessages();

        if (windowChanges.HasFlags(WindowChanges.Close))
        {
            this.Close();

            return;
        }

        if (windowChanges.HasFlags(WindowChanges.Size))
        {
            this.Resized?.Invoke();
        }
    }

    public void Hide() =>
        WindowManager.Instance.HideWindow(this);

    public void Maximize() =>
        WindowManager.Instance.MaximizeWindow(this);

    public void Minimize() =>
        WindowManager.Instance.MinimizeWindow(this);

    public void Restore() =>
        WindowManager.Instance.RestoreWindow(this);

    public void SetClipboardData(string value) =>
        WindowManager.Instance.SetWindowClipboardData(this, value);

    public void Show() =>
        WindowManager.Instance.ShowWindow(this);
}
