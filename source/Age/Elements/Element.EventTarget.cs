using Age.Core.Collections;
using Age.Elements.Events;
using Age.Platforms.Display;
using AgeInput = Age.Input;

namespace Age.Elements;

public abstract partial class Element
{
    private readonly Lock                               elementLock = new();
    private readonly KeyedList<EventProperty, Delegate> events      = [];

    private Action?              ActivatedEvent     => this.GetEvent<Action>(EventProperty.Activated);
    private MouseEventHandler?   BluredEvent        => this.GetEvent<MouseEventHandler>(EventProperty.Blured);
    private MouseEventHandler?   ClickedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Clicked);
    private ContextEventHandler? ContextEvent       => this.GetEvent<ContextEventHandler>(EventProperty.Context);
    private Action?              DeactivatedEvent   => this.GetEvent<Action>(EventProperty.Deactivated);
    private MouseEventHandler?   DoubleClickedEvent => this.GetEvent<MouseEventHandler>(EventProperty.DoubleClicked);
    private MouseEventHandler?   FocusedEvent       => this.GetEvent<MouseEventHandler>(EventProperty.Focused);
    private InputEventHandler?   InputEvent         => this.GetEvent<InputEventHandler>(EventProperty.Input);
    private KeyEventHandler?     KeyDownEvent       => this.GetEvent<KeyEventHandler>(EventProperty.KeyDown);
    private KeyEventHandler?     KeyUpEvent         => this.GetEvent<KeyEventHandler>(EventProperty.KeyUp);
    private MouseEventHandler?   MouseDownEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseDown);
    private MouseEventHandler?   MouseEnterEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseEnter);
    private MouseEventHandler?   MouseLeaveEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseLeave);
    private MouseEventHandler?   MouseMovedEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseMoved);
    private MouseEventHandler?   MouseOutEvent      => this.GetEvent<MouseEventHandler>(EventProperty.MouseOut);
    private MouseEventHandler?   MouseOverEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseOver);
    private MouseEventHandler?   MouseReleaseEvent  => this.GetEvent<MouseEventHandler>(EventProperty.MouseRelease);
    private MouseEventHandler?   MouseUpEvent       => this.GetEvent<MouseEventHandler>(EventProperty.MouseUp);
    private MouseEventHandler?   MouseWheelEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseWheel);

    public event Action? Activated
    {
        add => this.AddEvent(EventProperty.Activated, value);
        remove => this.RemoveEvent(EventProperty.Activated, value);
    }

    public event MouseEventHandler? Blured
    {
        add => this.AddEvent(EventProperty.Blured, value);
        remove => this.RemoveEvent(EventProperty.Blured, value);
    }

    public event MouseEventHandler? Clicked
    {
        add => this.AddEvent(EventProperty.Clicked, value);
        remove => this.RemoveEvent(EventProperty.Clicked, value);
    }

    public event ContextEventHandler? Context
    {
        add => this.AddEvent(EventProperty.Context, value);
        remove => this.RemoveEvent(EventProperty.Context, value);
    }

    public event Action? Deactivated
    {
        add => this.AddEvent(EventProperty.Deactivated, value);
        remove => this.RemoveEvent(EventProperty.Deactivated, value);
    }

    public event MouseEventHandler? DoubleClicked
    {
        add => this.AddEvent(EventProperty.DoubleClicked, value);
        remove => this.RemoveEvent(EventProperty.DoubleClicked, value);
    }

    public event MouseEventHandler? Focused
    {
        add => this.AddEvent(EventProperty.Focused, value);
        remove => this.RemoveEvent(EventProperty.Focused, value);
    }

    public event InputEventHandler? Input
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.Input, value, out var added);

                if (this.Scene?.Viewport?.Window is Window window && added)
                {
                    window.Input += this.OnInput;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.Input, value, out var removed);

                if (this.Scene?.Viewport?.Window is Window window && removed)
                {
                    window.Input -= this.OnInput;
                }
            }
        }
    }

    public event KeyEventHandler? KeyDown
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.KeyDown, value, out var added);

                if (this.Scene?.Viewport?.Window is Window window && added)
                {
                    window.KeyDown += this.OnKeyDown;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyDown, value, out var removed);

                if (this.Scene?.Viewport?.Window is Window window && removed)
                {
                    window.KeyDown -= this.OnKeyDown;
                }
            }
        }
    }

    public event KeyEventHandler? KeyUp
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.KeyUp, value, out var added);

                if (this.Scene?.Viewport?.Window is Window window  && added)
                {
                    window.KeyUp += this.OnKeyUp;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.KeyUp, value, out var removed);

                if (this.Scene?.Viewport?.Window is Window window && removed)
                {
                    window.KeyUp -= this.OnKeyUp;
                }
            }
        }
    }

    public event MouseEventHandler? MouseDown
    {
        add => this.AddEvent(EventProperty.MouseDown, value);
        remove => this.RemoveEvent(EventProperty.MouseDown, value);
    }

    public event MouseEventHandler? MouseEnter
    {
        add => this.AddEvent(EventProperty.MouseEnter, value);
        remove => this.RemoveEvent(EventProperty.MouseEnter, value);
    }

    public event MouseEventHandler? MouseLeave
    {
        add => this.AddEvent(EventProperty.MouseLeave, value);
        remove => this.RemoveEvent(EventProperty.MouseLeave, value);
    }

    public event MouseEventHandler? MouseMoved
    {
        add => this.AddEvent(EventProperty.MouseMoved, value);
        remove => this.RemoveEvent(EventProperty.MouseMoved, value);
    }

    public event MouseEventHandler? MouseOut
    {
        add => this.AddEvent(EventProperty.MouseOut, value);
        remove => this.RemoveEvent(EventProperty.MouseOut, value);
    }

    public event MouseEventHandler? MouseOver
    {
        add => this.AddEvent(EventProperty.MouseOver, value);
        remove => this.RemoveEvent(EventProperty.MouseOver, value);
    }

    public event MouseEventHandler? MouseUp
    {
        add => this.AddEvent(EventProperty.MouseUp, value);
        remove => this.RemoveEvent(EventProperty.MouseUp, value);
    }

    public event MouseEventHandler? MouseWheel
    {
        add
        {
            lock (this.elementLock)
            {
                this.AddEvent(EventProperty.MouseWheel, value, out var added);

                if (this.Scene?.Viewport?.Window is Window window  && added)
                {
                    window.MouseWheel += this.OnMouseWheel;
                }
            }
        }
        remove
        {
            lock (this.elementLock)
            {
                this.RemoveEvent(EventProperty.MouseWheel, value, out var removed);

                if (this.Scene?.Viewport?.Window is Window window && removed)
                {
                    window.MouseWheel -= this.OnMouseWheel;
                }
            }
        }
    }

    private void AddEvent(EventProperty key, Delegate? handler) =>
        this.AddEvent(key, handler, out _);

    private void AddEvent(EventProperty key, Delegate? handler, out bool added)
    {
        if (handler == null)
        {
            added = false;
            return;
        }

        added = !this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Combine(@delegate, handler);

        this.events[key] = @delegate;
    }

    internal void ApplyCursor() =>
        this.SetCursor(this.ComputedStyle.Cursor);

    private MouseEvent CreateEvent(in WindowMouseEvent mouseEvent, bool indirect) =>
        new()
        {
            Target        = this,
            Button        = mouseEvent.Button,
            Delta         = mouseEvent.Delta,
            KeyStates     = mouseEvent.KeyStates,
            PrimaryButton = mouseEvent.PrimaryButton,
            X             = mouseEvent.X,
            Y             = mouseEvent.Y,
            Indirect      = indirect,
        };

    private ContextEvent CreateEvent(in WindowContextEvent windowContextEvent) =>
        new()
        {
            Target  = this,
            X       = windowContextEvent.X,
            Y       = windowContextEvent.Y,
            ScreenX = windowContextEvent.ScreenX,
            ScreenY = windowContextEvent.ScreenY,
        };

    private T? GetEvent<T>(EventProperty key) where T : Delegate =>
        this.events.TryGet(key, out var @delegate) ? (T)@delegate : null;

    private void RemoveEvent(EventProperty key, Delegate? handler) =>
        this.RemoveEvent(key, handler, out _);

    private void RemoveEvent(EventProperty key, Delegate? handler, out bool removed)
    {
        if (handler == null)
        {
            removed = false;
            return;
        }

        this.events.TryGet(key, out var @delegate);

        @delegate = Delegate.Remove(@delegate, handler);

        if (removed = @delegate == null)
        {
            this.events.Remove(key);
        }
    }

    private void OnInput(char character)
    {
        if (this.IsFocused)
        {
            this.InputEvent?.Invoke(character);
        }
    }

    private void OnKeyDown(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key       = key,
                Holding   = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.KeyDownEvent?.Invoke(keyEvent);
        }
    }

    private void OnMouseWheel(in WindowMouseEvent windowMouseEvent)
    {
        if (this.IsScrollable)
        {
            var mouseEvent = this.CreateEvent(windowMouseEvent, false);

            this.MouseWheelEvent?.Invoke(mouseEvent);
        }
    }

    private void OnKeyUp(Key key)
    {
        if (this.IsFocused)
        {
            var keyEvent = new KeyEvent
            {
                Key       = key,
                Holding   = !AgeInput.IsKeyJustPressed(key),
                Modifiers = AgeInput.GetModifiers(),
            };

            this.KeyUpEvent?.Invoke(keyEvent);
        }
    }

    internal void InvokeActivate()
    {
        this.AddState(State.Active);
        this.ActivatedEvent?.Invoke();
    }

    internal void InvokeBlur(in WindowMouseEvent windowMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.RemoveState(State.Focused);

            this.BluredEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));
        }
    }

    internal void InvokeClick(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.ClickedEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeContext(in WindowContextEvent windowContextEvent) =>
        this.ContextEvent?.Invoke(this.CreateEvent(windowContextEvent));

    internal void InvokeDeactivate()
    {
        this.RemoveState(State.Active);
        this.DeactivatedEvent?.Invoke();
    }

    internal void InvokeDoubleClick(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.DoubleClickedEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeFocus(in WindowMouseEvent windowMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.AddState(State.Focused);

            this.FocusedEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));
        }
    }

    internal void InvokeMouseDown(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.MouseDownEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeMouseEnter(in WindowMouseEvent windowMouseEvent)
    {
        this.MouseEnterEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));

        if (this.CanScroll)
        {
            this.HandleScrollBarMouseEnter(windowMouseEvent);
        }
    }

    internal void InvokeMouseLeave(in WindowMouseEvent windowMouseEvent)
    {
        this.MouseLeaveEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));

        if (this.CanScroll)
        {
            this.HandleScrollBarMouseLeave();
        }
    }

    internal void InvokeMouseMoved(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.MouseMovedEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeMouseOut(in WindowMouseEvent windowMouseEvent)
    {
        if (!IsSelectingText)
        {
            this.RemoveState(State.Hovered);
        }

        this.MouseOutEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));
    }

    internal void InvokeMouseOver(in WindowMouseEvent windowMouseEvent)
    {
        if (!IsSelectingText)
        {
            this.AddState(State.Hovered);
            this.ApplyCursor();
        }

        this.MouseOverEvent?.Invoke(this.CreateEvent(windowMouseEvent, false));
    }

    internal void InvokeMouseRelease(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.MouseReleaseEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeMouseUp(in WindowMouseEvent windowMouseEvent, bool indirect) =>
        this.MouseUpEvent?.Invoke(this.CreateEvent(windowMouseEvent, indirect));

    internal void InvokeMouseWheel(in WindowMouseEvent mouseEvent)
    {
        this.MouseWheelEvent?.Invoke(this.CreateEvent(mouseEvent, false));

        this.HandleMouseWheel(mouseEvent);
    }

    public void Blur()
    {
        this.RemoveState(State.Focused);
        this.BluredEvent?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.AddState(State.Active);
        this.ClickedEvent?.Invoke(new() { Target = this });
    }

    public void Focus()
    {
        this.AddState(State.Focused);
        this.FocusedEvent?.Invoke(new() { Target = this });
    }
}
