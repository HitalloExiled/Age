using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Scene;
using Age.Styling;
using static Age.Shaders.CanvasShader;

using AgeInput = Age.Input;

namespace Age.Elements;

public abstract partial class Element
{
    private const uint SCROLL_BAR_DEFAULT_BORDER_RADIUS = 3;
    private const uint SCROLL_BAR_DEFAULT_MARGIN        = 9;
    private const uint SCROLL_BAR_DEFAULT_SIZE          = 6;
    private const uint SCROLL_BAR_HOVER_BORDER_RADIUS   = 6;
    private const uint SCROLL_BAR_HOVER_MARGIN          = 6;
    private const uint SCROLL_BAR_HOVER_SIZE            = 12;
    private const uint SCROLL_BAR_MARGIN                = 6;

    private static readonly Color scrollBarActiveColor  = (Color.White * 0.8f).WithAlpha(1);
    private static readonly Color scrollBarHoverColor   = (Color.White * 0.6f).WithAlpha(1);
    private static readonly Color scrollBarDefaultColor = (Color.White * 0.4f).WithAlpha(0.75f);

    private bool IsScrollBarVisible  => this.HasAnyLayoutCommand(LayoutCommand.ScrollBarX | LayoutCommand.ScrollBarY);
    private bool IsScrollBarXVisible => this.HasAnyLayoutCommand(LayoutCommand.ScrollBarX);
    private bool IsScrollBarYVisible => this.HasAnyLayoutCommand(LayoutCommand.ScrollBarY);

