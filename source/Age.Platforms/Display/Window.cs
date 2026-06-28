using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;

namespace Age.Platforms.Display;

public delegate void WindowMouseEventHandler(in WindowMouseEvent mouseEvent);
public delegate void WindowContextEventHandler(in WindowContextEvent mouseEvent);
public delegate void WindowKeyEventHandler(in WindowKeyEvent keyEvent);
public delegate void WindowInputEventHandler(char character);

public unsafe partial class Window : Disposable
{
    #region events
    public event WindowMouseEventHandler?   Click;
    public event Action?                    Closed;
    public event WindowContextEventHandler? Context;
    public event WindowMouseEventHandler?   DoubleClick;
    public event Action?                    FocusIn;
    public event Action?                    FocusOut;
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

    internal WindowState* State { get; }

    public static Cursor Cursor
    {
        get => WindowManager.Instance.Cursor;
        set => WindowManager.Instance.Cursor = value;
    }

    public bool IsClosed    { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsVisible   { get; private set; } = true;

    public string Title
    {
        get => this.title;
        set
        {
            if (this.title == value)
            {
                return;
            }

            WindowManager.Instance.SetWindowTitle(this, this.title = value);
        }
    }

    public ReadOnlySpan<Window> Children => this.children.AsSpan();
    public Window?              Parent   { get; }
    public Size<uint>           Size     => this.State->Size.Cast<uint>();

    public Window(string? title, Size<uint>? size, Window? parent)
    {
        this.title  = title ?? "Untitled";
        this.Parent = parent;

        parent?.children.Add(this);

        this.State = WindowManager.Instance.CreateWindow(this.title, (size ?? defaultSize).Cast<int>(), parent);
    }

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

    public void DoEvents()
    {
        using var messages = WindowManager.Instance.FlushWindowEvents(this);

        if (messages.IsEmpty)
        {
            return;
        }

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
                    this.Context?.Invoke(message.Value.ContextEvent);

                    break;

                case MessageKind.CursorChanged:
                    WindowManager.Instance.UpdateCursor();

                    break;

                case MessageKind.DoubleClick:
                    this.DoubleClick?.Invoke(message.Value.MouseEvent);

                    break;

                case MessageKind.FocusIn:
                    this.FocusIn?.Invoke();

                    break;

                case MessageKind.FocusOut:
                    this.FocusOut?.Invoke();

                    break;

                case MessageKind.Input:
                    this.Input?.Invoke(message.Value.Input);

                    break;

                case MessageKind.KeyDown:
                    this.KeyDown?.Invoke(message.Value.KeyEvent);
                    this.KeyPress?.Invoke(message.Value.KeyEvent);

                    break;

                case MessageKind.KeyUp:
                    this.KeyUp?.Invoke(message.Value.KeyEvent);
                    this.KeyPress?.Invoke(message.Value.KeyEvent);

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
                    this.Resized?.Invoke();

                    break;
            }
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
