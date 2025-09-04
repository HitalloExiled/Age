using Age.Platforms.Display;

namespace Age.Elements;

public abstract partial class Element
{
    internal void HandleVirtualChildMouseDown(in WindowMouseEvent windowMouseEvent, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollBarMouseDown(windowMouseEvent, (LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseMoved(in WindowMouseEvent windowMouseEvent, uint _2)
    {
        if (this.CanScroll)
        {
            this.HandleScrollBarMouseMoved(windowMouseEvent);
        }
    }

    internal void HandleVirtualChildMouseOut(in WindowMouseEvent windowMouseEvent, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollBarMouseOut(windowMouseEvent, (LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseOver(in WindowMouseEvent _, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollBarMouseOver((LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseRelease(in WindowMouseEvent _, uint virtualChildIndex)
    {
        if (this.CanScroll)
        {
            this.HandleScrollBarMouseRelease((LayoutCommand)virtualChildIndex);
        }
    }

    internal void HandleVirtualChildMouseUp(in WindowMouseEvent _1, uint _2)
    {
        // Just for symmetry
    }
}