    internal bool CanScroll  => this.CanScrollX || this.CanScrollY;
    internal bool CanScrollX => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollX) == true;
    internal bool CanScrollY => this.ComputedStyle.Overflow?.HasFlags(Overflow.ScrollY) == true;

    public Point<uint> Scroll
    {
        get => this.contentOffset;
        set
        {
            value.X = uint.Clamp(value.X, 0, this.content.Width.ClampSubtract(this.size.Width));
            value.Y = uint.Clamp(value.Y, 0, this.content.Height.ClampSubtract(this.size.Height));

            if (this.contentOffset != value)
            {
                this.contentOffset = value;

                if (this.CanScrollX && IsScrollBarXVisible)
                {
                    this.UpdateScrollBarXControl();
                }

                if (this.CanScrollY && IsScrollBarYVisible)
                {
                    this.UpdateScrollBarYControl();
                }

                this.ownStencilLayer?.MakeChildrenDirty();
                this.RequestUpdate(false);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateScrollBarClickPosition(in Vector2<float> position, RectCommand command) =>
        ScrollBarClickPosition = (position - command.Transform.Position).ToPoint();

    private void ApplyScroll(in WindowMouseEvent mouseEvent)
    {
        if (this.CanScrollX && mouseEvent.KeyStates.HasFlags(MouseKeyStates.Shift))
        {
            this.Scroll = this.Scroll with
            {
                X = (uint)(this.contentOffset.X + (10 * -mouseEvent.Delta))
            };
        }
        else if (this.CanScrollY)
        {
            this.Scroll = this.Scroll with
            {
                Y = (uint)(this.Scroll.Y + (10 * -mouseEvent.Delta))
            };
        }
    }

    private void DrawScrollBarControls()
    {
        var canDrawScrollBarX = this.CanScrollX && this.size.Width < this.content.Width;
        var canDrawScrollBarY = this.CanScrollY && this.size.Height < this.content.Height;

        if (canDrawScrollBarX)
        {
            this.DrawScrollBarXControl();
        }

        if (canDrawScrollBarY)
        {
            this.DrawScrollBarYControl();
        }

        if (canDrawScrollBarX || canDrawScrollBarY)
        {
            this.RequestUpdate(false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollBarXControl()
    {
        var offset         = this.CanScrollY ? SCROLL_BAR_HOVER_SIZE : 0;
        var scale          = 1 - ((float)this.Boundings.Width / this.content.Width);
        var scrollBarWidth = this.Boundings.Width - this.border.Horizontal - offset - (SCROLL_BAR_MARGIN * 2);

        var command = this.AllocateLayoutCommandScrollBarX();

        command.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);
        command.Color     = scrollBarDefaultColor;
        command.Flags     = Flags.ColorAsBackground;
        command.Metadata  = scrollBarWidth;
        command.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarX);
        command.Size      = new(float.Max(scrollBarWidth * scale, SCROLL_BAR_DEFAULT_SIZE * 2), SCROLL_BAR_DEFAULT_SIZE);
        command.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_BAR_MARGIN, this.GetScrollBarXPositionY());

        this.UpdateScrollBarXControl(command);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollBarYControl()
    {
        var offset          = this.CanScrollX ? SCROLL_BAR_HOVER_SIZE : 0;
        var scrollBarHeight = this.Boundings.Height - this.border.Vertical - offset - (SCROLL_BAR_MARGIN * 2);
        var scale           = 1 - ((float)this.Boundings.Height / this.content.Height);

        var command = this.AllocateLayoutCommandScrollBarY();

        command.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);
        command.Color     = scrollBarDefaultColor;
        command.Flags     = Flags.ColorAsBackground;
        command.Metadata  = scrollBarHeight;
        command.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarY);
        command.Size      = new(SCROLL_BAR_DEFAULT_SIZE, float.Max(scrollBarHeight * scale, SCROLL_BAR_DEFAULT_SIZE * 2));
        command.Transform = Transform2D.CreateTranslated(this.GetScrollBarYPositionX(), -(this.border.Top + SCROLL_BAR_MARGIN));

        this.UpdateScrollBarYControl(command);
    }

    private void DestroyScrollBarControls()
    {
        this.ReleaseLayoutCommandScrollBarX();
        this.ReleaseLayoutCommandScrollBarY();
        this.RequestUpdate(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2<float> GetCursorLocalPosition(in WindowMouseEvent windowMouseEvent)
    {
        var position = new Vector2<float>(windowMouseEvent.X, -windowMouseEvent.Y);

        return this.Transform.Matrix.Inverse() * position;
    }

    private float GetScrollBarXPositionY() =>
        -(this.Boundings.Height - this.border.Top - SCROLL_BAR_DEFAULT_SIZE - SCROLL_BAR_DEFAULT_MARGIN);

    private float GetScrollBarYPositionX() =>
        this.Boundings.Width - this.border.Left - SCROLL_BAR_DEFAULT_SIZE - SCROLL_BAR_DEFAULT_MARGIN;

    private void HandleMouseWheel(in WindowMouseEvent mouseEvent)
    {
        if (IsDraggingScrollBar)
        {
            return;
        }

        for (var element = this; element != null; element = element.ComposedParentElement)
        {
            if (element.IsScrollable)
            {
                element.ApplyScroll(mouseEvent);

                break;
            }
        }
    }

    private void HandleScrollBarMouseDown(in WindowMouseEvent windowMouseEvent, LayoutCommand layoutCommand)
    {
        if (windowMouseEvent.IsHoldingPrimaryButton && layoutCommand is LayoutCommand.ScrollBarX or LayoutCommand.ScrollBarY)
        {
            var localPosition = this.GetCursorLocalPosition(windowMouseEvent);

            if (layoutCommand == LayoutCommand.ScrollBarX)
            {
                this.SetScrollBarXActiveStyle();

                UpdateScrollBarClickPosition(localPosition, this.GetLayoutCommandScrollBarX());

                IsDraggingScrollBarX = true;
            }

            if (layoutCommand == LayoutCommand.ScrollBarY)
            {
                this.SetScrollBarYActiveStyle();

                UpdateScrollBarClickPosition(localPosition, this.GetLayoutCommandScrollBarY());

                IsDraggingScrollBarY = true;
            }

            ActiveScrollBarTarget = this;
        }
    }

    private void HandleScrollBarMouseEnter(in WindowMouseEvent windowMouseEvent)
    {
        if (ActiveScrollBarTarget == this)
        {
            this.RenderTree!.MouseMoved -= this.OnRenderTreeMouseMoved;
            this.HandleScrollBarMouseMoved(windowMouseEvent);
        }
        else if (this.CanScroll && !this.IsScrollBarVisible)
        {
            this.DrawScrollBarControls();
        }
    }

    private void HandleScrollBarMouseLeave()
    {
        if (!IsDraggingScrollBar || ActiveScrollBarTarget != this)
        {
            this.DestroyScrollBarControls();
        }
    }

    private void HandleScrollBarMouseOver(LayoutCommand layoutCommand)
    {
        if (IsDraggingScrollBar)
        {
            return;
        }

        this.SetCursor(Cursor.Arrow);

        if (!IsHoveringScrollBarX && layoutCommand == LayoutCommand.ScrollBarX)
        {
            IsHoveringScrollBarX = true;

            this.SetScrollBarXHoverStyle();
        }
        else if (!IsHoveringScrollBarY && layoutCommand == LayoutCommand.ScrollBarY)
        {
            IsHoveringScrollBarY = true;

            this.SetScrollBarYHoverStyle();
        }
    }

    private void HandleScrollBarMouseOut(in WindowMouseEvent windowMouseEvent, LayoutCommand _)
    {
        if (IsDraggingScrollBar)
        {
            IsHoveringScrollBarX = false;
            IsHoveringScrollBarY = false;

            this.RenderTree!.MouseMoved += this.OnRenderTreeMouseMoved;
            this.HandleScrollBarMouseMoved(windowMouseEvent);

            return;
        }

        this.SetCursor(Cursor.Arrow);

        if (IsHoveringScrollBarX)
        {
            IsHoveringScrollBarX = false;

            this.SetScrollBarXDefaultStyle();
        }
        else if (IsHoveringScrollBarY)
        {
            IsHoveringScrollBarY = false;

            this.SetScrollBarYDefaultStyle();
        }
    }

    private void HandleScrollBarMouseMoved(in WindowMouseEvent windowMouseEvent)
    {
        if (ActiveScrollBarTarget != this || !this.IsScrollBarVisible)
        {
            return;
        }

        var localPosition = this.GetCursorLocalPosition(windowMouseEvent);

        if (IsDraggingScrollBarX)
        {
            this.UpdateScrollBarXOffset(localPosition.X);
        }
        else if (IsDraggingScrollBarY)
        {
            this.UpdateScrollBarYOffset(localPosition.Y);
        }
    }

    private void HandleScrollBarMouseRelease(LayoutCommand layoutCommand)
    {
        if (!IsHoveringScrollBar)
        {
            this.ApplyCursor();

            if (IsDraggingScrollBarX)
            {
                this.SetScrollBarXDefaultStyle();
            }

            if (IsDraggingScrollBarY)
            {
                this.SetScrollBarYDefaultStyle();
            }
        }
        else if (IsHoveringScrollBarX)
        {
            if (layoutCommand == LayoutCommand.ScrollBarX)
            {
                IsHoveringScrollBarX = true;

                this.SetScrollBarXHoverStyle();
            }
            else
            {
                this.SetScrollBarXDefaultStyle();
            }
        }
        else if (IsHoveringScrollBarY)
        {
            if (layoutCommand == LayoutCommand.ScrollBarY)
            {
                IsHoveringScrollBarY = true;

                this.SetScrollBarYHoverStyle();
            }
            else
            {
                this.SetScrollBarYDefaultStyle();
            }
        }

        IsDraggingScrollBarX  = IsDraggingScrollBarY = false;
        ActiveScrollBarTarget = null;
    }

    private void OnRenderTreeMouseMoved(in WindowMouseEvent windowMouseEvent, Node? node)
    {
        if (windowMouseEvent.IsHoldingPrimaryButton)
        {
            this.HandleScrollBarMouseMoved(windowMouseEvent);
        }
        else
        {
            IsDraggingScrollBarX = IsDraggingScrollBarY = false;

            if (node is Element element)
            {
                for (var current = this; current != null; current = current.ComposedParentElement)
                {
                    if (element == current || element.IsComposedDescendent(current))
                    {
                        break;
                    }

                    if (current.IsScrollable)
                    {
                        current.DestroyScrollBarControls();
                    }
                }
            }

            this.RenderTree!.MouseMoved -= this.OnRenderTreeMouseMoved;
        }
    }

    private void RefreshScrollBarControls()
    {
        this.DestroyScrollBarControls();
        this.DrawScrollBarControls();

        if (ActiveScrollBarTarget == this)
        {
            if (IsDraggingScrollBarX = IsDraggingScrollBarX && this.IsScrollBarXVisible)
            {
                this.SetScrollBarXHoverStyle();
                this.SetScrollBarXActiveStyle();
            }

            if (IsDraggingScrollBarY = IsDraggingScrollBarY && this.IsScrollBarYVisible)
            {
                this.SetScrollBarYHoverStyle();
                this.SetScrollBarYActiveStyle();
            }
        }
    }

    private void SetScrollBarXActiveStyle()
    {
        var command = this.GetLayoutCommandScrollBarX();

        command.Color = scrollBarActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollBarXDefaultStyle()
    {
        Debug.Assert(this.IsScrollBarXVisible);

        var command = this.GetLayoutCommandScrollBarX();

        command.Color     = scrollBarDefaultColor;
        command.Size      = command.Size with { Height = SCROLL_BAR_DEFAULT_SIZE };
        command.Transform = Transform2D.CreateTranslated(command.Transform.Position.X, this.GetScrollBarXPositionY());
        command.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarXHoverStyle()
    {
        Debug.Assert(this.IsScrollBarXVisible);

        var command = this.GetLayoutCommandScrollBarX();

        command.Color     = scrollBarHoverColor;
        command.Size      = command.Size with { Height = SCROLL_BAR_HOVER_SIZE };
        command.Transform = Transform2D.CreateTranslated(command.Transform.Position.X, -(this.Boundings.Height - this.border.Top - SCROLL_BAR_HOVER_SIZE - SCROLL_BAR_HOVER_MARGIN));
        command.Border    = new(0, SCROLL_BAR_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarYActiveStyle()
    {
        Debug.Assert(this.IsScrollBarYVisible);

        var command = this.GetLayoutCommandScrollBarY();

        command.Color = scrollBarActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollBarYDefaultStyle()
    {
        Debug.Assert(this.IsScrollBarYVisible);

        var command = this.GetLayoutCommandScrollBarY();

        command.Color     = scrollBarDefaultColor;
        command.Size      = command.Size with { Width = SCROLL_BAR_DEFAULT_SIZE };
        command.Transform = Transform2D.CreateTranslated(this.GetScrollBarYPositionX(), command.Transform.Position.Y);
        command.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarYHoverStyle()
    {
        Debug.Assert(this.IsScrollBarYVisible);

        var command = this.GetLayoutCommandScrollBarY();

        command.Color     = scrollBarHoverColor;
        command.Size      = command.Size with { Width = SCROLL_BAR_HOVER_SIZE };
        command.Transform = Transform2D.CreateTranslated(this.Boundings.Width - this.border.Left - SCROLL_BAR_HOVER_SIZE - SCROLL_BAR_HOVER_MARGIN, command.Transform.Position.Y);
        command.Border    = new(0, SCROLL_BAR_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarXOffset(float localPosition)
    {
        Debug.Assert(this.IsScrollBarXVisible);

        var command = this.GetLayoutCommandScrollBarX();

        var start = this.border.Left + SCROLL_BAR_MARGIN;
        var end   = command.Metadata - command.Size.Width;

        var position = float.Round(float.Clamp(localPosition - ScrollBarClickPosition.X, start, start + end));

        command.Transform = Transform2D.CreateTranslated(position, command.Transform.Position.Y);

        var ratio  = (position - start) / end;
        var offset = this.content.Width.ClampSubtract(this.size.Width);

        this.contentOffset.X = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarXControl(RectCommand command)
    {
        Debug.Assert(this.IsScrollBarXVisible);

        var ratio  = (float)this.contentOffset.X / this.content.Width.ClampSubtract(this.size.Width);
        var offset = command.Metadata - command.Size.Width;

        command.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_BAR_MARGIN + (offset * ratio), command.Transform.Position.Y);
    }

    private void UpdateScrollBarXControl() =>
        this.UpdateScrollBarXControl(this.GetLayoutCommandScrollBarX());

    private void UpdateScrollBarYOffset(float localPosition)
    {
        Debug.Assert(this.IsScrollBarYVisible);

        var command = this.GetLayoutCommandScrollBarY();

        var start = this.border.Top + SCROLL_BAR_MARGIN;
        var end   = command.Metadata - command.Size.Height;

        var position = float.Clamp(-(localPosition - ScrollBarClickPosition.Y), start, start + end);

        command.Transform = Transform2D.CreateTranslated(command.Transform.Position.X, -position);

        var ratio  = (position - start) / end;
        var offset = this.content.Height.ClampSubtract(this.size.Height);

        this.contentOffset.Y = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarYControl(RectCommand command)
    {
        Debug.Assert(this.IsScrollBarYVisible);

        var ratio  = (float)this.contentOffset.Y / this.content.Height.ClampSubtract(this.size.Height);
        var offset = command.Metadata - command.Size.Height;

        command.Transform = Transform2D.CreateTranslated(command.Transform.Position.X, -(this.border.Top + SCROLL_BAR_MARGIN + (offset * ratio)));
    }

    private void UpdateScrollBarYControl() =>
        this.UpdateScrollBarYControl(this.GetLayoutCommandScrollBarY());

    internal static bool IsScrollBarControl(uint virtualChildIndex) =>
        (LayoutCommand)virtualChildIndex is LayoutCommand.ScrollBarX or LayoutCommand.ScrollBarY;

    public void ScrollTo(in Rect<int> boundings)
    {
        if (!this.CanScrollX || !this.CanScrollY)
        {
            return;
        }

        var boxModel = this.GetBoxModel();

        var boundsLeft   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
        var boundsRight  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
        var boundsTop    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
        var boundsBottom = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

        var scroll = this.Scroll;

        if (this.CanScrollX)
        {
            if (boundings.Left < boundsLeft)
            {
                var characterLeft = boundings.Left + scroll.X;

                scroll.X = (uint)(characterLeft - boundsLeft);
            }
            else if (boundings.Right > boundsRight)
            {
                var characterRight = boundings.Right + scroll.X;

                scroll.X = (uint)(characterRight - boundsRight);
            }
        }

        if (this.CanScrollY)
        {
            if (boundings.Top < boundsTop)
            {
                var characterTop = boundings.Top + scroll.Y;

                scroll.Y = (uint)(characterTop - boundsTop);
            }
            else if (boundings.Bottom > boundsBottom)
            {
                var characterBottom = boundings.Bottom + scroll.Y;

                scroll.Y = (uint)(characterBottom - boundsBottom);
            }
        }

        this.Scroll = scroll;
    }

    public void ScrollTo(Element element) =>
        this.ScrollTo(element.GetUpdatedBoundings());
}
