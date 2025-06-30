using Age.Core.Collections;
using Age.Elements.Layouts;
using Age.Platforms.Display;
using Age.Scene;

using AgeInput = Age.Input;
using Key = Age.Platforms.Display.Key;
using PlatformContextEvent = Age.Platforms.Display.ContextEvent;
using PlatformMouseEvent = Age.Platforms.Display.MouseEvent;

namespace Age.Elements;

public struct KeyEvent
{
    public Key       Key;
    public KeyStates Modifiers;
    public bool      Holding;
}

public delegate void ContextEventHandler(in ContextEvent mouseEvent);
public delegate void MouseEventHandler(in MouseEvent mouseEvent);
public delegate void KeyEventHandler(in KeyEvent keyEvent);
public delegate void InputEventHandler(char keyEvent);

public abstract partial class EventTarget : Layoutable
{
    #region events

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

    public event MouseEventHandler? MouseDown
    {
        add => this.AddEvent(EventProperty.MouseDown, value);
        remove => this.RemoveEvent(EventProperty.MouseDown, value);
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

    public event InputEventHandler? Input
    {
        add
        {
            lock(this.@lock)
            {
                this.AddEvent(EventProperty.Input, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.Input += this.OnInput;
                }
            }
        }
        remove
        {
            lock(this.@lock)
            {
                this.RemoveEvent(EventProperty.Input, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.Input -= this.OnInput;
                }
            }
        }
    }

    public event KeyEventHandler? KeyDown
    {
        add
        {
            lock(this.@lock)
            {
                this.AddEvent(EventProperty.KeyDown, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyDown += this.OnKeyDown;
                }
            }
        }
        remove
        {
            lock(this.@lock)
            {
                this.RemoveEvent(EventProperty.KeyDown, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.KeyDown -= this.OnKeyDown;
                }
            }
        }
    }

    public event KeyEventHandler? KeyUp
    {
        add
        {
            lock(this.@lock)
            {
                this.AddEvent(EventProperty.KeyUp, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.KeyUp += this.OnKeyUp;
                }
            }
        }
        remove
        {
            lock(this.@lock)
            {
                this.RemoveEvent(EventProperty.KeyUp, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.KeyUp -= this.OnKeyUp;
                }
            }
        }
    }

    public event MouseEventHandler? Scrolled
    {
        add
        {
            lock(this.@lock)
            {
                this.AddEvent(EventProperty.Scrolled, value, out var added);

                if (this.Tree is RenderTree renderTree && added)
                {
                    renderTree.Window.MouseWheel += this.OnScroll;
                }
            }
        }
        remove
        {
            lock(this.@lock)
            {
                this.RemoveEvent(EventProperty.Scrolled, value, out var removed);

                if (this.Tree is RenderTree renderTree && removed)
                {
                    renderTree.Window.MouseWheel -= this.OnScroll;
                }
            }
        }
    }
    #endregion events

    private readonly Lock                               @lock  = new();
    private readonly KeyedList<EventProperty, Delegate> events = [];

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
    private MouseEventHandler?   MouseMovedEvent    => this.GetEvent<MouseEventHandler>(EventProperty.MouseMoved);
    private MouseEventHandler?   MouseOutEvent      => this.GetEvent<MouseEventHandler>(EventProperty.MouseOut);
    private MouseEventHandler?   MouseOverEvent     => this.GetEvent<MouseEventHandler>(EventProperty.MouseOver);
    private MouseEventHandler?   MouseUpEvent       => this.GetEvent<MouseEventHandler>(EventProperty.MouseUp);
    private MouseEventHandler?   ScrolledEvent      => this.GetEvent<MouseEventHandler>(EventProperty.Scrolled);

    protected bool IsFocusable  { get; set; }
    protected bool IsScrollable { get; set; }

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }

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

    private void AddEvent(EventProperty key, Delegate? handler) =>
        this.AddEvent(key, handler, out _);

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

    private T? GetEvent<T>(EventProperty key) where T : Delegate =>
        this.events.TryGet(key, out var @delegate) ? (T)@delegate : null;

