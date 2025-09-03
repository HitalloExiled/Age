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
    private const uint SCROLL_DEFAULT_BORDER_RADIUS = 3;
    private const uint SCROLL_DEFAULT_MARGIN        = 9;
    private const uint SCROLL_DEFAULT_SIZE          = 6;
    private const uint SCROLL_HOVER_BORDER_RADIUS   = 6;
    private const uint SCROLL_HOVER_MARGIN          = 6;
    private const uint SCROLL_HOVER_SIZE            = 12;
    private const uint SCROLL_MARGIN                = 6;

    private static readonly Color scrollActiveColor  = (Color.White * 0.8f).WithAlpha(1);
    private static readonly Color scrollHoverColor   = (Color.White * 0.6f).WithAlpha(1);
    private static readonly Color scrollDefaultColor = (Color.White * 0.4f).WithAlpha(0.75f);

    private bool IsScrollVisible => this.HasAnyLayoutCommand(LayoutCommand.ScrollX | LayoutCommand.ScrollY);

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

                if (this.IsScrollVisible)
                {
                    if (this.CanScrollX)
                    {
                        this.UpdateScrollXControl();
                    }

                    if (this.CanScrollY)
                    {
                        this.UpdateScrollYControl();
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

    private void DrawScrollControls()
    {
        var canDrawScrollX = this.CanScrollX && this.size.Width < this.content.Width;
        var canDrawScrollY = this.CanScrollY && this.size.Height < this.content.Height;

        if (canDrawScrollX)
        {
            this.DrawScrollXControl();
        }

        if (canDrawScrollY)
        {
            this.DrawScrollYControl();
        }

        if (canDrawScrollX || canDrawScrollY)
        {
            this.RequestUpdate(false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollXControl()
    {
        var offset         = this.CanScrollY ? SCROLL_HOVER_SIZE : 0;
        var scale          = 1 - ((float)this.Boundings.Width / this.content.Width);
        var scrollBarWidth = this.Boundings.Width - this.border.Horizontal - offset - (SCROLL_MARGIN * 2);

        var scrollCommand = this.AllocateLayoutCommandScrollX();

        scrollCommand.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);
        scrollCommand.Color     = scrollDefaultColor;
        scrollCommand.Flags     = Flags.ColorAsBackground;
        scrollCommand.Metadata  = scrollBarWidth;
        scrollCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollX);
        scrollCommand.Size      = new(float.Max(scrollBarWidth * scale, SCROLL_DEFAULT_SIZE * 2), SCROLL_DEFAULT_SIZE);
        scrollCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_MARGIN, this.GetScrollXPositionY());

        this.UpdateScrollXControl(scrollCommand);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawScrollYControl()
    {
        var offset          = this.CanScrollX ? SCROLL_HOVER_SIZE : 0;
        var scrollBarHeight = this.Boundings.Height - this.border.Vertical - offset - (SCROLL_MARGIN * 2);
        var scale           = 1 - ((float)this.Boundings.Height / this.content.Height);

        var scrollCommand = this.AllocateLayoutCommandScrollY();

        scrollCommand.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);
        scrollCommand.Color     = scrollDefaultColor;
        scrollCommand.Flags     = Flags.ColorAsBackground;
        scrollCommand.Metadata  = scrollBarHeight;
        scrollCommand.ObjectId  = CombineIds(this.Index + 1, (int)LayoutCommand.ScrollY);
        scrollCommand.Size      = new(SCROLL_DEFAULT_SIZE, float.Max(scrollBarHeight * scale, SCROLL_DEFAULT_SIZE * 2));
        scrollCommand.Transform = Transform2D.CreateTranslated(this.GetScrollYPositionX(), -(this.border.Top + SCROLL_MARGIN));

        this.UpdateScrollYControl(scrollCommand);
    }

    private void DestroyScrollControls()
    {
        if (this.layoutCommands.HasAnyFlag(LayoutCommand.ScrollX | LayoutCommand.ScrollY))
        {
            this.ReleaseLayoutCommandScrollX();
            this.ReleaseLayoutCommandScrollY();
            this.RequestUpdate(false);
        }
    }

    private float GetScrollXPositionY() =>
        -(this.Boundings.Height - this.border.Top  - SCROLL_DEFAULT_SIZE - SCROLL_DEFAULT_MARGIN);

    private float GetScrollYPositionX() =>
        this.Boundings.Width - this.border.Left - SCROLL_DEFAULT_SIZE - SCROLL_DEFAULT_MARGIN;

    private void HandleMouseWheel(in WindowMouseEvent mouseEvent)
    {
        if (IsScrolling)
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

    private void HandleScrollMouseDown(in WindowMouseEvent windowMouseEvent, LayoutCommand layoutCommand)
    {
        if (windowMouseEvent.IsHoldingPrimaryButton && layoutCommand is LayoutCommand.ScrollX or LayoutCommand.ScrollY)
        {
            if (layoutCommand == LayoutCommand.ScrollX)
            {
                IsScrollingX = true;

                this.SetScrollXActiveStyle();
            }

            if (layoutCommand == LayoutCommand.ScrollY)
            {
                IsScrollingY = true;

                this.SetScrollYActiveStyle();
            }

            ActiveScrollTarget = this;
        }
    }

    private void HandleScrollMouseEnter()
    {
        if (ActiveScrollTarget == this)
        {
            this.RenderTree!.MouseMoved -= this.OnMouseMoved;
            this.HandleScrollMouseMoved();
        }
        else if (this.CanScroll)
        {
            this.DrawScrollControls();
        }
        else
        {
            for (var element = this; element != null; element = element.ComposedParentElement)
            {
                if (element.CanScroll)
                {
                    element.DrawScrollXControl();

                    break;
                }
            }
        }
    }

    private void HandleScrollMouseLeave()
    {
        if (!IsScrolling || ActiveScrollTarget != this)
        {
            this.DestroyScrollControls();
        }
    }

    private void OnMouseMoved(in WindowMouseEvent mouseEvent, Node? node)
    {
        if (mouseEvent.IsHoldingPrimaryButton)
        {
            this.HandleScrollMouseMoved();
        }
        else
        {
            IsScrollingX = IsScrollingY = false;

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
                        current.DestroyScrollControls();
                    }
                }
            }

            this.RenderTree!.MouseMoved -= this.OnMouseMoved;
        }
    }

    private void HandleScrollMouseOver(LayoutCommand layoutCommand)
    {
        if (IsScrolling)
        {
            return;
        }

        this.SetCursor(Cursor.Arrow);

        if (!IsHoveringScrollX && layoutCommand == LayoutCommand.ScrollX)
        {
            IsHoveringScrollX = true;

            this.SetScrollXHoverStyle();
        }
        else if (!IsHoveringScrollY && layoutCommand == LayoutCommand.ScrollY)
        {
            IsHoveringScrollY = true;

            this.SetScrollYHoverStyle();
        }
    }

    private void HandleScrollMouseOut(LayoutCommand _)
    {
        if (IsScrolling)
        {
            IsHoveringScrollX = false;
            IsHoveringScrollY = false;

            this.RenderTree!.MouseMoved += this.OnMouseMoved;
            this.HandleScrollMouseMoved();

            return;
        }

        this.SetCursor(Cursor.Arrow);

        if (IsHoveringScrollX)
        {
            IsHoveringScrollX = false;

            this.SetScrollXDefaultStyle();
        }
        else if (IsHoveringScrollY)
        {
            IsHoveringScrollY = false;

            this.SetScrollYDefaultStyle();
        }
    }

    private void HandleScrollMouseMoved()
    {
        if (ActiveScrollTarget != this || !this.IsScrollVisible)
        {
            return;
        }

        var globalDelta = AgeInput.GetMouseDeltaPosition();
        var delta       = (this.Transform.Matrix.ExtractRotation() * new Vector2<float>(globalDelta.X, globalDelta.Y)).ToPoint();

        if (IsScrollingX)
        {
            this.UpdateScrollXOffset(delta);
        }
        else if (IsScrollingY)
        {
            this.UpdateScrollYOffset(delta);
        }
    }

    private void HandleScrollMouseRelease(in WindowMouseEvent windowMouseEvent, LayoutCommand layoutCommand)
    {
        if (!IsHoveringScroll)
        {
            this.SetCursor(this.ComputedStyle.Cursor);
            this.SetScrollXDefaultStyle();
            this.SetScrollYDefaultStyle();
        }
        else if (IsHoveringScrollX)
        {
            if (layoutCommand == LayoutCommand.ScrollX)
            {
                IsHoveringScrollX = true;

                this.SetScrollXHoverStyle();
            }
            else
            {
                this.SetScrollXDefaultStyle();
            }
        }
        else if (IsHoveringScrollY)
        {
            if (layoutCommand == LayoutCommand.ScrollY)
            {
                IsHoveringScrollY = true;

                this.SetScrollYHoverStyle();
            }
            else
            {
                this.SetScrollYDefaultStyle();
            }
        }

        IsScrollingX = IsScrollingY = false;
        ActiveScrollTarget = null;
    }

    private void SetScrollXActiveStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color = scrollActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollXDefaultStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color     = scrollDefaultColor;
        commandScrollX.Size      = commandScrollX.Size with { Height = SCROLL_DEFAULT_SIZE };
        commandScrollX.Transform = Transform2D.CreateTranslated(commandScrollX.Transform.Position.X, this.GetScrollXPositionY());
        commandScrollX.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollXHoverStyle()
    {
        var commandScrollX = this.GetLayoutCommandScrollX();

        commandScrollX.Color     = scrollHoverColor;
        commandScrollX.Size      = commandScrollX.Size with { Height = SCROLL_HOVER_SIZE };
        commandScrollX.Transform = Transform2D.CreateTranslated(commandScrollX.Transform.Position.X, -(this.Boundings.Height - this.border.Top - SCROLL_HOVER_SIZE - SCROLL_HOVER_MARGIN));
        commandScrollX.Border    = new(0, SCROLL_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollYActiveStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color = scrollActiveColor;

        this.RequestUpdate(false);
    }

    private void SetScrollYDefaultStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color     = scrollDefaultColor;
        commandScrollY.Size      = commandScrollY.Size with { Width = SCROLL_DEFAULT_SIZE };
        commandScrollY.Transform = Transform2D.CreateTranslated(this.GetScrollYPositionX(), commandScrollY.Transform.Position.Y);
        commandScrollY.Border    = new(0, SCROLL_DEFAULT_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void SetScrollYHoverStyle()
    {
        var commandScrollY = this.GetLayoutCommandScrollY();

        commandScrollY.Color     = scrollHoverColor;
        commandScrollY.Size      = commandScrollY.Size with { Width = SCROLL_HOVER_SIZE };
        commandScrollY.Transform = Transform2D.CreateTranslated(this.Boundings.Width - this.border.Left - SCROLL_HOVER_SIZE - SCROLL_HOVER_MARGIN, commandScrollY.Transform.Position.Y);
        commandScrollY.Border    = new(0, SCROLL_HOVER_BORDER_RADIUS, default);

        this.RequestUpdate(false);
    }

    private void UpdateScrollXOffset(in Point<float> delta)
    {
        var scrollCommand = this.GetLayoutCommandScrollX();
        var controlOffset = this.border.Left + SCROLL_MARGIN;
        var localPosition = scrollCommand.Transform.Position.X - controlOffset;
        var position      = float.Clamp(localPosition + delta.X, 0, scrollCommand.Metadata - scrollCommand.Size.Width);
        var ratio         = position / (scrollCommand.Metadata - scrollCommand.Size.Width);
        var offset        = this.content.Width.ClampSubtract(this.size.Width);

        scrollCommand.Transform = Transform2D.CreateTranslated(position + controlOffset, scrollCommand.Transform.Position.Y);

        this.contentOffset.X = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollXControl(RectCommand scrollCommand)
    {
        var ratio  = (float)this.contentOffset.X / this.content.Width.ClampSubtract(this.size.Width);
        var offset = scrollCommand.Metadata - scrollCommand.Size.Width;

        scrollCommand.Transform = Transform2D.CreateTranslated(this.border.Left + SCROLL_MARGIN + (offset * ratio), scrollCommand.Transform.Position.Y);
    }

    private void UpdateScrollXControl() =>
        this.UpdateScrollXControl(this.GetLayoutCommandScrollX());

    private void UpdateScrollYOffset(in Point<float> delta)
    {
        var scrollCommand = this.GetLayoutCommandScrollY();
        var controlOffset = this.border.Top                     + SCROLL_MARGIN;
        var localPosition = -scrollCommand.Transform.Position.Y - controlOffset;
        var position      = float.Clamp(localPosition           + delta.Y, 0, scrollCommand.Metadata - scrollCommand.Size.Height);
        var ratio         = position                            / (scrollCommand.Metadata            - scrollCommand.Size.Height);
        var offset        = this.content.Height.ClampSubtract(this.size.Height);

        scrollCommand.Transform = Transform2D.CreateTranslated(scrollCommand.Transform.Position.X, -(position + controlOffset));

        this.contentOffset.Y = (uint)float.Round(offset * ratio);

        this.RequestUpdate(false);
    }

    private void UpdateScrollYControl(RectCommand scrollCommand)
    {
        var ratio  = (float)this.contentOffset.Y / this.content.Height.ClampSubtract(this.size.Height);
        var offset = scrollCommand.Metadata - scrollCommand.Size.Height;

        scrollCommand.Transform = Transform2D.CreateTranslated(scrollCommand.Transform.Position.X, -(this.border.Top + SCROLL_MARGIN + (offset * ratio)));
    }

    private void UpdateScrollYControl() =>
        this.UpdateScrollYControl(this.GetLayoutCommandScrollY());

    internal static bool IsScrollControl(uint virtualChildIndex) =>
        (LayoutCommand)virtualChildIndex is LayoutCommand.ScrollX or LayoutCommand.ScrollY;

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
