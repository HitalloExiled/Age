#if LINUX
using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using static Age.Platforms.Linux.LibDecor.LibDecor;
using static Age.Platforms.Linux.Wayland.ViewporterProtocol;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    internal WindowState* State { get; }

    public partial Size<uint> ClientSize => this.State->Size.Cast<uint>();

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

    public partial Point<int> Position
    {
        get => this.State->Position;
        set => throw new NotImplementedException();
    }

    public partial Size<uint> Size
    {
        get => this.State->Size.Cast<uint>();
        set => UpdateSize(this.State, value.Cast<int>());
    }

    public partial string Title
    {
        get => this.title;
        set => throw new NotImplementedException();
    }

    public partial nint Surface => (nint)this.State->Surface;

    public partial Window(string? title, Size<uint>? size, Point<int>? position, Window? parent)
    {
        this.title = title ?? "Untitled";
        this.State = WindowManager.Instance.CreateState(this, (size ?? defaultSize), position ?? default);

        this.Parent?.Children.Add(this);
    }

    internal static void UpdateSize(WindowState* state, Size<int> size)
    {
        var sizeHasChanged = false;

        if (state->Size != size)
        {
            state->Size = size;

            sizeHasChanged = true;
        }

        if (state->Surface != null && state->Viewport != null)
        {
            wp_viewport_set_destination(state->Viewport, size.Width, size.Height);
        }

        var libdecorState = libdecor_state_new(size.Width, size.Height);

        libdecor_frame_commit(state->Frame, libdecorState, state->PendingLibdecorConfiguration);
        libdecor_state_free(libdecorState);

        if (sizeHasChanged)
        {
            state->Messages.Add(WindowMessage.Resized());
        }

        state->PendingLibdecorConfiguration = null;
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
        using var _ = UnsafeLock.Lock(ref this.State->Lock);

        this.windowChanges = default;

        foreach (var message in this.State->Messages)
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

        this.State->Messages.Clear();

        if (this.windowChanges.HasFlags(WindowChanges.Close))
        {
            this.Close();

            return;
        }

        if (this.windowChanges.HasFlags(WindowChanges.Size))
        {
            this.Resized?.Invoke();
        }
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