    private MouseEvent CreateEvent(in PlatformMouseEvent mouseEvent, bool indirect) =>
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

    private ContextEvent CreateEvent(in PlatformContextEvent platformContextEvent) =>
        new()
        {
            Target  = this,
            X       = platformContextEvent.X,
            Y       = platformContextEvent.Y,
            ScreenX = platformContextEvent.ScreenX,
            ScreenY = platformContextEvent.ScreenY,
        };

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

    private void OnScroll(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsScrollable)
        {
            var mouseEvent = this.CreateEvent(platformMouseEvent, false);

            this.ScrolledEvent?.Invoke(mouseEvent);
        }
    }

    protected override void OnConnected(RenderTree renderTree)
    {
        base.OnConnected(renderTree);

        if (this.events.ContainsKey(EventProperty.Input))
        {
            renderTree.Window.Input += this.OnInput;
        }

        if (this.events.ContainsKey(EventProperty.KeyDown))
        {
            renderTree.Window.KeyDown += this.OnKeyDown;
        }

        if (this.events.ContainsKey(EventProperty.KeyUp))
        {
            renderTree.Window.KeyUp += this.OnKeyUp;
        }

        if (this.events.ContainsKey(EventProperty.Scrolled))
        {
            renderTree.Window.MouseWheel += this.OnScroll;
        }
    }

    protected override void OnDisconnected(RenderTree renderTree)
    {
        base.OnDisconnected(renderTree);

        renderTree.Window.Input      -= this.OnInput;
        renderTree.Window.KeyDown    -= this.OnKeyDown;
        renderTree.Window.KeyUp      -= this.OnKeyUp;
        renderTree.Window.MouseWheel -= this.OnScroll;
    }

    internal void InvokeActivate()
    {
        this.OnStateChangedAdded(ElementState.Active);
        this.ActivatedEvent?.Invoke();
    }

    internal void InvokeBlur(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.OnStateChangedRemoved(ElementState.Focus);
            this.IsFocused = false;
            this.BluredEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.ClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeContext(in PlatformContextEvent platformContextEvent) =>
        this.ContextEvent?.Invoke(this.CreateEvent(platformContextEvent));

    internal void InvokeDeactivate()
    {
        this.OnStateChangedRemoved(ElementState.Active);
        this.DeactivatedEvent?.Invoke();
    }

    internal void InvokeDoubleClick(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.DoubleClickedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeFocus(in PlatformMouseEvent platformMouseEvent)
    {
        if (this.IsFocusable)
        {
            this.IsFocused = true;
            this.OnStateChangedAdded(ElementState.Focus);
            this.FocusedEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
        }
    }

    internal void InvokeMouseDown(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseDownEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseMoved(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseMovedEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseUp(in PlatformMouseEvent platformMouseEvent, bool indirect) =>
        this.MouseUpEvent?.Invoke(this.CreateEvent(platformMouseEvent, indirect));

    internal void InvokeMouseOut(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = false;

        if (!Layout.IsSelectingText)
        {
            this.OnStateChangedRemoved(ElementState.Hovered);
        }

        this.MouseOutEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    internal void InvokeMouseOver(in PlatformMouseEvent platformMouseEvent)
    {
        this.IsHovered = true;

        if (!Layout.IsSelectingText)
        {
            this.OnStateChangedAdded(ElementState.Hovered);
            // this.Layout.HandleTargetMouseOver();
        }

        this.MouseOverEvent?.Invoke(this.CreateEvent(platformMouseEvent, false));
    }

    public void Blur()
    {
        this.IsFocused = false;
        this.OnStateChangedRemoved(ElementState.Focus);
        this.BluredEvent?.Invoke(new() { Target = this });
    }

    public void Click()
    {
        this.OnStateChangedAdded(ElementState.Active);
        this.ClickedEvent?.Invoke(new() { Target = this });
    }

    public void Focus()
    {
        this.OnStateChangedAdded(ElementState.Focus);
        this.IsFocused = true;
        this.FocusedEvent?.Invoke(new() { Target = this });
    }

    protected abstract void OnStateChangedAdded(ElementState state);
    protected abstract void OnStateChangedRemoved(ElementState state);
}
