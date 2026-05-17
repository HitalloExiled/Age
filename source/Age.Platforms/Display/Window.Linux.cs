#if LINUX
using Age.Core.Extensions;
using Age.Numerics;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    internal WindowState* State { get; }

    public partial Cursor Cursor
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            this.UpdateCursor();
        }
    }

    public partial Size<uint> Size => this.State->Size.Cast<uint>();

    public partial string Title
    {
        get => this.title;
        set => throw new NotImplementedException();
    }

    public partial nint Surface => (nint)this.State->Surface;

    public partial Window(string? title, Size<uint>? size, Window? parent)
    {
        this.title = title ?? "Untitled";
        this.State = WindowManager.Instance.CreateState(this, size ?? defaultSize);

        this.Parent?.Children.Add(this);
    }

    internal partial void UpdateCursor() => throw new NotImplementedException();

    public partial void Close()
    {
        if (!this.IsClosed)
        {
            this.IsClosed = true;

            this.Closed?.Invoke();

            foreach (var child in this.Children)
            {
                child.Close();
            }

            WindowManager.Instance.ReleaseState(this);

            this.Parent?.Children.Remove(this);
        }
    }

    public partial void DoEvents()
    {
        using var messages = this.State->GetMessages();

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
                    this.windowChanges |= WindowChanges.Close;

                    break;

                case MessageKind.Context:
                    this.Context?.Invoke(message.Value.WindowContextEvent);

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
                    this.windowChanges |= WindowChanges.Size;

                    break;
            }
        }

        this.State->ClearMessages();

        if (this.windowChanges.HasFlags(WindowChanges.Close))
        {
            this.Close();

            return;
        }

        if (this.windowChanges.HasFlags(WindowChanges.Size))
        {
            this.Resized?.Invoke();
        }

        this.windowChanges = default;
    }

    public partial string? GetClipboardData() => throw new NotImplementedException();
    public partial void Hide() => throw new NotImplementedException();
    public partial void Maximize() => throw new NotImplementedException();
    public partial void Minimize() => throw new NotImplementedException();
    public partial void Restore() => throw new NotImplementedException();
    public partial void SetClipboardData(string value) => throw new NotImplementedException();
    public partial void Show() => throw new NotImplementedException();
}
#endif
