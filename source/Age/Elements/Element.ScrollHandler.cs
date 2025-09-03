using System;
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

    private bool IsScrollBarVisible => this.HasAnyLayoutCommand(LayoutCommand.ScrollBarX | LayoutCommand.ScrollBarY);

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

                if (this.IsScrollBarVisible)
                {
                    if (this.CanScrollX)
                    {
                        this.UpdateScrollBarXControl();
                    }

                    if (this.CanScrollY)
                    {
                        this.UpdateScrollBarYControl();
                    }
                }

                this.ownStencilLayer?.MakeChildrenDirty();
                this.RequestUpdate(false);
            }
        }
    }

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

        var scrollBarCommand = this.AllocateLayoutCommandScrollBarX();

        scrollBarCommand.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);
        scrollBarCommand.Color     = scrollBarDefaultColor;
        scrollBarCommand.Flags     = Flags.ColorAsBackground;
        scrollBarCommand.Metadata  = scrollBarWidth;
        scrollBarCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarX);
        scrollBarCommand.Size      = new(float.Max(scrollBarWidth * scale, SCROLL_BAR_DEFAULT_SIZE * 2), SCROLL_BAR_DEFAULT_SIZE);
        scrollBarCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_BAR_MARGIN, this.GetScrollBarXPositionY());

        this.UpdateScrollBarXControl(scrollBarCommand);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollBarYControl()
    {
        var offset          = this.CanScrollX ? SCROLL_BAR_HOVER_SIZE : 0;
        var scrollBarHeight = this.Boundings.Height - this.border.Vertical - offset - (SCROLL_BAR_MARGIN * 2);
        var scale           = 1 - ((float)this.Boundings.Height / this.content.Height);

        var scrollBarCommand = this.AllocateLayoutCommandScrollBarY();

        scrollBarCommand.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);
        scrollBarCommand.Color     = scrollBarDefaultColor;
        scrollBarCommand.Flags     = Flags.ColorAsBackground;
        scrollBarCommand.Metadata  = scrollBarHeight;
        scrollBarCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollBarY);
        scrollBarCommand.Size      = new(SCROLL_BAR_DEFAULT_SIZE, float.Max(scrollBarHeight * scale, SCROLL_BAR_DEFAULT_SIZE * 2));
        scrollBarCommand.Transform = Transform2D.CreateTranslated(this.GetScrollBarYPositionX(), -(this.border.Top + SCROLL_BAR_MARGIN));

        this.UpdateScrollBarYControl(scrollBarCommand);
    }

    private void DestroyScrollBarControls()
    {
        if (this.layoutCommands.HasAnyFlag(LayoutCommand.ScrollBarX | LayoutCommand.ScrollBarY))
        {
            this.ReleaseLayoutCommandScrollBarX();
            this.ReleaseLayoutCommandScrollBarY();
            this.RequestUpdate(false);
        }
    }

    private float GetScrollBarXPositionY() =>
        -(this.Boundings.Height - this.border.Top  - SCROLL_BAR_DEFAULT_SIZE - SCROLL_BAR_DEFAULT_MARGIN);

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
            if (layoutCommand == LayoutCommand.ScrollBarX)
            {
                IsDraggingScrollBarX = true;

                this.SetScrollBarXActiveStyle();
            }

            if (layoutCommand == LayoutCommand.ScrollBarY)
            {
                IsDraggingScrollBarY = true;

                this.SetScrollBarYActiveStyle();
            }

            ActiveScrollBarTarget = this;
        }
    }

    private void HandleScrollBarMouseEnter()
    {
        if (ActiveScrollBarTarget == this)
        {
            this.RenderTree!.MouseMoved -= this.OnMouseMoved;
            this.HandleScrollBarMouseMoved();
        }
        else if (this.CanScroll)
        {
            this.DrawScrollBarControls();
        }
        else
        {
            for (var element = this; element != null; element = element.ComposedParentElement)
            {
                if (element.CanScroll)
                {
                    element.DrawScrollBarXControl();

                    break;
                }
            }
        }
    }

    private void HandleScrollBarMouseLeave()
    {
        if (!IsDraggingScrollBar || ActiveScrollBarTarget != this)
        {
            this.DestroyScrollBarControls();
        }
    }

    private void OnMouseMoved(in WindowMouseEvent mouseEvent, Node? node)
    {
        if (mouseEvent.IsHoldingPrimaryButton)
        {
            this.HandleScrollBarMouseMoved();
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

            this.RenderTree!.MouseMoved -= this.OnMouseMoved;
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

    private void HandleScrollBarMouseOut(LayoutCommand _)
    {
        if (IsDraggingScrollBar)
        {
            IsHoveringScrollBarX = false;
            IsHoveringScrollBarY = false;

            this.RenderTree!.MouseMoved += this.OnMouseMoved;
            this.HandleScrollBarMouseMoved();

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

    private void HandleScrollBarMouseMoved()
    {
        if (ActiveScrollBarTarget != this || !this.IsScrollBarVisible)
        {
            return;
        }

        var globalDelta = AgeInput.GetMouseDeltaPosition();
        var delta       = (this.Transform.Matrix.ExtractRotation() * new Vector2<float>(globalDelta.X, globalDelta.Y)).ToPoint();

        if (IsDraggingScrollBarX)
        {
            this.UpdateScrollBarXOffset(delta);
        }
        else if (IsDraggingScrollBarY)
        {
            this.UpdateScrollBarYOffset(delta);
        }
    }

    private void HandleScrollBarMouseRelease(LayoutCommand layoutCommand)
    {
        if (!IsHoveringScrollBar)
        {
            this.ApplyCursor();
            this.SetScrollBarXDefaultStyle();
            this.SetScrollBarYDefaultStyle();
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

    private void SetScrollBarXActiveStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarX();

        commandScrollBar.Color = scrollBarActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollBarXDefaultStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarX();

        commandScrollBar.Color     = scrollBarDefaultColor;
        commandScrollBar.Size      = commandScrollBar.Size with { Height = SCROLL_BAR_DEFAULT_SIZE };
        commandScrollBar.Transform = Transform2D.CreateTranslated(commandScrollBar.Transform.Position.X, this.GetScrollBarXPositionY());
        commandScrollBar.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarXHoverStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarX();

        commandScrollBar.Color     = scrollBarHoverColor;
        commandScrollBar.Size      = commandScrollBar.Size with { Height = SCROLL_BAR_HOVER_SIZE };
        commandScrollBar.Transform = Transform2D.CreateTranslated(commandScrollBar.Transform.Position.X, -(this.Boundings.Height - this.border.Top - SCROLL_BAR_HOVER_SIZE - SCROLL_BAR_HOVER_MARGIN));
        commandScrollBar.Border    = new(0, SCROLL_BAR_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarYActiveStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarY();

        commandScrollBar.Color = scrollBarActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollBarYDefaultStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarY();

        commandScrollBar.Color     = scrollBarDefaultColor;
        commandScrollBar.Size      = commandScrollBar.Size with { Width = SCROLL_BAR_DEFAULT_SIZE };
        commandScrollBar.Transform = Transform2D.CreateTranslated(this.GetScrollBarYPositionX(), commandScrollBar.Transform.Position.Y);
        commandScrollBar.Border    = new(0, SCROLL_BAR_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollBarYHoverStyle()
    {
        var commandScrollBar = this.GetLayoutCommandScrollBarY();

        commandScrollBar.Color     = scrollBarHoverColor;
        commandScrollBar.Size      = commandScrollBar.Size with { Width = SCROLL_BAR_HOVER_SIZE };
        commandScrollBar.Transform = Transform2D.CreateTranslated(this.Boundings.Width - this.border.Left - SCROLL_BAR_HOVER_SIZE - SCROLL_BAR_HOVER_MARGIN, commandScrollBar.Transform.Position.Y);
        commandScrollBar.Border    = new(0, SCROLL_BAR_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarXOffset(in Point<float> delta)
    {
        var scrollBarCommand = this.GetLayoutCommandScrollBarX();
        var controlOffset    = this.border.Left + SCROLL_BAR_MARGIN;
        var localPosition    = scrollBarCommand.Transform.Position.X - controlOffset;
        var position         = float.Clamp(localPosition + delta.X, 0, scrollBarCommand.Metadata - scrollBarCommand.Size.Width);
        var ratio            = position / (scrollBarCommand.Metadata - scrollBarCommand.Size.Width);
        var offset           = this.content.Width.ClampSubtract(this.size.Width);

        scrollBarCommand.Transform = Transform2D.CreateTranslated(position + controlOffset, scrollBarCommand.Transform.Position.Y);

        this.contentOffset.X = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarXControl(RectCommand scrollBarCommand)
    {
        var ratio  = (float)this.contentOffset.X / this.content.Width.ClampSubtract(this.size.Width);
        var offset = scrollBarCommand.Metadata - scrollBarCommand.Size.Width;

        scrollBarCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_BAR_MARGIN + (offset * ratio), scrollBarCommand.Transform.Position.Y);
    }

    private void UpdateScrollBarXControl() =>
        this.UpdateScrollBarXControl(this.GetLayoutCommandScrollBarX());

    private void UpdateScrollBarYOffset(in Point<float> delta)
    {
        var scrollBarCommand = this.GetLayoutCommandScrollBarY();
        var controlOffset    = this.border.Top                     + SCROLL_BAR_MARGIN;
        var localPosition    = -scrollBarCommand.Transform.Position.Y - controlOffset;
        var position         = float.Clamp(localPosition           + delta.Y, 0, scrollBarCommand.Metadata - scrollBarCommand.Size.Height);
        var ratio            = position                            / (scrollBarCommand.Metadata            - scrollBarCommand.Size.Height);
        var offset           = this.content.Height.ClampSubtract(this.size.Height);

        scrollBarCommand.Transform = Transform2D.CreateTranslated(scrollBarCommand.Transform.Position.X, -(position + controlOffset));

        this.contentOffset.Y = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollBarYControl(RectCommand scrollBarCommand)
    {
        var ratio  = (float)this.contentOffset.Y / this.content.Height.ClampSubtract(this.size.Height);
        var offset = scrollBarCommand.Metadata - scrollBarCommand.Size.Height;

        scrollBarCommand.Transform = Transform2D.CreateTranslated(scrollBarCommand.Transform.Position.X, -(this.border.Top + SCROLL_BAR_MARGIN + (offset * ratio)));
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
